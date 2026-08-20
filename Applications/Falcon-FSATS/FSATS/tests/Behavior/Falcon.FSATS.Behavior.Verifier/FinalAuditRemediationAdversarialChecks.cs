using System.Runtime.CompilerServices;
using T = Falcon.FSATS.Trading.Domain;
using TA = Falcon.FSATS.Trading.Application;
using P = Falcon.FSATS.FSAPMA.Domain;
using G = Falcon.FSATS.TradingGuardian.Domain;
using S = Falcon.FSATS.FSTSimA.Domain;
using R = Falcon.FSATS.ResourceManagement.Domain;

internal static class FinalAuditRemediationAdversarialChecks
{
    [ModuleInitializer]
    internal static void Initialize() => Run();

    internal static void Run()
    {
        BrokerRecoveryRejectsStaleFutureAndMixedAgeEvidence();
        SafetyEnvelopeRequiresExactDecisionAndExecutionBinding();
        GuardianSafeModeIsMonotonicAndRecoveryIsGoverned();
        ResourceReclaimabilityRequiresActualIdleHeadroom();
        FailureLocalityRequiresFreshClockValidEvidence();
        ProviderFutureObservationFailsClosed();
        SimulatorLowLevelImpossibleInputsFailClosed();
    }

    private static T.BrokerAccountContext Account(string id = "PA-A") => new("ALPACA", id, "PAPER");

    private static TA.BrokerAccountObservation BrokerObservation(T.BrokerAccountContext account, DateTimeOffset observedAt)
        => new(account, new T.PositionId("pos-a"), new T.Quantity(1m), false, true,
            TA.BrokerAccountEvidenceSource.BrokerApiConfirmed, "broker-observation", observedAt);

    private static TA.BrokerAccountReconciliationEvidence BrokerReconciliation(T.BrokerAccountContext account, DateTimeOffset aggregateAt, DateTimeOffset dimensionsAt)
    {
        var dimensions = Enum.GetValues<TA.BrokerReconciliationDimension>()
            .Select(x => new TA.BrokerReconciliationDimensionEvidence(x, true, $"dimension:{x}", dimensionsAt))
            .ToArray();
        return new(account, true, true, true, true, true, true, true, "reconciliation", aggregateAt, dimensions);
    }

    private static void BrokerRecoveryRejectsStaleFutureAndMixedAgeEvidence()
    {
        var now = DateTimeOffset.UtcNow;
        var account = Account();
        var maxAge = TimeSpan.FromMinutes(2);
        var maxSkew = TimeSpan.FromSeconds(30);

        var stale = TA.BrokerOutageRecoveryPolicy.Assess(TA.BrokerConnectivityState.Available, TA.BrokerSubmissionTruth.Reconciled,
            BrokerObservation(account, now.AddMinutes(-10)), BrokerReconciliation(account, now.AddSeconds(-10), now.AddSeconds(-10)), now, maxAge, maxSkew);
        Require(!stale.MayResumeRiskIncreasingAction && stale.RecoveryState != T.OperationalRecoveryState.Recovered, "AUDIT_H01_STALE_OBSERVATION_RECOVERED");

        var future = TA.BrokerOutageRecoveryPolicy.Assess(TA.BrokerConnectivityState.Available, TA.BrokerSubmissionTruth.Reconciled,
            BrokerObservation(account, now.AddSeconds(1)), BrokerReconciliation(account, now, now), now, maxAge, maxSkew);
        Require(!future.MayResumeRiskIncreasingAction && future.RecoveryState != T.OperationalRecoveryState.Recovered, "AUDIT_H01_FUTURE_OBSERVATION_RECOVERED");

        var mixed = TA.BrokerOutageRecoveryPolicy.Assess(TA.BrokerConnectivityState.Available, TA.BrokerSubmissionTruth.Reconciled,
            BrokerObservation(account, now.AddSeconds(-5)), BrokerReconciliation(account, now.AddSeconds(-5), now.AddMinutes(-10)), now, maxAge, maxSkew);
        Require(!mixed.MayResumeRiskIncreasingAction && mixed.RecoveryState != T.OperationalRecoveryState.Recovered, "AUDIT_H01_MIXED_AGE_RECONCILIATION_RECOVERED");

        var fresh = TA.BrokerOutageRecoveryPolicy.Assess(TA.BrokerConnectivityState.Available, TA.BrokerSubmissionTruth.Reconciled,
            BrokerObservation(account, now.AddSeconds(-5)), BrokerReconciliation(account, now.AddSeconds(-5), now.AddSeconds(-4)), now, maxAge, maxSkew);
        Require(fresh.MayResumeRiskIncreasingAction && fresh.RecoveryState == T.OperationalRecoveryState.Recovered, "AUDIT_H01_FRESH_COHERENT_RECONCILIATION_REJECTED");
    }

