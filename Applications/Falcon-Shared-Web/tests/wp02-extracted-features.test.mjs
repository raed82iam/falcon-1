import test from 'node:test';
import assert from 'node:assert/strict';
import { createSettingsFeature } from '../src/features/settings/settings.js';
import { createCatalogPresentation } from '../src/features/catalog/catalog-presentation.js';

const workspace=(markup)=>markup;
const t=key=>({settings:'Settings',language:'Language',schools:'Schools',strategies:'Strategies'}[key]??key);

test('settings feature renders language state without creating business authority',()=>{
  const {settingsPage}=createSettingsFeature({
    t,
    language:()=> 'en',
    workspace,
    localize:(ar,en)=>en
  });
  const html=settingsPage();
  assert.match(html,/data-language-select/u);
  assert.match(html,/value="en" selected/u);
  assert.match(html,/RTL\/LTR applies/u);
});

test('catalog presentation safely renders supplied presentation model only',()=>{
  const {catalogMarkup}=createCatalogPresentation({
    t,
    language:()=> 'en',
    catalog:[
      {id:'s1',name:'Safe <School>',kind:'SCHOOL',availability:'AVAILABLE',applicability:'APPLICABLE'},
      {id:'s2',name:'Disabled Strategy',kind:'STRATEGY',availability:'AVAILABLE',applicability:'NOT_APPLICABLE',reason:'Not applicable'}
    ]
  });
  const html=catalogMarkup();
  assert.match(html,/Safe &lt;School&gt;/u);
  assert.doesNotMatch(html,/Safe <School>/u);
  assert.match(html,/Disabled Strategy/u);
  assert.match(html,/disabled/u);
  assert.match(html,/Not applicable/u);
});

test('extracted features validate required dependencies',()=>{
  assert.throws(()=>createSettingsFeature({}),/t must be a function/u);
  assert.throws(()=>createCatalogPresentation({t,language:()=> 'en',catalog:null}),/catalog must be an array/u);
});
