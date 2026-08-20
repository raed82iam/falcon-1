using Falcon.FSATS.Trading.Domain;

namespace Falcon.FSATS.Trading.Application;

public enum BrokerAccountEvidenceSource
{
    BrokerApiConfirmed,
    UserReported,
    ScreenshotObserved,
    LastConfirmedState,
    Unknown
}

public enum BrokerConnectivityState { Available, Unavailable, Ambiguous }
public enum BrokerSubmissionTruth { NotSubmitted, SubmittedKnown, SubmittedOutcomeUnknown, Reconciled }
public enum BrokerReconciliationDimension
{
    BalanceAndBuyingPower,
    Positions,
    WorkingOrders,
    Fills,
    ProtectionOrders,
    CapitalReservations,
    AmbiguousSubmissions
}

public sealed record BrokerReconciliationDimensionEvidence(
    BrokerReconciliationDimension Dimension,
    bool Complete,
    string EvidenceReference,
    DateTimeOffset ObservedAt)
{
    public bool IsValid => Complete && !string.IsNullOrWhiteSpace(EvidenceReference) && ObservedAt != default;
}

public sealed record BrokerAccountObservation(
    BrokerAccountContext Account,
    PositionId? PositionId,
    Quantity? DisplayedQuantity,
    bool? WorkingOrPendingOrderVisible,
    bool? ProtectionVisible,
    BrokerAccountEvidenceSource EvidenceSource,
    string EvidenceReference,
    DateTimeOffset ObservedAt);

public sealed record BrokerAccountReconciliationEvidence(
    BrokerAccountContext Account,
    bool BalanceAndBuyingPowerReconciled,
    bool PositionsReconciled,
    bool WorkingOrdersReconciled,
    bool FillsReconciled,
    bool ProtectionOrdersReconciled,
    bool CapitalReservationsReconciled,
    bool AmbiguousSubmissionsReconciled,
    string EvidenceReference,
    DateTimeOffset ObservedAt,
    IReadOnlyList<BrokerReconciliationDimensionEvidence>? DimensionEvidence = null)
{
    private static readonly BrokerReconciliationDimension[] RequiredDimensions = Enum.GetValues<BrokerReconciliationDimension>();

    public bool HasCompleteDimensionEvidence
    {
        get
        {
            if (DimensionEvidence is null || DimensionEvidence.Count != RequiredDimensions.Length) return false;
            if (DimensionEvidence.GroupBy(x => x.Dimension).Any(g => g.Count() != 1)) return false;
            return RequiredDimensions.All(required => DimensionEvidence.Single(x => x.Dimension == required).IsValid);
        }
    }

    public bool IsComplete
        => Account is not null &&
           BalanceAndBuyingPowerReconciled &&
           PositionsReconciled &&
           WorkingOrdersReconciled &&
           FillsReconciled &&
           ProtectionOrdersReconciled &&
           CapitalReservationsReconciled &&
           AmbiguousSubmissionsReconciled &&
           !string.IsNullOrWhiteSpace(EvidenceReference) &&
           ObservedAt != default &&
           HasCompleteDimensionEvidence;
}

public sealed record GuidedRecoveryRequest(
    string RequestId,
    BrokerAccountContext Account,
    string RequiredBusinessFact,
    string ReasonCode,
    string CorrelationId);

public sealed record GuidedRecoveryAssessment(
    OperationalTruthState TruthState,
    OperationalRecoveryState RecoveryState,
    bool MayResumeRiskIncreasingAction,
    string ReasonCode);

public static class BrokerOutageRecoveryPolicy
{
    public static readonly TimeSpan DefaultMaximumEvidenceAge = TimeSpan.FromMinutes(2);
    public static readonly TimeSpan DefaultMaximumTemporalSkew = TimeSpan.FromSeconds(30);

    public static bool IsBrokerAuthoritative(BrokerAccountObservation observation)
        => IsObservationIdentityComplete(observation) && observation.EvidenceSource == BrokerAccountEvidenceSource.BrokerApiConfirmed;

    public static bool IsBrokerAuthoritative(BrokerAccountObservation observation, DateTimeOffset now, TimeSpan maximumEvidenceAge)
        => IsBrokerAuthoritative(observation) && IsCurrentEvidenceTime(observation.ObservedAt, now, maximumEvidenceAge);

    public static bool IsSafeToBlindRetry(BrokerSubmissionTruth submissionTruth, BrokerAccountReconciliationEvidence? reconciliation = null)
        => submissionTruth == BrokerSubmissionTruth.NotSubmitted ||
           (submissionTruth == BrokerSubmissionTruth.Reconciled && reconciliation is not null &&
            IsFreshCompleteReconciliationFor(reconciliation.Account, reconciliation, DateTimeOffset.UtcNow));

    public static bool IsObservationIdentityComplete(BrokerAccountObservation? observation)
        => observation is not null &&
           observation.Account is not null &&
           !string.IsNullOrWhiteSpace(observation.Account.BrokerId) &&
           !string.IsNullOrWhiteSpace(observation.Account.BrokerAccountId) &&
           !string.IsNullOrWhiteSpace(observation.Account.Environment) &&
           !string.IsNullOrWhiteSpace(observation.EvidenceReference) &&
           observation.ObservedAt != default &&
           (observation.PositionId is null || !string.IsNullOrWhiteSpace(observation.PositionId.Value.Value));

