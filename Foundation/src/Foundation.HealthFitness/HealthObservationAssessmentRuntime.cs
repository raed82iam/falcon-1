using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Foundation.Contracts;

namespace Foundation.HealthFitness;

public enum HealthDimension
{
    Availability = 1,
    Correctness = 2,
    Integrity = 3,
    Performance = 4,
    Dependency = 5
}

public enum HealthEvidenceRole
{
    RequiredPrimary = 1,
    RequiredIndependent = 2,
    Supporting = 3,
    DiagnosticOnly = 4
}

public enum HealthFreshnessProfile
{
    Critical = 1,
    Fast = 2,
    Standard = 3,
    Slow = 4,
    SourceBound = 5,
    EventBound = 6
}

public enum HealthConsequenceClass
{
    ObservationOnly = 1,
    Degrading = 2,
    CapabilityBlocking = 3,
    TrustBlocking = 4,
    RecoveryGated = 5
}

public enum HealthDependencyCriticality
{
    Required = 1,
    Degradable = 2,
    Informational = 3
}

public enum HealthObservationCondition
{
    Satisfied = 1,
    Degraded = 2,
    Failed = 3,
    Unknown = 4,
    NotApplicable = 5
}

public sealed record HealthEvidenceRequirement(
    string RequirementId,
    HealthDimension Dimension,
    HealthEvidenceRole Role,
    string SourceId,
    string SourceOwner);

public sealed record HealthDependencyRequirement(
    string DependencyId,
    string Capability,
    HealthDependencyCriticality Criticality,
    bool DegradedModeDeclared);

public sealed record HealthRuleDefinition(
    string RuleId,
    string RuleVersion,
    string SubjectId,
    string Capability,
    HealthFreshnessProfile FreshnessProfile,
    TimeSpan? ConfiguredFreshnessWindow,
    HealthConsequenceClass ConsequenceClass,
    string AccountableOwner,
    string GoverningAuthority,
    bool Applicable,
    bool UsesIndependentEventWitness,
    IReadOnlyList<HealthEvidenceRequirement> EvidenceRequirements,
    IReadOnlyList<HealthDependencyRequirement> Dependencies);

public sealed record HealthObservation(
    string ObservationId,
    string RequirementId,
    string SubjectId,
    string Capability,
    HealthDimension Dimension,
    string SourceId,
    string SourceOwner,
    string EvidenceReference,
    HealthObservationCondition Condition,
    DateTimeOffset ObservationTime,
    DateTimeOffset? SourceExpiry,
    bool ProvenanceValid,
    bool IntegrityValid,
    bool ClockValid,
    bool PositiveProofAcyclic,
    bool Visible,
    bool EventWitnessCurrent);

public sealed record HealthDependencyAssessment(
    string DependencyId,
    string Capability,
    HealthState HealthState,
    EvidenceQuality EvidenceQuality,
    string EvidenceReference,
    DateTimeOffset ObservationTime,
    DateTimeOffset Expiry,
    bool IndependentModeEvidenceValid);

public sealed record CanonicalHealthAssessment(
    string AssessmentId,
    string SubjectId,
    string Capability,
    HealthState HealthState,
    EvidenceQuality EvidenceQuality,
    string EvidenceReference,
    string Confidence,
    string Contradictions,
    string BlindSpots,
    string ReasonCode,
    string ReducedByDependencyId,
    HealthConsequenceClass ConsequenceClass,
    string RuleId,
    string RuleVersion,
    DateTimeOffset ObservationTime,
    DateTimeOffset AssessmentTime)
{
    public string Identity => HealthAssessmentIdentity.Compute(this);
}

public sealed record HealthStateTransition(
    string TransitionId,
    string SubjectId,
    string Capability,
    HealthState From,
    HealthState To,
    string AssessmentId,
    DateTimeOffset OccurredAt,
    string ReasonCode);

public sealed record HealthEvaluationResult(
    CanonicalHealthAssessment Assessment,
    HealthStateTransition? Transition);

