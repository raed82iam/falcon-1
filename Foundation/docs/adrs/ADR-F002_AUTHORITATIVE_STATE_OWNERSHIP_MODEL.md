# ADR-F002 — Authoritative State Ownership Model

**Identifier:** ADR-F002  
**Version:** 1.0  
**Status:** Accepted  
**Date:** 2026-07-24  
**Decision Owner:** Falcon Project Owner  
**Scope:** Ownership and change of authoritative state within FRS-001  
**Affected Specifications:** SYS-002, SYS-011, DEC-006, FRS-001  
**Applicable Standards:** STD-003  
**Related ADRs:** ADR-F001, ADR-F003, ADR-F005  
**Supersedes:** None  
**Superseded By:** None  
**Decision Record:** Project Owner approval recorded on 2026-07-24

## 1. Context

Falcon must remain able to identify which state is true, who is accountable for it, and how it changed. If multiple components can independently declare the same governed fact, failures, delayed information, recovery, or concurrent actions can create competing truths.

The Foundation Release must therefore establish authoritative ownership before selecting communication or persistence mechanisms.

## 2. Decision Drivers

- maintain one unambiguous source for each governed fact;
- prevent unauthorized or conflicting state changes;
- preserve accountability and reconstructability;
- support failure recovery without inventing state;
- keep storage, caches, reports, and observations from acquiring authority implicitly;
- enable replaceable components with explicit ownership boundaries; and
- avoid creating one universal owner for unrelated state.

## 3. Higher-Authority Constraints

This decision is constrained by:

- the Vision priority of capital protection and disciplined operation;
- constitutional requirements for bounded authority, attributable decisions, truthful state, evidence, and controlled recovery;
- SYS-002 requirements for exactly one authoritative lifecycle state and authorized transitions;
- SYS-011 requirements for one accountable owner and one identified authoritative source for every durable data set;
- DEC-006 requirements to distinguish proposals, decisions, authorization, execution, outcomes, and later evaluation; and
- FRS-001 invariants requiring attributable authority, valid state transitions, and complete reconstruction.

## 4. Alternatives Considered

### 4.1 Shared mutable ownership

Multiple components could directly change the same governed state.

This was rejected because accountability becomes ambiguous and concurrent or failed changes can create conflicting truths.

### 4.2 One universal state owner

A single authority could own all state in Falcon.

This was rejected because it would combine unrelated responsibilities, enlarge the trusted boundary, create excessive concentration, and reduce independent evolution.

### 4.3 Single authoritative owner per state class

Each governed state class has one declared authoritative owner. Other actors submit requests, observations, or evidence but cannot independently redefine the owned fact.

This alternative was selected because it creates clear truth and accountability without centralizing all Falcon state in one authority.

## 5. Decision

Every governed state class in Falcon SHALL have exactly one declared authoritative owner and one identified authoritative source at any effective time.

Only the authoritative owner may accept a change to the state it owns. Other actors may request change, provide evidence, publish observations, or maintain derived views, but SHALL NOT represent those outputs as authoritative state.

An accepted state change SHALL be authorized, validated against the applicable prior state and rules, attributable to its requester and authority, and preserved with sufficient evidence for reconstruction.

Commands, proposals, events, logs, caches, replicas, reports, and stored copies SHALL NOT acquire semantic authority merely because they contain or transport state. Persistence preserves authoritative state but does not create ownership or authority.

Derived or replicated state SHALL identify its authoritative source, version or causal position, and freshness where material. When it conflicts with, cannot reach, or cannot verify its source, it SHALL be treated as stale, uncertain, or unavailable rather than promoted to a competing truth.

After failure or restoration, Falcon SHALL reconcile state with the declared authoritative source and preserved evidence. A component SHALL NOT approve its own recovery by asserting an unverified replacement state.

Ownership SHALL be distributed by responsibility. This decision does not create one universal state authority and does not select a storage technology, consistency mechanism, or communication topology.

## 6. Consequences

- Every governed fact has a clear accountable owner.
- Conflicting components cannot legitimately create competing truth.
- State changes become explicit requests and accepted transitions.
- Recovery can determine what must be restored and independently validated.
- Caches and read models remain replaceable because they do not own the underlying fact.
- Each future capability must declare the state it owns, reads, derives, and requests to change.
- Temporary unavailability may be required when the authoritative source cannot be trusted; Falcon may not substitute fabricated certainty.

## 7. Risks and Mitigations

- **Risk:** An authoritative owner may become unavailable.  
  **Mitigation:** Preserve evidence and recovery paths; restrict affected actions while authority or integrity is uncertain.

- **Risk:** Ownership may be defined too broadly and create a hidden central authority.  
  **Mitigation:** Assign ownership by coherent state class and minimum necessary responsibility.

- **Risk:** A cache or replica may be mistaken for current truth.  
  **Mitigation:** Require source identity, freshness, and explicit non-authoritative status for derived state.

- **Risk:** Recovery may restore an older but internally valid state.  
  **Mitigation:** Reconcile version, causal position, integrity, and evidence before authority is restored.

- **Risk:** Self-maintenance or intelligence may attempt to change state outside its authority.  
  **Mitigation:** Treat their output as a proposal or request unless the governing baseline explicitly grants the required ownership and authority.

## 8. Compatibility and Transition

No prior Falcon1 ADR is superseded.

Every FRS-001 state field and contract shall identify its authoritative owner before implementation authorization. Any existing design that permits multiple writers to independently declare the same governed fact must be revised or explicitly separated into distinct state classes.

Later ADRs may select consistency, communication, and persistence mechanisms but shall preserve the ownership meaning established here.

## 9. Conformance Evidence

Conformance shall be demonstrated by evidence that:

- every FRS-001 governed state class has one declared owner and authoritative source;
- unauthorized and invalid state changes are rejected;
- simultaneous requests cannot create two authoritative outcomes;
- stale or unavailable replicas do not become authoritative;
- lifecycle state remains singular through failure and recovery;
- restored state is reconciled and independently validated;
- every accepted change is attributable and reconstructable; and
- self-maintenance cannot modify state outside its granted authority.

## 10. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner and current Constitutional Authority | Accepted | رائد عموره — “موافق على القرار الثاني” | 2026-07-24 |
