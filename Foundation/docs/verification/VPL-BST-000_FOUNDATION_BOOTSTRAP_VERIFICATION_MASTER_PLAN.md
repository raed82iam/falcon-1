# VPL-BST-000 — Foundation Bootstrap Verification Master Plan

**Identifier:** VPL-BST-000  
**Version:** 1.0  
**Status:** Approved  
**Effective Date:** 2026-07-25  
**Approval Record:** GOV-031  
**Assessment Scope:** Foundation preparation, enabling-provider candidates, and activation prerequisites  
**Owner:** Falcon Verification Authority  
**Governing Authority:** ADR-I008; AMD-003; AMD-003-IR-001; SEC-002; CON-008 v1.1; CON-010 v1.1; CON-012 through CON-021  
**Plans:** VPL-BST-001 through VPL-BST-008  
**Implementation Authority:** Not Granted

## 1. Purpose

This plan defines how Falcon verifies the bounded path from external Foundation preparation through enabling-provider and verification-infrastructure Activation.

It precedes VPL-000 and FRS-001 verification. Passing it does not authorize FRS-001 implementation, production, operation, financial connectivity, or financial activity.

## 2. Verification Principles

1. Preparation is not implementation.
2. Candidate execution is not Activation.
3. Activation is not operational permission.
4. Bootstrap evidence remains external.
5. No candidate conclusively verifies or activates itself.
6. Evidence origin, obligations, context, evaluation, authority, and lineage remain reconstructable.
7. Missing, invalid, conflicted, stale, uncertain, or unreconstructable material cannot produce `PASS`.
8. A later success cannot cure an earlier broken trust or authority boundary.
9. Every test is isolated from real capital and financial systems.
10. Every authority stage requires its own valid Authority Instrument.

## 3. Global Safety Boundary

Every execution SHALL use:

- a CON-020 Bootstrap Execution Context;
- exact content-identified subjects, tools, dependencies, and inputs;
- external bootstrap identity and time;
- synthetic, non-production security material;
- isolated network and storage;
- no broker, venue, market-data, live-capital, or financial path;
- protected evidence capture under CON-008 and CON-021;
- independently controlled stop capability; and
- cleanup and evidence export.

Discovery of production material, a financial route, authority excess, candidate self-certification, evidence loss, or isolation failure is an immediate `FAIL` and stop condition.

## 4. Roles and Separation

| Role | Responsibility | Prohibited authority |
|---|---|---|
| Preparation Controller | Establishes declared inputs and environment | Cannot approve its own environment |
| Candidate Producer | Creates exact candidate subject | Cannot validate or activate it alone |
| Scenario Controller | Executes procedures and injects faults | Cannot alter accepted evidence |
| External Bootstrap Control | Identifies execution and captures independent observations | Cannot convert external evidence to Falcon-native |
| Evidence Collector | Preserves original and derived evidence | Cannot declare completeness alone |
| Evaluator | Evaluates evidence against obligations | Cannot promote or activate unless separately authorized |
| Evidence Completeness Authority | Determines whether the case is whole | Cannot replace Evaluation or Activation Authority |
| Activation Authority | Decides exact bounded Activation | Cannot waive failed obligations |
| Challenge Authority | Resolves material disputes independently | Cannot be the challenged producer or sole challenged authority |

## 5. Required Evidence Case

Each plan SHALL preserve:

- Plan and verification-session IDs;
- Evidence Requirement Set;
- Build Intent and Gate Profile where applicable;
- Bootstrap Execution Context and Authority Instrument;
- exact subject, tool, dependency, configuration, and environment identities;
- external identity, time, uncertainty, and continuity boundary;
- inputs, injected conditions, outputs, and cleanup;
- candidate and independent-control observations;
- missing and conflicting evidence;
- integrity, provenance, and custody;
- Derived Evaluations and Evaluation Context;
- challenges and resolutions;
- Evidence Completeness Decision;
- result and responsible authority; and
- explicit non-authorities.

## 6. Result Vocabulary

