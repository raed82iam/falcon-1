using System;
using System.Collections.Generic;
using System.Linq;
using Foundation.Contracts;
using Foundation.Core;
using Foundation.Infrastructure;

namespace Falcon.Stage3.WP05.Verifier;

internal static class Program
{
    private const string ArtifactDigest =
        "A1A2A3A4A5A6A7A8A9AAABACADAEAFB0B1B2B3B4B5B6B7B8B9BABBBCBDBEBFC0";
    private const string ManifestDigest =
        "C1C2C3C4C5C6C7C8C9CACBCCCDCECFC0D1D2D3D4D5D6D7D8D9DADBDCDDDEDFE0";
    private const string ApprovedGraphDigest =
        "D06C6EDE16D2A55F4FBA36B965C5EECA0A98CE5AE11CE711ABCB4E8FECFF992E";

    private static readonly DateTimeOffset ObservationTime =
        DateTimeOffset.Parse("2026-08-03T00:45:00+03:00");

    private static int Main()
    {
        var failures = new List<string>();

var nullBootstrap = BootstrapContextGate.Validate(null);
if (nullBootstrap.Accepted || !string.Equals(nullBootstrap.ReasonCode, "INVALID_BOOTSTRAP_REQUEST", StringComparison.Ordinal))
{
    failures.Add("null bootstrap request did not fail closed");
}

var nullLifecycleService = new LifecycleControlService(LifecycleStateModel.CreateCanonical());
var nullRegistration = nullLifecycleService.Register(null, string.Empty);
if (nullRegistration.Registration.Accepted)
{
    failures.Add("null lifecycle registration unexpectedly succeeded");
}
var nullTransition = nullLifecycleService.Transition(null, null);
if (!string.Equals(nullTransition.ContractResult.Decision, "REJECTED", StringComparison.Ordinal) || nullTransition.Event is not null)
{
    failures.Add("null lifecycle transition did not reject without success event");
}
if (!string.Equals(LifecycleEvidenceBinding.Compute(null, null), string.Empty, StringComparison.Ordinal))
{
    failures.Add("null lifecycle evidence binding did not fail closed");
}


        ValidateCanonicalBootstrapPolicy(failures);
        ValidateLifecycleModel(failures);
        ValidateEvidenceBoundLifecycle(failures);
        ValidateGlobalIdentityReservation(failures);
        ValidateBootstrapExpiry(failures);
        ValidateRestrictedStoppedRecovery(failures);
        ValidateRestartBoundAndTerminalState(failures);

        if (failures.Count > 0)
        {
            Console.Error.WriteLine("Stage 3 WP-05: FAIL");
            foreach (var failure in failures)
            {
                Console.Error.WriteLine($"- {failure}");
            }

            return 1;
        }

        var policy = BootstrapPolicyCatalog.GetStage3Wp05Policy();
        var model = LifecycleStateModel.CreateCanonical();

        Console.WriteLine("Stage 3 WP-05: PASS");
        Console.WriteLine($"Canonical bootstrap policy: {policy.PolicyId} v{policy.Version}");
        Console.WriteLine($"Approved dependency graph digest: {policy.DependencyGraphDigest}");
        Console.WriteLine($"Lifecycle model: {model.ModelId} v{model.Version}");
        Console.WriteLine("GLOBAL_SINGLE_USE_IDENTITIES_VALIDATED");
        Console.WriteLine("BOUND_AUTHORITY_TIME_DEPENDENCY_RELEASE_RECOVERY_EVIDENCE_VALIDATED");
        Console.WriteLine("BOOTSTRAP_VALIDITY_BOUNDARY_VALIDATED");
        Console.WriteLine("RESTRICTED_STOPPED_CONTROLLED_RECOVERY_VALIDATED");
        Console.WriteLine("WP05_REMEDIATION_VALIDATED");
        return 0;
    }

