# Stage 6 WP-07 — Planning v0.1

Status: PROPOSED / NOT OWNER-ACCEPTED / NOT IMPLEMENTATION AUTHORITY
Date: 2026-08-10

## 1. Purpose

Plan Stage 6 WP-07 `Reclamation, Redistribution, Rebalance and Restoration` without changing any accepted WP-01 through WP-06 closure and without leaking WP-08 behavior.

WP-07 exists to provide a governed resource-mutation layer with two explicitly separated execution lanes:

1. delegated effective-distribution mutation inside a valid coordination envelope;
2. Foundation-authoritative allocation mutation when canonical Foundation resource truth/grants/ceilings must change.

## 2. Core invariants

- `EFFECTIVE_DISTRIBUTION != FOUNDATION_AUTHORITATIVE_ALLOCATION`
- `ELIGIBILITY != MUTATION_AUTHORITY`
- `WP06_DECISION != WP07_APPLIED_MUTATION`
- `INTERNAL_COORDINATION_MUTATION != FOUNDATION_GRANT_MUTATION`
- `INTERNAL_REDISTRIBUTION_FIRST`
- `FOUNDATION_ADDITIONAL_REQUEST_SECOND`
- `PRESERVE_PROTECTION_FLOORS = TRUE`
- `PRESERVE_RECOVERY_RESERVES = TRUE`
- `PRESERVE_CONSTITUENT_ATTRIBUTION = TRUE`
- `NO_OPAQUE_AGGREGATE_POOL = TRUE`
- `NO_APPLICATION_BUSINESS_SEMANTICS_IN_FOUNDATION = TRUE`

## 3. Lane A — Delegated coordination-envelope mutation

### 3.1 Purpose

Permit an exact delegated aggregate coordinator to perform bounded effective reclamation/redistribution/rebalance/restoration across an exact constituent Application set without requiring a Foundation round-trip for every move.

### 3.2 Coordination envelope

The envelope shall be Foundation-authorized and shall bind at minimum:

- exact envelope identity;
- exact Foundation authorization/evidence identity;
- exact coordination scope;
- exact coordinator instance and role;
- exact constituent Application identities;
- exact resource class;
- exact predecessor Foundation allocation snapshot identity;
- exact envelope generation;
- bounded effective lifetime;
- fencing generation/token;
- per-Application protected effective minimums;
- per-Application maximum effective-use bounds;
- total movable capacity permitted inside the envelope;
- reclaimability constraints;
- correlation/causation/evidence requirements.

The envelope shall fail closed if stale, expired, superseded, split-brain, cross-epoch, predecessor-mismatched or constituent-mismatched.

### 3.3 Effective distribution ledger

WP-07 shall maintain a separately identified effective-distribution state derived from, but not replacing, the accepted Foundation allocation snapshot.

For each exact constituent Application/resource-class binding it shall retain at minimum:

- authoritative grant/allocation/quota/ceiling predecessor identity;
- effective assigned quantity;
- protected effective minimum;
- reclaimable effective quantity;
- mutation sequence;
- last mutation identity;
- evidence references.

The sum of effective assigned quantities shall remain inside the envelope and shall never exceed the Foundation-authoritative capacity represented by the valid predecessor/envelope.

### 3.4 Allowed internal mutation types

- `EffectiveReclaim`
- `EffectiveRedistribute`
- `EffectiveRebalance`
- `EffectiveRestore`

These names describe internal effective-distribution mutations only and SHALL NOT imply Foundation grant/ceiling mutation.

### 3.5 Internal reclaim rule

A source Application may provide effective capacity for redistribution only when:

- the exact allocation/grant is still valid;
- the resource is reclaimable under accepted WP-05 evidence/policy;
- the requested reclaim does not violate the Application's protected effective minimum;
- the envelope permits the source/target/resource class and quantity;
- the coordinator fence and authority remain current;
- the source effective assignment after mutation remains non-negative and valid.

Pressure/preemption eligibility alone does not authorize the mutation.

### 3.6 Internal redistribution rule

An effective redistribution shall be represented as one atomic governed mutation set containing exact source(s), exact target(s), exact quantities and exact before/after effective-distribution identities.

No capacity may appear from nowhere, disappear without evidence, or be double-counted.

### 3.7 Rebalance rule

Rebalance is a deterministic multi-binding effective redistribution performed under one valid envelope and one exact predecessor effective-distribution state.

A rebalance must be all-or-nothing for its declared mutation set. Partial commit is rejected unless an explicitly governed partial-application model is separately defined and accepted; v0.1 does not define one.

### 3.8 Effective restoration

Restoration of a previously reduced effective assignment is not automatic.

It requires:

- valid current envelope/authority/fence;
- exact restoration target state/evidence;
- current predecessor state matching;
- sufficient currently available effective capacity;
- no protected-floor/reserve violation;
- no stale/cross-epoch restoration evidence.

Restoration must not blindly recreate an old state when current authoritative truth has changed.

## 4. Lane B — Foundation-authoritative allocation mutation

### 4.1 Purpose

Handle resource actions that actually change Foundation authoritative Application allocation truth, including governed `Reduce`, `Revoke`, Foundation-authoritative `Rebalance` and `Restore` behavior when separately authorized.

### 4.2 Authority rule

No Foundation-authoritative mutation may be inferred from:

- WP-05 pressure state;
- preemption eligibility;
- Application priority;
- technical criticality;
- FSARM coordination preference;
- an internal effective-distribution mutation;
- a WP-06 additional-resource decision unless the exact decision/authority explicitly creates the required mutation authority.

Every Foundation-authoritative mutation shall bind exact authority/decision evidence, exact predecessor allocation truth, exact resource class/Application set, mutation kind, quantity, lifetime and deterministic identity.

