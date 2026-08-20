import { safeText } from '../../security/safe-html.js';

function requireFunction(value, name) {
  if (typeof value !== 'function') throw new TypeError(`${name} must be a function`);
  return value;
}

const moneyClass = value => value == null ? 'muted' : String(value).trim().startsWith('-') ? 'negative' : 'positive';
const displayValue = (value, unavailableLabel='—') => value === null || value === undefined || value === '' ? unavailableLabel : value;
const metric = (id, label, value, delta = null, unavailableLabel='—') => `<section class="widget metric" data-widget="${safeText(id)}"><div class="widget-head"><span>${safeText(label)}</span></div><strong class="${value == null ? 'muted' : ''}">${safeText(displayValue(value,unavailableLabel))}</strong>${delta === null || delta === undefined || delta === '' ? '' : `<small class="${moneyClass(delta)}">${safeText(delta)}</small>`}</section>`;

/**
 * Customer portfolio presentation for the authenticated FSATS workspace.
 * Trading/FSATS remains the owner of portfolio, position and performance truth.
 * Null/no-source values are displayed as unavailable and are never coerced to zero.
 */
export function createPortfolioFeature({ t, workspace, data } = {}) {
  const translate = requireFunction(t, 't');
  const renderWorkspace = requireFunction(workspace, 'workspace');
  if (!data?.portfolio || !Array.isArray(data?.positions)) throw new TypeError('data.portfolio and data.positions are required');
  const unavailable = () => translate('unavailable') === 'unavailable' ? '—' : translate('unavailable');

  function portfolioPage() {
    const p = data.portfolio;
    const truth = p.envelope ?? null;
    const truthStrip = truth ? `<div class="portfolio-truth-strip"><span>${safeText(truth.truthState)}</span><span>${safeText(truth.freshnessState)}</span><span>${safeText(truth.completeness)}</span><span>${safeText(truth.availabilityState)}</span><small>${safeText(truth.asOfTime)}</small></div>` : '';
    return renderWorkspace(`<div class="page-head"><h1>${safeText(translate('portfolio'))}</h1></div>${truthStrip}<div class="metrics-row">${metric('x1',translate('totalValue'),p.value ?? p.totalEquity,p.valueDelta ?? p.returnPercent ?? null,unavailable())}${metric('x2',translate('todayPL'),p.today ?? p.dayPL,p.todayDelta ?? null,unavailable())}${metric('x3',translate('available'),p.available ?? p.availableFunds ?? p.cash,null,unavailable())}</div><section class="widget page-widget"><div class="widget-head"><h3>${safeText(translate('positions'))}</h3></div><div class="data-table">${data.positions.length ? data.positions.map(raw=>{const item=Array.isArray(raw)?{instrument:raw[0],quantity:raw[1],price:raw[2],pnl:raw[3]}:raw;return `<div class="row"><b>${safeText(displayValue(item.instrument ?? item.symbol ?? item?.instrument?.value,unavailable()))}</b><span>${safeText(displayValue(item.quantity,unavailable()))}</span><span>${safeText(displayValue(item.price ?? item.marketPrice,unavailable()))}</span><strong class="${moneyClass(item.pnl ?? item.unrealizedPnl)}">${safeText(displayValue(item.pnl ?? item.unrealizedPnl,unavailable()))}</strong></div>`;}).join('') : `<div class="empty-projection">${safeText(unavailable())}</div>`}</div><p class="truth-note">${safeText(translate('portfolioTruthNote'))}</p></section>`,'portfolio');
  }

  return Object.freeze({ portfolioPage });
}
