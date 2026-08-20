import { WebMarketProviderRoutes } from './ports/web-market-data-port.js';
import { PendingMarketDataDestinations } from './market-data-destinations.pending.js';

/**
 * Shared Web presentation-only market-data sourcing plan.
 *
 * This is routing preference/configuration, not external-connectivity authority.
 * Exact destinations remain fail-closed until Web principal/policy/credential
 * binding and governed verification complete. No record in this file may become
 * FSATS input.
 */
const destinationByFcr = fcr => PendingMarketDataDestinations.find(item => item.fcr === fcr) ?? null;
const nonEmpty = value => typeof value === 'string' && value.trim().length > 0;

export const MarketDataPlan = Object.freeze({
  sourcingPolicy:Object.freeze({
    preferred:'WEB_INDEPENDENT_GOVERNED_PRESENTATION_SOURCE',
    fallback:'SHARED_CONSTRAINED_POOL_ONLY_WHEN_NO_SUITABLE_INDEPENDENT_SOURCE',
    sharedPoolWebHardCeiling:0.5,
    sharedPoolFsatsReservedShare:0.5,
    plainUrlWithoutDocumentedConstraintRequiresSplit:false,
    providerNameAloneDefinesSharedPool:false,
    unknownQuotaScopeImpliesIndependentCapacity:false,
    missingQuotaDimensionsImplyUsableCapacity:false,
    multipleQuotaDimensionsEvaluatedIndependently:true,
    oddDiscreteQuotaRemainder:'UNALLOCATED_SAFETY_REMAINDER'
  }),
  US_EQUITIES: Object.freeze({
    universe: Object.freeze({ provider:'ALPACA', route:destinationByFcr('FCR-0196'), purpose:'ACTIVE_US_EQUITY_UNIVERSE' }),
    history: Object.freeze({ provider:'ALPACA', route:destinationByFcr('FCR-0197'), purpose:'MULTI_SYMBOL_HISTORICAL_BARS' }),
    live: Object.freeze({
      mode:'DYNAMIC_WINDOW',
      providers:Object.freeze(['ALPACA_IEX','FINNHUB']),
      routes:Object.freeze([WebMarketProviderRoutes.ALPACA_IEX,WebMarketProviderRoutes.FINNHUB]),
      priority:'VISIBLE_PORTFOLIO_INCIDENT_WATCHLIST_REQUESTED'
    })
  }),
  CRYPTO_SPOT: Object.freeze({
    universe: Object.freeze({ provider:'BINANCE', route:destinationByFcr('FCR-0198'), purpose:'SPOT_SYMBOL_UNIVERSE' }),
    history: Object.freeze({ provider:'BINANCE', route:destinationByFcr('FCR-0199'), purpose:'SELECTED_SYMBOL_KLINES' }),
    live: Object.freeze({
      mode:'BROAD_MARKET_PLUS_ON_DEMAND_SECONDARY',
      broadMarketRoute:destinationByFcr('FCR-0200'),
      providers:Object.freeze(['BINANCE','COINBASE','BYBIT']),
      routes:Object.freeze([WebMarketProviderRoutes.BINANCE,WebMarketProviderRoutes.COINBASE,WebMarketProviderRoutes.BYBIT])
    })
  })
});

export function decideQuotaCoordination({
  hasSuitableIndependentPresentationSource,
  webQuotaPoolId=null,
  fsapmaQuotaPoolId=null,
  quotaPoolConstrained=false,
  documentedLimitKnown=false
} = {}) {
  if (hasSuitableIndependentPresentationSource === true) {
    return Object.freeze({ mode:'INDEPENDENT_SOURCE', webMaxShare:null, fsapmaReservedShare:null, reason:'NO_SHARED_POOL_REQUIRED' });
  }

  // FCR-0220: UNKNOWN_QUOTA_SCOPE != INDEPENDENT_CAPACITY. If either side's
  // provider-enforced pool identity is unknown, Web must not infer that the
  // capacities are independent merely because equality cannot be proven.
  if (!nonEmpty(webQuotaPoolId) || !nonEmpty(fsapmaQuotaPoolId)) {
    return Object.freeze({ mode:'FAIL_CLOSED', webMaxShare:0, fsapmaReservedShare:1, reason:'QUOTA_POOL_IDENTITY_UNKNOWN' });
  }

  const samePool = webQuotaPoolId === fsapmaQuotaPoolId;
  if (!samePool) {
    return Object.freeze({ mode:'NO_SHARED_POOL', webMaxShare:null, fsapmaReservedShare:null, reason:'QUOTA_POOL_NOT_SHARED' });
  }

  if (quotaPoolConstrained !== true) {
    return Object.freeze({ mode:'NO_SPLIT_REQUIRED', webMaxShare:null, fsapmaReservedShare:null, reason:'POOL_NOT_DOCUMENTED_AS_CONSTRAINED' });
  }

  if (documentedLimitKnown !== true) {
    return Object.freeze({ mode:'FAIL_CLOSED', webMaxShare:0, fsapmaReservedShare:1, reason:'SHARED_CONSTRAINED_LIMIT_UNKNOWN' });
  }

  return Object.freeze({ mode:'SHARED_POOL_FALLBACK', webMaxShare:0.5, fsapmaReservedShare:0.5, reason:'FCR_0220_SHARED_CONSTRAINED_POOL' });
}

