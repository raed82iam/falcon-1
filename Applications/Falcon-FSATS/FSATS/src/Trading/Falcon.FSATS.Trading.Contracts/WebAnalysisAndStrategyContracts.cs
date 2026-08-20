namespace Falcon.FSATS.Trading.Contracts;

public enum WebAnalysisLifecycleState
{
    Requested = 1,
    Accepted = 2,
    Running = 3,
    Completed = 4,
    CancelRequested = 5,
    Canceled = 6,
    Failed = 7,
    Unavailable = 8,
    Rejected = 9
}

public enum WebOnDemandAnalysisResultState
{
    Completed = 1,
    Partial = 2,
    Unavailable = 3,
    Unsupported = 4,
    NeedsClarification = 5,
    Rejected = 6
}

public enum WebRiskBand
{
    Unknown = 0,
    Low = 1,
    Moderate = 2,
    High = 3,
    VeryHigh = 4
}

public enum WebCatalogApplicabilityState
{
    Applicable = 1,
    NotApplicable = 2,
    Unknown = 3,
    Unavailable = 4
}

public enum WebDetailedSynthesisState
{
    Complete = 1,
    Partial = 2,
    Conflicted = 3,
    Unavailable = 4,
    Unknown = 5
}

public static class WebOnDemandAnalysisContractIds
{
    public const string RequestV1 = "FSATS.WebOnDemandAnalysisRequest.v1";
    public const string ResultV1 = "FSATS.WebOnDemandAnalysisResult.v1";
    public const string DetailedAssetAnalysisProjectionV1 = "FSATS.WebDetailedAssetAnalysisProjection.v1";

    // Historical R3 compatibility identity only. It is intentionally retained without
    // compiler-level Obsolete warnings because the repository treats warnings as errors.
    // Current FCR-0127/FCR-0130 binding uses ResultV1 instead.
    public const string ProjectionV1 = "FSATS.WebOnDemandAnalysisProjection.v1";

    // Historical R3 compatibility identity only. No active current FCR grants this
    // command as the canonical public Web analysis surface.
    public const string CommandV1 = "FSATS.WebOnDemandAnalysisCommand.v1";
}

public static class WebStrategyCatalogContractIds
{
    public const string RequestV1 = "FSATS.WebStrategyCatalogRequest.v1";
    public const string ProjectionV1 = "FSATS.WebStrategyCatalogProjection.v1";
    public const string UpdateV1 = "FSATS.WebStrategyCatalogUpdate.v1";
}

public sealed record WebOnDemandAnalysisRequest
{
    public const string SharedWebApplicationId = "SHARED_WEB";

    public string RequestId { get; }
    public CorrelationId Correlation { get; }
    public string RequestingApplicationId { get; }
    public TradingInstrumentRef RequestedInstrumentReference { get; }
    public string? MarketOrVenueHint { get; }
    public string? AssetClassHint { get; }
    public string AnalysisIntent { get; }
    public DateTimeOffset RequestedAt { get; }
    public string? EntitlementReference { get; }
    public BrokerAccountScope? AccountScope { get; }

    public WebOnDemandAnalysisRequest(
        string requestId,
        CorrelationId correlation,
        string requestingApplicationId,
        TradingInstrumentRef requestedInstrumentReference,
        string? marketOrVenueHint,
        string? assetClassHint,
        string analysisIntent,
        DateTimeOffset requestedAt,
        string? entitlementReference = null,
        BrokerAccountScope? accountScope = null)
    {
        if (string.IsNullOrWhiteSpace(requestId)) throw new ArgumentException("WEB_ANALYSIS_REQUEST_ID_REQUIRED", nameof(requestId));
        if (!StringComparer.Ordinal.Equals(requestingApplicationId, SharedWebApplicationId))
            throw new ArgumentException("WEB_ANALYSIS_REQUESTING_APPLICATION_MUST_BE_SHARED_WEB", nameof(requestingApplicationId));
        if (string.IsNullOrWhiteSpace(requestedInstrumentReference.Value)) throw new ArgumentException("WEB_ANALYSIS_INSTRUMENT_REFERENCE_REQUIRED", nameof(requestedInstrumentReference));
        if (string.IsNullOrWhiteSpace(analysisIntent)) throw new ArgumentException("WEB_ANALYSIS_INTENT_REQUIRED", nameof(analysisIntent));

        RequestId = requestId.Trim();
        Correlation = correlation;
        RequestingApplicationId = SharedWebApplicationId;
        RequestedInstrumentReference = new TradingInstrumentRef(requestedInstrumentReference.Value.Trim());
        MarketOrVenueHint = NormalizeOptional(marketOrVenueHint);
        AssetClassHint = NormalizeOptional(assetClassHint);
        AnalysisIntent = analysisIntent.Trim().ToUpperInvariant();
        RequestedAt = requestedAt;
        EntitlementReference = NormalizeOptional(entitlementReference);
        AccountScope = accountScope;
    }

