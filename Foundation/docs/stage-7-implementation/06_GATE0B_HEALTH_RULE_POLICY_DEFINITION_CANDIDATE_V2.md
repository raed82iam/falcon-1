# Stage 7 - Gate 0B Health Rule Policy Definition Candidate V2

**Date:** 2026-08-12  
**Status:** `PROPOSED / SPECIFICATION_DEFINITION_REVIEW_ACTIVATION_GATE / OWNER APPROVAL REQUIRED`  
**Supersedes for review:** `03_GATE0B_HEALTH_RULE_POLICY_DEFINITION_CANDIDATE.md`  
**Revision Basis:** `05_GATE0B_RED_TEAM_V1.md`  
**Stage 7 Plan:** `v0.3 OWNER_ACCEPTED`  
**Stage 7 Implementation Authority:** `GRANTED`  
**Gate 0B Authority:** `GRANTED`  
**Runtime Policy Activation:** `NOT GRANTED BY THIS DOCUMENT`  
**WP-01 Production/Source Start:** `BLOCKED UNTIL GATE 0B GOVERNED ACTIVATION AND PLAN RECONCILIATION`  

## 1. Purpose

This V2 candidate supplies the missing normative Health Rule policy required by Stage 7 Gate 0B before executable Health/Fitness rules may depend on policy values.

It is a definition candidate only. It is not an active specification, it does not close Gate 0B, and it does not authorize WP-01 source implementation.

V2 incorporates all mandatory remediations from Gate 0B Red-Team V1, including explicit prohibition of circular positive proof, freshness-source feasibility proof, mandatory FDN-004 synchronization, and hard exclusions on `HC-OBSERVATION_ONLY`.

## 2. Controlling Sources

- Falcon Vision and Constitution;
- `SYS-008 - Health Monitoring`;
- `AWR-001 v2.1 - Foundation Self-Awareness System`;
- `CON-006 v1.1 - Health and Fitness Contract`;
- `VPL-005 v1.1 - Health Evidence Loss Plan`;
- `FDN-004 v1.0 - Foundation Configuration Catalog`;
- `FDN-005 v1.0 - Foundation Protection and Release Control Matrix`;
- active `TIM-001` temporal policy and preserved temporal profiles;
- Stage 7 Implementation Plan v0.3;
- Stage 7 Gate 0A Exact Code Reuse / Ownership Census;
- Stage 7 implementation authorization dated 2026-08-11.

## 3. Gate 0B Source Finding

| Policy group | Current source result | Gate 0B classification |
|---|---|---|
| Freshness windows by subject/capability/Core component | SYS-008 explicitly leaves required freshness by Core component unresolved; FDN-004 declares a key but no values/mapping | `MISSING_NORMATIVE_DEFINITION` |
| Health consequence classes | SYS-008 explicitly lists Health consequence classes as unresolved | `MISSING_NORMATIVE_DEFINITION` |
| Evidence requirements | Evidence semantics exist but no complete deterministic rule-evidence set exists | `PARTIALLY_DEFINED` |
| Confidence/evidence-quality rules | Qualitative behavior exists but deterministic qualification rules are incomplete | `PARTIALLY_DEFINED` |
| Critical dependency aggregation | Critical-failure visibility invariants exist but no complete aggregation policy exists | `PARTIALLY_DEFINED` |
| `RECOVERY_REQUIRED -> RESTRICTED / NOT_FIT` | CON-006 states `consequence-dependent` without the deciding rule | `MISSING_NORMATIVE_DEFINITION` |

Therefore:

```text
CURRENT_EFFECTIVE_GATE0B_POLICY = INCOMPLETE
SOURCE_CODE_POLICY_INVENTION = PROHIBITED
```

## 4. Non-Negotiable Responsibility Boundaries

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
- expose dependency condition, contradiction and blind spots;
- publish attributable Health evidence and state changes;
- provide qualified evidence to governed consumers.

Health Monitoring SHALL NOT:

- issue Guardian commands;
- restrict, isolate, kill, stop, release, or revive a subject;
- grant or revoke authority;
- command Lifecycle transitions;
- declare recovery accepted or complete;
- replace Security, Evidence, State, Time, Resource, Guardian, Authority, Lifecycle, Recovery, or FSA truth owners.

Protective enforcement remains outside Health Monitoring.

## 5. Canonical Health Rule Definition

Every executable Health rule SHALL be declared before use and SHALL contain at minimum:

- `RuleId`;
- `RuleVersion`;
- `SubjectClass`;
- exact subject identity or governed identity selector;
- `CapabilityScope`;
- applicable dimensions: availability, correctness, integrity, performance, dependency;
- required evidence roles;
- authoritative evidence source identity/owner;
- independent evidence requirement where applicable;
- freshness profile;
- evidence-quality requirement;
- dependency set and dependency criticality;
- Health consequence class;
- contradiction treatment;
- accountable owner;
- governing authority;
- effective/expiry conditions;
- deterministic reason/result codes.

An undeclared runtime rule SHALL NOT infer a positive Health state.

## 6. Freshness Policy

### 6.1 Strictest-Bound Rule

Freshness is evaluated per evidence relation.

Effective permitted freshness SHALL be the strictest applicable bound among:

1. authoritative source validity or expiry;
2. the rule freshness profile;
3. an active authorized `falcon.health.freshness_window` value only when it is stricter than the rule profile;
4. governing temporal/clock validity under TIM-001.

No configuration value may extend a normative Health freshness limit.

If required evidence has no valid freshness basis, the affected rule cannot produce `HEALTHY`.

### 6.2 Proposed Initial Freshness Profiles

| Profile | Maximum evidence age | Intended use |
|---|---:|---|
| `HFP-CRITICAL` | 5 seconds | authority/trust-sensitive runtime condition, Health-monitor availability, resource-pressure emergency visibility |
| `HFP-FAST` | 15 seconds | runtime/lifecycle/service-bus/FIL/event/dependency condition and FSA technical runtime condition |
| `HFP-STANDARD` | 60 seconds | state/persistence/evidence-path operational integrity and ordinary technical capability condition |
| `HFP-SLOW` | 300 seconds | backup/restore readiness and slow-changing technical readiness |
| `HFP-SOURCE_BOUND` | source/TIM validity controls | evidence governed by explicit stronger validity/expiry semantics |
| `HFP-EVENT_BOUND` | event/version/change-witness controls | immutable/versioned condition whose currentness is proven by an independently trustworthy change witness |

For `HFP-EVENT_BOUND`, loss of the change witness or visibility makes currentness `UNKNOWN`. Without an independently trustworthy change witness, the rule SHALL fall back to a time-bounded profile no looser than `HFP-SLOW`.

### 6.3 Initial Subject/Capability Mapping

| Subject / evidence family | Default treatment |
|---|---|
| Health Monitoring runtime availability and publication continuity | `HFP-CRITICAL` |
| Authority Engine runtime availability / policy-evaluation availability | `HFP-CRITICAL` |
| Security/trust/revocation evidence | strictest of `HFP-CRITICAL`, source validity, security policy |
| Stage 6 resource pressure / exhaustion-risk evidence | `HFP-CRITICAL` |
| FSA technical runtime availability and assessment-pipeline continuity | `HFP-FAST` |
| Lifecycle state observation | `HFP-FAST` |
| Service Bus / FIL / Event operational condition | `HFP-FAST` |
| Critical dependency availability/compatibility | `HFP-FAST` unless stricter source policy applies |
| Time/clock-quality evidence | `HFP-SOURCE_BOUND` under TIM-001 |
| State/persistence operational integrity | `HFP-STANDARD` |
| Evidence journal/audit-path operational integrity | `HFP-STANDARD`, or stricter when governing policy requires |
| Backup/restore readiness | `HFP-SLOW` |
| Configuration/baseline/document identity/integrity | `HFP-EVENT_BOUND` when independently witnessed; otherwise no looser than `HFP-SLOW` |

A specific rule MAY be stricter. It SHALL NOT be looser without a separately governed policy decision.

### 6.4 Mandatory Freshness Feasibility Proof

Before activation of any profile-to-source mapping, Gate 0B activation evidence SHALL prove that the accepted source can satisfy the proposed freshness relation through:

- existing accepted public behavior; or
- a Stage-7-authorized observation mechanism that does not change predecessor semantics.

The proof SHALL record:

- source identity;
- source observation mechanism;
- achievable observation/refresh cadence or source expiry semantics;
- expected worst-case acquisition delay;
- resource/polling impact;
- whether any predecessor modification would be required.

If the source cannot satisfy the proposed profile without unauthorized predecessor change or pathological load:

```text
DO_NOT_RELAX_IN_CODE
DO_NOT_REWRITE_PREDECESSOR_SILENTLY
RETURN_TO_GATE0B_POLICY_REVIEW
```

## 7. Evidence Requirements

### 7.1 Minimum Evidence Set

A positive Health assessment requires all evidence declared `REQUIRED` by the rule. At minimum the basis SHALL expose:

- exact subject identity;
- exact capability/scope;
- observation identity;
- observation time;
- assessment time;
- authoritative source identity and owner;
- evidence reference and provenance/integrity result;
- applicable source validity/expiry;
- sufficient clock-quality/temporal validity;
- rule identity/version;
- dependency evidence for every `REQUIRED` dependency;
- known contradictions and blind spots.

### 7.2 Evidence Roles

- `REQUIRED_PRIMARY`;
- `REQUIRED_INDEPENDENT`;
- `SUPPORTING`;
- `DIAGNOSTIC_ONLY`.

Supporting or diagnostic evidence SHALL NOT substitute for required evidence.

### 7.3 Independent Evidence Rule

Independent evidence is mandatory when self-report alone could establish trust in the subject being assessed.

At minimum this applies to:

- Health Monitoring self-health;
- FSA technical Health;
- any rule where subject output is the material basis for declaring subject Health;
- any `HC-TRUST_BLOCKING` rule whose positive result would otherwise depend only on the affected subject.

Self-produced evidence MAY be used, but SHALL NOT be the sole required proof in these cases.

## 8. Acyclic Positive-Proof Rule

No `HEALTHY`, positive Fitness inference, or trust-restoring result may depend on an evidence chain that transitively depends on the same assessment/result being produced.

Mandatory rules:

1. positive proof chains SHALL be acyclic for the decision being produced;
2. a subject's self-produced result MAY be supporting/diagnostic evidence but SHALL NOT close the required independent proof chain where independence is required;
3. FSA technical Health SHALL NOT use the FSA Self Model's interpretation of its own Health as required positive proof;
4. Health Monitoring self-health SHALL NOT use Health Monitoring's own final Health result as required positive proof;
5. rule evaluation SHALL reject a positive decision when a required evidence dependency cycle is detected;
6. cycle detection SHALL preserve the cycle identities in evidence/reason output;
7. bootstrap/self-health SHALL rely on predecessor truth and externally observable runtime/identity/time/evidence/publication-path signals that do not depend on the final Health conclusion.

A detected required positive-proof cycle produces `EQ-INSUFFICIENT` and affected `UNKNOWN`, unless independent current evidence separately proves an explicit failed invariant.

## 9. Evidence Quality and Confidence Policy

Normative classes:

- `EQ-SUFFICIENT`;
- `EQ-LIMITED`;
- `EQ-INSUFFICIENT`;
- `EQ-INVALID`.

### `EQ-SUFFICIENT`

All required evidence is present, fresh, attributable, integrity-valid, temporally valid, within competence, acyclic for positive proof, and free of unresolved material contradiction.

### `EQ-LIMITED`

Required evidence remains usable, but a non-required or explicitly degradable relation is delayed, partially unavailable, lower quality, or bounded by a declared constraint.

### `EQ-INSUFFICIENT`

One or more required relations is missing, stale, delayed beyond the positive-decision bound, inaccessible, materially contradictory, cyclic for required positive proof, partially visible where completeness is required, or outside demonstrated competence.

### `EQ-INVALID`

Evidence is corrupted, provenance-invalid, integrity-invalid, or otherwise prohibited from reliance.

### 9.1 Health-State Qualification

- `HEALTHY` requires `EQ-SUFFICIENT`.
- `DEGRADED` may use `EQ-SUFFICIENT` or `EQ-LIMITED` only for a known bounded degradation.
- `UNHEALTHY` requires current positive evidence of a failed condition or invariant. Missing evidence alone does not prove `UNHEALTHY`.
- `UNKNOWN` is mandatory when current required Health cannot be established because evidence is `EQ-INSUFFICIENT`.
- `EQ-INVALID` evidence is excluded from positive inference and preserved as failure evidence.
- `NOT_APPLICABLE` requires a governed determination that the rule/dimension does not apply.

Existing CON-006 confidence text may carry additional detail but SHALL NOT upgrade a lower evidence-quality class.

## 10. Health Consequence Classes

These classes describe technical Health/Fitness interpretation only. They are not Guardian commands.

### `HC-OBSERVATION_ONLY`

Material for visibility/history but no direct reduction in the declared capability's technical fitness.

`HC-OBSERVATION_ONLY` SHALL NOT be used for:

- a failed required invariant;
- required dependency failure or required dependency `UNKNOWN`;
- identity/provenance/integrity/authority/trust requirements necessary for reliance;
- missing/stale required evidence;
- any condition whose governing source requires fail-closed treatment.

### `HC-DEGRADING`

Known bounded degradation. The affected dimension may be `DEGRADED`; technical fitness may remain usable only with explicit constraints.

### `HC-CAPABILITY_BLOCKING`

