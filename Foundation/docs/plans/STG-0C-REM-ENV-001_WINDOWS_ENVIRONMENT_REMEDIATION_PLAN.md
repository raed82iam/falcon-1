# STG-0C-REM-ENV-001 — Windows Environment Remediation Plan

**Identifier:** STG-0C-REM-ENV-001  
**Version:** 1.0  
**Status:** Approved  
**Approval Date:** 2026-07-27  
**Approval Record:** GOV-058  
**Environment Use:** Granted within the local remediation case only  
**Environment Activation:** Requires an individual evidence-backed decision

## 1. Proposed Environment

One exact Windows Foundation build-verification environment would bind:

- OS and patch identity;
- admitted SDK, tools, dependencies, sources, and digests;
- repository-local isolated .NET configuration;
- active Provider Profile identities;
- configuration, policy, and trust snapshots;
- filesystem, process, network, and export boundaries;
- runtime epoch and bootstrap origin;
- cleanup, expiry, restriction, revocation, and restoration controls.

## 2. Network

Default network is denied. Package sources, telemetry, cloud, financial endpoints, and undeclared destinations remain prohibited.

## 3. Portability

Windows-specific behavior remains behind Falcon Contracts and Adapters. The resulting profile shall not prevent a later Linux or Oracle Cloud implementation.

## 4. Activation Condition

VPL-BST-006 must pass with active prerequisite Profiles and independent review before an environment decision is eligible.
