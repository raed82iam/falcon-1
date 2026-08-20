using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Foundation.State;

namespace Foundation.Evidence;

public sealed class FileEvidenceJournalProvider
{
    private const string JournalFileName = "journal.ndjson";
    private const string HeadFileName = "journal.head";
    private const string FactsFileName = "accepted-facts.ndjson";
    private const string CompletionBlocksFileName = "evidence-completion-blocks.ndjson";
    private const string CompletionBlocksHeadFileName = "evidence-completion-blocks.head";
    private const string CompletionBlocksAnchorFileName = "evidence-completion-blocks.anchor";

    private const string CompletionBlockAnchorNamespace = "foundation.evidence";
    private const string CompletionBlockAnchorSubject = "evidence-completion-block-store";

    private readonly string _root;
    private readonly FileAuthoritativeStateProvider _completionBlockAnchorState;
    private readonly object _sync = new();

    public FileEvidenceJournalProvider(
        string root,
        string? trustedCompletionBlockAnchorRoot = null)
    {
        if (string.IsNullOrWhiteSpace(root))
        {
            throw new ArgumentException("Evidence root is required.", nameof(root));
        }

        _root = Path.GetFullPath(root);
        var anchorRoot = string.IsNullOrWhiteSpace(trustedCompletionBlockAnchorRoot)
            ? ResolveDefaultTrustedAnchorRoot(_root)
            : Path.GetFullPath(trustedCompletionBlockAnchorRoot);
        _completionBlockAnchorState = new FileAuthoritativeStateProvider(anchorRoot);
    }

    public EvidenceJournalReadResult ReadJournal()
    {
        lock (_sync)
        {
            return ReadJournalUnsafe();
        }
    }

    public EvidenceAppendResult Append(EvidenceAppendRequest request)
    {
        lock (_sync)
        {
            if (!IsValid(request))
            {
                return Reject(EvidenceJournalClassification.Malformed, "INVALID_EVIDENCE_REQUEST");
            }

            Directory.CreateDirectory(_root);
            var read = ReadJournalUnsafe();
            if (read.Classification != EvidenceJournalClassification.Accepted &&
                read.Classification != EvidenceJournalClassification.Missing)
            {
                return Reject(read.Classification, read.Reason);
            }

            var records = read.Records;
            var canonicalEvidenceId =
                EvidenceCanonicalEncoding.DeterministicEvidenceId(request);

            if (!string.IsNullOrWhiteSpace(request.EvidenceId) &&
                !string.Equals(
                    request.EvidenceId,
                    canonicalEvidenceId,
                    StringComparison.Ordinal))
            {
                return Reject(
                    EvidenceJournalClassification.Conflicting,
                    "EVIDENCE_ID_CANONICAL_MISMATCH");
            }

            var evidenceId = canonicalEvidenceId;

            var duplicate = records.FirstOrDefault(record =>
                string.Equals(record.EvidenceId, evidenceId, StringComparison.Ordinal));

            if (duplicate is not null)
            {
                var candidate = CreateRecord(
                    request with { EvidenceId = evidenceId },
                    duplicate.Sequence,
                    duplicate.PreviousRecordDigest);

                if (string.Equals(
                        candidate.RecordDigest,
                        duplicate.RecordDigest,
                        StringComparison.Ordinal))
                {
                    return new EvidenceAppendResult(
                        EvidenceJournalClassification.Accepted,
                        "EVIDENCE_ALREADY_APPENDED",
                        duplicate,
                        true);
                }

                return Reject(
                    EvidenceJournalClassification.Conflicting,
                    "EVIDENCE_ID_CONTENT_CONFLICT");
            }

            if (!string.IsNullOrWhiteSpace(request.CorrectionOfEvidenceId) &&
                !records.Any(record => string.Equals(
                    record.EvidenceId,
                    request.CorrectionOfEvidenceId,
                    StringComparison.Ordinal)))
            {
                return Reject(
                    EvidenceJournalClassification.Conflicting,
                    "CORRECTION_TARGET_NOT_FOUND");
            }

            var sequence = records.Count;
            var previousDigest = records.Count == 0
                ? string.Empty
                : records[^1].RecordDigest;
            var record = CreateRecord(
                request with { EvidenceId = evidenceId },
                sequence,
                previousDigest);

            var journalPath = Path.Combine(_root, JournalFileName);
            var headPath = Path.Combine(_root, HeadFileName);

            File.AppendAllText(
                journalPath,
                EvidenceCanonicalEncoding.SerializeRecord(record) + Environment.NewLine);

            var head = new EvidenceJournalHead(
                sequence + 1,
                record.RecordDigest,
                string.Empty).WithComputedDigest();

            WriteReplace(headPath, EvidenceCanonicalEncoding.SerializeHead(head));

            var verification = ReadJournalUnsafe();
            if (!verification.Accepted ||
                verification.Records.Count != sequence + 1 ||
                !string.Equals(
                    verification.Records[^1].RecordDigest,
                    record.RecordDigest,
                    StringComparison.Ordinal))
            {
                return Reject(verification.Classification, verification.Reason);
            }

            return new EvidenceAppendResult(
                EvidenceJournalClassification.Accepted,
                "EVIDENCE_APPENDED",
                record,
                false);
        }
    }

