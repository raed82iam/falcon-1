using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Foundation.Authority;
using Foundation.Contracts;
using Foundation.Core;
using Foundation.Evidence;
using Foundation.Reconciliation;
using Foundation.State;

namespace Foundation.Infrastructure;

public enum BootstrapSubjectKind
{
    Service,
    Plugin
}

public sealed record BootstrapSubjectAdmissionEvidence
{
    public string SubjectId { get; init; } = string.Empty;

    public string SubjectVersion { get; init; } = string.Empty;

    public BootstrapSubjectKind SubjectKind { get; init; }

    public string ArtifactIdentity { get; init; } = string.Empty;

    public string ArtifactDigest { get; init; } = string.Empty;

    public string ManifestIdentity { get; init; } = string.Empty;

    public string ManifestDigest { get; init; } = string.Empty;

    public string AdmissionDecisionId { get; init; } = string.Empty;

    public string AdmissionState { get; init; } = string.Empty;

    public string RegistrationEvidenceId { get; init; } = string.Empty;

    public string RegistrationState { get; init; } = string.Empty;

    public string EvidenceAuthority { get; init; } = string.Empty;

    public DateTimeOffset EffectiveTime { get; init; }

    public DateTimeOffset Expiry { get; init; }
}

public sealed record DependencyActivationEvidence
{
    public string SubjectId { get; init; } = string.Empty;

    public string SubjectVersion { get; init; } = string.Empty;

    public string GraphId { get; init; } = string.Empty;

    public string GraphVersion { get; init; } = string.Empty;

    public string GraphDigest { get; init; } = string.Empty;

    public string DependencyValidationState { get; init; } = string.Empty;

    public string ActivationOrderState { get; init; } = string.Empty;

    public int SubjectActivationIndex { get; init; } = -1;

    public string EvidenceReference { get; init; } = string.Empty;

    public DateTimeOffset EffectiveTime { get; init; }

    public DateTimeOffset Expiry { get; init; }
}

public sealed record RestrictionReleaseEvidence
{
    public string ReleaseDecisionId { get; init; } = string.Empty;

    public string RestrictionId { get; init; } = string.Empty;

    public string SubjectId { get; init; } = string.Empty;

    public string TransitionRequestId { get; init; } = string.Empty;

    public string ReleaseAuthority { get; init; } = string.Empty;

    public string ReleaseConditionsEvidence { get; init; } = string.Empty;

    public string IndependentValidationEvidence { get; init; } = string.Empty;

    public string NewAuthorityDecisionReference { get; init; } = string.Empty;

    public string ReleaseState { get; init; } = string.Empty;

    public DateTimeOffset EffectiveTime { get; init; }

    public DateTimeOffset Expiry { get; init; }
}

public sealed record RecoveryValidationEvidence
{
    public string ValidationId { get; init; } = string.Empty;

    public string SubjectId { get; init; } = string.Empty;

    public string TransitionRequestId { get; init; } = string.Empty;

    public string BootstrapContextId { get; init; } = string.Empty;

    public string ValidatorAuthority { get; init; } = string.Empty;

    public string AuthorityDecisionReference { get; init; } = string.Empty;

    public string ValidationResult { get; init; } = string.Empty;

    public string EvidenceReference { get; init; } = string.Empty;

    public DateTimeOffset EffectiveTime { get; init; }

    public DateTimeOffset Expiry { get; init; }
}

public sealed record CanonicalBootstrapPolicy(
    string PolicyId,
    string Version,
    string BootstrapAuthority,
    string EnvironmentIdentity,
    string Scope,
    string SourceIdentity,
    string ProvenanceAuthority,
    string SubjectEvidenceAuthority,
    string TimeProviderId,
    string DependencyGraphId,
    string DependencyGraphVersion,
    string DependencyGraphDigest,
    int SubjectActivationIndex,
    string AuthorityBoundary,
    string LifecycleAuthorityPolicy,
    string RecoveryValidatorAuthority,
    DateTimeOffset EffectiveTime,
    DateTimeOffset Expiry);

public static class BootstrapPolicyCatalog
{
    private static readonly CanonicalBootstrapPolicy Stage3Wp05Policy = new(
        "STAGE3-WP05-CANONICAL-BOOTSTRAP-POLICY",
        "1.0",
        "FALCON-STAGE3-WP05-AUTHORITY",
        "ENV-STAGE3-WP05-ISOLATED",
        "STAGE3-WP05-BOOTSTRAP-AND-LIFECYCLE",
        "EXTERNAL-BOOTSTRAP-CONTROL",
        "FALCON-STAGE3-WP05-PROVENANCE-AUTHORITY",
        "FALCON-STAGE3-WP05-EVIDENCE",
        "TIME-PROVIDER-ACTIVE-001",
        "stage3-wp04-golden-graph",
        "1.0",
        "D06C6EDE16D2A55F4FBA36B965C5EECA0A98CE5AE11CE711ABCB4E8FECFF992E",
        4,
        "NO-PRODUCTION-NO-FINANCIAL-NO-EXTERNAL-CONNECTIVITY",
        "GOV-097",
        "FALCON-INDEPENDENT-RECOVERY-VALIDATOR",
        DateTimeOffset.MinValue,
        DateTimeOffset.MaxValue);

    public static CanonicalBootstrapPolicy GetStage3Wp05Policy()
        => Stage3Wp05Policy;
}

public sealed record BootstrapValidationRequest
{
    public string RequestId { get; init; } = string.Empty;

    public BootstrapSubjectAdmissionEvidence Subject { get; init; } = null!;

    public BootstrapExecutionContextRecord Context { get; init; } = null!;

    public BootstrapEvidenceProvenanceRecord Provenance { get; init; } = null!;

    public TimeProviderRecord TimeProvider { get; init; } = null!;

    public DependencyActivationEvidence DependencyEvidence { get; init; } = null!;

    public RestrictionRecord? Restriction { get; init; }

    public RestrictionReleaseEvidence? RestrictionRelease { get; init; }

    public DateTimeOffset ObservationTime { get; init; }
}

public sealed record BootstrapValidationResult(
    string Decision,
    string ReasonCode,
    string DecisionIdentity,
    string PolicyId,
    string PolicyVersion,
    string SubjectId,
    string SubjectVersion,
    BootstrapSubjectKind SubjectKind,
    string InitialLifecycleState,
    string BootstrapContextId,
    string ProvenanceId,
    string DependencyEvidenceReference,
    bool RestrictionActive,
    string ActiveRestrictionId,
    bool RestrictionReleaseValidated,
    DateTimeOffset ValidUntil,
    DateTimeOffset DecisionTime)
{
    public bool Accepted => string.Equals(Decision, "ACCEPTED", StringComparison.Ordinal);
}

