namespace Falcon.FSATS.Trading.Contracts;

public enum TruthClassification { Current, LastKnown, Stale, Unknown, Simulation, Replay }
public enum CommandOutcomeState { Received, Accepted, Rejected, Applied, PartiallyApplied, Expired, Revoked, ReconciliationRequired }
public enum WebProjectionCompleteness { Complete, Partial, Unknown }
public enum WebFreshnessState { Current, Stale, Unknown, Unavailable }
public enum WebAvailabilityState { Available, Unsupported, NotApplicable, Unknown, Unavailable, Degraded }
public enum WebOrderTradeState { Requested, Accepted, PartiallyFilled, Filled, CancelRequested, Cancelled, ReplacementRequested, Replaced, Rejected, UnknownBrokerOutcome }
public enum WebProjectionUpdateKind { Ordinary, Correction, Supersession }
public enum WebMarketDataPurpose { PresentationOnly, FsatsOperationalAnalysis }

public readonly record struct TradingInstrumentRef(string Value);
public readonly record struct TradingOrderRef(string Value);
public readonly record struct TradingPositionRef(string Value);
public readonly record struct CorrelationId(string Value);
public readonly record struct CausationId(string Value);
public readonly record struct TrustEpoch(long Value);
public readonly record struct BrokerId(string Value);
public readonly record struct BrokerAccountId(string Value);

public sealed record BrokerAccountScope
{
    public BrokerId Broker { get; }
    public BrokerAccountId Account { get; }
    public string Environment { get; }

    public BrokerAccountScope(BrokerId broker, BrokerAccountId account, string environment)
    {
        if (string.IsNullOrWhiteSpace(broker.Value)) throw new ArgumentException("BROKER_ID_REQUIRED", nameof(broker));
        if (string.IsNullOrWhiteSpace(account.Value)) throw new ArgumentException("BROKER_ACCOUNT_ID_REQUIRED", nameof(account));
        if (string.IsNullOrWhiteSpace(environment)) throw new ArgumentException("BROKER_ENVIRONMENT_REQUIRED", nameof(environment));
        Broker = new BrokerId(broker.Value.Trim().ToUpperInvariant());
        Account = new BrokerAccountId(account.Value.Trim());
        Environment = environment.Trim().ToUpperInvariant();
    }

    public string NamespaceKey => string.Join('|', Part(Broker.Value), Part(Account.Value), Part(Environment));
    private static string Part(string value) => Uri.EscapeDataString(value);
}

public sealed record TradingDecisionEvidence(
    string DecisionId,
    TradingInstrumentRef Instrument,
    string StrategyId,
    decimal Confidence,
    TrustEpoch TrustEpoch,
    CorrelationId Correlation,
    CausationId Causation,
    DateTimeOffset EffectiveAt);

public sealed record ExposureSafetyProjection(
    TradingPositionRef Position,
    TradingInstrumentRef Instrument,
    decimal Quantity,
    decimal MaximumAuthorizedLoss,
    string ProtectionState,
    string ReconciliationState,
    TruthClassification Truth,
    DateTimeOffset ObservedAt);

public sealed record ProtectionCommandOutcome(
    string CommandId,
    CommandOutcomeState State,
    string ReasonCode,
    CorrelationId Correlation,
    DateTimeOffset EffectiveAt);

public sealed record TradingResourceEvidence(
    string ApplicationId,
    string ResourceClass,
    decimal CurrentConsumption,
    decimal MinimumSafeRequirement,
    decimal DesiredCapacity,
    decimal ReclaimableCapacity,
    string DegradationOptions,
    DateTimeOffset ObservedAt);

public static class WebAnalysisContractIds
{
    public const string OnDemandAnalysisRequest = "FSATS.WebOnDemandAnalysisRequest.v1";
    public const string OnDemandAnalysisResult = "FSATS.WebOnDemandAnalysisResult.v1";
    public const string DetailedAssetAnalysisProjection = "FSATS.WebDetailedAssetAnalysisProjection.v1";
}

public static class SharedWebTradingBoundary
{
    public const string FsatsOperationalMarketDataOwner = "FSAPMA";
    public const bool AcceptsWebPresentationDataAsAnalysisInput = false;

    public static bool IsPermittedFsatsAnalysisInput(WebMarketDataPurpose purpose)
        => purpose == WebMarketDataPurpose.FsatsOperationalAnalysis;
}

