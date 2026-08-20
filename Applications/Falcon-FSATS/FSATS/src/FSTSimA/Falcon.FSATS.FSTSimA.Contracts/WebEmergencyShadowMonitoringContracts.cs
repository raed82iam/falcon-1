namespace Falcon.FSATS.FSTSimA.Contracts;

public enum EmergencyShadowState
{
    Active = 1,
    Reconciling = 2,
    EndedReconciled = 3,
    EndedUnresolved = 4
}

public enum EmergencyShadowScenarioKind
{
    LastBrokerConfirmedPosition = 1,
    NotExecuted = 2,
    PartiallyExecuted = 3,
    FullyExecuted = 4,
    UserReportedState = 5
}

public enum EmergencyShadowThresholdState
{
    Unknown = 0,
    WithinObservedThreshold = 1,
    Warning = 2,
    Critical = 3,
    Unavailable = 4
}

public enum EmergencyShadowEvidenceTruth
{
    SimulatorEstimate = 1,
    UserReportedInput = 2,
    LastBrokerConfirmedSeed = 3
}

public enum EmergencyShadowProjectionTruth
{
    Simulator = 1,
    Replay = 2,
    Synthetic = 3,
    Test = 4
}

public enum EmergencyShadowFreshnessState
{
    Current = 1,
    Stale = 2,
    Unknown = 3,
    Unavailable = 4
}

public static class WebEmergencyShadowMonitoringContractIds
{
    public const string RequestV1 = "FSATS.WebEmergencyShadowMonitoringRequest.v1";
    public const string ProjectionV1 = "FSATS.WebEmergencyShadowMonitoringProjection.v1";
    public const string UpdateV1 = "FSATS.WebEmergencyShadowMonitoringUpdate.v1";
}

public sealed record EmergencyShadowBrokerAccountScope
{
    public string BrokerId { get; }
    public string BrokerAccountId { get; }
    public string Environment { get; }

    public EmergencyShadowBrokerAccountScope(string brokerId, string brokerAccountId, string environment)
    {
        if (string.IsNullOrWhiteSpace(brokerId)) throw new ArgumentException("SHADOW_BROKER_ID_REQUIRED", nameof(brokerId));
        if (string.IsNullOrWhiteSpace(brokerAccountId)) throw new ArgumentException("SHADOW_BROKER_ACCOUNT_ID_REQUIRED", nameof(brokerAccountId));
        if (string.IsNullOrWhiteSpace(environment)) throw new ArgumentException("SHADOW_ENVIRONMENT_REQUIRED", nameof(environment));
        BrokerId = brokerId.Trim().ToUpperInvariant();
        BrokerAccountId = brokerAccountId.Trim();
        Environment = environment.Trim().ToUpperInvariant();
    }

    public string NamespaceKey => string.Join('|', Part(BrokerId), Part(BrokerAccountId), Part(Environment));
    private static string Part(string value) => Uri.EscapeDataString(value);
}

public sealed record WebEmergencyShadowMonitoringRequest
{
    public string RequestId { get; }
    public string RequestingApplicationId { get; }
    public string IncidentId { get; }
    public EmergencyShadowBrokerAccountScope Account { get; }
    public string? PositionId { get; }
    public string? SourceOrderId { get; }
    public DateTimeOffset RequestedAt { get; }

    public WebEmergencyShadowMonitoringRequest(
        string requestId,
        string requestingApplicationId,
        string incidentId,
        EmergencyShadowBrokerAccountScope account,
        string? positionId,
        string? sourceOrderId,
        DateTimeOffset requestedAt)
    {
        if (string.IsNullOrWhiteSpace(requestId)) throw new ArgumentException("SHADOW_REQUEST_ID_REQUIRED", nameof(requestId));
        if (!StringComparer.Ordinal.Equals(requestingApplicationId, "SHARED_WEB"))
            throw new ArgumentException("SHADOW_REQUESTING_APPLICATION_MUST_BE_SHARED_WEB", nameof(requestingApplicationId));
        if (string.IsNullOrWhiteSpace(incidentId)) throw new ArgumentException("SHADOW_INCIDENT_ID_REQUIRED", nameof(incidentId));
        ArgumentNullException.ThrowIfNull(account);
        if (positionId is not null && string.IsNullOrWhiteSpace(positionId)) throw new ArgumentException("SHADOW_POSITION_FILTER_MUST_BE_NONEMPTY_WHEN_PRESENT", nameof(positionId));
        if (sourceOrderId is not null && string.IsNullOrWhiteSpace(sourceOrderId)) throw new ArgumentException("SHADOW_ORDER_FILTER_MUST_BE_NONEMPTY_WHEN_PRESENT", nameof(sourceOrderId));
        if (requestedAt == default) throw new ArgumentException("SHADOW_REQUEST_TIME_REQUIRED", nameof(requestedAt));

        RequestId = requestId.Trim();
        RequestingApplicationId = "SHARED_WEB";
        IncidentId = incidentId.Trim();
        Account = account;
        PositionId = positionId?.Trim();
        SourceOrderId = sourceOrderId?.Trim();
        RequestedAt = requestedAt;
    }
}

