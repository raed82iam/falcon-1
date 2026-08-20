# Stage 6 WP-08 — Planning Draft v0.1

**Status:** PROPOSED / OWNER REVIEW REQUIRED  
**Stage/WP:** Stage 6 WP-08 — Per-Application Resource State and Load-Shedding Signal Boundary  
**Planning Authority:** DOCUMENTARY PLANNING ONLY  
**Implementation Authority:** NOT GRANTED  
**Date:** 2026-08-10

## 1. Purpose

Stage 6 WP-08 shall provide a generic Foundation-owned boundary that projects exact accepted resource state to an Application, or to an explicitly authorized aggregate coordinator acting over an exact constituent set, and produces deterministic technical load-shedding signals without executing Application business shedding.

WP-08 completes the remaining Stage 6 pre-integration resource-governance boundary before WP-09 integration/hardening.

## 2. Governing predecessors

WP-08 shall consume and preserve the accepted meaning of:

- WP-01 canonical resource primitives;
- WP-02 Foundation resource truth, protected floors and recovery reserves;
- WP-03 Application allocation/quota/ceiling/isolation truth;
- WP-04 Application priority versus Foundation technical criticality separation;
- WP-05 Application/global pressure, preemption-eligibility and enforcement-state truth;
- WP-06 additional-resource request/decision truth;
- WP-07 effective redistribution, Foundation-authoritative mutation, effect evidence and accepted post-mutation truth.

No predecessor is reopened by WP-08.

## 3. Core invariant set

The following invariants are mandatory:

- `RESOURCE_STATE_PROJECTION != RESOURCE_AUTHORITY`
- `LOAD_SHEDDING_SIGNAL != LOAD_SHEDDING_EXECUTOR`
- `PRESSURE != AUTHORITY`
- `RECLAIMABILITY != MUTATION_AUTHORITY`
- `PRIORITY != AUTHORITY`
- `REQUEST_DECISION != APPLIED_MUTATION`
- `MUTATION_INTENT != APPLIED_EFFECT_EVIDENCE != ACCEPTED_POST_MUTATION_TRUTH`
- `APPLICATION_INTERNAL_SHEDDING_ORDER = APPLICATION_OWNED`
- `AUTHORIZED_AGGREGATE_VIEW = ATTRIBUTED_CONSTITUENT_VIEWS`, never an opaque pool
- `ZERO_APPLICATION_OPERATION_IS_VALID = TRUE`

## 4. Per-Application Resource State Projection

WP-08 shall introduce one canonical resource-state projection per exact Application and resource class.

Each projected resource entry shall bind, where applicable:

1. exact `ApplicationPrincipalId`;
2. exact `ResourceClassId`;
3. exact Foundation resource epoch;
4. exact authoritative WP-03 grant/allocation/quota/ceiling identity;
5. exact current effective capacity after accepted WP-07 redistribution/mutation state;
6. exact borrowed-capacity provenance affecting that Application, without transferring authoritative grant ownership;
7. exact WP-05 Application pressure state and pressure availability;
8. exact WP-05 utilization basis points where available;
9. exact WP-05 enforcement-observation state;
10. exact preemption/reclaimability eligibility evidence where relevant;
11. latest exact WP-06 request/decision reference affecting the same Application/resource scope where present;
12. exact WP-07 accepted mutation/effective-distribution evidence reference where relevant;
13. observation time;
14. deterministic identity.

The projection shall be a **derived read model**. It shall not become a new authoritative allocation, pressure, request-decision or mutation source of truth.

## 5. Direct Application view

A direct Application view shall expose only projections whose `ApplicationPrincipalId` exactly matches the requesting/authorized Application identity.

Cross-Application substitution, accidental disclosure, wildcard access and caller-selected arbitrary Application IDs shall fail closed.

An Application identity alone does not create authority to read another Application's state.

## 6. Authorized aggregate-coordinator view

WP-08 may support a generic delegated aggregate-coordinator projection view only when bound to an exact accepted coordinator authority/fence and exact constituent Application set.

Requirements:

- coordinator instance identity is exact;
- coordinator role identity is exact;
- scope identity is exact;
- authority generation is current;
- fence generation/token is current;
- authority lifetime is valid;
- constituent Application set is exact;
- returned state remains a collection of individually attributable per-Application projections;
- no constituent identity, grant identity or borrowed-capacity provenance may be erased;
- no merged opaque resource pool may be emitted.