    private static void ValidateCanonicalBootstrapPolicy(ICollection<string> failures)
    {
        var policy = BootstrapPolicyCatalog.GetStage3Wp05Policy();
        Expect(
            failures,
            "canonical policy identity",
            policy.PolicyId == "STAGE3-WP05-CANONICAL-BOOTSTRAP-POLICY" &&
            policy.Version == "1.0" &&
            policy.DependencyGraphDigest == ApprovedGraphDigest &&
            policy.LifecycleAuthorityPolicy == "GOV-097");

        var request = BuildBootstrapRequest("service.bootstrap-positive", ObservationTime);
        var accepted = BootstrapContextGate.Validate(request);
        var repeat = BootstrapContextGate.Validate(request);

        ExpectBootstrap(
            failures,
            "canonical service bootstrap",
            accepted,
            "ACCEPTED",
            "BOOTSTRAP_ACCEPTED");
        Expect(
            failures,
            "deterministic bootstrap decision identity",
            accepted.DecisionIdentity == repeat.DecisionIdentity &&
            accepted.ValidUntil == repeat.ValidUntil);
        Expect(
            failures,
            "bootstrap policy is external to request",
            accepted.PolicyId == policy.PolicyId &&
            accepted.PolicyVersion == policy.Version);

        Expect(
            failures,
            "canonical policy is calendar-independent",
            policy.EffectiveTime == DateTimeOffset.MinValue &&
            policy.Expiry == DateTimeOffset.MaxValue);

        var distantPastObservation =
            DateTimeOffset.Parse("2001-01-15T12:00:00+03:00");
        var distantFutureObservation =
            DateTimeOffset.Parse("2099-11-20T18:30:00+03:00");

        ExpectBootstrap(
            failures,
            "calendar-independent bootstrap distant past",
            BootstrapContextGate.Validate(
                BuildBootstrapRequest(
                    "service.calendar-independent-past",
                    distantPastObservation)),
            "ACCEPTED",
            "BOOTSTRAP_ACCEPTED");

        ExpectBootstrap(
            failures,
            "calendar-independent bootstrap distant future",
            BootstrapContextGate.Validate(
                BuildBootstrapRequest(
                    "service.calendar-independent-future",
                    distantFutureObservation)),
            "ACCEPTED",
            "BOOTSTRAP_ACCEPTED");

        var attackerSelected = BuildBootstrapRequest(
            "service.attacker-policy",
            ObservationTime,
            contextOverride: context => context with
            {
                BootstrapAuthority = "CALLER-SELECTED-AUTHORITY",
                EnvironmentIdentity = "CALLER-SELECTED-ENVIRONMENT",
                Scope = "CALLER-SELECTED-SCOPE",
                SourceIdentity = "CALLER-SELECTED-SOURCE",
                AuthorityBoundary = "CALLER-SELECTED-BOUNDARY"
            },
            dependencyOverride: dependency => dependency with
            {
                GraphId = "CALLER-SELECTED-GRAPH",
                GraphVersion = "99.0",
                GraphDigest = new string('D', 64),
                SubjectActivationIndex = 999
            });

        ExpectBootstrap(
            failures,
            "caller-selected bootstrap policy rejected",
            BootstrapContextGate.Validate(attackerSelected),
            "REJECTED",
            "BOOTSTRAP_AUTHORITY_MISMATCH");

        ExpectBootstrap(
            failures,
            "unapproved dependency digest rejected",
            BootstrapContextGate.Validate(
                request with
                {
                    DependencyEvidence = request.DependencyEvidence with
                    {
                        GraphDigest = new string('E', 64)
                    }
                }),
            "REJECTED",
            "DEPENDENCY_EVIDENCE_BINDING_MISMATCH");

        ExpectBootstrap(
            failures,
            "subject evidence authority rejected",
            BootstrapContextGate.Validate(
                request with
                {
                    Subject = request.Subject with
                    {
                        EvidenceAuthority = "CALLER-SELECTED-EVIDENCE-AUTHORITY"
                    }
                }),
            "REJECTED",
            "SUBJECT_EVIDENCE_AUTHORITY_MISMATCH");

        var restrictedRequest = BuildBootstrapRequest(
            "service.bootstrap-restricted",
            ObservationTime,
            restriction: BuildRestriction("service.bootstrap-restricted", ObservationTime));
        var restricted = BootstrapContextGate.Validate(restrictedRequest);

        ExpectBootstrap(
            failures,
            "restricted bootstrap accepted",
            restricted,
            "ACCEPTED",
            "BOOTSTRAP_ACCEPTED_RESTRICTED");
        Expect(
            failures,
            "restricted bootstrap identity retained",
            restricted.RestrictionActive &&
            restricted.ActiveRestrictionId == restrictedRequest.Restriction?.RestrictionId &&
            restricted.InitialLifecycleState == "RESTRICTED");

        var release = BuildRestrictionRelease(
            restrictedRequest.Restriction!,
            restrictedRequest.RequestId,
            "OWNER-APPROVAL-GOV-097-20260803",
            ObservationTime);
        var released = BootstrapContextGate.Validate(
            restrictedRequest with { RestrictionRelease = release });

        ExpectBootstrap(
            failures,
            "bootstrap controlled release accepted",
            released,
            "ACCEPTED",
            "BOOTSTRAP_ACCEPTED_AFTER_CONTROLLED_RELEASE");
        Expect(
            failures,
            "released bootstrap is unrestricted",
            !released.RestrictionActive &&
            released.RestrictionReleaseValidated &&
            string.IsNullOrEmpty(released.ActiveRestrictionId));
    }

    private static void ValidateLifecycleModel(ICollection<string> failures)
    {
        var mutableRules = new List<LifecycleTransitionRule>
        {
            new(LifecycleState.Registered, LifecycleState.Initializing)
        };
        var frozen = new LifecycleStateModel("freeze-test", "1.0", 1, mutableRules);
        mutableRules.Clear();

        Expect(
            failures,
            "lifecycle rule input frozen",
            frozen.Allows(LifecycleState.Registered, LifecycleState.Initializing) &&
            frozen.Rules.Count == 1);

        var duplicateRejected = false;
        try
        {
            _ = new LifecycleStateModel(
                "duplicate-test",
                "1.0",
                1,
                new[]
                {
                    new LifecycleTransitionRule(
                        LifecycleState.Registered,
                        LifecycleState.Initializing),
                    new LifecycleTransitionRule(
                        LifecycleState.Registered,
                        LifecycleState.Initializing)
                });
        }
        catch (ArgumentException)
        {
            duplicateRejected = true;
        }

        Expect(failures, "duplicate lifecycle rules rejected", duplicateRejected);

        var canonical = LifecycleStateModel.CreateCanonical();
        Expect(
            failures,
            "controlled stopped recovery exists",
            canonical.Version == "1.1" &&
            canonical.Allows(LifecycleState.Stopped, LifecycleState.Recovering));
        Expect(
            failures,
            "retired has no outbound transitions",
            !canonical.Rules.Any(rule => rule.Source == LifecycleState.Retired));
    }

