using Falcon.FSATS.Primitives;

namespace Falcon.Trading.FSAPMA;

public static class FsapmaApplicationShell
{
    public static FsatsApplicationId ApplicationId { get; } = new("falcon.trading.fsapma");
    public static PackageId PackageId { get; } = new("package:falcon.trading.fsapma");
    public static VersionId ApplicationVersion { get; } = new("1.4.0");
    public static AwarenessEntityId MainSelfAwarenessId { get; } = new("fsapma.msa");

    public static IReadOnlyList<AwarenessRoomId> MajorBranches { get; } = Array.AsReadOnly(
        new[]
        {
            new AwarenessRoomId("fsapma.provider-registry-and-onboarding"),
            new AwarenessRoomId("fsapma.data-product-and-semantics"),
            new AwarenessRoomId("fsapma.provider-selection-and-routing"),
            new AwarenessRoomId("fsapma.quota-capacity-and-cost"),
            new AwarenessRoomId("fsapma.data-quality-and-reconciliation"),
            new AwarenessRoomId("fsapma.broker-and-account-capability")
        });

    public static HealthSnapshot CreateInitialHealth(UtcInstant observedAt, EvidenceId evidenceId) =>
        new(
            HealthDisposition.Restricted,
            observedAt,
            "PART1_NO_OPERATIONAL_DATA_AUTHORITY",
            evidenceId);
}
