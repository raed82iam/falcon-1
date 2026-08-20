namespace Falcon.FSATS.FSAPMA.Domain;

public enum StreamContinuityState
{
    Initializing,
    Current,
    DuplicateObserved,
    GapDetected,
    ReconciliationRequired,
    Stale,
    Closed
}

public sealed record ProviderStreamSessionIdentity
{
    public ProviderRouteIdentity Route { get; }
    public string EndpointId { get; }
    public string SessionId { get; }
    public string SubscriptionKey { get; }

    public ProviderStreamSessionIdentity(ProviderRouteIdentity route, string endpointId, string sessionId, string subscriptionKey)
    {
        Route = route ?? throw new ArgumentNullException(nameof(route));
        EndpointId = Require(endpointId, nameof(endpointId));
        SessionId = Require(sessionId, nameof(sessionId));
        SubscriptionKey = Require(subscriptionKey, nameof(subscriptionKey));
    }

    public string CanonicalKey => string.Join('|', Route.NamespaceKey, Part(EndpointId), Part(SessionId), Part(SubscriptionKey));

    private static string Require(string value, string parameter)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("STREAM_SESSION_IDENTITY_REQUIRED", parameter);
        return value.Trim();
    }

    private static string Part(string value) => Uri.EscapeDataString(value);
}

public sealed record StreamContinuityObservation(
    StreamContinuityState State,
    long? LastAcceptedSequence,
    long? MissingFromSequence,
    long? MissingToSequence,
    string ReasonCode,
    string EvidenceReference,
    DateTimeOffset ObservedAt);

public sealed class ProviderStreamContinuityTracker
{
    private readonly object _gate = new();
    private readonly ProviderStreamSessionIdentity _identity;
    private long? _lastAcceptedSequence;
    private StreamContinuityState _state = StreamContinuityState.Initializing;
    private string _lastEvidence = "INITIALIZING";
    private DateTimeOffset _lastObservedAt;

    public ProviderStreamContinuityTracker(ProviderStreamSessionIdentity identity)
        => _identity = identity ?? throw new ArgumentNullException(nameof(identity));

    public ProviderStreamSessionIdentity Identity => _identity;

    public StreamContinuityObservation Connected(bool isReconnect, string evidenceReference, DateTimeOffset observedAt)
    {
        ValidateEvidence(evidenceReference, observedAt);
        lock (_gate)
        {
            _lastEvidence = evidenceReference.Trim();
            _lastObservedAt = observedAt;
            if (isReconnect)
            {
                _state = StreamContinuityState.ReconciliationRequired;
                return Snapshot("STREAM_RECONNECTED_CONTINUITY_NOT_PROVEN");
            }

            _state = StreamContinuityState.Initializing;
            _lastAcceptedSequence = null;
            return Snapshot("STREAM_CONNECTED_AWAITING_FIRST_OBSERVATION");
        }
    }

    public StreamContinuityObservation ObserveSequenced(long sequence, DateTimeOffset sourceTime, DateTimeOffset receiveTime, string evidenceReference)
    {
        ValidateEvidence(evidenceReference, receiveTime);
        if (sequence < 0) throw new ArgumentOutOfRangeException(nameof(sequence));
        if (sourceTime == default || sourceTime > receiveTime) throw new ArgumentException("STREAM_EVENT_TIME_INVALID");

        lock (_gate)
        {
            if (_state == StreamContinuityState.Closed)
                return Snapshot("STREAM_CLOSED_EVENT_REJECTED");
            if (_state == StreamContinuityState.ReconciliationRequired || _state == StreamContinuityState.GapDetected)
                return Snapshot("STREAM_RECONCILIATION_REQUIRED_BEFORE_NEW_SEQUENCE_TRUST");

            _lastEvidence = evidenceReference.Trim();
            _lastObservedAt = receiveTime;

            if (_lastAcceptedSequence is null)
            {
                _lastAcceptedSequence = sequence;
                _state = StreamContinuityState.Current;
                return Snapshot("STREAM_SEQUENCE_BASELINE_ESTABLISHED");
            }

            var last = _lastAcceptedSequence.Value;
            if (sequence == last)
            {
                _state = StreamContinuityState.DuplicateObserved;
                return Snapshot("STREAM_DUPLICATE_SEQUENCE");
            }

            if (sequence < last)
            {
                _state = StreamContinuityState.ReconciliationRequired;
                return Snapshot("STREAM_OUT_OF_ORDER_SEQUENCE_RECONCILIATION_REQUIRED");
            }

            if (last == long.MaxValue)
            {
                _state = StreamContinuityState.ReconciliationRequired;
                return Snapshot("STREAM_SEQUENCE_SPACE_EXHAUSTED_RECONCILIATION_REQUIRED");
            }

            var expected = last + 1;
            if (sequence != expected)
            {
                _state = StreamContinuityState.GapDetected;
                return new(
                    _state,
                    _lastAcceptedSequence,
                    expected,
                    sequence > expected ? sequence - 1 : null,
                    "STREAM_SEQUENCE_GAP_DETECTED",
                    _lastEvidence,
                    _lastObservedAt);
            }

            _lastAcceptedSequence = sequence;
            _state = StreamContinuityState.Current;
            return Snapshot("STREAM_SEQUENCE_CONTIGUOUS");
        }
    }

