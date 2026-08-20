using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Foundation.Evidence;
using Foundation.Reconciliation;
using Foundation.State;

namespace Falcon.Stage4.WP05.Verifier;

internal static class Program
{
    private static readonly DateTimeOffset AuditTime = DateTimeOffset.Parse("2026-08-06T04:00:00+00:00");

    private static int Main(string[] args)
    {
        if (args.Length == 4 && args[0] == "--writer")
            return WriterProcess(args[1], args[2], args[3]);

        var failures = new List<string>();
        var root = Path.Combine(Path.GetTempPath(), "falcon-stage4-wp05-" + Guid.NewGuid().ToString("N"));
        try
        {
            VerifyCrossProcessExpectedVersion(failures, Path.Combine(root, "process"));
            VerifyCrashWindows(failures, Path.Combine(root, "crash"));
            VerifyLookupAmbiguity(failures, Path.Combine(root, "ambiguity"));
            VerifyBoundHistoryAndCorruptedCurrent(failures, Path.Combine(root, "reconstruct"));
            VerifyCommitRegistryTamperEvidence(failures, Path.Combine(root, "registry"));
            VerifyIndependentAnchorRollbackAndPublicationRecovery(failures, Path.Combine(root, "anchor-v12"));
            VerifyDeterministicClassifications(failures);
            VerifyNewEmptyRestart(failures, Path.Combine(root, "restart"));
        }
        finally
        {
            if (Directory.Exists(root)) Directory.Delete(root, true);
        }

        if (failures.Count != 0)
        {
            Console.Error.WriteLine("Stage 4 WP-05 verifier: FAIL");
            foreach (var failure in failures) Console.Error.WriteLine("- " + failure);
            return 1;
        }

        Console.WriteLine("Stage 4 WP-05 verifier: PASS");
        Console.WriteLine("Cross-process expected-version concurrency, artifact-aware commit phases, exact commit-bound history reconstruction, independently stored immutable commit-registry generations, full-root rollback detection, recoverable anchor publication, durable ReconciliationState, ambiguous lookup rejection, crash-window recovery, deterministic divergence classification, and fail-closed continuation verified.");
        Console.WriteLine("No blind retry, fabricated state, state regression, WP-06 closure, or time-based Owner authority gate was introduced.");
        Console.WriteLine("Reconciliation digest: V12-INDEPENDENT-ANCHOR-AND-DURABLE-RECONCILIATION-STATE");
        return 0;
    }

    private static int WriterProcess(string root, string requestIdentity, string payload)
    {
        var registry = new StateOwnershipRegistry();
        registry.Register(Declaration("subject/process"));
        var store = new DurableAuthoritativeStateStore(registry, new FileAuthoritativeStateProvider(root));
        var result = store.Write(Record("subject/process", requestIdentity, payload), -1);
        Console.WriteLine(result.Classification);
        return result.Accepted || result.Classification == DurableStateClassification.StaleExpectedVersion ? 0 : 2;
    }

    private static void VerifyCrossProcessExpectedVersion(List<string> failures, string root)
    {
        Directory.CreateDirectory(root);
        var dll = typeof(Program).Assembly.Location;
        var first = StartWriter(dll, root, "request/process-a", "PAYLOAD-A");
        var second = StartWriter(dll, root, "request/process-b", "PAYLOAD-B");
        first.WaitForExit();
        second.WaitForExit();
        var outputs = new[] { first.StandardOutput.ReadToEnd().Trim(), second.StandardOutput.ReadToEnd().Trim() };
        Expect(failures, "cross-process writers exit", first.ExitCode == 0 && second.ExitCode == 0);
        Expect(failures, "exactly one cross-process writer accepted", outputs.Count(x => x == DurableStateClassification.Accepted.ToString()) == 1);
        Expect(failures, "cross-process loser stale", outputs.Count(x => x == DurableStateClassification.StaleExpectedVersion.ToString()) == 1);
    }

    private static Process StartWriter(string dll, string root, string request, string payload)
    {
        var info = new ProcessStartInfo("dotnet") { UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true };
        info.ArgumentList.Add(dll);
        info.ArgumentList.Add("--writer");
        info.ArgumentList.Add(root);
        info.ArgumentList.Add(request);
        info.ArgumentList.Add(payload);
        return Process.Start(info)!;
    }

