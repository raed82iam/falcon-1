# PIPE-001 — Foundation Pipeline Specification

**Identifier:** PIPE-001  
**Version:** 1.1  
**Status:** Approved  
**Effective Date:** 2026-07-25  
**Approval and Activation Record:** GOV-039  
**Amendment Package:** AMD-003  
**Owner:** Falcon Foundation Governance  
**Governing Authority:** Falcon Vision; Falcon Constitution; ADR-I007; ADR-I008; AMD-003; SEC-002; GOV-AUT-001; AUT-001 v1.1; CON-012; CON-013; CON-020; CON-021; BLD-001 v1.1; ENV-001 v1.1; IDN-001 v1.1; TIM-001 v1.1; CRY-001 v1.1; DESIGN-SEC-001 v1.1; VPL-BST-000 through VPL-BST-008  
**Initial Execution Platform:** Windows  
**Future Deployment Direction:** Provider-neutral, including separately admitted Oracle Cloud Infrastructure  
**Implementation Authority:** Not Granted  
**Supersedes:** PIPE-001 v1.0  
**Superseded By:** None

## 1. Purpose

PIPE-001 v1.1 defines the governed path from externally controlled Foundation preparation through enabling-Provider candidate verification and, only after exact Activation, into the canonical Foundation Pipeline defined by v1.0.

It closes the bootstrap circularity without allowing preparation, candidates, tools, environments, evidence, or Pipeline machinery to establish their own authority.

## 2. Preserved Specification

Unless expressly amended here, PIPE-001 v1.0 remains controlling for:

- Build Intents and Gate Profiles;
- Pipeline Definitions, Executions, Verification Sessions, and stage results;
- Evidence Requirement Sets;
- observed evidence and Derived Evaluations;
- Evaluation Contexts and Root Verification Evidence Sets;
- completeness, validity, acceptance, reliance, challenge, and promotion;
- artifact identity, immutability, reproducibility, SBOM, and provenance;
- role and authority separation;
- retries, waivers, AI-produced material, failure, and portability;
- all requirements `PIPE-001-REQ-001` through `PIPE-001-REQ-030`; and
- all deliberate non-activation conditions.

No Gate Profile, runner, Pipeline Definition, Provider, or environment becomes active through this amendment.

## 3. Pipeline Operating Modes

PIPE-001 recognizes two strictly separated modes:

| Mode | Purpose | Authority result |
|---|---|---|
| `BOOTSTRAP` | Prepare exact tools and environments; verify enabling Provider candidates; assemble Activation evidence | Cannot build general Falcon, promote artifacts, or grant operational authority |
| `GOVERNED` | Execute the canonical v1.0 Pipeline after its prerequisites and exact subjects are active | May produce governed verification cases; promotion still requires separate authority |

One Pipeline Execution SHALL have exactly one immutable mode.

An execution, artifact, session, result, or Evidence Set SHALL NOT be relabeled from `BOOTSTRAP` to `GOVERNED`.

## 4. Bootstrap Build Intents

The following Build Intents are added:

| Intent ID | Purpose | Promotion eligibility |
|---|---|---|
| `FOUNDATION_PREPARATION` | Acquire, verify, seal, and prepare exact tool, dependency, and environment candidates | None |
| `ENABLING_PROVIDER_CANDIDATE` | Construct and verify only explicitly authorized enabling Provider and Adapter candidates | None |
| `BOOTSTRAP_RECONSTRUCTION` | Independently reconstruct bootstrap evidence and Activation decisions | None |

These Intents:

- require one competent Authority Instrument;
- require one immutable CON-020 context;
- use only the subject and actions enumerated by that authority;
- prohibit general Foundation implementation;
- prohibit production and financial inputs, connectivity, and consequences;
- prohibit artifact promotion; and
- terminate when their bounded purpose or authority ends.

## 5. Bootstrap Gate Profiles

PIPE-001 v1.1 defines:

| Gate Profile ID | Intent | Lifecycle |
|---|---|---|
| `FALCON-GATE-BOOTSTRAP-PREP-1` | `FOUNDATION_PREPARATION` | `APPROVED`, not `ACTIVE` |
| `FALCON-GATE-BOOTSTRAP-PROVIDER-1` | `ENABLING_PROVIDER_CANDIDATE` | `APPROVED`, not `ACTIVE` |
| `FALCON-GATE-BOOTSTRAP-RECON-1` | `BOOTSTRAP_RECONSTRUCTION` | `APPROVED`, not `ACTIVE` |

