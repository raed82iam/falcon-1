# FSATS Part 1-NG — Implementation Architecture, Structural Materialization and Build Readiness

**Status:** `DESIGN_CANDIDATE / NOT_OWNER_ACCEPTED / NOT_CLOSED`  
**Branch:** `application-development`  
**Authority Type:** `DESIGN ONLY`  
**Implementation Authority:** `NOT GRANTED`  
**Runtime Authority:** `NOT GRANTED`  
**Historical Part 1:** `PRESERVED / NOT CURRENT BASELINE`  
**Candidate Identity:** `PART 1-NG` path retained for continuity; Owner has selected current working identity `Part 1`

---

## 1. Purpose

Part 1 converts the Owner-accepted and closed Part 0 design into an exact, code-ready implementation architecture without writing production implementation or granting runtime authority.

Part 0 established what FSATS must be. Part 1 establishes how that accepted design, together with later explicit Owner-directed prospective corrections, will be materialized into independently buildable Application-owned projects, packages, modules, manifests, contracts, schemas, verifiers, dependency gates and implementation Work Packages.

```text
PART 0 = ACCEPTED DESIGN BASELINE
PART 1 = CODE-READY MATERIALIZATION + EXPLICIT PROSPECTIVE OWNER CORRECTIONS
FUTURE IMPLEMENTATION = SEPARATELY AUTHORIZED EXECUTION
```

Part 1 SHALL NOT silently redesign Part 0. Any prospective semantic difference from accepted Part 0 SHALL be explicit, attributable to a later Owner decision, traced to affected Foundation/FCR boundaries and freshly reviewed before Owner acceptance.

---

## 2. Historical Part 1 and Current Candidate

A historical Part 1 implementation is preserved under archive and remains a valid historical Owner-closed record. It is not the current P0-NG implementation baseline.

Historical Part 1 materially predates the current topology and contract baseline. Historical artifacts MAY supply lessons and reusable evidence but SHALL NOT be treated as a current implementation baseline unless a future explicit compatibility decision proves an individual artifact still conforms.

No historical record is rewritten.

---

## 3. Part Boundary

### In Scope

Part 1 SHALL define:

1. exact Foundation-integration architecture and current capability/FCR baseline before physical build topology is closed;
2. exact repository / solution / project / package topology for current FSATS Applications and system-level operational roles;
3. canonical Application-owned implementation primitives required by current design without cloning Foundation semantics;
4. Application identity, Manifest and lifecycle materialization plan;
5. exact per-Application implementation decomposition for all current MSAs, LSAs, operational controllers and eligible CSA boundaries;
6. exact FSATS-wide resource-management materialization through FSARM;
7. exact materialization plan for all governed cross-Application contract families, including any contract impact created by FSARM;
8. Foundation capability-consumption and FCR gating model;
9. verifier, architecture-test, security-test, failure-test and performance-test architecture;
10. implementation dependency DAG, parallelization rules and future implementation authorization slices;
11. integrated implementation-readiness and closure evidence.

### Explicitly Out of Scope

Part 1 SHALL NOT authorize or perform:

- production implementation code;
- Foundation code changes;
- runtime route activation;
- external provider connectivity;
- broker connectivity;
- credential use;
- Paper, Shadow, Tiny Live or Live operation;
- deployment;
- autonomous promotion;
- leverage, derivatives or additional markets;
- rewriting accepted Part 0 historical records;
- reopening or replacing historical Part 1 records.

---

## 4. Foundation-First Materialization Rule

No Application component, controller, contract, runtime path or project topology may be considered implementation-ready until its Foundation dependency, boundary, binding method, current capability state and fail-closed behavior are identified.

For every material unit, Part 1 SHALL establish in this order:

```text
CURRENT OWNER / APPLICATION NEED
 -> CURRENT FOUNDATION AUTHORITY / CAPABILITY
 -> GOVERNING SPEC / CONTRACT / ADR / FCR
 -> FOUNDATION-SIDE BOUNDARY
 -> APPLICATION-SIDE BINDING
 -> CURRENT AVAILABILITY / BLOCKER
 -> FAIL-CLOSED BEHAVIOR
 -> ONLY THEN PHYSICAL MATERIALIZATION
```

If the required Foundation information or capability is missing, partial or incompatible, the governed FCR channel is mandatory. No Application-local substitute may be invented for a Foundation-owned responsibility.

---

## 5. Candidate Work-Package Direction

The Part 1 decomposition SHALL be revised so Foundation integration is established before physical topology and so FSARM is a first-class system-wide resource-management concern.

The exact WP count is not fixed. A separate WP exists only where the scope has distinct responsibility, ownership/boundary, dependencies, independently verifiable output and independent closure value.

