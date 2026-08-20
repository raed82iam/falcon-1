using System;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Foundation.Guardian;

public enum GuardianSafeStateOperation
{
    ReportHealth = 1,
    PublishEvidence = 2,
    ComplyWithProtectiveControl = 3
}

public sealed record GuardianPlatformSafeState(
    string SafeStateId,
    string SourceDecisionId,
    string SourceDecisionIdentity,
    string SourceRestrictionId,
    string SourceRestrictionIdentity,
    string TargetId,
    GuardianScopeKind ScopeKind,
    string ScopeId,
    string TriggerEvidence,
    string AuthorityReference,
    string PolicyReference,
    DateTimeOffset EnteredAt)
{
    public string Identity => GuardianSafeStateIdentity.Compute(this);
}

public sealed record GuardianSafeStateEvaluation(
    bool Success,
    string Reason,
    bool AppliesToRequestedScope,
    bool OperationWithinSafeStateCeiling,
    bool IndependentAuthorityStillRequired,
    bool AuthorityGranted,
    bool ContainmentRemainsRequired);

public static class GuardianPlatformSafeStateRuntime
{
    public const string CanonicalAllowedActions =
        "REPORT_HEALTH|PUBLISH_EVIDENCE|COMPLY_WITH_PROTECTIVE_CONTROL";

    public static GuardianPlatformSafeState Create(
        GuardianProtectiveDecision decision,
        GuardianProtectiveRestriction restriction,
        string safeStateId,
        DateTimeOffset enteredAt)
    {
        var validation = ValidateSource(decision, restriction);
        if (!validation.Success)
            throw new ArgumentException("Invalid Safe-State source: " + validation.Reason, nameof(restriction));
        if (!CanonicalToken(safeStateId))
            throw new ArgumentException("Safe-State ID must be a canonical token.", nameof(safeStateId));
        if (enteredAt == default || enteredAt < restriction.EffectiveAt)
            throw new ArgumentException("Safe-State entry time must be at or after restriction effectiveness.", nameof(enteredAt));

        return new GuardianPlatformSafeState(
            safeStateId,
            decision.DecisionId,
            decision.Identity,
            restriction.RestrictionId,
            restriction.Identity,
            restriction.TargetId,
            restriction.ScopeKind,
            restriction.ScopeId,
            restriction.EvidenceReference,
            restriction.AuthorityReference,
            restriction.PolicyReference,
            enteredAt);
    }

