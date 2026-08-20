import { AnalysisResultState, ApplicabilityState, AvailabilityState, CompletenessState, FreshnessState, TruthState } from '../contracts.js';
import { deepFreeze } from '../core/immutable.js';

const resultStates = new Set(Object.values(AnalysisResultState));
const applicabilityStates = new Set(['APPLICABLE','NOT_APPLICABLE','UNKNOWN','UNAVAILABLE']);
const truthStates = new Set(['CURRENT','LAST_KNOWN','STALE','UNKNOWN','SIMULATION','REPLAY']);
const freshnessStates = new Set(Object.values(FreshnessState));
const completenessStates = new Set(Object.values(CompletenessState));
const availabilityStates = new Set(Object.values(AvailabilityState));
const synthesisStates = new Set(['COMPLETE','PARTIAL','CONFLICTED','UNAVAILABLE','UNKNOWN']);

function requiredString(value, name) {
  if (typeof value !== 'string' || value.trim().length === 0) throw new TypeError(`${name} is required`);
  return value.trim();
}
function optionalNumber(value, name) {
  if (value === null || value === undefined) return null;
  if (typeof value !== 'number' || !Number.isFinite(value)) throw new TypeError(`${name} must be a finite number or null`);
  return value;
}
function array(value, name) { if (!Array.isArray(value)) throw new TypeError(`${name} must be an array`); return value; }
function refValue(value, name) {
  if (typeof value === 'string') return requiredString(value,name);
  if (value && typeof value.value === 'string') return requiredString(value.value,`${name}.value`);
  throw new TypeError(`${name} is required`);
}
function assertTruthSummary(summary) {
  if (!summary || typeof summary !== 'object') throw new TypeError('inputTruthFreshnessSummary is required');
  if (!truthStates.has(summary.truthState)) throw new TypeError('invalid input truthState');
  if (!freshnessStates.has(summary.freshnessState)) throw new TypeError('invalid input freshnessState');
  if (!completenessStates.has(summary.completeness)) throw new TypeError('invalid input completeness');
  array(summary.limitations ?? [],'input limitations');
  return summary;
}

function bindMaterialTargets(items, name) {
  return array(items ?? [],name).map((item,index) => {
    requiredString(item.kind,`${name}[${index}].kind`);
    if (typeof item.value !== 'number' || !Number.isFinite(item.value)) throw new TypeError(`${name}[${index}].value must be finite`);
    requiredString(item.evidenceReference,`${name}[${index}].evidenceReference`);
    return deepFreeze(structuredClone(item));
  });
}

export function bindDetailedAssetAnalysisV1(payload) {
  if (!payload || typeof payload !== 'object') throw new TypeError('detailed asset analysis is required');
  refValue(payload.resolvedInstrumentIdentity,'resolvedInstrumentIdentity');
  requiredString(payload.analysisResultId,'analysisResultId');
  requiredString(payload.asOfTime,'asOfTime');
  if (!truthStates.has(payload.overallTruthState)) throw new TypeError('invalid overallTruthState');
  const input = assertTruthSummary(payload.inputTruthFreshnessSummary);
  const horizons = array(payload.horizonViews,'horizonViews');
  const strategies = array(payload.strategyViews,'strategyViews');
  const schools = array(payload.schoolViews,'schoolViews');
  const synthesis = payload.synthesis;
  if (!synthesis || !synthesisStates.has(synthesis.synthesisState)) throw new TypeError('invalid synthesis');
  array(synthesis.agreements,'synthesis.agreements');
  array(synthesis.disagreements,'synthesis.disagreements');
  array(synthesis.unresolvedConflicts,'synthesis.unresolvedConflicts');
  requiredString(synthesis.boundedCombinedExplanation,'synthesis.boundedCombinedExplanation');
  array(synthesis.contributingOutputReferences,'synthesis.contributingOutputReferences');
  array(synthesis.limitations,'synthesis.limitations');

  if (payload.overallTruthState === TruthState.CURRENT && (input.truthState !== TruthState.CURRENT || input.freshnessState !== FreshnessState.CURRENT)) {
    throw new TypeError('CURRENT_SYNTHESIS_REQUIRES_CURRENT_INPUT_TRUTH_AND_FRESHNESS');
  }
  if (input.completeness !== CompletenessState.COMPLETE && synthesis.synthesisState === 'COMPLETE') {
    throw new TypeError('PARTIAL_INPUTS_CANNOT_PRODUCE_COMPLETE_SYNTHESIS');
  }
  if ((synthesis.disagreements.length > 0 || synthesis.unresolvedConflicts.length > 0) && synthesis.synthesisState === 'COMPLETE') {
    throw new TypeError('MATERIAL_DISAGREEMENT_CANNOT_BE_COMPLETE_SYNTHESIS');
  }

  horizons.forEach((view,index) => {
    requiredString(view.horizonId,`horizonViews[${index}].horizonId`);
    if (!resultStates.has(view.resultState)) throw new TypeError(`invalid horizon resultState at ${index}`);
    requiredString(view.conclusion,`horizonViews[${index}].conclusion`);
    bindMaterialTargets(view.materialLevelsOrTargets,`horizonViews[${index}].materialLevelsOrTargets`);
    optionalNumber(view.confidenceOrStrength,`horizonViews[${index}].confidenceOrStrength`);
    array(view.limitations ?? [],`horizonViews[${index}].limitations`);
    array(view.evidenceOrSourceOutputReferences ?? [],`horizonViews[${index}].evidenceOrSourceOutputReferences`);
  });

  for (const [name,views] of [['strategyViews',strategies],['schoolViews',schools]]) {
    views.forEach((view,index) => {
      requiredString(view[name==='strategyViews'?'strategyId':'schoolId'],`${name}[${index}].id`);
      if (!applicabilityStates.has(view.applicabilityState)) throw new TypeError(`invalid ${name} applicabilityState at ${index}`);
      if (!resultStates.has(view.resultState)) throw new TypeError(`invalid ${name} resultState at ${index}`);
      requiredString(name==='strategyViews'?view.conclusion:view.perspectiveOrConclusion,`${name}[${index}].conclusion`);
      bindMaterialTargets(view.materialLevelsOrTargets,`${name}[${index}].materialLevelsOrTargets`);
      optionalNumber(view.confidenceOrStrength,`${name}[${index}].confidenceOrStrength`);
      requiredString(view.asOfTime,`${name}[${index}].asOfTime`);
      if (!truthStates.has(view.truthState)) throw new TypeError(`invalid ${name} truthState at ${index}`);
      if (!freshnessStates.has(view.freshnessState)) throw new TypeError(`invalid ${name} freshnessState at ${index}`);
      array(view.limitations ?? [],`${name}[${index}].limitations`);
      array(view.evidenceOrSourceOutputReferences ?? [],`${name}[${index}].evidenceOrSourceOutputReferences`);
    });
  }
  return deepFreeze(structuredClone(payload));
}

