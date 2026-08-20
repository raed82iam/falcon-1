namespace Falcon.FSATS.Trading.Application;

public enum TradingLifecycleTransitionKind { Update, Rollback, Replacement, Removal }
public enum TradingLifecycleCompatibility { CompatibleAsIs, MigrationRequired, Incompatible, Unknown }
public enum TradingLifecycleReadiness { Proposed, Validated, MigrationRequired, MigrationValidated, ReadyForExternalLifecycleReview, Blocked, RollbackEligible, RollbackBlocked, RemovalReady }

public sealed record TradingLifecycleTransition(string TransitionId, TradingLifecycleTransitionKind Kind, string ApplicationId, string SourceVersion, string TargetVersion, string SourcePackageId, string TargetPackageId, string SourceSchema, string TargetSchema, long SourceTrustEpoch, long CurrentTrustEpoch, string EvidenceId, bool MigrationEvidenceValidated, TradingLifecycleCompatibility Compatibility);
public sealed record TradingLifecycleSafetyState(string BrokerId, string BrokerAccountId, string Environment, bool HasOpenExposure, bool HasQueuedOrLeasedWork, bool HasDispatchStartedWork, bool IsContained, bool HasCancellationTombstones, bool HasUnresolvedBrokerReconciliation, bool HasCapitalReservations, bool HasStaleExecutionPermit, bool RequiredEvidenceRetained);
public sealed record TradingLifecycleAssessment(bool Accepted, TradingLifecycleReadiness Readiness, string ReasonCode, bool RequiresMigration, bool RequiresReconciliation, bool PreservesCurrentSafetyFences, bool GrantsRuntimeAuthority)
{
    public static TradingLifecycleAssessment Reject(string reason, TradingLifecycleReadiness readiness = TradingLifecycleReadiness.Blocked) => new(false, readiness, reason, false, true, false, false);
}

public static class TradingLifecycleEvolution
{
    public const string ApplicationId = "FSATS-TRADING";

    public static TradingLifecycleAssessment Assess(TradingLifecycleTransition? transition, TradingLifecycleSafetyState? safety)
    {
        if (transition is null || safety is null) return TradingLifecycleAssessment.Reject("TRADING_LIFECYCLE_INPUT_REQUIRED");
        if (!Enum.IsDefined(transition.Kind) || !Enum.IsDefined(transition.Compatibility)) return TradingLifecycleAssessment.Reject("TRADING_LIFECYCLE_ENUM_INVALID");
        if (!Valid(transition.TransitionId) || !Valid(transition.ApplicationId) || !Valid(transition.SourceVersion) || !Valid(transition.SourcePackageId) || !Valid(transition.SourceSchema) || !Valid(transition.EvidenceId)) return TradingLifecycleAssessment.Reject("TRADING_LIFECYCLE_IDENTITY_INCOMPLETE");
        if (!StringComparer.Ordinal.Equals(transition.ApplicationId, ApplicationId)) return TradingLifecycleAssessment.Reject("TRADING_LIFECYCLE_APPLICATION_ID_MISMATCH");
        if (transition.SourceTrustEpoch < 0 || transition.CurrentTrustEpoch < 0 || transition.SourceTrustEpoch != transition.CurrentTrustEpoch) return TradingLifecycleAssessment.Reject("TRADING_LIFECYCLE_STALE_TRUST_EPOCH");
        if (!Valid(safety.BrokerId) || !Valid(safety.BrokerAccountId) || !Valid(safety.Environment)) return TradingLifecycleAssessment.Reject("TRADING_LIFECYCLE_BROKER_ACCOUNT_IDENTITY_INCOMPLETE");
        if (!safety.RequiredEvidenceRetained) return TradingLifecycleAssessment.Reject("TRADING_LIFECYCLE_REQUIRED_EVIDENCE_NOT_RETAINED");
        if (safety.HasStaleExecutionPermit) return TradingLifecycleAssessment.Reject("TRADING_LIFECYCLE_STALE_EXECUTION_PERMIT_PRESENT");
        if (transition.Kind != TradingLifecycleTransitionKind.Removal && (!Valid(transition.TargetVersion) || !Valid(transition.TargetPackageId) || !Valid(transition.TargetSchema))) return TradingLifecycleAssessment.Reject("TRADING_LIFECYCLE_TARGET_IDENTITY_INCOMPLETE");
        if (transition.Kind == TradingLifecycleTransitionKind.Replacement && StringComparer.Ordinal.Equals(transition.SourcePackageId, transition.TargetPackageId)) return TradingLifecycleAssessment.Reject("TRADING_LIFECYCLE_REPLACEMENT_PACKAGE_IDENTITY_NOT_DISTINCT");
        if (transition.Compatibility is TradingLifecycleCompatibility.Unknown or TradingLifecycleCompatibility.Incompatible) return TradingLifecycleAssessment.Reject(transition.Compatibility == TradingLifecycleCompatibility.Unknown ? "TRADING_LIFECYCLE_COMPATIBILITY_UNKNOWN" : "TRADING_LIFECYCLE_INCOMPATIBLE");

        var unresolvedExternal = safety.HasDispatchStartedWork || safety.HasUnresolvedBrokerReconciliation;
        var activeObligations = safety.HasOpenExposure || safety.HasQueuedOrLeasedWork || unresolvedExternal || safety.HasCapitalReservations;
        var safetyFences = safety.IsContained || safety.HasCancellationTombstones || unresolvedExternal;

        if (transition.Kind == TradingLifecycleTransitionKind.Removal)
        {
            if (activeObligations) return TradingLifecycleAssessment.Reject("TRADING_LIFECYCLE_REMOVAL_BLOCKED_BY_OPEN_OBLIGATIONS");
            return new(true, TradingLifecycleReadiness.RemovalReady, "TRADING_LIFECYCLE_REMOVAL_RECONCILED", false, false, true, false);
        }
        if (transition.Kind == TradingLifecycleTransitionKind.Rollback && safetyFences) return new(false, TradingLifecycleReadiness.RollbackBlocked, "TRADING_LIFECYCLE_ROLLBACK_BLOCKED_BY_CURRENT_SAFETY_FENCES", transition.Compatibility == TradingLifecycleCompatibility.MigrationRequired, unresolvedExternal, true, false);
        if (unresolvedExternal) return TradingLifecycleAssessment.Reject("TRADING_LIFECYCLE_EXTERNAL_RECONCILIATION_REQUIRED");
        if (transition.Compatibility == TradingLifecycleCompatibility.MigrationRequired && !transition.MigrationEvidenceValidated) return new(true, TradingLifecycleReadiness.MigrationRequired, "TRADING_LIFECYCLE_MIGRATION_REQUIRED", true, false, true, false);

        var readiness = transition.Kind == TradingLifecycleTransitionKind.Rollback ? TradingLifecycleReadiness.RollbackEligible : TradingLifecycleReadiness.ReadyForExternalLifecycleReview;
        var reason = transition.Compatibility == TradingLifecycleCompatibility.MigrationRequired ? "TRADING_LIFECYCLE_MIGRATION_VALIDATED_READY_FOR_EXTERNAL_REVIEW" : "TRADING_LIFECYCLE_READY_FOR_EXTERNAL_REVIEW";
        return new(true, readiness, reason, false, false, true, false);
    }

    private static bool Valid(string? value) => !string.IsNullOrWhiteSpace(value) && StringComparer.Ordinal.Equals(value, value.Trim());
}