    internal AcceptedFactPublishResult AppendAcceptedFact(AcceptedFactEvent fact)
    {
        lock (_sync)
        {
            if (string.IsNullOrWhiteSpace(fact.FactId) ||
                string.IsNullOrWhiteSpace(fact.EvidenceId) ||
                string.IsNullOrWhiteSpace(fact.StateNamespace) ||
                string.IsNullOrWhiteSpace(fact.SubjectId) ||
                fact.StateVersion < 0 ||
                string.IsNullOrWhiteSpace(fact.StateDigest) ||
                string.IsNullOrWhiteSpace(fact.DurableCommitIdentity))
            {
                return new AcceptedFactPublishResult(
                    EvidenceJournalClassification.Malformed,
                    "INVALID_ACCEPTED_FACT",
                    null,
                    false);
            }

            Directory.CreateDirectory(_root);

            var journal = ReadJournalUnsafe();
            if (!journal.Accepted)
            {
                return new AcceptedFactPublishResult(
                    journal.Classification,
                    journal.Reason,
                    null,
                    false);
            }

            var evidence = journal.Records.FirstOrDefault(record =>
                string.Equals(
                    record.EvidenceId,
                    fact.EvidenceId,
                    StringComparison.Ordinal));

            if (evidence is null)
            {
                return new AcceptedFactPublishResult(
                    EvidenceJournalClassification.Conflicting,
                    "ACCEPTED_FACT_EVIDENCE_NOT_FOUND",
                    null,
                    false);
            }

            if (evidence.Decision != EvidenceDecisionKind.Allow ||
                evidence.ExecutionOutcome != EvidenceExecutionOutcome.Accepted ||
                evidence.PersistenceOutcome != EvidencePersistenceOutcome.Accepted ||
                !string.Equals(
                    evidence.StateNamespace,
                    fact.StateNamespace,
                    StringComparison.Ordinal) ||
                !string.Equals(
                    evidence.SubjectId,
                    fact.SubjectId,
                    StringComparison.Ordinal) ||
                evidence.StateVersion != fact.StateVersion ||
                !string.Equals(
                    evidence.StateDigest,
                    fact.StateDigest,
                    StringComparison.Ordinal))
            {
                return new AcceptedFactPublishResult(
                    EvidenceJournalClassification.Conflicting,
                    "ACCEPTED_FACT_EVIDENCE_BINDING_MISMATCH",
                    null,
                    false);
            }

            var expectedFactId = EvidenceCanonicalEncoding.DeterministicFactId(
                fact.EvidenceId,
                fact.FactKind,
                fact.StateNamespace,
                fact.SubjectId,
                fact.StateVersion,
                fact.StateDigest,
                fact.DurableCommitIdentity);

            if (!string.Equals(
                    fact.FactId,
                    expectedFactId,
                    StringComparison.Ordinal))
            {
                return new AcceptedFactPublishResult(
                    EvidenceJournalClassification.Conflicting,
                    "ACCEPTED_FACT_ID_CANONICAL_MISMATCH",
                    null,
                    false);
            }

            var facts = ReadFactsUnsafe(out var classification, out var reason);
            if (classification != EvidenceJournalClassification.Accepted &&
                classification != EvidenceJournalClassification.Missing)
            {
                return new AcceptedFactPublishResult(
                    classification,
                    reason,
                    null,
                    false);
            }

            var computed = fact.WithComputedDigest();
            var existing = facts.FirstOrDefault(item =>
                string.Equals(item.FactId, computed.FactId, StringComparison.Ordinal));

            if (existing is not null)
            {
                if (string.Equals(
                        existing.FactDigest,
                        computed.FactDigest,
                        StringComparison.Ordinal))
                {
                    return new AcceptedFactPublishResult(
                        EvidenceJournalClassification.Accepted,
                        "ACCEPTED_FACT_ALREADY_PUBLISHED",
                        existing,
                        true);
                }

                return new AcceptedFactPublishResult(
                    EvidenceJournalClassification.Conflicting,
                    "ACCEPTED_FACT_ID_CONTENT_CONFLICT",
                    null,
                    false);
            }

            File.AppendAllText(
                Path.Combine(_root, FactsFileName),
                EvidenceCanonicalEncoding.SerializeFact(computed) + Environment.NewLine);

            return new AcceptedFactPublishResult(
                EvidenceJournalClassification.Accepted,
                "ACCEPTED_FACT_PUBLISHED",
                computed,
                false);
        }
    }

