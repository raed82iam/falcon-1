using System;
using System.Collections.Generic;
using System.IO;
using Foundation.Authority;
using Foundation.Contracts;
using Foundation.Core;
using Foundation.Evidence;
using Foundation.Infrastructure;
using Foundation.State;
using System.Linq;
using System.Reflection;

namespace Falcon.Stage4.WP04.Verifier;

internal static class Program
{
    private static readonly DateTimeOffset Now =
        DateTimeOffset.Parse("2026-08-05T20:00:00+00:00");

    private static int Main()
    {
        var failures = new List<string>();
        var root = Path.Combine(
            Path.GetTempPath(),
            "falcon-stage4-wp04-" + Guid.NewGuid().ToString("N"));

        try
        {
            var registry = new StateOwnershipRegistry();
            var declaration = Declaration("subject/wp03");
            Expect(failures, "ownership accepted",
                registry.Register(declaration) == DurableStateClassification.Accepted);

            Expect(failures, "identical ownership idempotent",
                registry.Register(declaration) == DurableStateClassification.Accepted);

            Expect(failures, "conflicting ownership rejected",
                registry.Register(declaration with { WriteAuthority = "writer/other" }) ==
                DurableStateClassification.OwnershipConflict);

            var provider = new FileAuthoritativeStateProvider(root);
            var store = new DurableAuthoritativeStateStore(registry, provider);

            var missing = store.Read(
                "foundation.lifecycle",
                "subject/wp03",
                FoundationStateClass.LifecycleState);
            Expect(failures, "missing classified",
                missing.Classification == DurableStateClassification.Missing);

            var version0 = Record(
                "subject/wp03",
                0,
                "REGISTERED",
                string.Empty,
                StateRepresentationKind.Authoritative,
                "Foundation.LifecycleControlService");

            var accepted0 = store.Write(version0, -1);
            Expect(failures, "initial state persisted",
                accepted0.Accepted && accepted0.Current?.StateVersion == 0);

            var reload0 = store.Read(
                "foundation.lifecycle",
                "subject/wp03",
                FoundationStateClass.LifecycleState);
            Expect(failures, "deterministic reload",
                reload0.Accepted &&
                reload0.Current == accepted0.Current);

            var stale = store.Write(
                Record(
                    "subject/wp03",
                    1,
                    "INITIALIZING",
                    accepted0.Current!.RecordDigest,
                    StateRepresentationKind.Authoritative,
                    "Foundation.LifecycleControlService"),
                -1);
            Expect(failures, "stale expected version rejected",
                stale.Classification == DurableStateClassification.StaleExpectedVersion);

            var unauthorized = store.Write(
                Record(
                    "subject/wp03",
                    1,
                    "INITIALIZING",
                    accepted0.Current.RecordDigest,
                    StateRepresentationKind.Authoritative,
                    "writer/other"),
                0);
            Expect(failures, "unauthorized writer rejected",
                unauthorized.Classification == DurableStateClassification.UnauthorizedWriter);

            var cached = store.Write(
                Record(
                    "subject/wp03",
                    1,
                    "INITIALIZING",
                    accepted0.Current.RecordDigest,
                    StateRepresentationKind.Cached,
                    "Foundation.LifecycleControlService"),
                0);
            Expect(failures, "cache never authoritative",
                cached.Classification == DurableStateClassification.Malformed ||
                cached.Classification == DurableStateClassification.NonAuthoritativeRepresentation);

            var version1 = Record(
                "subject/wp03",
                1,
                "INITIALIZING",
                accepted0.Current.RecordDigest,
                StateRepresentationKind.Authoritative,
                "Foundation.LifecycleControlService");
            var accepted1 = store.Write(version1, 0);
            Expect(failures, "successor persisted",
                accepted1.Accepted && accepted1.Current?.StateVersion == 1);

            var history0 = FindHistory(root, 0);
            Expect(failures, "prior history retained", history0 is not null);

            File.AppendAllText(
                FindCurrent(root)!,
                "CORRUPTION");

            var corrupted = store.Read(
                "foundation.lifecycle",
                "subject/wp03",
                FoundationStateClass.LifecycleState);
            Expect(failures, "corruption classified",
                corrupted.Classification == DurableStateClassification.Corrupted);

            var ownershipMissingStore = new DurableAuthoritativeStateStore(
                new StateOwnershipRegistry(),
                new FileAuthoritativeStateProvider(
                    Path.Combine(root, "missing-owner")));
            var ownershipMissing = ownershipMissingStore.Write(version0, -1);
            Expect(failures, "missing ownership fails closed",
                ownershipMissing.Classification == DurableStateClassification.OwnershipMissing);

            VerifyAllStateClasses(failures);
            VerifyLifecycleIntegration(failures, root);
            VerifyEvidenceJournal(failures, root);

            if (failures.Count > 0)
            {
                Console.Error.WriteLine("Stage 4 WP-03 verifier: FAIL");
                foreach (var failure in failures) Console.Error.WriteLine("- " + failure);
                return 1;
            }

            Console.WriteLine("Stage 4 WP-04 verifier: PASS");
            Console.WriteLine("FDN-002 canonical evidence identity, integrity-linked evidence, controlled accepted facts, durable restart evidence blocking, tamper-evident completion block store, post-commit failure blocking, tamper detection, correction append, and deterministic replay verified.");
            Console.WriteLine("Application business state remains out of scope; time is not used as an evidence validity gate.");
            Console.WriteLine("State digest: " + accepted1.Current!.RecordDigest);
            return 0;
        }
        finally
        {
            if (Directory.Exists(root))
            {
                Directory.Delete(root, recursive: true);
            }
        }
    }

