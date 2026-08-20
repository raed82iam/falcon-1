using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Foundation.Contracts;

namespace Foundation.HealthFitness;

public enum HealthEvidenceLossClass
{
    Available = 1,
    Missing = 2,
    Stale = 3,
    Delayed = 4,
    Contradictory = 5,
    Unverifiable = 6,
    Inaccessible = 7,
    Corrupted = 8,
    ProvenanceFailure = 9,
    PartialVisibility = 10
}

public enum HealthEvidenceAcquisitionState
{
    Arrived = 1,
    Pending = 2,
    Expired = 3,
    Unavailable = 4
}

public sealed record HealthEvidenceRelationAssessment(
    string RelationAssessmentId,
    string HealthRequirementId,
    string HealthRuleId,
    string HealthRuleVersion,
    string SubjectId,
    string Capability,
    string Scope,
    HealthEvidenceRole EvidenceRole,
    string SourceId,
    string SourceOwner,
    string EvidenceReference,
    HealthEvidenceAcquisitionState AcquisitionState,
    HealthEvidenceLossClass LossClass,
    EvidenceQuality StatusQuality,
    string Reason,
    DateTimeOffset ObservationTime,
    DateTimeOffset AssessmentTime,
    DateTimeOffset? SourceExpiry,
    string CanonicalHealthAssessmentId,
    string CanonicalHealthAssessmentIdentity)
{
    public string Identity => HealthEvidenceQualityIdentity.ComputeRelation(this);
}

public sealed record HealthEvidenceQualityResult(
    string ResultId,
    string SubjectId,
    string Capability,
    string Scope,
    string HealthRequirementId,
    string RelationIdentity,
    string CanonicalHealthAssessmentId,
    string CanonicalHealthAssessmentIdentity,
    HealthEvidenceLossClass LossClass,
    EvidenceQuality CanonicalHealthQuality,
    EvidenceQuality StatusQuality,
    EvidenceQuality CompetenceQuality,
    EvidenceQuality ChallengeQuality,
    EvidenceQuality EffectiveQuality,
    string Contradiction,
    string Reason,
    DateTimeOffset AssessmentTime)
{
    public string Identity => HealthEvidenceQualityIdentity.ComputeResult(this);
}

public static class HealthEvidenceQualityRuntime
{
    public static HealthEvidenceQualityResult Evaluate(
        string resultId,
        HealthRuleDefinition healthRule,
        CanonicalHealthAssessment canonicalHealth,
        HealthEvidenceRelationAssessment relation,
        EvidenceQuality competenceQuality,
        EvidenceQuality challengeQuality)
    {
        if (!HealthFitnessContractV12.IsCanonicalIdentifier(resultId))
            throw new ArgumentException("Stage 7 WP05 result identity rejected", nameof(resultId));

        var ruleValidation = HealthRuleValidator.Validate(healthRule);
        if (ruleValidation.Result != ValidationResult.Pass)
            throw new ArgumentException(ruleValidation.Message, nameof(healthRule));

        ArgumentNullException.ThrowIfNull(canonicalHealth);
        ArgumentNullException.ThrowIfNull(relation);

        if (!Enum.IsDefined(competenceQuality) || !Enum.IsDefined(challengeQuality))
            throw new ArgumentException("Stage 7 WP05 quality enum rejected");

        ValidateCanonicalHealthBinding(healthRule, canonicalHealth);
        var relationValidation = ValidateRelation(healthRule, canonicalHealth, relation);
        if (relationValidation.Result != ValidationResult.Pass)
            throw new ArgumentException(relationValidation.Message, nameof(relation));

        var required = relation.EvidenceRole is HealthEvidenceRole.RequiredPrimary or HealthEvidenceRole.RequiredIndependent;
        var derivedStatusQuality = DeriveStatusQuality(relation, required);

        if (Strength(relation.StatusQuality) > Strength(derivedStatusQuality))
            throw new ArgumentException("Stage 7 WP05 optimistic relation status quality rejected", nameof(relation));

        var statusQuality = Weaker(relation.StatusQuality, derivedStatusQuality);
        var effective = Weaker(canonicalHealth.EvidenceQuality, statusQuality, competenceQuality, challengeQuality);

        if (Strength(effective) > Strength(canonicalHealth.EvidenceQuality))
            throw new InvalidOperationException("Stage 7 WP05 quality cannot improve canonical Health quality");

        var contradiction = BuildContradiction(canonicalHealth, relation, effective);
        var reason = BuildReason(relation, effective, contradiction);

        return new HealthEvidenceQualityResult(
            resultId,
            relation.SubjectId,
            relation.Capability,
            relation.Scope,
            relation.HealthRequirementId,
            relation.Identity,
            canonicalHealth.AssessmentId,
            canonicalHealth.Identity,
            relation.LossClass,
            canonicalHealth.EvidenceQuality,
            statusQuality,
            competenceQuality,
            challengeQuality,
            effective,
            contradiction,
            reason,
            relation.AssessmentTime);
    }

