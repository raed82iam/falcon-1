# SYS-002 — Lifecycle

**Identifier:** SYS-002  
**Version:** 1.0  
**Status:** Approved  
**Effective Date:** 2026-07-24  
**Approval Record:** GOV-003
**Owner:** Falcon Core Authority  
**Governing Authority:** Constitution Articles 18, 24, 26, 32–36, 39  
**Affected Domains:** SYS, AUT, OPS, SEC

## 1. Purpose

Lifecycle governs the valid existence and operating state of every Core component.

It ensures that components start, change state, stop, and retire only through authorized, observable, and recoverable transitions.

## 2. Scope

SYS-002 governs:

- lifecycle state definitions;
- transition authorization and validation;
- dependency-aware sequencing;
- startup and shutdown;
- suspension, isolation, and retirement;
- transition evidence; and
- lifecycle behavior during failure and recovery.

## 3. Non-Scope

Lifecycle does not:

- determine financial safety;
- assess health independently;
- create component authority;
- allocate general resources;
- define recovery success;
- replace Guardian; or
- interpret business meaning.

## 4. Canonical States

The minimum lifecycle model shall distinguish:

`REGISTERED`, `INITIALIZING`, `READY`, `RUNNING`, `RESTRICTED`, `SUSPENDED`, `STOPPING`, `STOPPED`, `FAILED`, `RECOVERING`, and `RETIRED`.

An ADR may choose a realization, but SHALL preserve the semantic distinctions required here.

## 5. Normative Requirements

- **SYS-002-REQ-001:** Every Core component SHALL have exactly one authoritative lifecycle state.
- **SYS-002-REQ-002:** Every transition SHALL identify requester, authority, source state, target state, reason, and time.
- **SYS-002-REQ-003:** Lifecycle SHALL reject transitions not permitted by the component’s approved state model.
- **SYS-002-REQ-004:** Lifecycle SHALL obtain authorization through AUT-001 for governed transitions.
- **SYS-002-REQ-005:** A component SHALL NOT report itself as successfully transitioned without Lifecycle confirmation.
- **SYS-002-REQ-006:** Startup SHALL validate identity, configuration, essential dependencies, security conditions, and required authority before `RUNNING`.
- **SYS-002-REQ-007:** Dependency order SHALL be respected without creating circular startup authority.
- **SYS-002-REQ-008:** Shutdown SHALL prefer controlled completion while preserving the right to immediate protective termination.
- **SYS-002-REQ-009:** Repeated failure SHALL NOT create an unbounded restart loop.
- **SYS-002-REQ-010:** Guardian SHALL be able to request authorized restriction, suspension, isolation, or stop.
- **SYS-002-REQ-011:** Recovery transitions SHALL be coordinated with OPS-003 and SHALL NOT return a component to `RUNNING` before recovery validation.
- **SYS-002-REQ-012:** Lifecycle SHALL publish immutable transition facts through SYS-010.
- **SYS-002-REQ-013:** Lifecycle SHALL preserve transition history through OPS-004 and, when durability is required, SYS-011.
- **SYS-002-REQ-014:** Retirement SHALL revoke active authority, prevent new execution, and preserve required records.
- **SYS-002-REQ-015:** Loss of Lifecycle authority SHALL prevent new nonessential transitions.

## 6. Invariants

1. No component occupies two authoritative lifecycle states.
2. No transition occurs without a valid prior state.
3. No recovery completes by self-declaration.
4. `RETIRED` is terminal unless a new identity is admitted.

## 7. Acceptance Evidence

Approval requires evidence for:

- rejection of every invalid state transition;
- dependency-aware startup and reverse-order shutdown;
- bounded restart behavior;
- Guardian-requested protective transitions;
- recovery validation before return to service; and
- complete reconstruction of component transition history.

## 8. ADR Candidates

- State coordination consistency model;
- lifecycle command and event transport;
- process termination strategy; and
- distributed lifecycle ownership.

## 9. Unresolved Matters

- Consequence-based transition timeouts.
- Formal dependency cycle resolution policy.