    public static bool IsCompleteReconciliationFor(BrokerAccountContext account, BrokerAccountReconciliationEvidence? reconciliation)
        => IsFreshCompleteReconciliationFor(account, reconciliation, DateTimeOffset.UtcNow);

    public static bool IsStructurallyCompleteReconciliationFor(BrokerAccountContext account, BrokerAccountReconciliationEvidence? reconciliation)
        => account is not null && reconciliation is { IsComplete: true } && reconciliation.Account == account;

    public static bool IsFreshCompleteReconciliationFor(
        BrokerAccountContext account,
        BrokerAccountReconciliationEvidence? reconciliation,
        DateTimeOffset now,
        TimeSpan? maximumEvidenceAge = null,
        TimeSpan? maximumTemporalSkew = null)
    {
        var maxAge = maximumEvidenceAge ?? DefaultMaximumEvidenceAge;
        var maxSkew = maximumTemporalSkew ?? DefaultMaximumTemporalSkew;
        ValidateEvidenceWindow(now, maxAge, maxSkew);
        if (!IsStructurallyCompleteReconciliationFor(account, reconciliation) || reconciliation is null) return false;
        if (!IsCurrentEvidenceTime(reconciliation.ObservedAt, now, maxAge)) return false;
        if (reconciliation.DimensionEvidence is null) return false;

        foreach (var dimension in reconciliation.DimensionEvidence)
        {
            if (!dimension.IsValid || !IsCurrentEvidenceTime(dimension.ObservedAt, now, maxAge)) return false;
            if (AbsoluteDifference(dimension.ObservedAt, reconciliation.ObservedAt) > maxSkew) return false;
        }
        return true;
    }

    public static GuidedRecoveryAssessment Assess(
        BrokerConnectivityState connectivity,
        BrokerSubmissionTruth submissionTruth,
        BrokerAccountObservation? observation,
        BrokerAccountReconciliationEvidence? reconciliation = null)
        => Assess(connectivity, submissionTruth, observation, reconciliation, DateTimeOffset.UtcNow, DefaultMaximumEvidenceAge, DefaultMaximumTemporalSkew);

    public static GuidedRecoveryAssessment Assess(
        BrokerConnectivityState connectivity,
        BrokerSubmissionTruth submissionTruth,
        BrokerAccountObservation? observation,
        BrokerAccountReconciliationEvidence? reconciliation,
        DateTimeOffset now,
        TimeSpan maximumEvidenceAge,
        TimeSpan maximumTemporalSkew)
    {
        ValidateEvidenceWindow(now, maximumEvidenceAge, maximumTemporalSkew);
        if (submissionTruth == BrokerSubmissionTruth.SubmittedOutcomeUnknown)
            return new(OperationalTruthState.ReconciliationRequired, OperationalRecoveryState.AwaitingBrokerReconciliation, false, "UNKNOWN_SUBMISSION_OUTCOME_NO_BLIND_RETRY");

        if (observation is not null && !IsObservationIdentityComplete(observation))
            return new(OperationalTruthState.Unknown, OperationalRecoveryState.HumanAssisted, false, "BROKER_OBSERVATION_IDENTITY_INCOMPLETE");

        if (connectivity == BrokerConnectivityState.Available && submissionTruth == BrokerSubmissionTruth.Reconciled)
        {
            if (observation is null || !IsBrokerAuthoritative(observation))
                return new(OperationalTruthState.ReconciliationRequired, OperationalRecoveryState.AwaitingBrokerReconciliation, false, "BROKER_ACCOUNT_OBSERVATION_NOT_AUTHORITATIVE");
            if (!IsCurrentEvidenceTime(observation.ObservedAt, now, maximumEvidenceAge))
                return new(OperationalTruthState.LastConfirmedStale, OperationalRecoveryState.AwaitingBrokerReconciliation, false, "BROKER_ACCOUNT_OBSERVATION_STALE_OR_CLOCK_INVALID");
            if (!IsFreshCompleteReconciliationFor(observation.Account, reconciliation, now, maximumEvidenceAge, maximumTemporalSkew))
                return new(OperationalTruthState.ReconciliationRequired, OperationalRecoveryState.AwaitingBrokerReconciliation, false, "BROKER_ACCOUNT_RECONCILIATION_INCOMPLETE_STALE_OR_EVIDENCE_UNBOUND");
            if (reconciliation is null || AbsoluteDifference(observation.ObservedAt, reconciliation.ObservedAt) > maximumTemporalSkew)
                return new(OperationalTruthState.ReconciliationRequired, OperationalRecoveryState.AwaitingBrokerReconciliation, false, "BROKER_OBSERVATION_RECONCILIATION_TEMPORAL_MISMATCH");
            return new(OperationalTruthState.BrokerConfirmed, OperationalRecoveryState.Recovered, true, "BROKER_ACCOUNT_FRESH_COHERENT_RECONCILIATION_CONFIRMED");
        }

        if (observation is null)
            return new(OperationalTruthState.Unknown, OperationalRecoveryState.HumanAssisted, false, "BROKER_ACCOUNT_TRUTH_UNAVAILABLE_REQUEST_GUIDED_FACT");

        if (!IsCurrentEvidenceTime(observation.ObservedAt, now, maximumEvidenceAge))
            return new(OperationalTruthState.LastConfirmedStale, OperationalRecoveryState.AwaitingBrokerReconciliation, false, "BROKER_OBSERVATION_STALE_OR_CLOCK_INVALID");

        return observation.EvidenceSource switch
        {
            BrokerAccountEvidenceSource.UserReported => new(OperationalTruthState.UserReported, OperationalRecoveryState.HumanAssisted, false, "HUMAN_REPORTED_NOT_BROKER_CONFIRMED"),
            BrokerAccountEvidenceSource.ScreenshotObserved => new(OperationalTruthState.ScreenshotObserved, OperationalRecoveryState.HumanAssisted, false, "SCREENSHOT_OBSERVED_NOT_BROKER_CONFIRMED"),
            BrokerAccountEvidenceSource.LastConfirmedState => new(OperationalTruthState.LastConfirmedStale, OperationalRecoveryState.AwaitingBrokerReconciliation, false, "LAST_CONFIRMED_STATE_IS_STALE"),
            BrokerAccountEvidenceSource.BrokerApiConfirmed when connectivity != BrokerConnectivityState.Available => new(OperationalTruthState.LastConfirmedStale, OperationalRecoveryState.AwaitingBrokerReconciliation, false, "BROKER_CONFIRMATION_NOT_CURRENT_AFTER_OUTAGE"),
            _ => new(OperationalTruthState.Unknown, OperationalRecoveryState.HumanAssisted, false, "BROKER_ACCOUNT_TRUTH_UNKNOWN")
        };
    }

