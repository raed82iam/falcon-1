# SYS-008 — Health Monitoring

**Identifier:** SYS-008  
**Version:** 1.1  
**Status:** Approved  
**Effective Date:** 2026-08-12  
**Approval Reference:** Explicit Project Owner approval in the Falcon Foundation workstream on 2026-08-12, limited to the Gate 0B additions defined in this version  
**Owner:** Falcon Operational Integrity Authority  
**Governing Authority:** Constitution Articles 13, 17–18, 23, 31–34, 40–43  
**Affected Domains:** SYS, OPS, AUT, SEC  
**Supersedes:** SYS-008 v1.0  
**Superseded By:** None

## 1. Purpose

Health Monitoring converts trustworthy observations into bounded assessments of operational condition.

It tells Falcon what is known about the condition of components and dependencies. It does not decide capital policy or exercise protective command authority.

## 2. Scope

SYS-008 governs:

- health subject identity;
- signals and observations;
- assessment rules;
- freshness and confidence;
- evidence quality;
- dependency and aggregate health;
- health consequence classes;
- health-state transitions;
- blind spots and unknown state;
- publication of health evidence;
- degradation detection;
- technical Health assessment of FSA as a Foundation subject; and
- Health Monitoring self-health visibility.

## 3. Non-Scope

Health Monitoring does not:

- repair components;
- command lifecycle transitions;
- authorize action;
- define business success;
- infer health solely from process existence;
- replace Logging or Observability;
- declare recovery complete;
- issue Guardian commands;
- restrict, isolate, kill, stop, release, or revive a subject;
- act as Monitor AI for FSA;
- judge FSA goals, intent, deception, governance legitimacy, or self-evolution; or
- replace Security, Evidence, State, Time, Resource, Guardian, Authority, Lifecycle, Recovery, or FSA truth owners.

The following invariants apply:

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

## 4. Canonical Health States

The minimum health model SHALL distinguish:

`HEALTHY`, `DEGRADED`, `UNHEALTHY`, `UNKNOWN`, and `NOT_APPLICABLE`.

`UNKNOWN` SHALL remain distinct from `HEALTHY`.

## 5. Normative Requirements

- **SYS-008-REQ-001:** Every health assessment SHALL identify subject, state, assessment time, evidence time, rule version, and confidence or evidence quality.
- **SYS-008-REQ-002:** Health SHALL be derived from defined evidence, not self-assertion alone.
- **SYS-008-REQ-003:** Stale evidence SHALL reduce confidence or produce `UNKNOWN` according to approved policy.
- **SYS-008-REQ-004:** Missing evidence SHALL NOT produce `HEALTHY`.
- **SYS-008-REQ-005:** Health rules SHALL distinguish availability, correctness, integrity, performance, and dependency condition where material.
- **SYS-008-REQ-006:** Aggregate health SHALL preserve visibility of critical unhealthy dependencies.
- **SYS-008-REQ-007:** A healthy aggregate SHALL NOT conceal a failed condition that violates a required invariant.
- **SYS-008-REQ-008:** Health-state changes SHALL be published as events through SYS-010.
- **SYS-008-REQ-009:** Material health assessments SHALL be traceable to their observations.
- **SYS-008-REQ-010:** Contradictory signals SHALL produce an explicit uncertainty condition rather than arbitrary selection.
- **SYS-008-REQ-011:** Monitoring failure SHALL be observable as loss of health knowledge.
- **SYS-008-REQ-012:** Guardian SHALL receive timely evidence for conditions within its protective mandate.
- **SYS-008-REQ-013:** Recovery SHALL use health evidence but SHALL require independent recovery acceptance criteria.
- **SYS-008-REQ-014:** Health thresholds SHALL be versioned, owned, and authorized.
- **SYS-008-REQ-015:** A component SHALL NOT suppress its material unhealthy evidence from Health Monitoring.
- **SYS-008-REQ-016:** Every executable Health rule SHALL identify rule identity/version, subject, capability scope, applicable Health dimensions, required evidence roles, authoritative evidence source, independent-evidence requirement where applicable, freshness profile, evidence-quality requirement, dependency set and criticality, consequence class, contradiction treatment, accountable owner, governing authority, effective/expiry conditions, and deterministic reason/result codes.
- **SYS-008-REQ-017:** An undeclared Health rule SHALL NOT infer a positive Health state.
- **SYS-008-REQ-018:** Positive Health, positive Fitness inference, or trust-restoring evidence chains SHALL be acyclic for the result being produced.
- **SYS-008-REQ-019:** FSA self-report and Health Monitoring self-report SHALL NOT be the sole required positive proof of their own Health when independent evidence is required.
- **SYS-008-REQ-020:** Health Monitoring SHALL treat FSA as an eligible Foundation technical Health subject while remaining outside Stage 13 Monitor AI, FSA governance, integrity-investigation, containment, kill, release, and self-evolution responsibilities.
- **SYS-008-REQ-021:** Health Monitoring itself SHALL be observable as a Health subject, and loss of its runtime, ingestion, rule-registry, or publication visibility SHALL remain explicit.
- **SYS-008-REQ-022:** A Health consequence class SHALL describe technical interpretation only and SHALL NOT itself authorize Guardian, Authority, Lifecycle, or Recovery action.

