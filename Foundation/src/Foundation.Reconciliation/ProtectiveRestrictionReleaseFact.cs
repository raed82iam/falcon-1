using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Foundation.Reconciliation;

public enum ProtectiveRestrictionReleaseClassification
{
    Released = 1,
    Partial = 2,
    Failed = 3,
    Uncertain = 4
}

public enum ProtectiveEnforcementReleaseState
{
    Released = 1,
    StillEnforced = 2,
    Failed = 3,
    Unknown = 4
}

public static class ProtectiveRestrictionReleaseReason
{
    public const string Pass = "PROTECTIVE_RESTRICTION_RELEASE_FACT_RECORDED";
    public const string InvalidInput = "INVALID_PROTECTIVE_RESTRICTION_RELEASE_INPUT";
    public const string AuthorizationNotGranted = "RELEASE_AUTHORIZATION_NOT_GRANTED";
    public const string AuthorizationExpired = "RELEASE_AUTHORIZATION_EXPIRED";
    public const string AuthorizationBindingMismatch = "RELEASE_AUTHORIZATION_BINDING_MISMATCH";
    public const string ReadinessBindingMismatch = "RELEASE_READINESS_BINDING_MISMATCH";
    public const string RestrictionChanged = "CONTROLLING_RESTRICTION_CHANGED_BEFORE_RELEASE_EXECUTION";
    public const string NewerStricterRestriction = "NEWER_OR_STRICTER_CONTROLLING_RESTRICTION_PRESENT";
    public const string ReconciliationChanged = "RECOVERY_RECONCILIATION_CHANGED_BEFORE_RELEASE_EXECUTION";
    public const string SecurityChanged = "SECURITY_TRUST_CHANGED_BEFORE_RELEASE_EXECUTION";
    public const string DependencyChanged = "DEPENDENCY_TRUST_CHANGED_BEFORE_RELEASE_EXECUTION";
    public const string ResidualRiskChanged = "RESIDUAL_RISK_CHANGED_BEFORE_RELEASE_EXECUTION";
    public const string MaterialTrustUncertain = "MATERIAL_TRUST_UNCERTAIN_AT_RELEASE_EXECUTION";
    public const string EnforcementEvidenceMissing = "ENFORCEMENT_RELEASE_EVIDENCE_MISSING";
    public const string EnforcementEvidenceUncertain = "ENFORCEMENT_RELEASE_EVIDENCE_UNCERTAIN";
    public const string EnforcementPartial = "ENFORCEMENT_RELEASE_PARTIAL";
    public const string EnforcementFailed = "ENFORCEMENT_RELEASE_FAILED";
}

public sealed record ProtectiveEnforcementReleaseAcknowledgement(
    string EnforcementPointIdentity,
    ProtectiveEnforcementReleaseState State,
    string EvidenceIdentity,
    bool Current,
    bool Trusted,
    DateTimeOffset ObservedAt);

public sealed record ProtectiveRestrictionReleaseExecutionInput(
    string RecoveryCaseIdentity,
    string SubjectIdentity,
    string OriginalRestrictionIdentity,
    string OriginalRestrictionIntegrityEvidenceIdentity,
    string ReleaseAuthorizationIdentity,
    string RecoveryReadinessIdentity,
    string IndependentValidationIdentity,
    string ReleaseConditionSatisfactionIdentity,
    RecoveryReleaseTrustSnapshot CurrentTrustSnapshot,
    IReadOnlyCollection<string> ExpectedEnforcementPointIdentities,
    IReadOnlyCollection<ProtectiveEnforcementReleaseAcknowledgement> EnforcementAcknowledgements,
    DateTimeOffset ExecutionTime);

public sealed record ProtectiveRestrictionReleaseFact(
    ProtectiveRestrictionReleaseClassification Classification,
    string Reason,
    string RecoveryCaseIdentity,
    string SubjectIdentity,
    string OriginalRestrictionIdentity,
    string OriginalRestrictionIntegrityEvidenceIdentity,
    string ReleaseAuthorizationIdentity,
    string RecoveryReadinessIdentity,
    string IndependentValidationIdentity,
    string ReleaseConditionSatisfactionIdentity,
    string RecoveryReconciliationIdentity,
    string SecurityStateEvidenceIdentity,
    string DependencyStateEvidenceIdentity,
    string ResidualRiskEvidenceIdentity,
    string ResidualRiskProfileIdentity,
    string EnforcementEvidenceIdentity,
    DateTimeOffset EffectiveBoundary)
{
    public string Identity => ProtectiveRestrictionReleaseIdentity.Compute(this);
}

