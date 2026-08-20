import { VoicePolicy, localVoiceProviderPlan } from './voice/voice-policy.js';

export const IncidentPriority = Object.freeze({ HIGH:'HIGH', MEDIUM:'MEDIUM', LOW:'LOW' });
export const IncidentInteractionMode = Object.freeze({
  FALCON_ACTIVE:'FALCON_ACTIVE',
  SUPPORT_ESCALATED_FALCON_ACTIVE:'SUPPORT_ESCALATED_FALCON_ACTIVE',
  SUPPORT_TAKEOVER:'SUPPORT_TAKEOVER'
});

export function canShowCredentialInChat() { return false; }

export function validateScreenshotMeta(meta) {
  if (!meta) return { allowed:false, reason:'MISSING_METADATA' };
  if ((meta.fileCount ?? 1) !== 1) return { allowed:false, reason:'ONE_SCREENSHOT_AT_A_TIME' };
  if (meta.containsSecret === true) return { allowed:false, reason:'SECRET_CONTENT_PROHIBITED' };
  if (meta.securityScanState !== 'CLEAN') return { allowed:false, reason:'AUTHORITATIVE_SECURITY_SCAN_REQUIRED' };
  if (meta.scanProvenance !== 'GOVERNED_UPLOAD_SECURITY_SCANNER') return { allowed:false, reason:'UNTRUSTED_SCAN_PROVENANCE' };
  return { allowed:true, reason:null };
}

export function ownerDelayAlert({ priority, viewedAt, repliedAt, now }) {
  if (priority !== IncidentPriority.HIGH) return false;
  if (!viewedAt || repliedAt || !now) return false;
  const elapsed = new Date(now).getTime() - new Date(viewedAt).getTime();
  return Number.isFinite(elapsed) && elapsed >= 5 * 60 * 1000;
}

export function supportTakeoverAllowed({
  mode,
  explicitTakeover,
  supportIdentityVisible,
  authoritativeSupportSession,
  supportPrincipalId,
  takeoverCapability
}) {
  return mode === IncidentInteractionMode.SUPPORT_TAKEOVER
    && explicitTakeover === true
    && supportIdentityVisible === true
    && authoritativeSupportSession === true
    && typeof supportPrincipalId === 'string'
    && supportPrincipalId.length > 0
    && takeoverCapability === 'INCIDENT_SUPPORT_TAKEOVER';
}

export function createOwnerReportedContactRecord({ incidentId, attemptCount, outcome, reportedAt } = {}) {
  if (typeof incidentId !== 'string' || incidentId.length===0) throw new TypeError('incidentId is required');
  if (!Number.isInteger(attemptCount) || attemptCount < 0) throw new TypeError('attemptCount must be a non-negative integer');
  if (!['REACHED','NOT_REACHED','UNKNOWN'].includes(outcome)) throw new TypeError('invalid contact outcome');
  if (typeof reportedAt !== 'string' || reportedAt.length===0) throw new TypeError('reportedAt is required');
  return Object.freeze({
    incidentId,
    attemptCount,
    outcome,
    reportedAt,
    provenance:'OWNER_REPORTED',
    telephonyVerified:false,
    independentCommunicationProof:false,
    brokerTruth:false,
    businessStateOverride:false,
    incidentResolution:false
  });
}

export function createCustomerReturnNotice({ incidentId, contactRecord, authoritativeIncidentState } = {}) {
  if (!contactRecord || contactRecord.incidentId !== incidentId) return null;
  if (contactRecord.outcome !== 'NOT_REACHED') return null;
  return Object.freeze({
    incidentId,
    showOnce:true,
    attemptCount:contactRecord.attemptCount,
    historicalContactOutcome:'NOT_REACHED',
    authoritativeIncidentState:authoritativeIncidentState ?? 'UNKNOWN',
    seeingNoticeReactivatesAccount:false,
    noticeIsIncidentResolution:false,
    historicalContactFailureOverridesCurrentTruth:false
  });
}

export function incidentConversationState(input = {}) {
  const mode = input.mode ?? IncidentInteractionMode.FALCON_ACTIVE;
  const supportCanType = supportTakeoverAllowed({
    mode,
    explicitTakeover: input.explicitTakeover,
    supportIdentityVisible: input.supportIdentityVisible,
    authoritativeSupportSession: input.authoritativeSupportSession,
    supportPrincipalId: input.supportPrincipalId,
    takeoverCapability: input.takeoverCapability
  });

  return Object.freeze({
    persistent: true,
    supportCanObserve: true,
    supportCanType,
    supportCanImpersonateAI: false,
    falconCustomerFacing: mode !== IncidentInteractionMode.SUPPORT_TAKEOVER,
    falconSilentObserver: mode === IncidentInteractionMode.SUPPORT_TAKEOVER,
    credentialEntrySurface: 'OUTSIDE_CHAT_SECURE_SURFACE',
    credentialsInChatAllowed: false,
    screenshotUploadPolicy: 'ONE_AT_A_TIME_NO_SECRETS_GOVERNED_SCAN_REQUIRED',
    acknowledgementIsResolution: false,
    deliveredIsResolution: false,
    escalationIsTakeover: false,
    takeoverIsResolution: false,
    dismissOrMinimizeStopsEscalationTimer: false,
    voiceMessageSilenceAutoStop: VoicePolicy.voiceMessageSilenceAutoStop,
    liveVoiceSilenceToleranceMs: VoicePolicy.liveVoiceSilenceToleranceMs,
    liveVoiceExplicitOptInRequired: VoicePolicy.liveVoiceExplicitOptInRequired,
    hiddenMicrophoneActivationAllowed: VoicePolicy.hiddenMicrophoneActivationAllowed,
    voiceProviderPlan: localVoiceProviderPlan(),
    ...input,
    mode,
    supportCanType,
    supportCanImpersonateAI: false,
    credentialsInChatAllowed: false,
    acknowledgementIsResolution: false,
    deliveredIsResolution: false,
    escalationIsTakeover: false,
    takeoverIsResolution: false,
    dismissOrMinimizeStopsEscalationTimer: false,
    voiceMessageSilenceAutoStop: false,
    liveVoiceSilenceToleranceMs: 15_000,
    liveVoiceExplicitOptInRequired: true,
    hiddenMicrophoneActivationAllowed: false,
    voiceProviderPlan: localVoiceProviderPlan()
  });
}
