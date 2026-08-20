import test from 'node:test';
import assert from 'node:assert/strict';
import { readFile } from 'node:fs/promises';
import { messages } from '../src/i18n.js';
import { createShell } from '../src/composition/shell.js';
import { createNotificationsFeature } from '../src/features/notifications/notifications.js';
import { createOwnerCommandCenterFeature } from '../src/features/owner-command-center/owner-command-center.js';
import { createOwnerAiEmergencyFeature } from '../src/features/owner-ai-emergency/owner-ai-emergency.js';

const workspace=(body)=>body;
const tAr=key=>messages.ar[key] ?? key;
const tEn=key=>messages.en[key] ?? key;

const incident={priority:'HIGH',status:'OPEN',resolved:false,mode:'FALCON_ACTIVE',message:'x'};

test('disabled incident actions expose aria-disabled and text input has accessible name',()=>{
  const html=createNotificationsFeature({t:tEn,workspace,data:{incidentConversation:incident}}).notificationsPage();
  assert.match(html,/data-incident-action="upload-screenshot" disabled aria-disabled="true"/);
  assert.match(html,/data-incident-action="voice" disabled aria-disabled="true"/);
  assert.match(html,/aria-label="Reply to Falcon…" disabled aria-disabled="true"/);
});

test('disabled Support takeover exposes aria-disabled',()=>{
  const feature=createOwnerCommandCenterFeature({
    t:tEn,language:()=> 'en',workspace,
    data:{owner:{health:'UNAVAILABLE',apps:'0',users:'0',incidents:'1',approvals:'0'},services:[],incidents:[['HIGH','x']]},
    supportAuthorization:()=>null
  });
  assert.match(feature.ownerIncidents(),/data-support-takeover="0" disabled aria-disabled="true"/);
});

test('AI emergency unavailable controls expose disabled state and Arabic/English fail-closed copy',()=>{
  const model={targets:[],selectedTarget:null,selectedAction:'KILL',blastRadius:null,decision:null,transportAvailable:false};
  const english=createOwnerAiEmergencyFeature({t:tEn,language:()=> 'en',workspace,session:null,model}).page();
  assert.match(english,/data-ai-emergency-target disabled aria-disabled="true"/);
  assert.match(english,/data-ai-emergency-submit disabled aria-disabled="true"/);
  assert.match(english,/Runtime binding unavailable/);
  const arabic=createOwnerAiEmergencyFeature({t:tAr,language:()=> 'ar',workspace,session:null,model}).page();
  assert.match(arabic,/التحكم الطارئ بالـAI/);
  assert.match(arabic,/الإرسال مقفول/);
  assert.doesNotMatch(arabic,/Submission locked/);
});

test('new security and escalation labels exist in both Arabic and English',()=>{
  for (const key of ['incidentUnavailableActionsNotice','supportTakeoverUnavailable','fiveMinuteNoReplyFollowup','fiveMinuteNoReplyTruthNotice']) {
    assert.equal(typeof messages.ar[key],'string');
    assert.ok(messages.ar[key].length>0);
    assert.equal(typeof messages.en[key],'string');
    assert.ok(messages.en[key].length>0);
    assert.notEqual(messages.ar[key],key);
    assert.notEqual(messages.en[key],key);
  }
});

test('Arabic incident security labels do not fall back to English text',()=>{
  const html=createNotificationsFeature({t:tAr,workspace,data:{incidentConversation:incident}}).notificationsPage();
  assert.match(html,/بعض إجراءات الحادث غير متاحة/);
  assert.doesNotMatch(html,/Some incident actions remain unavailable/);
});

test('workspace navigation exposes keyboard-native mobile menu and current-page semantics',()=>{
  const shell=createShell({t:tEn,language:()=> 'en',demoLabel:()=> ''});
  const html=shell.workspace('<section>content</section>','advisory-markets',false);
  assert.match(html,/<details class="mobile-menu">/);
  assert.match(html,/<summary aria-label="Navigation menu">☰<\/summary>/);
  assert.match(html,/nav aria-label="Primary navigation"/);
  assert.match(html,/data-nav="advisory-markets" class="active" aria-current="page"/);
});

test('public Falcon navigation exposes a keyboard-native responsive menu with the full public navigation set',()=>{
  const html=createShell({t:tEn,language:()=> 'en',demoLabel:()=> ''}).publicShell('<section>content</section>','falcon');
  assert.match(html,/<details class="public-mobile-menu">/);
  assert.match(html,/<summary aria-label="Navigation menu">☰<\/summary>/);
  assert.match(html,/data-nav="home"/);
  assert.match(html,/data-nav="apps"/);
  for (const label of ['Features','Pricing','Partners','Resources','About us','Contact']) assert.match(html,new RegExp(`>${label}<`));
});

