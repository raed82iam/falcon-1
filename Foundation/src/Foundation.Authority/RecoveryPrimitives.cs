using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Foundation.Recovery;

public enum RecoveryCaseState
{
    InitiationPending = 1,
    AuthorizedForAssessment = 2,
    PlanAuthorizationPending = 3,
    PlanAuthorized = 4,
    RestorationInProgress = 5,
    RestorationReported = 6,
    ReconciliationPending = 7,
    ValidationPending = 8,
    ValidationFailed = 9,
    ReadyForReleaseDecision = 10,
    ReleaseDenied = 11,
    ReleaseAuthorized = 12,
    ReintroductionPending = 13,
    RecoveryGuardObservation = 14,
    RecoveredWithRestrictedAuthority = 15,
    RecoveryComplete = 16,
    Aborted = 17,
    Escalated = 18
}

public static class RecoveryPrimitiveReason
{
    public const string Pass = "PASS";
    public const string InvalidCaseId = "INVALID_RECOVERY_CASE_ID";
    public const string InvalidSubjectId = "INVALID_SUBJECT_ID";
    public const string InvalidGuardianIdentity = "INVALID_GUARDIAN_IDENTITY";
    public const string InvalidRestrictionId = "INVALID_RESTRICTION_ID";
    public const string InvalidRestrictionIntegrity = "INVALID_RESTRICTION_INTEGRITY";
    public const string InvalidHandoffId = "INVALID_STAGE8_HANDOFF_ID";
    public const string InvalidHandoffIdentity = "INVALID_STAGE8_HANDOFF_IDENTITY";
    public const string InvalidTriggerEvidence = "INVALID_TRIGGER_EVIDENCE";
    public const string InvalidContainmentEvidence = "INVALID_CONTAINMENT_EVIDENCE";
    public const string InvalidCoordinatorIdentity = "INVALID_RECOVERY_COORDINATOR_IDENTITY";
    public const string InvalidCaseState = "INVALID_RECOVERY_CASE_STATE";
    public const string InvalidCreatedAt = "INVALID_CREATED_AT";
    public const string InvalidPlanId = "INVALID_RECOVERY_PLAN_ID";
    public const string InvalidPlanVersion = "INVALID_RECOVERY_PLAN_VERSION";
    public const string InvalidCaseBinding = "INVALID_RECOVERY_CASE_BINDING";
    public const string InvalidPlanOwner = "INVALID_PLAN_OWNER_IDENTITY";
    public const string InvalidRepairActor = "INVALID_REPAIR_ACTOR_IDENTITY";
    public const string InvalidIndependentVerifier = "INVALID_INDEPENDENT_VERIFIER_IDENTITY";
    public const string InvalidReleaseAuthority = "INVALID_DECLARED_RELEASE_AUTHORITY_IDENTITY";
    public const string InvalidPrerequisites = "INVALID_PREREQUISITE_SET_IDENTITY";
    public const string InvalidRestorationSequence = "INVALID_RESTORATION_SEQUENCE_IDENTITY";
    public const string InvalidValidationCriteria = "INVALID_VALIDATION_CRITERIA_SET_IDENTITY";
    public const string InvalidAbortConditions = "INVALID_ABORT_CONDITION_SET_IDENTITY";
    public const string InvalidRollbackDirection = "INVALID_ROLLBACK_DIRECTION_IDENTITY";
    public const string InvalidAttemptBound = "INVALID_MAXIMUM_AUTHORIZED_ATTEMPTS";
    public const string InvalidResidualRiskRequirements = "INVALID_RESIDUAL_RISK_REQUIREMENTS_IDENTITY";
    public const string RepairActorVerifierCollision = "REPAIR_ACTOR_CANNOT_BE_INDEPENDENT_VERIFIER";
    public const string RepairActorReleaseAuthorityCollision = "REPAIR_ACTOR_CANNOT_BE_RELEASE_AUTHORITY";
    public const string SubjectVerifierCollision = "SUBJECT_CANNOT_BE_INDEPENDENT_VERIFIER";
    public const string GuardianVerifierCollision = "GUARDIAN_CANNOT_BE_INDEPENDENT_VERIFIER";
    public const string SubjectReleaseAuthorityCollision = "SUBJECT_CANNOT_BE_RELEASE_AUTHORITY";
    public const string GuardianReleaseAuthorityCollision = "GUARDIAN_CANNOT_BE_RELEASE_AUTHORITY";
    public const string VerifierReleaseAuthorityCollision = "INDEPENDENT_VERIFIER_CANNOT_BE_RELEASE_AUTHORITY";
}

