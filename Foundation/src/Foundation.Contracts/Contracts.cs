using System;
using System.Linq;

namespace Foundation.Contracts;

public static class ContractIdentity
{
    public const string Con001 = "CON-001";
    public const string Con002 = "CON-002";
    public const string Con003 = "CON-003";
    public const string Con004 = "CON-004";
    public const string Con005 = "CON-005";
    public const string Con006 = "CON-006";
    public const string Con007 = "CON-007";
    public const string Con008 = "CON-008";
    public const string Con009 = "CON-009";
    public const string Con010 = "CON-010";
    public const string Con011 = "CON-011";
    public const string Con012 = "CON-012";
    public const string Con013 = "CON-013";
    public const string Con014 = "CON-014";
    public const string Con015 = "CON-015";
    public const string Con016 = "CON-016";
    public const string Con017 = "CON-017";
    public const string Con018 = "CON-018";
    public const string Con019 = "CON-019";
    public const string Con020 = "CON-020";
    public const string Con021 = "CON-021";
}

public sealed record CoreIdentity(
    string SubjectId,
    string SubjectClass,
    string InstanceId,
    string Version,
    string Owner,
    string AdmittedCapability,
    string ArtifactIdentity,
    string AuthorityContext,
    string LifecycleIdentity,
    DateTimeOffset CreationTime,
    string IntegrityEvidence)
{
    public string ContractId => ContractIdentity.Con001;
}

public sealed record AuthorityRequest(
    string RequestId,
    string ActorIdentity,
    string Action,
    string Resource,
    string Purpose,
    string RequestedScope,
    string OperatingContext,
    string SecurityContext,
    string RequiredFitnessToOperate,
    string Correlation,
    DateTimeOffset RequestTime,
    DateTimeOffset Expiry)
{
    public string ContractId => ContractIdentity.Con002;
}

public sealed record AuthorityResult(
    string RequestId,
    string DecisionId,
    string Decision,
    string EffectiveScope,
    string ControllingPolicy,
    string PolicyVersion,
    string MaterialConditions,
    string Constraints,
    string Reason,
    DateTimeOffset DecisionTime,
    DateTimeOffset Expiry,
    string EvidenceReference)
{
    public string ContractId => ContractIdentity.Con002;
}

public sealed record LifecycleTransitionRequest(
    string TransitionRequestId,
    string ComponentIdentity,
    string AuthoritativeSourceState,
    string RequestedTargetState,
    string Requester,
    string AuthorityReference,
    string Reason,
    string DependencyContext,
    DateTimeOffset RequestTime,
    DateTimeOffset Expiry)
{
    public string ContractId => ContractIdentity.Con003;
}

public sealed record LifecycleTransitionResult(
    string RequestId,
    string TransitionId,
    string Decision,
    string SourceState,
    string TargetState,
    string ActualResultingState,
    string Reason,
    string ValidationEvidence,
    DateTimeOffset CompletionTime,
    string EmittedEventReference)
{
    public string ContractId => ContractIdentity.Con003;
}

public sealed record FilEnvelope(
    string MessageId,
    string MessageKind,
    string MessageType,
    string SchemaId,
    string SchemaVersion,
    string ProducerIdentity,
    DateTimeOffset CreationTime,
    string Purpose,
    string SecurityClassification,
    string? CorrelationId,
    string? CausationId,
    DateTimeOffset? Expiry,
    string PriorityAuthority,
    string IntegrityEvidence,
    string ProtectionProfileId,
    string ProtectionProfileVersion,
    string IntegrityScope,
    string EncryptionScope,
    string AuthorizedKeyReference,
    string AuthorizedKeyVersion,
    string? IntendedRecipientBinding,
    string ReplayPolicy,
    string DeliveryAttemptId,
    string? Nonce,
    string Payload)
{
    public string ContractId => ContractIdentity.Con004;
}

public sealed record FilEvent(
    string EventId,
    string EventType,
    string SchemaVersion,
    string AuthoritativeFactOwner,
    string SubjectIdentity,
    DateTimeOffset OccurrenceTime,
    DateTimeOffset PublicationTime,
    string SourceEvidence,
    string? Correlation,
    string? Causation,
    bool ReplayIndicator,
    string? CorrectionRelationship,
    string Payload)
{
    public string ContractId => ContractIdentity.Con005;
}

public enum ValidationResult
{
    Pass,
    Fail
}

public sealed record ValidationOutcome(ValidationResult Result, string Message)
{
    public static ValidationOutcome Passed(string message) => new(ValidationResult.Pass, message);
    public static ValidationOutcome Failed(string message) => new(ValidationResult.Fail, message);
}

public static partial class ContractValidators
{
    public static ValidationOutcome Validate(CoreIdentity? value)
    {
        if (value is null)
        {
            return ValidationOutcome.Failed("CoreIdentity validation rejected");
        }

        return ValidatePrefix(ContractIdentity.Con001, value.ContractId, !string.IsNullOrWhiteSpace(value.SubjectId) &&
            !string.IsNullOrWhiteSpace(value.SubjectClass) &&
            !string.IsNullOrWhiteSpace(value.InstanceId) &&
            !string.IsNullOrWhiteSpace(value.Version) &&
            !string.IsNullOrWhiteSpace(value.Owner) &&
            !string.IsNullOrWhiteSpace(value.AdmittedCapability) &&
            !string.IsNullOrWhiteSpace(value.ArtifactIdentity) &&
            !string.IsNullOrWhiteSpace(value.AuthorityContext) &&
            !string.IsNullOrWhiteSpace(value.LifecycleIdentity) &&
            value.CreationTime != default &&
            !string.IsNullOrWhiteSpace(value.IntegrityEvidence));
    }

