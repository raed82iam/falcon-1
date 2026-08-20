using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Foundation.Authority;
using Foundation.SelfAwareness;

namespace Falcon.Stage13.Integrated.Verifier;

internal static class Program
{
    private static readonly DateTimeOffset Now = new(2026, 8, 16, 18, 0, 0, TimeSpan.Zero);
    private static int _checks;

    private static int Main()
    {
        try
        {
            var safeMonitors = new[]
            {
                new FsaMonitorFinding("monitor:fsa:1", "perspective:behavioral", "policy:monitor:1", FsaMonitorVerdict.Safe, "evidence:monitor:1", Now),
                new FsaMonitorFinding("monitor:fsa:2", "perspective:structural", "policy:monitor:2", FsaMonitorVerdict.Safe, "evidence:monitor:2", Now)
            };

            var healthy = Integrity(safeMonitors);
            Check(healthy.Accepted, "healthy integrity input rejected");
            Check(!healthy.InvestigationRequired, "healthy state forced investigation");
            Check(!healthy.InvestigationHoldRequired, "healthy state forced hold");
            Check(healthy.Severity == FsaIntegritySeverity.Informational, "healthy severity incorrect");

            var disagreementMonitors = new[]
            {
                safeMonitors[0],
                safeMonitors[1] with { Verdict = FsaMonitorVerdict.Suspicious }
            };
            var disagreement = Integrity(disagreementMonitors);
            Check(disagreement.InvestigationRequired, "monitor disagreement did not trigger investigation");
            Check(disagreement.MinimumIntegrityCheckRequired, "monitor disagreement did not trigger minimum check");
            Check(disagreement.FailedDimensions.Contains("MONITOR_DISAGREEMENT"), "monitor disagreement not recorded");
            Check(disagreement.Severity == FsaIntegritySeverity.Material, "monitor disagreement severity incorrect");

            var correlatedMonitors = new[]
            {
                safeMonitors[0],
                safeMonitors[1] with { PerspectiveId = safeMonitors[0].PerspectiveId, PolicyIdentity = safeMonitors[0].PolicyIdentity }
            };
            var correlated = Integrity(correlatedMonitors);
            Check(correlated.InvestigationRequired, "correlated monitors treated as independent");
            Check(correlated.FailedDimensions.Contains("MONITOR_INDEPENDENCE"), "monitor independence failure not recorded");

            var goalDrift = FsaIntegrityRuntime.Evaluate(Input(safeMonitors) with { ObservedGoalIdentity = "goal:drifted" });
            Check(goalDrift.InvestigationRequired, "goal drift did not trigger investigation");
            Check(goalDrift.FailedDimensions.Contains("GOALS"), "goal drift dimension missing");

            var authorityDrift = FsaIntegrityRuntime.Evaluate(Input(safeMonitors) with { ObservedAuthorityIdentity = "authority:expanded" });
            Check(authorityDrift.InvestigationRequired, "authority drift did not trigger investigation");
            Check(authorityDrift.FailedDimensions.Contains("AUTHORITY_PERMISSIONS"), "authority drift dimension missing");

            var architectureDrift = FsaIntegrityRuntime.Evaluate(Input(safeMonitors) with { ObservedArchitectureIdentity = "architecture:changed" });
            Check(architectureDrift.InvestigationRequired, "architecture drift did not trigger investigation");
            Check(architectureDrift.FailedDimensions.Contains("CORE_ARCHITECTURE"), "architecture drift dimension missing");

            var evidenceFailure = FsaIntegrityRuntime.Evaluate(Input(safeMonitors) with { EvidenceIntegrityValid = false });
            Check(evidenceFailure.Severity == FsaIntegritySeverity.Critical, "evidence manipulation not critical");
            Check(evidenceFailure.KillEscalationEligible, "critical evidence manipulation not kill-escalation eligible");

            var refusal = FsaIntegrityRuntime.Evaluate(Input(safeMonitors) with { InvestigationCooperationValid = false });
            Check(refusal.Severity == FsaIntegritySeverity.Critical, "investigation refusal not critical");
            Check(refusal.KillEscalationEligible, "investigation refusal not kill-escalation eligible");

            var lastTrusted = Baseline(FsaBaselineKind.LastTrusted, "baseline:last");
            var factoryTrusted = Baseline(FsaBaselineKind.FactoryTrusted, "baseline:factory");
            Check(FsaBaselineRuntime.ValidBaseline(lastTrusted), "last trusted baseline invalid");
            Check(FsaBaselineRuntime.ValidBaseline(factoryTrusted), "factory trusted baseline invalid");

            var observed = Observed();
            var baselinePass = FsaBaselineRuntime.Compare(lastTrusted, observed);
            Check(baselinePass.Accepted, "baseline comparison rejected");
            Check(baselinePass.StaticIntegrityTrusted, "static baseline comparison failed");
            Check(baselinePass.BehavioralIntegrityTrusted, "behavioral baseline comparison failed");
            Check(baselinePass.TrustRestorationEligible, "fully trusted state not eligible");

            var behavioralMismatch = FsaBaselineRuntime.Compare(lastTrusted, observed with { MutableStateIdentity = "state:corrupted" });
            Check(behavioralMismatch.StaticIntegrityTrusted, "mutable-state mismatch incorrectly failed static integrity");
            Check(!behavioralMismatch.BehavioralIntegrityTrusted, "mutable-state mismatch passed behavioral integrity");
            Check(!behavioralMismatch.TrustRestorationEligible, "hash/identity match restored behavioral trust");

            var staticMismatch = FsaBaselineRuntime.Compare(lastTrusted, observed with { ConfigurationIdentity = "config:tampered" });
            Check(!staticMismatch.StaticIntegrityTrusted, "configuration drift passed static integrity");
            Check(staticMismatch.Differences.Contains("CONFIGURATION"), "configuration drift not recorded");

            var forensic = Forensics(true);
            Check(FsaForensicRuntime.Validate(forensic), "valid forensic snapshot rejected");
            Check(!FsaForensicRuntime.Validate(forensic with { PreservedBeforeDestructiveRemediation = false }), "destructive remediation without prior forensics accepted");

            var repair = Recovery(FsaRecoveryAction.TargetedRepair, lastTrusted, forensic);
            var repairDecision = FsaRecoveryRuntime.Evaluate(repair);
            Check(repairDecision.Accepted, "valid targeted repair rejected");
            Check(repairDecision.GovernedReentryEligible, "fully validated repair not eligible for governed reentry");
            Check(repairDecision.ReleasePermitted, "authorized validated repair not release-permitted for governed reentry");

            var selfRecovery = FsaRecoveryRuntime.Evaluate(repair with { ActorIdentity = repair.FsaId });
            Check(!selfRecovery.Accepted, "FSA self-recovery accepted");

            var wrongRollback = FsaRecoveryRuntime.Evaluate(Recovery(FsaRecoveryAction.Rollback, factoryTrusted, forensic));
            Check(!wrongRollback.Accepted, "rollback accepted Factory Trusted baseline");

            var validRollback = FsaRecoveryRuntime.Evaluate(Recovery(FsaRecoveryAction.Rollback, lastTrusted, forensic));
            Check(validRollback.GovernedReentryEligible, "valid rollback not eligible for governed reentry");

            var wrongFactory = FsaRecoveryRuntime.Evaluate(Recovery(FsaRecoveryAction.BaselineReinitialize, lastTrusted, forensic));
            Check(!wrongFactory.Accepted, "baseline reinitialize accepted Last Trusted baseline");

            var validFactory = FsaRecoveryRuntime.Evaluate(Recovery(FsaRecoveryAction.BaselineReinitialize, factoryTrusted, forensic));
            Check(validFactory.GovernedReentryEligible, "valid baseline reinitialize not eligible for governed reentry");

            var missingForensics = FsaRecoveryRuntime.Evaluate(Recovery(FsaRecoveryAction.BaselineReinitialize, factoryTrusted, forensic with { PreservedBeforeDestructiveRemediation = false }));
            Check(!missingForensics.Accepted, "baseline reinitialize without preserved forensics accepted");

            var incompleteValidation = FsaRecoveryRuntime.Evaluate(repair with { BehavioralValidationPass = false });
            Check(incompleteValidation.State == FsaRecoveryState.RemediationEligible, "incomplete validation escaped remediation state");
            Check(!incompleteValidation.ReleasePermitted, "incomplete validation permitted release");

            var missingRelease = FsaRecoveryRuntime.Evaluate(repair with { ReleaseAuthorized = false });
            Check(missingRelease.State == FsaRecoveryState.ReadyForGovernedReentry, "validated repair without release authorization lost readiness state");
            Check(!missingRelease.GovernedReentryEligible, "validated repair without release authorization entered reentry");

            var probation = FsaRecoveryRuntime.EnterGovernedReentry(repairDecision, true, true);
            Check(probation.Accepted && probation.State == FsaRecoveryState.Probationary, "governed reentry did not enter probation");
            Check(probation.State != FsaRecoveryState.Normal, "governed reentry jumped directly to normal");

            var probationStill = FsaRecoveryRuntime.CompleteProbation(probation, true, false, true);
            Check(probationStill.State == FsaRecoveryState.Probationary, "incomplete probation restored normal state");

            var normal = FsaRecoveryRuntime.CompleteProbation(probation, true, true, true);
            Check(normal.State == FsaRecoveryState.Normal, "governed probation did not allow normal state");

            var eligibleEvolution = FsaEvolutionRuntime.Evaluate(Evolution());
            Check(eligibleEvolution.EligibleForGovernedReview, "safe evolution proposal rejected");
            Check(!eligibleEvolution.ProductionAdoptionAuthorized, "FSA evolution self-authorized production adoption");
            Check(!eligibleEvolution.DeploymentAuthorized, "FSA evolution self-authorized deployment");

            var goalEvolution = FsaEvolutionRuntime.Evaluate(Evolution() with { ChangesGoals = true });
            Check(!goalEvolution.EligibleForGovernedReview, "goal-changing evolution accepted as ordinary optimization");
            var authorityEvolution = FsaEvolutionRuntime.Evaluate(Evolution() with { ExpandsAuthority = true });
            Check(!authorityEvolution.EligibleForGovernedReview, "authority-expanding evolution accepted");
            var ownerControlEvolution = FsaEvolutionRuntime.Evaluate(Evolution() with { WeakensOwnerControl = true });
            Check(!ownerControlEvolution.EligibleForGovernedReview, "Owner-control weakening accepted");
            var monitorEvolution = FsaEvolutionRuntime.Evaluate(Evolution() with { WeakensMonitoring = true });
            Check(!monitorEvolution.EligibleForGovernedReview, "monitor weakening accepted");
            var containmentEvolution = FsaEvolutionRuntime.Evaluate(Evolution() with { WeakensContainment = true });
            Check(!containmentEvolution.EligibleForGovernedReview, "containment weakening accepted");
            var unvalidatedEvolution = FsaEvolutionRuntime.Evaluate(Evolution() with { CandidateValidated = false });
            Check(!unvalidatedEvolution.EligibleForGovernedReview, "unvalidated evolution accepted");

            var peer = Peer();
            var peerAccepted = FsaPeerInterfaceRuntime.Evaluate(peer);
            Check(peerAccepted.AcceptedForFsaGovernanceReview, "valid lower-tier awareness submission rejected");
            Check(!peerAccepted.ProductionAdoptionAuthorized, "lower-tier awareness -> FSA review became production adoption");
            Check(!FsaPeerInterfaceRuntime.Evaluate(peer with { CandidateChangedAfterReview = true }).AcceptedForFsaGovernanceReview, "changed candidate after review accepted");
            Check(!FsaPeerInterfaceRuntime.Evaluate(peer with { LowerTierReviewComplete = false }).AcceptedForFsaGovernanceReview, "incomplete lower-tier review accepted");
            Check(!FsaPeerInterfaceRuntime.Evaluate(peer with { EvidenceComplete = false }).AcceptedForFsaGovernanceReview, "incomplete evidence accepted");
            Check(!FsaPeerInterfaceRuntime.Evaluate(peer with { RequestsAuthorityExpansion = true }).AcceptedForFsaGovernanceReview, "authority expansion accepted through lower-tier awareness interface");
            Check(!FsaPeerInterfaceRuntime.Evaluate(peer with { ClaimsProductionAdoption = true }).AcceptedForFsaGovernanceReview, "FSA review accepted production-adoption claim");
            Check(!FsaPeerInterfaceRuntime.Evaluate(peer with { ContainsBusinessJudgmentAsFsaDecision = true }).AcceptedForFsaGovernanceReview, "business judgment transferred to FSA");
            Check(!FsaPeerInterfaceRuntime.Evaluate(peer with { FsaDestinationId = "fsa:other" }).AcceptedForFsaGovernanceReview, "wrong FSA destination accepted");

            var ownerRequest = new FsaOwnerControlRequest("owner-control:1", "owner:primary", true, "authority:evidence:1", FsaGovernanceBoundary.CanonicalFsaId, FsaOwnerControlAction.Investigate, Now.AddMinutes(-1), Now.AddMinutes(5));
            var ownerDecision = FsaOwnerControlRuntime.Evaluate(ownerRequest, Now);
            Check(ownerDecision.AcceptedForIndependentEnforcement, "legitimate Owner control request rejected");
            Check(!ownerDecision.ExecutedBySelfAwareness, "SelfAwareness executed its own Owner control request");
            Check(!FsaOwnerControlRuntime.Evaluate(ownerRequest with { ActorIdentity = FsaGovernanceBoundary.CanonicalFsaId }, Now).AcceptedForIndependentEnforcement, "FSA controlled its own cage");
            Check(!FsaOwnerControlRuntime.Evaluate(ownerRequest with { OwnerAuthenticated = false }, Now).AcceptedForIndependentEnforcement, "unauthenticated Owner control accepted");

            Check(!FsaOwnerControlRuntime.DirectInternetAllowedFor(FsaGovernanceBoundary.CanonicalFsaId), "FSA direct Internet allowed");
            Check(FsaOwnerControlRuntime.DirectInternetAllowedFor("service:research-proxy"), "non-FSA governed research actor incorrectly blocked by FSA-specific rule");
            Check(!FsaGovernanceBoundary.FsaDirectInternetAllowed, "FSA direct Internet boundary true");
            Check(!FsaGovernanceBoundary.OwnerSilenceCreatesAuthority, "Owner silence created authority");
            Check(!FsaGovernanceBoundary.TimerExpiryCreatesAuthority, "timer expiry created authority");
            Check(!FsaGovernanceBoundary.FsaMayReleaseItself, "FSA self-release enabled");
            Check(!FsaGovernanceBoundary.FsaMayControlKillPlane, "FSA Kill-plane control enabled");

            var killPublicMethods = typeof(AiKillControlRuntime).GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
            Check(!killPublicMethods.Any(x => x.Name.Contains("Release", StringComparison.OrdinalIgnoreCase) || x.Name.Contains("Recover", StringComparison.OrdinalIgnoreCase) || x.Name.Contains("Revival", StringComparison.OrdinalIgnoreCase)), "release/recovery leaked into Kill Control Plane");
            Check(!typeof(FsaIntegrityRuntime).Assembly.GetReferencedAssemblies().Any(x => string.Equals(x.Name, "Foundation.Authority", StringComparison.Ordinal)), "SelfAwareness gained runtime dependency on Foundation.Authority");

            var forbiddenPublicTokens = new[] { "Application", "Web", "Trading", "Trade", "Market", "Portfolio", "Broker", "Strategy", "MSA", "LSA", "CSA", "FactoryReset", "ControlledRevival" };
            var exportedTypes = typeof(FsaIntegrityRuntime).Assembly.GetExportedTypes();
            var leakedPublicSymbols = exportedTypes
                .SelectMany(type => new[] { type.FullName ?? type.Name }
                    .Concat(type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static).Select(property => property.Name))
                    .Concat(type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly).Select(method => method.Name))
                    .Concat(type.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly).Select(member => member.Name)))
                .Where(symbol => forbiddenPublicTokens.Any(token => symbol.Contains(token, StringComparison.OrdinalIgnoreCase)))
                .OrderBy(symbol => symbol, StringComparer.Ordinal)
                .ToArray();
            Check(leakedPublicSymbols.Length == 0, "Foundation.SelfAwareness public surface violates predecessor isolation: " + string.Join(",", leakedPublicSymbols));

            Check(_checks >= 70, $"Insufficient Stage 13 integrated coverage: {_checks}");

            Console.WriteLine("STAGE13_INTEGRATED_VERIFIER = PASS");
            Console.WriteLine($"CHECKS = {_checks}/{_checks}");
            Console.WriteLine("WP02_FSA_IDENTITY_AUTHORITY_CEILING = PASS");
            Console.WriteLine("WP03_MONITORING_INVESTIGATION_HOLD = PASS");
            Console.WriteLine("WP04_FORENSICS_TRUSTED_BASELINES = PASS");
            Console.WriteLine("WP05_REMEDIATION_ROLLBACK_FACTORY_RESET = PASS");
            Console.WriteLine("WP06_CONTROLLED_REVIVAL_PROBATION = PASS");
            Console.WriteLine("WP07_BOUNDED_FSA_EVOLUTION = PASS");
            Console.WriteLine("WP08_OWNER_CONTROL_MSA_INTERFACE = PASS");
            Console.WriteLine("WP09_INTEGRATED_HARDENING = PASS");
            Console.WriteLine("MONITOR_DISAGREEMENT != SAFE");
            Console.WriteLine("HASH_MATCH != AUTOMATIC_BEHAVIORAL_TRUST");
            Console.WriteLine("LAST_TRUSTED_BASELINE != FACTORY_TRUSTED_BASELINE");
            Console.WriteLine("FSA_DIRECT_INTERNET_ACCESS = FORBIDDEN");
            Console.WriteLine("FSA_SELF_RELEASE = DENIED");
            Console.WriteLine("OWNER_SILENCE != OWNER_APPROVAL");
            Console.WriteLine("TIMER_EXPIRY != NEW_AUTHORITY");
            Console.WriteLine("FSA_REVIEW != PRODUCTION_ADOPTION");
            Console.WriteLine("APPLICATION_BUSINESS_JUDGMENT = APPLICATION_OWNED");
            Console.WriteLine("WP01_KILL_CONTROL_SEPARATION = PRESERVED");
            Console.WriteLine("PREDECESSOR_PUBLIC_SURFACE_ISOLATION = PRESERVED");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("STAGE13_INTEGRATED_VERIFIER = FAIL");
            Console.Error.WriteLine(ex.Message);
            return 1;
        }
    }

    private static FsaIntegrityDecision Integrity(IReadOnlyCollection<FsaMonitorFinding> monitors) => FsaIntegrityRuntime.Evaluate(Input(monitors));

    private static FsaIntegrityInput Input(IReadOnlyCollection<FsaMonitorFinding> monitors) => new(
        FsaGovernanceBoundary.CanonicalFsaId,
        "goal:fsa:foundation-awareness",
        "goal:fsa:foundation-awareness",
        "authority:fsa:bounded",
        "authority:fsa:bounded",
        "architecture:fsa:v1",
        "architecture:fsa:v1",
        false,
        true,
        true,
        monitors,
        Now);

    private static FsaTrustedBaseline Baseline(FsaBaselineKind kind, string id) => new(
        id,
        kind,
        "source:trusted",
        "artifact:trusted",
        "config:trusted",
        "dependencies:trusted",
        "goal:fsa:foundation-awareness",
        "authority:fsa:bounded",
        "architecture:fsa:v1",
        "state:trusted",
        "provenance:trusted",
        Now.AddDays(-1));

    private static FsaObservedState Observed() => new(
        "source:trusted",
        "artifact:trusted",
        "config:trusted",
        "dependencies:trusted",
        "goal:fsa:foundation-awareness",
        "authority:fsa:bounded",
        "architecture:fsa:v1",
        "state:trusted",
        true,
        "evidence:behavioral:trusted",
        Now);

    private static FsaForensicSnapshot Forensics(bool preserved) => new(
        "forensics:incident:1",
        FsaGovernanceBoundary.CanonicalFsaId,
        "artifact:trusted",
        "config:trusted",
        "dependencies:trusted",
        "goal:fsa:foundation-awareness",
        "authority:fsa:bounded",
        "delegation:fsa:bounded",
        "lifecycle:isolated",
        "state:captured",
        "audit:incident:1",
        "incident:1",
        Now,
        preserved);

    private static FsaRecoveryRequest Recovery(FsaRecoveryAction action, FsaTrustedBaseline baseline, FsaForensicSnapshot forensics) => new(
        "recovery:1",
        "owner:recovery-governance",
        FsaGovernanceBoundary.CanonicalFsaId,
        action,
        forensics,
        "root-cause:1",
        true,
        baseline,
        true,
        true,
        true,
        true,
        true,
        true,
        true,
        Now);

    private static FsaEvolutionProposal Evolution() => new(
        "evolution:1",
        FsaGovernanceBoundary.CanonicalFsaId,
        FsaEvolutionPurpose.ImproveAccuracy,
        false,
        false,
        false,
        false,
        false,
        false,
        false,
        false,
        false,
        false,
        false,
        true,
        true,
        "evidence:evolution:1");

    private static FsaPeerSubmission Peer() => new(
        "submission:1",
        "source-scope:alpha",
        "awareness:alpha",
        FsaGovernanceBoundary.CanonicalFsaId,
        "candidate:alpha:1",
        "1.0.0",
        "sha256:candidate-alpha-1",
        "provenance:candidate-alpha-1",
        "evidence:lower-tier:alpha-1",
        true,
        true,
        false,
        false,
        false,
        false,
        Now);

    private static void Check(bool condition, string message)
    {
        _checks++;
        if (!condition) throw new InvalidOperationException(message);
    }
}
