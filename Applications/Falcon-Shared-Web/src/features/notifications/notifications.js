import { IncidentInteractionMode } from '../../incidents.js';
import { safeText } from '../../security/safe-html.js';

function requireFunction(value, name) {
  if (typeof value !== 'function') throw new TypeError(`${name} must be a function`);
  return value;
}

const messageMarkup = (message, t) => {
  const sender = message?.sender === 'SUPPORT' ? t('supportAgent') : message?.sender === 'CUSTOMER' ? t('customer') : 'Falcon';
  return `<div class="message ${message?.sender === 'CUSTOMER' ? 'user' : 'assistant'}"><b>${safeText(sender)}</b><p>${safeText(message?.text)}</p>${message?.edited ? `<small>${safeText(t('transcriptEdited'))}</small>` : ''}</div>`;
};

const disabled = enabled => enabled ? '' : 'disabled aria-disabled="true"';

/**
 * Customer incident notification and persistent interaction presentation.
 *
 * Application/FSATS owns incident and broker-account business semantics. Web owns
 * customer-facing interaction only. Screenshots are observations; credentials are
 * prohibited in chat; delivery/acknowledgement/takeover never resolve an incident.
 */
export function createNotificationsFeature({ t, workspace, data } = {}) {
  const translate = requireFunction(t, 't');
  const renderWorkspace = requireFunction(workspace, 'workspace');
  if (!data?.incidentConversation) throw new TypeError('data.incidentConversation is required');

  function notificationsPage() {
    const c = data.incidentConversation;
    const mode = c.mode ?? IncidentInteractionMode.FALCON_ACTIVE;
    const unresolved = c.resolved !== true;
    const minimized = c.minimized === true;
    const messages = Array.isArray(c.messages) && c.messages.length
      ? c.messages
      : [{ sender:'FALCON', text:c.message }];
    const supportTakenOver = mode === IncidentInteractionMode.SUPPORT_TAKEOVER;
    const waitingForReply = c.awaitingCustomerReply === true;
    const capabilities = c.capabilities ?? {};
    const screenshotEnabled = capabilities.screenshotUploadAuthorized === true;
    const voiceEnabled = capabilities.voiceConversationAuthorized === true;
    const textReplyEnabled = capabilities.textReplyAuthorized === true;

    const shellClass = `incident-center ${unresolved ? 'incident-unresolved' : 'incident-resolved'} ${minimized ? 'incident-minimized' : ''}`;
    const modeBanner = supportTakenOver
      ? `<div class="attention"><b>${safeText(translate('supportTakeoverActive'))}</b><p>${safeText(translate('supportIdentityNotice'))}</p></div>`
      : c.escalatedToSupport
        ? `<div class="attention"><b>${safeText(translate('supportEscalated'))}</b><p>${safeText(translate('falconStillActive'))}</p></div>`
        : '';

    const followup = waitingForReply
      ? `<div class="incident-followup"><b>${safeText(translate('actionRequired'))}</b><p>${safeText(c.outstandingAction ?? translate('incidentFollowupGeneric'))}</p></div>`
      : '';

    const timeline = Array.isArray(c.timeline) && c.timeline.length
      ? `<details class="incident-timeline"><summary>${safeText(translate('incidentTimeline'))}</summary>${c.timeline.map(e=>`<div class="audit-line"><time>${safeText(e.at)}</time><p>${safeText(e.label)}</p><small>${safeText(e.source,'')}</small></div>`).join('')}</details>`
      : '';

    const replyTarget = supportTakenOver ? translate('replyToSupport') : translate('replyToFalcon');
    return renderWorkspace(`<div class="page-head"><h1>${safeText(translate('notifications'))}</h1></div><section class="widget page-widget ${shellClass}"><div class="incident-title"><span class="severity high">${safeText(translate(`priority_${c.priority ?? 'HIGH'}`))}</span><div><h3>${safeText(translate('incidentConversation'))}</h3><small>${safeText(translate(`incidentStatus_${c.status ?? 'OPEN'}`))}</small></div></div>${modeBanner}${messages.map(m=>messageMarkup(m,translate)).join('')}${followup}${timeline}<div class="incident-actions"><button class="secondary" data-incident-action="upload-screenshot" ${disabled(screenshotEnabled)}>${safeText(translate('uploadOneScreenshot'))}</button><button class="secondary" data-incident-action="voice" ${disabled(voiceEnabled)}>${safeText(translate('voiceConversation'))}</button><button class="secondary" data-incident-action="minimize">${safeText(minimized ? translate('restoreIncident') : translate('minimizeIncident'))}</button></div><p class="muted tiny">${safeText(translate('incidentTruthNotice'))}</p>${(!screenshotEnabled || !voiceEnabled || !textReplyEnabled) ? `<p class="muted tiny">${safeText(translate('incidentUnavailableActionsNotice'))}</p>` : ''}${c.voiceListening ? `<div class="privacy-indicator"><b>${safeText(translate('voiceListeningActive'))}</b><button class="secondary" data-incident-action="stop-listening">${safeText(translate('stopVoiceListening'))}</button></div>` : ''}<div class="chat-input"><input placeholder="${safeText(replyTarget)}" aria-label="${safeText(replyTarget)}" ${disabled(textReplyEnabled)}><button class="primary" ${disabled(textReplyEnabled)}>➤</button></div></section><section class="widget page-widget"><div class="attention"><b>${safeText(translate('information'))}</b><p>${safeText(translate('incidentResolutionNotice'))}</p></div></section>`,'notifications');
  }

  return Object.freeze({ notificationsPage });
}
