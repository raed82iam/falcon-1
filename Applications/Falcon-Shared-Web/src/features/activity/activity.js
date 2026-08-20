import { safeText } from '../../security/safe-html.js';

function requireFunction(value, name) {
  if (typeof value !== 'function') throw new TypeError(`${name} must be a function`);
  return value;
}

const normalizeItem = raw => Array.isArray(raw)
  ? { instrument: raw[0], side: raw[1], quantity: raw[2], price: raw[3], effectiveAt: raw[4], state: raw[5] ?? 'UNKNOWN_BROKER_OUTCOME' }
  : raw;

const display = value => value === null || value === undefined || value === '' ? '—' : value;

/**
 * Customer order/trade activity presentation for the authenticated FSATS workspace.
 * Trading/FSATS owns lifecycle and execution truth. Web renders the supplied
 * exact state and never upgrades an unknown/accepted/partial outcome to FILLED.
 */
export function createActivityFeature({ t, workspace, data } = {}) {
  const translate = requireFunction(t, 't');
  const renderWorkspace = requireFunction(workspace, 'workspace');
  if (!Array.isArray(data?.trades)) throw new TypeError('data.trades is required');

  function truthStrip() {
    const truth=data.activityEnvelope ?? null;
    if (!truth) return `<div class="truth-note">${safeText(translate('orderTruthNote'))}</div>`;
    return `<div class="portfolio-truth-strip"><span>${safeText(display(truth.truthState))}</span><span>${safeText(display(truth.freshnessState))}</span><span>${safeText(display(truth.completeness))}</span><span>${safeText(display(truth.availabilityState))}</span><small>${safeText(display(truth.asOfTime))}</small></div>`;
  }

  function lineage(item) {
    const kind=item.updateKind ?? item.lineageKind ?? null;
    const corrects=item.correctsUpdateId ?? null;
    const supersedes=item.supersedesUpdateId ?? null;
    if (!kind && !corrects && !supersedes) return '';
    const refs=[];
    if (corrects) refs.push(`corrects ${corrects}`);
    if (supersedes) refs.push(`supersedes ${supersedes}`);
    return `<small class="muted">${safeText(kind ?? 'LINEAGE')} ${refs.length ? `· ${refs.join(' · ')}` : ''}</small>`;
  }

  function activityPage() {
    const rows=data.trades.length
      ? data.trades.map(raw=>{
          const item=normalizeItem(raw);
          const side=item.side ?? '—';
          return `<div class="row"><b>${safeText(display(item.instrument ?? item?.instrument?.value))}</b><span class="${side==='BUY'?'positive':side==='SELL'?'negative':''}">${safeText(translate(`orderSide_${side}`))}</span><span>${safeText(display(item.quantity ?? item.requestedQuantity))}</span><span>${safeText(display(item.price ?? item.averageFillPrice))}</span><span>${safeText(display(item.effectiveAt ?? item.asOfTime))}</span><span class="status-chip">${safeText(translate(`orderState_${item.state ?? 'UNKNOWN_BROKER_OUTCOME'}`))}</span>${item.truthState||item.freshnessState?`<small>${safeText(display(item.truthState))} · ${safeText(display(item.freshnessState))}</small>`:''}${lineage(item)}</div>`;
        }).join('')
      : `<div class="empty-projection">—</div>`;

    return renderWorkspace(`<div class="page-head"><h1>${safeText(translate('activity'))}</h1></div>${truthStrip()}<section class="widget page-widget"><div class="tabs"><button class="active">${safeText(translate('active'))}</button><button>${safeText(translate('trades'))}</button><button>${safeText(translate('history'))}</button></div><div class="data-table">${rows}</div><div class="truth-note">${safeText(translate('orderTruthNote'))}</div></section>`,'activity');
  }

  return Object.freeze({ activityPage });
}
