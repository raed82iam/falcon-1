import { safeText } from '../../security/safe-html.js';

function requireFunction(value, name) {
  if (typeof value !== 'function') throw new TypeError(`${name} must be a function`);
  return value;
}

const moneyClass = value => value === null || value === undefined || value === '' ? 'muted' : String(value).trim().startsWith('-') ? 'negative' : 'positive';
const display = value => value === null || value === undefined || value === '' ? '—' : value;

/**
 * FSATS authenticated user workspace presentation.
 *
 * Authoritative portfolio, market, activity, catalog, analysis and incident truth
 * stays owned by the relevant contracts. This feature only composes Web presentation
 * and Web-owned layout preferences. It does not calculate performance or upgrade truth.
 */
export function createFsatsWorkspaceFeature({ t, language, workspace, store, data, catalogMarkup, previewMode=false } = {}) {
  const translate = requireFunction(t, 't');
  const currentLanguage = requireFunction(language, 'language');
  const renderWorkspace = requireFunction(workspace, 'workspace');
  if (!store || typeof store.layout !== 'function') throw new TypeError('store.layout must be a function');
  if (!data) throw new TypeError('data is required');
  const renderCatalog = requireFunction(catalogMarkup, 'catalogMarkup');

  const local = (ar,en) => currentLanguage()==='ar' ? ar : en;
  const widgetMenu = id => `<div class="widget-actions"><button data-size="${safeText(id)}" title="${safeText(local('تغيير الحجم','Resize'))}">↔</button><button data-hide="${safeText(id)}" title="${safeText(translate('hide'))}">×</button></div>`;
  const metric = (id,label,value,delta=null) => `<section class="widget metric" data-widget="${safeText(id)}"><div class="widget-head"><span>${safeText(label)}</span>${widgetMenu(id)}</div><strong class="${value == null ? 'muted' : ''}">${safeText(display(value))}</strong>${delta === null || delta === undefined || delta === '' ? '' : `<small class="${moneyClass(delta)}">${safeText(delta)}</small>`}</section>`;

  function overlayMarkup() {
    const overlay = data.tradingOverlay ?? null;
    if (!overlay) return `<div class="overlay-status muted"><b>${safeText(local('طبقة التداول','Trading overlay'))}</b><span>${safeText(local('غير متوفرة من المصدر','Unavailable from source'))}</span></div>`;
    if (overlay.renderable !== true) {
      return `<div class="overlay-status muted"><b>${safeText(local('طبقة التداول','Trading overlay'))}</b><span>${safeText(overlay.applicability ?? 'UNAVAILABLE')}</span>${overlay.reasonCode?`<small>${safeText(overlay.reasonCode)}</small>`:''}</div>`;
    }
    const elements = Array.isArray(overlay.elements) ? overlay.elements : [];
    return `<div class="overlay-status"><div class="overlay-head"><b>${safeText(local('طبقة التداول من FSATS','FSATS Trading overlay'))}</b><span class="status-chip">${safeText(overlay.applicability ?? 'UNKNOWN')} · ${safeText(overlay.truth ?? 'UNKNOWN')}</span></div><div class="overlay-elements">${elements.length ? elements.map(element=>`<span><b>${safeText(element.label ?? element.type)}</b><small>${safeText(element.price ?? element.value ?? element.state ?? '—')}</small></span>`).join('') : `<span class="muted">${safeText(local('لا توجد عناصر قابلة للرسم','No renderable elements'))}</span>`}</div>${overlay.asOfTime?`<small>${safeText(overlay.asOfTime)}</small>`:''}<p class="muted tiny">${safeText(local('هذه العناصر من إسقاط Application ولا يعيد Web بناء منطق المدرسة أو الاستراتيجية.','These elements come from the Application projection; Web does not reconstruct School or Strategy logic.'))}</p></div>`;
  }

  function catalogBody() {
    const items = Array.isArray(data.catalog) ? data.catalog : [];
    const hasAuthoritativeSchoolMetadata = items.some(item => item?.schoolId || item?.schoolName);
    if (!hasAuthoritativeSchoolMetadata) return renderCatalog();

    const groups = new Map();
    for (const item of items) {
      const key = String(item.schoolId ?? item.schoolName ?? 'UNSPECIFIED_SCHOOL');
      const entry = groups.get(key) ?? { id:key, name:item.schoolName ?? key, items:[] };
      entry.items.push(item);
      groups.set(key,entry);
    }

    return `<div class="catalog-school-groups">${[...groups.values()].map(group=>`<section class="catalog-school-group"><div class="catalog-school-head"><b>${safeText(group.name)}</b><small>${safeText(local('مجموعة مدرسة من كتالوج Trading','School group from Trading catalog'))}</small></div>${group.items.map(item=>{const enabled=item.enabled === true && item.applicability === 'APPLICABLE';return `<div class="catalog-item ${enabled?'':'disabled'}"><div><b>${safeText(item.name)}</b><small>${safeText(item.applicability ?? 'UNKNOWN')}</small></div><button ${enabled?'':'disabled aria-disabled="true"'}>${enabled?'+':'×'}</button>${item.reason?`<p>${safeText(item.reason)}</p>`:''}</div>`;}).join('')}</section>`).join('')}</div>`;
  }

  const positionsMarkup = () => data.positions.length
    ? data.positions.map(raw=>{const p=Array.isArray(raw)?{instrument:raw[0],quantity:raw[1],price:raw[2],pnl:raw[3]}:raw;return `<div class="row"><b>${safeText(display(p.instrument ?? p?.instrument?.value))}</b><span>${safeText(display(p.quantity))}</span><span>${safeText(display(p.price ?? p.marketPrice))}</span><strong class="${moneyClass(p.pnl ?? p.unrealizedPnl)}">${safeText(display(p.pnl ?? p.unrealizedPnl))}</strong></div>`;}).join('')
    : `<div class="empty-projection">${safeText(local('لا توجد مراكز متاحة من المصدر','No positions available from source'))}</div>`;
  const tradesMarkup = () => data.trades.length
    ? data.trades.map(raw=>{const p=Array.isArray(raw)?{instrument:raw[0],side:raw[1],quantity:raw[2],effectiveAt:raw[4],state:raw[5]}:raw;return `<div class="row"><b>${safeText(display(p.instrument ?? p?.instrument?.value))}</b><span class="${p.side==='BUY'?'positive':p.side==='SELL'?'negative':''}">${safeText(translate(`orderSide_${p.side ?? '—'}`))}</span><span>${safeText(display(p.quantity ?? p.requestedQuantity))}</span><span>${safeText(display(p.effectiveAt ?? p.asOfTime))}</span><small>${safeText(translate(`orderState_${p.state ?? 'UNKNOWN_BROKER_OUTCOME'}`))}</small></div>`;}).join('')
    : `<div class="empty-projection">${safeText(local('لا يوجد نشاط أو صفقات متاحة من المصدر','No activity or trades available from source'))}</div>`;

  const marketInsights = () => {
    const assets=(Array.isArray(data.positions)?data.positions:[]).slice(0,4).map(raw=>Array.isArray(raw)
      ? {name:raw[0],value:raw[2],change:raw[3]}
      : {name:raw.instrument?.value ?? raw.instrument,value:raw.marketPrice ?? raw.price,change:raw.unrealizedPnl ?? raw.pnl});
    return `<div class="market-insights"><section class="allocation-card"><div class="allocation-ring"><span>${safeText(display(data.portfolio.value ?? data.portfolio.totalEquity))}</span><small>${safeText(local('الإجمالي','Total'))}</small></div><div><b>${safeText(local('توزيع المحفظة','Portfolio allocation'))}</b><ul><li><i class="alloc-a"></i>${safeText(local('أسهم','Equities'))}<span>45%</span></li><li><i class="alloc-b"></i>${safeText(local('أصول رقمية','Digital assets'))}<span>30%</span></li><li><i class="alloc-c"></i>${safeText(local('نقد','Cash'))}<span>25%</span></li></ul></div></section><section class="top-assets"><b>${safeText(local('أهم الأصول','Top assets'))}</b>${assets.length?assets.map(asset=>`<div><span><strong>${safeText(display(asset.name))}</strong><small>${safeText(display(asset.value))}</small></span><em class="${moneyClass(asset.change)}">${safeText(display(asset.change))}</em></div>`).join(''):`<p class="muted">${safeText(local('لا توجد أصول متاحة','No assets available'))}</p>`}</section></div>`;
  };

  const widgets = {
    market: () => `<section class="widget wide" data-widget="market"><div class="widget-head"><h3>${safeText(translate('chart'))}</h3>${widgetMenu('market')}</div><div class="chart"><div class="chart-grid"></div><svg viewBox="0 0 800 220" preserveAspectRatio="none" aria-label="${safeText(local('رسم بياني تجريبي','Development preview chart'))}"><polyline fill="none" stroke="currentColor" stroke-width="3" points="0,170 70,145 130,155 190,95 250,115 310,70 380,105 450,65 520,82 590,45 650,63 720,35 800,48"/></svg></div>${marketInsights()}${overlayMarkup()}<p class="muted tiny">${safeText(local('بيانات السوق للعرض وطبقات التداول مصدران منفصلان. Web يجمعهما بصريًا فقط ولا يعيد بيانات العرض إلى FSATS.','Market display data and Trading overlays are separate sources. Web composes them visually only and never feeds display data back to FSATS.'))}</p></section>`,
    catalog: () => `<section class="widget" data-widget="catalog"><div class="widget-head"><h3>${safeText(translate('schools'))} &amp; ${safeText(translate('strategies'))}</h3>${widgetMenu('catalog')}</div>${catalogBody()}<p class="muted tiny">${safeText(local('اسم المدرسة هنا تجميع عرضي من SchoolId/SchoolName المرسلة مع الاستراتيجيات، وليس School applicability مخترعة. ظهور الاستراتيجية لا يعني تفعيلها.','School names here are presentation grouping from SchoolId/SchoolName supplied with strategies, not invented School applicability. Strategy visibility does not mean activation.'))}</p></section>`,
    portfolio: () => metric('portfolio',translate('totalValue'),data.portfolio.value ?? data.portfolio.totalEquity,data.portfolio.valueDelta ?? data.portfolio.returnPercent ?? null),
    daily: () => metric('daily',translate('todayPL'),data.portfolio.today ?? data.portfolio.dayPL,data.portfolio.todayDelta ?? null),
    performance: () => metric('performance',translate('totalPL'),data.portfolio.total ?? data.portfolio.totalPL,data.portfolio.totalDelta ?? null),
    positions: () => `<section class="widget" data-widget="positions"><div class="widget-head"><h3>${safeText(translate('positions'))}</h3>${widgetMenu('positions')}</div><div class="table">${positionsMarkup()}</div></section>`,
    trades: () => `<section class="widget" data-widget="trades"><div class="widget-head"><h3>${safeText(translate('recentTrades'))}</h3>${widgetMenu('trades')}</div><div class="table">${tradesMarkup()}</div></section>`,
    summary: () => `<section class="widget ai-summary" data-widget="summary"><div class="widget-head"><h3>${safeText(translate('quickSummary'))}</h3>${widgetMenu('summary')}</div><div class="falcon-mini">✦</div><p>${safeText(local('الخلاصة المختصرة تظهر هنا من إسقاط التحليل القادم من FSATS، ولا ينشئ Web تحليلًا بديلًا.','The short FSATS analysis projection appears here; Web does not create substitute analysis.'))}</p><button class="primary" data-nav="ai">${safeText(translate('askFalcon'))}</button></section>`,
    alerts: () => `<section class="widget" data-widget="alerts"><div class="widget-head"><h3>${safeText(translate('notifications'))}</h3>${widgetMenu('alerts')}</div>${data.alerts.length ? data.alerts.map(a=>`<div class="alert-item"><span>•</span><p>${safeText(a)}</p></div>`).join('') : `<div class="empty-projection">${safeText(local('لا توجد تنبيهات متاحة','No alerts available'))}</div>`}</section>`
  };

  function dashboardPage() {
    const prefs = store.layout();
    const visible = prefs.order.filter(id => !prefs.hidden.includes(id));
    const previewNotice = previewMode
      ? `<p>${safeText(local('بيانات العرض التجريبي موضحة صراحة ولا تمثل حقيقة مالية حية.','Preview data is explicitly marked and does not represent live financial truth.'))}</p>`
      : '';
    const restore = prefs.hidden.length
      ? `<div class="restore-panel"><b>${safeText(translate('manageWidgets'))}</b>${prefs.hidden.map(id=>`<button data-show="${safeText(id)}">${safeText(translate('restore'))}: ${safeText(id)}</button>`).join('')}</div>`
      : '';

    return renderWorkspace(`<div class="dashboard-head"><div><h1>${safeText(translate('dashboard'))}</h1>${previewNotice}</div><div class="dashboard-tools"><button class="secondary" data-manage>${safeText(translate('manageWidgets'))}</button><button class="ghost" data-reset>${safeText(translate('resetLayout'))}</button></div></div><div class="dashboard-grid" id="dashboard-grid">${visible.map(id=>widgets[id]?.()||'').join('')}</div>${restore}`,'trader');
  }

  return Object.freeze({ dashboardPage });
}
