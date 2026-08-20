# Stage 7 — Foundation Health, Self-Awareness and Technical Fitness

Implementation Plan: v0.1 CANDIDATE
Status: PROPOSED / NOT OWNER ACCEPTED / IMPLEMENTATION NOT AUTHORIZED
Date: 2026-08-11

## 1. Purpose

Stage 7 completes the runtime implementation and integration required by the currently effective Foundation Health, Foundation Self-Awareness and Technical Fitness baseline without redesigning accepted predecessor capabilities or importing future-Stage authority.

Controlling Stage 7 sources:

- `SYS-008 — Health Monitoring`;
- `AWR-001 v2.1 — Foundation Self-Awareness System`;
- `CON-006 v1.1 — Health and Fitness Contract`;
- `VPL-005 v1.1 — Health Evidence Loss Plan`;
- IMP-001 v1.3;
- TRC-001 v1.4;
- accepted Stage 0..6 baseline.

## 2. Current authority

`STAGE6 = ACCEPTED_AND_CLOSED`

`STAGE7_PLANNING_AND_DESIGN = AUTHORIZED`

`STAGE7_IMPLEMENTATION = NOT_AUTHORIZED`

This plan is a candidate only. No production code may be written from it until separate Owner acceptance and implementation authority are recorded.

## 3. Mandatory pre-implementation Gate 0

Before WP-01 source implementation begins, Foundation shall perform an exact live-branch code-level reuse census for the Stage 7 scope.

Gate 0 shall identify every existing reusable runtime type/project/API related to:

- health observations/assessments;
- evidence freshness/confidence;
- Foundation Self Model projections;
- technical fitness;
- health/fitness event identities;
- health/fitness persistence/history;
- authority/lifecycle consumption of fitness evidence.

Disposition for each discovered item:

- `REUSE_AS_IS`;
- `REUSE_WITH_BOUNDED_EXTENSION`;
- `NOT_APPLICABLE`;
- `GENUINELY_MISSING`.

No duplicate production type/project may be created while an accepted equivalent exists.

If Gate 0 discovers that required runtime behavior depends on normative meaning absent from the current effective SYS-008/AWR-001/CON-006/VPL-005 sources, implementation shall stop and the applicable `SPECIFICATION_DEFINITION_REVIEW_ACTIVATION_GATE` shall be performed before that behavior is implemented.

## 4. Stage 7 architecture boundary

The intended responsibility chain is:

```text
Authoritative observations / accepted predecessor truth
                    ↓
            Health Monitoring
                    ↓
          Health Assessment
                    ↓
        Foundation Self Model
                    ↓
     Technical Fitness Evaluation
                    ↓
        CON-006 projection
                    ↓
      FIT / RESTRICTED / NOT_FIT
                    ↓
    Authority / governed consumers
```

Hard boundaries:

- Health does not grant authority.
- Fitness does not grant authority.
- FSA does not own Lifecycle.
- FSA does not own Guardian.
- FSA does not own Recovery.
- FSA does not own Stage 6 resource truth.
- FSA does not interpret Application business meaning.
- Stage 7 does not implement MSA/LSA/CSA internals.
- Stage 7 does not implement Stage 13 FSA/Owner governance.

## 5. Proposed Work Package sequence

### WP-01 — Canonical Health and Fitness Runtime Primitives

Purpose:

Implement or reuse the minimum canonical runtime representations required by SYS-008 and CON-006.

Required outcomes:

- canonical health-state representation;
- health assessment identity and fields;
- canonical technical-fitness state representation aligned to AWR-001;
- CON-006 `FIT / RESTRICTED / NOT_FIT` projection representation;
- explicit subject/capability/scope identity;
- evidence references;
- assessment/evidence/effective/expiry time semantics using accepted time primitives;
- confidence/unknown/constraint/reason fields;
- immutable or value-semantic assessment behavior where applicable;
- strict enum/identifier validation;
- no authority-grant surface.

