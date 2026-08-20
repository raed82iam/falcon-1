import { createRuntimeAdapter } from '../adapters.js';
import { createAuthAdapter } from '../auth.js';
import { createWebDataSource, DataSourceMode } from '../core/data-source-profile.js';
import { createWebIncidentRuntimePolicy } from '../core/web-incident-runtime-policy.js';
import { evaluateWebRuntimePreflight } from '../core/web-runtime-preflight.js';
import { createLocalSafeOwnerAuthAdapter, validateLocalSafeOwnerConfig } from '../local-safe-owner-auth.js';

const INJECTED_BINDING_KEY='__FALCON_WEB_RUNTIME_BINDINGS__';
const FORBIDDEN_SECRET_KEY=/^(?:secret|password|apiKey|accessToken|refreshToken|privateKey|clientSecret)$/i;
const FORBIDDEN_AUTHORITY_TRUE_KEYS=new Set([
  'businessAuthorityGranted','tradingAuthorityGranted','brokerAuthorityGranted','foundationAuthorityGranted',
  'killAuthorityGranted','runtimeActivationAuthorized','deploymentAuthorized','connectivityActivated'
]);
const OPAQUE_OR_DATA_KEYS=new Set([
  'credentialReferences','authoritativeData','authAdapter','runtimePort','ownerFsatsAccess','applicationAccess',
  'subscriptionModel','ownerGovernanceModel','ownerAiEmergencyModel'
]);

function isObject(value){
  return Boolean(value && typeof value==='object' && !Array.isArray(value));
}

function rejectSecretMaterial(value,path='bindings',seen=new Set()){
  if (!isObject(value) || seen.has(value)) return;
  seen.add(value);
  for (const [key,child] of Object.entries(value)) {
    if (FORBIDDEN_SECRET_KEY.test(key)) throw new TypeError(`${path}.${key} must not contain secret material`);
    if (OPAQUE_OR_DATA_KEYS.has(key)) continue;
    rejectSecretMaterial(child,`${path}.${key}`,seen);
  }
}

function rejectAuthorityEscalation(bindings,path='bindings'){
  if (!isObject(bindings)) return;
  for (const key of FORBIDDEN_AUTHORITY_TRUE_KEYS) {
    if (bindings[key] === true) throw new TypeError(`${path}.${key} cannot grant authority through Web runtime binding`);
  }
}

function validateBindingEnvelope(bindings){
  rejectSecretMaterial(bindings);
  rejectAuthorityEscalation(bindings);
}

function validAuthoritativeData(value){
  return Boolean(
    isObject(value)
    && value.sourceKind === 'AUTHORITATIVE_PUBLIC_CONTRACTS'
    && value.transportAuthorityCreated === false
  );
}

function tryFactory(factory,candidate){
  try { return Object.freeze({ ok:true, value:factory(candidate) }); }
  catch (error) { return Object.freeze({ ok:false, value:null, error }); }
}

function incidentPolicyInput(mode,incident={}){
  return {
    dataSourceMode:mode,
    productionPersistenceBinding:incident.productionPersistenceBinding ?? null,
    screenshotScanner:incident.screenshotScanner ?? null,
    localVoiceRuntime:incident.localVoiceRuntime ?? null,
    supportTransportPort:incident.supportTransportPort ?? null,
    principalId:incident.principalId ?? null,
    tenantId:incident.tenantId ?? null,
    sessionId:incident.sessionId ?? null
  };
}

export function readInjectedWebRuntimeBindings(globalRef=globalThis){
  const candidate=globalRef?.[INJECTED_BINDING_KEY];
  if (candidate === undefined || candidate === null) return null;
  if (!isObject(candidate)) throw new TypeError(`${INJECTED_BINDING_KEY} must be an object`);
  validateBindingEnvelope(candidate);
  return candidate;
}

/**
 * Single Web-owned composition root for Preview and governed Authoritative runtime.
 *
 * Preview remains the default when no external binding envelope is injected.
 * Authoritative mode is fail-closed: every required runtime dependency and the
 * unified runtime preflight must be valid before any authoritative data, auth,
 * incident dependency, entitlement or runtime port is exposed to the UI.
 */
