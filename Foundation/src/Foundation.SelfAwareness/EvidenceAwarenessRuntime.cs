using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Foundation.Contracts;
using Foundation.HealthFitness;

namespace Foundation.SelfAwareness;

public enum EvidenceDriftDomain
{
    Data = 1,
    FoundationModel = 2,
    Behavior = 3,
    Configuration = 4,
    Authority = 5,
    ObjectivePurposeIdentity = 6,
    Dependency = 7,
    OwnAssessment = 8
}

public enum DriftApplicability
{
    Applicable = 1,
    NotApplicable = 2
}

public enum DriftFindingState
{
    NoMaterialDrift = 1,
    MaterialDriftDetected = 2,
    Unknown = 3
}

public enum AuthorityImpactClass
{
    NoneDeclared = 1,
    PositiveInferenceBlocked = 2,
    RequiresGovernedReassessment = 3
}

public enum ChallengeResult
{
    Confirmed = 1,
    Contradicted = 2,
    Inconclusive = 3,
    ReassessmentRequired = 4
}

public enum SourceAuthenticityState
{
    PendingWp06 = 1,
    Verified = 2,
    Failed = 3
}

public enum RestorationGateState
{
    NoPriorLoss = 1,
    LossActive = 2,
    SourceReappearedPendingIndependentReassessment = 3,
    IndependentlyReassessed = 4
}

public sealed record DriftCoverageDeclaration(
    string DeclarationId,
    string RuleId,
    string RuleVersion,
    string GoverningAuthority,
    string EvaluatorId,
    string SubjectId,
    string Capability,
    string Scope,
    EvidenceDriftDomain Domain,
    DriftApplicability Applicability,
    string ComparisonBasisId,
    string EvidenceReference,
    string Reason,
    DateTimeOffset EffectiveTime,
    DateTimeOffset Expiry);

public sealed record CompetenceDeclaration(
    string DeclarationId,
    string EvaluatorId,
    string EvaluatorOwner,
    EvidenceDriftDomain Domain,
    FoundationSelfModelArea Area,
    string SubjectId,
    string Scope,
    string EvidenceReference,
    string EvidenceSource,
    string EvidenceOwner,
    string RuleId,
    string RuleVersion,
    string GoverningAuthority,
    DateTimeOffset EffectiveTime,
    DateTimeOffset Expiry);

public sealed record DriftFinding(
    string FindingId,
    string CoverageDeclarationId,
    EvidenceDriftDomain Domain,
    string SubjectId,
    string Scope,
    DriftFindingState State,
    string ObservedIdentity,
    string ReferenceIdentity,
    string EvidenceReference,
    string Reason,
    DateTimeOffset ObservationTime,
    DateTimeOffset AssessmentTime);

public sealed record IndependentChallengeRecord(
    string ChallengeId,
    string ChallengedRelationIdentity,
    string ChallengedSourceOwner,
    string ChallengerId,
    string ChallengerOwner,
    string AuthorizationEvidenceReference,
    string IndependentEvidenceReference,
    SourceAuthenticityState SourceAuthenticity,
    ChallengeResult Result,
    string Reason,
    DateTimeOffset ObservationTime,
    DateTimeOffset AssessmentTime,
    DateTimeOffset Expiry)
{
    public string Identity => EvidenceAwarenessIdentity.ComputeChallenge(this);
}

public sealed record KnownBlindSpot(
    string BlindSpotId,
    string SubjectId,
    string Capability,
    string Scope,
    EvidenceDriftDomain Domain,
    string Reason,
    string EvidenceReference,
    string AffectedAuthorityContext,
    AuthorityImpactClass AuthorityImpact,
    string GoverningBasis,
    DateTimeOffset ObservationTime,
    DateTimeOffset AssessmentTime,
    DateTimeOffset Expiry)
{
    public string Identity => EvidenceAwarenessIdentity.ComputeBlindSpot(this);
}

public sealed record LastKnownRelianceAssessment(
    string AssessmentId,
    string AssertionId,
    string PolicyReference,
    string SourceId,
    string SourceOwner,
    string EvidenceReference,
    DateTimeOffset OriginalObservationTime,
    DateTimeOffset EffectiveTime,
    DateTimeOffset Expiry,
    DateTimeOffset AssessmentTime,
    bool Eligible,
    string Reason);

