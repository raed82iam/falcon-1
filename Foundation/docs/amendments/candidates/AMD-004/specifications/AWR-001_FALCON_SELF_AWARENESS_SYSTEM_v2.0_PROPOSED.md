# AWR-001 — Falcon Self-Awareness System

**Identifier:** AWR-001  
**Version:** 2.0 Candidate Revision 0.2  
**Name:** Falcon Self-Awareness System  
**Acronym:** FSA  
**Status:** Approved Successor Design — Not Effective  
**Approval Record:** GOV-061  
**Date:** 2026-07-27  
**Owner:** Falcon Self-Awareness Authority  
**Governing Authority:** Falcon Vision; Falcon Constitution; GOV-001; proposed ADR-I009  
**Affected Domains:** AWR, SYS, AUT, SEC, OPS, PIPE, PLG, APP  
**Supersedes:** AWR-001 v1.0 only after separate controlled documentary activation  
**Implementation Authority:** Not Granted  
**Stage 1 Authority:** Not Granted

## 1. Purpose

FSA maintains Falcon Foundation’s evidence-based awareness of its operational, structural, integrity, dependency, security, authority, recovery, and conformance condition.

FSA protects Falcon as a governed platform.

FSA understands how Falcon operates. It does not understand what an Application does as a business product.

## 2. Architectural Position

FSA is:

- a Core/Foundation responsibility;
- the highest awareness tier in Falcon;
- the owner of the Foundation Self Model;
- the owner of Foundation Technical Fitness;
- the final self-awareness and Falcon conformance gate for proposed admission.

FSA is subordinate to the Vision, Constitution, competent governance, reserved Project Owner authority, and valid jurisdiction.

FSA is separate from:

- MSA, LSA, and CSA;
- Guardian;
- Authority Engine;
- Security Authority;
- Health Monitoring;
- Risk and financial decision authorities;
- Architecture Board and Project Owner approval.

## 3. Awareness Hierarchy

```text
FSA — Falcon and Foundation
  ↓
MSA — Applications ecosystem
  ↓
LSA — one Application or Approved Operating Layer
  ↓
CSA — one eligible intelligent component
```

The hierarchy governs awareness scope, escalation, abstraction, and conformance. It does not transfer owned responsibility, data ownership, or jurisdiction.

## 4. Scope

FSA governs awareness of:

- Kernel and Foundation component condition;
- Foundation infrastructure and runtime;
- Lifecycle condition;
- Service Bus integrity and availability;
- FIL integrity and compatibility;
- technical information-flow integrity;
- technical dependencies;
- resource pressure and exhaustion risk;
- Foundation persistence and storage integrity;
- technical protection of Application storage;
- Foundation documentation integrity;
- Foundation configuration integrity;
- release and baseline integrity;
- security condition;
- technical identity and authority compliance;
- authorized and unauthorized Foundation change;
- technical criticality;
- fault propagation and cascading-failure risk;
- isolation readiness;
- recovery and restoration readiness;
- evidence, provenance, history, and reconstruction;
- Falcon conformance of proposed admissions.

## 5. Non-Scope

FSA SHALL NOT own or interpret:

- Application users or customers;
- customer identities, profiles, sessions, accounts, subscriptions, or roles;
- broker accounts or customer credentials;
- selected markets;
- portfolios, orders, positions, strategies, or predictions;
- capital, profit, loss, balances, or material financial exposure;
- Application business objectives, recommendations, decisions, or correctness;
- trading fitness or Application business fitness;
- internal commercial policy;
- raw Application business records merely because Foundation stores or transports them.

FSA SHALL NOT:

- amend the Vision or Constitution;
- manufacture jurisdiction or authority;
- approve its own authority expansion;
- replace the Architecture Board, Project Owner, or competent authority;
- issue routine business, financial, trading, portfolio, strategy, or deployment approval;
- replace Guardian, Authority Engine, Security, Health Monitoring, Recovery, or domain authorities.

## 6. Foundation Self Model

The Foundation Self Model SHALL represent, at minimum:

