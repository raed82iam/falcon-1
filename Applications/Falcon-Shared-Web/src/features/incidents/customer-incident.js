import { safeText } from '../../security/safe-html.js';

function local(language, ar, en) { return language() === 'ar' ? ar : en; }
function valueText(value) { return value == null || value === '' ? '—' : String(value); }
function actorLabel(actor, language) {
  const map = { CUSTOMER:['العميل','Customer'], FALCON:['Falcon','Falcon'], SUPPORT:['الدعم','Support'], APPLICATION:['Application','Application'], GUARDIAN:['Guardian','Guardian'], SYSTEM:['النظام','System'] };
  const pair = map[actor] ?? [actor,actor];
  return language() === 'ar' ? pair[0] : pair[1];
}
function listText(value) {
  if (Array.isArray(value)) return value.map(item => typeof item === 'object' ? JSON.stringify(item) : String(item)).join('، ');
  if (value && typeof value === 'object') return JSON.stringify(value);
  return valueText(value);
}

function renderTimeline(conversation, language) {
  const events = Array.isArray(conversation.timeline) && conversation.timeline.length ? conversation.timeline : (conversation.messages ?? []).map((m,index) => ({
    eventId:`legacy-${index}`, timestamp:m.timestamp ?? null, actor:m.sender ?? 'FALCON', type:'TEXT_MESSAGE', payload:{ text:m.text ?? '' }
  }));
  return events.map(event => {
    const text = event?.payload?.text ?? event?.text ?? '';
    const voice = event?.type === 'VOICE_MESSAGE' || event?.type === 'FALCON_VOICE_REPLY';
    const transcript = event?.type === 'VOICE_TRANSCRIPT';
    const supportBoundary = event?.type === 'SUPPORT_TAKEOVER' || event?.type === 'SUPPORT_RELEASE' || event?.type === 'SUPPORT_AVAILABLE' || event?.type === 'SUPPORT_REQUESTED';
    return `<article class="incident-event ${safeText(String(event.actor ?? 'SYSTEM').toLowerCase())} ${supportBoundary?'boundary':''}">
      <div class="incident-event-meta"><b>${safeText(actorLabel(event.actor ?? 'SYSTEM',language))}</b><span>${safeText(valueText(event.timestamp))}</span></div>
      ${voice ? `<button class="incident-audio-row" data-audio-artifact="${safeText(event?.payload?.artifactId ?? '')}" ${event?.payload?.artifactId?'':'disabled'}><span aria-hidden="true">▶</span><span>${safeText(local(language,'تشغيل الرسالة الصوتية','Play voice message'))}</span></button>` : ''}
      ${transcript ? `<div class="incident-transcript"><span>${safeText(local(language,'النص المستخرج','Transcript'))}</span><p>${safeText(text)}</p>${event?.payload?.edited ? `<small>${safeText(local(language,'تم تعديله بواسطة العميل','Edited by customer'))}</small>` : ''}</div>` : (text ? `<p>${safeText(text)}</p>` : '')}
      ${supportBoundary && !text ? `<p class="muted">${safeText(event.type.replaceAll('_',' '))}</p>` : ''}
    </article>`;
  }).join('');
}

function renderActions(actions, language) {
  if (!Array.isArray(actions) || actions.length === 0) return '';
  return `<div class="incident-required-actions"><b>${safeText(local(language,'الخطوات المطلوبة','Required steps'))}</b><ol>${actions.map(action=>`<li>${safeText(action)}</li>`).join('')}</ol></div>`;
}

function renderPosition(item, language) {
  const instrument = item.instrument?.symbol ?? item.instrument?.value ?? item.instrument ?? '—';
  return `<article class="incident-followup-card ${item.followupRequirement==='REQUIRED'?'requires-action':''}">
    <div><b>${safeText(instrument)}</b><span class="incident-kind">${safeText(local(language,'مركز متأثر','Affected position'))}</span></div>
    <dl><div><dt>${safeText(local(language,'الحماية','Protection'))}</dt><dd>${safeText(item.protectionState)}</dd></div><div><dt>${safeText(local(language,'المتابعة','Follow-up'))}</dt><dd>${safeText(item.followupRequirement)}</dd></div><div><dt>${safeText(local(language,'آخر تأكيد من البروكر','Last broker confirmation'))}</dt><dd>${safeText(valueText(item.lastBrokerConfirmedAt))}</dd></div><div><dt>${safeText(local(language,'حقيقة الإسقاط','Truth'))}</dt><dd>${safeText(item.truthState ?? '—')}</dd></div></dl>
    ${renderActions(item.orderedActions,language)}
  </article>`;
}