public static class WebPortfolioContractIds
{
    public const string PortfolioViewRequest = "FSATS.WebPortfolioViewRequest.v1";
    public const string PortfolioSummaryProjection = "FSATS.WebPortfolioSummaryProjection.v1";
    public const string PositionCollectionProjection = "FSATS.WebPositionCollectionProjection.v1";
    public const string OrderTradeActivityProjection = "FSATS.WebOrderTradeActivityProjection.v1";
    public const string PortfolioPerformanceProjection = "FSATS.WebPortfolioPerformanceProjection.v1";
    public const string PortfolioProjectionUpdate = "FSATS.WebPortfolioProjectionUpdate.v1";
}

public sealed record WebPageInfo
{
    public string? ContinuationToken { get; }
    public bool HasMore { get; }
    public int? PageSize { get; }

    public WebPageInfo(string? continuationToken, bool hasMore, int? pageSize)
    {
        if (pageSize is <= 0) throw new ArgumentOutOfRangeException(nameof(pageSize));
        if (hasMore && string.IsNullOrWhiteSpace(continuationToken))
            throw new ArgumentException("HAS_MORE_REQUIRES_CONTINUATION_TOKEN", nameof(continuationToken));

        ContinuationToken = string.IsNullOrWhiteSpace(continuationToken) ? null : continuationToken.Trim();
        HasMore = hasMore;
        PageSize = pageSize;
    }
}

public sealed record WebPortfolioViewRequest
{
    public string RequestId { get; }
    public CorrelationId Correlation { get; }
    public IReadOnlyList<BrokerAccountScope> BrokerAccounts { get; }
    public DateTimeOffset RequestedAt { get; }
    public int? PageSize { get; }
    public string? PositionContinuationToken { get; }
    public string? ActivityContinuationToken { get; }
    public string? PerformanceContinuationToken { get; }

    public WebPortfolioViewRequest(
        string requestId,
        CorrelationId correlation,
        IReadOnlyList<BrokerAccountScope> brokerAccounts,
        DateTimeOffset requestedAt,
        int? pageSize = null,
        string? positionContinuationToken = null,
        string? activityContinuationToken = null,
        string? performanceContinuationToken = null)
    {
        if (string.IsNullOrWhiteSpace(requestId)) throw new ArgumentException("WEB_PORTFOLIO_REQUEST_ID_REQUIRED", nameof(requestId));
        if (brokerAccounts is null || brokerAccounts.Count == 0) throw new ArgumentException("BROKER_ACCOUNT_SCOPE_REQUIRED", nameof(brokerAccounts));
        if (brokerAccounts.Any(x => x is null)) throw new ArgumentException("BROKER_ACCOUNT_SCOPE_REQUIRED", nameof(brokerAccounts));
        if (brokerAccounts.Select(x => x.NamespaceKey).Distinct(StringComparer.Ordinal).Count() != brokerAccounts.Count)
            throw new ArgumentException("BROKER_ACCOUNT_SCOPES_MUST_BE_DISTINCT", nameof(brokerAccounts));
        if (pageSize is <= 0) throw new ArgumentOutOfRangeException(nameof(pageSize));
        ValidateOptionalToken(positionContinuationToken, nameof(positionContinuationToken));
        ValidateOptionalToken(activityContinuationToken, nameof(activityContinuationToken));
        ValidateOptionalToken(performanceContinuationToken, nameof(performanceContinuationToken));

        RequestId = requestId.Trim();
        Correlation = correlation;
        BrokerAccounts = Array.AsReadOnly(brokerAccounts.ToArray());
        RequestedAt = requestedAt;
        PageSize = pageSize;
        PositionContinuationToken = NormalizeOptional(positionContinuationToken);
        ActivityContinuationToken = NormalizeOptional(activityContinuationToken);
        PerformanceContinuationToken = NormalizeOptional(performanceContinuationToken);
    }

    public bool HasExactBrokerAccountScope => true;

    private static void ValidateOptionalToken(string? value, string parameter)
    {
        if (value is not null && string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("CONTINUATION_TOKEN_MUST_BE_OPAQUE_NONEMPTY_WHEN_PRESENT", parameter);
    }

    private static string? NormalizeOptional(string? value) => value is null ? null : value.Trim();
}

public sealed record WebProjectionEnvelope
{
    public string ProjectionId { get; }
    public string ContractId { get; }
    public string Version { get; }
    public BrokerAccountScope Account { get; }
    public DateTimeOffset AsOfTime { get; }
    public TruthClassification TruthState { get; }
    public WebFreshnessState FreshnessState { get; }
    public WebProjectionCompleteness Completeness { get; }
    public WebAvailabilityState AvailabilityState { get; }
    public string EvidenceReference { get; }
    public string ReasonCode { get; }
    public string? LimitationDetail { get; }
    public string? CorrectsProjectionId { get; }
    public string? SupersedesProjectionId { get; }

