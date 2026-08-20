# Stage 13 WP-02 through WP-09 Implementation Plan and Pre-Implementation Red Team

Status: OWNER_AUTHORIZED_FOR_IMPLEMENTATION
Authorization basis: Project Owner direction on 2026-08-16 to accept/close WP-01 and complete the remainder of Stage 13.

## Preserved prerequisite

Stage 13 / WP-01 is `ACCEPTED_AND_CLOSED` and remains the independent Falcon-wide AI Kill Control Plane and Safe Core prerequisite.

The remaining Stage 13 implementation SHALL NOT move Kill authority into FSA or into the two Monitor AI subjects.

## Work Package map

- WP-02: FSA identity, jurisdiction, authority ceiling, and exact MSA -> FSA review boundary.
- WP-03: independent Monitor AI evidence, minimum integrity checks, anomaly classification, and Investigation Hold.
- WP-04: forensic preservation, Last Trusted vs Factory Trusted baselines, static and behavioral integrity comparison.
- WP-05: remediation sandbox, targeted repair, governed rollback, and Factory Reset decision requirements.
- WP-06: Controlled Revival, independent recovery validation, new-authority decision requirement, and probation before normal operation.
- WP-07: bounded FSA self-maintenance/evolution eligibility for performance, speed, and accuracy only, with protected-property prohibition and no self-deployment.
- WP-08: Owner-facing FSA control request boundary, FSA no-direct-Internet invariant, and exact MSA -> FSA proposal/evidence admission semantics.
- WP-09: integrated adversarial verification, separation-of-powers hardening, FCR-0012/FCR-0030 disposition readiness, and Stage 13 closure readiness after executable validation.

## Ownership separation

`Foundation.SelfAwareness` may observe, assess, classify, preserve structured evidence, produce governance recommendations, and evaluate readiness. It SHALL NOT become Kill authority, Authority issuance, Guardian, Lifecycle executor, release authority, or Application business authority.

`Foundation.Authority` retains the independent Kill Control Plane from WP-01.

Application business/domain judgment remains Application-owned.

## Required invariants

```text
SELF_AWARENESS != AUTHORITY
FSA != ITS_KILL_AUTHORITY
MONITOR_AI != KILL_AUTHORITY
MONITOR_DISAGREEMENT != SAFE
OWNER_SILENCE != OWNER_APPROVAL
TIMER_EXPIRY != NEW_AUTHORITY
FSA_REVIEW != PRODUCTION_ADOPTION
RESTART != RECOVERY
REPAIRED != TRUSTED
TESTED != RELEASED
HASH_MATCH != AUTOMATIC_BEHAVIORAL_TRUST
LAST_TRUSTED_BASELINE != FACTORY_TRUSTED_BASELINE
FACTORY_RESET != KILL
FACTORY_RESET != ROLLBACK
FSA_DIRECT_INTERNET_ACCESS = FORBIDDEN
APPLICATION_BUSINESS_JUDGMENT = APPLICATION_OWNED
```

## Pre-implementation Red Team

The implementation/verifier must challenge at minimum:

1. FSA attempting to alter its own Kill/control boundary;
2. one monitor SAFE and one SUSPICIOUS;
3. two nominal monitors sharing the same independence identity;
4. FSA goal drift;
5. FSA authority/permission drift;
6. FSA architecture drift;
7. missing or manipulated evidence;
8. refusal/interference with authorized investigation;
9. static hash match with behavioral-state mismatch;
10. tampered or absent trusted baseline;
11. rollback against a Factory baseline and Factory Reset against a Last Trusted baseline;
12. Factory Reset without forensic preservation;
13. Controlled Revival without independent recovery validation;
14. Controlled Revival without release authorization/new authority where required;
15. direct transition from repaired state to normal operation;
16. FSA evolution proposal touching goals, jurisdiction, authority, Owner controls, monitoring, audit, containment, security, or core architecture;
17. FSA self-deployment;
18. MSA submission with changed candidate after review;
19. incomplete lower-tier evidence/provenance;
20. MSA submission attempting to convert Application recommendation into production adoption;
21. FSA direct Internet request;
22. Owner-control request whose actor is the target FSA;
23. timer expiry or Owner silence treated as authority;
24. Application business semantics leaking into FSA governance code;
25. recovery or release API leaking into the WP-01 Kill Control Plane.

## Implementation authority

```text
STAGE13_WP01 = ACCEPTED_AND_CLOSED
STAGE13_WP02_WP09_IMPLEMENTATION_AUTHORITY = OWNER_GRANTED_2026_08_16
STAGE13_FINAL_OWNER_CLOSURE = NOT_GRANTED_BY_THIS_PLAN
DEPLOYMENT_AUTHORITY = NOT_GRANTED
PRODUCTION_RUNTIME_ACTIVATION = NOT_GRANTED
```