Prevents reliable use of the affected capability. Proven failure produces `UNHEALTHY`; inability to establish current required condition produces `UNKNOWN`. The capability cannot project as `FIT`.

### `HC-TRUST_BLOCKING`

Required technical trust has failed or cannot be established. Proven trust failure maps toward `INTEGRITY_FAILURE`; inability to establish it maps toward `UNKNOWN/UNAVAILABLE` as applicable. The affected capability SHALL NOT be `FIT`.

### `HC-RECOVERY_GATED`

The subject/capability requires governed recovery/reassessment before unrestricted reliance can return. Source reappearance alone is insufficient.

No consequence class authorizes Guardian/Lifecycle/Recovery/Authority action.

## 11. Critical Dependency Aggregation Policy

Dependency classifications:

- `REQUIRED`;
- `DEGRADABLE`;
- `INFORMATIONAL`.

### 11.1 `REQUIRED`

- `UNHEALTHY` prevents aggregate `HEALTHY` and produces aggregate `UNHEALTHY` for the dependent capability when the failure is positively established;
- `UNKNOWN` produces aggregate `UNKNOWN`;
- `DEGRADED` produces at least aggregate `DEGRADED`, unless consequence policy makes it capability-blocking;
- a failed required invariant cannot be hidden by healthy siblings.

### 11.2 `DEGRADABLE`

May reduce aggregate Health to `DEGRADED` only when:

- the rule explicitly declares the degraded mode;
- fresh independent evidence proves the bounded mode does not require the failed portion;
- constraints are explicit.

Otherwise the result is `UNKNOWN` or `UNHEALTHY` according to evidence.

### 11.3 `INFORMATIONAL`

Visible but no aggregate effect unless a later governed rule changes classification.

### 11.4 Aggregation Invariants

- no averaging;
- no majority vote can erase a critical failure;
- contradiction on required evidence produces explicit uncertainty;
- unaffected capability separation requires proven dependency independence plus fresh evidence;
- aggregate reduction preserves the responsible dependency identity.

## 12. `RECOVERY_REQUIRED` Mapping Policy

Default:

```text
RECOVERY_REQUIRED -> NOT_FIT
```

`RECOVERY_REQUIRED -> RESTRICTED` is permitted only when ALL are true:

1. fault is technically isolated to a declared scope;
2. requested capability does not depend on the affected subject/path in restricted mode;
3. fresh independent evidence proves the unaffected capability remains usable;
4. no unresolved integrity/provenance/identity/authority/trust/cross-scope contamination affects it;
5. the rule predeclares the restricted mode and exact constraints;
6. the restricted result has expiry and mandatory reassessment;
7. source reappearance alone does not restore `FIT`;
8. required recovery acceptance/release remains separately owned.

Otherwise:

`RECOVERY_REQUIRED -> NOT_FIT`

## 13. FSA as a Technical Health Subject

Stage 7 Health Monitoring SHALL treat FSA as an eligible Foundation technical Health subject without becoming FSA's independent integrity-governance monitor.

Stage 7 may assess:

- runtime availability;
- assessment/publication continuity;
- input-evidence ingestion continuity;
- Self Model update freshness as an observed output-continuity fact, not self-certifying semantic truth;
- dependency availability;
- resource pressure affecting operation;
- persistence/evidence-publication condition;
- configuration/baseline identity/integrity evidence already owned by accepted Foundation truth sources;
- blind spots and monitoring-visibility loss.

FSA self-report is never sufficient by itself where independent evidence is required.

FSA technical Health SHALL obey the acyclic positive-proof rule in Section 8.

Stage 7 SHALL NOT implement through this policy:

- Monitor AI;
- FSA goal/purpose integrity investigation;
- behavioral intent/deception judgment;
- FSA containment/kill/release;
- Owner/FSA governance workflow;
- Stage 13 self-maintenance/evolution control plane.

## 14. Health Monitoring Self-Health

Health Monitoring is itself an observable technical subject.

Independent evidence SHALL be able to establish, as applicable:

- evaluator runtime availability;
- observation-ingestion continuity;
- rule-registry identity/version;
- event/evidence publication-path continuity;
- time/evidence dependencies;
- loss of visibility.

Health Monitoring SHALL NOT declare itself `HEALTHY` solely from its own output and SHALL obey Section 8 acyclic positive proof.

Loss of Health visibility becomes explicit `UNKNOWN` for affected Health knowledge.

## 15. Configuration Interaction

The existing `falcon.health.freshness_window` key is preserved as a Health-owned duration input.

Under the proposed policy it may only:

- make an applicable rule stricter;
- never extend a normative maximum age;
- never replace source-specific validity/expiry;
- never bypass TIM-001 clock/trust requirements.