public sealed record RestorationAssessment(
    string AssessmentId,
    string HealthRequirementId,
    string RelationIdentity,
    RestorationGateState State,
    string CurrentHealthAssessmentIdentity,
    string ChallengeIdentity,
    string Reason,
    DateTimeOffset AssessmentTime)
{
    public string Identity => EvidenceAwarenessIdentity.ComputeRestoration(this);
}

public sealed record EvidenceAwarenessEvaluation(
    IReadOnlyList<DriftCoverageDeclaration> DriftCoverage,
    IReadOnlyList<DriftFinding> DriftFindings,
    IReadOnlyList<KnownBlindSpot> BlindSpots,
    EvidenceQuality CompetenceQuality,
    EvidenceQuality ChallengeQuality);

public static class EvidenceAwarenessRuntime
{
    private static readonly EvidenceDriftDomain[] RequiredDomains = Enum.GetValues<EvidenceDriftDomain>();

    public static EvidenceAwarenessEvaluation Evaluate(
        string subjectId,
        string capability,
        string scope,
        IReadOnlyCollection<DriftCoverageDeclaration> coverage,
        IReadOnlyCollection<CompetenceDeclaration> competence,
        IReadOnlyCollection<DriftFinding> findings,
        IndependentChallengeRecord? challenge,
        DateTimeOffset assessmentTime)
    {
        if (!Id(subjectId) || !Id(capability) || !Id(scope) || assessmentTime == default)
            throw new ArgumentException("Stage 7 WP05 awareness identity/time rejected");

        ArgumentNullException.ThrowIfNull(coverage);
        ArgumentNullException.ThrowIfNull(competence);
        ArgumentNullException.ThrowIfNull(findings);

        var blindSpots = new List<KnownBlindSpot>();
        var coverageList = coverage.OrderBy(x => x.Domain).ThenBy(x => x.DeclarationId, StringComparer.Ordinal).ToArray();

        foreach (var domain in RequiredDomains)
        {
            var declarations = coverageList.Where(x => x.Domain == domain).ToArray();
            if (declarations.Length != 1)
            {
                blindSpots.Add(BuildBlindSpot(subjectId, capability, scope, domain, "DRIFT_COVERAGE_MISSING_OR_DUPLICATED", assessmentTime));
                continue;
            }

            var declaration = declarations[0];
            ValidateCoverage(declaration, subjectId, capability, scope, assessmentTime);

            if (declaration.Applicability == DriftApplicability.Applicable)
            {
                var competenceMatches = competence.Where(x =>
                    x.Domain == domain &&
                    string.Equals(x.EvaluatorId, declaration.EvaluatorId, StringComparison.Ordinal) &&
                    string.Equals(x.SubjectId, subjectId, StringComparison.Ordinal) &&
                    string.Equals(x.Scope, scope, StringComparison.Ordinal) &&
                    x.EffectiveTime <= assessmentTime && x.Expiry > assessmentTime).ToArray();

                if (competenceMatches.Length != 1 || !ValidateCompetence(competenceMatches.SingleOrDefault(), declaration, assessmentTime))
                    blindSpots.Add(BuildBlindSpot(subjectId, capability, scope, domain, "COMPETENCE_INSUFFICIENT", assessmentTime));
            }
        }

        foreach (var finding in findings)
        {
            ValidateFinding(finding, subjectId, scope, coverageList, assessmentTime);
            if (finding.State == DriftFindingState.Unknown)
                blindSpots.Add(BuildBlindSpot(subjectId, capability, scope, finding.Domain, "DRIFT_STATE_UNKNOWN", assessmentTime));
        }

        foreach (var blindSpot in blindSpots)
        {
            var validation = ValidateBlindSpot(blindSpot, assessmentTime);
            if (validation.Result != ValidationResult.Pass)
                throw new InvalidOperationException(validation.Message);
        }

        var challengeQuality = EvidenceQuality.Sufficient;
        if (challenge is not null)
        {
            var validation = ValidateChallenge(challenge, assessmentTime);
            if (validation.Result != ValidationResult.Pass)
                challengeQuality = EvidenceQuality.Invalid;
            else if (challenge.Result is ChallengeResult.Contradicted or ChallengeResult.Inconclusive or ChallengeResult.ReassessmentRequired)
                challengeQuality = EvidenceQuality.Insufficient;
        }

        var competenceQuality = blindSpots.Count == 0 ? EvidenceQuality.Sufficient : EvidenceQuality.Insufficient;
        return new EvidenceAwarenessEvaluation(
            coverageList,
            findings.OrderBy(x => x.FindingId, StringComparer.Ordinal).ToArray(),
            blindSpots.OrderBy(x => x.BlindSpotId, StringComparer.Ordinal).ToArray(),
            competenceQuality,
            challengeQuality);
    }

