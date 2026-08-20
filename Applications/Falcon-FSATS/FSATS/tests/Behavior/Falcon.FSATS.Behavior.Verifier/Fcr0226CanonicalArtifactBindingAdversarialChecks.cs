using System.Runtime.CompilerServices;
using TA = Falcon.FSATS.Trading.Application;
using PA = Falcon.FSATS.FSAPMA.Application;
using GA = Falcon.FSATS.TradingGuardian.Application;
using SA = Falcon.FSATS.FSTSimA.Application;
using RA = Falcon.FSATS.ResourceManagement.Application;

internal static class Fcr0226CanonicalArtifactBindingAdversarialChecks
{
    private const string ExactCanonicalIdentity = "foundation/contracts/ai-kill-control-plane|1.0.0|sha256/BD36F1A4B8D49EC08347D3051461D788C636D03AF65B5616DE6B52B7A112B770|evidence:foundation:stage13:owner-closure:e59ccbba5786755b4e7f17a29810465ab0d4d6ed|compat:foundation-ai-kill-control-plane:v1|Foundation.Authority.AiKillControlPlaneContract|8453bd5961eb4ef3c9e35650f94d8c91ad0b81bc|Contract|foundation.authority|Published";

    [ModuleInitializer]
    internal static void Run()
    {
        AllApplicationFencesPinTheExactCanonicalIdentity();
        MissingOrMutatedCanonicalIdentityFailsClosedAcrossAllApplications();
    }

    private static void AllApplicationFencesPinTheExactCanonicalIdentity()
    {
        Require(TA.TradingRuntimeAdmissionReadiness.ExpectedStage13CanonicalAiKillArtifactIdentity == ExactCanonicalIdentity, "FCR0226_TRADING_CANONICAL_IDENTITY_DRIFT");
        Require(PA.FSAPMARuntimeAdmissionReadiness.ExpectedStage13CanonicalAiKillArtifactIdentity == ExactCanonicalIdentity, "FCR0226_FSAPMA_CANONICAL_IDENTITY_DRIFT");
        Require(GA.GuardianRuntimeAdmissionReadiness.ExpectedStage13CanonicalAiKillArtifactIdentity == ExactCanonicalIdentity, "FCR0226_GUARDIAN_CANONICAL_IDENTITY_DRIFT");
        Require(SA.FSTSimARuntimeAdmissionReadiness.ExpectedStage13CanonicalAiKillArtifactIdentity == ExactCanonicalIdentity, "FCR0226_FSTSIMA_CANONICAL_IDENTITY_DRIFT");
        Require(RA.ResourceRuntimeAdmissionReadiness.ExpectedStage13CanonicalAiKillArtifactIdentity == ExactCanonicalIdentity, "FCR0226_APP_RSC_CANONICAL_IDENTITY_DRIFT");
    }

