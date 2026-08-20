import test from 'node:test';
import assert from 'node:assert/strict';
import { validateOwnerProposalIngress, evaluateOwnerProposalIngress } from '../src/contracts/owner-update-proposal-boundary-v1.js';

function validProposal(overrides={}) {
  return {
    ProposalId:'proposal-1',ProposalVersion:'1',ChangeIdentity:'change-1',MaterialFingerprintSha256:'A'.repeat(64),OwningApplicationIdentity:'fsats',ProducerAiIdentity:'ai-1',UpdateClass:'Maintenance',UpdateClassVersion:'1',ClassificationAuthoritySource:'GovernedApplicationClassifier',Impact:'Low',Environment:'Sandbox',RequestedLifecyclePhase:'AdoptionReview',AffectedScopes:['scope-1'],BehaviorImpact:{BusinessBehaviorChanges:false,RiskBehaviorChanges:false,ExecutionBehaviorChanges:false,SecurityBehaviorChanges:false,AuthorityBehaviorChanges:false,DeploymentBehaviorChanges:false},Evidence:{ClassificationEvidenceReference:'classification',TestEvidenceReference:'test',SandboxEvidenceReference:'sandbox',FsaReviewRequired:false,FsaReviewSatisfied:false,FsaEvidenceReference:null},PreviousStateIdentity:'state-1',LineageReference:'lineage-1',MateriallyChangesPriorProposal:false,SupersedesProposalId:null,RollbackPlan:{PlanId:'plan-1',PlanVersion:'1',ProposalId:'proposal-1',ChangeIdentity:'change-1',PreviousStateIdentity:'state-1',TargetScopes:['scope-1'],FullRollbackSupported:true,PartialRollbackTargets:['scope-1'],Prerequisites:['snapshot'],KnownNonReversibleEffects:[],DataOrSchemaMigrationImplications:[],CompatibilityConstraints:[],Current:true,Compatible:true,Validated:true,ValidationEvidenceReference:'rollback-validation',ExpectedRollbackResult:'state-1',RecoveryObservationSteps:['observe'],EvidenceReference:'rollback-evidence'},...overrides
  };
}
const policy={PolicyId:'policy-1',PolicyVersion:'1',AuthoritySource:'OwnerViaSharedWeb',AuthorityEvidenceReference:'authority/policy',Rules:[{UpdateClass:'Maintenance',UpdateClassVersion:'1',AllowNonReversibleChange:false}]};

for (const claim of [
  ['AutoAccepted',true],['ownerApproved',true],['ROLLBACK_AUTHORIZED',true],['ProducerClaimsAutoAccept',true],['producerClaimsRollbackAuthority',true]
]) {
  test(`producer claim ${claim[0]} is rejected at Web ingress`,()=>{
    const result=validateOwnerProposalIngress(validProposal({[claim[0]]:claim[1]}));
    assert.equal(result.valid,false);
    assert.equal(result.reason,'APPLICATION_AI_SELF_APPROVAL_FORBIDDEN');
  });
}

test('self-approval claim can never produce standing preapproval eligibility',()=>{
  const result=evaluateOwnerProposalIngress(validProposal({AutoAccepted:true}),policy);
  assert.equal(result.disposition,'MANUAL_OWNER_REVIEW_REQUIRED');
  assert.equal(result.reason,'APPLICATION_AI_SELF_APPROVAL_FORBIDDEN');
});
