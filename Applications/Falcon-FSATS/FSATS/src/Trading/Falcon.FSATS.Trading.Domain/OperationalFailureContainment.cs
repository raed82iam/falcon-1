namespace Falcon.FSATS.Trading.Domain;

public enum OperationalFailureClass { ProviderUnavailable, BrokerExecutionApiUnavailable, CredentialUnavailableOrInvalid, ExecutionRouteUnavailable, AccountTruthUnavailable, Unknown }
public enum OperationalTruthState { BrokerConfirmed, ProviderConfirmedMarketOnly, UserReported, ScreenshotObserved, LastConfirmedStale, Unknown, ReconciliationRequired }
public enum OperationalContainmentState { None, Scoped, Expanded }
public enum OperationalRecoveryState { Healthy, Investigating, HumanAssisted, AwaitingBrokerReconciliation, Recovered }
public enum ProvenFailureBlastRadius { AccountLocal, ExplicitAccountSet, BrokerWide, ProviderAccountWide, ProviderWide, ExecutionRouteWide, Unknown }

public sealed class FailureLocalityEvidence
{
    public static readonly TimeSpan DefaultMaximumAge = TimeSpan.FromMinutes(2);

    public FailureLocalityEvidence(string evidenceReference, ProvenFailureBlastRadius blastRadius, DateTimeOffset observedAt, IEnumerable<BrokerAccountContext>? affectedAccounts = null, string? sharedDependencyId = null)
    {
        if (string.IsNullOrWhiteSpace(evidenceReference)) throw new ArgumentException("LOCALITY_EVIDENCE_REFERENCE_REQUIRED", nameof(evidenceReference));
        if (observedAt == default) throw new ArgumentException("LOCALITY_EVIDENCE_TIME_REQUIRED", nameof(observedAt));
        EvidenceReference = evidenceReference.Trim();
        BlastRadius = blastRadius;
        ObservedAt = observedAt;
        SharedDependencyId = Normalize(sharedDependencyId);
        AffectedAccounts = Array.AsReadOnly((affectedAccounts ?? Array.Empty<BrokerAccountContext>()).Distinct().ToArray());
    }

    public string EvidenceReference { get; }
    public ProvenFailureBlastRadius BlastRadius { get; }
    public DateTimeOffset ObservedAt { get; }
    public string? SharedDependencyId { get; }
    public IReadOnlyList<BrokerAccountContext> AffectedAccounts { get; }

    public bool IsCurrent(DateTimeOffset now, TimeSpan? maximumAge = null)
    {
        var maxAge = maximumAge ?? DefaultMaximumAge;
        return now != default && maxAge >= TimeSpan.Zero && ObservedAt != default && ObservedAt <= now && now - ObservedAt <= maxAge;
    }

    public bool IsApplicableTo(OperationalFailureScope scope)
        => IsApplicableTo(scope, DateTimeOffset.UtcNow, DefaultMaximumAge);

    public bool IsApplicableTo(OperationalFailureScope scope, DateTimeOffset now, TimeSpan? maximumAge = null)
    {
        ArgumentNullException.ThrowIfNull(scope);
        if (!IsCurrent(now, maximumAge)) return false;
        return BlastRadius switch
        {
            ProvenFailureBlastRadius.AccountLocal => AffectedAccounts.Count == 1 && AffectedAccounts[0] == scope.Account,
            ProvenFailureBlastRadius.ExplicitAccountSet => AffectedAccounts.Count > 0 && AffectedAccounts.Contains(scope.Account),
            ProvenFailureBlastRadius.BrokerWide => StringComparer.Ordinal.Equals(SharedDependencyId, BrokerDependency(scope.BrokerId, scope.Environment)),
            ProvenFailureBlastRadius.ProviderAccountWide => scope.ProviderId is not null && scope.ProviderAccountId is not null && StringComparer.Ordinal.Equals(SharedDependencyId, ProviderAccountDependency(scope.ProviderId, scope.ProviderAccountId)),
            ProvenFailureBlastRadius.ProviderWide => scope.ProviderId is not null && StringComparer.Ordinal.Equals(SharedDependencyId, ProviderDependency(scope.ProviderId)),
            ProvenFailureBlastRadius.ExecutionRouteWide => scope.ExecutionRouteId is not null && StringComparer.Ordinal.Equals(SharedDependencyId, ExecutionRouteDependency(scope.BrokerId, scope.BrokerAccountId, scope.Environment, scope.ExecutionRouteId)),
            _ => false
        };
    }

    public static string BrokerDependency(string brokerId, string environment)
        => $"BROKER:{CanonicalPart(brokerId)}|{CanonicalPart(environment)}";

    public static string ProviderAccountDependency(string providerId, string providerAccountId)
        => $"PROVIDER_ACCOUNT:{CanonicalPart(providerId)}|{OpaquePart(providerAccountId)}";

    public static string ProviderDependency(string providerId)
        => $"PROVIDER:{CanonicalPart(providerId)}";

    public static string ExecutionRouteDependency(string brokerId, string brokerAccountId, string environment, string executionRouteId)
        => $"EXECUTION_ROUTE:{CanonicalPart(brokerId)}|{OpaquePart(brokerAccountId)}|{CanonicalPart(environment)}|{OpaquePart(executionRouteId)}";

    private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    private static string CanonicalPart(string value) => Uri.EscapeDataString(value.Trim().ToUpperInvariant());
    private static string OpaquePart(string value) => Uri.EscapeDataString(value.Trim());
}

