# FSATS Part 1-NG — Dependency, FCR and Parallelization Model

**Status:** `DESIGN_CANDIDATE / NOT_OWNER_ACCEPTED / NOT_CLOSED`  
**Implementation Authority:** `NOT GRANTED`

## 1. Dependency Principle

Part 1 SHALL be planned as a dependency graph, not a fixed linear checklist.

A WP may begin in parallel only when all authority, semantic and dependency prerequisites for that WP are satisfied independently.

Foundation integration must be established before physical topology is considered implementation-ready.

## 2. Remediated Candidate Dependency DAG

```text
P1-A  Authority / baseline / compatibility lock
   |
   +--> P1-B  Foundation integration / capability / FCR baseline
           |
           +--> P1-C  Repository / project / package topology
                   |
                   +--> P1-D  Canonical Application primitives
                           |
                           +--> P1-E  Identity / Manifest / lifecycle materialization
                                   |
                                   +--> P1-F  Trading decomposition
                                   +--> P1-G  FSAPMA decomposition
                                   +--> P1-H  Guardian decomposition
                                   +--> P1-I  FSTSimA decomposition
                                   |
                                   +--> P1-J  FSARM resource management

P1-F/G/H/I + P1-J + P1-E + P1-B
          -----------------------> P1-K  Governed contract/FIL/event/route materialization

P1-A through K ------------------> P1-L  Verification + integrated build/readiness gate
```

This is a design dependency model. It grants no WP implementation authority.

## 3. FSARM Cross-Cutting Dependency

FSARM is not a Trading-only branch dependency. It is cross-cutting across the four independent Applications inside FSATS.

Every Application decomposition SHALL provide FSARM with attributable resource information including, as applicable:

```text
CURRENT_CONSUMPTION
CURRENT_ALLOCATION
MINIMUM_SAFE_RESOURCE
DESIRED_RESOURCE
RECLAIMABLE_RESOURCE
PRESSURE
URGENCY / CONSEQUENCE EVIDENCE
DEGRADATION / SHEDDING ELIGIBILITY
RESTORATION EVIDENCE
```

FSARM SHALL use this evidence to perform governed internal redistribution before escalating to Foundation.

```text
INTERNAL_REDISTRIBUTION_FIRST
FOUNDATION_ADDITIONAL_REQUEST_SECOND
```

If safe internal redistribution leaves a remaining deficit:

```text
REMAINING_DEFICIT > 0
 -> FSARM RESOURCE REQUEST TO FOUNDATION RESOURCE GOVERNANCE
```

The requested amount is the evidenced remaining need after safe internal redistribution, not automatically the gross originating demand.

## 4. Parallel Lanes

After P1-E is sufficiently stable, the following design lanes are naturally parallel because they retain separate Application ownership:

- P1-F Trading;
- P1-G FSAPMA;
- P1-H Guardian;
- P1-I FSTSimA.

P1-J FSARM runs cross-cutting against these lanes. Each lane may refine its own resource profile independently, but integrated FSARM closure requires all four resource profiles plus the Foundation reconciliation under FCR-0031.

They converge at P1-K through governed contracts rather than direct hidden coupling.

## 5. Current Foundation/FCR Planning Matrix

