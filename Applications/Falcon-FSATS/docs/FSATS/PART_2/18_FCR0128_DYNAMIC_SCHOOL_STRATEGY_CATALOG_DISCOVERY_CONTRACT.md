# FCR-0128 — Dynamic School / Strategy Catalog Discovery Contract

**Status:** `APPLICATION_RESPONSE_DEFINED / CROSS_WORKSTREAM_HANDOFF_READY`  
**Branch:** `application-development`  
**Scope:** Shared Web discovery/catalog projection for Trading-owned Schools and Strategies  
**Runtime Authority:** `NOT_GRANTED`  
**Provider/Broker Connectivity Authority:** `NOT_GRANTED`  
**Paper/Shadow/Tiny-Live/Live Authority:** `NOT_GRANTED`

## 1. Governing Boundary

This clarification reuses the accepted Trading and cross-Application ownership model. It does not create a new Falcon Application, Foundation service, runtime route, strategy activation path, trading authority, entitlement authority, or Web-owned business catalog.

Accepted ownership already establishes:

```text
T-LSA-04 = CLASSICAL TRADING SCHOOL OWNER
T-LSA-05 = OPPORTUNITY HUNTING SCHOOL OWNER
T-LSA-06 = CENTRAL STRATEGY REGISTRY / CONTROLLER OWNER
STRATEGIES = CENTRALLY REGISTERED, NOT DUPLICATED PER MARKET
WEB = PRESENTATION + USER-INTENT / INFORMATIONAL REQUEST SURFACE
```

The accepted P1-K family reused by this clarification is:

```text
P1K-019 — Shared Web Informational Query / Response
```

No new cross-Application semantic family is required by FCR-0128. This record defines an exact permitted discovery/catalog purpose inside that already accepted family.

Mandatory distinctions:

```text
WEB_SELECTOR_OPTIONS != HARD_CODED_FALCON_PRODUCT_TRUTH
TRADING_CATALOG = AUTHORITATIVE_DISCOVERY_SOURCE
CATALOG_PRESENT != APPLICABLE_TO_CURRENT_ASSET
CATALOG_AVAILABLE != STRATEGY_ACTIVATED
SELECTOR_VISIBLE != TRADE_AUTHORIZED
CATALOG_DISCOVERY != ENTITLEMENT_GRANT
```

---

## 2. Canonical Sender / Receiver Flow

```text
WEB CATALOG DISCOVERY REQUEST
-> Shared Web / Trading INFORMATIONAL_QUERY_RESPONSE family
-> Trading T-LSA-06 central Strategy Registry / Controller
-> Trading resolves current School / Strategy catalog state
-> Trading returns authoritative catalog projection
-> WEB renders only returned discoverable state
```

Shared Web SHALL NOT hard-code Falcon School/Strategy business truth, infer availability from historical UI state, or manufacture an entry that is absent from the current Trading catalog projection.

---

## 3. Canonical Request

Application-side semantic request identity:

```text
FSATS.WebStrategyCatalogRequest.v1
```

Required request semantics:

```text
RequestId
CorrelationId
RequestingApplicationId = SHARED_WEB
RequestedSubjectKinds[]?       // SCHOOL | STRATEGY; omitted means both
MarketScopeHint?               // optional discovery filter only
AssetClassHint?                // optional discovery filter only
EntitlementReference?          // governed reference only where applicable
KnownCatalogVersion?           // optional for refresh/delta semantics
RequestedAt
```

A Web-supplied market/asset/entitlement hint is a request filter, not authoritative Trading truth.

```text
WEB_FILTER_HINT != APPLICATION_APPLICABILITY_DECISION
WEB_ENTITLEMENT_REFERENCE != ENTITLEMENT_AUTHORITY
```

---

## 4. Canonical Catalog Projection

Application-side semantic projection identity:

```text
FSATS.WebStrategyCatalogProjection.v1
```

Projection envelope semantics:

```text
RequestId
CorrelationId
CatalogVersion
EffectiveAt
AsOfTime
CatalogTruthState
Items[]
ReasonCode?
```

`CatalogTruthState` SHALL preserve at least:

```text
CURRENT
STALE
UNKNOWN
UNAVAILABLE
```

Only `CURRENT` may be presented by Web as authoritative current availability. A stale/unknown/unavailable catalog may be displayed as last-known/status context, but SHALL NOT be represented as confirmed-current product truth.

Each `Item` shall contain the canonical equivalent of:

```text
SubjectId                    // stable Trading-owned identity
SubjectKind                  // SCHOOL | STRATEGY
DisplayLabelOrReference      // authoritative label or localization/display reference
DescriptionOrCategoryRef?    // optional presentation reference
ItemVersion
AvailabilityState
EffectiveAt
MarketScopes[]?              // discovery-level scope only
AssetClasses[]?              // discovery-level scope only
AssetSpecificApplicabilityCheckRequired
EntitlementRequirementReference?
ReplacementSubjectId?
ReasonCode?
```

`AvailabilityState` SHALL distinguish at least:

```text
AVAILABLE
TEMPORARILY_UNAVAILABLE
DEPRECATED
RETIRED
REPLACED
UNKNOWN
```

