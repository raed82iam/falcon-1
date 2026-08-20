# FCR-0125 / FCR-0126 / FCR-0127 — Application Contract Clarification

**Status:** `APPLICATION_RESPONSE_DEFINED / CROSS_WORKSTREAM_HANDOFF_READY`  
**Branch:** `application-development`  
**Scope:** Shared Web presentation/request contracts for chart market data, School/Strategy overlays, and on-demand analysis outside the active Trading universe  
**Runtime Authority:** `NOT_GRANTED`  
**Provider/Broker Connectivity Authority:** `NOT_GRANTED`  
**Paper/Shadow/Tiny-Live/Live Authority:** `NOT_GRANTED`

## 1. Governing Boundary

This clarification preserves the current accepted FSATS ownership model and reuses the existing accepted P0-F Application-to-Application contract families. It does not create a new Falcon Application, Foundation service, runtime route, provider connection, broker connection, trading authority, or universe-admission authority.

Controlling distinctions:

```text
WEB = PRESENTATION + USER-INTENT TRANSPORT
WEB != MARKET-DATA TRUTH OWNER
WEB != STRATEGY OWNER
WEB != ANALYSIS OWNER
WEB != TRADING-UNIVERSE OWNER

FSAPMA = OPERATIONAL MARKET-DATA / DATA-PRODUCT OWNER
TRADING = MARKET/INSTRUMENT ELIGIBILITY + ANALYSIS + SCHOOL/STRATEGY SEMANTICS OWNER

CUSTOMER_VIEW_REQUEST != TRADING_UNIVERSE_ADMISSION
CUSTOMER_ANALYSIS_REQUEST != TRADING_UNIVERSE_ADMISSION
CUSTOMER_ANALYSIS_REQUEST != STRATEGY_ACTIVATION
DISPLAYED_OVERLAY != CURRENT_APPLICATION_TRUTH
WEB_REQUEST_TRANSPORT != BUSINESS AUTHORITY
```

The existing accepted P0-F families used by this clarification are:

```text
Web -> FSAPMA:
falcon.xapp.shared.web.trading.fsapma.user-intent

FSAPMA -> Web:
falcon.xapp.trading.fsapma.shared.web.presentation-projection
falcon.xapp.trading.fsapma.shared.web.user-intent-outcome

Web -> Trading:
falcon.xapp.shared.web.trading.core.user-intent

Trading -> Web:
falcon.xapp.trading.core.shared.web.presentation-projection
falcon.xapp.trading.core.shared.web.user-intent-outcome
```

No new contract family is required by FCR-0125, FCR-0126, or FCR-0127. These FCRs define exact permitted message purposes and projection semantics inside already accepted families.

---

# 2. FCR-0125 — Chart Market Data

## 2.1 Canonical ownership and flow

```text
WEB CHART REQUEST
-> Web/FSAPMA USER_INTENT family
-> FSAPMA validates request and resolves provider-neutral data availability
-> FSAPMA obtains/constructs governed Data Product through its own provider path when runtime authority exists
-> FSAPMA/Web PRESENTATION_PROJECTION and USER_INTENT_OUTCOME families
-> WEB renders only returned truth
```

Shared Web SHALL NOT select an external provider, use provider credentials, bypass FSAPMA, or upgrade stale/partial/unknown data to current truth.

## 2.2 Canonical chart request shape

Application-side semantic message identity: `FSATS.WebChartDataRequest.v1`.

Required fields:

```text
RequestId
CorrelationId
RequestingApplicationId = SHARED_WEB
RequestedInstrumentReference
MarketOrVenueHint?            // optional only when the user input is otherwise ambiguous
AssetClassHint?               // optional only when needed for disambiguation
Timeframe
RangeStart
RangeEnd
RequestedSeriesType           // e.g. OHLCV/BAR, PRICE_SERIES, VOLUME where supported
RequestedAt
EntitlementReference?         // governed reference only where required
```

`RequestedInstrumentReference` may be an already canonical Falcon instrument identity or a user-supplied resolvable identity. If identity is ambiguous, FSAPMA SHALL NOT guess. The response is `NEEDS_CLARIFICATION` with bounded candidate/reference information sufficient for Web to ask the customer to choose.

Data identity resolution for display does not establish Trading eligibility.

```text
DISPLAY_DATA_IDENTITY_RESOLVED != TRADING_INSTRUMENT_ADMITTED
```

## 2.3 Historical response

Application-side semantic projection identity: `FSATS.WebChartHistoricalProjection.v1`.

Required semantics:

```text
RequestId
CorrelationId
ResolvedInstrumentIdentity
Timeframe
RequestedRange
ReturnedRange
SeriesState
Points/Bars
AsOfTime
TruthState
FreshnessState
CompletenessState
GapRanges[]
CorrectionOrRevisionIdentity?
ReasonCode?
```

For OHLCV/bar data, each bar shall carry the canonical equivalent of:

```text
BucketStart
BucketEnd
Open
High
Low
Close
Volume?          // unavailable/not-applicable must remain explicit
TruthState
```

Response states:

```text
COMPLETED
PARTIAL
UNAVAILABLE
UNSUPPORTED
NEEDS_CLARIFICATION
REJECTED
```

Truth/freshness states shall preserve at least current/stale/unknown/unavailable/degraded distinctions as applicable to the FSAPMA Data Product.

## 2.4 Historical-to-current continuation

Application-side semantic projection identity: `FSATS.WebChartUpdateProjection.v1`.

The update stream shall preserve:

```text
RequestId / StreamIdentity
CorrelationId
ResolvedInstrumentIdentity
Timeframe
UpdateType = NEW | CORRECTION | SUPERSESSION | GAP | STATUS
ObservationTime
EffectiveTime
TruthState
FreshnessState
Payload
RelatedObservationOrBarId?
ReasonCode?
```

A reconnect does not prove continuity. Missing intervals remain explicit until reconciled.

## 2.5 Instrument outside active Trading universe

Chart display is allowed as a request semantic when the instrument can be resolved and FSAPMA has an authorized/supported Data Product path. Servicing a chart request has no Trading-universe side effect.

```text
ON_DEMAND_CHART_DISPLAY
!= CANDIDATE_UNIVERSE_ADMISSION
!= MANAGED_INSTRUMENT_ADMISSION
!= STRATEGY_ELIGIBILITY
!= CAPITAL_AUTHORITY
!= EXECUTION_AUTHORITY
```

If the instrument belongs to a market/data scope that FSAPMA cannot support under the current admitted capability, the response is `UNSUPPORTED` or `UNAVAILABLE`. Web displays that result and does not source alternative operational market data itself.

---

# 3. FCR-0126 — School and Strategy Chart Overlays

## 3.1 Canonical ownership and flow

```text
WEB OVERLAY REQUEST
-> Web/Trading USER_INTENT family
-> Trading resolves instrument/context and School/Strategy identity
-> Trading evaluates applicability using Trading-owned Market Profile, Analysis, School and Strategy semantics
-> Trading returns bounded render projection and business outcome
-> WEB renders projection without recomputing Trading semantics
```

Application-side request identity: `FSATS.WebTradingOverlayRequest.v1`.

Required request fields:

```text
RequestId
CorrelationId
RequestingApplicationId = SHARED_WEB
InstrumentReference
MarketOrVenueContext?
Timeframe
RangeStart
RangeEnd
OverlaySubjectKind = SCHOOL | STRATEGY
OverlaySubjectId
RequestedAt
EntitlementReference?       // governed reference where required
```

## 3.2 Applicability result

Trading owns applicability. Web does not infer it.

Application-side result states:

```text
APPLICABLE
NOT_APPLICABLE
PARTIAL
UNAVAILABLE
UNKNOWN
NEEDS_CLARIFICATION
REJECTED
```

Mandatory distinctions:

```text
USER_SELECTED_OVERLAY != APPLICATION_CONFIRMED_APPLICABILITY
WEB_RENDER_ATTEMPT != STRATEGY_ACTIVATION
OVERLAY_APPLICABLE != TRADE_AUTHORIZED
```

## 3.3 Render projection

Application-side semantic projection identity: `FSATS.WebTradingOverlayProjection.v1`.

Required envelope semantics:

```text
RequestId
CorrelationId
OverlayProjectionId
OverlaySubjectKind
OverlaySubjectId
ResolvedInstrumentIdentity
Timeframe
Range
ApplicabilityState
TruthState
AsOfTime
ProjectionVersion
Elements[]
ReasonCode?
```

Each `Element` is a provider-neutral rendering primitive owned semantically by Trading and may be one of:

```text
POINT
PRICE_LEVEL
HORIZONTAL_LINE
VERTICAL_LINE
ZONE
SERIES
MARKER
ANNOTATION
```

Each element shall carry only what Web needs to draw it, such as time anchor/range, price/value coordinate(s), bounded label/tooltip reference, semantic type, state, and stable element identity. Web shall not recreate strategy logic from those primitives.

## 3.4 Update, correction, invalidation and removal

Application-side update identity: `FSATS.WebTradingOverlayUpdate.v1`.

Update types:

```text
ADD
UPDATE
CORRECT
INVALIDATE
REMOVE
STATUS
```

Every update binds the exact `OverlayProjectionId` and element identity where applicable.

```text
DISPLAYED_OVERLAY != CURRENT_APPLICATION_TRUTH
```

If Trading invalidates/removes an element, Web shall stop presenting it as current Trading truth even if historical display is retained visually under an explicit historical state.

## 3.5 Market-data relationship

Overlay semantics and chart price data remain separate:

```text
FSAPMA MARKET-DATA PROJECTION
+
TRADING OVERLAY PROJECTION
-> WEB VISUAL COMPOSITION ONLY
```

