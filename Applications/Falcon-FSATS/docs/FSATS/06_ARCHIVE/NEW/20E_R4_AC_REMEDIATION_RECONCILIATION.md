# FSATS SIA — R4 Architecture/Consistency Remediation Reconciliation

**Package:** `FSATS-SIA-v0.1`
**Record Type:** `PRE-R5 SEMANTIC RECONCILIATION`
**R4 A/C:** `21C_ARCHITECTURE_AND_CONSISTENCY_REVIEW_R4.md` = FAIL

## 1. Purpose

Bind every R4 gap-hunt finding to an exact semantic remediation before R5 freeze. This record is not a PASS.

## 2. AC-GOV-001 — HIGH — Capability / Permission / Route Declaration Registry

Remediation:

`05B_CANONICAL_APPLICATION_CAPABILITY_PERMISSION_AND_ROUTE_DECLARATION_REGISTRY.md`

Now exact:

- stable `falcon.cap.*` Application capability identities;
- stable `falcon.perm.*` internal permission identities;
- external provider/broker/research/FSA-submission permission request identities;
- exact owner/consumer scope;
- no wildcard/broad permission;
- exact environment separation;
- exact future FCR gates;
- cross-App routes reference canonical 12A `falcon.xapp.*` families rather than duplicate aliases;
- no locally invented Foundation capability identity.

Status: `REMEDIATED_AT_DESIGN_CANDIDATE_LEVEL`.

## 3. AC-CAPITAL-001 — HIGH — Internal Strategy Capital Competition

Remediation:

`07F_STRATEGY_CAPITAL_COMPETITION_AND_RESERVATION_PRIORITY_SPEC.md`

Now exact:

- competition only when scarce new capital/capacity exists;
- 250ms half-open competition boundary;
- hard Risk/Guardian/market/data exclusion before ranking;
- exact CapitalPriorityScore:
  - 30% Net Edge;
  - 20% Calibration;
  - 15% Execution Quality;
  - 15% Diversification;
  - 10% Recent Strategy Efficiency;
  - 10% Capital Consumption Efficiency;
- deterministic tie-break;
- atomic winner-by-winner reservation;
- portfolio/correlation ceilings recomputed after prior winners;
- original Risk maximum never increased;
- partial award only when strategy/proposal explicitly remains viable;
- no pro-rata invention;
- no score-based preemption of existing positions/orders/reservations/protective buffers;
- losers do not retry inside the same boundary;
- freed later capital does not auto-award stale losers.

Status: `REMEDIATED_AT_DESIGN_CANDIDATE_LEVEL`.

## 4. AC-STRAT-002 — HIGH — School Weighting / Meta-Learning Active Boundary

Remediation:

`17C_INITIAL_SCHOOL_WEIGHTING_AND_META_LEARNING_ACTIVE_BOUNDARY.md`

Now exact:

```text
Initial active school weighting = NEUTRAL
Classical = 1.0000
Opportunity Hunting = 1.0000
No additional active EvalScore/CapitalPriority multiplier
```

T-LSA-12 may propose candidate successor SchoolWeightProfiles only through FSTSimA + review/governance/Owner lifecycle. No hidden adaptive active runtime.

Status: `REMEDIATED_AT_DESIGN_CANDIDATE_LEVEL`.

## 5. AC-EVID-001 — HIGH — Immutable Audit Provenance Graph

Remediation:

`19A_IMMUTABLE_AUDIT_PROVENANCE_GRAPH_SPEC.md`

Now exact:

- federated per-Application immutable shards; no mutable FSATS graph owner;
- canonical node/edge IDs via SHA-256;
- registered Data/Strategy/Risk/Capital/Execution/Guardian/Resource/Simulation/Awareness/Owner node types;
- exact edge semantics including DERIVED_FROM, CAUSED_BY, AUTHORIZED_BY, CONSTRAINED_BY, SELECTED_FROM, EXECUTED_AS, EFFECT_OF, VALIDATED_BY, ADOPTED_BY_DECISION, SUPERSEDES, CORRECTS, CORRELATES_WITH, REPLAYS, REFERENCES_EXTERNAL;
- high-consequence required graph closure;
- append-only correction/supersession;
- cross-App immutable-reference rules;
- deterministic Merkle checkpoint roots;
- graph completeness state;
- causal queries exclude correlation edges;
- graph remains index/provenance model, not second business-state authority.

Status: `REMEDIATED_AT_DESIGN_CANDIDATE_LEVEL`.

## 6. R4 Outcome Preservation

```text
R4 A/C = FAIL
R4 Red-Team = NOT_RUN_AS_FINAL because A/C blocked progression
R4 Owner review = NOT_ELIGIBLE
```

The R4 review remains immutable history.

## 7. R5 Required Lifecycle

```text
FREEZE R5
-> FRESH A/C R5
-> FRESH RED-TEAM R5
-> OWNER REVIEW only if the same unchanged R5 passes both
```

No earlier PASS is inherited.

## 8. Pre-R5 Known Finding State

```text
AC-GOV-001 = REMEDIATED
AC-CAPITAL-001 = REMEDIATED
AC-STRAT-002 = REMEDIATED
AC-EVID-001 = REMEDIATED

KNOWN OPEN CRITICAL = 0
KNOWN OPEN HIGH = 0
KNOWN OPEN MEDIUM = 0
```

This reconciliation does not itself prove R5 correctness.
