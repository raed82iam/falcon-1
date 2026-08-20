# AUT-001 — Authority Engine

**Version:** 1.0  
**Status:** Approved  
**Effective Date:** 2026-07-24  
**Approval Record:** GOV-003
**Owner:** Falcon Governance Authority  
**Governing Authority:** Constitution Articles 3–5, 16, 19, 24, 26–27, 30, 39–44  
**Affected Domains:** All

## 1. Purpose

The Authority Engine determines whether a governed actor is permitted to perform a governed action under the currently effective authority, constraints, and operating condition.

It is the operational interpreter of approved authority. It is not the source of that authority.

## 2. Scope

AUT-001 governs:

- actor and authority identity;
- permission evaluation;
- scope, condition, and duration of delegated authority;
- denial and revocation;
- conflict resolution between subordinate policies;
- authorization evidence; and
- operation under incomplete or untrustworthy authority data.

## 3. Non-Scope

The Authority Engine SHALL NOT:

- create constitutional or financial policy;
- approve its own permissions;
- execute the requested action;
- infer permission from technical capability;
- convert recommendation into authorization;
- resolve conflicts with the Vision or Constitution in favor of lower authority; or
- grant permanent authority through repeated use.

## 4. Decision Model

An authority decision shall resolve:

- **Actor:** who or what requests authority;
- **Action:** the governed act requested;
- **Resource:** the subject affected;
- **Purpose:** the legitimate objective served;
- **Scope:** the permitted boundary;
- **Conditions:** facts that must be true;
- **Constraints:** limits that remain binding;
- **Duration:** when authority begins and ends; and
- **Provenance:** the higher authority from which permission derives.

## 5. Normative Requirements

- **AUT-001-REQ-001:** Every material authority decision SHALL be attributable to an authenticated actor and an authoritative policy baseline.
- **AUT-001-REQ-002:** The Authority Engine SHALL deny authority that has no traceable source.
- **AUT-001-REQ-003:** The Authority Engine SHALL apply the highest controlling authority when applicable rules conflict.
- **AUT-001-REQ-004:** A lower authority SHALL NOT waive or weaken a higher constraint.
- **AUT-001-REQ-005:** Permission SHALL be limited to the narrowest scope, duration, and capability sufficient for the declared purpose.
- **AUT-001-REQ-006:** Authority SHALL expire or become invalid when its purpose ends, its conditions fail, its source is withdrawn, or its time limit is reached.
- **AUT-001-REQ-007:** Denial SHALL be the default when actor identity, policy integrity, applicable authority, or material conditions cannot be established.
- **AUT-001-REQ-008:** Emergency authority SHALL remain bounded by non-waivable constitutional and protective constraints.
- **AUT-001-REQ-009:** The Authority Engine SHALL support immediate revocation of material delegated authority.
- **AUT-001-REQ-010:** A recommendation, prediction, lifecycle state, or successful prior action SHALL NOT be treated as permission.
- **AUT-001-REQ-011:** Every material authorization result SHALL record decision, basis, policy version, conditions, and reason.
- **AUT-001-REQ-012:** Authorization evidence SHALL be immutable after the decision is issued.
- **AUT-001-REQ-013:** The Authority Engine SHALL detect and reject attempts to authorize itself or modify its governing authority through ordinary requests.
- **AUT-001-REQ-014:** Protective restrictions issued lawfully by Guardian SHALL constrain authorization until revoked by legitimate authority.
- **AUT-001-REQ-015:** Authority evaluation SHALL be deterministic for the same trusted inputs and effective policy baseline.

## 6. Failure and Degraded Behavior

Failure to establish trustworthy authority SHALL result in denial, not optimistic continuation.

Cached authority MAY be used only where an approved policy explicitly defines its scope, maximum age, revocation behavior, and acceptable risk.

## 7. Acceptance Evidence

Approval requires evidence that:

- conflicting lower policies cannot override higher constraints;
- expired and revoked permissions are rejected;
- missing provenance results in denial;
- the Engine cannot grant itself authority;
- identical trusted inputs produce identical results; and
- every material result is reconstructable from preserved evidence.

## 8. ADR Candidates

- Policy representation and evaluation model;
- distribution and consistency of authority state;
- revocation propagation mechanism; and
- isolation boundary for the Authority Engine.

## 9. Unresolved Matters

- Formal catalog of non-waivable constraints.
- Maximum acceptable authorization latency by consequence class.