export function allocateSharedDiscreteQuota(totalUnits) {
  if (!Number.isSafeInteger(totalUnits) || totalUnits < 0) {
    return Object.freeze({ mode:'FAIL_CLOSED', webMaxUnits:0, fsapmaMaxUnits:0, unallocatedSafetyRemainder:null, reason:'INVALID_OR_UNKNOWN_DISCRETE_LIMIT' });
  }
  const half = Math.floor(totalUnits / 2);
  return Object.freeze({
    mode:'SHARED_DISCRETE_50_50_CEILING',
    webMaxUnits:half,
    fsapmaMaxUnits:half,
    unallocatedSafetyRemainder:totalUnits - half - half,
    reason:'FCR_0220_ODD_REMAINDER_UNALLOCATED'
  });
}

/**
 * Evaluate provider-enforced quota dimensions independently. This models Web's
 * planning/budget boundary only; it does not reserve FSAPMA capacity or enforce
 * Application internals.
 */
export function decideQuotaDimensions({ hasSuitableIndependentPresentationSource, dimensions=[] } = {}) {
  if (hasSuitableIndependentPresentationSource === true) {
    return Object.freeze({ mode:'INDEPENDENT_SOURCE', dimensions:Object.freeze([]), reason:'NO_SHARED_POOL_REQUIRED' });
  }

  if (!Array.isArray(dimensions)) {
    return Object.freeze({ mode:'FAIL_CLOSED', dimensions:Object.freeze([]), reason:'QUOTA_DIMENSIONS_INVALID' });
  }
  if (dimensions.length === 0) {
    return Object.freeze({ mode:'FAIL_CLOSED', dimensions:Object.freeze([]), reason:'QUOTA_DIMENSIONS_REQUIRED' });
  }

  const seenDimensionIds = new Set();
  const results = dimensions.map((dimension = {}) => {
    const dimensionId = String(dimension.dimensionId ?? '').trim();
    const invalidDimensionIdentity = !dimensionId || seenDimensionIds.has(dimensionId);
    if (dimensionId) seenDimensionIds.add(dimensionId);

    const coordination = invalidDimensionIdentity
      ? Object.freeze({ mode:'FAIL_CLOSED', webMaxShare:0, fsapmaReservedShare:1, reason:dimensionId ? 'DUPLICATE_QUOTA_DIMENSION_ID' : 'QUOTA_DIMENSION_ID_REQUIRED' })
      : decideQuotaCoordination({
          hasSuitableIndependentPresentationSource:false,
          webQuotaPoolId:dimension.webQuotaPoolId ?? null,
          fsapmaQuotaPoolId:dimension.fsapmaQuotaPoolId ?? null,
          quotaPoolConstrained:dimension.quotaPoolConstrained === true,
          documentedLimitKnown:dimension.documentedLimitKnown === true
        });

    const sharedDiscrete = coordination.mode === 'SHARED_POOL_FALLBACK' && dimension.discrete === true;
    const unitAllocation = sharedDiscrete
      ? allocateSharedDiscreteQuota(dimension.totalUnits)
      : null;

    const failClosed = coordination.mode === 'FAIL_CLOSED' || unitAllocation?.mode === 'FAIL_CLOSED';
    return Object.freeze({
      dimensionId,
      coordination,
      unitAllocation,
      failClosed
    });
  });

  return Object.freeze({
    mode:results.some(result => result.failClosed) ? 'PARTIAL_OR_FAIL_CLOSED' : 'DIMENSIONAL_EVALUATION',
    dimensions:Object.freeze(results),
    reason:null
  });
}

export function normalizePresentationObservation(input = {}) {
  return Object.freeze({
    instrumentId: input.instrumentId ?? null,
    providerSymbol: input.providerSymbol ?? null,
    market: input.market ?? null,
    price: input.price ?? null,
    bid: input.bid ?? null,
    ask: input.ask ?? null,
    volume: input.volume ?? null,
    observedAt: input.observedAt ?? null,
    source: input.source ?? null,
    freshness: input.freshness ?? 'UNKNOWN',
    presentationOnly: true,
    eligibleForFsatsInput: false
  });
}
