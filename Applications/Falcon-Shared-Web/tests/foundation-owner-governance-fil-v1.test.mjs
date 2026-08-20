import test from 'node:test';
import assert from 'node:assert/strict';
import {
  OwnerPolicyManagementFilProfileV1,
  OwnerPreapprovalEvaluationFilProfileV1,
  OwnerRollbackOrderFilProfileV1,
  ownerGovernanceProfileIdentity,
  buildOwnerGovernanceRequest,
  consumeOwnerGovernanceResponse,
  createFoundationOwnerGovernanceTransportAdapter
} from '../src/adapters/foundation-owner-governance-fil-v1.js';
import {createUnavailableOwnerUpdateGovernancePort,createOwnerUpdateGovernanceTransportPort} from '../src/core/ports/owner-update-governance-port.js';

const now=Date.parse('2026-08-18T00:00:00Z');
const observed=Date.parse('2026-08-18T00:00:30Z');
const identity=Object.freeze({messageId:'message:web:owner-governance:1',correlationId:'correlation:web:owner-governance:1',causationId:'causation:web:owner-action:1',idempotencyId:'idempotency:web:owner-governance:1',deliveryAttemptId:'delivery:web:owner-governance:1',retryLineageId:'retry:web:owner-governance:1'});

async function digest(value){const result=await globalThis.crypto.subtle.digest('SHA-256',new TextEncoder().encode(value));return Array.from(new Uint8Array(result),b=>b.toString(16).padStart(2,'0')).join('').toUpperCase();}
async function build(profile,payload={x:1},{createdAt='2026-08-18T00:00:00Z',expiresAt='2026-08-18T00:02:00Z',at=now,id=identity}={}){
  return buildOwnerGovernanceRequest(profile,payload,id,{createdAt,expiresAt,now:at});
}
async function responsePacket(profile,request,payload={Applied:true},{createdAt='2026-08-18T00:00:01Z',expiresAt='2026-08-18T00:01:01Z'}={}){
  const profileId=await ownerGovernanceProfileIdentity(profile);const serialized=JSON.stringify(payload);
  return {Accepted:true,Reason:'PUBLIC_RESPONSE_FIL_ENVELOPE_ACCEPTED',ProfileIdentitySha256:profileId,RouteAvailable:true,RouteActivated:false,RouteAuthorized:false,ConnectionExecuted:false,ExecutionAuthorized:false,BusinessAuthorityGranted:false,Envelope:{MessageId:{Value:'message:foundation:owner-governance:result:1'},MessageKind:3,Classification:2,MessageType:profile.responseMessageType,SchemaId:{Value:profile.responseSchemaIdentity},SchemaVersion:'1.0.0',Producer:{Value:'foundation.runtime'},RecipientScope:{Value:'shared-web'},CorrelationId:{Value:request.envelope.correlationId},CausationId:{Value:request.envelope.messageId},Authority:{Value:'authority:transport:owner-governance-response'},Provenance:{Value:`response-profile:${profileId.slice(7)}`},IdempotencyId:{Value:'idempotency:foundation:owner-governance:result:1'},DeliveryAttemptId:{Value:'delivery:foundation:owner-governance:result:1'},RetryLineageId:{Value:'retry:foundation:owner-governance:result:1'},Time:{CreatedAt:createdAt,ExpiresAt:expiresAt},Outcome:{Code:1,Reason:'governed_result'},Payload:serialized,PayloadSha256:await digest(serialized)}};
}

for(const [name,profile,expectedKind] of [['policy management',OwnerPolicyManagementFilProfileV1,1],['standing preapproval evaluation',OwnerPreapprovalEvaluationFilProfileV1,2],['rollback order',OwnerRollbackOrderFilProfileV1,1]]){
  test(`${name} builds exact canonical FCR-0241 request and consumes bound response`,async()=>{
    const request=await build(profile,{RequestId:'request-1'});
    assert.equal(request.built,true);
    assert.equal('accepted' in request,false,'Web request builder must not self-declare Foundation acceptance');
    assert.equal('routeAvailable' in request,false,'Web request builder must not self-declare Foundation route availability');
    assert.equal(request.envelope.messageKind,expectedKind);assert.equal(request.envelope.classification,2);assert.equal(request.envelope.producer,'shared-web');assert.equal(request.envelope.recipientScope,'foundation.owner-governance');assert.equal(request.envelope.authority,'authority:transport:owner-command-center-request');
    const response=await consumeOwnerGovernanceResponse(await responsePacket(profile,request),profile,request,{now:observed});
    assert.equal(response.accepted,true);assert.equal(response.profile,profile);
  });
}

