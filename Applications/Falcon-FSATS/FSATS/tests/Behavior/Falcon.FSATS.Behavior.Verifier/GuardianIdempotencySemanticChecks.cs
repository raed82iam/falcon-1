using GA = Falcon.FSATS.TradingGuardian.Application;
using GC = Falcon.FSATS.TradingGuardian.Contracts;

internal static class GuardianIdempotencySemanticChecks
{
    internal static void Run() => LogicalRetryAndCrossAccountIsolation();

    private static void LogicalRetryAndCrossAccountIsolation()
    {
        var now = DateTimeOffset.UtcNow;
        var targetA = new GC.ProtectionTarget(GC.ProtectionTargetKind.BrokerAccount, "ALPACA", "PA-ACCOUNT-A", "PAPER");
        var command = new GC.ProtectionCommand(new GC.CommandId("cmd-semantic-idem"), GC.ProtectionCommandType.NewRiskFreeze, "TARGET", targetA,
            "authority", "reason", new GC.ProtectionEpoch(11), now.AddSeconds(-1), now.AddMinutes(2), "corr", "cause");
        var envelope = new GA.GovernedProtectionCommandEnvelope("message-1", "schema", "1.0", GA.TradingGuardianManifest.Current.ApplicationId, "TARGET",
            "authority", "provenance-1", "corr", "cause", "idem-semantic", "attempt-1", "retry-1", GA.ProtectionTrafficTruth.Operational,
            now.AddSeconds(-1), now.AddMinutes(2), "evidence-1", command);

        var route = new CountingRoute();
        var dispatcher = new GA.GovernedProtectionCommandDispatcher(route);
        var first = dispatcher.DispatchAsync(envelope, 11, now, CancellationToken.None).AsTask().GetAwaiter().GetResult();
        if (first.State != GC.ProtectionOutcomeState.Applied || route.Calls != 1) throw new InvalidOperationException("GUARDIAN_BASELINE_COMMAND_NOT_APPLIED");

        var retry = envelope with { MessageId = "message-2", ProvenanceReference = "provenance-2", DeliveryAttemptId = "attempt-2", RetryLineageId = "retry-2", EvidenceReference = "evidence-2", CreatedAt = now };
        var retried = dispatcher.DispatchAsync(retry, 11, now, CancellationToken.None).AsTask().GetAwaiter().GetResult();
        if (retried.State != GC.ProtectionOutcomeState.Applied || route.Calls != 1) throw new InvalidOperationException("TRANSPORT_RETRY_FALSELY_REDISPATCHED");

        var changedMeaningSameTarget = retry with { Command = command with { ReasonCode = "different-reason" } };
        var conflict = dispatcher.DispatchAsync(changedMeaningSameTarget, 11, now, CancellationToken.None).AsTask().GetAwaiter().GetResult();
        if (conflict.State != GC.ProtectionOutcomeState.Rejected || conflict.ReasonCode != "IDEMPOTENCY_CONFLICT" || route.Calls != 1)
            throw new InvalidOperationException("SAME_TARGET_SEMANTIC_IDEMPOTENCY_CONFLICT_NOT_REJECTED");

        var targetB = new GC.ProtectionTarget(GC.ProtectionTargetKind.BrokerAccount, "alpaca", "PA-ACCOUNT-B", "paper");
        var commandB = command with { CommandId = new GC.CommandId("cmd-b"), Target = targetB, CorrelationId = "corr-b", CausationId = "cause-b" };
        var envelopeB = envelope with { MessageId = "message-b", CorrelationId = "corr-b", CausationId = "cause-b", Command = commandB };
        var independent = dispatcher.DispatchAsync(envelopeB, 11, now, CancellationToken.None).AsTask().GetAwaiter().GetResult();
        if (independent.State != GC.ProtectionOutcomeState.Applied || route.Calls != 2 || targetB.BrokerId != "ALPACA")
            throw new InvalidOperationException("CROSS_ACCOUNT_IDEMPOTENCY_NAMESPACE_COLLISION_OR_CASE_BYPASS");
    }

    private sealed class CountingRoute : GA.IGovernedProtectionCommandRoutePort
    {
        public int Calls { get; private set; }
        public ValueTask<GC.ProtectionCommandOutcome> DispatchAsync(GA.GovernedProtectionCommandEnvelope envelope, CancellationToken cancellationToken)
        {
            Calls++;
            return ValueTask.FromResult(new GC.ProtectionCommandOutcome(envelope.Command.CommandId, GC.ProtectionOutcomeState.Applied,
                envelope.Command.TargetApplication, envelope.Command.Target, "APPLIED", DateTimeOffset.UtcNow, envelope.CorrelationId));
        }
    }
}
