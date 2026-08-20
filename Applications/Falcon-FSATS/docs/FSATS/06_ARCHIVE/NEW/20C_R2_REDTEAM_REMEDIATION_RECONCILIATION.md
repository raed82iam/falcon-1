# FSATS SIA — R2 Red-Team Remediation Reconciliation

**Package:** `FSATS-SIA-v0.1`
**Record Type:** `PRE-R3 SEMANTIC RECONCILIATION`
**R2 Red-Team:** `22_FRESH_RED_TEAM_REVIEW_R2.md` = FAIL

## 1. Purpose

Bind every R2 Red-Team finding to an explicit semantic remediation before creating the next freeze. This record does not claim PASS; it proves the remediations exist and identifies the exact changed semantics requiring fresh R3 reviews.

## 2. RT-DATA-001 — HIGH

### Finding

Initial Data Product set/field/time/correction/quality semantics were not fully materialized.

### Remediation

- `08A_INITIAL_CANONICAL_DATA_PRODUCT_AND_QUALITY_PROFILE.md`
- `08B_DATA_QUALITY_DIMENSION_APPLICABILITY_PROFILE.md`
- `08C_CROSS_SOURCE_COMPARISON_PROFILE_RULES.md`

### Result

```text
Initial DataProduct IDs = DP-001..DP-010
Core Trading DataProducts = DP-001..DP-008
Payload schemas = explicit
Bar/session/time semantics = explicit
Trade correction lineage = explicit
L2 reconstruction = explicit
News/event normalization = explicit
Quality thresholds = 8000 VALID / 6500 DEGRADED floor
New-risk required product state = VALID
Quality dimension applicability = explicit by product
Cross-source independence != semantic comparability
Cross-source comparison requires exact profile/tolerance evidence
```

Status: `REMEDIATED_AT_DESIGN_CANDIDATE_LEVEL`.

## 3. RT-SIM-001 — HIGH

### Finding

Reproducibility still depended on unspecified PRNG/distribution/event-priority/numerical behavior.

### Remediation

`10A_FSTSIMA_DETERMINISTIC_RANDOMNESS_AND_NUMERICS_PROFILE.md`

### Result

```text
PRNG = xoshiro256**
seed expansion = SplitMix64
stream derivation = SHA-256 over canonical master-seed bytes + NFC UTF-8 name
U(0,1) conversion = exact 53-bit rule
normal = Box-Muller first variate, no cached partner
Student-t = normal / sqrt(sum(nu normals^2)/nu)
equal-time scheduler priority = exact numeric order
checkpoint includes full RNG state + draw count
canonical stochastic output quantization = explicit
```

Status: `REMEDIATED_AT_DESIGN_CANDIDATE_LEVEL`.

## 4. RT-STRAT-001 — HIGH

### Finding

HNT-004/HNT-006/statistical/NetEdge paths still allowed materially different implementations.

### Remediation

`17B_STATISTICAL_PRIMITIVE_AND_NET_EDGE_ESTIMATOR_SPEC.md`

### Result

```text
sample stddev = n-1 exact
nearest-rank percentile = exact
cross-sectional mid-rank = exact
Pearson correlation = exact
HNT-006 alignment = exact
OLS alpha/beta = exact
AR(1) half-life = exact
HNT-004 midpoint/tick direction classification = exact
large-trade P95 reference window/sample = exact
US/crypto VWAP boundary = exact
weighted 35th target quantile = exact
NetEdge historical outcome estimator + conservative prior = exact
```

Status: `REMEDIATED_AT_DESIGN_CANDIDATE_LEVEL`.

## 5. RT-GRD-001 — MEDIUM

### Finding

Guardian generic directive actions lacked exact action-specific required/forbidden parameters.

### Remediation

`09A_GUARDIAN_DIRECTIVE_ACTION_PARAMETER_SPEC.md`

### Result

Discriminated schemas now exist for:

- RESTRICT_NEW_RISK;
- REDUCE_ALLOWED_EXPOSURE;
- SUSPEND_STRATEGY_SCOPE;
- SUSPEND_INSTRUMENT_SCOPE;
- SUSPEND_MARKET_SCOPE;
- CANCEL_OPEN_ORDERS;
- EXIT_POSITION_SCOPE;
- HOLD_PROMOTION;
- provider isolation/restriction/multi-source confirmation.

`REQUEST_RESOURCE_PRIORITY` is explicitly removed as a protection-command action and represented instead as Guardian resource-demand evidence through APP-RSC when available.

Status: `REMEDIATED_AT_DESIGN_CANDIDATE_LEVEL`.

## 6. RT-RISK-002 — MEDIUM

### Finding

Non-base quote-asset conversion path was required but not defined.

### Remediation

`07C_INITIAL_RISK_BASE_CURRENCY_AND_CRYPTO_QUOTE_POLICY.md`

### Result

```text
Initial RiskBaseCurrency = USD
Initial US Equities quote currency = USD only
Initial Crypto Spot quote asset = USD only
Stablecoin != USD by assumption
Multi-hop conversion = out of scope v1
unknown/non-USD unexpected balance = reconciliation required, not zero
```

This deliberately constrains initial scope instead of inventing a conversion graph.

Status: `REMEDIATED_AT_DESIGN_CANDIDATE_LEVEL`.

## 7. Additional Data Hardening

During remediation cross-check, Data Quality source-comparison ambiguity was tightened before R3:

- two independent sources are not automatically semantically comparable;
- cross-source scoring requires `CrossSourceComparisonProfileId` with exact comparison method/tolerance evidence;
- no coding-worker default tolerance;
- Guardian-required multi-source mode fails closed when the profile/sources are unavailable.

This is included in 08B/08C and is part of the R3 semantic delta.

## 8. R2 Review Consequence

R2 remains preserved as:

```text
A/C R2 = PASS before Red-Team
Red-Team R2 = FAIL
R2 OWNER REVIEW = NOT_ELIGIBLE
```

The semantic remediations above prevent reuse of R2 A/C for final review.

Required next lifecycle:

```text
R3 SEMANTIC FREEZE
-> FRESH A/C R3
-> FRESH RED-TEAM R3
-> OWNER REVIEW only on unchanged R3 freeze if both pass
```

## 9. Open Finding Count Before R3 Freeze

At remediation-reconciliation level:

```text
RT-DATA-001 = REMEDIATED
RT-SIM-001 = REMEDIATED
RT-STRAT-001 = REMEDIATED
RT-GRD-001 = REMEDIATED
RT-RISK-002 = REMEDIATED

KNOWN OPEN CRITICAL = 0
KNOWN OPEN HIGH = 0
KNOWN OPEN MEDIUM = 0
```

This is not a review PASS. R3 Red-Team must independently retest the new semantics.