    public static ValidationOutcome ValidateChallenge(IndependentChallengeRecord? challenge, DateTimeOffset assessmentTime)
    {
        if (challenge is null)
            return ValidationOutcome.Failed("Stage 7 WP05 challenge missing");
        if (!Enum.IsDefined(challenge.SourceAuthenticity) || !Enum.IsDefined(challenge.Result))
            return ValidationOutcome.Failed("Stage 7 WP05 challenge enum rejected");
        if (!Id(challenge.ChallengeId) || !Id(challenge.ChallengedRelationIdentity) || !Id(challenge.ChallengedSourceOwner) ||
            !Id(challenge.ChallengerId) || !Id(challenge.ChallengerOwner) || !Id(challenge.AuthorizationEvidenceReference) ||
            !Id(challenge.IndependentEvidenceReference) || string.IsNullOrWhiteSpace(challenge.Reason))
            return ValidationOutcome.Failed("Stage 7 WP05 challenge identity rejected");
        if (string.Equals(challenge.ChallengedSourceOwner, challenge.ChallengerOwner, StringComparison.Ordinal))
            return ValidationOutcome.Failed("Stage 7 WP05 independent challenge owner separation rejected");
        if (string.Equals(challenge.IndependentEvidenceReference, challenge.ChallengedRelationIdentity, StringComparison.Ordinal) ||
            string.Equals(challenge.IndependentEvidenceReference, challenge.ChallengeId, StringComparison.Ordinal))
            return ValidationOutcome.Failed("Stage 7 WP05 circular independent challenge rejected");
        if (challenge.ObservationTime == default || challenge.AssessmentTime == default || challenge.Expiry == default ||
            challenge.ObservationTime > challenge.AssessmentTime || challenge.AssessmentTime > assessmentTime || challenge.Expiry <= assessmentTime)
            return ValidationOutcome.Failed("Stage 7 WP05 challenge time rejected");
        if (challenge.SourceAuthenticity == SourceAuthenticityState.Failed)
            return ValidationOutcome.Failed("Stage 7 WP05 challenge source authenticity failed");
        return ValidationOutcome.Passed("Stage 7 WP05 challenge valid");
    }

    public static ValidationOutcome ValidateBlindSpot(KnownBlindSpot? blindSpot, DateTimeOffset assessmentTime)
    {
        if (blindSpot is null)
            return ValidationOutcome.Failed("Stage 7 WP05 blind spot missing");
        if (!Enum.IsDefined(blindSpot.Domain) || !Enum.IsDefined(blindSpot.AuthorityImpact))
            return ValidationOutcome.Failed("Stage 7 WP05 blind spot enum rejected");
        if (!Id(blindSpot.BlindSpotId) || !Id(blindSpot.SubjectId) || !Id(blindSpot.Capability) ||
            !Id(blindSpot.Scope) || !Id(blindSpot.EvidenceReference) || !Id(blindSpot.AffectedAuthorityContext) ||
            !Id(blindSpot.GoverningBasis) || string.IsNullOrWhiteSpace(blindSpot.Reason))
            return ValidationOutcome.Failed("Stage 7 WP05 blind spot identity/governing basis rejected");
        if (blindSpot.ObservationTime == default || blindSpot.AssessmentTime == default || blindSpot.Expiry == default ||
            blindSpot.ObservationTime > blindSpot.AssessmentTime || blindSpot.AssessmentTime > assessmentTime ||
            blindSpot.Expiry <= assessmentTime)
            return ValidationOutcome.Failed("Stage 7 WP05 blind spot time rejected");
        if (blindSpot.AuthorityImpact == AuthorityImpactClass.NoneDeclared && !Id(blindSpot.GoverningBasis))
            return ValidationOutcome.Failed("Stage 7 WP05 NONE_DECLARED requires explicit governing basis");
        return ValidationOutcome.Passed("Stage 7 WP05 blind spot valid");
    }

