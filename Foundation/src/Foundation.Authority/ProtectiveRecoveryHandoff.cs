using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Foundation.Contracts;

namespace Foundation.Authority;

public enum ProtectiveReleaseActorRole
{
    Subject = 1,
    Guardian = 2,
    RepairActor = 3,
    IndependentVerifier = 4,
    DeclaredReleaseAuthority = 5,
    Other = 6
}

public static class ProtectiveReleaseGuardReason
{
    public const string SubjectSelfReleaseDenied = "SUBJECT_SELF_RELEASE_DENIED";
    public const string GuardianSelfReleaseDenied = "GUARDIAN_SELF_RELEASE_DENIED";
    public const string IndependentRecoveryReleaseRequired = "INDEPENDENT_RECOVERY_RELEASE_REQUIRED";
    public const string InvalidRequest = "INVALID_PROTECTIVE_RELEASE_GUARD_REQUEST";
}

public sealed record ProtectiveReleaseGuardResult(
    bool Allowed,
    string Reason,
    string ActorIdentity,
    ProtectiveReleaseActorRole ActorRole,
    string SubjectId,
    string RestrictionId,
    bool RestrictionRemainsEnforced,
    bool RecoveryAuthorityRequired,
    DateTimeOffset DecisionTime);

public static class ProtectiveReleaseGuard
{
    public static ProtectiveReleaseGuardResult Evaluate(
        string? actorIdentity,
        ProtectiveReleaseActorRole actorRole,
        string? subjectId,
        string? guardianIdentity,
        string? restrictionId,
        DateTimeOffset observationTime)
    {
        if (!CanonicalToken(actorIdentity) ||
            !Enum.IsDefined(actorRole) ||
            !CanonicalToken(subjectId) ||
            !CanonicalToken(guardianIdentity) ||
            !CanonicalToken(restrictionId) ||
            observationTime == default)
        {
            return new ProtectiveReleaseGuardResult(
                false,
                ProtectiveReleaseGuardReason.InvalidRequest,
                Clean(actorIdentity, "missing-actor"),
                Enum.IsDefined(actorRole) ? actorRole : ProtectiveReleaseActorRole.Other,
                Clean(subjectId, "missing-subject"),
                Clean(restrictionId, "missing-restriction"),
                true,
                true,
                observationTime == default ? DateTimeOffset.UnixEpoch : observationTime);
        }

        if (actorRole == ProtectiveReleaseActorRole.Subject ||
            string.Equals(actorIdentity, subjectId, StringComparison.Ordinal))
        {
            return Denied(
                ProtectiveReleaseGuardReason.SubjectSelfReleaseDenied,
                actorIdentity!,
                actorRole,
                subjectId!,
                restrictionId!,
                observationTime);
        }

        if (actorRole == ProtectiveReleaseActorRole.Guardian ||
            string.Equals(actorIdentity, guardianIdentity, StringComparison.Ordinal))
        {
            return Denied(
                ProtectiveReleaseGuardReason.GuardianSelfReleaseDenied,
                actorIdentity!,
                actorRole,
                subjectId!,
                restrictionId!,
                observationTime);
        }

        return Denied(
            ProtectiveReleaseGuardReason.IndependentRecoveryReleaseRequired,
            actorIdentity!,
            actorRole,
            subjectId!,
            restrictionId!,
            observationTime);
    }

    private static ProtectiveReleaseGuardResult Denied(
        string reason,
        string actorIdentity,
        ProtectiveReleaseActorRole actorRole,
        string subjectId,
        string restrictionId,
        DateTimeOffset decisionTime) =>
        new(
            false,
            reason,
            actorIdentity,
            actorRole,
            subjectId,
            restrictionId,
            true,
            true,
            decisionTime);

