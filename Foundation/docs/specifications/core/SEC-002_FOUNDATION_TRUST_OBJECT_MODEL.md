# SEC-002 — Foundation Trust Object Model

**Identifier:** SEC-002  
**Version:** 1.0  
**Status:** Approved  
**Effective Date:** 2026-07-25  
**Approval Record:** Project Owner approval recorded on 2026-07-25  
**Owner:** Falcon Security Authority  
**Governing Authority:** Falcon Vision; Falcon Constitution; GOV-001; SEC-001; ADR-I007  
**Affected Domains:** All  
**Registration Status:** Registered in SPEC-000  
**Implementation Authority:** Not Granted

## 1. Purpose

SEC-002 establishes the common trust model for governed objects, claims, assessments, decisions, and reliance throughout Falcon.

It ensures that Falcon does not treat an object, record, result, status, or authority statement as trustworthy merely because it exists, carries a trusted name, or was produced by an approved component.

> **A Trust Object is not trusted merely because it is classified as a Trust Object.**

> **Trust is established through governed verification, not through object classification.**

## 2. Scope

SEC-002 governs the shared trust properties of:

- artifacts;
- evidence;
- manifests;
- provenance records;
- lineage records;
- policy and configuration snapshots;
- rule sets;
- Evaluation Contexts;
- SBOMs;
- Verification Evidence Sets;
- identity and time observations;
- validity assessments;
- acceptance decisions;
- challenges; and
- other objects explicitly classified by an Approved Specification or Contract as Trust Objects.

It defines:

- identity;
- provenance;
- integrity;
- lineage;
- lifecycle;
- immutability;
- correction and supersession;
- governed Claims;
- scoped verification;
- validity;
- acceptance;
- reliance;
- jurisdiction and authority constraints; and
- independent challenge.

## 3. Non-Scope

SEC-002 does not:

- declare any object trustworthy;
- grant jurisdiction or authority;
- define financial, trading, capital, or risk decisions;
- replace AUT-001 or the future GOV-AUT-001;
- replace SEC-001 or cryptographic policy;
- define the full schema of every Trust Object;
- define build or promotion procedure;
- define canonical binary or textual encoding;
- determine business acceptance;
- make a valid object mandatory to accept;
- make an accepted object valid outside its assessed scope; or
- authorize implementation.

Type-specific Specifications and Contracts remain responsible for the meaning and required fields of their objects. SEC-002 defines the common trust obligations those objects inherit.

## 4. Foundational Model

Falcon applies the following trust sequence:

```text
Identity
    ↓
Provenance
    ↓
Integrity
    ↓
Scoped Verification
    ↓
Validity Assessment
    ↓
Jurisdiction
    ↓
Authority
    ↓
Acceptance
    ↓
Bounded Reliance
```

The broader governance sequence is:

```text
Trust Objects
    ↓
Governed Claims
    ↓
Governed Assessments and Decisions
    ↓
Governed Reliance
```

No later stage repairs missing proof from an earlier stage. Acceptance does not cure unknown identity, broken integrity, absent provenance, invalid verification, or missing jurisdiction.

## 5. Definitions

### 5.1 Trust Object

A **Trust Object** is a governed object whose identity, provenance, integrity, lineage, lifecycle, validity, authority scope, and use may materially affect a Falcon decision or the evidence supporting it.

Classification as a Trust Object imposes obligations. It does not confer trust.

### 5.2 Claim

A **Claim** is an attributable assertion carried by or bound to a Trust Object.

Examples include:

- `PASS`;
- `VALID`;
- `COMPLETE`;
- `VERIFIED`;
- authorship;
- origin;
- integrity;
- compliance;
- authority;
- suitability; and
- absence of a prohibited condition.

A Claim is not an independent truth. Its value depends on scope, evidence, governing rules, authority, and current validity.

### 5.3 Observation

An **Observation** is a recorded fact, measurement, event, or output captured without converting it into a broader conclusion.

### 5.4 Evaluation

An **Evaluation** applies governed rules or judgment to identified inputs and produces an outcome within a declared scope.

### 5.5 Validity Assessment

A **Validity Assessment** determines whether a Trust Object or Claim is fit for a declared scope under identified governing rules and conditions.

