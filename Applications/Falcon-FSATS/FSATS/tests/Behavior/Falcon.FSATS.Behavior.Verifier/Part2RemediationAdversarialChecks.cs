using T = Falcon.FSATS.Trading.Domain;
using TA = Falcon.FSATS.Trading.Application;
using TW = Falcon.FSATS.Trading.Awareness;
using P = Falcon.FSATS.FSAPMA.Domain;
using PA = Falcon.FSATS.FSAPMA.Application;
using GA = Falcon.FSATS.TradingGuardian.Application;
using GC = Falcon.FSATS.TradingGuardian.Contracts;
using SC = Falcon.FSATS.FSTSimA.Contracts;
using SA = Falcon.FSATS.FSTSimA.Application;
using R = Falcon.FSATS.ResourceManagement.Domain;
using RA = Falcon.FSATS.ResourceManagement.Application;

internal static class Part2RemediationAdversarialChecks
{
    internal static void Run()
    {
        EventIngressConcurrency();
        EventScopeSmugglingFailsClosed();
        ManifestCollectionsAreNotArrayBacked();
        GuardianRouteFailuresFailClosed();
        GuardianCancellationIsReconciliationOwned();
        GuardianTargetShapeCannotSmuggleExtraScope();
        AwarenessCandidateBindingFailsClosed();
        BrokerAccountFailureContainmentIsScoped();
        BrokerOutageTruthAndRetryRulesHold();
        ProviderRouteIdentityIsCanonicalAndFaultsFailClosed();
        SimulationScopeIdentityIsCanonical();
        ResourcePlanningAmbiguityFailsClosed();
        ResourceOscillationGuardIsConcurrentSafe();
    }

    private static void EventIngressConcurrency()
    {
        var now = DateTimeOffset.UtcNow;
        var digest = new string('A', 64);
        var trading = new TA.GovernedApplicationEventIngress();
        var envelope = new TA.GovernedApplicationEventEnvelope(
            "evt-concurrent", "test", PA.FSAPMAManifest.Current.ApplicationId, TA.TradingManifest.Current.ApplicationId,
            "schema", "1.0", "msg", "corr", "cause", "idem", TA.ApplicationEventTruth.NonAuthoritativeEvidence,
            TA.ApplicationEventRelation.None, null, "ordering", 1, now.AddSeconds(-1), "evidence", digest,
            TA.GovernedApplicationEventIngress.BrokerAccountScope("ALPACA", "PA-ACCOUNT-A", "PAPER"));
        var states = Task.WhenAll(Enumerable.Range(0, 32).Select(_ => Task.Run(() => trading.Consume(envelope, now)))).GetAwaiter().GetResult();
        if (states.Count(x => x.State == TA.ApplicationEventIngressState.AcceptedEvidence) != 1 || states.Count(x => x.State == TA.ApplicationEventIngressState.Duplicate) != 31)
            throw new InvalidOperationException("TRADING_EVENT_INGRESS_CONCURRENCY_FAILED");
    }

