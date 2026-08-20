# FDN-005 — Foundation Protection and Release Control Matrix

**Identifier:** FDN-005  
**Version:** 1.0  
**Status:** Approved  
**Effective Date:** 2026-07-24  
**Approval Record:** GOV-008  
**Owner:** Falcon Protection Authority  
**Governing Authority:** AUT-001; AUT-002; SYS-001; SYS-002; OPS-003; CON-011; ADR-F008  
**Supersedes:** None  
**Superseded By:** None

## 1. Purpose

This document defines the FRS-001 Guardian mandate, minimum Safe-state allowlist, restriction enforcement points, and release-authority separation.

It is non-financial and cannot authorize trading, capital exposure, or external financial connectivity.

## 2. Protective Mandate Matrix

| Condition | Evidence threshold | Guardian action | Authority effect | Lifecycle target | Release minimum |
|---|---|---|---|---|---|
| unknown or invalid baseline | one failed required bootstrap check | restrict entire instance | deny non-diagnostic authority | `RESTRICTED` or remain stopped | valid baseline and independent bootstrap verification |
| unknown or revoked identity | failed identity or revocation check | isolate affected subject | revoke affected authority | `SUSPENDED` | new identity and independent trust validation |
| missing required Fitness | CON-006 result not fit or unknown | restrict affected scope | deny dependent action | `RESTRICTED` | fresh evidence and new Fitness decision |
| invalid authoritative state | conflict, corruption, or uncertain write | isolate state owner | deny state-dependent action | `SUSPENDED` or `FAILED` | reconciliation and independent integrity validation |
| audit-critical evidence loss | missing acceptance or integrity | restrict material activity | deny actions requiring audit evidence | `RESTRICTED` | evidence path restored and verified |
| Authority Engine trust loss | policy, identity, or integrity unknown | instance-wide restriction | default deny | `RESTRICTED` | restored authority baseline and independent verification |
| Guardian trust loss | capability absent or compromised | escalate and restrict dependent scope | deny activity dependent on Guardian | `SAFE` | Guardian restored or approved independent protection present |
| prohibited financial path detected | any reachable broker, venue, order, live-capital, or financial credential path | emergency instance restriction | deny all non-diagnostic activity | `SAFE` or `STOPPED` | path removed, evidence preserved, constitutional and security review |
| repeated recovery failure | approved attempt bound exceeded | stop recovery loop | deny reintroduction | `FAILED` | higher-authority plan or retirement |

## 3. Minimum Safe-State Allowlist

Only the following action classes may be permitted, and each still requires identity and authorization:

- verify baseline, identity, configuration, restriction, and integrity;
- receive and enforce stricter protective restriction;
- observe health and required evidence;
- preserve audit-critical evidence;
- read authoritative state for reconciliation without mutation;
- revoke identity, security context, delegation, and secret access;
- perform approved containment;
- execute an approved bounded recovery step;
- run independent validation;
- perform controlled shutdown; and
- communicate an authorized protective or recovery result.

Trading, market access, order handling, portfolio action, capital allocation, prediction, learning, self-evolution promotion, third-party plugin execution, unrestricted configuration change, and expansion of authority are denied.

## 4. Enforcement-Point Catalog

| Enforcement point | Enforces | Independent evidence |
|---|---|---|
| Kernel admission gate | baseline, identity, artifact, Core admission | admission decision and artifact verification |
| Authority Engine | action permission and immediate revocation | CON-002 decision |
| Service Bus admission | producer, message, destination, purpose, expiry | transport admission or rejection |
| Lifecycle transition gate | valid authorized protective state | CON-003 transition result |
| Configuration activation gate | source, authority, validation, protective floor | CON-007 activation result |
| Persistence acceptance gate | version, integrity, concurrency, evidence link | persistence outcome |
| Secret-access gate | subject, purpose, scope, validity, revocation | security access decision |
| Recovery step gate | approved plan, phase, prerequisites, bounds | recovery-step result |
| Instance execution boundary | current authority and restriction state | permitted or denied execution observation |

No single enforcement result substitutes for the others when their boundaries are applicable.

## 5. Release-Authority Matrix

| Restriction class | Repair actor | Independent verifier | Release approver | Final restoration |
|---|---|---|---|---|
| component health or evidence | Recovery Authority | Verification Authority | Guardian within mandate | Lifecycle plus new Authority Engine decision |
| identity or security trust | Security Recovery Authority | independent Security Verifier | Security Authority and Guardian | new security context plus new authority decision |
| authoritative state integrity | state owner under Recovery plan | independent State Integrity Verifier | Recovery Release Authority and Guardian | reconciled state, Lifecycle, new authority decision |
| Authority Engine trust | independent recovery operator | Constitutional/Authority Verifier | Project Owner or delegated Governance Authority | verified baseline and controlled restart |
| Guardian compromise | independent containment operator | Security and Constitutional Verifiers | Project Owner or delegated Protection Authority | new Guardian identity and mandate |
| prohibited financial path | removal operator | Constitutional and Security Verifiers | Project Owner | new baseline approval required |

The repair actor, affected subject, and evidence source controlled solely by them cannot constitute independent verification.

## 6. Release Sequence

1. Preserve restriction and evidence.
2. Confirm trigger containment.
3. Execute the approved repair.
4. Reconcile authoritative state and trust.
5. Perform independent validation.
6. Record release-approver decision.
7. Confirm Guardian release conditions.
8. Execute controlled Lifecycle transition.
9. issue a new Authority Engine decision.
10. Monitor under heightened conditions before `NORMAL`.

Passage of time, restart, repair completion, or disappearance of one alarm does not release a restriction.

## 7. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner | Approved | GOV-008 | 2026-07-24 |
