using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Falcon.FSATS.Trading.Domain;

namespace Falcon.FSATS.Trading.Application;

public static class TradingDurableStateContract
{
    public const string Owner = "FSATS.TRADING";
    public const string SchemaVersion = "P3.TRADING.DURABLE.v1";
}

public sealed record DurableExecutionRecord(QueuedExecutionWork Work, ExecutionQueueState CapturedState, string ReasonCode, string EvidenceReference, string? ContainmentIncidentId, long Generation, DateTimeOffset CapturedAt);
public sealed record DurableAccountContainment(BrokerAccountContext Account, string IncidentId, string ReasonCode, string EvidenceReference, DateTimeOffset ObservedAt);
public sealed record DurableBrokerContainment(string BrokerId, string Environment, string IncidentId, string ReasonCode, string EvidenceReference, DateTimeOffset ObservedAt, IReadOnlyList<BrokerAccountContext> AffectedAccounts);
public sealed record DurableReconciliationObligation(BrokerExecutionIdentity Identity, BrokerSubmissionTruth SubmissionTruth, string ReasonCode, string EvidenceReference, DateTimeOffset ObservedAt);
public sealed record DurableCapitalReservation(BrokerAccountContext Account, ReservationId ReservationId, Money Amount, string EvidenceReference, DateTimeOffset ObservedAt);

public sealed record TradingDurableSnapshot(
    string Owner,
    string SchemaVersion,
    long SnapshotGeneration,
    DateTimeOffset CapturedAt,
    IReadOnlyList<DurableExecutionRecord> ExecutionRecords,
    IReadOnlyList<DurableAccountContainment> AccountContainments,
    IReadOnlyList<DurableBrokerContainment> BrokerContainments,
    IReadOnlyList<DurableReconciliationObligation> ReconciliationObligations,
    IReadOnlyList<DurableCapitalReservation> CapitalReservations,
    string PayloadSha256)
{
    public static TradingDurableSnapshot Create(
        long generation,
        DateTimeOffset capturedAt,
        IEnumerable<DurableExecutionRecord> executionRecords,
        IEnumerable<DurableAccountContainment>? accountContainments = null,
        IEnumerable<DurableBrokerContainment>? brokerContainments = null,
        IEnumerable<DurableReconciliationObligation>? reconciliationObligations = null,
        IEnumerable<DurableCapitalReservation>? capitalReservations = null)
    {
        if (generation < 0) throw new ArgumentOutOfRangeException(nameof(generation));
        if (capturedAt == default) throw new ArgumentException("DURABLE_CAPTURE_TIME_REQUIRED", nameof(capturedAt));
        var snapshot = new TradingDurableSnapshot(
            TradingDurableStateContract.Owner,
            TradingDurableStateContract.SchemaVersion,
            generation,
            capturedAt,
            Array.AsReadOnly((executionRecords ?? throw new ArgumentNullException(nameof(executionRecords))).ToArray()),
            Array.AsReadOnly((accountContainments ?? Array.Empty<DurableAccountContainment>()).ToArray()),
            Array.AsReadOnly((brokerContainments ?? Array.Empty<DurableBrokerContainment>()).ToArray()),
            Array.AsReadOnly((reconciliationObligations ?? Array.Empty<DurableReconciliationObligation>()).ToArray()),
            Array.AsReadOnly((capitalReservations ?? Array.Empty<DurableCapitalReservation>()).ToArray()),
            string.Empty);
        return snapshot with { PayloadSha256 = TradingDurableIntegrity.Compute(snapshot) };
    }
}

public enum RestartExecutionDisposition { QueueEligible, CancelledTombstone, ReconciliationRequired, TerminalIdentityFence }
public sealed record RestartExecutionRecord(DurableExecutionRecord DurableRecord, RestartExecutionDisposition Disposition, string ReasonCode);
public sealed record DurableExecutionIdentityTombstone(string ExecutionIdentityKey, string WorkId, string EvidenceReference, DateTimeOffset CompactedAt, string TombstoneSha256);