public static class BootstrapContextGate
{
    public static BootstrapValidationResult Validate(BootstrapValidationRequest? request)
    {
        var policy = BootstrapPolicyCatalog.GetStage3Wp05Policy();
        if (request is null)
        {
            return Reject(new BootstrapValidationRequest(), policy, "INVALID_BOOTSTRAP_REQUEST");
        }

        if (string.IsNullOrWhiteSpace(request.RequestId) ||
            request.Subject is null ||
            request.Context is null ||
            request.Provenance is null ||
            request.TimeProvider is null ||
            request.DependencyEvidence is null ||
            request.ObservationTime == default)
        {
            return Reject(request, policy, "INVALID_BOOTSTRAP_REQUEST");
        }

        var subjectFailure = ValidateSubject(request.Subject, policy, request.ObservationTime);
        if (subjectFailure is not null)
        {
            return Reject(request, policy, subjectFailure);
        }

        if (BootstrapContractValidators.Validate(request.Context).Result != ValidationResult.Pass)
        {
            return Reject(request, policy, "INVALID_BOOTSTRAP_CONTEXT");
        }

        if (!ContainsTime(request.Context.EffectiveTime, request.Context.Expiry, request.ObservationTime))
        {
            return Reject(request, policy, "BOOTSTRAP_CONTEXT_NOT_EFFECTIVE");
        }

        if (!string.Equals(request.Context.BootstrapAuthority, policy.BootstrapAuthority, StringComparison.Ordinal))
        {
            return Reject(request, policy, "BOOTSTRAP_AUTHORITY_MISMATCH");
        }

        if (!string.Equals(request.Context.EnvironmentIdentity, policy.EnvironmentIdentity, StringComparison.Ordinal))
        {
            return Reject(request, policy, "BOOTSTRAP_ENVIRONMENT_MISMATCH");
        }

        if (!string.Equals(request.Context.Scope, policy.Scope, StringComparison.Ordinal))
        {
            return Reject(request, policy, "BOOTSTRAP_SCOPE_MISMATCH");
        }

        if (!string.Equals(request.Context.SourceIdentity, policy.SourceIdentity, StringComparison.Ordinal))
        {
            return Reject(request, policy, "BOOTSTRAP_SOURCE_MISMATCH");
        }

        if (!string.Equals(request.Context.AuthorityBoundary, policy.AuthorityBoundary, StringComparison.Ordinal))
        {
            return Reject(request, policy, "BOOTSTRAP_AUTHORITY_BOUNDARY_MISMATCH");
        }

        if (BootstrapContractValidators.Validate(request.Provenance).Result != ValidationResult.Pass)
        {
            return Reject(request, policy, "INVALID_BOOTSTRAP_PROVENANCE");
        }

        if (!ContainsTime(request.Provenance.EffectiveTime, request.Provenance.Expiry, request.ObservationTime))
        {
            return Reject(request, policy, "BOOTSTRAP_PROVENANCE_NOT_EFFECTIVE");
        }

        var expectedSourceRecordId =
            $"source-record:{request.Subject.SubjectId}:{request.Subject.SubjectVersion}";

        if (!string.Equals(request.Provenance.SourceRecordId, expectedSourceRecordId, StringComparison.Ordinal) ||
            !string.Equals(request.Provenance.SourceIdentity, policy.SourceIdentity, StringComparison.Ordinal) ||
            !string.Equals(request.Provenance.ProvenanceAuthority, policy.ProvenanceAuthority, StringComparison.Ordinal))
        {
            return Reject(request, policy, "BOOTSTRAP_PROVENANCE_BINDING_MISMATCH");
        }

        if (!string.Equals(
                request.Provenance.SourceDigest,
                request.Subject.ArtifactDigest,
                StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(
                request.Provenance.ArtifactIdentity,
                request.Subject.ArtifactIdentity,
                StringComparison.Ordinal))
        {
            return Reject(request, policy, "BOOTSTRAP_ARTIFACT_PROVENANCE_MISMATCH");
        }

        if (ProviderContractValidators.Validate(request.TimeProvider).Result != ValidationResult.Pass)
        {
            return Reject(request, policy, "INVALID_TIME_PROVIDER");
        }

        if (!ContainsTime(
                request.TimeProvider.EffectiveTime,
                request.TimeProvider.Expiry,
                request.ObservationTime))
        {
            return Reject(request, policy, "TIME_PROVIDER_NOT_EFFECTIVE");
        }

        if (!string.Equals(request.TimeProvider.ProviderId, policy.TimeProviderId, StringComparison.Ordinal))
        {
            return Reject(request, policy, "TIME_PROVIDER_IDENTITY_MISMATCH");
        }

        var dependencyFailure = ValidateDependencyEvidence(
            request.Subject,
            request.DependencyEvidence,
            policy,
            request.ObservationTime);

        if (dependencyFailure is not null)
        {
            return Reject(request, policy, dependencyFailure);
        }

        var restrictionActive = false;
        var activeRestrictionId = string.Empty;
        var releaseValidated = false;
        var initialState = "INITIALIZING";
        var acceptedReason = "BOOTSTRAP_ACCEPTED";

        if (request.Restriction is not null)
        {
            var restrictionFailure = ValidateRestriction(
                request.Restriction,
                request.Subject.SubjectId,
                request.ObservationTime);

            if (restrictionFailure is not null)
            {
                return Reject(request, policy, restrictionFailure);
            }

            if (request.RestrictionRelease is null)
            {
                restrictionActive = true;
                activeRestrictionId = request.Restriction.RestrictionId;
                initialState = "RESTRICTED";
                acceptedReason = "BOOTSTRAP_ACCEPTED_RESTRICTED";
            }
            else
            {
                var releaseFailure = ValidateRestrictionRelease(
                    request.Restriction,
                    request.RestrictionRelease,
                    request.RequestId,
                    "OWNER-APPROVAL-GOV-097-20260803",
                    request.ObservationTime);

                if (releaseFailure is not null)
                {
                    return Reject(request, policy, releaseFailure);
                }

                releaseValidated = true;
                acceptedReason = "BOOTSTRAP_ACCEPTED_AFTER_CONTROLLED_RELEASE";
            }
        }
        else if (request.RestrictionRelease is not null)
        {
            return Reject(request, policy, "RELEASE_WITHOUT_RESTRICTION");
        }

        var validUntil = Earliest(
            policy.Expiry,
            request.Subject.Expiry,
            request.Context.Expiry,
            request.Provenance.Expiry,
            request.TimeProvider.Expiry,
            request.DependencyEvidence.Expiry,
            request.Restriction?.Expiry,
            request.RestrictionRelease?.Expiry);

        return CreateResult(
            request,
            policy,
            "ACCEPTED",
            acceptedReason,
            initialState,
            restrictionActive,
            activeRestrictionId,
            releaseValidated,
            validUntil);
    }

    internal static string? ValidateDependencyEvidence(
        BootstrapSubjectAdmissionEvidence subject,
        DependencyActivationEvidence evidence,
        CanonicalBootstrapPolicy policy,
        DateTimeOffset observationTime)
    {
        if (string.IsNullOrWhiteSpace(evidence.SubjectId) ||
            string.IsNullOrWhiteSpace(evidence.SubjectVersion) ||
            string.IsNullOrWhiteSpace(evidence.GraphId) ||
            string.IsNullOrWhiteSpace(evidence.GraphVersion) ||
            !IsHexDigest(evidence.GraphDigest) ||
            string.IsNullOrWhiteSpace(evidence.EvidenceReference) ||
            evidence.SubjectActivationIndex < 0)
        {
            return "INCOMPLETE_DEPENDENCY_EVIDENCE";
        }

        if (!string.Equals(evidence.SubjectId, subject.SubjectId, StringComparison.Ordinal) ||
            !string.Equals(evidence.SubjectVersion, subject.SubjectVersion, StringComparison.Ordinal))
        {
            return "DEPENDENCY_SUBJECT_MISMATCH";
        }

        if (!string.Equals(
                evidence.EvidenceReference,
                $"dependency-evidence:{subject.SubjectId}",
                StringComparison.Ordinal))
        {
            return "DEPENDENCY_EVIDENCE_REFERENCE_MISMATCH";
        }

        if (!string.Equals(evidence.GraphId, policy.DependencyGraphId, StringComparison.Ordinal) ||
            !string.Equals(evidence.GraphVersion, policy.DependencyGraphVersion, StringComparison.Ordinal) ||
            !string.Equals(
                evidence.GraphDigest,
                policy.DependencyGraphDigest,
                StringComparison.OrdinalIgnoreCase) ||
            evidence.SubjectActivationIndex != policy.SubjectActivationIndex)
        {
            return "DEPENDENCY_EVIDENCE_BINDING_MISMATCH";
        }

        if (!string.Equals(evidence.DependencyValidationState, "VALIDATED", StringComparison.Ordinal))
        {
            return "DEPENDENCY_GRAPH_NOT_VALIDATED";
        }

        if (!string.Equals(evidence.ActivationOrderState, "VALIDATED", StringComparison.Ordinal))
        {
            return "ACTIVATION_ORDER_NOT_VALIDATED";
        }

        if (!ContainsTime(evidence.EffectiveTime, evidence.Expiry, observationTime))
        {
            return "DEPENDENCY_EVIDENCE_NOT_EFFECTIVE";
        }

        return null;
    }

    internal static string? ValidateRestriction(
        RestrictionRecord restriction,
        string subjectId,
        DateTimeOffset observationTime)
    {
        if (ContractValidators.Validate(restriction).Result != ValidationResult.Pass)
        {
            return "INVALID_PROTECTIVE_RESTRICTION";
        }

        if (!string.Equals(restriction.SubjectId, subjectId, StringComparison.Ordinal))
        {
            return "RESTRICTION_SUBJECT_MISMATCH";
        }

        if (!ContainsTime(restriction.EffectiveTime, restriction.Expiry, observationTime))
        {
            return "PROTECTIVE_RESTRICTION_NOT_EFFECTIVE";
        }

        return null;
    }

    internal static string? ValidateRestrictionRelease(
        RestrictionRecord restriction,
        RestrictionReleaseEvidence release,
        string expectedTransitionRequestId,
        string expectedAuthorityDecisionId,
        DateTimeOffset observationTime)
    {
        if (string.IsNullOrWhiteSpace(release.ReleaseDecisionId) ||
            string.IsNullOrWhiteSpace(release.RestrictionId) ||
            string.IsNullOrWhiteSpace(release.SubjectId) ||
            string.IsNullOrWhiteSpace(release.TransitionRequestId) ||
            string.IsNullOrWhiteSpace(release.ReleaseAuthority) ||
            string.IsNullOrWhiteSpace(release.ReleaseConditionsEvidence) ||
            string.IsNullOrWhiteSpace(release.IndependentValidationEvidence) ||
            string.IsNullOrWhiteSpace(release.NewAuthorityDecisionReference))
        {
            return "INCOMPLETE_RESTRICTION_RELEASE";
        }

        if (!string.Equals(release.ReleaseState, "RELEASED", StringComparison.Ordinal))
        {
            return "RESTRICTION_NOT_RELEASED";
        }

        var expectedReleaseDecisionId =
            $"release:{restriction.RestrictionId}:{expectedTransitionRequestId}";
        if (!string.Equals(
                release.ReleaseDecisionId,
                expectedReleaseDecisionId,
                StringComparison.Ordinal) ||
            !string.Equals(
                release.ReleaseConditionsEvidence,
                "RELEASE-CONDITIONS-SATISFIED-001",
                StringComparison.Ordinal) ||
            !string.Equals(
                release.IndependentValidationEvidence,
                "INDEPENDENT-VALIDATION-001",
                StringComparison.Ordinal))
        {
            return "RESTRICTION_RELEASE_RECORD_MISMATCH";
        }

        if (!string.Equals(release.RestrictionId, restriction.RestrictionId, StringComparison.Ordinal) ||
            !string.Equals(release.SubjectId, restriction.SubjectId, StringComparison.Ordinal) ||
            !string.Equals(release.ReleaseAuthority, restriction.ReleaseAuthority, StringComparison.Ordinal) ||
            !string.Equals(
                release.TransitionRequestId,
                expectedTransitionRequestId,
                StringComparison.Ordinal))
        {
            return "RESTRICTION_RELEASE_BINDING_MISMATCH";
        }

        if (!string.IsNullOrWhiteSpace(expectedAuthorityDecisionId) &&
            !string.Equals(
                release.NewAuthorityDecisionReference,
                expectedAuthorityDecisionId,
                StringComparison.Ordinal))
        {
            return "RESTRICTION_RELEASE_AUTHORITY_MISMATCH";
        }

        if (release.EffectiveTime < restriction.EffectiveTime ||
            !ContainsTime(release.EffectiveTime, release.Expiry, observationTime))
        {
            return "RESTRICTION_RELEASE_NOT_EFFECTIVE";
        }

        return null;
    }

    private static string? ValidateSubject(
        BootstrapSubjectAdmissionEvidence subject,
        CanonicalBootstrapPolicy policy,
        DateTimeOffset observationTime)
    {
        if (!Enum.IsDefined(typeof(BootstrapSubjectKind), subject.SubjectKind))
        {
            return "INVALID_SUBJECT_KIND";
        }

        if (string.IsNullOrWhiteSpace(subject.SubjectId) ||
            string.IsNullOrWhiteSpace(subject.SubjectVersion) ||
            string.IsNullOrWhiteSpace(subject.ArtifactIdentity) ||
            !IsHexDigest(subject.ArtifactDigest) ||
            string.IsNullOrWhiteSpace(subject.ManifestIdentity) ||
            !IsHexDigest(subject.ManifestDigest) ||
            string.IsNullOrWhiteSpace(subject.AdmissionDecisionId) ||
            string.IsNullOrWhiteSpace(subject.EvidenceAuthority) ||
            string.IsNullOrWhiteSpace(subject.RegistrationEvidenceId))
        {
            return "INCOMPLETE_SUBJECT_ADMISSION_EVIDENCE";
        }

        if (!string.Equals(
                subject.EvidenceAuthority,
                policy.SubjectEvidenceAuthority,
                StringComparison.Ordinal))
        {
            return "SUBJECT_EVIDENCE_AUTHORITY_MISMATCH";
        }

        var expectedArtifactIdentity =
            $"artifact:{subject.SubjectId}:{subject.SubjectVersion}";
        var expectedManifestIdentity =
            $"manifest:{subject.SubjectId}:{subject.SubjectVersion}";
        var expectedAdmissionDecisionId =
            $"admission:{subject.SubjectId}:{subject.SubjectVersion}";
        var expectedRegistrationEvidenceId = subject.SubjectKind == BootstrapSubjectKind.Service
            ? $"registration:{subject.SubjectId}:{subject.SubjectVersion}"
            : $"not-applicable:{subject.SubjectId}:{subject.SubjectVersion}";

        if (!string.Equals(subject.ArtifactIdentity, expectedArtifactIdentity, StringComparison.Ordinal) ||
            !string.Equals(subject.ManifestIdentity, expectedManifestIdentity, StringComparison.Ordinal) ||
            !string.Equals(subject.AdmissionDecisionId, expectedAdmissionDecisionId, StringComparison.Ordinal) ||
            !string.Equals(
                subject.RegistrationEvidenceId,
                expectedRegistrationEvidenceId,
                StringComparison.Ordinal))
        {
            return "SUBJECT_ADMISSION_BINDING_MISMATCH";
        }

        if (!string.Equals(subject.AdmissionState, "ADMITTED", StringComparison.Ordinal))
        {
            return "SUBJECT_NOT_ADMITTED";
        }

        if (subject.SubjectKind == BootstrapSubjectKind.Service &&
            !string.Equals(subject.RegistrationState, "REGISTERED", StringComparison.Ordinal))
        {
            return "SERVICE_NOT_REGISTERED";
        }

        if (subject.SubjectKind == BootstrapSubjectKind.Plugin &&
            !string.Equals(subject.RegistrationState, "NOT_APPLICABLE", StringComparison.Ordinal))
        {
            return "PLUGIN_REGISTRATION_STATE_INVALID";
        }

        if (!ContainsTime(subject.EffectiveTime, subject.Expiry, observationTime))
        {
            return "SUBJECT_ADMISSION_NOT_EFFECTIVE";
        }

        return null;
    }

    private static BootstrapValidationResult Reject(
        BootstrapValidationRequest request,
        CanonicalBootstrapPolicy policy,
        string reasonCode)
        => CreateResult(
            request,
            policy,
            "REJECTED",
            reasonCode,
            "STOPPED",
            false,
            string.Empty,
            false,
            request.ObservationTime);

    private static BootstrapValidationResult CreateResult(
        BootstrapValidationRequest request,
        CanonicalBootstrapPolicy policy,
        string decision,
        string reasonCode,
        string initialState,
        bool restrictionActive,
        string activeRestrictionId,
        bool releaseValidated,
        DateTimeOffset validUntil)
    {
        var subjectId = request.Subject?.SubjectId ?? string.Empty;
        var subjectVersion = request.Subject?.SubjectVersion ?? string.Empty;
        var subjectKind = request.Subject?.SubjectKind ?? default;
        var contextId = request.Context?.ContextId ?? string.Empty;
        var provenanceId = request.Provenance?.ProvenanceId ?? string.Empty;
        var dependencyEvidence = request.DependencyEvidence?.EvidenceReference ?? string.Empty;

        var subject = request.Subject;
        var context = request.Context;
        var provenance = request.Provenance;
        var timeProvider = request.TimeProvider;
        var dependency = request.DependencyEvidence;
        var restriction = request.Restriction;
        var release = request.RestrictionRelease;

        var canonical = CanonicalEncoding.Build(
            policy.PolicyId,
            policy.Version,
            policy.BootstrapAuthority,
            policy.EnvironmentIdentity,
            policy.Scope,
            policy.SourceIdentity,
            policy.ProvenanceAuthority,
            policy.SubjectEvidenceAuthority,
            policy.TimeProviderId,
            policy.DependencyGraphId,
            policy.DependencyGraphVersion,
            policy.DependencyGraphDigest,
            policy.SubjectActivationIndex,
            policy.AuthorityBoundary,
            request.RequestId,
            decision,
            reasonCode,
            initialState,
            restrictionActive,
            activeRestrictionId,
            releaseValidated,
            validUntil,
            request.ObservationTime,
            subject?.SubjectId,
            subject?.SubjectVersion,
            subject?.SubjectKind,
            subject?.ArtifactIdentity,
            subject?.ArtifactDigest,
            subject?.ManifestIdentity,
            subject?.ManifestDigest,
            subject?.AdmissionDecisionId,
            subject?.AdmissionState,
            subject?.RegistrationEvidenceId,
            subject?.RegistrationState,
            subject?.EvidenceAuthority,
            subject?.EffectiveTime,
            subject?.Expiry,
            context?.ContextId,
            context?.Version,
            context?.BootstrapAuthority,
            context?.EnvironmentIdentity,
            context?.Scope,
            context?.SourceIdentity,
            context?.ValidationEvidence,
            context?.ContextState,
            context?.AuthorityBoundary,
            context?.EffectiveTime,
            context?.Expiry,
            provenance?.ProvenanceId,
            provenance?.Version,
            provenance?.SourceRecordId,
            provenance?.SourceDigest,
            provenance?.SourceIdentity,
            provenance?.ProvenanceAuthority,
            provenance?.ValidationEvidence,
            provenance?.ProvenanceState,
            provenance?.ArtifactIdentity,
            provenance?.EffectiveTime,
            provenance?.Expiry,
            timeProvider?.ProviderId,
            timeProvider?.Version,
            timeProvider?.ProviderClass,
            timeProvider?.AdmissionAuthority,
            timeProvider?.Boundaries,
            timeProvider?.SourceOfTime,
            timeProvider?.ValidationEvidence,
            timeProvider?.AdmissionResult,
            timeProvider?.EffectiveTime,
            timeProvider?.Expiry,
            dependency?.SubjectId,
            dependency?.SubjectVersion,
            dependency?.GraphId,
            dependency?.GraphVersion,
            dependency?.GraphDigest,
            dependency?.DependencyValidationState,
            dependency?.ActivationOrderState,
            dependency?.SubjectActivationIndex,
            dependency?.EvidenceReference,
            dependency?.EffectiveTime,
            dependency?.Expiry,
            restriction?.RestrictionId,
            restriction?.SubjectId,
            restriction?.ReleaseAuthority,
            restriction?.IntegrityEvidence,
            restriction?.EffectiveTime,
            restriction?.Expiry,
            release?.ReleaseDecisionId,
            release?.RestrictionId,
            release?.SubjectId,
            release?.TransitionRequestId,
            release?.ReleaseAuthority,
            release?.NewAuthorityDecisionReference,
            release?.ReleaseState,
            release?.EffectiveTime,
            release?.Expiry);

        var decisionIdentity = Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(canonical)));

