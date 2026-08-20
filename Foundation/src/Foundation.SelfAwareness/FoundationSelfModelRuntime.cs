using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Foundation.Contracts;
using Foundation.HealthFitness;

namespace Foundation.SelfAwareness;

public enum FoundationSelfModelArea
{
    FoundationIdentity = 1,
    AdmittedBaseline = 2,
    CoreComponentIdentity = 3,
    CoreComponentVersion = 4,
    LifecycleCondition = 5,
    CoreComponentIntegrity = 6,
    RuntimeCondition = 7,
    InfrastructureCondition = 8,
    HealthCondition = 9,
    ServiceBusCondition = 10,
    FilCondition = 11,
    DependencyAvailability = 12,
    DependencyCompatibility = 13,
    DependencyCriticality = 14,
    ResourceCapacity = 15,
    ResourcePressure = 16,
    ResourceExhaustionRisk = 17,
    PersistenceCondition = 18,
    BackupCondition = 19,
    RestoreCondition = 20,
    CorruptionCondition = 21,
    DocumentationIntegrity = 22,
    ConfigurationIntegrity = 23,
    SecurityCondition = 24,
    AuthorityCondition = 25,
    IncidentCondition = 26,
    FaultCondition = 27,
    ContradictionCondition = 28,
    BlindSpotCondition = 29,
    IsolationReadiness = 30,
    RecoveryReadiness = 31,
    ActiveRestriction = 32,
    TechnicalFitness = 33,
    PendingConformance = 34
}

public enum FoundationSelfModelAssertionKind
{
    Fact = 1,
    Estimate = 2,
    Assumption = 3,
    Interpretation = 4,
    Unknown = 5
}

public enum FoundationSelfModelTemporalView
{
    Current = 1,
    LastKnown = 2,
    Expected = 3,
    Desired = 4,
    Historical = 5
}

public sealed record FoundationSelfModelAssertion(
    string AssertionId,
    string SubjectId,
    FoundationSelfModelArea Area,
    FoundationSelfModelAssertionKind AssertionKind,
    FoundationSelfModelTemporalView TemporalView,
    string Scope,
    string ValueIdentity,
    string AuthoritativeSourceId,
    string SourceOwner,
    string EvidenceReference,
    EvidenceQuality EvidenceQuality,
    string Confidence,
    string Uncertainty,
    string FreshnessReference,
    string RuleId,
    string RuleVersion,
    DateTimeOffset ObservationTime,
    DateTimeOffset EffectiveTime,
    DateTimeOffset Expiry,
    string? SourceAssessmentReference,
    string? SupersedesAssertionId)
{
    public string Identity =>
        FoundationSelfModelIdentity.ComputeAssertion(this);
}

public sealed record FoundationSelfModelContradiction(
    string ContradictionId,
    string SubjectId,
    FoundationSelfModelArea Area,
    string Scope,
    IReadOnlyList<string> AssertionIds);

public sealed record FoundationSelfModelSnapshot(
    string ModelId,
    string FoundationId,
    string AdmittedBaselineId,
    DateTimeOffset ModelTime,
    string? PreviousModelIdentity,
    IReadOnlyList<FoundationSelfModelAssertion> Assertions,
    IReadOnlyList<FoundationSelfModelContradiction> Contradictions,
    string EvidenceReference)
{
    public string Identity =>
        FoundationSelfModelIdentity.ComputeSnapshot(this);
}

