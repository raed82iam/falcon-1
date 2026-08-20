# FSA Self-Repair and Self-Evolution Impact Assessment

**Status:** Approved Assessment  
**Approval Record:** GOV-061  
**Assessment Date:** 2026-07-27  
**Target Package:** AMD-004 v0.1 candidate  
**Stage 1 Authority:** Not Granted

## 1. Executive Summary

AMD-004, ADR-I009, and AWR-001 v2.0 are not Approved. Therefore the correct governance treatment is a controlled revision of the candidate package to v0.2, not a new Accepted ADR, silent amendment, or supersession.

The extension is constitutionally compatible when two operations remain strictly separate:

- **Self-Repair:** restoration of a previously Approved trusted state using an Approved playbook and authority.
- **Self-Evolution:** creation and validation of a new non-authoritative candidate state in isolation, followed by independent validation and Project Owner approval before admission.

The extension does not reopen FSA naming, hierarchy, or Application-business boundaries.

## 2. Affected Candidate Documents

| Document | Current Status | Required Treatment |
|---|---|---|
| AMD-004 README and Changelog | Approved by GOV-061; activation deferred | v0.2 scope recorded |
| ADR-I009 | Accepted by GOV-061; activation deferred | decision extended before approval |
| AWR-001 v2.0 successor design | Approved by GOV-061; not effective | normative repair/evolution sections included |
| Awareness diagrams and matrices | Approved by GOV-061 | repair/evolution flows and authority matrix included |
| Migration and prerequisite register | Approved by GOV-061; execution deferred | playbook, candidate, approval-center, Digital City, and deployment prerequisites included |
| Acceptance plan | Candidate | add repair/evolution boundary cases |
| Constitutional report | Approved by GOV-061 | review extended |
| Owner Approval Package | Completed by GOV-061 | decisions and consequences recorded |
| Readiness result | Architecture approved; activation deferred | reassessed after approval |

MSA, LSA, and CSA specifications require no responsibility change. They remain outside Foundation repair scope.

## 3. Existing Approved Documents Affected by Future Alignment

### EVO-001 — Self-Maintenance and Evolution

- **Status:** Approved
- **Relevant sections:** maintenance/evolution distinction, requirements around repair, candidate change, validation, promotion, and post-change Fitness.
- **Impact:** ADR-I009 and AWR-001 v2.0 must remain compatible. EVO-001 remains the higher existing behavioral requirement where applicable; later cross-reference alignment may qualify FSA-specific ownership.
- **Method:** do not edit silently; create a versioned successor only if approval review finds substantive conflict.

### AUT-002, CON-011, FDN-005, ADR-F008

- **Status:** Approved
- **Impact:** FSA may supervise and repair Guardian technical readiness through Approved procedures but cannot change Guardian authority, mandate, restriction ownership, or release rules.
- **Method:** preserve current authority; later Guardian architecture remains a separate decision.

### OPS-003 and SYS-002

- **Status:** Approved
- **Impact:** repair orchestration must preserve Recovery ownership and Lifecycle authority. FSA awareness cannot silently become execution authority.

### ADR-I007 and PIPE-001

- **Status:** Accepted/Approved
- **Impact:** candidate evidence, independent evaluation, completeness, and promotion authority must remain separate. FSA-created candidates cannot self-promote.

### PLG-001 and STD-012

- **Status:** Approved
- **Impact:** candidate versions require distinct identity, passport/admission evidence, permissions, isolation, health, recovery, and removal behavior where applicable.

### GOV-AUT-001 and SEC-002

- **Status:** Approved
- **Impact:** jurisdiction, acceptance, reliance, Claims, Challenges, and independent validation constrain repair and evolution.

## 4. Authority Implications

### Self-Repair

Autonomous repair is permitted only when:

- the target is Foundation-owned;
- the restored state is already Approved, trusted, compatible, and not revoked;
- an Approved versioned playbook permits the exact action;
- the authority instrument covers target, scope, time, actions, retries, and consequences;
- Guardian, Security, Lifecycle, Recovery, and other competent authorities retain jurisdiction;
- post-repair verification is independent where required.

Self-Repair cannot introduce a new state or reinterpret an existing baseline.

### Self-Evolution

FSA may research, design, code, build, and test a candidate only in an Approved isolated environment. This is candidate-development authority, not admission or production authority.

Independent validation and explicit Project Owner approval are mandatory before any newly created state may enter Falcon.

### FSA Self-Evolution

A candidate successor to FSA requires:

- independent evaluator and integrity authority;
- independent authority-boundary validation;
- Guardian and protection continuity;
- Owner approval;
- rollback to the prior trusted FSA;
- inability of active FSA to approve its successor.

## 5. Security Implications

- Candidate environments must have no production authority, business-state access, live Guardian mandate, or uncontrolled external input.
- Repair playbooks must be signed, versioned, bounded, revocable, and least-privileged.
- Repair/evolution evidence must exclude reusable secrets and sensitive Application content.
- Candidate source and artifacts must be attributable, reproducible, scanned, removable, and non-authoritative.
- Failure must not trigger insecure fallback or unbounded retry.

## 6. Guardian Impact

FSA may supervise Guardian technical readiness through independent evidence and safe tests.

FSA may restore an Approved Guardian runtime, configuration, standby, or version under an Approved playbook.

FSA may create a candidate Guardian technical version in isolation.

FSA may not:

- change Guardian jurisdiction or protective mandate;
- release Guardian restrictions;
- approve or activate its Guardian candidate;
- disable independent protection;
- finalize Guardian placement or complete architecture.

## 7. Constitutional Review

No constitutional amendment is required.

The extension implements governed self-maintenance and evolution while preserving:

- Vision and Constitution supremacy;
- bounded and revocable authority;
- separation of awareness, execution, validation, and approval;
- independent protection;
- evidence and historical integrity;
- safe failure;
- Application business ownership.

## 8. Stage 1 Impact

Stage 1 remains blocked.

Architecture approval alone does not authorize:

- creation of production code;
- candidate development execution;
- repair execution;
- Sandbox or Digital City execution;
- deployment;
- component replacement;
- operational activation.

Separate authority instruments, environment approval, playbooks, contracts, verification, and Stage 1 authorization remain required.

## 9. Required Migration and Approval Actions

1. Approve revised ADR-I009 and AWR-001 v2.0 together.
2. Approve the Repair Authority Matrix.
3. Approve the candidate and Owner-decision lifecycles.
4. Approve the Owner Communication and Approval Center specification.
5. Approve Guardian readiness supervision only as an FSA-side boundary.
6. Authorize later creation of repair-playbook contracts/catalogs.
7. Authorize later candidate-environment and Digital City governance work.
8. Preserve separate Stage 1 authority.