function renderOrder(item, language) {
  const instrument = item.instrument?.symbol ?? item.instrument?.value ?? item.instrument ?? '—';
  return `<article class="incident-followup-card ${item.followupRequirement==='REQUIRED'?'requires-action':''}">
    <div><b>${safeText(instrument)}</b><span class="incident-kind">${safeText(local(language,'أمر متأثر','Affected order'))}</span></div>
    <dl><div><dt>${safeText(local(language,'حقيقة الأمر','Order truth'))}</dt><dd>${safeText(item.orderTruthState)}</dd></div><div><dt>${safeText(local(language,'المتابعة','Follow-up'))}</dt><dd>${safeText(item.followupRequirement)}</dd></div><div><dt>${safeText(local(language,'آخر تأكيد','Last confirmation'))}</dt><dd>${safeText(valueText(item.lastBrokerConfirmedAt))}</dd></div><div><dt>${safeText(local(language,'الحداثة','Freshness'))}</dt><dd>${safeText(item.freshnessState ?? '—')}</dd></div></dl>
    ${renderActions(item.orderedActions,language)}
  </article>`;
}

function renderShadow(item, language) {
  const scenarios = Array.isArray(item.scenarios) ? item.scenarios : [];
  return `<article class="incident-shadow-card">
    <div class="incident-shadow-head"><b>${safeText(item.instrumentId)}</b><span>${safeText(item.projectionTruth)}</span></div>
    <dl><div><dt>${safeText(local(language,'حالة المراقبة','Monitoring state'))}</dt><dd>${safeText(item.shadowState)}</dd></div><div><dt>${safeText(local(language,'من','From'))}</dt><dd>${safeText(item.monitoringStartedAt)}</dd></div><div><dt>${safeText(local(language,'إلى','To'))}</dt><dd>${safeText(valueText(item.monitoringEndedAt))}</dd></div><div><dt>${safeText(local(language,'الحداثة','Freshness'))}</dt><dd>${safeText(item.freshnessState)}</dd></div></dl>
    ${scenarios.length ? `<div class="shadow-scenarios"><b>${safeText(local(language,'سيناريوهات تشخيصية','Diagnostic scenarios'))}</b>${scenarios.map(s=>`<span>${safeText(s.scenarioIdentity ?? s.identity ?? String(s))}</span>`).join('')}</div>` : ''}
    <p class="incident-truth-warning">${safeText(local(language,'هذه محاكاة تشخيصية وليست حقيقة بروكر مؤكدة.','Diagnostic simulator evidence, not broker-confirmed truth.'))}</p>
  </article>`;
}

function renderClosureSummary(summary, language) {
  if (!summary) return '';
  const windowValue = summary.simulatorWindow && typeof summary.simulatorWindow === 'object'
    ? `${valueText(summary.simulatorWindow.from ?? summary.simulatorWindow.startedAt)} → ${valueText(summary.simulatorWindow.to ?? summary.simulatorWindow.endedAt)}`
    : listText(summary.simulatorWindow);
  return `<section class="incident-closure-summary" aria-label="${safeText(local(language,'ملخص إغلاق الحادثة','Incident closure summary'))}">
    <div class="closure-title"><span>✓</span><div><b>${safeText(local(language,'تم حل الحادثة','Incident resolved'))}</b><small>${safeText(local(language,'ملخص إلزامي من بيانات Application/Guardian','Mandatory summary from Application/Guardian data'))}</small></div></div>
    <dl>
      <div><dt>${safeText(local(language,'المشكلة','Problem'))}</dt><dd>${safeText(listText(summary.problem))}</dd></div>
      <div><dt>${safeText(local(language,'العناصر المتأثرة','Affected items'))}</dt><dd>${safeText(listText(summary.affectedItems))}</dd></div>
      <div><dt>${safeText(local(language,'فترة Simulator','Simulator window'))}</dt><dd>${safeText(windowValue)}</dd></div>
      <div><dt>${safeText(local(language,'ما أعاد الحالة','Restoration'))}</dt><dd>${safeText(listText(summary.restoration))}</dd></div>
      <div><dt>${safeText(local(language,'المتابعة المتبقية','Remaining follow-up'))}</dt><dd>${safeText(listText(summary.remainingFollowup))}</dd></div>
    </dl>
  </section>`;
}

