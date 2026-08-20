import test from 'node:test';
import assert from 'node:assert/strict';
import { createAiFeature } from '../src/features/ai/ai.js';

const t=key=>key;
const workspace=content=>content;
const baseProjection={
  overallTruthState:'CURRENT',
  inputTruthFreshnessSummary:{freshnessState:'CURRENT',completeness:'COMPLETE'},
  synthesis:{synthesisState:'COMPLETE',summary:'Combined view'},
  summary:'Current summary',
  horizonViews:[{horizonId:'SHORT',conclusion:'up'}],
  strategyViews:[{strategyId:'S1',conclusion:'ok'}],
  schoolViews:[{schoolId:'C1',summary:'view'}]
};

function render(onDemand,detailed){
  return createAiFeature({t,language:()=> 'en',workspace,data:{onDemandAnalysis:onDemand,detailedAnalysis:detailed}}).aiPage();
}

test('current complete analysis exposes detailed presentation',()=>{
  const html=render({resultState:'COMPLETED'},baseProjection);
  assert.match(html,/analysis-placeholder/);
  assert.match(html,/Current summary/);
  assert.match(html,/showFullAnalysis/);
});

test('stale completed analysis hides detailed projections and keeps limitation explicit',()=>{
  const stale={...baseProjection,overallTruthState:'STALE'};
  const html=render({resultState:'COMPLETED',summary:'must not appear'},stale);
  assert.doesNotMatch(html,/analysis-placeholder/);
  assert.doesNotMatch(html,/must not appear/);
  assert.match(html,/ANALYSIS_NOT_CURRENT/);
  assert.doesNotMatch(html,/showFullAnalysis/);
});

test('partial analysis does not expose strategy school horizon details',()=>{
  const html=render({resultState:'PARTIAL',summary:'limited summary'},baseProjection);
  assert.match(html,/limited summary/);
  assert.match(html,/PARTIAL_ANALYSIS_RESULT/);
  assert.doesNotMatch(html,/analysis-placeholder/);
});

test('clarification-required result hides supplied analysis details',()=>{
  const html=render({resultState:'NEEDS_CLARIFICATION',summary:'unsafe summary'},baseProjection);
  assert.match(html,/ANALYSIS_NEEDS_CLARIFICATION/);
  assert.doesNotMatch(html,/unsafe summary/);
  assert.doesNotMatch(html,/analysis-placeholder/);
});