The active candidate decomposition is defined in `01_PART1NG_WORK_PACKAGE_DECOMPOSITION.md` and may be further remediated during the current Owner review cycle.

---

## 6. Required Output Standard for Every WP

Every Part 1 WP SHALL produce, where applicable:

- exact responsibility and non-responsibility boundary;
- governing Part 0 sources and any later controlling Owner correction;
- authoritative owner;
- Foundation Integration Profile;
- input dependencies;
- exact output artifacts;
- project/package/module placement;
- public/internal boundary;
- interfaces / schemas / data ownership;
- authority and permission requirements;
- state ownership and persistence assumptions;
- time / freshness / version / epoch semantics;
- failure / degraded / recovery behavior;
- security and isolation requirements;
- resource / performance requirements;
- Foundation capability dependencies;
- FCR gates;
- positive and negative verifier cases;
- acceptance criteria;
- implementation non-authorities;
- evidence required for closure.

A WP SHALL fail closed where a required authority-bearing identity, Foundation capability or contract cannot yet be resolved.

---

## 7. Current FSATS Topology Input and Prospective FSARM Correction

The accepted Part 0 topology remains preserved as historical accepted design evidence. Part 1 now carries a later explicit Owner-directed prospective resource-management correction that SHALL be freshly reviewed before it can become the accepted current design.

Current Application topology remains:

```text
FSATS SYSTEM BOUNDARY
  MSA = 0
  LSA = 0

FALCON SELF-AWARE TRADING APPLICATION
  MSA = 1
  LSA = 13

FSAPMA
  MSA = 1
  LSA = 6
  PROVIDER CONTROLLER = operational controller inside P-LSA-04

FALCON TRADING GUARDIAN APPLICATION
  MSA = 1
  LSA = 4

FSTSIMA
  MSA = 1
  LSA = 8
```

Shared Web and Shared Communication remain independent Shared Applications outside the FSATS ownership boundary while remaining governed counterparties where applicable.

### 7.1 FSARM — Falcon Self-Aware Resource Management

Part 1 prospectively replaces the Trading-only TARC future model with:

```text
FSARM = FALCON SELF-AWARE RESOURCE MANAGEMENT
```

FSARM is the single FSATS-wide operational resource-management authority for the Trading System resource envelope, subject to Foundation reconciliation under FCR-0031.

FSARM SHALL coordinate resource availability across:

- Falcon Self-Aware Trading Application;
- FSAPMA;
- Falcon Trading Guardian Application;
- FSTSimA.

FSARM is not an MSA, LSA or CSA. Its final structural identity and exact Foundation admission/binding model remain subject to the Foundation reconciliation required by FCR-0031. Part 1 SHALL NOT silently convert the non-owning FSATS system boundary into an Application or hidden runtime principal to accommodate FSARM.

T-LSA-13 remains Trading-side resource awareness/evaluation only. It does not own FSATS-wide operational resource control.

```text
T_LSA13 != FSARM
FSARM != FOUNDATION_RESOURCE_GOVERNANCE
```

### 7.2 FSARM Primary Operating Principle

FSARM exists first to optimize and protect use of resources already available to the Trading System.

```text
INTERNAL_REDISTRIBUTION_FIRST
FOUNDATION_ADDITIONAL_REQUEST_SECOND
```

FSARM SHALL maintain an attributable FSATS-wide resource picture including, as applicable:

- current effective allocation;
- current consumption;
- minimum safe / survival requirement;
- desired allocation;
- reclaimable capacity;
- pressure and urgency evidence;
- protection/live-critical classification;
- degradation/shedding eligibility;
- restoration eligibility.

Under pressure, overload, protection events or crisis conditions, FSARM MAY, within governed policy and actual granted authority:

- reserve capacity;
- rebalance capacity;
- reclaim eligible capacity;
- throttle lower-priority work;
- shed or suspend eligible deferrable work;
- reassign capacity to higher-priority obligations;
- restore capacity in controlled stages after recovery evidence exists.

Example: if Guardian requires additional CPU/memory during a crisis and FSTSimA is consuming reclaimable capacity not needed for current live protection/trading continuity, FSARM may reduce or pause eligible FSTSimA workload and reassign that already-available capacity to Guardian without first requesting additional Foundation resources.

FSARM SHALL NOT use a permanently fixed Application ranking as a substitute for current consequence-aware evidence. Priority is based on the active obligation, consequence of starvation, minimum safe requirement, reclaimability and governed policy.

### 7.3 Additional Foundation Resource Request

If internal redistribution, throttling and eligible shedding cannot satisfy the proven required minimum or urgent need, FSARM MAY aggregate the remaining deficit and request additional resource capacity from Foundation Resource Governance when the governed Foundation request boundary is available and authorized.

