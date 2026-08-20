using System;
using System.Linq;
using System.Reflection;
using Foundation.ApplicationLifecycle;
using Foundation.Authority;
using Foundation.Contracts;

namespace Falcon.Stage8.WP08.Verifier;

internal static class Program
{
    private static readonly DateTimeOffset Now = new(2026, 8, 15, 12, 0, 0, TimeSpan.Zero);
    private static int _checks;

    private static int Main()
    {
        try
        {
            var localRequest = EmergencyRequest(
                "emergency:wp08:local",
                "owner:governed",
                "guardian:primary",
                EmergencyControlScopeKind.Principal,
                "guardian:primary",
                EmergencyControlAction.EmergencyStop);
            var localAuthorityRequest = EmergencyAuthorityRequest(localRequest);
            var localAuthorityContext = EmergencyAuthorityContext(localRequest, localAuthorityRequest);
            var localEvidence = BlastEvidence(
                localRequest,
                EmergencyTrustState.Trustworthy,
                EmergencyPropagationState.Excluded,
                EmergencyTrustState.Trustworthy,
                EmergencyTrustState.Trustworthy,
                guardianCompromise: false,
                guardianSoleSource: false);
            var local = IndependentEmergencyControlRuntime.Evaluate(
                localRequest, localAuthorityRequest, localAuthorityContext, localEvidence, Now);

            // 01
            Check(local.Accepted && local.Reason == IndependentEmergencyControlReason.AcceptedLocal,
                "trusted local emergency control was not accepted locally");
            // 02
            Check(local.EffectiveScopeKind == EmergencyControlScopeKind.Principal && local.EffectiveScopeId == "guardian:primary",
                "trusted local boundary did not preserve exact principal scope");
            // 03
            Check(local.UnaffectedOperationEligible && local.UnaffectedOperationStillRequiresAuthority,
                "unaffected operation eligibility incorrectly changed authority semantics");
            // 04
            Check(IndependentEmergencyControlRuntime.ValidateDecision(local),
                "accepted decision failed canonical validation");
            // 05
            Check(typeof(IndependentEmergencyControlDecision)
                    .GetConstructors(BindingFlags.Public | BindingFlags.Instance).Length == 0,
                "external caller can construct an accepted emergency decision");

            var mutatedEvidence = localEvidence with { EvidenceId = "blast:wp08:mutation" };
            var mutatedDecision = IndependentEmergencyControlRuntime.Evaluate(
                localRequest, localAuthorityRequest, localAuthorityContext, mutatedEvidence, Now);
            // 06
            Check(mutatedDecision.Accepted && mutatedDecision.Identity != local.Identity,
                "decision identity was not mutation-sensitive");

            var compromisedGuardian = IndependentEmergencyControlRuntime.Evaluate(
                localRequest,
                localAuthorityRequest,
                localAuthorityContext,
                BlastEvidence(
                    localRequest,
                    EmergencyTrustState.Trustworthy,
                    EmergencyPropagationState.Excluded,
                    EmergencyTrustState.Trustworthy,
                    EmergencyTrustState.Trustworthy,
                    guardianCompromise: true,
                    guardianSoleSource: true),
                Now);
            // 07
            Check(compromisedGuardian.Accepted &&
                  compromisedGuardian.Reason == IndependentEmergencyControlReason.AcceptedExpanded &&
                  compromisedGuardian.EffectiveScopeKind == EmergencyControlScopeKind.FalconWide,
                "compromised Guardian sole evidence preserved local containment");
            // 08
            Check(!compromisedGuardian.UnaffectedOperationEligible,
                "Falcon-wide expansion preserved unaffected-operation eligibility");

            var unknownPropagation = IndependentEmergencyControlRuntime.Evaluate(
                localRequest,
                localAuthorityRequest,
                localAuthorityContext,
                BlastEvidence(
                    localRequest,
                    EmergencyTrustState.Trustworthy,
                    EmergencyPropagationState.Unknown,
                    EmergencyTrustState.Trustworthy,
                    EmergencyTrustState.Trustworthy,
                    false,
                    false),
                Now);
            // 09
            Check(unknownPropagation.EffectiveScopeKind == EmergencyControlScopeKind.FalconWide,
                "unknown propagation did not expand containment");

            var untrustedEvidenceSource = IndependentEmergencyControlRuntime.Evaluate(
                localRequest,
                localAuthorityRequest,
                localAuthorityContext,
                BlastEvidence(
                    localRequest,
                    EmergencyTrustState.Trustworthy,
                    EmergencyPropagationState.Excluded,
                    EmergencyTrustState.Trustworthy,
                    EmergencyTrustState.Compromised,
                    false,
                    false),
                Now);
            // 10
            Check(untrustedEvidenceSource.EffectiveScopeKind == EmergencyControlScopeKind.FalconWide,
                "untrusted evidence source preserved local containment");

            var untrustedUnaffected = IndependentEmergencyControlRuntime.Evaluate(
                localRequest,
                localAuthorityRequest,
                localAuthorityContext,
                BlastEvidence(
                    localRequest,
                    EmergencyTrustState.Trustworthy,
                    EmergencyPropagationState.Excluded,
                    EmergencyTrustState.Unavailable,
                    EmergencyTrustState.Trustworthy,
                    false,
                    false),
                Now);
            // 11
            Check(untrustedUnaffected.EffectiveScopeKind == EmergencyControlScopeKind.FalconWide &&
                  !untrustedUnaffected.UnaffectedOperationEligible,
                "untrusted unaffected scope did not fail closed");

            var deniedContext = EmergencyAuthorityContext(
                localRequest,
                localAuthorityRequest,
                allowedActionOverride: "HOLD");
            var deniedAuthority = IndependentEmergencyControlRuntime.Evaluate(
                localRequest, localAuthorityRequest, deniedContext, localEvidence, Now);
            // 12
            Check(!deniedAuthority.Accepted && deniedAuthority.Reason == IndependentEmergencyControlReason.AuthorityNotGranted,
                "AUT-001 denial did not block emergency control");

            var mismatchedAuthorityRequest = localAuthorityRequest with { Correlation = "correlation:wrong" };
            var bindingMismatch = IndependentEmergencyControlRuntime.Evaluate(
                localRequest,
                mismatchedAuthorityRequest,
                EmergencyAuthorityContext(localRequest, mismatchedAuthorityRequest),
                localEvidence,
                Now);
            // 13
            Check(!bindingMismatch.Accepted && bindingMismatch.Reason == IndependentEmergencyControlReason.AuthorityBindingMismatch,
                "authority/request binding mismatch was accepted");

            var wrongTargetEvidence = localEvidence with { TargetSubjectId = "guardian:other" };
            var wrongTarget = IndependentEmergencyControlRuntime.Evaluate(
                localRequest, localAuthorityRequest, localAuthorityContext, wrongTargetEvidence, Now);
            // 14
            Check(!wrongTarget.Accepted && wrongTarget.Reason == IndependentEmergencyControlReason.InvalidBlastRadiusEvidence,
                "blast-radius evidence for another target was accepted");

            var restriction = IndependentEmergencyControlRuntime.CreateTargetRestrictionRecord(
                local, "restriction:wp08:guardian-primary");
            // 15
            Check(restriction.ProtectiveMode == "SAFE" &&
                  restriction.AllowedSafeActions == ProtectiveSafeStateContractPolicy.CanonicalAllowedSafeActions,
                "target restriction did not preserve canonical Safe-State policy");
            // 16
            Check(restriction.Expiry == DateTimeOffset.MaxValue &&
                  restriction.ReleaseConditions == "STAGE9_INDEPENDENT_RECOVERY_VALIDATION_AND_AUTHORIZED_RELEASE_REQUIRED" &&
                  restriction.ReleaseAuthority == "INDEPENDENT_GOVERNED_RELEASE_AUTHORITY",
                "target containment was time-releasable or leaked release ownership");
            // 17
            Check(restriction.IntegrityEvidence == local.Identity,
                "target restriction lost independent-control provenance");

            var lifecycle = ProtectiveLifecycleEnforcer.Enforce(new ProtectiveLifecycleRequest(
                "lifecycle:wp08:guardian-primary",
                restriction.SubjectId,
                restriction.RestrictionId,
                restriction.IntegrityEvidence,
                restriction.MandateReference,
                restriction.TriggerEvidence,
                restriction.ProtectiveMode,
                ProtectiveLifecycleEvidenceState.Valid,
                ProtectiveLifecycleEvidenceState.Valid,
                restriction.EffectiveTime,
                Now.AddMinutes(1)));
            // 18
            Check(lifecycle.Success &&
                  lifecycle.Target == ProtectiveLifecycleTarget.Stopped &&
                  lifecycle.IsolationRequired &&
                  !lifecycle.NewExecutionAllowed &&
                  lifecycle.RemainsRestricted,
                "Lifecycle did not enforce SAFE target stop/isolation");

            var appRequest = EmergencyRequest(
                "emergency:wp08:app-alpha",
                "owner:governed",
                "ai:alpha:1",
                EmergencyControlScopeKind.Application,
                "application:alpha",
                EmergencyControlAction.IsolateTarget);
            var appAuthorityRequest = EmergencyAuthorityRequest(appRequest);
            var appDecision = IndependentEmergencyControlRuntime.Evaluate(
                appRequest,
                appAuthorityRequest,
                EmergencyAuthorityContext(appRequest, appAuthorityRequest),
                BlastEvidence(
                    appRequest,
                    EmergencyTrustState.Trustworthy,
                    EmergencyPropagationState.Excluded,
                    EmergencyTrustState.Trustworthy,
                    EmergencyTrustState.Trustworthy,
                    false,
                    false),
                Now);
            // 19
            Check(appDecision.Accepted && appDecision.EffectiveScopeKind == EmergencyControlScopeKind.Application,
                "proven Application-local containment did not remain local");

            var enforcer = new IndependentEmergencyControlAuthorityEnforcer();
            var appExecution = ExecutionRequest(
                "exec:wp08:app-alpha",
                "ai:alpha:2",
                "EXECUTE",
                "resource:alpha",
                "application:alpha:worker");
            var deniedAppExecution = enforcer.Evaluate(
                appExecution,
                ExecutionContext(appExecution, Now),
                Array.Empty<RestrictionRecord>(),
                new[] { appDecision });
            // 20
            Check(deniedAppExecution.Decision == AuthorityDecision.Deny &&
                  deniedAppExecution.Reason == IndependentEmergencyAuthorityReason.Restricted,
                "Application-scope containment did not deny governed execution");

            var health = appExecution with { RequestId = "exec:wp08:app-alpha:health", Action = "REPORT_HEALTH" };
            var healthResult = enforcer.Evaluate(
                health,
                ExecutionContext(health, Now),
                Array.Empty<RestrictionRecord>(),
                new[] { appDecision });
            // 21
            Check(healthResult.Decision == AuthorityDecision.Allow,
                "canonical Safe-State health action did not remain subject to independent AUT-001");

            var unrelated = ExecutionRequest(
                "exec:wp08:beta",
                "ai:beta:1",
                "EXECUTE",
                "resource:beta",
                "application:beta");
            var unrelatedContext = ExecutionContext(unrelated, Now);
            // 22
            Check(enforcer.Evaluate(
                    unrelated, unrelatedContext, Array.Empty<RestrictionRecord>(), new[] { local }).Decision == AuthorityDecision.Allow,
                "principal-local containment leaked into unrelated trustworthy scope");
            // 23
            Check(enforcer.Evaluate(
                    unrelated, unrelatedContext, Array.Empty<RestrictionRecord>(), new[] { compromisedGuardian }).Decision == AuthorityDecision.Deny,
                "Falcon-wide expansion did not deny unrelated execution");

            var afterReview = ExecutionRequest(
                "exec:wp08:after-review",
                "guardian:primary",
                "EXECUTE",
                "resource:guardian",
                "guardian:primary");
            // 24
            Check(enforcer.Evaluate(
                    afterReview,
                    ExecutionContext(afterReview, local.ReviewDeadline.AddMinutes(10)),
                    Array.Empty<RestrictionRecord>(),
                    new[] { local }).Decision == AuthorityDecision.Deny,
                "review deadline released containment");

            // 25
            Check(enforcer.Evaluate(
                    unrelated, unrelatedContext, Array.Empty<RestrictionRecord>(), null).Reason ==
                  IndependentEmergencyAuthorityReason.EvidenceUnavailable,
                "missing emergency-control evidence did not fail closed");

            var falconRequest = EmergencyRequest(
                "emergency:wp08:falcon",
                "owner:governed",
                "ai:any",
                EmergencyControlScopeKind.FalconWide,
                "falcon:platform",
                EmergencyControlAction.EnterPlatformSafe);
            var falconAuthorityRequest = EmergencyAuthorityRequest(falconRequest);
            var falconDecision = IndependentEmergencyControlRuntime.Evaluate(
                falconRequest,
                falconAuthorityRequest,
                EmergencyAuthorityContext(falconRequest, falconAuthorityRequest),
                BlastEvidence(
                    falconRequest,
                    EmergencyTrustState.Unavailable,
                    EmergencyPropagationState.Unknown,
                    EmergencyTrustState.Unavailable,
                    EmergencyTrustState.Unavailable,
                    true,
                    true),
                Now);
            // 26
            Check(falconDecision.Accepted && falconDecision.EffectiveScopeKind == EmergencyControlScopeKind.FalconWide,
                "explicit authorized Falcon-wide emergency control was not preserved");

            // 27
            Check(IndependentEmergencyControlRuntime.ToAuthorityAction(EmergencyControlAction.Hold) == "HOLD" &&
                  IndependentEmergencyControlRuntime.ToAuthorityAction(EmergencyControlAction.DenyNewActivity) == "DENY_NEW_ACTIVITY" &&
                  IndependentEmergencyControlRuntime.ToAuthorityAction(EmergencyControlAction.IsolateTarget) == "ISOLATE_TARGET" &&
                  IndependentEmergencyControlRuntime.ToAuthorityAction(EmergencyControlAction.EnterPlatformSafe) == "ENTER_PLATFORM_SAFE" &&
                  IndependentEmergencyControlRuntime.ToAuthorityAction(EmergencyControlAction.EmergencyStop) == "EMERGENCY_STOP",
                "narrow emergency action vocabulary drifted");

            var publicMethods = typeof(IndependentEmergencyControlRuntime)
                .GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
            // 28
            Check(!publicMethods.Any(method =>
                    method.Name.Contains("Release", StringComparison.OrdinalIgnoreCase) ||
                    method.Name.Contains("Recover", StringComparison.OrdinalIgnoreCase) ||
                    method.Name.Contains("RestoreTrust", StringComparison.OrdinalIgnoreCase) ||
                    method.Name.Contains("Revival", StringComparison.OrdinalIgnoreCase)),
                "Stage 9 recovery/release surface leaked into WP-08");

            // 29
            Check(!typeof(IndependentEmergencyControlRuntime).Assembly
                    .GetReferencedAssemblies()
                    .Any(name => string.Equals(name.Name, "Foundation.Guardian", StringComparison.Ordinal)),
                "independent emergency path depends on Foundation.Guardian");

            // 30
            Check(!typeof(IndependentEmergencyControlRuntime).Assembly
                    .GetExportedTypes()
                    .Select(type => type.Name)
                    .Any(name =>
                        name.Contains("Trade", StringComparison.OrdinalIgnoreCase) ||
                        name.Contains("Strategy", StringComparison.OrdinalIgnoreCase) ||
                        name.Contains("Portfolio", StringComparison.OrdinalIgnoreCase) ||
                        name.Contains("Broker", StringComparison.OrdinalIgnoreCase) ||
                        name.Contains("Market", StringComparison.OrdinalIgnoreCase)),
                "Application business semantics leaked into Foundation.Authority");

            if (_checks != 30)
                throw new InvalidOperationException($"Unexpected check count: {_checks}, expected 30.");

            Console.WriteLine("STAGE8_WP08_VERIFIER = PASS");
            Console.WriteLine("CHECKS = 30/30");
            Console.WriteLine("INDEPENDENT_EMERGENCY_CONTROL = AUTHORITY_OWNED_NOT_GUARDIAN_OWNED");
            Console.WriteLine("EMERGENCY_DECISION_CONSTRUCTION = FOUNDATION_AUTHORITY_INTERNAL_ONLY");
            Console.WriteLine("UNTRUSTED_BLAST_RADIUS = EXPAND_CONTAINMENT");
            Console.WriteLine("UNAFFECTED_OPERATION != AUTHORITY_GRANT");
            Console.WriteLine("REVIEW_DEADLINE != RELEASE");
            Console.WriteLine("STAGE9_RECOVERY_RELEASE = NOT_GRANTED");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("STAGE8_WP08_VERIFIER = FAIL");
            Console.Error.WriteLine(ex.Message);
            return 1;
        }
    }

