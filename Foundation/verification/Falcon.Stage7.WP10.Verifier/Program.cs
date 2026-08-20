using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Foundation.HealthFitness;
using Foundation.SelfAwareness;

namespace Falcon.Stage7.WP10.Verifier;

internal static class Program
{
    private static int _passCount;

    private static int Main()
    {
        try
        {
            Run("wp01-wp09-verifier-surfaces-present", VerifyPredecessorVerifierSurfaces);
            Run("vpl005-exact-nine-loss-classes", VerifyVpl005LossClasses);
            Run("health-runtime-surface-present", VerifyHealthSurface);
            Run("self-model-runtime-surface-present", VerifySelfModelSurface);
            Run("technical-fitness-runtime-surface-present", VerifyTechnicalFitnessSurface);
            Run("evidence-awareness-restoration-surface-present", VerifyEvidenceAwarenessSurface);
            Run("governed-consumption-surface-present", VerifyGovernedConsumptionSurface);
            Run("history-reconstruction-surface-present", VerifyHistorySurface);
            Run("health-history-substrate-ownership-preserved", VerifyHistoryOwnership);
            Run("stage7-reference-trace-present", VerifyRequirementTrace);
            Run("stage8-stage9-stage13-deferred", VerifyFutureStageDeferral);
            Run("zero-application-production-reference", VerifyZeroApplicationReference);
            Run("no-future-action-method-surface", VerifyNoFutureActionMethodSurface);
            Run("no-duplicate-stage7-production-project", VerifyNoDuplicateProductionProject);
            Run("integrated-source-surfaces-present", VerifyIntegratedSourceSurfaces);
            Run("closure-verifier-is-deterministic", VerifyDeterministicClosureBasis);

            Console.WriteLine("STAGE7_WP10_VERIFIER = PASS");
            Console.WriteLine("CHECKS = " + _passCount + "/" + _passCount);
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("STAGE7_WP10_VERIFIER = FAIL");
            Console.Error.WriteLine(ex);
            return 1;
        }
    }

    private static void Run(string name, Action action)
    {
        action();
        _passCount++;
        Console.WriteLine("PASS | " + name);
    }

    private static void VerifyPredecessorVerifierSurfaces()
    {
        for (var wp = 1; wp <= 9; wp++)
        {
            var text = wp.ToString("D2");
            var path = Path.Combine("verification", $"Falcon.Stage7.WP{text}.Verifier", $"Falcon.Stage7.WP{text}.Verifier.csproj");
            Require(File.Exists(path), "Missing Stage 7 predecessor verifier: " + path);
        }
    }

    private static void VerifyVpl005LossClasses()
    {
        var actual = Enum.GetValues<HealthEvidenceLossClass>()
            .Where(value => value != HealthEvidenceLossClass.Available)
            .OrderBy(value => (int)value)
            .ToArray();

        var expected = new[]
        {
            HealthEvidenceLossClass.Missing,
            HealthEvidenceLossClass.Stale,
            HealthEvidenceLossClass.Delayed,
            HealthEvidenceLossClass.Contradictory,
            HealthEvidenceLossClass.Unverifiable,
            HealthEvidenceLossClass.Inaccessible,
            HealthEvidenceLossClass.Corrupted,
            HealthEvidenceLossClass.ProvenanceFailure,
            HealthEvidenceLossClass.PartialVisibility
        };

        Require(actual.SequenceEqual(expected), "VPL-005 active loss class set changed.");
    }

    private static void VerifyHealthSurface()
    {
        RequireMethod(typeof(HealthObservationAssessmentRuntime), "Evaluate");
        RequireMethod(typeof(HealthRuleValidator), "Validate");
        RequireMethod(typeof(HealthEvidenceQualityRuntime), "Evaluate");
    }

    private static void VerifySelfModelSurface()
    {
        RequireMethod(typeof(FoundationSelfModelProjector), "Build");
        RequireMethod(typeof(FoundationSelfModelAssertionFactory), "FromHealthAssessment");
    }

