import test from 'node:test';
import assert from 'node:assert/strict';
import { bindDetailedAssetAnalysisV1, bindOnDemandAnalysisResultV1, bindStrategyCatalogProjectionV1 } from '../src/adapters/fsats-analysis-strategy-v1.js';

const truthSummary = (overrides={}) => ({ truthState:'CURRENT', freshnessState:'CURRENT', completeness:'COMPLETE', limitations:[], ...overrides });
const synthesis = (overrides={}) => ({ synthesisState:'PARTIAL', agreements:[], disagreements:[], unresolvedConflicts:[], boundedCombinedExplanation:'Bounded synthesis', contributingOutputReferences:[], limitations:[], ...overrides });
const detailed = (overrides={}) => ({
  resolvedInstrumentIdentity:{ value:'AAPL' },
  analysisResultId:'AR-1',
  asOfTime:'2026-08-16T00:00:00Z',
  overallTruthState:'CURRENT',
  inputTruthFreshnessSummary:truthSummary(),
  horizonViews:[], strategyViews:[], schoolViews:[], synthesis:synthesis(),
  ...overrides
});

test('detailed analysis rejects CURRENT when inputs are stale', () => {
  assert.throws(() => bindDetailedAssetAnalysisV1(detailed({ inputTruthFreshnessSummary:truthSummary({ freshnessState:'STALE' }) })), /CURRENT_SYNTHESIS_REQUIRES_CURRENT_INPUT_TRUTH_AND_FRESHNESS/);
});

test('detailed analysis rejects COMPLETE synthesis from partial inputs', () => {
  assert.throws(() => bindDetailedAssetAnalysisV1(detailed({
    overallTruthState:'STALE',
    inputTruthFreshnessSummary:truthSummary({ truthState:'STALE', freshnessState:'STALE', completeness:'PARTIAL' }),
    synthesis:synthesis({ synthesisState:'COMPLETE' })
  })), /PARTIAL_INPUTS_CANNOT_PRODUCE_COMPLETE_SYNTHESIS/);
});

test('detailed analysis preserves material disagreement instead of COMPLETE', () => {
  assert.throws(() => bindDetailedAssetAnalysisV1(detailed({ synthesis:synthesis({ synthesisState:'COMPLETE', disagreements:['School A vs School B'] }) })), /MATERIAL_DISAGREEMENT/);
});

test('NEEDS_CLARIFICATION requires candidates and cannot claim resolved instrument', () => {
  const base = {
    requestId:'REQ-1', analysisResultId:'AR-2', analysisIntent:'GENERAL_ANALYSIS', resultState:'NEEDS_CLARIFICATION',
    asOfTime:'2026-08-16T00:00:00Z', inputTruthFreshnessSummary:truthSummary({ truthState:'UNKNOWN', freshnessState:'UNKNOWN', completeness:'UNKNOWN' }),
    confidenceOrStrength:null, limitations:[], analysisProjection:null, clarificationCandidates:[{value:'BRK.B'}], resolvedInstrumentIdentity:null
  };
  assert.doesNotThrow(() => bindOnDemandAnalysisResultV1(base));
  assert.throws(() => bindOnDemandAnalysisResultV1({ ...base, resolvedInstrumentIdentity:{value:'BRK.B'} }), /CANNOT_CLAIM_RESOLVED_INSTRUMENT/);
  assert.throws(() => bindOnDemandAnalysisResultV1({ ...base, clarificationCandidates:[] }), /REQUIRES_CANDIDATES/);
});

test('COMPLETED analysis requires complete inputs and a projection', () => {
  const base = {
    requestId:'REQ-2', analysisResultId:'AR-3', analysisIntent:'GENERAL_ANALYSIS', resultState:'COMPLETED',
    resolvedInstrumentIdentity:{value:'AAPL'}, asOfTime:'2026-08-16T00:00:00Z', inputTruthFreshnessSummary:truthSummary(),
    confidenceOrStrength:null, limitations:[], clarificationCandidates:[], analysisProjection:{ detailedAssetAnalysis:detailed() }
  };
  assert.doesNotThrow(() => bindOnDemandAnalysisResultV1(base));
  assert.throws(() => bindOnDemandAnalysisResultV1({ ...base, analysisProjection:null }), /REQUIRES_PROJECTION/);
  assert.throws(() => bindOnDemandAnalysisResultV1({ ...base, inputTruthFreshnessSummary:truthSummary({completeness:'PARTIAL'}) }), /CANNOT_HAVE_PARTIAL_INPUTS/);
});

test('NOT_APPLICABLE catalog strategy must remain visible and disabled', () => {
  const base = {
    projectionId:'CAT-1', requestId:'REQ-CAT', truthState:'CURRENT', freshnessState:'CURRENT', completeness:'COMPLETE', availabilityState:'AVAILABLE', evidenceReference:'evidence/catalog', reasonCode:'OK',
    strategies:[{ strategyId:'S1', strategyName:'Breakout', schoolId:'SC1', schoolName:'Momentum', applicability:'NOT_APPLICABLE', visible:true, enabled:false, reasonCode:'NOT_FOR_CONTEXT', explanation:'Not applicable now' }]
  };
  assert.doesNotThrow(() => bindStrategyCatalogProjectionV1(base));
  assert.throws(() => bindStrategyCatalogProjectionV1({ ...base, strategies:[{ ...base.strategies[0], visible:false }] }), /VISIBLE_DISABLED/);
  assert.throws(() => bindStrategyCatalogProjectionV1({ ...base, strategies:[{ ...base.strategies[0], enabled:true }] }), /VISIBLE_DISABLED/);
});