## 6. Health Rule Evidence Policy

### 6.1 Minimum Evidence Basis

A positive Health assessment requires every evidence item declared `REQUIRED` by the governing Health rule.

At minimum, the assessment basis SHALL expose:

- exact subject identity;
- exact capability/scope;
- observation identity;
- observation time;
- assessment time;
- authoritative source identity and owner;
- evidence reference and provenance/integrity result;
- applicable source validity or expiry;
- sufficient governed clock-quality/temporal validity;
- Health rule identity/version;
- dependency evidence for every `REQUIRED` dependency; and
- known contradictions and blind spots.

### 6.2 Evidence Roles

Health rules SHALL classify evidence as:

- `REQUIRED_PRIMARY`;
- `REQUIRED_INDEPENDENT`;
- `SUPPORTING`; or
- `DIAGNOSTIC_ONLY`.

`SUPPORTING` and `DIAGNOSTIC_ONLY` evidence SHALL NOT substitute for missing required evidence.

Independent evidence is mandatory when self-report alone could establish trust in the subject being assessed. At minimum, this applies to:

- Health Monitoring self-health;
- FSA technical Health;
- any rule where the subject's own output is the material basis for declaring its Health; and
- any `HC-TRUST_BLOCKING` rule whose positive result would otherwise depend only on the affected subject.

### 6.3 Acyclic Positive-Proof Rule

No `HEALTHY`, positive Fitness inference, or trust-restoring result may depend on an evidence chain that transitively depends on the same assessment or result being produced.

A required positive-proof cycle SHALL produce `EQ-INSUFFICIENT` and the affected Health result SHALL be `UNKNOWN`, unless independent current evidence separately proves an explicit failed invariant.

FSA technical Health SHALL NOT use the FSA Self Model's interpretation of its own Health as required positive proof. Health Monitoring self-health SHALL NOT use Health Monitoring's own final Health result as required positive proof.

## 7. Evidence Quality and Confidence

The canonical Health evidence-quality classes are:

- `EQ-SUFFICIENT`;
- `EQ-LIMITED`;
- `EQ-INSUFFICIENT`; and
- `EQ-INVALID`.

### 7.1 `EQ-SUFFICIENT`

All required evidence is present, fresh, attributable, integrity-valid, temporally valid, within the applicable assessment competence, acyclic for positive proof, and free of unresolved material contradiction.

### 7.2 `EQ-LIMITED`

Required evidence remains usable, but a non-required or explicitly degradable evidence relation is delayed, partially unavailable, lower quality, or otherwise bounded by a declared constraint.

