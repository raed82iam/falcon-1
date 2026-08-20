using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Foundation.Contracts;

namespace Foundation.Reconciliation;

public enum RecoveryReintroductionClassification
{
    RecoveryComplete = 1,
    RecoveredWithRestrictedAuthority = 2,
    Failed = 3,
    Uncertain = 4
}

public enum RecoveryObservationMode
{
    None = 0,
    Heightened = 1,
    RecoveryGuard = 2
}

public static class RecoveryReintroductionReason
{
    public const string Complete = "RECOVERY_COMPLETE";
    public const string ObservationInProgress = "RECOVERY_GUARD_OBSERVATION_IN_PROGRESS";
    public const string InvalidInput = "INVALID_RECOVERY_REINTRODUCTION_INPUT";
    public const string ReleaseNotComplete = "VALID_WP08_RELEASE_FACT_REQUIRED";
    public const string ReleaseBindingMismatch = "WP08_RELEASE_FACT_BINDING_MISMATCH";
    public const string ReintroductionTrustInvalid = "REINTRODUCTION_TRUST_RECHECK_FAILED";
    public const string LifecycleRequestInvalid = "LIFECYCLE_REINTRODUCTION_REQUEST_INVALID";
    public const string LifecycleResultInvalid = "LIFECYCLE_REINTRODUCTION_RESULT_INVALID";
    public const string LifecycleBindingMismatch = "LIFECYCLE_REINTRODUCTION_BINDING_MISMATCH";
    public const string LifecycleTransitionNotAccepted = "LIFECYCLE_REINTRODUCTION_NOT_ACCEPTED";
    public const string LifecycleTransitionBeforeRelease = "LIFECYCLE_REINTRODUCTION_PRECEDES_RELEASE";
    public const string DirectRunningWithoutValidatedRelease = "DIRECT_RUNNING_WITHOUT_VALIDATED_RELEASE_DENIED";
    public const string NewAuthorityRequestInvalid = "NEW_AUTHORITY_REQUEST_INVALID";
    public const string NewAuthorityResultInvalid = "NEW_AUTHORITY_RESULT_INVALID";
    public const string NewAuthorityBindingMismatch = "NEW_AUTHORITY_DECISION_BINDING_MISMATCH";
    public const string OldAuthorityReuseDenied = "OLD_PRE_RESTRICTION_AUTHORITY_REUSE_DENIED";
    public const string NewAuthorityDenied = "NEW_AUTHORITY_DECISION_DENIED";
    public const string NewAuthorityExpired = "NEW_AUTHORITY_DECISION_EXPIRED";
    public const string ObservationRequired = "RECOVERY_GUARD_OBSERVATION_REQUIRED";
    public const string ObservationUncertain = "RECOVERY_GUARD_OBSERVATION_UNCERTAIN";
    public const string ObservationFailed = "RECOVERY_GUARD_OBSERVATION_FAILED";
    public const string ObservationExitInvalid = "RECOVERY_GUARD_EXIT_NOT_GOVERNED";
}

public sealed record RecoveryReintroductionTrustEvidence(
    string IdentityEvidenceIdentity,
    bool IdentityCurrent,
    bool IdentityTrusted,
    string ConfigurationEvidenceIdentity,
    bool ConfigurationCurrent,
    bool ConfigurationTrusted,
    string DependencyEvidenceIdentity,
    bool DependencyCurrent,
    bool DependencyTrusted,
    string SecurityEvidenceIdentity,
    bool SecurityCurrent,
    bool SecurityTrusted)
{
    public string Identity => RecoveryReintroductionIdentity.ComputeTrust(this);
}

public sealed record RecoveryObservationEvidence(
    RecoveryObservationMode Mode,
    string EvidenceIdentity,
    bool Current,
    bool Trusted,
    bool Satisfactory,
    bool ExitAuthorized,
    string ExitEvidenceIdentity,
    DateTimeOffset ObservedAt)
{
    public string Identity => RecoveryReintroductionIdentity.ComputeObservation(this);
}