### 5.6 Acceptance Decision

An **Acceptance Decision** is an attributable decision by an authorized party to rely on a Validity Assessment for a specific purpose and within defined limits.

### 5.7 Reliance

**Reliance** is the permitted use of an accepted Trust Object, Claim, or assessment for an explicitly bounded purpose.

### 5.8 Challenge

A **Challenge** is a governed objection to the identity, provenance, integrity, interpretation, validity, authority, acceptance, or permitted reliance of a Trust Object or Claim.

### 5.9 Jurisdiction

**Jurisdiction** is the formally established subject, scope, environment, consequence level, and decision class within which an authority may act.

### 5.10 Authority

**Authority** is the governed power to make a defined decision within an existing jurisdiction.

### 5.11 Supersession

**Supersession** records that a newer Trust Object replaces an earlier object for a declared purpose without rewriting or erasing the earlier record.

## 6. Core Principles

### 6.1 Trust Is Evidentiary

Trust SHALL arise from governed evidence and verification appropriate to the declared scope.

Names, locations, classifications, signatures, producers, or prior acceptance SHALL NOT establish trust by themselves.

### 6.2 Trust Is Scoped

No Claim, Validity Assessment, Acceptance Decision, or Reliance SHALL be interpreted outside its declared:

- subject;
- purpose;
- environment;
- time;
- governing policy;
- protection profile;
- consequence class; and
- validity conditions.

### 6.3 Trust Is Current, Not Permanent

Past validity or acceptance SHALL NOT guarantee current validity or acceptance.

Changes to material inputs, governing policy, context, integrity, authority, threat state, or validity conditions SHALL trigger re-evaluation, restriction, or expiry as required by governing policy.

### 6.4 Claims Remain Challengeable

Claims SHALL remain attributable, scoped, verifiable, and independently challengeable throughout their retained lifecycle.

### 6.5 Validity Is Not Acceptance

> **Validity is an assessment of fitness for a declared scope under defined governing rules. Acceptance is an authority decision to rely upon that assessment for a specific purpose.**

A `VALID` status does not compel acceptance.

Acceptance SHALL NOT expand the scope or strength of the underlying Validity Assessment.

### 6.6 Reliance Is Bounded

> **Reliance SHALL remain explicitly bounded by the scope, purpose, governing policy, and validity conditions declared by the corresponding Acceptance Decision.**

Reliance SHALL NOT inherit rights, certainty, duration, or applicability beyond those expressly granted.

### 6.7 Jurisdiction Precedes Authority

> **Authority SHALL be exercised only within its declared jurisdiction. Delegation SHALL NOT create jurisdiction where none exists.**

Delegation MAY transfer or constrain authority within established jurisdiction. It SHALL NOT enlarge the delegator's jurisdiction or create new jurisdiction.

### 6.8 No Self-Establishing Truth

No Trust Object, Claim, producer, evaluator, signer, aggregator, or decision record SHALL establish its own truth solely by asserting it.

## 7. Trust Object Requirements

