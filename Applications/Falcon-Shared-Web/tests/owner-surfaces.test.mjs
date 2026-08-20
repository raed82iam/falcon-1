import test from 'node:test';
import assert from 'node:assert/strict';
import { createOwnerSurfaces } from '../src/composition/owner-surfaces.js';

const t = key => key;
const language = () => 'en';
const workspace = (html) => html;
const data = Object.freeze({
  owner:{ health:'CURRENT', apps:0, users:0, incidents:0, approvals:0 },
  services:[],
  incidents:[],
  ownerProviderActions:[]
});

test('Owner surfaces compose command center, provider actions and emergency page without granting transport', () => {
  const surfaces = createOwnerSurfaces({ t, language, workspace, data, session:() => null });

  assert.equal(typeof surfaces.owner,'function');
  assert.equal(typeof surfaces.ownerProviderActionsPage,'function');
  assert.equal(typeof surfaces.ownerAiEmergencyPage,'function');

  const emergency = surfaces.ownerAiEmergencyPage();
  assert.match(emergency,/KILL|kill|owner/i);
});

test('Owner surface composition fails closed without mandatory dependencies', () => {
  assert.throws(() => createOwnerSurfaces({ t:null, language, workspace, data, session:() => null }),/t must be a function/);
  assert.throws(() => createOwnerSurfaces({ t, language, workspace, data:null, session:() => null }),/data is required/);
  assert.throws(() => createOwnerSurfaces({ t, language, workspace, data, session:null }),/session must be a function/);
});
