import { safeText } from '../../security/safe-html.js';
import { StatusTone, statusBadge, notice, disabledControlAttributes } from '../../design-system/primitives.js';
import { normalizeOwnerUpdateProposal } from '../../contracts/owner-update-governance-v1.js';
import { validateOwnerProposalIngress, evaluateOwnerProposalIngress } from '../../contracts/owner-update-proposal-boundary-v1.js';

function requireFunction(value,name) {
  if (typeof value !== 'function') throw new TypeError(`${name} must be a function`);
  return value;
}

function arr(value) { return Array.isArray(value) ? value : []; }

function toneForDisposition(value) {
  const text=String(value ?? '').toUpperCase();
  if (text.includes('REJECT') || text.includes('FAILED') || text.includes('REVOKED')) return StatusTone.NEGATIVE;
  if (text.includes('MANUAL') || text.includes('PENDING') || text.includes('REVIEW')) return StatusTone.WARNING;
  if (text.includes('UNAVAILABLE') || text.includes('UNKNOWN')) return StatusTone.UNAVAILABLE;
  if (text.includes('AUTO_ACCEPTED') || text === 'ACCEPTED' || text.includes('COMPLETED')) return StatusTone.POSITIVE;
  return StatusTone.NEUTRAL;
}

function isCurrentPolicy(policy) {
  if (!policy || typeof policy !== 'object') return false;
  if (policy.revoked === true || policy.isRevoked === true) return false;
  if (policy.current === false || policy.isCurrent === false) return false;
  const expiry=policy.expiry ?? policy.expiresAt ?? null;
  if (expiry) {
    const expiryMs=Date.parse(expiry);
    if (!Number.isFinite(expiryMs) || expiryMs <= Date.now()) return false;
  }
  return true;
}

/**
 * Project Owner update-governance surface.
 *
 * It presents governed proposal/policy/history snapshots and request controls.
 * It NEVER mints Owner authority locally. Auto-accept decisions and rollback
 * authorization must be returned from the Foundation-owned boundary.
 */