public static class HealthRuleValidator
{
    public static ValidationOutcome Validate(HealthRuleDefinition? rule)
    {
        if (rule is null)
        {
            return ValidationOutcome.Failed("Stage 7 Health rule missing");
        }

        if (!HealthFitnessContractV12.IsCanonicalIdentifier(rule.RuleId) ||
            !HealthFitnessContractV12.IsCanonicalIdentifier(rule.RuleVersion) ||
            !HealthFitnessContractV12.IsCanonicalIdentifier(rule.SubjectId) ||
            !HealthFitnessContractV12.IsCanonicalIdentifier(rule.Capability) ||
            !HealthFitnessContractV12.IsCanonicalIdentifier(rule.AccountableOwner) ||
            !HealthFitnessContractV12.IsCanonicalIdentifier(rule.GoverningAuthority))
        {
            return ValidationOutcome.Failed("Stage 7 Health rule canonical identity rejected");
        }

        if (!Enum.IsDefined(rule.FreshnessProfile) || !Enum.IsDefined(rule.ConsequenceClass))
        {
            return ValidationOutcome.Failed("Stage 7 Health rule enum rejected");
        }

        if (rule.ConfiguredFreshnessWindow is { } configured && configured <= TimeSpan.Zero)
        {
            return ValidationOutcome.Failed("Stage 7 Health configured freshness must be positive");
        }

        if (rule.EvidenceRequirements is null || rule.Dependencies is null)
        {
            return ValidationOutcome.Failed("Stage 7 Health rule declarations missing");
        }

        if (rule.Applicable &&
            !rule.EvidenceRequirements.Any(requirement =>
                requirement is not null &&
                (requirement.Role is HealthEvidenceRole.RequiredPrimary or HealthEvidenceRole.RequiredIndependent)))
        {
            return ValidationOutcome.Failed("Stage 7 Health applicable rule requires required evidence");
        }

        var requirementIds = new HashSet<string>(StringComparer.Ordinal);
        foreach (var requirement in rule.EvidenceRequirements)
        {
            if (requirement is null ||
                !HealthFitnessContractV12.IsCanonicalIdentifier(requirement.RequirementId) ||
                !HealthFitnessContractV12.IsCanonicalIdentifier(requirement.SourceId) ||
                !HealthFitnessContractV12.IsCanonicalIdentifier(requirement.SourceOwner) ||
                !Enum.IsDefined(requirement.Dimension) ||
                !Enum.IsDefined(requirement.Role))
            {
                return ValidationOutcome.Failed("Stage 7 Health evidence declaration rejected");
            }

            if (!requirementIds.Add(requirement.RequirementId))
            {
                return ValidationOutcome.Failed("Stage 7 Health evidence requirement duplicated");
            }
        }

        var dependencyIds = new HashSet<string>(StringComparer.Ordinal);
        foreach (var dependency in rule.Dependencies)
        {
            if (dependency is null ||
                !HealthFitnessContractV12.IsCanonicalIdentifier(dependency.DependencyId) ||
                !HealthFitnessContractV12.IsCanonicalIdentifier(dependency.Capability) ||
                !Enum.IsDefined(dependency.Criticality))
            {
                return ValidationOutcome.Failed("Stage 7 Health dependency declaration rejected");
            }

            if (!dependencyIds.Add(dependency.DependencyId))
            {
                return ValidationOutcome.Failed("Stage 7 Health dependency duplicated");
            }
        }

        return ValidationOutcome.Passed("Stage 7 Health rule valid");
    }
}

public static class HealthFreshnessPolicy
{
    public static readonly TimeSpan CriticalMaximumAge = TimeSpan.FromSeconds(5);
    public static readonly TimeSpan FastMaximumAge = TimeSpan.FromSeconds(15);
    public static readonly TimeSpan StandardMaximumAge = TimeSpan.FromSeconds(60);
    public static readonly TimeSpan SlowMaximumAge = TimeSpan.FromSeconds(300);

    public static TimeSpan? GetProfileMaximumAge(HealthFreshnessProfile profile, bool independentEventWitnessConfigured)
    {
        return profile switch
        {
            HealthFreshnessProfile.Critical => CriticalMaximumAge,
            HealthFreshnessProfile.Fast => FastMaximumAge,
            HealthFreshnessProfile.Standard => StandardMaximumAge,
            HealthFreshnessProfile.Slow => SlowMaximumAge,
            HealthFreshnessProfile.SourceBound => null,
            HealthFreshnessProfile.EventBound when independentEventWitnessConfigured => null,
            HealthFreshnessProfile.EventBound => SlowMaximumAge,
            _ => throw new ArgumentOutOfRangeException(nameof(profile))
        };
    }

