import test from 'node:test';
import assert from 'node:assert/strict';
import { bindPortfolioViewRequestV1, bindPortfolioSummaryV1, bindPositionsV1, bindOrderActivityV1, bindPerformanceV1, bindPortfolioUpdateV1 } from '../src/adapters/fsats-portfolio-v1.js';

const envelope=(contractId,extra={})=>({
  projectionId:'p-1',contractId,version:'1',
  account:{brokerId:'ALPACA',brokerAccountId:'PA-001',environment:'PAPER'},
  asOfTime:'2026-08-15T17:00:00Z',truthState:'CURRENT',freshnessState:'CURRENT',
  completeness:'COMPLETE',availabilityState:'AVAILABLE',evidenceReference:'ev-1',reasonCode:'CURRENT',...extra
});

const portfolioRequest=extra=>({
  contractId:'FSATS.WebPortfolioViewRequest.v1',version:'1',requestId:'req-1',correlation:{value:'corr-1'},
  brokerAccounts:[{brokerId:'ALPACA',brokerAccountId:'PA-001',environment:'PAPER'}],
  requestedAt:'2026-08-15T17:00:00Z',pageSize:100,
  positionContinuationToken:null,activityContinuationToken:null,performanceContinuationToken:null,
  ...extra
});

test('portfolio request binds exact broker-account scopes without customer/user identity',()=>{
  const bound=bindPortfolioViewRequestV1(portfolioRequest());
  assert.equal(bound.brokerAccounts[0].brokerAccountId,'PA-001');
  assert.equal(Object.isFrozen(bound),true);
  assert.equal(Object.isFrozen(bound.brokerAccounts),true);
  assert.equal(Object.isFrozen(bound.brokerAccounts[0]),true);
});

test('portfolio request rejects Web customer/user identity leakage into FSATS payload',()=>{
  assert.throws(()=>bindPortfolioViewRequestV1(portfolioRequest({customerId:'customer-1'})),/customerId is not legal/);
  assert.throws(()=>bindPortfolioViewRequestV1(portfolioRequest({principalId:'web-principal-1'})),/principalId is not legal/);
});

test('portfolio request requires distinct exact uppercase broker-account scopes',()=>{
  const duplicate={brokerId:'ALPACA',brokerAccountId:'PA-001',environment:'PAPER'};
  assert.throws(()=>bindPortfolioViewRequestV1(portfolioRequest({brokerAccounts:[duplicate,{...duplicate}]})),/duplicates an exact broker-account scope/);
  assert.throws(()=>bindPortfolioViewRequestV1(portfolioRequest({brokerAccounts:[{brokerId:'alpaca',brokerAccountId:'PA-001',environment:'PAPER'}]})),/canonical uppercase identity/);
  assert.throws(()=>bindPortfolioViewRequestV1(portfolioRequest({brokerAccounts:[{brokerId:'ALPACA',brokerAccountId:'PA-001',environment:'paper'}]})),/canonical uppercase identity/);
});

test('portfolio request continuation tokens stay opaque and page size fails closed',()=>{
  const bound=bindPortfolioViewRequestV1(portfolioRequest({positionContinuationToken:'opaque-token'}));
  assert.equal(bound.positionContinuationToken,'opaque-token');
  assert.throws(()=>bindPortfolioViewRequestV1(portfolioRequest({pageSize:0})),/pageSize/);
  assert.throws(()=>bindPortfolioViewRequestV1(portfolioRequest({activityContinuationToken:''})),/opaque string/);
});

test('portfolio v1 preserves null instead of zero-filling unavailable values',()=>{
  const bound=bindPortfolioSummaryV1({
    envelope:envelope('FSATS.WebPortfolioSummaryProjection.v1',{truthState:'LAST_KNOWN',freshnessState:'UNAVAILABLE',completeness:'PARTIAL',availabilityState:'DEGRADED'}),
    currency:'USD',totalEquity:null,cash:100,marketValue:null,reservedCapital:10,realizedPnl:5,unrealizedPnl:null
  });
  assert.equal(bound.totalEquity,null);
  assert.equal(bound.unrealizedPnl,null);
  assert.equal(bound.envelope.truthState,'LAST_KNOWN');
  assert.equal(Object.isFrozen(bound),true);
  assert.equal(Object.isFrozen(bound.envelope),true);
  assert.equal(Object.isFrozen(bound.envelope.account),true);
});

