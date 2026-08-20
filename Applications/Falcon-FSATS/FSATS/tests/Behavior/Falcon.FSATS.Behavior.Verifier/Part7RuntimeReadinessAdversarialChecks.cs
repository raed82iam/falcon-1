using TA = Falcon.FSATS.Trading.Application;
using PA = Falcon.FSATS.FSAPMA.Application;
using GA = Falcon.FSATS.TradingGuardian.Application;
using SA = Falcon.FSATS.FSTSimA.Application;
using RA = Falcon.FSATS.ResourceManagement.Application;

internal static class Part7RuntimeReadinessAdversarialChecks
{
    internal static void Run()
    {
        TradingReadinessIsBrokerAccountScopedAndNonAuthoritative();
        FSAPMARouteReadinessRequiresExactCurrentRouteIdentityAndAuthority();
        GuardianCannotSelfRelease();
        AppRscCannotMintFoundationAuthority();
        FSTSimACannotEscalateToPaperOrLive();
        RepairSuccessDoesNotEqualRelease();
        Stage13AiKillGateIsMandatoryAcrossAllApplications();
        Stage13AiTargetSetCannotBeMissingExpandedOrDuplicated();
        RuntimeReplacementAndRestartCannotReuseStage13Binding();
        ExternalAuthorityClaimRequiresBoundEvidence();
    }

    private static TA.TradingRuntimeReadinessEvidence TradingEvidence(string? external = null, bool externalValidated = false) =>
        new("cfg-evidence", "health-evidence", "recovery-evidence", "declaration-evidence", external, externalValidated);

    private static TA.TradingRuntimeReadinessInput WithSatisfiedAiKillGate(TA.TradingRuntimeReadinessInput input) => input with
    {
        Stage13RegisteredAiTargetIds = TA.TradingRuntimeAdmissionReadiness.ExpectedStage13AiTargetIds,
        Stage13CanonicalAiKillArtifactIdentity = TA.TradingRuntimeAdmissionReadiness.ExpectedStage13CanonicalAiKillArtifactIdentity,
        ApplicationRuntimeInstanceId = "trading-runtime-1",
        ApplicationRuntimeGeneration = 1,
        Stage13BoundRuntimeInstanceId = "trading-runtime-1",
        Stage13BoundRuntimeGeneration = 1,
        Stage13AiKillTargetRegistrationSatisfied = true,
        Stage13AiKillEnforcementBindingSatisfied = true,
        CurrentFoundationAiReleaseSatisfied = true,
        AiKillBindingEvidenceId = "stage13-ai-kill-evidence"
    };

    private static void TradingReadinessIsBrokerAccountScopedAndNonAuthoritative()
    {
        var input = new TA.TradingRuntimeReadinessInput(
            TA.TradingRuntimeAdmissionReadiness.ApplicationId, "eval-trading-1", "ALPACA", "PA-ACCOUNT-A", "PAPER", 7,
            true, true, true, true, true, true, true, true, true, false, false, true, false, true, true, true,
            TradingEvidence());
        var pending = TA.TradingRuntimeAdmissionReadiness.Assess(input);
        Require(pending.LocalReadinessPassed, "P7_RT_TRADING_LOCAL_READINESS_NOT_RECOGNIZED");
        Require(!pending.ExternalGatesSatisfied && !pending.EligibleForAdmissionReview, "P7_RT_TRADING_MISSING_EXTERNAL_GATES_ACCEPTED");
        Require(pending.ReadyForExternalReleaseReview && !pending.GrantsRuntimeAuthority, "P7_RT_TRADING_RELEASE_OR_AUTHORITY_BOUNDARY_BROKEN");
        var customerInjected = TA.TradingRuntimeAdmissionReadiness.Assess(input with { ContainsCustomerOrUserIdentity = true });
        Require(!customerInjected.LocalReadinessPassed && customerInjected.ReasonCode == "P7_TRADING_CUSTOMER_USER_IDENTITY_PROHIBITED", "P7_RT_TRADING_CUSTOMER_IDENTITY_ACCEPTED");
        var externallySatisfied = TA.TradingRuntimeAdmissionReadiness.Assess(WithSatisfiedAiKillGate(input with
        {
            BrokerExecutionAuthoritySatisfied = true,
            Evidence = TradingEvidence("broker-authority-evidence", true)
        }));
        Require(externallySatisfied.EligibleForAdmissionReview && !externallySatisfied.GrantsRuntimeAuthority, "P7_RT_TRADING_ADMISSION_REVIEW_CONFUSED_WITH_RUNTIME_AUTHORITY");
    }

