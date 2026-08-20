using System.Runtime.CompilerServices;
using P = Falcon.FSATS.FSAPMA.Domain;

internal static class QuotaPoolAdversarialChecks
{
    [ModuleInitializer]
    internal static void Run()
    {
        UnknownScopeDoesNotMultiplyCapacity();
        ExplicitSharedPoolIsConsumedOnce();
        SharedWebFsapmaHalfCeilingIsFailClosed();
        ProvenIndependentPoolsRemainIndependent();
        MultipleQuotaDimensionsConsumeAtomically();
        SinglePoolAccessorFailsClosedForMultiDimensionRoute();
    }

    private static void UnknownScopeDoesNotMultiplyCapacity()
    {
        var routeA = Route("acct-a", "api-a", "endpoint-a", "cred-a");
        var routeB = Route("acct-b", "api-b", "endpoint-b", "cred-b");
        var ledger = new P.QuotaLedger();

        ledger.SetWindow(routeA, 1);
        ledger.SetWindow(routeB, 1);

        if (ledger.ResolvePool(routeA) != ledger.ResolvePool(routeB))
            throw new InvalidOperationException("UNKNOWN_QUOTA_SCOPE_WAS_TREATED_AS_INDEPENDENT_CAPACITY");
        if (!ledger.TryConsume(routeA))
            throw new InvalidOperationException("UNKNOWN_SHARED_PROVIDER_POOL_FIRST_CONSUME_FAILED");
        if (ledger.TryConsume(routeB))
            throw new InvalidOperationException("MULTIPLE_ACCOUNTS_OR_KEYS_MULTIPLIED_UNKNOWN_PROVIDER_CAPACITY");
    }

    private static void ExplicitSharedPoolIsConsumedOnce()
    {
        var routeA = Route("acct-a", "api-a", "endpoint-a", "cred-a");
        var routeB = Route("acct-b", "api-b", "endpoint-b", "cred-b");
        var ledger = new P.QuotaLedger();
        var pool = new P.ProviderQuotaPoolId("provider-x-shared-minute-budget");

        ledger.BindRouteToPool(routeA, pool);
        ledger.BindRouteToPool(routeB, new P.ProviderQuotaPoolId("PROVIDER-X-SHARED-MINUTE-BUDGET"));
        ledger.SetPoolWindow(pool, 2);

        if (!ledger.HasExplicitPoolBinding(routeA) || !ledger.HasExplicitPoolBinding(routeB))
            throw new InvalidOperationException("EXPLICIT_QUOTA_POOL_BINDING_NOT_RECORDED");
        if (!ledger.TryConsume(routeA) || !ledger.TryConsume(routeB) || ledger.TryConsume(routeA))
            throw new InvalidOperationException("SHARED_QUOTA_POOL_WAS_NOT_CONSUMED_AS_ONE_POOL");
        if (ledger.Remaining(pool) != 0)
            throw new InvalidOperationException("SHARED_QUOTA_POOL_REMAINING_COUNT_INCONSISTENT");
    }

    private static void SharedWebFsapmaHalfCeilingIsFailClosed()
    {
        var route = Route("acct-a", "api-a", "endpoint-a", "cred-a");
        var ledger = new P.QuotaLedger();
        var pool = new P.ProviderQuotaPoolId("provider-x-shared-daily-budget");
        ledger.BindRouteToPool(route, pool);

        ledger.SetSharedWebFsapmaWindow(pool, 5);

        if (ledger.Remaining(pool) != 2)
            throw new InvalidOperationException("SHARED_POOL_ODD_REMAINDER_WAS_NOT_LEFT_UNALLOCATED");
        if (!ledger.TryConsume(route, 2) || ledger.TryConsume(route))
            throw new InvalidOperationException("FSAPMA_EXCEEDED_SHARED_POOL_HALF_CEILING");
    }

