import test from 'node:test';
import assert from 'node:assert/strict';
import { adaptFoundationAutoAcceptDecision } from '../src/adapters/foundation-owner-standing-preapproval-v1.js';

const now=Date.parse('2026-08-18T01:00:00Z');

test('governed denied auto-accept decision may carry NONE candidate/plan/registration identities',()=>{
  const result=adaptFoundationAutoAcceptDecision({
    AcceptedUnderStandingOwnerPolicy:false,
    Reason:'WEB_OWNER_PREAPPROVAL_BACKUP_ROLLBACK_PLAN_REQUIRED',
    DecisionIdentitySha256:'sha256/'+'A'.repeat(64),
    ProposalIdentitySha256:'NONE',
    BackupRollbackPlanIdentitySha256:'NONE',
    RegistrationIdentitySha256:'NONE',
    UnderlyingAuthorityDecisionId:'NONE',
    ExecutionAuthorized:false,
    DeploymentAuthorized:false,
    BusinessAuthorityGranted:false,
    DecisionTime:'2026-08-18T00:50:00Z',
    Expiry:'2026-08-18T02:00:00Z',
    EvidenceReference:'evidence/denied'
  },{now});
  assert.equal(result.accepted,false);
  assert.equal(result.proposalIdentity,'NONE');
  assert.equal(result.executionAuthorized,false);
});

test('accepted auto-accept decision cannot use NONE identities',()=>{
  assert.throws(()=>adaptFoundationAutoAcceptDecision({
    AcceptedUnderStandingOwnerPolicy:true,
    Reason:'WEB_OWNER_DERIVED_AUTO_ACCEPT_ACCEPTED',
    DecisionIdentitySha256:'sha256/'+'A'.repeat(64),
    ProposalIdentitySha256:'NONE',
    BackupRollbackPlanIdentitySha256:'NONE',
    RegistrationIdentitySha256:'NONE',
    UnderlyingAuthorityDecisionId:'NONE',
    ExecutionAuthorized:false,
    DeploymentAuthorized:false,
    BusinessAuthorityGranted:false,
    DecisionTime:'2026-08-18T00:50:00Z',
    Expiry:'2026-08-18T02:00:00Z',
    EvidenceReference:'evidence/invalid'
  },{now}),/requires exact identities/);
});
