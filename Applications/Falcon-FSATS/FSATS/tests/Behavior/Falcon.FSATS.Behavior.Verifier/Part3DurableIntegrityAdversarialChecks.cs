using System.Runtime.CompilerServices;
using T = Falcon.FSATS.Trading.Domain;
using TA = Falcon.FSATS.Trading.Application;
using P = Falcon.FSATS.FSAPMA.Domain;
using PA = Falcon.FSATS.FSAPMA.Application;
using G = Falcon.FSATS.TradingGuardian.Contracts;
using GA = Falcon.FSATS.TradingGuardian.Application;

internal static class Part3DurableIntegrityAdversarialChecks
{
    [ModuleInitializer]
    internal static void Run()
    {
        TradingSemanticPayloadTamperFailsDigest();
        FsAPMAStreamIdentityTamperFailsDigest();
        GuardianTargetTamperFailsDigest();
    }

    private static void TradingSemanticPayloadTamperFailsDigest()
    {
        var now = DateTimeOffset.UtcNow;
        var account = new T.BrokerAccountContext("ALPACA", "ACC-TAMPER", "PAPER");
        var identity = new TA.BrokerExecutionIdentity(account, "route", "submission", new T.OrderId("order"));
        var safety = new T.PositionSafetyEnvelope(new T.PositionId("position"), new T.InstrumentId("AAPL"), new T.Quantity(1), new T.Money(10m, new T.Currency("USD")), "guardian", "protected", "exit", "current", new T.TrustEpoch(1));
        var intent = new TA.OrderIntent(identity, new T.InstrumentId("AAPL"), new T.Quantity(1), new T.TrustEpoch(1), safety);
        var work = new TA.QueuedExecutionWork("work", intent, now.AddSeconds(-1), "evidence");
        var durable = new TA.DurableExecutionRecord(work, TA.ExecutionQueueState.Queued, "queued", "evidence", null, 1, now);
        var snapshot = TA.TradingDurableSnapshot.Create(1, now, new[] { durable });
        var changedIntent = intent with { Quantity = new T.Quantity(99m) };
        var changedWork = work with { Intent = changedIntent };
        var tampered = snapshot with { ExecutionRecords = new[] { durable with { Work = changedWork } } };
        if (TA.TradingRestartReconstructor.Reconstruct(tampered, now.AddSeconds(1)).Accepted)
            throw new InvalidOperationException("P3_TRADING_SEMANTIC_PAYLOAD_TAMPER_NOT_DETECTED");
    }

    private static void FsAPMAStreamIdentityTamperFailsDigest()
    {
        var now = DateTimeOffset.UtcNow;
        var route = new P.ProviderRouteIdentity(new P.ProviderId("ALPACA"), new P.ProviderAccountId("DATA-A"), "PAPER", "MARKET_DATA", new P.CredentialReference("cred"));
        var identity = new P.ProviderStreamSessionIdentity(route, "endpoint", "session", "AAPL");
        var record = new PA.DurableStreamContinuityRecord(identity, P.StreamContinuityState.GapDetected, 10, "ev", now);
        var snapshot = PA.FSAPMADurableSnapshot.Create(1, now, new[] { record }, Array.Empty<PA.DurableOperationalDeliveryRecord>());
        var otherIdentity = new P.ProviderStreamSessionIdentity(route, "endpoint", "session", "MSFT");
        var tampered = snapshot with { Streams = new[] { record with { Identity = otherIdentity } } };
        if (PA.FSAPMARestartReconstructor.Reconstruct(tampered, now.AddSeconds(1)).Accepted)
            throw new InvalidOperationException("P3_FSAPMA_STREAM_IDENTITY_TAMPER_NOT_DETECTED");
    }

    private static void GuardianTargetTamperFailsDigest()
    {
        var now = DateTimeOffset.UtcNow;
        var target = new G.ProtectionTarget(G.ProtectionTargetKind.BrokerAccount, "ALPACA", "ACC-A", "PAPER");
        var outcome = new G.ProtectionCommandOutcome(new G.CommandId("cmd"), G.ProtectionOutcomeState.ReconciliationRequired, "Trading", target, "unknown", now, "corr", new string('A', 64), "ev");
        var record = new GA.DurableProtectionOutcomeRecord("scope", new string('A', 64), outcome, now);
        var snapshot = GA.GuardianDurableSnapshot.Create(1, now, new[] { record });
        var wrong = new G.ProtectionTarget(G.ProtectionTargetKind.BrokerAccount, "ALPACA", "ACC-B", "PAPER");
        var tampered = snapshot with { Outcomes = new[] { record with { Outcome = outcome with { Target = wrong } } } };
        if (GA.GuardianRestartReconstructor.Reconstruct(tampered, now.AddSeconds(1)).Accepted)
            throw new InvalidOperationException("P3_GUARDIAN_TARGET_TAMPER_NOT_DETECTED");
    }
}
