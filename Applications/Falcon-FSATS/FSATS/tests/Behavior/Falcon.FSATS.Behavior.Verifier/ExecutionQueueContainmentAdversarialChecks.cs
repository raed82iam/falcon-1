using T = Falcon.FSATS.Trading.Domain;
using TA = Falcon.FSATS.Trading.Application;

internal static class ExecutionQueueContainmentAdversarialChecks
{
    internal static void Run()
    {
        AccountContainmentPurgesOnlyAffectedPending();
        LeasedWorkCannotEscapeContainment();
        PermitIssuedBeforeContainmentCannotReachBroker();
        InFlightContainmentForcesReconciliation();
        ExpiredLeaseCannotStrandOrDoubleDispatchWork();
        ContainedAccountRejectsNewQueueWork();
        CancelledWorkCannotResurrectAfterRecovery();
        BrokerWideContainmentIsStickyForNewAccounts();
        BrokerWideReleaseRequiresDeclaredAndObservedAccounts();
        SameWorkIdAcrossAccountsRemainsIndependent();
        BrokerIdentityCaseCannotBypassContainment();
        DispatchPermitIsSingleUse();
        SubmissionExceptionFailsClosedToReconciliation();
        ConcurrentEnqueueContainmentLeavesNoPendingWork();
    }

    private static T.BrokerAccountContext Account(string broker, string account, string environment = "PAPER") => new(broker, account, environment);