    private static void EventScopeSmugglingFailsClosed()
    {
        var now = DateTimeOffset.UtcNow;
        var digest = new string('A', 64);
        var trading = new TA.GovernedApplicationEventIngress();
        var tradingEvent = new TA.GovernedApplicationEventEnvelope(
            "evt-scope-t", "test", PA.FSAPMAManifest.Current.ApplicationId, TA.TradingManifest.Current.ApplicationId,
            "schema", "1.0", "msg-t", "corr-t", "cause-t", "idem-t", TA.ApplicationEventTruth.NonAuthoritativeEvidence,
            TA.ApplicationEventRelation.None, null, null, 0, now.AddSeconds(-1), "evidence", digest,
            "BROKER_ACCOUNTX|ALPACA|PA-A|PAPER");
        if (trading.Consume(tradingEvent, now).ReasonCode != "EVENT_SCOPE_INVALID")
            throw new InvalidOperationException("TRADING_RESERVED_SCOPE_SMUGGLING_ACCEPTED");

        var provider = new PA.GovernedApplicationEventIngress();
        var providerEvent = new PA.GovernedApplicationEventEnvelope(
            "evt-scope-p", "test", TA.TradingManifest.Current.ApplicationId, PA.FSAPMAManifest.Current.ApplicationId,
            "schema", "1.0", "msg-p", "corr-p", "cause-p", "idem-p", PA.ApplicationEventTruth.NonAuthoritativeEvidence,
            PA.ApplicationEventRelation.None, null, null, 0, now.AddSeconds(-1), "evidence", digest,
            "PROVIDER_ROUTE|ALPACA|acct|PAPER");
        if (provider.Consume(providerEvent, now).ReasonCode != "EVENT_SCOPE_INVALID")
            throw new InvalidOperationException("FSAPMA_MALFORMED_PROVIDER_SCOPE_ACCEPTED");

        var guardian = new GA.GovernedApplicationEventIngress();
        var guardianEvent = new GA.GovernedApplicationEventEnvelope(
            "evt-scope-g", "test", TA.TradingManifest.Current.ApplicationId, GA.TradingGuardianManifest.Current.ApplicationId,
            "schema", "1.0", "msg-g", "corr-g", "cause-g", "idem-g", GA.ApplicationEventTruth.NonAuthoritativeEvidence,
            GA.ApplicationEventRelation.None, null, null, 0, now.AddSeconds(-1), "evidence", digest,
            "BROKER_ACCOUNT|ALPACA|PA-A|PAPER|EXTRA");
        if (guardian.Consume(guardianEvent, now).ReasonCode != "EVENT_SCOPE_INVALID")
            throw new InvalidOperationException("GUARDIAN_MALFORMED_ACCOUNT_SCOPE_ACCEPTED");

        var opaque = "PA|OPAQUE%ACCOUNT";
        var canonical = TA.GovernedApplicationEventIngress.BrokerAccountScope("alpaca", opaque, "paper");
        if (!canonical.Contains("PA%7COPAQUE%25ACCOUNT", StringComparison.Ordinal))
            throw new InvalidOperationException("OPAQUE_ACCOUNT_SCOPE_NOT_ESCAPED");
    }

    private static void ManifestCollectionsAreNotArrayBacked()
    {
        AssertReadOnly(TA.TradingManifest.Current.LsaIds, "TRADING_MANIFEST_LSA");
        AssertReadOnly(PA.FSAPMAManifest.Current.LsaIds, "FSAPMA_MANIFEST_LSA");
        AssertReadOnly(GA.TradingGuardianManifest.Current.LsaIds, "GUARDIAN_MANIFEST_LSA");
        AssertReadOnly(SA.FSTSimAManifest.Current.LsaIds, "FSTSIMA_MANIFEST_LSA");
        AssertReadOnly(RA.ResourceManagementManifest.Current.LsaIds, "RSC_MANIFEST_LSA");
        if (TA.TradingManifest.Current.RuntimeAuthorized || TA.TradingManifest.Current.ExternalEgressAuthorized ||
            PA.FSAPMAManifest.Current.RuntimeAuthorized || PA.FSAPMAManifest.Current.ProviderEgressAuthorized ||
            GA.TradingGuardianManifest.Current.RuntimeAuthorized || GA.TradingGuardianManifest.Current.ProtectionRouteBound ||
            SA.FSTSimAManifest.Current.RuntimeAuthorized || SA.FSTSimAManifest.Current.OperationalEgressAuthorized || SA.FSTSimAManifest.Current.PaperAuthority ||
            RA.ResourceManagementManifest.Current.RuntimeAuthorized || RA.ResourceManagementManifest.Current.FoundationResourceBindingBound)
            throw new InvalidOperationException("PART2_REMEDIATION_ACCIDENTALLY_ACTIVATED_RUNTIME_AUTHORITY");
    }

    private static void AssertReadOnly(IReadOnlyList<string> values, string name)
    {
        if (values is string[]) throw new InvalidOperationException($"{name}_ARRAY_BACKING_EXPOSED");
        if (values.Count == 0) return;
        if (values is IList<string> list)
        {
            try { list[0] = "MUTATED"; throw new InvalidOperationException($"{name}_MUTATION_SUCCEEDED"); }
            catch (NotSupportedException) { }
        }
    }

