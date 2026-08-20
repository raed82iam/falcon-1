using PA = Falcon.FSATS.FSAPMA.Application;
using PC = Falcon.FSATS.FSAPMA.Contracts;

var failures = new List<string>();
var checks = 0;
var now = DateTimeOffset.UtcNow;

PC.OperationalDataProjection Projection(string observationId, PC.DataTruthState truth, DateTimeOffset observedAt)
    => new(
        new PC.ObservationId(observationId),
        new PC.ProviderId("provider-1"),
        new PC.ProducerInstrumentId("provider-1", "AAPL"),
        new PC.DataProductId("last-price"),
        101.25m,
        observedAt,
        now.AddSeconds(-1),
        truth,
        "prov-1",
        "1.0",
        new PC.ProviderAccountId("provider-account-1"),
        "PAPER",
        "MARKET_DATA",
        "credential-ref-1");

PA.OperationalDataDeliveryEnvelope Envelope(string suffix, PC.OperationalDataProjection projection)
    => new(
        $"msg-{suffix}",
        "fsats.fsapma.operational-data",
        "1.0",
        PA.FSAPMAManifest.Current.ApplicationId,
        "FSATS-TRADING",
        "authority-data-1",
        projection.Provenance,
        $"corr-{suffix}",
        $"cause-{suffix}",
        $"idem-{suffix}",
        $"attempt-{suffix}",
        $"retry-{suffix}",
        PA.OperationalDataTrafficTruth.Operational,
        now.AddSeconds(-1),
        now.AddMinutes(1),
        TimeSpan.FromSeconds(30),
        null,
        $"evidence-{suffix}",
        projection);

void Check(bool condition, string message)
{
    checks++;
    if (!condition) failures.Add(message);
}

var currentProjection = Projection("obs-current", PC.DataTruthState.Current, now.AddSeconds(-2));
var currentEnvelope = Envelope("current", currentProjection);
var currentPort = new ControlledRoutePort((envelope, _) => ValueTask.FromResult(Bound(envelope, PA.OperationalDataDeliveryState.DeliveredCurrent, "ROUTE_DELIVERED")));
var currentService = new PA.GovernedOperationalDataDeliveryService(currentPort);
var currentResult = await currentService.DeliverAsync(currentEnvelope, now, CancellationToken.None);
Check(currentResult.State == PA.OperationalDataDeliveryState.DeliveredCurrent, "Current data plus current route outcome must remain DeliveredCurrent");

var rejectedProjection = Projection("obs-rejected", PC.DataTruthState.Current, now.AddSeconds(-2));
var rejectedEnvelope = Envelope("rejected", rejectedProjection);
var rejectedPort = new ControlledRoutePort((envelope, _) => ValueTask.FromResult(Bound(envelope, PA.OperationalDataDeliveryState.Rejected, "ROUTE_REJECTED")));
var rejectedService = new PA.GovernedOperationalDataDeliveryService(rejectedPort);
var rejectedResult = await rejectedService.DeliverAsync(rejectedEnvelope, now, CancellationToken.None);
Check(rejectedResult.State == PA.OperationalDataDeliveryState.Rejected && rejectedResult.ReasonCode == "ROUTE_REJECTED", "A route rejection must remain rejected and preserve its reason");
var rejectedDuplicate = await rejectedService.DeliverAsync(rejectedEnvelope, now, CancellationToken.None);
Check(rejectedDuplicate.State == PA.OperationalDataDeliveryState.Rejected && rejectedPort.Calls == 1, "An idempotent retry of a rejected delivery must not turn rejection into Duplicate or redispatch");