| FCR | Current planning meaning for Part 1 | Part 1 treatment |
|---|---|---|
| FCR-0004 | Existing Stage 5 route capability requires Application binding/reconciliation | P1-H/K verify Guardian route binding |
| FCR-0005 | Existing Stage 5 communication/delivery capability requires Application binding/reconciliation | P1-G/K verify normalized-data delivery binding |
| FCR-0006 | Existing Stage 5 event/evidence/replay capability requires Application verification | P1-I/K/L verify replay/evidence semantics |
| FCR-0007 | Stage 6 WP-06 resource request/decision need remains valid, but prior TARC-only requester identity is prospectively superseded where it conflicts with FCR-0031 | P1-J binds future request semantics to FSARM-compatible Foundation design; fail closed until reconciled/implemented/verified |
| FCR-0008 | Stage 12 research-only egress | Awareness research runtime remains disabled until available/verified |
| FCR-0009 | Stage 11 transport QoS/deadline governance | P1-K/L design deadlines/QoS; runtime remains blocked |
| FCR-0010 | Stage 6 WP-05..08 pressure/preemption/request/reclamation/load-shedding family; TARC-only future assumptions are superseded-in-part pending FCR-0031 reconciliation | P1-J/L consume only reconciled FSARM-compatible semantics; stale TARC-specific implementation assumptions prohibited |
| FCR-0011 | Stage 12 non-Live isolation/egress enforcement | P1-I/L require explicit non-Live fail-closed gate |
| FCR-0012 | Stage 13 FSA/Owner bounded evolution control plane | P1-F/G/H/I/B/L may design proposal/evidence boundaries; autonomous promotion disabled |
| FCR-0013 | Stage 12 provider egress / credential-reference security | P1-G external provider adapters remain non-operational design only |
| FCR-0014 | Stage 12 broker execution egress / credential-reference security | P1-F execution adapters remain non-operational design only |
| FCR-0016 | Stage 14 canonical Foundation artifact publication / Application consumption | P1-B/C/E cannot authorize canonical build wiring until available/verified |
| FCR-0030 | Stage 13 MSA -> FSA implementation-facing interface/transport binding remains unresolved | P1-B/E/F/G/H/I/L keep exact MSA->FSA runtime binding fail closed pending Foundation evidence |
| FCR-0031 | Owner-directed FSARM aggregate resource model accepted for planning; Foundation reconciliation pending | P1-B/J/L treat FSARM as current prospective resource architecture and block final integration closure until Foundation reconciliation/Application verification |

At the time of this update, no material FCR in this matrix requires an immediate Application-side response. FCR states SHALL still be re-read live before every FSATS response and before any dependent closure action.

## 6. Foundation Consumption Rules

Part 1 SHALL distinguish independently:

```text
DESIGN_TIME_SPEC_AVAILABLE
BUILD_TIME_ARTIFACT_AVAILABLE
RUNTIME_CAPABILITY_AVAILABLE
RUNTIME_AUTHORITY_GRANTED
```

These are not interchangeable.

An accepted Foundation specification may be sufficient for design-time conformance while build-time artifact consumption or runtime capability remains unavailable.

No design shall silently bridge those states with copied source, local reimplementation, unpinned binaries or moving branch references.

## 7. FSARM / Foundation Separation

FSARM operates only within actual admitted resource authority.

```text
FSARM_INTERNAL_DISTRIBUTION != FOUNDATION_TOTAL_RESOURCE_TRUTH
FSARM_REQUEST != FOUNDATION_GRANT
FSARM_PRIORITY_EVIDENCE != FOUNDATION_TECHNICAL_CRITICALITY
```

Foundation retains total-resource truth, protected Foundation floors and final grant/cap/deny/reduce/revoke/reclaim/rebalance/restore authority.

Part 1 SHALL not assume that FCR-0031 planning acceptance equals implemented aggregate resource-envelope support.

## 8. Failure of Dependency Resolution

If a required dependency cannot be proved:

```text
DEPENDENCY = UNRESOLVED
AFFECTED OUTPUT = FAIL_CLOSED
IMPLEMENTATION SLICE = NOT READY
```

The result SHALL NOT be converted to a mock production dependency merely to preserve schedule.

Test doubles are permitted only inside explicitly non-authoritative test/simulation scope and must be labeled as such.

For FSARM specifically, if the Foundation aggregate allocation/accounting/isolation model remains unresolved, Part 1 may continue design study but SHALL NOT claim resource integration is implementation-ready.

## 9. Historical Part 1 and Prior Candidate Review Rule

Historical Part 1 contains potentially useful implementation evidence, but reuse is artifact-specific.

Likewise, prior Part 1 candidate freeze/review records remain historical evidence for the exact earlier semantic candidate. The FSARM semantic change requires a new freeze and fresh Architecture/Consistency + Red-Team reviews before Owner acceptance.

```text
PRIOR_PASS != CURRENT_FSARM_CANDIDATE_PASS
```

## 10. Closure Rule

Part 1 shall not close merely because every candidate WP has a document.

It closes only if P1-L proves that the resulting implementation architecture is complete, internally coherent, Foundation-honest, FSARM-consistent, testable, parallelizable where safe, fail-closed where blocked, and decomposable into separately authorizable implementation slices.
