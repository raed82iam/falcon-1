import test from 'node:test';
import assert from 'node:assert/strict';
import {
  OwnerAiEmergencyAction,
  OwnerAiEmergencyTargetScope,
  canPrepareOwnerAiEmergencySubmission,
  bindOwnerAiEmergencyDecision
} from '../src/core/ports/owner-ai-emergency-port.js';

const session={authoritativeSession:true,role:'PROJECT_OWNER',principalId:'owner-1'};
const currentBlast={authoritative:true,freshness:'CURRENT',targetIds:['ai-1']};

test('missing or ambiguous target fails closed without scope widening',()=>{
  assert.equal(canPrepareOwnerAiEmergencySubmission({session,target:null,action:OwnerAiEmergencyAction.KILL,blastRadius:currentBlast}).allowed,false);
  assert.equal(canPrepareOwnerAiEmergencySubmission({session,target:{id:'ai-1',scope:'UNKNOWN'},action:OwnerAiEmergencyAction.KILL,blastRadius:currentBlast}).allowed,false);
});

test('ALL_AI target cannot be used with targeted KILL',()=>{
  const decision=canPrepareOwnerAiEmergencySubmission({
    session,
    target:{id:'all-ai',scope:OwnerAiEmergencyTargetScope.ALL_AI,truth:'CURRENT',freshness:'CURRENT'},
    action:OwnerAiEmergencyAction.KILL,
    blastRadius:{authoritative:true,freshness:'CURRENT',targetIds:['ai-1','ai-2']}
  });
  assert.equal(decision.allowed,false);
  assert.equal(decision.reasonCode,'ALL_AI_TARGET_REQUIRES_GLOBAL_AI_KILL');
});

test('GLOBAL AI kill must preserve Safe Core and cannot authorize Falcon shutdown',()=>{
  const base={requestId:'r1',correlationId:'c1',targetId:'all-ai',action:OwnerAiEmergencyAction.GLOBAL_AI_KILL,accepted:true,impactedTargetIds:['ai-1'],reason:'owner emergency',safeCorePreserved:true,falconShutdownAuthorized:false,completed:false};
  assert.doesNotThrow(()=>bindOwnerAiEmergencyDecision(base));
  assert.throws(()=>bindOwnerAiEmergencyDecision({...base,safeCorePreserved:false}),/Safe Core/);
  assert.throws(()=>bindOwnerAiEmergencyDecision({...base,falconShutdownAuthorized:true}),/Falcon shutdown/);
});

test('denied decision cannot report impacted targets',()=>{
  assert.throws(()=>bindOwnerAiEmergencyDecision({
    requestId:'r2',correlationId:'c2',targetId:'ai-1',action:OwnerAiEmergencyAction.SUSPEND,
    accepted:false,impactedTargetIds:['ai-1'],reason:'denied'
  }),/denied decision cannot widen blast radius/);
});

test('ACTION_COMPLETED cannot be inferred from acceptance',()=>{
  assert.throws(()=>bindOwnerAiEmergencyDecision({
    requestId:'r3',correlationId:'c3',targetId:'ai-1',action:OwnerAiEmergencyAction.SUSPEND,
    accepted:true,impactedTargetIds:['ai-1'],reason:'accepted',outcome:'ACTION_COMPLETED',completed:false
  }),/authoritative completion evidence/);
});
