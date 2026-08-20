import test from 'node:test';
import assert from 'node:assert/strict';
import { __test } from '../src/composition/app-ui-bindings.js';

function storeWith(order){
  let current=[...order];
  return {
    layout(){ return {order:[...current],hidden:[],sizes:{}}; },
    reorderWidget(source,target){
      const a=current.indexOf(source), b=current.indexOf(target);
      if(a<0||b<0||a===b) return;
      current.splice(a,1);
      current.splice(b,0,source);
    },
    snapshot(){ return [...current]; }
  };
}

test('keyboard move up/down changes only Web layout order',()=>{
  const store=storeWith(['a','b','c']);
  assert.equal(__test.moveWidgetByKeyboard(store,'b','up'),true);
  assert.deepEqual(store.snapshot(),['b','a','c']);
  assert.equal(__test.moveWidgetByKeyboard(store,'b','down'),true);
  assert.deepEqual(store.snapshot(),['a','b','c']);
});

test('keyboard move at boundaries is a no-op',()=>{
  const store=storeWith(['a','b']);
  assert.equal(__test.moveWidgetByKeyboard(store,'a','up'),false);
  assert.equal(__test.moveWidgetByKeyboard(store,'b','down'),false);
  assert.deepEqual(store.snapshot(),['a','b']);
});