    public WebProjectionEnvelope(
        string projectionId,
        string contractId,
        string version,
        BrokerAccountScope account,
        DateTimeOffset asOfTime,
        TruthClassification truthState,
        WebFreshnessState freshnessState,
        WebProjectionCompleteness completeness,
        WebAvailabilityState availabilityState,
        string evidenceReference,
        string reasonCode,
        string? limitationDetail = null,
        string? correctsProjectionId = null,
        string? supersedesProjectionId = null)
    {
        if (string.IsNullOrWhiteSpace(projectionId)) throw new ArgumentException("PROJECTION_ID_REQUIRED", nameof(projectionId));
        if (string.IsNullOrWhiteSpace(contractId)) throw new ArgumentException("CONTRACT_ID_REQUIRED", nameof(contractId));
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentException("PROJECTION_VERSION_REQUIRED", nameof(version));
        ArgumentNullException.ThrowIfNull(account);
        if (string.IsNullOrWhiteSpace(evidenceReference)) throw new ArgumentException("EVIDENCE_REFERENCE_REQUIRED", nameof(evidenceReference));
        if (string.IsNullOrWhiteSpace(reasonCode)) throw new ArgumentException("REASON_CODE_REQUIRED", nameof(reasonCode));
        if (!string.IsNullOrWhiteSpace(correctsProjectionId) && !string.IsNullOrWhiteSpace(supersedesProjectionId))
            throw new ArgumentException("PROJECTION_CANNOT_CORRECT_AND_SUPERSEDE_SIMULTANEOUSLY");

        ProjectionId = projectionId.Trim();
        ContractId = contractId.Trim();
        Version = version.Trim();
        Account = account;
        AsOfTime = asOfTime;
        TruthState = truthState;
        FreshnessState = freshnessState;
        Completeness = completeness;
        AvailabilityState = availabilityState;
        EvidenceReference = evidenceReference.Trim();
        ReasonCode = reasonCode.Trim();
        LimitationDetail = limitationDetail;
        CorrectsProjectionId = NormalizeOptional(correctsProjectionId);
        SupersedesProjectionId = NormalizeOptional(supersedesProjectionId);
    }

    public bool RequiresNoBusinessPayload
        => AvailabilityState is WebAvailabilityState.Unsupported or WebAvailabilityState.NotApplicable;

    private static string? NormalizeOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

public sealed record WebPortfolioSummaryProjection
{
    public WebProjectionEnvelope Envelope { get; }
    public string Currency { get; }
    public decimal? TotalEquity { get; }
    public decimal? Cash { get; }
    public decimal? MarketValue { get; }
    public decimal? ReservedCapital { get; }
    public decimal? RealizedPnl { get; }
    public decimal? UnrealizedPnl { get; }

    public WebPortfolioSummaryProjection(
        WebProjectionEnvelope envelope,
        string currency,
        decimal? totalEquity,
        decimal? cash,
        decimal? marketValue,
        decimal? reservedCapital,
        decimal? realizedPnl,
        decimal? unrealizedPnl)
    {
        ArgumentNullException.ThrowIfNull(envelope);
        if (string.IsNullOrWhiteSpace(currency)) throw new ArgumentException("PORTFOLIO_CURRENCY_REQUIRED", nameof(currency));
        if (envelope.RequiresNoBusinessPayload && new[] { totalEquity, cash, marketValue, reservedCapital, realizedPnl, unrealizedPnl }.Any(x => x is not null))
            throw new ArgumentException("UNSUPPORTED_OR_NOT_APPLICABLE_PORTFOLIO_SUMMARY_MUST_KEEP_NUMERIC_VALUES_NULL");

        Envelope = envelope;
        Currency = currency.Trim().ToUpperInvariant();
        TotalEquity = totalEquity;
        Cash = cash;
        MarketValue = marketValue;
        ReservedCapital = reservedCapital;
        RealizedPnl = realizedPnl;
        UnrealizedPnl = unrealizedPnl;
    }
}

public sealed record WebPositionItem(
    TradingPositionRef Position,
    TradingInstrumentRef Instrument,
    decimal? Quantity,
    decimal? AverageCost,
    decimal? MarketPrice,
    decimal? MarketValue,
    decimal? UnrealizedPnl,
    string Currency,
    TruthClassification TruthState,
    WebFreshnessState FreshnessState,
    string ReasonCode);

public sealed record WebPositionCollectionProjection
{
    public WebProjectionEnvelope Envelope { get; }
    public IReadOnlyList<WebPositionItem> Positions { get; }
    public WebPageInfo Page { get; }

