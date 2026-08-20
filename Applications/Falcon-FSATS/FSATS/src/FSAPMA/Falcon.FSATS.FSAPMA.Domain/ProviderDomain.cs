namespace Falcon.FSATS.FSAPMA.Domain;

public enum CapabilityState { Supported, Unsupported, Conditional, Unknown }
public enum QualityState { Healthy, Degraded, Conflicted, Stale, Unknown, Unavailable }
public readonly record struct ProviderId(string Value);
public readonly record struct ProviderAccountId(string Value);
public readonly record struct ApiInstanceId(string Value);
public readonly record struct ProviderEndpointId(string Value);
public readonly record struct DataProductId(string Value);
public readonly record struct CredentialReference(string Value);
public readonly record struct ProviderQuotaPoolId(string Value);

public sealed record ProviderRouteIdentity
{
    public ProviderId Provider { get; }
    public ProviderAccountId Account { get; }
    public string Environment { get; }
    public string ServiceRole { get; }
    public ApiInstanceId ApiInstance { get; }
    public ProviderEndpointId Endpoint { get; }
    public CredentialReference CredentialReference { get; }

    public ProviderRouteIdentity(
        ProviderId provider,
        ProviderAccountId account,
        string environment,
        string serviceRole,
        ApiInstanceId apiInstance,
        ProviderEndpointId endpoint,
        CredentialReference credentialReference)
    {
        if (string.IsNullOrWhiteSpace(provider.Value)) throw new ArgumentException("PROVIDER_ID_REQUIRED", nameof(provider));
        if (string.IsNullOrWhiteSpace(account.Value)) throw new ArgumentException("PROVIDER_ACCOUNT_ID_REQUIRED", nameof(account));
        if (string.IsNullOrWhiteSpace(environment)) throw new ArgumentException("PROVIDER_ENVIRONMENT_REQUIRED", nameof(environment));
        if (string.IsNullOrWhiteSpace(serviceRole)) throw new ArgumentException("PROVIDER_SERVICE_ROLE_REQUIRED", nameof(serviceRole));
        if (string.IsNullOrWhiteSpace(apiInstance.Value)) throw new ArgumentException("PROVIDER_API_INSTANCE_ID_REQUIRED", nameof(apiInstance));
        if (string.IsNullOrWhiteSpace(endpoint.Value)) throw new ArgumentException("PROVIDER_ENDPOINT_ID_REQUIRED", nameof(endpoint));
        if (string.IsNullOrWhiteSpace(credentialReference.Value)) throw new ArgumentException("CREDENTIAL_REFERENCE_REQUIRED", nameof(credentialReference));

        Provider = new ProviderId(provider.Value.Trim().ToUpperInvariant());
        Account = new ProviderAccountId(account.Value.Trim());
        Environment = environment.Trim().ToUpperInvariant();
        ServiceRole = serviceRole.Trim().ToUpperInvariant();
        ApiInstance = new ApiInstanceId(apiInstance.Value.Trim());
        Endpoint = new ProviderEndpointId(endpoint.Value.Trim());
        CredentialReference = new CredentialReference(credentialReference.Value.Trim());
    }

    public ProviderRouteIdentity(
        ProviderId provider,
        ProviderAccountId account,
        string environment,
        string serviceRole,
        CredentialReference credentialReference)
    {
        if (string.IsNullOrWhiteSpace(provider.Value)) throw new ArgumentException("PROVIDER_ID_REQUIRED", nameof(provider));
        if (string.IsNullOrWhiteSpace(account.Value)) throw new ArgumentException("PROVIDER_ACCOUNT_ID_REQUIRED", nameof(account));
        if (string.IsNullOrWhiteSpace(environment)) throw new ArgumentException("PROVIDER_ENVIRONMENT_REQUIRED", nameof(environment));
        if (string.IsNullOrWhiteSpace(serviceRole)) throw new ArgumentException("PROVIDER_SERVICE_ROLE_REQUIRED", nameof(serviceRole));
        if (string.IsNullOrWhiteSpace(credentialReference.Value)) throw new ArgumentException("CREDENTIAL_REFERENCE_REQUIRED", nameof(credentialReference));

        Provider = new ProviderId(provider.Value.Trim().ToUpperInvariant());
        Account = new ProviderAccountId(account.Value.Trim());
        Environment = environment.Trim().ToUpperInvariant();
        ServiceRole = serviceRole.Trim().ToUpperInvariant();
        ApiInstance = default;
        Endpoint = default;
        CredentialReference = new CredentialReference(credentialReference.Value.Trim());
    }

