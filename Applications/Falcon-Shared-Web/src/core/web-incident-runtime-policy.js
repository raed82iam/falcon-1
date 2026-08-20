import { createIncidentPersistenceBinding } from '../incidents/incident-persistence-binding.js';
import { createBrowserLocalVoiceBinding } from '../voice/browser-local-voice-binding.js';
import { createUnavailableIncidentSupportTransportPort } from './ports/incident-support-transport-port.js';

const nonEmpty=value=>typeof value==='string'&&value.trim().length>0;

function validScanner(scanner){
  return Boolean(scanner&&typeof scanner.scanScreenshot==='function');
}

/**
 * Central Web-owned incident runtime composition policy.
 *
 * Production capabilities are explicit injected dependencies. This module never
 * discovers globals, substitutes preview persistence for authoritative mode,
 * invents Support delivery, or falls back to a remote paid voice service.
 */
export function createWebIncidentRuntimePolicy({
  dataSourceMode,
  indexedDBImpl=globalThis.indexedDB,
  productionPersistenceBinding=null,
  screenshotScanner=null,
  localVoiceRuntime=null,
  supportTransportPort=null,
  principalId=null,
  tenantId=null,
  sessionId=null
}={}){
  const persistence=createIncidentPersistenceBinding({
    dataSourceMode,
    indexedDBImpl,
    productionBinding:productionPersistenceBinding
  });
  const voice=createBrowserLocalVoiceBinding({runtime:localVoiceRuntime});
  const support=supportTransportPort??createUnavailableIncidentSupportTransportPort();
  const tenantIdentityReady=nonEmpty(principalId)&&nonEmpty(tenantId)&&nonEmpty(sessionId);
  const scannerReady=validScanner(screenshotScanner);
  const voiceState=voice.readiness();
  const voiceReady=voiceState.speechToText==='READY'&&voiceState.textToSpeech==='READY'&&voiceState.remotePaidApiUsed===false;
  const supportReady=typeof supportTransportPort?.requestSupport==='function';
  const productionReady=Boolean(
    persistence.productionReady===true
    && tenantIdentityReady
    && scannerReady
    && voiceReady
    && supportReady
  );

  return Object.freeze({
    persistence,
    screenshotScanner:scannerReady?screenshotScanner:null,
    localVoiceRuntime:voiceReady?localVoiceRuntime:null,
    supportTransportPort:support,
    tenantIdentityReady,
    scannerReady,
    voiceReady,
    supportReady,
    productionReady,
    principalId:tenantIdentityReady?principalId:null,
    tenantId:tenantIdentityReady?tenantId:null,
    sessionId:tenantIdentityReady?sessionId:null,
    businessAuthorityGranted:false,
    tradingAuthorityGranted:false
  });
}