public sealed class OperationalFailureScope
{
    public OperationalFailureScope(BrokerAccountContext account, string market, string? providerId, string? providerAccountId, string? executionRouteId, IEnumerable<PositionId>? affectedPositions, IEnumerable<OrderId>? affectedOrders, IEnumerable<string>? affectedDataProducts, OperationalFailureClass failureClass, OperationalTruthState truthState, OperationalContainmentState containmentState, OperationalRecoveryState recoveryState)
    {
        Account = account ?? throw new ArgumentNullException(nameof(account));
        Market = Require(market, nameof(market)).ToUpperInvariant();
        ProviderId = Normalize(providerId)?.ToUpperInvariant();
        ProviderAccountId = Normalize(providerAccountId);
        ExecutionRouteId = Normalize(executionRouteId);
        AffectedPositions = Array.AsReadOnly((affectedPositions ?? Array.Empty<PositionId>()).ToArray());
        AffectedOrders = Array.AsReadOnly((affectedOrders ?? Array.Empty<OrderId>()).ToArray());
        AffectedDataProducts = Array.AsReadOnly((affectedDataProducts ?? Array.Empty<string>()).Select(x => Require(x, nameof(affectedDataProducts))).ToArray());
        FailureClass = failureClass;
        TruthState = truthState;
        ContainmentState = containmentState;
        RecoveryState = recoveryState;
    }

    public BrokerAccountContext Account { get; }
    public string BrokerId => Account.BrokerId;
    public string BrokerAccountId => Account.BrokerAccountId;
    public string Environment => Account.Environment;
    public string Market { get; }
    public string? ProviderId { get; }
    public string? ProviderAccountId { get; }
    public string? ExecutionRouteId { get; }
    public IReadOnlyList<PositionId> AffectedPositions { get; }
    public IReadOnlyList<OrderId> AffectedOrders { get; }
    public IReadOnlyList<string> AffectedDataProducts { get; }
    public OperationalFailureClass FailureClass { get; }
    public OperationalTruthState TruthState { get; }
    public OperationalContainmentState ContainmentState { get; }
    public OperationalRecoveryState RecoveryState { get; }

    public bool AffectsAccount(BrokerAccountContext account) => Account == account;
    public bool HasSpecificFailureTarget() => ProviderId is not null || BrokerId.Length > 0 || ExecutionRouteId is not null || AffectedPositions.Count > 0 || AffectedOrders.Count > 0 || AffectedDataProducts.Count > 0;

    private static string Require(string value, string parameter) { if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("IDENTITY_OR_SCOPE_REQUIRED", parameter); return value.Trim(); }
    private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

public sealed record ContainmentDecision(OperationalContainmentState State, string ReasonCode, BrokerAccountContext Account, ProvenFailureBlastRadius ProvenBlastRadius, string EvidenceReference);

public static class OperationalFailureContainmentPolicy
{
    public static ContainmentDecision Decide(OperationalFailureScope scope, FailureLocalityEvidence evidence)
        => Decide(scope, evidence, DateTimeOffset.UtcNow, FailureLocalityEvidence.DefaultMaximumAge);

    public static ContainmentDecision Decide(OperationalFailureScope scope, FailureLocalityEvidence evidence, DateTimeOffset now, TimeSpan? maximumEvidenceAge = null)
    {
        ArgumentNullException.ThrowIfNull(scope); ArgumentNullException.ThrowIfNull(evidence);
        if (evidence.IsApplicableTo(scope, now, maximumEvidenceAge) && scope.HasSpecificFailureTarget())
            return new(OperationalContainmentState.Scoped, "FRESH_EVIDENCE_BOUND_MINIMUM_NECESSARY_CONTAINMENT", scope.Account, evidence.BlastRadius, evidence.EvidenceReference);
        return new(OperationalContainmentState.Expanded, "BLAST_RADIUS_UNKNOWN_STALE_OR_EVIDENCE_MISMATCH_EXPAND_CONTAINMENT", scope.Account, ProvenFailureBlastRadius.Unknown, evidence.EvidenceReference);
    }

    public static bool ShouldAffectPeer(OperationalFailureScope scope, BrokerAccountContext peerAccount, FailureLocalityEvidence evidence)
        => ShouldAffectPeer(scope, peerAccount, evidence, DateTimeOffset.UtcNow, FailureLocalityEvidence.DefaultMaximumAge);

    public static bool ShouldAffectPeer(OperationalFailureScope scope, BrokerAccountContext peerAccount, FailureLocalityEvidence evidence, DateTimeOffset now, TimeSpan? maximumEvidenceAge = null)
    {
        ArgumentNullException.ThrowIfNull(scope); ArgumentNullException.ThrowIfNull(peerAccount); ArgumentNullException.ThrowIfNull(evidence);
        if (scope.AffectsAccount(peerAccount)) return true;
        if (!evidence.IsApplicableTo(scope, now, maximumEvidenceAge)) return true;
        if (evidence.AffectedAccounts.Contains(peerAccount)) return true;

        return evidence.BlastRadius switch
        {
            ProvenFailureBlastRadius.AccountLocal => false,
            ProvenFailureBlastRadius.ExplicitAccountSet => false,
            ProvenFailureBlastRadius.BrokerWide => scope.BrokerId == peerAccount.BrokerId && scope.Environment == peerAccount.Environment,
            ProvenFailureBlastRadius.ProviderAccountWide or ProvenFailureBlastRadius.ProviderWide or ProvenFailureBlastRadius.ExecutionRouteWide => evidence.AffectedAccounts.Count == 0,
            _ => true
        };
    }
}