    public static ValidationOutcome Validate(AuthorityRequest? value)
    {
        if (value is null)
        {
            return ValidationOutcome.Failed("AuthorityRequest validation rejected");
        }

        return ValidatePrefix(ContractIdentity.Con002, value.ContractId, !string.IsNullOrWhiteSpace(value.RequestId) &&
            !string.IsNullOrWhiteSpace(value.ActorIdentity) &&
            !string.IsNullOrWhiteSpace(value.Action) &&
            !string.IsNullOrWhiteSpace(value.Resource) &&
            !string.IsNullOrWhiteSpace(value.Purpose) &&
            !string.IsNullOrWhiteSpace(value.RequestedScope) &&
            !string.IsNullOrWhiteSpace(value.OperatingContext) &&
            !string.IsNullOrWhiteSpace(value.SecurityContext) &&
            !string.IsNullOrWhiteSpace(value.RequiredFitnessToOperate) &&
            !string.IsNullOrWhiteSpace(value.Correlation) &&
            value.RequestTime != default &&
            value.Expiry > value.RequestTime);
    }

    public static ValidationOutcome Validate(AuthorityResult? value)
    {
        if (value is null)
        {
            return ValidationOutcome.Failed("AuthorityResult validation rejected");
        }

        return ValidatePrefix(ContractIdentity.Con002, ContractIdentity.Con002, IsDecision(value.Decision) &&
            !string.IsNullOrWhiteSpace(value.RequestId) &&
            !string.IsNullOrWhiteSpace(value.DecisionId) &&
            !string.IsNullOrWhiteSpace(value.EffectiveScope) &&
            !string.IsNullOrWhiteSpace(value.ControllingPolicy) &&
            !string.IsNullOrWhiteSpace(value.PolicyVersion) &&
            !string.IsNullOrWhiteSpace(value.MaterialConditions) &&
            !string.IsNullOrWhiteSpace(value.Constraints) &&
            !string.IsNullOrWhiteSpace(value.Reason) &&
            value.DecisionTime != default &&
            value.Expiry > value.DecisionTime &&
            !string.IsNullOrWhiteSpace(value.EvidenceReference));
    }

    public static ValidationOutcome Validate(LifecycleTransitionRequest? value)
    {
        if (value is null)
        {
            return ValidationOutcome.Failed("LifecycleTransitionRequest validation rejected");
        }

        return ValidatePrefix(ContractIdentity.Con003, value.ContractId, !string.IsNullOrWhiteSpace(value.TransitionRequestId) &&
            !string.IsNullOrWhiteSpace(value.ComponentIdentity) &&
            !string.IsNullOrWhiteSpace(value.AuthoritativeSourceState) &&
            !string.IsNullOrWhiteSpace(value.RequestedTargetState) &&
            !string.IsNullOrWhiteSpace(value.Requester) &&
            !string.IsNullOrWhiteSpace(value.AuthorityReference) &&
            !string.IsNullOrWhiteSpace(value.Reason) &&
            !string.IsNullOrWhiteSpace(value.DependencyContext) &&
            value.RequestTime != default &&
            value.Expiry > value.RequestTime);
    }

    public static ValidationOutcome Validate(LifecycleTransitionResult? value)
    {
        if (value is null)
        {
            return ValidationOutcome.Failed("LifecycleTransitionResult validation rejected");
        }

        return ValidatePrefix(ContractIdentity.Con003, ContractIdentity.Con003, IsDecision(value.Decision) &&
            !string.IsNullOrWhiteSpace(value.RequestId) &&
            !string.IsNullOrWhiteSpace(value.TransitionId) &&
            !string.IsNullOrWhiteSpace(value.SourceState) &&
            !string.IsNullOrWhiteSpace(value.TargetState) &&
            !string.IsNullOrWhiteSpace(value.ActualResultingState) &&
            !string.IsNullOrWhiteSpace(value.Reason) &&
            !string.IsNullOrWhiteSpace(value.ValidationEvidence) &&
            value.CompletionTime != default &&
            !string.IsNullOrWhiteSpace(value.EmittedEventReference));
    }

