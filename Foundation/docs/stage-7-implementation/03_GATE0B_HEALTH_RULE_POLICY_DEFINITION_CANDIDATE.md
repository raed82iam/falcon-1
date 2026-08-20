# Stage 7 - Gate 0B Health Rule Policy Definition Candidate

**Date:** 2026-08-12  
**Status:** `PROPOSED / SPECIFICATION_DEFINITION_REVIEW_ACTIVATION_GATE / OWNER APPROVAL REQUIRED`  
**Stage 7 Plan:** `v0.3 OWNER_ACCEPTED`  
**Stage 7 Implementation Authority:** `GRANTED`  
**Gate 0B Authority:** `GRANTED`  
**Runtime Policy Activation:** `NOT GRANTED BY THIS DOCUMENT`  
**WP-01 Production/Source Start:** `BLOCKED UNTIL GATE 0B GOVERNED ACTIVATION AND PLAN RECONCILIATION`  

## 1. Purpose

This candidate supplies the missing normative Health Rule policy required by Stage 7 Gate 0B before executable Health/Fitness rules may depend on policy values.

It is intentionally a definition candidate, not an active specification and not implementation authority. The current approved sources establish the Health/Fitness model but leave material policy meaning unresolved. The Owner-accepted Stage 7 plan therefore requires a stop at the `SPECIFICATION_DEFINITION_REVIEW_ACTIVATION_GATE` rather than invention inside source code.

This candidate is designed as the proposed policy basis for a coordinated successor update centered on SYS-008 and the minimum dependent documentary synchronization required by CON-006 and FDN-004.

## 2. Controlling Sources

- Falcon Vision and Constitution;
- `SYS-008 - Health Monitoring`;
- `AWR-001 v2.1 - Foundation Self-Awareness System`;
- `CON-006 v1.1 - Health and Fitness Contract`;
- `VPL-005 v1.1 - Health Evidence Loss Plan`;
- `FDN-004 v1.0 - Foundation Configuration Catalog`;
- `FDN-005 v1.0 - Foundation Protection and Release Control Matrix`;
- `TIM-001` active temporal policy and preserved v1.0 temporal profiles;
- Stage 7 Implementation Plan v0.3;
- Stage 7 Gate 0A Exact Code Reuse / Ownership Census;
- Stage 7 implementation authorization dated 2026-08-11.

## 3. Gate 0B Source Finding

The current effective corpus does not fully define the policy required by Gate 0B.

| Policy group | Current source result | Gate 0B classification |
|---|---|---|
| Freshness windows by subject/capability/Core component | SYS-008 explicitly leaves required freshness by Core component unresolved; FDN-004 declares a configuration key but no values/mapping | `MISSING_NORMATIVE_DEFINITION` |
| Health consequence classes | SYS-008 explicitly lists Health consequence classes as unresolved | `MISSING_NORMATIVE_DEFINITION` |
| Evidence requirements | Required evidence semantics exist, but not a deterministic minimum rule evidence set by subject/capability | `PARTIALLY_DEFINED` |
| Confidence/evidence-quality rules | Qualitative evidence-loss behavior exists, but exact deterministic qualification rules are incomplete | `PARTIALLY_DEFINED` |
| Critical dependency aggregation | Critical failure visibility invariants exist, but no complete deterministic aggregation policy exists | `PARTIALLY_DEFINED` |
| `RECOVERY_REQUIRED -> RESTRICTED / NOT_FIT` | CON-006 states `consequence-dependent` without the deciding policy | `MISSING_NORMATIVE_DEFINITION` |

Therefore:

`CURRENT_EFFECTIVE_GATE0B_POLICY = INCOMPLETE`

`SOURCE_CODE_POLICY_INVENTION = PROHIBITED`

## 4. Non-Negotiable Responsibility Boundaries

The following boundaries are mandatory and are part of this candidate policy:

```text
HEALTH = TECHNICAL OBSERVATION + TECHNICAL ASSESSMENT
HEALTH != AUTHORITY
HEALTH != GUARDIAN
HEALTH != LIFECYCLE
HEALTH != RECOVERY AUTHORITY
HEALTH != BUSINESS MEANING
FITNESS != AUTHORITY
FSA != GUARDIAN
```

Health Monitoring MAY:

- consume trustworthy technical observations;
- classify Health state;
- qualify evidence freshness and evidence quality;
- expose dependency condition and blind spots;
- publish attributable Health evidence and state changes;
- provide qualified evidence to governed consumers.

