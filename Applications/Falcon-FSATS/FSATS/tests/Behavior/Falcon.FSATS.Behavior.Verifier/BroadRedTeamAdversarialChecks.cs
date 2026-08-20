using T = Falcon.FSATS.Trading.Domain;
using G = Falcon.FSATS.TradingGuardian.Domain;
using GA = Falcon.FSATS.TradingGuardian.Application;
using GC = Falcon.FSATS.TradingGuardian.Contracts;
using R = Falcon.FSATS.ResourceManagement.Domain;
using RA = Falcon.FSATS.ResourceManagement.Application;

internal static class BroadRedTeamAdversarialChecks
{
    internal static void Run()
    {
        GuardianGovernedCoordinatorRejectsWrongTargetOutcome();
        GuardianGovernedCoordinatorFailsClosedOnDispatchException();
        AppRscRejectsExpiredResidualRequestBeforeEgress();
        AppRscRejectsFoundationOverGrant();
        AppRscFailsClosedWhenFoundationOutcomeUnavailable();
        Part7RuntimeReadinessAdversarialChecks.Run();
    }

    private static void GuardianGovernedCoordinatorRejectsWrongTargetOutcome()
    {
        var now = DateTimeOffset.UtcNow;
        var expected = new GC.ProtectionTarget(GC.ProtectionTargetKind.BrokerAccount, "ALPACA", "PA-A", "PAPER");
        var wrong = new GC.ProtectionTarget(GC.ProtectionTargetKind.BrokerAccount, "ALPACA", "PA-B", "PAPER");
        var command = Command(expected, now);
        var port = new WrongTargetCommandRoutePort(wrong);
        var dispatcher = new GA.GovernedProtectionCommandDispatcher(port);
        var coordinator = new GA.ProtectionCoordinator(new G.IncidentClassifier(), new G.CrisisStateMachine(), dispatcher);
        var envelope = Envelope(command, now);
        var outcome = coordinator.IssueAsync(envelope, command.Epoch.Value, now, CancellationToken.None).AsTask().GetAwaiter().GetResult();
        if (outcome.State != GC.ProtectionOutcomeState.ReconciliationRequired || outcome.Target != expected || port.Calls != 1 || outcome.ReasonCode != "ROUTE_OUTCOME_BINDING_MISMATCH")
            throw new InvalidOperationException("BROAD_RT_GUARDIAN_GOVERNED_WRONG_TARGET_OUTCOME_ACCEPTED");
    }

    private static void GuardianGovernedCoordinatorFailsClosedOnDispatchException()
    {
        var now = DateTimeOffset.UtcNow;
        var target = new GC.ProtectionTarget(GC.ProtectionTargetKind.BrokerAccount, "ALPACA", "PA-A", "PAPER");
        var command = Command(target, now);
        var dispatcher = new GA.GovernedProtectionCommandDispatcher(new ThrowingCommandRoutePort());
        var coordinator = new GA.ProtectionCoordinator(new G.IncidentClassifier(), new G.CrisisStateMachine(), dispatcher);
        var envelope = Envelope(command, now);
        var outcome = coordinator.IssueAsync(envelope, command.Epoch.Value, now, CancellationToken.None).AsTask().GetAwaiter().GetResult();
        if (outcome.State != GC.ProtectionOutcomeState.ReconciliationRequired || outcome.Target != target || outcome.ReasonCode != "ROUTE_DISPATCH_EXCEPTION")
            throw new InvalidOperationException("BROAD_RT_GUARDIAN_GOVERNED_EXCEPTION_NOT_RECONCILIATION_OWNED");
    }

    private static GC.ProtectionCommand Command(GC.ProtectionTarget target, DateTimeOffset now)
        => new(
            new GC.CommandId("broad-rt-command"),
            GC.ProtectionCommandType.NewRiskFreeze,
            "TARGET",
            target,
            "authority",
            "reason",
            new GC.ProtectionEpoch(21),
            now.AddSeconds(-1),
            now.AddMinutes(1),
            "corr-broad",
            "cause-broad");

    private static GA.GovernedProtectionCommandEnvelope Envelope(GC.ProtectionCommand command, DateTimeOffset now)
        => new(
            "msg-broad",
            "falcon.protection-command",
            "1.0",
            GA.TradingGuardianManifest.Current.ApplicationId,
            command.TargetApplication,
            command.AuthorityBasis,
            "prov-broad",
            command.CorrelationId,
            command.CausationId,
            "idem-broad",
            "delivery-broad",
            "retry-broad",
            GA.ProtectionTrafficTruth.Operational,
            now.AddSeconds(-1),
            command.ExpiresAt,
            "evidence-broad",
            command);

    private static void AppRscRejectsExpiredResidualRequestBeforeEgress()
    {
        var now = DateTimeOffset.UtcNow;
        var port = new FoundationPort();
        var service = new RA.FoundationResourceBindingService(port);
        var claim = Claim();
        var result = service.RequestResidualAsync(
            "expired-request", claim, 0m, "evidence", "instance", "coordinator", "epoch-1", "fsats", "unit",
            "corr", "cause", now.AddMinutes(-2), now.AddMinutes(-1), now, CancellationToken.None).AsTask().GetAwaiter().GetResult();
        if (result.Bound || result.Granted || port.RequestCalls != 0 || result.ReasonCode != "INVALID_FOUNDATION_REQUEST_BINDING")
            throw new InvalidOperationException("BROAD_RT_APP_RSC_EXPIRED_REQUEST_REACHED_FOUNDATION_PORT");
    }

