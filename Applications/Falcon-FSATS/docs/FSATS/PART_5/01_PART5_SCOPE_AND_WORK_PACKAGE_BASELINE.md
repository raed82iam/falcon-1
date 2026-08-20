# FSATS Part 5 — Scope and Work-Package Baseline

**Status:** `OWNER_AUTHORIZED / APPLICATION_DEFINED_CURRENT_SCOPE`  
**Branch:** `application-development`  
**Runtime authority:** `NOT_GRANTED`

## Mission

Part 5 establishes **Application-Owned Operational Health, Readiness, Degradation, and Evidence Truth** for all five independent FSATS Applications.

The purpose is to make each Application able to state, in a bounded and reconstructable way, what it currently knows about its own business-operational fitness without converting health reporting into runtime authority, Foundation lifecycle authority, cross-Application ownership, or customer/user identity ownership.

## Why Part 5 Exists

Current higher authority requires every Falcon Application to be independently observable and to declare health reporting and failure-containment interfaces. Parts 2 through 4 already provide important operational truth, restart reconstruction, durability, reconciliation, update/rollback/replacement/removal safety, and stale-authority fencing.

Part 5 closes the next Application-owned gap by defining and implementing the local health/readiness projection that consumes those truths without weakening them.

## Prime Invariants

```text
HEALTHY != AUTHORIZED
READY != ACTIVE
READY != ADMITTED
READY != FOUNDATION_RELEASED
DEGRADED != FAILED
DEGRADED != PERMISSION_TO_IGNORE_SAFETY
PARTIAL != COMPLETE
LAST_KNOWN != CURRENT
STALE != CURRENT
NO_SIGNAL != HEALTHY
NO_ERROR_OBSERVED != PROVEN_HEALTHY
RECOVERY_QUALIFIED != RECOVERY_RELEASED
LOCAL_APPLICATION_HEALTH != FOUNDATION_HEALTH
APPLICATION_HEALTH_PROJECTION != FOUNDATION_LIFECYCLE_DECISION
CROSS_APPLICATION_HEALTH_PROJECTION != CROSS_APPLICATION_OWNERSHIP
ALL_GREEN != OWNER_APPROVAL
```

Unknown, stale, incomplete, contradicted, or integrity-unverified health evidence SHALL reduce readiness or fail closed according to consequence.

## Canonical Applications

```text
FSATS-TRADING
FSATS-FSAPMA
FSATS-TRADING-GUARDIAN
FSATS-FSTSIMA
APP-RSC
```

FSATS itself remains a non-owning/non-runtime system boundary and does not become a shared mutable health authority.

## Work Packages

### P5-A — Common Health / Readiness Semantics

Define common semantic dimensions that each Application must express locally without introducing a shared runtime owner:

- observation identity;
- Application identity;
- observed-at and valid-until semantics;
- evidence identity;
- evidence integrity state;
- local operating condition;
- local readiness condition;
- degradation state;
- unresolved obligation state;
- containment/restriction state where applicable;
- recovery/reconciliation dependency state;
- explicit runtime-authority refusal.

### P5-B — Trading Health and Readiness

Trading health remains broker-account centric and evaluates at minimum:

- exact `BrokerId + BrokerAccountId + Environment`;
- trading truth freshness;
- open exposure and capital-reservation obligations;
- queued/leased/dispatch-started execution work;
- unresolved broker reconciliation;
- containment and cancellation/no-resurrection fences;
- stale execution authority;
- restart reconstruction state;
- lifecycle-transition blocking state;
- evidence integrity.

Trading SHALL NOT introduce CustomerId/UserId ownership.

### P5-C — FSAPMA Health and Readiness

FSAPMA evaluates at minimum:

- provider/provider-account/service-role/environment identity;
- stream continuity and gap/staleness truth;
- delivery-outcome uncertainty;
- quota/entitlement observation state;
- credential-reference presence without secret-byte ownership;
- restart reconstruction state;
- lifecycle-transition blocking state;
- evidence integrity.

Provider data unavailability or staleness SHALL NOT be reported as healthy current data.

### P5-D — Trading Guardian Health and Readiness

Guardian evaluates at minimum:

- exact protected target identity;
- active containment/restriction state;
- unresolved protection command/outcome truth;
- requirement for current protection-truth verification;
- stale protection authority;
- incident/correlation/idempotency continuity;
- restart reconstruction state;
- lifecycle-transition blocking state;
- evidence integrity.

Historical `Applied` state alone SHALL NOT prove current protection health.

### P5-E — APP-RSC Health and Readiness

APP-RSC evaluates at minimum:

- current coordinator epoch;
- Foundation envelope/reference observation identity;
- unresolved/pending Foundation outcome state;
- stale coordinator state;
- resource-pressure/degradation truth;
- required safety-floor preservation;
- restart reconstruction state;
- lifecycle-transition blocking state;
- evidence integrity.

APP-RSC SHALL NOT mint or reinterpret Foundation resource authority.

### P5-F — FSTSimA Health and Readiness

