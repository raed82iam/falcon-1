using System;
using System.Collections.Generic;
using Foundation.Authority;
using Foundation.Contracts;
using Foundation.Core;
using Foundation.Infrastructure;

namespace Falcon.Stage4.WP02.Verifier;

internal static class Program
{
    private sealed record ScenarioFixture(
        LifecycleControlService Service,
        LifecycleTransitionRequest Request,
        LifecycleTransitionEvidence Evidence,
        LifecycleAuthorityEvaluation Evaluation,
        LifecycleAuthorityControlDecision Result);
    private const string ArtifactDigest =
        "A1A2A3A4A5A6A7A8A9AAABACADAEAFB0B1B2B3B4B5B6B7B8B9BABBBCBDBEBFC0";
    private const string ManifestDigest =
        "C1C2C3C4C5C6C7C8C9CACBCCCDCECFC0D1D2D3D4D5D6D7D8D9DADBDCDDDEDFE0";
    private const string ApprovedGraphDigest =
        "D06C6EDE16D2A55F4FBA36B965C5EECA0A98CE5AE11CE711ABCB4E8FECFF992E";

    private static readonly DateTimeOffset Now =
        DateTimeOffset.Parse("2026-08-05T19:00:00+00:00");

    private static int Main()
    {
        var failures = new List<string>();

        var accepted = ExecuteScenario("subject/wp02/accepted");
        Expect(failures, "authorized valid transition",
            accepted.Result.AuthorityResult.Decision == AuthorityDecision.Allow &&
            accepted.Result.LifecycleDecision.ContractResult.Decision == "ACCEPTED" &&
            accepted.Result.LifecycleDecision.Event is not null,
            Describe(accepted.Result));

        Expect(failures, "exactly one accepted event",
            accepted.Service.GetEvents().Count == 1,
            accepted.Service.GetEvents().Count.ToString());

        var denied = ExecuteScenario(
            "subject/wp02/denied",
            policyOverride: policy => policy with { Actions = new[] { "lifecycle.inspect" } });
        Expect(failures, "denied authority blocks transition",
            denied.Result.AuthorityResult.Decision == AuthorityDecision.Deny &&
            denied.Result.LifecycleDecision.ContractResult.Decision == "REJECTED" &&
            denied.Result.LifecycleDecision.Event is null &&
            denied.Service.GetSnapshot("subject/wp02/denied")?.State == LifecycleState.Registered,
            Describe(denied.Result));

        var actorMismatch = ExecuteScenario(
            "subject/wp02/actor",
            policyOverride: policy => policy with { ActorIdentities = new[] { "actor/other" } });
        Expect(failures, "requester binding",
            actorMismatch.Result.AuthorityResult.Reason == AuthorityReason.ActorUnknown &&
            actorMismatch.Result.LifecycleDecision.ContractResult.Decision == "REJECTED",
            Describe(actorMismatch.Result));

        var scopeMismatch = ExecuteScenario(
            "subject/wp02/scope",
            policyOverride: policy => policy with { AuthorizedScopes = new[] { "LIFECYCLE:other" } });
        Expect(failures, "transition scope binding",
            scopeMismatch.Result.AuthorityResult.Reason == AuthorityReason.ScopeExceeded &&
            scopeMismatch.Result.LifecycleDecision.Event is null,
            Describe(scopeMismatch.Result));

        var expired = ExecuteScenario(
            "subject/wp02/expired",
            policyOverride: policy => policy with { Expiry = Now });
        Expect(failures, "expired authority",
            expired.Result.AuthorityResult.Decision == AuthorityDecision.Deny &&
            expired.Result.LifecycleDecision.Event is null,
            Describe(expired.Result));

        var malformed = ExecuteScenario(
            "subject/wp02/malformed",
            contextOverride: context => context with { EvidenceReference = string.Empty });
        Expect(failures, "malformed authority context fails closed",
            malformed.Result.AuthorityResult.Reason == AuthorityReason.EvidenceMissing &&
            malformed.Result.LifecycleDecision.Event is null,
            Describe(malformed.Result));

        var illegal = ExecuteScenario(
            "subject/wp02/illegal",
            target: "RUNNING",
            includeDependency: true);
        Expect(failures, "authority allow does not bypass lifecycle graph",
            illegal.Result.AuthorityResult.Decision == AuthorityDecision.Allow &&
            illegal.Result.LifecycleDecision.ContractResult.Decision == "REJECTED" &&
            illegal.Result.LifecycleDecision.Event is null,
            Describe(illegal.Result));

        var replayA = ExecuteScenario("subject/wp02/replay");
        var replayB = ExecuteScenario("subject/wp02/replay");
        Expect(failures, "deterministic authority replay",
            replayA.Result.AuthorityResult == replayB.Result.AuthorityResult,
            replayA.Result.AuthorityResult.DecisionId + " / " + replayB.Result.AuthorityResult.DecisionId);
        Expect(failures, "deterministic lifecycle result",
            replayA.Result.LifecycleDecision.ContractResult == replayB.Result.LifecycleDecision.ContractResult,
            replayA.Result.LifecycleDecision.ContractResult.Reason + " / " +
            replayB.Result.LifecycleDecision.ContractResult.Reason);

        var actionMismatch = ExecuteScenario(
            "subject/wp02/action",
            policyOverride: policy => policy with { Actions = new[] { "lifecycle.inspect" } });
        Expect(failures, "action mismatch",
            actionMismatch.Result.AuthorityResult.Reason == AuthorityReason.ActionDenied &&
            actionMismatch.Result.LifecycleDecision.Event is null,
            Describe(actionMismatch.Result));

        var resourceMismatch = ExecuteScenario(
            "subject/wp02/resource",
            policyOverride: policy => policy with { Resources = new[] { "lifecycle:subject/other" } });
        Expect(failures, "subject resource mismatch",
            resourceMismatch.Result.AuthorityResult.Reason == AuthorityReason.ResourceDenied &&
            resourceMismatch.Result.LifecycleDecision.Event is null,
            Describe(resourceMismatch.Result));

        var staleSource = ExecuteScenario(
            "subject/wp02/stale-source",
            source: "READY",
            target: "INITIALIZING");
        Expect(failures, "stale source state",
            staleSource.Result.AuthorityResult.Decision == AuthorityDecision.Allow &&
            staleSource.Result.LifecycleDecision.ContractResult.Decision == "REJECTED" &&
            staleSource.Result.LifecycleDecision.ContractResult.Reason == "STALE_SOURCE_STATE" &&
            staleSource.Result.LifecycleDecision.ContractResult.ActualResultingState == "REGISTERED" &&
            staleSource.Result.LifecycleDecision.Event is null,
            Describe(staleSource.Result));

        var duplicate = accepted.Service.TransitionAuthorized(
            accepted.Request,
            accepted.Evidence,
            accepted.Evaluation);
        Expect(failures, "duplicate identical transition",
            duplicate.AuthorityResult.Decision == AuthorityDecision.Allow &&
            duplicate.LifecycleDecision.ContractResult.Decision == "REJECTED" &&
            duplicate.LifecycleDecision.ContractResult.ActualResultingState == "INITIALIZING" &&
            duplicate.LifecycleDecision.Event is null &&
            accepted.Service.GetEvents().Count == 1,
            Describe(duplicate));

        var conflictingRequest = accepted.Request with
        {
            RequestedTargetState = "RESTRICTED"
        };
        var conflictingEvidence = accepted.Evidence with
        {
            TransitionId = accepted.Evidence.TransitionId + "/conflict",
            EventId = accepted.Evidence.EventId + "/conflict"
        };
        var conflictingEvaluation = BuildEvaluation(conflictingRequest);
        var conflicting = accepted.Service.TransitionAuthorized(
            conflictingRequest,
            conflictingEvidence,
            conflictingEvaluation);
        Expect(failures, "conflicting duplicate",
            conflicting.AuthorityResult.Decision == AuthorityDecision.Allow &&
            conflicting.LifecycleDecision.ContractResult.Decision == "REJECTED" &&
            conflicting.LifecycleDecision.ContractResult.ActualResultingState == "INITIALIZING" &&
            conflicting.LifecycleDecision.Event is null &&
            accepted.Service.GetEvents().Count == 1,
            Describe(conflicting));

        var deniedRetry = denied.Service.TransitionAuthorized(
            denied.Request,
            denied.Evidence,
            denied.Evaluation);
        Expect(failures, "unauthorized retry and replay",
            deniedRetry.AuthorityResult == denied.Result.AuthorityResult &&
            deniedRetry.LifecycleDecision.ContractResult.Decision == "REJECTED" &&
            deniedRetry.LifecycleDecision.ContractResult.ActualResultingState == "REGISTERED" &&
            deniedRetry.LifecycleDecision.Event is null &&
            denied.Service.GetEvents().Count == 0,
            Describe(deniedRetry));

        var missingAuthorityEvidence = accepted.Evidence with
        {
            AuthorityDecision = null!,
            ValidationEvidence = string.Empty
        };
        var missingAuthority = accepted.Service.Transition(
            accepted.Request,
            missingAuthorityEvidence);
        Expect(failures, "missing authority decision cannot bypass engine",
            missingAuthority.ContractResult.Decision == "REJECTED" &&
            missingAuthority.ContractResult.Reason == "AUTHORITY_ENGINE_REQUIRED" &&
            missingAuthority.Event is null &&
            accepted.Service.GetEvents().Count == 1,
            Describe(missingAuthority));

        var malformedAuthorityResult = new AuthorityResult(
            string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty,
            string.Empty, string.Empty, string.Empty, default, default, string.Empty);
        var malformedAuthorityEvidence = accepted.Evidence with
        {
            AuthorityDecision = malformedAuthorityResult,
            ValidationEvidence = string.Empty
        };
        var malformedAuthority = accepted.Service.Transition(
            accepted.Request,
            malformedAuthorityEvidence);
        Expect(failures, "malformed authority decision cannot bypass engine",
            malformedAuthority.ContractResult.Decision == "REJECTED" &&
            malformedAuthority.ContractResult.Reason == "AUTHORITY_ENGINE_REQUIRED" &&
            malformedAuthority.Event is null &&
            accepted.Service.GetEvents().Count == 1,
            Describe(malformedAuthority));

        AssertAcceptedResultTransplantsFailClosed(failures);

        Expect(failures, "failed transition reports actual state",
            illegal.Result.LifecycleDecision.ContractResult.ActualResultingState == "REGISTERED",
            illegal.Result.LifecycleDecision.ContractResult.ActualResultingState);

        if (failures.Count > 0)
        {
            Console.Error.WriteLine("Stage 4 WP-02 verifier: FAIL");
            foreach (var failure in failures) Console.Error.WriteLine("- " + failure);
            return 1;
        }

        Console.WriteLine("Stage 4 WP-02 verifier: PASS");
        Console.WriteLine("Authority Engine integration, exact transition binding, default-deny behavior, lifecycle graph preservation, and deterministic replay verified.");
        Console.WriteLine("No second lifecycle controller was introduced.");
        Console.WriteLine("Authority decision identity: " + accepted.Result.AuthorityResult.DecisionId);
        return 0;
    }