test('UNSUPPORTED portfolio summary requires all business numbers to be null',()=>{
  const unavailableEnvelope=envelope('FSATS.WebPortfolioSummaryProjection.v1',{truthState:'UNKNOWN',freshnessState:'UNAVAILABLE',completeness:'UNKNOWN',availabilityState:'UNSUPPORTED',reasonCode:'UNSUPPORTED'});
  assert.doesNotThrow(()=>bindPortfolioSummaryV1({envelope:unavailableEnvelope,currency:'USD',totalEquity:null,cash:null,marketValue:null,reservedCapital:null,realizedPnl:null,unrealizedPnl:null}));
  assert.throws(()=>bindPortfolioSummaryV1({envelope:unavailableEnvelope,currency:'USD',totalEquity:0,cash:null,marketValue:null,reservedCapital:null,realizedPnl:null,unrealizedPnl:null}),/must be null/);
});

test('NOT_APPLICABLE positions and activity must be empty rather than stale rows',()=>{
  const positionsEnvelope=envelope('FSATS.WebPositionCollectionProjection.v1',{truthState:'UNKNOWN',freshnessState:'UNAVAILABLE',completeness:'UNKNOWN',availabilityState:'NOT_APPLICABLE',reasonCode:'NOT_APPLICABLE'});
  const activityEnvelope=envelope('FSATS.WebOrderTradeActivityProjection.v1',{truthState:'UNKNOWN',freshnessState:'UNAVAILABLE',completeness:'UNKNOWN',availabilityState:'NOT_APPLICABLE',reasonCode:'NOT_APPLICABLE'});
  const page={continuationToken:null,hasMore:false,pageSize:100};
  assert.doesNotThrow(()=>bindPositionsV1({envelope:positionsEnvelope,positions:[],page}));
  assert.doesNotThrow(()=>bindOrderActivityV1({envelope:activityEnvelope,activity:[],page}));
  assert.throws(()=>bindPositionsV1({envelope:positionsEnvelope,positions:[{}],page}),/MUST_BE_EMPTY/);
  assert.throws(()=>bindOrderActivityV1({envelope:activityEnvelope,activity:[{}],page}),/MUST_BE_EMPTY/);
});

test('UNSUPPORTED performance requires null numbers and empty history',()=>{
  const performanceEnvelope=envelope('FSATS.WebPortfolioPerformanceProjection.v1',{truthState:'UNKNOWN',freshnessState:'UNAVAILABLE',completeness:'UNKNOWN',availabilityState:'UNSUPPORTED',reasonCode:'UNSUPPORTED'});
  const base={envelope:performanceEnvelope,periodStart:'2026-08-01T00:00:00Z',periodEnd:'2026-08-15T00:00:00Z',currency:'USD',openingEquity:null,closingEquity:null,realizedPnl:null,unrealizedPnl:null,netPnl:null,returnPercent:null,history:[],page:{continuationToken:null,hasMore:false,pageSize:100}};
  assert.doesNotThrow(()=>bindPerformanceV1(base));
  assert.throws(()=>bindPerformanceV1({...base,netPnl:0}),/must be null/);
  assert.throws(()=>bindPerformanceV1({...base,history:[{}]}),/HISTORY_MUST_BE_EMPTY/);
});

test('AVAILABLE authoritative zero remains legal zero',()=>{
  const bound=bindPortfolioSummaryV1({envelope:envelope('FSATS.WebPortfolioSummaryProjection.v1'),currency:'USD',totalEquity:0,cash:0,marketValue:0,reservedCapital:0,realizedPnl:0,unrealizedPnl:0});
  assert.equal(bound.totalEquity,0);
  assert.equal(bound.cash,0);
});

test('required-nullable fields must be present and are not silently manufactured as null',()=>{
  assert.throws(()=>bindPortfolioSummaryV1({
    envelope:envelope('FSATS.WebPortfolioSummaryProjection.v1'),currency:'USD',
    totalEquity:null,cash:null,marketValue:null,reservedCapital:null,realizedPnl:null
  }),/unrealizedPnl is required/);
});

test('portfolio v1 rejects a future/unknown major contract identity',()=>{
  assert.throws(()=>bindPortfolioSummaryV1({
    envelope:envelope('FSATS.WebPortfolioSummaryProjection.v2'),currency:'USD',
    totalEquity:null,cash:null,marketValue:null,reservedCapital:null,realizedPnl:null,unrealizedPnl:null
  }),/unexpected contractId/);
  assert.throws(()=>bindPortfolioViewRequestV1(portfolioRequest({contractId:'FSATS.WebPortfolioViewRequest.v2'})),/unexpected contractId/);
});