    private static IndependentEmergencyControlRequest EmergencyRequest(
        string id,
        string actor,
        string target,
        EmergencyControlScopeKind scopeKind,
        string scopeId,
        EmergencyControlAction action)
        => new(
            id,
            actor,
            target,
            action,
            scopeKind,
            scopeId,
            "AUT-002:v1.0/STAGE8-WP08",
            "correlation:" + id,
            Now.AddMinutes(-5),
            Now.AddHours(1));

    private static AuthorityRequest EmergencyAuthorityRequest(IndependentEmergencyControlRequest request)
        => new(
            "authority-request:" + request.RequestId,
            request.ActorIdentity,
            IndependentEmergencyControlRuntime.ToAuthorityAction(request.Action),
            request.TargetSubjectId,
            "PROTECTIVE_EMERGENCY_CONTROL",
            request.RequestedScopeId,
            "EMERGENCY_CONTROL",
            "TRUSTED",
            "FIT",
            request.Correlation,
            request.RequestTime,
            request.Expiry);

    private static AuthorityEvaluationContext EmergencyAuthorityContext(
        IndependentEmergencyControlRequest request,
        AuthorityRequest authorityRequest,
        string? allowedActionOverride = null)
    {
        var action = allowedActionOverride ?? authorityRequest.Action;
        return new AuthorityEvaluationContext(
            new AuthorityPolicy(
                "policy:wp08:emergency",
                "1.0",
                "owner:governed",
                Now.AddHours(-1),
                Now.AddHours(2),
                new[] { request.ActorIdentity },
                new[] { action },
                new[] { request.TargetSubjectId },
                new[] { "PROTECTIVE_EMERGENCY_CONTROL" },
                new[] { request.RequestedScopeId },
                new[] { "TRUSTED" }),
            new DelegationEvidence(
                "delegation:wp08:emergency",
                request.ActorIdentity,
                "owner:governed",
                new[] { request.RequestedScopeId },
                Now.AddHours(-1),
                Now.AddHours(2),
                false),
            new FitnessEvidence(
                request.ActorIdentity,
                "FIT",
                true,
                Now.AddMinutes(-10),
                Now.AddHours(2),
                "fitness:wp08:emergency"),
            Now,
            "authority-evidence:wp08:emergency");
    }