    public static ValidationOutcome Validate(FilEnvelope? value)
    {
        if (value is null)
        {
            return ValidationOutcome.Failed("FilEnvelope validation rejected");
        }

        return ValidatePrefix(ContractIdentity.Con004, value.ContractId, !string.IsNullOrWhiteSpace(value.MessageId) &&
            !string.IsNullOrWhiteSpace(value.MessageKind) &&
            !string.IsNullOrWhiteSpace(value.MessageType) &&
            !string.IsNullOrWhiteSpace(value.SchemaId) &&
            !string.IsNullOrWhiteSpace(value.SchemaVersion) &&
            !string.IsNullOrWhiteSpace(value.ProducerIdentity) &&
            value.CreationTime != default &&
            !string.IsNullOrWhiteSpace(value.Purpose) &&
            !string.IsNullOrWhiteSpace(value.SecurityClassification) &&
            !string.IsNullOrWhiteSpace(value.PriorityAuthority) &&
            !string.IsNullOrWhiteSpace(value.IntegrityEvidence) &&
            !string.IsNullOrWhiteSpace(value.ProtectionProfileId) &&
            !string.IsNullOrWhiteSpace(value.ProtectionProfileVersion) &&
            !string.IsNullOrWhiteSpace(value.IntegrityScope) &&
            !string.IsNullOrWhiteSpace(value.EncryptionScope) &&
            !string.IsNullOrWhiteSpace(value.AuthorizedKeyReference) &&
            !string.IsNullOrWhiteSpace(value.AuthorizedKeyVersion) &&
            !string.IsNullOrWhiteSpace(value.ReplayPolicy) &&
            !string.IsNullOrWhiteSpace(value.DeliveryAttemptId) &&
            !string.IsNullOrWhiteSpace(value.Payload));
    }

    public static ValidationOutcome Validate(FilEvent? value)
    {
        if (value is null)
        {
            return ValidationOutcome.Failed("FilEvent validation rejected");
        }

        return ValidatePrefix(ContractIdentity.Con005, value.ContractId, !string.IsNullOrWhiteSpace(value.EventId) &&
            !string.IsNullOrWhiteSpace(value.EventType) &&
            !string.IsNullOrWhiteSpace(value.SchemaVersion) &&
            !string.IsNullOrWhiteSpace(value.AuthoritativeFactOwner) &&
            !string.IsNullOrWhiteSpace(value.SubjectIdentity) &&
            value.OccurrenceTime != default &&
            value.PublicationTime != default &&
            value.PublicationTime >= value.OccurrenceTime &&
            !string.IsNullOrWhiteSpace(value.SourceEvidence) &&
            !string.IsNullOrWhiteSpace(value.Payload) &&
            (value.ReplayIndicator || value.CorrectionRelationship is null || value.CorrectionRelationship.Length > 0));
    }

    private static ValidationOutcome ValidatePrefix(string expected, string actual, bool ok)
        => ok && string.Equals(expected, actual, StringComparison.Ordinal)
            ? ValidationOutcome.Passed("ok")
            : ValidationOutcome.Failed($"{expected} validation rejected");

    private static bool IsDecision(string value)
        => string.Equals(value, "ALLOW", StringComparison.Ordinal) ||
           string.Equals(value, "DENY", StringComparison.Ordinal) ||
           string.Equals(value, "ACCEPTED", StringComparison.Ordinal) ||
           string.Equals(value, "REJECTED", StringComparison.Ordinal) ||
           string.Equals(value, "FAILED", StringComparison.Ordinal);
}

public static class ContractVersions
{
    public const string Con006 = "1.1";
    public const string Con007 = "1.0";
    public const string Con008 = "1.1";
    public const string Con009 = "1.0";
    public const string Con010 = "1.1";
    public const string Con011 = "1.0";
}

public sealed record HealthFitnessAssessment(
    string AssessmentId,
    string Version,
    string SubjectId,
    string Capability,
    string RequestedAuthorityLevel,
    string HealthState,
    string FitnessResult,
    string Scope,
    string EvidenceReference,
    string SelfModelReference,
    string Confidence,
    string Constraints,
    string Reason,
    DateTimeOffset EffectiveTime,
    DateTimeOffset Expiry)
{
    public string ContractId => ContractIdentity.Con006;
}

public sealed record ConfigurationAdmission(
    string AdmissionId,
    string Version,
    string ConfigurationKey,
    string Owner,
    string Source,
    string Scope,
    string ActivationMode,
    string ResolutionResult,
    string ValidationEvidence,
    string AuthorityReference,
    string RejectionReason,
    DateTimeOffset EffectiveTime,
    DateTimeOffset Expiry)
{
    public string ContractId => ContractIdentity.Con007;
}

public sealed record EvidenceLinkRecord(
    string EvidenceId,
    string Version,
    string SubjectId,
    string Origin,
    string Completeness,
    string LinkageState,
    string SourceRecordId,
    string TargetRecordId,
    string Digest,
    string AuthorityReference,
    string ValidationEvidence,
    DateTimeOffset CaptureTime)
{
    public string ContractId => ContractIdentity.Con008;
}

public sealed record SecurityBoundaryRecord(
    string BoundaryId,
    string Version,
    string SubjectId,
    string BoundaryClass,
    string AuthorityReference,
    string AllowedReferences,
    string ProhibitedReferences,
    string ValidationEvidence,
    string BoundaryResult,
    DateTimeOffset EffectiveTime,
    DateTimeOffset Expiry)
{
    public string ContractId => ContractIdentity.Con009;
}

public sealed record ManifestSurfaceRecord(
    string ManifestId,
    string Version,
    string ManifestClass,
    string SubjectId,
    string EvidenceSetId,
    string SeparationResult,
    string IntegrityResult,
    string AuthorityReference,
    string ValidationEvidence,
    string CanonicalDigest,
    DateTimeOffset EffectiveTime,
    DateTimeOffset Expiry)
{
    public string ContractId => ContractIdentity.Con010;
}

