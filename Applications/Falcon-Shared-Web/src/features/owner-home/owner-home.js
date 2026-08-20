import { safeText } from '../../security/safe-html.js';
import { StatusTone, notice, disabledControlAttributes } from '../../design-system/primitives.js';
import { isAuthoritativeSession, hasVerifiedOwnerFsatsFeatureAccess } from '../../auth.js';

function requireFunction(value,name) {
  if (typeof value !== 'function') throw new TypeError(`${name} must be a function`);
  return value;
}

/**
 * Falcon-wide Project Owner landing surface.
 *
 * Navigation and attention only. It is not Command Center, FSATS, or an
 * authority engine. Destination visibility never authorizes actions.
 */
export function createOwnerHomeFeature({ language, session, ownerFsatsAccess = null } = {}) {
  const currentLanguage=requireFunction(language,'language');
  const currentSession=requireFunction(session,'session');
  const local=(ar,en)=>currentLanguage()==='ar'?ar:en;

  function ownerHome() {
    const s=currentSession();
    const authoritative=Boolean(isAuthoritativeSession(s) && s.role==='PROJECT_OWNER' && Array.isArray(s.surfaceGrants) && s.surfaceGrants.includes('OWNER'));
    const fsatsAuthoritative=hasVerifiedOwnerFsatsFeatureAccess(ownerFsatsAccess);
    const fsatsReason=local('صلاحية ميزات FSATS الدائمة للمالك غير متاحة بدون قرار entitlement حالي وموثق من عقد FCR-0242.','Permanent Owner FSATS feature access remains unavailable without a current governed FCR-0242 entitlement decision.');

    return `<div class="owner-home-shell"><header class="workspace-top"><button type="button" class="brand" data-nav="owner-home"><b>FALCON OS</b><small>${safeText(local('بوابة المالك','Owner Home'))}</small></button><div class="top-actions"><button type="button" data-language>${currentLanguage()==='ar'?'English':'العربية'}</button><button type="button" data-nav="owner-incidents" aria-label="${safeText(local('تحتاج انتباه','Needs Your Attention'))}">△</button><span class="avatar" aria-hidden="true">O</span></div></header><main id="main" class="section page owner-home-page">
      <div class="owner-head"><div><span class="eyebrow">FALCON OWNER</span><h1>${safeText(local('الصفحة الرئيسية للمالك','Falcon Owner Home'))}</h1><p>${safeText(local('اختر الوجهة التي تريدها. هذه الصفحة للتنقل والانتباه فقط وليست مركز صلاحيات.','Choose where you want to go. This page is navigation and attention only, not an authority engine.'))}</p></div></div>
      ${!authoritative?notice({title:local('جلسة المالك غير موثقة','Owner session is not authoritative'),body:local('لن تُفتح وجهات المالك المحمية بدون جلسة وهوية وربط Web مخول.','Protected Owner destinations stay closed without an authoritative identity/session and Web access binding.'),tone:StatusTone.NEGATIVE,role:'alert'}):''}
      <section class="owner-home-destinations" aria-label="${safeText(local('وجهات المالك','Owner destinations'))}">
        <article class="widget owner-home-card"><span class="app-icon" aria-hidden="true">⌘</span><h2>${safeText(local('مركز قيادة المالك','Owner Command Center'))}</h2><p>${safeText(local('الإدارة والحوكمة، صحة النظام، الحوادث، الموافقات، الأدلة والطوارئ.','Management and governance, system health, incidents, approvals, evidence and emergency surfaces.'))}</p><button type="button" class="primary" data-nav="owner"${disabledControlAttributes(!authoritative,local('جلسة المالك المخولة مطلوبة.','Authoritative Owner session required.'))}>${safeText(local('فتح Command Center','Open Command Center'))}</button></article>
        <article class="widget owner-home-card"><span class="app-icon" aria-hidden="true">▥</span><h2>FSATS</h2><p>${safeText(local('نظام التداول وتجربة الميزات الخاصة بالمالك. Feature access لا يعني صلاحية تداول أو تنفيذ.','Trading system and Owner feature experience. Feature access is not trading or execution authority.'))}</p><button type="button" class="primary" data-nav="trader"${disabledControlAttributes(!authoritative || !fsatsAuthoritative,fsatsReason)}>${safeText(local('فتح FSATS','Open FSATS'))}</button><p class="muted tiny">${safeText(fsatsAuthoritative?local('Owner feature entitlement موثق وحالي.','Owner feature entitlement is current and governed.'):fsatsReason)}</p></article>
        <article class="widget owner-home-card"><span class="app-icon" aria-hidden="true">＋</span><h2>${safeText(local('أنظمة مستقبلية','Future systems'))}</h2><p>${safeText(local('تظهر هنا عند وجود نظام حقيقي وعقد وصول مخول.','They appear here only when a real system and governed access contract exist.'))}</p><button type="button" class="secondary" disabled aria-disabled="true">${safeText(local('غير متاح حاليًا','Not available yet'))}</button></article>
      </section>
      <section class="widget page-widget owner-home-attention"><div class="widget-head"><h2>${safeText(local('تحتاج انتباهك','Needs Your Attention'))}</h2></div><p class="muted">${safeText(local('هذه اختصارات عرض فقط. فتح البطاقة أو مشاهدتها لا يوافق على أي إجراء.','These are presentation shortcuts only. Opening or viewing an item does not approve any action.'))}</p><div class="hero-actions"><button type="button" class="secondary" data-nav="owner-approvals">${safeText(local('الموافقات والتحديثات','Approvals & Updates'))}</button><button type="button" class="secondary" data-nav="owner-incidents">${safeText(local('الحوادث','Incidents'))}</button><button type="button" class="secondary" data-nav="owner-provider-actions">${safeText(local('إجراءات المزود','Provider Actions'))}</button></div></section>
      <p class="muted tiny">OWNER_HOME = NAVIGATION_AND_ATTENTION_SURFACE · NAVIGATION_VISIBILITY ≠ ACTION_AUTHORIZATION</p>
    </main></div>`;
  }

  return Object.freeze({ownerHome});
}
