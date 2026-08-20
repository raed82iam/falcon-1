import test from 'node:test';
import assert from 'node:assert/strict';
import { readFile } from 'node:fs/promises';
import { messages } from '../src/i18n.js';
import { createShell } from '../src/composition/shell.js';

const tAr=key=>messages.ar[key] ?? key;
const tEn=key=>messages.en[key] ?? key;

test('Falcon public brand keeps title and subtitle stacked while preserving Arabic logo order',async()=>{
  const arabic=createShell({t:tAr,language:()=> 'ar',demoLabel:()=> ''}).publicShell('<section>محتوى</section>','falcon');
  const english=createShell({t:tEn,language:()=> 'en',demoLabel:()=> ''}).publicShell('<section>content</section>','falcon');
  const css=await readFile(new URL('../src/design-system.css',import.meta.url),'utf8');
  const arabicBrand=arabic.match(/<button type="button" class="brand" data-nav="home">.*?<\/button>/s)?.[0] ?? '';
  const englishBrand=english.match(/<button type="button" class="brand" data-nav="home">.*?<\/button>/s)?.[0] ?? '';

  assert.match(arabicBrand,/<span class="brand-copy"><b>FALCON OS<\/b><small>نظام تشغيل ذكي متعدد التطبيقات<\/small><\/span>/);
  assert.match(englishBrand,/<span class="brand-copy"><b>FALCON OS<\/b><small>Intelligent multi-application OS<\/small><\/span>/);
  assert.ok(arabicBrand.indexOf('brand-copy') < arabicBrand.indexOf('falcon-brand-logo'));
  assert.ok(englishBrand.indexOf('falcon-brand-logo') < englishBrand.indexOf('brand-copy'));
  assert.match(css,/\.brand-copy\s*\{[^}]*display:flex;[^}]*flex-direction:column;[^}]*align-items:flex-start;/s);
});
