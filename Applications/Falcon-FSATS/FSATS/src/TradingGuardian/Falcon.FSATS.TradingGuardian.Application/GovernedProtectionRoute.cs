using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using Falcon.FSATS.TradingGuardian.Contracts;

namespace Falcon.FSATS.TradingGuardian.Application;

public enum ProtectionTrafficTruth { Operational = 1, Replay = 2, Test = 3, Simulation = 4 }

public sealed record GovernedProtectionCommandEnvelope(string MessageId, string SchemaId, string SchemaVersion, string ProducerApplicationId, string RecipientApplicationId, string AuthorityReference, string ProvenanceReference, string CorrelationId, string CausationId, string IdempotencyId, string DeliveryAttemptId, string RetryLineageId, ProtectionTrafficTruth Truth, DateTimeOffset CreatedAt, DateTimeOffset? ExpiresAt, string EvidenceReference, ProtectionCommand Command);
public sealed record ProtectionRouteValidation(bool Accepted, string ReasonCode)
{
    public static ProtectionRouteValidation Pass() => new(true, "PROTECTION_ROUTE_ACCEPTED");
    public static ProtectionRouteValidation Reject(string reason) => new(false, reason);
}

public interface IGovernedProtectionCommandRoutePort
{
    ValueTask<ProtectionCommandOutcome> DispatchAsync(GovernedProtectionCommandEnvelope envelope, CancellationToken cancellationToken);
}

public static class GovernedProtectionRouteGuards
{
    public static ProtectionRouteValidation Validate(GovernedProtectionCommandEnvelope? envelope, long currentEpoch, DateTimeOffset now)
    {
        if (envelope is null) return ProtectionRouteValidation.Reject("NULL_PROTECTION_ENVELOPE");
        if (envelope.Command is null) return ProtectionRouteValidation.Reject("NULL_PROTECTION_COMMAND");
        if (envelope.Command.Target is null || !envelope.Command.Target.IsStructurallyValid()) return ProtectionRouteValidation.Reject("PROTECTION_TARGET_INVALID");
        if (envelope.Truth != ProtectionTrafficTruth.Operational) return ProtectionRouteValidation.Reject("NON_OPERATIONAL_PROTECTION_TRAFFIC");
        if (!StringComparer.Ordinal.Equals(envelope.ProducerApplicationId, TradingGuardianManifest.Current.ApplicationId)) return ProtectionRouteValidation.Reject("PRODUCER_APPLICATION_MISMATCH");
        if (!StringComparer.Ordinal.Equals(envelope.RecipientApplicationId, envelope.Command.TargetApplication)) return ProtectionRouteValidation.Reject("RECIPIENT_APPLICATION_MISMATCH");
        if (!StringComparer.Ordinal.Equals(envelope.AuthorityReference, envelope.Command.AuthorityBasis)) return ProtectionRouteValidation.Reject("AUTHORITY_BINDING_MISMATCH");
        if (!StringComparer.Ordinal.Equals(envelope.CorrelationId, envelope.Command.CorrelationId) || !StringComparer.Ordinal.Equals(envelope.CausationId, envelope.Command.CausationId)) return ProtectionRouteValidation.Reject("CAUSATION_BINDING_MISMATCH");
        if (StringComparer.Ordinal.Equals(envelope.CorrelationId, envelope.CausationId)) return ProtectionRouteValidation.Reject("CORRELATION_CAUSATION_COLLISION");
        if (string.IsNullOrWhiteSpace(envelope.MessageId) || string.IsNullOrWhiteSpace(envelope.SchemaId) || string.IsNullOrWhiteSpace(envelope.SchemaVersion) || string.IsNullOrWhiteSpace(envelope.IdempotencyId) || string.IsNullOrWhiteSpace(envelope.DeliveryAttemptId) || string.IsNullOrWhiteSpace(envelope.RetryLineageId) || string.IsNullOrWhiteSpace(envelope.ProvenanceReference) || string.IsNullOrWhiteSpace(envelope.EvidenceReference)) return ProtectionRouteValidation.Reject("INCOMPLETE_PROTECTION_ENVELOPE");
        if (envelope.CreatedAt == default || envelope.CreatedAt > now) return ProtectionRouteValidation.Reject("INVALID_PROTECTION_MESSAGE_TIME");
        if (envelope.ExpiresAt is { } envelopeExpiry && envelopeExpiry <= now) return ProtectionRouteValidation.Reject("PROTECTION_MESSAGE_EXPIRED");
        if (envelope.Command.Epoch.Value != currentEpoch) return ProtectionRouteValidation.Reject("STALE_COMMAND_EPOCH");
        if (envelope.Command.ExpiresAt is { } commandExpiry && commandExpiry <= now) return ProtectionRouteValidation.Reject("PROTECTION_COMMAND_EXPIRED");
        if (envelope.Command.EffectiveAt > now) return ProtectionRouteValidation.Reject("COMMAND_NOT_YET_EFFECTIVE");
        if (envelope.ExpiresAt is { } outer && envelope.Command.ExpiresAt is { } inner && outer != inner) return ProtectionRouteValidation.Reject("EXPIRY_BINDING_MISMATCH");
        return ProtectionRouteValidation.Pass();
    }