public sealed record RestrictionRecord(
    string RestrictionId,
    string Version,
    string SubjectId,
    string MandateReference,
    string TriggerEvidence,
    string ProtectiveMode,
    string AllowedSafeActions,
    string ProhibitedActions,
    string ReleaseConditions,
    string ReleaseAuthority,
    string Result,
    string IntegrityEvidence,
    DateTimeOffset EffectiveTime,
    DateTimeOffset Expiry)
{
    public string ContractId => ContractIdentity.Con011;
}

public static partial class ContractValidators
{
    public static ValidationOutcome Validate(HealthFitnessAssessment? value)
    {
        if (value is null)
        {
            return ValidationOutcome.Failed("HealthFitnessAssessment validation rejected");
        }

        return ValidateContract(ContractIdentity.Con006, value.ContractId,
            IsExpectedVersion(ContractVersions.Con006, value.Version) &&
            !string.IsNullOrWhiteSpace(value.AssessmentId) &&
            !string.IsNullOrWhiteSpace(value.SubjectId) &&
            !string.IsNullOrWhiteSpace(value.Capability) &&
            !string.IsNullOrWhiteSpace(value.RequestedAuthorityLevel) &&
            IsOneOf(value.HealthState, "HEALTHY", "DEGRADED", "UNHEALTHY", "UNKNOWN", "NOT_APPLICABLE") &&
            IsOneOf(value.FitnessResult, "FIT", "RESTRICTED", "NOT_FIT") &&
            !string.IsNullOrWhiteSpace(value.Scope) &&
            !string.IsNullOrWhiteSpace(value.EvidenceReference) &&
            !string.IsNullOrWhiteSpace(value.SelfModelReference) &&
            !string.IsNullOrWhiteSpace(value.Confidence) &&
            !string.IsNullOrWhiteSpace(value.Constraints) &&
            !string.IsNullOrWhiteSpace(value.Reason) &&
            value.EffectiveTime != default &&
            value.Expiry > value.EffectiveTime);
    }

    public static ValidationOutcome Validate(ConfigurationAdmission? value)
    {
        if (value is null)
        {
            return ValidationOutcome.Failed("ConfigurationAdmission validation rejected");
        }

        return ValidateContract(ContractIdentity.Con007, value.ContractId,
            IsExpectedVersion(ContractVersions.Con007, value.Version) &&
            !string.IsNullOrWhiteSpace(value.AdmissionId) &&
            !string.IsNullOrWhiteSpace(value.ConfigurationKey) &&
            !string.IsNullOrWhiteSpace(value.Owner) &&
            !string.IsNullOrWhiteSpace(value.Source) &&
            !string.IsNullOrWhiteSpace(value.Scope) &&
            !string.IsNullOrWhiteSpace(value.ActivationMode) &&
            IsOneOf(value.ResolutionResult, "ADMITTED", "REJECTED") &&
            !string.IsNullOrWhiteSpace(value.ValidationEvidence) &&
            !string.IsNullOrWhiteSpace(value.AuthorityReference) &&
            value.EffectiveTime != default &&
            value.Expiry > value.EffectiveTime &&
            (value.ResolutionResult != "REJECTED" || !string.IsNullOrWhiteSpace(value.RejectionReason)));
    }

    public static ValidationOutcome Validate(EvidenceLinkRecord? value)
    {
        if (value is null)
        {
            return ValidationOutcome.Failed("EvidenceLinkRecord validation rejected");
        }

        return ValidateContract(ContractIdentity.Con008, value.ContractId,
            IsExpectedVersion(ContractVersions.Con008, value.Version) &&
            !string.IsNullOrWhiteSpace(value.EvidenceId) &&
            !string.IsNullOrWhiteSpace(value.SubjectId) &&
            IsOneOf(value.Origin, "BOOTSTRAP_EXTERNAL", "CANDIDATE_PRODUCED", "FALCON_NATIVE", "IMPORTED_EXTERNAL") &&
            IsOneOf(value.Completeness, "COMPLETE", "PARTIAL", "INCOMPLETE", "INVALID") &&
            IsOneOf(value.LinkageState, "LINKED", "UNLINKED", "AMBIGUOUS") &&
            !string.IsNullOrWhiteSpace(value.SourceRecordId) &&
            !string.IsNullOrWhiteSpace(value.TargetRecordId) &&
            IsHexDigest(value.Digest) &&
            !string.IsNullOrWhiteSpace(value.AuthorityReference) &&
            !string.IsNullOrWhiteSpace(value.ValidationEvidence) &&
            value.CaptureTime != default &&
            value.Completeness == "COMPLETE" &&
            value.LinkageState == "LINKED");
    }

