# Stage 7 — Existing Capability Reconciliation v0.1 DRAFT

Date: 2026-08-11

Status: `DRAFT / OWNER REVIEW NOT YET REQUESTED`

Stage: `Stage 7 — Foundation Health, Self-Awareness and Technical Fitness`

Authority state:

`STAGE7_PLANNING_AND_DESIGN = AUTHORIZED`

`STAGE7_IMPLEMENTATION_AUTHORITY = NOT_GRANTED`

`STAGE8_AUTHORITY = NOT_GRANTED`

## 1. Purpose

This document executes the mandatory Stage 7 entry gate:

`EXISTING_CAPABILITY_RECONCILIATION`

Its purpose is to determine, source-first, which Stage 7 responsibilities already exist in accepted Foundation specifications/contracts/capabilities, which are partial, which are missing, and which planned Specification subjects require definition and activation before any implementation may be authorized.

This document is planning only. It does not implement Stage 7, activate any Specification, modify accepted predecessor semantics, authorize Stage 8, or create Application/business authority.

## 2. Governing authority and stop rules

Current Stage 7 Owner authority permits:

- fresh source-first reconciliation;
- census of relevant specifications, contracts, ADRs, governance, code/tests/verifiers and predecessor capabilities;
- classification of existing/partial/missing behavior;
- definition of required Specification gates;
- draft architecture/plan/WP decomposition;
- Architecture/Consistency and Red-Team review;
- presentation to the Owner before implementation.

It explicitly does not permit Stage 7 production/source implementation.

IMP-001 v1.3 defines Stage 7 purpose as completing the Foundation-only Health, Self-Awareness and technical Fitness family using accepted authority, lifecycle, communication, evidence and resource truth.

IMP-001 requires:

1. `EXISTING_CAPABILITY_RECONCILIATION` first; and
2. `SPECIFICATION_DEFINITION_REVIEW_ACTIVATION_GATE` for planned future Specification subjects lacking effective bodies before implementation of their missing behavior.

## 3. Foundation invariants preserved

The Stage 7 plan SHALL preserve at minimum:

- Foundation remains valid with zero Applications;
- Applications remain Plug-and-Play consumers rather than Foundation prerequisites;
- Health, Self-Awareness and Fitness do not create authority;
- unknown or stale evidence does not become healthy or fit;
- Foundation Self-Awareness does not own Application business meaning;
- Health Monitoring does not replace Guardian, Lifecycle, Recovery or Authority;
- Fitness informs authority evaluation but does not grant permission;
- Stage 7 does not borrow Stage 8 protective-enforcement authority;
- no external connectivity, deployment, trading, broker, market-data or financial authority is created.

## 4. Current effective Stage 7 normative basis

### 4.1 AWR-001 v2.1 — Foundation Self-Awareness System

State: `EXISTING / EFFECTIVE / APPROVED`

Canonical path:

`docs/specifications/core/AWR-001_SELF_AWARENESS_SYSTEM.md`

AWR-001 already defines the Foundation-only Self Model and requires awareness of:

- Foundation identity and admitted baseline;
- component identity/version/lifecycle/integrity;
- runtime/infrastructure condition;
- Service Bus/FIL technical condition;
- dependency availability/compatibility/criticality;
- resource capacity/pressure/exhaustion risk;
- persistence/backup/restore/corruption condition;
- documentation/configuration integrity;
- technical security/authority condition;
- incidents/faults/contradictions/blind spots;
- isolation/recovery readiness;
- active restrictions;
- Foundation Technical Fitness;
- conformance cases;
- provenance/freshness/confidence/uncertainty;
- history and supersession.

AWR-001 also defines the technical Fitness state family:

`FIT`

`FIT_WITH_CONSTRAINTS`

`DEGRADED`

`UNKNOWN`

`UNAVAILABLE`

`INTEGRITY_FAILURE`

`ISOLATION_REQUIRED`

`RECOVERY_REQUIRED`

`NOT_FIT`

Important boundary: AWR-001 explicitly does not replace Health Monitoring, Guardian, Authority Engine, Security, Recovery or Lifecycle and does not grant authority.

Classification:

`STAGE7_SELF_AWARENESS_NORMATIVE_CORE = EXISTS`

### 4.2 SYS-008 v1.0 — Health Monitoring

State: `EXISTING / EFFECTIVE / APPROVED`

