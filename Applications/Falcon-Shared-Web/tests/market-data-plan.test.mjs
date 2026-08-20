import test from 'node:test';
import assert from 'node:assert/strict';
import { MarketDataPlan, normalizePresentationObservation } from '../src/core/market-data-plan.js';

test('US equities use exact full universe/history routes plus dynamic live window',()=>{
  assert.equal(MarketDataPlan.US_EQUITIES.universe.provider,'ALPACA');
  assert.equal(MarketDataPlan.US_EQUITIES.universe.route.fcr,'FCR-0196');
  assert.equal(MarketDataPlan.US_EQUITIES.universe.route.url,'https://paper-api.alpaca.markets/v2/assets');
  assert.equal(MarketDataPlan.US_EQUITIES.history.provider,'ALPACA');
  assert.equal(MarketDataPlan.US_EQUITIES.history.route.fcr,'FCR-0197');
  assert.equal(MarketDataPlan.US_EQUITIES.history.route.url,'https://data.alpaca.markets/v2/stocks/bars');
  assert.equal(MarketDataPlan.US_EQUITIES.live.mode,'DYNAMIC_WINDOW');
  assert.deepEqual(MarketDataPlan.US_EQUITIES.live.providers,['ALPACA_IEX','FINNHUB']);
});

test('crypto uses exact universe/history and broad-market route with secondary live routes',()=>{
  assert.equal(MarketDataPlan.CRYPTO_SPOT.universe.provider,'BINANCE');
  assert.equal(MarketDataPlan.CRYPTO_SPOT.universe.route.fcr,'FCR-0198');
  assert.equal(MarketDataPlan.CRYPTO_SPOT.history.provider,'BINANCE');
  assert.equal(MarketDataPlan.CRYPTO_SPOT.history.route.fcr,'FCR-0199');
  assert.equal(MarketDataPlan.CRYPTO_SPOT.live.broadMarketRoute.fcr,'FCR-0200');
  assert.equal(MarketDataPlan.CRYPTO_SPOT.live.broadMarketRoute.url,'wss://stream.binance.com:9443/ws/!miniTicker@arr');
  assert.deepEqual(MarketDataPlan.CRYPTO_SPOT.live.providers,['BINANCE','COINBASE','BYBIT']);
});

test('all exact REST/broad-market destinations remain Web-binding pending rather than active',()=>{
  const routes = [
    MarketDataPlan.US_EQUITIES.universe.route,
    MarketDataPlan.US_EQUITIES.history.route,
    MarketDataPlan.CRYPTO_SPOT.universe.route,
    MarketDataPlan.CRYPTO_SPOT.history.route,
    MarketDataPlan.CRYPTO_SPOT.live.broadMarketRoute
  ];
  for (const route of routes) {
    assert.equal(route.foundationDisposition,'STAGE12_ACCEPTED_AND_CLOSED');
    assert.equal(route.activation,'WEB_BINDING_AND_VERIFICATION_PENDING');
  }
});

test('normalized Web market observation can never become FSATS operational input',()=>{
  const observation=normalizePresentationObservation({providerSymbol:'AAPL',market:'US_EQUITIES',price:200,source:'ALPACA_IEX',freshness:'CURRENT'});
  assert.equal(observation.presentationOnly,true);
  assert.equal(observation.eligibleForFsatsInput,false);
  assert.equal(observation.price,200);
});
