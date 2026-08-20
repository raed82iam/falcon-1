using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;

namespace Foundation.Evidence;

public enum EvidenceDecisionKind
{
    Allow,
    Deny
}

public enum EvidenceExecutionOutcome
{
    Accepted,
    Rejected
}

public enum EvidencePersistenceOutcome
{
    Accepted,
    Rejected,
    NotAttempted
}

public enum EvidenceJournalClassification
{
    Accepted,
    Missing,
    Partial,
    Malformed,
    Corrupted,
    Conflicting,
    Truncated
}

public sealed record EvidenceAppendRequest(
    string EvidenceId,
    string ActorIdentity,
    string RequestIdentity,
    EvidenceDecisionKind Decision,
    string DecisionIdentity,
    string Reason,
    EvidenceExecutionOutcome ExecutionOutcome,
    EvidencePersistenceOutcome PersistenceOutcome,
    string StateNamespace,
    string SubjectId,
    long StateVersion,
    string StateDigest,
    string SourceIdentity,
    string CorrectionOfEvidenceId);

public sealed record IntegrityLinkedEvidenceRecord(
    long Sequence,
    string EvidenceId,
    string ActorIdentity,
    string RequestIdentity,
    EvidenceDecisionKind Decision,
    string DecisionIdentity,
    string Reason,
    EvidenceExecutionOutcome ExecutionOutcome,
    EvidencePersistenceOutcome PersistenceOutcome,
    string StateNamespace,
    string SubjectId,
    long StateVersion,
    string StateDigest,
    string SourceIdentity,
    string CorrectionOfEvidenceId,
    string PreviousRecordDigest,
    string RecordDigest)
{
    public IntegrityLinkedEvidenceRecord WithComputedDigest()
        => this with { RecordDigest = EvidenceCanonicalEncoding.ComputeRecordDigest(this) };
}

public sealed record EvidenceJournalHead(
    long RecordCount,
    string LastRecordDigest,
    string HeadDigest)
{
    public EvidenceJournalHead WithComputedDigest()
        => this with { HeadDigest = EvidenceCanonicalEncoding.ComputeHeadDigest(this) };
}

public sealed record EvidenceJournalReadResult(
    EvidenceJournalClassification Classification,
    string Reason,
    System.Collections.Generic.IReadOnlyList<IntegrityLinkedEvidenceRecord> Records,
    EvidenceJournalHead? Head)
{
    public bool Accepted => Classification == EvidenceJournalClassification.Accepted;
}

public sealed record EvidenceAppendResult(
    EvidenceJournalClassification Classification,
    string Reason,
    IntegrityLinkedEvidenceRecord? Record,
    bool Idempotent)
{
    public bool Accepted => Classification == EvidenceJournalClassification.Accepted;
}

public sealed record AcceptedFactEvent(
    string FactId,
    string EvidenceId,
    string FactKind,
    string StateNamespace,
    string SubjectId,
    long StateVersion,
    string StateDigest,
    string DurableCommitIdentity,
    string FactDigest)
{
    public AcceptedFactEvent WithComputedDigest()
        => this with { FactDigest = EvidenceCanonicalEncoding.ComputeFactDigest(this) };
}

public sealed record AcceptedFactPublishResult(
    EvidenceJournalClassification Classification,
    string Reason,
    AcceptedFactEvent? Fact,
    bool Idempotent)
{
    public bool Accepted => Classification == EvidenceJournalClassification.Accepted;
}


public sealed record EvidenceCompletionBlock(
    string SubjectId,
    string Reason,
    string BlockDigest)
{
    public EvidenceCompletionBlock WithComputedDigest()
        => this with
        {
            BlockDigest = EvidenceCanonicalEncoding.ComputeCompletionBlockDigest(this)
        };
}



public sealed record EvidenceCompletionBlockHead(
    long Generation,
    int RecordCount,
    string AggregateDigest,
    string HeadDigest)
{
    public EvidenceCompletionBlockHead WithComputedDigest()
        => this with
        {
            HeadDigest = EvidenceCanonicalEncoding.ComputeCompletionBlockHeadDigest(this)
        };
}

public sealed record EvidenceCompletionBlockAnchor(
    long Generation,
    int RecordCount,
    string AggregateDigest,
    string AnchorDigest)
{
    public EvidenceCompletionBlockAnchor WithComputedDigest()
        => this with
        {
            AnchorDigest = EvidenceCanonicalEncoding.ComputeCompletionBlockAnchorDigest(this)
        };
}

public sealed record EvidenceCompletionBlockResult(
    EvidenceJournalClassification Classification,
    string Reason,
    EvidenceCompletionBlock? Block,
    bool Idempotent)
{
    public bool Accepted => Classification == EvidenceJournalClassification.Accepted;
}

public static class EvidenceCanonicalEncoding
{
    private const char Separator = '\u001F';