public sealed record RecoveryReintroductionInput(
    string RecoveryCaseIdentity,
    string SubjectIdentity,
    string ProtectiveRestrictionReleaseFactIdentity,
    string PriorRestrictedAuthorityDecisionIdentity,
    string ExpectedAuthorityAction,
    string ExpectedAuthorityPurpose,
    string ExpectedAuthorityScope,
    RecoveryReintroductionTrustEvidence Rechecks,
    bool ObservationRequired,
    RecoveryObservationEvidence Observation,
    string ResidualRiskEvidenceIdentity,
    string DataLossDeclarationIdentity,
    string CapabilityLossDeclarationIdentity,
    string ApprovalEvidenceIdentity,
    string FollowUpObligationsIdentity,
    DateTimeOffset EvaluationTime);

public sealed record RecoveryReintroductionDecision(
    RecoveryReintroductionClassification Classification,
    string Reason,
    string RecoveryCaseIdentity,
    string SubjectIdentity,
    string ProtectiveRestrictionReleaseFactIdentity,
    string LifecycleTransitionRequestIdentity,
    string LifecycleTransitionIdentity,
    string LifecycleResultingState,
    string NewAuthorityRequestIdentity,
    string NewAuthorityDecisionIdentity,
    string NewAuthorityEffectiveScope,
    string ReintroductionTrustEvidenceIdentity,
    string ObservationEvidenceIdentity,
    string ResidualRiskEvidenceIdentity,
    string DataLossDeclarationIdentity,
    string CapabilityLossDeclarationIdentity,
    string ApprovalEvidenceIdentity,
    string FollowUpObligationsIdentity,
    bool ObservationRequired,
    bool ObservationExitAuthorized,
    DateTimeOffset EvaluatedAt)
{
    public string Identity => RecoveryReintroductionIdentity.ComputeDecision(this);
}

