import test from 'node:test';
import assert from 'node:assert/strict';
import {
  OwnerAiEmergencyAction,
  OwnerAiEmergencyTargetScope,
  OwnerAiEmergencyOutcome,
  createUnavailableOwnerAiEmergencyPort,
  canPrepareOwnerAiEmergencySubmission,
  buildOwnerAiEmergencyRequest,
  bindOwnerAiEmergencyDecision
} from '../src/core/ports/owner-ai-emergency-port.js';

const ownerSession={authoritativeSession:true,role:'PROJECT_OWNER',principalId:'owner-1'};
const targeted={id:'ai:app-alpha:msa',scope:OwnerAiEmergencyTargetScope.ONE_MSA_OR_APPLICATION_AI_SCOPE,truth:'CURRENT',freshness:'CURRENT'};
const targetedBlast={authoritative:true,freshness:'CURRENT',targetIds:['ai:app-alpha:csa:one','ai:app-alpha:lsa','ai:app-alpha:msa']};

test('Owner AI emergency port remains fail-closed without a governed Web runtime binding', async () => {
  const port=createUnavailableOwnerAiEmergencyPort();
  const inventory=await port.targetInventory();
  const request=await port.submitAction();
  assert.equal(inventory.truth,'UNAVAILABLE');
  assert.deepEqual(inventory.items,[]);
  assert.equal(request.outcome,OwnerAiEmergencyOutcome.UNAVAILABLE);
  assert.equal(request.requestId,null);
  assert.equal(request.correlationId,null);
  assert.equal(request.reasonCode,'WEB_AI_KILL_REQUEST_TRANSPORT_UNAVAILABLE');
});

test('submission preparation requires an authoritative Owner session', () => {
  const result=canPrepareOwnerAiEmergencySubmission({
    session:{authoritativeSession:false,role:'PROJECT_OWNER',principalId:'owner-1'},
    target:targeted,
    action:OwnerAiEmergencyAction.KILL,
    blastRadius:targetedBlast
  });
  assert.equal(result.allowed,false);
  assert.equal(result.reasonCode,'AUTHORITATIVE_OWNER_SESSION_REQUIRED');
});

test('stale target or blast radius blocks preparation', () => {
  assert.equal(canPrepareOwnerAiEmergencySubmission({
    session:ownerSession,
    target:{...targeted,freshness:'STALE'},
    action:OwnerAiEmergencyAction.KILL,
    blastRadius:targetedBlast
  }).reasonCode,'CURRENT_AUTHORITATIVE_TARGET_STATE_REQUIRED');

  assert.equal(canPrepareOwnerAiEmergencySubmission({
    session:ownerSession,
    target:targeted,
    action:OwnerAiEmergencyAction.KILL,
    blastRadius:{...targetedBlast,freshness:'STALE'}
  }).reasonCode,'CURRENT_AUTHORITATIVE_BLAST_RADIUS_REQUIRED');
});

test('Global AI Kill requires explicit ALL_AI target and non-empty authoritative census', () => {
  const denied=canPrepareOwnerAiEmergencySubmission({
    session:ownerSession,
    target:targeted,
    action:OwnerAiEmergencyAction.GLOBAL_AI_KILL,
    blastRadius:targetedBlast
  });
  assert.equal(denied.allowed,false);
  assert.equal(denied.reasonCode,'GLOBAL_KILL_REQUIRES_ALL_AI_TARGET');

  const all={id:'ALL_AI',scope:OwnerAiEmergencyTargetScope.ALL_AI,truth:'CURRENT',freshness:'CURRENT'};
  assert.equal(canPrepareOwnerAiEmergencySubmission({
    session:ownerSession,target:all,action:OwnerAiEmergencyAction.GLOBAL_AI_KILL,
    blastRadius:{authoritative:true,freshness:'CURRENT',targetIds:[]}
  }).reasonCode,'GLOBAL_KILL_REQUIRES_NONEMPTY_AI_CENSUS');

  assert.equal(canPrepareOwnerAiEmergencySubmission({
    session:ownerSession,target:all,action:OwnerAiEmergencyAction.GLOBAL_AI_KILL,
    blastRadius:{authoritative:true,freshness:'CURRENT',targetIds:['fsa:primary','ai:app-alpha:msa']}
  }).allowed,true);
});

