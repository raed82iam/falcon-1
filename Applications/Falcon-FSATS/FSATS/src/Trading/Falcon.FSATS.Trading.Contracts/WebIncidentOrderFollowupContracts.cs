namespace Falcon.FSATS.Trading.Contracts;

public enum WebIncidentOrderTruthState
{
    BrokerConfirmedWorking = 1,
    BrokerConfirmedRejected = 2,
    BrokerConfirmedPartiallyFilled = 3,
    BrokerConfirmedFilled = 4,
    BrokerConfirmedCancelled = 5,
    OutcomeUnknownOrAmbiguous = 6,
    ReconciliationRequired = 7
}

public static class WebIncidentOrderFollowupContractIds
{
    public const string AffectedOrderProjectionV1 = "FSATS.WebAffectedOrderFollowupProjection.v1";
    public const string AffectedOrderUpdateV1 = "FSATS.WebAffectedOrderFollowupUpdate.v1";
}

public sealed record WebAffectedOrderFollowupProjection
{
    public string ProjectionId { get; }
    public string IncidentId { get; }
    public BrokerAccountScope Account { get; }
    public TradingOrderRef Order { get; }
    public TradingInstrumentRef Instrument { get; }
    public DateTimeOffset? LastBrokerConfirmedAt { get; }
    public WebIncidentOrderTruthState OrderTruthState { get; }
    public WebCustomerFollowupRequirement FollowupRequirement { get; }
    public string FollowupReasonCode { get; }
    public IReadOnlyList<WebIncidentActionInstruction> OrderedActions { get; }
    public TruthClassification TruthState { get; }
    public WebFreshnessState FreshnessState { get; }
    public string EvidenceReference { get; }
    public DateTimeOffset AsOfTime { get; }

    public WebAffectedOrderFollowupProjection(
        string projectionId,
        string incidentId,
        BrokerAccountScope account,
        TradingOrderRef order,
        TradingInstrumentRef instrument,
        DateTimeOffset? lastBrokerConfirmedAt,
        WebIncidentOrderTruthState orderTruthState,
        WebCustomerFollowupRequirement followupRequirement,
        string followupReasonCode,
        IReadOnlyList<WebIncidentActionInstruction> orderedActions,
        TruthClassification truthState,
        WebFreshnessState freshnessState,
        string evidenceReference,
        DateTimeOffset asOfTime)
    {
        if (string.IsNullOrWhiteSpace(projectionId)) throw new ArgumentException("INCIDENT_ORDER_PROJECTION_ID_REQUIRED", nameof(projectionId));
        if (string.IsNullOrWhiteSpace(incidentId)) throw new ArgumentException("INCIDENT_ID_REQUIRED", nameof(incidentId));
        ArgumentNullException.ThrowIfNull(account);
        if (string.IsNullOrWhiteSpace(order.Value)) throw new ArgumentException("INCIDENT_ORDER_ID_REQUIRED", nameof(order));
        if (string.IsNullOrWhiteSpace(instrument.Value)) throw new ArgumentException("INCIDENT_ORDER_INSTRUMENT_REQUIRED", nameof(instrument));
        if (asOfTime == default || lastBrokerConfirmedAt > asOfTime) throw new ArgumentException("INVALID_INCIDENT_ORDER_TIME_ORDER");
        if (!Enum.IsDefined(orderTruthState) || !Enum.IsDefined(followupRequirement)) throw new ArgumentException("INVALID_INCIDENT_ORDER_ENUM");
        if (orderTruthState is WebIncidentOrderTruthState.OutcomeUnknownOrAmbiguous or WebIncidentOrderTruthState.ReconciliationRequired
            && followupRequirement == WebCustomerFollowupRequirement.None)
            throw new ArgumentException("UNRESOLVED_ORDER_TRUTH_REQUIRES_FOLLOWUP");
        if (orderTruthState is WebIncidentOrderTruthState.BrokerConfirmedWorking
            or WebIncidentOrderTruthState.BrokerConfirmedRejected
            or WebIncidentOrderTruthState.BrokerConfirmedPartiallyFilled
            or WebIncidentOrderTruthState.BrokerConfirmedFilled
            or WebIncidentOrderTruthState.BrokerConfirmedCancelled)
        {
            if (lastBrokerConfirmedAt is null || truthState != TruthClassification.Current || freshnessState != WebFreshnessState.Current)
                throw new ArgumentException("BROKER_CONFIRMED_ORDER_STATE_REQUIRES_CURRENT_BROKER_TRUTH");
        }
        if (string.IsNullOrWhiteSpace(followupReasonCode)) throw new ArgumentException("INCIDENT_ORDER_FOLLOWUP_REASON_REQUIRED", nameof(followupReasonCode));
        ArgumentNullException.ThrowIfNull(orderedActions);
        if (orderedActions.GroupBy(x => x.Sequence).Any(x => x.Count() != 1) || !orderedActions.Select(x => x.Sequence).SequenceEqual(orderedActions.Select(x => x.Sequence).OrderBy(x => x)))
            throw new ArgumentException("INCIDENT_ORDER_ACTIONS_MUST_HAVE_UNIQUE_ASCENDING_SEQUENCE", nameof(orderedActions));
        if (followupRequirement == WebCustomerFollowupRequirement.Required && !orderedActions.Any(x => x.Required))
            throw new ArgumentException("REQUIRED_ORDER_FOLLOWUP_REQUIRES_REQUIRED_ACTION", nameof(orderedActions));
        if (followupRequirement == WebCustomerFollowupRequirement.None && orderedActions.Any(x => x.Required))
            throw new ArgumentException("NO_ORDER_FOLLOWUP_CANNOT_CONTAIN_REQUIRED_ACTION", nameof(orderedActions));
        if (string.IsNullOrWhiteSpace(evidenceReference)) throw new ArgumentException("INCIDENT_ORDER_EVIDENCE_REQUIRED", nameof(evidenceReference));

        ProjectionId = projectionId.Trim();
        IncidentId = incidentId.Trim();
        Account = account;
        Order = new TradingOrderRef(order.Value.Trim());
        Instrument = new TradingInstrumentRef(instrument.Value.Trim());
        LastBrokerConfirmedAt = lastBrokerConfirmedAt;
        OrderTruthState = orderTruthState;
        FollowupRequirement = followupRequirement;
        FollowupReasonCode = followupReasonCode.Trim();
        OrderedActions = Array.AsReadOnly(orderedActions.ToArray());
        TruthState = truthState;
        FreshnessState = freshnessState;
        EvidenceReference = evidenceReference.Trim();
        AsOfTime = asOfTime;
    }
}

