# FSATS Part 1 — FSARM Owner-Directed Semantic Remediation Record

**Status:** `SEMANTIC REMEDIATION RECORDED / NOT OWNER ACCEPTED / NOT CLOSED`  
**Branch:** `application-development`  
**Implementation Authority:** `NOT GRANTED`  
**Runtime Authority:** `NOT GRANTED`

## 1. Purpose

This record preserves the Project Owner's current Part 1 resource-management direction so it cannot be lost, collapsed back into the prior Trading-only TARC model, or reconstructed from memory.

This record does not rewrite accepted Part 0 history. It documents a later prospective semantic correction being carried in Part 1 and therefore requires the normal fresh review lifecycle before final Owner acceptance.

## 2. Owner-Directed Resource Model

The future-facing Trading-only TARC model is replaced in the current Part 1 candidate by:

```text
FSARM = FALCON SELF-AWARE RESOURCE MANAGEMENT
```

FSARM is intended to be the single FSATS-wide operational resource-management authority coordinating resource use across:

- Falcon Self-Aware Trading Application;
- Falcon Self-Aware Provider Management Application (FSAPMA);
- Falcon Trading Guardian Application;
- Falcon Self-Aware Trading Simulation Application (FSTSimA).

FSARM is not an MSA, LSA or CSA.

T-LSA-13 remains Trading resource awareness/evaluation only.

```text
T_LSA13 != FSARM
```

The exact structural identity/admission/binding of FSARM remains subject to Foundation reconciliation under FCR-0031. The FSATS non-owning system boundary SHALL NOT be silently converted into an Application or hidden runtime principal merely to host FSARM.

## 3. Primary Resource-Control Objective

FSARM's primary purpose is to control and dynamically redistribute already-available FSATS resources according to the current importance and consequence of active obligations.

The governing sequence is:

```text
INTERNAL_REDISTRIBUTION_FIRST
FOUNDATION_ADDITIONAL_REQUEST_SECOND
```

A local resource shortage does not automatically cause a Foundation request.

FSARM first determines whether the need can be satisfied safely from existing FSATS resources through governed:

- reserve;
- redistribution;
- rebalance;
- reclaim;
- throttle;
- shedding;
- temporary suspension of eligible lower-priority work;
- staged restoration after recovery.

## 4. Required Per-Application Resource Evidence

Each FSATS Application SHALL expose attributable resource information to FSARM as applicable, including:

```text
CURRENT_ALLOCATION
CURRENT_CONSUMPTION
MINIMUM_SAFE_RESOURCE
DESIRED_RESOURCE
RECLAIMABLE_RESOURCE
PRESSURE
URGENCY / CONSEQUENCE_OF_STARVATION
DEGRADATION / SHEDDING ELIGIBILITY
RESTORATION EVIDENCE
```

Exact Foundation-owned semantics SHALL be consumed rather than reimplemented where applicable.

## 5. Dynamic Priority, Not Permanent Application Rank

FSARM SHALL NOT use one permanently fixed Application ranking as the sole resource decision rule.

It SHALL evaluate the current obligation and evidence, including:

- live/protection criticality;
- consequence of starvation;
- minimum-safe resource floor;
- current pressure;
- reclaimability;
- active crisis/protection state;
- current dependency state;
- admitted resource policy.

Design intent is to preserve capital protection, crisis handling, reconciliation, open-position safety and required operational-data paths before simulation, experimentation, discovery, analytics, research and other deferrable workloads when current evidence justifies that ordering.

## 6. Guardian / FSTSimA Crisis Example

If Guardian requires additional resource capacity during a crisis while FSTSimA is consuming reclaimable resource not required for current live protection/trading continuity:

```text
GUARDIAN REPORTS ATTRIBUTABLE CRISIS RESOURCE NEED
 -> FSARM EVALUATES FSATS CURRENT RESOURCE PICTURE
 -> FSARM IDENTIFIES ELIGIBLE FSTSIMA RECLAIMABLE CAPACITY
 -> FSARM REDUCES / PAUSES ELIGIBLE FSTSIMA WORK
 -> FSARM REALLOCATES EXISTING CAPACITY TO GUARDIAN
 -> NO FOUNDATION REQUEST IF THE NEED IS NOW SATISFIED
```

