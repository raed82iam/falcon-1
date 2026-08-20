# Stage 6 WP-07 — Planning v0.3 Final Candidate

Status: PROPOSED / READY_FOR_FINAL_RED_TEAM / NOT OWNER-ACCEPTED / NOT IMPLEMENTATION AUTHORITY
Date: 2026-08-10
Supersedes WP-07 planning proposals v0.1 and v0.2 prospectively.

## 1. Purpose

Plan Stage 6 WP-07 `Reclamation, Redistribution, Rebalance and Restoration` as the governed resource-mutation layer built on accepted WP-01 through WP-06 truth and authority.

WP-07 has two strictly separated execution lanes:

1. delegated effective-distribution mutation inside an exact Foundation-authorized coordination envelope;
2. Foundation-authoritative allocation mutation only under exact Foundation mutation authority.

Neither lane may borrow authority from the other.

## 2. Core invariants

- `EFFECTIVE_DISTRIBUTION != FOUNDATION_AUTHORITATIVE_ALLOCATION`
- `ELIGIBILITY != MUTATION_AUTHORITY`
- `WP06_DECISION != WP07_APPLIED_MUTATION`
- `INTERNAL_COORDINATION_MUTATION != FOUNDATION_GRANT_MUTATION`
- `MUTATION_INTENT != APPLIED_EFFECT_EVIDENCE != ACCEPTED_POST_MUTATION_TRUTH`
- `NATIVE_EFFECTIVE_CAPACITY != BORROWED_EFFECTIVE_CAPACITY`
- `BORROWED_CAPACITY_RETAINS_SOURCE_GRANT_PROVENANCE = TRUE`
- `INTERNAL_REDISTRIBUTION_FIRST`
- `FOUNDATION_ADDITIONAL_REQUEST_SECOND`
- `QUOTA_HEADROOM != GRANTED_CAPACITY`
- `CEILING_HEADROOM != GRANTED_CAPACITY`
- `PRESERVE_PROTECTION_FLOORS = TRUE`
- `PRESERVE_RECOVERY_RESERVES = TRUE`
- `PRESERVE_CONSTITUENT_ATTRIBUTION = TRUE`
- `NO_OPAQUE_AGGREGATE_POOL = TRUE`
- `ENVIRONMENT_NEUTRAL_RESOURCE_EFFECT_CONTRACT = TRUE`
- `NO_APPLICATION_BUSINESS_SEMANTICS_IN_FOUNDATION = TRUE`

## 3. Accepted predecessor use

WP-07 consumes accepted predecessors without modification:

- WP-01 canonical resource identities, quantities, evidence/lifetime, correlation/causation and deterministic identity;
- WP-02 Foundation resource truth, protection floors and recovery reserves;
- WP-03 exact Application allocation/quota/ceiling/grant/isolation truth;
- WP-04 Application priority and technical criticality governance;
- WP-05 pressure/reclaimability/preemption-eligibility/enforcement-state truth;
- WP-06 direct/coordinator request authority, delegation/fencing semantics and additional-resource request/decision truth.

WP-05 eligibility is evidence only. WP-06 decision truth is predecessor evidence only. Neither automatically applies a WP-07 mutation.

## 4. Lane A — Delegated coordination-envelope mutation

### 4.1 Purpose

Permit an exact delegated aggregate coordinator to perform bounded operational effective reclamation/redistribution/rebalance/restoration across an exact constituent Application set without a Foundation round-trip for every valid internal move.

This lane never alters Foundation authoritative grants, allocations, quotas or ceilings.

### 4.2 Foundation-authorized coordination envelope

Every envelope shall bind at minimum:

- exact envelope identity;
- exact Foundation authorization/evidence identity;
- exact coordination scope;
- exact coordinator instance and role;
- exact constituent Application identities;
- exact resource class;
- exact predecessor Foundation allocation snapshot identity;
- exact envelope generation;
- bounded effective lifetime;
- coordinator fencing generation/token;
- per-Application protected effective minimums;
- per-Application maximum effective-use bounds;
- exact total capacity admitted into the envelope;
- exact movable-capacity limit;
- reclaimability constraints;
- correlation/causation/evidence requirements.