    private static void VerifyAllStateClasses(ICollection<string> failures)
    {
        var values = Enum.GetValues<FoundationStateClass>();
        Expect(failures, "exact eight state classes", values.Length == 8);
        Expect(failures, "lifecycle state class", Array.Exists(values, v => v == FoundationStateClass.LifecycleState));
        Expect(failures, "authority policy class", Array.Exists(values, v => v == FoundationStateClass.AuthorityPolicyBaseline));
        Expect(failures, "authority decision class", Array.Exists(values, v => v == FoundationStateClass.AuthorityDecision));
        Expect(failures, "ownership declaration class", Array.Exists(values, v => v == FoundationStateClass.StateOwnershipDeclaration));
        Expect(failures, "operational evidence class", Array.Exists(values, v => v == FoundationStateClass.OperationalEvidence));
        Expect(failures, "accepted fact class", Array.Exists(values, v => v == FoundationStateClass.AcceptedFactEvent));
        Expect(failures, "commit state class", Array.Exists(values, v => v == FoundationStateClass.PersistenceCommitState));
        Expect(failures, "reconciliation class", Array.Exists(values, v => v == FoundationStateClass.ReconciliationState));
    }

    private static StateOwnershipDeclaration Declaration(string subjectId) => new(
        "ownership/foundation.lifecycle/" + subjectId,
        "foundation.lifecycle",
        subjectId,
        FoundationStateClass.LifecycleState,
        "Foundation.LifecycleControlService",
        "DurableAuthoritativeLifecycleRecord",
        "Foundation.State.FileAuthoritativeStateProvider",
        "Foundation.ControlPlaneReaders",
        "Foundation.LifecycleControlService",
        "FULL_HISTORY",
        1,
        Now.AddHours(-1),
        Now.AddHours(1));

    private static AuthoritativeStateRecord Record(
        string subjectId,
        long version,
        string payload,
        string previousDigest,
        StateRepresentationKind representation,
        string writer) => new(
            $"record/{subjectId}/{version}",
            "foundation.lifecycle",
            subjectId,
            FoundationStateClass.LifecycleState,
            representation,
            "Foundation.LifecycleControlService",
            "DurableAuthoritativeLifecycleRecord",
            "Foundation.State.FileAuthoritativeStateProvider",
            writer,
            $"source/{subjectId}/{version}",
            version,
            Now.AddMinutes(version),
            "FULL_HISTORY",
            payload,
            previousDigest,
            string.Empty);

    private static string? FindCurrent(string root)
    {
        var files = Directory.GetFiles(root, "current.state", SearchOption.AllDirectories);
        return files.Length == 1 ? files[0] : null;
    }

    private static string? FindHistory(string root, long version)
    {
        var name = version.ToString("D20") + ".state";
        var files = Directory.GetFiles(root, name, SearchOption.AllDirectories);
        return files.Length == 1 ? files[0] : null;
    }

    private static void Expect(
        ICollection<string> failures,
        string scenario,
        bool condition)
    {
        if (!condition) failures.Add(scenario);
    }

    private static void VerifyLifecycleIntegration(
        ICollection<string> failures,
        string root)
    {
        var lifecycleRoot = Path.Combine(root, "lifecycle-integration");
        var ownership = new StateOwnershipRegistry();
        var subjectId = "subject/wp03/lifecycle";
        ownership.Register(Declaration(subjectId));

        var provider = new FileAuthoritativeStateProvider(lifecycleRoot);
        var store = new DurableAuthoritativeStateStore(ownership, provider);
        var service = CreateRegisteredLifecycleService(subjectId, store);

        var durable0 = store.Read(
            "foundation.lifecycle",
            subjectId,
            FoundationStateClass.LifecycleState);

        Expect(
            failures,
            "registration persists durable version zero",
            durable0.Accepted &&
            durable0.Current?.StateVersion == 0 &&
            durable0.Current.Payload == "REGISTERED");

        var accepted = ExecuteAuthorizedTransition(service, subjectId);
        var durable1 = store.Read(
            "foundation.lifecycle",
            subjectId,
            FoundationStateClass.LifecycleState);

        Console.WriteLine(
            $"DIAG accepted authority={accepted.AuthorityResult.Decision}/{accepted.AuthorityResult.Reason}; " +
            $"lifecycle={accepted.LifecycleDecision.ContractResult.Decision}/{accepted.LifecycleDecision.ContractResult.Reason}; " +
            $"memory={service.GetSnapshot(subjectId)?.State}/{service.GetSnapshot(subjectId)?.StateVersion}; " +
            $"durable={durable1.Classification}/{durable1.Reason}/{durable1.Current?.Payload}/{durable1.Current?.StateVersion}; " +
            $"events={service.GetEvents().Count}");

        Expect(
            failures,
            "accepted transition persists one successor",
            accepted.LifecycleDecision.ContractResult.Decision == "ACCEPTED" &&
            service.GetSnapshot(subjectId)?.State == LifecycleState.Initializing &&
            durable1.Accepted &&
            durable1.Current?.StateVersion == 1 &&
            durable1.Current.Payload == "INITIALIZING" &&
            service.GetEvents().Count == 1);

        var beforeSnapshot = service.GetSnapshot(subjectId);
        var beforeDigest = durable1.Current!.RecordDigest;

        var rejectingSubject = "subject/wp03/rejecting";
        var rejectingOwnership = new StateOwnershipRegistry();
        rejectingOwnership.Register(Declaration(rejectingSubject));
        var rejectingProvider = new FileAuthoritativeStateProvider(
            Path.Combine(root, "rejecting"));
        var rejectingStore = new DurableAuthoritativeStateStore(
            rejectingOwnership,
            new RejectingProvider(
                rejectingProvider,
                DurableStateClassification.Conflicting,
                "SIMULATED_COMMIT_REJECTION"));
        var rejectingService = CreateRegisteredLifecycleService(
            rejectingSubject,
            rejectingStore);

        var rejectingSnapshotBefore =
            rejectingService.GetSnapshot(rejectingSubject);

        var rejectingDecision = ExecuteAuthorizedTransition(
            rejectingService,
            rejectingSubject);

        Console.WriteLine(
            $"DIAG rejecting authority={rejectingDecision.AuthorityResult.Decision}/{rejectingDecision.AuthorityResult.Reason}; " +
            $"lifecycle={rejectingDecision.LifecycleDecision.ContractResult.Decision}/{rejectingDecision.LifecycleDecision.ContractResult.Reason}; " +
            $"before={rejectingSnapshotBefore?.State}/{rejectingSnapshotBefore?.StateVersion}; " +
            $"after={rejectingService.GetSnapshot("subject/wp03/rejecting")?.State}/{rejectingService.GetSnapshot("subject/wp03/rejecting")?.StateVersion}; " +
            $"events={rejectingService.GetEvents().Count}");

        Expect(
            failures,
            "provider rejection leaves lifecycle unchanged",
            rejectingDecision.LifecycleDecision.ContractResult.Decision == "REJECTED" &&
            rejectingDecision.LifecycleDecision.ContractResult.Reason.Contains(
                "AUTHORITATIVE_STATE_PERSISTENCE_REJECTED",
                StringComparison.Ordinal) &&
            rejectingService.GetSnapshot("subject/wp03/rejecting") ==
                rejectingSnapshotBefore &&
            rejectingService.GetEvents().Count == 0);

        var partialRoot = Path.Combine(root, "partial");
        var partialOwnership = new StateOwnershipRegistry();
        var partialSubject = "subject/wp03/partial";
        partialOwnership.Register(Declaration(partialSubject));
        var partialProvider = new FileAuthoritativeStateProvider(partialRoot);
        var partialStore = new DurableAuthoritativeStateStore(
            partialOwnership,
            partialProvider);

        var initial = partialStore.Write(
            Record(
                partialSubject,
                0,
                "REGISTERED",
                string.Empty,
                StateRepresentationKind.Authoritative,
                "Foundation.LifecycleControlService"),
            -1);

        var currentPath = Directory.GetFiles(
            partialRoot,
            "current.state",
            SearchOption.AllDirectories)[0];
        File.Delete(currentPath);

        var partialWrite = partialStore.Write(
            Record(
                partialSubject,
                1,
                "INITIALIZING",
                initial.Current!.RecordDigest,
                StateRepresentationKind.Authoritative,
                "Foundation.LifecycleControlService"),
            0);

        Expect(
            failures,
            "partial durable state rejects writes",
            partialWrite.Classification == DurableStateClassification.Partial);

        Expect(
            failures,
            "existing durable state remains deterministic",
            service.GetSnapshot(subjectId) == beforeSnapshot &&
            store.Read(
                "foundation.lifecycle",
                subjectId,
                FoundationStateClass.LifecycleState).Current?.RecordDigest ==
                beforeDigest);
    }

