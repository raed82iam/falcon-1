namespace Falcon.FSATS.Trading.Contracts;

public enum WebOwnerUpdateClass
{
    Unknown = 0,
    Maintenance = 1,
    ModelRefresh = 2,
    ParameterTuning = 3,
    StrategyRevision = 4,
    DataSourceChange = 5,
    PresentationOnlySuggestion = 6,
    BusinessRuleChange = 7,
    RiskRuleChange = 8,
    ExecutionBehaviorChange = 9,
    AuthorityOrSecurityChange = 10,
    DeploymentOrAdoptionChange = 11,
    AiSelfDevelopment = 12
}

public enum WebOwnerUpdateReviewDisposition
{
    ManualOwnerReviewRequired = 0,
    StandingPreApprovalEligible = 1
}

public enum WebOwnerUpdateReviewFloor
{
    ManualOwnerReviewRequired = 0,
    EligibleForStandingPreApprovalEvaluation = 1
}

public enum WebOwnerUpdateAuthoritySource
{
    Unspecified = 0,
    Application = 1,
    ArtificialIntelligence = 2,
    OwnerViaSharedWeb = 3
}

public enum WebOwnerClassificationAuthoritySource
{
    Unspecified = 0,
    ProducerSelfClaim = 1,
    GovernedApplicationClassifier = 2
}

public enum WebOwnerUpdateImpact
{
    Unknown = 0,
    Low = 1,
    Moderate = 2,
    High = 3,
    Critical = 4
}

public enum WebOwnerUpdateEnvironment
{
    Unknown = 0,
    Development = 1,
    Sandbox = 2,
    Test = 3,
    Paper = 4,
    Shadow = 5,
    TinyLive = 6,
    Live = 7
}

public enum WebOwnerRequestedLifecyclePhase
{
    Unknown = 0,
    ProposalReview = 1,
    SandboxValidation = 2,
    AdoptionReview = 3,
    DeploymentReview = 4,
    RuntimeActivationReview = 5
}

public enum WebOwnerRollbackRequestMode
{
    Full = 0,
    BoundedPartial = 1
}

public enum WebOwnerRollbackLifecycleState
{
    Received = 0,
    Accepted = 1,
    Rejected = 2,
    ExecutionStarted = 3,
    ExecutionCompleted = 4,
    ExecutionFailed = 5,
    PostRollbackValidationRequired = 6,
    PostRollbackValidationCompleted = 7,
    PostRollbackValidationFailed = 8
}

public enum WebOwnerDerivedDispositionState
{
    ManualReviewRequired = 0,
    OwnerAcceptedByStandingPolicy = 1,
    OwnerAcceptedManually = 2,
    OwnerRejected = 3
}

public sealed record WebOwnerUpdateBehaviorImpact(
    bool BusinessBehaviorChanges,
    bool RiskBehaviorChanges,
    bool ExecutionBehaviorChanges,
    bool SecurityBehaviorChanges,
    bool AuthorityBehaviorChanges,
    bool DeploymentBehaviorChanges);

public sealed record WebOwnerUpdateEvidenceBundle(
    string ClassificationEvidenceReference,
    string TestEvidenceReference,
    string SandboxEvidenceReference,
    bool FsaReviewRequired,
    bool FsaReviewSatisfied,
    string? FsaEvidenceReference);

public sealed record WebOwnerUpdateRollbackPlan(
    string PlanId,
    string PlanVersion,
    string ProposalId,
    string ChangeIdentity,
    string PreviousStateIdentity,
    IReadOnlyList<string> TargetScopes,
    bool FullRollbackSupported,
    IReadOnlyList<string> PartialRollbackTargets,
    IReadOnlyList<string> Prerequisites,
    IReadOnlyList<string> KnownNonReversibleEffects,
    IReadOnlyList<string> DataOrSchemaMigrationImplications,
    IReadOnlyList<string> CompatibilityConstraints,
    bool Current,
    bool Compatible,
    bool Validated,
    string ValidationEvidenceReference,
    string ExpectedRollbackResult,
    IReadOnlyList<string> RecoveryObservationSteps,
    string EvidenceReference);