export function createCustomerIncidentFeature({ language } = {}) {
  if (typeof language !== 'function') throw new TypeError('language must be a function');

  function incidentSurface({ conversation, affectedPositions = [], affectedOrders = [], shadowMonitoring = [], voiceReadiness = null } = {}) {
    if (!conversation?.incidentId) return '';
    if (conversation.resolved === true && conversation.closureSummary) {
      return `<div class="incident-resolved-drawer">${renderClosureSummary(conversation.closureSummary,language)}<button class="secondary" data-incident-dismiss-summary>${safeText(local(language,'إغلاق الملخص','Close summary'))}</button></div>`;
    }
    if (conversation.resolved === true && !conversation.closureSummary) return '';
    if (conversation.minimized === true) {
      return `<button class="incident-minimized-edge" data-incident-expand aria-label="${safeText(local(language,'فتح الحادثة','Open incident'))}"><span>!</span><b>${safeText(conversation.incidentId)}</b></button>`;
    }
    const supportTakeover = conversation.mode === 'SUPPORT_TAKEOVER';
    const supportRequested = conversation.supportRequested === true;
    const supportAvailable = conversation.supportAvailable === true;
    const sttReady = voiceReadiness?.speechToText === 'READY';
    const ttsReady = voiceReadiness?.textToSpeech === 'READY';
    return `<div class="incident-backdrop" role="presentation"><section class="customer-incident-modal ${String(conversation.priority ?? '').toLowerCase()==='high'?'critical':''}" role="dialog" aria-modal="true" aria-labelledby="incident-title">
      <header class="incident-modal-head"><div><span class="incident-severity">${safeText(conversation.priority ?? 'HIGH')}</span><h2 id="incident-title">${safeText(local(language,'حادثة تحتاج انتباهك','Incident needs your attention'))}</h2><small>${safeText(conversation.incidentId)}</small></div><div class="incident-head-actions"><button class="secondary" data-support-request ${supportRequested?'disabled':''}>${safeText(supportRequested?local(language,'تم طلب الدعم','Support requested'):local(language,'طلب دعم بشري','Request human support'))}</button><button data-incident-minimize aria-label="${safeText(local(language,'تصغير','Minimize'))}">−</button></div></header>
      ${supportTakeover ? `<div class="support-takeover-banner"><b>${safeText(local(language,'الدعم البشري متصل الآن','Human Support is connected'))}</b><span>${safeText(conversation.supportDisplayName ?? local(language,'الدعم','Support'))}</span><small>${safeText(local(language,'Falcon صامت في واجهة العميل أثناء الاستلام.','Falcon is customer-facing silent during takeover.'))}</small></div>` : supportRequested ? `<div class="support-waiting-banner"><b>${safeText(supportAvailable?local(language,'الدعم أصبح متاحًا','Support is now available'):local(language,'طلب الدعم مفتوح','Support request is open'))}</b><span>${safeText(supportAvailable?local(language,'يمكن الانتقال للدعم عندما تختار.','You can transfer when you choose.'):local(language,'Falcon سيكمل مساعدتك ضمن صلاحياته إلى أن يتوفر الدعم.','Falcon continues bounded guidance while support is unavailable.'))}</span></div>` : ''}
      <div class="incident-modal-grid">
        <div class="incident-conversation-pane">
          <div class="incident-timeline">${renderTimeline(conversation,language)}</div>
          <div class="incident-composer">
            <button data-incident-screenshot title="${safeText(local(language,'إرسال لقطة شاشة آمنة','Send safe screenshot'))}">▣</button>
            <button data-incident-voice-start ${sttReady?'':'disabled'} title="${safeText(sttReady?local(language,'بدء تسجيل صوتي','Start voice recording'):local(language,'Whisper.cpp المحلي غير جاهز','Local Whisper.cpp is not ready'))}">🎙</button>
            <button data-incident-voice-stop hidden title="${safeText(local(language,'إيقاف وإرسال التسجيل','Stop and send recording'))}">■</button>
            <input data-incident-text placeholder="${safeText(local(language,'اكتب ردك...','Type your reply...'))}">
            <button class="primary" data-incident-send>${safeText(local(language,'إرسال','Send'))}</button>
          </div>
          <div class="incident-live-voice"><button class="secondary" data-live-voice-start ${sttReady&&ttsReady?'':'disabled'}>${safeText(local(language,'بدء التوجيه الصوتي المباشر','Start Live Voice Guidance'))}</button><span>${safeText(local(language,'Falcon ينتظر حتى 15 ثانية من السكوت قبل الرد.','Falcon allows up to 15 seconds of silence before replying.'))}</span></div>
          <div class="incident-voice-readiness"><span>STT: ${safeText(voiceReadiness?.speechToText ?? 'UNKNOWN')}</span><span>TTS: ${safeText(voiceReadiness?.textToSpeech ?? 'UNKNOWN')}</span><span>${safeText(local(language,'محلي فقط، بدون API صوت مدفوع','Local only, no paid voice API'))}</span></div>
          <p class="incident-security-note">${safeText(local(language,'لا ترسل كلمات مرور أو مفاتيح API أو أي بيانات سرية.','Do not send passwords, API keys, or secrets.'))}</p>
        </div>
        <aside class="incident-context-pane">
          <section><h3>${safeText(local(language,'العناصر المتأثرة','Affected items'))}</h3>${affectedPositions.map(x=>renderPosition(x,language)).join('')}${affectedOrders.map(x=>renderOrder(x,language)).join('')}${affectedPositions.length===0&&affectedOrders.length===0?`<p class="muted">${safeText(local(language,'لا توجد عناصر موثقة للعرض حاليًا.','No authoritative affected items available to display.'))}</p>`:''}</section>
          <section><h3>FSTSimA</h3>${shadowMonitoring.map(x=>renderShadow(x,language)).join('')}${shadowMonitoring.length===0?`<p class="muted">${safeText(local(language,'لا توجد مراقبة Shadow معروضة حاليًا.','No shadow-monitoring projection currently available.'))}</p>`:''}</section>
        </aside>
      </div>
    </section></div>`;
  }

  return Object.freeze({ incidentSurface });
}
