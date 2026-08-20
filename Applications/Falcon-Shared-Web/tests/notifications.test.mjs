import test from 'node:test';
import assert from 'node:assert/strict';
import { createNotificationsFeature } from '../src/features/notifications/notifications.js';

const labels={
  notifications:'Notifications',priority_HIGH:'High',incidentConversation:'Incident conversation',incidentStatus_OPEN:'Open',
  uploadOneScreenshot:'Upload one screenshot',voiceConversation:'Voice conversation',minimizeIncident:'Minimize incident',restoreIncident:'Restore incident',
  incidentTruthNotice:'Never send passwords, API keys or secrets here. A screenshot is an observation, not broker API confirmation. Delivery, reply, escalation or takeover does not resolve the incident.',
  incidentUnavailableActionsNotice:'Some incident actions remain unavailable until secure authorized binding exists.',
  replyToFalcon:'Reply to Falcon…',replyToSupport:'Reply to Support…',information:'Information',incidentResolutionNotice:'Incident remains open until authoritative confirmation.',
  supportTakeoverActive:'Support has taken over the conversation',supportIdentityNotice:'You are now speaking with a human Support agent.',supportAgent:'Support',customer:'Customer',
  actionRequired:'Action required',incidentFollowupGeneric:'Update required',incidentTimeline:'Incident timeline',voiceListeningActive:'Voice listening active',stopVoiceListening:'Stop voice listening',transcriptEdited:'Transcript edited'
};
const t = key => labels[key] ?? key;
const workspace = (body, active) => `<main data-active="${active}">${body}</main>`;

const baseIncident={
  priority:'HIGH',status:'OPEN',resolved:false,mode:'FALCON_ACTIVE',
  message:'Please confirm the broker-account observation.'
};

test('notifications feature preserves customer interaction truth boundaries', () => {
  const { notificationsPage } = createNotificationsFeature({ t, workspace, data:{incidentConversation:baseIncident} });
  const html = notificationsPage();

  assert.match(html, /data-active="notifications"/);
  assert.match(html, /Never send passwords, API keys or secrets here/);
  assert.match(html, /screenshot is an observation, not broker API confirmation/);
  assert.match(html, /does not resolve the incident/);
  assert.match(html, /Upload one screenshot/);
  assert.match(html, /data-incident-action="upload-screenshot" disabled/);
  assert.match(html, /data-incident-action="voice" disabled/);
  assert.match(html, /Some incident actions remain unavailable/);
});

test('authorized incident capabilities enable only their matching controls',()=>{
  const incident={...baseIncident,capabilities:{screenshotUploadAuthorized:true,voiceConversationAuthorized:false,textReplyAuthorized:true}};
  const html=createNotificationsFeature({t,workspace,data:{incidentConversation:incident}}).notificationsPage();
  assert.doesNotMatch(html,/data-incident-action="upload-screenshot" disabled/);
  assert.match(html,/data-incident-action="voice" disabled/);
  assert.doesNotMatch(html,/aria-label="Reply to Falcon…" disabled/);
});

test('Support takeover is explicit to the customer while interaction transport remains capability-gated', () => {
  const incident={...baseIncident,mode:'SUPPORT_TAKEOVER',messages:[{sender:'SUPPORT',text:'I am taking over this incident.'}],capabilities:{textReplyAuthorized:false}};
  const html=createNotificationsFeature({t,workspace,data:{incidentConversation:incident}}).notificationsPage();
  assert.match(html,/Support has taken over the conversation/);
  assert.match(html,/human Support agent/);
  assert.match(html,/Reply to Support/);
  assert.match(html,/aria-label="Reply to Support…" disabled/);
});

test('notifications feature fails closed without incident conversation data', () => {
  assert.throws(
    () => createNotificationsFeature({ t, workspace, data: {} }),
    /data\.incidentConversation is required/
  );
});

test('notifications feature validates presentation dependencies', () => {
  assert.throws(() => createNotificationsFeature({ workspace, data:{incidentConversation:baseIncident} }), /t must be a function/);
  assert.throws(() => createNotificationsFeature({ t, data:{incidentConversation:baseIncident} }), /workspace must be a function/);
});
