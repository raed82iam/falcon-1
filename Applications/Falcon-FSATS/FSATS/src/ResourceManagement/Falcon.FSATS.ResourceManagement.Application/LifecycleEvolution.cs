namespace Falcon.FSATS.ResourceManagement.Application;

public enum ResourceLifecycleTransitionKind { Update, Rollback, Replacement, Removal }
public enum ResourceLifecycleCompatibility { CompatibleAsIs, MigrationRequired, Incompatible, Unknown }
public enum ResourceLifecycleReadiness { ReadyForExternalLifecycleReview, MigrationRequired, MigrationValidated, RollbackEligible, RollbackBlocked, RemovalReady, Blocked }

public sealed record ResourceLifecycleTransition(string TransitionId, ResourceLifecycleTransitionKind Kind, string ApplicationId, string SourceVersion, string TargetVersion, string SourcePackageId, string TargetPackageId, string SourceSchema, string TargetSchema, long SourceTrustEpoch, long CurrentTrustEpoch, string EvidenceId, bool MigrationEvidenceValidated, ResourceLifecycleCompatibility Compatibility);
public sealed record ResourceLifecycleSafetyState(long SourceCoordinatorEpoch, long CurrentCoordinatorEpoch, string FoundationEnvelopeReference, bool HasPendingResourceOutcome, bool HasUnresolvedFoundationOutcome, bool ReferenceClaimsFoundationGrant, bool RequiredEvidenceRetained);
public sealed record ResourceLifecycleAssessment(bool Accepted, ResourceLifecycleReadiness Readiness, string ReasonCode, bool PreservesFoundationAuthorityBoundary, bool GrantsRuntimeAuthority)
{
    public static ResourceLifecycleAssessment Reject(string reason, ResourceLifecycleReadiness readiness = ResourceLifecycleReadiness.Blocked) => new(false, readiness, reason, true, false);
}

public static class ResourceLifecycleEvolution
{
    public const string ApplicationId = "APP-RSC";

    public static ResourceLifecycleAssessment Assess(ResourceLifecycleTransition? transition, ResourceLifecycleSafetyState? safety)
    {
        if (transition is null || safety is null) return ResourceLifecycleAssessment.Reject("APP_RSC_LIFECYCLE_INPUT_REQUIRED");
        if (!Enum.IsDefined(transition.Kind) || !Enum.IsDefined(transition.Compatibility)) return ResourceLifecycleAssessment.Reject("APP_RSC_LIFECYCLE_ENUM_INVALID");
        if (!Valid(transition.TransitionId) || !Valid(transition.ApplicationId) || !Valid(transition.SourceVersion) || !Valid(transition.SourcePackageId) || !Valid(transition.SourceSchema) || !Valid(transition.EvidenceId)) return ResourceLifecycleAssessment.Reject("APP_RSC_LIFECYCLE_IDENTITY_INCOMPLETE");
        if (!StringComparer.Ordinal.Equals(transition.ApplicationId, ApplicationId)) return ResourceLifecycleAssessment.Reject("APP_RSC_LIFECYCLE_APPLICATION_ID_MISMATCH");
        if (transition.SourceTrustEpoch < 0 || transition.SourceTrustEpoch != transition.CurrentTrustEpoch) return ResourceLifecycleAssessment.Reject("APP_RSC_LIFECYCLE_STALE_TRUST_EPOCH");
        if (safety.SourceCoordinatorEpoch < 0 || safety.CurrentCoordinatorEpoch < 0 || safety.SourceCoordinatorEpoch != safety.CurrentCoordinatorEpoch) return ResourceLifecycleAssessment.Reject("APP_RSC_LIFECYCLE_STALE_COORDINATOR_EPOCH");
        if (!Valid(safety.FoundationEnvelopeReference)) return ResourceLifecycleAssessment.Reject("APP_RSC_LIFECYCLE_FOUNDATION_ENVELOPE_REFERENCE_REQUIRED");
        if (safety.ReferenceClaimsFoundationGrant) return ResourceLifecycleAssessment.Reject("APP_RSC_LIFECYCLE_REFERENCE_CANNOT_MINT_FOUNDATION_GRANT");
        if (!safety.RequiredEvidenceRetained) return ResourceLifecycleAssessment.Reject("APP_RSC_LIFECYCLE_REQUIRED_EVIDENCE_NOT_RETAINED");
        if (transition.Kind != ResourceLifecycleTransitionKind.Removal && (!Valid(transition.TargetVersion) || !Valid(transition.TargetPackageId) || !Valid(transition.TargetSchema))) return ResourceLifecycleAssessment.Reject("APP_RSC_LIFECYCLE_TARGET_IDENTITY_INCOMPLETE");
        if (transition.Kind == ResourceLifecycleTransitionKind.Replacement && StringComparer.Ordinal.Equals(transition.SourcePackageId, transition.TargetPackageId)) return ResourceLifecycleAssessment.Reject("APP_RSC_LIFECYCLE_REPLACEMENT_PACKAGE_IDENTITY_NOT_DISTINCT");
        if (transition.Compatibility is ResourceLifecycleCompatibility.Unknown or ResourceLifecycleCompatibility.Incompatible) return ResourceLifecycleAssessment.Reject(transition.Compatibility == ResourceLifecycleCompatibility.Unknown ? "APP_RSC_LIFECYCLE_COMPATIBILITY_UNKNOWN" : "APP_RSC_LIFECYCLE_INCOMPATIBLE");
        var unresolved = safety.HasPendingResourceOutcome || safety.HasUnresolvedFoundationOutcome;
        if (transition.Kind == ResourceLifecycleTransitionKind.Removal)
        {
            if (unresolved) return ResourceLifecycleAssessment.Reject("APP_RSC_LIFECYCLE_REMOVAL_BLOCKED_BY_PENDING_RESOURCE_OUTCOME");
            return new(true, ResourceLifecycleReadiness.RemovalReady, "APP_RSC_LIFECYCLE_REMOVAL_RECONCILED", true, false);
        }
        if (transition.Kind == ResourceLifecycleTransitionKind.Rollback && unresolved) return ResourceLifecycleAssessment.Reject("APP_RSC_LIFECYCLE_ROLLBACK_BLOCKED_BY_PENDING_RESOURCE_OUTCOME", ResourceLifecycleReadiness.RollbackBlocked);
        if (transition.Compatibility == ResourceLifecycleCompatibility.MigrationRequired && !transition.MigrationEvidenceValidated) return new(true, ResourceLifecycleReadiness.MigrationRequired, "APP_RSC_LIFECYCLE_MIGRATION_REQUIRED", true, false);
        var readiness = transition.Kind == ResourceLifecycleTransitionKind.Rollback ? ResourceLifecycleReadiness.RollbackEligible : ResourceLifecycleReadiness.ReadyForExternalLifecycleReview;
        var reason = transition.Compatibility == ResourceLifecycleCompatibility.MigrationRequired ? "APP_RSC_LIFECYCLE_MIGRATION_VALIDATED_READY_FOR_EXTERNAL_REVIEW" : "APP_RSC_LIFECYCLE_READY_FOR_EXTERNAL_REVIEW";
        return new(true, readiness, reason, true, false);
    }

    private static bool Valid(string? value) => !string.IsNullOrWhiteSpace(value) && StringComparer.Ordinal.Equals(value, value.Trim());
}
