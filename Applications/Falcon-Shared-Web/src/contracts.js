/** Shared Web boundary types and truth-preserving helpers. */
export const TruthState = Object.freeze({
  CURRENT: 'CURRENT',
  LAST_KNOWN: 'LAST_KNOWN',
  STALE: 'STALE',
  UNKNOWN: 'UNKNOWN',
  UNAVAILABLE: 'UNAVAILABLE',
  DEGRADED: 'DEGRADED',
  UNTRUSTED: 'UNTRUSTED',
  SIMULATION: 'SIMULATION',
  REPLAY: 'REPLAY'
});

export const FreshnessState = Object.freeze({
  CURRENT: 'CURRENT', STALE: 'STALE', UNKNOWN: 'UNKNOWN', UNAVAILABLE: 'UNAVAILABLE'
});

export const CompletenessState = Object.freeze({
  COMPLETE: 'COMPLETE', PARTIAL: 'PARTIAL', UNKNOWN: 'UNKNOWN'
});

export const AvailabilityState = Object.freeze({
  AVAILABLE: 'AVAILABLE', UNSUPPORTED: 'UNSUPPORTED', NOT_APPLICABLE: 'NOT_APPLICABLE',
  UNKNOWN: 'UNKNOWN', UNAVAILABLE: 'UNAVAILABLE', DEGRADED: 'DEGRADED'
});

export const ApplicabilityState = Object.freeze({
  APPLICABLE: 'APPLICABLE', NOT_APPLICABLE: 'NOT_APPLICABLE', PARTIAL: 'PARTIAL',
  UNAVAILABLE: 'UNAVAILABLE', UNKNOWN: 'UNKNOWN', NEEDS_CLARIFICATION: 'NEEDS_CLARIFICATION', REJECTED: 'REJECTED'
});

export const AnalysisResultState = Object.freeze({
  COMPLETED: 'COMPLETED', PARTIAL: 'PARTIAL', UNAVAILABLE: 'UNAVAILABLE', UNSUPPORTED: 'UNSUPPORTED',
  NEEDS_CLARIFICATION: 'NEEDS_CLARIFICATION', REJECTED: 'REJECTED'
});

export const OrderState = Object.freeze({
  REQUESTED: 'REQUESTED', ACCEPTED: 'ACCEPTED', PARTIALLY_FILLED: 'PARTIALLY_FILLED', FILLED: 'FILLED',
  CANCEL_REQUESTED: 'CANCEL_REQUESTED', CANCELLED: 'CANCELLED', CANCELED: 'CANCELLED',
  REPLACEMENT_REQUESTED: 'REPLACEMENT_REQUESTED', REPLACED: 'REPLACED', REJECTED: 'REJECTED',
  UNKNOWN_BROKER_OUTCOME: 'UNKNOWN_BROKER_OUTCOME', UNKNOWN: 'UNKNOWN_BROKER_OUTCOME'
});

export const ContractIds = Object.freeze({
  chartRequest: 'FSATS.WebChartDataRequest.v1',
  chartHistory: 'FSATS.WebChartHistoricalProjection.v1',
  chartUpdate: 'FSATS.WebChartUpdateProjection.v1',
  overlayRequest: 'FSATS.WebTradingOverlayRequest.v1',
  overlayProjection: 'FSATS.WebTradingOverlayProjection.v1',
  overlayUpdate: 'FSATS.WebTradingOverlayUpdate.v1',
  onDemandRequest: 'FSATS.WebOnDemandAnalysisRequest.v1',
  onDemandResult: 'FSATS.WebOnDemandAnalysisResult.v1',
  detailedAnalysis: 'FSATS.WebDetailedAssetAnalysisProjection.v1',
  catalogRequest: 'FSATS.WebStrategyCatalogRequest.v1',
  catalogProjection: 'FSATS.WebStrategyCatalogProjection.v1',
  catalogUpdate: 'FSATS.WebStrategyCatalogUpdate.v1',
  portfolioRequest: 'FSATS.WebPortfolioViewRequest.v1',
  portfolioSummary: 'FSATS.WebPortfolioSummaryProjection.v1',
  positions: 'FSATS.WebPositionCollectionProjection.v1',
  activity: 'FSATS.WebOrderTradeActivityProjection.v1',
  performance: 'FSATS.WebPortfolioPerformanceProjection.v1',
  portfolioUpdate: 'FSATS.WebPortfolioProjectionUpdate.v1'
});

export function displayValue(value, fallback = '—') {
  return value === null || value === undefined || value === '' ? fallback : value;
}

export function catalogPresentation(item) {
  if (!item || ['RETIRED', 'REPLACED'].includes(item.availability) || item.removed === true) {
    return { visible: false, enabled: false, reason: null };
  }
  if (item.applicability === ApplicabilityState.APPLICABLE) return { visible: true, enabled: true, reason: null };
  return { visible: true, enabled: false, reason: item.reason ?? null };
}

export function assertNoRegulatoryClaims(text) {
  const forbidden = /\b(licensed|regulated|authorized by|CMA|capital market authority|مرخص(?:ة|ون)?|منظم(?:ة|ون)?|هيئة السوق المالية)\b/i;
  return !forbidden.test(String(text ?? ''));
}
