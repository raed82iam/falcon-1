using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Foundation.Contracts;
using Foundation.HealthFitness;

namespace Foundation.SelfAwareness;

public enum RecoveryRestrictedCondition
{
    FaultTechnicallyIsolated = 1,
    CapabilityIndependentOfAffectedPath = 2,
    IndependentUsabilityProven = 3,
    TrustBoundaryClear = 4
}

public sealed record TechnicalFitnessRequirement(
    string RequirementId,
    FoundationSelfModelArea Area,
    string SubjectId,
    string Scope,
    IReadOnlyList<string> AcceptableValueIdentities,
    TechnicalFitnessState FailureState,
    TechnicalFitnessState LimitedEvidenceState,
    int FailurePriority,
    string Constraint,
    string? RequiredSourceOwner = null);

public sealed record RecoveryRestrictedConditionProof(
    RecoveryRestrictedCondition Condition,
    string AssertionId,
    FoundationSelfModelArea Area,
    string SubjectId,
    string Scope,
    string ExpectedValueIdentity,
    string? RequiredSourceOwner = null);

public sealed record RecoveryRestrictedModeDeclaration(
    string FaultSourceOwner,
    string Constraints,
    IReadOnlyList<RecoveryRestrictedConditionProof> Proofs);

public sealed record TechnicalFitnessRuleDefinition(
    string RuleId,
    string RuleVersion,
    string SubjectId,
    string Capability,
    string RequestedAuthorityLevel,
    string Scope,
    IReadOnlyList<TechnicalFitnessRequirement> Requirements,
    RecoveryRestrictedModeDeclaration? RecoveryRestrictedMode);

public static class TechnicalFitnessRuleValidator
{
    public static ValidationOutcome Validate(TechnicalFitnessRuleDefinition? rule)
    {
        if (rule is null)
            return ValidationOutcome.Failed("Stage 7 Technical Fitness rule missing");

        if (!Id(rule.RuleId) || !Id(rule.RuleVersion) || !Id(rule.SubjectId) ||
            !Id(rule.Capability) || !Id(rule.RequestedAuthorityLevel) || !Id(rule.Scope))
            return ValidationOutcome.Failed("Stage 7 Technical Fitness rule canonical identity rejected");

        if (rule.Requirements is null || rule.Requirements.Count == 0)
            return ValidationOutcome.Failed("Stage 7 Technical Fitness rule requirements missing");

        var ids = new HashSet<string>(StringComparer.Ordinal);
        foreach (var requirement in rule.Requirements)
        {
            if (requirement is null || !Id(requirement.RequirementId) || !Id(requirement.SubjectId) ||
                !Id(requirement.Scope) || !Enum.IsDefined(requirement.Area) ||
                !Enum.IsDefined(requirement.FailureState) || !Enum.IsDefined(requirement.LimitedEvidenceState) ||
                requirement.FailurePriority <= 0 || requirement.AcceptableValueIdentities is null ||
                requirement.AcceptableValueIdentities.Count == 0 || requirement.AcceptableValueIdentities.Any(value => !Id(value)))
                return ValidationOutcome.Failed("Stage 7 Technical Fitness requirement rejected");

            if (!ids.Add(requirement.RequirementId))
                return ValidationOutcome.Failed("Stage 7 Technical Fitness requirement duplicated");
            if (requirement.Area == FoundationSelfModelArea.TechnicalFitness)
                return ValidationOutcome.Failed("Stage 7 Technical Fitness circular Self Model requirement rejected");
            if (requirement.FailureState == TechnicalFitnessState.Fit)
                return ValidationOutcome.Failed("Stage 7 Technical Fitness failure state cannot be FIT");
            if (MapBase(requirement.LimitedEvidenceState) == FitnessProjectionResult.Fit)
                return ValidationOutcome.Failed("Stage 7 Technical Fitness limited evidence cannot yield FIT");
            if ((MapBase(requirement.FailureState) == FitnessProjectionResult.Restricted ||
                 MapBase(requirement.LimitedEvidenceState) == FitnessProjectionResult.Restricted) &&
                !MeaningfulConstraint(requirement.Constraint))
                return ValidationOutcome.Failed("Stage 7 Technical Fitness RESTRICTED constraint missing");
            if (requirement.RequiredSourceOwner is not null && !Id(requirement.RequiredSourceOwner))
                return ValidationOutcome.Failed("Stage 7 Technical Fitness required source owner rejected");
        }

        if (!rule.Requirements.Any(requirement =>
                requirement.Area == FoundationSelfModelArea.HealthCondition &&
                string.Equals(requirement.SubjectId, rule.SubjectId, StringComparison.Ordinal) &&
                string.Equals(requirement.Scope, rule.Scope, StringComparison.Ordinal)))
            return ValidationOutcome.Failed("Stage 7 Technical Fitness scoped Health requirement missing");

        if (rule.RecoveryRestrictedMode is { } recovery)
        {
            if (!Id(recovery.FaultSourceOwner) || !MeaningfulConstraint(recovery.Constraints) || recovery.Proofs is null)
                return ValidationOutcome.Failed("Stage 7 Technical Fitness recovery exception declaration rejected");

            var conditions = Enum.GetValues<RecoveryRestrictedCondition>();
            if (recovery.Proofs.Count < conditions.Length ||
                recovery.Proofs.Any(proof => proof is null || !Enum.IsDefined(proof.Condition) ||
                    !Enum.IsDefined(proof.Area) || proof.Area == FoundationSelfModelArea.TechnicalFitness ||
                    !Id(proof.AssertionId) || !Id(proof.SubjectId) || !Id(proof.Scope) || !Id(proof.ExpectedValueIdentity) ||
                    !string.Equals(proof.SubjectId, rule.SubjectId, StringComparison.Ordinal) ||
                    !string.Equals(proof.Scope, rule.Scope, StringComparison.Ordinal) ||
                    (proof.RequiredSourceOwner is not null && !Id(proof.RequiredSourceOwner))) ||
                conditions.Any(condition => !recovery.Proofs.Any(proof => proof.Condition == condition)) ||
                recovery.Proofs.Select(proof => proof.AssertionId).Distinct(StringComparer.Ordinal).Count() != recovery.Proofs.Count)
                return ValidationOutcome.Failed("Stage 7 Technical Fitness recovery exception proof set rejected");
        }

        return ValidationOutcome.Passed("Stage 7 Technical Fitness rule valid");
    }

