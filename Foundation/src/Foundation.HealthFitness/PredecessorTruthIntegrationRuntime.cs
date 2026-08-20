using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Foundation.Contracts;

namespace Foundation.HealthFitness;

public enum PredecessorTruthDomain
{
    Stage3DependencyConfiguration = 1,
    Stage4AuthorityLifecycleState = 2,
    Stage4EvidenceReconciliation = 3,
    Stage5ContractMessageEventProtection = 4,
    Stage6ResourcePressureIsolationLoadShedding = 5,
    SecurityTrustIdentity = 6,
    LoggingPersistence = 7
}

public enum PredecessorTruthAvailability
{
    Available = 1,
    Missing = 2,
    Inaccessible = 3
}

public enum PredecessorTruthAuthenticityStatus
{
    Verified = 1,
    Unverified = 2,
    Mismatch = 3
}

public enum PredecessorTruthIntegrityStatus
{
    Verified = 1,
    Unverified = 2,
    Corrupted = 3
}

public enum PredecessorTruthProvenanceStatus
{
    Verified = 1,
    Unverified = 2,
    Failed = 3
}

public enum PredecessorTruthOperationalClassification
{
    AuthoritativeCurrent = 1,
    AuthoritativeHistorical = 2,
    Replay = 3,
    Test = 4,
    Simulation = 5,
    NonAuthoritative = 6
}

public sealed record PredecessorTruthSourceDefinition(
    string DefinitionId,
    PredecessorTruthDomain Domain,
    string SourceId,
    string SourceOwner,
    string TruthKind,
    string SchemaId,
    string SchemaVersion,
    string GoverningAuthorityId)
{
    public string Identity => PredecessorTruthIntegrationIdentity.ComputeDefinition(this);
}

public sealed record PredecessorTruthEvidence(
    string EvidenceId,
    PredecessorTruthDomain Domain,
    string SourceId,
    string SourceOwner,
    string TruthKind,
    string SchemaId,
    string SchemaVersion,
    string SubjectId,
    string Capability,
    string Scope,
    string RecordIdentity,
    string RecordVersion,
    string PayloadDigest,
    string EvidenceReference,
    string ProvenanceReference,
    PredecessorTruthProvenanceStatus ProvenanceStatus,
    string IntegrityReference,
    PredecessorTruthIntegrityStatus IntegrityStatus,
    PredecessorTruthAuthenticityStatus AuthenticityStatus,
    PredecessorTruthAvailability Availability,
    PredecessorTruthOperationalClassification OperationalClassification,
    DateTimeOffset ObservationTime,
    DateTimeOffset EffectiveTime,
    DateTimeOffset AssessmentTime,
    DateTimeOffset Expiry,
    string Reason)
{
    public string Identity => PredecessorTruthIntegrationIdentity.ComputeEvidence(this);
}

public sealed record PredecessorTruthIntegrationResult(
    string ResultId,
    PredecessorTruthDomain Domain,
    string SourceDefinitionIdentity,
    string EvidenceIdentity,
    string SourceId,
    string SourceOwner,
    string EvidenceReference,
    PredecessorTruthOperationalClassification OperationalClassification,
    PredecessorTruthAuthenticityStatus AuthenticityStatus,
    PredecessorTruthIntegrityStatus IntegrityStatus,
    PredecessorTruthProvenanceStatus ProvenanceStatus,
    PredecessorTruthAvailability Availability,
    HealthEvidenceLossClass LossClass,
    EvidenceQuality EvidenceQuality,
    bool CanSupportCurrentAwareness,
    string Reason,
    DateTimeOffset AssessmentTime)
{
    public string Identity => PredecessorTruthIntegrationIdentity.ComputeResult(this);
}

public sealed record PredecessorTruthCoverageResult(
    string CoverageId,
    bool CompleteCurrentCoverage,
    EvidenceQuality EvidenceQuality,
    IReadOnlyList<PredecessorTruthDomain> MissingDomains,
    IReadOnlyList<string> ResultIdentities,
    string Reason)
{
    public string Identity => PredecessorTruthIntegrationIdentity.ComputeCoverage(this);
}

