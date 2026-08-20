import { TruthState, AnalysisResultState, displayValue, catalogPresentation } from './contracts.js';

const envelopeOf = projection => projection?.envelope ?? null;

export function presentPortfolio(projection) {
  if (!projection) return { state:'unavailable', values:null };
  const envelope = envelopeOf(projection);
  const truth = envelope?.truthState ?? projection.truth ?? TruthState.UNKNOWN;
  const availability = envelope?.availabilityState ?? null;
  if (truth === TruthState.UNAVAILABLE || availability === 'UNAVAILABLE') {
    return { state:'unavailable', values:null, asOf:envelope?.asOfTime ?? projection.asOfTime ?? null };
  }

  return {
    state: truth,
    freshness: envelope?.freshnessState ?? projection.freshnessState ?? null,
    completeness: envelope?.completeness ?? projection.completeness ?? null,
    availability,
    reasonCode: envelope?.reasonCode ?? projection.reasonCode ?? null,
    values: {
      equity: displayValue(projection.totalEquity ?? projection.equity),
      cash: displayValue(projection.cash),
      marketValue: displayValue(projection.marketValue),
      reservedCapital: displayValue(projection.reservedCapital),
      realizedPnl: displayValue(projection.realizedPnl),
      unrealizedPnl: displayValue(projection.unrealizedPnl),
      available: displayValue(projection.availableFunds ?? projection.buyingPower)
    },
    currency: projection.currency ?? null,
    asOf: envelope?.asOfTime ?? projection.asOfTime ?? null
  };
}

export function presentCatalog(items=[]) {
  return items.map(item => ({ ...item, presentation: catalogPresentation(item) })).filter(item => item.presentation.visible);
}

export function presentDetailedAnalysis(result) {
  if (!result) return { state:AnalysisResultState.UNAVAILABLE, sections:[] };
  const state = result.resultState ?? AnalysisResultState.UNAVAILABLE;
  if (state !== AnalysisResultState.COMPLETED && state !== AnalysisResultState.PARTIAL) return { state, sections:[] };
  const projection = result.detailedProjection ?? {};
  return {
    state,
    horizons: projection.horizonViews ?? [],
    strategies: projection.strategyViews ?? [],
    schools: projection.schoolViews ?? [],
    synthesis: projection.synthesis ?? null,
    asOf: projection.asOfTime ?? result.asOfTime ?? null,
    truth: projection.overallTruthState ?? result.truth ?? TruthState.UNKNOWN
  };
}

export function presentOrderActivity(items=[]) {
  return items.map(item => ({
    id:item?.order?.value ?? item.orderId ?? item.executionId ?? 'UNKNOWN',
    instrument:item?.instrument?.value ?? item.instrument ?? '—',
    state:item.state ?? 'UNKNOWN_BROKER_OUTCOME',
    side:item.side ?? '—',
    requestedQuantity:displayValue(item.requestedQuantity ?? item.quantity),
    filledQuantity:displayValue(item.filledQuantity),
    averageFillPrice:displayValue(item.averageFillPrice ?? item.price),
    currency:item.currency ?? null,
    truth:item.truthState ?? TruthState.UNKNOWN,
    freshness:item.freshnessState ?? null,
    reasonCode:item.reasonCode ?? null,
    asOf:item.effectiveAt ?? item.asOfTime ?? null
  }));
}
