namespace Falcon.FSATS.FSAPMA.Application;

public enum FSAPMARuntimeReadinessCondition { NotReady, LocalReadyExternalAuthorityPending, EligibleForAdmissionReview }

public sealed record FSAPMARuntimeReadinessEvidence(
    string ConfigurationEvidenceId,
    string HealthEvidenceId,
    string RecoveryEvidenceId,
    string DeclarationEvidenceId,
    string? ExternalAuthorityEvidenceId,
    bool ExternalAuthorityEvidenceValidated);

public sealed record FSAPMARuntimeReadinessInput(
    string ApplicationId, string EvaluationId, string ProviderId, string ProviderAccountId, string Environment,
    string ServiceRole, string ApiInstanceId, string EndpointId, string CredentialReference,
    bool ConfigurationCurrentAndValid, bool OperationalHealthReady, bool RecoveryEvidenceComplete, bool ProviderContinuityReconciled,
    bool DependencyDeclarationsComplete, bool PermissionDeclarationsComplete, bool RouteDeclarationsComplete, bool EvidenceIntegrityValid,
    bool ContainsSecretBytes, bool OperationalProviderEgressRequired, bool OperationalProviderEgressAuthoritySatisfied,
    bool RecoveryReleaseReviewRequired, bool RepairSucceeded, bool IndependentRecoveryValidated,
    FSAPMARuntimeReadinessEvidence? Evidence)
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

public sealed record FSAPMARuntimeReadinessAssessment(
    FSAPMARuntimeReadinessCondition Condition, string ReasonCode, bool LocalReadinessPassed, bool ExternalGatesSatisfied,
    bool EligibleForAdmissionReview, bool ReadyForExternalReleaseReview, bool GrantsRuntimeAuthority)
{
    public static FSAPMARuntimeReadinessAssessment NotReady(string reason) => new(FSAPMARuntimeReadinessCondition.NotReady, reason, false, false, false, false, false);
}

public static class FSAPMARuntimeAdmissionReadiness
{
    public const string ApplicationId = "FSATS-FSAPMA";
    public const string ExpectedStage13CanonicalAiKillArtifactIdentity = "foundation/contracts/ai-kill-control-plane|1.0.0|sha256/BD36F1A4B8D49EC08347D3051461D788C636D03AF65B5616DE6B52B7A112B770|evidence:foundation:stage13:owner-closure:e59ccbba5786755b4e7f17a29810465ab0d4d6ed|compat:foundation-ai-kill-control-plane:v1|Foundation.Authority.AiKillControlPlaneContract|8453bd5961eb4ef3c9e35650f94d8c91ad0b81bc|Contract|foundation.authority|Published";
    public static IReadOnlyList<string> ExpectedStage13AiTargetIds { get; } = Array.AsReadOnly(
        new[] { "MSA-FSAPMA-01" }
            .Concat(Enumerable.Range(1, 6).Select(i => $"P-LSA-{i:00}"))
            .Concat(new[] { "CSA-P05-01" })
            .ToArray());