public static class FoundationSelfModelAssertionValidator
{
    public static ValidationOutcome Validate(
        FoundationSelfModelAssertion? assertion,
        DateTimeOffset modelTime)
    {
        if (assertion is null)
        {
            return ValidationOutcome.Failed(
                "Stage 7 Self Model assertion missing");
        }

        if (modelTime == default)
        {
            return ValidationOutcome.Failed(
                "Stage 7 Self Model model time missing");
        }

        if (!Enum.IsDefined(assertion.Area) ||
            !Enum.IsDefined(assertion.AssertionKind) ||
            !Enum.IsDefined(assertion.TemporalView) ||
            !Enum.IsDefined(assertion.EvidenceQuality))
        {
            return ValidationOutcome.Failed(
                "Stage 7 Self Model assertion enum rejected");
        }

        if (!HealthFitnessContractV12.IsCanonicalIdentifier(assertion.AssertionId) ||
            !HealthFitnessContractV12.IsCanonicalIdentifier(assertion.SubjectId) ||
            !HealthFitnessContractV12.IsCanonicalIdentifier(assertion.Scope) ||
            !HealthFitnessContractV12.IsCanonicalIdentifier(assertion.ValueIdentity) ||
            !HealthFitnessContractV12.IsCanonicalIdentifier(assertion.AuthoritativeSourceId) ||
            !HealthFitnessContractV12.IsCanonicalIdentifier(assertion.SourceOwner) ||
            !HealthFitnessContractV12.IsCanonicalIdentifier(assertion.EvidenceReference) ||
            !HealthFitnessContractV12.IsCanonicalIdentifier(assertion.FreshnessReference) ||
            !HealthFitnessContractV12.IsCanonicalIdentifier(assertion.RuleId) ||
            !HealthFitnessContractV12.IsCanonicalIdentifier(assertion.RuleVersion))
        {
            return ValidationOutcome.Failed(
                "Stage 7 Self Model assertion canonical identity rejected");
        }

        if (assertion.SourceAssessmentReference is not null &&
            !HealthFitnessContractV12.IsCanonicalIdentifier(
                assertion.SourceAssessmentReference))
        {
            return ValidationOutcome.Failed(
                "Stage 7 Self Model source assessment identity rejected");
        }

        if (assertion.SupersedesAssertionId is not null &&
            !HealthFitnessContractV12.IsCanonicalIdentifier(
                assertion.SupersedesAssertionId))
        {
            return ValidationOutcome.Failed(
                "Stage 7 Self Model supersession identity rejected");
        }

        if (string.IsNullOrWhiteSpace(assertion.Confidence) ||
            string.IsNullOrWhiteSpace(assertion.Uncertainty))
        {
            return ValidationOutcome.Failed(
                "Stage 7 Self Model confidence or uncertainty missing");
        }

        if (assertion.ObservationTime == default ||
            assertion.EffectiveTime == default ||
            assertion.Expiry == default)
        {
            return ValidationOutcome.Failed(
                "Stage 7 Self Model assertion time missing");
        }

        if (assertion.ObservationTime > modelTime)
        {
            return ValidationOutcome.Failed(
                "Stage 7 Self Model future observation rejected");
        }

        if (assertion.Expiry <= assertion.EffectiveTime)
        {
            return ValidationOutcome.Failed(
                "Stage 7 Self Model assertion validity interval rejected");
        }

        switch (assertion.TemporalView)
        {
            case FoundationSelfModelTemporalView.Current:
                if (assertion.EffectiveTime > modelTime ||
                    assertion.Expiry <= modelTime)
                {
                    return ValidationOutcome.Failed(
                        "Stage 7 Self Model current assertion is not current");
                }

                break;

            case FoundationSelfModelTemporalView.LastKnown:
            case FoundationSelfModelTemporalView.Historical:
                if (assertion.EffectiveTime > modelTime)
                {
                    return ValidationOutcome.Failed(
                        "Stage 7 Self Model non-current historical assertion is future-effective");
                }

                break;

            case FoundationSelfModelTemporalView.Expected:
            case FoundationSelfModelTemporalView.Desired:
                break;

            default:
                return ValidationOutcome.Failed(
                    "Stage 7 Self Model temporal view rejected");
        }

        if (assertion.AssertionKind ==
                FoundationSelfModelAssertionKind.Unknown &&
            assertion.EvidenceQuality ==
                EvidenceQuality.Sufficient)
        {
            return ValidationOutcome.Failed(
                "Stage 7 Self Model unknown assertion cannot claim sufficient evidence");
        }

        if (assertion.Area ==
                FoundationSelfModelArea.HealthCondition &&
            string.IsNullOrWhiteSpace(
                assertion.SourceAssessmentReference))
        {
            return ValidationOutcome.Failed(
                "Stage 7 Self Model Health assertion source assessment missing");
        }

        if (assertion.SupersedesAssertionId is not null &&
            string.Equals(
                assertion.SupersedesAssertionId,
                assertion.AssertionId,
                StringComparison.Ordinal))
        {
            return ValidationOutcome.Failed(
                "Stage 7 Self Model assertion cannot supersede itself");
        }

        return ValidationOutcome.Passed(
            "Stage 7 Self Model assertion valid");
    }
}