export function createOwnerUpdateGovernanceFeature({ language, workspace, model = {} } = {}) {
  const currentLanguage=requireFunction(language,'language');
  const renderWorkspace=requireFunction(workspace,'workspace');
  const local=(ar,en)=>currentLanguage()==='ar'?ar:en;

  const transportAvailable=model.transportAvailable===true;
  const policies=arr(model.policies);
  const proposals=arr(model.proposals);
  const history=arr(model.history);

  function policyCard(policy,index) {
    const currentPolicy=isCurrentPolicy(policy);
    const revoked=policy?.revoked===true || policy?.isRevoked===true;
    const status=revoked
      ? local('ملغاة','Revoked')
      : currentPolicy
        ? local('فعالة','Active')
        : local('غير حالية','Not current');
    const tone=revoked ? StatusTone.NEGATIVE : currentPolicy ? StatusTone.POSITIVE : StatusTone.UNAVAILABLE;
    const editEnabled=transportAvailable && currentPolicy;
    return `<article class="widget owner-policy-card" data-owner-policy="${index}">
      <div class="widget-head"><div><h3>${safeText(policy?.policyId ?? local('سياسة غير متوفرة','Unavailable policy'))}</h3><small>${safeText(policy?.policyVersion ?? '—')}</small></div>${statusBadge({label:status,tone})}</div>
      <p class="muted">${safeText(local('هذه السياسة لا تصبح صلاحية إلا من خلال العقد المحكوم الحالي للمالك.','This policy becomes authority only through the current governed Owner contract.'))}</p>
      <div class="truth-grid"><span><b>${safeText(local('أقصى مخاطرة','Risk ceiling'))}</b>${safeText(policy?.maximumRiskTier ?? '—')}</span><span><b>${safeText(local('تنتهي','Expires'))}</b>${safeText(policy?.expiry ?? policy?.expiresAt ?? '—')}</span><span><b>${safeText(local('الدليل','Evidence'))}</b>${safeText(policy?.evidenceReference ?? '—')}</span></div>
      <div class="hero-actions"><button class="secondary" data-owner-policy-edit="${index}"${disabledControlAttributes(!editEnabled,currentPolicy?local('ربط إدارة السياسة عبر FIL غير مفعل بعد.','Governed FIL policy-management transport is not active yet.'):local('السياسة ليست حالية ولا يمكن التعامل معها كسياسة فعالة.','The policy is not current and cannot be treated as active.'))}>${safeText(local('تعديل','Edit'))}</button><button class="secondary" data-owner-policy-revoke="${index}"${disabledControlAttributes(!editEnabled,currentPolicy?local('ربط إلغاء السياسة عبر FIL غير مفعل بعد.','Governed FIL policy-revocation transport is not active yet.'):local('السياسة ليست حالية ولا يمكن إلغاؤها كسياسة فعالة.','The policy is not current and cannot be revoked as an active policy.'))}>${safeText(local('إلغاء','Revoke'))}</button></div>
    </article>`;
  }

  function proposalCard(raw,index) {
    let checked;
    try { checked=validateOwnerProposalIngress(raw); }
    catch { checked={ valid:false, proposal:null, reason:'INVALID_PROPOSAL' }; }
    let display;
    try { display=checked.proposal ?? normalizeOwnerUpdateProposal(raw); }
    catch { display={ proposalId:'', owningApplicationIdentity:'', updateClass:'', proposalVersion:'', impact:'', rollbackPlan:{} }; }
    const policy=policies.find(item=>item?.policyId===raw?.standingPolicyId || item?.policyId===raw?.policyId) ?? null;
    let eligibility;
    try { eligibility=evaluateOwnerProposalIngress(raw,policy); }
    catch { eligibility={ disposition:'MANUAL_REVIEW_REQUIRED', reason:'INVALID_PROPOSAL_OR_POLICY' }; }
    const rollback=display.rollbackPlan ?? {};
    const autoEligible=checked.valid===true && eligibility.disposition==='STANDING_PREAPPROVAL_ELIGIBLE_FOR_OWNER_DECISION' && isCurrentPolicy(policy);
    const resultLabel=autoEligible
      ? local('مؤهل لفحص موافقة المالك المسبقة','Eligible for Owner standing-policy decision')
      : local('مراجعة المالك مطلوبة','Manual Owner review required');
    return `<article class="widget owner-proposal-card" data-owner-proposal="${index}">
      <div class="widget-head"><div><h3>${safeText(display.proposalId || local('Proposal غير صالح','Invalid proposal'))}</h3><small>${safeText(display.owningApplicationIdentity || '—')}</small></div>${statusBadge({label:resultLabel,tone:autoEligible?StatusTone.WARNING:StatusTone.NEUTRAL})}</div>
      <div class="truth-grid"><span><b>${safeText(local('التصنيف','Class'))}</b>${safeText(display.updateClass || '—')}</span><span><b>${safeText(local('الإصدار','Version'))}</b>${safeText(display.proposalVersion || '—')}</span><span><b>${safeText(local('الأثر','Impact'))}</b>${safeText(display.impact || '—')}</span></div>
      ${notice({title:local('خطة الرجوع إلزامية','Backup / rollback plan required'),body:checked.valid?local(`الخطة: ${rollback.planId || '—'} / ${rollback.planVersion || '—'}. وجود الخطة لا يعني أن rollback مخول أو مضمون النجاح.`,`Plan: ${rollback.planId || '—'} / ${rollback.planVersion || '—'}. A plan does not mean rollback is authorized or guaranteed to succeed.`):local('Proposal أو ادعاء الصلاحية أو خطة الرجوع غير صالح، لذلك تبقى المراجعة يدوية fail-closed.','Proposal, authority claim, or rollback plan is invalid, so review remains manual and fail-closed.'),tone:checked.valid?StatusTone.NEUTRAL:StatusTone.NEGATIVE,role:checked.valid?'status':'alert'})}
      <p class="muted">${safeText(local(`النتيجة المحلية: ${eligibility.reason}. هذه أهلية فقط وليست Auto Accept.`,`Local result: ${eligibility.reason}. This is eligibility only, not Auto Accept.`))}</p>
      <div class="hero-actions"><button class="primary" data-owner-proposal-evaluate="${index}"${disabledControlAttributes(!transportAvailable || !autoEligible,local('قرار Auto Accept الحقيقي يحتاج عقد Foundation وtransport محكوم فعال وسياسة حالية.','A real Auto Accept decision requires the Foundation contract, active governed transport, and a current policy.'))}>${safeText(local('فحص/إصدار قرار المالك','Evaluate Owner decision'))}</button><button class="secondary" data-owner-proposal-review="${index}" disabled aria-disabled="true" data-disabled-reason="${safeText(local('مسار المراجعة اليدوية التشغيلي غير مربوط بعد.','The governed manual-review action path is not bound yet.'))}">${safeText(local('مراجعة يدوية','Manual review'))}</button></div>
    </article>`;
  }

  function historyCard(item,index) {
    const canRequestRollback=item?.rollbackAvailable===true && transportAvailable;
    const decision=item?.decisionState ?? item?.state ?? 'UNAVAILABLE';
    return `<article class="widget owner-history-card" data-owner-history="${index}">
      <div class="widget-head"><div><h3>${safeText(item?.proposalId ?? '—')}</h3><small>${safeText(item?.decisionId ?? '—')}</small></div>${statusBadge({label:decision,tone:toneForDisposition(decision)})}</div>
      <div class="truth-grid"><span><b>${safeText(local('السياسة','Policy'))}</b>${safeText(`${item?.policyId ?? '—'} / ${item?.policyVersion ?? '—'}`)}</span><span><b>${safeText(local('خطة الرجوع','Rollback plan'))}</b>${safeText(`${item?.planId ?? '—'} / ${item?.planVersion ?? '—'}`)}</span><span><b>${safeText(local('الدليل','Evidence'))}</b>${safeText(item?.evidenceReference ?? '—')}</span></div>
      <p class="muted">${safeText(local('هذا السجل يشرح لماذا تم القبول. القبول لا يعني تنفيذ أو نشر أو صلاحية تجارية.','This record explains why the update was accepted. Acceptance is not execution, deployment, or business authority.'))}</p>
      <button class="secondary" data-owner-rollback-request="${index}"${disabledControlAttributes(!canRequestRollback,transportAvailable?local('خطة الرجوع غير متاحة/صالحة لهذا القرار.','Rollback is not available/valid for this decision.'):local('Rollback Order يحتاج transport محكوم غير مفعل بعد.','Rollback Order requires governed transport that is not active yet.'))}>${safeText(local('طلب Rollback','Request rollback'))}</button>
    </article>`;
  }

  function ownerApprovalsPage() {
    const transportNotice=transportAvailable
      ? notice({title:local('الربط المحكوم متاح','Governed binding available'),body:local('كل قرار يظل مربوطًا بالسياسة والـProposal والدليل الحالي.','Every decision remains bound to the current policy, proposal, and evidence.'),tone:StatusTone.POSITIVE})
      : notice({title:local('القرارات التشغيلية مقفولة حاليًا','Operational decisions are currently locked'),body:local('العقود الدلالية جاهزة، لكن FIL request transport ما زال بانتظار FCR-0241. العرض والتحقق المحلي لا ينشئان Owner authority.','Semantic contracts are ready, but the FIL request transport is pending FCR-0241. Presentation and local validation do not create Owner authority.'),tone:StatusTone.WARNING,role:'alert'});

    return renderWorkspace(`<div class="page-head"><div><h1>${safeText(local('الموافقات والتحديثات','Approvals & Updates'))}</h1><p class="muted">${safeText(local('Standing Approvals، Auto-Accept history، وخطط الرجوع في مكان واحد.','Standing approvals, Auto-Accept history, and rollback plans in one place.'))}</p></div></div>
      <section class="page-widget">${transportNotice}</section>
      <section class="page-widget"><div class="section-head"><div><h2>${safeText(local('Standing Approvals','Standing Approvals'))}</h2><p>${safeText(local('قواعد المالك الحالية القابلة للتعديل أو الإلغاء.','Current Owner rules that can be changed or revoked.'))}</p></div></div><div class="owner-app-list">${policies.length?policies.map(policyCard).join(''):`<article class="widget"><p class="muted">${safeText(local('لا توجد سياسات محكومة معروضة من المصدر الحالي.','No governed policies are available from the current source.'))}</p></article>`}</div></section>
      <section class="page-widget"><div class="section-head"><div><h2>${safeText(local('Proposal Inbox','Proposal Inbox'))}</h2><p>${safeText(local('الـApplication والـAI يرسلون Proposal فقط. لا يملكون Auto Accept.','Applications and AIs submit proposals only. They cannot self Auto Accept.'))}</p></div></div><div class="owner-app-list">${proposals.length?proposals.map(proposalCard).join(''):`<article class="widget"><p class="muted">${safeText(local('لا توجد Proposals من مصدر محكوم حاليًا.','No proposals are available from a governed source.'))}</p></article>`}</div></section>
      <section class="page-widget"><div class="section-head"><div><h2>${safeText(local('Auto-Accepted History','Auto-Accepted History'))}</h2><p>${safeText(local('كل قرار يحتفظ بالسياسة والـProposal وخطة الرجوع والدليل.','Every decision retains its policy, proposal, rollback plan, and evidence.'))}</p></div></div><div class="owner-app-list">${history.length?history.map(historyCard).join(''):`<article class="widget"><p class="muted">${safeText(local('لا يوجد سجل Auto Accept موثوق من المصدر الحالي.','No authoritative Auto Accept history is available from the current source.'))}</p></article>`}</div></section>`,'owner-approvals',true);
  }

  return Object.freeze({ ownerApprovalsPage });
}
