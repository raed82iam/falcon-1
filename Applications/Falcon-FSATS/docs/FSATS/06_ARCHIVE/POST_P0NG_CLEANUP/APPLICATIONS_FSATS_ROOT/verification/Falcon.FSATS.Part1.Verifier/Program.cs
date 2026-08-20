using Falcon.FSATS.ContractSpine;
using Falcon.FSATS.FoundationBindings;
using Falcon.FSATS.Primitives;
using Falcon.Trading.Application;
using Falcon.Trading.FSAPMA;
using Falcon.Trading.Guardian;

namespace Falcon.FSATS.Part1.Verifier;

internal static class Program
{
    private static readonly List<string> Failures = new();
    private const int GateCount = 18;

    private static int Main()
    {
        Run("APPLICATION_IDS_UNIQUE", VerifyApplicationIdsUnique);
        Run("PACKAGE_IDS_UNIQUE", VerifyPackageIdsUnique);
        Run("MSA_IDS_UNIQUE", VerifyMsaIdsUnique);
        Run("GUARDIAN_ROOM_COUNT_4", () => Require(GuardianApplicationShell.MajorBranches.Count == 4, "guardian_room_count_mismatch"));
        Run("FSAPMA_ROOM_COUNT_6", () => Require(FsapmaApplicationShell.MajorBranches.Count == 6, "fsapma_room_count_mismatch"));
        Run("TRADING_ROOM_COUNT_12", () => Require(TradingApplicationShell.MajorBranches.Count == 12, "trading_room_count_mismatch"));
        Run("ROOM_IDS_GLOBALLY_UNIQUE", VerifyRoomIdsGloballyUnique);
        Run("INITIAL_HEALTH_RESTRICTED", VerifyInitialHealthRestricted);
        Run("DEADLINE_SEMANTICS", VerifyDeadlineSemantics);
        Run("INVALID_IDENTIFIER_FAILS_CLOSED", VerifyInvalidIdentifierFailsClosed);
        Run("CONTRACT_IDS_UNIQUE", VerifyContractIdsUnique);
        Run("CONTRACT_ENDPOINTS_KNOWN", VerifyContractEndpointsKnown);
        Run("LATENCY_SENSITIVE_CONTRACTS_BIND_FCR_0009", VerifyLatencySensitiveFcrBinding);
        Run("PROTECTION_CONTRACTS_BIND_FCR_0004", VerifyProtectionFcrBinding);
        Run("MARKET_DATA_CONTRACTS_BIND_FCR_0005", VerifyMarketDataFcrBinding);
        Run("CANONICAL_ENCODING_DETERMINISTIC", VerifyCanonicalEncoding);
        Run("WP03_IDENTITY_PINNED", VerifyWp03IdentityPinned);
        Run("CORE_APPLICATIONS_BOUND_TO_WP03", VerifyCoreApplicationsBoundToWp03);

        if (Failures.Count == 0)
        {
            Console.WriteLine($"FSATS_PART1_VERIFIER_PASS {GateCount}/{GateCount}");
            return 0;
        }

        Console.Error.WriteLine($"FSATS_PART1_VERIFIER_FAIL {GateCount - Failures.Count}/{GateCount}");
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
        var ids = new[]
        {
            GuardianApplicationShell.ApplicationId.Value,
            FsapmaApplicationShell.ApplicationId.Value,
            TradingApplicationShell.ApplicationId.Value
        };

        Require(ids.Distinct(StringComparer.Ordinal).Count() == ids.Length, "duplicate_application_identity");
    }

    private static void VerifyPackageIdsUnique()
    {
        var ids = new[]
        {
            GuardianApplicationShell.PackageId.Value,
            FsapmaApplicationShell.PackageId.Value,
            TradingApplicationShell.PackageId.Value
        };

        Require(ids.Distinct(StringComparer.Ordinal).Count() == ids.Length, "duplicate_package_identity");
    }

    private static void VerifyMsaIdsUnique()
    {
        var ids = new[]
        {
            GuardianApplicationShell.MainSelfAwarenessId.Value,
            FsapmaApplicationShell.MainSelfAwarenessId.Value,
            TradingApplicationShell.MainSelfAwarenessId.Value
        };

        Require(ids.Distinct(StringComparer.Ordinal).Count() == ids.Length, "duplicate_msa_identity");
    }

    private static void VerifyRoomIdsGloballyUnique()
    {
        var rooms = GuardianApplicationShell.MajorBranches
            .Concat(FsapmaApplicationShell.MajorBranches)
            .Concat(TradingApplicationShell.MajorBranches)
            .Select(x => x.Value)
            .ToArray();

        Require(rooms.Length == 22, "room_inventory_not_22");
        Require(rooms.Distinct(StringComparer.Ordinal).Count() == rooms.Length, "duplicate_room_identity");
    }

    private static void VerifyInitialHealthRestricted()
    {
        var instant = new UtcInstant(DateTimeOffset.UnixEpoch);
        var evidence = new EvidenceId("evidence:part1:initial");

        Require(GuardianApplicationShell.CreateInitialHealth(instant, evidence).Disposition == HealthDisposition.Restricted, "guardian_not_restricted");
        Require(FsapmaApplicationShell.CreateInitialHealth(instant, evidence).Disposition == HealthDisposition.Restricted, "fsapma_not_restricted");
        Require(TradingApplicationShell.CreateInitialHealth(instant, evidence).Disposition == HealthDisposition.Restricted, "trading_not_restricted");
    }