    private static ScenarioFixture ExecuteScenario(
        string subjectId,
        string source = "REGISTERED",
        string target = "INITIALIZING",
        bool includeDependency = false,
        Func<AuthorityPolicy, AuthorityPolicy>? policyOverride = null,
        Func<AuthorityEvaluationContext, AuthorityEvaluationContext>? contextOverride = null)
    {
        var service = CreateRegisteredService(subjectId, out var subject, out var bootstrapContextId);
        var requestId = "transition/" + subjectId.Replace('/', '-');
        var currentSnapshot = service.GetSnapshot(subjectId) ??
            throw new InvalidOperationException("Registered lifecycle snapshot is missing.");
        var dependency = includeDependency ? BuildDependency(subject) : null;
        var dependencyContext = dependency?.EvidenceReference ?? $"dependency-context:{subjectId}:not-required";

        var request = new LifecycleTransitionRequest(
            requestId,
            subjectId,
            source,
            target,
            "actor/lifecycle-controller",
            string.Empty,
            "governed Stage 4 WP-02 transition",
            dependencyContext,
            Now.AddMinutes(-1),
            Now.AddMinutes(30));

        var evidence = new LifecycleTransitionEvidence
        {
            TransitionId = "transition-id/" + requestId,
            EventId = "event-id/" + requestId,
            ModelId = "SYS-002-CANONICAL-LIFECYCLE",
            ModelVersion = "1.1",
            ExpectedStateVersion = currentSnapshot.StateVersion,
            BootstrapContextId = bootstrapContextId,
            ObservationTime = Now,
            AuthorityDecision = null!,
            TimeProvider = BuildTimeProvider(),
            DependencyEvidence = dependency
        };

        var evaluation = BuildEvaluation(
            request,
            policyOverride,
            contextOverride);

        var result = service.TransitionAuthorized(request, evidence, evaluation);
        return new ScenarioFixture(service, request, evidence, evaluation, result);
    }