public static class FoundationSelfModelProjector
{
    public static FoundationSelfModelSnapshot Build(
        string modelId,
        string foundationId,
        string admittedBaselineId,
        DateTimeOffset modelTime,
        IReadOnlyCollection<FoundationSelfModelAssertion> assertions,
        string? previousModelIdentity = null)
    {
        if (!HealthFitnessContractV12.IsCanonicalIdentifier(modelId) ||
            !HealthFitnessContractV12.IsCanonicalIdentifier(foundationId) ||
            !HealthFitnessContractV12.IsCanonicalIdentifier(admittedBaselineId))
        {
            throw new ArgumentException(
                "Stage 7 Self Model canonical model identity rejected");
        }

        if (modelTime == default)
        {
            throw new ArgumentException(
                "Stage 7 Self Model time missing",
                nameof(modelTime));
        }

        if (previousModelIdentity is not null &&
            !HealthFitnessContractV12.IsCanonicalIdentifier(
                previousModelIdentity))
        {
            throw new ArgumentException(
                "Stage 7 Self Model previous identity rejected",
                nameof(previousModelIdentity));
        }

        ArgumentNullException.ThrowIfNull(assertions);

        var assertionArray = assertions.ToArray();

        foreach (var assertion in assertionArray)
        {
            var validation =
                FoundationSelfModelAssertionValidator.Validate(
                    assertion,
                    modelTime);

            if (validation.Result != ValidationResult.Pass)
            {
                throw new ArgumentException(
                    validation.Message,
                    nameof(assertions));
            }
        }

        var duplicateIds = assertionArray
            .GroupBy(
                assertion => assertion.AssertionId,
                StringComparer.Ordinal)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .OrderBy(
                value => value,
                StringComparer.Ordinal)
            .ToArray();

        if (duplicateIds.Length > 0)
        {
            throw new ArgumentException(
                "Stage 7 Self Model duplicate assertion identity: " +
                string.Join(",", duplicateIds),
                nameof(assertions));
        }

        var representedCurrentAreas = assertionArray
            .Where(assertion =>
                assertion.TemporalView ==
                    FoundationSelfModelTemporalView.Current)
            .Select(assertion => assertion.Area)
            .ToHashSet();

        var missingCurrentAreas =
            Enum.GetValues<FoundationSelfModelArea>()
                .Where(area =>
                    !representedCurrentAreas.Contains(area))
                .OrderBy(area => (int)area)
                .ToArray();

        if (missingCurrentAreas.Length > 0)
        {
            throw new ArgumentException(
                "Stage 7 Self Model required current area missing: " +
                string.Join(",", missingCurrentAreas),
                nameof(assertions));
        }

        var sortedAssertions = assertionArray
            .OrderBy(assertion => (int)assertion.Area)
            .ThenBy(
                assertion => assertion.SubjectId,
                StringComparer.Ordinal)
            .ThenBy(
                assertion => assertion.Scope,
                StringComparer.Ordinal)
            .ThenBy(
                assertion => assertion.AssertionId,
                StringComparer.Ordinal)
            .ToArray();

        var contradictions =
            BuildContradictions(sortedAssertions);

        var evidenceReference =
            FoundationSelfModelIdentity.BuildEvidenceReference(
                sortedAssertions,
                contradictions);

        return new FoundationSelfModelSnapshot(
            modelId,
            foundationId,
            admittedBaselineId,
            modelTime,
            previousModelIdentity,
            Array.AsReadOnly(sortedAssertions),
            Array.AsReadOnly(contradictions),
            evidenceReference);
    }

