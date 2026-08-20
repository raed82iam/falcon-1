# Stage 7 — Foundation Health, Self-Awareness and Technical Fitness

Implementation Plan: v0.3 FINAL CANDIDATE
Status: PROPOSED / OWNER REVIEW REQUIRED / IMPLEMENTATION NOT AUTHORIZED
Date: 2026-08-11
Supersedes for review: v0.2 candidate

## 1. Objective

Complete the Foundation runtime implementation and integration required for Health Monitoring, Foundation Self-Awareness state, and scoped Technical Fitness using the current effective documentary baseline, while reusing accepted Stage 0..6 truth owners and preserving later Stage boundaries.

Controlling sources:

- SYS-008 — Health Monitoring;
- AWR-001 v2.1 — Foundation Self-Awareness System;
- CON-006 v1.1 — Health and Fitness Contract;
- VPL-005 v1.1 — Health Evidence Loss Plan;
- IMP-001 v1.3;
- TRC-001 v1.4;
- Falcon Vision and Constitution;
- accepted Stage 0..6 baseline.

## 2. Authority state

`STAGE6 = ACCEPTED_AND_CLOSED`

`STAGE7_PLANNING_AND_DESIGN = AUTHORIZED`

`STAGE7_PLAN_OWNER_ACCEPTANCE = NOT_YET`

`STAGE7_IMPLEMENTATION_AUTHORITY = NOT_GRANTED`

No production implementation follows from this candidate.

## 3. Mandatory pre-implementation gates

### Gate 0A — Exact Code Reuse / Ownership Census

Before WP-01 source implementation, inspect the live `foundation-development` branch and classify all existing potentially reusable runtime types/projects/APIs related to:

- health observations and assessments;
- freshness/confidence/evidence quality;
- Foundation Self Model projection;
- technical fitness;
- health/fitness events;
- health/fitness persistence/history;
- Authority/Lifecycle consumption of fitness evidence.

Allowed dispositions:

- `REUSE_AS_IS`;
- `REUSE_WITH_BOUNDED_EXTENSION`;
- `NOT_APPLICABLE`;
- `GENUINELY_MISSING`.

No duplicate implementation may be introduced while an accepted equivalent exists.

If Stage 7 requires touching a project created in a closed predecessor Stage, classify the exact change as:

1. existing-public-behavior consumption;
2. bounded additive extension preserving accepted semantics; or
3. true predecessor accepted-scope defect.

A true predecessor defect requires explicit trace and separately governed remediation authority. Stage 7 may not silently repair it.

### Gate 0B — Health Rule Policy Definition

Before executable health/fitness rules depend on policy values, determine whether approved sources already define:

- freshness windows by subject/capability;
- health consequence classes;
- evidence requirements;
- confidence/evidence-quality rules;
- critical dependency aggregation policy;
- applicable `RECOVERY_REQUIRED -> RESTRICTED/NOT_FIT` consequence policy.

Every runtime rule shall bind:

- subject/capability scope;
- evidence set;
- freshness rule;
- confidence rule;
- consequence treatment;
- rule identity/version;
- accountable owner;
- governing authority.

Source code shall not invent thresholds, consequence classes, freshness windows or recovery consequence policy.

If normative meaning is genuinely absent from current effective sources:

`STOP -> SPECIFICATION_DEFINITION_REVIEW_ACTIVATION_GATE -> GOVERNED DECISION -> PLAN RECONCILIATION`.

## 4. Architectural responsibility chain

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

Invariants:

- Health != Authority.
- Fitness != Authority.
- Self Model != authoritative predecessor state.
- FSA != Lifecycle.
- FSA != Guardian.
- FSA != Recovery Authority.
- FSA != Stage 6 Resource Governance.
- FSA does not interpret Application business meaning.
- Stage 7 does not implement Application MSA/LSA/CSA internals.
- Stage 7 does not implement Stage 13 Owner/FSA governance.

## 5. Exact AWR-001 Stage-placement matrix

### AWR-001 REQ-001 through REQ-020

Disposition:

`STAGE7_OWNED_FOR_IMPLEMENTATION_OR_EXPLICIT_REUSE_TRACE`

Stage 7 shall provide exact requirement-by-requirement trace for:

- assertion source/time/freshness/confidence;
- fact/estimate/assumption/interpretation/unknown distinction;
- missing/stale evidence effect;
- contradiction visibility;
- blind spots;
- scoped competence/evidence/authority/risk/security/dependency/temporal fitness inputs;
- automatic fitness reduction;
- attributable scoped fitness results;
- fitness/authority separation;
- material fitness events made available to governed protective consumers;
- correlation of health/config/security/lifecycle/exposure/decision/dependency evidence without owning source facts or Application meaning;
- current/last-known/expected/desired distinction;
- drift detection;
- independent evidence where required;
- reconstructability;
- immutable awareness history/supersession;
- loss-of-awareness fitness consequence;
- honest uncertainty;
- independent challengeability;
- competence bounds.

