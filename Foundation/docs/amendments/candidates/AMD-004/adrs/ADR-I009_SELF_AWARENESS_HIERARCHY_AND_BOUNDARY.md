# ADR-I009 — Establish Falcon Self-Awareness and Separate Foundation Awareness from Application Awareness

**Identifier:** ADR-I009  
**Candidate Revision:** 0.2  
**Status:** Accepted — Documentary Activation Deferred  
**Decision Record:** GOV-061  
**Date:** 2026-07-27  
**Decision Owner:** Falcon Project Owner  
**Scope:** Falcon self-awareness hierarchy and Foundation/Application boundary  
**Approval Requirement:** Explicit Project Owner approval  
**Affected Specifications:** AWR-001, AWR-002–AWR-005, proposed AWR-006–AWR-008, AUT-001, AUT-002, SYS-001, SYS-005, SYS-008, SYS-009, SYS-011, OPS-003, OPS-004, SEC-001, EVO-001, DEC-006, RSK-005, PLG-001, APP-001  
**Affected Contracts:** CON-002, CON-006, CON-009, CON-011  
**Affected Catalogs:** FDN-001, FDN-002, FDN-004, FDN-005  
**Applicable Standards:** STD-003, STD-004, STD-006, STD-007, STD-009, STD-010, STD-011  
**Related ADRs:** ADR-F001, ADR-F002, ADR-F008, ADR-I003, ADR-I004, ADR-I005, ADR-I006, ADR-I007, ADR-I008  
**Supersedes:** None  
**Superseded By:** None  
**Implementation Authority:** Not Granted  
**Stage 1 Authority:** Not Granted

## 1. Context

Approved AWR-001 v1.0 defines a single Core Self-Awareness System with operational, financial, decisional, capability, dependency, temporal, and authority awareness. That design predates the explicit separation between Falcon Foundation and Application-owned business understanding.

Falcon requires Foundation awareness capable of protecting platform integrity without absorbing the business meaning, users, data, decisions, or responsibilities of installed Applications. It also requires awareness at the Applications ecosystem, individual Application, and eligible intelligent-component levels.

The prior informal use of “Main Self-Awareness” for Foundation awareness is no longer correct.

## 2. Problem Statement

A single unified awareness owner creates four risks:

1. Foundation could become coupled to Application business models.
2. Application users and financial facts could leak into Foundation state.
3. Fitness and authority could be interpreted without a declared scope.
4. A final conformance gate could be mistaken for supreme constitutional or business authority.

## 3. Decision Drivers

- preserve Foundation domain independence;
- protect Falcon without interpreting Application business meaning;
- maintain awareness appropriate to every ownership boundary;
- preserve one accountable owner for each awareness assertion;
- prevent higher awareness rank from acquiring lower-level responsibility;
- preserve Guardian, Authority Engine, Security, Risk, and governance separation;
- support controlled change admission;
- keep awareness evidence attributable, reconstructable, and privacy-minimized;
- preserve Approved history.

## 4. Higher-Authority Constraints

This decision is constrained by:

- the Vision’s Prime Objective and explicit-boundary principle;
- Constitution Articles 6, 11, 13, 15–19, 21, 24–26, 28–36D, and 39–44;
- GOV-001 document-authority rules;
- GOV-AUT-001 jurisdiction and delegation rules;
- SYS-001 minimal Core and domain independence;
- AUT-001 separation between evidence and authority;
- AUT-002 ownership of protective restriction;
- SEC-001 identity, minimum access, and sensitive-information rules;
- ADR-F002 single authoritative owner per state class.

## 5. Decision

Falcon SHALL establish four bounded awareness tiers:

```text
FSA — Falcon Self-Awareness System
  ↓
MSA — Main Self-Awareness
  ↓
LSA — Local Self-Awareness
  ↓
CSA — Component Self-Awareness
```

The direction expresses awareness rank, scope, evidence aggregation, escalation, and conformance relationships. It does not create command inheritance, business ownership, jurisdiction inheritance, or unrestricted data visibility.

### 5.1 FSA