public sealed record TradingRestartPlan(
    bool Accepted,
    string ReasonCode,
    IReadOnlyList<RestartExecutionRecord> Execution,
    IReadOnlySet<BrokerAccountContext> ContainedAccounts,
    IReadOnlySet<string> ReservedExecutionIdentityKeys,
    IReadOnlyList<DurableReconciliationObligation> ReconciliationObligations,
    IReadOnlyList<DurableCapitalReservation> CapitalReservations)
{
    public bool AllowsNewRisk(BrokerAccountContext account)
        => Accepted &&
           !ContainedAccounts.Contains(account) &&
           !ReconciliationObligations.Any(x => x.Identity.Account == account && x.SubmissionTruth != BrokerSubmissionTruth.NotSubmitted) &&
           !CapitalReservations.Any(x => x.Account == account) &&
           !Execution.Any(x => x.DurableRecord.Work.Intent.ExecutionIdentity.Account == account && x.Disposition == RestartExecutionDisposition.ReconciliationRequired);
}

public interface ITradingDurableStatePort
{
    ValueTask<TradingDurableSnapshot?> LoadAsync(CancellationToken cancellationToken);
    ValueTask SaveAsync(TradingDurableSnapshot snapshot, CancellationToken cancellationToken);
}

public static class TradingRestartReconstructor
{
    public static TradingRestartPlan Reconstruct(TradingDurableSnapshot? snapshot, DateTimeOffset now)
    {
        if (snapshot is null) return Reject("TRADING_DURABLE_STATE_MISSING");
        if (now == default || snapshot.CapturedAt == default || snapshot.CapturedAt > now) return Reject("TRADING_DURABLE_TIME_INVALID");
        if (!StringComparer.Ordinal.Equals(snapshot.Owner, TradingDurableStateContract.Owner)) return Reject("TRADING_DURABLE_OWNER_MISMATCH");
        if (!StringComparer.Ordinal.Equals(snapshot.SchemaVersion, TradingDurableStateContract.SchemaVersion)) return Reject("TRADING_DURABLE_SCHEMA_UNSUPPORTED");
        if (snapshot.SnapshotGeneration < 0 || !TradingDurableIntegrity.Verify(snapshot)) return Reject("TRADING_DURABLE_INTEGRITY_INVALID");

        var records = snapshot.ExecutionRecords ?? Array.Empty<DurableExecutionRecord>();
        if (records.Any(x => !ValidExecutionRecord(x, snapshot.CapturedAt))) return Reject("TRADING_DURABLE_EXECUTION_RECORD_INVALID");
        if (records.GroupBy(x => (x.Work.Intent.ExecutionIdentity.Account, x.Work.WorkId)).Any(g => g.Count() != 1)) return Reject("TRADING_DURABLE_DUPLICATE_WORK_IDENTITY");
        if (records.GroupBy(x => x.Work.Intent.ExecutionIdentity).Any(g => g.Count() != 1)) return Reject("TRADING_DURABLE_DUPLICATE_EXECUTION_IDENTITY");

        var accountContainments = snapshot.AccountContainments ?? Array.Empty<DurableAccountContainment>();
        var brokerContainments = snapshot.BrokerContainments ?? Array.Empty<DurableBrokerContainment>();
        if (!ValidateContainments(accountContainments, brokerContainments, snapshot.CapturedAt)) return Reject("TRADING_DURABLE_CONTAINMENT_INVALID");

        var contained = new HashSet<BrokerAccountContext>(accountContainments.Select(x => x.Account));
        foreach (var broker in brokerContainments)
        {
            var brokerId = broker.BrokerId.Trim().ToUpperInvariant();
            var environment = broker.Environment.Trim().ToUpperInvariant();
            foreach (var account in broker.AffectedAccounts) contained.Add(account);
            foreach (var account in records.Select(x => x.Work.Intent.ExecutionIdentity.Account).Where(x => x.BrokerId == brokerId && x.Environment == environment)) contained.Add(account);
        }

        var reconciliation = new List<DurableReconciliationObligation>(snapshot.ReconciliationObligations ?? Array.Empty<DurableReconciliationObligation>());
        if (reconciliation.Any(x => x is null || x.Identity is null || !Enum.IsDefined(x.SubmissionTruth) || x.ObservedAt == default || x.ObservedAt > snapshot.CapturedAt || string.IsNullOrWhiteSpace(x.EvidenceReference) || string.IsNullOrWhiteSpace(x.ReasonCode))) return Reject("TRADING_DURABLE_RECONCILIATION_INVALID");
        if (reconciliation.GroupBy(x => x.Identity).Any(g => g.Count() != 1)) return Reject("TRADING_DURABLE_RECONCILIATION_DUPLICATE");

        var output = new List<RestartExecutionRecord>(records.Count);
        var reserved = new HashSet<string>(StringComparer.Ordinal);
        foreach (var record in records)
        {
            var identity = record.Work.Intent.ExecutionIdentity;
            var isContained = contained.Contains(identity.Account);
            var (disposition, reason) = record.CapturedState switch
            {
                ExecutionQueueState.Queued => isContained
                    ? (RestartExecutionDisposition.CancelledTombstone, "RESTART_QUEUED_WORK_CANCELLED_BY_PERSISTED_CONTAINMENT")
                    : (RestartExecutionDisposition.QueueEligible, "RESTART_QUEUED_WORK_RESTORED_PENDING"),
                ExecutionQueueState.Leased => isContained
                    ? (RestartExecutionDisposition.CancelledTombstone, "RESTART_LEASED_WORK_CANCELLED_BY_PERSISTED_CONTAINMENT")
                    : (RestartExecutionDisposition.QueueEligible, "PRE_RESTART_LEASE_INVALIDATED_REQUEUED"),
                ExecutionQueueState.DispatchStarted or ExecutionQueueState.ReconciliationRequired
                    => (RestartExecutionDisposition.ReconciliationRequired, "PRE_RESTART_EXTERNAL_DISPATCH_REQUIRES_RECONCILIATION"),
                ExecutionQueueState.CancelledByContainment
                    => (RestartExecutionDisposition.CancelledTombstone, "CANCELLED_IDENTITY_REMAINS_NON_RESURRECTABLE_AFTER_RESTART"),
                ExecutionQueueState.Completed
                    => (RestartExecutionDisposition.TerminalIdentityFence, "COMPLETED_IDENTITY_REMAINS_RESERVED_AFTER_RESTART"),
                _ => throw new InvalidOperationException("UNREACHABLE_EXECUTION_QUEUE_STATE")
            };

            if (disposition == RestartExecutionDisposition.ReconciliationRequired && !reconciliation.Any(x => x.Identity == identity))
                reconciliation.Add(new(identity, BrokerSubmissionTruth.SubmittedOutcomeUnknown, reason, record.EvidenceReference, snapshot.CapturedAt));
            if (disposition != RestartExecutionDisposition.QueueEligible) reserved.Add(identity.NamespaceKey);
            output.Add(new(record, disposition, reason));
        }

        var reservations = snapshot.CapitalReservations ?? Array.Empty<DurableCapitalReservation>();
        if (reservations.Any(x => x is null || x.Account is null || string.IsNullOrWhiteSpace(x.ReservationId.Value) || x.Amount.Amount <= 0m || string.IsNullOrWhiteSpace(x.Amount.Currency.Code) || x.ObservedAt == default || x.ObservedAt > snapshot.CapturedAt || string.IsNullOrWhiteSpace(x.EvidenceReference))) return Reject("TRADING_DURABLE_RESERVATION_INVALID");
        if (reservations.GroupBy(x => (x.Account, x.ReservationId)).Any(g => g.Count() != 1)) return Reject("TRADING_DURABLE_RESERVATION_DUPLICATE");

        return new(true, "TRADING_RESTART_RECONSTRUCTION_ACCEPTED", output, contained, reserved, reconciliation, reservations);
    }