export function createWebRuntimeBootstrap({ bindings=null, previewData=null }={}){
  if (bindings !== null && !isObject(bindings)) throw new TypeError('runtime bindings must be null or an object');
  if (bindings !== null) validateBindingEnvelope(bindings);

  const mode=bindings?.mode ?? DataSourceMode.PREVIEW;
  if (!Object.values(DataSourceMode).includes(mode)) throw new TypeError('unsupported Web runtime mode');

  if (mode === DataSourceMode.PREVIEW) {
    const localSafeOwner=bindings?.localSafeOwner ?? null;
    const localSafeOwnerEnabled=localSafeOwner !== null;
    if (localSafeOwnerEnabled) validateLocalSafeOwnerConfig(localSafeOwner);
    const dataSource=createWebDataSource({ mode, previewData });
    return Object.freeze({
      mode,
      authoritative:false,
      ready:false,
      blockers:Object.freeze(localSafeOwnerEnabled ? ['LOCAL_SAFE_DEMO_MODE','NO_LIVE_AUTHORITY'] : ['PREVIEW_MODE']),
      preflight:null,
      dataSource,
      auth:localSafeOwnerEnabled ? createLocalSafeOwnerAuthAdapter(localSafeOwner) : createAuthAdapter(),
      runtime:createRuntimeAdapter(),
      incidentPolicy:createWebIncidentRuntimePolicy({ dataSourceMode:mode }),
      ownerFsatsAccess:localSafeOwnerEnabled ? Object.freeze({
        available:true,fullVipFeatureSet:true,futureVipIncluded:true,commercialSubscription:false,trial:false,
        actionAuthorizationGranted:false,tradingExecutionAuthorityGranted:false,brokerAuthorityGranted:false,
        foundationAuthorityGranted:false,killAuthorityGranted:false,runtimeActivationAuthorized:false,deploymentAuthorized:false
      }) : null,
      applicationAccess:null,
      subscriptionModel:null,
      ownerGovernanceModel:null,
      ownerAiEmergencyModel:null
    });
  }

  const provider=bindings.provider ?? {};
  const incident=bindings.incident ?? {};
  const browser=bindings.browserVerification ?? {};
  const preflight=evaluateWebRuntimePreflight({ provider, incident, browser });
  const blockers=[];

  if (!preflight.provider.ready) blockers.push('GOVERNED_PROVIDER_BINDING_REQUIRED');
  if (!preflight.incident.ready) blockers.push(...preflight.incident.blockers);
  if (!preflight.browser.ready) blockers.push(...preflight.browser.missing.map(name=>`BROWSER_${String(name).toUpperCase()}_VERIFICATION_REQUIRED`));

  if (!validAuthoritativeData(bindings.authoritativeData)) blockers.push('AUTHORITATIVE_PUBLIC_CONTRACT_DATA_REQUIRED');

  const authCandidate=bindings.authAdapter ?? null;
  const authAttempt=authCandidate === null
    ? Object.freeze({ ok:false, value:null })
    : tryFactory(createAuthAdapter,authCandidate);
  if (!authAttempt.ok) blockers.push('AUTHORITATIVE_AUTH_ADAPTER_REQUIRED');

  const runtimeCandidate=bindings.runtimePort ?? null;
  const runtimeAttempt=runtimeCandidate === null
    ? Object.freeze({ ok:false, value:null })
    : tryFactory(createRuntimeAdapter,runtimeCandidate);
  if (!runtimeAttempt.ok) blockers.push('GOVERNED_RUNTIME_PORT_REQUIRED');

  const candidateIncidentPolicy=createWebIncidentRuntimePolicy(incidentPolicyInput(mode,incident));
  if (!candidateIncidentPolicy.productionReady) blockers.push('PRODUCTION_INCIDENT_POLICY_NOT_READY');

  const ready=preflight.ready && blockers.length===0;
  const dataSource=createWebDataSource({
    mode,
    authoritativeData:ready ? bindings.authoritativeData : null
  });

  return Object.freeze({
    mode,
    authoritative:ready,
    ready,
    blockers:Object.freeze([...new Set(blockers)]),
    preflight,
    dataSource,
    auth:ready ? authAttempt.value : createAuthAdapter(),
    runtime:ready ? runtimeAttempt.value : createRuntimeAdapter(),
    incidentPolicy:ready
      ? candidateIncidentPolicy
      : createWebIncidentRuntimePolicy({ dataSourceMode:DataSourceMode.AUTHORITATIVE }),
    ownerFsatsAccess:ready ? (bindings.ownerFsatsAccess ?? null) : null,
    applicationAccess:ready ? (bindings.applicationAccess ?? null) : null,
    subscriptionModel:ready ? (bindings.subscriptionModel ?? null) : null,
    ownerGovernanceModel:ready ? (bindings.ownerGovernanceModel ?? null) : null,
    ownerAiEmergencyModel:ready ? (bindings.ownerAiEmergencyModel ?? null) : null
  });
}

export const __test=Object.freeze({
  rejectSecretMaterial,
  rejectAuthorityEscalation,
  validAuthoritativeData,
  INJECTED_BINDING_KEY
});
