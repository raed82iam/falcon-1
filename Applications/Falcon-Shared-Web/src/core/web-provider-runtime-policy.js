const EXACT_PROVIDER_FCRS=Object.freeze([
  'FCR-0173','FCR-0174','FCR-0175','FCR-0176','FCR-0177',
  'FCR-0196','FCR-0197','FCR-0198','FCR-0199','FCR-0200'
]);

const CREDENTIAL_FCRS=new Set(['FCR-0176','FCR-0177','FCR-0196','FCR-0197']);

/**
 * Web-owned policy declaration for the exact Stage12 presentation routes.
 *
 * Route policy and exact-route verification are source/governance facts. Runtime
 * service principal identities and opaque credential references remain injected
 * deployment facts. Secret bytes are never accepted here.
 *
 * Coinbase FCR-0174 is deliberately constrained to unauthenticated public market
 * presentation use on ws-feed. Authenticated/private Coinbase channels are out of
 * scope and would require a new governed binding rather than silently widening
 * this policy.
 */
export function createWebProviderRuntimePolicy({
  webPrincipalId=null,
  webServiceRole=null,
  credentialReferences={}
}={}) {
  const bindingsByFcr={};
  for(const fcr of EXACT_PROVIDER_FCRS) {
    bindingsByFcr[fcr]={
      routePolicyBound:true,
      exactRouteVerified:true,
      credentialReference:CREDENTIAL_FCRS.has(fcr) ? (credentialReferences?.[fcr] ?? null) : null
    };
  }
  bindingsByFcr['FCR-0174'].channelCredentialRequired=false;

  return Object.freeze({
    webPrincipalId,
    webServiceRole,
    bindingsByFcr:Object.freeze(Object.fromEntries(Object.entries(bindingsByFcr).map(([key,value])=>[key,Object.freeze(value)]))),
    policyIdentity:'policy:shared-web:presentation-provider-routes:v1',
    connectivityActivated:false,
    businessAuthorityGranted:false,
    tradingAuthorityGranted:false
  });
}

export function requiredWebProviderCredentialFcrs(){
  return Object.freeze([...CREDENTIAL_FCRS]);
}