public sealed record WebOwnerUpdateProposal(
    string ProposalId,
    string ProposalVersion,
    string ChangeIdentity,
    string MaterialFingerprintSha256,
    string OwningApplicationIdentity,
    string? ProducerAiIdentity,
    WebOwnerUpdateClass UpdateClass,
    string UpdateClassVersion,
    WebOwnerClassificationAuthoritySource ClassificationAuthoritySource,
    WebOwnerUpdateImpact Impact,
    WebOwnerUpdateEnvironment Environment,
    WebOwnerRequestedLifecyclePhase RequestedLifecyclePhase,
    IReadOnlyList<string> AffectedScopes,
    WebOwnerUpdateBehaviorImpact BehaviorImpact,
    WebOwnerUpdateEvidenceBundle Evidence,
    string PreviousStateIdentity,
    string LineageReference,
    bool MateriallyChangesPriorProposal,
    string? SupersedesProposalId,
    WebOwnerUpdateRollbackPlan RollbackPlan);

public sealed record WebOwnerStandingPreApprovalRule(
    WebOwnerUpdateClass UpdateClass,
    string UpdateClassVersion,
    bool AllowNonReversibleChange);

public sealed record WebOwnerStandingPreApprovalPolicySnapshot(
    string PolicyId,
    string PolicyVersion,
    WebOwnerUpdateAuthoritySource AuthoritySource,
    string AuthorityEvidenceReference,
    IReadOnlyList<WebOwnerStandingPreApprovalRule> Rules);

public sealed record WebOwnerUpdateReviewDecision(
    WebOwnerUpdateReviewDisposition Disposition,
    string Reason,
    bool ProposalAcceptanceGranted,
    bool ExecutionAuthorityGranted,
    bool DeploymentAuthorityGranted,
    bool RuntimeActivationAuthorityGranted);

public sealed record WebOwnerDerivedDisposition(
    string DecisionId,
    WebOwnerDerivedDispositionState State,
    string ProposalId,
    string ProposalVersion,
    string ChangeIdentity,
    string MaterialFingerprintSha256,
    string PolicyId,
    string PolicyVersion,
    WebOwnerUpdateAuthoritySource AuthoritySource,
    string AuthorityEvidenceReference);

public sealed record WebOwnerRollbackRequest(
    string RequestId,
    string ProposalId,
    string ProposalVersion,
    string ChangeIdentity,
    string PlanId,
    string PlanVersion,
    WebOwnerUpdateAuthoritySource AuthoritySource,
    string AuthorityEvidenceReference,
    WebOwnerRollbackRequestMode Mode,
    IReadOnlyList<string> BoundedTargets);

public sealed record WebOwnerRollbackResult(
    string ResultId,
    string RequestId,
    string ProposalId,
    string ProposalVersion,
    string ChangeIdentity,
    string PlanId,
    string PlanVersion,
    WebOwnerRollbackLifecycleState State,
    string RestoredFromStateIdentity,
    string ResultingStateIdentity,
    string HistoryReference,
    string EvidenceReference);

public static class WebOwnerUpdateTaxonomy
{
    public static WebOwnerUpdateReviewFloor GetReviewFloor(WebOwnerUpdateClass updateClass) =>
        updateClass switch
        {
            WebOwnerUpdateClass.Maintenance => WebOwnerUpdateReviewFloor.EligibleForStandingPreApprovalEvaluation,
            WebOwnerUpdateClass.ModelRefresh => WebOwnerUpdateReviewFloor.EligibleForStandingPreApprovalEvaluation,
            WebOwnerUpdateClass.ParameterTuning => WebOwnerUpdateReviewFloor.EligibleForStandingPreApprovalEvaluation,
            WebOwnerUpdateClass.PresentationOnlySuggestion => WebOwnerUpdateReviewFloor.EligibleForStandingPreApprovalEvaluation,
            _ => WebOwnerUpdateReviewFloor.ManualOwnerReviewRequired
        };
}