    private static bool Id(string value) => HealthFitnessContractV12.IsCanonicalIdentifier(value);
    private static bool MeaningfulConstraint(string? value) =>
        !string.IsNullOrWhiteSpace(value) && !string.Equals(value.Trim(), "NONE", StringComparison.OrdinalIgnoreCase);

    internal static FitnessProjectionResult MapBase(TechnicalFitnessState state) => state switch
    {
        TechnicalFitnessState.Fit => FitnessProjectionResult.Fit,
        TechnicalFitnessState.FitWithConstraints => FitnessProjectionResult.Restricted,
        TechnicalFitnessState.Degraded => FitnessProjectionResult.Restricted,
        TechnicalFitnessState.Unknown => FitnessProjectionResult.NotFit,
        TechnicalFitnessState.Unavailable => FitnessProjectionResult.NotFit,
        TechnicalFitnessState.IntegrityFailure => FitnessProjectionResult.NotFit,
        TechnicalFitnessState.IsolationRequired => FitnessProjectionResult.Restricted,
        TechnicalFitnessState.RecoveryRequired => FitnessProjectionResult.NotFit,
        TechnicalFitnessState.NotFit => FitnessProjectionResult.NotFit,
        _ => throw new ArgumentOutOfRangeException(nameof(state))
    };
}

public static class TechnicalFitnessEvaluationRuntime
{
    private sealed record RequirementOutcome(
        TechnicalFitnessRequirement Requirement,
        TechnicalFitnessState State,
        IReadOnlyList<FoundationSelfModelAssertion> Assertions,
        string UnknownReason,
        IReadOnlyList<string> ContradictionIds);

