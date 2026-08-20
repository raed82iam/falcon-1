using System.Collections.Concurrent;
using Falcon.FSATS.Trading.Domain;

namespace Falcon.FSATS.Trading.Application;

public enum ExecutionQueueState { Queued, Leased, DispatchStarted, Completed, CancelledByContainment, ReconciliationRequired }

public sealed record QueuedExecutionWork(string WorkId, OrderIntent Intent, DateTimeOffset EnqueuedAt, string EvidenceReference);
public sealed record ExecutionQueueSnapshot(string WorkId, BrokerExecutionIdentity Identity, ExecutionQueueState State, string ReasonCode, string EvidenceReference, string? ContainmentIncidentId, DateTimeOffset? LeaseExpiresAt = null);
public sealed record ExecutionContainmentResult(IReadOnlyList<ExecutionQueueSnapshot> Cancelled, IReadOnlyList<ExecutionQueueSnapshot> Reconcile);

public sealed record ExecutionContainmentEvidence
{
    public string IncidentId { get; }
    public string ReasonCode { get; }
    public string EvidenceReference { get; }
    public DateTimeOffset ObservedAt { get; }
    public IReadOnlyList<BrokerAccountContext> AffectedAccounts { get; }

    public ExecutionContainmentEvidence(string incidentId, string reasonCode, string evidenceReference, DateTimeOffset observedAt, IEnumerable<BrokerAccountContext> affectedAccounts)
    {
        IncidentId = Require(incidentId, nameof(incidentId));
        ReasonCode = Require(reasonCode, nameof(reasonCode));
        EvidenceReference = Require(evidenceReference, nameof(evidenceReference));
        if (observedAt == default) throw new ArgumentException("CONTAINMENT_OBSERVED_AT_REQUIRED", nameof(observedAt));
        ObservedAt = observedAt;
        AffectedAccounts = Array.AsReadOnly((affectedAccounts ?? throw new ArgumentNullException(nameof(affectedAccounts))).Distinct().ToArray());
        if (AffectedAccounts.Count == 0) throw new ArgumentException("CONTAINMENT_AFFECTED_ACCOUNTS_REQUIRED", nameof(affectedAccounts));
    }

    private static string Require(string value, string parameter)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("CONTAINMENT_EVIDENCE_VALUE_REQUIRED", parameter);
        return value.Trim();
    }
}

internal readonly record struct ExecutionQueueKey(BrokerAccountContext Account, string WorkId);

public sealed class ExecutionQueueLease
{
    internal ExecutionQueueLease(ExecutionQueueKey key, string workId, BrokerExecutionIdentity identity, long generation, long leaseVersion, DateTimeOffset expiresAt)
        => (Key, WorkId, Identity, Generation, LeaseVersion, ExpiresAt) = (key, workId, identity, generation, leaseVersion, expiresAt);
    internal ExecutionQueueKey Key { get; }
    internal long Generation { get; }
    internal long LeaseVersion { get; }
    public string WorkId { get; }
    public BrokerExecutionIdentity Identity { get; }
    public DateTimeOffset ExpiresAt { get; }
}

public sealed class ExecutionDispatchPermit
{
    private int _used;
    internal ExecutionDispatchPermit(ExecutionQueueKey key, BrokerExecutionIdentity identity, string decisionBindingReference, long generation, long leaseVersion, DateTimeOffset expiresAt)
        => (Key, Identity, DecisionBindingReference, Generation, LeaseVersion, ExpiresAt) = (key, identity, decisionBindingReference, generation, leaseVersion, expiresAt);
    internal ExecutionQueueKey Key { get; }
    internal long Generation { get; }
    internal long LeaseVersion { get; }
    public BrokerExecutionIdentity Identity { get; }
    public string DecisionBindingReference { get; }
    public DateTimeOffset ExpiresAt { get; }
    internal bool TryUse() => Interlocked.CompareExchange(ref _used, 1, 0) == 0;
}

public sealed class AccountScopedExecutionQueue
{
    private static readonly TimeSpan DefaultLeaseDuration = TimeSpan.FromSeconds(30);

