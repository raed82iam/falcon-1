using System.Runtime.CompilerServices;
using T = Falcon.FSATS.Trading.Domain;
using TA = Falcon.FSATS.Trading.Application;

internal static class Part3TradingRecoverySessionAdversarialChecks
{
    [ModuleInitializer]
    internal static void Run()
    {
        DurableCapitalReservationBlocksRiskUntilExactReconciliation();
        WrongAccountReconciliationCannotReleaseContainment();
    }

    private static void DurableCapitalReservationBlocksRiskUntilExactReconciliation()
    {
        var now = DateTimeOffset.UtcNow.AddSeconds(-10);
        var account = new T.BrokerAccountContext("ALPACA", "ACC-R", "PAPER");
        var reservation = new TA.DurableCapitalReservation(account, new T.ReservationId("res-1"), new T.Money(25m, new T.Currency("USD")), "ev-res", now);
        var snapshot = TA.TradingDurableSnapshot.Create(1, now, Array.Empty<TA.DurableExecutionRecord>(), capitalReservations: new[] { reservation });
        var plan = TA.TradingRestartReconstructor.Reconstruct(snapshot, now.AddSeconds(1));
        var session = new TA.TradingRestartRecoverySession(plan, snapshot);
        if (session.CanIncreaseRisk(account)) throw new InvalidOperationException("P3_TRADING_DURABLE_RESERVATION_DID_NOT_BLOCK_NEW_RISK");
        if (!session.TryResolveAccount(account, Complete(account, now.AddSeconds(2))) || !session.CanIncreaseRisk(account))
            throw new InvalidOperationException("P3_TRADING_EXACT_RECONCILIATION_DID_NOT_RELEASE_RESERVATION_HOLD");
    }

    private static void WrongAccountReconciliationCannotReleaseContainment()
    {
        var now = DateTimeOffset.UtcNow.AddSeconds(-10);
        var accountA = new T.BrokerAccountContext("ALPACA", "ACC-A", "PAPER");
        var accountB = new T.BrokerAccountContext("ALPACA", "ACC-B", "PAPER");
        var containment = new TA.DurableAccountContainment(accountA, "incident", "risk", "ev", now);
        var snapshot = TA.TradingDurableSnapshot.Create(2, now, Array.Empty<TA.DurableExecutionRecord>(), new[] { containment });
        var plan = TA.TradingRestartReconstructor.Reconstruct(snapshot, now.AddSeconds(1));
        var session = new TA.TradingRestartRecoverySession(plan, snapshot);
        if (session.TryResolveAccount(accountA, Complete(accountB, now.AddSeconds(2))) || session.CanIncreaseRisk(accountA))
            throw new InvalidOperationException("P3_TRADING_WRONG_ACCOUNT_RECONCILIATION_RELEASED_CONTAINMENT");
        if (!session.TryResolveAccount(accountA, Complete(accountA, now.AddSeconds(3))) || !session.CanIncreaseRisk(accountA))
            throw new InvalidOperationException("P3_TRADING_EXACT_ACCOUNT_RECONCILIATION_FAILED_TO_RELEASE_LOCAL_CONTAINMENT");
    }

    private static TA.BrokerAccountReconciliationEvidence Complete(T.BrokerAccountContext account, DateTimeOffset at)
    {
        var dimensions = Enum.GetValues<TA.BrokerReconciliationDimension>()
            .Select(x => new TA.BrokerReconciliationDimensionEvidence(x, true, "ev-" + x, at))
            .ToArray();
        return new TA.BrokerAccountReconciliationEvidence(account, true, true, true, true, true, true, true, "ev-full", at, dimensions);
    }
}