Health Monitoring SHALL NOT:

- issue Guardian commands;
- restrict, isolate, kill, stop, release, or revive a subject;
- grant or revoke authority;
- command Lifecycle transitions;
- declare recovery accepted or complete;
- replace Security, Evidence, State, Time, Resource, Guardian, Authority, Lifecycle, Recovery, or FSA truth owners.

Protective action remains owned by Guardian and other governed protective authorities under their own mandate. Health produces evidence; it does not become the protection plane.

## 5. Canonical Health Rule Definition

Every executable Health rule SHALL be declared before use and SHALL contain at minimum:

- `RuleId`;
- `RuleVersion`;
- `SubjectClass`;
- `SubjectIdentity` or governed identity selector;
- `CapabilityScope`;
- applicable Health dimensions: availability, correctness, integrity, performance, dependency;
- required evidence roles;
- authoritative evidence source identity/owner;
- independent evidence requirement where applicable;
- freshness profile;
- evidence-quality requirement;
- dependency set and dependency criticality;
- consequence class;
- contradiction treatment;
- accountable policy owner;
- governing authority;
- effective/expiry conditions;
- deterministic reason/result codes.

An undeclared runtime rule SHALL NOT infer a positive Health state.

## 6. Freshness Policy

### 6.1 General Rule

Freshness is evaluated per evidence relation, not by one global timer for all Foundation subjects.

For any evidence item, the effective permitted freshness SHALL be the strictest applicable bound among:

1. authoritative source validity or expiry;
2. the Health rule freshness profile;
3. an active authorized `falcon.health.freshness_window` value only when that value is stricter than the rule profile;
4. governing temporal/clock validity under TIM-001.

A configuration value SHALL NOT extend a normative Health freshness limit.

If required evidence has no valid freshness basis, the affected rule cannot produce `HEALTHY`.

### 6.2 Freshness Profiles

The proposed initial Foundation profiles are:

| Profile | Maximum evidence age | Intended use |
|---|---:|---|
| `HFP-CRITICAL` | 5 seconds | authority/trust-sensitive runtime condition, Health monitor availability, resource-pressure emergency visibility |
| `HFP-FAST` | 15 seconds | runtime/lifecycle/service-bus/FIL/event/dependency condition and FSA technical runtime condition |
| `HFP-STANDARD` | 60 seconds | state/persistence/evidence-path operational integrity and ordinary technical capability condition |
| `HFP-SLOW` | 300 seconds | backup/restore readiness, low-change infrastructure readiness and similar slow technical condition |
| `HFP-SOURCE_BOUND` | source/TIM validity controls | evidence with an explicit stronger governing validity/expiry model |
| `HFP-EVENT_BOUND` | event/version/change witness controls | immutable/versioned condition whose currentness is proven by an independently trustworthy change witness |

For `HFP-EVENT_BOUND`, loss of the change witness or loss of visibility makes currentness `UNKNOWN`. If an independently trustworthy change witness does not exist, the rule SHALL fall back to a time-bounded profile no looser than `HFP-SLOW`.

### 6.3 Initial Subject/Capability Mapping

| Subject / evidence family | Default freshness treatment |
|---|---|
| Health Monitoring runtime availability and publication continuity | `HFP-CRITICAL` |
| Authority Engine runtime availability / policy-evaluation availability evidence | `HFP-CRITICAL` |
| Security/trust/revocation evidence | strictest of `HFP-CRITICAL`, source validity and security policy |
| Stage 6 resource-pressure / exhaustion-risk signals | `HFP-CRITICAL` |
| FSA technical runtime availability and assessment-pipeline continuity | `HFP-FAST` |
| Lifecycle state observation | `HFP-FAST` |
| Service Bus / FIL / Event delivery operational condition | `HFP-FAST` |
| Critical dependency availability/compatibility observation | `HFP-FAST` unless a stricter source rule applies |
| Time/clock-quality evidence | `HFP-SOURCE_BOUND` under TIM-001; Health SHALL NOT weaken TIM-001 |
| State/persistence operational integrity | `HFP-STANDARD` |
| Evidence journal/audit-path operational integrity | `HFP-STANDARD`, or stricter when audit-critical policy requires |
| Backup/restore readiness evidence | `HFP-SLOW` |
| Configuration/baseline/document identity/integrity | `HFP-EVENT_BOUND` when independently witnessed; otherwise no looser than `HFP-SLOW` |

