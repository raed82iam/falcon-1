# VPL-000 — Foundation Verification Master Plan

**Identifier:** VPL-000  
**Version:** 1.0  
**Status:** Approved  
**Effective Date:** 2026-07-24  
**Approval Record:** GOV-007  
**Assessment Scope:** FRS-001 Foundation Release  
**Owner:** Falcon Verification Authority  
**Governing Authority:** FRS-001; Approved Foundation Specifications, Contracts, and Accepted ADR-F001 through ADR-F008  
**Plans:** VPL-001 through VPL-008  
**Implementation Authority:** None
**Supersedes:** None  
**Superseded By:** None

## 1. Purpose

This plan defines how Falcon shall verify the eight mandatory FRS-001 demonstration scenarios before the Foundation Release may be accepted.

It creates no implementation authority and makes no financial-readiness claim.

## 2. Verification Principles

1. Evidence, not assertion, determines the result.
2. The actor that performs a material action shall not be the sole authority that verifies its success.
3. Expected denial, restriction, or failure is a successful verification outcome when required by the governing baseline.
4. Missing, corrupt, contradictory, or unreconstructable required evidence causes failure.
5. A passed scenario shall not compensate for a failed release invariant.
6. Repeated execution shall begin from a declared clean or reconciled state.
7. Every injected condition shall be controlled, attributable, reversible, and incapable of creating financial consequence.
8. Verification shall distinguish request, admission, authorization, execution, persistence, outcome, and later evaluation.

## 3. Global Safety Boundary

All verification shall occur in an isolated, non-financial environment with:

- no broker, venue, market-data, or live-capital connection;
- no financial credential or production secret;
- no trading, order, portfolio, allocation, or strategy capability;
- controlled time, identity, configuration, and fault inputs;
- a recoverable initial state;
- protected evidence collection; and
- an independently controlled abort path.

Any discovered path capable of reaching real capital or an external financial action is an immediate release-blocking failure.

## 4. Roles and Separation

| Role | Responsibility | Prohibited combination |
|---|---|---|
| Scenario Controller | Starts the scenario and injects declared conditions | Shall not alter resulting evidence |
| System Under Verification | Performs the governed Falcon behavior | Shall not approve its own result |
| Evidence Collector | Preserves records and integrity state | Shall not reinterpret missing evidence as success |
| Independent Verifier | Evaluates evidence against the plan | Shall not be the actor that performed the repair or governed action being certified |
| Release Authority | Accepts or rejects the release result | Shall not waive failed invariants through ordinary approval |

Automation may perform more than one operational function only when role identity, permissions, evidence, and independent result evaluation remain separated.

## 5. Required Evidence Package

Every scenario package shall contain:

- plan ID and execution ID;
- approved baseline and artifact identities;
- environment and effective-configuration identity;
- participant identities and security contexts;
- initial authoritative state;
- controlled inputs and injected faults;
- authority decisions;
- FIL messages and event identities where applicable;
- lifecycle, health, Fitness, Guardian, and recovery records where applicable;
- persistence outcomes and integrity checkpoints;
- expected and actual results;
- deviations, uncertainties, and missing evidence;
- independent-verifier identity and decision; and
- start, end, duration, and clock-quality evidence.

Secrets and prohibited sensitive values shall not enter the package.

## 6. Result Vocabulary

- **PASS:** Every required claim and invariant is supported by complete, valid evidence.
- **FAIL:** At least one required claim or invariant is contradicted or unsatisfied.
- **INCONCLUSIVE:** Required evidence is missing, corrupt, stale, contradictory, or outside the declared assurance.
- **BLOCKED:** A prerequisite prevents safe execution before the scenario begins.

Only `PASS` satisfies the FRS-001 exit criterion. `INCONCLUSIVE` shall never be converted to `PASS` through assumption.

## 7. Scenario Plans and Traceability

| Plan | Scenario | Primary proof | Release invariants |
|---|---|---|---|
| VPL-001 | FRS-SCN-001 Trusted Bootstrap | Only a verified baseline reaches restricted non-financial operation | INV-001, INV-002, INV-007, INV-008 |
| VPL-002 | FRS-SCN-002 Unauthorized Action | Authenticated but unauthorized action is denied | INV-001, INV-004, INV-007 |
| VPL-003 | FRS-SCN-003 Invalid Lifecycle Transition | Invalid transition cannot change authoritative state | INV-001, INV-004, INV-007 |
| VPL-004 | FRS-SCN-004 Invalid FIL Message | Invalid or unauthorized message is rejected explicitly | INV-001, INV-004, INV-007 |
| VPL-005 | FRS-SCN-005 Health Evidence Loss | Unknown required fitness reduces authority | INV-003, INV-004, INV-007 |
| VPL-006 | FRS-SCN-006 Guardian Restriction | Guardian independently imposes enforceable Safe state | INV-004, INV-005, INV-007 |
| VPL-007 | FRS-SCN-007 Controlled Recovery | Repair cannot certify or release itself | INV-004, INV-006, INV-007 |
| VPL-008 | FRS-SCN-008 Evidence Reconstruction | Every prior scenario is reconstructed completely | INV-001, INV-004, INV-007, INV-008 |

## 8. Execution Order

The default order is VPL-001 through VPL-008.

VPL-008 shall run last because it assesses the evidence packages produced by VPL-001 through VPL-007. VPL-007 depends on a restriction established through the controls verified by VPL-006.

Any out-of-order execution shall state the reason and preserve equivalent prerequisites.

## 9. Release Decision Rule

FRS-001 verification passes only when:

1. VPL-001 through VPL-008 each result in `PASS`;
2. every FRS-001 invariant is independently confirmed;
3. no financial or live-capital path exists;
4. no unresolved release-blocking security issue exists;
5. recovery and rollback evidence is complete;
6. known limitations are explicit and owned; and
7. the Release Authority records the decision.

Passing these plans does not authorize implementation, financial operation, autonomous evolution, or exposure of capital.

## 10. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner | Approved | GOV-007 | 2026-07-24 |