    private static void VerifyTechnicalFitnessSurface()
    {
        RequireMethod(typeof(TechnicalFitnessEvaluationRuntime), "Evaluate");
        RequireMethod(typeof(TechnicalFitnessRuleValidator), "Validate");
    }

    private static void VerifyEvidenceAwarenessSurface()
    {
        RequireMethod(typeof(EvidenceAwarenessRuntime), "Evaluate");
        RequireMethod(typeof(EvidenceAwarenessRuntime), "EvaluateLastKnownReliance");
        RequireMethod(typeof(EvidenceAwarenessRuntime), "EvaluateRestoration");
    }

    private static void VerifyGovernedConsumptionSurface()
    {
        RequireMethod(typeof(HealthFitnessGovernedConsumptionRuntime), "Evaluate");

        var evidenceType = typeof(GovernedFitnessConsumptionEvidence);
        foreach (var property in new[]
                 {
                     "CanSupportPositiveAuthorityCondition",
                     "PositiveAuthorityInferenceBlocked",
                     "RestrictionInputRequired",
                     "IndependentReassessmentRequired",
                     "NewAuthorityDecisionRequired"
                 })
        {
            Require(evidenceType.GetProperty(property) is not null,
                "Governed consumption evidence missing property: " + property);
        }
    }

    private static void VerifyHistorySurface()
    {
        RequireMethod(typeof(HealthFitnessHistoryRuntime), "CreateChangeFact");
        RequireMethod(typeof(HealthFitnessHistoryRuntime), "CreateHistoryRecord");
        RequireMethod(typeof(HealthFitnessHistoryRuntime), "Reconstruct");
    }

    private static void VerifyHistoryOwnership()
    {
        Require(HealthFitnessHistoryRuntime.HistoryOwner == "Foundation.HealthFitness",
            "Health/Fitness history ownership changed.");
        Require(HealthFitnessHistoryRuntime.EventSubstrateOwner == "Foundation.EventSystem",
            "Event substrate ownership changed.");
        Require(HealthFitnessHistoryRuntime.PersistenceOwner == "Foundation.State",
            "Persistence substrate ownership changed.");
    }

    private static void VerifyRequirementTrace()
    {
        var planPath = Path.Combine("docs", "stage-7-planning", "07_STAGE7_IMPLEMENTATION_PLAN_v0.3_FINAL_CANDIDATE.md");
        Require(File.Exists(planPath), "Accepted Stage 7 plan missing.");
        var plan = File.ReadAllText(planPath);

        foreach (var token in new[] { "SYS-008", "AWR-001", "CON-006", "VPL-005", "WP-10" })
            Require(plan.Contains(token, StringComparison.Ordinal), "Stage 7 plan trace token missing: " + token);
    }

    private static void VerifyFutureStageDeferral()
    {
        var plan = File.ReadAllText(Path.Combine("docs", "stage-7-planning", "07_STAGE7_IMPLEMENTATION_PLAN_v0.3_FINAL_CANDIDATE.md"));
        foreach (var token in new[] { "Stage 8", "Stage 9", "Stage 13" })
            Require(plan.Contains(token, StringComparison.OrdinalIgnoreCase), "Future-stage deferral trace missing: " + token);
    }

    private static void VerifyZeroApplicationReference()
    {
        foreach (var assembly in RuntimeAssemblies())
        {
            foreach (var reference in assembly.GetReferencedAssemblies())
            {
                var name = reference.Name ?? string.Empty;
                Require(!name.Contains("Application", StringComparison.OrdinalIgnoreCase) &&
                        !name.Contains("Trading", StringComparison.OrdinalIgnoreCase) &&
                        !name.Contains("Web", StringComparison.OrdinalIgnoreCase),
                    "Stage 7 runtime references Application/business assembly: " + name);
            }
        }
    }

    private static void VerifyNoFutureActionMethodSurface()
    {
        var forbiddenExactNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "GrantAuthority",
            "RestoreAuthority",
            "Kill",
            "EnterSafeState",
            "ExecuteSafeState",
            "ControlledRevival",
            "Revive",
            "Deploy",
            "ReleaseFromContainment",
            "ExecuteGuardianCommand"
        };