    public static TimeSpan? GetEffectiveMaximumAge(HealthRuleDefinition rule)
    {
        ArgumentNullException.ThrowIfNull(rule);
        var profile = GetProfileMaximumAge(rule.FreshnessProfile, rule.UsesIndependentEventWitness);
        if (rule.ConfiguredFreshnessWindow is not { } configured)
        {
            return profile;
        }

        return profile is null || configured < profile.Value ? configured : profile;
    }
}

public static class HealthObservationAssessmentRuntime
{
    public static HealthEvaluationResult Evaluate(
        HealthRuleDefinition rule,
        IReadOnlyCollection<HealthObservation> observations,
        IReadOnlyCollection<HealthDependencyAssessment> dependencies,
        DateTimeOffset assessmentTime,
        HealthState? previousState = null)
    {
        var ruleValidation = HealthRuleValidator.Validate(rule);
        if (ruleValidation.Result != ValidationResult.Pass)
        {
            throw new ArgumentException(ruleValidation.Message, nameof(rule));
        }

        ArgumentNullException.ThrowIfNull(observations);
        ArgumentNullException.ThrowIfNull(dependencies);

        if (assessmentTime == default)
        {
            throw new ArgumentException("Stage 7 Health assessment time missing", nameof(assessmentTime));
        }

        if (!rule.Applicable)
        {
            return BuildResult(
                rule,
                HealthState.NotApplicable,
                EvidenceQuality.Sufficient,
                "health:evidence:not-applicable:" + rule.RuleId,
                "SUFFICIENT",
                "NONE",
                "NONE",
                "RULE_NOT_APPLICABLE",
                "NONE",
                assessmentTime,
                assessmentTime,
                previousState);
        }

        var selectedEvidence = new List<HealthObservation>();
        var limited = false;
        var degraded = false;

        foreach (var requirement in rule.EvidenceRequirements)
        {
            var matching = observations
                .Where(observation => MatchesRuleEvidence(rule, requirement, observation))
                .OrderBy(observation => observation.ObservationId, StringComparer.Ordinal)
                .ToArray();

            var required = requirement.Role is HealthEvidenceRole.RequiredPrimary or HealthEvidenceRole.RequiredIndependent;

            if (matching.Length == 0)
            {
                if (required)
                {
                    return FailClosedUnknown(
                        rule,
                        "MISSING_REQUIRED_EVIDENCE",
                        requirement.RequirementId,
                        assessmentTime,
                        selectedEvidence,
                        previousState);
                }

                continue;
            }

            var structurallyValid = matching.Where(IsObservationStructurallyValid).ToArray();
            if (structurallyValid.Length == 0)
            {
                if (required)
                {
                    return BuildUnknown(
                        rule,
                        EvidenceQuality.Invalid,
                        "INVALID_REQUIRED_EVIDENCE",
                        requirement.RequirementId,
                        assessmentTime,
                        selectedEvidence.Concat(matching),
                        previousState);
                }

                limited = true;
                continue;
            }

            if (required && structurallyValid.All(observation => !observation.Visible))
            {
                return FailClosedUnknown(
                    rule,
                    "MONITOR_VISIBILITY_LOST",
                    requirement.RequirementId,
                    assessmentTime,
                    selectedEvidence.Concat(structurallyValid),
                    previousState);
            }

            var reliable = structurallyValid
                .Where(observation => observation.Visible &&
                                      observation.ProvenanceValid &&
                                      observation.IntegrityValid &&
                                      observation.ClockValid)
                .ToArray();

            if (reliable.Length == 0)
            {
                if (required)
                {
                    return BuildUnknown(
                        rule,
                        EvidenceQuality.Invalid,
                        "INVALID_REQUIRED_EVIDENCE",
                        requirement.RequirementId,
                        assessmentTime,
                        selectedEvidence.Concat(structurallyValid),
                        previousState);
                }

                limited = true;
                continue;
            }

            var fresh = reliable.Where(observation => IsFresh(rule, observation, assessmentTime)).ToArray();
            if (fresh.Length == 0)
            {
                if (required)
                {
                    return FailClosedUnknown(
                        rule,
                        "STALE_REQUIRED_EVIDENCE",
                        requirement.RequirementId,
                        assessmentTime,
                        selectedEvidence.Concat(reliable),
                        previousState);
                }

                limited = true;
                continue;
            }

            if (required && fresh.All(observation => !observation.PositiveProofAcyclic))
            {
                return FailClosedUnknown(
                    rule,
                    "CYCLIC_REQUIRED_POSITIVE_PROOF",
                    requirement.RequirementId,
                    assessmentTime,
                    selectedEvidence.Concat(fresh),
                    previousState);
            }

            var usable = fresh.Where(observation => observation.PositiveProofAcyclic).ToArray();
            if (usable.Length == 0)
            {
                limited = true;
                continue;
            }

            var conditions = usable.Select(observation => observation.Condition).Distinct().ToArray();
            if (conditions.Length > 1)
            {
                var contradiction = string.Join(",", usable.Select(observation => observation.ObservationId).OrderBy(value => value, StringComparer.Ordinal));
                return BuildUnknown(
                    rule,
                    required ? EvidenceQuality.Insufficient : EvidenceQuality.Limited,
                    required ? "CONTRADICTORY_REQUIRED_EVIDENCE" : "CONTRADICTORY_NON_REQUIRED_EVIDENCE",
                    contradiction,
                    assessmentTime,
                    selectedEvidence.Concat(usable),
                    previousState,
                    contradiction);
            }

            selectedEvidence.AddRange(usable);

            if (!required)
            {
                if (usable.Any(observation => observation.Condition != HealthObservationCondition.Satisfied))
                {
                    limited = true;
                }

                continue;
            }

            var condition = usable[0].Condition;
            switch (condition)
            {
                case HealthObservationCondition.Satisfied:
                    break;
                case HealthObservationCondition.Degraded:
                    degraded = true;
                    break;
                case HealthObservationCondition.Failed:
                    return BuildTerminal(
                        rule,
                        HealthState.Unhealthy,
                        EvidenceQuality.Sufficient,
                        "REQUIRED_CONDITION_FAILED",
                        "NONE",
                        assessmentTime,
                        selectedEvidence,
                        previousState);
                case HealthObservationCondition.Unknown:
                    return FailClosedUnknown(
                        rule,
                        "REQUIRED_CONDITION_UNKNOWN",
                        requirement.RequirementId,
                        assessmentTime,
                        selectedEvidence,
                        previousState);
                case HealthObservationCondition.NotApplicable:
                    return FailClosedUnknown(
                        rule,
                        "REQUIRED_EVIDENCE_NOT_APPLICABLE",
                        requirement.RequirementId,
                        assessmentTime,
                        selectedEvidence,
                        previousState);
                default:
                    throw new InvalidOperationException("Unhandled Health observation condition");
            }
        }

        foreach (var dependencyRequirement in rule.Dependencies)
        {
            var matchingDependencies = dependencies
                .Where(candidate =>
                    string.Equals(candidate.DependencyId, dependencyRequirement.DependencyId, StringComparison.Ordinal) &&
                    string.Equals(candidate.Capability, dependencyRequirement.Capability, StringComparison.Ordinal))
                .OrderBy(candidate => candidate.EvidenceReference, StringComparer.Ordinal)
                .ThenBy(candidate => candidate.ObservationTime)
                .ToArray();

            if (dependencyRequirement.Criticality == HealthDependencyCriticality.Informational)
            {
                continue;
            }

            if (matchingDependencies.Length > 1)
            {
                var contradiction = string.Join(",", matchingDependencies
                    .Select(candidate => candidate.EvidenceReference)
                    .OrderBy(value => value, StringComparer.Ordinal));

                return BuildUnknown(
                    rule,
                    EvidenceQuality.Insufficient,
                    "CONTRADICTORY_DEPENDENCY_EVIDENCE",
                    dependencyRequirement.DependencyId,
                    assessmentTime,
                    selectedEvidence,
                    previousState,
                    contradiction,
                    additionalEvidenceReferences: matchingDependencies.Select(candidate => candidate.EvidenceReference),
                    additionalObservationTimes: matchingDependencies.Select(candidate => candidate.ObservationTime));
            }

            var dependency = matchingDependencies.SingleOrDefault();

            if (dependency is null)
            {
                return FailClosedUnknown(
                    rule,
                    "DEPENDENCY_EVIDENCE_MISSING",
                    dependencyRequirement.DependencyId,
                    assessmentTime,
                    selectedEvidence,
                    previousState);
            }

            if (dependency.ObservationTime > assessmentTime)
            {
                return BuildUnknown(
                    rule,
                    EvidenceQuality.Insufficient,
                    "DEPENDENCY_EVIDENCE_FUTURE_DATED",
                    dependencyRequirement.DependencyId,
                    assessmentTime,
                    selectedEvidence,
                    previousState,
                    additionalEvidenceReferences: new[] { dependency.EvidenceReference },
                    additionalObservationTimes: new[] { dependency.ObservationTime });
            }

            if (!IsDependencyAssessmentValid(dependency) || dependency.Expiry <= assessmentTime)
            {
                return BuildUnknown(
                    rule,
                    EvidenceQuality.Insufficient,
                    "DEPENDENCY_EVIDENCE_STALE_OR_INVALID",
                    dependencyRequirement.DependencyId,
                    assessmentTime,
                    selectedEvidence,
                    previousState,
                    additionalEvidenceReferences: new[] { dependency.EvidenceReference },
                    additionalObservationTimes: new[] { dependency.ObservationTime });
            }

            if (dependency.EvidenceQuality is EvidenceQuality.Insufficient or EvidenceQuality.Invalid)
            {
                return BuildUnknown(
                    rule,
                    dependency.EvidenceQuality,
                    "DEPENDENCY_EVIDENCE_INSUFFICIENT",
                    dependencyRequirement.DependencyId,
                    assessmentTime,
                    selectedEvidence,
                    previousState,
                    additionalEvidenceReferences: new[] { dependency.EvidenceReference },
                    additionalObservationTimes: new[] { dependency.ObservationTime });
            }

            if (dependencyRequirement.Criticality == HealthDependencyCriticality.Required)
            {
                switch (dependency.HealthState)
                {
                    case HealthState.Unhealthy:
                        return BuildTerminal(
                            rule,
                            HealthState.Unhealthy,
                            EvidenceQuality.Sufficient,
                            "REQUIRED_DEPENDENCY_UNHEALTHY",
                            dependency.DependencyId,
                            assessmentTime,
                            selectedEvidence,
                            previousState,
                            dependency.EvidenceReference,
                            dependency.ObservationTime);
                    case HealthState.Unknown:
                        return BuildUnknown(
                            rule,
                            EvidenceQuality.Insufficient,
                            "REQUIRED_DEPENDENCY_UNKNOWN",
                            dependency.DependencyId,
                            assessmentTime,
                            selectedEvidence,
                            previousState,
                            additionalEvidenceReferences: new[] { dependency.EvidenceReference },
                            additionalObservationTimes: new[] { dependency.ObservationTime });
                    case HealthState.Degraded:
                        degraded = true;
                        limited = limited || dependency.EvidenceQuality == EvidenceQuality.Limited;
                        break;
                    case HealthState.Healthy:
                        if (dependency.EvidenceQuality == EvidenceQuality.Limited)
                        {
                            return BuildUnknown(
                                rule,
                                EvidenceQuality.Limited,
                                "REQUIRED_DEPENDENCY_EVIDENCE_LIMITED",
                                dependency.DependencyId,
                                assessmentTime,
                                selectedEvidence,
                                previousState,
                                additionalEvidenceReferences: new[] { dependency.EvidenceReference },
                                additionalObservationTimes: new[] { dependency.ObservationTime });
                        }

                        break;
                    case HealthState.NotApplicable:
                        return BuildUnknown(
                            rule,
                            EvidenceQuality.Insufficient,
                            "REQUIRED_DEPENDENCY_NOT_APPLICABLE",
                            dependency.DependencyId,
                            assessmentTime,
                            selectedEvidence,
                            previousState,
                            additionalEvidenceReferences: new[] { dependency.EvidenceReference },
                            additionalObservationTimes: new[] { dependency.ObservationTime });
                    default:
                        throw new InvalidOperationException("Unhandled required dependency Health state");
                }

                continue;
            }

            if (dependencyRequirement.Criticality == HealthDependencyCriticality.Degradable)
            {
                if (dependency.HealthState == HealthState.Healthy)
                {
                    limited = limited || dependency.EvidenceQuality == EvidenceQuality.Limited;
                    continue;
                }

                if (dependency.HealthState == HealthState.NotApplicable)
                {
                    continue;
                }

                if (dependencyRequirement.DegradedModeDeclared &&
                    dependency.IndependentModeEvidenceValid &&
                    dependency.EvidenceQuality is EvidenceQuality.Sufficient or EvidenceQuality.Limited &&
                    dependency.HealthState is HealthState.Degraded or HealthState.Unhealthy)
                {
                    degraded = true;
                    limited = true;
                    continue;
                }

                if (dependency.HealthState == HealthState.Unhealthy)
                {
                    return BuildTerminal(
                        rule,
                        HealthState.Unhealthy,
                        EvidenceQuality.Sufficient,
                        "DEGRADABLE_DEPENDENCY_UNHEALTHY_WITHOUT_PROVEN_MODE",
                        dependency.DependencyId,
                        assessmentTime,
                        selectedEvidence,
                        previousState,
                        dependency.EvidenceReference,
                        dependency.ObservationTime);
                }

                return BuildUnknown(
                    rule,
                    EvidenceQuality.Insufficient,
                    "DEGRADABLE_DEPENDENCY_UNPROVEN_MODE",
                    dependency.DependencyId,
                    assessmentTime,
                    selectedEvidence,
                    previousState,
                    additionalEvidenceReferences: new[] { dependency.EvidenceReference },
                    additionalObservationTimes: new[] { dependency.ObservationTime });
            }
        }

        if (limited && !degraded)
        {
            return BuildUnknown(
                rule,
                EvidenceQuality.Limited,
                "NON_REQUIRED_EVIDENCE_LIMITED",
                "NON_REQUIRED_EVIDENCE_LIMITED",
                assessmentTime,
                selectedEvidence,
                previousState);
        }

        var state = degraded ? HealthState.Degraded : HealthState.Healthy;
        var quality = limited ? EvidenceQuality.Limited : EvidenceQuality.Sufficient;
        var reason = degraded ? "KNOWN_BOUNDED_DEGRADATION" : "ALL_REQUIRED_EVIDENCE_HEALTHY";

        return BuildTerminal(
            rule,
            state,
            quality,
            reason,
            "NONE",
            assessmentTime,
            selectedEvidence,
            previousState);
    }