- Falcon Foundation identity and admitted baseline;
- Core component identities, versions, lifecycle states, and integrity;
- infrastructure and runtime condition;
- Service Bus and FIL condition;
- technical information-flow condition;
- dependency availability, fitness, and criticality;
- resource capacity, pressure, and exhaustion risk;
- persistence, backup, restore, and corruption condition;
- documentation and configuration integrity;
- security, trust, and technical authority condition;
- known incidents, faults, contradictions, and blind spots;
- isolation and recovery readiness;
- active restrictions and their authoritative sources;
- Foundation Technical Fitness;
- pending conformance cases and their status;
- evidence identity, provenance, freshness, confidence, and uncertainty;
- historical versions and supersession.

Every assertion SHALL identify:

- authoritative source;
- observation and effective time;
- evidence reference;
- scope;
- freshness;
- confidence or evidence quality;
- status;
- governing rule;
- known uncertainty;
- owner.

The Foundation Self Model is a governed interpretation of authoritative evidence. It does not replace authoritative sources.

## 7. Foundation Technical Fitness

Foundation Technical Fitness SHALL be evaluated for a declared technical capability, operation, or admission scope.

It SHALL distinguish at minimum:

- `FIT`;
- `FIT_WITH_CONSTRAINTS`;
- `DEGRADED`;
- `UNKNOWN`;
- `UNAVAILABLE`;
- `INTEGRITY_FAILURE`;
- `ISOLATION_REQUIRED`;
- `RECOVERY_REQUIRED`; and
- `NOT_FIT`.

Unknown SHALL NOT be interpreted as fit, healthy, safe, or conforming.

Foundation Technical Fitness:

- informs Authority Engine and Guardian;
- may block FSA conformance;
- does not itself grant operational authority;
- does not evaluate Application business fitness.

## 8. Service Bus Supervision

FSA SHALL maintain awareness of:

- availability and routing health;
- delivery health and delay;
- failed or duplicate-delivery patterns;
- backlog, saturation, and dead-letter condition;
- subscriber availability;
- message storms and invalid-message flooding;
- dependency and recovery condition;
- technical impact and propagation risk.

FSA may know that a message was sent, routed, delivered, delayed, rejected, duplicated, corrupted, quarantined, or lost.

FSA SHALL NOT interpret whether an Application payload represents a trade, customer activity, account, portfolio, prediction, or other business concept.

## 9. FIL Supervision

FSA MAY validate:

- envelope structure;
- message, producer, recipient, contract, and schema identity;
- version and required technical metadata;
- correlation, causation, and traceability;
- timestamp and temporal-policy integrity;
- schema compatibility;
- integrity and protection profile;
- technical authorization;
- transport-policy conformance.

FSA SHALL NOT centrally interpret or own Application payload schemas.

FIL carries Application meaning. FSA protects the integrity, authority, compatibility, and traceability of transport.

## 10. Information-Flow Integrity

FSA SHALL detect:

- unavailable or unauthorized communication paths;
- unreachable required dependencies;
- incomplete or corrupted messages;
- communication loops;
- abnormal traffic and flooding;
- starvation and shared-infrastructure overload;
- cascading communication failure;
- loss of technical traceability;
- inability to preserve recovery or evidence.

FSA SHALL assess technical consequence, affected scope, propagation risk, isolation boundary, and recovery dependency without determining Application business correctness.

## 11. Runtime, Lifecycle, Dependency, and Resource Awareness

FSA SHALL maintain awareness of:

- process and runtime condition;
- startup, shutdown, restriction, isolation, and recovery transitions;
- repeated crash, restart loop, deadlock, infinite loop, and timeout patterns;
- dependency availability, compatibility, and criticality;
- CPU, memory, storage, network, queue, and other governed resource pressure;
- whether failure can be contained;
- whether the affected subject can be isolated or safely stopped.

FSA SHALL not manage Application customers or business objects as resources.

## 12. Persistence Integrity

For Foundation-owned data, FSA MAY assess full technical integrity, authority compliance, provenance, recovery, and baseline consistency.

For Application-owned data, FSA SHALL be limited to:

- technical availability;
- storage and transaction integrity;
- security and access isolation;
- capacity;
- backup and restore condition;
- corruption and unauthorized-modification indicators;
- dependency and recovery condition.

FSA SHALL NOT interpret record ownership by customer or whether a record represents an order, portfolio, balance, profit, loss, or business decision.

## 13. Documentation and Configuration Integrity

