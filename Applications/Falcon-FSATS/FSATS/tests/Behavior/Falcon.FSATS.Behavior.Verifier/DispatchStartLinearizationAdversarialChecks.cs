using T = Falcon.FSATS.Trading.Domain;
using TA = Falcon.FSATS.Trading.Application;

internal static class DispatchStartLinearizationAdversarialChecks
{
    internal static void Run()
    {
        ContainmentCannotSlipBetweenCommitAndExternalInvocation();
    }

    private static void ContainmentCannotSlipBetweenCommitAndExternalInvocation()
    {
        var queue = new TA.AccountScopedExecutionQueue();
        var account = new T.BrokerAccountContext("ALPACA", "LINEARIZATION", "PAPER");
        var identity = new TA.BrokerExecutionIdentity(account, "route-linear", "submission-linear", new T.OrderId("order-linear"));
        var safety = new T.PositionSafetyEnvelope(
            new T.PositionId("position-linear"),
            new T.InstrumentId("AAPL"),
            new T.Quantity(1),
            new T.Money(10, new T.Currency("USD")),
            "guardian", "protected", "exit", "known", new T.TrustEpoch(1))
        {
            AccountContext = account,
            ProtectionEvidenceReference = "evidence-linear-protection"
        };
        var reservationId = new T.ReservationId("reservation-linear");
        var preparation = new TA.TradingDecisionPipeline(new T.CapitalReservationLedger()).Prepare(
            new T.RiskRequest(
                new T.InstrumentId("AAPL"),
                new T.Quantity(1),
                new T.Money(10, new T.Currency("USD")),
                new T.Money(20, new T.Currency("USD")),
                true,
                true)
            {
                AccountContext = account
            },
            reservationId,
            new T.Money(100, new T.Currency("USD")),
            safety,
            new T.TrustEpoch(1),
            new T.TrustEpoch(1),
            identity);
        if (!preparation.Allowed || preparation.ReservationId is null || string.IsNullOrWhiteSpace(preparation.DecisionBindingReference))
            throw new InvalidOperationException("Q-LIN_DECISION_BINDING_SETUP_FAILED");
        var intent = new TA.OrderIntent(identity, new T.InstrumentId("AAPL"), new T.Quantity(1), new T.TrustEpoch(1), safety)
        {
            RiskReservationId = preparation.ReservationId,
            DecisionBindingReference = preparation.DecisionBindingReference
        };
        var work = new TA.QueuedExecutionWork("work-linear", intent, DateTimeOffset.UtcNow, "evidence-linear");

        if (!queue.Enqueue(work, out _) ||
            !queue.TryLeaseNext(out var lease) || lease is null ||
            !queue.TryBeginDispatch(lease, out var permit) || permit is null)
            throw new InvalidOperationException("Q-LIN_SETUP_FAILED");

        var broker = new SynchronouslyBlockingBrokerPort();
        var coordinator = new TA.ExecutionCoordinator(broker, queue);
        var execution = Task.Run(() => coordinator.SubmitOrReconcileAsync(intent, permit, CancellationToken.None).AsTask().GetAwaiter().GetResult());

        if (!broker.InvocationEntered.Wait(TimeSpan.FromSeconds(5)))
            throw new InvalidOperationException("Q-LIN_EXTERNAL_INVOCATION_DID_NOT_START");

        var containmentStarted = new ManualResetEventSlim(false);
        TA.ExecutionContainmentResult? containmentResult = null;
        var containment = Task.Run(() =>
        {
            containmentStarted.Set();
            containmentResult = queue.ContainAccount(account,
                new TA.ExecutionContainmentEvidence("linear-incident", "INCIDENT", "evidence-linear-incident", DateTimeOffset.UtcNow, new[] { account }));
        });

        containmentStarted.Wait();
        Thread.Sleep(25);

        if (containment.IsCompleted)
            throw new InvalidOperationException("Q-LIN_CONTAINMENT_INTERLEAVED_BEFORE_EXTERNAL_INVOCATION_RETURNED");

        broker.ReleaseInvocation.Set();
        Task.WaitAll(execution, containment);

        if (broker.Submits != 1)
            throw new InvalidOperationException("Q-LIN_EXTERNAL_INVOCATION_COUNT_INVALID");

        if (containmentResult is null || containmentResult.Reconcile.Count != 1)
            throw new InvalidOperationException("Q-LIN_INFLIGHT_DISPATCH_NOT_RECONCILIATION_OWNED");

        if (execution.Result.State != T.OrderState.ReconciliationRequired)
            throw new InvalidOperationException("Q-LIN_EXECUTION_DID_NOT_FAIL_CLOSED_AFTER_CONTAINMENT");

        var snapshot = queue.Snapshot(account).Single();
        if (snapshot.State != TA.ExecutionQueueState.ReconciliationRequired || snapshot.ContainmentIncidentId != "linear-incident")
            throw new InvalidOperationException("Q-LIN_QUEUE_TRUTH_NOT_RECONCILIATION_REQUIRED");
    }

    private sealed class SynchronouslyBlockingBrokerPort : TA.IBrokerExecutionPort
    {
        private int _submits;
        public int Submits => Volatile.Read(ref _submits);
        public ManualResetEventSlim InvocationEntered { get; } = new(false);
        public ManualResetEventSlim ReleaseInvocation { get; } = new(false);

        public ValueTask<TA.BrokerSubmissionResult> SubmitAsync(TA.OrderIntent intent, CancellationToken cancellationToken)
        {
            Interlocked.Increment(ref _submits);
            InvocationEntered.Set();
            if (!ReleaseInvocation.Wait(TimeSpan.FromSeconds(5)))
                throw new TimeoutException("TEST_EXTERNAL_INVOCATION_RELEASE_TIMEOUT");
            return ValueTask.FromResult(new TA.BrokerSubmissionResult(intent.ExecutionIdentity, true, true, "SUBMITTED"));
        }

        public ValueTask<TA.BrokerOrderSnapshot> ReconcileAsync(TA.BrokerExecutionIdentity identity, CancellationToken cancellationToken)
            => ValueTask.FromResult(new TA.BrokerOrderSnapshot(identity, T.OrderState.ReconciliationRequired, new T.Quantity(0), "RECONCILIATION_REQUIRED"));
    }
}
