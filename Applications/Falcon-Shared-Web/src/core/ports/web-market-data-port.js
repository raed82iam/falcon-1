import { TruthState } from '../../contracts.js';

/**
 * Web-owned presentation-only market-data boundary.
 *
 * This port is deliberately separate from FSATS. Data obtained through this
 * boundary may be rendered by Shared Web, but MUST NOT be fed back into FSATS
 * as operational/analysis truth or used to bypass FSAPMA.
 */
export const WebMarketDataPortMethods = Object.freeze([
  'marketUniverse',
  'marketSnapshot',
  'marketHistory',
  'marketStreamState'
]);

const stage12BoundRoute = ({ fcr, market, url, pathTemplate = null, credentialMode }) => Object.freeze({
  fcr,
  market,
  url,
  pathTemplate,
  credentialMode,
  foundationDisposition: 'STAGE12_ACCEPTED_AND_CLOSED',
  activation: 'WEB_BINDING_AND_VERIFICATION_PENDING'
});

export const WebMarketProviderRoutes = Object.freeze({
  BINANCE: stage12BoundRoute({
    fcr: 'FCR-0173',
    market: 'CRYPTO_SPOT',
    url: 'wss://stream.binance.com:9443',
    pathTemplate: '/ws/{symbol-lowercase}@trade',
    credentialMode: 'PUBLIC'
  }),
  COINBASE: stage12BoundRoute({
    fcr: 'FCR-0174',
    market: 'CRYPTO_SPOT',
    url: 'wss://ws-feed.exchange.coinbase.com',
    credentialMode: 'CHANNEL_DEPENDENT'
  }),
  BYBIT: stage12BoundRoute({
    fcr: 'FCR-0175',
    market: 'CRYPTO_SPOT',
    url: 'wss://stream.bybit.com/v5/public/spot',
    credentialMode: 'PUBLIC'
  }),
  ALPACA_IEX: stage12BoundRoute({
    fcr: 'FCR-0176',
    market: 'US_EQUITIES',
    url: 'wss://stream.data.alpaca.markets/v2/iex',
    credentialMode: 'API_CREDENTIAL_REFERENCE'
  }),
  FINNHUB: stage12BoundRoute({
    fcr: 'FCR-0177',
    market: 'US_EQUITIES',
    url: 'wss://ws.finnhub.io',
    pathTemplate: '?token={credential-reference}',
    credentialMode: 'API_CREDENTIAL_REFERENCE'
  })
});

const unavailable = extra => Object.freeze({ truth: TruthState.UNAVAILABLE, ...extra });

export function createUnavailableWebMarketDataPort() {
  return Object.freeze({
    async marketUniverse() { return unavailable({ items: [], source: null }); },
    async marketSnapshot() { return unavailable({ items: [], source: null }); },
    async marketHistory() { return unavailable({ bars: [], source: null }); },
    async marketStreamState() {
      return unavailable({
        connected: false,
        routes: WebMarketProviderRoutes,
        reasonCode: 'WEB_PROVIDER_BINDING_NOT_VERIFIED'
      });
    }
  });
}