        return new BootstrapValidationResult(
            decision,
            reasonCode,
            decisionIdentity,
            policy.PolicyId,
            policy.Version,
            subjectId,
            subjectVersion,
            subjectKind,
            initialState,
            contextId,
            provenanceId,
            dependencyEvidence,
            restrictionActive,
            activeRestrictionId,
            releaseValidated,
            validUntil,
            request.ObservationTime);
    }

    private static DateTimeOffset Earliest(
        DateTimeOffset first,
        params DateTimeOffset?[] values)
    {
        var earliest = first;

        foreach (var value in values)
        {
            if (value.HasValue && value.Value < earliest)
            {
                earliest = value.Value;
            }
        }

        return earliest;
    }

    internal static bool ContainsTime(
        DateTimeOffset effectiveTime,
        DateTimeOffset expiry,
        DateTimeOffset observationTime)
        => effectiveTime != default
           && expiry > effectiveTime
           && observationTime >= effectiveTime
           && observationTime < expiry;

    internal static bool IsHexDigest(string value)
        => !string.IsNullOrWhiteSpace(value) &&
           value.Length == 64 &&
           value.All(character =>
               (character >= '0' && character <= '9') ||
               (character >= 'A' && character <= 'F') ||
               (character >= 'a' && character <= 'f'));
}

public sealed record LifecycleTransitionEvidence
{
    public string TransitionId { get; init; } = string.Empty;

    public string EventId { get; init; } = string.Empty;

    public string ModelId { get; init; } = string.Empty;

    public string ModelVersion { get; init; } = string.Empty;

    public long ExpectedStateVersion { get; init; }

    public string BootstrapContextId { get; init; } = string.Empty;

    public string ValidationEvidence { get; init; } = string.Empty;

    public DateTimeOffset ObservationTime { get; init; }

    public AuthorityResult AuthorityDecision { get; init; } = null!;

    public TimeProviderRecord TimeProvider { get; init; } = null!;

    public DependencyActivationEvidence? DependencyEvidence { get; init; }

    public RestrictionRecord? Restriction { get; init; }

    public RestrictionReleaseEvidence? RestrictionRelease { get; init; }

    public RecoveryValidationEvidence? RecoveryValidation { get; init; }
}