public sealed record WebEmergencyShadowScenarioProjection
{
    public string ScenarioId { get; }
    public EmergencyShadowScenarioKind ScenarioKind { get; }
    public EmergencyShadowEvidenceTruth EvidenceTruth { get; }
    public decimal? EstimatedQuantity { get; }
    public decimal? EstimatedMarketValue { get; }
    public decimal? EstimatedRiskAmount { get; }
    public EmergencyShadowThresholdState ThresholdState { get; }
    public string Currency { get; }
    public string ReasonCode { get; }
    public IReadOnlyList<string> EvidenceReferences { get; }

    public WebEmergencyShadowScenarioProjection(
        string scenarioId,
        EmergencyShadowScenarioKind scenarioKind,
        EmergencyShadowEvidenceTruth evidenceTruth,
        decimal? estimatedQuantity,
        decimal? estimatedMarketValue,
        decimal? estimatedRiskAmount,
        EmergencyShadowThresholdState thresholdState,
        string currency,
        string reasonCode,
        IReadOnlyList<string> evidenceReferences)
    {
        if (string.IsNullOrWhiteSpace(scenarioId)) throw new ArgumentException("SHADOW_SCENARIO_ID_REQUIRED", nameof(scenarioId));
        if (!Enum.IsDefined(scenarioKind) || !Enum.IsDefined(evidenceTruth) || !Enum.IsDefined(thresholdState)) throw new ArgumentException("INVALID_SHADOW_SCENARIO_ENUM");
        if (estimatedQuantity is < 0m || estimatedMarketValue is < 0m || estimatedRiskAmount is < 0m) throw new ArgumentOutOfRangeException(nameof(estimatedQuantity));
        if (string.IsNullOrWhiteSpace(currency)) throw new ArgumentException("SHADOW_CURRENCY_REQUIRED", nameof(currency));
        if (string.IsNullOrWhiteSpace(reasonCode)) throw new ArgumentException("SHADOW_SCENARIO_REASON_REQUIRED", nameof(reasonCode));
        if (evidenceReferences is null || evidenceReferences.Count == 0 || evidenceReferences.Any(string.IsNullOrWhiteSpace))
            throw new ArgumentException("SHADOW_SCENARIO_EVIDENCE_REQUIRED", nameof(evidenceReferences));

        ScenarioId = scenarioId.Trim();
        ScenarioKind = scenarioKind;
        EvidenceTruth = evidenceTruth;
        EstimatedQuantity = estimatedQuantity;
        EstimatedMarketValue = estimatedMarketValue;
        EstimatedRiskAmount = estimatedRiskAmount;
        ThresholdState = thresholdState;
        Currency = currency.Trim().ToUpperInvariant();
        ReasonCode = reasonCode.Trim();
        EvidenceReferences = Array.AsReadOnly(evidenceReferences.Select(x => x.Trim()).ToArray());
    }
}

public sealed record WebEmergencyShadowMonitoringProjection
{
    public string ProjectionId { get; }
    public string IncidentId { get; }
    public string ShadowCaseId { get; }
    public EmergencyShadowBrokerAccountScope Account { get; }
    public string? PositionId { get; }
    public string? SourceOrderId { get; }
    public string InstrumentId { get; }
    public DateTimeOffset LastBrokerConfirmedAt { get; }
    public DateTimeOffset MonitoringStartedAt { get; }
    public DateTimeOffset? MonitoringEndedAt { get; }
    public EmergencyShadowState ShadowState { get; }
    public bool ContainsExecutionAmbiguity { get; }
    public IReadOnlyList<WebEmergencyShadowScenarioProjection> Scenarios { get; }
    public string? ProtectionClassificationProjectionReference { get; }
    public DateTimeOffset AsOfTime { get; }
    public EmergencyShadowProjectionTruth ProjectionTruth { get; }
    public EmergencyShadowFreshnessState FreshnessState { get; }
    public string ProvenanceReference { get; }
    public string EvidenceReference { get; }

