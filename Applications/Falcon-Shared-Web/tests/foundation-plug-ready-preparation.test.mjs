import test from 'node:test';
import assert from 'node:assert/strict';
import { readFile } from 'node:fs/promises';
import { fileURLToPath } from 'node:url';
import { dirname, resolve } from 'node:path';
import { evaluateWebFoundationPlugReadyPreparation } from '../src/core/foundation-plug-ready-preflight.js';

const here = dirname(fileURLToPath(import.meta.url));
const root = resolve(here,'..');
const preparation = JSON.parse(await readFile(resolve(root,'governance/WEB_FOUNDATION_PLUG_READY_PREPARATION_V1.json'),'utf8'));
const manifest = JSON.parse(await readFile(resolve(root,'governance/SHARED_WEB_APPLICATION_ADMISSION_MANIFEST_V1.json'),'utf8'));

test('Web materializes exactly one admission/runtime-registration preparation pair', () => {
  assert.equal(preparation.application.identity,'FALCON_SHARED_WEB');
  assert.equal(preparation.application.admissionCandidateCount,1);
  assert.equal(preparation.application.runtimeRegistrationTemplateCount,1);
  assert.equal(preparation.application.requestPairCount,1);
  assert.equal(preparation.admissionCandidate.applicationIdentity,manifest.application.identity);
  assert.equal(preparation.admissionCandidate.manifestIdentity,manifest.manifestIdentity);
});

test('Web Foundation preparation binds the sealed generic contract baseline without requesting Foundation change', () => {
  assert.deepEqual(preparation.foundationContractBaseline.applicationContract,{ identity:'CON-023', version:'1.1' });
  assert.deepEqual(preparation.foundationContractBaseline.genericApplicationDependency,{ identity:'CON-001', version:'1.0' });
  assert.deepEqual(preparation.foundationContractBaseline.integrationProfiles,['FDN-006@1.0','FDN-007@1.0']);
  assert.equal(preparation.foundationContractBaseline.foundationChangeRequired,false);
  assert.equal(manifest.foundationNeutrality.foundationChangeRequiredForWebFit,false);
  assert.equal(manifest.foundationNeutrality.webMustAdaptToSealedFoundation,true);
});

test('runtime-current truth is explicitly bind-at-operation and is not fabricated during preparation', () => {
  for (const [key,value] of Object.entries(preparation.admissionCandidate)) {
    if (key.endsWith('BindingMode')) assert.equal(value,'AUTHORITATIVE_AT_OPERATION');
  }
  assert.equal(preparation.runtimeRegistrationTemplate.registrationBindingMode,'AUTHORITATIVE_AT_OPERATION');
  assert.equal(preparation.runtimeRegistrationTemplate.runtimePortBindingMode,'AUTHORITATIVE_AT_OPERATION');
  assert.equal(preparation.preflightSemantics.runtimeCurrentValues,'BIND_AT_OPERATION');
  assert.equal(preparation.preflightSemantics.missingBindAtOperationValuesInvalidatePreparation,false);
  assert.equal(preparation.preflightSemantics.missingBindAtOperationValuesPreventActualOperation,true);
});

test('only the four credentialed Web provider FCRs require opaque credential references', () => {
  assert.deepEqual(
    preparation.bindAtOperation.credentialReferences.map(item=>item.fcr),
    ['FCR-0176','FCR-0177','FCR-0196','FCR-0197']
  );
  assert.equal(preparation.bindAtOperation.credentialReferences.every(item=>item.secretBytesAllowed===false),true);
  assert.deepEqual(
    preparation.bindAtOperation.publicProviderRoutesWithoutCredentialReference,
    ['FCR-0173','FCR-0174','FCR-0175','FCR-0198','FCR-0199','FCR-0200']
  );
});

test('preparation contains no activation deployment connectivity production business or trading authority', () => {
  for (const value of Object.values(preparation.runtimeRegistrationTemplate)) {
    if (typeof value === 'boolean') assert.equal(value,false);
  }
  for (const value of Object.values(preparation.mandatoryNoLinkState)) assert.equal(value,false);
  assert.equal(preparation.secretSafety.rawSecretBytesPresent,false);
  assert.equal(preparation.secretSafety.credentialReferencesAreOpaqueIdentifiersOnly,true);
});

test('composition evaluator declares full plug-ready only when every preparation invariant passes', () => {
  const result = evaluateWebFoundationPlugReadyPreparation(preparation,manifest);
  assert.equal(result.status,'FULL_PLUG_READY_PREFLIGHT_VERIFIED_BY_COMPOSITION');
  assert.equal(result.webPreparation,'READY');
  assert.equal(result.foundationGenericCapability,'READY');
  assert.equal(result.fullPlugReadyContractPreflight,'VERIFIED');
  assert.equal(result.fullPlugReadyPreflight,'VERIFIED_BY_COMPOSITION');
  assert.equal(result.runtimeCurrentValues,'BIND_AT_OPERATION');
  assert.deepEqual(result.failedChecks,[]);
  assert.equal(result.actualAdmissionExecuted,false);
  assert.equal(result.actualCanonicalRuntimeRegistrationExecuted,false);
});

test('composition evaluator fails closed if preparation tries to smuggle authority or secret-shaped material', () => {
  const bad = structuredClone(preparation);
  bad.runtimeRegistrationTemplate.activationRequested = true;
  bad.secret = 'forbidden';
  const result = evaluateWebFoundationPlugReadyPreparation(bad,manifest);
  assert.equal(result.status,'PLUG_READY_PREFLIGHT_BLOCKED');
  assert.ok(result.failedChecks.includes('noAuthority'));
  assert.ok(result.failedChecks.includes('secretSafety'));
});