Approval establishes Gate meaning only.

Each Gate Profile requires a separate exact Activation Decision before use.

No bootstrap Gate may waive or satisfy a governed Release Candidate Gate.

## 6. Bootstrap Identity

Before active Falcon Identifier and Time Providers:

- Pipeline Execution, Verification Session, Evidence Requirement Set, Evidence Set, context, and artifact identities SHALL use attributable external identifiers;
- each external identifier SHALL be marked `BOOTSTRAP_EXTERNAL_ID`;
- time observations SHALL remain `BOOTSTRAP_EXTERNAL`;
- Runtime Epoch and continuity limits SHALL remain external and explicit;
- external identifiers SHALL be unique within the declared external scheme and scope;
- later Falcon IDs SHALL cross-link rather than replace historical external IDs; and
- import SHALL not upgrade origin, validity, completeness, acceptance, or authority.

Bootstrap external identity establishes attribution only.

## 7. Bootstrap Pipeline Inputs

Every bootstrap execution SHALL bind:

- one authorized request;
- one bootstrap Build Intent;
- one active bootstrap Gate Profile;
- one Authority Instrument and Authority Chain;
- one immutable CON-020 Bootstrap Execution Context;
- exact subject, candidate, Adapter, tool, dependency, source, and configuration identities;
- one approved BLD-001 baseline class;
- one exact ENV-001 candidate environment;
- one immutable Evidence Requirement Set;
- external identity, time, and Runtime Epoch;
- synthetic-only data, identity, secret, certificate, key, and randomness classes;
- network, storage, consequence, duration, and resource limits;
- stop and cleanup conditions;
- evidence export destination; and
- explicit non-authorities.

Changing an immutable input creates a new external Pipeline Execution identity.

## 8. Bootstrap Evidence Requirement Set

Before any evidence-producing action, the bootstrap control SHALL seal one immutable Evidence Requirement Set.

It SHALL include obligations for:

- authority and jurisdiction;
- context integrity;
- exact subject and environment identity;
- tool and dependency provenance;
- synthetic-material enforcement;
- network, storage, production, and financial isolation;
- candidate and external-control separation;
- positive, negative, failure, recovery, and cleanup behavior;
- evidence origin, integrity, custody, export, and reconstruction;
- independence and shared-dependency disclosure;
- self-certification prohibition;
- Activation prerequisites; and
- explicit non-authorities.

The candidate subject SHALL NOT define or weaken its own obligations.

## 9. Bootstrap Stage Graph

The canonical bootstrap flow is:

```text
0. Authority Instrument and Context Admission
1. Evidence Requirement Set Sealing
2. Preparation Environment Admission
3. Tool and Dependency Bundle Verification
4. Candidate Subject Admission
5. Enabling Provider Candidate Verification
6. Environment Candidate Verification
7. Pipeline and Trace Candidate Verification
8. Cleanup and Evidence Export
9. Independent Evidence Reconstruction
10. Completeness and Scoped Validity Evaluation
11. Separate Exact Activation Decision
```

Stages MAY be skipped only when the selected bootstrap Intent and sealed Evidence Requirement Set explicitly classify them as not applicable.

A later stage SHALL NOT repair a broken authority, provenance, isolation, identity, or evidence boundary from an earlier stage.

## 10. Stage 0 — Authority and Context Admission

This stage SHALL verify:

- declared Build Intent and Gate Profile;
- jurisdiction and competent authority;
- Authority Instrument identity, scope, validity, delegation, expiry, revocation, and consequence limits;
- complete Authority Chain;
- CON-020 context identity and integrity;
- exact subjects and permitted actions;
- prohibited actions;
- external bootstrap control;
- independent stop capability; and
- explicit non-authorities.

Failure SHALL stop before candidate preparation or execution.

## 11. Stage 1 — Requirement Set Sealing

The external bootstrap control SHALL:

- derive obligations from the approved documents and selected VPL-BST plan;
- bind exact subjects, environments, tools, roles, and authorities;
- preserve mandatory, optional, conditional, excluded, and derived classifications;
- define evaluation and completeness authorities;
- define pass, fail, inconclusive, blocked, and stopped criteria;
- produce a canonical manifest and digest; and
- seal the snapshot before evidence production.

Sealing evidence SHALL itself be preserved under a fixed bootstrap obligation and SHALL not permit the sealer to declare completeness alone.

