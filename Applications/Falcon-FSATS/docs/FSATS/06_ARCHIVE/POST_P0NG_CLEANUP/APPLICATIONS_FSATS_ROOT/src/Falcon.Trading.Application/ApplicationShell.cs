using Falcon.FSATS.Primitives;

namespace Falcon.Trading.Application;

public static class TradingApplicationShell
{
    public static FsatsApplicationId ApplicationId { get; } = new("falcon.trading.application");
    public static PackageId PackageId { get; } = new("package:falcon.trading.application");
    public static VersionId ApplicationVersion { get; } = new("1.4.0");
    public static AwarenessEntityId MainSelfAwarenessId { get; } = new("trading.msa");

    public static IReadOnlyList<AwarenessRoomId> MajorBranches { get; } = Array.AsReadOnly(
        new[]
        {
            new AwarenessRoomId("trading.operations-account-and-environment"),
            new AwarenessRoomId("trading.market-and-instrument-universe"),
            new AwarenessRoomId("trading.analysis-frameworks"),
            new AwarenessRoomId("trading.classical-trading-school"),
            new AwarenessRoomId("trading.opportunity-hunting-school"),
            new AwarenessRoomId("trading.strategy-orchestration-and-decision"),
            new AwarenessRoomId("trading.unified-risk-management"),
            new AwarenessRoomId("trading.portfolio-and-capital-management"),
            new AwarenessRoomId("trading.execution-and-position-lifecycle"),
            new AwarenessRoomId("trading.trading-learning-and-knowledge"),
            new AwarenessRoomId("trading.trading-analytics-and-attribution"),
            new AwarenessRoomId("trading.strategy-evolution-and-experimentation")
        });

    public static HealthSnapshot CreateInitialHealth(UtcInstant observedAt, EvidenceId evidenceId) =>
        new(
            HealthDisposition.Restricted,
            observedAt,
            "PART1_NO_TRADING_AUTHORITY",
            evidenceId);
}
