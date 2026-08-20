# AUT-002 — Falcon Foundation Guardian

**Identifier:** AUT-002  
**Version:** Proposed 2.0  
**Status:** Approved Successor Design — Not Effective  
**Approval Record:** GOV-060  
**Owner:** Falcon Foundation Protection Authority, subject to an Approved authority charter  
**Governing Authority:** Falcon Vision; Falcon Constitution; AUT-001; proposed ADR-I010  
**Affected Domain:** Falcon Foundation and the technical runtime boundary of admitted Applications  
**Supersession:** Will supersede AUT-002 v1.0 only after separate controlled documentary activation  
**Implementation Authority:** Not Granted

## 1. Purpose

Falcon Foundation Guardian (FFG) is Falcon Foundation’s bounded emergency protection authority.

FFG protects Foundation integrity, availability, information flow, critical control capability, safe recovery, and admitted Application runtime continuity against technical harm.

FFG owns Foundation protective modes, including Platform Safe Mode.

FFG knows how an Application technically affects Falcon. It does not know what business the Application performs.

## 2. Identity and Position

FFG is separate from:

- Falcon Self-Awareness System;
- Health Monitoring;
- Authority Engine;
- Runtime and Lifecycle;
- Resource Management;
- Security;
- Service Bus and FIL;
- Persistence and Recovery; and
- Application-level protection authorities.

The responsibility chain is:

1. FSA and competent observers establish technical condition evidence.
2. FFG evaluates the need for emergency protection.
3. FFG issues a binding directive within its mandate.
4. The competent owning mechanism executes its technical operation.
5. Independent evidence supports restriction reduction or release.

Awareness rank, operational ownership, emergency authority, verification authority, and release authority SHALL remain distinct.

## 3. Authority

FFG authority SHALL derive from:

- the Falcon Constitution;
- Approved Guardian policy;
- AUT-001 or its Approved successor;
- Accepted ADRs;
- an Approved protection mandate; and
- explicit delegated emergency authority.

Every authority exercise SHALL declare jurisdiction, target, permitted action, consequence class, duration or review condition, evidence basis, and release requirements.

FFG SHALL NOT create, expand, reinterpret, or renew its own authority.

FFG MAY hold the highest bounded emergency intervention authority during an active Foundation protection condition. It is not Falcon’s universal highest authority.

## 4. Scope

FFG governs:

- Foundation protection-condition evaluation;
- heightened protection and technical containment;
- component, runtime, traffic, and Application isolation;
- suspension of harmful workloads;
- Platform Safe Mode;
- preservation of the minimum trusted control plane;
- emergency technical authority restriction;
- emergency resource-priority directives;
- cascading-failure prevention;
- information-flow quarantine;
- controlled transition into recovery;
- restriction persistence;
- release-condition evaluation;
- compromised-component containment; and
- protection evidence and escalation.

## 5. Non-Scope

FFG SHALL NOT:

- trade, select strategy, allocate capital, or evaluate profit and loss;
- interpret markets, instruments, portfolios, orders, positions, predictions, customers, balances, or exposure;
- inspect or alter Application business state to perform technical protection;
- decide whether an Application business action is correct;
- authenticate Application customers;
- own routine operation, ordinary lifecycle, ordinary resource scheduling, or ordinary maintenance;
- own Foundation Self-Repair or Self-Evolution;
- create or activate a new component version;
- modify the Constitution, approve architecture, or grant itself authority;
- alter or erase evidence;
- use emergency authority for performance optimization; or
- declare technical recovery complete without the required independent evidence and authority.

## 6. Technical Knowledge Boundary

FFG MAY consume governed technical information including:

- Application, component, runtime, service, and deployment identity;
- dependency and runtime state;
- approved technical criticality;
- resource consumption and communication condition;
- isolation, restart, failover, and recovery policy;
- maximum tolerated technical downtime;
- minimum technical dependencies and resources;
- approved degraded mode;
- technical envelope and validation outcome;
- authority and restriction state; and
- protection and recovery readiness.

FFG SHALL NOT require Application business meaning.

Applications and Application authorities SHALL expose only the minimum authorized technical treatment required by FFG. Business rationale and business payload remain outside FFG.

## 7. Technical Criticality

Every governed Foundation component and admitted Application SHALL reference an Approved technical criticality classification.

The minimum proposed classes are:

- `CRITICAL`;
- `ESSENTIAL`;
- `STANDARD`; and
- `OPTIONAL`.

The classification and its metadata SHALL be defined in a governed catalog and SHALL include its authority, scope, version, provenance, review conditions, and conflict rules.