## 12. Stages 2 and 3 — Preparation and Bundle Verification

Preparation and tool acquisition SHALL conform to:

- BLD-001 v1.1;
- ENV-001 v1.1;
- CON-020;
- CON-021;
- VPL-BST-001; and
- VPL-BST-002.

The acquisition or preparation environment:

- MAY produce sealed candidate inputs and provenance;
- SHALL NOT produce an official Foundation release artifact;
- SHALL NOT become the verification environment by reuse;
- SHALL NOT introduce undeclared network, host, user, cache, credential, or tool state; and
- SHALL NOT certify its own outputs as admitted.

## 13. Stages 4 and 5 — Enabling Provider Candidates

Only candidates explicitly named by the Authority Instrument may enter:

- Identifier Provider under CON-014 and IDN-001 v1.1;
- Time Provider under CON-015 and TIM-001 v1.1;
- Cryptographic Provider under CON-016;
- Secret Provider under CON-017;
- Certificate and Identity Provider under CON-018;
- Randomness Provider under CON-019; and
- their exact Falcon Adapters.

Verification SHALL follow VPL-BST-003 through VPL-BST-005 as applicable.

Candidate success produces observations. It does not establish Activation, operational trust, completeness, authority, or fitness.

## 14. Stage 6 — Environment Candidate Verification

The first Environment Activation case SHALL target the exact Windows Foundation candidate established under ENV-001 v1.1.

VPL-BST-006 SHALL verify:

- exact image, update, package, tool, configuration, network, storage, identity, time, custody, and evidence boundaries;
- no production or financial path;
- failure, recovery, cleanup, and evidence export;
- candidate dependency states;
- Windows-scoped validity; and
- independent Activation authority.

Windows evidence SHALL not imply Linux or Oracle Cloud validity.

Linux and future Oracle Cloud Pipeline execution require separately admitted and activated Environment Profiles.

## 15. Stage 7 — Pipeline and Trace Candidate Verification

VPL-BST-007 SHALL prove that the candidate Pipeline and trace artifacts:

- enforce the declared Intent and Gate Profile;
- reject altered definitions and Requirement Sets;
- preserve evidence origin and immutability;
- bind exact artifacts and contexts;
- separate production, evaluation, completeness, acceptance, and promotion roles;
- preserve failures and retries;
- prevent stage omission and result reinterpretation;
- prevent self-promotion and self-Activation;
- remain independent of the runner and automation provider; and
- cannot reach a financial system.

The Pipeline candidate SHALL not execute its own final Activation as the sole control.

## 16. Stages 8 and 9 — Cleanup, Export, and Reconstruction

Cleanup and export SHALL:

- stop candidate execution;
- revoke temporary authority and synthetic material;
- preserve all required original and derived evidence;
- record incomplete removal and residual uncertainty;
- verify transfer identity and integrity;
- prevent exported evidence from returning as an undeclared input; and
- obtain independent cleanup confirmation.

VPL-BST-008 SHALL allow an independent reviewer to reconstruct:

- every authority and context;
- every subject, input, tool, environment, session, and output;
- every evidence obligation and item;
- every failure, omission, retry, Challenge, and cleanup result;
- every evaluation and context;
- every completeness and validity decision; and
- every Activation decision and non-authority.

Unreconstructable material cannot produce `PASS`.

## 17. Stages 10 and 11 — Evaluation and Activation

One Root Verification Evidence Set SHALL preserve the complete case and its governing obligations.

Activation requires:

1. a `COMPLETE` Root Verification Evidence Set;
2. valid evidence integrity and provenance;
3. scoped validity assessment;
4. competent acceptance within jurisdiction;
5. no unresolved material Challenge;
6. independent completeness authority;
7. exact subject, version, environment, purpose, scope, and validity conditions; and
8. a separate Activation Authority Instrument and Decision.

Promotion is not available in `BOOTSTRAP` mode.

Activation of a Provider, Environment Profile, Gate Profile, Pipeline Definition, or trace subject SHALL be a separate decision for each exact subject.

## 18. Transition to Governed Mode

`GOVERNED` Pipeline mode remains blocked until:

- enabling Provider dependencies required by the execution are active;
- the exact Windows Foundation Environment Profile is active;
- the applicable Gate Profile and Pipeline Definition are active;
- canonical encoding and evidence schemas are active;
- traceability is complete;
- authorities and jurisdictions are appointed;
- VPL-BST-000 through VPL-BST-008 pass;
- bootstrap evidence is independently reconstructable; and
- explicit execution or implementation authority exists for the requested scope.

