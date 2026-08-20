# AUT-002 — Falcon Foundation Guardian

**Identifier:** AUT-002  
**Version:** Proposed 2.1  
**Status:** Approved Successor Design — Not Effective  
**Approval Record:** GOV-062  
**Owner:** Falcon Foundation Protection Authority, subject to Approved charter  
**Governing Sources:** Vision; Constitution; AUT-001; proposed ADR-I011  
**Refines:** AUT-002 v2.0 Approved successor design  
**Would Supersede:** AUT-002 v1.0 only through separate activation  
**Stage 1 Authority:** Not Granted

## 1. Purpose

FFG is Falcon Foundation’s bounded technical emergency protection authority. It contains faults, governs Platform protective modes, preserves the minimum trusted control plane, and controls release from Foundation restrictions.

FFG protects Applications as technical workloads. It does not understand their business.

## 2. Position and Separation

FFG is inside Foundation and separate from FSA, AUT-001, Health Monitoring, Runtime, Lifecycle, Resources, Security, FIL, Service Bus, Persistence, Recovery, and every Application Guardian.

FSA establishes technical awareness and repair evidence. FFG selects and owns Foundation protective restrictions. Competent mechanisms execute their owned actions.

## 3. Scope

FFG governs:

- technical warning, containment, isolation, and Platform Safe Mode;
- component, runtime, sender, route, resource-consumer, and Application isolation;
- shared-resource and information-flow protection;
- fault propagation and cascading-failure prevention;
- emergency technical priority;
- restriction persistence;
- technical recovery guarding;
- cross-Application protection-request decisions; and
- Platform restriction reduction and release.

## 4. Non-Scope

FFG SHALL NOT:

- understand capital, exposure, portfolios, orders, positions, trades, accounting, customers, patients, inventory, strategy, or business payload;
- decide Application-domain safety;
- trade or execute broker actions;
- own ordinary lifecycle, resources, communication, persistence, security, repair, or evolution;
- accept an Application request blindly;
- invent or expand authority;
- approve architecture or its own recovery; or
- alter evidence.

## 5. Knowledge Boundary

FFG MAY know identity, technical criticality, dependency, runtime state, resource use, communication state, isolation policy, recovery priority, maximum technical downtime, required technical capabilities, and approved degraded modes.

Business reason SHALL remain outside FFG. Technical claims derived from business danger SHALL be minimized, attributable, integrity-protected, and independently verifiable.

## 6. Technical Criticality

FFG SHALL use only an Approved technical-criticality classification and metadata. Minimum proposed classes are `CRITICAL`, `ESSENTIAL`, `STANDARD`, and `OPTIONAL`.

Criticality SHALL NOT be inferred from business value. Conflicts unresolved by technical policy SHALL be protected conservatively and escalated without invented business priority.

## 7. Platform Modes

- `PLATFORM_NORMAL`: ordinary Approved technical operation.
- `PLATFORM_HEIGHTENED`: increased evidence, monitoring, and readiness.
- `PLATFORM_CONTAINMENT`: smallest trustworthy harmful scope restricted.
- `PLATFORM_SAFE`: nonessential work suspended; minimum trusted control plane preserved.
- `PLATFORM_RECOVERY_GUARD`: progressive verified restoration under continuing protection.

Time, restart, silence, or self-attestation SHALL NOT restore `PLATFORM_NORMAL`.

## 8. Triggers and Evidence

Triggers include crash loops, deadlock, exhaustion, message storms, invalid traffic, corrupted state, persistence or evidence failure, unauthorized modification or authority use, compromised components, isolation failure, recovery failure, cascading risk, and severe technical uncertainty.

FFG MAY consume evidence from FSA, Health Monitoring, Runtime, Lifecycle, Resources, Service Bus, FIL, Security, Persistence, AUT-001, audit, watchdogs, and authorized Application Guardian summaries.

FFG SHALL NOT rely exclusively on itself, the actor being restricted, or the requesting Application Guardian.

## 9. Protective Actions

