import test from 'node:test';
import assert from 'node:assert/strict';
import { createOwnerCommandCenterFeature } from '../src/features/owner-command-center/owner-command-center.js';

const t=key=>key;
const workspace=body=>body;
const language=()=> 'en';

function feature(extra={}) {
  return createOwnerCommandCenterFeature({
    t,language,workspace,
    data:{owner:{health:'UNAVAILABLE',apps:'—',users:'—',incidents:'—',approvals:'—'},services:[],incidents:[],...extra}
  });
}

test('unavailable system health is not styled as positive',()=>{
  const html=feature().owner();
  assert.match(html,/class="muted">UNAVAILABLE</);
  assert.doesNotMatch(html,/class="positive">UNAVAILABLE</);
});

test('Owner users page does not invent synthetic users',()=>{
  const html=feature().ownerUsers();
  assert.match(html,/User projection unavailable/);
  assert.doesNotMatch(html,/User 01/);
  assert.doesNotMatch(html,/User 02/);
});

test('Owner audit page does not invent synthetic audit timestamps',()=>{
  const html=feature().ownerAudit();
  assert.match(html,/Audit projection unavailable/);
  assert.doesNotMatch(html,/10:21/);
  assert.doesNotMatch(html,/10:15/);
  assert.doesNotMatch(html,/09:58/);
});

test('unbound conversational and control actions remain disabled',()=>{
  const html=feature().owner();
  assert.match(html,/chat-input/);
  assert.match(html,/disabled aria-disabled="true"/);
  assert.match(html,/Owner conversational transport is unavailable/);
});

test('authoritative supplied user and audit projections render without fabrication',()=>{
  const f=feature({ownerUsers:[{principalId:'p-1',displayName:'Alice',status:'ACTIVE',system:'FSATS',truthState:'CURRENT'}],ownerAudit:[{time:'2026-08-18T15:00:00Z',message:'Owner reviewed evidence',evidenceReference:'ev-1'}]});
  assert.match(f.ownerUsers(),/Alice/);
  assert.match(f.ownerAudit(),/Owner reviewed evidence/);
  assert.match(f.ownerAudit(),/ev-1/);
});
