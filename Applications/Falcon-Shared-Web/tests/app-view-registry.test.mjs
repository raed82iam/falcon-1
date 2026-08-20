import test from 'node:test';
import assert from 'node:assert/strict';
import { createAppViewRegistry, isCustomerWorkspaceRoute } from '../src/composition/app-view-registry.js';

function fixtureViews() {
  const names = [
    'publicHome','applicationsPage','fsatsLanding','myApplicationsPage','dashboardPage',
    'portfolioPage','activityPage','marketsPage','advisoryMarketsPage','aiPage',
    'notificationsPage','settingsPage','ownerHome','owner','ownerApps','ownerIncidents','ownerApprovals',
    'ownerAiEmergencyPage','ownerProviderActionsPage','ownerUsers','ownerAudit','ownerSettings',
    'ownerSimulator'
  ];
  const views = Object.fromEntries(names.map(name => [name, () => name]));
  views.placeholder = (route,ownerMode) => `placeholder:${route}:${ownerMode}`;
  return views;
}

test('view registry keeps public customer and Owner route mappings stable', () => {
  const registry = createAppViewRegistry(fixtureViews());

  assert.equal(registry.viewFor('home')(),'publicHome');
  assert.equal(registry.viewFor('trader')(),'dashboardPage');
  assert.equal(registry.viewFor('owner-home')(),'ownerHome');
  assert.equal(registry.viewFor('owner')(),'owner');
  assert.equal(registry.viewFor('owner-ai-emergency')(),'ownerAiEmergencyPage');
  assert.equal(registry.viewFor('missing')(),'placeholder:missing:false');
  assert.equal(registry.viewFor('owner-future')(),'placeholder:owner-future:true');
});

test('customer incident overlay scope remains explicit and does not include Owner routes', () => {
  assert.equal(isCustomerWorkspaceRoute('trader'),true);
  assert.equal(isCustomerWorkspaceRoute('settings'),true);
  assert.equal(isCustomerWorkspaceRoute('owner-home'),false);
  assert.equal(isCustomerWorkspaceRoute('owner'),false);
  assert.equal(isCustomerWorkspaceRoute('owner-incidents'),false);
  assert.equal(isCustomerWorkspaceRoute('home'),false);
});

test('view registry fails closed when a required view factory is missing', () => {
  const views = fixtureViews();
  delete views.ownerHome;
  assert.throws(() => createAppViewRegistry(views),/ownerHome must be a function/);
});
