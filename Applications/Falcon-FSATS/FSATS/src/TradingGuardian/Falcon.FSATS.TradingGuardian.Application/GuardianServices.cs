using Falcon.FSATS.TradingGuardian.Contracts;
using Falcon.FSATS.TradingGuardian.Domain;

namespace Falcon.FSATS.TradingGuardian.Application;

public sealed class ProtectionCoordinator
{
    private readonly IncidentClassifier _classifier;
    private readonly CrisisStateMachine _crisis;
    private readonly GovernedProtectionCommandDispatcher _commands;

    public ProtectionCoordinator(
        IncidentClassifier classifier,
        CrisisStateMachine crisis,
        GovernedProtectionCommandDispatcher commands)
    {
        _classifier = classifier ?? throw new ArgumentNullException(nameof(classifier));
        _crisis = crisis ?? throw new ArgumentNullException(nameof(crisis));
        _commands = commands ?? throw new ArgumentNullException(nameof(commands));
    }

    public IncidentQualification Observe(IReadOnlyCollection<ProtectionSignal> signals)
    {
        ArgumentNullException.ThrowIfNull(signals);
        var result = _classifier.Classify(signals);
        _crisis.Apply(result);
        return result;
    }

    // The only dispatch-capable coordinator path requires the complete governed envelope.
    public ValueTask<ProtectionCommandOutcome> IssueAsync(
        GovernedProtectionCommandEnvelope envelope,
        long currentEpoch,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(envelope);
        return _commands.DispatchAsync(envelope, currentEpoch, now, cancellationToken);
    }

    // Historical raw-command surface retained only as a fail-closed compatibility trap.
    // It can never reach an external route because raw ProtectionCommand lacks the governed
    // producer/provenance/evidence/idempotency/traffic-truth envelope required by FCR-0004.
    public ValueTask<ProtectionCommandOutcome> IssueAsync(
        ProtectionCommand command,
        long currentEpoch,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);
        cancellationToken.ThrowIfCancellationRequested();
        return ValueTask.FromResult(new ProtectionCommandOutcome(
            command.CommandId,
            ProtectionOutcomeState.Rejected,
            command.TargetApplication,
            command.Target,
            "GOVERNED_PROTECTION_ENVELOPE_REQUIRED",
            now,
            command.CorrelationId,
            string.Empty,
            string.Empty));
    }
}
