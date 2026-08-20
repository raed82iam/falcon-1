using System;
using System.Collections.Generic;
using Foundation.HealthFitness;

var checks = new List<(string Name, Action Test)>
{
    ("fit-can-support-positive-condition", () =>
    {
        var result = HealthFitnessGovernedConsumptionRuntime.Evaluate(Samples.Assessment(HealthState.Healthy, TechnicalFitnessState.Fit, FitnessProjectionResult.Fit, EvidenceQuality.Sufficient, "none"), Samples.Context(GovernedFitnessConsumerRole.AuthorityEngine));
        Require(result.State == GovernedFitnessConsumptionState.PositiveConditionInput, "expected positive condition input");
        Require(result.CanSupportPositiveAuthorityCondition, "fit should support condition input");
        Require(!result.PositiveAuthorityInferenceBlocked, "fit should not be blocked");
        Require(!result.RestrictionInputRequired, "fit should not require restriction input");
    }),
    ("fit-does-not-equal-authority-restoration", () =>
    {
        var context = Samples.Context(GovernedFitnessConsumerRole.AuthorityEngine) with { PriorAuthorityRestrictionOrDenial = true };
        var result = HealthFitnessGovernedConsumptionRuntime.Evaluate(Samples.Assessment(HealthState.Healthy, TechnicalFitnessState.Fit, FitnessProjectionResult.Fit, EvidenceQuality.Sufficient, "none"), context);
        Require(result.CanSupportPositiveAuthorityCondition, "fit should be usable as input");
        Require(result.NewAuthorityDecisionRequired, "restoration must require new authority decision");
    }),
    ("restricted-blocks-positive-inference", () =>
    {
        var result = HealthFitnessGovernedConsumptionRuntime.Evaluate(Samples.Assessment(HealthState.Degraded, TechnicalFitnessState.Degraded, FitnessProjectionResult.Restricted, EvidenceQuality.Limited, "none"), Samples.Context(GovernedFitnessConsumerRole.AuthorityEngine));
        Require(result.State == GovernedFitnessConsumptionState.RestrictionInput, "expected restriction input");
        Require(result.RestrictionInputRequired && result.PositiveAuthorityInferenceBlocked, "restriction must block positive inference");
    }),
    ("not-fit-blocks-positive-inference", () =>
    {
        var result = HealthFitnessGovernedConsumptionRuntime.Evaluate(Samples.Assessment(HealthState.Unhealthy, TechnicalFitnessState.NotFit, FitnessProjectionResult.NotFit, EvidenceQuality.Sufficient, "none"), Samples.Context(GovernedFitnessConsumerRole.AuthorityEngine));
        Require(result.State == GovernedFitnessConsumptionState.PositiveInferenceBlocked, "expected positive inference block");
        Require(result.PositiveAuthorityInferenceBlocked, "not fit must block positive inference");
    }),
    ("missing-assessment-fails-closed", () =>
    {
        var result = HealthFitnessGovernedConsumptionRuntime.Evaluate(null, Samples.Context(GovernedFitnessConsumerRole.AuthorityEngine));
        Require(result.PositiveAuthorityInferenceBlocked && result.RestrictionInputRequired, "missing assessment must fail closed");
        Require(result.Reason == HealthFitnessGovernedConsumptionRuntime.ReasonAwarenessMissing, "missing reason mismatch");
    }),
    ("missing-awareness-fails-closed", () =>
    {
        var context = Samples.Context(GovernedFitnessConsumerRole.AuthorityEngine) with { RequiredAwarenessAvailable = false };
        var result = HealthFitnessGovernedConsumptionRuntime.Evaluate(Samples.Assessment(HealthState.Healthy, TechnicalFitnessState.Fit, FitnessProjectionResult.Fit, EvidenceQuality.Sufficient, "none"), context);
        Require(result.PositiveAuthorityInferenceBlocked && result.IndependentReassessmentRequired, "missing awareness must fail closed");
    }),
    ("expired-assessment-fails-closed", () =>
    {
        var assessment = Samples.Assessment(HealthState.Healthy, TechnicalFitnessState.Fit, FitnessProjectionResult.Fit, EvidenceQuality.Sufficient, "none");
        var context = Samples.Context(GovernedFitnessConsumerRole.AuthorityEngine) with { EvaluationTime = assessment.Expiry };
        var result = HealthFitnessGovernedConsumptionRuntime.Evaluate(assessment, context);
        Require(result.PositiveAuthorityInferenceBlocked && !result.AssessmentCurrent, "expired assessment must fail closed");
    }),
    ("future-effective-assessment-fails-closed", () =>
    {
        var assessment = Samples.Assessment(HealthState.Healthy, TechnicalFitnessState.Fit, FitnessProjectionResult.Fit, EvidenceQuality.Sufficient, "none");
        var context = Samples.Context(GovernedFitnessConsumerRole.AuthorityEngine) with { EvaluationTime = assessment.EffectiveTime.AddSeconds(-1) };
        var result = HealthFitnessGovernedConsumptionRuntime.Evaluate(assessment, context);
        Require(result.PositiveAuthorityInferenceBlocked && !result.AssessmentCurrent, "not-yet-effective assessment must fail closed");
    }),
    ("insufficient-evidence-fails-closed", () =>
    {
        var result = HealthFitnessGovernedConsumptionRuntime.Evaluate(Samples.Assessment(HealthState.Healthy, TechnicalFitnessState.Fit, FitnessProjectionResult.Fit, EvidenceQuality.Insufficient, "none"), Samples.Context(GovernedFitnessConsumerRole.AuthorityEngine));
        Require(result.PositiveAuthorityInferenceBlocked, "insufficient evidence must block");
        Require(result.Reason == HealthFitnessGovernedConsumptionRuntime.ReasonEvidenceInsufficient, "insufficient evidence reason mismatch");
    }),
    ("invalid-evidence-fails-closed", () =>
    {
        var result = HealthFitnessGovernedConsumptionRuntime.Evaluate(Samples.Assessment(HealthState.Healthy, TechnicalFitnessState.Fit, FitnessProjectionResult.Fit, EvidenceQuality.Invalid, "none"), Samples.Context(GovernedFitnessConsumerRole.AuthorityEngine));
        Require(result.PositiveAuthorityInferenceBlocked, "invalid evidence must block");
    }),
    ("contradiction-fails-closed", () =>
    {
        var result = HealthFitnessGovernedConsumptionRuntime.Evaluate(Samples.Assessment(HealthState.Healthy, TechnicalFitnessState.Fit, FitnessProjectionResult.Fit, EvidenceQuality.Sufficient, "material-conflict"), Samples.Context(GovernedFitnessConsumerRole.AuthorityEngine));
        Require(result.PositiveAuthorityInferenceBlocked && result.IndependentReassessmentRequired, "contradiction must block and require reassessment");
        Require(result.Reason == HealthFitnessGovernedConsumptionRuntime.ReasonContradictory, "contradiction reason mismatch");
    }),
    ("recovery-required-gates", () =>
    {
        var result = HealthFitnessGovernedConsumptionRuntime.Evaluate(Samples.Assessment(HealthState.Degraded, TechnicalFitnessState.RecoveryRequired, FitnessProjectionResult.Restricted, EvidenceQuality.Sufficient, "none"), Samples.Context(GovernedFitnessConsumerRole.AuthorityEngine));
        Require(result.State == GovernedFitnessConsumptionState.RecoveryGate, "expected recovery gate");
        Require(result.RecoveryGateRequired && result.PositiveAuthorityInferenceBlocked, "recovery required must gate");
        Require(result.NewAuthorityDecisionRequired, "recovery gate must require new authority decision");
    }),
    ("source-recovery-alone-does-not-restore", () =>
    {
        var context = Samples.Context(GovernedFitnessConsumerRole.AuthorityEngine) with
        {
            PriorMaterialAwarenessOrFitnessLoss = true,
            IndependentReassessmentConfirmed = false,
            PriorAuthorityRestrictionOrDenial = true
        };
        var result = HealthFitnessGovernedConsumptionRuntime.Evaluate(Samples.Assessment(HealthState.Healthy, TechnicalFitnessState.Fit, FitnessProjectionResult.Fit, EvidenceQuality.Sufficient, "none"), context);
        Require(result.PositiveAuthorityInferenceBlocked, "source recovery alone must not restore positive authority input");
        Require(result.IndependentReassessmentRequired, "independent reassessment must be required");
        Require(result.NewAuthorityDecisionRequired, "new authority decision must remain required");
    }),
    ("independent-reassessment-restores-input-not-authority", () =>
    {
        var context = Samples.Context(GovernedFitnessConsumerRole.AuthorityEngine) with
        {
            PriorMaterialAwarenessOrFitnessLoss = true,
            IndependentReassessmentConfirmed = true,
            PriorAuthorityRestrictionOrDenial = true
        };
        var result = HealthFitnessGovernedConsumptionRuntime.Evaluate(Samples.Assessment(HealthState.Healthy, TechnicalFitnessState.Fit, FitnessProjectionResult.Fit, EvidenceQuality.Sufficient, "none"), context);
        Require(result.CanSupportPositiveAuthorityCondition, "reassessment may restore input eligibility");
        Require(result.NewAuthorityDecisionRequired, "reassessment must not restore authority itself");
    }),
    ("lifecycle-consumer-supported-without-command", () =>
    {
        var result = HealthFitnessGovernedConsumptionRuntime.Evaluate(Samples.Assessment(HealthState.Degraded, TechnicalFitnessState.Degraded, FitnessProjectionResult.Restricted, EvidenceQuality.Limited, "none"), Samples.Context(GovernedFitnessConsumerRole.Lifecycle));
        Require(result.ConsumerRole == GovernedFitnessConsumerRole.Lifecycle, "lifecycle consumer identity missing");
        Require(result.RestrictionInputRequired, "lifecycle restriction input missing");
    }),
    ("protective-consumer-supported-without-command", () =>
    {
        var result = HealthFitnessGovernedConsumptionRuntime.Evaluate(Samples.Assessment(HealthState.Unhealthy, TechnicalFitnessState.NotFit, FitnessProjectionResult.NotFit, EvidenceQuality.Sufficient, "none"), Samples.Context(GovernedFitnessConsumerRole.ProtectiveConsumer));
        Require(result.ConsumerRole == GovernedFitnessConsumerRole.ProtectiveConsumer, "protective consumer identity missing");
        Require(result.PositiveAuthorityInferenceBlocked, "protective evidence should preserve reduction");
    }),
    ("consumer-role-changes-identity", () =>
    {
        var assessment = Samples.Assessment(HealthState.Healthy, TechnicalFitnessState.Fit, FitnessProjectionResult.Fit, EvidenceQuality.Sufficient, "none");
        var authority = HealthFitnessGovernedConsumptionRuntime.Evaluate(assessment, Samples.Context(GovernedFitnessConsumerRole.AuthorityEngine));
        var lifecycle = HealthFitnessGovernedConsumptionRuntime.Evaluate(assessment, Samples.Context(GovernedFitnessConsumerRole.Lifecycle));
        Require(authority.Identity != lifecycle.Identity, "consumer role must affect identity");
    }),
    ("deterministic-identical-input", () =>
    {
        var assessment = Samples.Assessment(HealthState.Healthy, TechnicalFitnessState.Fit, FitnessProjectionResult.Fit, EvidenceQuality.Sufficient, "none");
        var context = Samples.Context(GovernedFitnessConsumerRole.AuthorityEngine);
        var a = HealthFitnessGovernedConsumptionRuntime.Evaluate(assessment, context);
        var b = HealthFitnessGovernedConsumptionRuntime.Evaluate(assessment, context);
        Require(a == b && a.Identity == b.Identity, "identical inputs must be deterministic");
    }),
    ("context-mutation-sensitive", () =>
    {
        var assessment = Samples.Assessment(HealthState.Healthy, TechnicalFitnessState.Fit, FitnessProjectionResult.Fit, EvidenceQuality.Sufficient, "none");
        var a = HealthFitnessGovernedConsumptionRuntime.Evaluate(assessment, Samples.Context(GovernedFitnessConsumerRole.AuthorityEngine));
        var b = HealthFitnessGovernedConsumptionRuntime.Evaluate(assessment, Samples.Context(GovernedFitnessConsumerRole.AuthorityEngine) with { PriorAuthorityRestrictionOrDenial = true });
        Require(a.Identity != b.Identity, "context mutation must change evidence identity");
    }),
    ("assessment-mutation-sensitive", () =>
    {
        var context = Samples.Context(GovernedFitnessConsumerRole.AuthorityEngine);
        var a = HealthFitnessGovernedConsumptionRuntime.Evaluate(Samples.Assessment(HealthState.Healthy, TechnicalFitnessState.Fit, FitnessProjectionResult.Fit, EvidenceQuality.Sufficient, "none"), context);
        var b = HealthFitnessGovernedConsumptionRuntime.Evaluate(Samples.Assessment(HealthState.Degraded, TechnicalFitnessState.Degraded, FitnessProjectionResult.Restricted, EvidenceQuality.Limited, "none"), context);
        Require(a.Identity != b.Identity, "assessment mutation must change evidence identity");
    }),
    ("malformed-assessment-fails-closed", () =>
    {
        var malformed = Samples.Assessment(HealthState.Healthy, TechnicalFitnessState.Fit, FitnessProjectionResult.Fit, EvidenceQuality.Sufficient, "none") with { EvidenceReference = "" };
        var result = HealthFitnessGovernedConsumptionRuntime.Evaluate(malformed, Samples.Context(GovernedFitnessConsumerRole.AuthorityEngine));
        Require(result.PositiveAuthorityInferenceBlocked, "malformed assessment must fail closed");
    }),
    ("invalid-consumer-enum-rejected", () => ExpectFailure(() =>
    {
        var context = Samples.Context((GovernedFitnessConsumerRole)999);
        HealthFitnessGovernedConsumptionRuntime.Evaluate(Samples.Assessment(HealthState.Healthy, TechnicalFitnessState.Fit, FitnessProjectionResult.Fit, EvidenceQuality.Sufficient, "none"), context);
    })),
    ("zero-evaluation-time-rejected", () => ExpectFailure(() =>
    {
        var context = Samples.Context(GovernedFitnessConsumerRole.AuthorityEngine) with { EvaluationTime = default };
        HealthFitnessGovernedConsumptionRuntime.Evaluate(Samples.Assessment(HealthState.Healthy, TechnicalFitnessState.Fit, FitnessProjectionResult.Fit, EvidenceQuality.Sufficient, "none"), context);
    })),
    ("fit-with-limited-evidence-is-not-positive", () =>
    {
        var result = HealthFitnessGovernedConsumptionRuntime.Evaluate(Samples.Assessment(HealthState.Healthy, TechnicalFitnessState.Fit, FitnessProjectionResult.Fit, EvidenceQuality.Limited, "none"), Samples.Context(GovernedFitnessConsumerRole.AuthorityEngine));
        Require(!result.CanSupportPositiveAuthorityCondition && result.PositiveAuthorityInferenceBlocked, "limited evidence must not produce positive condition input");
    }),
    ("fit-with-degraded-health-is-not-positive", () =>
    {
        var result = HealthFitnessGovernedConsumptionRuntime.Evaluate(Samples.Assessment(HealthState.Degraded, TechnicalFitnessState.Fit, FitnessProjectionResult.Fit, EvidenceQuality.Sufficient, "none"), Samples.Context(GovernedFitnessConsumerRole.AuthorityEngine));
        Require(!result.CanSupportPositiveAuthorityCondition && result.PositiveAuthorityInferenceBlocked, "degraded health must not produce positive condition input");
    })
};

