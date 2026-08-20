import test from 'node:test';
import assert from 'node:assert/strict';
import { readFile } from 'node:fs/promises';

const indexUrl = new URL('../index.html',import.meta.url);
const designCssUrl = new URL('../src/design-system.css',import.meta.url);
const accessibilityCssUrl = new URL('../src/accessibility.css',import.meta.url);

test('document loads semantic design system before feature-specific styles and exposes localizable skip link', async () => {
  const html = await readFile(indexUrl,'utf8');
  assert.match(html,/href="\.\/src\/styles\.css"[\s\S]*href="\.\/src\/design-system\.css"/);
  assert.match(html,/data-skip-link/);
  assert.match(html,/href="#main"/);
});

test('design system gives status semantics a non-color cue and supports forced-colors mode', async () => {
  const css = await readFile(designCssUrl,'utf8');
  assert.match(css,/\.ds-status__symbol/);
  assert.match(css,/\.ds-status--unavailable/);
  assert.match(css,/border-style:dashed/);
  assert.match(css,/@media \(forced-colors:active\)/);
});

test('design system exposes reusable semantic surface/content/status tokens', async () => {
  const css = await readFile(designCssUrl,'utf8');
  for (const token of [
    '--surface-canvas',
    '--surface-panel',
    '--content-primary',
    '--content-secondary',
    '--status-positive',
    '--status-warning',
    '--status-negative',
    '--status-unavailable'
  ]) {
    assert.match(css,new RegExp(token.replace('--','--')));
  }
});

test('keyboard focus and reduced-motion rules remain loaded alongside design primitives', async () => {
  const css = await readFile(accessibilityCssUrl,'utf8');
  assert.match(css,/:focus-visible/);
  assert.match(css,/prefers-reduced-motion: reduce/);
});
