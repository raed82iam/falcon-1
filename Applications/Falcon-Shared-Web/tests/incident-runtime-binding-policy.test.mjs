import test from 'node:test';
import assert from 'node:assert/strict';
import { DataSourceMode } from '../src/core/data-source-profile.js';
import { createIncidentPersistenceBinding } from '../src/incidents/incident-persistence-binding.js';
import { createBrowserLocalVoiceBinding } from '../src/voice/browser-local-voice-binding.js';

test('authoritative incident persistence never falls back to local IndexedDB', async () => {
  const binding=createIncidentPersistenceBinding({ dataSourceMode:DataSourceMode.AUTHORITATIVE, productionBinding:null, indexedDBImpl:{} });
  assert.equal(binding.mode,'AUTHORITATIVE_FAIL_CLOSED');
  assert.equal(binding.productionReady,false);
  const result=await binding.port.loadRecord('incident-1');
  assert.equal(result.ok,false);
  assert.equal(result.reason,'PRODUCTION_TENANT_SCOPED_INCIDENT_PERSISTENCE_REQUIRED');
});

test('production incident persistence requires explicit tenant-scoped non-authority binding', () => {
  const unavailable=async()=>({ok:false});
  const port={
    saveRecord:unavailable,loadRecord:unavailable,appendEvent:unavailable,putArtifact:unavailable,getArtifact:unavailable,
    loadEvents:unavailable,commitRecordAndEvent:unavailable,commitArtifactAndEvents:unavailable
  };
  const accepted=createIncidentPersistenceBinding({
    dataSourceMode:DataSourceMode.AUTHORITATIVE,
    productionBinding:{
      authoritative:true,
      tenantScoped:true,
      businessAuthorityGranted:false,
      tenantNamespace:'tenant:customer-001',
      evidenceReference:'evidence:incident-persistence:1',
      port
    }
  });
  assert.equal(accepted.mode,'AUTHORITATIVE_TENANT_SCOPED');
  assert.equal(accepted.productionReady,true);
  assert.equal(accepted.port,port);

  const authorityLeak=createIncidentPersistenceBinding({
    dataSourceMode:DataSourceMode.AUTHORITATIVE,
    productionBinding:{
      authoritative:true,tenantScoped:true,businessAuthorityGranted:true,
      tenantNamespace:'tenant:customer-001',evidenceReference:'evidence:1',port
    }
  });
  assert.equal(authorityLeak.productionReady,false);
});

test('local voice remains unavailable unless runtime is explicitly injected', () => {
  const old=globalThis.FalconLocalVoiceRuntime;
  globalThis.FalconLocalVoiceRuntime={
    transcribeWithWhisperCpp(){ throw new Error('must not be discovered'); },
    synthesizeWithPiper(){ throw new Error('must not be discovered'); }
  };
  try {
    const binding=createBrowserLocalVoiceBinding();
    assert.equal(binding.readiness().speechToText,'UNAVAILABLE');
    assert.equal(binding.readiness().textToSpeech,'UNAVAILABLE');
    assert.equal(binding.readiness().bindingMode,'EXPLICIT_COMPOSITION_ONLY');
  } finally {
    if (old === undefined) delete globalThis.FalconLocalVoiceRuntime;
    else globalThis.FalconLocalVoiceRuntime=old;
  }
});

test('explicit local voice runtime is accepted without remote fallback', () => {
  const binding=createBrowserLocalVoiceBinding({ runtime:{
    transcribeWithWhisperCpp:async()=>({text:'ok'}),
    synthesizeWithPiper:async()=>new Uint8Array([1])
  }});
  const readiness=binding.readiness();
  assert.equal(readiness.speechToText,'READY');
  assert.equal(readiness.textToSpeech,'READY');
  assert.equal(readiness.remotePaidApiUsed,false);
});
