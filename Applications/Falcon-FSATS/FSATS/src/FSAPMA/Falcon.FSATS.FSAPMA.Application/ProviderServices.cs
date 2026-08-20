using Falcon.FSATS.FSAPMA.Domain;

namespace Falcon.FSATS.FSAPMA.Application;

public interface IProviderEgressPort
{
    ValueTask<ProviderFetchResult> FetchAsync(ProviderRouteIdentity route, DataProductId product, CancellationToken cancellationToken);
}

public sealed record ProviderFetchResult(ProviderRouteIdentity Route, bool Succeeded, bool Authoritative, string ReasonCode, decimal? Value, DateTimeOffset? ObservedAt);

public sealed class ProviderDataCoordinator
{
    private readonly ProviderController _controller;
    private readonly QuotaLedger _quota;
    private readonly IProviderEgressPort _egress;

    public ProviderDataCoordinator(ProviderController controller, QuotaLedger quota, IProviderEgressPort egress)
    {
        _controller = controller ?? throw new ArgumentNullException(nameof(controller));
        _quota = quota ?? throw new ArgumentNullException(nameof(quota));
        _egress = egress ?? throw new ArgumentNullException(nameof(egress));
    }

    public async ValueTask<ProviderFetchResult?> FetchAsync(DataProductId product, IReadOnlyList<ProviderRouteCandidate> candidates, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(candidates);
        var route = _controller.SelectRoute(candidates);
        if (route is null) return null;
        if (!_quota.TryConsume(route)) return new(route, false, false, "PROVIDER_ROUTE_QUOTA_UNAVAILABLE", null, null);

        try
        {
            var result = await _egress.FetchAsync(route, product, cancellationToken).ConfigureAwait(false);
            if (result is null || result.Route != route) return new(route, false, false, "PROVIDER_ROUTE_IDENTITY_MISMATCH", null, null);
            if (string.IsNullOrWhiteSpace(result.ReasonCode)) return new(route, false, false, "PROVIDER_RESULT_REASON_REQUIRED", null, null);
            if (result.Succeeded && result.ObservedAt is null) return new(route, false, false, "PROVIDER_SUCCESS_OBSERVED_AT_REQUIRED", null, null);
            return result;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            return new(route, false, false, $"PROVIDER_ROUTE_FAILURE:{ex.GetType().Name}", null, null);
        }
    }
}
