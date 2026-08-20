using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Foundation.Authority;
using Foundation.Contracts;

namespace Falcon.Stage13.WP01.AiKillControl.Verifier;

internal static class ProgramFixed
{
    private static readonly DateTimeOffset Now = new(2026, 8, 16, 17, 0, 0, TimeSpan.Zero);
    private static int _checks;

    private static int Main()
    {
        try
        {
            var registry = Registry();
            var target = Request("kill:targeted", "owner:governed", AiKillIngress.WebOwner, AiEmergencyAction.Kill, AiTargetKind.Msa, "ai:app-alpha:msa");
            var targetDecision = Evaluate(target, registry);
            Check(targetDecision.Accepted && targetDecision.Reason == AiKillControlReason.AcceptedTargeted, "targeted MSA Kill was not accepted");
            Check(targetDecision.ImpactedTargetIds.SequenceEqual(new[] { "ai:app-alpha:csa:one", "ai:app-alpha:lsa", "ai:app-alpha:msa" }), "targeted hierarchy was not exact");
            Check(targetDecision.StopRequired && targetDecision.IsolationRequired && targetDecision.AuthorityRevocationRequired, "Kill semantics incomplete");
            Check(targetDecision.EvidenceFreezeRequired && targetDecision.ReleaseRequiresGovernedRecovery, "Kill evidence/recovery semantics incomplete");
            Check(targetDecision.SafeCorePreserved && !targetDecision.FalconShutdownAuthorized, "targeted Kill authorized Falcon shutdown");
            Check(!targetDecision.TargetCooperationRequired, "target cooperation was required");
            Check(AiKillControlRuntime.ValidateDecision(targetDecision), "accepted targeted decision failed canonical validation");
            Check(typeof(AiKillControlDecision).GetConstructors(BindingFlags.Public | BindingFlags.Instance).Length == 0, "external caller can construct accepted Kill decision");

            var global = Request("kill:global", "owner:governed", AiKillIngress.ExternalOwner, AiEmergencyAction.GlobalAiKill, AiTargetKind.AllAi, AiKillControlPlaneContract.AllAiTargetId);
            var globalDecision = Evaluate(global, registry);
            Check(globalDecision.Accepted && globalDecision.Reason == AiKillControlReason.AcceptedGlobal, "global AI Kill was not accepted");
            Check(globalDecision.ImpactedTargetIds.SequenceEqual(new[] { "ai:app-alpha:csa:one", "ai:app-alpha:lsa", "ai:app-alpha:msa", "ai:app-beta:component", "fsa:primary" }), "global Kill did not contain exact executable AI census");
            Check(globalDecision.SafeCorePreserved && !globalDecision.FalconShutdownAuthorized, "GLOBAL_AI_KILL became Falcon shutdown");
            Check(AiKillControlPlaneContract.SafeCoreCapabilities.Contains("OWNER_CONTROL") && AiKillControlPlaneContract.SafeCoreCapabilities.Contains("AI_KILL_CONTROL") && AiKillControlPlaneContract.SafeCoreCapabilities.Contains("AUDIT_EVIDENCE") && AiKillControlPlaneContract.SafeCoreCapabilities.Contains("RECOVERY_INFRASTRUCTURE"), "Safe Core minimum set incomplete");

            var unknown = Request("kill:unknown", "owner:governed", AiKillIngress.WebOwner, AiEmergencyAction.Kill, AiTargetKind.Component, "ai:unknown");
            var unknownDecision = Evaluate(unknown, registry);
            Check(!unknownDecision.Accepted && unknownDecision.Reason == AiKillControlReason.TargetNotFound && unknownDecision.ImpactedTargetIds.Count == 0, "unknown target widened blast radius");

            var wrongKind = Request("kill:wrong-kind", "owner:governed", AiKillIngress.WebOwner, AiEmergencyAction.Kill, AiTargetKind.Csa, "ai:app-alpha:msa");
            Check(Evaluate(wrongKind, registry).Reason == AiKillControlReason.TargetNotFound, "target-kind mismatch was accepted");

            var invalidGlobal = Request("kill:invalid-global", "owner:governed", AiKillIngress.ExternalOwner, AiEmergencyAction.GlobalAiKill, AiTargetKind.Msa, "ai:app-alpha:msa");
            Check(Evaluate(invalidGlobal, registry).Reason == AiKillControlReason.InvalidGlobalRequest, "GLOBAL_AI_KILL accepted non-global target");
            var allAiWithKill = Request("kill:all-with-normal", "owner:governed", AiKillIngress.ExternalOwner, AiEmergencyAction.Kill, AiTargetKind.AllAi, AiKillControlPlaneContract.AllAiTargetId);
            Check(Evaluate(allAiWithKill, registry).Reason == AiKillControlReason.InvalidGlobalRequest, "ordinary Kill targeted ALL_AI");

            var fsaRequest = Request("kill:fsa-self", "fsa:primary", AiKillIngress.ExternalOwner, AiEmergencyAction.Kill, AiTargetKind.Fsa, "fsa:primary");
            Check(Evaluate(fsaRequest, registry).Reason == AiKillControlReason.AiActorProhibited, "FSA invoked its own Kill Control Plane");
            var appAiRequest = Request("kill:ai-actor", "ai:app-alpha:msa", AiKillIngress.WebOwner, AiEmergencyAction.Kill, AiTargetKind.Component, "ai:app-beta:component");
            Check(Evaluate(appAiRequest, registry).Reason == AiKillControlReason.AiActorProhibited, "Application AI invoked Kill Control Plane");

            var badAuthorityRequest = AuthorityRequestFor(target) with { Correlation = "correlation:wrong" };
            var badAuthority = AiKillControlRuntime.Evaluate(target, registry, badAuthorityRequest, AuthorityContextFor(target, badAuthorityRequest), Now);
            Check(!badAuthority.Accepted && badAuthority.Reason == AiKillControlReason.AuthorityBindingMismatch, "authority binding mismatch accepted");

            var deniedAuthorityRequest = AuthorityRequestFor(target);
            var deniedAuthority = AiKillControlRuntime.Evaluate(target, registry, deniedAuthorityRequest, AuthorityContextFor(target, deniedAuthorityRequest, "RESTRICT_AI"), Now);
            Check(!deniedAuthority.Accepted && deniedAuthority.Reason == AiKillControlReason.AuthorityNotGranted, "AUT-001 denial did not block Kill");

            var duplicateRegistry = registry.Concat(new[] { registry[0] with { RegistrationId = "registration:duplicate" } }).ToArray();
            Check(Evaluate(target, duplicateRegistry).Reason == AiKillControlReason.InvalidRegistry, "duplicate target identity was accepted");
            var missingParent = registry.Select(item => item.TargetId == "ai:app-alpha:csa:one" ? item with { ParentTargetId = "ai:missing-parent" } : item).ToArray();
            Check(Evaluate(target, missingParent).Reason == AiKillControlReason.InvalidRegistry, "missing parent was accepted");
            var cycle = registry.Select(item => item.TargetId == "ai:app-alpha:msa" ? item with { ParentTargetId = "ai:app-alpha:lsa" } : item).ToArray();
            Check(Evaluate(target, cycle).Reason == AiKillControlReason.InvalidRegistry, "registry cycle was accepted");
            var controlPlaneAsAi = registry.Append(Registration("registration:bad-control", AiKillControlPlaneContract.ControlPlaneId, AiTargetKind.Component, null, "foundation", true)).ToArray();
            Check(Evaluate(target, controlPlaneAsAi).Reason == AiKillControlReason.InvalidRegistry, "Kill Control Plane was registerable as target AI");

            var group = Request("kill:group", "owner:governed", AiKillIngress.ExternalOwner, AiEmergencyAction.Isolate, AiTargetKind.DefinedGroup, "ai-group:alpha");
            var groupDecision = Evaluate(group, registry);
            Check(groupDecision.Accepted && groupDecision.ImpactedTargetIds.SequenceEqual(new[] { "ai:app-alpha:csa:one", "ai:app-alpha:lsa", "ai:app-alpha:msa" }), "defined group did not resolve exact descendants");
            Check(!groupDecision.StopRequired && groupDecision.IsolationRequired, "Isolate semantics drifted into Kill");

            var restrict = Evaluate(Request("kill:restrict", "owner:governed", AiKillIngress.WebOwner, AiEmergencyAction.Restrict, AiTargetKind.Component, "ai:app-beta:component"), registry);
            Check(restrict.Accepted && !restrict.StopRequired && !restrict.IsolationRequired && restrict.AuthorityRevocationRequired, "Restrict action semantics drifted");
            var suspend = Evaluate(Request("kill:suspend", "owner:governed", AiKillIngress.WebOwner, AiEmergencyAction.Suspend, AiTargetKind.Component, "ai:app-beta:component"), registry);
            Check(suspend.Accepted && suspend.SuspensionRequired && !suspend.StopRequired, "Suspend semantics drifted");

            var enforcer = new AiKillControlAuthorityEnforcer();
            var killed = ExecutionRequest("exec:killed", "ai:app-alpha:csa:one", "EXECUTE", "resource:alpha", "application:alpha");
            var killedResult = enforcer.Evaluate(killed, ExecutionContext(killed), new[] { targetDecision });
            Check(killedResult.Decision == AuthorityDecision.Deny && killedResult.Reason == AiKillAuthorityReason.TargetContained, "killed AI retained authority");
            var restarted = killed with { RequestId = "exec:restarted", Correlation = "correlation:exec:restarted" };
            Check(enforcer.Evaluate(restarted, ExecutionContext(restarted), new[] { targetDecision }).Decision == AuthorityDecision.Deny, "restart restored killed AI authority");

            var safeCore = ExecutionRequest("exec:safe-core", "owner-control:primary", "CONTROL", "foundation:safe-core", "foundation:safe-core");
            Check(enforcer.Evaluate(safeCore, ExecutionContext(safeCore), new[] { globalDecision }).Decision == AuthorityDecision.Allow, "global AI Kill stopped non-AI Safe Core authority");
            var fsaExecution = ExecutionRequest("exec:fsa", "fsa:primary", "REVIEW", "foundation:fsa", "foundation:fsa");
            Check(enforcer.Evaluate(fsaExecution, ExecutionContext(fsaExecution), new[] { globalDecision }).Decision == AuthorityDecision.Deny, "global AI Kill did not contain FSA");
            Check(enforcer.Evaluate(safeCore, ExecutionContext(safeCore), Array.Empty<AiKillControlDecision>()).Decision == AuthorityDecision.Allow, "empty Kill evidence changed baseline authority");
            Check(enforcer.Evaluate(safeCore, ExecutionContext(safeCore), null).Reason == AiKillAuthorityReason.EvidenceUnavailable, "missing Kill evidence did not fail closed");

            var afterDeadline = ExecutionRequest("exec:after-deadline", "ai:app-alpha:msa", "EXECUTE", "resource:alpha", "application:alpha");
            Check(enforcer.Evaluate(afterDeadline, ExecutionContext(afterDeadline, targetDecision.ReviewDeadline.AddHours(1)), new[] { targetDecision }).Decision == AuthorityDecision.Deny, "review deadline released containment");

            Check(AiKillControlRuntime.ToAuthorityAction(AiEmergencyAction.Restrict) == "RESTRICT_AI" && AiKillControlRuntime.ToAuthorityAction(AiEmergencyAction.Suspend) == "SUSPEND_AI" && AiKillControlRuntime.ToAuthorityAction(AiEmergencyAction.Isolate) == "ISOLATE_AI" && AiKillControlRuntime.ToAuthorityAction(AiEmergencyAction.Kill) == "KILL_AI" && AiKillControlRuntime.ToAuthorityAction(AiEmergencyAction.GlobalAiKill) == "GLOBAL_AI_KILL", "AI emergency action vocabulary drifted");
            var publicMethods = typeof(AiKillControlRuntime).GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
            Check(!publicMethods.Any(method => method.Name.Contains("Release", StringComparison.OrdinalIgnoreCase) || method.Name.Contains("Recover", StringComparison.OrdinalIgnoreCase) || method.Name.Contains("RestoreTrust", StringComparison.OrdinalIgnoreCase) || method.Name.Contains("Revival", StringComparison.OrdinalIgnoreCase)), "release/recovery execution leaked into Kill Control Plane");
            Check(!typeof(AiKillControlRuntime).Assembly.GetExportedTypes().Select(type => type.Name).Any(name => name.Contains("Trade", StringComparison.OrdinalIgnoreCase) || name.Contains("Strategy", StringComparison.OrdinalIgnoreCase) || name.Contains("Portfolio", StringComparison.OrdinalIgnoreCase) || name.Contains("Broker", StringComparison.OrdinalIgnoreCase) || name.Contains("Market", StringComparison.OrdinalIgnoreCase)), "Application business semantics leaked into Foundation.Authority");
            Check(!typeof(AiKillControlRuntime).Assembly.GetReferencedAssemblies().Any(name => string.Equals(name.Name, "Foundation.SelfAwareness", StringComparison.Ordinal)), "Kill Control Plane depends on FSA/SelfAwareness runtime");
            Check(!typeof(AiKillControlRuntime).Assembly.GetReferencedAssemblies().Any(name => string.Equals(name.Name, "Foundation.Guardian", StringComparison.Ordinal)), "Kill Control Plane depends on Guardian runtime");

            var web = Evaluate(Request("kill:web-ingress", "owner:governed", AiKillIngress.WebOwner, AiEmergencyAction.Kill, AiTargetKind.Component, "ai:app-beta:component"), registry);
            var external = Evaluate(Request("kill:external-ingress", "owner:governed", AiKillIngress.ExternalOwner, AiEmergencyAction.Kill, AiTargetKind.Component, "ai:app-beta:component"), registry);
            Check(web.Accepted && external.Accepted && web.TargetId == external.TargetId, "dual ingress did not converge on one control plane semantics");
            Check(web.Ingress != external.Ingress, "independent ingress identity was lost");

            var emptyGlobal = Request("kill:empty-global", "owner:governed", AiKillIngress.ExternalOwner, AiEmergencyAction.GlobalAiKill, AiTargetKind.AllAi, AiKillControlPlaneContract.AllAiTargetId);
            Check(Evaluate(emptyGlobal, Array.Empty<AiTargetRegistration>()).Reason == AiKillControlReason.NoImpactedAi, "zero-AI global Kill fabricated success");

            if (_checks != 43)
                throw new InvalidOperationException($"Unexpected check count: {_checks}, expected 43.");

            Console.WriteLine("STAGE13_WP01_AI_KILL_CONTROL_VERIFIER = PASS");
            Console.WriteLine("CHECKS = 43/43");
            Console.WriteLine("FSA_CONTROL_PLANE_ACCESS = DENIED");
            Console.WriteLine("AMBIGUOUS_OR_UNKNOWN_TARGET = FAIL_CLOSED_NO_WIDEN");
            Console.WriteLine("TARGETED_KILL = EXACT_HIERARCHY");
            Console.WriteLine("GLOBAL_AI_KILL = ALL_REGISTERED_AI");
            Console.WriteLine("GLOBAL_AI_KILL != FALCON_SHUTDOWN");
            Console.WriteLine("FALCON_SAFE_CORE = PRESERVED");
            Console.WriteLine("AI_RESTART != AUTHORITY_RESTORATION");
            Console.WriteLine("TARGET_AI_COOPERATION_NOT_REQUIRED = PASS");
            Console.WriteLine("WEB_UI != KILL_AUTHORITY");
            Console.WriteLine("CONTROL_PLANE_RELEASE_API = ABSENT");
            Console.WriteLine("ZERO_APPLICATION_OPERATION = VALID");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("STAGE13_WP01_AI_KILL_CONTROL_VERIFIER = FAIL");
            Console.Error.WriteLine(ex.Message);
            return 1;
        }
    }

