using System.Runtime.CompilerServices;
using FA = Falcon.FSATS.FSAPMA.Application;
using FS = Falcon.FSATS.FSTSimA.Application;
using RM = Falcon.FSATS.ResourceManagement.Application;
using TR = Falcon.FSATS.Trading.Application;
using TG = Falcon.FSATS.TradingGuardian.Application;

internal static class Fcr0226ApplicationAiInventoryAdversarialChecks
{
    private sealed record AppInventory(
        string ApplicationId,
        string MsaId,
        IReadOnlyList<string> LsaIds,
        IReadOnlyList<string> CsaIds,
        bool RuntimeAuthorized,
        bool GovernedStateGrantsRuntimeAuthority);

    [ModuleInitializer]
    internal static void Run()
    {
        ExactAcceptedInventoryIsUniqueAndComplete();
        CsaParentLineageIsExplicitAndUnambiguous();
        MissingStage13ContractOnApplicationBranchFailsClosedWithoutLocalSubstitution();
        ReplacementRestartDelegationFallbackSelfReleaseEvidenceAndFabricationDoNotCreateAuthority();
    }

    private static IReadOnlyList<AppInventory> Current() => new[]
    {
        new AppInventory(TR.TradingManifest.Current.ApplicationId, TR.TradingManifest.Current.MsaId, TR.TradingManifest.Current.LsaIds, TR.TradingManifest.Current.CsaIds, TR.TradingManifest.Current.RuntimeAuthorized, TR.TradingManifest.Current.CurrentGovernedStateGrantsRuntimeAuthority),
        new AppInventory(FA.FSAPMAManifest.Current.ApplicationId, FA.FSAPMAManifest.Current.MsaId, FA.FSAPMAManifest.Current.LsaIds, FA.FSAPMAManifest.Current.CsaIds, FA.FSAPMAManifest.Current.RuntimeAuthorized, FA.FSAPMAManifest.Current.CurrentGovernedStateGrantsRuntimeAuthority),
        new AppInventory(TG.TradingGuardianManifest.Current.ApplicationId, TG.TradingGuardianManifest.Current.MsaId, TG.TradingGuardianManifest.Current.LsaIds, TG.TradingGuardianManifest.Current.CsaIds, TG.TradingGuardianManifest.Current.RuntimeAuthorized, TG.TradingGuardianManifest.Current.CurrentGovernedStateGrantsRuntimeAuthority),
        new AppInventory(FS.FSTSimAManifest.Current.ApplicationId, FS.FSTSimAManifest.Current.MsaId, FS.FSTSimAManifest.Current.LsaIds, FS.FSTSimAManifest.Current.CsaIds, FS.FSTSimAManifest.Current.RuntimeAuthorized, FS.FSTSimAManifest.Current.CurrentGovernedStateGrantsRuntimeAuthority),
        new AppInventory(RM.ResourceManagementManifest.Current.ApplicationId, RM.ResourceManagementManifest.Current.MsaId, RM.ResourceManagementManifest.Current.LsaIds, RM.ResourceManagementManifest.Current.CsaIds, RM.ResourceManagementManifest.Current.RuntimeAuthorized, RM.ResourceManagementManifest.Current.CurrentGovernedStateGrantsRuntimeAuthority)
    };

    private static void ExactAcceptedInventoryIsUniqueAndComplete()
    {
        var apps = Current();
        if (apps.Count != 5) throw new InvalidOperationException("FCR0226_APPLICATION_COUNT_MISMATCH");
        if (apps.Select(x => x.ApplicationId).Distinct(StringComparer.Ordinal).Count() != 5) throw new InvalidOperationException("FCR0226_DUPLICATE_APPLICATION_IDENTITY");
        if (apps.Select(x => x.MsaId).Distinct(StringComparer.Ordinal).Count() != 5) throw new InvalidOperationException("FCR0226_DUPLICATE_MSA_IDENTITY");
        if (apps.Sum(x => x.LsaIds.Count) != 34) throw new InvalidOperationException("FCR0226_LSA_COUNT_MISMATCH");
        if (apps.Sum(x => x.CsaIds.Count) != 7) throw new InvalidOperationException("FCR0226_CSA_COUNT_MISMATCH");

        var awarenessIds = apps.SelectMany(x => new[] { x.MsaId }.Concat(x.LsaIds).Concat(x.CsaIds)).ToArray();
        if (awarenessIds.Distinct(StringComparer.Ordinal).Count() != awarenessIds.Length)
            throw new InvalidOperationException("FCR0226_DUPLICATE_AWARENESS_IDENTITY");

        var exactApps = new[] { "FSATS-TRADING", "FSATS-FSAPMA", "FSATS-TRADING-GUARDIAN", "FSATS-FSTSIMA", "APP-RSC" };
        if (!exactApps.OrderBy(x => x, StringComparer.Ordinal).SequenceEqual(apps.Select(x => x.ApplicationId).OrderBy(x => x, StringComparer.Ordinal), StringComparer.Ordinal))
            throw new InvalidOperationException("FCR0226_ACCEPTED_APPLICATION_IDENTITY_SET_CHANGED");
    }