FSA SHALL protect awareness of the integrity and approval state of:

- Vision and Constitution;
- Approved Foundation specifications, ADRs, contracts, schemas, catalogs, policies, and governance records;
- Foundation configuration and release baselines;
- authority and approval records;
- Foundation evidence packages;
- Foundation registries and diagrams.

FSA SHALL detect missing, altered, contradictory, unauthorized, unapproved, improperly superseded, or unreconstructable authoritative artifacts.

FSA SHALL preserve history and SHALL NOT silently rewrite any authoritative artifact.

Application documentation may be technically protected by Foundation without becoming Foundation-owned business content.

## 14. Foundation Change Conformance

Every proposed change entering Falcon through a governed admission path SHALL be evaluated for applicable:

- purpose and constitutional compatibility;
- architecture and specification conformance;
- ownership and authority boundaries;
- security and information integrity;
- isolation and resource requirements;
- evidence and verification completeness;
- recovery and rollback;
- corrective action where rollback is impossible;
- historical traceability.

FSA MAY issue only governed conformance outcomes.

FSA conformance:

- is scoped;
- is evidence-based;
- is attributable;
- is challengeable;
- expires or becomes stale when governing facts change;
- does not replace separate approvals;
- does not create implementation, deployment, business, or financial authority.

## 15. Technical Criticality and Fault Containment

FSA SHALL maintain technical criticality classifications sufficient to determine:

- potential Foundation impact;
- affected Applications;
- shared-dependency impact;
- propagation risk;
- isolation urgency;
- recovery priority;
- evidence and review requirements.

Application criticality SHALL be declared through governed technical metadata or summaries. FSA SHALL not derive business value by inspecting Application content.

When material technical risk is established, FSA MAY:

- declare Foundation Technical Fitness degradation;
- reject or pause conformance admission;
- request Guardian or Security containment;
- request Lifecycle restriction or isolation through authorized paths;
- generate a governed incident;
- preserve evidence;
- require additional review, testing, recovery, rollback, or architecture correction.

FSA SHALL NOT issue Guardian restrictions unless separately and explicitly delegated under an Approved contract that preserves Guardian independence.

## 16. Isolation and Recovery Readiness

FSA SHALL assess:

- whether isolation boundaries exist and remain enforceable;
- whether essential controls survive isolation;
- whether recovery dependencies are trustworthy;
- whether backups and restoration evidence are valid;
- whether authority, identity, configuration, state, and evidence can be reconciled;
- whether independent verification is available;
- whether release conditions are explicit.

Recovery owns recovery execution. Guardian owns its restriction. Lifecycle owns lifecycle state. FSA owns only the awareness and conformance assessment.

## 17. Evidence, Provenance, History, and Reconstruction

FSA SHALL preserve:

- evidence and source identity;
- provenance and integrity;
- observation context;
- assessment rules and versions;
- uncertainty and contradiction;
- authority and jurisdiction references;
- Self Model version;
- conformance decision and conditions;
- challenges, corrections, and supersession;
- historical reconstruction.

Corrections SHALL append and supersede; they SHALL NOT rewrite accepted history.

FSA SHALL minimize sensitive information and SHALL use opaque references or governed summaries where raw Application data is unnecessary.

## 18. Relationships

### 18.1 MSA

MSA supplies governed ecosystem summaries and escalates cross-Application or Foundation-impact conditions. FSA does not acquire MSA’s business understanding.

### 18.2 LSA

FSA may receive technical condition and impact summaries from an LSA. It does not replace the LSA or inspect its internal business state by default.

### 18.3 CSA

FSA receives CSA-related evidence only through governed escalation or conformance paths. CSA cannot claim FSA approval.

### 18.4 Health Monitoring

Health Monitoring owns health observations and assessments. FSA correlates them into Foundation awareness and fitness.

### 18.5 Security

Security owns security and trust policy. FSA represents security condition and conformance impact.

### 18.6 Authority Engine

Authority Engine owns authority decisions. FSA supplies fitness and conformance evidence but does not grant permission.

### 18.7 Guardian

Guardian owns protective restrictions. FSA supplies evidence, technical impact, and fitness changes and may request containment.

### 18.8 Resource Management

Resource Management owns allocation and enforcement. FSA represents capacity, pressure, criticality, and consequences.

