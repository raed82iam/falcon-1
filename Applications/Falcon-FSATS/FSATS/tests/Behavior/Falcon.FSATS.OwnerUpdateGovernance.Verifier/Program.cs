using C = Falcon.FSATS.Trading.Contracts;

var failures = new List<string>();
var checks = 0;

void Check(bool condition, string description)
{
    checks++;
    if (!condition)
    {
        failures.Add(description);
    }
}

var behaviorImpact = new C.WebOwnerUpdateBehaviorImpact(
    BusinessBehaviorChanges: false,
    RiskBehaviorChanges: false,
    ExecutionBehaviorChanges: false,
    SecurityBehaviorChanges: false,
    AuthorityBehaviorChanges: false,
    DeploymentBehaviorChanges: false);

var evidence = new C.WebOwnerUpdateEvidenceBundle(
    "evidence/classification-1",
    "evidence/test-1",
    "evidence/sandbox-1",
    FsaReviewRequired: false,
    FsaReviewSatisfied: false,
    FsaEvidenceReference: null);

var rollbackPlan = new C.WebOwnerUpdateRollbackPlan(
    "rollback-plan-1",
    "1.0",
    "proposal-1",
    "change-1",
    "state/accepted/42",
    new[] { "configuration/strategy-selection" },
    FullRollbackSupported: true,
    new[] { "configuration/strategy-selection/weight" },
    new[] { "snapshot/available" },
    Array.Empty<string>(),
    Array.Empty<string>(),
    new[] { "compat/fsats-owner-update/v1" },
    Current: true,
    Compatible: true,
    Validated: true,
    "evidence/rollback-validation-1",
    "restore exact prior configuration state",
    new[] { "verify restored state identity", "observe governed health" },
    "evidence/rollback-plan-1");

var proposal = new C.WebOwnerUpdateProposal(
    "proposal-1",
    "1.0",
    "change-1",
    new string('A', 64),
    "FSATS.APPLICATION",
    ProducerAiIdentity: null,
    C.WebOwnerUpdateClass.Maintenance,
    "1.0",
    C.WebOwnerClassificationAuthoritySource.GovernedApplicationClassifier,
    C.WebOwnerUpdateImpact.Low,
    C.WebOwnerUpdateEnvironment.Sandbox,
    C.WebOwnerRequestedLifecyclePhase.ProposalReview,
    new[] { "configuration/strategy-selection" },
    behaviorImpact,
    evidence,
    "state/accepted/42",
    "lineage/proposal-1",
    MateriallyChangesPriorProposal: false,
    SupersedesProposalId: null,
    rollbackPlan);

var ownerRule = new C.WebOwnerStandingPreApprovalRule(
    C.WebOwnerUpdateClass.Maintenance,
    "1.0",
    AllowNonReversibleChange: false);
var ownerPolicy = new C.WebOwnerStandingPreApprovalPolicySnapshot(
    "owner-policy-1",
    "1.0",
    C.WebOwnerUpdateAuthoritySource.OwnerViaSharedWeb,
    "evidence/owner-policy-1",
    new[] { ownerRule });

Check(C.WebOwnerUpdateGovernance.IsValidProposal(proposal), "Complete governed proposal must validate.");
var eligible = C.WebOwnerUpdateGovernance.Evaluate(proposal, ownerPolicy);
Check(
    eligible.Disposition == C.WebOwnerUpdateReviewDisposition.StandingPreApprovalEligible,
    "Exact low-impact canonical class plus exact Owner-via-Web standing policy may be eligible.");
Check(
    !eligible.ProposalAcceptanceGranted
    && !eligible.ExecutionAuthorityGranted
    && !eligible.DeploymentAuthorityGranted
    && !eligible.RuntimeActivationAuthorityGranted,
    "Eligibility must not mint acceptance, execution, deployment or runtime authority.");

Check(
    C.WebOwnerUpdateTaxonomy.GetReviewFloor(C.WebOwnerUpdateClass.Maintenance)
        == C.WebOwnerUpdateReviewFloor.EligibleForStandingPreApprovalEvaluation,
    "Maintenance may enter standing-preapproval evaluation.");
Check(
    C.WebOwnerUpdateTaxonomy.GetReviewFloor(C.WebOwnerUpdateClass.StrategyRevision)
        == C.WebOwnerUpdateReviewFloor.ManualOwnerReviewRequired,
    "Strategy revision has a manual Owner-review floor.");
Check(
    C.WebOwnerUpdateTaxonomy.GetReviewFloor(C.WebOwnerUpdateClass.RiskRuleChange)
        == C.WebOwnerUpdateReviewFloor.ManualOwnerReviewRequired,
    "Risk-rule changes have a manual Owner-review floor.");
