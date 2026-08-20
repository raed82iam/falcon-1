import test from 'node:test';
import assert from 'node:assert/strict';
import { presentTradingOverlay, applyOverlayUpdate } from '../src/presenters/trading-overlay.js';

test('non-applicable overlay is not rendered',()=>{
  const view=presentTradingOverlay({overlayProjectionId:'ov1',applicabilityState:'NOT_APPLICABLE',truthState:'CURRENT',elements:[{id:'x',type:'POINT'}],reasonCode:'NOT_APPLICABLE'});
  assert.equal(view.renderable,false);
  assert.deepEqual(view.elements,[]);
});

test('applicable overlay renders only supplied provider-neutral primitives',()=>{
  const view=presentTradingOverlay({overlayProjectionId:'ov1',overlaySubjectKind:'STRATEGY',overlaySubjectId:'s1',resolvedInstrumentIdentity:'AAPL',timeframe:'1m',applicabilityState:'APPLICABLE',truthState:'CURRENT',elements:[{id:'e1',type:'PRICE_LEVEL',price:200,label:'Target'}]});
  assert.equal(view.renderable,true);
  assert.equal(view.elements[0].type,'PRICE_LEVEL');
  assert.equal(view.elements[0].price,200);
});

test('unsupported overlay primitive fails closed instead of recreating strategy logic',()=>{
  assert.throws(()=>presentTradingOverlay({overlayProjectionId:'ov1',applicabilityState:'APPLICABLE',truthState:'CURRENT',elements:[{id:'e1',type:'MAGIC_STRATEGY'}]}),/Unsupported overlay element type/);
});

test('overlay update requires exact projection identity',()=>{
  const current={projectionId:'ov1'};
  assert.equal(applyOverlayUpdate(current,{overlayProjectionId:'ov2',updateType:'REMOVE'}).accepted,false);
  assert.equal(applyOverlayUpdate(current,{overlayProjectionId:'ov1',updateType:'REMOVE'}).accepted,true);
});
