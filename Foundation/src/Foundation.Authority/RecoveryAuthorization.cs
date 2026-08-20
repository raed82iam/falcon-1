using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Foundation.Recovery;

public enum RecoveryAuthorizationOutcome
{
    Deny = 0,
    Allow = 1
}

public enum RecoveryAttemptDisposition
{
    Authorized = 1,
    Aborted = 2,
    Escalated = 3
}

public static class RecoveryAuthorizationReason
{
    public const string Pass = "PASS";
    public const string InvalidRequest = "INVALID_RECOVERY_AUTHORIZATION_REQUEST";
    public const string InvalidDecision = "INVALID_RECOVERY_AUTHORIZATION_DECISION";
    public const string InvalidAuthority = "INVALID_RECOVERY_AUTHORITY_BINDING";
    public const string InvalidCaseBinding = "INVALID_RECOVERY_CASE_BINDING";
    public const string InvalidPlanBinding = "INVALID_RECOVERY_PLAN_BINDING";
    public const string InitiationNotAllowed = "RECOVERY_INITIATION_NOT_ALLOWED";
    public const string PlanNotAllowed = "RECOVERY_PLAN_NOT_ALLOWED";
    public const string PlanMutation = "AUTHORIZED_PLAN_IDENTITY_MISMATCH";
    public const string InvalidRestrictionBinding = "INVALID_CURRENT_RESTRICTION_BINDING";
    public const string InvalidHandoffBinding = "INVALID_CURRENT_HANDOFF_BINDING";
    public const string InvalidAttemptLedger = "INVALID_RECOVERY_ATTEMPT_LEDGER";
    public const string AttemptSequenceMismatch = "RECOVERY_ATTEMPT_SEQUENCE_MISMATCH";
    public const string AttemptBudgetExceeded = "RECOVERY_ATTEMPT_BUDGET_EXCEEDED";
    public const string AttemptBudgetResetForbidden = "RECOVERY_ATTEMPT_BUDGET_CANNOT_RESET_BY_PLAN_VERSION_CHANGE";
    public const string InvalidCeilingAdjustment = "INVALID_RECOVERY_ATTEMPT_CEILING_ADJUSTMENT";
}

public readonly record struct RecoveryAuthorizationValidation(bool Success, string Reason)
{
    public static RecoveryAuthorizationValidation Passed => new(true, RecoveryAuthorizationReason.Pass);
    public static RecoveryAuthorizationValidation Failed(string reason) => new(false, reason);
}

public sealed record RecoveryInitiationRequest(
    string RequestId,
    string RecoveryCaseId,
    string RecoveryCaseIdentity,
    string ActorIdentity,
    string ActionIdentity,
    string ResourceIdentity,
    string PurposeIdentity,
    string JurisdictionIdentity,
    string CorrelationIdentity,
    string CausationIdentity,
    DateTimeOffset RequestedAt,
    DateTimeOffset ExpiresAt)
{
    public string Identity => RecoveryAuthorizationIdentity.Digest("recovery-initiation-request-v1", string.Join("\n", new[]
    {
        RequestId, RecoveryCaseId, RecoveryCaseIdentity, ActorIdentity, ActionIdentity, ResourceIdentity,
        PurposeIdentity, JurisdictionIdentity, CorrelationIdentity, CausationIdentity,
        RecoveryAuthorizationIdentity.Time(RequestedAt), RecoveryAuthorizationIdentity.Time(ExpiresAt)
    }));
}

public sealed record RecoveryInitiationDecision(
    string DecisionId,
    string RequestIdentity,
    string RecoveryCaseIdentity,
    RecoveryAuthorizationOutcome Outcome,
    string AuthorityDecisionIdentity,
    string AuthorityBasisIdentity,
    string ConditionsIdentity,
    string ReasonIdentity,
    DateTimeOffset DecidedAt,
    DateTimeOffset ExpiresAt)
{
    public string Identity => RecoveryAuthorizationIdentity.Digest("recovery-initiation-decision-v1", string.Join("\n", new[]
    {
        DecisionId, RequestIdentity, RecoveryCaseIdentity, ((int)Outcome).ToString(CultureInfo.InvariantCulture),
        AuthorityDecisionIdentity, AuthorityBasisIdentity, ConditionsIdentity, ReasonIdentity,
        RecoveryAuthorizationIdentity.Time(DecidedAt), RecoveryAuthorizationIdentity.Time(ExpiresAt)
    }));
}