    private static T.PositionSafetyEnvelope BoundSafety(T.BrokerAccountContext account, string instrument = "AAPL", decimal quantity = 1m, long epoch = 7, string position = "pos-a", string evidence = "protection-evidence")
        => new(new T.PositionId(position), new T.InstrumentId(instrument), new T.Quantity(quantity),
            new T.Money(20m, new T.Currency("USD")), "guardian", "protected", "emergency-exit", "reconciled", new T.TrustEpoch(epoch))
        {
            AccountContext = account,
            ProtectionEvidenceReference = evidence
        };

    private static void SafetyEnvelopeRequiresExactDecisionAndExecutionBinding()
    {
        var account = Account();
        var other = Account("PA-B");
        var envelope = BoundSafety(account);
        var requiredLoss = new T.Money(10m, new T.Currency("USD"));
        Require(T.PositionSafetyEnvelopeBindingGuard.IsBoundForRiskDecision(envelope, account, new T.InstrumentId("AAPL"), new T.Quantity(1m), requiredLoss, new T.TrustEpoch(7)), "AUDIT_H02_VALID_DECISION_BINDING_REJECTED");
        Require(!T.PositionSafetyEnvelopeBindingGuard.IsBoundForRiskDecision(envelope, other, new T.InstrumentId("AAPL"), new T.Quantity(1m), requiredLoss, new T.TrustEpoch(7)), "AUDIT_H02_WRONG_ACCOUNT_ACCEPTED");
        Require(!T.PositionSafetyEnvelopeBindingGuard.IsBoundForExecution(envelope, account, new T.InstrumentId("MSFT"), new T.Quantity(1m), new T.TrustEpoch(7)), "AUDIT_H02_WRONG_INSTRUMENT_ACCEPTED");
        Require(!T.PositionSafetyEnvelopeBindingGuard.IsBoundForExecution(envelope, account, new T.InstrumentId("AAPL"), new T.Quantity(2m), new T.TrustEpoch(7)), "AUDIT_H02_UNCOVERED_QUANTITY_ACCEPTED");
        Require(!T.PositionSafetyEnvelopeBindingGuard.IsBoundForExecution(envelope, account, new T.InstrumentId("AAPL"), new T.Quantity(1m), new T.TrustEpoch(8)), "AUDIT_H02_STALE_TRUST_EPOCH_ACCEPTED");

        var identityA = new TA.BrokerExecutionIdentity(account, "route-a", "submission-a", new T.OrderId("order-a"));
        var envelopeA = BoundSafety(account, position: "pos-a", evidence: "evidence-a");
        var envelopeB = BoundSafety(account, position: "pos-b", evidence: "evidence-b");
        var pipeline = new TA.TradingDecisionPipeline(new T.CapitalReservationLedger());
        var riskRequest = new T.RiskRequest(
            new T.InstrumentId("AAPL"),
            new T.Quantity(1m),
            new T.Money(10m, new T.Currency("USD")),
            new T.Money(20m, new T.Currency("USD")),
            true,
            true)
        {
            AccountContext = account
        };

        var preparationA = pipeline.Prepare(riskRequest, new T.ReservationId("reservation-a"), new T.Money(100m, new T.Currency("USD")), envelopeA,
            new T.TrustEpoch(7), new T.TrustEpoch(7), identityA);
        var preparationB = pipeline.Prepare(riskRequest, new T.ReservationId("reservation-b"), new T.Money(100m, new T.Currency("USD")), envelopeB,
            new T.TrustEpoch(7), new T.TrustEpoch(7), identityA);
        Require(preparationA.Allowed && preparationA.ReservationId is not null && !string.IsNullOrWhiteSpace(preparationA.DecisionBindingReference), "AUDIT_H02_PREPARATION_A_BINDING_NOT_ISSUED");
        Require(preparationB.Allowed && preparationB.ReservationId is not null && !string.IsNullOrWhiteSpace(preparationB.DecisionBindingReference), "AUDIT_H02_SECOND_SAME_IDENTITY_BINDING_NOT_ISSUED");
        Require(!StringComparer.Ordinal.Equals(preparationA.DecisionBindingReference, preparationB.DecisionBindingReference), "AUDIT_H02_DISTINCT_DECISIONS_COLLAPSED_TO_ONE_BINDING");

        var intentA = new TA.OrderIntent(identityA, new T.InstrumentId("AAPL"), new T.Quantity(1m), new T.TrustEpoch(7), envelopeA)
        {
            RiskReservationId = preparationA.ReservationId,
            DecisionBindingReference = preparationA.DecisionBindingReference
        };
        var queue = new TA.AccountScopedExecutionQueue();
        Require(queue.Enqueue(new TA.QueuedExecutionWork("work-a", intentA, DateTimeOffset.UtcNow, "queue-evidence-a"), out _), "AUDIT_H02_VALID_BOUND_INTENT_NOT_QUEUED");

        if (!queue.TryLeaseNext(out var lease) || lease is null)
            throw new InvalidOperationException("AUDIT_H02_VALID_BOUND_INTENT_NO_LEASE");
        if (!queue.TryBeginDispatch(lease, out var permit) || permit is null)
            throw new InvalidOperationException("AUDIT_H02_VALID_BOUND_INTENT_NO_DISPATCH_PERMIT");

        var broker = new CountingBrokerPort();
        var coordinator = new TA.ExecutionCoordinator(broker, queue);
        var swapped = intentA with
        {
            SafetyEnvelope = envelopeB,
            RiskReservationId = preparationB.ReservationId,
            DecisionBindingReference = preparationB.DecisionBindingReference
        };
        var swappedResult = coordinator.SubmitOrReconcileAsync(swapped, permit, CancellationToken.None).AsTask().GetAwaiter().GetResult();
        Require(swappedResult.State == T.OrderState.Rejected && swappedResult.ReasonCode == "EXECUTION_DISPATCH_PERMIT_DECISION_BINDING_MISMATCH" && broker.Submits == 0,
            "AUDIT_H02_SAME_IDENTITY_SECOND_DECISION_REACHED_BROKER");

        var mutated = intentA with
        {
            SafetyEnvelope = envelopeA with { ProtectionEvidenceReference = "mutated-after-preparation" }
        };
        var mutatedResult = coordinator.SubmitOrReconcileAsync(mutated, permit, CancellationToken.None).AsTask().GetAwaiter().GetResult();
        Require(mutatedResult.State == T.OrderState.Rejected && mutatedResult.ReasonCode == "TRADING_DECISION_BINDING_MISSING_OR_MISMATCHED" && broker.Submits == 0,
            "AUDIT_H02_POST_PREPARATION_MUTATION_REACHED_BROKER");

        var validResult = coordinator.SubmitOrReconcileAsync(intentA, permit, CancellationToken.None).AsTask().GetAwaiter().GetResult();
        Require(validResult.State == T.OrderState.SubmissionAttempted && broker.Submits == 1, "AUDIT_H02_VALID_PREPARED_INTENT_DID_NOT_DISPATCH");
    }