    // Historical R3 source-compatible constructor. New callers use the canonical FCR-0127
    // shape above, but this overload remains warning-free for warnings-as-errors builds.
    public WebOnDemandAnalysisRequest(
        string requestId,
        CorrelationId correlation,
        TradingInstrumentRef instrument,
        BrokerAccountScope? accountScope,
        IReadOnlyList<string> requestedSections,
        DateTimeOffset requestedAt)
        : this(
            requestId,
            correlation,
            SharedWebApplicationId,
            instrument,
            null,
            null,
            requestedSections is { Count: > 0 } ? string.Join('+', requestedSections.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim())) : "GENERAL_ANALYSIS",
            requestedAt,
            null,
            accountScope)
    {
    }

    private static string? NormalizeOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

public sealed record WebSchoolApplicabilityItem(
    string SchoolId,
    string SchoolName,
    WebCatalogApplicabilityState Applicability,
    decimal? Confidence,
    string ReasonCode,
    string? Explanation = null);

public sealed record WebStrategyApplicabilityItem(
    string StrategyId,
    string StrategyName,
    WebCatalogApplicabilityState Applicability,
    decimal? Score,
    bool IsBestCurrentCandidate,
    string ReasonCode,
    string? Explanation = null);

public sealed record WebTradingRiskProjection
{
    public WebRiskBand RiskBand { get; }
    public decimal? RiskScore { get; }
    public bool IsAccountAware { get; }
    public BrokerAccountScope? AccountScope { get; }
    public string ReasonCode { get; }
    public string? Explanation { get; }

    public WebTradingRiskProjection(
        WebRiskBand riskBand,
        decimal? riskScore,
        bool isAccountAware,
        BrokerAccountScope? accountScope,
        string reasonCode,
        string? explanation = null)
    {
        if (!Enum.IsDefined(riskBand)) throw new ArgumentOutOfRangeException(nameof(riskBand));
        if (string.IsNullOrWhiteSpace(reasonCode)) throw new ArgumentException("WEB_RISK_REASON_CODE_REQUIRED", nameof(reasonCode));
        if (isAccountAware != (accountScope is not null))
            throw new ArgumentException("ACCOUNT_AWARE_RISK_REQUIRES_EXACT_BROKER_ACCOUNT_SCOPE", nameof(accountScope));

        RiskBand = riskBand;
        RiskScore = riskScore;
        IsAccountAware = isAccountAware;
        AccountScope = accountScope;
        ReasonCode = reasonCode.Trim();
        Explanation = explanation;
    }
}

public sealed record WebAnalysisInputTruthFreshnessSummary(
    TruthClassification TruthState,
    WebFreshnessState FreshnessState,
    WebProjectionCompleteness Completeness,
    IReadOnlyList<string> Limitations);

public sealed record WebMaterialLevelOrTarget(
    string Kind,
    decimal Value,
    string? UnitOrCurrency,
    string EvidenceReference);

public sealed record WebDetailedHorizonView(
    string HorizonId,
    WebOnDemandAnalysisResultState ResultState,
    string Conclusion,
    IReadOnlyList<WebMaterialLevelOrTarget> MaterialLevelsOrTargets,
    decimal? ConfidenceOrStrength,
    IReadOnlyList<string> Limitations,
    IReadOnlyList<string> EvidenceOrSourceOutputReferences);

public sealed record WebDetailedStrategyView(
    string StrategyId,
    WebCatalogApplicabilityState ApplicabilityState,
    WebOnDemandAnalysisResultState ResultState,
    string Conclusion,
    IReadOnlyList<WebMaterialLevelOrTarget> MaterialLevelsOrTargets,
    decimal? ConfidenceOrStrength,
    DateTimeOffset AsOfTime,
    TruthClassification TruthState,
    WebFreshnessState FreshnessState,
    IReadOnlyList<string> Limitations,
    IReadOnlyList<string> EvidenceOrSourceOutputReferences);

