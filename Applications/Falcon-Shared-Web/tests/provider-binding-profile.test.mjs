import test from 'node:test';
import assert from 'node:assert/strict';
import { evaluateProviderBindingProfile, requiredProviderBindingFcrs, ProviderBindingProfileState } from '../src/core/provider-binding-profile.js';

const expectedFcrs=['FCR-0173','FCR-0174','FCR-0175','FCR-0176','FCR-0177','FCR-0196','FCR-0197','FCR-0198','FCR-0199','FCR-0200'];

function completeBindings(){
  return {
    'FCR-0173':{routePolicyBound:true,exactRouteVerified:true},
    'FCR-0174':{routePolicyBound:true,exactRouteVerified:true,channelCredentialRequired:false},
    'FCR-0175':{routePolicyBound:true,exactRouteVerified:true},
    'FCR-0176':{routePolicyBound:true,exactRouteVerified:true,credentialReference:'credref:web:alpaca-iex'},
    'FCR-0177':{routePolicyBound:true,exactRouteVerified:true,credentialReference:'credref:web:finnhub'},
    'FCR-0196':{routePolicyBound:true,exactRouteVerified:true,credentialReference:'credref:web:alpaca-universe'},
    'FCR-0197':{routePolicyBound:true,exactRouteVerified:true,credentialReference:'credref:web:alpaca-history'},
    'FCR-0198':{routePolicyBound:true,exactRouteVerified:true},
    'FCR-0199':{routePolicyBound:true,exactRouteVerified:true},
    'FCR-0200':{routePolicyBound:true,exactRouteVerified:true}
  };
}

test('provider profile covers every current Stage12 Web presentation FCR exactly once',()=>{
  assert.deepEqual([...requiredProviderBindingFcrs()].sort(),[...expectedFcrs].sort());
  assert.equal(new Set(requiredProviderBindingFcrs()).size,expectedFcrs.length);
});

test('missing runtime principal and policy context stays fully fail closed',()=>{
  const profile=evaluateProviderBindingProfile();
  assert.equal(profile.state,ProviderBindingProfileState.FAIL_CLOSED);
  assert.equal(profile.readyRoutes,0);
  assert.equal(profile.connectivityActivated,false);
  for(const route of profile.routes) assert.equal(route.connectivityActivated,false);
});

test('complete separately supplied binding metadata is readiness only and never activates connectivity',()=>{
  const profile=evaluateProviderBindingProfile({
    webPrincipalId:'principal:web:shared-falcon-web',
    webServiceRole:'service-role:web:presentation-market-data',
    bindingsByFcr:completeBindings()
  });
  assert.equal(profile.state,ProviderBindingProfileState.READY);
  assert.equal(profile.readyRoutes,10);
  assert.equal(profile.connectivityActivated,false);
  for(const route of profile.routes){
    assert.equal(route.decision,'READY_FOR_GOVERNED_VERIFICATION',route.fcr);
    assert.equal(route.connectivityActivated,false,route.fcr);
  }
});

test('credential-bearing routes reject raw secrets and missing references',()=>{
  const bindings=completeBindings();
  bindings['FCR-0176']={routePolicyBound:true,exactRouteVerified:true,credentialReference:'sk-secret-value'};
  bindings['FCR-0197']={routePolicyBound:true,exactRouteVerified:true};
  const profile=evaluateProviderBindingProfile({
    webPrincipalId:'principal:web:shared-falcon-web',
    webServiceRole:'service-role:web:presentation-market-data',
    bindingsByFcr:bindings
  });
  assert.equal(profile.state,ProviderBindingProfileState.PARTIAL);
  const byFcr=Object.fromEntries(profile.routes.map(route=>[route.fcr,route]));
  assert.equal(byFcr['FCR-0176'].reason,'INVALID_WEB_CREDENTIAL_REFERENCE');
  assert.equal(byFcr['FCR-0197'].reason,'CREDENTIAL_REFERENCE_REQUIRED');
});

test('channel-dependent Coinbase auth remains fail closed while requirement is unknown',()=>{
  const bindings=completeBindings();
  delete bindings['FCR-0174'].channelCredentialRequired;
  const profile=evaluateProviderBindingProfile({
    webPrincipalId:'principal:web:shared-falcon-web',
    webServiceRole:'service-role:web:presentation-market-data',
    bindingsByFcr:bindings
  });
  const coinbase=profile.routes.find(route=>route.fcr==='FCR-0174');
  assert.equal(coinbase.reason,'CHANNEL_AUTH_REQUIREMENT_UNKNOWN');
  assert.equal(coinbase.connectivityActivated,false);
});

test('route policy or governed verification omissions cannot be hidden by other ready routes',()=>{
  const bindings=completeBindings();
  bindings['FCR-0200'].routePolicyBound=false;
  bindings['FCR-0198'].exactRouteVerified=false;
  const profile=evaluateProviderBindingProfile({
    webPrincipalId:'principal:web:shared-falcon-web',
    webServiceRole:'service-role:web:presentation-market-data',
    bindingsByFcr:bindings
  });
  const byFcr=Object.fromEntries(profile.routes.map(route=>[route.fcr,route]));
  assert.equal(byFcr['FCR-0200'].reason,'WEB_ROUTE_POLICY_NOT_BOUND');
  assert.equal(byFcr['FCR-0198'].reason,'WEB_GOVERNED_VERIFICATION_PENDING');
  assert.equal(profile.connectivityActivated,false);
});
