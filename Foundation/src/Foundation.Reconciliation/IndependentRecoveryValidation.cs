using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Foundation.Reconciliation;

public enum IndependentRecoveryValidationClassification
{
    Validated = 1,
    Denied = 2,
    Uncertain = 3
}

public static class IndependentRecoveryValidationReason
{
    public const string Pass = "PASS";
    public const string InvalidInput = "INVALID_INDEPENDENT_RECOVERY_VALIDATION_INPUT";
    public const string ReconciliationIdentityMismatch = "RECOVERY_RECONCILIATION_IDENTITY_MISMATCH";
    public const string RecoveryBindingMismatch = "RECOVERY_BINDING_MISMATCH";
    public const string VerifierNotIndependent = "INDEPENDENT_RECOVERY_VERIFIER_ROLE_CONFLICT";
    public const string EvidenceNotCurrent = "INDEPENDENT_RECOVERY_VALIDATION_EVIDENCE_NOT_CURRENT";
    public const string EvidenceNotTrusted = "INDEPENDENT_RECOVERY_VALIDATION_EVIDENCE_NOT_TRUSTED";
    public const string ReconciliationFailed = "RECOVERY_RECONCILIATION_FAILED";
    public const string ReconciliationPartial = "RECOVERY_RECONCILIATION_PARTIAL";
    public const string ReconciliationUncertain = "RECOVERY_RECONCILIATION_UNCERTAIN";
}

public sealed record IndependentRecoveryValidationInput(
    string RecoveryCaseIdentity,
    string AuthorizedRecoveryPlanIdentity,
    string RestorationOutcomeIdentity,
    string RecoveryReconciliationIdentity,
    string SubjectIdentity,
    string GuardianIdentity,
    string RepairActorIdentity,
    string IndependentVerifierIdentity,
    string DeclaredReleaseAuthorityIdentity,
    string ValidationEvidenceIdentity,
    bool EvidenceCurrent,
    bool EvidenceTrusted,
    DateTimeOffset ValidatedAt);

public sealed record IndependentRecoveryValidationDecision(
    IndependentRecoveryValidationClassification Classification,
    string Reason,
    string RecoveryCaseIdentity,
    string AuthorizedRecoveryPlanIdentity,
    string RestorationOutcomeIdentity,
    string RecoveryReconciliationIdentity,
    string IndependentVerifierIdentity,
    string DeclaredReleaseAuthorityIdentity,
    string ValidationEvidenceIdentity,
    DateTimeOffset ValidatedAt)
{
    public string Identity => IndependentRecoveryValidationIdentity.Compute(this);
}

