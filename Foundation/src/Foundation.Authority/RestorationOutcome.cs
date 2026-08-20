using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Foundation.Recovery;

public enum RestorationOutcomeKind
{
    Requested = 1,
    Attempted = 2,
    Completed = 3,
    Failed = 4,
    Partial = 5
}

public enum RestorationLossDeclaration
{
    None = 0,
    DataLoss = 1,
    CapabilityLoss = 2,
    DataAndCapabilityLoss = 3,
    Unknown = 4
}

public static class RestorationOutcomeReason
{
    public const string Pass = "PASS";
    public const string InvalidOutcome = "INVALID_RESTORATION_OUTCOME";
    public const string InvalidCaseBinding = "INVALID_RECOVERY_CASE_BINDING";
    public const string InvalidPlanBinding = "INVALID_AUTHORIZED_RECOVERY_PLAN_BINDING";
    public const string InvalidAttemptBinding = "INVALID_AUTHORIZED_RECOVERY_ATTEMPT_BINDING";
    public const string InvalidRepairActor = "INVALID_REPAIR_ACTOR_IDENTITY";
    public const string RepairActorMismatch = "REPAIR_ACTOR_MISMATCH";
    public const string MissingRestorationEvidence = "MISSING_RESTORATION_EVIDENCE";
    public const string MissingChangedStateEvidence = "MISSING_CHANGED_STATE_EVIDENCE";
    public const string MissingFailureEvidence = "MISSING_FAILURE_EVIDENCE";
    public const string PartialReportedAsComplete = "PARTIAL_RESTORATION_CANNOT_BE_REPORTED_COMPLETE";
    public const string InvalidRollbackEvidence = "INVALID_ROLLBACK_EVIDENCE";
    public const string MissingLossDeclaration = "MISSING_LOSS_DECLARATION";
    public const string EvidenceNotPreserved = "RESTORATION_EVIDENCE_NOT_PRESERVED";
}

public readonly record struct RestorationOutcomeValidation(bool Success, string Reason)
{
    public static RestorationOutcomeValidation Passed => new(true, RestorationOutcomeReason.Pass);
    public static RestorationOutcomeValidation Failed(string reason) => new(false, reason);
}

public sealed record RestorationOutcomeRecord(
    string RestorationActionId,
    string RecoveryCaseIdentity,
    string RecoveryPlanIdentity,
    string PlanAuthorizationDecisionIdentity,
    string AttemptAuthorizationDecisionIdentity,
    int AttemptNumber,
    string RepairActorIdentity,
    RestorationOutcomeKind Outcome,
    string RestorationActionEvidenceIdentity,
    string ChangedArtifactEvidenceIdentity,
    string ChangedConfigurationEvidenceIdentity,
    string ChangedStateEvidenceIdentity,
    string ChangedDependencyEvidenceIdentity,
    bool RollbackApplicable,
    string RollbackActionEvidenceIdentity,
    string RollbackResultEvidenceIdentity,
    RestorationLossDeclaration LossDeclaration,
    string LossDeclarationEvidenceIdentity,
    bool EvidencePreserved,
    DateTimeOffset ReportedAt)
{
    public string Identity => RestorationOutcomeIdentity.Digest("restoration-outcome-v1", string.Join("\n", new[]
    {
        RestorationActionId,
        RecoveryCaseIdentity,
        RecoveryPlanIdentity,
        PlanAuthorizationDecisionIdentity,
        AttemptAuthorizationDecisionIdentity,
        AttemptNumber.ToString(CultureInfo.InvariantCulture),
        RepairActorIdentity,
        ((int)Outcome).ToString(CultureInfo.InvariantCulture),
        RestorationActionEvidenceIdentity,
        ChangedArtifactEvidenceIdentity,
        ChangedConfigurationEvidenceIdentity,
        ChangedStateEvidenceIdentity,
        ChangedDependencyEvidenceIdentity,
        RollbackApplicable ? "1" : "0",
        RollbackActionEvidenceIdentity,
        RollbackResultEvidenceIdentity,
        ((int)LossDeclaration).ToString(CultureInfo.InvariantCulture),
        LossDeclarationEvidenceIdentity,
        EvidencePreserved ? "1" : "0",
        RestorationOutcomeIdentity.Time(ReportedAt)
    }));
}