    private static bool MatchesRuleEvidence(
        HealthRuleDefinition rule,
        HealthEvidenceRequirement requirement,
        HealthObservation observation)
    {
        return observation is not null &&
               string.Equals(observation.RequirementId, requirement.RequirementId, StringComparison.Ordinal) &&
               string.Equals(observation.SubjectId, rule.SubjectId, StringComparison.Ordinal) &&
               string.Equals(observation.Capability, rule.Capability, StringComparison.Ordinal) &&
               observation.Dimension == requirement.Dimension &&
               string.Equals(observation.SourceId, requirement.SourceId, StringComparison.Ordinal) &&
               string.Equals(observation.SourceOwner, requirement.SourceOwner, StringComparison.Ordinal);
    }

    private static bool IsObservationStructurallyValid(HealthObservation observation)
    {
        if (!HealthFitnessContractV12.IsCanonicalIdentifier(observation.ObservationId) ||
            !HealthFitnessContractV12.IsCanonicalIdentifier(observation.RequirementId) ||
            !HealthFitnessContractV12.IsCanonicalIdentifier(observation.SubjectId) ||
            !HealthFitnessContractV12.IsCanonicalIdentifier(observation.Capability) ||
            !HealthFitnessContractV12.IsCanonicalIdentifier(observation.SourceId) ||
            !HealthFitnessContractV12.IsCanonicalIdentifier(observation.SourceOwner) ||
            !HealthFitnessContractV12.IsCanonicalIdentifier(observation.EvidenceReference) ||
            !Enum.IsDefined(observation.Dimension) ||
            !Enum.IsDefined(observation.Condition) ||
            observation.ObservationTime == default)
        {
            return false;
        }

        return observation.SourceExpiry is null || observation.SourceExpiry.Value > observation.ObservationTime;
    }

