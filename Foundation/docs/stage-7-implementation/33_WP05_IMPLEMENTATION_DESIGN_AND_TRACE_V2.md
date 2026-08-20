# Stage 7 WP-05 — Evidence Quality, Drift, Blind Spots and Independent Challenge

**Design and Trace:** V2  
**Date:** 2026-08-13  
**Status:** `PRE_EXECUTABLE_DESIGN_CANDIDATE`  
**Foundation Branch:** `foundation-development`  
**Starting Accepted Closure State:** Gate 0A through WP-04 = `ACCEPTED_AND_CLOSED`  
**WP-05 Owner Closure:** `NOT_YET`  
**Stage 7 Closure:** `NOT_YET`

## 1. Purpose

Define the minimum bounded implementation for Stage 7 WP-05 required to realize current effective evidence-quality, evidence-loss, drift, blind-spot, competence and independent-challenge semantics without duplicating WP-02 Health truth, WP-03 Self Model truth, WP-04 Technical Fitness truth, or pulling WP-06 through WP-10 and later-stage authority into WP-05.

This design is additive. It does not reinterpret accepted WP-01 through WP-04 semantics.

## 2. Controlling Sources

The implementation SHALL remain traceable to the current effective sources, including:

- Falcon Vision;
- Falcon Constitution;
- AWR-001 v2.1;
- SYS-008 v1.1;
- CON-006 v1.2;
- VPL-005 v1.1;
- Stage 7 Implementation Plan v0.3 accepted implementation sequence;
- Gate 0A reuse/ownership census;
- Gate 0B policy and freshness feasibility evidence;
- accepted/closed Stage 7 WP-01 through WP-04 runtime and verification evidence.

No planned AWR-003/AWR-004/AWR-005 body is inferred or activated.

## 3. Fresh Reuse and Ownership Census

### 3.1 `REUSE_AS_IS`

#### WP-01 Health/Fitness primitives

Reuse:

- `HealthState`;
- `TechnicalFitnessState`;
- `FitnessProjectionResult`;
- `EvidenceQuality`;
- `CanonicalHealthFitnessAssessment`;
- existing canonical identifier and time validators.

WP-05 SHALL NOT create a competing evidence-quality scale.

#### WP-02 Health Observation and Assessment Runtime

Reuse existing ownership of:

- required vs optional/diagnostic evidence roles;
- freshness evaluation;
- provenance validation;
- integrity validation;
- clock validation;
- visibility loss;
- contradiction handling;
- independent-required evidence declaration;
- fail-closed `UNKNOWN` for missing/stale/invalid required evidence;
- dependency evidence reduction;
- positive-proof acyclicity.

WP-05 SHALL NOT re-run or override Health determination as a second Health evaluator.

#### WP-03 Foundation Self Model Runtime

Reuse existing ownership of:

- Current vs LastKnown vs Expected vs Desired vs Historical;
- Fact/Estimate/Assumption/Interpretation/Unknown distinction;
- evidence quality, confidence and uncertainty;
- contradiction representation;
- explicit BlindSpotCondition model area;
- assertion source/owner/evidence/freshness/rule/time/expiry identity;
- current assertion coverage requirements;
- canonical model identity.

WP-05 SHALL NOT allow LastKnown to satisfy missing Current awareness.

#### WP-04 Technical Fitness Runtime

Reuse existing ownership of:

- capability-scoped deterministic Technical Fitness;
- evidence-quality reduction;
- limited/insufficient evidence prohibition on unrestricted FIT;
- contradiction consequence;
- CON-006 projection;
- RecoveryRequired bounded exception handling;
- no authority-grant surface.

WP-05 SHALL NOT issue Fitness permission or Authority decisions.

### 3.2 `REUSE_WITH_BOUNDED_EXTENSION`

A bounded additive runtime is required to represent the full VPL-005 evidence-loss taxonomy explicitly and correlate it to existing canonical Health/Self Model/Fitness evidence without changing WP-02 Health semantics.

### 3.3 `GENUINELY_MISSING`

Current accepted code does not yet provide a complete runtime for:

- full nine-class evidence-loss declaration and coverage;
- explicit evidence acquisition/arrival state for `DELAYED`;
- required evidence-loss coverage completeness;
- eight-domain drift coverage declarations and findings;
- demonstrated assessment competence declarations;
- explicit known blind-spot assessments derived from uncovered/incompetent scope;
- authorized independent challenge records;
- post-reappearance independent reassessment/restoration-gate evidence;
- WP-05 verifier;
- WP-05 architecture guard.

### 3.4 `NOT_APPLICABLE / DEFERRED`

WP-05 does not own:

- exact Stage 3 through Stage 6 and security source binding/authenticity integration: WP-06;
- persistence, reconstruction and event publication: WP-07;
- Authority/Lifecycle/protective-consumer enforcement: WP-08;
- full VPL-005 end-to-end executable path: WP-09;
- Stage 8 Guardian/Safe-State enforcement;
- Stage 9 recovery execution/release;
- Stage 13 FSA/Owner governance, Monitor AI or evolution control plane.

## 4. Predecessor Defect Classification

Fresh review finds no true accepted-scope predecessor semantic defect required for WP-05.

```text
TRUE_PREDECESSOR_DEFECT_FOUND = NO
WP01_TO_WP04_REOPEN_REQUIRED = NO
```

WP-05 is an additive Stage 7 capability over accepted predecessor behavior.

The Gate 0B Evidence journal bounded-probe observation remains rule-activation-specific. WP-05 shall not activate a periodic full-journal Health rule and therefore does not require an Evidence-journal predecessor rewrite merely to implement WP-05 semantics.

## 5. Proposed Minimal Change Surface

### 5.1 Health/Fitness-owned additive file

Create:

`src/Foundation.HealthFitness/HealthEvidenceQualityRuntime.cs`

Responsibilities:

- canonical evidence-loss declaration;
- nine VPL-005 classes;
- explicit availability/non-loss declaration used to prove coverage;
- acquisition/arrival state required for delayed evidence;
- evidence age/expiry inputs;
- required/optional relation criticality;
- quality consequence bounded by existing `EvidenceQuality`;
- binding to an existing `CanonicalHealthAssessment`;
- effective-quality calculation that can only preserve or reduce canonical WP-02 Health evidence quality.

It shall not determine Health independently.

### 5.2 Self-Awareness-owned additive file

Create:

`src/Foundation.SelfAwareness/EvidenceAwarenessRuntime.cs`

Responsibilities:

- eight-domain drift coverage declarations;
- drift finding representation;
- competence declarations;
- known blind-spot derivation;
- independent challenge and reassessment records;
- LastKnown reliance eligibility tagging;
- source-reappearance/restoration-gate state;
- Self Model correlation only, without becoming source truth;
- deterministic canonical identities.

### 5.3 Verification project

Create a Stage 7 WP-05 verifier under:

`verification/Falcon.Stage7.WP05.Verifier/`

It shall verify positive and fail-closed/mutation scenarios for the WP-05-owned behavior only.

### 5.4 Architecture guard

Add a WP-05 architecture guard under Foundation architecture tests to enforce:

- no Application references;
- no Guardian command surface;
- no Authority decision/grant surface;
- no Lifecycle transition surface;
- no Recovery execution/release surface;
- no persistence/event engine duplication;
- no AWR-003/AWR-004/AWR-005 activation;
- no source-owner mutation path.

### 5.5 Solution membership

Add only the WP-05 verifier project to the governed solution if required by the existing Stage 7 verification pattern. No unrelated project membership changes are permitted.

## 6. Evidence-Loss Model

### 6.1 Loss classes

The canonical WP-05 loss enum SHALL represent exactly:

- `MISSING`;
- `STALE`;
- `DELAYED`;
- `CONTRADICTORY`;
- `UNVERIFIABLE`;
- `INACCESSIBLE`;
- `CORRUPTED`;
- `PROVENANCE_FAILURE`;
- `PARTIAL_VISIBILITY`.

A separate explicit non-loss state such as `AVAILABLE` SHALL exist only to prove declaration/coverage completeness. It is not an additional VPL-005 loss class.

