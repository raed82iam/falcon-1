# Falcon Foundation Complete Requirement and Dependency Coverage Study

**Version:** 0.1  
**Status:** COVERAGE STUDY / OWNER REVIEW REQUIRED FOR MATERIAL ROADMAP CHANGES  
**Date:** 2026-08-09  
**Branch:** `foundation-development`  
**Baseline at study start:** `c9fc3cca45b03cbadd9287dd829b8a5d0c4166ed`  
**Implementation Authority:** NOT GRANTED  
**Canonical Master-Plan Authority:** NOT CHANGED  

## 1. Purpose

This study performs the coverage and dependency work required by the Owner-approved `FOUNDATION_MASTER_STAGE_SEQUENCE_CORRECTION_PLAN.md` before a successor to `IMP-001 v1.2` may be prepared for canonical activation.

The study is intentionally inventory-first. It does not infer implementation need from document titles alone and does not treat registration, approval, implementation, closure, or activation as interchangeable states.

Its goals are to:

1. identify every currently known Falcon Specification subject relevant to Foundation planning;
2. distinguish current-effective Specifications from registered future subjects whose Specification bodies do not yet exist;
3. preserve accepted Stage/WP closures;
4. reconcile current-effective requirements against accepted implementation before proposing new work;
5. separate Foundation OS responsibilities from Falcon-wide financial, intelligence, risk, capital, and Application business responsibilities;
6. identify missing Foundation capability families not yet represented in the approved corrective Stage map;
7. establish prerequisite ordering for future Stage design; and
8. prevent any known actionable Foundation obligation from remaining unassigned at final Master Plan activation.

## 2. Governing distinctions

The study uses the following classifications.

### 2.1 Documentary state

- `CURRENT_EFFECTIVE_SPECIFICATION`
- `REGISTERED_PLANNED_SUBJECT_NO_BODY`
- `CURRENT_EFFECTIVE_CONTRACT`
- `CURRENT_EFFECTIVE_ADR_OR_GOVERNANCE`
- `HISTORICAL_OR_SUPERSEDED`

### 2.2 Coverage state

- `SATISFIED_BY_ACCEPTED_CLOSED_BASELINE`
- `PARTIALLY_SATISFIED_RECONCILIATION_REQUIRED`
- `FUTURE_FOUNDATION_IMPLEMENTATION_REQUIRED`
- `FUTURE_SPECIFICATION_DEFINITION_REQUIRED`
- `DEPENDENCY_ONLY`
- `APPLICATION_OR_DOMAIN_OWNED`
- `POST_FOUNDATION_FALCON_ROADMAP`
- `DOCUMENTARY_REMEDIATION_REQUIRED`
- `UNKNOWN_PENDING_EXACT_EVIDENCE`

### 2.3 Closure protection

A requirement is not a closure defect merely because it remains work somewhere else.

`CLOSURE_DEFECT` may be used only when evidence proves that the requirement was inside the exact authorized and accepted closure scope and was not actually satisfied.

No such defect has been established by this study for Stage 0 through Stage 5 or Stage 6 WP-01 through WP-04.

## 3. Specification inventory result

The active Specification Tree and SPEC-000 registry define the following domains:

- CAP — Capital Stewardship
- RSK — Risk and Protection
- DEC — Decision System
- AWR — Self-Awareness
- INT — Intelligence
- EVO — Maintenance and Evolution
- AUT — Autonomy and Control
- FIN — Financial Operations
- SYS — Operating System Foundation
- PLG — Replaceable Capability Ecosystem
- SEC — Trust and Security
- FCE — Canonical Representation
- PIPE — Build, Verification, and Promotion
- OPS — Reliability and Operations
- EXT — External Relationships
- APP — Applications and Experiences

The current registry contains both effective Specifications and registered planned subjects.

### 3.1 Registered planned subjects whose canonical bodies do not currently exist

Direct path verification on `foundation-development` found the following 38 registered subjects to be `NOT YET EFFECTIVE` and their canonical Markdown bodies absent:

- CAP-001 through CAP-005
- RSK-001 through RSK-004
- DEC-001 through DEC-005
- AWR-002 through AWR-005
- INT-001 through INT-003
- EVO-002 through EVO-005
- AUT-003
- FIN-001 through FIN-004
- PLG-002 through PLG-004
- SEC-003
- OPS-001 through OPS-002
- EXT-001 through EXT-002

These rows are valid registry visibility of future subjects. They are not Approved Specifications and do not supply detailed implementation requirements.

**Rule:** a future Stage depending materially on one of these subjects SHALL include a `SPECIFICATION_DEFINITION_REVIEW_ACTIVATION_GATE` before implementation of that subject's behavior.

No detailed requirement may be invented from the title or registry dependency row.

## 4. FRS-001 boundary result

FRS-001 is intentionally the first non-financial Foundation release.

Its required demonstration includes identity, authority, lifecycle, FIL, event truth, configuration, logging, security, Health, Foundation Self-Awareness, Fitness, Guardian restriction/Safe State, controlled Recovery, and complete reconstruction.

It explicitly excludes trading/order execution, broker/venue connectivity, live capital, portfolio management, market data, prediction/adaptive intelligence, autonomous strategy, autonomous self-evolution/production promotion, third-party plugin execution, distributed operation, high-availability claims, and performance/scale claims beyond test needs.

Therefore:

- planned CAP/FIN/INT requirements are not missing requirements of accepted current Foundation Stages;
- capital/business Risk and financial decision semantics must not be pulled into Foundation merely because they are registered in SPEC-000;
- Stage 7 through Stage 10 remain the correct family for completing the historical FRS-001 obligations after current Stage 6 Resource Governance;
- post-FRS Foundation platform capabilities require separate planning and authority.

## 5. Current-effective Foundation/Core coverage families

### 5.1 Stage 0 through Stage 3 preserved families

The accepted baseline already owns or consumes the enabling identities, time, canonical encoding, trust objects, authority instruments, configuration/security profiles, pipeline/bootstrap separation, and Contract execution prerequisites required by IMP-001 v1.2.

Current catalogs/specifications relevant to this preserved area include:

- IDN-001 v1.1
- TIM-001 v1.1
- CRY-001 v1.1
- FCE-001 v1.0
- SEC-002 v1.0
- PIPE-001 v1.1
- CON-012 through CON-021
- ADR-I005 through ADR-I008

These SHALL be reconciled against accepted Stage 0-3 evidence before any unresolved-text item is classified as new work.

### 5.2 Stage 4 preserved family

Primary current-effective subjects include:

- AUT-001 Authority Engine
- SYS-002 Lifecycle
- Trust/evidence/state ownership surfaces

Stage 4 remains accepted and closed. Unresolved text in an older/current Specification does not reopen Stage 4 unless the exact accepted closure evidence proves the requirement was in scope and unsatisfied.

### 5.3 Stage 5 preserved family

Primary current-effective subjects include:

- SYS-005 Service Bus
- SYS-009 FIL
- SYS-010 Event System
- SEC-001 / SEC-002 communication protection constraints
- APP-001 communication/lifecycle declaration boundaries
- ADR-I012 Plug-and-Play Application integration boundary

Stage 5 remains accepted and closed.

FCR-0004, FCR-0005, and FCR-0006 SHALL first reconcile against accepted Stage 5 behavior rather than create replacement communication subsystems.

### 5.4 Stage 6 Resource Governance

SYS-006 is current-effective and directly owns the current Stage 6 family.

WP-01 through WP-04 remain accepted and closed.

WP-05 through WP-10 remain separately gated.

FCR-0007 and FCR-0010 remain direct known Stage 6 inputs. Residual FCR-0009 consumes Stage 6 technical priority/pressure truth but is not fully owned by Stage 6.

## 6. Corrected FRS-001 completion sequence

### Stage 7 — Foundation Health, Self-Awareness and Technical Fitness

Primary current-effective inputs:

- AWR-001
- SYS-008
- CON-006
- SYS-006 resource truth as a consumed source
- OPS-004 evidence
- applicable SEC-002 trust semantics