    public static CanonicalHealthFitnessAssessment Evaluate(
        string assessmentId,
        TechnicalFitnessRuleDefinition rule,
        FoundationSelfModelSnapshot model,
        DateTimeOffset assessmentTime,
        DateTimeOffset requestedExpiry)
    {
        if (!HealthFitnessContractV12.IsCanonicalIdentifier(assessmentId))
            throw new ArgumentException("Stage 7 Technical Fitness assessment identity rejected", nameof(assessmentId));

        var ruleValidation = TechnicalFitnessRuleValidator.Validate(rule);
        if (ruleValidation.Result != ValidationResult.Pass)
            throw new ArgumentException(ruleValidation.Message, nameof(rule));

        ArgumentNullException.ThrowIfNull(model);
        ValidateModel(rule, model, assessmentTime, requestedExpiry);

        var outcomes = rule.Requirements
            .OrderBy(requirement => requirement.RequirementId, StringComparer.Ordinal)
            .Select(requirement => EvaluateRequirement(requirement, model, assessmentTime))
            .ToArray();

        var evidence = outcomes.SelectMany(outcome => outcome.Assertions)
            .DistinctBy(assertion => assertion.AssertionId, StringComparer.Ordinal)
            .OrderBy(assertion => assertion.AssertionId, StringComparer.Ordinal)
            .ToList();

        var healthState = ResolveHealthState(rule, model, evidence, assessmentTime);
        var technicalState = SelectState(outcomes);
        var recoveryRestricted = false;
        var recoveryDenial = "NOT_APPLICABLE";
        var recoveryContradictions = new List<string>();

        if (technicalState == TechnicalFitnessState.RecoveryRequired && HasOtherNotFitBlocker(outcomes))
            recoveryDenial = "OTHER_NOT_FIT_BLOCKER";
        else if (technicalState == TechnicalFitnessState.RecoveryRequired)
            recoveryRestricted = EvaluateRecoveryException(
                assessmentId, rule, model, outcomes, evidence, recoveryContradictions,
                assessmentTime, out recoveryDenial);

        var fitnessResult = technicalState == TechnicalFitnessState.RecoveryRequired && recoveryRestricted
            ? FitnessProjectionResult.Restricted
            : TechnicalFitnessRuleValidator.MapBase(technicalState);

        var contradictions = outcomes.SelectMany(outcome => outcome.ContradictionIds)
            .Concat(recoveryContradictions).Distinct(StringComparer.Ordinal)
            .OrderBy(value => value, StringComparer.Ordinal).ToArray();

        var unknowns = outcomes.Where(outcome => !string.IsNullOrWhiteSpace(outcome.UnknownReason))
            .Select(outcome => outcome.Requirement.RequirementId + "=" + outcome.UnknownReason).ToList();

        var directCircular = evidence.Any(assertion =>
            string.Equals(assertion.SourceAssessmentReference, assessmentId, StringComparison.Ordinal));
        if (directCircular)
        {
            technicalState = TechnicalFitnessState.Unknown;
            fitnessResult = FitnessProjectionResult.NotFit;
            recoveryRestricted = false;
            recoveryDenial = "DIRECT_CIRCULAR_SELF_REFERENCE";
            unknowns.Add("assessment=DIRECT_CIRCULAR_SELF_REFERENCE");
        }

        var evidenceQuality = DeriveEvidenceQuality(outcomes, evidence);
        if (directCircular && evidenceQuality != EvidenceQuality.Invalid)
            evidenceQuality = EvidenceQuality.Insufficient;
        if (contradictions.Length > 0 && evidenceQuality != EvidenceQuality.Invalid)
            evidenceQuality = EvidenceQuality.Insufficient;

        if (fitnessResult == FitnessProjectionResult.Fit && evidenceQuality != EvidenceQuality.Sufficient)
        {
            technicalState = TechnicalFitnessState.Unknown;
            fitnessResult = FitnessProjectionResult.NotFit;
            unknowns.Add("aggregate-evidence=not-sufficient-for-fit");
        }

        var freshExpiries = evidence.Where(assertion => assertion.Expiry > assessmentTime)
            .Select(assertion => assertion.Expiry).ToArray();
        var expiry = freshExpiries.Length == 0
            ? requestedExpiry
            : freshExpiries.Append(requestedExpiry).Min();
        if (expiry <= assessmentTime)
            throw new ArgumentException("Stage 7 Technical Fitness effective expiry rejected", nameof(requestedExpiry));

        var observationTime = evidence.Count == 0 ? assessmentTime : evidence.Min(assertion => assertion.ObservationTime);
        var constraints = BuildConstraints(outcomes, fitnessResult, recoveryRestricted, rule);
        var evidenceReference = BuildEvidenceReference(rule, model, evidence, contradictions);
        var reason = BuildReason(outcomes, technicalState, fitnessResult, recoveryRestricted, recoveryDenial);

        var assessment = new CanonicalHealthFitnessAssessment(
            assessmentId, rule.SubjectId, rule.Capability, rule.RequestedAuthorityLevel,
            healthState, technicalState, fitnessResult, rule.Scope, evidenceReference, model.Identity,
            evidenceQuality, ToConfidence(evidenceQuality),
            unknowns.Count == 0 ? "NONE" : string.Join(",", unknowns.Distinct(StringComparer.Ordinal).OrderBy(value => value, StringComparer.Ordinal)),
            contradictions.Length == 0 ? "NONE" : string.Join(",", contradictions),
            constraints, reason, rule.RuleId, rule.RuleVersion,
            observationTime, assessmentTime, assessmentTime, expiry);

        var primitive = HealthFitnessPrimitiveValidator.Validate(assessment);
        if (primitive.Result != ValidationResult.Pass)
            throw new InvalidOperationException(primitive.Message);
        if (HealthFitnessV12Validator.Validate(HealthFitnessContractProjection.ToContractV12(assessment)).Result != ValidationResult.Pass)
            throw new InvalidOperationException("Stage 7 Technical Fitness CON-006 v1.2 projection rejected");

        return assessment;
    }