Web SHALL offer an item as currently selectable only when all applicable current projection semantics say it is discoverable and `AVAILABLE`.

---

## 5. Applicability and Entitlement Semantics

Catalog discovery is intentionally broader than asset-specific applicability.

A catalog item may state its broad market/asset discovery scope, but exact applicability to a specific instrument/chart context remains owned by Trading and is resolved through the FCR-0126 overlay request/applicability flow.

```text
CATALOG_PRESENT
!= APPLICABLE_TO_CURRENT_INSTRUMENT
!= OVERLAY_APPLICABLE
!= STRATEGY_ACTIVATED
!= TRADE_AUTHORIZED
```

Where entitlement applies, the catalog may carry a governed entitlement requirement/reference. Trading may use authoritative entitlement truth available through governed boundaries when deciding discoverability. Shared Web does not mint or reinterpret entitlement authority.

If entitlement truth is unknown where it is required for availability, the item SHALL NOT be upgraded to `AVAILABLE` merely for presentation convenience.

---

## 6. Dynamic Catalog Updates

Application-side semantic update identity:

```text
FSATS.WebStrategyCatalogUpdate.v1
```

Update types:

```text
ADD
UPDATE
AVAILABILITY_CHANGE
DEPRECATE
RETIRE
REPLACE
REMOVE
STATUS
```

Every update SHALL bind:

```text
CatalogVersion
PreviousCatalogVersion?
SubjectId?                   // required for item-specific changes
SubjectKind?
UpdateType
EffectiveAt
AsOfTime
CatalogTruthState
UpdatedItem?                 // when applicable
ReplacementSubjectId?
ReasonCode?
```

A future School/Strategy addition becomes discoverable through `ADD` or a newer full catalog projection. Web shall not require a hard-coded business-list release to discover it.

A `REMOVE` or `RETIRE` update does not erase historical audit truth, but Web SHALL stop presenting the subject as currently available.

A `REPLACE` update preserves the distinction between old and replacement identities. Web SHALL NOT silently rewrite historical selection identity.

---

## 7. Refresh, Ordering and Staleness

Catalog ordering/version semantics follow the accepted P1-K governed communication rules:

```text
LATE_OLDER_CATALOG != CURRENT_CATALOG
DUPLICATE_UPDATE != NEW_STATE
STALE != CURRENT
UNKNOWN != AVAILABLE
```

Web shall reject or ignore an older catalog/update as current when a newer accepted `CatalogVersion` is already known.

If continuity is uncertain, Web requests a fresh full catalog projection rather than merging ambiguous deltas into authoritative current state.

---

## 8. Relationship to FCR-0126

FCR-0128 owns **discovery of what may be offered**.

FCR-0126 owns **asset-specific applicability and render projection after selection**.

Canonical sequence:

```text
FCR-0128 CATALOG DISCOVERY
-> WEB PRESENTS CURRENT AVAILABLE OPTIONS
-> USER SELECTS SCHOOL / STRATEGY
-> FCR-0126 OVERLAY REQUEST FOR EXACT CHART CONTEXT
-> TRADING CONFIRMS APPLICABILITY
-> TRADING RETURNS RENDER PROJECTION / OUTCOME
```

Mandatory boundary:

```text
CATALOG_DISCOVERED != OVERLAY_APPLICABLE
USER_SELECTED != STRATEGY_ACTIVATED
OVERLAY_RENDERED != TRADE_AUTHORIZED
```

---

## 9. Failure / Truth Rules

```text
REQUEST_RECEIVED != CATALOG_CURRENT
CATALOG_STALE != AVAILABLE_CURRENT
CATALOG_UNKNOWN != EMPTY_CATALOG
NOT_IN_CURRENT_CATALOG != AVAILABLE_BY_WEB_MEMORY
TEMPORARILY_UNAVAILABLE != RETIRED
DEPRECATED != REMOVED
REPLACED != SAME_IDENTITY
```

If current catalog truth cannot be established, Trading returns the explicit degraded state and Web must preserve that uncertainty.

---

## 10. Implementation and Lifecycle State

This record is an Application semantic/interface clarification for cross-workstream FCR disposition and Web planning/binding.

```text
APPLICATION CATALOG SEMANTICS = DEFINED
OWNING TRADING COMPONENT = T-LSA-06 CENTRAL STRATEGY REGISTRY / CONTROLLER
CROSS-APP FAMILY = EXISTING P1K-019 INFORMATIONAL_QUERY_RESPONSE
NEW CROSS-APP CONTRACT FAMILY = NOT REQUIRED
SHARED WEB CONSUMPTION / IMPLEMENTATION = PENDING WEB WORKSTREAM
RUNTIME ROUTE IMPLEMENTATION = NOT AUTHORIZED BY THIS RECORD
STRATEGY ACTIVATION AUTHORITY = NOT CREATED
TRADING / EXECUTION AUTHORITY = NOT CREATED
PART 3 = NOT AUTHORIZED / NOT STARTED
```

Any later executable schema/route implementation remains subject to the then-current Part/implementation authority, Manifest declaration, Foundation transport capability, runtime binding, executable verification and fresh review requirements.