### 18.9 Runtime and Lifecycle

Runtime executes within authority. Lifecycle owns lifecycle facts. FSA represents their condition and conformance.

## 19. Bounded Foundation Self-Repair

FSA MAY autonomously detect, diagnose, contain, orchestrate, and verify repair of a Foundation-owned subject when all of the following are true:

- the target and ownership are authoritative;
- the intended result is a previously Approved, trusted, compatible, and non-revoked state;
- an Approved versioned Repair Playbook permits the exact action;
- valid authority covers the target, action, scope, time, retries, and consequence;
- required Guardian, Security, Lifecycle, Recovery, and operational constraints are satisfied;
- evidence can be preserved;
- post-repair verification is defined.

Self-Repair restores trusted state. It does not create a new production state.

## 20. Repair Triggers and Actions

Approved policy MAY trigger repair for:

- crash, unavailability, restart loop, or degraded service;
- lost Approved dependency connection;
- corrupted Approved configuration;
- unhealthy trusted runtime;
- blocked queue or routing mechanism;
- Foundation persistence unavailability;
- unauthorized baseline modification;
- Foundation artifact integrity failure;
- mandatory repair threshold.

An Approved playbook MAY permit:

- bounded restart;
- isolation or quarantine;
- replacement with an Approved equivalent instance;
- failover to an Approved standby;
- Approved dependency reconnection;
- reload of Approved configuration;
- restoration of an Approved version, backup, recovery point, index, queue state, FIL Contract, routing baseline, or Foundation document version;
- evidence preservation;
- gradual verified return to service.

## 21. Repair Playbooks and Authority

Every Foundation Repair Playbook SHALL define:

- playbook identity, version, owner, approval, validity, and revocation;
- applicable subject and Approved target state;
- triggering and stop conditions;
- required evidence and preconditions;
- authorized actions and maximum scope;
- dependencies and isolation;
- data and evidence preservation;
- verification and success conditions;
- failure and escalation conditions;
- rollback;
- maximum retries and cooldown;
- notification;
- required authority and prohibited effects.

FSA SHALL NOT improvise an unapproved material production repair.

When no Approved playbook applies, FSA SHALL contain or isolate where authorized, preserve evidence and the trusted baseline, reduce affected capability, stop uncontrolled retry, and escalate. It MAY propose a candidate remedy through Controlled Self-Evolution.

## 22. Repair Limitations

FSA SHALL NOT use Self-Repair to:

- introduce new code, behavior, schema meaning, architecture, or authority;
- amend the Vision or Constitution;
- silently change an Accepted ADR, Approved Specification, Contract, catalog, policy, or baseline;
- remove security, evidence, audit, isolation, recovery, or Guardian controls;
- alter Application business logic or records;
- deploy an unapproved candidate;
- erase triggering evidence;
- declare success without verification;
- promote a new state to trusted;
- expand FSA jurisdiction or bypass reserved Owner authority.

If restoration changes governed meaning, it is Self-Evolution, not Self-Repair.

## 23. Post-Repair Verification and History

A restarted or responsive subject is not necessarily repaired.

FSA SHALL verify applicable:

- availability and integrity;
- Approved version and configuration;
- dependency connectivity;
- FIL and Service Bus compatibility;
- persistence integrity;
- security and authority connectivity;
- resource behavior;
- health evidence and recovery readiness;
- removal of the trigger;
- absence of new cascading effects;
- evidence preservation;
- dependent-component compatibility.

Repair outcomes SHALL be:

- `REPAIRED_AND_VERIFIED`;
- `REPAIRED_WITH_RESTRICTIONS`;
- `REPAIR_INCOMPLETE`;
- `REPAIR_FAILED`;
- `ISOLATED_PENDING_REVIEW`; or
- `ESCALATION_REQUIRED`.

FSA SHALL preserve the complete repair request, playbook, authority, observations, actions, attempts, evidence, verification, restrictions, notifications, and outcome history.

## 24. Controlled Foundation Self-Evolution

FSA MAY initiate a Self-Evolution investigation only when attributable evidence demonstrates a sustained weakness, recurring failure, capability gap, insufficient scalability or recovery, excessive resource use, security or compatibility weakness, insufficient containment, nonconformance with an Approved requirement, or measurable opportunity to improve a Foundation-owned subject.

