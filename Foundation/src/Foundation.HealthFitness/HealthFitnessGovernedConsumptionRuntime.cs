using System;
using System.Security.Cryptography;
using System.Text;

namespace Foundation.HealthFitness;

public enum GovernedFitnessConsumerRole
{
    AuthorityEngine = 1,
    Lifecycle = 2,
    ProtectiveConsumer = 3
}

public enum GovernedFitnessConsumptionState
{
    PositiveConditionInput = 1,
    RestrictionInput = 2,
    PositiveInferenceBlocked = 3,
    RecoveryGate = 4
}

public sealed record GovernedFitnessConsumptionContext(
    GovernedFitnessConsumerRole ConsumerRole,
    DateTimeOffset EvaluationTime,
    bool RequiredAwarenessAvailable,
    bool IndependentReassessmentConfirmed,
    bool PriorMaterialAwarenessOrFitnessLoss,
    bool PriorAuthorityRestrictionOrDenial)
{
    public string Identity => GovernedFitnessConsumptionIdentity.ComputeContext(this);
}

public sealed record GovernedFitnessConsumptionEvidence(
    string AssessmentIdentity,
    string SubjectId,
    string Capability,
    string Scope,
    GovernedFitnessConsumerRole ConsumerRole,
    GovernedFitnessConsumptionState State,
    bool AssessmentCurrent,
    bool CanSupportPositiveAuthorityCondition,
    bool RestrictionInputRequired,
    bool PositiveAuthorityInferenceBlocked,
    bool RecoveryGateRequired,
    bool IndependentReassessmentRequired,
    bool NewAuthorityDecisionRequired,
    string Reason,
    DateTimeOffset EvaluatedAt,
    string ContextIdentity)
{
    public string Identity => GovernedFitnessConsumptionIdentity.ComputeEvidence(this);
}

public static class HealthFitnessGovernedConsumptionRuntime
{
    public const string ReasonPositiveConditionInput = "FITNESS_POSITIVE_CONDITION_INPUT_AVAILABLE";
    public const string ReasonRestricted = "FITNESS_RESTRICTION_INPUT_REQUIRED";
    public const string ReasonNotFit = "FITNESS_POSITIVE_AUTHORITY_INFERENCE_BLOCKED";
    public const string ReasonRecoveryGate = "FITNESS_RECOVERY_GATE_REQUIRED";
    public const string ReasonAwarenessMissing = "REQUIRED_AWARENESS_UNAVAILABLE";
    public const string ReasonExpired = "FITNESS_ASSESSMENT_EXPIRED";
    public const string ReasonEvidenceInsufficient = "FITNESS_EVIDENCE_INSUFFICIENT";
    public const string ReasonContradictory = "FITNESS_EVIDENCE_CONTRADICTORY";
    public const string ReasonReassessmentRequired = "INDEPENDENT_REASSESSMENT_REQUIRED_AFTER_MATERIAL_LOSS";