    private static void ValidateEvidenceBoundLifecycle(ICollection<string> failures)
    {
        var subjectId = "service.lifecycle-positive";
        var bootstrapRequest = BuildBootstrapRequest(subjectId, ObservationTime);
        var service = RegisterService(failures, bootstrapRequest, out var bootstrap);
        if (service is null)
        {
            return;
        }

        var initializing = ExecuteTransition(
            service,
            bootstrapRequest.Subject,
            subjectId,
            "positive-initializing",
            "REGISTERED",
            "INITIALIZING",
            1,
            bootstrap.BootstrapContextId,
            ObservationTime);
        ExpectDecision(failures, "registered to initializing", initializing, "ACCEPTED", "TRANSITION_ACCEPTED");

        var ready = ExecuteTransition(
            service,
            bootstrapRequest.Subject,
            subjectId,
            "positive-ready",
            "INITIALIZING",
            "READY",
            2,
            bootstrap.BootstrapContextId,
            ObservationTime);
        ExpectDecision(failures, "initializing to ready", ready, "ACCEPTED", "TRANSITION_ACCEPTED");

        var noDependency = ExecuteTransition(
            service,
            bootstrapRequest.Subject,
            subjectId,
            "running-without-dependency",
            "READY",
            "RUNNING",
            3,
            bootstrap.BootstrapContextId,
            ObservationTime,
            includeDependency: false);
        ExpectDecision(
            failures,
            "running requires dependency record",
            noDependency,
            "REJECTED",
            "RUNNING_DEPENDENCY_EVIDENCE_REQUIRED");

        var running = ExecuteTransition(
            service,
            bootstrapRequest.Subject,
            subjectId,
            "positive-running",
            "READY",
            "RUNNING",
            3,
            bootstrap.BootstrapContextId,
            ObservationTime,
            includeDependency: true);
        ExpectDecision(failures, "ready to running", running, "ACCEPTED", "TRANSITION_ACCEPTED");

        var missingAuthority = BuildTransition(
            bootstrapRequest.Subject,
            subjectId,
            "missing-authority-record",
            "RUNNING",
            "STOPPING",
            4,
            bootstrap.BootstrapContextId,
            ObservationTime,
            includeDependency: false);
        var noAuthorityEvidence = missingAuthority.Evidence with { AuthorityDecision = null! };
        noAuthorityEvidence = noAuthorityEvidence with
        {
            ValidationEvidence = LifecycleEvidenceBinding.Compute(
                missingAuthority.Request,
                noAuthorityEvidence)
        };
        var noAuthority = service.Transition(missingAuthority.Request, noAuthorityEvidence);
        ExpectDecision(
            failures,
            "authority boolean replacement requires record",
            noAuthority,
            "REJECTED",
            "INVALID_LIFECYCLE_AUTHORITY_DECISION");

        var missingTime = BuildTransition(
            bootstrapRequest.Subject,
            subjectId,
            "missing-time-record",
            "RUNNING",
            "STOPPING",
            4,
            bootstrap.BootstrapContextId,
            ObservationTime,
            includeDependency: false);
        var noTimeEvidence = missingTime.Evidence with { TimeProvider = null! };
        noTimeEvidence = noTimeEvidence with
        {
            ValidationEvidence = LifecycleEvidenceBinding.Compute(
                missingTime.Request,
                noTimeEvidence)
        };
        var noTime = service.Transition(missingTime.Request, noTimeEvidence);
        ExpectDecision(
            failures,
            "trusted-time boolean replacement requires record",
            noTime,
            "REJECTED",
            "INVALID_LIFECYCLE_TIME_PROVIDER");

        var tampered = BuildTransition(
            bootstrapRequest.Subject,
            subjectId,
            "tampered-evidence-binding",
            "RUNNING",
            "STOPPING",
            4,
            bootstrap.BootstrapContextId,
            ObservationTime,
            includeDependency: false);
        var tamperedEvidence = tampered.Evidence with
        {
            ValidationEvidence = new string('F', 64)
        };
        var tamperedDecision = service.Transition(tampered.Request, tamperedEvidence);
        ExpectDecision(
            failures,
            "record bundle digest mismatch rejected",
            tamperedDecision,
            "REJECTED",
            "LIFECYCLE_EVIDENCE_BINDING_MISMATCH");

        var stopping = ExecuteTransition(
            service,
            bootstrapRequest.Subject,
            subjectId,
            "positive-stopping",
            "RUNNING",
            "STOPPING",
            4,
            bootstrap.BootstrapContextId,
            ObservationTime);
        ExpectDecision(failures, "running to stopping", stopping, "ACCEPTED", "TRANSITION_ACCEPTED");

        var stopped = ExecuteTransition(
            service,
            bootstrapRequest.Subject,
            subjectId,
            "positive-stopped",
            "STOPPING",
            "STOPPED",
            5,
            bootstrap.BootstrapContextId,
            ObservationTime);
        ExpectDecision(failures, "stopping to stopped", stopped, "ACCEPTED", "TRANSITION_ACCEPTED");

        Expect(
            failures,
            "only accepted transitions emit success events",
            service.GetEvents().Count == 5 &&
            service.GetContractRejections().Count >= 4 &&
            service.GetEvents().All(item => !string.IsNullOrWhiteSpace(item.EventId)));
    }

