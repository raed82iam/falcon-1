import test from 'node:test';
import assert from 'node:assert/strict';
import { displayValue, catalogPresentation, ApplicabilityState, assertNoRegulatoryClaims } from '../src/contracts.js';

test('missing source value is not silently converted to zero', () => {
  assert.equal(displayValue(undefined), '—');
  assert.equal(displayValue(null), '—');
  assert.equal(displayValue(0), 0);
});

test('catalog present but not applicable is visible disabled with authoritative reason', () => {
  assert.deepEqual(catalogPresentation({availability:'AVAILABLE',applicability:ApplicabilityState.NOT_APPLICABLE,reason:'Not applicable to this asset'}), {visible:true,enabled:false,reason:'Not applicable to this asset'});
});

test('retired catalog item is not offered as current capability', () => {
  assert.equal(catalogPresentation({availability:'RETIRED',applicability:ApplicabilityState.APPLICABLE}).visible, false);
});

test('regulatory marketing claims are blocked by policy helper', () => {
  assert.equal(assertNoRegulatoryClaims('Licensed by CMA'), false);
  assert.equal(assertNoRegulatoryClaims('Falcon protects, manages and grows capital'), true);
});