    public static string IdempotencyScopeKey(GovernedProtectionCommandEnvelope envelope)
        => string.Join('|', Part(envelope.Command.TargetApplication), envelope.Command.Target.CanonicalKey, Part(envelope.IdempotencyId.Trim()));

    public static string Fingerprint(GovernedProtectionCommandEnvelope envelope)
    {
        var value = string.Join('|',
            Part(envelope.SchemaId), Part(envelope.SchemaVersion), Part(envelope.ProducerApplicationId), Part(envelope.RecipientApplicationId),
            Part(envelope.AuthorityReference), Part(envelope.CorrelationId), Part(envelope.CausationId), Part(envelope.IdempotencyId),
            Part(envelope.Command.CommandId.Value), envelope.Command.Type, Part(envelope.Command.TargetApplication), envelope.Command.Target.CanonicalKey,
            Part(envelope.Command.AuthorityBasis), Part(envelope.Command.ReasonCode), envelope.Command.Epoch.Value,
            envelope.Command.EffectiveAt.ToUniversalTime().ToString("O"), envelope.Command.ExpiresAt?.ToUniversalTime().ToString("O") ?? string.Empty);
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
    }

    private static string Part(string value) => Uri.EscapeDataString(value ?? string.Empty);
}

public sealed class GovernedProtectionCommandDispatcher
{
    private readonly IGovernedProtectionCommandRoutePort _route;
    private readonly ConcurrentDictionary<string, (string Fingerprint, ProtectionCommandOutcome Outcome)> _idempotency = new(StringComparer.Ordinal);
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _dispatchGates = new(StringComparer.Ordinal);
    private readonly object _capacityGate = new();
    private readonly HashSet<string> _admittedKeys = new(StringComparer.Ordinal);
    private readonly int _maximumTrackedCommands;

    public GovernedProtectionCommandDispatcher(IGovernedProtectionCommandRoutePort route, int maximumTrackedCommands = 100_000)
    {
        _route = route ?? throw new ArgumentNullException(nameof(route));
        if (maximumTrackedCommands <= 0) throw new ArgumentOutOfRangeException(nameof(maximumTrackedCommands));
        _maximumTrackedCommands = maximumTrackedCommands;
    }

