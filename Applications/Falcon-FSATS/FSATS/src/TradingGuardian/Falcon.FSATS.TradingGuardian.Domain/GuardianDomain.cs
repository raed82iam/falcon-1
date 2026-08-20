namespace Falcon.FSATS.TradingGuardian.Domain;

public enum IncidentClass { Anomaly, Degraded, ProtectionIncident, IntegrityIncident, Unknown }
public enum GuardianMode { Normal, Warning, Restricted, SafeMode, Recovery }
public enum SafetyAction { DenyExpansion, FreezeNewRisk, PreserveProtection, Reconcile, Reduce, Exit }

public sealed record ProtectionSignal(string Source, string Type, int Severity, bool Trusted, DateTimeOffset ObservedAt);
public sealed record IncidentQualification(IncidentClass Classification, int Severity, string ReasonCode);
public sealed record GuardianRecoveryEvidence(
    string EvidenceReference,
    long RecoveryEpoch,
    DateTimeOffset ObservedAt,
    bool IntegrityTrusted,
    bool ProtectionVerified,
    bool ExecutionTruthKnown);

public sealed class IncidentClassifier
{
    public static readonly TimeSpan DefaultMaximumSignalAge = TimeSpan.FromMinutes(2);

    public IncidentQualification Classify(IReadOnlyCollection<ProtectionSignal> signals)
        => Classify(signals, DateTimeOffset.UtcNow, DefaultMaximumSignalAge);

    public IncidentQualification Classify(IReadOnlyCollection<ProtectionSignal> signals, DateTimeOffset now, TimeSpan maximumSignalAge)
    {
        ArgumentNullException.ThrowIfNull(signals);
        if (now == default || maximumSignalAge < TimeSpan.Zero) return new(IncidentClass.IntegrityIncident, 100, "INVALID_GUARDIAN_TIME_POLICY");
        if (signals.Count == 0) return new(IncidentClass.Unknown, 100, "NO_EVIDENCE");
        if (signals.Any(x => x is null || !x.Trusted)) return new(IncidentClass.IntegrityIncident, 100, "UNTRUSTED_SIGNAL");
        if (signals.Any(x => x.Severity is < 0 or > 100)) return new(IncidentClass.IntegrityIncident, 100, "INVALID_SIGNAL_SEVERITY");
        if (signals.Any(x => x.ObservedAt == default || x.ObservedAt > now || now - x.ObservedAt > maximumSignalAge))
            return new(IncidentClass.IntegrityIncident, 100, "STALE_OR_CLOCK_INVALID_SIGNAL");

        var max = signals.Max(x => x.Severity);
        return max >= 90 ? new(IncidentClass.ProtectionIncident, max, "MATERIAL_PROTECTION_EVENT")
            : max >= 50 ? new(IncidentClass.Degraded, max, "DEGRADED")
            : new(IncidentClass.Anomaly, max, "OBSERVED_ANOMALY");
    }
}

public sealed class CrisisStateMachine
{
    public static readonly TimeSpan DefaultMaximumRecoveryEvidenceAge = TimeSpan.FromMinutes(2);
    public GuardianMode Mode { get; private set; } = GuardianMode.Normal;
    public long? ActiveRecoveryEpoch { get; private set; }

    public void Apply(IncidentQualification incident)
    {
        ArgumentNullException.ThrowIfNull(incident);
        var proposed = incident.Classification switch
        {
            IncidentClass.IntegrityIncident => GuardianMode.SafeMode,
            IncidentClass.ProtectionIncident => GuardianMode.SafeMode,
            IncidentClass.Degraded => GuardianMode.Restricted,
            IncidentClass.Anomaly => GuardianMode.Warning,
            _ => GuardianMode.Restricted
        };

        if (Mode == GuardianMode.Recovery)
        {
            if (ProtectionRank(proposed) >= ProtectionRank(GuardianMode.Restricted))
            {
                Mode = proposed;
                ActiveRecoveryEpoch = null;
            }
            return;
        }

        if (ProtectionRank(proposed) > ProtectionRank(Mode)) Mode = proposed;
    }