    private sealed class Item
    {
        public required ExecutionQueueKey Key { get; init; }
        public required QueuedExecutionWork Work { get; init; }
        public required long Generation { get; init; }
        public required ExecutionQueueState State { get; set; }
        public required string Reason { get; set; }
        public required string Evidence { get; set; }
        public string? ContainmentIncidentId { get; set; }
        public long LeaseVersion { get; set; }
        public DateTimeOffset? LeaseExpiresAt { get; set; }
        public LinkedListNode<ExecutionQueueKey>? Node { get; set; }
        public long DispatchContainmentIntentVersion { get; set; }
    }

    private readonly object _gate = new();
    private readonly Dictionary<ExecutionQueueKey, Item> _items = new();
    private readonly Dictionary<BrokerExecutionIdentity, ExecutionQueueKey> _identityOwners = new();
    private readonly LinkedList<ExecutionQueueKey> _pending = new();
    private readonly Dictionary<BrokerAccountContext, ExecutionContainmentEvidence> _containedAccounts = new();
    private readonly Dictionary<(string BrokerId, string Environment), ExecutionContainmentEvidence> _containedBrokers = new();
    private readonly Dictionary<BrokerAccountContext, long> _generation = new();
    private readonly ConcurrentDictionary<BrokerAccountContext, long> _accountContainmentIntents = new();
    private readonly ConcurrentDictionary<(string BrokerId, string Environment), long> _brokerContainmentIntents = new();
    private readonly ConcurrentDictionary<BrokerAccountContext, long> _accountAppliedContainmentIntents = new();
    private readonly ConcurrentDictionary<(string BrokerId, string Environment), long> _brokerAppliedContainmentIntents = new();
    private long _containmentIntentSequence;

    public bool Enqueue(QueuedExecutionWork work, out ExecutionQueueSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(work);
        Validate(work);
        var account = work.Intent.ExecutionIdentity.Account;
        var key = new ExecutionQueueKey(account, Require(work.WorkId));
        var identity = work.Intent.ExecutionIdentity;
        lock (_gate)
        {
            if (_items.TryGetValue(key, out var old)) { snapshot = Snap(old); return false; }
            if (_identityOwners.TryGetValue(identity, out var owner)) { snapshot = Snap(_items[owner]); return false; }

            var containment = CurrentContainment(account);
            var blocked = containment is not null || HasPendingContainmentIntent(account);
            var item = new Item
            {
                Key = key,
                Work = work,
                Generation = Gen(account),
                State = blocked ? ExecutionQueueState.CancelledByContainment : ExecutionQueueState.Queued,
                Reason = blocked ? "EXECUTION_SCOPE_CONTAINMENT_PENDING_OR_ACTIVE_ENQUEUE_DENIED" : "QUEUED",
                Evidence = containment?.EvidenceReference ?? work.EvidenceReference,
                ContainmentIncidentId = containment?.IncidentId
            };
            if (!blocked) item.Node = _pending.AddLast(key);
            _items.Add(key, item);
            _identityOwners.Add(identity, key);
            snapshot = Snap(item);
            return !blocked;
        }
    }

    public bool TryLeaseNext(out ExecutionQueueLease? lease)
        => TryLeaseNext(DateTimeOffset.UtcNow, DefaultLeaseDuration, out lease);

    public bool TryLeaseNext(DateTimeOffset now, TimeSpan leaseDuration, out ExecutionQueueLease? lease)
    {
        if (now == default || leaseDuration <= TimeSpan.Zero) throw new ArgumentException("VALID_LEASE_WINDOW_REQUIRED");
        lock (_gate)
        {
            ReclaimExpiredLeasesCore(now);
            while (_pending.First is { } node)
            {
                _pending.RemoveFirst();
                var item = _items[node.Value];
                item.Node = null;
                var account = item.Work.Intent.ExecutionIdentity.Account;
                if (item.State != ExecutionQueueState.Queued) continue;
                var containment = CurrentContainment(account);
                if (containment is not null || HasPendingContainmentIntent(account) || item.Generation != Gen(account))
                { Cancel(item, "EXECUTION_SCOPE_CONTAINED_OR_PENDING_BEFORE_LEASE", containment); continue; }

                item.LeaseVersion = checked(item.LeaseVersion + 1);
                item.LeaseExpiresAt = now.Add(leaseDuration);
                item.State = ExecutionQueueState.Leased;
                item.Reason = "LEASED_FOR_DISPATCH";
                lease = new(item.Key, item.Work.WorkId, item.Work.Intent.ExecutionIdentity, item.Generation, item.LeaseVersion, item.LeaseExpiresAt.Value);
                return true;
            }
        }
        lease = null;
        return false;
    }