Registered future subjects requiring reconciliation before authoring:

- AWR-002 Fitness to Operate
- AWR-003 Confidence and Uncertainty
- AWR-004 Temporal Awareness
- AWR-005 Drift and Blind-Spot Detection

These subjects overlap materially with current-effective AWR-001 v2.1. They SHALL NOT automatically become four new implementations. Stage 7 design must first determine whether each planned subject should become:

- a separate Specification;
- a narrower successor/addendum;
- a no-change/covered-by-AWR-001 decision; or
- a post-FRS capability.

### Stage 8 — Foundation Guardian, Protective Restriction and Platform Safe State

Primary current-effective inputs:

- AUT-002
- AUT-001
- SYS-002
- CON-011
- OPS-003
- ADR-F008

Registered planned inputs:

- AUT-003 Intervention, Revocation, and Recovery
- OPS-002 Fault Containment and Degradation

Stage 8 must begin with Guardian documentary reconciliation under the accepted Foundation/Application Guardian separation before behavior implementation.

AUT-003 and OPS-002 have no current Specification bodies. Their exact scope must be authored and activated before implementation if Stage 8 dependency analysis confirms they are required.

### Stage 9 — Controlled Recovery and Independent Release

Primary current-effective input:

- OPS-003

Consumes:

- Stage 4 authority/lifecycle
- Stage 6 recovery reserves/resource truth
- Stage 7 Health/Fitness
- Stage 8 Guardian/restriction
- SYS-011 persistent state
- OPS-004 evidence

Potential planned dependency:

- AUT-003, if its future approved scope contains recovery intervention/revocation behavior not already governed by AUT-001/AUT-002/OPS-003.

### Stage 10 — Full FRS-001 Reconstruction and Foundation Release Review

Primary scope:

- VPL-001 through VPL-008 reconstruction
- constitutional/security/authority review
- complete traceability
- residual-risk and limitation inventory
- FRS-001 release approval package

Potential documentary inputs:

- SEC-003 Auditability, if future specification review determines it is required for post-FRS auditability rather than already satisfied for FRS-001 by SEC-002, OPS-004, DEC-006, contracts, TRC, and verification evidence.

SEC-003 has no current Specification body and SHALL NOT be retroactively treated as an FRS-001 requirement merely because it is registered.

## 7. Post-FRS Foundation platform capability sequence

### Stage 11 — Transport QoS, Deadline Governance and Observability

Known inputs:

- residual FCR-0009
- Stage 5 transport/delivery surfaces
- Stage 6 priority/pressure/resource truth
- SYS-005 existing deadline/priority/flow-control requirements

Registered planned subject:

- OPS-001 Observability

OPS-001 has no current body. Stage 11 must author/review/activate the required observability Specification before implementing behavior not already covered by current effective Specifications.

Potential cross-cutting input:

- SEC-003 Auditability, subject to exact future specification ownership.

### Stage 12 — Governed External Access, Egress and Credential-Reference Security

Known inputs:

- FCR-0008 research egress
- FCR-0011 non-Live isolation/egress guard
- FCR-0013 operational provider egress
- FCR-0014 broker-execution egress boundary
- SEC-001 external-dependency trust obligations
- APP-001 declared permissions/dependencies
- SYS-004 dependency governance

Registered planned subject:

- EXT-001 External Dependency Governance

EXT-001 has no current body and requires Specification definition/review/activation before it can govern implementation.

EXT-002 Broker and Venue Relationship is a separate Falcon external/financial relationship subject depending on FIN-002 and EXT-001. Its business/relationship semantics are not Foundation egress semantics and belong to a post-Foundation Falcon financial/external roadmap unless a future approved Specification explicitly assigns a generic Foundation portion.

### Stage 13 — FSA / Owner Governance and Bounded Self-Maintenance & Evolution Control Plane

Known inputs:

- FCR-0012
- AWR-001
- EVO-001
- AUT-001
- GOV-AUT-001
- PIPE-001
- OPS-003
- Owner/governance command and delegation requirements