- **SEC-002-REQ-001:** Every Trust Object SHALL have a stable governed identity appropriate to its type.
- **SEC-002-REQ-002:** Identity SHALL distinguish the object from its versions, copies, representations, predecessors, successors, and related objects.
- **SEC-002-REQ-003:** Reuse of an identity for a different logical object SHALL be prohibited.
- **SEC-002-REQ-004:** Every Trust Object SHALL identify its type, schema or governing Contract, and version.
- **SEC-002-REQ-005:** Every Trust Object SHALL identify its producer or accountable source.
- **SEC-002-REQ-006:** Every Trust Object SHALL preserve provenance sufficient to determine how, when, under what authority, and from which material inputs it arose.
- **SEC-002-REQ-007:** Every material transformation SHALL preserve input identity, transformation identity, applicable rule or process, output identity, and accountable actor.
- **SEC-002-REQ-008:** Integrity SHALL be verifiable using an Approved protection appropriate to consequence and retention needs.
- **SEC-002-REQ-009:** A valid integrity check SHALL prove only the protection claim it is designed to establish. It SHALL NOT be interpreted as proof of correctness, truth, authority, or acceptance.
- **SEC-002-REQ-010:** Trust Object lineage SHALL preserve predecessor, derivation, correction, aggregation, split, merge, and supersession relationships where applicable.
- **SEC-002-REQ-011:** A missing or uncertain material lineage link SHALL be explicit and SHALL constrain validity and reliance.
- **SEC-002-REQ-012:** Trust Objects SHALL be immutable once sealed or used as the basis of a governed decision.
- **SEC-002-REQ-013:** Corrections SHALL create a new Trust Object linked to the corrected object and SHALL preserve the historical record.
- **SEC-002-REQ-014:** Supersession SHALL declare its scope and SHALL NOT silently change the meaning of the superseded object.
- **SEC-002-REQ-015:** Deletion, expiry, archival, and retention SHALL be governed by object type, evidence obligations, legal constraints, and recovery needs.
- **SEC-002-REQ-016:** A Trust Object SHALL NOT be accepted when its required identity, provenance, integrity, lineage, validity, or authority evidence is missing, invalid, conflicted, or materially uncertain unless an Approved policy expressly permits bounded degraded reliance.
- **SEC-002-REQ-017:** Canonical representation SHALL be governed by FCE-001 when identity, integrity, signing, comparison, derivation, or cross-platform interpretation depends on representation.
- **SEC-002-REQ-018:** Type-specific Contracts MAY impose stronger requirements but SHALL NOT weaken this Specification.

## 8. Provenance

Provenance SHALL answer:

- what produced the object;
- who or what initiated production;
- which inputs were used;
- which governing rules applied;
- which environment and configuration applied;
- which authority permitted the activity;
- when the activity occurred;
- which transformations occurred; and
- where supporting evidence can be verified.

Provenance SHALL distinguish:

- observed facts;
- declared information;
- imported information;
- derived information;
- human judgment;
- automated evaluation; and
- unresolved assumptions.

Unknown provenance SHALL NOT be silently replaced by inferred provenance.

## 9. Integrity

Integrity protection SHALL bind the material content and identity required to prevent substitution, ambiguity, or unauthorized alteration.

Integrity verification SHALL produce an attributable result containing:

- protected object identity;
- protected representation and version;
- protection profile;
- verification time observation;
- verifier identity;
- outcome;
- failure reason when applicable; and
- evidence reference.

An integrity failure SHALL invalidate reliance on the affected representation until reconciliation establishes an Approved replacement or restoration path.

## 10. Lifecycle

Every Trust Object type SHALL define an Approved lifecycle. The common lifecycle MAY include:

- `DRAFT`;
- `COLLECTING`;
- `VALIDATED`;
- `SEALED`;
- `ACTIVE`;
- `SUSPENDED`;
- `SUPERSEDED`;
- `EXPIRED`;
- `REVOKED`;
- `REJECTED`; and
- `ARCHIVED`.

Object-type Specifications SHALL select only applicable states and define allowed transitions.

Lifecycle state SHALL NOT be treated as a Validity Assessment unless the governing Contract explicitly defines that meaning.

State transitions SHALL preserve:

- previous state;
- new state;
- transition reason;
- authorizing authority;
- applicable jurisdiction;
- time observation;
- evidence; and
- correlation to the governing action.

## 11. Claims

Every material Claim SHALL identify:

- Claim ID;
- Claim type and version;
- subject;
- exact asserted proposition;
- scope;
- issuer;
- supporting evidence;
- governing rules;
- issue time;
- validity conditions;
- expiry or review condition;
- authority basis where authority is claimed; and
- current challenge or supersession status.

Claims SHALL NOT:

- omit material qualifications;
- present derived conclusions as direct observations;
- exceed the strength of their evidence;
- inherit authority from their producer's identity alone;
- become universal through reuse; or
- conceal uncertainty.

Conflicting Claims SHALL remain separately attributable until a governed resolution is recorded.

## 12. Scoped Verification and Validity

Verification SHALL declare:

- subject;
- scope;
- governing Specification, Contract, policy, or rule set;
- evaluation context;
- evidence inputs;
- evaluator;
- Evaluation Mode;
- Evaluation Nature;
- Evaluation Authority;
- method;
- outcome;
- uncertainty;
- limitations; and
- validity period or invalidation conditions.

