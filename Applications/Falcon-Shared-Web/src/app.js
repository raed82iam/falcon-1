import { createI18n } from './i18n.js';
import { store } from './state.js';
import { demo } from './data/demo.js';
import { canAccessRoute } from './auth.js';
import { safeText } from './security/safe-html.js';
import { demoBadge as renderDemoBadge } from './design-system/presentation.js';
import { createShell } from './composition/shell.js';
import { createIncidentUiRuntime } from './composition/incident-ui-runtime.js';
import { createAppViewRegistry, isCustomerWorkspaceRoute } from './composition/app-view-registry.js';
import { bindAppUi } from './composition/app-ui-bindings.js';
import { createOwnerSurfaces } from './composition/owner-surfaces.js';
import { createWebRuntimeBootstrap, readInjectedWebRuntimeBindings } from './composition/runtime-bootstrap.js';
import { normalizeRoute, routeHash } from './platform/navigation/routes.js';
import { createFalconPublicFeature } from './features/falcon-public/falcon-public.js';
import { createFsatsPublicFeature } from './features/fsats-public/fsats-public.js';
import { createMyApplicationsFeature } from './features/my-applications/my-applications.js';
import { createFsatsWorkspaceFeature } from './features/fsats-workspace/fsats-workspace.js';
import { createPortfolioFeature } from './features/portfolio/portfolio.js';
import { createActivityFeature } from './features/activity/activity.js';
import { createMarketsFeature } from './features/markets/markets.js';
import { createAdvisoryMarketsFeature } from './features/advisory-markets/advisory-markets.js';
import { createAiFeature } from './features/ai/ai.js';
import { createNotificationsFeature } from './features/notifications/notifications.js';
import { createSettingsFeature } from './features/settings/settings.js';
import { createCatalogPresentation } from './features/catalog/catalog-presentation.js';
import { createProfessionalTraderFeature } from './features/professional-trader/professional-trader.js';
import { createProfessionalOwnerFeature } from './features/professional-owner/professional-owner.js';

const root = document.querySelector('#app');
const runtimeBindings = readInjectedWebRuntimeBindings();
const runtimeBootstrap = createWebRuntimeBootstrap({ bindings:runtimeBindings, previewData:demo });
const auth = runtimeBootstrap.auth;
let authenticatedSession = null;
const dataSource = runtimeBootstrap.dataSource;
const data = dataSource.data;
const incidentRuntimePolicy = runtimeBootstrap.incidentPolicy;
const i18n = createI18n(store.language);
try {
  i18n.set(i18n.lang);
} catch {
  document.documentElement.lang = i18n.lang;
  document.documentElement.dir = i18n.lang === 'ar' ? 'rtl' : 'ltr';
}
const t = key => i18n.t(key);
const go = route => { location.hash = routeHash(route); };
const route = () => normalizeRoute(location.hash);
const demoBadge = () => dataSource.preview ? renderDemoBadge(t('demo')) : '';
const local = (ar,en) => i18n.lang === 'ar' ? ar : en;
const { publicShell, workspace } = createShell({
  t,
  language:() => i18n.lang,
  demoLabel:() => dataSource.preview ? t('demo') : ''
});

const { catalogMarkup } = createCatalogPresentation({
  t,
  language:() => i18n.lang,
  catalog:data.catalog
});

