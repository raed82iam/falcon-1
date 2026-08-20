# Stage 6 WP-08 — Planning v0.3 — Final Candidate

**Status:** PROPOSED FINAL CANDIDATE / OWNER REVIEW REQUIRED  
**Stage/WP:** Stage 6 WP-08 — Per-Application Resource State and Load-Shedding Signal Boundary  
**Planning Authority:** DOCUMENTARY PLANNING ONLY  
**Implementation Authority:** NOT GRANTED  
**Date:** 2026-08-10

## 1. Purpose

Stage 6 WP-08 shall provide a generic Foundation-owned **per-Application resource-state projection and technical load-shedding signal boundary** over exact accepted Stage 6 resource-governance truth.

It shall not execute Application business shedding, create resource authority, or create runtime Application authentication/admission/hosting.

## 2. Preserved predecessor authority

WP-08 consumes and preserves:

- WP-01 canonical resource primitives;
- WP-02 Foundation total-resource truth, protection floors and recovery reserves;
- WP-03 Application allocation/quota/ceiling/isolation truth;
- WP-04 Application priority versus Foundation technical criticality separation;
- WP-05 pressure/preemption-eligibility/enforcement-observation truth;
- WP-06 additional-resource request/decision truth;
- WP-07 effective redistribution, Foundation-authoritative mutation, effect evidence and accepted post-mutation truth.

Stage 6 WP-01 through WP-07 remain `ACCEPTED_AND_CLOSED`.

## 3. Mandatory invariants

- `RESOURCE_STATE_PROJECTION != RESOURCE_AUTHORITY`
- `LOAD_SHEDDING_SIGNAL != LOAD_SHEDDING_EXECUTOR`
- `PROJECTION_SCOPE != RUNTIME_AUTHENTICATION_OR_ADMISSION`
- `PRESSURE != AUTHORITY`
- `ENFORCEMENT_OBSERVATION != AUTHORITY`
- `RECLAIMABILITY != MUTATION_AUTHORITY`
- `PRIORITY != AUTHORITY`
- `REQUEST_DECISION != APPLIED_MUTATION`
- `MUTATION_INTENT != APPLIED_EFFECT_EVIDENCE != ACCEPTED_POST_MUTATION_TRUTH`
- `APPLICATION_INTERNAL_SHEDDING_ORDER = APPLICATION_OWNED`
- `AUTHORIZED_AGGREGATE_PROJECTION = ATTRIBUTED_CONSTITUENT_PROJECTIONS`
- `OPAQUE_AGGREGATE_RESOURCE_POOL = FORBIDDEN`
- `ZERO_APPLICATION_OPERATION_IS_VALID = TRUE`

## 4. Canonical Per-Application Resource State Projection

WP-08 shall introduce one generic derived projection per exact Application/resource-class scope.

Each projection shall bind, where applicable:

1. exact `ApplicationPrincipalId`;
2. exact `ResourceClassId`;
3. exact Foundation resource epoch;
4. exact WP-03 allocation predecessor identity;
5. exact authoritative grant/allocation/quota/ceiling values;
6. exact currently accepted effective capacity from WP-07 state where applicable;
7. exact borrowed-effective-capacity provenance affecting the Application, preserving source Application + source Grant attribution;
8. exact WP-05 Application pressure state and pressure availability;
9. exact WP-05 utilization basis points where available;
10. exact WP-05 enforcement-observation state;
11. exact reclaimability/preemption-eligibility evidence where relevant;
12. an explicitly supplied exact applicable WP-06 request/decision reference when present and valid for the same Application/resource scope;
13. exact WP-07 accepted mutation/effective-distribution evidence reference where relevant;
14. observation/generation time;
15. deterministic projection identity.

The projection is a derived read model only. It does not become a new source of allocation, pressure, request-decision, redistribution or mutation truth.

## 5. Exact WP-06 decision-reference rule

WP-08 SHALL NOT invent a generic `latest decision` selector.

Only an explicitly supplied exact accepted WP-06 request/decision reference may be projected, and it must validate for exact Application/resource/predecessor attribution.

Absence of an applicable decision reference is valid.

A WP-06 Grant/PartialGrant/Cap decision is **decision truth**, not proof that allocation/effective capacity has already changed. Applied capacity must come from accepted downstream allocation/mutation truth.

## 6. Direct Application projection scope

WP-08 shall support exact per-Application projection scoping so the resulting projection collection contains only the specified Application's records.

This is data isolation, not live-caller authentication.

WP-08 SHALL NOT claim to establish runtime caller identity, Application admission, session authorization or hosting.

Within its projection model:

- cross-Application substitution fails closed;
- wildcard Application projection is forbidden;
- another Application's state cannot appear in a direct Application-scoped result;
- Application identity does not create resource authority.

## 7. Aggregate coordinator projection scope

A generic aggregate-coordinator projection may be produced only from already accepted exact WP-07 coordination-envelope/coordinator state and the exact constituent Application set.

