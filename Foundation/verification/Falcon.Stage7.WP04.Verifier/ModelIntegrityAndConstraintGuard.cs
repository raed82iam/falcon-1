using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Foundation.HealthFitness;
using Foundation.SelfAwareness;

namespace Falcon.Stage7.WP04.Verifier;

internal static class ModelIntegrityAndConstraintGuard
{
    [ModuleInitializer]
    internal static void VerifyModelIntegrityAndRestrictedConstraints()
    {
        var model = BaseModel();
        VerifyHiddenContradictionCannotBypassCanonicalModel(model);
        VerifyAllRestrictedConstraintsArePreserved(model, BaseRecoveryDeclaration());
    }

    private static void VerifyHiddenContradictionCannotBypassCanonicalModel(
        FoundationSelfModelSnapshot model)
    {
        var security = model.Assertions.Single(assertion =>
            assertion.Area == FoundationSelfModelArea.SecurityCondition);

        var conflictingAssertions = model.Assertions.Append(security with
        {
            AssertionId = "selfmodel:assertion:securitycondition:model-integrity-conflict",
            ValueIdentity = "technical:value:trust:conflicted"
        }).ToArray();

        var canonicalConflicting = FoundationSelfModelProjector.Build(
            "selfmodel:wp04:guard:model-integrity",
            model.FoundationId,
            model.AdmittedBaselineId,
            model.ModelTime,
            conflictingAssertions,
            null);

        if (canonicalConflicting.Contradictions.Count == 0)
            throw new InvalidOperationException("WP-04 model-integrity guard failed to construct a canonical contradiction.");

        var forged = canonicalConflicting with
        {
            Contradictions = Array.Empty<FoundationSelfModelContradiction>()
        };

        var rule = Rule(new[]
        {
            Requirement(
                "fitness:req:guard:model-integrity-runtime",
                FoundationSelfModelArea.RuntimeCondition,
                "technical:value:runtime:usable",
                TechnicalFitnessState.NotFit,
                "NONE",
                50)
        }, null);

        try
        {
            _ = Evaluate("fitness:assessment:guard:model-integrity", rule, forged);
        }
        catch (ArgumentException exception) when (
            exception.Message.Contains("non-canonical Self Model", StringComparison.Ordinal))
        {
            return;
        }

        throw new InvalidOperationException(
            "WP-04 accepted a Self Model whose canonical contradiction set was manually removed.");
    }

    private static void VerifyAllRestrictedConstraintsArePreserved(
        FoundationSelfModelSnapshot model,
        RecoveryRestrictedModeDeclaration declaration)
    {
        const string secondaryConstraint = "constraint:backup-degraded";

        var rule = Rule(new[]
        {
            Requirement(
                "fitness:req:guard:constraint-recovery",
                FoundationSelfModelArea.RecoveryReadiness,
                "technical:value:recovery:not-required",
                TechnicalFitnessState.RecoveryRequired,
                "NONE",
                100),
            Requirement(
                "fitness:req:guard:constraint-backup",
                FoundationSelfModelArea.BackupCondition,
                "technical:value:backup:fully-ready",
                TechnicalFitnessState.Degraded,
                secondaryConstraint,
                90)
        }, declaration);

        var result = Evaluate("fitness:assessment:guard:constraint-union", rule, model);

        if (result.FitnessResult != FitnessProjectionResult.Restricted ||
            !result.Constraints.Contains(declaration.Constraints, StringComparison.Ordinal) ||
            !result.Constraints.Contains(secondaryConstraint, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                "WP-04 RESTRICTED result did not preserve both Recovery and independent restriction constraints.");
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
            "fitness:req:guard:model-constraint-health-current",
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
            "fitness:rule:wp04:model-constraint-guard",
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
        string constraint,
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
            constraint,
            null);
}