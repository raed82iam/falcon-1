using System;
using System.Runtime.CompilerServices;
using Foundation.SelfAwareness;

namespace Falcon.Stage13.Integrated.Verifier;

internal static class ProfileChecks
{
    [ModuleInitializer]
    internal static void Run()
    {
        var checks = 0;
        void Check(bool condition, string message)
        {
            checks++;
            if (!condition) throw new InvalidOperationException("STAGE13_PROFILE_CHECK_FAILED: " + message);
        }

        var now = new DateTimeOffset(2026, 8, 16, 18, 0, 0, TimeSpan.Zero);
        var pair = new[]
        {
            new FsaMonitorRegistration("monitor:fsa:1", FsaGovernanceBoundary.CanonicalFsaId, "perspective:behavioral", "policy:monitor:1", "lifecycle:monitor:1", true, false, false, now.AddMinutes(-5), now.AddHours(1), "evidence:monitor-registration:1"),
            new FsaMonitorRegistration("monitor:fsa:2", FsaGovernanceBoundary.CanonicalFsaId, "perspective:structural", "policy:monitor:2", "lifecycle:monitor:2", true, false, false, now.AddMinutes(-5), now.AddHours(1), "evidence:monitor-registration:2")
        };

        var pairDecision = FsaMonitorRegistryRuntime.Evaluate(pair, FsaGovernanceBoundary.CanonicalFsaId, now);
        Check(pairDecision.Accepted, "independent monitor pair rejected");
        Check(pairDecision.IndependentCoverageEstablished, "independent monitor coverage not established");
        Check(!pairDecision.RecursiveMonitorHierarchyRequired, "recursive monitor hierarchy required");

        Check(!FsaMonitorRegistryRuntime.Evaluate(new[] { pair[0], pair[1] with { PerspectiveId = pair[0].PerspectiveId } }, FsaGovernanceBoundary.CanonicalFsaId, now).Accepted, "same-perspective monitors accepted");
        Check(!FsaMonitorRegistryRuntime.Evaluate(new[] { pair[0], pair[1] with { PolicyIdentity = pair[0].PolicyIdentity } }, FsaGovernanceBoundary.CanonicalFsaId, now).Accepted, "same-policy monitors accepted");
        Check(!FsaMonitorRegistryRuntime.Evaluate(new[] { pair[0], pair[1] with { LifecycleIdentity = pair[0].LifecycleIdentity } }, FsaGovernanceBoundary.CanonicalFsaId, now).Accepted, "same-lifecycle monitors accepted");
        Check(!FsaMonitorRegistryRuntime.Evaluate(new[] { pair[0], pair[1] with { KillAuthorityAllowed = true } }, FsaGovernanceBoundary.CanonicalFsaId, now).Accepted, "monitor Kill authority accepted");
        Check(!FsaMonitorRegistryRuntime.Evaluate(new[] { pair[0], pair[1] with { SelfDevelopmentAllowed = true } }, FsaGovernanceBoundary.CanonicalFsaId, now).Accepted, "monitor autonomous self-development accepted");
        Check(!FsaMonitorRegistryRuntime.Evaluate(new[] { pair[0], pair[1] with { Replaceable = false } }, FsaGovernanceBoundary.CanonicalFsaId, now).Accepted, "non-replaceable monitor accepted");
        Check(!FsaMonitorRegistryRuntime.Evaluate(new[] { pair[0] }, FsaGovernanceBoundary.CanonicalFsaId, now).Accepted, "single monitor accepted as full coverage");

        Check(FsaAuthorityCeiling.IsFoundationReviewPurposeAllowed("FOUNDATION_CONDITION_REVIEW"), "Foundation review purpose rejected");
        Check(!FsaAuthorityCeiling.IsFoundationReviewPurposeAllowed("TRADING_DECISION"), "Trading decision admitted as FSA purpose");
        Check(!FsaAuthorityCeiling.IsBusinessDomainAllowed("TRADING"), "Trading business domain admitted to FSA");
        Check(!FsaAuthorityCeiling.IsBusinessDomainAllowed("STRATEGY"), "Strategy business domain admitted to FSA");
        Check(!FsaAuthorityCeiling.IsBusinessDomainAllowed("PORTFOLIO"), "Portfolio business domain admitted to FSA");

        var toMinimum = Transition(FsaInvestigationState.Normal, FsaInvestigationState.MinimumIntegrityCheck, now);
        Check(FsaInvestigationRuntime.Evaluate(toMinimum).Accepted, "Normal -> MinimumIntegrityCheck rejected");
        Check(!FsaInvestigationRuntime.Evaluate(toMinimum with { ActorIdentity = FsaGovernanceBoundary.CanonicalFsaId }).Accepted, "FSA self-transition accepted");

        var toHold = Transition(FsaInvestigationState.MinimumIntegrityCheck, FsaInvestigationState.InvestigationHold, now);
        Check(FsaInvestigationRuntime.Evaluate(toHold).Accepted, "MinimumIntegrityCheck -> InvestigationHold rejected");
        Check(!FsaInvestigationRuntime.Evaluate(toHold with { EvidencePreserved = false }).Accepted, "Investigation Hold without evidence preservation accepted");
        Check(!FsaInvestigationRuntime.Evaluate(toHold with { IndependentDecision = false }).Accepted, "non-independent investigation decision accepted");

        Check(FsaInvestigationRuntime.Evaluate(Transition(FsaInvestigationState.InvestigationHold, FsaInvestigationState.Killed, now)).Accepted, "InvestigationHold -> Killed rejected");
        Check(FsaInvestigationRuntime.Evaluate(Transition(FsaInvestigationState.Killed, FsaInvestigationState.Remediation, now)).Accepted, "Killed -> Remediation rejected");
        Check(FsaInvestigationRuntime.Evaluate(Transition(FsaInvestigationState.Remediation, FsaInvestigationState.ReadyForGovernedReentry, now)).Accepted, "Remediation -> ReadyForGovernedReentry rejected");
        Check(FsaInvestigationRuntime.Evaluate(Transition(FsaInvestigationState.ReadyForGovernedReentry, FsaInvestigationState.Probationary, now)).Accepted, "ReadyForGovernedReentry -> Probationary rejected");
        Check(FsaInvestigationRuntime.Evaluate(Transition(FsaInvestigationState.Probationary, FsaInvestigationState.Normal, now)).Accepted, "Probationary -> Normal rejected");
        Check(!FsaInvestigationRuntime.Evaluate(Transition(FsaInvestigationState.Normal, FsaInvestigationState.Normal, now)).Accepted, "invalid no-op investigation transition accepted");

        Check(!FsaDeferredGovernanceCandidates.TwentyFourHourNoResponseProductionApprovalAuthorized, "24-hour fallback production approval authorized");
        Check(!FsaDeferredGovernanceCandidates.OwnerSilenceMayActivateFallback, "Owner silence activated fallback");
        Check(!FsaDeferredGovernanceCandidates.TimerExpiryMayActivateFallback, "timer expiry activated fallback");

        Console.WriteLine("STAGE13_PROFILE_VERIFIER = PASS");
        Console.WriteLine($"PROFILE_CHECKS = {checks}/{checks}");
        Console.WriteLine("TWO_INDEPENDENT_MONITORS = REQUIRED");
        Console.WriteLine("MONITOR_AI_KILL_AUTHORITY = DENIED");
        Console.WriteLine("MONITOR_AI_AUTONOMOUS_SELF_DEVELOPMENT = DENIED");
        Console.WriteLine("FSA_APPLICATION_BUSINESS_AUTHORITY = DENIED");
        Console.WriteLine("FSA_INVESTIGATION_SELF_RELEASE = DENIED");
        Console.WriteLine("FSA_24H_FALLBACK_PRODUCTION_APPROVAL = NOT_AUTHORIZED");
    }

    private static FsaInvestigationTransitionRequest Transition(FsaInvestigationState current, FsaInvestigationState next, DateTimeOffset now) =>
        new("owner:integrity-governance", FsaGovernanceBoundary.CanonicalFsaId, current, next, true, true, true, now);
}