public sealed record WebDetailedSchoolView(
    string SchoolId,
    WebCatalogApplicabilityState ApplicabilityState,
    WebOnDemandAnalysisResultState ResultState,
    string PerspectiveOrConclusion,
    IReadOnlyList<WebMaterialLevelOrTarget> MaterialLevelsOrTargets,
    decimal? ConfidenceOrStrength,
    DateTimeOffset AsOfTime,
    TruthClassification TruthState,
    WebFreshnessState FreshnessState,
    IReadOnlyList<string> Limitations,
    IReadOnlyList<string> EvidenceOrSourceOutputReferences);

public sealed record WebDetailedAnalysisSynthesis(
    WebDetailedSynthesisState SynthesisState,
    IReadOnlyList<string> Agreements,
    IReadOnlyList<string> Disagreements,
    IReadOnlyList<string> UnresolvedConflicts,
    string BoundedCombinedExplanation,
    IReadOnlyList<string> ContributingOutputReferences,
    IReadOnlyList<string> Limitations);

public sealed record WebDetailedAssetAnalysisProjection
{
    public TradingInstrumentRef ResolvedInstrumentIdentity { get; }
    public string AnalysisResultId { get; }
    public DateTimeOffset AsOfTime { get; }
    public TruthClassification OverallTruthState { get; }
    public WebAnalysisInputTruthFreshnessSummary InputTruthFreshnessSummary { get; }
    public IReadOnlyList<WebDetailedHorizonView> HorizonViews { get; }
    public IReadOnlyList<WebDetailedStrategyView> StrategyViews { get; }
    public IReadOnlyList<WebDetailedSchoolView> SchoolViews { get; }
    public WebDetailedAnalysisSynthesis Synthesis { get; }

    public WebDetailedAssetAnalysisProjection(
        TradingInstrumentRef resolvedInstrumentIdentity,
        string analysisResultId,
        DateTimeOffset asOfTime,
        TruthClassification overallTruthState,
        WebAnalysisInputTruthFreshnessSummary inputTruthFreshnessSummary,
        IReadOnlyList<WebDetailedHorizonView> horizonViews,
        IReadOnlyList<WebDetailedStrategyView> strategyViews,
        IReadOnlyList<WebDetailedSchoolView> schoolViews,
        WebDetailedAnalysisSynthesis synthesis)
    {
        if (string.IsNullOrWhiteSpace(resolvedInstrumentIdentity.Value)) throw new ArgumentException("DETAILED_ANALYSIS_RESOLVED_INSTRUMENT_REQUIRED", nameof(resolvedInstrumentIdentity));
        if (string.IsNullOrWhiteSpace(analysisResultId)) throw new ArgumentException("DETAILED_ANALYSIS_RESULT_ID_REQUIRED", nameof(analysisResultId));
        ArgumentNullException.ThrowIfNull(inputTruthFreshnessSummary);
        ArgumentNullException.ThrowIfNull(horizonViews);
        ArgumentNullException.ThrowIfNull(strategyViews);
        ArgumentNullException.ThrowIfNull(schoolViews);
        ArgumentNullException.ThrowIfNull(synthesis);

        if (overallTruthState == TruthClassification.Current
            && (inputTruthFreshnessSummary.TruthState != TruthClassification.Current
                || inputTruthFreshnessSummary.FreshnessState != WebFreshnessState.Current))
            throw new ArgumentException("CURRENT_SYNTHESIS_REQUIRES_CURRENT_INPUT_TRUTH_AND_FRESHNESS");
        if (inputTruthFreshnessSummary.Completeness != WebProjectionCompleteness.Complete && synthesis.SynthesisState == WebDetailedSynthesisState.Complete)
            throw new ArgumentException("PARTIAL_INPUTS_CANNOT_PRODUCE_COMPLETE_SYNTHESIS");
        if ((synthesis.Disagreements.Count > 0 || synthesis.UnresolvedConflicts.Count > 0) && synthesis.SynthesisState == WebDetailedSynthesisState.Complete)
            throw new ArgumentException("MATERIAL_DISAGREEMENT_CANNOT_BE_REPRESENTED_AS_UNQUALIFIED_COMPLETE_SYNTHESIS");

        ResolvedInstrumentIdentity = new TradingInstrumentRef(resolvedInstrumentIdentity.Value.Trim());
        AnalysisResultId = analysisResultId.Trim();
        AsOfTime = asOfTime;
        OverallTruthState = overallTruthState;
        InputTruthFreshnessSummary = inputTruthFreshnessSummary;
        HorizonViews = Array.AsReadOnly(horizonViews.ToArray());
        StrategyViews = Array.AsReadOnly(strategyViews.ToArray());
        SchoolViews = Array.AsReadOnly(schoolViews.ToArray());
        Synthesis = synthesis;
    }
}