    private static void GuardianRouteFailuresFailClosed()
    {
        var now = DateTimeOffset.UtcNow;
        var target = new GC.ProtectionTarget(GC.ProtectionTargetKind.BrokerAccount, "ALPACA", "PA-ACCOUNT-A", "PAPER");
        var command = new GC.ProtectionCommand(new GC.CommandId("cmd-fail"), GC.ProtectionCommandType.AiKill, "TARGET", target, "authority", "reason",
            new GC.ProtectionEpoch(4), now.AddSeconds(-1), now.AddMinutes(1), "corr", "cause");
        var envelope = new GA.GovernedProtectionCommandEnvelope("msg", "schema", "1.0", GA.TradingGuardianManifest.Current.ApplicationId, "TARGET",
            "authority", "prov", "corr", "cause", "idem-fail", "attempt", "retry", GA.ProtectionTrafficTruth.Operational,
            now.AddSeconds(-1), now.AddMinutes(1), "evidence-route", command);

        var failed = new GA.GovernedProtectionCommandDispatcher(new ThrowingRoute()).DispatchAsync(envelope, 4, now, CancellationToken.None).AsTask().GetAwaiter().GetResult();
        if (failed.State != GC.ProtectionOutcomeState.ReconciliationRequired || failed.Target != target || failed.RequestFingerprint.Length != 64)
            throw new InvalidOperationException("GUARDIAN_ROUTE_EXCEPTION_NOT_FAIL_CLOSED");

        var nullOutcome = new GA.GovernedProtectionCommandDispatcher(new NullRoute()).DispatchAsync(envelope with { IdempotencyId = "idem-null" }, 4, now, CancellationToken.None).AsTask().GetAwaiter().GetResult();
        if (nullOutcome.State != GC.ProtectionOutcomeState.ReconciliationRequired || nullOutcome.Target != target || nullOutcome.ReasonCode != "NULL_ROUTE_OUTCOME")
            throw new InvalidOperationException("GUARDIAN_NULL_ROUTE_OUTCOME_NOT_FAIL_CLOSED");
    }

    private static void GuardianCancellationIsReconciliationOwned()
    {
        var now = DateTimeOffset.UtcNow;
        var target = new GC.ProtectionTarget(GC.ProtectionTargetKind.BrokerAccount, "ALPACA", "PA-CANCEL", "PAPER");
        var command = new GC.ProtectionCommand(new GC.CommandId("cmd-cancel"), GC.ProtectionCommandType.CancelWorkingEntries, "TARGET", target, "authority", "reason",
            new GC.ProtectionEpoch(9), now.AddSeconds(-1), now.AddMinutes(1), "corr-cancel", "cause-cancel");
        var envelope = new GA.GovernedProtectionCommandEnvelope("msg-cancel", "schema", "1.0", GA.TradingGuardianManifest.Current.ApplicationId, "TARGET",
            "authority", "prov", "corr-cancel", "cause-cancel", "idem-cancel", "attempt", "retry", GA.ProtectionTrafficTruth.Operational,
            now.AddSeconds(-1), now.AddMinutes(1), "evidence-cancel", command);
        var route = new CancellingRoute();
        var dispatcher = new GA.GovernedProtectionCommandDispatcher(route);
        var first = dispatcher.DispatchAsync(envelope, 9, now, CancellationToken.None).AsTask().GetAwaiter().GetResult();
        var retry = dispatcher.DispatchAsync(envelope with { MessageId = "transport-retry", DeliveryAttemptId = "attempt-2" }, 9, now, CancellationToken.None).AsTask().GetAwaiter().GetResult();
        if (first.State != GC.ProtectionOutcomeState.ReconciliationRequired || first.ReasonCode != "ROUTE_DISPATCH_CANCELLATION_AMBIGUOUS" || retry.State != GC.ProtectionOutcomeState.ReconciliationRequired || route.Calls != 1)
            throw new InvalidOperationException("GUARDIAN_AMBIGUOUS_CANCELLATION_WAS_RETRIED");
    }

    private static void GuardianTargetShapeCannotSmuggleExtraScope()
    {
        var failed = false;
        try { _ = new GC.ProtectionTarget(GC.ProtectionTargetKind.BrokerAccount, "ALPACA", "PA-A", "PAPER", executionRouteId: "route-smuggled"); }
        catch (ArgumentException) { failed = true; }
        if (!failed) throw new InvalidOperationException("GUARDIAN_TARGET_ACCEPTED_IRRELEVANT_EXTRA_ROUTE_DIMENSION");
        var canonical = new GC.ProtectionTarget(GC.ProtectionTargetKind.BrokerAccount, "alpaca", "PA-A", "paper");
        if (canonical.BrokerId != "ALPACA" || canonical.Environment != "PAPER") throw new InvalidOperationException("GUARDIAN_TARGET_IDENTITY_NOT_CANONICAL");
    }

