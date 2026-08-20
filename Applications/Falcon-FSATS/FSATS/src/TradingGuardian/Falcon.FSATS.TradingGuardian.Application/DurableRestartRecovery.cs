using System.Security.Cryptography;
using System.Text;
using Falcon.FSATS.TradingGuardian.Contracts;

namespace Falcon.FSATS.TradingGuardian.Application;

public static class GuardianDurableStateContract
{
    public const string Owner = "FSATS.TRADING_GUARDIAN";
    public const string SchemaVersion = "P3.GUARDIAN.DURABLE.v1";
}

public sealed record DurableProtectionOutcomeRecord(string IdempotencyScopeKey, string Fingerprint, ProtectionCommandOutcome Outcome, DateTimeOffset RecordedAt);

public sealed record GuardianDurableSnapshot(
    string Owner,
    string SchemaVersion,
    long SnapshotGeneration,
    DateTimeOffset CapturedAt,
    IReadOnlyList<DurableProtectionOutcomeRecord> Outcomes,
    string PayloadSha256)
{
    public static GuardianDurableSnapshot Create(long generation, DateTimeOffset capturedAt, IEnumerable<DurableProtectionOutcomeRecord> outcomes)
    {
        if (generation < 0) throw new ArgumentOutOfRangeException(nameof(generation));
        if (capturedAt == default) throw new ArgumentException("GUARDIAN_DURABLE_CAPTURE_TIME_REQUIRED", nameof(capturedAt));
        var provisional = new GuardianDurableSnapshot(GuardianDurableStateContract.Owner, GuardianDurableStateContract.SchemaVersion, generation, capturedAt,
            Array.AsReadOnly((outcomes ?? throw new ArgumentNullException(nameof(outcomes))).ToArray()), string.Empty);
        return provisional with { PayloadSha256 = GuardianDurableIntegrity.Compute(provisional) };
    }
}

public sealed record GuardianRestartPlan(bool Accepted, string ReasonCode, IReadOnlyDictionary<string, DurableProtectionOutcomeRecord> IdempotencyTombstones, IReadOnlyList<ProtectionCommandOutcome> ReconciliationRequired, bool RequiresCurrentProtectionTruthVerification);

public interface IGuardianDurableStatePort
{
    ValueTask<GuardianDurableSnapshot?> LoadAsync(CancellationToken cancellationToken);
    ValueTask SaveAsync(GuardianDurableSnapshot snapshot, CancellationToken cancellationToken);
}

public static class GuardianRestartReconstructor
{
    public static GuardianRestartPlan Reconstruct(GuardianDurableSnapshot? snapshot, DateTimeOffset now)
    {
        if (snapshot is null) return Reject("GUARDIAN_DURABLE_STATE_MISSING");
        if (now == default || snapshot.CapturedAt == default || snapshot.CapturedAt > now) return Reject("GUARDIAN_DURABLE_TIME_INVALID");
        if (!StringComparer.Ordinal.Equals(snapshot.Owner, GuardianDurableStateContract.Owner)) return Reject("GUARDIAN_DURABLE_OWNER_MISMATCH");
        if (!StringComparer.Ordinal.Equals(snapshot.SchemaVersion, GuardianDurableStateContract.SchemaVersion)) return Reject("GUARDIAN_DURABLE_SCHEMA_UNSUPPORTED");
        if (snapshot.SnapshotGeneration < 0 || !GuardianDurableIntegrity.Verify(snapshot)) return Reject("GUARDIAN_DURABLE_INTEGRITY_INVALID");

        var records = snapshot.Outcomes ?? Array.Empty<DurableProtectionOutcomeRecord>();
        if (records.Any(x => x is null || string.IsNullOrWhiteSpace(x.IdempotencyScopeKey) || !IsSha256(x.Fingerprint) || x.Outcome is null || !Enum.IsDefined(x.Outcome.State) || x.Outcome.Target is null || !x.Outcome.Target.IsStructurallyValid() || x.RecordedAt == default || x.RecordedAt > snapshot.CapturedAt || x.Outcome.EffectiveAt == default || x.Outcome.EffectiveAt > x.RecordedAt || string.IsNullOrWhiteSpace(x.Outcome.CommandId.Value) || string.IsNullOrWhiteSpace(x.Outcome.TargetApplication) || string.IsNullOrWhiteSpace(x.Outcome.ReasonCode) || string.IsNullOrWhiteSpace(x.Outcome.CorrelationId))) return Reject("GUARDIAN_DURABLE_OUTCOME_INVALID");
        if (records.GroupBy(x => x.IdempotencyScopeKey, StringComparer.Ordinal).Any(g => g.Count() != 1)) return Reject("GUARDIAN_DURABLE_IDEMPOTENCY_DUPLICATE");

        var tombstones = records.ToDictionary(x => x.IdempotencyScopeKey, StringComparer.Ordinal);
        var reconcile = records
            .Where(x => IsReconciliationOwnedAfterRestart(x.Outcome.State))
            .Select(x => x.Outcome with { State = ProtectionOutcomeState.ReconciliationRequired, ReasonCode = "PROCESS_RESTART_PROTECTION_OUTCOME_REQUIRES_RECONCILIATION" })
            .ToArray();
        var verifyCurrent = records.Any(x => RequiresCurrentProtectionTruthVerification(x.Outcome.State));
        return new(true, "GUARDIAN_RESTART_RECONSTRUCTION_ACCEPTED", tombstones, reconcile, verifyCurrent);
    }

