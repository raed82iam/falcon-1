import test from 'node:test';
import assert from 'node:assert/strict';
import {
  OwnerUpdateAuthoritySource,
  evaluateStandingPreapprovalEligibility,
  validateOwnerUpdateProposal,
  isCurrentOwnerDisposition,
  isValidRollbackRequest,
  isValidRollbackTransition
} from '../src/contracts/owner-update-governance-v1.js';

const fingerprint='A'.repeat(64);
function proposal(overrides={}) {
  const base={
    ProposalId:'proposal-1', ProposalVersion:'1.0.0', ChangeIdentity:'change-1', MaterialFingerprintSha256:fingerprint,
    OwningApplicationIdentity:'fsats', ProducerAiIdentity:null, UpdateClass:'Maintenance', UpdateClassVersion:'1',
    ClassificationAuthoritySource:'GovernedApplicationClassifier', Impact:'Low', Environment:'Sandbox', RequestedLifecyclePhase:'AdoptionReview',
    AffectedScopes:['strategy-cache'],
    BehaviorImpact:{BusinessBehaviorChanges:false,RiskBehaviorChanges:false,ExecutionBehaviorChanges:false,SecurityBehaviorChanges:false,AuthorityBehaviorChanges:false,DeploymentBehaviorChanges:false},
    Evidence:{ClassificationEvidenceReference:'evidence/classification',TestEvidenceReference:'evidence/test',SandboxEvidenceReference:'evidence/sandbox',FsaReviewRequired:false,FsaReviewSatisfied:false,FsaEvidenceReference:null},
    PreviousStateIdentity:'state-before-1',LineageReference:'lineage-1',MateriallyChangesPriorProposal:false,SupersedesProposalId:null,
    RollbackPlan:{PlanId:'rollback-1',PlanVersion:'1',ProposalId:'proposal-1',ChangeIdentity:'change-1',PreviousStateIdentity:'state-before-1',TargetScopes:['strategy-cache'],FullRollbackSupported:true,PartialRollbackTargets:['strategy-cache'],Prerequisites:['snapshot-present'],KnownNonReversibleEffects:[],DataOrSchemaMigrationImplications:[],CompatibilityConstraints:[],Current:true,Compatible:true,Validated:true,ValidationEvidenceReference:'evidence/rollback-validation',ExpectedRollbackResult:'state-before-1',RecoveryObservationSteps:['verify-health'],EvidenceReference:'evidence/rollback'}
  };
  const merged={...base,...overrides};
  if (overrides.BehaviorImpact) merged.BehaviorImpact={...base.BehaviorImpact,...overrides.BehaviorImpact};
  if (overrides.Evidence) merged.Evidence={...base.Evidence,...overrides.Evidence};
  if (overrides.RollbackPlan) merged.RollbackPlan={...base.RollbackPlan,...overrides.RollbackPlan};
  return merged;
}
function policy(overrides={}) {
  return {PolicyId:'owner-policy-1',PolicyVersion:'4',AuthoritySource:'OwnerViaSharedWeb',AuthorityEvidenceReference:'authority/owner-policy-1',Rules:[{UpdateClass:'Maintenance',UpdateClassVersion:'1',AllowNonReversibleChange:false}],...overrides};
}

test('low-impact governed Maintenance with valid rollback is eligibility only, never Owner approval',()=>{
  const result=evaluateStandingPreapprovalEligibility(proposal(),policy());
  assert.equal(result.disposition,'STANDING_PREAPPROVAL_ELIGIBLE_FOR_OWNER_DECISION');
  assert.equal(result.proposalAcceptanceGranted,false);
  assert.equal(result.executionAuthorityGranted,false);
  assert.equal(result.deploymentAuthorityGranted,false);
  assert.equal(result.runtimeActivationAuthorityGranted,false);
});

test('producer self classification is invalid and cannot reach standing preapproval',()=>{
  const result=evaluateStandingPreapprovalEligibility(proposal({ClassificationAuthoritySource:'ProducerSelfClaim'}),policy());
  assert.equal(result.disposition,'MANUAL_OWNER_REVIEW_REQUIRED');
  assert.equal(result.reason,'INVALID_PROPOSAL_OR_ROLLBACK');
});

