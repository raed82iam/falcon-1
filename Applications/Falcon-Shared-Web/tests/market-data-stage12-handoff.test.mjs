import test from 'node:test';
import assert from 'node:assert/strict';
import { PendingMarketDataDestinations } from '../src/core/market-data-destinations.pending.js';
import { WebMarketProviderRoutes, createUnavailableWebMarketDataPort } from '../src/core/ports/web-market-data-port.js';

const expectedFoundationState = 'STAGE12_ACCEPTED_AND_CLOSED';
const expectedWebState = 'WEB_BINDING_AND_VERIFICATION_PENDING';

test('Stage 12 completion is recorded without activating Web market-data destinations', () => {
  assert.equal(PendingMarketDataDestinations.length, 5);
  for (const destination of PendingMarketDataDestinations) {
    assert.equal(destination.foundationDisposition, expectedFoundationState);
    assert.equal(destination.activation, expectedWebState);
    assert.notEqual(destination.activation, 'FOUNDATION_STAGE12_PENDING');
  }
});

test('Stage 12 completion is recorded without activating Web streaming routes', () => {
  for (const route of Object.values(WebMarketProviderRoutes)) {
    assert.equal(route.foundationDisposition, expectedFoundationState);
    assert.equal(route.activation, expectedWebState);
    assert.notEqual(route.activation, 'FOUNDATION_STAGE12_PENDING');
  }
});

test('default Web market-data port remains fail-closed until Web binding is governed-verified', async () => {
  const state = await createUnavailableWebMarketDataPort().marketStreamState();
  assert.equal(state.connected, false);
  assert.equal(state.reasonCode, 'WEB_PROVIDER_BINDING_NOT_VERIFIED');
  assert.equal(state.truth, 'UNAVAILABLE');
});
