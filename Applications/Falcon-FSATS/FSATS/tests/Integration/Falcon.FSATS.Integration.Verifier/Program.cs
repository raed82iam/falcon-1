using System.Text.Json;
using TA = Falcon.FSATS.Trading.Application;
using TW = Falcon.FSATS.Trading.Awareness;
using TD = Falcon.FSATS.Trading.Domain;
using PA = Falcon.FSATS.FSAPMA.Application;
using PW = Falcon.FSATS.FSAPMA.Awareness;
using PD = Falcon.FSATS.FSAPMA.Domain;
using GA = Falcon.FSATS.TradingGuardian.Application;
using GW = Falcon.FSATS.TradingGuardian.Awareness;
using GD = Falcon.FSATS.TradingGuardian.Domain;
using SA = Falcon.FSATS.FSTSimA.Application;
using SW = Falcon.FSATS.FSTSimA.Awareness;
using SD = Falcon.FSATS.FSTSimA.Domain;
using RA = Falcon.FSATS.ResourceManagement.Application;
using RW = Falcon.FSATS.ResourceManagement.Awareness;
using RD = Falcon.FSATS.ResourceManagement.Domain;

var failures = new List<string>();
var checks = 0;

Check(TA.TradingManifest.Current.LsaIds.Count == 13 && TA.TradingManifest.Current.CsaIds.Count == 3, "Trading manifest topology");
Check(PA.FSAPMAManifest.Current.LsaIds.Count == 6 && PA.FSAPMAManifest.Current.CsaIds.Count == 1, "FSAPMA manifest topology");
Check(GA.TradingGuardianManifest.Current.LsaIds.Count == 4 && GA.TradingGuardianManifest.Current.CsaIds.Count == 1, "Guardian manifest topology");
Check(SA.FSTSimAManifest.Current.LsaIds.Count == 8 && SA.FSTSimAManifest.Current.CsaIds.Count == 2, "FSTSimA manifest topology");
Check(RA.ResourceManagementManifest.Current.LsaIds.Count == 3 && RA.ResourceManagementManifest.Current.CsaIds.Count == 0, "APP-RSC manifest topology");

Check(TW.TradingAwarenessTopology.All.Count(x => x.Tier == TW.AwarenessTier.Msa) == 1, "Trading exactly one MSA");
Check(PW.FSAPMAAwarenessTopology.All.Count(x => x.Tier == PW.AwarenessTier.Msa) == 1, "FSAPMA exactly one MSA");
Check(GW.GuardianAwarenessTopology.All.Count(x => x.Tier == GW.AwarenessTier.Msa) == 1, "Guardian exactly one MSA");
Check(SW.FSTSimAAwarenessTopology.All.Count(x => x.Tier == SW.AwarenessTier.Msa) == 1, "FSTSimA exactly one MSA");
Check(RW.ResourceManagementAwarenessTopology.All.Count(x => x.Tier == RW.AwarenessTier.Msa) == 1, "APP-RSC exactly one MSA");

var totalLsa = TW.TradingAwarenessTopology.All.Count(x => x.Tier == TW.AwarenessTier.Lsa)
    + PW.FSAPMAAwarenessTopology.All.Count(x => x.Tier == PW.AwarenessTier.Lsa)
    + GW.GuardianAwarenessTopology.All.Count(x => x.Tier == GW.AwarenessTier.Lsa)
    + SW.FSTSimAAwarenessTopology.All.Count(x => x.Tier == SW.AwarenessTier.Lsa)
    + RW.ResourceManagementAwarenessTopology.All.Count(x => x.Tier == RW.AwarenessTier.Lsa);
var totalCsa = TW.TradingAwarenessTopology.All.Count(x => x.Tier == TW.AwarenessTier.Csa)
    + PW.FSAPMAAwarenessTopology.All.Count(x => x.Tier == PW.AwarenessTier.Csa)
    + GW.GuardianAwarenessTopology.All.Count(x => x.Tier == GW.AwarenessTier.Csa)
    + SW.FSTSimAAwarenessTopology.All.Count(x => x.Tier == SW.AwarenessTier.Csa)
    + RW.ResourceManagementAwarenessTopology.All.Count(x => x.Tier == RW.AwarenessTier.Csa);
