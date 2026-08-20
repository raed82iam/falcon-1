import { displayText } from '../../design-system/presentation.js';

function requireFunction(value, name) {
  if (typeof value !== 'function') throw new TypeError(`${name} must be a function`);
  return value;
}

function normalizeApps(apps) {
  if (!Array.isArray(apps)) throw new TypeError('apps must be an array');
  return apps.map(app => Object.freeze({
    id: String(app?.id ?? ''),
    name: String(app?.name ?? ''),
    kind: String(app?.kind ?? ''),
    shortName: String(app?.shortName ?? '')
  }));
}

const FUTURE_SYSTEMS = Object.freeze([
  Object.freeze({
    id:'future-accounting',
    ar:'نظام المحاسبة المستقبلي',
    en:'Future Accounting System',
    arDesc:'عائلة نظام مستقبلية تحت Falcon OS. ليست نظامًا تشغيليًا متاحًا حاليًا.',
    enDesc:'A future system family under Falcon OS. It is not currently an operationally available system.'
  }),
  Object.freeze({
    id:'future-warehouse',
    ar:'نظام المستودعات المستقبلي',
    en:'Future Warehouse System',
    arDesc:'عائلة نظام مستقبلية تحت Falcon OS. لا يوجد رابط تشغيل أو صلاحية استخدام حالية.',
    enDesc:'A future system family under Falcon OS. No current runtime link or use authority is implied.'
  }),
  Object.freeze({
    id:'future-other',
    ar:'أنظمة Falcon مستقبلية أخرى',
    en:'Other Future Falcon Systems',
    arDesc:'Falcon مصمم لاستقبال أنظمة مستقلة إضافية مستقبلًا بدون جعلها تطبيقات داخل FSATS.',
    enDesc:'Falcon is designed to host additional independent future systems without making them children of FSATS.'
  })
]);

const childPresentation = Object.freeze({
  fsata:{ icon:'⌁', ar:'تطبيق التداول الذكي', en:'Self-Aware Trading', arDesc:'مساحة التداول والتحليل واتخاذ القرار ضمن حدود FSATS.', enDesc:'Trading, analysis and decision experience within FSATS boundaries.' },
  fsapma:{ icon:'◎', ar:'إدارة المزودين', en:'Provider Management', arDesc:'إدارة مصادر البيانات والمزودين مع فصل واضح عن بيانات العرض.', enDesc:'Provider and data-source management with strict presentation-data separation.' },
  ftga:{ icon:'◇', ar:'حارس التداول', en:'Trading Guardian', arDesc:'حماية تشغيل التداول ومراقبة الحالات الحرجة دون امتلاك منطق الاستراتيجية.', enDesc:'Trading protection and critical-state supervision without owning strategy logic.' },
  fstsim:{ icon:'◫', ar:'المحاكاة والتجربة', en:'Trading Simulation', arDesc:'اختبار السيناريوهات في بيئة غير حية قبل أي انتقال تشغيلي.', enDesc:'Scenario validation in non-Live operation before any runtime progression.' },
  'app-rsc':{ icon:'↻', ar:'إدارة موارد FSATS', en:'FSATS Resource Management', arDesc:'تنسيق موارد تطبيقات FSATS ضمن الصلاحيات والحدود المعتمدة.', enDesc:'Coordinates FSATS Application resources within governed limits.' }
});

/**
 * Falcon-wide public presentation.
 *
 * The visual composition follows the Project Owner's accepted public landing
 * reference while preserving the current governed Falcon/FSATS hierarchy.
 */