    private static EmergencyBlastRadiusEvidence BlastEvidence(
        IndependentEmergencyControlRequest request,
        EmergencyTrustState local,
        EmergencyPropagationState propagation,
        EmergencyTrustState unaffected,
        EmergencyTrustState source,
        bool guardianCompromise,
        bool guardianSoleSource)
        => new(
            "blast:" + request.RequestId,
            request.TargetSubjectId,
            request.RequestedScopeId,
            local,
            propagation,
            unaffected,
            source,
            guardianCompromise,
            guardianSoleSource,
            Now.AddMinutes(-2),
            Now.AddHours(1));

    private static AuthorityRequest ExecutionRequest(
        string id,
        string actor,
        string action,
        string resource,
        string scope)
        => new(
            id,
            actor,
            action,
            resource,
            "EXECUTION",
            scope,
            "LIVE",
            "TRUSTED",
            "FIT",
            "correlation:" + id,
            Now.AddMinutes(-1),
            Now.AddDays(2));

    private static AuthorityEvaluationContext ExecutionContext(
        AuthorityRequest request,
        DateTimeOffset observationTime)
        => new(
            new AuthorityPolicy(
                "policy:" + request.RequestId,
                "1.0",
                "owner:governed",
                observationTime.AddHours(-1),
                observationTime.AddDays(2),
                new[] { request.ActorIdentity },
                new[] { request.Action },
                new[] { request.Resource },
                new[] { request.Purpose },
                new[] { request.RequestedScope },
                new[] { "TRUSTED" }),
            new DelegationEvidence(
                "delegation:" + request.RequestId,
                request.ActorIdentity,
                "owner:governed",
                new[] { request.RequestedScope },
                observationTime.AddHours(-1),
                observationTime.AddDays(2),
                false),
            new FitnessEvidence(
                request.ActorIdentity,
                "FIT",
                true,
                observationTime.AddMinutes(-10),
                observationTime.AddDays(2),
                "fitness:" + request.RequestId),
            observationTime,
            "authority-evidence:" + request.RequestId);

    private static void Check(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException("CHECK FAILED: " + message);
        _checks++;
    }
}
