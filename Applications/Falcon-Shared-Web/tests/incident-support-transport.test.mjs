import test from 'node:test';
import assert from 'node:assert/strict';
import { createUnavailableIncidentSupportTransportPort, validateSupportTransportDecision } from '../src/core/ports/incident-support-transport-port.js';

const request={incidentId:'incident-1',principalId:'customer-1',sessionId:'session-1'};
const accepted=()=>({
  accepted:true,delivered:true,authorityGranted:false,
  requestId:'support-request-1',evidenceReference:'evidence:support:1',
  incidentId:'incident-1',principalId:'customer-1',sessionId:'session-1'
});

test('default Support transport is fail-closed',async()=>{
  const result=await createUnavailableIncidentSupportTransportPort().requestSupport(request);
  assert.equal(result.accepted,false);
  assert.equal(result.delivered,false);
  assert.equal(result.authorityGranted,false);
});

test('accepted Support transport remains transport-only and exact-session bound',()=>{
  const result=validateSupportTransportDecision(accepted(),request);
  assert.equal(result.accepted,true);
  assert.equal(result.delivered,true);
  assert.equal(result.authorityGranted,false);
  assert.equal(result.incidentId,request.incidentId);
});

test('Support transport rejects authority leakage and identity mutations',()=>{
  for(const mutate of [
    d=>{d.authorityGranted=true;},
    d=>{d.incidentId='other-incident';},
    d=>{d.principalId='other-customer';},
    d=>{d.sessionId='other-session';},
    d=>{d.requestId='';},
    d=>{d.evidenceReference='';}
  ]){
    const decision=accepted(); mutate(decision);
    assert.equal(validateSupportTransportDecision(decision,request).accepted,false);
  }
});
