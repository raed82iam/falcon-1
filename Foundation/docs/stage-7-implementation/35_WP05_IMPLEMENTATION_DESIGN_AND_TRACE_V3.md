# Stage 7 WP-05 — Evidence Quality, Drift, Blind Spots and Independent Challenge

**Design and Trace:** V3  
**Date:** 2026-08-13  
**Status:** `PRE_EXECUTABLE_DESIGN_CANDIDATE / V2 FINDINGS REMEDIATED`  
**Foundation Branch:** `foundation-development`  
**Supersedes for implementation review:** `33_WP05_IMPLEMENTATION_DESIGN_AND_TRACE_V2.md`  
**Remediates:** `34_WP05_PRE_EXECUTABLE_ARCHITECTURE_CONSISTENCY_AND_RED_TEAM_V2.md`  
**Starting Accepted Closure State:** Gate 0A through WP-04 = `ACCEPTED_AND_CLOSED`  
**WP-05 Owner Closure:** `NOT_YET`

## 1. Purpose

Define the minimum bounded implementation for Stage 7 WP-05 required to realize current effective evidence-quality, evidence-loss, drift, blind-spot, competence and independent-challenge semantics without duplicating accepted WP-02 Health truth, WP-03 Self Model truth, WP-04 Technical Fitness truth, or pulling WP-06 through WP-10 and later-stage authority into WP-05.

V3 incorporates every finding from the V2 pre-executable Red-Team. No source implementation is created or authorized by this document itself.

## 2. Controlling Sources

Implementation remains bound to current effective:

- Falcon Vision;
- Falcon Constitution;
- AWR-001 v2.1;
- SYS-008 v1.1;
- CON-006 v1.2;
- VPL-005 v1.1;
- accepted Stage 7 Plan v0.3 implementation sequence;
- Gate 0A reuse/ownership census;
- Gate 0B policy/freshness feasibility evidence;
- accepted and closed Stage 7 WP-01 through WP-04 runtime/evidence.

No AWR-003/AWR-004/AWR-005 body is inferred or activated.

## 3. Reuse and Ownership

### 3.1 Reuse as-is

WP-05 reuses without reinterpretation:

- WP-01 `HealthState`, `TechnicalFitnessState`, `FitnessProjectionResult`, `EvidenceQuality`, canonical assessment identities and validators;
- WP-02 `HealthRuleDefinition`, `HealthEvidenceRequirement`, evidence roles, canonical Health assessment, freshness/provenance/integrity/clock/visibility/contradiction/fail-closed behavior;
- WP-03 Self Model assertion kinds, Current/LastKnown/Expected/Desired/Historical views, contradiction and BlindSpotCondition areas, canonical assertion/snapshot identity;
- WP-04 deterministic Technical Fitness and CON-006 projection, including the rule that limited/insufficient evidence cannot manufacture unrestricted `FIT`.

### 3.2 Bounded additive extension

WP-05 adds only the missing explicit evidence-loss/challenge/awareness semantics. It SHALL NOT create a second Health evaluator, second Self Model, second Fitness evaluator, Authority engine, Guardian, Lifecycle, Recovery, persistence engine or event engine.

### 3.3 Genuinely missing and WP-05-owned

- complete nine-class VPL-005 loss representation;
- explicit availability/non-loss coverage declaration;
- acquisition/arrival state for delayed evidence;
- exact WP-02 Health-requirement relation binding;
- eight-domain drift coverage/findings;
- competence declarations;
- known blind-spot runtime including affected-authority impact evidence;
- authorized independent challenge/reassessment;
- source-reappearance/restoration-gate evidence;
- WP-05 verifier and architecture guard.

### 3.4 Deferred

- exact Stage 3..6/security predecessor truth/source-authenticity integration: WP-06;
- persistence/reconstruction/event publication: WP-07;
- Authority/Lifecycle/protective-consumer enforcement: WP-08;
- full VPL-005 end-to-end validation: WP-09;
- Guardian/Safe State: Stage 8;
- Recovery execution/release: Stage 9;
- FSA/Owner governance, Monitor AI and evolution control plane: Stage 13.

## 4. Predecessor Classification

```text
TRUE_PREDECESSOR_DEFECT_FOUND = NO
WP01_TO_WP04_REOPEN_REQUIRED = NO
```

The Evidence-journal bounded-probe requirement remains rule-activation-specific. WP-05 shall not activate a periodic full-journal Health probe and does not require predecessor semantic repair.