FSA is the Foundation-level operational, structural, integrity, and conformance-awareness system.

FSA is the highest awareness tier in Falcon for:

- Foundation protection;
- Falcon-wide technical impact awareness;
- Foundation integrity;
- awareness-boundary escalation; and
- final Falcon conformance review for proposed admission.

FSA SHALL understand how Falcon operates as a governed platform.

FSA SHALL NOT understand or own what an Application does as a business product.

### 5.2 MSA

MSA is the highest awareness tier inside the Falcon Applications environment.

MSA SHALL understand the Applications ecosystem, general Application purposes, relationships, collective capabilities, dependencies, conflicts, incidents, readiness, and cross-Application impact.

MSA SHALL NOT replace any LSA or become a Foundation component.

### 5.3 LSA

Each first-class Falcon Application or Approved Operating Layer SHALL own one LSA unless an Approved specification explicitly establishes an equivalent bounded arrangement.

LSA SHALL understand the owning Application’s purpose, business-domain condition, capabilities, limitations, dependencies, performance, failures, confidence, gaps, and improvement opportunities.

LSA SHALL NOT acquire cross-Application authority by existence.

### 5.4 CSA

A CSA MAY exist only for an explicitly eligible intelligent component.

CSA SHALL understand only its owned component, specialization, output quality, performance, confidence, limitations, weaknesses, capability gaps, improvement opportunities, and owned tools, models, methods, and code.

CSA SHALL NOT self-approve, modify Foundation components, or acquire Application-wide authority.

## 6. Responsibility Boundary

| Tier | Owns | May receive | Shall not own or interpret |
|---|---|---|---|
| FSA | Foundation Self Model; Foundation Technical Fitness; Foundation conformance assessment | technical state, declared criticality, opaque impact, governed summaries | Application users, customers, accounts, markets, portfolios, orders, positions, strategies, predictions, capital, P/L, business decisions |
| MSA | Applications ecosystem model and collective readiness | governed summaries from LSAs | an Application’s private internal truth or Foundation control |
| LSA | one Application’s Self Model and business/application fitness | CSA evidence and technical Foundation status | other Applications’ private state or Foundation authority |
| CSA | one eligible component’s bounded Self Model | owned component evidence | Application-wide or cross-Application authority |

## 7. Authority Boundary

Awareness produces observations, interpretations, fitness assessments, impact assessments, and conformance outcomes. Awareness does not manufacture jurisdiction.

- FSA conformance does not amend the Constitution.
- FSA does not replace the Project Owner or a competent governance authority.
- FSA does not bypass the Falcon Architecture Board where that Board has valid jurisdiction.
- FSA does not grant business, financial, trading, risk, or deployment approval.
- MSA, LSA, and CSA do not acquire Foundation authority.
- Guardian remains the authoritative owner of protective restrictions within its mandate.
- Authority Engine remains the owner of Falcon authority decisions.
- Security Authority remains the owner of security policy and trust decisions within its jurisdiction.
- Risk and domain authorities remain owners of financial and business meaning.

Delegation SHALL NOT create jurisdiction absent from the governing model.

## 8. FSA Conformance Boundary

FSA SHALL be the final self-awareness and Falcon conformance gate for a change proposed for admission into Falcon.

FSA SHALL evaluate whether the proposed admission conforms to applicable:

- Vision and Constitution;
- Approved architecture and specifications;
- ownership and authority boundaries;
- security and information-integrity requirements;
- isolation and resource requirements;
- testing and evidence obligations;
- recovery requirements;
- rollback requirements where rollback is possible;
- corrective-action requirements where rollback is impossible; and
- change-governance rules.

Permitted conformance outcomes are governed values and may include:

- `CONFORMING`;
- `CONFORMING_WITH_CONDITIONS`;
- `NON_CONFORMING`;
- `MORE_EVIDENCE_REQUIRED`;
- `SANDBOX_REQUIRED`;
- `DIGITAL_CITY_REQUIRED`;
- `SECURITY_REVIEW_REQUIRED`;
- `AUTHORITY_REVIEW_REQUIRED`;
- `OWNERSHIP_CLARIFICATION_REQUIRED`;
- `RECOVERY_PLAN_REQUIRED`;
- `ROLLBACK_PLAN_REQUIRED`; and
- `ARCHITECTURE_CORRECTION_REQUIRED`.