FFG SHALL use technical criticality to select containment, preservation, resource priority, recovery order, and minimum evidence. FFG SHALL NOT infer it from commercial importance, financial value, or Application payload.

No component may preserve itself merely by claiming criticality.

## 8. Protective Modes

FFG SHALL support:

- `PLATFORM_NORMAL`;
- `PLATFORM_HEIGHTENED`;
- `PLATFORM_CONTAINMENT`;
- `PLATFORM_SAFE`; and
- `PLATFORM_RECOVERY_GUARD`.

### 8.1 PLATFORM_NORMAL

Ordinary Approved authority applies and no Foundation-wide protective condition is active. Normal does not assert perfect health.

### 8.2 PLATFORM_HEIGHTENED

Credible warning exists. FFG MAY increase observation and evidence, restrict risky change, prepare containment, notify competent authorities, and verify protection readiness.

### 8.3 PLATFORM_CONTAINMENT

FFG isolates the smallest trustworthy source or propagation path of technical harm while preserving unaffected operation.

### 8.4 PLATFORM_SAFE

When narrower containment is not trustworthy, FFG preserves the Approved minimum survival set and denies nonessential or unsafe activity. It SHALL preserve authority enforcement, evidence, recovery control, essential security, minimum communication and persistence, and technically safe critical workloads where possible.

### 8.5 PLATFORM_RECOVERY_GUARD

After immediate danger is controlled, restoration proceeds gradually under continuing restriction, verification, rollback readiness, dependency checks, and recurrence monitoring.

Time passage, restart, silence, or self-attestation SHALL NOT return Falcon to `PLATFORM_NORMAL`.

## 9. Protection Triggers

FFG MAY act on credible evidence of:

- crash, repeated crash, restart loop, deadlock, or uncontrolled execution;
- memory, CPU, storage, network, queue, or other resource exhaustion;
- message storm, invalid-message flooding, retry amplification, or critical route failure;
- corrupted Foundation or runtime state;
- persistence integrity or evidence integrity failure;
- unauthorized modification, authority use, or privilege escalation;
- security-control or mandatory Foundation-service failure;
- dependency, isolation, recovery, or enforcement failure;
- compromised Foundation component, FSA instance, or FFG instance;
- an Application threatening shared infrastructure or other Applications;
- cascading-failure risk;
- loss of technical traceability; or
- severe technical uncertainty where continued operation may cause material harm.

Mandatory thresholds SHALL be defined by an Approved trigger matrix.

FFG need not wait for certainty when potential harm is severe. It SHALL choose the smallest trustworthy protective scope.

## 10. Evidence

FFG MAY consume evidence from FSA, Health Monitoring, Runtime, Lifecycle, Resources, FIL, Service Bus, Security, Persistence, AUT-001, independent watchdogs, audit, authorized operators, admitted technical manifests, and governed summaries from Application authorities.

FFG SHALL NOT rely exclusively on:

- the actor it may restrict;
- its own self-report;
- FSA self-report when FSA may be compromised; or
- one source lacking the verification required by the consequence class.

Contradictory evidence and uncertainty SHALL remain visible. Unknown SHALL NOT be treated as safe.

## 11. Protective Directives

Within its mandate, FFG MAY issue directives including:

- warn or increase monitoring;
- restrict or block change;
- throttle, quarantine, isolate, suspend, or preserve a governed technical target;
- restrict or prioritize resource use;
- preserve a critical route;
- request restart, failover, rollback, or trusted restore;
- enter containment, safe, or recovery-guard mode;
- hold recovery or block release; and
- request emergency termination.

Each directive SHALL be typed, attributable, authorized, scoped, integrity-protected, expiring or review-bound, persistent as required, and acknowledged by the execution owner.

The owning mechanism executes the directive. FFG SHALL NOT silently assume ownership of that mechanism.

## 12. Application Isolation

FFG MAY technically isolate an admitted Application when its behavior threatens Foundation, shared resources, communication integrity, persistence integrity, authority enforcement, recovery capability, or other Applications.

Isolation MAY stop traffic, block publication, remove runtime access, restrict resources, suspend execution, prevent restart, preserve evidence, and maintain storage boundaries.

FFG SHALL record the cause, evidence, authority, affected dependencies, preserved operation, mode, and release conditions. It SHALL NOT inspect or modify business state to perform isolation.

## 13. Resource and Information-Flow Protection