Canonical path:

`docs/specifications/core/SYS-008_HEALTH_MONITORING.md`

SYS-008 already defines:

- canonical health states;
- evidence-derived assessment;
- freshness/confidence handling;
- dependency/aggregate health;
- contradiction visibility;
- monitoring-loss visibility;
- health event publication;
- Guardian evidence availability;
- independent recovery acceptance separation.

Canonical minimum health states:

`HEALTHY`

`DEGRADED`

`UNHEALTHY`

`UNKNOWN`

`NOT_APPLICABLE`

Classification:

`STAGE7_HEALTH_NORMATIVE_CORE = EXISTS`

### 4.3 CON-006 v1.1 — Health and Fitness Contract

State: `EXISTING / EFFECTIVE / APPROVED`

Canonical path:

`docs/contracts/CON-006_HEALTH_AND_FITNESS.md`

CON-006 already separates health from Fitness and defines observable assessment fields, health states, fitness results and the mapping from AWR-001 technical states into contract results.

Contract fitness results:

`FIT`

`RESTRICTED`

`NOT_FIT`

Key preserved rule:

`HEALTHY != UNIVERSALLY_FIT`

and:

`FITNESS != AUTHORITY`

Classification:

`STAGE7_HEALTH_FITNESS_CONTRACT = EXISTS`

## 5. Existing accepted predecessor substrate

Stage 7 SHALL reuse accepted predecessor capabilities rather than duplicate them.

### Lifecycle substrate

`SYS-002 — Lifecycle` is Approved and already governs authoritative component lifecycle state, restriction/suspension/isolation states, dependency-aware transitions, recovery coordination and immutable transition facts.

Stage 7 may consume lifecycle truth. It must not create a second Lifecycle authority.

### Authority substrate

`AUT-001 — Authority Engine` is Approved and already owns permission evaluation, fail-closed authority, delegation/revocation and operating-condition constraints.

Stage 7 may supply scoped Fitness evidence. It must not grant permission or create a second authority engine.

### Recovery substrate

`OPS-003 — Recovery` is Approved and already defines containment, assessment, authorized plan, restoration, validation, controlled reintroduction and closure.

Stage 7 may assess recovery readiness/fitness and expose evidence. It must not declare recovery complete independently.

### Logging/evidence substrate

`OPS-004 — Logging` is Approved and already governs attributable operational records, evidence integrity, correlation, logging failure visibility and reconstruction support.

Stage 7 shall consume/publish governed evidence without treating logs as automatically authoritative state.

### Stage 5 communication substrate

Stage 5 is accepted and closed. Stage 7 shall use the accepted FIL/Service Bus/Event System and message/event evidence boundaries rather than create a new transport system.

### Stage 6 resource substrate

Stage 6 is accepted and closed. Stage 7 shall consume accepted Foundation resource truth, pressure, Application allocation/isolation and load-shedding/resource-state evidence as technical awareness inputs where applicable.

Stage 7 shall not become a resource allocator or mutate Stage 6 ownership.

## 6. Planned Stage 7 Specification subjects requiring definition gates

SPEC-000 v1.5 registers the following Self-Awareness Specifications as planned and `NOT YET EFFECTIVE`:

- `AWR-002 — Fitness to Operate`
- `AWR-003 — Confidence and Uncertainty`
- `AWR-004 — Temporal Awareness`
- `AWR-005 — Drift and Blind-Spot Detection`

Fresh canonical-path checks found that the registered files do not currently exist at:

- `docs/specifications/self-awareness/AWR-002_FITNESS_TO_OPERATE.md`
- `docs/specifications/self-awareness/AWR-003_CONFIDENCE_AND_UNCERTAINTY.md`
- `docs/specifications/self-awareness/AWR-004_TEMPORAL_AWARENESS.md`
- `docs/specifications/self-awareness/AWR-005_DRIFT_AND_BLIND_SPOT_DETECTION.md`

Therefore:

`AWR002_BODY = MISSING`

`AWR003_BODY = MISSING`

`AWR004_BODY = MISSING`

`AWR005_BODY = MISSING`

and each is subject to:

`SPECIFICATION_DEFINITION_REVIEW_ACTIVATION_GATE`

before any missing behavior assigned to it may be implemented.