public sealed record RecoveryPlanAuthorizationRequest(
    string RequestId,
    string RecoveryCaseId,
    string RecoveryCaseIdentity,
    string RecoveryPlanId,
    int RecoveryPlanVersion,
    string RecoveryPlanIdentity,
    string InitiationDecisionIdentity,
    string ActorIdentity,
    DateTimeOffset RequestedAt)
{
    public string Identity => RecoveryAuthorizationIdentity.Digest("recovery-plan-authorization-request-v1", string.Join("\n", new[]
    {
        RequestId, RecoveryCaseId, RecoveryCaseIdentity, RecoveryPlanId,
        RecoveryPlanVersion.ToString(CultureInfo.InvariantCulture), RecoveryPlanIdentity,
        InitiationDecisionIdentity, ActorIdentity, RecoveryAuthorizationIdentity.Time(RequestedAt)
    }));
}

public sealed record RecoveryPlanAuthorizationDecision(
    string DecisionId,
    string RequestIdentity,
    string RecoveryCaseIdentity,
    string RecoveryPlanIdentity,
    RecoveryAuthorizationOutcome Outcome,
    string ActorIdentity,
    string AuthorityDecisionIdentity,
    string AuthorityBasisIdentity,
    string ConditionsIdentity,
    string ReasonIdentity,
    DateTimeOffset DecidedAt)
{
    public string Identity => RecoveryAuthorizationIdentity.Digest("recovery-plan-authorization-decision-v1", string.Join("\n", new[]
    {
        DecisionId, RequestIdentity, RecoveryCaseIdentity, RecoveryPlanIdentity,
        ((int)Outcome).ToString(CultureInfo.InvariantCulture), ActorIdentity, AuthorityDecisionIdentity,
        AuthorityBasisIdentity, ConditionsIdentity, ReasonIdentity, RecoveryAuthorizationIdentity.Time(DecidedAt)
    }));
}

public sealed record RecoveryAttemptLedger(
    string RecoveryCaseId,
    string RecoveryCaseIdentity,
    int CumulativeAttempts,
    int AuthorizedCaseCeiling,
    string CeilingAuthorityDecisionIdentity)
{
    public string Identity => RecoveryAuthorizationIdentity.Digest("recovery-attempt-ledger-v1", string.Join("\n", new[]
    {
        RecoveryCaseId, RecoveryCaseIdentity,
        CumulativeAttempts.ToString(CultureInfo.InvariantCulture),
        AuthorizedCaseCeiling.ToString(CultureInfo.InvariantCulture),
        CeilingAuthorityDecisionIdentity
    }));
}

public sealed record RecoveryAttemptCeilingAdjustmentDecision(
    string DecisionId,
    string RecoveryCaseIdentity,
    string PriorLedgerIdentity,
    int NewAuthorizedCaseCeiling,
    RecoveryAuthorizationOutcome Outcome,
    string ActorIdentity,
    string AuthorityDecisionIdentity,
    string AuthorityBasisIdentity,
    string ReasonIdentity,
    DateTimeOffset DecidedAt)
{
    public string Identity => RecoveryAuthorizationIdentity.Digest("recovery-attempt-ceiling-adjustment-v1", string.Join("\n", new[]
    {
        DecisionId, RecoveryCaseIdentity, PriorLedgerIdentity,
        NewAuthorizedCaseCeiling.ToString(CultureInfo.InvariantCulture),
        ((int)Outcome).ToString(CultureInfo.InvariantCulture), ActorIdentity,
        AuthorityDecisionIdentity, AuthorityBasisIdentity, ReasonIdentity,
        RecoveryAuthorizationIdentity.Time(DecidedAt)
    }));
}

public sealed record RecoveryAttemptAuthorizationRequest(
    string RequestId,
    string RecoveryCaseIdentity,
    string RecoveryPlanIdentity,
    string PlanAuthorizationDecisionIdentity,
    string AttemptLedgerIdentity,
    int RequestedAttemptNumber,
    string CurrentControllingRestrictionId,
    string CurrentControllingRestrictionIntegrityEvidence,
    string CurrentRecoveryHandoffIdentity,
    DateTimeOffset RequestedAt)
{
    public string Identity => RecoveryAuthorizationIdentity.Digest("recovery-attempt-authorization-request-v1", string.Join("\n", new[]
    {
        RequestId, RecoveryCaseIdentity, RecoveryPlanIdentity, PlanAuthorizationDecisionIdentity,
        AttemptLedgerIdentity, RequestedAttemptNumber.ToString(CultureInfo.InvariantCulture),
        CurrentControllingRestrictionId, CurrentControllingRestrictionIntegrityEvidence,
        CurrentRecoveryHandoffIdentity, RecoveryAuthorizationIdentity.Time(RequestedAt)
    }));
}

