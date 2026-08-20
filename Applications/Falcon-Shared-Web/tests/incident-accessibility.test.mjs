import test from 'node:test';
import assert from 'node:assert/strict';
import { synchronizeIncidentAccessibility } from '../src/incidents/incident-accessibility.js';

function element(){
  const attrs=new Map();
  return {
    id:'',
    disabled:false,
    focusCalled:false,
    setAttribute(name,value){attrs.set(name,String(value));},
    getAttribute(name){return attrs.get(name) ?? null;},
    focus(){this.focusCalled=true;}
  };
}

function documentFor({lang='en',dialog=null,minimized=null,reply=null,disabled=[]}={}){
  const body={};
  const documentElement={lang};
  const security=element();
  const doc={
    body,
    documentElement,
    activeElement:body,
    querySelector(selector){
      if(selector==='.customer-incident-modal[role="dialog"]') return dialog;
      if(selector==='[data-incident-expand]') return minimized;
      if(selector==='[data-incident-text]') return reply;
      if(selector==='.incident-security-note') return security;
      return null;
    },
    querySelectorAll(selector){return selector==='.customer-incident-modal button:disabled'?disabled:[];}
  };
  if(dialog){dialog.focus=()=>{dialog.focusCalled=true;doc.activeElement=dialog;};}
  if(minimized){minimized.focus=()=>{minimized.focusCalled=true;doc.activeElement=minimized;};}
  return {doc,security};
}

test('open incident dialog gets programmatic focus target and accessible reply label',()=>{
  const dialog=element();
  const reply=element();
  const disabled=[element(),element()];
  const {doc,security}=documentFor({dialog,reply,disabled,lang:'en'});
  const result=synchronizeIncidentAccessibility(doc);
  assert.equal(result.state,'DIALOG');
  assert.equal(result.focused,true);
  assert.equal(dialog.getAttribute('tabindex'),'-1');
  assert.equal(dialog.getAttribute('aria-describedby'),'incident-security-note');
  assert.equal(security.id,'incident-security-note');
  assert.equal(reply.getAttribute('aria-label'),'Type your incident reply');
  for(const control of disabled) assert.equal(control.getAttribute('aria-disabled'),'true');
});

test('Arabic incident reply receives Arabic accessible label',()=>{
  const dialog=element();
  const reply=element();
  const {doc}=documentFor({dialog,reply,lang:'ar'});
  synchronizeIncidentAccessibility(doc);
  assert.equal(reply.getAttribute('aria-label'),'اكتب ردك للحادثة');
});

test('minimized incident edge receives focus after modal is removed',()=>{
  const minimized=element();
  const {doc}=documentFor({minimized});
  const result=synchronizeIncidentAccessibility(doc);
  assert.equal(result.state,'MINIMIZED');
  assert.equal(result.focused,true);
  assert.equal(minimized.getAttribute('aria-expanded'),'false');
});

test('incident synchronization does not steal focus from another active control',()=>{
  const dialog=element();
  const reply=element();
  const {doc}=documentFor({dialog,reply});
  const other=element();
  doc.activeElement=other;
  const result=synchronizeIncidentAccessibility(doc);
  assert.equal(result.state,'DIALOG');
  assert.equal(result.focused,false);
  assert.equal(dialog.focusCalled,false);
});