    public static LastKnownRelianceAssessment EvaluateLastKnownReliance(
        string assessmentId,
        FoundationSelfModelAssertion assertion,
        string policyReference,
        DateTimeOffset assessmentTime)
    {
        ArgumentNullException.ThrowIfNull(assertion);
        if (!Id(assessmentId) || !Id(policyReference) || assessmentTime == default)
            throw new ArgumentException("Stage 7 WP05 LastKnown identity/time rejected");

        var eligible = assertion.TemporalView == FoundationSelfModelTemporalView.LastKnown &&
                       assertion.ObservationTime <= assessmentTime &&
                       assertion.Expiry > assessmentTime &&
                       assertion.EvidenceQuality is EvidenceQuality.Sufficient or EvidenceQuality.Limited &&
                       Id(assertion.AuthoritativeSourceId) && Id(assertion.SourceOwner) && Id(assertion.EvidenceReference);

        return new LastKnownRelianceAssessment(
            assessmentId, assertion.AssertionId, policyReference, assertion.AuthoritativeSourceId, assertion.SourceOwner,
            assertion.EvidenceReference, assertion.ObservationTime, assertion.EffectiveTime, assertion.Expiry,
            assessmentTime, eligible, eligible ? "LAST_KNOWN_POLICY_ELIGIBLE" : "LAST_KNOWN_POLICY_INELIGIBLE");
    }

    public static RestorationAssessment EvaluateRestoration(
        string assessmentId,
        HealthEvidenceRelationAssessment relation,
        HealthEvidenceQualityResult quality,
        CanonicalHealthAssessment canonicalHealth,
        IndependentChallengeRecord? challenge,
        EvidenceQuality competenceQuality,
        DateTimeOffset restoredObservationTime,
        DateTimeOffset assessmentTime)
    {
        if (!Id(assessmentId) || relation is null || quality is null || canonicalHealth is null ||
            !Enum.IsDefined(competenceQuality) || restoredObservationTime == default || assessmentTime == default)
            throw new ArgumentException("Stage 7 WP05 restoration input rejected");

        if (!string.Equals(quality.HealthRequirementId, relation.HealthRequirementId, StringComparison.Ordinal) ||
            !string.Equals(quality.RelationIdentity, relation.Identity, StringComparison.Ordinal) ||
            !string.Equals(quality.CanonicalHealthAssessmentIdentity, canonicalHealth.Identity, StringComparison.Ordinal))
            throw new ArgumentException("Stage 7 WP05 restoration binding rejected");

        if (relation.LossClass != HealthEvidenceLossClass.Available)
            return BuildRestoration(assessmentId, relation, canonicalHealth, challenge, RestorationGateState.LossActive, "REQUIRED_LOSS_ACTIVE", assessmentTime);

        if (challenge is null)
            return BuildRestoration(assessmentId, relation, canonicalHealth, null, RestorationGateState.SourceReappearedPendingIndependentReassessment, "INDEPENDENT_REASSESSMENT_MISSING", assessmentTime);

        var challengeValidation = ValidateChallenge(challenge, assessmentTime);
        var canRestore = canonicalHealth.EvidenceQuality == EvidenceQuality.Sufficient &&
                         quality.StatusQuality == EvidenceQuality.Sufficient &&
                         quality.EffectiveQuality == EvidenceQuality.Sufficient &&
                         competenceQuality == EvidenceQuality.Sufficient &&
                         challengeValidation.Result == ValidationResult.Pass &&
                         challenge.SourceAuthenticity == SourceAuthenticityState.Verified &&
                         challenge.Result == ChallengeResult.Confirmed &&
                         challenge.ObservationTime > restoredObservationTime &&
                         string.Equals(challenge.ChallengedRelationIdentity, relation.Identity, StringComparison.Ordinal);

        return BuildRestoration(
            assessmentId,
            relation,
            canonicalHealth,
            challenge,
            canRestore ? RestorationGateState.IndependentlyReassessed : RestorationGateState.SourceReappearedPendingIndependentReassessment,
            canRestore ? "INDEPENDENT_REASSESSMENT_SATISFIED" : "RESTORATION_GATE_PENDING",
            assessmentTime);
    }

