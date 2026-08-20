using T = Falcon.FSATS.Trading.Domain;
using TA = Falcon.FSATS.Trading.Application;
using P = Falcon.FSATS.FSAPMA.Domain;
using PA = Falcon.FSATS.FSAPMA.Application;
using PC = Falcon.FSATS.FSAPMA.Contracts;
using G = Falcon.FSATS.TradingGuardian.Domain;
using GA = Falcon.FSATS.TradingGuardian.Application;
using GC = Falcon.FSATS.TradingGuardian.Contracts;
using S = Falcon.FSATS.FSTSimA.Domain;
using R = Falcon.FSATS.ResourceManagement.Domain;
using RA = Falcon.FSATS.ResourceManagement.Application;

var failures = new List<string>();
var checks = 0;

Check(T.UnifiedRiskGate.Evaluate(new T.RiskRequest(new T.InstrumentId("A"), new T.Quantity(10), new T.Money(5, new T.Currency("USD")), new T.Money(10, new T.Currency("USD")), true, false)).Decision == T.RiskDecision.Denied, "Risk must deny untrusted data");
Check(T.UnifiedRiskGate.Evaluate(new T.RiskRequest(new T.InstrumentId("A"), new T.Quantity(10), new T.Money(20, new T.Currency("USD")), new T.Money(10, new T.Currency("USD")), true, true)).Decision == T.RiskDecision.Reduced, "Risk must reduce oversized loss");
Check(!T.TrustEpochFence.IsEligible(new T.TrustEpoch(1), new T.TrustEpoch(2), true), "Stale trust epoch must fence risk-increasing work");
Check(T.TrustEpochFence.IsEligible(new T.TrustEpoch(1), new T.TrustEpoch(2), false), "Non-risk-increasing protective work may remain eligible");

var lifecycle = new T.OrderLifecycle();
lifecycle.SubmissionAttempt(); lifecycle.BrokerAck(); lifecycle.PartialFill(); lifecycle.CancelRequest(); lifecycle.Cancelled();
Check(lifecycle.State == T.OrderState.Cancelled, "Order lifecycle partial-fill cancel path");
var ambiguous = new T.OrderLifecycle(); ambiguous.MarkAmbiguous();
Check(ambiguous.State == T.OrderState.ReconciliationRequired, "Ambiguous order must enter reconciliation");

var provider = new P.ProviderController();
var selected = provider.Select(new[]
{
    (new P.ProviderId("bad"), P.CapabilityState.Supported, P.QualityState.Stale, 100),
    (new P.ProviderId("good"), P.CapabilityState.Supported, P.QualityState.Healthy, 50)
});
Check(selected?.Value == "good", "ProviderController must reject stale provider");
var anomaly = new P.AnomalyDetector().Evaluate(10m, 10m, DateTimeOffset.UnixEpoch, DateTimeOffset.UnixEpoch.AddMinutes(10), TimeSpan.FromMinutes(1));
Check(anomaly.State == P.QualityState.Stale, "Stale observation must be explicit");

var actionsUnknown = G.DeterministicSafetyKernel.Decide(new G.SafetyContext(false, false, false, true, true));
Check(actionsUnknown.Contains(G.SafetyAction.Reconcile) && !actionsUnknown.Contains(G.SafetyAction.Exit), "Unknown execution truth must reconcile, not blind exit");
Check(actionsUnknown.Contains(G.SafetyAction.DenyExpansion), "Safety kernel must deny expansion");
var actionsKnown = G.DeterministicSafetyKernel.Decide(new G.SafetyContext(false, true, false, true, true));
Check(actionsKnown.Contains(G.SafetyAction.Exit), "Known unprotected exposure may use authorized exit policy");

var generator = new S.SyntheticMarketGenerator();
var a = generator.Generate(77, 20, 100m, new S.SimulationInstant(0), "STRESS");
var b = generator.Generate(77, 20, 100m, new S.SimulationInstant(0), "STRESS");
Check(a.SequenceEqual(b), "Synthetic market same seed must reproduce exactly");
var assessor = new S.ValidationAssessor();
Check(assessor.Assess(true, false, 1m).Recommendation == "NOT_READY", "Validation must reject non-independent calibration evidence");
Check(assessor.Assess(true, true, 0.95m).Recommendation == "READY_FOR_PAPER_QUALIFICATION_REVIEW", "Qualified simulation may only recommend Paper review");