    private static bool ValidExecutionRecord(DurableExecutionRecord? x, DateTimeOffset capturedAt)
    {
        if (x is null || !Enum.IsDefined(x.CapturedState) || x.Work is null || x.Work.Intent is null || x.Work.Intent.ExecutionIdentity is null || x.Work.Intent.ExecutionIdentity.Account is null || x.Work.Intent.SafetyEnvelope is null) return false;
        var intent = x.Work.Intent;
        var identity = intent.ExecutionIdentity;
        var safety = intent.SafetyEnvelope;
        return x.CapturedAt != default && x.CapturedAt <= capturedAt && x.Work.EnqueuedAt != default && x.Work.EnqueuedAt <= capturedAt && x.Generation >= 0 &&
               !string.IsNullOrWhiteSpace(x.Work.WorkId) && !string.IsNullOrWhiteSpace(x.Work.EvidenceReference) && !string.IsNullOrWhiteSpace(x.ReasonCode) && !string.IsNullOrWhiteSpace(x.EvidenceReference) &&
               !string.IsNullOrWhiteSpace(identity.Account.BrokerId) && !string.IsNullOrWhiteSpace(identity.Account.BrokerAccountId) && !string.IsNullOrWhiteSpace(identity.Account.Environment) &&
               !string.IsNullOrWhiteSpace(identity.BrokerRouteId) && !string.IsNullOrWhiteSpace(identity.SubmissionId) && !string.IsNullOrWhiteSpace(identity.OrderId.Value) &&
               !string.IsNullOrWhiteSpace(intent.Instrument.Value) && intent.Quantity.Amount > 0m && intent.TrustEpoch.Value >= 0 &&
               !string.IsNullOrWhiteSpace(safety.PositionId.Value) && !string.IsNullOrWhiteSpace(safety.InstrumentId.Value) && safety.Quantity.Amount >= 0m && safety.MaximumAuthorizedLoss.Amount >= 0m &&
               !string.IsNullOrWhiteSpace(safety.MaximumAuthorizedLoss.Currency.Code) && !string.IsNullOrWhiteSpace(safety.ProtectionOwner) && !string.IsNullOrWhiteSpace(safety.ProtectionState) &&
               !string.IsNullOrWhiteSpace(safety.EmergencyExitRule) && !string.IsNullOrWhiteSpace(safety.ReconciliationState) && safety.LastTrustedRiskEpoch.Value >= 0;
    }

