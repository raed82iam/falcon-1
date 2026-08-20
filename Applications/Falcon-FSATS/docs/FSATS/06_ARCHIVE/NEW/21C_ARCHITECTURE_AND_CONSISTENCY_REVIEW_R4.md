# FSATS SIA v0.1 R4 — Fresh Architecture and Consistency Review

**Review Type:** `FRESH STATIC ARCHITECTURE / CONSISTENCY REVIEW`
**Reviewed Freeze:** `FSATS-SIA-v0.1-R4`
**Freeze Manifest:** `00_CURRENT_SIA_MASTER_AND_SEMANTIC_FREEZE_R4.md`
**Status:** `FAIL / SEMANTIC COMPLETENESS REMEDIATION REQUIRED`
**Owner Review Eligible:** `NO`
**Implementation Authority:** `NOT_GRANTED`

## 1. Review Rule

R4 was reviewed independently after all R3 A/C findings were remediated. No earlier PASS is inherited.

R4 successfully closes the previously identified Universe ranking, MarketCapitalFitness, provider-route selection and FSTSimA seed-initialization ambiguities. A wider gap hunt was then performed against the full SIA rather than only prior findings.

## 2. AC-GOV-001 — Canonical Application Capability / Permission / Route Declaration Identities Are Not Fully Materialized

**Severity:** `HIGH`
**Status:** `OPEN`

### Finding

File 05 defines complete Manifest declaration classes and files 12/12A define exact cross-Application contract families, but the SIA still lacks one canonical initial registry that binds:

- Application-owned capability IDs;
- consumed capability IDs;
- permission IDs/scopes;
- provider/broker/research external capability permissions;
- environment applicability;
- resource/route declaration references;
- exact deny-by-default behavior for an unknown declaration ID.

Without the registry, two implementation teams can invent different Manifest identifiers for the same semantic capability or over-broaden permission strings while still claiming structural CON-023 compliance.

### Required remediation

Create one initial `Application Capability / Permission / Route Declaration Registry` with exact stable IDs, owning Application, scope/environment and future-FCR gates. Cross-App contract family IDs remain the canonical business route identities; the new registry SHALL NOT duplicate them under incompatible names.

## 3. AC-CAPITAL-001 — Internal Strategy Capital Competition Is Not Yet An Exact Algorithm

**Severity:** `HIGH`
**Status:** `OPEN`

### Finding

The design preserves the Owner-directed concept that strategies should compete for capital by proven efficiency, and file 17 contains a conceptual Strategy Capital Efficiency score. Files 07A/07B define hard Risk/capital ceilings and atomic reservations.

However, the actual **contention algorithm** when multiple simultaneously valid proposals compete for insufficient unreserved capital is not yet exact.

Open choices include:

- exact score formula/normalization;
- whether a high-scoring proposal may partially consume capital and starve lower proposals;
- whether previously held capital can be preempted;
- batching/decision-boundary semantics;
- tie-breaks;
- whether a rejected proposal retries within the same cycle;
- how diversification and correlated clusters influence contention.

### Required remediation

Define a deterministic internal capital-allocation competition that operates **after** hard Risk/Guardian/market allocation gates and **before** atomic reservations, cannot preempt existing confirmed obligations merely by score, and has explicit tie/retry/evidence rules.

## 4. AC-STRAT-002 — Active School Weighting / Meta-Learning Boundary Is Still Ambiguous

**Severity:** `HIGH`
**Status:** `OPEN`

### Finding

File 17 describes contextual school weighting and T-LSA-12 Meta-Learning candidate proposals, but the initial active-runtime behavior does not fix whether an additional dynamic `SchoolWeight` multiplier is actually applied to Strategy EvalScores.

This can materially alter strategy selection and capital competition while all individual strategy formulas remain unchanged.

### Required remediation

For v1, either define one exact school-weight algorithm or explicitly set the active baseline to **no independent school-weight multiplier / equal neutral school weight**, with T-LSA-12 allowed only to propose candidate successor weighting profiles through FSTSimA and the normal governance lifecycle.

The simpler fail-closed initial choice is preferred unless a proven exact algorithm is already required.

## 5. AC-EVID-001 — Immutable Audit Provenance Graph Concept Is Not Yet Materialized As A Canonical Graph Model

**Severity:** `HIGH`
**Status:** `OPEN`

### Finding

The SIA has strong evidence requirements, immutable ledgers, correlation/causation and traceability, but it does not yet define the Owner-directed `Immutable Audit Provenance Graph` as an exact cross-domain causal/provenance model.

Without this, implementations may store good local evidence while lacking one reconstructable canonical relation model for questions such as:

```text
Which Data Product / feature / strategy evaluation caused this TradeProposal?
Which RiskDecision and capital reservation authorized the resulting order intent?
Which broker event produced this fill/position change?
Which Guardian/FSARM action affected the decision?
Which candidate/Owner decision changed the active artifact?
```

### Required remediation

Define canonical graph node/edge identities, edge semantics, required graph coverage for high-consequence actions, immutable correction/supersession, graph digest/reconstruction and the distinction between causal edges and non-causal correlation links.

The graph SHALL index/reference authoritative evidence; it must not become a second business-state owner.

## 6. R3 Findings Retest

R4 static review found the R3 remediations coherent:

- `AC-ALG-001` Universe ranking: closed by 07E;
- `AC-ALG-002` Market allocation: closed by 07D;
- `AC-PMA-001` Provider route scoring: closed by 08D;
- `AC-SIM-001` digest-to-PRNG state: closed by 10B.

No regression was found in those subjects.

## 7. Other Architecture Subjects Retested Without New Finding

No new architecture conflict was opened against:

- current four-Application / 31-LSA boundary;
- APP-RSC prospective fifth-Application isolation;
- accepted P0-F 43/43 contract preservation;
- exact Data Product catalog/quality profiles;
- Trading Risk/capital/time/tail/USD-only rules;
- Guardian discriminated directive actions;
- FSTSimA deterministic stochastic profile;
- exact strategy/statistical formulas;
- Provider route certification/failover;
- Awareness/CSA/Monitor/research/FSA boundary;
- persistence/concurrency/outbox/inbox;
- runtime/queue/backpressure rules;
- future Foundation/FCR fail-closed seams.

## 8. Severity Summary

| ID | Severity | Status |
|---|---|---|
| AC-GOV-001 | HIGH | OPEN |
| AC-CAPITAL-001 | HIGH | OPEN |
| AC-STRAT-002 | HIGH | OPEN |
| AC-EVID-001 | HIGH | OPEN |

```text
OPEN_CRITICAL = 0
OPEN_HIGH = 4
OPEN_MEDIUM = 0
A_C_R4 = FAIL
RED_TEAM_R4_ELIGIBLE = NO
OWNER_REVIEW_ELIGIBLE = NO
```

## 9. Required Lifecycle

Because all four findings are semantic:

```text
REMEDIATE AC-GOV-001 / AC-CAPITAL-001 / AC-STRAT-002 / AC-EVID-001
-> FREEZE R5
-> FRESH A/C R5
-> FRESH RED-TEAM R5
-> OWNER REVIEW only if the same unchanged R5 passes both
```

## 10. Final Disposition

```text
FSATS_SIA_v0.1_R4_ARCHITECTURE_CONSISTENCY = FAIL
SEMANTIC_REMEDIATION_REQUIRED = YES
OWNER_ACCEPTANCE = NOT_ELIGIBLE
IMPLEMENTATION_AUTHORITY = NOT_GRANTED
```
