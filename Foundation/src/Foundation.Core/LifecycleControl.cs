using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Foundation.Infrastructure")]

namespace Foundation.Core;

public enum LifecycleState
{
    Registered,
    Initializing,
    Ready,
    Running,
    Restricted,
    Suspended,
    Stopping,
    Stopped,
    Failed,
    Recovering,
    Retired
}

public enum LifecycleAttemptDecision
{
    Accepted,
    Rejected,
    Failed
}

public sealed record LifecycleTransitionRule(LifecycleState Source, LifecycleState Target);

public static class LifecycleStateVocabulary
{
    public static string ToContractText(LifecycleState state) => state switch
    {
        LifecycleState.Registered => "REGISTERED",
        LifecycleState.Initializing => "INITIALIZING",
        LifecycleState.Ready => "READY",
        LifecycleState.Running => "RUNNING",
        LifecycleState.Restricted => "RESTRICTED",
        LifecycleState.Suspended => "SUSPENDED",
        LifecycleState.Stopping => "STOPPING",
        LifecycleState.Stopped => "STOPPED",
        LifecycleState.Failed => "FAILED",
        LifecycleState.Recovering => "RECOVERING",
        LifecycleState.Retired => "RETIRED",
        _ => throw new ArgumentOutOfRangeException(nameof(state), state, "Unknown lifecycle state.")
    };

    public static bool TryParseContractText(string value, out LifecycleState state)
    {
        switch (value)
        {
            case "REGISTERED":
                state = LifecycleState.Registered;
                return true;
            case "INITIALIZING":
                state = LifecycleState.Initializing;
                return true;
            case "READY":
                state = LifecycleState.Ready;
                return true;
            case "RUNNING":
                state = LifecycleState.Running;
                return true;
            case "RESTRICTED":
                state = LifecycleState.Restricted;
                return true;
            case "SUSPENDED":
                state = LifecycleState.Suspended;
                return true;
            case "STOPPING":
                state = LifecycleState.Stopping;
                return true;
            case "STOPPED":
                state = LifecycleState.Stopped;
                return true;
            case "FAILED":
                state = LifecycleState.Failed;
                return true;
            case "RECOVERING":
                state = LifecycleState.Recovering;
                return true;
            case "RETIRED":
                state = LifecycleState.Retired;
                return true;
            default:
                state = default;
                return false;
        }
    }
}

public sealed class LifecycleStateModel
{
    private readonly HashSet<LifecycleTransitionRule> _rules;

