using TA = Falcon.FSATS.Trading.Application;
using PA = Falcon.FSATS.FSAPMA.Application;
using GA = Falcon.FSATS.TradingGuardian.Application;

internal static class EventIngressOrderingAdversarialChecks
{
    internal static void Run()
    {
        TradingOrderingRace();
        ProviderOrderingRace();
        GuardianOrderingRace();
    }

    private static void TradingOrderingRace()
    {
        var now = DateTimeOffset.UtcNow;
        var ingress = new TA.GovernedApplicationEventIngress();
        var first = new TA.GovernedApplicationEventEnvelope(
            "evt-order-t-a", "test", PA.FSAPMAManifest.Current.ApplicationId, TA.TradingManifest.Current.ApplicationId,
            "schema", "1.0", "msg-a", "corr-a", "cause-a", "idem-a", TA.ApplicationEventTruth.NonAuthoritativeEvidence,
            TA.ApplicationEventRelation.None, null, "same-key", 1, now.AddSeconds(-1), "evidence", new string('A', 64));
        var second = first with { EventId = "evt-order-t-b", MessageId = "msg-b", CorrelationId = "corr-b", CausationId = "cause-b", IdempotencyId = "idem-b" };
        AssertOneAcceptedOneRejected(
            Task.WhenAll(Task.Run(() => ingress.Consume(first, now)), Task.Run(() => ingress.Consume(second, now))).GetAwaiter().GetResult().Select(x => (int)x.State),
            (int)TA.ApplicationEventIngressState.AcceptedEvidence,
            (int)TA.ApplicationEventIngressState.Rejected,
            "TRADING_ORDERING_RACE");
    }

    private static void ProviderOrderingRace()
    {
        var now = DateTimeOffset.UtcNow;
        var ingress = new PA.GovernedApplicationEventIngress();
        var first = new PA.GovernedApplicationEventEnvelope(
            "evt-order-p-a", "test", GA.TradingGuardianManifest.Current.ApplicationId, PA.FSAPMAManifest.Current.ApplicationId,
            "schema", "1.0", "msg-a", "corr-a", "cause-a", "idem-a", PA.ApplicationEventTruth.NonAuthoritativeEvidence,
            PA.ApplicationEventRelation.None, null, "same-key", 1, now.AddSeconds(-1), "evidence", new string('A', 64));
        var second = first with { EventId = "evt-order-p-b", MessageId = "msg-b", CorrelationId = "corr-b", CausationId = "cause-b", IdempotencyId = "idem-b" };
        AssertOneAcceptedOneRejected(
            Task.WhenAll(Task.Run(() => ingress.Consume(first, now)), Task.Run(() => ingress.Consume(second, now))).GetAwaiter().GetResult().Select(x => (int)x.State),
            (int)PA.ApplicationEventIngressState.AcceptedEvidence,
            (int)PA.ApplicationEventIngressState.Rejected,
            "FSAPMA_ORDERING_RACE");
    }

    private static void GuardianOrderingRace()
    {
        var now = DateTimeOffset.UtcNow;
        var ingress = new GA.GovernedApplicationEventIngress();
        var first = new GA.GovernedApplicationEventEnvelope(
            "evt-order-g-a", "test", TA.TradingManifest.Current.ApplicationId, GA.TradingGuardianManifest.Current.ApplicationId,
            "schema", "1.0", "msg-a", "corr-a", "cause-a", "idem-a", GA.ApplicationEventTruth.NonAuthoritativeEvidence,
            GA.ApplicationEventRelation.None, null, "same-key", 1, now.AddSeconds(-1), "evidence", new string('A', 64));
        var second = first with { EventId = "evt-order-g-b", MessageId = "msg-b", CorrelationId = "corr-b", CausationId = "cause-b", IdempotencyId = "idem-b" };
        AssertOneAcceptedOneRejected(
            Task.WhenAll(Task.Run(() => ingress.Consume(first, now)), Task.Run(() => ingress.Consume(second, now))).GetAwaiter().GetResult().Select(x => (int)x.State),
            (int)GA.ApplicationEventIngressState.AcceptedEvidence,
            (int)GA.ApplicationEventIngressState.Rejected,
            "GUARDIAN_ORDERING_RACE");
    }

    private static void AssertOneAcceptedOneRejected(IEnumerable<int> states, int accepted, int rejected, string name)
    {
        var values = states.ToArray();
        if (values.Count(x => x == accepted) != 1 || values.Count(x => x == rejected) != 1)
            throw new InvalidOperationException($"{name}_FAILED");
    }
}