test('request freshness requires UTC, maximum 120-second TTL, and a current observation window',async()=>{
  let result=await buildOwnerGovernanceRequest(OwnerPolicyManagementFilProfileV1,{x:1},identity,{createdAt:'2026-08-18T03:00:00+03:00',expiresAt:'2026-08-18T03:01:00+03:00',now});
  assert.equal(result.built,false);assert.equal(result.reason,'OWNER_GOVERNANCE_REQUEST_FRESHNESS_INVALID');
  result=await buildOwnerGovernanceRequest(OwnerPolicyManagementFilProfileV1,{x:1},identity,{createdAt:'2026-08-18T00:00:00Z',expiresAt:'2026-08-18T00:02:01Z',now});
  assert.equal(result.built,false);assert.equal(result.reason,'OWNER_GOVERNANCE_REQUEST_FRESHNESS_INVALID');
  result=await buildOwnerGovernanceRequest(OwnerPolicyManagementFilProfileV1,{x:1},identity,{createdAt:'2026-08-18T00:00:00Z',expiresAt:'2026-08-18T00:00:29Z',now:observed});
  assert.equal(result.built,false);assert.equal(result.reason,'OWNER_GOVERNANCE_REQUEST_FRESHNESS_INVALID');
});

test('response is rejected when the originating accepted request is no longer current',async()=>{
  const request=await build(OwnerPolicyManagementFilProfileV1,{x:1},{expiresAt:'2026-08-18T00:00:20Z'});
  assert.equal(request.built,true);
  const packet=await responsePacket(OwnerPolicyManagementFilProfileV1,request,{Applied:true},{createdAt:'2026-08-18T00:00:10Z',expiresAt:'2026-08-18T00:01:10Z'});
  const result=await consumeOwnerGovernanceResponse(packet,OwnerPolicyManagementFilProfileV1,request,{now:observed});
  assert.equal(result.accepted,false);assert.equal(result.reason,'OWNER_GOVERNANCE_REQUEST_NO_LONGER_CURRENT');
});

test('cross-family response substitution fails closed',async()=>{
  const request=await build(OwnerPolicyManagementFilProfileV1);
  const packet=await responsePacket(OwnerPreapprovalEvaluationFilProfileV1,{envelope:request.envelope});
  const result=await consumeOwnerGovernanceResponse(packet,OwnerPolicyManagementFilProfileV1,request,{now:observed});
  assert.equal(result.accepted,false);assert.equal(result.reason,'OWNER_GOVERNANCE_PROFILE_IDENTITY_MISMATCH');
});

test('response correlation or causation mismatch fails closed',async()=>{
  const request=await build(OwnerRollbackOrderFilProfileV1);
  const packet=await responsePacket(OwnerRollbackOrderFilProfileV1,request);packet.Envelope.CorrelationId={Value:'correlation:wrong'};
  let result=await consumeOwnerGovernanceResponse(packet,OwnerRollbackOrderFilProfileV1,request,{now:observed});assert.equal(result.reason,'OWNER_GOVERNANCE_RESPONSE_REQUEST_BINDING_MISMATCH');
  const packet2=await responsePacket(OwnerRollbackOrderFilProfileV1,request);packet2.Envelope.CausationId={Value:'message:wrong'};
  result=await consumeOwnerGovernanceResponse(packet2,OwnerRollbackOrderFilProfileV1,request,{now:observed});assert.equal(result.reason,'OWNER_GOVERNANCE_RESPONSE_REQUEST_BINDING_MISMATCH');
});

test('route activation or authority escalation in transport decision fails closed',async()=>{
  const request=await build(OwnerPreapprovalEvaluationFilProfileV1);
  for(const field of ['RouteActivated','RouteAuthorized','ConnectionExecuted','ExecutionAuthorized','BusinessAuthorityGranted']){const packet=await responsePacket(OwnerPreapprovalEvaluationFilProfileV1,request);packet[field]=true;const result=await consumeOwnerGovernanceResponse(packet,OwnerPreapprovalEvaluationFilProfileV1,request,{now:observed});assert.equal(result.reason,'OWNER_GOVERNANCE_RESPONSE_AUTHORITY_VIOLATION');}
});

