# FSATS Part 8 — Scope and Work Package Baseline

**Status:** `OWNER_AUTHORIZED / BASELINED_FOR_IMPLEMENTATION`  
**Date:** `2026-08-16`  
**Branch:** `application-development`

## 1. Mission

Part 8 materializes the Application-owned evidence and analytics boundary between already-produced trading/simulation outcomes and any later self-development/adoption workflow.

It does not own strategy deployment, production adoption, runtime authority, provider/broker connectivity, or Foundation/FSA governance.

## 2. Work Packages

### P8-WP01 — Evidence identity and truth classification

Materialize immutable attributable evidence records with exact evidence identity, decision identity, strategy identity, BrokerId, BrokerAccountId, Environment, market, horizon, trust epoch, source kind, truth state, completeness and measured outcome/process-quality fields.

Operational evidence remains broker-account scoped. Simulation/replay evidence must preserve an explicit governed subject/environment identity and never impersonate Live truth.

### P8-WP02 — Evidence quality and attribution gate

Reject or hold evidence with missing identity, duplicate evidence identity, duplicate decision identity inside one evaluated set, stale/conflicted/incomplete truth, unknown source/truth class, invalid scope, or attribution mismatch. Preserve losses and unfavorable outcomes instead of survivorship-filtering them away.

### P8-WP03 — Deterministic scoped analytics

Compute deterministic analytics only within one exact:

```text
StrategyId
+ BrokerId
+ BrokerAccountId
+ Environment
+ MarketId
+ Horizon
+ TrustEpoch
```

Mixed-scope evidence must never be silently aggregated.

Analytics shall expose sample count, positive/negative/flat outcomes, average outcome, average risk-adjusted outcome, process-validity ratio, evidence IDs, decision IDs, and source mix.

### P8-WP04 — Baseline/candidate comparison

Compare baseline and candidate analytics only when BrokerId + BrokerAccountId + Environment + Market + Horizon + TrustEpoch are compatible and both evidence sets independently satisfy required minimum evidence gates.

Baseline and candidate strategy identities must be distinct. The same evidence identity must not be counted in both sets. The same governed decision identity must not be counted on both sides through different evidence identities.

### P8-WP05 — Candidate readiness decision

Produce one bounded readiness result:

```text
NOT_READY
READY_FOR_GOVERNED_CANDIDATE_REVIEW
```

The result shall contain reason codes and exact supporting evidence identities and shall always declare:

```text
GrantsAdoptionAuthority = false
GrantsDeploymentAuthority = false
GrantsRuntimeAuthority = false
```

### P8-WP06 — Adversarial verification

Cover at minimum:

- profitable result with invalid decision process does not promote;
- losses remain in analytics;
- insufficient sample blocks readiness;
- duplicate evidence identity blocks the affected set;
- duplicate decision identity blocks sample inflation;
- baseline/candidate evidence overlap blocks double counting;
- baseline/candidate decision overlap blocks double counting through different evidence identities;
- same baseline/candidate strategy identity blocks self-comparison promotion;
- stale/conflicted/incomplete evidence blocks readiness;
- unknown evidence source/truth blocks readiness;
- mixed broker account/environment/market/horizon/epoch/strategy evidence is not silently aggregated;
- simulation evidence remains simulation and cannot be represented as Live truth;
- candidate underperforms baseline -> not ready;
- deterministic input -> deterministic analytics/readiness;
- candidate readiness never grants activation/adoption/deployment/runtime authority.

## 3. Evidence Source Classes

Current Part 8 recognizes bounded evidence source classes conceptually:

```text
OPERATIONAL
SIMULATION
REPLAY
```

Source classification records provenance, not authority. Replay and simulation may support learning/analysis but do not become Live operational truth merely because their outcomes are favorable.

## 4. Readiness Policy

Part 8 uses explicit evidence-policy inputs rather than hidden constants where material:

```text
minimumBaselineSamples
minimumCandidateSamples
minimumProcessValidityRatio
minimumRiskAdjustedImprovement
allowSimulationEvidenceForCandidateReview
```

Even when simulation evidence is allowed for **candidate review**, the result remains a review recommendation only.

## 5. Non-Scope

Explicitly excluded:

- automatic strategy mutation;
- automatic weight changes;
- strategy activation/deactivation;
- MSA/FSA adoption workflow;
- provider/broker API calls;
- external Internet research;
- runtime bindings held by FCR-0009/FCR-0082;
- Shared Web implementation;
- Foundation implementation;
- Part 9 or Part 10.

## 6. Acceptance Criteria

Part 8 reaches technical closure-readiness only when:

1. source implementation is complete within Application ownership;
2. existing Part 0-Part 7 behavior remains build-compatible;
3. Part 8 adversarial checks pass;
4. normal governed verifier set passes;
5. fresh post-implementation Architecture/Consistency review passes;
6. fresh broad Red Team passes with no unresolved blocking finding;
7. audit confirms exact HEAD, scope discipline and no authority leakage;
8. final state is presented to the Project Owner for explicit acceptance/closure.
