import test from 'node:test';
import assert from 'node:assert/strict';
import { createActivityFeature } from '../src/features/activity/activity.js';

const t=key=>key;
const workspace=body=>body;

test('activity exposes envelope truth and does not coerce null values to zero',()=>{
  const feature=createActivityFeature({t,workspace,data:{
    activityEnvelope:{truthState:'CURRENT',freshnessState:'CURRENT',completeness:'COMPLETE',availabilityState:'AVAILABLE',asOfTime:'2026-08-18T15:00:00Z'},
    trades:[{instrument:{value:'AAPL'},side:'BUY',requestedQuantity:null,averageFillPrice:null,effectiveAt:'2026-08-18T15:00:00Z',state:'UNKNOWN_BROKER_OUTCOME',truthState:'CURRENT',freshnessState:'CURRENT'}]
  }});
  const html=feature.activityPage();
  assert.match(html,/CURRENT/);
  assert.match(html,/COMPLETE/);
  assert.match(html,/AVAILABLE/);
  assert.match(html,/UNKNOWN_BROKER_OUTCOME/);
  assert.doesNotMatch(html,/>0</);
  assert.match(html,/—/);
});

test('activity renders correction or supersession lineage without changing order state',()=>{
  const feature=createActivityFeature({t,workspace,data:{trades:[{
    instrument:{value:'BTCUSD'},side:'SELL',requestedQuantity:1,averageFillPrice:100,effectiveAt:'2026-08-18T15:00:00Z',state:'ACCEPTED',updateKind:'CORRECTION',correctsUpdateId:'upd-1'
  }]}});
  const html=feature.activityPage();
  assert.match(html,/CORRECTION/);
  assert.match(html,/corrects upd-1/);
  assert.match(html,/ACCEPTED/);
  assert.doesNotMatch(html,/FILLED/);
});
