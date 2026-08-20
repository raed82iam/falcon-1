using Falcon.FSATS.Trading.Domain;

namespace Falcon.FSATS.Trading.Application;

public static class TradingDurableCapture
{
    public static DurableExecutionRecord CaptureExecution(QueuedExecutionWork work, ExecutionQueueSnapshot snapshot, long generation, DateTimeOffset capturedAt)
    {
        ArgumentNullException.ThrowIfNull(work);
        ArgumentNullException.ThrowIfNull(snapshot);
        if (work.Intent.ExecutionIdentity != snapshot.Identity || !StringComparer.Ordinal.Equals(work.WorkId, snapshot.WorkId))
            throw new ArgumentException("DURABLE_EXECUTION_CAPTURE_BINDING_MISMATCH");
        if (generation < 0 || capturedAt == default) throw new ArgumentException("DURABLE_EXECUTION_CAPTURE_METADATA_INVALID");
        return new(work, snapshot.State, snapshot.ReasonCode, snapshot.EvidenceReference, snapshot.ContainmentIncidentId, generation, capturedAt);
    }

    public static DurableAccountContainment CaptureAccountContainment(BrokerAccountContext account, ExecutionContainmentEvidence evidence)
    {
        ArgumentNullException.ThrowIfNull(account);
        ArgumentNullException.ThrowIfNull(evidence);
        if (evidence.AffectedAccounts.Count != 1 || evidence.AffectedAccounts[0] != account) throw new ArgumentException("DURABLE_ACCOUNT_CONTAINMENT_SCOPE_MISMATCH");
        return new(account, evidence.IncidentId, evidence.ReasonCode, evidence.EvidenceReference, evidence.ObservedAt);
    }

    public static DurableBrokerContainment CaptureBrokerContainment(string brokerId, string environment, ExecutionContainmentEvidence evidence)
    {
        ArgumentNullException.ThrowIfNull(evidence);
        if (string.IsNullOrWhiteSpace(brokerId) || string.IsNullOrWhiteSpace(environment)) throw new ArgumentException("DURABLE_BROKER_CONTAINMENT_SCOPE_REQUIRED");
        var broker = brokerId.Trim().ToUpperInvariant();
        var env = environment.Trim().ToUpperInvariant();
        if (evidence.AffectedAccounts.Count == 0 || evidence.AffectedAccounts.Any(x => x.BrokerId != broker || x.Environment != env)) throw new ArgumentException("DURABLE_BROKER_CONTAINMENT_SCOPE_MISMATCH");
        return new(broker, env, evidence.IncidentId, evidence.ReasonCode, evidence.EvidenceReference, evidence.ObservedAt, evidence.AffectedAccounts);
    }
}
