using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using Falcon.FSATS.FSAPMA.Contracts;

namespace Falcon.FSATS.FSAPMA.Application;

public enum OperationalDataTrafficTruth { Operational = 1, Replay = 2, Test = 3, Simulation = 4 }
public enum OperationalDataDeliveryState { DeliveredCurrent = 1, DeliveredDegraded = 2, Duplicate = 3, Rejected = 4, DeliveryOutcomeUnknown = 5 }

public sealed record OperationalDataDeliveryEnvelope(string MessageId, string SchemaId, string SchemaVersion, string ProducerApplicationId, string ConsumerApplicationId, string AuthorityReference, string ProvenanceReference, string CorrelationId, string CausationId, string IdempotencyId, string DeliveryAttemptId, string RetryLineageId, OperationalDataTrafficTruth TrafficTruth, DateTimeOffset CreatedAt, DateTimeOffset? ExpiresAt, TimeSpan MaximumFreshness, string? CorrectionOfObservationId, string EvidenceReference, OperationalDataProjection Projection);
public sealed record OperationalDataDeliveryResult(OperationalDataDeliveryState State, string ObservationId, string ConsumerApplicationId, string ReasonCode, string CorrelationId, DateTimeOffset EffectiveAt, string ProviderRouteNamespace = "");

public interface IGovernedOperationalDataRoutePort
{
    ValueTask<OperationalDataDeliveryResult> DeliverAsync(OperationalDataDeliveryEnvelope envelope, CancellationToken cancellationToken);
}

public sealed record OperationalDataValidation(bool Accepted, bool Current, string ReasonCode)
{
    public static OperationalDataValidation Pass(bool current, string reason) => new(true, current, reason);
    public static OperationalDataValidation Reject(string reason) => new(false, false, reason);
}