    private static bool IsFresh(HealthRuleDefinition rule, HealthObservation observation, DateTimeOffset assessmentTime)
    {
        if (observation.ObservationTime > assessmentTime)
        {
            return false;
        }

        if (observation.SourceExpiry is { } sourceExpiry && sourceExpiry <= assessmentTime)
        {
            return false;
        }

        if (rule.FreshnessProfile == HealthFreshnessProfile.SourceBound && observation.SourceExpiry is null)
        {
            return false;
        }

        if (rule.FreshnessProfile == HealthFreshnessProfile.EventBound &&
            rule.UsesIndependentEventWitness &&
            !observation.EventWitnessCurrent)
        {
            return false;
        }

        var maximumAge = HealthFreshnessPolicy.GetEffectiveMaximumAge(rule);
        if (maximumAge is { } bound && assessmentTime - observation.ObservationTime > bound)
        {
            return false;
        }

        return true;
    }

    private static bool IsDependencyAssessmentValid(HealthDependencyAssessment dependency)
    {
        return HealthFitnessContractV12.IsCanonicalIdentifier(dependency.DependencyId) &&
               HealthFitnessContractV12.IsCanonicalIdentifier(dependency.Capability) &&
               HealthFitnessContractV12.IsCanonicalIdentifier(dependency.EvidenceReference) &&
               Enum.IsDefined(dependency.HealthState) &&
               Enum.IsDefined(dependency.EvidenceQuality) &&
               dependency.ObservationTime != default &&
               dependency.Expiry > dependency.ObservationTime;
    }