    private static string Clean(string? value, string fallback) =>
        string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();

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

public sealed record RecoveryEvidencePackage(
    string EvidencePackageId,
    string SubjectId,
    string RestrictionId,
    string RestrictionIntegrityEvidence,
    string GuardianIdentity,
    string RepairActorIdentity,
    string IndependentVerifierIdentity,
    string DeclaredReleaseAuthorityIdentity,
    string AuthoritativeStateReconciliationEvidence,
    string SecurityContextReestablishmentEvidence,
    string DependencyReconciliationEvidence,
    string IndependentRecoveryValidationEvidence,
    string GuardianConditionEvidence,
    string ResidualRiskEvidence,
    bool AuthoritativeStateReconciled,
    bool SecurityContextReestablished,
    bool DependenciesReconciled,
    bool IndependentRecoveryValidationPassed,
    bool GuardianConditionsSatisfied,
    DateTimeOffset ObservedAt,
    DateTimeOffset Expiry)
{
    public string Identity => RecoveryHandoffIdentity.ComputeEvidence(this);
}

public static class RecoveryHandoffReason
{
    public const string ReadyForRecoveryEvaluation = "READY_FOR_RECOVERY_EVALUATION";
    public const string InvalidRestriction = "INVALID_RESTRICTION";
    public const string InvalidRecoveryEvidence = "INVALID_RECOVERY_EVIDENCE";
    public const string RoleSeparationFailure = "ROLE_SEPARATION_FAILURE";
    public const string RecoveryEvidenceIncomplete = "RECOVERY_EVIDENCE_INCOMPLETE";
    public const string RecoveryValidationFailed = "RECOVERY_VALIDATION_FAILED";
}

public sealed class RecoveryHandoffRecord
{
    internal RecoveryHandoffRecord(
        string handoffId,
        string reason,
        string subjectId,
        string restrictionId,
        string restrictionIntegrityEvidence,
        string releaseConditions,
        string releaseAuthority,
        string recoveryEvidenceIdentity,
        string independentVerifierIdentity,
        string declaredReleaseAuthorityIdentity,
        bool readyForRecoveryEvaluation,
        bool releaseEligibleInProtectionContext,
        bool restrictionRemainsEnforced,
        bool independentRecoveryValidationRequired,
        bool authorizedReleaseDecisionRequired,
        bool lifecycleReintroductionRequired,
        bool newAuthorityDecisionRequired,
        DateTimeOffset handoffTime)
    {
        HandoffId = handoffId;
        Reason = reason;
        SubjectId = subjectId;
        RestrictionId = restrictionId;
        RestrictionIntegrityEvidence = restrictionIntegrityEvidence;
        ReleaseConditions = releaseConditions;
        ReleaseAuthority = releaseAuthority;
        RecoveryEvidenceIdentity = recoveryEvidenceIdentity;
        IndependentVerifierIdentity = independentVerifierIdentity;
        DeclaredReleaseAuthorityIdentity = declaredReleaseAuthorityIdentity;
        ReadyForRecoveryEvaluation = readyForRecoveryEvaluation;
        ReleaseEligibleInProtectionContext = releaseEligibleInProtectionContext;
        RestrictionRemainsEnforced = restrictionRemainsEnforced;
        IndependentRecoveryValidationRequired = independentRecoveryValidationRequired;
        AuthorizedReleaseDecisionRequired = authorizedReleaseDecisionRequired;
        LifecycleReintroductionRequired = lifecycleReintroductionRequired;
        NewAuthorityDecisionRequired = newAuthorityDecisionRequired;
        HandoffTime = handoffTime;
    }

    public string HandoffId { get; }
    public string Reason { get; }
    public string SubjectId { get; }
    public string RestrictionId { get; }
    public string RestrictionIntegrityEvidence { get; }
    public string ReleaseConditions { get; }
    public string ReleaseAuthority { get; }
    public string RecoveryEvidenceIdentity { get; }
    public string IndependentVerifierIdentity { get; }
    public string DeclaredReleaseAuthorityIdentity { get; }
    public bool ReadyForRecoveryEvaluation { get; }
    public bool ReleaseEligibleInProtectionContext { get; }
    public bool RestrictionRemainsEnforced { get; }
    public bool IndependentRecoveryValidationRequired { get; }
    public bool AuthorizedReleaseDecisionRequired { get; }
    public bool LifecycleReintroductionRequired { get; }
    public bool NewAuthorityDecisionRequired { get; }
    public DateTimeOffset HandoffTime { get; }
    public string Identity => RecoveryHandoffIdentity.ComputeHandoff(this);
}

public static class RecoveryHandoffRuntime
{
    public static RecoveryHandoffRecord Evaluate(
        string handoffId,
        RestrictionRecord? restriction,
        RecoveryEvidencePackage? evidence,
        DateTimeOffset observationTime)
    {
        if (!CanonicalToken(handoffId) ||
            restriction is null ||
            ContractValidators.Validate(restriction).Result != ValidationResult.Pass ||
            !string.Equals(restriction.Result, "IMPOSED", StringComparison.Ordinal) ||
            observationTime == default ||
            observationTime < restriction.EffectiveTime)
        {
            return Create(
                Clean(handoffId, "invalid-handoff"),
                RecoveryHandoffReason.InvalidRestriction,
                restriction,
                evidence,
                false,
                observationTime);
        }

        if (!ValidEvidence(restriction, evidence, observationTime))
        {
            return Create(
                handoffId,
                RecoveryHandoffReason.InvalidRecoveryEvidence,
                restriction,
                evidence,
                false,
                observationTime);
        }

        var package = evidence!;

        if (!RolesSeparated(package))
        {
            return Create(
                handoffId,
                RecoveryHandoffReason.RoleSeparationFailure,
                restriction,
                package,
                false,
                observationTime);
        }

        if (!package.IndependentRecoveryValidationPassed)
        {
            return Create(
                handoffId,
                RecoveryHandoffReason.RecoveryValidationFailed,
                restriction,
                package,
                false,
                observationTime);
        }

        if (!package.AuthoritativeStateReconciled ||
            !package.SecurityContextReestablished ||
            !package.DependenciesReconciled ||
            !package.GuardianConditionsSatisfied)
        {
            return Create(
                handoffId,
                RecoveryHandoffReason.RecoveryEvidenceIncomplete,
                restriction,
                package,
                false,
                observationTime);
        }

        return Create(
            handoffId,
            RecoveryHandoffReason.ReadyForRecoveryEvaluation,
            restriction,
            package,
            true,
            observationTime);
    }

