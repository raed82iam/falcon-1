namespace Falcon.FSATS.Trading.Contracts;

public enum WebIncidentProtectionState
{
    BrokerConfirmedProtected = 1,
    ProtectionUnknownOrAmbiguous = 2,
    IntentionallyRetainedWithoutCurrentBrokerProtection = 3,
    UnexpectedlyMissingOrIncompleteProtection = 4,
    ReconciliationRequired = 5,
    NotApplicable = 6
}

public enum WebCustomerFollowupRequirement
{
    None = 1,
    Recommended = 2,
    Required = 3
}

public enum WebIncidentNextAction
{
    None = 1,
    VerifyBrokerAccountState = 2,
    VerifyOpenPosition = 3,
    VerifyWorkingOrders = 4,
    VerifyProtectionOrders = 5,
    ResolveAmbiguousSubmission = 6,
    TakeProtectiveActionAtBrokerIfNeeded = 7,
    RepairGovernedCredentialPath = 8,
    AwaitFalconReconciliation = 9,
    ContactSupport = 10
}

public enum WebIncidentFollowupLifecycleState
{
    Active = 1,
    AwaitingCustomerFact = 2,
    AwaitingBrokerReconciliation = 3,
    Reconciled = 4,
    Closed = 5
}

public static class WebIncidentFollowupContractIds
{
    public const string AffectedPositionProjectionV1 = "FSATS.WebAffectedPositionFollowupProjection.v1";
    public const string AffectedPositionUpdateV1 = "FSATS.WebAffectedPositionFollowupUpdate.v1";
}

public sealed record WebIncidentActionInstruction
{
    public int Sequence { get; }
    public WebIncidentNextAction Action { get; }
    public bool Required { get; }
    public string ReasonCode { get; }

    public WebIncidentActionInstruction(int sequence, WebIncidentNextAction action, bool required, string reasonCode)
    {
        if (sequence <= 0) throw new ArgumentOutOfRangeException(nameof(sequence));
        if (!Enum.IsDefined(action) || action == WebIncidentNextAction.None && required)
            throw new ArgumentException("INVALID_INCIDENT_ACTION", nameof(action));
        if (string.IsNullOrWhiteSpace(reasonCode)) throw new ArgumentException("INCIDENT_ACTION_REASON_REQUIRED", nameof(reasonCode));
        Sequence = sequence;
        Action = action;
        Required = required;
        ReasonCode = reasonCode.Trim();
    }
}

public sealed record WebAffectedPositionFollowupProjection
{
    public string ProjectionId { get; }
    public string IncidentId { get; }
    public BrokerAccountScope Account { get; }
    public TradingPositionRef Position { get; }
    public TradingInstrumentRef Instrument { get; }
    public DateTimeOffset LastBrokerConfirmedAt { get; }
    public WebIncidentProtectionState ProtectionState { get; }
    public WebCustomerFollowupRequirement FollowupRequirement { get; }
    public string FollowupReasonCode { get; }
    public IReadOnlyList<WebIncidentActionInstruction> OrderedActions { get; }
    public bool EmergencyShadowMonitoringActive { get; }
    public string? ShadowCaseId { get; }
    public WebIncidentFollowupLifecycleState IncidentState { get; }
    public TruthClassification TruthState { get; }
    public WebFreshnessState FreshnessState { get; }
    public string EvidenceReference { get; }
    public DateTimeOffset AsOfTime { get; }