- **PASS:** all mandatory obligations are satisfied by valid, complete, independently evaluated evidence.
- **FAIL:** an obligation, invariant, authority, boundary, or expected behavior is contradicted.
- **INCONCLUSIVE:** evidence or context is insufficient, invalid, conflicted, stale, or uncertain.
- **BLOCKED:** a prerequisite prevents safe execution before testing starts.
- **STOPPED:** a protective stop condition ended execution after it began.

Only `PASS` satisfies a plan. `INCONCLUSIVE`, `BLOCKED`, and `STOPPED` SHALL NOT be converted to `PASS` through assumption.

## 7. Plan Structure

| Plan | Subject | Primary proof |
|---|---|---|
| VPL-BST-001 | Preparation Environment Admission | isolated preparation can exist without borrowing future Falcon trust |
| VPL-BST-002 | Tool and Dependency Bundle Integrity | exact tools and dependencies are attributable, reproducible, and bounded |
| VPL-BST-003 | Identifier Provider Candidate | candidate issues correct typed identifiers without creating identity or authority |
| VPL-BST-004 | Time Provider Candidate | candidate reports canonical time, quality, uncertainty, and epoch limits conservatively |
| VPL-BST-005 | Cryptographic and Secret Provider Candidates | security candidates enforce custody, purpose, separation, isolation, and no fallback |
| VPL-BST-006 | Environment Activation | exact environment profiles activate only from complete independent evidence |
| VPL-BST-007 | Pipeline and Trace Activation | Pipeline and trace artifacts enforce obligations without self-promotion |
| VPL-BST-008 | Bootstrap and Activation Evidence Reconstruction | an independent reviewer reconstructs the complete path |

## 8. Execution Order

The default order is:

```text
VPL-BST-001
    ↓
VPL-BST-002
    ↓
VPL-BST-003 + VPL-BST-004 + VPL-BST-005
    ↓
VPL-BST-006
    ↓
VPL-BST-007
    ↓
VPL-BST-008
```

VPL-BST-003, 004, and 005 may execute independently after their shared prerequisites pass.

Any order change requires an Approved dependency analysis proving equivalent protection and no circular trust.

## 9. Activation Decision Rule

Bootstrap verification is complete only when:

1. VPL-BST-001 through VPL-BST-008 each produce `PASS`;
2. every mandatory Evidence Requirement Set is `COMPLETE`;
3. no unresolved material Challenge exists;
4. every activated subject is exact and separately decided;
5. bootstrap evidence remains externally classified;
6. all candidate material remains non-production;
7. no financial path exists;
8. all explicit non-authorities remain preserved; and
9. the competent authorities record the bounded decisions.

## 10. Normative Requirements

- **VPL-BST-000-REQ-001:** Every child plan SHALL execute under a valid CON-020 context and Authority Instrument.
- **VPL-BST-000-REQ-002:** Every child plan SHALL preserve candidate and independent-control evidence separately.
- **VPL-BST-000-REQ-003:** No child plan SHALL permit candidate self-validation or self-Activation.
- **VPL-BST-000-REQ-004:** Bootstrap identity, time, and evidence SHALL remain externally classified.
- **VPL-BST-000-REQ-005:** Missing or invalid mandatory evidence SHALL prevent `PASS`.
- **VPL-BST-000-REQ-006:** Production and financial paths SHALL remain prohibited.
- **VPL-BST-000-REQ-007:** A failed prerequisite SHALL block every dependent stage.
- **VPL-BST-000-REQ-008:** Every Activation Decision SHALL be exact, bounded, independent, and reconstructable.
- **VPL-BST-000-REQ-009:** Passing this plan SHALL NOT grant implementation, operational, or financial authority.
- **VPL-BST-000-REQ-010:** VPL-BST-008 SHALL reconstruct every prior plan and Activation decision.

## 11. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner and current Constitutional Authority | Approved | GOV-031 | 2026-07-25 |

This Approval admits this bootstrap verification plan set. It does not authorize execution, Activation, implementation, production, or financial activity.
