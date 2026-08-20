using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Foundation.Contracts;

namespace Foundation.Reconciliation;

public enum RecoveryReleaseAuthorizationClassification
{
    Authorized = 1,
    Denied = 2,
    Uncertain = 3
}

public static class RecoveryReleaseAuthorizationReason
{
    public const string Pass = "RELEASE_AUTHORIZED";
    public const string InvalidInput = "INVALID_RECOVERY_RELEASE_AUTHORIZATION_INPUT";
    public const string ReadinessNotReady = "RECOVERY_RELEASE_READINESS_NOT_READY";
    public const string ReadinessIdentityMismatch = "RECOVERY_RELEASE_READINESS_IDENTITY_MISMATCH";
    public const string RecoveryBindingMismatch = "RECOVERY_RELEASE_AUTHORIZATION_BINDING_MISMATCH";
    public const string ReleaseAuthorityMismatch = "DECLARED_RELEASE_AUTHORITY_MISMATCH";
    public const string ReleaseAuthorityRoleConflict = "RELEASE_AUTHORITY_ROLE_CONFLICT";
    public const string AuthorityRequestMismatch = "RELEASE_AUTHORITY_REQUEST_MISMATCH";
    public const string AuthorityResultInvalid = "RELEASE_AUTHORITY_RESULT_INVALID";
    public const string AuthorityResultMismatch = "RELEASE_AUTHORITY_RESULT_MISMATCH";
    public const string AuthorityDenied = "RELEASE_AUTHORITY_DENIED";
    public const string AuthorityExpired = "RELEASE_AUTHORITY_EXPIRED";
    public const string RestrictionChanged = "CONTROLLING_RESTRICTION_CHANGED";
    public const string NewerStricterRestriction = "NEWER_OR_STRICTER_CONTROLLING_RESTRICTION_PRESENT";
    public const string ReconciliationChanged = "RECOVERY_RECONCILIATION_TRUST_SNAPSHOT_CHANGED";
    public const string SecurityStateChanged = "SECURITY_TRUST_SNAPSHOT_CHANGED";
    public const string DependencyStateChanged = "DEPENDENCY_TRUST_SNAPSHOT_CHANGED";
    public const string ResidualRiskChanged = "RESIDUAL_RISK_SNAPSHOT_CHANGED";
    public const string MaterialTrustUncertain = "MATERIAL_TRUST_SNAPSHOT_NOT_CURRENT_OR_TRUSTED";
}

public sealed record RecoveryReleaseTrustSnapshot(
    string CurrentControllingRestrictionIdentity,
    string CurrentRestrictionIntegrityEvidenceIdentity,
    bool NewerOrStricterRestrictionPresent,
    string RecoveryReconciliationIdentity,
    bool ReconciliationCurrent,
    bool ReconciliationTrusted,
    string SecurityStateEvidenceIdentity,
    bool SecurityStateCurrent,
    bool SecurityStateTrusted,
    string DependencyStateEvidenceIdentity,
    bool DependencyStateCurrent,
    bool DependencyStateTrusted,
    string ResidualRiskEvidenceIdentity,
    string ResidualRiskProfileIdentity,
    bool ResidualRiskCurrent,
    bool ResidualRiskTrusted,
    bool ResidualRiskWithinAuthorizedBounds);

public sealed record RecoveryReleaseAuthorizationInput(
    string RecoveryCaseIdentity,
    string AuthorizedRecoveryPlanIdentity,
    string SubjectIdentity,
    string GuardianIdentity,
    string RepairActorIdentity,
    string IndependentVerifierIdentity,
    string DeclaredReleaseAuthorityIdentity,
    string RecoveryReadinessIdentity,
    string ExpectedReleaseAction,
    string ExpectedReleaseResource,
    string ExpectedReleasePurpose,
    string ExpectedReleaseScope,
    RecoveryReleaseTrustSnapshot CurrentTrustSnapshot,
    DateTimeOffset AuthorizationTime);

public sealed record RecoveryReleaseAuthorizationDecision(
    RecoveryReleaseAuthorizationClassification Classification,
    string Reason,
    string RecoveryCaseIdentity,
    string AuthorizedRecoveryPlanIdentity,
    string SubjectIdentity,
    string CurrentControllingRestrictionIdentity,
    string CurrentRestrictionIntegrityEvidenceIdentity,
    string RecoveryReadinessIdentity,
    string DeclaredReleaseAuthorityIdentity,
    string AuthorityRequestIdentity,
    string AuthorityDecisionIdentity,
    string AuthorityPolicyIdentity,
    string AuthorityPolicyVersion,
    string AuthorityMaterialConditions,
    string AuthorityConstraints,
    string AuthorityEvidenceIdentity,
    string ResidualRiskEvidenceIdentity,
    string ResidualRiskProfileIdentity,
    DateTimeOffset AuthorizationTime,
    DateTimeOffset AuthorityExpiry)
{
    public string Identity => RecoveryReleaseAuthorizationIdentity.Compute(this);
}