public static class WebOwnerUpdateGovernance
{
    private const string InvalidProposalReason = "INVALID_PROPOSAL_OR_ROLLBACK";
    private const string ManualReviewReason = "MANUAL_OWNER_REVIEW_REQUIRED";
    private const string HighImpactReason = "MATERIAL_OR_HIGH_IMPACT_CHANGE_REQUIRES_MANUAL_OWNER_REVIEW";
    private const string StandingPolicyReason = "OWNER_STANDING_POLICY_MATCH";

    public static WebOwnerUpdateReviewDecision Evaluate(
        WebOwnerUpdateProposal? proposal,
        WebOwnerStandingPreApprovalPolicySnapshot? standingPolicy)
    {
        if (!IsValidProposal(proposal))
        {
            return Manual(InvalidProposalReason);
        }

        if (WebOwnerUpdateTaxonomy.GetReviewFloor(proposal!.UpdateClass)
            != WebOwnerUpdateReviewFloor.EligibleForStandingPreApprovalEvaluation)
        {
            return Manual(ManualReviewReason);
        }

        if (proposal.Impact is not WebOwnerUpdateImpact.Low
            || HasHighImpactBehaviorChange(proposal.BehaviorImpact))
        {
            return Manual(HighImpactReason);
        }

        if (!IsValidStandingPolicy(standingPolicy))
        {
            return Manual(ManualReviewReason);
        }

        var rule = standingPolicy!.Rules.SingleOrDefault(candidate =>
            candidate.UpdateClass == proposal.UpdateClass
            && string.Equals(candidate.UpdateClassVersion, proposal.UpdateClassVersion, StringComparison.Ordinal));

        if (rule is null)
        {
            return Manual(ManualReviewReason);
        }

        var rollback = proposal.RollbackPlan;
        var hasNonReversibleEffects = rollback.KnownNonReversibleEffects.Count > 0;
        if ((!rollback.FullRollbackSupported && !hasNonReversibleEffects)
            || (!rollback.FullRollbackSupported && !rule.AllowNonReversibleChange)
            || (hasNonReversibleEffects && !rule.AllowNonReversibleChange))
        {
            return Manual(ManualReviewReason);
        }

        return new WebOwnerUpdateReviewDecision(
            WebOwnerUpdateReviewDisposition.StandingPreApprovalEligible,
            StandingPolicyReason,
            ProposalAcceptanceGranted: false,
            ExecutionAuthorityGranted: false,
            DeploymentAuthorityGranted: false,
            RuntimeActivationAuthorityGranted: false);
    }

    public static bool IsValidProposal(WebOwnerUpdateProposal? proposal)
    {
        if (proposal is null
            || string.IsNullOrWhiteSpace(proposal.ProposalId)
            || string.IsNullOrWhiteSpace(proposal.ProposalVersion)
            || string.IsNullOrWhiteSpace(proposal.ChangeIdentity)
            || !IsSha256Hex(proposal.MaterialFingerprintSha256)
            || string.IsNullOrWhiteSpace(proposal.OwningApplicationIdentity)
            || proposal.UpdateClass == WebOwnerUpdateClass.Unknown
            || string.IsNullOrWhiteSpace(proposal.UpdateClassVersion)
            || proposal.ClassificationAuthoritySource != WebOwnerClassificationAuthoritySource.GovernedApplicationClassifier
            || proposal.Impact == WebOwnerUpdateImpact.Unknown
            || proposal.Environment == WebOwnerUpdateEnvironment.Unknown
            || proposal.RequestedLifecyclePhase == WebOwnerRequestedLifecyclePhase.Unknown
            || proposal.AffectedScopes.Count == 0
            || !HasOnlyDistinctNonBlankValues(proposal.AffectedScopes)
            || string.IsNullOrWhiteSpace(proposal.PreviousStateIdentity)
            || string.IsNullOrWhiteSpace(proposal.LineageReference)
            || !IsValidEvidence(proposal.Evidence, proposal.UpdateClass, proposal.ProducerAiIdentity))
        {
            return false;
        }

        if (proposal.MateriallyChangesPriorProposal && string.IsNullOrWhiteSpace(proposal.SupersedesProposalId))
        {
            return false;
        }

        return IsValidRollbackPlan(
            proposal.RollbackPlan,
            proposal.ProposalId,
            proposal.ChangeIdentity,
            proposal.PreviousStateIdentity,
            proposal.AffectedScopes);
    }

