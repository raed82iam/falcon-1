# FSATS SIA — Provider Route Fitness and Failover Formula Specification v1.0

**Package:** `FSATS-SIA-v0.1`
**Status:** `SEMANTIC REMEDIATION / DESIGN CANDIDATE`
**Triggered By:** `AC-PMA-001`
**Owner:** APP-PMA / P-LSA-04 with P-LSA-05/P-LSA-06 evidence

## 1. Purpose

Make provider-route selection deterministic for one exact Data Product demand without letting different adapters/controllers invent subscore normalization, switch thresholds or failover preference.

## 2. Route Candidate Key

One candidate route is identified by:

```text
ProviderId
ProviderProfileVersion
ProviderAccountProfileId
ProviderRouteId
CanonicalDataProductId/Version
MarketId
InstrumentScope/UniverseScope as applicable
OperationalEnvironment
```

Same provider with different plan/account/feed/route is a different route candidate.

## 3. Hard Eligibility Before Scoring

A route is scored only when all pass:

- provider/route certification current;
- requested canonical Data Product mapping certified;
- market/instrument scope supported;
- entitlement/license permits the use;
- Foundation external egress/credential capability available and exact route allowed;
- credentials/account profile valid;
- provider/route not Guardian-isolated;
- required quality profile/version compatible;
- quota has enough immediately reservable capacity for this request class or stream reservation;
- cost policy permits the route;
- route health is not OPEN/UNAVAILABLE;
- source semantics are not falsely represented as a broader product class.

Hard-ineligible route score is not 0; it is `INELIGIBLE` and cannot win.

## 4. Quality Score — Weight 30%

For the exact requested Data Product route, maintain a rolling route-quality projection from the most recent 100 canonical observations or previous 60 minutes, whichever contains fewer but at least 10 valid observations.

For each observation use the final P-LSA-05 `QualityScore` from 08A/08B.

RouteQualityScore = exponentially weighted mean in chronological order with:

```text
alpha = 0.10
EWMA_1 = first score
EWMA_t = alpha*score_t + (1-alpha)*EWMA_(t-1)
```

If fewer than 10 historical observations exist for a newly certified route:

```text
RouteQualityScore = CertificationQualityBaselineScore
```

where that baseline is a mandatory certification fixture result 0..10000. Missing baseline -> route not eligible.

Any current hard-invalid product state for the exact demanded observation/stream can make the route temporarily ineligible regardless of historical EWMA.

## 5. Freshness / Latency Score — Weight 20%

Two exact subscores:

### Current Freshness Fitness — 50%

For the latest required observation/update:

```text
Age = max(0, DecisionTime - Effective/Received time according to DataProduct freshness profile)
MaxAge = exact product/strategy route freshness bound
FreshnessFitness = round(10000 * clamp(1 - Age/MaxAge,0,1))
```

If Age >= MaxAge and the product hard rule says STALE, route cannot satisfy that current demand.

### Route P95 Latency Fitness — 50%

Over the latest 100 successful comparable route observations or previous 60 minutes, minimum 20 samples:

```text
P95Latency = nearest-rank 95th percentile under 17B
MaxAllowedLatency = exact DataProduct/strategy/provider route profile value
LatencyFitness = round(10000 * clamp(1 - P95Latency/MaxAllowedLatency,0,1))
```

If fewer than 20 runtime samples, use mandatory certification P95 fixture result. Missing -> ineligible.

```text
FreshnessLatencyScore = round(0.50*FreshnessFitness + 0.50*LatencyFitness)
```

## 6. Quota Headroom Score — Weight 20%

For the exact quota bucket governing the request/stream:

```text
Capacity = current allowed units for quota window
Used = confirmed consumed units
Reserved = admitted outstanding reservations
Headroom = Capacity - Used - Reserved
```

Precondition: `Capacity >0`, accounting current and reconstructable.

```text
HeadroomRatio = clamp(Headroom/Capacity,0,1)
QuotaHeadroomScore = round(10000*HeadroomRatio)
```

If current request cannot reserve its exact required units without exceeding capacity, route is hard-ineligible even if score otherwise high.

Unknown quota state -> ineligible, not unlimited.

For streaming/session limits, Capacity/Used/Reserved represent concurrent/session slots rather than request count; same formula applies only within a semantically compatible quota bucket.

## 7. Reliability Score — Weight 15%

Use exact INT-007 Provider Reliability Forecast v1.0 score from file 17:

```text
35% EWMA success
20% inverse error
15% inverse quality-failure
15% latency fitness
15% quota-headroom fitness
```

The route-level INT-007 instance is bound to the exact ProviderRouteId/account/product class.

No separate local P-LSA-04 reliability heuristic.

## 8. Cost Efficiency Score — Weight 10%

Provider certification exposes an exact `MarginalCostProfile` for the requested product/route:

```text
BillingCurrency
CostUnit = REQUEST | MB | STREAM_SESSION | FIXED_PLAN_ALLOCATED_UNIT
ExpectedMarginalCost
ApprovedMarginalCostCeiling
FreeWithinCurrentPlan = bool
```