    public static ValidationOutcome Validate(SecurityBoundaryRecord? value)
    {
        if (value is null)
        {
            return ValidationOutcome.Failed("SecurityBoundaryRecord validation rejected");
        }

        return ValidateContract(ContractIdentity.Con009, value.ContractId,
            IsExpectedVersion(ContractVersions.Con009, value.Version) &&
            !string.IsNullOrWhiteSpace(value.BoundaryId) &&
            !string.IsNullOrWhiteSpace(value.SubjectId) &&
            !string.IsNullOrWhiteSpace(value.BoundaryClass) &&
            !string.IsNullOrWhiteSpace(value.AuthorityReference) &&
            !string.IsNullOrWhiteSpace(value.AllowedReferences) &&
            !string.IsNullOrWhiteSpace(value.ProhibitedReferences) &&
            !string.IsNullOrWhiteSpace(value.ValidationEvidence) &&
            IsOneOf(value.BoundaryResult, "COMPLIANT", "VIOLATION") &&
            value.EffectiveTime != default &&
            value.Expiry > value.EffectiveTime &&
            value.BoundaryResult == "COMPLIANT");
    }

    public static ValidationOutcome Validate(ManifestSurfaceRecord? value)
    {
        if (value is null)
        {
            return ValidationOutcome.Failed("ManifestSurfaceRecord validation rejected");
        }

        return ValidateContract(ContractIdentity.Con010, value.ContractId,
            IsExpectedVersion(ContractVersions.Con010, value.Version) &&
            !string.IsNullOrWhiteSpace(value.ManifestId) &&
            !string.IsNullOrWhiteSpace(value.ManifestClass) &&
            !string.IsNullOrWhiteSpace(value.SubjectId) &&
            !string.IsNullOrWhiteSpace(value.EvidenceSetId) &&
            IsOneOf(value.SeparationResult, "SEPARATE", "MIXED") &&
            IsOneOf(value.IntegrityResult, "INTACT", "FAILED") &&
            !string.IsNullOrWhiteSpace(value.AuthorityReference) &&
            !string.IsNullOrWhiteSpace(value.ValidationEvidence) &&
            IsHexDigest(value.CanonicalDigest) &&
            value.EffectiveTime != default &&
            value.Expiry > value.EffectiveTime &&
            value.SeparationResult == "SEPARATE" &&
            value.IntegrityResult == "INTACT");
    }

    public static ValidationOutcome Validate(RestrictionRecord? value)
    {
        if (value is null)
        {
            return ValidationOutcome.Failed("RestrictionRecord validation rejected");
        }

        return ValidateContract(ContractIdentity.Con011, value.ContractId,
            IsExpectedVersion(ContractVersions.Con011, value.Version) &&
            !string.IsNullOrWhiteSpace(value.RestrictionId) &&
            !string.IsNullOrWhiteSpace(value.SubjectId) &&
            !string.IsNullOrWhiteSpace(value.MandateReference) &&
            !string.IsNullOrWhiteSpace(value.TriggerEvidence) &&
            !string.IsNullOrWhiteSpace(value.ProtectiveMode) &&
            !string.IsNullOrWhiteSpace(value.AllowedSafeActions) &&
            !string.IsNullOrWhiteSpace(value.ProhibitedActions) &&
            !string.IsNullOrWhiteSpace(value.ReleaseConditions) &&
            !string.IsNullOrWhiteSpace(value.ReleaseAuthority) &&
            !string.IsNullOrWhiteSpace(value.Result) &&
            !string.IsNullOrWhiteSpace(value.IntegrityEvidence) &&
            value.EffectiveTime != default &&
            value.Expiry > value.EffectiveTime &&
            IsOneOf(value.Result, "IMPOSED", "REJECTED") &&
            value.Result == "IMPOSED");
    }

    private static ValidationOutcome ValidateContract(string expected, string actual, bool ok)
        => ok && string.Equals(expected, actual, StringComparison.Ordinal)
            ? ValidationOutcome.Passed("ok")
            : ValidationOutcome.Failed($"{expected} validation rejected");

    private static bool IsExpectedVersion(string expected, string actual)
        => string.Equals(expected, actual, StringComparison.Ordinal);

    private static bool IsOneOf(string value, params string[] options)
        => options.Any(option => string.Equals(value, option, StringComparison.Ordinal));