    private static void AwarenessCandidateBindingFailsClosed()
    {
        var unsigned = new TW.AwarenessCandidate("candidate-1", TW.ProposalOrigin.Csa, "CSA-T06-01", "T-LSA-06", TW.TradingAwarenessTopology.MsaId, "evidence-1", true)
        { CandidateSha256 = new string('A', 64), EvidenceSha256 = new string('B', 64), LineageId = "lineage-1", ParentIdentity = "T-LSA-06" };
        var good = unsigned with { BindingSha256 = TW.AwarenessGovernance.ComputeBindingSha256(unsigned) };
        if (!TW.AwarenessGovernance.IsIdentityAndEvidenceBound(good)) throw new InvalidOperationException("AWARENESS_VALID_BINDING_REJECTED");
        if (TW.AwarenessGovernance.IsIdentityAndEvidenceBound(good with { EvidenceSha256 = new string('C', 64) })) throw new InvalidOperationException("AWARENESS_TAMPERING_ACCEPTED");
    }

    private static void BrokerAccountFailureContainmentIsScoped()
    {
        var accountA = new T.BrokerAccountContext("ALPACA", "PA-ACCOUNT-A", "PAPER");
        var accountB = new T.BrokerAccountContext("ALPACA", "PA-ACCOUNT-B", "PAPER");
        var otherBroker = new T.BrokerAccountContext("IBKR", "IB-ACCOUNT-A", "PAPER");
        var scope = new T.OperationalFailureScope(accountA, "US", null, null, "route-a",
            new[] { new T.PositionId("pos-a") }, Array.Empty<T.OrderId>(), Array.Empty<string>(),
            T.OperationalFailureClass.BrokerExecutionApiUnavailable, T.OperationalTruthState.Unknown,
            T.OperationalContainmentState.None, T.OperationalRecoveryState.Investigating);

        var local = new T.FailureLocalityEvidence("evidence-local", T.ProvenFailureBlastRadius.AccountLocal, DateTimeOffset.UtcNow, new[] { accountA }, "account-session-a");
        if (T.OperationalFailureContainmentPolicy.Decide(scope, local).State != T.OperationalContainmentState.Scoped) throw new InvalidOperationException("ACCOUNT_FAILURE_NOT_SCOPED");
        if (T.OperationalFailureContainmentPolicy.ShouldAffectPeer(scope, accountB, local)) throw new InvalidOperationException("ACCOUNT_A_FAILURE_POISONED_ACCOUNT_B");

        var brokerWide = new T.FailureLocalityEvidence("evidence-broker", T.ProvenFailureBlastRadius.BrokerWide, DateTimeOffset.UtcNow, sharedDependencyId: "BROKER:ALPACA|PAPER");
        if (!T.OperationalFailureContainmentPolicy.ShouldAffectPeer(scope, accountB, brokerWide)) throw new InvalidOperationException("BROKER_WIDE_FAILURE_DID_NOT_REACH_BROKER_PEER");
        if (T.OperationalFailureContainmentPolicy.ShouldAffectPeer(scope, otherBroker, brokerWide)) throw new InvalidOperationException("BROKER_WIDE_FAILURE_POISONED_OTHER_BROKER");

        var mismatchedBrokerEvidence = new T.FailureLocalityEvidence("evidence-wrong-broker", T.ProvenFailureBlastRadius.BrokerWide, DateTimeOffset.UtcNow, sharedDependencyId: "BROKER:IBKR|PAPER");
        if (T.OperationalFailureContainmentPolicy.Decide(scope, mismatchedBrokerEvidence).State != T.OperationalContainmentState.Expanded)
            throw new InvalidOperationException("MISMATCHED_LOCALITY_EVIDENCE_WAS_TRUSTED");

        var unknown = new T.FailureLocalityEvidence("evidence-unknown", T.ProvenFailureBlastRadius.Unknown, DateTimeOffset.UtcNow);
        if (!T.OperationalFailureContainmentPolicy.ShouldAffectPeer(scope, accountB, unknown)) throw new InvalidOperationException("UNKNOWN_BLAST_RADIUS_NOT_EXPANDED");
    }