public static class RecoveryReleaseAuthorizationEvaluator
{
    public static RecoveryReleaseAuthorizationDecision Evaluate(
        RecoveryReleaseReadinessDecision readiness,
        AuthorityRequest authorityRequest,
        AuthorityResult authorityResult,
        RecoveryReleaseAuthorizationInput input)
    {
        if (!ValidInput(input) ||
            ContractValidators.Validate(authorityRequest).Result != ValidationResult.Pass)
        {
            return Create(readiness, authorityRequest, authorityResult, input,
                RecoveryReleaseAuthorizationClassification.Denied,
                RecoveryReleaseAuthorizationReason.InvalidInput);
        }

        if (readiness.Classification != RecoveryReleaseReadinessClassification.ReadyForReleaseDecision)
        {
            return Create(readiness, authorityRequest, authorityResult, input,
                readiness.Classification == RecoveryReleaseReadinessClassification.Uncertain
                    ? RecoveryReleaseAuthorizationClassification.Uncertain
                    : RecoveryReleaseAuthorizationClassification.Denied,
                RecoveryReleaseAuthorizationReason.ReadinessNotReady);
        }

        if (!Same(input.RecoveryReadinessIdentity, readiness.Identity))
        {
            return Create(readiness, authorityRequest, authorityResult, input,
                RecoveryReleaseAuthorizationClassification.Denied,
                RecoveryReleaseAuthorizationReason.ReadinessIdentityMismatch);
        }

        if (!Same(input.RecoveryCaseIdentity, readiness.RecoveryCaseIdentity) ||
            !Same(input.AuthorizedRecoveryPlanIdentity, readiness.AuthorizedRecoveryPlanIdentity))
        {
            return Create(readiness, authorityRequest, authorityResult, input,
                RecoveryReleaseAuthorizationClassification.Denied,
                RecoveryReleaseAuthorizationReason.RecoveryBindingMismatch);
        }

        if (!Same(input.DeclaredReleaseAuthorityIdentity, readiness.DeclaredReleaseAuthorityIdentity))
        {
            return Create(readiness, authorityRequest, authorityResult, input,
                RecoveryReleaseAuthorizationClassification.Denied,
                RecoveryReleaseAuthorizationReason.ReleaseAuthorityMismatch);
        }

        if (!ReleaseAuthoritySeparated(input))
        {
            return Create(readiness, authorityRequest, authorityResult, input,
                RecoveryReleaseAuthorizationClassification.Denied,
                RecoveryReleaseAuthorizationReason.ReleaseAuthorityRoleConflict);
        }

        if (!AuthorityRequestMatches(readiness, authorityRequest, input))
        {
            return Create(readiness, authorityRequest, authorityResult, input,
                RecoveryReleaseAuthorizationClassification.Denied,
                RecoveryReleaseAuthorizationReason.AuthorityRequestMismatch);
        }

        if (ContractValidators.Validate(authorityResult).Result != ValidationResult.Pass)
        {
            return Create(readiness, authorityRequest, authorityResult, input,
                RecoveryReleaseAuthorizationClassification.Denied,
                RecoveryReleaseAuthorizationReason.AuthorityResultInvalid);
        }

        if (!Same(authorityResult.RequestId, authorityRequest.RequestId) ||
            !Same(authorityResult.EffectiveScope,
                string.Equals(authorityResult.Decision, "ALLOW", StringComparison.Ordinal)
                    ? authorityRequest.RequestedScope
                    : "NONE") ||
            authorityResult.DecisionTime != input.AuthorizationTime)
        {
            return Create(readiness, authorityRequest, authorityResult, input,
                RecoveryReleaseAuthorizationClassification.Denied,
                RecoveryReleaseAuthorizationReason.AuthorityResultMismatch);
        }

        if (input.AuthorizationTime < readiness.EvaluatedAt ||
            authorityResult.Expiry <= input.AuthorizationTime)
        {
            return Create(readiness, authorityRequest, authorityResult, input,
                RecoveryReleaseAuthorizationClassification.Denied,
                RecoveryReleaseAuthorizationReason.AuthorityExpired);
        }

        if (!Same(input.CurrentTrustSnapshot.CurrentControllingRestrictionIdentity,
                readiness.CurrentControllingRestrictionIdentity) ||
            !Same(input.CurrentTrustSnapshot.CurrentRestrictionIntegrityEvidenceIdentity,
                readiness.CurrentRestrictionIntegrityEvidenceIdentity))
        {
            return Create(readiness, authorityRequest, authorityResult, input,
                RecoveryReleaseAuthorizationClassification.Denied,
                RecoveryReleaseAuthorizationReason.RestrictionChanged);
        }

        if (input.CurrentTrustSnapshot.NewerOrStricterRestrictionPresent)
        {
            return Create(readiness, authorityRequest, authorityResult, input,
                RecoveryReleaseAuthorizationClassification.Denied,
                RecoveryReleaseAuthorizationReason.NewerStricterRestriction);
        }

        if (!Same(input.CurrentTrustSnapshot.RecoveryReconciliationIdentity,
                readiness.RecoveryReconciliationIdentity))
        {
            return Create(readiness, authorityRequest, authorityResult, input,
                RecoveryReleaseAuthorizationClassification.Denied,
                RecoveryReleaseAuthorizationReason.ReconciliationChanged);
        }

        if (!Same(input.CurrentTrustSnapshot.SecurityStateEvidenceIdentity,
                readiness.SecurityStateEvidenceIdentity))
        {
            return Create(readiness, authorityRequest, authorityResult, input,
                RecoveryReleaseAuthorizationClassification.Denied,
                RecoveryReleaseAuthorizationReason.SecurityStateChanged);
        }

        if (!Same(input.CurrentTrustSnapshot.DependencyStateEvidenceIdentity,
                readiness.DependencyStateEvidenceIdentity))
        {
            return Create(readiness, authorityRequest, authorityResult, input,
                RecoveryReleaseAuthorizationClassification.Denied,
                RecoveryReleaseAuthorizationReason.DependencyStateChanged);
        }

        if (!Same(input.CurrentTrustSnapshot.ResidualRiskEvidenceIdentity,
                readiness.ResidualRiskEvidenceIdentity) ||
            !Same(input.CurrentTrustSnapshot.ResidualRiskProfileIdentity,
                readiness.ResidualRiskProfileIdentity) ||
            !input.CurrentTrustSnapshot.ResidualRiskWithinAuthorizedBounds)
        {
            return Create(readiness, authorityRequest, authorityResult, input,
                RecoveryReleaseAuthorizationClassification.Denied,
                RecoveryReleaseAuthorizationReason.ResidualRiskChanged);
        }

        if (!MaterialTrustCurrentAndTrusted(input.CurrentTrustSnapshot))
        {
            return Create(readiness, authorityRequest, authorityResult, input,
                RecoveryReleaseAuthorizationClassification.Uncertain,
                RecoveryReleaseAuthorizationReason.MaterialTrustUncertain);
        }

        if (!string.Equals(authorityResult.Decision, "ALLOW", StringComparison.Ordinal))
        {
            return Create(readiness, authorityRequest, authorityResult, input,
                RecoveryReleaseAuthorizationClassification.Denied,
                RecoveryReleaseAuthorizationReason.AuthorityDenied);
        }

        return Create(readiness, authorityRequest, authorityResult, input,
            RecoveryReleaseAuthorizationClassification.Authorized,
            RecoveryReleaseAuthorizationReason.Pass);
    }

