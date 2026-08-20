using Falcon.FSATS.Trading.Domain;

namespace Falcon.FSATS.Trading.Application;

public sealed class RestartAwareExecutionAdmission
{
    private readonly TradingRestartRecoverySession _session;

    public RestartAwareExecutionAdmission(TradingRestartRecoverySession session)
        => _session = session ?? throw new ArgumentNullException(nameof(session));

    public bool CanAdmit(QueuedExecutionWork work, TrustEpoch currentTrustedEpoch, bool riskIncreasing, out string reasonCode)
    {
        ArgumentNullException.ThrowIfNull(work);
        ArgumentNullException.ThrowIfNull(work.Intent);
        ArgumentNullException.ThrowIfNull(work.Intent.ExecutionIdentity);

        var identity = work.Intent.ExecutionIdentity;
        if (_session.ReservedExecutionIdentityKeys.Contains(identity.NamespaceKey))
        {
            reasonCode = "PRE_RESTART_EXECUTION_IDENTITY_RESERVED_NO_RESURRECTION";
            return false;
        }

        if (riskIncreasing && work.Intent.TrustEpoch != currentTrustedEpoch)
        {
            reasonCode = "PRE_RESTART_WORK_TRUST_EPOCH_NOT_CURRENT";
            return false;
        }

        if (riskIncreasing && !_session.CanIncreaseRisk(identity.Account))
        {
            reasonCode = "PRE_RESTART_ACCOUNT_TRUTH_OR_CAPITAL_RECONCILIATION_REQUIRED";
            return false;
        }

        reasonCode = "RESTART_EXECUTION_ADMISSION_ELIGIBLE";
        return true;
    }

    public bool TryRestoreQueueEligible(RestartExecutionRecord record, AccountScopedExecutionQueue queue, TrustEpoch currentTrustedEpoch, out ExecutionQueueSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(record);
        ArgumentNullException.ThrowIfNull(queue);
        if (record.Disposition != RestartExecutionDisposition.QueueEligible || !CanAdmit(record.DurableRecord.Work, currentTrustedEpoch, true, out _))
        {
            var identity = record.DurableRecord.Work.Intent.ExecutionIdentity;
            snapshot = new(record.DurableRecord.Work.WorkId, identity, ExecutionQueueState.CancelledByContainment, "RESTART_RESTORE_NOT_ELIGIBLE", record.DurableRecord.EvidenceReference, record.DurableRecord.ContainmentIncidentId);
            return false;
        }
        return queue.Enqueue(record.DurableRecord.Work, out snapshot);
    }
}