### 7.3 `EQ-INSUFFICIENT`

One or more required evidence relations is missing, stale, delayed beyond the rule's positive-decision bound, inaccessible, materially contradictory, cyclic for required positive proof, partially visible where completeness is required, or outside demonstrated assessment competence.

### 7.4 `EQ-INVALID`

Evidence is corrupted, provenance-invalid, integrity-invalid, or otherwise prohibited from reliance.

### 7.5 Health-State Qualification

- `HEALTHY` requires `EQ-SUFFICIENT`.
- `DEGRADED` may use `EQ-SUFFICIENT` or `EQ-LIMITED` only when a known bounded degradation is positively established.
- `UNHEALTHY` requires current positive evidence of a failed condition or required invariant. Mere absence of evidence is insufficient to prove `UNHEALTHY`.
- `UNKNOWN` is mandatory when current required Health cannot be established because evidence is `EQ-INSUFFICIENT`.
- `EQ-INVALID` evidence SHALL be excluded from positive inference and preserved only as failure evidence.
- `NOT_APPLICABLE` requires a governed determination that the rule or dimension does not apply to the declared subject/capability scope.

A numeric or textual confidence value SHALL NOT upgrade a lower evidence-quality class into a more favorable Health state.

## 8. Freshness Policy

### 8.1 Strictest-Bound Rule

Freshness SHALL be evaluated per evidence relation, not through one universal timer for all Foundation subjects.

The effective permitted freshness SHALL be the strictest applicable bound among:

1. authoritative source validity or expiry;
2. the governing Health rule freshness profile;
3. an active authorized `falcon.health.freshness_window` value only when that value is stricter than the governing rule profile; and
4. governing temporal/clock validity under TIM-001.

A configuration value SHALL NOT extend a normative Health freshness limit.

If required evidence has no valid freshness basis, the affected rule SHALL NOT produce `HEALTHY`.

### 8.2 Initial Foundation Freshness Profiles

| Profile | Maximum evidence age | Intended use |
|---|---:|---|
| `HFP-CRITICAL` | 5 seconds | authority/trust-sensitive runtime condition, Health Monitoring availability, resource-pressure emergency visibility |
| `HFP-FAST` | 15 seconds | runtime/lifecycle/service-bus/FIL/event/dependency condition and FSA technical runtime condition |
| `HFP-STANDARD` | 60 seconds | state/persistence/evidence-path operational integrity and ordinary technical capability condition |
| `HFP-SLOW` | 300 seconds | backup/restore readiness and slow-changing technical readiness |
| `HFP-SOURCE_BOUND` | source/TIM validity controls | evidence governed by explicit stronger validity/expiry semantics |
| `HFP-EVENT_BOUND` | event/version/change-witness controls | immutable or versioned condition whose currentness is proven by an independently trustworthy change witness |

For `HFP-EVENT_BOUND`, loss of the change witness or loss of visibility SHALL make currentness `UNKNOWN`. Without an independently trustworthy change witness, the rule SHALL fall back to a time-bounded profile no looser than `HFP-SLOW`.

### 8.3 Initial Subject/Capability Mapping

| Subject / evidence family | Default freshness treatment |
|---|---|
| Health Monitoring runtime availability and publication continuity | `HFP-CRITICAL` |
| Authority Engine runtime availability / policy-evaluation availability evidence | `HFP-CRITICAL` |
| Security/trust/revocation evidence | strictest of `HFP-CRITICAL`, source validity, and security policy |
| Stage 6 resource-pressure / exhaustion-risk signals | `HFP-CRITICAL` |
| FSA technical runtime availability and assessment-pipeline continuity | `HFP-FAST` |
| Lifecycle state observation | `HFP-FAST` |
| Service Bus / FIL / Event operational condition | `HFP-FAST` |
| Critical dependency availability/compatibility | `HFP-FAST` unless a stricter source rule applies |
| Time/clock-quality evidence | `HFP-SOURCE_BOUND` under TIM-001 |
| State/persistence operational integrity | `HFP-STANDARD` |
| Evidence journal/audit-path operational integrity | `HFP-STANDARD`, or stricter when governing policy requires |
| Backup/restore readiness evidence | `HFP-SLOW` |
| Configuration/baseline/document identity/integrity | `HFP-EVENT_BOUND` when independently witnessed; otherwise no looser than `HFP-SLOW` |