### 4.3 Canonical mutation outcomes

WP-07 planning recognizes these Foundation-authoritative mutation families:

- `Reduce`
- `Revoke`
- `Rebalance`
- `Restore`

Whether a specific family is executable in the implementation package depends on the exact accepted authority model and predecessor primitives; implementation shall not invent authority.

## 5. Atomicity, replay and fencing

Every mutation lane shall provide:

- monotonically advancing mutation sequence per exact governed scope;
- deterministic mutation identity;
- idempotent replay behavior;
- duplicate mutation rejection;
- stale predecessor rejection;
- cross-epoch rejection;
- expired/superseded authority rejection;
- stale fencing rejection;
- split-brain coordinator rejection;
- before/after state identity binding;
- correlation and causation preservation;
- reconstructable evidence.

## 6. Failure and recovery semantics

A failed mutation shall leave the last accepted state authoritative for that lane.

Coordinator failure shall not transfer authority to another instance without valid successor authority/fencing.

An interrupted atomic mutation shall not expose a half-applied distribution as accepted truth.

Recovery after restart shall reconstruct the last accepted effective-distribution/mutation state from governed evidence and exact predecessor identities. WP-07 does not create Stage 9 platform recovery authority.

## 7. Relationship to WP-06

WP-06 request/decision truth remains read-only predecessor evidence.

A `Grant`, `PartialGrant` or `Cap` decision can be consumed only through an explicit WP-07 mutation rule that proves the decision is current, applicable and authorized for the exact mutation performed.

`Deny` and `Defer` do not authorize positive resource mutation.

WP-07 shall not modify WP-06 request/decision records.

## 8. Relationship to WP-08

WP-07 may produce accepted mutation/effective-distribution truth that WP-08 can later project safely.

WP-07 shall not implement:

- per-Application safe projection contracts;
- Application load-shedding commands/signals;
- Application-local degradation order;
- notification/display behavior.

## 9. Application neutrality

Production Foundation code shall use generic resource/Application/coordinator terminology only.

Guardian, FSTSimA, Trading, FSATS-specific business names may exist only in planning examples or verifier fixtures if necessary, never as production architecture binding.

## 10. Zero-Application validity

Foundation zero-Application operation remains valid.

A coordination envelope requires a non-empty exact constituent set, but the absence of an envelope or Applications is not a Foundation failure state.

## 11. Proposed implementation surface

Subject to later file-level reconciliation and Owner implementation authorization, WP-07 is expected to require a narrow Foundation-owned surface under resource governance, likely including:

- coordination-envelope definition/validation;
- effective-distribution snapshot/ledger;
- internal mutation command/record/processor;
- Foundation-authoritative mutation command/record/processor or adapter when exact authority exists;
- dedicated WP-07 verifier.

No Application code shall be modified by Foundation WP-07 implementation.

## 12. Verification families

The dedicated WP-07 verifier shall cover at minimum:

1. positive envelope creation;
2. exact coordinator identity/role binding;
3. exact constituent binding;
4. envelope predecessor allocation binding;
5. envelope generation supersession;
6. envelope expiry;
7. stale fence rejection;
8. split-brain rejection;
9. cross-epoch rejection;
10. protected minimum preservation;
11. Foundation floor/reserve preservation;
12. positive effective reclaim;
13. non-reclaimable source rejection;
14. eligibility-without-authority rejection;
15. positive redistribution;
16. conservation of effective quantity;
17. no double allocation;
18. target bound enforcement;
19. source bound enforcement;
20. atomic rebalance positive path;
21. atomic rebalance partial-failure rollback;
22. positive effective restoration;
23. stale restoration rejection;
24. restoration capacity-unavailable rejection;
25. deterministic effective state identity;
26. deterministic mutation identity;
27. mutation sequence monotonicity;
28. duplicate mutation rejection;
29. replay/idempotency behavior;
30. stale predecessor rejection;
31. correlation preservation;
32. causation preservation;
33. evidence epoch/time validation;
34. Foundation-authoritative Reduce requires explicit authority;
35. Foundation-authoritative Revoke requires explicit authority;
36. Foundation-authoritative Rebalance requires explicit authority;
37. Foundation-authoritative Restore requires explicit authority;
38. pressure does not mint mutation authority;
39. priority does not mint mutation authority;
40. criticality does not mint mutation authority;
41. internal move does not mutate Foundation grants/ceilings;
42. Foundation mutation does not become opaque aggregate state;
43. exact per-Application attribution after every mutation;
44. application-neutral production surface;
45. no FSATS/TARC/Guardian/FSTSimA production hard-binding;
46. no WP-08 load-shedding executor;
47. zero-Application validity;
48. accepted WP-01 through WP-06 predecessor truth remains read-only;
49. Architecture regression pass;
50. Security regression pass;
51. WP-01 through WP-06 verifier regressions pass;
52. WP-07 verifier run twice from same Release outputs.

## 13. Stop conditions

Implementation shall stop if:

- the coordination envelope cannot be defined without granting FSARM Foundation authority;
- an internal move would require mutation of Foundation authoritative grants/ceilings but lacks explicit Foundation mutation authority;
- protected floors/reserves cannot be proven preserved;
- exact constituent attribution/accounting cannot be reconstructed;
- atomic mutation cannot be guaranteed under the chosen implementation model;
- WP-08 semantics are required to make WP-07 function;
- any accepted WP-01 through WP-06 closure would need reinterpretation without an explicit closure-defect trace.

## 14. Planning state

`WP07_PLANNING_v0.1 = PROPOSED`

`WP07_OWNER_ACCEPTANCE = NOT_YET`

`WP07_IMPLEMENTATION_AUTHORITY = NOT_GRANTED`

`WP08_IMPLEMENTATION_AUTHORITY = NOT_GRANTED`
