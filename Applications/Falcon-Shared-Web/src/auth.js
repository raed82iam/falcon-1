export const AuthResult = Object.freeze({
  AUTHENTICATED:'AUTHENTICATED',
  MFA_REQUIRED:'MFA_REQUIRED',
  UNAVAILABLE:'UNAVAILABLE',
  REJECTED:'REJECTED'
});

export const AuthProvider = Object.freeze({
  PASSWORD:'PASSWORD',
  GOOGLE:'GOOGLE',
  MICROSOFT:'MICROSOFT'
});

export const WebSurfaceGrant = Object.freeze({
  OWNER:'OWNER',
  CUSTOMER:'CUSTOMER'
});

const CUSTOMER_ROUTES = Object.freeze([
  'my-apps',
  'trader',
  'portfolio',
  'activity',
  'markets',
  'advisory-markets',
  'ai',
  'notifications',
  'settings'
]);

const unavailableResult = (provider = null) => ({
  state:AuthResult.UNAVAILABLE,
  provider,
  role:null,
  applications:[],
  challenge:null,
  authoritativeSession:false,
  principalId:null,
  surfaceGrants:[]
});

/**
 * Fail-closed default adapter.
 *
 * Shared Web owns presentation and request transport only. Live provider
 * redirects, Falcon identity/session consumption, Web access binding and MFA
 * challenge verification require separately governed authoritative bindings.
 */
export class UnavailableAuthAdapter {
  async signIn(){
    return unavailableResult(AuthProvider.PASSWORD);
  }

  async signInWithProvider(provider){
    if (!Object.values(AuthProvider).includes(provider) || provider === AuthProvider.PASSWORD) {
      return { ...unavailableResult(null), state:AuthResult.REJECTED };
    }
    return unavailableResult(provider);
  }

  async verifyMfa(){
    return unavailableResult(null);
  }
}

export function assertAuthAdapter(candidate) {
  if (!candidate || typeof candidate !== 'object') throw new TypeError('auth adapter must be an object');
  for (const method of ['signIn','signInWithProvider','verifyMfa']) {
    if (typeof candidate[method] !== 'function') throw new TypeError(`auth adapter is missing method: ${method}`);
  }
  return candidate;
}

function hasSurfaceGrant(session, grant) {
  return Array.isArray(session?.surfaceGrants) && session.surfaceGrants.includes(grant);
}

export function isAuthoritativeSession(session) {
  return Boolean(
    session
    && session.state === AuthResult.AUTHENTICATED
    && session.authoritativeSession === true
    && typeof session.principalId === 'string'
    && session.principalId.length > 0
    && typeof session.sessionId === 'string'
    && session.sessionId.length > 0
    && session.businessAuthorityGranted === false
  );
}

export function hasVerifiedOwnerFsatsFeatureAccess(access) {
  return Boolean(
    access
    && access.available === true
    && access.fullVipFeatureSet === true
    && access.futureVipIncluded === true
    && access.commercialSubscription === false
    && access.trial === false
    && access.actionAuthorizationGranted === false
    && access.tradingExecutionAuthorityGranted === false
    && access.brokerAuthorityGranted === false
    && access.foundationAuthorityGranted === false
    && access.killAuthorityGranted === false
    && access.runtimeActivationAuthorized === false
    && access.deploymentAuthorized === false
  );
}

/**
 * Role facts alone never create Web surface access. A separately governed Web
 * access binding must provide the matching surface grant.
 */
export function routeAuthenticatedIdentity(result){
  if (!isAuthoritativeSession(result)) return null;
  if (result.role === 'PROJECT_OWNER' && hasSurfaceGrant(result, WebSurfaceGrant.OWNER)) return 'owner-home';
  if (result.role === 'CUSTOMER' && hasSurfaceGrant(result, WebSurfaceGrant.CUSTOMER)) return 'my-apps';
  return null;
}

export function canAccessRoute(route, session, { ownerFsatsAccess = null } = {}) {
  const name = String(route ?? '');
  const ownerRoute = name === 'owner-home' || name === 'owner' || name.startsWith('owner-');
  const customerRoute = CUSTOMER_ROUTES.includes(name);

  if (!ownerRoute && !customerRoute) return true;
  if (!isAuthoritativeSession(session)) return false;

  if (ownerRoute) {
    return session.role === 'PROJECT_OWNER' && hasSurfaceGrant(session, WebSurfaceGrant.OWNER);
  }

  if (session.role === 'PROJECT_OWNER') {
    return hasSurfaceGrant(session, WebSurfaceGrant.OWNER) && hasVerifiedOwnerFsatsFeatureAccess(ownerFsatsAccess);
  }

  return session.role === 'CUSTOMER' && hasSurfaceGrant(session, WebSurfaceGrant.CUSTOMER);
}

export function createAuthAdapter(candidate = null){
  // Live provider/authentication activation remains separately governed.
  return candidate === null ? new UnavailableAuthAdapter() : assertAuthAdapter(candidate);
}