A specific Health rule MAY be stricter than this table. It SHALL NOT be looser without a separately governed policy decision.

## 7. Evidence Requirements

### 7.1 Minimum Evidence Set

A positive Health assessment requires all evidence declared `REQUIRED` by the rule. At minimum the assessment basis SHALL expose:

- exact subject identity;
- exact capability/scope;
- observation identity;
- observation time;
- assessment time;
- authoritative source identity and owner;
- evidence reference and provenance/integrity result;
- applicable source validity/expiry;
- clock-quality/temporal validity sufficient for the rule;
- rule identity/version;
- dependency evidence for every `REQUIRED` dependency;
- known contradictions and blind spots.

### 7.2 Evidence Roles

Health rules SHALL classify evidence as:

- `REQUIRED_PRIMARY`;
- `REQUIRED_INDEPENDENT`;
- `SUPPORTING`;
- `DIAGNOSTIC_ONLY`.

`SUPPORTING` or `DIAGNOSTIC_ONLY` evidence SHALL NOT substitute for missing required evidence.

### 7.3 Independent Evidence Rule

Independent evidence is mandatory when self-report alone could establish trust in the subject being assessed.

At minimum this applies to:

- Health Monitoring assessing its own availability/integrity-relevant operation;
- FSA technical Health assessment;
- any rule where the subject's own output is the material basis for declaring its Health;
- any `HC-TRUST_BLOCKING` rule whose positive result would otherwise depend only on the affected subject.

The subject's own output MAY be used as evidence, but SHALL NOT be the sole evidence when `REQUIRED_INDEPENDENT` applies.

## 8. Evidence Quality and Confidence Policy

The normative evidence-quality classes are:

- `EQ-SUFFICIENT`;
- `EQ-LIMITED`;
- `EQ-INSUFFICIENT`;
- `EQ-INVALID`.

### `EQ-SUFFICIENT`

All required evidence is present, fresh, attributable, integrity-valid, temporally valid, within competence, and free of unresolved material contradiction.

### `EQ-LIMITED`

Required evidence remains usable, but a non-required or explicitly degradable evidence relation is delayed, partially unavailable, lower quality, or otherwise bounded by a declared constraint.

### `EQ-INSUFFICIENT`

One or more required evidence relations is missing, stale, delayed beyond the rule's positive-decision bound, inaccessible, materially contradictory, partially visible where completeness is required, or outside demonstrated assessment competence.

### `EQ-INVALID`

Evidence is corrupted, provenance-invalid, integrity-invalid, or otherwise prohibited from reliance.

### 8.1 Health-State Qualification

- `HEALTHY` requires `EQ-SUFFICIENT`.
- `DEGRADED` may use `EQ-SUFFICIENT` or `EQ-LIMITED` only when a known bounded degradation is positively established.
- `UNHEALTHY` requires current positive evidence of a failed condition or required invariant. Mere absence of evidence is not sufficient to prove `UNHEALTHY`.
- `UNKNOWN` is mandatory when required current Health cannot be established because evidence is `EQ-INSUFFICIENT`.
- `EQ-INVALID` evidence SHALL be excluded from positive inference and preserved only as failure evidence.
- `NOT_APPLICABLE` requires a governed determination that the rule/dimension does not apply to the declared subject/capability scope.

Numeric or textual confidence recorded in existing contracts MAY carry additional detail, but it SHALL NOT upgrade a lower evidence-quality class into a more favorable Health state.

## 9. Health Consequence Classes

Health consequence classes describe the technical effect of a Health finding on Health/Fitness interpretation. They are not Guardian commands.

### `HC-OBSERVATION_ONLY`

The condition is material for visibility/history but does not by itself reduce the declared capability's technical fitness.

### `HC-DEGRADING`

The condition is a known bounded degradation. The affected Health dimension may be `DEGRADED`; technical fitness may remain usable only with explicit constraints.

### `HC-CAPABILITY_BLOCKING`

The condition prevents reliable use of the affected capability. A proven failed condition produces `UNHEALTHY`; inability to establish current required condition produces `UNKNOWN`. The affected capability cannot be projected as `FIT`.

### `HC-TRUST_BLOCKING`