    private static void ValidateGlobalIdentityReservation(ICollection<string> failures)
    {
        var subjectId = "service.identity-reservation";
        var bootstrapRequest = BuildBootstrapRequest(subjectId, ObservationTime);
        var service = RegisterService(failures, bootstrapRequest, out var bootstrap);
        if (service is null)
        {
            return;
        }

        var first = BuildTransition(
            bootstrapRequest.Subject,
            subjectId,
            "identity-contract-rejection",
            "REGISTERED",
            "INITIALIZING",
            1,
            bootstrap.BootstrapContextId,
            ObservationTime,
            includeDependency: false);
        var invalidRequest = first.Request with { Requester = string.Empty };
        var invalidEvidence = first.Evidence with
        {
            ValidationEvidence = LifecycleEvidenceBinding.Compute(
                invalidRequest,
                first.Evidence)
        };
        var rejected = service.Transition(invalidRequest, invalidEvidence);
        ExpectDecision(
            failures,
            "contract rejection established",
            rejected,
            "REJECTED",
            "INVALID_CON003_REQUEST");

        var reusedEvidence = first.Evidence with
        {
            ValidationEvidence = LifecycleEvidenceBinding.Compute(
                first.Request,
                first.Evidence)
        };
        var reused = service.Transition(first.Request, reusedEvidence);
        ExpectDecision(
            failures,
            "request identity reserved after contract rejection",
            reused,
            "REJECTED",
            "DUPLICATE_REQUEST_ID");

        var unknown = BuildTransition(
            BuildSubject("service.unknown", BootstrapSubjectKind.Service, ObservationTime),
            "service.unknown",
            "unknown-subject-first",
            "REGISTERED",
            "INITIALIZING",
            1,
            "context:unknown",
            ObservationTime,
            includeDependency: false,
            transitionIdOverride: "transition:reserved-after-unknown",
            eventIdOverride: "event:reserved-after-unknown");
        var unknownDecision = service.Transition(unknown.Request, unknown.Evidence);
        ExpectDecision(
            failures,
            "unknown subject rejection established",
            unknownDecision,
            "REJECTED",
            "UNKNOWN_SUBJECT");

        var knownReuse = BuildTransition(
            bootstrapRequest.Subject,
            subjectId,
            "known-reuses-unknown-identities",
            "REGISTERED",
            "INITIALIZING",
            1,
            bootstrap.BootstrapContextId,
            ObservationTime,
            includeDependency: false,
            transitionIdOverride: "transition:reserved-after-unknown",
            eventIdOverride: "event:reserved-after-unknown");
        var knownReuseDecision = service.Transition(knownReuse.Request, knownReuse.Evidence);
        ExpectDecision(
            failures,
            "transition identity reserved after unknown-subject rejection",
            knownReuseDecision,
            "REJECTED",
            "DUPLICATE_TRANSITION_ID");

        var missingRequest = BuildTransition(
            bootstrapRequest.Subject,
            subjectId,
            "temporary-request-for-empty-id",
            "REGISTERED",
            "INITIALIZING",
            1,
            bootstrap.BootstrapContextId,
            ObservationTime,
            includeDependency: false,
            transitionIdOverride: "transition:reserved-with-empty-request",
            eventIdOverride: "event:reserved-with-empty-request");
        var emptyRequest = missingRequest.Request with { TransitionRequestId = string.Empty };
        var emptyAuthority = missingRequest.Evidence.AuthorityDecision with { RequestId = string.Empty };
        var emptyEvidence = missingRequest.Evidence with { AuthorityDecision = emptyAuthority };
        emptyEvidence = emptyEvidence with
        {
            ValidationEvidence = LifecycleEvidenceBinding.Compute(emptyRequest, emptyEvidence)
        };
        var emptyRejected = service.Transition(emptyRequest, emptyEvidence);
        ExpectDecision(
            failures,
            "nonempty identities reserved when request id missing",
            emptyRejected,
            "REJECTED",
            "INVALID_CON003_REQUEST");

        var afterEmpty = BuildTransition(
            bootstrapRequest.Subject,
            subjectId,
            "new-request-reuses-empty-identities",
            "REGISTERED",
            "INITIALIZING",
            1,
            bootstrap.BootstrapContextId,
            ObservationTime,
            includeDependency: false,
            transitionIdOverride: "transition:reserved-with-empty-request",
            eventIdOverride: "event:reserved-with-empty-request");
        var afterEmptyDecision = service.Transition(afterEmpty.Request, afterEmpty.Evidence);
        ExpectDecision(
            failures,
            "transition identity reserved despite empty request id",
            afterEmptyDecision,
            "REJECTED",
            "DUPLICATE_TRANSITION_ID");
    }

    private static void ValidateBootstrapExpiry(ICollection<string> failures)
    {
        var subjectId = "service.bootstrap-expiry";
        var shortExpiry = ObservationTime.AddMinutes(10);
        var request = BuildBootstrapRequest(
            subjectId,
            ObservationTime,
            subjectOverride: subject => subject with { Expiry = shortExpiry });
        var service = RegisterService(failures, request, out var bootstrap);
        if (service is null)
        {
            return;
        }

        Expect(
            failures,
            "earliest bootstrap expiry retained",
            bootstrap.ValidUntil == shortExpiry);

        var afterExpiry = ObservationTime.AddMinutes(11);
        var transition = ExecuteTransition(
            service,
            request.Subject,
            subjectId,
            "entry-after-bootstrap-expiry",
            "REGISTERED",
            "INITIALIZING",
            1,
            bootstrap.BootstrapContextId,
            afterExpiry,
            authorityStart: ObservationTime,
            timeStart: ObservationTime);

        ExpectDecision(
            failures,
            "lifecycle entry after bootstrap expiry rejected",
            transition,
            "REJECTED",
            "BOOTSTRAP_EVIDENCE_EXPIRED");
    }

