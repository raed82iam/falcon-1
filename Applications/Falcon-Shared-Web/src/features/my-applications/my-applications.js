import { displayText } from '../../design-system/presentation.js';
import { renderSubscriptionPresentation } from './subscription-presentation.js';

function requireFunction(value, name) {
  if (typeof value !== 'function') throw new TypeError(`${name} must be a function`);
  return value;
}

function normalizeApps(apps) {
  if (!Array.isArray(apps)) throw new TypeError('apps must be an array');
  return apps.map(app => Object.freeze({
    id: String(app?.id ?? ''),
    name: String(app?.name ?? ''),
    kind: String(app?.kind ?? '')
  }));
}

function applicationEntitled(applicationAccess, appId) {
  const access=applicationAccess?.[appId];
  return Boolean(
    access
    && access.entitled === true
    && access.current === true
    && access.businessAuthorityGranted === false
  );
}

/** Authenticated Falcon user home for Application discovery/access presentation. */
export function createMyApplicationsFeature({
  t,
  language,
  publicShell,
  demoBadge,
  apps,
  previewMode = false,
  applicationAccess = null,
  subscriptionModel = null
} = {}) {
  const translate = requireFunction(t, 't');
  const currentLanguage = requireFunction(language, 'language');
  const shell = requireFunction(publicShell, 'publicShell');
  const renderDemoBadge = requireFunction(demoBadge, 'demoBadge');
  const catalog = normalizeApps(apps);

  function myApplicationsPage() {
    const arabic = currentLanguage() === 'ar';
    const description = arabic
      ? 'بوابة Falcon للتطبيقات والأنظمة. ظهور البطاقة لا يعني أن الوصول التشغيلي مسموح.'
      : 'Falcon home for Applications and systems. Card visibility does not imply operational access.';

    const cards = catalog.map(app => {
      const isFsats = app.id === 'fsats' || app.kind === 'trading';
      if (isFsats) {
        const entitled=applicationEntitled(applicationAccess,app.id);
        const preview=previewMode === true;
        const canOpen=preview || entitled;
        const openLabel=preview
          ? (arabic ? 'معاينة' : 'Preview')
          : entitled ? translate('open') : (arabic ? 'غير متاح' : 'Unavailable');
        const accessNote=preview
          ? (arabic ? 'هذه معاينة فقط ولا تمثل entitlement أو تشغيلًا حيًا.' : 'Preview only; this is not entitlement or live activation.')
          : entitled
            ? (arabic ? 'الوصول مبني على entitlement حالية، ولا يمنح صلاحية تداول أو تنفيذ.' : 'Access is based on current entitlement and grants no trading or execution authority.')
            : (arabic ? 'الوصول مقفول حتى تصل entitlement موثوقة وحالية.' : 'Access stays locked until current authoritative entitlement is available.');
        return `<article class="app-card featured"><div class="app-icon">◫</div><h3>${displayText(app.name || 'FSATS Trading')}</h3><p>${translate('tradingDesc')}</p><p class="muted tiny">${displayText(accessNote)}</p><div class="hero-actions"><button class="primary" ${canOpen?'data-nav="trader"':'disabled aria-disabled="true"'}>${displayText(openLabel)}</button><button class="secondary" data-nav="fsats">${translate('discover')}</button></div></article>`;
      }
      return `<article class="app-card"><h3>${displayText(app.name)}</h3><p>${translate(`${app.kind}Desc`)}</p><span class="status-chip">${translate('coming')}</span></article>`;
    }).join('');

    const subscription=renderSubscriptionPresentation({
      language:arabic?'ar':'en',
      contractAvailable:subscriptionModel?.contractAvailable === true,
      tiers:subscriptionModel?.tiers ?? null
    });

    return shell(`${renderDemoBadge()}<section class="section page"><div class="section-head"><div><h1>${translate('myApps')}</h1><p>${description}</p></div></div><nav class="apps-grid" aria-label="${arabic?'أنظمة وتطبيقات Falcon':'Falcon systems and Applications'}">${cards}</nav><p class="muted tiny">CARD_VISIBLE ≠ ENTITLED ≠ ACTION_AUTHORIZED ≠ RUNTIME_ACTIVATED</p>${subscription}</section>`);
  }

  return Object.freeze({ myApplicationsPage });
}
