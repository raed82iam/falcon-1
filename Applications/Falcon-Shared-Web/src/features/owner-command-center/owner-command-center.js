import { safeText } from '../../security/safe-html.js';
import { ownerDelayAlert, IncidentPriority } from '../../incidents.js';

function requireFunction(value, name) {
  if (typeof value !== 'function') throw new TypeError(`${name} must be a function`);
  return value;
}

const list = value => Array.isArray(value) ? value : [];
const currentClass = value => ['CURRENT','HEALTHY','READY','AVAILABLE'].includes(String(value ?? '').toUpperCase()) ? 'positive' : 'muted';

export function createOwnerCommandCenterFeature({ t, language, workspace, data, supportAuthorization = () => null } = {}) {
  const translate = requireFunction(t, 't');
  const currentLanguage = requireFunction(language, 'language');
  const renderWorkspace = requireFunction(workspace, 'workspace');
  const getSupportAuthorization = requireFunction(supportAuthorization, 'supportAuthorization');
  if (!data?.owner || !Array.isArray(data?.services) || !Array.isArray(data?.incidents)) {
    throw new TypeError('data.owner, data.services and data.incidents are required');
  }

  const local = (ar,en) => currentLanguage()==='ar' ? ar : en;
  const localizedTruth = value => translate(`truth_${String(value ?? 'UNKNOWN')}`);
  const localizedPriority = value => translate(`priority_${String(value ?? 'LOW').toUpperCase()}`);
  const supportCanTakeover = () => {
    const session = getSupportAuthorization();
    return Boolean(
      session
      && session.authoritativeSession === true
      && typeof session.principalId === 'string'
      && session.principalId.length > 0
      && (session.role === 'PROJECT_OWNER' || session.role === 'SUPPORT')
      && Array.isArray(session.capabilities)
      && session.capabilities.includes('INCIDENT_SUPPORT_TAKEOVER')
    );
  };

  function owner() {
    const gatewayAvailable=data.ownerGatewayTransportAvailable === true;
    return renderWorkspace(`<div class="owner-head"><div><h1>${safeText(translate('ownerCenter'))}</h1><p>${safeText(translate('noTradingOwner'))}</p></div><span class="status-chip">${safeText(translate('systemController'))}</span></div><div class="owner-metrics"><article><span>${safeText(translate('systemHealth'))}</span><strong class="${currentClass(data.owner.health)}">${safeText(data.owner.health)}</strong></article><article><span>${safeText(translate('activeApps'))}</span><strong>${safeText(data.owner.apps)}</strong></article><article><span>${safeText(translate('activeUsers'))}</span><strong>${safeText(data.owner.users)}</strong></article><article><span>${safeText(translate('incidents'))}</span><strong class="${String(data.owner.incidents)==='0'?'muted':'negative'}">${safeText(data.owner.incidents)}</strong></article><article><span>${safeText(translate('approvals'))}</span><strong>${safeText(data.owner.approvals)}</strong></article></div><div class="owner-grid"><section class="widget system-map"><div class="widget-head"><h3>${safeText(translate('systemOverview'))}</h3></div><div class="system-core">FALCON<div class="nodes">${data.services.map(s=>`<span>${safeText(s[0])}<small class="${currentClass(s[1])}">${safeText(localizedTruth(s[1]))}</small></span>`).join('')}</div></div></section><section class="widget"><div class="widget-head"><h3>${safeText(translate('needsAttention'))}</h3></div>${data.incidents.length?data.incidents.map(i=>`<div class="incident"><span class="severity ${String(i[0]).toLowerCase()==='high'?'high':String(i[0]).toLowerCase()==='medium'?'medium':'low'}">${safeText(localizedPriority(i[0]))}</span><p>${safeText(i[1])}</p></div>`).join(''):`<p class="muted">${safeText(local('لا توجد Incident projection موثوقة للعرض حاليًا.','No authoritative incident projection is currently available.'))}</p>`}</section><section class="widget owner-chat"><div class="widget-head"><h3>${safeText(translate('systemChat'))}</h3></div><div class="message assistant"><b>Falcon System</b><p>${safeText(currentLanguage()==='ar'?'أنا بوابة محادثة للنظام. أعرض الحالة والنتائج من مصادرها الموثوقة، والمحادثة وحدها لا تنشئ صلاحية تنفيذ.':'I am the system conversational gateway. I present authoritative state and outcomes; conversation alone does not create execution authority.')}</p></div><div class="quick-prompts"><button disabled aria-disabled="true">${safeText(translate('systemHealth'))}</button><button disabled aria-disabled="true">${safeText(translate('incidents'))}</button><button disabled aria-disabled="true">${safeText(translate('approvals'))}</button></div><div class="chat-input"><input placeholder="${safeText(translate('systemChat'))}…" disabled aria-disabled="true"><button class="primary" disabled aria-disabled="true">➤</button></div><p class="muted tiny">${safeText(gatewayAvailable?local('Gateway transport معلن لكن واجهة الإرسال التنفيذية لم تُربط بعد.','Gateway transport is declared available, but executable submission UI is not bound yet.'):local('Owner conversational transport غير متاح؛ لا يتم ادعاء إرسال أي طلب.','Owner conversational transport is unavailable; no request delivery is claimed.'))}</p></section><section class="widget controls"><div class="widget-head"><h3>${safeText(translate('controls'))}</h3></div><button data-nav="owner-simulator">${safeText(translate('simulator'))}</button><button disabled aria-disabled="true">${safeText(translate('backup'))}</button><button disabled aria-disabled="true">${safeText(translate('security'))}</button><p class="muted">${safeText(translate('controlTruthNotice'))}</p></section></div>`,'owner',true);
  }

  function ownerSection(active, title, body) {
    return renderWorkspace(`<div class="page-head"><h1>${safeText(title)}</h1></div>${body}`,active,true);
  }

  function ownerApps() {
    return ownerSection('owner-apps',translate('applications'),`<div class="owner-app-list">${data.services.length?data.services.map(s=>`<article class="widget"><b>${safeText(s[0])}</b><span class="status-chip">${safeText(localizedTruth(s[1]))}</span><p class="muted">${safeText(translate('ownerAppTruthNotice'))}</p></article>`).join(''):`<section class="widget page-widget"><p class="muted">${safeText(local('لا توجد Application projection موثوقة متاحة حاليًا.','No authoritative Application projection is currently available.'))}</p></section>`}</div>`);
  }

  function ownerIncidents() {
    const canTakeover = supportCanTakeover();
    const c = data.incidentConversation ?? {};
    const fiveMinuteAlert = ownerDelayAlert({
      priority:c.priority ?? IncidentPriority.HIGH,
      viewedAt:c.viewedAt ?? null,
      repliedAt:c.repliedAt ?? null,
      now:data.now ?? null
    });
    const escalationNotice = fiveMinuteAlert
      ? `<div class="attention"><b>${safeText(translate('fiveMinuteNoReplyFollowup'))}</b><p>${safeText(translate('fiveMinuteNoReplyTruthNotice'))}</p></div>`
      : '';
    const rows=data.incidents.length?data.incidents.map((i,index)=>`<div class="incident incident-row"><span class="severity ${String(i[0]).toLowerCase()==='high'?'high':String(i[0]).toLowerCase()==='medium'?'medium':'low'}">${safeText(localizedPriority(i[0]))}</span><p>${safeText(i[1])}</p><button class="secondary" data-support-view="${index}">${safeText(translate('view'))}</button><button class="secondary" data-support-takeover="${index}" ${canTakeover?'':'disabled aria-disabled="true"'}>${safeText(translate('takeOverAsSupport'))}</button></div>`).join(''):`<p class="muted">${safeText(local('لا توجد Incident projection موثوقة متاحة حاليًا.','No authoritative incident projection is currently available.'))}</p>`;
    return ownerSection('owner-incidents',translate('incidents'),`<section class="widget page-widget"><div class="owner-observer-banner">${safeText(translate('supportIncidentAccessNotice'))}</div>${escalationNotice}${rows}<p class="muted tiny">${safeText(canTakeover ? translate('supportAuthorityNotice') : translate('supportTakeoverUnavailable'))}</p></section>`);
  }

  function ownerApprovals() {
    return ownerSection('owner-approvals',translate('approvals'),`<section class="widget page-widget"><div class="attention"><b>${safeText(translate('pendingGovernedDecision'))}</b><p>${safeText(translate('approvalTruthNotice'))}</p><div class="hero-actions"><button class="primary" disabled aria-disabled="true">${safeText(translate('review'))}</button><button class="secondary" disabled aria-disabled="true">${safeText(translate('evidence'))}</button></div></div></section>`);
  }

  function ownerUsers() {
    const users=list(data.ownerUsers);
    const body=users.length
      ? `<div class="data-table">${users.map(user=>`<div class="row"><b>${safeText(user.displayName ?? user.principalId ?? '—')}</b><span>${safeText(user.status ?? 'UNKNOWN')}</span><span>${safeText(user.system ?? '—')}</span><span>${safeText(localizedTruth(user.truthState ?? 'UNKNOWN'))}</span></div>`).join('')}</div>`
      : `<div class="empty-state"><h2>${safeText(local('بيانات المستخدمين غير متاحة','User projection unavailable'))}</h2><p>${safeText(local('Web لا يخترع مستخدمين أو حالات وصول عند غياب authoritative projection.','Web does not invent users or access states when the authoritative projection is unavailable.'))}</p></div>`;
    return ownerSection('owner-users',translate('users'),`<section class="widget page-widget">${body}</section>`);
  }

  function ownerAudit() {
    const events=list(data.ownerAudit);
    const body=events.length
      ? events.map(event=>`<div class="audit-line"><time>${safeText(event.time ?? event.effectiveAt ?? '—')}</time><p>${safeText(event.message ?? event.summary ?? '—')}</p>${event.evidenceReference?`<small>${safeText(event.evidenceReference)}</small>`:''}</div>`).join('')
      : `<div class="empty-state"><h2>${safeText(local('سجل التدقيق غير متاح','Audit projection unavailable'))}</h2><p>${safeText(local('لا يتم إنشاء أحداث Audit تجريبية على سطح الـOwner.','No synthetic Audit events are created on the Owner surface.'))}</p></div>`;
    return ownerSection('owner-audit',translate('audit'),`<section class="widget page-widget">${body}</section>`);
  }

  function ownerSettings() {
    return ownerSection('owner-settings',translate('settings'),`<section class="widget form-settings"><label>${safeText(translate('language'))}<select data-language-select><option value="ar" ${currentLanguage()==='ar'?'selected':''}>العربية</option><option value="en" ${currentLanguage()==='en'?'selected':''}>English</option></select></label></section>`);
  }

  function ownerSimulator() {
    return ownerSection('owner',translate('simulator'),`<section class="widget page-widget"><span class="status-chip">${safeText(translate('ownerOnly'))}</span><h3>${safeText(translate('simulationDiagnostics'))}</h3><p class="muted">${safeText(translate('simulatorTruthNotice'))}</p></section>`);
  }

  return Object.freeze({ owner, ownerApps, ownerIncidents, ownerApprovals, ownerUsers, ownerAudit, ownerSettings, ownerSimulator });
}