Integrity, provenance, identity, authority/trust basis, or equivalent technical trust required for reliance has failed or cannot be established. The affected capability SHALL NOT be `FIT`. Proven trust failure maps toward `INTEGRITY_FAILURE`; inability to establish trust maps toward `UNKNOWN/UNAVAILABLE` as appropriate.

### `HC-RECOVERY_GATED`

The subject/capability requires governed recovery/reassessment before unrestricted reliance can return. Source reappearance alone is insufficient.

No Health consequence class authorizes restriction, isolation, kill, lifecycle transition, recovery execution, release, or authority change. Those remain separate governed decisions.

## 10. Critical Dependency Aggregation Policy

Every dependency relation SHALL be classified by the Health rule as:

- `REQUIRED`;
- `DEGRADABLE`;
- `INFORMATIONAL`.

### 10.1 Required Dependencies

For a `REQUIRED` dependency:

- dependency `UNHEALTHY` SHALL prevent aggregate `HEALTHY` and produce aggregate `UNHEALTHY` for the dependent capability unless a more specific governed rule requires `UNKNOWN` because the consequence itself cannot be established;
- dependency `UNKNOWN` SHALL produce aggregate `UNKNOWN` for the dependent capability;
- dependency `DEGRADED` SHALL produce at least aggregate `DEGRADED`, unless the declared consequence class makes the degraded condition capability-blocking;
- a failed required invariant SHALL never be hidden by healthy sibling dependencies.

### 10.2 Degradable Dependencies

A `DEGRADABLE` dependency may reduce aggregate Health to `DEGRADED` while the capability remains boundedly usable only when:

- the rule explicitly declares the degraded operating mode;
- independent evidence proves the capability does not require the failed portion for that bounded mode;
- constraints are explicit and attributable.

If those conditions cannot be proven, the aggregate becomes `UNKNOWN` or `UNHEALTHY` according to the evidence and failure state.

### 10.3 Informational Dependencies

`INFORMATIONAL` dependencies remain visible but do not affect aggregate Health unless a later governed rule changes their classification.

### 10.4 Aggregation Invariants

- no averaging of Health states;
- no majority vote can erase a critical failure;
- contradiction on required evidence produces explicit uncertainty;
- one unaffected capability MAY remain independently assessable only when dependency independence is proven by governed dependency truth and fresh evidence;
- aggregate results SHALL retain the identity of the dependency responsible for reduction.

## 11. `RECOVERY_REQUIRED` Mapping Policy

Default rule:

```text
RECOVERY_REQUIRED -> NOT_FIT
```

`RECOVERY_REQUIRED -> RESTRICTED` is permitted only when ALL of the following are true:

1. the recovery-required fault is technically isolated to a declared scope;
2. the requested capability does not depend on the affected subject/path for the restricted mode;
3. fresh independent evidence proves the unaffected capability remains technically usable;
4. no unresolved integrity, provenance, identity, authority/trust, or cross-scope contamination affects that capability;
5. the Health/Fitness rule explicitly predeclares the permitted restricted mode and its constraints;
6. the restricted result has an expiry and remains subject to new assessment;
7. source reappearance alone does not restore `FIT`;
8. any required recovery acceptance/release remains owned by the separately governed Recovery/Release path.

If any condition is absent or unknown:

`RECOVERY_REQUIRED -> NOT_FIT`

## 12. FSA as a Health Subject

Stage 7 Health Monitoring SHALL treat FSA as an eligible Foundation technical Health subject without becoming FSA's independent integrity-governance monitor.

The Stage 7 FSA technical Health profile may assess:

- runtime availability;
- expected assessment/publication continuity;
- input-evidence ingestion continuity;
- Self Model update freshness;
- dependency availability;
- resource pressure affecting operation;
- persistence/evidence-publication condition;
- configuration/baseline identity/integrity evidence already available from accepted Foundation truth owners;
- explicit technical blind spots and monitoring visibility loss.

FSA self-report SHALL NOT be sufficient by itself for a positive FSA Health assessment where independent evidence is required.

Stage 7 Health Monitoring SHALL NOT use this scope to implement:

- Monitor AI;
- FSA goal/purpose integrity investigation;
- behavioral intent/deception judgment;
- FSA containment/kill/release authority;
- Owner/FSA governance workflow;
- Stage 13 self-maintenance/evolution control plane.

Those remain outside Stage 7 and are preserved for their separately governed stage/scope.

## 13. Health Monitoring Self-Health

