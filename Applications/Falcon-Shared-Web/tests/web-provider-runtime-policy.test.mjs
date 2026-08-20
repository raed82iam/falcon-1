import test from 'node:test';
import assert from 'node:assert/strict';
import { createWebProviderRuntimePolicy, requiredWebProviderCredentialFcrs } from '../src/core/web-provider-runtime-policy.js';
import { evaluateProviderBindingProfile } from '../src/core/provider-binding-profile.js';

const refs=Object.freeze({
  'FCR-0176':'credref:web:alpaca:iex',
  'FCR-0177':'credref:web:finnhub:market-data',
  'FCR-0196':'credref:web:alpaca:assets',
  'FCR-0197':'credref:web:alpaca:bars'
});

test('provider policy binds all ten exact routes without activating connectivity',()=>{
  const policy=createWebProviderRuntimePolicy();
  assert.equal(Object.keys(policy.bindingsByFcr).length,10);
  assert.equal(policy.bindingsByFcr['FCR-0174'].channelCredentialRequired,false);
  assert.equal(policy.connectivityActivated,false);
  assert.equal(policy.businessAuthorityGranted,false);
  assert.equal(policy.tradingAuthorityGranted,false);
});

test('only exact Web credential-reference routes require injected references',()=>{
  assert.deepEqual([...requiredWebProviderCredentialFcrs()].sort(),['FCR-0176','FCR-0177','FCR-0196','FCR-0197']);
});

test('complete governed metadata reaches readiness without claiming connection execution',()=>{
  const policy=createWebProviderRuntimePolicy({
    webPrincipalId:'principal:shared-web:market-presentation',
    webServiceRole:'service-role:shared-web:market-presentation',
    credentialReferences:refs
  });
  const result=evaluateProviderBindingProfile(policy);
  assert.equal(result.state,'READY');
  assert.equal(result.totalRoutes,10);
  assert.equal(result.readyRoutes,10);
  assert.equal(result.connectivityActivated,false);
  assert.ok(result.routes.every(route=>route.connectivityActivated===false));
});

test('missing credential references remain fail closed even when policy metadata exists',()=>{
  const policy=createWebProviderRuntimePolicy({
    webPrincipalId:'principal:shared-web:market-presentation',
    webServiceRole:'service-role:shared-web:market-presentation'
  });
  const result=evaluateProviderBindingProfile(policy);
  assert.notEqual(result.state,'READY');
  assert.ok(result.routes.some(route=>route.reason==='CREDENTIAL_REFERENCE_REQUIRED'));
});
