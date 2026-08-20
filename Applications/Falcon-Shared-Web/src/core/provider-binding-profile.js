import { WebMarketProviderRoutes } from './ports/web-market-data-port.js';
import { PendingMarketDataDestinations } from './market-data-destinations.pending.js';
import { evaluateProviderBindingReadiness } from './provider-binding-readiness.js';

const ALL_ROUTES = Object.freeze([
  WebMarketProviderRoutes.BINANCE,
  WebMarketProviderRoutes.COINBASE,
  WebMarketProviderRoutes.BYBIT,
  WebMarketProviderRoutes.ALPACA_IEX,
  WebMarketProviderRoutes.FINNHUB,
  ...PendingMarketDataDestinations
]);

const EXPECTED = Object.freeze(Object.fromEntries(ALL_ROUTES.map(route => [route.fcr,Object.freeze({
  fcr:route.fcr,
  url:route.url,
  pathTemplate:route.pathTemplate ?? null
})])));

const nonEmpty = value => typeof value === 'string' && value.trim().length > 0;

export const ProviderBindingProfileState = Object.freeze({
  READY:'READY',
  PARTIAL:'PARTIAL',
  FAIL_CLOSED:'FAIL_CLOSED'
});

/**
 * Build a Web-owned presentation-provider binding profile without activating
 * connectivity or resolving credential secrets.
 *
 * `bindingsByFcr` must come from separately governed runtime/deployment state.
 * This module never invents a principal, service role, policy decision or
 * credential reference. Missing values keep the corresponding route fail-closed.
 */
export function evaluateProviderBindingProfile({
  webPrincipalId,
  webServiceRole,
  bindingsByFcr = {}
} = {}) {
  const principalReady = nonEmpty(webPrincipalId) && nonEmpty(webServiceRole);
  const results = ALL_ROUTES.map(route => {
    const expected = EXPECTED[route.fcr];
    const binding = bindingsByFcr?.[route.fcr] ?? {};
    const result = evaluateProviderBindingReadiness({
      route,
      expectedFcr:expected.fcr,
      expectedUrl:expected.url,
      expectedPathTemplate:expected.pathTemplate,
      webPrincipalId,
      webServiceRole,
      credentialReference:binding.credentialReference ?? null,
      channelCredentialRequired:binding.channelCredentialRequired ?? null,
      routePolicyBound:binding.routePolicyBound === true,
      exactRouteVerified:binding.exactRouteVerified === true
    });
    return Object.freeze({
      fcr:route.fcr,
      provider:route.provider ?? null,
      market:route.market ?? null,
      purpose:route.purpose ?? null,
      decision:result.decision,
      reason:result.reason,
      connectivityActivated:false
    });
  });

  const readyCount = results.filter(item => item.decision === 'READY_FOR_GOVERNED_VERIFICATION').length;
  const state = readyCount === results.length && principalReady
    ? ProviderBindingProfileState.READY
    : readyCount > 0
      ? ProviderBindingProfileState.PARTIAL
      : ProviderBindingProfileState.FAIL_CLOSED;

  return Object.freeze({
    state,
    webPrincipalId:principalReady ? webPrincipalId : null,
    webServiceRole:principalReady ? webServiceRole : null,
    totalRoutes:results.length,
    readyRoutes:readyCount,
    connectivityActivated:false,
    routes:Object.freeze(results)
  });
}

export function requiredProviderBindingFcrs() {
  return Object.freeze(ALL_ROUTES.map(route => route.fcr));
}