    private static void GuardianSafeModeIsMonotonicAndRecoveryIsGoverned()
    {
        var now = DateTimeOffset.UtcNow;
        var classifier = new G.IncidentClassifier();
        var crisis = new G.CrisisStateMachine();
        crisis.Apply(classifier.Classify(new[] { new G.ProtectionSignal("guardian", "protection", 95, true, now.AddSeconds(-1)) }, now, TimeSpan.FromMinutes(1)));
        Require(crisis.Mode == G.GuardianMode.SafeMode, "AUDIT_H03_SAFEMODE_NOT_ENTERED");
        crisis.Apply(classifier.Classify(new[] { new G.ProtectionSignal("guardian", "minor", 10, true, now) }, now, TimeSpan.FromMinutes(1)));
        Require(crisis.Mode == G.GuardianMode.SafeMode, "AUDIT_H03_WEAK_SIGNAL_RELAXED_SAFEMODE");

        var stale = classifier.Classify(new[] { new G.ProtectionSignal("guardian", "minor", 10, true, now.AddMinutes(-10)) }, now, TimeSpan.FromMinutes(1));
        Require(stale.Classification == G.IncidentClass.IntegrityIncident, "AUDIT_H03_STALE_SIGNAL_NOT_FAIL_CLOSED");
        var future = classifier.Classify(new[] { new G.ProtectionSignal("guardian", "minor", 10, true, now.AddSeconds(1)) }, now, TimeSpan.FromMinutes(1));
        Require(future.Classification == G.IncidentClass.IntegrityIncident, "AUDIT_H03_FUTURE_SIGNAL_NOT_FAIL_CLOSED");
        var invalidSeverity = classifier.Classify(new[] { new G.ProtectionSignal("guardian", "bad", 101, true, now) }, now, TimeSpan.FromMinutes(1));
        Require(invalidSeverity.Classification == G.IncidentClass.IntegrityIncident, "AUDIT_H03_INVALID_SEVERITY_NOT_FAIL_CLOSED");

        var recovery = new G.GuardianRecoveryEvidence("recovery-evidence", 9, now, true, true, true);
        Require(!crisis.BeginRecovery(recovery with { RecoveryEpoch = 8 }, 9, now), "AUDIT_H03_WRONG_RECOVERY_EPOCH_ACCEPTED");
        Require(crisis.BeginRecovery(recovery, 9, now) && crisis.Mode == G.GuardianMode.Recovery, "AUDIT_H03_GOVERNED_RECOVERY_NOT_STARTED");
        Require(crisis.CompleteRecovery(recovery, 9, now) && crisis.Mode == G.GuardianMode.Normal, "AUDIT_H03_GOVERNED_RECOVERY_NOT_COMPLETED");
    }