    public LifecycleStateModel(
        string modelId,
        string version,
        int maxRestartAttempts,
        IEnumerable<LifecycleTransitionRule> rules)
    {
        if (string.IsNullOrWhiteSpace(modelId))
        {
            throw new ArgumentException("Lifecycle model identity is required.", nameof(modelId));
        }

        if (string.IsNullOrWhiteSpace(version))
        {
            throw new ArgumentException("Lifecycle model version is required.", nameof(version));
        }

        if (maxRestartAttempts < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxRestartAttempts));
        }

        ArgumentNullException.ThrowIfNull(rules);

        var frozenRules = rules.ToArray();
        if (frozenRules.Length == 0)
        {
            throw new ArgumentException("At least one lifecycle transition rule is required.", nameof(rules));
        }

        _rules = new HashSet<LifecycleTransitionRule>(frozenRules);
        if (_rules.Count != frozenRules.Length)
        {
            throw new ArgumentException("Duplicate lifecycle transition rules are prohibited.", nameof(rules));
        }

        ModelId = modelId;
        Version = version;
        MaxRestartAttempts = maxRestartAttempts;
        Rules = Array.AsReadOnly(frozenRules);
    }

    public string ModelId { get; }

    public string Version { get; }

    public int MaxRestartAttempts { get; }

    public ReadOnlyCollection<LifecycleTransitionRule> Rules { get; }

    public bool Allows(LifecycleState source, LifecycleState target)
        => _rules.Contains(new LifecycleTransitionRule(source, target));

    public static LifecycleStateModel CreateCanonical()
        => new(
            "SYS-002-CANONICAL-LIFECYCLE",
            "1.1",
            3,
            new[]
            {
                new LifecycleTransitionRule(LifecycleState.Registered, LifecycleState.Initializing),
                new LifecycleTransitionRule(LifecycleState.Registered, LifecycleState.Restricted),
                new LifecycleTransitionRule(LifecycleState.Registered, LifecycleState.Retired),

                new LifecycleTransitionRule(LifecycleState.Initializing, LifecycleState.Ready),
                new LifecycleTransitionRule(LifecycleState.Initializing, LifecycleState.Restricted),
                new LifecycleTransitionRule(LifecycleState.Initializing, LifecycleState.Suspended),
                new LifecycleTransitionRule(LifecycleState.Initializing, LifecycleState.Stopping),
                new LifecycleTransitionRule(LifecycleState.Initializing, LifecycleState.Failed),

                new LifecycleTransitionRule(LifecycleState.Ready, LifecycleState.Running),
                new LifecycleTransitionRule(LifecycleState.Ready, LifecycleState.Restricted),
                new LifecycleTransitionRule(LifecycleState.Ready, LifecycleState.Suspended),
                new LifecycleTransitionRule(LifecycleState.Ready, LifecycleState.Stopping),
                new LifecycleTransitionRule(LifecycleState.Ready, LifecycleState.Failed),

                new LifecycleTransitionRule(LifecycleState.Running, LifecycleState.Restricted),
                new LifecycleTransitionRule(LifecycleState.Running, LifecycleState.Suspended),
                new LifecycleTransitionRule(LifecycleState.Running, LifecycleState.Stopping),
                new LifecycleTransitionRule(LifecycleState.Running, LifecycleState.Failed),

                new LifecycleTransitionRule(LifecycleState.Restricted, LifecycleState.Recovering),
                new LifecycleTransitionRule(LifecycleState.Restricted, LifecycleState.Suspended),
                new LifecycleTransitionRule(LifecycleState.Restricted, LifecycleState.Stopping),
                new LifecycleTransitionRule(LifecycleState.Restricted, LifecycleState.Failed),

                new LifecycleTransitionRule(LifecycleState.Suspended, LifecycleState.Recovering),
                new LifecycleTransitionRule(LifecycleState.Suspended, LifecycleState.Stopping),
                new LifecycleTransitionRule(LifecycleState.Suspended, LifecycleState.Failed),

                new LifecycleTransitionRule(LifecycleState.Stopping, LifecycleState.Stopped),
                new LifecycleTransitionRule(LifecycleState.Stopping, LifecycleState.Failed),

                new LifecycleTransitionRule(LifecycleState.Stopped, LifecycleState.Initializing),
                new LifecycleTransitionRule(LifecycleState.Stopped, LifecycleState.Recovering),
                new LifecycleTransitionRule(LifecycleState.Stopped, LifecycleState.Retired),

                new LifecycleTransitionRule(LifecycleState.Failed, LifecycleState.Recovering),
                new LifecycleTransitionRule(LifecycleState.Failed, LifecycleState.Stopping),
                new LifecycleTransitionRule(LifecycleState.Failed, LifecycleState.Retired),

                new LifecycleTransitionRule(LifecycleState.Recovering, LifecycleState.Ready),
                new LifecycleTransitionRule(LifecycleState.Recovering, LifecycleState.Restricted),
                new LifecycleTransitionRule(LifecycleState.Recovering, LifecycleState.Stopping),
                new LifecycleTransitionRule(LifecycleState.Recovering, LifecycleState.Failed)
            });
}

public sealed record LifecycleStateSnapshot(
    string SubjectId,
    string ModelId,
    string ModelVersion,
    LifecycleState State,
    long StateVersion,
    string BootstrapContextId,
    DateTimeOffset BootstrapValidUntil,
    bool ProtectiveRestrictionActive,
    string ActiveRestrictionId,
    int RestartAttempts,
    long AcceptedTransitionCount,
    string LastTransitionId,
    DateTimeOffset EffectiveTime,
    string EvidenceReference);

public sealed record LifecycleRegistrationResult(
    bool Accepted,
    string ReasonCode,
    LifecycleStateSnapshot? Snapshot);

internal sealed record LifecycleIdentityReservation(
    bool Accepted,
    string ReasonCode);

