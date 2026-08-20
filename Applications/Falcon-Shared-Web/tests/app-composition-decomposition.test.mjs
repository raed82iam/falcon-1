import test from 'node:test';
import assert from 'node:assert/strict';
import { readFile } from 'node:fs/promises';

const appUrl = new URL('../src/app.js', import.meta.url);
const incidentRuntimeUrl = new URL('../src/composition/incident-ui-runtime.js', import.meta.url);
const uiBindingsUrl = new URL('../src/composition/app-ui-bindings.js', import.meta.url);

test('app composition delegates incident browser orchestration to its Web-owned runtime module', async () => {
  const source = await readFile(appUrl, 'utf8');
  const bindings = await readFile(uiBindingsUrl,'utf8');

  assert.match(source, /createIncidentUiRuntime/);
  assert.match(source, /incidentRuntime\?\.markup\?\.\(\)/);
  assert.match(bindings, /incidentRuntime\.bindActions\(\)/);
  assert.match(source, /incidentRuntime\.initialize\(\)/);

  for (const forbiddenImport of [
    'features/incidents/customer-incident.js',
    'adapters/fsats-incident-followup-v1.js',
    'voice/browser-microphone.js',
    'voice/browser-local-voice-binding.js',
    'voice/incident-voice-controller.js',
    'voice/live-voice-session.js',
    'incidents/incident-persistence.js',
    'incidents/incident-controller.js',
    'incidents/screenshot-upload-controller.js'
  ]) {
    assert.equal(source.includes(forbiddenImport), false, `app.js regained incident runtime dependency: ${forbiddenImport}`);
  }
});

test('main app composition consumes one governed runtime bootstrap instead of hard-coded Preview/runtime nulls', async () => {
  const source = await readFile(appUrl,'utf8');
  assert.match(source,/createWebRuntimeBootstrap/);
  assert.match(source,/readInjectedWebRuntimeBindings/);
  assert.match(source,/runtimeBootstrap\.dataSource/);
  assert.match(source,/runtimeBootstrap\.auth/);
  assert.match(source,/runtimeBootstrap\.incidentPolicy/);
  assert.match(source,/ownerFsatsAccess:runtimeBootstrap\.ownerFsatsAccess/);
  assert.match(source,/applicationAccess:runtimeBootstrap\.applicationAccess/);
  assert.doesNotMatch(source,/DataSourceMode\.PREVIEW/);
  assert.doesNotMatch(source,/applicationAccess:null/);
  assert.doesNotMatch(source,/ownerFsatsAccess:null/);
});

test('Owner traversal into customer workspace passes the separately governed FSATS feature entitlement', async () => {
  const source = await readFile(appUrl,'utf8');
  assert.match(source,/canAccessRoute\(currentRoute, authenticatedSession, \{ ownerFsatsAccess:runtimeBootstrap\.ownerFsatsAccess \}\)/);
});

test('incident UI runtime remains Web-owned orchestration without direct network or foreign-workstream coupling', async () => {
  const source = await readFile(incidentRuntimeUrl, 'utf8');

  assert.equal(/\bfetch\s*\(|\bWebSocket\s*\(|\bEventSource\s*\(/.test(source), false);
  assert.equal(/applications\/FSATS|applications\/docs\/FSATS|(?:^|\/)foundation(?:\/|$)/im.test(source), false);
  assert.match(source, /createIncidentController/);
  assert.match(source, /createIncidentVoiceController/);
  assert.match(source, /createIncidentScreenshotUploadController/);
});
