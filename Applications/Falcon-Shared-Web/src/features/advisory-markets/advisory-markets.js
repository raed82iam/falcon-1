import { safeText } from '../../security/safe-html.js';

function requireFunction(value, name) {
  if (typeof value !== 'function') throw new TypeError(`${name} must be a function`);
  return value;
}

const allowedHorizons = new Set(['DAILY','WEEKLY','MONTHLY']);

function normalizeMarket(raw = {}) {
  const horizons = Array.isArray(raw.supportedOpportunityHorizons)
    ? raw.supportedOpportunityHorizons.filter(value => allowedHorizons.has(String(value)))
    : [];

  return Object.freeze({
    marketId:String(raw.marketId ?? ''),
    marketCode:String(raw.marketCode ?? ''),
    displayName:String(raw.displayName ?? raw.marketCode ?? ''),
    operatingMode:String(raw.operatingMode ?? 'UNAVAILABLE'),
    horizons:Object.freeze(horizons),
    intradayOpportunityEnabled:raw.intradayOpportunityEnabled === true,
    executionCapability:String(raw.executionCapability ?? 'NONE'),
    positionTrackingForAdvisoryOpportunities:raw.positionTrackingForAdvisoryOpportunities === true,
    opportunityFollowUpEnabled:raw.opportunityFollowUpEnabled === true,
    availability:String(raw.availability ?? 'UNAVAILABLE'),
    reason:String(raw.reason ?? ''),
    providerDisplayName:String(raw.providerDisplayName ?? ''),
    sourceAccessType:String(raw.sourceAccessType ?? ''),
    dataMode:String(raw.dataMode ?? 'UNKNOWN'),
    delayMinutes:Number.isFinite(raw.delayMinutes) ? Math.max(0, raw.delayMinutes) : null,
    disclosureText:String(raw.disclosureText ?? ''),
    preview:raw.preview === true
  });
}

/**
 * FCR-0220 advisory-market presentation surface.
 *
 * This is presentation-only. The current Application market-profile/chart-source
 * identities are still planning semantics, not an executable runtime contract.
 * Web therefore renders supplied state without inventing activation, execution,
 * provider connectivity, School/Strategy applicability, or FSAPMA fetch authority.
 */
export function createAdvisoryMarketsFeature({ t, language, workspace, markets = [] } = {}) {
  requireFunction(t, 't');
  const currentLanguage = requireFunction(language, 'language');
  const renderWorkspace = requireFunction(workspace, 'workspace');
  const normalized = Array.isArray(markets) ? markets.map(normalizeMarket) : [];
  const local = (ar,en) => currentLanguage() === 'ar' ? ar : en;

  const horizonLabel = value => ({
    DAILY:local('يومي','Daily'),
    WEEKLY:local('أسبوعي','Weekly'),
    MONTHLY:local('شهري','Monthly')
  })[value] ?? value;

  function card(market) {
    const advisoryOnly = market.operatingMode === 'ADVISORY_ONLY';
    const available = market.availability === 'AVAILABLE';
    const delay = market.delayMinutes === null
      ? local('تأخير البيانات غير معروف','Data delay unknown')
      : market.delayMinutes > 0
        ? local(`البيانات متأخرة ${market.delayMinutes} دقيقة`,`Data delayed ${market.delayMinutes} minutes`)
        : local('لا يوجد تأخير معلن','No declared delay');
    const source = market.providerDisplayName
      ? `${market.providerDisplayName}${market.sourceAccessType ? ` · ${market.sourceAccessType}` : ''}`
      : local('مصدر العرض غير متاح بعد','Presentation source unavailable');
    const semanticViolation = !advisoryOnly
      || market.intradayOpportunityEnabled
      || market.executionCapability !== 'NONE'
      || market.positionTrackingForAdvisoryOpportunities
      || market.opportunityFollowUpEnabled;

    return `<article class="widget advisory-market-card ${semanticViolation || !available ? 'attention':''}">
      <div class="widget-head"><div><h3>${safeText(market.displayName || market.marketCode || market.marketId || local('سوق غير معروف','Unknown market'))}</h3><small>${safeText(market.marketCode)}</small></div><div class="advisory-statuses"><span class="status-chip">${safeText(advisoryOnly?'ADVISORY ONLY':market.operatingMode)}</span><span class="status-chip ${available?'':'muted'}">${safeText(market.availability)}</span></div></div>
      ${market.reason?`<div class="availability-reason"><b>${safeText(local('حالة المصدر','Source state'))}</b><span>${safeText(market.reason)}</span></div>`:''}
      <div class="advisory-horizons">${market.horizons.length ? market.horizons.map(h=>`<span>${safeText(horizonLabel(h))}</span>`).join('') : `<span class="muted">${safeText(local('الأفق غير متاح','Horizon unavailable'))}</span>`}</div>
      <div class="truth-grid"><span><b>${safeText(local('التنفيذ','Execution'))}</b>${safeText(market.executionCapability)}</span><span><b>${safeText(local('Intraday','Intraday'))}</b>${safeText(market.intradayOpportunityEnabled?local('مفعّل','Enabled'):local('معطّل','Disabled'))}</span><span><b>${safeText(local('متابعة المراكز','Position follow-up'))}</b>${safeText(market.positionTrackingForAdvisoryOpportunities?local('مفعّلة','Enabled'):local('غير موجودة','None'))}</span></div>
      <div class="provider-disclosure"><b>${safeText(source)}</b><span>${safeText(market.disclosureText || delay)}</span><small>${safeText(local('مصدر الشارت للعرض فقط ولا يمنح Web أو FSAPMA صلاحية اتصال أو حقيقة تداول.','Chart source is presentation-only and grants neither Web nor FSAPMA connectivity or Trading authority.'))}</small></div>
      ${!available?`<p class="muted tiny">${safeText(local('عدم توفر السوق أو المصدر يبقى ظاهرًا ولا يحوله Web إلى حالة حالية أو متاحة.','Unavailable market/source state remains explicit; Web does not upgrade it to current or available.'))}</p>`:''}
      ${market.preview?`<p class="muted tiny">${safeText(local('بيانات تجريبية للواجهة وليست حالة سوق حية.','Preview presentation data, not live market truth.'))}</p>`:''}
      ${semanticViolation?`<p class="negative tiny">${safeText(local('تم حجب الادعاء التشغيلي لأن الحالة المقدمة لا تطابق حدود ADVISORY_ONLY الحالية.','Operational claim suppressed because supplied state does not match current ADVISORY_ONLY boundaries.'))}</p>`:''}
    </article>`;
  }

  function advisoryMarketsPage() {
    const body = normalized.length
      ? `<div class="advisory-market-grid">${normalized.map(card).join('')}</div>`
      : `<section class="widget page-widget"><div class="empty-state"><h2>${safeText(local('لا توجد أسواق استشارية متاحة','No advisory markets available'))}</h2><p>${safeText(local('Web لا يخترع Market Profile أو Provider Source عند غياب العقد/المصدر.','Web does not invent a Market Profile or provider source when the governed source is absent.'))}</p></div></section>`;

    return renderWorkspace(`<div class="page-head"><div><h1>${safeText(local('الأسواق الاستشارية','Advisory Markets'))}</h1><p class="muted">${safeText(local('التحليل يبدأ بطلب المستخدم فقط. تحديث الشارت لا يشغّل FSAPMA ولا يوجد مسح تلقائي للخلفية.','Analysis starts only from a user request. Chart refresh does not trigger FSAPMA and there is no autonomous background scanning.'))}</p></div></div>${body}`,'advisory-markets');
  }

  return Object.freeze({ advisoryMarketsPage });
}
