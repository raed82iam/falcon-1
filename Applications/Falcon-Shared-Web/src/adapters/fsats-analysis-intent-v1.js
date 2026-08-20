import { ContractIds } from '../contracts.js';
import { deepFreeze } from '../core/immutable.js';
import { bindOnDemandAnalysisResultV1 as bindCanonicalOnDemandAnalysisResultV1 } from './fsats-analysis-strategy-v1.js';

function nonEmpty(value, name) {
  if (typeof value !== 'string' || value.trim() === '') throw new TypeError(`${name} must be a non-empty string`);
  return value;
}

function optionalString(value, name) {
  if (value === null || value === undefined) return null;
  return nonEmpty(value,name);
}

export function createOnDemandAnalysisRequestV1({
  requestId,
  correlationId,
  requestedInstrumentReference,
  marketOrVenueHint = null,
  assetClassHint = null,
  analysisIntent,
  requestedAt,
  entitlementReference = null
} = {}) {
  nonEmpty(requestId,'requestId');
  nonEmpty(correlationId,'correlationId');
  nonEmpty(requestedInstrumentReference,'requestedInstrumentReference');
  nonEmpty(analysisIntent,'analysisIntent');
  nonEmpty(requestedAt,'requestedAt');
  optionalString(marketOrVenueHint,'marketOrVenueHint');
  optionalString(assetClassHint,'assetClassHint');
  optionalString(entitlementReference,'entitlementReference');

  return deepFreeze({
    contractId:ContractIds.onDemandRequest,
    requestId,
    correlationId,
    requestingApplicationId:'SHARED_WEB',
    requestedInstrumentReference,
    marketOrVenueHint,
    assetClassHint,
    analysisIntent,
    requestedAt,
    entitlementReference,
    universeMutationRequested:false,
    strategyActivationRequested:false,
    capitalReservationRequested:false,
    orderIntentCreated:false,
    executionAuthorityRequested:false
  });
}

/**
 * Canonical result binding delegates to the exact current v1 analysis semantics in
 * fsats-analysis-strategy-v1.js. This prevents two Web adapters from silently
 * accepting different meanings for the same FSATS.WebOnDemandAnalysisResult.v1.
 */
export function bindOnDemandAnalysisResultV1(result) {
  return bindCanonicalOnDemandAnalysisResultV1(result);
}

export function presentOnDemandAnalysisResultV1(result) {
  let bound;
  try {
    bound = bindCanonicalOnDemandAnalysisResultV1(result);
  } catch {
    return deepFreeze({ state:'UNAVAILABLE', projection:null, limitations:[], clarificationCandidates:[], reasonCode:'MALFORMED_APPLICATION_RESULT' });
  }

  const completedOrPartial = bound.resultState === 'COMPLETED' || bound.resultState === 'PARTIAL';
  return deepFreeze({
    requestId:bound.requestId,
    correlationId:bound.correlationId ?? null,
    analysisResultId:bound.analysisResultId,
    resolvedInstrumentIdentity:bound.resolvedInstrumentIdentity ?? null,
    analysisIntent:bound.analysisIntent,
    state:bound.resultState,
    projection:completedOrPartial ? (bound.analysisProjection ?? null) : null,
    asOfTime:bound.asOfTime,
    inputTruth:bound.inputTruthFreshnessSummary ?? null,
    confidenceOrStrength:bound.confidenceOrStrength ?? null,
    limitations:bound.limitations ?? [],
    clarificationCandidates:bound.clarificationCandidates ?? [],
    reasonCode:bound.reasonCode ?? null
  });
}
