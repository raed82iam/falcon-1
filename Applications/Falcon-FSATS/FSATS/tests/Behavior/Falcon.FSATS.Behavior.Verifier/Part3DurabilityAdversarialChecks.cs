using T = Falcon.FSATS.Trading.Domain;
using TA = Falcon.FSATS.Trading.Application;
using P = Falcon.FSATS.FSAPMA.Domain;
using PA = Falcon.FSATS.FSAPMA.Application;
using G = Falcon.FSATS.TradingGuardian.Contracts;
using GA = Falcon.FSATS.TradingGuardian.Application;
using R = Falcon.FSATS.ResourceManagement.Domain;
using RA = Falcon.FSATS.ResourceManagement.Application;
using S = Falcon.FSATS.FSTSimA.Domain;
using SA = Falcon.FSATS.FSTSimA.Application;

internal static class Part3DurabilityAdversarialChecks
{
    internal static void Run()
    {
        TradingDispatchStartedCannotBecomeCompletedAfterRestart();
        TradingContainmentAndTombstoneSurviveRestart();
        TradingLeasedWorkLosesLeaseAuthority();
        TradingCorruptSnapshotFailsClosed();
        TradingAccountIsolationSurvivesRestart();
        FsAPMACurrentStreamCannotRemainCurrentAcrossRestart();
        FsAPMAUnknownDeliveryRemainsUnknownWithoutRedispatch();
        GuardianAmbiguousProtectionRemainsReconciliationOwned();
        AppRscPersistedEpochCannotMintRestartAuthority();
        SimulationInterruptedRunCannotQualify();
        RetentionCannotCompactSafetyCriticalState();
    }

    private static void TradingDispatchStartedCannotBecomeCompletedAfterRestart()
    {
        var now = DateTimeOffset.UtcNow;
        var work = Work("dispatch-started", "ACC-A");
        var snapshot = TA.TradingDurableSnapshot.Create(1, now,
            new[] { new TA.DurableExecutionRecord(work, TA.ExecutionQueueState.DispatchStarted, "started", "ev-dispatch", null, 4, now) });
        var plan = TA.TradingRestartReconstructor.Reconstruct(snapshot, now.AddSeconds(1));
        var record = plan.Execution.Single();
        if (!plan.Accepted || record.Disposition != TA.RestartExecutionDisposition.ReconciliationRequired || plan.AllowsNewRisk(work.Intent.ExecutionIdentity.Account))
            throw new InvalidOperationException("P3_TRADING_INFLIGHT_DISPATCH_RESTART_NOT_RECONCILIATION_OWNED");
    }

    private static void TradingContainmentAndTombstoneSurviveRestart()
    {
        var now = DateTimeOffset.UtcNow;
        var account = Account("ACC-A");
        var work = Work("cancelled", account.BrokerAccountId);
        var containment = new TA.DurableAccountContainment(account, "incident-a", "risk", "ev-contain", now);
        var snapshot = TA.TradingDurableSnapshot.Create(2, now,
            new[] { new TA.DurableExecutionRecord(work, TA.ExecutionQueueState.CancelledByContainment, "cancelled", "ev-cancel", "incident-a", 5, now) },
            new[] { containment });
        var plan = TA.TradingRestartReconstructor.Reconstruct(snapshot, now.AddSeconds(1));
        if (!plan.Accepted || !plan.ContainedAccounts.Contains(account) || !plan.ReservedExecutionIdentityKeys.Contains(work.Intent.ExecutionIdentity.NamespaceKey) || plan.Execution.Single().Disposition != TA.RestartExecutionDisposition.CancelledTombstone)
            throw new InvalidOperationException("P3_TRADING_CONTAINMENT_OR_TOMBSTONE_RESURRECTED");
    }

    private static void TradingLeasedWorkLosesLeaseAuthority()
    {
        var now = DateTimeOffset.UtcNow;
        var work = Work("leased", "ACC-A");
        var snapshot = TA.TradingDurableSnapshot.Create(3, now,
            new[] { new TA.DurableExecutionRecord(work, TA.ExecutionQueueState.Leased, "leased", "ev-lease", null, 2, now) });
        var plan = TA.TradingRestartReconstructor.Reconstruct(snapshot, now.AddSeconds(1));
        if (!plan.Accepted || plan.Execution.Single().Disposition != TA.RestartExecutionDisposition.QueueEligible || plan.Execution.Single().ReasonCode != "PRE_RESTART_LEASE_INVALIDATED_REQUEUED")
            throw new InvalidOperationException("P3_TRADING_PRE_RESTART_LEASE_SURVIVED_RESTART");
    }