    public static bool ValidateHandoff(RecoveryHandoffRecord? handoff)
    {
        if (handoff is null ||
            !CanonicalToken(handoff.HandoffId) ||
            !CanonicalToken(handoff.Reason) ||
            !CanonicalToken(handoff.SubjectId) ||
            !CanonicalToken(handoff.RestrictionId) ||
            !CanonicalToken(handoff.RestrictionIntegrityEvidence) ||
            !CanonicalToken(handoff.ReleaseConditions) ||
            !CanonicalToken(handoff.ReleaseAuthority) ||
            !CanonicalToken(handoff.RecoveryEvidenceIdentity) ||
            !CanonicalToken(handoff.IndependentVerifierIdentity) ||
            !CanonicalToken(handoff.DeclaredReleaseAuthorityIdentity) ||
            handoff.HandoffTime == default ||
            handoff.ReleaseEligibleInProtectionContext ||
            !handoff.RestrictionRemainsEnforced ||
            !handoff.IndependentRecoveryValidationRequired ||
            !handoff.AuthorizedReleaseDecisionRequired ||
            !handoff.LifecycleReintroductionRequired ||
            !handoff.NewAuthorityDecisionRequired)
        {
            return false;
        }

        return string.Equals(
            handoff.Identity,
            RecoveryHandoffIdentity.ComputeHandoff(handoff),
            StringComparison.Ordinal);
    }

    private static RecoveryHandoffRecord Create(
        string handoffId,
        string reason,
        RestrictionRecord? restriction,
        RecoveryEvidencePackage? evidence,
        bool ready,
        DateTimeOffset observationTime)
    {
        var subjectId = Clean(restriction?.SubjectId, evidence?.SubjectId ?? "missing-subject");
        var restrictionId = Clean(restriction?.RestrictionId, evidence?.RestrictionId ?? "missing-restriction");
        var restrictionIntegrity = Clean(restriction?.IntegrityEvidence, evidence?.RestrictionIntegrityEvidence ?? "missing-restriction-integrity");
        var releaseConditions = Clean(restriction?.ReleaseConditions, "independent-recovery-validation-and-authorized-release-required");
        var releaseAuthority = Clean(restriction?.ReleaseAuthority, "independent-governed-release-authority");
        var recoveryEvidenceIdentity = evidence is null ? "missing-recovery-evidence" : evidence.Identity;
        var verifier = Clean(evidence?.IndependentVerifierIdentity, "missing-independent-verifier");
        var declaredReleaseAuthority = Clean(evidence?.DeclaredReleaseAuthorityIdentity, "missing-declared-release-authority");
        var time = observationTime == default ? DateTimeOffset.UnixEpoch : observationTime;

        return new RecoveryHandoffRecord(
            handoffId,
            reason,
            subjectId,
            restrictionId,
            restrictionIntegrity,
            releaseConditions,
            releaseAuthority,
            recoveryEvidenceIdentity,
            verifier,
            declaredReleaseAuthority,
            ready,
            false,
            true,
            true,
            true,
            true,
            true,
            time);
    }