The existence of requirements in AWR-001 does not authorize silently inventing the specialized AWR-002..005 bodies. Their exact responsibilities must be reconciled against AWR-001, SYS-008, CON-006 and accepted predecessor ownership to avoid duplication or contradiction.

## 7. Controlled-project / implementation-surface census

The current controlled Foundation solution contains accepted production projects through Stage 6 and verifier projects through Stage 6, including the Stage 6 Cross-Stage verifier.

No Stage 7-specific verifier project is currently registered in the controlled solution.

No dedicated project named for Health Monitoring, Foundation Self-Awareness, or Fitness appears as a separate controlled project surface.

This is not sufficient by itself to prove that zero relevant implementation exists inside generic accepted projects. Therefore code implementation status is classified conservatively as:

`STAGE7_CODE_LEVEL_EXISTING_CAPABILITY_CENSUS = INCOMPLETE / MUST_BE_COMPLETED_BEFORE_IMPLEMENTATION_PLAN_FREEZE`

The planning phase must inspect generic accepted production projects and tests for reusable health/fitness/awareness primitives before declaring any implementation item `MISSING`.

## 8. Reconciliation matrix v0.1

| Capability family | Current classification | Existing authoritative basis | Planning consequence |
|---|---|---|---|
| Health state model | EXISTS | SYS-008 | Reuse; do not redesign casually |
| Health evidence/freshness/confidence rules | EXISTS | SYS-008 | Reuse and bind to implementation |
| Foundation Self Model | EXISTS NORMATIVELY | AWR-001 | Implementation census required |
| Foundation Technical Fitness state family | EXISTS NORMATIVELY | AWR-001 | Reconcile with CON-006 |
| Health/Fitness observable contract | EXISTS | CON-006 | Reuse unless a proven gap requires amendment |
| Fitness-to-authority separation | EXISTS | AWR-001 + CON-006 + AUT-001 | Preserve strictly |
| Lifecycle awareness source | EXISTS | SYS-002 | Consume authoritative truth |
| Authority-condition source | EXISTS | AUT-001 | Consume; no duplicate authority |
| Recovery readiness source | EXISTS | OPS-003 | Consume; no self-release |
| Evidence/logging source | EXISTS | OPS-004 + accepted evidence substrate | Consume/publish governed evidence |
| Communication/event publication | EXISTS | accepted Stage 5 | Reuse |
| Resource pressure/allocation awareness source | EXISTS | accepted Stage 6 | Reuse |
| Specialized Fitness specification | MISSING BODY / PLANNED | AWR-002 registry row | Definition gate required |
| Confidence/Uncertainty specification | MISSING BODY / PLANNED | AWR-003 registry row | Definition gate required |
| Temporal Awareness specification | MISSING BODY / PLANNED | AWR-004 registry row | Definition gate required |
| Drift/Blind-Spot specification | MISSING BODY / PLANNED | AWR-005 registry row | Definition gate required |
| Stage 7 dedicated executable verification | MISSING AS STAGE-SPECIFIC SURFACE | controlled solution census | Plan verifier architecture before implementation |
| Existing reusable code primitives | UNRESOLVED | generic current production projects may contain partial behavior | Complete code-level census |

## 9. Boundary with Stage 8

Stage 7 may determine health, awareness, uncertainty, technical fitness and the need for restriction/isolation/recovery as evidence-bearing states.

Stage 7 SHALL NOT implement the Stage 8 protective enforcement plane.

In particular:

- Stage 7 may emit evidence that a subject is `NOT_FIT`, `ISOLATION_REQUIRED`, or `RECOVERY_REQUIRED`;
- Stage 7 may make material fitness reduction available to existing Authority/Guardian boundaries;
- Stage 7 does not create Guardian command authority;
- Stage 7 does not define Platform Safe State enforcement;
- Stage 7 does not release protective restrictions;
- Stage 7 does not borrow Stage 8 implementation authority.

## 10. Boundary with Stage 13

Stage 7 implements/realizes Foundation technical self-awareness and fitness only after separate implementation authority.

Stage 13 remains the future FSA/Owner Governance and bounded self-maintenance/evolution control plane.

The expanded FSA governance/containment/monitoring requirements in FCR-0012 and exact MSA-to-FSA governed interface in FCR-0030 remain preserved for Stage 13 reconciliation and SHALL NOT be pulled backward into Stage 7 merely because Stage 7 contains Foundation Self-Awareness technical behavior.

