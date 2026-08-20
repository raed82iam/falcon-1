using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Foundation.Contracts;
using Foundation.HealthFitness;
using Foundation.SelfAwareness;

namespace Falcon.Stage7.WP04.Verifier;

internal static class Program
{
    private static readonly DateTimeOffset T = new(2026, 8, 13, 8, 0, 0, TimeSpan.Zero);

    private static int Main()
    {
        try
        {
            VerifyFixedMappings();
            VerifyRecoveryDefaultAndException();
            VerifyRecoveryDenials();
            VerifyEvidenceFailureModes();
            VerifyDeterminismAndProjection();
            VerifyBoundary();
            Console.WriteLine("STAGE7_WP04_VERIFIER=PASS");
            return 0;
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine("STAGE7_WP04_VERIFIER=FAIL");
            Console.Error.WriteLine(exception);
            return 1;
        }
    }

    private static void VerifyFixedMappings()
    {
        Map(TechnicalFitnessState.FitWithConstraints, FitnessProjectionResult.Restricted, "constraint:bounded");
        Map(TechnicalFitnessState.Degraded, FitnessProjectionResult.Restricted, "constraint:degraded");
        Map(TechnicalFitnessState.Unknown, FitnessProjectionResult.NotFit, "NONE");
        Map(TechnicalFitnessState.Unavailable, FitnessProjectionResult.NotFit, "NONE");
        Map(TechnicalFitnessState.IntegrityFailure, FitnessProjectionResult.NotFit, "NONE");
        Map(TechnicalFitnessState.IsolationRequired, FitnessProjectionResult.Restricted, "constraint:isolation");
        Map(TechnicalFitnessState.NotFit, FitnessProjectionResult.NotFit, "NONE");

        var fit = Evaluate(Rule(Req("fitness:req:runtime", FoundationSelfModelArea.RuntimeCondition,
            "technical:value:runtime:usable", TechnicalFitnessState.NotFit, "NONE")), Model());
        Require(fit.TechnicalFitnessState == TechnicalFitnessState.Fit, "Satisfied requirements did not produce FIT.");
        Require(fit.FitnessResult == FitnessProjectionResult.Fit, "FIT did not map to CON-006 FIT.");
    }

    private static void Map(TechnicalFitnessState state, FitnessProjectionResult expected, string constraint)
    {
        var result = Evaluate(Rule(Req("fitness:req:mapping:" + state.ToString().ToLowerInvariant(),
            FoundationSelfModelArea.RuntimeCondition, "technical:value:runtime:impossible", state, constraint)), Model());
        Require(result.TechnicalFitnessState == state, "Technical state changed for " + state + ".");
        Require(result.FitnessResult == expected, "CON-006 mapping mismatch for " + state + ".");
    }

    private static void VerifyRecoveryDefaultAndException()
    {
        var defaultResult = Evaluate(RecoveryRule(null), Model());
        Require(defaultResult.TechnicalFitnessState == TechnicalFitnessState.RecoveryRequired,
            "RECOVERY_REQUIRED state was not preserved.");
        Require(defaultResult.FitnessResult == FitnessProjectionResult.NotFit,
            "RECOVERY_REQUIRED did not default to NOT_FIT.");

        var restricted = Evaluate(RecoveryRule(RecoveryDeclaration()), Model());
        Require(restricted.TechnicalFitnessState == TechnicalFitnessState.RecoveryRequired,
            "Recovery exception changed the technical source state.");
        Require(restricted.FitnessResult == FitnessProjectionResult.Restricted,
            "Fully evidenced recovery exception did not produce RESTRICTED.");
        Require(restricted.Constraints.Contains("recovery-bounded", StringComparison.Ordinal),
            "Recovery RESTRICTED constraint missing.");
    }

