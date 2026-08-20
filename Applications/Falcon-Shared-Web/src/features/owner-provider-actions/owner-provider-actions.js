import { safeText } from '../../security/safe-html.js';
import { StatusTone, statusBadge, notice, disabledControlAttributes } from '../../design-system/primitives.js';

function requireFunction(value, name) {
  if (typeof value !== 'function') throw new TypeError(`${name} must be a function`);
  return value;
}

function normalizeAction(raw = {}) {
  return Object.freeze({
    requestId:String(raw.requestId ?? ''),
    marketId:String(raw.marketId ?? ''),
    providerId:String(raw.providerId ?? ''),
    providerDisplayName:String(raw.providerDisplayName ?? raw.providerId ?? ''),
    actionType:String(raw.actionType ?? 'UNKNOWN'),
    message:String(raw.message ?? ''),
    reason:String(raw.reason ?? ''),
    credentialType:String(raw.credentialType ?? ''),
    providerCostClass:String(raw.providerCostClass ?? ''),
    status:String(raw.status ?? 'UNAVAILABLE'),
    secureEntryRequired:raw.secureEntryRequired === true,
    chatEntryProhibited:raw.chatEntryProhibited !== false,
    providerHelpOrSignupUrl:String(raw.providerHelpOrSignupUrl ?? ''),
    providerHelpOrSignupUrlValidation:String(raw.providerHelpOrSignupUrlValidation ?? 'UNVALIDATED'),
    preview:raw.preview === true
  });
}

function statusTone(status) {
  const value=String(status ?? '').toUpperCase();
  if (['READY','CURRENT','COMPLETED'].includes(value)) return StatusTone.POSITIVE;
  if (['ACTION_REQUIRED','PENDING','NEEDS_ATTENTION'].includes(value)) return StatusTone.WARNING;
  if (['ERROR','FAILED','REJECTED','REVOKED'].includes(value)) return StatusTone.NEGATIVE;
  if (['UNAVAILABLE','UNKNOWN'].includes(value)) return StatusTone.UNAVAILABLE;
  return StatusTone.NEUTRAL;
}

/**
 * Owner-only provider action presentation for FCR-0220.
 *
 * It intentionally contains no plaintext credential input. A future secure-entry
 * destination may be enabled only after a governed runtime/storage mechanism and
 * destination validation exist. ACTION_REQUIRED is a request for Owner attention,
 * not provider connectivity authority.
 */
export function createOwnerProviderActionsFeature({ t, language, workspace, actions = [] } = {}) {
  requireFunction(t, 't');
  const currentLanguage = requireFunction(language, 'language');
  const renderWorkspace = requireFunction(workspace, 'workspace');
  const local = (ar,en) => currentLanguage() === 'ar' ? ar : en;
  const normalized = Array.isArray(actions) ? actions.map(normalizeAction) : [];

  function actionCard(action) {
    const helpUrlTrusted = action.providerHelpOrSignupUrlValidation === 'VALIDATED';
    const canNavigateSecureEntry = false;
    const secureEntryReason=local('المسار الآمن التشغيلي غير مفعّل بعد.','The governed secure runtime entry path is not active yet.');
    const providerUrlReason=local('رابط المزود غير موثّق بعد.','Provider URL is not validated yet.');
    return `<article class="widget provider-action-card">
      <div class="widget-head"><div><h3>${safeText(action.providerDisplayName || local('مزود غير معروف','Unknown provider'))}</h3><small>${safeText(action.marketId)}</small></div>${statusBadge({label:action.status,tone:statusTone(action.status)})}</div>
      <p>${safeText(action.message || local('مطلوب إجراء من الـOwner لإكمال إعداد المزود المجاني.','Owner action is required to complete free-provider setup.'))}</p>
      ${action.reason?`<p class="muted">${safeText(action.reason)}</p>`:''}
      <div class="truth-grid"><span><b>${safeText(local('نوع الإجراء','Action'))}</b>${safeText(action.actionType)}</span><span><b>${safeText(local('نوع الاعتماد','Credential'))}</b>${safeText(action.credentialType || '—')}</span><span><b>${safeText(local('التكلفة','Cost'))}</b>${safeText(action.providerCostClass || '—')}</span></div>
      ${notice({title:local('إدخال آمن فقط','Secure entry only'),body:local('ممنوع لصق API key أو أي secret في المحادثة أو payload عادي. المسار الآمن التشغيلي غير مفعّل بعد.','Do not paste an API key or any secret into chat or ordinary payloads. The governed secure runtime entry path is not active yet.'),tone:StatusTone.WARNING,role:'alert'})}
      <div class="hero-actions"><button class="primary"${disabledControlAttributes(!canNavigateSecureEntry,secureEntryReason)}>${safeText(local('إضافة الاعتماد بشكل آمن','Add credential securely'))}</button>${action.providerHelpOrSignupUrl?`<button class="secondary"${disabledControlAttributes(!helpUrlTrusted,providerUrlReason)}>${safeText(helpUrlTrusted?local('فتح صفحة المزود الموثقة','Open validated provider page'):local('رابط المزود غير موثّق بعد','Provider URL not validated'))}</button>`:''}</div>
      <p class="muted tiny">${safeText(local('توفير الاعتماد لا يعني أن الاتصال بالمزود أصبح مخولًا.','Providing a credential does not authorize provider connectivity.'))}</p>
      ${action.preview?`<p class="muted tiny">${safeText(local('بطاقة تجريبية للواجهة فقط.','Preview UI card only.'))}</p>`:''}
    </article>`;
  }

  function ownerProviderActionsPage() {
    const body = normalized.length
      ? `<div class="owner-app-list">${normalized.map(actionCard).join('')}</div>`
      : `<section class="widget page-widget"><div class="empty-state"><h2>${safeText(local('لا توجد إجراءات مزود مطلوبة','No provider actions required'))}</h2><p>${safeText(local('لن ينشئ Web طلب API key من عنده. يجب أن يأتي الطلب من الحالة الموثوقة.','Web will not invent an API-key request; it must come from governed state.'))}</p></div></section>`;
    return renderWorkspace(`<div class="page-head"><div><h1>${safeText(local('إجراءات المزود','Provider Actions'))}</h1><p class="muted">${safeText(local('سطح Owner منفصل عن تجربة المستخدم والدعم. لا توجد secrets في المحادثة.','Owner-only surface, separate from the customer and Support experience. No secrets in chat.'))}</p></div></div>${body}`,'owner-provider-actions',true);
  }

  return Object.freeze({ ownerProviderActionsPage });
}
