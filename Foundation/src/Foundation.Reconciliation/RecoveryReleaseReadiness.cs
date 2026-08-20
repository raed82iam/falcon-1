using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Foundation.Reconciliation;

public enum RecoveryReleaseReadinessClassification
{
    ReadyForReleaseDecision = 1,
    NotReady = 2,
    Uncertain = 3
}

public static class RecoveryReleaseReadinessReason
{
    public const string Pass = "READY_FOR_RELEASE_DECISION";
    public const string InvalidInput = "INVALID_RECOVERY_RELEASE_READINESS_INPUT";
    public const string InvalidHandoff = "INVALID_OR_STALE_RECOVERY_HANDOFF";
    public const string RecoveryBindingMismatch = "RECOVERY_READINESS_BINDING_MISMATCH";
    public const string ReconciliationNotComplete = "RECOVERY_RECONCILIATION_NOT_COMPLETE";
    public const string IndependentValidationNotPassed = "INDEPENDENT_RECOVERY_VALIDATION_NOT_PASSED";
    public const string RestrictionMismatch = "CONTROLLING_RESTRICTION_MISMATCH";
    public const string NewerStricterRestriction = "NEWER_OR_STRICTER_CONTROLLING_RESTRICTION_PRESENT";
    public const string GuardianConditionsUnsatisfied = "GUARDIAN_RELEASE_CONDITIONS_UNSATISFIED";
    public const string GuardianConditionsUntrusted = "GUARDIAN_RELEASE_CONDITIONS_UNTRUSTED";
    public const string SecurityStateNotCurrent = "SECURITY_STATE_NOT_CURRENT_OR_TRUSTED";
    public const string DependencyStateNotCurrent = "DEPENDENCY_STATE_NOT_CURRENT_OR_TRUSTED";
    public const string ResidualRiskMissing = "RESIDUAL_RISK_EVIDENCE_MISSING_OR_UNTRUSTED";
    public const string ResidualRiskOutsideBounds = "RESIDUAL_RISK_OUTSIDE_AUTHORIZED_BOUNDS";
}

public sealed record RecoveryReadinessHandoffSnapshot(
    string HandoffIdentity,
    string SubjectIdentity,
    string RestrictionIdentity,
    string RestrictionIntegrityEvidenceIdentity,
    string ReleaseConditionsIdentity,
    string DeclaredReleaseAuthorityIdentity,
    string IndependentVerifierIdentity,
    bool HandoffValid,
    bool ReadyForRecoveryEvaluation,
    bool RestrictionRemainsEnforced,
    DateTimeOffset HandoffTime);

public sealed record RecoveryReadinessConditionEvidence(
    string EvidenceIdentity,
    bool Satisfied,
    bool Current,
    bool Trusted);

public sealed record RecoveryResidualRiskEvidence(
    string EvidenceIdentity,
    string RiskProfileIdentity,
    bool Current,
    bool Trusted,
    bool WithinAuthorizedBounds);

public sealed record RecoveryReleaseReadinessInput(
    string RecoveryCaseIdentity,
    string AuthorizedRecoveryPlanIdentity,
    RecoveryReadinessHandoffSnapshot Handoff,
    string CurrentControllingRestrictionIdentity,
    string CurrentRestrictionIntegrityEvidenceIdentity,
    bool NewerOrStricterRestrictionPresent,
    RecoveryReadinessConditionEvidence GuardianConditions,
    RecoveryReadinessConditionEvidence SecurityState,
    RecoveryReadinessConditionEvidence DependencyState,
    RecoveryResidualRiskEvidence ResidualRisk,
    DateTimeOffset EvaluatedAt);

public sealed record RecoveryReleaseReadinessDecision(
    RecoveryReleaseReadinessClassification Classification,
    string Reason,
    string RecoveryCaseIdentity,
    string AuthorizedRecoveryPlanIdentity,
    string HandoffIdentity,
    string RecoveryReconciliationIdentity,
    string IndependentValidationIdentity,
    string CurrentControllingRestrictionIdentity,
    string CurrentRestrictionIntegrityEvidenceIdentity,
    string GuardianConditionEvidenceIdentity,
    string SecurityStateEvidenceIdentity,
    string DependencyStateEvidenceIdentity,
    string ResidualRiskEvidenceIdentity,
    string ResidualRiskProfileIdentity,
    string DeclaredReleaseAuthorityIdentity,
    DateTimeOffset EvaluatedAt)
{
    public string Identity => RecoveryReleaseReadinessIdentity.Compute(this);
}

