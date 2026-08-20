# FSATS SIA v0.1 — Current Master and Semantic Freeze R5

**Canonical Candidate Package:** `FSATS-SIA-v0.1-R5`
**Workspace:** `applications/docs/FSATS/NEW/`
**Branch:** `application-development`
**Status:** `SEMANTIC_FREEZE_R5 / NOT_OWNER_ACCEPTED / NOT_CLOSED`
**Implementation Authority:** `NOT_GRANTED`
**Runtime / Paper / Tiny Live / Live / Deployment Authority:** `NOT_GRANTED`
**Supersedes as current freeze:** R4

## 1. Freeze Identity

The R5 semantic freeze is the exact Git commit created with unique commit message:

```text
Freeze FSATS SIA v0.1 R5 semantic baseline
```

and containing this file plus the semantic set below.

Any material semantic change after this commit requires a new freeze and fresh A/C + Red-Team before Owner review.

## 2. Governing Order

```text
Falcon Vision
> Falcon Constitution
> current explicit Owner decisions
> approved Specifications / Contracts / accepted ADRs
> current Foundation capability / FCR dispositions
> current accepted FSATS semantics
> FSATS-SIA-v0.1-R5 candidate
> historical P0/P1/V1.3 references
```

Prime rule:

```text
SOURCE -> AUTHORITY -> COMPARE -> DECIDE -> CHANGE
```

## 3. R5 Semantic File Set

R5 includes every semantic file listed by R4 plus these R5 additions:

- `05B_CANONICAL_APPLICATION_CAPABILITY_PERMISSION_AND_ROUTE_DECLARATION_REGISTRY.md`
- `07F_STRATEGY_CAPITAL_COMPETITION_AND_RESERVATION_PRIORITY_SPEC.md`
- `17C_INITIAL_SCHOOL_WEIGHTING_AND_META_LEARNING_ACTIVE_BOUNDARY.md`
- `19A_IMMUTABLE_AUDIT_PROVENANCE_GRAPH_SPEC.md`
- `20E_R4_AC_REMEDIATION_RECONCILIATION.md`
- this `00_CURRENT_SIA_MASTER_AND_SEMANTIC_FREEZE_R5.md`

The complete controlling semantic chain therefore includes:

```text
01,02,03,04,
05,05A,05B,
06,
07,07A,07B,07C,07D,07E,07F,
08,08A,08B,08C,08D,
09,09A,
10,10A,10B,
11,
12,12A,
13,14,15,16,
17,17A,17B,17C,
18,18A,
19,19A,
20,20A,20B,20C,20D,20E,
plus this R5 freeze manifest.
```

Historical R1-R4 freeze/review files and the Owner-created workspace index remain audit/history only and do not redefine R5 semantics.

## 4. Candidate Coverage Counts

```text
Current Applications = 4
Current MSAs = 4
Current LSAs = 31
Candidate CSA profiles = 26
Current Monitor AI perspectives = 8
Initial Markets = 2
Historical Provider Candidate Pool = 13
Canonical Data Products = 10
Core Trading Data Products = 8
Strategy Families = 14 candidate
Intelligence Algorithm Baselines = 11
Accepted P0-F Contract Families Preserved = 43/43
```

If APP-RSC is Owner-accepted:

```text
Applications = 5
MSAs = 5
LSAs = 34
Monitor AI perspectives = 10
Cross-Application contract families = 59
APP-RSC initial CSA candidates = 0
```

## 5. Material Owner Decisions In R5

### D01 — APP-RSC / FSARM placement

Dedicated fifth FSATS-scoped Application `falcon.app.resource.fsarm`, 3 LSAs, no Foundation resource ownership/grant authority.

### D02 — APP-RSC 16 bilateral resource contract additions

Accepted P0-F 43 remain unchanged regardless.

### D03 — 14 strategy-family initial catalog

Prospective expansion beyond historical V1.3 recorded 10-model count; exact algorithms/statistics/parameters are versioned.

### D04 — 26 CSA candidate eligibility profiles

No activation/authority by listing.

### D05 — Initial Risk / Capital / Promotion policy

Includes Paper/Tiny-Live candidate limits, cash-flow-adjusted RiskEquity, tail-aware risk, USD-only initial quote policy, dynamic market allocation, exact promotion evidence minima.

### D06 — Physical .NET architecture

Independent Application hosts, one major-LSA project/assembly, contract-only cross-App references, isolated persistence/Foundation adapter seams.

### D07 — Initial canonical Data Product / quality model

Ten canonical products; required v1 new-risk products must be VALID.

### D08 — FSTSimA deterministic randomness/numerics

xoshiro256**, exact independent SplitMix64 seed initialization, named SHA-256 streams, exact transforms/event ordering/checkpoints.

### D09 — Universe and provider-route selection algorithms

Exact ranking/subscore/hysteresis and failover semantics.