public static class RecoveryReintroductionEvaluator
{
    public static RecoveryReintroductionDecision Evaluate(
        ProtectiveRestrictionReleaseFact releaseFact,
        LifecycleTransitionRequest lifecycleRequest,
        LifecycleTransitionResult lifecycleResult,
        AuthorityRequest newAuthorityRequest,
        AuthorityResult newAuthorityResult,
        RecoveryReintroductionInput input)
    {
        if (!ValidInput(input))
            return Create(releaseFact, lifecycleRequest, lifecycleResult, newAuthorityRequest, newAuthorityResult, input,
                RecoveryReintroductionClassification.Failed,
                RecoveryReintroductionReason.InvalidInput);

        if (releaseFact.Classification != ProtectiveRestrictionReleaseClassification.Released)
            return Create(releaseFact, lifecycleRequest, lifecycleResult, newAuthorityRequest, newAuthorityResult, input,
                releaseFact.Classification == ProtectiveRestrictionReleaseClassification.Uncertain
                    ? RecoveryReintroductionClassification.Uncertain
                    : RecoveryReintroductionClassification.Failed,
                RecoveryReintroductionReason.ReleaseNotComplete);

        if (!Same(input.ProtectiveRestrictionReleaseFactIdentity, releaseFact.Identity) ||
            !Same(input.RecoveryCaseIdentity, releaseFact.RecoveryCaseIdentity) ||
            !Same(input.SubjectIdentity, releaseFact.SubjectIdentity))
            return Create(releaseFact, lifecycleRequest, lifecycleResult, newAuthorityRequest, newAuthorityResult, input,
                RecoveryReintroductionClassification.Failed,
                RecoveryReintroductionReason.ReleaseBindingMismatch);

        if (!Trustworthy(input.Rechecks))
            return Create(releaseFact, lifecycleRequest, lifecycleResult, newAuthorityRequest, newAuthorityResult, input,
                RecoveryReintroductionClassification.Uncertain,
                RecoveryReintroductionReason.ReintroductionTrustInvalid);

        if (ContractValidators.Validate(lifecycleRequest).Result != ValidationResult.Pass)
            return Create(releaseFact, lifecycleRequest, lifecycleResult, newAuthorityRequest, newAuthorityResult, input,
                RecoveryReintroductionClassification.Failed,
                RecoveryReintroductionReason.LifecycleRequestInvalid);

        if (!LifecycleRequestMatches(releaseFact, lifecycleRequest, input))
            return Create(releaseFact, lifecycleRequest, lifecycleResult, newAuthorityRequest, newAuthorityResult, input,
                RecoveryReintroductionClassification.Failed,
                RecoveryReintroductionReason.LifecycleBindingMismatch);

        if (ContractValidators.Validate(lifecycleResult).Result != ValidationResult.Pass)
            return Create(releaseFact, lifecycleRequest, lifecycleResult, newAuthorityRequest, newAuthorityResult, input,
                RecoveryReintroductionClassification.Failed,
                RecoveryReintroductionReason.LifecycleResultInvalid);

        if (!LifecycleResultMatches(lifecycleRequest, lifecycleResult))
            return Create(releaseFact, lifecycleRequest, lifecycleResult, newAuthorityRequest, newAuthorityResult, input,
                RecoveryReintroductionClassification.Failed,
                RecoveryReintroductionReason.LifecycleBindingMismatch);

        if (!string.Equals(lifecycleResult.Decision, "ACCEPTED", StringComparison.Ordinal))
            return Create(releaseFact, lifecycleRequest, lifecycleResult, newAuthorityRequest, newAuthorityResult, input,
                RecoveryReintroductionClassification.Failed,
                RecoveryReintroductionReason.LifecycleTransitionNotAccepted);

        if (!Same(lifecycleResult.ActualResultingState, lifecycleRequest.RequestedTargetState))
            return Create(releaseFact, lifecycleRequest, lifecycleResult, newAuthorityRequest, newAuthorityResult, input,
                RecoveryReintroductionClassification.Failed,
                RecoveryReintroductionReason.LifecycleBindingMismatch);

        if (lifecycleResult.CompletionTime < releaseFact.EffectiveBoundary)
            return Create(releaseFact, lifecycleRequest, lifecycleResult, newAuthorityRequest, newAuthorityResult, input,
                RecoveryReintroductionClassification.Failed,
                RecoveryReintroductionReason.LifecycleTransitionBeforeRelease);

        if (string.Equals(lifecycleRequest.RequestedTargetState, "RUNNING", StringComparison.Ordinal) &&
            (releaseFact.Classification != ProtectiveRestrictionReleaseClassification.Released || !Trustworthy(input.Rechecks)))
            return Create(releaseFact, lifecycleRequest, lifecycleResult, newAuthorityRequest, newAuthorityResult, input,
                RecoveryReintroductionClassification.Failed,
                RecoveryReintroductionReason.DirectRunningWithoutValidatedRelease);

        if (ContractValidators.Validate(newAuthorityRequest).Result != ValidationResult.Pass)
            return Create(releaseFact, lifecycleRequest, lifecycleResult, newAuthorityRequest, newAuthorityResult, input,
                RecoveryReintroductionClassification.Failed,
                RecoveryReintroductionReason.NewAuthorityRequestInvalid);

        if (!NewAuthorityRequestMatches(lifecycleResult, newAuthorityRequest, input))
            return Create(releaseFact, lifecycleRequest, lifecycleResult, newAuthorityRequest, newAuthorityResult, input,
                RecoveryReintroductionClassification.Failed,
                RecoveryReintroductionReason.NewAuthorityBindingMismatch);

        if (ContractValidators.Validate(newAuthorityResult).Result != ValidationResult.Pass)
            return Create(releaseFact, lifecycleRequest, lifecycleResult, newAuthorityRequest, newAuthorityResult, input,
                RecoveryReintroductionClassification.Failed,
                RecoveryReintroductionReason.NewAuthorityResultInvalid);

        if (!NewAuthorityResultMatches(newAuthorityRequest, newAuthorityResult, input))
            return Create(releaseFact, lifecycleRequest, lifecycleResult, newAuthorityRequest, newAuthorityResult, input,
                RecoveryReintroductionClassification.Failed,
                RecoveryReintroductionReason.NewAuthorityBindingMismatch);

        if (Same(input.PriorRestrictedAuthorityDecisionIdentity, newAuthorityResult.DecisionId))
            return Create(releaseFact, lifecycleRequest, lifecycleResult, newAuthorityRequest, newAuthorityResult, input,
                RecoveryReintroductionClassification.Failed,
                RecoveryReintroductionReason.OldAuthorityReuseDenied);

        if (!string.Equals(newAuthorityResult.Decision, "ALLOW", StringComparison.Ordinal))
            return Create(releaseFact, lifecycleRequest, lifecycleResult, newAuthorityRequest, newAuthorityResult, input,
                RecoveryReintroductionClassification.Failed,
                RecoveryReintroductionReason.NewAuthorityDenied);

        if (newAuthorityResult.Expiry <= input.EvaluationTime)
            return Create(releaseFact, lifecycleRequest, lifecycleResult, newAuthorityRequest, newAuthorityResult, input,
                RecoveryReintroductionClassification.Failed,
                RecoveryReintroductionReason.NewAuthorityExpired);

        if (input.ObservationRequired)
        {
            if (input.Observation.Mode is not (RecoveryObservationMode.Heightened or RecoveryObservationMode.RecoveryGuard))
                return Create(releaseFact, lifecycleRequest, lifecycleResult, newAuthorityRequest, newAuthorityResult, input,
                    RecoveryReintroductionClassification.Failed,
                    RecoveryReintroductionReason.ObservationRequired);

            if (!input.Observation.Current || !input.Observation.Trusted)
                return Create(releaseFact, lifecycleRequest, lifecycleResult, newAuthorityRequest, newAuthorityResult, input,
                    RecoveryReintroductionClassification.Uncertain,
                    RecoveryReintroductionReason.ObservationUncertain);

            if (!input.Observation.Satisfactory)
                return Create(releaseFact, lifecycleRequest, lifecycleResult, newAuthorityRequest, newAuthorityResult, input,
                    RecoveryReintroductionClassification.Failed,
                    RecoveryReintroductionReason.ObservationFailed);

            if (!input.Observation.ExitAuthorized)
                return Create(releaseFact, lifecycleRequest, lifecycleResult, newAuthorityRequest, newAuthorityResult, input,
                    RecoveryReintroductionClassification.RecoveredWithRestrictedAuthority,
                    RecoveryReintroductionReason.ObservationInProgress);

            if (!Token(input.Observation.ExitEvidenceIdentity))
                return Create(releaseFact, lifecycleRequest, lifecycleResult, newAuthorityRequest, newAuthorityResult, input,
                    RecoveryReintroductionClassification.Failed,
                    RecoveryReintroductionReason.ObservationExitInvalid);
        }

        return Create(releaseFact, lifecycleRequest, lifecycleResult, newAuthorityRequest, newAuthorityResult, input,
            RecoveryReintroductionClassification.RecoveryComplete,
            RecoveryReintroductionReason.Complete);
    }

