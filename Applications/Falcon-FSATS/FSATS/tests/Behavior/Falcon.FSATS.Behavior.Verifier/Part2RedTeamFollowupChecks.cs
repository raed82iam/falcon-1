using T = Falcon.FSATS.Trading.Domain;
using GA = Falcon.FSATS.TradingGuardian.Application;
using GC = Falcon.FSATS.TradingGuardian.Contracts;

internal static class Part2RedTeamFollowupChecks
{
    internal static void Run()
    {
        RejectInvalidReservationIdentity();
        PreDispatchCallerCancellationDoesNotInvokeRoute();
        CancellationAfterDispatchStartIsReconciliationOwned();
    }

    private static void RejectInvalidReservationIdentity()
    {
        var ledger = new T.CapitalReservationLedger();
        var account = new T.BrokerAccountContext("ALPACA", "PA-ACCOUNT-A", "PAPER");
        var usd = new T.Currency("USD");
        if (ledger.TryReserve(account, new T.ReservationId(""), new T.Money(1m, usd), new T.Money(10m, usd)))
            throw new InvalidOperationException("C-01_EMPTY_RESERVATION_ID_ACCEPTED");
        if (ledger.TryReserve(account, new T.ReservationId("   "), new T.Money(1m, usd), new T.Money(10m, usd)))
            throw new InvalidOperationException("C-01_WHITESPACE_RESERVATION_ID_ACCEPTED");
        var uninitialized = default(T.Currency);
        if (ledger.TryReserve(account, new T.ReservationId("default-currency"), new T.Money(1m, uninitialized), new T.Money(10m, uninitialized)))
            throw new InvalidOperationException("C-01_UNINITIALIZED_CURRENCY_ACCEPTED");
    }

    private static GA.GovernedProtectionCommandEnvelope Envelope(DateTimeOffset now, string idempotencyId)
    {
        var target = new GC.ProtectionTarget(GC.ProtectionTargetKind.BrokerAccount, "ALPACA", "PA-ACCOUNT-A", "PAPER");
        var command = new GC.ProtectionCommand(
            new GC.CommandId($"cmd-{idempotencyId}"), GC.ProtectionCommandType.NewRiskFreeze, "TARGET", target,
            "authority", "reason", new GC.ProtectionEpoch(9), now.AddSeconds(-1), now.AddMinutes(2), $"corr-{idempotencyId}", $"cause-{idempotencyId}");
        return new GA.GovernedProtectionCommandEnvelope(
            $"msg-{idempotencyId}", "schema", "1.0", GA.TradingGuardianManifest.Current.ApplicationId, "TARGET",
            "authority", "prov", command.CorrelationId, command.CausationId, idempotencyId, "attempt", "retry",
            GA.ProtectionTrafficTruth.Operational, now.AddSeconds(-1), now.AddMinutes(2), "evidence", command);
    }

    private static void PreDispatchCallerCancellationDoesNotInvokeRoute()
    {
        var now = DateTimeOffset.UtcNow;
        var route = new CountingApplyRoute();
        var dispatcher = new GA.GovernedProtectionCommandDispatcher(route);
        using var cts = new CancellationTokenSource();
        cts.Cancel();
        try
        {
            dispatcher.DispatchAsync(Envelope(now, "pre-cancel"), 9, now, cts.Token).AsTask().GetAwaiter().GetResult();
            throw new InvalidOperationException("PRE_DISPATCH_CALLER_CANCELLATION_WAS_NOT_PROPAGATED");
        }
        catch (OperationCanceledException) when (cts.IsCancellationRequested)
        {
        }

        if (route.Calls != 0)
            throw new InvalidOperationException("PRE_DISPATCH_CALLER_CANCELLATION_REACHED_ROUTE");
    }

    private static void CancellationAfterDispatchStartIsReconciliationOwned()
    {
        var now = DateTimeOffset.UtcNow;
        using var cts = new CancellationTokenSource();
        var route = new CancelAfterStartRoute(cts);
        var dispatcher = new GA.GovernedProtectionCommandDispatcher(route);
        var envelope = Envelope(now, "started-cancel");

        var first = dispatcher.DispatchAsync(envelope, 9, now, cts.Token).AsTask().GetAwaiter().GetResult();
        var retry = dispatcher.DispatchAsync(envelope with { MessageId = "transport-retry", DeliveryAttemptId = "attempt-2" }, 9, now, CancellationToken.None).AsTask().GetAwaiter().GetResult();

        if (first.State != GC.ProtectionOutcomeState.ReconciliationRequired ||
            first.ReasonCode != "ROUTE_DISPATCH_CANCELLATION_AMBIGUOUS" ||
            retry.State != GC.ProtectionOutcomeState.ReconciliationRequired ||
            route.Calls != 1)
            throw new InvalidOperationException("POST_DISPATCH_CANCELLATION_WAS_BLINDLY_RETRIED");
    }

    private sealed class CountingApplyRoute : GA.IGovernedProtectionCommandRoutePort
    {
        public int Calls { get; private set; }
        public ValueTask<GC.ProtectionCommandOutcome> DispatchAsync(GA.GovernedProtectionCommandEnvelope envelope, CancellationToken cancellationToken)
        {
            Calls++;
            return ValueTask.FromResult(new GC.ProtectionCommandOutcome(
                envelope.Command.CommandId,
                GC.ProtectionOutcomeState.Applied,
                envelope.Command.TargetApplication,
                envelope.Command.Target,
                "APPLIED",
                DateTimeOffset.UtcNow,
                envelope.CorrelationId));
        }
    }

    private sealed class CancelAfterStartRoute : GA.IGovernedProtectionCommandRoutePort
    {
        private readonly CancellationTokenSource _cts;
        public CancelAfterStartRoute(CancellationTokenSource cts) => _cts = cts;
        public int Calls { get; private set; }

        public ValueTask<GC.ProtectionCommandOutcome> DispatchAsync(GA.GovernedProtectionCommandEnvelope envelope, CancellationToken cancellationToken)
        {
            Calls++;
            _cts.Cancel();
            return ValueTask.FromCanceled<GC.ProtectionCommandOutcome>(cancellationToken);
        }
    }
}
