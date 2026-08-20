using P = Falcon.FSATS.FSAPMA.Domain;
using PA = Falcon.FSATS.FSAPMA.Application;
using PC = Falcon.FSATS.FSAPMA.Contracts;
using TC = Falcon.FSATS.Trading.Contracts;

internal static class CrossPartSynchronizationAdversarialChecks
{
    internal static void Run()
    {
        ProviderRouteRequiresDistinctCurrentBinding();
        ProviderConfigurationBindsSafeEndpointWithoutGrantingAuthority();
        BrokerAccountWebContractsHaveNoUserPrincipal();
        SharedWebPresentationDataCannotBecomeFsatsAnalysisInput();
        WebAnalysisAndStrategyContractsRemainApplicationOwned();
        WebTruthOrderingPaginationAndLineageRemainExplicit();
    }

    private static void ProviderRouteRequiresDistinctCurrentBinding()
    {
        var legacy = new P.ProviderRouteIdentity(
            new P.ProviderId("ALPACA"), new P.ProviderAccountId("DATA-01"), "PAPER", "MARKET_DATA_STREAM", new P.CredentialReference("CRED-01"));
        if (legacy.HasCurrentRouteBinding)
            throw new InvalidOperationException("LEGACY_PROVIDER_ROUTE_FALSELY_CURRENT");

        var current = new P.ProviderRouteIdentity(
            new P.ProviderId("ALPACA"), new P.ProviderAccountId("DATA-01"), "PAPER", "MARKET_DATA_STREAM",
            new P.ApiInstanceId("API-INSTANCE-01"), new P.ProviderEndpointId("ALPACA_US_EQUITIES_IEX"), new P.CredentialReference("CRED-01"));
        if (!current.HasCurrentRouteBinding)
            throw new InvalidOperationException("CURRENT_PROVIDER_ROUTE_BINDING_REJECTED");

        var candidates = new[]
        {
            new P.ProviderRouteCandidate(legacy, P.CapabilityState.Supported, P.QualityState.Healthy, 100),
            new P.ProviderRouteCandidate(current, P.CapabilityState.Supported, P.QualityState.Healthy, 10)
        };
        var selected = new P.ProviderController().SelectCurrentRoute(candidates);
        if (selected != current)
            throw new InvalidOperationException("CURRENT_PROVIDER_ROUTE_SELECTION_ACCEPTED_INCOMPLETE_LEGACY_ROUTE");
        if (new P.ProviderController().SelectRoute(candidates) != current)
            throw new InvalidOperationException("HISTORICAL_SELECT_ROUTE_ALIAS_BYPASSED_CURRENT_ROUTE_RULES");

        var now = DateTimeOffset.UtcNow;
        var projection = new PC.OperationalDataProjection(
            new PC.ObservationId("obs"), new PC.ProviderId("ALPACA"), new PC.ProducerInstrumentId("ALPACA", "AAPL"),
            new PC.DataProductId("last-price"), 100m, now, now, PC.DataTruthState.Current, "prov", "1",
            new PC.ProviderAccountId("DATA-01"), "PAPER", "MARKET_DATA_STREAM", "CRED-01",
            new PC.ApiInstanceId("API-INSTANCE-01"), new PC.ProviderEndpointId("ALPACA_US_EQUITIES_IEX"));
        if (!projection.HasCurrentProviderRouteIdentity)
            throw new InvalidOperationException("CURRENT_PROVIDER_PROJECTION_BINDING_INCOMPLETE");
    }

