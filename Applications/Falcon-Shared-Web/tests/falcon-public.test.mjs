import test from 'node:test';
import assert from 'node:assert/strict';
import { createFalconPublicFeature } from '../src/features/falcon-public/falcon-public.js';

const t = key => `[${key}]`;
const shell = (content, product = 'falcon') => `<shell product="${product}">${content}</shell>`;
const language = () => 'en';

function featureWithFsats() {
  return createFalconPublicFeature({
    t,
    language,
    publicShell: shell,
    apps: [
      { id: 'fsats', name: 'Falcon Self-Aware Trading System (FSATS)', kind: 'trading' }
    ],
    fsatsApps: [
      { id: 'fsata', name: 'Falcon Self-Aware Trading Application', shortName: 'FSATA' },
      { id: 'fsapma', name: 'Falcon Self-Aware Provider Management Application', shortName: 'FSAPMA' },
      { id: 'ftga', name: 'Falcon Trading Guardian Application', shortName: 'FTGA' },
      { id: 'fstsim', name: 'Falcon Self-Aware Trading Simulation Application', shortName: 'FSTSimA' },
      { id: 'app-rsc', name: 'Falcon Self-Aware Resource Management Application', shortName: 'APP-RSC' }
    ]
  });
}

test('Falcon public home follows the Owner reference multi-application landing composition', () => {
  const html = featureWithFsats().publicHome();
  assert.match(html, /FALCON OS/);
  assert.match(html, /An intelligent multi-application operating system/);
  assert.match(html, /falcon-shared-visual\.jpg/);
  assert.match(html, /hero-side-features/);
  assert.match(html, /hero-feature-point/);
  assert.doesNotMatch(html, /hero-side-card/);
  assert.doesNotMatch(html, /side-icon/);
  assert.match(html, /Falcon OS Applications/);
  assert.match(html, /FSATS Trading System Applications/);
  assert.doesNotMatch(html, /Protect\. Manage\. Grow capital\./);
  assert.doesNotMatch(html, /class="wing">F/);
});

test('Falcon public home presents FSATS as the current top-level Trading system', () => {
  const html = featureWithFsats().publicHome();
  assert.match(html, /data-nav="fsats"/);
  assert.match(html, /FALCON OS → FSATS/);
});

test('Falcon public home explains current child identities without flattening them into Falcon OS sibling systems', () => {
  const html = featureWithFsats().publicHome();
  for (const app of ['FSATA','FSAPMA','FTGA','FSTSimA','APP-RSC']) assert.match(html, new RegExp(app));
  assert.match(html, /Their explanations and discovery experience live inside the FSATS page/);
});

test('future top-level Falcon applications are not presented as operational actions', () => {
  const feature = createFalconPublicFeature({
    t,
    language,
    publicShell: shell,
    apps: [{ id: 'future', name: 'Future App', kind: 'future' }]
  });

  const html = feature.applicationsPage();
  assert.match(html, /disabled/);
  assert.doesNotMatch(html, /data-nav="future"/);
});

test('application display names are escaped before public rendering', () => {
  const feature = createFalconPublicFeature({
    t,
    language,
    publicShell: shell,
    apps: [{ id: 'unsafe', name: '<script>alert(1)</script>', kind: 'future' }],
    fsatsApps: [{ id: 'unsafe-child', name: '<img src=x onerror=alert(1)>', shortName: '<b>X</b>' }]
  });

  const homeHtml = feature.publicHome();
  const appsHtml = feature.applicationsPage();

  for (const html of [homeHtml, appsHtml]) {
    assert.doesNotMatch(html, /<script>/);
    assert.doesNotMatch(html, /<img src=x/);
    assert.doesNotMatch(html, /<b>X<\/b>/);
  }

  assert.match(appsHtml, /&lt;script&gt;/);
  assert.match(homeHtml, /&lt;b&gt;X&lt;\/b&gt;/);
});
