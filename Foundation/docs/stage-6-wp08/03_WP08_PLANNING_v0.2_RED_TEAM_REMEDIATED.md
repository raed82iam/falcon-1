# Stage 6 WP-08 — Planning v0.2 — Red-Team Remediated

**Status:** PROPOSED FINAL CANDIDATE / OWNER REVIEW REQUIRED  
**Stage/WP:** Stage 6 WP-08 — Per-Application Resource State and Load-Shedding Signal Boundary  
**Planning Authority:** DOCUMENTARY PLANNING ONLY  
**Implementation Authority:** NOT GRANTED  
**Date:** 2026-08-10

## 1. Purpose

Stage 6 WP-08 shall provide a generic Foundation-owned **resource-state projection and technical load-shedding signal boundary** over exact accepted Stage 6 resource-governance truth.

WP-08 does not execute Application business shedding and does not create runtime hosting/authentication/admission. It prepares deterministic, isolated, attributable resource-state and signal artifacts that later runtime consumers may use under separately accepted authority.

## 2. Governing predecessors

WP-08 consumes and preserves:

- WP-01 canonical resource primitives;
- WP-02 Foundation total-resource truth, protection floors and recovery reserves;
- WP-03 Application allocation/quota/ceiling/isolation truth;
- WP-04 Application priority versus Foundation technical criticality separation;
- WP-05 pressure/preemption-eligibility/enforcement-state truth;
- WP-06 additional-resource request/decision truth;
- WP-07 effective redistribution, authoritative mutation, effect evidence and accepted post-mutation truth.

Stage 6 WP-01 through WP-07 remain `ACCEPTED_AND_CLOSED` and are not reopened.

## 3. Mandatory semantic invariants

- `RESOURCE_STATE_PROJECTION != RESOURCE_AUTHORITY`
- `LOAD_SHEDDING_SIGNAL != LOAD_SHEDDING_EXECUTOR`
- `PROJECTION_SCOPE != RUNTIME_AUTHENTICATION_OR_ADMISSION`
- `PRESSURE != AUTHORITY`
- `RECLAIMABILITY != MUTATION_AUTHORITY`
- `PRIORITY != AUTHORITY`
- `REQUEST_DECISION != APPLIED_MUTATION`
- `MUTATION_INTENT != APPLIED_EFFECT_EVIDENCE != ACCEPTED_POST_MUTATION_TRUTH`
- `APPLICATION_INTERNAL_SHEDDING_ORDER = APPLICATION_OWNED`
- `AUTHORIZED_AGGREGATE_PROJECTION = ATTRIBUTED_CONSTITUENT_PROJECTIONS`
- `OPAQUE_AGGREGATE_RESOURCE_POOL = FORBIDDEN`
- `ZERO_APPLICATION_OPERATION_IS_VALID = TRUE`

## 4. Canonical Per-Application Resource State Projection

WP-08 shall introduce one generic projection record per exact Application/resource-class scope.

Each projection shall bind, where applicable:

1. exact `ApplicationPrincipalId`;
2. exact `ResourceClassId`;
3. exact Foundation resource epoch;
4. exact WP-03 allocation predecessor identity;
5. exact authoritative grant/allocation/quota/ceiling values;
6. exact currently accepted effective capacity from WP-07 state where applicable;
7. exact borrowed-effective-capacity provenance affecting the Application, preserving source Application + source Grant attribution;
8. exact WP-05 Application pressure state and availability;
9. exact WP-05 utilization basis points where available;
10. exact WP-05 enforcement-observation state;
11. exact reclaimability/preemption-eligibility evidence where relevant;
12. **an explicitly supplied exact applicable WP-06 request/decision reference**, when one is part of the projection input and validates against the same Application/resource scope;
13. exact WP-07 accepted mutation/effective-distribution evidence reference where relevant;
14. observation/generation time;
15. deterministic projection identity.

The projection is a derived read model. It is not a second source of authoritative allocation, pressure, request-decision, redistribution or mutation truth.

## 5. WP-06 decision-reference rule

WP-08 SHALL NOT invent a generic `latest decision` selector unless a future separately governed capability provides canonical ordering/selection semantics.

WP-08 may project only an **exact applicable accepted WP-06 request/decision reference** supplied as part of projection construction and validated for:

- exact Application/resource attribution;
- exact request/decision identity;
- exact predecessor epoch/snapshot relationships required by WP-06;
- requested quantity versus proven residual need versus decided quantity separation.

Absence of an applicable decision reference is valid.

A WP-06 `Grant`/`PartialGrant`/`Cap` result shall not be projected as applied allocation/effective capacity unless accepted downstream allocation/mutation truth proves application.

## 6. Direct Application projection scope

WP-08 shall provide an exact per-Application projection-scoping operation that can return only records for one specified Application identity.

