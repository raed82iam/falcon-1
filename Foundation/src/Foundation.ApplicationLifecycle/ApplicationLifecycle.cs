using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Foundation.ApplicationLifecycle;

public enum LifecycleTransitionKind { Attach = 1, UpgradeOrReplace = 2, DetachOrRemove = 3, Rollback = 4 }
public enum LifecycleState { Detached = 1, Attached = 2, Draining = 3, RollbackRequired = 4, Removed = 5 }
public enum LifecycleDecisionKind { Allowed = 1, Rejected = 2, DrainRequired = 3, RollbackRequired = 4 }
public enum LifecycleEvidenceStatus { Valid = 1, Missing = 2, Stale = 3, Revoked = 4, Invalid = 5, Ambiguous = 6 }

public static class LifecycleReason
{
    public const string AttachEligible = "ATTACH_ELIGIBLE";
    public const string UpgradeEligible = "UPGRADE_REPLACEMENT_ELIGIBLE";
    public const string DetachEligible = "SAFE_DETACH_REMOVAL_ELIGIBLE";
    public const string RollbackEligible = "ROLLBACK_ELIGIBLE";
    public const string DrainRequired = "DRAIN_REQUIRED";
    public const string AuthorityMissing = "LIFECYCLE_AUTHORITY_MISSING";
    public const string AuthorityStale = "LIFECYCLE_AUTHORITY_STALE";
    public const string AuthorityRevoked = "LIFECYCLE_AUTHORITY_REVOKED";
    public const string AuthorityInvalid = "LIFECYCLE_AUTHORITY_INVALID";
    public const string AuthorityAmbiguous = "LIFECYCLE_AUTHORITY_AMBIGUOUS";
    public const string AuthoritySubjectMismatch = "LIFECYCLE_AUTHORITY_SUBJECT_MISMATCH";
    public const string AuthorityTransitionMismatch = "LIFECYCLE_AUTHORITY_TRANSITION_MISMATCH";
    public const string AuthorityVersionMismatch = "LIFECYCLE_AUTHORITY_VERSION_MISMATCH";
    public const string ManifestInvalid = "MANIFEST_PREREQUISITE_INVALID";
    public const string DependencyInvalid = "DEPENDENCY_CONTINUITY_INVALID";
    public const string CompatibilityInvalid = "CONTRACT_SCHEMA_COMPATIBILITY_INVALID";
    public const string SecurityInvalid = "SECURITY_CONTROL_CONTINUITY_INVALID";
    public const string AuthorityExpansion = "AUTHORITY_EXPANSION_PROHIBITED";
    public const string ProtectedControlWeakening = "PROTECTED_CONTROL_WEAKENING_PROHIBITED";
    public const string HiddenCoupling = "HIDDEN_COUPLING_PREVENTS_SAFE_LIFECYCLE";
    public const string DrainEvidenceInvalid = "DRAIN_EVIDENCE_INVALID";
    public const string RollbackEvidenceInvalid = "ROLLBACK_EVIDENCE_INVALID";
    public const string RollbackTargetMismatch = "ROLLBACK_TARGET_MISMATCH";
    public const string RollbackAuthorityInvalid = "ROLLBACK_AUTHORITY_NO_LONGER_VALID";
    public const string InvalidTransition = "INVALID_LIFECYCLE_TRANSITION";
    public const string TargetVersionRequired = "TARGET_VERSION_REQUIRED";
    public const string TargetVersionUnchanged = "TARGET_VERSION_UNCHANGED";
}

public sealed record LifecycleEvidence
{
    public LifecycleEvidence(string evidenceIdentity, LifecycleEvidenceStatus status)
    {
        EvidenceIdentity = LifecycleRules.RequireIdentifier(evidenceIdentity, nameof(evidenceIdentity));
        Status = LifecycleRules.RequireDefined(status, nameof(status));
        EvidenceDigest = LifecycleCanonicalization.Hash(("evidence_identity", EvidenceIdentity), ("status", Status.ToString()));
    }
    public string EvidenceIdentity { get; }
    public LifecycleEvidenceStatus Status { get; }
    public string EvidenceDigest { get; }
}

