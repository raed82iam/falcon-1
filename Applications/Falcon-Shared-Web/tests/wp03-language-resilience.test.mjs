import test from 'node:test';
import assert from 'node:assert/strict';
import { __test } from '../src/composition/app-ui-bindings.js';

const { setLanguageResilient, syncLanguageAccessibility } = __test;

test('language change remains usable when persistence throws',()=>{
  const i18n={
    lang:'ar',
    set(next){ this.lang=next; throw new Error('storage blocked'); }
  };
  const persisted=setLanguageResilient(i18n,'en');
  assert.equal(persisted,false);
  assert.equal(i18n.lang,'en');
});

test('document language and direction follow current language independently of storage',()=>{
  const documentRef={
    documentElement:{lang:'ar',dir:'rtl'},
    querySelector(){ return null; }
  };
  syncLanguageAccessibility(documentRef,{lang:'en'});
  assert.equal(documentRef.documentElement.lang,'en');
  assert.equal(documentRef.documentElement.dir,'ltr');
  syncLanguageAccessibility(documentRef,{lang:'ar'});
  assert.equal(documentRef.documentElement.lang,'ar');
  assert.equal(documentRef.documentElement.dir,'rtl');
});