    private sealed class RejectingProvider : IAuthoritativeStateProvider
    {
        private readonly IAuthoritativeStateProvider _inner;
        private readonly DurableStateClassification _classification;
        private readonly string _reason;

        public RejectingProvider(
            IAuthoritativeStateProvider inner,
            DurableStateClassification classification,
            string reason)
        {
            _inner = inner;
            _classification = classification;
            _reason = reason;
        }

        public DurableStateReadResult ReadCurrent(
            string stateNamespace,
            string subjectId,
            FoundationStateClass stateClass)
            => _inner.ReadCurrent(stateNamespace, subjectId, stateClass);

        public DurableStateWriteResult WriteCurrent(
            AuthoritativeStateRecord record,
            long expectedVersion)
        {
            if (record.StateVersion > 0)
            {
                return new DurableStateWriteResult(
                    _classification,
                    _reason,
                    null,
                    string.Empty);
            }

            return _inner.WriteCurrent(record, expectedVersion);
        }
    }

    private static DurableAuthoritativeStateStore CreateStoreForSubject(
        string subjectId,
        string root,
        out FileAuthoritativeStateProvider provider,
        out StateOwnershipRegistry ownership)
    {
        ownership = new StateOwnershipRegistry();
        ownership.Register(Declaration(subjectId));
        provider = new FileAuthoritativeStateProvider(root);
        return new DurableAuthoritativeStateStore(ownership, provider);
    }

    private static LifecycleControlService CreateRegisteredLifecycleService(
        string subjectId,
        DurableAuthoritativeStateStore store)
    {
        var service = new LifecycleControlService(
            LifecycleStateModel.CreateCanonical(),
            LifecycleAuthorityMode.AuthorityEngineRequired,
            store);

        var registration = service.Register(
            BuildBootstrapRequest(subjectId),
            "registration-evidence/" + subjectId);

        if (!registration.Registration.Accepted)
        {
            throw new InvalidOperationException(
                "Lifecycle registration fixture failed: " +
                registration.Registration.ReasonCode);
        }

        return service;
    }

    private static LifecycleAuthorityControlDecision ExecuteAuthorizedTransition(
        LifecycleControlService service,
        string subjectId)
    {
        var request = new LifecycleTransitionRequest(
            "request/" + subjectId,
            subjectId,
            "REGISTERED",
            "INITIALIZING",
            "actor/foundation",
            string.Empty,
            "bootstrap initialization",
            "dependency-evidence:" + subjectId,
            Now.AddMinutes(1),
            Now.AddMinutes(30));

        var evidence = new LifecycleTransitionEvidence
        {
            TransitionId = "transition/" + subjectId,
            EventId = "event/" + subjectId,
            ModelId = LifecycleStateModel.CreateCanonical().ModelId,
            ModelVersion = LifecycleStateModel.CreateCanonical().Version,
            ExpectedStateVersion = 1,
            BootstrapContextId = "context:" + subjectId + ":1.0",
            TimeProvider = BuildTimeProvider(),
            ObservationTime = Now.AddMinutes(2)
        };

        var evaluation = BuildEvaluation(request);
        return service.TransitionAuthorized(request, evidence, evaluation);
    }

