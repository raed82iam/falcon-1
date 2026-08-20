using Falcon.FSATS.Primitives;

namespace Falcon.Trading.Guardian;

public static class GuardianApplicationShell
{
    public static FsatsApplicationId ApplicationId { get; } = new("falcon.trading.guardian");
    public static PackageId PackageId { get; } = new("package:falcon.trading.guardian");
    public static VersionId ApplicationVersion { get; } = new("1.4.0");
    public static AwarenessEntityId MainSelfAwarenessId { get; } = new("guardian.msa");

    public static IReadOnlyList<AwarenessRoomId> MajorBranches { get; } = Array.AsReadOnly(
        new[]
        {
            new AwarenessRoomId("guardian.crisis-detection-and-severity"),
            new AwarenessRoomId("guardian.incident-command-and-safe-mode"),
            new AwarenessRoomId("guardian.open-position-protection"),
            new AwarenessRoomId("guardian.recovery-and-reconciliation")
        });

    public static HealthSnapshot CreateInitialHealth(UtcInstant observedAt, EvidenceId evidenceId) =>
        new(
            HealthDisposition.Restricted,
            observedAt,
            "PART1_NO_RUNTIME_AUTHORITY",
            evidenceId);
}