public sealed record WebOnDemandAnalysisProjectionPayload(
    IReadOnlyList<WebSchoolApplicabilityItem> Schools,
    IReadOnlyList<WebStrategyApplicabilityItem> Strategies,
    WebTradingRiskProjection? Risk,
    string? BestStrategyId,
    WebDetailedAssetAnalysisProjection? DetailedAssetAnalysis);

public sealed record WebOnDemandAnalysisResult
{
    public string RequestId { get; }
    public CorrelationId Correlation { get; }
    public string AnalysisResultId { get; }
    public TradingInstrumentRef? ResolvedInstrumentIdentity { get; }
    public string AnalysisIntent { get; }
    public WebOnDemandAnalysisResultState ResultState { get; }
    public WebOnDemandAnalysisProjectionPayload? AnalysisProjection { get; }
    public DateTimeOffset AsOfTime { get; }
    public WebAnalysisInputTruthFreshnessSummary InputTruthFreshnessSummary { get; }
    public decimal? ConfidenceOrStrength { get; }
    public IReadOnlyList<string> Limitations { get; }
    public string? ReasonCode { get; }
    public IReadOnlyList<TradingInstrumentRef> ClarificationCandidates { get; }

    public WebOnDemandAnalysisResult(
        string requestId,
        CorrelationId correlation,
        string analysisResultId,
        TradingInstrumentRef? resolvedInstrumentIdentity,
        string analysisIntent,
        WebOnDemandAnalysisResultState resultState,
        WebOnDemandAnalysisProjectionPayload? analysisProjection,
        DateTimeOffset asOfTime,
        WebAnalysisInputTruthFreshnessSummary inputTruthFreshnessSummary,
        decimal? confidenceOrStrength,
        IReadOnlyList<string> limitations,
        string? reasonCode = null,
        IReadOnlyList<TradingInstrumentRef>? clarificationCandidates = null)
    {
        if (string.IsNullOrWhiteSpace(requestId)) throw new ArgumentException("WEB_ANALYSIS_RESULT_REQUEST_ID_REQUIRED", nameof(requestId));
        if (string.IsNullOrWhiteSpace(analysisResultId)) throw new ArgumentException("WEB_ANALYSIS_RESULT_ID_REQUIRED", nameof(analysisResultId));
        if (string.IsNullOrWhiteSpace(analysisIntent)) throw new ArgumentException("WEB_ANALYSIS_RESULT_INTENT_REQUIRED", nameof(analysisIntent));
        if (!Enum.IsDefined(resultState)) throw new ArgumentOutOfRangeException(nameof(resultState));
        ArgumentNullException.ThrowIfNull(inputTruthFreshnessSummary);
        ArgumentNullException.ThrowIfNull(limitations);

        var candidates = clarificationCandidates ?? Array.Empty<TradingInstrumentRef>();
        if (resultState == WebOnDemandAnalysisResultState.NeedsClarification)
        {
            if (resolvedInstrumentIdentity is not null) throw new ArgumentException("NEEDS_CLARIFICATION_CANNOT_CLAIM_RESOLVED_INSTRUMENT", nameof(resolvedInstrumentIdentity));
            if (candidates.Count == 0 || candidates.Any(x => string.IsNullOrWhiteSpace(x.Value)))
                throw new ArgumentException("NEEDS_CLARIFICATION_REQUIRES_BOUNDED_CANDIDATE_IDENTITIES", nameof(clarificationCandidates));
            if (analysisProjection is not null) throw new ArgumentException("NEEDS_CLARIFICATION_CANNOT_CLAIM_ANALYSIS_PROJECTION", nameof(analysisProjection));
        }
        else if (resultState == WebOnDemandAnalysisResultState.Completed)
        {
            if (resolvedInstrumentIdentity is null || string.IsNullOrWhiteSpace(resolvedInstrumentIdentity.Value.Value))
                throw new ArgumentException("COMPLETED_ANALYSIS_REQUIRES_RESOLVED_INSTRUMENT", nameof(resolvedInstrumentIdentity));
            if (analysisProjection is null) throw new ArgumentException("COMPLETED_ANALYSIS_REQUIRES_PROJECTION", nameof(analysisProjection));
            if (inputTruthFreshnessSummary.Completeness != WebProjectionCompleteness.Complete)
                throw new ArgumentException("COMPLETED_ANALYSIS_CANNOT_HAVE_PARTIAL_INPUTS", nameof(inputTruthFreshnessSummary));
        }

        RequestId = requestId.Trim();
        Correlation = correlation;
        AnalysisResultId = analysisResultId.Trim();
        ResolvedInstrumentIdentity = resolvedInstrumentIdentity is null ? null : new TradingInstrumentRef(resolvedInstrumentIdentity.Value.Value.Trim());
        AnalysisIntent = analysisIntent.Trim().ToUpperInvariant();
        ResultState = resultState;
        AnalysisProjection = analysisProjection;
        AsOfTime = asOfTime;
        InputTruthFreshnessSummary = inputTruthFreshnessSummary;
        ConfidenceOrStrength = confidenceOrStrength;
        Limitations = Array.AsReadOnly(limitations.ToArray());
        ReasonCode = string.IsNullOrWhiteSpace(reasonCode) ? null : reasonCode.Trim();
        ClarificationCandidates = Array.AsReadOnly(candidates.ToArray());
    }
}

