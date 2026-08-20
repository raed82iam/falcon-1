using System.Security.Cryptography;
using System.Text;
using Falcon.FSATS.ResourceManagement.Domain;

namespace Falcon.FSATS.ResourceManagement.Application;

public static class ResourceDurableStateContract
{
    public const string Owner = "FSATS.APP-RSC";
    public const string SchemaVersion = "P3.APP-RSC.DURABLE.v1";
}

public sealed record DurableRedistributionRecord(RedistributionDecision Decision, string EvidenceReference, DateTimeOffset RecordedAt);

public sealed record ResourceDurableSnapshot(
    string Owner,
    string SchemaVersion,
    long SnapshotGeneration,
    DateTimeOffset CapturedAt,
    CoordinationEpoch LastObservedCoordinationEpoch,
    string? LastObservedFoundationEnvelopeReference,
    IReadOnlyList<DurableRedistributionRecord> Decisions,
    string PayloadSha256)
{
    public static ResourceDurableSnapshot Create(long generation, DateTimeOffset capturedAt, CoordinationEpoch epoch, string? envelopeReference, IEnumerable<DurableRedistributionRecord> decisions)
    {
        if (generation < 0 || epoch.Value < 0) throw new ArgumentOutOfRangeException(nameof(generation));
        if (capturedAt == default) throw new ArgumentException("APP_RSC_DURABLE_CAPTURE_TIME_REQUIRED", nameof(capturedAt));
        var provisional = new ResourceDurableSnapshot(ResourceDurableStateContract.Owner, ResourceDurableStateContract.SchemaVersion, generation, capturedAt, epoch,
            string.IsNullOrWhiteSpace(envelopeReference) ? null : envelopeReference.Trim(), Array.AsReadOnly((decisions ?? throw new ArgumentNullException(nameof(decisions))).ToArray()), string.Empty);
        return provisional with { PayloadSha256 = ResourceDurableIntegrity.Compute(provisional) };
    }
}

public sealed record ResourceRestartPlan(bool Accepted, string ReasonCode, CoordinationEpoch LastObservedEpoch, IReadOnlySet<string> FencedDecisionIds, bool AllowsRedistribution);

public interface IResourceDurableStatePort
{
    ValueTask<ResourceDurableSnapshot?> LoadAsync(CancellationToken cancellationToken);
    ValueTask SaveAsync(ResourceDurableSnapshot snapshot, CancellationToken cancellationToken);
}

public static class ResourceRestartReconstructor
{
    public static ResourceRestartPlan Reconstruct(ResourceDurableSnapshot? snapshot, DateTimeOffset now)
    {
        if (snapshot is null) return Reject("APP_RSC_DURABLE_STATE_MISSING");
        if (now == default || snapshot.CapturedAt == default || snapshot.CapturedAt > now) return Reject("APP_RSC_DURABLE_TIME_INVALID");
        if (!StringComparer.Ordinal.Equals(snapshot.Owner, ResourceDurableStateContract.Owner)) return Reject("APP_RSC_DURABLE_OWNER_MISMATCH");
        if (!StringComparer.Ordinal.Equals(snapshot.SchemaVersion, ResourceDurableStateContract.SchemaVersion)) return Reject("APP_RSC_DURABLE_SCHEMA_UNSUPPORTED");
        if (snapshot.SnapshotGeneration < 0 || snapshot.LastObservedCoordinationEpoch.Value < 0 || !ResourceDurableIntegrity.Verify(snapshot)) return Reject("APP_RSC_DURABLE_INTEGRITY_INVALID");

        var decisions = snapshot.Decisions ?? Array.Empty<DurableRedistributionRecord>();
        if (decisions.Any(x => x is null || x.Decision is null || string.IsNullOrWhiteSpace(x.Decision.DecisionId) || string.IsNullOrWhiteSpace(x.Decision.SourceApplication) || string.IsNullOrWhiteSpace(x.Decision.TargetApplication) || string.IsNullOrWhiteSpace(x.Decision.ResourceClass) || x.Decision.Amount <= 0m || x.Decision.Epoch.Value < 0 || x.Decision.Epoch.Value > snapshot.LastObservedCoordinationEpoch.Value || string.IsNullOrWhiteSpace(x.Decision.EnvelopeReference) || string.IsNullOrWhiteSpace(x.Decision.ReasonCode) || string.IsNullOrWhiteSpace(x.EvidenceReference) || x.RecordedAt == default || x.RecordedAt > snapshot.CapturedAt)) return Reject("APP_RSC_DURABLE_DECISION_INVALID");
        if (decisions.GroupBy(x => x.Decision.DecisionId, StringComparer.Ordinal).Any(g => g.Count() != 1)) return Reject("APP_RSC_DURABLE_DECISION_DUPLICATE");
        if (decisions.Count > 0 && string.IsNullOrWhiteSpace(snapshot.LastObservedFoundationEnvelopeReference)) return Reject("APP_RSC_DURABLE_ENVELOPE_REFERENCE_REQUIRED");
        if (snapshot.LastObservedFoundationEnvelopeReference is not null && decisions.Any(x => !StringComparer.Ordinal.Equals(x.Decision.EnvelopeReference, snapshot.LastObservedFoundationEnvelopeReference))) return Reject("APP_RSC_DURABLE_ENVELOPE_REFERENCE_MISMATCH");

        var fenced = new HashSet<string>(decisions.Select(x => x.Decision.DecisionId), StringComparer.Ordinal);
        return new(true, "APP_RSC_RESTART_ACCEPTED_FRESH_FOUNDATION_TRUTH_REQUIRED", snapshot.LastObservedCoordinationEpoch, fenced, false);
    }