    public int ReclaimExpiredLeases(DateTimeOffset now)
    {
        if (now == default) throw new ArgumentException("LEASE_RECLAIM_TIME_REQUIRED", nameof(now));
        lock (_gate) return ReclaimExpiredLeasesCore(now);
    }

    public bool TryBeginDispatch(ExecutionQueueLease lease, out ExecutionDispatchPermit? permit)
        => TryBeginDispatch(lease, DateTimeOffset.UtcNow, out permit);

    public bool TryBeginDispatch(ExecutionQueueLease lease, DateTimeOffset now, out ExecutionDispatchPermit? permit)
    {
        ArgumentNullException.ThrowIfNull(lease);
        if (now == default) throw new ArgumentException("DISPATCH_TIME_REQUIRED", nameof(now));
        lock (_gate)
        {
            if (!_items.TryGetValue(lease.Key, out var item) ||
                item.State != ExecutionQueueState.Leased ||
                item.Generation != lease.Generation ||
                item.LeaseVersion != lease.LeaseVersion ||
                item.LeaseExpiresAt is null || item.LeaseExpiresAt <= now ||
                item.Work.Intent.ExecutionIdentity != lease.Identity ||
                string.IsNullOrWhiteSpace(item.Work.Intent.DecisionBindingReference))
            { permit = null; return false; }

            var account = item.Work.Intent.ExecutionIdentity.Account;
            var containment = CurrentContainment(account);
            if (containment is not null || HasPendingContainmentIntent(account) || item.Generation != Gen(account))
            { Cancel(item, "EXECUTION_SCOPE_CONTAINED_OR_PENDING_BEFORE_PERMIT", containment); permit = null; return false; }

            item.Reason = "DISPATCH_PERMIT_ISSUED";
            permit = new(item.Key, item.Work.Intent.ExecutionIdentity, item.Work.Intent.DecisionBindingReference, item.Generation, item.LeaseVersion, item.LeaseExpiresAt.Value);
            return true;
        }
    }

    internal bool TryCommitAndStartDispatch(
        ExecutionDispatchPermit permit,
        Func<ValueTask<BrokerSubmissionResult>> startDispatch,
        out ValueTask<BrokerSubmissionResult> startedDispatch,
        out Exception? synchronousStartFailure)
    {
        ArgumentNullException.ThrowIfNull(permit);
        ArgumentNullException.ThrowIfNull(startDispatch);
        startedDispatch = default;
        synchronousStartFailure = null;
        var now = DateTimeOffset.UtcNow;

        lock (_gate)
        {
            if (!_items.TryGetValue(permit.Key, out var item) ||
                item.State != ExecutionQueueState.Leased ||
                item.Generation != permit.Generation ||
                item.LeaseVersion != permit.LeaseVersion ||
                item.LeaseExpiresAt is null || item.LeaseExpiresAt <= now ||
                permit.ExpiresAt != item.LeaseExpiresAt ||
                item.Work.Intent.ExecutionIdentity != permit.Identity ||
                !StringComparer.Ordinal.Equals(item.Work.Intent.DecisionBindingReference, permit.DecisionBindingReference))
                return false;

            var account = item.Work.Intent.ExecutionIdentity.Account;
            var containment = CurrentContainment(account);
            if (containment is not null || HasPendingContainmentIntent(account) || item.Generation != Gen(account))
            {
                Cancel(item, "EXECUTION_SCOPE_CONTAINED_OR_PENDING_BEFORE_EXTERNAL_DISPATCH", containment);
                return false;
            }

            if (!permit.TryUse()) return false;

            item.DispatchContainmentIntentVersion = ContainmentIntentVersion(account);
            item.State = ExecutionQueueState.DispatchStarted;
            item.Reason = "EXTERNAL_DISPATCH_INVOCATION_STARTED";
            item.LeaseExpiresAt = null;

            try
            {
                // External invocation begins while the same queue gate is held. A containment caller
                // registers its intent before waiting for this gate. Therefore an intent registered
                // before this point blocks dispatch, while an intent registered after this point is
                // detected by Complete() and forces reconciliation of the ambiguous external outcome.
                startedDispatch = startDispatch();
            }
            catch (Exception ex)
            {
                synchronousStartFailure = ex;
            }

            return true;
        }
    }

