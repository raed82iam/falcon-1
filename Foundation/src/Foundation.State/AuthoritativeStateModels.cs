using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Foundation.State;

public enum FoundationStateClass
{
    LifecycleState,
    AuthorityPolicyBaseline,
    AuthorityDecision,
    StateOwnershipDeclaration,
    OperationalEvidence,
    AcceptedFactEvent,
    PersistenceCommitState,
    ReconciliationState
}

public enum StateRepresentationKind
{
    Authoritative,
    Derived,
    Cached,
    Observed,
    LastKnown,
    Expected,
    Desired,
    Historical
}

public enum DurableStateClassification
{
    Accepted,
    Missing,
    Partial,
    Malformed,
    Corrupted,
    Conflicting,
    StaleExpectedVersion,
    UnauthorizedWriter,
    OwnershipMissing,
    OwnershipConflict,
    NonAuthoritativeRepresentation,
    LockUnavailable,
    UncertainBeforeCommit,
    UncertainAfterCommit,
    TrustedHistoryReconstructed
}

public sealed record StateOwnershipDeclaration(
    string DeclarationId,
    string Namespace,
    string SubjectId,
    FoundationStateClass StateClass,
    string AuthoritativeOwner,
    string AuthoritativeSource,
    string PersistenceOwner,
    string ReadAuthorities,
    string WriteAuthority,
    string RetentionClassification,
    long DeclarationVersion,
    DateTimeOffset EffectiveTime,
    DateTimeOffset Expiry)
{
    public string Key => StateCanonicalEncoding.Key(Namespace, SubjectId, StateClass);
}

public sealed record AuthoritativeStateRecord(
    string RecordId,
    string Namespace,
    string SubjectId,
    FoundationStateClass StateClass,
    StateRepresentationKind Representation,
    string AuthoritativeOwner,
    string AuthoritativeSource,
    string PersistenceOwner,
    string WriterAuthority,
    string SourceIdentity,
    long StateVersion,
    DateTimeOffset EffectiveTime,
    string RetentionClassification,
    string Payload,
    string PreviousRecordDigest,
    string RecordDigest)
{
    public string Key => StateCanonicalEncoding.Key(Namespace, SubjectId, StateClass);

    public AuthoritativeStateRecord WithComputedDigest()
        => this with { RecordDigest = StateCanonicalEncoding.ComputeRecordDigest(this) };
}


public enum PersistenceCommitPhase
{
    Prepared,
    HistoryCommitted,
    CurrentCommitted
}

public enum PersistenceWriteInterruptionPoint
{
    None,
    AfterPrepared,
    AfterHistoryFile,
    AfterHistoryCommitted,
    AfterCurrentFile,
    AfterCurrentCommitted,
    AfterRegistryAnchorGeneration
}

public sealed record PersistenceCommitRecord(
    string RequestIdentity,
    string DecisionIdentity,
    string CommitIdentity,
    string StateKey,
    long StateVersion,
    string StateDigest,
    PersistenceCommitPhase Phase,
    string RecordDigest)
{
    public PersistenceCommitRecord WithComputedDigest()
    {
        var canonical = string.Join("\u001F",
            RequestIdentity, DecisionIdentity, CommitIdentity, StateKey,
            StateVersion.ToString(CultureInfo.InvariantCulture), StateDigest, Phase.ToString());
        return this with
        {
            RecordDigest = Convert.ToHexString(
                SHA256.HashData(Encoding.UTF8.GetBytes(canonical)))
        };
    }
}

public sealed record DurableCommitLookupResult(
    DurableStateClassification Classification,
    string Reason,
    PersistenceCommitRecord? Commit);

public sealed record DurableHistoryReadResult(
    DurableStateClassification Classification,
    string Reason,
    AuthoritativeStateRecord? Latest);

public sealed record DurableStateWriteResult(
    DurableStateClassification Classification,
    string Reason,
    AuthoritativeStateRecord? Current,
    string CommitIdentity)
{
    public bool Accepted => Classification == DurableStateClassification.Accepted;
}

public sealed record DurableStateReadResult(
    DurableStateClassification Classification,
    string Reason,
    AuthoritativeStateRecord? Current)
{
    public bool Accepted => Classification == DurableStateClassification.Accepted;
}

public static class StateCanonicalEncoding
{
    public static string Key(
        string stateNamespace,
        string subjectId,
        FoundationStateClass stateClass)
        => $"{stateNamespace}|{subjectId}|{stateClass}";

    public static string ComputeRecordDigest(AuthoritativeStateRecord record)
    {
        var canonical = string.Join(
            "\u001F",
            record.RecordId,
            record.Namespace,
            record.SubjectId,
            record.StateClass.ToString(),
            record.Representation.ToString(),
            record.AuthoritativeOwner,
            record.AuthoritativeSource,
            record.PersistenceOwner,
            record.WriterAuthority,
            record.SourceIdentity,
            record.StateVersion.ToString(CultureInfo.InvariantCulture),
            record.EffectiveTime.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture),
            record.RetentionClassification,
            record.Payload,
            record.PreviousRecordDigest);

        return Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(canonical)));
    }

    public static string SerializeRecord(AuthoritativeStateRecord record)
        => JsonSerializer.Serialize(
            record,
            new JsonSerializerOptions
            {
                WriteIndented = false,
                PropertyNamingPolicy = null
            });

    public static bool TryDeserializeRecord(
        string text,
        out AuthoritativeStateRecord? record)
    {
        try
        {
            record = JsonSerializer.Deserialize<AuthoritativeStateRecord>(
                text,
                new JsonSerializerOptions { PropertyNamingPolicy = null });
            return record is not null;
        }
        catch (JsonException)
        {
            record = null;
            return false;
        }
    }

    public static string CommitIdentity(AuthoritativeStateRecord record)
    {
        var canonical = $"{record.Key}\u001F{record.StateVersion}\u001F{record.RecordDigest}";
        return "state-commit/sha256/" +
            Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(canonical)));
    }
}