    private static bool LifecycleRequestMatches(
        ProtectiveRestrictionReleaseFact releaseFact,
        LifecycleTransitionRequest request,
        RecoveryReintroductionInput input) =>
        Same(request.ComponentIdentity, input.SubjectIdentity) &&
        OneOf(request.AuthoritativeSourceState, "RESTRICTED", "SUSPENDED", "RECOVERING") &&
        OneOf(request.RequestedTargetState, "READY", "RUNNING") &&
        Same(request.AuthorityReference, releaseFact.ReleaseAuthorizationIdentity) &&
        Same(request.DependencyContext, input.Rechecks.DependencyEvidenceIdentity) &&
        request.RequestTime >= releaseFact.EffectiveBoundary &&
        request.Expiry > input.EvaluationTime;

    private static bool LifecycleResultMatches(
        LifecycleTransitionRequest request,
        LifecycleTransitionResult result) =>
        Same(result.RequestId, request.TransitionRequestId) &&
        Same(result.SourceState, request.AuthoritativeSourceState) &&
        Same(result.TargetState, request.RequestedTargetState) &&
        result.CompletionTime >= request.RequestTime;

    private static bool NewAuthorityRequestMatches(
        LifecycleTransitionResult lifecycleResult,
        AuthorityRequest request,
        RecoveryReintroductionInput input) =>
        Same(request.Action, input.ExpectedAuthorityAction) &&
        Same(request.Resource, input.SubjectIdentity) &&
        Same(request.Purpose, input.ExpectedAuthorityPurpose) &&
        Same(request.RequestedScope, input.ExpectedAuthorityScope) &&
        Same(request.Correlation, lifecycleResult.TransitionId) &&
        request.RequestTime >= lifecycleResult.CompletionTime &&
        request.Expiry > input.EvaluationTime;