    private static bool IsHexDigest(string? value)
        => value is not null && value.Length == 64 && value.All(c => (c >= '0' && c <= '9') || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f'));
}

public static class ProviderContractVersions
{
    public const string Con012 = "1.0";
    public const string Con013 = "1.0";
    public const string Con014 = "1.0";
    public const string Con015 = "1.0";
    public const string Con016 = "1.0";
    public const string Con017 = "1.0";
    public const string Con018 = "1.0";
    public const string Con019 = "1.0";
    public const string Con020 = "1.0";
    public const string Con021 = "1.0";
}

public sealed record AuthorityInstrumentRecord(
    string InstrumentId,
    string Version,
    string GoverningAuthority,
    string Scope,
    string Issuer,
    string AuthoritySource,
    string DelegationPolicy,
    string ValidationEvidence,
    string Decision,
    DateTimeOffset EffectiveTime,
    DateTimeOffset Expiry)
{
    public string ContractId => ContractIdentity.Con012;
}

public sealed record DelegationRecord(
    string DelegationId,
    string Version,
    string Grantor,
    string Grantee,
    string Scope,
    string ChainIdentity,
    string AuthoritySource,
    string ValidationEvidence,
    string DelegationState,
    string TerminationRule,
    DateTimeOffset EffectiveTime,
    DateTimeOffset Expiry)
{
    public string ContractId => ContractIdentity.Con013;
}

public sealed record IdentifierProviderRecord(
    string ProviderId,
    string Version,
    string ProviderClass,
    string AdmissionAuthority,
    string Boundaries,
    string IdentityEvidence,
    string AdmissionResult,
    string BypassProtection,
    string ValidationEvidence,
    DateTimeOffset EffectiveTime,
    DateTimeOffset Expiry)
{
    public string ContractId => ContractIdentity.Con014;
}

public sealed record TimeProviderRecord(
    string ProviderId,
    string Version,
    string ProviderClass,
    string AdmissionAuthority,
    string Boundaries,
    string SourceOfTime,
    string ValidationEvidence,
    string AdmissionResult,
    DateTimeOffset EffectiveTime,
    DateTimeOffset Expiry)
{
    public string ContractId => ContractIdentity.Con015;
}

public sealed record CryptographicProviderRecord(
    string ProviderId,
    string Version,
    string ProviderClass,
    string AdmissionAuthority,
    string Boundaries,
    string KeyMaterialReference,
    string ValidationEvidence,
    string AdmissionResult,
    DateTimeOffset EffectiveTime,
    DateTimeOffset Expiry)
{
    public string ContractId => ContractIdentity.Con016;
}

public sealed record SecretCustodyRecord(
    string CustodyId,
    string Version,
    string ProviderId,
    string SecretClass,
    string CustodyPolicy,
    string AccessBoundary,
    string ValidationEvidence,
    string CustodyResult,
    DateTimeOffset EffectiveTime,
    DateTimeOffset Expiry)
{
    public string ContractId => ContractIdentity.Con017;
}

public sealed record CertificateIdentityProviderRecord(
    string ProviderId,
    string Version,
    string ProviderClass,
    string AdmissionAuthority,
    string TrustAnchorReference,
    string AdmissionResult,
    string ValidationEvidence,
    DateTimeOffset EffectiveTime,
    DateTimeOffset Expiry)
{
    public string ContractId => ContractIdentity.Con018;
}

public sealed record RandomnessProviderRecord(
    string ProviderId,
    string Version,
    string ProviderClass,
    string AdmissionAuthority,
    string EntropySource,
    string ValidationEvidence,
    string AdmissionResult,
    DateTimeOffset EffectiveTime,
    DateTimeOffset Expiry)
{
    public string ContractId => ContractIdentity.Con019;
}

public static partial class ProviderContractValidators
{
    public static ValidationOutcome Validate(AuthorityInstrumentRecord? value)
    {
        if (value is null)
        {
            return ValidationOutcome.Failed("AuthorityInstrumentRecord validation rejected");
        }

        return ValidateContract(ContractIdentity.Con012, value.ContractId,
            IsExpectedVersion(ProviderContractVersions.Con012, value.Version) &&
            !string.IsNullOrWhiteSpace(value.InstrumentId) &&
            !string.IsNullOrWhiteSpace(value.GoverningAuthority) &&
            !string.IsNullOrWhiteSpace(value.Scope) &&
            !string.IsNullOrWhiteSpace(value.Issuer) &&
            !string.IsNullOrWhiteSpace(value.AuthoritySource) &&
            !string.IsNullOrWhiteSpace(value.DelegationPolicy) &&
            !string.IsNullOrWhiteSpace(value.ValidationEvidence) &&
            IsOneOf(value.Decision, "ISSUED", "REJECTED") &&
            value.EffectiveTime != default &&
            value.Expiry > value.EffectiveTime &&
            value.Decision == "ISSUED");
    }

    public static ValidationOutcome Validate(DelegationRecord? value)
    {
        if (value is null)
        {
            return ValidationOutcome.Failed("DelegationRecord validation rejected");
        }

        return ValidateContract(ContractIdentity.Con013, value.ContractId,
            IsExpectedVersion(ProviderContractVersions.Con013, value.Version) &&
            !string.IsNullOrWhiteSpace(value.DelegationId) &&
            !string.IsNullOrWhiteSpace(value.Grantor) &&
            !string.IsNullOrWhiteSpace(value.Grantee) &&
            !string.IsNullOrWhiteSpace(value.Scope) &&
            !string.IsNullOrWhiteSpace(value.ChainIdentity) &&
            !string.IsNullOrWhiteSpace(value.AuthoritySource) &&
            !string.IsNullOrWhiteSpace(value.ValidationEvidence) &&
            IsOneOf(value.DelegationState, "GRANTED", "REVOKED") &&
            !string.IsNullOrWhiteSpace(value.TerminationRule) &&
            value.EffectiveTime != default &&
            value.Expiry > value.EffectiveTime &&
            value.DelegationState == "GRANTED");
    }

