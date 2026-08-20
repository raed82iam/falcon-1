namespace Falcon.FSATS.FSAPMA.Application;

public sealed record FSAPMADurableCapacityPolicy(int MaximumStreamRecords, int MaximumDeliveryTombstones)
{
    public bool IsValid => MaximumStreamRecords > 0 && MaximumDeliveryTombstones > 0;
}

public sealed record FSAPMADurableCapacityAssessment(bool WithinCapacity, string ReasonCode);

public static class FSAPMADurableCapacity
{
    public static FSAPMADurableCapacityAssessment Assess(FSAPMARestartPlan plan, FSAPMADurableCapacityPolicy policy)
    {
        if (plan is null || !plan.Accepted || policy is null || !policy.IsValid) return new(false, "FSAPMA_DURABLE_CAPACITY_POLICY_OR_STATE_INVALID");
        if (plan.Streams.Count > policy.MaximumStreamRecords) return new(false, "FSAPMA_STREAM_DURABLE_CAPACITY_EXHAUSTED_FAIL_CLOSED");
        if (plan.DeliveryTombstones.Count > policy.MaximumDeliveryTombstones) return new(false, "FSAPMA_DELIVERY_TOMBSTONE_CAPACITY_EXHAUSTED_FAIL_CLOSED");
        return new(true, "FSAPMA_DURABLE_CAPACITY_WITHIN_BOUND");
    }
}