    private static bool ValidateContainments(IEnumerable<DurableAccountContainment> accounts, IEnumerable<DurableBrokerContainment> brokers, DateTimeOffset capturedAt)
    {
        foreach (var x in accounts)
            if (x is null || x.Account is null || string.IsNullOrWhiteSpace(x.Account.BrokerId) || string.IsNullOrWhiteSpace(x.Account.BrokerAccountId) || string.IsNullOrWhiteSpace(x.Account.Environment) || string.IsNullOrWhiteSpace(x.IncidentId) || string.IsNullOrWhiteSpace(x.ReasonCode) || string.IsNullOrWhiteSpace(x.EvidenceReference) || x.ObservedAt == default || x.ObservedAt > capturedAt) return false;
        foreach (var x in brokers)
        {
            if (x is null || string.IsNullOrWhiteSpace(x.BrokerId) || string.IsNullOrWhiteSpace(x.Environment) || string.IsNullOrWhiteSpace(x.IncidentId) || string.IsNullOrWhiteSpace(x.ReasonCode) || string.IsNullOrWhiteSpace(x.EvidenceReference) || x.ObservedAt == default || x.ObservedAt > capturedAt || x.AffectedAccounts is null || x.AffectedAccounts.Count == 0) return false;
            var broker = x.BrokerId.Trim().ToUpperInvariant(); var environment = x.Environment.Trim().ToUpperInvariant();
            if (x.AffectedAccounts.Any(a => a is null || a.BrokerId != broker || a.Environment != environment)) return false;
        }
        return true;
    }

    private static TradingRestartPlan Reject(string reason)
        => new(false, reason, Array.Empty<RestartExecutionRecord>(), new HashSet<BrokerAccountContext>(), new HashSet<string>(StringComparer.Ordinal), Array.Empty<DurableReconciliationObligation>(), Array.Empty<DurableCapitalReservation>());
}