public sealed record RecoveryAttemptAuthorizationDecision(
    string DecisionId,
    string RequestIdentity,
    string RecoveryCaseIdentity,
    string RecoveryPlanIdentity,
    int AttemptNumber,
    RecoveryAuthorizationOutcome Outcome,
    RecoveryAttemptDisposition Disposition,
    string AuthorityDecisionIdentity,
    string AuthorityBasisIdentity,
    string ReasonIdentity,
    DateTimeOffset DecidedAt)
{
    public string Identity => RecoveryAuthorizationIdentity.Digest("recovery-attempt-authorization-decision-v1", string.Join("\n", new[]
    {
        DecisionId, RequestIdentity, RecoveryCaseIdentity, RecoveryPlanIdentity,
        AttemptNumber.ToString(CultureInfo.InvariantCulture),
        ((int)Outcome).ToString(CultureInfo.InvariantCulture),
        ((int)Disposition).ToString(CultureInfo.InvariantCulture), AuthorityDecisionIdentity,
        AuthorityBasisIdentity, ReasonIdentity, RecoveryAuthorizationIdentity.Time(DecidedAt)
    }));
}

public static class RecoveryAuthorizationValidator
{
    public static RecoveryAuthorizationValidation ValidateInitiation(
        RecoveryCase recoveryCase,
        RecoveryInitiationRequest request,
        RecoveryInitiationDecision decision,
        DateTimeOffset evaluationTime)
    {
        if (!RecoveryPrimitiveValidator.ValidateCase(recoveryCase).Success)
            return RecoveryAuthorizationValidation.Failed(RecoveryAuthorizationReason.InvalidCaseBinding);
        if (!Token(request.RequestId) || !Token(request.ActorIdentity) || !Token(request.ActionIdentity) ||
            !Token(request.ResourceIdentity) || !Token(request.PurposeIdentity) || !Token(request.JurisdictionIdentity) ||
            !Token(request.CorrelationIdentity) || !Token(request.CausationIdentity) || request.RequestedAt == default ||
            request.ExpiresAt <= request.RequestedAt || evaluationTime > request.ExpiresAt)
            return RecoveryAuthorizationValidation.Failed(RecoveryAuthorizationReason.InvalidRequest);
        if (!Same(request.RecoveryCaseId, recoveryCase.RecoveryCaseId) || !Same(request.RecoveryCaseIdentity, recoveryCase.Identity))
            return RecoveryAuthorizationValidation.Failed(RecoveryAuthorizationReason.InvalidCaseBinding);
        if (!Token(decision.DecisionId) || !Same(decision.RequestIdentity, request.Identity) ||
            !Same(decision.RecoveryCaseIdentity, recoveryCase.Identity) || !Enum.IsDefined(decision.Outcome) ||
            decision.DecidedAt == default || decision.ExpiresAt <= decision.DecidedAt || evaluationTime > decision.ExpiresAt)
            return RecoveryAuthorizationValidation.Failed(RecoveryAuthorizationReason.InvalidDecision);
        if (!Token(decision.AuthorityDecisionIdentity) || !Token(decision.AuthorityBasisIdentity) ||
            !Token(decision.ConditionsIdentity) || !Token(decision.ReasonIdentity))
            return RecoveryAuthorizationValidation.Failed(RecoveryAuthorizationReason.InvalidAuthority);
        return RecoveryAuthorizationValidation.Passed;
    }