During an active protection condition, FFG MAY direct competent mechanisms to preserve critical control capacity, throttle abnormal consumers, suspend optional workloads, protect recovery capacity, quarantine malformed or abusive technical traffic, limit retry loops, preserve control routes, and block unauthorized communication.

FFG MAY inspect technical envelope, identity, version, contract reference, rate, size, route, integrity, authorization, expiry, and replay state.

FFG SHALL NOT interpret business payload meaning.

## 14. Relationship with FSA

FSA SHALL evaluate FFG technical readiness periodically and in response to relevant events, using independent evidence and safe tests where possible.

FSA MAY verify FFG availability, identity, version, configuration, policy loading, authority connectivity, Safe Mode and isolation paths, restriction persistence, evidence, standby, failover, stop channel, and recovery readiness.

Under separately Approved repair authority and playbooks, FSA MAY isolate an unhealthy FFG instance, restore Approved configuration or version, activate an Approved standby, reconnect Approved dependencies, and verify readiness.

FSA SHALL NOT change FFG mandate, release its restrictions, approve its own Guardian candidate, or activate a newly created Guardian version.

FFG MAY restrict unsafe FSA behavior only within explicit mandate and on credible evidence independent of the affected FSA instance. FFG SHALL NOT modify FSA.

## 15. Relationship with Authority and Execution Owners

AUT-001 owns authority validation and delegated authority state. FFG MAY request restriction, suspension, or revocation but SHALL NOT invent permission.

Loss of AUT-001 connectivity SHALL restrict FFG to explicitly pre-authorized fail-safe actions.

Health Monitoring owns health observations. FFG owns the protection consequence.

Runtime, Lifecycle, Resources, Security, Service Bus, FIL, Persistence, and Recovery own their respective technical operations and results. A valid FFG directive is binding within its scope, but it does not transfer ownership.

## 16. Relationship with Repair and Evolution

FFG protects Falcon while FSA and competent mechanisms perform repair.

The governed sequence is:

1. condition detection and diagnosis;
2. protection evaluation;
3. restriction or isolation;
4. selection of an Approved repair playbook;
5. repair execution by the competent owner;
6. independent technical verification;
7. FFG release-condition evaluation;
8. authorized progressive restoration.

FFG does not perform Self-Evolution. It MAY prevent admission or deployment while a candidate violates an Approved protection threshold.

## 17. Future Application Guardian

A future Application Guardian MAY protect capital, financial exposure, orders, positions, Application business continuity, and domain-specific safety only under a separate Approved specification and authority charter.

It MAY submit an abstract technical protection request containing the requesting authority, technical target, requested capability, technical criticality reference, duration, release conditions, and evidence reference.

FFG SHALL validate authority, technical feasibility, conflicts, and Foundation safety. It SHALL NOT inspect the business rationale.

AMD-005 does not create or authorize an Application Guardian.

## 18. Intervention Record

Every intervention SHALL preserve:

- intervention, Guardian, mandate, and target identity;
- trigger, evidence, confidence, uncertainty, and contradictions;
- authority source and consequence class;
- affected, isolated, and preserved scope;
- selected mode and issued directives;
- execution owner, expected effect, actual effect, and failed actions;
- unresolved risk, release conditions, and recovery state;
- trusted time observations;
- restriction persistence and failover state; and
- final disposition and decision authority.

The record SHALL be an immutable governed Trust Object. Correction occurs by superseding evidence, never historical rewrite.

## 19. Restriction Persistence

Unresolved restrictions SHALL survive process, component, Application, Guardian, Runtime, and Foundation restart and ordinary deployment.

The trusted active restriction state SHALL be restored before broader authority or ordinary operation resumes.

Restart SHALL NOT clear risk.

## 20. Release

Restriction reduction or release SHALL require:

- evidence that the trigger is resolved or acceptably contained;
- FSA verification when technical repair is involved;
- independent verification required by consequence class;
- restored authority connectivity or an Approved exception;
- successful recovery checks;
- no unresolved cascading risk; and
- the release authority defined for that consequence class.

The producer of the triggering condition, the restricted actor, FSA, or FFG SHALL NOT be the sole conclusive release authority in a material case involving its own conduct.

Outcomes SHALL distinguish maintained, reduced, recovery guard, normal, repair required, continuing isolation, and Owner review required.

## 21. Guardian Failure and Compromise

Loss or uncertainty of FFG SHALL be treated as material protection loss and SHALL NOT be reported as normal.

Dependent high-risk activity SHALL reduce or cease. Existing restrictions remain. Independent protection channels remain available. An Approved standby MAY be activated only after identity, version, configuration, policy, and authority verification.