    private static RestorationAssessment BuildRestoration(
        string assessmentId,
        HealthEvidenceRelationAssessment relation,
        CanonicalHealthAssessment canonicalHealth,
        IndependentChallengeRecord? challenge,
        RestorationGateState state,
        string reason,
        DateTimeOffset assessmentTime)
    {
        return new RestorationAssessment(
            assessmentId, relation.HealthRequirementId, relation.Identity, state, canonicalHealth.Identity,
            challenge?.Identity ?? "NONE", reason, assessmentTime);
    }

    private static void ValidateCoverage(
        DriftCoverageDeclaration declaration,
        string subjectId,
        string capability,
        string scope,
        DateTimeOffset assessmentTime)
    {
        if (!Enum.IsDefined(declaration.Domain) || !Enum.IsDefined(declaration.Applicability) ||
            !Id(declaration.DeclarationId) || !Id(declaration.RuleId) || !Id(declaration.RuleVersion) ||
            !Id(declaration.GoverningAuthority) || !Id(declaration.EvaluatorId) || !Id(declaration.SubjectId) ||
            !Id(declaration.Capability) || !Id(declaration.Scope) || !Id(declaration.EvidenceReference) ||
            string.IsNullOrWhiteSpace(declaration.Reason) || declaration.EffectiveTime == default ||
            declaration.Expiry == default || declaration.EffectiveTime > assessmentTime || declaration.Expiry <= assessmentTime)
            throw new ArgumentException("Stage 7 WP05 drift coverage rejected");

        if (!string.Equals(declaration.SubjectId, subjectId, StringComparison.Ordinal) ||
            !string.Equals(declaration.Capability, capability, StringComparison.Ordinal) ||
            !string.Equals(declaration.Scope, scope, StringComparison.Ordinal))
            throw new ArgumentException("Stage 7 WP05 drift coverage scope rejected");

        if (declaration.Applicability == DriftApplicability.Applicable && !Id(declaration.ComparisonBasisId))
            throw new ArgumentException("Stage 7 WP05 applicable drift comparison basis missing");

        if (declaration.Applicability == DriftApplicability.NotApplicable &&
            (!Id(declaration.GoverningAuthority) || !Id(declaration.EvidenceReference) || string.IsNullOrWhiteSpace(declaration.Reason)))
            throw new ArgumentException("Stage 7 WP05 non-applicable drift governance rejected");
    }

    private static void ValidateFinding(
        DriftFinding finding,
        string subjectId,
        string scope,
        IReadOnlyCollection<DriftCoverageDeclaration> coverage,
        DateTimeOffset assessmentTime)
    {
        if (!Enum.IsDefined(finding.Domain) || !Enum.IsDefined(finding.State) ||
            !Id(finding.FindingId) || !Id(finding.CoverageDeclarationId) || !Id(finding.SubjectId) ||
            !Id(finding.Scope) || !Id(finding.ObservedIdentity) || !Id(finding.ReferenceIdentity) ||
            !Id(finding.EvidenceReference) || string.IsNullOrWhiteSpace(finding.Reason) ||
            finding.ObservationTime == default || finding.AssessmentTime == default ||
            finding.ObservationTime > finding.AssessmentTime || finding.AssessmentTime > assessmentTime)
            throw new ArgumentException("Stage 7 WP05 drift finding rejected");

        if (!string.Equals(finding.SubjectId, subjectId, StringComparison.Ordinal) ||
            !string.Equals(finding.Scope, scope, StringComparison.Ordinal))
            throw new ArgumentException("Stage 7 WP05 drift finding scope rejected");

        if (!coverage.Any(x => x.Domain == finding.Domain && string.Equals(x.DeclarationId, finding.CoverageDeclarationId, StringComparison.Ordinal)))
            throw new ArgumentException("Stage 7 WP05 drift finding coverage binding rejected");
    }