    public static GovernedFitnessConsumptionEvidence Evaluate(
        CanonicalHealthFitnessAssessment? assessment,
        GovernedFitnessConsumptionContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (!Enum.IsDefined(context.ConsumerRole))
            throw new ArgumentException("WP08 consumer role rejected", nameof(context));

        if (context.EvaluationTime == default)
            throw new ArgumentException("WP08 evaluation time required", nameof(context));

        if (assessment is null)
        {
            return BuildMissingAssessment(context);
        }

        var validation = HealthFitnessPrimitiveValidator.Validate(assessment);
        if (validation.Result != Foundation.Contracts.ValidationResult.Pass)
        {
            return Build(
                assessment,
                context,
                GovernedFitnessConsumptionState.PositiveInferenceBlocked,
                assessmentCurrent: false,
                canSupportPositiveAuthorityCondition: false,
                restrictionInputRequired: true,
                positiveAuthorityInferenceBlocked: true,
                recoveryGateRequired: false,
                independentReassessmentRequired: context.PriorMaterialAwarenessOrFitnessLoss,
                newAuthorityDecisionRequired: context.PriorAuthorityRestrictionOrDenial,
                reason: ReasonEvidenceInsufficient);
        }

        if (!context.RequiredAwarenessAvailable)
        {
            return Build(
                assessment,
                context,
                GovernedFitnessConsumptionState.PositiveInferenceBlocked,
                assessmentCurrent: false,
                canSupportPositiveAuthorityCondition: false,
                restrictionInputRequired: true,
                positiveAuthorityInferenceBlocked: true,
                recoveryGateRequired: false,
                independentReassessmentRequired: true,
                newAuthorityDecisionRequired: context.PriorAuthorityRestrictionOrDenial,
                reason: ReasonAwarenessMissing);
        }

        if (context.EvaluationTime < assessment.EffectiveTime || context.EvaluationTime >= assessment.Expiry)
        {
            return Build(
                assessment,
                context,
                GovernedFitnessConsumptionState.PositiveInferenceBlocked,
                assessmentCurrent: false,
                canSupportPositiveAuthorityCondition: false,
                restrictionInputRequired: true,
                positiveAuthorityInferenceBlocked: true,
                recoveryGateRequired: false,
                independentReassessmentRequired: context.PriorMaterialAwarenessOrFitnessLoss,
                newAuthorityDecisionRequired: context.PriorAuthorityRestrictionOrDenial,
                reason: ReasonExpired);
        }

        if (assessment.EvidenceQuality is EvidenceQuality.Insufficient or EvidenceQuality.Invalid)
        {
            return Build(
                assessment,
                context,
                GovernedFitnessConsumptionState.PositiveInferenceBlocked,
                assessmentCurrent: true,
                canSupportPositiveAuthorityCondition: false,
                restrictionInputRequired: true,
                positiveAuthorityInferenceBlocked: true,
                recoveryGateRequired: assessment.TechnicalFitnessState == TechnicalFitnessState.RecoveryRequired,
                independentReassessmentRequired: context.PriorMaterialAwarenessOrFitnessLoss,
                newAuthorityDecisionRequired: context.PriorAuthorityRestrictionOrDenial,
                reason: ReasonEvidenceInsufficient);
        }

        if (!string.Equals(assessment.Contradictions, "none", StringComparison.OrdinalIgnoreCase))
        {
            return Build(
                assessment,
                context,
                GovernedFitnessConsumptionState.PositiveInferenceBlocked,
                assessmentCurrent: true,
                canSupportPositiveAuthorityCondition: false,
                restrictionInputRequired: true,
                positiveAuthorityInferenceBlocked: true,
                recoveryGateRequired: assessment.TechnicalFitnessState == TechnicalFitnessState.RecoveryRequired,
                independentReassessmentRequired: true,
                newAuthorityDecisionRequired: context.PriorAuthorityRestrictionOrDenial,
                reason: ReasonContradictory);
        }

        if (assessment.TechnicalFitnessState == TechnicalFitnessState.RecoveryRequired)
        {
            return Build(
                assessment,
                context,
                GovernedFitnessConsumptionState.RecoveryGate,
                assessmentCurrent: true,
                canSupportPositiveAuthorityCondition: false,
                restrictionInputRequired: true,
                positiveAuthorityInferenceBlocked: true,
                recoveryGateRequired: true,
                independentReassessmentRequired: true,
                newAuthorityDecisionRequired: true,
                reason: ReasonRecoveryGate);
        }

        if (context.PriorMaterialAwarenessOrFitnessLoss && !context.IndependentReassessmentConfirmed)
        {
            return Build(
                assessment,
                context,
                GovernedFitnessConsumptionState.PositiveInferenceBlocked,
                assessmentCurrent: true,
                canSupportPositiveAuthorityCondition: false,
                restrictionInputRequired: true,
                positiveAuthorityInferenceBlocked: true,
                recoveryGateRequired: false,
                independentReassessmentRequired: true,
                newAuthorityDecisionRequired: context.PriorAuthorityRestrictionOrDenial,
                reason: ReasonReassessmentRequired);
        }

        if (assessment.FitnessResult == FitnessProjectionResult.Fit &&
            assessment.TechnicalFitnessState == TechnicalFitnessState.Fit &&
            assessment.HealthState == HealthState.Healthy &&
            assessment.EvidenceQuality == EvidenceQuality.Sufficient)
        {
            return Build(
                assessment,
                context,
                GovernedFitnessConsumptionState.PositiveConditionInput,
                assessmentCurrent: true,
                canSupportPositiveAuthorityCondition: true,
                restrictionInputRequired: false,
                positiveAuthorityInferenceBlocked: false,
                recoveryGateRequired: false,
                independentReassessmentRequired: false,
                newAuthorityDecisionRequired: context.PriorAuthorityRestrictionOrDenial,
                reason: ReasonPositiveConditionInput);
        }

        if (assessment.FitnessResult == FitnessProjectionResult.Restricted)
        {
            return Build(
                assessment,
                context,
                GovernedFitnessConsumptionState.RestrictionInput,
                assessmentCurrent: true,
                canSupportPositiveAuthorityCondition: false,
                restrictionInputRequired: true,
                positiveAuthorityInferenceBlocked: true,
                recoveryGateRequired: false,
                independentReassessmentRequired: false,
                newAuthorityDecisionRequired: context.PriorAuthorityRestrictionOrDenial,
                reason: ReasonRestricted);
        }

        return Build(
            assessment,
            context,
            GovernedFitnessConsumptionState.PositiveInferenceBlocked,
            assessmentCurrent: true,
            canSupportPositiveAuthorityCondition: false,
            restrictionInputRequired: true,
            positiveAuthorityInferenceBlocked: true,
            recoveryGateRequired: false,
            independentReassessmentRequired: false,
            newAuthorityDecisionRequired: context.PriorAuthorityRestrictionOrDenial,
            reason: ReasonNotFit);
    }