Check(
    C.WebOwnerUpdateTaxonomy.GetReviewFloor(C.WebOwnerUpdateClass.ExecutionBehaviorChange)
        == C.WebOwnerUpdateReviewFloor.ManualOwnerReviewRequired,
    "Execution-behavior changes have a manual Owner-review floor.");
Check(
    C.WebOwnerUpdateTaxonomy.GetReviewFloor(C.WebOwnerUpdateClass.AuthorityOrSecurityChange)
        == C.WebOwnerUpdateReviewFloor.ManualOwnerReviewRequired,
    "Authority/security changes have a manual Owner-review floor.");

var silence = C.WebOwnerUpdateGovernance.Evaluate(proposal, null);
Check(
    silence.Disposition == C.WebOwnerUpdateReviewDisposition.ManualOwnerReviewRequired,
    "Owner silence or absent policy must fail closed.");

var aiPolicy = ownerPolicy with
{
    AuthoritySource = C.WebOwnerUpdateAuthoritySource.ArtificialIntelligence,
    AuthorityEvidenceReference = "evidence/ai-self-policy"
};
Check(
    C.WebOwnerUpdateGovernance.Evaluate(proposal, aiPolicy).Disposition
        == C.WebOwnerUpdateReviewDisposition.ManualOwnerReviewRequired,
    "AI must not mint Owner standing-policy authority.");

var appPolicy = ownerPolicy with
{
    AuthoritySource = C.WebOwnerUpdateAuthoritySource.Application,
    AuthorityEvidenceReference = "evidence/application-self-policy"
};
Check(
    C.WebOwnerUpdateGovernance.Evaluate(proposal, appPolicy).Disposition
        == C.WebOwnerUpdateReviewDisposition.ManualOwnerReviewRequired,
    "Application must not mint Owner standing-policy authority.");

var selfClassified = proposal with
{
    ClassificationAuthoritySource = C.WebOwnerClassificationAuthoritySource.ProducerSelfClaim
};
Check(!C.WebOwnerUpdateGovernance.IsValidProposal(selfClassified), "Producer self-classification must fail closed.");

var unknownClass = proposal with { UpdateClass = C.WebOwnerUpdateClass.Unknown };
Check(!C.WebOwnerUpdateGovernance.IsValidProposal(unknownClass), "Unknown update class must fail closed.");

var highImpactMaintenance = proposal with
{
    Impact = C.WebOwnerUpdateImpact.High
};
Check(
    C.WebOwnerUpdateGovernance.Evaluate(highImpactMaintenance, ownerPolicy).Disposition
        == C.WebOwnerUpdateReviewDisposition.ManualOwnerReviewRequired,
    "High-impact proposal cannot inherit low-risk eligibility from its nominal class.");

var executionChangingMaintenance = proposal with
{
    BehaviorImpact = behaviorImpact with { ExecutionBehaviorChanges = true }
};
Check(
    C.WebOwnerUpdateGovernance.Evaluate(executionChangingMaintenance, ownerPolicy).Disposition
        == C.WebOwnerUpdateReviewDisposition.ManualOwnerReviewRequired,
    "Execution behavior change cannot be downgraded by a maintenance label.");

var missingAffectedScope = proposal with { AffectedScopes = Array.Empty<string>() };
Check(!C.WebOwnerUpdateGovernance.IsValidProposal(missingAffectedScope), "Affected scope is mandatory.");

var missingSandboxEvidence = proposal with
{
    Evidence = evidence with { SandboxEvidenceReference = string.Empty }
};
Check(!C.WebOwnerUpdateGovernance.IsValidProposal(missingSandboxEvidence), "Sandbox evidence is mandatory.");

var staleRollback = proposal with
{
    RollbackPlan = rollbackPlan with { Current = false }
};
Check(!C.WebOwnerUpdateGovernance.IsValidProposal(staleRollback), "Stale rollback plan must fail closed.");

var incompatibleRollback = proposal with
{
    RollbackPlan = rollbackPlan with { Compatible = false }
};
Check(!C.WebOwnerUpdateGovernance.IsValidProposal(incompatibleRollback), "Incompatible rollback plan must fail closed.");

var unvalidatedRollback = proposal with
{
    RollbackPlan = rollbackPlan with { Validated = false }
};
Check(!C.WebOwnerUpdateGovernance.IsValidProposal(unvalidatedRollback), "Unvalidated rollback plan must fail closed.");

var mismatchedRollbackState = proposal with
{
    RollbackPlan = rollbackPlan with { PreviousStateIdentity = "state/accepted/wrong" }
};
Check(!C.WebOwnerUpdateGovernance.IsValidProposal(mismatchedRollbackState), "Rollback must bind exact previous state.");