    private static bool ValidEvidence(
        RestrictionRecord restriction,
        RecoveryEvidencePackage? evidence,
        DateTimeOffset observationTime)
    {
        if (evidence is null ||
            !CanonicalToken(evidence.EvidencePackageId) ||
            !CanonicalToken(evidence.SubjectId) ||
            !CanonicalToken(evidence.RestrictionId) ||
            !CanonicalToken(evidence.RestrictionIntegrityEvidence) ||
            !CanonicalToken(evidence.GuardianIdentity) ||
            !CanonicalToken(evidence.RepairActorIdentity) ||
            !CanonicalToken(evidence.IndependentVerifierIdentity) ||
            !CanonicalToken(evidence.DeclaredReleaseAuthorityIdentity) ||
            !CanonicalToken(evidence.AuthoritativeStateReconciliationEvidence) ||
            !CanonicalToken(evidence.SecurityContextReestablishmentEvidence) ||
            !CanonicalToken(evidence.DependencyReconciliationEvidence) ||
            !CanonicalToken(evidence.IndependentRecoveryValidationEvidence) ||
            !CanonicalToken(evidence.GuardianConditionEvidence) ||
            !CanonicalToken(evidence.ResidualRiskEvidence) ||
            evidence.ObservedAt == default ||
            evidence.Expiry <= evidence.ObservedAt ||
            observationTime < evidence.ObservedAt ||
            observationTime >= evidence.Expiry)
        {
            return false;
        }

        return string.Equals(evidence.SubjectId, restriction.SubjectId, StringComparison.Ordinal) &&
            string.Equals(evidence.RestrictionId, restriction.RestrictionId, StringComparison.Ordinal) &&
            string.Equals(evidence.RestrictionIntegrityEvidence, restriction.IntegrityEvidence, StringComparison.Ordinal);
    }

    private static bool RolesSeparated(RecoveryEvidencePackage evidence)
    {
        if (string.Equals(evidence.IndependentVerifierIdentity, evidence.SubjectId, StringComparison.Ordinal) ||
            string.Equals(evidence.IndependentVerifierIdentity, evidence.GuardianIdentity, StringComparison.Ordinal) ||
            string.Equals(evidence.IndependentVerifierIdentity, evidence.RepairActorIdentity, StringComparison.Ordinal))
        {
            return false;
        }

        if (string.Equals(evidence.DeclaredReleaseAuthorityIdentity, evidence.SubjectId, StringComparison.Ordinal) ||
            string.Equals(evidence.DeclaredReleaseAuthorityIdentity, evidence.GuardianIdentity, StringComparison.Ordinal) ||
            string.Equals(evidence.DeclaredReleaseAuthorityIdentity, evidence.RepairActorIdentity, StringComparison.Ordinal))
        {
            return false;
        }

        return true;
    }

    private static string Clean(string? value, string fallback) =>
        string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();

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

internal static class RecoveryHandoffIdentity
{
    internal static string ComputeEvidence(RecoveryEvidencePackage value)
    {
        var canonical = string.Join("\n", new[]
        {
            value.EvidencePackageId,
            value.SubjectId,
            value.RestrictionId,
            value.RestrictionIntegrityEvidence,
            value.GuardianIdentity,
            value.RepairActorIdentity,
            value.IndependentVerifierIdentity,
            value.DeclaredReleaseAuthorityIdentity,
            value.AuthoritativeStateReconciliationEvidence,
            value.SecurityContextReestablishmentEvidence,
            value.DependencyReconciliationEvidence,
            value.IndependentRecoveryValidationEvidence,
            value.GuardianConditionEvidence,
            value.ResidualRiskEvidence,
            value.AuthoritativeStateReconciled ? "1" : "0",
            value.SecurityContextReestablished ? "1" : "0",
            value.DependenciesReconciled ? "1" : "0",
            value.IndependentRecoveryValidationPassed ? "1" : "0",
            value.GuardianConditionsSatisfied ? "1" : "0",
            Time(value.ObservedAt),
            Time(value.Expiry)
        });

        return Digest("recovery-evidence", canonical);
    }

    internal static string ComputeHandoff(RecoveryHandoffRecord value)
    {
        var canonical = string.Join("\n", new[]
        {
            value.HandoffId,
            value.Reason,
            value.SubjectId,
            value.RestrictionId,
            value.RestrictionIntegrityEvidence,
            value.ReleaseConditions,
            value.ReleaseAuthority,
            value.RecoveryEvidenceIdentity,
            value.IndependentVerifierIdentity,
            value.DeclaredReleaseAuthorityIdentity,
            value.ReadyForRecoveryEvaluation ? "1" : "0",
            value.ReleaseEligibleInProtectionContext ? "1" : "0",
            value.RestrictionRemainsEnforced ? "1" : "0",
            value.IndependentRecoveryValidationRequired ? "1" : "0",
            value.AuthorizedReleaseDecisionRequired ? "1" : "0",
            value.LifecycleReintroductionRequired ? "1" : "0",
            value.NewAuthorityDecisionRequired ? "1" : "0",
            Time(value.HandoffTime)
        });

        return Digest("recovery-handoff", canonical);
    }

    private static string Digest(string prefix, string canonical) =>
        prefix + "/sha256/" + Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(canonical)));

    private static string Time(DateTimeOffset value) =>
        value.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture);
}
