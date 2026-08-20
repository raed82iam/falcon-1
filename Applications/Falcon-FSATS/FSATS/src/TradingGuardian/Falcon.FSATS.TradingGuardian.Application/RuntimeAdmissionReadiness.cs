namespace Falcon.FSATS.TradingGuardian.Application;

public enum GuardianRuntimeReadinessCondition { NotReady, LocalReadyExternalAuthorityPending, EligibleForAdmissionReview }

public sealed record GuardianRuntimeReadinessEvidence(
    string ConfigurationEvidenceId,
    string HealthEvidenceId,
    string RecoveryEvidenceId,
    string DeclarationEvidenceId,
    string? ExternalAuthorityEvidenceId,
    bool ExternalAuthorityEvidenceValidated);

public sealed record GuardianRuntimeReadinessInput(
    string ApplicationId, string EvaluationId, string ProtectionTargetKind, string ProtectionTargetId, string Environment,
    bool ConfigurationCurrentAndValid, bool OperationalHealthReady, bool RecoveryEvidenceComplete, bool ProtectionTruthReconciled,
    bool OutstandingCommandOutcomesResolved, bool ContainmentStateKnown, bool DependencyDeclarationsComplete,
    bool PermissionDeclarationsComplete, bool RouteDeclarationsComplete, bool EvidenceIntegrityValid, bool AttemptsSelfRelease,
    bool FoundationProtectionRouteRequired, bool FoundationProtectionRouteAuthoritySatisfied,
    bool RecoveryReleaseReviewRequired, bool RepairSucceeded, bool IndependentRecoveryValidated,
    GuardianRuntimeReadinessEvidence? Evidence)
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

public sealed record GuardianRuntimeReadinessAssessment(
    GuardianRuntimeReadinessCondition Condition, string ReasonCode, bool LocalReadinessPassed, bool ExternalGatesSatisfied,
    bool EligibleForAdmissionReview, bool ReadyForExternalReleaseReview, bool GrantsRuntimeAuthority)
{
    public static GuardianRuntimeReadinessAssessment NotReady(string reason) => new(GuardianRuntimeReadinessCondition.NotReady, reason, false, false, false, false, false);
}

public static class GuardianRuntimeAdmissionReadiness
{
    public const string ApplicationId = "FSATS-TRADING-GUARDIAN";
    public const string ExpectedStage13CanonicalAiKillArtifactIdentity = "foundation/contracts/ai-kill-control-plane|1.0.0|sha256/BD36F1A4B8D49EC08347D3051461D788C636D03AF65B5616DE6B52B7A112B770|evidence:foundation:stage13:owner-closure:e59ccbba5786755b4e7f17a29810465ab0d4d6ed|compat:foundation-ai-kill-control-plane:v1|Foundation.Authority.AiKillControlPlaneContract|8453bd5961eb4ef3c9e35650f94d8c91ad0b81bc|Contract|foundation.authority|Published";
    public static IReadOnlyList<string> ExpectedStage13AiTargetIds { get; } = Array.AsReadOnly(
        new[] { "MSA-GUARDIAN-01" }
            .Concat(Enumerable.Range(1, 4).Select(i => $"G-LSA-{i:00}"))
            .Concat(new[] { "CSA-G01-01" })
            .ToArray());

