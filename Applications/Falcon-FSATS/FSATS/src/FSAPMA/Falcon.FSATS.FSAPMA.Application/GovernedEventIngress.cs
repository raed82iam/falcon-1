using System.Collections.Concurrent;

namespace Falcon.FSATS.FSAPMA.Application;

public enum ApplicationEventTruth { AuthoritativeOperational = 1, Replay = 2, Test = 3, Simulation = 4, NonAuthoritativeEvidence = 5 }
public enum ApplicationEventRelation { None = 1, ReplayOf = 2, CorrectionOf = 3, Supersedes = 4 }
public enum ApplicationEventIngressState { AcceptedOperational = 1, AcceptedEvidence = 2, Duplicate = 3, Rejected = 4 }

public sealed record GovernedApplicationEventEnvelope(string EventId, string EventType, string PublisherApplicationId, string SubscriberApplicationId, string SchemaId, string SchemaVersion, string MessageId, string CorrelationId, string CausationId, string IdempotencyId, ApplicationEventTruth Truth, ApplicationEventRelation Relation, string? RelatedEventId, string? OrderingKey, long SequenceNumber, DateTimeOffset ObservedAt, string EvidenceReference, string PayloadSha256, string ScopeKey = "APPLICATION");
public sealed record ApplicationEventIngressResult(ApplicationEventIngressState State, string EventId, string ReasonCode);

public sealed class GovernedApplicationEventIngress
{
    private readonly object _stateGate = new();
    private readonly ConcurrentDictionary<string, GovernedApplicationEventEnvelope> _events = new(StringComparer.Ordinal);
    private readonly ConcurrentDictionary<string, long> _ordering = new(StringComparer.Ordinal);
    private readonly int _maximumTrackedEvents;

    public GovernedApplicationEventIngress(int maximumTrackedEvents = 100_000)
    {
        if (maximumTrackedEvents <= 0) throw new ArgumentOutOfRangeException(nameof(maximumTrackedEvents));
        _maximumTrackedEvents = maximumTrackedEvents;
    }

    public ApplicationEventIngressResult Consume(GovernedApplicationEventEnvelope? envelope, DateTimeOffset now)
    {
        var validation = Validate(envelope, now);
        if (validation is not null) return new(ApplicationEventIngressState.Rejected, envelope?.EventId ?? "unknown-event", validation);
        var acceptedEnvelope = envelope!;
        var canonicalScope = CanonicalizeScope(acceptedEnvelope.ScopeKey);
        var eventKey = ScopedKey(canonicalScope, acceptedEnvelope.EventId);
        lock (_stateGate)
        {
            if (_events.TryGetValue(eventKey, out var prior))
            {
                var same = StringComparer.Ordinal.Equals(prior.IdempotencyId, acceptedEnvelope.IdempotencyId) && StringComparer.Ordinal.Equals(prior.PayloadSha256, acceptedEnvelope.PayloadSha256) && prior.Truth == acceptedEnvelope.Truth && prior.Relation == acceptedEnvelope.Relation && StringComparer.Ordinal.Equals(prior.RelatedEventId, acceptedEnvelope.RelatedEventId);
                return same ? new(ApplicationEventIngressState.Duplicate, acceptedEnvelope.EventId, "EVENT_DUPLICATE_IDEMPOTENT") : new(ApplicationEventIngressState.Rejected, acceptedEnvelope.EventId, "EVENT_DUPLICATE_IDENTITY_CONFLICT");
            }
            if (_events.Count >= _maximumTrackedEvents)
                return new(ApplicationEventIngressState.Rejected, acceptedEnvelope.EventId, "EVENT_IDEMPOTENCY_CAPACITY_EXHAUSTED_FAIL_CLOSED");
            if (acceptedEnvelope.OrderingKey is not null)
            {
                var orderingKey = ScopedKey(canonicalScope, acceptedEnvelope.OrderingKey);
                if (_ordering.TryGetValue(orderingKey, out var last) && acceptedEnvelope.SequenceNumber <= last) return new(ApplicationEventIngressState.Rejected, acceptedEnvelope.EventId, "EVENT_SEQUENCE_VIOLATION");
                _ordering[orderingKey] = acceptedEnvelope.SequenceNumber;
            }
            _events[eventKey] = acceptedEnvelope;
            return acceptedEnvelope.Truth == ApplicationEventTruth.AuthoritativeOperational ? new(ApplicationEventIngressState.AcceptedOperational, acceptedEnvelope.EventId, "EVENT_OPERATIONAL_ACCEPTED") : new(ApplicationEventIngressState.AcceptedEvidence, acceptedEnvelope.EventId, "EVENT_NON_OPERATIONAL_EVIDENCE_ACCEPTED");
        }
    }