    public static ValidationOutcome ValidateRelation(
        HealthRuleDefinition healthRule,
        CanonicalHealthAssessment canonicalHealth,
        HealthEvidenceRelationAssessment? relation)
    {
        if (relation is null)
            return ValidationOutcome.Failed("Stage 7 WP05 evidence relation missing");

        if (!Enum.IsDefined(relation.EvidenceRole) ||
            !Enum.IsDefined(relation.AcquisitionState) ||
            !Enum.IsDefined(relation.LossClass) ||
            !Enum.IsDefined(relation.StatusQuality))
            return ValidationOutcome.Failed("Stage 7 WP05 evidence relation enum rejected");

        if (!Id(relation.RelationAssessmentId) ||
            !Id(relation.HealthRequirementId) ||
            !Id(relation.HealthRuleId) ||
            !Id(relation.HealthRuleVersion) ||
            !Id(relation.SubjectId) ||
            !Id(relation.Capability) ||
            !Id(relation.Scope) ||
            !Id(relation.SourceId) ||
            !Id(relation.SourceOwner) ||
            !Id(relation.EvidenceReference) ||
            !Id(relation.CanonicalHealthAssessmentId) ||
            !Id(relation.CanonicalHealthAssessmentIdentity) ||
            string.IsNullOrWhiteSpace(relation.Reason))
            return ValidationOutcome.Failed("Stage 7 WP05 evidence relation identity rejected");

        if (relation.ObservationTime == default || relation.AssessmentTime == default ||
            relation.ObservationTime > relation.AssessmentTime)
            return ValidationOutcome.Failed("Stage 7 WP05 evidence relation time rejected");

        if (relation.SourceExpiry is { } sourceExpiry && sourceExpiry <= relation.ObservationTime)
            return ValidationOutcome.Failed("Stage 7 WP05 evidence relation source expiry rejected");

        if (!string.Equals(relation.HealthRuleId, healthRule.RuleId, StringComparison.Ordinal) ||
            !string.Equals(relation.HealthRuleVersion, healthRule.RuleVersion, StringComparison.Ordinal) ||
            !string.Equals(relation.SubjectId, healthRule.SubjectId, StringComparison.Ordinal) ||
            !string.Equals(relation.Capability, healthRule.Capability, StringComparison.Ordinal))
            return ValidationOutcome.Failed("Stage 7 WP05 Health rule binding rejected");

        if (!string.Equals(relation.CanonicalHealthAssessmentId, canonicalHealth.AssessmentId, StringComparison.Ordinal) ||
            !string.Equals(relation.CanonicalHealthAssessmentIdentity, canonicalHealth.Identity, StringComparison.Ordinal))
            return ValidationOutcome.Failed("Stage 7 WP05 canonical Health assessment binding rejected");

        var matches = healthRule.EvidenceRequirements
            .Where(requirement => string.Equals(requirement.RequirementId, relation.HealthRequirementId, StringComparison.Ordinal))
            .ToArray();

        if (matches.Length != 1)
            return ValidationOutcome.Failed("Stage 7 WP05 Health requirement binding rejected");

        var declared = matches[0];
        if (declared.Role != relation.EvidenceRole ||
            !string.Equals(declared.SourceId, relation.SourceId, StringComparison.Ordinal) ||
            !string.Equals(declared.SourceOwner, relation.SourceOwner, StringComparison.Ordinal))
            return ValidationOutcome.Failed("Stage 7 WP05 Health requirement source binding rejected");

        if (relation.LossClass == HealthEvidenceLossClass.Delayed &&
            relation.AcquisitionState != HealthEvidenceAcquisitionState.Pending)
            return ValidationOutcome.Failed("Stage 7 WP05 delayed evidence requires pending acquisition");

        if (relation.AcquisitionState == HealthEvidenceAcquisitionState.Pending &&
            relation.LossClass != HealthEvidenceLossClass.Delayed)
            return ValidationOutcome.Failed("Stage 7 WP05 pending acquisition must be delayed");

        if (relation.LossClass == HealthEvidenceLossClass.Available &&
            relation.AcquisitionState != HealthEvidenceAcquisitionState.Arrived)
            return ValidationOutcome.Failed("Stage 7 WP05 available evidence must have arrived");

        if (relation.LossClass == HealthEvidenceLossClass.Stale)
        {
            if (relation.SourceExpiry is not { } staleExpiry)
                return ValidationOutcome.Failed("Stage 7 WP05 stale evidence requires expiry evidence");

            if (staleExpiry > relation.AssessmentTime)
                return ValidationOutcome.Failed("Stage 7 WP05 stale evidence has not expired");
        }

        if (relation.ObservationTime > canonicalHealth.AssessmentTime ||
            relation.AssessmentTime < canonicalHealth.AssessmentTime)
            return ValidationOutcome.Failed("Stage 7 WP05 relation/canonical Health time binding rejected");

        return ValidationOutcome.Passed("Stage 7 WP05 evidence relation valid");
    }