Negative requirements:

- unknown health/fitness values rejected;
- malformed identity rejected;
- impossible expiry/effective ordering rejected;
- missing required evidence references cannot become positive fitness;
- no Application business fields added.

### WP-02 — Health Observation and Assessment Runtime

Purpose:

Implement SYS-008 Health Monitoring runtime behavior over governed observations.

Required outcomes:

- attributable observations;
- evidence-time and assessment-time separation;
- freshness evaluation;
- health rule identity/version;
- availability/correctness/integrity/performance/dependency dimensions where declared;
- deterministic health-state evaluation;
- aggregate health preserving critical unhealthy dependencies;
- contradictory signal handling;
- monitoring-loss visibility;
- no repair/lifecycle/authority command behavior.

Mandatory fail-closed cases:

- missing required observation -> not `HEALTHY`;
- stale required observation -> not silently `HEALTHY`;
- contradictory evidence -> explicit uncertainty;
- failed monitor source -> explicit loss of visibility.

### WP-03 — Foundation Self Model Runtime

Purpose:

Implement AWR-001 Foundation Self Model as a governed projection of authoritative evidence, not a competing authoritative source.

Required outcomes:

- Foundation identity/baseline awareness;
- component/lifecycle/integrity awareness;
- dependency awareness;
- resource-capacity/pressure awareness;
- communication/FIL technical awareness;
- persistence/configuration/security/authority awareness;
- faults/contradictions/blind spots;
- current vs last-known vs expected vs desired state distinction;
- evidence provenance/freshness/confidence/uncertainty;
- historical assessment identity and reconstruction basis;
- zero-Application validity.

Non-scope:

- Application business state interpretation;
- direct mutation of predecessor authoritative state;
- Guardian/recovery authority.

### WP-04 — Technical Fitness Evaluation and CON-006 Projection

Purpose:

Implement deterministic scoped technical Fitness evaluation under AWR-001 and exact CON-006 projection.

Required outcomes:

AWR-001 technical states:

- `FIT`;
- `FIT_WITH_CONSTRAINTS`;
- `DEGRADED`;
- `UNKNOWN`;
- `UNAVAILABLE`;
- `INTEGRITY_FAILURE`;
- `ISOLATION_REQUIRED`;
- `RECOVERY_REQUIRED`;
- `NOT_FIT`.

Exact projection to CON-006:

- `FIT -> FIT`;
- `FIT_WITH_CONSTRAINTS -> RESTRICTED`;
- `DEGRADED -> RESTRICTED`;
- `UNKNOWN -> NOT_FIT`;
- `UNAVAILABLE -> NOT_FIT`;
- `INTEGRITY_FAILURE -> NOT_FIT`;
- `ISOLATION_REQUIRED -> RESTRICTED`;
- `RECOVERY_REQUIRED -> RESTRICTED or NOT_FIT` only under an explicit consequence-governed policy;
- `NOT_FIT -> NOT_FIT`.

Every result must bind scope, capability, evidence, confidence, constraints, effective time, expiry and reason.

Fitness shall not issue permission.

### WP-05 — Evidence Quality, Freshness, Contradiction and Last-Known-State Control

Purpose:

Implement the evidence-quality behaviors required by AWR-001, SYS-008, CON-006 and VPL-005 without inventing AWR-002..AWR-005 semantics.

Required evidence-loss classes:

- `MISSING`;
- `STALE`;
- `DELAYED`;
- `CONTRADICTORY`;
- `UNVERIFIABLE`;
- `INACCESSIBLE`;
- `CORRUPTED`;
- `PROVENANCE_FAILURE`;
- `PARTIAL_VISIBILITY`.

Required outcomes:

- explicit evidence age;
- explicit confidence degradation;
- contradiction preservation;
- provenance validation;
- last-known trustworthy state tagged as last-known, never current;
- explicit freshness/expiry enforcement;
- no optimistic fallback;
- no silent restoration when a source merely reappears.