    private static void VerifyRecoveryDenials()
    {
        var declaration = RecoveryDeclaration();
        var missing = declaration with
        {
            Proofs = declaration.Proofs.Select(proof =>
                proof.Condition == RecoveryRestrictedCondition.CapabilityIndependentOfAffectedPath
                    ? proof with { AssertionId = "selfmodel:assertion:missing" }
                    : proof).ToArray()
        };
        Require(Evaluate(RecoveryRule(missing), Model()).FitnessResult == FitnessProjectionResult.NotFit,
            "Missing recovery proof did not fail closed.");

        var nonIndependentModel = Model(assertions => Replace(assertions, FoundationSelfModelArea.RuntimeCondition,
            assertion => assertion with { SourceOwner = declaration.FaultSourceOwner }));
        Require(Evaluate(RecoveryRule(declaration), nonIndependentModel).FitnessResult == FitnessProjectionResult.NotFit,
            "Same-origin usability evidence was treated as independent.");

        var spoofed = declaration with { FaultSourceOwner = "owner:foundation:spoofed-fault" };
        var spoofedResult = Evaluate(RecoveryRule(spoofed), Model());
        Require(spoofedResult.FitnessResult == FitnessProjectionResult.NotFit,
            "Spoofed fault owner enabled recovery RESTRICTED.");
        Require(spoofedResult.Reason.Contains("FAULT_SOURCE_BINDING_FAILED", StringComparison.Ordinal),
            "Fault-source binding denial reason missing.");

        var badScope = declaration with
        {
            Proofs = declaration.Proofs.Select(proof =>
                proof.Condition == RecoveryRestrictedCondition.FaultTechnicallyIsolated
                    ? proof with { Scope = "foundation:other-scope" }
                    : proof).ToArray()
        };
        Require(TechnicalFitnessRuleValidator.Validate(RecoveryRule(badScope)).Result != ValidationResult.Pass,
            "Recovery proof escaped exact scope binding.");

        var secondBlockerRule = Rule(new[]
        {
            Req("fitness:req:recovery", FoundationSelfModelArea.RecoveryReadiness,
                "technical:value:recovery:not-required", TechnicalFitnessState.RecoveryRequired, "NONE", 100),
            Req("fitness:req:integrity", FoundationSelfModelArea.CoreComponentIntegrity,
                "technical:value:integrity:trusted", TechnicalFitnessState.IntegrityFailure, "NONE", 90)
        }, declaration);
        var integrityFailed = Model(assertions => Replace(assertions, FoundationSelfModelArea.CoreComponentIntegrity,
            assertion => assertion with { ValueIdentity = "technical:value:integrity:failed" }));
        Require(Evaluate(secondBlockerRule, integrityFailed).FitnessResult == FitnessProjectionResult.NotFit,
            "Recovery exception overrode another NOT_FIT blocker.");
    }

