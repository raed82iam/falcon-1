namespace Falcon.FSATS.FSAPMA.Application;

public enum ProviderLifecycleTransitionKind { Update, Rollback, Replacement, Removal }
public enum ProviderLifecycleCompatibility { CompatibleAsIs, MigrationRequired, Incompatible, Unknown }
public enum ProviderLifecycleReadiness { ReadyForExternalLifecycleReview, MigrationRequired, MigrationValidated, RollbackEligible, RollbackBlocked, RemovalReady, Blocked }

public sealed record ProviderLifecycleTransition(string TransitionId, ProviderLifecycleTransitionKind Kind, string ApplicationId, string SourceVersion, string TargetVersion, string SourcePackageId, string TargetPackageId, string SourceSchema, string TargetSchema, long SourceTrustEpoch, long CurrentTrustEpoch, string EvidenceId, bool MigrationEvidenceValidated, ProviderLifecycleCompatibility Compatibility);
public sealed record ProviderLifecycleSafetyState(string ProviderId, string ProviderAccountId, string ServiceRole, string Environment, bool DeliveryOutcomeUnknown, bool StreamGapDetected, bool StreamStale, bool HasUnresolvedIdempotency, bool HasCurrentCredentialReference, bool ContainsSecretBytes, bool RequiredEvidenceRetained);
public sealed record ProviderLifecycleAssessment(bool Accepted, ProviderLifecycleReadiness Readiness, string ReasonCode, bool PreservesProviderTruth, bool GrantsRuntimeAuthority)
{
    public static ProviderLifecycleAssessment Reject(string reason, ProviderLifecycleReadiness readiness = ProviderLifecycleReadiness.Blocked) => new(false, readiness, reason, true, false);
}

public static class ProviderLifecycleEvolution
{
    public const string ApplicationId = "FSATS-FSAPMA";

    public static ProviderLifecycleAssessment Assess(ProviderLifecycleTransition? transition, ProviderLifecycleSafetyState? safety)
    {
        if (transition is null || safety is null) return ProviderLifecycleAssessment.Reject("FSAPMA_LIFECYCLE_INPUT_REQUIRED");
        if (!Enum.IsDefined(transition.Kind) || !Enum.IsDefined(transition.Compatibility)) return ProviderLifecycleAssessment.Reject("FSAPMA_LIFECYCLE_ENUM_INVALID");
        if (!Valid(transition.TransitionId) || !Valid(transition.ApplicationId) || !Valid(transition.SourceVersion) || !Valid(transition.SourcePackageId) || !Valid(transition.SourceSchema) || !Valid(transition.EvidenceId)) return ProviderLifecycleAssessment.Reject("FSAPMA_LIFECYCLE_IDENTITY_INCOMPLETE");
        if (!StringComparer.Ordinal.Equals(transition.ApplicationId, ApplicationId)) return ProviderLifecycleAssessment.Reject("FSAPMA_LIFECYCLE_APPLICATION_ID_MISMATCH");
        if (transition.SourceTrustEpoch < 0 || transition.SourceTrustEpoch != transition.CurrentTrustEpoch) return ProviderLifecycleAssessment.Reject("FSAPMA_LIFECYCLE_STALE_TRUST_EPOCH");
        if (!Valid(safety.ProviderId) || !Valid(safety.ProviderAccountId) || !Valid(safety.ServiceRole) || !Valid(safety.Environment)) return ProviderLifecycleAssessment.Reject("FSAPMA_LIFECYCLE_ROUTE_IDENTITY_INCOMPLETE");
        if (safety.ContainsSecretBytes) return ProviderLifecycleAssessment.Reject("FSAPMA_LIFECYCLE_SECRET_BYTES_PROHIBITED");
        if (!safety.RequiredEvidenceRetained) return ProviderLifecycleAssessment.Reject("FSAPMA_LIFECYCLE_REQUIRED_EVIDENCE_NOT_RETAINED");
        if (transition.Kind != ProviderLifecycleTransitionKind.Removal && (!Valid(transition.TargetVersion) || !Valid(transition.TargetPackageId) || !Valid(transition.TargetSchema))) return ProviderLifecycleAssessment.Reject("FSAPMA_LIFECYCLE_TARGET_IDENTITY_INCOMPLETE");
        if (transition.Kind == ProviderLifecycleTransitionKind.Replacement && StringComparer.Ordinal.Equals(transition.SourcePackageId, transition.TargetPackageId)) return ProviderLifecycleAssessment.Reject("FSAPMA_LIFECYCLE_REPLACEMENT_PACKAGE_IDENTITY_NOT_DISTINCT");
        if (transition.Compatibility is ProviderLifecycleCompatibility.Unknown or ProviderLifecycleCompatibility.Incompatible) return ProviderLifecycleAssessment.Reject(transition.Compatibility == ProviderLifecycleCompatibility.Unknown ? "FSAPMA_LIFECYCLE_COMPATIBILITY_UNKNOWN" : "FSAPMA_LIFECYCLE_INCOMPATIBLE");
        var unresolved = safety.DeliveryOutcomeUnknown || safety.StreamGapDetected || safety.StreamStale || safety.HasUnresolvedIdempotency;
        if (transition.Kind == ProviderLifecycleTransitionKind.Removal)
        {
            if (unresolved || safety.HasCurrentCredentialReference) return ProviderLifecycleAssessment.Reject("FSAPMA_LIFECYCLE_REMOVAL_BLOCKED_BY_OPEN_OBLIGATIONS");
            return new(true, ProviderLifecycleReadiness.RemovalReady, "FSAPMA_LIFECYCLE_REMOVAL_RECONCILED", true, false);
        }
        if (transition.Kind == ProviderLifecycleTransitionKind.Rollback && unresolved) return ProviderLifecycleAssessment.Reject("FSAPMA_LIFECYCLE_ROLLBACK_BLOCKED_BY_UNRESOLVED_PROVIDER_TRUTH", ProviderLifecycleReadiness.RollbackBlocked);
        if (transition.Compatibility == ProviderLifecycleCompatibility.MigrationRequired && !transition.MigrationEvidenceValidated) return new(true, ProviderLifecycleReadiness.MigrationRequired, "FSAPMA_LIFECYCLE_MIGRATION_REQUIRED", true, false);
        var readiness = transition.Kind == ProviderLifecycleTransitionKind.Rollback ? ProviderLifecycleReadiness.RollbackEligible : ProviderLifecycleReadiness.ReadyForExternalLifecycleReview;
        var reason = transition.Compatibility == ProviderLifecycleCompatibility.MigrationRequired ? "FSAPMA_LIFECYCLE_MIGRATION_VALIDATED_READY_FOR_EXTERNAL_REVIEW" : "FSAPMA_LIFECYCLE_READY_FOR_EXTERNAL_REVIEW";
        return new(true, readiness, reason, true, false);
    }

    private static bool Valid(string? value) => !string.IsNullOrWhiteSpace(value) && StringComparer.Ordinal.Equals(value, value.Trim());
}