public static class RestorationOutcomeValidator
{
    public static RestorationOutcomeValidation Validate(
        RecoveryCase recoveryCase,
        RecoveryPlan recoveryPlan,
        RecoveryPlanAuthorizationDecision planAuthorization,
        RecoveryAttemptAuthorizationDecision attemptAuthorization,
        RestorationOutcomeRecord outcome)
    {
        if (!RecoveryPrimitiveValidator.ValidatePlan(recoveryCase, recoveryPlan).Success)
            return RestorationOutcomeValidation.Failed(RestorationOutcomeReason.InvalidPlanBinding);

        if (!Same(outcome.RecoveryCaseIdentity, recoveryCase.Identity))
            return RestorationOutcomeValidation.Failed(RestorationOutcomeReason.InvalidCaseBinding);

        if (planAuthorization.Outcome != RecoveryAuthorizationOutcome.Allow ||
            !Same(planAuthorization.RecoveryCaseIdentity, recoveryCase.Identity) ||
            !Same(planAuthorization.RecoveryPlanIdentity, recoveryPlan.Identity) ||
            !Same(outcome.RecoveryPlanIdentity, recoveryPlan.Identity) ||
            !Same(outcome.PlanAuthorizationDecisionIdentity, planAuthorization.Identity))
        {
            return RestorationOutcomeValidation.Failed(RestorationOutcomeReason.InvalidPlanBinding);
        }

        if (attemptAuthorization.Outcome != RecoveryAuthorizationOutcome.Allow ||
            attemptAuthorization.Disposition != RecoveryAttemptDisposition.Authorized ||
            !Same(attemptAuthorization.RecoveryCaseIdentity, recoveryCase.Identity) ||
            !Same(attemptAuthorization.RecoveryPlanIdentity, recoveryPlan.Identity) ||
            !Same(outcome.AttemptAuthorizationDecisionIdentity, attemptAuthorization.Identity) ||
            outcome.AttemptNumber != attemptAuthorization.AttemptNumber)
        {
            return RestorationOutcomeValidation.Failed(RestorationOutcomeReason.InvalidAttemptBinding);
        }

        if (!Token(outcome.RestorationActionId) || !Enum.IsDefined(outcome.Outcome) || outcome.ReportedAt == default)
            return RestorationOutcomeValidation.Failed(RestorationOutcomeReason.InvalidOutcome);

        if (!Token(outcome.RepairActorIdentity))
            return RestorationOutcomeValidation.Failed(RestorationOutcomeReason.InvalidRepairActor);

        if (!Same(outcome.RepairActorIdentity, recoveryPlan.RepairActorIdentity))
            return RestorationOutcomeValidation.Failed(RestorationOutcomeReason.RepairActorMismatch);

        if (!Token(outcome.RestorationActionEvidenceIdentity))
            return RestorationOutcomeValidation.Failed(RestorationOutcomeReason.MissingRestorationEvidence);

        if (outcome.Outcome is RestorationOutcomeKind.Attempted or RestorationOutcomeKind.Completed or RestorationOutcomeKind.Partial)
        {
            if (!AnyEvidence(outcome.ChangedArtifactEvidenceIdentity,
                    outcome.ChangedConfigurationEvidenceIdentity,
                    outcome.ChangedStateEvidenceIdentity,
                    outcome.ChangedDependencyEvidenceIdentity))
            {
                return RestorationOutcomeValidation.Failed(RestorationOutcomeReason.MissingChangedStateEvidence);
            }
        }

        if (outcome.Outcome == RestorationOutcomeKind.Failed &&
            !AnyEvidence(outcome.ChangedArtifactEvidenceIdentity,
                outcome.ChangedConfigurationEvidenceIdentity,
                outcome.ChangedStateEvidenceIdentity,
                outcome.ChangedDependencyEvidenceIdentity,
                outcome.RollbackResultEvidenceIdentity))
        {
            return RestorationOutcomeValidation.Failed(RestorationOutcomeReason.MissingFailureEvidence);
        }

        if (outcome.Outcome == RestorationOutcomeKind.Completed && outcome.LossDeclaration == RestorationLossDeclaration.Unknown)
            return RestorationOutcomeValidation.Failed(RestorationOutcomeReason.PartialReportedAsComplete);

        if (!Enum.IsDefined(outcome.LossDeclaration) || !Token(outcome.LossDeclarationEvidenceIdentity))
            return RestorationOutcomeValidation.Failed(RestorationOutcomeReason.MissingLossDeclaration);

        if (outcome.RollbackApplicable)
        {
            if (!Token(outcome.RollbackActionEvidenceIdentity) || !Token(outcome.RollbackResultEvidenceIdentity))
                return RestorationOutcomeValidation.Failed(RestorationOutcomeReason.InvalidRollbackEvidence);
        }
        else if (Token(outcome.RollbackActionEvidenceIdentity) || Token(outcome.RollbackResultEvidenceIdentity))
        {
            return RestorationOutcomeValidation.Failed(RestorationOutcomeReason.InvalidRollbackEvidence);
        }

        if (!outcome.EvidencePreserved)
            return RestorationOutcomeValidation.Failed(RestorationOutcomeReason.EvidenceNotPreserved);

        return RestorationOutcomeValidation.Passed;
    }

    private static bool AnyEvidence(params string[] values)
    {
        foreach (var value in values)
        {
            if (Token(value))
                return true;
        }
        return false;
    }

    private static bool Same(string left, string right) =>
        string.Equals(left, right, StringComparison.Ordinal);

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

internal static class RestorationOutcomeIdentity
{
    internal static string Time(DateTimeOffset value) =>
        value.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture);

    internal static string Digest(string domain, string canonical)
    {
        var bytes = Encoding.UTF8.GetBytes(domain + "\n" + canonical);
        return Convert.ToHexString(SHA256.HashData(bytes));
    }
}