    public static bool IsValidRollbackPlan(
        WebOwnerUpdateRollbackPlan? plan,
        string expectedProposalId,
        string expectedChangeIdentity,
        string expectedPreviousStateIdentity,
        IReadOnlyList<string> affectedScopes)
    {
        if (plan is null
            || string.IsNullOrWhiteSpace(plan.PlanId)
            || string.IsNullOrWhiteSpace(plan.PlanVersion)
            || string.IsNullOrWhiteSpace(plan.ProposalId)
            || string.IsNullOrWhiteSpace(plan.ChangeIdentity)
            || string.IsNullOrWhiteSpace(plan.PreviousStateIdentity)
            || plan.TargetScopes.Count == 0
            || !HasOnlyDistinctNonBlankValues(plan.TargetScopes)
            || !HasOnlyDistinctNonBlankValues(plan.PartialRollbackTargets)
            || !HasOnlyDistinctNonBlankValues(plan.Prerequisites)
            || !HasOnlyDistinctNonBlankValues(plan.KnownNonReversibleEffects)
            || !HasOnlyDistinctNonBlankValues(plan.DataOrSchemaMigrationImplications)
            || !HasOnlyDistinctNonBlankValues(plan.CompatibilityConstraints)
            || !plan.Current
            || !plan.Compatible
            || !plan.Validated
            || string.IsNullOrWhiteSpace(plan.ValidationEvidenceReference)
            || string.IsNullOrWhiteSpace(plan.ExpectedRollbackResult)
            || plan.RecoveryObservationSteps.Count == 0
            || !HasOnlyDistinctNonBlankValues(plan.RecoveryObservationSteps)
            || string.IsNullOrWhiteSpace(plan.EvidenceReference)
            || !string.Equals(plan.ProposalId, expectedProposalId, StringComparison.Ordinal)
            || !string.Equals(plan.ChangeIdentity, expectedChangeIdentity, StringComparison.Ordinal)
            || !string.Equals(plan.PreviousStateIdentity, expectedPreviousStateIdentity, StringComparison.Ordinal))
        {
            return false;
        }

        return affectedScopes.All(scope => plan.TargetScopes.Contains(scope, StringComparer.Ordinal));
    }

    public static bool IsValidStandingPolicy(WebOwnerStandingPreApprovalPolicySnapshot? policy)
    {
        if (policy is null
            || string.IsNullOrWhiteSpace(policy.PolicyId)
            || string.IsNullOrWhiteSpace(policy.PolicyVersion)
            || policy.AuthoritySource != WebOwnerUpdateAuthoritySource.OwnerViaSharedWeb
            || string.IsNullOrWhiteSpace(policy.AuthorityEvidenceReference)
            || policy.Rules.Count == 0)
        {
            return false;
        }

        if (policy.Rules.Any(rule =>
            rule.UpdateClass == WebOwnerUpdateClass.Unknown
            || string.IsNullOrWhiteSpace(rule.UpdateClassVersion)))
        {
            return false;
        }

        return policy.Rules
            .Select(rule => $"{(int)rule.UpdateClass}:{rule.UpdateClassVersion}")
            .Distinct(StringComparer.Ordinal)
            .Count() == policy.Rules.Count;
    }

