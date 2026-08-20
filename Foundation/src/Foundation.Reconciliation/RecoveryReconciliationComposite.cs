using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Foundation.Reconciliation;

public enum RecoveryReconciliationClassification
{
    Complete = 1,
    Partial = 2,
    Failed = 3,
    Uncertain = 4
}

public enum RecoveryReconciliationDimensionStatus
{
    Complete = 1,
    Partial = 2,
    Failed = 3,
    Uncertain = 4
}

public static class RecoveryReconciliationReason
{
    public const string Pass = "PASS";
    public const string InvalidInput = "INVALID_RECOVERY_RECONCILIATION_INPUT";
    public const string InvalidFoundationResult = "INVALID_FOUNDATION_RECONCILIATION_RESULT";
    public const string MissingDimensionEvidence = "MISSING_REQUIRED_RECOVERY_RECONCILIATION_EVIDENCE";
    public const string FoundationFailed = "FOUNDATION_RECONCILIATION_FAILED";
    public const string FoundationUncertain = "FOUNDATION_RECONCILIATION_UNCERTAIN";
    public const string DimensionFailed = "RECOVERY_RECONCILIATION_DIMENSION_FAILED";
    public const string DimensionUncertain = "RECOVERY_RECONCILIATION_DIMENSION_UNCERTAIN";
    public const string DimensionPartial = "RECOVERY_RECONCILIATION_DIMENSION_PARTIAL";
    public const string StaleSecurityContext = "STALE_SECURITY_CONTEXT_NOT_TRUSTED";
}

public readonly record struct RecoveryReconciliationValidation(bool Success, string Reason)
{
    public static RecoveryReconciliationValidation Passed => new(true, RecoveryReconciliationReason.Pass);
    public static RecoveryReconciliationValidation Failed(string reason) => new(false, reason);
}

public sealed record RecoveryReconciliationDimension(
    string DimensionIdentity,
    RecoveryReconciliationDimensionStatus Status,
    string EvidenceIdentity,
    bool Current,
    bool Trusted);

public sealed record RecoveryReconciliationInput(
    string RecoveryCaseIdentity,
    string AuthorizedRecoveryPlanIdentity,
    string RestorationOutcomeIdentity,
    string FoundationReconciliationRequestIdentity,
    string FoundationReconciliationResultIdentity,
    RecoveryReconciliationDimension Configuration,
    RecoveryReconciliationDimension Authority,
    RecoveryReconciliationDimension Security,
    RecoveryReconciliationDimension DurableState,
    RecoveryReconciliationDimension Dependency,
    RecoveryReconciliationDimension Restriction,
    RecoveryReconciliationDimension EvidenceProvenance,
    DateTimeOffset EvaluatedAt);

public sealed record RecoveryReconciliationComposite(
    RecoveryReconciliationClassification Classification,
    string Reason,
    string RecoveryCaseIdentity,
    string AuthorizedRecoveryPlanIdentity,
    string RestorationOutcomeIdentity,
    string FoundationReconciliationResultIdentity,
    string ConfigurationEvidenceIdentity,
    string AuthorityEvidenceIdentity,
    string SecurityEvidenceIdentity,
    string DurableStateEvidenceIdentity,
    string DependencyEvidenceIdentity,
    string RestrictionEvidenceIdentity,
    string EvidenceProvenanceIdentity,
    DateTimeOffset EvaluatedAt)
{
    public string Identity => RecoveryReconciliationIdentity.Compute(this);
}

public static class RecoveryReconciliationCompositeBuilder
{
    public static RecoveryReconciliationValidation ValidateInput(
        ReconciliationRequest foundationRequest,
        ReconciliationResult foundationResult,
        RecoveryReconciliationInput input)
    {
        if (!Token(input.RecoveryCaseIdentity) ||
            !Token(input.AuthorizedRecoveryPlanIdentity) ||
            !Token(input.RestorationOutcomeIdentity) ||
            !Token(input.FoundationReconciliationRequestIdentity) ||
            !Token(input.FoundationReconciliationResultIdentity) ||
            input.EvaluatedAt == default)
        {
            return RecoveryReconciliationValidation.Failed(RecoveryReconciliationReason.InvalidInput);
        }

        var actualRequestIdentity = ReconciliationCanonicalEncoding.RequestIdentity(foundationRequest);
        var actualResultIdentity = ReconciliationCanonicalEncoding.ResultIdentity(foundationRequest, foundationResult);

        if (!Same(input.FoundationReconciliationRequestIdentity, actualRequestIdentity) ||
            !Same(input.FoundationReconciliationResultIdentity, actualResultIdentity))
        {
            return RecoveryReconciliationValidation.Failed(RecoveryReconciliationReason.InvalidFoundationResult);
        }

        foreach (var dimension in RequiredDimensions(input))
        {
            if (!Token(dimension.DimensionIdentity) ||
                !Token(dimension.EvidenceIdentity) ||
                !Enum.IsDefined(dimension.Status))
            {
                return RecoveryReconciliationValidation.Failed(RecoveryReconciliationReason.MissingDimensionEvidence);
            }
        }

        return RecoveryReconciliationValidation.Passed;
    }