var routeDegradedProjection = Projection("obs-route-degraded", PC.DataTruthState.Current, now.AddSeconds(-2));
var routeDegradedEnvelope = Envelope("route-degraded", routeDegradedProjection);
var routeDegradedPort = new ControlledRoutePort((envelope, _) => ValueTask.FromResult(Bound(envelope, PA.OperationalDataDeliveryState.DeliveredDegraded, "ROUTE_DEGRADED")));
var routeDegradedService = new PA.GovernedOperationalDataDeliveryService(routeDegradedPort);
var routeDegradedResult = await routeDegradedService.DeliverAsync(routeDegradedEnvelope, now, CancellationToken.None);
Check(routeDegradedResult.State == PA.OperationalDataDeliveryState.DeliveredDegraded && routeDegradedResult.ReasonCode == "ROUTE_DEGRADED", "A degraded route outcome must not be upgraded to DeliveredCurrent");
var routeDegradedDuplicate = await routeDegradedService.DeliverAsync(routeDegradedEnvelope, now, CancellationToken.None);
Check(routeDegradedDuplicate.State == PA.OperationalDataDeliveryState.DeliveredDegraded && routeDegradedPort.Calls == 1, "An idempotent retry of degraded delivery must preserve degraded truth and not redispatch");

var staleProjection = Projection("obs-stale", PC.DataTruthState.Stale, now.AddMinutes(-5));
var staleEnvelope = Envelope("stale", staleProjection);
var stalePort = new ControlledRoutePort((envelope, _) => ValueTask.FromResult(Bound(envelope, PA.OperationalDataDeliveryState.DeliveredCurrent, "ROUTE_DELIVERED")));
var staleService = new PA.GovernedOperationalDataDeliveryService(stalePort);
var staleResult = await staleService.DeliverAsync(staleEnvelope, now, CancellationToken.None);
Check(staleResult.State == PA.OperationalDataDeliveryState.DeliveredDegraded && staleResult.ReasonCode == "OPERATIONAL_DATA_EXPLICITLY_DEGRADED", "Semantically stale data must stay degraded even if the route reports transport success");

var mismatchProjection = Projection("obs-mismatch", PC.DataTruthState.Current, now.AddSeconds(-2));
var mismatchEnvelope = Envelope("mismatch", mismatchProjection);
var mismatchPort = new ControlledRoutePort((envelope, _) => ValueTask.FromResult(new PA.OperationalDataDeliveryResult(
    PA.OperationalDataDeliveryState.DeliveredCurrent,
    "wrong-observation",
    envelope.ConsumerApplicationId,
    "ROUTE_DELIVERED",
    envelope.CorrelationId,
    now,
    envelope.Projection.ProviderRouteNamespace)));
var mismatchService = new PA.GovernedOperationalDataDeliveryService(mismatchPort);
var mismatchResult = await mismatchService.DeliverAsync(mismatchEnvelope, now, CancellationToken.None);
Check(mismatchResult.State == PA.OperationalDataDeliveryState.DeliveryOutcomeUnknown && mismatchResult.ReasonCode == "DELIVERY_OUTCOME_BINDING_MISMATCH_AMBIGUOUS", "A post-dispatch route outcome bound to the wrong observation must preserve ambiguous delivery truth");

var wrongRouteProjection = Projection("obs-wrong-route", PC.DataTruthState.Current, now.AddSeconds(-2));
var wrongRouteEnvelope = Envelope("wrong-route", wrongRouteProjection);
var wrongRoutePort = new ControlledRoutePort((envelope, _) => ValueTask.FromResult(new PA.OperationalDataDeliveryResult(
    PA.OperationalDataDeliveryState.DeliveredCurrent,
    envelope.Projection.ObservationId.Value,
    envelope.ConsumerApplicationId,
    "ROUTE_DELIVERED",
    envelope.CorrelationId,
    now,
    "OTHER|PROVIDER|ROUTE")));
var wrongRouteService = new PA.GovernedOperationalDataDeliveryService(wrongRoutePort);
var wrongRouteResult = await wrongRouteService.DeliverAsync(wrongRouteEnvelope, now, CancellationToken.None);
Check(wrongRouteResult.State == PA.OperationalDataDeliveryState.DeliveryOutcomeUnknown && wrongRouteResult.ReasonCode == "DELIVERY_OUTCOME_BINDING_MISMATCH_AMBIGUOUS", "A post-dispatch route outcome bound to the wrong provider account/API route must preserve ambiguous delivery truth");