    private static void FSAPMARouteReadinessRequiresExactCurrentRouteIdentityAndAuthority()
    {
        var evidence = new PA.FSAPMARuntimeReadinessEvidence("cfg", "health", "recovery", "decl", null, false);
        var input = new PA.FSAPMARuntimeReadinessInput(
            PA.FSAPMARuntimeAdmissionReadiness.ApplicationId, "eval-provider-1", "ALPACA-DATA", "provider-account-a", "PAPER",
            "MARKET_DATA", "paper-primary", "marketdata-stream-v2", "cred-ref-a",
            true, true, true, true, true, true, true, true, false, true, false, false, false, false, evidence);
        var pending = PA.FSAPMARuntimeAdmissionReadiness.Assess(input);
        Require(pending.LocalReadinessPassed && !pending.ExternalGatesSatisfied && !pending.GrantsRuntimeAuthority, "P7_RT_FSAPMA_PROVIDER_EGRESS_HOLD_NOT_PRESERVED");
        var incomplete = PA.FSAPMARuntimeAdmissionReadiness.Assess(input with { EndpointId = " " });
        Require(!incomplete.LocalReadinessPassed && incomplete.ReasonCode == "P7_FSAPMA_ROUTE_IDENTITY_INCOMPLETE", "P7_RT_FSAPMA_INCOMPLETE_ROUTE_IDENTITY_ACCEPTED");
        var secretBytes = PA.FSAPMARuntimeAdmissionReadiness.Assess(input with { ContainsSecretBytes = true });
        Require(!secretBytes.LocalReadinessPassed, "P7_RT_FSAPMA_SECRET_BYTES_ACCEPTED");
    }

    private static void GuardianCannotSelfRelease()
    {
        var evidence = new GA.GuardianRuntimeReadinessEvidence("cfg", "health", "recovery", "decl", null, false);
        var input = new GA.GuardianRuntimeReadinessInput(
            GA.GuardianRuntimeAdmissionReadiness.ApplicationId, "eval-guardian-1", "BROKER_ACCOUNT", "ALPACA/PA-ACCOUNT-A", "PAPER",
            true, true, true, true, true, true, true, true, true, true, false, true, false, true, true, true, evidence);
        var pending = GA.GuardianRuntimeAdmissionReadiness.Assess(input);
        Require(pending.LocalReadinessPassed && !pending.ExternalGatesSatisfied && pending.ReadyForExternalReleaseReview && !pending.GrantsRuntimeAuthority, "P7_RT_GUARDIAN_EXTERNAL_HOLD_OR_RELEASE_REVIEW_WRONG");
        var selfRelease = GA.GuardianRuntimeAdmissionReadiness.Assess(input with { AttemptsSelfRelease = true });
        Require(!selfRelease.LocalReadinessPassed && selfRelease.ReasonCode == "P7_GUARDIAN_SELF_RELEASE_PROHIBITED", "P7_RT_GUARDIAN_SELF_RELEASE_ACCEPTED");
    }

    private static void AppRscCannotMintFoundationAuthority()
    {
        var evidence = new RA.ResourceRuntimeReadinessEvidence("cfg", "health", "recovery", "decl", null, false);
        var input = new RA.ResourceRuntimeReadinessInput(
            RA.ResourceRuntimeAdmissionReadiness.ApplicationId, "eval-rsc-1", "PAPER", "epoch-7", "foundation-envelope-ref-7",
            true, true, true, true, true, true, true, true, true, true, false, true, false, false, false, false, evidence);
        var pending = RA.ResourceRuntimeAdmissionReadiness.Assess(input);
        Require(pending.LocalReadinessPassed && !pending.ExternalGatesSatisfied && !pending.GrantsRuntimeAuthority, "P7_RT_APP_RSC_CANONICAL_BINDING_HOLD_NOT_PRESERVED");
        var mint = RA.ResourceRuntimeAdmissionReadiness.Assess(input with { AttemptsToMintFoundationGrantOrTotalTruth = true });
        Require(!mint.LocalReadinessPassed && mint.ReasonCode == "P7_APP_RSC_FOUNDATION_AUTHORITY_MINTING_PROHIBITED", "P7_RT_APP_RSC_FOUNDATION_AUTHORITY_MINTING_ACCEPTED");
    }

