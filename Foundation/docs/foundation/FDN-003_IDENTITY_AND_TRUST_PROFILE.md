# FDN-003 — Foundation Identity and Trust Profile

**Identifier:** FDN-003  
**Version:** 1.0  
**Status:** Approved  
**Effective Date:** 2026-07-24  
**Approval Record:** GOV-008  
**Owner:** Falcon Security Authority  
**Governing Authority:** SEC-001; CON-001; CON-009; CON-010; ADR-F006; STD-007  
**Supersedes:** None  
**Superseded By:** None

## 1. Purpose

This profile defines root-anchor custody, instance and workload identity issuance, revocation input, and trust-recovery verification for the isolated FRS-001 Foundation.

## 2. Root-Anchor Custody

| Control | Foundation rule |
|---|---|
| Owner | Project Owner acting as current Foundation Trust Custodian |
| Use | Verify or authorize Foundation release manifests only |
| Online state | Root private material remains offline except during an attributable signing ceremony |
| Access | Explicit custodian authorization; no component access |
| Copies | Minimum governed copies, individually inventoried and protected |
| Evidence | Ceremony ID, participants, purpose, manifest digest, time, and result |
| Prohibition | Root private material never enters repository, configuration, logs, messages, test evidence, or runtime |
| Compromise | Suspend affected trust, revoke dependent manifests, investigate scope, replace anchor through governance |

The implementation mechanism must satisfy this profile and shall be selected through an approved security design before code handles real private material.

## 3. Identity Issuance Flow

1. Verify the root anchor and CON-010 manifest.
2. Verify issuer identity, authority, validity, and revocation freshness.
3. Establish a unique Falcon instance ID.
4. Bind the instance identity to release, environment, configuration snapshot, and validity.
5. For each admitted Core component, verify artifact identity and admission record.
6. Issue a distinct scoped workload identity bound to instance, component, artifact, capability, owner, and validity.
7. Create a CON-009 security context.
8. Request operational authority separately from Authority Engine.
9. Preserve issuance evidence without private material.

## 4. Identity Classes

| Class | Scope | Maximum authority implication |
|---|---|---|
| Root trust anchor | Manifest verification | No operational authority |
| Release-signing identity | Named release manifest | No runtime authority |
| Instance identity | One Falcon Foundation instance | Identity only |
| Core workload identity | One admitted component instance | Identity and authentication only |
| Human review identity | Named governed review | Only separately delegated review authority |
| Verification identity | One VPL execution role | Only declared verification authority |

## 5. Revocation Input

The revocation input SHALL contain issuer, version, issue time, maximum age, revoked identity or manifest ID, effective time, reason class, scope, replacement reference when applicable, and integrity evidence.

Unknown, invalid, stale, contradictory, or unavailable required revocation input prevents unrestricted startup or continued affected authority.

Revocation SHALL propagate to:

- security context;
- Authority Engine decisions and caches;
- Kernel admission;
- communication authorization;
- configuration and secret access;
- Guardian and Health Monitoring; and
- recovery planning.

## 6. Trust-Recovery Verification

Trust restoration requires:

1. containment of the suspected identity and reachable authority;
2. cause and scope analysis;
3. revocation of affected identity, delegation, context, and secrets;
4. artifact, configuration, state, evidence, and dependency integrity checks;
5. issuance of new identity material;
6. independent validation using evidence not solely produced by the repaired subject;
7. controlled readmission and Lifecycle transition;
8. new Authority Engine decision; and
9. heightened monitoring with explicit closure criteria.

The repair actor and compromised subject cannot be the sole verifier. Passing VPL-007 is required for the Foundation trust-recovery claim.

## 7. Minimum Rejection Cases

Unknown anchor, invalid manifest signature, duplicate instance ID, wrong-instance workload, artifact mismatch, expired identity, revoked context, stale revocation input, replayed context, insufficient assurance, and reuse of compromised context shall each prevent affected authority.

## 8. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner | Approved | GOV-008 | 2026-07-24 |
