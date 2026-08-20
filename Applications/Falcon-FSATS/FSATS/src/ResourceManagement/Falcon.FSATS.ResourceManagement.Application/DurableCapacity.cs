namespace Falcon.FSATS.ResourceManagement.Application;

public sealed record ResourceDurableCapacityPolicy(int MaximumFencedDecisionRecords)
{
    public bool IsValid => MaximumFencedDecisionRecords > 0;
}

public sealed record ResourceDurableCapacityAssessment(bool WithinCapacity, string ReasonCode);

public static class ResourceDurableCapacity
{
    public static ResourceDurableCapacityAssessment Assess(ResourceRestartPlan plan, ResourceDurableCapacityPolicy policy)
    {
        if (plan is null || !plan.Accepted || policy is null || !policy.IsValid) return new(false, "APP_RSC_DURABLE_CAPACITY_POLICY_OR_STATE_INVALID");
        if (plan.FencedDecisionIds.Count > policy.MaximumFencedDecisionRecords) return new(false, "APP_RSC_FENCED_DECISION_CAPACITY_EXHAUSTED_FAIL_CLOSED");
        return new(true, "APP_RSC_DURABLE_CAPACITY_WITHIN_BOUND");
    }
}