This view does not make the coordinator a Foundation principal and does not grant mutation or request-decision authority beyond separately accepted predecessor authority.

## 7. Load-Shedding Signal model

WP-08 shall emit generic technical load-shedding signals derived from accepted Foundation resource truth.

A signal shall bind:

- exact Application;
- exact resource class;
- exact source resource-state projection identity;
- signal classification;
- current effective capacity;
- observed used capacity where available;
- target compliant capacity or required reduction quantity when calculable;
- source pressure/enforcement/mutation evidence identities;
- correlation/causation where inherited from a relevant accepted predecessor event;
- observation/generation time;
- deterministic identity.

### Signal classifications

The planning model distinguishes at least:

1. `NoAction`
   - accepted state does not currently require or recommend technical consumption reduction.

2. `AdvisoryReduction`
   - accepted pressure state justifies a technical reduction recommendation, but no separate Foundation-authoritative capacity mutation makes a specific reduction binding.
   - advisory signal **does not create authority**.

3. `ComplianceReductionRequired`
   - accepted authoritative/effective capacity is below current observed use, or an accepted predecessor enforcement/mutation state makes compliance with a lower capacity boundary mandatory.
   - required quantity/target shall be mathematically derived from accepted predecessor truth, not caller supplied.
   - authority comes from the accepted predecessor mutation/enforcement boundary, not from the WP-08 signal itself.

4. `StateUnavailable`
   - required predecessor truth is unavailable, stale, epoch-inconsistent or otherwise not safely projectable.
   - fail closed; do not fabricate a normal/advisory/required signal.

Final implementation naming may use equivalent generic identifiers, but these semantics shall be preserved.

## 8. What Foundation may tell an Application

Foundation may communicate technical facts and bounds such as:

- your authoritative allocation/quota/ceiling is X/Y/Z;
- your currently effective capacity is E;
- your observed resource use is U;
- your current pressure state is P;
- your accepted request outcome is D;
- your accepted capacity was reduced/restored under exact predecessor evidence;
- reduce technical consumption by Q / to target E when exact accepted truth requires it.

Foundation shall **not** tell the Application which internal business function to stop first.

## 9. Application-owned shedding decision

The Application remains responsible for deciding how to comply internally with a generic Foundation technical reduction requirement.

Examples of prohibited Foundation leakage include naming or ordering Application-specific:

- strategies;
- trading execution flows;
- market scanners;
- providers;
- simulations;
- Guardian business actions;
- MSA/LSA/CSA internal functions;
- broker/data/trading workflows.

Those are Application business semantics and remain outside Foundation WP-08.

## 10. Signal derivation constraints

### 10.1 No caller-supplied reduction truth

Required reduction quantity or target capacity shall be derived from accepted predecessor state. Caller-supplied arbitrary reduction quantities shall not become trusted signal truth.

### 10.2 Pressure does not mint authority

Pressure may produce an advisory signal. A binding compliance reduction must remain traceable to an accepted authoritative/effective capacity boundary or enforcement condition.

### 10.3 Headroom is not capacity

Quota/ceiling headroom shall never be projected as granted/effective capacity.

### 10.4 Borrowed capacity provenance

If WP-07 accepted borrowed effective capacity is part of current effective capacity, WP-08 shall preserve source Application + source Grant provenance while attributing use/availability to the target Application.

### 10.5 Mutation truth precedence

WP-08 shall project accepted post-mutation truth, not mutation intent alone. A proposed/failed/partial effect shall not be projected as accepted reduced/restored state.

### 10.6 Epoch and predecessor coherence

Allocation, pressure, request/decision, effective-distribution/mutation and projection evidence used in one projected state must be coherently attributable to the accepted resource epoch and exact predecessor identities required by their contracts.

Incompatible predecessor truth shall fail closed.

## 11. Request/decision status projection

WP-08 may expose an Application's latest applicable WP-06 request/decision status only as attributable state context.

It shall preserve:

- exact request identity;
- exact decision identity;
- exact requester attribution;
- decision outcome;
- requested quantity versus proven residual need versus decided quantity distinction.

WP-08 shall not infer that a `Grant` or `PartialGrant` is already applied allocation truth unless accepted downstream allocation/mutation truth proves application.

## 12. Restoration state projection

WP-08 may expose restoration eligibility/state only from accepted predecessor evidence.

It shall not infer restoration from pressure improvement alone.