    internal EvidenceCompletionBlockResult AppendEvidenceCompletionBlock(
        string subjectId,
        string reason)
    {
        lock (_sync)
        {
            if (string.IsNullOrWhiteSpace(subjectId) ||
                string.IsNullOrWhiteSpace(reason))
            {
                return new EvidenceCompletionBlockResult(
                    EvidenceJournalClassification.Malformed,
                    "INVALID_EVIDENCE_COMPLETION_BLOCK",
                    null,
                    false);
            }

            Directory.CreateDirectory(_root);
            var blocks = ReadEvidenceCompletionBlocksUnsafe(
                out var classification,
                out var readReason,
                out var generation);

            if (classification != EvidenceJournalClassification.Accepted &&
                classification != EvidenceJournalClassification.Missing)
            {
                return new EvidenceCompletionBlockResult(
                    classification,
                    readReason,
                    null,
                    false);
            }

            var candidate = new EvidenceCompletionBlock(
                subjectId,
                reason,
                string.Empty).WithComputedDigest();
            var existing = blocks.FirstOrDefault(block =>
                string.Equals(
                    block.SubjectId,
                    subjectId,
                    StringComparison.Ordinal));

            if (existing is not null)
            {
                return new EvidenceCompletionBlockResult(
                    EvidenceJournalClassification.Accepted,
                    "EVIDENCE_COMPLETION_ALREADY_BLOCKED",
                    existing,
                    true);
            }

            var updated = blocks.Concat(new[] { candidate }).ToArray();
            var nextGeneration = generation + 1;
            var aggregateDigest =
                EvidenceCanonicalEncoding.ComputeCompletionBlockAggregateDigest(updated);
            var head = new EvidenceCompletionBlockHead(
                nextGeneration,
                updated.Length,
                aggregateDigest,
                string.Empty).WithComputedDigest();
            var anchor = new EvidenceCompletionBlockAnchor(
                nextGeneration,
                updated.Length,
                aggregateDigest,
                string.Empty).WithComputedDigest();

            var dataPath = Path.Combine(_root, CompletionBlocksFileName);
            var headPath = Path.Combine(_root, CompletionBlocksHeadFileName);
            var anchorPath = Path.Combine(_root, CompletionBlocksAnchorFileName);

            WriteReplace(
                dataPath,
                string.Join(
                    Environment.NewLine,
                    updated.Select(EvidenceCanonicalEncoding.SerializeCompletionBlock)) +
                Environment.NewLine);
            WriteReplace(
                headPath,
                EvidenceCanonicalEncoding.SerializeCompletionBlockHead(head));
            WriteReplace(
                anchorPath,
                EvidenceCanonicalEncoding.SerializeCompletionBlockAnchor(anchor));

            var trustedAnchorWrite = PersistTrustedCompletionBlockAnchor(anchor);
            if (!trustedAnchorWrite.Accepted)
            {
                return new EvidenceCompletionBlockResult(
                    EvidenceJournalClassification.Conflicting,
                    "TRUSTED_COMPLETION_BLOCK_ANCHOR_REJECTED:" + trustedAnchorWrite.Reason,
                    null,
                    false);
            }

            var verification = ReadEvidenceCompletionBlocksUnsafe(
                out classification,
                out readReason,
                out var verifiedGeneration);

            if (classification != EvidenceJournalClassification.Accepted ||
                verifiedGeneration != nextGeneration ||
                !verification.Any(block => string.Equals(
                    block.SubjectId,
                    subjectId,
                    StringComparison.Ordinal)))
            {
                return new EvidenceCompletionBlockResult(
                    classification,
                    readReason,
                    null,
                    false);
            }

            return new EvidenceCompletionBlockResult(
                EvidenceJournalClassification.Accepted,
                "EVIDENCE_COMPLETION_BLOCK_RECORDED",
                candidate,
                false);
        }
    }