internal sealed record LifecycleTransitionCommand(
    string RequestId,
    string TransitionId,
    string EventId,
    string SubjectId,
    string ModelId,
    string ModelVersion,
    LifecycleState SourceState,
    LifecycleState TargetState,
    long ExpectedStateVersion,
    string Requester,
    string AuthorityReference,
    string Reason,
    string DependencyContext,
    string BootstrapContextId,
    string ValidationEvidence,
    string RestrictionId,
    DateTimeOffset RequestTime,
    DateTimeOffset Expiry,
    DateTimeOffset ObservationTime,
    bool AuthorityAccepted,
    bool TrustedTime,
    bool BootstrapAccepted,
    bool DependenciesReady,
    bool RestrictionActive,
    bool ProtectiveTransition,
    bool RestrictionReleaseValidated,
    bool RecoveryValidated);

public sealed record LifecycleTransitionEvent(
    string EventId,
    string TransitionId,
    string RequestId,
    string SubjectId,
    LifecycleState SourceState,
    LifecycleState TargetState,
    long PriorStateVersion,
    long ResultingStateVersion,
    DateTimeOffset EffectiveTime,
    string AuthorityReference,
    string ValidationEvidence);

public sealed record LifecycleTransitionAttempt(
    string AttemptId,
    string RequestId,
    string TransitionId,
    string SubjectId,
    LifecycleAttemptDecision Decision,
    string ReasonCode,
    LifecycleState? ClaimedSourceState,
    LifecycleState? ClaimedTargetState,
    long ExpectedStateVersion,
    LifecycleState? AuthoritativeState,
    long AuthoritativeStateVersion,
    DateTimeOffset ObservationTime,
    string EvidenceReference);

internal sealed record LifecycleTransitionOutcome(
    LifecycleAttemptDecision Decision,
    string ReasonCode,
    LifecycleStateSnapshot? Snapshot,
    LifecycleTransitionAttempt Attempt,
    LifecycleTransitionEvent? Event);

internal sealed class LifecycleController
{
    private sealed class SubjectState
    {
        public SubjectState(
            string subjectId,
            LifecycleStateModel model,
            string bootstrapContextId,
            DateTimeOffset bootstrapValidUntil,
            bool protectiveRestrictionActive,
            string activeRestrictionId,
            string evidenceReference,
            DateTimeOffset effectiveTime)
        {
            SubjectId = subjectId;
            Model = model;
            State = LifecycleState.Registered;
            StateVersion = 1;
            BootstrapContextId = bootstrapContextId;
            BootstrapValidUntil = bootstrapValidUntil;
            ProtectiveRestrictionActive = protectiveRestrictionActive;
            ActiveRestrictionId = activeRestrictionId;
            RestartAttempts = 0;
            AcceptedTransitionCount = 0;
            LastTransitionId = string.Empty;
            EffectiveTime = effectiveTime;
            EvidenceReference = evidenceReference;
        }

        public string SubjectId { get; }

        public LifecycleStateModel Model { get; }

        public LifecycleState State { get; set; }

        public long StateVersion { get; set; }

        public string BootstrapContextId { get; set; }

        public DateTimeOffset BootstrapValidUntil { get; }

        public bool ProtectiveRestrictionActive { get; set; }

        public string ActiveRestrictionId { get; set; }

        public int RestartAttempts { get; set; }

        public long AcceptedTransitionCount { get; set; }

        public string LastTransitionId { get; set; }

        public DateTimeOffset EffectiveTime { get; set; }

        public string EvidenceReference { get; set; }
    }

    private readonly object _sync = new();
    private readonly Dictionary<string, SubjectState> _subjects = new(StringComparer.Ordinal);
    private readonly HashSet<string> _observedRequestIds = new(StringComparer.Ordinal);
    private readonly HashSet<string> _observedTransitionIds = new(StringComparer.Ordinal);
    private readonly HashSet<string> _observedEventIds = new(StringComparer.Ordinal);
    private readonly List<LifecycleTransitionAttempt> _attempts = new();
    private readonly List<LifecycleTransitionEvent> _events = new();

    public LifecycleIdentityReservation ReserveIdentities(
        string requestId,
        string transitionId,
        string eventId)
    {
        lock (_sync)
        {
            var duplicateRequest = !string.IsNullOrWhiteSpace(requestId) &&
                !_observedRequestIds.Add(requestId);
            var duplicateTransition = !string.IsNullOrWhiteSpace(transitionId) &&
                !_observedTransitionIds.Add(transitionId);
            var duplicateEvent = !string.IsNullOrWhiteSpace(eventId) &&
                !_observedEventIds.Add(eventId);

            if (duplicateRequest)
            {
                return new LifecycleIdentityReservation(false, "DUPLICATE_REQUEST_ID");
            }

            if (duplicateTransition)
            {
                return new LifecycleIdentityReservation(false, "DUPLICATE_TRANSITION_ID");
            }

            if (duplicateEvent)
            {
                return new LifecycleIdentityReservation(false, "DUPLICATE_EVENT_ID");
            }

            return new LifecycleIdentityReservation(true, "IDENTITIES_RESERVED");
        }
    }