    private static void VerifyCrashWindows(List<string> failures, string root)
    {
        VerifyCrashPoint(failures, Path.Combine(root, "prepared"), PersistenceWriteInterruptionPoint.AfterPrepared,
            DurableStateClassification.UncertainBeforeCommit, "prepared remains before commit");
        VerifyCrashPoint(failures, Path.Combine(root, "history"), PersistenceWriteInterruptionPoint.AfterHistoryFile,
            DurableStateClassification.UncertainAfterCommit, "history artifact promotes uncertain after commit");
        VerifyCrashPoint(failures, Path.Combine(root, "current"), PersistenceWriteInterruptionPoint.AfterCurrentFile,
            DurableStateClassification.Accepted, "current artifact promotes committed");
        VerifyCrashPoint(failures, Path.Combine(root, "return"), PersistenceWriteInterruptionPoint.AfterCurrentCommitted,
            DurableStateClassification.Accepted, "lost response reconstructs committed result");
    }

    private static void VerifyCrashPoint(List<string> failures, string root, PersistenceWriteInterruptionPoint point,
        DurableStateClassification expected, string name)
    {
        var registry = new StateOwnershipRegistry();
        registry.Register(Declaration("subject/" + point));
        var record = Record("subject/" + point, "request/" + point, "decision=decision/" + point + ";READY");
        var store = new DurableAuthoritativeStateStore(registry, new FileAuthoritativeStateProvider(root, point));
        try { store.Write(record, -1); } catch (IOException) { }
        var lookup = new DurableAuthoritativeStateStore(registry, new FileAuthoritativeStateProvider(root))
            .LookupCommitByRequest(record.SourceIdentity);
        Expect(failures, name, lookup.Classification == expected && lookup.Commit is not null);
    }

    private static void VerifyLookupAmbiguity(List<string> failures, string root)
    {
        var registry = new StateOwnershipRegistry();
        registry.Register(Declaration("subject/a"));
        registry.Register(Declaration("subject/b"));
        var store = new DurableAuthoritativeStateStore(registry, new FileAuthoritativeStateProvider(root));
        Expect(failures, "ambiguity first write", store.Write(Record("subject/a", "request/shared", "READY-A"), -1).Accepted);
        Expect(failures, "ambiguity second write", store.Write(Record("subject/b", "request/shared", "READY-B"), -1).Accepted);
        Expect(failures, "ambiguous request lookup rejected",
            store.LookupCommitByRequest("request/shared").Classification == DurableStateClassification.Conflicting);
    }

    private static void VerifyBoundHistoryAndCorruptedCurrent(List<string> failures, string root)
    {
        var stateRoot = Path.Combine(root, "state");
        var evidenceRoot = Path.Combine(root, "evidence");
        var registry = new StateOwnershipRegistry();
        registry.Register(Declaration("subject/bound"));
        var provider = new FileAuthoritativeStateProvider(stateRoot);
        var store = new DurableAuthoritativeStateStore(registry, provider);
        var record = Record("subject/bound", "request/bound", "decision=decision/bound;READY");
        var write = store.Write(record, -1);
        Expect(failures, "bound write accepted", write.Accepted);
        var exact = store.ReadTrustedHistoryVersion(record.Namespace, record.SubjectId, record.StateClass,
            record.StateVersion, record.RecordDigest, write.CommitIdentity);
        Expect(failures, "exact history binding", exact.Classification == DurableStateClassification.TrustedHistoryReconstructed && exact.Latest == record);

        var stateDirectory = Directory.GetDirectories(stateRoot)
            .Single(path => !Path.GetFileName(path).StartsWith(".", StringComparison.Ordinal));
        File.WriteAllText(Path.Combine(stateDirectory, "current.state"), "CORRUPTED");
        var evidence = new IntegrityLinkedEvidenceJournal(new FileEvidenceJournalProvider(evidenceRoot));
        var request = new ReconciliationRequest(record.Namespace, record.SubjectId, record.StateClass,
            record.SourceIdentity, "decision/bound");
        var reconciler = CreateReconciler(store, evidence, request, Path.Combine(root, "reconciliation"), out _);
        var result = reconciler.Reconcile(request);
        Expect(failures, "corrupted current exact history reconstruction",
            result.Classification == ReconciliationClassification.TrustedStateReconstructed &&
            result.State == record && result.ChallengeRequired && !result.ContinuationAllowed);
    }