    public static ValidationOutcome Validate(IdentifierProviderRecord? value)
    {
        if (value is null)
        {
            return ValidationOutcome.Failed("IdentifierProviderRecord validation rejected");
        }

        return ValidateContract(ContractIdentity.Con014, value.ContractId,
            IsExpectedVersion(ProviderContractVersions.Con014, value.Version) &&
            !string.IsNullOrWhiteSpace(value.ProviderId) &&
            !string.IsNullOrWhiteSpace(value.ProviderClass) &&
            !string.IsNullOrWhiteSpace(value.AdmissionAuthority) &&
            !string.IsNullOrWhiteSpace(value.Boundaries) &&
            !string.IsNullOrWhiteSpace(value.IdentityEvidence) &&
            IsOneOf(value.AdmissionResult, "ADMITTED", "REJECTED") &&
            IsOneOf(value.BypassProtection, "PROHIBITED", "ALLOWED") &&
            !string.IsNullOrWhiteSpace(value.ValidationEvidence) &&
            value.EffectiveTime != default &&
            value.Expiry > value.EffectiveTime &&
            value.AdmissionResult == "ADMITTED" &&
            value.BypassProtection == "PROHIBITED");
    }

    public static ValidationOutcome Validate(TimeProviderRecord? value)
    {
        if (value is null)
        {
            return ValidationOutcome.Failed("TimeProviderRecord validation rejected");
        }

        return ValidateContract(ContractIdentity.Con015, value.ContractId,
            IsExpectedVersion(ProviderContractVersions.Con015, value.Version) &&
            !string.IsNullOrWhiteSpace(value.ProviderId) &&
            !string.IsNullOrWhiteSpace(value.ProviderClass) &&
            !string.IsNullOrWhiteSpace(value.AdmissionAuthority) &&
            !string.IsNullOrWhiteSpace(value.Boundaries) &&
            !string.IsNullOrWhiteSpace(value.SourceOfTime) &&
            !string.IsNullOrWhiteSpace(value.ValidationEvidence) &&
            IsOneOf(value.AdmissionResult, "ADMITTED", "REJECTED") &&
            value.EffectiveTime != default &&
            value.Expiry > value.EffectiveTime &&
            value.AdmissionResult == "ADMITTED");
    }

    public static ValidationOutcome Validate(CryptographicProviderRecord? value)
    {
        if (value is null)
        {
            return ValidationOutcome.Failed("CryptographicProviderRecord validation rejected");
        }

        return ValidateContract(ContractIdentity.Con016, value.ContractId,
            IsExpectedVersion(ProviderContractVersions.Con016, value.Version) &&
            !string.IsNullOrWhiteSpace(value.ProviderId) &&
            !string.IsNullOrWhiteSpace(value.ProviderClass) &&
            !string.IsNullOrWhiteSpace(value.AdmissionAuthority) &&
            !string.IsNullOrWhiteSpace(value.Boundaries) &&
            !string.IsNullOrWhiteSpace(value.KeyMaterialReference) &&
            !string.IsNullOrWhiteSpace(value.ValidationEvidence) &&
            IsOneOf(value.AdmissionResult, "ADMITTED", "REJECTED") &&
            value.EffectiveTime != default &&
            value.Expiry > value.EffectiveTime &&
            value.AdmissionResult == "ADMITTED");
    }

    public static ValidationOutcome Validate(SecretCustodyRecord? value)
    {
        if (value is null)
        {
            return ValidationOutcome.Failed("SecretCustodyRecord validation rejected");
        }

        return ValidateContract(ContractIdentity.Con017, value.ContractId,
            IsExpectedVersion(ProviderContractVersions.Con017, value.Version) &&
            !string.IsNullOrWhiteSpace(value.CustodyId) &&
            !string.IsNullOrWhiteSpace(value.ProviderId) &&
            !string.IsNullOrWhiteSpace(value.SecretClass) &&
            !string.IsNullOrWhiteSpace(value.CustodyPolicy) &&
            !string.IsNullOrWhiteSpace(value.AccessBoundary) &&
            !string.IsNullOrWhiteSpace(value.ValidationEvidence) &&
            IsOneOf(value.CustodyResult, "ADMITTED", "REJECTED") &&
            value.EffectiveTime != default &&
            value.Expiry > value.EffectiveTime &&
            value.CustodyResult == "ADMITTED");
    }

    public static ValidationOutcome Validate(CertificateIdentityProviderRecord? value)
    {
        if (value is null)
        {
            return ValidationOutcome.Failed("CertificateIdentityProviderRecord validation rejected");
        }

        return ValidateContract(ContractIdentity.Con018, value.ContractId,
            IsExpectedVersion(ProviderContractVersions.Con018, value.Version) &&
            !string.IsNullOrWhiteSpace(value.ProviderId) &&
            !string.IsNullOrWhiteSpace(value.ProviderClass) &&
            !string.IsNullOrWhiteSpace(value.AdmissionAuthority) &&
            !string.IsNullOrWhiteSpace(value.TrustAnchorReference) &&
            !string.IsNullOrWhiteSpace(value.ValidationEvidence) &&
            IsOneOf(value.AdmissionResult, "ADMITTED", "REJECTED") &&
            value.EffectiveTime != default &&
            value.Expiry > value.EffectiveTime &&
            value.AdmissionResult == "ADMITTED");
    }