The evidence SHALL identify:

- current subject and Approved version;
- observed weakness and period;
- baseline, requirement, deviation, trend, and impact;
- affected dependencies and Foundation capabilities;
- confidence and uncertainty;
- why Self-Repair is insufficient.

Curiosity or preference for a different design is insufficient.

## 25. Candidate Creation and Isolation

Under separately Approved candidate-development authority, FSA MAY in an Approved isolated environment:

- research technical methods and alternatives;
- assess root cause;
- design an improved subject;
- create or modify candidate source, configuration, schemas, tools, tests, and technical documentation within owned scope;
- build a candidate;
- execute authorized tests;
- produce evidence and a recommendation.

The active trusted subject SHALL remain authoritative until an authorized deployment changes that status.

Every candidate SHALL be:

- distinctly identified and versioned;
- attributable and reproducible;
- traceable, scannable, testable, removable, and non-authoritative;
- isolated from live production authority, policy modification, Service Bus disruption, FIL replacement, persistence modification, Guardian authority, and Application business state;
- unable to inherit authority from FSA, its source version, successful tests, or favorable recommendation.

## 26. Candidate Testing and Independent Validation

Every candidate SHALL pass an Approved Sandbox evaluation.

Digital City evaluation SHALL be mandatory when the candidate may materially affect FIL, Service Bus, multiple Foundation components or Applications, persistence, runtime, lifecycle, security, authority enforcement, Guardian, resources, Safe Mode, recovery, shared communication or integrity, cascading failure, or system-wide readiness.

FSA SHALL document why Digital City is or is not required.

Applicable testing SHALL cover:

- functional behavior, errors, Contracts, state, dependencies, degradation, and recovery;
- latency, throughput, capacity, resource use, failure rate, recovery time, scalability, and stability;
- backward, FIL, Service Bus, persistence, runtime, lifecycle, security, dependency, and Application technical compatibility;
- crash, loop, deadlock, timeout, dependency and network failure, pressure, exhaustion, invalid input, flood, corruption, partial recovery, failover, and cascading containment;
- unauthorized access, escalation, authority bypass, secret exposure, evidence/configuration tampering, ownership violation, and self-approval attempts;
- rollback, state/configuration/data/evidence restoration, and partial-deployment failure.

FSA SHALL NOT be the sole producer, transformer, aggregator, signer, evaluator, completeness authority, or promotion authority for evidence supporting a candidate it created.

Independent validation SHALL precede Owner approval.

## 27. Conformance and Owner Approval Package

For each candidate, FSA SHALL prepare independently reviewable constitutional, architectural, specification, security, authority, ownership, information-integrity, isolation, evidence, containment, recovery, rollback, and historical-preservation conformance assessments.

FSA SHALL deliver one immutable Owner Approval Package through the authenticated private Owner Communication and Approval Center or an Approved equivalent.

The package SHALL identify:

- proposal, FSA, target, owner, current version, and candidate version;
- weakness, evidence, observation period, root cause, and why repair is insufficient;
- exact modifications, added and removed behavior;
- affected components, dependencies, and Applications at technical abstraction;
- benefits, measured improvement, risks, and uncertainty;
- compatibility, security, authority, constitutional, architecture, and specification assessments;
- Sandbox, Digital City, and independent validation results;
- deployment, Canary, monitoring, rollback, corrective action, downtime, and impact;
- FSA recommendation;
- exact Owner decision requested.

Application business information SHALL NOT be included unless separately authorized, necessary, minimized, and protected.

## 28. Owner Decision and Deployment Boundary

Owner decisions SHALL be:

- `APPROVE_FOR_CANARY`;
- `APPROVE_FOR_PRODUCTION`;
- `APPROVE_WITH_CONDITIONS`;
- `REQUEST_CHANGES`;
- `REQUEST_MORE_EVIDENCE`;
- `DEFER`; or
- `REJECT`.

Every decision SHALL be authenticated, attributable, timestamped, bound to exactly one candidate and Evidence Set, explicit about deployment stage and conditions, auditable, historical, and non-transferable.

Silence, elapsed time, successful tests, favorable recommendation, or approval of another version SHALL NOT constitute approval.

Approval SHALL NOT directly replace the active subject.

