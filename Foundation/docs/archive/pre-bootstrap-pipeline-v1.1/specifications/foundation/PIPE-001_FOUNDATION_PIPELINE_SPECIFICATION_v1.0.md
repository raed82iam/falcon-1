# PIPE-001 — Foundation Pipeline Specification

**Version:** 1.0  
**Status:** Approved  
**Owner:** Falcon Foundation Governance  
**Governing Authority:** Falcon Vision; Falcon Constitution; GOV-001; GOV-AUT-001; ADR-I007  
**Applicable Baseline:** FRS-001; BLD-001 v1.0  
**Applicable Specifications:** AUT-001; AUT-002; SEC-001; SEC-002; FCE-001  
**Applicable Standards:** STD-004; STD-005; STD-007; STD-008; STD-009; STD-013  
**Related Documents:** TRC-001; ENV-001; VPL-000 through VPL-008  
**Supersedes:** None  
**Superseded By:** None  
**Implementation Authority:** Not Granted

## 1. Purpose

This Specification defines the canonical Foundation pipeline that transforms an authorized immutable source revision into governed observations, artifacts, evidence, evaluations, and—only where permitted—a promotion decision.

It defines:

- Build Intents;
- Gate Profiles;
- execution stages;
- evidence obligations;
- Evaluation Context;
- evidence completeness;
- artifact identity;
- promotion;
- authority separation;
- failure;
- challenge;
- portability; and
- lifecycle.

The Pipeline proves a declared verification case. It does not create requirements, jurisdiction, authority, truth, implementation permission, or financial permission.

## 2. Scope

PIPE-001 governs:

- non-financial FRS-001 builds;
- validation executions;
- Release Candidate executions;
- Hotfix and Emergency Recovery executions when separately authorized;
- reproducibility executions;
- security investigations;
- Windows and Linux execution;
- dependency acquisition;
- isolated build and verification;
- packaging and attestation;
- Evidence Requirement Sets;
- Verification Sessions;
- Root Verification Evidence Sets;
- completeness evaluation; and
- promotion decisions.

## 3. Non-Scope

PIPE-001 does not:

- authorize implementation;
- authorize production deployment;
- authorize trading or financial activity;
- define business functionality;
- select tools beyond BLD-001;
- define runner images beyond ENV-001;
- replace approved VPL procedures;
- appoint an authority;
- grant jurisdiction;
- permit self-approval;
- guarantee that passing evidence establishes absolute safety; or
- activate any tool, environment, cryptographic profile, identifier profile, or time profile.

## 4. Foundational Model

The canonical flow is:

```text
Authorized Build Request
        ↓
Build Intent
        ↓
Gate Profile
        ↓
Pipeline Execution Identity
        ↓
Evidence Requirement Set Snapshot
        ↓
Input and Environment Admission
        ↓
Dependency Acquisition
        ↓
Isolated Build and Verification
        ↓
Observed Evidence
        ↓
Derived Evaluations
        ↓
Root Verification Evidence Set
        ↓
Completeness Decision
        ↓
Validity and Acceptance
        ↓
Promotion Decision
```

No later stage may silently redefine an earlier stage.

## 5. Governing Principles

1. No evidence, no promotion.
2. A successful build is not a valid release.
3. The artifact verified is the only artifact eligible for promotion.
4. Evidence obligations are fixed before evidence production.
5. Observations remain distinct from evaluations.
6. Validity remains distinct from acceptance.
7. Acceptance remains distinct from reliance.
8. Jurisdiction precedes authority.
9. Failure is preserved; it is not erased by retry.
10. Missing proof never becomes permission.
11. A provider invokes the Pipeline; it does not define it.
12. Every material object remains attributable, immutable, scoped, and challengeable.

## 6. Pipeline Definitions

### 6.1 Pipeline Definition

The Pipeline Definition is the repository-owned, versioned, content-identified authority for execution stages and semantics.

It SHALL identify:

- Pipeline ID;
- version;
- canonical digest;
- applicable baseline;
- supported Build Intents;
- Gate Profiles;
- stage graph;
- result vocabulary;
- required schemas;
- transition rules;
- failure rules;
- authority requirements; and
- compatibility rules.

### 6.2 Pipeline Execution

A Pipeline Execution is one immutable attempt to execute one Build Intent against one declared source and input set.

### 6.3 Verification Session

A Verification Session is one independently meaningful evidence-producing execution within a Pipeline Execution.

### 6.4 Gate

A Gate is a governed condition that must produce an explicit evaluation before the Pipeline may cross a declared boundary.

