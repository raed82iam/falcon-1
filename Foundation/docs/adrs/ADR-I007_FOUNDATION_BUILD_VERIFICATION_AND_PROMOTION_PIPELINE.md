# ADR-I007 — Foundation Build, Verification, and Promotion Pipeline

**Identifier:** ADR-I007  
**Version:** 1.0  
**Status:** Accepted  
**Date:** 2026-07-25  
**Decision Owner:** Falcon Project Owner  
**Scope:** FRS-001 build intent, reproducibility, verification evidence, evaluation authority, promotion, provenance, and pipeline governance  
**Affected Specifications:** SYS-001, SEC-001, SEC-002, AUT-001, AUT-002, EVO-001, OPS-004, FRS-001  
**Applicable Standards:** STD-004, STD-007, STD-008, STD-009, STD-012, STD-013  
**Related ADRs:** ADR-F001 through ADR-F008; ADR-I001 through ADR-I006  
**Supersedes:** None  
**Superseded By:** None  
**Decision Record:** Project Owner approval recorded on 2026-07-25

## 1. Context

FRS-001 requires a reproducible build and a governed verification pipeline covering static, unit, Contract, security, integration, fault, degraded, recovery, and VPL verification.

Compilation alone cannot establish that an artifact conforms to Falcon's Vision, Constitution, Specifications, Contracts, security rules, or release scope. Promotion must depend on immutable evidence tied to the exact artifact, the obligations governing its verification, the context in which evaluation occurred, and authorities acting within declared jurisdiction.

## 2. Decision Drivers

- one portable and repository-owned pipeline definition;
- exact source, toolchain, dependency, environment, and policy identity;
- deterministic and independently reproducible output;
- isolated, non-financial build and verification;
- complete requirement-to-verification traceability;
- immutable observations, results, evidence, and corrections;
- explicit Build Intent and fixed evidence obligations;
- separation of observed evidence from derived evaluation;
- preserved Evaluation Context and provenance;
- scoped validity distinct from acceptance;
- acceptance distinct from reliance and promotion;
- independent completeness and promotion authority;
- no vendor lock-in;
- safe treatment of generated and AI-produced material; and
- prevention of self-approval by implementation or self-evolution.

## 3. Higher-Authority Constraints

This decision remains subordinate to the Vision, Constitution, GOV-001, approved Specifications and Standards, FRS-001, IMP-001, and VPL-000 through VPL-008.

> **No evidence, no promotion.**

> **A successful build does not prove a valid release.**

> **The artifact that was verified SHALL be the artifact that is promoted.**

The Pipeline implements gates. It does not create requirements, jurisdiction, authority, or permission to implement.

## 4. Alternatives Considered

### Provider-specific CI definition as the authority

This was rejected because Falcon's build and verification meaning would become dependent on a vendor's workflow model and unavailable outside that provider.

### Developer-built release artifacts

This was rejected because local state, tools, caches, dependencies, and uncommitted changes could not be reconstructed or independently trusted.

### Passing tests selected after execution

This was rejected because evidence obligations could be altered after failure, omitted results could be concealed, and historical completeness could not be evaluated.

### Individual test sessions as promotion evidence

This was rejected because a promoter could combine favorable sessions without preserving the complete required verification case.

### Repository-owned, intent-aware, evidence-gated pipeline

This was selected because the exact artifact, obligations, evidence, evaluation context, authorities, and promotion decision remain attributable, immutable, reconstructable, and provider-independent.

## 5. Decision

### 5.1 Canonical Pipeline

The canonical Pipeline Definition SHALL be owned by the Falcon repository, versioned, integrity-identified, reviewable, and executable in an Approved local environment and by compliant automation providers.

Provider configuration MAY invoke the Pipeline but SHALL NOT redefine:

- Build Intents;
- gates;
- required evidence;
- result vocabulary;
- authority separation;
- promotion criteria;
- failure behavior; or
- artifact identity.

The same canonical commands and semantics SHALL apply locally and in automation. Environment-specific wrappers SHALL remain Adapters.

### 5.2 Build Intent

Every Pipeline Execution SHALL declare one Approved Build Intent before execution.

Initial Build Intents are:

| Build Intent | Purpose | Promotion eligibility |
|---|---|---|
| `DEVELOPER` | Local development feedback | None |
| `VALIDATION` | Governed verification of a declared scope | None by itself |
| `RELEASE_CANDIDATE` | Produce and verify a complete release candidate | Eligible after every promotion gate |
| `HOTFIX` | Produce an urgent bounded correction | Eligible only under an Approved Hotfix Gate Profile |
| `EMERGENCY_RECOVERY` | Produce a bounded recovery artifact | Eligible only within approved recovery authority and Gate Profile |
| `REPRODUCIBILITY` | Independently rebuild an existing candidate | None |
| `SECURITY_INVESTIGATION` | Isolated security analysis | None |

The Build Intent SHALL become immutable before any evidence-producing stage begins.

Changing intent requires a new Pipeline Execution. An artifact built for one intent SHALL NOT be relabeled for another.