This is a **data-isolation rule**, not runtime authentication, runtime admission, session authorization or Application hosting.

WP-08 SHALL NOT claim to authenticate a live caller. Runtime identity establishment/admission/hosting remains governed by its separately planned Foundation stages.

Within the projection model:

- records from another Application cannot appear in a direct Application-scoped projection;
- cross-Application substitution fails closed;
- wildcard Application projection is forbidden;
- Application identity does not create resource authority.

## 7. Aggregate coordinator projection scope

A generic aggregate-coordinator projection may be produced only from an **already accepted exact WP-07 coordination-envelope / coordinator scope** and its exact constituent Application set.

WP-08 does not establish runtime coordinator authentication. It validates projection inputs against accepted predecessor coordinator/envelope identity material.

Requirements:

- exact coordinator instance/role/scope identity;
- exact accepted envelope/fence generation/token material where required by WP-07;
- exact constituent Application set;
- canonical collection ordering;
- each constituent remains an individually attributable per-Application projection;
- source grant and borrowed-capacity provenance remain visible and reconstructable;
- no merged resource pool is emitted;
- no new request, mutation or redistribution authority is minted.

## 8. Technical Load-Shedding Signal

WP-08 shall introduce a generic deterministic signal record bound to one exact Application/resource projection.

Signal material shall include, where applicable:

- exact Application/resource identity;
- exact projection identity;
- signal classification;
- accepted current effective capacity;
- observed used capacity where available;
- target compliant capacity and/or required reduction quantity when safely derivable;
- exact source pressure/enforcement/effective-capacity evidence identities;
- inherited correlation/causation only when an exact predecessor relationship exists;
- generation time;
- deterministic identity.

## 9. Signal classes and authority semantics

The implementation may use equivalent generic names, but shall preserve these four semantic states:

### 9.1 `NoAction`

No accepted predecessor condition currently supports a technical reduction recommendation or requirement.

### 9.2 `AdvisoryReduction`

Accepted pressure truth supports a technical consumption-reduction recommendation, but there is no accepted predecessor capacity/enforcement state making a specific reduction binding.

An advisory signal creates **no authority**.

### 9.3 `ComplianceReductionRequired`

This state is permitted only when an exact accepted predecessor already establishes the lower technical capacity/compliance boundary, specifically:

- accepted WP-07 authoritative/effective post-mutation state; or
- an exact accepted WP-05 enforcement condition whose semantics require compliance and whose resource scope is exact.

The WP-08 signal merely projects that accepted requirement.

If current observed use exceeds the exact accepted compliant capacity, required reduction quantity may be mathematically derived as:

`REQUIRED_REDUCTION = MAX(OBSERVED_USE - ACCEPTED_COMPLIANT_CAPACITY, 0)`

The quantity is not caller supplied and the signal itself does not mint the underlying authority.

### 9.4 `StateUnavailable`

If truth required for a safe signal is unavailable, stale, epoch-inconsistent, contradictory or cannot be tied to exact accepted predecessor evidence, WP-08 shall fail closed to unavailable rather than fabricate `NoAction`, advisory or compliance-required state.

## 10. Pressure and effective-capacity semantics

Pressure may support advisory signaling but does not by itself create mutation authority.

Accepted effective-capacity truth may reflect WP-07 delegated redistribution while authoritative grant ownership remains unchanged. WP-08 shall project both correctly:

- authoritative grant/allocation ownership remains the WP-03/WP-07 authoritative state;
- effective capacity may differ due to accepted WP-07 delegated movement;
- borrowed segments retain source Application + source Grant provenance;
- quota/ceiling headroom is never counted as granted/effective capacity.

## 11. Application-internal shedding remains outside Foundation

WP-08 may tell an Application generic technical facts/bounds, such as:

- authoritative allocation/quota/ceiling;
- currently accepted effective capacity;
- observed use and pressure state;
- exact applicable request/decision context;
- accepted reduced/restored capacity state;
- a generic required/recommended reduction quantity or compliant target.

WP-08 SHALL NOT choose or encode which internal Application workload is shed first.

Foundation public surfaces shall not encode Application business entities such as strategies, brokers, markets, providers, trading flows, simulations, Guardian business actions, MSA/LSA/CSA internals or other Application-specific workload ordering.

## 12. Restoration projection

Restoration shall be projected only from accepted predecessor state.

WP-08 shall not infer restoration eligibility or restored capacity merely because pressure improves.

Accepted Restore state must remain bounded by WP-07 captured authoritative restoration basis and accepted applied-effect/post-mutation truth.

## 13. Determinism and reconstructability

Every projection/signal identity shall include all semantically material fields and predecessor identities.

Canonical ordering is required for constituent projections and provenance collections.

Semantically material changes shall alter identity, including changes to:

- Application/resource/epoch;
- allocation predecessor;
- grant/allocation/quota/ceiling;
- accepted effective capacity;
- borrowed provenance;
- pressure/enforcement state;
- exact applicable request/decision reference;
- accepted mutation/effect reference;
- signal class;
- target/reduction quantity;
- generation/evidence time where part of the accepted record.

## 14. Fail-closed rules

WP-08 shall fail closed for at least:

- unknown Application/resource identity;
- unavailable required allocation truth;
- cross-epoch predecessor mismatch;
- mismatched pressure scope;
- mismatched exact WP-06 decision reference;
- unaccepted/failed/partial mutation effect presented as accepted state;
- WP-07 effective state inconsistent with its authoritative predecessor;
- aggregate constituent-set mismatch;
- stale/conflicting accepted coordinator-envelope/fence material;
- cross-Application projection substitution;
- duplicate ambiguous records;
- quantity unit mismatch;
- required reduction not derivable from exact accepted predecessor state.

When pressure is optional for a pure allocation-state projection, unavailable pressure may be represented explicitly as unavailable rather than invalidating unrelated authoritative allocation truth. A signal that requires pressure shall fail closed if that pressure truth is unavailable.

## 15. Zero-Application validity

Zero Applications is valid. WP-08 shall allow an empty projection set and shall not fabricate placeholder Applications.

## 16. Expected implementation placement

Subject to mandatory pre-implementation file-level reconciliation:

- Foundation-owned production logic under `src/Foundation.State/` and the existing generic resource-governance surface;
- dedicated `verification/Falcon.Stage6.WP08.Verifier/`;
- controlled solution integration only;
- no `applications/**` or `reference/**` writes;
- Foundation Contracts modification only if exact reconciliation proves a missing generic primitive cannot be represented safely by existing accepted primitives.

## 17. Mandatory verifier coverage

### 17.1 Projection truth

- direct Application positive projection;
- exact allocation/quota/ceiling binding;
- exact effective-capacity projection;
- borrowed provenance preservation;
- pressure available/unavailable representation;
- enforcement-state projection;
- exact applicable WP-06 decision-reference validation;
- request decision not treated as applied capacity;
- accepted WP-07 mutation/effect state projection;
- restoration only from accepted Restore truth;
- deterministic identity and canonical ordering;
- zero-Application validity.

### 17.2 Isolation and aggregate projection

- direct projection contains only exact Application;
- cross-Application substitution rejected;
- wildcard projection absent;
- exact WP-07 coordinator/envelope constituent set;
- stale/conflicting envelope/fence state rejected;
- every aggregate constituent remains individually attributable;
- no opaque aggregate pool.

### 17.3 Signals

- NoAction path;
- AdvisoryReduction path;
- ComplianceReductionRequired path;
- StateUnavailable path;
- advisory pressure does not mint authority;
- compliance-required signal has exact accepted predecessor authority/effective-state basis;
- required reduction mathematically derived from observed use and accepted compliant capacity;
- no caller-supplied arbitrary required quantity;
- headroom not treated as capacity;
- failed/partial effect does not create accepted reduction signal;
- pressure recovery alone does not create Restore signal/state.

### 17.4 Architecture boundaries

- projection scope does not claim runtime authentication/admission;
- no Application-internal shedding executor;
- no FSATS/TARC/FSARM hard binding in generic Foundation surface;
- no WP-09 integration/hardening executor;
- no environment-specific architecture meaning;
- no external-access/credential/financial authority;
- WP-01 through WP-07 regressions remain clean.

## 18. Acceptance gates

WP-08 requires, in order:

1. final planning Red-Team PASS;
2. explicit Owner planning acceptance;
3. separate explicit Owner implementation authorization;
4. pre-implementation file-level reconciliation/Red-Team;
5. implementation and implementation self-review;
6. post-implementation static Red-Team;
7. exact-commit executable validation: Restore, Release Build, Architecture, Security, WP-01 through WP-07 predecessor verifiers, WP-08 verifier twice from the same Release outputs, final clean/exact-HEAD integrity;
8. post-executable Red-Team/reconciliation;
9. Application implementation-compatibility verification for FCR-0010/FCR-0031 as applicable;
10. explicit Owner final closure.

## 19. Explicit non-authorities

Neither this planning artifact nor later technical success grants:

- WP-09/WP-10 implementation authority;
- runtime Application admission/hosting authority;
- Application business-policy authority;
- production/deployment authority;
- external access/credentials;
- broker, market-data, trading or financial authority;
- authority to reopen WP-01 through WP-07 without explicit closure-defect evidence.

## 20. Planning disposition

`WP08_PLANNING = PROPOSED_v0.2_FINAL_CANDIDATE`

`WP08_IMPLEMENTATION_AUTHORITY = NOT_GRANTED`

`WP09_IMPLEMENTATION_AUTHORITY = NOT_GRANTED`

This artifact is ready for final Red-Team and Owner review only.