    private static void TradingCorruptSnapshotFailsClosed()
    {
        var now = DateTimeOffset.UtcNow;
        var snapshot = TA.TradingDurableSnapshot.Create(4, now, new[] { new TA.DurableExecutionRecord(Work("queued", "ACC-A"), TA.ExecutionQueueState.Queued, "queued", "ev-q", null, 1, now) });
        var corrupt = snapshot with { PayloadSha256 = new string('F', 64) == snapshot.PayloadSha256 ? new string('E', 64) : new string('F', 64) };
        var plan = TA.TradingRestartReconstructor.Reconstruct(corrupt, now.AddSeconds(1));
        if (plan.Accepted || plan.AllowsNewRisk(Account("ACC-A"))) throw new InvalidOperationException("P3_TRADING_CORRUPT_STATE_DID_NOT_FAIL_CLOSED");
    }

    private static void TradingAccountIsolationSurvivesRestart()
    {
        var now = DateTimeOffset.UtcNow;
        var a = Work("unknown-a", "ACC-A");
        var b = Work("queued-b", "ACC-B");
        var snapshot = TA.TradingDurableSnapshot.Create(5, now, new[]
        {
            new TA.DurableExecutionRecord(a, TA.ExecutionQueueState.DispatchStarted, "started", "ev-a", null, 1, now),
            new TA.DurableExecutionRecord(b, TA.ExecutionQueueState.Queued, "queued", "ev-b", null, 1, now)
        });
        var plan = TA.TradingRestartReconstructor.Reconstruct(snapshot, now.AddSeconds(1));
        if (!plan.Accepted || plan.AllowsNewRisk(a.Intent.ExecutionIdentity.Account) || !plan.AllowsNewRisk(b.Intent.ExecutionIdentity.Account))
            throw new InvalidOperationException("P3_TRADING_RESTART_ACCOUNT_ISOLATION_COLLAPSED");
    }

    private static void FsAPMACurrentStreamCannotRemainCurrentAcrossRestart()
    {
        var now = DateTimeOffset.UtcNow;
        var route = new P.ProviderRouteIdentity(new P.ProviderId("ALPACA"), new P.ProviderAccountId("DATA-A"), "PAPER", "MARKET_DATA", new P.CredentialReference("cred-ref"));
        var identity = new P.ProviderStreamSessionIdentity(route, "endpoint-1", "session-1", "AAPL");
        var snapshot = PA.FSAPMADurableSnapshot.Create(1, now,
            new[] { new PA.DurableStreamContinuityRecord(identity, P.StreamContinuityState.Current, 123, "ev-stream", now) },
            Array.Empty<PA.DurableOperationalDeliveryRecord>());
        var plan = PA.FSAPMARestartReconstructor.Reconstruct(snapshot, now.AddSeconds(1));
        if (!plan.Accepted || plan.Streams.Single().State != P.StreamContinuityState.ReconciliationRequired || plan.IsOperationalDataCurrent(identity))
            throw new InvalidOperationException("P3_FSAPMA_RESTART_FABRICATED_STREAM_CONTINUITY");
    }

    private static void FsAPMAUnknownDeliveryRemainsUnknownWithoutRedispatch()
    {
        var now = DateTimeOffset.UtcNow;
        var result = new PA.OperationalDataDeliveryResult(PA.OperationalDataDeliveryState.DeliveryOutcomeUnknown, "obs", "Trading", "AMBIGUOUS", "corr", now, "route");
        var record = new PA.DurableOperationalDeliveryRecord("scope", new string('A', 64), result, now);
        var snapshot = PA.FSAPMADurableSnapshot.Create(2, now, Array.Empty<PA.DurableStreamContinuityRecord>(), new[] { record });
        var plan = PA.FSAPMARestartReconstructor.Reconstruct(snapshot, now.AddSeconds(1));
        var replay = PA.FSAPMARestartReconstructor.ReplayView(plan.DeliveryTombstones["scope"]);
        if (!plan.Accepted || replay.State != PA.OperationalDataDeliveryState.DeliveryOutcomeUnknown)
            throw new InvalidOperationException("P3_FSAPMA_UNKNOWN_DELIVERY_BECAME_RETRYABLE");
    }

    private static void GuardianAmbiguousProtectionRemainsReconciliationOwned()
    {
        var now = DateTimeOffset.UtcNow;
        var target = new G.ProtectionTarget(G.ProtectionTargetKind.BrokerAccount, "ALPACA", "ACC-A", "PAPER");
        var outcome = new G.ProtectionCommandOutcome(new G.CommandId("cmd"), G.ProtectionOutcomeState.Accepted, "Trading", target, "accepted", now, "corr", new string('B', 64), "ev");
        var record = new GA.DurableProtectionOutcomeRecord("scope", new string('B', 64), outcome, now);
        var snapshot = GA.GuardianDurableSnapshot.Create(1, now, new[] { record });
        var plan = GA.GuardianRestartReconstructor.Reconstruct(snapshot, now.AddSeconds(1));
        if (!plan.Accepted || plan.ReconciliationRequired.Single().State != G.ProtectionOutcomeState.ReconciliationRequired || !plan.RequiresCurrentProtectionTruthVerification)
            throw new InvalidOperationException("P3_GUARDIAN_AMBIGUOUS_PROTECTION_RESTARTED_AS_SUCCESS");
    }

