const POSITION_PROTECTION = new Set([
  'BROKER_CONFIRMED_PROTECTED',
  'PROTECTION_UNKNOWN_OR_AMBIGUOUS',
  'INTENTIONALLY_RETAINED_WITHOUT_CURRENT_BROKER_PROTECTION',
  'UNEXPECTEDLY_MISSING_OR_INCOMPLETE_PROTECTION',
  'RECONCILIATION_REQUIRED',
  'NOT_APPLICABLE'
]);

const FOLLOWUP = new Set(['NONE','RECOMMENDED','REQUIRED']);
const ORDER_TRUTH = new Set([
  'BROKER_CONFIRMED_WORKING','BROKER_CONFIRMED_REJECTED','BROKER_CONFIRMED_PARTIALLY_FILLED',
  'BROKER_CONFIRMED_FILLED','BROKER_CONFIRMED_CANCELLED','OUTCOME_UNKNOWN_OR_AMBIGUOUS','RECONCILIATION_REQUIRED'
]);
const SHADOW_STATE = new Set(['ACTIVE','RECONCILING','ENDED_RECONCILED','ENDED_UNRESOLVED']);
const SHADOW_TRUTH = new Set(['SIMULATOR','REPLAY','SYNTHETIC','TEST']);
const FRESHNESS = new Set(['CURRENT','STALE','UNKNOWN','UNAVAILABLE']);
const TRUTH = new Set(['CURRENT','LAST_KNOWN','UNKNOWN','UNAVAILABLE']);

function text(value, name) {
  if (typeof value !== 'string' || value.trim().length === 0) throw new TypeError(`${name} is required`);
  return value;
}

function account(value) {
  if (!value || typeof value !== 'object') throw new TypeError('account is required');
  return Object.freeze({
    brokerId:text(value.brokerId,'account.brokerId'),
    brokerAccountId:text(value.brokerAccountId,'account.brokerAccountId'),
    environment:text(value.environment,'account.environment')
  });
}

function enumValue(value, allowed, name) {
  if (!allowed.has(value)) throw new TypeError(`invalid ${name}`);
  return value;
}

function actions(value) {
  if (value == null) return Object.freeze([]);
  if (!Array.isArray(value)) throw new TypeError('orderedActions must be an array');
  return Object.freeze(value.map((item,index) => Object.freeze({ ...item, action:item?.action ?? item?.type ?? text(item,`orderedActions[${index}]`) })));
}

export function adaptAffectedPositionProjection(input = {}) {
  const protectionState = enumValue(input.protectionState, POSITION_PROTECTION, 'protectionState');
  const followupRequirement = enumValue(input.followupRequirement, FOLLOWUP, 'followupRequirement');
  const orderedActions = actions(input.orderedActions);
  if (protectionState === 'UNEXPECTEDLY_MISSING_OR_INCOMPLETE_PROTECTION' && followupRequirement !== 'REQUIRED') {
    throw new TypeError('unexpectedly missing protection requires REQUIRED follow-up');
  }
  if (followupRequirement === 'REQUIRED' && orderedActions.length === 0) throw new TypeError('REQUIRED follow-up needs an ordered action');
  return Object.freeze({
    kind:'POSITION', projectionId:text(input.projectionId,'projectionId'), incidentId:text(input.incidentId,'incidentId'),
    account:account(input.account), position:input.position ?? null, instrument:input.instrument ?? null,
    lastBrokerConfirmedAt:input.lastBrokerConfirmedAt ?? null, protectionState, followupRequirement,
    followupReasonCode:input.followupReasonCode ?? null, orderedActions,
    emergencyShadowMonitoringActive:input.emergencyShadowMonitoringActive === true,
    shadowCaseId:input.shadowCaseId ?? null, incidentState:input.incidentState ?? null,
    truthState:enumValue(input.truthState, TRUTH, 'truthState'), freshnessState:enumValue(input.freshnessState, FRESHNESS, 'freshnessState'),
    evidenceReference:input.evidenceReference ?? null, asOfTime:input.asOfTime ?? null
  });
}

