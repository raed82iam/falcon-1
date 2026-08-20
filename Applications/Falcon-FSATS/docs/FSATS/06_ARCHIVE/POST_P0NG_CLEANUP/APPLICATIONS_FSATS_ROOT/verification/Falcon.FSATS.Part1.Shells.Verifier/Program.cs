using Falcon.FSATS.Primitives;
using Falcon.Trading.Application;
using Falcon.Trading.FSAPMA;
using Falcon.Trading.Guardian;

namespace Falcon.FSATS.Part1.Shells.Verifier;

internal static class Program
{
    private static readonly List<string> Failures = new();
    private const int GateCount = 12;

    private static int Main()
    {
        Run("APPLICATION_IDS_UNIQUE", VerifyApplicationIdsUnique);
        Run("PACKAGE_IDS_UNIQUE", VerifyPackageIdsUnique);
        Run("MSA_IDS_UNIQUE", VerifyMsaIdsUnique);
        Run("APPLICATION_VERSION_ALIGNED", VerifyApplicationVersionsAligned);
        Run("GUARDIAN_ROOM_COUNT_4", () => Require(GuardianApplicationShell.MajorBranches.Count == 4, "guardian_room_count_mismatch"));
        Run("FSAPMA_ROOM_COUNT_6", () => Require(FsapmaApplicationShell.MajorBranches.Count == 6, "fsapma_room_count_mismatch"));
        Run("TRADING_ROOM_COUNT_12", () => Require(TradingApplicationShell.MajorBranches.Count == 12, "trading_room_count_mismatch"));
        Run("ROOM_IDS_GLOBALLY_UNIQUE", VerifyRoomIdsGloballyUnique);
        Run("ROOM_PREFIX_OWNERSHIP", VerifyRoomPrefixOwnership);
        Run("INITIAL_HEALTH_RESTRICTED", VerifyInitialHealthRestricted);
        Run("INITIAL_HEALTH_EVIDENCE_BOUND", VerifyInitialHealthEvidenceBound);
        Run("NO_SHELL_RUNTIME_AUTHORITY_SURFACE", VerifyNoRuntimeAuthoritySurface);

        if (Failures.Count == 0)
        {
            Console.WriteLine($"FSATS_P1C_SHELLS_VERIFIER_PASS {GateCount}/{GateCount}");
            return 0;
        }

        Console.Error.WriteLine($"FSATS_P1C_SHELLS_VERIFIER_FAIL {GateCount - Failures.Count}/{GateCount}");
        foreach (var failure in Failures)
        {
            Console.Error.WriteLine(failure);
        }

        return 1;
    }

    private static void Run(string name, Action verification)
    {
        try
        {
            verification();
            Console.WriteLine($"PASS {name}");
        }
        catch (Exception ex)
        {
            Failures.Add($"FAIL {name}: {ex.GetType().Name}: {ex.Message}");
        }
    }

    private static void VerifyApplicationIdsUnique()
    {
        var values = new[]
        {
            GuardianApplicationShell.ApplicationId.Value,
            FsapmaApplicationShell.ApplicationId.Value,
            TradingApplicationShell.ApplicationId.Value
        };
        Require(values.Distinct(StringComparer.Ordinal).Count() == 3, "duplicate_application_id");
    }

    private static void VerifyPackageIdsUnique()
    {
        var values = new[]
        {
            GuardianApplicationShell.PackageId.Value,
            FsapmaApplicationShell.PackageId.Value,
            TradingApplicationShell.PackageId.Value
        };
        Require(values.Distinct(StringComparer.Ordinal).Count() == 3, "duplicate_package_id");
    }

    private static void VerifyMsaIdsUnique()
    {
        var values = new[]
        {
            GuardianApplicationShell.MainSelfAwarenessId.Value,
            FsapmaApplicationShell.MainSelfAwarenessId.Value,
            TradingApplicationShell.MainSelfAwarenessId.Value
        };
        Require(values.Distinct(StringComparer.Ordinal).Count() == 3, "duplicate_msa_id");
    }

