import test from 'node:test';
import assert from 'node:assert/strict';
import { createFalconPublicFeature } from '../src/features/falcon-public/falcon-public.js';
import { assertNoRegulatoryClaims } from '../src/contracts.js';

const fsats={id:'fsats',name:'Falcon Self-Aware Trading System (FSATS)',kind:'trading',shortName:'FSATS'};
const children=[
  {id:'fsata',name:'Falcon Self-Aware Trading Application',shortName:'FSATA'},
  {id:'fsapma',name:'Falcon Self-Aware Provider Management Application',shortName:'FSAPMA'},
  {id:'ftga',name:'Falcon Trading Guardian Application',shortName:'FTGA'},
  {id:'fstsim',name:'Falcon Self-Aware Trading Simulation Application',shortName:'FSTSimA'},
  {id:'app-rsc',name:'Falcon Self-Aware Resource Management Application',shortName:'APP-RSC'}
];

const copy={
  discover:'Discover',coming:'Coming soon',createAccount:'Create account',signIn:'Sign in',
  tradingDesc:'Current Trading system',appsTitle:'Falcon Applications'
};

function render(language='en'){
  const {publicHome,applicationsPage}=createFalconPublicFeature({
    t:key=>copy[key]??key,
    language:()=>language,
    publicShell:content=>content,
    apps:[fsats],
    fsatsApps:children
  });
  return {home:publicHome(),apps:applicationsPage()};
}

function futureCards(html){
  return html.split('<article class="app-card future-system-card"').slice(1).map(part=>part.split('</article>')[0]);
}

test('public home preserves Falcon OS above FSATS and its five internal Applications',()=>{
  const {home}=render('en');
  assert.match(home,/FALCON OS → FSATS/u);
  for(const child of children) assert.match(home,new RegExp(child.shortName,'u'));
  assert.match(home,/FSATS Trading System Applications/u);
});

test('future system families are explicit non-operational presentation only',()=>{
  const {home,apps}=render('en');
  for(const html of [home,apps]){
    assert.match(html,/Future Accounting System/u);
    assert.match(html,/Future Warehouse System/u);
    assert.match(html,/Other Future Falcon Systems/u);
    const cards=futureCards(html);
    assert.equal(cards.length,3);
    for(const card of cards){
      assert.match(card,/Future • Not operational/u);
      assert.match(card,/disabled aria-disabled="true"/u);
      assert.doesNotMatch(card,/data-nav=/u);
    }
  }
});

test('public Falcon separates Sign In and direct Create Account entry points',()=>{
  const {home}=render('en');
  assert.match(home,/>Sign in 🔒<\/button>/u);
  assert.match(home,/>Create account<\/button>/u);
  assert.equal((home.match(/data-nav="login"/gu)??[]).length,1);
  assert.equal((home.match(/data-nav="register"/gu)??[]).length,1);
});

test('Arabic future presentation remains explicit and non-operational',()=>{
  const {home}=render('ar');
  assert.match(home,/نظام المحاسبة المستقبلي/u);
  assert.match(home,/نظام المستودعات المستقبلي/u);
  assert.match(home,/مستقبلي • غير تشغيلي/u);
});

test('public markup contains no unapproved regulatory or licensing claim',()=>{
  for(const language of ['en','ar']){
    const {home,apps}=render(language);
    assert.equal(assertNoRegulatoryClaims(home),true);
    assert.equal(assertNoRegulatoryClaims(apps),true);
  }
});
