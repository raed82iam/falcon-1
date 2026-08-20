import test from 'node:test';
import assert from 'node:assert/strict';
import { createOwnerHomeFeature } from '../src/features/owner-home/owner-home.js';
import { AuthResult, WebSurfaceGrant } from '../src/auth.js';

const authoritative={
  state:AuthResult.AUTHENTICATED,
  authoritativeSession:true,
  principalId:'owner-1',
  sessionId:'session-1',
  businessAuthorityGranted:false,
  role:'PROJECT_OWNER',
  surfaceGrants:[WebSurfaceGrant.OWNER]
};

function render(session){
  return createOwnerHomeFeature({language:()=> 'en',session:()=>session,ownerFsatsAccess:null}).ownerHome();
}

function commandCenterButton(html){
  const match=html.match(/<button type="button" class="primary" data-nav="owner"[^>]*>/u);
  assert.ok(match,'Owner Command Center button must exist');
  return match[0];
}

function fsatsButton(html){
  const match=html.match(/<button type="button" class="primary" data-nav="trader"[^>]*>/u);
  assert.ok(match,'Owner FSATS button must exist');
  return match[0];
}

test('canonical authoritative Owner session enables navigation to Command Center only',()=>{
  const html=render(authoritative);
  assert.doesNotMatch(commandCenterButton(html),/disabled/u);
  assert.match(fsatsButton(html),/disabled aria-disabled="true"/u);
  assert.match(html,/FCR-0242/u);
});

test('role and surface grant are insufficient without canonical principal and session identity',()=>{
  const html=render({...authoritative,principalId:null,sessionId:null});
  assert.match(commandCenterButton(html),/disabled aria-disabled="true"/u);
  assert.match(html,/Owner session is not authoritative/u);
});

test('business authority leakage invalidates Web authoritative-session state',()=>{
  const html=render({...authoritative,businessAuthorityGranted:true});
  assert.match(commandCenterButton(html),/disabled aria-disabled="true"/u);
  assert.match(html,/Owner session is not authoritative/u);
});

test('missing Owner surface grant keeps Owner navigation fail closed',()=>{
  const html=render({...authoritative,surfaceGrants:[]});
  assert.match(commandCenterButton(html),/disabled aria-disabled="true"/u);
});
