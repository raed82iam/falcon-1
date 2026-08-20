const CustomerWorkspaceRoutes = Object.freeze(new Set([
  'my-apps',
  'trader',
  'portfolio',
  'activity',
  'markets',
  'advisory-markets',
  'ai',
  'notifications',
  'settings'
]));

export function isCustomerWorkspaceRoute(route) {
  return CustomerWorkspaceRoutes.has(route);
}

/**
 * Web-owned route-to-view composition registry.
 *
 * The registry selects presentation factories only. Route authorization stays
 * in the authentication boundary and runtime truth stays behind Web ports.
 */
export function createAppViewRegistry(views = {}) {
  const required = [
    'publicHome','applicationsPage','fsatsLanding','myApplicationsPage','dashboardPage',
    'portfolioPage','activityPage','marketsPage','advisoryMarketsPage','aiPage',
    'notificationsPage','settingsPage','ownerHome','owner','ownerApps','ownerIncidents','ownerApprovals',
    'ownerAiEmergencyPage','ownerProviderActionsPage','ownerUsers','ownerAudit','ownerSettings',
    'ownerSimulator','placeholder'
  ];

  for (const name of required) {
    if (typeof views[name] !== 'function') throw new TypeError(`${name} must be a function`);
  }

  const registry = Object.freeze({
    home:views.publicHome,
    apps:views.applicationsPage,
    fsats:views.fsatsLanding,
    login:views.fsatsLanding,
    register:views.fsatsLanding,
    'my-apps':views.myApplicationsPage,
    trader:views.dashboardPage,
    portfolio:views.portfolioPage,
    activity:views.activityPage,
    markets:views.marketsPage,
    'advisory-markets':views.advisoryMarketsPage,
    ai:views.aiPage,
    notifications:views.notificationsPage,
    settings:views.settingsPage,
    'owner-home':views.ownerHome,
    owner:views.owner,
    'owner-apps':views.ownerApps,
    'owner-incidents':views.ownerIncidents,
    'owner-approvals':views.ownerApprovals,
    'owner-ai-emergency':views.ownerAiEmergencyPage,
    'owner-provider-actions':views.ownerProviderActionsPage,
    'owner-users':views.ownerUsers,
    'owner-audit':views.ownerAudit,
    'owner-settings':views.ownerSettings,
    'owner-simulator':views.ownerSimulator
  });

  return Object.freeze({
    viewFor(route) {
      return registry[route] ?? (() => views.placeholder(route,String(route ?? '').startsWith('owner-')));
    },
    has(route) {
      return Object.hasOwn(registry,route);
    }
  });
}