public sealed record LifecycleAuthorityEvidence
{
    public LifecycleAuthorityEvidence(string authorityIdentity, LifecycleEvidenceStatus status, string subjectIdentity,
        LifecycleTransitionKind permittedTransition, string currentVersion, string targetVersion)
    {
        AuthorityIdentity = LifecycleRules.RequireIdentifier(authorityIdentity, nameof(authorityIdentity));
        Status = LifecycleRules.RequireDefined(status, nameof(status));
        SubjectIdentity = LifecycleRules.RequireIdentifier(subjectIdentity, nameof(subjectIdentity));
        PermittedTransition = LifecycleRules.RequireDefined(permittedTransition, nameof(permittedTransition));
        CurrentVersion = LifecycleRules.RequireOptionalIdentifier(currentVersion, nameof(currentVersion));
        TargetVersion = LifecycleRules.RequireOptionalIdentifier(targetVersion, nameof(targetVersion));
        AuthorityEvidenceDigest = LifecycleCanonicalization.Hash(
            ("authority_identity", AuthorityIdentity), ("status", Status.ToString()), ("subject_identity", SubjectIdentity),
            ("permitted_transition", PermittedTransition.ToString()), ("current_version", CurrentVersion), ("target_version", TargetVersion));
    }
    public string AuthorityIdentity { get; }
    public LifecycleEvidenceStatus Status { get; }
    public string SubjectIdentity { get; }
    public LifecycleTransitionKind PermittedTransition { get; }
    public string CurrentVersion { get; }
    public string TargetVersion { get; }
    public string AuthorityEvidenceDigest { get; }
}

public sealed record LifecycleContinuityEvidence
{
    public LifecycleContinuityEvidence(string evidenceIdentity, LifecycleEvidenceStatus status, bool authorityDoesNotExpand,
        bool protectedControlsNotWeakened, bool requiredDependenciesSatisfied, bool contractsCompatible, bool hiddenCouplingAbsent)
    {
        EvidenceIdentity = LifecycleRules.RequireIdentifier(evidenceIdentity, nameof(evidenceIdentity));
        Status = LifecycleRules.RequireDefined(status, nameof(status));
        AuthorityDoesNotExpand = authorityDoesNotExpand;
        ProtectedControlsNotWeakened = protectedControlsNotWeakened;
        RequiredDependenciesSatisfied = requiredDependenciesSatisfied;
        ContractsCompatible = contractsCompatible;
        HiddenCouplingAbsent = hiddenCouplingAbsent;
        ContinuityDigest = LifecycleCanonicalization.Hash(
            ("evidence_identity", EvidenceIdentity), ("status", Status.ToString()),
            ("authority_does_not_expand", authorityDoesNotExpand ? "true" : "false"),
            ("protected_controls_not_weakened", protectedControlsNotWeakened ? "true" : "false"),
            ("required_dependencies_satisfied", requiredDependenciesSatisfied ? "true" : "false"),
            ("contracts_compatible", contractsCompatible ? "true" : "false"),
            ("hidden_coupling_absent", hiddenCouplingAbsent ? "true" : "false"));
    }
    public string EvidenceIdentity { get; }
    public LifecycleEvidenceStatus Status { get; }
    public bool AuthorityDoesNotExpand { get; }
    public bool ProtectedControlsNotWeakened { get; }
    public bool RequiredDependenciesSatisfied { get; }
    public bool ContractsCompatible { get; }
    public bool HiddenCouplingAbsent { get; }
    public string ContinuityDigest { get; }
}

