using Falcon.FSATS.TradingGuardian.Application;
using Falcon.FSATS.TradingGuardian.Contracts;

namespace Falcon.FSATS.TradingGuardian.Infrastructure;

// Non-runtime composition port. Even this disabled adapter receives the complete governed
// envelope so there is no production-bindable raw ProtectionCommand route.
public sealed class DisabledProtectionCommandPort : IGovernedProtectionCommandRoutePort
{
    public ValueTask<ProtectionCommandOutcome> DispatchAsync(
        GovernedProtectionCommandEnvelope envelope,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(envelope);
        ArgumentNullException.ThrowIfNull(envelope.Command);
        cancellationToken.ThrowIfCancellationRequested();

        var command = envelope.Command;
        return ValueTask.FromResult(new ProtectionCommandOutcome(
            command.CommandId,
            ProtectionOutcomeState.Rejected,
            command.TargetApplication,
            command.Target,
            "FOUNDATION_PROTECTION_ROUTE_NOT_BOUND",
            DateTimeOffset.UtcNow,
            envelope.CorrelationId,
            GovernedProtectionRouteGuards.Fingerprint(envelope),
            envelope.EvidenceReference));
    }
}
