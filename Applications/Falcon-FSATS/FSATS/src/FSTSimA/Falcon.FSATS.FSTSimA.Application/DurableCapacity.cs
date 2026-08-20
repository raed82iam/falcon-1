namespace Falcon.FSATS.FSTSimA.Application;

public sealed record SimulationDurableCapacityPolicy(int MaximumRunRecords)
{
    public bool IsValid => MaximumRunRecords > 0;
}

public sealed record SimulationDurableCapacityAssessment(bool WithinCapacity, string ReasonCode);

public static class SimulationDurableCapacity
{
    public static SimulationDurableCapacityAssessment Assess(SimulationRestartPlan plan, SimulationDurableCapacityPolicy policy)
    {
        if (plan is null || !plan.Accepted || policy is null || !policy.IsValid) return new(false, "SIMULATION_DURABLE_CAPACITY_POLICY_OR_STATE_INVALID");
        if (plan.Runs.Count > policy.MaximumRunRecords) return new(false, "SIMULATION_RUN_DURABLE_CAPACITY_EXHAUSTED_FAIL_CLOSED");
        return new(true, "SIMULATION_DURABLE_CAPACITY_WITHIN_BOUND");
    }
}