    public static ProtectionCommandOutcome ReplayView(DurableProtectionOutcomeRecord prior)
        => prior.Outcome.State switch
        {
            ProtectionOutcomeState.ReconciliationRequired or ProtectionOutcomeState.DispatchFailed or ProtectionOutcomeState.PartiallyApplied or ProtectionOutcomeState.Received or ProtectionOutcomeState.Accepted
                => prior.Outcome with { State = ProtectionOutcomeState.ReconciliationRequired, ReasonCode = "RESTART_IDEMPOTENT_PROTECTION_RECONCILIATION_REQUIRED", RequestFingerprint = prior.Fingerprint },
            ProtectionOutcomeState.Applied
                => prior.Outcome with { ReasonCode = "RESTART_HISTORICAL_APPLIED_CURRENT_PROTECTION_TRUTH_REVALIDATION_REQUIRED", RequestFingerprint = prior.Fingerprint },
            _ => prior.Outcome with { RequestFingerprint = prior.Fingerprint }
        };

    private static bool IsReconciliationOwnedAfterRestart(ProtectionOutcomeState state)
        => state is ProtectionOutcomeState.Received
            or ProtectionOutcomeState.Accepted
            or ProtectionOutcomeState.PartiallyApplied
            or ProtectionOutcomeState.DispatchFailed
            or ProtectionOutcomeState.ReconciliationRequired;

    private static bool RequiresCurrentProtectionTruthVerification(ProtectionOutcomeState state)
        => state is ProtectionOutcomeState.Received
            or ProtectionOutcomeState.Accepted
            or ProtectionOutcomeState.Applied
            or ProtectionOutcomeState.PartiallyApplied
            or ProtectionOutcomeState.DispatchFailed
            or ProtectionOutcomeState.ReconciliationRequired;

    private static GuardianRestartPlan Reject(string reason) => new(false, reason, new Dictionary<string, DurableProtectionOutcomeRecord>(StringComparer.Ordinal), Array.Empty<ProtectionCommandOutcome>(), true);
    private static bool IsSha256(string value) => value is { Length: 64 } && value.All(Uri.IsHexDigit);
}

internal static class GuardianDurableIntegrity
{
    public static bool Verify(GuardianDurableSnapshot snapshot)
    {
        try { return snapshot.PayloadSha256 is { Length: 64 } && snapshot.PayloadSha256.All(Uri.IsHexDigit) && StringComparer.Ordinal.Equals(snapshot.PayloadSha256, Compute(snapshot)); }
        catch { return false; }
    }

    public static string Compute(GuardianDurableSnapshot snapshot)
    {
        var sb = new StringBuilder(); Add(sb, snapshot.Owner); Add(sb, snapshot.SchemaVersion); Add(sb, snapshot.SnapshotGeneration.ToString()); Add(sb, snapshot.CapturedAt.ToUniversalTime().ToString("O"));
        foreach (var x in (snapshot.Outcomes ?? Array.Empty<DurableProtectionOutcomeRecord>()).OrderBy(x => x.IdempotencyScopeKey, StringComparer.Ordinal))
        {
            Add(sb, x.IdempotencyScopeKey); Add(sb, x.Fingerprint); Add(sb, x.Outcome.CommandId.Value); Add(sb, x.Outcome.State.ToString()); Add(sb, x.Outcome.TargetApplication); Add(sb, x.Outcome.Target.CanonicalKey); Add(sb, x.Outcome.ReasonCode); Add(sb, x.Outcome.CorrelationId); Add(sb, x.Outcome.EffectiveAt.ToUniversalTime().ToString("O")); Add(sb, x.Outcome.EvidenceReference ?? string.Empty); Add(sb, x.RecordedAt.ToUniversalTime().ToString("O"));
        }
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(sb.ToString())));
    }
    private static void Add(StringBuilder sb, string value) => sb.Append(value.Length).Append(':').Append(value).Append('|');
}
