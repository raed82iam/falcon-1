using T = Falcon.FSATS.Trading.Domain;
using TA = Falcon.FSATS.Trading.Application;
using P = Falcon.FSATS.FSAPMA.Domain;
using PA = Falcon.FSATS.FSAPMA.Application;
using GA = Falcon.FSATS.TradingGuardian.Application;
using GC = Falcon.FSATS.TradingGuardian.Contracts;
using SC = Falcon.FSATS.FSTSimA.Contracts;
using SA = Falcon.FSATS.FSTSimA.Application;
using S = Falcon.FSATS.FSTSimA.Domain;

internal static class BrokerAccountIsolationAdversarialChecks
{
    internal static void Run()
    {
        SameOrderIdAcrossAccountsCannotCrossReconcile();
        GuardianWrongAccountOutcomeFailsClosed();
        ProviderAccountsHaveIndependentQuotaAndRouteTruth();
        EventNamespacesAreIndependentAcrossAccounts();
        SimulationEvidenceIsScopeDistinct();
        ExecutionQueueContainmentAdversarialChecks.Run();
        DispatchStartLinearizationAdversarialChecks.Run();
        ProviderStreamingCatalogAdversarialChecks.Run();
    }

    private static T.BrokerAccountContext Account(string broker, string account, string environment = "PAPER")
        => new(broker, account, environment);

    private static void SameOrderIdAcrossAccountsCannotCrossReconcile()
    {
        var a = new TA.BrokerExecutionIdentity(Account("ALPACA", "PA-A"), "route-a", "submission-a", new T.OrderId("same-order"));
        var b = new TA.BrokerExecutionIdentity(Account("ALPACA", "PA-B"), "route-b", "submission-b", new T.OrderId("same-order"));
        if (a == b || a.NamespaceKey == b.NamespaceKey)
            throw new InvalidOperationException("C-02_SAME_ORDER_ID_COLLAPSED_ACROSS_BROKER_ACCOUNTS");

        var queue = new TA.AccountScopedExecutionQueue();
        var port = new WrongIdentityBrokerPort(b);
        var coordinator = new TA.ExecutionCoordinator(port, queue);
        var safety = new T.PositionSafetyEnvelope(new T.PositionId("pos-a"), new T.InstrumentId("AAPL"), new T.Quantity(1),
            new T.Money(10, new T.Currency("USD")), "guardian", "protected", "exit", "known", new T.TrustEpoch(1))
        {
            AccountContext = a.Account,
            ProtectionEvidenceReference = "evidence-a-protection"
        };
        var preparation = new TA.TradingDecisionPipeline(new T.CapitalReservationLedger()).Prepare(
            new T.RiskRequest(
                new T.InstrumentId("AAPL"),
                new T.Quantity(1),
                new T.Money(10, new T.Currency("USD")),
                new T.Money(20, new T.Currency("USD")),
                true,
                true)
            {
                AccountContext = a.Account
            },
            new T.ReservationId("reservation-a"),
            new T.Money(100, new T.Currency("USD")),
            safety,
            new T.TrustEpoch(1),
            new T.TrustEpoch(1),
            a);
        if (!preparation.Allowed || preparation.ReservationId is null || string.IsNullOrWhiteSpace(preparation.DecisionBindingReference))
            throw new InvalidOperationException("C-02_DECISION_BINDING_SETUP_FAILED");
        var intent = new TA.OrderIntent(a, new T.InstrumentId("AAPL"), new T.Quantity(1), new T.TrustEpoch(1), safety)
        {
            RiskReservationId = preparation.ReservationId,
            DecisionBindingReference = preparation.DecisionBindingReference
        };
        if (!queue.Enqueue(new TA.QueuedExecutionWork("work-a", intent, DateTimeOffset.UtcNow, "evidence-a"), out _) ||
            !queue.TryLeaseNext(out var lease) || lease is null ||
            !queue.TryBeginDispatch(lease, out var permit) || permit is null)
            throw new InvalidOperationException("C-02_GOVERNED_EXECUTION_QUEUE_SETUP_FAILED");
        var result = coordinator.SubmitOrReconcileAsync(intent, permit, CancellationToken.None).AsTask().GetAwaiter().GetResult();
        if (result.State != T.OrderState.ReconciliationRequired || result.ExecutionIdentity != a)
            throw new InvalidOperationException("C-02_WRONG_ACCOUNT_RECONCILIATION_NOT_REJECTED");
    }