public static class LifecycleEvidenceBinding
{
    public static string Compute(
        LifecycleTransitionRequest? request,
        LifecycleTransitionEvidence? evidence)
    {
        if (request is null || evidence is null) return string.Empty;

        var authority = evidence.AuthorityDecision;
        var timeProvider = evidence.TimeProvider;
        var dependency = evidence.DependencyEvidence;
        var restriction = evidence.Restriction;
        var release = evidence.RestrictionRelease;
        var recovery = evidence.RecoveryValidation;

        var canonical = CanonicalEncoding.Build(
            request.TransitionRequestId,
            request.ComponentIdentity,
            request.AuthoritativeSourceState,
            request.RequestedTargetState,
            request.AuthorityReference,
            request.DependencyContext,
            request.RequestTime,
            request.Expiry,
            evidence.TransitionId,
            evidence.EventId,
            evidence.ModelId,
            evidence.ModelVersion,
            evidence.ExpectedStateVersion,
            evidence.BootstrapContextId,
            evidence.ObservationTime,
            authority?.RequestId,
            authority?.DecisionId,
            authority?.Decision,
            authority?.EffectiveScope,
            authority?.ControllingPolicy,
            authority?.PolicyVersion,
            authority?.DecisionTime,
            authority?.Expiry,
            authority?.EvidenceReference,
            timeProvider?.ProviderId,
            timeProvider?.Version,
            timeProvider?.AdmissionAuthority,
            timeProvider?.ValidationEvidence,
            timeProvider?.EffectiveTime,
            timeProvider?.Expiry,
            dependency?.SubjectId,
            dependency?.SubjectVersion,
            dependency?.GraphId,
            dependency?.GraphVersion,
            dependency?.GraphDigest,
            dependency?.EvidenceReference,
            dependency?.EffectiveTime,
            dependency?.Expiry,
            restriction?.RestrictionId,
            restriction?.SubjectId,
            restriction?.ReleaseAuthority,
            restriction?.IntegrityEvidence,
            restriction?.EffectiveTime,
            restriction?.Expiry,
            release?.ReleaseDecisionId,
            release?.RestrictionId,
            release?.SubjectId,
            release?.TransitionRequestId,
            release?.NewAuthorityDecisionReference,
            release?.ReleaseState,
            release?.EffectiveTime,
            release?.Expiry,
            recovery?.ValidationId,
            recovery?.SubjectId,
            recovery?.TransitionRequestId,
            recovery?.BootstrapContextId,
            recovery?.ValidatorAuthority,
            recovery?.AuthorityDecisionReference,
            recovery?.ValidationResult,
            recovery?.EvidenceReference,
            recovery?.EffectiveTime,
            recovery?.Expiry);

        return Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(canonical)));
    }
}

public sealed record LifecycleContractRejection(
    string RequestId,
    string TransitionId,
    string EventId,
    string SubjectId,
    string ReasonCode,
    DateTimeOffset ObservationTime,
    string EvidenceReference);

public sealed record LifecycleControlDecision(
    LifecycleTransitionResult ContractResult,
    LifecycleStateSnapshot? Snapshot,
    LifecycleTransitionAttempt? Attempt,
    LifecycleTransitionEvent? Event);

public sealed record LifecycleAuthorityEvaluation(
    string SecurityContext,
    string RequiredFitnessToOperate,
    AuthorityEvaluationContext Context);

public sealed record LifecycleAuthorityControlDecision(
    AuthorityRequest AuthorityRequest,
    AuthorityResult AuthorityResult,
    LifecycleControlDecision LifecycleDecision);

public enum LifecycleAuthorityMode
{
    LegacyStage3Compatibility = 0,
    AuthorityEngineRequired = 1
}

public sealed record BootstrapLifecycleRegistrationOutcome(
    BootstrapValidationResult BootstrapResult,
    LifecycleRegistrationResult Registration);

public enum LifecycleRestartDisposition
{
    Continued,
    Restricted,
    ChallengeRequired
}

public sealed record LifecycleRestartOutcome(
    LifecycleRestartDisposition Disposition,
    string Reason,
    ReconciliationResult Reconciliation,
    LifecycleControlService? Service)
{
    public bool ContinuationAllowed =>
        Disposition == LifecycleRestartDisposition.Continued &&
        Service is not null;
}

public sealed class LifecycleControlService
{
    private sealed record RegistrationReplay(
        string SubjectId,
        string BootstrapContextId,
        DateTimeOffset BootstrapValidUntil,
        bool ProtectiveRestrictionActive,
        string ActiveRestrictionId,
        string EvidenceReference,
        DateTimeOffset EffectiveTime);

    private sealed record IdentityReplay(
        string RequestId,
        string TransitionId,
        string EventId);

    private readonly object _sync = new();
    private readonly object _commitSync = new();
    private LifecycleController _controller;
    private readonly LifecycleStateModel _model;
    private readonly Dictionary<string, BootstrapValidationResult> _bootstrapDecisions = new(StringComparer.Ordinal);
    private readonly Dictionary<string, BootstrapSubjectAdmissionEvidence> _subjects = new(StringComparer.Ordinal);
    private readonly Dictionary<string, RestrictionRecord> _activeRestrictions = new(StringComparer.Ordinal);
    private readonly List<LifecycleContractRejection> _contractRejections = new();
    private readonly LifecycleAuthorityMode _authorityMode;
    private readonly DurableAuthoritativeStateStore? _stateStore;
    private readonly IntegrityLinkedEvidenceJournal? _evidenceJournal;
    private readonly AcceptedFactPublisher? _acceptedFactPublisher;
    private readonly RestartReconciler? _restartReconciler;
    private readonly bool _restartVerified;
    private readonly List<RegistrationReplay> _committedRegistrations = new();
    private readonly List<IdentityReplay> _reservedIdentities = new();
    private readonly List<LifecycleTransitionCommand> _committedTransitions = new();
    private readonly HashSet<string> _evidenceBlockedSubjects = new(StringComparer.Ordinal);

    public LifecycleControlService(LifecycleStateModel model)
        : this(model, LifecycleAuthorityMode.LegacyStage3Compatibility)
    {
    }

    public LifecycleControlService(
        LifecycleStateModel model,
        LifecycleAuthorityMode authorityMode)
        : this(model, authorityMode, null)
    {
    }

    public LifecycleControlService(
        LifecycleStateModel model,
        LifecycleAuthorityMode authorityMode,
        DurableAuthoritativeStateStore? stateStore)
        : this(model, authorityMode, stateStore, null, null)
    {
    }

    public LifecycleControlService(
        LifecycleStateModel model,
        LifecycleAuthorityMode authorityMode,
        DurableAuthoritativeStateStore? stateStore,
        IntegrityLinkedEvidenceJournal? evidenceJournal,
        AcceptedFactPublisher? acceptedFactPublisher)
        : this(model, authorityMode, stateStore, evidenceJournal, acceptedFactPublisher, null)
    {
    }

    public LifecycleControlService(
        LifecycleStateModel model,
        LifecycleAuthorityMode authorityMode,
        DurableAuthoritativeStateStore? stateStore,
        IntegrityLinkedEvidenceJournal? evidenceJournal,
        AcceptedFactPublisher? acceptedFactPublisher,
        RestartReconciler? restartReconciler)
        : this(
            model,
            authorityMode,
            stateStore,
            evidenceJournal,
            acceptedFactPublisher,
            restartReconciler,
            restartVerified: false)
    {
    }

    private LifecycleControlService(
        LifecycleStateModel model,
        LifecycleAuthorityMode authorityMode,
        DurableAuthoritativeStateStore? stateStore,
        IntegrityLinkedEvidenceJournal? evidenceJournal,
        AcceptedFactPublisher? acceptedFactPublisher,
        RestartReconciler? restartReconciler,
        bool restartVerified)
    {
        _model = model ?? throw new ArgumentNullException(nameof(model));
        _authorityMode = authorityMode;
        _stateStore = stateStore;
        _evidenceJournal = evidenceJournal;
        _acceptedFactPublisher = acceptedFactPublisher;
        _restartReconciler = restartReconciler;
        _restartVerified = restartVerified;
        _controller = new LifecycleController();
        RestoreEvidenceCompletionBlocks();
    }

    public static LifecycleRestartOutcome Restart(
        LifecycleStateModel model,
        LifecycleAuthorityMode authorityMode,
        DurableAuthoritativeStateStore? stateStore,
        IntegrityLinkedEvidenceJournal? evidenceJournal,
        AcceptedFactPublisher? acceptedFactPublisher,
        RestartReconciler? restartReconciler,
        ReconciliationRequest? request)
    {
        if (restartReconciler is null || request is null)
        {
            var failed = new ReconciliationResult(
                ReconciliationClassification.FailedClosed,
                "RESTART_RECONCILER_AND_REQUEST_REQUIRED",
                null, null, null, false, true);
            return new LifecycleRestartOutcome(
                LifecycleRestartDisposition.ChallengeRequired,
                failed.Reason,
                failed,
                null);
        }

        if (!restartReconciler.MatchesStores(
                stateStore,
                evidenceJournal,
                acceptedFactPublisher))
        {
            var failed = new ReconciliationResult(
                ReconciliationClassification.FailedClosed,
                "RESTART_STORE_BINDING_MISMATCH",
                null, null, null, false, true);
            return new LifecycleRestartOutcome(
                LifecycleRestartDisposition.ChallengeRequired,
                failed.Reason,
                failed,
                null);
        }

        var reconciliation = restartReconciler.Reconcile(request);
        if (!reconciliation.ContinuationAllowed || reconciliation.ChallengeRequired)
        {
            return new LifecycleRestartOutcome(
                reconciliation.ChallengeRequired
                    ? LifecycleRestartDisposition.ChallengeRequired
                    : LifecycleRestartDisposition.Restricted,
                reconciliation.Reason,
                reconciliation,
                null);
        }

        var service = new LifecycleControlService(
            model,
            authorityMode,
            stateStore,
            evidenceJournal,
            acceptedFactPublisher,
            restartReconciler,
            restartVerified: true);

        return new LifecycleRestartOutcome(
            LifecycleRestartDisposition.Continued,
            reconciliation.Reason,
            reconciliation,
            service);
    }

    public LifecycleRestartOutcome ReconcileRestart(ReconciliationRequest? request)
        => Restart(
            _model,
            _authorityMode,
            _stateStore,
            _evidenceJournal,
            _acceptedFactPublisher,
            _restartReconciler,
            request);

