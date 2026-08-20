# VPL-001 — Trusted Bootstrap Verification Plan

**Identifier:** VPL-001  
**Version:** 1.0  
**Status:** Approved  
**Effective Date:** 2026-07-24  
**Approval Record:** GOV-007  
**Scenario:** FRS-SCN-001  
**Owner:** Falcon Verification Authority  
**Governing Sources:** FRS-001; SYS-001; SYS-007; SEC-001; CON-001; CON-007; CON-009; ADR-F001; ADR-F006; ADR-F007
**Master Plan:** VPL-000  
**Supersedes:** None  
**Superseded By:** None

## 1. Verification Objective

Prove that Falcon reaches only a restricted, non-financial running state after verifying its approved baseline, instance identity, component identities, effective configuration, security context, and Core admission conditions.

## Scope and Non-Scope

This plan verifies Foundation bootstrap identity, trust, admission, configuration, and restricted startup. It does not verify financial readiness, external identity federation, distributed startup, high availability, or production security.

Result vocabulary and global evidence rules are governed by VPL-000.

## 2. Required Setup

- isolated non-financial environment;
- approved baseline manifest and trust anchor;
- exact admitted artifact set;
- valid configuration sources;
- unique instance and component identities;
- protected evidence store; and
- controlled invalid variants for each required trust input.

## 3. Primary Procedure

1. Record the initial stopped state and execution identity.
2. Present the approved baseline and valid configuration sources.
3. Start Falcon and capture each verification and admission decision.
4. Confirm one unique Falcon instance identity.
5. Confirm one distinct identity for every admitted Core component.
6. Confirm that authentication does not itself grant operational authority.
7. Confirm deterministic effective configuration and protected secret references.
8. Confirm admission only after identity, artifact, owner, capability, lifecycle, and authority checks.
9. Confirm the final state is explicitly restricted and non-financial.
10. Stop and reconstruct the complete bootstrap sequence from preserved evidence.

## 4. Mandatory Negative Variants

Repeat with each condition independently:

- unknown trust anchor;
- invalid or expired baseline signature;
- modified artifact;
- duplicate active instance identity;
- wrong-instance component identity;
- revoked or expired identity;
- missing required configuration;
- unauthorized configuration override;
- unavailable required revocation status; and
- secret value placed directly in ordinary configuration.

## 5. Expected Results

- The valid baseline reaches the declared restricted non-financial state.
- Every invalid variant prevents unrestricted startup.
- No invalid subject is admitted as Core.
- No identity receives authority merely by authentication.
- Failure is explicit and produces protected evidence without exposing secrets.
- No financial capability or external financial path exists.

## 6. Required Evidence

Baseline identity, signature result, artifact digests, instance and component identity records, security contexts, configuration snapshot, admission decisions, lifecycle transitions, authority decisions, rejection reasons, and integrity checkpoint.

## 7. Pass Rule

`PASS` requires the primary procedure to succeed and every negative variant to fail closed with complete reconstructable evidence. Any unrestricted startup under an invalid or unknown trust condition is an immediate `FAIL`.

## 8. Independent Verification

The Independent Verifier shall compare the running identities and effective configuration to the approved baseline without relying solely on claims produced by the admitted component.

## 9. Containment, Cleanup, and Repeatability

Each variant shall use an isolated identity scope and recoverable baseline. Invalid identities, secrets, and configuration shall be revoked or destroyed after evidence capture. Repetition shall use fresh execution and instance identities and shall preserve every prior result.

## 10. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner | Approved | GOV-007 | 2026-07-24 |
