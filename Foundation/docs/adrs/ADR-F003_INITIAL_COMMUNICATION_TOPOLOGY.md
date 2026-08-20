# ADR-F003 — Initial Communication Topology

**Identifier:** ADR-F003  
**Version:** 1.0  
**Status:** Accepted  
**Date:** 2026-07-24  
**Decision Owner:** Falcon Project Owner  
**Scope:** Governed communication topology for FRS-001  
**Affected Specifications:** SYS-005, SYS-009, SYS-010, FRS-001  
**Applicable Standards:** STD-003  
**Related ADRs:** ADR-F001, ADR-F002, ADR-F004, ADR-F006  
**Supersedes:** None  
**Superseded By:** None  
**Decision Record:** Project Owner approval recorded on 2026-07-24

## 1. Context

Falcon Foundation components must cooperate without creating hidden dependencies, implicit authority, or untraceable side effects. Unrestricted direct communication would couple component identities and lifecycles, weaken isolation, and make replacement, restriction, and reconstruction unreliable.

The Foundation Release requires an initial topology that proves governed communication while remaining non-distributed and independent of a particular transport product.

## 2. Decision Drivers

- preserve the isolation established by ADR-F001;
- preserve authoritative ownership established by ADR-F002;
- make material interactions identifiable, authorized, and reconstructable;
- distinguish requests, information, responses, and established facts;
- prevent communication infrastructure from becoming a decision authority;
- support controlled replacement and future plug-and-play capabilities;
- contain delivery failure and congestion; and
- avoid premature distributed-operation or scale claims.

## 3. Higher-Authority Constraints

This decision is constrained by:

- the Vision priority of capital protection and disciplined operation;
- constitutional requirements for explicit authority, traceability, bounded failure, and governed evolution;
- SYS-005 requirements for authorized transport, preserved message integrity, truthful delivery status, and failure containment;
- SYS-009 requirements for canonical message kinds, identity, provenance, compatibility, and separation of messaging from authority;
- SYS-010 requirements for authoritative, immutable facts and safe replay; and
- FRS-001 requirements for valid FIL communication, immutable events, Guardian restriction, and evidence reconstruction.

## 4. Alternatives Considered

### 4.1 Unrestricted direct connections

Components could select their own communication paths and formats.

This was rejected because dependencies and authority crossings would become hidden, replacement would become unsafe, and evidence would be incomplete.

### 4.2 Communication only through a universal controlling component

One component could interpret and direct every interaction.

This was rejected because transport would become a system-wide decision authority and an excessive concentration of responsibility.

### 4.3 Governed mediated communication across boundaries

Material communication across governed component or isolation boundaries uses FIL through the Service Bus. Local interaction wholly inside one declared boundary may remain direct where no authority or isolation boundary is crossed.

This alternative was selected because it provides traceability, policy enforcement, and replaceability without assigning business or state authority to the transport.

## 5. Decision

FRS-001 SHALL use a logically mediated communication topology for every material interaction that crosses a governed component or isolation boundary.

Such interactions SHALL use FIL messages admitted and transported through the governed Service Bus. Undeclared side channels that bypass identity, authorization, validation, classification, routing, or evidence controls are prohibited.

Communication semantics SHALL remain explicit:

- commands request an authorized state owner to attempt an action;
- queries request information without authority to change state;
- responses report the result of a named request;
- events publish immutable facts under the authority of their fact owner; and
- notices carry non-authoritative information.

Commands SHALL target the responsible authority or declared destination. Events may be distributed to authorized subscribers without granting them authority to act. Acknowledgment, transport delivery, acceptance, execution, persistence, and successful outcome SHALL remain distinct.

The Service Bus SHALL transport and enforce communication policy but SHALL NOT decide business meaning, own communicated state, create authority, or alter payload meaning.

Direct interaction MAY occur wholly within one declared component and one isolation boundary when no governed boundary is crossed and the interaction does not evade required evidence or authority enforcement.

The initial Foundation topology is logically unified and non-distributed. This decision does not select a transport product, serialization format, physical process layout, queue technology, or future federation model.

## 6. Consequences

- Material cross-boundary communication becomes visible and governable.
- Components depend on approved contracts rather than hidden internal connections.
- Communication can be denied, traced, expired, retried, or contained according to declared rules.
- Components can be replaced while preserving their governed interaction contracts.
- The communication path does not become the owner of decisions or state.
- FRS-001 must demonstrate delivery uncertainty and failure truthfully.
- The Service Bus becomes essential Foundation infrastructure and must enter a protective degraded condition when trustworthy communication cannot be maintained.

## 7. Risks and Mitigations

- **Risk:** The mediated path could become a single point of failure.  
  **Mitigation:** Require truthful degradation, failure containment, protective-message behavior, and safe restriction; do not claim high availability in FRS-001.

- **Risk:** The Service Bus could accumulate business logic.  
  **Mitigation:** Restrict it to admission, policy enforcement, routing, delivery, flow control, and transport evidence.

- **Risk:** Acknowledgment could be mistaken for successful execution.  
  **Mitigation:** Preserve distinct states for admission, delivery, acceptance, execution, persistence, and outcome.

- **Risk:** Direct calls could become hidden boundary crossings.  
  **Mitigation:** Permit them only inside one declared component and isolation boundary, subject to review.

- **Risk:** Message replay or duplication could repeat a harmful action.  
  **Mitigation:** Preserve logical identity, authorization, expiry, replay controls, and idempotency expectations.

## 8. Compatibility and Transition

No prior Falcon1 ADR is superseded.

All FRS-001 cross-boundary interactions shall be cataloged as FIL contracts before implementation authorization. Any existing design that uses an undeclared cross-component side channel must route the interaction through the governed topology or obtain a later superseding ADR.

Future federated or distributed topology requires a separate ADR and shall preserve the semantic and authority boundaries established here.

## 9. Conformance Evidence

Conformance shall be demonstrated by evidence that:

- unauthorized publication, subscription, command, and query attempts are rejected;
- malformed, expired, unsupported, or integrity-failed messages do not reach governed handling;
- message identity, producer, correlation, causation, and classification survive transport;
- transport delivery is not represented as execution success;
- a command cannot bypass its authoritative state owner;
- an event cannot be published without authority to assert its fact;
- an undeliverable message cannot disappear silently;
- failure of one route or consumer is contained from unrelated communication; and
- the FRS-001 scenarios can be reconstructed across the communication chain.

## 10. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner and current Constitutional Authority | Accepted | رائد عموره — “موافق على القرار الثالث” | 2026-07-24 |