    private static void MissingOrMutatedCanonicalIdentityFailsClosedAcrossAllApplications()
    {
        var trading = new TA.TradingRuntimeReadinessInput(
            TA.TradingRuntimeAdmissionReadiness.ApplicationId, "eval-canonical-trading", "ALPACA", "PA-ACCOUNT-A", "PAPER", 7,
            true, true, true, true, true, true, true, true, true, false, false, false, false, false, false, false,
            new TA.TradingRuntimeReadinessEvidence("cfg", "health", "recovery", "decl", null, false))
        {
            Stage13RegisteredAiTargetIds = TA.TradingRuntimeAdmissionReadiness.ExpectedStage13AiTargetIds,
            ApplicationRuntimeInstanceId = "trading-runtime-1", ApplicationRuntimeGeneration = 1,
            Stage13BoundRuntimeInstanceId = "trading-runtime-1", Stage13BoundRuntimeGeneration = 1,
            Stage13AiKillTargetRegistrationSatisfied = true, Stage13AiKillEnforcementBindingSatisfied = true,
            CurrentFoundationAiReleaseSatisfied = true, AiKillBindingEvidenceId = "kill-binding-evidence"
        };
        AssertCanonicalHold(TA.TradingRuntimeAdmissionReadiness.Assess(trading).ReasonCode, "P7_TRADING_STAGE13_CANONICAL_AI_KILL_ARTIFACT_PENDING_OR_MISMATCH", "FCR0226_TRADING_MISSING_CANONICAL_IDENTITY_ACCEPTED");
        AssertCanonicalHold(TA.TradingRuntimeAdmissionReadiness.Assess(trading with { Stage13CanonicalAiKillArtifactIdentity = ExactCanonicalIdentity + "-MUTATED" }).ReasonCode, "P7_TRADING_STAGE13_CANONICAL_AI_KILL_ARTIFACT_PENDING_OR_MISMATCH", "FCR0226_TRADING_MUTATED_CANONICAL_IDENTITY_ACCEPTED");

        var fsapma = new PA.FSAPMARuntimeReadinessInput(
            PA.FSAPMARuntimeAdmissionReadiness.ApplicationId, "eval-canonical-fsapma", "ALPACA-DATA", "provider-account-a", "PAPER",
            "MARKET_DATA", "api", "endpoint", "cred-ref",
            true, true, true, true, true, true, true, true, false, false, false, false, false, false,
            new PA.FSAPMARuntimeReadinessEvidence("cfg", "health", "recovery", "decl", null, false))
        {
            Stage13RegisteredAiTargetIds = PA.FSAPMARuntimeAdmissionReadiness.ExpectedStage13AiTargetIds,
            ApplicationRuntimeInstanceId = "fsapma-runtime-1", ApplicationRuntimeGeneration = 1,
            Stage13BoundRuntimeInstanceId = "fsapma-runtime-1", Stage13BoundRuntimeGeneration = 1,
            Stage13AiKillTargetRegistrationSatisfied = true, Stage13AiKillEnforcementBindingSatisfied = true,
            CurrentFoundationAiReleaseSatisfied = true, AiKillBindingEvidenceId = "kill-binding-evidence"
        };
        AssertCanonicalHold(PA.FSAPMARuntimeAdmissionReadiness.Assess(fsapma).ReasonCode, "P7_FSAPMA_STAGE13_CANONICAL_AI_KILL_ARTIFACT_PENDING_OR_MISMATCH", "FCR0226_FSAPMA_MISSING_CANONICAL_IDENTITY_ACCEPTED");
        AssertCanonicalHold(PA.FSAPMARuntimeAdmissionReadiness.Assess(fsapma with { Stage13CanonicalAiKillArtifactIdentity = ExactCanonicalIdentity + "-MUTATED" }).ReasonCode, "P7_FSAPMA_STAGE13_CANONICAL_AI_KILL_ARTIFACT_PENDING_OR_MISMATCH", "FCR0226_FSAPMA_MUTATED_CANONICAL_IDENTITY_ACCEPTED");

        var guardian = new GA.GuardianRuntimeReadinessInput(
            GA.GuardianRuntimeAdmissionReadiness.ApplicationId, "eval-canonical-guardian", "APPLICATION", "FSATS-TRADING", "PAPER",
            true, true, true, true, true, true, true, true, true, true, false, false, false, false, false, false,
            new GA.GuardianRuntimeReadinessEvidence("cfg", "health", "recovery", "decl", null, false))
        {
            Stage13RegisteredAiTargetIds = GA.GuardianRuntimeAdmissionReadiness.ExpectedStage13AiTargetIds,
            ApplicationRuntimeInstanceId = "guardian-runtime-1", ApplicationRuntimeGeneration = 1,
            Stage13BoundRuntimeInstanceId = "guardian-runtime-1", Stage13BoundRuntimeGeneration = 1,
            Stage13AiKillTargetRegistrationSatisfied = true, Stage13AiKillEnforcementBindingSatisfied = true,
            CurrentFoundationAiReleaseSatisfied = true, AiKillBindingEvidenceId = "kill-binding-evidence"
        };
        AssertCanonicalHold(GA.GuardianRuntimeAdmissionReadiness.Assess(guardian).ReasonCode, "P7_GUARDIAN_STAGE13_CANONICAL_AI_KILL_ARTIFACT_PENDING_OR_MISMATCH", "FCR0226_GUARDIAN_MISSING_CANONICAL_IDENTITY_ACCEPTED");
        AssertCanonicalHold(GA.GuardianRuntimeAdmissionReadiness.Assess(guardian with { Stage13CanonicalAiKillArtifactIdentity = ExactCanonicalIdentity + "-MUTATED" }).ReasonCode, "P7_GUARDIAN_STAGE13_CANONICAL_AI_KILL_ARTIFACT_PENDING_OR_MISMATCH", "FCR0226_GUARDIAN_MUTATED_CANONICAL_IDENTITY_ACCEPTED");

        var simulator = new SA.FSTSimARuntimeReadinessInput(
            SA.FSTSimARuntimeAdmissionReadiness.ApplicationId, "eval-canonical-sim", "TEST", "scenario", SA.FSTSimAExecutionClass.Simulation,
            true, true, true, true, true, true, true, true, false, false, false, false, false,
            new SA.FSTSimARuntimeReadinessEvidence("cfg", "health", "recovery", "decl", null, false))
        {
            Stage13RegisteredAiTargetIds = SA.FSTSimARuntimeAdmissionReadiness.ExpectedStage13AiTargetIds,
            ApplicationRuntimeInstanceId = "sim-runtime-1", ApplicationRuntimeGeneration = 1,
            Stage13BoundRuntimeInstanceId = "sim-runtime-1", Stage13BoundRuntimeGeneration = 1,
            Stage13AiKillTargetRegistrationSatisfied = true, Stage13AiKillEnforcementBindingSatisfied = true,
            CurrentFoundationAiReleaseSatisfied = true, AiKillBindingEvidenceId = "kill-binding-evidence"
        };
        AssertCanonicalHold(SA.FSTSimARuntimeAdmissionReadiness.Assess(simulator).ReasonCode, "P7_FSTSIMA_STAGE13_CANONICAL_AI_KILL_ARTIFACT_PENDING_OR_MISMATCH", "FCR0226_FSTSIMA_MISSING_CANONICAL_IDENTITY_ACCEPTED");
        AssertCanonicalHold(SA.FSTSimARuntimeAdmissionReadiness.Assess(simulator with { Stage13CanonicalAiKillArtifactIdentity = ExactCanonicalIdentity + "-MUTATED" }).ReasonCode, "P7_FSTSIMA_STAGE13_CANONICAL_AI_KILL_ARTIFACT_PENDING_OR_MISMATCH", "FCR0226_FSTSIMA_MUTATED_CANONICAL_IDENTITY_ACCEPTED");

        var rsc = new RA.ResourceRuntimeReadinessInput(
            RA.ResourceRuntimeAdmissionReadiness.ApplicationId, "eval-canonical-rsc", "PAPER", "epoch", "foundation-envelope",
            true, true, true, true, true, true, true, true, true, true, false, false, false, false, false, false,
            new RA.ResourceRuntimeReadinessEvidence("cfg", "health", "recovery", "decl", null, false))
        {
            Stage13RegisteredAiTargetIds = RA.ResourceRuntimeAdmissionReadiness.ExpectedStage13AiTargetIds,
            ApplicationRuntimeInstanceId = "rsc-runtime-1", ApplicationRuntimeGeneration = 1,
            Stage13BoundRuntimeInstanceId = "rsc-runtime-1", Stage13BoundRuntimeGeneration = 1,
            Stage13AiKillTargetRegistrationSatisfied = true, Stage13AiKillEnforcementBindingSatisfied = true,
            CurrentFoundationAiReleaseSatisfied = true, AiKillBindingEvidenceId = "kill-binding-evidence"
        };
        AssertCanonicalHold(RA.ResourceRuntimeAdmissionReadiness.Assess(rsc).ReasonCode, "P7_APP_RSC_STAGE13_CANONICAL_AI_KILL_ARTIFACT_PENDING_OR_MISMATCH", "FCR0226_APP_RSC_MISSING_CANONICAL_IDENTITY_ACCEPTED");
        AssertCanonicalHold(RA.ResourceRuntimeAdmissionReadiness.Assess(rsc with { Stage13CanonicalAiKillArtifactIdentity = ExactCanonicalIdentity + "-MUTATED" }).ReasonCode, "P7_APP_RSC_STAGE13_CANONICAL_AI_KILL_ARTIFACT_PENDING_OR_MISMATCH", "FCR0226_APP_RSC_MUTATED_CANONICAL_IDENTITY_ACCEPTED");
    }

    private static void AssertCanonicalHold(string actualReason, string expectedReason, string failure)
    {
        Require(StringComparer.Ordinal.Equals(actualReason, expectedReason), failure);
    }

    private static void Require(bool condition, string failure)
    {
        if (!condition) throw new InvalidOperationException(failure);
    }
}