    public static bool IsCurrentOwnerDispositionForProposal(
        WebOwnerDerivedDisposition? disposition,
        WebOwnerUpdateProposal? proposal,
        WebOwnerStandingPreApprovalPolicySnapshot? standingPolicy)
    {
        return disposition is not null
            && IsValidProposal(proposal)
            && IsValidStandingPolicy(standingPolicy)
            && !string.IsNullOrWhiteSpace(disposition.DecisionId)
            && disposition.AuthoritySource == WebOwnerUpdateAuthoritySource.OwnerViaSharedWeb
            && !string.IsNullOrWhiteSpace(disposition.AuthorityEvidenceReference)
            && string.Equals(disposition.ProposalId, proposal!.ProposalId, StringComparison.Ordinal)
            && string.Equals(disposition.ProposalVersion, proposal.ProposalVersion, StringComparison.Ordinal)
            && string.Equals(disposition.ChangeIdentity, proposal.ChangeIdentity, StringComparison.Ordinal)
            && string.Equals(disposition.MaterialFingerprintSha256, proposal.MaterialFingerprintSha256, StringComparison.Ordinal)
            && string.Equals(disposition.PolicyId, standingPolicy!.PolicyId, StringComparison.Ordinal)
            && string.Equals(disposition.PolicyVersion, standingPolicy.PolicyVersion, StringComparison.Ordinal);
    }

    public static bool IsValidRollbackRequest(
        WebOwnerRollbackRequest? request,
        WebOwnerUpdateProposal? proposal)
    {
        if (request is null
            || !IsValidProposal(proposal)
            || string.IsNullOrWhiteSpace(request.RequestId)
            || request.AuthoritySource != WebOwnerUpdateAuthoritySource.OwnerViaSharedWeb
            || string.IsNullOrWhiteSpace(request.AuthorityEvidenceReference)
            || !string.Equals(request.ProposalId, proposal!.ProposalId, StringComparison.Ordinal)
            || !string.Equals(request.ProposalVersion, proposal.ProposalVersion, StringComparison.Ordinal)
            || !string.Equals(request.ChangeIdentity, proposal.ChangeIdentity, StringComparison.Ordinal)
            || !string.Equals(request.PlanId, proposal.RollbackPlan.PlanId, StringComparison.Ordinal)
            || !string.Equals(request.PlanVersion, proposal.RollbackPlan.PlanVersion, StringComparison.Ordinal))
        {
            return false;
        }

        if (request.Mode == WebOwnerRollbackRequestMode.Full)
        {
            return proposal.RollbackPlan.FullRollbackSupported && request.BoundedTargets.Count == 0;
        }

        if (request.Mode != WebOwnerRollbackRequestMode.BoundedPartial
            || request.BoundedTargets.Count == 0
            || !HasOnlyDistinctNonBlankValues(request.BoundedTargets))
        {
            return false;
        }

        return request.BoundedTargets.All(target =>
            proposal.RollbackPlan.PartialRollbackTargets.Contains(target, StringComparer.Ordinal));
    }

    public static bool IsValidRollbackResult(
        WebOwnerRollbackResult? result,
        WebOwnerRollbackRequest? request,
        WebOwnerUpdateProposal? proposal)
    {
        return result is not null
            && IsValidRollbackRequest(request, proposal)
            && !string.IsNullOrWhiteSpace(result.ResultId)
            && !string.IsNullOrWhiteSpace(result.ResultingStateIdentity)
            && !string.IsNullOrWhiteSpace(result.HistoryReference)
            && !string.IsNullOrWhiteSpace(result.EvidenceReference)
            && string.Equals(result.RequestId, request!.RequestId, StringComparison.Ordinal)
            && string.Equals(result.ProposalId, proposal!.ProposalId, StringComparison.Ordinal)
            && string.Equals(result.ProposalVersion, proposal.ProposalVersion, StringComparison.Ordinal)
            && string.Equals(result.ChangeIdentity, proposal.ChangeIdentity, StringComparison.Ordinal)
            && string.Equals(result.PlanId, proposal.RollbackPlan.PlanId, StringComparison.Ordinal)
            && string.Equals(result.PlanVersion, proposal.RollbackPlan.PlanVersion, StringComparison.Ordinal)
            && string.Equals(result.RestoredFromStateIdentity, proposal.PreviousStateIdentity, StringComparison.Ordinal);
    }