> **Build Intent may select an Approved Gate Profile; it may not invent or weaken one.**

Hotfix and Emergency Recovery Intents SHALL NOT waive non-waivable constitutional, authority, Guardian, security, evidence-integrity, Contract, or VPL gates.

### 5.3 Official Build Inputs

An official build SHALL identify:

- Build Intent;
- Pipeline Execution ID;
- Approved Foundation Baseline;
- immutable Source Revision;
- canonical Pipeline Definition and version;
- exact Toolchain Baseline;
- locked dependency closure and source provenance;
- Build and Verification Environment Profile;
- Effective Build Configuration;
- Gate Profile;
- Evidence Requirement Set;
- actor and service identities; and
- required time and clock-quality evidence.

An official candidate SHALL NOT be produced from:

- an uncommitted or ambiguous working tree;
- an unapproved branch or source revision;
- a developer-supplied binary;
- an unlocked or floating dependency;
- an unidentified tool;
- an environment whose required properties cannot be verified; or
- a path capable of reaching financial systems, credentials, market data, or capital.

### 5.4 Dependency Acquisition and Isolated Build

Dependency acquisition SHALL be separated from compilation.

The acquisition stage SHALL:

- use only Approved sources;
- restore exact locked versions;
- verify content identity and provenance;
- evaluate licenses and vulnerabilities;
- record the complete transitive graph;
- apply ADR-I002 admission and Adapter policy;
- create a content-identified dependency bundle or cache; and
- produce preliminary SBOM material.

The isolated build stage SHALL:

- use only the verified dependency bundle;
- disable external network access;
- fail when an input is absent or differs;
- prohibit tool or package download;
- avoid uncontrolled global caches;
- preserve exact input identities; and
- prove absence of financial and production paths.

A Cache is a performance aid, not an authoritative source. Removing it SHALL NOT change the resolved dependency graph or output.

### 5.5 Toolchain Pinning

BLD-001 SHALL pin at least:

- .NET SDK and runtime patch;
- roll-forward policy;
- compiler and MSBuild;
- NuGet and restore behavior;
- analyzers and formatters;
- test runners and coverage tools;
- security, secret, and dependency scanners;
- SBOM and provenance generators;
- canonical-encoding tools;
- PostgreSQL verification version;
- Windows and Linux runner profiles; and
- every generator that can affect governed output.

Every tool is a dependency subject to ADR-I002. Availability on a developer machine does not admit a tool.

### 5.6 Deterministic and Reproducible Builds

Falcon SHALL distinguish:

- **Deterministic Build:** the same declared inputs in the same declared environment produce byte-identical output; and
- **Reproducible Build:** independently established clean environments rebuild the same declared source and inputs with the required equivalent output.

For one target and platform, two independent official rebuilds SHALL be byte-for-byte identical where the output format permits it.

Platform-specific Windows and Linux artifacts MAY differ. Their cross-platform comparison SHALL still prove equivalent:

- source and baseline;
- Contracts and schemas;
- dependency closure;
- version and feature set;
- configuration meaning;
- generated source;
- embedded provenance fields; and
- declared functional scope.

Any unexplained difference SHALL produce `FAIL` or `INCONCLUSIVE`; it SHALL NOT be normalized away after the fact.

### 5.7 Pipeline Stages

PIPE-001 SHALL define the canonical gates. At minimum, a Release Candidate Pipeline SHALL include:

1. **Governance and Scope:** baseline, authority, traceability, constitutional compliance, and non-financial boundary.
2. **Dependency Admission:** lock, provenance, license, vulnerability, SBOM, boundaries, and financial-dependency exclusion.
3. **Static Quality:** formatting, compile, nullable analysis, warnings-as-errors, analyzers, prohibited APIs, architecture boundaries, secret scans, and generated-artifact checks.
4. **Unit, Contract, and Schema:** unit behavior, CON-001 through CON-011, FIL schemas, FCE vectors, IDN, TIM, cryptographic, persistence, and transport boundaries.
5. **Security:** unauthorized, wrong-context, secret, replay, downgrade, key-domain, privilege, boundary, malformed, abuse, and security-case verification.
6. **Integration:** PostgreSQL, IPC, FIL, identity, time, configuration, persistence, restart, and reconciliation on Windows and Linux.
7. **Fault and Degraded:** dependency, communication, evidence, database, key, clock, partial-startup, duplicate, reorder, Guardian, Safe-state, and recovery cases.
8. **VPL:** VPL-001 through VPL-008 under their Approved order and prerequisites.
9. **Reproducibility:** clean independent rebuild and comparison.
10. **Packaging and Attestation:** artifact digests, manifest material, SBOM, provenance, results, limitations, and signed evidence.
11. **Independent Promotion Review:** completeness, validity, result, artifact identity, jurisdiction, and authority.

No successful stage compensates for a failed invariant or mandatory gate.

### 5.8 Pipeline Execution Identity

Every Pipeline Execution SHALL receive a typed Pipeline Execution ID through the Falcon Identifier Provider.

It SHALL bind:

- Build Intent;
- source, baseline, Pipeline, Gate Profile, toolchain, and environment identities;
- actor identities;
- Evidence Requirement Set;
- start, end, duration, and time quality;
- all Verification Sessions;
- candidate artifact identity; and
- final execution result.

Changing an immutable execution input requires a new Pipeline Execution ID.

### 5.9 Verification Session Identity

Every independently meaningful verification execution SHALL receive a Verification Session ID through the Falcon Identifier Provider.

Examples include:

- static analysis;
- Contract verification;
- security verification;
- Windows integration;
- Linux integration;
- one VPL execution; and
- reproducibility comparison.

Every retry or rerun SHALL receive a new Session ID and preserve prior outcomes. A later pass does not erase an earlier failure.

### 5.10 Evidence Requirement Set

Before any evidence-producing stage begins, each governed Pipeline Execution SHALL bind to one immutable Evidence Requirement Set.

The sequence is:

```text
Build Request
↓
Build Intent
↓
Gate Profile
↓
Pipeline Execution ID
↓
Evidence Requirement Set Snapshot
↓
Evidence-Producing Stages
↓
Evidence Completeness Evaluation
↓
Root Verification Evidence Set
↓
Promotion Decision
```

The Evidence Requirement Set SHALL contain:

- Evidence Requirement Set ID and version;
- Build Intent;
- Gate Profile ID, version, and digest;
- Pipeline Definition ID, version, and digest;
- Approved Baseline;
- Source Revision;
- target artifact classes and platforms;
- Environment Profile;
- creation identity, time, and clock quality;
- approval and jurisdiction references; and
- the complete expected evidence inventory.

Each Evidence Requirement SHALL define:

- immutable Evidence Requirement ID;
- Evidence Type;
- governing requirement, invariant, Contract, risk control, or gate;
- expected subject and scope;
- expected producer role;
- applicability;
- production mode;
- condition when applicable;
- required schema and canonical format;
- integrity and provenance;
- freshness and time conditions;
- independence;
- platforms and environments;
- pass, fail, inconclusive, and blocked criteria; and
- exclusion authority and reason where applicable.

Once evidence production begins, the Requirement Set SHALL NOT change. A later Gate Profile change does not reinterpret historical executions.

### 5.11 Requirement Applicability

Every Evidence Requirement SHALL declare one applicability:

| Applicability | Meaning |
|---|---|
| `MANDATORY` | Absence makes the case incomplete |
| `OPTIONAL` | Absence does not prevent completeness for the declared intent |
| `CONDITIONAL` | Applicability is determined by a predeclared governed predicate |
| `EXCLUDED` | Explicitly out of scope under an authorized Gate Profile and recorded reason |

A Mandatory Requirement SHALL NOT be weakened after execution begins.

An Optional artifact that is included SHALL still be valid, attributable, and integrity-protected.

A Conditional Requirement SHALL declare its canonical predicate, input sources, evaluation context, authority, time, and failure behavior before execution. Unknown predicate outcome SHALL NOT be treated as false.

An Exclusion SHALL be pre-authorized, visible, justified, and prohibited for non-waivable gates.

### 5.12 Evidence Production Mode

Evidence production mode is orthogonal to applicability:

| Production mode | Meaning |
|---|---|
| `DIRECT` | Produced from execution, observation, measurement, or inspection |
| `DERIVED` | Computed or judged from preserved evidence and governing rules |

Direct Evidence includes test observations, analyzer findings, artifact digests, fault observations, and VPL records.

Derived Evidence includes completeness reports, coverage summaries, traceability summaries, reproducibility conclusions, risk aggregation, and overall verification outcome.

A Derived Evaluation SHALL identify:

- Derived Evaluation ID;
- Requirement ID;
- input Evidence IDs and digests;
- Evaluation Context ID and digest;
- Derivation Rule ID and version;
- Evaluation Mode;
- Evaluation Nature;
- Evaluator identity;
- Evaluation Authority;
- result, rationale, uncertainty, and limitations;
- approval decision; and
- integrity seal.

Derived Evidence SHALL NOT replace, conceal, or compensate for required Direct Evidence.

### 5.13 Evaluation Mode

Every Derived Evaluation SHALL declare one Evaluation Mode:

| Mode | Meaning |
|---|---|
| `AUTOMATED` | Executed by a governed automated evaluator |
| `HUMAN` | Executed by an identified authorized human reviewer |
| `HYBRID` | Combines preserved automated evaluation and substantive human review |

Automated evaluation SHALL record tool, version, digest, configuration, rule set, inputs, environment, and execution evidence.

Human evaluation SHALL record reviewer identity, competence, authority, conflict disclosure, evidence reviewed, governing rules, rationale, limitations, and decision.

Hybrid evaluation SHALL identify each contribution, sequence, decision owner, veto, disagreement, and resolution. A nominal human click does not establish substantive hybrid review.

### 5.14 Evaluation Nature

Every Derived Evaluation SHALL declare one Evaluation Nature:

| Nature | Meaning |
|---|---|
| `DETERMINISTIC` | The same preserved inputs, rules, and context must reproduce the same result |
| `JUDGMENT_BASED` | A reasoned conclusion requiring independent review rather than guaranteed identical recomputation |

