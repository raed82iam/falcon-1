# FSATS Shared-Web Portfolio Contract Binding v1

**Status:** `APPLICATION_PUBLIC_PAYLOAD_CONTRACT_MATERIALIZED / EXECUTABLE_PUBLIC_ROUTE_NOT_MATERIALIZED / RUNTIME_ROUTE_NOT_GRANTED`  
**Owner:** `Falcon Self-Aware Trading Application`  
**Consumer:** `Shared Web Application`  
**Customer/User Identity Owner:** `Shared Web Application`

## 1. Identity and wire rules

FSATS does not accept a customer/user principal for this family. Web resolves authenticated customer context to explicit broker-account scopes before invocation.

Canonical JSON field names are lower camel case. Enums serialize by the exact uppercase semantic tokens shown below, not ordinal values. Timestamps are RFC3339/ISO-8601 UTC strings. Decimal values serialize as JSON numbers and MUST NOT be fabricated when source truth is absent.

`ExactBrokerAccountScope` wire shape:

```json
{
  "brokerId": "ALPACA",
  "brokerAccountId": "PA-001",
  "environment": "PAPER"
}
```

All three fields are required, non-empty strings. `brokerId` and `environment` are canonical uppercase identities. `brokerAccountId` is opaque and case-preserving. Multi-account requests contain a non-empty array of distinct scopes. No user/customer/contact identifier is legal inside these Application payloads.

## 2. Common enums

`truthState`: `CURRENT | LAST_KNOWN | STALE | UNKNOWN | SIMULATION | REPLAY`  
`freshnessState`: `CURRENT | STALE | UNKNOWN | UNAVAILABLE`  
`completeness`: `COMPLETE | PARTIAL | UNKNOWN`  
`availabilityState`: `AVAILABLE | UNSUPPORTED | NOT_APPLICABLE | UNKNOWN | UNAVAILABLE | DEGRADED`

Rules:
- `NO_SOURCE_VALUE != ZERO`: unavailable numeric values are `null`.
- `UNSUPPORTED` and `NOT_APPLICABLE` require the numeric/derived value to be `null` unless the field has an independent supported meaning.
- `UNKNOWN`, `UNAVAILABLE`, `STALE`, `PARTIAL`, and `DEGRADED` are explicit states, never inferred from zero/empty values.
- `reasonCode` is always required on projection envelopes/items and uses a stable Application-owned reason token. `limitationDetail` is optional explanatory text and MUST NOT be parsed as authority.

## 3. Common projection envelope

Required fields:

```text
projectionId: string
contractId: string
version: string
account: ExactBrokerAccountScope
asOfTime: timestamp
truthState: enum
freshnessState: enum
completeness: enum
availabilityState: enum
evidenceReference: string
reasonCode: string
```

Optional:

```text
limitationDetail: string|null
correctsProjectionId: string|null
supersedesProjectionId: string|null
```

A correction has `correctsProjectionId != null`. An ordinary newer projection has neither correction nor supersession identity unless it intentionally replaces a prior semantic instance. Supersession is explicit through `supersedesProjectionId`.

## 4. FSATS.WebPortfolioViewRequest.v1

Required:

```text
requestId: string
correlation.value: string
brokerAccounts: ExactBrokerAccountScope[] (1..N)
requestedAt: timestamp
```

Optional:

```text
pageSize: integer|null, >0 when present
positionContinuationToken: string|null
activityContinuationToken: string|null
performanceContinuationToken: string|null
```

Continuation tokens are opaque to Web and must be replayed only for the same contract version, broker-account scope set, filter/query semantics, and logical traversal. Web MUST NOT manufacture or decode them.

## 5. FSATS.WebPortfolioSummaryProjection.v1

Required: `envelope`, `currency`.  
Nullable decimal fields: `totalEquity`, `cash`, `marketValue`, `reservedCapital`, `realizedPnl`, `unrealizedPnl`.

A null numeric field plus the envelope availability/truth/freshness/reason semantics communicates unsupported/not-applicable/unknown/unavailable/stale/partial/degraded truth without zero filling.

## 6. FSATS.WebPositionCollectionProjection.v1

Required: `envelope`, `positions`, `page`.

Each position item requires:
`position.value`, `instrument.value`, `currency`, `truthState`, `freshnessState`, `reasonCode`.