    private static BootstrapValidationRequest BuildBootstrapRequest(string subjectId)
    {
        const string artifactDigest =
            "A1A2A3A4A5A6A7A8A9AAABACADAEAFB0B1B2B3B4B5B6B7B8B9BABBBCBDBEBFC0";
        const string manifestDigest =
            "C1C2C3C4C5C6C7C8C9CACBCCCDCECFC0D1D2D3D4D5D6D7D8D9DADBDCDDDEDFE0";

        var subject = new BootstrapSubjectAdmissionEvidence
        {
            SubjectId = subjectId,
            SubjectVersion = "1.0",
            SubjectKind = BootstrapSubjectKind.Service,
            ArtifactIdentity = $"artifact:{subjectId}:1.0",
            ArtifactDigest = artifactDigest,
            ManifestIdentity = $"manifest:{subjectId}:1.0",
            ManifestDigest = manifestDigest,
            AdmissionDecisionId = $"admission:{subjectId}:1.0",
            AdmissionState = "ADMITTED",
            RegistrationEvidenceId = $"registration:{subjectId}:1.0",
            RegistrationState = "REGISTERED",
            EvidenceAuthority = "FALCON-STAGE3-WP05-EVIDENCE",
            EffectiveTime = Now.AddHours(-1),
            Expiry = Now.AddHours(2)
        };

        return new BootstrapValidationRequest
        {
            RequestId = $"bootstrap-request:{subjectId}:1.0",
            Subject = subject,
            Context = new BootstrapExecutionContextRecord(
                $"context:{subjectId}:1.0",
                "1.0",
                "FALCON-STAGE3-WP05-AUTHORITY",
                "ENV-STAGE3-WP05-ISOLATED",
                "STAGE3-WP05-BOOTSTRAP-AND-LIFECYCLE",
                "EXTERNAL-BOOTSTRAP-CONTROL",
                $"context-evidence:{subjectId}",
                "DEFINED",
                "NO-PRODUCTION-NO-FINANCIAL-NO-EXTERNAL-CONNECTIVITY",
                Now.AddHours(-1),
                Now.AddHours(2)),
            Provenance = new BootstrapEvidenceProvenanceRecord(
                $"provenance:{subjectId}:1.0",
                "1.0",
                $"source-record:{subjectId}:1.0",
                artifactDigest,
                "EXTERNAL-BOOTSTRAP-CONTROL",
                "FALCON-STAGE3-WP05-PROVENANCE-AUTHORITY",
                $"provenance-evidence:{subjectId}",
                "PROVEN",
                subject.ArtifactIdentity,
                Now.AddHours(-1),
                Now.AddHours(2)),
            TimeProvider = BuildTimeProvider(),
            DependencyEvidence = new DependencyActivationEvidence
            {
                SubjectId = subjectId,
                SubjectVersion = "1.0",
                GraphId = "stage3-wp04-golden-graph",
                GraphVersion = "1.0",
                GraphDigest =
                    "D06C6EDE16D2A55F4FBA36B965C5EECA0A98CE5AE11CE711ABCB4E8FECFF992E",
                DependencyValidationState = "VALIDATED",
                ActivationOrderState = "VALIDATED",
                SubjectActivationIndex = 4,
                EvidenceReference = $"dependency-evidence:{subjectId}",
                EffectiveTime = Now.AddHours(-1),
                Expiry = Now.AddHours(2)
            },
            ObservationTime = Now
        };
    }

    private static TimeProviderRecord BuildTimeProvider() => new(
        "TIME-PROVIDER-ACTIVE-001",
        "1.0",
        "FOUNDATION_TIME_PROVIDER",
        "GOV-027",
        "STAGE3-WP05-ISOLATED",
        "FALCON-GOVERNED-TIME",
        "TIME-EVIDENCE-001",
        "ADMITTED",
        Now.AddHours(-1),
        Now.AddHours(2));

    private static LifecycleAuthorityEvaluation BuildEvaluation(
        LifecycleTransitionRequest request)
    {
        var scope =
            $"LIFECYCLE:{request.ComponentIdentity}:{request.AuthoritativeSourceState}->{request.RequestedTargetState}";
        var policy = new AuthorityPolicy(
            "policy/stage4/wp03/" + request.ComponentIdentity.Replace('/', '-'),
            "1.0",
            "authority/owner-approved",
            Now.AddHours(-1),
            Now.AddHours(1),
            new[] { request.Requester },
            new[] { "lifecycle.transition" },
            new[] { "lifecycle:" + request.ComponentIdentity },
            new[] { "authoritative-lifecycle-transition" },
            new[] { scope },
            new[] { "foundation-internal" });

        var delegation = new DelegationEvidence(
            "delegation/stage4/wp03/" + request.ComponentIdentity.Replace('/', '-'),
            request.Requester,
            "authority/owner-approved",
            new[] { scope },
            Now.AddHours(-1),
            Now.AddHours(1),
            false);

        var fitness = new FitnessEvidence(
            request.Requester,
            "FIT",
            true,
            Now.AddMinutes(-5),
            Now.AddMinutes(20),
            "evidence/fitness/stage4/wp03");

        return new LifecycleAuthorityEvaluation(
            "foundation-internal",
            "FIT",
            new AuthorityEvaluationContext(
                policy,
                delegation,
                fitness,
                Now.AddMinutes(2),
                "evidence/authority/stage4/wp03/" +
                request.ComponentIdentity.Replace('/', '-')));
    }


