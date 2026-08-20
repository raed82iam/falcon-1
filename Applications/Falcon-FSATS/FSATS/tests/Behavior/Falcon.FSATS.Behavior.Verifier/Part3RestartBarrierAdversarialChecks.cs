using System.Runtime.CompilerServices;
using T = Falcon.FSATS.Trading.Domain;
using TA = Falcon.FSATS.Trading.Application;
using P = Falcon.FSATS.FSAPMA.Domain;
using PA = Falcon.FSATS.FSAPMA.Application;
using PC = Falcon.FSATS.FSAPMA.Contracts;
using G = Falcon.FSATS.TradingGuardian.Contracts;
using GA = Falcon.FSATS.TradingGuardian.Application;

internal static class Part3RestartBarrierAdversarialChecks
{
    [ModuleInitializer]
    internal static void Run()
    {
        TradingReservedIdentityCannotReenterQueue();
        TradingStaleTrustEpochCannotRestoreQueuedRisk();
        FsAPMADurableUnknownTombstoneSuppressesRedispatch();
        GuardianDurableAmbiguitySuppressesRedispatch();
    }

    private static void TradingReservedIdentityCannotReenterQueue()
    {
        var now = DateTimeOffset.UtcNow;
        var account = new T.BrokerAccountContext("ALPACA", "ACC-A", "PAPER");
        var identity = new TA.BrokerExecutionIdentity(account, "route", "submission", new T.OrderId("order"));
        var safety = new T.PositionSafetyEnvelope(new T.PositionId("position"), new T.InstrumentId("AAPL"), new T.Quantity(1), new T.Money(5m, new T.Currency("USD")), "guardian", "protected", "exit", "current", new T.TrustEpoch(1));
        var intent = new TA.OrderIntent(identity, new T.InstrumentId("AAPL"), new T.Quantity(1), new T.TrustEpoch(1), safety);
        var work = new TA.QueuedExecutionWork("work", intent, now.AddSeconds(-1), "ev");
        var snapshot = TA.TradingDurableSnapshot.Create(1, now, new[] { new TA.DurableExecutionRecord(work, TA.ExecutionQueueState.Completed, "done", "ev", null, 1, now) });
        var plan = TA.TradingRestartReconstructor.Reconstruct(snapshot, now.AddSeconds(1));
        var session = new TA.TradingRestartRecoverySession(plan, snapshot);
        var barrier = new TA.RestartAwareExecutionAdmission(session);
        if (barrier.CanAdmit(work, new T.TrustEpoch(1), true, out var reason) || reason != "PRE_RESTART_EXECUTION_IDENTITY_RESERVED_NO_RESURRECTION")
            throw new InvalidOperationException("P3_TRADING_RESERVED_EXECUTION_IDENTITY_REENTERED_AFTER_RESTART");
    }

    private static void TradingStaleTrustEpochCannotRestoreQueuedRisk()
    {
        var now = DateTimeOffset.UtcNow;
        var account = new T.BrokerAccountContext("ALPACA", "ACC-B", "PAPER");
        var identity = new TA.BrokerExecutionIdentity(account, "route", "submission-b", new T.OrderId("order-b"));
        var safety = new T.PositionSafetyEnvelope(new T.PositionId("position-b"), new T.InstrumentId("AAPL"), new T.Quantity(1), new T.Money(5m, new T.Currency("USD")), "guardian", "protected", "exit", "current", new T.TrustEpoch(1));
        var work = new TA.QueuedExecutionWork("work-b", new TA.OrderIntent(identity, new T.InstrumentId("AAPL"), new T.Quantity(1), new T.TrustEpoch(1), safety), now.AddSeconds(-1), "ev-b");
        var snapshot = TA.TradingDurableSnapshot.Create(2, now, new[] { new TA.DurableExecutionRecord(work, TA.ExecutionQueueState.Queued, "queued", "ev-b", null, 1, now) });
        var plan = TA.TradingRestartReconstructor.Reconstruct(snapshot, now.AddSeconds(1));
        var barrier = new TA.RestartAwareExecutionAdmission(new TA.TradingRestartRecoverySession(plan, snapshot));
        if (barrier.CanAdmit(work, new T.TrustEpoch(2), true, out var reason) || reason != "PRE_RESTART_WORK_TRUST_EPOCH_NOT_CURRENT")
            throw new InvalidOperationException("P3_TRADING_STALE_TRUST_EPOCH_RESTORED_RISK_INCREASING_WORK");
    }

