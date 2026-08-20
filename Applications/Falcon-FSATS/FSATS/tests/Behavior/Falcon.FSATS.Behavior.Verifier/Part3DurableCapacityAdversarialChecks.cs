using System.Runtime.CompilerServices;
using T = Falcon.FSATS.Trading.Domain;
using TA = Falcon.FSATS.Trading.Application;
using PA = Falcon.FSATS.FSAPMA.Application;
using GA = Falcon.FSATS.TradingGuardian.Application;
using RA = Falcon.FSATS.ResourceManagement.Application;
using SA = Falcon.FSATS.FSTSimA.Application;

internal static class Part3DurableCapacityAdversarialChecks
{
    [ModuleInitializer]
    internal static void Run()
    {
        TradingCapacityExhaustionFailsClosed();
        InvalidPoliciesFailClosed();
    }

    private static void TradingCapacityExhaustionFailsClosed()
    {
        var now = DateTimeOffset.UtcNow;
        var account = new T.BrokerAccountContext("ALPACA", "ACC-CAP", "PAPER");
        var identity = new TA.BrokerExecutionIdentity(account, "route", "submission", new T.OrderId("order"));
        var safety = new T.PositionSafetyEnvelope(new T.PositionId("position"), new T.InstrumentId("AAPL"), new T.Quantity(1), new T.Money(5m, new T.Currency("USD")), "guardian", "protected", "exit", "current", new T.TrustEpoch(1));
        var work = new TA.QueuedExecutionWork("work", new TA.OrderIntent(identity, new T.InstrumentId("AAPL"), new T.Quantity(1), new T.TrustEpoch(1), safety), now.AddSeconds(-1), "ev");
        var snapshot = TA.TradingDurableSnapshot.Create(1, now, new[] { new TA.DurableExecutionRecord(work, TA.ExecutionQueueState.DispatchStarted, "started", "ev", null, 1, now) });
        var plan = TA.TradingRestartReconstructor.Reconstruct(snapshot, now.AddSeconds(1));
        var result = TA.TradingDurableCapacity.Assess(plan, 0, new TA.TradingDurableCapacityPolicy(1, 10));
        if (result.WithinCapacity || result.ReasonCode != "SAFETY_DURABLE_CAPACITY_EXHAUSTED_FAIL_CLOSED")
            throw new InvalidOperationException("P3_TRADING_SAFETY_CAPACITY_EXHAUSTION_DID_NOT_FAIL_CLOSED");
    }

    private static void InvalidPoliciesFailClosed()
    {
        var emptyFsapma = new PA.FSAPMARestartPlan(true, "ok", Array.Empty<PA.FSAPMARestartStreamState>(), new Dictionary<string, PA.DurableOperationalDeliveryRecord>());
        var emptyGuardian = new GA.GuardianRestartPlan(true, "ok", new Dictionary<string, GA.DurableProtectionOutcomeRecord>(), Array.Empty<Falcon.FSATS.TradingGuardian.Contracts.ProtectionCommandOutcome>(), false);
        var emptyResource = new RA.ResourceRestartPlan(true, "ok", new Falcon.FSATS.ResourceManagement.Domain.CoordinationEpoch(0), new HashSet<string>(), false);
        var emptySimulation = new SA.SimulationRestartPlan(true, "ok", Array.Empty<SA.SimulationRestartRun>());
        if (PA.FSAPMADurableCapacity.Assess(emptyFsapma, new PA.FSAPMADurableCapacityPolicy(0, 0)).WithinCapacity ||
            GA.GuardianDurableCapacity.Assess(emptyGuardian, new GA.GuardianDurableCapacityPolicy(0, 0)).WithinCapacity ||
            RA.ResourceDurableCapacity.Assess(emptyResource, new RA.ResourceDurableCapacityPolicy(0)).WithinCapacity ||
            SA.SimulationDurableCapacity.Assess(emptySimulation, new SA.SimulationDurableCapacityPolicy(0)).WithinCapacity)
            throw new InvalidOperationException("P3_INVALID_DURABLE_CAPACITY_POLICY_DID_NOT_FAIL_CLOSED");
    }
}