    public IReadOnlyList<EvidenceCompletionBlock> ReadEvidenceCompletionBlocks()
    {
        lock (_sync)
        {
            var blocks = ReadEvidenceCompletionBlocksUnsafe(
                out var classification,
                out var reason,
                out _);

            if (classification != EvidenceJournalClassification.Accepted &&
                classification != EvidenceJournalClassification.Missing)
            {
                throw new InvalidDataException(reason);
            }

            return blocks;
        }
    }

    public IReadOnlyList<AcceptedFactEvent> ReadAcceptedFacts()
    {
        lock (_sync)
        {
            var facts = ReadFactsUnsafe(out var classification, out var reason);
            if (classification != EvidenceJournalClassification.Accepted &&
                classification != EvidenceJournalClassification.Missing)
            {
                throw new InvalidDataException(reason);
            }

            return facts;
        }
    }

    private IReadOnlyList<EvidenceCompletionBlock> ReadEvidenceCompletionBlocksUnsafe(
        out EvidenceJournalClassification classification,
        out string reason,
        out long generation)
    {
        generation = 0;
        var dataPath = Path.Combine(_root, CompletionBlocksFileName);
        var headPath = Path.Combine(_root, CompletionBlocksHeadFileName);
        var anchorPath = Path.Combine(_root, CompletionBlocksAnchorFileName);

        var dataExists = File.Exists(dataPath);
        var headExists = File.Exists(headPath);
        var anchorExists = File.Exists(anchorPath);
        var trustedAnchor = _completionBlockAnchorState.ReadCurrent(
            CompletionBlockAnchorNamespace,
            CompletionBlockAnchorSubject,
            FoundationStateClass.PersistenceCommitState);

        if (!dataExists && !headExists && !anchorExists)
        {
            if (trustedAnchor.Classification == DurableStateClassification.Missing)
            {
                classification = EvidenceJournalClassification.Missing;
                reason = "EVIDENCE_COMPLETION_BLOCK_STORE_MISSING";
                return Array.Empty<EvidenceCompletionBlock>();
            }

            classification = EvidenceJournalClassification.Conflicting;
            reason = "EVIDENCE_COMPLETION_BLOCK_STORE_DELETED_WITH_TRUSTED_ANCHOR";
            return Array.Empty<EvidenceCompletionBlock>();
        }

        if (!trustedAnchor.Accepted || trustedAnchor.Current is null)
        {
            classification = EvidenceJournalClassification.Partial;
            reason = "TRUSTED_COMPLETION_BLOCK_ANCHOR_MISSING_OR_INVALID:" + trustedAnchor.Reason;
            return Array.Empty<EvidenceCompletionBlock>();
        }

        if (!(dataExists && headExists && anchorExists))
        {
            classification = EvidenceJournalClassification.Partial;
            reason = "EVIDENCE_COMPLETION_BLOCK_STORE_PARTIAL";
            return Array.Empty<EvidenceCompletionBlock>();
        }

        string[] lines;
        string headText;
        string anchorText;
        try
        {
            lines = File.ReadAllLines(dataPath);
            headText = File.ReadAllText(headPath);
            anchorText = File.ReadAllText(anchorPath);
        }
        catch (IOException)
        {
            classification = EvidenceJournalClassification.Corrupted;
            reason = "EVIDENCE_COMPLETION_BLOCK_STORE_UNREADABLE";
            return Array.Empty<EvidenceCompletionBlock>();
        }

        if (!EvidenceCanonicalEncoding.TryDeserializeCompletionBlockHead(
                headText,
                out var head) ||
            head is null ||
            !string.Equals(
                head.HeadDigest,
                head.WithComputedDigest().HeadDigest,
                StringComparison.Ordinal))
        {
            classification = EvidenceJournalClassification.Corrupted;
            reason = "EVIDENCE_COMPLETION_BLOCK_HEAD_CORRUPTED";
            return Array.Empty<EvidenceCompletionBlock>();
        }

        if (!EvidenceCanonicalEncoding.TryDeserializeCompletionBlockAnchor(
                anchorText,
                out var anchor) ||
            anchor is null ||
            !string.Equals(
                anchor.AnchorDigest,
                anchor.WithComputedDigest().AnchorDigest,
                StringComparison.Ordinal))
        {
            classification = EvidenceJournalClassification.Corrupted;
            reason = "EVIDENCE_COMPLETION_BLOCK_ANCHOR_CORRUPTED";
            return Array.Empty<EvidenceCompletionBlock>();
        }

        if (head.Generation != anchor.Generation ||
            head.RecordCount != anchor.RecordCount ||
            !string.Equals(
                head.AggregateDigest,
                anchor.AggregateDigest,
                StringComparison.Ordinal))
        {
            classification = EvidenceJournalClassification.Conflicting;
            reason = "EVIDENCE_COMPLETION_BLOCK_HEAD_ANCHOR_MISMATCH";
            return Array.Empty<EvidenceCompletionBlock>();
        }

        var trustedPayload = SerializeTrustedCompletionBlockAnchor(anchor);
        if (!string.Equals(
                trustedAnchor.Current.Payload,
                trustedPayload,
                StringComparison.Ordinal))
        {
            classification = EvidenceJournalClassification.Conflicting;
            reason = "TRUSTED_COMPLETION_BLOCK_ANCHOR_MISMATCH";
            return Array.Empty<EvidenceCompletionBlock>();
        }

        var blocks = new List<EvidenceCompletionBlock>();
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line) ||
                !EvidenceCanonicalEncoding.TryDeserializeCompletionBlock(
                    line,
                    out var block) ||
                block is null ||
                string.IsNullOrWhiteSpace(block.SubjectId) ||
                string.IsNullOrWhiteSpace(block.Reason))
            {
                classification = EvidenceJournalClassification.Malformed;
                reason = "EVIDENCE_COMPLETION_BLOCK_MALFORMED";
                return Array.Empty<EvidenceCompletionBlock>();
            }

            if (!string.Equals(
                    block.BlockDigest,
                    EvidenceCanonicalEncoding.ComputeCompletionBlockDigest(block),
                    StringComparison.Ordinal))
            {
                classification = EvidenceJournalClassification.Corrupted;
                reason = "EVIDENCE_COMPLETION_BLOCK_CORRUPTED";
                return Array.Empty<EvidenceCompletionBlock>();
            }

            if (blocks.Any(existing => string.Equals(
                    existing.SubjectId,
                    block.SubjectId,
                    StringComparison.Ordinal)))
            {
                classification = EvidenceJournalClassification.Conflicting;
                reason = "EVIDENCE_COMPLETION_BLOCK_DUPLICATE_SUBJECT";
                return Array.Empty<EvidenceCompletionBlock>();
            }

            blocks.Add(block);
        }

        var aggregateDigest =
            EvidenceCanonicalEncoding.ComputeCompletionBlockAggregateDigest(blocks);

        if (head.RecordCount != blocks.Count ||
            !string.Equals(
                head.AggregateDigest,
                aggregateDigest,
                StringComparison.Ordinal))
        {
            classification = EvidenceJournalClassification.Corrupted;
            reason = "EVIDENCE_COMPLETION_BLOCK_STORE_TRUNCATED_OR_ROLLED_BACK";
            return Array.Empty<EvidenceCompletionBlock>();
        }

        generation = head.Generation;
        classification = EvidenceJournalClassification.Accepted;
        reason = "EVIDENCE_COMPLETION_BLOCKS_LOADED";
        return blocks;
    }

    private EvidenceJournalReadResult ReadJournalUnsafe()
    {
        var journalPath = Path.Combine(_root, JournalFileName);
        var headPath = Path.Combine(_root, HeadFileName);
        var journalExists = File.Exists(journalPath);
        var headExists = File.Exists(headPath);

        if (!journalExists && !headExists)
        {
            return new EvidenceJournalReadResult(
                EvidenceJournalClassification.Missing,
                "JOURNAL_MISSING",
                Array.Empty<IntegrityLinkedEvidenceRecord>(),
                null);
        }

        if (journalExists != headExists)
        {
            return new EvidenceJournalReadResult(
                EvidenceJournalClassification.Partial,
                "JOURNAL_LAYOUT_PARTIAL",
                Array.Empty<IntegrityLinkedEvidenceRecord>(),
                null);
        }

        string[] lines;
        try
        {
            lines = File.ReadAllLines(journalPath);
        }
        catch (IOException)
        {
            return ReadReject(EvidenceJournalClassification.Corrupted, "JOURNAL_UNREADABLE");
        }

        if (!EvidenceCanonicalEncoding.TryDeserializeHead(
                File.ReadAllText(headPath),
                out var head) ||
            head is null ||
            !string.Equals(
                head.HeadDigest,
                head.WithComputedDigest().HeadDigest,
                StringComparison.Ordinal))
        {
            return ReadReject(EvidenceJournalClassification.Corrupted, "JOURNAL_HEAD_CORRUPTED");
        }

        var records = new List<IntegrityLinkedEvidenceRecord>();
        var expectedPrevious = string.Empty;

        for (var index = 0; index < lines.Length; index++)
        {
            if (string.IsNullOrWhiteSpace(lines[index]) ||
                !EvidenceCanonicalEncoding.TryDeserializeRecord(lines[index], out var record) ||
                record is null)
            {
                return ReadReject(EvidenceJournalClassification.Malformed, "JOURNAL_RECORD_MALFORMED");
            }

            if (record.Sequence != index)
            {
                return ReadReject(EvidenceJournalClassification.Conflicting, "JOURNAL_SEQUENCE_CONFLICT");
            }

            if (!string.Equals(
                    record.PreviousRecordDigest,
                    expectedPrevious,
                    StringComparison.Ordinal))
            {
                return ReadReject(EvidenceJournalClassification.Conflicting, "JOURNAL_LINK_CONFLICT");
            }

            if (!string.Equals(
                    record.RecordDigest,
                    record.WithComputedDigest().RecordDigest,
                    StringComparison.Ordinal))
            {
                return ReadReject(EvidenceJournalClassification.Corrupted, "JOURNAL_RECORD_CORRUPTED");
            }

            if (records.Any(existing => string.Equals(
                    existing.EvidenceId,
                    record.EvidenceId,
                    StringComparison.Ordinal)))
            {
                return ReadReject(EvidenceJournalClassification.Conflicting, "JOURNAL_DUPLICATE_EVIDENCE_ID");
            }

            records.Add(record);
            expectedPrevious = record.RecordDigest;
        }

        if (head.RecordCount > records.Count)
        {
            return ReadReject(EvidenceJournalClassification.Truncated, "JOURNAL_TRUNCATED");
        }

        if (head.RecordCount < records.Count)
        {
            return ReadReject(EvidenceJournalClassification.Conflicting, "JOURNAL_INSERTION_DETECTED");
        }

        var actualLastDigest = records.Count == 0 ? string.Empty : records[^1].RecordDigest;
        if (!string.Equals(head.LastRecordDigest, actualLastDigest, StringComparison.Ordinal))
        {
            return ReadReject(EvidenceJournalClassification.Conflicting, "JOURNAL_HEAD_MISMATCH");
        }

        return new EvidenceJournalReadResult(
            EvidenceJournalClassification.Accepted,
            "JOURNAL_VALIDATED",
            records,
            head);
    }

    private DurableStateWriteResult PersistTrustedCompletionBlockAnchor(
        EvidenceCompletionBlockAnchor anchor)
    {
        var existing = _completionBlockAnchorState.ReadCurrent(
            CompletionBlockAnchorNamespace,
            CompletionBlockAnchorSubject,
            FoundationStateClass.PersistenceCommitState);
        var expectedVersion = existing.Current?.StateVersion ?? -1;
        var previousDigest = existing.Current?.RecordDigest ?? string.Empty;
        var record = new AuthoritativeStateRecord(
            $"evidence-completion-anchor:{anchor.Generation}",
            CompletionBlockAnchorNamespace,
            CompletionBlockAnchorSubject,
            FoundationStateClass.PersistenceCommitState,
            StateRepresentationKind.Authoritative,
            "Foundation.Evidence",
            "Foundation.Evidence.FileEvidenceJournalProvider",
            "Foundation.State.FileAuthoritativeStateProvider",
            "Foundation.Evidence.FileEvidenceJournalProvider",
            anchor.AnchorDigest,
            expectedVersion + 1,
            DateTimeOffset.UnixEpoch,
            "IMMUTABLE_AUDIT_CONTROL",
            SerializeTrustedCompletionBlockAnchor(anchor),
            previousDigest,
            string.Empty).WithComputedDigest();

        return _completionBlockAnchorState.WriteCurrent(record, expectedVersion);
    }

    private static string SerializeTrustedCompletionBlockAnchor(
        EvidenceCompletionBlockAnchor anchor)
        => string.Join(
            "|",
            anchor.Generation,
            anchor.RecordCount,
            anchor.AggregateDigest,
            anchor.AnchorDigest);

    private static string ResolveDefaultTrustedAnchorRoot(string evidenceRoot)
    {
        var parent = Path.GetDirectoryName(evidenceRoot) ?? evidenceRoot;
        var identity = Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(evidenceRoot)));
        return Path.Combine(parent, ".falcon-trusted-evidence-anchors", identity);
    }

    private IReadOnlyList<AcceptedFactEvent> ReadFactsUnsafe(
        out EvidenceJournalClassification classification,
        out string reason)
    {
        var path = Path.Combine(_root, FactsFileName);
        if (!File.Exists(path))
        {
            classification = EvidenceJournalClassification.Missing;
            reason = "ACCEPTED_FACTS_MISSING";
            return Array.Empty<AcceptedFactEvent>();
        }

        var facts = new List<AcceptedFactEvent>();
        foreach (var line in File.ReadAllLines(path))
        {
            if (string.IsNullOrWhiteSpace(line) ||
                !EvidenceCanonicalEncoding.TryDeserializeFact(line, out var fact) ||
                fact is null ||
                !string.Equals(
                    fact.FactDigest,
                    fact.WithComputedDigest().FactDigest,
                    StringComparison.Ordinal))
            {
                classification = EvidenceJournalClassification.Corrupted;
                reason = "ACCEPTED_FACTS_CORRUPTED";
                return Array.Empty<AcceptedFactEvent>();
            }

            if (facts.Any(existing => string.Equals(
                    existing.FactId,
                    fact.FactId,
                    StringComparison.Ordinal)))
            {
                classification = EvidenceJournalClassification.Conflicting;
                reason = "ACCEPTED_FACT_DUPLICATE";
                return Array.Empty<AcceptedFactEvent>();
            }

            facts.Add(fact);
        }

        classification = EvidenceJournalClassification.Accepted;
        reason = "ACCEPTED_FACTS_VALIDATED";
        return facts;
    }

    private static IntegrityLinkedEvidenceRecord CreateRecord(
        EvidenceAppendRequest request,
        long sequence,
        string previousDigest)
        => new IntegrityLinkedEvidenceRecord(
            sequence,
            request.EvidenceId,
            request.ActorIdentity,
            request.RequestIdentity,
            request.Decision,
            request.DecisionIdentity,
            request.Reason,
            request.ExecutionOutcome,
            request.PersistenceOutcome,
            request.StateNamespace,
            request.SubjectId,
            request.StateVersion,
            request.StateDigest,
            request.SourceIdentity,
            request.CorrectionOfEvidenceId,
            previousDigest,
            string.Empty).WithComputedDigest();

    private static bool IsValid(EvidenceAppendRequest request)
        => !string.IsNullOrWhiteSpace(request.ActorIdentity) &&
           !string.IsNullOrWhiteSpace(request.RequestIdentity) &&
           !string.IsNullOrWhiteSpace(request.DecisionIdentity) &&
           !string.IsNullOrWhiteSpace(request.Reason) &&
           !string.IsNullOrWhiteSpace(request.SourceIdentity) &&
           request.StateVersion >= -1;

    private static void WriteReplace(string path, string content)
    {
        var temporary = path + ".tmp";
        File.WriteAllText(temporary, content);
        File.Move(temporary, path, true);
    }

    private static EvidenceAppendResult Reject(
        EvidenceJournalClassification classification,
        string reason)
        => new(classification, reason, null, false);

    private static EvidenceJournalReadResult ReadReject(
        EvidenceJournalClassification classification,
        string reason)
        => new(
            classification,
            reason,
            Array.Empty<IntegrityLinkedEvidenceRecord>(),
            null);
}