    private static void VerifyDeadlineSemantics()
    {
        var start = new UtcInstant(DateTimeOffset.UnixEpoch);
        var deadline = new Deadline(new UtcInstant(DateTimeOffset.UnixEpoch.AddSeconds(10)));

        Require(!deadline.IsExpired(start), "deadline_expired_too_early");
        Require(deadline.Remaining(start) == TimeSpan.FromSeconds(10), "deadline_remaining_incorrect");
        Require(deadline.IsExpired(new UtcInstant(DateTimeOffset.UnixEpoch.AddSeconds(10))), "deadline_not_expired_at_boundary");
        Require(deadline.Remaining(new UtcInstant(DateTimeOffset.UnixEpoch.AddSeconds(11))) == TimeSpan.Zero, "expired_deadline_remaining_not_zero");
    }

    private static void VerifyInvalidIdentifierFailsClosed()
    {
        try
        {
            _ = new FsatsApplicationId(" invalid id ");
        }
        catch (ArgumentException)
        {
            return;
        }

        throw new InvalidOperationException("invalid_identifier_accepted");
    }

    private static void VerifyContractIdsUnique()
    {
        var ids = FsatsContractFamilies.Core.Select(x => x.Id.Value).ToArray();
        Require(ids.Distinct(StringComparer.Ordinal).Count() == ids.Length, "duplicate_contract_family_identity");
    }

    private static void VerifyContractEndpointsKnown()
    {
        var allowed = new HashSet<string>(StringComparer.Ordinal)
        {
            GuardianApplicationShell.ApplicationId.Value,
            FsapmaApplicationShell.ApplicationId.Value,
            TradingApplicationShell.ApplicationId.Value
        };

        foreach (var contract in FsatsContractFamilies.Core)
        {
            Require(allowed.Contains(contract.Source.ApplicationId.Value), "unknown_contract_source");
            Require(allowed.Contains(contract.Target.ApplicationId.Value), "unknown_contract_target");
        }
    }

    private static void VerifyLatencySensitiveFcrBinding()
    {
        foreach (var contract in FsatsContractFamilies.Core.Where(x => x.LatencySensitive))
        {
            Require(contract.FoundationRequestIds.Contains("FCR-0009", StringComparer.Ordinal), $"missing_fcr_0009:{contract.Id.Value}");
        }
    }

    private static void VerifyProtectionFcrBinding()
    {
        foreach (var contract in FsatsContractFamilies.Core.Where(x => x.Context == ContractTrafficContext.Protection))
        {
            Require(contract.FoundationRequestIds.Contains("FCR-0004", StringComparer.Ordinal), $"missing_fcr_0004:{contract.Id.Value}");
        }
    }

    private static void VerifyMarketDataFcrBinding()
    {
        var marketDataContracts = FsatsContractFamilies.Core.Where(x => x.Id.Value.Contains("market-data", StringComparison.Ordinal)).ToArray();
        Require(marketDataContracts.Length == 2, "market_data_contract_inventory_mismatch");

        foreach (var contract in marketDataContracts)
        {
            Require(contract.FoundationRequestIds.Contains("FCR-0005", StringComparer.Ordinal), $"missing_fcr_0005:{contract.Id.Value}");
        }
    }

    private static void VerifyCanonicalEncoding()
    {
        var a = CanonicalEncoding.Encode(("application", "falcon.trading.application"), ("version", "1.4.0"));
        var b = CanonicalEncoding.Encode(("application", "falcon.trading.application"), ("version", "1.4.0"));
        Require(string.Equals(a, b, StringComparison.Ordinal), "canonical_encoding_not_deterministic");
    }

    private static void VerifyWp03IdentityPinned()
    {
        var identity = FsatsWp03ManifestBindings.AcceptedWp03;
        Require(identity.ImplementationCommit == "5b2998d4329b518d422e815a5fdd60015627f8d8", "wp03_commit_pin_mismatch");
        Require(identity.ProjectBlob == "d086d03af1a0e5bffd45e02e6813cfdd7511dd62", "wp03_project_blob_pin_mismatch");
        Require(identity.SourceBlob == "556cf7ac3511e1ea614a61d5e070a4645c0377bf", "wp03_source_blob_pin_mismatch");
    }

    private static void VerifyCoreApplicationsBoundToWp03()
    {
        var expected = new HashSet<string>(StringComparer.Ordinal)
        {
            GuardianApplicationShell.ApplicationId.Value,
            FsapmaApplicationShell.ApplicationId.Value,
            TradingApplicationShell.ApplicationId.Value
        };

        var actual = FsatsWp03ManifestBindings.CoreApplications.Select(x => x.ApplicationId.Value).ToHashSet(StringComparer.Ordinal);
        Require(actual.SetEquals(expected), "wp03_core_application_binding_mismatch");
        Require(FsatsWp03ManifestBindings.BindingState == "FOUNDATION_IDENTITY_BOUND", "wp03_binding_state_mismatch");
        Require(FsatsWp03ManifestBindings.BuildConsumptionState == "DEFERRED_OUTSIDE_PART1_CURRENT_SCOPE", "build_consumption_scope_leak");
    }

    private static void Require(bool condition, string reason)
    {
        if (!condition)
        {
            throw new InvalidOperationException(reason);
        }
    }
}