var validClaim = new R.ResourceClaim("Trading", "CPU", 10, 8, 5, 12, 2, 80, true, true);
var staleClaim = validClaim with { ApplicationId = "FSTSimA", Fresh = false };
Check(R.DemandIntegrityEvaluator.IsEligible(validClaim), "Fresh trusted resource claim eligible");
Check(!R.DemandIntegrityEvaluator.IsEligible(staleClaim), "Stale resource claim must be ineligible");
var now = DateTimeOffset.UnixEpoch.AddDays(1);
var envelope = new R.FoundationEnvelope("env-1", "CPU", 20, DateTimeOffset.UnixEpoch, DateTimeOffset.MaxValue, false);
Check(R.ResourceEpochFence.IsCurrent(new R.CoordinationEpoch(2), new R.CoordinationEpoch(2), "env-1", envelope, now), "Current resource epoch eligible");
Check(!R.ResourceEpochFence.IsCurrent(new R.CoordinationEpoch(1), new R.CoordinationEpoch(2), "env-1", envelope, now), "Stale resource epoch fenced");

var foundationNow = DateTimeOffset.UtcNow;
var validProjection = new RA.FoundationResourceStateProjection(
    "APP-RSC", "CPU", "epoch-7", "grant-1", 20m, 20m, 25m, 20m,
    true, RA.FoundationPressureState.Constrained, 8000, true,
    "decision-1", "basis-1", foundationNow.AddSeconds(-2), new string('A', 64));
Check(RA.FoundationResourceBindingGuards.IsUsable(validProjection, "APP-RSC", "CPU", "epoch-7", foundationNow), "Exact Foundation resource projection should be consumable");
Check(!RA.FoundationResourceBindingGuards.IsUsable(validProjection, "APP-RSC", "CPU", "epoch-8", foundationNow), "Stale/mismatched Foundation resource epoch must fail closed");

var validSignal = new RA.FoundationLoadSheddingSignal(
    "APP-RSC", "CPU", "epoch-7", RA.FoundationLoadSheddingClass.ComplianceReductionRequired,
    15m, 5m, validProjection.FoundationIdentitySha256, "basis-1", foundationNow.AddSeconds(-1), new string('B', 64));
Check(RA.FoundationResourceBindingGuards.IsCurrent(validSignal, validProjection, foundationNow), "Exact load-shedding signal should bind to exact projection");
Check(!RA.FoundationResourceBindingGuards.IsCurrent(validSignal with { ProjectionIdentitySha256 = new string('C', 64) }, validProjection, foundationNow), "Load-shedding signal with wrong projection identity must fail closed");

var fakePort = new FakeFoundationResourceBindingPort(validProjection, validSignal);
var bindingService = new RA.FoundationResourceBindingService(fakePort);
var state = await bindingService.ReadCurrentStateAsync("APP-RSC", "CPU", "epoch-7", foundationNow, CancellationToken.None);
Check(state is not null, "Binding service should accept exact Foundation resource state");
var rejectedState = await bindingService.ReadCurrentStateAsync("APP-RSC", "CPU", "epoch-wrong", foundationNow, CancellationToken.None);
Check(rejectedState is null, "Binding service must reject mismatched Foundation resource state");

var targetClaim = new R.ResourceClaim("Trading", "CPU", 10m, 9m, 7m, 16m, 1m, 90, true, true);
fakePort.OutcomeFactory = request => new RA.FoundationAdditionalResourceOutcome(
    true, RA.FoundationResourceDecisionKind.PartialGrant, 3m, request.Unit, "fd-1", request.RequestId,
    request.EpochId, "foundation-outcome-1", foundationNow, foundationNow.AddMinutes(5));
var residualResult = await bindingService.RequestResidualAsync(
    "req-1", targetClaim, 2m, "evidence-1", "rsc-instance-1", "resource-coordinator",
    "epoch-7", "fsats-scope", "cpu-unit", "corr-1", "cause-1",
    foundationNow.AddSeconds(-1), foundationNow.AddMinutes(5), foundationNow, CancellationToken.None);
Check(residualResult.Bound && residualResult.Granted && residualResult.GrantedAmount == 3m, "Partial Foundation grant should remain explicitly bounded and attributable");

var routeNow = DateTimeOffset.UtcNow;
var protectionTarget = new GC.ProtectionTarget(GC.ProtectionTargetKind.BrokerAccount, "ALPACA", "PA-ACCOUNT-A", "PAPER");
var protectionCommand = new GC.ProtectionCommand(
    new GC.CommandId("cmd-p1"), GC.ProtectionCommandType.NewRiskFreeze, "TARGET", protectionTarget,
    "authority-p1", "risk-freeze", new GC.ProtectionEpoch(7), routeNow.AddSeconds(-1), routeNow.AddMinutes(1), "corr-p1", "cause-p1");
