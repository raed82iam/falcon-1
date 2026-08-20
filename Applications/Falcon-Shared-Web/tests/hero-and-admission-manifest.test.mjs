import test from 'node:test';
import assert from 'node:assert/strict';
import { readFile } from 'node:fs/promises';
import { createFalconPublicFeature } from '../src/features/falcon-public/falcon-public.js';

const shell = body => `<main class="public-landing">${body}</main>`;
const t = key => key;
const apps = [{ id:'fsats', name:'FSATS', kind:'trading', shortName:'FSATS' }];
const fsatsApps = [{ id:'fsata', name:'FSATA', kind:'trading', shortName:'FSATA' }];

test('public hero keeps falcon image inside a dedicated circular mask and feature points outside it',()=>{
  const html=createFalconPublicFeature({ t, language:()=> 'en', publicShell:shell, apps, fsatsApps }).publicHome();
  assert.match(html,/<div class="falcon-image-mask"><img[^>]+falcon-shared-visual\.jpg[^>]*><\/div>\s*<div class="hero-side-features"/);
  assert.doesNotMatch(html,/falcon-shared-visual\.jpg[^>]+style=/);
  assert.match(html,/One system/);
  assert.match(html,/Disciplined intelligence/);
  assert.match(html,/Governance &amp; security|Governance & security/);
});

test('hero CSS clips only the image, fills the whole circular mask, mirrors Arabic image, and keeps Arabic points physically left of the circle',async()=>{
  const css=await readFile(new URL('../src/public-landing.css',import.meta.url),'utf8');
  assert.match(css,/\.falcon-image-mask\{[^}]*border-radius:50%[^}]*overflow:hidden/);
  assert.match(css,/\.falcon-image-mask img\{[^}]*inset:0[^}]*width:100%[^}]*height:100%[^}]*object-fit:cover/);
  assert.doesNotMatch(css,/\.falcon-image-mask img\{[^}]*(?:inset:[^;]*(?:3%|4%|8%)|width:89%|height:92%|object-fit:contain)/);
  assert.match(css,/\.falcon-image-mask img\{[^}]*transform:none/);
  assert.match(css,/:dir\(rtl\) \.falcon-image-mask img\{transform:scaleX\(-1\)\}/);
  assert.match(css,/\.falcon-image-mask\{grid-column:1/);
  assert.match(css,/\.hero-side-features\{grid-column:2/);
  assert.match(css,/:dir\(rtl\) \.falcon-visual\{direction:ltr;grid-template-columns:minmax\(170px,220px\) minmax\(280px,390px\)\}/);
  assert.match(css,/:dir\(rtl\) \.falcon-image-mask\{grid-column:2\}/);
  assert.match(css,/:dir\(rtl\) \.hero-side-features\{grid-column:1;grid-row:1;direction:rtl;text-align:right\}/);
});

test('narrow mobile hero contains text pills actions note and visual inside the viewport',async()=>{
  const css=await readFile(new URL('../src/public-landing.css',import.meta.url),'utf8');
  assert.match(css,/@media\(max-width:560px\)[\s\S]*?\.public-landing \.hero\{[^}]*padding:[^}]*min-width:0[^}]*overflow:hidden/);
  assert.match(css,/@media\(max-width:560px\)[\s\S]*?\.public-landing \.hero-copy\{[^}]*width:100%[^}]*max-width:100%[^}]*min-width:0/);
  assert.match(css,/@media\(max-width:560px\)[\s\S]*?\.hero-feature-pills\{[^}]*display:grid[^}]*grid-template-columns:repeat\(2,minmax\(0,1fr\)\)[^}]*width:100%/);
  assert.match(css,/@media\(max-width:560px\)[\s\S]*?\.hero-feature-pills span\{[^}]*min-width:0[^}]*width:100%/);
  assert.match(css,/@media\(max-width:560px\)[\s\S]*?\.public-landing \.hero-actions\{[^}]*display:grid[^}]*grid-template-columns:1fr[^}]*width:100%/);
  assert.match(css,/@media\(max-width:560px\)[\s\S]*?\.public-landing \.hero-actions \.primary,[^}]*\.public-landing \.hero-actions \.secondary\{[^}]*min-width:0[^}]*width:100%/);
  assert.match(css,/@media\(max-width:560px\)[\s\S]*?\.falcon-visual\{[^}]*width:100%[^}]*min-width:0/);
});

test('Shared Web admission manifest declares bounded application admission without runtime authority',async()=>{
  const text=await readFile(new URL('../governance/SHARED_WEB_APPLICATION_ADMISSION_MANIFEST_V1.json',import.meta.url),'utf8');
  const manifest=JSON.parse(text);
  assert.equal(manifest.subjectKind,'APPLICATION');
  assert.equal(manifest.application.identity,'FALCON_SHARED_WEB');
  assert.equal(manifest.securityAndIsolation.failClosed,true);
  assert.equal(manifest.securityAndIsolation.ordinaryWebStateMayContainSecretBytes,false);
  assert.deepEqual(manifest.authoritiesRequested,[]);
  assert.equal(manifest.declarationSemantics.declarationIsAuthorization,false);
  assert.equal(manifest.declarationSemantics.admissionIsActivation,false);
  assert.equal(manifest.declarationSemantics.manifestCreatesDeploymentAuthority,false);
  assert.equal(manifest.declarationSemantics.manifestCreatesProductionAuthority,false);
  assert.equal(manifest.declarationSemantics.manifestCreatesBusinessAuthority,false);
  assert.equal(manifest.foundationNeutrality.foundationChangeRequiredForWebFit,false);
  assert.equal(manifest.awareness.msa.identity,'SHARED_WEB_MSA');
  assert.equal(manifest.awareness.msa.count,1);
  assert.equal(manifest.providerExternalAccess.credentialReferenceMayContainSecretBytes,false);
  assert.ok(manifest.authorityInvariants.includes('WEB_PRESENTATION_DATA != FSATS_OPERATIONAL_DATA'));
  assert.ok(manifest.authorityInvariants.includes('CREDENTIAL_REFERENCE_ID != SECRET_BYTES'));
  assert.ok(manifest.authorityInvariants.includes('REGISTERED != ACTIVATED'));
});