public sealed record LifecycleDrainEvidence
{
    public LifecycleDrainEvidence(bool drainRequired, bool drainComplete, string evidenceIdentity, LifecycleEvidenceStatus status)
    {
        DrainRequired = drainRequired;
        DrainComplete = drainComplete;
        EvidenceIdentity = LifecycleRules.RequireOptionalIdentifier(evidenceIdentity, nameof(evidenceIdentity));
        Status = LifecycleRules.RequireDefined(status, nameof(status));
        DrainDigest = LifecycleCanonicalization.Hash(("drain_required", drainRequired ? "true" : "false"),
            ("drain_complete", drainComplete ? "true" : "false"), ("evidence_identity", EvidenceIdentity), ("status", Status.ToString()));
    }
    public bool DrainRequired { get; }
    public bool DrainComplete { get; }
    public string EvidenceIdentity { get; }
    public LifecycleEvidenceStatus Status { get; }
    public string DrainDigest { get; }
}

public sealed record LifecycleRollbackEvidence
{
    public LifecycleRollbackEvidence(string evidenceIdentity, LifecycleEvidenceStatus status, string rollbackTargetVersion, bool rollbackAuthorityStillValid)
    {
        EvidenceIdentity = LifecycleRules.RequireIdentifier(evidenceIdentity, nameof(evidenceIdentity));
        Status = LifecycleRules.RequireDefined(status, nameof(status));
        RollbackTargetVersion = LifecycleRules.RequireIdentifier(rollbackTargetVersion, nameof(rollbackTargetVersion));
        RollbackAuthorityStillValid = rollbackAuthorityStillValid;
        RollbackDigest = LifecycleCanonicalization.Hash(("evidence_identity", EvidenceIdentity), ("status", Status.ToString()),
            ("rollback_target_version", RollbackTargetVersion), ("rollback_authority_still_valid", rollbackAuthorityStillValid ? "true" : "false"));
    }
    public string EvidenceIdentity { get; }
    public LifecycleEvidenceStatus Status { get; }
    public string RollbackTargetVersion { get; }
    public bool RollbackAuthorityStillValid { get; }
    public string RollbackDigest { get; }
}