var protectionEnvelope = new GA.GovernedProtectionCommandEnvelope(
    "msg-p1", "fsats.guardian.protection", "1.0", GA.TradingGuardianManifest.Current.ApplicationId, "TARGET",
    "authority-p1", "prov-p1", "corr-p1", "cause-p1", "idem-p1", "attempt-p1", "retry-p1",
    GA.ProtectionTrafficTruth.Operational, routeNow.AddSeconds(-1), routeNow.AddMinutes(1), "evidence-p1", protectionCommand);
var protectionPort = new FakeProtectionRoutePort();
var protectionDispatcher = new GA.GovernedProtectionCommandDispatcher(protectionPort);
var protectionOutcome = await protectionDispatcher.DispatchAsync(protectionEnvelope, 7, routeNow, CancellationToken.None);
Check(protectionOutcome.State == GC.ProtectionOutcomeState.Applied && protectionPort.Calls == 1, "Valid governed protection command should dispatch once");
var protectionDuplicate = await protectionDispatcher.DispatchAsync(protectionEnvelope, 7, routeNow, CancellationToken.None);
Check(protectionDuplicate.State == GC.ProtectionOutcomeState.Applied && protectionPort.Calls == 1, "Protection duplicate must be idempotent");
var staleProtection = protectionEnvelope with { MessageId = "msg-p2", IdempotencyId = "idem-p2", Command = protectionCommand with { Epoch = new GC.ProtectionEpoch(6), CommandId = new GC.CommandId("cmd-p2") } };
var staleProtectionOutcome = await protectionDispatcher.DispatchAsync(staleProtection, 7, routeNow, CancellationToken.None);
Check(staleProtectionOutcome.State == GC.ProtectionOutcomeState.Rejected && protectionPort.Calls == 1, "Stale protection epoch must fail closed before dispatch");

var dataProjection = new PC.OperationalDataProjection(
    new PC.ObservationId("obs-d1"), new PC.ProviderId("provider-1"), new PC.ProducerInstrumentId("provider-1", "AAPL"),
    new PC.DataProductId("last-price"), 101.25m, routeNow.AddSeconds(-2), routeNow.AddSeconds(-1),
    PC.DataTruthState.Current, "prov-d1", "1.0", new PC.ProviderAccountId("provider-account-1"), "PAPER", "MARKET_DATA", "credential-ref-1");
var dataEnvelope = new PA.OperationalDataDeliveryEnvelope(
    "msg-d1", "fsats.fsapma.operational-data", "1.0", PA.FSAPMAManifest.Current.ApplicationId, TA.TradingManifest.Current.ApplicationId,
    "authority-data-1", "prov-d1", "corr-d1", "cause-d1", "idem-d1", "attempt-d1", "retry-d1",
    PA.OperationalDataTrafficTruth.Operational, routeNow.AddSeconds(-1), routeNow.AddMinutes(1), TimeSpan.FromSeconds(30), null,
    "evidence-d1", dataProjection);
var dataPort = new FakeOperationalDataRoutePort();
var dataService = new PA.GovernedOperationalDataDeliveryService(dataPort);
var dataResult = await dataService.DeliverAsync(dataEnvelope, routeNow, CancellationToken.None);
Check(dataResult.State == PA.OperationalDataDeliveryState.DeliveredCurrent, "Current attributable operational data should deliver as current");
var dataDuplicate = await dataService.DeliverAsync(dataEnvelope, routeNow, CancellationToken.None);
Check(dataDuplicate.State == PA.OperationalDataDeliveryState.Duplicate && dataPort.Calls == 1, "Operational data duplicate must be idempotent and not redispatch");
var degradedProjection = dataProjection with { ObservationId = new PC.ObservationId("obs-d2"), Truth = PC.DataTruthState.Stale, ObservedAt = routeNow.AddMinutes(-5), ReceivedAt = routeNow.AddSeconds(-1) };
var degradedEnvelope = dataEnvelope with { MessageId = "msg-d2", IdempotencyId = "idem-d2", DeliveryAttemptId = "attempt-d2", Projection = degradedProjection };
var degradedResult = await dataService.DeliverAsync(degradedEnvelope, routeNow, CancellationToken.None);
Check(degradedResult.State == PA.OperationalDataDeliveryState.DeliveredDegraded, "Stale data may be delivered only with explicit degraded truth");
var replayData = dataEnvelope with { MessageId = "msg-d3", IdempotencyId = "idem-d3", TrafficTruth = PA.OperationalDataTrafficTruth.Replay };
var replayDataResult = await dataService.DeliverAsync(replayData, routeNow, CancellationToken.None);
Check(replayDataResult.State == PA.OperationalDataDeliveryState.Rejected && dataPort.Calls == 2, "Replay provider data must not become operational data");