    private static AiTargetRegistration[] Registry() => new[]
    {
        Registration("registration:group-alpha", "ai-group:alpha", AiTargetKind.DefinedGroup, null, "application:alpha", false),
        Registration("registration:msa-alpha", "ai:app-alpha:msa", AiTargetKind.Msa, "ai-group:alpha", "application:alpha", true),
        Registration("registration:lsa-alpha", "ai:app-alpha:lsa", AiTargetKind.Lsa, "ai:app-alpha:msa", "application:alpha", true),
        Registration("registration:csa-alpha", "ai:app-alpha:csa:one", AiTargetKind.Csa, "ai:app-alpha:lsa", "application:alpha", true),
        Registration("registration:component-beta", "ai:app-beta:component", AiTargetKind.Component, null, "application:beta", true),
        Registration("registration:fsa", "fsa:primary", AiTargetKind.Fsa, null, "foundation:fsa", true)
    };

    private static AiTargetRegistration Registration(string id, string target, AiTargetKind kind, string? parent, string scope, bool executable) =>
        new(id, target, kind, parent, scope, executable, "evidence:" + id, Now.AddHours(-1), Now.AddHours(4));

    private static AiKillRequest Request(string id, string actor, AiKillIngress ingress, AiEmergencyAction action, AiTargetKind kind, string target) =>
        new(id, actor, ingress, action, kind, target, "correlation:" + id, Now.AddMinutes(-5), Now.AddHours(1));

