import test from 'node:test';
import assert from 'node:assert/strict';
import {
  WebOwnership,
  classifyOwnerRequestOwnership,
  decideWebMsaDevelopment,
  decideWebMsaResearch,
  decideCustomerSupportLsaResearch,
  WebAwarenessInvariants
} from '../src/core/web-awareness-model.js';

test('explicit Web path classifies as Web-owned',()=>{
  assert.equal(classifyOwnerRequestOwnership({targetPath:'applications/shared/web/src/app.js'}),WebOwnership.WEB);
});

test('FSATS request is foreign Application work and cannot be executed by Web MSA',()=>{
  const owner=classifyOwnerRequestOwnership({text:'Improve FSATS opportunity analysis'});
  assert.equal(owner,WebOwnership.APPLICATION);
  const decision=decideWebMsaDevelopment({ownerDirectRequest:true,ownership:owner});
  assert.equal(decision.allowed,false);
  assert.equal(decision.reason,'FOREIGN_OR_UNKNOWN_SCOPE_MUST_BE_ROUTED');
});

test('direct Owner Web request permits governed Web development but creates no bypass authority',()=>{
  const decision=decideWebMsaDevelopment({ownerDirectRequest:true,ownership:WebOwnership.WEB});
  assert.equal(decision.allowed,true);
  assert.equal(decision.authorityCreated,false);
  assert.equal(decision.validationRequired,true);
  assert.equal(decision.redTeamRequired,true);
});

test('autonomous Web self-development research stays disabled',()=>{
  const decision=decideWebMsaResearch({purpose:'self-development'});
  assert.equal(decision.allowed,false);
  assert.equal(decision.authoritativeTruthCreated,false);
});

test('customer support LSA research is bounded to support and creates no Falcon truth',()=>{
  const support=decideCustomerSupportLsaResearch({purpose:'incident-support'});
  assert.equal(support.allowed,true);
  assert.equal(support.authoritativeTruthCreated,false);
  assert.equal(support.developmentAuthorityCreated,false);
  assert.equal(decideCustomerSupportLsaResearch({purpose:'self-development'}).allowed,false);
});

test('awareness invariants preserve authority separation',()=>{
  assert.equal(WebAwarenessInvariants.autonomousSelfDevelopment,false);
  assert.equal(WebAwarenessInvariants.foreignWorkstreamImplementation,false);
  assert.equal(WebAwarenessInvariants.selfAwarenessCreatesAuthority,false);
  assert.equal(WebAwarenessInvariants.requestSentEqualsAccepted,false);
  assert.equal(WebAwarenessInvariants.acceptedEqualsCompleted,false);
});