var eventDigest = new string('A', 64);
var tradingIngress = new TA.GovernedApplicationEventIngress();
var tradingEvent = new TA.GovernedApplicationEventEnvelope(
    "evt-t1", "provider.data-quality", PA.FSAPMAManifest.Current.ApplicationId, TA.TradingManifest.Current.ApplicationId,
    "schema-event", "1.0", "msg-e1", "corr-e1", "cause-e1", "idem-e1",
    TA.ApplicationEventTruth.AuthoritativeOperational, TA.ApplicationEventRelation.None, null, null, 0,
    routeNow.AddSeconds(-1), "evidence-e1", eventDigest);
Check(tradingIngress.Consume(tradingEvent, routeNow).State == TA.ApplicationEventIngressState.AcceptedOperational, "Trading must accept valid authoritative operational event");
Check(tradingIngress.Consume(tradingEvent, routeNow).State == TA.ApplicationEventIngressState.Duplicate, "Trading event duplicate must be idempotent");
var tradingReplay = tradingEvent with { EventId = "evt-t2", MessageId = "msg-e2", IdempotencyId = "idem-e2", Truth = TA.ApplicationEventTruth.Replay, Relation = TA.ApplicationEventRelation.ReplayOf, RelatedEventId = "evt-t1" };
Check(tradingIngress.Consume(tradingReplay, routeNow).State == TA.ApplicationEventIngressState.AcceptedEvidence, "Trading replay may be consumed only as non-operational evidence");
var replayEscalation = tradingReplay with { EventId = "evt-t3", MessageId = "msg-e3", IdempotencyId = "idem-e3", Truth = TA.ApplicationEventTruth.AuthoritativeOperational };
Check(tradingIngress.Consume(replayEscalation, routeNow).State == TA.ApplicationEventIngressState.Rejected, "Replay lineage must never escalate to authoritative operational truth");

var providerIngress = new PA.GovernedApplicationEventIngress();
var providerEvidenceEvent = new PA.GovernedApplicationEventEnvelope(
    "evt-p1", "guardian.incident-evidence", GA.TradingGuardianManifest.Current.ApplicationId, PA.FSAPMAManifest.Current.ApplicationId,
    "schema-event", "1.0", "msg-pe1", "corr-pe1", "cause-pe1", "idem-pe1",
    PA.ApplicationEventTruth.NonAuthoritativeEvidence, PA.ApplicationEventRelation.None, null, "provider-key", 1,
    routeNow.AddSeconds(-1), "evidence-pe1", eventDigest);
Check(providerIngress.Consume(providerEvidenceEvent, routeNow).State == PA.ApplicationEventIngressState.AcceptedEvidence, "FSAPMA must preserve non-authoritative evidence classification");
var providerOutOfOrder = providerEvidenceEvent with { EventId = "evt-p2", MessageId = "msg-pe2", IdempotencyId = "idem-pe2", SequenceNumber = 1 };
Check(providerIngress.Consume(providerOutOfOrder, routeNow).State == PA.ApplicationEventIngressState.Rejected, "FSAPMA must reject non-monotonic per-key event sequence");

var guardianIngress = new GA.GovernedApplicationEventIngress();
var guardianEvidenceEvent = new GA.GovernedApplicationEventEnvelope(
    "evt-g1", "trading.safety-state", TA.TradingManifest.Current.ApplicationId, GA.TradingGuardianManifest.Current.ApplicationId,
    "schema-event", "1.0", "msg-ge1", "corr-ge1", "cause-ge1", "idem-ge1",
    GA.ApplicationEventTruth.NonAuthoritativeEvidence, GA.ApplicationEventRelation.None, null, null, 0,
    routeNow.AddSeconds(-1), "evidence-ge1", eventDigest);