> **Every derived evaluation SHALL be reproducible from the preserved evidence, applicable derivation rules, and recorded evaluation context, unless explicitly classified as judgment-based.**

A Judgment-Based evaluation SHALL remain independently reviewable and reperformable from preserved evidence, rules, context, rationale, authority, assumptions, and conflict disclosures.

AI evaluations SHALL explicitly declare their Evaluation Nature. Unless deterministic behavior is demonstrated under a governed execution profile, AI evaluations SHALL be treated as `JUDGMENT_BASED`.

AI is classified by demonstrated execution properties, not by label. Determinism does not establish correctness or authority.

### 5.15 Evaluation Authority

Execution mode and evaluation authority SHALL remain distinct.

Every Derived Evaluation SHALL identify:

- Evaluation Authority Reference;
- originating jurisdiction and governing model;
- delegation chain;
- authority scope and validity;
- permitted Evaluation Modes and Nature;
- required Approval Mode;
- independence and conflict conditions; and
- evaluation acceptance decision.

Evaluation Approval Mode SHALL be one of:

| Approval mode | Meaning |
|---|---|
| `AUTOMATED_AUTHORIZED` | A specifically delegated automated identity may accept the evaluation within fixed rules and scope |
| `HUMAN_REQUIRED` | An authorized human must accept the evaluation |
| `HYBRID_REQUIRED` | Automated result and substantive human approval are both required |
| `COLLEGIATE_REQUIRED` | A governed board or multiple roles must decide under quorum and conflict rules |

Automation possesses no inherent authority. Automated acceptance requires explicit, bounded, revocable delegation.

Evaluation acceptance does not establish Evidence Completeness or Promotion Authority.

### 5.16 Evaluation Context as a Governed Artifact

> **An Evaluation Context is a governed artifact that captures the authoritative policy, configuration, environment, authority, and trust state under which one or more evaluations are performed.**

It SHALL contain:

- Evaluation Context ID;
- schema and version;
- canonical manifest;
- content digest;
- Context Provenance;
- lineage;
- integrity evidence;
- classification;
- accountable owner;
- declared scope;
- Artifact Lifecycle;
- source-effective times; and
- supersession history.

Its Artifact Lifecycle SHALL include:

- `COLLECTING`;
- `VALIDATED`;
- `SEALED`;
- `SUPERSEDED`; and
- `ARCHIVED`.

A Sealed Context is immutable.

### 5.17 Evaluation Context Contents

An Evaluation Context SHALL identify:

- Environment Profile;
- Deployment Profile;
- Policy Baseline;
- Effective Configuration Snapshot;
- Feature Flags;
- Active Rule IDs and versions;
- Toolchain and evaluator profiles;
- Authority Baseline;
- identity and trust state;
- time and Clock Quality Profile;
- creation time and uncertainty; and
- Context digest and integrity.

Any material change produces a new Evaluation Context.

Multiple Derived Evaluations MAY reference the same immutable Evaluation Context when no material context element has changed and the Context remains valid for the declared evaluation scope and governing policy.

### 5.18 Context Provenance

Every material Evaluation Context element SHALL preserve its own provenance. Aggregation or signature of the Context SHALL NOT make its sources authoritative or correct.

Each element SHALL identify:

- Context Element ID and type;
- authoritative source and owner;
- source jurisdiction and authority;
- version and digest;
- retrieval or observation method and actor;
- issue, effective, expiry, and freshness times;
- validation result;
- transformation history;
- predecessor or superseded reference;
- integrity and classification; and
- uncertainty, conflict, or limitation.

Context construction SHALL validate source identity, jurisdiction, authority, integrity, effectiveness, compatibility, and temporal alignment before sealing the Canonical Context Manifest.

### 5.19 Context Validity Assessment

Context Artifact Lifecycle SHALL remain separate from Context Validity.

Every Context Validity Assessment SHALL identify:

- Context ID and digest;
- Evaluation Scope;
- Governing Policy ID and version;
- intended purpose;
- Build Intent and Gate Profile where applicable;
- Deployment Profile;
- assessment time and Clock Quality;
- freshness bounds;
- evaluator and Evaluation Authority;
- evidence and rationale; and
- one scoped status.

The statuses are:

| Status | Meaning |
|---|---|
| `VALID` | Fit for the declared scope, purpose, policy, and validity conditions |
| `INCOMPLETE` | Required Context material or provenance is absent |
| `INVALID` | Integrity, authority, identity, or mandatory validation failed |
| `CONFLICTED` | Controlling Context sources contradict one another |
| `STALE` | Required freshness or validity has expired |
| `UNCERTAIN` | Available evidence cannot establish another status safely |

`VALID` SHALL NOT be treated as global. Validity for unit tests does not imply validity for security review or release promotion.

A later assessment SHALL append; it SHALL NOT modify the Context or prior assessment.

### 5.20 Evidence Immutability

> **Verification evidence SHALL be immutable once recorded. Corrections SHALL produce new evidence linked to the previous record rather than modifying historical evidence.**