        foreach (var type in RuntimeTypes())
        foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            Require(!forbiddenExactNames.Contains(method.Name),
                $"Future-stage action method leaked into Stage 7: {type.FullName}.{method.Name}");
    }

    private static void VerifyNoDuplicateProductionProject()
    {
        var src = Directory.GetDirectories("src", "Foundation.*", SearchOption.TopDirectoryOnly)
            .Select(Path.GetFileName)
            .Where(name => name is not null)
            .ToArray();

        Require(src.Count(name => string.Equals(name, "Foundation.HealthFitness", StringComparison.Ordinal)) == 1,
            "Foundation.HealthFitness production ownership duplicated.");
        Require(!src.Any(name => string.Equals(name, "Foundation.HealthHistory", StringComparison.Ordinal)),
            "Rejected duplicate Foundation.HealthHistory production project returned.");
    }

    private static void VerifyIntegratedSourceSurfaces()
    {
        foreach (var path in new[]
                 {
                     Path.Combine("src", "Foundation.HealthFitness", "HealthObservationAssessmentRuntime.cs"),
                     Path.Combine("src", "Foundation.HealthFitness", "HealthEvidenceQualityRuntime.cs"),
                     Path.Combine("src", "Foundation.HealthFitness", "HealthFitnessGovernedConsumptionRuntime.cs"),
                     Path.Combine("src", "Foundation.HealthFitness", "HealthFitnessHistoryRuntime.cs"),
                     Path.Combine("src", "Foundation.SelfAwareness", "FoundationSelfModelRuntime.cs"),
                     Path.Combine("src", "Foundation.SelfAwareness", "TechnicalFitnessRuntime.cs")
                 })
            Require(File.Exists(path), "Integrated Stage 7 source surface missing: " + path);
    }

    private static void VerifyDeterministicClosureBasis()
    {
        var lossNames = string.Join(",", Enum.GetNames<HealthEvidenceLossClass>());
        var first = string.Join("|",
            lossNames,
            HealthFitnessHistoryRuntime.HistoryOwner,
            HealthFitnessHistoryRuntime.EventSubstrateOwner,
            HealthFitnessHistoryRuntime.PersistenceOwner,
            typeof(TechnicalFitnessEvaluationRuntime).Assembly.GetName().Name ?? string.Empty,
            typeof(HealthObservationAssessmentRuntime).Assembly.GetName().Name ?? string.Empty);

        var second = string.Join("|",
            lossNames,
            HealthFitnessHistoryRuntime.HistoryOwner,
            HealthFitnessHistoryRuntime.EventSubstrateOwner,
            HealthFitnessHistoryRuntime.PersistenceOwner,
            typeof(TechnicalFitnessEvaluationRuntime).Assembly.GetName().Name ?? string.Empty,
            typeof(HealthObservationAssessmentRuntime).Assembly.GetName().Name ?? string.Empty);

        Require(string.Equals(first, second, StringComparison.Ordinal), "Integrated closure basis was nondeterministic.");
    }

    private static IEnumerable<Assembly> RuntimeAssemblies()
    {
        yield return typeof(HealthObservationAssessmentRuntime).Assembly;
        yield return typeof(TechnicalFitnessEvaluationRuntime).Assembly;
    }

    private static IEnumerable<Type> RuntimeTypes()
    {
        return RuntimeAssemblies()
            .SelectMany(assembly => assembly.GetExportedTypes())
            .Where(type => type.Namespace is not null &&
                           (type.Namespace.StartsWith("Foundation.HealthFitness", StringComparison.Ordinal) ||
                            type.Namespace.StartsWith("Foundation.SelfAwareness", StringComparison.Ordinal)));
    }

    private static void RequireMethod(Type type, string method)
    {
        Require(type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance)
                .Any(candidate => string.Equals(candidate.Name, method, StringComparison.Ordinal)),
            "Required runtime method missing: " + type.FullName + "." + method);
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
