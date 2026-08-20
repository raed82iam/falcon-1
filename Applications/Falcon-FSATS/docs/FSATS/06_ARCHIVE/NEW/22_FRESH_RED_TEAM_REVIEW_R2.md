# FSATS SIA v0.1 R2 — Fresh Red-Team Review

**Review Type:** `FRESH STATIC ADVERSARIAL / RED-TEAM REVIEW`
**Reviewed Freeze:** `FSATS-SIA-v0.1-R2`
**Freeze Manifest:** `00_CURRENT_SIA_MASTER_AND_SEMANTIC_FREEZE_R2.md`
**Prerequisite A/C:** `21A_ARCHITECTURE_AND_CONSISTENCY_REVIEW_R2.md` = PASS
**Status:** `FAIL / SEMANTIC REMEDIATION REQUIRED`
**Owner Acceptance:** `NOT_ELIGIBLE`
**Implementation Authority:** `NOT_GRANTED`

## 1. Purpose

Attack the R2 package as though a coding worker, hostile integration, race condition, stale provider, inconsistent market, faulty AI component or ambiguous external response would exploit every unspecified semantic.

This Red-Team does not reward file count or design detail. A material behavior that still requires implementation-time invention is a finding.

## 2. Adversarial Families Challenged

The review challenged at minimum:

- source/authority inversion;
- hidden Application ownership;
- cross-App direct coupling;
- contract omission/alias confusion;
- forged/replayed authority;
- Paper/Replay/Simulation -> Live confusion;
- provider/broker ambiguity;
- Data Product identity/quality/freshness substitution;
- strategy look-ahead/statistical ambiguity;
- Risk/capital clock/accounting manipulation;
- order/cancel/fill race conditions;
- persistence/outbox/inbox inconsistency;
- Guardian command/effect/release confusion;
- FSARM double allocation/split brain/minimum inflation;
- FSTSimA reproducibility/randomness drift;
- CSA/Monitor self-expansion;
- research/operational-data collapse;
- queue/overload/retry storms;
- configuration drift;
- future Foundation capability workaround attempts.

## 3. Previously Found Risk Issue

`RT-RISK-001` from the R1 attack was remediated by 07B and re-reviewed by A/C R2.

R2 retest result for:

- UTC risk clock;
- cash-flow-adjusted RiskEquity;
- tail-aware sizing;
- unconfirmed-exit risk;
- sample-count integrity;

= `PASS` at static design level.

## 4. RT-DATA-001 — Canonical Operational Data Product Set Is Not Fully Materialized

**Severity:** `HIGH`
**Status:** `OPEN`

### Attack

A coding worker implements `P-LSA-02` and `P-LSA-05` from files 08/12/16/17.

The architecture defines a strong generic DataProduct record and quality pipeline, but it does not yet provide one exact canonical initial catalog for the products the strategies actually consume.

The worker must still decide, among other things:

- exact product IDs/versions for quote/trade/bar/session/instrument/corporate-action/order-book/news;
- exact payload fields and units;
- whether bar end time is inclusive/exclusive and when a bar becomes complete;
- exact trade correction/cancel semantics;
- exact quote validity/crossed/locked behavior;
- exact order-book level semantics/aggregation;
- exact required freshness/quality floor per product class;
- exact normalized event/news fields consumed by HNT-008;
- exact hard invalidity conditions vs score-based degradation;
- whether two superficially similar products can be substituted.

### Exploit / Failure

Two adapters can emit different bar/session/trade semantics under the same generic product label, allowing simulation/strategy parity to pass locally while production uses semantically different bytes.

### Required remediation

Create an initial canonical Data Product + quality profile containing at least:

- product IDs/versions;
- payload schemas/types/units;
- interval/time semantics;
- correction/duplicate rules;
- provider mapping expectations;
- freshness/quality floors;
- operational/replay/simulation classification;
- strategy requirement mapping;
- negative fixtures.

## 5. RT-SIM-001 — Reproducibility Claims Depend On Unspecified Random/Distribution Algorithms

**Severity:** `HIGH`
**Status:** `OPEN`

### Attack

FSTSimA R2 declares:

- named random streams;
- stable hash derivation;
- Student-t scenario draws;
- exact replay/reproducibility.

But the freeze does not fix the initial:

- PRNG algorithm/version;
- master-seed canonical byte encoding;
- stream-name seed derivation algorithm;
- uniform integer -> decimal conversion;
- normal/gamma/Student-t sampling procedure;
- event-priority enum/order for equal timestamps;
- floating/numeric rounding behavior for stochastic model outputs.

### Exploit / Failure

Two correct-looking .NET implementations can use different runtime RNG/distribution libraries and produce different scenario sequences from the same `MasterSeed`, breaking cross-version reproducibility while both claim compliance.

### Required remediation

Define one portable deterministic initial randomness/numerics profile and golden vectors. A later RNG/distribution algorithm must be a new profile/version and is part of RunDefinition identity.

## 6. RT-STRAT-001 — Statistical Primitives Still Permit Multiple Material Implementations

**Severity:** `HIGH`
**Status:** `OPEN`

