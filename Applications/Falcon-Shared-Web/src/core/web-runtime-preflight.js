import { evaluateProviderBindingProfile, ProviderBindingProfileState } from './provider-binding-profile.js';
import { createBrowserLocalVoiceBinding } from '../voice/browser-local-voice-binding.js';

const nonEmpty = value => typeof value === 'string' && value.trim().length > 0;

function validPersistence(binding) {
  const required=['saveRecord','loadRecord','appendEvent','putArtifact','getArtifact','loadEvents','commitRecordAndEvent','commitArtifactAndEvents'];
  return Boolean(
    binding?.authoritative === true
    && binding?.tenantScoped === true
    && binding?.businessAuthorityGranted === false
    && nonEmpty(binding?.tenantNamespace)
    && nonEmpty(binding?.evidenceReference)
    && required.every(name => typeof binding?.port?.[name] === 'function')
  );
}

function validScanner(scanner) {
  return Boolean(scanner && typeof scanner.scanScreenshot === 'function');
}

function validSupportTransport(port) {
  return Boolean(port && typeof port.requestSupport === 'function');
}

function browserCapabilityState(browser = {}) {
  const required = Object.freeze({
    document:browser.documentAvailable === true,
    keyboard:browser.keyboardEvents === true,
    focus:browser.focusManagement === true,
    rtl:browser.rtlLayout === true,
    ltr:browser.ltrLayout === true,
    mobile:browser.mobileViewport === true,
    indexedDb:browser.indexedDb === true,
    microphone:browser.microphoneApi === true,
    blob:browser.blobApi === true,
    objectUrl:browser.objectUrlApi === true
  });
  const missing=Object.entries(required).filter(([,ready])=>!ready).map(([name])=>name);
  return Object.freeze({ ready:missing.length===0, missing:Object.freeze(missing), capabilities:required });
}

/**
 * Unified Web-owned readiness report for final runtime/browser verification.
 *
 * This module never resolves provider secrets, creates principals, activates
 * routes, connects to providers, deploys the Web app, or grants business/trading
 * authority. It only evaluates explicitly injected governed runtime metadata.
 */
export function evaluateWebRuntimePreflight({
  provider = {},
  incident = {},
  browser = {}
} = {}) {
  const providerProfile=evaluateProviderBindingProfile({
    webPrincipalId:provider.webPrincipalId,
    webServiceRole:provider.webServiceRole,
    bindingsByFcr:provider.bindingsByFcr ?? {}
  });

  const voice=createBrowserLocalVoiceBinding({ runtime:incident.localVoiceRuntime ?? null }).readiness();
  const persistenceReady=validPersistence(incident.productionPersistenceBinding);
  const scannerReady=validScanner(incident.screenshotScanner);
  const supportReady=validSupportTransport(incident.supportTransportPort);
  const tenantIdentityReady=nonEmpty(incident.principalId) && nonEmpty(incident.tenantId) && nonEmpty(incident.sessionId);
  const voiceReady=voice.speechToText === 'READY' && voice.textToSpeech === 'READY' && voice.remotePaidApiUsed === false;
  const browserState=browserCapabilityState(browser);

  const incidentBlockers=[];
  if (!tenantIdentityReady) incidentBlockers.push('AUTHORITATIVE_PRINCIPAL_TENANT_SESSION_REQUIRED');
  if (!persistenceReady) incidentBlockers.push('PRODUCTION_TENANT_SCOPED_PERSISTENCE_REQUIRED');
  if (!scannerReady) incidentBlockers.push('GOVERNED_SCREENSHOT_SCANNER_REQUIRED');
  if (!supportReady) incidentBlockers.push('GOVERNED_SUPPORT_TRANSPORT_REQUIRED');
  if (!voiceReady) incidentBlockers.push('LOCAL_WHISPER_PIPER_RUNTIME_REQUIRED');

  const providerReady=providerProfile.state === ProviderBindingProfileState.READY;
  const incidentReady=incidentBlockers.length===0;
  const ready=providerReady && incidentReady && browserState.ready;

  return Object.freeze({
    ready,
    state:ready?'READY_FOR_FINAL_RUNTIME_VERIFICATION':'BLOCKED',
    provider:Object.freeze({
      ready:providerReady,
      state:providerProfile.state,
      totalRoutes:providerProfile.totalRoutes,
      readyRoutes:providerProfile.readyRoutes,
      routes:providerProfile.routes
    }),
    incident:Object.freeze({
      ready:incidentReady,
      tenantIdentityReady,
      persistenceReady,
      scannerReady,
      supportReady,
      voiceReady,
      voice,
      blockers:Object.freeze(incidentBlockers)
    }),
    browser:browserState,
    connectivityActivated:false,
    deploymentAuthorized:false,
    businessAuthorityGranted:false,
    tradingAuthorityGranted:false
  });
}