## 5. Minimal Source Change Surface

Create:

- `src/Foundation.HealthFitness/HealthEvidenceQualityRuntime.cs`
- `src/Foundation.SelfAwareness/EvidenceAwarenessRuntime.cs`
- `verification/Falcon.Stage7.WP05.Verifier/`
- `tests/Falcon.Foundation.Architecture.Tests/Stage7Wp05ArchitectureGuard.cs`

Add only controlled verifier solution membership required by the established Stage 7 verification pattern.

No Application/Web/reference path change is permitted.

## 6. Canonical Evidence Relation Binding

### 6.1 Exact binding fields

Every WP-05 evidence relation SHALL bind:

- relation/assessment identity;
- `HealthRequirementId`;
- `HealthRuleId`;
- `HealthRuleVersion`;
- subject;
- capability;
- scope;
- declared `HealthEvidenceRole`;
- declared source ID;
- declared source owner;
- evidence reference;
- observation time;
- assessment time;
- source expiry where available;
- acquisition/arrival state;
- evidence-loss state;
- canonical WP-02 Health assessment reference;
- bounded `EvidenceQuality`;
- reason.

### 6.2 Mandatory WP-02 declaration cross-check

Before producing any WP-05 quality, challenge or restoration result, runtime SHALL receive the canonical `HealthRuleDefinition` used by the referenced WP-02 assessment and validate that:

1. Health rule ID/version match the referenced `CanonicalHealthAssessment`;
2. subject and capability match;
3. `HealthRequirementId` exists exactly once in `HealthRuleDefinition.EvidenceRequirements`;
4. role matches the declared WP-02 requirement role;
5. source ID matches;
6. source owner matches;
7. the canonical Health assessment reference/identity is structurally valid and rule-bound.

Any omission, duplicate or mismatch fails closed.

This cross-check proves Stage 7 internal Health-requirement relation identity. It does not claim WP-06 predecessor source-authenticity integration.

## 7. Evidence-Loss Taxonomy

Canonical VPL-005 loss classes are exactly:

- `MISSING`;
- `STALE`;
- `DELAYED`;
- `CONTRADICTORY`;
- `UNVERIFIABLE`;
- `INACCESSIBLE`;
- `CORRUPTED`;
- `PROVENANCE_FAILURE`;
- `PARTIAL_VISIBILITY`.

An explicit non-loss state `AVAILABLE` or equivalent is permitted only to prove coverage completeness. It is not a tenth VPL-005 loss class.

Silence, omission or absence of a relation declaration SHALL NOT imply `AVAILABLE`.

## 8. Delayed, Clock and Future Evidence

`DELAYED` requires explicit acquisition/arrival state showing expected evidence is pending before governed arrival/expiry resolution.

```text
FUTURE_DATED != DELAYED
```

Future-dated evidence cannot support current positive reliance, cannot extend freshness and must fail structural/current verification or be represented as unverifiable/insufficient according to the exact condition.

Delayed evidence remains pending until arrival or expiry and cannot become a positive Health claim.

## 9. Evidence-Loss Consequences

For required evidence relations:

- `MISSING` => no positive Health/Fitness inference;
- `STALE` => not fresh, no positive current inference;
- `DELAYED` => pending, no positive conclusion;
- `CONTRADICTORY` => explicit contradiction, unresolved;
- `UNVERIFIABLE` => insufficient;
- `INACCESSIBLE` => insufficient for unrestricted reliance;
- `CORRUPTED` => invalid for reliance;
- `PROVENANCE_FAILURE` => invalid relation;
- `PARTIAL_VISIBILITY` => incomplete/insufficient.

Optional/supporting evidence may reduce to bounded `Limited` only when existing Health semantics permit. It never repairs a required relation.

## 10. Effective Quality Cannot Improve WP-02

Strength ordering:

```text
Sufficient > Limited > Insufficient > Invalid
```

Effective WP-05 quality is the weakest applicable input among:

```text
CANONICAL_WP02_HEALTH_QUALITY
WP05_LOSS_STATUS_QUALITY
WP05_COMPETENCE_QUALITY
WP05_CHALLENGE_QUALITY
```

Invariant:

```text
WP05_EFFECTIVE_QUALITY_CAN_NEVER_BE_STRONGER_THAN_WP02 = TRUE
```

A disagreement with canonical WP-02 truth is retained as contradiction/blind-spot evidence and resolves to the less optimistic result. WP-05 never overwrites WP-02 Health.