public static class PredecessorTruthIntegrationRuntime
{
    public static IReadOnlyList<PredecessorTruthDomain> RequiredDomains { get; } = new[]
    {
        PredecessorTruthDomain.Stage3DependencyConfiguration,
        PredecessorTruthDomain.Stage4AuthorityLifecycleState,
        PredecessorTruthDomain.Stage4EvidenceReconciliation,
        PredecessorTruthDomain.Stage5ContractMessageEventProtection,
        PredecessorTruthDomain.Stage6ResourcePressureIsolationLoadShedding,
        PredecessorTruthDomain.SecurityTrustIdentity,
        PredecessorTruthDomain.LoggingPersistence
    };

    public static ValidationOutcome ValidateDefinition(PredecessorTruthSourceDefinition? definition)
    {
        if (definition is null)
            return ValidationOutcome.Failed("Stage 7 WP06 predecessor source definition missing");

        if (!Enum.IsDefined(definition.Domain))
            return ValidationOutcome.Failed("Stage 7 WP06 predecessor source domain rejected");

        if (!Id(definition.DefinitionId) || !Id(definition.SourceId) || !Id(definition.SourceOwner) ||
            !Id(definition.TruthKind) || !Id(definition.SchemaId) || !Id(definition.SchemaVersion) ||
            !Id(definition.GoverningAuthorityId))
            return ValidationOutcome.Failed("Stage 7 WP06 predecessor source identity rejected");

        return ValidationOutcome.Passed("Stage 7 WP06 predecessor source definition valid");
    }

    public static PredecessorTruthIntegrationResult Evaluate(
        string resultId,
        PredecessorTruthSourceDefinition definition,
        PredecessorTruthEvidence evidence)
    {
        if (!Id(resultId))
            throw new ArgumentException("Stage 7 WP06 integration result identity rejected", nameof(resultId));

        var definitionValidation = ValidateDefinition(definition);
        if (definitionValidation.Result != ValidationResult.Pass)
            throw new ArgumentException(definitionValidation.Message, nameof(definition));

        var evidenceValidation = ValidateEvidence(definition, evidence);
        if (evidenceValidation.Result != ValidationResult.Pass)
            throw new ArgumentException(evidenceValidation.Message, nameof(evidence));

        var stale = evidence.Expiry <= evidence.AssessmentTime;
        var positive =
            evidence.Availability == PredecessorTruthAvailability.Available &&
            evidence.AuthenticityStatus == PredecessorTruthAuthenticityStatus.Verified &&
            evidence.IntegrityStatus == PredecessorTruthIntegrityStatus.Verified &&
            evidence.ProvenanceStatus == PredecessorTruthProvenanceStatus.Verified &&
            evidence.OperationalClassification == PredecessorTruthOperationalClassification.AuthoritativeCurrent &&
            !stale;

        var (loss, quality, reason) = Classify(evidence, stale, positive);

        return new PredecessorTruthIntegrationResult(
            resultId,
            definition.Domain,
            definition.Identity,
            evidence.Identity,
            evidence.SourceId,
            evidence.SourceOwner,
            evidence.EvidenceReference,
            evidence.OperationalClassification,
            evidence.AuthenticityStatus,
            evidence.IntegrityStatus,
            evidence.ProvenanceStatus,
            evidence.Availability,
            loss,
            quality,
            positive,
            reason,
            evidence.AssessmentTime);
    }