    private static void ValidateModel(
        TechnicalFitnessRuleDefinition rule,
        FoundationSelfModelSnapshot model,
        DateTimeOffset assessmentTime,
        DateTimeOffset requestedExpiry)
    {
        if (!HealthFitnessContractV12.IsCanonicalIdentifier(model.ModelId) ||
            !HealthFitnessContractV12.IsCanonicalIdentifier(model.FoundationId) ||
            !HealthFitnessContractV12.IsCanonicalIdentifier(model.AdmittedBaselineId) ||
            !HealthFitnessContractV12.IsCanonicalIdentifier(model.EvidenceReference) ||
            string.IsNullOrWhiteSpace(model.Identity))
            throw new ArgumentException("Stage 7 Technical Fitness Self Model identity rejected", nameof(model));
        if (assessmentTime == default || requestedExpiry == default || requestedExpiry <= assessmentTime)
            throw new ArgumentException("Stage 7 Technical Fitness assessment time or expiry rejected");
        if (model.ModelTime > assessmentTime)
            throw new ArgumentException("Stage 7 Technical Fitness future Self Model rejected", nameof(model));
        if (!string.Equals(model.FoundationId, rule.SubjectId, StringComparison.Ordinal))
            throw new ArgumentException("Stage 7 Technical Fitness Self Model subject binding rejected", nameof(model));

        var canonicalModel = FoundationSelfModelProjector.Build(
            model.ModelId,
            model.FoundationId,
            model.AdmittedBaselineId,
            model.ModelTime,
            model.Assertions,
            model.PreviousModelIdentity);
        if (!string.Equals(canonicalModel.Identity, model.Identity, StringComparison.Ordinal))
            throw new ArgumentException("Stage 7 Technical Fitness non-canonical Self Model rejected", nameof(model));
    }