WP-08 validates projection inputs against predecessor identity material; it does not create runtime coordinator authentication.

The aggregate projection shall:

- bind exact coordinator instance/role/scope;
- bind exact accepted envelope/fence generation/token material where required;
- preserve exact constituent set;
- preserve every constituent as an individually attributable per-Application projection;
- preserve source grant and borrowed-capacity provenance;
- use canonical ordering;
- never emit an opaque merged resource pool;
- mint no new request, redistribution or mutation authority.

## 8. Technical Load-Shedding Signal

WP-08 shall introduce a deterministic generic signal bound to one exact Application/resource projection.

Signal material shall include, where applicable:

- exact Application/resource identity;
- exact projection identity;
- signal class;
- exact accepted compliant/effective capacity target when available;
- WP-05 pressure state/utilization basis points as observational context;
- exact accepted WP-07 effective/mutation state identity when the signal is binding;
- exact source evidence identities;
- inherited correlation/causation only when an exact predecessor relationship exists;
- generation time;
- deterministic signal identity.

## 9. Signal classes

Implementation naming may differ, but these semantics are mandatory.

### 9.1 `NoAction`

No accepted predecessor condition currently supports a reduction recommendation or binding compliance signal.

### 9.2 `AdvisoryReduction`

Accepted WP-05 pressure truth supports a generic technical recommendation to reduce consumption.

This is advisory only:

- pressure does not mint authority;
- enforcement observation does not mint authority;
- no specific resource mutation is implied.

### 9.3 `ComplianceReductionRequired`

This class is permitted only when **accepted WP-07 post-effect/post-mutation truth** already establishes a lower currently effective or authoritative capacity boundary for that exact Application/resource scope.

The WP-08 signal projects the accepted WP-07 requirement; it does not create it.

The signal shall provide the exact accepted **compliant target capacity**.

A numeric `required reduction quantity` may be included only if an exact accepted observed-use quantity is explicitly available from an accepted predecessor/input contract and is coherently attributable to the same Application/resource/epoch. If such exact quantity is not available, WP-08 SHALL NOT reconstruct it from rounded utilization basis points or fabricate it.

If exact observed use is available, the reduction may be derived as:

`REQUIRED_REDUCTION = MAX(EXACT_OBSERVED_USE - ACCEPTED_COMPLIANT_CAPACITY, 0)`

### 9.4 `StateUnavailable`

When truth required to safely classify a signal is unavailable, stale, contradictory, epoch-inconsistent or cannot be bound to exact accepted evidence, the signal fails closed to unavailable.

## 10. WP-05 truth usage rule

WP-05 supplies observational pressure/enforcement truth.

WP-08 may project:

- pressure availability/state;
- utilization basis points;
- enforcement observation state;
- reclaimability/preemption eligibility evidence.

WP-08 SHALL NOT reinterpret WP-05 enforcement observation as mutation authority or as proof that a lower capacity boundary was authorized/applied.

WP-08 SHALL NOT infer exact used quantity from utilization basis points when doing so would depend on rounding or an unavailable original observation quantity.

## 11. Effective-capacity and provenance rule

WP-08 shall distinguish:

- authoritative grant/allocation/quota/ceiling ownership;
- current accepted effective capacity;
- borrowed effective capacity and exact provenance.

Quota/ceiling headroom is not granted/effective capacity.

Borrowed capacity shall preserve source Application + source Grant provenance and target attribution.

## 12. Accepted effect rule

WP-08 projects accepted post-effect/post-mutation truth only.

A mutation intent, failed effect, partial effect, unapplied request decision or unaccepted candidate state SHALL NOT be projected as accepted capacity change.

## 13. Restoration rule

WP-08 shall project restored capacity only from accepted WP-07 Restore post-effect truth bounded by the captured authoritative restoration basis.

Pressure recovery or a WP-05 enforcement observation alone shall not create restoration state.

## 14. Application-internal shedding remains Application-owned

Foundation may tell an Application generic technical facts such as:

- authoritative allocation/quota/ceiling;
- current accepted effective capacity;
- pressure state/utilization basis points;
- exact applicable request/decision context;
- accepted reduced/restored capacity state;
- advisory reduction recommendation;
- exact compliant capacity target from accepted WP-07 state.

Foundation SHALL NOT choose or encode which internal Application workload is reduced/stopped first.

No generic Foundation public surface may encode FSATS-specific strategies, brokers, markets, providers, trading flows, simulations, Guardian business actions, MSA/LSA/CSA internals or other Application-specific shedding order.

## 15. Determinism and reconstructability

Projection and signal identities shall include all semantically material fields and exact predecessor identities.

Canonical ordering is mandatory for constituent projections and provenance collections.

Material changes to Application/resource/epoch, allocation predecessor, grant/allocation/quota/ceiling, effective capacity, provenance, pressure/enforcement state, decision reference, accepted mutation/effect reference, signal class, compliant target or evidence/generation material shall alter identity where applicable.