This rule applies to:

- observations and measurements;
- test and VPL results;
- analyzer and security reports;
- SBOM;
- provenance;
- traceability;
- reproducibility comparisons;
- completeness decisions;
- evaluation and review records; and
- promotion decisions.

Retries, corrections, transformations, and re-evaluations SHALL receive new identities and preserve their input and predecessor lineage.

### 5.21 Verification Evidence Set Identity

Every governed Pipeline Execution SHALL produce a Root Verification Evidence Set or an attributable record explaining why it could not.

The Root Verification Evidence Set SHALL have:

- one typed Evidence Set ID;
- immutable canonical manifest;
- artifact and baseline binding;
- Evidence Requirement Set ID and digest;
- Pipeline Execution ID;
- all Verification Session IDs;
- all Direct Evidence identities and digests;
- all Derived Evaluations;
- Evaluation Contexts and Validity Assessments;
- completeness and overall outcome;
- missing, invalid, excluded, and superseded evidence;
- provenance and SBOM;
- integrity seal; and
- correction and supersession history.

> **The Root Verification Evidence Set SHALL preserve not only produced evidence, but also the verification obligations against which that evidence was evaluated.**

### 5.22 Root Evidence Set Layers

The Root Verification Evidence Set SHALL preserve five formal layers:

1. **Verification Obligations:** Build Intent, Gate Profile, immutable Evidence Requirement Set, applicability, conditions, exclusions, and result rules.
2. **Observed Evidence:** observations, logs, measurements, results, provenance, integrity, missing items, and failed attempts.
3. **Evaluation Context:** the sealed policy, configuration, environment, authority, trust, rule, and provenance state.
4. **Derived Evaluations:** rules, inputs, mode, nature, evaluator, authority, rationale, result, and reproducibility or review.
5. **Promotion Context:** candidate identity, completeness, overall outcome, jurisdiction, acceptance, reliance limits, and promotion eligibility.

The causal model is:

```text
Verification Obligations
        +
Observed Evidence
        +
Evaluation Context
        ↓
Derived Evaluations
        ↓
Promotion Context
```

### 5.23 Evidence Completeness

Evidence Completeness SHALL remain distinct from Verification Outcome.

Completeness statuses are:

| Status | Meaning |
|---|---|
| `COMPLETE` | Every item required for the declared Intent and Gate Profile exists, is mapped, and is integrity-valid |
| `PARTIAL` | The set intentionally covers a declared non-promotion scope and does not claim a complete release case |
| `INCOMPLETE` | At least one required item is missing or unfinished |
| `INVALID` | The set, seal, identity, provenance, mapping, or included evidence cannot be trusted |

A Complete set may contain a `FAIL`. Completeness is not success.

The verification outcomes remain:

- `PASS`;
- `FAIL`;
- `INCONCLUSIVE`; and
- `BLOCKED`.

The Evidence Completeness Authority SHALL evaluate actual evidence against the immutable Evidence Requirement Set, not against a later Gate Profile.

No component that produces, transforms, aggregates, or signs verification evidence SHALL be the sole authority that declares that evidence complete or promotion-ready.

### 5.24 Root Evidence Set as Sole Promotion Evidence

> **Promotion decisions SHALL reference exactly one root Verification Evidence Set. Individual Verification Sessions SHALL NOT be used directly as promotion evidence.**

The root may bind externally retained evidence by immutable identity and digest. It SHALL remain the one authoritative promotion-case manifest.

A correction, added session, or re-evaluation creates a new root Evidence Set linked to the previous one. The Promotion Decision SHALL reference the exact current root set.

Evidence from multiple sessions MAY be combined only through a new governed root set that preserves every included, failed, superseded, missing, and excluded result without selective omission.

### 5.25 Artifact Immutability and Promotion

After candidate creation:

- content and digest SHALL be fixed;
- manual repackaging or modification is prohibited;
- signing SHALL occur through a declared stage and identity;
- any content change creates a new candidate;
- the verified digest SHALL remain the promoted digest; and
- promotion SHALL move the same identity between states rather than rebuild it.

Candidate lifecycle states include:

```text
BUILT
↓
VERIFIED
↓
ACCEPTED
↓
PROMOTED
```

Each transition remains a distinct governed decision.

Only a `COMPLETE`, integrity-valid Root Evidence Set with overall `PASS` MAY satisfy Release Candidate, Hotfix, or Emergency Recovery promotion requirements.

Promotion additionally requires:

- promotable Build Intent;
- matching artifact digest;
- valid Approved Baseline;
- valid Evaluation Context for promotion scope and policy;
- valid jurisdiction and authority;
- no unresolved non-waivable blocker; and
- independent Promotion Authority approval.

`PARTIAL`, `INCOMPLETE`, `INVALID`, `FAIL`, `INCONCLUSIVE`, or `BLOCKED` SHALL NOT be promoted as a release.

### 5.26 Build Provenance and SBOM

Every candidate SHALL preserve build provenance identifying:

- builder identity;
- source revision;
- Pipeline and Build Intent;
- top-level and dependency inputs;
- Toolchain and Environment Profiles;
- execution and time evidence;
- output artifacts and digests; and
- attestation mechanism.

Every candidate SHALL include an open-standard SBOM, initially using an Approved SPDX profile, identifying direct, transitive, embedded, generated, and build-affecting components, their versions, sources, licenses, relationships, and integrity identities.

SBOM and provenance SHALL bind to the exact artifact. Neither proves security by existence.

Falcon SHALL NOT claim a SLSA level until every requirement of the identified SLSA version and track has been independently assessed.

### 5.27 Test Integrity and Flakiness

An intermittent result is unresolved evidence.

- Every retry SHALL be retained.
- Flaky tests SHALL NOT be averaged into success.
- Disabling a test requires an Approved, scoped, time-bounded change.
- Coverage metrics identify gaps but do not prove correctness.
- Mocks do not prove target-environment behavior.
- Expected denial or Safe-state behavior counts as success only when the governing requirement requires it.
- Cleanup and restoration SHALL be verified separately where consequence requires.

### 5.28 Generated and AI-Produced Material

Generated or AI-produced source, tests, designs, rules, reports, and configurations are untrusted inputs until governed review and verification.

The Pipeline SHALL preserve generator or model identity, version, digest, configuration, material inputs, and provenance where required.

Generated output SHALL pass the same gates as human-authored output.

A generator that affects output is part of the Toolchain Baseline.

Self-Maintenance, AI, or a generator SHALL NOT:

- weaken the gates evaluating its change;
- alter required evidence after failure;
- approve its own output;
- control the sole independent abort path;
- sign its own promotion case; or
- expand its authority through repeated success.

### 5.29 Separation of Duties

The following roles SHALL remain distinct according to consequence:

- Source Author;
- Builder;
- Test and Observation Producer;
- Evidence Collector;
- Evidence Transformer or Aggregator;
- Evaluator;
- Evaluation Authority;
- Evidence Completeness Authority;
- Artifact Signer;
- Promotion Authority; and
- Challenge Resolution Authority.

Automation MAY implement multiple mechanical functions only when identity, permission, evidence, and decision authority remain separately governed.

Release-signing keys SHALL NOT be available during compilation or ordinary testing.

### 5.30 Trust Objects

SEC-002 SHALL define the Foundation Trust Object Model used by artifacts, Evidence, Evaluation Contexts, SBOMs, provenance, manifests, policy snapshots, configuration snapshots, Rule Sets, and Root Evidence Sets.

> **A Trust Object is not trusted merely because it is classified as a Trust Object.**

> **Trust is established through governed verification, not through object classification.**

A Trust Object may carry claims and evidence about trust. It SHALL NOT grant itself validity, authority, acceptance, or reliance.

SEC-002 SHALL define common identity, provenance, integrity, lineage, validity, authority scope, lifecycle, immutability, correction, supersession, canonical encoding, and challenge properties without replacing type-specific Contracts.

### 5.31 Trust Claims

Claims are assertions carried by Trust Objects.

> **Claims SHALL remain attributable, scoped, verifiable, and independently challengeable throughout their lifecycle.**

Claims include `VALID`, `VERIFIED`, `COMPLETE`, `PASS`, `TRUSTED`, `APPROVED`, `REPRODUCIBLE`, `INTEGRITY_VALID`, and `RECOVERABLE`.

Every material Claim SHALL identify:

- Claim ID and type;
- claimant;
- subject;
- scope and purpose;
- governing rules;
- evidence and Evaluation Context;
- issue, effective, expiry, and review time;
- uncertainty and limitations;
- challenge path;
- withdrawal and supersession; and
- integrity protection.

Recording, signing, or persisting a Claim does not make it universally true.

### 5.32 Validity and Acceptance

> **Validity is an assessment of fitness for a declared scope under defined governing rules. Acceptance is an authority decision to rely upon that assessment for a specific purpose.**

`VALID` does not imply `ACCEPTED`.

An Acceptance Decision SHALL reference the exact Trust Object, Claim, Validity Assessment, scope, purpose, governing policy, accepting authority, jurisdiction, conditions, effective time, expiry or review, revocation, and evidence.

Acceptance does not widen validity or make a Claim universally true.

### 5.33 Bounded Reliance

> **Reliance SHALL remain explicitly bounded by the scope, purpose, governing policy, and validity conditions declared by the corresponding Acceptance Decision.**

Reliance MAY preserve or narrow an Accepted scope; it SHALL NOT expand it.

Every material Reliance Record SHALL identify:

- Reliance ID;
- Acceptance Decision;
- relying subject;
- Trust Object and Claim;
- exact scope and purpose;
- policy and validity conditions;
- downstream-use limits;
- effective time, expiry, and revocation;
- authority limits; and
- evidence.

Acceptance for Foundation testing does not authorize production or financial use.

### 5.34 Jurisdiction and Delegation

> **Authority SHALL be exercised only within its declared jurisdiction. Delegation SHALL NOT create jurisdiction where none exists.**