    public WebPositionCollectionProjection(WebProjectionEnvelope envelope, IReadOnlyList<WebPositionItem> positions, WebPageInfo page)
    {
        ArgumentNullException.ThrowIfNull(envelope);
        ArgumentNullException.ThrowIfNull(positions);
        ArgumentNullException.ThrowIfNull(page);
        if (envelope.RequiresNoBusinessPayload && positions.Count != 0)
            throw new ArgumentException("UNSUPPORTED_OR_NOT_APPLICABLE_POSITION_PROJECTION_MUST_BE_EMPTY", nameof(positions));

        Envelope = envelope;
        Positions = Array.AsReadOnly(positions.ToArray());
        Page = page;
    }
}

public sealed record WebOrderTradeActivityItem(
    TradingOrderRef Order,
    TradingInstrumentRef Instrument,
    WebOrderTradeState State,
    decimal? RequestedQuantity,
    decimal? FilledQuantity,
    decimal? AverageFillPrice,
    string Currency,
    DateTimeOffset EffectiveAt,
    TruthClassification TruthState,
    WebFreshnessState FreshnessState,
    string ReasonCode);

public sealed record WebOrderTradeActivityProjection
{
    public WebProjectionEnvelope Envelope { get; }
    public IReadOnlyList<WebOrderTradeActivityItem> Activity { get; }
    public WebPageInfo Page { get; }

    public WebOrderTradeActivityProjection(WebProjectionEnvelope envelope, IReadOnlyList<WebOrderTradeActivityItem> activity, WebPageInfo page)
    {
        ArgumentNullException.ThrowIfNull(envelope);
        ArgumentNullException.ThrowIfNull(activity);
        ArgumentNullException.ThrowIfNull(page);
        if (envelope.RequiresNoBusinessPayload && activity.Count != 0)
            throw new ArgumentException("UNSUPPORTED_OR_NOT_APPLICABLE_ORDER_ACTIVITY_PROJECTION_MUST_BE_EMPTY", nameof(activity));

        Envelope = envelope;
        Activity = Array.AsReadOnly(activity.ToArray());
        Page = page;
    }
}

public sealed record WebPerformancePoint(
    DateTimeOffset EffectiveAt,
    decimal? Equity,
    decimal? NetPnl,
    decimal? ReturnPercent,
    TruthClassification TruthState,
    WebFreshnessState FreshnessState,
    string ReasonCode);

public sealed record WebPortfolioPerformanceProjection
{
    public WebProjectionEnvelope Envelope { get; }
    public DateTimeOffset PeriodStart { get; }
    public DateTimeOffset PeriodEnd { get; }
    public string Currency { get; }
    public decimal? OpeningEquity { get; }
    public decimal? ClosingEquity { get; }
    public decimal? RealizedPnl { get; }
    public decimal? UnrealizedPnl { get; }
    public decimal? NetPnl { get; }
    public decimal? ReturnPercent { get; }
    public IReadOnlyList<WebPerformancePoint> History { get; }
    public WebPageInfo Page { get; }