Health Monitoring itself SHALL be observable as a Health subject.

At minimum, independent evidence SHALL be able to establish:

- Health evaluator runtime availability;
- observation-ingestion continuity;
- rule-registry identity/version;
- event/evidence publication continuity;
- time/evidence dependencies;
- loss of visibility.

Health Monitoring SHALL NOT declare itself `HEALTHY` solely from its own self-report.

Loss of Health Monitoring visibility SHALL become explicit `UNKNOWN` for affected Health knowledge as required by SYS-008.

## 14. Configuration Interaction

The existing `falcon.health.freshness_window` key is preserved as an authorized Health-owned duration input.

Under this candidate policy its role is constrained to:

- an optional global/instance ceiling that may make an applicable rule stricter;
- never extending a rule's normative maximum age;
- never replacing source-specific validity/expiry;
- never bypassing TIM-001 clock/trust requirements.

If implementation requires independently configurable per-rule/per-profile values rather than a governed policy registry, FDN-004 SHALL receive a separately reviewed bounded successor update before such configuration keys are introduced.

No undeclared configuration key may be invented in source code.

## 15. Proposed Documentary Activation Package

The smallest coherent activation package is:

1. `SYS-008` successor - add the Health Rule policy in Sections 5 through 13 of this candidate and close the two current unresolved matters;
2. `CON-006` successor - clarify that `RECOVERY_REQUIRED` is resolved by the activated Health/Fitness policy with default `NOT_FIT` and the explicit restricted-mode exception conditions above;
3. `FDN-004` successor clarification only if required to bind the global freshness key semantics stated in Section 14;
4. traceability/verification synchronization so VPL-005 uses the activated rule identities/thresholds without becoming a truth source;
5. Stage 7 plan reconciliation confirming Gate 0B is closed only after the normative package is activated.

No new independent Guardian policy is created by this package.

## 16. Required Verification Before Activation

The candidate SHALL be challenged against at least:

- stale required evidence remaining `HEALTHY`;
- missing evidence becoming `UNHEALTHY` without proof rather than `UNKNOWN`;
- corrupted/provenance-invalid evidence being used positively;
- Health self-report serving as sole proof of Health Monitoring Health;
- FSA self-report serving as sole proof of FSA Health;
- FSA technical Health scope expanding into Stage 13 Monitor AI/governance;
- Health issuing or implying Guardian commands;
- Guardian protection classes being silently reused as Health consequence classes;
- a required unhealthy dependency hidden by healthy siblings;
- a required unknown dependency averaged into a positive aggregate;
- unrelated capability incorrectly blocked despite proven dependency independence;
- `RECOVERY_REQUIRED` silently becoming `RESTRICTED` without all exception conditions;
- restored evidence source silently restoring `FIT`;
- `falcon.health.freshness_window` weakening a normative rule;
- source/TIM expiry being ignored by a looser Health timer;
- zero-Application Foundation incorrectly treated as unhealthy;
- Application business meaning leaking into Health rules.

## 17. Gate 0B Candidate Disposition

```text
GATE0B_SOURCE_REVIEW = COMPLETE
MISSING_NORMATIVE_DEFINITION_CONFIRMED = YES
HEALTH_RULE_POLICY_CANDIDATE = COMPLETE
FRESHNESS_POLICY = PROPOSED
HEALTH_CONSEQUENCE_CLASSES = PROPOSED
EVIDENCE_REQUIREMENTS = PROPOSED
EVIDENCE_QUALITY_POLICY = PROPOSED
CRITICAL_DEPENDENCY_AGGREGATION = PROPOSED
RECOVERY_REQUIRED_MAPPING = PROPOSED
FSA_TECHNICAL_HEALTH_SUBJECT = PROPOSED_WITH_STAGE13_BOUNDARY
HEALTH_GUARDIAN_SEPARATION = EXPLICIT
SPECIFICATION_DEFINITION_REVIEW_ACTIVATION_GATE = OPEN
OWNER_APPROVAL_REQUIRED = YES
GATE0B_RUNTIME_ACTIVATION = NOT_YET
WP01_SOURCE_IMPLEMENTATION = BLOCKED
STAGE8_AUTHORITY = NOT_GRANTED
STAGE13_AUTHORITY = NOT_GRANTED
```

This candidate SHALL receive Architecture/Consistency review and fresh Red-Team review before any Owner activation decision.