    public static ValidationOutcome Validate(RandomnessProviderRecord? value)
    {
        if (value is null)
        {
            return ValidationOutcome.Failed("RandomnessProviderRecord validation rejected");
        }

        return ValidateContract(ContractIdentity.Con019, value.ContractId,
            IsExpectedVersion(ProviderContractVersions.Con019, value.Version) &&
            !string.IsNullOrWhiteSpace(value.ProviderId) &&
            !string.IsNullOrWhiteSpace(value.ProviderClass) &&
            !string.IsNullOrWhiteSpace(value.AdmissionAuthority) &&
            !string.IsNullOrWhiteSpace(value.EntropySource) &&
            !string.IsNullOrWhiteSpace(value.ValidationEvidence) &&
            IsOneOf(value.AdmissionResult, "ADMITTED", "REJECTED") &&
            value.EffectiveTime != default &&
            value.Expiry > value.EffectiveTime &&
            value.AdmissionResult == "ADMITTED");
    }

    private static ValidationOutcome ValidateContract(string expected, string actual, bool ok)
        => ok && string.Equals(expected, actual, StringComparison.Ordinal)
            ? ValidationOutcome.Passed("ok")
            : ValidationOutcome.Failed($"{expected} validation rejected");

    private static bool IsExpectedVersion(string expected, string actual)
        => string.Equals(expected, actual, StringComparison.Ordinal);

    private static bool IsOneOf(string value, params string[] options)
        => options.Any(option => string.Equals(value, option, StringComparison.Ordinal));
}

public sealed record BootstrapExecutionContextRecord(
    string ContextId,
    string Version,
    string BootstrapAuthority,
    string EnvironmentIdentity,
    string Scope,
    string SourceIdentity,
    string ValidationEvidence,
    string ContextState,
    string AuthorityBoundary,
    DateTimeOffset EffectiveTime,
    DateTimeOffset Expiry)
{
    public string ContractId => ContractIdentity.Con020;
}

public sealed record BootstrapEvidenceProvenanceRecord(
    string ProvenanceId,
    string Version,
    string SourceRecordId,
    string SourceDigest,
    string SourceIdentity,
    string ProvenanceAuthority,
    string ValidationEvidence,
    string ProvenanceState,
    string ArtifactIdentity,
    DateTimeOffset EffectiveTime,
    DateTimeOffset Expiry)
{
    public string ContractId => ContractIdentity.Con021;
}

public static partial class BootstrapContractValidators
{
    public static ValidationOutcome Validate(BootstrapExecutionContextRecord? value)
    {
        if (value is null)
        {
            return ValidationOutcome.Failed("BootstrapExecutionContextRecord validation rejected");
        }

        return ValidateContract(ContractIdentity.Con020, value.ContractId,
            IsExpectedVersion(ProviderContractVersions.Con020, value.Version) &&
            !string.IsNullOrWhiteSpace(value.ContextId) &&
            !string.IsNullOrWhiteSpace(value.BootstrapAuthority) &&
            !string.IsNullOrWhiteSpace(value.EnvironmentIdentity) &&
            !string.IsNullOrWhiteSpace(value.Scope) &&
            !string.IsNullOrWhiteSpace(value.SourceIdentity) &&
            !string.IsNullOrWhiteSpace(value.ValidationEvidence) &&
            IsOneOf(value.ContextState, "DEFINED", "REJECTED") &&
            !string.IsNullOrWhiteSpace(value.AuthorityBoundary) &&
            value.EffectiveTime != default &&
            value.Expiry > value.EffectiveTime &&
            value.ContextState == "DEFINED");
    }

    public static ValidationOutcome Validate(BootstrapEvidenceProvenanceRecord? value)
    {
        if (value is null)
        {
            return ValidationOutcome.Failed("BootstrapEvidenceProvenanceRecord validation rejected");
        }

        return ValidateContract(ContractIdentity.Con021, value.ContractId,
            IsExpectedVersion(ProviderContractVersions.Con021, value.Version) &&
            !string.IsNullOrWhiteSpace(value.ProvenanceId) &&
            !string.IsNullOrWhiteSpace(value.SourceRecordId) &&
            IsHexDigest(value.SourceDigest) &&
            !string.IsNullOrWhiteSpace(value.SourceIdentity) &&
            !string.IsNullOrWhiteSpace(value.ProvenanceAuthority) &&
            !string.IsNullOrWhiteSpace(value.ValidationEvidence) &&
            IsOneOf(value.ProvenanceState, "PROVEN", "REJECTED") &&
            !string.IsNullOrWhiteSpace(value.ArtifactIdentity) &&
            value.EffectiveTime != default &&
            value.Expiry > value.EffectiveTime &&
            value.ProvenanceState == "PROVEN");
    }

    private static ValidationOutcome ValidateContract(string expected, string actual, bool ok)
        => ok && string.Equals(expected, actual, StringComparison.Ordinal)
            ? ValidationOutcome.Passed("ok")
            : ValidationOutcome.Failed($"{expected} validation rejected");

    private static bool IsExpectedVersion(string expected, string actual)
        => string.Equals(expected, actual, StringComparison.Ordinal);

    private static bool IsOneOf(string value, params string[] options)
        => options.Any(option => string.Equals(value, option, StringComparison.Ordinal));

    private static bool IsHexDigest(string? value)
        => value is not null && value.Length == 64 && value.All(c => (c >= '0' && c <= '9') || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f'));
}
