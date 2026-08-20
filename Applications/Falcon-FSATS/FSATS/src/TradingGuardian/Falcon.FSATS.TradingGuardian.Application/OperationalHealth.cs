namespace Falcon.FSATS.TradingGuardian.Application;

public enum GuardianOperationalHealthCondition
{
    Healthy,
    DegradedSafe,
    ReconciliationRequired,
    Contained,
    NotReady,
    Unknown
}

public enum GuardianHealthEvidenceIntegrity
{
    Valid,
    Invalid,
    Unknown
}

public sealed record GuardianOperationalHealthSnapshot(
    string ApplicationId,
    string ProtectedTargetIdentity,
    string IncidentId,
    string CorrelationId,
    DateTimeOffset ObservedAtUtc,
    DateTimeOffset ValidUntilUtc,
    string EvidenceId,
    GuardianHealthEvidenceIntegrity EvidenceIntegrity,
    bool HasActiveContainmentOrRestriction,
    bool HasUnresolvedProtectionOutcome,
    bool RequiresCurrentProtectionTruthVerification,
    bool HasStaleProtectionAuthority,
    bool RestartReconstructionComplete,
    bool LifecycleTransitionBlocked);

public sealed record GuardianOperationalHealthAssessment(
    bool Accepted,
    GuardianOperationalHealthCondition Condition,
    string ReasonCode,
    bool CurrentEvidence,
    bool RequiresReconciliation,
    bool ProtectionTruthCurrentByHealthOnly,
    bool PreservesProtectionState,
    bool GrantsRuntimeAuthority)
{
    public static GuardianOperationalHealthAssessment Reject(string reason) =>
        new(false, GuardianOperationalHealthCondition.NotReady, reason, false, true, false, true, false);
}

public static class GuardianOperationalHealth
{
    public const string ApplicationId = "FSATS-TRADING-GUARDIAN";

    public static GuardianOperationalHealthAssessment Assess(
        GuardianOperationalHealthSnapshot? snapshot,
        DateTimeOffset authoritativeNowUtc)
    {
        if (snapshot is null)
            return GuardianOperationalHealthAssessment.Reject("GUARDIAN_HEALTH_INPUT_REQUIRED");

        if (!Enum.IsDefined(snapshot.EvidenceIntegrity))
            return GuardianOperationalHealthAssessment.Reject("GUARDIAN_HEALTH_EVIDENCE_INTEGRITY_ENUM_INVALID");

        if (!Valid(snapshot.ApplicationId) ||
            !Valid(snapshot.ProtectedTargetIdentity) ||
            !Valid(snapshot.IncidentId) ||
            !Valid(snapshot.CorrelationId) ||
            !Valid(snapshot.EvidenceId))
            return GuardianOperationalHealthAssessment.Reject("GUARDIAN_HEALTH_IDENTITY_OR_EVIDENCE_INCOMPLETE");

        if (!StringComparer.Ordinal.Equals(snapshot.ApplicationId, ApplicationId))
            return GuardianOperationalHealthAssessment.Reject("GUARDIAN_HEALTH_APPLICATION_ID_MISMATCH");

        if (snapshot.EvidenceIntegrity != GuardianHealthEvidenceIntegrity.Valid)
            return GuardianOperationalHealthAssessment.Reject("GUARDIAN_HEALTH_EVIDENCE_INTEGRITY_NOT_VALID");

        if (!TemporalEvidenceValid(snapshot.ObservedAtUtc, snapshot.ValidUntilUtc, authoritativeNowUtc, out var temporalReason))
            return GuardianOperationalHealthAssessment.Reject(temporalReason);

        if (snapshot.HasStaleProtectionAuthority)
            return GuardianOperationalHealthAssessment.Reject("GUARDIAN_HEALTH_STALE_PROTECTION_AUTHORITY");

        if (!snapshot.RestartReconstructionComplete)
            return new(true, GuardianOperationalHealthCondition.NotReady, "GUARDIAN_HEALTH_RESTART_RECONSTRUCTION_INCOMPLETE", true, true, false, true, false);

        if (snapshot.LifecycleTransitionBlocked)
            return new(true, GuardianOperationalHealthCondition.NotReady, "GUARDIAN_HEALTH_LIFECYCLE_TRANSITION_BLOCKED", true, true, false, true, false);

        if (snapshot.HasActiveContainmentOrRestriction)
            return new(true, GuardianOperationalHealthCondition.Contained, "GUARDIAN_HEALTH_ACTIVE_CONTAINMENT_OR_RESTRICTION", true, snapshot.HasUnresolvedProtectionOutcome || snapshot.RequiresCurrentProtectionTruthVerification, false, true, false);

        if (snapshot.HasUnresolvedProtectionOutcome || snapshot.RequiresCurrentProtectionTruthVerification)
            return new(true, GuardianOperationalHealthCondition.ReconciliationRequired, "GUARDIAN_HEALTH_CURRENT_PROTECTION_TRUTH_VERIFICATION_REQUIRED", true, true, false, true, false);

        return new(true, GuardianOperationalHealthCondition.Healthy, "GUARDIAN_HEALTH_CURRENT_AND_RECONCILED", true, false, true, true, false);
    }

    private static bool TemporalEvidenceValid(
        DateTimeOffset observedAtUtc,
        DateTimeOffset validUntilUtc,
        DateTimeOffset authoritativeNowUtc,
        out string reason)
    {
        if (validUntilUtc < observedAtUtc)
        {
            reason = "GUARDIAN_HEALTH_TEMPORAL_INTERVAL_INVALID";
            return false;
        }

        if (observedAtUtc > authoritativeNowUtc)
        {
            reason = "GUARDIAN_HEALTH_OBSERVATION_FROM_FUTURE";
            return false;
        }

        if (validUntilUtc < authoritativeNowUtc)
        {
            reason = "GUARDIAN_HEALTH_OBSERVATION_EXPIRED";
            return false;
        }

        reason = string.Empty;
        return true;
    }

    private static bool Valid(string? value) =>
        !string.IsNullOrWhiteSpace(value) && StringComparer.Ordinal.Equals(value, value.Trim());
}
