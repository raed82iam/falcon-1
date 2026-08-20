import { ContractIds } from '../contracts.js';
import { deepFreeze } from '../core/immutable.js';

const truthStates = new Set(['CURRENT','LAST_KNOWN','STALE','UNKNOWN','SIMULATION','REPLAY']);
const freshnessStates = new Set(['CURRENT','STALE','UNKNOWN','UNAVAILABLE']);
const completenessStates = new Set(['COMPLETE','PARTIAL','UNKNOWN']);
const availabilityStates = new Set(['AVAILABLE','UNSUPPORTED','NOT_APPLICABLE','UNKNOWN','UNAVAILABLE','DEGRADED']);
const orderStates = new Set(['REQUESTED','ACCEPTED','PARTIALLY_FILLED','FILLED','CANCEL_REQUESTED','CANCELLED','REPLACEMENT_REQUESTED','REPLACED','REJECTED','UNKNOWN_BROKER_OUTCOME']);
const updateKinds = new Set(['ORDINARY','CORRECTION','SUPERSESSION']);
const noSourceAvailability = new Set(['UNSUPPORTED','NOT_APPLICABLE']);
const forbiddenApplicationIdentityFields = Object.freeze(['userId','customerId','contactId','principalId','email','phone']);

const hasOwn = (object, key) => Object.prototype.hasOwnProperty.call(object ?? {}, key);

function requiredString(value, name) {
  if (typeof value !== 'string' || value.length === 0) throw new TypeError(`${name} must be a non-empty string`);
  return value;
}
function requiredUppercaseIdentity(value, name) {
  requiredString(value,name);
  if (value !== value.toUpperCase()) throw new TypeError(`${name} must be a canonical uppercase identity`);
  return value;
}
function nullableOpaqueToken(value, name) {
  if (value === null || value === undefined) return null;
  if (typeof value !== 'string' || value.length === 0) throw new TypeError(`${name} must be null or a non-empty opaque string`);
  return value;
}
function nullableNumber(value, name) {
  if (value === null) return null;
  if (typeof value !== 'number' || !Number.isFinite(value)) throw new TypeError(`${name} must be a finite number or null`);
  return value;
}
function requiredNullableNumber(object, field, prefix='') {
  if (!hasOwn(object, field)) throw new TypeError(`${prefix}${field} is required`);
  return nullableNumber(object[field],`${prefix}${field}`);
}
function assertAllNull(object, fields, reason) {
  for (const field of fields) if (object[field] !== null) throw new TypeError(`${reason}: ${field} must be null`);
}
function assertEmpty(array, reason) {
  if (array.length !== 0) throw new TypeError(reason);
}
function assertPage(page, name='page') {
  if (!page || typeof page !== 'object') throw new TypeError(`${name} is required`);
  if (!hasOwn(page,'continuationToken')) throw new TypeError(`${name}.continuationToken is required`);
  if (page.continuationToken !== null && (typeof page.continuationToken !== 'string' || page.continuationToken.length===0)) throw new TypeError(`${name}.continuationToken must be null or non-empty string`);
  if (typeof page.hasMore !== 'boolean') throw new TypeError(`${name}.hasMore must be boolean`);
  if (!Number.isInteger(page.pageSize) || page.pageSize <= 0) throw new TypeError(`${name}.pageSize must be a positive integer`);
  if (page.hasMore && !page.continuationToken) throw new TypeError(`${name}.continuationToken is required when hasMore=true`);
  return page;
}

export function assertBrokerAccountScope(account) {
  if (!account || typeof account !== 'object') throw new TypeError('account is required');
  requiredUppercaseIdentity(account.brokerId,'account.brokerId');
  requiredString(account.brokerAccountId,'account.brokerAccountId');
  requiredUppercaseIdentity(account.environment,'account.environment');
  return account;
}