public sealed record LifecycleRequest
{
    public LifecycleRequest(string requestIdentity, string subjectIdentity, LifecycleTransitionKind transition, LifecycleState currentState,
        string currentVersion, string targetVersion, LifecycleAuthorityEvidence authority, LifecycleEvidence manifestEvidence,
        LifecycleEvidence dependencyEvidence, LifecycleEvidence compatibilityEvidence, LifecycleEvidence securityEvidence,
        LifecycleContinuityEvidence continuityEvidence, LifecycleDrainEvidence drainEvidence, LifecycleRollbackEvidence? rollbackEvidence,
        string correlationIdentity, string causationIdentity)
    {
        RequestIdentity = LifecycleRules.RequireIdentifier(requestIdentity, nameof(requestIdentity));
        SubjectIdentity = LifecycleRules.RequireIdentifier(subjectIdentity, nameof(subjectIdentity));
        Transition = LifecycleRules.RequireDefined(transition, nameof(transition));
        CurrentState = LifecycleRules.RequireDefined(currentState, nameof(currentState));
        CurrentVersion = LifecycleRules.RequireOptionalIdentifier(currentVersion, nameof(currentVersion));
        TargetVersion = LifecycleRules.RequireOptionalIdentifier(targetVersion, nameof(targetVersion));
        Authority = authority ?? throw new ArgumentNullException(nameof(authority));
        ManifestEvidence = manifestEvidence ?? throw new ArgumentNullException(nameof(manifestEvidence));
        DependencyEvidence = dependencyEvidence ?? throw new ArgumentNullException(nameof(dependencyEvidence));
        CompatibilityEvidence = compatibilityEvidence ?? throw new ArgumentNullException(nameof(compatibilityEvidence));
        SecurityEvidence = securityEvidence ?? throw new ArgumentNullException(nameof(securityEvidence));
        ContinuityEvidence = continuityEvidence ?? throw new ArgumentNullException(nameof(continuityEvidence));
        DrainEvidence = drainEvidence ?? throw new ArgumentNullException(nameof(drainEvidence));
        RollbackEvidence = rollbackEvidence;
        CorrelationIdentity = LifecycleRules.RequireOptionalIdentifier(correlationIdentity, nameof(correlationIdentity));
        CausationIdentity = LifecycleRules.RequireOptionalIdentifier(causationIdentity, nameof(causationIdentity));
        RequestDigest = LifecycleCanonicalization.Hash(
            ("request_identity", RequestIdentity), ("subject_identity", SubjectIdentity), ("transition", Transition.ToString()),
            ("current_state", CurrentState.ToString()), ("current_version", CurrentVersion), ("target_version", TargetVersion),
            ("authority", Authority.AuthorityEvidenceDigest), ("manifest", ManifestEvidence.EvidenceDigest),
            ("dependency", DependencyEvidence.EvidenceDigest), ("compatibility", CompatibilityEvidence.EvidenceDigest),
            ("security", SecurityEvidence.EvidenceDigest), ("continuity", ContinuityEvidence.ContinuityDigest),
            ("drain", DrainEvidence.DrainDigest), ("rollback", RollbackEvidence?.RollbackDigest ?? string.Empty),
            ("correlation", CorrelationIdentity), ("causation", CausationIdentity));
    }
    public string RequestIdentity { get; }
    public string SubjectIdentity { get; }
    public LifecycleTransitionKind Transition { get; }
    public LifecycleState CurrentState { get; }
    public string CurrentVersion { get; }
    public string TargetVersion { get; }
    public LifecycleAuthorityEvidence Authority { get; }
    public LifecycleEvidence ManifestEvidence { get; }
    public LifecycleEvidence DependencyEvidence { get; }
    public LifecycleEvidence CompatibilityEvidence { get; }
    public LifecycleEvidence SecurityEvidence { get; }
    public LifecycleContinuityEvidence ContinuityEvidence { get; }
    public LifecycleDrainEvidence DrainEvidence { get; }
    public LifecycleRollbackEvidence? RollbackEvidence { get; }
    public string CorrelationIdentity { get; }
    public string CausationIdentity { get; }
    public string RequestDigest { get; }
}

public sealed record LifecycleDecision
{
    internal LifecycleDecision(LifecycleDecisionKind kind, string reason, LifecycleRequest request)
    {
        Kind = LifecycleRules.RequireDefined(kind, nameof(kind));
        Reason = LifecycleRules.RequireIdentifier(reason, nameof(reason));
        SubjectIdentity = request.SubjectIdentity;
        Transition = request.Transition;
        CurrentVersion = request.CurrentVersion;
        TargetVersion = request.TargetVersion;
        CorrelationIdentity = request.CorrelationIdentity;
        CausationIdentity = request.CausationIdentity;
        DecisionIdentity = LifecycleCanonicalization.Hash(("kind", Kind.ToString()), ("reason", Reason), ("request_digest", request.RequestDigest));
    }
    public LifecycleDecisionKind Kind { get; }
    public string Reason { get; }
    public string SubjectIdentity { get; }
    public LifecycleTransitionKind Transition { get; }
    public string CurrentVersion { get; }
    public string TargetVersion { get; }
    public string CorrelationIdentity { get; }
    public string CausationIdentity { get; }
    public string DecisionIdentity { get; }
}

