import test from 'node:test';
import assert from 'node:assert/strict';
import {
  FsatsRuntimePortMethods,
  createUnavailableFsatsRuntimePort
} from '../src/core/ports/fsats-runtime-port.js';
import {
  FalconSystemRuntimePortMethods,
  createUnavailableFalconSystemRuntimePort
} from '../src/core/ports/falcon-system-runtime-port.js';
import {
  WebMarketDataPortMethods,
  WebMarketProviderRoutes,
  createUnavailableWebMarketDataPort
} from '../src/core/ports/web-market-data-port.js';
import {
  RuntimePortMethods,
  assertRuntimePort,
  createUnavailableRuntimePort
} from '../src/core/runtime-port.js';

test('runtime methods are partitioned into explicit owning contract families', () => {
  const allFamilies=[FalconSystemRuntimePortMethods,FsatsRuntimePortMethods,WebMarketDataPortMethods];
  for (let i=0;i<allFamilies.length;i++) for (let j=i+1;j<allFamilies.length;j++) {
    assert.deepEqual(allFamilies[i].filter(method=>allFamilies[j].includes(method)),[]);
  }
  assert.deepEqual(new Set(RuntimePortMethods),new Set(allFamilies.flat()));
});

test('FSATS unavailable port covers governed Application projections fail closed', async () => {
  const port = createUnavailableFsatsRuntimePort();
  for (const method of FsatsRuntimePortMethods) assert.equal((await port[method]()).truth,'UNAVAILABLE');
  assert.deepEqual((await port.activity()).items, []);
  assert.deepEqual((await port.tradingOverlay()).items, []);
  assert.equal((await port.onDemandAnalysis()).resultState, 'UNAVAILABLE');
  assert.equal((await port.chart()).compatibilityOnly,true);
});

test('Web presentation market-data routes are configured but not activated', async () => {
  const port=createUnavailableWebMarketDataPort();
  const state=await port.marketStreamState();
  assert.equal(state.truth,'UNAVAILABLE');
  assert.equal(state.connected,false);
  assert.equal(state.reasonCode,'WEB_PROVIDER_BINDING_NOT_VERIFIED');
  assert.equal(WebMarketProviderRoutes.BINANCE.url,'wss://stream.binance.com:9443');
  assert.equal(WebMarketProviderRoutes.COINBASE.url,'wss://ws-feed.exchange.coinbase.com');
  assert.equal(WebMarketProviderRoutes.BYBIT.url,'wss://stream.bybit.com/v5/public/spot');
  assert.equal(WebMarketProviderRoutes.ALPACA_IEX.url,'wss://stream.data.alpaca.markets/v2/iex');
  assert.equal(WebMarketProviderRoutes.FINNHUB.url,'wss://ws.finnhub.io');
});

test('Falcon system unavailable port remains Foundation-authority neutral', async () => {
  const port = createUnavailableFalconSystemRuntimePort();
  assert.deepEqual((await port.applications()).items, []);
  assert.equal((await port.systemOverview()).truth, 'UNAVAILABLE');
});

test('aggregate unavailable runtime port remains complete and fail-closed', async () => {
  const port = createUnavailableRuntimePort();
  assert.equal(assertRuntimePort(port), port);
  for (const method of RuntimePortMethods) assert.equal((await port[method]()).truth,'UNAVAILABLE');
});
