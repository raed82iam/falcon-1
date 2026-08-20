import test from 'node:test';
import assert from 'node:assert/strict';
import { store, __test } from '../src/state.js';

function withLocalStorage(value, fn) {
  const descriptor = Object.getOwnPropertyDescriptor(globalThis,'localStorage');
  Object.defineProperty(globalThis,'localStorage',{ configurable:true, value });
  try { return fn(); }
  finally {
    if (descriptor) Object.defineProperty(globalThis,'localStorage',descriptor);
    else delete globalThis.localStorage;
  }
}

test('language falls back to Arabic when browser storage is unavailable',()=>{
  withLocalStorage({ getItem(){ throw new Error('blocked'); } },()=>{
    assert.equal(store.language,'ar');
  });
});

test('layout falls back to defaults when storage read is unavailable',()=>{
  withLocalStorage({ getItem(){ throw new Error('blocked'); } },()=>{
    const layout=store.layout();
    assert.deepEqual(layout.hidden,[]);
    assert.ok(layout.order.includes('portfolio'));
    assert.ok(layout.order.includes('alerts'));
  });
});

test('layout save/remove failures degrade without throwing',()=>{
  withLocalStorage({
    setItem(){ throw new Error('blocked'); },
    removeItem(){ throw new Error('blocked'); }
  },()=>{
    assert.doesNotThrow(()=>store.saveLayout({hidden:[],sizes:{},order:[]}));
    assert.doesNotThrow(()=>store.resetLayout());
    assert.equal(__test.storageSet('x','y'),false);
    assert.equal(__test.storageRemove('x'),false);
  });
});

test('malformed stored layout shapes are normalized instead of trusted',()=>{
  withLocalStorage({
    getItem(key){
      if (key==='falcon.dashboard.layout') return JSON.stringify({hidden:'bad',sizes:[],order:'bad'});
      return null;
    }
  },()=>{
    const layout=store.layout();
    assert.deepEqual(layout.hidden,[]);
    assert.deepEqual(layout.sizes,{});
    assert.ok(Array.isArray(layout.order));
    assert.ok(layout.order.includes('portfolio'));
  });
});