FFG MAY warn, increase monitoring, restrict change, throttle, quarantine, isolate, suspend, preserve, prioritize, request failover/restart/rollback/restore, enter a Platform mode, hold recovery, block release, or request emergency termination.

Every directive SHALL be authorized, scoped, attributable, bounded, integrity-protected, persistent where required, and executed by its competent owner.

## 10. Application Guardian Requests

FFG SHALL receive requests only through CON-022 or an Approved successor.

For every request FFG SHALL independently validate identity, authority, integrity, evidence, technical effect, suspected source, dependencies, criticality, feasibility, proportionality, reversibility, conflicts, and current Platform condition.

FFG MAY reject, investigate, request evidence, accept, narrow, strengthen, or apply provisional containment.

Only FFG may impose cross-Application technical isolation or a Platform protective mode.

## 11. Provisional Containment

Provisional containment requires explicit pre-authorization, severe-delay consequence, smallest trustworthy scope, evidence, audit, reversibility where possible, and expiry or mandatory review.

Review delay SHALL NOT make provisional containment permanent.

## 12. Relationships

- **FSA:** investigates, diagnoses, verifies, and performs separately authorized bounded repair; it does not release FFG restrictions.
- **AUT-001:** validates FFG and requester authority; loss limits FFG to pre-authorized fail-safe actions.
- **Runtime/Lifecycle/Resources/Service Bus/FIL/Security/Persistence/Recovery:** execute only their owned technical actions.
- **Application Guardians:** own their domain restrictions and may request, but not command, cross-Application action.

## 13. Persistence and Release

Restrictions SHALL survive relevant process, component, Application, Guardian, Runtime, and Foundation restart or failover.

Release requires resolved/contained trigger evidence, FSA technical verification where applicable, independent evidence required by consequence, restored authority, successful recovery checks, and competent release authority.

Platform release does not release an Application-domain restriction.

## 14. Failure and Compromise

FFG loss is material protection loss. Dependent activity SHALL reduce or cease. Existing restrictions and independent stop controls remain.

A compromised FFG SHALL be isolatable and unable to release its own restrictions, erase history, expand authority, or disable independent protection.

High availability, stop channel, quorum, and maximum autonomous duration require separate Approved decisions.

## 15. Normative Requirements

- **AUT-002-v2.1-REQ-001:** FFG SHALL protect Foundation technically without business interpretation.
- **AUT-002-v2.1-REQ-002:** FFG SHALL own Platform modes and cross-Application technical isolation within mandate.
- **AUT-002-v2.1-REQ-003:** Application Guardians SHALL request cross-Application protection only through a governed Contract.
- **AUT-002-v2.1-REQ-004:** FFG SHALL independently evaluate every Application Guardian request.
- **AUT-002-v2.1-REQ-005:** FFG MAY reject, narrow, strengthen, investigate, or accept a request.
- **AUT-002-v2.1-REQ-006:** FFG SHALL prefer the smallest trustworthy protective scope.
- **AUT-002-v2.1-REQ-007:** FFG SHALL use governed technical criticality, never hidden business judgment.
- **AUT-002-v2.1-REQ-008:** FFG directives SHALL NOT transfer execution ownership.
- **AUT-002-v2.1-REQ-009:** FFG SHALL preserve restrictions and evidence across restart and failover.
- **AUT-002-v2.1-REQ-010:** FFG SHALL NOT infer recovery from time, silence, restart, or self-report.
- **AUT-002-v2.1-REQ-011:** FFG and FSA SHALL remain mutually observable but not mutually self-validating.
- **AUT-002-v2.1-REQ-012:** FFG SHALL remain independently interruptible, isolatable, and auditable.

## 16. Acceptance Evidence

Acceptance requires all FFG cases in VPL-GDN-002, including unsupported-request rejection, narrow/strong response, cross-Application isolation, payload opacity, restriction persistence, conflicting-request treatment, FSA investigation, and compromised-FFG containment.

## 17. Unresolved Matters

Technical-criticality catalog, survival set, trigger matrix, release matrix, high availability, stop channel, autonomous-duration ceiling, irreversible-action quorum, and manifest Contracts remain unresolved.