    public static GuardianValidationOutcome Validate(
        GuardianPlatformSafeState? safeState,
        GuardianProtectiveDecision? decision,
        GuardianProtectiveRestriction? restriction)
    {
        if (safeState is null)
            return GuardianValidationOutcome.Fail("MISSING_SAFE_STATE");
        if (decision is null)
            return GuardianValidationOutcome.Fail("MISSING_SAFE_STATE_SOURCE_DECISION");
        if (restriction is null)
            return GuardianValidationOutcome.Fail("MISSING_SAFE_STATE_SOURCE_RESTRICTION");

        var sourceValidation = ValidateSource(decision, restriction);
        if (!sourceValidation.Success)
            return sourceValidation;

        if (!CanonicalToken(safeState.SafeStateId))
            return GuardianValidationOutcome.Fail("INVALID_SAFE_STATE_ID");
        if (!CanonicalToken(safeState.SourceDecisionId) ||
            !CanonicalToken(safeState.SourceDecisionIdentity) ||
            !CanonicalToken(safeState.SourceRestrictionId) ||
            !CanonicalToken(safeState.SourceRestrictionIdentity) ||
            !CanonicalToken(safeState.TargetId) ||
            !CanonicalToken(safeState.ScopeId) ||
            !CanonicalToken(safeState.TriggerEvidence) ||
            !CanonicalToken(safeState.AuthorityReference) ||
            !CanonicalToken(safeState.PolicyReference))
        {
            return GuardianValidationOutcome.Fail("INVALID_SAFE_STATE_IDENTITY_SURFACE");
        }

        if (!Enum.IsDefined(safeState.ScopeKind))
            return GuardianValidationOutcome.Fail("INVALID_SAFE_STATE_SCOPE_KIND");
        if (safeState.EnteredAt == default || safeState.EnteredAt < restriction.EffectiveAt)
            return GuardianValidationOutcome.Fail("INVALID_SAFE_STATE_ENTRY_TIME");

        if (!string.Equals(safeState.SourceDecisionId, decision.DecisionId, StringComparison.Ordinal) ||
            !string.Equals(safeState.SourceDecisionIdentity, decision.Identity, StringComparison.Ordinal))
        {
            return GuardianValidationOutcome.Fail("SAFE_STATE_DECISION_BINDING_MISMATCH");
        }

        if (!string.Equals(safeState.SourceRestrictionId, restriction.RestrictionId, StringComparison.Ordinal) ||
            !string.Equals(safeState.SourceRestrictionIdentity, restriction.Identity, StringComparison.Ordinal))
        {
            return GuardianValidationOutcome.Fail("SAFE_STATE_RESTRICTION_BINDING_MISMATCH");
        }

        if (!string.Equals(safeState.TargetId, restriction.TargetId, StringComparison.Ordinal) ||
            safeState.ScopeKind != restriction.ScopeKind ||
            !string.Equals(safeState.ScopeId, restriction.ScopeId, StringComparison.Ordinal))
        {
            return GuardianValidationOutcome.Fail("SAFE_STATE_SCOPE_BINDING_MISMATCH");
        }

        if (!string.Equals(safeState.TriggerEvidence, restriction.EvidenceReference, StringComparison.Ordinal) ||
            !string.Equals(safeState.AuthorityReference, restriction.AuthorityReference, StringComparison.Ordinal) ||
            !string.Equals(safeState.PolicyReference, restriction.PolicyReference, StringComparison.Ordinal))
        {
            return GuardianValidationOutcome.Fail("SAFE_STATE_GOVERNANCE_BINDING_MISMATCH");
        }

        return GuardianValidationOutcome.Pass();
    }

    public static GuardianSafeStateEvaluation EvaluateOperation(
        GuardianPlatformSafeState? safeState,
        GuardianProtectiveDecision? decision,
        GuardianProtectiveRestriction? restriction,
        GuardianSafeStateOperation operation,
        string requestedTargetId,
        GuardianScopeKind requestedScopeKind,
        string requestedScopeId,
        DateTimeOffset evaluationTime)
    {
        var validation = Validate(safeState, decision, restriction);
        if (!validation.Success)
            return FailClosed(validation.Reason);
        if (!Enum.IsDefined(operation))
            return FailClosed("UNKNOWN_SAFE_STATE_OPERATION");
        if (!CanonicalToken(requestedTargetId) || !CanonicalToken(requestedScopeId) || !Enum.IsDefined(requestedScopeKind))
            return FailClosed("INVALID_SAFE_STATE_REQUEST_SCOPE");
        if (evaluationTime == default || evaluationTime < safeState!.EnteredAt)
            return FailClosed("INVALID_SAFE_STATE_EVALUATION_TIME");

        var restrictionEvaluation = GuardianProtectiveRestrictionRuntime.EvaluateAt(
            restriction,
            decision,
            evaluationTime);

        if (!restrictionEvaluation.Success || !restrictionEvaluation.RemainsEnforced)
            return FailClosed("SAFE_STATE_SOURCE_RESTRICTION_NOT_ENFORCED:" + restrictionEvaluation.Reason);

        var applies = AppliesToScope(
            safeState,
            requestedTargetId,
            requestedScopeKind,
            requestedScopeId);

        if (!applies)
        {
            return new GuardianSafeStateEvaluation(
                true,
                "SAFE_STATE_NOT_APPLICABLE_TO_INDEPENDENT_SCOPE",
                false,
                false,
                true,
                false,
                false);
        }

        var withinCeiling = IsCanonicalAllowedOperation(operation);

        return new GuardianSafeStateEvaluation(
            true,
            withinCeiling ? "SAFE_STATE_OPERATION_WITHIN_CEILING" : "SAFE_STATE_OPERATION_DENIED",
            true,
            withinCeiling,
            true,
            false,
            true);
    }