FSTSimA evaluates at minimum:

- simulation/run identity;
- replay/synthetic/operational classification;
- run/checkpoint completeness;
- interruption state;
- pending validation;
- evidence digest/integrity;
- qualification truth;
- restart reconstruction state;
- lifecycle-transition blocking state.

Synthetic or partial evidence SHALL NOT become production qualification truth.

### P5-G — Bounded Degradation and Safe Continuity

Define Application-local degradation results so partial operation is explicit and bounded.

Examples:

```text
HEALTHY
DEGRADED_SAFE
RECONCILIATION_REQUIRED
CONTAINED
NOT_READY
UNKNOWN
```

A degraded state may preserve safe read-only or reconciliation work while denying risk-increasing/new-authority work. Part 5 does not itself start or authorize runtime work.

### P5-H — Freshness, Expiry, Contradiction, and Evidence Integrity

Health evidence SHALL carry explicit freshness/expiry and integrity semantics.

Part 5 SHALL reject or reduce readiness for:

- malformed identity;
- invalid enum values;
- observation from the future relative to supplied authoritative time;
- expired observation;
- missing evidence identity;
- failed evidence integrity;
- contradictory safety state;
- stale authority;
- unresolved high-consequence truth.

### P5-I — Cross-Application Projection Boundary

Materialize declaration-only Application health/readiness projection contracts suitable for governed cross-Application or Shared Web consumption later, without creating a shared mutable runtime health service.

Producer owns business meaning. Consumer receives a bounded projection only.

```text
PROJECTION_CONSUMPTION != INTERNAL_STATE_ACCESS
PROJECTION_PRESENT != CURRENT
PROJECTION_HEALTHY != FOUNDATION_HEALTHY
```

No Shared Web implementation is included in Part 5.

### P5-J — Integrated Adversarial Verification

Executable checks SHALL challenge at minimum:

- stale observation reported current;
- expired evidence reported healthy;
- missing evidence identity;
- malformed Application identity;
- healthy status with unresolved high-consequence obligation;
- degraded state incorrectly permitting risk increase;
- stale execution/protection/coordinator authority reported ready;
- provider gap/staleness laundering;
- Guardian historical-applied laundering;
- APP-RSC Foundation authority minting;
- FSTSimA synthetic/partial evidence qualification laundering;
- customer/user identity injection into Trading health;
- cross-Application ownership collapse;
- health result granting runtime authority;
- health result granting Part 6 or deployment authority.

## Required Implementation Shape

Part 5 SHALL preserve maintainability and replaceability:

- one local health/readiness evaluator per Application;
- no shared mutable FSATS health owner;
- no direct project reference to another Application's internals;
- small explicit records/enums with deterministic reason codes;
- no network/database/Foundation implementation dependency in the evaluator;
- contract declaration separate from runtime authority;
- deterministic pure evaluation suitable for adversarial tests;
- existing Part 2/3/4 semantics reused as inputs rather than duplicated as new authority.

## Explicit Exclusions

Part 5 does **not** authorize:

- Foundation health/lifecycle/security internals;
- Foundation source modification;
- Shared Web source modification;
- external provider/broker egress;
- credentials or secret-byte handling;
- production persistence binding;
- runtime activation;
- Paper, Shadow, Tiny-Live, Live;
- deployment;
- FSA internals or MSA-to-FSA runtime transport;
- Part 6 through Part 10.

## Exit Criteria

Part 5 becomes eligible for Owner final closure only after all of the following:

1. P5-A through P5-I implementation is complete under `applications/**`.
2. all five Applications have deterministic local health/readiness evaluators.
3. health/readiness never grants runtime or Foundation authority.
4. Trading remains broker-account centric with no UserId/CustomerId ownership.
5. unknown/stale/expired/integrity-failed evidence cannot become healthy current truth.
6. unresolved high-consequence obligations reduce readiness or fail closed.
7. bounded degradation cannot silently permit risk increase.
8. cross-Application projection declarations preserve producer ownership and no hidden internal coupling.
9. Release build passes.
10. direct Part 5 adversarial behavior verification passes.
11. governed Application verifier suite passes twice against the same exact source.
12. final validation HEAD is exact and validation tree is clean.
13. fresh post-executable Architecture/Consistency review passes.
14. fresh post-executable broad Red-Team passes with `0 Critical / 0 High / 0 Medium` open findings within the authorized scope.
15. the Project Owner explicitly accepts and closes Part 5.

## Current Authority State

```text
PART 4 = OWNER_ACCEPTED_AND_CLOSED
PART 5 = OWNER_AUTHORIZED_AND_ACTIVE
PART 5 SCOPE = DEFINED
PART 5 IMPLEMENTATION = AUTHORIZED
PART 6 THROUGH PART 10 = NOT_AUTHORIZED
RUNTIME = NOT_AUTHORIZED
PROVIDER / BROKER CONNECTIVITY = NOT_AUTHORIZED
PAPER / SHADOW / TINY-LIVE / LIVE / DEPLOYMENT = NOT_AUTHORIZED
```