    public bool HasCurrentRouteBinding
        => !string.IsNullOrWhiteSpace(Provider.Value)
           && !string.IsNullOrWhiteSpace(Account.Value)
           && !string.IsNullOrWhiteSpace(Environment)
           && !string.IsNullOrWhiteSpace(ServiceRole)
           && !string.IsNullOrWhiteSpace(ApiInstance.Value)
           && !string.IsNullOrWhiteSpace(Endpoint.Value)
           && !string.IsNullOrWhiteSpace(CredentialReference.Value);

    public string NamespaceKey => string.Join('|',
        Part(Provider.Value), Part(Account.Value), Part(Environment), Part(ServiceRole),
        Part(ApiInstance.Value ?? string.Empty), Part(Endpoint.Value ?? string.Empty), Part(CredentialReference.Value));

    private static string Part(string value) => Uri.EscapeDataString(value);
}

public sealed record ProviderProfile(ProviderId Id, IReadOnlySet<string> Markets, IReadOnlySet<DataProductId> Products, bool Enabled);
public sealed record EntitlementState(ProviderId Provider, ProviderAccountId Account, DataProductId Product, CapabilityState Capability, DateTimeOffset ObservedAt, CredentialReference? CredentialReference);
public sealed record QualityAssessment(QualityState State, decimal Confidence, string ReasonCode, DateTimeOffset ObservedAt);
public sealed record ProviderRouteCandidate(ProviderRouteIdentity Route, CapabilityState Capability, QualityState Quality, int ReliabilityScore);

public sealed class ProviderRegistry
{
    private readonly Dictionary<ProviderId, ProviderProfile> _providers = new();
    public bool Register(ProviderProfile profile) => _providers.TryAdd(profile.Id, profile);
    public ProviderProfile? Find(ProviderId id) => _providers.TryGetValue(id, out var value) ? value : null;
}

public sealed class AnomalyDetector
{
    public QualityAssessment Evaluate(decimal value, decimal? reference, DateTimeOffset observedAt, DateTimeOffset now, TimeSpan maxAge)
    {
        if (maxAge < TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(maxAge));
        if (observedAt == default || now == default || observedAt > now)
            return new(QualityState.Unknown, 0m, "CLOCK_INVALID", observedAt);
        if (now - observedAt > maxAge) return new(QualityState.Stale, 1m, "STALE", observedAt);
        if (reference is null) return new(QualityState.Unknown, 0m, "NO_REFERENCE", observedAt);

        try
        {
            var denominator = Math.Max(Math.Abs(reference.Value), 0.00000001m);
            var relative = checked(Math.Abs(checked(value - reference.Value)) / denominator);
            return relative > 0.05m
                ? new(QualityState.Conflicted, Math.Min(1m, relative), "CROSS_SOURCE_DEVIATION", observedAt)
                : new(QualityState.Healthy, 1m - relative, "WITHIN_TOLERANCE", observedAt);
        }
        catch (OverflowException)
        {
            return new(QualityState.Unknown, 0m, "ARITHMETIC_OVERFLOW", observedAt);
        }
    }
}

public sealed class QuotaLedger
{
    private readonly object _gate = new();
    private readonly Dictionary<ProviderQuotaPoolId, int> _remainingByPool = new();
    private readonly Dictionary<ProviderRouteIdentity, HashSet<ProviderQuotaPoolId>> _explicitRoutePools = new();

    public void BindRouteToPool(ProviderRouteIdentity route, ProviderQuotaPoolId pool)
    {
        ArgumentNullException.ThrowIfNull(route);
        var normalizedPool = NormalizePool(pool);
        lock (_gate)
        {
            if (!_explicitRoutePools.TryGetValue(route, out var pools))
            {
                pools = new HashSet<ProviderQuotaPoolId>();
                _explicitRoutePools[route] = pools;
            }
            pools.Add(normalizedPool);
        }
    }

    public void SetPoolWindow(ProviderQuotaPoolId pool, int remaining)
    {
        var normalizedPool = NormalizePool(pool);
        if (remaining < 0) throw new ArgumentOutOfRangeException(nameof(remaining));
        lock (_gate) _remainingByPool[normalizedPool] = remaining;
    }

    public void SetSharedWebFsapmaWindow(ProviderQuotaPoolId pool, int upstreamAvailableUnits)
    {
        var normalizedPool = NormalizePool(pool);
        if (upstreamAvailableUnits < 0) throw new ArgumentOutOfRangeException(nameof(upstreamAvailableUnits));
        var fsapmaMaximum = upstreamAvailableUnits / 2;
        lock (_gate) _remainingByPool[normalizedPool] = fsapmaMaximum;
    }

    public void SetWindow(ProviderRouteIdentity route, int remaining)
    {
        ArgumentNullException.ThrowIfNull(route);
        if (remaining < 0) throw new ArgumentOutOfRangeException(nameof(remaining));
        lock (_gate) _remainingByPool[ResolveSinglePoolNoLock(route)] = remaining;
    }

