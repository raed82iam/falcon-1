namespace Falcon.FSATS.Trading.Application;

public enum TradingRuntimeReadinessCondition
{
    NotReady,
    LocalReadyExternalAuthorityPending,
    EligibleForAdmissionReview
}

public sealed record TradingRuntimeReadinessEvidence(
    string ConfigurationEvidenceId,
    string HealthEvidenceId,
    string RecoveryEvidenceId,
    string DeclarationEvidenceId,
    string? ExternalAuthorityEvidenceId,
    bool ExternalAuthorityEvidenceValidated);

public sealed record TradingRuntimeReadinessInput(
    string ApplicationId,
    string EvaluationId,
    string BrokerId,
    string BrokerAccountId,
    string Environment,
    long ConfigurationEpoch,
    bool ConfigurationCurrentAndValid,
    bool OperationalHealthReady,
    bool RecoveryEvidenceComplete,
    bool BrokerReconciliationComplete,
    bool ProtectionObligationsResolved,
    bool DependencyDeclarationsComplete,
    bool PermissionDeclarationsComplete,
    bool RouteDeclarationsComplete,
    bool EvidenceIntegrityValid,
    bool ContainsCustomerOrUserIdentity,
    bool ContainsSecretBytes,
    bool BrokerExecutionRouteRequired,
    bool BrokerExecutionAuthoritySatisfied,
    bool RecoveryReleaseReviewRequired,
    bool RepairSucceeded,
    bool IndependentRecoveryValidated,
    TradingRuntimeReadinessEvidence? Evidence)
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

public sealed record TradingRuntimeReadinessAssessment(
    TradingRuntimeReadinessCondition Condition,
    string ReasonCode,
    bool LocalReadinessPassed,
    bool ExternalGatesSatisfied,
    bool EligibleForAdmissionReview,
    bool ReadyForExternalReleaseReview,
    bool GrantsRuntimeAuthority)
{
    public static TradingRuntimeReadinessAssessment NotReady(string reason) =>
        new(TradingRuntimeReadinessCondition.NotReady, reason, false, false, false, false, false);
}

public static class TradingRuntimeAdmissionReadiness
{
    public const string ApplicationId = "FSATS-TRADING";
    public const string ExpectedStage13CanonicalAiKillArtifactIdentity = "foundation/contracts/ai-kill-control-plane|1.0.0|sha256/BD36F1A4B8D49EC08347D3051461D788C636D03AF65B5616DE6B52B7A112B770|evidence:foundation:stage13:owner-closure:e59ccbba5786755b4e7f17a29810465ab0d4d6ed|compat:foundation-ai-kill-control-plane:v1|Foundation.Authority.AiKillControlPlaneContract|8453bd5961eb4ef3c9e35650f94d8c91ad0b81bc|Contract|foundation.authority|Published";
    public static IReadOnlyList<string> ExpectedStage13AiTargetIds { get; } = Array.AsReadOnly(
        new[] { "MSA-TRADING-01" }
            .Concat(Enumerable.Range(1, 13).Select(i => $"T-LSA-{i:00}"))
            .Concat(new[] { "CSA-T05-01", "CSA-T06-01", "CSA-T12-01" })
            .ToArray());