Rules:

### Free current marginal use

```text
FreeWithinCurrentPlan = true
-> CostEfficiencyScore = 10000
```

provided quota/entitlement remains available.

### Paid marginal use

Requires explicit approved cost ceiling >0.

```text
ratio = ExpectedMarginalCost / ApprovedMarginalCostCeiling
CostEfficiencyScore = round(10000 * clamp(1-ratio,0,1))
```

If expected marginal cost > approved ceiling -> route hard-ineligible.

Unknown cost or no approved paid-cost authority -> paid route ineligible.

A sunk subscription fee does not make marginal use "free" unless certification marks the current unit as included in paid plan capacity.

## 9. Continuity Score — Weight 5%

Continuity prevents needless route flapping without protecting a degraded route from hard failure.

At evaluation:

```text
if route == current canonical route and health ELIGIBLE/HEALTHY:
    ContinuityScore = 10000
else if route is eligible and has been continuously healthy >=30 minutes:
    ContinuityScore = 8000
else if route is eligible and recovered 10..30 minutes ago:
    ContinuityScore = 6000
else if route is eligible and recovered <10 minutes ago:
    ContinuityScore = 4000
else:
    route ineligible
```

A route newly certified with no failure history uses 8000 after certification fixtures pass; it is not considered "recovered".

## 10. Final Route Score

```text
RouteScore = round(
  0.30*RouteQualityScore
+ 0.20*FreshnessLatencyScore
+ 0.20*QuotaHeadroomScore
+ 0.15*ReliabilityScore
+ 0.10*CostEfficiencyScore
+ 0.05*ContinuityScore
)
```

All component scores must be present. Unknown component with no declared certification fallback -> route ineligible.

## 11. Deterministic Sort

Among eligible candidates, descending:

1. RouteScore;
2. RouteQualityScore;
3. ReliabilityScore;
4. QuotaHeadroomScore;
5. CostEfficiencyScore;
6. canonical ProviderId ordinal;
7. canonical ProviderRouteId ordinal.

## 12. Switch Hysteresis

If current canonical route remains hard eligible:

Switch to the best alternative only when:

```text
AlternativeRouteScore >= CurrentRouteScore + 750
```

(7.5 percentage-point improvement).

If current route becomes hard-ineligible, hysteresis does not apply; select the highest-scoring eligible route immediately.

## 13. Route Failure / Failover

On current route hard failure:

```text
mark current route ineligible for exact affected scope
-> preserve failure evidence
-> reconcile any ambiguous outstanding request/delivery state
-> select best eligible alternate
-> do not duplicate a non-idempotent request until delivery/effect semantics permit retry
```

Data acquisition requests are retried only under their exact idempotency/freshness policy. A route failover does not permit sending old stale demand that would now produce misleading current data.

## 14. Multi-Provider Canonical Truth

Selecting an alternate route does not merge observations from multiple routes into one native Data Product.

P-LSA-05 may reconcile/corroborate according to Data Product comparison profiles. Exactly one canonical observation/result identity is published for the decision boundary according to the reconciliation policy.

## 15. Free-First Semantics

Historical FREE-FIRST intent is preserved **inside the score and hard cost authority**, not as a rule to choose an unusable free route.

A free route with lower quality/latency/quota may lose to another already-approved/included route if the total RouteScore and hard policy justify it.

A paid marginal route cannot become eligible merely because it scores well; paid cost authority must exist.

## 16. Quota Reservation

Before issuing a provider operation, P-LSA-04/P-LSA-06 atomically reserves the required quota unit/session slot against exact quota bucket version.

Selection score alone does not reserve quota.

If reservation fails due concurrency/state change:

- recompute candidate eligibility/score on new quota state;
- do not over-consume quota.

## 17. Stream Route Stability

For long-lived streaming Data Products, route selection is performed at stream-session establishment and on hard degradation/failure or scheduled certification/health reevaluation.

Do not switch every observation based on score.

If an alternate stream is used for comparison, it has separate route/session identity and does not become canonical until explicit route transition/reconciliation completes.

## 18. Verification Families

Verifier SHALL cover:

1. hard eligibility before score;
2. quality EWMA and certification fallback;
3. exact freshness score;
4. nearest-rank P95 latency and fallback;
5. quota headroom after reservations;
6. request cannot reserve -> ineligible;
7. INT-007 reliability reuse;
8. free vs paid cost formula/authority;
9. continuity state values;
10. exact weighted score;
11. deterministic tie-break;
12. 750-point switch hysteresis;
13. hard failure bypasses hysteresis;
14. ambiguous outstanding delivery not blindly duplicated;
15. atomic quota reservation race;
16. no multi-route native-payload mixing;
17. free-first does not override quality hard gates;
18. streaming route does not flap per message.

## 19. Finding Disposition

```text
AC-PMA-001 = REMEDIATED_AT_DESIGN_CANDIDATE_LEVEL
PROVIDER_ROUTE_SUBSCORES = EXACT v1
SWITCH_HYSTERESIS = EXACT v1
QUOTA/COST/FAILOVER = EXACT v1
```
