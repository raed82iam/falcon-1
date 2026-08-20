import { demoBadge, icon } from '../design-system/presentation.js';

const FALCON_BRAND_LOGO = './src/assets/falcon-brand-owner-reference.jpg';
const brandLogo = () => `<img class="falcon-brand-logo" src="${FALCON_BRAND_LOGO}" alt="" aria-hidden="true">`;

const USER_ITEMS = Object.freeze([
  ['trader','home','dashboard'],
  ['markets','market','markets'],
  ['advisory-markets','market','__ADVISORY_MARKETS__'],
  ['portfolio','portfolio','portfolio'],
  ['activity','activity','activity'],
  ['ai','ai','falconAI'],
  ['notifications','bell','notifications'],
  ['settings','gear','settings']
]);

const OWNER_ITEMS = Object.freeze([
  ['owner','home','systemOverview'],
  ['owner-apps','apps','applications'],
  ['owner-incidents','warning','incidents'],
  ['owner-approvals','shield','approvals'],
  ['owner-ai-emergency','warning','__AI_EMERGENCY__'],
  ['owner-provider-actions','market','__PROVIDER_ACTIONS__'],
  ['owner-users','user','users'],
  ['owner-audit','audit','audit'],
  ['owner-settings','gear','settings']
]);

function requireFunction(value, name) {
  if (typeof value !== 'function') throw new TypeError(`${name} must be a function`);
  return value;
}

export function createShell({ t, language, demoLabel } = {}) {
  const translate = requireFunction(t, 't');
  const currentLanguage = requireFunction(language, 'language');
  const currentDemoLabel = requireFunction(demoLabel, 'demoLabel');

  const languageAction = () => currentLanguage() === 'ar' ? 'English' : 'العربية';
  const navigationLabel = () => currentLanguage() === 'ar' ? 'التنقل الرئيسي' : 'Primary navigation';
  const mobileMenuLabel = () => currentLanguage() === 'ar' ? 'قائمة التنقل' : 'Navigation menu';
  const skipLabel = () => currentLanguage() === 'ar' ? 'تخطي إلى المحتوى الرئيسي' : 'Skip to main content';
  const publicLabel = (ar,en) => currentLanguage() === 'ar' ? ar : en;
  const navLabel = key => key === '__ADVISORY_MARKETS__'
    ? (currentLanguage() === 'ar' ? 'الأسواق الاستشارية' : 'Advisory Markets')
    : key === '__PROVIDER_ACTIONS__'
      ? (currentLanguage() === 'ar' ? 'إجراءات المزود' : 'Provider Actions')
      : key === '__AI_EMERGENCY__'
        ? (currentLanguage() === 'ar' ? 'طوارئ AI' : 'AI Emergency')
        : translate(key);

  const skipLink = () => `<a class="skip-link" href="#main">${skipLabel()}</a>`;

  const navigationButtons = (active, owner) => (owner ? OWNER_ITEMS : USER_ITEMS)
    .map(([route, iconName, labelKey]) => `<button type="button" data-nav="${route}" class="${active===route?'active':''}"${active===route?' aria-current="page"':''}>${icon(iconName)}<span>${navLabel(labelKey)}</span></button>`)
    .join('');

  const publicBrand = isFsats => {
    const text = `<span class="brand-copy"><b>${isFsats?'FALCON':'FALCON OS'}</b><small>${isFsats?'FSATS':publicLabel('نظام تشغيل ذكي متعدد التطبيقات','Intelligent multi-application OS')}</small></span>`;
    return currentLanguage() === 'ar' ? `${text}${brandLogo()}` : `${brandLogo()}${text}`;
  };

  function publicShell(content, product = 'falcon') {
    const isFsats = product === 'fsats';
    const publicNav = isFsats
      ? `<button type="button" data-nav="home">${translate('home')}</button><button type="button" data-nav="apps">${translate('applications')}</button>`
      : `<button type="button" class="active-public" data-nav="home">${translate('home')}</button><button type="button" data-nav="apps">${translate('applications')}</button><span>${publicLabel('المميزات','Features')}</span><span>${publicLabel('الأسعار','Pricing')}</span><span>${publicLabel('الشركاء','Partners')}</span><span>${publicLabel('الموارد','Resources')}</span><span>${publicLabel('من نحن','About us')}</span><span>${publicLabel('تواصل معنا','Contact')}</span>`;
    const mobilePublicNav = `<details class="public-mobile-menu"><summary aria-label="${mobileMenuLabel()}">☰</summary><nav aria-label="${navigationLabel()}">${publicNav}</nav></details>`;
    return `<div class="${isFsats?'':'public-landing'}">${skipLink()}<header class="topbar"><button type="button" class="brand" data-nav="home">${publicBrand(isFsats)}</button><nav class="topnav" aria-label="${navigationLabel()}">${publicNav}</nav>${mobilePublicNav}<div class="top-actions"><button type="button" data-language>${languageAction()}</button>${isFsats?'':`<button type="button" class="primary" data-nav="apps">${publicLabel('ابدأ الآن','Start now')} ✧</button>`}<button type="button" class="ghost" data-nav="login">${translate('signIn')} 🔒</button></div></header><main id="main" tabindex="-1">${content}</main></div>`;
  }

  function sidebar(active, owner = false) {
    const navigation = navigationButtons(active, owner);
    const footer = owner
      ? `<div class="owner-note">${translate('noTradingOwner')}</div>`
      : `<div class="ai-card"><b>${translate('falconAI')}</b><p>${translate('quickSummary')}</p><button type="button" data-nav="ai">${translate('askFalcon')}</button></div>`;
    return `<aside class="sidebar"><div class="side-brand">${brandLogo()}<b>${owner?'FALCON':'FSATS'}</b></div><nav aria-label="${navigationLabel()}">${navigation}</nav>${footer}</aside>`;
  }

  function mobileNavigation(active, owner) {
    return `<details class="mobile-menu"><summary aria-label="${mobileMenuLabel()}">☰</summary><nav aria-label="${navigationLabel()}">${navigationButtons(active,owner)}</nav></details>`;
  }

  function workspace(content, active = 'trader', owner = false) {
    const destination = owner ? 'owner-apps' : 'my-apps';
    const title = owner ? translate('ownerCenter') : 'FSATS';
    const subtitle = owner ? translate('systemOverview') : translate('applications');
    const attentionRoute = owner ? 'owner-incidents' : 'notifications';
    const attentionLabel = owner ? translate('incidents') : translate('notifications');
    return `<div class="workspace">${skipLink()}${sidebar(active, owner)}<div class="workspace-main"><header class="workspace-top">${mobileNavigation(active,owner)}<button type="button" class="app-switcher" data-nav="${destination}"><b>${title} ▾</b><small>${subtitle}</small></button><div class="top-actions"><button type="button" data-language>${languageAction()}</button><button type="button" data-nav="${attentionRoute}" aria-label="${attentionLabel}">${icon(owner?'warning':'bell')}</button><span class="avatar" aria-hidden="true">${owner?'O':'U'}</span></div></header>${demoBadge(currentDemoLabel())}<main id="main" tabindex="-1">${content}</main></div></div>`;
  }

  return Object.freeze({ publicShell, sidebar, workspace });
}
