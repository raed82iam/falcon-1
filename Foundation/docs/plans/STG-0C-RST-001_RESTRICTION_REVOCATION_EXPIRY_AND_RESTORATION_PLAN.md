# STG-0C-RST-001 — Restriction, Revocation, Expiry, and Restoration Plan

**Identifier:** STG-0C-RST-001  
**Version:** 1.0  
**Status:** Approved  
**Prepared:** 2026-07-27  
**Approval Date:** 2026-07-27  
**Approval Record:** GOV-055  
**Governing Authority:** GOV-054; STG-0C-PROP-001; CON-011  
**Control Activation:** Not Granted

## 1. Purpose

This candidate ensures protective controls exist before any scoped reliance.

## 2. Control States

Every activated subject shall support `ACTIVE_SCOPED`, `RESTRICTED`, `SUSPENDED`, `REVOKED`, `EXPIRED`, and `SUPERSEDED` as applicable.

The strongest applicable protective state governs.

## 3. Triggers

Restriction or stop is required for authority uncertainty, evidence invalidity, health or dependency degradation, custody concern, identity or time uncertainty, scope breach, stale policy, Challenge, suspected compromise, financial/cloud contact, or inability to prove the subject’s state.

## 4. Non-Self-Release

A subject shall not modify, suppress, weaken, release, or restore its own protective state; treat repair as release; or resume unrestricted behavior merely because a dependency returned.

Guardian policy remains superior. Stage 0C shall not implement Guardian.

## 5. Restoration

Restoration requires:

1. preserved incident and restriction evidence;
2. remediation under separate authority;
3. independent confirmation that the material risk ended;
4. current dependency and validity evidence;
5. competent jurisdiction and Acceptance;
6. a new scoped authority decision;
7. controlled transition and monitoring.

## 6. Current Effect

No control state is activated or changed.