var duplicatePartialTargets = proposal with
{
    RollbackPlan = rollbackPlan with
    {
        PartialRollbackTargets = new[]
        {
            "configuration/strategy-selection/weight",
            "configuration/strategy-selection/weight"
        }
    }
};
Check(!C.WebOwnerUpdateGovernance.IsValidProposal(duplicatePartialTargets), "Partial rollback targets must be unique.");

var nonReversiblePlan = rollbackPlan with
{
    FullRollbackSupported = false,
    KnownNonReversibleEffects = new[] { "external irreversible side effect" }
};
var nonReversibleProposal = proposal with { RollbackPlan = nonReversiblePlan };
Check(
    C.WebOwnerUpdateGovernance.Evaluate(nonReversibleProposal, ownerPolicy).Disposition
        == C.WebOwnerUpdateReviewDisposition.ManualOwnerReviewRequired,
    "Non-reversible update requires manual review unless exact Owner policy permits it.");

var explicitNonReversiblePolicy = ownerPolicy with
{
    Rules = new[] { ownerRule with { AllowNonReversibleChange = true } }
};
Check(
    C.WebOwnerUpdateGovernance.Evaluate(nonReversibleProposal, explicitNonReversiblePolicy).Disposition
        == C.WebOwnerUpdateReviewDisposition.StandingPreApprovalEligible,
    "Exact Owner-via-Web policy may explicitly permit the exact non-reversible class for eligibility evaluation.");

var materialChangeWithoutSupersession = proposal with
{
    MateriallyChangesPriorProposal = true,
    SupersedesProposalId = null
};
Check(!C.WebOwnerUpdateGovernance.IsValidProposal(materialChangeWithoutSupersession), "Materially changed proposal must identify superseded proposal.");

var aiSelfDevelopmentEvidence = evidence with
{
    FsaReviewRequired = true,
    FsaReviewSatisfied = false,
    FsaEvidenceReference = null
};
var aiSelfDevelopment = proposal with
{
    ProducerAiIdentity = "AI/CSA-1",
    UpdateClass = C.WebOwnerUpdateClass.AiSelfDevelopment,
    Evidence = aiSelfDevelopmentEvidence
};
Check(!C.WebOwnerUpdateGovernance.IsValidProposal(aiSelfDevelopment), "AI self-development without satisfied FSA evidence must fail closed.");

var ownerDisposition = new C.WebOwnerDerivedDisposition(
    "owner-decision-1",
    C.WebOwnerDerivedDispositionState.OwnerAcceptedByStandingPolicy,
    proposal.ProposalId,
    proposal.ProposalVersion,
    proposal.ChangeIdentity,
    proposal.MaterialFingerprintSha256,
    ownerPolicy.PolicyId,
    ownerPolicy.PolicyVersion,
    C.WebOwnerUpdateAuthoritySource.OwnerViaSharedWeb,
    "evidence/owner-decision-1");
Check(
    C.WebOwnerUpdateGovernance.IsCurrentOwnerDispositionForProposal(ownerDisposition, proposal, ownerPolicy),
    "Returned Owner-derived disposition must bind exact proposal/change/fingerprint and current policy.");
Check(
    !C.WebOwnerUpdateGovernance.IsCurrentOwnerDispositionForProposal(
        ownerDisposition with { MaterialFingerprintSha256 = new string('B', 64) }, proposal, ownerPolicy),
    "Materially different proposal fingerprint must invalidate prior Owner disposition.");
Check(
    !C.WebOwnerUpdateGovernance.IsCurrentOwnerDispositionForProposal(
        ownerDisposition with { PolicyVersion = "0.9" }, proposal, ownerPolicy),
    "Stale Owner policy version must invalidate prior policy matching.");

var fullRollbackRequest = new C.WebOwnerRollbackRequest(
    "rollback-request-1",
    proposal.ProposalId,
    proposal.ProposalVersion,
    proposal.ChangeIdentity,
    proposal.RollbackPlan.PlanId,
    proposal.RollbackPlan.PlanVersion,
    C.WebOwnerUpdateAuthoritySource.OwnerViaSharedWeb,
    "evidence/owner-rollback-1",
    C.WebOwnerRollbackRequestMode.Full,
    Array.Empty<string>());
Check(
    C.WebOwnerUpdateGovernance.IsValidRollbackRequest(fullRollbackRequest, proposal),
    "Exact Owner-via-Web full rollback request must validate.");

Check(
    !C.WebOwnerUpdateGovernance.IsValidRollbackRequest(
        fullRollbackRequest with
        {
            RequestId = "rollback-request-app",
            AuthoritySource = C.WebOwnerUpdateAuthoritySource.Application,
            AuthorityEvidenceReference = "evidence/application-rollback"
        }, proposal),
    "Application must not mint rollback-command authority.");