### 6.5 Promotion

Promotion is an authority decision permitting one exact verified artifact to advance to one declared state for one declared purpose.

Promotion is not deployment.

## 7. Build Intents

Every execution SHALL declare exactly one Approved Build Intent before receiving evidence-producing inputs.

| Intent ID | Purpose | Promotion eligibility |
|---|---|---|
| `DEVELOPER` | Fast local feedback | None |
| `VALIDATION` | Governed verification of a declared scope | None |
| `RELEASE_CANDIDATE` | Produce and verify a complete candidate | Eligible after all applicable gates |
| `HOTFIX` | Produce an urgent bounded correction | Only under an Approved Hotfix Gate Profile |
| `EMERGENCY_RECOVERY` | Produce a bounded recovery artifact | Only under approved recovery jurisdiction and profile |
| `REPRODUCIBILITY` | Independently rebuild an existing artifact | None |
| `SECURITY_INVESTIGATION` | Isolated security analysis | None |

The Build Intent becomes immutable when the Pipeline Execution ID is issued.

Changing intent requires a new execution.

An artifact SHALL NOT be relabeled from one intent to another.

## 8. Gate Profiles

A Gate Profile defines which governed obligations apply to a Build Intent and target scope.

Each Gate Profile SHALL contain:

- Gate Profile ID;
- version;
- digest;
- lifecycle;
- applicable Build Intents;
- artifact classes;
- target platforms;
- required stages;
- stage dependencies;
- evidence rules;
- authority rules;
- permitted completeness state;
- validity conditions;
- waiver rules;
- challenge path; and
- approval record.

Initial profiles are:

| Gate Profile ID | Intent | Lifecycle |
|---|---|---|
| `FALCON-GATE-DEVELOPER-1` | `DEVELOPER` | `PROPOSED` |
| `FALCON-GATE-VALIDATION-1` | `VALIDATION` | `PROPOSED` |
| `FALCON-GATE-RC-1` | `RELEASE_CANDIDATE` | `PROPOSED` |
| `FALCON-GATE-REPRO-1` | `REPRODUCIBILITY` | `PROPOSED` |
| `FALCON-GATE-SECURITY-1` | `SECURITY_INVESTIGATION` | `PROPOSED` |

No Hotfix or Emergency Recovery Gate Profile is initially approved.

A Build Intent may select only a compatible `ACTIVE` Gate Profile.

## 9. Official Execution Inputs

Before evidence production, every governed execution SHALL bind:

- authorized Build Request;
- Build Intent;
- Pipeline Execution ID;
- immutable Source Revision;
- Approved Foundation Baseline;
- Pipeline Definition;
- Gate Profile;
- BLD-001 Toolchain Baseline;
- ENV-001 Environment Profile;
- Effective Build Configuration;
- locked dependency closure;
- target artifact class and platforms;
- actor and service identities;
- authority and jurisdiction evidence;
- Time Profile and Time Observations;
- Evidence Requirement Set; and
- financial-isolation declaration.

Changing any immutable input creates a new Pipeline Execution.

## 10. Pipeline Execution Identity

Every execution SHALL receive a typed identifier through the Falcon Identifier Provider Contract.

The identity SHALL bind:

- all official inputs;
- start and end observations;
- Runtime Epoch;
- Clock Quality;
- all Verification Sessions;
- produced artifact identities;
- Evidence Requirement Set;
- Root Verification Evidence Set;
- execution result;
- termination reason; and
- supersession links.

The Pipeline Execution ID does not imply success.

## 11. Evidence Requirement Set

Before any evidence-producing stage begins, the Pipeline SHALL create and seal one immutable Evidence Requirement Set snapshot.

It SHALL preserve:

- Requirement Set ID;
- Build Intent;
- Gate Profile identity;
- Pipeline identity;
- baseline and source;
- toolchain and environment;
- target scope;
- creation identity and time;
- applicable authorities;
- complete expected evidence inventory; and
- canonical digest.

Each requirement SHALL define:

- immutable Requirement ID;
- governing source;
- subject;
- scope;
- applicability;
- production mode;
- expected producer role;
- evidence schema;
- integrity requirements;
- freshness;
- independence;
- evaluation rules;
- expected platforms;
- pass criteria;
- fail criteria;
- inconclusive criteria; and
- exclusion authority where applicable.

The snapshot SHALL NOT change after evidence production begins.

## 12. Requirement Applicability

