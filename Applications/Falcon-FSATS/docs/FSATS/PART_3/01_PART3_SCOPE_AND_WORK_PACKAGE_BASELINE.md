# FSATS Part 3 — Scope and Work-Package Baseline

**Status:** `OWNER_DELEGATED_SCOPE_DEFINITION / ACTIVE_IMPLEMENTATION_BASELINE`  
**Branch:** `application-development`  
**Owner Authority:** Project Owner direction dated 2026-08-15: `عرّف Part 3 أنت واعتمده كنطاق عمل وكمّل كامل`  
**Part 2:** `OWNER_ACCEPTED_AND_CLOSED`  
**Runtime Authority:** `NOT_GRANTED`  
**External Provider/Broker Connectivity:** `NOT_GRANTED`  
**Paper/Shadow/Tiny-Live/Live/Deployment:** `NOT_GRANTED`

## 1. Part 3 Mission

Part 3 is defined as:

> **Application-Owned Operational Durability, Restart Reconstruction, Bounded Retention, and Fail-Closed Recovery Readiness.**

Part 2 established correct in-process business semantics for broker-account identity, execution containment, reconciliation, provider continuity, Guardian protection routing, simulation evidence, and APP-RSC fail-closed Foundation binding. Part 3 hardens the Application-owned state transitions so process restart, stale replay, partial persistence, corruption, lease expiry, retained tombstones, and reconstruction uncertainty cannot silently resurrect risk or manufacture trustworthy state.

Part 3 deliberately does **not** activate external routes or claim production persistence binding. It materializes and executable-validates the Application-owned persistence/reconstruction semantics and ports that later governed Foundation/runtime binding must consume.

## 2. Authority and Boundary

This scope is derived from:

- Falcon Vision protection/continuity/integrity duties;
- Falcon Constitution resilience, evidence, traceability, fail-safe and bounded-authority duties;
- APP-001 Application-owned internal business recovery and independent lifecycle requirements;
- CON-023 persistence/evidence/recovery/rollback declaration requirements;
- ADR-I012 Application-neutral Foundation integration and no hidden coupling;
- ADR-I015 Application-owned internal recovery and Foundation-owned lifecycle/resource/platform authority;
- accepted Part 0 / Part 1 recovery, evidence and verification design;
- Part 2 closure records and the explicitly preserved future restart/retention holds;
- current live FCR state.

No Foundation-owned capability is invented. No Shared-Web-owned implementation is modified.

## 3. Prime Invariants

```text
RESTART != RECOVERY
PROCESS_RECREATION != TRUST_RESTORATION
PERSISTED_BYTES != TRUSTED_STATE
MISSING_DURABLE_STATE != EMPTY_SAFE_STATE
CORRUPT_DURABLE_STATE != RESET_AND_CONTINUE
UNKNOWN_EXTERNAL_OUTCOME != SAFE_TO_RETRY
CONTAINMENT_BEFORE_RESTART != RELEASED_AFTER_RESTART
CANCELLED_TOMBSTONE != REUSABLE_IDENTITY
STALE_EPOCH != CURRENT_AUTHORITY
LEASE_BEFORE_RESTART != VALID_AFTER_RESTART
COMPACTION != HISTORY_ERASURE
RETENTION_PRESSURE != PERMISSION_TO_DROP_SAFETY_STATE
```

All risk-increasing action remains denied until required reconstructed truth is complete enough for the exact action.

## 4. Work Packages

### P3-A — Durable-State Contract and Integrity Envelope
Define versioned Application-owned durable snapshot/journal envelopes with exact owner, schema version, generation/epoch, captured-at time, payload digest, and fail-closed validation. Unknown schema, wrong owner, malformed identity, digest mismatch, duplicate identity, impossible transition, or temporal contradiction are rejected.

### P3-B — Trading Execution Queue / Containment Reconstruction
Materialize capture and reconstruction for account-scoped queued work, cancellation tombstones, containment state, generations, and unresolved dispatch ambiguity.

Mandatory restart rules:
- `Leased` at capture reconstructs as `Queued` only if no containment exists and no dispatch began;
- `DispatchStarted` always reconstructs as `ReconciliationRequired`;
- `CancelledByContainment` remains cancelled and non-resurrectable;
- active account/broker containment survives reconstruction;
- completed and reconciliation-required identities remain reserved against duplicate enqueue;
- pre-restart dispatch permit/lease objects cannot become valid post-restart authority.

### P3-C — Trading Broker-Reconciliation Durability and Startup Gate
Persist/reconstruct broker-account reconciliation obligations and unresolved submission truth. New risk remains denied for an affected broker account until complete exact-account reconciliation evidence exists. Human/screenshot/last-known evidence never upgrades to broker-confirmed truth.