public sealed record DurableRetentionPolicy(TimeSpan TerminalIdentityMinimumRetention, int MaximumCompactableTerminalRecords)
{
    public bool IsValid => TerminalIdentityMinimumRetention > TimeSpan.Zero && MaximumCompactableTerminalRecords >= 0;
}

public static class TradingDurableRetention
{
    public static bool IsSafetyCritical(RestartExecutionRecord record)
        => record.Disposition is RestartExecutionDisposition.QueueEligible or RestartExecutionDisposition.CancelledTombstone or RestartExecutionDisposition.ReconciliationRequired;

    public static IReadOnlyList<RestartExecutionRecord> SelectCompactable(IEnumerable<RestartExecutionRecord> records, DurableRetentionPolicy policy, DateTimeOffset now)
    {
        if (records is null) throw new ArgumentNullException(nameof(records));
        if (policy is null || !policy.IsValid || now == default) return Array.Empty<RestartExecutionRecord>();
        return records.Where(x => !IsSafetyCritical(x) && x.Disposition == RestartExecutionDisposition.TerminalIdentityFence && now >= x.DurableRecord.CapturedAt && now - x.DurableRecord.CapturedAt >= policy.TerminalIdentityMinimumRetention)
            .OrderBy(x => x.DurableRecord.CapturedAt).Take(policy.MaximumCompactableTerminalRecords).ToArray();
    }

    public static DurableExecutionIdentityTombstone CompactToIdentityTombstone(RestartExecutionRecord record, DateTimeOffset compactedAt)
    {
        if (record is null || record.Disposition != RestartExecutionDisposition.TerminalIdentityFence || compactedAt == default || compactedAt < record.DurableRecord.CapturedAt)
            throw new InvalidOperationException("ONLY_TERMINAL_EXECUTION_IDENTITY_MAY_COMPACT");
        var identity = record.DurableRecord.Work.Intent.ExecutionIdentity.NamespaceKey;
        var workId = record.DurableRecord.Work.WorkId;
        var evidence = record.DurableRecord.EvidenceReference;
        var value = TradingDurableIntegrity.Pack(identity, workId, evidence, compactedAt.ToUniversalTime().ToString("O"));
        return new(identity, workId, evidence, compactedAt, Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value))));
    }
}

internal static class TradingDurableIntegrity
{
    public static bool Verify(TradingDurableSnapshot snapshot)
    {
        try { return IsSha256(snapshot.PayloadSha256) && StringComparer.Ordinal.Equals(snapshot.PayloadSha256, Compute(snapshot)); }
        catch { return false; }
    }

