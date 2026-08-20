import { safeText } from '../../security/safe-html.js';
import {
  OwnerAiEmergencyAction,
  OwnerAiEmergencyOutcome,
  canPrepareOwnerAiEmergencySubmission,
  bindOwnerAiEmergencyDecision
} from '../../core/ports/owner-ai-emergency-port.js';

function requireFunction(value, name) {
  if (typeof value !== 'function') throw new TypeError(`${name} must be a function`);
  return value;
}

const list = value => Array.isArray(value) ? value : [];
const current = value => value === 'CURRENT';

/**
 * Owner-only FCR-0225 presentation.
 *
 * Shared Web shows authoritative target/blast-radius/outcome facts and may prepare
 * an Owner request when all prerequisites are current. It never presents a click
 * as authorization/completion and exposes no release/revival control.
 */
export function createOwnerAiEmergencyFeature({ t, language, workspace, session = null, model = {} } = {}) {
  requireFunction(t, 't');
  const currentLanguage = requireFunction(language, 'language');
  const renderWorkspace = requireFunction(workspace, 'workspace');
  const local = (ar,en) => currentLanguage() === 'ar' ? ar : en;

  const targets = list(model.targets);
  const selectedTarget = model.selectedTarget ?? null;
  const selectedAction = model.selectedAction ?? OwnerAiEmergencyAction.KILL;
  const blastRadius = model.blastRadius ?? null;
  let decision = null;
  let decisionRejected = false;
  if (model.decision !== null && model.decision !== undefined) {
    try { decision = bindOwnerAiEmergencyDecision(model.decision); }
    catch { decisionRejected = true; }
  }
  const transportAvailable = model.transportAvailable === true;
  const eligible = canPrepareOwnerAiEmergencySubmission({ session, target:selectedTarget, action:selectedAction, blastRadius });
  const canSubmit = transportAvailable && eligible.allowed;
  const isGlobal = selectedAction === OwnerAiEmergencyAction.GLOBAL_AI_KILL;

  function targetOptions() {
    if (!targets.length) return `<option value="">${safeText(local('قائمة أهداف AI الموثوقة غير متاحة','Authoritative AI target inventory unavailable'))}</option>`;
    return targets.map(target => `<option value="${safeText(target.id)}" ${selectedTarget?.id===target.id?'selected':''}>${safeText(target.id)} · ${safeText(target.scope ?? 'UNKNOWN')}</option>`).join('');
  }

  function actionOptions() {
    return Object.values(OwnerAiEmergencyAction).map(action => `<option value="${safeText(action)}" ${selectedAction===action?'selected':''}>${safeText(action)}</option>`).join('');
  }

  function blastRadiusMarkup() {
    if (!blastRadius || blastRadius.authoritative !== true || !current(blastRadius.freshness)) {
      return `<div class="attention high"><b>${safeText(local('نطاق التأثير غير موثوق حاليًا','Blast radius is not currently authoritative'))}</b><p>${safeText(local('الإرسال مقفول. Web لن يخمن الهدف ولن يوسّع النطاق.','Submission is locked. Web will neither guess the target nor widen scope.'))}</p></div>`;
    }
    return `<section class="widget emergency-blast"><div class="widget-head"><h3>${safeText(local('تأكيد نطاق التأثير','Blast-radius confirmation'))}</h3><span class="status-chip">CURRENT</span></div><div class="emergency-target-list">${list(blastRadius.targetIds).map(id=>`<span>${safeText(id)}</span>`).join('') || `<span>${safeText(local('لا يوجد AI متأثر موثق','No impacted AI authoritatively identified'))}</span>`}</div></section>`;
  }

  function decisionMarkup() {
    if (decisionRejected) return `<section class="widget emergency-outcome"><div class="attention high"><b>${safeText(local('تم حجب نتيجة غير صالحة','Malformed outcome suppressed'))}</b><p>${safeText(local('Web رفض عرض نتيجة Foundation غير متوافقة مع عقد Stage 13 WP-01.','Web refused to present an outcome that does not satisfy the Stage 13 WP-01 contract.'))}</p></div></section>`;
    if (!decision) return `<section class="widget emergency-outcome"><h3>${safeText(local('نتيجة Foundation','Foundation outcome'))}</h3><p class="muted">${safeText(local('لا توجد نتيجة موثوقة بعد. إرسال الطلب، قبوله، واكتماله حالات منفصلة.','No authoritative outcome is available yet. Request sent, action accepted, and action completed are separate states.'))}</p></section>`;
    const outcome = decision.outcome ?? OwnerAiEmergencyOutcome.UNKNOWN;
    return `<section class="widget emergency-outcome"><div class="widget-head"><h3>${safeText(local('نتيجة Foundation','Foundation outcome'))}</h3><span class="status-chip">${safeText(outcome)}</span></div><dl class="emergency-facts"><div><dt>${safeText(local('Request','Request'))}</dt><dd>${safeText(decision.requestId)}</dd></div><div><dt>${safeText(local('Correlation','Correlation'))}</dt><dd>${safeText(decision.correlationId)}</dd></div><div><dt>${safeText(local('السبب','Reason'))}</dt><dd>${safeText(decision.reason)}</dd></div><div><dt>${safeText(local('Safe Core','Safe Core'))}</dt><dd>${safeText(decision.safeCorePreserved===true?'PRESERVED':'NOT_CONFIRMED')}</dd></div><div><dt>${safeText(local('إطفاء Falcon','Falcon shutdown'))}</dt><dd>${safeText(decision.falconShutdownAuthorized===true?'AUTHORIZED':'NOT_AUTHORIZED')}</dd></div><div><dt>${safeText(local('Recovery قبل Release','Recovery before release'))}</dt><dd>${safeText(decision.releaseRequiresGovernedRecovery===true?'REQUIRED':'NOT_CONFIRMED')}</dd></div></dl>${decision.evidenceReference?`<p class="muted tiny">${safeText(decision.evidenceReference)}</p>`:''}${decision.incidentReference?`<p class="muted tiny">${safeText(decision.incidentReference)}</p>`:''}</section>`;
  }

  function page() {
    const globalNotice = isGlobal
      ? `<div class="attention high"><b>${safeText(local('GLOBAL AI KILL يوقف AI فقط','GLOBAL AI KILL targets AI only'))}</b><p>${safeText(local('هذا ليس إطفاء Falcon. Falcon Safe Core يبقى تشغيليًا، بما فيه تحكم المالك وAI Kill Control والأدلة والبنية اللازمة للتعافي.','This is not a Falcon shutdown. Falcon Safe Core remains operational, including Owner control, AI Kill Control, audit evidence, and recovery infrastructure.'))}</p></div>`
      : `<div class="truth-note">${safeText(local('الطلب المستهدف يبقى على الهوية والنوع المحددين. الهدف الغامض أو غير الموجود لا يتحول تلقائيًا إلى نطاق أوسع.','A targeted request stays on the exact identity/type supplied. An ambiguous or missing target never widens automatically.'))}</div>`;
    const unavailable = !transportAvailable
      ? `<div class="attention"><b>${safeText(local('Runtime binding غير متاح','Runtime binding unavailable'))}</b><p>${safeText(local('واجهة الطوارئ جاهزة للعرض والتحقق، لكن Web لا يملك endpoint مخولًا ولن يخترع واحدًا.','The emergency surface is ready for presentation/validation, but Web has no authorized endpoint and will not invent one.'))}</p></div>`
      : '';

    return renderWorkspace(`<div class="page-head"><div><h1>${safeText(local('التحكم الطارئ بالـAI','AI Emergency Control'))}</h1><p class="muted">${safeText(local('واجهة Owner لإرسال طلبات إلى Foundation Kill Control Plane. الواجهة ليست سلطة Kill.','Owner surface for requests to the Foundation Kill Control Plane. The UI is not Kill authority.'))}</p></div><span class="status-chip">OWNER ONLY</span></div>${unavailable}<div class="owner-ai-emergency-grid"><section class="widget emergency-request"><div class="widget-head"><h3>${safeText(local('طلب طارئ','Emergency request'))}</h3><span class="status-chip">${safeText(selectedTarget?.freshness ?? 'UNAVAILABLE')}</span></div><label>${safeText(local('الهدف الموثوق','Authoritative target'))}<select data-ai-emergency-target ${targets.length?'':'disabled aria-disabled="true"'}>${targetOptions()}</select></label><label>${safeText(local('الإجراء','Action'))}<select data-ai-emergency-action>${actionOptions()}</select></label>${globalNotice}<button class="primary" data-ai-emergency-submit ${canSubmit?'':'disabled aria-disabled="true"'}>${safeText(local('إرسال الطلب إلى Foundation','Submit request to Foundation'))}</button><p class="muted tiny">${safeText(canSubmit ? local('جاهز لإرسال طلب فقط. القبول والتنفيذ يحتاجان نتيجة Foundation.','Ready to submit a request only. Acceptance and execution require Foundation outcome.') : local(`الإرسال مقفول: ${eligible.reasonCode ?? 'RUNTIME_BINDING_UNAVAILABLE'}`,`Submission locked: ${eligible.reasonCode ?? 'RUNTIME_BINDING_UNAVAILABLE'}`))}</p></section>${blastRadiusMarkup()}${decisionMarkup()}</div><section class="widget page-widget"><div class="attention"><b>REQUEST_SENT ≠ ACTION_ACCEPTED ≠ ACTION_COMPLETED</b><p>${safeText(local('لا يوجد زر Release أو Revival هنا. أي إعادة ثقة أو Release تحتاج مسار Recovery محكوم مستقل.','There is no Release or Revival control here. Trust restoration or release requires a separate governed recovery path.'))}</p></div><p class="muted tiny">WEB_UI != KILL_AUTHORITY · UI_CLICK != AUTHORIZATION · WEB_CANNOT_RELEASE_KILLED_AI</p></section>`,'owner-ai-emergency',true);
  }

  return Object.freeze({ page });
}