Validity status SHALL be explicit. The common status set is:

- `VALID`;
- `INCOMPLETE`;
- `INVALID`;
- `CONFLICTED`;
- `STALE`; and
- `UNCERTAIN`.

`VALID` SHALL mean valid only for the declared evaluation scope and governing policy.

`INCOMPLETE` SHALL mean required material is missing.

`INVALID` SHALL mean a governing requirement is violated or required trust cannot be established.

`CONFLICTED` SHALL mean material sources or assessments disagree without governed resolution.

`STALE` SHALL mean a material time, version, context, or change condition requires re-evaluation.

`UNCERTAIN` SHALL mean available evidence cannot support a determinate validity conclusion.

No non-`VALID` status SHALL be treated as `VALID` by omission, fallback, or convenience.

## 13. Acceptance

An Acceptance Decision SHALL identify:

- accepted subject and version;
- relied-upon Validity Assessment;
- decision purpose;
- jurisdiction;
- accepting authority;
- governing policy;
- permitted Reliance;
- prohibited Reliance;
- effective period;
- conditions;
- revocation and review triggers;
- decision evidence; and
- challenge route.

Acceptance SHALL be denied or restricted when:

- jurisdiction is absent;
- authority is invalid, expired, revoked, or insufficient;
- material evidence is missing;
- the Validity Assessment does not cover the intended purpose;
- integrity or provenance is unresolved;
- governing policy prohibits reliance; or
- the Claim is under a challenge that requires suspension.

## 14. Reliance

Every material act of Reliance SHALL be traceable to:

- the exact Trust Object or Claim;
- the applicable Validity Assessment;
- the governing Acceptance Decision;
- the relying actor;
- the permitted purpose;
- the active jurisdiction and authority;
- the reliance time; and
- the evidence required by consequence.

Downstream reuse SHALL NOT broaden the original Acceptance Decision.

Where a Trust Object is reused in a new scope, purpose, environment, policy, or consequence class, a new Validity Assessment or Acceptance Decision SHALL be obtained as required.

## 15. Jurisdiction, Authority, and Delegation

No Acceptance Decision, challenge resolution, validity approval, or Reliance authorization is legitimate without:

1. established jurisdiction;
2. an authority entitled to act within that jurisdiction;
3. a valid delegation when the authority is delegated;
4. compliance with separation-of-duty requirements;
5. evidence of the decision; and
6. conformity with higher authority.

Delegation SHALL declare:

- delegator;
- delegate;
- source jurisdiction;
- delegated decision classes;
- scope;
- constraints;
- effective period;
- revocation conditions;
- redelegation rule; and
- evidence.

> **No delegated authority acquires jurisdiction beyond that explicitly established by the governing model.**

GOV-AUT-001 SHALL define the complete jurisdiction and delegation model. AUT-001 v1.1 SHALL define operational verification. Until both are Approved and active, no new authority is implied by this Specification.

## 16. Challenge and Independent Review

Every material Claim, Validity Assessment, Acceptance Decision, and Reliance authorization SHALL have a governed challenge path.

A Challenge SHALL identify:

- challenged subject;
- challenged Claim or decision;
- challenger;
- grounds;
- supporting evidence;
- consequence;
- requested restriction or remedy;
- receiving authority;
- resolution jurisdiction; and
- current status.

> **A Challenge SHALL NOT be conclusively resolved solely by the producer of the challenged Claim or by the authority whose decision is under challenge, unless explicitly permitted by governing policy for low-impact cases.**

Governing policy SHALL define when a Challenge:

- suspends Reliance;
- restricts authority;
- requires independent review;
- requires Guardian involvement;
- requires preservation of additional evidence; or
- may remain open under bounded operation.

Challenge resolution SHALL preserve the original Claim, decision, challenge, evidence, reasoning, authority, and outcome.

## 17. Aggregation and Derivation

An aggregate Trust Object SHALL NOT conceal the state of its inputs.

Aggregation SHALL preserve:

- every included input;
- excluded inputs and reasons;
- missing required inputs;
- failed or conflicting inputs;
- superseded inputs;
- aggregation rules;
- Evaluation Context;
- aggregate identity; and
- accountable evaluator.

A Derived Claim SHALL:

- identify all material inputs;
- identify its derivation rules;
- distinguish deterministic derivation from judgment;
- be reproducible when classified as deterministic; and
- preserve its Evaluation Context.

The derived result SHALL NOT possess stronger validity, authority, or integrity than its governing rules and material inputs can support.

## 18. Evaluation Context as a Trust Object

An Evaluation Context is a governed Trust Object that captures the authoritative policy, configuration, environment, authority, and trust state under which one or more evaluations are performed.

It SHALL include:

- Context ID and version;
- declared evaluation scope;
- governing policy snapshot;
- configuration snapshot;
- environment identity;
- active rule versions;
- feature-state snapshot where applicable;
- deployment profile;
- authority and jurisdiction state;
- trust-boundary state;
- time and identifier profile;
- Context Provenance;
- canonical manifest;
- integrity identity;
- lifecycle state; and
- Validity Assessment.

Multiple Derived Evaluations MAY reference the same immutable Evaluation Context only when:

- no material context element has changed;
- the context remains integrity-valid;
- it remains valid for the declared evaluation scope and governing policy; and
- reuse is allowed by the applicable Contract.

A material context change SHALL create a new Evaluation Context identity or version according to its Contract. Historical Contexts SHALL remain preserved.

## 19. Failure and Uncertainty

Failure to establish required trust SHALL reduce permitted reliance. It SHALL NOT be converted into success by absence of evidence.

When required identity, provenance, integrity, lineage, context, validity, jurisdiction, or authority cannot be established:

- the affected Claim SHALL be marked with the applicable non-valid status;
- unrestricted Reliance SHALL be denied;
- affected authority SHALL be restricted when required;
- uncertainty SHALL be preserved;
- reconciliation SHALL use original identities and evidence;
- Blind Retry SHALL be prohibited where duplicate or conflicting effects are possible; and
- Guardian or the responsible safety authority SHALL be notified when capital protection, system integrity, or recovery may be affected.

## 20. Conformance Evidence

Conformance with SEC-002 requires evidence that:

- every in-scope type is explicitly classified or excluded;
- identity is stable and collision-safe;
- provenance and lineage survive transformation;
- integrity verification detects substitution and alteration;
- corrections append rather than rewrite history;
- Claims remain scoped and attributable;
- Validity is distinct from Acceptance;
- Acceptance is distinct from Reliance;
- Reliance does not exceed accepted scope;
- jurisdiction is verified before authority;
- delegation cannot create jurisdiction;
- challenges have an independent resolution path;
- aggregates preserve missing, failed, conflicting, and excluded inputs;
- Evaluation Contexts are immutable, attributable, and reusable only within valid scope;
- uncertainty restricts action; and
- type-specific Contracts do not weaken this model.

## 21. Required Dependent Documents

Full activation of this Specification requires:

- the SEC-002 title and scope update in SPEC-000;
- GOV-AUT-001 — Authority Jurisdiction and Delegation Model;
- AUT-001 v1.1 — Jurisdiction Verification Amendment;
- FCE-001 — Falcon Canonical Encoding Specification;
- applicable Trust Object Contracts;
- type-specific lifecycle definitions;
- challenge and retention policies; and
- verification coverage in TRC-001 and PIPE-001.

## 22. Foundational Rules

> **No Claim establishes its own truth.**

> **No Acceptance expands its own scope.**

> **No challenged authority conclusively validates itself in a material dispute.**

> **No delegated authority acquires jurisdiction beyond that explicitly established by the governing model.**

> **Evidence without its governing obligation is incomplete context. An obligation without its evaluated evidence is an unproven claim. An evaluation without preserved evidence and governing rules is an unverifiable conclusion.**

## 23. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner and current Constitutional Authority | Approved | رائد عموره — “موافق” | 2026-07-25 |

This Approval adopts the title, scope, and normative content of SEC-002 v1.0 and authorizes its registration in SPEC-000 and inclusion in the Foundation Baseline.

It does not authorize implementation.