    public BootstrapLifecycleRegistrationOutcome Register(
        BootstrapValidationRequest? bootstrapRequest,
        string? evidenceReference)
    {
        lock (_commitSync)
        {
            var bootstrapResult = BootstrapContextGate.Validate(bootstrapRequest);

            if (!bootstrapResult.Accepted || bootstrapRequest is null)
            {
                return new BootstrapLifecycleRegistrationOutcome(
                    bootstrapResult,
                    new LifecycleRegistrationResult(
                        false,
                        "BOOTSTRAP_DECISION_NOT_ACCEPTED",
                        null));
            }

            if (!string.Equals(
                    bootstrapResult.InitialLifecycleState,
                    "INITIALIZING",
                    StringComparison.Ordinal) &&
                !string.Equals(
                    bootstrapResult.InitialLifecycleState,
                    "RESTRICTED",
                    StringComparison.Ordinal))
            {
                return new BootstrapLifecycleRegistrationOutcome(
                    bootstrapResult,
                    new LifecycleRegistrationResult(
                        false,
                        "INVALID_BOOTSTRAP_INITIAL_STATE",
                        null));
            }

            var registration = _controller.RegisterSubject(
                bootstrapResult.SubjectId,
                _model,
                bootstrapResult.BootstrapContextId,
                bootstrapResult.ValidUntil,
                bootstrapResult.RestrictionActive,
                bootstrapResult.ActiveRestrictionId,
                evidenceReference ?? string.Empty,
                bootstrapResult.DecisionTime);

            if (!registration.Accepted || registration.Snapshot is null)
            {
                return new BootstrapLifecycleRegistrationOutcome(
                    bootstrapResult,
                    registration);
            }

            if (_stateStore is not null)
            {
                var persistence = PersistLifecycleSnapshot(
                    registration.Snapshot,
                    bootstrapResult.DecisionIdentity,
                    expectedLifecycleVersion: 0);

                if (!persistence.Accepted)
                {
                    RebuildCommittedController();
                    return new BootstrapLifecycleRegistrationOutcome(
                        bootstrapResult,
                        new LifecycleRegistrationResult(
                            false,
                            $"AUTHORITATIVE_STATE_PERSISTENCE_REJECTED:{persistence.Reason}",
                            null));
                }
            }

            lock (_sync)
            {
                _bootstrapDecisions.Add(bootstrapResult.SubjectId, bootstrapResult);
                _subjects.Add(bootstrapResult.SubjectId, bootstrapRequest.Subject);

                if (bootstrapResult.RestrictionActive &&
                    bootstrapRequest.Restriction is not null)
                {
                    _activeRestrictions.Add(
                        bootstrapResult.SubjectId,
                        bootstrapRequest.Restriction);
                }
            }

            _committedRegistrations.Add(
                new RegistrationReplay(
                    bootstrapResult.SubjectId,
                    bootstrapResult.BootstrapContextId,
                    bootstrapResult.ValidUntil,
                    bootstrapResult.RestrictionActive,
                    bootstrapResult.ActiveRestrictionId,
                    evidenceReference ?? string.Empty,
                    bootstrapResult.DecisionTime));

            return new BootstrapLifecycleRegistrationOutcome(
                bootstrapResult,
                registration);
        }
    }

    public LifecycleAuthorityControlDecision TransitionAuthorized(
        LifecycleTransitionRequest? request,
        LifecycleTransitionEvidence? evidence,
        LifecycleAuthorityEvaluation? authorityEvaluation)
    {
        var safeRequest = request ?? new LifecycleTransitionRequest(
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            default,
            default);

        var safeEvidence = evidence ?? new LifecycleTransitionEvidence();
        var authorityRequest = BuildLifecycleAuthorityRequest(safeRequest, authorityEvaluation);
        var engine = new DefaultDenyAuthorityEngine();
        var authorityResult = engine.Evaluate(authorityRequest, authorityEvaluation?.Context);

        var boundRequest = safeRequest with
        {
            AuthorityReference = authorityResult.DecisionId
        };

        var boundEvidence = safeEvidence with
        {
            AuthorityDecision = authorityResult,
            ValidationEvidence = string.Empty
        };

        boundEvidence = boundEvidence with
        {
            ValidationEvidence = LifecycleEvidenceBinding.Compute(boundRequest, boundEvidence)
        };

        if (!string.Equals(authorityResult.Decision, AuthorityDecision.Allow, StringComparison.Ordinal))
        {
            var denied = RejectAtContractBoundary(
                boundRequest,
                boundEvidence,
                $"LIFECYCLE_AUTHORITY_DENIED:{authorityResult.Reason}");

            return new LifecycleAuthorityControlDecision(
                authorityRequest,
                authorityResult,
                denied);
        }

        var lifecycleDecision = TransitionCore(boundRequest, boundEvidence, authorityEngineEvaluated: true);
        return new LifecycleAuthorityControlDecision(
            authorityRequest,
            authorityResult,
            lifecycleDecision);
    }

    public LifecycleControlDecision Transition(
        LifecycleTransitionRequest? request,
        LifecycleTransitionEvidence? evidence)
    {
        if (_authorityMode == LifecycleAuthorityMode.AuthorityEngineRequired)
        {
            var rejectedRequest = request ?? new LifecycleTransitionRequest(
                string.Empty, string.Empty, string.Empty, string.Empty, string.Empty,
                string.Empty, string.Empty, string.Empty, default, default);
            return RejectAtContractBoundary(
                rejectedRequest,
                evidence ?? new LifecycleTransitionEvidence(),
                "AUTHORITY_ENGINE_REQUIRED");
        }

        return TransitionCore(request, evidence, authorityEngineEvaluated: false);
    }

    private LifecycleControlDecision TransitionCore(
        LifecycleTransitionRequest? request,
        LifecycleTransitionEvidence? evidence,
        bool authorityEngineEvaluated)
    {
        lock (_commitSync)
        {
            return TransitionCoreCommitted(
                request,
                evidence,
                authorityEngineEvaluated);
        }
    }