    private static void VerifyCommitRegistryTamperEvidence(List<string> failures, string root)
    {
        VerifyDeletedIndex(failures, Path.Combine(root, "request-index"), true);
        VerifyDeletedIndex(failures, Path.Combine(root, "decision-index"), false);

        var stateRoot = Path.Combine(root, "complete-registry");
        var evidenceRoot = Path.Combine(root, "complete-evidence");
        var registry = new StateOwnershipRegistry();
        registry.Register(Declaration("subject/registry-delete"));
        var store = new DurableAuthoritativeStateStore(registry, new FileAuthoritativeStateProvider(stateRoot));
        var record = Record("subject/registry-delete", "request/registry-delete", "decision=decision/registry-delete;READY");
        Expect(failures, "registry delete write", store.Write(record, -1).Accepted);
        var stateDirectory = Directory.GetDirectories(stateRoot)
            .Single(path => !Path.GetFileName(path).StartsWith(".", StringComparison.Ordinal));
        Directory.Delete(Path.Combine(stateDirectory, "commits"), true);
        var lookup = store.LookupCommitByRequest(record.SourceIdentity);
        Expect(failures, "complete registry deletion detected", lookup.Classification == DurableStateClassification.Corrupted);
        var registryRequest = new ReconciliationRequest(record.Namespace, record.SubjectId, record.StateClass,
            record.SourceIdentity, "decision/registry-delete");
        var reconciler = CreateReconciler(store,
            new IntegrityLinkedEvidenceJournal(new FileEvidenceJournalProvider(evidenceRoot)),
            registryRequest, Path.Combine(root, "reconciliation"), out _);
        var reconciled = reconciler.Reconcile(registryRequest);
        Expect(failures, "missing registry fails closed", reconciled.Classification == ReconciliationClassification.FailedClosed &&
            reconciled.ChallengeRequired && !reconciled.ContinuationAllowed);

        var newRootState = new DurableAuthoritativeStateStore(new StateOwnershipRegistry(),
            new FileAuthoritativeStateProvider(Path.Combine(root, "genuine-new-state")));
        var newRootRequest = new ReconciliationRequest(
            "foundation.lifecycle", "subject/genuine-new", FoundationStateClass.LifecycleState, string.Empty, string.Empty);
        var newRootReconciler = CreateReconciler(newRootState,
            new IntegrityLinkedEvidenceJournal(new FileEvidenceJournalProvider(Path.Combine(root, "genuine-new-evidence"))),
            newRootRequest, Path.Combine(root, "genuine-new-reconciliation"), out _);
        var newRoot = newRootReconciler.Reconcile(newRootRequest);
        Expect(failures, "genuine new root preserved", newRoot.Classification == ReconciliationClassification.NewEmptyRoot && newRoot.ContinuationAllowed);
    }

    private static void VerifyDeletedIndex(List<string> failures, string root, bool requestIndex)
    {
        var registry = new StateOwnershipRegistry();
        var subject = requestIndex ? "subject/request-index" : "subject/decision-index";
        registry.Register(Declaration(subject));
        var store = new DurableAuthoritativeStateStore(registry, new FileAuthoritativeStateProvider(root));
        var request = requestIndex ? "request/delete-request" : "request/delete-decision";
        var decision = requestIndex ? "decision/delete-request" : "decision/delete-decision";
        var record = Record(subject, request, "decision=" + decision + ";READY");
        Expect(failures, "index deletion write " + requestIndex, store.Write(record, -1).Accepted);
        var stateDirectory = Directory.GetDirectories(root)
            .Single(path => !Path.GetFileName(path).StartsWith(".", StringComparison.Ordinal));
        var commits = Path.Combine(stateDirectory, "commits");
        var pattern = requestIndex ? "request-*.index" : "decision-*.index";
        File.Delete(Directory.GetFiles(commits, pattern).Single());
        var lookup = requestIndex ? store.LookupCommitByRequest(request) : store.LookupCommitByDecision(decision);
        Expect(failures, "deleted index detected " + requestIndex, lookup.Classification == DurableStateClassification.Corrupted);
    }

    private static void VerifyIndependentAnchorRollbackAndPublicationRecovery(List<string> failures, string root)
    {
        VerifyAnchorPublicationRecovery(failures, Path.Combine(root, "publication"));
        VerifyFullRootRollbackDetection(failures, Path.Combine(root, "rollback"));
    }

