import test from 'node:test';
import assert from 'node:assert/strict';
import { AppRoute, SurfaceKind, normalizeRoute, routeBelongsToSurface, routeHash } from '../src/platform/navigation/routes.js';
import { createAppViewRegistry } from '../src/composition/app-view-registry.js';
import { canAccessRoute } from '../src/auth.js';

function fixtureViews(){
  const names=[
    'publicHome','applicationsPage','fsatsLanding','myApplicationsPage','dashboardPage',
    'portfolioPage','activityPage','marketsPage','advisoryMarketsPage','aiPage','notificationsPage','settingsPage',
    'ownerHome','owner','ownerApps','ownerIncidents','ownerApprovals','ownerAiEmergencyPage','ownerProviderActionsPage',
    'ownerUsers','ownerAudit','ownerSettings','ownerSimulator','placeholder'
  ];
  return Object.fromEntries(names.map(name=>[name,()=>name]));
}

test('register is a canonical public route and creates no authentication requirement',()=>{
  assert.equal(AppRoute.REGISTER,'register');
  assert.equal(normalizeRoute('#/register'),'register');
  assert.equal(routeHash('register'),'#/register');
  assert.equal(routeBelongsToSurface('register',SurfaceKind.PUBLIC),true);
  assert.equal(canAccessRoute('register',null),true);
});

test('register reuses public FSATS onboarding presentation rather than a protected workspace',()=>{
  const views=fixtureViews();
  const registry=createAppViewRegistry(views);
  assert.equal(registry.viewFor('register')(), 'fsatsLanding');
  assert.equal(registry.viewFor('trader')(), 'dashboardPage');
});
