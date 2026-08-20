import test from 'node:test';
import assert from 'node:assert/strict';
import { bindAppUi, __test } from '../src/composition/app-ui-bindings.js';

function emptyDocument() {
  return {
    documentElement:{ lang:'', dir:'' },
    querySelectorAll() { return []; },
    querySelector() { return null; }
  };
}

test('app UI binding composes browser events and delegates incident actions', () => {
  let incidentBound = 0;
  bindAppUi({
    documentRef:emptyDocument(),
    navigate() {},
    i18n:{ lang:'en', set() {} },
    render() {},
    auth:{ async signIn() { return null; } },
    setSession() {},
    store:{},
    incidentRuntime:{ bindActions() { incidentBound += 1; } }
  });

  assert.equal(incidentBound,1);
});

test('language accessibility sync updates document direction and localized skip-link copy', () => {
  const skip = { textContent:'' };
  const documentRef = {
    documentElement:{ lang:'', dir:'' },
    querySelector(selector) { return selector === '[data-skip-link]' ? skip : null; }
  };

  __test.syncLanguageAccessibility(documentRef,{ lang:'ar' });
  assert.equal(documentRef.documentElement.lang,'ar');
  assert.equal(documentRef.documentElement.dir,'rtl');
  assert.equal(skip.textContent,'تجاوز إلى المحتوى');

  __test.syncLanguageAccessibility(documentRef,{ lang:'en' });
  assert.equal(documentRef.documentElement.lang,'en');
  assert.equal(documentRef.documentElement.dir,'ltr');
  assert.equal(skip.textContent,'Skip to content');
});

test('language change remains renderable when preference persistence throws', () => {
  let lang = 'ar';
  const i18n = {
    get lang() { return lang; },
    set(next) { lang = next; throw new Error('storage blocked'); }
  };
  assert.equal(__test.setLanguageResilient(i18n,'en'),false);
  assert.equal(i18n.lang,'en');
});

test('app UI binding fails closed when required composition dependencies are absent', () => {
  const base = {
    documentRef:emptyDocument(),
    navigate() {},
    i18n:{ lang:'en', set() {} },
    render() {},
    auth:{ async signIn() { return null; } },
    setSession() {},
    store:{},
    incidentRuntime:{ bindActions() {} }
  };

  assert.throws(() => bindAppUi({ ...base, incidentRuntime:null }),/incidentRuntime\.bindActions is required/);
  assert.throws(() => bindAppUi({ ...base, auth:null }),/auth\.signIn is required/);
  assert.throws(() => bindAppUi({ ...base, navigate:null }),/navigate must be a function/);
});