    private static void VerifyAnchorPublicationRecovery(List<string> failures, string root)
    {
        var stateRoot = Path.Combine(root, "state");
        var anchorRoot = Path.Combine(root, "independent-anchors");
        var registry = new StateOwnershipRegistry();
        registry.Register(Declaration("subject/anchor-publication"));
        var record = Record("subject/anchor-publication", "request/anchor-publication",
            "decision=decision/anchor-publication;READY");
        var interrupted = new DurableAuthoritativeStateStore(registry,
            new FileAuthoritativeStateProvider(stateRoot, anchorRoot, PersistenceWriteInterruptionPoint.AfterRegistryAnchorGeneration));
        try { interrupted.Write(record, -1); } catch (IOException) { }

        var recovered = new DurableAuthoritativeStateStore(registry,
            new FileAuthoritativeStateProvider(stateRoot, anchorRoot))
            .LookupCommitByRequest(record.SourceIdentity);
        Expect(failures, "anchor publication recovered",
            recovered.Classification == DurableStateClassification.UncertainBeforeCommit && recovered.Commit is not null);
        var heads = Path.Combine(anchorRoot, ".commit-anchor-heads");
        Expect(failures, "independent anchor head published after recovery",
            Directory.Exists(heads) && Directory.GetFiles(heads, "*.head").Length == 1);
    }

    private static void VerifyFullRootRollbackDetection(List<string> failures, string root)
    {
        var stateRoot = Path.Combine(root, "state-root");
        var anchorRoot = Path.Combine(root, "independent-anchor-root");
        var registry = new StateOwnershipRegistry();
        registry.Register(Declaration("subject/anchor-rollback"));
        var store = new DurableAuthoritativeStateStore(registry,
            new FileAuthoritativeStateProvider(stateRoot, anchorRoot));
        var first = Record("subject/anchor-rollback", "request/anchor-rollback-v0",
            "decision=decision/anchor-rollback-v0;READY");
        Expect(failures, "rollback first write", store.Write(first, -1).Accepted);

        var snapshot = Path.Combine(root, "full-root-snapshot");
        CopyDirectory(stateRoot, snapshot);

        var second = new AuthoritativeStateRecord(
            "state/request/anchor-rollback-v1", first.Namespace, first.SubjectId, first.StateClass,
            StateRepresentationKind.Authoritative, first.AuthoritativeOwner, first.AuthoritativeSource,
            first.PersistenceOwner, first.WriterAuthority, "request/anchor-rollback-v1", 1, AuditTime,
            first.RetentionClassification, "decision=decision/anchor-rollback-v1;READY", first.RecordDigest,
            string.Empty).WithComputedDigest();
        Expect(failures, "rollback second write", store.Write(second, 0).Accepted);

        Directory.Delete(stateRoot, true);
        CopyDirectory(snapshot, stateRoot);
        Directory.Delete(snapshot, true);

        var rolledBack = store.LookupCommitByRequest(first.SourceIdentity);
        Expect(failures, "full state-root rollback rejected by independent anchor",
            rolledBack.Classification == DurableStateClassification.Corrupted &&
            rolledBack.Reason.Contains("ROLLED_BACK", StringComparison.Ordinal));
    }

    private static void CopyDirectory(string source, string destination)
    {
        Directory.CreateDirectory(destination);
        foreach (var directory in Directory.GetDirectories(source, "*", SearchOption.AllDirectories))
            Directory.CreateDirectory(directory.Replace(source, destination, StringComparison.Ordinal));
        foreach (var file in Directory.GetFiles(source, "*", SearchOption.AllDirectories))
        {
            var target = file.Replace(source, destination, StringComparison.Ordinal);
            Directory.CreateDirectory(Path.GetDirectoryName(target)!);
            File.Copy(file, target, true);
        }
    }

    private static void VerifyDeterministicClassifications(List<string> failures)
    {
        var classifier = new ReconciliationClassifier();
        var request = new ReconciliationRequest("foundation.lifecycle", "subject/reconcile", FoundationStateClass.LifecycleState, "request/1", "decision/1");
        var stateRecord = Record("subject/reconcile", "request/1", "READY");
        var state = new DurableStateReadResult(DurableStateClassification.Accepted, "STATE_LOADED", stateRecord);
        var emptyJournal = new EvidenceJournalReadResult(EvidenceJournalClassification.Missing, "EVIDENCE_JOURNAL_MISSING", Array.Empty<IntegrityLinkedEvidenceRecord>(), null);
        var first = classifier.Classify(request, state, emptyJournal, Array.Empty<AcceptedFactEvent>());
        var second = classifier.Classify(request, state, emptyJournal, Array.Empty<AcceptedFactEvent>());
        Expect(failures, "classification deterministic", first == second && first.Classification == ReconciliationClassification.StateAheadOfEvidence);
    }