    private static void VerifyEvidenceFailureModes()
    {
        var missingRule = Rule(Req("fitness:req:missing", FoundationSelfModelArea.RuntimeCondition,
            "technical:value:runtime:usable", TechnicalFitnessState.NotFit, "NONE", subject: "foundation:missing"));
        var missing = Evaluate(missingRule, Model());
        Require(missing.TechnicalFitnessState == TechnicalFitnessState.Unknown &&
                missing.FitnessResult == FitnessProjectionResult.NotFit &&
                missing.Unknowns.Contains("MISSING_CURRENT_EVIDENCE", StringComparison.Ordinal),
            "Missing current evidence did not fail closed.");

        var unknownModel = Model(assertions => Replace(assertions, FoundationSelfModelArea.BackupCondition,
            assertion => assertion with
            {
                AssertionKind = FoundationSelfModelAssertionKind.Unknown,
                EvidenceQuality = EvidenceQuality.Insufficient,
                Confidence = "INSUFFICIENT",
                Uncertainty = "backup-current-unknown"
            }));
        var unknown = Evaluate(Rule(Req("fitness:req:backup", FoundationSelfModelArea.BackupCondition,
            "technical:value:backup:ready", TechnicalFitnessState.NotFit, "NONE")), unknownModel);
        Require(unknown.TechnicalFitnessState == TechnicalFitnessState.Unknown &&
                unknown.FitnessResult == FitnessProjectionResult.NotFit,
            "Explicit UNKNOWN evidence did not fail closed.");

        var limitedModel = Model(assertions => Replace(assertions, FoundationSelfModelArea.RuntimeCondition,
            assertion => assertion with
            {
                EvidenceQuality = EvidenceQuality.Limited,
                Confidence = "LIMITED",
                Uncertainty = "bounded-runtime-visibility"
            }));
        var limitedRule = Rule(Req("fitness:req:limited", FoundationSelfModelArea.RuntimeCondition,
            "technical:value:runtime:usable", TechnicalFitnessState.NotFit,
            "constraint:limited-evidence", limited: TechnicalFitnessState.Degraded));
        var limited = Evaluate(limitedRule, limitedModel);
        Require(limited.FitnessResult == FitnessProjectionResult.Restricted,
            "EQ-LIMITED incorrectly produced unrestricted FIT.");

        var assertions = Assertions().ToList();
        var runtime = assertions.Single(assertion => assertion.Area == FoundationSelfModelArea.RuntimeCondition);
        assertions.Add(runtime with
        {
            AssertionId = "selfmodel:assertion:runtimecondition:conflict",
            ValueIdentity = "technical:value:runtime:conflict"
        });
        var contradictionModel = FoundationSelfModelProjector.Build(
            "selfmodel:wp04:contradiction", "foundation", "baseline:accepted", T, assertions, null);
        var contradiction = Evaluate(Rule(Req("fitness:req:runtime", FoundationSelfModelArea.RuntimeCondition,
            "technical:value:runtime:usable", TechnicalFitnessState.NotFit, "NONE")), contradictionModel);
        Require(contradiction.TechnicalFitnessState == TechnicalFitnessState.Unknown && contradiction.Contradictions != "NONE",
            "Current contradiction did not fail closed or remain visible.");

        var expiringModel = Model(values => Replace(values, FoundationSelfModelArea.RuntimeCondition,
            assertion => assertion with { Expiry = T.AddSeconds(7) }));
        var stale = TechnicalFitnessEvaluationRuntime.Evaluate(
            "fitness:assessment:stale", Rule(Req("fitness:req:runtime", FoundationSelfModelArea.RuntimeCondition,
                "technical:value:runtime:usable", TechnicalFitnessState.NotFit, "NONE")),
            expiringModel, T.AddSeconds(10), T.AddSeconds(30));
        Require(stale.FitnessResult == FitnessProjectionResult.NotFit &&
                stale.EvidenceQuality == EvidenceQuality.Insufficient &&
                stale.Unknowns.Contains("STALE_CURRENT_EVIDENCE", StringComparison.Ordinal),
            "Evidence expiring after Self Model time remained positive at Fitness time.");

        var expiryClamp = TechnicalFitnessEvaluationRuntime.Evaluate(
            "fitness:assessment:expiry", Rule(Req("fitness:req:runtime", FoundationSelfModelArea.RuntimeCondition,
                "technical:value:runtime:usable", TechnicalFitnessState.NotFit, "NONE")),
            expiringModel, T, T.AddSeconds(30));
        Require(expiryClamp.Expiry == T.AddSeconds(7), "Positive Fitness outlived supporting evidence.");

        var directCircularModel = Model(values => Replace(values, FoundationSelfModelArea.RuntimeCondition,
            assertion => assertion with { SourceAssessmentReference = "fitness:assessment:wp04" }));
        var directCircular = Evaluate(Rule(Req("fitness:req:runtime", FoundationSelfModelArea.RuntimeCondition,
            "technical:value:runtime:usable", TechnicalFitnessState.NotFit, "NONE")), directCircularModel);
        Require(directCircular.FitnessResult == FitnessProjectionResult.NotFit &&
                directCircular.Unknowns.Contains("DIRECT_CIRCULAR_SELF_REFERENCE", StringComparison.Ordinal),
            "Direct circular proof was accepted.");

        var circularRule = Rule(Req("fitness:req:circular", FoundationSelfModelArea.TechnicalFitness,
            "technical:value:fitness:fit", TechnicalFitnessState.NotFit, "NONE"));
        Require(TechnicalFitnessRuleValidator.Validate(circularRule).Result != ValidationResult.Pass,
            "Self Model TechnicalFitness was accepted as a Fitness input.");
    }

    private static void VerifyDeterminismAndProjection()
    {
        var a = Req("fitness:req:runtime", FoundationSelfModelArea.RuntimeCondition,
            "technical:value:runtime:usable", TechnicalFitnessState.NotFit, "NONE");
        var b = Req("fitness:req:security", FoundationSelfModelArea.SecurityCondition,
            "technical:value:trust:clear", TechnicalFitnessState.IntegrityFailure, "NONE");
        var first = Evaluate(Rule(new[] { a, b }), Model(), "fitness:assessment:deterministic");
        var second = Evaluate(Rule(new[] { b, a }), Model(), "fitness:assessment:deterministic");
        Require(first.Identity == second.Identity && first.EvidenceReference == second.EvidenceReference,
            "Requirement ordering changed deterministic Fitness identity.");

        var contract = HealthFitnessContractProjection.ToContractV12(first);
        Require(contract.Version == "1.2" && HealthFitnessV12Validator.Validate(contract).Result == ValidationResult.Pass,
            "WP-04 did not project exact valid CON-006 v1.2.");
    }

