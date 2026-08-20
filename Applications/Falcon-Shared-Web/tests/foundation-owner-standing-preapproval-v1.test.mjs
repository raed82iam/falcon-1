import test from 'node:test';
import assert from 'node:assert/strict';
import {
  adaptFoundationAutoAcceptDecision,
  adaptFoundationPolicyManagementDecision,
  adaptFoundationRollbackOrderDecision,
  adaptFoundationRollbackStatus
} from '../src/adapters/foundation-owner-standing-preapproval-v1.js';

const now=Date.parse('2026-08-18T01:00:00Z');
const id=char=>`sha256/${char.repeat(64)}`;
const authorityId=char=>`authority-decision/sha256/${char.repeat(64)}`;

const acceptedAuto=()=>({
  AcceptedUnderStandingOwnerPolicy:true,Reason:'WEB_OWNER_DERIVED_AUTO_ACCEPT_ACCEPTED',DecisionIdentitySha256:id('A'),ProposalIdentitySha256:id('B'),BackupRollbackPlanIdentitySha256:id('C'),RegistrationIdentitySha256:id('D'),UnderlyingAuthorityDecisionId:authorityId('E'),ExecutionAuthorized:false,DeploymentAuthorized:false,BusinessAuthorityGranted:false,DecisionTime:'2026-08-18T00:50:00Z',Expiry:'2026-08-18T02:00:00Z',EvidenceReference:'evidence/owner-autoaccept'
});

const appliedPolicy=()=>({Applied:true,Reason:'STANDING_OWNER_POLICY_MUTATION_APPLIED',DecisionIdentitySha256:id('A'),RegistrationIdentitySha256:id('B'),PolicyId:'policy-1',PolicyVersion:'2',Revoked:false,DecisionTime:'2026-08-18T00:55:00Z',Expiry:'2026-08-18T02:00:00Z',EvidenceReference:'evidence/policy'});

const acceptedRollback=()=>({State:'Accepted',Reason:'ROLLBACK_ORDER_ACCEPTED_FOR_SEPARATE_EXECUTION',DecisionIdentitySha256:id('A'),RollbackAuthorized:true,RollbackExecuted:false,AuthorityRestored:false,TrustRestored:false,DecisionTime:'2026-08-18T00:55:00Z',Expiry:'2026-08-18T02:00:00Z',EvidenceReference:'evidence/rollback-order'});

test('accepted Owner-derived auto accept remains non-execution/non-deployment/non-business authority',()=>{
  const result=adaptFoundationAutoAcceptDecision(acceptedAuto(),{now});
  assert.equal(result.accepted,true);
  assert.equal(result.executionAuthorized,false);
  assert.equal(result.deploymentAuthorized,false);
  assert.equal(result.businessAuthorityGranted,false);
});

test('auto accept result leaking execution authority is rejected',()=>{
  assert.throws(()=>adaptFoundationAutoAcceptDecision({...acceptedAuto(),ExecutionAuthorized:true},{now}),/leaked/);
});

test('accepted auto accept requires exact registered and underlying authority identities',()=>{
  assert.throws(()=>adaptFoundationAutoAcceptDecision({...acceptedAuto(),RegistrationIdentitySha256:'registration-1'},{now}),/malformed|exact identities/);
  assert.throws(()=>adaptFoundationAutoAcceptDecision({...acceptedAuto(),UnderlyingAuthorityDecisionId:'authority-1'},{now}),/malformed|exact identities/);
});

test('auto accept semantic timestamps must remain canonical UTC',()=>{
  assert.throws(()=>adaptFoundationAutoAcceptDecision({...acceptedAuto(),DecisionTime:'2026-08-18T03:50:00+03:00'},{now}),/malformed/);
  assert.throws(()=>adaptFoundationAutoAcceptDecision({...acceptedAuto(),Expiry:'2026-08-18T05:00:00+03:00'},{now}),/malformed/);
});

test('applied standing policy decision requires exact applied reason and current time',()=>{
  const result=adaptFoundationPolicyManagementDecision(appliedPolicy(),{now});
  assert.equal(result.applied,true);
  assert.equal(result.policyVersion,'2');
  assert.equal(result.registrationIdentity,id('B'));
});

test('applied policy requires canonical registration identity and denied policy cannot mint one',()=>{
  assert.throws(()=>adaptFoundationPolicyManagementDecision({...appliedPolicy(),RegistrationIdentitySha256:'registration-1'},{now}),/malformed|exact registration/);
  const denied={...appliedPolicy(),Applied:false,Reason:'STANDING_OWNER_POLICY_MUTATION_AUTHENTICATION_REQUIRED',RegistrationIdentitySha256:'NONE'};
  assert.equal(adaptFoundationPolicyManagementDecision(denied,{now}).applied,false);
  assert.throws(()=>adaptFoundationPolicyManagementDecision({...denied,RegistrationIdentitySha256:id('F')},{now}),/cannot claim registration/);
});

test('policy decision semantic timestamps must remain canonical UTC',()=>{
  assert.throws(()=>adaptFoundationPolicyManagementDecision({...appliedPolicy(),DecisionTime:'2026-08-18T03:55:00+03:00'},{now}),/malformed/);
});

test('accepted rollback order authorizes separate rollback only and not execution/restoration',()=>{
  const result=adaptFoundationRollbackOrderDecision(acceptedRollback(),{now});
  assert.equal(result.rollbackAuthorized,true);
  assert.equal(result.rollbackExecuted,false);
  assert.equal(result.authorityRestored,false);
  assert.equal(result.trustRestored,false);
});

test('rollback order cannot claim execution at authorization stage',()=>{
  assert.throws(()=>adaptFoundationRollbackOrderDecision({...acceptedRollback(),RollbackExecuted:true},{now}),/cannot imply execution/);
});

test('rollback decision semantic timestamps must remain canonical UTC',()=>{
  assert.throws(()=>adaptFoundationRollbackOrderDecision({...acceptedRollback(),DecisionTime:'2026-08-18T03:55:00+03:00'},{now}),/malformed/);
});

test('rollback status cannot silently restore authority, trust, credentials, Live or Kill state',()=>{
  const base={RollbackOrderDecisionIdentitySha256:id('A'),ExecutionState:'Completed',ExecutorIdentity:'fsats.rollback',ResultEvidenceReference:'evidence/result',ObservedAt:'2026-08-18T00:58:00Z',AuthorityRestored:false,TrustRestored:false,CredentialsRestored:false,LiveTradingAuthorityRestored:false,KillReleaseRevivalAuthorityRestored:false};
  assert.equal(adaptFoundationRollbackStatus(base).executionState,'Completed');
  for (const key of ['AuthorityRestored','TrustRestored','CredentialsRestored','LiveTradingAuthorityRestored','KillReleaseRevivalAuthorityRestored']) {
    assert.throws(()=>adaptFoundationRollbackStatus({...base,[key]:true}),/cannot silently restore/);
  }
  assert.throws(()=>adaptFoundationRollbackStatus({...base,ObservedAt:'2026-08-18T03:58:00+03:00'}),/malformed/);
});