public static class GovernedOperationalDataGuards
{
    public static OperationalDataValidation Validate(OperationalDataDeliveryEnvelope? envelope, DateTimeOffset now)
    {
        if (envelope is null) return OperationalDataValidation.Reject("NULL_OPERATIONAL_DATA_ENVELOPE");
        if (envelope.Projection is null) return OperationalDataValidation.Reject("NULL_OPERATIONAL_DATA_PROJECTION");
        if (envelope.TrafficTruth != OperationalDataTrafficTruth.Operational) return OperationalDataValidation.Reject("NON_OPERATIONAL_DATA_TRAFFIC");
        if (!StringComparer.Ordinal.Equals(envelope.ProducerApplicationId, FSAPMAManifest.Current.ApplicationId)) return OperationalDataValidation.Reject("PRODUCER_APPLICATION_MISMATCH");
        if (string.IsNullOrWhiteSpace(envelope.ConsumerApplicationId)) return OperationalDataValidation.Reject("CONSUMER_APPLICATION_REQUIRED");
        if (!StringComparer.Ordinal.Equals(envelope.SchemaVersion, envelope.Projection.SchemaVersion)) return OperationalDataValidation.Reject("SCHEMA_VERSION_MISMATCH");
        if (!envelope.Projection.HasCompleteProviderRouteIdentity) return OperationalDataValidation.Reject("PROVIDER_ROUTE_IDENTITY_INCOMPLETE");
        if (!StringComparer.OrdinalIgnoreCase.Equals(envelope.Projection.Provider.Value.Trim(), envelope.Projection.Instrument.Provider.Trim())) return OperationalDataValidation.Reject("PROVIDER_INSTRUMENT_BINDING_MISMATCH");
        if (StringComparer.Ordinal.Equals(envelope.CorrelationId, envelope.CausationId)) return OperationalDataValidation.Reject("CORRELATION_CAUSATION_COLLISION");
        if (string.IsNullOrWhiteSpace(envelope.MessageId) || string.IsNullOrWhiteSpace(envelope.SchemaId) || string.IsNullOrWhiteSpace(envelope.SchemaVersion) || string.IsNullOrWhiteSpace(envelope.AuthorityReference) || string.IsNullOrWhiteSpace(envelope.ProvenanceReference) || string.IsNullOrWhiteSpace(envelope.IdempotencyId) || string.IsNullOrWhiteSpace(envelope.DeliveryAttemptId) || string.IsNullOrWhiteSpace(envelope.RetryLineageId) || string.IsNullOrWhiteSpace(envelope.EvidenceReference)) return OperationalDataValidation.Reject("INCOMPLETE_OPERATIONAL_DATA_ENVELOPE");
        if (envelope.CreatedAt == default || envelope.CreatedAt > now) return OperationalDataValidation.Reject("INVALID_OPERATIONAL_DATA_MESSAGE_TIME");
        if (envelope.ExpiresAt is { } expiry && expiry <= now) return OperationalDataValidation.Reject("OPERATIONAL_DATA_MESSAGE_EXPIRED");
        if (envelope.MaximumFreshness <= TimeSpan.Zero) return OperationalDataValidation.Reject("INVALID_FRESHNESS_WINDOW");
        if (envelope.Projection.ObservedAt == default || envelope.Projection.ReceivedAt == default || envelope.Projection.ObservedAt > envelope.Projection.ReceivedAt || envelope.Projection.ReceivedAt > now) return OperationalDataValidation.Reject("INVALID_OBSERVATION_TIME_ORDER");
        if (!StringComparer.Ordinal.Equals(envelope.ProvenanceReference, envelope.Projection.Provenance)) return OperationalDataValidation.Reject("PROVENANCE_BINDING_MISMATCH");

        if (envelope.Projection.Truth == DataTruthState.Correction)
        {
            if (string.IsNullOrWhiteSpace(envelope.CorrectionOfObservationId) || StringComparer.Ordinal.Equals(envelope.CorrectionOfObservationId, envelope.Projection.ObservationId.Value)) return OperationalDataValidation.Reject("CORRECTION_LINEAGE_INVALID");
        }
        else if (envelope.CorrectionOfObservationId is not null) return OperationalDataValidation.Reject("UNEXPECTED_CORRECTION_LINEAGE");

        var freshByTime = now - envelope.Projection.ObservedAt <= envelope.MaximumFreshness;
        var semanticallyCurrent = envelope.Projection.Truth == DataTruthState.Current;
        if (freshByTime && semanticallyCurrent) return OperationalDataValidation.Pass(true, "OPERATIONAL_DATA_CURRENT");
        return envelope.Projection.Truth switch
        {
            DataTruthState.Stale or DataTruthState.Conflicted or DataTruthState.Unknown or DataTruthState.Unavailable or DataTruthState.Correction => OperationalDataValidation.Pass(false, "OPERATIONAL_DATA_EXPLICITLY_DEGRADED"),
            _ when !freshByTime => OperationalDataValidation.Pass(false, "OPERATIONAL_DATA_STALE_BY_TIME"),
            _ => OperationalDataValidation.Reject("OPERATIONAL_DATA_TRUTH_INVALID")
        };
    }

    public static string IdempotencyScopeKey(OperationalDataDeliveryEnvelope envelope)
        => string.Join('|', Part(envelope.ConsumerApplicationId), envelope.Projection.ProviderRouteNamespace, Part(envelope.IdempotencyId.Trim()));

    public static string Fingerprint(OperationalDataDeliveryEnvelope envelope)
    {
        var value = string.Join('|',
            Part(envelope.MessageId), Part(envelope.SchemaId), Part(envelope.SchemaVersion), Part(envelope.ProducerApplicationId), Part(envelope.ConsumerApplicationId),
            Part(envelope.AuthorityReference), Part(envelope.ProvenanceReference), Part(envelope.CorrelationId), Part(envelope.CausationId), Part(envelope.IdempotencyId),
            Part(envelope.Projection.ObservationId.Value), envelope.Projection.ProviderRouteNamespace,
            Part(envelope.Projection.Instrument.Provider), Part(envelope.Projection.Instrument.Value), Part(envelope.Projection.Product.Value),
            envelope.Projection.Value, envelope.Projection.ObservedAt.ToUniversalTime().ToString("O"), envelope.Projection.ReceivedAt.ToUniversalTime().ToString("O"),
            envelope.Projection.Truth, Part(envelope.CorrectionOfObservationId ?? string.Empty));
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
    }

