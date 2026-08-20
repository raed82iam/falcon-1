using Falcon.FSATS.Trading.Domain;

namespace Falcon.FSATS.Trading.Application;

public sealed class TradingRestartRecoverySession
{
    private readonly object _gate = new();
    private readonly TradingRestartPlan _plan;
    private readonly DateTimeOffset _snapshotCapturedAt;
    private readonly HashSet<BrokerAccountContext> _contained;
    private readonly List<DurableReconciliationObligation> _reconciliation;
    private readonly List<DurableCapitalReservation> _reservations;
    private readonly IReadOnlyList<DurableBrokerContainment> _brokerContainments;

    public TradingRestartRecoverySession(TradingRestartPlan plan, TradingDurableSnapshot snapshot)
    {
        _plan = plan ?? throw new ArgumentNullException(nameof(plan));
        ArgumentNullException.ThrowIfNull(snapshot);
        if (!plan.Accepted) throw new InvalidOperationException("TRADING_RESTART_PLAN_NOT_ACCEPTED");
        _snapshotCapturedAt = snapshot.CapturedAt;
        _contained = new HashSet<BrokerAccountContext>(plan.ContainedAccounts);
        _reconciliation = new List<DurableReconciliationObligation>(plan.ReconciliationObligations);
        _reservations = new List<DurableCapitalReservation>(plan.CapitalReservations);
        _brokerContainments = Array.AsReadOnly((snapshot.BrokerContainments ?? Array.Empty<DurableBrokerContainment>()).ToArray());
    }

    public IReadOnlySet<string> ReservedExecutionIdentityKeys => _plan.ReservedExecutionIdentityKeys;

    public bool CanIncreaseRisk(BrokerAccountContext account)
    {
        ArgumentNullException.ThrowIfNull(account);
        lock (_gate)
        {
            return !_contained.Contains(account) &&
                   !_reconciliation.Any(x => x.Identity.Account == account && x.SubmissionTruth != BrokerSubmissionTruth.NotSubmitted) &&
                   !_reservations.Any(x => x.Account == account) &&
                   !_plan.Execution.Any(x => x.DurableRecord.Work.Intent.ExecutionIdentity.Account == account && x.Disposition == RestartExecutionDisposition.ReconciliationRequired && _reconciliation.Any(r => r.Identity == x.DurableRecord.Work.Intent.ExecutionIdentity));
        }
    }

    public bool TryResolveAccount(BrokerAccountContext account, BrokerAccountReconciliationEvidence reconciliation)
    {
        ArgumentNullException.ThrowIfNull(account);
        ArgumentNullException.ThrowIfNull(reconciliation);
        lock (_gate)
        {
            if (_brokerContainments.Any(x => x.AffectedAccounts.Contains(account))) return false;
            if (!IsFreshComplete(account, reconciliation)) return false;
            _contained.Remove(account);
            _reconciliation.RemoveAll(x => x.Identity.Account == account);
            _reservations.RemoveAll(x => x.Account == account);
            return true;
        }
    }

    public bool TryResolveBroker(string brokerId, string environment, IReadOnlyCollection<BrokerAccountReconciliationEvidence> reconciliation)
    {
        if (string.IsNullOrWhiteSpace(brokerId) || string.IsNullOrWhiteSpace(environment) || reconciliation is null) return false;
        var broker = brokerId.Trim().ToUpperInvariant();
        var env = environment.Trim().ToUpperInvariant();
        lock (_gate)
        {
            var declared = _brokerContainments.Where(x => x.BrokerId.Trim().ToUpperInvariant() == broker && x.Environment.Trim().ToUpperInvariant() == env).SelectMany(x => x.AffectedAccounts);
            var affected = declared
                .Concat(_contained.Where(x => x.BrokerId == broker && x.Environment == env))
                .Concat(_reconciliation.Select(x => x.Identity.Account).Where(x => x.BrokerId == broker && x.Environment == env))
                .Concat(_reservations.Select(x => x.Account).Where(x => x.BrokerId == broker && x.Environment == env))
                .Distinct()
                .ToArray();
            if (affected.Length == 0) return false;
            if (affected.Any(account => !reconciliation.Any(x => IsFreshComplete(account, x)))) return false;
            _contained.RemoveWhere(x => x.BrokerId == broker && x.Environment == env);
            _reconciliation.RemoveAll(x => x.Identity.Account.BrokerId == broker && x.Identity.Account.Environment == env);
            _reservations.RemoveAll(x => x.Account.BrokerId == broker && x.Account.Environment == env);
            return true;
        }
    }

    private bool IsFreshComplete(BrokerAccountContext account, BrokerAccountReconciliationEvidence reconciliation)
        => BrokerOutageRecoveryPolicy.IsCompleteReconciliationFor(account, reconciliation) &&
           reconciliation.ObservedAt > _snapshotCapturedAt &&
           reconciliation.DimensionEvidence is not null &&
           reconciliation.DimensionEvidence.All(x => x.ObservedAt > _snapshotCapturedAt);
}