    private static bool ValidateCompetence(
        CompetenceDeclaration? competence,
        DriftCoverageDeclaration declaration,
        DateTimeOffset assessmentTime)
    {
        if (competence is null || !Enum.IsDefined(competence.Domain) || !Enum.IsDefined(competence.Area))
            return false;
        if (!Id(competence.DeclarationId) || !Id(competence.EvaluatorId) || !Id(competence.EvaluatorOwner) ||
            !Id(competence.SubjectId) || !Id(competence.Scope) || !Id(competence.EvidenceReference) ||
            !Id(competence.EvidenceSource) || !Id(competence.EvidenceOwner) || !Id(competence.RuleId) ||
            !Id(competence.RuleVersion) || !Id(competence.GoverningAuthority))
            return false;
        if (competence.EffectiveTime == default || competence.Expiry == default ||
            competence.EffectiveTime > assessmentTime || competence.Expiry <= assessmentTime)
            return false;
        if (string.Equals(competence.EvaluatorOwner, competence.EvidenceOwner, StringComparison.Ordinal))
            return false;
        return competence.Domain == declaration.Domain &&
               string.Equals(competence.EvaluatorId, declaration.EvaluatorId, StringComparison.Ordinal) &&
               string.Equals(competence.RuleId, declaration.RuleId, StringComparison.Ordinal) &&
               string.Equals(competence.RuleVersion, declaration.RuleVersion, StringComparison.Ordinal) &&
               string.Equals(competence.GoverningAuthority, declaration.GoverningAuthority, StringComparison.Ordinal);
    }

    private static KnownBlindSpot BuildBlindSpot(
        string subjectId,
        string capability,
        string scope,
        EvidenceDriftDomain domain,
        string reason,
        DateTimeOffset assessmentTime)
    {
        var suffix = domain.ToString().ToLowerInvariant();
        return new KnownBlindSpot(
            "blindspot:" + suffix,
            subjectId,
            capability,
            scope,
            domain,
            reason,
            "evidence:blindspot:" + suffix,
            "authority-context:affected:" + capability,
            AuthorityImpactClass.PositiveInferenceBlocked,
            "awr-001:req-005",
            assessmentTime,
            assessmentTime,
            assessmentTime.AddMinutes(1));
    }

    private static bool Id(string? value) =>
        !string.IsNullOrWhiteSpace(value) && HealthFitnessContractV12.IsCanonicalIdentifier(value);
}

public static class EvidenceAwarenessIdentity
{
    public static string ComputeChallenge(IndependentChallengeRecord value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return Hash(string.Join("\n", new[]
        {
            value.ChallengeId, value.ChallengedRelationIdentity, value.ChallengedSourceOwner,
            value.ChallengerId, value.ChallengerOwner, value.AuthorizationEvidenceReference,
            value.IndependentEvidenceReference, value.SourceAuthenticity.ToString(), value.Result.ToString(),
            value.Reason, value.ObservationTime.ToUniversalTime().ToString("O"),
            value.AssessmentTime.ToUniversalTime().ToString("O"), value.Expiry.ToUniversalTime().ToString("O")
        }));
    }

    public static string ComputeBlindSpot(KnownBlindSpot value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return Hash(string.Join("\n", new[]
        {
            value.BlindSpotId, value.SubjectId, value.Capability, value.Scope, value.Domain.ToString(),
            value.Reason, value.EvidenceReference, value.AffectedAuthorityContext, value.AuthorityImpact.ToString(),
            value.GoverningBasis, value.ObservationTime.ToUniversalTime().ToString("O"),
            value.AssessmentTime.ToUniversalTime().ToString("O"), value.Expiry.ToUniversalTime().ToString("O")
        }));
    }

    public static string ComputeRestoration(RestorationAssessment value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return Hash(string.Join("\n", new[]
        {
            value.AssessmentId, value.HealthRequirementId, value.RelationIdentity, value.State.ToString(),
            value.CurrentHealthAssessmentIdentity, value.ChallengeIdentity, value.Reason,
            value.AssessmentTime.ToUniversalTime().ToString("O")
        }));
    }

    private static string Hash(string canonical)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(canonical));
        return "wp05-awareness:" + Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
