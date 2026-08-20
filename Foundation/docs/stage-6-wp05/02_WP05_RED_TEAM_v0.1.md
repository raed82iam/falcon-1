# Stage 6 WP-05 Planning Draft v0.1 — Red-Team Review

**Status:** FINDINGS OPEN / REMEDIATION REQUIRED BEFORE OWNER REVIEW  
**Reviewed Artifact:** `docs/stage-6-wp05/01_WP05_PLANNING_DRAFT_v0.1.md`  
**Authority:** Planning review only; no implementation authority.

## 1. Red-Team Goal

Attempt to break the WP-05 draft by testing for:

- duplicate ownership of accepted Stage 5 pressure-consumption capability;
- duplicate ownership of accepted Stage 6 WP-01 through WP-04 truth;
- leakage of Application business load-shedding policy into Foundation;
- leakage of WP-06 request/decision authority into WP-05;
- leakage of WP-07 reclamation execution into WP-05;
- pressure state being treated as authority;
- caller-declared business urgency minting Foundation priority/technical criticality;
- inability to operate with zero Applications;
- non-deterministic or non-reconstructable pressure truth;
- FCR-0010 mapping/ACK bypass.

## 2. Findings

### RT-WP05-001 — HIGH — Enforcement-state wording can accidentally imply WP-07 execution

The draft allows enforcement truth wording such as `ceiling reduction pending/effective where that reduction is already authorized` and `lower-priority reclaimable allocation marked eligible for reduction/preemption`.

Risk:
A later implementer could interpret WP-05 as both deriving pressure truth and mutating allocations/ceilings. That would collide with WP-07 reclamation/rebalance execution and potentially with accepted WP-03 allocation/ceiling ownership.

Required remediation:
- state explicitly that WP-05 is an observation/derivation truth producer;
- WP-05 may report an enforcement state produced by an accepted/external governed authority, but SHALL NOT mutate allocation, ceiling, grant, or reclaimable quantity;
- any state transition requiring a resource mutation belongs to its proper authorized owner;
- distinguish `ENFORCEMENT_OBSERVED` from `ENFORCEMENT_EXECUTED_BY_WP05`, with the latter prohibited.

Severity: HIGH.

### RT-WP05-002 — HIGH — CRITICAL pressure semantics are too close to Guardian/protection authority

The draft says CRITICAL includes conditions where Foundation survival/protection/control capacity is at risk or stronger protective action may be needed.

Risk:
Pressure truth could be misread as directly authorizing Guardian restriction, safe-state entry, or later recovery behavior. Stage 8 owns Guardian/protective restriction. Resource pressure is evidence, not protection authority.

Required remediation:
- explicitly state `PRESSURE_STATE != PROTECTIVE_AUTHORITY`;
- WP-05 may emit attributable evidence consumable by protection authorities later, but cannot command safe state, Guardian restriction, lifecycle halt, or release;
- any immediate resource-local fail-closed technical behavior must stay inside already authorized resource enforcement boundaries and must not become Falcon-wide protection authority.

Severity: HIGH.

### RT-WP05-003 — MEDIUM — Proposed NORMAL/CONSTRAINED/DEGRADED/CRITICAL taxonomy lacks explicit anti-hysteresis/flapping requirement

Risk:
Without governed transition stability, noisy resource observations may make pressure truth oscillate rapidly and generate contradictory downstream behavior/evidence.

Required remediation:
Planning should require deterministic transition/debounce/hysteresis policy or an equivalent governed stability mechanism, without inventing numeric thresholds yet. The mechanism must be versioned/evidenced and must not hide genuine critical deterioration.

Severity: MEDIUM.

### RT-WP05-004 — MEDIUM — Missing explicit aggregate-vs-per-Application pressure separation

WP-05 mentions Foundation and Application-specific pressure but does not fully define separation.

Risk:
A per-Application constrained state could be mistaken for global Foundation pressure, or global pressure could expose another Application's allocation state.

Required remediation:
Require distinct attributable scopes:
- Foundation-global/resource-class pressure truth;
- exact Application-bound pressure/enforcement truth;
- no hidden aggregation that leaks another Application's confidential allocation details;
- no Application may infer another Application's allocation from shared pressure evidence.

Severity: MEDIUM.

### RT-WP05-005 — MEDIUM — Freshness/expiry is required but monotonic ordering/supersession ambiguity remains

Risk:
Two individually valid snapshots may arrive out of order, creating rollback to stale pressure truth.

Required remediation:
Require exact epoch/version plus monotonic sequence/effective-order or equivalent deterministic supersession mechanism, and fail closed on rollback/substitution.

Severity: MEDIUM.

### RT-WP05-006 — PASS — No Application business load-shedding ownership leakage found

The draft correctly keeps Trading/business degradation hierarchy in Application scope and preserves TARC as the Trading Application operational resource controller.

### RT-WP05-007 — PASS — WP-06 request/decision authority remains excluded

No grant/cap/deny request-decision implementation is assigned to WP-05.

### RT-WP05-008 — PASS — Existing-capability reconciliation is structurally preserved

The draft consumes accepted WP-01 through WP-04 and accepted Stage 5 pressure-consumption behavior rather than reopening them.

### RT-WP05-009 — PASS — Zero-Application invariant preserved

The draft includes zero-Application verification and does not make an Application a Foundation prerequisite.

### RT-WP05-010 — PASS WITH HOLD — FCR-0010 is not bypassed

The draft allows preparation while ACK is pending but requires reconciliation before final Owner design acceptance and implementation authorization. This is acceptable for draft planning.

## 3. Required Remediation Set

Before Owner review, WP-05 planning shall add:

1. explicit non-mutation rule for allocation/ceiling/grant/reclamation state;
2. explicit `PRESSURE_STATE != PROTECTIVE_AUTHORITY` boundary;
3. governed deterministic transition-stability requirement;
4. explicit global-vs-Application pressure scope separation and non-disclosure rule;
5. monotonic ordering/supersession protection;
6. verification families for all five remediations.

## 4. Red-Team Result

`WP05_DRAFT_V0_1_ARCHITECTURAL_DIRECTION = PASS`

`WP05_DRAFT_V0_1_READY_FOR_OWNER_ACCEPTANCE = NO`

`OPEN_HIGH_FINDINGS = 2`

`OPEN_MEDIUM_FINDINGS = 3`

`IMPLEMENTATION_AUTHORITY = NO`

The draft is coherent in direction but requires remediation and another fresh Red-Team before it should be presented to the Owner for acceptance.