Web composition does not merge ownership or create new business meaning.

---

# 4. FCR-0127 — On-Demand Analysis Outside Active Universe

## 4.1 Canonical flow

```text
WEB ANALYSIS REQUEST
-> Web/Trading USER_INTENT family
-> Trading resolves requested instrument and admitted market context
-> Trading determines whether on-demand analysis is serviceable
-> Trading requests any required Data Products from FSAPMA through Trading/FSAPMA governed contracts
-> Trading runs only currently admitted/available analysis methods
-> Trading returns a bounded customer-facing analysis result
-> WEB presents the returned result without upgrading confidence or eligibility
```

Application-side request identity: `FSATS.WebOnDemandAnalysisRequest.v1`.

Required fields:

```text
RequestId
CorrelationId
RequestingApplicationId = SHARED_WEB
RequestedInstrumentReference
MarketOrVenueHint?
AssetClassHint?
AnalysisIntent
RequestedAt
EntitlementReference?
```

## 4.2 Instrument resolution

Trading T-LSA-02 remains owner of Trading market/instrument identity and eligibility semantics.

If the user-supplied identity is ambiguous:

```text
RESULT = NEEDS_CLARIFICATION
```

Trading returns bounded candidate identities/context. Web asks the customer to clarify and resubmits a new/continued correlated request.

Web shall not silently choose an exchange, market, asset class, or instrument.

## 4.3 Serviceability states

Application-side result identity: `FSATS.WebOnDemandAnalysisResult.v1`.

States:

```text
COMPLETED
PARTIAL
UNAVAILABLE
UNSUPPORTED
NEEDS_CLARIFICATION
REJECTED
```

The result shall bind:

```text
RequestId
CorrelationId
AnalysisResultId
ResolvedInstrumentIdentity
AnalysisIntent
ResultState
AnalysisProjection
AsOfTime
InputTruth/Freshness summary
ConfidenceOrStrength?          // only where owned/exposed by the analysis method
Limitations[]
ReasonCode?
```

## 4.4 Outside-list rule

An instrument may be analyzed on demand without being inserted into the Candidate Universe or Managed Instrument Set when all of the following hold:

```text
market context is already within an admitted Trading market scope
instrument identity is resolvable
required FSAPMA Data Products are available/authorized
requested analysis method is applicable and available
no control/entitlement boundary forbids the request
```

The initial admitted Trading market scope remains US Equities and Crypto Spot unless separately changed by governed authority.

If the request concerns a market/asset class outside the admitted Trading scope, Trading returns `UNSUPPORTED` or `REJECTED` according to the exact reason. It does not silently expand scope.

## 4.5 No universe side effect

Default and controlling behavior:

```text
ON_DEMAND_ANALYSIS
!= SILENT_UNIVERSE_MUTATION
!= CANDIDATE_UNIVERSE_ADMISSION
!= MANAGED_POSITION_SET
!= STRATEGY_ACTIVATION
!= CAPITAL_RESERVATION
!= ORDER_INTENT
!= EXECUTION_AUTHORITY
```

A later decision to admit the instrument to the normal Candidate Universe is a separate Trading-owned governed decision using the ordinary T-LSA-02 universe logic.

---

# 5. Failure and Truth Rules Common to All Three FCRs

```text
TECHNICAL_RECEIPT != BUSINESS_ACCEPTANCE
REQUEST_ACCEPTED != RESULT_COMPLETED
CURRENT != STALE
PARTIAL != COMPLETE
UNKNOWN != AVAILABLE
DISPLAYED != AUTHORITATIVE_CURRENT
USER_REQUEST != TRADING_AUTHORITY
```

Transport duplicate/replay/idempotency, correlation/causation, expiry/freshness, exact producer/consumer identity, entitlement references, and evidence must use the existing governed P0-F/P1-K communication rules when implementation/runtime binding is later authorized.

No customer/user identity is introduced into Trading broker-account semantics by these contracts. Shared Web may correlate its own customer/session context internally, but FSATS consumes only the bounded request context required for the requested business operation.

---

# 6. Implementation and Lifecycle State

This document is an Application semantic/interface clarification for cross-workstream planning and FCR disposition.

```text
APPLICATION CONTRACT SEMANTICS = DEFINED
SHARED WEB CONSUMPTION/BINDING = PENDING WEB WORKSTREAM
RUNTIME ROUTE IMPLEMENTATION = NOT AUTHORIZED BY THIS RECORD
PROVIDER CONNECTIVITY = NOT GRANTED
BROKER CONNECTIVITY = NOT GRANTED
PAPER/SHADOW/TINY-LIVE/LIVE = NOT GRANTED
PART 3 = NOT AUTHORIZED / NOT STARTED
```

Any later executable message schema/route implementation remains subject to the then-current Part/implementation authority, Manifest declaration, Foundation transport capability, runtime binding, verification, and fresh review requirements.