    private static void VerifyEvidenceJournal(
        ICollection<string> failures,
        string root)
    {
        var evidenceRoot = Path.Combine(root, "evidence-journal");
        var provider = new FileEvidenceJournalProvider(evidenceRoot);
        var journal = new IntegrityLinkedEvidenceJournal(provider);
        var publisher = new AcceptedFactPublisher(journal);

        var firstRequest = new EvidenceAppendRequest(
            string.Empty,
            "actor/foundation",
            "request/evidence/1",
            EvidenceDecisionKind.Allow,
            "decision/evidence/1",
            "AUTHORITY_ALLOWED",
            EvidenceExecutionOutcome.Accepted,
            EvidencePersistenceOutcome.Accepted,
            "foundation.lifecycle",
            "subject/evidence/1",
            0,
            "STATE-DIGEST-0001",
            "Foundation.LifecycleControlService",
            string.Empty);

        var first = journal.Append(firstRequest);
        var firstReplay = journal.Append(firstRequest);

        var forgedIdentity = journal.Append(
            firstRequest with { EvidenceId = "evidence/forged" });

        Expect(
            failures,
            "first journal append",
            first.Accepted &&
            first.Record?.Sequence == 0 &&
            !string.IsNullOrWhiteSpace(first.Record.RecordDigest));

        Expect(
            failures,
            "deterministic duplicate append",
            firstReplay.Accepted &&
            firstReplay.Idempotent &&
            firstReplay.Record?.RecordDigest == first.Record?.RecordDigest);

        Expect(
            failures,
            "forged evidence identity rejected",
            !forgedIdentity.Accepted &&
            forgedIdentity.Reason == "EVIDENCE_ID_CANONICAL_MISMATCH");

        var deny = journal.Append(
            new EvidenceAppendRequest(
                string.Empty,
                "actor/foundation",
                "request/evidence/deny",
                EvidenceDecisionKind.Deny,
                "decision/evidence/deny",
                "AUTHORITY_DENIED",
                EvidenceExecutionOutcome.Rejected,
                EvidencePersistenceOutcome.NotAttempted,
                "foundation.lifecycle",
                "subject/evidence/1",
                0,
                "STATE-DIGEST-0001",
                "Foundation.LifecycleControlService",
                string.Empty));

        Expect(
            failures,
            "deny evidence appended",
            deny.Accepted &&
            deny.Record?.Decision == EvidenceDecisionKind.Deny &&
            deny.Record.ExecutionOutcome == EvidenceExecutionOutcome.Rejected);

        var correction = journal.AppendCorrection(
            new EvidenceAppendRequest(
                string.Empty,
                "actor/foundation",
                "request/evidence/correction",
                EvidenceDecisionKind.Allow,
                "decision/evidence/correction",
                "CORRECTION_APPENDED",
                EvidenceExecutionOutcome.Accepted,
                EvidencePersistenceOutcome.NotAttempted,
                "foundation.lifecycle",
                "subject/evidence/1",
                0,
                "STATE-DIGEST-0001",
                "Foundation.LifecycleControlService",
                string.Empty),
            first.Record!.EvidenceId);

        Expect(
            failures,
            "correction appends new record",
            correction.Accepted &&
            correction.Record?.Sequence == 2 &&
            correction.Record.CorrectionOfEvidenceId == first.Record.EvidenceId);

        var read = journal.Read();
        Expect(
            failures,
            "journal chain validates",
            read.Accepted &&
            read.Records.Count == 3 &&
            read.Records[1].PreviousRecordDigest == read.Records[0].RecordDigest &&
            read.Records[2].PreviousRecordDigest == read.Records[1].RecordDigest);

        var publicFactAppend = typeof(FileEvidenceJournalProvider).GetMethod(
            "AppendAcceptedFact",
            BindingFlags.Public | BindingFlags.Instance);

        Expect(
            failures,
            "accepted fact provider bypass unavailable",
            publicFactAppend is null);

        var ownership = new StateOwnershipRegistry();
        var subjectId = "subject/wp04/lifecycle";
        ownership.Register(Declaration(subjectId));
        var stateProvider = new FileAuthoritativeStateProvider(
            Path.Combine(root, "wp04-state"));
        var stateStore = new DurableAuthoritativeStateStore(
            ownership,
            stateProvider);

        var fabricatedStateRoot = Path.Combine(root, "fabricated-state");
        var fabricatedOwnership = new StateOwnershipRegistry();
        var fabricatedSubject = "subject/wp04/fabricated";
        fabricatedOwnership.Register(Declaration(fabricatedSubject));
        var fabricatedStore = new DurableAuthoritativeStateStore(
            fabricatedOwnership,
            new FileAuthoritativeStateProvider(fabricatedStateRoot));
        var fabricatedPersistence = fabricatedStore.Write(
            Record(
                fabricatedSubject,
                0,
                "REGISTERED",
                string.Empty,
                StateRepresentationKind.Authoritative,
                "Foundation.LifecycleControlService"),
            -1);
        var fabricatedRecord = new IntegrityLinkedEvidenceRecord(
            0,
            "evidence/sha256/FABRICATED",
            "actor/foundation",
            "request/fabricated",
            EvidenceDecisionKind.Allow,
            "decision/fabricated",
            "AUTHORITY_ALLOWED",
            EvidenceExecutionOutcome.Accepted,
            EvidencePersistenceOutcome.Accepted,
            "foundation.lifecycle",
            fabricatedSubject,
            0,
            fabricatedPersistence.Current!.RecordDigest,
            "WP04Verifier",
            string.Empty,
            string.Empty,
            string.Empty).WithComputedDigest();
        var fabricatedPublish = publisher.Publish(
            new EvidenceAppendResult(
                EvidenceJournalClassification.Accepted,
                "FABRICATED_RESULT",
                fabricatedRecord,
                false),
            fabricatedPersistence,
            "LIFECYCLE_STATE_TRANSITION");

        Expect(
            failures,
            "fabricated accepted fact evidence rejected",
            !fabricatedPublish.Accepted &&
            fabricatedPublish.Reason ==
                "EVIDENCE_NOT_PRESENT_IN_ACCEPTED_JOURNAL");

        var service = new LifecycleControlService(
            LifecycleStateModel.CreateCanonical(),
            LifecycleAuthorityMode.AuthorityEngineRequired,
            stateStore,
            journal,
            publisher);

        var registration = service.Register(
            BuildBootstrapRequest(subjectId),
            "registration-evidence/" + subjectId);

        Expect(
            failures,
            "evidence lifecycle registration",
            registration.Registration.Accepted);

        var transition = ExecuteAuthorizedTransition(service, subjectId);
        var lifecycleEvidence = service.GetEvidenceJournal();
        var facts = service.GetAcceptedFacts();

        Expect(
            failures,
            "accepted transition journaled",
            transition.LifecycleDecision.ContractResult.Decision == "ACCEPTED" &&
            lifecycleEvidence?.Accepted == true &&
            lifecycleEvidence.Records.Any(record =>
                record.RequestIdentity == "request/" + subjectId &&
                record.Decision == EvidenceDecisionKind.Allow &&
                record.PersistenceOutcome == EvidencePersistenceOutcome.Accepted));

        Expect(
            failures,
            "accepted fact after durable commit",
            facts.Count == 1 &&
            facts[0].SubjectId == subjectId &&
            facts[0].StateVersion == 1);

        var rejectSubject = "subject/wp04/rejecting";
        var rejectOwnership = new StateOwnershipRegistry();
        rejectOwnership.Register(Declaration(rejectSubject));
        var rejectBaseProvider = new FileAuthoritativeStateProvider(
            Path.Combine(root, "wp04-reject-state"));
        var rejectingProvider = new RejectingProvider(
            rejectBaseProvider,
            DurableStateClassification.Conflicting,
            "SIMULATED_WP04_COMMIT_REJECTION");
        var rejectStore = new DurableAuthoritativeStateStore(
            rejectOwnership,
            rejectingProvider);
        var rejectJournal = new IntegrityLinkedEvidenceJournal(
            new FileEvidenceJournalProvider(
                Path.Combine(root, "wp04-reject-evidence")));
        var rejectPublisher = new AcceptedFactPublisher(rejectJournal);
        var rejectService = new LifecycleControlService(
            LifecycleStateModel.CreateCanonical(),
            LifecycleAuthorityMode.AuthorityEngineRequired,
            rejectStore,
            rejectJournal,
            rejectPublisher);

        var rejectRegistration = rejectService.Register(
            BuildBootstrapRequest(rejectSubject),
            "registration-evidence/" + rejectSubject);
        Expect(
            failures,
            "rejecting lifecycle registration",
            rejectRegistration.Registration.Accepted);

        var rejected = ExecuteAuthorizedTransition(
            rejectService,
            rejectSubject);
        var rejectedFacts = rejectService.GetAcceptedFacts();
        var rejectedJournal = rejectService.GetEvidenceJournal();

        Expect(
            failures,
            "commit rejection journaled without accepted fact",
            rejected.LifecycleDecision.ContractResult.Decision == "REJECTED" &&
            rejectedFacts.Count == 0 &&
            rejectedJournal?.Records.Count == 1 &&
            rejectedJournal.Records[0].PersistenceOutcome ==
                EvidencePersistenceOutcome.Rejected);

        VerifyPostCommitEvidenceFailures(failures, root);
        VerifyCompletionBlockStoreTamper(failures, root);
        VerifyTamperScenarios(failures, root);
    }