    private LifecycleControlDecision TransitionCoreCommitted(
        LifecycleTransitionRequest? request,
        LifecycleTransitionEvidence? evidence,
        bool authorityEngineEvaluated)
    {
        if (request is null || evidence is null)
        {
            var rejectedRequest = request ?? new LifecycleTransitionRequest(
                string.Empty, string.Empty, string.Empty, string.Empty, string.Empty,
                string.Empty, string.Empty, string.Empty, default, default);
            return RejectAtContractBoundary(
                rejectedRequest,
                evidence ?? new LifecycleTransitionEvidence(),
                "INVALID_CON003_REQUEST");
        }

        if (!_restartVerified &&
            _stateStore is not null &&
            !_committedRegistrations.Any(registration =>
                string.Equals(
                    registration.SubjectId,
                    request.ComponentIdentity,
                    StringComparison.Ordinal)))
        {
            var durable = _stateStore.Read(
                "foundation.lifecycle",
                request.ComponentIdentity,
                FoundationStateClass.LifecycleState);

            if (durable.Classification != DurableStateClassification.Missing)
            {
                return RejectAtContractBoundary(
                    request,
                    evidence,
                    "RESTART_RECONCILIATION_REQUIRED");
            }
        }

        if (_evidenceBlockedSubjects.Contains(request.ComponentIdentity))
        {
            return RejectAtContractBoundary(
                request,
                evidence,
                "POST_COMMIT_EVIDENCE_COMPLETION_REQUIRED");
        }

        var reservation = _controller.ReserveIdentities(
            request.TransitionRequestId,
            evidence.TransitionId,
            evidence.EventId);

        if (!reservation.Accepted)
        {
            return RejectAtContractBoundary(request, evidence, reservation.ReasonCode);
        }

        _reservedIdentities.Add(
            new IdentityReplay(
                request.TransitionRequestId,
                evidence.TransitionId,
                evidence.EventId));

        var contractValidation = ContractValidators.Validate(request);
        if (contractValidation.Result != ValidationResult.Pass)
        {
            return RejectAtContractBoundary(request, evidence, "INVALID_CON003_REQUEST");
        }

        if (!LifecycleStateVocabulary.TryParseContractText(
                request.AuthoritativeSourceState,
                out var sourceState) ||
            !LifecycleStateVocabulary.TryParseContractText(
                request.RequestedTargetState,
                out var targetState))
        {
            return RejectAtContractBoundary(request, evidence, "UNKNOWN_LIFECYCLE_STATE");
        }

        var snapshotBefore = _controller.GetSnapshot(request.ComponentIdentity);
        if (snapshotBefore is null)
        {
            return RejectAtContractBoundary(request, evidence, "UNKNOWN_SUBJECT");
        }

        BootstrapValidationResult? bootstrapDecision;
        BootstrapSubjectAdmissionEvidence? subject;
        RestrictionRecord? storedRestriction;

        lock (_sync)
        {
            _bootstrapDecisions.TryGetValue(request.ComponentIdentity, out bootstrapDecision);
            _subjects.TryGetValue(request.ComponentIdentity, out subject);
            _activeRestrictions.TryGetValue(request.ComponentIdentity, out storedRestriction);
        }

        if (bootstrapDecision is null || subject is null)
        {
            return RejectAtContractBoundary(request, evidence, "MISSING_REGISTERED_BOOTSTRAP_EVIDENCE");
        }

        var isBootstrapEntry =
            sourceState == LifecycleState.Registered &&
            targetState is LifecycleState.Initializing or LifecycleState.Restricted;

        if (isBootstrapEntry &&
            !string.Equals(
                bootstrapDecision.InitialLifecycleState,
                request.RequestedTargetState,
                StringComparison.Ordinal))
        {
            return RejectAtContractBoundary(request, evidence, "BOOTSTRAP_INITIAL_STATE_MISMATCH");
        }

        if (isBootstrapEntry && evidence.ObservationTime >= bootstrapDecision.ValidUntil)
        {
            return RejectAtContractBoundary(request, evidence, "BOOTSTRAP_EVIDENCE_EXPIRED");
        }

        var authorityFailure = ValidateAuthorityDecision(
            request,
            evidence.AuthorityDecision,
            evidence.ObservationTime,
            authorityEngineEvaluated);
        if (authorityFailure is not null)
        {
            return RejectAtContractBoundary(request, evidence, authorityFailure);
        }

        var timeFailure = ValidateTimeProvider(evidence.TimeProvider, evidence.ObservationTime);
        if (timeFailure is not null)
        {
            return RejectAtContractBoundary(request, evidence, timeFailure);
        }

        var dependencyReady = false;
        if (evidence.DependencyEvidence is not null)
        {
            var dependencyFailure = BootstrapContextGate.ValidateDependencyEvidence(
                subject,
                evidence.DependencyEvidence,
                BootstrapPolicyCatalog.GetStage3Wp05Policy(),
                evidence.ObservationTime);

            if (dependencyFailure is not null)
            {
                return RejectAtContractBoundary(request, evidence, dependencyFailure);
            }

            if (!string.Equals(
                    request.DependencyContext,
                    evidence.DependencyEvidence.EvidenceReference,
                    StringComparison.Ordinal))
            {
                return RejectAtContractBoundary(
                    request,
                    evidence,
                    "LIFECYCLE_DEPENDENCY_CONTEXT_MISMATCH");
            }

            dependencyReady = true;
        }

        if (targetState == LifecycleState.Running && !dependencyReady)
        {
            return RejectAtContractBoundary(
                request,
                evidence,
                "RUNNING_DEPENDENCY_EVIDENCE_REQUIRED");
        }

        var activeRestriction = storedRestriction;
        if (snapshotBefore.ProtectiveRestrictionActive && activeRestriction is null)
        {
            return RejectAtContractBoundary(
                request,
                evidence,
                "ACTIVE_RESTRICTION_EVIDENCE_MISSING");
        }

        if (evidence.Restriction is not null)
        {
            var restrictionFailure = BootstrapContextGate.ValidateRestriction(
                evidence.Restriction,
                request.ComponentIdentity,
                evidence.ObservationTime);

            if (restrictionFailure is not null)
            {
                return RejectAtContractBoundary(request, evidence, restrictionFailure);
            }

            if (activeRestriction is not null &&
                !string.Equals(
                    activeRestriction.RestrictionId,
                    evidence.Restriction.RestrictionId,
                    StringComparison.Ordinal))
            {
                return RejectAtContractBoundary(
                    request,
                    evidence,
                    "ACTIVE_RESTRICTION_ID_MISMATCH");
            }

            activeRestriction ??= evidence.Restriction;
        }

        if (targetState == LifecycleState.Restricted && activeRestriction is null)
        {
            return RejectAtContractBoundary(
                request,
                evidence,
                "RESTRICTION_EVIDENCE_REQUIRED");
        }

        var controlledReleaseTransition =
            snapshotBefore.ProtectiveRestrictionActive &&
            targetState == LifecycleState.Recovering;
        var releaseValidated = false;

        if (controlledReleaseTransition)
        {
            if (activeRestriction is null || evidence.RestrictionRelease is null)
            {
                return RejectAtContractBoundary(
                    request,
                    evidence,
                    "RESTRICTION_RELEASE_EVIDENCE_REQUIRED");
            }

            var releaseFailure = BootstrapContextGate.ValidateRestrictionRelease(
                activeRestriction,
                evidence.RestrictionRelease,
                request.TransitionRequestId,
                evidence.AuthorityDecision.DecisionId,
                evidence.ObservationTime);

            if (releaseFailure is not null)
            {
                return RejectAtContractBoundary(request, evidence, releaseFailure);
            }

            releaseValidated = true;
        }
        else if (evidence.RestrictionRelease is not null)
        {
            return RejectAtContractBoundary(
                request,
                evidence,
                "UNEXPECTED_RESTRICTION_RELEASE");
        }

        var recoveryValidated = false;
        if (sourceState == LifecycleState.Recovering && targetState == LifecycleState.Ready)
        {
            var recoveryFailure = ValidateRecovery(
                request,
                evidence,
                bootstrapDecision,
                evidence.RecoveryValidation);

            if (recoveryFailure is not null)
            {
                return RejectAtContractBoundary(request, evidence, recoveryFailure);
            }

            recoveryValidated = true;
        }
        else if (evidence.RecoveryValidation is not null)
        {
            return RejectAtContractBoundary(
                request,
                evidence,
                "UNEXPECTED_RECOVERY_VALIDATION");
        }

        var expectedValidationEvidence = LifecycleEvidenceBinding.Compute(request, evidence);
        if (!string.Equals(
                evidence.ValidationEvidence,
                expectedValidationEvidence,
                StringComparison.Ordinal))
        {
            return RejectAtContractBoundary(
                request,
                evidence,
                "LIFECYCLE_EVIDENCE_BINDING_MISMATCH");
        }

        var bootstrapBindingAccepted =
            bootstrapDecision.Accepted &&
            string.Equals(
                bootstrapDecision.BootstrapContextId,
                evidence.BootstrapContextId,
                StringComparison.Ordinal) &&
            evidence.ObservationTime < bootstrapDecision.ValidUntil;

        var controllingRestriction = activeRestriction;
        var protectiveTransition =
            controllingRestriction is not null &&
            targetState is LifecycleState.Restricted
                or LifecycleState.Suspended
                or LifecycleState.Stopping
                or LifecycleState.Stopped
                or LifecycleState.Failed;

        var command = new LifecycleTransitionCommand(
            request.TransitionRequestId,
            evidence.TransitionId,
            evidence.EventId,
            request.ComponentIdentity,
            evidence.ModelId,
            evidence.ModelVersion,
            sourceState,
            targetState,
            evidence.ExpectedStateVersion,
            request.Requester,
            request.AuthorityReference,
            request.Reason,
            request.DependencyContext,
            evidence.BootstrapContextId,
            evidence.ValidationEvidence,
            controllingRestriction?.RestrictionId ?? string.Empty,
            request.RequestTime,
            request.Expiry,
            evidence.ObservationTime,
            true,
            true,
            bootstrapBindingAccepted,
            dependencyReady,
            controllingRestriction is not null,
            protectiveTransition,
            releaseValidated,
            recoveryValidated);

        var outcome = _controller.ApplyTransition(command);
        var resultingState = outcome.Snapshot is null
            ? "UNKNOWN"
            : LifecycleStateVocabulary.ToContractText(outcome.Snapshot.State);
        var eventReference = outcome.Event is null
            ? $"NO_SUCCESS_EVENT:{outcome.Attempt.AttemptId}"
            : outcome.Event.EventId;

        var contractResult = new LifecycleTransitionResult(
            request.TransitionRequestId,
            evidence.TransitionId,
            ToContractDecision(outcome.Decision),
            request.AuthoritativeSourceState,
            request.RequestedTargetState,
            resultingState,
            outcome.ReasonCode,
            evidence.ValidationEvidence,
            evidence.ObservationTime,
            eventReference);

        if (outcome.Decision == LifecycleAttemptDecision.Accepted &&
            outcome.Snapshot is not null)
        {
            DurableStateWriteResult? persistence = null;
            if (_stateStore is not null)
            {
                persistence = PersistLifecycleSnapshot(
                    outcome.Snapshot,
                    outcome.Event?.EventId ?? outcome.Attempt.AttemptId,
                    evidence.ExpectedStateVersion);

                if (!persistence.Accepted)
                {
                    RebuildCommittedController();
                    return RejectAtContractBoundary(
                        request,
                        evidence,
                        $"AUTHORITATIVE_STATE_PERSISTENCE_REJECTED:{persistence.Reason}");
                }
            }

            _committedTransitions.Add(command);

            var evidenceAppend = RecordEvidence(
                request,
                evidence,
                outcome.ReasonCode,
                EvidenceExecutionOutcome.Accepted,
                persistence is null
                    ? EvidencePersistenceOutcome.NotAttempted
                    : EvidencePersistenceOutcome.Accepted,
                persistence?.Current?.StateVersion ?? outcome.Snapshot.StateVersion - 1,
                persistence?.Current?.RecordDigest ?? string.Empty);

            if (_evidenceJournal is not null &&
                (evidenceAppend is null || !evidenceAppend.Accepted))
            {
                PersistEvidenceCompletionBlock(
                    request.ComponentIdentity,
                    "POST_COMMIT_EVIDENCE_APPEND_REJECTED:" +
                    (evidenceAppend?.Reason ?? "NO_EVIDENCE_RESULT"));
                return CreatePostCommitEvidenceFailureDecision(
                    request,
                    evidence,
                    outcome.Snapshot,
                    outcome.Attempt,
                    outcome.Event,
                    "POST_COMMIT_EVIDENCE_APPEND_REJECTED:" +
                    (evidenceAppend?.Reason ?? "NO_EVIDENCE_RESULT"));
            }

            if (persistence is not null &&
                evidenceAppend is not null &&
                _acceptedFactPublisher is not null)
            {
                var fact = _acceptedFactPublisher.Publish(
                    evidenceAppend,
                    persistence,
                    "LIFECYCLE_STATE_TRANSITION");

                if (!fact.Accepted)
                {
                    PersistEvidenceCompletionBlock(
                        request.ComponentIdentity,
                        "POST_COMMIT_ACCEPTED_FACT_REJECTED:" + fact.Reason);
                    return CreatePostCommitEvidenceFailureDecision(
                        request,
                        evidence,
                        outcome.Snapshot,
                        outcome.Attempt,
                        outcome.Event,
                        "POST_COMMIT_ACCEPTED_FACT_REJECTED:" + fact.Reason);
                }
            }
            else if (persistence is not null &&
                     (_evidenceJournal is null) != (_acceptedFactPublisher is null))
            {
                PersistEvidenceCompletionBlock(
                    request.ComponentIdentity,
                    "POST_COMMIT_EVIDENCE_PIPELINE_INCOMPLETE");
                return CreatePostCommitEvidenceFailureDecision(
                    request,
                    evidence,
                    outcome.Snapshot,
                    outcome.Attempt,
                    outcome.Event,
                    "POST_COMMIT_EVIDENCE_PIPELINE_INCOMPLETE");
            }

            lock (_sync)
            {
                if (releaseValidated)
                {
                    _activeRestrictions.Remove(request.ComponentIdentity);
                }
                else if (outcome.Snapshot.ProtectiveRestrictionActive &&
                         controllingRestriction is not null)
                {
                    _activeRestrictions[request.ComponentIdentity] = controllingRestriction;
                }
            }
        }

        return new LifecycleControlDecision(
            contractResult,
            outcome.Snapshot,
            outcome.Attempt,
            outcome.Event);
    }

