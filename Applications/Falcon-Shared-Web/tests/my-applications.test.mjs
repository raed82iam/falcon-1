import test from 'node:test';
import assert from 'node:assert/strict';
import { createMyApplicationsFeature } from '../src/features/my-applications/my-applications.js';

const dictionary = {
  myApps:'My Applications', tradingDesc:'Trading application', open:'Open', discover:'Discover', coming:'Coming Soon', riskDesc:'Risk application'
};
const t = key => dictionary[key] ?? key;
const publicShell = content => `<main>${content}</main>`;
const demoBadge = () => '<span>DEMO</span>';

function render(apps, language='en', options={}) {
  return createMyApplicationsFeature({
    t,
    language:()=>language,
    publicShell,
    demoBadge,
    apps,
    previewMode:options.previewMode === true,
    applicationAccess:options.applicationAccess ?? null
  }).myApplicationsPage();
}

test('renders entitled-facing FSATS entry without turning visibility into authority', () => {
  const html = render(
    [{id:'fsats',name:'FSATS Trading',kind:'trading'}],
    'en',
    {applicationAccess:{fsats:{entitled:true,current:true,businessAuthorityGranted:false}}}
  );
  assert.match(html,/My Applications/);
  assert.match(html,/data-nav="trader"/);
  assert.match(html,/data-nav="fsats"/);
  assert.match(html,/grants no trading or execution authority/);
});

test('visible FSATS card remains locked without authoritative entitlement', () => {
  const html = render([{id:'fsats',name:'FSATS Trading',kind:'trading'}]);
  assert.doesNotMatch(html,/data-nav="trader"/);
  assert.match(html,/disabled aria-disabled="true"/);
  assert.match(html,/data-nav="fsats"/);
});

test('future applications remain visibly unavailable', () => {
  const html = render([{id:'risk',name:'Risk Management',kind:'risk'}]);
  assert.match(html,/Coming Soon/);
  assert.doesNotMatch(html,/data-nav="risk"/);
});

test('escapes application display names', () => {
  const html = render([{id:'risk',name:'<script>alert(1)<\/script>',kind:'risk'}]);
  assert.doesNotMatch(html,/<script>/);
  assert.match(html,/&lt;script&gt;/);
});
