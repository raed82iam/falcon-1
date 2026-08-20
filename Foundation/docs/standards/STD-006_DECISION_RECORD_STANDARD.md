# Decision Record Standard

**Identifier:** STD-006  
**Version:** 1.0  
**Status:** Approved  
**Effective Date:** 2026-07-24  
**Approval Record:** GOV-006  
**Owner:** Falcon Standards Authority  
**Governing Authority:** Falcon Constitution Articles 13, 16–20, 26–32, and 39; DEC-006
**Supersedes:** None  
**Superseded By:** None

## 1. Purpose

This Standard defines the required form, evidence, and lifecycle of records for material Falcon decisions.

It governs decision records. It does not make decisions, grant authority, or replace domain Specifications, financial books, operational state, or Architecture Decision Records.

## 2. Materiality

A decision record is mandatory when a decision may materially affect:

- capital, exposure, allocation, or financial obligation;
- protection, risk limits, or Safe state;
- authority, autonomy, or delegation;
- system identity, security, trust, or evidence integrity;
- authoritative state or recovery;
- capability admission, replacement, or removal;
- self-maintenance or self-evolution;
- constitutional interpretation or exception; or
- a release or high-consequence operational action.

Materiality SHALL consider cumulative and correlated decisions, not only each decision in isolation.

## 3. Decision Stages

The record SHALL distinguish:

1. observation or trigger;
2. analysis;
3. proposal or recommendation;
4. decision;
5. authorization;
6. execution attempt;
7. persistence or external acceptance;
8. outcome;
9. later evaluation; and
10. learning or corrective action.

Completion of one stage SHALL NOT imply completion or authority for another.

## 4. Required Decision Record

Every material decision record SHALL contain or reference:

- unique decision ID and decision class;
- accountable owner and participating actors;
- purpose, scope, effective time, and expiry where applicable;
- governing authority and applicable constraints;
- evidence, provenance, quality, and effective time;
- historical Self Model and Fitness to Operate context where relevant;
- assumptions, uncertainty, confidence, contradictions, and blind spots;
- alternatives considered, including non-action when applicable;
- expected benefit and capital, risk, security, operational, and long-term effects;
- reversibility, dependencies, limits, and stop conditions;
- recommendation authority;
- decision authority;
- approval authority;
- execution authority;
- Guardian, Capital Safety, Security, or other independent constraints;
- actual execution, including rejection, delay, partial result, or external modification;
- outcome and residual state;
- later evaluation separated from outcome; and
- correlation, causation, and related record identities.

## 5. Decision-Time Integrity

The record SHALL preserve what was reasonably known, assumed, unknown, and authorized at decision time.

Later outcomes SHALL NOT rewrite the original evidence, reasoning, uncertainty, or authority. Corrections and later learning SHALL append attributable records linked to the original.

## 6. Authority Separation

Recommendation, decision, approval, and execution authorities SHALL remain explicit and distinguishable.

No record shall convert recommendation into permission, technical ability into authority, or successful execution into retrospective authorization.

Required independence SHALL increase with consequence. A self-generated high-consequence change SHALL NOT be produced, verified, approved, and promoted by one unchallenged authority.

## 7. Decision Quality and Outcome

Decision quality SHALL be evaluated against the evidence, uncertainty, alternatives, authority, and obligations applicable at decision time.

Outcome quality SHALL be recorded separately. Profit does not validate an unsound decision; loss does not alone invalidate a sound decision.

Evaluation criteria and evaluator identity SHALL be preserved.

## 8. Record Integrity and Access

Accepted decision history SHALL be protected against undetected alteration and unauthorized deletion.

Access SHALL protect confidentiality while preserving authorized accountability. Redaction SHALL be attributable and SHALL NOT conceal information required by the authorized reviewer.

Missing required evidence SHALL cause the decision to be marked incomplete, unauditable, or invalid according to governing consequence.

## 9. Exceptions, Corrections, and Supersession

An exception SHALL identify the rule, authority, justification, risk, scope, duration, controls, and expiry.

A correction SHALL append without erasing the original. A later decision that changes the governing result SHALL identify the prior decision and whether it supersedes, narrows, extends, or revokes it.

## 10. Prohibited Practices

A decision record SHALL NOT:

- be created retrospectively as if it existed before action;
- conceal rejected alternatives or material adverse evidence;
- combine decision, authorization, execution, and outcome into one ambiguous status;
- infer authority from identity, capability, repetition, or prior success;
- rewrite history after outcomes are known;
- present generated explanation disconnected from actual reasoning; or
- claim complete auditability while required evidence is missing.

## 11. Compatibility and Transition

Approval of this Standard SHALL NOT rewrite or retroactively complete a prior decision record.

Existing material decisions SHALL preserve their original history and add any new classification, limitation, or correction as linked evidence. New decisions, renewals, changed authority, and material extensions SHALL conform from their effective date.

## 12. Acceptance Evidence

Conformance requires:

- end-to-end stage separation;
- attributable and legitimate authority;
- decision-time evidence and uncertainty;
- alternatives and non-action analysis where applicable;
- actual execution and outcome distinction;
- append-only correction and evaluation;
- integrity and access-control evidence; and
- successful independent reconstruction.

## 13. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Falcon Standards Authority | Approved | GOV-006 | 2026-07-24 |