Silence or omitted declaration SHALL NOT mean `AVAILABLE`.

### 6.2 Required declaration fields

Every evidence-loss assessment SHALL bind at minimum:

- assessment identity;
- evidence/relation identity;
- subject;
- capability/scope;
- observation time;
- assessment time;
- evidence source and owner;
- evidence reference;
- relation role/criticality;
- loss class or explicit available state;
- acquisition/arrival state where applicable;
- evidence age/expiry relation;
- canonical WP-02 Health assessment reference;
- resulting bounded `EvidenceQuality`;
- reason.

### 6.3 `DELAYED` semantics

`DELAYED` SHALL be derived only from an explicit acquisition/arrival condition showing that expected evidence has not yet arrived but remains within a governed pending/expiry relation.

A future-dated observation is NOT `DELAYED`.

Future-dated evidence is structurally untrustworthy for current positive reliance and shall be rejected or treated as `UNVERIFIABLE`/insufficient according to the exact validated condition. No future timestamp may extend freshness.

### 6.4 Required relation effects

For a required evidence relation:

- active `MISSING` => no positive Health/Fitness inference;
- active `STALE` => no fresh positive inference;
- active `DELAYED` => pending, no positive conclusion;
- active `CONTRADICTORY` => explicit contradiction, no collapse;
- active `UNVERIFIABLE` => insufficient;
- active `INACCESSIBLE` => insufficient for unrestricted reliance;
- active `CORRUPTED` => invalid for reliance;
- active `PROVENANCE_FAILURE` => invalid relation;
- active `PARTIAL_VISIBILITY` => insufficient/incomplete.

Optional/supporting relations may reduce to bounded `Limited` when governing Health semantics permit, but may never repair a required relation.

## 7. No Optimistic Divergence from WP-02

WP-05 cannot improve the canonical WP-02 Health evidence truth.

Define ordering from strongest to weakest:

```text
Sufficient > Limited > Insufficient > Invalid
```

The effective WP-05 evidence quality SHALL be the worst applicable quality among:

```text
CANONICAL_WP02_HEALTH_QUALITY
WP05_LOSS_STATUS_QUALITY
WP05_COMPETENCE_QUALITY
WP05_CHALLENGE_QUALITY
```

Therefore:

```text
WP05_EFFECTIVE_QUALITY <= CANONICAL_WP02_HEALTH_QUALITY
```

where `<=` means no more trustworthy than the canonical WP-02 quality.

If an input view conflicts with WP-02 canonical Health, WP-05 SHALL preserve the conflict as a contradiction/blind spot and choose the less optimistic quality. It SHALL NOT overwrite WP-02.

## 8. Current vs Last-Known Behavior

### 8.1 Historical preservation

A prior trustworthy assertion may remain preserved as LastKnown/Historical evidence.

Historical preservation does not itself authorize operational reliance.

### 8.2 LastKnown reliance eligibility

LastKnown reliance is eligible only when all required fields are present:

- explicit governing policy reference allowing LastKnown fallback;
- authoritative source identity and owner;
- original evidence reference;
- original observation/effective time;
- explicit age;
- explicit expiry/freshness limit;
- no unresolved contradiction/provenance/integrity failure;
- current time remains inside the permitted reliance interval.

When any condition is absent or expired:

```text
LAST_KNOWN_RELIANCE = INELIGIBLE
```

LastKnown SHALL NEVER become Current by tagging or by source silence.

When required current awareness is lost, an explicit Current Unknown/insufficient assertion must coexist with any retained LastKnown history for the affected area/scope.

## 9. Drift Model

### 9.1 Exact domains

WP-05 SHALL represent explicit drift coverage for all eight AWR-001 domains:

1. `DATA`;
2. `FOUNDATION_MODEL` for Foundation-owned/applicable models only;
3. `BEHAVIOR`;
4. `CONFIGURATION`;
5. `AUTHORITY`;
6. `OBJECTIVE_PURPOSE_IDENTITY`;
7. `DEPENDENCY`;
8. `OWN_ASSESSMENT`.

### 9.2 Coverage completeness

Every WP-05 awareness evaluation SHALL declare each domain as one of:

- applicable and watched;
- explicitly non-applicable with evidence-bound reason.

A missing domain declaration is not interpreted as no drift.

It becomes a known blind spot and insufficient awareness for that domain.

### 9.3 Materiality

WP-05 SHALL NOT invent numeric drift thresholds.

Materiality is represented by a governed watch/declaration containing rule identity/version, subject, scope, expected/reference identity or comparison basis, source/evidence reference and competence requirement.

A finding states observed mismatch/change against that governed basis. The runtime does not fabricate a policy threshold absent from governing authority.

## 10. Competence Bounds

Every evaluator claiming coverage for an applicable drift/awareness domain SHALL have a current competence declaration binding:

- evaluator identity;
- domain;
- Self Model area or governed assessment area;
- subject;
- scope;
- competence evidence reference;
- source/owner of competence evidence;
- effective time;
- expiry;
- rule identity/version.

Missing, expired, mismatched or unverifiable competence SHALL produce:

```text
COMPETENCE = INSUFFICIENT
KNOWN_BLIND_SPOT = YES
POSITIVE_COVERAGE_CLAIM = PROHIBITED
```

WP-05 does not create self-issued competence authority.

## 11. Known Blind Spots

A blind spot SHALL be explicit when any applicable required awareness relation has, at minimum:

- missing coverage declaration;
- missing/expired competence;
- inaccessible or partial visibility;
- source authenticity not yet establishable within current WP boundaries;
- unresolved contradiction preventing reliable interpretation;
- independent evidence required but not available;
- challenge state unresolved.

Blind spots bind subject, scope/domain, reason, evidence references and observation/assessment times.

A blind spot is evidence of limited awareness. It is never automatically treated as a positive condition.

## 12. Independent Evidence and Challenge

### 12.1 Independence

Where independent evidence is required, the challenge/reassessment evidence owner SHALL differ from the owner of the challenged evidence relation.

Identity labels alone do not prove independence. WP-05 shall enforce structural owner separation and bind authorization/evidence references.

Exact predecessor/source authenticity integration remains WP-06-owned. Therefore the WP-05 record shall expose authenticity state explicitly as structurally bound but, where exact predecessor integration is not yet available:

```text
SOURCE_AUTHENTICITY = PENDING_WP06
```

This state can never improve evidence quality or permit authority.

### 12.2 Authorized challenge record

Every challenge SHALL bind:

- challenge identity;
- challenged assessment/evidence identity;
- challenger identity and owner;
- authorization evidence reference;
- independent evidence reference;
- challenge observation time;
- challenge assessment time;
- challenge result;
- reason;
- expiry where applicable.

Unauthorized, expired, self-owned or circular challenge evidence is invalid for positive reassessment.

### 12.3 Challenge effect

Challenge may:

- confirm a bounded assessment when independently supported;
- reduce confidence/quality;
- expose contradiction/blind spot;
- require reassessment.

Challenge SHALL NOT:

- rewrite the canonical WP-02 Health assessment;
- convert insufficient Health evidence into sufficient evidence by itself;
- grant Fitness;
- grant Authority;
- release Recovery/Isolation state.

## 13. Source Reappearance and Restoration Gate

Represent restoration state explicitly, using equivalent canonical states to:

- `NO_PRIOR_LOSS`;
- `LOSS_ACTIVE`;
- `SOURCE_REAPPEARED_PENDING_INDEPENDENT_REASSESSMENT`;
- `INDEPENDENTLY_REASSESSED`.

There SHALL be no `AUTHORITY_RESTORED` state in WP-05.

`INDEPENDENTLY_REASSESSED` may be reached only when ALL are true:

1. no active required evidence-loss condition remains;
2. the current canonical WP-02 Health assessment evidence quality is `Sufficient` for the affected relation;
3. the current WP-05 loss-status quality is `Sufficient`;
4. a fresh independent reassessment occurs after the restored evidence observation;
5. challenger/reassessor owner differs from the restored source owner where independence is required;
6. authorization evidence is bound and current;
7. no unresolved contradiction, provenance, integrity, competence or visibility blocker remains.

