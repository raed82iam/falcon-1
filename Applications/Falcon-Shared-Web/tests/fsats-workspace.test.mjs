import test from 'node:test';
import assert from 'node:assert/strict';
import { createFsatsWorkspaceFeature } from '../src/features/fsats-workspace/fsats-workspace.js';

const labels = {
  hide:'Hide', chart:'Chart', totalValue:'Portfolio Value', todayPL:'Today P/L', totalPL:'Total P/L', positions:'Positions', recentTrades:'Recent Trades', quickSummary:'Quick Summary', askFalcon:'Ask Falcon', notifications:'Notifications', dashboard:'Dashboard', manageWidgets:'Manage Widgets', resetLayout:'Reset Layout', restore:'Restore', schools:'Schools', strategies:'Strategies', orderSide_BUY:'Buy', orderState_UNKNOWN_BROKER_OUTCOME:'Unknown broker outcome'
};
const t = key => labels[key] ?? key;
const workspace = (content, active) => `<main data-active="${active}">${content}</main>`;
const data = {
  portfolio:{value:'$10,000',today:'+$100',total:'+$900'},
  positions:[['AAPL','10','190','+$50']],
  trades:[['AAPL','BUY','10','190','10:00']],
  alerts:['Projection updated']
};

function render({ language='en', hidden=[], previewMode=true }={}) {
  const store = { layout:()=>({order:['market','catalog','portfolio','daily','performance','positions','trades','summary','alerts'],hidden,sizes:{}}) };
  return createFsatsWorkspaceFeature({ t, language:()=>language, workspace, store, data, catalogMarkup:()=>'<div>Catalog projection</div>', previewMode }).dashboardPage();
}

test('renders dashboard through FSATS workspace feature boundary', () => {
  const html = render();
  assert.match(html,/data-active="trader"/);
  assert.match(html,/Portfolio Value/);
  assert.match(html,/Catalog projection/);
  assert.match(html,/Preview data is explicitly marked/);
});

test('authoritative composition does not display preview notice', () => {
  assert.doesNotMatch(render({previewMode:false}),/Preview data is explicitly marked/);
});

test('respects Web-owned hidden widget preference', () => {
  const html = render({hidden:['market']});
  assert.doesNotMatch(html,/Development preview chart/);
  assert.match(html,/Restore: market/);
});

test('Arabic preview text remains explicit about non-live demo truth', () => {
  const html = render({language:'ar'});
  assert.match(html,/لا تمثل حقيقة مالية حية/);
});
