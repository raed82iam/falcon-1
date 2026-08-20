const FULL_DETAIL_STATES = new Set(['COMPLETED']);
const LIMITED_DETAIL_STATES = new Set(['PARTIAL']);
const CURRENT_TRUTH = 'CURRENT';
const CURRENT_FRESHNESS = 'CURRENT';
const COMPLETE = 'COMPLETE';

function value(value) {
  return typeof value === 'string' ? value.trim() : '';
}

/**
 * Web-only presentation policy for Application-owned FSATS analysis.
 *
 * This policy never creates analysis truth, confidence, targets, applicability,
 * or Trading authority. It only decides how much already-supplied analysis may
 * be shown to a customer.
 */
export function decideAnalysisPresentation({ onDemand = null, detailed = null } = {}) {
  const requestState = value(onDemand?.resultState ?? onDemand?.state);
  const projection = detailed?.detailedProjection ?? detailed ?? null;
  const truthState = value(projection?.overallTruthState ?? onDemand?.inputTruth?.truthState);
  const freshnessState = value(projection?.inputTruthFreshnessSummary?.freshnessState ?? onDemand?.inputTruth?.freshnessState);
  const completeness = value(projection?.inputTruthFreshnessSummary?.completeness ?? onDemand?.inputTruth?.completeness);
  const synthesisState = value(projection?.synthesis?.synthesisState);

  if (!requestState) {
    return Object.freeze({ mode:'UNAVAILABLE', showDetails:false, showSummary:false, reason:'ANALYSIS_RESULT_STATE_UNAVAILABLE' });
  }

  if (FULL_DETAIL_STATES.has(requestState)) {
    const current = truthState === CURRENT_TRUTH && freshnessState === CURRENT_FRESHNESS;
    const complete = completeness === COMPLETE;
    const synthesisComplete = synthesisState === COMPLETE;
    if (current && complete && synthesisComplete) {
      return Object.freeze({ mode:'CURRENT_COMPLETE', showDetails:true, showSummary:true, reason:null });
    }
    return Object.freeze({
      mode:'LIMITED',
      showDetails:false,
      showSummary:true,
      reason:!current ? 'ANALYSIS_NOT_CURRENT' : !complete ? 'ANALYSIS_INPUTS_INCOMPLETE' : 'ANALYSIS_SYNTHESIS_NOT_COMPLETE'
    });
  }

  if (LIMITED_DETAIL_STATES.has(requestState)) {
    return Object.freeze({ mode:'LIMITED', showDetails:false, showSummary:true, reason:'PARTIAL_ANALYSIS_RESULT' });
  }

  if (requestState === 'NEEDS_CLARIFICATION') {
    return Object.freeze({ mode:'CLARIFICATION_REQUIRED', showDetails:false, showSummary:false, reason:'ANALYSIS_NEEDS_CLARIFICATION' });
  }

  return Object.freeze({ mode:'UNAVAILABLE', showDetails:false, showSummary:false, reason:`ANALYSIS_${requestState}` });
}