public sealed record WebAffectedOrderFollowupUpdate
{
    public string UpdateId { get; }
    public long UpdateSequence { get; }
    public string IncidentId { get; }
    public BrokerAccountScope Account { get; }
    public IReadOnlyList<WebAffectedOrderFollowupProjection> AffectedOrders { get; }
    public DateTimeOffset EffectiveAt { get; }
    public string EvidenceReference { get; }
    public string ReasonCode { get; }

    public WebAffectedOrderFollowupUpdate(
        string updateId,
        long updateSequence,
        string incidentId,
        BrokerAccountScope account,
        IReadOnlyList<WebAffectedOrderFollowupProjection> affectedOrders,
        DateTimeOffset effectiveAt,
        string evidenceReference,
        string reasonCode)
    {
        if (string.IsNullOrWhiteSpace(updateId)) throw new ArgumentException("INCIDENT_ORDER_UPDATE_ID_REQUIRED", nameof(updateId));
        if (updateSequence <= 0) throw new ArgumentOutOfRangeException(nameof(updateSequence));
        if (string.IsNullOrWhiteSpace(incidentId)) throw new ArgumentException("INCIDENT_ID_REQUIRED", nameof(incidentId));
        ArgumentNullException.ThrowIfNull(account);
        ArgumentNullException.ThrowIfNull(affectedOrders);
        if (affectedOrders.Any(x => x.Account != account || !StringComparer.Ordinal.Equals(x.IncidentId, incidentId)))
            throw new ArgumentException("INCIDENT_ORDER_UPDATE_SCOPE_MISMATCH", nameof(affectedOrders));
        if (affectedOrders.Select(x => x.ProjectionId).Distinct(StringComparer.Ordinal).Count() != affectedOrders.Count)
            throw new ArgumentException("INCIDENT_ORDER_UPDATE_DUPLICATE_PROJECTION", nameof(affectedOrders));
        if (effectiveAt == default) throw new ArgumentException("INCIDENT_ORDER_UPDATE_EFFECTIVE_TIME_REQUIRED", nameof(effectiveAt));
        if (string.IsNullOrWhiteSpace(evidenceReference)) throw new ArgumentException("INCIDENT_ORDER_UPDATE_EVIDENCE_REQUIRED", nameof(evidenceReference));
        if (string.IsNullOrWhiteSpace(reasonCode)) throw new ArgumentException("INCIDENT_ORDER_UPDATE_REASON_REQUIRED", nameof(reasonCode));

        UpdateId = updateId.Trim();
        UpdateSequence = updateSequence;
        IncidentId = incidentId.Trim();
        Account = account;
        AffectedOrders = Array.AsReadOnly(affectedOrders.ToArray());
        EffectiveAt = effectiveAt;
        EvidenceReference = evidenceReference.Trim();
        ReasonCode = reasonCode.Trim();
    }
}