    public static TradingRuntimeReadinessAssessment Assess(TradingRuntimeReadinessInput? input)
    {
        if (input is null)
            return TradingRuntimeReadinessAssessment.NotReady("P7_TRADING_INPUT_REQUIRED");

        if (!Valid(input.ApplicationId) || !Valid(input.EvaluationId) || !Valid(input.BrokerId) ||
            !Valid(input.BrokerAccountId) || !Valid(input.Environment) || input.ConfigurationEpoch < 0)
            return TradingRuntimeReadinessAssessment.NotReady("P7_TRADING_IDENTITY_OR_EPOCH_INVALID");

        if (!StringComparer.Ordinal.Equals(input.ApplicationId, ApplicationId))
            return TradingRuntimeReadinessAssessment.NotReady("P7_TRADING_APPLICATION_ID_MISMATCH");

        if (input.Evidence is null || !Valid(input.Evidence.ConfigurationEvidenceId) || !Valid(input.Evidence.HealthEvidenceId) ||
            !Valid(input.Evidence.RecoveryEvidenceId) || !Valid(input.Evidence.DeclarationEvidenceId))
            return TradingRuntimeReadinessAssessment.NotReady("P7_TRADING_EVIDENCE_BINDING_INCOMPLETE");

        if (input.ContainsCustomerOrUserIdentity)
            return TradingRuntimeReadinessAssessment.NotReady("P7_TRADING_CUSTOMER_USER_IDENTITY_PROHIBITED");

        if (input.ContainsSecretBytes)
            return TradingRuntimeReadinessAssessment.NotReady("P7_TRADING_SECRET_BYTES_PROHIBITED");

        if (!input.EvidenceIntegrityValid)
            return TradingRuntimeReadinessAssessment.NotReady("P7_TRADING_EVIDENCE_NOT_VALID");

        if (!input.ConfigurationCurrentAndValid)
            return TradingRuntimeReadinessAssessment.NotReady("P7_TRADING_CONFIGURATION_NOT_CURRENT_VALID");

        if (!input.OperationalHealthReady)
            return TradingRuntimeReadinessAssessment.NotReady("P7_TRADING_HEALTH_NOT_READY");

        if (!input.RecoveryEvidenceComplete)
            return TradingRuntimeReadinessAssessment.NotReady("P7_TRADING_RECOVERY_EVIDENCE_INCOMPLETE");

        if (!input.BrokerReconciliationComplete)
            return TradingRuntimeReadinessAssessment.NotReady("P7_TRADING_BROKER_RECONCILIATION_REQUIRED");

        if (!input.ProtectionObligationsResolved)
            return TradingRuntimeReadinessAssessment.NotReady("P7_TRADING_PROTECTION_OBLIGATION_UNRESOLVED");

        if (!input.DependencyDeclarationsComplete || !input.PermissionDeclarationsComplete || !input.RouteDeclarationsComplete)
            return TradingRuntimeReadinessAssessment.NotReady("P7_TRADING_DECLARATIONS_INCOMPLETE");

        var releaseReady = !input.RecoveryReleaseReviewRequired || (input.RepairSucceeded && input.IndependentRecoveryValidated);
        if (!releaseReady)
            return TradingRuntimeReadinessAssessment.NotReady("P7_TRADING_RELEASE_REVIEW_EVIDENCE_INCOMPLETE");

        if (!ExactTargetSet(input.Stage13RegisteredAiTargetIds))
            return new(TradingRuntimeReadinessCondition.LocalReadyExternalAuthorityPending, "P7_TRADING_STAGE13_AI_TARGET_SET_PENDING_OR_MISMATCH", true, false, false, input.RecoveryReleaseReviewRequired, false);

        if (!RuntimeIdentityBound(input.ApplicationRuntimeInstanceId, input.ApplicationRuntimeGeneration, input.Stage13BoundRuntimeInstanceId, input.Stage13BoundRuntimeGeneration))
            return new(TradingRuntimeReadinessCondition.LocalReadyExternalAuthorityPending, "P7_TRADING_STAGE13_AI_RUNTIME_IDENTITY_PENDING_OR_MISMATCH", true, false, false, input.RecoveryReleaseReviewRequired, false);

        if (!input.Stage13AiKillTargetRegistrationSatisfied || !input.Stage13AiKillEnforcementBindingSatisfied || !input.CurrentFoundationAiReleaseSatisfied)
            return new(TradingRuntimeReadinessCondition.LocalReadyExternalAuthorityPending, "P7_TRADING_STAGE13_AI_KILL_BINDING_OR_RELEASE_PENDING", true, false, false, input.RecoveryReleaseReviewRequired, false);

        if (!Valid(input.AiKillBindingEvidenceId))
            return TradingRuntimeReadinessAssessment.NotReady("P7_TRADING_STAGE13_AI_KILL_BINDING_EVIDENCE_INVALID");

        if (!StringComparer.Ordinal.Equals(input.Stage13CanonicalAiKillArtifactIdentity, ExpectedStage13CanonicalAiKillArtifactIdentity))
            return new(TradingRuntimeReadinessCondition.LocalReadyExternalAuthorityPending, "P7_TRADING_STAGE13_CANONICAL_AI_KILL_ARTIFACT_PENDING_OR_MISMATCH", true, false, false, input.RecoveryReleaseReviewRequired, false);

        if (input.BrokerExecutionRouteRequired && !input.BrokerExecutionAuthoritySatisfied)
            return new(TradingRuntimeReadinessCondition.LocalReadyExternalAuthorityPending, "P7_TRADING_BROKER_EXECUTION_AUTHORITY_PENDING", true, false, false, input.RecoveryReleaseReviewRequired, false);

        if (input.BrokerExecutionAuthoritySatisfied &&
            (!input.Evidence.ExternalAuthorityEvidenceValidated || !Valid(input.Evidence.ExternalAuthorityEvidenceId)))
            return TradingRuntimeReadinessAssessment.NotReady("P7_TRADING_EXTERNAL_AUTHORITY_EVIDENCE_INVALID");

        return new(TradingRuntimeReadinessCondition.EligibleForAdmissionReview, "P7_TRADING_ELIGIBLE_FOR_EXTERNAL_ADMISSION_REVIEW", true, true, true, input.RecoveryReleaseReviewRequired, false);
    }

    private static bool ExactTargetSet(IReadOnlyList<string>? actual)
        => actual is not null &&
           actual.Count == ExpectedStage13AiTargetIds.Count &&
           actual.Distinct(StringComparer.Ordinal).Count() == actual.Count &&
           ExpectedStage13AiTargetIds.All(expected => actual.Contains(expected, StringComparer.Ordinal));

    private static bool RuntimeIdentityBound(string? currentInstanceId, long currentGeneration, string? boundInstanceId, long boundGeneration)
        => Valid(currentInstanceId) && Valid(boundInstanceId) && currentGeneration > 0 && boundGeneration > 0 &&
           currentGeneration == boundGeneration && StringComparer.Ordinal.Equals(currentInstanceId, boundInstanceId);

    private static bool Valid(string? value) => !string.IsNullOrWhiteSpace(value) && StringComparer.Ordinal.Equals(value, value.Trim());
}