    public WebEmergencyShadowMonitoringProjection(
        string projectionId,
        string incidentId,
        string shadowCaseId,
        EmergencyShadowBrokerAccountScope account,
        string? positionId,
        string? sourceOrderId,
        string instrumentId,
        DateTimeOffset lastBrokerConfirmedAt,
        DateTimeOffset monitoringStartedAt,
        DateTimeOffset? monitoringEndedAt,
        EmergencyShadowState shadowState,
        bool containsExecutionAmbiguity,
        IReadOnlyList<WebEmergencyShadowScenarioProjection> scenarios,
        string? protectionClassificationProjectionReference,
        DateTimeOffset asOfTime,
        EmergencyShadowProjectionTruth projectionTruth,
        EmergencyShadowFreshnessState freshnessState,
        string provenanceReference,
        string evidenceReference)
    {
        if (string.IsNullOrWhiteSpace(projectionId) || string.IsNullOrWhiteSpace(incidentId) || string.IsNullOrWhiteSpace(shadowCaseId))
            throw new ArgumentException("SHADOW_PROJECTION_IDENTITY_REQUIRED");
        ArgumentNullException.ThrowIfNull(account);
        if (string.IsNullOrWhiteSpace(positionId) && string.IsNullOrWhiteSpace(sourceOrderId)) throw new ArgumentException("SHADOW_POSITION_OR_SOURCE_ORDER_REQUIRED");
        if (positionId is not null && string.IsNullOrWhiteSpace(positionId)) throw new ArgumentException("SHADOW_POSITION_ID_INVALID", nameof(positionId));
        if (sourceOrderId is not null && string.IsNullOrWhiteSpace(sourceOrderId)) throw new ArgumentException("SHADOW_SOURCE_ORDER_ID_INVALID", nameof(sourceOrderId));
        if (string.IsNullOrWhiteSpace(instrumentId)) throw new ArgumentException("SHADOW_INSTRUMENT_REQUIRED", nameof(instrumentId));
        if (lastBrokerConfirmedAt == default || monitoringStartedAt == default || asOfTime == default || lastBrokerConfirmedAt > monitoringStartedAt || monitoringStartedAt > asOfTime)
            throw new ArgumentException("INVALID_SHADOW_TIME_ORDER");
        if (monitoringEndedAt is not null && (monitoringEndedAt < monitoringStartedAt || monitoringEndedAt > asOfTime)) throw new ArgumentException("INVALID_SHADOW_END_TIME");
        if (!Enum.IsDefined(shadowState)) throw new ArgumentOutOfRangeException(nameof(shadowState));
        if (!Enum.IsDefined(projectionTruth)) throw new ArgumentOutOfRangeException(nameof(projectionTruth));
        if (!Enum.IsDefined(freshnessState)) throw new ArgumentOutOfRangeException(nameof(freshnessState));
        if (shadowState == EmergencyShadowState.Active && monitoringEndedAt is not null) throw new ArgumentException("ACTIVE_SHADOW_CANNOT_HAVE_END_TIME");
        if ((shadowState is EmergencyShadowState.EndedReconciled or EmergencyShadowState.EndedUnresolved) && monitoringEndedAt is null)
            throw new ArgumentException("ENDED_SHADOW_REQUIRES_END_TIME");
        if (scenarios is null || scenarios.Count == 0) throw new ArgumentException("SHADOW_SCENARIOS_REQUIRED", nameof(scenarios));
        if (scenarios.GroupBy(x => x.ScenarioId, StringComparer.Ordinal).Any(x => x.Count() != 1)) throw new ArgumentException("SHADOW_SCENARIO_IDS_MUST_BE_DISTINCT", nameof(scenarios));
        if (containsExecutionAmbiguity)
        {
            if (string.IsNullOrWhiteSpace(sourceOrderId)) throw new ArgumentException("AMBIGUOUS_SHADOW_REQUIRES_SOURCE_ORDER_ID", nameof(sourceOrderId));
            var kinds = scenarios.Select(x => x.ScenarioKind).ToHashSet();
            if (!kinds.Contains(EmergencyShadowScenarioKind.NotExecuted)
                || !kinds.Contains(EmergencyShadowScenarioKind.PartiallyExecuted)
                || !kinds.Contains(EmergencyShadowScenarioKind.FullyExecuted))
                throw new ArgumentException("AMBIGUOUS_SHADOW_REQUIRES_EXPLICIT_EXECUTION_OUTCOME_SCENARIOS", nameof(scenarios));
        }
        if (!string.IsNullOrWhiteSpace(positionId) && string.IsNullOrWhiteSpace(protectionClassificationProjectionReference))
            throw new ArgumentException("POSITION_SHADOW_REQUIRES_APPLICATION_PROTECTION_CLASSIFICATION_REFERENCE", nameof(protectionClassificationProjectionReference));
        if (string.IsNullOrWhiteSpace(provenanceReference) || string.IsNullOrWhiteSpace(evidenceReference))
            throw new ArgumentException("SHADOW_PROVENANCE_EVIDENCE_REQUIRED");

        ProjectionId = projectionId.Trim();
        IncidentId = incidentId.Trim();
        ShadowCaseId = shadowCaseId.Trim();
        Account = account;
        PositionId = string.IsNullOrWhiteSpace(positionId) ? null : positionId.Trim();
        SourceOrderId = string.IsNullOrWhiteSpace(sourceOrderId) ? null : sourceOrderId.Trim();
        InstrumentId = instrumentId.Trim();
        LastBrokerConfirmedAt = lastBrokerConfirmedAt;
        MonitoringStartedAt = monitoringStartedAt;
        MonitoringEndedAt = monitoringEndedAt;
        ShadowState = shadowState;
        ContainsExecutionAmbiguity = containsExecutionAmbiguity;
        Scenarios = Array.AsReadOnly(scenarios.ToArray());
        ProtectionClassificationProjectionReference = string.IsNullOrWhiteSpace(protectionClassificationProjectionReference) ? null : protectionClassificationProjectionReference.Trim();
        AsOfTime = asOfTime;
        ProjectionTruth = projectionTruth;
        FreshnessState = freshnessState;
        ProvenanceReference = provenanceReference.Trim();
        EvidenceReference = evidenceReference.Trim();
    }
}