    public static bool IsCurrentEvidenceTime(DateTimeOffset observedAt, DateTimeOffset now, TimeSpan maximumEvidenceAge)
    {
        if (observedAt == default || now == default || maximumEvidenceAge < TimeSpan.Zero) return false;
        if (observedAt > now) return false;
        return now - observedAt <= maximumEvidenceAge;
    }

    public static GuidedRecoveryRequest RequestPositionOpenFact(string requestId, BrokerAccountContext account, PositionId positionId, string correlationId)
    {
        Require(requestId, nameof(requestId));
        ArgumentNullException.ThrowIfNull(account);
        Require(positionId.Value, nameof(positionId));
        Require(correlationId, nameof(correlationId));
        return new(requestId.Trim(), account, $"CONFIRM_POSITION_OPEN:{positionId.Value.Trim()}", "BROKER_API_UNAVAILABLE_NEED_ACCOUNT_VISIBLE_FACT", correlationId.Trim());
    }

    public static IReadOnlyList<string> ManualRecoverySequence { get; } = Array.AsReadOnly(new[]
    {
        "VERIFY_BROKER_DISPLAYED_BALANCE_AND_BUYING_POWER",
        "VERIFY_BROKER_DISPLAYED_POSITION_QUANTITY",
        "VERIFY_ALL_WORKING_OR_PENDING_ORDERS",
        "VERIFY_FILLS_AND_PARTIAL_FILLS",
        "VERIFY_PROTECTION_ORDERS",
        "IDENTIFY_ANY_UNRESOLVED_PRIOR_FALCON_SUBMISSION",
        "DO_NOT_DUPLICATE_UNKNOWN_SUBMISSION",
        "RECONCILE_CAPITAL_RESERVATIONS_TO_BROKER_TRUTH",
        "ONLY_IF_NO_CONFLICT_AND_MANUAL_ACTION_IS_GOVERNED_HUMAN_MAY_ACT_THROUGH_BROKER",
        "CAPTURE_HUMAN_REPORTED_RESULT_AS_NON_BROKER_TRUTH",
        "VERIFY_DISPLAYED_ZERO_EXPOSURE_IF_APPLICABLE",
        "VERIFY_NO_ORPHANED_WORKING_OR_PROTECTION_ORDER",
        "REQUIRE_FINAL_COMPLETE_BROKER_ACCOUNT_RECONCILIATION_WHEN_API_RETURNS"
    });

    private static void ValidateEvidenceWindow(DateTimeOffset now, TimeSpan maximumEvidenceAge, TimeSpan maximumTemporalSkew)
    {
        if (now == default) throw new ArgumentException("BROKER_RECOVERY_NOW_REQUIRED", nameof(now));
        if (maximumEvidenceAge < TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(maximumEvidenceAge));
        if (maximumTemporalSkew < TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(maximumTemporalSkew));
    }

    private static TimeSpan AbsoluteDifference(DateTimeOffset left, DateTimeOffset right)
    {
        var delta = left - right;
        return delta < TimeSpan.Zero ? -delta : delta;
    }

    private static void Require(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("GUIDED_RECOVERY_IDENTITY_REQUIRED", parameterName);
    }
}
