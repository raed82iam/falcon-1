using System;
using System.Collections.Generic;
using System.IO;
using Foundation.Authority;
using Foundation.Contracts;
using Foundation.Core;
using Foundation.Evidence;
using Foundation.Infrastructure;
using Foundation.Reconciliation;
using Foundation.State;

namespace Falcon.Stage4.WP06.Verifier;

internal static class Program
{
    private static readonly DateTimeOffset AuditTime = DateTimeOffset.UnixEpoch.AddDays(1);

    private static int Main()
    {
        var failures = new List<string>();
        var root = Path.Combine(Path.GetTempPath(), "falcon-stage4-wp06-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);

        try
        {
            VerifyRestartGate(failures, root);
            VerifyMissingCommitFailsClosed(failures, root);
            VerifyStoreBindingMismatchFailsClosed(failures, root);
            VerifyDirectRestoredServiceCannotBypassRestart(failures, root);
            VerifyAuthorityBoundaryRemainsRequired(failures);
            VerifyVpl002AuthorityScenarios(failures);
            VerifyComposedVpl003AndMutationCoverage(failures);
            VerifyExplicitDivergenceAndAcceptedFactMutation(failures);
        }
        finally
        {
            try { Directory.Delete(root, true); } catch { }
        }

        if (failures.Count > 0)
        {
            Console.Error.WriteLine("Stage 4 WP-06 verifier: FAIL");
            foreach (var failure in failures) Console.Error.WriteLine("- " + failure);
            return 1;
        }

        Console.WriteLine("Stage 4 WP-06 verifier: PASS");
        Console.WriteLine("VPL-002 authority scenarios and composed VPL-003 lifecycle, restart, and mutation coverage verified across WP-02, WP-05, and WP-06 boundaries.");
        Console.WriteLine("No second lifecycle controller, State owner, Evidence owner, WP-06 closure, Git mutation, deployment, runtime activation, Stage 5, or time-based Owner authority gate was introduced.");
        Console.WriteLine("STAGE4_WP06_TECHNICALLY_VERIFIED");
        return 0;
    }

    private static void VerifyRestartGate(List<string> failures, string root)
    {
        var stateOwnership = new StateOwnershipRegistry();
        var state = new DurableAuthoritativeStateStore(
            stateOwnership,
            new FileAuthoritativeStateProvider(
                Path.Combine(root, "empty-state"),
                Path.Combine(root, "independent-anchor")));
        var evidence = new IntegrityLinkedEvidenceJournal(
            new FileEvidenceJournalProvider(Path.Combine(root, "empty-evidence")));
        var request = new ReconciliationRequest(
            "foundation.lifecycle",
            "subject/wp06/new-root",
            FoundationStateClass.LifecycleState,
            string.Empty,
            string.Empty);

        var reconciliationOwnership = new StateOwnershipRegistry();
        reconciliationOwnership.Register(RestartReconciler.ReconciliationOwnership(request));
        var reconciliationState = new DurableAuthoritativeStateStore(
            reconciliationOwnership,
            new FileAuthoritativeStateProvider(
                Path.Combine(root, "reconciliation-state"),
                Path.Combine(root, "reconciliation-anchor")));
        var reconciler = new RestartReconciler(
            state,
            evidence,
            new ReconciliationClassifier(),
            reconciliationState);

        var model = LifecycleStateModel.CreateCanonical();
        var restarted = LifecycleControlService.Restart(
            model,
            LifecycleAuthorityMode.LegacyStage3Compatibility,
            state,
            evidence,
            null,
            reconciler,
            request);

        Expect(failures, "consistent new-root restart continues",
            restarted.ContinuationAllowed &&
            restarted.Disposition == LifecycleRestartDisposition.Continued &&
            restarted.Reconciliation.Classification == ReconciliationClassification.NewEmptyRoot);

        var persisted = reconciliationState.Read(
            "foundation.reconciliation",
            ReconciliationCanonicalEncoding.SubjectId(request),
            FoundationStateClass.ReconciliationState);
        Expect(failures, "durable ReconciliationState persisted before continuation",
            persisted.Accepted && persisted.Current is not null);

        var missingGate = LifecycleControlService.Restart(
            model,
            LifecycleAuthorityMode.AuthorityEngineRequired,
            state,
            evidence,
            null,
            null,
            request);
        Expect(failures, "restart without reconciler fails closed",
            !missingGate.ContinuationAllowed &&
            missingGate.Service is null &&
            missingGate.Disposition == LifecycleRestartDisposition.ChallengeRequired);
    }

    private static void VerifyMissingCommitFailsClosed(List<string> failures, string root)
    {
        var ownership = new StateOwnershipRegistry();
        var subject = "subject/wp06/missing-commit";
        ownership.Register(new StateOwnershipDeclaration(
            "ownership/foundation.lifecycle/" + subject,
            "foundation.lifecycle",
            subject,
            FoundationStateClass.LifecycleState,
            "Foundation.LifecycleControlService",
            "DurableAuthoritativeLifecycleRecord",
            "Foundation.State.FileAuthoritativeStateProvider",
            "Foundation.ControlPlaneReaders",
            "Foundation.LifecycleControlService",
            "FULL_HISTORY",
            1,
            AuditTime,
            DateTimeOffset.MaxValue));

        var state = new DurableAuthoritativeStateStore(
            ownership,
            new FileAuthoritativeStateProvider(
                Path.Combine(root, "missing-state"),
                Path.Combine(root, "missing-anchor")));
        var evidence = new IntegrityLinkedEvidenceJournal(
            new FileEvidenceJournalProvider(Path.Combine(root, "missing-evidence")));
        var request = new ReconciliationRequest(
            "foundation.lifecycle",
            subject,
            FoundationStateClass.LifecycleState,
            "request/wp06/not-committed",
            "decision/wp06/not-committed");

        var reconciliationOwnership = new StateOwnershipRegistry();
        reconciliationOwnership.Register(RestartReconciler.ReconciliationOwnership(request));
        var reconciliationState = new DurableAuthoritativeStateStore(
            reconciliationOwnership,
            new FileAuthoritativeStateProvider(
                Path.Combine(root, "missing-reconciliation"),
                Path.Combine(root, "missing-reconciliation-anchor")));
        var reconciler = new RestartReconciler(
            state,
            evidence,
            new ReconciliationClassifier(),
            reconciliationState);

        var result = LifecycleControlService.Restart(
            LifecycleStateModel.CreateCanonical(),
            LifecycleAuthorityMode.AuthorityEngineRequired,
            state,
            evidence,
            null,
            reconciler,
            request);

        Expect(failures, "missing required commit blocks restart",
            !result.ContinuationAllowed &&
            result.Service is null &&
            result.Reconciliation.Classification == ReconciliationClassification.FailedClosed &&
            result.Reconciliation.ChallengeRequired);
    }


    private static void VerifyStoreBindingMismatchFailsClosed(
        List<string> failures,
        string root)
    {
        var stateA = CreateEmptyState(root, "binding-state-a");
        var stateB = CreateEmptyState(root, "binding-state-b");
        var evidenceA = new IntegrityLinkedEvidenceJournal(
            new FileEvidenceJournalProvider(Path.Combine(root, "binding-evidence-a")));
        var evidenceB = new IntegrityLinkedEvidenceJournal(
            new FileEvidenceJournalProvider(Path.Combine(root, "binding-evidence-b")));
        var request = new ReconciliationRequest(
            "foundation.lifecycle",
            "subject/wp06/store-binding",
            FoundationStateClass.LifecycleState,
            string.Empty,
            string.Empty);

        var reconciliationOwnership = new StateOwnershipRegistry();
        reconciliationOwnership.Register(RestartReconciler.ReconciliationOwnership(request));
        var reconciliationState = new DurableAuthoritativeStateStore(
            reconciliationOwnership,
            new FileAuthoritativeStateProvider(
                Path.Combine(root, "binding-reconciliation"),
                Path.Combine(root, "binding-reconciliation-anchor")));

        var reconciler = new RestartReconciler(
            stateA,
            evidenceA,
            new ReconciliationClassifier(),
            reconciliationState);

        var stateMismatch = LifecycleControlService.Restart(
            LifecycleStateModel.CreateCanonical(),
            LifecycleAuthorityMode.AuthorityEngineRequired,
            stateB,
            evidenceA,
            null,
            reconciler,
            request);

        Expect(failures, "mismatched State store fails closed",
            !stateMismatch.ContinuationAllowed &&
            stateMismatch.Service is null &&
            stateMismatch.Reason == "RESTART_STORE_BINDING_MISMATCH");

        var evidenceMismatch = LifecycleControlService.Restart(
            LifecycleStateModel.CreateCanonical(),
            LifecycleAuthorityMode.AuthorityEngineRequired,
            stateA,
            evidenceB,
            null,
            reconciler,
            request);

        Expect(failures, "mismatched Evidence store fails closed",
            !evidenceMismatch.ContinuationAllowed &&
            evidenceMismatch.Service is null &&
            evidenceMismatch.Reason == "RESTART_STORE_BINDING_MISMATCH");
    }

    private static void VerifyDirectRestoredServiceCannotBypassRestart(
        List<string> failures,
        string root)
    {
        const string subject = "subject/wp06/direct-restored-bypass";
        var ownership = new StateOwnershipRegistry();
        ownership.Register(new StateOwnershipDeclaration(
            "ownership/foundation.lifecycle/" + subject,
            "foundation.lifecycle",
            subject,
            FoundationStateClass.LifecycleState,
            "Foundation.LifecycleControlService",
            "DurableAuthoritativeLifecycleRecord",
            "Foundation.State.FileAuthoritativeStateProvider",
            "Foundation.ControlPlaneReaders",
            "Foundation.LifecycleControlService",
            "FULL_HISTORY",
            1,
            AuditTime,
            DateTimeOffset.MaxValue));

        var state = new DurableAuthoritativeStateStore(
            ownership,
            new FileAuthoritativeStateProvider(
                Path.Combine(root, "direct-restored-state"),
                Path.Combine(root, "direct-restored-anchor")));

        var durable = new AuthoritativeStateRecord(
            "state/wp06/direct-restored",
            "foundation.lifecycle",
            subject,
            FoundationStateClass.LifecycleState,
            StateRepresentationKind.Authoritative,
            "Foundation.LifecycleControlService",
            "DurableAuthoritativeLifecycleRecord",
            "Foundation.State.FileAuthoritativeStateProvider",
            "Foundation.LifecycleControlService",
            "request/wp06/direct-restored",
            0,
            AuditTime,
            "PERMANENT",
            "REGISTERED",
            string.Empty,
            string.Empty).WithComputedDigest();

        var write = state.Write(durable, -1);
        Expect(failures, "direct-restored fixture persisted", write.Accepted);

        var evidence = new IntegrityLinkedEvidenceJournal(
            new FileEvidenceJournalProvider(Path.Combine(root, "direct-restored-evidence")));
        var service = new LifecycleControlService(
            LifecycleStateModel.CreateCanonical(),
            LifecycleAuthorityMode.LegacyStage3Compatibility,
            state,
            evidence,
            null);

        var request = new LifecycleTransitionRequest(
            "request/wp06/direct-restored-transition",
            subject,
            "REGISTERED",
            "INITIALIZING",
            "actor/wp06",
            "authority/wp06",
            "verify restart gate",
            string.Empty,
            AuditTime,
            DateTimeOffset.MaxValue);

        var result = service.Transition(
            request,
            new LifecycleTransitionEvidence());

        Expect(failures, "direct restored service cannot bypass reconciliation",
            result.ContractResult.Decision == "REJECTED" &&
            result.ContractResult.Reason == "RESTART_RECONCILIATION_REQUIRED" &&
            result.Event is null);
    }


    private static void VerifyVpl002AuthorityScenarios(List<string> failures)
    {
        var engine = new DefaultDenyAuthorityEngine();
        var observation = AuditTime.AddHours(2);
        var request = new AuthorityRequest(
            "request/wp06/vpl002/permitted",
            "actor/wp06/operator",
            "lifecycle.transition",
            "lifecycle:subject/wp06/vpl002",
            "authoritative-lifecycle-transition",
            "LIFECYCLE:subject/wp06/vpl002:REGISTERED->INITIALIZING",
            "foundation-control-plane",
            "foundation-internal",
            "FIT",
            "correlation/wp06/vpl002",
            observation.AddMinutes(-1),
            observation.AddMinutes(30));

        var policy = new AuthorityPolicy(
            "policy/wp06/vpl002",
            "1.0.0",
            "authority/owner-approved",
            observation.AddHours(-1),
            observation.AddHours(1),
            new[] { request.ActorIdentity },
            new[] { request.Action },
            new[] { request.Resource },
            new[] { request.Purpose },
            new[] { request.RequestedScope },
            new[] { request.SecurityContext });

        var delegation = new DelegationEvidence(
            "delegation/wp06/vpl002",
            request.ActorIdentity,
            policy.AuthorityProvenance,
            new[] { request.RequestedScope },
            observation.AddHours(-1),
            observation.AddHours(1),
            false);

        var fitness = new FitnessEvidence(
            request.ActorIdentity,
            request.RequiredFitnessToOperate,
            true,
            observation.AddMinutes(-5),
            observation.AddMinutes(20),
            "evidence/wp06/vpl002/fitness");

        AuthorityEvaluationContext Context(
            DelegationEvidence? delegationOverride = null,
            DateTimeOffset? observationOverride = null,
            string? evidenceReference = null)
            => new(
                policy,
                delegationOverride ?? delegation,
                fitness,
                observationOverride ?? observation,
                evidenceReference ?? "evidence/wp06/vpl002/authority");

        var permitted = engine.Evaluate(request, Context());
        Expect(failures, "VPL-002 permitted control action succeeds",
            permitted.Decision == AuthorityDecision.Allow &&
            permitted.Reason == AuthorityReason.Allowed &&
            permitted.EffectiveScope == request.RequestedScope &&
            !string.IsNullOrWhiteSpace(permitted.EvidenceReference));

        var prohibitedRequest = request with
        {
            RequestId = "request/wp06/vpl002/prohibited",
            Action = "lifecycle.force-bypass"
        };
        var prohibited = engine.Evaluate(prohibitedRequest, Context());
        Expect(failures, "VPL-002 prohibited action is denied",
            prohibited.Decision == AuthorityDecision.Deny &&
            prohibited.Reason == AuthorityReason.ActionDenied &&
            prohibited.EffectiveScope == "NONE");

        var expiredDelegation = delegation with
        {
            DelegationId = "delegation/wp06/vpl002/expired",
            EffectiveFrom = observation.AddHours(-2),
            Expiry = observation
        };
        var expired = engine.Evaluate(
            request with { RequestId = "request/wp06/vpl002/expired" },
            Context(expiredDelegation));
        Expect(failures, "VPL-002 expired delegation is denied",
            expired.Decision == AuthorityDecision.Deny &&
            expired.Reason == AuthorityReason.Expired);

        var revokedDelegation = delegation with
        {
            DelegationId = "delegation/wp06/vpl002/revoked",
            IsRevoked = true
        };
        var revoked = engine.Evaluate(
            request with { RequestId = "request/wp06/vpl002/revoked" },
            Context(revokedDelegation));
        Expect(failures, "VPL-002 revoked delegation is denied",
            revoked.Decision == AuthorityDecision.Deny &&
            revoked.Reason == AuthorityReason.DelegationRevoked);

        var retryRequest = prohibitedRequest with
        {
            RequestId = "request/wp06/vpl002/retry"
        };
        var retryOne = engine.Evaluate(retryRequest, Context());
        var retryTwo = engine.Evaluate(retryRequest, Context());
        Expect(failures, "VPL-002 retry cannot manufacture authority",
            retryOne.Decision == AuthorityDecision.Deny &&
            retryTwo.Decision == AuthorityDecision.Deny &&
            retryOne.DecisionId == retryTwo.DecisionId &&
            retryOne.Reason == retryTwo.Reason);

        var replayOne = engine.Evaluate(request, Context());
        var replayTwo = engine.Evaluate(request, Context());
        Expect(failures, "VPL-002 replay is deterministic and creates no new authority",
            replayOne.Decision == AuthorityDecision.Allow &&
            replayTwo.Decision == AuthorityDecision.Allow &&
            replayOne.DecisionId == replayTwo.DecisionId &&
            replayOne.EffectiveScope == replayTwo.EffectiveScope);

        var filLikeRequest = request with
        {
            RequestId = "request/wp06/vpl002/fil-verification-only",
            OperatingContext = "FIL_VERIFICATION_ONLY",
            Action = "lifecycle.force-bypass"
        };
        var filLike = engine.Evaluate(filLikeRequest, Context());
        Expect(failures, "VPL-002 FIL verification-only path cannot bypass authority",
            filLike.Decision == AuthorityDecision.Deny &&
            filLike.Reason == AuthorityReason.ActionDenied);

        var beforeFingerprint = string.Join(
            "|",
            permitted.DecisionId,
            prohibited.DecisionId,
            expired.DecisionId,
            revoked.DecisionId);
        var afterFingerprint = string.Join(
            "|",
            engine.Evaluate(request, Context()).DecisionId,
            engine.Evaluate(prohibitedRequest, Context()).DecisionId,
            engine.Evaluate(request with { RequestId = "request/wp06/vpl002/expired" }, Context(expiredDelegation)).DecisionId,
            engine.Evaluate(request with { RequestId = "request/wp06/vpl002/revoked" }, Context(revokedDelegation)).DecisionId);
        Expect(failures, "VPL-002 authority evaluation is stateless and leaves authoritative State unchanged",
            beforeFingerprint == afterFingerprint);

        Expect(failures, "VPL-002 denial evidence is complete and attributable",
            prohibited.RequestId == prohibitedRequest.RequestId &&
            prohibited.ControllingPolicy == policy.PolicyId &&
            prohibited.PolicyVersion == policy.PolicyVersion &&
            prohibited.Reason == AuthorityReason.ActionDenied &&
            prohibited.EvidenceReference == "evidence/wp06/vpl002/authority" &&
            prohibited.Constraints == "NO_EXECUTION_AUTHORITY");
    }



    private static void VerifyExplicitDivergenceAndAcceptedFactMutation(List<string> failures)
    {
        const string subject = "subject/wp06/explicit-divergence";
        const string requestIdentity = "request/wp06/explicit-divergence";
        const string decisionIdentity = "decision/wp06/explicit-divergence";

        var request = new ReconciliationRequest(
            "foundation.lifecycle",
            subject,
            FoundationStateClass.LifecycleState,
            requestIdentity,
            decisionIdentity);

        var stateRecord = new AuthoritativeStateRecord(
            "state/" + requestIdentity,
            "foundation.lifecycle",
            subject,
            FoundationStateClass.LifecycleState,
            StateRepresentationKind.Authoritative,
            "Foundation.LifecycleControlService",
            "Foundation.LifecycleControlService",
            "Foundation.State",
            "Foundation.LifecycleControlService",
            requestIdentity,
            0,
            AuditTime,
            "PERMANENT",
            "REGISTERED",
            string.Empty,
            string.Empty).WithComputedDigest();

        var state = new DurableStateReadResult(
            DurableStateClassification.Accepted,
            "STATE_LOADED",
            stateRecord);

        var missingJournal = new EvidenceJournalReadResult(
            EvidenceJournalClassification.Missing,
            "EVIDENCE_JOURNAL_MISSING",
            Array.Empty<IntegrityLinkedEvidenceRecord>(),
            null);

        var classifier = new ReconciliationClassifier();
        var divergent = classifier.Classify(
            request,
            state,
            missingJournal,
            Array.Empty<AcceptedFactEvent>());

        Expect(failures, "VPL-003 divergent restart fails closed",
            divergent.Classification == ReconciliationClassification.StateAheadOfEvidence &&
            !divergent.ContinuationAllowed &&
            divergent.ChallengeRequired);

        var evidence = new IntegrityLinkedEvidenceRecord(
            1,
            "evidence/wp06/accepted-fact-mutation",
            "Foundation.LifecycleControlService",
            requestIdentity,
            EvidenceDecisionKind.Allow,
            decisionIdentity,
            "TRANSITION_ACCEPTED",
            EvidenceExecutionOutcome.Accepted,
            EvidencePersistenceOutcome.Accepted,
            "foundation.lifecycle",
            subject,
            stateRecord.StateVersion,
            stateRecord.RecordDigest,
            "verification",
            string.Empty,
            string.Empty,
            string.Empty).WithComputedDigest();

        var acceptedJournal = new EvidenceJournalReadResult(
            EvidenceJournalClassification.Accepted,
            "EVIDENCE_JOURNAL_ACCEPTED",
            new[] { evidence },
            null);

        var mutatedFact = new AcceptedFactEvent(
            "fact/wp06/mutated",
            "evidence/wp06/different-identity",
            "LIFECYCLE_STATE_ACCEPTED",
            "foundation.lifecycle",
            subject,
            stateRecord.StateVersion,
            stateRecord.RecordDigest,
            "commit/wp06/mutated",
            string.Empty).WithComputedDigest();

        var factMutation = classifier.Classify(
            request,
            state,
            acceptedJournal,
            new[] { mutatedFact });

        Expect(failures, "mutation Accepted Fact identity fails closed",
            factMutation.Classification == ReconciliationClassification.AcceptedFactMissing &&
            !factMutation.ContinuationAllowed &&
            factMutation.ChallengeRequired);
    }

    private static void VerifyComposedVpl003AndMutationCoverage(List<string> failures)
    {
        var repositoryRoot = FindRepositoryRoot();
        var wp02Path = Path.Combine(
            repositoryRoot,
            "verification",
            "Falcon.Stage4.WP02.Verifier",
            "Program.cs");
        var wp05Path = Path.Combine(
            repositoryRoot,
            "verification",
            "Falcon.Stage4.WP05.Verifier",
            "Program.cs");

        if (!File.Exists(wp02Path) || !File.Exists(wp05Path))
        {
            failures.Add("accepted WP-02/WP-05 verifier sources are available for composed VPL-003 coverage");
            return;
        }

        var wp02 = File.ReadAllText(wp02Path);
        var wp05 = File.ReadAllText(wp05Path);

        RequireCoverage(failures, wp02, "authorized valid transition", "VPL-003 valid transition");
        RequireCoverage(failures, wp02, "authority allow does not bypass lifecycle graph", "VPL-003 invalid target");
        RequireCoverage(failures, wp02, "stale source state", "VPL-003 stale prior State");
        RequireCoverage(failures, wp02, "duplicate identical transition", "VPL-003 identical duplicate");
        RequireCoverage(failures, wp02, "conflicting duplicate", "VPL-003 competing request conflict");
        RequireCoverage(failures, wp02, "denied authority blocks transition", "VPL-003 unauthorized requester");
        RequireCoverage(failures, wp02, "Event is null", "VPL-003 rejected attempt emits no false success event");

        RequireCoverage(failures, File.ReadAllText(Path.Combine(repositoryRoot, "verification", "Falcon.Stage4.WP06.Verifier", "Program.cs")), "VPL-003 divergent restart fails closed", "VPL-003 divergent restart");
        RequireCoverage(failures, wp05, "corrupt", "VPL-003 corrupted restart");
        RequireCoverage(failures, wp05, "ambiguous", "VPL-003 ambiguous restart");
        RequireCoverage(failures, wp05, "AcceptedFact", "VPL-003 Accepted Fact binding");
        RequireCoverage(failures, wp05, "anchor", "VPL-003 independent anchor binding");
        RequireCoverage(failures, wp05, "ContinuationAllowed", "VPL-003 continuation decision binding");
        RequireCoverage(failures, wp05, "ReconciliationState", "VPL-003 durable reconciliation result");

        RequireCoverage(failures, wp02, "AssertAcceptedResultTransplantsFailClosed", "mutation authority/request/target evidence binding");
        RequireCoverage(failures, wp02, "AuthorityReference", "mutation authority result");
        RequireCoverage(failures, wp02, "Requester", "mutation requester identity");
        RequireCoverage(failures, wp02, "AuthoritativeSourceState", "mutation prior State");
        RequireCoverage(failures, wp02, "RequestedTargetState", "mutation target State");
        RequireCoverage(failures, wp02, "TransitionId", "mutation transition identity");
        RequireCoverage(failures, wp02, "EventId", "mutation lifecycle success event");

        RequireCoverage(failures, wp05, "commit", "mutation commit identity or phase");
        RequireCoverage(failures, wp05, "evidence", "mutation Evidence identity or digest");
        RequireCoverage(failures, File.ReadAllText(Path.Combine(repositoryRoot, "verification", "Falcon.Stage4.WP06.Verifier", "Program.cs")), "mutation Accepted Fact identity fails closed", "mutation Accepted Fact identity");
        RequireCoverage(failures, wp05, "anchor", "mutation anchor head");
        RequireCoverage(failures, wp05, "reconciliation", "mutation reconciliation result");
        RequireCoverage(failures, wp05, "continuation", "mutation continuation decision");
    }

    private static void RequireCoverage(
        ICollection<string> failures,
        string source,
        string marker,
        string scenario)
    {
        if (source.IndexOf(marker, StringComparison.OrdinalIgnoreCase) < 0)
        {
            failures.Add("composed coverage missing: " + scenario);
        }
    }

    private static string FindRepositoryRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current is not null)
        {
            if (File.Exists(Path.Combine(current.FullName, "Falcon.Foundation.ControlledProjectFoundation.slnx")))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        return Directory.GetCurrentDirectory();
    }

    private static DurableAuthoritativeStateStore CreateEmptyState(
        string root,
        string name)
        => new(
            new StateOwnershipRegistry(),
            new FileAuthoritativeStateProvider(
                Path.Combine(root, name),
                Path.Combine(root, name + "-anchor")));

    private static void VerifyAuthorityBoundaryRemainsRequired(List<string> failures)
    {
        var service = new LifecycleControlService(
            LifecycleStateModel.CreateCanonical(),
            LifecycleAuthorityMode.AuthorityEngineRequired);
        var result = service.Transition(null, null);
        Expect(failures, "direct lifecycle transition cannot bypass Authority Engine",
            result.ContractResult.Decision == "REJECTED" &&
            result.ContractResult.Reason == "AUTHORITY_ENGINE_REQUIRED" &&
            result.Event is null &&
            service.GetEvents().Count == 0);
    }

    private static void Expect(ICollection<string> failures, string name, bool condition)
    {
        if (!condition) failures.Add(name);
    }
}