    private static void GuardianWrongAccountOutcomeFailsClosed()
    {
        var now = DateTimeOffset.UtcNow;
        var requested = new GC.ProtectionTarget(GC.ProtectionTargetKind.BrokerAccount, "ALPACA", "PA-A", "PAPER");
        var wrong = new GC.ProtectionTarget(GC.ProtectionTargetKind.BrokerAccount, "ALPACA", "PA-B", "PAPER");
        var command = new GC.ProtectionCommand(new GC.CommandId("cmd-target"), GC.ProtectionCommandType.NewRiskFreeze,
            "FSATS-TRADING", requested, "authority", "reason", new GC.ProtectionEpoch(3), now.AddSeconds(-1), now.AddMinutes(1), "corr", "cause");
        var envelope = new GA.GovernedProtectionCommandEnvelope("msg", "schema", "1.0", GA.TradingGuardianManifest.Current.ApplicationId,
            "FSATS-TRADING", "authority", "prov", "corr", "cause", "idem-target", "attempt", "retry",
            GA.ProtectionTrafficTruth.Operational, now.AddSeconds(-1), now.AddMinutes(1), "evidence", command);
        var result = new GA.GovernedProtectionCommandDispatcher(new WrongTargetProtectionRoute(wrong))
            .DispatchAsync(envelope, 3, now, CancellationToken.None).AsTask().GetAwaiter().GetResult();
        if (result.State != GC.ProtectionOutcomeState.ReconciliationRequired || result.Target != requested || result.ReasonCode != "ROUTE_OUTCOME_BINDING_MISMATCH")
            throw new InvalidOperationException("C-03_WRONG_BROKER_ACCOUNT_PROTECTION_OUTCOME_ACCEPTED");
    }

    private static void ProviderAccountsHaveIndependentQuotaAndRouteTruth()
    {
        var quota = new P.QuotaLedger();
        var routeA = new P.ProviderRouteIdentity(
            new P.ProviderId("ALPACA-DATA"), new P.ProviderAccountId("API-A"), "PAPER", "MARKET_DATA",
            new P.ApiInstanceId("api-1"), new P.ProviderEndpointId("endpoint-1"), new P.CredentialReference("cred-a"));
        var routeB = new P.ProviderRouteIdentity(
            new P.ProviderId("ALPACA-DATA"), new P.ProviderAccountId("API-B"), "PAPER", "MARKET_DATA",
            new P.ApiInstanceId("api-1"), new P.ProviderEndpointId("endpoint-1"), new P.CredentialReference("cred-b"));
        if (!routeA.HasCurrentRouteBinding || !routeB.HasCurrentRouteBinding)
            throw new InvalidOperationException("H-01_CURRENT_PROVIDER_ROUTE_WAS_NOT_ADMISSIBLE");

        quota.SetWindow(routeA, 1);
        quota.SetWindow(routeB, 1);
        if (quota.ResolvePool(routeA) != quota.ResolvePool(routeB))
            throw new InvalidOperationException("H-01_UNKNOWN_QUOTA_SCOPE_WAS_TREATED_AS_INDEPENDENT_CAPACITY");
        if (!quota.TryConsume(routeA) || quota.Remaining(routeA) != 0 || quota.Remaining(routeB) != 0)
            throw new InvalidOperationException("H-01_UNKNOWN_SHARED_QUOTA_POOL_NOT_CONSERVATIVELY_SHARED");

        var poolA = new P.ProviderQuotaPoolId("ALPACA-DATA:ACCOUNT-A:MARKET-DATA");
        var poolB = new P.ProviderQuotaPoolId("ALPACA-DATA:ACCOUNT-B:MARKET-DATA");
        quota.BindRouteToPool(routeA, poolA);
        quota.BindRouteToPool(routeB, poolB);
        quota.SetPoolWindow(poolA, 1);
        quota.SetPoolWindow(poolB, 1);

        if (!quota.TryConsume(routeA) || quota.Remaining(routeA) != 0 || quota.Remaining(routeB) != 1)
            throw new InvalidOperationException("H-01_EXPLICIT_INDEPENDENT_PROVIDER_POOL_CROSS_CONSUMPTION");

        var coordinator = new PA.ProviderDataCoordinator(new P.ProviderController(), quota, new WrongProviderRoutePort(routeA));
        var result = coordinator.FetchAsync(new P.DataProductId("last-price"), new[]
        {
            new P.ProviderRouteCandidate(routeB, P.CapabilityState.Supported, P.QualityState.Healthy, 100)
        }, CancellationToken.None).AsTask().GetAwaiter().GetResult();
        if (result is null || result.Succeeded || result.ReasonCode != "PROVIDER_ROUTE_IDENTITY_MISMATCH")
            throw new InvalidOperationException("H-01_WRONG_PROVIDER_ACCOUNT_ROUTE_ACCEPTED");
    }

