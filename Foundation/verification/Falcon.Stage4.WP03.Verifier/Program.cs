using System;
using System.Collections.Generic;
using System.IO;
using Foundation.Authority;
using Foundation.Contracts;
using Foundation.Core;
using Foundation.Infrastructure;
using Foundation.State;

namespace Falcon.Stage4.WP03.Verifier;

internal static class Program
{
    private static readonly DateTimeOffset Now =
        DateTimeOffset.Parse("2026-08-05T20:00:00+00:00");

    private static int Main()
    {
        var failures = new List<string>();
        var root = Path.Combine(
            Path.GetTempPath(),
            "falcon-stage4-wp03-" + Guid.NewGuid().ToString("N"));

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

            if (failures.Count > 0)
            {
                Console.Error.WriteLine("Stage 4 WP-03 verifier: FAIL");
                foreach (var failure in failures) Console.Error.WriteLine("- " + failure);
                return 1;
            }

            Console.WriteLine("Stage 4 WP-03 verifier: PASS");
            Console.WriteLine("FDN-001 ownership, singular write authority, durable versioned current state, immutable history, explicit failure classification, and deterministic reload verified.");
            Console.WriteLine("Application business state remains out of scope.");
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

}
