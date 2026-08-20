import test from 'node:test';
import assert from 'node:assert/strict';
import { readFile } from 'node:fs/promises';
import { createFalconPublicFeature } from '../src/features/falcon-public/falcon-public.js';

const translations={tradingDesc:'Trading',riskDesc:'Risk',aiDesc:'AI',discover:'Discover',coming:'Coming soon',appsTitle:'Apps',signIn:'Sign in'};
const t=key=>translations[key] ?? key;
const shell=body=>body;
const apps=[
  {id:'fsats',name:'FSATS',kind:'trading',shortName:'FSATS'},
  {id:'future-risk',name:'Future Risk',kind:'risk',shortName:'Risk'}
];
const fsatsApps=[{id:'fsata',name:'Falcon Self-Aware Trading Application',kind:'trading',shortName:'FSATA'}];

test('Arabic public hero does not claim ungoverned free pricing',()=>{
  const html=createFalconPublicFeature({t,language:()=> 'ar',publicShell:shell,apps,fsatsApps}).publicHome();
  assert.doesNotMatch(html,/مجانًا/);
  assert.match(html,/ابدأ الآن/);
  assert.match(html,/لا تعني تشغيلًا حيًا أو صلاحية تداول/);
});

test('future top-level application remains disabled and exposes aria-disabled',()=>{
  const html=createFalconPublicFeature({t,language:()=> 'en',publicShell:shell,apps,fsatsApps}).applicationsPage();
  assert.match(html,/Future Risk/);
  assert.match(html,/disabled aria-disabled="true"/);
  assert.doesNotMatch(html,/data-nav="future-risk"/);
});

test('public metadata describes product without Live or investment-performance claims',async()=>{
  const index=await readFile(new URL('../index.html',import.meta.url),'utf8');
  assert.match(index,/name="description"/);
  assert.match(index,/without implying live operation or trading authority/i);
  assert.doesNotMatch(index,/guaranteed profit|guaranteed return|get rich|risk-free/i);
});