    public static ValidationOutcome ValidateEvidence(
        PredecessorTruthSourceDefinition definition,
        PredecessorTruthEvidence? evidence)
    {
        ArgumentNullException.ThrowIfNull(definition);

        if (evidence is null)
            return ValidationOutcome.Failed("Stage 7 WP06 predecessor evidence missing");

        if (!Enum.IsDefined(evidence.Domain) || !Enum.IsDefined(evidence.ProvenanceStatus) ||
            !Enum.IsDefined(evidence.IntegrityStatus) || !Enum.IsDefined(evidence.AuthenticityStatus) ||
            !Enum.IsDefined(evidence.Availability) || !Enum.IsDefined(evidence.OperationalClassification))
            return ValidationOutcome.Failed("Stage 7 WP06 predecessor evidence enum rejected");

        if (!Id(evidence.EvidenceId) || !Id(evidence.SourceId) || !Id(evidence.SourceOwner) ||
            !Id(evidence.TruthKind) || !Id(evidence.SchemaId) || !Id(evidence.SchemaVersion) ||
            !Id(evidence.SubjectId) || !Id(evidence.Capability) || !Id(evidence.Scope) ||
            !Id(evidence.RecordIdentity) || !Id(evidence.RecordVersion) || !Id(evidence.PayloadDigest) ||
            !Id(evidence.EvidenceReference) || !Id(evidence.ProvenanceReference) || !Id(evidence.IntegrityReference) ||
            string.IsNullOrWhiteSpace(evidence.Reason))
            return ValidationOutcome.Failed("Stage 7 WP06 predecessor evidence identity rejected");

        if (evidence.Domain != definition.Domain ||
            !string.Equals(evidence.SourceId, definition.SourceId, StringComparison.Ordinal) ||
            !string.Equals(evidence.SourceOwner, definition.SourceOwner, StringComparison.Ordinal) ||
            !string.Equals(evidence.TruthKind, definition.TruthKind, StringComparison.Ordinal) ||
            !string.Equals(evidence.SchemaId, definition.SchemaId, StringComparison.Ordinal) ||
            !string.Equals(evidence.SchemaVersion, definition.SchemaVersion, StringComparison.Ordinal))
            return ValidationOutcome.Failed("Stage 7 WP06 predecessor definition/evidence binding rejected");

        if (evidence.ObservationTime == default || evidence.EffectiveTime == default ||
            evidence.AssessmentTime == default || evidence.Expiry == default ||
            evidence.ObservationTime > evidence.EffectiveTime ||
            evidence.EffectiveTime > evidence.AssessmentTime ||
            evidence.Expiry <= evidence.EffectiveTime)
            return ValidationOutcome.Failed("Stage 7 WP06 predecessor evidence time order rejected");

        return ValidationOutcome.Passed("Stage 7 WP06 predecessor evidence valid");
    }

    public static PredecessorTruthCoverageResult EvaluateCoverage(
        string coverageId,
        IEnumerable<PredecessorTruthIntegrationResult> results)
    {
        if (!Id(coverageId))
            throw new ArgumentException("Stage 7 WP06 coverage identity rejected", nameof(coverageId));
        ArgumentNullException.ThrowIfNull(results);

        var materialized = results.ToArray();
        if (materialized.Any(result => result is null))
            throw new ArgumentException("Stage 7 WP06 coverage result missing", nameof(results));

        var duplicate = materialized.GroupBy(result => result.Domain).FirstOrDefault(group => group.Count() > 1);
        if (duplicate is not null)
            throw new ArgumentException("Stage 7 WP06 duplicate predecessor domain rejected: " + duplicate.Key, nameof(results));

        var missing = RequiredDomains.Where(domain => materialized.All(result => result.Domain != domain)).ToArray();
        var ordered = materialized.OrderBy(result => result.Domain).ToArray();
        var complete = missing.Length == 0 && ordered.Length == RequiredDomains.Count && ordered.All(result => result.CanSupportCurrentAwareness);

        var quality = ordered.Any(result => result.EvidenceQuality == EvidenceQuality.Invalid)
            ? EvidenceQuality.Invalid
            : complete
                ? EvidenceQuality.Sufficient
                : EvidenceQuality.Insufficient;

        var reason = complete
            ? "Stage 7 WP06 all accepted predecessor truth domains are qualified for current awareness"
            : missing.Length > 0
                ? "Stage 7 WP06 predecessor truth coverage incomplete: " + string.Join(",", missing)
                : "Stage 7 WP06 predecessor truth coverage contains non-current or insufficient truth";

        return new PredecessorTruthCoverageResult(
            coverageId,
            complete,
            quality,
            missing,
            ordered.Select(result => result.Identity).ToArray(),
            reason);
    }

