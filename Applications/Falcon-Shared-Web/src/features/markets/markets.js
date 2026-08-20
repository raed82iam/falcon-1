function requireFunction(value, name) {
  if (typeof value !== 'function') throw new TypeError(`${name} must be a function`);
  return value;
}

/**
 * Customer market discovery and presentation surface.
 *
 * Ordinary raw market display data is Web-owned presentation data under FCR-0125.
 * FSATS analysis/School/Strategy/Risk results remain Application-owned. Searching
 * or displaying an instrument never admits it to the Trading universe.
 */
export function createMarketsFeature({ t, language, workspace, catalogMarkup } = {}) {
  const translate = requireFunction(t, 't');
  const currentLanguage = requireFunction(language, 'language');
  const renderWorkspace = requireFunction(workspace, 'workspace');
  const renderCatalog = requireFunction(catalogMarkup, 'catalogMarkup');

  function marketsPage() {
    return renderWorkspace(`<div class="page-head"><h1>${translate('markets')}</h1></div><div class="market-cards"><article><b>${translate('usEquities')}</b><strong>${translate('marketOpen')}</strong><small>${translate('preview')}</small></article><article><b>${translate('cryptoSpot')}</b><strong>${translate('aroundClock')}</strong><small>${translate('preview')}</small></article></div><section class="widget page-widget"><h3>${translate('assetSearch')}</h3><input class="search" placeholder="AAPL, BTC…" aria-label="${translate('assetSearch')}"><p class="muted">${currentLanguage()==='ar'?'عرض الأصل أو البحث عنه لا يعني إدخاله إلى عالم التداول ولا تفعيل أي استراتيجية.':'Viewing or searching an asset does not admit it to the Trading universe or activate a strategy.'}</p><div class="catalog-list">${renderCatalog()}</div><div class="attention"><b>${translate('information')}</b><p>${translate('marketDisplayNotice')}</p><small>${translate('providerRoutesPending')}</small></div></section>`,'markets');
  }

  return Object.freeze({ marketsPage });
}