    private static RequirementOutcome EvaluateRequirement(
        TechnicalFitnessRequirement requirement,
        FoundationSelfModelSnapshot model,
        DateTimeOffset assessmentTime)
    {
        var all = model.Assertions.Where(assertion =>
                assertion.TemporalView == FoundationSelfModelTemporalView.Current &&
                assertion.Area == requirement.Area &&
                string.Equals(assertion.SubjectId, requirement.SubjectId, StringComparison.Ordinal) &&
                string.Equals(assertion.Scope, requirement.Scope, StringComparison.Ordinal) &&
                (requirement.RequiredSourceOwner is null ||
                 string.Equals(assertion.SourceOwner, requirement.RequiredSourceOwner, StringComparison.Ordinal)))
            .OrderBy(assertion => assertion.AssertionId, StringComparer.Ordinal).ToArray();

        if (all.Length == 0)
            return Outcome(requirement, TechnicalFitnessState.Unknown, all, "MISSING_CURRENT_EVIDENCE");

        var fresh = all.Where(assertion => assertion.Expiry > assessmentTime).ToArray();
        if (fresh.Length == 0)
            return Outcome(requirement, TechnicalFitnessState.Unknown, all, "STALE_CURRENT_EVIDENCE");

        var freshIds = fresh.Select(assertion => assertion.AssertionId).ToHashSet(StringComparer.Ordinal);
        var contradictions = model.Contradictions.Where(value =>
                value.Area == requirement.Area &&
                string.Equals(value.SubjectId, requirement.SubjectId, StringComparison.Ordinal) &&
                string.Equals(value.Scope, requirement.Scope, StringComparison.Ordinal) &&
                value.AssertionIds.Count(id => freshIds.Contains(id)) > 1)
            .Select(value => value.ContradictionId).OrderBy(value => value, StringComparer.Ordinal).ToArray();
        if (contradictions.Length > 0)
            return new RequirementOutcome(requirement, TechnicalFitnessState.Unknown, fresh,
                "CONTRADICTORY_CURRENT_EVIDENCE", contradictions);

        if (fresh.Any(assertion => assertion.AssertionKind == FoundationSelfModelAssertionKind.Unknown))
            return Outcome(requirement, TechnicalFitnessState.Unknown, fresh, "CURRENT_EVIDENCE_UNKNOWN");

        var quality = AggregateEvidenceQuality(fresh);
        if (quality == EvidenceQuality.Invalid)
            return Outcome(requirement, TechnicalFitnessState.Unknown, fresh, "INVALID_CURRENT_EVIDENCE");
        if (quality == EvidenceQuality.Insufficient)
            return Outcome(requirement, TechnicalFitnessState.Unknown, fresh, "INSUFFICIENT_CURRENT_EVIDENCE");

        var accepted = fresh.Any(assertion =>
            requirement.AcceptableValueIdentities.Contains(assertion.ValueIdentity, StringComparer.Ordinal));
        if (!accepted)
            return Outcome(requirement, requirement.FailureState, fresh,
                requirement.FailureState == TechnicalFitnessState.Unknown ? "RULE_CONDITION_UNKNOWN" : string.Empty);
        if (quality == EvidenceQuality.Limited)
            return Outcome(requirement, requirement.LimitedEvidenceState, fresh, "LIMITED_CURRENT_EVIDENCE");
        return Outcome(requirement, TechnicalFitnessState.Fit, fresh, string.Empty);
    }

    private static RequirementOutcome Outcome(
        TechnicalFitnessRequirement requirement,
        TechnicalFitnessState state,
        IReadOnlyList<FoundationSelfModelAssertion> assertions,
        string unknownReason) =>
        new(requirement, state, assertions, unknownReason, Array.Empty<string>());

    private static TechnicalFitnessState SelectState(IReadOnlyCollection<RequirementOutcome> outcomes)
    {
        var failures = outcomes.Where(outcome => outcome.State != TechnicalFitnessState.Fit).ToArray();
        if (failures.Length == 0) return TechnicalFitnessState.Fit;
        return failures.GroupBy(outcome => RestrictionRank(outcome.State)).OrderByDescending(group => group.Key).First()
            .OrderByDescending(outcome => outcome.Requirement.FailurePriority)
            .ThenBy(outcome => outcome.Requirement.RequirementId, StringComparer.Ordinal).First().State;
    }

    private static bool HasOtherNotFitBlocker(IEnumerable<RequirementOutcome> outcomes) =>
        outcomes.Any(outcome => outcome.State != TechnicalFitnessState.Fit &&
            outcome.State != TechnicalFitnessState.RecoveryRequired &&
            TechnicalFitnessRuleValidator.MapBase(outcome.State) == FitnessProjectionResult.NotFit);

