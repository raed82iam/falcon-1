import test from 'node:test';
import assert from 'node:assert/strict';
import { createOwnerSurfaces } from '../src/composition/owner-surfaces.js';

const t=key=>key;
const language=()=> 'en';
const workspace=body=>body;
const data={owner:{health:'UNAVAILABLE',apps:'—',users:'—',incidents:'—',approvals:'—'},services:[],incidents:[],ownerProviderActions:[]};
const session=()=>({authoritativeSession:true,role:'PROJECT_OWNER',principalId:'owner-1',capabilities:[]});

test('Owner emergency surface defaults fail-closed',()=>{
  const surfaces=createOwnerSurfaces({t,language,workspace,data,session});
  const html=surfaces.ownerAiEmergencyPage();
  assert.match(html,/Runtime binding unavailable/);
  assert.match(html,/disabled aria-disabled="true"/);
});

test('Owner emergency surface can consume an injected model without creating authority in composition',()=>{
  const surfaces=createOwnerSurfaces({
    t,language,workspace,data,session,
    ownerAiEmergencyModel:{
      targets:[{id:'ai-1',scope:'ONE_AI_COMPONENT',truth:'CURRENT',freshness:'CURRENT'}],
      selectedTarget:{id:'ai-1',scope:'ONE_AI_COMPONENT',truth:'CURRENT',freshness:'CURRENT'},
      selectedAction:'SUSPEND',
      blastRadius:{authoritative:true,freshness:'CURRENT',targetIds:['ai-1']},
      decision:null,
      transportAvailable:false
    }
  });
  const html=surfaces.ownerAiEmergencyPage();
  assert.match(html,/ai-1/);
  assert.match(html,/Runtime binding unavailable/);
  assert.match(html,/REQUEST_SENT ≠ ACTION_ACCEPTED ≠ ACTION_COMPLETED/);
});