    private static void FsAPMADurableUnknownTombstoneSuppressesRedispatch()
    {
        var now = DateTimeOffset.UtcNow;
        var projection = new PC.OperationalDataProjection(
            new PC.ObservationId("obs-p3"), new PC.ProviderId("ALPACA"), new PC.ProducerInstrumentId("ALPACA", "AAPL"),
            new PC.DataProductId("last-price"), 100m, now.AddSeconds(-2), now.AddSeconds(-1), PC.DataTruthState.Current,
            "prov", "1.0", new PC.ProviderAccountId("DATA-A"), "PAPER", "MARKET_DATA", "cred-ref");
        var envelope = new PA.OperationalDataDeliveryEnvelope(
            "msg", "schema", "1.0", PA.FSAPMAManifest.Current.ApplicationId, "FSATS-TRADING", "authority", "prov",
            "corr", "cause", "idem", "attempt", "retry", PA.OperationalDataTrafficTruth.Operational,
            now.AddSeconds(-1), now.AddMinutes(1), TimeSpan.FromMinutes(1), null, "ev", projection);
        var key = PA.GovernedOperationalDataGuards.IdempotencyScopeKey(envelope);
        var fingerprint = PA.GovernedOperationalDataGuards.Fingerprint(envelope);
        var unknown = new PA.OperationalDataDeliveryResult(PA.OperationalDataDeliveryState.DeliveryOutcomeUnknown, projection.ObservationId.Value, envelope.ConsumerApplicationId, "AMBIGUOUS", envelope.CorrelationId, now, projection.ProviderRouteNamespace);
        var durable = new PA.DurableOperationalDeliveryRecord(key, fingerprint, unknown, now);
        var restart = PA.FSAPMARestartReconstructor.Reconstruct(PA.FSAPMADurableSnapshot.Create(1, now, Array.Empty<PA.DurableStreamContinuityRecord>(), new[] { durable }), now.AddSeconds(1));
        var port = new CountingDataPort();
        var service = new PA.RestartAwareOperationalDataDeliveryService(new PA.GovernedOperationalDataDeliveryService(port), restart);
        var result = service.DeliverAsync(envelope, now.AddSeconds(1), CancellationToken.None).AsTask().GetAwaiter().GetResult();
        if (result.State != PA.OperationalDataDeliveryState.DeliveryOutcomeUnknown || port.Calls != 0)
            throw new InvalidOperationException("P3_FSAPMA_DURABLE_UNKNOWN_WAS_REDISPATCHED");
    }

    private static void GuardianDurableAmbiguitySuppressesRedispatch()
    {
        var now = DateTimeOffset.UtcNow;
        var target = new G.ProtectionTarget(G.ProtectionTargetKind.BrokerAccount, "ALPACA", "ACC-A", "PAPER");
        var command = new G.ProtectionCommand(new G.CommandId("cmd"), G.ProtectionCommandType.NewRiskFreeze, "FSATS-TRADING", target, "authority", "reason", new G.ProtectionEpoch(9), now.AddSeconds(-1), now.AddMinutes(1), "corr", "cause");
        var envelope = new GA.GovernedProtectionCommandEnvelope("msg", "schema", "1.0", GA.TradingGuardianManifest.Current.ApplicationId, command.TargetApplication, "authority", "prov", "corr", "cause", "idem", "attempt", "retry", GA.ProtectionTrafficTruth.Operational, now.AddSeconds(-1), now.AddMinutes(1), "ev", command);
        var key = GA.GovernedProtectionRouteGuards.IdempotencyScopeKey(envelope);
        var fingerprint = GA.GovernedProtectionRouteGuards.Fingerprint(envelope);
        var ambiguous = new G.ProtectionCommandOutcome(command.CommandId, G.ProtectionOutcomeState.ReconciliationRequired, command.TargetApplication, target, "AMBIGUOUS", now, envelope.CorrelationId, fingerprint, envelope.EvidenceReference);
        var durable = new GA.DurableProtectionOutcomeRecord(key, fingerprint, ambiguous, now);
        var restart = GA.GuardianRestartReconstructor.Reconstruct(GA.GuardianDurableSnapshot.Create(1, now, new[] { durable }), now.AddSeconds(1));
        var port = new CountingProtectionPort();
        var service = new GA.RestartAwareProtectionCommandDispatcher(new GA.GovernedProtectionCommandDispatcher(port), restart);
        var result = service.DispatchAsync(envelope, 9, now.AddSeconds(1), CancellationToken.None).AsTask().GetAwaiter().GetResult();
        if (result.State != G.ProtectionOutcomeState.ReconciliationRequired || port.Calls != 0)
            throw new InvalidOperationException("P3_GUARDIAN_DURABLE_AMBIGUITY_WAS_REDISPATCHED");
    }

    private sealed class CountingDataPort : PA.IGovernedOperationalDataRoutePort
    {
        public int Calls { get; private set; }
        public ValueTask<PA.OperationalDataDeliveryResult> DeliverAsync(PA.OperationalDataDeliveryEnvelope envelope, CancellationToken cancellationToken)
        {
            Calls++;
            return ValueTask.FromResult(new PA.OperationalDataDeliveryResult(PA.OperationalDataDeliveryState.DeliveredCurrent, envelope.Projection.ObservationId.Value, envelope.ConsumerApplicationId, "delivered", envelope.CorrelationId, DateTimeOffset.UtcNow, envelope.Projection.ProviderRouteNamespace));
        }
    }

    private sealed class CountingProtectionPort : GA.IGovernedProtectionCommandRoutePort
    {
        public int Calls { get; private set; }
        public ValueTask<G.ProtectionCommandOutcome> DispatchAsync(GA.GovernedProtectionCommandEnvelope envelope, CancellationToken cancellationToken)
        {
            Calls++;
            return ValueTask.FromResult(new G.ProtectionCommandOutcome(envelope.Command.CommandId, G.ProtectionOutcomeState.Applied, envelope.Command.TargetApplication, envelope.Command.Target, "applied", DateTimeOffset.UtcNow, envelope.CorrelationId));
        }
    }
}
