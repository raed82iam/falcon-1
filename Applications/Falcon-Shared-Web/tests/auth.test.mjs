import test from 'node:test';
import assert from 'node:assert/strict';
import { AuthProvider, AuthResult, UnavailableAuthAdapter, WebSurfaceGrant, routeAuthenticatedIdentity, canAccessRoute, isAuthoritativeSession } from '../src/auth.js';

const baseSession={state:AuthResult.AUTHENTICATED,authoritativeSession:true,sessionId:'session-1',businessAuthorityGranted:false};
const ownerSession={...baseSession,role:'PROJECT_OWNER',principalId:'owner-1',capabilities:['INCIDENT_SUPPORT_TAKEOVER'],surfaceGrants:[WebSurfaceGrant.OWNER]};
const customerSession={...baseSession,sessionId:'session-2',role:'CUSTOMER',principalId:'customer-1',capabilities:[],surfaceGrants:[WebSurfaceGrant.CUSTOMER]};
const supportSession={...baseSession,sessionId:'session-3',role:'SUPPORT',principalId:'support-1',capabilities:['INCIDENT_SUPPORT_TAKEOVER'],surfaceGrants:[]};

test('unavailable auth source does not fabricate identity', async () => {
  const result=await new UnavailableAuthAdapter().signIn();
  assert.equal(result.state,AuthResult.UNAVAILABLE);
  assert.equal(result.authoritativeSession,false);
  assert.equal(routeAuthenticatedIdentity(result),null);
});

test('federated providers remain fail-closed without authoritative binding', async () => {
  const auth = new UnavailableAuthAdapter();
  for (const provider of [AuthProvider.GOOGLE, AuthProvider.MICROSOFT]) {
    const result = await auth.signInWithProvider(provider);
    assert.equal(result.state, AuthResult.UNAVAILABLE);
    assert.equal(result.provider, provider);
    assert.equal(routeAuthenticatedIdentity(result), null);
  }
});

test('unsupported provider is rejected instead of guessed', async () => {
  const result = await new UnavailableAuthAdapter().signInWithProvider('UNKNOWN');
  assert.equal(result.state, AuthResult.REJECTED);
  assert.equal(routeAuthenticatedIdentity(result), null);
});

test('MFA verification cannot mint identity without authoritative binding', async () => {
  const result = await new UnavailableAuthAdapter().verifyMfa({ code:'123456' });
  assert.equal(result.state, AuthResult.UNAVAILABLE);
  assert.equal(routeAuthenticatedIdentity(result), null);
});

test('AUTHENTICATED token without authoritative session evidence still fails closed', () => {
  assert.equal(isAuthoritativeSession({state:AuthResult.AUTHENTICATED,role:'PROJECT_OWNER'}),false);
  assert.equal(routeAuthenticatedIdentity({state:AuthResult.AUTHENTICATED,role:'PROJECT_OWNER'}),null);
});

test('authoritative Owner identity plus explicit Owner surface grant routes to Owner Home', () => {
  assert.equal(routeAuthenticatedIdentity(ownerSession),'owner-home');
});

test('authoritative Customer identity plus explicit Customer surface grant routes to My Applications', () => {
  assert.equal(routeAuthenticatedIdentity(customerSession),'my-apps');
});

test('role fact alone does not create route authority', () => {
  assert.equal(routeAuthenticatedIdentity({...ownerSession,surfaceGrants:[]}),null);
  assert.equal(routeAuthenticatedIdentity({...customerSession,surfaceGrants:[]}),null);
  assert.equal(canAccessRoute('owner-home',{...ownerSession,surfaceGrants:[]}),false);
  assert.equal(canAccessRoute('owner',{...ownerSession,surfaceGrants:[]}),false);
  assert.equal(canAccessRoute('trader',{...customerSession,surfaceGrants:[]}),false);
});

test('business authority must remain false for a Web authoritative session', () => {
  assert.equal(isAuthoritativeSession({...ownerSession,businessAuthorityGranted:true}),false);
  assert.equal(canAccessRoute('owner-home',{...ownerSession,businessAuthorityGranted:true}),false);
});

test('unknown or Support roles do not get a customer destination by inference', () => {
  assert.equal(routeAuthenticatedIdentity(supportSession),null);
  assert.equal(routeAuthenticatedIdentity({...supportSession,role:'UNKNOWN'}),null);
});

test('owner and authenticated workspace routes fail closed without authoritative session', () => {
  for (const route of ['owner-home','owner','owner-incidents','owner-ai-emergency','my-apps','trader','portfolio','activity','markets','advisory-markets','ai','notifications','settings']) {
    assert.equal(canAccessRoute(route,null),false,route);
  }
  assert.equal(canAccessRoute('home',null),true);
  assert.equal(canAccessRoute('apps',null),true);
});

test('Owner routes require Owner grant while customer routes require Customer grant', () => {
  assert.equal(canAccessRoute('owner-home',customerSession),false);
  assert.equal(canAccessRoute('owner',customerSession),false);
  assert.equal(canAccessRoute('owner-incidents',customerSession),false);
  assert.equal(canAccessRoute('owner-ai-emergency',customerSession),false);
  assert.equal(canAccessRoute('portfolio',customerSession),true);
  assert.equal(canAccessRoute('advisory-markets',customerSession),true);
  assert.equal(canAccessRoute('owner-home',ownerSession),true);
  assert.equal(canAccessRoute('owner',ownerSession),true);
  assert.equal(canAccessRoute('owner-ai-emergency',ownerSession),true);
});

test('Project Owner session cannot cross into customer trading surfaces without separate entitlement/access binding', () => {
  for (const route of ['my-apps','trader','portfolio','activity','markets','advisory-markets','ai','notifications','settings']) {
    assert.equal(canAccessRoute(route,ownerSession),false,route);
  }
});

test('Support and unknown roles cannot cross into Owner or customer surfaces', () => {
  for (const route of ['owner-home','owner','owner-incidents','my-apps','trader','portfolio','markets','advisory-markets','ai','notifications']) {
    assert.equal(canAccessRoute(route,supportSession),false,route);
    assert.equal(canAccessRoute(route,{...supportSession,role:'UNKNOWN'}),false,route);
  }
});