    private static void VerifyPostCommitEvidenceFailures(
        ICollection<string> failures,
        string root)
    {
        var appendSubject = "subject/wp04/evidence-append-failure";
        var appendOwnership = new StateOwnershipRegistry();
        appendOwnership.Register(Declaration(appendSubject));
        var appendStore = new DurableAuthoritativeStateStore(
            appendOwnership,
            new FileAuthoritativeStateProvider(
                Path.Combine(root, "post-commit-append-state")));
        var appendEvidenceRoot = Path.Combine(root, "post-commit-append-evidence");
        Directory.CreateDirectory(appendEvidenceRoot);
        File.WriteAllText(
            Path.Combine(appendEvidenceRoot, "journal.ndjson"),
            string.Empty);
        var appendJournal = new IntegrityLinkedEvidenceJournal(
            new FileEvidenceJournalProvider(appendEvidenceRoot));
        var appendService = new LifecycleControlService(
            LifecycleStateModel.CreateCanonical(),
            LifecycleAuthorityMode.AuthorityEngineRequired,
            appendStore,
            appendJournal,
            new AcceptedFactPublisher(appendJournal));

        var appendRegistration = appendService.Register(
            BuildBootstrapRequest(appendSubject),
            "registration-evidence/" + appendSubject);
        var appendFailure = ExecuteAuthorizedTransition(
            appendService,
            appendSubject);
        var appendDurable = appendStore.Read(
            "foundation.lifecycle",
            appendSubject,
            FoundationStateClass.LifecycleState);
        var appendBlocked = ExecuteAuthorizedTransition(
            appendService,
            appendSubject);
        var restartedAppendService = new LifecycleControlService(
            LifecycleStateModel.CreateCanonical(),
            LifecycleAuthorityMode.AuthorityEngineRequired,
            appendStore,
            appendJournal,
            new AcceptedFactPublisher(appendJournal));

        Expect(
            failures,
            "post-commit evidence append failure classified and blocked",
            appendRegistration.Registration.Accepted &&
            appendFailure.LifecycleDecision.ContractResult.Decision == "FAILED" &&
            appendFailure.LifecycleDecision.ContractResult.Reason.StartsWith(
                "POST_COMMIT_EVIDENCE_APPEND_REJECTED:",
                StringComparison.Ordinal) &&
            appendDurable.Accepted &&
            appendDurable.Current?.StateVersion == 1 &&
            appendService.GetSnapshot(appendSubject)?.State ==
                LifecycleState.Initializing &&
            appendService.GetEvents().Count == 1 &&
            appendService.IsEvidenceCompletionBlocked(appendSubject) &&
            restartedAppendService.IsEvidenceCompletionBlocked(appendSubject) &&
            appendBlocked.LifecycleDecision.ContractResult.Decision == "REJECTED" &&
            appendBlocked.LifecycleDecision.ContractResult.Reason ==
                "POST_COMMIT_EVIDENCE_COMPLETION_REQUIRED");

        var factSubject = "subject/wp04/fact-append-failure";
        var factOwnership = new StateOwnershipRegistry();
        factOwnership.Register(Declaration(factSubject));
        var factStore = new DurableAuthoritativeStateStore(
            factOwnership,
            new FileAuthoritativeStateProvider(
                Path.Combine(root, "post-commit-fact-state")));
        var factEvidenceRoot = Path.Combine(root, "post-commit-fact-evidence");
        Directory.CreateDirectory(factEvidenceRoot);
        File.WriteAllText(
            Path.Combine(factEvidenceRoot, "accepted-facts.ndjson"),
            "CORRUPTED");
        var factJournal = new IntegrityLinkedEvidenceJournal(
            new FileEvidenceJournalProvider(factEvidenceRoot));
        var factService = new LifecycleControlService(
            LifecycleStateModel.CreateCanonical(),
            LifecycleAuthorityMode.AuthorityEngineRequired,
            factStore,
            factJournal,
            new AcceptedFactPublisher(factJournal));

        var factRegistration = factService.Register(
            BuildBootstrapRequest(factSubject),
            "registration-evidence/" + factSubject);
        var factFailure = ExecuteAuthorizedTransition(
            factService,
            factSubject);
        var factDurable = factStore.Read(
            "foundation.lifecycle",
            factSubject,
            FoundationStateClass.LifecycleState);
        var factBlocked = ExecuteAuthorizedTransition(
            factService,
            factSubject);
        var restartedFactService = new LifecycleControlService(
            LifecycleStateModel.CreateCanonical(),
            LifecycleAuthorityMode.AuthorityEngineRequired,
            factStore,
            factJournal,
            new AcceptedFactPublisher(factJournal));

        Expect(
            failures,
            "post-commit accepted fact failure classified and blocked",
            factRegistration.Registration.Accepted &&
            factFailure.LifecycleDecision.ContractResult.Decision == "FAILED" &&
            factFailure.LifecycleDecision.ContractResult.Reason.StartsWith(
                "POST_COMMIT_ACCEPTED_FACT_REJECTED:",
                StringComparison.Ordinal) &&
            factDurable.Accepted &&
            factDurable.Current?.StateVersion == 1 &&
            factService.GetSnapshot(factSubject)?.State ==
                LifecycleState.Initializing &&
            factService.GetEvents().Count == 1 &&
            factService.IsEvidenceCompletionBlocked(factSubject) &&
            restartedFactService.IsEvidenceCompletionBlocked(factSubject) &&
            factBlocked.LifecycleDecision.ContractResult.Decision == "REJECTED" &&
            factBlocked.LifecycleDecision.ContractResult.Reason ==
                "POST_COMMIT_EVIDENCE_COMPLETION_REQUIRED");
    }