public sealed record WebEmergencyShadowMonitoringUpdate
{
    public string UpdateId { get; }
    public long UpdateSequence { get; }
    public string IncidentId { get; }
    public EmergencyShadowBrokerAccountScope Account { get; }
    public IReadOnlyList<WebEmergencyShadowMonitoringProjection> Subjects { get; }
    public DateTimeOffset EffectiveAt { get; }
    public string EvidenceReference { get; }
    public string ReasonCode { get; }

    public WebEmergencyShadowMonitoringUpdate(
        string updateId,
        long updateSequence,
        string incidentId,
        EmergencyShadowBrokerAccountScope account,
        IReadOnlyList<WebEmergencyShadowMonitoringProjection> subjects,
        DateTimeOffset effectiveAt,
        string evidenceReference,
        string reasonCode)
    {
        if (string.IsNullOrWhiteSpace(updateId)) throw new ArgumentException("SHADOW_UPDATE_ID_REQUIRED", nameof(updateId));
        if (updateSequence <= 0) throw new ArgumentOutOfRangeException(nameof(updateSequence));
        if (string.IsNullOrWhiteSpace(incidentId)) throw new ArgumentException("SHADOW_INCIDENT_ID_REQUIRED", nameof(incidentId));
        ArgumentNullException.ThrowIfNull(account);
        ArgumentNullException.ThrowIfNull(subjects);
        if (subjects.Any(x => x.Account != account || !StringComparer.Ordinal.Equals(x.IncidentId, incidentId)))
            throw new ArgumentException("SHADOW_UPDATE_SUBJECT_SCOPE_MISMATCH", nameof(subjects));
        if (subjects.Select(x => x.ProjectionId).Distinct(StringComparer.Ordinal).Count() != subjects.Count)
            throw new ArgumentException("SHADOW_UPDATE_DUPLICATE_SUBJECT_PROJECTION", nameof(subjects));
        if (effectiveAt == default) throw new ArgumentException("SHADOW_UPDATE_EFFECTIVE_TIME_REQUIRED", nameof(effectiveAt));
        if (string.IsNullOrWhiteSpace(evidenceReference)) throw new ArgumentException("SHADOW_UPDATE_EVIDENCE_REQUIRED", nameof(evidenceReference));
        if (string.IsNullOrWhiteSpace(reasonCode)) throw new ArgumentException("SHADOW_UPDATE_REASON_REQUIRED", nameof(reasonCode));

        UpdateId = updateId.Trim();
        UpdateSequence = updateSequence;
        IncidentId = incidentId.Trim();
        Account = account;
        Subjects = Array.AsReadOnly(subjects.ToArray());
        EffectiveAt = effectiveAt;
        EvidenceReference = evidenceReference.Trim();
        ReasonCode = reasonCode.Trim();
    }
}