test('ordinary actions cannot target ALL_AI', () => {
  const result=canPrepareOwnerAiEmergencySubmission({
    session:ownerSession,
    target:{id:'ALL_AI',scope:OwnerAiEmergencyTargetScope.ALL_AI,truth:'CURRENT',freshness:'CURRENT'},
    action:OwnerAiEmergencyAction.KILL,
    blastRadius:{authoritative:true,freshness:'CURRENT',targetIds:['ai:one']}
  });
  assert.equal(result.allowed,false);
  assert.equal(result.reasonCode,'ALL_AI_TARGET_REQUIRES_GLOBAL_AI_KILL');
});

test('request intent preserves exact target/blast radius and never claims Web authorization or execution', () => {
  const request=buildOwnerAiEmergencyRequest({
    requestId:'kill:web:1',correlationId:'corr:1',session:ownerSession,target:targeted,
    action:OwnerAiEmergencyAction.KILL,blastRadius:targetedBlast,
    requestedAt:'2026-08-16T18:00:00Z',expiresAt:'2026-08-16T18:05:00Z'
  });
  assert.equal(request.actorIdentity,'owner-1');
  assert.equal(request.ingress,'WEB_OWNER');
  assert.equal(request.targetId,'ai:app-alpha:msa');
  assert.deepEqual(request.blastRadiusConfirmation.targetIds,targetedBlast.targetIds);
  assert.equal(request.webAuthorizationClaim,false);
  assert.equal(request.executionClaim,false);
  assert.equal(request.releaseOrRevivalRequested,false);
  assert.equal(Object.isFrozen(request.blastRadiusConfirmation.targetIds),true);
});

test('accepted targeted Kill remains ACTION_ACCEPTED until authoritative completion exists', () => {
  const decision=bindOwnerAiEmergencyDecision({
    requestId:'kill:web:1',correlationId:'corr:1',targetId:'ai:app-alpha:msa',action:'KILL',
    accepted:true,reason:'ACCEPTED_TARGETED',impactedTargetIds:targetedBlast.targetIds,
    safeCorePreserved:true,falconShutdownAuthorized:false,stopRequired:true,isolationRequired:true,
    authorityRevocationRequired:true,evidenceFreezeRequired:true,releaseRequiresGovernedRecovery:true,
    targetCooperationRequired:false,completed:false,evidenceReference:'evidence:kill:1'
  });
  assert.equal(decision.outcome,OwnerAiEmergencyOutcome.ACTION_ACCEPTED);
  assert.equal(decision.completed,false);
  assert.equal(decision.releaseAvailable,false);
  assert.equal(decision.webAuthorized,false);
  assert.equal(decision.webExecuted,false);
});

test('denied or unknown target decision cannot carry impacted targets', () => {
  assert.throws(()=>bindOwnerAiEmergencyDecision({
    requestId:'kill:web:2',correlationId:'corr:2',targetId:'ai:unknown',action:'KILL',
    accepted:false,reason:'TARGET_NOT_FOUND',impactedTargetIds:['ai:other'],
    safeCorePreserved:true,falconShutdownAuthorized:false,targetCooperationRequired:false
  }),/cannot widen blast radius/);
});

test('GLOBAL_AI_KILL cannot be presented as Falcon shutdown or Safe Core loss', () => {
  const base={
    requestId:'kill:web:3',correlationId:'corr:3',targetId:'ALL_AI',action:'GLOBAL_AI_KILL',
    accepted:true,reason:'ACCEPTED_GLOBAL',impactedTargetIds:['ai:one','fsa:primary'],
    safeCorePreserved:true,falconShutdownAuthorized:false,targetCooperationRequired:false
  };
  assert.doesNotThrow(()=>bindOwnerAiEmergencyDecision(base));
  assert.throws(()=>bindOwnerAiEmergencyDecision({...base,safeCorePreserved:false}),/preserve Falcon Safe Core/);
  assert.throws(()=>bindOwnerAiEmergencyDecision({...base,falconShutdownAuthorized:true}),/must not authorize Falcon shutdown/);
});

test('ACTION_COMPLETED requires separate authoritative completion evidence', () => {
  const base={
    requestId:'kill:web:4',correlationId:'corr:4',targetId:'ai:one',action:'RESTRICT',
    accepted:true,reason:'ACCEPTED_TARGETED',impactedTargetIds:['ai:one'],
    safeCorePreserved:true,falconShutdownAuthorized:false,targetCooperationRequired:false,
    outcome:OwnerAiEmergencyOutcome.ACTION_COMPLETED
  };
  assert.throws(()=>bindOwnerAiEmergencyDecision({...base,completed:false}),/authoritative completion evidence/);
  assert.equal(bindOwnerAiEmergencyDecision({...base,completed:true}).outcome,OwnerAiEmergencyOutcome.ACTION_COMPLETED);
});
