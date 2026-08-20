using P = Falcon.FSATS.FSAPMA.Domain;

internal static class ProviderStreamingCatalogAdversarialChecks
{
    internal static void Run()
    {
        var entries = P.ProviderStreamingCatalog.All;
        if (entries.Count != 5) throw new InvalidOperationException("STREAM_CATALOG_EXPECTED_FIVE_OWNER_REGISTERED_ENDPOINTS");
        if (entries.Select(x => x.EndpointId).Distinct(StringComparer.Ordinal).Count() != entries.Count)
            throw new InvalidOperationException("STREAM_CATALOG_DUPLICATE_ENDPOINT_ID");
        if (entries.Any(x => x.Endpoint.Scheme != "wss"))
            throw new InvalidOperationException("STREAM_CATALOG_NON_WSS_ENDPOINT");
        if (entries.Any(x => x.ConsolidatedMarketTruth))
            throw new InvalidOperationException("STREAM_CATALOG_FALSE_CONSOLIDATED_TRUTH_CLAIM");

        Require("BINANCE_SPOT_PUBLIC_TRADE", "BINANCE", P.StreamAuthenticationMode.Public);
        Require("COINBASE_EXCHANGE_MARKET_DATA", "COINBASE", P.StreamAuthenticationMode.ChannelDependent);
        Require("BYBIT_V5_PUBLIC_SPOT", "BYBIT", P.StreamAuthenticationMode.Public);
        Require("ALPACA_US_EQUITIES_IEX", "ALPACA", P.StreamAuthenticationMode.ApiCredentialReference);
        Require("FINNHUB_REALTIME", "FINNHUB", P.StreamAuthenticationMode.ApiCredentialReference);

        var alpaca = P.ProviderStreamingCatalog.Find("ALPACA_US_EQUITIES_IEX")!;
        if (!StringComparer.Ordinal.Equals(alpaca.MarketScope, "US_EQUITIES") || alpaca.ConsolidatedMarketTruth)
            throw new InvalidOperationException("STREAM_CATALOG_ALPACA_IEX_FALSE_SIP_SEMANTICS");

        if (P.ProviderStreamingCatalog.Find("UNKNOWN") is not null)
            throw new InvalidOperationException("STREAM_CATALOG_UNKNOWN_ENDPOINT_RESOLVED");

        CompositeIdentityEncodingAdversarialChecks.Run();
        OperationalDataDeliveryAmbiguityAdversarialChecks.Run();
        ProviderStreamContinuityAdversarialChecks.Run();
        CrossPartSynchronizationAdversarialChecks.Run();
        WebContractSerializationAdversarialChecks.Run();
        DetailedAnalysisContractAdversarialChecks.Run();
        IncidentShadowMonitoringAdversarialChecks.Run();
        IncidentProtectionTruthAdversarialChecks.Run();
        IncidentOrderFollowupAdversarialChecks.Run();
        PortfolioNullSemanticsAdversarialChecks.Run();
        CompatibilityWarningAdversarialChecks.Run();
    }

    private static void Require(string endpointId, string provider, P.StreamAuthenticationMode authenticationMode)
    {
        var entry = P.ProviderStreamingCatalog.Find(endpointId) ?? throw new InvalidOperationException($"STREAM_CATALOG_MISSING:{endpointId}");
        if (!StringComparer.Ordinal.Equals(entry.Provider.Value, provider) || entry.AuthenticationMode != authenticationMode)
            throw new InvalidOperationException($"STREAM_CATALOG_BINDING_MISMATCH:{endpointId}");
    }
}