// Historical R3 source-compatibility type only. New Web binding uses
// WebOnDemandAnalysisResult and FSATS.WebOnDemandAnalysisResult.v1.
public sealed record WebOnDemandAnalysisProjection(
    string ProjectionId,
    string RequestId,
    CorrelationId Correlation,
    TradingInstrumentRef Instrument,
    WebAnalysisLifecycleState State,
    IReadOnlyList<WebSchoolApplicabilityItem> Schools,
    IReadOnlyList<WebStrategyApplicabilityItem> Strategies,
    WebTradingRiskProjection Risk,
    string? BestStrategyId,
    TruthClassification TruthState,
    WebFreshnessState FreshnessState,
    WebProjectionCompleteness Completeness,
    WebAvailabilityState AvailabilityState,
    string EvidenceReference,
    string ReasonCode,
    DateTimeOffset AsOfTime,
    string? LimitationDetail = null);

// Historical R3 source-compatibility type only. Current FCR-0127/FCR-0130 do not
// grant this as a canonical public contract.
public sealed record WebOnDemandAnalysisCommand(
    string CommandId,
    string RequestId,
    CorrelationId Correlation,
    string Action,
    DateTimeOffset RequestedAt,
    string? Reason = null);

public sealed record WebStrategyCatalogRequest(
    string RequestId,
    CorrelationId Correlation,
    TradingInstrumentRef? Instrument,
    string? Market,
    DateTimeOffset RequestedAt);

public sealed record WebStrategyCatalogItem
{
    public string StrategyId { get; }
    public string StrategyName { get; }
    public string SchoolId { get; }
    public string SchoolName { get; }
    public WebCatalogApplicabilityState Applicability { get; }
    public bool Visible { get; }
    public bool Enabled { get; }
    public string ReasonCode { get; }
    public string? Explanation { get; }

    public WebStrategyCatalogItem(
        string strategyId,
        string strategyName,
        string schoolId,
        string schoolName,
        WebCatalogApplicabilityState applicability,
        bool visible,
        bool enabled,
        string reasonCode,
        string? explanation = null)
    {
        if (string.IsNullOrWhiteSpace(strategyId)) throw new ArgumentException("STRATEGY_ID_REQUIRED", nameof(strategyId));
        if (string.IsNullOrWhiteSpace(strategyName)) throw new ArgumentException("STRATEGY_NAME_REQUIRED", nameof(strategyName));
        if (string.IsNullOrWhiteSpace(schoolId)) throw new ArgumentException("SCHOOL_ID_REQUIRED", nameof(schoolId));
        if (string.IsNullOrWhiteSpace(schoolName)) throw new ArgumentException("SCHOOL_NAME_REQUIRED", nameof(schoolName));
        if (!Enum.IsDefined(applicability)) throw new ArgumentOutOfRangeException(nameof(applicability));
        if (string.IsNullOrWhiteSpace(reasonCode)) throw new ArgumentException("STRATEGY_APPLICABILITY_REASON_REQUIRED", nameof(reasonCode));
        if (applicability == WebCatalogApplicabilityState.NotApplicable && (!visible || enabled))
            throw new ArgumentException("NOT_APPLICABLE_STRATEGY_MUST_BE_VISIBLE_DISABLED_WITH_REASON", nameof(applicability));

        StrategyId = strategyId.Trim();
        StrategyName = strategyName.Trim();
        SchoolId = schoolId.Trim();
        SchoolName = schoolName.Trim();
        Applicability = applicability;
        Visible = visible;
        Enabled = enabled;
        ReasonCode = reasonCode.Trim();
        Explanation = explanation;
    }
}

