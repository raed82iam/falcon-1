import { displayText } from '../../design-system/presentation.js';

const VALID_TIERS=new Set(['STANDARD','VIP']);

function normalizeTier(tier={}) {
  const id=String(tier.id ?? '').toUpperCase();
  if (!VALID_TIERS.has(id)) throw new TypeError('unsupported subscription tier');
  return Object.freeze({
    id,
    entitled:tier.entitled === true,
    current:tier.current === true,
    locked:tier.locked !== false,
    priceText:typeof tier.priceText === 'string' && tier.priceText.trim() ? tier.priceText.trim() : null,
    authoritative:tier.authoritative === true
  });
}

/**
 * Presentation-only Standard/VIP tier view.
 *
 * Price, trial, upgrade and entitlement claims are emitted only when the supplied
 * model explicitly declares them authoritative. Missing contract truth is shown
 * as unavailable instead of guessed.
 */
export function renderSubscriptionPresentation({ language='en', tiers=null, contractAvailable=false }={}) {
  const ar=language === 'ar';
  const title=ar ? 'الوصول والخطط' : 'Access & plans';
  const pending=ar
    ? 'تفاصيل الاشتراك والسعر غير متاحة حتى يصل عقد authoritative. لا يتم افتراض Trial أو Upgrade.'
    : 'Subscription and pricing details remain unavailable until an authoritative contract is supplied. Trial or upgrade state is not inferred.';

  if (contractAvailable !== true || !Array.isArray(tiers)) {
    return `<section class="subscription-presentation" aria-label="${displayText(title)}"><h2>${displayText(title)}</h2><p class="muted">${displayText(pending)}</p><div class="tier-grid"><article class="tier-card locked"><h3>Standard</h3><span class="status-chip">${displayText(ar?'العقد غير متاح':'Contract unavailable')}</span></article><article class="tier-card locked"><h3>VIP</h3><span class="status-chip">${displayText(ar?'العقد غير متاح':'Contract unavailable')}</span></article></div></section>`;
  }

  const normalized=tiers.map(normalizeTier);
  const cards=normalized.map(tier=>{
    const authoritative=tier.authoritative === true;
    const entitled=authoritative && tier.entitled && tier.current;
    const state=entitled ? (ar?'متاح':'Available') : (ar?'مقفل':'Locked');
    const price=authoritative && tier.priceText ? `<p>${displayText(tier.priceText)}</p>` : `<p class="muted">${displayText(ar?'السعر غير متاح':'Price unavailable')}</p>`;
    return `<article class="tier-card ${entitled?'':'locked'}"><h3>${tier.id === 'VIP'?'VIP':'Standard'}</h3>${price}<span class="status-chip">${displayText(state)}</span></article>`;
  }).join('');

  return `<section class="subscription-presentation" aria-label="${displayText(title)}"><h2>${displayText(title)}</h2><div class="tier-grid">${cards}</div><p class="muted tiny">TIER_VISIBLE ≠ ENTITLED ≠ ACTION_AUTHORIZED</p></section>`;
}