Envelope validation fails closed for stale, expired, superseded, split-brain, cross-epoch, predecessor-mismatched or constituent-mismatched state.

### 4.3 Capacity accounting

The following values remain distinct:

- `AuthoritativeAllocation`
- `AuthoritativeQuota`
- `AuthoritativeCeiling`
- `ProtectedEffectiveMinimum`
- `NativeEffectiveAssignment`
- `BorrowedEffectiveAssignment`
- `EnvelopeMovableCapacity`

Quota or ceiling headroom alone is not granted capacity.

The envelope-admitted total must be explicitly Foundation-authorized and derived only from capacity already made available to the exact governed constituent set.

The post-mutation total effective assignment cannot exceed the envelope-admitted total, and no target can exceed its exact envelope target bound or authoritative ceiling.

### 4.4 Capacity provenance

Every cross-Application borrowed effective-capacity segment shall retain exact dual attribution:

- `SourceApplicationId`
- `SourceGrantId`
- `SourceResourceClassId`
- `TargetApplicationId`
- exact borrowed quantity
- envelope identity/generation
- mutation identity/sequence
- evidence identity

The Foundation authoritative allocation remains owned by the source Application/grant. The target receives only a bounded effective consumption entitlement against that exact provenance-bound capacity.

A borrowed segment is never anonymous aggregate-pool capacity.

The sum of all active borrowed segments sourced from one grant may not exceed the quantity that the valid envelope and current source state make movable from that grant.

Return/reclaim/restore operations must reference the same provenance-bound segment or a deterministic successor derived from it.

### 4.5 Effective-distribution truth

WP-07 shall maintain separately identified effective-distribution state. Each Application/resource binding records at minimum:

- exact Application/resource/grant predecessor identity;
- native effective assigned quantity;
- borrowed-in effective segments;
- borrowed-out effective segments;
- protected effective minimum;
- currently reclaimable effective quantity;
- current mutation sequence;
- last accepted mutation identity;
- evidence references.

The state must be exactly reconstructable to source grants and current effective consumers.

### 4.6 Internal operation names

The delegated lane recognizes these internal operation names only:

- `EffectiveReclaim`
- `EffectiveRedistribute`
- `EffectiveRebalance`
- `EffectiveRestore`

They are not Foundation resource decision kinds and create no Foundation grant authority.

### 4.7 Effective reclaim

Effective capacity may be released from a source only when:

- exact predecessor allocation/grant remains current;
- valid WP-05 reclaimability/eligibility evidence exists where required;
- separate valid envelope authority permits the move;
- source protected effective minimum remains satisfied;
- source native/borrowed provenance is exact;
- quantity/unit/resource class are valid;
- coordinator authority/fence remain current.

### 4.8 Effective redistribution

A redistribution binds exact provenance-aware source segments, target Applications, quantities, before-state identity and intended after-state identity.

Capacity conservation and provenance conservation are mandatory.

No capacity may be created, silently discarded, double-counted or moved outside envelope scope.

### 4.9 Effective rebalance

`Rebalance` is a transaction concept, not a new canonical decision kind.

An effective rebalance is one atomic batch of exact effective reclaim/redistribute/restore changes under one valid envelope and one exact predecessor effective-distribution state.

The declared batch is all-or-nothing.

### 4.10 Effective restoration

Historical pre-reclaim state is evidence only, never automatic restoration authority.

Restoration is recalculated against current:

- Foundation allocation truth;
- envelope identity/generation/bounds;
- current provenance segments;
- current effective distribution;
- current pressure/reclaimability evidence where applicable;
- authority/fencing;
- available capacity.

A stale historical target is rejected if current conditions cannot validly support it.

## 5. Resource-effect and commit contract

### 5.1 Three-state separation

Every operational mutation distinguishes:

1. `MutationIntent`
2. `AppliedEffectEvidence`
3. `AcceptedPostMutationTruth`