    private static void FSTSimACannotEscalateToPaperOrLive()
    {
        var evidence = new SA.FSTSimARuntimeReadinessEvidence("cfg", "health", "recovery", "decl", null, false);
        var input = new SA.FSTSimARuntimeReadinessInput(
            SA.FSTSimARuntimeAdmissionReadiness.ApplicationId, "eval-sim-1", "TEST", "scenario-stress-1", SA.FSTSimAExecutionClass.Simulation,
            true, true, true, true, true, true, true, true, true, false, false, false, false, evidence);
        var pending = SA.FSTSimARuntimeAdmissionReadiness.Assess(input);
        Require(pending.LocalReadinessPassed && !pending.ExternalGatesSatisfied && !pending.GrantsRuntimeAuthority, "P7_RT_FSTSIMA_NONLIVE_EGRESS_HOLD_NOT_PRESERVED");
        var paper = SA.FSTSimARuntimeAdmissionReadiness.Assess(input with { ExecutionClass = SA.FSTSimAExecutionClass.Paper });
        var live = SA.FSTSimARuntimeAdmissionReadiness.Assess(input with { ExecutionClass = SA.FSTSimAExecutionClass.Live });
        Require(!paper.LocalReadinessPassed && !live.LocalReadinessPassed, "P7_RT_FSTSIMA_PAPER_OR_LIVE_ESCALATION_ACCEPTED");
    }

    private static void RepairSuccessDoesNotEqualRelease()
    {
        var input = new TA.TradingRuntimeReadinessInput(
            TA.TradingRuntimeAdmissionReadiness.ApplicationId, "eval-trading-repair", "ALPACA", "PA-ACCOUNT-A", "PAPER", 7,
            true, true, true, true, true, true, true, true, true, false, false, false, false, true, true, false,
            TradingEvidence());
        var assessment = TA.TradingRuntimeAdmissionReadiness.Assess(input);
        Require(!assessment.LocalReadinessPassed && !assessment.ReadyForExternalReleaseReview && !assessment.GrantsRuntimeAuthority, "P7_RT_REPAIR_SUCCESS_WAS_TREATED_AS_RELEASE_READY");
    }

