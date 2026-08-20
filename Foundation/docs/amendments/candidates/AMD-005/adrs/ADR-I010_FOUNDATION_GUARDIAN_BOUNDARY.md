# ADR-I010 — Foundation Guardian Boundary

**Identifier:** ADR-I010  
**Version:** Proposed 1.0  
**Status:** Accepted — Documentary Activation Deferred  
**Date:** 2026-07-27  
**Decision Owner:** Falcon Project Owner  
**Affected Specifications:** AUT-001, AUT-002, AWR-001, OPS-003, SYS-001, SYS-002, SEC-001  
**Affected Contracts:** CON-006, CON-011  
**Stage 1 Authority:** Not Granted
**Decision Record:** GOV-060

## 1. Context

Falcon requires a technical protection authority capable of containing faults, preserving the Foundation control plane, and governing Platform Safe Mode. It also requires business-domain protection capable of protecting capital and financial state.

Combining both meanings in one undifferentiated Guardian causes Foundation to require business knowledge and creates an excessively broad authority.

## 2. Decision

Falcon SHALL separate:

- **Falcon Foundation Guardian (FFG):** Foundation technical protection authority; and
- **Application Guardian:** a future, separately specified Application authority for business and domain protection.

FFG SHALL own binding Foundation protective restrictions and Platform Safe Mode within its approved mandate.

FFG SHALL understand technical effect, dependency, criticality, resource pressure, communication condition, runtime state, and recovery readiness. It SHALL NOT interpret Application business purpose, financial state, customer state, strategies, orders, positions, portfolios, or business payload meaning.

An Application authority MAY later request technical protection through a governed abstract request. FFG SHALL evaluate only the requested technical treatment, its authority, feasibility, conflicts, and Foundation safety.

## 3. Responsibility Separation

- FSA observes, correlates, diagnoses, verifies, and conducts separately authorized bounded repair.
- FFG evaluates protection conditions, issues restrictions, governs Platform modes, and controls restriction release.
- AUT-001 validates authority and applies resulting authority constraints.
- Runtime, Lifecycle, Resources, Security, Service Bus, FIL, Persistence, and Recovery execute only the technical operations they own.
- A future Application Guardian owns only the business-protection jurisdiction explicitly granted to it.

No participant SHALL rewrite another participant’s authoritative fact.

## 4. Authority Rules

FFG authority SHALL:

- originate in Approved governance and a valid mandate;
- be scoped by target, action, consequence, duration, and protection condition;
- be independently enforceable where the consequence requires it;
- remain attributable and reconstructable;
- fail toward the smallest trustworthy protective state;
- survive restart and failover while unresolved; and
- end or require renewed authority when its lawful condition or authorized duration ends.

FFG SHALL NOT create jurisdiction, approve its own expansion, remove independent oversight, or use emergency power for routine optimization.

## 5. Mode Decision

Foundation protection SHALL distinguish:

- `PLATFORM_NORMAL`;
- `PLATFORM_HEIGHTENED`;
- `PLATFORM_CONTAINMENT`;
- `PLATFORM_SAFE`; and
- `PLATFORM_RECOVERY_GUARD`.

Containment SHALL be preferred when its boundary is trustworthy. Platform Safe Mode SHALL be preferred when narrower containment cannot be trusted.

Return to normal SHALL require authorized evidence. Time, restart, silence, or self-attestation SHALL NOT establish recovery.

## 6. Mutual Protection

FSA MAY supervise FFG technical readiness and restore only Approved trusted FFG state under an Approved repair playbook.

FFG MAY restrict or isolate an unsafe FSA instance only under explicit mandate and credible evidence independent of the affected instance.

Neither FSA nor FFG may be the sole conclusive authority over a material challenge to its own conduct, readiness, restriction, or release.

## 7. Consequences

- Approved `AUT-002 v1.0` requires an explicit successor decision.
- Foundation no longer interprets capital or business meaning through Guardian.
- Application protection requires a later independent specification and authority charter.
- Technical criticality, survival set, trigger thresholds, high availability, stop channel, release authority, and cross-boundary request semantics require governed supporting artifacts.
- Existing enforcement principles in `ADR-F008` remain applicable unless explicitly superseded.

## 8. Rejected Alternatives

### One Guardian with unrestricted technical and business knowledge

Rejected because it violates separation of concerns and creates excessive authority.

### FSA owns protection and repair

Rejected because diagnosis, repair, restriction, and release would become self-validating.

### Every Application protects only itself

Rejected because an Application cannot authoritatively protect shared Foundation resources or other Applications.

### Foundation-wide shutdown for every severe condition

Rejected because trustworthy isolation should preserve unaffected higher-priority technical operation.

## 9. Required Follow-on Decisions

Separate ADRs or governed catalogs SHALL define:

- technical criticality;
- Platform Safe Mode survival set;
- mandatory trigger matrix;
- isolation boundaries;
- FFG high availability;
- independent stop channel;
- restriction persistence;
- consequence classes and release authority;
- maximum autonomous containment duration;
- quorum for irreversible action; and
- Application-to-Foundation protection requests.

## 10. Activation

Approval of this ADR approves the architectural boundary only. It does not activate Guardian, supersede Approved documents by itself, authorize Stage 1, or permit implementation, deployment, cloud operation, or financial activity.
