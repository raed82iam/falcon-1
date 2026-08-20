import {
  bindPortfolioSummaryV1,
  bindPositionsV1,
  bindOrderActivityV1,
  bindPerformanceV1
} from '../adapters/fsats-portfolio-v1.js';
import {
  bindOnDemandAnalysisResultV1,
  bindDetailedAssetAnalysisV1,
  presentStrategyCatalogV1
} from '../adapters/fsats-analysis-strategy-v1.js';
import { presentTradingOverlay } from '../presenters/trading-overlay.js';
import { deepFreeze } from '../core/immutable.js';

function optional(value, binder) {
  if (value === null || value === undefined) return null;
  return binder(value);
}

/**
 * Converts exact Application-owned public v1 projections into the stable Web UI data model.
 *
 * This module does not define transport. It intentionally starts after a governed runtime
 * adapter has already obtained the public payloads. This preserves the current boundary:
 * public contract binding may be implemented now while cross-Application transport remains
 * separately governed and unavailable.
 */
export function buildFsatsAuthoritativeData({
  portfolioSummary=null,
  positions=null,
  activity=null,
  performance=null,
  strategyCatalog=null,
  tradingOverlay=null,
  onDemandAnalysis=null,
  detailedAnalysis=null,
  alerts=[],
  apps=[],
  fsatsApps=[],
  advisoryMarkets=[],
  ownerProviderActions=[],
  incidents=[],
  incidentConversation=null,
  owner=null,
  services=[]
} = {}) {
  const summary = optional(portfolioSummary, bindPortfolioSummaryV1);
  const positionProjection = optional(positions, bindPositionsV1);
  const activityProjection = optional(activity, bindOrderActivityV1);
  const performanceProjection = optional(performance, bindPerformanceV1);
  const catalogProjection = optional(strategyCatalog, presentStrategyCatalogV1);
  const overlayProjection = optional(tradingOverlay, presentTradingOverlay);
  const analysisResult = optional(onDemandAnalysis, bindOnDemandAnalysisResultV1);
  const detailed = optional(detailedAnalysis, bindDetailedAssetAnalysisV1);

  const portfolio = summary
    ? {
        totalEquity:summary.totalEquity,
        cash:summary.cash,
        marketValue:summary.marketValue,
        reservedCapital:summary.reservedCapital,
        realizedPnl:summary.realizedPnl,
        unrealizedPnl:summary.unrealizedPnl,
        currency:summary.currency,
        envelope:summary.envelope
      }
    : {};

  const uiPositions = positionProjection?.positions ?? [];
  const uiTrades = activityProjection?.activity ?? [];
  const uiCatalog = catalogProjection?.items?.map(item => ({
    id:item.id,
    kind:'STRATEGY',
    name:item.name,
    schoolId:item.schoolId,
    schoolName:item.schoolName,
    availability:catalogProjection.availabilityState,
    applicability:item.applicability,
    enabled:item.enabled,
    reason:item.reason
  })) ?? [];

  return deepFreeze({
    portfolio,
    positions:uiPositions,
    positionsEnvelope:positionProjection?.envelope ?? null,
    trades:uiTrades,
    activityEnvelope:activityProjection?.envelope ?? null,
    performance:performanceProjection,
    performanceEnvelope:performanceProjection?.envelope ?? null,
    catalog:uiCatalog,
    catalogTruth:catalogProjection ? {
      truthState:catalogProjection.truthState,
      freshnessState:catalogProjection.freshnessState,
      completeness:catalogProjection.completeness,
      availabilityState:catalogProjection.availabilityState
    } : null,
    tradingOverlay:overlayProjection,
    onDemandAnalysis:analysisResult,
    detailedAnalysis:detailed,
    alerts:Array.isArray(alerts) ? alerts : [],
    apps:Array.isArray(apps) ? apps : [],
    fsatsApps:Array.isArray(fsatsApps) ? fsatsApps : [],
    advisoryMarkets:Array.isArray(advisoryMarkets) ? advisoryMarkets : [],
    ownerProviderActions:Array.isArray(ownerProviderActions) ? ownerProviderActions : [],
    incidents:Array.isArray(incidents) ? incidents : [],
    incidentConversation,
    owner:owner ?? {health:'UNAVAILABLE',apps:'—',users:'—',incidents:'—',approvals:'—'},
    services:Array.isArray(services) ? services : [],
    sourceKind:'AUTHORITATIVE_PUBLIC_CONTRACTS',
    transportAuthorityCreated:false
  });
}