If this policy is accepted, FDN-004 synchronization is MANDATORY before runtime reliance on these ceiling-only semantics.

No undeclared per-rule configuration key may be invented in source code. If additional configurable keys are needed, they require a separately governed FDN-004 successor.

## 16. Proposed Documentary Activation Package

If the Owner accepts this candidate, the minimum coherent activation package SHALL include:

1. a governed SYS-008 successor carrying the accepted Health Rule policy and closing its two unresolved matters;
2. a coordinated CON-006 successor/clarification resolving `RECOVERY_REQUIRED` exactly;
3. a mandatory FDN-004 successor/synchronization defining the accepted `falcon.health.freshness_window` semantics;
4. freshness source-feasibility evidence for every activated profile mapping;
5. traceability/verification synchronization so VPL-005 consumes the activated rule identities/thresholds without becoming a truth source;
6. Stage 7 plan reconciliation binding executable work to the activated policy identity/version;
7. fresh post-activation Architecture/Consistency and Red-Team review before WP-01 source implementation.

No Guardian policy is created or amended by implication.

## 17. Required Verification Before Activation

Challenge at minimum:

- stale required evidence remains `HEALTHY`;
- missing evidence becomes proven `UNHEALTHY` without positive failure evidence;
- invalid evidence contributes positively;
- Health self-certifies;
- FSA self-certifies;
- Health <-> FSA circular positive proof;
- required evidence cycle is not detected;
- Health issues/implies Guardian command;
- FDN-005 classes are silently reused as Health consequence classes;
- `HC-OBSERVATION_ONLY` masks required/trust-critical failure;
- required unhealthy/unknown dependency is hidden by aggregate health;
- averaging/majority voting masks critical failure;
- unrelated capability is blocked without dependency proof;
- `RECOVERY_REQUIRED` becomes `RESTRICTED` without all conditions;
- source restoration silently restores `FIT`;
- global freshness key weakens a rule;
- TIM/source expiry is ignored;
- event-bound witness loss remains current;
- proposed freshness cannot be met without predecessor rewrite or pathological polling;
- Stage 13 Monitor AI/FSA governance leaks into Stage 7;
- Application business semantics enter Foundation Health;
- zero-Application Foundation is incorrectly unhealthy.

## 18. Red-Team V1 Remediation Trace

| Finding | V2 remediation |
|---|---|
| RT-G0B-V1-01 HIGH - circular positive evidence | Section 8 explicit acyclic positive-proof rule; Sections 13-14 bind FSA and Health self-health to it |
| RT-G0B-V1-02 MEDIUM - freshness feasibility | Section 6.4 mandatory source-feasibility proof and return-to-Gate rule |
| RT-G0B-V1-03 MEDIUM - FDN-004 optional sync | Sections 15-16 make FDN-004 synchronization mandatory if policy is accepted |
| RT-G0B-V1-04 MEDIUM - observation-only misuse | Section 10 hard exclusions for required/trust-critical/fail-closed conditions |

## 19. V2 Candidate Disposition

```text
GATE0B_SOURCE_REVIEW = COMPLETE
HEALTH_RULE_POLICY_CANDIDATE_V2 = COMPLETE
RED_TEAM_V1_FINDINGS_REMEDIATED = YES
FRESHNESS_POLICY = PROPOSED
FRESHNESS_FEASIBILITY_GATE = ADDED
HEALTH_CONSEQUENCE_CLASSES = PROPOSED
EVIDENCE_REQUIREMENTS = PROPOSED
ACYCLIC_POSITIVE_PROOF = REQUIRED
EVIDENCE_QUALITY_POLICY = PROPOSED
CRITICAL_DEPENDENCY_AGGREGATION = PROPOSED
RECOVERY_REQUIRED_MAPPING = PROPOSED
FSA_TECHNICAL_HEALTH = PROPOSED_WITH_STAGE13_BOUNDARY
HEALTH_GUARDIAN_SEPARATION = EXPLICIT
FDN004_SYNCHRONIZATION_IF_ACCEPTED = MANDATORY
SPECIFICATION_DEFINITION_REVIEW_ACTIVATION_GATE = OPEN
OWNER_APPROVAL_REQUIRED = YES
GATE0B_RUNTIME_ACTIVATION = NOT_YET
WP01_SOURCE_IMPLEMENTATION = BLOCKED
STAGE8_AUTHORITY = NOT_GRANTED
STAGE13_AUTHORITY = NOT_GRANTED
```

V2 SHALL receive a fresh Architecture/Consistency review and fresh Red-Team review before any Owner activation decision.