    private static GovernedFitnessConsumptionEvidence BuildMissingAssessment(
        GovernedFitnessConsumptionContext context)
        => new(
            AssessmentIdentity: "MISSING",
            SubjectId: "unknown-subject",
            Capability: "unknown-capability",
            Scope: "unknown-scope",
            ConsumerRole: context.ConsumerRole,
            State: GovernedFitnessConsumptionState.PositiveInferenceBlocked,
            AssessmentCurrent: false,
            CanSupportPositiveAuthorityCondition: false,
            RestrictionInputRequired: true,
            PositiveAuthorityInferenceBlocked: true,
            RecoveryGateRequired: false,
            IndependentReassessmentRequired: true,
            NewAuthorityDecisionRequired: context.PriorAuthorityRestrictionOrDenial,
            Reason: ReasonAwarenessMissing,
            EvaluatedAt: context.EvaluationTime,
            ContextIdentity: context.Identity);

    private static GovernedFitnessConsumptionEvidence Build(
        CanonicalHealthFitnessAssessment assessment,
        GovernedFitnessConsumptionContext context,
        GovernedFitnessConsumptionState state,
        bool assessmentCurrent,
        bool canSupportPositiveAuthorityCondition,
        bool restrictionInputRequired,
        bool positiveAuthorityInferenceBlocked,
        bool recoveryGateRequired,
        bool independentReassessmentRequired,
        bool newAuthorityDecisionRequired,
        string reason)
        => new(
            assessment.Identity,
            assessment.SubjectId,
            assessment.Capability,
            assessment.Scope,
            context.ConsumerRole,
            state,
            assessmentCurrent,
            canSupportPositiveAuthorityCondition,
            restrictionInputRequired,
            positiveAuthorityInferenceBlocked,
            recoveryGateRequired,
            independentReassessmentRequired,
            newAuthorityDecisionRequired,
            reason,
            context.EvaluationTime,
            context.Identity);
}

public static class GovernedFitnessConsumptionIdentity
{
    public static string ComputeContext(GovernedFitnessConsumptionContext value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return Hash(
            value.ConsumerRole.ToString(),
            value.EvaluationTime.ToUniversalTime().ToString("O"),
            value.RequiredAwarenessAvailable ? "1" : "0",
            value.IndependentReassessmentConfirmed ? "1" : "0",
            value.PriorMaterialAwarenessOrFitnessLoss ? "1" : "0",
            value.PriorAuthorityRestrictionOrDenial ? "1" : "0");
    }

    public static string ComputeEvidence(GovernedFitnessConsumptionEvidence value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return Hash(
            value.AssessmentIdentity,
            value.SubjectId,
            value.Capability,
            value.Scope,
            value.ConsumerRole.ToString(),
            value.State.ToString(),
            value.AssessmentCurrent ? "1" : "0",
            value.CanSupportPositiveAuthorityCondition ? "1" : "0",
            value.RestrictionInputRequired ? "1" : "0",
            value.PositiveAuthorityInferenceBlocked ? "1" : "0",
            value.RecoveryGateRequired ? "1" : "0",
            value.IndependentReassessmentRequired ? "1" : "0",
            value.NewAuthorityDecisionRequired ? "1" : "0",
            value.Reason,
            value.EvaluatedAt.ToUniversalTime().ToString("O"),
            value.ContextIdentity);
    }

    private static string Hash(params string[] values)
    {
        var canonical = string.Join("\u001F", values);
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(canonical)));
    }
}