    private static int RestrictionRank(TechnicalFitnessState state) =>
        TechnicalFitnessRuleValidator.MapBase(state) switch
        {
            FitnessProjectionResult.NotFit => 3,
            FitnessProjectionResult.Restricted => 2,
            _ => 1
        };

    private static HealthState ResolveHealthState(
        TechnicalFitnessRuleDefinition rule,
        FoundationSelfModelSnapshot model,
        ICollection<FoundationSelfModelAssertion> evidence,
        DateTimeOffset assessmentTime)
    {
        var health = model.Assertions.Where(assertion =>
                assertion.TemporalView == FoundationSelfModelTemporalView.Current &&
                assertion.Area == FoundationSelfModelArea.HealthCondition &&
                string.Equals(assertion.SubjectId, rule.SubjectId, StringComparison.Ordinal) &&
                string.Equals(assertion.Scope, rule.Scope, StringComparison.Ordinal) &&
                assertion.Expiry > assessmentTime)
            .OrderBy(assertion => assertion.AssertionId, StringComparer.Ordinal).ToArray();

        foreach (var assertion in health)
            if (!evidence.Any(existing => existing.AssertionId == assertion.AssertionId)) evidence.Add(assertion);

        var ids = health.Select(assertion => assertion.AssertionId).ToHashSet(StringComparer.Ordinal);
        if (health.Length == 0 || model.Contradictions.Any(value =>
                value.Area == FoundationSelfModelArea.HealthCondition &&
                string.Equals(value.SubjectId, rule.SubjectId, StringComparison.Ordinal) &&
                string.Equals(value.Scope, rule.Scope, StringComparison.Ordinal) &&
                value.AssertionIds.Count(id => ids.Contains(id)) > 1))
            return HealthState.Unknown;

        var states = health.Select(assertion => ParseHealth(assertion.ValueIdentity)).Distinct().ToArray();
        return states.Length == 1 ? states[0] : HealthState.Unknown;
    }

    private static HealthState ParseHealth(string value) => value switch
    {
        "health-state:healthy" => HealthState.Healthy,
        "health-state:degraded" => HealthState.Degraded,
        "health-state:unhealthy" => HealthState.Unhealthy,
        "health-state:not-applicable" or "health-state:notapplicable" => HealthState.NotApplicable,
        _ => HealthState.Unknown
    };

