import test from 'node:test';
import assert from 'node:assert/strict';
import {
  CustomerConversationMode,
  decideCustomerExplanationAccess,
  normalizeExplanationTruth,
  CustomerExplanationInvariants
} from '../src/core/customer-explanation-policy.js';

const session={authoritativeSession:true,principalId:'p1',tenantId:'t1'};
const projection={principalId:'p1',tenantId:'t1',incidentId:'inc-1'};

test('same principal and tenant permits explanation only',()=>{
  const decision=decideCustomerExplanationAccess({session,projection});
  assert.equal(decision.allowed,true);
  assert.equal(decision.explanationOnly,true);
  assert.equal(decision.analysisAuthorityCreated,false);
  assert.equal(decision.executionAuthorityCreated,false);
  assert.equal(decision.longTermMemoryWriteAuthorized,false);
});

test('tenant or principal mismatch fails closed',()=>{
  assert.equal(decideCustomerExplanationAccess({session,projection:{...projection,tenantId:'t2'}}).reason,'TENANT_MISMATCH');
  assert.equal(decideCustomerExplanationAccess({session,projection:{...projection,principalId:'p2'}}).reason,'PRINCIPAL_MISMATCH');
});

test('incident conversation requires incident identity',()=>{
  const decision=decideCustomerExplanationAccess({session,projection:{principalId:'p1',tenantId:'t1'},conversationMode:CustomerConversationMode.INCIDENT});
  assert.equal(decision.allowed,false);
  assert.equal(decision.reason,'INCIDENT_ID_REQUIRED');
});

test('stale explanation must preserve uncertainty',()=>{
  const truth=normalizeExplanationTruth({truthState:'LAST_KNOWN',freshnessState:'STALE',limitations:['provider delayed']});
  assert.equal(truth.mayPresentAsCurrent,false);
  assert.equal(truth.uncertaintyRequired,true);
  assert.deepEqual(truth.limitations,['provider delayed']);
});

test('invariants prohibit hidden authority and implicit memory',()=>{
  assert.equal(CustomerExplanationInvariants.explanationEqualsTradingDecision,false);
  assert.equal(CustomerExplanationInvariants.ordinaryChatEqualsIncidentConversation,false);
  assert.equal(CustomerExplanationInvariants.longTermMemoryImplicitlyAuthorized,false);
  assert.equal(CustomerExplanationInvariants.tenantIsolationRequired,true);
});
