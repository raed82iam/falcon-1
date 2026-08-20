import test from 'node:test';
import assert from 'node:assert/strict';
import { readdir, readFile } from 'node:fs/promises';
import { extname } from 'node:path';

const srcRoot = new URL('../src/', import.meta.url);

async function filesUnder(url) {
  const entries = await readdir(url, { withFileTypes: true });
  const result = [];
  for (const entry of entries) {
    const child = new URL(entry.name + (entry.isDirectory() ? '/' : ''), url);
    if (entry.isDirectory()) result.push(...await filesUnder(child));
    else if (['.js', '.mjs'].includes(extname(entry.name))) result.push(child);
  }
  return result;
}

test('Web source does not import Foundation or ordinary Application internals', async () => {
  for (const file of await filesUnder(srcRoot)) {
    const source = await readFile(file, 'utf8');
    const imports = [...source.matchAll(/(?:from\s+|import\s*)['"]([^'"]+)['"]/g)].map(match => match[1]);

    for (const specifier of imports) {
      assert.equal(
        /(?:^|\/)foundation(?:\/|$)|applications\/FSATS|applications\/docs\/FSATS/i.test(specifier),
        false,
        `${file.pathname} directly imports another workstream: ${specifier}`
      );
    }
  }
});

test('network transport primitives stay out of presentation modules', async () => {
  const allowed = new Set(['adapters.js']);
  for (const file of await filesUnder(srcRoot)) {
    const name = file.pathname.split('/').pop();
    if (allowed.has(name)) continue;

    const source = await readFile(file, 'utf8');
    assert.equal(
      /\bfetch\s*\(|\bWebSocket\s*\(|\bEventSource\s*\(/.test(source),
      false,
      `${file.pathname} contains direct transport logic outside the adapter boundary`
    );
  }
});

test('runtime source does not hard-code infrastructure vendor identity', async () => {
  const vendorPattern = /\b(?:cloudflare|oracle cloud|oci|aws|amazon web services|azure|akamai|fastly|vercel|netlify)\b/i;
  for (const file of await filesUnder(srcRoot)) {
    const source = await readFile(file, 'utf8');
    assert.equal(
      vendorPattern.test(source),
      false,
      `${file.pathname} hard-codes infrastructure vendor identity; bind vendors outside runtime source`
    );
  }
});

test('source modules remain inside the Shared Web subtree', async () => {
  for (const file of await filesUnder(srcRoot)) {
    const path = file.pathname.replace(/\\/g, '/');
    assert.match(path, /\/(?:applications\/shared\/web|applications\/falcon-shared-web)\/src\//i);
  }
});