Source reappearance alone SHALL remain pending.

Even `INDEPENDENTLY_REASSESSED` means only that the evidence-awareness restoration gate is satisfied. Authority restoration remains outside WP-05.

## 14. Self Model Correlation

WP-05 may produce governed assertions suitable for WP-03 Self Model projection, including:

- Current Unknown for affected awareness loss;
- LastKnown retained history with explicit reliance eligibility;
- BlindSpotCondition;
- ContradictionCondition;
- evidence-quality/competence/challenge references.

The Self Model remains a projection and SHALL NOT mutate Health source facts or predecessor truth.

## 15. Determinism and Identity

All material WP-05 runtime records SHALL use deterministic canonical identity from ordered canonical fields.

Equivalent input sets in different input order shall yield identical identities/results.

Mutation of any material field, including subject, scope, source owner, loss class, challenge owner, evidence reference, competence expiry, drift domain or restoration state, shall change the canonical identity or fail validation.

No random identity is required for deterministic assessment truth.

## 16. Fail-Closed Rules

WP-05 SHALL fail closed for at least:

- omitted required evidence relation declaration;
- omitted drift domain declaration;
- unknown enum;
- malformed canonical identity;
- future-dated observation used positively;
- expired evidence used as fresh;
- `DELAYED` inferred only from a future timestamp;
- source-owner mismatch;
- self-challenge where independence is required;
- missing/expired challenge authorization;
- missing/expired competence;
- Current positive assertion while an active required loss exists for the same relation;
- LastKnown treated as Current;
- source reappearance treated as restoration;
- WP-05 effective quality stronger than WP-02 canonical quality;
- challenge used to manufacture positive Health;
- any attempt to encode Authority grant/recovery release/Guardian command.

## 17. Verification Matrix

The WP-05 verifier SHALL include at minimum:

### Evidence-loss coverage

- fresh `AVAILABLE` required relation baseline;
- one scenario for each of the nine VPL-005 loss classes;
- required vs optional/supporting relation difference;
- corrupted/provenance failure rejection;
- partial visibility incompleteness;
- stale age/expiry visibility;
- delayed pending then arrival/expiry;
- future-dated evidence not classified as delayed;
- omitted required relation becomes missing/fail closed.

### WP-02 non-optimism

- canonical WP-02 `Insufficient` + WP-05 `Available` cannot become `Sufficient`;
- canonical WP-02 `Invalid` cannot be repaired by WP-05;
- disagreement remains explicit.

### LastKnown

- retained historical LastKnown with eligible policy;
- expiry makes reliance ineligible;
- no policy means ineligible;
- LastKnown cannot satisfy Current;
- active current loss requires Current Unknown/insufficient state.

### Drift

- all eight domains declared;
- one applicable watch positive/no-drift case;
- governed drift mismatch detected;
- omitted domain becomes blind spot;
- explicit non-applicable domain requires reason/evidence;
- no invented numeric threshold.

### Competence

- valid current competence;
- missing competence;
- expired competence;
- wrong subject/scope/domain competence;
- self-issued/unverifiable competence cannot improve quality.

### Independent challenge

- valid independent challenge;
- same-owner challenge rejected for independence-required relation;
- missing authorization rejected;
- expired challenge rejected;
- circular challenge rejected;
- challenge may reduce but not improve canonical Health truth.

### Restoration

- active loss;
- source reappearance remains pending;
- reassessment before restored observation rejected;
- same-owner reassessment rejected where independence required;
- unresolved blocker prevents independently reassessed state;
- valid independently reassessed state;
- no Authority restoration surface exists.

### Architecture

- zero-Application validity;
- no Application business semantics;
- no WP-06 source integration claim;
- no WP-07 persistence/events claim;
- no WP-08 Authority/Lifecycle enforcement;
- no Stage 8 Guardian/Safe State;
- no Stage 9 Recovery release;
- no Stage 13 governance/Monitor AI/evolution;
- no AWR-003/AWR-004/AWR-005 activation.

## 18. Requirement Trace