    private static AiKillControlDecision Evaluate(AiKillRequest request, IReadOnlyCollection<AiTargetRegistration> registry)
    {
        var authority = AuthorityRequestFor(request);
        return AiKillControlRuntime.Evaluate(request, registry, authority, AuthorityContextFor(request, authority), Now);
    }

    private static AuthorityRequest AuthorityRequestFor(AiKillRequest request) => new(
        "authority:" + request.RequestId,
        request.ActorIdentity,
        AiKillControlRuntime.ToAuthorityAction(request.Action),
        request.TargetId,
        AiKillControlPlaneContract.Purpose,
        request.TargetId,
        "EMERGENCY_CONTROL",
        "TRUSTED",
        "FIT",
        request.Correlation,
        request.RequestTime,
        request.Expiry);

    private static AuthorityEvaluationContext AuthorityContextFor(AiKillRequest request, AuthorityRequest authority, string? allowedActionOverride = null) => new(
        new AuthorityPolicy(
            "policy:" + request.RequestId, "1.0", "owner:governed", Now.AddHours(-1), Now.AddHours(2),
            new[] { request.ActorIdentity }, new[] { allowedActionOverride ?? authority.Action }, new[] { request.TargetId },
            new[] { AiKillControlPlaneContract.Purpose }, new[] { request.TargetId }, new[] { "TRUSTED" }),
        new DelegationEvidence("delegation:" + request.RequestId, request.ActorIdentity, "owner:governed", new[] { request.TargetId }, Now.AddHours(-1), Now.AddHours(2), false),
        new FitnessEvidence(request.ActorIdentity, "FIT", true, Now.AddMinutes(-10), Now.AddHours(2), "fitness:" + request.RequestId),
        Now,
        "authority-evidence:" + request.RequestId);