    internal bool Complete(ExecutionDispatchPermit permit, BrokerOrderSnapshot outcome)
    {
        ArgumentNullException.ThrowIfNull(permit); ArgumentNullException.ThrowIfNull(outcome);
        lock (_gate)
        {
            if (!_items.TryGetValue(permit.Key, out var item) || item.Work.Intent.ExecutionIdentity != permit.Identity || outcome.ExecutionIdentity != permit.Identity ||
                !StringComparer.Ordinal.Equals(item.Work.Intent.DecisionBindingReference, permit.DecisionBindingReference)) return false;
            if (item.State == ExecutionQueueState.ReconciliationRequired) return false;
            if (item.State != ExecutionQueueState.DispatchStarted) return false;

            var currentIntentVersion = ContainmentIntentVersion(item.Work.Intent.ExecutionIdentity.Account);
            if (currentIntentVersion > item.DispatchContainmentIntentVersion)
            {
                item.State = ExecutionQueueState.ReconciliationRequired;
                item.Reason = "CONTAINMENT_INTENT_OVERLAPPED_EXTERNAL_DISPATCH_RECONCILIATION_REQUIRED";
                return false;
            }

            item.State = outcome.State == OrderState.ReconciliationRequired ? ExecutionQueueState.ReconciliationRequired : ExecutionQueueState.Completed;
            item.Reason = outcome.ReasonCode;
            return true;
        }
    }

    public ExecutionContainmentResult ContainAccount(BrokerAccountContext account, ExecutionContainmentEvidence evidence)
    {
        ArgumentNullException.ThrowIfNull(account); ArgumentNullException.ThrowIfNull(evidence);
        if (evidence.AffectedAccounts.Count != 1 || evidence.AffectedAccounts[0] != account) throw new ArgumentException("ACCOUNT_CONTAINMENT_EVIDENCE_SCOPE_MISMATCH", nameof(evidence));
        var intentVersion = RegisterAccountContainmentIntent(account);
        lock (_gate)
        {
            Contain(account, evidence);
            MarkAccountContainmentIntentApplied(account, intentVersion);
            return Result(x => x.Work.Intent.ExecutionIdentity.Account == account);
        }
    }

    public ExecutionContainmentResult ContainBroker(string brokerId, string environment, ExecutionContainmentEvidence evidence)
    {
        brokerId = Require(brokerId).ToUpperInvariant(); environment = Require(environment).ToUpperInvariant(); ArgumentNullException.ThrowIfNull(evidence);
        if (evidence.AffectedAccounts.Any(a => a.BrokerId != brokerId || a.Environment != environment)) throw new ArgumentException("BROKER_CONTAINMENT_EVIDENCE_SCOPE_MISMATCH", nameof(evidence));
        var brokerKey = (brokerId, environment);
        var intentVersion = RegisterBrokerContainmentIntent(brokerKey);
        lock (_gate)
        {
            _containedBrokers[brokerKey] = evidence;
            foreach (var account in evidence.AffectedAccounts) Contain(account, evidence);
            foreach (var account in _items.Values.Select(x => x.Work.Intent.ExecutionIdentity.Account).Where(a => (a.BrokerId, a.Environment) == brokerKey).Distinct()) Contain(account, evidence);
            MarkBrokerContainmentIntentApplied(brokerKey, intentVersion);
            return Result(x => (x.Work.Intent.ExecutionIdentity.Account.BrokerId, x.Work.Intent.ExecutionIdentity.Account.Environment) == brokerKey);
        }
    }

    public bool TryReleaseAccount(BrokerAccountContext account, BrokerAccountReconciliationEvidence reconciliation)
    {
        ArgumentNullException.ThrowIfNull(account); ArgumentNullException.ThrowIfNull(reconciliation);
        lock (_gate)
        {
            if (_containedBrokers.ContainsKey((account.BrokerId, account.Environment))) return false;
            if (!_containedAccounts.ContainsKey(account) || !BrokerOutageRecoveryPolicy.IsCompleteReconciliationFor(account, reconciliation)) return false;
            _containedAccounts.Remove(account);
            return true;
        }
    }

