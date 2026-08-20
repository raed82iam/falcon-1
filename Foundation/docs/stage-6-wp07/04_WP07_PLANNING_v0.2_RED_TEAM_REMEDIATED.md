# Stage 6 WP-07 — Planning v0.2 Red-Team Remediated

Status: PROPOSED / NOT OWNER-ACCEPTED / NOT IMPLEMENTATION AUTHORITY
Date: 2026-08-10
Supersedes planning proposal v0.1 prospectively.

## 1. Purpose

Plan Stage 6 WP-07 `Reclamation, Redistribution, Rebalance and Restoration` as a governed mutation layer built on accepted WP-01 through WP-06 truth and authority.

WP-07 has two deliberately separate execution lanes:

1. delegated effective-distribution mutation inside an exact Foundation-authorized coordination envelope;
2. Foundation-authoritative allocation mutation only under exact Foundation mutation authority.

These lanes may share canonical evidence/identity primitives but SHALL NOT share authority by implication.

## 2. Core invariants

- `EFFECTIVE_DISTRIBUTION != FOUNDATION_AUTHORITATIVE_ALLOCATION`
- `ELIGIBILITY != MUTATION_AUTHORITY`
- `WP06_DECISION != WP07_APPLIED_MUTATION`
- `INTERNAL_COORDINATION_MUTATION != FOUNDATION_GRANT_MUTATION`
- `MUTATION_INTENT != APPLIED_EFFECT_EVIDENCE != ACCEPTED_POST_MUTATION_TRUTH`
- `INTERNAL_REDISTRIBUTION_FIRST`
- `FOUNDATION_ADDITIONAL_REQUEST_SECOND`
- `QUOTA_HEADROOM != GRANTED_CAPACITY`
- `CEILING_HEADROOM != GRANTED_CAPACITY`
- `PRESERVE_PROTECTION_FLOORS = TRUE`
- `PRESERVE_RECOVERY_RESERVES = TRUE`
- `PRESERVE_CONSTITUENT_ATTRIBUTION = TRUE`
- `NO_OPAQUE_AGGREGATE_POOL = TRUE`
- `NO_APPLICATION_BUSINESS_SEMANTICS_IN_FOUNDATION = TRUE`

## 3. Lane A — Delegated coordination-envelope mutation

### 3.1 Purpose

Permit an exact delegated aggregate coordinator to perform bounded operational effective reclamation/redistribution/rebalance/restoration across an exact constituent Application set without a Foundation round-trip for every valid internal move.

This lane does not alter Foundation authoritative grants/allocations/quotas/ceilings.

### 3.2 Foundation-authorized coordination envelope

The envelope shall bind at minimum:

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
- exact total capacity admitted into the envelope;
- exact movable-capacity limit;
- reclaimability constraints;
- correlation/causation/evidence requirements.

The envelope shall fail closed if stale, expired, superseded, split-brain, cross-epoch, predecessor-mismatched or constituent-mismatched.

### 3.3 Capacity accounting

WP-07 shall keep these values distinct for every Application/resource class:

- `AuthoritativeAllocation`
- `AuthoritativeQuota`
- `AuthoritativeCeiling`
- `ProtectedEffectiveMinimum`
- `CurrentEffectiveAssignment`
- `EnvelopeMovableCapacity`

Quota or ceiling headroom alone is not granted capacity and cannot enter the movable pool merely because it exists.

The total capacity admitted into a coordination envelope must be explicitly Foundation-authorized and derived only from capacity already made available to the exact governed constituent set.

A target's effective assignment may change inside the valid envelope, but:

- it may not exceed the target's authoritative ceiling;
- it may not create or imply a new Foundation grant;
- the envelope total must conserve already-authorized capacity;
- the post-mutation total effective assignment may not exceed the envelope-admitted total;
- source protected effective minimums may not be violated.

### 3.4 Effective-distribution state

WP-07 shall maintain separately identified effective-distribution truth derived from, but not replacing, the Foundation authoritative allocation snapshot.

For each constituent binding it shall retain at minimum:

- exact Application identity;
- exact resource class;
- authoritative grant identity;
- authoritative allocation/quota/ceiling predecessor values;
- effective assigned quantity;
- protected effective minimum;
- currently reclaimable effective quantity;
- mutation sequence;
- last accepted mutation identity;
- evidence references.

### 3.5 Internal mutation operations

The internal effective-distribution lane recognizes:

- `EffectiveReclaim`
- `EffectiveRedistribute`
- `EffectiveRebalance`
- `EffectiveRestore`

These are WP-07 internal effective-distribution operation names, not Foundation resource decision kinds.

### 3.6 Effective reclaim