A specific Health rule MAY be stricter than this table. It SHALL NOT be looser without a separately governed policy decision.

### 8.4 Freshness Feasibility Rule

Before runtime reliance on a profile-to-source mapping, evidence SHALL prove that the accepted source can satisfy the proposed freshness relation through existing accepted public behavior or a Stage-7-authorized observation mechanism that does not change predecessor semantics.

The proof SHALL identify source, observation mechanism, achievable cadence or source-expiry semantics, expected worst-case acquisition delay, resource/polling impact, and whether predecessor modification would be required.

If a proposed profile cannot be satisfied without unauthorized predecessor change or pathological load, implementation SHALL NOT relax the rule or silently rewrite the predecessor. The affected mapping SHALL return to governed Health policy review.

## 9. Health Consequence Classes

Health consequence classes describe the technical effect of a Health finding on Health/Fitness interpretation. They are not Guardian commands.

### 9.1 `HC-OBSERVATION_ONLY`

The condition is material for visibility/history but does not by itself reduce the declared capability's technical fitness.

`HC-OBSERVATION_ONLY` SHALL NOT be used for:

- a failed required invariant;
- required dependency failure or required dependency `UNKNOWN`;
- identity, provenance, integrity, authority, or trust requirements necessary for reliance;
- missing or stale required evidence; or
- any condition whose governing source requires fail-closed treatment.

### 9.2 `HC-DEGRADING`

The condition is a known bounded degradation. The affected Health dimension may be `DEGRADED`; technical Fitness may remain usable only with explicit constraints.

### 9.3 `HC-CAPABILITY_BLOCKING`

The condition prevents reliable use of the affected capability. A proven failed condition produces `UNHEALTHY`; inability to establish current required condition produces `UNKNOWN`. The affected capability SHALL NOT be projected as `FIT`.

### 9.4 `HC-TRUST_BLOCKING`

Integrity, provenance, identity, authority/trust basis, or equivalent technical trust required for reliance has failed or cannot be established. The affected capability SHALL NOT be `FIT`.

### 9.5 `HC-RECOVERY_GATED`

The subject/capability requires governed recovery/reassessment before unrestricted reliance can return. Source reappearance alone is insufficient.

No Health consequence class authorizes restriction, isolation, kill, Lifecycle transition, Recovery execution, release, or authority change.

## 10. Critical Dependency Aggregation Policy

Every dependency relation used by a Health rule SHALL be classified as:

- `REQUIRED`;
- `DEGRADABLE`; or
- `INFORMATIONAL`.

### 10.1 Required Dependencies

For a `REQUIRED` dependency:

- `UNHEALTHY` SHALL prevent aggregate `HEALTHY` and produce aggregate `UNHEALTHY` for the dependent capability when failure is positively established;
- `UNKNOWN` SHALL produce aggregate `UNKNOWN` for the dependent capability;
- `DEGRADED` SHALL produce at least aggregate `DEGRADED`, unless the governing consequence class makes the condition capability-blocking; and
- a failed required invariant SHALL never be hidden by healthy sibling dependencies.

### 10.2 Degradable Dependencies

A `DEGRADABLE` dependency may reduce aggregate Health to `DEGRADED` only when:

- the rule explicitly declares the degraded operating mode;
- fresh independent evidence proves the capability does not require the failed portion for that bounded mode; and
- the constraints are explicit and attributable.

Otherwise the aggregate SHALL be `UNKNOWN` or `UNHEALTHY` according to the evidence and proven failure state.

### 10.3 Informational Dependencies

