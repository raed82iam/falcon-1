# FSAPMA Permanent Streaming Provider Registry

**Status:** `OWNER_REQUESTED / APPLICATION_REGISTERED / FRESH_REVIEW_REQUIRED`
**Branch:** `application-development`
**Scope:** FSAPMA operational market-data provider streaming endpoints
**Runtime Authority:** `NOT_GRANTED`
**External Provider Connectivity:** `NOT_GRANTED`

## Purpose

This record registers the Project Owner-requested streaming endpoints as persistent FSAPMA provider-catalog inputs in addition to the existing REST/API provider set. Registration means FSAPMA shall retain these sources as governed provider options until a later evidence-backed supersession, incompatibility, provider retirement, entitlement change, or Owner/governance decision changes the catalog.

Registration does not grant external egress, provider connectivity, credentials, Paper, Tiny-Live, Live, deployment, or Part 3 authority.

## Canonical streaming provider entries

| Provider | Market scope | Endpoint / template | Authentication | Controlling coverage meaning |
|---|---|---|---|---|
| Binance | Crypto Spot | `wss://stream.binance.com:9443/ws/{symbol}@trade` | Public market-data stream | Exchange-specific real-time trade stream; symbol is lowercase; not consolidated crypto-market truth |
| Coinbase Exchange | Crypto Spot | `wss://ws-feed.exchange.coinbase.com` | Public market-data feed; some channels require authentication | Exchange-specific market-data feed; subscribe by product/channel; continuity/sequence rules apply |
| Bybit | Crypto Spot | `wss://stream.bybit.com/v5/public/spot` | Public market-data stream | Exchange-specific public Spot stream; subscription/connection rules apply |
| Alpaca | US Equities | `wss://stream.data.alpaca.markets/v2/iex` | API-key authentication required | Real-time IEX-only feed; explicitly NOT full consolidated SIP coverage |
| Finnhub | US Equities / Crypto / FX as entitled | `wss://ws.finnhub.io?token={API_KEY_REFERENCE}` | API key required | Availability, symbols, asset classes and connection limits remain plan/entitlement dependent |

## Permanent-use rule

```text
REGISTERED_PROVIDER_STREAM
!= ALWAYS_HEALTHY
!= ALWAYS_ENTITLED
!= ALWAYS_AVAILABLE
!= ABSOLUTE_MARKET_TRUTH
```

A registered endpoint remains in the Provider Capability Graph as a standing candidate, but FSAPMA must continuously evaluate current capability, entitlement, health, quota/session constraints, freshness, continuity, data quality and provider-side changes before selecting it.

`PERMANENT` therefore means persistent catalog registration, not unchangeable endpoint bytes or unconditional operational eligibility.

## REST + Stream composition

These streams complement, not replace, the existing REST/API provider inventory.

```text
REST / SNAPSHOT / HISTORY
+ STREAM / CURRENT EVENTS
= PROVIDER CAPABILITY SET
```

REST/snapshot/history paths remain necessary for bootstrap, historical data, gap recovery, reconciliation, explicit lookup and verification. Streaming is used for low-latency current updates where supported.

## FSAPMA ownership

All operational external market-data streams remain FSAPMA-owned:

```text
EXTERNAL MARKET-DATA STREAM
-> FSAPMA
-> NORMALIZATION / PROVENANCE / CONTINUITY / QUALITY
-> GOVERNED DATA PRODUCT
-> AUTHORIZED CONSUMER
```

Trading, Guardian, FSTSimA and Shared Web shall not bypass FSAPMA to connect directly to these market-data streams.

## Continuity requirements

For every stream, FSAPMA must preserve as applicable:

- exact Provider / ProviderAccount / Environment / ServiceRole / credential-reference identity;
- stream/session identity;
- subscribed instrument/product/channel set;
- source/event time versus receive time;
- sequence or provider-native continuity evidence where available;
- duplicate and out-of-order handling;
- reconnect state;
- explicit gaps and missing intervals;
- correction/supersession lineage;
- snapshot/stream reconciliation when required;
- stale/degraded/unavailable truth state;
- connection/session/rate limits;
- entitlement and redistribution/use-right constraints.

Mandatory invariants:

```text
RECONNECTED != GAP_FREE
RECENT_EVENT != COMPLETE_STREAM
NO_EVENT != NO_CHANGE
STREAM_EVENT != ABSOLUTE_MARKET_TRUTH
PROVIDER_A != PROVIDER_B
EXCHANGE_A_VIEW != CONSOLIDATED_MARKET_VIEW
```

## Coverage interpretation

The combined existing REST/API provider set plus these five streaming sources materially improves provider diversity, live-data availability, fallback options and cross-source evidence.

It SHALL NOT be described as complete market coverage without evidence for the exact market and data product:

- Binance, Coinbase and Bybit are exchange-specific crypto views.
- Alpaca free IEX is one US equity venue/feed and is not SIP consolidated coverage.
- Finnhub coverage is subject to plan, entitlement and endpoint limitations.

The Provider Controller may combine eligible sources for resilience and quality assessment, but source count alone does not create certainty or complete-market truth.

## Current authority state

FCR-0013 remains the Foundation-owned future operational-provider egress dependency. Therefore these endpoints are registered as Application-side provider catalog/configuration semantics only.

```text
STREAM_ENDPOINT_REGISTRY = REGISTERED
FSAPMA_PROVIDER_SELECTION_INPUT = YES
EXTERNAL_CONNECTION = NOT_AUTHORIZED
RUNTIME_ROUTE = NOT_AUTHORIZED
PAPER / SHADOW / TINY-LIVE / LIVE = NOT_AUTHORIZED
PART_3 = NOT_AUTHORIZED
```

This semantic addition must be included in the next fresh Architecture/Consistency review and the Owner-requested full Red-Team before executable test candidacy is frozen.
