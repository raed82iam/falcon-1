using System.Security.Cryptography;
using System.Text;

namespace Falcon.FSATS.Trading.Application;

public sealed record TradingDurableCapacityPolicy(int MaximumSafetyRecords, int MaximumIdentityTombstones)
{
    public bool IsValid => MaximumSafetyRecords > 0 && MaximumIdentityTombstones > 0;
}

public sealed record TradingDurableCapacityAssessment(bool WithinCapacity, string ReasonCode, int SafetyRecords, int IdentityTombstones);

public sealed record DurableCancelledExecutionTombstone(
    string ExecutionIdentityKey,
    string WorkId,
    string IncidentId,
    string EvidenceReference,
    DateTimeOffset CompactedAt,
    string TombstoneSha256);

public static class TradingDurableCapacity
{
    public static TradingDurableCapacityAssessment Assess(TradingRestartPlan plan, int existingIdentityTombstones, TradingDurableCapacityPolicy policy)
    {
        if (plan is null || !plan.Accepted || policy is null || !policy.IsValid || existingIdentityTombstones < 0)
            return new(false, "DURABLE_CAPACITY_POLICY_OR_STATE_INVALID", int.MaxValue, Math.Max(0, existingIdentityTombstones));

        var safety = plan.Execution.Count(TradingDurableRetention.IsSafetyCritical) + plan.ReconciliationObligations.Count + plan.CapitalReservations.Count + plan.ContainedAccounts.Count;
        if (safety > policy.MaximumSafetyRecords) return new(false, "SAFETY_DURABLE_CAPACITY_EXHAUSTED_FAIL_CLOSED", safety, existingIdentityTombstones);
        if (existingIdentityTombstones > policy.MaximumIdentityTombstones) return new(false, "IDENTITY_TOMBSTONE_CAPACITY_EXHAUSTED_FAIL_CLOSED", safety, existingIdentityTombstones);
        return new(true, "DURABLE_CAPACITY_WITHIN_BOUND", safety, existingIdentityTombstones);
    }

    public static DurableCancelledExecutionTombstone CompactReleasedCancellation(RestartExecutionRecord record, TradingRestartRecoverySession session, DateTimeOffset compactedAt)
    {
        ArgumentNullException.ThrowIfNull(record);
        ArgumentNullException.ThrowIfNull(session);
        var account = record.DurableRecord.Work.Intent.ExecutionIdentity.Account;
        if (record.Disposition != RestartExecutionDisposition.CancelledTombstone || !session.CanIncreaseRisk(account) || compactedAt == default || compactedAt < record.DurableRecord.CapturedAt)
            throw new InvalidOperationException("CANCELLED_TOMBSTONE_NOT_ELIGIBLE_FOR_SAFE_COMPACTION");
        var identity = record.DurableRecord.Work.Intent.ExecutionIdentity.NamespaceKey;
        var workId = record.DurableRecord.Work.WorkId;
        var incident = record.DurableRecord.ContainmentIncidentId ?? "RELEASED_CONTAINMENT_IDENTITY_FENCE";
        var evidence = record.DurableRecord.EvidenceReference;
        var packed = TradingDurableIntegrity.Pack(identity, workId, incident, evidence, compactedAt.ToUniversalTime().ToString("O"));
        var digest = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(packed)));
        return new(identity, workId, incident, evidence, compactedAt, digest);
    }
}