    private static void ProviderConfigurationBindsSafeEndpointWithoutGrantingAuthority()
    {
        var snapshot = new PA.ProviderOperationalConfigurationSnapshot(
            PA.ProviderOperationalConfiguration.ApplicationId, "ALPACA", "DATA-01", "MARKET_DATA_STREAM", "PAPER",
            "CFG", "1", 1, "DIGEST", "EVIDENCE", PA.ProviderConfigurationEvidenceIntegrity.Valid,
            PA.ProviderConfigurationCompatibility.Compatible, "CAP", "QUOTA", "CRED-01", false, false, false, false, true)
        {
            ApiInstanceId = "API-INSTANCE-01",
            EndpointId = "ALPACA_US_EQUITIES_IEX",
            EndpointBaseUrl = "wss://stream.data.alpaca.markets/v2/iex"
        };
        var result = PA.ProviderOperationalConfiguration.AssessCurrentRouteBinding(snapshot, 1);
        if (!result.Accepted || result.GrantsRuntimeAuthority || !snapshot.HasCurrentProviderRouteBinding)
            throw new InvalidOperationException("CURRENT_PROVIDER_CONFIG_BINDING_OR_AUTHORITY_FAILED");

        var unsafeUrl = snapshot with { EndpointBaseUrl = "wss://user:secret@example.com/path" };
        if (PA.ProviderOperationalConfiguration.AssessCurrentRouteBinding(unsafeUrl, 1).Accepted)
            throw new InvalidOperationException("PROVIDER_ENDPOINT_EMBEDDED_SECRET_ACCEPTED");

        var endpointMismatch = snapshot with { EndpointBaseUrl = "wss://stream.bybit.com/v5/public/spot" };
        if (PA.ProviderOperationalConfiguration.AssessCurrentRouteBinding(endpointMismatch, 1).Accepted)
            throw new InvalidOperationException("PROVIDER_ENDPOINT_ID_URL_MISMATCH_ACCEPTED");

        var providerMismatch = snapshot with { ProviderId = "BYBIT" };
        if (PA.ProviderOperationalConfiguration.AssessCurrentRouteBinding(providerMismatch, 1).Accepted)
            throw new InvalidOperationException("PROVIDER_ENDPOINT_ID_PROVIDER_MISMATCH_ACCEPTED");
    }

    private static void BrokerAccountWebContractsHaveNoUserPrincipal()
    {
        var accountA = new TC.BrokerAccountScope(new TC.BrokerId("alpaca"), new TC.BrokerAccountId("A"), "paper");
        var accountB = new TC.BrokerAccountScope(new TC.BrokerId("alpaca"), new TC.BrokerAccountId("B"), "paper");
        if (accountA == accountB || accountA.NamespaceKey == accountB.NamespaceKey)
            throw new InvalidOperationException("BROKER_ACCOUNTS_COLLAPSED");

        var request = new TC.WebPortfolioViewRequest("REQ-1", new TC.CorrelationId("CORR-1"), new[] { accountA, accountB }, DateTimeOffset.UtcNow, 100);
        if (!request.HasExactBrokerAccountScope || TC.WebPortfolioContractIds.PortfolioViewRequest != "FSATS.WebPortfolioViewRequest.v1")
            throw new InvalidOperationException("WEB_PORTFOLIO_ACCOUNT_SCOPE_NOT_IMPLEMENTATION_READY");

        ExpectThrows<ArgumentException>(() =>
            _ = new TC.WebPortfolioViewRequest("REQ-DUP", new TC.CorrelationId("CORR-DUP"), new[] { accountA, accountA }, DateTimeOffset.UtcNow, 100),
            "WEB_PORTFOLIO_DUPLICATE_ACCOUNT_SCOPE_ACCEPTED");
        ExpectThrows<ArgumentOutOfRangeException>(() =>
            _ = new TC.WebPortfolioViewRequest("REQ-PAGE", new TC.CorrelationId("CORR-PAGE"), new[] { accountA }, DateTimeOffset.UtcNow, 0),
            "WEB_PORTFOLIO_NONPOSITIVE_PAGE_SIZE_ACCEPTED");
    }

    private static void SharedWebPresentationDataCannotBecomeFsatsAnalysisInput()
    {
        if (TC.SharedWebTradingBoundary.AcceptsWebPresentationDataAsAnalysisInput)
            throw new InvalidOperationException("WEB_PRESENTATION_DATA_FALSELY_ACCEPTED_AS_FSATS_ANALYSIS_INPUT");

        if (TC.SharedWebTradingBoundary.IsPermittedFsatsAnalysisInput(TC.WebMarketDataPurpose.PresentationOnly))
            throw new InvalidOperationException("WEB_PRESENTATION_PURPOSE_BYPASSED_FSAPMA_OPERATIONAL_BOUNDARY");

        if (!TC.SharedWebTradingBoundary.IsPermittedFsatsAnalysisInput(TC.WebMarketDataPurpose.FsatsOperationalAnalysis))
            throw new InvalidOperationException("FSATS_OPERATIONAL_ANALYSIS_INPUT_REJECTED");

        if (TC.SharedWebTradingBoundary.FsatsOperationalMarketDataOwner != "FSAPMA")
            throw new InvalidOperationException("FSATS_OPERATIONAL_MARKET_DATA_OWNER_DRIFTED");
    }