    private static FoundationSelfModelContradiction[]
        BuildContradictions(
            IReadOnlyCollection<FoundationSelfModelAssertion> assertions)
    {
        var contradictions =
            new List<FoundationSelfModelContradiction>();

        var groups = assertions
            .Where(assertion =>
                assertion.TemporalView ==
                    FoundationSelfModelTemporalView.Current)
            .GroupBy(assertion => new
            {
                assertion.SubjectId,
                assertion.Area,
                assertion.Scope
            });

        foreach (var group in groups)
        {
            var materialValues = group
                .Select(assertion =>
                    assertion.ValueIdentity)
                .Distinct(StringComparer.Ordinal)
                .ToArray();

            if (materialValues.Length <= 1)
            {
                continue;
            }

            var assertionIds = group
                .Select(assertion =>
                    assertion.AssertionId)
                .OrderBy(
                    value => value,
                    StringComparer.Ordinal)
                .ToArray();

            var contradictionId =
                FoundationSelfModelIdentity.BuildContradictionId(
                    group.Key.SubjectId,
                    group.Key.Area,
                    group.Key.Scope,
                    assertionIds);

            contradictions.Add(
                new FoundationSelfModelContradiction(
                    contradictionId,
                    group.Key.SubjectId,
                    group.Key.Area,
                    group.Key.Scope,
                    Array.AsReadOnly(assertionIds)));
        }

        return contradictions
            .OrderBy(value => (int)value.Area)
            .ThenBy(
                value => value.SubjectId,
                StringComparer.Ordinal)
            .ThenBy(
                value => value.Scope,
                StringComparer.Ordinal)
            .ThenBy(
                value => value.ContradictionId,
                StringComparer.Ordinal)
            .ToArray();
    }
}

public static class FoundationSelfModelAssertionFactory
{
    public static FoundationSelfModelAssertion FromHealthAssessment(
        string assertionId,
        string authoritativeSourceId,
        string sourceOwner,
        string scope,
        string freshnessReference,
        DateTimeOffset expiry,
        CanonicalHealthAssessment assessment,
        FoundationSelfModelTemporalView temporalView =
            FoundationSelfModelTemporalView.Current,
        string? supersedesAssertionId = null)
    {
        ArgumentNullException.ThrowIfNull(assessment);

        ValidateHealthAssessmentForProjection(assessment);

        if (expiry == default ||
            expiry <= assessment.AssessmentTime)
        {
            throw new ArgumentException(
                "Stage 7 Self Model Health assertion expiry rejected",
                nameof(expiry));
        }

        var kind =
            assessment.HealthState == HealthState.Unknown
                ? FoundationSelfModelAssertionKind.Unknown
                : FoundationSelfModelAssertionKind.Interpretation;

        return new FoundationSelfModelAssertion(
            assertionId,
            assessment.SubjectId,
            FoundationSelfModelArea.HealthCondition,
            kind,
            temporalView,
            scope,
            "health-state:" +
                assessment.HealthState
                    .ToString()
                    .ToLowerInvariant(),
            authoritativeSourceId,
            sourceOwner,
            assessment.EvidenceReference,
            assessment.EvidenceQuality,
            assessment.Confidence,
            "reason=" +
                assessment.ReasonCode +
                ";contradictions=" +
                assessment.Contradictions +
                ";blindspots=" +
                assessment.BlindSpots,
            freshnessReference,
            assessment.RuleId,
            assessment.RuleVersion,
            assessment.ObservationTime,
            assessment.AssessmentTime,
            expiry,
            assessment.Identity,
            supersedesAssertionId);
    }

    private static void ValidateHealthAssessmentForProjection(
        CanonicalHealthAssessment assessment)
    {
        if (!Enum.IsDefined(assessment.HealthState) ||
            !Enum.IsDefined(assessment.EvidenceQuality) ||
            !Enum.IsDefined(assessment.ConsequenceClass))
        {
            throw new ArgumentException(
                "Stage 7 Self Model Health assessment enum rejected",
                nameof(assessment));
        }

        if (!HealthFitnessContractV12.IsCanonicalIdentifier(
                assessment.AssessmentId) ||
            !HealthFitnessContractV12.IsCanonicalIdentifier(
                assessment.SubjectId) ||
            !HealthFitnessContractV12.IsCanonicalIdentifier(
                assessment.Capability) ||
            !HealthFitnessContractV12.IsCanonicalIdentifier(
                assessment.EvidenceReference) ||
            !HealthFitnessContractV12.IsCanonicalIdentifier(
                assessment.RuleId) ||
            !HealthFitnessContractV12.IsCanonicalIdentifier(
                assessment.RuleVersion) ||
            !HealthFitnessContractV12.IsCanonicalIdentifier(
                assessment.ReducedByDependencyId))
        {
            throw new ArgumentException(
                "Stage 7 Self Model Health assessment canonical identity rejected",
                nameof(assessment));
        }

        if (string.IsNullOrWhiteSpace(assessment.Confidence) ||
            string.IsNullOrWhiteSpace(assessment.Contradictions) ||
            string.IsNullOrWhiteSpace(assessment.BlindSpots) ||
            string.IsNullOrWhiteSpace(assessment.ReasonCode))
        {
            throw new ArgumentException(
                "Stage 7 Self Model Health assessment detail missing",
                nameof(assessment));
        }

        if (assessment.ObservationTime == default ||
            assessment.AssessmentTime == default ||
            assessment.ObservationTime >
                assessment.AssessmentTime)
        {
            throw new ArgumentException(
                "Stage 7 Self Model Health assessment time order rejected",
                nameof(assessment));
        }
    }
}

