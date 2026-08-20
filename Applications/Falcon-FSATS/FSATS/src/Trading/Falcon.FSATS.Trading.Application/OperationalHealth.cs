namespace Falcon.FSATS.Trading.Application;

public enum TradingOperationalHealthCondition
{
    Healthy,
    DegradedSafe,
    ReconciliationRequired,
    Contained,
    NotReady,
    Unknown
}

public enum TradingHealthEvidenceIntegrity
{
    Valid,
    Invalid,
    Unknown
}

public sealed record TradingOperationalHealthSnapshot(
    string ApplicationId,
    string BrokerId,
    string BrokerAccountId,
    string Environment,
    DateTimeOffset ObservedAtUtc,
    DateTimeOffset ValidUntilUtc,
    string EvidenceId,
    TradingHealthEvidenceIntegrity EvidenceIntegrity,
    bool HasOpenExposure,
    bool HasQueuedOrLeasedWork,
    bool HasDispatchStartedWork,
    bool HasUnresolvedBrokerReconciliation,
    bool HasCapitalReservations,
    bool IsContained,
    bool HasCancellationTombstones,
    bool HasStaleExecutionAuthority,
    bool RestartReconstructionComplete,
    bool LifecycleTransitionBlocked);

public sealed record TradingOperationalHealthAssessment(
    bool Accepted,
    TradingOperationalHealthCondition Condition,
    string ReasonCode,
    bool CurrentEvidence,
    bool RequiresReconciliation,
    bool RiskIncreaseEligibleByHealthOnly,
    bool PreservesSafetyFences,
    bool GrantsRuntimeAuthority)
{
    public static TradingOperationalHealthAssessment Reject(string reason) =>
        new(false, TradingOperationalHealthCondition.NotReady, reason, false, true, false, true, false);
}

public static class TradingOperationalHealth
{
    public const string ApplicationId = "FSATS-TRADING";

    public static TradingOperationalHealthAssessment Assess(
        TradingOperationalHealthSnapshot? snapshot,
        DateTimeOffset authoritativeNowUtc)
    {
        if (snapshot is null)
            return TradingOperationalHealthAssessment.Reject("TRADING_HEALTH_INPUT_REQUIRED");

        if (!Enum.IsDefined(snapshot.EvidenceIntegrity))
            return TradingOperationalHealthAssessment.Reject("TRADING_HEALTH_EVIDENCE_INTEGRITY_ENUM_INVALID");

        if (!Valid(snapshot.ApplicationId) ||
            !Valid(snapshot.BrokerId) ||
            !Valid(snapshot.BrokerAccountId) ||
            !Valid(snapshot.Environment) ||
            !Valid(snapshot.EvidenceId))
            return TradingOperationalHealthAssessment.Reject("TRADING_HEALTH_IDENTITY_OR_EVIDENCE_INCOMPLETE");

        if (!StringComparer.Ordinal.Equals(snapshot.ApplicationId, ApplicationId))
            return TradingOperationalHealthAssessment.Reject("TRADING_HEALTH_APPLICATION_ID_MISMATCH");

        if (snapshot.EvidenceIntegrity != TradingHealthEvidenceIntegrity.Valid)
            return TradingOperationalHealthAssessment.Reject("TRADING_HEALTH_EVIDENCE_INTEGRITY_NOT_VALID");

        if (!TemporalEvidenceValid(snapshot.ObservedAtUtc, snapshot.ValidUntilUtc, authoritativeNowUtc, out var temporalReason))
            return TradingOperationalHealthAssessment.Reject(temporalReason);

        if (snapshot.HasStaleExecutionAuthority)
            return TradingOperationalHealthAssessment.Reject("TRADING_HEALTH_STALE_EXECUTION_AUTHORITY");

        if (!snapshot.RestartReconstructionComplete)
            return new(true, TradingOperationalHealthCondition.NotReady, "TRADING_HEALTH_RESTART_RECONSTRUCTION_INCOMPLETE", true, true, false, true, false);

        if (snapshot.LifecycleTransitionBlocked)
            return new(true, TradingOperationalHealthCondition.NotReady, "TRADING_HEALTH_LIFECYCLE_TRANSITION_BLOCKED", true, true, false, true, false);

        if (snapshot.IsContained)
            return new(true, TradingOperationalHealthCondition.Contained, "TRADING_HEALTH_CONTAINED", true, true, false, true, false);

        if (snapshot.HasDispatchStartedWork || snapshot.HasUnresolvedBrokerReconciliation)
            return new(true, TradingOperationalHealthCondition.ReconciliationRequired, "TRADING_HEALTH_BROKER_RECONCILIATION_REQUIRED", true, true, false, true, false);

        var boundedObligations = snapshot.HasOpenExposure ||
                                 snapshot.HasQueuedOrLeasedWork ||
                                 snapshot.HasCapitalReservations ||
                                 snapshot.HasCancellationTombstones;

        if (boundedObligations)
            return new(true, TradingOperationalHealthCondition.DegradedSafe, "TRADING_HEALTH_ACTIVE_OBLIGATIONS_REQUIRE_BOUNDED_CONTINUITY", true, false, false, true, false);

        return new(true, TradingOperationalHealthCondition.Healthy, "TRADING_HEALTH_CURRENT_AND_RECONCILED", true, false, true, true, false);
    }

    private static bool TemporalEvidenceValid(
        DateTimeOffset observedAtUtc,
        DateTimeOffset validUntilUtc,
        DateTimeOffset authoritativeNowUtc,
        out string reason)
    {
        if (validUntilUtc < observedAtUtc)
        {
            reason = "TRADING_HEALTH_TEMPORAL_INTERVAL_INVALID";
            return false;
        }

        if (observedAtUtc > authoritativeNowUtc)
        {
            reason = "TRADING_HEALTH_OBSERVATION_FROM_FUTURE";
            return false;
        }

        if (validUntilUtc < authoritativeNowUtc)
        {
            reason = "TRADING_HEALTH_OBSERVATION_EXPIRED";
            return false;
        }

        reason = string.Empty;
        return true;
    }

    private static bool Valid(string? value) =>
        !string.IsNullOrWhiteSpace(value) && StringComparer.Ordinal.Equals(value, value.Trim());
}
