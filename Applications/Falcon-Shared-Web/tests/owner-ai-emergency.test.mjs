import test from 'node:test';
import assert from 'node:assert/strict';
import { createOwnerAiEmergencyFeature } from '../src/features/owner-ai-emergency/owner-ai-emergency.js';
import { OwnerAiEmergencyAction, OwnerAiEmergencyTargetScope } from '../src/core/ports/owner-ai-emergency-port.js';

const t=key=>key;
const workspace=(body,active,owner)=>`<main data-active="${active}" data-owner="${owner}">${body}</main>`;
const session={authoritativeSession:true,role:'PROJECT_OWNER',principalId:'owner-1'};
const target={id:'ai:app-alpha:msa',scope:OwnerAiEmergencyTargetScope.ONE_MSA_OR_APPLICATION_AI_SCOPE,truth:'CURRENT',freshness:'CURRENT'};
const blastRadius={authoritative:true,freshness:'CURRENT',targetIds:['ai:app-alpha:csa:one','ai:app-alpha:lsa','ai:app-alpha:msa']};

function render(model={},language='en',currentSession=session) {
  return createOwnerAiEmergencyFeature({t,language:()=>language,workspace,session:currentSession,model}).page();
}

const acceptedDecision=(extra={})=>({
  requestId:'r1',correlationId:'c1',targetId:'ai:app-alpha:msa',action:'KILL',accepted:true,
  reason:'ACCEPTED_TARGETED',outcome:'ACTION_ACCEPTED',impactedTargetIds:[...blastRadius.targetIds],
  safeCorePreserved:true,falconShutdownAuthorized:false,releaseRequiresGovernedRecovery:true,
  targetCooperationRequired:false,evidenceReference:'ev-1',...extra
});

test('Owner emergency surface is Owner-only presentation and has no release/revival control', () => {
  const html=render({targets:[target],selectedTarget:target,selectedAction:OwnerAiEmergencyAction.KILL,blastRadius,transportAvailable:false});
  assert.match(html,/data-active="owner-ai-emergency"/);
  assert.match(html,/data-owner="true"/);
  assert.match(html,/Runtime binding unavailable/);
  assert.match(html,/data-ai-emergency-submit disabled aria-disabled="true"/);
  assert.match(html,/REQUEST_SENT ≠ ACTION_ACCEPTED ≠ ACTION_COMPLETED/);
  assert.doesNotMatch(html,/data-(?:release|revival)/i);
});

test('GLOBAL_AI_KILL presentation explicitly preserves Falcon Safe Core and is not Falcon shutdown', () => {
  const all={id:'ALL_AI',scope:OwnerAiEmergencyTargetScope.ALL_AI,truth:'CURRENT',freshness:'CURRENT'};
  const html=render({
    targets:[all],selectedTarget:all,selectedAction:OwnerAiEmergencyAction.GLOBAL_AI_KILL,
    blastRadius:{authoritative:true,freshness:'CURRENT',targetIds:['ai:one','fsa:primary']},transportAvailable:true
  });
  assert.match(html,/GLOBAL AI KILL targets AI only/);
  assert.match(html,/not a Falcon shutdown/);
  assert.match(html,/Falcon Safe Core remains operational/);
  assert.doesNotMatch(html,/data-ai-emergency-submit disabled/);
});

test('stale or unavailable blast radius disables submission and says Web will not widen scope', () => {
  const html=render({targets:[target],selectedTarget:target,selectedAction:'KILL',blastRadius:{authoritative:true,freshness:'STALE',targetIds:['ai:other']},transportAvailable:true});
  assert.match(html,/Blast radius is not currently authoritative/);
  assert.match(html,/neither guess the target nor widen scope/);
  assert.match(html,/data-ai-emergency-submit disabled aria-disabled="true"/);
});

test('Foundation accepted outcome remains distinct from completion and displays Safe Core facts', () => {
  const html=render({targets:[target],selectedTarget:target,selectedAction:'KILL',blastRadius,transportAvailable:false,decision:acceptedDecision()});
  assert.match(html,/ACTION_ACCEPTED/);
  assert.match(html,/PRESERVED/);
  assert.match(html,/NOT_AUTHORIZED/);
  assert.match(html,/REQUIRED/);
  assert.match(html,/ev-1/);
  assert.doesNotMatch(html,/<span class="status-chip">ACTION_COMPLETED<\/span>/);
});

test('malformed or contradictory Foundation outcome is suppressed instead of displayed', () => {
  const html=render({
    targets:[target],selectedTarget:target,selectedAction:'KILL',blastRadius,transportAvailable:false,
    decision:acceptedDecision({impactedTargetIds:[],safeCorePreserved:false})
  });
  assert.match(html,/Malformed outcome suppressed/);
  assert.doesNotMatch(html,/ACCEPTED_TARGETED/);
});

test('hostile target, reason and evidence strings are output encoded', () => {
  const evil='<img src=x onerror=alert(1)>';
  const evilTarget={...target,id:evil};
  const html=render({
    targets:[evilTarget],selectedTarget:evilTarget,selectedAction:'KILL',
    blastRadius:{authoritative:true,freshness:'CURRENT',targetIds:[evil]},transportAvailable:false,
    decision:{requestId:evil,correlationId:'c1',targetId:evil,action:'RESTRICT',accepted:false,reason:evil,outcome:'DENIED',impactedTargetIds:[],safeCorePreserved:true,falconShutdownAuthorized:false,targetCooperationRequired:false,evidenceReference:evil}
  });
  assert.doesNotMatch(html,/<img/);
  assert.match(html,/&lt;img/);
});

test('Arabic surface communicates fail-closed runtime state', () => {
  const html=render({targets:[],selectedTarget:null,selectedAction:'KILL',blastRadius:null,transportAvailable:false},'ar');
  assert.match(html,/التحكم الطارئ بالـAI/);
  assert.match(html,/Runtime binding غير متاح/);
  assert.match(html,/الإرسال مقفول/);
});