Registered planned subjects:

- EVO-002 Progressive Autonomy
- EVO-003 Safe Evolution Envelope
- EVO-004 Digital Twin and Simulation
- EVO-005 Shadow, Canary, Promotion, and Rollback

All four future EVO bodies are currently absent. Stage 13 design must determine their exact decomposition and activation before implementation.

Application MSA/LSA/CSA business evaluation remains Application-owned under AWR-006/AWR-007/AWR-008. Stage 13 must not move that judgment into Foundation.

### Stage 14 — Canonical Foundation Artifact Publication and Application Consumption

Known input:

- FCR-0016
- PIPE-001 artifact identity/provenance/promotion discipline
- SEC-002 trust objects
- FCE-001 canonical representation
- PLG-001 capability passport/admission
- ADR-I012 Application integration boundary

Registered planned subjects with likely dependency overlap:

- PLG-004 Supply Chain Trust
- part of PLG-003 Capability Update, Migration, and Removal

PLG-003 and PLG-004 have no current bodies. Stage 14 must not pre-emptively invent their semantics.

## 8. Additional known Foundation capability families discovered by full coverage

The Owner-approved correction plan explicitly allows additional Stages when complete coverage proves coherent known capability families. This study identifies three such families that should be added to the successor-plan proposal unless later evidence proves they are already completely owned elsewhere.

### Proposed Stage 15 — Application Runtime Hosting, Activation and Capability Isolation

**Reason this Stage is required:**

Stage 5 WP-09 is accepted only as an Application-neutral lifecycle decision/evidence eligibility boundary. Current Foundation state explicitly does not authorize deployment/runtime activation or complete Application lifecycle execution.

APP-001, however, defines Applications as independently installable, identifiable, validatable, registerable, admissible, activatable, observable, updateable, suspendable, isolatable, recoverable, replaceable and removable.

PLG-001 requires replaceable capabilities to be isolated, observable, interruptible, restrictable and removable.

Therefore there is a known gap between lifecycle eligibility truth and an actual generic Foundation runtime hosting/activation/isolation capability.

**Primary inputs:**

- APP-001
- ADR-I012
- SYS-003
- SYS-004
- SYS-006
- PLG-001
- Stage 5 lifecycle/communication acceptance
- Stage 6 resource governance
- Stage 14 canonical artifact consumption

**Registered planned subjects:**

- PLG-002 Falcon Cells and Capability Isolation
- residual PLG-003 update/migration/removal behavior not already satisfied by Stage 5

**Mandatory gate:**

Existing-capability reconciliation must prove exactly which lifecycle/update/removal semantics are already satisfied by Stage 5 before any Stage 15 runtime mechanism is designed.

No Application business logic moves into Foundation.

### Proposed Stage 16 — Provider-Neutral Deployment Environment Expansion and Platform Portability

**Reason this Stage is required:**

PIPE-001 explicitly establishes Windows as the first governed environment and identifies future Oracle Cloud execution as separately admitted and activated work requiring exact environment, network, identity, custody, storage, time, evidence, failure, recovery and exit verification.

IMP-001 Stage 0A also recognizes candidate Windows and Linux environments, while evidence from one environment does not imply another environment's validity.

This is a known Foundation portability/deployment capability, not an unknown future feature.

**Primary scope:**

- additional admitted Environment Profiles
- Linux environment qualification where still required
- OCI Environment Profile
- provider-neutral deployment semantics
- environment-specific network/storage/identity/custody evidence
- portable verification/reconstruction
- no cloud provider becoming governance authority
- exact activation per environment

**Non-scope unless later Specifications create requirements:**

- distributed operation as a blanket claim
- high availability as a blanket claim
- automatic production authority
- financial connectivity

Those FRS-001 exclusions are not silently converted into requirements merely by being excluded.

### Proposed Stage 17 — Foundation Operationalization and Non-Financial Production-Readiness Gate

**Reason this Stage is required:**

IMP-001 explicitly states that Operational Authority is absent from FRS-001. FRS-001 completion is also explicitly not production or financial readiness.

