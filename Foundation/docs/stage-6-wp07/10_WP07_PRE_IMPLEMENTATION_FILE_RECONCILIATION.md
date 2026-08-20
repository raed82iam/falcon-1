# Stage 6 WP-07 — Pre-Implementation File-Level Reconciliation

Status: IMPLEMENTATION_INPUT / AUTHORIZED / PRE-CODE
Date: 2026-08-10
Planning Baseline: WP-07 v0.3 OWNER_ACCEPTED
Implementation Authority: GRANTED
WP-08 Authority: NOT GRANTED

## 1. Purpose

Identify the minimum production/verifier surface required to implement the exact Owner-accepted WP-07 boundary without reopening or reinterpreting accepted Stage 6 WP-01 through WP-06 closures.

## 2. Accepted predecessor surface to reuse

WP-07 SHALL consume existing accepted public/state surfaces rather than duplicate them:

- WP-01 canonical identifiers, quantities, evidence, epoch, lifetime, reclaimability and canonical `ResourceDecisionKind` including `Reduce`, `Revoke`, `Restore`;
- WP-02 authoritative Foundation resource truth, allocatable capacity, protection floors and recovery reserves;
- WP-03 `ApplicationResourceAllocation`, `ApplicationResourceAllocationSnapshot`, per-Application grant/allocation/quota/ceiling/accounting/isolation truth;
- WP-04 priority/technical-criticality truth as policy/evidence input only;
- WP-05 pressure, reclaimability/preemption-eligibility and enforcement-state truth as eligibility evidence only;
- WP-06 exact requester/coordinator authority, coordinator fencing, delegation supersession, request/decision truth and `INTERNAL_REDISTRIBUTION_FIRST` semantics.

## 3. No predecessor closure defect found

No evidence found that WP-01 through WP-06 failed their exact accepted scopes.

`WP01_WP06_CLOSURES_REOPENED = NO`
`CLOSURE_DEFECT = NONE_FOUND`

## 4. Missing WP-07 behavior

No accepted predecessor provides the whole WP-07 execution boundary. Missing behavior is:

1. exact Foundation-authorized coordination envelope;
2. effective-distribution state separate from WP-03 authoritative allocation truth;
3. borrowed effective-capacity provenance binding exact source Application/source grant and target Application;
4. delegated effective redistribution execution inside the envelope;
5. Foundation-authoritative `Reduce` / `Revoke` / `Restore` application under exact mutation authority;
6. rebalance as an atomic batch/transaction concept, not a new decision kind;
7. environment-neutral resource-effect adapter/result contract;
8. strict separation of mutation intent, applied-effect evidence and accepted post-mutation truth;
9. replay, expiry, authority supersession, fencing, split-brain and partial-effect controls;
10. dedicated executable verification.

## 5. Minimum production file surface

### New production file

`src/Foundation.State/ResourceMutationGovernance.cs`

This file shall own only WP-07 state/processing behavior in `Foundation.State.ResourceGovernance`.

Expected generic types include conceptually:

- coordination-envelope authority/binding;
- exact envelope Application/grant membership and protected-effective-minimum constraints;
- borrowed effective-capacity provenance segment;
- effective-distribution snapshot/state;
- effective redistribution movement/batch intent;
- Foundation allocation-mutation authority and mutation intent;
- environment-neutral effect batch/result evidence contract;
- WP-07 processor/executor with deterministic replay/supersession/fencing validation.

Names may be adjusted during coding if needed for accepted public-surface consistency, but semantics SHALL NOT change.

### Existing production files

No production mutation is planned for WP-01 through WP-06 files unless compilation proves an exact generic primitive is missing. Any such need is a STOP condition requiring reconciliation before change.

## 6. Contract placement decision

The accepted WP-01 canonical contract layer currently exposes generic primitives needed by WP-07: identifiers, quantities, evidence, lifetime, reclaimability and canonical mutation decision meanings. No separate canonical `Rebalance` decision primitive exists or is required.

Initial implementation therefore SHALL NOT modify `Foundation.Contracts`.

If implementation discovers that a genuinely reusable cross-layer canonical primitive is required rather than WP-07 state/process material, work SHALL stop and reconcile that requirement before modifying Contracts.

