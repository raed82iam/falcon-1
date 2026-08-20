import test from 'node:test';
import assert from 'node:assert/strict';
import { createShell } from '../src/composition/shell.js';

const translations={
  home:'Home',applications:'Applications',signIn:'Sign in',ownerCenter:'Owner Center',systemOverview:'System Overview',incidents:'Incidents',notifications:'Notifications',
  noTradingOwner:'Owner system control only.',falconAI:'Falcon AI',quickSummary:'Quick summary',askFalcon:'Ask Falcon',dashboard:'Dashboard',markets:'Markets',
  portfolio:'Portfolio',activity:'Activity',settings:'Settings',users:'Users',audit:'Audit',approvals:'Approvals'
};
const t=key=>translations[key]??key;

function shell(language='en'){
  return createShell({t,language:()=>language,demoLabel:()=>''});
}

test('public shell exposes localized skip link and focusable main landmark',()=>{
  const html=shell('en').publicShell('<h1>Content</h1>');
  assert.match(html,/class="skip-link" href="#main">Skip to main content<\/a>/u);
  assert.match(html,/<main id="main" tabindex="-1">/u);
});

test('workspace shell exposes the same main landmark for user and Owner surfaces',()=>{
  for(const owner of [false,true]){
    const html=shell('en').workspace('<h1>Content</h1>',owner?'owner':'trader',owner);
    assert.match(html,/class="skip-link" href="#main">Skip to main content<\/a>/u);
    assert.equal((html.match(/id="main"/gu)??[]).length,1);
    assert.match(html,/<main id="main" tabindex="-1">/u);
  }
});

test('Arabic shell localizes skip-link text without changing target semantics',()=>{
  const html=shell('ar').publicShell('<h1>المحتوى</h1>');
  assert.match(html,/تخطي إلى المحتوى الرئيسي/u);
  assert.match(html,/href="#main"/u);
});