    public static FSAPMARuntimeReadinessAssessment Assess(FSAPMARuntimeReadinessInput? input)
    {
        if (input is null) return FSAPMARuntimeReadinessAssessment.NotReady("P7_FSAPMA_INPUT_REQUIRED");
        if (!Valid(input.ApplicationId) || !Valid(input.EvaluationId) || !Valid(input.ProviderId) || !Valid(input.ProviderAccountId) ||
            !Valid(input.Environment) || !Valid(input.ServiceRole) || !Valid(input.ApiInstanceId) || !Valid(input.EndpointId) || !Valid(input.CredentialReference))
            return FSAPMARuntimeReadinessAssessment.NotReady("P7_FSAPMA_ROUTE_IDENTITY_INCOMPLETE");
        if (!StringComparer.Ordinal.Equals(input.ApplicationId, ApplicationId)) return FSAPMARuntimeReadinessAssessment.NotReady("P7_FSAPMA_APPLICATION_ID_MISMATCH");
        if (input.Evidence is null || !Valid(input.Evidence.ConfigurationEvidenceId) || !Valid(input.Evidence.HealthEvidenceId) ||
            !Valid(input.Evidence.RecoveryEvidenceId) || !Valid(input.Evidence.DeclarationEvidenceId))
            return FSAPMARuntimeReadinessAssessment.NotReady("P7_FSAPMA_EVIDENCE_BINDING_INCOMPLETE");
        if (input.ContainsSecretBytes) return FSAPMARuntimeReadinessAssessment.NotReady("P7_FSAPMA_SECRET_BYTES_PROHIBITED");
        if (!input.EvidenceIntegrityValid) return FSAPMARuntimeReadinessAssessment.NotReady("P7_FSAPMA_EVIDENCE_NOT_VALID");
        if (!input.ConfigurationCurrentAndValid) return FSAPMARuntimeReadinessAssessment.NotReady("P7_FSAPMA_CONFIGURATION_NOT_CURRENT_VALID");
        if (!input.OperationalHealthReady) return FSAPMARuntimeReadinessAssessment.NotReady("P7_FSAPMA_HEALTH_NOT_READY");
        if (!input.RecoveryEvidenceComplete || !input.ProviderContinuityReconciled) return FSAPMARuntimeReadinessAssessment.NotReady("P7_FSAPMA_RECOVERY_OR_CONTINUITY_INCOMPLETE");
        if (!input.DependencyDeclarationsComplete || !input.PermissionDeclarationsComplete || !input.RouteDeclarationsComplete) return FSAPMARuntimeReadinessAssessment.NotReady("P7_FSAPMA_DECLARATIONS_INCOMPLETE");
        var releaseReady = !input.RecoveryReleaseReviewRequired || (input.RepairSucceeded && input.IndependentRecoveryValidated);
        if (!releaseReady) return FSAPMARuntimeReadinessAssessment.NotReady("P7_FSAPMA_RELEASE_REVIEW_EVIDENCE_INCOMPLETE");
        if (!ExactTargetSet(input.Stage13RegisteredAiTargetIds))
            return new(FSAPMARuntimeReadinessCondition.LocalReadyExternalAuthorityPending, "P7_FSAPMA_STAGE13_AI_TARGET_SET_PENDING_OR_MISMATCH", true, false, false, input.RecoveryReleaseReviewRequired, false);
        if (!RuntimeIdentityBound(input.ApplicationRuntimeInstanceId, input.ApplicationRuntimeGeneration, input.Stage13BoundRuntimeInstanceId, input.Stage13BoundRuntimeGeneration))
            return new(FSAPMARuntimeReadinessCondition.LocalReadyExternalAuthorityPending, "P7_FSAPMA_STAGE13_AI_RUNTIME_IDENTITY_PENDING_OR_MISMATCH", true, false, false, input.RecoveryReleaseReviewRequired, false);
        if (!input.Stage13AiKillTargetRegistrationSatisfied || !input.Stage13AiKillEnforcementBindingSatisfied || !input.CurrentFoundationAiReleaseSatisfied)
            return new(FSAPMARuntimeReadinessCondition.LocalReadyExternalAuthorityPending, "P7_FSAPMA_STAGE13_AI_KILL_BINDING_OR_RELEASE_PENDING", true, false, false, input.RecoveryReleaseReviewRequired, false);
        if (!Valid(input.AiKillBindingEvidenceId)) return FSAPMARuntimeReadinessAssessment.NotReady("P7_FSAPMA_STAGE13_AI_KILL_BINDING_EVIDENCE_INVALID");
        if (!StringComparer.Ordinal.Equals(input.Stage13CanonicalAiKillArtifactIdentity, ExpectedStage13CanonicalAiKillArtifactIdentity))
            return new(FSAPMARuntimeReadinessCondition.LocalReadyExternalAuthorityPending, "P7_FSAPMA_STAGE13_CANONICAL_AI_KILL_ARTIFACT_PENDING_OR_MISMATCH", true, false, false, input.RecoveryReleaseReviewRequired, false);
        if (input.OperationalProviderEgressRequired && !input.OperationalProviderEgressAuthoritySatisfied)
            return new(FSAPMARuntimeReadinessCondition.LocalReadyExternalAuthorityPending, "P7_FSAPMA_PROVIDER_EGRESS_AUTHORITY_PENDING", true, false, false, input.RecoveryReleaseReviewRequired, false);
        if (input.OperationalProviderEgressAuthoritySatisfied && (!input.Evidence.ExternalAuthorityEvidenceValidated || !Valid(input.Evidence.ExternalAuthorityEvidenceId)))
            return FSAPMARuntimeReadinessAssessment.NotReady("P7_FSAPMA_EXTERNAL_AUTHORITY_EVIDENCE_INVALID");
        return new(FSAPMARuntimeReadinessCondition.EligibleForAdmissionReview, "P7_FSAPMA_ELIGIBLE_FOR_EXTERNAL_ADMISSION_REVIEW", true, true, true, input.RecoveryReleaseReviewRequired, false);
    }

    private static bool ExactTargetSet(IReadOnlyList<string>? actual)
        => actual is not null && actual.Count == ExpectedStage13AiTargetIds.Count && actual.Distinct(StringComparer.Ordinal).Count() == actual.Count && ExpectedStage13AiTargetIds.All(expected => actual.Contains(expected, StringComparer.Ordinal));
    private static bool RuntimeIdentityBound(string? currentInstanceId, long currentGeneration, string? boundInstanceId, long boundGeneration)
        => Valid(currentInstanceId) && Valid(boundInstanceId) && currentGeneration > 0 && boundGeneration > 0 && currentGeneration == boundGeneration && StringComparer.Ordinal.Equals(currentInstanceId, boundInstanceId);
    private static bool Valid(string? value) => !string.IsNullOrWhiteSpace(value) && StringComparer.Ordinal.Equals(value, value.Trim());
}
