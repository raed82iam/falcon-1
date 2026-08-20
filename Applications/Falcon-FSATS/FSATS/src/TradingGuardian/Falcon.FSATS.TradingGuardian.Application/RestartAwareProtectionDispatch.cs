using Falcon.FSATS.TradingGuardian.Contracts;

namespace Falcon.FSATS.TradingGuardian.Application;

public sealed class RestartAwareProtectionCommandDispatcher
{
    private readonly GovernedProtectionCommandDispatcher _inner;
    private readonly GuardianRestartPlan _restart;

    public RestartAwareProtectionCommandDispatcher(GovernedProtectionCommandDispatcher inner, GuardianRestartPlan restart)
    {
        _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        _restart = restart ?? throw new ArgumentNullException(nameof(restart));
    }

    public async ValueTask<ProtectionCommandOutcome> DispatchAsync(GovernedProtectionCommandEnvelope envelope, long currentEpoch, DateTimeOffset now, CancellationToken cancellationToken)
    {
        if (!_restart.Accepted)
        {
            var target = envelope?.Command?.Target ?? new ProtectionTarget(ProtectionTargetKind.Application);
            return new(envelope?.Command?.CommandId ?? new CommandId("invalid-command"), ProtectionOutcomeState.Rejected, envelope?.Command?.TargetApplication ?? "UNKNOWN", target, "GUARDIAN_RESTART_STATE_NOT_TRUSTED", now, envelope?.CorrelationId ?? "unknown-correlation", string.Empty, envelope?.EvidenceReference ?? string.Empty);
        }

        var validation = GovernedProtectionRouteGuards.Validate(envelope, currentEpoch, now);
        if (!validation.Accepted) return await _inner.DispatchAsync(envelope, currentEpoch, now, cancellationToken).ConfigureAwait(false);

        var key = GovernedProtectionRouteGuards.IdempotencyScopeKey(envelope);
        if (_restart.IdempotencyTombstones.TryGetValue(key, out var prior))
        {
            var fingerprint = GovernedProtectionRouteGuards.Fingerprint(envelope);
            if (!StringComparer.Ordinal.Equals(prior.Fingerprint, fingerprint))
                return new(envelope.Command.CommandId, ProtectionOutcomeState.Rejected, envelope.Command.TargetApplication, envelope.Command.Target, "RESTART_IDEMPOTENCY_CONFLICT", now, envelope.CorrelationId, fingerprint, envelope.EvidenceReference);
            return GuardianRestartReconstructor.ReplayView(prior);
        }

        return await _inner.DispatchAsync(envelope, currentEpoch, now, cancellationToken).ConfigureAwait(false);
    }
}