    private static void ValidateRestrictedStoppedRecovery(ICollection<string> failures)
    {
        var subjectId = "service.restricted-stopped";
        var restriction = BuildRestriction(subjectId, ObservationTime);
        var request = BuildBootstrapRequest(
            subjectId,
            ObservationTime,
            restriction: restriction);
        var service = RegisterService(failures, request, out var bootstrap);
        if (service is null)
        {
            return;
        }

        var restricted = ExecuteTransition(
            service,
            request.Subject,
            subjectId,
            "restricted-entry",
            "REGISTERED",
            "RESTRICTED",
            1,
            bootstrap.BootstrapContextId,
            ObservationTime,
            restriction: restriction);
        ExpectDecision(failures, "registered to restricted", restricted, "ACCEPTED", "TRANSITION_ACCEPTED");

        var stopping = ExecuteTransition(
            service,
            request.Subject,
            subjectId,
            "restricted-stopping",
            "RESTRICTED",
            "STOPPING",
            2,
            bootstrap.BootstrapContextId,
            ObservationTime,
            restriction: restriction);
        ExpectDecision(failures, "restricted to stopping", stopping, "ACCEPTED", "TRANSITION_ACCEPTED");

        var stopped = ExecuteTransition(
            service,
            request.Subject,
            subjectId,
            "restricted-stopped",
            "STOPPING",
            "STOPPED",
            3,
            bootstrap.BootstrapContextId,
            ObservationTime,
            restriction: restriction);
        ExpectDecision(failures, "restricted stopping to stopped", stopped, "ACCEPTED", "TRANSITION_ACCEPTED");

        var noRelease = ExecuteTransition(
            service,
            request.Subject,
            subjectId,
            "restricted-recovery-without-release",
            "STOPPED",
            "RECOVERING",
            4,
            bootstrap.BootstrapContextId,
            ObservationTime,
            restriction: restriction);
        ExpectDecision(
            failures,
            "restricted stopped recovery requires release record",
            noRelease,
            "REJECTED",
            "RESTRICTION_RELEASE_EVIDENCE_REQUIRED");

        var recoveryRequestId = "restricted-recovery-with-release";
        var authorityDecisionId = $"authority:{recoveryRequestId}";
        var release = BuildRestrictionRelease(
            restriction,
            recoveryRequestId,
            authorityDecisionId,
            ObservationTime);
        var recovering = ExecuteTransition(
            service,
            request.Subject,
            subjectId,
            recoveryRequestId,
            "STOPPED",
            "RECOVERING",
            4,
            bootstrap.BootstrapContextId,
            ObservationTime,
            restriction: restriction,
            release: release);
        ExpectDecision(
            failures,
            "restricted stopped controlled release",
            recovering,
            "ACCEPTED",
            "TRANSITION_ACCEPTED");
        Expect(
            failures,
            "restriction cleared only after controlled recovery entry",
            recovering.Snapshot is not null &&
            recovering.Snapshot.State == LifecycleState.Recovering &&
            !recovering.Snapshot.ProtectiveRestrictionActive &&
            string.IsNullOrEmpty(recovering.Snapshot.ActiveRestrictionId) &&
            recovering.Snapshot.RestartAttempts == 1);

        var noIndependentRecovery = ExecuteTransition(
            service,
            request.Subject,
            subjectId,
            "ready-without-independent-recovery",
            "RECOVERING",
            "READY",
            5,
            bootstrap.BootstrapContextId,
            ObservationTime);
        ExpectDecision(
            failures,
            "recovery to ready requires independent record",
            noIndependentRecovery,
            "REJECTED",
            "RECOVERY_VALIDATION_EVIDENCE_REQUIRED");

        var readyRequestId = "ready-after-independent-recovery";
        var recoveryEvidence = BuildRecoveryValidation(
            subjectId,
            readyRequestId,
            bootstrap.BootstrapContextId,
            $"authority:{readyRequestId}",
            ObservationTime);
        var ready = ExecuteTransition(
            service,
            request.Subject,
            subjectId,
            readyRequestId,
            "RECOVERING",
            "READY",
            5,
            bootstrap.BootstrapContextId,
            ObservationTime,
            recovery: recoveryEvidence);
        ExpectDecision(
            failures,
            "independently validated recovery to ready",
            ready,
            "ACCEPTED",
            "TRANSITION_ACCEPTED");
    }

