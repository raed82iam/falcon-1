using GA = Falcon.FSATS.TradingGuardian.Application;
using GC = Falcon.FSATS.TradingGuardian.Contracts;

internal static class GuardianDispatcherAdversarialChecks
{
    internal static void Run()
        => RunAsync().GetAwaiter().GetResult();

    private static async Task RunAsync()
    {
        var now = DateTimeOffset.UtcNow;
        var target = new GC.ProtectionTarget(GC.ProtectionTargetKind.BrokerAccount, "ALPACA", "PA-ACCOUNT-A", "PAPER");
        var command = new GC.ProtectionCommand(
            new GC.CommandId("concurrent-kill-command"),
            GC.ProtectionCommandType.NewRiskFreeze,
            "FSATS-TRADING",
            target,
            "authority-guardian-concurrent",
            "risk-freeze",
            new GC.ProtectionEpoch(11),
            now.AddSeconds(-1),
            now.AddMinutes(2),
            "corr-concurrent",
            "cause-concurrent");

        var envelope = new GA.GovernedProtectionCommandEnvelope(
            "msg-concurrent",
            "fsats.guardian.protection",
            "1.0",
            GA.TradingGuardianManifest.Current.ApplicationId,
            "FSATS-TRADING",
            "authority-guardian-concurrent",
            "prov-concurrent",
            "corr-concurrent",
            "cause-concurrent",
            "idem-concurrent",
            "attempt-concurrent",
            "retry-concurrent",
            GA.ProtectionTrafficTruth.Operational,
            now.AddSeconds(-1),
            now.AddMinutes(2),
            "evidence-concurrent",
            command);

        var route = new BlockingProtectionRoutePort();
        var dispatcher = new GA.GovernedProtectionCommandDispatcher(route);

        var first = dispatcher.DispatchAsync(envelope, 11, now, CancellationToken.None).AsTask();
        await route.FirstDispatchEntered.Task.ConfigureAwait(false);
        var second = dispatcher.DispatchAsync(envelope, 11, now, CancellationToken.None).AsTask();

        await Task.Delay(25).ConfigureAwait(false);
        if (route.Calls != 1)
            throw new InvalidOperationException($"C-02_DUPLICATE_DISPATCH_RACE:{route.Calls}");

        route.AllowCompletion.TrySetResult();
        var outcomes = await Task.WhenAll(first, second).ConfigureAwait(false);

        if (route.Calls != 1) throw new InvalidOperationException($"C-02_DUPLICATE_DISPATCH_DETECTED:{route.Calls}");
        if (outcomes.Any(outcome => outcome.State != GC.ProtectionOutcomeState.Applied || outcome.Target != target))
            throw new InvalidOperationException("C-02_IDEMPOTENT_OUTCOME_TARGET_MISMATCH");
    }

    private sealed class BlockingProtectionRoutePort : GA.IGovernedProtectionCommandRoutePort
    {
        private int _calls;
        public int Calls => Volatile.Read(ref _calls);
        public TaskCompletionSource FirstDispatchEntered { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public TaskCompletionSource AllowCompletion { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public async ValueTask<GC.ProtectionCommandOutcome> DispatchAsync(
            GA.GovernedProtectionCommandEnvelope envelope,
            CancellationToken cancellationToken)
        {
            var calls = Interlocked.Increment(ref _calls);
            if (calls == 1) FirstDispatchEntered.TrySetResult();
            await AllowCompletion.Task.WaitAsync(cancellationToken).ConfigureAwait(false);

            return new GC.ProtectionCommandOutcome(
                envelope.Command.CommandId,
                GC.ProtectionOutcomeState.Applied,
                envelope.Command.TargetApplication,
                envelope.Command.Target,
                "PROTECTION_APPLIED",
                DateTimeOffset.UtcNow,
                envelope.CorrelationId);
        }
    }
}