    private static void ResourceReclaimabilityRequiresActualIdleHeadroom()
    {
        var overconsuming = new R.ResourceClaim("Trading", "CPU", 10m, 11m, 5m, 12m, 1m, 50, true, true);
        Require(!R.DemandIntegrityEvaluator.IsEligible(overconsuming), "AUDIT_M01_OVERCONSUMING_DONOR_ELIGIBLE");
        var contradictory = new R.ResourceClaim("Trading", "CPU", 10m, 9m, 5m, 12m, 2m, 50, true, true);
        Require(!R.DemandIntegrityEvaluator.IsEligible(contradictory), "AUDIT_M01_RECLAIMABLE_EXCEEDS_HEADROOM");
        var valid = contradictory with { Reclaimable = 1m };
        Require(R.DemandIntegrityEvaluator.IsEligible(valid) && R.DemandIntegrityEvaluator.SafeReclaimableHeadroom(valid) == 1m, "AUDIT_M01_VALID_HEADROOM_REJECTED");
    }

    private static void FailureLocalityRequiresFreshClockValidEvidence()
    {
        var now = DateTimeOffset.UtcNow;
        var account = Account();
        var scope = new T.OperationalFailureScope(account, "US", null, null, "route-a", null, null, null,
            T.OperationalFailureClass.ExecutionRouteUnavailable, T.OperationalTruthState.Unknown, T.OperationalContainmentState.None, T.OperationalRecoveryState.Investigating);
        var stale = new T.FailureLocalityEvidence("stale", T.ProvenFailureBlastRadius.AccountLocal, now.AddMinutes(-10), new[] { account });
        Require(T.OperationalFailureContainmentPolicy.Decide(scope, stale, now, TimeSpan.FromMinutes(1)).State == T.OperationalContainmentState.Expanded, "AUDIT_M02_STALE_LOCALITY_NARROWED_CONTAINMENT");
        var future = new T.FailureLocalityEvidence("future", T.ProvenFailureBlastRadius.AccountLocal, now.AddSeconds(1), new[] { account });
        Require(T.OperationalFailureContainmentPolicy.Decide(scope, future, now, TimeSpan.FromMinutes(1)).State == T.OperationalContainmentState.Expanded, "AUDIT_M02_FUTURE_LOCALITY_NARROWED_CONTAINMENT");
        var fresh = new T.FailureLocalityEvidence("fresh", T.ProvenFailureBlastRadius.AccountLocal, now.AddSeconds(-1), new[] { account });
        Require(T.OperationalFailureContainmentPolicy.Decide(scope, fresh, now, TimeSpan.FromMinutes(1)).State == T.OperationalContainmentState.Scoped, "AUDIT_M02_FRESH_LOCALITY_NOT_SCOPED");
    }