export function bindOnDemandAnalysisResultV1(payload) {
  if (!payload || typeof payload !== 'object') throw new TypeError('analysis result is required');
  requiredString(payload.requestId,'requestId');
  requiredString(payload.analysisResultId,'analysisResultId');
  requiredString(payload.analysisIntent,'analysisIntent');
  if (!resultStates.has(payload.resultState)) throw new TypeError('invalid resultState');
  requiredString(payload.asOfTime,'asOfTime');
  const input = assertTruthSummary(payload.inputTruthFreshnessSummary);
  optionalNumber(payload.confidenceOrStrength,'confidenceOrStrength');
  array(payload.limitations ?? [],'limitations');
  const candidates = array(payload.clarificationCandidates ?? [],'clarificationCandidates');

  if (payload.resultState === AnalysisResultState.NEEDS_CLARIFICATION) {
    if (payload.resolvedInstrumentIdentity != null) throw new TypeError('NEEDS_CLARIFICATION_CANNOT_CLAIM_RESOLVED_INSTRUMENT');
    if (payload.analysisProjection != null) throw new TypeError('NEEDS_CLARIFICATION_CANNOT_CLAIM_ANALYSIS_PROJECTION');
    if (candidates.length === 0) throw new TypeError('NEEDS_CLARIFICATION_REQUIRES_CANDIDATES');
    candidates.forEach((candidate,index)=>refValue(candidate,`clarificationCandidates[${index}]`));
  }
  if (payload.resultState === AnalysisResultState.COMPLETED) {
    refValue(payload.resolvedInstrumentIdentity,'resolvedInstrumentIdentity');
    if (!payload.analysisProjection) throw new TypeError('COMPLETED_ANALYSIS_REQUIRES_PROJECTION');
    if (input.completeness !== CompletenessState.COMPLETE) throw new TypeError('COMPLETED_ANALYSIS_CANNOT_HAVE_PARTIAL_INPUTS');
  }
  if (payload.analysisProjection?.detailedAssetAnalysis) bindDetailedAssetAnalysisV1(payload.analysisProjection.detailedAssetAnalysis);
  return deepFreeze(structuredClone(payload));
}

export function bindStrategyCatalogProjectionV1(payload) {
  if (!payload || typeof payload !== 'object') throw new TypeError('strategy catalog projection is required');
  requiredString(payload.projectionId,'projectionId');
  requiredString(payload.requestId,'requestId');
  if (!truthStates.has(payload.truthState)) throw new TypeError('invalid catalog truthState');
  if (!freshnessStates.has(payload.freshnessState)) throw new TypeError('invalid catalog freshnessState');
  if (!completenessStates.has(payload.completeness)) throw new TypeError('invalid catalog completeness');
  if (!availabilityStates.has(payload.availabilityState)) throw new TypeError('invalid catalog availabilityState');
  requiredString(payload.evidenceReference,'evidenceReference');
  requiredString(payload.reasonCode,'reasonCode');
  const strategies = array(payload.strategies,'strategies');
  strategies.forEach((item,index) => {
    requiredString(item.strategyId,`strategies[${index}].strategyId`);
    requiredString(item.strategyName,`strategies[${index}].strategyName`);
    requiredString(item.schoolId,`strategies[${index}].schoolId`);
    requiredString(item.schoolName,`strategies[${index}].schoolName`);
    if (!applicabilityStates.has(item.applicability)) throw new TypeError(`invalid strategies[${index}].applicability`);
    if (typeof item.visible !== 'boolean' || typeof item.enabled !== 'boolean') throw new TypeError(`strategies[${index}] visibility/enabled required`);
    requiredString(item.reasonCode,`strategies[${index}].reasonCode`);
    if (item.applicability === ApplicabilityState.NOT_APPLICABLE && (!item.visible || item.enabled)) throw new TypeError('NOT_APPLICABLE_MUST_BE_VISIBLE_DISABLED');
  });
  return deepFreeze(structuredClone(payload));
}

export function presentStrategyCatalogV1(payload) {
  const bound = bindStrategyCatalogProjectionV1(payload);
  return deepFreeze({
    truthState:bound.truthState,
    freshnessState:bound.freshnessState,
    completeness:bound.completeness,
    availabilityState:bound.availabilityState,
    items:bound.strategies.filter(item=>item.visible).map(item=>({
      id:item.strategyId,
      name:item.strategyName,
      schoolId:item.schoolId,
      schoolName:item.schoolName,
      applicability:item.applicability,
      enabled:item.enabled,
      reason:item.explanation ?? item.reasonCode
    }))
  });
}