| Applicability | Meaning |
|---|---|
| `MANDATORY` | Absence makes the verification case incomplete |
| `OPTIONAL` | Absence does not prevent completeness for the declared intent |
| `CONDITIONAL` | A governed predicate determines applicability |
| `EXCLUDED` | Explicitly outside scope under authorized policy and recorded reason |
| `DERIVED` | Produced only by governed evaluation from preserved inputs |

`DERIVED` material SHALL NOT be represented as directly observed evidence.

A requirement cannot be weakened after execution begins.

If a condition cannot be evaluated reliably, the requirement SHALL be treated conservatively as applicable or unresolved according to governing policy.

## 13. Evidence Production Mode

Each requirement SHALL declare one production mode:

| Mode | Meaning |
|---|---|
| `AUTOMATED` | Produced by an identified governed tool |
| `HUMAN` | Produced by an identified authorized person |
| `HYBRID` | Produced through a governed combination of automated and human actions |

Production Mode describes how evidence is produced. It does not determine whether the evidence is valid, accepted, or promotable.

## 14. Verification Session

Every independently meaningful execution SHALL receive a Verification Session ID.

Examples:

- dependency admission;
- compilation;
- static analysis;
- one Contract suite;
- one security suite;
- one platform integration suite;
- one VPL execution;
- one reproducibility comparison; and
- one packaging verification.

Every retry and rerun receives a new Session ID.

A Verification Session SHALL record:

- parent Pipeline Execution ID;
- requirement IDs;
- producer;
- tool and version;
- environment;
- inputs;
- configuration;
- start and end Time Observations;
- observations;
- results;
- omissions;
- failures;
- logs;
- output identities;
- integrity;
- provenance; and
- termination state.

## 15. Result Vocabulary

The canonical stage and requirement result vocabulary is:

| Result | Meaning |
|---|---|
| `PASS` | Declared criteria were proven within scope |
| `FAIL` | Declared criteria were violated |
| `INCONCLUSIVE` | Evidence cannot establish pass or fail |
| `BLOCKED` | Required execution could not proceed |
| `NOT_APPLICABLE` | Governed applicability evaluation excludes the requirement |
| `NOT_RUN` | Execution did not occur |
| `ERROR` | Verification mechanism failed |

`NOT_RUN`, `ERROR`, `BLOCKED`, and `INCONCLUSIVE` SHALL NOT be converted to `PASS`.

`NOT_APPLICABLE` requires preserved applicability evidence.

## 16. Canonical Stage Graph

The Release Candidate stage order is:

0. Evidence Requirement Set Sealing.
1. Request and Authority Admission.
2. Governance and Scope.
3. Input and Environment Admission.
4. Dependency Acquisition and Admission.
5. Isolated Build.
6. Static Quality and Boundary Verification.
7. Unit, Contract, Schema, and Canonical Vector Verification.
8. Security Verification.
9. Windows and Linux Integration.
10. Fault, Degraded, Safe-State, and Recovery Verification.
11. VPL Execution.
12. Independent Reproducibility.
13. Packaging, SBOM, and Provenance.
14. Root Evidence Set Construction.
15. Independent Completeness Evaluation.
16. Validity and Acceptance Review.
17. Independent Promotion Decision.

Stages MAY execute in parallel only when:

- dependencies are explicit;
- evidence remains independently identifiable;
- shared mutable state cannot alter outcomes;
- result aggregation is deterministic; and
- failure propagation remains equivalent to the canonical graph.

## 17. Stage 0 — Requirement Set Sealing

Before any evidence-producing stage begins, this stage SHALL:

- evaluate the Build Intent and Gate Profile;
- define the evidence required to evaluate request and authority admission;
- define the evidence required to evaluate every later stage;
- resolve only those conditional obligations whose predicates are already established;
- define how unresolved conditional obligations will be conservatively evaluated;
- record authorized exclusions;
- validate schemas;
- bind expected producers and evaluators;
- bind authority requirements;
- assign Requirement IDs;
- produce the canonical manifest;
- calculate its digest; and
- seal the Evidence Requirement Set.

Later policy change SHALL NOT reinterpret the sealed historical set.

The sealing operation itself SHALL produce bootstrap evidence governed by a fixed PIPE-001 bootstrap obligation. That evidence SHALL be included in the Root Verification Evidence Set and SHALL NOT be used to alter the sealed obligations.

## 18. Stage 1 — Request and Authority Admission

This stage SHALL verify:

- request identity;
- requesting subject;
- Build Intent;
- declared scope;
- jurisdiction;
- authority;
- delegation;
- expiry and revocation;
- target baseline;
- source eligibility;
- non-financial boundary; and
- absence of self-approval.

Failure terminates the execution before acquisition.

## 19. Stage 2 — Governance and Scope

This stage SHALL verify:

- Vision and Constitution precedence;
- Approved document baseline;
- requirement traceability;
- Contract applicability;
- unresolved amendment status;
- implementation authority where execution requires it;
- forbidden capabilities;
- financial exclusions;
- target platforms;
- Gate Profile compatibility; and
- no silent scope expansion.

## 20. Stage 3 — Input and Environment Admission

This stage SHALL verify:

- clean immutable source;
- exact pipeline digest;
- BLD-001 baseline;
- ENV-001 profile;
- runner identity;
- tool identity;
- time and epoch state;
- network policy;
- filesystem isolation;
- secret absence;
- credential absence;
- market-data absence;
- production-path absence; and
- evidence-storage availability.

An ambiguous or unverified environment SHALL fail closed.

## 21. Stage 4 — Dependency Acquisition and Admission

Acquisition SHALL:

- use Approved sources;
- resolve exact locked versions;
- verify content digests and signatures;
- preserve provenance;
- evaluate licenses;
- evaluate known vulnerabilities;
- record the full transitive graph;
- enforce BCL-first and Adapter policies;
- verify layer boundaries;
- produce preliminary SBOM material; and
- create a content-identified offline bundle.

Compilation SHALL NOT occur in the acquisition environment as the official build.

## 22. Stage 5 — Isolated Build

The official build SHALL:

- use only the verified offline bundle;
- disable undeclared network access;
- use the exact BLD-001 toolchain;
- use the exact ENV-001 runner;
- prohibit uncontrolled global caches;
- fail on missing inputs;
- enforce locked restore;
- enforce deterministic properties;
- preserve compiler and analyzer output;
- produce exact artifact digests; and
- preserve complete build provenance.

Any download attempt fails the stage.

## 23. Stage 6 — Static Quality and Boundaries

This stage SHALL evaluate:

- formatting policy;
- compilation;
- nullable analysis;
- warnings as errors;
- admitted analyzers;
- prohibited APIs;
- dependency direction;
- layer boundaries;
- Adapter enforcement;
- external-library leakage;
- secret scanning;
- vulnerable-component scanning;
- generated-material provenance;
- financial dependency exclusion; and
- suppression governance.

One passing analyzer SHALL NOT compensate for another missing mandatory analyzer.

## 24. Stage 7 — Unit, Contract, Schema, and Vector Verification

This stage SHALL verify:

- unit behavior;
- CON-001 through CON-011;
- FIL schemas;
- invalid input rejection;
- FCE canonical vectors;
- identifier semantics;
- time and uncertainty semantics;
- cryptographic domain separation;
- persistence boundaries;
- communication reordering;
- duplicate-effect prevention;
- lifecycle transitions; and
- evidence semantics.

Contract and schema failures are non-waivable for Release Candidate promotion.

## 25. Stage 8 — Security Verification

This stage SHALL cover:

- unauthorized action;
- wrong identity;
- wrong jurisdiction;
- invalid delegation;
- privilege escalation;
- replay;
- expiry;
- downgrade;
- malformed input;
- key-purpose misuse;
- cryptographic-domain confusion;
- secret exposure;
- custody failure;
- evidence tampering;
- dependency substitution;
- artifact substitution;
- boundary bypass;
- abuse;
- denial and restriction behavior; and
- restoration independence.

Security tool success does not replace adversarial and Contract-based verification.

## 26. Stage 9 — Platform Integration

Windows and Linux verification SHALL cover:

- bootstrap;
- identity;
- time;
- configuration;
- FIL;
- IPC or approved transport;
- PostgreSQL;
- persistence;
- restart;
- reconciliation;
- health;
- logging;
- event integrity;
- restriction;
- recovery; and
- shutdown.

Each platform produces separate Verification Sessions and artifact identities.

## 27. Stage 10 — Fault, Degraded, and Recovery

This stage SHALL inject and verify:

- dependency absence;
- dependency corruption;
- communication interruption;
- duplicate and reordered messages;
- persistence failure;
- ambiguous commit;
- evidence loss;
- database interruption;
- clock rollback and uncertainty;
- key unavailability;
- partial startup;
- health contradiction;
- Guardian restriction;
- Safe-state entry;
- failed recovery;
- independent recovery verification; and
- controlled restoration.

