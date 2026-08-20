import test from 'node:test';
import assert from 'node:assert/strict';
import { demoBadge, displayText, icon, toneClass } from '../src/design-system/presentation.js';

test('presentation text escapes markup instead of injecting it', () => {
  assert.equal(displayText('<script>alert(1)</script>'), '&lt;script&gt;alert(1)&lt;/script&gt;');
  assert.equal(demoBadge('<b>DEMO</b>'), '<div class="demo-badge">&lt;b&gt;DEMO&lt;/b&gt;</div>');
});

test('presentation fallback does not invent source values', () => {
  assert.equal(displayText(null), '—');
  assert.equal(displayText(undefined, 'UNAVAILABLE'), 'UNAVAILABLE');
  assert.equal(displayText(0), '0');
});

test('icon output stays decorative and tone classification is deterministic', () => {
  assert.match(icon('warning'), /aria-hidden="true"/);
  assert.equal(toneClass('-1.00'), 'negative');
  assert.equal(toneClass('+1.00'), 'positive');
});