    private static bool NewAuthorityResultMatches(
        AuthorityRequest request,
        AuthorityResult result,
        RecoveryReintroductionInput input) =>
        Same(result.RequestId, request.RequestId) &&
        Same(result.EffectiveScope,
            string.Equals(result.Decision, "ALLOW", StringComparison.Ordinal)
                ? request.RequestedScope
                : "NONE") &&
        result.DecisionTime >= request.RequestTime &&
        result.DecisionTime <= input.EvaluationTime;

    private static bool Trustworthy(RecoveryReintroductionTrustEvidence value) =>
        value.IdentityCurrent && value.IdentityTrusted &&
        value.ConfigurationCurrent && value.ConfigurationTrusted &&
        value.DependencyCurrent && value.DependencyTrusted &&
        value.SecurityCurrent && value.SecurityTrusted;

    private static bool ValidInput(RecoveryReintroductionInput input) =>
        Token(input.RecoveryCaseIdentity) &&
        Token(input.SubjectIdentity) &&
        Token(input.ProtectiveRestrictionReleaseFactIdentity) &&
        Token(input.PriorRestrictedAuthorityDecisionIdentity) &&
        Token(input.ExpectedAuthorityAction) &&
        Token(input.ExpectedAuthorityPurpose) &&
        Token(input.ExpectedAuthorityScope) &&
        input.Rechecks is not null &&
        ValidRechecks(input.Rechecks) &&
        input.Observation is not null &&
        ValidObservation(input.Observation) &&
        Token(input.ResidualRiskEvidenceIdentity) &&
        Token(input.DataLossDeclarationIdentity) &&
        Token(input.CapabilityLossDeclarationIdentity) &&
        Token(input.ApprovalEvidenceIdentity) &&
        Token(input.FollowUpObligationsIdentity) &&
        input.EvaluationTime != default &&
        input.Observation.ObservedAt <= input.EvaluationTime;

    private static bool ValidRechecks(RecoveryReintroductionTrustEvidence value) =>
        Token(value.IdentityEvidenceIdentity) &&
        Token(value.ConfigurationEvidenceIdentity) &&
        Token(value.DependencyEvidenceIdentity) &&
        Token(value.SecurityEvidenceIdentity);

    private static bool ValidObservation(RecoveryObservationEvidence value) =>
        Enum.IsDefined(value.Mode) &&
        Token(value.EvidenceIdentity) &&
        value.ObservedAt != default &&
        (!value.ExitAuthorized || Token(value.ExitEvidenceIdentity));

    private static RecoveryReintroductionDecision Create(
        ProtectiveRestrictionReleaseFact releaseFact,
        LifecycleTransitionRequest lifecycleRequest,
        LifecycleTransitionResult lifecycleResult,
        AuthorityRequest authorityRequest,
        AuthorityResult authorityResult,
        RecoveryReintroductionInput input,
        RecoveryReintroductionClassification classification,
        string reason) =>
        new(
            classification,
            reason,
            Clean(input.RecoveryCaseIdentity),
            Clean(input.SubjectIdentity),
            Clean(input.ProtectiveRestrictionReleaseFactIdentity),
            Clean(lifecycleRequest.TransitionRequestId),
            Clean(lifecycleResult.TransitionId),
            Clean(lifecycleResult.ActualResultingState),
            Clean(authorityRequest.RequestId),
            Clean(authorityResult.DecisionId),
            Clean(authorityResult.EffectiveScope),
            input.Rechecks is null ? "missing" : input.Rechecks.Identity,
            input.Observation is null ? "missing" : input.Observation.Identity,
            Clean(input.ResidualRiskEvidenceIdentity),
            Clean(input.DataLossDeclarationIdentity),
            Clean(input.CapabilityLossDeclarationIdentity),
            Clean(input.ApprovalEvidenceIdentity),
            Clean(input.FollowUpObligationsIdentity),
            input.ObservationRequired,
            input.Observation?.ExitAuthorized ?? false,
            input.EvaluationTime == default ? DateTimeOffset.UnixEpoch : input.EvaluationTime);