### P3-D — FSAPMA Delivery and Stream-Continuity Reconstruction
Persist/reconstruct provider-route delivery ambiguity and stream continuity identity/state without creating provider egress. Reconnect/restart never changes `GapDetected`, `ReconciliationRequired`, `Stale`, or unknown delivery truth into `Current`/delivered truth without fresh governed evidence.

### P3-E — Trading Guardian Protection-State Reconstruction
Persist/reconstruct target-scoped protection command outcome/idempotency/reconciliation state. Pending/started external protection actions after restart become reconciliation-required, never silently successful. Exact target/correlation identity is preserved.

### P3-F — APP-RSC Coordination Epoch Reconstruction
Persist/reconstruct current Application-owned coordination epoch/fencing evidence and pending outcomes while preserving Foundation authority separation. Stale pre-restart epoch/decision material cannot become a current Foundation grant or redistribution authority.

### P3-G — FSTSimA Evidence Checkpoint and Reproducibility Reconstruction
Persist/reconstruct simulation run/evidence checkpoint metadata. Interrupted/uncommitted runs remain incomplete and cannot qualify strategy/model readiness. Completed evidence remains immutable by reconstruction.

### P3-H — Bounded Retention, Capacity and Safe Compaction
Define deterministic retention policy for operational safety state. Safety-critical unresolved state, active containment, reconciliation-required identities, current protection state, current coordination fencing, and evidence needed to prevent duplicate action are non-evictable. Eligible terminal history may be compacted only into integrity-bound summaries/tombstones that preserve identity and no-resurrection guarantees.

### P3-I — Fail-Closed Application Startup Barriers
Each Application independently determines startup readiness from its own reconstructed state. No FSATS container coordinator is created. `UNKNOWN`, corrupt, incomplete, stale, unsupported, or missing required reconstruction state produces a bounded degraded/blocked result rather than fabricated normal readiness.

### P3-J — Integrated Executable Verification and Closure Evidence
Add adversarial behavior/failure/security verification covering restart, corruption, truncation, stale epoch, identity collision, tombstone resurrection, lease/permit reuse, ambiguous dispatch, incomplete stream continuity, protection ambiguity, APP-RSC stale epoch, simulation partial evidence, retention pressure and deterministic reconstruction.

## 5. Implementation Shape

Part 3 uses Application-owned contracts and reconstruction logic. Where a later production persistence service/binding is required, the Application exposes a narrow persistence port and keeps the current runtime adapter disabled/unbound until separately governed Foundation/runtime authority exists.

Test fixtures may use deterministic in-memory or test-only serialized stores to simulate process recreation. Such fixtures are evidence for Application reconstruction semantics only and are not presented as production persistence bindings.

## 6. Explicit Exclusions

Part 3 SHALL NOT:

- enable broker execution egress;
- enable provider network/stream egress;
- materialize Foundation credential storage or secret bytes;
- implement Foundation Persistence internals;
- consume Foundation artifacts through an unauthorized local substitute;
- activate APP-RSC production Foundation binding;
- implement FSA internals or MSA->FSA runtime transport;
- modify `applications/shared/web/**`;
- grant runtime, Paper, Shadow, Tiny-Live, Live, deployment, or Part 4 authority.

## 7. Part 3 Exit Criteria

Part 3 implementation is technically eligible for Owner closure only when all are true:

1. P3-A through P3-I source scope is implemented under `applications/**` only.
2. current external/Foundation ports remain fail-closed where authority is absent.
3. restart reconstruction cannot silently increase business authority or risk.
4. unresolved external outcomes survive restart as unresolved/reconciliation-required.
5. containment and no-resurrection identity fencing survive reconstruction.
6. stale lease/permit/epoch material cannot regain authority after reconstruction.
7. retention pressure cannot evict required safety truth.
8. corrupt/unsupported/missing required durable state fails closed.
9. Release build passes.
10. direct Part 3 behavior verification passes.
11. governed Application verifier suite passes twice from the same exact source.
12. working tree remains clean.
13. fresh post-executable Architecture/Consistency review passes.
14. fresh post-executable broad Red-Team passes with zero open Critical/High/Medium findings for Part 3 authorized scope.
15. Project Owner explicitly accepts and closes Part 3.

## 8. Current State

```text
PART 3 SCOPE = DEFINED
PART 3 SCOPE AUTHORITY = OWNER-DELEGATED AND ACTIVE
PART 3 IMPLEMENTATION = AUTHORIZED
RUNTIME = NOT AUTHORIZED
PART 4 = NOT AUTHORIZED
```
