namespace Falcon.FSATS.FSTSimA.Contracts;

public enum SimulationTruthClass { HistoricalReplay, Synthetic, FaultInjected, QualificationEvidence }
public readonly record struct ScenarioId(string Value);

public sealed record SimulationScope
{
    public string ScopeKey { get; }
    public string? BrokerId { get; }
    public string? BrokerAccountId { get; }
    public string? Environment { get; }
    public string? ProviderId { get; }
    public string? ProviderAccountId { get; }

    public SimulationScope(string scopeKey, string? brokerId = null, string? brokerAccountId = null, string? environment = null, string? providerId = null, string? providerAccountId = null)
    {
        if (string.IsNullOrWhiteSpace(scopeKey)) throw new ArgumentException("SIMULATION_SCOPE_KEY_REQUIRED", nameof(scopeKey));
        ScopeKey = scopeKey.Trim().ToUpperInvariant();
        BrokerId = Normalize(brokerId)?.ToUpperInvariant();
        BrokerAccountId = Normalize(brokerAccountId);
        Environment = Normalize(environment)?.ToUpperInvariant();
        ProviderId = Normalize(providerId)?.ToUpperInvariant();
        ProviderAccountId = Normalize(providerAccountId);
        if ((BrokerId is null) != (BrokerAccountId is null)) throw new ArgumentException("BROKER_AND_ACCOUNT_MUST_BE_BOUND_TOGETHER");
        if (BrokerId is not null && Environment is null) throw new ArgumentException("BROKER_ACCOUNT_ENVIRONMENT_REQUIRED");
        if ((ProviderId is null) != (ProviderAccountId is null)) throw new ArgumentException("PROVIDER_AND_PROVIDER_ACCOUNT_MUST_BE_BOUND_TOGETHER");
    }

    public static SimulationScope Global(string scopeKey = "GLOBAL") => new(scopeKey);
    public string CanonicalKey => string.Join('|', Part(ScopeKey), Part(BrokerId), Part(BrokerAccountId), Part(Environment), Part(ProviderId), Part(ProviderAccountId));
    private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    private static string Part(string? value) => Uri.EscapeDataString(value ?? string.Empty);
}

public sealed record SimulationRequest(ScenarioId ScenarioId, int Seed, string RequestingApplication, string Purpose, SimulationTruthClass Classification, DateTimeOffset RequestedAt, SimulationScope? Scope = null)
{
    public SimulationScope EffectiveScope => Scope ?? SimulationScope.Global();
}

public sealed record ValidationEvidence(ScenarioId ScenarioId, int Seed, string EvidenceId, bool Reproducible, decimal FidelityScore, string Limitations, string ReadinessRecommendation, DateTimeOffset CompletedAt, string ScopeKey = "GLOBAL");