    private static void ProvenIndependentPoolsRemainIndependent()
    {
        var routeA = Route("acct-a", "api-a", "endpoint-a", "cred-a");
        var routeB = Route("acct-b", "api-b", "endpoint-b", "cred-b");
        var ledger = new P.QuotaLedger();
        var poolA = new P.ProviderQuotaPoolId("independent-a");
        var poolB = new P.ProviderQuotaPoolId("independent-b");

        ledger.BindRouteToPool(routeA, poolA);
        ledger.BindRouteToPool(routeB, poolB);
        ledger.SetPoolWindow(poolA, 1);
        ledger.SetPoolWindow(poolB, 1);

        if (!ledger.TryConsume(routeA) || !ledger.TryConsume(routeB))
            throw new InvalidOperationException("PROVEN_INDEPENDENT_QUOTA_POOLS_WERE_INCORRECTLY_COLLAPSED");
        if (ledger.TryConsume(routeA) || ledger.TryConsume(routeB))
            throw new InvalidOperationException("INDEPENDENT_QUOTA_POOL_LIMIT_NOT_ENFORCED");
    }

    private static void MultipleQuotaDimensionsConsumeAtomically()
    {
        var route = Route("acct-a", "api-a", "endpoint-a", "cred-a");
        var minute = new P.ProviderQuotaPoolId("provider-x-minute");
        var daily = new P.ProviderQuotaPoolId("provider-x-daily");
        var burst = new P.ProviderQuotaPoolId("provider-x-burst");
        var ledger = new P.QuotaLedger();

        ledger.BindRouteToPool(route, minute);
        ledger.BindRouteToPool(route, daily);
        ledger.BindRouteToPool(route, burst);
        ledger.SetPoolWindow(minute, 3);
        ledger.SetPoolWindow(daily, 100);
        ledger.SetPoolWindow(burst, 1);

        if (ledger.ResolvePools(route).Count != 3)
            throw new InvalidOperationException("MULTI_DIMENSION_QUOTA_BINDINGS_NOT_PRESERVED");
        if (!ledger.TryConsume(route))
            throw new InvalidOperationException("MULTI_DIMENSION_FIRST_ATOMIC_CONSUME_FAILED");
        if (ledger.Remaining(minute) != 2 || ledger.Remaining(daily) != 99 || ledger.Remaining(burst) != 0)
            throw new InvalidOperationException("MULTI_DIMENSION_ATOMIC_CONSUME_DID_NOT_DECREMENT_ALL_POOLS");

        if (ledger.TryConsume(route))
            throw new InvalidOperationException("EXHAUSTED_BURST_DIMENSION_WAS_BYPASSED");
        if (ledger.Remaining(minute) != 2 || ledger.Remaining(daily) != 99 || ledger.Remaining(burst) != 0)
            throw new InvalidOperationException("FAILED_MULTI_DIMENSION_RESERVATION_PARTIALLY_MUTATED_OTHER_POOLS");
    }

    private static void SinglePoolAccessorFailsClosedForMultiDimensionRoute()
    {
        var route = Route("acct-a", "api-a", "endpoint-a", "cred-a");
        var ledger = new P.QuotaLedger();
        ledger.BindRouteToPool(route, new P.ProviderQuotaPoolId("minute"));
        ledger.BindRouteToPool(route, new P.ProviderQuotaPoolId("daily"));

        try
        {
            _ = ledger.ResolvePool(route);
            throw new InvalidOperationException("MULTI_DIMENSION_ROUTE_WAS_MISREPRESENTED_AS_SINGLE_POOL");
        }
        catch (InvalidOperationException ex) when (ex.Message == "PROVIDER_ROUTE_HAS_MULTIPLE_QUOTA_DIMENSIONS")
        {
        }
    }

    private static P.ProviderRouteIdentity Route(string account, string api, string endpoint, string credential)
        => new(
            new P.ProviderId("provider-x"),
            new P.ProviderAccountId(account),
            "paper",
            "market_data",
            new P.ApiInstanceId(api),
            new P.ProviderEndpointId(endpoint),
            new P.CredentialReference(credential));
}
