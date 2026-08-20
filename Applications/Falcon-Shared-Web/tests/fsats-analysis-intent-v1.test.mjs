import test from 'node:test';
import assert from 'node:assert/strict';
import { createOnDemandAnalysisRequestV1, bindOnDemandAnalysisResultV1, presentOnDemandAnalysisResultV1 } from '../src/adapters/fsats-analysis-intent-v1.js';

const inputTruth = { truthState:'CURRENT', freshnessState:'CURRENT', completeness:'COMPLETE', limitations:[] };

test('on-demand analysis request carries intent without Trading side effects',()=>{
  const request=createOnDemandAnalysisRequestV1({requestId:'r1',correlationId:'c1',requestedInstrumentReference:'AAPL',analysisIntent:'DETAILED_ASSET_STUDY',requestedAt:'2026-08-15T19:00:00Z'});
  assert.equal(request.contractId,'FSATS.WebOnDemandAnalysisRequest.v1');
  assert.equal(request.requestingApplicationId,'SHARED_WEB');
  assert.equal(request.universeMutationRequested,false);
  assert.equal(request.strategyActivationRequested,false);
  assert.equal(request.capitalReservationRequested,false);
  assert.equal(request.orderIntentCreated,false);
  assert.equal(request.executionAuthorityRequested,false);
});

test('on-demand result preserves clarification instead of guessing identity',()=>{
  const view=presentOnDemandAnalysisResultV1({
    requestId:'r1',analysisResultId:'a1',resolvedInstrumentIdentity:null,analysisIntent:'DETAILED_ASSET_STUDY',
    resultState:'NEEDS_CLARIFICATION',analysisProjection:null,asOfTime:'2026-08-15T19:00:01Z',
    inputTruthFreshnessSummary:inputTruth,confidenceOrStrength:null,limitations:['Ambiguous instrument'],
    clarificationCandidates:['AAPL:NASDAQ','AAPL:OTHER'],reasonCode:'AMBIGUOUS_INSTRUMENT'
  });
  assert.equal(view.state,'NEEDS_CLARIFICATION');
  assert.equal(view.resolvedInstrumentIdentity,null);
  assert.equal(view.projection,null);
  assert.deepEqual(view.limitations,['Ambiguous instrument']);
  assert.deepEqual(view.clarificationCandidates,['AAPL:NASDAQ','AAPL:OTHER']);
});

test('clarification result cannot claim resolved identity',()=>{
  assert.throws(()=>bindOnDemandAnalysisResultV1({
    requestId:'r1',analysisResultId:'a1',resolvedInstrumentIdentity:'AAPL',analysisIntent:'DETAILED_ASSET_STUDY',
    resultState:'NEEDS_CLARIFICATION',analysisProjection:null,asOfTime:'2026-08-15T19:00:01Z',
    inputTruthFreshnessSummary:inputTruth,confidenceOrStrength:null,limitations:[],clarificationCandidates:['AAPL:NASDAQ']
  }),/NEEDS_CLARIFICATION_CANNOT_CLAIM_RESOLVED_INSTRUMENT/);
});

test('completed result requires a projection',()=>{
  assert.throws(()=>bindOnDemandAnalysisResultV1({
    requestId:'r1',analysisResultId:'a1',resolvedInstrumentIdentity:'AAPL',analysisIntent:'DETAILED_ASSET_STUDY',
    resultState:'COMPLETED',analysisProjection:null,asOfTime:'2026-08-15T19:00:01Z',
    inputTruthFreshnessSummary:inputTruth,confidenceOrStrength:null,limitations:[],clarificationCandidates:[]
  }),/COMPLETED_ANALYSIS_REQUIRES_PROJECTION/);
});

test('unknown or malformed result fails closed',()=>{
  assert.equal(presentOnDemandAnalysisResultV1({resultState:'MAGIC_SUCCESS'}).state,'UNAVAILABLE');
  assert.equal(presentOnDemandAnalysisResultV1({resultState:'PARTIAL'}).reasonCode,'MALFORMED_APPLICATION_RESULT');
});