public static class ProtectiveRestrictionReleaseExecutor
{
    public static ProtectiveRestrictionReleaseFact Execute(
        RecoveryReleaseReadinessDecision readiness,
        RecoveryReleaseAuthorizationDecision authorization,
        ProtectiveRestrictionReleaseExecutionInput input)
    {
        if (!ValidInput(input))
            return Create(readiness, authorization, input,
                ProtectiveRestrictionReleaseClassification.Failed,
                ProtectiveRestrictionReleaseReason.InvalidInput,
                "missing-enforcement-evidence");

        if (authorization.Classification != RecoveryReleaseAuthorizationClassification.Authorized)
            return Create(readiness, authorization, input,
                authorization.Classification == RecoveryReleaseAuthorizationClassification.Uncertain
                    ? ProtectiveRestrictionReleaseClassification.Uncertain
                    : ProtectiveRestrictionReleaseClassification.Failed,
                ProtectiveRestrictionReleaseReason.AuthorizationNotGranted,
                EnforcementDigest(input.EnforcementAcknowledgements));

        if (input.ExecutionTime < authorization.AuthorizationTime ||
            input.ExecutionTime >= authorization.AuthorityExpiry)
            return Create(readiness, authorization, input,
                ProtectiveRestrictionReleaseClassification.Failed,
                ProtectiveRestrictionReleaseReason.AuthorizationExpired,
                EnforcementDigest(input.EnforcementAcknowledgements));

        if (!Same(input.RecoveryCaseIdentity, authorization.RecoveryCaseIdentity) ||
            !Same(input.SubjectIdentity, authorization.SubjectIdentity) ||
            !Same(input.OriginalRestrictionIdentity, authorization.CurrentControllingRestrictionIdentity) ||
            !Same(input.OriginalRestrictionIntegrityEvidenceIdentity, authorization.CurrentRestrictionIntegrityEvidenceIdentity) ||
            !Same(input.ReleaseAuthorizationIdentity, authorization.Identity) ||
            !Same(input.RecoveryReadinessIdentity, authorization.RecoveryReadinessIdentity))
            return Create(readiness, authorization, input,
                ProtectiveRestrictionReleaseClassification.Failed,
                ProtectiveRestrictionReleaseReason.AuthorizationBindingMismatch,
                EnforcementDigest(input.EnforcementAcknowledgements));

        if (readiness.Classification != RecoveryReleaseReadinessClassification.ReadyForReleaseDecision ||
            !Same(input.RecoveryCaseIdentity, readiness.RecoveryCaseIdentity) ||
            !Same(input.RecoveryReadinessIdentity, readiness.Identity) ||
            !Same(input.OriginalRestrictionIdentity, readiness.CurrentControllingRestrictionIdentity) ||
            !Same(input.OriginalRestrictionIntegrityEvidenceIdentity, readiness.CurrentRestrictionIntegrityEvidenceIdentity) ||
            !Same(input.IndependentValidationIdentity, readiness.IndependentValidationIdentity) ||
            !Same(input.ReleaseConditionSatisfactionIdentity, readiness.GuardianConditionEvidenceIdentity))
            return Create(readiness, authorization, input,
                ProtectiveRestrictionReleaseClassification.Failed,
                ProtectiveRestrictionReleaseReason.ReadinessBindingMismatch,
                EnforcementDigest(input.EnforcementAcknowledgements));

        var snapshot = input.CurrentTrustSnapshot;

        if (!Same(snapshot.CurrentControllingRestrictionIdentity, input.OriginalRestrictionIdentity) ||
            !Same(snapshot.CurrentRestrictionIntegrityEvidenceIdentity, input.OriginalRestrictionIntegrityEvidenceIdentity))
            return Create(readiness, authorization, input,
                ProtectiveRestrictionReleaseClassification.Failed,
                ProtectiveRestrictionReleaseReason.RestrictionChanged,
                EnforcementDigest(input.EnforcementAcknowledgements));

        if (snapshot.NewerOrStricterRestrictionPresent)
            return Create(readiness, authorization, input,
                ProtectiveRestrictionReleaseClassification.Failed,
                ProtectiveRestrictionReleaseReason.NewerStricterRestriction,
                EnforcementDigest(input.EnforcementAcknowledgements));

        if (!Same(snapshot.RecoveryReconciliationIdentity, readiness.RecoveryReconciliationIdentity))
            return Create(readiness, authorization, input,
                ProtectiveRestrictionReleaseClassification.Failed,
                ProtectiveRestrictionReleaseReason.ReconciliationChanged,
                EnforcementDigest(input.EnforcementAcknowledgements));

        if (!Same(snapshot.SecurityStateEvidenceIdentity, readiness.SecurityStateEvidenceIdentity))
            return Create(readiness, authorization, input,
                ProtectiveRestrictionReleaseClassification.Failed,
                ProtectiveRestrictionReleaseReason.SecurityChanged,
                EnforcementDigest(input.EnforcementAcknowledgements));

        if (!Same(snapshot.DependencyStateEvidenceIdentity, readiness.DependencyStateEvidenceIdentity))
            return Create(readiness, authorization, input,
                ProtectiveRestrictionReleaseClassification.Failed,
                ProtectiveRestrictionReleaseReason.DependencyChanged,
                EnforcementDigest(input.EnforcementAcknowledgements));

        if (!Same(snapshot.ResidualRiskEvidenceIdentity, readiness.ResidualRiskEvidenceIdentity) ||
            !Same(snapshot.ResidualRiskProfileIdentity, readiness.ResidualRiskProfileIdentity) ||
            !snapshot.ResidualRiskWithinAuthorizedBounds)
            return Create(readiness, authorization, input,
                ProtectiveRestrictionReleaseClassification.Failed,
                ProtectiveRestrictionReleaseReason.ResidualRiskChanged,
                EnforcementDigest(input.EnforcementAcknowledgements));

        if (!MaterialTrustCurrentAndTrusted(snapshot))
            return Create(readiness, authorization, input,
                ProtectiveRestrictionReleaseClassification.Uncertain,
                ProtectiveRestrictionReleaseReason.MaterialTrustUncertain,
                EnforcementDigest(input.EnforcementAcknowledgements));

        var enforcement = EvaluateEnforcement(input);
        return Create(readiness, authorization, input,
            enforcement.Classification,
            enforcement.Reason,
            enforcement.EvidenceIdentity);
    }