var nullProjection = Projection("obs-null", PC.DataTruthState.Current, now.AddSeconds(-2));
var nullEnvelope = Envelope("null", nullProjection);
var nullPort = new ControlledRoutePort((_, _) => ValueTask.FromResult<PA.OperationalDataDeliveryResult>(null!));
var nullService = new PA.GovernedOperationalDataDeliveryService(nullPort);
var nullResult = await nullService.DeliverAsync(nullEnvelope, now, CancellationToken.None);
Check(nullResult.State == PA.OperationalDataDeliveryState.DeliveryOutcomeUnknown && nullResult.ReasonCode == "NULL_DELIVERY_OUTCOME_AMBIGUOUS", "A null post-dispatch route outcome must remain unknown rather than fabricating non-delivery");

var exceptionProjection = Projection("obs-exception", PC.DataTruthState.Current, now.AddSeconds(-2));
var exceptionEnvelope = Envelope("exception", exceptionProjection);
var exceptionPort = new ControlledRoutePort((_, _) => ValueTask.FromException<PA.OperationalDataDeliveryResult>(new InvalidOperationException("route failure")));
var exceptionService = new PA.GovernedOperationalDataDeliveryService(exceptionPort);
var exceptionResult = await exceptionService.DeliverAsync(exceptionEnvelope, now, CancellationToken.None);
Check(exceptionResult.State == PA.OperationalDataDeliveryState.DeliveryOutcomeUnknown && exceptionResult.ReasonCode == "DELIVERY_ROUTE_FAILURE_AMBIGUOUS:InvalidOperationException", "A route exception after dispatch begins must become attributable unknown delivery truth");

var blankReasonProjection = Projection("obs-blank-reason", PC.DataTruthState.Current, now.AddSeconds(-2));
var blankReasonEnvelope = Envelope("blank-reason", blankReasonProjection);
var blankReasonPort = new ControlledRoutePort((envelope, _) => ValueTask.FromResult(Bound(envelope, PA.OperationalDataDeliveryState.DeliveredCurrent, "   ")));
var blankReasonService = new PA.GovernedOperationalDataDeliveryService(blankReasonPort);
var blankReasonResult = await blankReasonService.DeliverAsync(blankReasonEnvelope, now, CancellationToken.None);
Check(blankReasonResult.State == PA.OperationalDataDeliveryState.DeliveryOutcomeUnknown && blankReasonResult.ReasonCode == "DELIVERY_OUTCOME_REASON_MISSING_AMBIGUOUS", "A post-dispatch route outcome without a reason code must preserve ambiguous delivery truth");

var raceProjection = Projection("obs-race", PC.DataTruthState.Current, now.AddSeconds(-2));
var raceEnvelope = Envelope("race", raceProjection);
var racePort = new DelayedRoutePort(now);
var raceService = new PA.GovernedOperationalDataDeliveryService(racePort);
var raceTask1 = raceService.DeliverAsync(raceEnvelope, now, CancellationToken.None).AsTask();
var raceTask2 = raceService.DeliverAsync(raceEnvelope, now, CancellationToken.None).AsTask();
var raceResults = await Task.WhenAll(raceTask1, raceTask2);
Check(racePort.Calls == 1, "Concurrent identical idempotent delivery calls must dispatch the route exactly once");
Check(raceResults.Count(r => r.State == PA.OperationalDataDeliveryState.DeliveredCurrent) == 1 && raceResults.Count(r => r.State == PA.OperationalDataDeliveryState.Duplicate) == 1, "Concurrent identical idempotent calls must produce one primary result and one duplicate view");

var raceConflictEnvelope = raceEnvelope with { MessageId = "msg-race-conflict" };
var raceConflictResult = await raceService.DeliverAsync(raceConflictEnvelope, now, CancellationToken.None);
Check(raceConflictResult.State == PA.OperationalDataDeliveryState.Rejected && raceConflictResult.ReasonCode == "IDEMPOTENCY_CONFLICT" && racePort.Calls == 1, "A semantically different envelope reusing an idempotency identity must fail closed without redispatch");