    public static bool CanResumeWithFreshFoundationTruth(ResourceRestartPlan plan, CoordinationEpoch currentEpoch, FoundationEnvelope currentEnvelope, DateTimeOffset now)
    {
        if (plan is null || !plan.Accepted || currentEnvelope is null || now == default) return false;
        return currentEpoch.Value > plan.LastObservedEpoch.Value &&
               ResourceEpochFence.IsCurrent(currentEpoch, currentEpoch, currentEnvelope.Reference, currentEnvelope, now);
    }

    private static ResourceRestartPlan Reject(string reason) => new(false, reason, new CoordinationEpoch(0), new HashSet<string>(StringComparer.Ordinal), false);
}

internal static class ResourceDurableIntegrity
{
    public static bool Verify(ResourceDurableSnapshot snapshot)
    {
        try { return snapshot.PayloadSha256 is { Length: 64 } && snapshot.PayloadSha256.All(Uri.IsHexDigit) && StringComparer.Ordinal.Equals(snapshot.PayloadSha256, Compute(snapshot)); }
        catch { return false; }
    }

    public static string Compute(ResourceDurableSnapshot snapshot)
    {
        var sb = new StringBuilder();
        Add(sb, snapshot.Owner); Add(sb, snapshot.SchemaVersion); Add(sb, snapshot.SnapshotGeneration.ToString()); Add(sb, snapshot.CapturedAt.ToUniversalTime().ToString("O")); Add(sb, snapshot.LastObservedCoordinationEpoch.Value.ToString()); Add(sb, snapshot.LastObservedFoundationEnvelopeReference ?? string.Empty);
        foreach (var x in (snapshot.Decisions ?? Array.Empty<DurableRedistributionRecord>()).OrderBy(x => x.Decision.DecisionId, StringComparer.Ordinal))
        { Add(sb, x.Decision.DecisionId); Add(sb, x.Decision.SourceApplication); Add(sb, x.Decision.TargetApplication); Add(sb, x.Decision.ResourceClass); Add(sb, x.Decision.Amount.ToString(System.Globalization.CultureInfo.InvariantCulture)); Add(sb, x.Decision.Epoch.Value.ToString()); Add(sb, x.Decision.EnvelopeReference); Add(sb, x.Decision.ReasonCode); Add(sb, x.EvidenceReference); Add(sb, x.RecordedAt.ToUniversalTime().ToString("O")); }
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(sb.ToString())));
    }
    private static void Add(StringBuilder sb, string value) => sb.Append(value.Length).Append(':').Append(value).Append('|');
}
