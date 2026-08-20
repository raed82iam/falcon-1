# VPL-007 — Controlled Recovery Verification Plan

**Identifier:** VPL-007  
**Version:** 1.0  
**Status:** Approved  
**Effective Date:** 2026-07-24  
**Approval Record:** GOV-007  
**Scenario:** FRS-SCN-007  
**Owner:** Falcon Verification Authority  
**Governing Sources:** FRS-001; OPS-003; AUT-001; AUT-002; SYS-002; SYS-011; ADR-F005; ADR-F008  
**Master Plan:** VPL-000  
**Supersedes:** None  
**Superseded By:** None

## 1. Verification Objective

Prove that Falcon recovery is a controlled, evidenced progression in which repair cannot certify itself, failed validation prevents release, and unrestricted authority returns only after independent verification and authorized approval.

## Scope and Non-Scope

This plan verifies Foundation recovery from the harmless VPL-006 restriction. It does not verify disaster recovery, high availability, live-capital recovery, external institution reconciliation, or autonomous production promotion.

Result vocabulary and global evidence rules are governed by VPL-000.

## 2. Required Setup

- the unresolved restriction produced under VPL-006;
- an approved versioned recovery plan;
- controlled repair action and rollback direction;
- authoritative state and evidence checkpoint;
- an Independent Verifier separated from the repair actor;
- declared release authority; and
- valid and deliberately invalid recovery outcomes.

## 3. Primary Procedure

1. Confirm containment and the persistent Guardian restriction.
2. Record the fault, uncertainty, authoritative state, and recovery-plan identity.
3. Obtain authorized recovery initiation.
4. Execute restoration within the plan’s scope.
5. Reconcile configuration, identity, authority, security, durable state, dependencies, and evidence integrity.
6. Have the Independent Verifier apply the approved validation criteria.
7. Obtain the declared release-authority decision.
8. Confirm Guardian release conditions are satisfied.
9. Perform controlled Lifecycle reintroduction.
10. Issue a new attributable authority decision.
11. Observe the component under heightened monitoring before normal status.

## 4. Mandatory Negative Variants

- repair actor claims success without independent validation;
- validation fails;
- state reconciliation remains uncertain;
- required evidence is missing or integrity-failed;
- recovery is partial;
- the old security context is reused after compromise;
- the component restarts but the trigger remains unresolved; and
- repeated recovery exceeds its bounded attempt limit.

## 5. Expected Results

- Repair completion remains distinct from validated recovery.
- Every negative variant retains restriction and denies unrestricted authority.
- Partial recovery is reported as partial.
- Failed validation triggers rollback, continued restriction, or escalation.
- Recovery cannot release its own Guardian constraint.
- Successful release requires independent evidence, declared authority, controlled transition, and a new authorization result.

## 6. Required Evidence

Recovery initiation, plan and version, trigger and containment, repair actions, authoritative-state reconciliation, validation inputs and results, verifier identity, release decision, Guardian condition result, Lifecycle transitions, new authority decision, rollback or escalation records, and residual risk.

## 7. Pass Rule

`PASS` requires successful controlled recovery plus correct denial for every negative variant. Any self-certified release, unrestricted operation with uncertain integrity, or reuse of invalid trust is an immediate `FAIL`.

## 8. Independent Verification

The Independent Verifier shall not be the repair actor, the recovered component, or a decision derived solely from their evidence. It shall use independently obtained or integrity-verified evidence and shall record its own attributable result.

## 9. Containment, Cleanup, and Repeatability

Failed variants shall remain restricted. Successful cleanup requires independently verified authoritative-state reconciliation and shall preserve the incident and recovery record. Each repetition shall start from a newly identified restriction or a proven equivalent checkpoint.

## 10. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner | Approved | GOV-007 | 2026-07-24 |
