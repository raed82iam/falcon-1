namespace Falcon.FSATS.TradingGuardian.Application;

public enum GuardianLifecycleTransitionKind { Update, Rollback, Replacement, Removal }
public enum GuardianLifecycleCompatibility { CompatibleAsIs, MigrationRequired, Incompatible, Unknown }
public enum GuardianLifecycleReadiness { ReadyForExternalLifecycleReview, MigrationRequired, MigrationValidated, RollbackEligible, RollbackBlocked, RemovalReady, Blocked }

public sealed record GuardianLifecycleTransition(string TransitionId, GuardianLifecycleTransitionKind Kind, string ApplicationId, string SourceVersion, string TargetVersion, string SourcePackageId, string TargetPackageId, string SourceSchema, string TargetSchema, long SourceTrustEpoch, long CurrentTrustEpoch, string EvidenceId, bool MigrationEvidenceValidated, GuardianLifecycleCompatibility Compatibility);
public sealed record GuardianLifecycleSafetyState(string ProtectionTargetId, string IncidentId, string CorrelationId, string IdempotencyKey, bool HasUnresolvedProtectionOutcome, bool RequiresCurrentProtectionTruthVerification, bool HasActiveContainmentOrRestriction, bool HasStaleProtectionAuthority, bool RequiredEvidenceRetained);
public sealed record GuardianLifecycleAssessment(bool Accepted, GuardianLifecycleReadiness Readiness, string ReasonCode, bool PreservesProtectionTruth, bool GrantsRuntimeAuthority)
{
    public static GuardianLifecycleAssessment Reject(string reason, GuardianLifecycleReadiness readiness = GuardianLifecycleReadiness.Blocked) => new(false, readiness, reason, true, false);
}

public static class GuardianLifecycleEvolution
{
    public const string ApplicationId = "FSATS-TRADING-GUARDIAN";

    public static GuardianLifecycleAssessment Assess(GuardianLifecycleTransition? transition, GuardianLifecycleSafetyState? safety)
    {
        if (transition is null || safety is null) return GuardianLifecycleAssessment.Reject("GUARDIAN_LIFECYCLE_INPUT_REQUIRED");
        if (!Enum.IsDefined(transition.Kind) || !Enum.IsDefined(transition.Compatibility)) return GuardianLifecycleAssessment.Reject("GUARDIAN_LIFECYCLE_ENUM_INVALID");
        if (!Valid(transition.TransitionId) || !Valid(transition.ApplicationId) || !Valid(transition.SourceVersion) || !Valid(transition.SourcePackageId) || !Valid(transition.SourceSchema) || !Valid(transition.EvidenceId)) return GuardianLifecycleAssessment.Reject("GUARDIAN_LIFECYCLE_IDENTITY_INCOMPLETE");
        if (!StringComparer.Ordinal.Equals(transition.ApplicationId, ApplicationId)) return GuardianLifecycleAssessment.Reject("GUARDIAN_LIFECYCLE_APPLICATION_ID_MISMATCH");
        if (transition.SourceTrustEpoch < 0 || transition.SourceTrustEpoch != transition.CurrentTrustEpoch) return GuardianLifecycleAssessment.Reject("GUARDIAN_LIFECYCLE_STALE_TRUST_EPOCH");
        if (!Valid(safety.ProtectionTargetId) || !Valid(safety.IncidentId) || !Valid(safety.CorrelationId) || !Valid(safety.IdempotencyKey)) return GuardianLifecycleAssessment.Reject("GUARDIAN_LIFECYCLE_PROTECTION_IDENTITY_INCOMPLETE");
        if (!safety.RequiredEvidenceRetained) return GuardianLifecycleAssessment.Reject("GUARDIAN_LIFECYCLE_REQUIRED_EVIDENCE_NOT_RETAINED");
        if (safety.HasStaleProtectionAuthority) return GuardianLifecycleAssessment.Reject("GUARDIAN_LIFECYCLE_STALE_PROTECTION_AUTHORITY_PRESENT");
        if (transition.Kind != GuardianLifecycleTransitionKind.Removal && (!Valid(transition.TargetVersion) || !Valid(transition.TargetPackageId) || !Valid(transition.TargetSchema))) return GuardianLifecycleAssessment.Reject("GUARDIAN_LIFECYCLE_TARGET_IDENTITY_INCOMPLETE");
        if (transition.Kind == GuardianLifecycleTransitionKind.Replacement && StringComparer.Ordinal.Equals(transition.SourcePackageId, transition.TargetPackageId)) return GuardianLifecycleAssessment.Reject("GUARDIAN_LIFECYCLE_REPLACEMENT_PACKAGE_IDENTITY_NOT_DISTINCT");
        if (transition.Compatibility is GuardianLifecycleCompatibility.Unknown or GuardianLifecycleCompatibility.Incompatible) return GuardianLifecycleAssessment.Reject(transition.Compatibility == GuardianLifecycleCompatibility.Unknown ? "GUARDIAN_LIFECYCLE_COMPATIBILITY_UNKNOWN" : "GUARDIAN_LIFECYCLE_INCOMPATIBLE");
        var protectionUnresolved = safety.HasUnresolvedProtectionOutcome || safety.RequiresCurrentProtectionTruthVerification || safety.HasActiveContainmentOrRestriction;
        if (transition.Kind == GuardianLifecycleTransitionKind.Removal)
        {
            if (protectionUnresolved) return GuardianLifecycleAssessment.Reject("GUARDIAN_LIFECYCLE_REMOVAL_BLOCKED_BY_PROTECTION_OBLIGATION");
            return new(true, GuardianLifecycleReadiness.RemovalReady, "GUARDIAN_LIFECYCLE_REMOVAL_RECONCILED", true, false);
        }
        if (transition.Kind == GuardianLifecycleTransitionKind.Rollback && protectionUnresolved) return GuardianLifecycleAssessment.Reject("GUARDIAN_LIFECYCLE_ROLLBACK_BLOCKED_BY_CURRENT_PROTECTION_TRUTH", GuardianLifecycleReadiness.RollbackBlocked);
        if (transition.Compatibility == GuardianLifecycleCompatibility.MigrationRequired && !transition.MigrationEvidenceValidated) return new(true, GuardianLifecycleReadiness.MigrationRequired, "GUARDIAN_LIFECYCLE_MIGRATION_REQUIRED", true, false);
        var readiness = transition.Kind == GuardianLifecycleTransitionKind.Rollback ? GuardianLifecycleReadiness.RollbackEligible : GuardianLifecycleReadiness.ReadyForExternalLifecycleReview;
        var reason = transition.Compatibility == GuardianLifecycleCompatibility.MigrationRequired ? "GUARDIAN_LIFECYCLE_MIGRATION_VALIDATED_READY_FOR_EXTERNAL_REVIEW" : "GUARDIAN_LIFECYCLE_READY_FOR_EXTERNAL_REVIEW";
        return new(true, readiness, reason, true, false);
    }

    private static bool Valid(string? value) => !string.IsNullOrWhiteSpace(value) && StringComparer.Ordinal.Equals(value, value.Trim());
}
