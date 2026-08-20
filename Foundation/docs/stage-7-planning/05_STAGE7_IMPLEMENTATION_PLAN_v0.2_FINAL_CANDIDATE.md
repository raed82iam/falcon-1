# Stage 7 — Foundation Health, Self-Awareness and Technical Fitness

Implementation Plan: v0.2 FINAL CANDIDATE
Status: PROPOSED / OWNER REVIEW REQUIRED / IMPLEMENTATION NOT AUTHORIZED
Date: 2026-08-11
Supersedes for review: `03_STAGE7_IMPLEMENTATION_PLAN_v0.1_CANDIDATE.md`

## 1. Purpose

Stage 7 completes the runtime implementation and integration required by the currently effective Foundation Health, Foundation Self-Awareness and Technical Fitness baseline without redesigning accepted predecessor capabilities or importing future-Stage authority.

Controlling sources:

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

`STAGE7_PLAN_OWNER_ACCEPTANCE = NOT_YET`

`STAGE7_IMPLEMENTATION = NOT_AUTHORIZED`

No production code may be written from this plan until separate Owner acceptance and implementation authority are recorded.

## 3. Mandatory pre-implementation gates

### Gate 0A — Code Reuse and Ownership Census

Before WP-01 implementation, perform an exact live-branch code-level census for existing runtime types/projects/APIs related to:

- health observations/assessments;
- evidence freshness/confidence;
- Foundation Self Model projections;
- technical fitness;
- health/fitness events;
- health/fitness persistence/history;
- authority/lifecycle consumption of fitness evidence.

Each item shall be classified:

- `REUSE_AS_IS`;
- `REUSE_WITH_BOUNDED_EXTENSION`;
- `NOT_APPLICABLE`;
- `GENUINELY_MISSING`.

No duplicate project/type shall be created while an accepted equivalent exists.

If a required change touches a production project created in a closed predecessor Stage, the exact change shall be classified before implementation as one of:

- compatible consumption/integration using existing public behavior;
- bounded additive extension preserving accepted semantics;
- true predecessor accepted-scope defect.

A true predecessor defect shall NOT be silently repaired under Stage 7. It requires explicit trace and separately governed remediation authority.

### Gate 0B — Health Rule Policy Definition

SYS-008 leaves health consequence classes and required freshness by Core component unresolved.

Before WP-02/WP-04 executable rule implementation relies on such values, Foundation shall determine whether approved configuration, catalog, policy, ADR or another effective source already defines them.

Every required health rule shall identify:

- subject/capability scope;
- evidence requirements;
- freshness requirement;
- confidence/evidence-quality rule;
- consequence/criticality treatment where applicable;
- rule identity/version;
- accountable owner;
- governing authority.

Missing design/policy choices shall be separately governed before code uses them.

Source code SHALL NOT invent health thresholds, consequence classes, freshness windows or `RECOVERY_REQUIRED -> RESTRICTED/NOT_FIT` policy.

If missing normative meaning cannot be derived from current effective sources, stop for:

`SPECIFICATION_DEFINITION_REVIEW_ACTIVATION_GATE`.

## 4. Architecture boundary

```text
Authoritative observations / accepted predecessor truth
                    ↓
            SYS-008 Health Monitoring
                    ↓
              Health Assessment
                    ↓
          AWR-001 Foundation Self Model
                    ↓
          Technical Fitness Evaluation
                    ↓
              CON-006 Projection
                    ↓
           FIT / RESTRICTED / NOT_FIT
                    ↓
             Governed Consumers
```

Hard invariants:

- Health does not grant authority.
- Fitness does not grant authority.
- FSA does not own Lifecycle.
- FSA does not own Guardian.
- FSA does not own Recovery.
- FSA does not own Stage 6 resource truth.
- Self Model projection does not replace authoritative source truth.
- FSA does not interpret Application business meaning.
- Stage 7 does not implement MSA/LSA/CSA internals.
- Stage 7 does not implement Stage 13 FSA/Owner governance.

## 5. Work Package sequence

### WP-01 — Canonical Health and Fitness Runtime Primitives

Purpose: implement or reuse the minimum canonical runtime representations required by SYS-008, AWR-001 and CON-006.

Required outcomes:

- health states: `HEALTHY`, `DEGRADED`, `UNHEALTHY`, `UNKNOWN`, `NOT_APPLICABLE`;
- AWR-001 technical-fitness states;
- CON-006 results: `FIT`, `RESTRICTED`, `NOT_FIT`;
- health assessment identity and required fields;
- fitness assessment identity and required fields;
- exact subject/capability/scope identity;
- evidence references/provenance;
- observation/assessment/effective/expiry time semantics using accepted time primitives;
- confidence/unknown/constraint/reason fields;
- strict enum/identifier validation;
- immutable/value-semantic assessment behavior where applicable;
- no authority-grant surface.