    public static ValidationOutcome ValidateWp05RelationBinding(
        PredecessorTruthIntegrationResult predecessor,
        HealthEvidenceRelationAssessment? relation)
    {
        ArgumentNullException.ThrowIfNull(predecessor);
        if (relation is null)
            return ValidationOutcome.Failed("Stage 7 WP06 WP05 relation missing");

        if (!string.Equals(predecessor.SourceId, relation.SourceId, StringComparison.Ordinal) ||
            !string.Equals(predecessor.SourceOwner, relation.SourceOwner, StringComparison.Ordinal) ||
            !string.Equals(predecessor.EvidenceReference, relation.EvidenceReference, StringComparison.Ordinal))
            return ValidationOutcome.Failed("Stage 7 WP06 WP05 predecessor binding rejected");

        if (!predecessor.CanSupportCurrentAwareness &&
            (relation.LossClass == HealthEvidenceLossClass.Available || relation.StatusQuality == EvidenceQuality.Sufficient))
            return ValidationOutcome.Failed("Stage 7 WP06 non-current predecessor truth cannot support optimistic WP05 relation");

        if (predecessor.CanSupportCurrentAwareness && relation.LossClass != HealthEvidenceLossClass.Available)
            return ValidationOutcome.Failed("Stage 7 WP06 current predecessor truth requires explicit WP05 available relation");

        return ValidationOutcome.Passed("Stage 7 WP06 WP05 predecessor relation binding valid");
    }

    private static (HealthEvidenceLossClass Loss, EvidenceQuality Quality, string Reason) Classify(
        PredecessorTruthEvidence evidence,
        bool stale,
        bool positive)
    {
        if (positive)
            return (HealthEvidenceLossClass.Available, EvidenceQuality.Sufficient, "Accepted predecessor truth verified for current Stage 7 awareness");

        if (evidence.AuthenticityStatus == PredecessorTruthAuthenticityStatus.Mismatch)
            return (HealthEvidenceLossClass.ProvenanceFailure, EvidenceQuality.Invalid, "Predecessor source authenticity mismatch");
        if (evidence.IntegrityStatus == PredecessorTruthIntegrityStatus.Corrupted)
            return (HealthEvidenceLossClass.Corrupted, EvidenceQuality.Invalid, "Predecessor truth integrity corrupted");
        if (evidence.ProvenanceStatus == PredecessorTruthProvenanceStatus.Failed)
            return (HealthEvidenceLossClass.ProvenanceFailure, EvidenceQuality.Invalid, "Predecessor truth provenance failed");
        if (evidence.Availability == PredecessorTruthAvailability.Missing)
            return (HealthEvidenceLossClass.Missing, EvidenceQuality.Insufficient, "Predecessor truth missing");
        if (evidence.Availability == PredecessorTruthAvailability.Inaccessible)
            return (HealthEvidenceLossClass.Inaccessible, EvidenceQuality.Insufficient, "Predecessor truth inaccessible");
        if (stale)
            return (HealthEvidenceLossClass.Stale, EvidenceQuality.Insufficient, "Predecessor truth expired and cannot support current awareness");
        if (evidence.AuthenticityStatus == PredecessorTruthAuthenticityStatus.Unverified)
            return (HealthEvidenceLossClass.Unverifiable, EvidenceQuality.Insufficient, "Predecessor source authenticity unverified");
        if (evidence.IntegrityStatus == PredecessorTruthIntegrityStatus.Unverified)
            return (HealthEvidenceLossClass.Unverifiable, EvidenceQuality.Insufficient, "Predecessor truth integrity unverified");
        if (evidence.ProvenanceStatus == PredecessorTruthProvenanceStatus.Unverified)
            return (HealthEvidenceLossClass.Unverifiable, EvidenceQuality.Insufficient, "Predecessor truth provenance unverified");

        return evidence.OperationalClassification switch
        {
            PredecessorTruthOperationalClassification.AuthoritativeHistorical =>
                (HealthEvidenceLossClass.Stale, EvidenceQuality.Insufficient, "Historical predecessor truth cannot support current awareness"),
            PredecessorTruthOperationalClassification.Replay =>
                (HealthEvidenceLossClass.Unverifiable, EvidenceQuality.Insufficient, "Replay predecessor truth cannot become current awareness"),
            PredecessorTruthOperationalClassification.Test =>
                (HealthEvidenceLossClass.Unverifiable, EvidenceQuality.Insufficient, "Test predecessor truth cannot become current awareness"),
            PredecessorTruthOperationalClassification.Simulation =>
                (HealthEvidenceLossClass.Unverifiable, EvidenceQuality.Insufficient, "Simulation predecessor truth cannot become current awareness"),
            PredecessorTruthOperationalClassification.NonAuthoritative =>
                (HealthEvidenceLossClass.Unverifiable, EvidenceQuality.Insufficient, "Non-authoritative predecessor truth cannot become current awareness"),
            _ => (HealthEvidenceLossClass.Unverifiable, EvidenceQuality.Insufficient, "Predecessor truth not qualified for current awareness")
        };
    }