    private static void AppRscRejectsFoundationOverGrant()
    {
        var now = DateTimeOffset.UtcNow;
        var port = new FoundationPort
        {
            OutcomeFactory = request => new RA.FoundationAdditionalResourceOutcome(
                true,
                RA.FoundationResourceDecisionKind.Grant,
                request.ProvenResidualNeed + 1m,
                request.Unit,
                "decision-overgrant",
                request.RequestId,
                request.EpochId,
                "foundation-outcome-overgrant",
                now,
                now.AddMinutes(1))
        };
        var service = new RA.FoundationResourceBindingService(port);
        var result = service.RequestResidualAsync(
            "overgrant-request", Claim(), 0m, "evidence", "instance", "coordinator", "epoch-1", "fsats", "unit",
            "corr", "cause", now.AddSeconds(-1), now.AddMinutes(1), now, CancellationToken.None).AsTask().GetAwaiter().GetResult();
        if (result.Bound || result.Granted || result.ReasonCode != "FOUNDATION_OUTCOME_BINDING_REJECTED")
            throw new InvalidOperationException("BROAD_RT_APP_RSC_FOUNDATION_OVERGRANT_ACCEPTED");
    }

    private static void AppRscFailsClosedWhenFoundationOutcomeUnavailable()
    {
        var now = DateTimeOffset.UtcNow;
        var port = new FoundationPort { ThrowOnRequest = true };
        var service = new RA.FoundationResourceBindingService(port);
        var result = service.RequestResidualAsync(
            "unavailable-request", Claim(), 0m, "evidence", "instance", "coordinator", "epoch-1", "fsats", "unit",
            "corr", "cause", now.AddSeconds(-1), now.AddMinutes(1), now, CancellationToken.None).AsTask().GetAwaiter().GetResult();
        if (result.Bound || result.Granted || result.ReasonCode != "FOUNDATION_REQUEST_OUTCOME_UNAVAILABLE")
            throw new InvalidOperationException("BROAD_RT_APP_RSC_FOUNDATION_EXCEPTION_ESCAPED_FAIL_CLOSED_RESULT");
    }

    private static R.ResourceClaim Claim()
        => new("Trading", "CPU", 10m, 9m, 5m, 15m, 1m, 100, true, true);

    private sealed class WrongTargetCommandRoutePort : GA.IGovernedProtectionCommandRoutePort
    {
        private readonly GC.ProtectionTarget _wrongTarget;
        public WrongTargetCommandRoutePort(GC.ProtectionTarget wrongTarget) => _wrongTarget = wrongTarget;
        public int Calls { get; private set; }

        public ValueTask<GC.ProtectionCommandOutcome> DispatchAsync(GA.GovernedProtectionCommandEnvelope envelope, CancellationToken cancellationToken)
        {
            Calls++;
            var command = envelope.Command;
            return ValueTask.FromResult(new GC.ProtectionCommandOutcome(
                command.CommandId,
                GC.ProtectionOutcomeState.Applied,
                command.TargetApplication,
                _wrongTarget,
                "wrong-target-applied",
                DateTimeOffset.UtcNow,
                command.CorrelationId));
        }
    }

    private sealed class ThrowingCommandRoutePort : GA.IGovernedProtectionCommandRoutePort
    {
        public ValueTask<GC.ProtectionCommandOutcome> DispatchAsync(GA.GovernedProtectionCommandEnvelope envelope, CancellationToken cancellationToken)
            => ValueTask.FromException<GC.ProtectionCommandOutcome>(new IOException("route unavailable"));
    }

    private sealed class FoundationPort : RA.IFoundationResourceBindingPort
    {
        public int RequestCalls { get; private set; }
        public bool ThrowOnRequest { get; init; }
        public Func<RA.FoundationAdditionalResourceRequest, RA.FoundationAdditionalResourceOutcome>? OutcomeFactory { get; init; }

        public ValueTask<RA.FoundationResourceStateProjection?> ReadApplicationStateAsync(string applicationId, string resourceClass, string expectedEpochId, CancellationToken cancellationToken)
            => ValueTask.FromResult<RA.FoundationResourceStateProjection?>(null);

        public ValueTask<RA.FoundationLoadSheddingSignal?> ReadLoadSheddingSignalAsync(string applicationId, string resourceClass, string expectedEpochId, CancellationToken cancellationToken)
            => ValueTask.FromResult<RA.FoundationLoadSheddingSignal?>(null);

        public ValueTask<RA.FoundationAdditionalResourceOutcome> RequestAdditionalAsync(RA.FoundationAdditionalResourceRequest request, CancellationToken cancellationToken)
        {
            RequestCalls++;
            if (ThrowOnRequest) return ValueTask.FromException<RA.FoundationAdditionalResourceOutcome>(new IOException("foundation unavailable"));
            var outcome = OutcomeFactory?.Invoke(request) ?? new RA.FoundationAdditionalResourceOutcome(
                true,
                RA.FoundationResourceDecisionKind.Grant,
                request.ProvenResidualNeed,
                request.Unit,
                "decision",
                request.RequestId,
                request.EpochId,
                "foundation-outcome",
                DateTimeOffset.UtcNow,
                DateTimeOffset.UtcNow.AddMinutes(1));
            return ValueTask.FromResult(outcome);
        }
    }
}