public sealed record WebStrategyCatalogProjection(
    string ProjectionId,
    string RequestId,
    CorrelationId Correlation,
    IReadOnlyList<WebStrategyCatalogItem> Strategies,
    TruthClassification TruthState,
    WebFreshnessState FreshnessState,
    WebProjectionCompleteness Completeness,
    WebAvailabilityState AvailabilityState,
    string EvidenceReference,
    string ReasonCode,
    DateTimeOffset AsOfTime);

public sealed record WebStrategyCatalogUpdate
{
    public string UpdateId { get; }
    public long UpdateSequence { get; }
    public CorrelationId Correlation { get; }
    public IReadOnlyList<string> ChangedStrategyIds { get; }
    public WebProjectionUpdateKind UpdateKind { get; }
    public TruthClassification TruthState { get; }
    public WebFreshnessState FreshnessState { get; }
    public string EvidenceReference { get; }
    public string ReasonCode { get; }
    public DateTimeOffset EffectiveAt { get; }
    public string? CorrectsUpdateId { get; }
    public string? SupersedesUpdateId { get; }

    public WebStrategyCatalogUpdate(
        string updateId,
        long updateSequence,
        CorrelationId correlation,
        IReadOnlyList<string> changedStrategyIds,
        WebProjectionUpdateKind updateKind,
        TruthClassification truthState,
        WebFreshnessState freshnessState,
        string evidenceReference,
        string reasonCode,
        DateTimeOffset effectiveAt,
        string? correctsUpdateId = null,
        string? supersedesUpdateId = null)
    {
        if (string.IsNullOrWhiteSpace(updateId)) throw new ArgumentException("STRATEGY_UPDATE_ID_REQUIRED", nameof(updateId));
        if (updateSequence <= 0) throw new ArgumentOutOfRangeException(nameof(updateSequence));
        if (changedStrategyIds is null || changedStrategyIds.Count == 0 || changedStrategyIds.Any(string.IsNullOrWhiteSpace) || changedStrategyIds.Distinct(StringComparer.Ordinal).Count() != changedStrategyIds.Count)
            throw new ArgumentException("CHANGED_STRATEGY_IDS_MUST_BE_NONEMPTY_DISTINCT", nameof(changedStrategyIds));
        if (!Enum.IsDefined(updateKind)) throw new ArgumentOutOfRangeException(nameof(updateKind));
        if (string.IsNullOrWhiteSpace(evidenceReference)) throw new ArgumentException("STRATEGY_UPDATE_EVIDENCE_REQUIRED", nameof(evidenceReference));
        if (string.IsNullOrWhiteSpace(reasonCode)) throw new ArgumentException("STRATEGY_UPDATE_REASON_REQUIRED", nameof(reasonCode));
        ValidateLineage(updateKind, correctsUpdateId, supersedesUpdateId);

        UpdateId = updateId.Trim();
        UpdateSequence = updateSequence;
        Correlation = correlation;
        ChangedStrategyIds = Array.AsReadOnly(changedStrategyIds.Select(x => x.Trim()).ToArray());
        UpdateKind = updateKind;
        TruthState = truthState;
        FreshnessState = freshnessState;
        EvidenceReference = evidenceReference.Trim();
        ReasonCode = reasonCode.Trim();
        EffectiveAt = effectiveAt;
        CorrectsUpdateId = NormalizeOptional(correctsUpdateId);
        SupersedesUpdateId = NormalizeOptional(supersedesUpdateId);
    }

    private static void ValidateLineage(WebProjectionUpdateKind kind, string? corrects, string? supersedes)
    {
        var hasCorrects = !string.IsNullOrWhiteSpace(corrects);
        var hasSupersedes = !string.IsNullOrWhiteSpace(supersedes);
        var valid = kind switch
        {
            WebProjectionUpdateKind.Ordinary => !hasCorrects && !hasSupersedes,
            WebProjectionUpdateKind.Correction => hasCorrects && !hasSupersedes,
            WebProjectionUpdateKind.Supersession => !hasCorrects && hasSupersedes,
            _ => false
        };
        if (!valid) throw new ArgumentException("STRATEGY_UPDATE_LINEAGE_DOES_NOT_MATCH_UPDATE_KIND");
    }

    private static string? NormalizeOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}