    private static bool Id(string value) => HealthFitnessContractV12.IsCanonicalIdentifier(value);
}

public static class PredecessorTruthIntegrationIdentity
{
    public static string ComputeDefinition(PredecessorTruthSourceDefinition value) => Hash(string.Join("|",
        value.DefinitionId, (int)value.Domain, value.SourceId, value.SourceOwner, value.TruthKind,
        value.SchemaId, value.SchemaVersion, value.GoverningAuthorityId));

    public static string ComputeEvidence(PredecessorTruthEvidence value) => Hash(string.Join("|",
        value.EvidenceId, (int)value.Domain, value.SourceId, value.SourceOwner, value.TruthKind,
        value.SchemaId, value.SchemaVersion, value.SubjectId, value.Capability, value.Scope,
        value.RecordIdentity, value.RecordVersion, value.PayloadDigest, value.EvidenceReference,
        value.ProvenanceReference, (int)value.ProvenanceStatus, value.IntegrityReference,
        (int)value.IntegrityStatus, (int)value.AuthenticityStatus, (int)value.Availability,
        (int)value.OperationalClassification, value.ObservationTime.ToUniversalTime().ToString("O"),
        value.EffectiveTime.ToUniversalTime().ToString("O"), value.AssessmentTime.ToUniversalTime().ToString("O"),
        value.Expiry.ToUniversalTime().ToString("O"), value.Reason));

    public static string ComputeResult(PredecessorTruthIntegrationResult value) => Hash(string.Join("|",
        value.ResultId, (int)value.Domain, value.SourceDefinitionIdentity, value.EvidenceIdentity,
        value.SourceId, value.SourceOwner, value.EvidenceReference, (int)value.OperationalClassification,
        (int)value.AuthenticityStatus, (int)value.IntegrityStatus, (int)value.ProvenanceStatus,
        (int)value.Availability, (int)value.LossClass, (int)value.EvidenceQuality,
        value.CanSupportCurrentAwareness, value.Reason, value.AssessmentTime.ToUniversalTime().ToString("O")));

    public static string ComputeCoverage(PredecessorTruthCoverageResult value) => Hash(string.Join("|",
        value.CoverageId, value.CompleteCurrentCoverage, (int)value.EvidenceQuality,
        string.Join(",", value.MissingDomains.Select(domain => ((int)domain).ToString())),
        string.Join(",", value.ResultIdentities), value.Reason));

    private static string Hash(string canonical)
    {
        var digest = SHA256.HashData(Encoding.UTF8.GetBytes(canonical));
        return Convert.ToHexString(digest);
    }
}
