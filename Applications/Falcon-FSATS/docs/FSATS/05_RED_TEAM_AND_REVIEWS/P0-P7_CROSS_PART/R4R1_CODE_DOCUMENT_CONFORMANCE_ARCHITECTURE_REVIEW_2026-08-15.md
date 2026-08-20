# FSATS R4R1 Code-to-Document Architecture / Consistency Re-Review

**Date:** `2026-08-15`  
**Exact semantic source reviewed:** `6925a38a3476466fb847f2d0a87349fdb1ce23e9`  
**Predecessor Red Team:** `R4_CODE_DOCUMENT_CONFORMANCE_RED_TEAM_2026-08-15.md`  
**Review type:** fresh post-remediation static Architecture / Consistency review  
**Executable evidence:** `PENDING / SEPARATE`  
**Runtime authority:** `NOT_GRANTED`

## 1. Review purpose

This review verifies whether the exact post-R4 remediation source remains consistent with the controlling FSATS documents and current FCR semantics, with special attention to the four Medium findings raised by R4.

The review does not inherit executable PASS from any earlier source and does not grant Part 7, runtime, provider/broker egress, Paper, Shadow, Tiny-Live, Live, deployment, or execution authority.

## 2. Governing semantic surfaces reviewed

The review reconciled the changed source against the current Application-owned public contract documents and current FCR semantics, including:

- `applications/FSATS/contracts/web/FSATS.WebAnalysisAndStrategyContracts.v1.md`
- `applications/FSATS/contracts/web/FSATS.WebPortfolioContracts.v1.md`
- `applications/FSATS/contracts/web/FSATS.WebIncidentAffectedPositionAndShadowMonitoringContracts.v1.md`
- FCR-0125 Web/FSATS data-source separation
- FCR-0128 Strategy catalog applicability semantics
- FCR-0130 detailed analysis truth/synthesis semantics
- FCR-0133 portfolio/order/trade/performance semantics
- FCR-0201 affected-position/order and FSTSimA shadow-monitoring semantics
- current FSATS identity boundary: `FSATS_USER_ID = NONE`, `FSATS_CUSTOMER_ID = NONE`

## 3. Exact remediation delta

Compared with the R4 attacked source `aa61f45fae6b4593d387a85600996222a8ee7c55`, the exact semantic source contains post-R4 changes only within Application-owned paths.

The material remediation affects:

- typed FSTSimA shadow truth/freshness contracts;
- detailed-analysis truth/freshness invariant enforcement;
- portfolio no-source/null/empty-payload enforcement;
- historical R3 source-compatibility warning behavior;
- adversarial verification fixtures for each remediated invariant;
- FCR-0201 public contract documentation synchronization.

## 4. R4 finding closure matrix

### R4-RT-01 — FSTSimA top-level shadow truth/freshness

**Previous problem:** top-level `FreshnessState` was a free string and no strongly typed top-level shadow truth class existed.

**Remediation:** `WebEmergencyShadowMonitoringProjection` now carries:

```text
EmergencyShadowProjectionTruth ProjectionTruth
EmergencyShadowFreshnessState FreshnessState
```

Allowed top-level truth values are limited to:

```text
SIMULATOR
REPLAY
SYNTHETIC
TEST
```

Allowed freshness values are limited to:

```text
CURRENT
STALE
UNKNOWN
UNAVAILABLE
```

No broker-confirmed/live truth class exists in the FSTSimA top-level truth enum. The public contract document now explicitly states:

```text
SHADOW_PROJECTION_TRUTH != BROKER_TRUTH
CURRENT_SHADOW_FRESHNESS != CURRENT_BROKER_ACCOUNT_TRUTH
```

Adversarial coverage verifies that the top-level truth enum cannot expose broker/live truth names and that typed truth/freshness values are retained.

**Disposition:** `CLOSED_STATICALLY`.

### R4-RT-02 — contradictory detailed-analysis truth/freshness

**Previous problem:** `OverallTruthState=Current` could theoretically be constructed from `TruthState=Stale` plus `FreshnessState=Current`.

**Remediation:** current synthesis now requires both dimensions to be current:

```text
OverallTruthState = CURRENT
-> InputTruthFreshnessSummary.TruthState = CURRENT
-> InputTruthFreshnessSummary.FreshnessState = CURRENT
```

A new adversarial fixture explicitly constructs `TruthState=Stale` + `FreshnessState=Current` and requires constructor rejection.

This is consistent with the contract rule:

```text
STALE_OR_NONCURRENT_INPUT -> CURRENT_OVERALL_TRUTH = PROHIBITED
```

**Disposition:** `CLOSED_STATICALLY`.

### R4-RT-03 — portfolio no-source/null semantics

**Previous problem:** documentation prohibited fabricated zero/derived values when the projection was unsupported/not-applicable, but direct construction could still carry such business payload.

**Remediation:** `WebProjectionEnvelope.RequiresNoBusinessPayload` is true for `UNSUPPORTED` and `NOT_APPLICABLE`. Constructors now fail closed as follows:

```text
WebPortfolioSummaryProjection
  -> all numeric business values MUST be null

WebPositionCollectionProjection
  -> positions MUST be empty

WebOrderTradeActivityProjection
  -> activity MUST be empty

WebPortfolioPerformanceProjection
  -> all numeric business values MUST be null
  -> history MUST be empty
```

A dedicated `PortfolioNullSemanticsAdversarialChecks` suite attacks zero-filled unsupported summaries, nonempty unsupported position/activity collections, and numeric/history payload on not-applicable performance projections.

This directly enforces:

```text
NO_SOURCE_VALUE != ZERO
UNSUPPORTED / NOT_APPLICABLE != FABRICATED BUSINESS PAYLOAD
```

**Disposition:** `CLOSED_STATICALLY`.

### R4-RT-04 — compatibility markers and warnings-as-errors

**Previous problem:** `[Obsolete]` attributes on retained historical compatibility members could become compile errors because `TreatWarningsAsErrors=true`.

**Remediation:** historical R3 compatibility constants, types, and constructor remain available but no longer emit compiler-level Obsolete warnings. Their noncanonical status is expressed by comments/documentation rather than warnings.

A dedicated `CompatibilityWarningAdversarialChecks` suite verifies that the retained historical surfaces are present and are not decorated with `ObsoleteAttribute`.

The canonical current Web analysis identities remain unchanged:

```text
FSATS.WebOnDemandAnalysisRequest.v1
FSATS.WebOnDemandAnalysisResult.v1
FSATS.WebDetailedAssetAnalysisProjection.v1
```

**Disposition:** `CLOSED_STATICALLY`.

## 5. FCR-0201 consistency

The current source preserves separate Application-owned affected-position and affected-order projections and a separate FSTSimA diagnostic shadow projection.

The following boundaries remain explicit:

```text
POSITION_TRUTH != ORDER_TRUTH
ORDER_AMBIGUITY != INVENTED_POSITION_ID
FSTSIMA_SIMULATOR_EVIDENCE != BROKER_TRUTH
WEB_PRESENTATION != APPLICATION_BUSINESS_CLASSIFICATION
WEB_ACCOUNT_TO_CUSTOMER_MAPPING != FSATS_USER_IDENTITY
```

Position protection truth, affected-order truth, customer follow-up requirement, ordered action semantics, shadow start/end timing, scenario identity, typed shadow truth/freshness, provenance, and evidence all remain Application/FSTSimA supplied rather than Web-inferred.

Reconnect remains insufficient for incident release:

```text
RECONNECT != RECOVERED
RECONNECT != INCIDENT_RESOLVED
```

## 6. Cross-contract consistency

The review found no static contradiction among the current public contract documents and code for:

- Web presentation-only market data versus FSAPMA operational analysis data;
- canonical Request/Result/Detailed analysis identities;
- Strategy applicability and visible-disabled `NOT_APPLICABLE` behavior;
- exact broker-account identity;
- portfolio null/availability semantics;
- affected-position/order separation;
- FSTSimA diagnostic truth separation;
- no implicit runtime authority.

## 7. Compatibility note

Several portfolio projection records were changed from positional-record declarations to explicit validating record constructors while preserving their public type names, property names, property types, and constructor parameter order.

Static source review found no governing document that grants tuple-style positional deconstruction as a public semantic contract. Nevertheless, executable build/test verification remains mandatory before claiming source-level compatibility because compilation can reveal an in-repository caller that depended on compiler-generated positional-record members.

Therefore:

```text
STATIC SEMANTIC COMPATIBILITY = PASS
EXECUTABLE SOURCE COMPATIBILITY = PENDING EXACT BUILD/TEST
```

## 8. Architecture / consistency result

No open Critical, High, or Medium static architecture/consistency contradiction was found against the reviewed semantic source.

```text
R4R1 ARCHITECTURE / CONSISTENCY = PASS_STATIC
CRITICAL = 0
HIGH = 0
MEDIUM = 0
```

## 9. Evidence limitation

GitHub-hosted executable validation has not produced an exact run for this semantic source. Earlier runs were blocked before job execution by GitHub account billing/spending-limit state.

This review therefore does **not** claim:

```text
BUILD = PASS
TESTS = PASS
APPLICATION VERIFIERS = PASS
EXECUTABLE VALIDATION = PASS
```

Those claims require exact executable evidence for `6925a38a3476466fb847f2d0a87349fdb1ce23e9`.

## 10. Authority state

This review grants no new authority.

```text
PART 7 = NOT STARTED BY THIS REVIEW
RUNTIME AUTHORITY = NOT_GRANTED
PROVIDER/BROKER EGRESS AUTHORITY = NOT_GRANTED
PAPER = NOT_AUTHORIZED
SHADOW TRADING = NOT_AUTHORIZED
TINY LIVE = NOT_AUTHORIZED
LIVE = NOT_AUTHORIZED
DEPLOYMENT = NOT_AUTHORIZED
```
