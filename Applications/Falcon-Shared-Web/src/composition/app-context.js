import { normalizeRoute, routeHash } from '../platform/navigation/routes.js';

/**
 * Explicit composition context passed into feature renderers.
 *
 * Feature modules receive only what they need from Web-owned composition.
 * They do not discover Foundation/Application internals or transports.
 */
export function createAppContext({ i18n, store, demo, auth, runtime } = {}) {
  if (!i18n || typeof i18n.t !== 'function') throw new TypeError('i18n service is required');
  if (!store) throw new TypeError('store is required');
  if (!demo) throw new TypeError('demo fixture registry is required');
  if (!auth) throw new TypeError('auth adapter is required');

  const t = key => i18n.t(key);

  return Object.freeze({
    i18n,
    t,
    store,
    demo,
    auth,
    runtime: runtime ?? null,
    currentRoute() {
      return normalizeRoute(globalThis.location?.hash ?? '');
    },
    navigate(route) {
      if (!globalThis.location) return routeHash(route);
      globalThis.location.hash = routeHash(route);
      return globalThis.location.hash;
    }
  });
}
