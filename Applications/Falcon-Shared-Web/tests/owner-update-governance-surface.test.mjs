import test from 'node:test';
import assert from 'node:assert/strict';
import { createOwnerUpdateGovernanceFeature } from '../src/features/owner-approvals/owner-update-governance.js';

const workspace=body=>body;
const proposal={
  ProposalId:'proposal-1',ProposalVersion:'1.0.0',ChangeIdentity:'change-1',MaterialFingerprintSha256:'A'.repeat(64),OwningApplicationIdentity:'fsats',ProducerAiIdentity:null,UpdateClass:'Maintenance',UpdateClassVersion:'1',ClassificationAuthoritySource:'GovernedApplicationClassifier',Impact:'Low',Environment:'Sandbox',RequestedLifecyclePhase:'AdoptionReview',AffectedScopes:['scope-1'],BehaviorImpact:{BusinessBehaviorChanges:false,RiskBehaviorChanges:false,ExecutionBehaviorChanges:false,SecurityBehaviorChanges:false,AuthorityBehaviorChanges:false,DeploymentBehaviorChanges:false},Evidence:{ClassificationEvidenceReference:'class-evidence',TestEvidenceReference:'test-evidence',SandboxEvidenceReference:'sandbox-evidence',FsaReviewRequired:false,FsaReviewSatisfied:false,FsaEvidenceReference:null},PreviousStateIdentity:'state-1',LineageReference:'lineage-1',MateriallyChangesPriorProposal:false,SupersedesProposalId:null,RollbackPlan:{PlanId:'plan-1',PlanVersion:'1',ProposalId:'proposal-1',ChangeIdentity:'change-1',PreviousStateIdentity:'state-1',TargetScopes:['scope-1'],FullRollbackSupported:true,PartialRollbackTargets:['scope-1'],Prerequisites:['snapshot'],KnownNonReversibleEffects:[],DataOrSchemaMigrationImplications:[],CompatibilityConstraints:[],Current:true,Compatible:true,Validated:true,ValidationEvidenceReference:'rollback-validation',ExpectedRollbackResult:'state-1',RecoveryObservationSteps:['observe'],EvidenceReference:'rollback-evidence'},policyId:'policy-1'};
const policy={policyId:'policy-1',policyVersion:'1',authoritySource:'OwnerViaSharedWeb',authorityEvidenceReference:'authority/policy',rules:[{updateClass:'Maintenance',updateClassVersion:'1',allowNonReversibleChange:false}],maximumRiskTier:1,expiry:'2099-09-01T00:00:00Z',evidenceReference:'policy-evidence'};

test('Owner governance page exposes three required sections and locks runtime decisions without transport',()=>{
  const html=createOwnerUpdateGovernanceFeature({language:()=> 'en',workspace,model:{transportAvailable:false,policies:[policy],proposals:[proposal],history:[]}}).ownerApprovalsPage();
  assert.match(html,/Standing Approvals/);
  assert.match(html,/Proposal Inbox/);
  assert.match(html,/Auto-Accepted History/);
  assert.match(html,/FCR-0241/);
  assert.match(html,/data-owner-proposal-evaluate="0" disabled aria-disabled="true"/);
  assert.match(html,/A real Auto Accept decision requires the Foundation contract/);
});

test('rollback request stays disabled when transport is unavailable even with history eligibility',()=>{
  const history=[{proposalId:'proposal-1',decisionId:'decision-1',decisionState:'AUTO_ACCEPTED',policyId:'policy-1',policyVersion:'1',planId:'plan-1',planVersion:'1',evidenceReference:'decision-evidence',rollbackAvailable:true}];
  const html=createOwnerUpdateGovernanceFeature({language:()=> 'en',workspace,model:{transportAvailable:false,policies:[],proposals:[],history}}).ownerApprovalsPage();
  assert.match(html,/data-owner-rollback-request="0" disabled aria-disabled="true"/);
  assert.match(html,/Rollback Order requires governed transport/);
});

test('Application proposal is described as eligibility, not local Auto Accept',()=>{
  const html=createOwnerUpdateGovernanceFeature({language:()=> 'en',workspace,model:{transportAvailable:true,policies:[policy],proposals:[proposal],history:[]}}).ownerApprovalsPage();
  assert.match(html,/This is eligibility only, not Auto Accept/);
  assert.match(html,/Eligible for Owner standing-policy decision/);
});

test('AUTO_REJECTED history is rendered with negative status tone',()=>{
  const history=[{proposalId:'proposal-2',decisionId:'decision-2',decisionState:'AUTO_REJECTED',policyId:'policy-1',policyVersion:'1',planId:'plan-1',planVersion:'1',evidenceReference:'decision-evidence',rollbackAvailable:false}];
  const html=createOwnerUpdateGovernanceFeature({language:()=> 'en',workspace,model:{transportAvailable:true,policies:[],proposals:[],history}}).ownerApprovalsPage();
  assert.match(html,/ds-status--negative/);
  assert.match(html,/AUTO_REJECTED/);
});

test('expired or explicitly non-current standing policy is never presented as active',()=>{
  const expired={...policy,expiry:'2020-01-01T00:00:00Z'};
  const html=createOwnerUpdateGovernanceFeature({language:()=> 'en',workspace,model:{transportAvailable:true,policies:[expired],proposals:[],history:[]}}).ownerApprovalsPage();
  assert.match(html,/Not current/);
  assert.doesNotMatch(html,/>Active</);
  assert.match(html,/data-owner-policy-edit="0" disabled aria-disabled="true"/);
  assert.match(html,/data-owner-policy-revoke="0" disabled aria-disabled="true"/);
});

test('manual review control stays fail-closed until a governed action path is bound',()=>{
  const html=createOwnerUpdateGovernanceFeature({language:()=> 'en',workspace,model:{transportAvailable:true,policies:[policy],proposals:[proposal],history:[]}}).ownerApprovalsPage();
  assert.match(html,/data-owner-proposal-review="0" disabled aria-disabled="true"/);
  assert.match(html,/governed manual-review action path is not bound yet/);
});

test('malformed proposal is rendered fail-closed instead of throwing the whole Owner page',()=>{
  const malformed={ ProposalId:'broken', policyId:'policy-1', autoAccepted:true };
  const html=createOwnerUpdateGovernanceFeature({language:()=> 'en',workspace,model:{transportAvailable:true,policies:[policy],proposals:[malformed],history:[]}}).ownerApprovalsPage();
  assert.match(html,/Manual Owner review required/);
  assert.match(html,/invalid/i);
  assert.match(html,/data-owner-proposal-evaluate="0" disabled aria-disabled="true"/);
});
