# DEC-006 — Decision Ledger

**Identifier:** DEC-006  
**Version:** 1.0  
**Status:** Approved  
**Effective Date:** 2026-07-24  
**Approval Record:** GOV-003
**Owner:** Falcon Decision Authority  
**Governing Authority:** Constitution Articles 13, 16–17, 19–20, 26–28, 31–32  
**Affected Domains:** CAP, RSK, DEC, AWR, INT, AUT, FIN, EVO, SEC

## 1. Purpose

The Decision Ledger preserves the evidence, authority, reasoning, execution, outcome, and evaluation of every material Falcon decision.

It enables accountability and learning without confusing a profitable result with a sound decision or a loss with an unsound one.

## 2. Scope

DEC-006 governs:

- decision identity and lineage;
- evidence and data provenance;
- assumptions and uncertainty;
- Self Model and Fitness context;
- alternatives, including non-action;
- risk and capital impact;
- recommendation, decision, approval, and execution authority;
- protective constraints;
- actual execution and outcome;
- later evaluation and learning; and
- historical immutability.

## 3. Non-Scope

The Decision Ledger does not:

- make decisions;
- grant authority;
- replace financial books and records;
- rewrite decisions after outcomes are known;
- require publication of protected information to unauthorized actors; or
- treat stored narrative as proof without linked evidence.

## 4. Normative Requirements

- **DEC-006-REQ-001:** Every material decision SHALL have a globally unique decision identity.
- **DEC-006-REQ-002:** The Ledger SHALL distinguish proposal, decision, authorization, execution, outcome, and evaluation.
- **DEC-006-REQ-003:** The Ledger SHALL record the evidence, provenance, quality, and effective time used by the decision.
- **DEC-006-REQ-004:** Material assumptions, uncertainty, confidence, and known blind spots SHALL be explicit.
- **DEC-006-REQ-005:** The relevant historical Self Model and Fitness to Operate result SHALL be linked or reconstructable.
- **DEC-006-REQ-006:** Alternatives considered, including non-action where applicable, SHALL be recorded.
- **DEC-006-REQ-007:** Expected capital effect, material risks, limits, and residual risk SHALL be recorded.
- **DEC-006-REQ-008:** Recommendation, decision, approval, and execution authorities SHALL remain distinguishable and attributable.
- **DEC-006-REQ-009:** Guardian restrictions, Capital Safety results, and other independent constraints SHALL be recorded.
- **DEC-006-REQ-010:** Execution SHALL record what actually occurred, including partial, rejected, delayed, failed, or externally modified action.
- **DEC-006-REQ-011:** Outcome SHALL remain distinct from decision quality and execution quality.
- **DEC-006-REQ-012:** Later evaluation SHALL preserve what was reasonably known at decision time.
- **DEC-006-REQ-013:** Accepted Ledger history SHALL be protected against undetected alteration and deletion.
- **DEC-006-REQ-014:** Corrections SHALL append a traceable correction without rewriting the original record.
- **DEC-006-REQ-015:** Causal and correlation relationships SHALL be preserved across related decisions, actions, and events.
- **DEC-006-REQ-016:** Access SHALL protect confidentiality without preventing authorized accountability.
- **DEC-006-REQ-017:** A material decision SHALL NOT be treated as fully auditable when required Ledger evidence is missing.
- **DEC-006-REQ-018:** Self-maintenance and evolution decisions SHALL use the same Ledger principles as financial decisions.

## 5. Acceptance Evidence

Approval requires evidence for:

- end-to-end reconstruction of financial and evolution decisions;
- authority separation;
- historical Self Model linkage;
- decision/outcome distinction;
- correction without rewriting;
- partial execution representation;
- tamper detection; and
- protected but reviewable access.

## 6. ADR Candidates

- Ledger storage model;
- cryptographic integrity approach;
- financial-record linkage;
- retention and archival mechanism; and
- cross-environment identity.

## 7. Unresolved Matters

- Materiality thresholds by decision class.
- Retention obligations by jurisdiction.