    public async ValueTask<ProtectionCommandOutcome> DispatchAsync(GovernedProtectionCommandEnvelope envelope, long currentEpoch, DateTimeOffset now, CancellationToken cancellationToken)
    {
        var validation = GovernedProtectionRouteGuards.Validate(envelope, currentEpoch, now);
        if (!validation.Accepted)
        {
            var target = envelope?.Command?.Target ?? new ProtectionTarget(ProtectionTargetKind.Application);
            return new ProtectionCommandOutcome(envelope?.Command?.CommandId ?? new CommandId("invalid-command"), ProtectionOutcomeState.Rejected, envelope?.Command?.TargetApplication ?? "UNKNOWN", target, validation.ReasonCode, now, envelope?.CorrelationId ?? "unknown-correlation", string.Empty, envelope?.EvidenceReference ?? string.Empty);
        }

        var fingerprint = GovernedProtectionRouteGuards.Fingerprint(envelope);
        var idempotencyKey = GovernedProtectionRouteGuards.IdempotencyScopeKey(envelope);
        if (_idempotency.TryGetValue(idempotencyKey, out var prior)) return ResolvePrior(envelope, fingerprint, prior, now);
        if (!TryAdmitKey(idempotencyKey)) return CapacityRejected(envelope, fingerprint, now);

        var gate = _dispatchGates.GetOrAdd(idempotencyKey, static _ => new SemaphoreSlim(1, 1));
        await gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (_idempotency.TryGetValue(idempotencyKey, out prior)) return ResolvePrior(envelope, fingerprint, prior, now);

            ProtectionCommandOutcome outcome;
            try
            {
                outcome = await _route.DispatchAsync(envelope, cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                outcome = ReconciliationOutcome(envelope, fingerprint, now, "ROUTE_DISPATCH_CANCELLATION_AMBIGUOUS");
                _idempotency[idempotencyKey] = (fingerprint, outcome);
                return outcome;
            }
            catch (Exception)
            {
                outcome = ReconciliationOutcome(envelope, fingerprint, now, "ROUTE_DISPATCH_EXCEPTION");
                _idempotency[idempotencyKey] = (fingerprint, outcome);
                return outcome;
            }

            if (outcome is null) outcome = ReconciliationOutcome(envelope, fingerprint, now, "NULL_ROUTE_OUTCOME");
            else if (!StringComparer.Ordinal.Equals(outcome.CommandId.Value, envelope.Command.CommandId.Value) || !StringComparer.Ordinal.Equals(outcome.TargetApplication, envelope.Command.TargetApplication) || outcome.Target != envelope.Command.Target || !StringComparer.Ordinal.Equals(outcome.CorrelationId, envelope.CorrelationId)) outcome = ReconciliationOutcome(envelope, fingerprint, now, "ROUTE_OUTCOME_BINDING_MISMATCH");
            else outcome = outcome with { RequestFingerprint = fingerprint, EvidenceReference = envelope.EvidenceReference };

            _idempotency[idempotencyKey] = (fingerprint, outcome);
            return outcome;
        }
        finally { gate.Release(); }
    }

    private bool TryAdmitKey(string key)
    {
        lock (_capacityGate)
        {
            if (_admittedKeys.Contains(key)) return true;
            if (_admittedKeys.Count >= _maximumTrackedCommands) return false;
            return _admittedKeys.Add(key);
        }
    }

    private static ProtectionCommandOutcome CapacityRejected(GovernedProtectionCommandEnvelope envelope, string fingerprint, DateTimeOffset now)
        => new(envelope.Command.CommandId, ProtectionOutcomeState.Rejected, envelope.Command.TargetApplication, envelope.Command.Target, "PROTECTION_IDEMPOTENCY_CAPACITY_EXHAUSTED_FAIL_CLOSED", now, envelope.CorrelationId, fingerprint, envelope.EvidenceReference);

    private static ProtectionCommandOutcome ReconciliationOutcome(GovernedProtectionCommandEnvelope envelope, string fingerprint, DateTimeOffset now, string reasonCode)
        => new(envelope.Command.CommandId, ProtectionOutcomeState.ReconciliationRequired, envelope.Command.TargetApplication, envelope.Command.Target, reasonCode, now, envelope.CorrelationId, fingerprint, envelope.EvidenceReference);

    private static ProtectionCommandOutcome ResolvePrior(GovernedProtectionCommandEnvelope envelope, string fingerprint, (string Fingerprint, ProtectionCommandOutcome Outcome) prior, DateTimeOffset now)
    {
        if (StringComparer.Ordinal.Equals(prior.Fingerprint, fingerprint)) return prior.Outcome;
        return new ProtectionCommandOutcome(envelope.Command.CommandId, ProtectionOutcomeState.Rejected, envelope.Command.TargetApplication, envelope.Command.Target, "IDEMPOTENCY_CONFLICT", now, envelope.CorrelationId, fingerprint, envelope.EvidenceReference);
    }
}