    public StreamContinuityObservation ObserveUnsequenced(DateTimeOffset sourceTime, DateTimeOffset receiveTime, string evidenceReference)
    {
        ValidateEvidence(evidenceReference, receiveTime);
        if (sourceTime == default || sourceTime > receiveTime) throw new ArgumentException("STREAM_EVENT_TIME_INVALID");
        lock (_gate)
        {
            if (_state == StreamContinuityState.Closed) return Snapshot("STREAM_CLOSED_EVENT_REJECTED");
            _lastEvidence = evidenceReference.Trim();
            _lastObservedAt = receiveTime;
            if (_state is StreamContinuityState.ReconciliationRequired or StreamContinuityState.GapDetected)
                return Snapshot("UNSEQUENCED_EVENT_CANNOT_PROVE_CONTINUITY");
            _state = StreamContinuityState.Current;
            return Snapshot("UNSEQUENCED_EVENT_CURRENT_BUT_CONTINUITY_NOT_SEQUENCE_PROVEN");
        }
    }

    public StreamContinuityObservation MarkReconciled(long? authoritativeSequence, string evidenceReference, DateTimeOffset observedAt)
    {
        ValidateEvidence(evidenceReference, observedAt);
        if (authoritativeSequence < 0) throw new ArgumentOutOfRangeException(nameof(authoritativeSequence));
        lock (_gate)
        {
            if (_state == StreamContinuityState.Closed) return Snapshot("STREAM_CLOSED_RECONCILIATION_REJECTED");
            _lastAcceptedSequence = authoritativeSequence;
            _lastEvidence = evidenceReference.Trim();
            _lastObservedAt = observedAt;
            _state = StreamContinuityState.Current;
            return Snapshot("STREAM_CONTINUITY_RECONCILED");
        }
    }

    public StreamContinuityObservation EvaluateFreshness(DateTimeOffset now, TimeSpan maximumAge)
    {
        if (now == default || maximumAge <= TimeSpan.Zero) throw new ArgumentException("STREAM_FRESHNESS_WINDOW_INVALID");
        lock (_gate)
        {
            if (_state == StreamContinuityState.Closed) return Snapshot("STREAM_CLOSED");
            if (_lastObservedAt == default || now < _lastObservedAt || now - _lastObservedAt > maximumAge)
            {
                _state = StreamContinuityState.Stale;
                return Snapshot("STREAM_STALE_OR_CLOCK_INVALID");
            }
            return Snapshot("STREAM_FRESHNESS_WITHIN_WINDOW");
        }
    }

    public StreamContinuityObservation Closed(string evidenceReference, DateTimeOffset observedAt)
    {
        ValidateEvidence(evidenceReference, observedAt);
        lock (_gate)
        {
            _lastEvidence = evidenceReference.Trim();
            _lastObservedAt = observedAt;
            _state = StreamContinuityState.Closed;
            return Snapshot("STREAM_CLOSED");
        }
    }

    private StreamContinuityObservation Snapshot(string reason)
        => new(_state, _lastAcceptedSequence, null, null, reason, _lastEvidence, _lastObservedAt);

    private static void ValidateEvidence(string evidenceReference, DateTimeOffset observedAt)
    {
        if (string.IsNullOrWhiteSpace(evidenceReference)) throw new ArgumentException("STREAM_CONTINUITY_EVIDENCE_REQUIRED", nameof(evidenceReference));
        if (observedAt == default) throw new ArgumentException("STREAM_CONTINUITY_TIME_REQUIRED", nameof(observedAt));
    }
}
