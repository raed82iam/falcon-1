/**
 * Shared Web route registry.
 *
 * Routes are presentation destinations only. They do not grant identity,
 * entitlement, business authority, or execution authority.
 */

export const SurfaceKind = Object.freeze({
  PUBLIC: 'PUBLIC',
  USER: 'USER',
  OWNER: 'OWNER'
});

export const AppRoute = Object.freeze({
  HOME: 'home',
  APPLICATIONS: 'apps',
  LOGIN: 'login',
  REGISTER: 'register',
  FSATS_PUBLIC: 'fsats',
  MY_APPLICATIONS: 'my-apps',
  TRADER_HOME: 'trader',
  MARKETS: 'markets',
  ADVISORY_MARKETS: 'advisory-markets',
  PORTFOLIO: 'portfolio',
  ACTIVITY: 'activity',
  AI: 'ai',
  NOTIFICATIONS: 'notifications',
  SETTINGS: 'settings',
  OWNER_HOME: 'owner-home',
  OWNER_COMMAND_CENTER: 'owner',
  OWNER_APPLICATIONS: 'owner-apps',
  OWNER_INCIDENTS: 'owner-incidents',
  OWNER_APPROVALS: 'owner-approvals',
  OWNER_AI_EMERGENCY: 'owner-ai-emergency',
  OWNER_PROVIDER_ACTIONS: 'owner-provider-actions',
  OWNER_USERS: 'owner-users',
  OWNER_AUDIT: 'owner-audit',
  OWNER_SETTINGS: 'owner-settings',
  OWNER_SIMULATOR: 'owner-simulator'
});

const ROUTE_META = Object.freeze({
  [AppRoute.HOME]: { surface: SurfaceKind.PUBLIC },
  [AppRoute.APPLICATIONS]: { surface: SurfaceKind.PUBLIC },
  [AppRoute.LOGIN]: { surface: SurfaceKind.PUBLIC },
  [AppRoute.REGISTER]: { surface: SurfaceKind.PUBLIC },
  [AppRoute.FSATS_PUBLIC]: { surface: SurfaceKind.PUBLIC },
  [AppRoute.MY_APPLICATIONS]: { surface: SurfaceKind.USER },
  [AppRoute.TRADER_HOME]: { surface: SurfaceKind.USER, application: 'FSATS' },
  [AppRoute.MARKETS]: { surface: SurfaceKind.USER, application: 'FSATS' },
  [AppRoute.ADVISORY_MARKETS]: { surface: SurfaceKind.USER, application: 'FSATS' },
  [AppRoute.PORTFOLIO]: { surface: SurfaceKind.USER, application: 'FSATS' },
  [AppRoute.ACTIVITY]: { surface: SurfaceKind.USER, application: 'FSATS' },
  [AppRoute.AI]: { surface: SurfaceKind.USER, application: 'FSATS' },
  [AppRoute.NOTIFICATIONS]: { surface: SurfaceKind.USER },
  [AppRoute.SETTINGS]: { surface: SurfaceKind.USER },
  [AppRoute.OWNER_HOME]: { surface: SurfaceKind.OWNER },
  [AppRoute.OWNER_COMMAND_CENTER]: { surface: SurfaceKind.OWNER },
  [AppRoute.OWNER_APPLICATIONS]: { surface: SurfaceKind.OWNER },
  [AppRoute.OWNER_INCIDENTS]: { surface: SurfaceKind.OWNER },
  [AppRoute.OWNER_APPROVALS]: { surface: SurfaceKind.OWNER },
  [AppRoute.OWNER_AI_EMERGENCY]: { surface: SurfaceKind.OWNER },
  [AppRoute.OWNER_PROVIDER_ACTIONS]: { surface: SurfaceKind.OWNER },
  [AppRoute.OWNER_USERS]: { surface: SurfaceKind.OWNER },
  [AppRoute.OWNER_AUDIT]: { surface: SurfaceKind.OWNER },
  [AppRoute.OWNER_SETTINGS]: { surface: SurfaceKind.OWNER },
  [AppRoute.OWNER_SIMULATOR]: { surface: SurfaceKind.OWNER }
});

const ROUTES = new Set(Object.values(AppRoute));

export function normalizeRoute(value) {
  const candidate = String(value ?? '').trim().replace(/^#\/?/, '').replace(/^\/+/, '');
  return ROUTES.has(candidate) ? candidate : AppRoute.HOME;
}

export function routeMeta(route) {
  return ROUTE_META[normalizeRoute(route)];
}

export function routeBelongsToSurface(route, surface) {
  return routeMeta(route).surface === surface;
}

export function routeHash(route) {
  return `#/${normalizeRoute(route)}`;
}