    public LifecycleRegistrationResult RegisterSubject(
        string subjectId,
        LifecycleStateModel model,
        string bootstrapContextId,
        DateTimeOffset bootstrapValidUntil,
        bool protectiveRestrictionActive,
        string activeRestrictionId,
        string evidenceReference,
        DateTimeOffset effectiveTime)
    {
        ArgumentNullException.ThrowIfNull(model);

        lock (_sync)
        {
            if (string.IsNullOrWhiteSpace(subjectId))
            {
                return new LifecycleRegistrationResult(false, "MISSING_SUBJECT_IDENTITY", null);
            }

            if (string.IsNullOrWhiteSpace(bootstrapContextId))
            {
                return new LifecycleRegistrationResult(false, "MISSING_BOOTSTRAP_CONTEXT", null);
            }

            if (bootstrapValidUntil <= effectiveTime)
            {
                return new LifecycleRegistrationResult(false, "INVALID_BOOTSTRAP_VALIDITY", null);
            }

            if (protectiveRestrictionActive && string.IsNullOrWhiteSpace(activeRestrictionId))
            {
                return new LifecycleRegistrationResult(false, "MISSING_ACTIVE_RESTRICTION_ID", null);
            }

            if (!protectiveRestrictionActive && !string.IsNullOrWhiteSpace(activeRestrictionId))
            {
                return new LifecycleRegistrationResult(false, "UNEXPECTED_ACTIVE_RESTRICTION_ID", null);
            }

            if (string.IsNullOrWhiteSpace(evidenceReference))
            {
                return new LifecycleRegistrationResult(false, "MISSING_REGISTRATION_EVIDENCE", null);
            }

            if (effectiveTime == default)
            {
                return new LifecycleRegistrationResult(false, "INVALID_REGISTRATION_TIME", null);
            }

            if (_subjects.ContainsKey(subjectId))
            {
                return new LifecycleRegistrationResult(
                    false,
                    "SUBJECT_ALREADY_REGISTERED",
                    Snapshot(_subjects[subjectId]));
            }

            var state = new SubjectState(
                subjectId,
                model,
                bootstrapContextId,
                bootstrapValidUntil,
                protectiveRestrictionActive,
                activeRestrictionId,
                evidenceReference,
                effectiveTime);
            _subjects.Add(subjectId, state);

            return new LifecycleRegistrationResult(true, "SUBJECT_REGISTERED", Snapshot(state));
        }
    }

    public LifecycleStateSnapshot? GetSnapshot(string subjectId)
    {
        lock (_sync)
        {
            return _subjects.TryGetValue(subjectId, out var state)
                ? Snapshot(state)
                : null;
        }
    }

    public ReadOnlyCollection<LifecycleTransitionAttempt> GetAttempts()
    {
        lock (_sync)
        {
            return Array.AsReadOnly(_attempts.ToArray());
        }
    }

    public ReadOnlyCollection<LifecycleTransitionEvent> GetEvents()
    {
        lock (_sync)
        {
            return Array.AsReadOnly(_events.ToArray());
        }
    }