const { publicHome, applicationsPage } = createFalconPublicFeature({
  t,
  language:() => i18n.lang,
  publicShell,
  apps:data.apps,
  fsatsApps:data.fsatsApps
});
const { fsatsLanding } = createFsatsPublicFeature({
  t,
  language:() => i18n.lang,
  publicShell,
  fsatsApps:data.fsatsApps,
  icon:name => `<span class="icon" aria-hidden="true">${safeText(({ ai:'✦', shield:'◇', market:'◉' })[name] || '•')}</span>`
});
const { myApplicationsPage } = createMyApplicationsFeature({
  t,
  language:() => i18n.lang,
  publicShell,
  demoBadge,
  apps:data.apps,
  previewMode:dataSource.preview,
  applicationAccess:runtimeBootstrap.applicationAccess,
  subscriptionModel:runtimeBootstrap.subscriptionModel
});
createFsatsWorkspaceFeature({ t, language:() => i18n.lang, workspace, store, data, catalogMarkup, previewMode:dataSource.preview });
const { dashboardPage } = createProfessionalTraderFeature({ language:() => i18n.lang, workspace, data });
const { portfolioPage } = createPortfolioFeature({ t, workspace, data });
const { activityPage } = createActivityFeature({ t, workspace, data });
const { marketsPage } = createMarketsFeature({ t, language:() => i18n.lang, workspace, catalogMarkup });
const { advisoryMarketsPage } = createAdvisoryMarketsFeature({ t, language:() => i18n.lang, workspace, markets:data.advisoryMarkets });
const { aiPage } = createAiFeature({ t, language:() => i18n.lang, workspace, data });
const { notificationsPage } = createNotificationsFeature({ t, workspace, data });
const { settingsPage } = createSettingsFeature({ t, language:() => i18n.lang, workspace, localize:local });
const {
  ownerHome,
  owner:legacyOwner,
  ownerApps,
  ownerIncidents,
  ownerApprovals,
  ownerUsers,
  ownerAudit,
  ownerSettings,
  ownerSimulator,
  ownerProviderActionsPage,
  ownerAiEmergencyPage
} = createOwnerSurfaces({
  t,
  language:() => i18n.lang,
  workspace,
  data,
  session:() => authenticatedSession,
  ownerFsatsAccess:runtimeBootstrap.ownerFsatsAccess,
  ownerGovernanceModel:runtimeBootstrap.ownerGovernanceModel,
  ownerAiEmergencyModel:runtimeBootstrap.ownerAiEmergencyModel
});
void legacyOwner;
const { owner } = createProfessionalOwnerFeature({ language:() => i18n.lang, workspace, data });

function placeholder(name, ownerMode=false) {
  return workspace(`<div class="empty-state"><div>◈</div><h1>${safeText(name)}</h1><p>${safeText(local('تم إنشاء سطح الواجهة، والربط التشغيلي يبقى تابعًا للعقود والمصادر الموثوقة.','The UI surface exists; operational binding remains governed by authoritative contracts and sources.'))}</p></div>`,ownerMode?'owner':'trader',ownerMode);
}

const viewRegistry = createAppViewRegistry({
  publicHome,
  applicationsPage,
  fsatsLanding,
  myApplicationsPage,
  dashboardPage,
  portfolioPage,
  activityPage,
  marketsPage,
  advisoryMarketsPage,
  aiPage,
  notificationsPage,
  settingsPage,
  ownerHome,
  owner,
  ownerApps,
  ownerIncidents,
  ownerApprovals,
  ownerAiEmergencyPage,
  ownerProviderActionsPage,
  ownerUsers,
  ownerAudit,
  ownerSettings,
  ownerSimulator,
  placeholder
});

let incidentRuntime = null;

function render() {
  const currentRoute = route();
  if (!canAccessRoute(currentRoute, authenticatedSession, { ownerFsatsAccess:runtimeBootstrap.ownerFsatsAccess })) {
    root.innerHTML = fsatsLanding();
    bind();
    return;
  }
  const base = viewRegistry.viewFor(currentRoute)();
  root.innerHTML = base + (isCustomerWorkspaceRoute(currentRoute) ? incidentRuntime?.markup?.() ?? '' : '');
  bind();
  if (currentRoute === 'register') root.querySelector('.onboarding-section input')?.focus();
}

incidentRuntime = createIncidentUiRuntime({
  data,
  language:() => i18n.lang,
  localize:local,
  render,
  session:() => authenticatedSession,
  persistencePort:incidentRuntimePolicy.persistence.port,
  screenshotScanner:incidentRuntimePolicy.screenshotScanner,
  localVoiceRuntime:incidentRuntimePolicy.localVoiceRuntime,
  supportTransportPort:incidentRuntimePolicy.supportTransportPort
});

function bind() {
  bindAppUi({
    navigate:go,
    i18n,
    render,
    auth,
    setSession:value => { authenticatedSession = value; },
    store,
    incidentRuntime
  });
}

window.addEventListener('hashchange', render);
render();
void incidentRuntime.initialize();
