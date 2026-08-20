/*
 * Exact presentation-only destinations required by the full-market plan.
 *
 * Foundation Stage 12 is implemented, governed-verified and Owner accepted/closed.
 * These records still remain fail-closed because Foundation completion does not
 * activate Shared Web connectivity. Web must complete its own principal/policy/
 * credential-reference binding and governed verification before activation.
 */
export const PendingMarketDataDestinations = Object.freeze([
  Object.freeze({
    fcr:'FCR-0196',
    provider:'ALPACA',
    purpose:'US_EQUITY_UNIVERSE',
    url:'https://paper-api.alpaca.markets/v2/assets',
    credentialMode:'API_CREDENTIAL_REFERENCE',
    foundationDisposition:'STAGE12_ACCEPTED_AND_CLOSED',
    activation:'WEB_BINDING_AND_VERIFICATION_PENDING'
  }),
  Object.freeze({
    fcr:'FCR-0197',
    provider:'ALPACA',
    purpose:'US_EQUITY_HISTORICAL_BARS',
    url:'https://data.alpaca.markets/v2/stocks/bars',
    credentialMode:'API_CREDENTIAL_REFERENCE',
    foundationDisposition:'STAGE12_ACCEPTED_AND_CLOSED',
    activation:'WEB_BINDING_AND_VERIFICATION_PENDING'
  }),
  Object.freeze({
    fcr:'FCR-0198',
    provider:'BINANCE',
    purpose:'CRYPTO_SPOT_UNIVERSE',
    url:'https://data-api.binance.vision/api/v3/exchangeInfo',
    credentialMode:'PUBLIC',
    foundationDisposition:'STAGE12_ACCEPTED_AND_CLOSED',
    activation:'WEB_BINDING_AND_VERIFICATION_PENDING'
  }),
  Object.freeze({
    fcr:'FCR-0199',
    provider:'BINANCE',
    purpose:'CRYPTO_SPOT_HISTORICAL_KLINES',
    url:'https://data-api.binance.vision/api/v3/klines',
    credentialMode:'PUBLIC',
    foundationDisposition:'STAGE12_ACCEPTED_AND_CLOSED',
    activation:'WEB_BINDING_AND_VERIFICATION_PENDING'
  }),
  Object.freeze({
    fcr:'FCR-0200',
    provider:'BINANCE',
    purpose:'CRYPTO_SPOT_BROAD_MARKET_MINI_TICKER',
    url:'wss://stream.binance.com:9443/ws/!miniTicker@arr',
    credentialMode:'PUBLIC',
    foundationDisposition:'STAGE12_ACCEPTED_AND_CLOSED',
    activation:'WEB_BINDING_AND_VERIFICATION_PENDING'
  })
]);