    private static void ValidateCanonicalHealthBinding(
        HealthRuleDefinition healthRule,
        CanonicalHealthAssessment canonicalHealth)
    {
        if (!string.Equals(canonicalHealth.RuleId, healthRule.RuleId, StringComparison.Ordinal) ||
            !string.Equals(canonicalHealth.RuleVersion, healthRule.RuleVersion, StringComparison.Ordinal) ||
            !string.Equals(canonicalHealth.SubjectId, healthRule.SubjectId, StringComparison.Ordinal) ||
            !string.Equals(canonicalHealth.Capability, healthRule.Capability, StringComparison.Ordinal))
            throw new ArgumentException("Stage 7 WP05 canonical Health rule binding rejected", nameof(canonicalHealth));

        if (!Id(canonicalHealth.AssessmentId) || !Id(canonicalHealth.Identity) ||
            !Enum.IsDefined(canonicalHealth.EvidenceQuality))
            throw new ArgumentException("Stage 7 WP05 canonical Health assessment rejected", nameof(canonicalHealth));
    }

    private static EvidenceQuality DeriveStatusQuality(HealthEvidenceRelationAssessment relation, bool required)
    {
        return relation.LossClass switch
        {
            HealthEvidenceLossClass.Available => EvidenceQuality.Sufficient,
            HealthEvidenceLossClass.Missing => required ? EvidenceQuality.Insufficient : EvidenceQuality.Limited,
            HealthEvidenceLossClass.Stale => required ? EvidenceQuality.Insufficient : EvidenceQuality.Limited,
            HealthEvidenceLossClass.Delayed => required ? EvidenceQuality.Insufficient : EvidenceQuality.Limited,
            HealthEvidenceLossClass.Contradictory => required ? EvidenceQuality.Insufficient : EvidenceQuality.Limited,
            HealthEvidenceLossClass.Unverifiable => required ? EvidenceQuality.Insufficient : EvidenceQuality.Limited,
            HealthEvidenceLossClass.Inaccessible => required ? EvidenceQuality.Insufficient : EvidenceQuality.Limited,
            HealthEvidenceLossClass.Corrupted => required ? EvidenceQuality.Invalid : EvidenceQuality.Limited,
            HealthEvidenceLossClass.ProvenanceFailure => required ? EvidenceQuality.Invalid : EvidenceQuality.Limited,
            HealthEvidenceLossClass.PartialVisibility => required ? EvidenceQuality.Insufficient : EvidenceQuality.Limited,
            _ => EvidenceQuality.Invalid
        };
    }