    public bool TryReleaseBroker(string brokerId, string environment, IReadOnlyCollection<BrokerAccountReconciliationEvidence> reconciliation)
    {
        brokerId = Require(brokerId).ToUpperInvariant(); environment = Require(environment).ToUpperInvariant(); ArgumentNullException.ThrowIfNull(reconciliation);
        var brokerKey = (brokerId, environment);
        lock (_gate)
        {
            if (!_containedBrokers.TryGetValue(brokerKey, out var containment)) return false;
            var accounts = containment.AffectedAccounts.Concat(_items.Values.Select(x => x.Work.Intent.ExecutionIdentity.Account).Where(a => (a.BrokerId, a.Environment) == brokerKey)).Distinct().ToArray();
            if (accounts.Length == 0) return false;
            foreach (var account in accounts)
                if (!reconciliation.Any(x => BrokerOutageRecoveryPolicy.IsCompleteReconciliationFor(account, x))) return false;
            _containedBrokers.Remove(brokerKey);
            foreach (var account in accounts) _containedAccounts.Remove(account);
            return true;
        }
    }

    public bool IsContained(BrokerAccountContext account) { lock (_gate) return CurrentContainment(account) is not null || HasPendingContainmentIntent(account); }
    public int PendingCount(BrokerAccountContext account) { lock (_gate) return _items.Values.Count(x => x.Work.Intent.ExecutionIdentity.Account == account && x.State is ExecutionQueueState.Queued or ExecutionQueueState.Leased); }
    public IReadOnlyList<ExecutionQueueSnapshot> Snapshot(BrokerAccountContext account) { lock (_gate) return _items.Values.Where(x => x.Work.Intent.ExecutionIdentity.Account == account).Select(Snap).ToArray(); }

    private int ReclaimExpiredLeasesCore(DateTimeOffset now)
    {
        var reclaimed = 0;
        foreach (var item in _items.Values.Where(x => x.State == ExecutionQueueState.Leased && x.LeaseExpiresAt is { } expiry && expiry <= now).ToArray())
        {
            var account = item.Work.Intent.ExecutionIdentity.Account;
            var containment = CurrentContainment(account);
            if (containment is not null || HasPendingContainmentIntent(account) || item.Generation != Gen(account))
            {
                Cancel(item, "EXPIRED_LEASE_CANCELLED_BY_CONTAINMENT", containment);
            }
            else
            {
                item.State = ExecutionQueueState.Queued;
                item.Reason = "EXPIRED_LEASE_REQUEUED_BEFORE_DISPATCH";
                item.LeaseExpiresAt = null;
                if (item.Node is null) item.Node = _pending.AddLast(item.Key);
            }
            reclaimed++;
        }
        return reclaimed;
    }

    private void Contain(BrokerAccountContext account, ExecutionContainmentEvidence evidence)
    {
        _containedAccounts[account] = evidence;
        _generation[account] = checked(Gen(account) + 1);
        foreach (var item in _items.Values.Where(x => x.Work.Intent.ExecutionIdentity.Account == account))
        {
            if (item.State == ExecutionQueueState.Queued && item.Node is not null) { _pending.Remove(item.Node); item.Node = null; }
            if (item.State is ExecutionQueueState.Queued or ExecutionQueueState.Leased)
                Cancel(item, "CANCELLED_BY_ACCOUNT_CONTAINMENT", evidence);
            else if (item.State == ExecutionQueueState.CancelledByContainment && item.ContainmentIncidentId is null)
            {
                item.Reason = "CANCELLED_BY_ACCOUNT_CONTAINMENT";
                item.Evidence = evidence.EvidenceReference;
                item.ContainmentIncidentId = evidence.IncidentId;
                item.LeaseExpiresAt = null;
            }
            else if (item.State is ExecutionQueueState.DispatchStarted or ExecutionQueueState.ReconciliationRequired)
            {
                item.State = ExecutionQueueState.ReconciliationRequired;
                item.Reason = "DISPATCH_STARTED_BEFORE_CONTAINMENT_RECONCILIATION_REQUIRED";
                item.Evidence = evidence.EvidenceReference;
                item.ContainmentIncidentId = evidence.IncidentId;
                item.LeaseExpiresAt = null;
            }
        }
    }

