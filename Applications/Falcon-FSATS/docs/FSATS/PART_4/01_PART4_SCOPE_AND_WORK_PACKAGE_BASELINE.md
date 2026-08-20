# FSATS Part 4 — Scope and Work-Package Baseline

**Status:** `OWNER_DELEGATED_SCOPE_DEFINITION / ACTIVE_IMPLEMENTATION_BASELINE`  
**Branch:** `application-development`  
**Owner Authority:** Project Owner direction dated 2026-08-15: `عرّف Part 4 أنت واعتمده كنطاق عمل وكمّل كامل.`  
**Part 3:** `OWNER_ACCEPTED_AND_CLOSED`  
**Runtime Authority:** `NOT_GRANTED`  
**External Provider/Broker Connectivity:** `NOT_GRANTED`  
**Paper/Shadow/Tiny-Live/Live/Deployment:** `NOT_GRANTED`

## 1. Part 4 Mission

Part 4 is defined as:

> **Application-Owned Version Evolution, Migration, Rollback, Replacement, Removal, and Stale-Authority Fencing.**

Part 2 established correct in-process business semantics. Part 3 established durable restart/reconstruction and bounded retention semantics. Part 4 closes the next Application-owned lifecycle gap required by APP-001 and CON-023: changing, rolling back, replacing, or removing an Application version must not resurrect stale authority, lose safety truth, erase evidence, silently transfer business ownership, or require Foundation redesign.

Part 4 is deliberately non-runtime. It implements deterministic Application-owned lifecycle-transition semantics and executable proof fixtures only. Foundation remains owner of actual Application installation/admission/activation/lifecycle enforcement and no production deployment or route activation is granted.

## 2. Authority and Reconciliation Basis

This scope is derived from current controlling sources:

- Falcon Vision protection, continuity, future-choice and integrity duties;
- Falcon Constitution authority, traceability, resilience, evidence and fail-safe duties;
- APP-001 requirements for update compatibility, migration, rollback/corrective action, replacement, removal, retained evidence and independent Application lifecycle;
- CON-023 required lifecycle, persistence, rollback, replacement/removal and immutable reconstructable declarations;
- ADR-I012 Plug-and-Play replaceability without Foundation redesign and no hidden cross-Application coupling;
- ADR-I015 independent Application lifecycle and Application-owned business recovery;
- accepted Part 0 through Part 3 semantics;
- P1-L V-04 and V-17 lifecycle/removal proof obligations and Application-removal fault suite;
- current five-Application topology and broker-account identity model;
- current live FCR holds.

The historical `FSATS_COMPLETE_BLUEPRINT` is reference input only. Its obsolete four-Application/FSARM topology is not imported. APP-RSC remains the fifth independent Application.

## 3. Prime Invariants

```text
VERSION_CHANGE != AUTHORITY_EXPANSION
PACKAGE_PRESENT != ADMITTED
UPDATE_INSTALLED != ACTIVATED
MIGRATION_COMPLETED != TRUST_RESTORED
ROLLBACK_TARGET_EXISTS != ROLLBACK_SAFE
ROLLBACK != STATE_AMNESIA
REMOVAL != EVIDENCE_ERASURE
REMOVAL != AUTHORITY_TRANSFER
REPLACEMENT != AUTOMATIC_IDENTITY_CONTINUITY
OLD_VERSION_LEASE != CURRENT_AUTHORITY
OLD_VERSION_PERMIT != CURRENT_AUTHORITY
OLD_VERSION_EPOCH != CURRENT_AUTHORITY
OLD_VERSION_IDEMPOTENCY_RESULT != UNCONDITIONAL_CURRENT_RESULT
UNKNOWN_SCHEMA != MIGRATE_AND_CONTINUE
UNKNOWN_MIGRATION != ACTIVATE
FAILED_MIGRATION != EMPTY_SAFE_STATE
REMOVED_APPLICATION != SILENTLY_RECREATED_APPLICATION
```

A lifecycle transition may reduce authority to preserve safety. It may not increase authority merely because new bytes, a new version, a rollback target, or a replacement package exists.

## 4. Work Packages

### P4-A — Versioned Lifecycle Transition Contract

Define deterministic Application-owned transition identities for current version, candidate version, package identity, transition ID, transition kind, source/target schema, trust epoch, captured evidence, and exact state.

Allowed transition kinds are explicitly distinguished: `Update`, `Rollback`, `Replacement`, `Removal`.

Unknown transition kind, missing version/package identity, self-contradictory source/target, duplicate transition identity with different semantics, or stale trust epoch fails closed.

### P4-B — Compatibility and Migration Planning

Define compatibility/migration decisions that separate:

```text
COMPATIBLE_AS_IS
MIGRATION_REQUIRED
INCOMPATIBLE
UNKNOWN
```

A migration plan binds exact source version/schema, target version/schema, required retained-state classes, evidence identity, and safety disposition. `UNKNOWN` and `INCOMPATIBLE` cannot become activation-ready.

### P4-C — Update / Rollback Eligibility State Machine

Implement lifecycle candidate states including at least:

```text
Proposed
Validated
MigrationRequired
MigrationValidated
ReadyForExternalLifecycleReview
Blocked
RollbackEligible
RollbackBlocked
RemovalReady
```

No state grants Foundation admission or activation. Rollback eligibility requires explicit compatibility with current durable safety state and no resurrection of revoked/cancelled/contained authority.

### P4-D — Trading Lifecycle Safety Migration

Trading lifecycle transition proof must preserve or explicitly reconcile:

- exact BrokerId + BrokerAccountId (+ environment where material);
- capital reservations and exposure ownership;
- queued/leased/dispatch-started execution state;
- active containment and cancellation tombstones;
- unresolved broker reconciliation obligations;
- idempotency/reserved identities;
- current trust/causation epochs;
- retained risk/protection evidence.