The exact catalog requires later approval.

FSA conformance approval means only that the candidate satisfies Falcon admission rules within FSA’s declared jurisdiction. Separate acceptance, architecture, Owner, release, deployment, business, risk, and financial approvals remain required where applicable.

FSA SHALL NOT be the sole authority approving:

- expansion of FSA jurisdiction;
- changes to its governing specification;
- removal of its required review;
- a constitutional amendment;
- a disputed material Claim produced by FSA; or
- a change whose governing policy requires independent approval.

## 9. Information Abstraction

Lower tiers SHALL disclose upward only information necessary for the declared purpose.

FSA may know:

- Application identity and admitted version;
- lifecycle, availability, health, dependency, and resource state;
- technical communication and persistence state;
- declared technical criticality;
- abstract impact and required protective handling;
- evidence identity, provenance, integrity, freshness, and classification.

FSA shall not require raw Application business records merely to establish technical awareness.

Business identifiers inside Application-owned payloads do not become Foundation-owned concepts merely because FIL transports them or Foundation infrastructure protects them.

## 10. Service Bus, FIL, Persistence, and Documentation

FSA SHALL supervise:

- Service Bus availability, routing health, delivery health, backlog, saturation, dead-letter condition, duplication patterns, and recovery;
- FIL envelope, identity, version, correlation, integrity, compatibility, technical authorization, and traceability;
- technical information-flow integrity and propagation risk;
- Foundation persistence integrity and Application-storage technical availability, isolation, security, backup, and restore condition;
- authoritative Foundation documentation integrity, approval state, provenance, baseline consistency, and unauthorized modification.

FSA SHALL NOT interpret Application business payloads, database records, or documentation content beyond the technical metadata necessary for protection and conformance.

## 11. Alternatives Considered

### 11.1 Retain one unified Core Self-Awareness System

Rejected. It allows business meaning to enter Foundation and obscures ownership.

### 11.2 Rename all existing Self-Awareness references to FSA

Rejected. A blind replacement would incorrectly move Application and component awareness into Foundation.

### 11.3 Place all awareness outside Foundation

Rejected. Falcon requires independent awareness of Foundation integrity, operation, evidence, and conformance.

### 11.4 Independent awareness tiers with no hierarchy

Rejected. It preserves local ownership but lacks governed escalation and Falcon-wide conformance integration.

### 11.5 Hierarchical awareness with bounded ownership

Selected. It supports escalation and protection while preserving domain independence.

## 12. Positive Consequences

- Foundation remains stable and Application-independent.
- Applications can evolve without teaching Foundation their business models.
- Awareness state has clear ownership.
- Privacy and data minimization improve.
- FSA can protect technical infrastructure and admission boundaries consistently.
- MSA can reason across Applications without becoming Foundation.
- LSA and CSA retain local expertise.
- Fitness and conformance can be scoped explicitly.

## 13. Trade-offs

- More contracts and evidence abstractions are required.
- Cross-tier summaries must be designed and versioned.
- Contradictions between tiers require reconciliation rules.
- Conformance review may add admission latency.
- FSA must remain useful without demanding raw business data.
- Independent challenge and escalation mechanisms become mandatory.

## 14. Migration Impact

- AWR-001 v1.0 remains Approved until a successor is approved.
- Proposed AWR-001 v2.0 defines FSA.
- Proposed AWR-006, AWR-007, and AWR-008 define MSA, LSA, and CSA.
- AWR-002 through AWR-005 retain their existing reservations and require later tier-aware alignment.
- CON-006, FDN-001, FDN-002, and FDN-004 require versioned successors.
- Accepted ADRs are not rewritten; old Foundation-awareness references map to FSA when their subject is technical Foundation awareness.
- Application-domain awareness requirements move to MSA/LSA/CSA or competent domains without deletion.

## 15. Compatibility Requirements