    public static string Compute(TradingDurableSnapshot snapshot)
    {
        var sb = new StringBuilder();
        Add(sb, snapshot.Owner); Add(sb, snapshot.SchemaVersion); Add(sb, snapshot.SnapshotGeneration.ToString(CultureInfo.InvariantCulture)); Add(sb, snapshot.CapturedAt.ToUniversalTime().ToString("O"));
        foreach (var x in (snapshot.ExecutionRecords ?? Array.Empty<DurableExecutionRecord>()).OrderBy(x => x.Work.Intent.ExecutionIdentity.NamespaceKey, StringComparer.Ordinal))
        {
            Add(sb, x.Work.WorkId); Add(sb, x.Work.EvidenceReference); Add(sb, x.Work.EnqueuedAt.ToUniversalTime().ToString("O"));
            Add(sb, IntentFingerprint(x.Work.Intent)); Add(sb, x.CapturedState.ToString()); Add(sb, x.ReasonCode); Add(sb, x.EvidenceReference); Add(sb, x.ContainmentIncidentId ?? string.Empty); Add(sb, x.Generation.ToString(CultureInfo.InvariantCulture)); Add(sb, x.CapturedAt.ToUniversalTime().ToString("O"));
        }
        foreach (var x in (snapshot.AccountContainments ?? Array.Empty<DurableAccountContainment>()).OrderBy(x => x.Account.NamespaceKey, StringComparer.Ordinal))
            AppendContainment(sb, x.Account.NamespaceKey, x.IncidentId, x.ReasonCode, x.EvidenceReference, x.ObservedAt);
        foreach (var x in (snapshot.BrokerContainments ?? Array.Empty<DurableBrokerContainment>()).OrderBy(x => x.BrokerId, StringComparer.Ordinal).ThenBy(x => x.Environment, StringComparer.Ordinal))
        {
            AppendContainment(sb, x.BrokerId.Trim().ToUpperInvariant() + "|" + x.Environment.Trim().ToUpperInvariant(), x.IncidentId, x.ReasonCode, x.EvidenceReference, x.ObservedAt);
            foreach (var a in x.AffectedAccounts.OrderBy(a => a.NamespaceKey, StringComparer.Ordinal)) Add(sb, a.NamespaceKey);
        }
        foreach (var x in (snapshot.ReconciliationObligations ?? Array.Empty<DurableReconciliationObligation>()).OrderBy(x => x.Identity.NamespaceKey, StringComparer.Ordinal))
        { Add(sb, x.Identity.NamespaceKey); Add(sb, x.SubmissionTruth.ToString()); Add(sb, x.ReasonCode); Add(sb, x.EvidenceReference); Add(sb, x.ObservedAt.ToUniversalTime().ToString("O")); }
        foreach (var x in (snapshot.CapitalReservations ?? Array.Empty<DurableCapitalReservation>()).OrderBy(x => x.Account.NamespaceKey, StringComparer.Ordinal).ThenBy(x => x.ReservationId.Value, StringComparer.Ordinal))
        { Add(sb, x.Account.NamespaceKey); Add(sb, x.ReservationId.Value); Add(sb, x.Amount.Amount.ToString(CultureInfo.InvariantCulture)); Add(sb, x.Amount.Currency.Code); Add(sb, x.EvidenceReference); Add(sb, x.ObservedAt.ToUniversalTime().ToString("O")); }
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(sb.ToString())));
    }

    private static string IntentFingerprint(OrderIntent intent)
    {
        var s = intent.SafetyEnvelope;
        return Pack(
            intent.ExecutionIdentity.NamespaceKey,
            intent.RiskReservationId?.Value ?? string.Empty,
            intent.DecisionBindingReference ?? string.Empty,
            intent.Instrument.Value ?? string.Empty,
            intent.Quantity.Amount.ToString(CultureInfo.InvariantCulture),
            intent.TrustEpoch.Value.ToString(CultureInfo.InvariantCulture),
            s.AccountContext?.NamespaceKey ?? string.Empty,
            s.PositionId.Value ?? string.Empty,
            s.InstrumentId.Value ?? string.Empty,
            s.Quantity.Amount.ToString(CultureInfo.InvariantCulture),
            s.MaximumAuthorizedLoss.Amount.ToString(CultureInfo.InvariantCulture),
            s.MaximumAuthorizedLoss.Currency.Code ?? string.Empty,
            s.ProtectionOwner ?? string.Empty,
            s.ProtectionState ?? string.Empty,
            s.EmergencyExitRule ?? string.Empty,
            s.ReconciliationState ?? string.Empty,
            s.LastTrustedRiskEpoch.Value.ToString(CultureInfo.InvariantCulture),
            s.ProtectionEvidenceReference ?? string.Empty);
    }

    private static void AppendContainment(StringBuilder sb, string scope, string incident, string reason, string evidence, DateTimeOffset observedAt)
    { Add(sb, scope); Add(sb, incident); Add(sb, reason); Add(sb, evidence); Add(sb, observedAt.ToUniversalTime().ToString("O")); }
    internal static string Pack(params string[] values) { var sb = new StringBuilder(); foreach (var value in values) Add(sb, value ?? string.Empty); return sb.ToString(); }
    private static void Add(StringBuilder sb, string value) => sb.Append(value.Length).Append(':').Append(value).Append('|');
    private static bool IsSha256(string value) => value is { Length: 64 } && value.All(Uri.IsHexDigit);
}