## 11. Current and LastKnown

A previously trustworthy state may remain historically preserved as LastKnown/Historical.

Operational LastKnown reliance requires all:

- explicit governing policy reference allowing fallback;
- authoritative source identity/owner;
- original evidence reference;
- original observation/effective time;
- explicit age;
- explicit freshness/expiry limit;
- no unresolved contradiction/provenance/integrity failure;
- current time within the permitted relation.

Otherwise:

```text
LAST_KNOWN_RELIANCE = INELIGIBLE
```

LastKnown never becomes Current. Required loss of current awareness requires explicit Current Unknown/insufficient representation while preserved LastKnown may coexist only as history/fallback evidence under policy.

## 12. Drift Coverage

### 12.1 Domains

Every WP-05 awareness evaluation SHALL cover exactly these eight AWR-001 drift domains:

1. `DATA`;
2. `FOUNDATION_MODEL` limited to Foundation-owned/applicable models;
3. `BEHAVIOR`;
4. `CONFIGURATION`;
5. `AUTHORITY`;
6. `OBJECTIVE_PURPOSE_IDENTITY`;
7. `DEPENDENCY`;
8. `OWN_ASSESSMENT`.

### 12.2 Canonical drift coverage declaration

Every domain, whether applicable or not applicable, SHALL have an explicit declaration binding:

- declaration identity;
- rule ID/version;
- governing authority identity;
- evaluator identity;
- subject;
- capability/scope;
- domain;
- applicability state;
- comparison/reference-basis identity where applicable;
- evidence reference;
- reason;
- effective time;
- expiry.

A missing domain declaration becomes a known blind spot and insufficient awareness. Silence never means no drift.

`NOT_APPLICABLE` requires explicit evidence-bound reason and governing identity. Runtime cannot mark a domain non-applicable by bare caller assertion.

### 12.3 Materiality

No numeric threshold is invented by source code. Materiality is evaluated against the governed rule/reference basis supplied by the declaration. A finding records mismatch/change against that basis.

## 13. Competence Bounds

Every applicable coverage/evaluation claim SHALL bind a current competence declaration containing:

- evaluator identity;
- evaluator owner;
- domain;
- Self Model/governed assessment area;
- subject;
- scope;
- competence evidence reference;
- competence evidence source/owner;
- governing rule ID/version and authority;
- effective time;
- expiry.

Missing, expired, mismatched, unverifiable or circular self-issued competence evidence cannot support positive competence.

```text
INSUFFICIENT_COMPETENCE -> KNOWN_BLIND_SPOT
INSUFFICIENT_COMPETENCE -> POSITIVE_COVERAGE_PROHIBITED
```

WP-05 does not create competence authority.

## 14. Known Blind Spots and Authority Impact Evidence

### 14.1 Blind-spot triggers

Known blind spots include at minimum:

- omitted required coverage;
- missing/expired competence;
- inaccessible/partial visibility;
- unresolved contradiction;
- required independent evidence unavailable;
- unresolved challenge;
- exact source authenticity pending where it is material to the claim.

### 14.2 Blind-spot record

Every blind-spot assessment SHALL bind:

- blind-spot identity;
- subject;
- capability;
- scope/domain;
- reason;
- evidence references;
- observation/assessment time;
- expiry if time-bounded;
- affected requested authority level/class/context identity;
- authority-impact classification.

Allowed authority-impact classifications are bounded evidence semantics only, for example:

- `NONE_DECLARED` only when a governing rule explicitly establishes no authority dependency;
- `POSITIVE_INFERENCE_BLOCKED`;
- `REQUIRES_GOVERNED_REASSESSMENT`.

`NONE_DECLARED` cannot be the default and cannot be inferred from silence.

The authority-impact field does not grant, revoke, restrict or restore Authority. It records technical impact evidence for later AUT-001/WP-08 governed consumption.

## 15. Independent Evidence and Authorized Challenge

Where independent evidence is required, challenger/reassessor evidence owner must differ from the challenged source owner.

Every challenge SHALL bind:

- challenge identity;
- challenged relation/assessment identity;
- challenger identity and owner;
- authorization evidence reference;
- independent evidence reference;
- source-authenticity status;
- challenge observation/assessment time;
- result;
- reason;
- expiry where applicable.

Same-owner challenge where independence is required, missing/expired authorization, circular challenge, future-dated evidence or malformed binding fails closed.