- Existing technical identity, authority, security, FIL envelope, evidence, persistence, and Guardian semantics remain valid.
- No higher tier may silently reinterpret a lower tier’s authoritative fact.
- No lower tier may claim Foundation conformance.
- Every cross-tier message must declare producer, scope, abstraction, authority, evidence, freshness, and privacy classification.
- Historical AWR-001 v1.0 decisions remain attributable to the governing version used at the time.

## 16. Stage 1 Implications

Stage 1 remains blocked.

After Owner approval, Stage 1 planning may rely on this architecture only if separately authorized. No implementation authority follows from this ADR.

Before implementing awareness:

- activate the revised specifications;
- approve contract and catalog successors;
- approve cross-tier message and evidence boundaries;
- resolve the status and jurisdiction of the Falcon Architecture Board;
- establish acceptance tests for business-knowledge non-leakage.

## 17. Risks and Mitigations

- **Risk:** FSA becomes a universal decision authority.  
  **Mitigation:** restrict FSA to awareness and conformance; preserve competent acceptance authorities.

- **Risk:** FSA requires raw business data.  
  **Mitigation:** enforce minimal governed summaries and prohibit business interpretation.

- **Risk:** MSA centralizes all Application truth.  
  **Mitigation:** LSAs retain authoritative Application awareness; MSA aggregates only declared summaries.

- **Risk:** CSA self-promotes changes.  
  **Mitigation:** require LSA/MSA impact handling, FSA conformance, and independent approval.

- **Risk:** hierarchy is interpreted as command authority.  
  **Mitigation:** state explicitly that rank is awareness and escalation, not jurisdiction inheritance.

- **Risk:** FSA approves its own expansion.  
  **Mitigation:** require external competent governance and Owner approval.

## 18. Approval Conditions

Approval requires:

1. explicit Project Owner approval of this ADR;
2. approval of the FSA, MSA, LSA, and CSA specifications;
3. confirmation that no constitutional amendment is required;
4. acceptance of proposed identifiers;
5. approval of migration and historical-preservation treatment;
6. recorded decision on the Falcon Architecture Board dependency;
7. continued prohibition on Stage 1 absent separate authority.

## 19. Bounded Foundation Self-Repair

FSA MAY autonomously orchestrate bounded repair of a Foundation-owned subject only by restoring a previously Approved, trusted, compatible, non-revoked state through an Approved repair playbook and valid authority.

Self-Repair may include:

- restart or controlled replacement with an Approved equivalent;
- failover to an Approved standby;
- reconnection of an Approved dependency;
- reload of Approved configuration;
- rollback to the last Approved trusted version;
- restoration from an Approved backup or recovery point;
- restoration of Approved FIL, routing, documentation, index, or queue baselines;
- isolation, quarantine, evidence preservation, and gradual return to service.

Self-Repair SHALL NOT:

- create a new production state;
- deploy candidate code;
- change architecture, specification, Contract meaning, jurisdiction, security control, audit obligation, or Guardian mandate;
- erase triggering evidence;
- claim success without post-repair verification;
- convert a candidate into a trusted baseline.

Where no Approved playbook exists, FSA may contain, isolate, preserve evidence, reduce capability, and escalate. It may prepare a repair candidate through Self-Evolution, but may not improvise a material production repair.

## 20. Controlled Foundation Self-Evolution

FSA MAY initiate Controlled Foundation Self-Evolution when evidence demonstrates a sustained Foundation-owned weakness, recurring failure, capability gap, insufficient performance, security weakness, compatibility weakness, recovery deficiency, resource inefficiency, or inability to meet an Approved requirement, and ordinary Self-Repair is insufficient.

Within an Approved isolated candidate environment and separate authority, FSA MAY:

- investigate and research;
- compare alternatives and assess root cause;
- design a candidate;
- create or modify candidate source, configuration, schema, tool, test, and documentation artifacts within owned scope;
- build and test a distinct candidate version;
- validate functional, performance, compatibility, containment, security, authority, recovery, and rollback properties;
- prepare conformance evidence and an Owner Approval Package;
- recommend adoption, revision, deferral, or rejection.