Mutation intent does not change accepted truth.

### 5.2 Environment-neutral effect port

WP-07 shall define only a generic Foundation resource-effect port/adapter contract.

The contract shall express generic mutation identity, exact before-state, intended effect, exact scope/resource/Application provenance, success/failure/partial result and evidence.

WP-07 production semantics shall not embed Windows, Linux, container, hypervisor or other environment-specific enforcement meaning.

A logical/test adapter may be used for Stage 6 verification to prove the generic transaction contract. Such evidence does not claim Stage 16 environment qualification or Stage 17 operational readiness.

### 5.3 Commit rule

Accepted post-mutation truth advances only when:

- exact effect evidence proves the declared mutation was applied; or
- an explicitly verified in-process atomic mechanism makes effect and truth commit inseparable.

Failed or partial effect cannot publish intended after-state as accepted success.

### 5.4 Partial-effect handling

For multi-binding mutation batches:

- partial application is not accepted final success;
- compensation/rollback shall restore the last accepted state or yield an explicit governed failure state without false success;
- effect and compensation evidence remain reconstructable;
- retry is bound to the same or explicitly superseding mutation identity and current predecessor state.

## 6. Lane B — Foundation-authoritative mutation

### 6.1 Purpose

Apply actions that actually change canonical Foundation Application allocation truth only under exact Foundation mutation authority.

### 6.2 Authority rule

Foundation-authoritative mutation authority cannot be inferred from pressure, reclaimability, priority, technical criticality, FSARM preference, internal redistribution or urgency.

A WP-06 record is evidence only unless the exact accepted authority model makes that record applicable to the exact mutation.

### 6.3 Canonical action use

WP-07 may consume exact separately authorized canonical actions such as:

- `Reduce`
- `Revoke`
- `Restore`

WP-07 does not invent `Rebalance` as a canonical resource decision kind.

A Foundation-authoritative rebalance is a governed atomic transaction composed from exact separately authorized canonical mutations against one exact predecessor allocation state.

### 6.4 Authoritative mutation record

Every Foundation-authoritative mutation binds:

- exact authority/decision identity;
- exact predecessor allocation snapshot identity;
- exact Application/resource/grant binding;
- exact canonical action and quantity;
- exact before/after authoritative state identity;
- epoch/lifetime;
- correlation/causation;
- exact applied-effect evidence;
- deterministic mutation identity.

Foundation-authoritative mutation must preserve WP-02 floors/reserves and WP-03 exact attribution/isolation.

## 7. Ordering, replay, fencing and reconstruction

Both lanes require:

- monotonically advancing sequence per exact governed scope;
- deterministic identities;
- duplicate rejection;
- replay-safe idempotency;
- stale predecessor rejection;
- cross-epoch rejection;
- expired/superseded authority rejection;
- stale fencing rejection;
- split-brain rejection;
- before/after identity binding;
- correlation/causation preservation;
- exact evidence provenance.

Coordinator failure never transfers authority automatically. A successor requires valid successor authority/fencing.

Restart reconstruction recovers the last accepted WP-07 state from governed evidence. This does not create Stage 9 platform recovery/release authority.

## 8. Relationships to surrounding WPs

### WP-05
Read-only pressure/reclaimability/eligibility/enforcement-state predecessor truth.

### WP-06
Read-only request/decision predecessor truth. `Deny`/`Defer` never authorize positive mutation. WP-07 does not rewrite WP-06 records.

### WP-08
Future consumer of WP-07 mutation/effective-distribution truth. WP-07 does not implement safe projection contracts, Application load-shedding signals/commands, Application degradation order or notification/UI behavior.

## 9. Application neutrality and zero-Application validity

Foundation production code remains generic and Application-neutral.

FSATS, Guardian, FSTSimA, TARC and trading/business names are not production architecture bindings.

Zero Applications remains valid. A coordination envelope requires an exact non-empty constituent set, but absence of an envelope is not Foundation failure.

## 10. Proposed implementation surface