    private static bool OneOf(string? value, params string[] expected)
    {
        foreach (var item in expected)
            if (string.Equals(value, item, StringComparison.Ordinal))
                return true;
        return false;
    }

    private static bool Same(string? left, string? right) =>
        string.Equals(left, right, StringComparison.Ordinal);

    private static string Clean(string? value) =>
        string.IsNullOrWhiteSpace(value) ? "missing" : value.Trim();

    private static bool Token(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) || !string.Equals(value, value.Trim(), StringComparison.Ordinal))
            return false;
        foreach (var ch in value)
            if (char.IsControl(ch) || char.IsWhiteSpace(ch))
                return false;
        return true;
    }
}

internal static class RecoveryReintroductionIdentity
{
    internal static string ComputeTrust(RecoveryReintroductionTrustEvidence value)
    {
        var canonical = string.Join("\n", new[]
        {
            value.IdentityEvidenceIdentity,
            value.IdentityCurrent ? "1" : "0",
            value.IdentityTrusted ? "1" : "0",
            value.ConfigurationEvidenceIdentity,
            value.ConfigurationCurrent ? "1" : "0",
            value.ConfigurationTrusted ? "1" : "0",
            value.DependencyEvidenceIdentity,
            value.DependencyCurrent ? "1" : "0",
            value.DependencyTrusted ? "1" : "0",
            value.SecurityEvidenceIdentity,
            value.SecurityCurrent ? "1" : "0",
            value.SecurityTrusted ? "1" : "0"
        });
        return Digest("recovery-reintroduction-trust-v1\n" + canonical);
    }

    internal static string ComputeObservation(RecoveryObservationEvidence value)
    {
        var canonical = string.Join("\n", new[]
        {
            ((int)value.Mode).ToString(CultureInfo.InvariantCulture),
            value.EvidenceIdentity,
            value.Current ? "1" : "0",
            value.Trusted ? "1" : "0",
            value.Satisfactory ? "1" : "0",
            value.ExitAuthorized ? "1" : "0",
            value.ExitEvidenceIdentity ?? string.Empty,
            value.ObservedAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)
        });
        return Digest("recovery-observation-v1\n" + canonical);
    }

    internal static string ComputeDecision(RecoveryReintroductionDecision value)
    {
        var canonical = string.Join("\n", new[]
        {
            ((int)value.Classification).ToString(CultureInfo.InvariantCulture),
            value.Reason,
            value.RecoveryCaseIdentity,
            value.SubjectIdentity,
            value.ProtectiveRestrictionReleaseFactIdentity,
            value.LifecycleTransitionRequestIdentity,
            value.LifecycleTransitionIdentity,
            value.LifecycleResultingState,
            value.NewAuthorityRequestIdentity,
            value.NewAuthorityDecisionIdentity,
            value.NewAuthorityEffectiveScope,
            value.ReintroductionTrustEvidenceIdentity,
            value.ObservationEvidenceIdentity,
            value.ResidualRiskEvidenceIdentity,
            value.DataLossDeclarationIdentity,
            value.CapabilityLossDeclarationIdentity,
            value.ApprovalEvidenceIdentity,
            value.FollowUpObligationsIdentity,
            value.ObservationRequired ? "1" : "0",
            value.ObservationExitAuthorized ? "1" : "0",
            value.EvaluatedAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)
        });
        return Digest("recovery-reintroduction-decision-v1\n" + canonical);
    }

    private static string Digest(string value) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
}