test('payload mutation, stale response, non-UTC response and response kind swap fail closed',async()=>{
  const request=await build(OwnerPolicyManagementFilProfileV1);
  const mutated=await responsePacket(OwnerPolicyManagementFilProfileV1,request);mutated.Envelope.Payload='{"Applied":false}';assert.equal((await consumeOwnerGovernanceResponse(mutated,OwnerPolicyManagementFilProfileV1,request,{now:observed})).reason,'OWNER_GOVERNANCE_RESPONSE_PAYLOAD_DIGEST_MISMATCH');
  const stale=await responsePacket(OwnerPolicyManagementFilProfileV1,request);assert.equal((await consumeOwnerGovernanceResponse(stale,OwnerPolicyManagementFilProfileV1,request,{now:Date.parse('2026-08-18T00:02:30Z')})).reason,'OWNER_GOVERNANCE_RESPONSE_NOT_CURRENT');
  const nonUtc=await responsePacket(OwnerPolicyManagementFilProfileV1,request);nonUtc.Envelope.Time={CreatedAt:'2026-08-18T03:00:01+03:00',ExpiresAt:'2026-08-18T03:01:01+03:00'};assert.equal((await consumeOwnerGovernanceResponse(nonUtc,OwnerPolicyManagementFilProfileV1,request,{now:observed})).reason,'OWNER_GOVERNANCE_RESPONSE_NOT_CURRENT');
  const wrongKind=await responsePacket(OwnerPolicyManagementFilProfileV1,request);wrongKind.Envelope.MessageKind=1;assert.equal((await consumeOwnerGovernanceResponse(wrongKind,OwnerPolicyManagementFilProfileV1,request,{now:observed})).reason,'OWNER_GOVERNANCE_RESPONSE_KIND_MISMATCH');
});

test('non-canonical and malformed caller profiles are rejected without normalization or silent upgrade',async()=>{
  const forged={...OwnerPolicyManagementFilProfileV1,requestRouteIdentity:'route:attacker'};
  let result=await buildOwnerGovernanceRequest(forged,{x:1},identity,{createdAt:'2026-08-18T00:00:00Z',expiresAt:'2026-08-18T00:02:00Z',now});
  assert.equal(result.reason,'OWNER_GOVERNANCE_PROFILE_NOT_CANONICAL');
  const malformed={...OwnerPolicyManagementFilProfileV1,requestSchemaIdentity:null};
  result=await buildOwnerGovernanceRequest(malformed,{x:1},identity,{createdAt:'2026-08-18T00:00:00Z',expiresAt:'2026-08-18T00:02:00Z',now});
  assert.equal(result.reason,'OWNER_GOVERNANCE_PROFILE_NOT_CANONICAL');
});

test('transport adapter observes time again after exchange before accepting a response',async()=>{
  let calls=0;
  const clock=()=>{calls++;return calls===1?now:observed;};
  const transport=createFoundationOwnerGovernanceTransportAdapter({clock,identityFactory:()=>identity,exchange:async({profile,request})=>responsePacket(profile,request,{ok:true})});
  const result=await transport.manageStandingPolicy({x:1});
  assert.equal(result.accepted,true);assert.equal(calls,2);
});

test('stable Web governance port exposes only the three injected FCR-0241 writes while reads remain fail closed',async()=>{
  let calls=0;
  const clock=()=>{calls++;return calls%2===1?now:observed;};
  const transport=createFoundationOwnerGovernanceTransportAdapter({clock,identityFactory:()=>identity,exchange:async({profile,request})=>responsePacket(profile,request,{ok:true})});
  const port=createOwnerUpdateGovernanceTransportPort({basePort:createUnavailableOwnerUpdateGovernancePort(),transportAdapter:transport});
  assert.equal((await port.standingPolicies()).available,false);assert.equal((await port.proposalInbox()).available,false);assert.equal((await port.autoAcceptedHistory()).available,false);assert.equal((await port.rollbackStatus()).available,false);
  assert.equal((await port.manageStandingPolicy({x:1})).accepted,true);assert.equal((await port.evaluateStandingProposal({x:1})).accepted,true);assert.equal((await port.requestRollback({x:1})).accepted,true);
});