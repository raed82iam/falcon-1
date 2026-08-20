using System.Security.Cryptography;
using System.Text;
using Falcon.FSATS.FSAPMA.Contracts;
using Falcon.FSATS.FSAPMA.Domain;

namespace Falcon.FSATS.FSAPMA.Application;

public static class FSAPMADurableStateContract
{
    public const string Owner = "FSATS.FSAPMA";
    public const string SchemaVersion = "P3.FSAPMA.DURABLE.v1";
}

public sealed record DurableStreamContinuityRecord(ProviderStreamSessionIdentity Identity, StreamContinuityState State, long? LastAcceptedSequence, string EvidenceReference, DateTimeOffset ObservedAt);
public sealed record DurableOperationalDeliveryRecord(string IdempotencyScopeKey, string Fingerprint, OperationalDataDeliveryResult Result, DateTimeOffset RecordedAt);

public sealed record FSAPMADurableSnapshot(
    string Owner,
    string SchemaVersion,
    long SnapshotGeneration,
    DateTimeOffset CapturedAt,
    IReadOnlyList<DurableStreamContinuityRecord> Streams,
    IReadOnlyList<DurableOperationalDeliveryRecord> Deliveries,
    string PayloadSha256)
{
    public static FSAPMADurableSnapshot Create(long generation, DateTimeOffset capturedAt, IEnumerable<DurableStreamContinuityRecord> streams, IEnumerable<DurableOperationalDeliveryRecord> deliveries)
    {
        if (generation < 0) throw new ArgumentOutOfRangeException(nameof(generation));
        if (capturedAt == default) throw new ArgumentException("FSAPMA_DURABLE_CAPTURE_TIME_REQUIRED", nameof(capturedAt));
        var provisional = new FSAPMADurableSnapshot(FSAPMADurableStateContract.Owner, FSAPMADurableStateContract.SchemaVersion, generation, capturedAt,
            Array.AsReadOnly((streams ?? throw new ArgumentNullException(nameof(streams))).ToArray()),
            Array.AsReadOnly((deliveries ?? throw new ArgumentNullException(nameof(deliveries))).ToArray()), string.Empty);
        return provisional with { PayloadSha256 = FSAPMADurableIntegrity.Compute(provisional) };
    }
}

public sealed record FSAPMARestartStreamState(ProviderStreamSessionIdentity Identity, StreamContinuityState State, long? LastAcceptedSequence, string ReasonCode, string EvidenceReference);
public sealed record FSAPMARestartPlan(bool Accepted, string ReasonCode, IReadOnlyList<FSAPMARestartStreamState> Streams, IReadOnlyDictionary<string, DurableOperationalDeliveryRecord> DeliveryTombstones)
{
    public bool IsOperationalDataCurrent(ProviderStreamSessionIdentity identity)
        => Accepted && Streams.Any(x => x.Identity == identity && x.State == StreamContinuityState.Current);
}

public interface IFSAPMADurableStatePort
{
    ValueTask<FSAPMADurableSnapshot?> LoadAsync(CancellationToken cancellationToken);
    ValueTask SaveAsync(FSAPMADurableSnapshot snapshot, CancellationToken cancellationToken);
}

public static class FSAPMARestartReconstructor
{
    public static FSAPMARestartPlan Reconstruct(FSAPMADurableSnapshot? snapshot, DateTimeOffset now)
    {
        if (snapshot is null) return Reject("FSAPMA_DURABLE_STATE_MISSING");
        if (now == default || snapshot.CapturedAt == default || snapshot.CapturedAt > now) return Reject("FSAPMA_DURABLE_TIME_INVALID");
        if (!StringComparer.Ordinal.Equals(snapshot.Owner, FSAPMADurableStateContract.Owner)) return Reject("FSAPMA_DURABLE_OWNER_MISMATCH");
        if (!StringComparer.Ordinal.Equals(snapshot.SchemaVersion, FSAPMADurableStateContract.SchemaVersion)) return Reject("FSAPMA_DURABLE_SCHEMA_UNSUPPORTED");
        if (snapshot.SnapshotGeneration < 0 || !FSAPMADurableIntegrity.Verify(snapshot)) return Reject("FSAPMA_DURABLE_INTEGRITY_INVALID");

        var streams = snapshot.Streams ?? Array.Empty<DurableStreamContinuityRecord>();
        if (streams.Any(x => x is null || x.Identity is null || !Enum.IsDefined(x.State) || string.IsNullOrWhiteSpace(x.EvidenceReference) || x.ObservedAt == default || x.ObservedAt > snapshot.CapturedAt || x.LastAcceptedSequence < 0)) return Reject("FSAPMA_STREAM_RECORD_INVALID");
        if (streams.GroupBy(x => x.Identity.CanonicalKey, StringComparer.Ordinal).Any(g => g.Count() != 1)) return Reject("FSAPMA_STREAM_IDENTITY_DUPLICATE");

        var rebuilt = streams.Select(x =>
        {
            var state = x.State switch
            {
                StreamContinuityState.Current or StreamContinuityState.DuplicateObserved => StreamContinuityState.ReconciliationRequired,
                StreamContinuityState.GapDetected => StreamContinuityState.GapDetected,
                StreamContinuityState.ReconciliationRequired => StreamContinuityState.ReconciliationRequired,
                StreamContinuityState.Stale => StreamContinuityState.Stale,
                StreamContinuityState.Closed => StreamContinuityState.Closed,
                StreamContinuityState.Initializing => StreamContinuityState.Initializing,
                _ => throw new InvalidOperationException("UNREACHABLE_STREAM_STATE")
            };
            var reason = x.State is StreamContinuityState.Current or StreamContinuityState.DuplicateObserved
                ? "PROCESS_RESTART_CONTINUITY_NOT_PROVEN_RECONCILIATION_REQUIRED"
                : "PROCESS_RESTART_PRESERVED_NONCURRENT_STREAM_TRUTH";
            return new FSAPMARestartStreamState(x.Identity, state, x.LastAcceptedSequence, reason, x.EvidenceReference);
        }).ToArray();

        var deliveries = snapshot.Deliveries ?? Array.Empty<DurableOperationalDeliveryRecord>();
        if (deliveries.Any(x => x is null || string.IsNullOrWhiteSpace(x.IdempotencyScopeKey) || !IsSha256(x.Fingerprint) || x.Result is null || !Enum.IsDefined(x.Result.State) || string.IsNullOrWhiteSpace(x.Result.ObservationId) || string.IsNullOrWhiteSpace(x.Result.ConsumerApplicationId) || string.IsNullOrWhiteSpace(x.Result.ReasonCode) || string.IsNullOrWhiteSpace(x.Result.CorrelationId) || x.Result.EffectiveAt == default || x.Result.EffectiveAt > x.RecordedAt || x.RecordedAt == default || x.RecordedAt > snapshot.CapturedAt)) return Reject("FSAPMA_DELIVERY_RECORD_INVALID");
        if (deliveries.GroupBy(x => x.IdempotencyScopeKey, StringComparer.Ordinal).Any(g => g.Count() != 1)) return Reject("FSAPMA_DELIVERY_IDEMPOTENCY_DUPLICATE");
        var tombstones = deliveries.ToDictionary(x => x.IdempotencyScopeKey, StringComparer.Ordinal);

        return new(true, "FSAPMA_RESTART_RECONSTRUCTION_ACCEPTED", rebuilt, tombstones);
    }