test('activity binding preserves PARTIALLY_FILLED distinctly from FILLED and freezes nested evidence',()=>{
  const bound=bindOrderActivityV1({
    envelope:envelope('FSATS.WebOrderTradeActivityProjection.v1'),
    activity:[{order:{value:'ord-1'},instrument:{value:'AAPL'},state:'PARTIALLY_FILLED',requestedQuantity:10,filledQuantity:4,averageFillPrice:220.1,currency:'USD',effectiveAt:'2026-08-15T17:01:50Z',truthState:'CURRENT',freshnessState:'CURRENT',reasonCode:'BROKER_PARTIAL_FILL_CONFIRMED'}],
    page:{continuationToken:null,hasMore:false,pageSize:100}
  });
  assert.equal(bound.activity[0].state,'PARTIALLY_FILLED');
  assert.equal(Object.isFrozen(bound.activity),true);
  assert.equal(Object.isFrozen(bound.activity[0]),true);
  assert.equal(Object.isFrozen(bound.activity[0].order),true);
});

test('activity binding accepts explicit unknown broker outcome without converting it to rejected',()=>{
  const bound=bindOrderActivityV1({
    envelope:envelope('FSATS.WebOrderTradeActivityProjection.v1',{truthState:'UNKNOWN',freshnessState:'UNKNOWN',completeness:'PARTIAL',availabilityState:'DEGRADED'}),
    activity:[{order:{value:'ord-2'},instrument:{value:'MSFT'},state:'UNKNOWN_BROKER_OUTCOME',requestedQuantity:5,filledQuantity:null,averageFillPrice:null,currency:'USD',effectiveAt:'2026-08-15T17:02:55Z',truthState:'UNKNOWN',freshnessState:'UNKNOWN',reasonCode:'SUBMISSION_OUTCOME_UNKNOWN'}],
    page:{continuationToken:null,hasMore:false,pageSize:100}
  });
  assert.equal(bound.activity[0].state,'UNKNOWN_BROKER_OUTCOME');
});

test('pagination fails closed when hasMore is true without continuation token',()=>{
  assert.throws(()=>bindOrderActivityV1({
    envelope:envelope('FSATS.WebOrderTradeActivityProjection.v1'),activity:[],
    page:{continuationToken:null,hasMore:true,pageSize:100}
  }),/continuationToken is required when hasMore=true/);
});

test('performance history requires exact truth and nullable numeric fields',()=>{
  const bound=bindPerformanceV1({
    envelope:envelope('FSATS.WebPortfolioPerformanceProjection.v1'),
    periodStart:'2026-08-01T00:00:00Z',periodEnd:'2026-08-15T00:00:00Z',currency:'USD',
    openingEquity:10000,closingEquity:10100,realizedPnl:50,unrealizedPnl:50,netPnl:100,returnPercent:1,
    history:[{effectiveAt:'2026-08-15T00:00:00Z',truthState:'CURRENT',freshnessState:'CURRENT',reasonCode:'CURRENT',equity:10100,netPnl:100,returnPercent:1}],
    page:{continuationToken:null,hasMore:false,pageSize:100}
  });
  assert.equal(bound.history[0].returnPercent,1);
  assert.equal(Object.isFrozen(bound.history[0]),true);
});

test('portfolio update lineage semantics fail closed',()=>{
  const base={
    updateId:'u-1',updateSequence:1,updateKind:'ORDINARY',correlation:{value:'c-1'},
    account:{brokerId:'ALPACA',brokerAccountId:'PA-001',environment:'PAPER'},
    changedProjectionContractIds:['FSATS.WebPortfolioSummaryProjection.v1'],projectionVersion:'1',effectiveAt:'2026-08-15T17:03:00Z',
    truthState:'CURRENT',freshnessState:'CURRENT',evidenceReference:'ev-u1',reasonCode:'UPDATED',correctsUpdateId:null,supersedesUpdateId:null
  };
  const bound=bindPortfolioUpdateV1(base);
  assert.equal(bound.updateKind,'ORDINARY');
  assert.equal(Object.isFrozen(bound.account),true);
  assert.throws(()=>bindPortfolioUpdateV1({...base,updateKind:'CORRECTION',correctsUpdateId:null}),/requires correctsUpdateId/);
  assert.throws(()=>bindPortfolioUpdateV1({...base,updateKind:'SUPERSESSION',supersedesUpdateId:null}),/requires supersedesUpdateId/);
});