    public void BeginRecovery()
        => throw new InvalidOperationException("GOVERNED_RECOVERY_EVIDENCE_REQUIRED");

    public bool BeginRecovery(
        GuardianRecoveryEvidence evidence,
        long currentRecoveryEpoch,
        DateTimeOffset now,
        TimeSpan? maximumEvidenceAge = null)
    {
        ArgumentNullException.ThrowIfNull(evidence);
        var maxAge = maximumEvidenceAge ?? DefaultMaximumRecoveryEvidenceAge;
        if (Mode == GuardianMode.Normal || Mode == GuardianMode.Recovery) return false;
        if (!IsRecoveryEvidenceCurrent(evidence, currentRecoveryEpoch, now, maxAge)) return false;
        Mode = GuardianMode.Recovery;
        ActiveRecoveryEpoch = currentRecoveryEpoch;
        return true;
    }

    public bool CompleteRecovery(
        GuardianRecoveryEvidence evidence,
        long currentRecoveryEpoch,
        DateTimeOffset now,
        TimeSpan? maximumEvidenceAge = null)
    {
        ArgumentNullException.ThrowIfNull(evidence);
        var maxAge = maximumEvidenceAge ?? DefaultMaximumRecoveryEvidenceAge;
        if (Mode != GuardianMode.Recovery || ActiveRecoveryEpoch != currentRecoveryEpoch) return false;
        if (!IsRecoveryEvidenceCurrent(evidence, currentRecoveryEpoch, now, maxAge)) return false;
        Mode = GuardianMode.Normal;
        ActiveRecoveryEpoch = null;
        return true;
    }

    public static bool IsRecoveryEvidenceCurrent(
        GuardianRecoveryEvidence evidence,
        long currentRecoveryEpoch,
        DateTimeOffset now,
        TimeSpan maximumEvidenceAge)
        => evidence is not null &&
           !string.IsNullOrWhiteSpace(evidence.EvidenceReference) &&
           evidence.RecoveryEpoch == currentRecoveryEpoch &&
           evidence.IntegrityTrusted &&
           evidence.ProtectionVerified &&
           evidence.ExecutionTruthKnown &&
           evidence.ObservedAt != default &&
           now != default &&
           maximumEvidenceAge >= TimeSpan.Zero &&
           evidence.ObservedAt <= now &&
           now - evidence.ObservedAt <= maximumEvidenceAge;

    private static int ProtectionRank(GuardianMode mode) => mode switch
    {
        GuardianMode.Normal => 0,
        GuardianMode.Warning => 1,
        GuardianMode.Restricted => 2,
        GuardianMode.SafeMode => 3,
        GuardianMode.Recovery => 0,
        _ => 3
    };
}

public sealed record SafetyContext(bool IntelligenceTrusted, bool ExecutionTruthKnown, bool ProtectionVerified, bool ExposureExists, bool ExitPolicyAuthorized);

public static class DeterministicSafetyKernel
{
    public static IReadOnlyList<SafetyAction> Decide(SafetyContext context)
    {
        var actions = new List<SafetyAction> { SafetyAction.DenyExpansion, SafetyAction.FreezeNewRisk };
        if (!context.ExecutionTruthKnown) { actions.Add(SafetyAction.Reconcile); return actions; }
        if (context.ProtectionVerified) actions.Add(SafetyAction.PreserveProtection);
        if (context.ExposureExists && !context.ProtectionVerified)
        {
            actions.Add(SafetyAction.Reconcile);
            actions.Add(context.ExitPolicyAuthorized ? SafetyAction.Exit : SafetyAction.Reduce);
        }
        return actions;
    }
}

public static class CommandLease
{
    public static bool IsCurrent(long commandEpoch, long currentEpoch, DateTimeOffset? expiresAt, DateTimeOffset now)
        => commandEpoch == currentEpoch && (expiresAt is null || expiresAt.Value > now);
}
