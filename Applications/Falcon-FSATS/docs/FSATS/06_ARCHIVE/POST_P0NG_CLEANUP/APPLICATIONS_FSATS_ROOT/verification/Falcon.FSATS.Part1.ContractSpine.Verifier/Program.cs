using Falcon.FSATS.ContractSpine;
using Falcon.Trading.Application;
using Falcon.Trading.FSAPMA;
using Falcon.Trading.Guardian;

namespace Falcon.FSATS.Part1.ContractSpine.Verifier;

internal static class Program
{
    private static readonly List<string> Failures = new();
    private const int GateCount = 14;

    private static int Main()
    {
        Run("CONTRACT_IDS_UNIQUE", VerifyContractIdsUnique);
        Run("KNOWN_CORE_APPLICATIONS_ONLY", VerifyKnownApplicationsOnly);
        Run("NO_SELF_ROUTE", VerifyNoSelfRoute);
        Run("ROLE_PAIRS_VALID", VerifyRolePairsValid);
        Run("TRAFFIC_CONTEXT_DEFINED", VerifyTrafficContextDefined);
        Run("FCR_IDS_CANONICAL", VerifyFcrIdsCanonical);
        Run("FCR_IDS_UNIQUE_PER_CONTRACT", VerifyFcrIdsUniquePerContract);
        Run("LATENCY_SENSITIVE_BINDS_FCR_0009", VerifyLatencySensitiveBinding);
        Run("PROTECTION_BINDS_FCR_0004", VerifyProtectionBinding);
        Run("MARKET_DATA_BINDS_FCR_0005", VerifyMarketDataBinding);
        Run("EVIDENCE_BINDS_FCR_0006", VerifyEvidenceBinding);
        Run("MARKET_DATA_DIRECTION", VerifyMarketDataDirection);
        Run("PROTECTION_DIRECTION", VerifyProtectionDirection);
        Run("NO_RUNTIME_METHOD_SURFACE", VerifyNoRuntimeMethodSurface);

        if (Failures.Count == 0)
        {
            Console.WriteLine($"FSATS_P1D_CONTRACT_SPINE_VERIFIER_PASS {GateCount}/{GateCount}");
            return 0;
        }

        Console.Error.WriteLine($"FSATS_P1D_CONTRACT_SPINE_VERIFIER_FAIL {GateCount - Failures.Count}/{GateCount}");
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

    private static void VerifyContractIdsUnique()
    {
        var ids = FsatsContractFamilies.Core.Select(x => x.Id.Value).ToArray();
        Require(ids.Distinct(StringComparer.Ordinal).Count() == ids.Length, "duplicate_contract_id");
    }

    private static void VerifyKnownApplicationsOnly()
    {
        var known = new HashSet<string>(StringComparer.Ordinal)
        {
            GuardianApplicationShell.ApplicationId.Value,
            FsapmaApplicationShell.ApplicationId.Value,
            TradingApplicationShell.ApplicationId.Value
        };

        foreach (var contract in FsatsContractFamilies.Core)
        {
            Require(known.Contains(contract.Source.ApplicationId.Value), "unknown_contract_source");
            Require(known.Contains(contract.Target.ApplicationId.Value), "unknown_contract_target");
        }
    }

    private static void VerifyNoSelfRoute()
    {
        foreach (var contract in FsatsContractFamilies.Core)
        {
            Require(contract.Source.ApplicationId != contract.Target.ApplicationId, "self_route_detected");
        }
    }

    private static void VerifyRolePairsValid()
    {
        foreach (var contract in FsatsContractFamilies.Core)
        {
            var valid =
                contract.Source.Role == ContractEndpointRole.Producer && contract.Target.Role == ContractEndpointRole.Consumer ||
                contract.Source.Role == ContractEndpointRole.Requester && contract.Target.Role == ContractEndpointRole.Responder;
            Require(valid, "invalid_role_pair");
        }
    }

    private static void VerifyTrafficContextDefined()
    {
        foreach (var contract in FsatsContractFamilies.Core)
        {
            Require(Enum.IsDefined(contract.Context), "undefined_traffic_context");
        }
    }

    private static void VerifyFcrIdsCanonical()
    {
        foreach (var id in FsatsContractFamilies.Core.SelectMany(x => x.FoundationRequestIds))
        {
            Require(System.Text.RegularExpressions.Regex.IsMatch(id, "^FCR-[0-9]{4}$"), $"noncanonical_fcr:{id}");
        }
    }

    private static void VerifyFcrIdsUniquePerContract()
    {
        foreach (var contract in FsatsContractFamilies.Core)
        {
            Require(contract.FoundationRequestIds.Distinct(StringComparer.Ordinal).Count() == contract.FoundationRequestIds.Count, $"duplicate_fcr:{contract.Id.Value}");
        }
    }

    private static void VerifyLatencySensitiveBinding()
    {
        foreach (var contract in FsatsContractFamilies.Core.Where(x => x.LatencySensitive))
        {
            Require(contract.FoundationRequestIds.Contains("FCR-0009", StringComparer.Ordinal), $"missing_fcr_0009:{contract.Id.Value}");
        }
    }

    private static void VerifyProtectionBinding()
    {
        foreach (var contract in FsatsContractFamilies.Core.Where(x => x.Context == ContractTrafficContext.Protection))
        {
            Require(contract.FoundationRequestIds.Contains("FCR-0004", StringComparer.Ordinal), $"missing_fcr_0004:{contract.Id.Value}");
        }
    }

    private static void VerifyMarketDataBinding()
    {
        foreach (var contract in FsatsContractFamilies.Core.Where(x => x.Id.Value.Contains("market-data", StringComparison.Ordinal)))
        {
            Require(contract.FoundationRequestIds.Contains("FCR-0005", StringComparer.Ordinal), $"missing_fcr_0005:{contract.Id.Value}");
        }
    }

    private static void VerifyEvidenceBinding()
    {
        foreach (var contract in FsatsContractFamilies.Core.Where(x => x.Context == ContractTrafficContext.Evidence))
        {
            Require(contract.FoundationRequestIds.Contains("FCR-0006", StringComparer.Ordinal), $"missing_fcr_0006:{contract.Id.Value}");
        }
    }

    private static void VerifyMarketDataDirection()
    {
        var delivery = FsatsContractFamilies.Core.Single(x => x.Id.Value == "fsats.contract.normalized-operational-market-data");
        Require(delivery.Source.ApplicationId == FsapmaApplicationShell.ApplicationId, "market_data_source_not_fsapma");
        Require(delivery.Target.ApplicationId == TradingApplicationShell.ApplicationId, "market_data_target_not_trading");
    }

    private static void VerifyProtectionDirection()
    {
        foreach (var contract in FsatsContractFamilies.Core.Where(x => x.Context == ContractTrafficContext.Protection))
        {
            Require(contract.Source.ApplicationId == GuardianApplicationShell.ApplicationId, "protection_source_not_guardian");
        }
    }

    private static void VerifyNoRuntimeMethodSurface()
    {
        var names = typeof(FsatsContractFamilies)
            .GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
            .Select(x => x.Name)
            .ToArray();

        var forbidden = new[] { "Route", "Send", "Publish", "Deliver", "Execute", "Activate" };
        foreach (var token in forbidden)
        {
            Require(!names.Any(x => x.Contains(token, StringComparison.OrdinalIgnoreCase)), $"runtime_method_surface:{token}");
        }
    }

    private static void Require(bool condition, string reason)
    {
        if (!condition)
        {
            throw new InvalidOperationException(reason);
        }
    }
}