Challenge may confirm a bounded assessment when independently supported, reduce quality, expose contradiction/blind spot, or require reassessment. It cannot rewrite WP-02 Health, manufacture sufficient Health evidence, grant Fitness/Authority, or release Recovery/Isolation.

Exact predecessor/source authenticity integration is WP-06-owned. Where not yet established, the record uses an explicit state equivalent to:

```text
SOURCE_AUTHENTICITY = PENDING_WP06
```

That state can never improve evidence quality.

## 16. Restoration Gate

Canonical restoration states shall be equivalent to:

- `NO_PRIOR_LOSS`;
- `LOSS_ACTIVE`;
- `SOURCE_REAPPEARED_PENDING_INDEPENDENT_REASSESSMENT`;
- `INDEPENDENTLY_REASSESSED`.

WP-05 has no `AUTHORITY_RESTORED` state.

`INDEPENDENTLY_REASSESSED` requires all:

1. exact WP-02 Health requirement/rule binding from Section 6 is valid;
2. no active required loss remains for that exact requirement relation;
3. current canonical WP-02 Health evidence quality is `Sufficient`;
4. WP-05 relation/loss status quality is `Sufficient`;
5. required competence is current/sufficient;
6. fresh independent reassessment occurs after the restored evidence observation;
7. reassessor owner differs from restored source owner where independence is required;
8. current authorization evidence is bound;
9. no unresolved contradiction, provenance, integrity, competence, visibility or challenge blocker remains.

Source reappearance alone remains pending.

`INDEPENDENTLY_REASSESSED` means only WP-05 evidence-awareness restoration criteria are satisfied. It does not restore Authority, Recovery acceptance, release, Lifecycle state or Guardian state.

## 17. Self Model Correlation

WP-05 may produce governed inputs/assertions suitable for WP-03 projection, including:

- Current Unknown/insufficient awareness;
- retained LastKnown history with reliance eligibility;
- BlindSpotCondition;
- ContradictionCondition;
- competence/challenge/evidence-quality references.

WP-05 never mutates predecessor truth and Self Model remains interpretation only.

## 18. Determinism and Canonical Identity

All material WP-05 records use deterministic canonical identity from ordered canonical fields.

Input ordering must not change equivalent result/identity. Mutation of material relation, source owner, rule, requirement, loss, domain, competence, challenge, authority-impact or restoration field must change identity or fail validation.

## 19. Fail-Closed Matrix

WP-05 SHALL fail closed for at least:

- required evidence relation omitted;
- Health requirement/rule/source binding mismatch;
- duplicate Health requirement binding;
- drift domain omitted;
- non-applicable drift domain without governing identity/evidence/reason;
- malformed identity or enum;
- future-dated positive evidence;
- expired evidence claimed fresh;
- delayed inferred from future timestamp only;
- source-owner mismatch;
- missing/expired/mismatched competence;
- blind spot without affected-authority impact evidence;
- `NONE_DECLARED` authority impact used without explicit governing basis;
- self-challenge where independence required;
- challenge authorization missing/expired;
- LastKnown treated as Current;
- Current positive state while active required relation loss exists;
- source reappearance treated as restoration;
- reassessment not after restored observation;
- WP-05 quality stronger than WP-02 canonical quality;
- challenge used to repair canonical Health truth;
- any attempt to grant Authority, command Guardian, transition Lifecycle or release Recovery.

## 20. Verification Requirements

The WP-05 verifier SHALL cover:

### Relation binding

- exact valid WP-02 Health rule/requirement/assessment binding;
- missing requirement;
- wrong role/source/source owner;
- wrong rule version;
- wrong subject/capability;
- fabricated relation rejected.

### Nine loss classes

- valid `AVAILABLE` required baseline;
- every VPL-005 loss class;
- required vs optional/supporting behavior;
- stale age/expiry;
- delayed pending -> arrival/expiry;
- future timestamp not delayed;
- omitted required relation fail closed;
- corrupted/provenance failure invalid;
- partial visibility incomplete.

### No optimism

- WP-02 Insufficient cannot become Sufficient;
- WP-02 Invalid cannot be repaired;
- disagreement stays explicit.

### LastKnown

- policy-eligible fallback;
- no-policy ineligible;
- expiry ineligible;
- never Current;
- current loss coexists with explicit current unknown.

### Drift