    private static AuthorityRequest ExecutionRequest(string id, string actor, string action, string resource, string scope) => new(
        id, actor, action, resource, "EXECUTION", scope, "LIVE", "TRUSTED", "FIT", "correlation:" + id, Now.AddMinutes(-1), Now.AddDays(2));

    private static AuthorityEvaluationContext ExecutionContext(AuthorityRequest request, DateTimeOffset? observation = null)
    {
        var time = observation ?? Now;
        return new AuthorityEvaluationContext(
            new AuthorityPolicy("policy:" + request.RequestId, "1.0", "owner:governed", time.AddHours(-1), time.AddDays(2),
                new[] { request.ActorIdentity }, new[] { request.Action }, new[] { request.Resource }, new[] { request.Purpose }, new[] { request.RequestedScope }, new[] { "TRUSTED" }),
            new DelegationEvidence("delegation:" + request.RequestId, request.ActorIdentity, "owner:governed", new[] { request.RequestedScope }, time.AddHours(-1), time.AddDays(2), false),
            new FitnessEvidence(request.ActorIdentity, "FIT", true, time.AddMinutes(-10), time.AddDays(2), "fitness:" + request.RequestId),
            time,
            "authority-evidence:" + request.RequestId);
    }

    private static void Check(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException("CHECK FAILED: " + message);
        _checks++;
    }
}