    public static RecoveryAuthorizationValidation ValidatePlanAuthorization(
        RecoveryCase recoveryCase,
        RecoveryPlan plan,
        RecoveryInitiationDecision initiationDecision,
        RecoveryPlanAuthorizationRequest request,
        RecoveryPlanAuthorizationDecision decision)
    {
        if (!RecoveryPrimitiveValidator.ValidatePlan(recoveryCase, plan).Success)
            return RecoveryAuthorizationValidation.Failed(RecoveryAuthorizationReason.InvalidPlanBinding);
        if (initiationDecision.Outcome != RecoveryAuthorizationOutcome.Allow)
            return RecoveryAuthorizationValidation.Failed(RecoveryAuthorizationReason.InitiationNotAllowed);
        if (!Token(request.RequestId) || !Same(request.RecoveryCaseId, recoveryCase.RecoveryCaseId) ||
            !Same(request.RecoveryCaseIdentity, recoveryCase.Identity) || !Same(request.RecoveryPlanId, plan.RecoveryPlanId) ||
            request.RecoveryPlanVersion != plan.Version || !Same(request.RecoveryPlanIdentity, plan.Identity) ||
            !Same(request.InitiationDecisionIdentity, initiationDecision.Identity) || !Token(request.ActorIdentity) ||
            request.RequestedAt == default)
            return RecoveryAuthorizationValidation.Failed(RecoveryAuthorizationReason.InvalidPlanBinding);
        if (!Token(decision.DecisionId) || !Same(decision.RequestIdentity, request.Identity) ||
            !Same(decision.RecoveryCaseIdentity, recoveryCase.Identity) || !Same(decision.RecoveryPlanIdentity, plan.Identity) ||
            !Enum.IsDefined(decision.Outcome) || !Token(decision.ActorIdentity) || !Token(decision.AuthorityDecisionIdentity) ||
            !Token(decision.AuthorityBasisIdentity) || !Token(decision.ConditionsIdentity) || !Token(decision.ReasonIdentity) ||
            decision.DecidedAt == default)
            return RecoveryAuthorizationValidation.Failed(RecoveryAuthorizationReason.InvalidDecision);
        return RecoveryAuthorizationValidation.Passed;
    }

    public static RecoveryAuthorizationValidation ValidateAttempt(
        RecoveryCase recoveryCase,
        RecoveryPlan plan,
        RecoveryPlanAuthorizationDecision planDecision,
        RecoveryAttemptLedger ledger,
        RecoveryAttemptAuthorizationRequest request,
        RecoveryAttemptAuthorizationDecision decision)
    {
        if (planDecision.Outcome != RecoveryAuthorizationOutcome.Allow)
            return RecoveryAuthorizationValidation.Failed(RecoveryAuthorizationReason.PlanNotAllowed);
        if (!Same(planDecision.RecoveryPlanIdentity, plan.Identity))
            return RecoveryAuthorizationValidation.Failed(RecoveryAuthorizationReason.PlanMutation);
        if (!ValidateLedger(recoveryCase, ledger).Success)
            return RecoveryAuthorizationValidation.Failed(RecoveryAuthorizationReason.InvalidAttemptLedger);
        if (!Same(request.RecoveryCaseIdentity, recoveryCase.Identity) || !Same(request.RecoveryPlanIdentity, plan.Identity) ||
            !Same(request.PlanAuthorizationDecisionIdentity, planDecision.Identity) || !Same(request.AttemptLedgerIdentity, ledger.Identity) ||
            !Same(request.CurrentControllingRestrictionId, recoveryCase.ControllingRestrictionId) ||
            !Same(request.CurrentControllingRestrictionIntegrityEvidence, recoveryCase.ControllingRestrictionIntegrityEvidence))
            return RecoveryAuthorizationValidation.Failed(RecoveryAuthorizationReason.InvalidRestrictionBinding);
        if (!Same(request.CurrentRecoveryHandoffIdentity, recoveryCase.Stage8RecoveryHandoffIdentity))
            return RecoveryAuthorizationValidation.Failed(RecoveryAuthorizationReason.InvalidHandoffBinding);
        var expectedAttempt = checked(ledger.CumulativeAttempts + 1);
        if (request.RequestedAttemptNumber != expectedAttempt)
            return RecoveryAuthorizationValidation.Failed(RecoveryAuthorizationReason.AttemptSequenceMismatch);
        var effectiveCeiling = Math.Min(ledger.AuthorizedCaseCeiling, plan.MaximumAuthorizedAttempts);
        if (request.RequestedAttemptNumber > effectiveCeiling)
            return RecoveryAuthorizationValidation.Failed(RecoveryAuthorizationReason.AttemptBudgetExceeded);
        if (!Token(decision.DecisionId) || !Same(decision.RequestIdentity, request.Identity) ||
            !Same(decision.RecoveryCaseIdentity, recoveryCase.Identity) || !Same(decision.RecoveryPlanIdentity, plan.Identity) ||
            decision.AttemptNumber != request.RequestedAttemptNumber || !Enum.IsDefined(decision.Outcome) ||
            !Enum.IsDefined(decision.Disposition) || !Token(decision.AuthorityDecisionIdentity) ||
            !Token(decision.AuthorityBasisIdentity) || !Token(decision.ReasonIdentity) || decision.DecidedAt == default)
            return RecoveryAuthorizationValidation.Failed(RecoveryAuthorizationReason.InvalidDecision);
        if (decision.Outcome == RecoveryAuthorizationOutcome.Allow && decision.Disposition != RecoveryAttemptDisposition.Authorized)
            return RecoveryAuthorizationValidation.Failed(RecoveryAuthorizationReason.InvalidDecision);
        if (decision.Outcome == RecoveryAuthorizationOutcome.Deny && decision.Disposition == RecoveryAttemptDisposition.Authorized)
            return RecoveryAuthorizationValidation.Failed(RecoveryAuthorizationReason.InvalidDecision);
        return RecoveryAuthorizationValidation.Passed;
    }