Nullable decimals:
`quantity`, `averageCost`, `marketPrice`, `marketValue`, `unrealizedPnl`.

`page` shape:

```json
{"continuationToken":null,"hasMore":false,"pageSize":100}
```

If `hasMore=true`, `continuationToken` MUST be non-null/non-empty. Token ordering is Application-owned and stable only within the same v1 query identity.

## 7. FSATS.WebOrderTradeActivityProjection.v1

Required: `envelope`, `activity`, `page`.

Each activity item requires:
`order.value`, `instrument.value`, `state`, `currency`, `effectiveAt`, `truthState`, `freshnessState`, `reasonCode`.

Nullable decimals: `requestedQuantity`, `filledQuantity`, `averageFillPrice`.

`state` exact values:

`REQUESTED | ACCEPTED | PARTIALLY_FILLED | FILLED | CANCEL_REQUESTED | CANCELLED | REPLACEMENT_REQUESTED | REPLACED | REJECTED | UNKNOWN_BROKER_OUTCOME`

The lifecycle distinctions are semantic and MUST NOT be collapsed.

## 8. FSATS.WebPortfolioPerformanceProjection.v1

Required: `envelope`, `periodStart`, `periodEnd`, `currency`, `history`, `page`.

Nullable decimals:
`openingEquity`, `closingEquity`, `realizedPnl`, `unrealizedPnl`, `netPnl`, `returnPercent`.

Each history point requires `effectiveAt`, `truthState`, `freshnessState`, `reasonCode`; `equity`, `netPnl`, and `returnPercent` are nullable decimals. Pagination follows the same opaque continuation rule.

## 9. FSATS.WebPortfolioProjectionUpdate.v1

Required:

```text
updateId: string
updateSequence: int64, strictly increasing within exact broker-account update stream
updateKind: ORDINARY | CORRECTION | SUPERSESSION
correlation.value: string
account: ExactBrokerAccountScope
changedProjectionContractIds: string[]
projectionVersion: string
effectiveAt: timestamp
truthState: enum
freshnessState: enum
evidenceReference: string
reasonCode: string
```

Optional:

```text
correctsUpdateId: string|null
supersedesUpdateId: string|null
```

Rules:
- `CORRECTION` requires `correctsUpdateId` and does not become ordinary merely because its timestamp is newer.
- `SUPERSESSION` requires `supersedesUpdateId`.
- `ORDINARY` requires both lineage fields null.
- duplicate `updateId` with different content is an idempotency conflict.
- update ordering is by `updateSequence` within the exact broker-account stream; timestamps are evidence, not the sole ordering authority.

## 10. Canonical examples

### Portfolio summary CURRENT

```json
{
  "envelope":{"projectionId":"ps-100","contractId":"FSATS.WebPortfolioSummaryProjection.v1","version":"1","account":{"brokerId":"ALPACA","brokerAccountId":"PA-001","environment":"PAPER"},"asOfTime":"2026-08-15T17:00:00Z","truthState":"CURRENT","freshnessState":"CURRENT","completeness":"COMPLETE","availabilityState":"AVAILABLE","evidenceReference":"ev-100","reasonCode":"PORTFOLIO_CURRENT","limitationDetail":null,"correctsProjectionId":null,"supersedesProjectionId":null},
  "currency":"USD","totalEquity":10000.25,"cash":3000.25,"marketValue":7000.00,"reservedCapital":250.00,"realizedPnl":50.00,"unrealizedPnl":125.00
}
```

### Portfolio summary PARTIAL/UNAVAILABLE

```json
{
  "envelope":{"projectionId":"ps-101","contractId":"FSATS.WebPortfolioSummaryProjection.v1","version":"1","account":{"brokerId":"ALPACA","brokerAccountId":"PA-001","environment":"PAPER"},"asOfTime":"2026-08-15T17:01:00Z","truthState":"LAST_KNOWN","freshnessState":"UNAVAILABLE","completeness":"PARTIAL","availabilityState":"DEGRADED","evidenceReference":"ev-101","reasonCode":"BROKER_PORTFOLIO_SOURCE_PARTIAL","limitationDetail":"Current broker values unavailable; only bounded last-known fields are present.","correctsProjectionId":null,"supersedesProjectionId":null},
  "currency":"USD","totalEquity":null,"cash":3000.25,"marketValue":null,"reservedCapital":250.00,"realizedPnl":50.00,"unrealizedPnl":null
}
```

