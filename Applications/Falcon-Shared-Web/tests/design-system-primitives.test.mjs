import test from 'node:test';
import assert from 'node:assert/strict';
import {
  StatusTone,
  statusBadge,
  notice,
  visuallyHidden,
  disabledControlAttributes
} from '../src/design-system/primitives.js';

test('status badge carries a non-color symbol and safely encodes untrusted copy', () => {
  const html = statusBadge({ label:'<Current>', tone:StatusTone.POSITIVE });
  assert.match(html,/ds-status--positive/);
  assert.match(html,/aria-hidden="true">✓/);
  assert.match(html,/&lt;Current&gt;/);
  assert.doesNotMatch(html,/<Current>/);
});

test('status badge can expose one explicit accessible label without duplicate visible text', () => {
  const html = statusBadge({ label:'Current', tone:StatusTone.POSITIVE, accessibleLabel:'System status: Current' });
  assert.match(html,/aria-label="System status: Current"/);
  assert.match(html,/<span aria-hidden="true">Current<\/span>/);
});

test('visually hidden text stays truly empty when no copy exists', () => {
  assert.equal(visuallyHidden(''),'');
  assert.equal(visuallyHidden(null),'');
});

test('notice limits live-region role to status or alert and encodes content', () => {
  assert.match(notice({ title:'Careful', body:'<unsafe>', tone:StatusTone.WARNING, role:'alert' }),/role="alert"/);
  assert.match(notice({ title:'Info', body:'x', role:'dialog' }),/role="status"/);
  assert.match(notice({ body:'<unsafe>' }),/&lt;unsafe&gt;/);
});

test('disabled controls expose native and aria disabled semantics with safely encoded reason', () => {
  const attrs = disabledControlAttributes(true,'Not "authorized"');
  assert.match(attrs,/ disabled aria-disabled="true"/);
  assert.match(attrs,/aria-description="Not &quot;authorized&quot;"/);
  assert.equal(disabledControlAttributes(false,'x'),'');
});