### WP-06 — Accepted Predecessor Truth Integration

Purpose:

Bind Stage 7 to accepted Foundation truth owners without duplicating them.

Required integrations:

- Stage 3 dependency/configuration truth;
- Stage 4 Authority/Lifecycle/state/evidence/reconciliation truth;
- Stage 5 message/event/contract/protection truth;
- Stage 6 resource truth/pressure/isolation/load-shedding truth;
- current security/trust identities;
- logging/persistence evidence.

Required properties:

- exact source attribution;
- source state remains authoritative at its owner;
- Stage 7 projections cannot mutate predecessor truth;
- stale/replayed predecessor evidence cannot silently become current awareness;
- unavailable predecessor truth reduces affected health/fitness rather than being fabricated.

### WP-07 — Health/Fitness Events, Persistence and Reconstruction

Purpose:

Make material Stage 7 assessments observable and reconstructable using accepted Stage 5/Stage 4/SYS-011/OPS-004 substrates.

Required outcomes:

- governed health-state change events;
- governed material fitness-change events;
- exact event owner/type/schema/version/provenance;
- correction-by-new-event rather than history mutation;
- replay distinguishability;
- durable assessment history where required;
- reconstruction of the Self Model/fitness basis used for a material decision;
- logging/evidence failure visible as degraded evidence quality;
- bounded persistence behavior with no new general persistence engine.

### WP-08 — Authority and Protective-Consumer Boundary

Purpose:

Expose Stage 7 outputs to accepted governed consumers while preserving separation of powers.

Required outcomes:

- AUT-001 can consume scoped CON-006 fitness evidence as a condition/input;
- material reductions are available to Authority Engine;
- material health/fitness reduction is publishable for future/current Guardian consumption through existing governed boundaries;
- Lifecycle may consume qualified health/fitness evidence where governing rules require it;
- loss of required self-awareness/fitness evidence prevents positive authority inference;
- restored health evidence does not automatically restore authority;
- Stage 7 does not issue lifecycle commands, Guardian commands or recovery release.

Stage 8 implementation is not part of this WP.

### WP-09 — VPL-005 Executable Evidence-Loss Validation and Stage 7 Hardening

Purpose:

Convert the active VPL-005 documentary plan into exact executable Stage 7 validation evidence and challenge integration failures.

Required scenarios include:

- fresh valid evidence baseline;
- missing evidence;
- stale evidence;
- delayed evidence;
- contradictory evidence;
- unverifiable evidence;
- inaccessible evidence;
- corrupted evidence;
- provenance failure;
- partial visibility;
- stale cached success cannot override current unknown;
- evidence-source restoration alone does not restore authority;
- independent reassessment required before positive restoration;
- unaffected capability remains isolated where evidence independence is valid;
- no Application business semantics;
- zero-Application Foundation remains valid.

Required end-to-end path:

```text
raw observation change
-> health assessment
-> Self Model change
-> technical fitness change
-> CON-006 result
-> Authority consumption/denial or restriction evidence
-> attributable history/reconstruction
```

### WP-10 — Integrated Stage 7 Closure Verification

Purpose:

Prove Stage 7 as one coherent Foundation capability family before any Stage-level closure decision.

Required verification:

- WP-01..WP-09 regression;
- SYS-008 requirement coverage;
- AWR-001 Stage 7 implementation-scope requirement coverage;
- CON-006 requirement coverage;
- VPL-005 end-to-end PASS;
- deterministic identical-input assessment behavior;
- exact current-candidate build/test identity;
- Foundation Architecture PASS;
- Foundation Security PASS;
- accepted predecessor regression preservation;
- zero-Application validity;
- multi-Application technical isolation where Application identities appear only as governed subjects;
- no Application business interpretation;
- no Stage 8/9/13 authority surface;
- no deployment/external/financial authority claim;
- fresh post-executable Red-Team;
- final Stage 7 closure-readiness report;
- separate Owner Stage 7 closure decision.

