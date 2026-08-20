import test from 'node:test';
import assert from 'node:assert/strict';
import { WebMarketProviderRoutes } from '../src/core/ports/web-market-data-port.js';
import { PendingMarketDataDestinations } from '../src/core/market-data-destinations.pending.js';
import { evaluateProviderBindingReadiness, ProviderBindingDecision, isWebCredentialReference } from '../src/core/provider-binding-readiness.js';

const base = route => ({
  route,
  expectedFcr:route.fcr,
  expectedUrl:route.url,
  expectedPathTemplate:route.pathTemplate ?? null,
  webPrincipalId:'shared-web-provider-presentation',
  webServiceRole:'SHARED_WEB_PRESENTATION_PROVIDER',
  routePolicyBound:true,
  exactRouteVerified:true,
  ...(route.credentialMode === 'CHANNEL_DEPENDENT' ? { channelCredentialRequired:false } : {})
});

test('public exact route can become verification-ready without implying connectivity', () => {
  const result = evaluateProviderBindingReadiness(base(WebMarketProviderRoutes.BINANCE));
  assert.equal(result.decision, ProviderBindingDecision.READY_FOR_GOVERNED_VERIFICATION);
  assert.equal(result.connectivityActivated, false);
});

test('exact FCR, URL and path template identity are all required', () => {
  const route = WebMarketProviderRoutes.BINANCE;

  const wrongFcr = evaluateProviderBindingReadiness({
    ...base(route),
    expectedFcr:'FCR-9999'
  });
  assert.equal(wrongFcr.reason,'EXACT_FCR_ROUTE_IDENTITY_MISMATCH');

  const wrongUrl = evaluateProviderBindingReadiness({
    ...base(route),
    expectedUrl:'wss://example.invalid'
  });
  assert.equal(wrongUrl.reason,'EXACT_ROUTE_MISMATCH');

  const wrongPath = evaluateProviderBindingReadiness({
    ...base(route),
    expectedPathTemplate:'/ws/{symbol}@trade'
  });
  assert.equal(wrongPath.reason,'EXACT_ROUTE_PATH_TEMPLATE_MISMATCH');
});

test('credentialed route requires an opaque Web credential reference', () => {
  const missing = evaluateProviderBindingReadiness(base(WebMarketProviderRoutes.ALPACA_IEX));
  assert.equal(missing.decision, ProviderBindingDecision.FAIL_CLOSED);
  assert.equal(missing.reason,'CREDENTIAL_REFERENCE_REQUIRED');

  const present = evaluateProviderBindingReadiness({
    ...base(WebMarketProviderRoutes.ALPACA_IEX),
    credentialReference:'credref:web:alpaca-iex:primary'
  });
  assert.equal(present.decision, ProviderBindingDecision.READY_FOR_GOVERNED_VERIFICATION);
  assert.equal(present.connectivityActivated,false);
});

test('raw secret-like strings are not accepted as Web credential references', () => {
  const rejected = [
    'sk-live-1234567890',
    'token=abcdef123456',
    'Bearer abc.def.ghi',
    'password:super-secret',
    'credref:fsapma:alpaca:primary',
    ' credref:web:alpaca:primary',
    'credref:web:alpaca:primary?token=secret'
  ];

  for (const value of rejected) {
    assert.equal(isWebCredentialReference(value),false,value);
    const result = evaluateProviderBindingReadiness({
      ...base(WebMarketProviderRoutes.ALPACA_IEX),
      credentialReference:value
    });
    assert.equal(result.decision,ProviderBindingDecision.FAIL_CLOSED,value);
    assert.equal(result.reason,'INVALID_WEB_CREDENTIAL_REFERENCE',value);
  }

  assert.equal(isWebCredentialReference('credref:web:alpaca-iex:primary'),true);
});

test('public route rejects an unnecessary credential reference', () => {
  const result = evaluateProviderBindingReadiness({
    ...base(WebMarketProviderRoutes.BYBIT),
    credentialReference:'credref:web:must-not-bind-here'
  });
  assert.equal(result.decision, ProviderBindingDecision.FAIL_CLOSED);
  assert.equal(result.reason,'UNNEEDED_CREDENTIAL_REFERENCE');
});

test('channel-dependent route fails closed until channel authentication requirement is explicit', () => {
  const unknown = evaluateProviderBindingReadiness({
    ...base(WebMarketProviderRoutes.COINBASE),
    channelCredentialRequired:null
  });
  assert.equal(unknown.reason,'CHANNEL_AUTH_REQUIREMENT_UNKNOWN');

  const credentialRequired = evaluateProviderBindingReadiness({
    ...base(WebMarketProviderRoutes.COINBASE),
    channelCredentialRequired:true
  });
  assert.equal(credentialRequired.reason,'CREDENTIAL_REFERENCE_REQUIRED');

  const invalidCredential = evaluateProviderBindingReadiness({
    ...base(WebMarketProviderRoutes.COINBASE),
    channelCredentialRequired:true,
    credentialReference:'raw-provider-token'
  });
  assert.equal(invalidCredential.reason,'INVALID_WEB_CREDENTIAL_REFERENCE');

  const credentialed = evaluateProviderBindingReadiness({
    ...base(WebMarketProviderRoutes.COINBASE),
    channelCredentialRequired:true,
    credentialReference:'credref:web:coinbase:channel'
  });
  assert.equal(credentialed.decision,ProviderBindingDecision.READY_FOR_GOVERNED_VERIFICATION);
});

test('missing Web principal, policy binding or governed verification stays closed', () => {
  const noPrincipal = evaluateProviderBindingReadiness({
    ...base(WebMarketProviderRoutes.COINBASE),
    webPrincipalId:''
  });
  assert.equal(noPrincipal.reason,'WEB_PRINCIPAL_OR_SERVICE_ROLE_MISSING');

  const noPolicy = evaluateProviderBindingReadiness({
    ...base(WebMarketProviderRoutes.COINBASE),
    routePolicyBound:false
  });
  assert.equal(noPolicy.reason,'WEB_ROUTE_POLICY_NOT_BOUND');

  const unverified = evaluateProviderBindingReadiness({
    ...base(WebMarketProviderRoutes.COINBASE),
    exactRouteVerified:false
  });
  assert.equal(unverified.reason,'WEB_GOVERNED_VERIFICATION_PENDING');
});

test('all five Stage 12 REST/broad-market destinations use the same fail-closed binding guard', () => {
  assert.equal(PendingMarketDataDestinations.length,5);

  for (const route of PendingMarketDataDestinations) {
    const input = base(route);
    const missingCredential = route.credentialMode === 'API_CREDENTIAL_REFERENCE';
    const result = evaluateProviderBindingReadiness(input);

    if (missingCredential) {
      assert.equal(result.decision,ProviderBindingDecision.FAIL_CLOSED,route.fcr);
      assert.equal(result.reason,'CREDENTIAL_REFERENCE_REQUIRED',route.fcr);
      const withReference = evaluateProviderBindingReadiness({
        ...input,
        credentialReference:`credref:web:${route.fcr.toLowerCase()}`
      });
      assert.equal(withReference.decision,ProviderBindingDecision.READY_FOR_GOVERNED_VERIFICATION,route.fcr);
      assert.equal(withReference.connectivityActivated,false,route.fcr);
    } else {
      assert.equal(result.decision,ProviderBindingDecision.READY_FOR_GOVERNED_VERIFICATION,route.fcr);
      assert.equal(result.connectivityActivated,false,route.fcr);
    }
  }
});