The transition SHALL preserve historical bootstrap evidence as external.

No bootstrap artifact, identity, time observation, Provider, secret, environment, or Gate becomes operational through relabeling.

## 19. Post-Activation Bootstrap Prohibition

Once a governed capability is active:

- the active Falcon Provider SHALL satisfy that dependency;
- bootstrap substitutes SHALL be rejected;
- external IDs and time SHALL not serve as operational values;
- candidate Providers and Gate Profiles SHALL not serve governed execution;
- local or platform-default tools SHALL not replace the active baseline;
- direct runner or cloud-provider controls SHALL not redefine Pipeline meaning; and
- failure SHALL restrict the affected capability rather than revive a weaker bootstrap path.

Bootstrap may be used later only for a separately authorized recovery or replacement case whose policy explicitly permits it and whose outputs remain non-operational until independently activated.

## 20. Windows and Future Oracle Cloud

The initial governed Pipeline target SHALL be Windows.

Pipeline semantics SHALL remain independent of:

- Windows;
- local workstation state;
- virtual-machine provider;
- CI provider;
- cloud provider;
- Oracle Cloud Infrastructure; and
- any vendor SDK, runner label, account, region, storage, identity, or evidence service.

Future Oracle Cloud Pipeline execution SHALL require:

- an admitted OCI Environment Profile;
- exact cloud and Adapter identities;
- independent network, identity, custody, storage, time, evidence, failure, recovery, and exit verification;
- proof that OCI automation cannot redefine stages, gates, results, evidence, authority, completeness, or promotion; and
- a separate exact Activation Decision.

## 21. Self-Awareness and Pipeline Evolution

Self-Awareness MAY:

- observe Pipeline health, drift, failures, evidence gaps, dependency changes, and recurring uncertainty;
- diagnose bounded causes;
- recommend new tests, tool updates, Gate changes, environment changes, or restrictions;
- prepare proposals and candidate changes;
- initiate separately authorized maintenance workflows; and
- monitor rollback and post-change evidence.

Self-Awareness SHALL NOT:

- change its own Pipeline Definition or Gate Profile in place;
- weaken or reinterpret sealed obligations;
- convert failure, missing evidence, or uncertainty to `PASS`;
- declare its own evidence complete;
- accept or promote its own changes;
- expand its jurisdiction or authority;
- bypass Guardian or independent verification; or
- deploy, connect, or act financially without separate authority.

Every self-maintenance or evolution change remains a new immutable candidate governed by the same Pipeline.

## 22. Failure and Protective Stop

Bootstrap or governed execution SHALL fail, block, become inconclusive, or stop when:

- authority, jurisdiction, context, identity, or scope is invalid;
- a required Profile, Provider, environment, or tool is inactive or mismatched;
- an immutable input changes;
- evidence obligations cannot be sealed;
- isolation, provenance, integrity, custody, or evidence retention fails;
- production or financial data, credentials, connectivity, or consequences appear;
- a candidate attempts self-certification, self-promotion, or self-Activation;
- mandatory evidence is missing or invalid;
- a material Challenge remains unresolved; or
- cleanup cannot be established.

Failure SHALL preserve all observations, obligations, outputs, limitations, and residual uncertainty.

It SHALL NOT change the Intent, weaken the Gate, widen authority, substitute a Provider, or retry automatically outside an approved retry policy.

## 23. Requirements Added