    public WebAffectedPositionFollowupProjection(
        string projectionId,
        string incidentId,
        BrokerAccountScope account,
        TradingPositionRef position,
        TradingInstrumentRef instrument,
        DateTimeOffset lastBrokerConfirmedAt,
        WebIncidentProtectionState protectionState,
        WebCustomerFollowupRequirement followupRequirement,
        string followupReasonCode,
        IReadOnlyList<WebIncidentActionInstruction> orderedActions,
        bool emergencyShadowMonitoringActive,
        string? shadowCaseId,
        WebIncidentFollowupLifecycleState incidentState,
        TruthClassification truthState,
        WebFreshnessState freshnessState,
        string evidenceReference,
        DateTimeOffset asOfTime)
    {
        if (string.IsNullOrWhiteSpace(projectionId)) throw new ArgumentException("INCIDENT_PROJECTION_ID_REQUIRED", nameof(projectionId));
        if (string.IsNullOrWhiteSpace(incidentId)) throw new ArgumentException("INCIDENT_ID_REQUIRED", nameof(incidentId));
        ArgumentNullException.ThrowIfNull(account);
        if (string.IsNullOrWhiteSpace(position.Value)) throw new ArgumentException("INCIDENT_POSITION_ID_REQUIRED", nameof(position));
        if (string.IsNullOrWhiteSpace(instrument.Value)) throw new ArgumentException("INCIDENT_INSTRUMENT_ID_REQUIRED", nameof(instrument));
        if (lastBrokerConfirmedAt == default || asOfTime == default || lastBrokerConfirmedAt > asOfTime)
            throw new ArgumentException("INVALID_INCIDENT_TIME_ORDER");
        if (!Enum.IsDefined(protectionState) || !Enum.IsDefined(followupRequirement) || !Enum.IsDefined(incidentState))
            throw new ArgumentException("INVALID_INCIDENT_ENUM");
        if (protectionState == WebIncidentProtectionState.BrokerConfirmedProtected
            && (truthState != TruthClassification.Current || freshnessState != WebFreshnessState.Current))
            throw new ArgumentException("BROKER_CONFIRMED_PROTECTED_REQUIRES_CURRENT_BROKER_TRUTH");
        if (protectionState == WebIncidentProtectionState.UnexpectedlyMissingOrIncompleteProtection
            && followupRequirement != WebCustomerFollowupRequirement.Required)
            throw new ArgumentException("UNEXPECTED_PROTECTION_GAP_REQUIRES_CUSTOMER_FOLLOWUP");
        if (string.IsNullOrWhiteSpace(followupReasonCode)) throw new ArgumentException("FOLLOWUP_REASON_REQUIRED", nameof(followupReasonCode));
        if (orderedActions is null) throw new ArgumentNullException(nameof(orderedActions));
        if (orderedActions.GroupBy(x => x.Sequence).Any(x => x.Count() != 1) || !orderedActions.Select(x => x.Sequence).SequenceEqual(orderedActions.Select(x => x.Sequence).OrderBy(x => x)))
            throw new ArgumentException("INCIDENT_ACTIONS_MUST_HAVE_UNIQUE_ASCENDING_SEQUENCE", nameof(orderedActions));
        if (followupRequirement == WebCustomerFollowupRequirement.Required && !orderedActions.Any(x => x.Required))
            throw new ArgumentException("REQUIRED_FOLLOWUP_REQUIRES_AT_LEAST_ONE_REQUIRED_ACTION", nameof(orderedActions));
        if (followupRequirement == WebCustomerFollowupRequirement.None && orderedActions.Any(x => x.Required))
            throw new ArgumentException("NO_FOLLOWUP_CANNOT_CONTAIN_REQUIRED_ACTION", nameof(orderedActions));
        if (emergencyShadowMonitoringActive != !string.IsNullOrWhiteSpace(shadowCaseId))
            throw new ArgumentException("SHADOW_ACTIVE_STATE_REQUIRES_EXACT_SHADOW_CASE_ID", nameof(shadowCaseId));
        if (string.IsNullOrWhiteSpace(evidenceReference)) throw new ArgumentException("INCIDENT_EVIDENCE_REQUIRED", nameof(evidenceReference));

        ProjectionId = projectionId.Trim();
        IncidentId = incidentId.Trim();
        Account = account;
        Position = new TradingPositionRef(position.Value.Trim());
        Instrument = new TradingInstrumentRef(instrument.Value.Trim());
        LastBrokerConfirmedAt = lastBrokerConfirmedAt;
        ProtectionState = protectionState;
        FollowupRequirement = followupRequirement;
        FollowupReasonCode = followupReasonCode.Trim();
        OrderedActions = Array.AsReadOnly(orderedActions.ToArray());
        EmergencyShadowMonitoringActive = emergencyShadowMonitoringActive;
        ShadowCaseId = string.IsNullOrWhiteSpace(shadowCaseId) ? null : shadowCaseId.Trim();
        IncidentState = incidentState;
        TruthState = truthState;
        FreshnessState = freshnessState;
        EvidenceReference = evidenceReference.Trim();
        AsOfTime = asOfTime;
    }
}

public sealed record WebAffectedPositionFollowupUpdate
{
    public string UpdateId { get; }
    public long UpdateSequence { get; }
    public string IncidentId { get; }
    public BrokerAccountScope Account { get; }
    public IReadOnlyList<WebAffectedPositionFollowupProjection> AffectedPositions { get; }
    public DateTimeOffset EffectiveAt { get; }
    public string EvidenceReference { get; }
    public string ReasonCode { get; }

    public WebAffectedPositionFollowupUpdate(
        string updateId,
        long updateSequence,
        string incidentId,
        BrokerAccountScope account,
        IReadOnlyList<WebAffectedPositionFollowupProjection> affectedPositions,
        DateTimeOffset effectiveAt,
        string evidenceReference,
        string reasonCode)
    {
        if (string.IsNullOrWhiteSpace(updateId)) throw new ArgumentException("INCIDENT_UPDATE_ID_REQUIRED", nameof(updateId));
        if (updateSequence <= 0) throw new ArgumentOutOfRangeException(nameof(updateSequence));
        if (string.IsNullOrWhiteSpace(incidentId)) throw new ArgumentException("INCIDENT_ID_REQUIRED", nameof(incidentId));
        ArgumentNullException.ThrowIfNull(account);
        ArgumentNullException.ThrowIfNull(affectedPositions);
        if (affectedPositions.Any(x => x.Account != account || !StringComparer.Ordinal.Equals(x.IncidentId, incidentId)))
            throw new ArgumentException("INCIDENT_UPDATE_POSITION_SCOPE_MISMATCH", nameof(affectedPositions));
        if (affectedPositions.Select(x => x.ProjectionId).Distinct(StringComparer.Ordinal).Count() != affectedPositions.Count)
            throw new ArgumentException("INCIDENT_UPDATE_DUPLICATE_POSITION_PROJECTION", nameof(affectedPositions));
        if (effectiveAt == default) throw new ArgumentException("INCIDENT_UPDATE_EFFECTIVE_TIME_REQUIRED", nameof(effectiveAt));
        if (string.IsNullOrWhiteSpace(evidenceReference)) throw new ArgumentException("INCIDENT_UPDATE_EVIDENCE_REQUIRED", nameof(evidenceReference));
        if (string.IsNullOrWhiteSpace(reasonCode)) throw new ArgumentException("INCIDENT_UPDATE_REASON_REQUIRED", nameof(reasonCode));

        UpdateId = updateId.Trim();
        UpdateSequence = updateSequence;
        IncidentId = incidentId.Trim();
        Account = account;
        AffectedPositions = Array.AsReadOnly(affectedPositions.ToArray());
        EffectiveAt = effectiveAt;
        EvidenceReference = evidenceReference.Trim();
        ReasonCode = reasonCode.Trim();
    }
}
