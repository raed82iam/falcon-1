import test from 'node:test';
import assert from 'node:assert/strict';
import { DataSourceMode } from '../src/core/data-source-profile.js';
import { createWebIncidentRuntimePolicy } from '../src/core/web-incident-runtime-policy.js';

const noop=async()=>({ok:true});
const port=Object.freeze({
  saveRecord:noop,loadRecord:noop,appendEvent:noop,putArtifact:noop,getArtifact:noop,loadEvents:noop,
  commitRecordAndEvent:noop,commitArtifactAndEvents:noop
});
const productionPersistence=Object.freeze({
  authoritative:true,tenantScoped:true,businessAuthorityGranted:false,
  tenantNamespace:'tenant:1/incidents',evidenceReference:'evidence:web:incident:persistence:1',port
});
const scanner=Object.freeze({scanScreenshot:async()=>({state:'PASS'})});
const voice=Object.freeze({transcribeWithWhisperCpp:async()=>({text:''}),synthesizeWithPiper:async()=>new Blob([])});
const support=Object.freeze({requestSupport:async()=>({accepted:false})});

test('authoritative incident runtime never falls back to preview persistence',()=>{
  const result=createWebIncidentRuntimePolicy({dataSourceMode:DataSourceMode.AUTHORITATIVE});
  assert.equal(result.persistence.productionReady,false);
  assert.equal(result.persistence.mode,'AUTHORITATIVE_FAIL_CLOSED');
  assert.equal(result.productionReady,false);
  assert.equal(result.businessAuthorityGranted,false);
  assert.equal(result.tradingAuthorityGranted,false);
});

test('complete explicitly injected production incident dependencies become verification-ready only',()=>{
  const result=createWebIncidentRuntimePolicy({
    dataSourceMode:DataSourceMode.AUTHORITATIVE,
    productionPersistenceBinding:productionPersistence,
    screenshotScanner:scanner,
    localVoiceRuntime:voice,
    supportTransportPort:support,
    principalId:'principal:customer:1',tenantId:'tenant:1',sessionId:'session:1'
  });
  assert.equal(result.persistence.productionReady,true);
  assert.equal(result.tenantIdentityReady,true);
  assert.equal(result.scannerReady,true);
  assert.equal(result.voiceReady,true);
  assert.equal(result.supportReady,true);
  assert.equal(result.productionReady,true);
  assert.equal(result.businessAuthorityGranted,false);
  assert.equal(result.tradingAuthorityGranted,false);
});

test('missing scanner or local voice runtime keeps production readiness closed',()=>{
  const result=createWebIncidentRuntimePolicy({
    dataSourceMode:DataSourceMode.AUTHORITATIVE,
    productionPersistenceBinding:productionPersistence,
    supportTransportPort:support,
    principalId:'principal:customer:1',tenantId:'tenant:1',sessionId:'session:1'
  });
  assert.equal(result.scannerReady,false);
  assert.equal(result.voiceReady,false);
  assert.equal(result.productionReady,false);
});
