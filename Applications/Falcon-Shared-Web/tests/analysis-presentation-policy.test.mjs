import test from 'node:test';
import assert from 'node:assert/strict';
import { decideAnalysisPresentation } from '../src/features/ai/analysis-presentation-policy.js';

const currentComplete = {
  overallTruthState:'CURRENT',
  inputTruthFreshnessSummary:{ truthState:'CURRENT', freshnessState:'CURRENT', completeness:'COMPLETE' },
  synthesis:{ synthesisState:'COMPLETE' }
};

test('current complete analysis permits full detail presentation only',()=>{
  assert.deepEqual(decideAnalysisPresentation({ onDemand:{ resultState:'COMPLETED' }, detailed:currentComplete }), {
    mode:'CURRENT_COMPLETE', showDetails:true, showSummary:true, reason:null
  });
});

test('stale completed analysis is limited and cannot look current',()=>{
  const detailed={...currentComplete,overallTruthState:'STALE'};
  const decision=decideAnalysisPresentation({onDemand:{resultState:'COMPLETED'},detailed});
  assert.equal(decision.mode,'LIMITED');
  assert.equal(decision.showDetails,false);
  assert.equal(decision.showSummary,true);
  assert.equal(decision.reason,'ANALYSIS_NOT_CURRENT');
});

test('partial result never permits full detail',()=>{
  const decision=decideAnalysisPresentation({onDemand:{resultState:'PARTIAL'},detailed:currentComplete});
  assert.equal(decision.mode,'LIMITED');
  assert.equal(decision.showDetails,false);
  assert.equal(decision.reason,'PARTIAL_ANALYSIS_RESULT');
});

test('needs clarification hides analysis projection presentation',()=>{
  const decision=decideAnalysisPresentation({onDemand:{resultState:'NEEDS_CLARIFICATION'}});
  assert.equal(decision.mode,'CLARIFICATION_REQUIRED');
  assert.equal(decision.showDetails,false);
  assert.equal(decision.showSummary,false);
});

test('missing result state fails closed',()=>{
  assert.deepEqual(decideAnalysisPresentation({}), {
    mode:'UNAVAILABLE', showDetails:false, showSummary:false, reason:'ANALYSIS_RESULT_STATE_UNAVAILABLE'
  });
});