Removal or replacement cannot orphan open exposure or transfer Trading business authority to another Application.

### P4-E — FSAPMA Lifecycle Safety Migration

FSAPMA transition proof preserves provider/provider-account/API/service-role/environment identities, quota/entitlement metadata, delivery ambiguity, stream continuity/gap truth, idempotency tombstones, provenance and current credential references without storing secret bytes.

Replacement/restart/update never upgrades stale/gap/unknown provider truth to current truth.

### P4-F — Trading Guardian Lifecycle Safety Migration

Guardian transition proof preserves exact protection targets, incidents, command/correlation/idempotency identities, unresolved protection outcomes, current-protection truth verification requirements and containment/restriction evidence.

Historical `Applied` or `Accepted` protection state cannot become proof of current protection merely through update/rollback/replacement.

### P4-G — APP-RSC Lifecycle Safety Migration

APP-RSC transition proof preserves coordinator epoch/fencing, pending resource outcomes and exact Foundation-envelope references while preserving:

```text
APP_RSC != FOUNDATION_RESOURCE_GOVERNANCE
OLD_COORDINATION_EPOCH != CURRENT_AUTHORITY
MIGRATED_REFERENCE != FOUNDATION_GRANT
```

No version transition may mint, widen or reinterpret Foundation resource authority.

### P4-H — FSTSimA Lifecycle Safety Migration

FSTSimA transition proof preserves simulation/replay identity, committed evidence, checkpoint classification and non-Live boundaries. Interrupted/uncommitted runs remain incomplete. Migration or rollback cannot convert partial/synthetic/replay evidence into qualification or operational truth.

### P4-I — Removal / Replacement Reconciliation Package

Each of the five Applications independently produces an Application-owned reconciliation result covering its own business state before external lifecycle enforcement can safely proceed.

The package must classify:

- unresolved obligations;
- retained evidence;
- stale routes/leases/permits/epochs to fence;
- persisted state disposition;
- dependency impacts;
- removal blockers;
- replacement identity/compatibility result;
- whether safe removal/replacement is currently possible.

No sibling Application inherits removed Application business authority by default.

### P4-J — Integrated Adversarial Verification and Closure Evidence

Executable verification covers at least:

- stale old-version lease/permit/epoch reuse;
- rollback after containment;
- rollback after cancellation tombstone;
- rollback with unresolved broker outcome;
- partial migration/truncation/corruption;
- incompatible/unknown schema;
- replacement identity collision;
- removal while open Trading obligations exist;
- FSAPMA stream gap across version change;
- Guardian ambiguous protection across version change;
- APP-RSC stale coordinator epoch across version change;
- FSTSimA partial evidence migration;
- evidence erasure attempt;
- sibling authority inheritance attempt;
- runtime/egress authority escalation attempt.

## 5. Implementation Shape

Part 4 uses Application-local lifecycle transition models inside each independent Application. It SHALL NOT create a shared mutable FSATS runtime lifecycle owner.

Common proof semantics may be repeated locally where required for replaceability. Test-only utilities may coordinate verification but do not own runtime state or authority.

Actual Foundation lifecycle calls remain disabled/unbound. Part 4 produces Application-owned readiness/reconciliation results for a future governed Foundation lifecycle boundary; it does not implement Foundation Lifecycle internals.

## 6. Explicit Exclusions

Part 4 SHALL NOT:

- activate any Application version in Falcon runtime;
- perform real install/admission/activation/removal through Foundation;
- enable provider or broker egress;
- enable Paper, Shadow, Tiny-Live, Live or deployment;
- implement or copy Foundation Lifecycle/Persistence/Resource/Security internals;
- implement FSA internals or MSA-to-FSA production transport;
- modify `applications/shared/web/**`;
- grant Part 5 authority.

## 7. Part 4 Exit Criteria

Part 4 becomes technically eligible for Owner closure only when all are true:

1. P4-A through P4-I are implemented under authorized Application ownership.
2. each of the five Applications has deterministic local lifecycle-transition/reconciliation semantics.
3. stale old-version leases/permits/epochs cannot regain current authority.
4. rollback cannot resurrect containment-cancelled or reconciliation-owned work.
5. removal cannot erase required evidence or silently transfer business authority.
6. replacement cannot bypass identity/compatibility validation.
7. Trading open obligations cannot become ownerless through lifecycle change.
8. provider/protection/resource/simulation truth remains correctly classified across version change.
9. external/Foundation lifecycle adapters remain fail-closed/unbound where authority is absent.
10. Release build passes.
11. direct Part 4 adversarial behavior verification passes.
12. governed Application verifier suite passes twice from the same exact source.
13. final HEAD is exact and working tree clean.
14. fresh post-executable Architecture/Consistency review passes.
15. fresh post-executable broad Red-Team passes with zero open Critical/High/Medium findings for authorized Part 4 scope.
16. Project Owner explicitly accepts and closes Part 4.

## 8. Current State

```text
PART 3 = OWNER_ACCEPTED_AND_CLOSED
PART 4 SCOPE = DEFINED
PART 4 SCOPE AUTHORITY = OWNER-DELEGATED AND ACTIVE
PART 4 IMPLEMENTATION = AUTHORIZED WITHIN THIS NON-RUNTIME SCOPE
PART 5 = NOT_AUTHORIZED
RUNTIME = NOT_AUTHORIZED
PROVIDER / BROKER CONNECTIVITY = NOT_AUTHORIZED
PAPER / SHADOW / TINY-LIVE / LIVE / DEPLOYMENT = NOT_AUTHORIZED
```
