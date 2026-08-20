# Shared Web Market-Data FCR Reconciliation — 2026-08-15

Status: IMPLEMENTATION_IN_PROGRESS / EXTERNAL_ACTIVATION_FAIL_CLOSED
Branch: `web-development`
Writable scope: `applications/shared/web/**`

## Current Web-owned presentation routes already tracked

- FCR-0173 Binance WebSocket destination
- FCR-0174 Coinbase WebSocket destination
- FCR-0175 Bybit Spot WebSocket destination
- FCR-0176 Alpaca IEX WebSocket destination
- FCR-0177 Finnhub WebSocket destination

These routes are configured only as fail-closed presentation routes. They are not live connectivity authority.

## Full-market coverage requirement discovered during implementation reconciliation

The five existing WebSocket destinations are not by themselves sufficient to implement the complete presentation plan discussed with the Project Owner:

- full active US-equity instrument universe;
- US-equity historical bars/backfill;
- crypto Spot instrument universe;
- selected-symbol crypto historical bars/backfill;
- broad crypto market ticker presentation without one WebSocket subscription per symbol.

Because current governance requires exact external destinations to remain independently governed, those missing destinations must be represented by additional FCRs or exact existing-FCR amendments before activation. Web code therefore records these capabilities as `PENDING_EXACT_REST_FCR` / governed route requirements rather than silently connecting.

## Required separation

```text
WEB_PRESENTATION_DATA != FSATS_OPERATIONAL_DATA
WEB_RAW_DISPLAY_DATA -/-> FSATS_ANALYSIS_INPUT
WEB_PROVIDER_ROUTE != FSAPMA_PROVIDER_ROUTE
WEB_PROVIDER_CREDENTIAL != FSAPMA_PROVIDER_CREDENTIAL
SAME_PROVIDER != SAME_AUTHORITY
SAME_URL != SAME_AUTHORITY
```

## Current sourcing plan

### US equities

```text
Universe: Alpaca Web-owned presentation route, exact REST destination still separately governed
History/backfill: Alpaca Web-owned market-data REST route, exact destination still separately governed
Live: dynamic window using governed Alpaca IEX / Finnhub routes when activated
```

The dynamic live window prioritizes customer-visible instruments, open portfolio positions, active incident instruments, watchlist/monitored instruments and explicitly requested instruments. The universe itself remains broader than the live subscription window.

### Crypto Spot

```text
Universe: Binance public market-data route, exact REST destination separately governed
History/backfill: Binance selected-symbol public market-data route, exact REST destination separately governed
Live: Binance broad-market primary plus Coinbase/Bybit on-demand/secondary routes when governed
```

## Credential rule

No secret byte is stored in this repository, chat, demo fixture or reusable Web log/state.

When activation reaches a route that requires a credential reference, the Project Owner will be asked only to provision the credential through the governed secure credential path available at that time. Credentials must not be pasted into chat.