- **PIPE-001-REQ-031:** Every Pipeline Execution SHALL declare exactly one immutable mode: `BOOTSTRAP` or `GOVERNED`.
- **PIPE-001-REQ-032:** Bootstrap executions SHALL use only `FOUNDATION_PREPARATION`, `ENABLING_PROVIDER_CANDIDATE`, or `BOOTSTRAP_RECONSTRUCTION`.
- **PIPE-001-REQ-033:** Every bootstrap execution SHALL reference one competent Authority Instrument and immutable CON-020 context.
- **PIPE-001-REQ-034:** Bootstrap identities and time SHALL remain external and shall not be upgraded through import.
- **PIPE-001-REQ-035:** A bootstrap Evidence Requirement Set SHALL be sealed before evidence-producing action.
- **PIPE-001-REQ-036:** Candidate subjects SHALL NOT define, weaken, complete, validate, accept, promote, or activate their own verification case.
- **PIPE-001-REQ-037:** Preparation and acquisition outputs SHALL require independent admission before use.
- **PIPE-001-REQ-038:** Enabling Provider candidates SHALL be limited to exact subjects under CON-014 through CON-019.
- **PIPE-001-REQ-039:** The first Environment Activation case SHALL be Windows-scoped and SHALL not imply Linux or OCI validity.
- **PIPE-001-REQ-040:** VPL-BST-000 through VPL-BST-008 SHALL govern bootstrap verification and reconstruction.
- **PIPE-001-REQ-041:** Bootstrap mode SHALL not promote a release artifact or grant implementation, operational, or financial authority.
- **PIPE-001-REQ-042:** Activation SHALL be a separate exact decision for each Provider, Profile, environment, Gate, Pipeline, and trace subject.
- **PIPE-001-REQ-043:** Governed mode SHALL remain blocked until all applicable prerequisites are active and bootstrap evidence is reconstructable.
- **PIPE-001-REQ-044:** An active governed dependency SHALL not fall back to bootstrap, candidate, local-default, or weaker substitutes.
- **PIPE-001-REQ-045:** Pipeline semantics SHALL remain independent of Windows, Oracle Cloud, and every runner or automation provider.
- **PIPE-001-REQ-046:** OCI execution SHALL require its own admitted environment, complete evidence, and exact Activation.
- **PIPE-001-REQ-047:** Pipeline failure SHALL preserve evidence and SHALL not weaken gates, expand authority, change Intent, or invoke undeclared retry.
- **PIPE-001-REQ-048:** Self-Awareness SHALL not alter sealed obligations, accept its own changes, or expand Pipeline authority.
- **PIPE-001-REQ-049:** Every self-maintenance or evolution change SHALL be treated as a new immutable candidate.
- **PIPE-001-REQ-050:** Approval of PIPE-001 v1.1 SHALL not activate a Gate, Pipeline, runner, Provider, environment, or authorize execution or implementation.

## 24. Required Before Bootstrap Gate Activation

No bootstrap Gate Profile becomes active until:

1. PIPE-001 v1.1 is Approved;
2. BLD-001 v1.1 and ENV-001 v1.1 are Approved;
3. CON-012, CON-020, and CON-021 are Approved;
4. the applicable VPL-BST plan is Approved;
5. exact external identity, time, evidence, environment, and control mechanisms are known;
6. Gate-specific obligations and stop conditions are complete;
7. financial isolation is independently verified;
8. Gate enforcement and self-certification rejection are verified;
9. competent Gate Activation Authority exists; and
10. a separate exact Activation Decision is issued.

## 25. Required Before Governed Pipeline Activation

The canonical governed Pipeline remains non-active until:

1. the complete bootstrap verification plan set passes;
2. applicable Providers and the exact Windows Environment Profile are active;
3. the Pipeline Definition and Gate Profile identities are exact;
4. evidence schemas, canonical encoding, and traceability are complete;
5. authority roles and jurisdictions are recorded;
6. positive, negative, failure, challenge, retry, cleanup, and reconstruction cases pass;
7. portability outside one runner provider is proven;
8. the Root Verification Evidence Set is `COMPLETE`;
9. material Challenges are resolved; and
10. a separate exact Pipeline Activation Decision is issued.

## 26. Supersession

With this Approval:

- PIPE-001 v1.1 supersedes v1.0;
- all v1.0 requirements and Pipeline semantics not expressly amended remain controlling;
- the bootstrap Pipeline path and Gate definitions become governed but not active;
- Windows becomes the ordered first governed environment target;
- Oracle Cloud remains a future separately admitted execution environment;
- no runner, Gate, Pipeline, Provider, environment, artifact, or evidence case is created or activated; and
- implementation, production, promotion, and financial authority remain ungranted.

## 27. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner and current Constitutional Authority | Approved and Activated | GOV-039 | 2026-07-25 |

This Approval adopts PIPE-001 v1.1 as the controlling Foundation Pipeline Specification and archives v1.0.

It does not:

- activate a bootstrap or governed Gate Profile;
- activate a Pipeline Definition or runner;
- issue an Authority Instrument or Bootstrap Execution Context;
- execute preparation, verification, build, promotion, deployment, or migration;
- activate a Provider or environment;
- authorize implementation;
- authorize production;
- authorize financial connectivity; or
- authorize financial activity.