public readonly record struct RecoveryPrimitiveValidation(bool Success, string Reason)
{
    public static RecoveryPrimitiveValidation Passed => new(true, RecoveryPrimitiveReason.Pass);
    public static RecoveryPrimitiveValidation Failed(string reason) => new(false, reason);
}

public sealed record RecoveryCase(
    string RecoveryCaseId,
    string SubjectId,
    string GuardianIdentity,
    string ControllingRestrictionId,
    string ControllingRestrictionIntegrityEvidence,
    string Stage8RecoveryHandoffId,
    string Stage8RecoveryHandoffIdentity,
    string TriggerEvidenceIdentity,
    string ContainmentEvidenceIdentity,
    string RecoveryCoordinatorIdentity,
    RecoveryCaseState State,
    DateTimeOffset CreatedAt)
{
    public string Identity => RecoveryPrimitiveIdentity.ComputeCase(this);
}

public sealed record RecoveryPlan(
    string RecoveryPlanId,
    int Version,
    string RecoveryCaseId,
    string RecoveryCaseIdentity,
    string PlanOwnerIdentity,
    string RecoveryCoordinatorIdentity,
    string RepairActorIdentity,
    string IndependentRecoveryVerifierIdentity,
    string DeclaredReleaseAuthorityIdentity,
    string PrerequisiteSetIdentity,
    string RestorationSequenceIdentity,
    string ValidationCriteriaSetIdentity,
    string AbortConditionSetIdentity,
    string RollbackDirectionIdentity,
    int MaximumAuthorizedAttempts,
    string ResidualRiskRequirementsIdentity,
    DateTimeOffset CreatedAt)
{
    public string Identity => RecoveryPrimitiveIdentity.ComputePlan(this);
}

public static class RecoveryPrimitiveValidator
{
    public static RecoveryPrimitiveValidation ValidateCase(RecoveryCase? value)
    {
        if (value is null || !CanonicalToken(value.RecoveryCaseId))
            return RecoveryPrimitiveValidation.Failed(RecoveryPrimitiveReason.InvalidCaseId);
        if (!CanonicalToken(value.SubjectId))
            return RecoveryPrimitiveValidation.Failed(RecoveryPrimitiveReason.InvalidSubjectId);
        if (!CanonicalToken(value.GuardianIdentity))
            return RecoveryPrimitiveValidation.Failed(RecoveryPrimitiveReason.InvalidGuardianIdentity);
        if (!CanonicalToken(value.ControllingRestrictionId))
            return RecoveryPrimitiveValidation.Failed(RecoveryPrimitiveReason.InvalidRestrictionId);
        if (!CanonicalToken(value.ControllingRestrictionIntegrityEvidence))
            return RecoveryPrimitiveValidation.Failed(RecoveryPrimitiveReason.InvalidRestrictionIntegrity);
        if (!CanonicalToken(value.Stage8RecoveryHandoffId))
            return RecoveryPrimitiveValidation.Failed(RecoveryPrimitiveReason.InvalidHandoffId);
        if (!CanonicalToken(value.Stage8RecoveryHandoffIdentity))
            return RecoveryPrimitiveValidation.Failed(RecoveryPrimitiveReason.InvalidHandoffIdentity);
        if (!CanonicalToken(value.TriggerEvidenceIdentity))
            return RecoveryPrimitiveValidation.Failed(RecoveryPrimitiveReason.InvalidTriggerEvidence);
        if (!CanonicalToken(value.ContainmentEvidenceIdentity))
            return RecoveryPrimitiveValidation.Failed(RecoveryPrimitiveReason.InvalidContainmentEvidence);
        if (!CanonicalToken(value.RecoveryCoordinatorIdentity))
            return RecoveryPrimitiveValidation.Failed(RecoveryPrimitiveReason.InvalidCoordinatorIdentity);
        if (!Enum.IsDefined(value.State))
            return RecoveryPrimitiveValidation.Failed(RecoveryPrimitiveReason.InvalidCaseState);
        if (value.CreatedAt == default)
            return RecoveryPrimitiveValidation.Failed(RecoveryPrimitiveReason.InvalidCreatedAt);

        return RecoveryPrimitiveValidation.Passed;
    }

