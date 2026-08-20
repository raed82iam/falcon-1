namespace Falcon.FSATS.FSAPMA.Application;

public sealed class RestartAwareOperationalDataDeliveryService
{
    private readonly GovernedOperationalDataDeliveryService _inner;
    private readonly FSAPMARestartPlan _restart;

    public RestartAwareOperationalDataDeliveryService(GovernedOperationalDataDeliveryService inner, FSAPMARestartPlan restart)
    {
        _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        _restart = restart ?? throw new ArgumentNullException(nameof(restart));
    }

    public async ValueTask<OperationalDataDeliveryResult> DeliverAsync(OperationalDataDeliveryEnvelope envelope, DateTimeOffset now, CancellationToken cancellationToken)
    {
        if (!_restart.Accepted)
            return new(OperationalDataDeliveryState.Rejected, envelope?.Projection?.ObservationId.Value ?? "unknown-observation", envelope?.ConsumerApplicationId ?? "UNKNOWN", "FSAPMA_RESTART_STATE_NOT_TRUSTED", envelope?.CorrelationId ?? "unknown-correlation", now, envelope?.Projection?.ProviderRouteNamespace ?? string.Empty);

        var validation = GovernedOperationalDataGuards.Validate(envelope, now);
        if (!validation.Accepted) return await _inner.DeliverAsync(envelope, now, cancellationToken).ConfigureAwait(false);

        var key = GovernedOperationalDataGuards.IdempotencyScopeKey(envelope);
        if (_restart.DeliveryTombstones.TryGetValue(key, out var prior))
        {
            var fingerprint = GovernedOperationalDataGuards.Fingerprint(envelope);
            if (!StringComparer.Ordinal.Equals(prior.Fingerprint, fingerprint))
                return new(OperationalDataDeliveryState.Rejected, envelope.Projection.ObservationId.Value, envelope.ConsumerApplicationId, "RESTART_IDEMPOTENCY_CONFLICT", envelope.CorrelationId, now, envelope.Projection.ProviderRouteNamespace);
            return FSAPMARestartReconstructor.ReplayView(prior);
        }

        return await _inner.DeliverAsync(envelope, now, cancellationToken).ConfigureAwait(false);
    }
}
