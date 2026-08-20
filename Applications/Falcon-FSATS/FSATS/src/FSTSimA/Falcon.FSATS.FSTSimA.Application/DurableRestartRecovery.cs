using System.Security.Cryptography;
using System.Text;
using Falcon.FSATS.FSTSimA.Domain;

namespace Falcon.FSATS.FSTSimA.Application;

public static class SimulationDurableStateContract
{
    public const string Owner = "FSATS.FSTSIMA";
    public const string SchemaVersion = "P3.FSTSIMA.DURABLE.v1";
}

public enum SimulationRunDurableState { Started, Checkpointed, Completed, Interrupted, Invalid }

public sealed record DurableSimulationRunRecord(ScenarioId ScenarioId, string RunId, int Seed, SimulationInstant LastCommittedInstant, SimulationRunDurableState State, string EvidenceReference, string? ResultEvidenceSha256, DateTimeOffset RecordedAt);

public sealed record SimulationDurableSnapshot(
    string Owner, string SchemaVersion, long SnapshotGeneration, DateTimeOffset CapturedAt,
    IReadOnlyList<DurableSimulationRunRecord> Runs, string PayloadSha256)
{
    public static SimulationDurableSnapshot Create(long generation, DateTimeOffset capturedAt, IEnumerable<DurableSimulationRunRecord> runs)
    {
        if (generation < 0) throw new ArgumentOutOfRangeException(nameof(generation));
        if (capturedAt == default) throw new ArgumentException("SIMULATION_DURABLE_CAPTURE_TIME_REQUIRED", nameof(capturedAt));
        var provisional = new SimulationDurableSnapshot(SimulationDurableStateContract.Owner, SimulationDurableStateContract.SchemaVersion, generation, capturedAt,
            Array.AsReadOnly((runs ?? throw new ArgumentNullException(nameof(runs))).ToArray()), string.Empty);
        return provisional with { PayloadSha256 = SimulationDurableIntegrity.Compute(provisional) };
    }
}

public sealed record SimulationRestartRun(DurableSimulationRunRecord DurableRecord, bool QualificationEligible, bool MayResumeFromCheckpoint, string ReasonCode);
public sealed record SimulationRestartPlan(bool Accepted, string ReasonCode, IReadOnlyList<SimulationRestartRun> Runs);

public interface ISimulationDurableStatePort
{
    ValueTask<SimulationDurableSnapshot?> LoadAsync(CancellationToken cancellationToken);
    ValueTask SaveAsync(SimulationDurableSnapshot snapshot, CancellationToken cancellationToken);
}