### Attack

File 17 is substantially more exact than the old blueprint, but specific statistical paths remain implementation-variable:

### HNT-004

- trade buy/sell classification method is only described as quote/trade classification;
- trailing 95th-percentile trade-size calculation method/tie rule is not exact;
- rolling VWAP/reset boundary is not exact for all market profiles.

### HNT-006

- regression alpha/beta fitting method is not explicit;
- residual half-life estimator is not explicit;
- aligned missing-observation handling is not explicit;
- percentile/correlation numerical/tie method is not exact.

### NetEdge / target probabilities

`ExpectedGrossRewardR` and `ExpectedLossR` reference probability-weighted outcomes, but the exact initial estimator for target/stop probabilities from calibration evidence is not defined.

### Exploit / Failure

Different developers/libraries can produce different signals, NetEdge and order eligibility from identical market evidence while satisfying the prose.

### Required remediation

Create exact statistical primitive/estimator definitions and golden vectors for these paths.

## 7. RT-GRD-001 — Guardian Directive Action-Specific Parameters Are Not Schema-Closed

**Severity:** `MEDIUM`
**Status:** `OPEN`

### Attack

The generic protection directive schema contains `Action`, scope, authority and evidence, but actions such as:

- `REDUCE_ALLOWED_EXPOSURE`;
- `SUSPEND_STRATEGY_SCOPE`;
- `SUSPEND_INSTRUMENT_SCOPE`;
- `SUSPEND_MARKET_SCOPE`;
- `EXIT_POSITION_SCOPE`;
- `REQUEST_RESOURCE_PRIORITY`;

require exact action-specific parameter rules.

For example, a `REDUCE_ALLOWED_EXPOSURE` directive without an exact quantity/notional/risk ceiling/unit could be interpreted differently by Trading.

### Required remediation

Define a discriminated action parameter union/schema, required/forbidden fields per action and exact target/effect acknowledgement semantics.

## 8. RT-RISK-002 — Non-Base Quote-Asset Valuation Path Is Deferred To An Unspecified Deterministic Policy

**Severity:** `MEDIUM`
**Status:** `OPEN`

### Attack

07B correctly requires a deterministic conversion path for positions/assets not directly quoted in RiskBaseCurrency, but it does not define the initial path-selection algorithm.

### Exploit / Failure

Two implementations may choose different conversion chains/providers and produce different NAV/RiskEquity/drawdown for the same crypto holdings.

### Required remediation options

Either:

A. initial profile restricts eligible Trading instruments to quote assets directly equal to RiskBaseCurrency; or

B. define a canonical conversion graph/path-selection algorithm with freshness/quality/tie-break rules.

The simpler initial architecture may choose A while leaving B for a later profile.

## 9. Findings That Passed Adversarial Challenge

No new finding was opened against these R2 subjects:

- P0-F 43/43 contract inventory preservation;
- APP-001 no hidden current Application ownership;
- APP-RSC candidate is explicit rather than hidden;
- cross-App implementation/database access prohibition;
- Manifest/FCR fail-closed seams;
- Risk cash-flow/time/tail remediation;
- capital reservation atomicity;
- ambiguous broker reconciliation-before-retry;
- Guardian detection/incident/authority/effect separation;
- FSARM request != grant / reclaim before reassignment / coordinator fencing;
- Trading MSA direct-Internet prohibition;
- non-Trading future research eligibility under FCR-0008;
- FSA Foundation ownership;
- Monitor disagreement/no majority-vote rule;
- CSA protected-property boundary;
- FSTSimA non-Live authority separation (reproducibility algorithm detail remains a separate finding);
- bounded queue/overload rules;
- provider/broker point-in-time certification gate.

## 10. Severity Summary

| ID | Severity | Status |
|---|---|---|
| RT-DATA-001 | HIGH | OPEN |
| RT-SIM-001 | HIGH | OPEN |
| RT-STRAT-001 | HIGH | OPEN |
| RT-GRD-001 | MEDIUM | OPEN |
| RT-RISK-002 | MEDIUM | OPEN |

```text
OPEN_CRITICAL = 0
OPEN_HIGH = 3
OPEN_MEDIUM = 2
RED_TEAM_R2 = FAIL
OWNER_REVIEW_ELIGIBLE = NO
```

## 11. Required Governance Action

Because the required fixes are semantic, the existing R2 freeze cannot be patched and still retain its review status.

Required sequence:

```text
REMEDIATE RT-DATA-001 / RT-SIM-001 / RT-STRAT-001 / RT-GRD-001 / RT-RISK-002
-> NEW SEMANTIC FREEZE R3
-> FRESH A/C R3
-> FRESH RED-TEAM R3
-> OWNER REVIEW only if both pass and no later semantic change
```

## 12. Final R2 Red-Team Disposition

```text
FSATS_SIA_v0.1_R2_RED_TEAM = FAIL
REMEDIATION_REQUIRED = YES
OWNER_ACCEPTANCE_GATE = BLOCKED
IMPLEMENTATION_AUTHORITY = NOT_GRANTED
```