    private static HealthEvaluationResult FailClosedUnknown(
        HealthRuleDefinition rule,
        string reasonCode,
        string blindSpot,
        DateTimeOffset assessmentTime,
        IEnumerable<HealthObservation> selectedEvidence,
        HealthState? previousState)
    {
        return BuildUnknown(
            rule,
            EvidenceQuality.Insufficient,
            reasonCode,
            blindSpot,
            assessmentTime,
            selectedEvidence,
            previousState);
    }

    private static HealthEvaluationResult BuildUnknown(
        HealthRuleDefinition rule,
        EvidenceQuality quality,
        string reasonCode,
        string blindSpot,
        DateTimeOffset assessmentTime,
        IEnumerable<HealthObservation> selectedEvidence,
        HealthState? previousState,
        string contradictions = "NONE",
        IEnumerable<string>? additionalEvidenceReferences = null,
        IEnumerable<DateTimeOffset>? additionalObservationTimes = null)
    {
        var evidence = selectedEvidence.ToArray();
        var references = evidence.Select(item => item.EvidenceReference).ToList();

        if (additionalEvidenceReferences is not null)
        {
            references.AddRange(
                additionalEvidenceReferences.Where(value => !string.IsNullOrWhiteSpace(value)));
        }

        var evidenceReference = BuildEvidenceReference(references);
        var observationTime = GetObservationTime(evidence, assessmentTime);

        if (additionalObservationTimes is not null)
        {
            foreach (var time in additionalObservationTimes.Where(value => value != default))
            {
                if (time < observationTime)
                {
                    observationTime = time;
                }
            }
        }

        return BuildResult(
            rule,
            HealthState.Unknown,
            quality,
            evidenceReference,
            quality == EvidenceQuality.Limited ? "LIMITED" : "INSUFFICIENT",
            contradictions,
            blindSpot,
            reasonCode,
            "NONE",
            observationTime,
            assessmentTime,
            previousState);
    }