test('public Arabic navigation exposes Arabic responsive-menu accessibility labels',()=>{
  const html=createShell({t:tAr,language:()=> 'ar',demoLabel:()=> ''}).publicShell('<section>محتوى</section>','falcon');
  assert.match(html,/<details class="public-mobile-menu">/);
  assert.match(html,/aria-label="قائمة التنقل"/);
  assert.match(html,/aria-label="التنقل الرئيسي"/);
  for (const label of ['المميزات','الأسعار','الشركاء','الموارد','من نحن','تواصل معنا']) assert.match(html,new RegExp(`>${label}<`));
});

test('public responsive-menu stylesheet pins the mobile toggle to physical left and contains the menu inside the viewport in LTR and RTL',async()=>{
  const css=await readFile(new URL('../src/mobile-navigation.css',import.meta.url),'utf8');
  assert.match(css,/\.public-mobile-menu/);
  assert.match(css,/@media\(max-width:760px\)/);
  assert.match(css,/\.topbar\{height:auto;min-height:74px;flex-wrap:wrap;gap:10px;padding-block:10px;position:sticky\}/);
  assert.match(css,/\.topbar>\.brand\{order:1;margin-left:auto;margin-right:0\}/);
  assert.match(css,/\.topbar>\.public-mobile-menu\{order:2;position:absolute;left:4vw;right:auto;top:10px;margin:0\}/);
  assert.match(css,/\.topbar>\.public-mobile-menu nav\{inset:auto;top:calc\(100% \+ 8px\);left:0;right:auto;width:min\(240px,calc\(100vw - 8vw\)\);min-width:0;max-width:calc\(100vw - 8vw\)\}/);
  assert.match(css,/@media\(min-width:761px\)\{\.mobile-menu,\.public-mobile-menu\{display:none!important\}\}/);
});

test('Owner workspace attention control stays on Owner surface instead of customer notifications',()=>{
  const shell=createShell({t:tEn,language:()=> 'en',demoLabel:()=> ''});
  const html=shell.workspace('<section>owner</section>','owner',true);
  assert.match(html,/data-nav="owner-incidents" aria-label="Open Incidents"/);
  assert.doesNotMatch(html,/data-nav="notifications"/);
});

test('Arabic workspace navigation exposes Arabic accessible menu labels',()=>{
  const shell=createShell({t:tAr,language:()=> 'ar',demoLabel:()=> ''});
  const html=shell.workspace('<section>محتوى</section>','trader',false);
  assert.match(html,/aria-label="قائمة التنقل"/);
  assert.match(html,/aria-label="التنقل الرئيسي"/);
});

test('index loads visible keyboard focus and reduced-motion accessibility rules',async()=>{
  const index=await readFile(new URL('../index.html',import.meta.url),'utf8');
  const css=await readFile(new URL('../src/accessibility.css',import.meta.url),'utf8');
  assert.match(index,/href="\.\/src\/accessibility\.css"/);
  assert.match(css,/:focus-visible/);
  assert.match(css,/outline:3px solid var\(--blue\)/);
  assert.match(css,/prefers-reduced-motion: reduce/);
});

test('shell brand presentation is CSP-safe and uses the Owner-approved same-origin asset',async()=>{
  const shell=createShell({t:tEn,language:()=> 'en',demoLabel:()=> ''});
  const publicHtml=shell.publicShell('<section>content</section>','falcon');
  const workspaceHtml=shell.workspace('<section>content</section>','trader',false);
  const css=await readFile(new URL('../src/design-system.css',import.meta.url),'utf8');
  assert.doesNotMatch(publicHtml,/\sstyle=/i);
  assert.doesNotMatch(workspaceHtml,/\sstyle=/i);
  assert.doesNotMatch(publicHtml,/src="data:/i);
  assert.doesNotMatch(workspaceHtml,/src="data:/i);
  assert.match(publicHtml,/src="\.\/src\/assets\/falcon-brand-owner-reference\.jpg"/);
  assert.match(workspaceHtml,/src="\.\/src\/assets\/falcon-brand-owner-reference\.jpg"/);
  assert.match(publicHtml,/class="falcon-brand-logo"/);
  assert.match(workspaceHtml,/class="falcon-brand-logo"/);
  assert.match(css,/\.falcon-brand-logo\s*\{/);
  assert.match(css,/width:36px;/);
  assert.match(css,/height:36px;/);
});

test('Arabic public brand places the logo physically left of the brand text while English stays unchanged',()=>{
  const arabic=createShell({t:tAr,language:()=> 'ar',demoLabel:()=> ''}).publicShell('<section>محتوى</section>','falcon');
  const english=createShell({t:tEn,language:()=> 'en',demoLabel:()=> ''}).publicShell('<section>content</section>','falcon');
  const arabicBrand=arabic.match(/<button type="button" class="brand" data-nav="home">.*?<\/button>/s)?.[0] ?? '';
  const englishBrand=english.match(/<button type="button" class="brand" data-nav="home">.*?<\/button>/s)?.[0] ?? '';
  assert.ok(arabicBrand.indexOf('class="brand-copy"') < arabicBrand.indexOf('falcon-brand-logo'));
  assert.ok(englishBrand.indexOf('falcon-brand-logo') < englishBrand.indexOf('class="brand-copy"'));
});