export function adaptAffectedOrderProjection(input = {}) {
  const orderTruthState = enumValue(input.orderTruthState, ORDER_TRUTH, 'orderTruthState');
  const followupRequirement = enumValue(input.followupRequirement, FOLLOWUP, 'followupRequirement');
  if ((orderTruthState === 'OUTCOME_UNKNOWN_OR_AMBIGUOUS' || orderTruthState === 'RECONCILIATION_REQUIRED') && followupRequirement === 'NONE') {
    throw new TypeError('ambiguous/reconciliation order requires follow-up');
  }
  return Object.freeze({
    kind:'ORDER', projectionId:text(input.projectionId,'projectionId'), incidentId:text(input.incidentId,'incidentId'),
    account:account(input.account), order:input.order ?? null, instrument:input.instrument ?? null,
    lastBrokerConfirmedAt:input.lastBrokerConfirmedAt ?? null, orderTruthState, followupRequirement,
    followupReasonCode:input.followupReasonCode ?? null, orderedActions:actions(input.orderedActions),
    truthState:enumValue(input.truthState, TRUTH, 'truthState'), freshnessState:enumValue(input.freshnessState, FRESHNESS, 'freshnessState'),
    evidenceReference:input.evidenceReference ?? null, asOfTime:input.asOfTime ?? null
  });
}

export function adaptEmergencyShadowProjection(input = {}) {
  const shadowState = enumValue(input.shadowState, SHADOW_STATE, 'shadowState');
  const positionId = input.positionId ?? null;
  const sourceOrderId = input.sourceOrderId ?? null;
  if (!positionId && !sourceOrderId) throw new TypeError('positionId or sourceOrderId is required');
  if (shadowState === 'ACTIVE' && input.monitoringEndedAt) throw new TypeError('ACTIVE shadow cannot have monitoringEndedAt');
  if (shadowState.startsWith('ENDED_') && !input.monitoringEndedAt) throw new TypeError('ended shadow requires monitoringEndedAt');
  const scenarios = Array.isArray(input.scenarios) ? input.scenarios.map(x => Object.freeze({ ...x })) : [];
  if (input.containsExecutionAmbiguity === true) {
    if (!sourceOrderId) throw new TypeError('ambiguous execution requires sourceOrderId');
    const names = new Set(scenarios.map(s => s.scenarioType ?? s.type));
    for (const required of ['NOT_EXECUTED','PARTIALLY_EXECUTED','FULLY_EXECUTED']) if (!names.has(required)) throw new TypeError(`missing ${required} scenario`);
  }
  return Object.freeze({
    kind:'SHADOW', projectionId:text(input.projectionId,'projectionId'), incidentId:text(input.incidentId,'incidentId'), shadowCaseId:text(input.shadowCaseId,'shadowCaseId'),
    brokerId:text(input.brokerId,'brokerId'), brokerAccountId:text(input.brokerAccountId,'brokerAccountId'), environment:text(input.environment,'environment'),
    positionId, sourceOrderId, instrumentId:text(input.instrumentId,'instrumentId'), lastBrokerConfirmedAt:input.lastBrokerConfirmedAt ?? null,
    monitoringStartedAt:text(input.monitoringStartedAt,'monitoringStartedAt'), monitoringEndedAt:input.monitoringEndedAt ?? null,
    shadowState, containsExecutionAmbiguity:input.containsExecutionAmbiguity === true, scenarios:Object.freeze(scenarios),
    protectionClassificationProjectionReference:input.protectionClassificationProjectionReference ?? null,
    asOfTime:input.asOfTime ?? null, projectionTruth:enumValue(input.projectionTruth, SHADOW_TRUTH, 'projectionTruth'),
    freshnessState:enumValue(input.freshnessState, FRESHNESS, 'freshnessState'), provenanceReference:input.provenanceReference ?? null,
    evidenceReference:input.evidenceReference ?? null
  });
}