public sealed class ApplicationLifecycleEvaluator
{
    public LifecycleDecision Evaluate(LifecycleRequest request)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        var failure = ValidateAuthority(request) ?? ValidateCommonPrerequisites(request);
        if (failure is not null) return Reject(request, failure);
        return request.Transition switch
        {
            LifecycleTransitionKind.Attach => EvaluateAttach(request),
            LifecycleTransitionKind.UpgradeOrReplace => EvaluateUpgrade(request),
            LifecycleTransitionKind.DetachOrRemove => EvaluateDetach(request),
            LifecycleTransitionKind.Rollback => EvaluateRollback(request),
            _ => Reject(request, LifecycleReason.InvalidTransition)
        };
    }

    private static LifecycleDecision EvaluateAttach(LifecycleRequest request)
    {
        if (request.CurrentState is not (LifecycleState.Detached or LifecycleState.Removed)) return Reject(request, LifecycleReason.InvalidTransition);
        if (request.TargetVersion.Length == 0) return Reject(request, LifecycleReason.TargetVersionRequired);
        return Allow(request, LifecycleReason.AttachEligible);
    }

    private static LifecycleDecision EvaluateUpgrade(LifecycleRequest request)
    {
        if (request.CurrentState is not (LifecycleState.Attached or LifecycleState.Draining)) return Reject(request, LifecycleReason.InvalidTransition);
        if (request.CurrentVersion.Length == 0 || request.TargetVersion.Length == 0) return Reject(request, LifecycleReason.TargetVersionRequired);
        if (string.Equals(request.CurrentVersion, request.TargetVersion, StringComparison.Ordinal)) return Reject(request, LifecycleReason.TargetVersionUnchanged);
        return EvaluateDrain(request) ?? Allow(request, LifecycleReason.UpgradeEligible);
    }

    private static LifecycleDecision EvaluateDetach(LifecycleRequest request)
    {
        if (request.CurrentState is not (LifecycleState.Attached or LifecycleState.Draining or LifecycleState.RollbackRequired))
            return Reject(request, LifecycleReason.InvalidTransition);
        var drainDecision = EvaluateDrain(request);
        if (drainDecision is not null) return drainDecision;
        if (!request.ContinuityEvidence.HiddenCouplingAbsent) return Reject(request, LifecycleReason.HiddenCoupling);
        return Allow(request, LifecycleReason.DetachEligible);
    }

    private static LifecycleDecision EvaluateRollback(LifecycleRequest request)
    {
        if (request.CurrentState is not (LifecycleState.RollbackRequired or LifecycleState.Attached or LifecycleState.Draining))
            return Reject(request, LifecycleReason.InvalidTransition);
        if (request.RollbackEvidence is null || request.RollbackEvidence.Status != LifecycleEvidenceStatus.Valid)
            return Reject(request, LifecycleReason.RollbackEvidenceInvalid);
        if (!string.Equals(request.RollbackEvidence.RollbackTargetVersion, request.TargetVersion, StringComparison.Ordinal))
            return Reject(request, LifecycleReason.RollbackTargetMismatch);
        if (!request.RollbackEvidence.RollbackAuthorityStillValid) return Reject(request, LifecycleReason.RollbackAuthorityInvalid);
        return Allow(request, LifecycleReason.RollbackEligible);
    }

    private static LifecycleDecision? EvaluateDrain(LifecycleRequest request)
    {
        if (!request.DrainEvidence.DrainRequired) return null;
        if (request.DrainEvidence.Status == LifecycleEvidenceStatus.Missing)
            return new LifecycleDecision(LifecycleDecisionKind.DrainRequired, LifecycleReason.DrainRequired, request);
        if (request.DrainEvidence.Status != LifecycleEvidenceStatus.Valid)
            return Reject(request, LifecycleReason.DrainEvidenceInvalid);
        if (!request.DrainEvidence.DrainComplete)
            return new LifecycleDecision(LifecycleDecisionKind.DrainRequired, LifecycleReason.DrainRequired, request);
        return null;
    }

    private static string? ValidateAuthority(LifecycleRequest request)
    {
        var statusFailure = request.Authority.Status switch
        {
            LifecycleEvidenceStatus.Valid => null,
            LifecycleEvidenceStatus.Missing => LifecycleReason.AuthorityMissing,
            LifecycleEvidenceStatus.Stale => LifecycleReason.AuthorityStale,
            LifecycleEvidenceStatus.Revoked => LifecycleReason.AuthorityRevoked,
            LifecycleEvidenceStatus.Invalid => LifecycleReason.AuthorityInvalid,
            LifecycleEvidenceStatus.Ambiguous => LifecycleReason.AuthorityAmbiguous,
            _ => LifecycleReason.AuthorityInvalid
        };
        if (statusFailure is not null) return statusFailure;
        if (!string.Equals(request.Authority.SubjectIdentity, request.SubjectIdentity, StringComparison.Ordinal)) return LifecycleReason.AuthoritySubjectMismatch;
        if (request.Authority.PermittedTransition != request.Transition) return LifecycleReason.AuthorityTransitionMismatch;
        if (!string.Equals(request.Authority.CurrentVersion, request.CurrentVersion, StringComparison.Ordinal) ||
            !string.Equals(request.Authority.TargetVersion, request.TargetVersion, StringComparison.Ordinal)) return LifecycleReason.AuthorityVersionMismatch;
        return null;
    }

    private static string? ValidateCommonPrerequisites(LifecycleRequest request)
    {
        if (request.ManifestEvidence.Status != LifecycleEvidenceStatus.Valid) return LifecycleReason.ManifestInvalid;
        if (request.DependencyEvidence.Status != LifecycleEvidenceStatus.Valid || request.ContinuityEvidence.Status != LifecycleEvidenceStatus.Valid ||
            !request.ContinuityEvidence.RequiredDependenciesSatisfied) return LifecycleReason.DependencyInvalid;
        if (request.CompatibilityEvidence.Status != LifecycleEvidenceStatus.Valid || !request.ContinuityEvidence.ContractsCompatible) return LifecycleReason.CompatibilityInvalid;
        if (request.SecurityEvidence.Status != LifecycleEvidenceStatus.Valid) return LifecycleReason.SecurityInvalid;
        if (!request.ContinuityEvidence.AuthorityDoesNotExpand) return LifecycleReason.AuthorityExpansion;
        if (!request.ContinuityEvidence.ProtectedControlsNotWeakened) return LifecycleReason.ProtectedControlWeakening;
        return null;
    }

    private static LifecycleDecision Allow(LifecycleRequest request, string reason) => new(LifecycleDecisionKind.Allowed, reason, request);
    private static LifecycleDecision Reject(LifecycleRequest request, string reason) => new(LifecycleDecisionKind.Rejected, reason, request);
}