### Order/activity CURRENT

```json
{
  "envelope":{"projectionId":"oa-200","contractId":"FSATS.WebOrderTradeActivityProjection.v1","version":"1","account":{"brokerId":"ALPACA","brokerAccountId":"PA-001","environment":"PAPER"},"asOfTime":"2026-08-15T17:02:00Z","truthState":"CURRENT","freshnessState":"CURRENT","completeness":"COMPLETE","availabilityState":"AVAILABLE","evidenceReference":"ev-200","reasonCode":"ACTIVITY_CURRENT"},
  "activity":[{"order":{"value":"ord-1"},"instrument":{"value":"AAPL"},"state":"PARTIALLY_FILLED","requestedQuantity":10,"filledQuantity":4,"averageFillPrice":220.10,"currency":"USD","effectiveAt":"2026-08-15T17:01:50Z","truthState":"CURRENT","freshnessState":"CURRENT","reasonCode":"BROKER_PARTIAL_FILL_CONFIRMED"}],
  "page":{"continuationToken":null,"hasMore":false,"pageSize":100}
}
```

### Order/activity non-current UNKNOWN BROKER OUTCOME

```json
{
  "envelope":{"projectionId":"oa-201","contractId":"FSATS.WebOrderTradeActivityProjection.v1","version":"1","account":{"brokerId":"ALPACA","brokerAccountId":"PA-001","environment":"PAPER"},"asOfTime":"2026-08-15T17:03:00Z","truthState":"UNKNOWN","freshnessState":"UNKNOWN","completeness":"PARTIAL","availabilityState":"DEGRADED","evidenceReference":"ev-201","reasonCode":"BROKER_OUTCOME_RECONCILIATION_REQUIRED"},
  "activity":[{"order":{"value":"ord-2"},"instrument":{"value":"MSFT"},"state":"UNKNOWN_BROKER_OUTCOME","requestedQuantity":5,"filledQuantity":null,"averageFillPrice":null,"currency":"USD","effectiveAt":"2026-08-15T17:02:55Z","truthState":"UNKNOWN","freshnessState":"UNKNOWN","reasonCode":"SUBMISSION_OUTCOME_UNKNOWN"}],
  "page":{"continuationToken":null,"hasMore":false,"pageSize":100}
}
```

## 11. Public binding metadata

Public payload assembly: `Falcon.FSATS.Trading.Contracts`.

Current source path:
`applications/FSATS/src/Trading/Falcon.FSATS.Trading.Contracts/TradingContracts.cs`

Pattern:
`Web request -> exact broker-account scoped Trading projection request -> bounded projection response`, plus optional projection-update messages.

**No executable public transport route/message-bus route is materialized or authorized by this contract commit.** The consuming Web adapter may bind its own port to these public payload semantics, but actual cross-Application route activation remains governed future runtime work under FCR-0133 and the normal Foundation/runtime authorization gates. No route name is invented here.

## 12. Compatibility/versioning

- `v1` consumers reject unknown major contract IDs/versions fail-closed.
- additive optional fields may be ignored only when their omission cannot change authority, truth, identity, units, ordering, lifecycle state, or null semantics.
- any change to broker-account identity, enum meaning, required field, unit/currency meaning, correction/order semantics, pagination token scope, authority/truth semantics, or nullable-vs-zero meaning requires a new semantic version/contract identity and fresh Application/Web review.
- Web must never coerce a future version into v1 by dropping unknown authority/truth fields.

## 13. Authority non-grants

`WEB_DISPLAY != PORTFOLIO_TRUTH_OWNER`  
`WEB_DISPLAY != EXECUTION_TRUTH_OWNER`  
`WEB_DISPLAY != BROKER_TRUTH_OWNER`  
`WEB_DISPLAY != PERFORMANCE_CALCULATION_AUTHORITY`  
`PROJECTION != EXECUTION_AUTHORITY`

This contract creates no runtime route activation, provider/broker connectivity, Paper/Shadow/Tiny-Live/Live, secret-byte ownership, or deployment authority.