FSA SHALL NOT:

- represent a candidate as trusted or operational;
- approve a candidate it created;
- grant production authority;
- modify the active trusted version during candidate development;
- use live production authority or Application business state;
- activate, deploy, promote, or appoint the proposed replacement.

> FSA may build the proposed replacement. FSA may not appoint the proposed replacement.

## 21. Candidate Isolation and Lifecycle

Every candidate SHALL have:

- distinct identity, version, provenance, owner, authority, environment, permissions, dependencies, evidence, and lifecycle;
- no inherited production authority;
- reproducible build and independent validation;
- isolation from active Foundation state, live security policy, live Guardian authority, production persistence, and Application business data;
- explicit cleanup, rejection, retention, and supersession behavior.

The governed lifecycle SHALL distinguish:

```text
WEAKNESS_DETECTED
  → INVESTIGATION_AUTHORIZED
  → CANDIDATE_DESIGN
  → CANDIDATE_BUILD
  → SANDBOX_VALIDATION
  → DIGITAL_CITY_VALIDATION when required
  → INDEPENDENT_VALIDATION
  → OWNER_APPROVAL_PENDING
  → APPROVED_FOR_CANARY / APPROVED_FOR_PRODUCTION / APPROVED_WITH_CONDITIONS
  → AUTHORIZED_DEPLOYMENT
  → POST_ADOPTION_VERIFICATION
  → TRUSTED_BASELINE
```

Terminal or non-progressing outcomes include `REJECTED`, `DEFERRED`, `CHANGES_REQUIRED`, `MORE_EVIDENCE_REQUIRED`, `ISOLATED`, and `ROLLED_BACK`.

No lifecycle transition may infer approval from elapsed time, successful tests, prior-version approval, FSA recommendation, or silence.

## 22. Validation and Digital City

Every candidate SHALL undergo an Approved Sandbox evaluation.

Digital City evaluation SHALL be mandatory when the candidate may affect multiple components or Applications, FIL, Service Bus, persistence, runtime, lifecycle, security, authority enforcement, Guardian, resource allocation, Safe Mode, recovery, shared communication, shared integrity, cascading failure, or system-wide readiness.

FSA SHALL record why Digital City is or is not required.

Candidate validation SHALL cover applicable:

- functional and Contract behavior;
- performance, stability, capacity, and resource use;
- backward and dependent-component compatibility;
- failure, failover, and cascading containment;
- security, ownership, and authority boundaries;
- recovery, rollback, evidence, and historical integrity;
- Vision, Constitution, architecture, specification, and governance conformance.

## 23. Independent Validation and Self-Modification

FSA SHALL NOT be the sole producer, evaluator, completeness authority, conformance challenger, or promotion authority for a candidate it created.

Independent validation SHALL precede Owner approval.

When FSA creates a candidate successor to FSA:

- active FSA SHALL NOT approve it;
- independent integrity and authority-boundary validation are mandatory;
- Guardian and Foundation protection continuity SHALL be preserved;
- Project Owner approval is mandatory;
- rollback to the previous trusted FSA SHALL remain available;
- no candidate FSA may inherit active FSA jurisdiction before authorized admission.

## 24. Owner Communication and Approval

FSA SHALL present a complete candidate case to the authenticated Project Owner through the Owner Communication and Approval Center or an Approved equivalent.

The interface conveys a proposal; it does not manufacture approval.

Every Owner decision SHALL be:

- authenticated and attributable;
- timestamped;
- bound to exactly one candidate identity and evidence package;
- explicit about deployment stage and conditions;
- auditable and historically preserved;
- non-transferable to a materially different candidate.

Permitted Owner outcomes include:

- `APPROVE_FOR_CANARY`;
- `APPROVE_FOR_PRODUCTION`;
- `APPROVE_WITH_CONDITIONS`;
- `REQUEST_CHANGES`;
- `REQUEST_MORE_EVIDENCE`;
- `DEFER`; and
- `REJECT`.

Silence, elapsed time, or another version’s approval SHALL NOT constitute approval.