A source may release effective capacity only when:

- exact predecessor allocation/grant remains current;
- the relevant WP-05 reclaimability/eligibility evidence is current;
- separate valid envelope authority permits the mutation;
- protected effective minimum remains satisfied;
- quantity and resource-class units match exactly;
- coordinator authority/fence remain current;
- source and target belong to the exact envelope constituent set.

WP-05 eligibility is necessary evidence where applicable but never sufficient mutation authority.

### 3.7 Effective redistribution

A redistribution shall bind exact source(s), target(s), quantities, before-state identity and intended after-state identity.

Capacity conservation is mandatory.

No quantity may be created, silently discarded, counted twice or transferred across an unauthorized constituent/resource scope.

### 3.8 Effective rebalance

`Rebalance` is a transaction concept, not a new canonical resource decision kind.

An effective rebalance is one atomic batch of exact internal `EffectiveReclaim` / `EffectiveRedistribute` / `EffectiveRestore` changes under one valid envelope and one exact predecessor effective-distribution state.

The declared batch is all-or-nothing.

### 3.9 Effective restoration

A historical pre-reclaim state is evidence only. It is not automatic restoration authority and not automatically the correct current target.

Restoration shall be recalculated against current:

- Foundation authoritative allocation snapshot;
- envelope generation and bounds;
- current effective distribution;
- pressure/reclaimability evidence where applicable;
- coordinator authority/fencing;
- current available effective capacity.

A stale historical target shall be rejected if current conditions cannot validly support it.

## 4. Mutation effect and commit model

### 4.1 Three-state separation

Every operational mutation shall distinguish:

1. `MutationIntent`
2. `AppliedEffectEvidence`
3. `AcceptedPostMutationTruth`

A mutation intent alone does not change accepted effective-distribution truth.

### 4.2 Commit rule

Accepted post-mutation truth may advance only when:

- the underlying effect operation is proven applied by exact governed evidence; or
- the implementation uses an in-process atomic mechanism where effect and state commit are inseparable and this property is explicitly verified.

If application of the effect fails or becomes partial, the intended after-state shall not be published as accepted truth.

### 4.3 Partial failure

For multi-binding rebalance:

- partial application is not an accepted final state;
- compensation/rollback must restore the last accepted state or produce an explicitly governed failure state with no false success claim;
- all effect and compensation evidence must remain reconstructable.

## 5. Lane B — Foundation-authoritative mutation

### 5.1 Purpose

Handle actions that actually modify canonical Foundation Application allocation truth only under exact Foundation authority.

### 5.2 Authority rule

No Foundation-authoritative mutation authority may be inferred from pressure, preemption eligibility, priority, technical criticality, FSARM preference, internal effective redistribution or urgency.

A WP-06 record is predecessor evidence only unless the exact accepted authority model explicitly makes it applicable to the mutation being performed.

### 5.3 Canonical mutation actions

WP-07 may consume canonical Foundation mutation decisions/authority for exact actions such as:

- `Reduce`
- `Revoke`
- `Restore`

WP-07 SHALL NOT invent `Rebalance` as a new canonical decision kind.

A Foundation-authoritative rebalance, when required, is a governed atomic transaction composed from exact separately authorized canonical mutations against one exact predecessor allocation state.

### 5.4 Foundation authoritative mutation record

Every authoritative mutation shall bind at minimum:

- exact authority/decision identity;
- exact predecessor Foundation allocation snapshot identity;
- exact Application/resource/grant binding;
- exact mutation action and quantity;
- exact before/after authoritative state identity;
- exact epoch;
- lifetime;
- correlation/causation;
- applied-effect evidence;
- deterministic mutation identity.

## 6. Replay, ordering and fencing

Both lanes shall provide:

- monotonic mutation sequence per exact governed scope;
- deterministic identity;
- duplicate mutation rejection;
- replay-safe idempotency;
- stale predecessor rejection;
- cross-epoch rejection;
- expired/superseded authority rejection;
- stale fencing rejection;
- split-brain coordinator rejection;
- before/after state binding;
- correlation/causation preservation;
- exact evidence provenance.

## 7. Failure/restart semantics

A failed mutation leaves the last accepted truth valid.

Coordinator failure does not transfer authority automatically.

A successor coordinator requires valid successor authority and fencing.

Restart reconstruction shall recover the last accepted WP-07 state from exact governed evidence and predecessor identities. WP-07 reconstruction does not create Stage 9 platform recovery/release authority.

## 8. Relationship to WP-05

WP-05 pressure, reclaimability and enforcement-state truth remain read-only predecessor evidence.

WP-07 may consume eligibility but does not rewrite WP-05 truth.

## 9. Relationship to WP-06