This resource action does not:

- make FSARM Guardian authority;
- make Guardian Foundation Resource Governance;
- allow FSARM to alter FSTSimA validation/evidence truth;
- create new resources;
- create business authority from resource priority.

## 7. Additional Foundation Resource Request

If safe internal redistribution cannot satisfy the required need, FSARM may request additional Foundation resources through the governed Foundation Resource Governance boundary when that capability is available and authorized.

The request SHALL represent the evidenced remaining deficit after internal resource optimization, not automatically the original gross demand.

```text
GROSS_REQUIRED_RESOURCE
 - SAFE_INTERNAL_RESOURCE_AVAILABLE
 = REMAINING_DEFICIT

IF REMAINING_DEFICIT > 0
 -> FSARM MAY REQUEST REMAINING DEFICIT FROM FOUNDATION
```

The following invariant remains mandatory:

```text
REQUESTED_RESOURCE != GRANTED_RESOURCE
```

Foundation remains sole owner of total-resource truth and final resource grant/partial/cap/deny/reduce/revoke/reclaim/rebalance/restore authority.

## 8. FSARM Non-Authorities

FSARM SHALL NOT own or gain authority over:

- Trading decisions;
- Unified Risk decisions;
- Guardian protection commands;
- FSAPMA provider/data truth;
- FSTSimA validation truth;
- Application lifecycle/admission;
- FSA governance;
- security authority;
- Owner authority.

```text
FSARM_RESOURCE_CONTROL != BUSINESS_AUTHORITY
FSARM != FOUNDATION_RESOURCE_GOVERNANCE
```

## 9. Foundation / FCR Binding

FCR-0031 is the controlling current Foundation reconciliation request for this prospective architecture.

At the time of this record:

```text
FCR-0031 = ACCEPTED_FOR_PLANNING / Waiting On FOUNDATION
FCR-0010 = ACCEPTED_FOR_PLANNING / Waiting On FOUNDATION / prior TARC-only future assumptions superseded-in-part pending FSARM reconciliation
FCR-0007 = ACCEPTED_FOR_PLANNING / Waiting On FOUNDATION / future requester identity requires FSARM reconciliation
```

Foundation has paused further future-facing TARC-hard-bound assumptions pending governed FSARM reconciliation.

`ACCEPTED_FOR_PLANNING` does not prove the FSARM-compatible Foundation runtime capability exists.

## 10. Part 1 Files Updated by This Remediation

The following active Part 1 semantic candidate files were updated to incorporate FSARM and Foundation-first design ordering:

1. `00_PART1NG_MASTER_DESIGN_AND_SCOPE.md`
2. `01_PART1NG_WORK_PACKAGE_DECOMPOSITION.md`
3. `02_PART1NG_DEPENDENCY_FCR_AND_PARALLELIZATION_MODEL.md`
4. `03_PART1NG_PART0_TRACEABILITY_AND_COMPLETENESS_REGISTER.md`

This record is additional controlling trace evidence for the current semantic remediation.

## 11. Review Reset

FSARM is a material semantic change after the previous Part 1 candidate freeze/reviews.

Therefore:

```text
PREVIOUS_PART1_FREEZE = HISTORICAL_FOR_CHANGED_SCOPE
PREVIOUS_ARCHITECTURE_PASS = NOT CURRENT FOR FSARM-CHANGED SCOPE
PREVIOUS_RED_TEAM_PASS = NOT CURRENT FOR FSARM-CHANGED SCOPE
```

Required next lifecycle after semantic remediation is complete:

```text
NEW SEMANTIC FREEZE
 -> FRESH ARCHITECTURE / CONSISTENCY REVIEW
 -> FRESH RED TEAM
 -> OWNER REVIEW
 -> EXPLICIT OWNER ACCEPTANCE / CLOSURE
```

No implementation or runtime authority is granted by this record.