    public static bool IsValidRollbackStateTransition(
        WebOwnerRollbackLifecycleState from,
        WebOwnerRollbackLifecycleState to) =>
        (from, to) switch
        {
            (WebOwnerRollbackLifecycleState.Received, WebOwnerRollbackLifecycleState.Accepted) => true,
            (WebOwnerRollbackLifecycleState.Received, WebOwnerRollbackLifecycleState.Rejected) => true,
            (WebOwnerRollbackLifecycleState.Accepted, WebOwnerRollbackLifecycleState.ExecutionStarted) => true,
            (WebOwnerRollbackLifecycleState.ExecutionStarted, WebOwnerRollbackLifecycleState.ExecutionCompleted) => true,
            (WebOwnerRollbackLifecycleState.ExecutionStarted, WebOwnerRollbackLifecycleState.ExecutionFailed) => true,
            (WebOwnerRollbackLifecycleState.ExecutionCompleted, WebOwnerRollbackLifecycleState.PostRollbackValidationRequired) => true,
            (WebOwnerRollbackLifecycleState.PostRollbackValidationRequired, WebOwnerRollbackLifecycleState.PostRollbackValidationCompleted) => true,
            (WebOwnerRollbackLifecycleState.PostRollbackValidationRequired, WebOwnerRollbackLifecycleState.PostRollbackValidationFailed) => true,
            _ => false
        };

    private static bool IsValidEvidence(
        WebOwnerUpdateEvidenceBundle evidence,
        WebOwnerUpdateClass updateClass,
        string? producerAiIdentity)
    {
        if (string.IsNullOrWhiteSpace(evidence.ClassificationEvidenceReference)
            || string.IsNullOrWhiteSpace(evidence.TestEvidenceReference)
            || string.IsNullOrWhiteSpace(evidence.SandboxEvidenceReference))
        {
            return false;
        }

        var aiSelfDevelopment = updateClass == WebOwnerUpdateClass.AiSelfDevelopment
            || !string.IsNullOrWhiteSpace(producerAiIdentity) && evidence.FsaReviewRequired;

        if (!aiSelfDevelopment)
        {
            return true;
        }

        return evidence.FsaReviewRequired
            && evidence.FsaReviewSatisfied
            && !string.IsNullOrWhiteSpace(evidence.FsaEvidenceReference);
    }

    private static bool HasHighImpactBehaviorChange(WebOwnerUpdateBehaviorImpact impact) =>
        impact.BusinessBehaviorChanges
        || impact.RiskBehaviorChanges
        || impact.ExecutionBehaviorChanges
        || impact.SecurityBehaviorChanges
        || impact.AuthorityBehaviorChanges
        || impact.DeploymentBehaviorChanges;

    private static WebOwnerUpdateReviewDecision Manual(string reason) =>
        new(
            WebOwnerUpdateReviewDisposition.ManualOwnerReviewRequired,
            reason,
            ProposalAcceptanceGranted: false,
            ExecutionAuthorityGranted: false,
            DeploymentAuthorityGranted: false,
            RuntimeActivationAuthorityGranted: false);

    private static bool HasOnlyDistinctNonBlankValues(IReadOnlyList<string> values)
    {
        if (values.Any(string.IsNullOrWhiteSpace))
        {
            return false;
        }

        return values.Distinct(StringComparer.Ordinal).Count() == values.Count;
    }

    private static bool IsSha256Hex(string value) =>
        value.Length == 64 && value.All(Uri.IsHexDigit);
}
