namespace Falcon.FSATS.TradingGuardian.Application;

public sealed record GuardianDurableCapacityPolicy(int MaximumOutcomeTombstones, int MaximumUnresolvedProtectionOutcomes)
{
    public bool IsValid => MaximumOutcomeTombstones > 0 && MaximumUnresolvedProtectionOutcomes > 0;
}

public sealed record GuardianDurableCapacityAssessment(bool WithinCapacity, string ReasonCode);

public static class GuardianDurableCapacity
{
    public static GuardianDurableCapacityAssessment Assess(GuardianRestartPlan plan, GuardianDurableCapacityPolicy policy)
    {
        if (plan is null || !plan.Accepted || policy is null || !policy.IsValid) return new(false, "GUARDIAN_DURABLE_CAPACITY_POLICY_OR_STATE_INVALID");
        if (plan.IdempotencyTombstones.Count > policy.MaximumOutcomeTombstones) return new(false, "GUARDIAN_OUTCOME_TOMBSTONE_CAPACITY_EXHAUSTED_FAIL_CLOSED");
        if (plan.ReconciliationRequired.Count > policy.MaximumUnresolvedProtectionOutcomes) return new(false, "GUARDIAN_UNRESOLVED_PROTECTION_CAPACITY_EXHAUSTED_FAIL_CLOSED");
        return new(true, "GUARDIAN_DURABLE_CAPACITY_WITHIN_BOUND");
    }
}
