using System;

namespace Foundation.State;

public interface IAuthoritativeStateProvider
{
    DurableStateReadResult ReadCurrent(
        string stateNamespace,
        string subjectId,
        FoundationStateClass stateClass);

    DurableStateWriteResult WriteCurrent(
        AuthoritativeStateRecord record,
        long expectedVersion);

}

public interface IAuthoritativeStateReconciliationProvider
{
    DurableCommitLookupResult LookupCommitByRequest(string requestIdentity);

    DurableCommitLookupResult LookupCommitByDecision(string decisionIdentity);

    DurableHistoryReadResult ReadLatestTrustedHistory(
        string stateNamespace,
        string subjectId,
        FoundationStateClass stateClass);

    DurableHistoryReadResult ReadTrustedHistoryVersion(
        string stateNamespace,
        string subjectId,
        FoundationStateClass stateClass,
        long stateVersion,
        string stateDigest,
        string commitIdentity);
}

public sealed class DurableAuthoritativeStateStore
{
    private readonly StateOwnershipRegistry _ownership;
    private readonly IAuthoritativeStateProvider _provider;

    public DurableAuthoritativeStateStore(
        StateOwnershipRegistry ownership,
        IAuthoritativeStateProvider provider)
    {
        _ownership = ownership ?? throw new ArgumentNullException(nameof(ownership));
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
    }

    public DurableStateWriteResult Write(
        AuthoritativeStateRecord? proposed,
        long expectedVersion)
    {
        if (proposed is null ||
            string.IsNullOrWhiteSpace(proposed.RecordId) ||
            string.IsNullOrWhiteSpace(proposed.Namespace) ||
            string.IsNullOrWhiteSpace(proposed.SubjectId) ||
            proposed.Representation != StateRepresentationKind.Authoritative ||
            string.IsNullOrWhiteSpace(proposed.AuthoritativeOwner) ||
            string.IsNullOrWhiteSpace(proposed.AuthoritativeSource) ||
            string.IsNullOrWhiteSpace(proposed.PersistenceOwner) ||
            string.IsNullOrWhiteSpace(proposed.WriterAuthority) ||
            string.IsNullOrWhiteSpace(proposed.SourceIdentity) ||
            proposed.StateVersion < 0 ||
            proposed.EffectiveTime == default ||
            string.IsNullOrWhiteSpace(proposed.RetentionClassification) ||
            string.IsNullOrWhiteSpace(proposed.Payload))
        {
            return Reject(DurableStateClassification.Malformed, "MALFORMED_STATE_RECORD");
        }

        var declaration = _ownership.Resolve(
            proposed.Namespace,
            proposed.SubjectId,
            proposed.StateClass);

        if (declaration is null)
        {
            return Reject(
                DurableStateClassification.OwnershipMissing,
                "STATE_OWNERSHIP_MISSING");
        }

        if (!string.Equals(
                declaration.AuthoritativeOwner,
                proposed.AuthoritativeOwner,
                StringComparison.Ordinal) ||
            !string.Equals(
                declaration.AuthoritativeSource,
                proposed.AuthoritativeSource,
                StringComparison.Ordinal) ||
            !string.Equals(
                declaration.PersistenceOwner,
                proposed.PersistenceOwner,
                StringComparison.Ordinal))
        {
            return Reject(
                DurableStateClassification.OwnershipConflict,
                "STATE_OWNERSHIP_BINDING_MISMATCH");
        }

        if (!string.Equals(
                declaration.WriteAuthority,
                proposed.WriterAuthority,
                StringComparison.Ordinal))
        {
            return Reject(
                DurableStateClassification.UnauthorizedWriter,
                "STATE_WRITE_AUTHORITY_MISMATCH");
        }

        var computed = proposed.WithComputedDigest();
        if (!string.IsNullOrWhiteSpace(proposed.RecordDigest) &&
            !string.Equals(
                proposed.RecordDigest,
                computed.RecordDigest,
                StringComparison.Ordinal))
        {
            return Reject(
                DurableStateClassification.Corrupted,
                "STATE_RECORD_DIGEST_MISMATCH");
        }

        return _provider.WriteCurrent(computed, expectedVersion);
    }

    public DurableCommitLookupResult LookupCommitByRequest(string requestIdentity)
        => ReconciliationProviderOrNull()?.LookupCommitByRequest(requestIdentity)
           ?? new DurableCommitLookupResult(
               DurableStateClassification.UncertainBeforeCommit,
               "RECONCILIATION_LOOKUP_UNAVAILABLE",
               null);

    public DurableCommitLookupResult LookupCommitByDecision(string decisionIdentity)
        => ReconciliationProviderOrNull()?.LookupCommitByDecision(decisionIdentity)
           ?? new DurableCommitLookupResult(
               DurableStateClassification.UncertainBeforeCommit,
               "RECONCILIATION_LOOKUP_UNAVAILABLE",
               null);

    public DurableHistoryReadResult ReadLatestTrustedHistory(
        string stateNamespace,
        string subjectId,
        FoundationStateClass stateClass)
        => ReconciliationProviderOrNull()?.ReadLatestTrustedHistory(stateNamespace, subjectId, stateClass)
           ?? new DurableHistoryReadResult(
               DurableStateClassification.Partial,
               "TRUSTED_HISTORY_LOOKUP_UNAVAILABLE",
               null);

    public DurableHistoryReadResult ReadTrustedHistoryVersion(
        string stateNamespace,
        string subjectId,
        FoundationStateClass stateClass,
        long stateVersion,
        string stateDigest,
        string commitIdentity)
        => ReconciliationProviderOrNull()?.ReadTrustedHistoryVersion(
               stateNamespace, subjectId, stateClass, stateVersion, stateDigest, commitIdentity)
           ?? new DurableHistoryReadResult(
               DurableStateClassification.Partial,
               "TRUSTED_HISTORY_VERSION_LOOKUP_UNAVAILABLE",
               null);

    private IAuthoritativeStateReconciliationProvider? ReconciliationProviderOrNull()
        => _provider as IAuthoritativeStateReconciliationProvider;

    public DurableStateReadResult Read(
        string stateNamespace,
        string subjectId,
        FoundationStateClass stateClass)
        => _provider.ReadCurrent(stateNamespace, subjectId, stateClass);

    private static DurableStateWriteResult Reject(
        DurableStateClassification classification,
        string reason)
        => new(classification, reason, null, string.Empty);
}