## 6. Verification philosophy

Every WP shall include both positive behavior and fail-closed/mutation behavior.

No technical PASS shall automatically close a WP or Stage.

For executable validation:

- exact candidate shall be frozen;
- one controlled Release build shall be used for the run set;
- verifier identities shall be hashed where applicable;
- generated evidence shall remain outside the repository worktree unless the governed evidence package explicitly requires a canonical documentary record;
- final exact HEAD/worktree/remote identity shall be checked;
- deterministic reruns shall be used for material Stage 7 decision/evidence identities.

## 7. Specification-definition gate rule

AWR-002..AWR-005 remain planned subjects only.

If a WP needs behavior that current effective AWR-001/SYS-008/CON-006/VPL-005 cannot govern without inventing normative meaning:

`STOP -> SPECIFICATION_DEFINITION_REVIEW_ACTIVATION_GATE -> OWNER/GOVERNANCE DECISION -> PLAN RECONCILIATION`

No missing specification may be silently written into code.

## 8. Architecture review requirements

Before implementation authorization, the plan must prove:

- no duplicate Health authority;
- no duplicate Fitness authority;
- no duplicate Authority Engine;
- no duplicate Lifecycle;
- no duplicate event/persistence/resource engine;
- correct FSA vs MSA/LSA/CSA ownership;
- correct Stage 7 vs Stage 8/9/13 boundary;
- zero-Application validity;
- Application business neutrality;
- failure of awareness decreases authority rather than increasing it.

## 9. Red-Team requirements

The plan shall be challenged against at least:

- missing evidence reported healthy;
- stale evidence reported current;
- contradictory evidence silently collapsed;
- monitor/self-report accepted without required independent evidence;
- Foundation Self Model replacing authoritative predecessor state;
- fitness creating permission;
- health creating permission;
- expired fitness reused;
- stale last-known state reused after expiry;
- evidence source returns and authority silently restores;
- dependency failure hidden by aggregate health;
- resource pressure ignored by fitness;
- Application business preference converted into Foundation fitness/technical criticality;
- Application A influencing Application B awareness state without governed evidence;
- FSA crossing into MSA/LSA/CSA business semantics;
- Stage 7 implementing Guardian/Safe State;
- Stage 7 implementing Recovery release;
- Stage 7 implementing Stage 13 FSA/Owner governance;
- replay recreating current health/fitness state incorrectly;
- corrupted persistence reconstructing a favorable Self Model;
- loss of logging/evidence hidden;
- zero-Application state treated as unhealthy by default.

## 10. Proposed authority sequence after Owner plan acceptance

Even if this plan is accepted, implementation should proceed WP-by-WP.

For each WP:

1. fresh FCR check;
2. fresh governing-source read;
3. exact WP scope/allowlist;
4. separate implementation authority;
5. implementation;
6. executable verification;
7. Architecture/Security checks as applicable;
8. fresh Red-Team after any Owner-directed modification;
9. Owner closure of that WP;
10. only then proceed to the next authorized WP.

Plan acceptance alone shall not authorize all WP implementation automatically unless the Owner explicitly grants that broader authority.

## 11. Current disposition

`STAGE7_PLAN_v0.1 = CANDIDATE`

`WORK_PACKAGES = WP01_THROUGH_WP10_PROPOSED`

`STAGE7_EXISTING_CAPABILITY_RECONCILIATION = PASS_FOR_PLANNING`

`GATE0_CODE_REUSE_CENSUS = REQUIRED_BEFORE_WP01_IMPLEMENTATION`

`STAGE7_PLAN_OWNER_ACCEPTANCE = NOT_YET`

`STAGE7_IMPLEMENTATION_AUTHORITY = NOT_GRANTED`

`STAGE8_AUTHORITY = NOT_GRANTED`