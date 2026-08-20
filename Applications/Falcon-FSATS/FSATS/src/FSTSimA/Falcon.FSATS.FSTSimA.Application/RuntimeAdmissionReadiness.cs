namespace Falcon.FSATS.FSTSimA.Application;

public enum FSTSimAExecutionClass { Simulation, Replay, Synthetic, Test, Paper, Live }
public enum FSTSimARuntimeReadinessCondition { NotReady, LocalReadyExternalAuthorityPending, EligibleForAdmissionReview }

public sealed record FSTSimARuntimeReadinessEvidence(
    string ConfigurationEvidenceId,
    string HealthEvidenceId,
    string RecoveryEvidenceId,
    string DeclarationEvidenceId,
    string? ExternalAuthorityEvidenceId,
    bool ExternalAuthorityEvidenceValidated);

public sealed record FSTSimARuntimeReadinessInput(
    string ApplicationId, string EvaluationId, string Environment, string ScenarioOrRunProfileId, FSTSimAExecutionClass ExecutionClass,
    bool ConfigurationCurrentAndValid, bool OperationalHealthReady, bool RecoveryEvidenceComplete, bool EvidenceReproducibilityComplete,
    bool DependencyDeclarationsComplete, bool PermissionDeclarationsComplete, bool RouteDeclarationsComplete, bool EvidenceIntegrityValid,
    bool ExternalNonLiveEgressRequired, bool ExternalNonLiveEgressAuthoritySatisfied,
    bool RecoveryReleaseReviewRequired, bool RepairSucceeded, bool IndependentRecoveryValidated,
    FSTSimARuntimeReadinessEvidence? Evidence)
{
    public IReadOnlyList<string>? Stage13RegisteredAiTargetIds { get; init; }
    public string? Stage13CanonicalAiKillArtifactIdentity { get; init; }
    public string? ApplicationRuntimeInstanceId { get; init; }
    public long ApplicationRuntimeGeneration { get; init; }
    public string? Stage13BoundRuntimeInstanceId { get; init; }
    public long Stage13BoundRuntimeGeneration { get; init; }
    public bool Stage13AiKillTargetRegistrationSatisfied { get; init; }
    public bool Stage13AiKillEnforcementBindingSatisfied { get; init; }
    public bool CurrentFoundationAiReleaseSatisfied { get; init; }
    public string? AiKillBindingEvidenceId { get; init; }
}

public sealed record FSTSimARuntimeReadinessAssessment(
    FSTSimARuntimeReadinessCondition Condition, string ReasonCode, bool LocalReadinessPassed, bool ExternalGatesSatisfied,
    bool EligibleForAdmissionReview, bool ReadyForExternalReleaseReview, bool GrantsRuntimeAuthority)
{
    public static FSTSimARuntimeReadinessAssessment NotReady(string reason) => new(FSTSimARuntimeReadinessCondition.NotReady, reason, false, false, false, false, false);
}

public static class FSTSimARuntimeAdmissionReadiness
{
    public const string ApplicationId = "FSATS-FSTSIMA";
    public const string ExpectedStage13CanonicalAiKillArtifactIdentity = "foundation/contracts/ai-kill-control-plane|1.0.0|sha256/BD36F1A4B8D49EC08347D3051461D788C636D03AF65B5616DE6B52B7A112B770|evidence:foundation:stage13:owner-closure:e59ccbba5786755b4e7f17a29810465ab0d4d6ed|compat:foundation-ai-kill-control-plane:v1|Foundation.Authority.AiKillControlPlaneContract|8453bd5961eb4ef3c9e35650f94d8c91ad0b81bc|Contract|foundation.authority|Published";
    public static IReadOnlyList<string> ExpectedStage13AiTargetIds { get; } = Array.AsReadOnly(
        new[] { "MSA-FSTSIMA-01" }
            .Concat(Enumerable.Range(1, 8).Select(i => $"S-LSA-{i:00}"))
            .Concat(new[] { "CSA-S02-01", "CSA-S07-01" })
            .ToArray());