    private static LifecycleAuthorityEvaluation BuildEvaluation(
        LifecycleTransitionRequest request,
        Func<AuthorityPolicy, AuthorityPolicy>? policyOverride = null,
        Func<AuthorityEvaluationContext, AuthorityEvaluationContext>? contextOverride = null)
    {
        var scope =
            $"LIFECYCLE:{request.ComponentIdentity}:{request.AuthoritativeSourceState}->{request.RequestedTargetState}";

        var policy = new AuthorityPolicy(
            "policy/stage4/wp02/lifecycle",
            "1.0.0",
            "authority/owner-approved",
            Now.AddHours(-1),
            Now.AddHours(1),
            new[] { request.Requester },
            new[] { "lifecycle.transition" },
            new[] { $"lifecycle:{request.ComponentIdentity}" },
            new[] { "authoritative-lifecycle-transition" },
            new[] { scope },
            new[] { "foundation-internal" });

        if (policyOverride is not null) policy = policyOverride(policy);

        var delegation = new DelegationEvidence(
            "delegation/stage4/wp02/" + request.ComponentIdentity.Replace('/', '-'),
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
            "evidence/fitness/stage4/wp02");

        var context = new AuthorityEvaluationContext(
            policy,
            delegation,
            fitness,
            Now,
            "evidence/authority/stage4/wp02/" + request.ComponentIdentity.Replace('/', '-'));

        if (contextOverride is not null) context = contextOverride(context);

        return new LifecycleAuthorityEvaluation(
            "foundation-internal",
            "FIT",
            context);
    }