    private static void VerifyCompletionBlockStoreTamper(
        ICollection<string> failures,
        string root)
    {
        var baselineRoot = Path.Combine(root, "completion-block-store-baseline");
        var baselineTrustedRoot = Path.Combine(root, "completion-block-trusted-baseline");
        var baselineJournal = new IntegrityLinkedEvidenceJournal(
            new FileEvidenceJournalProvider(baselineRoot, baselineTrustedRoot));

        var first = baselineJournal.BlockEvidenceCompletion(
            "subject/wp04/block-one",
            "FIRST_BLOCK");
        var generationOneRoot = Path.Combine(root, "completion-block-generation-one");
        CopyDirectory(baselineRoot, generationOneRoot);

        var second = baselineJournal.BlockEvidenceCompletion(
            "subject/wp04/block-two",
            "SECOND_BLOCK");

        Expect(
            failures,
            "completion block baseline accepted",
            first.Accepted &&
            second.Accepted &&
            baselineJournal.ReadEvidenceCompletionBlocks().Count == 2);

        VerifyCompletionBlockTamperScenario(
            failures,
            baselineRoot,
            baselineTrustedRoot,
            Path.Combine(root, "completion-block-delete-data"),
            scenarioRoot => File.Delete(
                Path.Combine(scenarioRoot, "evidence-completion-blocks.ndjson")),
            "completion block data deletion fails closed");

        VerifyCompletionBlockTamperScenario(
            failures,
            baselineRoot,
            baselineTrustedRoot,
            Path.Combine(root, "completion-block-delete-head"),
            scenarioRoot => File.Delete(
                Path.Combine(scenarioRoot, "evidence-completion-blocks.head")),
            "completion block head deletion fails closed");

        VerifyCompletionBlockTamperScenario(
            failures,
            baselineRoot,
            baselineTrustedRoot,
            Path.Combine(root, "completion-block-delete-anchor"),
            scenarioRoot => File.Delete(
                Path.Combine(scenarioRoot, "evidence-completion-blocks.anchor")),
            "completion block anchor deletion fails closed");

        VerifyCompletionBlockTamperScenario(
            failures,
            baselineRoot,
            baselineTrustedRoot,
            Path.Combine(root, "completion-block-truncate"),
            scenarioRoot =>
            {
                var dataPath = Path.Combine(
                    scenarioRoot,
                    "evidence-completion-blocks.ndjson");
                var firstLine = File.ReadLines(dataPath).First();
                File.WriteAllText(dataPath, firstLine + Environment.NewLine);
            },
            "completion block valid-prefix truncation fails closed");

        VerifyCompletionBlockTamperScenario(
            failures,
            baselineRoot,
            baselineTrustedRoot,
            Path.Combine(root, "completion-block-rollback"),
            scenarioRoot =>
            {
                File.Copy(
                    Path.Combine(
                        generationOneRoot,
                        "evidence-completion-blocks.ndjson"),
                    Path.Combine(
                        scenarioRoot,
                        "evidence-completion-blocks.ndjson"),
                    true);
                File.Copy(
                    Path.Combine(
                        generationOneRoot,
                        "evidence-completion-blocks.head"),
                    Path.Combine(
                        scenarioRoot,
                        "evidence-completion-blocks.head"),
                    true);
                File.Copy(
                    Path.Combine(
                        generationOneRoot,
                        "evidence-completion-blocks.anchor"),
                    Path.Combine(
                        scenarioRoot,
                        "evidence-completion-blocks.anchor"),
                    true);
            },
            "completion block complete-set rollback fails closed");

        VerifyCompletionBlockTamperScenario(
            failures,
            baselineRoot,
            baselineTrustedRoot,
            Path.Combine(root, "completion-block-delete-all"),
            scenarioRoot =>
            {
                File.Delete(Path.Combine(scenarioRoot, "evidence-completion-blocks.ndjson"));
                File.Delete(Path.Combine(scenarioRoot, "evidence-completion-blocks.head"));
                File.Delete(Path.Combine(scenarioRoot, "evidence-completion-blocks.anchor"));
            },
            "completion block complete deletion fails closed");

        var emptyJournal = new IntegrityLinkedEvidenceJournal(
            new FileEvidenceJournalProvider(
                Path.Combine(root, "completion-block-new-empty"),
                Path.Combine(root, "completion-block-new-empty-trusted")));
        Expect(
            failures,
            "genuinely new empty completion block store is allowed",
            emptyJournal.ReadEvidenceCompletionBlocks().Count == 0);
    }