Opportunity preservation SHALL NOT override capital protection, evidence integrity, or safe-state requirements.

## 28. Stage 11 — VPL Execution

VPL-001 through VPL-008 SHALL execute:

- in Approved order;
- with declared prerequisites;
- against the exact candidate;
- in exact environments;
- with independent Session IDs;
- with preserved negative and positive observations; and
- without replacing their Approved acceptance criteria.

Failure of one required VPL prevents Release Candidate promotion.

## 29. Stage 12 — Reproducibility

Independent clean environments SHALL rebuild the declared candidate from the same governed inputs.

The stage SHALL compare:

- artifact bytes where applicable;
- source identity;
- dependency closure;
- generated material;
- Contract and schema outputs;
- version and feature set;
- configuration meaning;
- provenance fields; and
- declared scope.

Unexplained difference produces `FAIL` or `INCONCLUSIVE`.

## 30. Stage 13 — Packaging, SBOM, and Provenance

This stage SHALL:

- package the exact verified artifacts;
- preserve platform distinction;
- compute artifact digests;
- generate SPDX 3.0.1 SBOMs;
- generate governed provenance;
- record dependencies and runtime;
- record limitations;
- bind evidence to artifacts;
- verify absence of secrets;
- apply approved signing where available; and
- prevent post-verification mutation.

No SLSA Claim may be made unless independently proven.

## 31. Evidence Immutability

Once recorded, evidence SHALL NOT be edited, replaced, or deleted as a correction mechanism.

A correction SHALL:

- create a new evidence identity;
- preserve the prior object;
- state the correction reason;
- identify the correcting authority;
- link to the superseded object; and
- trigger reevaluation where material.

Logs and transient observations needed to support a Claim SHALL be preserved as governed evidence, not left solely in a runner workspace.

## 32. Evaluation Context

An Evaluation Context is a governed immutable artifact capturing the authoritative policy, configuration, environment, authority, and trust state under which evaluations are performed.

It SHALL contain:

- Evaluation Context ID;
- version;
- canonical digest;
- scope;
- purpose;
- source revision;
- policy snapshot;
- configuration snapshot;
- feature flags;
- environment profile;
- toolchain baseline;
- Gate Profile;
- rule versions;
- authority state;
- delegation state;
- time state;
- trust state;
- provenance of every context element;
- lineage;
- integrity;
- lifecycle; and
- status.

Context status is one of:

- `VALID`;
- `INCOMPLETE`;
- `INVALID`;
- `CONFLICTED`;
- `STALE`; or
- `UNCERTAIN`.

Validity is always for a declared evaluation scope and governing policy.

Multiple evaluations MAY reference the same immutable Context only while no material element changes and it remains valid for their declared scope and policy.

## 33. Derived Evaluation

A Derived Evaluation SHALL identify:

- Evaluation ID;
- evaluated requirements;
- input evidence identities;
- derivation rules;
- Evaluation Context ID;
- evaluator identity;
- Evaluation Mode;
- Evaluation Nature;
- Evaluation Authority;
- outcome;
- scope;
- uncertainty;
- limitations;
- time;
- provenance;
- integrity; and
- challenge path.

Evaluation Mode is:

- `AUTOMATED`;
- `HUMAN`; or
- `HYBRID`.

Evaluation Nature is:

- `DETERMINISTIC`;
- `PROBABILISTIC`; or
- `JUDGMENT_BASED`.

Every deterministic evaluation SHALL be reproducible from preserved evidence, rules, and Context.

AI evaluations SHALL explicitly declare their nature. Unless deterministic behavior is demonstrated under a governed execution profile, they SHALL be treated as `JUDGMENT_BASED`.

## 34. Root Verification Evidence Set

Every governed execution SHALL produce at most one Root Verification Evidence Set for one candidate scope.

It SHALL preserve:

1. Verification Obligations.
2. Observed Evidence.
3. Derived Evaluations.
4. Evaluation Context.
5. Completeness Evaluation.
6. Validity and Acceptance Context.
7. Promotion Context.
8. Integrity and provenance.

The Root Set preserves both the evidence and the obligations against which it was evaluated.

It SHALL identify:

- Root Evidence Set ID;
- Pipeline Execution ID;
- candidate artifact identities;
- Evidence Requirement Set;
- included and missing evidence;
- failed and inconclusive evidence;
- superseded evidence;
- completeness state;
- Context identity;
- evaluator identities;
- authority identities;
- canonical digest;
- signatures where approved;
- limitations;
- challenge state; and
- lifecycle.

