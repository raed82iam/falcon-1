import test from 'node:test';
import assert from 'node:assert/strict';
import {
  IncidentPriority,
  IncidentInteractionMode,
  canShowCredentialInChat,
  validateScreenshotMeta,
  ownerDelayAlert,
  supportTakeoverAllowed,
  createOwnerReportedContactRecord,
  createCustomerReturnNotice,
  incidentConversationState
} from '../src/incidents.js';

test('credentials are never accepted in chat', () => assert.equal(canShowCredentialInChat(), false));

test('screenshots containing secrets are rejected', () => assert.equal(validateScreenshotMeta({containsSecret:true,fileCount:1,securityScanState:'CLEAN',scanProvenance:'GOVERNED_UPLOAD_SECURITY_SCANNER'}).allowed, false));

test('only one screenshot is accepted at a time', () => assert.equal(validateScreenshotMeta({containsSecret:false,fileCount:2,securityScanState:'CLEAN',scanProvenance:'GOVERNED_UPLOAD_SECURITY_SCANNER'}).allowed, false));

test('screenshots fail closed without governed security scan evidence', () => {
  assert.equal(validateScreenshotMeta({containsSecret:false,fileCount:1}).allowed,false);
  assert.equal(validateScreenshotMeta({containsSecret:false,fileCount:1,securityScanState:'CLEAN',scanProvenance:'CLIENT_REPORTED'}).allowed,false);
  assert.equal(validateScreenshotMeta({containsSecret:false,fileCount:1,securityScanState:'CLEAN',scanProvenance:'GOVERNED_UPLOAD_SECURITY_SCANNER'}).allowed,true);
});

test('owner high alert starts five minutes after actual view and silence', () => {
  const viewedAt='2026-08-15T04:00:00Z';
  assert.equal(ownerDelayAlert({priority:IncidentPriority.HIGH,viewedAt,repliedAt:null,now:'2026-08-15T04:04:59Z'}), false);
  assert.equal(ownerDelayAlert({priority:IncidentPriority.HIGH,viewedAt,repliedAt:null,now:'2026-08-15T04:05:00Z'}), true);
});

test('dismiss or minimize does not stop five-minute silence escalation, but an actual reply does', () => {
  const common={priority:IncidentPriority.HIGH,viewedAt:'2026-08-15T04:00:00Z',now:'2026-08-15T04:10:00Z'};
  assert.equal(ownerDelayAlert({...common,repliedAt:'2026-08-15T04:06:00Z',dismissedAt:null}),false);
  assert.equal(ownerDelayAlert({...common,repliedAt:null,dismissedAt:'2026-08-15T04:06:00Z'}),true);
});

test('Support can type only after explicit visible takeover backed by authoritative Support capability', () => {
  const base={mode:IncidentInteractionMode.SUPPORT_TAKEOVER,explicitTakeover:true,supportIdentityVisible:true,authoritativeSupportSession:true,supportPrincipalId:'support-1',takeoverCapability:'INCIDENT_SUPPORT_TAKEOVER'};
  assert.equal(supportTakeoverAllowed({...base,explicitTakeover:false}),false);
  assert.equal(supportTakeoverAllowed({...base,supportIdentityVisible:false}),false);
  assert.equal(supportTakeoverAllowed({...base,authoritativeSupportSession:false}),false);
  assert.equal(supportTakeoverAllowed({...base,supportPrincipalId:null}),false);
  assert.equal(supportTakeoverAllowed({...base,takeoverCapability:'OTHER'}),false);
  assert.equal(supportTakeoverAllowed(base),true);
});

test('Falcon becomes silent observer during authorized Support takeover without transferring authority', () => {
  const state=incidentConversationState({mode:IncidentInteractionMode.SUPPORT_TAKEOVER,explicitTakeover:true,supportIdentityVisible:true,authoritativeSupportSession:true,supportPrincipalId:'support-1',takeoverCapability:'INCIDENT_SUPPORT_TAKEOVER'});
  assert.equal(state.supportCanObserve,true);
  assert.equal(state.supportCanType,true);
  assert.equal(state.supportCanImpersonateAI,false);
  assert.equal(state.falconCustomerFacing,false);
  assert.equal(state.falconSilentObserver,true);
  assert.equal(state.takeoverIsResolution,false);
  assert.equal(state.acknowledgementIsResolution,false);
  assert.equal(state.credentialsInChatAllowed,false);
  assert.equal(state.dismissOrMinimizeStopsEscalationTimer,false);
});

test('Owner-reported contact attempts remain human-reported evidence only',()=>{
  const record=createOwnerReportedContactRecord({incidentId:'inc-1',attemptCount:2,outcome:'NOT_REACHED',reportedAt:'2026-08-15T19:00:00Z'});
  assert.equal(record.provenance,'OWNER_REPORTED');
  assert.equal(record.telephonyVerified,false);
  assert.equal(record.independentCommunicationProof,false);
  assert.equal(record.brokerTruth,false);
  assert.equal(record.incidentResolution,false);
});

test('customer return notice is one-time presentation and does not reactivate or resolve',()=>{
  const record=createOwnerReportedContactRecord({incidentId:'inc-1',attemptCount:2,outcome:'NOT_REACHED',reportedAt:'2026-08-15T19:00:00Z'});
  const notice=createCustomerReturnNotice({incidentId:'inc-1',contactRecord:record,authoritativeIncidentState:'OPEN'});
  assert.equal(notice.showOnce,true);
  assert.equal(notice.seeingNoticeReactivatesAccount,false);
  assert.equal(notice.noticeIsIncidentResolution,false);
  assert.equal(notice.historicalContactFailureOverridesCurrentTruth,false);
});
