namespace Falcon.FSATS.FSAPMA.Domain;

public enum StreamAuthenticationMode
{
    Public,
    ApiCredentialReference,
    ChannelDependent
}

public sealed record ProviderStreamingEndpoint
{
    public string EndpointId { get; }
    public ProviderId Provider { get; }
    public string MarketScope { get; }
    public string ServiceRole { get; }
    public Uri Endpoint { get; }
    public string? PathTemplate { get; }
    public StreamAuthenticationMode AuthenticationMode { get; }
    public bool ConsolidatedMarketTruth { get; }

    public ProviderStreamingEndpoint(
        string endpointId,
        ProviderId provider,
        string marketScope,
        string serviceRole,
        string endpoint,
        StreamAuthenticationMode authenticationMode,
        bool consolidatedMarketTruth,
        string? pathTemplate = null)
    {
        EndpointId = Require(endpointId, nameof(endpointId));
        if (string.IsNullOrWhiteSpace(provider.Value)) throw new ArgumentException("STREAM_PROVIDER_REQUIRED", nameof(provider));
        Provider = new ProviderId(provider.Value.Trim().ToUpperInvariant());
        MarketScope = Require(marketScope, nameof(marketScope)).ToUpperInvariant();
        ServiceRole = Require(serviceRole, nameof(serviceRole)).ToUpperInvariant();
        if (!Uri.TryCreate(endpoint, UriKind.Absolute, out var uri) || uri.Scheme != "wss") throw new ArgumentException("STREAM_WSS_ENDPOINT_REQUIRED", nameof(endpoint));
        Endpoint = uri;
        PathTemplate = string.IsNullOrWhiteSpace(pathTemplate) ? null : pathTemplate.Trim();
        AuthenticationMode = authenticationMode;
        ConsolidatedMarketTruth = consolidatedMarketTruth;
    }

    private static string Require(string value, string parameter)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("STREAM_CATALOG_VALUE_REQUIRED", parameter);
        return value.Trim();
    }
}

public static class ProviderStreamingCatalog
{
    private static readonly IReadOnlyList<ProviderStreamingEndpoint> _all = Array.AsReadOnly(new[]
    {
        new ProviderStreamingEndpoint(
            "BINANCE_SPOT_PUBLIC_TRADE",
            new ProviderId("BINANCE"),
            "CRYPTO_SPOT",
            "MARKET_DATA_STREAM",
            "wss://stream.binance.com:9443",
            StreamAuthenticationMode.Public,
            consolidatedMarketTruth: false,
            pathTemplate: "/ws/{symbol-lowercase}@trade"),

        new ProviderStreamingEndpoint(
            "COINBASE_EXCHANGE_MARKET_DATA",
            new ProviderId("COINBASE"),
            "CRYPTO_SPOT",
            "MARKET_DATA_STREAM",
            "wss://ws-feed.exchange.coinbase.com",
            StreamAuthenticationMode.ChannelDependent,
            consolidatedMarketTruth: false),

        new ProviderStreamingEndpoint(
            "BYBIT_V5_PUBLIC_SPOT",
            new ProviderId("BYBIT"),
            "CRYPTO_SPOT",
            "MARKET_DATA_STREAM",
            "wss://stream.bybit.com/v5/public/spot",
            StreamAuthenticationMode.Public,
            consolidatedMarketTruth: false),

        new ProviderStreamingEndpoint(
            "ALPACA_US_EQUITIES_IEX",
            new ProviderId("ALPACA"),
            "US_EQUITIES",
            "MARKET_DATA_STREAM",
            "wss://stream.data.alpaca.markets/v2/iex",
            StreamAuthenticationMode.ApiCredentialReference,
            consolidatedMarketTruth: false),

        new ProviderStreamingEndpoint(
            "FINNHUB_REALTIME",
            new ProviderId("FINNHUB"),
            "US_EQUITIES_CRYPTO_FX_AS_ENTITLED",
            "MARKET_DATA_STREAM",
            "wss://ws.finnhub.io",
            StreamAuthenticationMode.ApiCredentialReference,
            consolidatedMarketTruth: false,
            pathTemplate: "?token={credential-reference}")
    });

    public static IReadOnlyList<ProviderStreamingEndpoint> All => _all;

    public static ProviderStreamingEndpoint? Find(string endpointId)
    {
        if (string.IsNullOrWhiteSpace(endpointId)) return null;
        var normalized = endpointId.Trim();
        return _all.FirstOrDefault(x => StringComparer.Ordinal.Equals(x.EndpointId, normalized));
    }
}