    private static void VerifyBoundary()
    {
        var assembly = typeof(TechnicalFitnessEvaluationRuntime).Assembly;
        var references = assembly.GetReferencedAssemblies().Select(reference => reference.Name ?? string.Empty)
            .Where(name => name.StartsWith("Foundation.", StringComparison.Ordinal))
            .OrderBy(name => name, StringComparer.Ordinal).ToArray();
        Require(references.SequenceEqual(new[] { "Foundation.Contracts", "Foundation.HealthFitness" }, StringComparer.Ordinal),
            "WP-04 changed the SelfAwareness production dependency boundary.");

        var forbiddenActions = new[] { "Grant", "Revoke", "Restrict", "Isolate", "Kill", "Recover", "Release", "Revive", "Deploy", "Activate", "Transition" };
        var forbiddenBusiness = new[] { "Trading", "Market", "Portfolio", "Broker", "Strategy", "SharedWeb", "WebBusiness", "MonitorAI", "FactoryReset", "ControlledRevival" };
        foreach (var type in assembly.GetExportedTypes())
        {
            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly))
                Require(!forbiddenActions.Any(prefix => method.Name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)),
                    "Forbidden action surface: " + type.FullName + "." + method.Name);
            var text = (type.FullName ?? type.Name) + " " + string.Join(" ", type.GetMembers().Select(member => member.Name));
            Require(!forbiddenBusiness.Any(token => text.Contains(token, StringComparison.OrdinalIgnoreCase)),
                "Later-stage/Application/Web semantic leaked into WP-04 surface: " + text);
        }
    }

    private static CanonicalHealthFitnessAssessment Evaluate(
        TechnicalFitnessRuleDefinition rule,
        FoundationSelfModelSnapshot model,
        string id = "fitness:assessment:wp04") =>
        TechnicalFitnessEvaluationRuntime.Evaluate(id, rule, model, T, T.AddSeconds(30));

    private static TechnicalFitnessRuleDefinition Rule(
        TechnicalFitnessRequirement requirement,
        RecoveryRestrictedModeDeclaration? recovery = null) => Rule(new[] { requirement }, recovery);

    private static TechnicalFitnessRuleDefinition Rule(
        IReadOnlyList<TechnicalFitnessRequirement> requirements,
        RecoveryRestrictedModeDeclaration? recovery = null)
    {
        var all = requirements.Where(requirement => requirement.Area != FoundationSelfModelArea.HealthCondition)
            .Append(new TechnicalFitnessRequirement(
                "fitness:req:health-current", FoundationSelfModelArea.HealthCondition,
                "foundation", "foundation",
                new[] { "health-state:healthy", "health-state:degraded", "health-state:unhealthy", "health-state:not-applicable", "health-state:notapplicable" },
                TechnicalFitnessState.Unknown, TechnicalFitnessState.Unknown, 1000, "NONE", null))
            .ToArray();
        return new TechnicalFitnessRuleDefinition(
            "fitness:rule:wp04", "1.0", "foundation", "foundation:technical-operation",
            "authority:level:technical", "foundation", all, recovery);
    }

    private static TechnicalFitnessRuleDefinition RecoveryRule(RecoveryRestrictedModeDeclaration? recovery) =>
        Rule(Req("fitness:req:recovery", FoundationSelfModelArea.RecoveryReadiness,
            "technical:value:recovery:not-required", TechnicalFitnessState.RecoveryRequired, "NONE", 100), recovery);

    private static TechnicalFitnessRequirement Req(
        string id,
        FoundationSelfModelArea area,
        string acceptable,
        TechnicalFitnessState failure,
        string constraint,
        int priority = 50,
        TechnicalFitnessState limited = TechnicalFitnessState.Unknown,
        string subject = "foundation") =>
        new(id, area, subject, "foundation", new[] { acceptable }, failure, limited, priority, constraint, null);

    private static RecoveryRestrictedModeDeclaration RecoveryDeclaration() =>
        new("owner:foundation:recovery-fault", "recovery-bounded:read-only-operation", new[]
        {
            Proof(RecoveryRestrictedCondition.FaultTechnicallyIsolated, FoundationSelfModelArea.IsolationReadiness, "technical:value:isolation:ready"),
            Proof(RecoveryRestrictedCondition.CapabilityIndependentOfAffectedPath, FoundationSelfModelArea.DependencyAvailability, "technical:value:dependency:independent"),
            Proof(RecoveryRestrictedCondition.IndependentUsabilityProven, FoundationSelfModelArea.RuntimeCondition, "technical:value:runtime:usable"),
            Proof(RecoveryRestrictedCondition.TrustBoundaryClear, FoundationSelfModelArea.SecurityCondition, "technical:value:trust:clear"),
            Proof(RecoveryRestrictedCondition.TrustBoundaryClear, FoundationSelfModelArea.CoreComponentIntegrity, "technical:value:integrity:trusted"),
            Proof(RecoveryRestrictedCondition.TrustBoundaryClear, FoundationSelfModelArea.AuthorityCondition, "technical:value:authority:trusted"),
            Proof(RecoveryRestrictedCondition.TrustBoundaryClear, FoundationSelfModelArea.FoundationIdentity, "technical:value:identity:trusted"),
            Proof(RecoveryRestrictedCondition.TrustBoundaryClear, FoundationSelfModelArea.ContradictionCondition, "technical:value:contradiction:none")
        });

    private static RecoveryRestrictedConditionProof Proof(
        RecoveryRestrictedCondition condition,
        FoundationSelfModelArea area,
        string value) =>
        new(condition, "selfmodel:assertion:" + area.ToString().ToLowerInvariant(), area,
            "foundation", "foundation", value, null);

    private static FoundationSelfModelSnapshot Model(
        Func<IReadOnlyList<FoundationSelfModelAssertion>, IReadOnlyList<FoundationSelfModelAssertion>>? mutate = null)
    {
        IReadOnlyList<FoundationSelfModelAssertion> assertions = Assertions();
        if (mutate is not null) assertions = mutate(assertions);
        return FoundationSelfModelProjector.Build("selfmodel:wp04", "foundation", "baseline:accepted", T, assertions, null);
    }

    private static FoundationSelfModelAssertion[] Assertions() =>
        Enum.GetValues<FoundationSelfModelArea>().Select(area =>
        {
            var unknown = area == FoundationSelfModelArea.TechnicalFitness;
            return new FoundationSelfModelAssertion(
                "selfmodel:assertion:" + area.ToString().ToLowerInvariant(), "foundation", area,
                unknown ? FoundationSelfModelAssertionKind.Unknown : FoundationSelfModelAssertionKind.Fact,
                FoundationSelfModelTemporalView.Current, "foundation", Value(area),
                "source:" + area.ToString().ToLowerInvariant(),
                area == FoundationSelfModelArea.RecoveryReadiness
                    ? "owner:foundation:recovery-fault"
                    : "owner:foundation:" + area.ToString().ToLowerInvariant(),
                "evidence:" + area.ToString().ToLowerInvariant(),
                unknown ? EvidenceQuality.Insufficient : EvidenceQuality.Sufficient,
                unknown ? "INSUFFICIENT" : "SUFFICIENT",
                unknown ? "producer=wp04-not-yet-evaluated" : "NONE",
                "freshness:wp04", "selfmodel:rule:wp04", "1.0",
                T.AddSeconds(-5), T.AddSeconds(-4), T.AddSeconds(20),
                area == FoundationSelfModelArea.HealthCondition ? "health:assessment:wp02" : null, null);
        }).ToArray();

    private static string Value(FoundationSelfModelArea area) => area switch
    {
        FoundationSelfModelArea.HealthCondition => "health-state:healthy",
        FoundationSelfModelArea.RuntimeCondition => "technical:value:runtime:usable",
        FoundationSelfModelArea.SecurityCondition => "technical:value:trust:clear",
        FoundationSelfModelArea.DependencyAvailability => "technical:value:dependency:independent",
        FoundationSelfModelArea.IsolationReadiness => "technical:value:isolation:ready",
        FoundationSelfModelArea.RecoveryReadiness => "technical:value:recovery:required",
        FoundationSelfModelArea.CoreComponentIntegrity => "technical:value:integrity:trusted",
        FoundationSelfModelArea.AuthorityCondition => "technical:value:authority:trusted",
        FoundationSelfModelArea.FoundationIdentity => "technical:value:identity:trusted",
        FoundationSelfModelArea.ContradictionCondition => "technical:value:contradiction:none",
        FoundationSelfModelArea.BackupCondition => "technical:value:backup:ready",
        FoundationSelfModelArea.TechnicalFitness => "technical:value:fitness:unknown",
        _ => "technical:value:" + area.ToString().ToLowerInvariant() + ":current"
    };

    private static FoundationSelfModelAssertion[] Replace(
        IEnumerable<FoundationSelfModelAssertion> source,
        FoundationSelfModelArea area,
        Func<FoundationSelfModelAssertion, FoundationSelfModelAssertion> mutate) =>
        source.Select(assertion => assertion.Area == area ? mutate(assertion) : assertion).ToArray();

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}