    private static void EventNamespacesAreIndependentAcrossAccounts()
    {
        var now = DateTimeOffset.UtcNow;
        var ingress = new TA.GovernedApplicationEventIngress();
        var digest = new string('A', 64);
        var scopeA = TA.GovernedApplicationEventIngress.BrokerAccountScope("ALPACA", "PA-A", "PAPER");
        var scopeB = TA.GovernedApplicationEventIngress.BrokerAccountScope("ALPACA", "PA-B", "PAPER");
        var a = new TA.GovernedApplicationEventEnvelope("same-event", "test", PA.FSAPMAManifest.Current.ApplicationId, TA.TradingManifest.Current.ApplicationId,
            "schema", "1.0", "msg-a", "corr-a", "cause-a", "idem-a", TA.ApplicationEventTruth.NonAuthoritativeEvidence,
            TA.ApplicationEventRelation.None, null, "same-ordering", 1, now.AddSeconds(-1), "evidence", digest, scopeA);
        var b = a with { MessageId = "msg-b", CorrelationId = "corr-b", CausationId = "cause-b", IdempotencyId = "idem-b", ScopeKey = scopeB };
        if (ingress.Consume(a, now).State != TA.ApplicationEventIngressState.AcceptedEvidence ||
            ingress.Consume(b, now).State != TA.ApplicationEventIngressState.AcceptedEvidence)
            throw new InvalidOperationException("H-02_ACCOUNT_EVENT_NAMESPACE_COLLISION");
    }

    private static void SimulationEvidenceIsScopeDistinct()
    {
        var sink = new CapturingEvidenceSink();
        var coordinator = new SA.SimulationCoordinator(new S.SyntheticMarketGenerator(), new S.ValidationAssessor(), sink);
        var a = new SC.SimulationScope("BROKER_ACCOUNT", "ALPACA", "PA-A", "PAPER");
        var b = new SC.SimulationScope("BROKER_ACCOUNT", "ALPACA", "PA-B", "PAPER");
        coordinator.RunDeterminismQualification(a, "scenario", 7, 100m);
        coordinator.RunDeterminismQualification(b, "scenario", 7, 100m);
        if (sink.EvidenceIds.Count != 2 || sink.EvidenceIds.Distinct(StringComparer.Ordinal).Count() != 2)
            throw new InvalidOperationException("M-01_SIMULATION_EVIDENCE_SCOPE_COLLISION");
    }

    private sealed class WrongIdentityBrokerPort : TA.IBrokerExecutionPort
    {
        private readonly TA.BrokerExecutionIdentity _wrong;
        public WrongIdentityBrokerPort(TA.BrokerExecutionIdentity wrong) => _wrong = wrong;
        public ValueTask<TA.BrokerSubmissionResult> SubmitAsync(TA.OrderIntent intent, CancellationToken cancellationToken)
            => ValueTask.FromResult(new TA.BrokerSubmissionResult(_wrong, false, false, "AMBIGUOUS"));
        public ValueTask<TA.BrokerOrderSnapshot> ReconcileAsync(TA.BrokerExecutionIdentity identity, CancellationToken cancellationToken)
            => ValueTask.FromResult(new TA.BrokerOrderSnapshot(_wrong, T.OrderState.Filled, new T.Quantity(1), "WRONG_ACCOUNT"));
    }

    private sealed class WrongTargetProtectionRoute : GA.IGovernedProtectionCommandRoutePort
    {
        private readonly GC.ProtectionTarget _wrong;
        public WrongTargetProtectionRoute(GC.ProtectionTarget wrong) => _wrong = wrong;
        public ValueTask<GC.ProtectionCommandOutcome> DispatchAsync(GA.GovernedProtectionCommandEnvelope envelope, CancellationToken cancellationToken)
            => ValueTask.FromResult(new GC.ProtectionCommandOutcome(envelope.Command.CommandId, GC.ProtectionOutcomeState.Applied,
                envelope.Command.TargetApplication, _wrong, "APPLIED", DateTimeOffset.UtcNow, envelope.CorrelationId));
    }

    private sealed class WrongProviderRoutePort : PA.IProviderEgressPort
    {
        private readonly P.ProviderRouteIdentity _wrong;
        public WrongProviderRoutePort(P.ProviderRouteIdentity wrong) => _wrong = wrong;
        public ValueTask<PA.ProviderFetchResult> FetchAsync(P.ProviderRouteIdentity route, P.DataProductId product, CancellationToken cancellationToken)
            => ValueTask.FromResult(new PA.ProviderFetchResult(_wrong, true, true, "OK", 1m, DateTimeOffset.UtcNow));
    }

    private sealed class CapturingEvidenceSink : SA.ISimulationEvidenceSink
    {
        public List<string> EvidenceIds { get; } = new();
        public void Commit(string evidenceId, string scenarioId, int seed, string digest) => EvidenceIds.Add(evidenceId);
    }
}