    public static FSTSimARuntimeReadinessAssessment Assess(FSTSimARuntimeReadinessInput? input)
    {
        if (input is null) return FSTSimARuntimeReadinessAssessment.NotReady("P7_FSTSIMA_INPUT_REQUIRED");
        if (!Valid(input.ApplicationId) || !Valid(input.EvaluationId) || !Valid(input.Environment) || !Valid(input.ScenarioOrRunProfileId)) return FSTSimARuntimeReadinessAssessment.NotReady("P7_FSTSIMA_IDENTITY_INCOMPLETE");
        if (!StringComparer.Ordinal.Equals(input.ApplicationId, ApplicationId)) return FSTSimARuntimeReadinessAssessment.NotReady("P7_FSTSIMA_APPLICATION_ID_MISMATCH");
        if (input.Evidence is null || !Valid(input.Evidence.ConfigurationEvidenceId) || !Valid(input.Evidence.HealthEvidenceId) || !Valid(input.Evidence.DeclarationEvidenceId) || !Valid(input.Evidence.RecoveryEvidenceId))
            return FSTSimARuntimeReadinessAssessment.NotReady("P7_FSTSIMA_EVIDENCE_BINDING_INCOMPLETE");
        if (!Enum.IsDefined(input.ExecutionClass)) return FSTSimARuntimeReadinessAssessment.NotReady("P7_FSTSIMA_EXECUTION_CLASS_INVALID");
        if (input.ExecutionClass is FSTSimAExecutionClass.Paper or FSTSimAExecutionClass.Live) return FSTSimARuntimeReadinessAssessment.NotReady("P7_FSTSIMA_PAPER_LIVE_ESCALATION_PROHIBITED");
        if (!input.EvidenceIntegrityValid || !input.EvidenceReproducibilityComplete) return FSTSimARuntimeReadinessAssessment.NotReady("P7_FSTSIMA_EVIDENCE_NOT_READY");
        if (!input.ConfigurationCurrentAndValid || !input.OperationalHealthReady || !input.RecoveryEvidenceComplete) return FSTSimARuntimeReadinessAssessment.NotReady("P7_FSTSIMA_CONFIG_HEALTH_OR_RECOVERY_NOT_READY");
        if (!input.DependencyDeclarationsComplete || !input.PermissionDeclarationsComplete || !input.RouteDeclarationsComplete) return FSTSimARuntimeReadinessAssessment.NotReady("P7_FSTSIMA_DECLARATIONS_INCOMPLETE");
        var releaseReady = !input.RecoveryReleaseReviewRequired || (input.RepairSucceeded && input.IndependentRecoveryValidated);
        if (!releaseReady) return FSTSimARuntimeReadinessAssessment.NotReady("P7_FSTSIMA_RELEASE_REVIEW_EVIDENCE_INCOMPLETE");
        if (!ExactTargetSet(input.Stage13RegisteredAiTargetIds))
            return new(FSTSimARuntimeReadinessCondition.LocalReadyExternalAuthorityPending, "P7_FSTSIMA_STAGE13_AI_TARGET_SET_PENDING_OR_MISMATCH", true, false, false, input.RecoveryReleaseReviewRequired, false);
        if (!RuntimeIdentityBound(input.ApplicationRuntimeInstanceId, input.ApplicationRuntimeGeneration, input.Stage13BoundRuntimeInstanceId, input.Stage13BoundRuntimeGeneration))
            return new(FSTSimARuntimeReadinessCondition.LocalReadyExternalAuthorityPending, "P7_FSTSIMA_STAGE13_AI_RUNTIME_IDENTITY_PENDING_OR_MISMATCH", true, false, false, input.RecoveryReleaseReviewRequired, false);
        if (!input.Stage13AiKillTargetRegistrationSatisfied || !input.Stage13AiKillEnforcementBindingSatisfied || !input.CurrentFoundationAiReleaseSatisfied)
            return new(FSTSimARuntimeReadinessCondition.LocalReadyExternalAuthorityPending, "P7_FSTSIMA_STAGE13_AI_KILL_BINDING_OR_RELEASE_PENDING", true, false, false, input.RecoveryReleaseReviewRequired, false);
        if (!Valid(input.AiKillBindingEvidenceId)) return FSTSimARuntimeReadinessAssessment.NotReady("P7_FSTSIMA_STAGE13_AI_KILL_BINDING_EVIDENCE_INVALID");
        if (!StringComparer.Ordinal.Equals(input.Stage13CanonicalAiKillArtifactIdentity, ExpectedStage13CanonicalAiKillArtifactIdentity))
            return new(FSTSimARuntimeReadinessCondition.LocalReadyExternalAuthorityPending, "P7_FSTSIMA_STAGE13_CANONICAL_AI_KILL_ARTIFACT_PENDING_OR_MISMATCH", true, false, false, input.RecoveryReleaseReviewRequired, false);
        if (input.ExternalNonLiveEgressRequired && !input.ExternalNonLiveEgressAuthoritySatisfied)
            return new(FSTSimARuntimeReadinessCondition.LocalReadyExternalAuthorityPending, "P7_FSTSIMA_NONLIVE_EGRESS_AUTHORITY_PENDING", true, false, false, input.RecoveryReleaseReviewRequired, false);
        if (input.ExternalNonLiveEgressAuthoritySatisfied && (!input.Evidence.ExternalAuthorityEvidenceValidated || !Valid(input.Evidence.ExternalAuthorityEvidenceId)))
            return FSTSimARuntimeReadinessAssessment.NotReady("P7_FSTSIMA_EXTERNAL_AUTHORITY_EVIDENCE_INVALID");
        return new(FSTSimARuntimeReadinessCondition.EligibleForAdmissionReview, "P7_FSTSIMA_ELIGIBLE_FOR_EXTERNAL_ADMISSION_REVIEW", true, true, true, input.RecoveryReleaseReviewRequired, false);
    }

    private static bool ExactTargetSet(IReadOnlyList<string>? actual)
        => actual is not null && actual.Count == ExpectedStage13AiTargetIds.Count && actual.Distinct(StringComparer.Ordinal).Count() == actual.Count && ExpectedStage13AiTargetIds.All(expected => actual.Contains(expected, StringComparer.Ordinal));
    private static bool RuntimeIdentityBound(string? currentInstanceId, long currentGeneration, string? boundInstanceId, long boundGeneration)
        => Valid(currentInstanceId) && Valid(boundInstanceId) && currentGeneration > 0 && boundGeneration > 0 && currentGeneration == boundGeneration && StringComparer.Ordinal.Equals(currentInstanceId, boundInstanceId);
    private static bool Valid(string? value) => !string.IsNullOrWhiteSpace(value) && StringComparer.Ordinal.Equals(value, value.Trim());
}