    private static void WebAnalysisAndStrategyContractsRemainApplicationOwned()
    {
        if (TC.WebOnDemandAnalysisContractIds.RequestV1 != "FSATS.WebOnDemandAnalysisRequest.v1"
            || TC.WebOnDemandAnalysisContractIds.ResultV1 != "FSATS.WebOnDemandAnalysisResult.v1"
            || TC.WebOnDemandAnalysisContractIds.DetailedAssetAnalysisProjectionV1 != "FSATS.WebDetailedAssetAnalysisProjection.v1")
            throw new InvalidOperationException("WEB_ON_DEMAND_ANALYSIS_CANONICAL_FCR_IDENTITY_DRIFTED");

        if (TC.WebStrategyCatalogContractIds.RequestV1 != "FSATS.WebStrategyCatalogRequest.v1"
            || TC.WebStrategyCatalogContractIds.ProjectionV1 != "FSATS.WebStrategyCatalogProjection.v1"
            || TC.WebStrategyCatalogContractIds.UpdateV1 != "FSATS.WebStrategyCatalogUpdate.v1")
            throw new InvalidOperationException("WEB_STRATEGY_CATALOG_CONTRACT_IDENTITY_DRIFTED");

        var forbiddenTokens = new[] { "provider", "url", "endpoint", "credential", "apikey", "secret", "rawmarket", "rawdata" };
        var requestProperties = typeof(TC.WebOnDemandAnalysisRequest).GetProperties().Select(x => x.Name.ToLowerInvariant()).ToArray();
        if (requestProperties.Any(name => forbiddenTokens.Any(token => name.Contains(token, StringComparison.Ordinal))))
            throw new InvalidOperationException("WEB_ANALYSIS_REQUEST_EXPOSES_PROVIDER_OR_RAW_DATA_CONTROL");

        var catalogProperties = typeof(TC.WebStrategyCatalogRequest).GetProperties().Select(x => x.Name.ToLowerInvariant()).ToArray();
        if (catalogProperties.Any(name => forbiddenTokens.Any(token => name.Contains(token, StringComparison.Ordinal))))
            throw new InvalidOperationException("WEB_STRATEGY_CATALOG_REQUEST_EXPOSES_PROVIDER_OR_RAW_DATA_CONTROL");

        var account = new TC.BrokerAccountScope(new TC.BrokerId("alpaca"), new TC.BrokerAccountId("A"), "paper");
        var genericRisk = new TC.WebTradingRiskProjection(TC.WebRiskBand.Moderate, 50m, false, null, "GENERAL_ANALYSIS");
        var accountRisk = new TC.WebTradingRiskProjection(TC.WebRiskBand.Moderate, 50m, true, account, "ACCOUNT_AWARE_ANALYSIS");
        if (genericRisk.IsAccountAware || genericRisk.AccountScope is not null || !accountRisk.IsAccountAware || accountRisk.AccountScope != account)
            throw new InvalidOperationException("WEB_ANALYSIS_ACCOUNT_AWARE_RISK_SCOPE_COLLAPSED");

        ExpectThrows<ArgumentException>(() =>
            _ = new TC.WebTradingRiskProjection(TC.WebRiskBand.Moderate, 50m, true, null, "INVALID_ACCOUNT_AWARE"),
            "ACCOUNT_AWARE_RISK_WITHOUT_SCOPE_ACCEPTED");
        ExpectThrows<ArgumentException>(() =>
            _ = new TC.WebTradingRiskProjection(TC.WebRiskBand.Moderate, 50m, false, account, "INVALID_GENERAL_RISK"),
            "GENERAL_RISK_WITH_ACCOUNT_SCOPE_ACCEPTED");

        if (TC.WebAnalysisLifecycleState.Accepted == TC.WebAnalysisLifecycleState.Completed
            || TC.WebAnalysisLifecycleState.CancelRequested == TC.WebAnalysisLifecycleState.Canceled
            || TC.WebAnalysisLifecycleState.Failed == TC.WebAnalysisLifecycleState.Unavailable
            || TC.WebAnalysisLifecycleState.Failed == TC.WebAnalysisLifecycleState.Rejected)
            throw new InvalidOperationException("WEB_ANALYSIS_LIFECYCLE_STATES_COLLAPSED");

        var notApplicable = new TC.WebStrategyCatalogItem("S-1", "Strategy 1", "SCH-1", "School 1", TC.WebCatalogApplicabilityState.NotApplicable, true, false, "NOT_APPLICABLE");
        if (!notApplicable.Visible || notApplicable.Enabled)
            throw new InvalidOperationException("WEB_NOT_APPLICABLE_STRATEGY_SELECTOR_RULE_BROKEN");

        ExpectThrows<ArgumentException>(() =>
            _ = new TC.WebStrategyCatalogItem("S-2", "Strategy 2", "SCH-1", "School 1", TC.WebCatalogApplicabilityState.NotApplicable, true, true, "NOT_APPLICABLE"),
            "WEB_NOT_APPLICABLE_ENABLED_STRATEGY_ACCEPTED");
        ExpectThrows<ArgumentException>(() =>
            _ = new TC.WebStrategyCatalogItem("S-3", "Strategy 3", "SCH-1", "School 1", TC.WebCatalogApplicabilityState.NotApplicable, false, false, "NOT_APPLICABLE"),
            "WEB_NOT_APPLICABLE_HIDDEN_STRATEGY_ACCEPTED");
    }