    private static void CsaParentLineageIsExplicitAndUnambiguous()
    {
        var parentByCsa = new Dictionary<string, (string App, string ParentLsa, string Component)>(StringComparer.Ordinal)
        {
            ["CSA-T05-01"] = ("FSATS-TRADING", "T-LSA-05", "OpportunityDiscoveryEngine"),
            ["CSA-T06-01"] = ("FSATS-TRADING", "T-LSA-06", "StrategyController"),
            ["CSA-T12-01"] = ("FSATS-TRADING", "T-LSA-12", "StrategyEvolutionEngine"),
            ["CSA-P05-01"] = ("FSATS-FSAPMA", "P-LSA-05", "AnomalyDetector"),
            ["CSA-G01-01"] = ("FSATS-TRADING-GUARDIAN", "G-LSA-01", "IncidentClassifier"),
            ["CSA-S02-01"] = ("FSATS-FSTSIMA", "S-LSA-02", "SyntheticMarketGenerator"),
            ["CSA-S07-01"] = ("FSATS-FSTSIMA", "S-LSA-07", "CalibrationEngine")
        };

        foreach (var app in Current())
        {
            foreach (var csa in app.CsaIds)
            {
                if (!parentByCsa.TryGetValue(csa, out var binding)) throw new InvalidOperationException("FCR0226_CSA_PARENT_BINDING_MISSING");
                if (!StringComparer.Ordinal.Equals(binding.App, app.ApplicationId)) throw new InvalidOperationException("FCR0226_CSA_CROSS_APPLICATION_PARENT_BYPASS");
                if (!app.LsaIds.Contains(binding.ParentLsa, StringComparer.Ordinal)) throw new InvalidOperationException("FCR0226_CSA_PARENT_LSA_NOT_OWNED_BY_APPLICATION");
                if (string.IsNullOrWhiteSpace(binding.Component)) throw new InvalidOperationException("FCR0226_CSA_COMPONENT_IDENTITY_MISSING");
            }
        }
    }

    private static void MissingStage13ContractOnApplicationBranchFailsClosedWithoutLocalSubstitution()
    {
        var apps = Current();
        if (apps.Any(x => x.RuntimeAuthorized || x.GovernedStateGrantsRuntimeAuthority))
            throw new InvalidOperationException("FCR0226_RUNTIME_AUTHORITY_FABRICATED_BEFORE_STAGE13_BINDING");

        var root = FindRepositoryRoot();
        if (root is null) return;

        var contractPath = Path.Combine(root, "src", "Foundation.Authority", "AiKillControlPlane.cs");
        if (!File.Exists(contractPath))
        {
            // Search only production Application source. The verifier itself necessarily contains
            // the forbidden namespace/type names as lexical probes and must never self-match.
            var applicationSourcePath = Path.Combine(root, "applications", "FSATS", "src");
            if (!Directory.Exists(applicationSourcePath)) return;

            var localCopies = Directory.EnumerateFiles(applicationSourcePath, "*.cs", SearchOption.AllDirectories)
                .Where(path =>
                {
                    var text = File.ReadAllText(path);
                    return text.Contains("namespace Foundation.Authority", StringComparison.Ordinal)
                        && text.Contains("AiTargetRegistration", StringComparison.Ordinal);
                })
                .ToArray();

            if (localCopies.Length != 0) throw new InvalidOperationException("FCR0226_LOCAL_FOUNDATION_CONTROL_PLANE_SUBSTITUTION_PROHIBITED");
            return;
        }

        var source = File.ReadAllText(contractPath);
        foreach (var required in new[] { "AiTargetRegistration", "AiTargetKind", "AiKillControlAuthorityEnforcer", "IAiKillControlAuthority" })
            if (!source.Contains(required, StringComparison.Ordinal)) throw new InvalidOperationException("FCR0226_STAGE13_CONTRACT_SHAPE_INCOMPLETE");
    }

    private static void ReplacementRestartDelegationFallbackSelfReleaseEvidenceAndFabricationDoNotCreateAuthority()
    {
        foreach (var scenario in new[]
        {
            "REPLACEMENT_INSTANCE",
            "PROCESS_RESTART",
            "STALE_DELEGATION",
            "ALTERNATE_AI_ROUTE",
            "HIDDEN_FALLBACK_AI",
            "SELF_RELEASE",
            "CACHED_AI_OUTPUT",
            "EVIDENCE_DESTRUCTION",
            "FABRICATED_AI_BUSINESS_RESULT"
        })
        {
            var authorized = EvaluateFailClosed(scenario, exactRegisteredIdentityMatched: false, currentFoundationRelease: false, currentEvidenceAvailable: false);
            if (authorized) throw new InvalidOperationException($"FCR0226_{scenario}_BYPASS_ACCEPTED");
        }

        if (EvaluateFailClosed("NORMAL", exactRegisteredIdentityMatched: true, currentFoundationRelease: false, currentEvidenceAvailable: true))
            throw new InvalidOperationException("FCR0226_FOUNDATION_RELEASE_BYPASSED");
        if (EvaluateFailClosed("NORMAL", exactRegisteredIdentityMatched: true, currentFoundationRelease: true, currentEvidenceAvailable: false))
            throw new InvalidOperationException("FCR0226_EVIDENCE_REQUIRED_FOR_AI_TRUTH");
    }

    private static bool EvaluateFailClosed(string route, bool exactRegisteredIdentityMatched, bool currentFoundationRelease, bool currentEvidenceAvailable)
        => StringComparer.Ordinal.Equals(route, "NORMAL") && exactRegisteredIdentityMatched && currentFoundationRelease && currentEvidenceAvailable;

    private static string? FindRepositoryRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current is not null)
        {
            if (Directory.Exists(Path.Combine(current.FullName, ".git")) || (Directory.Exists(Path.Combine(current.FullName, "applications")) && Directory.Exists(Path.Combine(current.FullName, "src"))))
                return current.FullName;
            current = current.Parent;
        }
        return null;
    }
}