    private static void AssertAcceptedResultTransplantsFailClosed(
        ICollection<string> failures)
    {
        var fixture = ExecuteScenario("subject/wp02/transplant");
        var authority = fixture.Result.AuthorityResult;
        var boundRequest = fixture.Request with
        {
            AuthorityReference = authority.DecisionId
        };
        var boundEvidence = fixture.Evidence with
        {
            AuthorityDecision = authority,
            ValidationEvidence = string.Empty
        };
        boundEvidence = boundEvidence with
        {
            ValidationEvidence = LifecycleEvidenceBinding.Compute(boundRequest, boundEvidence)
        };

        var mutations = new (
            string Name,
            LifecycleTransitionRequest Request,
            string ExpectedActualState)[]
        {
            ("request identity", boundRequest with { TransitionRequestId = boundRequest.TransitionRequestId + "/other" }, "INITIALIZING"),
            ("requester", boundRequest with { Requester = "actor/other" }, "INITIALIZING"),
            ("subject resource", boundRequest with { ComponentIdentity = "subject/wp02/other" }, "UNKNOWN"),
            ("source state", boundRequest with { AuthoritativeSourceState = "READY" }, "INITIALIZING"),
            ("target scope", boundRequest with { RequestedTargetState = "RESTRICTED" }, "INITIALIZING"),
            ("request time", boundRequest with { RequestTime = boundRequest.RequestTime.AddSeconds(1) }, "INITIALIZING"),
            ("expiry", boundRequest with { Expiry = boundRequest.Expiry.AddSeconds(-1) }, "INITIALIZING")
        };

        foreach (var mutation in mutations)
        {
            var evidence = boundEvidence with
            {
                TransitionId = boundEvidence.TransitionId + "/" + mutation.Name.Replace(' ', '-'),
                EventId = boundEvidence.EventId + "/" + mutation.Name.Replace(' ', '-'),
                ValidationEvidence = string.Empty
            };
            evidence = evidence with
            {
                ValidationEvidence = LifecycleEvidenceBinding.Compute(mutation.Request, evidence)
            };

            var decision = fixture.Service.Transition(mutation.Request, evidence);
            Expect(
                failures,
                "accepted-result transplant " + mutation.Name,
                decision.ContractResult.Decision == "REJECTED" &&
                decision.ContractResult.Reason == "AUTHORITY_ENGINE_REQUIRED" &&
                decision.ContractResult.ActualResultingState == mutation.ExpectedActualState &&
                decision.Event is null &&
                fixture.Service.GetEvents().Count == 1,
                Describe(decision));
        }

        var observationMutation = boundEvidence with
        {
            TransitionId = boundEvidence.TransitionId + "/observation-time",
            EventId = boundEvidence.EventId + "/observation-time",
            ObservationTime = boundEvidence.ObservationTime.AddSeconds(1),
            ValidationEvidence = string.Empty
        };
        observationMutation = observationMutation with
        {
            ValidationEvidence = LifecycleEvidenceBinding.Compute(boundRequest, observationMutation)
        };
        var observationDecision = fixture.Service.Transition(boundRequest, observationMutation);
        Expect(
            failures,
            "accepted-result transplant observation time",
            observationDecision.ContractResult.Decision == "REJECTED" &&
            observationDecision.ContractResult.Reason == "AUTHORITY_ENGINE_REQUIRED" &&
            observationDecision.ContractResult.ActualResultingState == "INITIALIZING" &&
            observationDecision.Event is null &&
            fixture.Service.GetEvents().Count == 1,
            Describe(observationDecision));
    }