    public static RecoveryPrimitiveValidation ValidatePlan(RecoveryCase? recoveryCase, RecoveryPlan? plan)
    {
        var caseValidation = ValidateCase(recoveryCase);
        if (!caseValidation.Success)
            return caseValidation;

        if (plan is null || !CanonicalToken(plan.RecoveryPlanId))
            return RecoveryPrimitiveValidation.Failed(RecoveryPrimitiveReason.InvalidPlanId);
        if (plan.Version <= 0)
            return RecoveryPrimitiveValidation.Failed(RecoveryPrimitiveReason.InvalidPlanVersion);
        if (!CanonicalToken(plan.RecoveryCaseId) ||
            !CanonicalToken(plan.RecoveryCaseIdentity) ||
            !string.Equals(plan.RecoveryCaseId, recoveryCase!.RecoveryCaseId, StringComparison.Ordinal) ||
            !string.Equals(plan.RecoveryCaseIdentity, recoveryCase.Identity, StringComparison.Ordinal))
        {
            return RecoveryPrimitiveValidation.Failed(RecoveryPrimitiveReason.InvalidCaseBinding);
        }
        if (!CanonicalToken(plan.PlanOwnerIdentity))
            return RecoveryPrimitiveValidation.Failed(RecoveryPrimitiveReason.InvalidPlanOwner);
        if (!CanonicalToken(plan.RecoveryCoordinatorIdentity) ||
            !string.Equals(plan.RecoveryCoordinatorIdentity, recoveryCase.RecoveryCoordinatorIdentity, StringComparison.Ordinal))
        {
            return RecoveryPrimitiveValidation.Failed(RecoveryPrimitiveReason.InvalidCoordinatorIdentity);
        }
        if (!CanonicalToken(plan.RepairActorIdentity))
            return RecoveryPrimitiveValidation.Failed(RecoveryPrimitiveReason.InvalidRepairActor);
        if (!CanonicalToken(plan.IndependentRecoveryVerifierIdentity))
            return RecoveryPrimitiveValidation.Failed(RecoveryPrimitiveReason.InvalidIndependentVerifier);
        if (!CanonicalToken(plan.DeclaredReleaseAuthorityIdentity))
            return RecoveryPrimitiveValidation.Failed(RecoveryPrimitiveReason.InvalidReleaseAuthority);
        if (!CanonicalToken(plan.PrerequisiteSetIdentity))
            return RecoveryPrimitiveValidation.Failed(RecoveryPrimitiveReason.InvalidPrerequisites);
        if (!CanonicalToken(plan.RestorationSequenceIdentity))
            return RecoveryPrimitiveValidation.Failed(RecoveryPrimitiveReason.InvalidRestorationSequence);
        if (!CanonicalToken(plan.ValidationCriteriaSetIdentity))
            return RecoveryPrimitiveValidation.Failed(RecoveryPrimitiveReason.InvalidValidationCriteria);
        if (!CanonicalToken(plan.AbortConditionSetIdentity))
            return RecoveryPrimitiveValidation.Failed(RecoveryPrimitiveReason.InvalidAbortConditions);
        if (!CanonicalToken(plan.RollbackDirectionIdentity))
            return RecoveryPrimitiveValidation.Failed(RecoveryPrimitiveReason.InvalidRollbackDirection);
        if (plan.MaximumAuthorizedAttempts <= 0)
            return RecoveryPrimitiveValidation.Failed(RecoveryPrimitiveReason.InvalidAttemptBound);
        if (!CanonicalToken(plan.ResidualRiskRequirementsIdentity))
            return RecoveryPrimitiveValidation.Failed(RecoveryPrimitiveReason.InvalidResidualRiskRequirements);
        if (plan.CreatedAt == default)
            return RecoveryPrimitiveValidation.Failed(RecoveryPrimitiveReason.InvalidCreatedAt);

        if (Same(plan.RepairActorIdentity, plan.IndependentRecoveryVerifierIdentity))
            return RecoveryPrimitiveValidation.Failed(RecoveryPrimitiveReason.RepairActorVerifierCollision);
        if (Same(plan.RepairActorIdentity, plan.DeclaredReleaseAuthorityIdentity))
            return RecoveryPrimitiveValidation.Failed(RecoveryPrimitiveReason.RepairActorReleaseAuthorityCollision);
        if (Same(recoveryCase.SubjectId, plan.IndependentRecoveryVerifierIdentity))
            return RecoveryPrimitiveValidation.Failed(RecoveryPrimitiveReason.SubjectVerifierCollision);
        if (Same(recoveryCase.GuardianIdentity, plan.IndependentRecoveryVerifierIdentity))
            return RecoveryPrimitiveValidation.Failed(RecoveryPrimitiveReason.GuardianVerifierCollision);
        if (Same(recoveryCase.SubjectId, plan.DeclaredReleaseAuthorityIdentity))
            return RecoveryPrimitiveValidation.Failed(RecoveryPrimitiveReason.SubjectReleaseAuthorityCollision);
        if (Same(recoveryCase.GuardianIdentity, plan.DeclaredReleaseAuthorityIdentity))
            return RecoveryPrimitiveValidation.Failed(RecoveryPrimitiveReason.GuardianReleaseAuthorityCollision);
        if (Same(plan.IndependentRecoveryVerifierIdentity, plan.DeclaredReleaseAuthorityIdentity))
            return RecoveryPrimitiveValidation.Failed(RecoveryPrimitiveReason.VerifierReleaseAuthorityCollision);

        return RecoveryPrimitiveValidation.Passed;
    }