Check(
    !C.WebOwnerUpdateGovernance.IsValidRollbackRequest(
        fullRollbackRequest with
        {
            RequestId = "rollback-request-ai",
            AuthoritySource = C.WebOwnerUpdateAuthoritySource.ArtificialIntelligence,
            AuthorityEvidenceReference = "evidence/ai-rollback"
        }, proposal),
    "AI must not mint rollback-command authority.");

var partialRollbackRequest = fullRollbackRequest with
{
    RequestId = "rollback-request-partial",
    Mode = C.WebOwnerRollbackRequestMode.BoundedPartial,
    BoundedTargets = new[] { "configuration/strategy-selection/weight" }
};
Check(
    C.WebOwnerUpdateGovernance.IsValidRollbackRequest(partialRollbackRequest, proposal),
    "Exact bounded partial Owner rollback request must validate.");
Check(
    !C.WebOwnerUpdateGovernance.IsValidRollbackRequest(
        partialRollbackRequest with
        {
            RequestId = "rollback-request-outside-plan",
            BoundedTargets = new[] { "configuration/not-declared" }
        }, proposal),
    "Partial rollback outside declared plan targets must fail closed.");

var rollbackResult = new C.WebOwnerRollbackResult(
    "rollback-result-1",
    fullRollbackRequest.RequestId,
    proposal.ProposalId,
    proposal.ProposalVersion,
    proposal.ChangeIdentity,
    proposal.RollbackPlan.PlanId,
    proposal.RollbackPlan.PlanVersion,
    C.WebOwnerRollbackLifecycleState.Received,
    proposal.PreviousStateIdentity,
    "state/restored/pending",
    "history/rollback-request-1",
    "evidence/rollback-result-1");
Check(
    C.WebOwnerUpdateGovernance.IsValidRollbackResult(rollbackResult, fullRollbackRequest, proposal),
    "Rollback result must preserve exact request/proposal/plan/state lineage.");
Check(
    !C.WebOwnerUpdateGovernance.IsValidRollbackResult(
        rollbackResult with { RequestId = "rollback-request-other" }, fullRollbackRequest, proposal),
    "Mismatched rollback-result request identity must fail closed.");
Check(
    !C.WebOwnerUpdateGovernance.IsValidRollbackResult(
        rollbackResult with { RestoredFromStateIdentity = "state/unknown" }, fullRollbackRequest, proposal),
    "Rollback result must preserve exact pre-change state lineage.");

Check(
    C.WebOwnerUpdateGovernance.IsValidRollbackStateTransition(
        C.WebOwnerRollbackLifecycleState.Received,
        C.WebOwnerRollbackLifecycleState.Accepted),
    "Rollback lifecycle must distinguish request receipt from acceptance.");
Check(
    C.WebOwnerUpdateGovernance.IsValidRollbackStateTransition(
        C.WebOwnerRollbackLifecycleState.Received,
        C.WebOwnerRollbackLifecycleState.Rejected),
    "Rollback lifecycle must distinguish request receipt from rejection.");
Check(
    C.WebOwnerUpdateGovernance.IsValidRollbackStateTransition(
        C.WebOwnerRollbackLifecycleState.Accepted,
        C.WebOwnerRollbackLifecycleState.ExecutionStarted),
    "Rollback lifecycle must distinguish acceptance from execution start.");
Check(
    C.WebOwnerUpdateGovernance.IsValidRollbackStateTransition(
        C.WebOwnerRollbackLifecycleState.ExecutionStarted,
        C.WebOwnerRollbackLifecycleState.ExecutionCompleted),
    "Rollback lifecycle must distinguish execution start from completion.");
Check(
    C.WebOwnerUpdateGovernance.IsValidRollbackStateTransition(
        C.WebOwnerRollbackLifecycleState.ExecutionStarted,
        C.WebOwnerRollbackLifecycleState.ExecutionFailed),
    "Rollback lifecycle must represent execution failure.");
Check(
    C.WebOwnerUpdateGovernance.IsValidRollbackStateTransition(
        C.WebOwnerRollbackLifecycleState.ExecutionCompleted,
        C.WebOwnerRollbackLifecycleState.PostRollbackValidationRequired),
    "Rollback lifecycle must preserve post-rollback validation requirement.");
Check(
    !C.WebOwnerUpdateGovernance.IsValidRollbackStateTransition(
        C.WebOwnerRollbackLifecycleState.Received,
        C.WebOwnerRollbackLifecycleState.ExecutionCompleted),
    "Rollback lifecycle must reject skipped acceptance/execution states.");

if (failures.Count > 0)
{
    Console.Error.WriteLine($"OWNER UPDATE GOVERNANCE VERIFIER: FAIL ({failures.Count}/{checks})");
    foreach (var failure in failures)
    {
        Console.Error.WriteLine($" - {failure}");
    }

    return 1;
}

Console.WriteLine($"OWNER UPDATE GOVERNANCE VERIFIER: PASS ({checks}/{checks})");
return 0;