`INFORMATIONAL` dependencies remain visible but do not affect aggregate Health unless a later governed rule changes their classification.

### 10.4 Aggregation Invariants

- Health states SHALL NOT be averaged.
- Majority voting SHALL NOT erase a required or critical failure.
- Contradiction on required evidence SHALL produce explicit uncertainty.
- An unaffected capability MAY remain independently assessable only when dependency independence is proven by governed dependency truth and fresh evidence.
- Aggregate results SHALL retain the identity of the dependency responsible for reduction.

## 11. FSA as a Technical Health Subject

Health Monitoring SHALL treat FSA as an eligible Foundation technical Health subject.

The Stage 7 FSA technical Health scope may assess:

- runtime availability;
- expected assessment/publication continuity;
- input-evidence ingestion continuity;
- Self Model update freshness as an observed output-continuity fact, not self-certifying semantic truth;
- dependency availability;
- resource pressure affecting operation;
- persistence/evidence-publication condition;
- configuration/baseline identity/integrity evidence already available from accepted Foundation truth owners; and
- explicit technical blind spots and monitoring-visibility loss.

FSA self-report SHALL NOT be sufficient by itself for a positive FSA Health assessment where independent evidence is required.

Health Monitoring SHALL NOT use this scope to implement Monitor AI, investigate FSA goals/purpose/deception/intent, perform FSA containment/kill/release, govern FSA self-development, or pull Stage 13 responsibilities into Stage 7.

## 12. Health Monitoring Self-Health

Health Monitoring itself SHALL be observable as a Health subject.

Independent evidence SHALL be able to establish, as applicable:

- Health evaluator runtime availability;
- observation-ingestion continuity;
- Health rule-registry identity/version;
- event/evidence publication continuity;
- required time/dependency condition; and
- material loss of monitoring visibility.

Health Monitoring SHALL NOT certify its own positive Health solely from its final Health result.

## 13. Failure and Degraded Behavior

When Health Monitoring is degraded, Falcon SHALL make the loss of visibility explicit. Protective policy SHALL determine which activities may continue under `UNKNOWN`.

Health Monitoring SHALL NOT generate false reassurance to preserve apparent availability.

Missing, stale, contradictory, cyclic, inaccessible, or otherwise insufficient required evidence SHALL fail closed for positive Health inference according to this Specification and CON-006.

## 14. Acceptance Evidence

Approval and implementation verification require evidence that:

- stale and missing signals cannot produce healthy state;
- critical dependency failure appears in aggregate assessment;
- contradictory evidence remains visible;
- monitoring failure is detectable;
- state changes are traceable to source observations;
- Guardian receives qualified rather than overstated assessments;
- FSA and Health Monitoring cannot self-certify through circular positive proof;
- `HC-OBSERVATION_ONLY` cannot mask required/trust-critical failure;
- freshness mappings are feasible for their accepted sources before runtime reliance; and
- Health consequence classes do not exercise Guardian, Authority, Lifecycle, or Recovery powers.

## 15. ADR Candidates

- Push, pull, or hybrid observation model;
- rule evaluation topology;
- storage of health history.

Aggregation semantics and Health consequence classes are no longer unresolved in this version.

## 16. Unresolved Matters

None for the Stage 7 Gate 0B policy scope defined by this version.

Any future material change to freshness values, consequence semantics, dependency criticality, FSA technical Health scope, or evidence-quality behavior requires separately governed review and SHALL NOT be introduced through implementation code alone.

## 17. Change History

| Version | Date | Change |
|---|---|---|
| 1.0 | 2026-07-24 | Initial approved Health Monitoring specification. |
| 1.1 | 2026-08-12 | Added the Owner-approved Stage 7 Gate 0B Health rule policy: evidence roles/quality, acyclic positive proof, freshness profiles and mappings, Health consequence classes, critical dependency aggregation, FSA technical Health scope, Health Monitoring self-health, and explicit Health/Guardian separation. |
