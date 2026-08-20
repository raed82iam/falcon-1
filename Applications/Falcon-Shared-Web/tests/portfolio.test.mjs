import test from 'node:test';
import assert from 'node:assert/strict';
import { createPortfolioFeature } from '../src/features/portfolio/portfolio.js';

const t = key => ({
  portfolio: 'Portfolio',
  totalValue: 'Total value',
  todayPL: 'Today P/L',
  available: 'Available',
  positions: 'Positions',
  portfolioTruthNote:'Missing is not zero; stale is not current.'
})[key] ?? key;

const workspace = (body, active) => `<main data-active="${active}">${body}</main>`;

const data = {
  portfolio: { value: '$12,345', today: '-$45', available: '$500' },
  positions: [
    ['AAPL', '10', '$190', '+$120'],
    ['BTC', '0.1', '$60,000', '-$80']
  ]
};

test('portfolio feature renders supplied projection without becoming truth owner', () => {
  const { portfolioPage } = createPortfolioFeature({ t, workspace, data });
  const html = portfolioPage();

  assert.match(html, /data-active="portfolio"/);
  assert.match(html, /\$12,345/);
  assert.match(html, /AAPL/);
  assert.match(html, /BTC/);
  assert.match(html, /class="positive">\+\$120/);
  assert.match(html, /class="negative">-\$80/);
  assert.doesNotMatch(html, /\+2\.35%/);
});

test('portfolio feature renders a delta only when the supplied projection actually contains it', () => {
  const withDelta={...data,portfolio:{...data.portfolio,valueDelta:'+1.5%'}};
  const html=createPortfolioFeature({t,workspace,data:withDelta}).portfolioPage();
  assert.match(html,/\+1\.5%/);
});

test('portfolio feature fails closed when required projection data is absent', () => {
  assert.throws(
    () => createPortfolioFeature({ t, workspace, data: { portfolio: {} } }),
    /data\.portfolio and data\.positions are required/
  );
});

test('portfolio feature validates presentation dependencies', () => {
  assert.throws(() => createPortfolioFeature({ workspace, data }), /t must be a function/);
  assert.throws(() => createPortfolioFeature({ t, data }), /workspace must be a function/);
});