    private static TA.OrderIntent Intent(T.BrokerAccountContext account, string suffix)
    {
        var identity = new TA.BrokerExecutionIdentity(account, $"route-{suffix}", $"submission-{suffix}", new T.OrderId($"order-{suffix}"));
        var safety = new T.PositionSafetyEnvelope(new T.PositionId($"position-{suffix}"), new T.InstrumentId("AAPL"), new T.Quantity(1),
            new T.Money(10, new T.Currency("USD")), "guardian", "protected", "exit", "known", new T.TrustEpoch(1))
        {
            AccountContext = account,
            ProtectionEvidenceReference = $"evidence-protection-{suffix}"
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
                AccountContext = account
            },
            new T.ReservationId($"reservation-{suffix}"),
            new T.Money(100, new T.Currency("USD")),
            safety,
            new T.TrustEpoch(1),
            new T.TrustEpoch(1),
            identity);
        if (!preparation.Allowed || preparation.ReservationId is null || string.IsNullOrWhiteSpace(preparation.DecisionBindingReference))
            throw new InvalidOperationException($"{suffix}_DECISION_BINDING_SETUP_FAILED");
        return new TA.OrderIntent(identity, new T.InstrumentId("AAPL"), new T.Quantity(1), new T.TrustEpoch(1), safety)
        {
            RiskReservationId = preparation.ReservationId,
            DecisionBindingReference = preparation.DecisionBindingReference
        };
    }

    private static TA.QueuedExecutionWork Work(T.BrokerAccountContext account, string suffix)
        => new($"work-{suffix}", Intent(account, suffix), DateTimeOffset.UtcNow, $"evidence-{suffix}");

    private static TA.ExecutionContainmentEvidence Evidence(string incident, params T.BrokerAccountContext[] accounts)
        => new(incident, "INCIDENT", $"evidence-{incident}", DateTimeOffset.UtcNow, accounts);

    private static TA.BrokerAccountReconciliationEvidence Reconciliation(T.BrokerAccountContext account, bool complete, string suffix)
    {
        var now = DateTimeOffset.UtcNow;
        var dimensions = Enum.GetValues<TA.BrokerReconciliationDimension>()
            .Select(x => new TA.BrokerReconciliationDimensionEvidence(x, complete, $"reconcile-{suffix}:{x}", now))
            .ToArray();
        return new(account, complete, complete, complete, complete, complete, complete, complete, $"reconcile-{suffix}", now, dimensions);
    }

    private static (TA.ExecutionDispatchPermit Permit, TA.OrderIntent Intent) PrepareDispatch(TA.AccountScopedExecutionQueue queue, T.BrokerAccountContext account, string suffix)
    {
        var work = Work(account, suffix);
        if (!queue.Enqueue(work, out _) || !queue.TryLeaseNext(out var lease) || lease is null || !queue.TryBeginDispatch(lease, out var permit) || permit is null)
            throw new InvalidOperationException($"{suffix}_DISPATCH_SETUP_FAILED");
        return (permit, work.Intent);
    }

    private static void AccountContainmentPurgesOnlyAffectedPending()
    {
        var queue = new TA.AccountScopedExecutionQueue();
        var a = Account("ALPACA", "A"); var b = Account("ALPACA", "B");
        if (!queue.Enqueue(Work(a, "a1"), out _) || !queue.Enqueue(Work(a, "a2"), out _) || !queue.Enqueue(Work(b, "b1"), out _)) throw new InvalidOperationException("Q-01_SETUP");
        var result = queue.ContainAccount(a, Evidence("account-a", a));
        if (queue.PendingCount(a) != 0 || result.Cancelled.Count != 2 || result.Reconcile.Count != 0) throw new InvalidOperationException("Q-01_AFFECTED_PENDING_SURVIVED");
        if (queue.PendingCount(b) != 1 || queue.IsContained(b)) throw new InvalidOperationException("Q-01_PEER_COLLATERAL");
        if (!queue.TryLeaseNext(out var peer) || peer is null || peer.Identity.Account != b) throw new InvalidOperationException("Q-01_PEER_DID_NOT_CONTINUE");
        if (queue.Snapshot(a).Any(x => x.State != TA.ExecutionQueueState.CancelledByContainment || x.ContainmentIncidentId != "account-a")) throw new InvalidOperationException("Q-01_AUDIT_TOMBSTONE_MISSING");
    }

    private static void LeasedWorkCannotEscapeContainment()
    {
        var queue = new TA.AccountScopedExecutionQueue(); var account = Account("ALPACA", "LEASE-RACE");
        queue.Enqueue(Work(account, "lease"), out _);
        if (!queue.TryLeaseNext(out var lease) || lease is null) throw new InvalidOperationException("Q-02_SETUP");
        queue.ContainAccount(account, Evidence("lease-incident", account));
        if (queue.TryBeginDispatch(lease, out _) || queue.PendingCount(account) != 0) throw new InvalidOperationException("Q-02_LEASE_ESCAPED");
    }

    private static void PermitIssuedBeforeContainmentCannotReachBroker()
    {
        var queue = new TA.AccountScopedExecutionQueue(); var account = Account("ALPACA", "PERMIT-RACE");
        var prepared = PrepareDispatch(queue, account, "permit-race");
        queue.ContainAccount(account, Evidence("permit-race-incident", account));
        var port = new CountingBrokerPort(); var coordinator = new TA.ExecutionCoordinator(port, queue);
        var result = coordinator.SubmitOrReconcileAsync(prepared.Intent, prepared.Permit, CancellationToken.None).AsTask().GetAwaiter().GetResult();
        if (result.State != T.OrderState.Rejected || port.Submits != 0) throw new InvalidOperationException("Q-03_PREISSUED_PERMIT_ESCAPED_CONTAINMENT");
    }

    private static void InFlightContainmentForcesReconciliation()
    {
        var queue = new TA.AccountScopedExecutionQueue(); var account = Account("ALPACA", "INFLIGHT");
        var prepared = PrepareDispatch(queue, account, "inflight");
        var port = new BlockingBrokerPort(); var coordinator = new TA.ExecutionCoordinator(port, queue);
        var task = coordinator.SubmitOrReconcileAsync(prepared.Intent, prepared.Permit, CancellationToken.None).AsTask();
        if (!port.Entered.Wait(TimeSpan.FromSeconds(5))) throw new InvalidOperationException("Q-04_BROKER_CALL_DID_NOT_START");
        var containment = queue.ContainAccount(account, Evidence("inflight-incident", account));
        port.Release.Set();
        var result = task.GetAwaiter().GetResult();
        if (containment.Reconcile.Count != 1 || result.State != T.OrderState.ReconciliationRequired || queue.Snapshot(account).Single().State != TA.ExecutionQueueState.ReconciliationRequired)
            throw new InvalidOperationException("Q-04_INFLIGHT_CONTAINMENT_NOT_RECONCILIATION_OWNED");
    }

    private static void ExpiredLeaseCannotStrandOrDoubleDispatchWork()
    {
        var queue = new TA.AccountScopedExecutionQueue(); var account = Account("ALPACA", "LEASE-EXPIRY"); var now = DateTimeOffset.UtcNow;
        var work = Work(account, "lease-expiry");
        queue.Enqueue(work, out _);
        if (!queue.TryLeaseNext(now, TimeSpan.FromSeconds(1), out var firstLease) || firstLease is null ||
            !queue.TryBeginDispatch(firstLease, now.AddMilliseconds(100), out var stalePermit) || stalePermit is null)
            throw new InvalidOperationException("Q-05_LEASE_EXPIRY_SETUP");
        if (queue.ReclaimExpiredLeases(now.AddSeconds(2)) != 1) throw new InvalidOperationException("Q-05_EXPIRED_LEASE_NOT_RECLAIMED");
        if (!queue.TryLeaseNext(now.AddSeconds(2), TimeSpan.FromSeconds(30), out var secondLease) || secondLease is null || secondLease.WorkId != firstLease.WorkId)
            throw new InvalidOperationException("Q-05_RECLAIMED_WORK_NOT_REQUEUED");
        var port = new CountingBrokerPort(); var coordinator = new TA.ExecutionCoordinator(port, queue);
        var staleResult = coordinator.SubmitOrReconcileAsync(work.Intent, stalePermit, CancellationToken.None).AsTask().GetAwaiter().GetResult();
        if (staleResult.State != T.OrderState.Rejected || port.Submits != 0) throw new InvalidOperationException("Q-05_EXPIRED_PERMIT_REACHED_BROKER");
        if (!queue.TryBeginDispatch(secondLease, now.AddSeconds(3), out var currentPermit) || currentPermit is null) throw new InvalidOperationException("Q-05_CURRENT_LEASE_COULD_NOT_DISPATCH");
    }

    private static void ContainedAccountRejectsNewQueueWork()
    {
        var queue = new TA.AccountScopedExecutionQueue(); var account = Account("ALPACA", "BLOCKED");
        queue.ContainAccount(account, Evidence("blocked-incident", account));
        if (queue.Enqueue(Work(account, "new"), out var s) || s.State != TA.ExecutionQueueState.CancelledByContainment) throw new InvalidOperationException("Q-06_NEW_WORK_ENTERED_CONTAINED_SCOPE");
    }

    private static void CancelledWorkCannotResurrectAfterRecovery()
    {
        var queue = new TA.AccountScopedExecutionQueue(); var account = Account("ALPACA", "RECOVERY"); var original = Work(account, "original");
        queue.Enqueue(original, out _); queue.ContainAccount(account, Evidence("recovery-incident", account));
        if (queue.TryReleaseAccount(account, Reconciliation(account, false, "incomplete"))) throw new InvalidOperationException("Q-07_INCOMPLETE_RELEASE");
        if (!queue.TryReleaseAccount(account, Reconciliation(account, true, "complete"))) throw new InvalidOperationException("Q-07_COMPLETE_NOT_RELEASED");
        if (queue.Enqueue(original, out _)) throw new InvalidOperationException("Q-07_WORK_RESURRECTED");
        if (queue.Enqueue(original with { WorkId = "renamed-work" }, out _)) throw new InvalidOperationException("Q-07_IDENTITY_RESURRECTED");
        if (!queue.Enqueue(Work(account, "fresh"), out _)) throw new InvalidOperationException("Q-07_FRESH_WORK_BLOCKED");
    }

    private static void BrokerWideContainmentIsStickyForNewAccounts()
    {
        var queue = new TA.AccountScopedExecutionQueue(); var a = Account("ALPACA", "BROKER-A"); var declared = Account("ALPACA", "DECLARED");
        queue.Enqueue(Work(a, "broker-a"), out _); queue.ContainBroker("alpaca", "paper", Evidence("broker-down", a, declared));
        if (queue.Enqueue(Work(Account("ALPACA", "BROKER-NEW"), "broker-new"), out _)) throw new InvalidOperationException("Q-08_NEW_ACCOUNT_BYPASSED_BROKER_HOLD");
        if (!queue.Enqueue(Work(Account("ALPACA", "LIVE-ACCOUNT", "LIVE"), "live"), out _)) throw new InvalidOperationException("Q-08_ENVIRONMENT_COLLATERAL");
        if (!queue.Enqueue(Work(Account("IBKR", "OTHER"), "other"), out _)) throw new InvalidOperationException("Q-08_OTHER_BROKER_COLLATERAL");
    }

    private static void BrokerWideReleaseRequiresDeclaredAndObservedAccounts()
    {
        var queue = new TA.AccountScopedExecutionQueue(); var a = Account("ALPACA", "REL-A"); var b = Account("ALPACA", "REL-B"); var declared = Account("ALPACA", "REL-DECLARED");
        queue.Enqueue(Work(a, "rel-a"), out _); queue.Enqueue(Work(b, "rel-b"), out _); queue.ContainBroker("ALPACA", "PAPER", Evidence("broker-release", a, b, declared));
        if (queue.TryReleaseAccount(a, Reconciliation(a, true, "account"))) throw new InvalidOperationException("Q-09_ACCOUNT_BYPASSED_BROKER_HOLD");
        if (queue.TryReleaseBroker("ALPACA", "PAPER", new[] { Reconciliation(a, true, "a"), Reconciliation(b, true, "b") })) throw new InvalidOperationException("Q-09_DECLARED_ACCOUNT_OMITTED");
        if (!queue.TryReleaseBroker("ALPACA", "PAPER", new[] { Reconciliation(a, true, "a"), Reconciliation(b, true, "b"), Reconciliation(declared, true, "d") })) throw new InvalidOperationException("Q-09_FULL_RELEASE_FAILED");
    }

    private static void SameWorkIdAcrossAccountsRemainsIndependent()
    {
        var queue = new TA.AccountScopedExecutionQueue();
        var a = Work(Account("ALPACA", "SAME-A"), "a") with { WorkId = "same-work" }; var b = Work(Account("ALPACA", "SAME-B"), "b") with { WorkId = "same-work" };
        if (!queue.Enqueue(a, out _) || !queue.Enqueue(b, out _)) throw new InvalidOperationException("Q-10_WORK_ID_GLOBAL_COLLISION");
    }

    private static void BrokerIdentityCaseCannotBypassContainment()
    {
        var lower = Account("alpaca", "CASE"); var upper = Account("ALPACA", "CASE");
        if (lower != upper || lower.NamespaceKey != upper.NamespaceKey) throw new InvalidOperationException("Q-11_CASE_PARALLEL_IDENTITY");
        var queue = new TA.AccountScopedExecutionQueue(); queue.ContainAccount(lower, Evidence("case-incident", lower));
        if (queue.Enqueue(Work(upper, "case"), out _)) throw new InvalidOperationException("Q-11_CASE_BYPASS");
    }

    private static void DispatchPermitIsSingleUse()
    {
        var queue = new TA.AccountScopedExecutionQueue(); var account = Account("ALPACA", "PERMIT"); var prepared = PrepareDispatch(queue, account, "permit");
        var port = new CountingBrokerPort(); var coordinator = new TA.ExecutionCoordinator(port, queue);
        var first = coordinator.SubmitOrReconcileAsync(prepared.Intent, prepared.Permit, CancellationToken.None).AsTask().GetAwaiter().GetResult();
        var second = coordinator.SubmitOrReconcileAsync(prepared.Intent, prepared.Permit, CancellationToken.None).AsTask().GetAwaiter().GetResult();
        if (first.State != T.OrderState.SubmissionAttempted || second.State != T.OrderState.Rejected || port.Submits != 1) throw new InvalidOperationException("Q-12_PERMIT_REUSE");
    }

    private static void SubmissionExceptionFailsClosedToReconciliation()
    {
        var queue = new TA.AccountScopedExecutionQueue(); var account = Account("ALPACA", "EXCEPTION"); var prepared = PrepareDispatch(queue, account, "exception");
        var result = new TA.ExecutionCoordinator(new ThrowingBrokerPort(), queue).SubmitOrReconcileAsync(prepared.Intent, prepared.Permit, CancellationToken.None).AsTask().GetAwaiter().GetResult();
        if (result.State != T.OrderState.ReconciliationRequired || queue.Snapshot(account).Single().State != TA.ExecutionQueueState.ReconciliationRequired) throw new InvalidOperationException("Q-13_SUBMISSION_EXCEPTION_NOT_FAIL_CLOSED");
    }

    private static void ConcurrentEnqueueContainmentLeavesNoPendingWork()
    {
        var queue = new TA.AccountScopedExecutionQueue(); var account = Account("ALPACA", "CONCURRENT"); var start = new ManualResetEventSlim(false);
        var producers = Enumerable.Range(0, 64).Select(i => Task.Run(() => { start.Wait(); queue.Enqueue(Work(account, $"race-{i}"), out _); })).ToArray();
        var containment = Task.Run(() => { start.Wait(); queue.ContainAccount(account, Evidence("race-incident", account)); });
        start.Set(); Task.WaitAll(producers.Append(containment).ToArray());
        if (!queue.IsContained(account) || queue.PendingCount(account) != 0 || queue.Snapshot(account).Any(x => x.State is TA.ExecutionQueueState.Queued or TA.ExecutionQueueState.Leased)) throw new InvalidOperationException("Q-14_CONCURRENT_PENDING_SURVIVED");
    }

    private sealed class CountingBrokerPort : TA.IBrokerExecutionPort
    {
        private int _submits; public int Submits => Volatile.Read(ref _submits);
        public ValueTask<TA.BrokerSubmissionResult> SubmitAsync(TA.OrderIntent intent, CancellationToken cancellationToken) { Interlocked.Increment(ref _submits); return ValueTask.FromResult(new TA.BrokerSubmissionResult(intent.ExecutionIdentity, true, true, "SUBMITTED")); }
        public ValueTask<TA.BrokerOrderSnapshot> ReconcileAsync(TA.BrokerExecutionIdentity identity, CancellationToken cancellationToken) => ValueTask.FromResult(new TA.BrokerOrderSnapshot(identity, T.OrderState.ReconciliationRequired, new T.Quantity(0), "RECONCILE"));
    }

    private sealed class BlockingBrokerPort : TA.IBrokerExecutionPort
    {
        public ManualResetEventSlim Entered { get; } = new(false);
        public ManualResetEventSlim Release { get; } = new(false);
        public async ValueTask<TA.BrokerSubmissionResult> SubmitAsync(TA.OrderIntent intent, CancellationToken cancellationToken)
        {
            Entered.Set();
            await Task.Run(() => Release.Wait(cancellationToken), cancellationToken);
            return new TA.BrokerSubmissionResult(intent.ExecutionIdentity, true, true, "SUBMITTED");
        }
        public ValueTask<TA.BrokerOrderSnapshot> ReconcileAsync(TA.BrokerExecutionIdentity identity, CancellationToken cancellationToken) => ValueTask.FromResult(new TA.BrokerOrderSnapshot(identity, T.OrderState.ReconciliationRequired, new T.Quantity(0), "RECONCILE"));
    }

    private sealed class ThrowingBrokerPort : TA.IBrokerExecutionPort
    {
        public ValueTask<TA.BrokerSubmissionResult> SubmitAsync(TA.OrderIntent intent, CancellationToken cancellationToken) => ValueTask.FromException<TA.BrokerSubmissionResult>(new TimeoutException("ambiguous submit"));
        public ValueTask<TA.BrokerOrderSnapshot> ReconcileAsync(TA.BrokerExecutionIdentity identity, CancellationToken cancellationToken) => ValueTask.FromException<TA.BrokerOrderSnapshot>(new IOException("broker unavailable"));
    }
}
