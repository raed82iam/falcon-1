# RSK-005 — Capital Safety Plane

**Identifier:** RSK-005  
**Version:** 1.0  
**Status:** Approved  
**Effective Date:** 2026-07-24  
**Approval Record:** GOV-003
**Owner:** Falcon Capital Protection Authority  
**Governing Authority:** Falcon Vision; Constitution Articles 7–10, 22–24, 34, 36B  
**Affected Domains:** CAP, RSK, DEC, AWR, AUT, FIN, EVO, SEC, OPS

## 1. Purpose

The Capital Safety Plane provides independently enforceable limits that prevent unacceptable capital exposure when intelligence, decision, execution, maintenance, or evolution capabilities fail or act outside authorized bounds.

## 2. Scope

RSK-005 governs:

- non-waivable capital safety limits;
- exposure admission;
- concentration, leverage, liquidity, and loss constraints;
- data and fitness prerequisites;
- independent protective enforcement;
- safe-state action;
- external safeguards; and
- limit-change authority and evidence.

## 3. Non-Scope

The Capital Safety Plane does not:

- select strategies;
- optimize return;
- generate financial recommendations;
- define all portfolio policy;
- accept risk outside its delegated mandate;
- modify its own upper limits; or
- replace Guardian, Risk evaluation, or Authority Engine.

## 4. Normative Requirements

- **RSK-005-REQ-001:** Every capital-affecting action SHALL be evaluated against currently effective safety limits before execution.
- **RSK-005-REQ-002:** Safety limits SHALL be independently enforceable from the intelligence or strategy requesting action.
- **RSK-005-REQ-003:** Limit evaluation SHALL consider current and resulting aggregate exposure, not the requested action in isolation.
- **RSK-005-REQ-004:** Required data quality, Fitness to Operate, authority, and trust conditions SHALL be treated as safety prerequisites.
- **RSK-005-REQ-005:** Missing or untrustworthy prerequisite evidence SHALL result in denial or a defined protective action.
- **RSK-005-REQ-006:** No expected return, model confidence, or prior success SHALL override a non-waivable limit.
- **RSK-005-REQ-007:** Safety limits SHALL have an accountable owner, authority source, version, effective time, scope, and change record.
- **RSK-005-REQ-008:** The Plane SHALL NOT approve a change to its own upper protective authority.
- **RSK-005-REQ-009:** Limit changes SHALL require independence and evidence proportionate to the maximum additional harm enabled.
- **RSK-005-REQ-010:** Rejected actions SHALL record the controlling limit and material evaluation evidence.
- **RSK-005-REQ-011:** The Plane SHALL remain effective during degraded decision and evolution capability to the required degree.
- **RSK-005-REQ-012:** Guardian SHALL be able to impose stricter temporary protection but SHALL NOT weaken an upper safety limit.
- **RSK-005-REQ-013:** Protective actions for open positions SHALL account for the risk of inaction, forced action, market liquidity, and partial execution.
- **RSK-005-REQ-014:** Falcon SHALL support safeguards outside the primary Falcon trust boundary where total internal compromise could create catastrophic loss.
- **RSK-005-REQ-015:** Safety-control failure SHALL reduce or suspend capital-affecting authority.
- **RSK-005-REQ-016:** Every material safety decision SHALL be attributable and reconstructable.

## 5. Invariants

1. Capital protection limits are not performance preferences.
2. A producer cannot approve the limit governing its own request.
3. Unknown aggregate exposure cannot be treated as zero.
4. Internal availability does not outrank external capital safety.

## 6. Acceptance Evidence

Approval requires evidence for:

- aggregate exposure enforcement;
- prerequisite failure behavior;
- independent denial under strategy or model pressure;
- inability to self-weaken limits;
- degraded-mode protection;
- open-position protective behavior; and
- independent external safeguard activation.

## 7. ADR Candidates

- Enforcement boundary;
- broker- or venue-side safeguards;
- limit distribution and consistency;
- exposure aggregation mechanism; and
- independent stop channel.

## 8. Unresolved Matters

- Capital risk taxonomy and upper-limit catalog.
- Jurisdiction and institution-specific external protection.