    public LifecycleTransitionOutcome ApplyTransition(LifecycleTransitionCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        lock (_sync)
        {
            if (!_subjects.TryGetValue(command.SubjectId, out var state))
            {
                return Reject(command, "UNKNOWN_SUBJECT");
            }

            if (string.IsNullOrWhiteSpace(command.RequestId) ||
                string.IsNullOrWhiteSpace(command.TransitionId) ||
                string.IsNullOrWhiteSpace(command.EventId) ||
                string.IsNullOrWhiteSpace(command.Requester) ||
                string.IsNullOrWhiteSpace(command.AuthorityReference) ||
                string.IsNullOrWhiteSpace(command.Reason) ||
                string.IsNullOrWhiteSpace(command.DependencyContext) ||
                string.IsNullOrWhiteSpace(command.ValidationEvidence))
            {
                return Reject(command, "INCOMPLETE_TRANSITION_REQUEST");
            }

            if (!string.Equals(state.Model.ModelId, command.ModelId, StringComparison.Ordinal) ||
                !string.Equals(state.Model.Version, command.ModelVersion, StringComparison.Ordinal))
            {
                return Reject(command, "LIFECYCLE_MODEL_MISMATCH");
            }

            if (state.State == LifecycleState.Retired)
            {
                return Reject(command, "RETIRED_STATE_IS_TERMINAL");
            }

            if (state.State != command.SourceState)
            {
                return Reject(command, "STALE_SOURCE_STATE");
            }

            if (state.StateVersion != command.ExpectedStateVersion)
            {
                return Reject(command, "STALE_STATE_VERSION");
            }

            if (command.SourceState == command.TargetState)
            {
                return Reject(command, "NO_STATE_CHANGE");
            }

            if (!command.TrustedTime ||
                command.RequestTime == default ||
                command.Expiry <= command.RequestTime ||
                command.ObservationTime < command.RequestTime ||
                command.ObservationTime >= command.Expiry)
            {
                return Reject(command, "INVALID_OR_UNTRUSTED_TIME");
            }

            if (!command.AuthorityAccepted)
            {
                return Reject(command, "AUTHORITY_REJECTED");
            }

            if (!state.Model.Allows(command.SourceState, command.TargetState))
            {
                return Reject(command, "INVALID_LIFECYCLE_TRANSITION");
            }

            var bootstrapEntryTransition =
                command.TargetState == LifecycleState.Initializing ||
                (command.SourceState == LifecycleState.Registered &&
                 command.TargetState == LifecycleState.Restricted);

            if (bootstrapEntryTransition)
            {
                if (!command.BootstrapAccepted || string.IsNullOrWhiteSpace(command.BootstrapContextId))
                {
                    return Reject(command, "BOOTSTRAP_NOT_ACCEPTED");
                }

                if (command.ObservationTime >= state.BootstrapValidUntil)
                {
                    return Reject(command, "BOOTSTRAP_EVIDENCE_EXPIRED");
                }

                if (!string.Equals(
                        state.BootstrapContextId,
                        command.BootstrapContextId,
                        StringComparison.Ordinal))
                {
                    return Reject(command, "BOOTSTRAP_CONTEXT_MISMATCH");
                }
            }

            if (!string.IsNullOrWhiteSpace(command.BootstrapContextId) &&
                !string.Equals(
                    state.BootstrapContextId,
                    command.BootstrapContextId,
                    StringComparison.Ordinal))
            {
                return Reject(command, "BOOTSTRAP_CONTEXT_MISMATCH");
            }

            var controllingRestrictionActive =
                state.ProtectiveRestrictionActive || command.RestrictionActive;
            var controlledReleaseTransition =
                state.ProtectiveRestrictionActive &&
                command.TargetState == LifecycleState.Recovering;

            if (command.RestrictionActive && string.IsNullOrWhiteSpace(command.RestrictionId))
            {
                return Reject(command, "MISSING_RESTRICTION_ID");
            }

            if (state.ProtectiveRestrictionActive &&
                !string.IsNullOrWhiteSpace(command.RestrictionId) &&
                !string.Equals(
                    state.ActiveRestrictionId,
                    command.RestrictionId,
                    StringComparison.Ordinal))
            {
                return Reject(command, "ACTIVE_RESTRICTION_ID_MISMATCH");
            }

            if (controlledReleaseTransition)
            {
                if (!command.RestrictionReleaseValidated)
                {
                    return Reject(command, "RESTRICTION_RELEASE_NOT_VALIDATED");
                }
            }
            else if (controllingRestrictionActive)
            {
                if (!command.ProtectiveTransition)
                {
                    return Reject(command, "ACTIVE_RESTRICTION_REQUIRES_PROTECTIVE_TRANSITION");
                }

                if (!IsProtectiveTarget(command.TargetState))
                {
                    return Reject(command, "ACTIVE_RESTRICTION_BLOCKS_TARGET_STATE");
                }
            }

            if (command.SourceState == LifecycleState.Recovering &&
                command.TargetState == LifecycleState.Ready &&
                !command.RecoveryValidated)
            {
                return Reject(command, "RECOVERY_NOT_INDEPENDENTLY_VALIDATED");
            }

            if (command.TargetState == LifecycleState.Running &&
                (!command.DependenciesReady || string.IsNullOrWhiteSpace(state.BootstrapContextId)))
            {
                return Reject(command, "RUNNING_PREREQUISITES_NOT_SATISFIED");
            }

            var isRestartAttempt =
                (command.SourceState == LifecycleState.Stopped &&
                 command.TargetState is LifecycleState.Initializing or LifecycleState.Recovering) ||
                (command.SourceState == LifecycleState.Failed &&
                 command.TargetState == LifecycleState.Recovering);

            if (isRestartAttempt && state.RestartAttempts >= state.Model.MaxRestartAttempts)
            {
                return Reject(command, "RESTART_LIMIT_EXCEEDED");
            }

            var priorState = state.State;
            var priorVersion = state.StateVersion;

            state.State = command.TargetState;
            state.StateVersion++;
            state.AcceptedTransitionCount++;
            state.LastTransitionId = command.TransitionId;
            state.EffectiveTime = command.ObservationTime;
            state.EvidenceReference = command.ValidationEvidence;

            if (isRestartAttempt)
            {
                state.RestartAttempts++;
            }

            if (controlledReleaseTransition)
            {
                state.ProtectiveRestrictionActive = false;
                state.ActiveRestrictionId = string.Empty;
            }
            else if (command.RestrictionActive)
            {
                state.ProtectiveRestrictionActive = true;
                state.ActiveRestrictionId = command.RestrictionId;
            }

            var transitionEvent = new LifecycleTransitionEvent(
                command.EventId,
                command.TransitionId,
                command.RequestId,
                command.SubjectId,
                priorState,
                command.TargetState,
                priorVersion,
                state.StateVersion,
                command.ObservationTime,
                command.AuthorityReference,
                command.ValidationEvidence);

            _events.Add(transitionEvent);

            var attempt = new LifecycleTransitionAttempt(
                NextAttemptId(command.RequestId),
                command.RequestId,
                command.TransitionId,
                command.SubjectId,
                LifecycleAttemptDecision.Accepted,
                "TRANSITION_ACCEPTED",
                command.SourceState,
                command.TargetState,
                command.ExpectedStateVersion,
                state.State,
                state.StateVersion,
                command.ObservationTime,
                command.ValidationEvidence);

            _attempts.Add(attempt);

            return new LifecycleTransitionOutcome(
                LifecycleAttemptDecision.Accepted,
                "TRANSITION_ACCEPTED",
                Snapshot(state),
                attempt,
                transitionEvent);
        }
    }