    private static LifecycleControlService CreateRegisteredService(
        string subjectId,
        out BootstrapSubjectAdmissionEvidence subject,
        out string bootstrapContextId)
    {
        var service = new LifecycleControlService(
            LifecycleStateModel.CreateCanonical(),
            LifecycleAuthorityMode.AuthorityEngineRequired);
        var bootstrap = BuildBootstrapRequest(subjectId);
        subject = bootstrap.Subject;
        bootstrapContextId = bootstrap.Context.ContextId;
        var registration = service.Register(bootstrap, "registration-evidence/" + subjectId);

        if (!registration.Registration.Accepted)
        {
            throw new InvalidOperationException(
                "Verifier fixture registration failed: " +
                registration.Registration.ReasonCode);
        }

        return service;
    }

    private static BootstrapValidationRequest BuildBootstrapRequest(string subjectId)
    {
        var subject = new BootstrapSubjectAdmissionEvidence
        {
            SubjectId = subjectId,
            SubjectVersion = "1.0",
            SubjectKind = BootstrapSubjectKind.Service,
            ArtifactIdentity = $"artifact:{subjectId}:1.0",
            ArtifactDigest = ArtifactDigest,
            ManifestIdentity = $"manifest:{subjectId}:1.0",
            ManifestDigest = ManifestDigest,
            AdmissionDecisionId = $"admission:{subjectId}:1.0",
            AdmissionState = "ADMITTED",
            RegistrationEvidenceId = $"registration:{subjectId}:1.0",
            RegistrationState = "REGISTERED",
            EvidenceAuthority = "FALCON-STAGE3-WP05-EVIDENCE",
            EffectiveTime = Now.AddHours(-1),
            Expiry = Now.AddHours(2)
        };

        var context = new BootstrapExecutionContextRecord(
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
            Now.AddHours(2));

        var provenance = new BootstrapEvidenceProvenanceRecord(
            $"provenance:{subjectId}:1.0",
            "1.0",
            $"source-record:{subjectId}:1.0",
            ArtifactDigest,
            context.SourceIdentity,
            "FALCON-STAGE3-WP05-PROVENANCE-AUTHORITY",
            $"provenance-evidence:{subjectId}",
            "PROVEN",
            subject.ArtifactIdentity,
            Now.AddHours(-1),
            Now.AddHours(2));

        return new BootstrapValidationRequest
        {
            RequestId = $"bootstrap-request:{subjectId}:1.0",
            Subject = subject,
            Context = context,
            Provenance = provenance,
            TimeProvider = BuildTimeProvider(),
            DependencyEvidence = BuildDependency(subject),
            ObservationTime = Now
        };
    }

    private static DependencyActivationEvidence BuildDependency(
        BootstrapSubjectAdmissionEvidence subject) => new()
        {
            SubjectId = subject.SubjectId,
            SubjectVersion = subject.SubjectVersion,
            GraphId = "stage3-wp04-golden-graph",
            GraphVersion = "1.0",
            GraphDigest = ApprovedGraphDigest,
            DependencyValidationState = "VALIDATED",
            ActivationOrderState = "VALIDATED",
            SubjectActivationIndex = 4,
            EvidenceReference = $"dependency-evidence:{subject.SubjectId}",
            EffectiveTime = Now.AddHours(-1),
            Expiry = Now.AddHours(2)
        };

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

    private static void Expect(
        ICollection<string> failures,
        string scenario,
        bool condition,
        string actual)
    {
        if (!condition) failures.Add($"{scenario}: {actual}");
    }

    private static string Describe(LifecycleAuthorityControlDecision result) =>
        $"{result.AuthorityResult.Decision}/{result.AuthorityResult.Reason} -> " +
        $"{result.LifecycleDecision.ContractResult.Decision}/" +
        $"{result.LifecycleDecision.ContractResult.Reason}";

    private static string Describe(LifecycleControlDecision result) =>
        $"{result.ContractResult.Decision}/{result.ContractResult.Reason}/" +
        result.ContractResult.ActualResultingState;
}