var passed = 0;
foreach (var check in checks)
{
    try
    {
        check.Test();
        passed++;
        Console.WriteLine($"PASS | {check.Name}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"FAIL | {check.Name} | {ex.GetType().Name}: {ex.Message}");
    }
}

if (passed == checks.Count)
{
    Console.WriteLine("STAGE7_WP08_VERIFIER = PASS");
    Console.WriteLine($"CHECKS = {passed}/{checks.Count}");
    return 0;
}

Console.WriteLine("STAGE7_WP08_VERIFIER = FAIL");
Console.WriteLine($"CHECKS = {passed}/{checks.Count}");
return 1;

static void Require(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
}

static void ExpectFailure(Action action)
{
    try
    {
        action();
    }
    catch
    {
        return;
    }

    throw new InvalidOperationException("expected rejection did not occur");
}

static class Samples
{
    private static readonly DateTimeOffset T0 = new(2026, 8, 14, 12, 0, 0, TimeSpan.Zero);

    public static GovernedFitnessConsumptionContext Context(GovernedFitnessConsumerRole role)
        => new(
            role,
            T0.AddMinutes(3),
            RequiredAwarenessAvailable: true,
            IndependentReassessmentConfirmed: false,
            PriorMaterialAwarenessOrFitnessLoss: false,
            PriorAuthorityRestrictionOrDenial: false);

    public static CanonicalHealthFitnessAssessment Assessment(
        HealthState health,
        TechnicalFitnessState fitness,
        FitnessProjectionResult result,
        EvidenceQuality quality,
        string contradictions)
        => new(
            "assessment-wp08",
            "subject-foundation-core",
            "capability-wp08",
            "authority-level-observed",
            health,
            fitness,
            result,
            "scope-foundation",
            "evidence-stage7-wp08",
            "self-model-stage7",
            quality,
            "0.95",
            "none",
            contradictions,
            result == FitnessProjectionResult.Fit ? "none" : "bounded-restriction",
            "stage7-wp08-verifier",
            "rule-wp08-consumption",
            "1.0",
            T0,
            T0.AddMinutes(1),
            T0.AddMinutes(2),
            T0.AddMinutes(10));
}