test('manual-floor classes cannot be weakened by Owner standing list',()=>{
  const p=proposal({UpdateClass:'RiskRuleChange'});
  const permissive=policy({Rules:[{UpdateClass:'RiskRuleChange',UpdateClassVersion:'1',AllowNonReversibleChange:true}]});
  assert.equal(evaluateStandingPreapprovalEligibility(p,permissive).disposition,'MANUAL_OWNER_REVIEW_REQUIRED');
});

test('execution-changing behavior cannot hide behind Maintenance label',()=>{
  const p=proposal({BehaviorImpact:{ExecutionBehaviorChanges:true}});
  const result=evaluateStandingPreapprovalEligibility(p,policy());
  assert.equal(result.disposition,'MANUAL_OWNER_REVIEW_REQUIRED');
  assert.equal(result.reason,'MATERIAL_OR_HIGH_IMPACT_CHANGE_REQUIRES_MANUAL_OWNER_REVIEW');
});

test('stale or unvalidated rollback plan invalidates proposal package',()=>{
  assert.equal(validateOwnerUpdateProposal(proposal({RollbackPlan:{Current:false}})).valid,false);
  assert.equal(validateOwnerUpdateProposal(proposal({RollbackPlan:{Validated:false}})).valid,false);
  assert.equal(validateOwnerUpdateProposal(proposal({RollbackPlan:{Compatible:false}})).valid,false);
});

test('AI self-development without required FSA evidence fails closed',()=>{
  const p=proposal({UpdateClass:'AiSelfDevelopment',ProducerAiIdentity:'ai/strategy-1',Evidence:{FsaReviewRequired:true,FsaReviewSatisfied:false,FsaEvidenceReference:null}});
  assert.equal(validateOwnerUpdateProposal(p).valid,false);
});

test('materially changed proposal must name superseded proposal',()=>{
  assert.equal(validateOwnerUpdateProposal(proposal({MateriallyChangesPriorProposal:true,SupersedesProposalId:null})).valid,false);
});

test('prior Owner disposition is invalid after proposal fingerprint or policy version changes',()=>{
  const p=proposal(); const pol=policy();
  const disposition={DecisionId:'decision-1',State:'OwnerAcceptedByStandingPolicy',ProposalId:p.ProposalId,ProposalVersion:p.ProposalVersion,ChangeIdentity:p.ChangeIdentity,MaterialFingerprintSha256:p.MaterialFingerprintSha256,PolicyId:pol.PolicyId,PolicyVersion:pol.PolicyVersion,AuthoritySource:'OwnerViaSharedWeb',AuthorityEvidenceReference:'authority/decision-1'};
  assert.equal(isCurrentOwnerDisposition(disposition,p,pol),true);
  assert.equal(isCurrentOwnerDisposition(disposition,{...p,MaterialFingerprintSha256:'B'.repeat(64)},pol),false);
  assert.equal(isCurrentOwnerDisposition(disposition,p,{...pol,PolicyVersion:'5'}),false);
});

test('rollback request must originate from OwnerViaSharedWeb and exact plan identity',()=>{
  const p=proposal();
  const request={RequestId:'rollback-request-1',ProposalId:p.ProposalId,ProposalVersion:p.ProposalVersion,ChangeIdentity:p.ChangeIdentity,PlanId:p.RollbackPlan.PlanId,PlanVersion:p.RollbackPlan.PlanVersion,AuthoritySource:OwnerUpdateAuthoritySource.OWNER_VIA_SHARED_WEB,AuthorityEvidenceReference:'authority/rollback',Mode:'Full',BoundedTargets:[]};
  assert.equal(isValidRollbackRequest(request,p),true);
  assert.equal(isValidRollbackRequest({...request,AuthoritySource:'ArtificialIntelligence'},p),false);
  assert.equal(isValidRollbackRequest({...request,PlanVersion:'old'},p),false);
});

test('rollback lifecycle rejects request-to-completed state skipping',()=>{
  assert.equal(isValidRollbackTransition('Received','Accepted'),true);
  assert.equal(isValidRollbackTransition('Accepted','ExecutionStarted'),true);
  assert.equal(isValidRollbackTransition('Received','ExecutionCompleted'),false);
  assert.equal(isValidRollbackTransition('ExecutionCompleted','PostRollbackValidationCompleted'),false);
});
