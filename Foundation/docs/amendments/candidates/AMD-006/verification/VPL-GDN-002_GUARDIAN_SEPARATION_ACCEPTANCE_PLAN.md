# VPL-GDN-002 — Guardian Separation Acceptance Plan

**Version:** Proposed 1.0  
**Status:** Approved Plan — Execution Not Authorized  
**Approval Record:** GOV-062  
**Governing Sources:** Proposed ADR-I011; AUT-002 v2.1; RSK-006; CON-022

## Objective

Prove that Platform protection and Trading protection remain independent, coordinated, authorized, payload-minimized, persistent, and reconstructable.

## Mandatory Scenarios

| ID | Required proof |
|---|---|
| GDN-001 | Trading-domain danger from a Trading Application is detected |
| GDN-002 | Trading Guardian remains available when FSATA fails |
| GDN-003 | new exposure is prevented when TG is absent, untrusted, or restrictive |
| GDN-004 | TG submits a valid CON-022 request |
| GDN-005 | TG requests investigation of another Application |
| GDN-006 | TG requests isolation of a harmful Accounting Application |
| GDN-007 | TG requests Platform Safe Mode without activating it |
| GDN-008 | FFG independently rejects unsupported request |
| GDN-009 | FFG applies narrower justified action |
| GDN-010 | FFG applies stronger justified action |
| GDN-011 | FFG isolates one harmful Application and preserves unaffected Applications |
| GDN-012 | technical criticality preserves Trading capabilities without business interpretation |
| GDN-013 | FFG cannot inspect Trading business payload |
| GDN-014 | FFG cannot inspect Accounting business payload |
| GDN-015 | FSA diagnoses technical source without deciding Trading safety |
| GDN-016 | FSA repair preserves FFG restriction and independent verification |
| GDN-017 | Platform restriction survives restart/failover |
| GDN-018 | Trading restriction survives restart/failover |
| GDN-019 | `PLATFORM_NORMAL` does not produce `TRADING_NORMAL` |
| GDN-020 | conflicting Guardian requests use technical policy, uncertainty, and escalation |
| GDN-021 | unauthorized Application request is rejected fail-closed |
| GDN-022 | compromised TG cannot isolate another Application |
| GDN-023 | compromised FFG is isolated without clearing all protection |

## Additional Contract Cases

- invalid identity, mandate, integrity, version, expiry, replay, and evidence;
- duplicate and reordered requests without duplicate effect;
- rate-limit and unsupported-request abuse;
- provisional containment expiry and mandatory review;
- decision/execution distinction;
- separate Platform and Trading release;
- complete reconstruction through FIL and Service Bus evidence.

## Evidence Set

The Root Verification Evidence Set SHALL preserve obligations, immutable environment/context, identities, authorities, inputs, business-data exclusion evidence, request/response lifecycle, independent observations, decisions, directives, execution outcomes, restart/failover observations, release decisions, contradictions, challenges, provenance, integrity, and completeness.

## Acceptance

Every mandatory scenario must pass under a `COMPLETE`, valid Evidence Set for the declared scope. Approval of this plan does not authorize execution.