## 35. Evidence Completeness

Completeness states are:

| State | Meaning |
|---|---|
| `COMPLETE` | Every applicable obligation is satisfied by validly included evidence |
| `PARTIAL` | The Build Intent permits declared non-required omissions |
| `INCOMPLETE` | Required evidence is absent, unresolved, blocked, or not evaluable |
| `INVALID` | Evidence Set integrity, provenance, identity, or governing context is not trustworthy |

Only `COMPLETE` Root Evidence Sets may satisfy Release Candidate promotion.

No producer, transformer, aggregator, or signer of evidence may be the sole Evidence Completeness Authority.

## 36. Validity, Acceptance, and Reliance

Validity assesses whether a Trust Object is fit for a declared scope under defined rules.

Acceptance is an authority decision to rely on that assessment for a specific purpose.

Reliance SHALL remain bounded by:

- scope;
- purpose;
- governing policy;
- validity conditions;
- time;
- jurisdiction;
- authority; and
- the Acceptance Decision.

`VALID` does not require acceptance.

Acceptance does not create wider reliance.

## 37. Promotion

Every promotion decision SHALL reference exactly one Root Verification Evidence Set.

Individual sessions, selected reports, verbal assurances, dashboards, or partial evidence collections SHALL NOT serve directly as promotion evidence.

Promotion requires:

- exact artifact identity;
- eligible Build Intent;
- compatible active Gate Profile;
- `COMPLETE` Root Evidence Set;
- valid Evaluation Context;
- all non-waivable gates passed;
- accepted validity assessments;
- no unresolved material challenge;
- competent Promotion Authority;
- verified jurisdiction and delegation;
- declared destination state;
- declared reliance scope;
- decision time and time quality; and
- immutable Promotion Decision.

Promotion SHALL NOT rebuild, repackage, resign with different material, or otherwise mutate the artifact.

Mutation creates a new artifact requiring a new verification case.

## 38. Authority Separation

The Pipeline SHALL distinguish:

- Execution Authority;
- Evidence Production Authority;
- Evaluation Authority;
- Evidence Completeness Authority;
- Acceptance Authority;
- Promotion Authority; and
- Challenge Resolution Authority.

Execution produces observations and results.

Evaluation Authority legitimizes evaluation outcomes within a defined scope.

Evidence Completeness Authority determines whether the required verification case is complete.

Promotion Authority alone determines whether the verified artifact may advance.

No authority acquires jurisdiction through delegation where no jurisdiction exists.

For material decisions, the same subject SHALL NOT be the sole authority across incompatible roles.

## 39. Challenge

Every material Claim, evaluation, completeness decision, Acceptance Decision, and Promotion Decision SHALL remain independently challengeable.

A Challenge SHALL:

- receive an identity;
- identify the challenged object and Claim;
- state scope and grounds;
- preserve evidence;
- identify potential conflicts;
- invoke competent jurisdiction;
- produce an explicit resolution; and
- preserve all prior states.

A Challenge SHALL NOT be conclusively resolved solely by:

- the producer of the challenged Claim;
- the evaluator whose outcome is challenged; or
- the authority whose decision is challenged,

unless governing policy explicitly permits it for a documented low-impact case.

A material unresolved Challenge blocks promotion or continued reliance as governing policy requires.

## 40. Retry, Flakiness, and Reexecution

Retries SHALL:

- receive new Verification Session IDs;
- preserve original results;
- preserve reason and authority;
- use the same immutable inputs or declare changed inputs;
- avoid selecting only favorable outcomes; and
- trigger completeness and validity reevaluation.

A flaky test is a failed reliability property.

Repeated execution does not turn nondeterminism into proof.

Changing an immutable input requires a new Pipeline Execution.

## 41. Generated and AI-Produced Material

Generated or AI-produced source, configuration, tests, documentation, evidence, or evaluation SHALL:

- be explicitly identified;
- preserve generator identity and version;
- preserve prompt or governing input where lawful and safe;
- preserve model or engine profile;
- preserve output digest;
- undergo the same review and verification as human-produced material;
- declare deterministic, probabilistic, or judgment-based nature;
- remain challengeable; and
- never self-approve.

AI output is an input or Claim. It is not authority.

## 42. Waivers

A waiver SHALL NOT:

- override the Vision or Constitution;
- grant missing jurisdiction;
- bypass Guardian;
- waive artifact identity;
- waive evidence integrity;
- waive authority separation;
- waive financial isolation;
- turn missing evidence into pass;
- turn an invalid Root Evidence Set into complete;
- suppress a Contract failure for Release Candidate promotion; or
- authorize an inactive cryptographic, identity, time, build, or environment profile.

