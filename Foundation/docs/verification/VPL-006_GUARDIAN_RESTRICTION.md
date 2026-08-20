# VPL-006 — Guardian Restriction Verification Plan

**Identifier:** VPL-006  
**Version:** 1.0  
**Status:** Approved  
**Effective Date:** 2026-07-24  
**Approval Record:** GOV-007  
**Scenario:** FRS-SCN-006  
**Owner:** Falcon Verification Authority  
**Governing Sources:** FRS-001; AUT-001; AUT-002; SYS-001; SYS-002; RSK-005; ADR-F001; ADR-F008
**Master Plan:** VPL-000  
**Supersedes:** None  
**Superseded By:** None

## 1. Verification Objective

Prove that Guardian can impose a binding, proportionate restriction independently of the subject, that Authority Engine and execution boundaries enforce it, and that Lifecycle enters the required protective state.

## Scope and Non-Scope

This plan verifies a harmless Foundation protective condition and non-financial Safe state. It does not verify live-capital limits, broker safeguards, open-position handling, or production emergency channels.

Result vocabulary and global evidence rules are governed by VPL-000.

## 2. Required Setup

- one admitted component with a harmless governed action;
- one mandatory protective trigger and mandate;
- protected Guardian control path;
- declared Safe-state allowlist;
- authority and lifecycle enforcement points;
- controlled restart and communication-loss capability; and
- no financial or external action path.

## 3. Procedure

1. Record normal authority, lifecycle state, health, and evidence.
2. Introduce the mandatory protective condition independently of the subject.
3. Observe Guardian issue the scoped restriction.
4. Attempt the affected action before, during, and after authority revocation.
5. Attempt bypass through direct path, retry, replay, restart, maintenance, recovery, and configuration.
6. Interrupt normal enforcement communication and verify fail-closed behavior.
7. Confirm Lifecycle reaches the required protective state.
8. Confirm the minimum Safe-state allowlist remains available.
9. Confirm unrelated safe operation continues only where isolation is trustworthy.
10. Leave the restriction unresolved and restart Falcon to verify persistence.

## 4. Expected Results

- Guardian intervention is attributable, within mandate, and proportionate.
- Affected authority is revoked at or before the execution boundary.
- Every bypass attempt is denied.
- Unknown restriction or revocation state fails closed.
- The restricted component cannot weaken or release the restriction.
- Safe state preserves only the declared protection, observation, security, evidence, containment, and recovery capabilities.
- The unresolved restriction survives restart.

## 5. Required Evidence

Trigger evidence, Guardian mandate and decision, restriction identity and scope, authority revocation, enforcement results, lifecycle transitions, bypass attempts, communication-loss response, Safe-state capability observations, restart persistence, and integrity checkpoint.

## 6. Pass Rule

`PASS` requires independent restriction, complete enforcement, correct protective state, preserved minimum protection, restart persistence, and no bypass. Any affected action executed after the binding restriction or any self-release is an immediate `FAIL`.

## 7. Independent Verification

The Independent Verifier shall control the harmless trigger and observe the execution boundary separately from Guardian and the restricted subject.

## 8. Containment, Cleanup, and Repeatability

The trigger shall be reversible and isolated. Cleanup shall not lift the restriction; it prepares the subject for VPL-007 controlled recovery. Repetition requires a new restriction identity and a reconciled starting state while preserving all prior interventions.

## 9. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner | Approved | GOV-007 | 2026-07-24 |
