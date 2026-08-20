namespace Falcon.FSATS.ResourceManagement.Application;

public enum ResourceRuntimeReadinessCondition { NotReady, LocalReadyExternalAuthorityPending, EligibleForAdmissionReview }

public sealed record ResourceRuntimeReadinessEvidence(
    string ConfigurationEvidenceId,
    string HealthEvidenceId,
    string RecoveryEvidenceId,
    string DeclarationEvidenceId,
    string? ExternalAuthorityEvidenceId,
    bool ExternalAuthorityEvidenceValidated);

public sealed record ResourceRuntimeReadinessInput(
    string ApplicationId, string EvaluationId, string Environment, string CoordinationEpochId, string FoundationEnvelopeReference,
    bool ConfigurationCurrentAndValid, bool OperationalHealthReady, bool RecoveryEvidenceComplete, bool CoordinationEpochCurrent,
    bool PendingOutcomesReconciled, bool SafeResourceFloorsEstablished, bool DependencyDeclarationsComplete,
    bool PermissionDeclarationsComplete, bool RouteDeclarationsComplete, bool EvidenceIntegrityValid,
    bool AttemptsToMintFoundationGrantOrTotalTruth, bool CanonicalFoundationResourceBindingRequired,
    bool CanonicalFoundationResourceBindingSatisfied, bool RecoveryReleaseReviewRequired, bool RepairSucceeded,
    bool IndependentRecoveryValidated, ResourceRuntimeReadinessEvidence? Evidence)
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

public sealed record ResourceRuntimeReadinessAssessment(
    ResourceRuntimeReadinessCondition Condition, string ReasonCode, bool LocalReadinessPassed, bool ExternalGatesSatisfied,
    bool EligibleForAdmissionReview, bool ReadyForExternalReleaseReview, bool GrantsRuntimeAuthority)
{
    public static ResourceRuntimeReadinessAssessment NotReady(string reason) => new(ResourceRuntimeReadinessCondition.NotReady, reason, false, false, false, false, false);
}

public static class ResourceRuntimeAdmissionReadiness
{
    public const string ApplicationId = "APP-RSC";
    public const string ExpectedStage13CanonicalAiKillArtifactIdentity = "foundation/contracts/ai-kill-control-plane|1.0.0|sha256/BD36F1A4B8D49EC08347D3051461D788C636D03AF65B5616DE6B52B7A112B770|evidence:foundation:stage13:owner-closure:e59ccbba5786755b4e7f17a29810465ab0d4d6ed|compat:foundation-ai-kill-control-plane:v1|Foundation.Authority.AiKillControlPlaneContract|8453bd5961eb4ef3c9e35650f94d8c91ad0b81bc|Contract|foundation.authority|Published";
    public static IReadOnlyList<string> ExpectedStage13AiTargetIds { get; } = Array.AsReadOnly(
        new[] { "MSA-APP-RSC-01" }
            .Concat(Enumerable.Range(1, 3).Select(i => $"R-LSA-{i:00}"))
            .ToArray());