Fail-closed coverage:

- unknown enum values;
- malformed/noncanonical identifiers;
- impossible time ordering;
- missing required evidence;
- invalid fitness scope;
- Application-business fields or semantics rejected from Foundation contracts.

### WP-02 — Health Observation and Assessment Runtime

Purpose: implement SYS-008 Health Monitoring runtime behavior over governed observations.

Required outcomes:

- attributable observation identity;
- evidence time distinct from assessment time;
- governed freshness rule resolution from Gate 0B;
- rule identity/version;
- availability/correctness/integrity/performance/dependency dimensions where declared;
- deterministic health evaluation;
- aggregate health preserving critical unhealthy dependencies;
- explicit contradictions and uncertainty;
- monitoring failure/loss-of-visibility state;
- event-ready material state transitions;
- no repair/lifecycle/authority command behavior.

Fail-closed cases:

- missing required observation cannot yield `HEALTHY`;
- stale required observation cannot remain silently current/healthy;
- contradictory evidence cannot be optimistically selected;
- failed monitor source becomes explicit loss of knowledge.

### WP-03 — Foundation Self Model Runtime

Purpose: implement AWR-001 Self Model as a governed evidence projection, never a competing authoritative state owner.

Required awareness:

- Foundation identity/baseline;
- component identity/version/lifecycle/integrity;
- runtime/infrastructure condition;
- Service Bus/FIL technical condition;
- dependencies;
- Stage 6 resource capacity/pressure/exhaustion risk;
- persistence/backup/restore/corruption condition;
- documentation/configuration integrity;
- security/authority condition;
- incidents/faults/contradictions/blind spots;
- isolation/recovery readiness as observed state only;
- active restrictions;
- evidence identity/provenance/freshness/confidence/uncertainty;
- current vs last-known vs expected vs desired state;
- historical assessment lineage;
- zero-Application validity.

The Self Model shall preserve exact links to source truth and shall not mutate predecessor authoritative state.

### WP-04 — Technical Fitness Evaluation and CON-006 Projection

Purpose: implement deterministic scoped technical Fitness evaluation under AWR-001 and exact CON-006 projection.

AWR-001 states:

- `FIT`;
- `FIT_WITH_CONSTRAINTS`;
- `DEGRADED`;
- `UNKNOWN`;
- `UNAVAILABLE`;
- `INTEGRITY_FAILURE`;
- `ISOLATION_REQUIRED`;
- `RECOVERY_REQUIRED`;
- `NOT_FIT`.

Mandatory CON-006 mapping:

- `FIT -> FIT`;
- `FIT_WITH_CONSTRAINTS -> RESTRICTED`;
- `DEGRADED -> RESTRICTED`;
- `UNKNOWN -> NOT_FIT`;
- `UNAVAILABLE -> NOT_FIT`;
- `INTEGRITY_FAILURE -> NOT_FIT`;
- `ISOLATION_REQUIRED -> RESTRICTED`;
- `RECOVERY_REQUIRED -> RESTRICTED or NOT_FIT` only through an explicit governed consequence policy from Gate 0B;
- `NOT_FIT -> NOT_FIT`.

Every result binds scope, capability, evidence, confidence, constraints, effective time, expiry and reason.

Fitness does not issue permission.

### WP-05 — Evidence Quality, Drift, Blind Spots and Assessment Challenge

Purpose: implement the existing AWR-001/SYS-008/CON-006/VPL-005 evidence-quality and awareness-quality obligations without automatically activating AWR-002..AWR-005.

Evidence-loss classes:

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

- explicit evidence age/freshness;
- confidence/evidence-quality reduction;
- contradiction preservation;
- provenance validation;
- last-known trustworthy state tagged as last-known, never current;
- explicit expiry;
- no optimistic fallback;
- no silent positive restoration when a source merely reappears;
- material drift detection across the AWR-001-required dimensions: data, models where Foundation-owned/applicable, behavior, configuration, authority, objectives/purpose identity, dependencies and the awareness system's own assessments;
- known blind spots represented explicitly;
- competence-bounded assessment: unsupported assessment scope is rejected or marked insufficient;
- self-assessment does not rely exclusively on subject-produced evidence where independent evidence is required;
- assessments remain challengeable by authorized independent evidence.

