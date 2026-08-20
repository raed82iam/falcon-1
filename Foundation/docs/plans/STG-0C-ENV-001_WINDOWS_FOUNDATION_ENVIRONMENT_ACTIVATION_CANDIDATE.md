# STG-0C-ENV-001 — Windows Foundation Environment Activation Candidate

**Identifier:** STG-0C-ENV-001  
**Version:** 1.0  
**Status:** Approved  
**Prepared:** 2026-07-27  
**Approval Date:** 2026-07-27  
**Approval Record:** GOV-055  
**Governing Authority:** GOV-054; STG-0C-PROP-001; ENV-001; VPL-BST-006  
**Environment Activation:** Not Granted

## 1. Purpose

This candidate defines the evidence needed to consider one exact local Windows environment for Foundation build and verification only.

## 2. Required Identity

The environment case shall record:

- environment ID, class, version, host and runtime epoch;
- operating-system identity and patch state;
- admitted tools, sources, versions, digests, and capabilities;
- configuration and policy snapshots;
- Provider dependencies and their exact active scopes;
- filesystem and process boundaries;
- network policy and observed endpoints;
- repository and working-tree baseline;
- time and clock-quality evidence;
- security posture, trust roots, restrictions, and expiry;
- and cleanup and residue results.

## 3. Permitted Scope

If activated, the environment may support only the exact Foundation build-verification purposes declared by its Manifest.

It shall not become Falcon’s runtime, production, Oracle Cloud, or a financial environment.

## 4. Verification

VPL-BST-006 shall cover valid, wrong-subject, wrong-profile, stale, restricted, revoked, expired, tampered, incomplete, and reconstruction cases.

The environment shall not verify or restore itself as the sole authority.

## 5. Portability

Windows is the first platform, not Falcon’s identity. Platform-specific behavior shall remain behind Falcon Contracts and Adapters. No persistence or cloud technology assumption may cross the declared boundary.

## 6. Current Effect

No environment is activated or authorized for use by this document.
