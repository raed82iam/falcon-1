import test from 'node:test';
import assert from 'node:assert/strict';
import { presentPortfolio, presentCatalog, presentDetailedAnalysis, presentOrderActivity } from '../src/presenters.js';
import { ApplicabilityState, AnalysisResultState } from '../src/contracts.js';

const envelope={projectionId:'p1',contractId:'FSATS.WebPortfolioSummaryProjection.v1',version:'1',account:{brokerId:'ALPACA',brokerAccountId:'PA-1',environment:'PAPER'},asOfTime:'2026-08-15T17:00:00Z',truthState:'LAST_KNOWN',freshnessState:'UNAVAILABLE',completeness:'PARTIAL',availabilityState:'DEGRADED',evidenceReference:'ev1',reasonCode:'PARTIAL'};

test('portfolio presenter keeps missing values explicit and preserves envelope truth', () => {
  const view=presentPortfolio({envelope,currency:'USD',totalEquity:100,cash:20,marketValue:null,reservedCapital:5,realizedPnl:2,unrealizedPnl:null});
  assert.equal(view.values.equity,100);
  assert.equal(view.values.marketValue,'—');
  assert.equal(view.values.unrealizedPnl,'—');
  assert.equal(view.state,'LAST_KNOWN');
  assert.equal(view.freshness,'UNAVAILABLE');
  assert.equal(view.completeness,'PARTIAL');
});

test('catalog keeps non-applicable current item visible but disabled', () => {
  const [item]=presentCatalog([{id:'s1',availability:'AVAILABLE',applicability:ApplicabilityState.NOT_APPLICABLE,reason:'Asset mismatch'}]);
  assert.equal(item.presentation.visible,true);
  assert.equal(item.presentation.enabled,false);
  assert.equal(item.presentation.reason,'Asset mismatch');
});

test('detailed analysis preserves disagreements from Application synthesis', () => {
  const view=presentDetailedAnalysis({resultState:AnalysisResultState.COMPLETED,detailedProjection:{synthesis:{agreements:['A'],disagreements:['B'],unresolvedConflicts:['C']}}});
  assert.deepEqual(view.synthesis.disagreements,['B']);
  assert.deepEqual(view.synthesis.unresolvedConflicts,['C']);
});

test('order activity presenter preserves unknown broker outcome distinctly',()=>{
  const [view]=presentOrderActivity([{order:{value:'ord-1'},instrument:{value:'MSFT'},state:'UNKNOWN_BROKER_OUTCOME',requestedQuantity:5,filledQuantity:null,averageFillPrice:null,effectiveAt:'2026-08-15T17:00:00Z',truthState:'UNKNOWN',freshnessState:'UNKNOWN',reasonCode:'OUTCOME_UNKNOWN'}]);
  assert.equal(view.state,'UNKNOWN_BROKER_OUTCOME');
  assert.equal(view.filledQuantity,'—');
  assert.equal(view.reasonCode,'OUTCOME_UNKNOWN');
});