## 16. Fail-closed rules

WP-08 shall fail closed for at least:

- unknown Application/resource;
- unavailable required allocation truth;
- cross-epoch predecessor mismatch;
- mismatched pressure scope;
- mismatched exact WP-06 decision reference;
- failed/partial/unaccepted WP-07 effect presented as accepted state;
- WP-07 effective state inconsistent with its authoritative predecessor;
- aggregate constituent-set mismatch;
- stale/conflicting accepted coordinator-envelope/fence material where current validity is required;
- cross-Application projection substitution;
- duplicate ambiguous records;
- quantity-unit mismatch;
- compliance signal without exact accepted WP-07 lower-capacity basis;
- fabricated exact used quantity or fabricated reduction amount.

Pressure may be explicitly unavailable in a pure allocation-state projection. A signal path requiring pressure must fail closed when the required pressure truth is unavailable.

## 17. Zero-Application validity

Zero Applications is valid. An empty projection set is valid and shall not manufacture placeholder Application state.

## 18. Expected implementation placement

Subject to mandatory pre-implementation file-level reconciliation:

- Foundation production logic under `src/Foundation.State/` using the existing generic resource-governance namespace;
- dedicated `verification/Falcon.Stage6.WP08.Verifier/`;
- controlled solution integration only;
- no writes under `applications/**` or `reference/**`;
- Foundation Contracts modification only if exact reconciliation proves a genuinely missing generic primitive cannot be represented safely by existing accepted primitives.

## 19. Mandatory verifier coverage

### Projection truth

- direct Application positive projection;
- exact allocation/quota/ceiling binding;
- exact accepted effective-capacity projection;
- borrowed provenance preservation;
- pressure available/unavailable representation;
- utilization-basis-points projection;
- enforcement-observation projection without authority inflation;
- exact applicable WP-06 decision reference;
- decision not treated as applied capacity;
- accepted WP-07 post-effect state projection;
- restoration only from accepted Restore truth;
- deterministic identity/canonical ordering;
- zero-Application validity.

### Isolation / aggregate coordinator

- direct projection contains only exact Application;
- cross-Application substitution rejected;
- wildcard projection absent;
- exact WP-07 coordinator/envelope constituent set;
- stale/conflicting coordinator-envelope/fence material rejected where required;
- every aggregate constituent remains independently attributable;
- no opaque aggregate pool.

### Signal behavior

- NoAction path;
- AdvisoryReduction path;
- ComplianceReductionRequired path;
- StateUnavailable path;
- pressure does not mint authority;
- enforcement observation does not mint authority;
- compliance signal requires exact accepted WP-07 capacity basis;
- compliant target equals accepted WP-07 effective/authoritative state;
- exact reduction quantity omitted when exact observed use is unavailable;
- exact reduction mathematically derived only when exact accepted observed use is explicitly available;
- utilization basis points are not reverse-engineered into fabricated exact use;
- headroom not treated as capacity;
- failed/partial effect does not create accepted reduction;
- pressure recovery alone does not create Restore state.

### Architecture boundaries

- projection scope does not claim runtime authentication/admission;
- no Application-internal shedding executor;
- no FSATS/TARC/FSARM hard binding in generic Foundation public surface;
- no WP-09 integration/hardening implementation;
- no environment-specific architecture meaning;
- no external-access/credential/financial authority;
- WP-01 through WP-07 regression-clean.

## 20. Acceptance gates

In order:

1. final planning Red-Team PASS;
2. explicit Owner planning acceptance;
3. separate explicit Owner implementation authorization;
4. pre-implementation file-level reconciliation/Red-Team;
5. implementation + self-review;
6. post-implementation static Red-Team;
7. exact-commit executable validation: Restore, Release Build, Architecture, Security, WP-01 through WP-07 predecessor verifiers, WP-08 verifier twice from the same Release outputs, final exact-HEAD/clean-worktree integrity;
8. post-executable Red-Team/reconciliation;
9. Application implementation-compatibility verification for FCR-0010/FCR-0031 as applicable;
10. explicit Owner final closure.

## 21. Explicit non-authorities

This planning artifact and any later technical success do not grant:

- WP-09/WP-10 implementation authority;
- runtime Application admission/hosting authority;
- Application business-policy authority;
- production/deployment authority;
- external access/credentials;
- broker, market-data, trading or financial authority;
- authority to reopen WP-01 through WP-07 absent explicit closure-defect evidence.

## 22. Planning disposition

`WP08_PLANNING = PROPOSED_v0.3_FINAL_CANDIDATE`

`WP08_IMPLEMENTATION_AUTHORITY = NOT_GRANTED`

`WP09_IMPLEMENTATION_AUTHORITY = NOT_GRANTED`

This artifact is ready for final Red-Team and Owner review only.