After the post-FRS Foundation capability families are implemented, Falcon still requires a governed integration/reconstruction/authority gate before the Foundation can truthfully be represented as an operational platform for later Applications.

**Primary scope:**

- integrated verification of all accepted post-FRS Foundation platform capabilities
- production-environment identity and trust prerequisites
- operational logging/audit/recovery readiness
- Application hosting readiness
- external-access enforcement readiness where applicable
- security/threat/asset review appropriate to the declared environment
- dependency exit/replacement evidence
- platform portability evidence for admitted environments
- exact operational Authority Instrument and acceptance boundaries
- explicit known-limitations and residual-risk inventory

**Hard boundary:**

Stage 17 does not grant trading, broker, capital, market-data, investment or other financial authority. Those remain separate Falcon financial/domain releases.

## 9. Falcon-wide subjects that SHALL NOT be forced into Foundation implementation Stages

The following registered domains are currently Falcon-wide or financial/domain authorities and shall not be placed into Foundation implementation merely because they are visible in SPEC-000:

### Capital Stewardship

- CAP-001 through CAP-005

### Financial Operations

- FIN-001 through FIN-004

### Intelligence

- INT-001 through INT-003

### Risk / capital-domain specifications

- RSK-001 through RSK-004
- RSK-005 remains current-effective as a Falcon-wide capital safety specification, but FRS-001 intentionally excludes live capital.

### Decision System

- DEC-001 through DEC-005
- DEC-006 is current-effective and cross-cutting, but does not make Foundation the owner of financial/business decisions.

These subjects require a separate post-Foundation Falcon roadmap and approved Specification bodies before implementation. Foundation may supply generic contracts/services they consume, but it shall not acquire their business authority.

## 10. Current-effective unresolved-matter reconciliation register

The following current-effective documents contain explicit unresolved matters that require evidence reconciliation before being classified as new work:

- SYS-001: essential/nonessential Core functions; Core admission ratifying authority
- SYS-002: consequence-based transition timeouts; dependency-cycle resolution
- SYS-005: communication consequence classes; protective delivery semantics
- SYS-007: configuration consequence classes; protective propagation delay
- SYS-008: health consequence classes; freshness by Core component
- SYS-009: canonical time/clock-quality metadata
- SYS-010: authoritative event catalog; event completeness classes
- SYS-011: data-class catalog/recovery objectives; jurisdictional retention/deletion
- SEC-001: threat model/protected-asset inventory; jurisdictional security/privacy obligations
- OPS-004: regulatory retention; audit-critical event catalog
- AUT-001: non-waivable constraints; authorization latency by consequence; canonical jurisdiction/decision/consequence catalogs; CON-002 amendment question
- AUT-002: protective mandate matrix; release authority by consequence
- OPS-003: recovery objectives; independent validation authority matrix
- EVO-001: M2/E2/E3 consequence thresholds; constitutionally reserved components
- PLG-001: capability consequence classes; constitutionally non-pluggable Core responsibilities
- DEC-006: materiality thresholds; jurisdictional retention
- RSK-005: capital risk taxonomy/upper-limit catalog; jurisdiction/institution protection

Each item SHALL receive one of:

- `RESOLVED_BY_ACCEPTED_ADR_OR_CATALOG`
- `RESOLVED_BY_ACCEPTED_IMPLEMENTATION_AND_SHOULD_UPDATE_SPEC_TEXT`
- `STILL_OPEN_AND_ASSIGNED_TO_FUTURE_STAGE`
- `POST_FOUNDATION_DOMAIN_ITEM`
- `REQUIRES_OWNER_OR_GOVERNANCE_DECISION`

No unresolved line is automatically a new Work Package.

## 11. Documentary consistency findings

### 11.1 AWR-001 internal state inconsistency

AWR-001 v2.1 top metadata identifies it as Approved and Active under GOV-092/GOV-093/GOV-094, while the document's final Approval section still contains historical candidate language showing `Pending` and stating that it is a proposed successor only.

This is a documentary internal-state inconsistency requiring a governed documentary correction. It does not by itself invalidate the activated AWR-001 meaning because the controlling metadata/registry/activation records identify the active version.

