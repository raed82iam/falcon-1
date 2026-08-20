# SYS-001 — Kernel

**Identifier:** SYS-001  
**Version:** 1.0  
**Status:** Approved  
**Effective Date:** 2026-07-24  
**Approval Record:** GOV-003
**Owner:** Falcon Core Authority  
**Governing Authority:** Falcon Vision; Constitution Articles 6, 11, 18, 35–40  
**Affected Domains:** AUT, SYS, SEC, OPS

## 1. Purpose

The Kernel is the minimal operating authority that establishes and preserves Falcon’s Core boundary and invariants.

It enables governed Core components to exist and cooperate without becoming a financial decision-maker or a container for unrelated shared functionality.

## 2. Scope

SYS-001 governs:

- Kernel identity and boundary;
- Core admission and registration;
- Core startup and shutdown coordination;
- preservation of Core invariants;
- access to governed Core authorities; and
- Kernel behavior under degraded conditions.

## 3. Non-Scope

The Kernel does not own:

- financial decisions or capital allocation;
- business workflows;
- risk acceptance;
- message semantics;
- general persistence;
- operational policy;
- autonomous learning; or
- component-specific recovery decisions.

## 4. Terms

- **Core invariant:** a condition that must remain true for Falcon Core operation to remain valid.
- **Core component:** a component formally admitted under this collection.
- **Kernel authority:** the minimum authority required to establish and preserve the Core operating boundary.

## 5. Normative Requirements

- **SYS-001-REQ-001:** The Kernel SHALL remain subordinate to the Vision, Constitution, and legitimate governance.
- **SYS-001-REQ-002:** The Kernel SHALL expose only the authority necessary to establish and preserve Core operation.
- **SYS-001-REQ-003:** The Kernel SHALL NOT contain financial, market, portfolio, strategy, or application policy.
- **SYS-001-REQ-004:** The Kernel SHALL admit only components with a registered identity, declared owner, approved authority, and valid lifecycle definition.
- **SYS-001-REQ-005:** The Kernel SHALL maintain a distinguishable operating identity for every admitted Core component.
- **SYS-001-REQ-006:** The Kernel SHALL prevent a Core component from acquiring undeclared authority through registration, execution, or dependency.
- **SYS-001-REQ-007:** The Kernel SHALL coordinate Core startup and shutdown through SYS-002.
- **SYS-001-REQ-008:** The Kernel SHALL obtain authorization for governed actions through AUT-001 and SHALL NOT reproduce authorization policy locally.
- **SYS-001-REQ-009:** The Kernel SHALL remain capable of entering a restricted condition in which nonessential Core activity is denied.
- **SYS-001-REQ-010:** The Kernel SHALL preserve protective controls during partial failure whenever technically possible.
- **SYS-001-REQ-011:** The Kernel SHALL expose sufficient evidence for health, audit, and constitutional review.
- **SYS-001-REQ-012:** Failure of a nonessential Core component SHALL NOT automatically invalidate unrelated Core components.
- **SYS-001-REQ-013:** The Kernel SHALL reject operation when its own identity, governing baseline, or essential authority cannot be established reliably.
- **SYS-001-REQ-014:** Kernel expansion SHALL require evidence that the responsibility is foundational and cannot be governed safely outside the Kernel.

## 6. Invariants

1. No Kernel action is above constitutional authority.
2. No component executes as a Core component without admission.
3. No Core authority exists without an accountable owner.
4. Protective restriction remains possible whenever continued operation is possible.

## 7. Failure and Degraded Behavior

When an essential Kernel invariant fails, the Kernel SHALL:

1. prevent new nonessential activity;
2. preserve available evidence;
3. notify Guardian and Health Monitoring where communication remains trustworthy;
4. enter the safest valid condition available; and
5. refuse unrestricted operation until recovery is independently validated.

## 8. Acceptance Evidence

Approval requires evidence that:

- domain-specific policy cannot enter the Kernel through declared extension paths;
- unregistered components cannot acquire Core authority;
- invalid Core baselines block unrestricted startup;
- nonessential component failure is contained; and
- restricted operation preserves Authority Engine, Security, Guardian, Logging, and Recovery access as applicable.

## 9. ADR Candidates

- Kernel hosting and isolation model;
- Core component discovery mechanism;
- process and deployment boundaries; and
- strategy for minimal trusted computing scope.

## 10. Unresolved Matters

- Formal list of essential versus nonessential Core functions.
- Ratifying authority for Core component admission.