    public static OperationalDataDeliveryResult ReplayView(DurableOperationalDeliveryRecord prior)
        => prior.Result.State switch
        {
            OperationalDataDeliveryState.DeliveryOutcomeUnknown => prior.Result with { ReasonCode = $"RESTART_IDEMPOTENT_DUPLICATE_OF_UNKNOWN:{prior.Result.ReasonCode}" },
            OperationalDataDeliveryState.Rejected => prior.Result with { ReasonCode = $"RESTART_IDEMPOTENT_DUPLICATE_OF_REJECTED:{prior.Result.ReasonCode}" },
            OperationalDataDeliveryState.DeliveredDegraded => prior.Result with { ReasonCode = $"RESTART_IDEMPOTENT_DUPLICATE_OF_DEGRADED:{prior.Result.ReasonCode}" },
            _ => prior.Result with { State = OperationalDataDeliveryState.Duplicate, ReasonCode = "RESTART_IDEMPOTENT_DUPLICATE_NO_REDISPATCH" }
        };

    private static FSAPMARestartPlan Reject(string reason) => new(false, reason, Array.Empty<FSAPMARestartStreamState>(), new Dictionary<string, DurableOperationalDeliveryRecord>(StringComparer.Ordinal));
    private static bool IsSha256(string value) => value is { Length: 64 } && value.All(Uri.IsHexDigit);
}

internal static class FSAPMADurableIntegrity
{
    public static bool Verify(FSAPMADurableSnapshot snapshot)
    {
        try { return snapshot.PayloadSha256 is { Length: 64 } && snapshot.PayloadSha256.All(Uri.IsHexDigit) && StringComparer.Ordinal.Equals(snapshot.PayloadSha256, Compute(snapshot)); }
        catch { return false; }
    }

    public static string Compute(FSAPMADurableSnapshot snapshot)
    {
        var sb = new StringBuilder(); Add(sb, snapshot.Owner); Add(sb, snapshot.SchemaVersion); Add(sb, snapshot.SnapshotGeneration.ToString()); Add(sb, snapshot.CapturedAt.ToUniversalTime().ToString("O"));
        foreach (var x in (snapshot.Streams ?? Array.Empty<DurableStreamContinuityRecord>()).OrderBy(x => x.Identity.CanonicalKey, StringComparer.Ordinal)) { Add(sb, x.Identity.CanonicalKey); Add(sb, x.State.ToString()); Add(sb, x.LastAcceptedSequence?.ToString() ?? string.Empty); Add(sb, x.EvidenceReference); Add(sb, x.ObservedAt.ToUniversalTime().ToString("O")); }
        foreach (var x in (snapshot.Deliveries ?? Array.Empty<DurableOperationalDeliveryRecord>()).OrderBy(x => x.IdempotencyScopeKey, StringComparer.Ordinal)) { Add(sb, x.IdempotencyScopeKey); Add(sb, x.Fingerprint); Add(sb, x.Result.State.ToString()); Add(sb, x.Result.ObservationId); Add(sb, x.Result.ConsumerApplicationId); Add(sb, x.Result.ReasonCode); Add(sb, x.Result.CorrelationId); Add(sb, x.Result.EffectiveAt.ToUniversalTime().ToString("O")); Add(sb, x.Result.ProviderRouteNamespace ?? string.Empty); Add(sb, x.RecordedAt.ToUniversalTime().ToString("O")); }
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(sb.ToString())));
    }
    private static void Add(StringBuilder sb, string value) => sb.Append(value.Length).Append(':').Append(value).Append('|');
}
