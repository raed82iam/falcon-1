namespace Falcon.FSATS.ResourceManagement.Application;

public enum ResourceOperationalHealthCondition
{
    Healthy,
    DegradedSafe,
    ReconciliationRequired,
    Contained,
    NotReady,
    Unknown
}

public enum ResourceHealthEvidenceIntegrity
{
    Valid,
    Invalid,
    Unknown
}

public sealed record ResourceOperationalHealthSnapshot(
    string ApplicationId,
    long SourceCoordinatorEpoch,
    long CurrentCoordinatorEpoch,
    string FoundationEnvelopeReference,
    DateTimeOffset ObservedAtUtc,
    DateTimeOffset ValidUntilUtc,
    string EvidenceId,
    ResourceHealthEvidenceIntegrity EvidenceIntegrity,
    bool ReferenceClaimsFoundationGrant,
    bool HasPendingResourceOutcome,
    bool HasUnresolvedFoundationOutcome,
    bool ResourcePressureActive,
    bool MinimumSafetyFloorPreserved,
    bool RestartReconstructionComplete,
    bool LifecycleTransitionBlocked);

public sealed record ResourceOperationalHealthAssessment(
    bool Accepted,
    ResourceOperationalHealthCondition Condition,
    string ReasonCode,
    bool CurrentEvidence,
    bool RequiresReconciliation,
    bool InternalCoordinationEligibleByHealthOnly,
    bool PreservesFoundationAuthorityBoundary,
    bool GrantsRuntimeAuthority)
{
    public static ResourceOperationalHealthAssessment Reject(string reason) =>
        new(false, ResourceOperationalHealthCondition.NotReady, reason, false, true, false, true, false);
}

public static class ResourceOperationalHealth
{
    public const string ApplicationId = "APP-RSC";

    public static ResourceOperationalHealthAssessment Assess(
        ResourceOperationalHealthSnapshot? snapshot,
        DateTimeOffset authoritativeNowUtc)
    {
        if (snapshot is null)
            return ResourceOperationalHealthAssessment.Reject("APP_RSC_HEALTH_INPUT_REQUIRED");

        if (!Enum.IsDefined(snapshot.EvidenceIntegrity))
            return ResourceOperationalHealthAssessment.Reject("APP_RSC_HEALTH_EVIDENCE_INTEGRITY_ENUM_INVALID");

        if (!Valid(snapshot.ApplicationId) ||
            !Valid(snapshot.FoundationEnvelopeReference) ||
            !Valid(snapshot.EvidenceId))
            return ResourceOperationalHealthAssessment.Reject("APP_RSC_HEALTH_IDENTITY_OR_EVIDENCE_INCOMPLETE");

        if (!StringComparer.Ordinal.Equals(snapshot.ApplicationId, ApplicationId))
            return ResourceOperationalHealthAssessment.Reject("APP_RSC_HEALTH_APPLICATION_ID_MISMATCH");

        if (snapshot.SourceCoordinatorEpoch < 0 || snapshot.CurrentCoordinatorEpoch < 0)
            return ResourceOperationalHealthAssessment.Reject("APP_RSC_HEALTH_COORDINATOR_EPOCH_INVALID");

        if (snapshot.SourceCoordinatorEpoch != snapshot.CurrentCoordinatorEpoch)
            return ResourceOperationalHealthAssessment.Reject("APP_RSC_HEALTH_STALE_COORDINATOR_EPOCH");

        if (snapshot.ReferenceClaimsFoundationGrant)
            return ResourceOperationalHealthAssessment.Reject("APP_RSC_HEALTH_FOUNDATION_AUTHORITY_MINTING_PROHIBITED");

        if (snapshot.EvidenceIntegrity != ResourceHealthEvidenceIntegrity.Valid)
            return ResourceOperationalHealthAssessment.Reject("APP_RSC_HEALTH_EVIDENCE_INTEGRITY_NOT_VALID");

        if (!TemporalEvidenceValid(snapshot.ObservedAtUtc, snapshot.ValidUntilUtc, authoritativeNowUtc, out var temporalReason))
            return ResourceOperationalHealthAssessment.Reject(temporalReason);

        if (!snapshot.RestartReconstructionComplete)
            return new(true, ResourceOperationalHealthCondition.NotReady, "APP_RSC_HEALTH_RESTART_RECONSTRUCTION_INCOMPLETE", true, true, false, true, false);

        if (snapshot.LifecycleTransitionBlocked)
            return new(true, ResourceOperationalHealthCondition.NotReady, "APP_RSC_HEALTH_LIFECYCLE_TRANSITION_BLOCKED", true, true, false, true, false);

        if (snapshot.HasPendingResourceOutcome || snapshot.HasUnresolvedFoundationOutcome)
            return new(true, ResourceOperationalHealthCondition.ReconciliationRequired, "APP_RSC_HEALTH_FOUNDATION_RESOURCE_OUTCOME_RECONCILIATION_REQUIRED", true, true, false, true, false);

        if (snapshot.ResourcePressureActive && !snapshot.MinimumSafetyFloorPreserved)
            return new(true, ResourceOperationalHealthCondition.NotReady, "APP_RSC_HEALTH_RESOURCE_PRESSURE_BELOW_SAFETY_FLOOR", true, false, false, true, false);

        if (snapshot.ResourcePressureActive)
            return new(true, ResourceOperationalHealthCondition.DegradedSafe, "APP_RSC_HEALTH_RESOURCE_PRESSURE_WITH_SAFETY_FLOOR_PRESERVED", true, false, true, true, false);

        if (!snapshot.MinimumSafetyFloorPreserved)
            return new(true, ResourceOperationalHealthCondition.NotReady, "APP_RSC_HEALTH_MINIMUM_SAFETY_FLOOR_NOT_PRESERVED", true, false, false, true, false);

        return new(true, ResourceOperationalHealthCondition.Healthy, "APP_RSC_HEALTH_CURRENT_AND_RECONCILED", true, false, true, true, false);
    }

    private static bool TemporalEvidenceValid(
        DateTimeOffset observedAtUtc,
        DateTimeOffset validUntilUtc,
        DateTimeOffset authoritativeNowUtc,
        out string reason)
    {
        if (validUntilUtc < observedAtUtc)
        {
            reason = "APP_RSC_HEALTH_TEMPORAL_INTERVAL_INVALID";
            return false;
        }

        if (observedAtUtc > authoritativeNowUtc)
        {
            reason = "APP_RSC_HEALTH_OBSERVATION_FROM_FUTURE";
            return false;
        }

        if (validUntilUtc < authoritativeNowUtc)
        {
            reason = "APP_RSC_HEALTH_OBSERVATION_EXPIRED";
            return false;
        }

        reason = string.Empty;
        return true;
    }

    private static bool Valid(string? value) =>
        !string.IsNullOrWhiteSpace(value) && StringComparer.Ordinal.Equals(value, value.Trim());
}