    private static (ProtectiveRestrictionReleaseClassification Classification, string Reason, string EvidenceIdentity) EvaluateEnforcement(
        ProtectiveRestrictionReleaseExecutionInput input)
    {
        var expected = input.ExpectedEnforcementPointIdentities
            .OrderBy(value => value, StringComparer.Ordinal)
            .ToArray();
        var acknowledgements = input.EnforcementAcknowledgements
            .OrderBy(value => value.EnforcementPointIdentity, StringComparer.Ordinal)
            .ToArray();
        var digest = EnforcementDigest(acknowledgements);

        if (expected.Length != acknowledgements.Length ||
            !expected.SequenceEqual(acknowledgements.Select(value => value.EnforcementPointIdentity), StringComparer.Ordinal))
            return (ProtectiveRestrictionReleaseClassification.Partial,
                ProtectiveRestrictionReleaseReason.EnforcementEvidenceMissing,
                digest);

        if (acknowledgements.Any(value => !value.Current || !value.Trusted || value.State == ProtectiveEnforcementReleaseState.Unknown))
            return (ProtectiveRestrictionReleaseClassification.Uncertain,
                ProtectiveRestrictionReleaseReason.EnforcementEvidenceUncertain,
                digest);

        if (acknowledgements.Any(value => value.State == ProtectiveEnforcementReleaseState.Failed))
            return (ProtectiveRestrictionReleaseClassification.Failed,
                ProtectiveRestrictionReleaseReason.EnforcementFailed,
                digest);

        if (acknowledgements.Any(value => value.State == ProtectiveEnforcementReleaseState.StillEnforced))
            return (ProtectiveRestrictionReleaseClassification.Partial,
                ProtectiveRestrictionReleaseReason.EnforcementPartial,
                digest);

        return (ProtectiveRestrictionReleaseClassification.Released,
            ProtectiveRestrictionReleaseReason.Pass,
            digest);
    }

    private static bool MaterialTrustCurrentAndTrusted(RecoveryReleaseTrustSnapshot value) =>
        value.ReconciliationCurrent && value.ReconciliationTrusted &&
        value.SecurityStateCurrent && value.SecurityStateTrusted &&
        value.DependencyStateCurrent && value.DependencyStateTrusted &&
        value.ResidualRiskCurrent && value.ResidualRiskTrusted;