    private static void BrokerOutageTruthAndRetryRulesHold()
    {
        if (TA.BrokerOutageRecoveryPolicy.IsSafeToBlindRetry(TA.BrokerSubmissionTruth.SubmittedOutcomeUnknown)) throw new InvalidOperationException("UNKNOWN_SUBMISSION_BLIND_RETRY_ALLOWED");
        var now = DateTimeOffset.UtcNow;
        var account = new T.BrokerAccountContext("ALPACA", "PA-ACCOUNT-A", "PAPER");
        var humanObservation = new TA.BrokerAccountObservation(account, new T.PositionId("p"), new T.Quantity(10), false, true,
            TA.BrokerAccountEvidenceSource.UserReported, "evidence-human", now);
        var humanAssessment = TA.BrokerOutageRecoveryPolicy.Assess(TA.BrokerConnectivityState.Unavailable, TA.BrokerSubmissionTruth.NotSubmitted, humanObservation);
        if (humanAssessment.TruthState != T.OperationalTruthState.UserReported || humanAssessment.MayResumeRiskIncreasingAction) throw new InvalidOperationException("HUMAN_REPORT_PROMOTED_TO_BROKER_TRUTH");
        var brokerObservation = humanObservation with { EvidenceSource = TA.BrokerAccountEvidenceSource.BrokerApiConfirmed };
        var incomplete = TA.BrokerOutageRecoveryPolicy.Assess(TA.BrokerConnectivityState.Available, TA.BrokerSubmissionTruth.Reconciled, brokerObservation);
        if (incomplete.RecoveryState == T.OperationalRecoveryState.Recovered) throw new InvalidOperationException("INCOMPLETE_RECONCILIATION_FALSELY_RECOVERED");
        var dimensions = Enum.GetValues<TA.BrokerReconciliationDimension>()
            .Select(x => new TA.BrokerReconciliationDimensionEvidence(x, true, $"reconcile:{x}", now))
            .ToArray();
        var complete = new TA.BrokerAccountReconciliationEvidence(account, true, true, true, true, true, true, true, "reconcile", now, dimensions);
        var recovered = TA.BrokerOutageRecoveryPolicy.Assess(TA.BrokerConnectivityState.Available, TA.BrokerSubmissionTruth.Reconciled, brokerObservation, complete);
        if (!recovered.MayResumeRiskIncreasingAction || recovered.TruthState != T.OperationalTruthState.BrokerConfirmed) throw new InvalidOperationException("COMPLETE_BROKER_RECONCILIATION_NOT_RECOGNIZED");
    }

    private static void ProviderRouteIdentityIsCanonicalAndFaultsFailClosed()
    {
        var lower = new P.ProviderRouteIdentity(
            new P.ProviderId("alpaca-data"), new P.ProviderAccountId("acct"), "paper", "market_data",
            new P.ApiInstanceId("api-1"), new P.ProviderEndpointId("endpoint-1"), new P.CredentialReference("cred"));
        var upper = new P.ProviderRouteIdentity(
            new P.ProviderId("ALPACA-DATA"), new P.ProviderAccountId("acct"), "PAPER", "MARKET_DATA",
            new P.ApiInstanceId("api-1"), new P.ProviderEndpointId("endpoint-1"), new P.CredentialReference("cred"));
        if (lower != upper) throw new InvalidOperationException("PROVIDER_ROUTE_CASE_CREATED_PARALLEL_IDENTITY");
        if (!lower.HasCurrentRouteBinding || !upper.HasCurrentRouteBinding)
            throw new InvalidOperationException("CURRENT_PROVIDER_ROUTE_WAS_NOT_ADMISSIBLE");
        var quota = new P.QuotaLedger(); quota.SetWindow(lower, 1);
        var coordinator = new PA.ProviderDataCoordinator(new P.ProviderController(), quota, new ThrowingProviderRoute());
        var result = coordinator.FetchAsync(new P.DataProductId("last"), new[] { new P.ProviderRouteCandidate(upper, P.CapabilityState.Supported, P.QualityState.Healthy, 10) }, CancellationToken.None).AsTask().GetAwaiter().GetResult();
        if (result is null || result.Succeeded || !result.ReasonCode.StartsWith("PROVIDER_ROUTE_FAILURE:", StringComparison.Ordinal))
            throw new InvalidOperationException("PROVIDER_ROUTE_EXCEPTION_ESCAPED_FAIL_CLOSED_RESULT");
    }