    private static void WebTruthOrderingPaginationAndLineageRemainExplicit()
    {
        if (TC.WebOrderTradeState.Accepted == TC.WebOrderTradeState.PartiallyFilled)
            throw new InvalidOperationException("ORDER_ACCEPTED_COLLAPSED_WITH_PARTIAL_FILL");
        if (TC.WebOrderTradeState.UnknownBrokerOutcome == TC.WebOrderTradeState.Rejected)
            throw new InvalidOperationException("UNKNOWN_BROKER_OUTCOME_COLLAPSED_WITH_REJECTION");
        if (TC.TruthClassification.LastKnown == TC.TruthClassification.Current || TC.WebFreshnessState.Stale == TC.WebFreshnessState.Current)
            throw new InvalidOperationException("WEB_CURRENTNESS_STATES_COLLAPSED");

        var page = new TC.WebPageInfo("opaque-next", true, 100);
        if (!page.HasMore || string.IsNullOrWhiteSpace(page.ContinuationToken))
            throw new InvalidOperationException("WEB_PAGINATION_CONTINUATION_NOT_EXPLICIT");
        ExpectThrows<ArgumentException>(() => _ = new TC.WebPageInfo(null, true, 100), "WEB_HAS_MORE_WITHOUT_TOKEN_ACCEPTED");
        ExpectThrows<ArgumentOutOfRangeException>(() => _ = new TC.WebPageInfo(null, false, 0), "WEB_NONPOSITIVE_PAGE_SIZE_ACCEPTED");

        var account = new TC.BrokerAccountScope(new TC.BrokerId("ALPACA"), new TC.BrokerAccountId("A"), "PAPER");
        _ = new TC.WebPortfolioProjectionUpdate(
            "UP-1", 1, TC.WebProjectionUpdateKind.Correction, new TC.CorrelationId("C-1"), account,
            new[] { TC.WebPortfolioContractIds.PortfolioSummaryProjection }, "1", DateTimeOffset.UtcNow,
            TC.TruthClassification.Current, TC.WebFreshnessState.Current, "EV-1", "CORRECTION", correctsUpdateId: "UP-0");
        ExpectThrows<ArgumentException>(() =>
            _ = new TC.WebPortfolioProjectionUpdate(
                "UP-2", 2, TC.WebProjectionUpdateKind.Correction, new TC.CorrelationId("C-2"), account,
                new[] { TC.WebPortfolioContractIds.PortfolioSummaryProjection }, "1", DateTimeOffset.UtcNow,
                TC.TruthClassification.Current, TC.WebFreshnessState.Current, "EV-2", "INVALID_CORRECTION"),
            "WEB_CORRECTION_WITHOUT_LINEAGE_ACCEPTED");

        _ = new TC.WebStrategyCatalogUpdate(
            "SU-1", 1, new TC.CorrelationId("SC-1"), new[] { "S-1" }, TC.WebProjectionUpdateKind.Supersession,
            TC.TruthClassification.Current, TC.WebFreshnessState.Current, "SEV-1", "SUPERSESSION", DateTimeOffset.UtcNow,
            supersedesUpdateId: "SU-0");
        ExpectThrows<ArgumentException>(() =>
            _ = new TC.WebStrategyCatalogUpdate(
                "SU-2", 2, new TC.CorrelationId("SC-2"), new[] { "S-1" }, TC.WebProjectionUpdateKind.Ordinary,
                TC.TruthClassification.Current, TC.WebFreshnessState.Current, "SEV-2", "INVALID_ORDINARY", DateTimeOffset.UtcNow,
                correctsUpdateId: "SU-1"),
            "WEB_ORDINARY_UPDATE_WITH_CORRECTION_LINEAGE_ACCEPTED");
    }

    private static void ExpectThrows<TException>(Action action, string failureCode) where TException : Exception
    {
        try
        {
            action();
        }
        catch (TException)
        {
            return;
        }
        throw new InvalidOperationException(failureCode);
    }
}