public static class RecoveryReleaseReadinessEvaluator
{
    public static RecoveryReleaseReadinessDecision Evaluate(
        RecoveryReconciliationComposite reconciliation,
        IndependentRecoveryValidationDecision validation,
        RecoveryReleaseReadinessInput input)
    {
        if (!ValidInput(input))
            return Create(reconciliation, validation, input,
                RecoveryReleaseReadinessClassification.NotReady,
                RecoveryReleaseReadinessReason.InvalidInput);

        if (!input.Handoff.HandoffValid ||
            !input.Handoff.ReadyForRecoveryEvaluation ||
            !input.Handoff.RestrictionRemainsEnforced ||
            input.Handoff.HandoffTime == default ||
            input.EvaluatedAt < input.Handoff.HandoffTime)
        {
            return Create(reconciliation, validation, input,
                RecoveryReleaseReadinessClassification.NotReady,
                RecoveryReleaseReadinessReason.InvalidHandoff);
        }

        if (!Same(input.RecoveryCaseIdentity, reconciliation.RecoveryCaseIdentity) ||
            !Same(input.RecoveryCaseIdentity, validation.RecoveryCaseIdentity) ||
            !Same(input.AuthorizedRecoveryPlanIdentity, reconciliation.AuthorizedRecoveryPlanIdentity) ||
            !Same(input.AuthorizedRecoveryPlanIdentity, validation.AuthorizedRecoveryPlanIdentity) ||
            !Same(validation.RecoveryReconciliationIdentity, reconciliation.Identity) ||
            !Same(input.Handoff.IndependentVerifierIdentity, validation.IndependentVerifierIdentity) ||
            !Same(input.Handoff.DeclaredReleaseAuthorityIdentity, validation.DeclaredReleaseAuthorityIdentity))
        {
            return Create(reconciliation, validation, input,
                RecoveryReleaseReadinessClassification.NotReady,
                RecoveryReleaseReadinessReason.RecoveryBindingMismatch);
        }

        if (reconciliation.Classification != RecoveryReconciliationClassification.Complete)
        {
            return Create(reconciliation, validation, input,
                reconciliation.Classification == RecoveryReconciliationClassification.Uncertain
                    ? RecoveryReleaseReadinessClassification.Uncertain
                    : RecoveryReleaseReadinessClassification.NotReady,
                RecoveryReleaseReadinessReason.ReconciliationNotComplete);
        }

        if (validation.Classification != IndependentRecoveryValidationClassification.Validated)
        {
            return Create(reconciliation, validation, input,
                validation.Classification == IndependentRecoveryValidationClassification.Uncertain
                    ? RecoveryReleaseReadinessClassification.Uncertain
                    : RecoveryReleaseReadinessClassification.NotReady,
                RecoveryReleaseReadinessReason.IndependentValidationNotPassed);
        }

        if (!Same(input.Handoff.RestrictionIdentity, input.CurrentControllingRestrictionIdentity) ||
            !Same(input.Handoff.RestrictionIntegrityEvidenceIdentity, input.CurrentRestrictionIntegrityEvidenceIdentity))
        {
            return Create(reconciliation, validation, input,
                RecoveryReleaseReadinessClassification.NotReady,
                RecoveryReleaseReadinessReason.RestrictionMismatch);
        }

        if (input.NewerOrStricterRestrictionPresent)
        {
            return Create(reconciliation, validation, input,
                RecoveryReleaseReadinessClassification.NotReady,
                RecoveryReleaseReadinessReason.NewerStricterRestriction);
        }

        if (!input.GuardianConditions.Satisfied)
        {
            return Create(reconciliation, validation, input,
                RecoveryReleaseReadinessClassification.NotReady,
                RecoveryReleaseReadinessReason.GuardianConditionsUnsatisfied);
        }

        if (!input.GuardianConditions.Current || !input.GuardianConditions.Trusted)
        {
            return Create(reconciliation, validation, input,
                RecoveryReleaseReadinessClassification.Uncertain,
                RecoveryReleaseReadinessReason.GuardianConditionsUntrusted);
        }

        if (!input.SecurityState.Satisfied || !input.SecurityState.Current || !input.SecurityState.Trusted)
        {
            return Create(reconciliation, validation, input,
                RecoveryReleaseReadinessClassification.Uncertain,
                RecoveryReleaseReadinessReason.SecurityStateNotCurrent);
        }

        if (!input.DependencyState.Satisfied || !input.DependencyState.Current || !input.DependencyState.Trusted)
        {
            return Create(reconciliation, validation, input,
                RecoveryReleaseReadinessClassification.Uncertain,
                RecoveryReleaseReadinessReason.DependencyStateNotCurrent);
        }

        if (!input.ResidualRisk.Current || !input.ResidualRisk.Trusted)
        {
            return Create(reconciliation, validation, input,
                RecoveryReleaseReadinessClassification.Uncertain,
                RecoveryReleaseReadinessReason.ResidualRiskMissing);
        }

        if (!input.ResidualRisk.WithinAuthorizedBounds)
        {
            return Create(reconciliation, validation, input,
                RecoveryReleaseReadinessClassification.NotReady,
                RecoveryReleaseReadinessReason.ResidualRiskOutsideBounds);
        }

        return Create(reconciliation, validation, input,
            RecoveryReleaseReadinessClassification.ReadyForReleaseDecision,
            RecoveryReleaseReadinessReason.Pass);
    }

