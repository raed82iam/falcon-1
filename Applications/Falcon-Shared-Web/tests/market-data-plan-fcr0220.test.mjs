import test from 'node:test';
import assert from 'node:assert/strict';
import { allocateSharedDiscreteQuota, decideQuotaCoordination, decideQuotaDimensions, MarketDataPlan } from '../src/core/market-data-plan.js';

test('independent Web source avoids FSAPMA quota sharing', () => {
  assert.deepEqual(decideQuotaCoordination({ hasSuitableIndependentPresentationSource:true }), {
    mode:'INDEPENDENT_SOURCE', webMaxShare:null, fsapmaReservedShare:null, reason:'NO_SHARED_POOL_REQUIRED'
  });
});

test('same provider name does not imply a shared quota pool when exact pool identities are known distinct', () => {
  const result = decideQuotaCoordination({
    hasSuitableIndependentPresentationSource:false,
    webQuotaPoolId:'WEB-POOL',
    fsapmaQuotaPoolId:'FSAPMA-POOL',
    quotaPoolConstrained:true,
    documentedLimitKnown:true
  });
  assert.equal(result.mode,'NO_SHARED_POOL');
  assert.equal(result.webMaxShare,null);
});

test('unknown quota pool identity never becomes assumed independent capacity', () => {
  for (const input of [
    { webQuotaPoolId:null, fsapmaQuotaPoolId:'POOL-A' },
    { webQuotaPoolId:'POOL-A', fsapmaQuotaPoolId:null },
    { webQuotaPoolId:null, fsapmaQuotaPoolId:null },
    { webQuotaPoolId:'', fsapmaQuotaPoolId:'POOL-A' }
  ]) {
    const result=decideQuotaCoordination({
      hasSuitableIndependentPresentationSource:false,
      quotaPoolConstrained:true,
      documentedLimitKnown:true,
      ...input
    });
    assert.equal(result.mode,'FAIL_CLOSED');
    assert.equal(result.reason,'QUOTA_POOL_IDENTITY_UNKNOWN');
    assert.equal(result.webMaxShare,0);
    assert.equal(result.fsapmaReservedShare,1);
  }
  assert.equal(MarketDataPlan.sourcingPolicy.unknownQuotaScopeImpliesIndependentCapacity,false);
});

test('shared constrained known pool applies the 50/50 fallback ceiling', () => {
  const result = decideQuotaCoordination({
    hasSuitableIndependentPresentationSource:false,
    webQuotaPoolId:'SHARED',
    fsapmaQuotaPoolId:'SHARED',
    quotaPoolConstrained:true,
    documentedLimitKnown:true
  });
  assert.equal(result.mode,'SHARED_POOL_FALLBACK');
  assert.equal(result.webMaxShare,0.5);
  assert.equal(result.fsapmaReservedShare,0.5);
});

test('unknown shared constrained limit fails closed', () => {
  const result = decideQuotaCoordination({
    hasSuitableIndependentPresentationSource:false,
    webQuotaPoolId:'SHARED',
    fsapmaQuotaPoolId:'SHARED',
    quotaPoolConstrained:true,
    documentedLimitKnown:false
  });
  assert.equal(result.mode,'FAIL_CLOSED');
  assert.equal(result.webMaxShare,0);
});

test('odd discrete shared quota leaves the remainder unallocated', () => {
  assert.deepEqual(allocateSharedDiscreteQuota(5), {
    mode:'SHARED_DISCRETE_50_50_CEILING',
    webMaxUnits:2,
    fsapmaMaxUnits:2,
    unallocatedSafetyRemainder:1,
    reason:'FCR_0220_ODD_REMAINDER_UNALLOCATED'
  });
});

test('invalid or unknown discrete quota fails closed', () => {
  const result = allocateSharedDiscreteQuota(null);
  assert.equal(result.mode,'FAIL_CLOSED');
  assert.equal(result.webMaxUnits,0);
  assert.equal(result.fsapmaMaxUnits,0);
});

test('missing quota dimensions fail closed when no independent Web source exists', () => {
  const result=decideQuotaDimensions({hasSuitableIndependentPresentationSource:false,dimensions:[]});
  assert.equal(result.mode,'FAIL_CLOSED');
  assert.equal(result.reason,'QUOTA_DIMENSIONS_REQUIRED');
  assert.deepEqual(result.dimensions,[]);
  assert.equal(MarketDataPlan.sourcingPolicy.missingQuotaDimensionsImplyUsableCapacity,false);
});