internal static class FoundationSelfModelIdentity
{
    public static string ComputeAssertion(
        FoundationSelfModelAssertion value)
    {
        return HashFields(
            new string?[]
            {
                "foundation-self-model-assertion-v2",
                value.AssertionId,
                value.SubjectId,
                ((int)value.Area).ToString(
                    CultureInfo.InvariantCulture),
                ((int)value.AssertionKind).ToString(
                    CultureInfo.InvariantCulture),
                ((int)value.TemporalView).ToString(
                    CultureInfo.InvariantCulture),
                value.Scope,
                value.ValueIdentity,
                value.AuthoritativeSourceId,
                value.SourceOwner,
                value.EvidenceReference,
                ((int)value.EvidenceQuality).ToString(
                    CultureInfo.InvariantCulture),
                value.Confidence,
                value.Uncertainty,
                value.FreshnessReference,
                value.RuleId,
                value.RuleVersion,
                CanonicalTime(value.ObservationTime),
                CanonicalTime(value.EffectiveTime),
                CanonicalTime(value.Expiry),
                value.SourceAssessmentReference,
                value.SupersedesAssertionId
            });
    }

    public static string BuildContradictionId(
        string subjectId,
        FoundationSelfModelArea area,
        string scope,
        IEnumerable<string> assertionIds)
    {
        var fields = new List<string?>
        {
            "foundation-self-model-contradiction-v2",
            subjectId,
            ((int)area).ToString(
                CultureInfo.InvariantCulture),
            scope
        };

        fields.AddRange(
            assertionIds
                .OrderBy(
                    value => value,
                    StringComparer.Ordinal));

        return "selfmodel:contradiction:sha256:" +
            HashFields(fields);
    }

    public static string BuildEvidenceReference(
        IEnumerable<FoundationSelfModelAssertion> assertions,
        IEnumerable<FoundationSelfModelContradiction> contradictions)
    {
        var fields = new List<string?>
        {
            "foundation-self-model-evidence-v2"
        };

        fields.AddRange(
            assertions.Select(
                assertion => assertion.Identity));

        fields.AddRange(
            contradictions.Select(
                contradiction =>
                    contradiction.ContradictionId));

        return "selfmodel:evidence:sha256:" +
            HashFields(fields);
    }

    public static string ComputeSnapshot(
        FoundationSelfModelSnapshot value)
    {
        var fields = new List<string?>
        {
            "foundation-self-model-snapshot-v2",
            value.ModelId,
            value.FoundationId,
            value.AdmittedBaselineId,
            CanonicalTime(value.ModelTime),
            value.PreviousModelIdentity,
            value.EvidenceReference
        };

        fields.AddRange(
            value.Assertions.Select(
                assertion => assertion.Identity));

        fields.AddRange(
            value.Contradictions.Select(
                contradiction =>
                    contradiction.ContradictionId));

        return HashFields(fields);
    }

    private static string CanonicalTime(
        DateTimeOffset value)
    {
        return value
            .ToUniversalTime()
            .ToString(
                "O",
                CultureInfo.InvariantCulture);
    }

    private static string HashFields(
        IEnumerable<string?> fields)
    {
        var builder = new StringBuilder();

        foreach (var field in fields)
        {
            AppendLengthPrefixed(
                builder,
                field);
        }

        return Convert.ToHexString(
            SHA256.HashData(
                Encoding.UTF8.GetBytes(
                    builder.ToString())));
    }

    private static void AppendLengthPrefixed(
        StringBuilder builder,
        string? value)
    {
        if (value is null)
        {
            builder.Append("-1:|");
            return;
        }

        builder
            .Append(
                value.Length.ToString(
                    CultureInfo.InvariantCulture))
            .Append(':')
            .Append(value)
            .Append('|');
    }
}