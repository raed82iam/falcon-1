namespace Falcon.FSATS.FSTSimA.Application;

public enum SimulationLifecycleTransitionKind { Update, Rollback, Replacement, Removal }
public enum SimulationLifecycleCompatibility { CompatibleAsIs, MigrationRequired, Incompatible, Unknown }
public enum SimulationLifecycleReadiness { ReadyForExternalLifecycleReview, MigrationRequired, MigrationValidated, RollbackEligible, RollbackBlocked, RemovalReady, Blocked }

public sealed record SimulationLifecycleTransition(string TransitionId, SimulationLifecycleTransitionKind Kind, string ApplicationId, string SourceVersion, string TargetVersion, string SourcePackageId, string TargetPackageId, string SourceSchema, string TargetSchema, long SourceTrustEpoch, long CurrentTrustEpoch, string EvidenceId, bool MigrationEvidenceValidated, SimulationLifecycleCompatibility Compatibility);
public sealed record SimulationLifecycleSafetyState(string SimulationRunId, string EvidenceSetId, bool RunCommitted, bool RunInterrupted, bool ContainsPartialCheckpoint, bool EvidenceIsReplayOrSynthetic, bool QualificationClaimed, bool HasPendingValidation, bool RequiredEvidenceRetained);
public sealed record SimulationLifecycleAssessment(bool Accepted, SimulationLifecycleReadiness Readiness, string ReasonCode, bool PreservesEvidenceClassification, bool GrantsRuntimeAuthority)
{
    public static SimulationLifecycleAssessment Reject(string reason, SimulationLifecycleReadiness readiness = SimulationLifecycleReadiness.Blocked) => new(false, readiness, reason, true, false);
}

public static class SimulationLifecycleEvolution
{
    public const string ApplicationId = "FSATS-FSTSIMA";

    public static SimulationLifecycleAssessment Assess(SimulationLifecycleTransition? transition, SimulationLifecycleSafetyState? safety)
    {
        if (transition is null || safety is null) return SimulationLifecycleAssessment.Reject("FSTSIMA_LIFECYCLE_INPUT_REQUIRED");
        if (!Enum.IsDefined(transition.Kind) || !Enum.IsDefined(transition.Compatibility)) return SimulationLifecycleAssessment.Reject("FSTSIMA_LIFECYCLE_ENUM_INVALID");
        if (!Valid(transition.TransitionId) || !Valid(transition.ApplicationId) || !Valid(transition.SourceVersion) || !Valid(transition.SourcePackageId) || !Valid(transition.SourceSchema) || !Valid(transition.EvidenceId)) return SimulationLifecycleAssessment.Reject("FSTSIMA_LIFECYCLE_IDENTITY_INCOMPLETE");
        if (!StringComparer.Ordinal.Equals(transition.ApplicationId, ApplicationId)) return SimulationLifecycleAssessment.Reject("FSTSIMA_LIFECYCLE_APPLICATION_ID_MISMATCH");
        if (transition.SourceTrustEpoch < 0 || transition.SourceTrustEpoch != transition.CurrentTrustEpoch) return SimulationLifecycleAssessment.Reject("FSTSIMA_LIFECYCLE_STALE_TRUST_EPOCH");
        if (!Valid(safety.SimulationRunId) || !Valid(safety.EvidenceSetId)) return SimulationLifecycleAssessment.Reject("FSTSIMA_LIFECYCLE_EVIDENCE_IDENTITY_INCOMPLETE");
        if (!safety.RequiredEvidenceRetained) return SimulationLifecycleAssessment.Reject("FSTSIMA_LIFECYCLE_REQUIRED_EVIDENCE_NOT_RETAINED");
        if (safety.QualificationClaimed && (!safety.RunCommitted || safety.RunInterrupted || safety.ContainsPartialCheckpoint || safety.EvidenceIsReplayOrSynthetic)) return SimulationLifecycleAssessment.Reject("FSTSIMA_LIFECYCLE_QUALIFICATION_TRUTH_INVALID");
        if (transition.Kind != SimulationLifecycleTransitionKind.Removal && (!Valid(transition.TargetVersion) || !Valid(transition.TargetPackageId) || !Valid(transition.TargetSchema))) return SimulationLifecycleAssessment.Reject("FSTSIMA_LIFECYCLE_TARGET_IDENTITY_INCOMPLETE");
        if (transition.Kind == SimulationLifecycleTransitionKind.Replacement && StringComparer.Ordinal.Equals(transition.SourcePackageId, transition.TargetPackageId)) return SimulationLifecycleAssessment.Reject("FSTSIMA_LIFECYCLE_REPLACEMENT_PACKAGE_IDENTITY_NOT_DISTINCT");
        if (transition.Compatibility is SimulationLifecycleCompatibility.Unknown or SimulationLifecycleCompatibility.Incompatible) return SimulationLifecycleAssessment.Reject(transition.Compatibility == SimulationLifecycleCompatibility.Unknown ? "FSTSIMA_LIFECYCLE_COMPATIBILITY_UNKNOWN" : "FSTSIMA_LIFECYCLE_INCOMPATIBLE");
        var incomplete = safety.RunInterrupted || safety.ContainsPartialCheckpoint || safety.HasPendingValidation || !safety.RunCommitted;
        if (transition.Kind == SimulationLifecycleTransitionKind.Removal)
        {
            if (incomplete) return SimulationLifecycleAssessment.Reject("FSTSIMA_LIFECYCLE_REMOVAL_BLOCKED_BY_INCOMPLETE_EVIDENCE");
            return new(true, SimulationLifecycleReadiness.RemovalReady, "FSTSIMA_LIFECYCLE_REMOVAL_RECONCILED", true, false);
        }
        if (transition.Kind == SimulationLifecycleTransitionKind.Rollback && incomplete) return SimulationLifecycleAssessment.Reject("FSTSIMA_LIFECYCLE_ROLLBACK_BLOCKED_BY_INCOMPLETE_EVIDENCE", SimulationLifecycleReadiness.RollbackBlocked);
        if (transition.Compatibility == SimulationLifecycleCompatibility.MigrationRequired && !transition.MigrationEvidenceValidated) return new(true, SimulationLifecycleReadiness.MigrationRequired, "FSTSIMA_LIFECYCLE_MIGRATION_REQUIRED", true, false);
        var readiness = transition.Kind == SimulationLifecycleTransitionKind.Rollback ? SimulationLifecycleReadiness.RollbackEligible : SimulationLifecycleReadiness.ReadyForExternalLifecycleReview;
        var reason = transition.Compatibility == SimulationLifecycleCompatibility.MigrationRequired ? "FSTSIMA_LIFECYCLE_MIGRATION_VALIDATED_READY_FOR_EXTERNAL_REVIEW" : "FSTSIMA_LIFECYCLE_READY_FOR_EXTERNAL_REVIEW";
        return new(true, readiness, reason, true, false);
    }

    private static bool Valid(string? value) => !string.IsNullOrWhiteSpace(value) && StringComparer.Ordinal.Equals(value, value.Trim());
}