    private long RegisterAccountContainmentIntent(BrokerAccountContext account)
    {
        var version = NextContainmentIntentVersion();
        _accountContainmentIntents.AddOrUpdate(account, version, (_, old) => Math.Max(old, version));
        return version;
    }

    private long RegisterBrokerContainmentIntent((string BrokerId, string Environment) brokerKey)
    {
        var version = NextContainmentIntentVersion();
        _brokerContainmentIntents.AddOrUpdate(brokerKey, version, (_, old) => Math.Max(old, version));
        return version;
    }

    private void MarkAccountContainmentIntentApplied(BrokerAccountContext account, long version)
        => _accountAppliedContainmentIntents.AddOrUpdate(account, version, (_, old) => Math.Max(old, version));

    private void MarkBrokerContainmentIntentApplied((string BrokerId, string Environment) brokerKey, long version)
        => _brokerAppliedContainmentIntents.AddOrUpdate(brokerKey, version, (_, old) => Math.Max(old, version));

    private long NextContainmentIntentVersion()
    {
        var version = Interlocked.Increment(ref _containmentIntentSequence);
        if (version <= 0) throw new InvalidOperationException("CONTAINMENT_INTENT_SEQUENCE_EXHAUSTED");
        return version;
    }

    private long ContainmentIntentVersion(BrokerAccountContext account)
    {
        _accountContainmentIntents.TryGetValue(account, out var accountVersion);
        _brokerContainmentIntents.TryGetValue((account.BrokerId, account.Environment), out var brokerVersion);
        return Math.Max(accountVersion, brokerVersion);
    }

    private bool HasPendingContainmentIntent(BrokerAccountContext account)
    {
        _accountContainmentIntents.TryGetValue(account, out var accountIntent);
        _accountAppliedContainmentIntents.TryGetValue(account, out var accountApplied);
        if (accountIntent > accountApplied) return true;

        var brokerKey = (account.BrokerId, account.Environment);
        _brokerContainmentIntents.TryGetValue(brokerKey, out var brokerIntent);
        _brokerAppliedContainmentIntents.TryGetValue(brokerKey, out var brokerApplied);
        return brokerIntent > brokerApplied;
    }

    private ExecutionContainmentResult Result(Func<Item, bool> predicate)
    {
        var all = _items.Values.Where(predicate).Select(Snap).ToArray();
        return new(all.Where(x => x.State == ExecutionQueueState.CancelledByContainment).ToArray(), all.Where(x => x.State == ExecutionQueueState.ReconciliationRequired).ToArray());
    }

    private ExecutionContainmentEvidence? CurrentContainment(BrokerAccountContext account)
    {
        if (_containedBrokers.TryGetValue((account.BrokerId, account.Environment), out var broker)) return broker;
        return _containedAccounts.TryGetValue(account, out var local) ? local : null;
    }

    private long Gen(BrokerAccountContext account) => _generation.TryGetValue(account, out var g) ? g : 0;
    private static void Cancel(Item item, string reason, ExecutionContainmentEvidence? evidence)
    {
        item.State = ExecutionQueueState.CancelledByContainment;
        item.Reason = reason;
        item.LeaseExpiresAt = null;
        if (evidence is not null) { item.Evidence = evidence.EvidenceReference; item.ContainmentIncidentId = evidence.IncidentId; }
    }
    private static ExecutionQueueSnapshot Snap(Item x) => new(x.Work.WorkId, x.Work.Intent.ExecutionIdentity, x.State, x.Reason, x.Evidence, x.ContainmentIncidentId, x.LeaseExpiresAt);
    private static string Require(string value) { if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("EXECUTION_QUEUE_VALUE_REQUIRED"); return value.Trim(); }
    private static void Validate(QueuedExecutionWork work) { Require(work.WorkId); Require(work.EvidenceReference); if (work.EnqueuedAt == default) throw new ArgumentException("ENQUEUE_TIME_REQUIRED"); ArgumentNullException.ThrowIfNull(work.Intent); ArgumentNullException.ThrowIfNull(work.Intent.ExecutionIdentity); }
}
