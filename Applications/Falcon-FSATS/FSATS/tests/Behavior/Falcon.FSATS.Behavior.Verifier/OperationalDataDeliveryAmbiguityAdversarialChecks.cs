using PA = Falcon.FSATS.FSAPMA.Application;
using PC = Falcon.FSATS.FSAPMA.Contracts;

internal static class OperationalDataDeliveryAmbiguityAdversarialChecks
{
    internal static void Run()
    {
        CancellationDoesNotFabricateRejection();
        RouteFailureDoesNotFabricateRejectionOrRetry();
        IdempotencyScopeRejectsDelimiterCollision();
    }

    private static PA.OperationalDataDeliveryEnvelope Envelope(string idempotencyId = "idem")
    {
        var now = DateTimeOffset.UtcNow;
        var projection = new PC.OperationalDataProjection(
            new PC.ObservationId("obs"),
            new PC.ProviderId("ALPACA"),
            new PC.ProducerInstrumentId("ALPACA", "AAPL"),
            new PC.DataProductId("last-price"),
            100m,
            now.AddSeconds(-1),
            now.AddMilliseconds(-500),
            PC.DataTruthState.Current,
            "provenance",
            "1.0",
            new PC.ProviderAccountId("provider-account"),
            "PAPER",
            "MARKET_DATA",
            "credential-ref");

        return new PA.OperationalDataDeliveryEnvelope(
            "message", "schema", "1.0", PA.FSAPMAManifest.Current.ApplicationId, "FSATS-TRADING",
            "authority", "provenance", "correlation", "causation", idempotencyId, "attempt", "retry",
            PA.OperationalDataTrafficTruth.Operational, now.AddSeconds(-1), now.AddMinutes(1), TimeSpan.FromMinutes(1),
            null, "evidence", projection);
    }

    private static void CancellationDoesNotFabricateRejection()
    {
        var route = new CancellingRoute();
        var service = new PA.GovernedOperationalDataDeliveryService(route);
        var envelope = Envelope("cancel-idem");
        var result = service.DeliverAsync(envelope, DateTimeOffset.UtcNow, CancellationToken.None).AsTask().GetAwaiter().GetResult();
        if (result.State != PA.OperationalDataDeliveryState.DeliveryOutcomeUnknown || result.ReasonCode != "DELIVERY_ROUTE_CANCELLATION_AMBIGUOUS")
            throw new InvalidOperationException("DATA_DELIVERY_CANCELLATION_FALSE_REJECTION");
    }

    private static void RouteFailureDoesNotFabricateRejectionOrRetry()
    {
        var route = new ThrowingRoute();
        var service = new PA.GovernedOperationalDataDeliveryService(route);
        var envelope = Envelope("failure-idem");
        var first = service.DeliverAsync(envelope, DateTimeOffset.UtcNow, CancellationToken.None).AsTask().GetAwaiter().GetResult();
        var duplicate = service.DeliverAsync(envelope with { DeliveryAttemptId = "attempt-2" }, DateTimeOffset.UtcNow, CancellationToken.None).AsTask().GetAwaiter().GetResult();
        if (first.State != PA.OperationalDataDeliveryState.DeliveryOutcomeUnknown ||
            duplicate.State != PA.OperationalDataDeliveryState.DeliveryOutcomeUnknown ||
            !duplicate.ReasonCode.StartsWith("IDEMPOTENT_DUPLICATE_OF_UNKNOWN:", StringComparison.Ordinal) ||
            route.Calls != 1)
            throw new InvalidOperationException("DATA_DELIVERY_AMBIGUOUS_OUTCOME_RETRIED_OR_MISCLASSIFIED");
    }

    private static void IdempotencyScopeRejectsDelimiterCollision()
    {
        var a = Envelope("B|C") with { ConsumerApplicationId = "A" };
        var b = Envelope("C") with { ConsumerApplicationId = "A|B" };
        if (StringComparer.Ordinal.Equals(PA.GovernedOperationalDataGuards.IdempotencyScopeKey(a), PA.GovernedOperationalDataGuards.IdempotencyScopeKey(b)))
            throw new InvalidOperationException("DATA_DELIVERY_IDEMPOTENCY_DELIMITER_COLLISION");
    }

    private sealed class CancellingRoute : PA.IGovernedOperationalDataRoutePort
    {
        public ValueTask<PA.OperationalDataDeliveryResult> DeliverAsync(PA.OperationalDataDeliveryEnvelope envelope, CancellationToken cancellationToken)
            => ValueTask.FromException<PA.OperationalDataDeliveryResult>(new OperationCanceledException("ambiguous cancellation"));
    }

    private sealed class ThrowingRoute : PA.IGovernedOperationalDataRoutePort
    {
        public int Calls { get; private set; }
        public ValueTask<PA.OperationalDataDeliveryResult> DeliverAsync(PA.OperationalDataDeliveryEnvelope envelope, CancellationToken cancellationToken)
        {
            Calls++;
            return ValueTask.FromException<PA.OperationalDataDeliveryResult>(new IOException("ambiguous route failure"));
        }
    }
}