    private static void ValidateRestartBoundAndTerminalState(ICollection<string> failures)
    {
        var canonical = LifecycleStateModel.CreateCanonical();
        var boundedModel = new LifecycleStateModel(
            "SYS-002-CANONICAL-LIFECYCLE",
            "1.1",
            1,
            canonical.Rules);
        var subjectId = "service.restart-bound";
        var request = BuildBootstrapRequest(subjectId, ObservationTime);
        var bootstrap = BootstrapContextGate.Validate(request);
        var service = new LifecycleControlService(boundedModel);
        var registration = service.Register(request, "registration:restart-bound");

        Expect(
            failures,
            "bounded registration",
            bootstrap.Accepted && registration.Registration.Accepted);
        if (!registration.Registration.Accepted)
        {
            return;
        }

        ExpectDecision(
            failures,
            "restart path initializing",
            ExecuteTransition(
                service,
                request.Subject,
                subjectId,
                "restart-init-1",
                "REGISTERED",
                "INITIALIZING",
                1,
                bootstrap.BootstrapContextId,
                ObservationTime),
            "ACCEPTED",
            "TRANSITION_ACCEPTED");
        ExpectDecision(
            failures,
            "restart path ready",
            ExecuteTransition(
                service,
                request.Subject,
                subjectId,
                "restart-ready-1",
                "INITIALIZING",
                "READY",
                2,
                bootstrap.BootstrapContextId,
                ObservationTime),
            "ACCEPTED",
            "TRANSITION_ACCEPTED");
        ExpectDecision(
            failures,
            "restart path stopping",
            ExecuteTransition(
                service,
                request.Subject,
                subjectId,
                "restart-stopping-1",
                "READY",
                "STOPPING",
                3,
                bootstrap.BootstrapContextId,
                ObservationTime),
            "ACCEPTED",
            "TRANSITION_ACCEPTED");
        ExpectDecision(
            failures,
            "restart path stopped",
            ExecuteTransition(
                service,
                request.Subject,
                subjectId,
                "restart-stopped-1",
                "STOPPING",
                "STOPPED",
                4,
                bootstrap.BootstrapContextId,
                ObservationTime),
            "ACCEPTED",
            "TRANSITION_ACCEPTED");
        ExpectDecision(
            failures,
            "first bounded restart accepted",
            ExecuteTransition(
                service,
                request.Subject,
                subjectId,
                "restart-init-2",
                "STOPPED",
                "INITIALIZING",
                5,
                bootstrap.BootstrapContextId,
                ObservationTime),
            "ACCEPTED",
            "TRANSITION_ACCEPTED");
        ExpectDecision(
            failures,
            "second cycle ready",
            ExecuteTransition(
                service,
                request.Subject,
                subjectId,
                "restart-ready-2",
                "INITIALIZING",
                "READY",
                6,
                bootstrap.BootstrapContextId,
                ObservationTime),
            "ACCEPTED",
            "TRANSITION_ACCEPTED");
        ExpectDecision(
            failures,
            "second cycle stopping",
            ExecuteTransition(
                service,
                request.Subject,
                subjectId,
                "restart-stopping-2",
                "READY",
                "STOPPING",
                7,
                bootstrap.BootstrapContextId,
                ObservationTime),
            "ACCEPTED",
            "TRANSITION_ACCEPTED");
        ExpectDecision(
            failures,
            "second cycle stopped",
            ExecuteTransition(
                service,
                request.Subject,
                subjectId,
                "restart-stopped-2",
                "STOPPING",
                "STOPPED",
                8,
                bootstrap.BootstrapContextId,
                ObservationTime),
            "ACCEPTED",
            "TRANSITION_ACCEPTED");
        ExpectDecision(
            failures,
            "restart limit enforced",
            ExecuteTransition(
                service,
                request.Subject,
                subjectId,
                "restart-init-3",
                "STOPPED",
                "INITIALIZING",
                9,
                bootstrap.BootstrapContextId,
                ObservationTime),
            "REJECTED",
            "RESTART_LIMIT_EXCEEDED");

        var retireSubject = "service.retired-terminal";
        var retireRequest = BuildBootstrapRequest(retireSubject, ObservationTime);
        var retireService = RegisterService(failures, retireRequest, out var retireBootstrap);
        if (retireService is null)
        {
            return;
        }

        var retired = ExecuteTransition(
            retireService,
            retireRequest.Subject,
            retireSubject,
            "retire-from-registered",
            "REGISTERED",
            "RETIRED",
            1,
            retireBootstrap.BootstrapContextId,
            ObservationTime);
        ExpectDecision(failures, "retire accepted", retired, "ACCEPTED", "TRANSITION_ACCEPTED");

        var afterRetired = ExecuteTransition(
            retireService,
            retireRequest.Subject,
            retireSubject,
            "transition-after-retired",
            "RETIRED",
            "FAILED",
            2,
            retireBootstrap.BootstrapContextId,
            ObservationTime);
        ExpectDecision(
            failures,
            "retired terminal",
            afterRetired,
            "REJECTED",
            "RETIRED_STATE_IS_TERMINAL");
    }

    private static LifecycleControlService? RegisterService(
        ICollection<string> failures,
        BootstrapValidationRequest request,
        out BootstrapValidationResult bootstrap)
    {
        bootstrap = BootstrapContextGate.Validate(request);
        var service = new LifecycleControlService(LifecycleStateModel.CreateCanonical());
        var registration = service.Register(request, $"registration-evidence:{request.Subject.SubjectId}");

        Expect(
            failures,
            $"registration {request.Subject.SubjectId}",
            bootstrap.Accepted &&
            registration.BootstrapResult.DecisionIdentity == bootstrap.DecisionIdentity &&
            registration.Registration.Accepted &&
            registration.Registration.Snapshot?.State == LifecycleState.Registered);

        return registration.Registration.Accepted ? service : null;
    }

    private static BootstrapSubjectAdmissionEvidence BuildSubject(
        string subjectId,
        BootstrapSubjectKind kind,
        DateTimeOffset observationTime)
        => new()
        {
            SubjectId = subjectId,
            SubjectVersion = "1.0",
            SubjectKind = kind,
            ArtifactIdentity = $"artifact:{subjectId}:1.0",
            ArtifactDigest = ArtifactDigest,
            ManifestIdentity = $"manifest:{subjectId}:1.0",
            ManifestDigest = ManifestDigest,
            AdmissionDecisionId = $"admission:{subjectId}:1.0",
            AdmissionState = "ADMITTED",
            RegistrationEvidenceId = kind == BootstrapSubjectKind.Service
                ? $"registration:{subjectId}:1.0"
                : $"not-applicable:{subjectId}:1.0",
            RegistrationState = kind == BootstrapSubjectKind.Service
                ? "REGISTERED"
                : "NOT_APPLICABLE",
            EvidenceAuthority = "FALCON-STAGE3-WP05-EVIDENCE",
            EffectiveTime = observationTime.AddHours(-1),
            Expiry = observationTime.AddHours(2)
        };