This WP implements only requirements already present in current effective AWR-001 and related active sources. It does not activate AWR-003/AWR-004/AWR-005 by implication.

### WP-06 — Accepted Predecessor Truth Integration

Purpose: bind Stage 7 to accepted truth owners without duplicating them.

Required integrations:

- Stage 3 dependency/configuration truth;
- Stage 4 Authority/Lifecycle/state/evidence/reconciliation truth;
- Stage 5 contracts/messages/events/protection truth;
- Stage 6 resource truth/pressure/isolation/load-shedding truth;
- security/trust identity and integrity evidence;
- logging/persistence evidence.

Required properties:

- exact source attribution;
- source remains authoritative at its owner;
- Stage 7 cannot mutate predecessor truth through a projection;
- stale/replayed predecessor evidence cannot silently become current awareness;
- unavailable predecessor truth reduces affected health/fitness;
- closed predecessor semantics remain preserved.

### WP-07 — Health/Fitness Events, Persistence and Reconstruction

Purpose: make material Stage 7 assessments observable and reconstructable using accepted Stage 5/Stage 4/SYS-011/OPS-004 substrates.

Required outcomes:

- governed health-state-change facts;
- governed material fitness-change facts;
- exact event identity/type/schema/version/owner/provenance;
- corrections as new related events, not history mutation;
- replay distinguishability;
- durable assessment history where required;
- reconstruction of the Self Model/fitness basis used for a material decision;
- logging/evidence failure reflected as degraded evidence quality;
- no new general persistence/event/logging engine.

### WP-08 — Authority, Lifecycle and Protective-Consumer Boundary

Purpose: expose Stage 7 outputs to governed consumers while preserving separation of powers.

Required outcomes:

- AUT-001 may consume scoped CON-006 fitness evidence as condition/input;
- material reductions are available to Authority Engine;
- material health/fitness reduction and evidence-loss triggers are publishable to governed protective consumers;
- Lifecycle may consume qualified health/fitness evidence only under its own governing rules;
- loss of required awareness/fitness evidence prevents positive authority inference;
- restoration of health evidence alone does not automatically restore authority;
- recovery-required state can gate affected operation without declaring recovery complete.

Explicit non-scope:

- Stage 7 does not issue Guardian protective commands;
- Stage 7 does not enforce Platform Safe State;
- Stage 7 does not perform isolation/restriction owned by Stage 8;
- Stage 7 does not execute recovery or independent release owned by Stage 9.

### WP-09 — VPL-005 Executable Health-Evidence-Loss Validation and Hardening

Purpose: execute the Stage 7-owned portion of active VPL-005 and prove end-to-end fail-closed Health/Fitness behavior.

Stage 7 executable proof shall cover:

- fresh valid evidence baseline;
- all nine VPL-005 evidence-loss classes;
- explicit uncertainty/health degradation;
- Self Model effect;
- fitness reduction;
- CON-006 projection;
- AUT-001 denial/restriction-input behavior where applicable;
- trigger/notification publication;
- recovery-required gating evidence where applicable;
- last-known state age/expiry;
- stale cached success cannot override unknown current evidence;
- source restoration alone cannot restore authority;
- independent reassessment required before positive restoration;
- unaffected capability isolation where evidence independence is trustworthy;
- zero-Application validity;
- no Application business semantics.

Required Stage 7 end-to-end path:

```text
raw observation change
-> health assessment
-> Self Model change
-> technical fitness change
-> CON-006 result
-> AUT-001 governed consumption evidence
-> trigger/gating evidence
-> attributable history/reconstruction
```

Future proof explicitly deferred without claiming PASS:

- actual Guardian/Safe-State restriction/isolation enforcement -> Stage 8;
- recovery execution, independent recovery acceptance and release -> Stage 9.

Stage 7 VPL-005 evidence shall record these downstream triggers/boundaries without pretending those future implementations have executed.

### WP-10 — Integrated Stage 7 Closure Verification

Purpose: prove Stage 7 as one coherent Foundation capability family before any Stage-level closure decision.

Required verification:

- WP-01..WP-09 regressions;
- exact SYS-008 requirement coverage;
- exact AWR-001 Stage 7 implementation-scope requirement coverage;
- CON-006 requirement coverage;
- Stage 7-owned VPL-005 end-to-end PASS with explicit Stage 8/9 boundary accounting;
- deterministic identical-input assessments;
- mutation sensitivity for evidence/authority/dependency/resource changes;
- exact candidate build/test identity;
- Foundation Architecture PASS;
- Foundation Security PASS;
- predecessor regression preservation;
- zero-Application validity;
- cross-Application technical isolation where Application identities are only governed technical subjects;
- no Application business interpretation;
- no Stage 8/9/13 implementation/authority surface;
- no deployment/external/financial authority claim;
- fresh post-executable Red-Team;
- final Stage 7 closure-readiness report;
- separate Project Owner Stage 7 closure decision.