    private static bool ValidInput(ProtectiveRestrictionReleaseExecutionInput input)
    {
        if (!Token(input.RecoveryCaseIdentity) ||
            !Token(input.SubjectIdentity) ||
            !Token(input.OriginalRestrictionIdentity) ||
            !Token(input.OriginalRestrictionIntegrityEvidenceIdentity) ||
            !Token(input.ReleaseAuthorizationIdentity) ||
            !Token(input.RecoveryReadinessIdentity) ||
            !Token(input.IndependentValidationIdentity) ||
            !Token(input.ReleaseConditionSatisfactionIdentity) ||
            input.CurrentTrustSnapshot is null ||
            input.ExpectedEnforcementPointIdentities is null ||
            input.EnforcementAcknowledgements is null ||
            input.ExecutionTime == default)
            return false;

        var expected = input.ExpectedEnforcementPointIdentities.ToArray();
        if (expected.Length == 0 || expected.Any(value => !Token(value)) ||
            expected.Distinct(StringComparer.Ordinal).Count() != expected.Length)
            return false;

        var acknowledgements = input.EnforcementAcknowledgements.ToArray();
        if (acknowledgements.Any(value => value is null ||
            !Token(value.EnforcementPointIdentity) ||
            !Enum.IsDefined(value.State) ||
            !Token(value.EvidenceIdentity) ||
            value.ObservedAt == default ||
            value.ObservedAt > input.ExecutionTime) ||
            acknowledgements.Select(value => value.EnforcementPointIdentity)
                .Distinct(StringComparer.Ordinal).Count() != acknowledgements.Length)
            return false;

        var snapshot = input.CurrentTrustSnapshot;
        return Token(snapshot.CurrentControllingRestrictionIdentity) &&
            Token(snapshot.CurrentRestrictionIntegrityEvidenceIdentity) &&
            Token(snapshot.RecoveryReconciliationIdentity) &&
            Token(snapshot.SecurityStateEvidenceIdentity) &&
            Token(snapshot.DependencyStateEvidenceIdentity) &&
            Token(snapshot.ResidualRiskEvidenceIdentity) &&
            Token(snapshot.ResidualRiskProfileIdentity);
    }

    private static ProtectiveRestrictionReleaseFact Create(
        RecoveryReleaseReadinessDecision readiness,
        RecoveryReleaseAuthorizationDecision authorization,
        ProtectiveRestrictionReleaseExecutionInput input,
        ProtectiveRestrictionReleaseClassification classification,
        string reason,
        string enforcementEvidenceIdentity) =>
        new(
            classification,
            reason,
            Clean(input.RecoveryCaseIdentity),
            Clean(input.SubjectIdentity),
            Clean(input.OriginalRestrictionIdentity),
            Clean(input.OriginalRestrictionIntegrityEvidenceIdentity),
            Clean(input.ReleaseAuthorizationIdentity),
            Clean(input.RecoveryReadinessIdentity),
            Clean(input.IndependentValidationIdentity),
            Clean(input.ReleaseConditionSatisfactionIdentity),
            Clean(input.CurrentTrustSnapshot?.RecoveryReconciliationIdentity),
            Clean(input.CurrentTrustSnapshot?.SecurityStateEvidenceIdentity),
            Clean(input.CurrentTrustSnapshot?.DependencyStateEvidenceIdentity),
            Clean(input.CurrentTrustSnapshot?.ResidualRiskEvidenceIdentity),
            Clean(input.CurrentTrustSnapshot?.ResidualRiskProfileIdentity),
            enforcementEvidenceIdentity,
            input.ExecutionTime == default ? DateTimeOffset.UnixEpoch : input.ExecutionTime);

    private static string EnforcementDigest(IEnumerable<ProtectiveEnforcementReleaseAcknowledgement> values)
    {
        var canonical = string.Join("\n", values
            .OrderBy(value => value.EnforcementPointIdentity, StringComparer.Ordinal)
            .Select(value => string.Join("|",
                value.EnforcementPointIdentity,
                ((int)value.State).ToString(CultureInfo.InvariantCulture),
                value.EvidenceIdentity,
                value.Current ? "1" : "0",
                value.Trusted ? "1" : "0",
                value.ObservedAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture))));
        return Digest("protective-enforcement-release-v1\n" + canonical);
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

    private static string Digest(string value) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
}

internal static class ProtectiveRestrictionReleaseIdentity
{
    internal static string Compute(ProtectiveRestrictionReleaseFact value)
    {
        var canonical = string.Join("\n", new[]
        {
            ((int)value.Classification).ToString(CultureInfo.InvariantCulture),
            value.Reason,
            value.RecoveryCaseIdentity,
            value.SubjectIdentity,
            value.OriginalRestrictionIdentity,
            value.OriginalRestrictionIntegrityEvidenceIdentity,
            value.ReleaseAuthorizationIdentity,
            value.RecoveryReadinessIdentity,
            value.IndependentValidationIdentity,
            value.ReleaseConditionSatisfactionIdentity,
            value.RecoveryReconciliationIdentity,
            value.SecurityStateEvidenceIdentity,
            value.DependencyStateEvidenceIdentity,
            value.ResidualRiskEvidenceIdentity,
            value.ResidualRiskProfileIdentity,
            value.EnforcementEvidenceIdentity,
            value.EffectiveBoundary.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)
        });
        return Convert.ToHexString(SHA256.HashData(
            Encoding.UTF8.GetBytes("protective-restriction-release-fact-v1\n" + canonical)));
    }
}