Every permitted waiver SHALL be:

- explicit;
- narrow;
- attributable;
- time-bounded;
- risk-assessed;
- evidence-backed;
- approved within jurisdiction;
- included in the Root Evidence Set; and
- independently challengeable.

## 43. Failure and Termination

Pipeline execution SHALL fail, block, or become inconclusive when:

- authority is absent or invalid;
- jurisdiction is unproven;
- scope is ambiguous;
- a mandatory input is missing;
- the source is mutable;
- the environment is unverified;
- a tool or dependency differs;
- isolation fails;
- financial connectivity is possible;
- the Requirement Set cannot be sealed;
- mandatory evidence is missing;
- a stage fails;
- evidence integrity fails;
- Context is invalid;
- artifact identity changes;
- completeness is not sufficient;
- a material Challenge is unresolved; or
- promotion authority is invalid.

Termination SHALL preserve:

- all produced evidence;
- the exact failure point;
- incomplete obligations;
- artifacts marked non-promotable;
- authority state;
- cleanup outcome; and
- recovery or rerun eligibility.

Failure SHALL NOT trigger automatic weakening, fallback to local tools, or a different Gate Profile.

## 44. Portability and Provider Independence

The canonical Pipeline SHALL be executable:

- in Approved local isolated environments;
- on Approved Windows runners;
- on Approved Linux runners; and
- through compliant automation providers.

Provider adapters MAY:

- schedule;
- provision an Approved environment;
- inject authorized sealed inputs;
- invoke the canonical entry point;
- collect already-governed outputs; and
- report status.

Provider adapters SHALL NOT:

- redefine stages;
- omit required evidence;
- change result meaning;
- select tools;
- alter authority;
- mutate artifacts;
- decide completeness;
- decide promotion; or
- become the sole evidence store.

## 45. Pipeline Change Governance

Any change to:

- stage graph;
- Build Intent;
- Gate Profile;
- result vocabulary;
- evidence schema;
- evaluation rule;
- completeness rule;
- authority rule;
- waiver rule;
- provider boundary; or
- promotion rule

requires:

- versioned proposal;
- traceability analysis;
- risk analysis;
- compatibility assessment;
- security review;
- verification;
- Approval; and
- explicit Activation.

Historical executions SHALL remain interpreted under the exact Pipeline and Gate Profile used when their Requirement Sets were sealed.

## 46. Pipeline Requirements

- **PIPE-001-REQ-001:** Every execution SHALL declare exactly one Approved Build Intent.
- **PIPE-001-REQ-002:** Every governed execution SHALL bind one immutable source, baseline, pipeline, toolchain, environment, configuration, and Gate Profile.
- **PIPE-001-REQ-003:** Every execution SHALL receive a typed Pipeline Execution ID.
- **PIPE-001-REQ-004:** An immutable Evidence Requirement Set SHALL be sealed before evidence production.
- **PIPE-001-REQ-005:** Requirement applicability SHALL use only the governed vocabulary.
- **PIPE-001-REQ-006:** Derived evidence SHALL remain distinguishable from observed evidence.
- **PIPE-001-REQ-007:** Every meaningful verification execution SHALL receive a Verification Session ID.
- **PIPE-001-REQ-008:** Retries SHALL preserve all prior sessions and results.
- **PIPE-001-REQ-009:** Dependency acquisition SHALL remain separate from isolated build.
- **PIPE-001-REQ-010:** Isolated build SHALL use only verified offline inputs.
- **PIPE-001-REQ-011:** The canonical Release Candidate stage graph SHALL be enforced.
- **PIPE-001-REQ-012:** Windows and Linux results SHALL remain separately attributable.
- **PIPE-001-REQ-013:** Evidence SHALL be immutable; corrections SHALL create linked new objects.
- **PIPE-001-REQ-014:** Every Derived Evaluation SHALL bind an immutable Evaluation Context.
- **PIPE-001-REQ-015:** Context validity SHALL be scoped to purpose and governing policy.
- **PIPE-001-REQ-016:** Every governed candidate SHALL have one Root Verification Evidence Set.
- **PIPE-001-REQ-017:** The Root Set SHALL preserve obligations, evidence, evaluation, Context, completeness, and decision context.
- **PIPE-001-REQ-018:** Only a `COMPLETE` Root Set may satisfy Release Candidate promotion.
- **PIPE-001-REQ-019:** Promotion SHALL reference exactly one Root Verification Evidence Set.
- **PIPE-001-REQ-020:** Promotion SHALL apply only to the exact verified artifact.
- **PIPE-001-REQ-021:** Validity, Acceptance, Reliance, and Promotion SHALL remain distinct.
- **PIPE-001-REQ-022:** Jurisdiction SHALL be verified before authority.
- **PIPE-001-REQ-023:** Evidence production, evaluation, completeness, acceptance, and promotion authority SHALL remain separated.
- **PIPE-001-REQ-024:** Material Claims and decisions SHALL remain independently challengeable.
- **PIPE-001-REQ-025:** Missing, failed, blocked, inconclusive, or invalid evidence SHALL NOT become pass.
- **PIPE-001-REQ-026:** AI-produced material SHALL declare provenance and Evaluation Nature and SHALL NOT self-approve.
- **PIPE-001-REQ-027:** Provider adapters SHALL NOT redefine Pipeline meaning.
- **PIPE-001-REQ-028:** Pipeline failure SHALL preserve evidence and SHALL NOT weaken gates automatically.
- **PIPE-001-REQ-029:** No execution may reach financial systems, credentials, data, or capital.
- **PIPE-001-REQ-030:** Approval of PIPE-001 SHALL NOT activate the Pipeline or authorize implementation.

