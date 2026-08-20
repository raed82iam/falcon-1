import test from 'node:test';
import assert from 'node:assert/strict';
import { adaptAffectedPositionProjection, adaptAffectedOrderProjection, adaptEmergencyShadowProjection } from '../src/adapters/fsats-incident-followup-v1.js';

const account = { brokerId:'ALPACA', brokerAccountId:'PAPER-1', environment:'PAPER' };

test('affected position preserves intentional vs unexpected protection semantics', () => {
  const base = { projectionId:'p1', incidentId:'i1', account, position:{ id:'pos1' }, instrument:{ symbol:'AAPL' }, lastBrokerConfirmedAt:'2026-08-16T00:00:00Z', followupReasonCode:'PROTECTION_CHECK', orderedActions:[{ action:'VERIFY_PROTECTION_ORDERS' }], emergencyShadowMonitoringActive:true, shadowCaseId:'s1', incidentState:'OPEN', truthState:'UNKNOWN', freshnessState:'UNKNOWN', evidenceReference:'ev1', asOfTime:'2026-08-16T00:01:00Z' };
  const intentional = adaptAffectedPositionProjection({ ...base, protectionState:'INTENTIONALLY_RETAINED_WITHOUT_CURRENT_BROKER_PROTECTION', followupRequirement:'RECOMMENDED' });
  assert.equal(intentional.protectionState,'INTENTIONALLY_RETAINED_WITHOUT_CURRENT_BROKER_PROTECTION');
  assert.throws(() => adaptAffectedPositionProjection({ ...base, protectionState:'UNEXPECTEDLY_MISSING_OR_INCOMPLETE_PROTECTION', followupRequirement:'RECOMMENDED' }), /REQUIRED/);
});

test('ambiguous affected order cannot claim no follow-up', () => {
  const base = { projectionId:'o1', incidentId:'i1', account, order:{ id:'ord1' }, instrument:{ symbol:'NVDA' }, orderTruthState:'OUTCOME_UNKNOWN_OR_AMBIGUOUS', followupReasonCode:'UNKNOWN_OUTCOME', orderedActions:[{ action:'RESOLVE_AMBIGUOUS_SUBMISSION' }], truthState:'UNKNOWN', freshnessState:'UNKNOWN', evidenceReference:'ev2', asOfTime:'2026-08-16T00:02:00Z' };
  assert.throws(() => adaptAffectedOrderProjection({ ...base, followupRequirement:'NONE' }), /requires follow-up/);
  assert.equal(adaptAffectedOrderProjection({ ...base, followupRequirement:'REQUIRED' }).orderTruthState,'OUTCOME_UNKNOWN_OR_AMBIGUOUS');
});

test('ambiguous shadow requires source order and three explicit execution scenarios', () => {
  const base = { projectionId:'sproj1', incidentId:'i1', shadowCaseId:'shadow1', brokerId:'ALPACA', brokerAccountId:'PAPER-1', environment:'PAPER', sourceOrderId:'ord1', instrumentId:'AAPL', lastBrokerConfirmedAt:'2026-08-16T00:00:00Z', monitoringStartedAt:'2026-08-16T00:01:00Z', monitoringEndedAt:null, shadowState:'ACTIVE', containsExecutionAmbiguity:true, scenarios:[{scenarioType:'NOT_EXECUTED'},{scenarioType:'PARTIALLY_EXECUTED'},{scenarioType:'FULLY_EXECUTED'}], asOfTime:'2026-08-16T00:02:00Z', projectionTruth:'SIMULATOR', freshnessState:'CURRENT', provenanceReference:'prov1', evidenceReference:'ev3' };
  const result = adaptEmergencyShadowProjection(base);
  assert.equal(result.projectionTruth,'SIMULATOR');
  assert.equal(result.freshnessState,'CURRENT');
  assert.throws(() => adaptEmergencyShadowProjection({ ...base, scenarios:[{scenarioType:'NOT_EXECUTED'}] }), /missing PARTIALLY_EXECUTED/);
});

test('active shadow cannot have an end time and simulator truth cannot become broker truth', () => {
  const base = { projectionId:'sproj2', incidentId:'i1', shadowCaseId:'shadow2', brokerId:'ALPACA', brokerAccountId:'PAPER-1', environment:'PAPER', positionId:'pos1', instrumentId:'BTCUSD', monitoringStartedAt:'2026-08-16T00:01:00Z', shadowState:'ACTIVE', containsExecutionAmbiguity:false, scenarios:[], projectionTruth:'SIMULATOR', freshnessState:'CURRENT' };
  assert.throws(() => adaptEmergencyShadowProjection({ ...base, monitoringEndedAt:'2026-08-16T00:03:00Z' }), /ACTIVE/);
  assert.throws(() => adaptEmergencyShadowProjection({ ...base, projectionTruth:'BROKER_CONFIRMED' }), /projectionTruth/);
});