    private static LifecycleControlDecision CreatePostCommitEvidenceFailureDecision(
        LifecycleTransitionRequest request,
        LifecycleTransitionEvidence evidence,
        LifecycleStateSnapshot? snapshot,
        LifecycleTransitionAttempt attempt,
        LifecycleTransitionEvent? transitionEvent,
        string reason)
    {
        var resultingState = snapshot is null
            ? "UNKNOWN"
            : LifecycleStateVocabulary.ToContractText(snapshot.State);
        var eventReference = transitionEvent is null
            ? $"NO_SUCCESS_EVENT:{attempt.AttemptId}"
            : transitionEvent.EventId;

        var result = new LifecycleTransitionResult(
            request.TransitionRequestId,
            evidence.TransitionId,
            "FAILED",
            request.AuthoritativeSourceState,
            request.RequestedTargetState,
            resultingState,
            reason,
            evidence.ValidationEvidence,
            evidence.ObservationTime,
            eventReference);

        return new LifecycleControlDecision(
            result,
            snapshot,
            attempt,
            transitionEvent);
    }

    private void RestoreEvidenceCompletionBlocks()
    {
        if (_evidenceJournal is null)
        {
            return;
        }

        foreach (var block in _evidenceJournal.ReadEvidenceCompletionBlocks())
        {
            _evidenceBlockedSubjects.Add(block.SubjectId);
        }
    }

    private void PersistEvidenceCompletionBlock(string subjectId, string reason)
    {
        if (_evidenceJournal is null)
        {
            _evidenceBlockedSubjects.Add(subjectId);
            return;
        }

        var result = _evidenceJournal.BlockEvidenceCompletion(subjectId, reason);
        if (!result.Accepted)
        {
            throw new InvalidOperationException(
                "EVIDENCE_COMPLETION_BLOCK_PERSISTENCE_REJECTED:" +
                result.Reason);
        }

        _evidenceBlockedSubjects.Add(subjectId);
    }

    public bool IsEvidenceCompletionBlocked(string subjectId)
    {
        lock (_commitSync)
        {
            return _evidenceBlockedSubjects.Contains(subjectId);
        }
    }

    private DurableStateWriteResult PersistLifecycleSnapshot(
        LifecycleStateSnapshot snapshot,
        string sourceIdentity,
        long expectedLifecycleVersion)
    {
        var durableVersion = snapshot.StateVersion - 1;
        var expectedDurableVersion = expectedLifecycleVersion - 1;
        var previousDigest = expectedDurableVersion < 0
            ? string.Empty
            : _stateStore!.Read(
                "foundation.lifecycle",
                snapshot.SubjectId,
                FoundationStateClass.LifecycleState).Current?.RecordDigest ?? string.Empty;

        var record = new AuthoritativeStateRecord(
            $"lifecycle-state:{snapshot.SubjectId}:{durableVersion}",
            "foundation.lifecycle",
            snapshot.SubjectId,
            FoundationStateClass.LifecycleState,
            StateRepresentationKind.Authoritative,
            "Foundation.LifecycleControlService",
            "DurableAuthoritativeLifecycleRecord",
            "Foundation.State.FileAuthoritativeStateProvider",
            "Foundation.LifecycleControlService",
            sourceIdentity,
            durableVersion,
            snapshot.EffectiveTime,
            "FULL_HISTORY",
            LifecycleStateVocabulary.ToContractText(snapshot.State),
            previousDigest,
            string.Empty);

        return _stateStore!.Write(record, expectedDurableVersion);
    }

    private void RebuildCommittedController()
    {
        var rebuilt = new LifecycleController();

        foreach (var registration in _committedRegistrations)
        {
            var result = rebuilt.RegisterSubject(
                registration.SubjectId,
                _model,
                registration.BootstrapContextId,
                registration.BootstrapValidUntil,
                registration.ProtectiveRestrictionActive,
                registration.ActiveRestrictionId,
                registration.EvidenceReference,
                registration.EffectiveTime);

            if (!result.Accepted)
            {
                throw new InvalidOperationException(
                    $"Committed lifecycle registration replay failed: {result.ReasonCode}");
            }
        }

        foreach (var reservation in _reservedIdentities)
        {
            var result = rebuilt.ReserveIdentities(
                reservation.RequestId,
                reservation.TransitionId,
                reservation.EventId);

            if (!result.Accepted)
            {
                throw new InvalidOperationException(
                    $"Committed lifecycle identity replay failed: {result.ReasonCode}");
            }
        }

        foreach (var command in _committedTransitions)
        {
            var result = rebuilt.ApplyTransition(command);
            if (result.Decision != LifecycleAttemptDecision.Accepted)
            {
                throw new InvalidOperationException(
                    $"Committed lifecycle transition replay failed: {result.ReasonCode}");
            }
        }

        _controller = rebuilt;
    }

    public LifecycleStateSnapshot? GetSnapshot(string? subjectId)
    {
        lock (_commitSync)
        {
            return string.IsNullOrWhiteSpace(subjectId)
                ? null
                : _controller.GetSnapshot(subjectId);
        }
    }

    public ReadOnlyCollection<LifecycleTransitionAttempt> GetAttempts()
    {
        lock (_commitSync)
        {
            return _controller.GetAttempts();
        }
    }

    public ReadOnlyCollection<LifecycleTransitionEvent> GetEvents()
    {
        lock (_commitSync)
        {
            return _controller.GetEvents();
        }
    }

    public ReadOnlyCollection<LifecycleContractRejection> GetContractRejections()
    {
        lock (_sync)
        {
            return Array.AsReadOnly(_contractRejections.ToArray());
        }
    }

    private LifecycleControlDecision RejectAtContractBoundary(
        LifecycleTransitionRequest request,
        LifecycleTransitionEvidence evidence,
        string reasonCode)
    {
        var snapshot = _controller.GetSnapshot(request.ComponentIdentity);
        var resultingState = snapshot is null
            ? "UNKNOWN"
            : LifecycleStateVocabulary.ToContractText(snapshot.State);
        var requestReference = string.IsNullOrWhiteSpace(request.TransitionRequestId)
            ? "UNIDENTIFIED"
            : request.TransitionRequestId;
        var rejectionReference = $"NO_SUCCESS_EVENT:CONTRACT_REJECTION:{requestReference}";

        var rejection = new LifecycleContractRejection(
            request.TransitionRequestId,
            evidence.TransitionId,
            evidence.EventId,
            request.ComponentIdentity,
            reasonCode,
            evidence.ObservationTime,
            evidence.ValidationEvidence);

        lock (_sync)
        {
            _contractRejections.Add(rejection);
        }

        var transitionId = string.IsNullOrWhiteSpace(evidence.TransitionId)
            ? "UNIDENTIFIED"
            : evidence.TransitionId;

        var result = new LifecycleTransitionResult(
            requestReference,
            transitionId,
            "REJECTED",
            string.IsNullOrWhiteSpace(request.AuthoritativeSourceState)
                ? "UNKNOWN"
                : request.AuthoritativeSourceState,
            string.IsNullOrWhiteSpace(request.RequestedTargetState)
                ? "UNKNOWN"
                : request.RequestedTargetState,
            resultingState,
            reasonCode,
            string.IsNullOrWhiteSpace(evidence.ValidationEvidence)
                ? "NO_VALIDATION_EVIDENCE"
                : evidence.ValidationEvidence,
            evidence.ObservationTime == default
                ? request.RequestTime
                : evidence.ObservationTime,
            rejectionReference);

        RecordEvidence(
            request,
            evidence,
            reasonCode,
            EvidenceExecutionOutcome.Rejected,
            reasonCode.StartsWith(
                "AUTHORITATIVE_STATE_PERSISTENCE_REJECTED:",
                StringComparison.Ordinal)
                ? EvidencePersistenceOutcome.Rejected
                : EvidencePersistenceOutcome.NotAttempted,
            snapshot is null ? -1 : snapshot.StateVersion - 1,
            string.Empty);

        return new LifecycleControlDecision(result, snapshot, null, null);
    }

    private EvidenceAppendResult? RecordEvidence(
        LifecycleTransitionRequest request,
        LifecycleTransitionEvidence evidence,
        string reason,
        EvidenceExecutionOutcome executionOutcome,
        EvidencePersistenceOutcome persistenceOutcome,
        long stateVersion,
        string stateDigest)
    {
        if (_evidenceJournal is null)
        {
            return null;
        }

        var authority = evidence.AuthorityDecision;
        var decision = string.Equals(
                authority?.Decision,
                AuthorityDecision.Allow,
                StringComparison.Ordinal)
            ? EvidenceDecisionKind.Allow
            : EvidenceDecisionKind.Deny;

        return _evidenceJournal.Append(
            new EvidenceAppendRequest(
                string.Empty,
                string.IsNullOrWhiteSpace(request.Requester)
                    ? "UNIDENTIFIED_ACTOR"
                    : request.Requester,
                string.IsNullOrWhiteSpace(request.TransitionRequestId)
                    ? "UNIDENTIFIED_REQUEST"
                    : request.TransitionRequestId,
                decision,
                string.IsNullOrWhiteSpace(authority?.DecisionId)
                    ? "UNIDENTIFIED_DECISION"
                    : authority.DecisionId,
                string.IsNullOrWhiteSpace(reason)
                    ? "UNSPECIFIED_REASON"
                    : reason,
                executionOutcome,
                persistenceOutcome,
                "foundation.lifecycle",
                string.IsNullOrWhiteSpace(request.ComponentIdentity)
                    ? "UNIDENTIFIED_SUBJECT"
                    : request.ComponentIdentity,
                stateVersion,
                stateDigest,
                "Foundation.LifecycleControlService",
                string.Empty));
    }

    public EvidenceJournalReadResult? GetEvidenceJournal()
        => _evidenceJournal?.Read();

    public IReadOnlyList<AcceptedFactEvent> GetAcceptedFacts()
        => _evidenceJournal?.ReadAcceptedFacts()
            ?? Array.Empty<AcceptedFactEvent>();