    private static bool AuthorityRequestMatches(
        RecoveryReleaseReadinessDecision readiness,
        AuthorityRequest request,
        RecoveryReleaseAuthorizationInput input) =>
        Same(request.ActorIdentity, input.DeclaredReleaseAuthorityIdentity) &&
        Same(request.Action, input.ExpectedReleaseAction) &&
        Same(request.Resource, input.ExpectedReleaseResource) &&
        Same(request.Purpose, input.ExpectedReleasePurpose) &&
        Same(request.RequestedScope, input.ExpectedReleaseScope) &&
        Same(request.Correlation, readiness.Identity) &&
        request.RequestTime <= input.AuthorizationTime &&
        request.Expiry > input.AuthorizationTime;

    private static bool ReleaseAuthoritySeparated(RecoveryReleaseAuthorizationInput input) =>
        !Same(input.DeclaredReleaseAuthorityIdentity, input.SubjectIdentity) &&
        !Same(input.DeclaredReleaseAuthorityIdentity, input.GuardianIdentity) &&
        !Same(input.DeclaredReleaseAuthorityIdentity, input.RepairActorIdentity) &&
        !Same(input.DeclaredReleaseAuthorityIdentity, input.IndependentVerifierIdentity);

    private static bool MaterialTrustCurrentAndTrusted(RecoveryReleaseTrustSnapshot snapshot) =>
        snapshot.ReconciliationCurrent && snapshot.ReconciliationTrusted &&
        snapshot.SecurityStateCurrent && snapshot.SecurityStateTrusted &&
        snapshot.DependencyStateCurrent && snapshot.DependencyStateTrusted &&
        snapshot.ResidualRiskCurrent && snapshot.ResidualRiskTrusted;

