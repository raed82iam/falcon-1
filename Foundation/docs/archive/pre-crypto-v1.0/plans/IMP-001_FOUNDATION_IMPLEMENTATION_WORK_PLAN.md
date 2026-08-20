# IMP-001 — Foundation Implementation Work Plan

**Version:** 1.0  
**Status:** Proposed  
**Effective Date:** Pending  
**Approval Record:** Pending  
**Owner:** Falcon Foundation Delivery Authority  
**Governing Authority:** FRS-001 and the Approved Falcon Foundation Baseline  
**Verification Authority:** STD-008; VPL-000 through VPL-008  
**Implementation Authority:** Not Granted  
**Supersedes:** None  
**Superseded By:** None

## 1. Purpose

This plan defines the controlled path from the Approved Falcon Foundation Baseline to the first non-financial executable FRS-001 demonstration.

It does not authorize implementation by its existence or approval. Source implementation begins only after the Project Owner records explicit implementation authorization and all entry gates in this plan are satisfied.

## 2. Required Outcome

The completed Foundation shall demonstrate:

1. trusted startup from an approved baseline;
2. unique instance and Core identities;
3. governed configuration;
4. default-deny authority;
5. authoritative lifecycle;
6. valid FIL communication and immutable events;
7. protected state and evidence;
8. evidence-based Health, Self-Awareness, and Fitness;
9. independently enforceable Guardian restriction and Safe state;
10. controlled recovery with independent validation; and
11. complete reconstruction under VPL-008.

No outcome may be described as financial, trading, production, enterprise, autonomous-evolution, or live-capital readiness.

## 3. Absolute Scope Boundary

The work SHALL NOT contain or connect:

- market or reference data;
- broker, exchange, venue, bank, custodian, or financial institution connectivity;
- trading, order, execution, position, portfolio, allocation, valuation, or reconciliation behavior;
- live capital, financial credentials, or production secrets;
- prediction, strategy, adaptive intelligence, or autonomous production promotion;
- third-party plugin execution;
- distributed operation or high-availability claims; or
- any path capable of external financial consequence.

Detection of such a path stops the work, preserves evidence, and requires constitutional and security review.

## 4. Entry Gates

No source implementation may begin until:

1. IMP-001 is Approved;
2. the Project Owner records explicit implementation authorization;
3. the technical decisions in Stage 0 are Accepted;
4. the isolated non-financial environment is verified;
5. traceability from FRS-001 through Contracts, ADRs, FDN definitions, and VPL plans is current;
6. no unresolved conflict exists with the Vision or Constitution; and
7. no real financial credential, dependency, data source, or external path exists.

## 5. Stage 0 — Technical Decision Package

**Code permitted:** No.

Before the first source artifact, narrow ADRs SHALL decide:

| Decision | Required outcome |
|---|---|
| Runtime and toolchain | Supported, maintainable, secure Foundation execution platform and reproducible toolchain |
| Repository and dependency policy | Canonical structure, dependency admission, version locking, provenance, and update rules |
| Persistence realization | Technology and transaction model for authoritative state and append-only evidence |
| Communication realization | In-process and isolated-boundary transport consistent with ADR-F003 |
| Cryptographic and secret profile | Approved algorithms, key custody mechanism, secret provider, rotation, and test substitutes |
| Time and identity realization | Canonical identifiers, clock source, clock-quality handling, and expiry evaluation |
| Build and verification pipeline | Reproducible build, static checks, unit, Contract, security, fault, and VPL execution stages |

Each decision SHALL conform to STD-003 and shall not redefine an approved Contract or Specification.

**Exit gate:** all Stage 0 ADRs Accepted and the exact implementation baseline identified.

## 6. Stage 1 — Controlled Project Foundation

**Purpose:** Establish the project boundary without implementing Falcon behavior.

Deliverables:

- canonical repository structure;
- dependency lock and provenance record;
- isolated build and test environments;
- formatting, static analysis, security scanning, and verification commands;
- generated-artifact and secret-exclusion rules;
- version and artifact identity mechanism;
- traceability and evidence-output locations; and
- a guaranteed absence of financial dependencies and paths.

**Exit gate:** reproducible empty build, dependency and secret review, and constitutional scope check.

## 7. Stage 2 — Contracts, Schemas, and Evidence Primitives

**Purpose:** Make approved meaning executable before operational behavior.

Deliverables:

- executable representations of CON-001 through CON-011;
- FIL-001 schema validation and canonicalization;
- valid and rejection fixtures for every FIL message kind;
- stable identities, correlation, causation, time, version, and classification primitives;
- structured CON-008 evidence record;
- append-only correction semantics; and
- Contract conformance tests separating validation, authorization, execution, persistence, and outcome.

**Exit gate:** every Contract fixture passes or rejects exactly as approved; no authority is created by structural validity.

## 8. Stage 3 — Trusted Bootstrap and Configuration

**Purpose:** Establish Falcon identity and approved operating context.

Deliverables:

- CON-010 manifest verification;
- root-anchor test profile and protected-reference boundary;
- instance and Core workload identities;
- revocation and expiry evaluation;
- CON-009 security context;
- deterministic FDN-004 configuration resolution;
- immutable effective-configuration snapshot; and
- restricted startup on valid trust with fail-closed invalid variants.

**Verification gate:** VPL-001 passes.

## 9. Stage 4 — Authority, Lifecycle, State, and Evidence

**Purpose:** Establish accountable state change.

Deliverables:

- default-deny Authority Engine;
- CON-002 attributable decisions;
- authoritative Lifecycle model and CON-003 transitions;
- FDN-001 state ownership enforcement;
- current-state persistence and integrity-linked evidence journal;
- concurrency conflict and uncertain-write handling;
- immutable events for accepted facts; and
- restart reconciliation without fabricated state.

**Verification gates:** VPL-002 and VPL-003 pass.

## 10. Stage 5 — FIL, Service Bus, and Event System

**Purpose:** Establish governed cross-boundary communication.

Deliverables:

- only FDN-002 cataloged interactions;
- Service Bus admission, routing, flow control, expiry, bounded retry, and undeliverable handling;
- preservation of identity, correlation, causation, classification, and integrity;
- distinct command, query, response, event, and notice handling;
- unauthorized publication and subscription denial; and
- no undeclared cross-boundary side channel.

**Verification gate:** VPL-004 passes.

## 11. Stage 6 — Health, Self-Awareness, and Fitness

**Purpose:** Make operational knowledge and uncertainty explicit.

Deliverables:

- attributable health observations;
- freshness, completeness, provenance, contradiction, and clock-quality assessment;
- versioned Foundation Self Model;
- scoped Fitness to Operate;
- explicit `UNKNOWN` and degraded results;
- authority reduction on missing required evidence; and
- no consciousness or full Self-Awareness claim.

**Verification gate:** VPL-005 passes.

## 12. Stage 7 — Guardian and Safe State

**Purpose:** Prove protection can constrain the subject independently.

Deliverables:

- FDN-005 mandate evaluation;
- CON-011 durable restriction;
- immediate Authority Engine revocation;
- Lifecycle protective transition;
- enforcement at every applicable FDN-005 boundary;
- fail-closed behavior under unknown restriction state;
- minimum Safe-state allowlist; and
- restriction persistence across restart and bypass attempts.

**Verification gate:** VPL-006 passes.

## 13. Stage 8 — Recovery and Independent Release

**Purpose:** Restore trusted operation without self-certification.

Deliverables:

- versioned bounded recovery plan;
- containment, assessment, restoration, validation, reintroduction, and closure phases;
- authoritative-state and trust reconciliation;
- failed, partial, uncertain, bounded-retry, rollback, and escalation behavior;
- verifier independence from repair actor and subject;
- authorized Guardian release; and
- new identity or authority context where required.

**Verification gate:** VPL-007 passes.

## 14. Stage 9 — Full Reconstruction and Foundation Review

**Purpose:** Prove the complete Foundation claim.

Deliverables:

- sealed evidence packages from VPL-001 through VPL-007;
- independent VPL-008 reconstruction;
- mutation, deletion, insertion, reordering, duplication, and correction detection;
- full FRS-001 invariant traceability;
- constitutional compliance review under STD-004;
- risk evidence review under STD-005;
- decision-record review under STD-006;
- security and trust review under STD-007;
- verification review under STD-008;
- documentation-language review under STD-009;
- known limitations and residual risks; and
- Foundation release decision package.

**Exit gate:** VPL-001 through VPL-008 all `PASS`; every FRS-001 exit criterion satisfied.

## 15. Change and Stop Control

Work SHALL stop and escalate when:

- a requested implementation conflicts with higher authority;
- scope expands beyond FRS-001;
- a new enduring architecture choice lacks an ADR;
- a Contract or FDN definition is incomplete or contradicted;
- required evidence becomes unavailable or untrustworthy;
- an unresolved security issue can violate authority, integrity, confidentiality, or protection;
- a stage attempts to bypass its verification gate; or
- real financial consequence becomes technically reachable.

Changes to an Approved artifact follow its governing change and supersession rules. Implementation shall not silently reinterpret documentation.

## 16. Delivery Discipline

- Work proceeds one stage at a time.
- A later stage may be prepared but shall not depend on an unpassed earlier gate.
- Every increment remains buildable, testable, attributable, and reversible.
- Failed and abandoned attempts remain preserved.
- Implementation and verification evidence stay linked to the exact artifact.
- Convenience, schedule, or prior success cannot waive a gate.

## 17. Completion Rule

Implementation completion means only that FRS-001 has satisfied its Approved scope and exit criteria.

It does not authorize:

- connection to financial systems;
- exposure of capital;
- trading or investment behavior;
- third-party capabilities;
- autonomous self-evolution promotion;
- production deployment; or
- representation of Falcon as financially operational.

Each requires a later governed release.

## 18. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner | Pending | Pending | Pending |
