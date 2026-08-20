import test from 'node:test';
import assert from 'node:assert/strict';
import { createAiFeature } from '../src/features/ai/ai.js';

const labels={falconAI:'Falcon AI',askFalcon:'Ask Falcon',analysisShort:'Short term',analysisMedium:'Medium term',analysisLong:'Long term',strategies:'Strategies',schools:'Schools',synthesis:'Synthesis',showFullAnalysis:'Show full analysis',analysisTruthNotice:'Falcon presents FSATS analysis only.',noAnalysisValue:'Unavailable from source'};
const t = key => labels[key] ?? key;
const workspace = (body, active) => `<main data-active="${active}">${body}</main>`;
const data={
  onDemandAnalysis:{resultState:'PARTIAL',asOfTime:'2026-08-16T12:00:00Z',limitations:['One source is stale']},
  detailedAnalysis:{
    analysisResultId:'A1',asOfTime:'2026-08-16T12:00:00Z',overallTruthState:'STALE',
    horizonViews:[{horizonId:'DAILY',resultState:'PARTIAL',conclusion:'Short view',confidenceOrStrength:0.61,limitations:['Delayed source']}],
    strategyViews:[{strategyId:'TREND',applicabilityState:'APPLICABLE',resultState:'PARTIAL',conclusion:'Bullish with limits',truthState:'STALE',freshnessState:'STALE'}],
    schoolViews:[{schoolId:'WHALE',applicabilityState:'APPLICABLE',resultState:'PARTIAL',perspectiveOrConclusion:'Accumulation signal',truthState:'STALE',freshnessState:'STALE'}],
    synthesis:{synthesisState:'CONFLICTED',boundedCombinedExplanation:'Material disagreement remains.',disagreements:['Trend and momentum disagree'],unresolvedConflicts:['Timing conflict']}
  }
};

function currentCompleteData() {
  const current=structuredClone(data);
  current.onDemandAnalysis.resultState='COMPLETED';
  current.onDemandAnalysis.limitations=[];
  current.detailedAnalysis.overallTruthState='CURRENT';
  current.detailedAnalysis.inputTruthFreshnessSummary={freshnessState:'CURRENT',completeness:'COMPLETE'};
  current.detailedAnalysis.synthesis.synthesisState='COMPLETE';
  return current;
}

test('AI feature renders canonical Application-owned horizons and disagreement only for current complete analysis', () => {
  const { aiPage } = createAiFeature({ t, language: () => 'en', workspace, data:currentCompleteData() });
  const html = aiPage();
  assert.match(html, /data-active="ai"/);
  assert.match(html, /COMPLETED/);
  assert.match(html, /Short view/);
  assert.match(html, /Bullish with limits/);
  assert.match(html, /Accumulation signal/);
  assert.match(html, /Material disagreement remains/);
  assert.match(html, /Trend and momentum disagree/);
  assert.match(html, /Timing conflict/);
  assert.match(html, /Operational request transport remains fail-closed/);
  assert.match(html, /data-analysis-request disabled aria-disabled="true"/);
});

test('AI feature does not dump arbitrary structured disagreement metadata', () => {
  const structured=currentCompleteData();
  structured.detailedAnalysis.synthesis.disagreements=[{explanation:'Bounded explanation',internalReference:'DO-NOT-DISPLAY'}];
  structured.detailedAnalysis.synthesis.unresolvedConflicts=[{internalReference:'ALSO-HIDDEN'}];
  const html=createAiFeature({t,language:()=> 'en',workspace,data:structured}).aiPage();
  assert.match(html,/Bounded explanation/);
  assert.match(html,/structured disagreement was supplied/i);
  assert.doesNotMatch(html,/DO-NOT-DISPLAY/);
  assert.doesNotMatch(html,/ALSO-HIDDEN/);
});

test('AI feature fails closed when analysis result state is unavailable', () => {
  const html=createAiFeature({t,language:()=> 'en',workspace,data:{detailedAnalysis:{}}}).aiPage();
  assert.match(html,/ANALYSIS_RESULT_STATE_UNAVAILABLE/);
  assert.match(html,/Current analysis details cannot be shown in this state/);
  assert.doesNotMatch(html,/analysis-placeholder/);
});

test('AI feature preserves Arabic fail-closed explanation when analysis result state is unavailable', () => {
  const { aiPage } = createAiFeature({ t, language: () => 'ar', workspace, data:{detailedAnalysis:{}} });
  assert.match(aiPage(), /لا يمكن عرض تفاصيل التحليل الحالية بهذه الحالة/);
  assert.match(aiPage(), /البيانات الناقصة أو القديمة أو غير المكتملة واضحة/);
});

test('AI feature validates presentation dependencies', () => {
  assert.throws(() => createAiFeature({ language: () => 'en', workspace }), /t must be a function/);
  assert.throws(() => createAiFeature({ t, workspace }), /language must be a function/);
  assert.throws(() => createAiFeature({ t, language: () => 'en' }), /workspace must be a function/);
});
