# FSATS Part 1 — APP-RSC Fifth-Application Owner Direction

**Status:** `OWNER-DIRECTED SEMANTIC CHANGE RECORDED / NOT FINAL OWNER ACCEPTANCE / NOT CLOSED`  
**Branch:** `application-development`  
**Authority Type:** `PART 1 DESIGN DIRECTION`  
**Implementation Authority:** `NOT GRANTED`  
**Runtime Authority:** `NOT GRANTED`

## 1. Purpose

This record preserves the Project Owner's direction to promote the FSATS-wide resource-management function from the previously proposed non-Application FSARM coordinator classification into a fifth independent Falcon Application candidate inside the FSATS system boundary.

This is a prospective Part 1 semantic change. It does not rewrite accepted Part 0 history and it does not constitute final Owner acceptance or closure of the changed Part 1 design.

## 2. Owner Direction

The current Part 1 candidate SHALL evaluate and materialize:

```text
APP-RSC = FALCON SELF-AWARE RESOURCE MANAGEMENT APPLICATION
APP_RSC_SCOPE = FSATS_ONLY
APP_RSC_IS_FALCON_APPLICATION = YES
APP_RSC_IS_FOUNDATION_RESOURCE_GOVERNANCE = NO
APP_RSC_IS_FSATS_CONTAINER = NO
```

APP-RSC is intended to become the fifth independent Application inside the FSATS trading-system boundary alongside:

1. Falcon Self-Aware Trading Application;
2. Falcon Self-Aware Provider Management Application (FSAPMA);
3. Falcon Trading Guardian Application;
4. Falcon Self-Aware Trading Simulation Application (FSTSimA);
5. Falcon Self-Aware Resource Management Application (APP-RSC).

FSATS itself remains a non-owning system boundary with no MSA, runtime principal, hidden mutable state or Foundation authority.

## 3. Scope Boundary

APP-RSC SHALL coordinate only resources inside the governed FSATS coordination envelope.

It SHALL NOT manage, allocate, reclaim, rebalance or prioritize resources belonging to Falcon Applications outside FSATS, including present or future Shared Applications, Accounting, Inventory/Warehouse Management, Feasibility Study, or any other non-FSATS Application.

Foundation Resource Governance remains the sole Falcon-wide owner of total-resource truth, authoritative grants, ceilings, quotas, protection floors, revocation and global redistribution.

```text
APP_RSC_FSATS_EFFECTIVE_COORDINATION != FOUNDATION_AUTHORITATIVE_RESOURCE_TRUTH
```

## 4. Coordination Model

The intended sequence remains:

```text
INTERNAL_REDISTRIBUTION_FIRST
FOUNDATION_ADDITIONAL_REQUEST_SECOND
REQUESTED_RESOURCE != GRANTED_RESOURCE
```

Within a valid Foundation-governed FSATS coordination envelope, APP-RSC may coordinate bounded effective resource use among the constituent FSATS Applications without requiring a Foundation round-trip for every internal move, provided the move does not mutate Foundation authoritative allocations/truth and preserves per-Application attribution, protected minimums, isolation, accounting, fencing and reconstructability.

A Foundation request is required when the evidenced remaining need cannot be safely satisfied inside the valid coordination envelope or when the requested move requires a change to Foundation authoritative grants, ceilings, floors or other Foundation-owned resource truth.

## 5. Application Identity and Awareness Candidate

As an independent Falcon Application candidate, APP-RSC SHALL satisfy the current applicable APP-001 and CON-023 requirements if the design is finally accepted.

The initial candidate awareness topology is:

```text
APP-RSC
  MSA = 1
  LSA = 3
  CSA = 0 initially
```

Proposed major branches for fresh design/review:

```text
R-LSA-01 Resource Picture, Demand Integrity and Coordination Envelope
R-LSA-02 Redistribution, Degradation and Rebalance
R-LSA-03 Foundation Binding, Restoration and Resource Evidence
```

The exact names and decomposition remain design-candidate material until fresh Architecture/Consistency and Red-Team review completes and the Project Owner issues final acceptance.

APP-RSC awareness SHALL remain separate from the operational Resource Strategy Controller:

```text
MSA_RSC != RESOURCE_STRATEGY_CONTROLLER
AWARENESS != OPERATIONAL_CONTROL
```

## 6. Required Failure and Security Properties

The changed Part 1 design SHALL explicitly prove:

- one valid APP-RSC coordination epoch at a time;
- fencing of stale/duplicate coordinators;
- idempotent and attributable resource actions;
- fail-closed behavior when Foundation envelope state is stale, unknown or revoked;
- no peer-to-peer FSATS resource seizure when APP-RSC is unavailable;
- safe degraded operation under last valid Foundation/App resource truth where permitted;
- no APP-RSC authority over Trading decisions, Unified Risk, Guardian commands, FSAPMA provider/data truth or FSTSimA validation truth;
- no authority over non-FSATS Applications;
- no ability to mint Foundation grants, floors, priority or technical criticality;
- bounded blast radius if APP-RSC is compromised;
- independent lifecycle, isolation, replacement and removal behavior consistent with APP-001;
- anti-gaming controls so Application-reported urgency, minimum-safe or reclaimability evidence cannot self-mint resource authority.

## 7. Relationship to Earlier Part 1 Clarification

This Owner direction prospectively supersedes the changed-scope classification in:

`13_PART1_FSARM_OPERATIONAL_SELF_AWARENESS_AND_FUTURE_EVOLUTION_OWNER_CLARIFICATION.md`

only where that record states or implies:

```text
FSARM_IS_FALCON_APPLICATION = NO
FSARM_MSA = 0
FSARM_LSA = 0
FSARM_CSA = 0
```

The earlier record remains preserved as historical Part 1 decision evidence and SHALL NOT be rewritten to make it appear that APP-RSC was always the selected classification.

The following semantics remain preserved unless fresh review identifies a conflict requiring explicit Owner disposition:

- FSATS-wide resource coordination;
- operational self-awareness;
- bounded adaptation only under separately approved bounds;
- no Foundation total-resource authority;
- no business-authority leakage;
- no hidden FSATS runtime principal;
- design-for-replaceability, evidence, fencing and migration.

## 8. Review Reset and Required Lifecycle

This is a material semantic change to the active Part 1 candidate.

Therefore:

```text
PREVIOUS_PART1_REVIEW_EVIDENCE = HISTORICAL_FOR_CHANGED_SCOPE
```

Required lifecycle:

```text
OWNER-DIRECTED APP-RSC CHANGE
 -> MATERIALIZE AFFECTED PART 1 DESIGN
 -> NEW EXACT SEMANTIC FREEZE
 -> FRESH ARCHITECTURE / CONSISTENCY REVIEW
 -> FRESH RED-TEAM REVIEW
 -> REPORT TO PROJECT OWNER
 -> EXPLICIT FINAL OWNER DECISION
```

No implementation, runtime route activation, provider/broker connectivity, Paper, Tiny Live, Live, deployment or Part 2 authority is granted by this record.