### 11.2 Master sequencing lag

IMP-001 v1.2 still preserves the historical Stage 4-9 purposes while current Stage 6 Resource Governance was introduced later and is partially implemented/closed.

This is the principal master-sequence reconciliation being corrected by the Owner-approved planning baseline.

### 11.3 Migration-map sequencing lag

GOV-002 records a controlled documentation migration order that anticipated CAP/RSK/DEC/AUT protective Specification definition before broad OS Foundation migration. Many of those future Specification bodies remain absent.

This is not treated as a retroactive implementation closure defect because GOV-002 is a documentary migration map and FRS-001 explicitly authorizes a bounded non-financial Foundation release. The relationship must nevertheless be reconciled in the formal successor package so documentary sequencing and implementation sequencing are no longer ambiguous.

## 12. Proposed corrected Foundation Stage map after coverage pass 1

- Stage 0A-5 — preserve accepted closure
- Stage 6 — Resource Governance, preserve WP-01 through WP-04 closure and separately gate WP-05 through WP-10
- Stage 7 — Foundation Health, Self-Awareness and Technical Fitness
- Stage 8 — Foundation Guardian, Protective Restriction and Platform Safe State
- Stage 9 — Controlled Recovery and Independent Release
- Stage 10 — Full FRS-001 Reconstruction and Foundation Release Review
- Stage 11 — Transport QoS, Deadline Governance and Observability
- Stage 12 — Governed External Access, Egress and Credential-Reference Security
- Stage 13 — FSA / Owner Governance and Bounded Self-Maintenance & Evolution Control Plane
- Stage 14 — Canonical Foundation Artifact Publication and Application Consumption
- **Stage 15 — Application Runtime Hosting, Activation and Capability Isolation — PROPOSED BY THIS STUDY**
- **Stage 16 — Provider-Neutral Deployment Environment Expansion and Platform Portability — PROPOSED BY THIS STUDY**
- **Stage 17 — Foundation Operationalization and Non-Financial Production-Readiness Gate — PROPOSED BY THIS STUDY**

Stages 15-17 are findings of the coverage study, not yet Owner-approved modifications to the corrective planning baseline.

## 13. Required next coverage work before final successor drafting

Before `IMP-001` successor drafting may begin, this study still requires:

1. exact TRC-001 current-path/content reconciliation;
2. current ROADMAP/high-level plan reconciliation;
3. VPL-000 through VPL-008 mapping against corrected Stage numbers;
4. accepted Stage 0-5 evidence check for every current-effective `Unresolved Matters` item that may already be resolved;
5. exact BLD-001/ENV-001 path and active-state reconciliation from canonical records rather than guessed paths;
6. contract-by-contract check for CON-002, CON-006, CON-011, CON-023 and any amendment need;
7. Guardian documentary successor/AMD impact inventory;
8. full FCR target synchronization after Owner accepts any material Stage 15-17 additions;
9. independent Red-Team of the complete matrix and dependency order.

## 14. Pass-1 decision markers

`ACCEPTED_CLOSURE_REOPEN_REQUIRED = NO`

`KNOWN_PLANNED_SPEC_SUBJECTS = 38`

`PLANNED_SPEC_BODIES_CURRENTLY_PRESENT = 0`

`SPEC_AUTHORING_GATE_REQUIRED_FOR_FUTURE_REGISTERED_SUBJECTS = YES`

`FRS001_REMAINS_NON_FINANCIAL = YES`

`FINANCIAL_DOMAIN_SPECS_FORCED_INTO_FOUNDATION = NO`

`STAGE_15_APPLICATION_RUNTIME_HOSTING_PROPOSED = YES`

`STAGE_16_ENVIRONMENT_PORTABILITY_PROPOSED = YES`

`STAGE_17_FOUNDATION_OPERATIONALIZATION_PROPOSED = YES`

`IMP001_SUCCESSOR_READY = NO`

`WP05_IMPLEMENTATION_AUTHORITY_CREATED = NO`