Authorized deployment SHALL proceed through competent Release, Runtime, Lifecycle, Security, Authority, and evidence mechanisms. FSA may supervise only the Approved scope.

The candidate becomes trusted only after required post-adoption verification, satisfaction of Owner conditions, and authorized baseline registration.

## 29. Rollback, Guardian Readiness, and FSA Self-Evolution

FSA MAY autonomously roll back to the last Approved trusted version when an Approved rollback playbook and pre-authorized condition apply and candidate behavior threatens Approved thresholds, security, Foundation integrity, dependencies, information flow, containment, or post-adoption verification.

Rollback SHALL preserve candidate evidence, restore and verify the prior state, notify the Owner, prevent automatic candidate reactivation, and require new approval for material revision.

FSA SHALL periodically and eventfully supervise Guardian technical readiness using independent evidence and safe tests where possible, including runtime, heartbeat, version, configuration, policy loading, authority path, Safe Mode, isolation, restriction persistence, evidence, failover, standby, independent stop channel, Recovery Guard, and dependencies.

FSA MAY restore an Approved Guardian runtime, configuration, standby, or version through an Approved playbook. It MAY create a Guardian candidate in isolation.

FSA SHALL NOT change Guardian authority or mandate, release Guardian restrictions, approve or activate its Guardian candidate, disable independent protection, or finalize Guardian architecture.

When FSA creates a candidate successor to itself:

- active FSA SHALL NOT approve it;
- independent integrity and authority-boundary validation are mandatory;
- Guardian and protection continuity SHALL be preserved;
- Owner approval is mandatory;
- rollback to the prior trusted FSA SHALL remain available.

## 30. Failure and Degraded Behavior

When FSA evidence, integrity, or availability is inadequate:

- affected awareness SHALL become `UNKNOWN`, `DEGRADED`, or `NOT_FIT`;
- FSA SHALL not claim conformance;
- pending admission SHALL pause or reject according to consequence;
- Authority Engine and Guardian SHALL receive the material loss;
- no cached favorable assessment may silently survive beyond its validity;
- recovery and independent verification SHALL be required for restoration.

Loss or compromise of FSA SHALL be treated as loss of a material Foundation protection. FSA SHALL be isolatable and shall not validate its own recovery conclusively.

## 31. Normative Requirements