    private static void ProviderFutureObservationFailsClosed()
    {
        var now = DateTimeOffset.UtcNow;
        var result = new P.AnomalyDetector().Evaluate(10m, 10m, now.AddSeconds(1), now, TimeSpan.FromMinutes(1));
        Require(result.State == P.QualityState.Unknown && result.ReasonCode == "CLOCK_INVALID", "AUDIT_L01_FUTURE_PROVIDER_OBSERVATION_ACCEPTED");
    }

    private static void SimulatorLowLevelImpossibleInputsFailClosed()
    {
        RequireThrows<ArgumentOutOfRangeException>(() => new S.SimulationClock(new S.SimulationInstant(-1)), "AUDIT_L01_NEGATIVE_SIMULATION_CLOCK_ACCEPTED");
        RequireThrows<ArgumentOutOfRangeException>(() => new S.BrokerSimulator().Execute(new S.SimulatedOrder("x", -1m, 1m), 1m, 1m), "AUDIT_L01_NEGATIVE_SIM_ORDER_ACCEPTED");
        RequireThrows<ArgumentException>(() => new S.CalibrationEngine().Calibrate(1m, 1m, 1m, ""), "AUDIT_L01_EMPTY_CALIBRATION_EVIDENCE_ACCEPTED");
        Require(new S.ValidationAssessor().Assess(true, true, 1.1m).Recommendation == "NOT_READY", "AUDIT_L01_OUT_OF_RANGE_FIDELITY_ACCEPTED");
    }

    private sealed class CountingBrokerPort : TA.IBrokerExecutionPort
    {
        public int Submits { get; private set; }
        public ValueTask<TA.BrokerSubmissionResult> SubmitAsync(TA.OrderIntent intent, CancellationToken cancellationToken)
        {
            Submits++;
            return ValueTask.FromResult(new TA.BrokerSubmissionResult(intent.ExecutionIdentity, true, true, "SUBMITTED"));
        }

        public ValueTask<TA.BrokerOrderSnapshot> ReconcileAsync(TA.BrokerExecutionIdentity identity, CancellationToken cancellationToken)
            => ValueTask.FromResult(new TA.BrokerOrderSnapshot(identity, T.OrderState.ReconciliationRequired, new T.Quantity(0m), "RECONCILE"));
    }

    private static void Require(bool condition, string failure)
    {
        if (!condition) throw new InvalidOperationException(failure);
    }

    private static void RequireThrows<TException>(Action action, string failure) where TException : Exception
    {
        try
        {
            action();
            throw new InvalidOperationException(failure);
        }
        catch (TException)
        {
        }
    }
}