```text
PROVEN_NEED
 - SAFE_INTERNAL_REDISTRIBUTION
 = REMAINING_DEFICIT

REMAINING_DEFICIT > 0
 -> FSARM MAY REQUEST ADDITIONAL FOUNDATION RESOURCE
```

FSARM SHALL request the evidenced remaining need, not automatically the original gross demand.

```text
REQUESTED_RESOURCE != GRANTED_RESOURCE
```

Foundation remains sole owner of total-resource truth and final grant, partial grant, cap, deny, reduce, revoke, reclaim, rebalance and restore authority.

FSARM does not gain Trading, Risk, Guardian command, provider, simulation, lifecycle, FSA, security or Owner authority by controlling resource distribution.

Every independent Application remains identifiable and accountable even when FSARM coordinates resource redistribution across the Trading System.

### 7.4 Foundation/FCR Status

FCR-0031 is the controlling current Foundation reconciliation request for FSARM and is `ACCEPTED_FOR_PLANNING / Waiting On FOUNDATION` at the time of this semantic update.

FCR-0007 and FCR-0010 have been prospectively reconciled by Foundation so prior TARC-only future assumptions are not treated as the future implementation target where they conflict with FCR-0031.

Part 1 resource-integration closure remains blocked until Foundation produces the required FSARM-compatible design evidence and Application verification is completed.

---

## 8. Foundation Consumption Principle

Part 1 SHALL classify every Foundation dependency across at least these independent dimensions:

```text
DESIGN_TIME_SPEC_AVAILABLE
BUILD_TIME_ARTIFACT_AVAILABLE
RUNTIME_CAPABILITY_AVAILABLE
RUNTIME_AUTHORITY_GRANTED
```

A design-time specification does not prove build-time artifact availability. Build-time availability does not prove runtime capability. Runtime capability does not grant runtime authority.

Part 1 SHALL not copy Foundation source, create Application-owned replacements for Foundation responsibilities, or bind to moving Foundation branch heads.

FCR-0016 remains a hard gate for canonical cross-workstream Foundation artifact publication/consumption where exact build-time consumption is required.

---

## 9. Implementation Slice Principle

Part 1 SHALL not produce one giant future implementation authorization.

Future implementation slices SHALL each have:

- an exact accepted design basis;
- all required Foundation dependencies available or explicitly fail-closed;
- independent testability;
- no unauthorized runtime routes;
- no implied Paper/Live authority;
- separate authorization, implementation, review and closure capability.

---

## 10. Parallelization Principle

Parallel work is permitted only where ownership and dependency edges prove independence.

Application decomposition may proceed in parallel after common Foundation/identity/topology prerequisites are stable, but FSARM is cross-cutting and its resource interfaces, minimum floors, priority evidence and reclaimability declarations SHALL be consistently represented across all four FSATS Applications before integrated readiness can close.

---

## 11. Review and Owner Lifecycle

This FSARM update is a semantic change after prior Part 1 candidate freeze/review evidence.

Therefore the prior freeze/reviews are historical evidence only for the old candidate and SHALL NOT be presented as current PASS evidence for this changed semantic scope.

Required lifecycle:

```text
CURRENT SEMANTIC REMEDIATION
 -> NEW SEMANTIC FREEZE
 -> FRESH ARCHITECTURE / CONSISTENCY REVIEW
 -> FRESH RED TEAM
 -> OWNER REVIEW
 -> EXPLICIT OWNER ACCEPTANCE / CLOSURE
```

Owner acceptance of Part 1 design SHALL NOT itself authorize implementation.

---

## 12. Candidate Success Condition

Part 1 is ready for Owner acceptance only when it can answer, without ambiguity:

1. What exact Foundation boundaries are used before physical topology is built?
2. What exact projects/packages/modules will exist?
3. Which Application owns every project and state boundary?
4. Where is each of the 13 + 6 + 4 + 8 LSA responsibilities implemented?
5. What is FSARM's exact structural identity and Foundation binding?
6. How does each Application report resource need, minimum safe demand, pressure, reclaimability and restoration evidence to FSARM?
7. How does FSARM perform internal redistribution first and request only proven remaining deficit from Foundation second?
8. How are Application identities and manifests materialized?
9. How are all governed contract families represented and verified after FSARM impact reconciliation?
10. Which Foundation capabilities are usable now, which require verification, and which remain blocked?
11. What tests/verifiers prove architecture, authority, security, isolation, resource redistribution, failure and performance behavior before runtime?
12. What is the exact build/dependency DAG?
13. Which future implementation slices can proceed independently and which must wait?
14. What is explicitly unauthorized?
15. What evidence is required before any future implementation WP can be Owner-authorized?

Until all answers are complete, Part 1 remains a design candidate.
