# FSATS Web Project Owner Feature Entitlement Contract v1

## Status

Application-owned semantic contract for FCR-0242.

This contract defines how Shared Falcon Web may consume Project Owner FSATS feature-entitlement truth. It does not define commercial subscription state, identity/session truth, network transport, action authorization, trading authority, broker authority, Foundation authority, Kill authority, runtime activation or deployment authority.

## Canonical identity

```text
EntitlementId = fsats.entitlement.project-owner.full-vip-or-greater
EntitlementVersion = 1.0.0
CatalogCompatibilityIdentity = compat:fsats-customer-feature-catalog:v1
```

## Governing rule

```text
PROJECT_OWNER_FSATS_FEATURE_ACCESS = ALL_ENABLED_CUSTOMER_FACING_STANDARD_AND_VIP_FEATURES
PROJECT_OWNER_FUTURE_VIP_FEATURE_ACCESS = AUTOMATIC_BY_CURRENT_CATALOG_REEVALUATION
PROJECT_OWNER_ACCESS != COMMERCIAL_VIP_SUBSCRIPTION
OWNER_ACCESS != TRIAL
OWNER_ACCESS != STANDARD_DOWNGRADE_TARGET
FEATURE_ACCESS != ACTION_AUTHORIZATION
FEATURE_ACCESS != TRADING_EXECUTION_AUTHORITY
FEATURE_ACCESS != BROKER_AUTHORITY
FEATURE_ACCESS != FOUNDATION_AUTHORITY
FEATURE_ACCESS != KILL_AUTHORITY
FEATURE_ACCESS != RUNTIME_ACTIVATION
FEATURE_ACCESS != DEPLOYMENT_AUTHORITY
```

The Project Owner is not represented as a commercial VIP subscriber. The Project Owner has an independent permanent entitlement rule that includes every enabled customer-facing feature whose commercial minimum tier is Standard or VIP. This is therefore equal to or greater than the complete customer-facing VIP feature set.

Future customer-facing VIP features are included by rule when they appear in a new governed feature-catalog version. A previously evaluated projection does not silently absorb catalog mutations. Web must re-evaluate against the current catalog identity/version/digest and freshness window.

Internal-only, disabled or malformed entries are not included. A customer-facing feature may require separate action, trading or broker authorization and still remain visible/available to the Project Owner. Feature access never supplies those separate authorities.

## Authoritative subject binding

Entitlement requires current authoritative Project Owner identity/session facts:

- exact subject identity;
- exact session identity;
- Owner identity-governance version;
- evidence reference;
- observation and expiry timestamps;
- authority source `AuthoritativeOwnerIdentitySession`;
- current Project Owner truth;
- not revoked;
- not superseded.

A role string, producer self-claim or stale/replayed Owner session is insufficient and fails closed.

FSATS does not mint Project Owner identity/session truth through this contract. The identity/session authority remains separately governed.

## Feature catalog binding

The entitlement request binds to an exact governed feature catalog:

- CatalogId;
- CatalogVersion;
- CatalogSha256;
- provenance reference;
- observation time;
- expiry time;
- feature identities and versions;
- audience;
- commercial minimum tier;
- enabled/customer-facing state;
- indicators that a feature requires separately governed action/trading/broker authorization.

Duplicate feature IDs, malformed catalog identity, invalid SHA-256, future-dated observation, expired catalog or incompatible catalog identity fail closed.

The decision expiry is the earlier of the authoritative Owner identity/session expiry and catalog expiry. A consumer cannot extend entitlement freshness beyond either source.

## Commercial subscription separation

For an accepted Project Owner entitlement:

```text
CommercialSubscriptionRequired = false
TrialApplies = false
SevenDayWarningApplies = false
StandardDowngradeApplies = false
UpgradePromptApplies = false
StandardFeatureLockApplies = false
```

Commercial subscription lifecycle, trial expiry and downgrade behavior must never be inferred for the Project Owner from this entitlement.

## Authority separation

An accepted feature-entitlement projection grants presentation/navigation/feature availability only. It always returns false for:

- action authorization;
- trading execution authority;
- broker authority;
- Foundation authority;
- Kill authority;
- runtime activation;
- deployment authority.

A feature that itself requires one or more of those separate authorities may still be part of the Owner's feature set. Attempting to mint the required authority through the entitlement request is rejected.

## Supersession and revocation

A prior entitlement decision must not be reused when any of the following changes materially:

- Project Owner subject or session identity;
- Owner identity-governance version;
- revocation/supersession state;
- entitlement ID/version;
- feature-catalog compatibility identity;
- catalog ID/version/digest;
- identity/session expiry;
- catalog expiry.

The consumer must fail closed and obtain/re-evaluate current facts.

## Web-facing source contract

C# source:

`applications/FSATS/src/Trading/Falcon.FSATS.Trading.Contracts/WebProjectOwnerFeatureEntitlementContracts.cs`

Primary evaluator:

`WebProjectOwnerFeatureEntitlementGovernance.Evaluate(...)`

## Transport ownership

No exact live transport for this entitlement projection is identified or authorized by FCR-0242.

```text
APPLICATION_SEMANTIC_CONTRACT = DEFINED
WEB_CONSUMING_CONTRACT = AVAILABLE
LIVE_ENTITLEMENT_TRANSPORT_BINDING = NOT_IDENTIFIED / SEPARATELY_GOVERNED
```

Shared Web may consume this semantic contract when an authoritative governed transport/projection source is available. It must not invent transport authority or infer entitlement merely from a local `PROJECT_OWNER` role.

## Fail-closed rules

Reject when:

- authoritative Project Owner identity/session evidence is missing, stale, revoked, superseded or self-claimed;
- subject kind is not ProjectOwner;
- contract/version or catalog compatibility mismatches;
- catalog provenance/identity/digest is malformed;
- catalog freshness has expired;
- commercial subscription/trial/downgrade semantics are applied to Project Owner;
- action/runtime/deployment/business authorities are requested through entitlement;
- feature catalog contains malformed or duplicate identities.

An unavailable or incompatible entitlement truth means entitlement-gated FSATS surfaces remain unavailable rather than being inferred.