var cancellationProjection = Projection("obs-cancel", PC.DataTruthState.Current, now.AddSeconds(-2));
var cancellationEnvelope = Envelope("cancel", cancellationProjection);
var cancellationPort = new ControlledRoutePort((envelope, _) => ValueTask.FromResult(Bound(envelope, PA.OperationalDataDeliveryState.DeliveredCurrent, "ROUTE_DELIVERED")));
var cancellationService = new PA.GovernedOperationalDataDeliveryService(cancellationPort);
using (var cancelled = new CancellationTokenSource())
{
    cancelled.Cancel();
    var cancellationObserved = false;
    try { await cancellationService.DeliverAsync(cancellationEnvelope, now, cancelled.Token); }
    catch (OperationCanceledException) { cancellationObserved = true; }
    Check(cancellationObserved && cancellationPort.Calls == 0, "Cancellation before idempotency-gate acquisition must not dispatch or cache a false delivery result");
}

var postCancellationResult = await cancellationService.DeliverAsync(cancellationEnvelope, now, CancellationToken.None);
Check(postCancellationResult.State == PA.OperationalDataDeliveryState.DeliveredCurrent && cancellationPort.Calls == 1, "A later valid attempt after caller cancellation must remain eligible and must not inherit poisoned idempotency state");

if (failures.Count > 0)
{
    Console.Error.WriteLine($"FSATS OPERATIONAL DATA OUTCOME VERIFIER: FAIL ({checks - failures.Count}/{checks})");
    foreach (var failure in failures) Console.Error.WriteLine(" - " + failure);
    return 1;
}

Console.WriteLine($"FSATS OPERATIONAL DATA OUTCOME VERIFIER: PASS ({checks}/{checks})");
return 0;

PA.OperationalDataDeliveryResult Bound(PA.OperationalDataDeliveryEnvelope envelope, PA.OperationalDataDeliveryState state, string reason)
    => new(state, envelope.Projection.ObservationId.Value, envelope.ConsumerApplicationId, reason, envelope.CorrelationId, now, envelope.Projection.ProviderRouteNamespace);

sealed class ControlledRoutePort : PA.IGovernedOperationalDataRoutePort
{
    private readonly Func<PA.OperationalDataDeliveryEnvelope, CancellationToken, ValueTask<PA.OperationalDataDeliveryResult>> _resultFactory;
    public ControlledRoutePort(Func<PA.OperationalDataDeliveryEnvelope, CancellationToken, ValueTask<PA.OperationalDataDeliveryResult>> resultFactory) => _resultFactory = resultFactory;
    public int Calls { get; private set; }
    public ValueTask<PA.OperationalDataDeliveryResult> DeliverAsync(PA.OperationalDataDeliveryEnvelope envelope, CancellationToken cancellationToken) { Calls++; return _resultFactory(envelope, cancellationToken); }
}

sealed class DelayedRoutePort : PA.IGovernedOperationalDataRoutePort
{
    private readonly DateTimeOffset _effectiveAt;
    private int _calls;
    public DelayedRoutePort(DateTimeOffset effectiveAt) => _effectiveAt = effectiveAt;
    public int Calls => Volatile.Read(ref _calls);
    public async ValueTask<PA.OperationalDataDeliveryResult> DeliverAsync(PA.OperationalDataDeliveryEnvelope envelope, CancellationToken cancellationToken)
    {
        Interlocked.Increment(ref _calls);
        await Task.Delay(50, cancellationToken);
        return new PA.OperationalDataDeliveryResult(PA.OperationalDataDeliveryState.DeliveredCurrent, envelope.Projection.ObservationId.Value, envelope.ConsumerApplicationId, "ROUTE_DELIVERED", envelope.CorrelationId, _effectiveAt, envelope.Projection.ProviderRouteNamespace);
    }
}