test('multiple provider quota dimensions are evaluated independently', () => {
  const result = decideQuotaDimensions({
    hasSuitableIndependentPresentationSource:false,
    dimensions:[
      {
        dimensionId:'PER_MINUTE_API_CREDITS',
        webQuotaPoolId:'SHARED-MINUTE',
        fsapmaQuotaPoolId:'SHARED-MINUTE',
        quotaPoolConstrained:true,
        documentedLimitKnown:true,
        discrete:true,
        totalUnits:101
      },
      {
        dimensionId:'WEBSOCKET_CONNECTION_LIMIT',
        webQuotaPoolId:'WEB-CONNECTIONS',
        fsapmaQuotaPoolId:'FSAPMA-CONNECTIONS',
        quotaPoolConstrained:true,
        documentedLimitKnown:true,
        discrete:true,
        totalUnits:5
      }
    ]
  });

  assert.equal(result.mode,'DIMENSIONAL_EVALUATION');
  assert.equal(result.dimensions[0].coordination.mode,'SHARED_POOL_FALLBACK');
  assert.equal(result.dimensions[0].unitAllocation.webMaxUnits,50);
  assert.equal(result.dimensions[0].unitAllocation.fsapmaMaxUnits,50);
  assert.equal(result.dimensions[0].unitAllocation.unallocatedSafetyRemainder,1);
  assert.equal(result.dimensions[1].coordination.mode,'NO_SHARED_POOL');
  assert.equal(result.dimensions[1].unitAllocation,null);
});

test('unknown quota identity inside one dimension makes the dimensional result fail closed', () => {
  const result=decideQuotaDimensions({
    hasSuitableIndependentPresentationSource:false,
    dimensions:[{
      dimensionId:'SOURCE_IP_POOL',
      webQuotaPoolId:null,
      fsapmaQuotaPoolId:'FSAPMA-IP-POOL',
      quotaPoolConstrained:true,
      documentedLimitKnown:true
    }]
  });
  assert.equal(result.mode,'PARTIAL_OR_FAIL_CLOSED');
  assert.equal(result.dimensions[0].coordination.reason,'QUOTA_POOL_IDENTITY_UNKNOWN');
  assert.equal(result.dimensions[0].failClosed,true);
});

test('missing or duplicate quota dimension identity fails closed instead of double-counting capacity', () => {
  const result=decideQuotaDimensions({
    hasSuitableIndependentPresentationSource:false,
    dimensions:[
      {dimensionId:'BURST',webQuotaPoolId:'A',fsapmaQuotaPoolId:'B',quotaPoolConstrained:true,documentedLimitKnown:true},
      {dimensionId:'BURST',webQuotaPoolId:'C',fsapmaQuotaPoolId:'D',quotaPoolConstrained:true,documentedLimitKnown:true},
      {dimensionId:'',webQuotaPoolId:'E',fsapmaQuotaPoolId:'F',quotaPoolConstrained:true,documentedLimitKnown:true}
    ]
  });
  assert.equal(result.mode,'PARTIAL_OR_FAIL_CLOSED');
  assert.equal(result.dimensions[1].coordination.reason,'DUPLICATE_QUOTA_DIMENSION_ID');
  assert.equal(result.dimensions[2].coordination.reason,'QUOTA_DIMENSION_ID_REQUIRED');
});

test('one unknown shared constrained dimension makes the dimensional result fail closed', () => {
  const result = decideQuotaDimensions({
    hasSuitableIndependentPresentationSource:false,
    dimensions:[{
      dimensionId:'BURST_REQUEST_LIMIT',
      webQuotaPoolId:'SHARED-BURST',
      fsapmaQuotaPoolId:'SHARED-BURST',
      quotaPoolConstrained:true,
      documentedLimitKnown:false,
      discrete:true,
      totalUnits:null
    }]
  });
  assert.equal(result.mode,'PARTIAL_OR_FAIL_CLOSED');
  assert.equal(result.dimensions[0].coordination.mode,'FAIL_CLOSED');
  assert.equal(result.dimensions[0].failClosed,true);
});