    private static HealthEvaluationResult BuildTerminal(
        HealthRuleDefinition rule,
        HealthState state,
        EvidenceQuality quality,
        string reasonCode,
        string reducedByDependencyId,
        DateTimeOffset assessmentTime,
        IEnumerable<HealthObservation> selectedEvidence,
        HealthState? previousState,
        string? additionalEvidenceReference = null,
        DateTimeOffset? additionalObservationTime = null)
    {
        var evidence = selectedEvidence.ToArray();
        var references = evidence.Select(item => item.EvidenceReference).ToList();
        if (!string.IsNullOrWhiteSpace(additionalEvidenceReference))
        {
            references.Add(additionalEvidenceReference);
        }

        var observationTime = GetObservationTime(evidence, assessmentTime);
        if (additionalObservationTime is { } dependencyObservation && dependencyObservation < observationTime)
        {
            observationTime = dependencyObservation;
        }

        return BuildResult(
            rule,
            state,
            quality,
            BuildEvidenceReference(references),
            quality == EvidenceQuality.Sufficient ? "SUFFICIENT" : "LIMITED",
            "NONE",
            "NONE",
            reasonCode,
            reducedByDependencyId,
            observationTime,
            assessmentTime,
            previousState);
    }

    private static HealthEvaluationResult BuildResult(
        HealthRuleDefinition rule,
        HealthState state,
        EvidenceQuality quality,
        string evidenceReference,
        string confidence,
        string contradictions,
        string blindSpots,
        string reasonCode,
        string reducedByDependencyId,
        DateTimeOffset observationTime,
        DateTimeOffset assessmentTime,
        HealthState? previousState)
    {
        var assessmentId = HealthAssessmentIdentity.CreateAssessmentId(
            rule,
            state,
            quality,
            evidenceReference,
            contradictions,
            blindSpots,
            reasonCode,
            reducedByDependencyId,
            observationTime,
            assessmentTime);

        var assessment = new CanonicalHealthAssessment(
            assessmentId,
            rule.SubjectId,
            rule.Capability,
            state,
            quality,
            evidenceReference,
            confidence,
            contradictions,
            blindSpots,
            reasonCode,
            reducedByDependencyId,
            rule.ConsequenceClass,
            rule.RuleId,
            rule.RuleVersion,
            observationTime,
            assessmentTime);

        HealthStateTransition? transition = null;
        if (previousState is { } prior && prior != state)
        {
            transition = new HealthStateTransition(
                HealthAssessmentIdentity.CreateTransitionId(assessment, prior),
                rule.SubjectId,
                rule.Capability,
                prior,
                state,
                assessment.AssessmentId,
                assessmentTime,
                reasonCode);
        }

        return new HealthEvaluationResult(assessment, transition);
    }