WP-06 remains accepted-and-closed request/decision truth.

`Grant`, `PartialGrant`, `Cap`, `Deny` and `Defer` remain WP-06 outcomes.

WP-07 shall not reinterpret `Deny` or `Defer` as positive mutation authority.

No WP-06 record is modified by WP-07.

## 10. Relationship to WP-08

WP-07 produces mutation/effective-distribution truth that WP-08 may later project.

WP-07 does not implement safe projection contracts, Application load-shedding signals/commands, Application-local degradation order or UI/notification behavior.

## 11. Application neutrality and zero-Application validity

Foundation production code remains generic and Application-neutral.

FSATS/Guardian/FSTSimA/TARC are not production architecture bindings.

Zero Applications remains a valid Foundation state. A coordination envelope itself requires an exact non-empty constituent set, but the absence of such an envelope is not an error.

## 12. Proposed implementation surface

Subject to later file-level reconciliation and separate Owner implementation authorization, likely Foundation-owned surface:

- coordination-envelope authority/validation;
- effective-distribution snapshot;
- internal mutation intent/effect/record processor;
- Foundation-authoritative mutation intent/effect/record processor or adapter where exact authority exists;
- deterministic mutation/evidence primitives only if existing WP-01 primitives are insufficient;
- dedicated WP-07 verifier.

The implementation shall prefer reuse of existing canonical identities/evidence/lifetimes/fencing primitives over duplication.

No Application code is in Foundation WP-07 write scope.

## 13. Verification families

At minimum:

1. positive envelope;
2. exact coordinator instance/role;
3. exact constituent set;
4. predecessor allocation binding;
5. envelope generation supersession;
6. expiry rejection;
7. stale fence rejection;
8. split-brain rejection;
9. cross-epoch rejection;
10. quota headroom is not granted capacity;
11. ceiling headroom is not granted capacity;
12. admitted envelope total derives only from authorized capacity;
13. per-App protected minimum preservation;
14. Foundation floor/reserve preservation;
15. positive effective reclaim;
16. non-reclaimable rejection;
17. eligibility-without-authority rejection;
18. positive redistribution;
19. conservation of effective capacity;
20. no double allocation;
21. target ceiling enforcement;
22. source minimum enforcement;
23. atomic rebalance positive path;
24. rebalance is not a canonical decision kind;
25. atomic partial-effect failure does not publish success;
26. compensation/rollback evidence;
27. positive effective restoration;
28. stale historical restoration target rejection;
29. restoration insufficient-capacity rejection;
30. mutation intent does not advance accepted truth;
31. effect evidence required before accepted post-state;
32. deterministic effective-state identity;
33. deterministic mutation identity;
34. monotonic sequence;
35. duplicate/replay rejection;
36. stale predecessor rejection;
37. correlation preservation;
38. causation preservation;
39. evidence epoch/time validation;
40. Foundation Reduce requires explicit authority;
41. Foundation Revoke requires explicit authority;
42. Foundation Restore requires explicit authority;
43. Foundation rebalance requires separately authorized canonical mutation set;
44. pressure does not mint authority;
45. priority does not mint authority;
46. criticality does not mint authority;
47. internal effective move does not mutate Foundation grants/ceilings;
48. Foundation authoritative mutation remains exact per-Application truth;
49. no opaque aggregate pool;
50. Application-neutral production surface;
51. no FSATS/TARC/Guardian/FSTSimA hard-binding;
52. no WP-08 load-shedding executor;
53. zero-Application validity;
54. WP-01 through WP-06 predecessor truth read-only;
55. Architecture regression;
56. Security regression;
57. WP-01 through WP-06 verifier regressions;
58. WP-07 verifier run twice from same Release outputs.

## 14. Stop conditions

Implementation shall stop if:

- FSARM would need Foundation authority merely to make the envelope functional;
- quota/ceiling headroom would be treated as granted capacity;
- an internal move would mutate Foundation authoritative grant/ceiling truth;
- an authoritative mutation lacks exact Foundation authority;
- accepted truth could advance without proven applied effect;
- protected floors/reserves or exact constituent attribution cannot be proven;
- atomicity cannot be maintained for declared multi-binding transactions;
- WP-08 behavior is required to make WP-07 complete;
- any WP-01 through WP-06 closure would need reinterpretation without explicit closure-defect evidence.

## 15. Planning state

`WP07_PLANNING_v0.2 = PROPOSED`

`WP07_OWNER_ACCEPTANCE = NOT_YET`

`WP07_IMPLEMENTATION_AUTHORITY = NOT_GRANTED`

`WP08_IMPLEMENTATION_AUTHORITY = NOT_GRANTED`