Subject to later file-level reconciliation and separate Owner implementation authorization:

- generic coordination-envelope authority/validation;
- provenance-aware effective-distribution snapshot;
- generic internal mutation intent/effect/record processor;
- environment-neutral resource-effect port;
- Foundation-authoritative mutation intent/effect/record processor or adapter only where exact authority exists;
- dedicated WP-07 verifier;
- reuse of existing canonical identities/evidence/lifetimes/fencing wherever sufficient.

No Application code belongs to Foundation WP-07 write scope.

## 11. Verification families

At minimum:

1. positive envelope creation;
2. exact coordinator instance/role;
3. exact constituent set;
4. predecessor allocation binding;
5. generation supersession;
6. expiry rejection;
7. stale fence rejection;
8. split-brain rejection;
9. cross-epoch rejection;
10. quota headroom not granted capacity;
11. ceiling headroom not granted capacity;
12. admitted envelope total authorized;
13. protected effective minimum preservation;
14. Foundation floor/reserve preservation;
15. positive effective reclaim;
16. non-reclaimable rejection;
17. eligibility-without-authority rejection;
18. positive redistribution;
19. capacity conservation;
20. provenance conservation;
21. source grant provenance retained;
22. borrowed-in/borrowed-out agreement;
23. no anonymous aggregate pool;
24. no double allocation;
25. target ceiling enforcement;
26. target envelope bound enforcement;
27. source minimum enforcement;
28. source movable-capacity enforcement;
29. atomic rebalance positive path;
30. rebalance not a canonical decision kind;
31. atomic partial-effect failure no false success;
32. compensation/rollback evidence;
33. positive effective restore;
34. restoration retains/returns exact provenance;
35. stale historical restore rejection;
36. insufficient-capacity restore rejection;
37. mutation intent alone does not advance truth;
38. effect evidence required;
39. environment-neutral effect port;
40. test adapter does not claim environment qualification;
41. deterministic effective-state identity;
42. deterministic mutation identity;
43. monotonic sequence;
44. duplicate/replay rejection;
45. stale predecessor rejection;
46. correlation preservation;
47. causation preservation;
48. evidence epoch/time validation;
49. Foundation Reduce explicit authority;
50. Foundation Revoke explicit authority;
51. Foundation Restore explicit authority;
52. Foundation rebalance exact authorized mutation set;
53. pressure does not mint authority;
54. priority does not mint authority;
55. criticality does not mint authority;
56. internal move does not mutate Foundation grants/ceilings;
57. Foundation mutation remains exact per-Application truth;
58. Application-neutral production surface;
59. no FSATS/TARC/Guardian/FSTSimA hard-binding;
60. no WP-08 load-shedding executor;
61. zero-Application validity;
62. WP-01 through WP-06 predecessor truth remains read-only;
63. Architecture regression;
64. Security regression;
65. WP-01 through WP-06 verifier regressions;
66. WP-07 verifier run twice from same Release outputs.

## 12. Stop conditions

Implementation shall stop if:

- the coordination envelope cannot function without silently granting FSARM Foundation authority;
- quota/ceiling headroom would be treated as granted capacity;
- borrowed capacity cannot retain exact source Application/grant provenance;
- internal movement would rewrite Foundation grant/ceiling truth;
- authoritative mutation lacks exact Foundation authority;
- accepted truth could advance without proven applied effect;
- effect semantics require environment-specific architecture inside WP-07;
- protected floors/reserves or exact attribution/accounting cannot be proven;
- atomicity cannot be maintained for declared batches;
- WP-08 behavior is required to complete WP-07;
- any WP-01 through WP-06 closure would require reinterpretation without explicit closure-defect evidence.

## 13. Planning state

`WP07_PLANNING_v0.3 = PROPOSED_FINAL_CANDIDATE`

`WP07_OWNER_ACCEPTANCE = NOT_YET`

`WP07_IMPLEMENTATION_AUTHORITY = NOT_GRANTED`

`WP08_IMPLEMENTATION_AUTHORITY = NOT_GRANTED`
