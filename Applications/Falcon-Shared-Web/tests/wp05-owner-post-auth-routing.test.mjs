import test from 'node:test';
import assert from 'node:assert/strict';
import { AuthResult, WebSurfaceGrant, routeAuthenticatedIdentity, canAccessRoute } from '../src/auth.js';
import { AppRoute, SurfaceKind, normalizeRoute, routeHash, routeBelongsToSurface } from '../src/platform/navigation/routes.js';
import { createAppViewRegistry } from '../src/composition/app-view-registry.js';

const ownerSession={
  state:AuthResult.AUTHENTICATED,
  authoritativeSession:true,
  principalId:'owner-1',
  sessionId:'session-owner-1',
  businessAuthorityGranted:false,
  role:'PROJECT_OWNER',
  surfaceGrants:[WebSurfaceGrant.OWNER]
};

function fixtureViews(){
  const names=[
    'publicHome','applicationsPage','fsatsLanding','myApplicationsPage','dashboardPage',
    'portfolioPage','activityPage','marketsPage','advisoryMarketsPage','aiPage','notificationsPage','settingsPage',
    'ownerHome','owner','ownerApps','ownerIncidents','ownerApprovals','ownerAiEmergencyPage','ownerProviderActionsPage',
    'ownerUsers','ownerAudit','ownerSettings','ownerSimulator','placeholder'
  ];
  return Object.fromEntries(names.map(name=>[name,()=>name]));
}

test('authoritative Project Owner resolves to canonical Owner Home route end-to-end',()=>{
  const destination=routeAuthenticatedIdentity(ownerSession);
  assert.equal(destination,'owner-home');
  assert.equal(AppRoute.OWNER_HOME,'owner-home');
  assert.equal(routeHash(destination),'#/owner-home');
  assert.equal(normalizeRoute('#/owner-home'),'owner-home');
  assert.equal(routeBelongsToSurface(destination,SurfaceKind.OWNER),true);
  assert.equal(canAccessRoute(destination,ownerSession),true);
});

test('Owner Home and Owner Command Center remain distinct destinations',()=>{
  assert.equal(AppRoute.OWNER_COMMAND_CENTER,'owner');
  assert.notEqual(AppRoute.OWNER_HOME,AppRoute.OWNER_COMMAND_CENTER);
  const registry=createAppViewRegistry(fixtureViews());
  assert.equal(registry.viewFor(AppRoute.OWNER_HOME)(),'ownerHome');
  assert.equal(registry.viewFor(AppRoute.OWNER_COMMAND_CENTER)(),'owner');
});

test('Owner role or OWNER surface grant does not unlock customer workspace routes',()=>{
  for(const route of ['my-apps','trader','portfolio','markets','ai','settings']){
    assert.equal(canAccessRoute(route,ownerSession),false,route);
  }
});

test('unauthenticated navigation cannot enter Owner Home or Command Center',()=>{
  assert.equal(canAccessRoute(AppRoute.OWNER_HOME,null),false);
  assert.equal(canAccessRoute(AppRoute.OWNER_COMMAND_CENTER,null),false);
});