An accepted WP-07 Restore remains bounded by captured authoritative restoration basis and applied-effect evidence.

## 13. Determinism and reconstructability

Every projection and signal shall have deterministic identity over all semantically material fields.

Changing any of the following shall change identity where relevant:

- Application/resource identity;
- predecessor snapshot identities;
- grant/allocation/quota/ceiling;
- current effective capacity;
- borrowed provenance;
- pressure/enforcement state;
- request/decision reference;
- mutation/effect reference;
- signal classification;
- target/reduction quantity;
- observation time/evidence.

Collections shall have canonical ordering.

## 14. Fail-closed conditions

WP-08 shall fail closed for at least:

- unknown Application/resource;
- unavailable authoritative allocation truth;
- unavailable required pressure truth;
- cross-epoch predecessor mismatch;
- stale or invalid aggregate coordinator authority/fence;
- constituent-set mismatch;
- cross-Application state substitution;
- inconsistent WP-07 effective state versus authoritative allocation predecessor;
- signal quantity unit mismatch;
- required reduction that cannot be proven from accepted state;
- failed/partial effect presented as accepted mutation truth;
- ambiguous duplicate predecessor/state record.

## 15. Zero-Application validity

Foundation with zero Applications remains valid.

WP-08 shall support an empty projection set without manufacturing placeholder Applications or treating zero Applications as an error.

## 16. Expected production placement

Planning expectation, subject to file-level reconciliation immediately before implementation:

- Foundation-owned production logic under `src/Foundation.State/` using the existing generic resource-governance namespace;
- dedicated `verification/Falcon.Stage6.WP08.Verifier/`;
- controlled solution integration only;
- Foundation Contracts changes only if exact reconciliation proves a genuinely missing generic primitive cannot be represented without them.

No Application or reference tree writes are authorized.

## 17. Required verifier coverage

The dedicated WP-08 verifier shall cover at least:

### Projection
- direct Application positive state;
- exact allocation/quota/ceiling projection;
- exact effective-capacity projection;
- pressure/enforcement projection;
- borrowed provenance preservation;
- request/decision attribution;
- accepted mutation/restoration evidence projection;
- deterministic identity;
- canonical ordering;
- zero-Application validity.

### Isolation
- another Application's state is not exposed;
- arbitrary caller Application substitution rejected;
- aggregate coordinator exact constituent scope;
- stale coordinator authority rejected;
- stale fence rejected;
- split-brain/conflicting coordinator state rejected;
- no opaque aggregate pool.

### Load-shedding signal
- `NoAction` positive path;
- advisory reduction positive path;
- compliance-required reduction positive path;
- unavailable state fail-closed path;
- required quantity mathematically derived;
- headroom not treated as capacity;
- pressure not treated as authority;
- request decision not treated as applied mutation;
- failed/partial mutation effect not treated as accepted truth;
- restoration not inferred from pressure recovery alone.

### Boundary protection
- no Application business names/semantics in Foundation public surface;
- no Application-internal shedding executor;
- no WP-09 integration/hardening engine;
- no environment-specific meaning;
- no egress/credentials/financial authority;
- WP-01 through WP-07 accepted behavior remains regression-clean.

## 18. Acceptance gates

Before technical acceptance, WP-08 shall require:

1. Owner acceptance of final planning artifact after Red-Team;
2. separate explicit Owner implementation authorization;
3. pre-implementation file-level reconciliation/Red-Team;
4. implementation self-review;
5. post-implementation static Red-Team;
6. exact-commit executable validation including Restore, Release Build, Architecture, Security, WP-01 through WP-07 regression verifiers and WP-08 verifier twice from the same Release outputs;
7. fresh post-executable Red-Team/reconciliation;
8. Application implementation-compatibility verification for applicable FCR boundaries;
9. explicit Owner final closure.

## 19. Explicit non-authorities

Planning or later technical success SHALL NOT grant:

- WP-09/WP-10 implementation authority;
- Application business-policy authority;
- runtime deployment/production authority;
- external connectivity/credential authority;
- broker, market-data, trading or financial authority;
- authority to reopen WP-01 through WP-07 without explicit closure-defect evidence.

## 20. Planning disposition

`WP08_PLANNING = PROPOSED_v0.1`

`WP08_IMPLEMENTATION_AUTHORITY = NOT_GRANTED`

`WP09_IMPLEMENTATION_AUTHORITY = NOT_GRANTED`

This draft remains subject to Red-Team and Owner review.