    private static BootstrapValidationRequest BuildBootstrapRequest(
        string subjectId,
        DateTimeOffset observationTime,
        BootstrapSubjectKind kind = BootstrapSubjectKind.Service,
        RestrictionRecord? restriction = null,
        Func<BootstrapSubjectAdmissionEvidence, BootstrapSubjectAdmissionEvidence>? subjectOverride = null,
        Func<BootstrapExecutionContextRecord, BootstrapExecutionContextRecord>? contextOverride = null,
        Func<DependencyActivationEvidence, DependencyActivationEvidence>? dependencyOverride = null)
    {
        var subject = BuildSubject(subjectId, kind, observationTime);
        if (subjectOverride is not null)
        {
            subject = subjectOverride(subject);
        }

        var context = new BootstrapExecutionContextRecord(
            $"context:{subject.SubjectId}:{subject.SubjectVersion}",
            "1.0",
            "FALCON-STAGE3-WP05-AUTHORITY",
            "ENV-STAGE3-WP05-ISOLATED",
            "STAGE3-WP05-BOOTSTRAP-AND-LIFECYCLE",
            "EXTERNAL-BOOTSTRAP-CONTROL",
            $"context-evidence:{subject.SubjectId}",
            "DEFINED",
            "NO-PRODUCTION-NO-FINANCIAL-NO-EXTERNAL-CONNECTIVITY",
            observationTime.AddHours(-1),
            observationTime.AddHours(2));
        if (contextOverride is not null)
        {
            context = contextOverride(context);
        }

        var provenance = new BootstrapEvidenceProvenanceRecord(
            $"provenance:{subject.SubjectId}:{subject.SubjectVersion}",
            "1.0",
            $"source-record:{subject.SubjectId}:{subject.SubjectVersion}",
            subject.ArtifactDigest,
            context.SourceIdentity,
            "FALCON-STAGE3-WP05-PROVENANCE-AUTHORITY",
            $"provenance-evidence:{subject.SubjectId}",
            "PROVEN",
            subject.ArtifactIdentity,
            observationTime.AddHours(-1),
            observationTime.AddHours(2));

        var timeProvider = BuildTimeProvider(observationTime, observationTime.AddHours(-1));
        var dependency = BuildDependency(subject, observationTime);
        if (dependencyOverride is not null)
        {
            dependency = dependencyOverride(dependency);
        }

        return new BootstrapValidationRequest
        {
            RequestId = $"bootstrap-request:{subject.SubjectId}:{subject.SubjectVersion}",
            Subject = subject,
            Context = context,
            Provenance = provenance,
            TimeProvider = timeProvider,
            DependencyEvidence = dependency,
            Restriction = restriction,
            ObservationTime = observationTime
        };
    }