    private static void Stage13AiKillGateIsMandatoryAcrossAllApplications()
    {
        var trading = new TA.TradingRuntimeReadinessInput(
            TA.TradingRuntimeAdmissionReadiness.ApplicationId, "eval-kill-trading", "ALPACA", "PA-ACCOUNT-A", "PAPER", 7,
            true, true, true, true, true, true, true, true, true, false, false, false, false, false, false, false, TradingEvidence())
        {
            Stage13RegisteredAiTargetIds = TA.TradingRuntimeAdmissionReadiness.ExpectedStage13AiTargetIds,
            ApplicationRuntimeInstanceId = "trading-runtime-1", ApplicationRuntimeGeneration = 1,
            Stage13BoundRuntimeInstanceId = "trading-runtime-1", Stage13BoundRuntimeGeneration = 1
        };
        Require(TA.TradingRuntimeAdmissionReadiness.Assess(trading).ReasonCode == "P7_TRADING_STAGE13_AI_KILL_BINDING_OR_RELEASE_PENDING", "P7_RT_TRADING_STAGE13_KILL_GATE_MISSING");

        var fsapma = new PA.FSAPMARuntimeReadinessInput(
            PA.FSAPMARuntimeAdmissionReadiness.ApplicationId, "eval-kill-fsapma", "ALPACA-DATA", "provider-account-a", "PAPER", "MARKET_DATA", "api", "endpoint", "cred-ref",
            true, true, true, true, true, true, true, true, false, false, false, false, false, false,
            new PA.FSAPMARuntimeReadinessEvidence("cfg", "health", "recovery", "decl", null, false))
        {
            Stage13RegisteredAiTargetIds = PA.FSAPMARuntimeAdmissionReadiness.ExpectedStage13AiTargetIds,
            ApplicationRuntimeInstanceId = "fsapma-runtime-1", ApplicationRuntimeGeneration = 1,
            Stage13BoundRuntimeInstanceId = "fsapma-runtime-1", Stage13BoundRuntimeGeneration = 1
        };
        Require(PA.FSAPMARuntimeAdmissionReadiness.Assess(fsapma).ReasonCode == "P7_FSAPMA_STAGE13_AI_KILL_BINDING_OR_RELEASE_PENDING", "P7_RT_FSAPMA_STAGE13_KILL_GATE_MISSING");

        var guardian = new GA.GuardianRuntimeReadinessInput(
            GA.GuardianRuntimeAdmissionReadiness.ApplicationId, "eval-kill-guardian", "APPLICATION", "FSATS-TRADING", "PAPER",
            true, true, true, true, true, true, true, true, true, true, false, false, false, false, false, false,
            new GA.GuardianRuntimeReadinessEvidence("cfg", "health", "recovery", "decl", null, false))
        {
            Stage13RegisteredAiTargetIds = GA.GuardianRuntimeAdmissionReadiness.ExpectedStage13AiTargetIds,
            ApplicationRuntimeInstanceId = "guardian-runtime-1", ApplicationRuntimeGeneration = 1,
            Stage13BoundRuntimeInstanceId = "guardian-runtime-1", Stage13BoundRuntimeGeneration = 1
        };
        Require(GA.GuardianRuntimeAdmissionReadiness.Assess(guardian).ReasonCode == "P7_GUARDIAN_STAGE13_AI_KILL_BINDING_OR_RELEASE_PENDING", "P7_RT_GUARDIAN_STAGE13_KILL_GATE_MISSING");

        var simulator = new SA.FSTSimARuntimeReadinessInput(
            SA.FSTSimARuntimeAdmissionReadiness.ApplicationId, "eval-kill-sim", "TEST", "scenario", SA.FSTSimAExecutionClass.Simulation,
            true, true, true, true, true, true, true, true, false, false, false, false, false,
            new SA.FSTSimARuntimeReadinessEvidence("cfg", "health", "recovery", "decl", null, false))
        {
            Stage13RegisteredAiTargetIds = SA.FSTSimARuntimeAdmissionReadiness.ExpectedStage13AiTargetIds,
            ApplicationRuntimeInstanceId = "sim-runtime-1", ApplicationRuntimeGeneration = 1,
            Stage13BoundRuntimeInstanceId = "sim-runtime-1", Stage13BoundRuntimeGeneration = 1
        };
        Require(SA.FSTSimARuntimeAdmissionReadiness.Assess(simulator).ReasonCode == "P7_FSTSIMA_STAGE13_AI_KILL_BINDING_OR_RELEASE_PENDING", "P7_RT_FSTSIMA_STAGE13_KILL_GATE_MISSING");

        var rsc = new RA.ResourceRuntimeReadinessInput(
            RA.ResourceRuntimeAdmissionReadiness.ApplicationId, "eval-kill-rsc", "PAPER", "epoch", "foundation-envelope",
            true, true, true, true, true, true, true, true, true, true, false, false, false, false, false, false,
            new RA.ResourceRuntimeReadinessEvidence("cfg", "health", "recovery", "decl", null, false))
        {
            Stage13RegisteredAiTargetIds = RA.ResourceRuntimeAdmissionReadiness.ExpectedStage13AiTargetIds,
            ApplicationRuntimeInstanceId = "rsc-runtime-1", ApplicationRuntimeGeneration = 1,
            Stage13BoundRuntimeInstanceId = "rsc-runtime-1", Stage13BoundRuntimeGeneration = 1
        };
        Require(RA.ResourceRuntimeAdmissionReadiness.Assess(rsc).ReasonCode == "P7_APP_RSC_STAGE13_AI_KILL_BINDING_OR_RELEASE_PENDING", "P7_RT_APP_RSC_STAGE13_KILL_GATE_MISSING");

        var missingEvidence = WithSatisfiedAiKillGate(trading) with { AiKillBindingEvidenceId = " " };
        Require(TA.TradingRuntimeAdmissionReadiness.Assess(missingEvidence).ReasonCode == "P7_TRADING_STAGE13_AI_KILL_BINDING_EVIDENCE_INVALID", "P7_RT_STAGE13_BOOLEAN_ACCEPTED_WITHOUT_EVIDENCE");
        var noRelease = WithSatisfiedAiKillGate(trading) with { CurrentFoundationAiReleaseSatisfied = false };
        Require(!TA.TradingRuntimeAdmissionReadiness.Assess(noRelease).EligibleForAdmissionReview, "P7_RT_BUSINESS_OR_RESTART_STATE_BYPASSED_FOUNDATION_RELEASE");
    }

