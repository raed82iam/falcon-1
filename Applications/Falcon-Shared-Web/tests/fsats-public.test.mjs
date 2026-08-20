import test from 'node:test';
import assert from 'node:assert/strict';
import { createFsatsPublicFeature } from '../src/features/fsats-public/fsats-public.js';

const t = key => `[${key}]`;
const icon = name => `<i>${name}</i>`;
const shell = (content, product = 'falcon') => `<shell product="${product}">${content}</shell>`;

function feature(language) {
  return createFsatsPublicFeature({ t, language: () => language, publicShell: shell, icon });
}

test('FSATS public surface stays a trading-specific public entry surface', () => {
  const html = feature('en').fsatsLanding();
  assert.match(html, /product="fsats"/);
  assert.match(html, /FALCON • FSATS/);
  assert.match(html, /data-auth-submit/);
  assert.match(html, /Operational sign-in binding is not available yet/);
});

test('FSATS page contains all five Owner-defined child Applications', () => {
  const html = feature('en').fsatsLanding();
  for (const app of ['FSATA','FSAPMA','FTGA','FSTSimA','APP-RSC']) assert.match(html, new RegExp(app));
  assert.match(html, /FSATS System Applications/);
  assert.match(html, /Discover Application/);
  assert.match(html, /Simple visual explainer/);
});

test('FSATS discovery remains explanatory and does not claim live runtime', () => {
  const html = feature('en').fsatsLanding();
  assert.match(html, /does not claim a live runtime/);
  assert.match(html, /Final video or animated assets can be added later/);
});

test('FSATS sign-in presents Google and Microsoft as federated options', () => {
  const html = feature('en').fsatsLanding();
  assert.match(html, /data-auth-provider="GOOGLE"/);
  assert.match(html, /Continue with Google/);
  assert.match(html, /data-auth-provider="MICROSOFT"/);
  assert.match(html, /Continue with Microsoft/);
});

test('FSATS sign-in explains portable Authenticator MFA', () => {
  const html = feature('en').fsatsLanding();
  assert.match(html, /Google Authenticator/);
  assert.match(html, /Microsoft Authenticator/);
  assert.match(html, /without locking Falcon to one brand/);
});

test('FSATS public form controls expose stable names for browser form semantics', () => {
  const html = feature('en').fsatsLanding();
  for (const name of ['username','password','remember-me','full-name','email','emergency-phone']) {
    assert.match(html, new RegExp(`name="${name}"`));
  }
});

test('new account onboarding requires emergency-contact phone presentation', () => {
  const html = feature('en').fsatsLanding();
  assert.match(html, /Phone number for emergency contact/);
  assert.match(html, /type="tel"/);
  assert.match(html, /required/);
  assert.match(html, /emergencies and high-priority incidents/);
  assert.match(html, /not OTP verification today/);
  assert.match(html, /PHONE_PROVIDED ≠ PHONE_VERIFIED ≠ FALCON_IDENTITY ≠ BUSINESS_AUTHORITY/);
});

test('OTP delivery remains deferred in the active onboarding surface', () => {
  const html = feature('en').fsatsLanding();
  assert.match(html, /OTP delivery through Telegram, WhatsApp or SMS is deferred/);
});

test('FSATS public surface renders the Arabic fail-closed auth and onboarding messages', () => {
  const html = feature('ar').fsatsLanding();
  assert.match(html, /معاينة واجهة المستخدم/);
  assert.match(html, /لن يخترع الويب هوية أو صلاحية/);
  assert.match(html, /المتابعة باستخدام Google/);
  assert.match(html, /المتابعة باستخدام Microsoft/);
  assert.match(html, /رقم الهاتف للتواصل الطارئ/);
  assert.match(html, /استكشف التطبيق/);
});

test('FSATS public surface carries no unlicensed regulatory claim', () => {
  const html = feature('en').fsatsLanding();
  assert.doesNotMatch(html, /\blicensed\b|\bregulated\b|\bCMA\b|هيئة سوق المال|مرخص/iu);
});
