# AUT-002 — Guardian

**Identifier:** AUT-002  
**Version:** 1.0  
**Status:** Approved  
**Effective Date:** 2026-07-24  
**Approval Record:** GOV-003
**Owner:** Falcon Protection Authority  
**Governing Authority:** Constitution Articles 7–8, 15, 18, 22–24, 26, 30, 34, 41–43  
**Affected Domains:** All

## 1. Purpose

Guardian is Falcon’s bounded operational protection authority.

Guardian detects or receives evidence of conditions that may threaten capital, constitutional invariants, integrity, or safe continuity, and imposes proportionate protective restrictions.

Guardian is not the Constitution, the highest universal authority, or the owner of routine operation.

## 2. Scope

AUT-002 governs:

- protective condition evaluation;
- warning, restriction, isolation, suspension, and emergency stop;
- protective operating modes;
- escalation;
- containment coordination;
- conditions for lifting restrictions; and
- evidence of protective action.

## 3. Non-Scope

Guardian SHALL NOT:

- trade, allocate capital, or select strategy;
- create constitutional policy;
- grant itself new authority;
- alter evidence to justify intervention;
- own ordinary lifecycle or recovery execution;
- declare recovery complete without required evidence; or
- use emergency power for performance optimization.

## 4. Protective Modes

The minimum mode model SHALL distinguish:

`NORMAL`, `HEIGHTENED`, `RESTRICTED`, `SAFE`, and `RECOVERY_GUARD`.

Mode names may be realized differently by ADR, but their protective semantics SHALL remain distinguishable.

## 5. Normative Requirements

- **AUT-002-REQ-001:** Guardian authority SHALL derive from explicit approved policy and AUT-001.
- **AUT-002-REQ-002:** Guardian SHALL act only within declared protective scope.
- **AUT-002-REQ-003:** Protective action SHALL be proportionate to credible harm, uncertainty, and reversibility.
- **AUT-002-REQ-004:** Guardian SHALL prefer containment that preserves unaffected safe operation when such isolation is trustworthy.
- **AUT-002-REQ-005:** Guardian SHALL be able to restrict, isolate, suspend, or request termination of governed activity within its mandate.
- **AUT-002-REQ-006:** Guardian SHALL be able to revoke or constrain delegated operational authority through AUT-001.
- **AUT-002-REQ-007:** Guardian SHALL NOT depend exclusively on evidence produced by the actor it may restrict.
- **AUT-002-REQ-008:** Severe conditions with insufficient knowledge SHALL favor protection over continued activity.
- **AUT-002-REQ-009:** Every intervention SHALL record trigger, evidence, authority, scope, action, and expected release conditions.
- **AUT-002-REQ-010:** Failure to intervene when a mandatory threshold is met SHALL be observable as a protection failure.
- **AUT-002-REQ-011:** Guardian action SHALL NOT erase the evidence that caused it.
- **AUT-002-REQ-012:** Protective restrictions SHALL persist across restart where their risk remains unresolved.
- **AUT-002-REQ-013:** Release from restriction SHALL require authorized evidence that the triggering condition is resolved or acceptably contained.
- **AUT-002-REQ-014:** Guardian SHALL remain subject to independent oversight, audit, interruption, and correction.
- **AUT-002-REQ-015:** A compromised Guardian SHALL be isolatable without silently removing all independent protection.

## 6. Invariants

1. Guardian protects; it does not pursue profit.
2. Guardian restricts authority; it does not invent authority.
3. Emergency scope ends when its lawful conditions end.
4. `NORMAL` is not restored by passage of time alone.

## 7. Failure and Degraded Behavior

Loss of Guardian capability SHALL be treated as loss of a material protection. Activities dependent on Guardian protection SHALL reduce or cease according to governing risk policy.

Guardian uncertainty SHALL not be concealed as `NORMAL`.

## 8. Acceptance Evidence

Approval requires evidence for:

- mandatory intervention at defined thresholds;
- proportionate scope and unaffected-domain isolation;
- persistence of unresolved restrictions;
- denial of unauthorized release;
- full intervention reconstruction;
- Guardian compromise containment; and
- operation under lost or contradictory health evidence.

## 9. ADR Candidates

- Guardian isolation boundary;
- rule evaluation model;
- high-availability strategy;
- protective mode realization; and
- independent stop channel.

## 10. Unresolved Matters

- Formal protective mandate matrix.
- Ratifying authority for release from each consequence class.