internal static class LifecycleRules
{
    public static string RequireIdentifier(string value, string parameterName)
    {
        if (value is null) throw new ArgumentNullException(parameterName);
        var canonical = value.Trim();
        if (canonical.Length == 0) throw new ArgumentException("identifier_required", parameterName);
        if (canonical.Length > 512) throw new ArgumentException("identifier_too_long", parameterName);
        if (canonical.IndexOfAny(new[] { '\r', '\n', '\0' }) >= 0) throw new ArgumentException("identifier_invalid", parameterName);
        return canonical;
    }
    public static string RequireOptionalIdentifier(string value, string parameterName)
    {
        if (value is null) throw new ArgumentNullException(parameterName);
        return value.Length == 0 ? string.Empty : RequireIdentifier(value, parameterName);
    }
    public static T RequireDefined<T>(T value, string parameterName) where T : struct, Enum
    {
        if (!Enum.IsDefined(value)) throw new ArgumentOutOfRangeException(parameterName);
        return value;
    }
}

internal static class LifecycleCanonicalization
{
    public static string Hash(params (string Name, string Value)[] fields) => Convert.ToHexString(SHA256.HashData(Serialize(fields)));
    private static byte[] Serialize(IReadOnlyList<(string Name, string Value)> fields)
    {
        var builder = new StringBuilder();
        for (var i = 0; i < fields.Count; i++)
        {
            var name = fields[i].Name ?? string.Empty;
            var value = fields[i].Value ?? string.Empty;
            builder.Append(name.Length).Append(':').Append(name).Append('=').Append(value.Length).Append(':').Append(value).Append(';');
        }
        return Encoding.UTF8.GetBytes(builder.ToString());
    }
}