public static class SimulationRestartReconstructor
{
    public static SimulationRestartPlan Reconstruct(SimulationDurableSnapshot? snapshot, DateTimeOffset now)
    {
        if (snapshot is null) return Reject("SIMULATION_DURABLE_STATE_MISSING");
        if (now == default || snapshot.CapturedAt == default || snapshot.CapturedAt > now) return Reject("SIMULATION_DURABLE_TIME_INVALID");
        if (!StringComparer.Ordinal.Equals(snapshot.Owner, SimulationDurableStateContract.Owner)) return Reject("SIMULATION_DURABLE_OWNER_MISMATCH");
        if (!StringComparer.Ordinal.Equals(snapshot.SchemaVersion, SimulationDurableStateContract.SchemaVersion)) return Reject("SIMULATION_DURABLE_SCHEMA_UNSUPPORTED");
        if (snapshot.SnapshotGeneration < 0 || !SimulationDurableIntegrity.Verify(snapshot)) return Reject("SIMULATION_DURABLE_INTEGRITY_INVALID");

        var runs = snapshot.Runs ?? Array.Empty<DurableSimulationRunRecord>();
        if (runs.Any(x => x is null || !Enum.IsDefined(x.State) || string.IsNullOrWhiteSpace(x.ScenarioId.Value) || string.IsNullOrWhiteSpace(x.RunId) || string.IsNullOrWhiteSpace(x.EvidenceReference) || x.LastCommittedInstant.Ticks < 0 || x.RecordedAt == default || x.RecordedAt > snapshot.CapturedAt || (x.ResultEvidenceSha256 is not null && !IsSha256(x.ResultEvidenceSha256)))) return Reject("SIMULATION_DURABLE_RUN_INVALID");
        if (runs.GroupBy(x => x.RunId, StringComparer.Ordinal).Any(g => g.Count() != 1)) return Reject("SIMULATION_DURABLE_RUN_DUPLICATE");

        var rebuilt = runs.Select(x => x.State switch
        {
            SimulationRunDurableState.Completed when IsSha256(x.ResultEvidenceSha256) => new SimulationRestartRun(x, true, false, "COMPLETED_EVIDENCE_PRESERVED_IMMUTABLE"),
            SimulationRunDurableState.Completed => new SimulationRestartRun(x with { State = SimulationRunDurableState.Invalid }, false, false, "COMPLETED_RUN_WITHOUT_VALID_BOUND_RESULT_EVIDENCE_NOT_QUALIFIED"),
            SimulationRunDurableState.Checkpointed => new SimulationRestartRun(x, false, true, "CHECKPOINT_MAY_RESUME_AS_ATTRIBUTABLE_SAME_RUN"),
            SimulationRunDurableState.Started => new SimulationRestartRun(x with { State = SimulationRunDurableState.Interrupted }, false, false, "STARTED_RUN_INTERRUPTED_NOT_QUALIFICATION_EVIDENCE"),
            SimulationRunDurableState.Interrupted => new SimulationRestartRun(x, false, false, "INTERRUPTED_RUN_REMAINS_INCOMPLETE"),
            SimulationRunDurableState.Invalid => new SimulationRestartRun(x, false, false, "INVALID_RUN_REMAINS_NONQUALIFYING"),
            _ => throw new InvalidOperationException("UNREACHABLE_SIMULATION_STATE")
        }).ToArray();

        return new(true, "SIMULATION_RESTART_RECONSTRUCTION_ACCEPTED", rebuilt);
    }

    private static SimulationRestartPlan Reject(string reason) => new(false, reason, Array.Empty<SimulationRestartRun>());
    private static bool IsSha256(string? value) => value is { Length: 64 } && value.All(Uri.IsHexDigit);
}

internal static class SimulationDurableIntegrity
{
    public static bool Verify(SimulationDurableSnapshot snapshot)
    {
        try { return snapshot.PayloadSha256 is { Length: 64 } && snapshot.PayloadSha256.All(Uri.IsHexDigit) && StringComparer.Ordinal.Equals(snapshot.PayloadSha256, Compute(snapshot)); }
        catch { return false; }
    }

    public static string Compute(SimulationDurableSnapshot snapshot)
    {
        var sb = new StringBuilder(); Add(sb, snapshot.Owner); Add(sb, snapshot.SchemaVersion); Add(sb, snapshot.SnapshotGeneration.ToString()); Add(sb, snapshot.CapturedAt.ToUniversalTime().ToString("O"));
        foreach (var x in (snapshot.Runs ?? Array.Empty<DurableSimulationRunRecord>()).OrderBy(x => x.RunId, StringComparer.Ordinal))
        { Add(sb, x.ScenarioId.Value); Add(sb, x.RunId); Add(sb, x.Seed.ToString()); Add(sb, x.LastCommittedInstant.Ticks.ToString()); Add(sb, x.State.ToString()); Add(sb, x.EvidenceReference); Add(sb, x.ResultEvidenceSha256 ?? string.Empty); Add(sb, x.RecordedAt.ToUniversalTime().ToString("O")); }
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(sb.ToString())));
    }
    private static void Add(StringBuilder sb, string value) => sb.Append(value.Length).Append(':').Append(value).Append('|');
}