    private static bool Same(string left, string right) =>
        string.Equals(left, right, StringComparison.Ordinal);

    private static bool CanonicalToken(string? value)
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

internal static class RecoveryPrimitiveIdentity
{
    internal static string ComputeCase(RecoveryCase value)
    {
        var canonical = string.Join("\n", new[]
        {
            value.RecoveryCaseId,
            value.SubjectId,
            value.GuardianIdentity,
            value.ControllingRestrictionId,
            value.ControllingRestrictionIntegrityEvidence,
            value.Stage8RecoveryHandoffId,
            value.Stage8RecoveryHandoffIdentity,
            value.TriggerEvidenceIdentity,
            value.ContainmentEvidenceIdentity,
            value.RecoveryCoordinatorIdentity,
            ((int)value.State).ToString(CultureInfo.InvariantCulture),
            Time(value.CreatedAt)
        });

        return Digest("stage9-recovery-case-v1", canonical);
    }

    internal static string ComputePlan(RecoveryPlan value)
    {
        var canonical = string.Join("\n", new[]
        {
            value.RecoveryPlanId,
            value.Version.ToString(CultureInfo.InvariantCulture),
            value.RecoveryCaseId,
            value.RecoveryCaseIdentity,
            value.PlanOwnerIdentity,
            value.RecoveryCoordinatorIdentity,
            value.RepairActorIdentity,
            value.IndependentRecoveryVerifierIdentity,
            value.DeclaredReleaseAuthorityIdentity,
            value.PrerequisiteSetIdentity,
            value.RestorationSequenceIdentity,
            value.ValidationCriteriaSetIdentity,
            value.AbortConditionSetIdentity,
            value.RollbackDirectionIdentity,
            value.MaximumAuthorizedAttempts.ToString(CultureInfo.InvariantCulture),
            value.ResidualRiskRequirementsIdentity,
            Time(value.CreatedAt)
        });

        return Digest("stage9-recovery-plan-v1", canonical);
    }

    private static string Time(DateTimeOffset value) =>
        value.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture);

    private static string Digest(string domain, string canonical)
    {
        var bytes = Encoding.UTF8.GetBytes(domain + "\n" + canonical);
        return Convert.ToHexString(SHA256.HashData(bytes));
    }
}