| Requirement family | WP-05 design disposition |
|---|---|
| AWR-001 missing/stale evidence / confidence reduction | explicit loss and effective-quality reduction, reusing WP-02 |
| AWR-001 contradiction visibility | reuse WP-02/WP-03 and preserve challenge conflict |
| AWR-001 known blind spots | explicit blind-spot runtime and Self Model correlation |
| AWR-001 current vs LastKnown | reuse WP-03; add LastKnown reliance eligibility only |
| AWR-001 drift | explicit eight-domain coverage and findings |
| AWR-001 independent evidence | structural independent-owner + evidence binding |
| AWR-001 honest uncertainty | no omitted-domain optimism, no LastKnown promotion |
| AWR-001 independent challengeability | authorized challenge/reassessment record |
| AWR-001 competence bounds | explicit competence declaration and fail-closed missing/expired behavior |
| SYS-008 Health evidence quality | reuse canonical Health assessment; WP-05 cannot improve it |
| CON-006 evidence quality effect | effective quality only preserves/reduces; no unrestricted FIT manufacture |
| VPL-005 REQ-001..017 relevant WP-05 semantics | full nine-class declaration, age/expiry, challenge, LastKnown and preservation semantics; later Authority/Guardian/Recovery execution deferred |

## 19. Explicit Non-Claims

```text
WP05_DOES_NOT_OWN_HEALTH_TRUTH = YES
WP05_DOES_NOT_OWN_FITNESS_PERMISSION = YES
WP05_DOES_NOT_GRANT_AUTHORITY = YES
WP05_DOES_NOT_RESTORE_AUTHORITY = YES
WP05_DOES_NOT_EXECUTE_RECOVERY = YES
WP05_DOES_NOT_COMMAND_GUARDIAN = YES
WP05_DOES_NOT_EXECUTE_LIFECYCLE = YES
WP05_DOES_NOT_IMPLEMENT_WP06_SOURCE_BINDING = YES
WP05_DOES_NOT_IMPLEMENT_WP07_PERSISTENCE_EVENTS = YES
WP05_DOES_NOT_IMPLEMENT_WP08_ENFORCEMENT = YES
WP05_DOES_NOT_CLOSE_VPL005_END_TO_END = YES
AWR003_004_005_ACTIVATED = NO
```

## 20. Pre-Executable Acceptance Gate

No WP-05 source implementation may begin until a fresh Architecture/Consistency and Red-Team review of this exact committed design returns no unresolved Critical/High/Medium finding and confirms:

- exact source alignment;
- no duplicate truth owner;
- no optimistic divergence from WP-02;
- correct `DELAYED` semantics;
- complete drift-domain coverage;
- competence fail-closed behavior;
- independent challenge separation;
- LastKnown non-promotion;
- source-reappearance non-restoration;
- WP-06/07/08/09 and Stage 8/9/13 boundaries preserved.

If the pre-executable review finds a material defect, this design must be revised and re-reviewed before implementation.

## 21. Design Verdict

```text
WP05_DESIGN_VERSION = V2
REUSE_OWNERSHIP_CENSUS = COMPLETE
TRUE_PREDECESSOR_DEFECT_FOUND = NO
DUPLICATE_HEALTH_EVALUATOR = PROHIBITED
DUPLICATE_SELF_MODEL = PROHIBITED
DUPLICATE_FITNESS_EVALUATOR = PROHIBITED
NINE_CLASS_EVIDENCE_LOSS = BOUNDED_ADDITIVE_EXTENSION
DRIFT_COMPETENCE_BLIND_SPOTS_CHALLENGE = GENUINELY_MISSING_AND_WP05_OWNED
SOURCE_AUTHENTICITY_EXACT_BINDING = PENDING_WP06
PERSISTENCE_EVENTS = PENDING_WP07
AUTHORITY_LIFECYCLE_PROTECTIVE_CONSUMPTION = PENDING_WP08
VPL005_END_TO_END = PENDING_WP09
SOURCE_IMPLEMENTATION_AUTHORIZED_BY_THIS_DOCUMENT = NO
NEXT_REQUIRED_ACTION = FRESH_PRE_EXECUTABLE_ARCHITECTURE_CONSISTENCY_AND_RED_TEAM
```
