import test from 'node:test';
import assert from 'node:assert/strict';
import { createOwnerCommandCenterFeature } from '../src/features/owner-command-center/owner-command-center.js';

const labels = {
  ownerCenter:'Owner Command Center',noTradingOwner:'System control only',systemHealth:'System Health',activeApps:'Active Apps',activeUsers:'Active Users',incidents:'Incidents',approvals:'Approvals',systemOverview:'System Overview',needsAttention:'Needs Attention',systemChat:'System Chat',controls:'Controls',simulator:'Simulator',backup:'Backup',security:'Security',applications:'Applications',users:'Users',audit:'Audit',settings:'Settings',language:'Language',systemController:'System Controller',
  truth_CURRENT:'Current',truth_UNKNOWN:'Unknown',priority_HIGH:'High',priority_MEDIUM:'Medium',priority_LOW:'Low',controlTruthNotice:'Controls submit requests only until authoritative outcome.',ownerAppTruthNotice:'System/Application status only.',supportIncidentAccessNotice:'Support may view and explicitly take over an incident conversation.',view:'View',takeOverAsSupport:'Take over as Support',supportAuthorityNotice:'Support takeover does not create execution authority or incident resolution.',supportTakeoverUnavailable:'Support takeover stays fail-closed until an authoritative Support/session identity is available.',fiveMinuteNoReplyFollowup:'5-minute no-reply follow-up',fiveMinuteNoReplyTruthNotice:'No reply after view is an observable fact; it does not mean the customer ignored or understood it.',pendingGovernedDecision:'Pending governed decision',approvalTruthNotice:'A UI click is not authoritative completion.',review:'Review',evidence:'Evidence',user:'User',active:'Active',inactive:'Inactive',auditIncidentEvidenceOpened:'Incident evidence opened.',auditAppProjectionUpdated:'Projection updated.',auditBackupProjectionReceived:'Backup projection received.',ownerOnly:'Owner only',simulationDiagnostics:'Simulation diagnostics',simulatorTruthNotice:'Simulator truth remains Application-owned and is not fabricated by Web.'
};
const t = key => labels[key] ?? key;
const workspace = (body, active, ownerMode) => `<main data-active="${active}" data-owner="${ownerMode}">${body}</main>`;
const data = {
  owner: { health: 'CURRENT', apps: '5', users: '2', incidents: '1', approvals: '1' },
  services: [['Foundation', 'CURRENT'], ['FSATS', 'CURRENT']],
  incidents: [['HIGH', 'Customer incident requires attention']]
};

test('Owner Command Center remains a request/presentation surface', () => {
  const feature = createOwnerCommandCenterFeature({ t, language: () => 'en', workspace, data });
  const html = feature.owner();
  assert.match(html, /data-owner="true"/);
  assert.match(html, /conversation alone does not create execution authority/);
  assert.match(html, /Controls submit requests only until authoritative outcome/);
});

test('Support takeover stays fail-closed without authoritative Support identity', () => {
  const feature = createOwnerCommandCenterFeature({ t, language: () => 'en', workspace, data });
  const html = feature.ownerIncidents();
  assert.match(html, /Support may view and explicitly take over/);
  assert.match(html, /Take over as Support/);
  assert.match(html, /data-support-takeover="0" disabled/);
  assert.match(html, /fail-closed until an authoritative Support\/session identity is available/);
});

test('Support takeover control enables only for authoritative support capability',()=>{
  const feature=createOwnerCommandCenterFeature({t,language:()=> 'en',workspace,data,supportAuthorization:()=>({authoritativeSession:true,principalId:'support-1',role:'SUPPORT',capabilities:['INCIDENT_SUPPORT_TAKEOVER']})});
  assert.doesNotMatch(feature.ownerIncidents(),/data-support-takeover="0" disabled/);
});

test('five-minute no-reply alert reports observable facts only',()=>{
  const withAlert={...data,incidentConversation:{priority:'HIGH',viewedAt:'2026-08-15T19:00:00Z',repliedAt:null,dismissedAt:'2026-08-15T19:01:00Z'},now:'2026-08-15T19:05:00Z'};
  const html=createOwnerCommandCenterFeature({t,language:()=> 'en',workspace,data:withAlert}).ownerIncidents();
  assert.match(html,/5-minute no-reply follow-up/);
  assert.match(html,/does not mean the customer ignored or understood it/);
});

test('Owner approvals preserve request, authority and outcome separation', () => {
  const feature = createOwnerCommandCenterFeature({ t, language: () => 'en', workspace, data });
  assert.match(feature.ownerApprovals(), /A UI click is not authoritative completion/);
});

test('Owner simulator remains Owner-only and does not fabricate Application truth', () => {
  const feature = createOwnerCommandCenterFeature({ t, language: () => 'en', workspace, data });
  const html = feature.ownerSimulator();
  assert.match(html, /Owner only/);
  assert.match(html, /Simulator truth remains Application-owned and is not fabricated by Web/);
});

test('Owner feature validates required dependencies and projections', () => {
  assert.throws(() => createOwnerCommandCenterFeature({ language: () => 'en', workspace, data }), /t must be a function/);
  assert.throws(() => createOwnerCommandCenterFeature({ t, workspace, data }), /language must be a function/);
  assert.throws(() => createOwnerCommandCenterFeature({ t, language: () => 'en', data }), /workspace must be a function/);
  assert.throws(() => createOwnerCommandCenterFeature({ t, language: () => 'en', workspace, data: {} }), /data\.owner, data\.services and data\.incidents are required/);
});