    public static RecoveryAuthorizationValidation ValidateLedger(RecoveryCase recoveryCase, RecoveryAttemptLedger ledger)
    {
        if (!Same(ledger.RecoveryCaseId, recoveryCase.RecoveryCaseId) || !Same(ledger.RecoveryCaseIdentity, recoveryCase.Identity) ||
            ledger.CumulativeAttempts < 0 || ledger.AuthorizedCaseCeiling <= 0 || ledger.CumulativeAttempts > ledger.AuthorizedCaseCeiling ||
            !Token(ledger.CeilingAuthorityDecisionIdentity))
            return RecoveryAuthorizationValidation.Failed(RecoveryAuthorizationReason.InvalidAttemptLedger);
        return RecoveryAuthorizationValidation.Passed;
    }

    public static RecoveryAuthorizationValidation ValidatePlanVersionContinuity(
        RecoveryAttemptLedger priorLedger,
        RecoveryAttemptLedger proposedLedger)
    {
        if (!Same(priorLedger.RecoveryCaseId, proposedLedger.RecoveryCaseId) ||
            !Same(priorLedger.RecoveryCaseIdentity, proposedLedger.RecoveryCaseIdentity) ||
            proposedLedger.CumulativeAttempts < priorLedger.CumulativeAttempts ||
            proposedLedger.AuthorizedCaseCeiling > priorLedger.AuthorizedCaseCeiling)
            return RecoveryAuthorizationValidation.Failed(RecoveryAuthorizationReason.AttemptBudgetResetForbidden);
        return RecoveryAuthorizationValidation.Passed;
    }

    public static RecoveryAuthorizationValidation ValidateCeilingAdjustment(
        RecoveryAttemptLedger priorLedger,
        RecoveryAttemptCeilingAdjustmentDecision adjustment,
        RecoveryAttemptLedger resultingLedger)
    {
        if (adjustment.Outcome != RecoveryAuthorizationOutcome.Allow || !Token(adjustment.DecisionId) ||
            !Same(adjustment.RecoveryCaseIdentity, priorLedger.RecoveryCaseIdentity) ||
            !Same(adjustment.PriorLedgerIdentity, priorLedger.Identity) ||
            adjustment.NewAuthorizedCaseCeiling <= priorLedger.AuthorizedCaseCeiling ||
            !Token(adjustment.ActorIdentity) || !Token(adjustment.AuthorityDecisionIdentity) ||
            !Token(adjustment.AuthorityBasisIdentity) || !Token(adjustment.ReasonIdentity) || adjustment.DecidedAt == default)
            return RecoveryAuthorizationValidation.Failed(RecoveryAuthorizationReason.InvalidCeilingAdjustment);
        if (!Same(resultingLedger.RecoveryCaseId, priorLedger.RecoveryCaseId) ||
            !Same(resultingLedger.RecoveryCaseIdentity, priorLedger.RecoveryCaseIdentity) ||
            resultingLedger.CumulativeAttempts != priorLedger.CumulativeAttempts ||
            resultingLedger.AuthorizedCaseCeiling != adjustment.NewAuthorizedCaseCeiling ||
            !Same(resultingLedger.CeilingAuthorityDecisionIdentity, adjustment.AuthorityDecisionIdentity))
            return RecoveryAuthorizationValidation.Failed(RecoveryAuthorizationReason.InvalidCeilingAdjustment);
        return RecoveryAuthorizationValidation.Passed;
    }

    private static bool Same(string left, string right) => string.Equals(left, right, StringComparison.Ordinal);

    private static bool Token(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) || !string.Equals(value, value.Trim(), StringComparison.Ordinal))
            return false;
        foreach (var ch in value)
            if (char.IsControl(ch) || char.IsWhiteSpace(ch)) return false;
        return true;
    }
}

internal static class RecoveryAuthorizationIdentity
{
    internal static string Time(DateTimeOffset value) => value.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture);

    internal static string Digest(string domain, string canonical)
    {
        var bytes = Encoding.UTF8.GetBytes(domain + "\n" + canonical);
        return Convert.ToHexString(SHA256.HashData(bytes));
    }
}