## 47. Conformance Evidence

Conformance requires evidence that:

- intent cannot change after execution identity;
- a Gate Profile cannot be weakened during execution;
- Requirement Sets remain immutable;
- historical evidence is evaluated under its original obligations;
- missing mandatory evidence prevents completeness;
- derived outputs cannot masquerade as observations;
- failed sessions remain visible after retry;
- acquisition and build are isolated;
- network download during build fails;
- provider configuration cannot omit a stage;
- Windows and Linux evidence remains distinct;
- an invalid Context prevents applicable evaluation;
- deterministic evaluations can be reproduced;
- AI evaluation cannot default to deterministic;
- individual sessions cannot authorize promotion;
- a partial or invalid Root Set cannot satisfy Release Candidate promotion;
- artifact mutation invalidates promotion eligibility;
- a producer cannot solely declare completeness;
- invalid jurisdiction defeats delegated authority;
- material challenges receive independent resolution;
- waivers cannot override non-waivable rules;
- failure preserves evidence; and
- financial paths remain unreachable.

## 48. Required Before Activation

No Gate Profile or Pipeline Definition becomes `ACTIVE` until:

1. PIPE-001 is Approved and registered;
2. BLD-001 remains Approved;
3. BLD-001 mandatory tool blocks are resolved;
4. TRC-001 is Approved;
5. ENV-001 is Approved for Windows and Linux;
6. Identifier and Time Provider Contracts required by the Pipeline are Approved and active;
7. evidence schemas and canonical representations are Approved;
8. authority assignments and jurisdictions are recorded;
9. Evidence Completeness and Promotion Authorities are independently appointed;
10. positive, negative, failure, challenge, and recovery verification passes;
11. pipeline portability is proven outside any one CI provider;
12. financial isolation is independently proven;
13. a complete activation Evidence Set is accepted; and
14. explicit implementation authority is granted.

## 49. Deliberate Non-Activation

At version 1.0:

- all initial Gate Profiles remain `PROPOSED`;
- no Hotfix Gate Profile exists;
- no Emergency Recovery Gate Profile exists;
- no Pipeline runner is active;
- no authority is appointed by this document;
- no evidence schema is activated by this document;
- no artifact may be promoted; and
- no code or pipeline implementation is authorized.

## 50. Foundational Rules

> **Evidence without its governing obligation is incomplete context.**

> **An obligation without evaluated evidence is an unproven Claim.**

> **An evaluation without preserved evidence and governing rules is an unverifiable conclusion.**

> **Execution produces observations and results. Authority governs what may be concluded and relied upon.**

> **No successful stage compensates for a failed invariant.**

> **The exact verified artifact—or nothing—may advance.**

## 51. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner and current Constitutional Authority | Approved | GOV-020 | 2026-07-25 |

Approval approves the Pipeline rules and initial proposed Gate Profile definitions.

It does not:

- activate a Gate Profile;
- activate a runner;
- resolve BLD-001 deliberate blocks;
- approve TRC-001 or ENV-001;
- appoint an authority;
- authorize implementation;
- authorize promotion;
- authorize production; or
- authorize financial activity.