A compromised FFG SHALL be isolatable without clearing protection. It SHALL NOT release its own restrictions, erase history, expand authority, disable independent protection, or change its mandate.

FFG SHALL NOT be an unprotected single point of failure. High availability and the independent stop channel require a separate Accepted ADR.

## 22. Normative Requirements

- **AUT-002-v2-REQ-001:** FFG SHALL operate only under explicit Approved jurisdiction, mandate, and authority.
- **AUT-002-v2-REQ-002:** FFG SHALL protect Foundation technical integrity without interpreting Application business meaning.
- **AUT-002-v2-REQ-003:** FFG SHALL own Platform Safe Mode and Foundation protective restrictions within its mandate.
- **AUT-002-v2-REQ-004:** Protective action SHALL be proportionate to harm, uncertainty, reversibility, scope, and consequence.
- **AUT-002-v2-REQ-005:** Trustworthy narrow containment SHALL be preferred over unnecessary platform-wide suspension.
- **AUT-002-v2-REQ-006:** Unaffected higher-priority technical operation SHALL be preserved when safe.
- **AUT-002-v2-REQ-007:** FFG SHALL use Approved technical criticality and SHALL NOT infer business priority.
- **AUT-002-v2-REQ-008:** FFG SHALL NOT rely exclusively on the actor it may restrict.
- **AUT-002-v2-REQ-009:** Severe technical uncertainty SHALL favor protection over unsafe continuation.
- **AUT-002-v2-REQ-010:** Restrictions SHALL persist across restart and failover while unresolved.
- **AUT-002-v2-REQ-011:** Release SHALL require authorized recovery evidence and consequence-appropriate authority.
- **AUT-002-v2-REQ-012:** Time, restart, silence, and self-attestation SHALL NOT restore normal operation.
- **AUT-002-v2-REQ-013:** FFG SHALL preserve immutable intervention evidence.
- **AUT-002-v2-REQ-014:** Mandatory failure to intervene SHALL be observable as a protection failure.
- **AUT-002-v2-REQ-015:** FFG SHALL remain independently observable, interruptible, isolatable, and auditable.
- **AUT-002-v2-REQ-016:** Isolation or loss of one FFG instance SHALL NOT silently remove all independent protection.
- **AUT-002-v2-REQ-017:** FFG SHALL NOT own routine operation, Foundation Self-Repair, or Self-Evolution.
- **AUT-002-v2-REQ-018:** FFG SHALL NOT create or expand its authority.
- **AUT-002-v2-REQ-019:** FFG SHALL NOT use emergency authority for ordinary optimization.
- **AUT-002-v2-REQ-020:** FFG and any Application Guardian SHALL remain separate.
- **AUT-002-v2-REQ-021:** Application-level protection requests SHALL be authorized, abstract, minimal, and technical.
- **AUT-002-v2-REQ-022:** FSA and FFG SHALL NOT conclusively self-validate in a material dispute.
- **AUT-002-v2-REQ-023:** Directive execution SHALL remain owned by the competent Foundation mechanism.
- **AUT-002-v2-REQ-024:** Emergency authority SHALL end or require authorized renewal when its lawful condition or duration ends.

## 23. Invariants

1. FFG protects Falcon’s technical body; it does not manage Application business.
2. FFG isolates technical harm; it does not judge business success.
3. FFG may restrict authority; it may not invent authority.
4. FFG manages Platform Safe Mode; it does not own ordinary operation.
5. Restart does not erase unresolved protection.
6. Unknown is not normal.
7. Awareness, restriction, execution, verification, and release remain separate.
8. Emergency authority ends when its lawful protection condition ends.
9. FSA diagnoses and repairs; FFG protects and restricts.
10. No Guardian determines the truth or lawful scope of its own material challenge.

## 24. Acceptance Evidence

Approval for realization requires the complete evidence cases defined by proposed `VPL-GDN-001`, including isolation, unaffected-operation preservation, Safe Mode survival, restart and failover persistence, unauthorized-release denial, compromise handling, mutual FSA/FFG supervision, business-knowledge rejection, and complete intervention reconstruction.

## 25. Unresolved Matters

- exact technical criticality catalog;
- Safe Mode survival set;
- mandatory trigger thresholds;
- high-availability topology;
- independent stop channel;
- pre-authorized actions during AUT-001 loss;
- release authority by consequence class;
- maximum autonomous containment duration;
- quorum for irreversible emergency action;
- Application Guardian identifier, charter, and request Contract; and
- manual Owner emergency channel.

These matters are not silently decided by this specification.