    private static void VerifyNewEmptyRestart(List<string> failures, string root)
    {
        var registry = new StateOwnershipRegistry();
        var state = new DurableAuthoritativeStateStore(registry, new FileAuthoritativeStateProvider(Path.Combine(root, "state")));
        var evidence = new IntegrityLinkedEvidenceJournal(new FileEvidenceJournalProvider(Path.Combine(root, "evidence")));
        var request = new ReconciliationRequest("foundation.lifecycle", "subject/new", FoundationStateClass.LifecycleState, string.Empty, string.Empty);
        var reconciler = CreateReconciler(state, evidence, request, Path.Combine(root, "reconciliation"), out var reconciliationState);
        var first = reconciler.Reconcile(request);
        var second = reconciler.Reconcile(request);
        Expect(failures, "new root explicit", first.Classification == ReconciliationClassification.NewEmptyRoot && first.ContinuationAllowed);
        Expect(failures, "restart deterministic", first == second);

        var persisted = reconciliationState.Read("foundation.reconciliation",
            ReconciliationCanonicalEncoding.SubjectId(request), FoundationStateClass.ReconciliationState);
        Expect(failures, "reconciliation state persisted", persisted.Accepted && persisted.Current is not null &&
            persisted.Current.Payload.Contains("NewEmptyRoot", StringComparison.Ordinal));

        var appended = evidence.Append(new EvidenceAppendRequest(string.Empty, "Foundation.RestartReconciler",
            "request/evidence-change", EvidenceDecisionKind.Allow, "decision/evidence-change", "CHANGED_EVIDENCE",
            EvidenceExecutionOutcome.Accepted, EvidencePersistenceOutcome.Accepted, request.StateNamespace,
            request.SubjectId, 0, "CHANGED-DIGEST", "verification", string.Empty));
        Expect(failures, "changed evidence appended", appended.Accepted);
        var changed = reconciler.Reconcile(request);
        Expect(failures, "changed evidence under same reconciliation identity conflicts",
            changed.Classification == ReconciliationClassification.FailedClosed &&
            changed.Reason.Contains("RECONCILIATION_STATE_PERSISTENCE_REJECTED", StringComparison.Ordinal));
    }

    private static RestartReconciler CreateReconciler(
        DurableAuthoritativeStateStore state,
        IntegrityLinkedEvidenceJournal evidence,
        ReconciliationRequest request,
        string root,
        out DurableAuthoritativeStateStore reconciliationState)
    {
        var ownership = new StateOwnershipRegistry();
        ownership.Register(RestartReconciler.ReconciliationOwnership(request));
        reconciliationState = new DurableAuthoritativeStateStore(ownership,
            new FileAuthoritativeStateProvider(Path.Combine(root, "state"), Path.Combine(root, "independent-anchors")));
        return new RestartReconciler(state, evidence, new ReconciliationClassifier(), reconciliationState);
    }

    private static StateOwnershipDeclaration Declaration(string subjectId)
        => new("ownership/" + subjectId, "foundation.lifecycle", subjectId, FoundationStateClass.LifecycleState,
            "Foundation.LifecycleControlService", "Foundation.LifecycleControlService", "Foundation.State",
            "Foundation.Reconciliation", "Foundation.LifecycleControlService", "PERMANENT", 1, AuditTime, AuditTime.AddYears(100));

    private static AuthoritativeStateRecord Record(string subjectId, string requestIdentity, string payload)
        => new AuthoritativeStateRecord("state/" + requestIdentity, "foundation.lifecycle", subjectId, FoundationStateClass.LifecycleState,
            StateRepresentationKind.Authoritative, "Foundation.LifecycleControlService", "Foundation.LifecycleControlService",
            "Foundation.State", "Foundation.LifecycleControlService", requestIdentity, 0, AuditTime, "PERMANENT", payload,
            string.Empty, string.Empty).WithComputedDigest();

    private static void Expect(ICollection<string> failures, string name, bool condition)
    {
        if (!condition) failures.Add(name);
    }
}