REQ-010 is Stage 7-owned only through publication/availability of governed fitness-change evidence. Actual Guardian enforcement remains Stage 8.

REQ-011 does not authorize Stage 7 to implement future Stage 11 QoS/deadline observability; it uses available accepted technical evidence within current effective Stage 7 scope.

### AWR-001 REQ-021

`DEFERRED_WITH_TRACE_TO_STAGE9_AND_LATER_GOVERNANCE_AS_APPLICABLE`

Stage 7 may report `RECOVERY_REQUIRED`, fitness loss and trusted-state evidence. It shall not implement bounded repair execution, recovery acceptance or release as Stage 7 authority.

### AWR-001 REQ-022 through REQ-024

`DEFERRED_WITH_TRACE_TO_STAGE13`

Stage 7 may expose evidence showing drift/weakness/capability gap. It shall not implement self-evolution classification/control-plane authority, candidate creation/promotion governance, or self-approval/deployment rules as an active Stage 7 subsystem.

### AWR-001 Section 9 — Foundation Change Conformance

Split placement:

- Stage 7: technical health/fitness/evidence support and current Self Model representation of conformance-relevant facts;
- Stage 13: governed FSA/Owner proposal-review/control-plane realization and adoption workflow.

Stage 7 shall not claim full Section 9 governance workflow closure.

### AWR-001 Section 10 — Repair, Evolution and Candidate Governance

Placement:

- Stage 9: recovery/controlled repair realization where applicable;
- Stage 13: FSA/Owner bounded self-maintenance/evolution governance and candidate/adoption controls.

Stage 7 closure shall preserve these obligations as future trace, not mark them executable PASS.

## 6. Work Package sequence

### WP-01 — Canonical Health and Fitness Runtime Primitives

Implement/reuse the minimum canonical runtime representation required by SYS-008, AWR-001 and CON-006:

- health states `HEALTHY / DEGRADED / UNHEALTHY / UNKNOWN / NOT_APPLICABLE`;
- AWR-001 technical-fitness states;
- CON-006 `FIT / RESTRICTED / NOT_FIT` results;
- assessment IDs;
- subject/capability/scope identity;
- evidence references/provenance;
- observation/assessment/effective/expiry times;
- confidence, constraints, unknowns and reason;
- strict canonical enum/identifier validation;
- immutable/value-semantic assessment identity where applicable;
- no authority-grant surface.

Negative coverage includes malformed identity, invalid enums, impossible time order, missing required evidence and any Application-business semantic leakage.

### WP-02 — Health Observation and Assessment Runtime

Implement SYS-008 evaluation:

- attributable observations;
- evidence time vs assessment time;
- Gate-0B governed freshness rules;
- rule identity/version;
- declared availability/correctness/integrity/performance/dependency dimensions;
- deterministic assessment;
- dependency/aggregate health preserving critical failure;
- contradiction/unknown behavior;
- monitor-source failure/loss of visibility;
- material transition output;
- no repair/lifecycle/authority action.

Fail closed when required evidence is missing, stale, contradictory or unavailable.

### WP-03 — Foundation Self Model Runtime

Implement AWR-001 Foundation-only Self Model projection over authoritative evidence:

- baseline and Foundation identity;
- component identity/version/lifecycle/integrity;
- runtime/infrastructure;
- Service Bus/FIL technical condition;
- dependencies;
- resource capacity/pressure/exhaustion risk from Stage 6;
- persistence/configuration/documentary integrity;
- security/authority condition;
- incidents/faults/contradictions/blind spots;
- isolation/recovery readiness as observed state;
- active restrictions;
- evidence provenance/freshness/confidence/uncertainty;
- current/last-known/expected/desired state distinction;
- historical lineage;
- zero-Application validity.

The Self Model is a projection and cannot mutate or replace authoritative source truth.

### WP-04 — Technical Fitness Evaluation and CON-006 Projection

Implement scoped deterministic AWR-001 technical fitness and exact CON-006 mapping:

- `FIT -> FIT`;
- `FIT_WITH_CONSTRAINTS -> RESTRICTED`;
- `DEGRADED -> RESTRICTED`;
- `UNKNOWN -> NOT_FIT`;
- `UNAVAILABLE -> NOT_FIT`;
- `INTEGRITY_FAILURE -> NOT_FIT`;
- `ISOLATION_REQUIRED -> RESTRICTED`;
- `RECOVERY_REQUIRED -> RESTRICTED or NOT_FIT` only through Gate-0B governed consequence policy;
- `NOT_FIT -> NOT_FIT`.