export function createFalconPublicFeature({ t, language, publicShell, apps, fsatsApps = [] } = {}) {
  const translate = requireFunction(t, 't');
  const currentLanguage = requireFunction(language, 'language');
  const shell = requireFunction(publicShell, 'publicShell');
  const catalog = normalizeApps(apps);
  const tradingChildren = normalizeApps(fsatsApps);
  const local = (ar,en) => currentLanguage() === 'ar' ? ar : en;

  const description = app => translate(app.kind === 'trading' ? 'tradingDesc' : `${app.kind}Desc`);

  function appCard(app, compact = false) {
    const isTrading = app.kind === 'trading';
    const icon = compact ? '' : `<div class="app-icon">${isTrading ? '▥' : '◈'}</div>`;
    const action = isTrading
      ? `<button type="button" class="${compact ? 'primary' : 'link'}" data-nav="fsats">${translate('discover')}${compact ? '' : ' ←'}</button>`
      : compact
        ? `<button type="button" class="secondary" disabled aria-disabled="true">${translate('coming')}</button>`
        : `<span class="status-chip">${translate('coming')}</span>`;

    return `<article class="app-card ${isTrading ? 'featured' : ''}">${icon}<h3>${displayText(app.name)}</h3><p>${description(app)}</p>${action}</article>`;
  }

  function futureSystemCard(system) {
    const futureLabel = local('مستقبلي • غير تشغيلي','Future • Not operational');
    return `<article class="app-card future-system-card" data-future-system="${displayText(system.id)}"><span class="status-chip">${displayText(futureLabel)}</span><h3>${displayText(local(system.ar,system.en))}</h3><p>${displayText(local(system.arDesc,system.enDesc))}</p><button type="button" class="secondary" disabled aria-disabled="true">${displayText(local('غير متاح حاليًا','Not currently available'))}</button></article>`;
  }

  function hierarchyNote() {
    const childNames = tradingChildren.map(app => displayText(app.shortName || app.name)).join(' • ');
    const text = local(
      `FSATS هو نظام التداول الحالي داخل Falcon OS، وتطبيقاته الداخلية هي: ${childNames}. التفاصيل والاستكشاف موجودة داخل صفحة FSATS.`,
      `FSATS is the current Trading system inside Falcon OS. Its internal Applications are: ${childNames}. Their explanations and discovery experience live inside the FSATS page.`
    );
    return `<aside class="hierarchy-note"><strong>FALCON OS → FSATS</strong><p>${text}</p></aside>`;
  }

  function tradingChildCard(app) {
    const profile = childPresentation[app.id] ?? { icon:'◈', ar:app.shortName || app.name, en:app.shortName || app.name, arDesc:'تطبيق مستقل داخل FSATS.', enDesc:'Independent Application inside FSATS.' };
    return `<article class="trading-app-card"><div class="mini-icon" aria-hidden="true">${profile.icon}</div><h3>${displayText(local(profile.ar,profile.en))}</h3><small>${displayText(app.shortName || app.name)}</small><p>${displayText(local(profile.arDesc,profile.enDesc))}</p><button type="button" class="link" data-nav="fsats">${translate('discover')} ←</button></article>`;
  }

  function publicHome() {
    const trading = catalog.find(app => app.kind === 'trading') ?? catalog[0];
    const tradingTitle = trading ? displayText(local('التداول FSATS','FSATS Trading')) : 'FSATS';
    return shell(`
      <section class="hero">
        <div class="hero-copy">
          <span class="eyebrow">FALCON OS</span>
          <h1>${displayText(local('فالكون OS','Falcon OS'))}<em>${displayText(local('نظام تشغيل ذكي متعدد التطبيقات','An intelligent multi-application operating system'))}</em></h1>
          <p>${displayText(local('منصة متكاملة تجمع التطبيقات المتخصصة والذكاء المنضبط وإدارة المخاطر وإدارة المزودين والتواصل ضمن نظام واحد آمن ومرن.','An integrated platform that brings specialized Applications, disciplined intelligence, risk management, provider management and communication into one secure, flexible operating system.'))}</p>
          <div class="hero-feature-pills" aria-label="${displayText(local('خصائص فالكون','Falcon characteristics'))}">
            <span><b>♢</b>${displayText(local('آمن','Secure'))}</span><span><b>◉</b>${displayText(local('ذكي','Intelligent'))}</span><span><b>⌘</b>${displayText(local('متكامل','Integrated'))}</span><span><b>↗</b>${displayText(local('قابل للتطوير','Extensible'))}</span>
          </div>
          <div class="hero-actions"><button type="button" class="primary" data-nav="apps">${displayText(local('ابدأ الآن','Start now'))} ✧</button><button type="button" class="secondary" data-nav="login">${translate('signIn')} 🔒</button><button type="button" class="secondary" data-nav="register">${translate('createAccount')}</button></div>
          <div class="hero-note">${displayText(local('واجهة استكشاف عامة، ولا تعني تشغيلًا حيًا أو صلاحية تداول.','Public discovery surface only. It does not imply Live operation or trading authority.'))}</div>
        </div>
        <div class="falcon-visual" aria-label="${displayText(local('صقر فالكون','Falcon artwork'))}">
          <div class="falcon-image-mask"><img src="./src/assets/falcon-shared-visual.jpg" alt="${displayText(local('صقر فالكون OS','Falcon OS falcon'))}"></div>
          <div class="hero-side-features" aria-label="${displayText(local('مزايا فالكون','Falcon advantages'))}">
            <div class="hero-feature-point"><span class="feature-dot" aria-hidden="true"></span><div><h3>${displayText(local('نظام واحد','One system'))}</h3><p>${displayText(local('تجربة موحدة لتطبيقات متعددة.','A unified experience across multiple Applications.'))}</p></div></div>
            <div class="hero-feature-point"><span class="feature-dot" aria-hidden="true"></span><div><h3>${displayText(local('ذكاء منضبط','Disciplined intelligence'))}</h3><p>${displayText(local('ذكاء وتحليل داخل حدود الصلاحيات.','Intelligence and analysis inside governed authority.'))}</p></div></div>
            <div class="hero-feature-point"><span class="feature-dot" aria-hidden="true"></span><div><h3>${displayText(local('حوكمة وأمان','Governance & security'))}</h3><p>${displayText(local('فصل واضح بين العرض والحقيقة والصلاحية.','Clear separation of presentation, truth and authority.'))}</p></div></div>
          </div>
        </div>
      </section>
      <section class="landing-section">
        <div class="landing-section-title"><h2>${displayText(local('تطبيقات فالكون OS','Falcon OS Applications'))}</h2><small>FALCON OS APPLICATIONS</small></div>
        <div class="os-app-row"><article class="os-app-card"><div class="app-icon">▥</div><h3>${tradingTitle}</h3><small>FSATS Trading</small><p>${displayText(local('نظام التداول الحالي داخل Falcon OS، ويضم تطبيقاته المتخصصة ضمن حدود واضحة.','The current Trading system inside Falcon OS, containing its specialized Applications within explicit boundaries.'))}</p><button type="button" class="link" data-nav="fsats">${translate('discover')} ←</button></article></div>
      </section>
      <section class="landing-section future-systems" aria-labelledby="future-systems-title">
        <div class="landing-section-title"><h2 id="future-systems-title">${displayText(local('أنظمة Falcon المستقبلية','Future Falcon Systems'))}</h2><small>${displayText(local('للتعريف فقط • غير تشغيلية','Discovery only • Not operational'))}</small></div>
        <div class="apps-grid">${FUTURE_SYSTEMS.map(futureSystemCard).join('')}</div>
      </section>
      <section class="landing-section trading-system">
        <div class="landing-section-title"><h2>${displayText(local('تطبيقات نظام التداول FSATS','FSATS Trading System Applications'))}</h2></div>
        <div class="trading-app-grid">${tradingChildren.map(tradingChildCard).join('')}</div>
      </section>
      <section class="landing-proof-strip" aria-label="${displayText(local('مبادئ فالكون','Falcon principles'))}">
        <div class="proof-item"><div class="proof-icon">◇</div><div><h4>${displayText(local('فصل الصلاحيات','Authority separation'))}</h4><p>${displayText(local('العرض لا يصبح سلطة تنفيذ.','Presentation never becomes execution authority.'))}</p></div></div>
        <div class="proof-item"><div class="proof-icon">≋</div><div><h4>${displayText(local('حقيقة موثوقة','Truth-aware'))}</h4><p>${displayText(local('الحالة غير المتوفرة لا تتحول إلى قيمة مختلقة.','Unavailable truth is never fabricated.'))}</p></div></div>
        <div class="proof-item"><div class="proof-icon">⌘</div><div><h4>${displayText(local('معمارية متعددة التطبيقات','Multi-application architecture'))}</h4><p>${displayText(local('كل تطبيق يبقى مستقلًا في مسؤوليته.','Each Application retains its own responsibility.'))}</p></div></div>
        <div class="proof-item"><div class="proof-icon">↗</div><div><h4>${displayText(local('قابل للتوسع','Extensible'))}</h4><p>${displayText(local('إضافة تطبيقات جديدة دون كسر الحدود القائمة.','New Applications can be added without collapsing existing boundaries.'))}</p></div></div>
      </section>
      ${hierarchyNote()}
    `);
  }

  function applicationsPage() {
    return shell(`<section class="section page"><div class="section-head"><div><h1>${translate('appsTitle')}</h1><p>${local('الأنظمة الحالية تظهر في هذا المستوى. تطبيقات كل نظام تبقى داخله.','Current systems appear at this level. Each system keeps its internal Applications inside its own product page.')}</p></div></div><div class="apps-grid">${catalog.map(app => appCard(app, true)).join('')}</div><div class="section-head future-system-heading"><div><h2>${displayText(local('أنظمة مستقبلية','Future systems'))}</h2><p>${displayText(local('هذه عائلات مستقبلية فقط ولا تمثل أنظمة تشغيلية أو اشتراكات متاحة الآن.','These are future families only and do not represent operational systems or subscriptions available today.'))}</p></div></div><div class="apps-grid">${FUTURE_SYSTEMS.map(futureSystemCard).join('')}</div>${hierarchyNote()}</section>`);
  }

  return Object.freeze({ publicHome, applicationsPage });
}
