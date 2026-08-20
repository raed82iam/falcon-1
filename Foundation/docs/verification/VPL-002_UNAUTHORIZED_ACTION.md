# VPL-002 — Unauthorized Action Verification Plan

**Identifier:** VPL-002  
**Version:** 1.0  
**Status:** Approved  
**Effective Date:** 2026-07-24  
**Approval Record:** GOV-007  
**Scenario:** FRS-SCN-002  
**Owner:** Falcon Verification Authority  
**Governing Sources:** FRS-001; AUT-001; SEC-001; CON-002; CON-009; ADR-F002; ADR-F003; ADR-F006
**Master Plan:** VPL-000  
**Supersedes:** None  
**Superseded By:** None

## 1. Verification Objective

Prove that a correctly authenticated component cannot perform an action outside its declared authority and that the denial is attributable and reconstructable.

## Scope and Non-Scope

This plan verifies Foundation authentication/authorization separation and denial enforcement. It does not grant authority, evaluate financial actions, or prove every future policy model.

Result vocabulary and global evidence rules are governed by VPL-000.

## 2. Required Setup

- a trusted Falcon instance from VPL-001;
- one authenticated component with deliberately limited authority;
- one permitted control action;
- one action outside the component’s scope;
- one expired delegation and one revoked delegation; and
- authoritative state whose unchanged value can be independently confirmed.

## 3. Procedure

1. Record actor identity, security context, authority baseline, and initial state.
2. Execute the permitted control action and record its separate authorization and outcome.
3. Request the prohibited action using the same valid identity.
4. Repeat using an expired delegation.
5. Repeat after immediate revocation of an otherwise valid delegation.
6. Attempt the prohibited action through the normal FIL path, retry, replay, and any declared direct execution boundary.
7. Confirm the authoritative state remains unchanged.
8. Reconstruct every decision and attempted path.

## 4. Expected Results

- Valid authentication is preserved and is not misreported as authorization.
- Every prohibited, expired, revoked, retried, or replayed request is denied.
- The component cannot gain permission from prior success, technical reach, or message validity.
- Denial records identify actor, action, resource, purpose, authority baseline, controlling rule, and reason.
- No prohibited side effect or authoritative state change occurs.

## 5. Required Evidence

Identity and security context, request messages, authority inputs and decisions, policy version, revocation record, execution-boundary result, state versions before and after, and correlated evidence records.

## 6. Pass Rule

`PASS` requires all unauthorized paths to be denied with unchanged authoritative state and complete evidence. Any prohibited execution or unexplained state change is an immediate `FAIL`.

## 7. Independent Verification

The Independent Verifier shall inspect the authoritative state owner and execution boundary directly; Authority Engine denial alone is insufficient proof that no action occurred.

## 8. Containment, Cleanup, and Repeatability

All requested actions shall be harmless Foundation actions in an isolated state scope. Cleanup shall reconcile authoritative state and revoke test delegations. Repetition shall use new request identities and preserve all denials, retries, and prior outcomes.

## 9. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner | Approved | GOV-007 | 2026-07-24 |