    private static string BuildContradiction(
        CanonicalHealthAssessment health,
        HealthEvidenceRelationAssessment relation,
        EvidenceQuality effective)
    {
        var contradictions = new List<string>();
        if (!string.Equals(health.Contradictions, "NONE", StringComparison.OrdinalIgnoreCase))
            contradictions.Add(health.Contradictions);
        if (relation.LossClass == HealthEvidenceLossClass.Contradictory)
            contradictions.Add("WP05_RELATION_CONTRADICTORY:" + relation.HealthRequirementId);
        if (relation.LossClass != HealthEvidenceLossClass.Available &&
            health.HealthState == HealthState.Healthy &&
            relation.EvidenceRole is HealthEvidenceRole.RequiredPrimary or HealthEvidenceRole.RequiredIndependent)
            contradictions.Add("WP05_REQUIRED_LOSS_VS_HEALTHY:" + relation.HealthRequirementId);
        if (Strength(effective) < Strength(health.EvidenceQuality))
            contradictions.Add("WP05_QUALITY_REDUCED_FROM_CANONICAL_HEALTH");
        return contradictions.Count == 0
            ? "NONE"
            : string.Join(",", contradictions.Distinct(StringComparer.Ordinal).OrderBy(value => value, StringComparer.Ordinal));
    }

    private static string BuildReason(
        HealthEvidenceRelationAssessment relation,
        EvidenceQuality effective,
        string contradiction)
    {
        return string.Join("|", new[]
        {
            "LOSS=" + relation.LossClass.ToString().ToUpperInvariant(),
            "ACQUISITION=" + relation.AcquisitionState.ToString().ToUpperInvariant(),
            "EFFECTIVE_QUALITY=" + effective.ToString().ToUpperInvariant(),
            "CONTRADICTION=" + contradiction,
            "DETAIL=" + relation.Reason.Trim()
        });
    }

    public static EvidenceQuality Weaker(params EvidenceQuality[] values)
    {
        if (values is null || values.Length == 0 || values.Any(value => !Enum.IsDefined(value)))
            throw new ArgumentException("Stage 7 WP05 quality set rejected", nameof(values));
        return values.OrderBy(Strength).First();
    }

    private static int Strength(EvidenceQuality quality) => quality switch
    {
        EvidenceQuality.Invalid => 0,
        EvidenceQuality.Insufficient => 1,
        EvidenceQuality.Limited => 2,
        EvidenceQuality.Sufficient => 3,
        _ => -1
    };

    private static bool Id(string value) => HealthFitnessContractV12.IsCanonicalIdentifier(value);
}

public static class HealthEvidenceQualityIdentity
{
    public static string ComputeRelation(HealthEvidenceRelationAssessment value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return Hash(string.Join("\n", new[]
        {
            value.RelationAssessmentId, value.HealthRequirementId, value.HealthRuleId, value.HealthRuleVersion,
            value.SubjectId, value.Capability, value.Scope, value.EvidenceRole.ToString(), value.SourceId,
            value.SourceOwner, value.EvidenceReference, value.AcquisitionState.ToString(), value.LossClass.ToString(),
            value.StatusQuality.ToString(), value.Reason, value.ObservationTime.ToUniversalTime().ToString("O"),
            value.AssessmentTime.ToUniversalTime().ToString("O"), value.SourceExpiry?.ToUniversalTime().ToString("O") ?? "NONE",
            value.CanonicalHealthAssessmentId, value.CanonicalHealthAssessmentIdentity
        }));
    }

    public static string ComputeResult(HealthEvidenceQualityResult value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return Hash(string.Join("\n", new[]
        {
            value.ResultId, value.SubjectId, value.Capability, value.Scope, value.HealthRequirementId,
            value.RelationIdentity, value.CanonicalHealthAssessmentId, value.CanonicalHealthAssessmentIdentity,
            value.LossClass.ToString(), value.CanonicalHealthQuality.ToString(), value.StatusQuality.ToString(),
            value.CompetenceQuality.ToString(), value.ChallengeQuality.ToString(), value.EffectiveQuality.ToString(),
            value.Contradiction, value.Reason, value.AssessmentTime.ToUniversalTime().ToString("O")
        }));
    }

    private static string Hash(string canonical)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(canonical));
        return "wp05:" + Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
