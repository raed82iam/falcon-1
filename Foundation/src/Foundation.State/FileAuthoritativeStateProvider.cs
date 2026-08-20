using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace Foundation.State;

public sealed class FileAuthoritativeStateProvider : IAuthoritativeStateProvider, IAuthoritativeStateReconciliationProvider
{
    private const string CurrentFileName = "current.state";
    private const string HistoryDirectoryName = "history";
    private const string CommitDirectoryName = "commits";
    private const string CommitRecordsDirectoryName = "records";
    private const string CommitAnchorHistoryDirectoryName = ".commit-anchor-history";
    private const string CommitAnchorHeadsDirectoryName = ".commit-anchor-heads";
    private readonly string _root;
    private readonly string _independentAnchorRoot;
    private readonly PersistenceWriteInterruptionPoint _interruptionPoint;

    public FileAuthoritativeStateProvider(
        string root,
        PersistenceWriteInterruptionPoint interruptionPoint = PersistenceWriteInterruptionPoint.None)
        : this(root, ResolveDefaultIndependentAnchorRoot(root), interruptionPoint)
    {
    }

    public FileAuthoritativeStateProvider(
        string root,
        string independentAnchorRoot,
        PersistenceWriteInterruptionPoint interruptionPoint = PersistenceWriteInterruptionPoint.None)
    {
        if (string.IsNullOrWhiteSpace(root)) throw new ArgumentException("State root is required.", nameof(root));
        if (string.IsNullOrWhiteSpace(independentAnchorRoot))
            throw new ArgumentException("Independent anchor root is required.", nameof(independentAnchorRoot));

        _root = Path.GetFullPath(root);
        _independentAnchorRoot = Path.GetFullPath(independentAnchorRoot);
        if (string.Equals(_root, _independentAnchorRoot, StringComparison.OrdinalIgnoreCase) ||
            _independentAnchorRoot.StartsWith(_root + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Independent anchor root must be outside the state rollback root.", nameof(independentAnchorRoot));

        _interruptionPoint = interruptionPoint;
        Directory.CreateDirectory(_root);
        Directory.CreateDirectory(_independentAnchorRoot);
    }

    public DurableStateReadResult ReadCurrent(string stateNamespace, string subjectId, FoundationStateClass stateClass)
    {
        var directory = ResolveDirectory(stateNamespace, subjectId, stateClass);
        var currentPath = Path.Combine(directory, CurrentFileName);
        var historyDirectory = Path.Combine(directory, HistoryDirectoryName);
        if (!Directory.Exists(directory)) return ReadReject(DurableStateClassification.Missing, "STATE_NOT_FOUND");
        if (!File.Exists(currentPath) || !Directory.Exists(historyDirectory))
            return ReadReject(DurableStateClassification.Partial, "PARTIAL_STATE_LAYOUT");
        if (!TryReadEnvelope(currentPath, out var current))
            return ReadReject(DurableStateClassification.Corrupted, "CORRUPTED_CURRENT_STATE");
        if (!MatchesKey(current!, stateNamespace, subjectId, stateClass))
            return ReadReject(DurableStateClassification.Conflicting, "STATE_KEY_CONFLICT");
        var historyPath = HistoryPath(directory, current!.StateVersion);
        if (!File.Exists(historyPath)) return ReadReject(DurableStateClassification.Partial, "CURRENT_HISTORY_GAP");
        if (!TryReadEnvelope(historyPath, out var history) || history != current)
            return ReadReject(DurableStateClassification.Conflicting, "CURRENT_HISTORY_CONFLICT");
        return new DurableStateReadResult(DurableStateClassification.Accepted, "STATE_LOADED", current);
    }

    public DurableStateWriteResult WriteCurrent(AuthoritativeStateRecord record, long expectedVersion)
    {
        var writeKey = StateCanonicalEncoding.Key(record.Namespace, record.SubjectId, record.StateClass);
        var mutexName = "Global\\Falcon-State-" + Digest(writeKey);
        using var mutex = new Mutex(false, mutexName);
        var acquired = false;
        try
        {
            acquired = mutex.WaitOne();
            if (!acquired)
                return WriteReject(DurableStateClassification.LockUnavailable, "CROSS_PROCESS_WRITE_LOCK_UNAVAILABLE");
            return WriteCurrentLocked(record, expectedVersion);
        }
        catch (AbandonedMutexException)
        {
            acquired = true;
            return WriteCurrentLocked(record, expectedVersion);
        }
        catch (UnauthorizedAccessException)
        {
            return WriteReject(DurableStateClassification.LockUnavailable, "CROSS_PROCESS_WRITE_LOCK_UNAVAILABLE");
        }
        finally
        {
            if (acquired)
            {
                try { mutex.ReleaseMutex(); } catch (ApplicationException) { }
            }
        }
    }

    public DurableCommitLookupResult LookupCommitByRequest(string requestIdentity)
        => LookupCommit("request-" + Digest(requestIdentity) + ".index");

    public DurableCommitLookupResult LookupCommitByDecision(string decisionIdentity)
        => LookupCommit("decision-" + Digest(decisionIdentity) + ".index");

    public DurableHistoryReadResult ReadLatestTrustedHistory(string stateNamespace, string subjectId, FoundationStateClass stateClass)
    {
        var chain = ReadTrustedHistoryChain(stateNamespace, subjectId, stateClass);
        if (chain.Classification != DurableStateClassification.TrustedHistoryReconstructed || chain.Records.Count == 0)
            return new DurableHistoryReadResult(chain.Classification, chain.Reason, null);
        return new DurableHistoryReadResult(
            DurableStateClassification.TrustedHistoryReconstructed,
            "TRUSTED_HISTORY_RECONSTRUCTED",
            chain.Records[^1]);
    }

    public DurableHistoryReadResult ReadTrustedHistoryVersion(
        string stateNamespace,
        string subjectId,
        FoundationStateClass stateClass,
        long stateVersion,
        string stateDigest,
        string commitIdentity)
    {
        var chain = ReadTrustedHistoryChain(stateNamespace, subjectId, stateClass);
        if (chain.Classification != DurableStateClassification.TrustedHistoryReconstructed)
            return new DurableHistoryReadResult(chain.Classification, chain.Reason, null);

        var matches = chain.Records.Where(record =>
            record.StateVersion == stateVersion &&
            string.Equals(record.RecordDigest, stateDigest, StringComparison.Ordinal) &&
            string.Equals(StateCanonicalEncoding.CommitIdentity(record), commitIdentity, StringComparison.Ordinal)).ToArray();

        if (matches.Length == 0)
            return new DurableHistoryReadResult(DurableStateClassification.Missing, "BOUND_HISTORY_NOT_FOUND", null);
        if (matches.Length != 1)
            return new DurableHistoryReadResult(DurableStateClassification.Conflicting, "BOUND_HISTORY_AMBIGUOUS", null);
        return new DurableHistoryReadResult(
            DurableStateClassification.TrustedHistoryReconstructed,
            "BOUND_TRUSTED_HISTORY_RECONSTRUCTED",
            matches[0]);
    }

    private DurableStateWriteResult WriteCurrentLocked(AuthoritativeStateRecord record, long expectedVersion)
    {
        if (record.Representation != StateRepresentationKind.Authoritative)
            return WriteReject(DurableStateClassification.NonAuthoritativeRepresentation, "NON_AUTHORITATIVE_REPRESENTATION");

        var directory = ResolveDirectory(record.Namespace, record.SubjectId, record.StateClass);
        var historyDirectory = Path.Combine(directory, HistoryDirectoryName);
        var commitsDirectory = Path.Combine(directory, CommitDirectoryName);
        var existing = ReadCurrent(record.Namespace, record.SubjectId, record.StateClass);
        if (existing.Classification is not DurableStateClassification.Accepted and not DurableStateClassification.Missing)
            return WriteReject(existing.Classification, existing.Reason);
        Directory.CreateDirectory(historyDirectory);
        Directory.CreateDirectory(commitsDirectory);

        var historyPath = HistoryPath(directory, record.StateVersion);
        if (File.Exists(historyPath))
        {
            if (TryReadEnvelope(historyPath, out var duplicate) && duplicate == record)
                return new(DurableStateClassification.Accepted, "IDENTICAL_STATE_ALREADY_PERSISTED", duplicate, StateCanonicalEncoding.CommitIdentity(duplicate!));
            if (duplicate is not null && string.Equals(duplicate.SourceIdentity, record.SourceIdentity, StringComparison.Ordinal))
                return WriteReject(DurableStateClassification.Conflicting, "CONFLICTING_DUPLICATE_REQUEST");
        }

        var actualVersion = existing.Current?.StateVersion ?? -1;
        if (actualVersion != expectedVersion)
            return WriteReject(DurableStateClassification.StaleExpectedVersion, "STALE_EXPECTED_STATE_VERSION");
        if (File.Exists(historyPath))
            return WriteReject(DurableStateClassification.Conflicting, "IMMUTABLE_HISTORY_CONFLICT");
        if (record.StateVersion != expectedVersion + 1)
            return WriteReject(DurableStateClassification.Conflicting, "NON_SUCCESSOR_STATE_VERSION");
        var expectedPreviousDigest = existing.Current?.RecordDigest ?? string.Empty;
        if (!string.Equals(record.PreviousRecordDigest, expectedPreviousDigest, StringComparison.Ordinal))
            return WriteReject(DurableStateClassification.Conflicting, "PREVIOUS_STATE_DIGEST_MISMATCH");

        var commitIdentity = StateCanonicalEncoding.CommitIdentity(record);
        WriteCommit(commitsDirectory, record, commitIdentity, PersistenceCommitPhase.Prepared);
        Interrupt(PersistenceWriteInterruptionPoint.AfterPrepared);

        var envelope = BuildEnvelope(record);
        WriteAtomicNew(historyPath, envelope);
        Interrupt(PersistenceWriteInterruptionPoint.AfterHistoryFile);
        WriteCommit(commitsDirectory, record, commitIdentity, PersistenceCommitPhase.HistoryCommitted);
        Interrupt(PersistenceWriteInterruptionPoint.AfterHistoryCommitted);

        var currentPath = Path.Combine(directory, CurrentFileName);
        WriteAtomic(currentPath, envelope);
        Interrupt(PersistenceWriteInterruptionPoint.AfterCurrentFile);
        WriteCommit(commitsDirectory, record, commitIdentity, PersistenceCommitPhase.CurrentCommitted);
        Interrupt(PersistenceWriteInterruptionPoint.AfterCurrentCommitted);

        return new(DurableStateClassification.Accepted, "STATE_PERSISTED", record, commitIdentity);
    }

    private void Interrupt(PersistenceWriteInterruptionPoint point)
    {
        if (_interruptionPoint == point)
            throw new IOException("SIMULATED_PERSISTENCE_INTERRUPTION_" + point.ToString().ToUpperInvariant());
    }

    private void WriteCommit(string commitsDirectory, AuthoritativeStateRecord record, string commitIdentity, PersistenceCommitPhase phase)
    {
        var decisionIdentity = ExtractDecisionIdentity(record.Payload);
        var commit = new PersistenceCommitRecord(record.SourceIdentity, decisionIdentity, commitIdentity, record.Key,
            record.StateVersion, record.RecordDigest, phase, string.Empty).WithComputedDigest();

        var recordsDirectory = Path.Combine(commitsDirectory, CommitRecordsDirectoryName);
        Directory.CreateDirectory(recordsDirectory);
        var canonicalName = Digest(commitIdentity) + ".commit";
        WriteAtomic(Path.Combine(recordsDirectory, canonicalName), JsonSerializer.Serialize(commit));

        var requestIndexName = "request-" + Digest(record.SourceIdentity) + ".index";
        WriteAtomic(Path.Combine(commitsDirectory, requestIndexName),
            SerializeIndex(new CommitIdentityIndex("REQUEST", record.SourceIdentity, commitIdentity, canonicalName, string.Empty)));

        if (!string.IsNullOrWhiteSpace(decisionIdentity))
        {
            var decisionIndexName = "decision-" + Digest(decisionIdentity) + ".index";
            WriteAtomic(Path.Combine(commitsDirectory, decisionIndexName),
                SerializeIndex(new CommitIdentityIndex("DECISION", decisionIdentity, commitIdentity, canonicalName, string.Empty)));
        }

        WriteRegistryAnchor(Path.GetDirectoryName(commitsDirectory)!, commitsDirectory, record.Key);
    }

    private DurableCommitLookupResult LookupCommit(string fileName)
    {
        var matches = new List<(string Directory, PersistenceCommitRecord Commit)>();
        var registryFailure = false;
        var expectedButMissing = false;

        foreach (var directory in Directory.Exists(_root) ? Directory.GetDirectories(_root) : Array.Empty<string>())
        {
            if (Path.GetFileName(directory).StartsWith(".", StringComparison.Ordinal)) continue;

            var commitsDirectory = Path.Combine(directory, CommitDirectoryName);
            if (!Directory.Exists(commitsDirectory))
            {
                var stateKey = ResolveStateKeyFromDirectory(directory);
                if (!string.IsNullOrWhiteSpace(stateKey))
                {
                    var anchorId = Digest(stateKey);
                    var historyDirectory = Path.Combine(_independentAnchorRoot, CommitAnchorHistoryDirectoryName, anchorId);
                    var headPath = Path.Combine(_independentAnchorRoot, CommitAnchorHeadsDirectoryName, anchorId + ".head");
                    if (Directory.Exists(historyDirectory) || File.Exists(headPath) ||
                        File.Exists(Path.Combine(directory, CurrentFileName)) ||
                        Directory.Exists(Path.Combine(directory, HistoryDirectoryName)))
                    {
                        registryFailure = true;
                    }
                }
                continue;
            }

            var registry = ValidateRegistry(directory, commitsDirectory);
            if (registry.Classification is DurableStateClassification.Corrupted or DurableStateClassification.Conflicting)
            {
                registryFailure = true;
                continue;
            }

            var path = Path.Combine(commitsDirectory, fileName);
            if (!File.Exists(path))
            {
                if (registry.ExpectedEntries.Contains(fileName, StringComparer.Ordinal)) expectedButMissing = true;
                continue;
            }

            if (!TryReadIndex(path, out var index) || index is null)
                return new DurableCommitLookupResult(DurableStateClassification.Corrupted, "COMMIT_INDEX_CORRUPTED", null);
            var canonicalPath = Path.Combine(commitsDirectory, CommitRecordsDirectoryName, index.CanonicalRecordName);
            if (!TryReadCommit(canonicalPath, out var commit) || commit is null)
                return new DurableCommitLookupResult(DurableStateClassification.Corrupted, "CANONICAL_COMMIT_RESULT_CORRUPTED", null);
            if (!IndexMatchesCommit(index, commit))
                return new DurableCommitLookupResult(DurableStateClassification.Conflicting, "COMMIT_INDEX_CONFLICT", null);
            if (!ValidatePeerIndexes(commitsDirectory, commit))
                return new DurableCommitLookupResult(DurableStateClassification.Conflicting, "COMMIT_INDEX_INCOMPLETE_OR_CONFLICTING", null);
            matches.Add((directory, commit));
        }

        if (registryFailure)
            return new DurableCommitLookupResult(DurableStateClassification.Corrupted, "COMMIT_REGISTRY_CORRUPTED_OR_ROLLED_BACK", null);
        if (expectedButMissing)
            return new DurableCommitLookupResult(DurableStateClassification.Corrupted, "COMMIT_INDEX_INCOMPLETE", null);
        if (matches.Count == 0)
            return new DurableCommitLookupResult(DurableStateClassification.Missing, "COMMIT_RESULT_NOT_FOUND", null);
        if (matches.Count > 1)
            return new DurableCommitLookupResult(DurableStateClassification.Conflicting, "COMMIT_LOOKUP_AMBIGUOUS", null);

        return ReconcileCommitArtifacts(matches[0].Directory, matches[0].Commit);
    }


    private void WriteRegistryAnchor(string stateDirectory, string commitsDirectory, string stateKey)
    {
        var entries = RegistryEntries(commitsDirectory);
        var aggregate = Digest(string.Join("\n", entries));
        var anchorId = Digest(stateKey);
        var historyDirectory = Path.Combine(_independentAnchorRoot, CommitAnchorHistoryDirectoryName, anchorId);
        var headsDirectory = Path.Combine(_independentAnchorRoot, CommitAnchorHeadsDirectoryName);
        Directory.CreateDirectory(historyDirectory);
        Directory.CreateDirectory(headsDirectory);

        var headPath = Path.Combine(headsDirectory, anchorId + ".head");
        CommitAnchorHead? currentHead = null;
        CommitRegistryAnchor? currentAnchor = null;
        if (File.Exists(headPath))
        {
            if (!TryReadHead(headPath, out currentHead) || currentHead is null ||
                !string.Equals(currentHead.StateKey, stateKey, StringComparison.Ordinal))
                throw new IOException("COMMIT_ANCHOR_HEAD_CORRUPTED");
            var currentAnchorPath = AnchorGenerationPath(historyDirectory, currentHead.Generation);
            if (!TryReadAnchor(currentAnchorPath, out currentAnchor) || currentAnchor is null ||
                !string.Equals(currentAnchor.RecordDigest, currentHead.AnchorDigest, StringComparison.Ordinal))
                throw new IOException("COMMIT_ANCHOR_HISTORY_CORRUPTED");
            if (string.Equals(currentAnchor.AggregateDigest, aggregate, StringComparison.Ordinal) &&
                currentAnchor.Entries.SequenceEqual(entries, StringComparer.Ordinal))
                return;
        }

        var generation = (currentHead?.Generation ?? -1) + 1;
        var previousDigest = currentHead?.AnchorDigest ?? string.Empty;
        var anchor = new CommitRegistryAnchor(stateKey, generation, previousDigest, entries, aggregate, string.Empty);
        var serialized = SerializeAnchor(anchor);
        var completed = JsonSerializer.Deserialize<CommitRegistryAnchor>(serialized)!;
        WriteAtomicNew(AnchorGenerationPath(historyDirectory, generation), serialized);
        Interrupt(PersistenceWriteInterruptionPoint.AfterRegistryAnchorGeneration);
        WriteAtomic(headPath, SerializeHead(new CommitAnchorHead(stateKey, generation, completed.RecordDigest, string.Empty)));
    }

    private RegistryValidation ValidateRegistry(string stateDirectory, string commitsDirectory)
    {
        if (!Directory.Exists(commitsDirectory))
            return new RegistryValidation(DurableStateClassification.Corrupted, "COMMIT_REGISTRY_MISSING", Array.Empty<string>());

        var stateKey = ResolveStateKeyFromDirectory(stateDirectory);
        if (string.IsNullOrWhiteSpace(stateKey))
            return new RegistryValidation(DurableStateClassification.Corrupted, "COMMIT_STATE_DIRECTORY_UNRESOLVED", Array.Empty<string>());

        var entries = RegistryEntries(commitsDirectory);
        var aggregate = Digest(string.Join("\n", entries));
        var anchorId = Digest(stateKey);
        var historyDirectory = Path.Combine(_independentAnchorRoot, CommitAnchorHistoryDirectoryName, anchorId);
        var headPath = Path.Combine(_independentAnchorRoot, CommitAnchorHeadsDirectoryName, anchorId + ".head");

        if (!Directory.Exists(historyDirectory))
            return new RegistryValidation(DurableStateClassification.Corrupted, "COMMIT_ANCHOR_HISTORY_MISSING", Array.Empty<string>());

        if (!File.Exists(headPath))
        {
            var recovered = TryRecoverHeadWithoutPublishedHead(stateKey, historyDirectory, headPath, entries, aggregate);
            if (recovered is null)
                return new RegistryValidation(DurableStateClassification.Corrupted, "COMMIT_ANCHOR_HEAD_MISSING", Array.Empty<string>());
            return new RegistryValidation(DurableStateClassification.Accepted, "COMMIT_ANCHOR_HEAD_RECOVERED", recovered.Entries.Select(EntryName).ToArray());
        }

        if (!TryReadHead(headPath, out var head) || head is null ||
            !string.Equals(head.StateKey, stateKey, StringComparison.Ordinal))
            return new RegistryValidation(DurableStateClassification.Corrupted, "COMMIT_ANCHOR_HEAD_CORRUPTED", Array.Empty<string>());

        var anchorPath = AnchorGenerationPath(historyDirectory, head.Generation);
        if (!TryReadAnchor(anchorPath, out var anchor) || anchor is null ||
            !string.Equals(anchor.RecordDigest, head.AnchorDigest, StringComparison.Ordinal) ||
            !string.Equals(anchor.StateKey, stateKey, StringComparison.Ordinal))
            return new RegistryValidation(DurableStateClassification.Corrupted, "COMMIT_ANCHOR_HISTORY_CORRUPTED", Array.Empty<string>());

        if (string.Equals(anchor.AggregateDigest, aggregate, StringComparison.Ordinal) &&
            anchor.Entries.SequenceEqual(entries, StringComparer.Ordinal))
            return new RegistryValidation(DurableStateClassification.Accepted, "COMMIT_REGISTRY_VALID", anchor.Entries.Select(EntryName).ToArray());

        var nextPath = AnchorGenerationPath(historyDirectory, head.Generation + 1);
        if (TryReadAnchor(nextPath, out var next) && next is not null &&
            string.Equals(next.StateKey, stateKey, StringComparison.Ordinal) &&
            next.Generation == head.Generation + 1 &&
            string.Equals(next.PreviousAnchorDigest, head.AnchorDigest, StringComparison.Ordinal) &&
            string.Equals(next.AggregateDigest, aggregate, StringComparison.Ordinal) &&
            next.Entries.SequenceEqual(entries, StringComparer.Ordinal))
        {
            WriteAtomic(headPath, SerializeHead(new CommitAnchorHead(stateKey, next.Generation, next.RecordDigest, string.Empty)));
            return new RegistryValidation(DurableStateClassification.Accepted, "COMMIT_ANCHOR_PUBLICATION_RECOVERED", next.Entries.Select(EntryName).ToArray());
        }

        return new RegistryValidation(DurableStateClassification.Corrupted, "COMMIT_REGISTRY_ROLLBACK_OR_TRUNCATION", anchor.Entries.Select(EntryName).ToArray());
    }

    private CommitRegistryAnchor? TryRecoverHeadWithoutPublishedHead(
        string stateKey,
        string historyDirectory,
        string headPath,
        string[] entries,
        string aggregate)
    {
        var candidates = Directory.GetFiles(historyDirectory, "*.anchor")
            .OrderBy(path => path, StringComparer.Ordinal)
            .Select(path => TryReadAnchor(path, out var candidate) ? candidate : null)
            .Where(candidate => candidate is not null &&
                string.Equals(candidate.StateKey, stateKey, StringComparison.Ordinal) &&
                string.Equals(candidate.AggregateDigest, aggregate, StringComparison.Ordinal) &&
                candidate.Entries.SequenceEqual(entries, StringComparer.Ordinal))
            .Cast<CommitRegistryAnchor>()
            .ToArray();
        if (candidates.Length != 1 || candidates[0].Generation != 0 ||
            !string.IsNullOrEmpty(candidates[0].PreviousAnchorDigest)) return null;
        Directory.CreateDirectory(Path.GetDirectoryName(headPath)!);
        WriteAtomic(headPath, SerializeHead(new CommitAnchorHead(stateKey, 0, candidates[0].RecordDigest, string.Empty)));
        return candidates[0];
    }

    private string? ResolveStateKeyFromDirectory(string stateDirectory)
    {
        var currentPath = Path.Combine(stateDirectory, CurrentFileName);
        if (TryReadEnvelope(currentPath, out var current) && current is not null) return current.Key;
        var historyDirectory = Path.Combine(stateDirectory, HistoryDirectoryName);
        if (Directory.Exists(historyDirectory))
            foreach (var path in Directory.GetFiles(historyDirectory, "*.state").OrderBy(path => path, StringComparer.Ordinal))
                if (TryReadEnvelope(path, out var record) && record is not null) return record.Key;
        var recordsDirectory = Path.Combine(stateDirectory, CommitDirectoryName, CommitRecordsDirectoryName);
        if (Directory.Exists(recordsDirectory))
            foreach (var path in Directory.GetFiles(recordsDirectory, "*.commit").OrderBy(path => path, StringComparer.Ordinal))
                if (TryReadCommit(path, out var commit) && commit is not null) return commit.StateKey;
        return null;
    }

    private static string[] RegistryEntries(string commitsDirectory)
        => Directory.GetFiles(commitsDirectory, "*", SearchOption.AllDirectories)
            .Where(path => !path.EndsWith(".tmp", StringComparison.Ordinal))
            .Select(path => Path.GetRelativePath(commitsDirectory, path).Replace('\\', '/') + "|" + Digest(File.ReadAllText(path, Encoding.UTF8)))
            .OrderBy(value => value, StringComparer.Ordinal)
            .ToArray();

    private static string AnchorGenerationPath(string historyDirectory, long generation)
        => Path.Combine(historyDirectory, generation.ToString("D20", CultureInfo.InvariantCulture) + ".anchor");

    private static string EntryName(string entry)
    {
        var separator = entry.LastIndexOf('|');
        return separator < 0 ? entry : entry[..separator];
    }

    private static string SerializeIndex(CommitIdentityIndex index)
    {
        var canonical = string.Join("\u001F", index.Kind, index.Identity, index.CommitIdentity, index.CanonicalRecordName);
        var completed = index with { RecordDigest = Digest(canonical) };
        return JsonSerializer.Serialize(completed);
    }

    private static bool TryReadIndex(string path, out CommitIdentityIndex? index)
    {
        index = null;
        try
        {
            index = JsonSerializer.Deserialize<CommitIdentityIndex>(File.ReadAllText(path, Encoding.UTF8));
            if (index is null) return false;
            var canonical = string.Join("\u001F", index.Kind, index.Identity, index.CommitIdentity, index.CanonicalRecordName);
            return string.Equals(index.RecordDigest, Digest(canonical), StringComparison.Ordinal);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException) { return false; }
    }

    private static string SerializeAnchor(CommitRegistryAnchor anchor)
    {
        var canonical = string.Join("\u001F", anchor.StateKey,
            anchor.Generation.ToString(CultureInfo.InvariantCulture), anchor.PreviousAnchorDigest,
            anchor.AggregateDigest, string.Join("\u001E", anchor.Entries));
        var completed = anchor with { RecordDigest = Digest(canonical) };
        return JsonSerializer.Serialize(completed);
    }

    private static bool TryReadAnchor(string path, out CommitRegistryAnchor? anchor)
    {
        anchor = null;
        try
        {
            anchor = JsonSerializer.Deserialize<CommitRegistryAnchor>(File.ReadAllText(path, Encoding.UTF8));
            if (anchor is null) return false;
            var canonical = string.Join("\u001F", anchor.StateKey,
                anchor.Generation.ToString(CultureInfo.InvariantCulture), anchor.PreviousAnchorDigest,
                anchor.AggregateDigest, string.Join("\u001E", anchor.Entries));
            return string.Equals(anchor.RecordDigest, Digest(canonical), StringComparison.Ordinal);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException) { return false; }
    }

    private static string SerializeHead(CommitAnchorHead head)
    {
        var canonical = string.Join("\u001F", head.StateKey,
            head.Generation.ToString(CultureInfo.InvariantCulture), head.AnchorDigest);
        return JsonSerializer.Serialize(head with { RecordDigest = Digest(canonical) });
    }

    private static bool TryReadHead(string path, out CommitAnchorHead? head)
    {
        head = null;
        try
        {
            head = JsonSerializer.Deserialize<CommitAnchorHead>(File.ReadAllText(path, Encoding.UTF8));
            if (head is null) return false;
            var canonical = string.Join("\u001F", head.StateKey,
                head.Generation.ToString(CultureInfo.InvariantCulture), head.AnchorDigest);
            return string.Equals(head.RecordDigest, Digest(canonical), StringComparison.Ordinal);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException) { return false; }
    }

    private static bool IndexMatchesCommit(CommitIdentityIndex index, PersistenceCommitRecord commit)
        => string.Equals(index.CommitIdentity, commit.CommitIdentity, StringComparison.Ordinal) &&
           (string.Equals(index.Kind, "REQUEST", StringComparison.Ordinal) && string.Equals(index.Identity, commit.RequestIdentity, StringComparison.Ordinal) ||
            string.Equals(index.Kind, "DECISION", StringComparison.Ordinal) && string.Equals(index.Identity, commit.DecisionIdentity, StringComparison.Ordinal));

    private static bool ValidatePeerIndexes(string commitsDirectory, PersistenceCommitRecord commit)
    {
        var requestPath = Path.Combine(commitsDirectory, "request-" + Digest(commit.RequestIdentity) + ".index");
        if (!TryReadIndex(requestPath, out var request) || request is null || !IndexMatchesCommit(request, commit)) return false;
        if (string.IsNullOrWhiteSpace(commit.DecisionIdentity)) return true;
        var decisionPath = Path.Combine(commitsDirectory, "decision-" + Digest(commit.DecisionIdentity) + ".index");
        return TryReadIndex(decisionPath, out var decision) && decision is not null && IndexMatchesCommit(decision, commit) &&
               string.Equals(request.CanonicalRecordName, decision.CanonicalRecordName, StringComparison.Ordinal);
    }

    private DurableCommitLookupResult ReconcileCommitArtifacts(string directory, PersistenceCommitRecord commit)
    {
        if (!TryParseStateKey(commit.StateKey, out var stateNamespace, out var subjectId, out var stateClass))
            return new DurableCommitLookupResult(DurableStateClassification.Corrupted, "COMMIT_STATE_KEY_INVALID", null);

        var expectedDirectory = ResolveDirectory(stateNamespace, subjectId, stateClass);
        if (!string.Equals(Path.GetFullPath(directory), Path.GetFullPath(expectedDirectory), StringComparison.OrdinalIgnoreCase))
            return new DurableCommitLookupResult(DurableStateClassification.Conflicting, "COMMIT_STATE_DIRECTORY_MISMATCH", null);

        var historyPath = HistoryPath(directory, commit.StateVersion);
        var historyExists = File.Exists(historyPath);
        var historyMatches = historyExists && TryReadEnvelope(historyPath, out var history) &&
            history is not null && MatchesCommit(history, commit);
        if (historyExists && !historyMatches)
            return new DurableCommitLookupResult(DurableStateClassification.Corrupted, "COMMIT_HISTORY_BINDING_MISMATCH", null);

        var currentPath = Path.Combine(directory, CurrentFileName);
        var currentExists = File.Exists(currentPath);
        var currentMatches = currentExists && TryReadEnvelope(currentPath, out var current) &&
            current is not null && MatchesCommit(current, commit);

        if (currentMatches)
        {
            var effective = (commit with { Phase = PersistenceCommitPhase.CurrentCommitted, RecordDigest = string.Empty }).WithComputedDigest();
            return new DurableCommitLookupResult(DurableStateClassification.Accepted, "COMMIT_RESULT_CURRENT_COMMITTED", effective);
        }

        if (historyMatches)
        {
            if (commit.Phase == PersistenceCommitPhase.CurrentCommitted)
            {
                var completed = (commit with { Phase = PersistenceCommitPhase.CurrentCommitted, RecordDigest = string.Empty }).WithComputedDigest();
                return new DurableCommitLookupResult(DurableStateClassification.Accepted, "COMMIT_RESULT_CURRENT_COMMITTED_HISTORY_VERIFIED", completed);
            }

            var effective = (commit with { Phase = PersistenceCommitPhase.HistoryCommitted, RecordDigest = string.Empty }).WithComputedDigest();
            return new DurableCommitLookupResult(DurableStateClassification.UncertainAfterCommit, "COMMIT_RESULT_HISTORY_COMMITTED", effective);
        }

        if (currentExists && commit.Phase == PersistenceCommitPhase.CurrentCommitted)
            return new DurableCommitLookupResult(DurableStateClassification.Corrupted, "COMMIT_CURRENT_AND_HISTORY_BINDING_MISMATCH", null);

        if (commit.Phase != PersistenceCommitPhase.Prepared)
            return new DurableCommitLookupResult(DurableStateClassification.Corrupted, "COMMIT_PHASE_AHEAD_OF_ARTIFACTS", null);

        var prepared = (commit with { Phase = PersistenceCommitPhase.Prepared, RecordDigest = string.Empty }).WithComputedDigest();
        return new DurableCommitLookupResult(DurableStateClassification.UncertainBeforeCommit, "COMMIT_RESULT_PREPARED", prepared);
    }

    private TrustedHistoryChain ReadTrustedHistoryChain(string stateNamespace, string subjectId, FoundationStateClass stateClass)
    {
        var directory = ResolveDirectory(stateNamespace, subjectId, stateClass);
        var historyDirectory = Path.Combine(directory, HistoryDirectoryName);
        if (!Directory.Exists(historyDirectory))
            return new TrustedHistoryChain(DurableStateClassification.Missing, "HISTORY_NOT_FOUND", Array.Empty<AuthoritativeStateRecord>());

        var files = Directory.GetFiles(historyDirectory, "*.state")
            .OrderBy(path => path, StringComparer.Ordinal)
            .ToArray();
        if (files.Length == 0)
            return new TrustedHistoryChain(DurableStateClassification.Missing, "HISTORY_NOT_FOUND", Array.Empty<AuthoritativeStateRecord>());

        var records = new List<AuthoritativeStateRecord>();
        AuthoritativeStateRecord? previous = null;
        foreach (var file in files)
        {
            if (!TryReadEnvelope(file, out var current) || current is null)
                return new TrustedHistoryChain(DurableStateClassification.Corrupted, "HISTORY_CORRUPTED", Array.Empty<AuthoritativeStateRecord>());
            if (!MatchesKey(current, stateNamespace, subjectId, stateClass))
                return new TrustedHistoryChain(DurableStateClassification.Conflicting, "HISTORY_KEY_CONFLICT", Array.Empty<AuthoritativeStateRecord>());
            var expectedFile = current.StateVersion.ToString("D20", CultureInfo.InvariantCulture) + ".state";
            if (!string.Equals(Path.GetFileName(file), expectedFile, StringComparison.Ordinal))
                return new TrustedHistoryChain(DurableStateClassification.Conflicting, "HISTORY_VERSION_FILE_CONFLICT", Array.Empty<AuthoritativeStateRecord>());
            if (previous is null)
            {
                if (current.StateVersion != 0 || !string.IsNullOrEmpty(current.PreviousRecordDigest))
                    return new TrustedHistoryChain(DurableStateClassification.Conflicting, "HISTORY_CHAIN_INVALID", Array.Empty<AuthoritativeStateRecord>());
            }
            else if (current.StateVersion != previous.StateVersion + 1 ||
                     !string.Equals(current.PreviousRecordDigest, previous.RecordDigest, StringComparison.Ordinal))
                return new TrustedHistoryChain(DurableStateClassification.Conflicting, "HISTORY_CHAIN_INVALID", Array.Empty<AuthoritativeStateRecord>());
            records.Add(current);
            previous = current;
        }
        return new TrustedHistoryChain(DurableStateClassification.TrustedHistoryReconstructed, "TRUSTED_HISTORY_RECONSTRUCTED", records);
    }

    private static bool TryReadCommit(string path, out PersistenceCommitRecord? commit)
    {
        commit = null;
        try
        {
            commit = JsonSerializer.Deserialize<PersistenceCommitRecord>(File.ReadAllText(path, Encoding.UTF8));
            return commit is not null && string.Equals(commit.WithComputedDigest().RecordDigest, commit.RecordDigest, StringComparison.Ordinal);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException)
        {
            return false;
        }
    }

    private static bool MatchesCommit(AuthoritativeStateRecord record, PersistenceCommitRecord commit)
        => string.Equals(record.Key, commit.StateKey, StringComparison.Ordinal) &&
           record.StateVersion == commit.StateVersion &&
           string.Equals(record.RecordDigest, commit.StateDigest, StringComparison.Ordinal) &&
           string.Equals(StateCanonicalEncoding.CommitIdentity(record), commit.CommitIdentity, StringComparison.Ordinal) &&
           string.Equals(record.SourceIdentity, commit.RequestIdentity, StringComparison.Ordinal);

    private static bool TryParseStateKey(string stateKey, out string stateNamespace, out string subjectId, out FoundationStateClass stateClass)
    {
        stateNamespace = string.Empty;
        subjectId = string.Empty;
        stateClass = default;
        var parts = stateKey.Split('|');
        return parts.Length == 3 &&
               !string.IsNullOrWhiteSpace(parts[0]) &&
               !string.IsNullOrWhiteSpace(parts[1]) &&
               Enum.TryParse(parts[2], false, out stateClass) &&
               Assign(parts[0], parts[1], out stateNamespace, out subjectId);
    }

    private static bool Assign(string ns, string subject, out string stateNamespace, out string subjectId)
    {
        stateNamespace = ns;
        subjectId = subject;
        return true;
    }

    private static string ExtractDecisionIdentity(string payload)
    {
        const string prefix = "decision=";
        var token = payload.Split(';', StringSplitOptions.RemoveEmptyEntries)
            .FirstOrDefault(x => x.StartsWith(prefix, StringComparison.Ordinal));
        return token is null ? string.Empty : token[prefix.Length..];
    }

    private static void WriteAtomic(string path, string content)
    {
        var temp = path + ".tmp";
        File.WriteAllText(temp, content, new UTF8Encoding(false));
        File.Move(temp, path, true);
    }

    private static void WriteAtomicNew(string path, string content)
    {
        var temp = path + ".tmp";
        File.WriteAllText(temp, content, new UTF8Encoding(false));
        File.Move(temp, path);
    }

    private string ResolveDirectory(string stateNamespace, string subjectId, FoundationStateClass stateClass)
        => Path.Combine(_root, Digest(StateCanonicalEncoding.Key(stateNamespace, subjectId, stateClass)));
    private static string HistoryPath(string directory, long stateVersion)
        => Path.Combine(directory, HistoryDirectoryName, stateVersion.ToString("D20", CultureInfo.InvariantCulture) + ".state");
    private static string ResolveDefaultIndependentAnchorRoot(string root)
    {
        if (string.IsNullOrWhiteSpace(root))
            throw new ArgumentException("State root is required.", nameof(root));
        var fullRoot = Path.GetFullPath(root);
        var parent = Directory.GetParent(fullRoot)?.FullName
            ?? throw new ArgumentException("State root must have a parent directory.", nameof(root));
        return Path.Combine(parent, ".falcon-independent-anchor-store", Digest(fullRoot));
    }

    private static string Digest(string value) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
    private static string BuildEnvelope(AuthoritativeStateRecord record)
    {
        var payload = StateCanonicalEncoding.SerializeRecord(record);
        return Digest(payload) + Environment.NewLine + payload;
    }
    private static bool TryReadEnvelope(string path, out AuthoritativeStateRecord? record)
    {
        record = null;
        try
        {
            var lines = File.ReadAllLines(path, Encoding.UTF8);
            if (lines.Length != 2 || lines[0].Length != 64 || lines[0] != Digest(lines[1])) return false;
            if (!StateCanonicalEncoding.TryDeserializeRecord(lines[1], out record) || record is null) return false;
            return record.RecordDigest == record.WithComputedDigest().RecordDigest;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException) { return false; }
    }
    private static bool MatchesKey(AuthoritativeStateRecord record, string ns, string subject, FoundationStateClass stateClass)
        => record.Namespace == ns && record.SubjectId == subject && record.StateClass == stateClass;
    private static DurableStateReadResult ReadReject(DurableStateClassification c, string r) => new(c, r, null);
    private static DurableStateWriteResult WriteReject(DurableStateClassification c, string r) => new(c, r, null, string.Empty);

    private sealed record CommitIdentityIndex(string Kind, string Identity, string CommitIdentity, string CanonicalRecordName, string RecordDigest);
    private sealed record CommitRegistryAnchor(string StateKey, long Generation, string PreviousAnchorDigest, string[] Entries, string AggregateDigest, string RecordDigest);
    private sealed record CommitAnchorHead(string StateKey, long Generation, string AnchorDigest, string RecordDigest);
    private sealed record RegistryValidation(DurableStateClassification Classification, string Reason, IReadOnlyList<string> ExpectedEntries);

    private sealed record TrustedHistoryChain(
        DurableStateClassification Classification,
        string Reason,
        IReadOnlyList<AuthoritativeStateRecord> Records);
}
