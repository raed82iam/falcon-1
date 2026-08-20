namespace Falcon.FSATS.FSAPMA.Application;

public enum ProviderOperationalHealthCondition
{
    Healthy,
    DegradedSafe,
    ReconciliationRequired,
    Contained,
    NotReady,
    Unknown
}

public enum ProviderHealthEvidenceIntegrity
{
    Valid,
    Invalid,
    Unknown
}

public sealed record ProviderOperationalHealthSnapshot(
    string ApplicationId,
    string ProviderId,
    string ProviderAccountId,
    string ServiceRole,
    string Environment,
    DateTimeOffset ObservedAtUtc,
    DateTimeOffset ValidUntilUtc,
    string EvidenceId,
    ProviderHealthEvidenceIntegrity EvidenceIntegrity,
    bool StreamGapDetected,
    bool StreamStale,
    bool DeliveryOutcomeUnknown,
    bool QuotaEntitlementKnown,
    bool QuotaPressureActive,
    bool ContainsSecretBytes,
    bool HasStaleProviderAuthority,
    bool RestartReconstructionComplete,
    bool LifecycleTransitionBlocked);

public sealed record ProviderOperationalHealthAssessment(
    bool Accepted,
    ProviderOperationalHealthCondition Condition,
    string ReasonCode,
    bool CurrentEvidence,
    bool RequiresReconciliation,
    bool OperationalDataEligibleByHealthOnly,
    bool PreservesProviderTruth,
    bool GrantsRuntimeAuthority)
{
    public static ProviderOperationalHealthAssessment Reject(string reason) =>
        new(false, ProviderOperationalHealthCondition.NotReady, reason, false, true, false, true, false);
}

public static class ProviderOperationalHealth
{
    public const string ApplicationId = "FSATS-FSAPMA";

    public static ProviderOperationalHealthAssessment Assess(
        ProviderOperationalHealthSnapshot? snapshot,
        DateTimeOffset authoritativeNowUtc)
    {
        if (snapshot is null)
            return ProviderOperationalHealthAssessment.Reject("FSAPMA_HEALTH_INPUT_REQUIRED");

        if (!Enum.IsDefined(snapshot.EvidenceIntegrity))
            return ProviderOperationalHealthAssessment.Reject("FSAPMA_HEALTH_EVIDENCE_INTEGRITY_ENUM_INVALID");

        if (!Valid(snapshot.ApplicationId) ||
            !Valid(snapshot.ProviderId) ||
            !Valid(snapshot.ProviderAccountId) ||
            !Valid(snapshot.ServiceRole) ||
            !Valid(snapshot.Environment) ||
            !Valid(snapshot.EvidenceId))
            return ProviderOperationalHealthAssessment.Reject("FSAPMA_HEALTH_IDENTITY_OR_EVIDENCE_INCOMPLETE");

        if (!StringComparer.Ordinal.Equals(snapshot.ApplicationId, ApplicationId))
            return ProviderOperationalHealthAssessment.Reject("FSAPMA_HEALTH_APPLICATION_ID_MISMATCH");

        if (snapshot.EvidenceIntegrity != ProviderHealthEvidenceIntegrity.Valid)
            return ProviderOperationalHealthAssessment.Reject("FSAPMA_HEALTH_EVIDENCE_INTEGRITY_NOT_VALID");

        if (snapshot.ContainsSecretBytes)
            return ProviderOperationalHealthAssessment.Reject("FSAPMA_HEALTH_SECRET_BYTES_PROHIBITED");

        if (!TemporalEvidenceValid(snapshot.ObservedAtUtc, snapshot.ValidUntilUtc, authoritativeNowUtc, out var temporalReason))
            return ProviderOperationalHealthAssessment.Reject(temporalReason);

        if (snapshot.HasStaleProviderAuthority)
            return ProviderOperationalHealthAssessment.Reject("FSAPMA_HEALTH_STALE_PROVIDER_AUTHORITY");

        if (!snapshot.RestartReconstructionComplete)
            return new(true, ProviderOperationalHealthCondition.NotReady, "FSAPMA_HEALTH_RESTART_RECONSTRUCTION_INCOMPLETE", true, true, false, true, false);

        if (snapshot.LifecycleTransitionBlocked)
            return new(true, ProviderOperationalHealthCondition.NotReady, "FSAPMA_HEALTH_LIFECYCLE_TRANSITION_BLOCKED", true, true, false, true, false);

        if (snapshot.StreamGapDetected || snapshot.StreamStale || snapshot.DeliveryOutcomeUnknown)
            return new(true, ProviderOperationalHealthCondition.ReconciliationRequired, "FSAPMA_HEALTH_STREAM_OR_DELIVERY_RECONCILIATION_REQUIRED", true, true, false, true, false);

        if (!snapshot.QuotaEntitlementKnown)
            return new(true, ProviderOperationalHealthCondition.NotReady, "FSAPMA_HEALTH_QUOTA_OR_ENTITLEMENT_UNKNOWN", true, false, false, true, false);

        if (snapshot.QuotaPressureActive)
            return new(true, ProviderOperationalHealthCondition.DegradedSafe, "FSAPMA_HEALTH_QUOTA_PRESSURE_ACTIVE", true, false, false, true, false);

        return new(true, ProviderOperationalHealthCondition.Healthy, "FSAPMA_HEALTH_CURRENT_AND_RECONCILED", true, false, true, true, false);
    }

    private static bool TemporalEvidenceValid(
        DateTimeOffset observedAtUtc,
        DateTimeOffset validUntilUtc,
        DateTimeOffset authoritativeNowUtc,
        out string reason)
    {
        if (validUntilUtc < observedAtUtc)
        {
            reason = "FSAPMA_HEALTH_TEMPORAL_INTERVAL_INVALID";
            return false;
        }

        if (observedAtUtc > authoritativeNowUtc)
        {
            reason = "FSAPMA_HEALTH_OBSERVATION_FROM_FUTURE";
            return false;
        }

        if (validUntilUtc < authoritativeNowUtc)
        {
            reason = "FSAPMA_HEALTH_OBSERVATION_EXPIRED";
            return false;
        }

        reason = string.Empty;
        return true;
    }

    private static bool Valid(string? value) =>
        !string.IsNullOrWhiteSpace(value) && StringComparer.Ordinal.Equals(value, value.Trim());
}