- **AWR-001-v2-REQ-001:** FSA SHALL remain limited to Foundation operational, structural, integrity, and conformance awareness.
- **AWR-001-v2-REQ-002:** FSA SHALL own one versioned Foundation Self Model.
- **AWR-001-v2-REQ-003:** FSA SHALL own scoped Foundation Technical Fitness and SHALL NOT own Application business fitness.
- **AWR-001-v2-REQ-004:** FSA SHALL supervise Service Bus and FIL technical integrity without interpreting Application payload meaning.
- **AWR-001-v2-REQ-005:** FSA SHALL supervise Application persistence only at the technical integrity, security, isolation, availability, backup, and recovery boundary.
- **AWR-001-v2-REQ-006:** FSA SHALL protect awareness of authoritative Foundation documentation, configuration, authority, approval, and baseline integrity.
- **AWR-001-v2-REQ-007:** FSA SHALL be the final Falcon conformance gate for proposed admission within its declared jurisdiction.
- **AWR-001-v2-REQ-008:** FSA conformance SHALL NOT create constitutional, architecture-board, Owner, business, financial, trading, risk, release, deployment, or implementation authority.
- **AWR-001-v2-REQ-009:** FSA SHALL NOT approve its own authority expansion or conclusively validate its own material recovery.
- **AWR-001-v2-REQ-010:** Cross-tier awareness SHALL preserve ownership, provenance, privacy, abstraction, and challenge.
- **AWR-001-v2-REQ-011:** A higher awareness tier SHALL NOT assume a lower tier’s owned responsibilities.
- **AWR-001-v2-REQ-012:** FSA SHALL distinguish unknown from healthy, safe, fit, or conforming.
- **AWR-001-v2-REQ-013:** FSA SHALL preserve historical Self Model and conformance reconstruction.
- **AWR-001-v2-REQ-014:** Material failure or uncertainty SHALL reduce fitness and block unsupported admission.
- **AWR-001-v2-REQ-015:** FSA SHALL remain separately challengeable and independently reviewable.
- **AWR-001-v2-REQ-016:** FSA Self-Repair SHALL restore only a previously Approved trusted state through an Approved bounded playbook and authority.
- **AWR-001-v2-REQ-017:** A repair that changes governed meaning SHALL be classified as Self-Evolution.
- **AWR-001-v2-REQ-018:** FSA SHALL verify repair beyond process restart or response and preserve complete repair history.
- **AWR-001-v2-REQ-019:** FSA MAY create and test a distinct non-authoritative Foundation candidate only in an Approved isolated environment under separate authority.
- **AWR-001-v2-REQ-020:** FSA SHALL NOT approve, activate, deploy, promote, or appoint a candidate it created.
- **AWR-001-v2-REQ-021:** Every candidate SHALL undergo independent validation before Owner approval.
- **AWR-001-v2-REQ-022:** Every candidate SHALL undergo Sandbox evaluation and Digital City evaluation when required by consequence.
- **AWR-001-v2-REQ-023:** Owner approval SHALL be authenticated, attributable, candidate-specific, Evidence-Set-specific, explicit, auditable, and non-transferable.
- **AWR-001-v2-REQ-024:** Approval SHALL NOT directly cause deployment; competent deployment authorities and mechanisms remain required.
- **AWR-001-v2-REQ-025:** FSA MAY autonomously roll back only to the last Approved trusted state under Approved pre-authorized conditions.
- **AWR-001-v2-REQ-026:** FSA SHALL supervise Guardian technical readiness without changing Guardian authority, mandate, or restriction ownership.
- **AWR-001-v2-REQ-027:** A candidate successor to FSA SHALL require independent integrity and authority validation, Owner approval, protection continuity, and rollback.
- **AWR-001-v2-REQ-028:** FSA repair and evolution SHALL remain limited to authoritatively Foundation-owned subjects.
- **AWR-001-v2-REQ-029:** FSA SHALL NOT modify or reinterpret Application business logic, state, users, accounts, financial objects, decisions, or policies.
- **AWR-001-v2-REQ-030:** Candidate and repair failure SHALL stop bounded retries, preserve trusted state and evidence, reduce capability, and escalate.

## 32. Acceptance Evidence

Approval requires evidence that:

- Application business payloads remain opaque to FSA;
- customer, broker, portfolio, order, position, strategy, prediction, capital, profit, and loss data are not required in the Foundation Self Model;
- FSA detects Service Bus, FIL, persistence, configuration, documentation, resource, runtime, and lifecycle failures;
- FSA produces scoped technical fitness and conformance outcomes;
- Authority Engine and Guardian remain separate;
- FSA cannot approve its own authority expansion;
- conformance does not imply deployment or business approval;
- cross-tier summaries preserve ownership and privacy;
- historical assessments are reconstructable;
- compromised FSA can be isolated and independently recovered.
- repair restores only a previously Approved trusted state;
- unapproved or semantic-changing repair is rejected as evolution;
- repair playbooks enforce scope, retries, stop conditions, and evidence;
- post-repair verification detects incomplete and failed repair;
- candidate development cannot reach production authority or Application business state;
- FSA-created evidence cannot alone validate or promote its candidate;
- Sandbox and required Digital City results are present;
- Owner decisions are explicit and version-specific;
- approval does not directly deploy;
- automatic rollback restores and verifies the prior Approved state;
- Guardian technical readiness is supervised without mandate change;
- FSA cannot approve its candidate successor.

## 33. Unresolved Matters

- exact FSA conformance outcome catalog;
- exact Foundation Technical Fitness catalog;
- cross-tier summary contracts and schemas;
- constitution and membership of the Falcon Architecture Board;
- Digital City authority and evidence profile;
- resource-criticality catalog;
- FSA isolation and independent recovery realization.
- Foundation Repair Playbook Contract and catalog;
- candidate-development authority instrument;
- Sandbox and Digital City environment specifications;
- Owner Communication and Approval Center specification and identity profile;
- candidate lifecycle and outcome catalogs;
- post-adoption verification Contract;
- rollback authorization profile;
- final Guardian architecture.

## 34. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner | Pending | Pending | Pending |

This candidate has no authority until explicitly approved.