    private static void SimulationScopeIdentityIsCanonical()
    {
        var lower = new SC.SimulationScope("broker_account", "alpaca", "PA-A", "paper");
        var upper = new SC.SimulationScope("BROKER_ACCOUNT", "ALPACA", "PA-A", "PAPER");
        if (lower.CanonicalKey != upper.CanonicalKey) throw new InvalidOperationException("SIMULATION_SCOPE_CASE_COLLISION");
    }

    private static void ResourcePlanningAmbiguityFailsClosed()
    {
        var controller = new R.ResourceStrategyController();
        var now = DateTimeOffset.UtcNow;
        var envelope = new R.FoundationEnvelope("env", "CPU", 30m, now.AddMinutes(-1), now.AddMinutes(10), false);
        var target = new R.ResourceClaim("Trading", "CPU", 10m, 8m, 5m, 20m, 3m, 100, true, true);
        var donor = new R.ResourceClaim("FSTSimA", "CPU", 10m, 5m, 5m, 10m, 5m, 10, true, true);
        if (controller.Plan(new[] { target, target with { Consumption = 7m }, donor }, envelope, new R.CoordinationEpoch(1), "Trading", "CPU", now) is not null)
            throw new InvalidOperationException("RSC_DUPLICATE_APPLICATION_CLAIM_DID_NOT_FAIL_CLOSED");
        if (controller.Plan(new[] { target with { Allocation = 25m }, donor with { Allocation = 10m, MinimumSafe = 5m, Reclaimable = 5m } }, envelope, new R.CoordinationEpoch(1), "Trading", "CPU", now) is not null)
            throw new InvalidOperationException("RSC_AGGREGATE_ALLOCATION_EXCEEDED_FOUNDATION_ENVELOPE");
        if (controller.Plan(new[] { target, donor with { Fresh = false } }, envelope, new R.CoordinationEpoch(1), "Trading", "CPU", now) is not null)
            throw new InvalidOperationException("RSC_PARTIAL_TRUTH_PLANNING_ACCEPTED_STALE_CLAIM");
    }

    private static void ResourceOscillationGuardIsConcurrentSafe()
    {
        var guard = new R.OscillationGuard();
        var now = DateTimeOffset.UtcNow;
        var results = Task.WhenAll(Enumerable.Range(0, 32).Select(_ => Task.Run(() => guard.Allow("CPU", now, TimeSpan.FromMinutes(1))))).GetAwaiter().GetResult();
        if (results.Count(x => x) != 1) throw new InvalidOperationException("RSC_OSCILLATION_GUARD_CONCURRENCY_BYPASS");
    }

    private sealed class ThrowingRoute : GA.IGovernedProtectionCommandRoutePort
    {
        public ValueTask<GC.ProtectionCommandOutcome> DispatchAsync(GA.GovernedProtectionCommandEnvelope envelope, CancellationToken cancellationToken)
            => ValueTask.FromException<GC.ProtectionCommandOutcome>(new InvalidOperationException("route failed"));
    }

    private sealed class NullRoute : GA.IGovernedProtectionCommandRoutePort
    {
        public ValueTask<GC.ProtectionCommandOutcome> DispatchAsync(GA.GovernedProtectionCommandEnvelope envelope, CancellationToken cancellationToken)
            => ValueTask.FromResult<GC.ProtectionCommandOutcome>(null!);
    }

    private sealed class CancellingRoute : GA.IGovernedProtectionCommandRoutePort
    {
        public int Calls { get; private set; }
        public ValueTask<GC.ProtectionCommandOutcome> DispatchAsync(GA.GovernedProtectionCommandEnvelope envelope, CancellationToken cancellationToken)
        {
            Calls++;
            return ValueTask.FromException<GC.ProtectionCommandOutcome>(new OperationCanceledException("ambiguous route cancellation"));
        }
    }

    private sealed class ThrowingProviderRoute : PA.IProviderEgressPort
    {
        public ValueTask<PA.ProviderFetchResult> FetchAsync(P.ProviderRouteIdentity route, P.DataProductId product, CancellationToken cancellationToken)
            => ValueTask.FromException<PA.ProviderFetchResult>(new IOException("provider route unavailable"));
    }
}