export function bindPortfolioViewRequestV1(payload) {
  if (!payload || typeof payload !== 'object') throw new TypeError('portfolio request is required');
  if (payload.contractId !== undefined && payload.contractId !== ContractIds.portfolioRequest) throw new TypeError(`unexpected contractId: ${payload.contractId}`);
  if (payload.version !== undefined && String(payload.version) !== '1') throw new TypeError(`unsupported contract version: ${payload.version}`);
  for (const field of forbiddenApplicationIdentityFields) {
    if (hasOwn(payload,field)) throw new TypeError(`${field} is not legal in FSATS portfolio request payloads`);
  }
  requiredString(payload.requestId,'requestId');
  requiredString(payload?.correlation?.value,'correlation.value');
  requiredString(payload.requestedAt,'requestedAt');
  if (!Array.isArray(payload.brokerAccounts) || payload.brokerAccounts.length === 0) throw new TypeError('brokerAccounts must be a non-empty array');
  const seen = new Set();
  payload.brokerAccounts.forEach((account,index)=>{
    assertBrokerAccountScope(account);
    const key = `${account.brokerId}\u0000${account.brokerAccountId}\u0000${account.environment}`;
    if (seen.has(key)) throw new TypeError(`brokerAccounts[${index}] duplicates an exact broker-account scope`);
    seen.add(key);
  });
  if (payload.pageSize !== null && payload.pageSize !== undefined && (!Number.isInteger(payload.pageSize) || payload.pageSize <= 0)) throw new TypeError('pageSize must be null or a positive integer');
  nullableOpaqueToken(payload.positionContinuationToken,'positionContinuationToken');
  nullableOpaqueToken(payload.activityContinuationToken,'activityContinuationToken');
  nullableOpaqueToken(payload.performanceContinuationToken,'performanceContinuationToken');
  return deepFreeze(structuredClone(payload));
}

export function assertPortfolioEnvelope(envelope, expectedContractId) {
  if (!envelope || typeof envelope !== 'object') throw new TypeError('projection envelope is required');
  requiredString(envelope.projectionId,'envelope.projectionId');
  if (envelope.contractId !== expectedContractId) throw new TypeError(`unexpected contractId: ${envelope.contractId}`);
  if (String(envelope.version) !== '1') throw new TypeError(`unsupported contract version: ${envelope.version}`);
  assertBrokerAccountScope(envelope.account);
  requiredString(envelope.asOfTime,'envelope.asOfTime');
  if (!truthStates.has(envelope.truthState)) throw new TypeError(`invalid truthState: ${envelope.truthState}`);
  if (!freshnessStates.has(envelope.freshnessState)) throw new TypeError(`invalid freshnessState: ${envelope.freshnessState}`);
  if (!completenessStates.has(envelope.completeness)) throw new TypeError(`invalid completeness: ${envelope.completeness}`);
  if (!availabilityStates.has(envelope.availabilityState)) throw new TypeError(`invalid availabilityState: ${envelope.availabilityState}`);
  requiredString(envelope.evidenceReference,'envelope.evidenceReference');
  requiredString(envelope.reasonCode,'envelope.reasonCode');
  return envelope;
}

