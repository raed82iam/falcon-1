using Falcon.FSATS.FSAPMA.Application;
using Falcon.FSATS.FSAPMA.Domain;

namespace Falcon.FSATS.FSAPMA.Infrastructure;

public sealed class DisabledProviderEgressPort : IProviderEgressPort
{
    public ValueTask<ProviderFetchResult> FetchAsync(ProviderRouteIdentity route, DataProductId product, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(route);
        return ValueTask.FromResult(new ProviderFetchResult(
            route,
            false,
            false,
            "PROVIDER_EGRESS_NOT_AUTHORIZED",
            null,
            null));
    }
}