    private static bool ValidInput(RecoveryReleaseReadinessInput input) =>
        Token(input.RecoveryCaseIdentity) &&
        Token(input.AuthorizedRecoveryPlanIdentity) &&
        input.Handoff is not null &&
        Token(input.Handoff.HandoffIdentity) &&
        Token(input.Handoff.SubjectIdentity) &&
        Token(input.Handoff.RestrictionIdentity) &&
        Token(input.Handoff.RestrictionIntegrityEvidenceIdentity) &&
        Token(input.Handoff.ReleaseConditionsIdentity) &&
        Token(input.Handoff.DeclaredReleaseAuthorityIdentity) &&
        Token(input.Handoff.IndependentVerifierIdentity) &&
        Token(input.CurrentControllingRestrictionIdentity) &&
        Token(input.CurrentRestrictionIntegrityEvidenceIdentity) &&
        ValidCondition(input.GuardianConditions) &&
        ValidCondition(input.SecurityState) &&
        ValidCondition(input.DependencyState) &&
        input.ResidualRisk is not null &&
        Token(input.ResidualRisk.EvidenceIdentity) &&
        Token(input.ResidualRisk.RiskProfileIdentity) &&
        input.EvaluatedAt != default;

    private static bool ValidCondition(RecoveryReadinessConditionEvidence? value) =>
        value is not null && Token(value.EvidenceIdentity);

    private static RecoveryReleaseReadinessDecision Create(
        RecoveryReconciliationComposite reconciliation,
        IndependentRecoveryValidationDecision validation,
        RecoveryReleaseReadinessInput input,
        RecoveryReleaseReadinessClassification classification,
        string reason) =>
        new(
            classification,
            reason,
            Clean(input.RecoveryCaseIdentity),
            Clean(input.AuthorizedRecoveryPlanIdentity),
            Clean(input.Handoff?.HandoffIdentity),
            reconciliation.Identity,
            validation.Identity,
            Clean(input.CurrentControllingRestrictionIdentity),
            Clean(input.CurrentRestrictionIntegrityEvidenceIdentity),
            Clean(input.GuardianConditions?.EvidenceIdentity),
            Clean(input.SecurityState?.EvidenceIdentity),
            Clean(input.DependencyState?.EvidenceIdentity),
            Clean(input.ResidualRisk?.EvidenceIdentity),
            Clean(input.ResidualRisk?.RiskProfileIdentity),
            Clean(input.Handoff?.DeclaredReleaseAuthorityIdentity),
            input.EvaluatedAt == default ? DateTimeOffset.UnixEpoch : input.EvaluatedAt);

    private static string Clean(string? value) =>
        string.IsNullOrWhiteSpace(value) ? "missing" : value.Trim();

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

internal static class RecoveryReleaseReadinessIdentity
{
    internal static string Compute(RecoveryReleaseReadinessDecision value)
    {
        var canonical = string.Join("\n", new[]
        {
            ((int)value.Classification).ToString(CultureInfo.InvariantCulture),
            value.Reason,
            value.RecoveryCaseIdentity,
            value.AuthorizedRecoveryPlanIdentity,
            value.HandoffIdentity,
            value.RecoveryReconciliationIdentity,
            value.IndependentValidationIdentity,
            value.CurrentControllingRestrictionIdentity,
            value.CurrentRestrictionIntegrityEvidenceIdentity,
            value.GuardianConditionEvidenceIdentity,
            value.SecurityStateEvidenceIdentity,
            value.DependencyStateEvidenceIdentity,
            value.ResidualRiskEvidenceIdentity,
            value.ResidualRiskProfileIdentity,
            value.DeclaredReleaseAuthorityIdentity,
            value.EvaluatedAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)
        });

        var bytes = Encoding.UTF8.GetBytes("stage9-recovery-release-readiness-v1\n" + canonical);
        return Convert.ToHexString(SHA256.HashData(bytes));
    }
}