    private static bool EvaluateRecoveryException(
        string assessmentId,
        TechnicalFitnessRuleDefinition rule,
        FoundationSelfModelSnapshot model,
        IReadOnlyCollection<RequirementOutcome> outcomes,
        ICollection<FoundationSelfModelAssertion> evidence,
        ICollection<string> contradictionIds,
        DateTimeOffset assessmentTime,
        out string denialReason)
    {
        var declaration = rule.RecoveryRestrictedMode;
        if (declaration is null)
        {
            denialReason = "NOT_DECLARED";
            return false;
        }

        var faultEvidence = outcomes.Where(outcome => outcome.State == TechnicalFitnessState.RecoveryRequired)
            .SelectMany(outcome => outcome.Assertions).ToArray();
        if (faultEvidence.Length == 0 || faultEvidence.Any(assertion =>
                !string.Equals(assertion.SourceOwner, declaration.FaultSourceOwner, StringComparison.Ordinal)))
        {
            denialReason = "FAULT_SOURCE_BINDING_FAILED";
            return false;
        }

        var denials = new List<string>();
        foreach (var proof in declaration.Proofs.OrderBy(value => (int)value.Condition)
                     .ThenBy(value => value.AssertionId, StringComparer.Ordinal))
        {
            var assertion = model.Assertions.SingleOrDefault(candidate =>
                candidate.TemporalView == FoundationSelfModelTemporalView.Current &&
                string.Equals(candidate.AssertionId, proof.AssertionId, StringComparison.Ordinal) &&
                candidate.Area == proof.Area &&
                string.Equals(candidate.SubjectId, proof.SubjectId, StringComparison.Ordinal) &&
                string.Equals(candidate.Scope, proof.Scope, StringComparison.Ordinal) &&
                (proof.RequiredSourceOwner is null ||
                 string.Equals(candidate.SourceOwner, proof.RequiredSourceOwner, StringComparison.Ordinal)));

            if (assertion is null)
            {
                denials.Add(proof.Condition + ":MISSING_PROOF");
                continue;
            }

            if (!evidence.Any(existing => existing.AssertionId == assertion.AssertionId)) evidence.Add(assertion);
            var activeContradictions = ActiveContradictions(model, assertion, assessmentTime);
            foreach (var id in activeContradictions)
                if (!contradictionIds.Contains(id, StringComparer.Ordinal)) contradictionIds.Add(id);

            if (assertion.Expiry <= assessmentTime) denials.Add(proof.Condition + ":STALE_PROOF");
            if (assertion.AssertionKind == FoundationSelfModelAssertionKind.Unknown) denials.Add(proof.Condition + ":UNKNOWN_PROOF");
            if (assertion.EvidenceQuality != EvidenceQuality.Sufficient) denials.Add(proof.Condition + ":NON_SUFFICIENT_PROOF");
            if (!string.Equals(assertion.ValueIdentity, proof.ExpectedValueIdentity, StringComparison.Ordinal)) denials.Add(proof.Condition + ":VALUE_MISMATCH");
            if (activeContradictions.Count > 0) denials.Add(proof.Condition + ":CONTRADICTORY_PROOF");
            if (string.Equals(assertion.SourceAssessmentReference, assessmentId, StringComparison.Ordinal)) denials.Add(proof.Condition + ":DIRECT_CIRCULAR_PROOF");
            if (proof.Condition == RecoveryRestrictedCondition.IndependentUsabilityProven &&
                string.Equals(assertion.SourceOwner, declaration.FaultSourceOwner, StringComparison.Ordinal))
                denials.Add(proof.Condition + ":NOT_INDEPENDENT");
        }

        if (denials.Count == 0)
        {
            denialReason = "NONE";
            return true;
        }

        denialReason = string.Join("|", denials.Distinct(StringComparer.Ordinal).OrderBy(value => value, StringComparer.Ordinal));
        return false;
    }

    private static IReadOnlyList<string> ActiveContradictions(
        FoundationSelfModelSnapshot model,
        FoundationSelfModelAssertion assertion,
        DateTimeOffset assessmentTime)
    {
        var activeIds = model.Assertions.Where(candidate =>
                candidate.TemporalView == FoundationSelfModelTemporalView.Current &&
                candidate.Area == assertion.Area &&
                string.Equals(candidate.SubjectId, assertion.SubjectId, StringComparison.Ordinal) &&
                string.Equals(candidate.Scope, assertion.Scope, StringComparison.Ordinal) &&
                candidate.Expiry > assessmentTime)
            .Select(candidate => candidate.AssertionId).ToHashSet(StringComparer.Ordinal);

        return model.Contradictions.Where(value =>
                value.Area == assertion.Area &&
                string.Equals(value.SubjectId, assertion.SubjectId, StringComparison.Ordinal) &&
                string.Equals(value.Scope, assertion.Scope, StringComparison.Ordinal) &&
                value.AssertionIds.Contains(assertion.AssertionId, StringComparer.Ordinal) &&
                value.AssertionIds.Count(id => activeIds.Contains(id)) > 1)
            .Select(value => value.ContradictionId).Distinct(StringComparer.Ordinal)
            .OrderBy(value => value, StringComparer.Ordinal).ToArray();
    }

    private static EvidenceQuality DeriveEvidenceQuality(
        IEnumerable<RequirementOutcome> outcomes,
        IEnumerable<FoundationSelfModelAssertion> assertions)
    {
        var reasons = outcomes.Select(outcome => outcome.UnknownReason).Where(value => !string.IsNullOrWhiteSpace(value)).ToArray();
        if (reasons.Contains("INVALID_CURRENT_EVIDENCE", StringComparer.Ordinal)) return EvidenceQuality.Invalid;
        if (reasons.Any(value => value is "MISSING_CURRENT_EVIDENCE" or "STALE_CURRENT_EVIDENCE" or
                "CURRENT_EVIDENCE_UNKNOWN" or "INSUFFICIENT_CURRENT_EVIDENCE" or
                "CONTRADICTORY_CURRENT_EVIDENCE" or "RULE_CONDITION_UNKNOWN"))
            return EvidenceQuality.Insufficient;
        return AggregateEvidenceQuality(assertions);
    }

