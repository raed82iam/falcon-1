import test from 'node:test';
import assert from 'node:assert/strict';
import { sectionCard, formField, dataTable } from '../src/design-system/primitives.js';

test('section card preserves semantic heading and escapes hostile content',()=>{
  const html=sectionCard({title:'Status <script>alert(1)</script>',body:'Safe & clear',headingLevel:3});
  assert.match(html,/<h3>/u);
  assert.match(html,/&lt;script&gt;/u);
  assert.doesNotMatch(html,/<script>/u);
  assert.match(html,/Safe &amp; clear/u);
});

test('form field binds label description required and disabled semantics safely',()=>{
  const html=formField({
    id:'account-id',
    label:'Account <Owner>',
    value:'" onfocus="evil',
    description:'Required <truth>',
    required:true,
    disabled:true
  });
  assert.match(html,/label for="account-id"/u);
  assert.match(html,/aria-describedby="account-id-description"/u);
  assert.match(html,/required aria-required="true"/u);
  assert.match(html,/disabled aria-disabled="true"/u);
  assert.match(html,/&quot; onfocus=&quot;evil/u);
  assert.doesNotMatch(html,/Account <Owner>/u);
});

test('data table uses caption and named region only when an accessible name exists',()=>{
  const html=dataTable({caption:'Positions',columns:['Asset','State'],rows:[['AAPL','Current'],['<BTC>','Unknown']]});
  assert.match(html,/<caption>Positions<\/caption>/u);
  assert.match(html,/role="region" aria-label="Positions"/u);
  assert.equal((html.match(/scope="col"/gu)??[]).length,2);
  assert.match(html,/&lt;BTC&gt;/u);
  assert.doesNotMatch(html,/<BTC>/u);

  const unnamed=dataTable({columns:['Asset'],rows:[['AAPL']]});
  assert.doesNotMatch(unnamed,/role="region"/u);
  assert.doesNotMatch(unnamed,/aria-label=/u);
});

test('data table rejects ambiguous row shape',()=>{
  assert.throws(()=>dataTable({columns:['A','B'],rows:[['only-one']]}),/each row must match columns length/u);
  assert.throws(()=>dataTable({columns:[],rows:[]}),/columns must not be empty/u);
});