    public static bool IsCanonicalAllowedOperation(GuardianSafeStateOperation operation)
        => operation is GuardianSafeStateOperation.ReportHealth or
            GuardianSafeStateOperation.PublishEvidence or
            GuardianSafeStateOperation.ComplyWithProtectiveControl;

    public static string ToAuthorityActionToken(GuardianSafeStateOperation operation)
        => operation switch
        {
            GuardianSafeStateOperation.ReportHealth => "REPORT_HEALTH",
            GuardianSafeStateOperation.PublishEvidence => "PUBLISH_EVIDENCE",
            GuardianSafeStateOperation.ComplyWithProtectiveControl => "COMPLY_WITH_PROTECTIVE_CONTROL",
            _ => "DENY_UNKNOWN_SAFE_STATE_OPERATION"
        };

    private static GuardianValidationOutcome ValidateSource(
        GuardianProtectiveDecision decision,
        GuardianProtectiveRestriction restriction)
    {
        var validation = GuardianProtectiveRestrictionRuntime.Validate(restriction, decision);
        if (!validation.Success)
            return GuardianValidationOutcome.Fail("INVALID_SAFE_STATE_SOURCE:" + validation.Reason);

        if (decision.ProtectiveMode != GuardianProtectiveMode.Safe)
            return GuardianValidationOutcome.Fail("SAFE_STATE_REQUIRES_SAFE_PROTECTIVE_MODE");
        if (restriction.Severity != GuardianRestrictionSeverity.Critical)
            return GuardianValidationOutcome.Fail("SAFE_STATE_REQUIRES_CRITICAL_RESTRICTION");
        if (!restriction.PersistAcrossRestart)
            return GuardianValidationOutcome.Fail("SAFE_STATE_REQUIRES_RESTART_PERSISTENCE");
        if (!restriction.SubjectSelfReleaseForbidden)
            return GuardianValidationOutcome.Fail("SAFE_STATE_REQUIRES_NO_SELF_RELEASE");

        return GuardianValidationOutcome.Pass();
    }

    private static bool AppliesToScope(
        GuardianPlatformSafeState safeState,
        string requestedTargetId,
        GuardianScopeKind requestedScopeKind,
        string requestedScopeId)
    {
        if (safeState.ScopeKind == GuardianScopeKind.FalconWide)
            return true;

        return string.Equals(safeState.TargetId, requestedTargetId, StringComparison.Ordinal) &&
            safeState.ScopeKind == requestedScopeKind &&
            string.Equals(safeState.ScopeId, requestedScopeId, StringComparison.Ordinal);
    }

    private static GuardianSafeStateEvaluation FailClosed(string reason)
        => new(false, reason, true, false, true, false, true);

    private static bool CanonicalToken(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;
        if (!string.Equals(value, value.Trim(), StringComparison.Ordinal))
            return false;
        return value.All(ch => !char.IsControl(ch) && !char.IsWhiteSpace(ch));
    }
}

internal static class GuardianSafeStateIdentity
{
    internal static string Compute(GuardianPlatformSafeState safeState)
    {
        var canonical = string.Join("\n", new[]
        {
            safeState.SafeStateId,
            safeState.SourceDecisionId,
            safeState.SourceDecisionIdentity,
            safeState.SourceRestrictionId,
            safeState.SourceRestrictionIdentity,
            safeState.TargetId,
            ((int)safeState.ScopeKind).ToString(CultureInfo.InvariantCulture),
            safeState.ScopeId,
            safeState.TriggerEvidence,
            safeState.AuthorityReference,
            safeState.PolicyReference,
            safeState.EnteredAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture),
            GuardianPlatformSafeStateRuntime.CanonicalAllowedActions
        });

        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(canonical)));
    }
}
