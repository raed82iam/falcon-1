import test from 'node:test';
import assert from 'node:assert/strict';
import { ContractIds } from '../src/contracts.js';
import { buildFsatsAuthoritativeData } from '../src/composition/fsats-authoritative-data.js';

const account = { brokerId:'B1', brokerAccountId:'A1', environment:'PAPER' };
const envelope = (contractId, availabilityState='AVAILABLE') => ({
  projectionId:`P-${contractId}`,
  contractId,
  version:'1',
  account,
  asOfTime:'2026-08-16T12:00:00Z',
  truthState:availabilityState === 'AVAILABLE' ? 'CURRENT' : 'UNKNOWN',
  freshnessState:availabilityState === 'AVAILABLE' ? 'CURRENT' : 'UNKNOWN',
  completeness:availabilityState === 'AVAILABLE' ? 'COMPLETE' : 'UNKNOWN',
  availabilityState,
  evidenceReference:'evidence-1',
  reasonCode:availabilityState === 'AVAILABLE' ? 'AVAILABLE' : 'NO_SOURCE'
});

test('no-source portfolio values remain null and are not coerced to zero', () => {
  const data = buildFsatsAuthoritativeData({
    portfolioSummary:{
      envelope:envelope(ContractIds.portfolioSummary,'UNSUPPORTED'),
      currency:'USD',
      totalEquity:null,
      cash:null,
      marketValue:null,
      reservedCapital:null,
      realizedPnl:null,
      unrealizedPnl:null
    },
    positions:{
      envelope:envelope(ContractIds.positions,'UNSUPPORTED'),
      positions:[],
      page:{continuationToken:null,hasMore:false,pageSize:50}
    }
  });
  assert.equal(data.portfolio.totalEquity,null);
  assert.equal(data.portfolio.cash,null);
  assert.deepEqual(data.positions,[]);
  assert.equal(data.sourceKind,'AUTHORITATIVE_PUBLIC_CONTRACTS');
  assert.equal(data.transportAuthorityCreated,false);
});

test('catalog NOT_APPLICABLE remains visible disabled with reason', () => {
  const data = buildFsatsAuthoritativeData({
    strategyCatalog:{
      projectionId:'CAT-1',
      requestId:'REQ-1',
      truthState:'CURRENT',
      freshnessState:'CURRENT',
      completeness:'COMPLETE',
      availabilityState:'AVAILABLE',
      evidenceReference:'catalog-evidence',
      reasonCode:'AVAILABLE',
      strategies:[{
        strategyId:'S1', strategyName:'Strategy One', schoolId:'SC1', schoolName:'School One',
        applicability:'NOT_APPLICABLE', visible:true, enabled:false, reasonCode:'NOT_FOR_ASSET', explanation:'Not applicable to this asset.'
      }]
    }
  });
  assert.equal(data.catalog.length,1);
  assert.equal(data.catalog[0].applicability,'NOT_APPLICABLE');
  assert.equal(data.catalog[0].enabled,false);
  assert.equal(data.catalog[0].reason,'Not applicable to this asset.');
});

test('invalid no-source payload is rejected rather than displayed', () => {
  assert.throws(() => buildFsatsAuthoritativeData({
    portfolioSummary:{
      envelope:envelope(ContractIds.portfolioSummary,'NOT_APPLICABLE'),
      currency:'USD',
      totalEquity:0,
      cash:null,
      marketValue:null,
      reservedCapital:null,
      realizedPnl:null,
      unrealizedPnl:null
    }
  }),/NO_SOURCE_PORTFOLIO_SUMMARY/);
});