    private LifecycleTransitionOutcome Reject(
        LifecycleTransitionCommand command,
        string reasonCode)
    {
        _subjects.TryGetValue(command.SubjectId, out var state);

        var attempt = new LifecycleTransitionAttempt(
            NextAttemptId(command.RequestId),
            command.RequestId,
            command.TransitionId,
            command.SubjectId,
            LifecycleAttemptDecision.Rejected,
            reasonCode,
            command.SourceState,
            command.TargetState,
            command.ExpectedStateVersion,
            state?.State,
            state?.StateVersion ?? 0,
            command.ObservationTime,
            command.ValidationEvidence);

        _attempts.Add(attempt);

        return new LifecycleTransitionOutcome(
            LifecycleAttemptDecision.Rejected,
            reasonCode,
            state is null ? null : Snapshot(state),
            attempt,
            null);
    }

    private string NextAttemptId(string requestId)
    {
        var normalized = string.IsNullOrWhiteSpace(requestId)
            ? "UNIDENTIFIED"
            : requestId;

        return $"ATTEMPT:{normalized}:{_attempts.Count + 1:D4}";
    }

    private static bool IsProtectiveTarget(LifecycleState state)
        => state is LifecycleState.Restricted
            or LifecycleState.Suspended
            or LifecycleState.Stopping
            or LifecycleState.Stopped
            or LifecycleState.Failed;

    private static LifecycleStateSnapshot Snapshot(SubjectState state)
        => new(
            state.SubjectId,
            state.Model.ModelId,
            state.Model.Version,
            state.State,
            state.StateVersion,
            state.BootstrapContextId,
            state.BootstrapValidUntil,
            state.ProtectiveRestrictionActive,
            state.ActiveRestrictionId,
            state.RestartAttempts,
            state.AcceptedTransitionCount,
            state.LastTransitionId,
            state.EffectiveTime,
            state.EvidenceReference);
}