    private static EvidenceQuality AggregateEvidenceQuality(IEnumerable<FoundationSelfModelAssertion> assertions)
    {
        var values = assertions.Select(assertion => assertion.EvidenceQuality).ToArray();
        if (values.Length == 0) return EvidenceQuality.Insufficient;
        if (values.Contains(EvidenceQuality.Invalid)) return EvidenceQuality.Invalid;
        if (values.Contains(EvidenceQuality.Insufficient)) return EvidenceQuality.Insufficient;
        if (values.Contains(EvidenceQuality.Limited)) return EvidenceQuality.Limited;
        return EvidenceQuality.Sufficient;
    }

    private static string BuildConstraints(
        IEnumerable<RequirementOutcome> outcomes,
        FitnessProjectionResult result,
        bool recoveryRestricted,
        TechnicalFitnessRuleDefinition rule)
    {
        if (result != FitnessProjectionResult.Restricted) return "NONE";

        IEnumerable<string> values = outcomes.Where(outcome => outcome.State != TechnicalFitnessState.Fit &&
                TechnicalFitnessRuleValidator.MapBase(outcome.State) == FitnessProjectionResult.Restricted)
            .Select(outcome => outcome.Requirement.Constraint);
        if (recoveryRestricted)
            values = values.Append(rule.RecoveryRestrictedMode!.Constraints);

        var material = values.Where(value => !string.IsNullOrWhiteSpace(value) &&
                !string.Equals(value.Trim(), "NONE", StringComparison.OrdinalIgnoreCase))
            .Distinct(StringComparer.Ordinal).OrderBy(value => value, StringComparer.Ordinal).ToArray();
        return material.Length == 0 ? "NONE" : string.Join(";", material);
    }

    private static string BuildReason(
        IEnumerable<RequirementOutcome> outcomes,
        TechnicalFitnessState state,
        FitnessProjectionResult result,
        bool recoveryRestricted,
        string recoveryDenial)
    {
        var failed = outcomes.Where(outcome => outcome.State != TechnicalFitnessState.Fit)
            .Select(outcome => outcome.Requirement.RequirementId + ":" + outcome.State)
            .OrderBy(value => value, StringComparer.Ordinal).ToArray();
        return "TECHNICAL_STATE=" + state + ";FITNESS_RESULT=" + result +
            ";RECOVERY_RESTRICTED_EXCEPTION=" + (recoveryRestricted ? "SATISFIED" : "NOT_SATISFIED") +
            ";RECOVERY_EXCEPTION_DENIAL=" + recoveryDenial +
            ";FAILED_REQUIREMENTS=" + (failed.Length == 0 ? "NONE" : string.Join(",", failed));
    }

    private static string BuildEvidenceReference(
        TechnicalFitnessRuleDefinition rule,
        FoundationSelfModelSnapshot model,
        IEnumerable<FoundationSelfModelAssertion> assertions,
        IEnumerable<string> contradictionIds)
    {
        var builder = new StringBuilder();
        Append(builder, rule.RuleId);
        Append(builder, rule.RuleVersion);
        Append(builder, model.Identity);
        foreach (var identity in assertions.Select(value => value.Identity).OrderBy(value => value, StringComparer.Ordinal)) Append(builder, identity);
        foreach (var id in contradictionIds.OrderBy(value => value, StringComparer.Ordinal)) Append(builder, id);
        return "fitness:evidence:sha256:" + Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(builder.ToString())));
    }

    private static void Append(StringBuilder builder, string value)
    {
        builder.Append(value.Length).Append(':').Append(value).Append('|');
    }

    private static string ToConfidence(EvidenceQuality quality) => quality switch
    {
        EvidenceQuality.Sufficient => "SUFFICIENT",
        EvidenceQuality.Limited => "LIMITED",
        EvidenceQuality.Insufficient => "INSUFFICIENT",
        _ => "INVALID"
    };
}