Stage 7 must, however, avoid architecture that would make Stage 13 requirements impossible or force FSA to control its own independent governance/protection cage.

## 11. Draft Stage 7 Work Package decomposition

This decomposition is a planning candidate only. No WP implementation authority is created.

### WP-01 — Specification Definition and Stage 7 Canonical Model Reconciliation

Purpose:

- define/reconcile AWR-002, AWR-003, AWR-004 and AWR-005;
- prove non-duplication with AWR-001, SYS-008 and CON-006;
- resolve exact state vocabularies, ownership and contract mapping;
- complete `SPECIFICATION_DEFINITION_REVIEW_ACTIVATION_GATE` for each required missing body;
- freeze the Stage 7 normative implementation baseline before production code.

### WP-02 — Canonical Health Observation and Assessment

Purpose:

- realize SYS-008 evidence-derived health assessment;
- preserve freshness, confidence, contradiction and unknown behavior;
- aggregate dependency health without hiding critical failure;
- publish health changes through accepted event boundaries.

### WP-03 — Foundation Self Model and Evidence Correlation

Purpose:

- realize the AWR-001 Foundation Self Model over authoritative evidence sources;
- preserve source ownership rather than copying authority into FSA;
- correlate lifecycle, security, dependency, communication, persistence and resource truth;
- preserve current/last-known/expected/desired distinctions and reconstruction identity.

### WP-04 — Scoped Foundation Technical Fitness Evaluation

Purpose:

- realize technical Fitness evaluation for declared capability/action/admission scopes;
- bind AWR-001 technical states to CON-006 results;
- enforce expiry, evidence basis, constraints, confidence and reasons;
- make Fitness evidence available to AUT-001 without granting authority.

### WP-05 — Confidence, Uncertainty, Freshness and Temporal Awareness

Purpose:

- realize the activated AWR-003/AWR-004 responsibilities after their definition gate;
- prevent stale/missing evidence from producing optimistic assessment;
- preserve clock/evidence-time limitations and time-bounded Fitness;
- represent uncertainty without fabricated precision.

### WP-06 — Drift, Blind-Spot and Contradiction Detection

Purpose:

- realize activated AWR-005 responsibilities after its definition gate;
- detect material drift in Foundation state/configuration/authority/dependency/assessment inputs;
- represent blind spots explicitly;
- preserve contradictory evidence until governed reconciliation.

### WP-07 — Lifecycle, Dependency, Communication and Resource Awareness Integration

Purpose:

- integrate accepted Stage 3/4/5/6 authoritative truth into the Self Model;
- validate no duplicate source-of-truth ownership;
- preserve zero-Application operation and multi-Application isolation;
- ensure Application payload/business meaning remains outside Foundation Self-Awareness.

### WP-08 — Awareness History, Persistence and Reconstruction

Purpose:

- preserve awareness history and supersession;
- reconstruct the Self Model and Fitness basis used for material decisions;
- retain provenance/freshness/confidence/uncertainty evidence;
- make persistence loss/corruption explicit without inventing recovery success.

### WP-09 — Degraded Awareness, Fitness Reduction and Protective Handoff Boundary

Purpose:

- implement Stage 7-side degraded/unknown awareness semantics;
- ensure loss/staleness/contradiction reduces Fitness rather than producing false reassurance;
- expose lawful evidence to existing Authority/Guardian/Lifecycle boundaries;
- prove Stage 7 does not implement Stage 8 protective enforcement or release.

### WP-10 — Integrated Stage 7 Closure Verification

Purpose:

- verify WP-01 through WP-09 as one Stage 7 system;
- execute health/self-awareness/fitness positive and fail-closed scenarios;
- verify deterministic/reconstructable evidence;
- verify zero-Application validity and Application-neutrality;
- verify no Stage 8+, deployment, external or financial authority leakage;
- produce closure-readiness evidence only, not automatic Stage closure.

## 12. Required architecture decisions before implementation authorization

Before substantive Stage 7 implementation, the planning package must resolve at minimum:

1. authoritative ownership of health observations versus health assessments versus Self Model interpretations;
2. canonical identity/versioning of Health Assessment and Fitness Assessment records;
3. exact mapping between SYS-008 health states, AWR-001 technical fitness states and CON-006 contract results;
4. observation topology: push, pull or hybrid without creating duplicate authority;
5. health aggregation and critical dependency propagation rules;
6. confidence/evidence-quality representation and stale-evidence policy;
7. temporal policy and clock-quality dependency;
8. contradiction and blind-spot semantics;
9. Self Model persistence/history and reconstruction model;
10. event publication and evidence correlation through accepted Stage 5 boundaries;
11. consumption of Stage 6 resource pressure/truth without ownership duplication;
12. Fitness-to-AUT-001 integration without permission inflation;
13. Guardian handoff boundary without Stage 8 implementation leakage;
14. failure of Health Monitoring itself and failure of Self-Awareness itself;
15. exact Stage 7 verifier/evidence architecture.

## 13. Preliminary Red-Team targets for the planning package

The eventual Stage 7 plan must be challenged against at least:

- missing evidence falsely producing `HEALTHY` or `FIT`;
- stale evidence remaining valid indefinitely;
- contradictory evidence being silently resolved optimistically;
- Self-Awareness replacing authoritative source systems;
- FSA taking Application business/domain ownership;
- Health Monitoring granting authority;
- Fitness granting authority;
- one Application contaminating another Application's awareness/resource state;
- zero-Application state failing;
- resource pressure being misinterpreted as financial/business priority;
- self-assessment relying only on the assessed subject where independent evidence is required;
- monitoring failure being hidden;
- FSA failure being treated as healthy continuation;
- drift/blind spots being omitted;
- Fitness result surviving source evidence expiry;
- recovery readiness being misrepresented as recovery completion;
- Stage 7 implementing Stage 8 protective enforcement;
- Stage 7 pulling Stage 13 governance/Owner-control work backward;
- Stage 7 creating external-connectivity or financial authority.

## 14. Current findings

### Critical

`NONE IDENTIFIED AT RECONCILIATION v0.1`

### High

**H-01 — AWR-002..AWR-005 registered subjects have no canonical bodies.**

Impact: implementation of their missing specialized behavior is blocked by `SPECIFICATION_DEFINITION_REVIEW_ACTIVATION_GATE`.

Disposition: Stage 7 planning shall define/reconcile them under WP-01 before implementation authorization.

### Medium

**M-01 — Code-level existing-capability census is not yet complete.**

Impact: planning must not label generic production behavior missing until existing source/tests are inspected in detail.

Disposition: complete code/test census before final Stage 7 plan freeze.

**M-02 — SYS-008 contains unresolved health consequence/freshness details.**

Impact: Stage 7 cannot invent thresholds ad hoc in implementation.

Disposition: resolve through Stage 7 planning/specification definition and applicable ADR/design decision.

**M-03 — OPS-003 retains unresolved recovery-objective and independent-validation authority details.**

Impact: Stage 7 must avoid implying recovery/release semantics while defining recovery readiness awareness.

Disposition: preserve Stage 9 ownership for full recovery/release completion and keep Stage 7 limited to assessment/handoff semantics.

## 15. Reconciliation verdict

`STAGE7_EXISTING_CAPABILITY_RECONCILIATION_v0.1 = PARTIAL / PLANNING_CONTINUES`

`EFFECTIVE_HEALTH_SPEC = EXISTS`

`EFFECTIVE_FOUNDATION_SELF_AWARENESS_SPEC = EXISTS`

`EFFECTIVE_HEALTH_FITNESS_CONTRACT = EXISTS`

`AWR002_TO_AWR005_DEFINITION_GATE = REQUIRED`

`CODE_LEVEL_REUSE_CENSUS = REQUIRED_BEFORE_PLAN_FREEZE`

`STAGE7_IMPLEMENTATION_AUTHORITY = NOT_GRANTED`

`STAGE8_AUTHORITY = NOT_GRANTED`

## 16. Next planning actions

Under current Planning & Design authority, Foundation shall next:

1. complete the source/test/verifier census for reusable Stage 7 primitives inside existing controlled projects;
2. draft the missing AWR-002..AWR-005 bodies as proposed specifications, explicitly reconciling them against AWR-001/SYS-008/CON-006;
3. produce the Stage 7 architecture and consistency matrix;
4. refine/freeze the proposed WP structure;
5. perform Architecture/Consistency review;
6. perform fresh Red-Team review;
7. present the complete Stage 7 plan and any exact unresolved Owner decisions for separate Owner acceptance and implementation authorization.

No production implementation begins before that stop point.