    private static void Stage13AiTargetSetCannotBeMissingExpandedOrDuplicated()
    {
        var baseline = new TA.TradingRuntimeReadinessInput(
            TA.TradingRuntimeAdmissionReadiness.ApplicationId, "eval-target-set", "ALPACA", "PA-ACCOUNT-A", "PAPER", 7,
            true, true, true, true, true, true, true, true, true, false, false, false, false, false, false, false, TradingEvidence());

        var missing = TA.TradingRuntimeAdmissionReadiness.Assess(baseline with { Stage13RegisteredAiTargetIds = TA.TradingRuntimeAdmissionReadiness.ExpectedStage13AiTargetIds.Skip(1).ToArray() });
        Require(missing.ReasonCode == "P7_TRADING_STAGE13_AI_TARGET_SET_PENDING_OR_MISMATCH", "FCR0226_MISSING_TARGET_ACCEPTED");

        var expanded = TA.TradingRuntimeAdmissionReadiness.Assess(baseline with { Stage13RegisteredAiTargetIds = TA.TradingRuntimeAdmissionReadiness.ExpectedStage13AiTargetIds.Concat(new[] { "UNKNOWN-AI-TARGET" }).ToArray() });
        Require(expanded.ReasonCode == "P7_TRADING_STAGE13_AI_TARGET_SET_PENDING_OR_MISMATCH", "FCR0226_WIDER_TARGET_ACCEPTED");

        var duplicated = TA.TradingRuntimeAdmissionReadiness.Assess(baseline with { Stage13RegisteredAiTargetIds = TA.TradingRuntimeAdmissionReadiness.ExpectedStage13AiTargetIds.SelectMany((x, i) => i == 0 ? new[] { x, x } : new[] { x }).ToArray() });
        Require(duplicated.ReasonCode == "P7_TRADING_STAGE13_AI_TARGET_SET_PENDING_OR_MISMATCH", "FCR0226_DUPLICATE_TARGET_ACCEPTED");
    }

    private static void RuntimeReplacementAndRestartCannotReuseStage13Binding()
    {
        var baseline = WithSatisfiedAiKillGate(new TA.TradingRuntimeReadinessInput(
            TA.TradingRuntimeAdmissionReadiness.ApplicationId, "eval-runtime-fence", "ALPACA", "PA-ACCOUNT-A", "PAPER", 7,
            true, true, true, true, true, true, true, true, true, false, false, false, false, false, false, false, TradingEvidence()));

        var replacement = TA.TradingRuntimeAdmissionReadiness.Assess(baseline with { ApplicationRuntimeInstanceId = "trading-runtime-replacement" });
        Require(replacement.ReasonCode == "P7_TRADING_STAGE13_AI_RUNTIME_IDENTITY_PENDING_OR_MISMATCH" && !replacement.EligibleForAdmissionReview,
            "FCR0226_REPLACEMENT_RUNTIME_INHERITED_OLD_BINDING");

        var restartGeneration = TA.TradingRuntimeAdmissionReadiness.Assess(baseline with { ApplicationRuntimeGeneration = 2 });
        Require(restartGeneration.ReasonCode == "P7_TRADING_STAGE13_AI_RUNTIME_IDENTITY_PENDING_OR_MISMATCH" && !restartGeneration.EligibleForAdmissionReview,
            "FCR0226_RESTART_GENERATION_INHERITED_OLD_BINDING");

        var invalidGeneration = TA.TradingRuntimeAdmissionReadiness.Assess(baseline with { ApplicationRuntimeGeneration = 0, Stage13BoundRuntimeGeneration = 0 });
        Require(invalidGeneration.ReasonCode == "P7_TRADING_STAGE13_AI_RUNTIME_IDENTITY_PENDING_OR_MISMATCH", "FCR0226_INVALID_RUNTIME_GENERATION_ACCEPTED");
    }

    private static void ExternalAuthorityClaimRequiresBoundEvidence()
    {
        var input = WithSatisfiedAiKillGate(new TA.TradingRuntimeReadinessInput(
            TA.TradingRuntimeAdmissionReadiness.ApplicationId, "eval-trading-evidence", "ALPACA", "PA-ACCOUNT-A", "PAPER", 7,
            true, true, true, true, true, true, true, true, true, false, false, true, true, false, false, false,
            TradingEvidence()));
        var assessment = TA.TradingRuntimeAdmissionReadiness.Assess(input);
        Require(!assessment.LocalReadinessPassed && assessment.ReasonCode == "P7_TRADING_EXTERNAL_AUTHORITY_EVIDENCE_INVALID", "P7_RT_EXTERNAL_AUTHORITY_BOOLEAN_ACCEPTED_WITHOUT_EVIDENCE_BINDING");
    }

    private static void Require(bool condition, string failure)
    {
        if (!condition) throw new InvalidOperationException(failure);
    }
}