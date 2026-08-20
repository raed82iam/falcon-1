import test from 'node:test';
import assert from 'node:assert/strict';
import { createMyApplicationsFeature } from '../src/features/my-applications/my-applications.js';
import { renderSubscriptionPresentation } from '../src/features/my-applications/subscription-presentation.js';

const t=key=>({myApps:'My Applications',tradingDesc:'Trading',open:'Open',discover:'Discover',coming:'Coming'}[key] ?? key);
const shell=body=>body;
const apps=[{id:'fsats',name:'FSATS',kind:'trading'}];

test('authoritative My Applications locks FSATS without entitlement',()=>{
  const {myApplicationsPage}=createMyApplicationsFeature({t,language:()=> 'en',publicShell:shell,demoBadge:()=>'',apps,previewMode:false,applicationAccess:null});
  const html=myApplicationsPage();
  assert.match(html,/disabled aria-disabled="true">Unavailable/);
  assert.doesNotMatch(html,/data-nav="trader">Open/);
});

test('preview navigation is labeled Preview instead of entitlement',()=>{
  const {myApplicationsPage}=createMyApplicationsFeature({t,language:()=> 'en',publicShell:shell,demoBadge:()=>'',apps,previewMode:true});
  const html=myApplicationsPage();
  assert.match(html,/data-nav="trader">Preview/);
  assert.match(html,/Preview only; this is not entitlement or live activation/);
});

test('current non-authority entitlement permits Application navigation',()=>{
  const {myApplicationsPage}=createMyApplicationsFeature({
    t,language:()=> 'en',publicShell:shell,demoBadge:()=>'',apps,previewMode:false,
    applicationAccess:{fsats:{entitled:true,current:true,businessAuthorityGranted:false}}
  });
  assert.match(myApplicationsPage(),/data-nav="trader">Open/);
});

test('subscription tiers do not invent pricing or trial without contract',()=>{
  const html=renderSubscriptionPresentation({language:'en',contractAvailable:false});
  assert.match(html,/Contract unavailable/);
  assert.match(html,/Trial or upgrade state is not inferred/);
  assert.doesNotMatch(html,/\$|SAR|USD/);
});

test('tier access requires each tier truth to be authoritative',()=>{
  const html=renderSubscriptionPresentation({language:'en',contractAvailable:true,tiers:[
    {id:'STANDARD',entitled:true,current:true,authoritative:false,priceText:'100'},
    {id:'VIP',entitled:true,current:true,authoritative:true,priceText:'200'}
  ]});
  assert.match(html,/Standard<\/h3><p class="muted">Price unavailable/);
  assert.match(html,/VIP<\/h3><p>200<\/p><span class="status-chip">Available/);
});
