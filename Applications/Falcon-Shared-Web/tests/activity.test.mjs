import test from 'node:test';
import assert from 'node:assert/strict';
import { createActivityFeature } from '../src/features/activity/activity.js';

const labels={
  activity:'Activity',active:'Active',trades:'Trades',history:'History',orderTruthNote:'ORDER REQUESTED ≠ ACCEPTED ≠ PARTIALLY FILLED ≠ FILLED',
  orderSide_BUY:'BUY',orderSide_SELL:'SELL',orderState_PARTIALLY_FILLED:'PARTIALLY FILLED',orderState_UNKNOWN_BROKER_OUTCOME:'UNKNOWN BROKER OUTCOME'
};
const t = key => labels[key] ?? key;
const workspace = (body, active) => `<main data-active="${active}">${body}</main>`;
const data = {
  trades: [
    ['AAPL', 'BUY', '10', '$190', '10:01','PARTIALLY_FILLED'],
    ['BTC', 'SELL', '0.1', '$60,000', '10:02','UNKNOWN_BROKER_OUTCOME']
  ]
};

test('activity feature renders supplied activity projection and preserves lifecycle distinctions', () => {
  const { activityPage } = createActivityFeature({ t, workspace, data });
  const html = activityPage();

  assert.match(html, /data-active="activity"/);
  assert.match(html, /AAPL/);
  assert.match(html, /class="positive">BUY/);
  assert.match(html, /class="negative">SELL/);
  assert.match(html, /PARTIALLY FILLED/);
  assert.match(html, /UNKNOWN BROKER OUTCOME/);
  assert.doesNotMatch(html, /<span class="status-chip">FILLED<\/span>/);
});

test('activity feature fails closed without an activity projection', () => {
  assert.throws(() => createActivityFeature({ t, workspace, data: {} }), /data\.trades is required/);
});

test('activity feature validates presentation dependencies', () => {
  assert.throws(() => createActivityFeature({ workspace, data }), /t must be a function/);
  assert.throws(() => createActivityFeature({ t, data }), /workspace must be a function/);
});