    private static void AppRscPersistedEpochCannotMintRestartAuthority()
    {
        var now = DateTimeOffset.UtcNow;
        var decision = new R.RedistributionDecision("d1", "FSTSimA", "Trading", "CPU", 1m, new R.CoordinationEpoch(7), "env-old", "pressure");
        var snapshot = RA.ResourceDurableSnapshot.Create(1, now, new R.CoordinationEpoch(7), "env-old", new[] { new RA.DurableRedistributionRecord(decision, "ev-rsc", now) });
        var plan = RA.ResourceRestartReconstructor.Reconstruct(snapshot, now.AddSeconds(1));
        var staleEnvelope = new R.FoundationEnvelope("env-old", "CPU", 10m, now.AddMinutes(-1), now.AddMinutes(5), false);
        var freshEnvelope = new R.FoundationEnvelope("env-new", "CPU", 10m, now.AddMinutes(-1), now.AddMinutes(5), false);
        if (!plan.Accepted || plan.AllowsRedistribution || RA.ResourceRestartReconstructor.CanResumeWithFreshFoundationTruth(plan, new R.CoordinationEpoch(7), staleEnvelope, now) || !RA.ResourceRestartReconstructor.CanResumeWithFreshFoundationTruth(plan, new R.CoordinationEpoch(8), freshEnvelope, now))
            throw new InvalidOperationException("P3_APP_RSC_RESTART_REUSED_STALE_COORDINATION_AUTHORITY");
    }

    private static void SimulationInterruptedRunCannotQualify()
    {
        var now = DateTimeOffset.UtcNow;
        var run = new SA.DurableSimulationRunRecord(new S.ScenarioId("scenario"), "run-1", 77, new S.SimulationInstant(10), SA.SimulationRunDurableState.Started, "ev-sim", null, now);
        var snapshot = SA.SimulationDurableSnapshot.Create(1, now, new[] { run });
        var plan = SA.SimulationRestartReconstructor.Reconstruct(snapshot, now.AddSeconds(1));
        if (!plan.Accepted || plan.Runs.Single().QualificationEligible || plan.Runs.Single().DurableRecord.State != SA.SimulationRunDurableState.Interrupted)
            throw new InvalidOperationException("P3_SIMULATION_PARTIAL_RUN_QUALIFIED_AFTER_RESTART");
    }

    private static void RetentionCannotCompactSafetyCriticalState()
    {
        var now = DateTimeOffset.UtcNow;
        var pending = new TA.RestartExecutionRecord(new TA.DurableExecutionRecord(Work("pending", "ACC-A"), TA.ExecutionQueueState.Queued, "q", "ev", null, 1, now.AddDays(-10)), TA.RestartExecutionDisposition.QueueEligible, "q");
        var terminal = new TA.RestartExecutionRecord(new TA.DurableExecutionRecord(Work("done", "ACC-A"), TA.ExecutionQueueState.Completed, "done", "ev", null, 1, now.AddDays(-10)), TA.RestartExecutionDisposition.TerminalIdentityFence, "done");
        var compactable = TA.TradingDurableRetention.SelectCompactable(new[] { pending, terminal }, new TA.DurableRetentionPolicy(TimeSpan.FromDays(1), 10), now);
        if (compactable.Count != 1 || compactable[0].DurableRecord.Work.WorkId != "done")
            throw new InvalidOperationException("P3_RETENTION_PRESSURE_SELECTED_SAFETY_CRITICAL_STATE");
    }

    private static T.BrokerAccountContext Account(string accountId) => new("ALPACA", accountId, "PAPER");

    private static TA.QueuedExecutionWork Work(string id, string accountId)
    {
        var account = Account(accountId);
        var execution = new TA.BrokerExecutionIdentity(account, "route-1", "submission-" + id, new T.OrderId("order-" + id));
        var envelope = new T.PositionSafetyEnvelope(new T.PositionId("position-" + id), new T.InstrumentId("AAPL"), new T.Quantity(1), new T.Money(10m, new T.Currency("USD")), "guardian", "protected", "exit", "current", new T.TrustEpoch(1));
        var intent = new TA.OrderIntent(execution, new T.InstrumentId("AAPL"), new T.Quantity(1), new T.TrustEpoch(1), envelope);
        return new TA.QueuedExecutionWork(id, intent, DateTimeOffset.UtcNow.AddSeconds(-1), "ev-" + id);
    }
}