Every result binds scope, level, evidence, confidence, constraints, expiry and reason.

Fitness cannot issue permission.

### WP-05 — Evidence Quality, Drift, Blind Spots and Independent Challenge

Implement current-effective AWR-001/SYS-008/CON-006/VPL-005 semantics for:

- `MISSING`;
- `STALE`;
- `DELAYED`;
- `CONTRADICTORY`;
- `UNVERIFIABLE`;
- `INACCESSIBLE`;
- `CORRUPTED`;
- `PROVENANCE_FAILURE`;
- `PARTIAL_VISIBILITY`;
- evidence age/expiry;
- confidence reduction;
- last-known state tagging and expiry;
- no optimistic fallback;
- drift in data, Foundation-owned/applicable models, behavior, configuration, authority, objectives/purpose identity, dependencies and own assessments;
- known blind spots;
- competence-bounded assessment;
- independent evidence where required;
- authorized challenge by independent evidence;
- no silent authority restoration after source reappearance.

This WP does not activate AWR-003/AWR-004/AWR-005 by implication.

### WP-06 — Accepted Predecessor Truth Integration

Bind Stage 7 to:

- Stage 3 dependency/configuration truth;
- Stage 4 Authority/Lifecycle/state/evidence/reconciliation truth;
- Stage 5 contracts/message/event/protection truth;
- Stage 6 resource truth/pressure/isolation/load-shedding truth;
- accepted security/trust identity;
- logging/persistence evidence.

Properties:

- source attribution remains exact;
- source owner remains authoritative;
- stale/replayed source evidence cannot silently become current awareness;
- unavailable truth reduces health/fitness;
- no projection mutates predecessor truth;
- no closed predecessor semantic repair without separate authority.

### WP-07 — Health/Fitness Events, Persistence and Reconstruction

Using accepted Stage 4/5/SYS-011/OPS-004 substrates:

- publish governed health-state-change facts;
- publish material fitness-change facts;
- preserve event type/schema/version/owner/provenance;
- corrections use new related events;
- replay is distinguishable;
- persist required assessment/history state;
- reconstruct exact Self Model/fitness basis for a material decision;
- reflect logging/evidence failure as evidence-quality loss;
- do not create duplicate event/logging/persistence engines.

### WP-08 — Authority, Lifecycle and Protective-Consumer Boundary

Provide governed consumption boundaries:

- AUT-001 may consume CON-006 fitness as condition/input;
- material reductions available to Authority Engine;
- fitness/health/evidence-loss changes publishable to governed protective consumers;
- Lifecycle consumes only under its own rules;
- missing required awareness/fitness blocks positive authority inference;
- source recovery alone does not restore authority;
- `RECOVERY_REQUIRED` can gate affected capability without declaring recovery complete.

Explicitly excluded:

- Guardian command/enforcement and Platform Safe State -> Stage 8;
- recovery execution/independent release -> Stage 9.

### WP-09 — VPL-005 Executable Health-Evidence-Loss Validation and Hardening

Execute the Stage 7-owned portion of active VPL-005:

- fresh valid baseline;
- all nine evidence-loss classes;
- explicit health uncertainty/degradation;
- Self Model impact;
- fitness reduction;
- CON-006 projection;
- AUT-001 denial/restriction input behavior where applicable;
- trigger/notification publication;
- recovery-required gating evidence;
- last-known-state expiry;
- stale cached success rejected;
- source restoration alone cannot restore authority;
- independent reassessment required;
- unaffected capability isolation where evidence independence is valid;
- zero-Application validity;
- no Application business semantics.

End-to-end Stage 7 path:

```text
observation change
-> health assessment
-> Self Model change
-> technical fitness
-> CON-006 result
-> AUT-001 governed consumption evidence
-> trigger/gating evidence
-> attributable history/reconstruction
```

Explicitly not claimed as Stage 7 PASS:

- actual Guardian/Safe-State enforcement -> Stage 8;
- recovery execution/independent recovery acceptance/release -> Stage 9.

### WP-10 — Integrated Stage 7 Closure Verification

Required closure proof:

- WP-01..WP-09 regressions;
- SYS-008 requirements mapped and validated;
- AWR-001 REQ-001..020 exact Stage 7 trace and validation/reuse evidence;
- AWR-001 REQ-021..024 preserved as deferred trace with no false PASS;
- AWR-001 Sections 9/10 split placement preserved;
- CON-006 requirements validated;
- Stage 7-owned VPL-005 PASS with explicit Stage 8/9 boundary accounting;
- deterministic identical-input assessments;
- mutation sensitivity;
- exact candidate/build/verifier identities;
- Foundation Architecture PASS;
- Foundation Security PASS;
- predecessor regressions preserved;
- zero-Application validity;
- cross-Application technical isolation;
- no Application business interpretation;
- no Stage 8/9/13 implementation or authority surface;
- no deployment/external/financial authority claim;
- fresh post-executable Red-Team;
- final Stage 7 closure-readiness report;
- separate Owner Stage 7 closure decision.

## 7. Verification discipline

Every WP requires positive plus fail-closed/mutation scenarios.

No technical PASS automatically closes a WP or Stage.

Executable runs shall:

- freeze exact candidate;
- use a controlled Release build;
- hash material verifier/executable identities;
- prohibit build/restore after the run phase begins;
- isolate generated evidence outside the repository worktree unless intentionally recorded as canonical documentation;
- verify final exact HEAD, clean worktree and refreshed remote identity;
- rerun deterministic material assessments from identical Release outputs;
- classify every failure before remediation.

## 8. Planned Specification rule

AWR-002..AWR-005 remain registry-only planned subjects with no effective bodies.

They contribute no invented normative requirements.

If a genuine missing normative behavior is discovered:

`STOP -> SPECIFICATION_DEFINITION_REVIEW_ACTIVATION_GATE -> GOVERNED DECISION -> PLAN RECONCILIATION`.

## 9. Minimum Architecture / Red-Team challenge set

The final plan and every material implementation revision shall challenge:

- missing/stale evidence shown healthy/current;
- contradiction silently collapsed;
- subject self-report treated as independent evidence;
- assessment beyond competence;
- material drift missed;
- Self Model replacing source truth;
- health/fitness creating permission;
- expired fitness/last-known state reused;
- source recovery silently restoring authority;
- critical dependency failure hidden by aggregate health;
- Stage 6 resource pressure ignored;
- Application business preference converted into Foundation fitness/criticality;
- cross-Application evidence contamination;
- FSA crossing into MSA/LSA/CSA business semantics;
- Stage 7 implementing Stage 8 Guardian/Safe State;
- Stage 7 implementing Stage 9 recovery release;
- Stage 7 implementing Stage 13 proposal/evolution control plane;
- replay recreating current fitness incorrectly;
- corrupted persistence reconstructing a favorable state;
- logging/evidence failure hidden;
- zero-Application state incorrectly treated as unhealthy;
- source-code invention of unresolved health policy;
- silent repair of a true predecessor defect;
- false claim that AWR-001 REQ-021..024 are closed by Stage 7.

## 10. WP-by-WP authority rule

After Owner acceptance of this plan, implementation remains separately gated WP-by-WP unless the Owner explicitly grants broader authority.

For every WP:

1. fresh FCR sweep;
2. fresh governing-source read;
3. exact reuse/ownership census;
4. exact scope/allowlist;
5. separate implementation authority;
6. implementation;
7. executable validation;
8. Architecture/Security review as applicable;
9. fresh Red-Team after any Owner-directed modification;
10. Owner closure;
11. only then proceed to the next authorized WP.

## 11. Remediation trace

Architecture Review V1:

- Finding A drift/competence/challenge -> resolved in WP-05.
- Finding B unresolved SYS-008 policy -> resolved in Gate 0B.
- Finding C VPL-005 Stage 8/9 boundary -> resolved in WP-08/WP-09.

Architecture Review V2:

- Finding D AWR-001 broad-scope ambiguity -> resolved by Section 5 exact requirement/stage-placement matrix and WP-10 trace rules.

## 12. Current disposition

`STAGE7_PLAN_v0.3 = FINAL_CANDIDATE`

`PROPOSED_WORK_PACKAGES = WP01_THROUGH_WP10`

`GATE0A_CODE_REUSE_CENSUS = MANDATORY`

`GATE0B_HEALTH_RULE_POLICY_DEFINITION = MANDATORY`

`AWR001_REQ001_TO_REQ020 = STAGE7_TRACE_REQUIRED`

`AWR001_REQ021_TO_REQ024 = DEFERRED_WITH_TRACE / NO_STAGE7_FALSE_PASS`

`STAGE7_PLAN_OWNER_ACCEPTANCE = NOT_YET`

`STAGE7_IMPLEMENTATION_AUTHORITY = NOT_GRANTED`

`STAGE8_AUTHORITY = NOT_GRANTED`