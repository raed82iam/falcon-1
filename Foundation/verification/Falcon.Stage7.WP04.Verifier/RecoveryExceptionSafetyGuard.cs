using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Foundation.HealthFitness;
using Foundation.SelfAwareness;

namespace Falcon.Stage7.WP04.Verifier;

internal static class RecoveryExceptionSafetyGuard
{
    [ModuleInitializer]
    internal static void VerifyRecoveryExceptionFailClosedEdges()
    {
        var model = BaseModel();
        var declaration = BaseRecoveryDeclaration();

        VerifyEveryRecoveryFaultBindsToDeclaredOwner(model, declaration);
        VerifyInvalidDominatesDirectCircularInsufficiency(model);
        VerifyRecoveryProofContradictionReducesEvidenceQuality(model, declaration);
    }

    private static void VerifyEveryRecoveryFaultBindsToDeclaredOwner(
        FoundationSelfModelSnapshot model,
        RecoveryRestrictedModeDeclaration declaration)
    {
        var rule = Rule(
            new[]
            {
                Requirement(
                    "fitness:req:guard:recovery-primary",
                    FoundationSelfModelArea.RecoveryReadiness,
                    "technical:value:recovery:not-required",
                    TechnicalFitnessState.RecoveryRequired,
                    100),
                Requirement(
                    "fitness:req:guard:recovery-second-source",
                    FoundationSelfModelArea.BackupCondition,
                    "technical:value:backup:not-required",
                    TechnicalFitnessState.RecoveryRequired,
                    90)
            },
            declaration);

        var result = Evaluate("fitness:assessment:guard:mixed-recovery-owner", rule, model);

        if (result.FitnessResult != FitnessProjectionResult.NotFit ||
            !result.Reason.Contains("FAULT_SOURCE_BINDING_FAILED", StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                "WP-04 recovery exception accepted a RECOVERY_REQUIRED fault that was not bound to the declared fault source owner.");
        }
    }

    private static void VerifyInvalidDominatesDirectCircularInsufficiency(
        FoundationSelfModelSnapshot model)
    {
        const string assessmentId = "fitness:assessment:guard:circular-invalid";
        var assertions = model.Assertions.Select(assertion =>
            assertion.Area == FoundationSelfModelArea.RuntimeCondition
                ? assertion with
                {
                    EvidenceQuality = EvidenceQuality.Invalid,
                    Confidence = "INVALID",
                    Uncertainty = "invalid-and-direct-circular",
                    SourceAssessmentReference = assessmentId
                }
                : assertion).ToArray();

        var rebuilt = FoundationSelfModelProjector.Build(
            "selfmodel:wp04:guard:circular-invalid",
            model.FoundationId,
            model.AdmittedBaselineId,
            model.ModelTime,
            assertions,
            null);

        var rule = Rule(new[]
        {
            Requirement(
                "fitness:req:guard:runtime-invalid",
                FoundationSelfModelArea.RuntimeCondition,
                "technical:value:runtime:usable",
                TechnicalFitnessState.NotFit,
                50)
        }, null);

        var result = Evaluate(assessmentId, rule, rebuilt);

        if (result.FitnessResult != FitnessProjectionResult.NotFit ||
            result.EvidenceQuality != EvidenceQuality.Invalid ||
            !result.Unknowns.Contains("DIRECT_CIRCULAR_SELF_REFERENCE", StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                "WP-04 direct-circular fail-closed handling masked higher-severity INVALID evidence.");
        }
    }

    private static void VerifyRecoveryProofContradictionReducesEvidenceQuality(
        FoundationSelfModelSnapshot model,
        RecoveryRestrictedModeDeclaration declaration)
    {
        var security = model.Assertions.Single(assertion =>
            assertion.Area == FoundationSelfModelArea.SecurityCondition);

        var assertions = model.Assertions.Append(security with
        {
            AssertionId = "selfmodel:assertion:securitycondition:guard-conflict",
            ValueIdentity = "technical:value:trust:conflicted"
        }).ToArray();

        var rebuilt = FoundationSelfModelProjector.Build(
            "selfmodel:wp04:guard:recovery-contradiction",
            model.FoundationId,
            model.AdmittedBaselineId,
            model.ModelTime,
            assertions,
            null);

        var rule = Rule(new[]
        {
            Requirement(
                "fitness:req:guard:recovery-contradiction",
                FoundationSelfModelArea.RecoveryReadiness,
                "technical:value:recovery:not-required",
                TechnicalFitnessState.RecoveryRequired,
                100)
        }, declaration);

        var result = Evaluate("fitness:assessment:guard:recovery-contradiction", rule, rebuilt);

        if (result.FitnessResult != FitnessProjectionResult.NotFit ||
            result.EvidenceQuality != EvidenceQuality.Insufficient ||
            string.Equals(result.Contradictions, "NONE", StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                "WP-04 recovery-proof contradiction did not remain explicit as insufficient evidence and NOT_FIT.");
        }
    }

    private static FoundationSelfModelSnapshot BaseModel()
    {
        var method = typeof(Program).GetMethod(
            "Model",
            BindingFlags.NonPublic | BindingFlags.Static)
            ?? throw new InvalidOperationException("WP-04 verifier model factory missing.");

        return method.Invoke(null, new object?[] { null }) as FoundationSelfModelSnapshot
            ?? throw new InvalidOperationException("WP-04 verifier model factory returned no model.");
    }

    private static RecoveryRestrictedModeDeclaration BaseRecoveryDeclaration()
    {
        var method = typeof(Program).GetMethod(
            "RecoveryDeclaration",
            BindingFlags.NonPublic | BindingFlags.Static)
            ?? throw new InvalidOperationException("WP-04 verifier recovery declaration factory missing.");

        return method.Invoke(null, null) as RecoveryRestrictedModeDeclaration
            ?? throw new InvalidOperationException("WP-04 verifier recovery declaration factory returned no declaration.");
    }

    private static CanonicalHealthFitnessAssessment Evaluate(
        string assessmentId,
        TechnicalFitnessRuleDefinition rule,
        FoundationSelfModelSnapshot model) =>
        TechnicalFitnessEvaluationRuntime.Evaluate(
            assessmentId,
            rule,
            model,
            model.ModelTime,
            model.ModelTime.AddSeconds(30));

    private static TechnicalFitnessRuleDefinition Rule(
        IReadOnlyList<TechnicalFitnessRequirement> requirements,
        RecoveryRestrictedModeDeclaration? recovery)
    {
        var all = requirements.Append(new TechnicalFitnessRequirement(
            "fitness:req:guard:health-current",
            FoundationSelfModelArea.HealthCondition,
            "foundation",
            "foundation",
            new[]
            {
                "health-state:healthy",
                "health-state:degraded",
                "health-state:unhealthy",
                "health-state:not-applicable",
                "health-state:notapplicable"
            },
            TechnicalFitnessState.Unknown,
            TechnicalFitnessState.Unknown,
            1000,
            "NONE",
            null)).ToArray();

        return new TechnicalFitnessRuleDefinition(
            "fitness:rule:wp04:recovery-safety-guard",
            "1.0",
            "foundation",
            "foundation:technical-operation",
            "authority:level:technical",
            "foundation",
            all,
            recovery);
    }

    private static TechnicalFitnessRequirement Requirement(
        string id,
        FoundationSelfModelArea area,
        string acceptable,
        TechnicalFitnessState failure,
        int priority) =>
        new(
            id,
            area,
            "foundation",
            "foundation",
            new[] { acceptable },
            failure,
            TechnicalFitnessState.Unknown,
            priority,
            "NONE",
            null);
}