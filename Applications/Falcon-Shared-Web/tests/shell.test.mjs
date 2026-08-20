import test from 'node:test';
import assert from 'node:assert/strict';
import { createShell } from '../src/composition/shell.js';

const dictionary = new Proxy({}, { get: (_, key) => String(key) });
const t = key => dictionary[key];

function shell(lang = 'en') {
  return createShell({ t, language: () => lang, demoLabel: () => 'DEMO' });
}

test('public shell preserves product identity and language action', () => {
  const english = shell('en').publicShell('<section>x</section>');
  assert.match(english, /FALCON OS/);
  assert.match(english, /العربية/);
  assert.match(english, /data-nav="login"/);

  const arabicFsats = shell('ar').publicShell('<section>x</section>', 'fsats');
  assert.match(arabicFsats, /FSATS/);
  assert.match(arabicFsats, /English/);
});

test('workspace shell keeps owner and user navigation separated', () => {
  const user = shell().workspace('<p>user</p>', 'portfolio', false);
  assert.match(user, /data-nav="portfolio" class="active"/);
  assert.match(user, /data-nav="my-apps"/);
  assert.doesNotMatch(user, /owner-note/);
  assert.doesNotMatch(user, /data-nav="owner-ai-emergency"/);

  const owner = shell().workspace('<p>owner</p>', 'owner-incidents', true);
  assert.match(owner, /data-nav="owner-incidents" class="active"/);
  assert.match(owner, /data-nav="owner-apps"/);
  assert.match(owner, /data-nav="owner-ai-emergency"/);
  assert.match(owner, /AI Emergency/);
  assert.match(owner, /owner-note/);
});

test('Arabic Owner shell localizes the AI emergency navigation label', () => {
  const owner = shell('ar').workspace('<p>owner</p>', 'owner-ai-emergency', true);
  assert.match(owner, /data-nav="owner-ai-emergency" class="active"/);
  assert.match(owner, /طوارئ AI/);
});

test('shell requires explicit dependencies', () => {
  assert.throws(() => createShell(), /t must be a function/);
});