Check(totalLsa == 34, $"Expected 34 LSA, found {totalLsa}");
Check(totalCsa == 7, $"Expected 7 CSA, found {totalCsa}");

Check(typeof(TD.OpportunityDiscoveryEngine).Name == "OpportunityDiscoveryEngine", "Trading CSA target exists");
Check(typeof(TD.StrategyController).Name == "StrategyController", "StrategyController CSA target exists");
Check(typeof(TD.StrategyEvolutionEngine).Name == "StrategyEvolutionEngine", "StrategyEvolutionEngine CSA target exists");
Check(typeof(PD.AnomalyDetector).Name == "AnomalyDetector", "AnomalyDetector CSA target exists");
Check(typeof(GD.IncidentClassifier).Name == "IncidentClassifier", "IncidentClassifier CSA target exists");
Check(typeof(SD.SyntheticMarketGenerator).Name == "SyntheticMarketGenerator", "SyntheticMarketGenerator CSA target exists");
Check(typeof(SD.CalibrationEngine).Name == "CalibrationEngine", "CalibrationEngine CSA target exists");

Check(!TA.TradingManifest.Current.RuntimeAuthorized && !TA.TradingManifest.Current.ExternalEgressAuthorized, "Trading runtime/egress remains disabled");
Check(!PA.FSAPMAManifest.Current.RuntimeAuthorized && !PA.FSAPMAManifest.Current.ProviderEgressAuthorized, "FSAPMA runtime/provider egress remains disabled");
Check(!GA.TradingGuardianManifest.Current.RuntimeAuthorized && !GA.TradingGuardianManifest.Current.ProtectionRouteBound, "Guardian route remains unbound");
Check(!SA.FSTSimAManifest.Current.RuntimeAuthorized && !SA.FSTSimAManifest.Current.OperationalEgressAuthorized && !SA.FSTSimAManifest.Current.PaperAuthority, "FSTSimA remains non-Live/no-Paper-authority");
Check(!RA.ResourceManagementManifest.Current.RuntimeAuthorized && !RA.ResourceManagementManifest.Current.FoundationResourceBindingBound, "APP-RSC exact Foundation binding remains unbound");
Check(!GW.GuardianAwarenessRules.CanControlDeterministicSafetyKernel("CSA-G01-01"), "Guardian CSA cannot control deterministic Safety Kernel");
Check(SW.ValidationIndependence.IsCsaForbiddenInitially("ValidationAssessor"), "ValidationAssessor remains non-CSA");
Check(RW.ResourceManagementAwarenessTopology.InitialCsaCount == 0, "APP-RSC initial CSA count remains zero");

var repo = FindRepoRoot();
var catalogPath = Path.Combine(repo, "applications", "FSATS", "contracts", "part1-contract-catalog.json");
using var catalog = JsonDocument.Parse(File.ReadAllText(catalogPath));
var root = catalog.RootElement;
Check(!root.GetProperty("runtimeRoutesActive").GetBoolean(), "Contract catalog must not activate runtime routes");
var families = root.GetProperty("families").EnumerateArray().ToArray();
Check(families.Length == 22, $"Expected 22 Part 1 contract families, found {families.Length}");
Check(families.Select(x => x.GetProperty("id").GetString()).Distinct(StringComparer.Ordinal).Count() == 22, "Contract family IDs unique");
Check(families.All(x => !x.GetProperty("routeActive").GetBoolean()), "All declared contract routes remain inactive");

if (failures.Count > 0)
{
    Console.Error.WriteLine($"FSATS INTEGRATION VERIFIER: FAIL ({checks - failures.Count}/{checks})");
    foreach (var failure in failures) Console.Error.WriteLine(" - " + failure);
    return 1;
}

Console.WriteLine($"FSATS INTEGRATION VERIFIER: PASS ({checks}/{checks}; 5 MSA / 34 LSA / 7 CSA / 22 contract families)");
return 0;

void Check(bool condition, string message) { checks++; if (!condition) failures.Add(message); }

static string FindRepoRoot()
{
    var dir = new DirectoryInfo(Directory.GetCurrentDirectory());
    while (dir is not null)
    {
        if (File.Exists(Path.Combine(dir.FullName, "Falcon.Foundation.ControlledProjectFoundation.slnx"))) return dir.FullName;
        dir = dir.Parent;
    }
    throw new InvalidOperationException("Repository root not found");
}