## 7. Dedicated verifier surface

Create:

- `verification/Falcon.Stage6.WP07.Verifier/Falcon.Stage6.WP07.Verifier.csproj`
- `verification/Falcon.Stage6.WP07.Verifier/Program.cs`

Update controlled solution only to include this verifier.

Verifier shall cover at minimum:

- accepted predecessor regression WP-01 through WP-06;
- zero-Application validity;
- envelope identity/scope/lifetime/evidence/generation/fence validation;
- exact constituent membership and grant provenance;
- no opaque pool;
- effective movement conservation;
- source protected-effective minimum;
- target authoritative ceiling bound;
- quota/ceiling headroom not treated as granted capacity;
- source grant ownership retained after borrowing;
- exact target attribution;
- no authoritative WP-03 grant/ceiling mutation from delegated lane;
- `ELIGIBILITY != MUTATION_AUTHORITY`;
- stale/replayed intent rejection;
- authority supersession rejection;
- stale fence/split-brain rejection;
- atomic batch/rebalance semantics;
- partial-effect failure cannot publish accepted truth;
- effect evidence bound to exact intent/batch;
- `Reduce` authority and applied truth;
- `Revoke` authority and applied truth;
- `Restore` current-state validation and applied truth;
- disallow `Grant` / `PartialGrant` / `Cap` / `Deny` / `Defer` as WP-07 Foundation mutation operations;
- `Rebalance` absent as canonical decision kind;
- floors/reserves preserved;
- deterministic identities;
- Application-neutral public surface;
- no FSARM/TARC/Trading public type names;
- no WP-08 load-shedding/projection behavior;
- environment-neutral effect contract;
- two-run verifier determinism.

## 8. Planned implementation boundaries

### Delegated effective-distribution lane

May change only WP-07 effective-distribution truth inside a valid envelope.

It SHALL NOT mutate:

- WP-03 grant identity;
- WP-03 quota;
- WP-03 ceiling;
- Foundation total-resource truth;
- protection floor;
- recovery reserve.

Every borrowed segment remains attributable as:

`SOURCE_APPLICATION + SOURCE_GRANT + RESOURCE_CLASS + QUANTITY -> TARGET_APPLICATION`

### Foundation-authoritative lane

May produce a successor authoritative allocation snapshot only after:

1. exact valid Foundation mutation authority;
2. allowed canonical operation `Reduce`, `Revoke` or `Restore`;
3. current predecessor truth validation;
4. protection-floor/recovery-reserve/allocatable-capacity validation;
5. successful environment-neutral applied-effect evidence bound to exact mutation intent;
6. no partial-effect ambiguity.

## 9. Rebalance semantics

`Rebalance` SHALL be represented as an atomic transaction/batch composed of one or more authorized movements/mutations.

It SHALL NOT be added to `ResourceDecisionKind` and SHALL NOT mint authority by being called a rebalance.

## 10. Environment neutrality

WP-07 shall define a generic logical effect contract and a deterministic test adapter for verification. This does not claim Windows/Linux/container/resource-controller qualification.

Environment realization remains later-stage work.

## 11. Stop conditions

Implementation SHALL stop if:

- Contracts must be changed without prior reconciliation;
- internal coordination requires changing Foundation authoritative grants/ceilings;
- a target would consume capacity not backed by exact granted source provenance;
- quota/ceiling headroom must be treated as granted capacity;
- an intended mutation could be published as accepted without applied-effect evidence;
- partial effect cannot be distinguished from success;
- WP-08 behavior becomes necessary;
- Application-specific business/degradation semantics leak into Foundation;
- an accepted predecessor must be reinterpreted rather than consumed.

## 12. Pre-code disposition

`FILE_LEVEL_RECONCILIATION = COMPLETE`
`PROPOSED_NEW_PRODUCTION_FILE_COUNT = 1`
`PLANNED_PREDECESSOR_PRODUCTION_MUTATION = NONE`
`PLANNED_CONTRACT_MUTATION = NONE`
`DEDICATED_WP07_VERIFIER = REQUIRED`
`PRE_IMPLEMENTATION_RED_TEAM = REQUIRED_BEFORE_CODE`