public static class IndependentRecoveryValidator
{
    public static IndependentRecoveryValidationDecision Evaluate(
        RecoveryReconciliationComposite reconciliation,
        IndependentRecoveryValidationInput input)
    {
        if (!ValidInput(input))
            return Create(input, IndependentRecoveryValidationClassification.Denied,
                IndependentRecoveryValidationReason.InvalidInput);

        if (!Same(input.RecoveryReconciliationIdentity, reconciliation.Identity))
            return Create(input, IndependentRecoveryValidationClassification.Denied,
                IndependentRecoveryValidationReason.ReconciliationIdentityMismatch);

        if (!Same(input.RecoveryCaseIdentity, reconciliation.RecoveryCaseIdentity) ||
            !Same(input.AuthorizedRecoveryPlanIdentity, reconciliation.AuthorizedRecoveryPlanIdentity) ||
            !Same(input.RestorationOutcomeIdentity, reconciliation.RestorationOutcomeIdentity))
        {
            return Create(input, IndependentRecoveryValidationClassification.Denied,
                IndependentRecoveryValidationReason.RecoveryBindingMismatch);
        }

        if (!VerifierIsIndependent(input))
            return Create(input, IndependentRecoveryValidationClassification.Denied,
                IndependentRecoveryValidationReason.VerifierNotIndependent);

        if (!input.EvidenceCurrent)
            return Create(input, IndependentRecoveryValidationClassification.Uncertain,
                IndependentRecoveryValidationReason.EvidenceNotCurrent);

        if (!input.EvidenceTrusted)
            return Create(input, IndependentRecoveryValidationClassification.Uncertain,
                IndependentRecoveryValidationReason.EvidenceNotTrusted);

        return reconciliation.Classification switch
        {
            RecoveryReconciliationClassification.Complete =>
                Create(input, IndependentRecoveryValidationClassification.Validated,
                    IndependentRecoveryValidationReason.Pass),
            RecoveryReconciliationClassification.Failed =>
                Create(input, IndependentRecoveryValidationClassification.Denied,
                    IndependentRecoveryValidationReason.ReconciliationFailed),
            RecoveryReconciliationClassification.Partial =>
                Create(input, IndependentRecoveryValidationClassification.Denied,
                    IndependentRecoveryValidationReason.ReconciliationPartial),
            RecoveryReconciliationClassification.Uncertain =>
                Create(input, IndependentRecoveryValidationClassification.Uncertain,
                    IndependentRecoveryValidationReason.ReconciliationUncertain),
            _ =>
                Create(input, IndependentRecoveryValidationClassification.Uncertain,
                    IndependentRecoveryValidationReason.ReconciliationUncertain)
        };
    }

    private static bool VerifierIsIndependent(IndependentRecoveryValidationInput input) =>
        !Same(input.IndependentVerifierIdentity, input.SubjectIdentity) &&
        !Same(input.IndependentVerifierIdentity, input.GuardianIdentity) &&
        !Same(input.IndependentVerifierIdentity, input.RepairActorIdentity) &&
        !Same(input.IndependentVerifierIdentity, input.DeclaredReleaseAuthorityIdentity);

    private static bool ValidInput(IndependentRecoveryValidationInput input) =>
        Token(input.RecoveryCaseIdentity) &&
        Token(input.AuthorizedRecoveryPlanIdentity) &&
        Token(input.RestorationOutcomeIdentity) &&
        Token(input.RecoveryReconciliationIdentity) &&
        Token(input.SubjectIdentity) &&
        Token(input.GuardianIdentity) &&
        Token(input.RepairActorIdentity) &&
        Token(input.IndependentVerifierIdentity) &&
        Token(input.DeclaredReleaseAuthorityIdentity) &&
        Token(input.ValidationEvidenceIdentity) &&
        input.ValidatedAt != default;

    private static IndependentRecoveryValidationDecision Create(
        IndependentRecoveryValidationInput input,
        IndependentRecoveryValidationClassification classification,
        string reason) =>
        new(
            classification,
            reason,
            input.RecoveryCaseIdentity,
            input.AuthorizedRecoveryPlanIdentity,
            input.RestorationOutcomeIdentity,
            input.RecoveryReconciliationIdentity,
            input.IndependentVerifierIdentity,
            input.DeclaredReleaseAuthorityIdentity,
            input.ValidationEvidenceIdentity,
            input.ValidatedAt);

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

internal static class IndependentRecoveryValidationIdentity
{
    internal static string Compute(IndependentRecoveryValidationDecision value)
    {
        var canonical = string.Join("\n", new[]
        {
            ((int)value.Classification).ToString(CultureInfo.InvariantCulture),
            value.Reason,
            value.RecoveryCaseIdentity,
            value.AuthorizedRecoveryPlanIdentity,
            value.RestorationOutcomeIdentity,
            value.RecoveryReconciliationIdentity,
            value.IndependentVerifierIdentity,
            value.DeclaredReleaseAuthorityIdentity,
            value.ValidationEvidenceIdentity,
            value.ValidatedAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)
        });

        var bytes = Encoding.UTF8.GetBytes("stage9-independent-recovery-validation-v1\n" + canonical);
        return Convert.ToHexString(SHA256.HashData(bytes));
    }
}