    private static void VerifyCompletionBlockTamperScenario(
        ICollection<string> failures,
        string baselineRoot,
        string baselineTrustedRoot,
        string scenarioRoot,
        Action<string> tamper,
        string name)
    {
        var scenarioTrustedRoot = scenarioRoot + "-trusted";
        CopyDirectory(baselineRoot, scenarioRoot);
        CopyDirectory(baselineTrustedRoot, scenarioTrustedRoot);
        tamper(scenarioRoot);

        var failedClosed = false;
        try
        {
            var journal = new IntegrityLinkedEvidenceJournal(
                new FileEvidenceJournalProvider(
                    scenarioRoot,
                    scenarioTrustedRoot));
            _ = new LifecycleControlService(
                LifecycleStateModel.CreateCanonical(),
                LifecycleAuthorityMode.AuthorityEngineRequired,
                null,
                journal,
                new AcceptedFactPublisher(journal));
        }
        catch (InvalidDataException)
        {
            failedClosed = true;
        }

        Expect(failures, name, failedClosed);
    }

    private static void CopyDirectory(string source, string destination)
    {
        Directory.CreateDirectory(destination);

        foreach (var file in Directory.GetFiles(source))
        {
            File.Copy(
                file,
                Path.Combine(destination, Path.GetFileName(file)),
                true);
        }

        foreach (var directory in Directory.GetDirectories(source))
        {
            CopyDirectory(
                directory,
                Path.Combine(destination, Path.GetFileName(directory)));
        }
    }

    private static void VerifyTamperScenarios(
        ICollection<string> failures,
        string root)
    {
        foreach (var scenario in new[]
        {
            "delete",
            "insert",
            "replace",
            "reorder",
            "link",
            "truncate"
        })
        {
            var scenarioRoot = Path.Combine(root, "tamper-" + scenario);
            var provider = new FileEvidenceJournalProvider(scenarioRoot);
            var journal = new IntegrityLinkedEvidenceJournal(provider);

            for (var index = 0; index < 3; index++)
            {
                journal.Append(
                    new EvidenceAppendRequest(
                        string.Empty,
                        "actor/tamper",
                        "request/tamper/" + index,
                        EvidenceDecisionKind.Allow,
                        "decision/tamper/" + index,
                        "TAMPER_FIXTURE",
                        EvidenceExecutionOutcome.Accepted,
                        EvidencePersistenceOutcome.NotAttempted,
                        "foundation.lifecycle",
                        "subject/tamper",
                        index,
                        "DIGEST-" + index,
                        "WP04Verifier",
                        string.Empty));
            }

            var journalPath = Path.Combine(scenarioRoot, "journal.ndjson");
            var headPath = Path.Combine(scenarioRoot, "journal.head");
            var lines = File.ReadAllLines(journalPath).ToList();

            switch (scenario)
            {
                case "delete":
                    lines.RemoveAt(1);
                    File.WriteAllLines(journalPath, lines);
                    break;
                case "insert":
                    lines.Insert(1, lines[0]);
                    File.WriteAllLines(journalPath, lines);
                    break;
                case "replace":
                    File.WriteAllText(
                        journalPath,
                        File.ReadAllText(journalPath).Replace(
                            "TAMPER_FIXTURE",
                            "REPLACED",
                            StringComparison.Ordinal));
                    break;
                case "reorder":
                    (lines[0], lines[1]) = (lines[1], lines[0]);
                    File.WriteAllLines(journalPath, lines);
                    break;
                case "link":
                    File.WriteAllText(
                        journalPath,
                        File.ReadAllText(journalPath).Replace(
                            "\"PreviousRecordDigest\":\"",
                            "\"PreviousRecordDigest\":\"BROKEN",
                            StringComparison.Ordinal));
                    break;
                case "truncate":
                    lines.RemoveAt(lines.Count - 1);
                    File.WriteAllLines(journalPath, lines);
                    break;
            }

            var result = provider.ReadJournal();
            Expect(
                failures,
                "tamper detected: " + scenario,
                !result.Accepted);
        }
    }


}