## 6. Verification rules

Every WP requires positive and fail-closed/mutation scenarios.

No technical PASS automatically closes a WP or Stage.

Executable evidence rules:

- freeze exact candidate;
- use one controlled Release build for the run set;
- hash material verifier/executable identities;
- no build/restore after run phase begins;
- keep generated evidence outside the repository worktree unless a governed canonical documentary record is intentionally created;
- verify exact final HEAD, clean worktree and refreshed remote identity;
- rerun deterministic material assessments from identical Release outputs;
- classify every failure before remediation.

## 7. Specification-definition rule

AWR-002..AWR-005 remain planned subjects only.

If any WP requires normative behavior unavailable from current effective AWR-001/SYS-008/CON-006/VPL-005:

`STOP -> SPECIFICATION_DEFINITION_REVIEW_ACTIVATION_GATE -> GOVERNED DECISION -> PLAN RECONCILIATION`.

No code may silently become specification.

## 8. Architecture requirements

Before implementation authorization, prove:

- no duplicate Health owner;
- no duplicate Fitness owner;
- no duplicate Authority/Lifecycle/Event/Persistence/Resource owner;
- correct FSA vs Application awareness ownership;
- correct Stage 7 vs Stage 8/9/13 boundary;
- zero-Application validity;
- Application business neutrality;
- awareness loss reduces/restricts capability rather than creating permission;
- any bounded extension to a closed predecessor project preserves its accepted semantics and receives exact WP authority.

## 9. Red-Team minimum coverage

Challenge at least:

- missing evidence shown healthy;
- stale evidence shown current;
- contradiction silently collapsed;
- subject self-report accepted as independent evidence;
- assessment beyond demonstrated competence;
- drift in configuration/authority/objective/dependency/assessment missed;
- Self Model replacing authoritative state;
- health/fitness creating permission;
- expired fitness reused;
- expired last-known state reused;
- source restoration silently restoring authority;
- aggregate health hiding critical dependency failure;
- resource pressure ignored;
- Application business preference converted to technical fitness/criticality;
- cross-Application evidence contamination;
- FSA crossing into Application awareness semantics;
- Stage 7 performing Guardian/Safe-State enforcement;
- Stage 7 performing recovery/release;
- Stage 7 importing Stage 13 Owner/FSA governance;
- replay recreating current fitness incorrectly;
- corrupted persistence reconstructing favorable state;
- logging/evidence loss hidden;
- zero-Application state treated as unhealthy by default;
- source code inventing unresolved health thresholds/consequence policy;
- Stage 7 silently repairing a true predecessor defect.

## 10. WP-by-WP authority discipline

After Owner plan acceptance, implementation remains WP-by-WP unless the Owner explicitly grants broader authority.

For each WP:

1. fresh FCR check;
2. fresh governing-source read;
3. exact reuse census relevant to the WP;
4. exact implementation scope/allowlist;
5. separate implementation authority;
6. implementation;
7. executable verification;
8. Architecture/Security checks as applicable;
9. fresh Red-Team after any user-directed modification;
10. Owner closure;
11. only then proceed to the next separately authorized scope.

## 11. Remediation trace from Architecture Review V1

Finding A — drift/competence/challenge coverage:

`RESOLVED` in WP-05.

Finding B — unresolved SYS-008 health policy:

`RESOLVED` by mandatory Gate 0B and prohibition on source-code policy invention.

Finding C — VPL-005 Stage 8/9 boundary:

`RESOLVED` in WP-08/WP-09 through explicit trigger/gating proof versus future enforcement/recovery proof separation.

## 12. Current disposition

`STAGE7_PLAN_v0.2 = FINAL_CANDIDATE`

`PROPOSED_WORK_PACKAGES = WP01_THROUGH_WP10`

`GATE0A_CODE_REUSE_CENSUS = MANDATORY`

`GATE0B_HEALTH_RULE_POLICY_DEFINITION = MANDATORY`

`STAGE7_PLAN_OWNER_ACCEPTANCE = NOT_YET`

`STAGE7_IMPLEMENTATION_AUTHORITY = NOT_GRANTED`

`STAGE8_AUTHORITY = NOT_GRANTED`