    private static DateTimeOffset GetObservationTime(IReadOnlyCollection<HealthObservation> evidence, DateTimeOffset assessmentTime)
    {
        return evidence.Count == 0 ? assessmentTime : evidence.Min(item => item.ObservationTime);
    }

    private static string BuildEvidenceReference(IEnumerable<string> references)
    {
        var normalized = references
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Distinct(StringComparer.Ordinal)
            .OrderBy(value => value, StringComparer.Ordinal)
            .ToArray();

        return normalized.Length == 0
            ? "health:evidence:none"
            : "health:evidence-set:" + Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(string.Join("|", normalized))));
    }
}

public static class HealthAssessmentIdentity
{
    public static string Compute(CanonicalHealthAssessment value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return Hash(
            value.AssessmentId,
            value.SubjectId,
            value.Capability,
            HealthFitnessContractProjection.ToContract(value.HealthState),
            HealthFitnessContractProjection.ToContract(value.EvidenceQuality),
            value.EvidenceReference,
            value.Confidence,
            value.Contradictions,
            value.BlindSpots,
            value.ReasonCode,
            value.ReducedByDependencyId,
            value.ConsequenceClass.ToString(),
            value.RuleId,
            value.RuleVersion,
            value.ObservationTime.ToUniversalTime().ToString("O"),
            value.AssessmentTime.ToUniversalTime().ToString("O"));
    }

    public static string CreateAssessmentId(
        HealthRuleDefinition rule,
        HealthState state,
        EvidenceQuality quality,
        string evidenceReference,
        string contradictions,
        string blindSpots,
        string reasonCode,
        string reducedByDependencyId,
        DateTimeOffset observationTime,
        DateTimeOffset assessmentTime)
    {
        return "health-assessment:" + Hash(
            rule.RuleId,
            rule.RuleVersion,
            rule.SubjectId,
            rule.Capability,
            HealthFitnessContractProjection.ToContract(state),
            HealthFitnessContractProjection.ToContract(quality),
            evidenceReference,
            contradictions,
            blindSpots,
            reasonCode,
            reducedByDependencyId,
            observationTime.ToUniversalTime().ToString("O"),
            assessmentTime.ToUniversalTime().ToString("O"));
    }

    public static string CreateTransitionId(CanonicalHealthAssessment assessment, HealthState from)
    {
        ArgumentNullException.ThrowIfNull(assessment);
        return "health-transition:" + Hash(
            assessment.SubjectId,
            assessment.Capability,
            HealthFitnessContractProjection.ToContract(from),
            HealthFitnessContractProjection.ToContract(assessment.HealthState),
            assessment.AssessmentId,
            assessment.AssessmentTime.ToUniversalTime().ToString("O"),
            assessment.ReasonCode);
    }

    private static string Hash(params string[] values)
    {
        var builder = new StringBuilder();
        foreach (var value in values)
        {
            var safe = value ?? string.Empty;
            builder.Append(safe.Length);
            builder.Append(':');
            builder.Append(safe);
            builder.Append('|');
        }

        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(builder.ToString())));
    }
}