    public bool TryConsume(ProviderRouteIdentity route, int units = 1)
    {
        ArgumentNullException.ThrowIfNull(route);
        if (units <= 0) return false;
        lock (_gate)
        {
            var pools = ResolvePoolsNoLock(route);
            foreach (var pool in pools)
                if (!_remainingByPool.TryGetValue(pool, out var remaining) || remaining < units) return false;
            foreach (var pool in pools)
                _remainingByPool[pool] = checked(_remainingByPool[pool] - units);
            return true;
        }
    }

    public int Remaining(ProviderRouteIdentity route)
    {
        ArgumentNullException.ThrowIfNull(route);
        lock (_gate)
        {
            var pools = ResolvePoolsNoLock(route);
            var remaining = int.MaxValue;
            foreach (var pool in pools)
            {
                if (!_remainingByPool.TryGetValue(pool, out var poolRemaining)) return 0;
                remaining = Math.Min(remaining, poolRemaining);
            }
            return remaining == int.MaxValue ? 0 : remaining;
        }
    }

    public int Remaining(ProviderQuotaPoolId pool)
    {
        var normalizedPool = NormalizePool(pool);
        lock (_gate) return _remainingByPool.TryGetValue(normalizedPool, out var remaining) ? remaining : 0;
    }

    public ProviderQuotaPoolId ResolvePool(ProviderRouteIdentity route)
    {
        ArgumentNullException.ThrowIfNull(route);
        lock (_gate)
        {
            var pools = ResolvePoolsNoLock(route);
            if (pools.Count != 1) throw new InvalidOperationException("PROVIDER_ROUTE_HAS_MULTIPLE_QUOTA_DIMENSIONS");
            return pools[0];
        }
    }

    public IReadOnlyList<ProviderQuotaPoolId> ResolvePools(ProviderRouteIdentity route)
    {
        ArgumentNullException.ThrowIfNull(route);
        lock (_gate) return ResolvePoolsNoLock(route).ToArray();
    }

    public bool HasExplicitPoolBinding(ProviderRouteIdentity route)
    {
        ArgumentNullException.ThrowIfNull(route);
        lock (_gate) return _explicitRoutePools.TryGetValue(route, out var pools) && pools.Count > 0;
    }

    private IReadOnlyList<ProviderQuotaPoolId> ResolvePoolsNoLock(ProviderRouteIdentity route)
    {
        if (_explicitRoutePools.TryGetValue(route, out var pools) && pools.Count > 0)
            return pools.OrderBy(x => x.Value, StringComparer.Ordinal).ToArray();
        return new[] { UnknownProviderPool(route.Provider) };
    }

    private ProviderQuotaPoolId ResolveSinglePoolNoLock(ProviderRouteIdentity route)
    {
        var pools = ResolvePoolsNoLock(route);
        if (pools.Count != 1) throw new InvalidOperationException("PROVIDER_ROUTE_HAS_MULTIPLE_QUOTA_DIMENSIONS");
        return pools[0];
    }

    private static ProviderQuotaPoolId UnknownProviderPool(ProviderId provider)
    {
        if (string.IsNullOrWhiteSpace(provider.Value)) throw new ArgumentException("PROVIDER_ID_REQUIRED", nameof(provider));
        return new ProviderQuotaPoolId($"UNKNOWN_PROVIDER_SCOPE:{provider.Value.Trim().ToUpperInvariant()}");
    }

    private static ProviderQuotaPoolId NormalizePool(ProviderQuotaPoolId pool)
    {
        if (string.IsNullOrWhiteSpace(pool.Value)) throw new ArgumentException("PROVIDER_QUOTA_POOL_ID_REQUIRED", nameof(pool));
        return new ProviderQuotaPoolId(pool.Value.Trim().ToUpperInvariant());
    }
}

public sealed class ProviderController
{
    public ProviderId? Select(IReadOnlyList<(ProviderId Provider, CapabilityState Capability, QualityState Quality, int ReliabilityScore)> candidates)
        => candidates.Where(x => x.Capability == CapabilityState.Supported && x.Quality == QualityState.Healthy)
            .OrderByDescending(x => x.ReliabilityScore)
            .Select(x => (ProviderId?)x.Provider)
            .FirstOrDefault();

    public ProviderRouteIdentity? SelectRoute(IReadOnlyList<ProviderRouteCandidate> candidates)
        => SelectCurrentRoute(candidates);

    public ProviderRouteIdentity? SelectCurrentRoute(IReadOnlyList<ProviderRouteCandidate> candidates)
        => candidates.Where(x => x.Route.HasCurrentRouteBinding && x.Capability == CapabilityState.Supported && x.Quality == QualityState.Healthy)
            .OrderByDescending(x => x.ReliabilityScore)
            .ThenBy(x => x.Route.NamespaceKey, StringComparer.Ordinal)
            .Select(x => x.Route)
            .FirstOrDefault();
}