    private static void VerifyApplicationVersionsAligned()
    {
        var versions = new[]
        {
            GuardianApplicationShell.ApplicationVersion.Value,
            FsapmaApplicationShell.ApplicationVersion.Value,
            TradingApplicationShell.ApplicationVersion.Value
        };
        Require(versions.All(x => string.Equals(x, "1.4.0", StringComparison.Ordinal)), "part1_shell_version_mismatch");
    }

    private static void VerifyRoomIdsGloballyUnique()
    {
        var rooms = AllRooms().Select(x => x.Value).ToArray();
        Require(rooms.Length == 22, "room_inventory_not_22");
        Require(rooms.Distinct(StringComparer.Ordinal).Count() == 22, "duplicate_room_identity");
    }

    private static void VerifyRoomPrefixOwnership()
    {
        Require(GuardianApplicationShell.MajorBranches.All(x => x.Value.StartsWith("guardian.", StringComparison.Ordinal)), "guardian_room_ownership_escape");
        Require(FsapmaApplicationShell.MajorBranches.All(x => x.Value.StartsWith("fsapma.", StringComparison.Ordinal)), "fsapma_room_ownership_escape");
        Require(TradingApplicationShell.MajorBranches.All(x => x.Value.StartsWith("trading.", StringComparison.Ordinal)), "trading_room_ownership_escape");
    }

    private static void VerifyInitialHealthRestricted()
    {
        var instant = new UtcInstant(DateTimeOffset.UnixEpoch);
        var evidence = new EvidenceId("evidence:p1c:initial");

        Require(GuardianApplicationShell.CreateInitialHealth(instant, evidence).Disposition == HealthDisposition.Restricted, "guardian_initial_health_not_restricted");
        Require(FsapmaApplicationShell.CreateInitialHealth(instant, evidence).Disposition == HealthDisposition.Restricted, "fsapma_initial_health_not_restricted");
        Require(TradingApplicationShell.CreateInitialHealth(instant, evidence).Disposition == HealthDisposition.Restricted, "trading_initial_health_not_restricted");
    }

    private static void VerifyInitialHealthEvidenceBound()
    {
        var instant = new UtcInstant(DateTimeOffset.UnixEpoch);
        var evidence = new EvidenceId("evidence:p1c:binding");

        Require(GuardianApplicationShell.CreateInitialHealth(instant, evidence).EvidenceId == evidence, "guardian_evidence_not_bound");
        Require(FsapmaApplicationShell.CreateInitialHealth(instant, evidence).EvidenceId == evidence, "fsapma_evidence_not_bound");
        Require(TradingApplicationShell.CreateInitialHealth(instant, evidence).EvidenceId == evidence, "trading_evidence_not_bound");
    }

    private static void VerifyNoRuntimeAuthoritySurface()
    {
        var shellTypes = new[]
        {
            typeof(GuardianApplicationShell),
            typeof(FsapmaApplicationShell),
            typeof(TradingApplicationShell)
        };

        var forbiddenTokens = new[]
        {
            "Activate", "Execute", "SendOrder", "ConnectBroker", "ConnectProvider", "RouteMessage", "PublishEvent", "GoLive"
        };

        foreach (var type in shellTypes)
        {
            var names = type.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                .Select(x => x.Name)
                .ToArray();

            foreach (var token in forbiddenTokens)
            {
                Require(!names.Any(name => name.Contains(token, StringComparison.OrdinalIgnoreCase)), $"runtime_authority_surface:{type.Name}:{token}");
            }
        }
    }

    private static IEnumerable<AwarenessRoomId> AllRooms() =>
        GuardianApplicationShell.MajorBranches
            .Concat(FsapmaApplicationShell.MajorBranches)
            .Concat(TradingApplicationShell.MajorBranches);

    private static void Require(bool condition, string reason)
    {
        if (!condition)
        {
            throw new InvalidOperationException(reason);
        }
    }
}
