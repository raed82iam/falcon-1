import test from 'node:test';
import assert from 'node:assert/strict';
import { createFsatsPublicFeature } from '../src/features/fsats-public/fsats-public.js';

const copy={
  fsatsHero:'FSATS',fsatsText:'Trading system',start:'Start',falconAI:'Falcon AI',risk:'Risk',markets:'Markets',
  signIn:'Sign in',welcome:'Welcome',email:'Email',password:'Password',remember:'Remember',forgot:'Forgot',createAccount:'Create account'
};

function render(fsatsApps){
  const {fsatsLanding}=createFsatsPublicFeature({
    t:key=>copy[key]??key,
    language:()=> 'en',
    publicShell:content=>content,
    icon:()=>'',
    fsatsApps
  });
  return fsatsLanding();
}

test('FSATS public consumes the supplied shared preview catalog',()=>{
  const html=render([
    {id:'fsata',shortName:'FSATA-VNEXT',name:'Current Composed Trading App'},
    {id:'custom',shortName:'CUSTOM',name:'Composed Future Test App'}
  ]);
  assert.match(html,/FSATA-VNEXT/u);
  assert.match(html,/Current Composed Trading App/u);
  assert.match(html,/CUSTOM/u);
  assert.match(html,/Composed Future Test App/u);
  assert.doesNotMatch(html,/Falcon Self-Aware Provider Management Application/u);
});

test('supplied catalog names are escaped before public rendering',()=>{
  const html=render([{id:'custom',shortName:'<svg onload=evil>',name:'<script>alert(1)</script>'}]);
  assert.match(html,/&lt;svg onload=evil&gt;/u);
  assert.match(html,/&lt;script&gt;alert\(1\)&lt;\/script&gt;/u);
  assert.doesNotMatch(html,/<script>/u);
  assert.doesNotMatch(html,/<svg onload=/u);
});

test('invalid supplied catalog shape fails closed',()=>{
  assert.throws(()=>createFsatsPublicFeature({
    t:key=>key,
    language:()=> 'en',
    publicShell:content=>content,
    icon:()=>'',
    fsatsApps:{not:'an-array'}
  }),/fsatsApps must be an array/u);
});