    private static DependencyActivationEvidence BuildDependency(
        BootstrapSubjectAdmissionEvidence subject,
        DateTimeOffset observationTime)
        => new()
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
            EffectiveTime = observationTime.AddHours(-1),
            Expiry = observationTime.AddHours(2)
        };

    private static RestrictionRecord BuildRestriction(
        string subjectId,
        DateTimeOffset observationTime)
        => new(
            $"restriction:{subjectId}",
            "1.0",
            subjectId,
            "GUARDIAN-MANDATE-001",
            "TRIGGER-EVIDENCE-001",
            "RESTRICTED",
            "EVIDENCE_EXPORT,CONTROLLED_STOP,RECOVERY_PREPARATION",
            "RUNNING,AUTHORITY_EXPANSION,SELF_RELEASE",
            "INDEPENDENT_VALIDATION_AND_NEW_AUTHORITY_DECISION",
            "FALCON-RELEASE-AUTHORITY",
            "IMPOSED",
            "RESTRICTION-INTEGRITY-001",
            observationTime.AddHours(-1),
            observationTime.AddHours(1));

    private static RestrictionReleaseEvidence BuildRestrictionRelease(
        RestrictionRecord restriction,
        string transitionRequestId,
        string authorityDecisionId,
        DateTimeOffset observationTime)
        => new()
        {
            ReleaseDecisionId = $"release:{restriction.RestrictionId}:{transitionRequestId}",
            RestrictionId = restriction.RestrictionId,
            SubjectId = restriction.SubjectId,
            TransitionRequestId = transitionRequestId,
            ReleaseAuthority = restriction.ReleaseAuthority,
            ReleaseConditionsEvidence = "RELEASE-CONDITIONS-SATISFIED-001",
            IndependentValidationEvidence = "INDEPENDENT-VALIDATION-001",
            NewAuthorityDecisionReference = authorityDecisionId,
            ReleaseState = "RELEASED",
            EffectiveTime = observationTime.AddMinutes(-10),
            Expiry = observationTime.AddHours(1)
        };

    private static RecoveryValidationEvidence BuildRecoveryValidation(
        string subjectId,
        string transitionRequestId,
        string bootstrapContextId,
        string authorityDecisionId,
        DateTimeOffset observationTime)
        => new()
        {
            ValidationId = $"recovery-validation:{subjectId}:{transitionRequestId}",
            SubjectId = subjectId,
            TransitionRequestId = transitionRequestId,
            BootstrapContextId = bootstrapContextId,
            ValidatorAuthority = "FALCON-INDEPENDENT-RECOVERY-VALIDATOR",
            AuthorityDecisionReference = authorityDecisionId,
            ValidationResult = "VALIDATED",
            EvidenceReference = $"recovery-evidence:{transitionRequestId}",
            EffectiveTime = observationTime.AddMinutes(-10),
            Expiry = observationTime.AddHours(1)
        };

    private static LifecycleControlDecision ExecuteTransition(
        LifecycleControlService service,
        BootstrapSubjectAdmissionEvidence subject,
        string subjectId,
        string requestId,
        string source,
        string target,
        long expectedVersion,
        string bootstrapContextId,
        DateTimeOffset observationTime,
        bool includeDependency = false,
        RestrictionRecord? restriction = null,
        RestrictionReleaseEvidence? release = null,
        RecoveryValidationEvidence? recovery = null,
        DateTimeOffset? authorityStart = null,
        DateTimeOffset? timeStart = null)
    {
        var transition = BuildTransition(
            subject,
            subjectId,
            requestId,
            source,
            target,
            expectedVersion,
            bootstrapContextId,
            observationTime,
            includeDependency,
            restriction,
            release,
            recovery,
            authorityStart,
            timeStart);

        return service.Transition(transition.Request, transition.Evidence);
    }

    private static (LifecycleTransitionRequest Request, LifecycleTransitionEvidence Evidence) BuildTransition(
        BootstrapSubjectAdmissionEvidence subject,
        string subjectId,
        string requestId,
        string source,
        string target,
        long expectedVersion,
        string bootstrapContextId,
        DateTimeOffset observationTime,
        bool includeDependency = false,
        RestrictionRecord? restriction = null,
        RestrictionReleaseEvidence? release = null,
        RecoveryValidationEvidence? recovery = null,
        DateTimeOffset? authorityStart = null,
        DateTimeOffset? timeStart = null,
        string? transitionIdOverride = null,
        string? eventIdOverride = null)
    {
        var authorityDecisionId = $"authority:{requestId}";
        var dependency = includeDependency
            ? BuildDependency(subject, observationTime)
            : null;
        var dependencyContext = dependency?.EvidenceReference ?? $"dependency-context:{subjectId}:not-required";
        var request = new LifecycleTransitionRequest(
            requestId,
            subjectId,
            source,
            target,
            "FALCON-STAGE3-WP05-CONTROLLER",
            authorityDecisionId,
            $"transition {source} to {target}",
            dependencyContext,
            observationTime.AddMinutes(-1),
            observationTime.AddHours(1));

        var authorityDecision = BuildAuthorityDecision(
            request,
            authorityStart ?? observationTime.AddMinutes(-5),
            observationTime.AddHours(1));
        var evidence = new LifecycleTransitionEvidence
        {
            TransitionId = transitionIdOverride ?? $"transition:{requestId}",
            EventId = eventIdOverride ?? $"event:{requestId}",
            ModelId = "SYS-002-CANONICAL-LIFECYCLE",
            ModelVersion = "1.1",
            ExpectedStateVersion = expectedVersion,
            BootstrapContextId = bootstrapContextId,
            ObservationTime = observationTime,
            AuthorityDecision = authorityDecision,
            TimeProvider = BuildTimeProvider(
                observationTime,
                timeStart ?? observationTime.AddHours(-1)),
            DependencyEvidence = dependency,
            Restriction = restriction,
            RestrictionRelease = release,
            RecoveryValidation = recovery
        };
        evidence = evidence with
        {
            ValidationEvidence = LifecycleEvidenceBinding.Compute(request, evidence)
        };

        return (request, evidence);
    }

    private static AuthorityResult BuildAuthorityDecision(
        LifecycleTransitionRequest request,
        DateTimeOffset effectiveTime,
        DateTimeOffset expiry)
        => new(
            request.TransitionRequestId,
            request.AuthorityReference,
            "ACCEPTED",
            $"LIFECYCLE:{request.ComponentIdentity}:{request.AuthoritativeSourceState}->{request.RequestedTargetState}",
            "GOV-097",
            "1.0",
            "BOUND_EVIDENCE_REQUIRED",
            "NO_BYPASS,NO_SELF_ATTESTATION",
            "BOUNDED_WP05_LIFECYCLE_TRANSITION",
            effectiveTime,
            expiry,
            $"authority-evidence:{request.TransitionRequestId}");

    private static TimeProviderRecord BuildTimeProvider(
        DateTimeOffset observationTime,
        DateTimeOffset effectiveTime)
        => new(
            "TIME-PROVIDER-ACTIVE-001",
            "1.0",
            "FOUNDATION_TIME_PROVIDER",
            "GOV-027",
            "STAGE3-WP05-ISOLATED",
            "FALCON-GOVERNED-TIME",
            "TIME-EVIDENCE-001",
            "ADMITTED",
            effectiveTime,
            observationTime.AddHours(2));

    private static void ExpectBootstrap(
        ICollection<string> failures,
        string scenario,
        BootstrapValidationResult result,
        string expectedDecision,
        string expectedReason)
        => Expect(
            failures,
            scenario,
            result.Decision == expectedDecision &&
            result.ReasonCode == expectedReason &&
            result.DecisionIdentity.Length == 64,
            $"{result.Decision}/{result.ReasonCode}");

    private static void ExpectDecision(
        ICollection<string> failures,
        string scenario,
        LifecycleControlDecision decision,
        string expectedDecision,
        string expectedReason)
        => Expect(
            failures,
            scenario,
            decision.ContractResult.Decision == expectedDecision &&
            decision.ContractResult.Reason == expectedReason &&
            (expectedDecision == "ACCEPTED"
                ? decision.Event is not null
                : decision.Event is null),
            $"{decision.ContractResult.Decision}/{decision.ContractResult.Reason}");

    private static void Expect(
        ICollection<string> failures,
        string scenario,
        bool condition,
        string? detail = null)
    {
        if (!condition)
        {
            failures.Add(detail is null
                ? scenario
                : $"{scenario}: {detail}");
        }
    }
}