    public WebPortfolioPerformanceProjection(
        WebProjectionEnvelope envelope,
        DateTimeOffset periodStart,
        DateTimeOffset periodEnd,
        string currency,
        decimal? openingEquity,
        decimal? closingEquity,
        decimal? realizedPnl,
        decimal? unrealizedPnl,
        decimal? netPnl,
        decimal? returnPercent,
        IReadOnlyList<WebPerformancePoint> history,
        WebPageInfo page)
    {
        ArgumentNullException.ThrowIfNull(envelope);
        if (periodStart == default || periodEnd == default || periodEnd < periodStart) throw new ArgumentException("INVALID_PERFORMANCE_PERIOD");
        if (string.IsNullOrWhiteSpace(currency)) throw new ArgumentException("PERFORMANCE_CURRENCY_REQUIRED", nameof(currency));
        ArgumentNullException.ThrowIfNull(history);
        ArgumentNullException.ThrowIfNull(page);
        if (envelope.RequiresNoBusinessPayload)
        {
            if (new[] { openingEquity, closingEquity, realizedPnl, unrealizedPnl, netPnl, returnPercent }.Any(x => x is not null))
                throw new ArgumentException("UNSUPPORTED_OR_NOT_APPLICABLE_PERFORMANCE_MUST_KEEP_NUMERIC_VALUES_NULL");
            if (history.Count != 0)
                throw new ArgumentException("UNSUPPORTED_OR_NOT_APPLICABLE_PERFORMANCE_HISTORY_MUST_BE_EMPTY", nameof(history));
        }

        Envelope = envelope;
        PeriodStart = periodStart;
        PeriodEnd = periodEnd;
        Currency = currency.Trim().ToUpperInvariant();
        OpeningEquity = openingEquity;
        ClosingEquity = closingEquity;
        RealizedPnl = realizedPnl;
        UnrealizedPnl = unrealizedPnl;
        NetPnl = netPnl;
        ReturnPercent = returnPercent;
        History = Array.AsReadOnly(history.ToArray());
        Page = page;
    }
}

public sealed record WebPortfolioProjectionUpdate
{
    public string UpdateId { get; }
    public long UpdateSequence { get; }
    public WebProjectionUpdateKind UpdateKind { get; }
    public CorrelationId Correlation { get; }
    public BrokerAccountScope Account { get; }
    public IReadOnlyList<string> ChangedProjectionContractIds { get; }
    public string ProjectionVersion { get; }
    public DateTimeOffset EffectiveAt { get; }
    public TruthClassification TruthState { get; }
    public WebFreshnessState FreshnessState { get; }
    public string EvidenceReference { get; }
    public string ReasonCode { get; }
    public string? CorrectsUpdateId { get; }
    public string? SupersedesUpdateId { get; }

    public WebPortfolioProjectionUpdate(
        string updateId,
        long updateSequence,
        WebProjectionUpdateKind updateKind,
        CorrelationId correlation,
        BrokerAccountScope account,
        IReadOnlyList<string> changedProjectionContractIds,
        string projectionVersion,
        DateTimeOffset effectiveAt,
        TruthClassification truthState,
        WebFreshnessState freshnessState,
        string evidenceReference,
        string reasonCode,
        string? correctsUpdateId = null,
        string? supersedesUpdateId = null)
    {
        if (string.IsNullOrWhiteSpace(updateId)) throw new ArgumentException("PORTFOLIO_UPDATE_ID_REQUIRED", nameof(updateId));
        if (updateSequence <= 0) throw new ArgumentOutOfRangeException(nameof(updateSequence));
        ArgumentNullException.ThrowIfNull(account);
        if (changedProjectionContractIds is null || changedProjectionContractIds.Count == 0 || changedProjectionContractIds.Any(string.IsNullOrWhiteSpace) || changedProjectionContractIds.Distinct(StringComparer.Ordinal).Count() != changedProjectionContractIds.Count)
            throw new ArgumentException("CHANGED_PROJECTION_CONTRACT_IDS_MUST_BE_NONEMPTY_DISTINCT", nameof(changedProjectionContractIds));
        if (string.IsNullOrWhiteSpace(projectionVersion)) throw new ArgumentException("PROJECTION_VERSION_REQUIRED", nameof(projectionVersion));
        if (string.IsNullOrWhiteSpace(evidenceReference)) throw new ArgumentException("PORTFOLIO_UPDATE_EVIDENCE_REQUIRED", nameof(evidenceReference));
        if (string.IsNullOrWhiteSpace(reasonCode)) throw new ArgumentException("PORTFOLIO_UPDATE_REASON_REQUIRED", nameof(reasonCode));
        ValidateLineage(updateKind, correctsUpdateId, supersedesUpdateId);

        UpdateId = updateId.Trim();
        UpdateSequence = updateSequence;
        UpdateKind = updateKind;
        Correlation = correlation;
        Account = account;
        ChangedProjectionContractIds = Array.AsReadOnly(changedProjectionContractIds.Select(x => x.Trim()).ToArray());
        ProjectionVersion = projectionVersion.Trim();
        EffectiveAt = effectiveAt;
        TruthState = truthState;
        FreshnessState = freshnessState;
        EvidenceReference = evidenceReference.Trim();
        ReasonCode = reasonCode.Trim();
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
        if (!valid) throw new ArgumentException("PORTFOLIO_UPDATE_LINEAGE_DOES_NOT_MATCH_UPDATE_KIND");
    }

    private static string? NormalizeOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
