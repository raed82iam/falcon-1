# FCR-0242 Shared Web Consuming Binding Checkpoint

**Date:** 2026-08-18  
**Branch:** `web-development`  
**Application exact contract candidate:** `0650bd136b9ff730420efdd00d1fd9e9f60b37c9`  
**Status:** `WEB_EXACT_SEMANTIC_CONSUMING_BINDING_IMPLEMENTED / TARGETED_EXECUTABLE_PASS / LIVE_TRANSPORT_NOT_IDENTIFIED`

## Purpose

This checkpoint records the Shared Falcon Web consuming-side implementation for the Application-owned Project Owner permanent FSATS feature entitlement defined by FCR-0242.

It does not invent a live Application transport, commercial VIP subscription, Trading authority, broker authority, Foundation authority, Kill authority, runtime activation or deployment authority.

## Canonical Application contract consumed

```text
EntitlementId = fsats.entitlement.project-owner.full-vip-or-greater
EntitlementVersion = 1.0.0
CatalogCompatibilityIdentity = compat:fsats-customer-feature-catalog:v1
```

The authoritative Application contract remains:

`applications/FSATS/contracts/web/FSATS.WebProjectOwnerFeatureEntitlementContracts.v1.md`

and exact executable source:

`applications/FSATS/src/Trading/Falcon.FSATS.Trading.Contracts/WebProjectOwnerFeatureEntitlementContracts.cs`

## Web implementation

New exact semantic consumer:

`src/adapters/fsats-project-owner-feature-entitlement-v1.js`

The consumer requires:

- an authoritative current Web Project Owner session;
- exact Project Owner subject identity;
- exact session identity;
- exact Owner identity-governance version;
- accepted canonical entitlement id/version and reason;
- current evaluation/evidence expiry;
- valid catalog id/version/SHA-256;
- unique non-empty granted feature identities;
- `IncludesCurrentAndFutureVipCustomerFeatures = true`;
- every commercial-subscription/trial/downgrade flag = false;
- every action/trading/broker/Foundation/Kill/runtime/deployment authority flag = false.

Fail-closed output is returned on any mismatch.

## Web routing and Owner Home binding

`src/auth.js` now permits a Project Owner to enter customer-facing FSATS routes only when both are true:

1. the authoritative session has the Owner Web surface grant; and
2. the FCR-0242 entitlement consumer produced a verified Owner FSATS access model.

Therefore:

```text
PROJECT_OWNER_ROLE != FSATS_FEATURE_ACCESS
OWNER_SURFACE_GRANT != FSATS_FEATURE_ACCESS
FCR0242_ENTITLEMENT_ACCESS != ACTION_AUTHORIZATION
FCR0242_ENTITLEMENT_ACCESS != TRADING_EXECUTION_AUTHORITY
```

`src/features/owner-home/owner-home.js` uses the same canonical Web access gate. The FSATS destination remains disabled when entitlement truth is absent, stale, mismatched or unavailable.

## Identity-session composition

`src/adapters/foundation-stage16-identity-session-v1.js` preserves an `ownerIdentityGovernanceVersion` when the separately governed Web access binding supplies one.

This field is not made mandatory for every generic Owner session. It becomes mandatory only when consuming FCR-0242 entitlement truth, preserving separation between authoritative identity/session truth and Application-owned feature entitlement.

## Tests

Added:

`tests/fcr0242-owner-fsats-entitlement.test.mjs`

Coverage includes:

- canonical accepted Owner entitlement;
- Owner role without entitlement remains denied from FSATS routes;
- subject mismatch;
- session mismatch;
- Owner governance-version mismatch;
- stale/expired evidence;
- malformed catalog SHA-256;
- duplicate feature identities;
- missing future-VIP inclusion rule;
- commercial subscription/trial/downgrade leakage;
- action/trading/broker/Foundation/Kill/runtime/deployment authority leakage.

## Targeted executable verification

A standalone exact logic harness using the Web adapter and routing gate executed successfully:

```text
NODE_CHECK_AUTH = PASS
NODE_CHECK_FCR0242_ADAPTER = PASS
FCR0242_TARGETED_EXECUTABLE = PASS
```

The adversarial mutations above fail closed.

## Source Red Team

```text
CRITICAL = 0
HIGH = 0
MEDIUM = 0
PRODUCT_LOW = 0
```

One composition concern was identified and remediated before this checkpoint: requiring the Owner identity-governance version globally in Stage16 session adaptation would have coupled the generic identity contract to the Application entitlement contract. The final implementation preserves the field when available but requires it only at the FCR-0242 entitlement boundary.

## Transport boundary

The Application contract explicitly states:

```text
LIVE_ENTITLEMENT_TRANSPORT_BINDING = NOT_IDENTIFIED / SEPARATELY_GOVERNED
```

Shared Web therefore does not create or activate a route. The exact semantic consuming boundary is ready for an authoritative governed projection/transport source when separately provided.

```text
SEMANTIC_CONSUMER_READY != LIVE_TRANSPORT_AVAILABLE
FEATURE_ENTITLEMENT_VERIFIED != RUNTIME_ACTIVATED
ROUTE_NOT_IDENTIFIED != PERMISSION_TO_INVENT_ROUTE
```

## Current disposition

```text
APPLICATION_FCR0242_CONTRACT = COMPLETE_AND_EXECUTABLE_VERIFIED
WEB_FCR0242_SEMANTIC_CONSUMING_BINDING = IMPLEMENTED
WEB_FCR0242_TARGETED_EXECUTABLE = PASS
WEB_FCR0242_SOURCE_RED_TEAM = PASS
LIVE_ENTITLEMENT_TRANSPORT = NOT_IDENTIFIED / SEPARATELY_GOVERNED
FCR0242_CROSS_WORKSTREAM_SEMANTIC_SCOPE = READY_FOR_CLOSURE_REVIEW
```