    public static string ComputeRecordDigest(IntegrityLinkedEvidenceRecord record)
    {
        var canonical = string.Join(
            Separator,
            record.Sequence.ToString(CultureInfo.InvariantCulture),
            record.EvidenceId,
            record.ActorIdentity,
            record.RequestIdentity,
            record.Decision.ToString(),
            record.DecisionIdentity,
            record.Reason,
            record.ExecutionOutcome.ToString(),
            record.PersistenceOutcome.ToString(),
            record.StateNamespace,
            record.SubjectId,
            record.StateVersion.ToString(CultureInfo.InvariantCulture),
            record.StateDigest,
            record.SourceIdentity,
            record.CorrectionOfEvidenceId,
            record.PreviousRecordDigest);

        return Hash(canonical);
    }

    public static string ComputeHeadDigest(EvidenceJournalHead head)
        => Hash(string.Join(
            Separator,
            head.RecordCount.ToString(CultureInfo.InvariantCulture),
            head.LastRecordDigest));

    public static string ComputeFactDigest(AcceptedFactEvent fact)
        => Hash(string.Join(
            Separator,
            fact.FactId,
            fact.EvidenceId,
            fact.FactKind,
            fact.StateNamespace,
            fact.SubjectId,
            fact.StateVersion.ToString(CultureInfo.InvariantCulture),
            fact.StateDigest,
            fact.DurableCommitIdentity));

    public static string DeterministicEvidenceId(EvidenceAppendRequest request)
    {
        var canonical = string.Join(
            Separator,
            request.ActorIdentity,
            request.RequestIdentity,
            request.Decision.ToString(),
            request.DecisionIdentity,
            request.Reason,
            request.ExecutionOutcome.ToString(),
            request.PersistenceOutcome.ToString(),
            request.StateNamespace,
            request.SubjectId,
            request.StateVersion.ToString(CultureInfo.InvariantCulture),
            request.StateDigest,
            request.SourceIdentity,
            request.CorrectionOfEvidenceId);

        return "evidence/sha256/" + Hash(canonical);
    }

    public static string DeterministicFactId(
        string evidenceId,
        string factKind,
        string stateNamespace,
        string subjectId,
        long stateVersion,
        string stateDigest,
        string durableCommitIdentity)
        => "accepted-fact/sha256/" + Hash(string.Join(
            Separator,
            evidenceId,
            factKind,
            stateNamespace,
            subjectId,
            stateVersion.ToString(CultureInfo.InvariantCulture),
            stateDigest,
            durableCommitIdentity));

    public static string ComputeCompletionBlockDigest(EvidenceCompletionBlock block)
        => Hash(string.Join(
            Separator,
            block.SubjectId,
            block.Reason));


    public static string ComputeCompletionBlockAggregateDigest(
        IEnumerable<EvidenceCompletionBlock> blocks)
        => Hash(string.Join(
            Separator,
            blocks.Select(block => block.BlockDigest)));

    public static string ComputeCompletionBlockHeadDigest(
        EvidenceCompletionBlockHead head)
        => Hash(string.Join(
            Separator,
            head.Generation.ToString(CultureInfo.InvariantCulture),
            head.RecordCount.ToString(CultureInfo.InvariantCulture),
            head.AggregateDigest));

    public static string ComputeCompletionBlockAnchorDigest(
        EvidenceCompletionBlockAnchor anchor)
        => Hash(string.Join(
            Separator,
            anchor.Generation.ToString(CultureInfo.InvariantCulture),
            anchor.RecordCount.ToString(CultureInfo.InvariantCulture),
            anchor.AggregateDigest));

    public static string SerializeCompletionBlock(EvidenceCompletionBlock block)
        => JsonSerializer.Serialize(block);

    public static string SerializeCompletionBlockHead(EvidenceCompletionBlockHead head)
        => JsonSerializer.Serialize(head);

    public static string SerializeCompletionBlockAnchor(EvidenceCompletionBlockAnchor anchor)
        => JsonSerializer.Serialize(anchor);

    public static bool TryDeserializeCompletionBlock(
        string text,
        out EvidenceCompletionBlock? block)
        => TryDeserialize(text, out block);

    public static bool TryDeserializeCompletionBlockHead(
        string text,
        out EvidenceCompletionBlockHead? head)
        => TryDeserialize(text, out head);

    public static bool TryDeserializeCompletionBlockAnchor(
        string text,
        out EvidenceCompletionBlockAnchor? anchor)
        => TryDeserialize(text, out anchor);

    public static string SerializeRecord(IntegrityLinkedEvidenceRecord record)
        => JsonSerializer.Serialize(record);

    public static string SerializeHead(EvidenceJournalHead head)
        => JsonSerializer.Serialize(head);

    public static string SerializeFact(AcceptedFactEvent fact)
        => JsonSerializer.Serialize(fact);

    public static bool TryDeserializeRecord(
        string text,
        out IntegrityLinkedEvidenceRecord? record)
        => TryDeserialize(text, out record);

    public static bool TryDeserializeHead(
        string text,
        out EvidenceJournalHead? head)
        => TryDeserialize(text, out head);

    public static bool TryDeserializeFact(
        string text,
        out AcceptedFactEvent? fact)
        => TryDeserialize(text, out fact);

    private static bool TryDeserialize<T>(string text, out T? value)
    {
        try
        {
            value = JsonSerializer.Deserialize<T>(text);
            return value is not null;
        }
        catch (JsonException)
        {
            value = default;
            return false;
        }
    }

    private static string Hash(string canonical)
        => Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(canonical)));
}