> **No delegated authority acquires jurisdiction beyond that explicitly established by the governing model.**

Jurisdiction defines the subject domain in which an authority may decide. Authority defines the permitted decisions and actions within it. Delegation may transfer only an explicitly delegable subset already possessed by the delegator.

A valid delegation SHALL establish:

- delegator and recipient identity;
- governing jurisdiction;
- delegator authority and right to delegate;
- delegated scope, purpose, duration, conditions, and exclusions;
- higher constraints;
- revocation and review; and
- evidence.

Technical ability, access, successful history, emergency, or possession of a Trust Object SHALL NOT create jurisdiction.

Cross-jurisdiction decisions require every competent authority or an expressly established higher jurisdiction.

Unknown jurisdiction or delegation chain SHALL cause default denial.

GOV-AUT-001 SHALL define the governing Jurisdiction and Delegation Model. AUT-001 v1.1 SHALL require operational jurisdiction verification.

### 5.35 Challenge Governance

Every material Claim, Validity Assessment, Acceptance, Completeness Decision, Evaluation, and Promotion Decision SHALL have a governed challenge path.

> **A Challenge SHALL NOT be conclusively resolved solely by the producer of the challenged Claim or by the authority whose decision is under challenge, unless explicitly permitted by governing policy for low-impact cases.**

The challenged party MAY respond, produce evidence, correct, withdraw, or impose temporary containment. It SHALL NOT be the sole final authority in a material challenge.

Challenge governance SHALL identify:

- Challenge ID;
- challenged subject and decision;
- challenger and standing;
- Challenge Resolution Authority;
- independence and conflicts;
- evidence access;
- containment or suspension;
- response time;
- escalation and appeal;
- final governing authority; and
- resolution evidence.

The chain SHALL terminate at a defined competent authority and SHALL NOT return circularly to the same jurisdictional decision-maker under another label.

Low-impact self-resolution must be pre-authorized, recorded, appealable, and prohibited for security, authority, Guardian, evidence integrity, promotion, or high-consequence recovery.

Challenge Records and resolutions append; they do not rewrite the challenged history.

### 5.36 Authority Separation Rule

> **Execution produces observations and results. Evaluation Authority legitimizes evaluation outcomes within a defined scope. Evidence Completeness Authority determines whether the required verification case is complete. Promotion Authority alone determines whether the verified artifact may advance.**

The governing chain is:

```text
Governing Model
↓
Jurisdiction
↓
Authority
↓
Evaluation and Completeness
↓
Validity
↓
Acceptance
↓
Bounded Reliance
↓
Promotion Decision
```

No subordinate success state skips a later authority decision.

### 5.37 Foundational Evidence Rule

> **Evidence without its governing obligation is incomplete context. An obligation without its evaluated evidence is an unproven claim. An evaluation without preserved evidence and governing rules is an unverifiable conclusion.**

And:

> **No Claim establishes its own truth. No Acceptance expands its own scope. No challenged authority conclusively validates itself in a material dispute. No delegated authority acquires jurisdiction beyond that explicitly established by the governing model.**

### 5.38 Pipeline Change Governance

The Pipeline Definition, Gate Profiles, Evidence Requirement schemas, evaluators, completeness rules, and promotion rules SHALL be versioned governed artifacts.

Deleting or weakening a test, analyzer, gate, evidence requirement, independence rule, or challenge path is a material change.

A candidate that changes its Pipeline SHALL NOT rely solely on the changed Pipeline to approve that change. The prior and proposed controls SHALL evaluate the change where applicable, with independent review.

Toolchain, Build Image, environment, policy, rule, or material configuration change SHALL trigger impact assessment and proportional re-verification.

### 5.39 Failure and Waiver Policy

Failure of a required gate SHALL stop promotion.

Missing evidence produces `INCOMPLETE` or `INCONCLUSIVE`, not assumed success.

Constitutional, authority, jurisdiction, Guardian, security, evidence-integrity, Contract, and VPL failures are non-waivable through ordinary exception.

A non-governing check may receive a documented exception only when it is:

- legitimately authorized;
- scoped and justified;
- risk-assessed;
- time-bounded;
- monitored;
- compensated where required;
- visible in the Evidence Requirement Set and Root Evidence Set; and
- incapable of changing a failed result into `PASS`.

Schedule, cost, convenience, or prior success are not evidence.

### 5.40 No Vendor Lock-in

Falcon Contracts, Gate Profiles, evidence schemas, provenance, SBOM, and promotion decisions SHALL NOT assume one CI provider.

Provider replacement SHALL preserve:

- exact gates;
- Build Intents;
- identities and authorities;
- isolation;
- provenance and evidence;
- secret boundaries;
- reproducibility;
- challenge paths; and
- promotion controls.

### 5.41 Required Pre-Implementation Artifacts

Build or verification implementation SHALL NOT begin until:

1. `BLD-001 — Foundation Toolchain and Build Baseline Catalog` is Approved;
2. `PIPE-001 — Foundation Pipeline Specification` is Approved and registered;
3. `TRC-001 — Foundation Requirement-to-Verification Traceability Matrix` is Approved;
4. `ENV-001 — Foundation Build and Verification Environment Profile` is Approved;
5. `SEC-002 — Foundation Trust Object Model` is Approved and registered;
6. `GOV-AUT-001 — Authority Jurisdiction and Delegation Model` is Approved;
7. the AUT-001 v1.1 amendment is Approved and activated;
8. Build Intent, Gate Profile, Evidence Requirement Set, Evaluation Context, Root Evidence Set, and Promotion Decision schemas are Approved;
9. independent authority and challenge assignments are recorded; and
10. the exact implementation baseline and non-financial environment are verified.

### 5.42 Scope Limitation

This decision does not authorize source implementation, CI-provider installation, package download, external network connectivity, release signing with production keys, production deployment, financial integration, or live-capital behavior.

## 6. Consequences

- Stage 0 gains a complete build, verification, evidence, and promotion decision.
- Build purpose is explicit before execution.
- Historical evidence obligations remain immutable and reconstructable.
- Direct observations and Derived Evaluations remain distinct.
- Evaluation Context becomes a reusable governed artifact with scoped validity.
- Root Evidence Set becomes the sole promotion evidence unit.
- Completeness, outcome, validity, acceptance, reliance, and promotion remain separate.
- Evaluation, completeness, signing, and promotion authorities remain separated.
- Jurisdiction constrains authority and delegation.
- Pipeline and AI-generated changes cannot approve themselves.
- SEC-002, GOV-AUT-001, AUT-001 v1.1, and four build-definition artifacts become implementation gates.

## 7. Risks and Mitigations

- **Pipeline bureaucracy obscures safety:** keep schemas canonical, automate mechanical evidence, and preserve explicit ownership.
- **Gate Profile changes rewrite history:** bind every execution to an immutable Evidence Requirement Set.
- **Evidence cherry-picking:** require one Root Evidence Set containing every required, failed, missing, excluded, and superseded result.
- **Complete mistaken for passing:** preserve independent Completeness and Verification Outcome.
- **Evaluator self-authorizes:** require jurisdiction, delegation, Evaluation Authority, and independent completeness.
- **Context becomes a black box:** preserve provenance per Context element and scoped Validity Assessments.
- **Context proliferation:** permit reuse only when sealed, materially unchanged, fresh, and valid for declared scope and policy.
- **Trust Object name implies trust:** require governed verification and prohibit self-validity.
- **Acceptance expands into general trust:** bind Reliance to exact Acceptance scope and conditions.
- **Circular challenge:** require an independent competent resolution path terminating at a defined authority.
- **Delegation crosses domains:** verify jurisdiction before authority and delegation.
- **Flaky success hides instability:** preserve all attempts and treat intermittence as unresolved.
- **CI provider lock-in:** keep canonical Pipeline semantics repository-owned and portable.

## 8. Compatibility and Transition

This decision realizes IMP-001 Stage 0 and VPL-000 verification discipline without modifying Approved Specifications or Contracts automatically.

SEC-002 is currently Planned in SPEC-000 and requires a governed title and scope registration update before Approval. AUT-001 v1.1 requires a separately activated amendment. Roadmap entries create no authority by listing them.

Future pipeline, evidence, Trust Object, or promotion changes SHALL preserve historical interpretation, immutable evidence, scoped validity, jurisdiction, challengeability, and the exact artifact-to-decision chain.

## 9. Conformance Evidence

Conformance requires:

- portable local and automation execution of the canonical Pipeline;
- immutable Build Intent and Gate Profile binding;
- locked offline build from verified dependencies;
- exact toolchain and environment identity;
- deterministic and independent reproducibility results;
- immutable Evidence Requirement Set before evidence production;
- Direct and Derived Evidence classification;
- Evaluation Mode, Nature, Context, Authority, and approval proof;
- Context Provenance and scoped Validity Assessment;
- complete immutable Root Evidence Set;
- separate Completeness and Verification Outcome;
- proof that Promotion references exactly one root set;
- artifact digest continuity from build through promotion;
- SBOM and provenance bound to the artifact;
- retained retries, failures, flaky results, corrections, and supersession;
- independent completeness, promotion, and material challenge resolution;
- jurisdiction and delegation verification;
- Trust Claim, Acceptance, and bounded Reliance evidence;
- Pipeline self-change and AI-generated-change protection;
- Windows and Linux verification;
- VPL-001 through VPL-008 `PASS`; and
- proof that no financial or live-capital path exists.

## 10. References

- Microsoft .NET SDK deterministic and Continuous Integration build properties.
- Microsoft NuGet PackageReference lock files and locked restore.
- SLSA Specification v1.2 Build Provenance.
- SPDX Specification 3.0.1.

## 11. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner and current Constitutional Authority | Accepted | رائد عموره — “موافق على ADR-I007 بصيغته النهائية، وعلى إضافة SEC-002 وGOV-AUT-001 وحزمة AUT-001 v1.1 وBLD-001 وPIPE-001 وTRC-001 وENV-001 إلى Roadmap.” | 2026-07-25 |