    public static string ProviderRouteScope(string providerId, string providerAccountId, string environment, string serviceRole)
        => $"PROVIDER_ROUTE|{Part(providerId, upper: true)}|{Part(providerAccountId)}|{Part(environment, upper: true)}|{Part(serviceRole, upper: true)}";

    private static string? Validate(GovernedApplicationEventEnvelope? envelope, DateTimeOffset now)
    {
        if (envelope is null) return "NULL_EVENT_ENVELOPE";
        if (!StringComparer.Ordinal.Equals(envelope.SubscriberApplicationId, FSAPMAManifest.Current.ApplicationId)) return "EVENT_SUBSCRIBER_MISMATCH";
        if (string.IsNullOrWhiteSpace(envelope.EventId) || string.IsNullOrWhiteSpace(envelope.EventType) || string.IsNullOrWhiteSpace(envelope.PublisherApplicationId) || string.IsNullOrWhiteSpace(envelope.SchemaId) || string.IsNullOrWhiteSpace(envelope.SchemaVersion) || string.IsNullOrWhiteSpace(envelope.MessageId) || string.IsNullOrWhiteSpace(envelope.CorrelationId) || string.IsNullOrWhiteSpace(envelope.CausationId) || string.IsNullOrWhiteSpace(envelope.IdempotencyId) || string.IsNullOrWhiteSpace(envelope.EvidenceReference) || string.IsNullOrWhiteSpace(envelope.ScopeKey)) return "INCOMPLETE_EVENT_ENVELOPE";
        if (!IsSupportedScope(envelope.ScopeKey)) return "EVENT_SCOPE_INVALID";
        if (StringComparer.Ordinal.Equals(envelope.CorrelationId, envelope.CausationId)) return "CORRELATION_CAUSATION_COLLISION";
        if (!IsSha256(envelope.PayloadSha256)) return "EVENT_PAYLOAD_DIGEST_INVALID";
        if (envelope.ObservedAt == default || envelope.ObservedAt > now) return "EVENT_OBSERVATION_TIME_INVALID";
        if (envelope.SequenceNumber < 0) return "EVENT_SEQUENCE_INVALID";
        if (envelope.OrderingKey is null && envelope.SequenceNumber != 0) return "EVENT_SEQUENCE_UNEXPECTED";
        if (envelope.OrderingKey is not null && envelope.SequenceNumber == 0) return "EVENT_SEQUENCE_REQUIRED";
        if (envelope.Relation == ApplicationEventRelation.None && envelope.RelatedEventId is not null) return "EVENT_RELATION_TARGET_UNEXPECTED";
        if (envelope.Relation != ApplicationEventRelation.None && string.IsNullOrWhiteSpace(envelope.RelatedEventId)) return "EVENT_RELATION_TARGET_REQUIRED";
        if (envelope.Truth == ApplicationEventTruth.Replay && envelope.Relation != ApplicationEventRelation.ReplayOf) return "EVENT_REPLAY_RELATION_REQUIRED";
        if (envelope.Truth == ApplicationEventTruth.AuthoritativeOperational && envelope.Relation == ApplicationEventRelation.ReplayOf) return "EVENT_REPLAY_OPERATIONAL_ESCALATION_REJECTED";
        return null;
    }

    private static bool IsSupportedScope(string scope)
    {
        try { _ = CanonicalizeScope(scope); return true; }
        catch (ArgumentException) { return false; }
        catch (UriFormatException) { return false; }
    }

    private static string CanonicalizeScope(string scope)
    {
        var trimmed = scope.Trim();
        if (trimmed == "APPLICATION") return "APPLICATION";
        var parts = trimmed.Split('|');
        if (parts.Length == 5 && parts[0] == "PROVIDER_ROUTE") return ProviderRouteScope(Decode(parts[1]), Decode(parts[2]), Decode(parts[3]), Decode(parts[4]));
        throw new ArgumentException("EVENT_SCOPE_INVALID", nameof(scope));
    }

    private static string Part(string value, bool upper = false)
    {
        var normalized = RequireScopePart(value);
        if (upper) normalized = normalized.ToUpperInvariant();
        return Uri.EscapeDataString(normalized);
    }

    private static string Decode(string value) => Uri.UnescapeDataString(RequireScopePart(value));
    private static string ScopedKey(string scopeKey, string value) => $"{scopeKey}|{Uri.EscapeDataString(value.Trim())}";
    private static string RequireScopePart(string value) { if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("EVENT_SCOPE_PART_REQUIRED"); return value.Trim(); }
    private static bool IsSha256(string? value) => value is { Length: 64 } && value.All(c => c is >= '0' and <= '9' or >= 'A' and <= 'F');
}