    public static RecoveryReconciliationComposite Build(
        ReconciliationRequest foundationRequest,
        ReconciliationResult foundationResult,
        RecoveryReconciliationInput input)
    {
        var validation = ValidateInput(foundationRequest, foundationResult, input);
        if (!validation.Success)
            throw new InvalidOperationException(validation.Reason);

        var foundationClassification = ClassifyFoundationResult(foundationResult);
        if (foundationClassification.Classification != RecoveryReconciliationClassification.Complete)
        {
            return Create(input, foundationClassification.Classification, foundationClassification.Reason);
        }

        if (!input.Security.Current || !input.Security.Trusted)
            return Create(input, RecoveryReconciliationClassification.Uncertain, RecoveryReconciliationReason.StaleSecurityContext);

        var dimensions = RequiredDimensions(input);

        foreach (var dimension in dimensions)
        {
            if (!dimension.Current || !dimension.Trusted || dimension.Status == RecoveryReconciliationDimensionStatus.Uncertain)
                return Create(input, RecoveryReconciliationClassification.Uncertain, RecoveryReconciliationReason.DimensionUncertain);
        }

        foreach (var dimension in dimensions)
        {
            if (dimension.Status == RecoveryReconciliationDimensionStatus.Failed)
                return Create(input, RecoveryReconciliationClassification.Failed, RecoveryReconciliationReason.DimensionFailed);
        }

        foreach (var dimension in dimensions)
        {
            if (dimension.Status == RecoveryReconciliationDimensionStatus.Partial)
                return Create(input, RecoveryReconciliationClassification.Partial, RecoveryReconciliationReason.DimensionPartial);
        }

        return Create(input, RecoveryReconciliationClassification.Complete, RecoveryReconciliationReason.Pass);
    }

    private static (RecoveryReconciliationClassification Classification, string Reason) ClassifyFoundationResult(
        ReconciliationResult result)
    {
        // Explicit authoritative failure classifications are terminal fail-closed facts.
        // A challenge requirement may accompany those failures, but it must never soften
        // a known failed/corrupted state into merely UNCERTAIN.
        if (result.Classification is ReconciliationClassification.ConflictingDuplicate or
                ReconciliationClassification.StaleWrite or
                ReconciliationClassification.StateAheadOfEvidence or
                ReconciliationClassification.EvidenceAheadOfState or
                ReconciliationClassification.AcceptedFactMissing or
                ReconciliationClassification.AcceptedFactWithoutDurableState or
                ReconciliationClassification.CurrentStateCorrupted or
                ReconciliationClassification.EvidenceJournalInvalid or
                ReconciliationClassification.FailedClosed)
        {
            return (RecoveryReconciliationClassification.Failed, RecoveryReconciliationReason.FoundationFailed);
        }

        // Explicit uncertainty remains uncertainty even when continuation is denied.
        // Denial here is a conservative consequence of unresolved truth, not proof that
        // the authoritative state itself has been classified as a terminal failure.
        if (result.ChallengeRequired ||
            result.Classification is ReconciliationClassification.UncertainBeforeCommit or
                ReconciliationClassification.UncertainAfterCommit or
                ReconciliationClassification.ChallengeRequired)
        {
            return (RecoveryReconciliationClassification.Uncertain, RecoveryReconciliationReason.FoundationUncertain);
        }

        if (!result.ContinuationAllowed)
        {
            return (RecoveryReconciliationClassification.Failed, RecoveryReconciliationReason.FoundationFailed);
        }

        return result.Classification switch
        {
            ReconciliationClassification.Consistent or
            ReconciliationClassification.NewEmptyRoot or
            ReconciliationClassification.DuplicateIdentical or
            ReconciliationClassification.TrustedStateReconstructed
                => (RecoveryReconciliationClassification.Complete, RecoveryReconciliationReason.Pass),
            _ => (RecoveryReconciliationClassification.Uncertain, RecoveryReconciliationReason.FoundationUncertain)
        };
    }

    private static RecoveryReconciliationComposite Create(
        RecoveryReconciliationInput input,
        RecoveryReconciliationClassification classification,
        string reason) =>
        new(
            classification,
            reason,
            input.RecoveryCaseIdentity,
            input.AuthorizedRecoveryPlanIdentity,
            input.RestorationOutcomeIdentity,
            input.FoundationReconciliationResultIdentity,
            input.Configuration.EvidenceIdentity,
            input.Authority.EvidenceIdentity,
            input.Security.EvidenceIdentity,
            input.DurableState.EvidenceIdentity,
            input.Dependency.EvidenceIdentity,
            input.Restriction.EvidenceIdentity,
            input.EvidenceProvenance.EvidenceIdentity,
            input.EvaluatedAt);

    private static RecoveryReconciliationDimension[] RequiredDimensions(RecoveryReconciliationInput input) =>
    [
        input.Configuration,
        input.Authority,
        input.Security,
        input.DurableState,
        input.Dependency,
        input.Restriction,
        input.EvidenceProvenance
    ];

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

internal static class RecoveryReconciliationIdentity
{
    internal static string Compute(RecoveryReconciliationComposite value)
    {
        var canonical = string.Join("\n", new[]
        {
            ((int)value.Classification).ToString(CultureInfo.InvariantCulture),
            value.Reason,
            value.RecoveryCaseIdentity,
            value.AuthorizedRecoveryPlanIdentity,
            value.RestorationOutcomeIdentity,
            value.FoundationReconciliationResultIdentity,
            value.ConfigurationEvidenceIdentity,
            value.AuthorityEvidenceIdentity,
            value.SecurityEvidenceIdentity,
            value.DurableStateEvidenceIdentity,
            value.DependencyEvidenceIdentity,
            value.RestrictionEvidenceIdentity,
            value.EvidenceProvenanceIdentity,
            value.EvaluatedAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)
        });

        var bytes = Encoding.UTF8.GetBytes("stage9-recovery-reconciliation-composite-v1\n" + canonical);
        return Convert.ToHexString(SHA256.HashData(bytes));
    }
}