- all eight domains;
- applicable watched case;
- drift detected against governed basis;
- omitted domain blind spot;
- non-applicable without governing identity rejected;
- evidence-bound non-applicable accepted;
- no invented threshold.

### Competence

- current valid competence;
- missing/expired/mismatched competence;
- wrong domain/subject/scope;
- circular self-issued/unverifiable competence rejected for positive claim.

### Blind spots

- affected authority context required;
- positive inference blocked classification;
- reassessment-required classification;
- `NONE_DECLARED` requires governing basis;
- blind spot cannot grant/revoke Authority.

### Challenge

- valid independent challenge;
- same-owner rejection where independence required;
- missing/expired authorization;
- circular challenge;
- challenge reduction allowed;
- challenge cannot improve canonical WP-02 Health.

### Restoration

- active loss;
- source reappearance remains pending;
- wrong requirement/rule relation rejected;
- reassessment before restored observation rejected;
- same-owner reassessment rejected where required;
- unresolved blocker prevents completion;
- valid independently reassessed state;
- no Authority restoration surface.

### Architecture

- zero-Application validity;
- no Application business semantics/references;
- no WP-06 predecessor source-integration claim;
- no WP-07 persistence/event duplication;
- no WP-08 Authority/Lifecycle enforcement;
- no Stage 8 Guardian/Safe State;
- no Stage 9 Recovery release;
- no Stage 13 governance/Monitor AI/evolution;
- no AWR-003/AWR-004/AWR-005 activation.

## 21. Requirement Trace

| Requirement | V3 disposition |
|---|---|
| AWR-001-REQ-003 | evidence loss reduces quality, no presumed readiness |
| AWR-001-REQ-004 | contradiction remains explicit |
| AWR-001-REQ-005 | blind spot includes affected-authority impact evidence without deciding Authority |
| AWR-001-REQ-012 | LastKnown remains distinct from Current |
| AWR-001-REQ-013 | all eight drift domains explicitly covered |
| AWR-001-REQ-014 | independent evidence required relation structurally separated |
| AWR-001-REQ-018 | omitted/unknown conditions cannot manufacture precision |
| AWR-001-REQ-019 | authorized independent challenge/reassessment supported |
| AWR-001-REQ-020 | competence-bound assessment fails closed when insufficient |
| SYS-008 | WP-02 remains Health truth owner; exact rule/requirement binding added |
| CON-006 | evidence quality only preserves/reduces Fitness support and grants no authority |
| VPL-005 | nine classes, age/expiry, LastKnown, challenge, restoration gating represented; later enforcement remains deferred |

## 22. V2 Finding Closure Trace

```text
H-01_BLIND_SPOT_AUTHORITY_IMPACT = REMEDIATED_BY_SECTION_14
H-02_EXACT_WP02_REQUIREMENT_BINDING = REMEDIATED_BY_SECTION_6_AND_16
M-01_NON_APPLICABLE_DRIFT_GOVERNING_IDENTITY = REMEDIATED_BY_SECTION_12
```

## 23. Explicit Non-Claims

```text
WP05_OWNS_HEALTH_TRUTH = NO
WP05_OWNS_FITNESS_PERMISSION = NO
WP05_GRANTS_OR_RESTORES_AUTHORITY = NO
WP05_COMMANDS_GUARDIAN = NO
WP05_EXECUTES_LIFECYCLE = NO
WP05_EXECUTES_RECOVERY_RELEASE = NO
WP05_IMPLEMENTS_WP06_SOURCE_INTEGRATION = NO
WP05_IMPLEMENTS_WP07_PERSISTENCE_EVENTS = NO
WP05_IMPLEMENTS_WP08_ENFORCEMENT = NO
WP05_CLOSES_VPL005_END_TO_END = NO
AWR003_004_005_ACTIVATED = NO
```

## 24. Pre-Executable Gate

No source implementation may begin until a fresh Architecture/Consistency and Red-Team review of this exact committed V3 design returns no unresolved Critical/High/Medium finding.

## 25. Design Verdict

```text
WP05_DESIGN_VERSION = V3
V2_FINDINGS_REMEDIATED = YES
TRUE_PREDECESSOR_DEFECT_FOUND = NO
REUSE_OWNERSHIP_CENSUS = COMPLETE
NEXT_REQUIRED_ACTION = FRESH_PRE_EXECUTABLE_ARCHITECTURE_CONSISTENCY_AND_RED_TEAM_V3
SOURCE_IMPLEMENTATION_STARTED = NO
```
