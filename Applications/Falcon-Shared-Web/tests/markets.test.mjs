import test from 'node:test';
import assert from 'node:assert/strict';
import { createMarketsFeature } from '../src/features/markets/markets.js';

const labels={markets:'Markets',usEquities:'US Equities',cryptoSpot:'Crypto Spot',marketOpen:'Open',aroundClock:'24/7',preview:'Preview',assetSearch:'Asset Search',information:'Information',marketDisplayNotice:'Presentation-only market data never flows into FSATS.',providerRoutesPending:'Provider routes are fail-closed pending governance.'};
const t = key => labels[key] ?? key;
const workspace = (body, active) => `<main data-active="${active}">${body}</main>`;
const catalogMarkup = () => '<div class="catalog-item">Strategy A</div>';

test('markets feature keeps viewing separate from Trading-universe admission and FSATS input', () => {
  const html=createMarketsFeature({t,language:()=> 'en',workspace,catalogMarkup}).marketsPage();
  assert.match(html, /data-active="markets"/);
  assert.match(html, /Viewing or searching an asset does not admit it to the Trading universe/);
  assert.match(html, /Presentation-only market data never flows into FSATS/);
  assert.match(html, /Provider routes are fail-closed pending governance/);
  assert.match(html, /Strategy A/);
});

test('markets feature preserves Arabic no-admission notice', () => {
  const html=createMarketsFeature({t,language:()=> 'ar',workspace,catalogMarkup}).marketsPage();
  assert.match(html, /عرض الأصل أو البحث عنه لا يعني إدخاله إلى عالم التداول/);
});

test('markets feature validates presentation dependencies', () => {
  assert.throws(() => createMarketsFeature({ language: () => 'en', workspace, catalogMarkup }), /t must be a function/);
  assert.throws(() => createMarketsFeature({ t, workspace, catalogMarkup }), /language must be a function/);
  assert.throws(() => createMarketsFeature({ t, language: () => 'en', catalogMarkup }), /workspace must be a function/);
  assert.throws(() => createMarketsFeature({ t, language: () => 'en', workspace }), /catalogMarkup must be a function/);
});