    public static GuardianRuntimeReadinessAssessment Assess(GuardianRuntimeReadinessInput? input)
    {
        if (input is null) return GuardianRuntimeReadinessAssessment.NotReady("P7_GUARDIAN_INPUT_REQUIRED");
        if (!Valid(input.ApplicationId) || !Valid(input.EvaluationId) || !Valid(input.ProtectionTargetKind) || !Valid(input.ProtectionTargetId) || !Valid(input.Environment))
            return GuardianRuntimeReadinessAssessment.NotReady("P7_GUARDIAN_TARGET_IDENTITY_INCOMPLETE");
        if (!StringComparer.Ordinal.Equals(input.ApplicationId, ApplicationId)) return GuardianRuntimeReadinessAssessment.NotReady("P7_GUARDIAN_APPLICATION_ID_MISMATCH");
        if (input.Evidence is null || !Valid(input.Evidence.ConfigurationEvidenceId) || !Valid(input.Evidence.HealthEvidenceId) ||
            !Valid(input.Evidence.RecoveryEvidenceId) || !Valid(input.Evidence.DeclarationEvidenceId))
            return GuardianRuntimeReadinessAssessment.NotReady("P7_GUARDIAN_EVIDENCE_BINDING_INCOMPLETE");
        if (input.AttemptsSelfRelease) return GuardianRuntimeReadinessAssessment.NotReady("P7_GUARDIAN_SELF_RELEASE_PROHIBITED");
        if (!input.EvidenceIntegrityValid) return GuardianRuntimeReadinessAssessment.NotReady("P7_GUARDIAN_EVIDENCE_NOT_VALID");
        if (!input.ConfigurationCurrentAndValid || !input.OperationalHealthReady) return GuardianRuntimeReadinessAssessment.NotReady("P7_GUARDIAN_CONFIG_OR_HEALTH_NOT_READY");
        if (!input.RecoveryEvidenceComplete || !input.ProtectionTruthReconciled || !input.OutstandingCommandOutcomesResolved || !input.ContainmentStateKnown)
            return GuardianRuntimeReadinessAssessment.NotReady("P7_GUARDIAN_PROTECTION_OR_RECOVERY_TRUTH_INCOMPLETE");
        if (!input.DependencyDeclarationsComplete || !input.PermissionDeclarationsComplete || !input.RouteDeclarationsComplete) return GuardianRuntimeReadinessAssessment.NotReady("P7_GUARDIAN_DECLARATIONS_INCOMPLETE");
        var releaseReady = !input.RecoveryReleaseReviewRequired || (input.RepairSucceeded && input.IndependentRecoveryValidated);
        if (!releaseReady) return GuardianRuntimeReadinessAssessment.NotReady("P7_GUARDIAN_RELEASE_REVIEW_EVIDENCE_INCOMPLETE");
        if (!ExactTargetSet(input.Stage13RegisteredAiTargetIds))
            return new(GuardianRuntimeReadinessCondition.LocalReadyExternalAuthorityPending, "P7_GUARDIAN_STAGE13_AI_TARGET_SET_PENDING_OR_MISMATCH", true, false, false, input.RecoveryReleaseReviewRequired, false);
        if (!RuntimeIdentityBound(input.ApplicationRuntimeInstanceId, input.ApplicationRuntimeGeneration, input.Stage13BoundRuntimeInstanceId, input.Stage13BoundRuntimeGeneration))
            return new(GuardianRuntimeReadinessCondition.LocalReadyExternalAuthorityPending, "P7_GUARDIAN_STAGE13_AI_RUNTIME_IDENTITY_PENDING_OR_MISMATCH", true, false, false, input.RecoveryReleaseReviewRequired, false);
        if (!input.Stage13AiKillTargetRegistrationSatisfied || !input.Stage13AiKillEnforcementBindingSatisfied || !input.CurrentFoundationAiReleaseSatisfied)
            return new(GuardianRuntimeReadinessCondition.LocalReadyExternalAuthorityPending, "P7_GUARDIAN_STAGE13_AI_KILL_BINDING_OR_RELEASE_PENDING", true, false, false, input.RecoveryReleaseReviewRequired, false);
        if (!Valid(input.AiKillBindingEvidenceId)) return GuardianRuntimeReadinessAssessment.NotReady("P7_GUARDIAN_STAGE13_AI_KILL_BINDING_EVIDENCE_INVALID");
        if (!StringComparer.Ordinal.Equals(input.Stage13CanonicalAiKillArtifactIdentity, ExpectedStage13CanonicalAiKillArtifactIdentity))
            return new(GuardianRuntimeReadinessCondition.LocalReadyExternalAuthorityPending, "P7_GUARDIAN_STAGE13_CANONICAL_AI_KILL_ARTIFACT_PENDING_OR_MISMATCH", true, false, false, input.RecoveryReleaseReviewRequired, false);
        if (input.FoundationProtectionRouteRequired && !input.FoundationProtectionRouteAuthoritySatisfied)
            return new(GuardianRuntimeReadinessCondition.LocalReadyExternalAuthorityPending, "P7_GUARDIAN_FOUNDATION_PROTECTION_ROUTE_PENDING", true, false, false, input.RecoveryReleaseReviewRequired, false);
        if (input.FoundationProtectionRouteAuthoritySatisfied && (!input.Evidence.ExternalAuthorityEvidenceValidated || !Valid(input.Evidence.ExternalAuthorityEvidenceId)))
            return GuardianRuntimeReadinessAssessment.NotReady("P7_GUARDIAN_EXTERNAL_AUTHORITY_EVIDENCE_INVALID");
        return new(GuardianRuntimeReadinessCondition.EligibleForAdmissionReview, "P7_GUARDIAN_ELIGIBLE_FOR_EXTERNAL_ADMISSION_REVIEW", true, true, true, input.RecoveryReleaseReviewRequired, false);
    }

    private static bool ExactTargetSet(IReadOnlyList<string>? actual)
        => actual is not null && actual.Count == ExpectedStage13AiTargetIds.Count && actual.Distinct(StringComparer.Ordinal).Count() == actual.Count && ExpectedStage13AiTargetIds.All(expected => actual.Contains(expected, StringComparer.Ordinal));
    private static bool RuntimeIdentityBound(string? currentInstanceId, long currentGeneration, string? boundInstanceId, long boundGeneration)
        => Valid(currentInstanceId) && Valid(boundInstanceId) && currentGeneration > 0 && boundGeneration > 0 && currentGeneration == boundGeneration && StringComparer.Ordinal.Equals(currentInstanceId, boundInstanceId);
    private static bool Valid(string? value) => !string.IsNullOrWhiteSpace(value) && StringComparer.Ordinal.Equals(value, value.Trim());
}