## 25. Deployment, Post-Adoption Verification, and Rollback

Owner approval does not itself cause replacement.

Deployment SHALL proceed only through separately authorized Release, Runtime, Lifecycle, Security, Authority, evidence, and operational mechanisms under the Approved plan.

FSA may supervise authorized staged deployment, including Shadow, Canary, limited activation, progressive expansion, monitoring, and post-adoption verification. FSA SHALL NOT expand deployment scope without further authority.

A candidate SHALL NOT become a trusted baseline merely because deployment completed. Post-adoption verification and satisfaction of Owner conditions are mandatory.

FSA MAY autonomously roll back to the last Approved trusted version when:

- the rollback condition and playbook were Approved in advance;
- candidate behavior violates an Approved threshold;
- security, Foundation integrity, dependency, information flow, containment, or post-adoption verification fails; and
- rollback remains within the granted repair authority.

Rollback is Self-Repair. It SHALL preserve evidence, verify restoration, notify the Owner, prevent automatic reactivation, and require new approval for a materially revised candidate.

## 26. Guardian Readiness Supervision

FSA SHALL supervise only the technical readiness of any Foundation-owned Guardian capability through periodic and event-driven independent evidence and safe readiness tests where possible.

FSA MAY assess runtime availability, heartbeat, version and configuration integrity, policy loading, Authority Engine connectivity, Safe Mode and isolation paths, restriction persistence, evidence path, failover, standby, independent stop channel, Recovery Guard, and dependency readiness.

When Guardian readiness is insufficient, FSA may:

- mark protection degraded or unavailable;
- preserve active restrictions;
- reduce activity dependent on Guardian;
- request or activate Approved independent technical protection paths within authority;
- initiate an Approved bounded Guardian repair;
- restore an Approved Guardian runtime, standby, configuration, or version;
- verify technical readiness and preserve evidence;
- escalate when trusted protection cannot be restored.

FSA SHALL NOT:

- change Guardian jurisdiction or protective mandate;
- release Guardian restrictions;
- approve or activate a newly created Guardian candidate;
- disable independent protection;
- decide the complete Guardian architecture through this ADR.

## 27. Foundation and Application Scope

Repair and evolution authority applies only to a subject proven by authoritative documents to be Foundation-owned.

Shared or reusable status does not establish Foundation ownership.

FSA SHALL NOT repair, evolve, modify, or reinterpret Application:

- business logic or code absent separate explicit delegation;
- users, customers, authentication, accounts, subscriptions, or credentials as business objects;
- markets, instruments, portfolios, orders, positions, trades, capital, P/L, exposure, strategies, predictions, risk policies, decisions, or domain records.

FSA may repair the platform hosting an Application. It does not repair the Application’s business decisions.

## 28. Additional Risks and Mitigations

- **Risk:** repair is used to deploy new behavior.  
  **Mitigation:** require identity equality with an Approved trusted state and reject semantic change as evolution.

- **Risk:** FSA becomes developer, validator, and promoter.  
  **Mitigation:** enforce independent validation, evidence completeness, Owner approval, and separate deployment authority.

- **Risk:** repair loops amplify failure.  
  **Mitigation:** bounded retry, stop conditions, isolation, and escalation.

- **Risk:** candidate environment reaches production.  
  **Mitigation:** explicit isolation, no live authority, and boundary verification.

- **Risk:** Guardian repair changes mandate.  
  **Mitigation:** allow technical restoration only; prohibit authority and mandate changes.

- **Risk:** Owner interface implies approval.  
  **Mitigation:** authenticated version-specific explicit decisions; silence is denial/no decision.

## 29. References

- Falcon Vision v1.0
- Falcon Constitution v1.0
- GOV-001 v1.2
- GOV-AUT-001
- AWR-001 v1.0
- AUT-001
- AUT-002
- SYS-001
- SYS-005
- SYS-009
- SEC-001
- ADR-F001
- ADR-F002
- ADR-F008
- ADR-I007
- EVO-001
- PIPE-001
- PLG-001
- CON-011
- FDN-005

## 30. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner | Pending | Pending | Pending |
