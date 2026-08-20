import test from 'node:test';
import assert from 'node:assert/strict';
import { evaluateWebRuntimePreflight } from '../src/core/web-runtime-preflight.js';

const browserReady=Object.freeze({
  documentAvailable:true,
  keyboardEvents:true,
  focusManagement:true,
  rtlLayout:true,
  ltrLayout:true,
  mobileViewport:true,
  indexedDb:true,
  microphoneApi:true,
  blobApi:true,
  objectUrlApi:true
});

const noop=async()=>({ok:true});
const persistencePort=Object.freeze({
  saveRecord:noop,loadRecord:noop,appendEvent:noop,putArtifact:noop,getArtifact:noop,loadEvents:noop,
  commitRecordAndEvent:noop,commitArtifactAndEvents:noop
});

const incidentReady=Object.freeze({
  principalId:'principal:web:user:1',
  tenantId:'tenant:1',
  sessionId:'session:1',
  productionPersistenceBinding:Object.freeze({
    authoritative:true,tenantScoped:true,businessAuthorityGranted:false,
    tenantNamespace:'tenant:1/incidents',evidenceReference:'evidence:web:incident:persistence:1',port:persistencePort
  }),
  screenshotScanner:Object.freeze({scanScreenshot:async()=>({state:'PASS'})}),
  supportTransportPort:Object.freeze({requestSupport:async()=>({accepted:false})}),
  localVoiceRuntime:Object.freeze({
    transcribeWithWhisperCpp:async()=>({text:''}),
    synthesizeWithPiper:async()=>new Blob([])
  })
});

test('missing runtime bindings fail closed and never create authority',()=>{
  const result=evaluateWebRuntimePreflight();
  assert.equal(result.ready,false);
  assert.equal(result.state,'BLOCKED');
  assert.equal(result.provider.ready,false);
  assert.equal(result.incident.ready,false);
  assert.equal(result.browser.ready,false);
  assert.equal(result.connectivityActivated,false);
  assert.equal(result.deploymentAuthorized,false);
  assert.equal(result.businessAuthorityGranted,false);
  assert.equal(result.tradingAuthorityGranted,false);
});

test('incident and browser readiness cannot hide missing governed provider binding',()=>{
  const result=evaluateWebRuntimePreflight({ incident:incidentReady, browser:browserReady });
  assert.equal(result.incident.ready,true);
  assert.equal(result.browser.ready,true);
  assert.equal(result.provider.ready,false);
  assert.equal(result.ready,false);
});

test('browser preflight reports exact missing capability',()=>{
  const result=evaluateWebRuntimePreflight({ browser:{...browserReady,mobileViewport:false} });
  assert.deepEqual(result.browser.missing,['mobile']);
});

test('incident preflight enumerates production blockers instead of falling back to preview facilities',()=>{
  const result=evaluateWebRuntimePreflight({
    incident:{principalId:'p',tenantId:'t',sessionId:'s'},
    browser:browserReady
  });
  assert.equal(result.incident.tenantIdentityReady,true);
  assert.equal(result.incident.persistenceReady,false);
  assert.equal(result.incident.scannerReady,false);
  assert.equal(result.incident.supportReady,false);
  assert.equal(result.incident.voiceReady,false);
  assert.deepEqual(result.incident.blockers,[
    'PRODUCTION_TENANT_SCOPED_PERSISTENCE_REQUIRED',
    'GOVERNED_SCREENSHOT_SCANNER_REQUIRED',
    'GOVERNED_SUPPORT_TRANSPORT_REQUIRED',
    'LOCAL_WHISPER_PIPER_RUNTIME_REQUIRED'
  ]);
});