    private static bool ValidInput(RecoveryReleaseAuthorizationInput input) =>
        Token(input.RecoveryCaseIdentity) &&
        Token(input.AuthorizedRecoveryPlanIdentity) &&
        Token(input.SubjectIdentity) &&
        Token(input.GuardianIdentity) &&
        Token(input.RepairActorIdentity) &&
        Token(input.IndependentVerifierIdentity) &&
        Token(input.DeclaredReleaseAuthorityIdentity) &&
        Token(input.RecoveryReadinessIdentity) &&
        Token(input.ExpectedReleaseAction) &&
        Token(input.ExpectedReleaseResource) &&
        Token(input.ExpectedReleasePurpose) &&
        Token(input.ExpectedReleaseScope) &&
        input.CurrentTrustSnapshot is not null &&
        ValidSnapshot(input.CurrentTrustSnapshot) &&
        input.AuthorizationTime != default;

    private static bool ValidSnapshot(RecoveryReleaseTrustSnapshot value) =>
        Token(value.CurrentControllingRestrictionIdentity) &&
        Token(value.CurrentRestrictionIntegrityEvidenceIdentity) &&
        Token(value.RecoveryReconciliationIdentity) &&
        Token(value.SecurityStateEvidenceIdentity) &&
        Token(value.DependencyStateEvidenceIdentity) &&
        Token(value.ResidualRiskEvidenceIdentity) &&
        Token(value.ResidualRiskProfileIdentity);

    private static RecoveryReleaseAuthorizationDecision Create(
        RecoveryReleaseReadinessDecision readiness,
        AuthorityRequest request,
        AuthorityResult result,
        RecoveryReleaseAuthorizationInput input,
        RecoveryReleaseAuthorizationClassification classification,
        string reason) =>
        new(
            classification,
            reason,
            Clean(input.RecoveryCaseIdentity),
            Clean(input.AuthorizedRecoveryPlanIdentity),
            Clean(input.SubjectIdentity),
            Clean(input.CurrentTrustSnapshot?.CurrentControllingRestrictionIdentity),
            Clean(input.CurrentTrustSnapshot?.CurrentRestrictionIntegrityEvidenceIdentity),
            Clean(input.RecoveryReadinessIdentity),
            Clean(input.DeclaredReleaseAuthorityIdentity),
            Clean(request.RequestId),
            Clean(result.DecisionId),
            Clean(result.ControllingPolicy),
            Clean(result.PolicyVersion),
            Clean(result.MaterialConditions),
            Clean(result.Constraints),
            Clean(result.EvidenceReference),
            Clean(input.CurrentTrustSnapshot?.ResidualRiskEvidenceIdentity),
            Clean(input.CurrentTrustSnapshot?.ResidualRiskProfileIdentity),
            input.AuthorizationTime == default ? DateTimeOffset.UnixEpoch : input.AuthorizationTime,
            result.Expiry == default ? DateTimeOffset.UnixEpoch : result.Expiry);

    private static bool Same(string? left, string? right) =>
        string.Equals(left, right, StringComparison.Ordinal);

    private static string Clean(string? value) =>
        string.IsNullOrWhiteSpace(value) ? "missing" : value.Trim();

    private static bool Token(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) || !string.Equals(value, value.Trim(), StringComparison.Ordinal))
            return false;

        foreach (var ch in value)
        {
            if (char.IsControl(ch) || char.IsWhiteSpace(ch))
                return false;
        }

        return true;
    }
}

internal static class RecoveryReleaseAuthorizationIdentity
{
    internal static string Compute(RecoveryReleaseAuthorizationDecision value)
    {
        var canonical = string.Join("\n", new[]
        {
            ((int)value.Classification).ToString(CultureInfo.InvariantCulture),
            value.Reason,
            value.RecoveryCaseIdentity,
            value.AuthorizedRecoveryPlanIdentity,
            value.SubjectIdentity,
            value.CurrentControllingRestrictionIdentity,
            value.CurrentRestrictionIntegrityEvidenceIdentity,
            value.RecoveryReadinessIdentity,
            value.DeclaredReleaseAuthorityIdentity,
            value.AuthorityRequestIdentity,
            value.AuthorityDecisionIdentity,
            value.AuthorityPolicyIdentity,
            value.AuthorityPolicyVersion,
            value.AuthorityMaterialConditions,
            value.AuthorityConstraints,
            value.AuthorityEvidenceIdentity,
            value.ResidualRiskEvidenceIdentity,
            value.ResidualRiskProfileIdentity,
            value.AuthorizationTime.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture),
            value.AuthorityExpiry.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)
        });

        var bytes = Encoding.UTF8.GetBytes("stage9-recovery-release-authorization-v1\n" + canonical);
        return Convert.ToHexString(SHA256.HashData(bytes));
    }
}