    private static string Part(string value) => Uri.EscapeDataString(value ?? string.Empty);
}

public sealed class GovernedOperationalDataDeliveryService
{
    private readonly IGovernedOperationalDataRoutePort _route;
    private readonly ConcurrentDictionary<string, (string Fingerprint, OperationalDataDeliveryResult Result)> _idempotency = new(StringComparer.Ordinal);
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _idempotencyGates = new(StringComparer.Ordinal);
    private readonly object _capacityGate = new();
    private readonly HashSet<string> _admittedKeys = new(StringComparer.Ordinal);
    private readonly int _maximumTrackedDeliveries;

    public GovernedOperationalDataDeliveryService(IGovernedOperationalDataRoutePort route, int maximumTrackedDeliveries = 100_000)
    {
        _route = route ?? throw new ArgumentNullException(nameof(route));
        if (maximumTrackedDeliveries <= 0) throw new ArgumentOutOfRangeException(nameof(maximumTrackedDeliveries));
        _maximumTrackedDeliveries = maximumTrackedDeliveries;
    }

    public async ValueTask<OperationalDataDeliveryResult> DeliverAsync(OperationalDataDeliveryEnvelope envelope, DateTimeOffset now, CancellationToken cancellationToken)
    {
        var validation = GovernedOperationalDataGuards.Validate(envelope, now);
        if (!validation.Accepted) return Rejected(envelope?.Projection?.ObservationId.Value ?? "unknown-observation", envelope?.ConsumerApplicationId ?? "UNKNOWN", validation.ReasonCode, envelope?.CorrelationId ?? "unknown-correlation", now, envelope?.Projection?.ProviderRouteNamespace ?? string.Empty);

        var fingerprint = GovernedOperationalDataGuards.Fingerprint(envelope);
        var idempotencyKey = GovernedOperationalDataGuards.IdempotencyScopeKey(envelope);
        if (_idempotency.TryGetValue(idempotencyKey, out var existing))
        {
            if (!StringComparer.Ordinal.Equals(existing.Fingerprint, fingerprint)) return Rejected(envelope.Projection.ObservationId.Value, envelope.ConsumerApplicationId, "IDEMPOTENCY_CONFLICT", envelope.CorrelationId, now, envelope.Projection.ProviderRouteNamespace);
            return DuplicateView(existing.Result);
        }
        if (!TryAdmitKey(idempotencyKey)) return Rejected(envelope.Projection.ObservationId.Value, envelope.ConsumerApplicationId, "DELIVERY_IDEMPOTENCY_CAPACITY_EXHAUSTED_FAIL_CLOSED", envelope.CorrelationId, now, envelope.Projection.ProviderRouteNamespace);

        var gate = _idempotencyGates.GetOrAdd(idempotencyKey, static _ => new SemaphoreSlim(1, 1));
        await gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (_idempotency.TryGetValue(idempotencyKey, out var prior))
            {
                if (!StringComparer.Ordinal.Equals(prior.Fingerprint, fingerprint)) return Rejected(envelope.Projection.ObservationId.Value, envelope.ConsumerApplicationId, "IDEMPOTENCY_CONFLICT", envelope.CorrelationId, now, envelope.Projection.ProviderRouteNamespace);
                return DuplicateView(prior.Result);
            }

            var result = await DispatchOnceAsync(envelope, validation, now, cancellationToken).ConfigureAwait(false);
            _idempotency.TryAdd(idempotencyKey, (fingerprint, result));
            return result;
        }
        finally { gate.Release(); }
    }

    private bool TryAdmitKey(string key)
    {
        lock (_capacityGate)
        {
            if (_admittedKeys.Contains(key)) return true;
            if (_admittedKeys.Count >= _maximumTrackedDeliveries) return false;
            return _admittedKeys.Add(key);
        }
    }

    private async Task<OperationalDataDeliveryResult> DispatchOnceAsync(OperationalDataDeliveryEnvelope envelope, OperationalDataValidation validation, DateTimeOffset now, CancellationToken cancellationToken)
    {
        OperationalDataDeliveryResult? routeResult;
        try { routeResult = await _route.DeliverAsync(envelope, cancellationToken).ConfigureAwait(false); }
        catch (OperationCanceledException) { return Unknown(envelope, "DELIVERY_ROUTE_CANCELLATION_AMBIGUOUS", now); }
        catch (Exception ex) { return Unknown(envelope, $"DELIVERY_ROUTE_FAILURE_AMBIGUOUS:{ex.GetType().Name}", now); }

        if (routeResult is null) return Unknown(envelope, "NULL_DELIVERY_OUTCOME_AMBIGUOUS", now);
        if (!StringComparer.Ordinal.Equals(routeResult.ObservationId, envelope.Projection.ObservationId.Value) ||
            !StringComparer.Ordinal.Equals(routeResult.ConsumerApplicationId, envelope.ConsumerApplicationId) ||
            !StringComparer.Ordinal.Equals(routeResult.CorrelationId, envelope.CorrelationId) ||
            !StringComparer.Ordinal.Equals(routeResult.ProviderRouteNamespace, envelope.Projection.ProviderRouteNamespace))
            return Unknown(envelope, "DELIVERY_OUTCOME_BINDING_MISMATCH_AMBIGUOUS", now);
        if (string.IsNullOrWhiteSpace(routeResult.ReasonCode)) return Unknown(envelope, "DELIVERY_OUTCOME_REASON_MISSING_AMBIGUOUS", now);

        return routeResult.State switch
        {
            OperationalDataDeliveryState.Rejected => routeResult,
            OperationalDataDeliveryState.Duplicate => routeResult,
            OperationalDataDeliveryState.DeliveryOutcomeUnknown => routeResult,
            OperationalDataDeliveryState.DeliveredDegraded => routeResult,
            OperationalDataDeliveryState.DeliveredCurrent when validation.Current => routeResult with { ReasonCode = validation.ReasonCode },
            OperationalDataDeliveryState.DeliveredCurrent => routeResult with { State = OperationalDataDeliveryState.DeliveredDegraded, ReasonCode = validation.ReasonCode },
            _ => Unknown(envelope, "DELIVERY_OUTCOME_STATE_INVALID_AMBIGUOUS", now)
        };
    }

    private static OperationalDataDeliveryResult DuplicateView(OperationalDataDeliveryResult prior)
        => prior.State switch
        {
            OperationalDataDeliveryState.Rejected => prior with { ReasonCode = $"IDEMPOTENT_DUPLICATE_OF_REJECTED:{prior.ReasonCode}" },
            OperationalDataDeliveryState.DeliveryOutcomeUnknown => prior with { ReasonCode = $"IDEMPOTENT_DUPLICATE_OF_UNKNOWN:{prior.ReasonCode}" },
            OperationalDataDeliveryState.DeliveredDegraded => prior with { ReasonCode = $"IDEMPOTENT_DUPLICATE_OF_DEGRADED:{prior.ReasonCode}" },
            _ => prior with { State = OperationalDataDeliveryState.Duplicate, ReasonCode = "IDEMPOTENT_DUPLICATE" }
        };

    private static OperationalDataDeliveryResult Unknown(OperationalDataDeliveryEnvelope envelope, string reasonCode, DateTimeOffset effectiveAt)
        => new(OperationalDataDeliveryState.DeliveryOutcomeUnknown, envelope.Projection.ObservationId.Value, envelope.ConsumerApplicationId, reasonCode, envelope.CorrelationId, effectiveAt, envelope.Projection.ProviderRouteNamespace);

    private static OperationalDataDeliveryResult Rejected(string observationId, string consumerApplicationId, string reasonCode, string correlationId, DateTimeOffset effectiveAt, string providerRouteNamespace)
        => new(OperationalDataDeliveryState.Rejected, observationId, consumerApplicationId, reasonCode, correlationId, effectiveAt, providerRouteNamespace);
}