### D10 — Canonical capability/permission declaration registry

Exact `falcon.cap.*`, `falcon.perm.*` and external-permission identities; no wildcard/generic Internet permission.

### D11 — Strategy capital competition

Exact new-risk contention algorithm with 250ms boundary, deterministic capital-efficiency score and atomic reservation. Existing obligations cannot be preempted by score.

### D12 — Initial school weighting

Neutral baseline; no active extra school multiplier. Meta-Learning may propose future candidate profiles only.

### D13 — Immutable Audit Provenance Graph

Federated per-Application immutable graph shards, canonical nodes/edges, causal/provenance relationships, Merkle checkpoints; no hidden FSATS state owner.

## 6. Preserved Finding History

Construction findings:

```text
PF-001 HIGH        contract baseline completeness -> 12A
PF-002 HIGH        strategy parameter completeness -> 17A
PF-003 MEDIUM-HIGH identity materialization -> 05A
PF-004 MEDIUM-HIGH research boundary -> 18A
PF-005 HIGH        Risk numeric policy -> 07A
```

Review findings:

```text
RT-RISK-001 HIGH   -> 07B
RT-DATA-001 HIGH   -> 08A/08B/08C
RT-SIM-001 HIGH    -> 10A
RT-STRAT-001 HIGH  -> 17B
RT-GRD-001 MEDIUM  -> 09A
RT-RISK-002 MEDIUM -> 07C
AC-ALG-001 HIGH    -> 07E
AC-ALG-002 HIGH    -> 07D
AC-PMA-001 HIGH    -> 08D
AC-SIM-001 MEDIUM  -> 10B
AC-GOV-001 HIGH    -> 05B
AC-CAPITAL-001 HIGH-> 07F
AC-STRAT-002 HIGH  -> 17C
AC-EVID-001 HIGH   -> 19A
```

No finding is erased by remediation.

## 7. New R5 Hard Invariants

### Capability / permission

```text
UNKNOWN_DECLARATION_ID = DENY
WILDCARD_PERMISSION = FORBIDDEN
CROSS_APP_ROUTE_ID = CANONICAL 12A FAMILY
GENERIC_INTERNET_PERMISSION = FORBIDDEN
```

### Capital competition

```text
RISK/GUARDIAN/HARD GATES FIRST
NEW CAPITAL CONTENTION = 250ms BOUNDARY
RANKING = EXACT CapitalPriorityScore
RESERVATION = ATOMIC IN RANK ORDER
EXISTING VALID OBLIGATION PREEMPTION BY SCORE = FORBIDDEN
```

### School weighting

```text
CLASSICAL_ACTIVE_WEIGHT = 1.0000
HUNTING_ACTIVE_WEIGHT = 1.0000
EXTRA_ACTIVE_SCHOOL_MULTIPLIER = NONE
META_LEARNING_WEIGHT_PROFILE = CANDIDATE_ONLY
```

### Provenance graph

```text
GRAPH_OWNERSHIP = FEDERATED PER APPLICATION
GRAPH != BUSINESS_STATE_OWNER
CORRELATION != CAUSATION
HISTORY_CORRECTION = APPEND + CORRECTS/SUPERSEDES
HIGH_CONSEQUENCE_PROVENANCE_CLOSURE = REQUIRED
```

## 8. Non-Ambiguity Contract

```text
MATERIAL SEMANTIC UNKNOWN/CONFLICT
-> STOP AFFECTED IMPLEMENTATION
-> REPORT EXACT GAP
-> DO NOT INVENT ALGORITHM / AUTHORITY / STATE / SCHEMA / POLICY
```

## 9. Legitimate Future / External Gates

R5 remains fail closed for:

- current official provider/broker certification and exact active provider subset;
- Shared Web/Communication exact canonical external Application identities/reciprocal manifests;
- Foundation Stage12 research/provider/broker egress/credentials and FSTSimA external isolation;
- FCR-0012/0030 FSA exact interface;
- FCR-0016 canonical Foundation artifact consumption;
- actual Application implementation/binding verification for current Application-hold FCRs;
- deployment/hardware-specific mandatory capacity configuration;
- Full Live/Scale policy and exact Owner-authorized capital.

## 10. Required Review Sequence

```text
R5 FREEZE
-> FRESH A/C R5
-> FRESH RED-TEAM R5
-> OWNER REVIEW only if SAME unchanged R5 passes both
```

No earlier PASS is inherited.

## 11. Authority Markers

```text
FSATS_SIA_v0.1_R5 = SEMANTIC_FREEZE
OWNER_ACCEPTED = NO
IMPLEMENTATION_AUTHORIZED = NO
RUNTIME_AUTHORIZED = NO
PAPER_AUTHORIZED = NO
TINY_LIVE_AUTHORIZED = NO
LIVE_AUTHORIZED = NO
DEPLOYMENT_AUTHORIZED = NO
```