Check(guardianIngress.Consume(guardianEvidenceEvent, routeNow).State == GA.ApplicationEventIngressState.AcceptedEvidence, "Guardian must accept attributable incident evidence without converting it to business authority");
var guardianWrongTarget = guardianEvidenceEvent with { EventId = "evt-g2", MessageId = "msg-ge2", IdempotencyId = "idem-ge2", SubscriberApplicationId = TA.TradingManifest.Current.ApplicationId };
Check(guardianIngress.Consume(guardianWrongTarget, routeNow).State == GA.ApplicationEventIngressState.Rejected, "Guardian must reject events addressed to another Application");

CapitalReservationLedgerAdversarialChecks.Run();
EventIngressOrderingAdversarialChecks.Run();
GuardianDispatcherAdversarialChecks.Run();
GuardianIdempotencySemanticChecks.Run();
Part2RedTeamFollowupChecks.Run();
Part2RemediationAdversarialChecks.Run();
BrokerRecoveryIdentityAdversarialChecks.Run();
BrokerAccountIsolationAdversarialChecks.Run();
BroadRedTeamAdversarialChecks.Run();

if (failures.Count > 0)
{
    Console.Error.WriteLine($"FSATS BEHAVIOR VERIFIER: FAIL ({checks - failures.Count}/{checks})");
    foreach (var failure in failures) Console.Error.WriteLine(" - " + failure);
    return 1;
}

Console.WriteLine($"FSATS BEHAVIOR VERIFIER: PASS ({checks}/{checks})");
return 0;

void Check(bool condition, string message)
{
    checks++;
    if (!condition) failures.Add(message);
}

sealed class FakeFoundationResourceBindingPort : RA.IFoundationResourceBindingPort
{
    private readonly RA.FoundationResourceStateProjection _projection;
    private readonly RA.FoundationLoadSheddingSignal _signal;

    public FakeFoundationResourceBindingPort(RA.FoundationResourceStateProjection projection, RA.FoundationLoadSheddingSignal signal)
    {
        _projection = projection;
        _signal = signal;
        OutcomeFactory = request => new RA.FoundationAdditionalResourceOutcome(
            false, RA.FoundationResourceDecisionKind.Deny, 0m, request.Unit, "unbound", request.RequestId,
            request.EpochId, "unbound", DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddMinutes(1));
    }

    public Func<RA.FoundationAdditionalResourceRequest, RA.FoundationAdditionalResourceOutcome> OutcomeFactory { get; set; }

    public ValueTask<RA.FoundationResourceStateProjection?> ReadApplicationStateAsync(string applicationId, string resourceClass, string expectedEpochId, CancellationToken cancellationToken)
        => ValueTask.FromResult<RA.FoundationResourceStateProjection?>(_projection);

    public ValueTask<RA.FoundationLoadSheddingSignal?> ReadLoadSheddingSignalAsync(string applicationId, string resourceClass, string expectedEpochId, CancellationToken cancellationToken)
        => ValueTask.FromResult<RA.FoundationLoadSheddingSignal?>(_signal);

    public ValueTask<RA.FoundationAdditionalResourceOutcome> RequestAdditionalAsync(RA.FoundationAdditionalResourceRequest request, CancellationToken cancellationToken)
        => ValueTask.FromResult(OutcomeFactory(request));
}

sealed class FakeProtectionRoutePort : GA.IGovernedProtectionCommandRoutePort
{
    public int Calls { get; private set; }
    public ValueTask<GC.ProtectionCommandOutcome> DispatchAsync(GA.GovernedProtectionCommandEnvelope envelope, CancellationToken cancellationToken)
    {
        Calls++;
        return ValueTask.FromResult(new GC.ProtectionCommandOutcome(
            envelope.Command.CommandId,
            GC.ProtectionOutcomeState.Applied,
            envelope.Command.TargetApplication,
            envelope.Command.Target,
            "PROTECTION_APPLIED",
            DateTimeOffset.UtcNow,
            envelope.CorrelationId));
    }
}

sealed class FakeOperationalDataRoutePort : PA.IGovernedOperationalDataRoutePort
{
    public int Calls { get; private set; }
    public ValueTask<PA.OperationalDataDeliveryResult> DeliverAsync(PA.OperationalDataDeliveryEnvelope envelope, CancellationToken cancellationToken)
    {
        Calls++;
        return ValueTask.FromResult(new PA.OperationalDataDeliveryResult(
            PA.OperationalDataDeliveryState.DeliveredCurrent,
            envelope.Projection.ObservationId.Value,
            envelope.ConsumerApplicationId,
            "ROUTE_DELIVERED",
            envelope.CorrelationId,
            DateTimeOffset.UtcNow,
            envelope.Projection.ProviderRouteNamespace));
    }
}