    private static AuthorityRequest BuildLifecycleAuthorityRequest(
        LifecycleTransitionRequest request,
        LifecycleAuthorityEvaluation? evaluation)
    {
        var requestedScope =
            $"LIFECYCLE:{request.ComponentIdentity}:{request.AuthoritativeSourceState}->{request.RequestedTargetState}";

        return new AuthorityRequest(
            request.TransitionRequestId,
            request.Requester,
            "lifecycle.transition",
            $"lifecycle:{request.ComponentIdentity}",
            "authoritative-lifecycle-transition",
            requestedScope,
            "foundation.lifecycle",
            evaluation?.SecurityContext ?? string.Empty,
            evaluation?.RequiredFitnessToOperate ?? string.Empty,
            $"lifecycle-correlation:{request.TransitionRequestId}",
            request.RequestTime,
            request.Expiry);
    }

    private static string? ValidateAuthorityDecision(
        LifecycleTransitionRequest request,
        AuthorityResult authority,
        DateTimeOffset observationTime,
        bool authorityEngineEvaluated)
    {
        if (authority is null ||
            ContractValidators.Validate(authority).Result != ValidationResult.Pass)
        {
            return "INVALID_LIFECYCLE_AUTHORITY_DECISION";
        }

        var expectedScope =
            $"LIFECYCLE:{request.ComponentIdentity}:{request.AuthoritativeSourceState}->{request.RequestedTargetState}";

        if (string.Equals(authority.Decision, AuthorityDecision.Allow, StringComparison.Ordinal))
        {
            if (!authorityEngineEvaluated)
            {
                return "AUTHORITY_ENGINE_EVALUATION_REQUIRED";
            }
            if (!string.Equals(authority.RequestId, request.TransitionRequestId, StringComparison.Ordinal) ||
                !authority.DecisionId.StartsWith("authority-decision/sha256/", StringComparison.Ordinal) ||
                !string.Equals(authority.EffectiveScope, expectedScope, StringComparison.Ordinal) ||
                string.IsNullOrWhiteSpace(authority.ControllingPolicy) ||
                string.IsNullOrWhiteSpace(authority.PolicyVersion) ||
                !string.Equals(authority.Constraints, "BOUNDED_TO_EFFECTIVE_SCOPE", StringComparison.Ordinal) ||
                !string.Equals(authority.Reason, AuthorityReason.Allowed, StringComparison.Ordinal) ||
                string.IsNullOrWhiteSpace(authority.EvidenceReference) ||
                !string.Equals(request.AuthorityReference, authority.DecisionId, StringComparison.Ordinal))
            {
                return "LIFECYCLE_AUTHORITY_BINDING_MISMATCH";
            }

            if (!BootstrapContextGate.ContainsTime(
                    authority.DecisionTime,
                    authority.Expiry,
                    observationTime))
            {
                return "LIFECYCLE_AUTHORITY_NOT_EFFECTIVE";
            }

            return null;
        }

        if (authorityEngineEvaluated)
        {
            return "INVALID_LIFECYCLE_AUTHORITY_DECISION";
        }

        var policy = BootstrapPolicyCatalog.GetStage3Wp05Policy();
        var expectedDecisionId = $"authority:{request.TransitionRequestId}";
        var expectedEvidenceReference = $"authority-evidence:{request.TransitionRequestId}";

        if (!string.Equals(authority.RequestId, request.TransitionRequestId, StringComparison.Ordinal) ||
            !string.Equals(authority.DecisionId, expectedDecisionId, StringComparison.Ordinal) ||
            !string.Equals(authority.Decision, "ACCEPTED", StringComparison.Ordinal) ||
            !string.Equals(authority.EffectiveScope, expectedScope, StringComparison.Ordinal) ||
            !string.Equals(authority.ControllingPolicy, policy.LifecycleAuthorityPolicy, StringComparison.Ordinal) ||
            !string.Equals(authority.PolicyVersion, policy.Version, StringComparison.Ordinal) ||
            !string.Equals(authority.MaterialConditions, "BOUND_EVIDENCE_REQUIRED", StringComparison.Ordinal) ||
            !string.Equals(authority.Constraints, "NO_BYPASS,NO_SELF_ATTESTATION", StringComparison.Ordinal) ||
            !string.Equals(authority.Reason, "BOUNDED_WP05_LIFECYCLE_TRANSITION", StringComparison.Ordinal) ||
            !string.Equals(authority.EvidenceReference, expectedEvidenceReference, StringComparison.Ordinal) ||
            !string.Equals(request.AuthorityReference, authority.DecisionId, StringComparison.Ordinal))
        {
            return "LIFECYCLE_AUTHORITY_BINDING_MISMATCH";
        }

        if (!BootstrapContextGate.ContainsTime(
                authority.DecisionTime,
                authority.Expiry,
                observationTime))
        {
            return "LIFECYCLE_AUTHORITY_NOT_EFFECTIVE";
        }

        return null;
    }

    private static string? ValidateTimeProvider(
        TimeProviderRecord timeProvider,
        DateTimeOffset observationTime)
    {
        if (timeProvider is null ||
            ProviderContractValidators.Validate(timeProvider).Result != ValidationResult.Pass)
        {
            return "INVALID_LIFECYCLE_TIME_PROVIDER";
        }

        var policy = BootstrapPolicyCatalog.GetStage3Wp05Policy();
        if (!string.Equals(timeProvider.ProviderId, policy.TimeProviderId, StringComparison.Ordinal) ||
            !string.Equals(timeProvider.ProviderClass, "FOUNDATION_TIME_PROVIDER", StringComparison.Ordinal) ||
            !string.Equals(timeProvider.AdmissionAuthority, "GOV-027", StringComparison.Ordinal) ||
            !string.Equals(timeProvider.Boundaries, "STAGE3-WP05-ISOLATED", StringComparison.Ordinal) ||
            !string.Equals(timeProvider.SourceOfTime, "FALCON-GOVERNED-TIME", StringComparison.Ordinal) ||
            !string.Equals(timeProvider.ValidationEvidence, "TIME-EVIDENCE-001", StringComparison.Ordinal))
        {
            return "LIFECYCLE_TIME_PROVIDER_MISMATCH";
        }

        if (!BootstrapContextGate.ContainsTime(
                timeProvider.EffectiveTime,
                timeProvider.Expiry,
                observationTime))
        {
            return "LIFECYCLE_TIME_PROVIDER_NOT_EFFECTIVE";
        }

        return null;
    }

    private static string? ValidateRecovery(
        LifecycleTransitionRequest request,
        LifecycleTransitionEvidence evidence,
        BootstrapValidationResult bootstrapDecision,
        RecoveryValidationEvidence? recovery)
    {
        if (recovery is null ||
            string.IsNullOrWhiteSpace(recovery.ValidationId) ||
            string.IsNullOrWhiteSpace(recovery.SubjectId) ||
            string.IsNullOrWhiteSpace(recovery.TransitionRequestId) ||
            string.IsNullOrWhiteSpace(recovery.BootstrapContextId) ||
            string.IsNullOrWhiteSpace(recovery.ValidatorAuthority) ||
            string.IsNullOrWhiteSpace(recovery.AuthorityDecisionReference) ||
            string.IsNullOrWhiteSpace(recovery.EvidenceReference))
        {
            return "RECOVERY_VALIDATION_EVIDENCE_REQUIRED";
        }

        var policy = BootstrapPolicyCatalog.GetStage3Wp05Policy();
        var expectedValidationId =
            $"recovery-validation:{request.ComponentIdentity}:{request.TransitionRequestId}";
        var expectedEvidenceReference =
            $"recovery-evidence:{request.TransitionRequestId}";
        if (!string.Equals(recovery.ValidationId, expectedValidationId, StringComparison.Ordinal) ||
            !string.Equals(recovery.EvidenceReference, expectedEvidenceReference, StringComparison.Ordinal) ||
            !string.Equals(recovery.SubjectId, request.ComponentIdentity, StringComparison.Ordinal) ||
            !string.Equals(
                recovery.TransitionRequestId,
                request.TransitionRequestId,
                StringComparison.Ordinal) ||
            !string.Equals(
                recovery.BootstrapContextId,
                bootstrapDecision.BootstrapContextId,
                StringComparison.Ordinal) ||
            !string.Equals(
                recovery.ValidatorAuthority,
                policy.RecoveryValidatorAuthority,
                StringComparison.Ordinal) ||
            !string.Equals(
                recovery.AuthorityDecisionReference,
                evidence.AuthorityDecision.DecisionId,
                StringComparison.Ordinal) ||
            !string.Equals(recovery.ValidationResult, "VALIDATED", StringComparison.Ordinal))
        {
            return "RECOVERY_VALIDATION_BINDING_MISMATCH";
        }

        if (!BootstrapContextGate.ContainsTime(
                recovery.EffectiveTime,
                recovery.Expiry,
                evidence.ObservationTime))
        {
            return "RECOVERY_VALIDATION_NOT_EFFECTIVE";
        }

        return null;
    }

    private static string ToContractDecision(LifecycleAttemptDecision decision)
        => decision switch
        {
            LifecycleAttemptDecision.Accepted => "ACCEPTED",
            LifecycleAttemptDecision.Rejected => "REJECTED",
            LifecycleAttemptDecision.Failed => "FAILED",
            _ => throw new ArgumentOutOfRangeException(
                nameof(decision),
                decision,
                "Unknown lifecycle decision.")
        };
}

internal static class CanonicalEncoding
{
    public static string Build(params object?[] values)
    {
        var builder = new StringBuilder();

        foreach (var value in values)
        {
            var text = Value(value);
            builder.Append(Encoding.UTF8.GetByteCount(text).ToString(CultureInfo.InvariantCulture));
            builder.Append(':');
            builder.Append(text);
            builder.Append(';');
        }

        return builder.ToString();
    }

    private static string Value(object? value)
        => value switch
        {
            null => "<null>",
            DateTimeOffset time => time.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture),
            bool flag => flag ? "true" : "false",
            Enum enumeration => Convert.ToInt64(enumeration, CultureInfo.InvariantCulture)
                .ToString(CultureInfo.InvariantCulture),
            IFormattable formattable =>
                formattable.ToString(null, CultureInfo.InvariantCulture) ?? string.Empty,
            _ => value.ToString() ?? string.Empty
        };
}