export function bindPortfolioSummaryV1(payload) {
  const envelope = assertPortfolioEnvelope(payload?.envelope, ContractIds.portfolioSummary);
  requiredString(payload.currency,'currency');
  const fields = ['totalEquity','cash','marketValue','reservedCapital','realizedPnl','unrealizedPnl'];
  for (const field of fields) requiredNullableNumber(payload,field);
  if (noSourceAvailability.has(envelope.availabilityState)) assertAllNull(payload,fields,'NO_SOURCE_PORTFOLIO_SUMMARY');
  return deepFreeze(structuredClone(payload));
}
export function bindPositionsV1(payload) {
  const envelope = assertPortfolioEnvelope(payload?.envelope, ContractIds.positions);
  if (!Array.isArray(payload.positions)) throw new TypeError('positions must be an array');
  if (noSourceAvailability.has(envelope.availabilityState)) assertEmpty(payload.positions,'NO_SOURCE_POSITIONS_MUST_BE_EMPTY');
  payload.positions.forEach((item,index)=>{
    requiredString(item?.position?.value,`positions[${index}].position.value`);
    requiredString(item?.instrument?.value,`positions[${index}].instrument.value`);
    requiredString(item.currency,`positions[${index}].currency`);
    if (!truthStates.has(item.truthState)) throw new TypeError(`invalid position truthState: ${item.truthState}`);
    if (!freshnessStates.has(item.freshnessState)) throw new TypeError(`invalid position freshnessState: ${item.freshnessState}`);
    requiredString(item.reasonCode,`positions[${index}].reasonCode`);
    ['quantity','averageCost','marketPrice','marketValue','unrealizedPnl'].forEach(f=>requiredNullableNumber(item,f,`positions[${index}].`));
  });
  assertPage(payload.page);
  return deepFreeze(structuredClone(payload));
}
export function bindOrderActivityV1(payload) {
  const envelope = assertPortfolioEnvelope(payload?.envelope, ContractIds.activity);
  if (!Array.isArray(payload.activity)) throw new TypeError('activity must be an array');
  if (noSourceAvailability.has(envelope.availabilityState)) assertEmpty(payload.activity,'NO_SOURCE_ACTIVITY_MUST_BE_EMPTY');
  payload.activity.forEach((item,index)=>{
    requiredString(item?.order?.value,`activity[${index}].order.value`);
    requiredString(item?.instrument?.value,`activity[${index}].instrument.value`);
    if (!orderStates.has(item.state)) throw new TypeError(`invalid order state: ${item.state}`);
    requiredString(item.currency,`activity[${index}].currency`);
    requiredString(item.effectiveAt,`activity[${index}].effectiveAt`);
    if (!truthStates.has(item.truthState)) throw new TypeError(`invalid activity truthState: ${item.truthState}`);
    if (!freshnessStates.has(item.freshnessState)) throw new TypeError(`invalid activity freshnessState: ${item.freshnessState}`);
    requiredString(item.reasonCode,`activity[${index}].reasonCode`);
    ['requestedQuantity','filledQuantity','averageFillPrice'].forEach(f=>requiredNullableNumber(item,f,`activity[${index}].`));
  });
  assertPage(payload.page);
  return deepFreeze(structuredClone(payload));
}
export function bindPerformanceV1(payload) {
  const envelope = assertPortfolioEnvelope(payload?.envelope, ContractIds.performance);
  requiredString(payload.periodStart,'periodStart');
  requiredString(payload.periodEnd,'periodEnd');
  requiredString(payload.currency,'currency');
  const fields = ['openingEquity','closingEquity','realizedPnl','unrealizedPnl','netPnl','returnPercent'];
  for (const field of fields) requiredNullableNumber(payload,field);
  if (!Array.isArray(payload.history)) throw new TypeError('history must be an array');
  if (noSourceAvailability.has(envelope.availabilityState)) {
    assertAllNull(payload,fields,'NO_SOURCE_PERFORMANCE');
    assertEmpty(payload.history,'NO_SOURCE_PERFORMANCE_HISTORY_MUST_BE_EMPTY');
  }
  payload.history.forEach((point,index)=>{
    requiredString(point?.effectiveAt,`history[${index}].effectiveAt`);
    if (!truthStates.has(point?.truthState)) throw new TypeError(`invalid history truthState: ${point?.truthState}`);
    if (!freshnessStates.has(point?.freshnessState)) throw new TypeError(`invalid history freshnessState: ${point?.freshnessState}`);
    requiredString(point?.reasonCode,`history[${index}].reasonCode`);
    ['equity','netPnl','returnPercent'].forEach(f=>requiredNullableNumber(point,f,`history[${index}].`));
  });
  assertPage(payload.page);
  return deepFreeze(structuredClone(payload));
}
export function bindPortfolioUpdateV1(payload) {
  if (!payload || typeof payload !== 'object') throw new TypeError('portfolio update is required');
  requiredString(payload.updateId,'updateId');
  if (!Number.isInteger(payload.updateSequence) || payload.updateSequence < 0) throw new TypeError('updateSequence must be a non-negative integer');
  if (!updateKinds.has(payload.updateKind)) throw new TypeError(`invalid updateKind: ${payload.updateKind}`);
  requiredString(payload?.correlation?.value,'correlation.value');
  assertBrokerAccountScope(payload.account);
  if (!Array.isArray(payload.changedProjectionContractIds) || payload.changedProjectionContractIds.length===0 || payload.changedProjectionContractIds.some(v=>typeof v!=='string'||v.length===0)) throw new TypeError('changedProjectionContractIds must be a non-empty string array');
  requiredString(payload.projectionVersion,'projectionVersion');
  requiredString(payload.effectiveAt,'effectiveAt');
  if (!truthStates.has(payload.truthState)) throw new TypeError(`invalid truthState: ${payload.truthState}`);
  if (!freshnessStates.has(payload.freshnessState)) throw new TypeError(`invalid freshnessState: ${payload.freshnessState}`);
  requiredString(payload.evidenceReference,'evidenceReference');
  requiredString(payload.reasonCode,'reasonCode');
  const corrects = payload.correctsUpdateId ?? null;
  const supersedes = payload.supersedesUpdateId ?? null;
  if (payload.updateKind === 'CORRECTION' && (typeof corrects !== 'string' || corrects.length===0)) throw new TypeError('CORRECTION requires correctsUpdateId');
  if (payload.updateKind === 'SUPERSESSION' && (typeof supersedes !== 'string' || supersedes.length===0)) throw new TypeError('SUPERSESSION requires supersedesUpdateId');
  if (payload.updateKind === 'ORDINARY' && (corrects !== null || supersedes !== null)) throw new TypeError('ORDINARY requires null lineage fields');
  return deepFreeze(structuredClone(payload));
}
