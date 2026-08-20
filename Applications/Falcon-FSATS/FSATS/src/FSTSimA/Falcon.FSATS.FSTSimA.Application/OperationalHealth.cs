namespace Falcon.FSATS.FSTSimA.Application;

public enum SimulationOperationalHealthCondition
{
    Healthy,
    DegradedSafe,
    ReconciliationRequired,
    Contained,
    NotReady,
    Unknown
}

public enum SimulationHealthEvidenceIntegrity
{
    Valid,
    Invalid,
    Unknown
}

public sealed record SimulationOperationalHealthSnapshot(
    string ApplicationId,
    string SimulationRunId,
    string EvidenceId,
    DateTimeOffset ObservedAtUtc,
    DateTimeOffset ValidUntilUtc,
    SimulationHealthEvidenceIntegrity EvidenceIntegrity,
    bool RunCommitted,
    bool RunInterrupted,
    bool ContainsPartialCheckpoint,
    bool HasPendingValidation,
    bool EvidenceIsReplayOrSynthetic,
    bool QualificationClaimed,
    bool RestartReconstructionComplete,
    bool LifecycleTransitionBlocked);

public sealed record SimulationOperationalHealthAssessment(
    bool Accepted,
    SimulationOperationalHealthCondition Condition,
    string ReasonCode,
    bool CurrentEvidence,
    bool RequiresReconciliation,
    bool QualificationEvidenceUsableByHealthOnly,
    bool PreservesEvidenceClassification,
    bool GrantsRuntimeAuthority)
{
    public static SimulationOperationalHealthAssessment Reject(string reason) =>
        new(false, SimulationOperationalHealthCondition.NotReady, reason, false, true, false, true, false);
}

public static class SimulationOperationalHealth
{
    public const string ApplicationId = "FSATS-FSTSIMA";

    public static SimulationOperationalHealthAssessment Assess(
        SimulationOperationalHealthSnapshot? snapshot,
        DateTimeOffset authoritativeNowUtc)
    {
        if (snapshot is null)
            return SimulationOperationalHealthAssessment.Reject("FSTSIMA_HEALTH_INPUT_REQUIRED");

        if (!Enum.IsDefined(snapshot.EvidenceIntegrity))
            return SimulationOperationalHealthAssessment.Reject("FSTSIMA_HEALTH_EVIDENCE_INTEGRITY_ENUM_INVALID");

        if (!Valid(snapshot.ApplicationId) ||
            !Valid(snapshot.SimulationRunId) ||
            !Valid(snapshot.EvidenceId))
            return SimulationOperationalHealthAssessment.Reject("FSTSIMA_HEALTH_IDENTITY_OR_EVIDENCE_INCOMPLETE");

        if (!StringComparer.Ordinal.Equals(snapshot.ApplicationId, ApplicationId))
            return SimulationOperationalHealthAssessment.Reject("FSTSIMA_HEALTH_APPLICATION_ID_MISMATCH");

        if (snapshot.EvidenceIntegrity != SimulationHealthEvidenceIntegrity.Valid)
            return SimulationOperationalHealthAssessment.Reject("FSTSIMA_HEALTH_EVIDENCE_INTEGRITY_NOT_VALID");

        if (!TemporalEvidenceValid(snapshot.ObservedAtUtc, snapshot.ValidUntilUtc, authoritativeNowUtc, out var temporalReason))
            return SimulationOperationalHealthAssessment.Reject(temporalReason);

        if (snapshot.EvidenceIsReplayOrSynthetic && snapshot.QualificationClaimed)
            return SimulationOperationalHealthAssessment.Reject("FSTSIMA_HEALTH_SYNTHETIC_EVIDENCE_CANNOT_CLAIM_OPERATIONAL_QUALIFICATION");

        if (snapshot.QualificationClaimed && (!snapshot.RunCommitted || snapshot.RunInterrupted || snapshot.ContainsPartialCheckpoint || snapshot.HasPendingValidation))
            return SimulationOperationalHealthAssessment.Reject("FSTSIMA_HEALTH_INCOMPLETE_RUN_CANNOT_CLAIM_QUALIFICATION");

        if (!snapshot.RestartReconstructionComplete)
            return new(true, SimulationOperationalHealthCondition.NotReady, "FSTSIMA_HEALTH_RESTART_RECONSTRUCTION_INCOMPLETE", true, true, false, true, false);

        if (snapshot.LifecycleTransitionBlocked)
            return new(true, SimulationOperationalHealthCondition.NotReady, "FSTSIMA_HEALTH_LIFECYCLE_TRANSITION_BLOCKED", true, true, false, true, false);

        if (snapshot.RunInterrupted || snapshot.ContainsPartialCheckpoint || snapshot.HasPendingValidation)
            return new(true, SimulationOperationalHealthCondition.ReconciliationRequired, "FSTSIMA_HEALTH_RUN_OR_VALIDATION_RECONCILIATION_REQUIRED", true, true, false, true, false);

        if (!snapshot.RunCommitted)
            return new(true, SimulationOperationalHealthCondition.NotReady, "FSTSIMA_HEALTH_RUN_NOT_COMMITTED", true, false, false, true, false);

        if (snapshot.EvidenceIsReplayOrSynthetic)
            return new(true, SimulationOperationalHealthCondition.DegradedSafe, "FSTSIMA_HEALTH_REPLAY_OR_SYNTHETIC_EVIDENCE_CLASSIFIED", true, false, false, true, false);

        return new(true, SimulationOperationalHealthCondition.Healthy, "FSTSIMA_HEALTH_CURRENT_COMPLETE_AND_RECONCILED", true, false, snapshot.QualificationClaimed, true, false);
    }

    private static bool TemporalEvidenceValid(
        DateTimeOffset observedAtUtc,
        DateTimeOffset validUntilUtc,
        DateTimeOffset authoritativeNowUtc,
        out string reason)
    {
        if (validUntilUtc < observedAtUtc)
        {
            reason = "FSTSIMA_HEALTH_TEMPORAL_INTERVAL_INVALID";
            return false;
        }

        if (observedAtUtc > authoritativeNowUtc)
        {
            reason = "FSTSIMA_HEALTH_OBSERVATION_FROM_FUTURE";
            return false;
        }

        if (validUntilUtc < authoritativeNowUtc)
        {
            reason = "FSTSIMA_HEALTH_OBSERVATION_EXPIRED";
            return false;
        }

        reason = string.Empty;
        return true;
    }

    private static bool Valid(string? value) =>
        !string.IsNullOrWhiteSpace(value) && StringComparer.Ordinal.Equals(value, value.Trim());
}