    public static ResourceRuntimeReadinessAssessment Assess(ResourceRuntimeReadinessInput? input)
    {
        if (input is null) return ResourceRuntimeReadinessAssessment.NotReady("P7_APP_RSC_INPUT_REQUIRED");
        if (!Valid(input.ApplicationId) || !Valid(input.EvaluationId) || !Valid(input.Environment) || !Valid(input.CoordinationEpochId) || !Valid(input.FoundationEnvelopeReference))
            return ResourceRuntimeReadinessAssessment.NotReady("P7_APP_RSC_IDENTITY_OR_FOUNDATION_REFERENCE_INCOMPLETE");
        if (!StringComparer.Ordinal.Equals(input.ApplicationId, ApplicationId)) return ResourceRuntimeReadinessAssessment.NotReady("P7_APP_RSC_APPLICATION_ID_MISMATCH");
        if (input.Evidence is null || !Valid(input.Evidence.ConfigurationEvidenceId) || !Valid(input.Evidence.HealthEvidenceId) ||
            !Valid(input.Evidence.RecoveryEvidenceId) || !Valid(input.Evidence.DeclarationEvidenceId))
            return ResourceRuntimeReadinessAssessment.NotReady("P7_APP_RSC_EVIDENCE_BINDING_INCOMPLETE");
        if (input.AttemptsToMintFoundationGrantOrTotalTruth) return ResourceRuntimeReadinessAssessment.NotReady("P7_APP_RSC_FOUNDATION_AUTHORITY_MINTING_PROHIBITED");
        if (!input.EvidenceIntegrityValid) return ResourceRuntimeReadinessAssessment.NotReady("P7_APP_RSC_EVIDENCE_NOT_VALID");
        if (!input.ConfigurationCurrentAndValid || !input.OperationalHealthReady || !input.RecoveryEvidenceComplete) return ResourceRuntimeReadinessAssessment.NotReady("P7_APP_RSC_CONFIG_HEALTH_OR_RECOVERY_NOT_READY");
        if (!input.CoordinationEpochCurrent || !input.PendingOutcomesReconciled || !input.SafeResourceFloorsEstablished) return ResourceRuntimeReadinessAssessment.NotReady("P7_APP_RSC_COORDINATION_TRUTH_NOT_READY");
        if (!input.DependencyDeclarationsComplete || !input.PermissionDeclarationsComplete || !input.RouteDeclarationsComplete) return ResourceRuntimeReadinessAssessment.NotReady("P7_APP_RSC_DECLARATIONS_INCOMPLETE");
        var releaseReady = !input.RecoveryReleaseReviewRequired || (input.RepairSucceeded && input.IndependentRecoveryValidated);
        if (!releaseReady) return ResourceRuntimeReadinessAssessment.NotReady("P7_APP_RSC_RELEASE_REVIEW_EVIDENCE_INCOMPLETE");
        if (!ExactTargetSet(input.Stage13RegisteredAiTargetIds))
            return new(ResourceRuntimeReadinessCondition.LocalReadyExternalAuthorityPending, "P7_APP_RSC_STAGE13_AI_TARGET_SET_PENDING_OR_MISMATCH", true, false, false, input.RecoveryReleaseReviewRequired, false);
        if (!RuntimeIdentityBound(input.ApplicationRuntimeInstanceId, input.ApplicationRuntimeGeneration, input.Stage13BoundRuntimeInstanceId, input.Stage13BoundRuntimeGeneration))
            return new(ResourceRuntimeReadinessCondition.LocalReadyExternalAuthorityPending, "P7_APP_RSC_STAGE13_AI_RUNTIME_IDENTITY_PENDING_OR_MISMATCH", true, false, false, input.RecoveryReleaseReviewRequired, false);
        if (!input.Stage13AiKillTargetRegistrationSatisfied || !input.Stage13AiKillEnforcementBindingSatisfied || !input.CurrentFoundationAiReleaseSatisfied)
            return new(ResourceRuntimeReadinessCondition.LocalReadyExternalAuthorityPending, "P7_APP_RSC_STAGE13_AI_KILL_BINDING_OR_RELEASE_PENDING", true, false, false, input.RecoveryReleaseReviewRequired, false);
        if (!Valid(input.AiKillBindingEvidenceId)) return ResourceRuntimeReadinessAssessment.NotReady("P7_APP_RSC_STAGE13_AI_KILL_BINDING_EVIDENCE_INVALID");
        if (!StringComparer.Ordinal.Equals(input.Stage13CanonicalAiKillArtifactIdentity, ExpectedStage13CanonicalAiKillArtifactIdentity))
            return new(ResourceRuntimeReadinessCondition.LocalReadyExternalAuthorityPending, "P7_APP_RSC_STAGE13_CANONICAL_AI_KILL_ARTIFACT_PENDING_OR_MISMATCH", true, false, false, input.RecoveryReleaseReviewRequired, false);
        if (input.CanonicalFoundationResourceBindingRequired && !input.CanonicalFoundationResourceBindingSatisfied)
            return new(ResourceRuntimeReadinessCondition.LocalReadyExternalAuthorityPending, "P7_APP_RSC_CANONICAL_FOUNDATION_BINDING_PENDING", true, false, false, input.RecoveryReleaseReviewRequired, false);
        if (input.CanonicalFoundationResourceBindingSatisfied && (!input.Evidence.ExternalAuthorityEvidenceValidated || !Valid(input.Evidence.ExternalAuthorityEvidenceId)))
            return ResourceRuntimeReadinessAssessment.NotReady("P7_APP_RSC_EXTERNAL_BINDING_EVIDENCE_INVALID");
        return new(ResourceRuntimeReadinessCondition.EligibleForAdmissionReview, "P7_APP_RSC_ELIGIBLE_FOR_EXTERNAL_ADMISSION_REVIEW", true, true, true, input.RecoveryReleaseReviewRequired, false);
    }

    private static bool ExactTargetSet(IReadOnlyList<string>? actual)
        => actual is not null && actual.Count == ExpectedStage13AiTargetIds.Count && actual.Distinct(StringComparer.Ordinal).Count() == actual.Count && ExpectedStage13AiTargetIds.All(expected => actual.Contains(expected, StringComparer.Ordinal));
    private static bool RuntimeIdentityBound(string? currentInstanceId, long currentGeneration, string? boundInstanceId, long boundGeneration)
        => Valid(currentInstanceId) && Valid(boundInstanceId) && currentGeneration > 0 && boundGeneration > 0 && currentGeneration == boundGeneration && StringComparer.Ordinal.Equals(currentInstanceId, boundInstanceId);
    private static bool Valid(string? value) => !string.IsNullOrWhiteSpace(value) && StringComparer.Ordinal.Equals(value, value.Trim());
}
