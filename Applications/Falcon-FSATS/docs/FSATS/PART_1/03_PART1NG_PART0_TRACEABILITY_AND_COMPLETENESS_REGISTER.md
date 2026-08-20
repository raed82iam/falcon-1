# FSATS Part 1-NG — Part 0 Traceability and Completeness Register

**Status:** `DESIGN_CANDIDATE / SEMANTIC REMEDIATION IN PROGRESS`  
**Implementation Authority:** `NOT GRANTED`

## 1. Purpose

This register proves that the Part 1 candidate is derived from the accepted Part 0 implementation-readiness baseline while explicitly recording later Owner-directed prospective semantic corrections rather than silently rewriting accepted history.

The current material prospective correction is FSARM, which supersedes the future-facing Trading-only TARC resource-controller assumption for Part 1 planning while preserving accepted Part 0 records as historical accepted evidence.

## 2. Part 0 Readiness-to-Part 1 Mapping

| Part 0 / P0-L implementation-readiness area | Required Part 1 owner |
|---|---|
| Governance / authority / evidence kernel | P1-A, D, L |
| Foundation integration / capability / FCR baseline | P1-B, L |
| Repository / project topology | P1-C, L |
| Application identity / Manifest / lifecycle | P1-E, B |
| Cross-Application contracts | P1-K, B, L |
| FSAPMA | P1-G, K, L |
| Trading Core | P1-F, K, L |
| Guardian | P1-H, K, L |
| Performance / QoS | P1-J, K, L |
| FSTSimA / Validation | P1-I, K, L |
| FSATS-wide resource management | P1-J, B, L |
| Awareness research / MSA->FSA binding | P1-B, E, F/G/H/I as applicable, L |
| Historical Part 1 compatibility | P1-A, L |
| Runtime blocker register | P1-B, J, L |
| Explicitly unauthorized register | P1-A, B, L |

No current major readiness area is intentionally left without a Part 1 owner.

## 3. Accepted Topology Coverage and Prospective FSARM Delta

| Topology element | Part 1 coverage | Current treatment |
|---|---|---|
| FSATS non-owning boundary 0 MSA / 0 LSA | P1-C, L | Preserved; FSARM must not silently turn FSATS into an Application/hidden principal |
| Trading 1 MSA / 13 LSA | P1-E, F | Preserved |
| T-LSA-13 Trading Resource Management | P1-F, J, L | Preserved as Trading awareness/evaluation |
| Historical/current Part 0 TARC separate Trading operational controller | P1-A, J | Accepted historical design evidence; prospectively superseded as future implementation target by Owner-directed FSARM where conflicting |
| FSARM system-wide resource manager | P1-B, C, E, J, L | New prospective Owner-directed Part 1 semantic correction; Foundation reconciliation FCR-0031 required |
| FSAPMA 1 MSA / 6 LSA | P1-E, G | Preserved |
| Provider Controller inside P-LSA-04 | P1-G, L | Preserved |
| Guardian 1 MSA / 4 LSA | P1-E, H | Preserved |
| FSTSimA 1 MSA / 8 LSA | P1-E, I | Preserved |
| S-LSA-07 / S-LSA-08 split | P1-I, L | Preserved |
| Shared Web exact independent counterparty | P1-K, B | Preserved unless a future separately governed change is approved |
| Shared Communication exact independent counterparty | P1-K, B | Preserved unless a future separately governed change is approved |

## 4. FSARM Semantic Trace

### Owner-directed intent

FSARM means:

```text
FSARM = FALCON SELF-AWARE RESOURCE MANAGEMENT
```

It is the prospective single FSATS-wide operational resource-management authority coordinating resources across:

- Falcon Self-Aware Trading Application;
- FSAPMA;
- Falcon Trading Guardian Application;
- FSTSimA.

### Core operating invariant

```text
INTERNAL_REDISTRIBUTION_FIRST
FOUNDATION_ADDITIONAL_REQUEST_SECOND
```

FSARM SHALL first determine whether already-available FSATS resources can safely satisfy a higher-priority need through governed redistribution, reclaim, throttle, shedding or temporary suspension of eligible lower-priority workloads.

Only the proven remaining deficit may be escalated as an additional resource request to Foundation Resource Governance when that governed capability is available and authorized.

```text
GROSS_NEED
 - SAFE_RECLAIMABLE_INTERNAL_CAPACITY
 = REMAINING_DEFICIT

REMAINING_DEFICIT > 0
 -> FOUNDATION_REQUEST_ALLOWED
```

`REQUESTED_RESOURCE != GRANTED_RESOURCE` remains mandatory.

### Crisis example

If Guardian requires additional compute/memory during a crisis and FSTSimA holds reclaimable resources not required for current live protection/trading continuity:

```text
GUARDIAN_CRISIS_RESOURCE_DEFICIT
 -> FSARM EVALUATES CURRENT FSATS RESOURCE STATE
 -> RECLAIM ELIGIBLE FSTSIMA CAPACITY
 -> REALLOCATE TO GUARDIAN
 -> REQUEST FOUNDATION ONLY IF A DEFICIT STILL REMAINS
```

Resource reallocation does not transfer Guardian authority to FSARM and does not permit FSARM to alter FSTSimA evidence truth.

### Dynamic priority rule

FSARM SHALL NOT encode one permanent Application ranking as the sole decision rule. It must evaluate current obligation, consequence of starvation, minimum-safe floor, reclaimability, current pressure, protection state and admitted policy.

## 5. Foundation / FCR Trace

Current relevant live state at the time of this semantic update:

```text
FCR-0031 = ACCEPTED_FOR_PLANNING / Waiting On FOUNDATION
FCR-0010 = ACCEPTED_FOR_PLANNING / Waiting On FOUNDATION / TARC-only future assumptions superseded-in-part pending FSARM reconciliation
FCR-0007 = ACCEPTED_FOR_PLANNING / Waiting On FOUNDATION / future requester identity requires FSARM reconciliation
FCR-0030 = ACCEPTED_FOR_PLANNING / Waiting On FOUNDATION
```

FCR-0031 is `INCOMPATIBLE` with the prior TARC-only future-facing model and blocks final Part 1 resource-integration closure until Foundation produces a reconciled FSARM-compatible identity/allocation/accounting/isolation design and Application verification is completed.

Part 1 SHALL NOT treat `ACCEPTED_FOR_PLANNING` as implementation availability.

## 6. Part 0 Responsibility Coverage

| Accepted Part 0 area | Part 1 materialization owner / treatment |
|---|---|
| P0-A Governance / Authority / Evidence | P1-A, D, B, L |
| P0-B Requirements / History / Traceability | P1-A, L |
| P0-C Application Topology / Awareness / Evolution | P1-E, F, G, H, I, B, L |
| P0-D Foundation Capability / Runtime Readiness | P1-B, J, L |
| P0-E Identity / Manifest / Lifecycle | P1-E, B |
| P0-F Cross-Application Contracts / Information Flow | P1-K, B, L; FSARM impact must be explicit and cannot silently modify historical 43/43 evidence |
| P0-G FSAPMA Operational Data Fabric | P1-G, K, B, L |
| P0-H Trading Core / 13 LSA / TARC | P1-F preserves Trading topology; P1-J prospectively replaces TARC future implementation role with FSARM under later Owner direction |
| P0-I Guardian Protection / Crisis / Recovery | P1-H, J, K, L |
| P0-J Performance / Resource / QoS / Resilience | P1-J, K, B, L |
| P0-K Validation / Credibility / Promotion | P1-I, B, L |
| P0-L Integration / Assurance / Implementation Readiness | P1-A through L integrated |

## 7. Cross-Cutting Invariants

Part 1 SHALL preserve all of the following across every WP:

- `SELF_AWARENESS != AUTHORITY`;
- `RECOMMENDATION != AUTHORIZATION`;
- `VALIDATION_EVIDENCE != PROMOTION_AUTHORITY`;
- `CONTRACT_VALID != ROUTE_ACTIVE`;
- `ROUTE_EXISTS != BUSINESS_ACTION_AUTHORIZED`;
- `REQUESTED_RESOURCE != GRANTED_RESOURCE`;
- `FSARM != FOUNDATION_RESOURCE_GOVERNANCE`;
- `RESOURCE_PRIORITY != BUSINESS_AUTHORITY`;
- `APPLICATION_PRIORITY != FOUNDATION_TECHNICAL_CRITICALITY`;
- `T_LSA13 != FSARM`;
- `INTERNAL_REDISTRIBUTION_FIRST`;
- `FOUNDATION_ADDITIONAL_REQUEST_SECOND`;
- `REPLAY_TEST_TRAFFIC != OPERATIONAL_TRAFFIC`;
- `PROVIDER_DATA_PERMISSION != BROKER_EXECUTION_PERMISSION`;
- `RESEARCH_EGRESS_PERMISSION != OPERATIONAL_DATA_PERMISSION`;
- `FSTSIMA_NONLIVE_PERMISSION != LIVE_PERMISSION`;
- `PART1_DESIGN_CLOSED != IMPLEMENTATION_AUTHORIZED`.

## 8. Current Scope / Market Constraints

Part 1 shall materialize only the current accepted initial Trading scope unless separately changed by the Owner:

```text
MARKETS = US_EQUITIES + CRYPTO_SPOT
FUNDED_EXPOSURE_MODEL = 1_TO_1
LEVERAGE = NOT_AUTHORIZED
DERIVATIVES = NOT_AUTHORIZED
ADDITIONAL_MARKETS = NOT_AUTHORIZED
```

Architecture may remain extensible, but no extra market/instrument authority may be embedded as an active initial requirement.

## 9. Historical Part 1 and Prior Candidate Delta Register

Historical Part 1 remains preserved and may be mined for implementation lessons only.

Known material deltas requiring fresh compatibility proof include current topology, independent FSTSimA, current contract graph, current Foundation stages and the FSARM resource-management correction.

The previous Part 1 candidate freeze/review set predates FSARM. It remains historical review evidence only for the exact earlier candidate.

```text
PRIOR_PART1_FREEZE_PASS != CURRENT_FSARM_CANDIDATE_PASS
```

## 10. Current Completeness / Review State

The Part 1 semantic candidate has changed materially after the prior freeze due to:

- Foundation-first WP reordering;
- FSARM replacing the future-facing TARC-only resource model;
- explicit internal-redistribution-first / Foundation-request-second semantics;
- new FSARM cross-Application resource profiles, minimum-safe floors, reclaimability, dynamic priority and crisis redistribution requirements;
- FCR-0031 Foundation reconciliation dependency.

Therefore:

```text
CURRENT_SEMANTIC_REMEDIATION = IN_PROGRESS
OLD_FREEZE_CURRENT_FOR_CHANGED_SCOPE = NO
OLD_ARCHITECTURE_PASS_CURRENT_FOR_CHANGED_SCOPE = NO
OLD_RED_TEAM_PASS_CURRENT_FOR_CHANGED_SCOPE = NO
NEW_SEMANTIC_FREEZE_REQUIRED = YES
FRESH_ARCHITECTURE_REVIEW_REQUIRED = YES
FRESH_RED_TEAM_REQUIRED = YES
OWNER_FINAL_REVIEW_REQUIRED = YES
IMPLEMENTATION_AUTHORITY_GRANTED = NO
```
