import test from 'node:test';
import assert from 'node:assert/strict';
import { readdir, readFile } from 'node:fs/promises';
import { fileURLToPath } from 'node:url';
import path from 'node:path';

const here = path.dirname(fileURLToPath(import.meta.url));
const featuresRoot = path.resolve(here, '../src/features');

async function jsFiles(root) {
  const result=[];
  for (const entry of await readdir(root,{withFileTypes:true})) {
    const full=path.join(root,entry.name);
    if (entry.isDirectory()) result.push(...await jsFiles(full));
    else if (entry.isFile() && entry.name.endsWith('.js')) result.push(full);
  }
  return result;
}

test('WP02 presentation features cannot own network transport or cross-workstream internals', async () => {
  const files=await jsFiles(featuresRoot);
  assert.ok(files.length>0,'expected feature modules');

  const forbidden=[
    [/\bfetch\s*\(/u,'direct fetch'],
    [/\bnew\s+WebSocket\s*\(/u,'direct WebSocket'],
    [/\bXMLHttpRequest\b/u,'XMLHttpRequest'],
    [/\bEventSource\b/u,'EventSource'],
    [/\.innerHTML\s*=/u,'direct innerHTML mutation'],
    [/from\s+['"][^'"]*(?:Foundation|applications\/FSATS|application-development|foundation-development)[^'"]*['"]/iu,'cross-workstream internal import']
  ];

  for (const file of files) {
    const source=await readFile(file,'utf8');
    for (const [pattern,label] of forbidden) {
      assert.doesNotMatch(source,pattern,`${path.relative(featuresRoot,file)} must not use ${label}`);
    }
  }
});

test('WP02 app bootstrap no longer owns settings or catalog presentation markup', async () => {
  const source=await readFile(path.resolve(here,'../src/app.js'),'utf8');
  assert.doesNotMatch(source,/function\s+settingsPage\s*\(/u);
  assert.doesNotMatch(source,/presentCatalog\s*\(/u);
  assert.match(source,/createSettingsFeature/u);
  assert.match(source,/createCatalogPresentation/u);
});
