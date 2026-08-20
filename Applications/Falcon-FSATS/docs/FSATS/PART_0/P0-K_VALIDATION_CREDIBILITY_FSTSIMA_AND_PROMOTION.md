# P0-K - Validation, Credibility, FSTSimA and Promotion

**Status:** `OWNER_DIRECTED_INTEGRATED_REWRITE_CANDIDATE / NOT_OWNER_ACCEPTED / NOT_CLOSED`
**Implementation Authority:** `NOT_GRANTED`
**Runtime Authority:** `NOT_GRANTED`

## 1. Purpose

P0-K defines how Falcon establishes that a strategy, model, algorithm, component, Application change or Trading capability is credible enough for its exact Intended Use without converting test success into authority. It integrates independent validation dimensions, V&V/UQ, preregistration, FSTSimA non-Live isolation, Shadow/Paper/Tiny-Live separation, evidence freshness, reversible promotion and current FSA/Owner authority separation.

## 2. Prime rule

```text
VALIDATION = EVIDENCE_PROGRESSION
VALIDATION != AUTHORIZATION
PASS != NEXT_STAGE_AUTHORITY
```

Backtest, Replay, Stress, Shadow, Paper, Tiny-Live or any other stage establishes evidence only for a separately governed next decision.

## 3. Independent state dimensions

Never collapse independent states into one label. At minimum track separately:

### 3.1 Foundation lifecycle state
Registered, admission-reviewed, active, suspended, isolated, recovering or applicable current Foundation states.

### 3.2 Environment class
Development, sandbox, simulation, test, paper, tiny-live, live or separately defined environments.

### 3.3 Execution mode
No-execution, simulated, paper, bounded-real or live.

### 3.4 Validation stage
Research, backtest, replay, stress/adversarial, shadow, paper, tiny-live observation, post-promotion monitoring.

### 3.5 Business authorization context
Exact broker account, market, instrument, strategy, capital, Risk, Guardian and Owner/governance authority.

### 3.6 External authority context
Exact provider/broker/credential/egress/route authority.

```text
FOUNDATION_ACTIVE != LIVE_AUTHORIZED
PAPER_ENVIRONMENT != PAPER_BUSINESS_AUTHORITY
VALIDATION_STAGE_PASS != EXECUTION_MODE_AUTHORITY
```

## 4. FSTSimA as independent non-Live Application

FSTSimA is an independent Falcon Application with:

```text
MSA = 1
LSA = 8
CSA = 2
```

It remains independently identifiable, manifest/lifecycle/resource governed, contract-bound, removable/replaceable and self-aware under P0-C.

FSTSimA is not a mode inside Trading and cannot obtain Live broker credentials, Live execution routes or Live-authoritative egress merely because it consumes replay/simulation/test inputs.

Current enforceable non-Live isolation/egress remains a future Foundation Stage 12 dependency under FCR-0011.

```text
FSTSIMA_RUNTIME_SAFE_OPERATIONAL_CONNECTION_CLAIM = BLOCKED_PENDING_REQUIRED_FOUNDATION_CAPABILITY
```

## 5. FSTSimA responsibilities

Within non-Live jurisdiction FSTSimA owns scenario simulation, historical/replay testing, deterministic experiment execution where required, candidate challenge, execution/slippage/latency simulation, stress/tail testing, cross-market/regime regression, adversarial validation support, uncertainty/credibility evidence, reproducible artifacts and comparison to accepted baselines.

It does not own Trading business authority, Guardian protection, APP-RSC resource authority, Foundation lifecycle or production adoption.

## 6. Intended Use Claim

Every material target defines Intended Use before promotion-grade final validation. It includes as applicable:

- exact candidate identity/version/digest;
- purpose;
- owning Application/LSA/CSA;
- market/asset/instrument scope;
- regime;
- liquidity/volatility range;
- horizon/session;
- exact broker-account/environment scope where relevant;
- required Data Products/quality;
- model/strategy dependencies;
- execution assumptions;
- Risk envelope;
- prohibited conditions;
- unknown/unvalidated conditions;
- consequence severity;
- requested promotion ceiling.

```text
VALIDATION_SCOPE = INTENDED_USE_SCOPE
AUTHORITY_CANNOT_EXCEED_VALIDATED_SCOPE
SUCCESS_IN_SCOPE_A != AUTHORITY_IN_SCOPE_B
```

## 7. Credibility case

Promotion-grade evidence is a structured credibility case, not one scalar score. Dimensions may include conceptual/model correctness, implementation verification, input/data validity, scenario coverage and representativeness, calibration, uncertainty quantification, statistical reliability, robustness/sensitivity, failure/adversarial challenge, independent review, reproducibility, evidence freshness, operational comparability, execution realism, Intended Use fit, known limitations and residual risk.

A blocker in one critical dimension cannot be averaged away.

```text
CREDIBILITY_VECTOR != AVERAGE_SCORE_THAT_CAN_HIDE_BLOCKER
```

## 8. Verification, Validation and Uncertainty

Verification asks whether implementation correctly realizes the intended specification/model. Validation asks whether the model/capability is sufficiently credible for Intended Use. Uncertainty quantification characterizes uncertainty where applicable.

```text
VERIFICATION_PASS != VALIDATION_PASS
VALIDATION_PASS != ZERO_UNCERTAINTY
```

Unknowns and limitations remain visible.

## 9. Evidence progression ladder

A governed evidence progression may include:

```text
RESEARCH / DEVELOPMENT
-> BACKTEST
-> REPLAY
-> STRESS / ADVERSARIAL / DIGITAL-TWIN CHALLENGE
-> SHADOW
-> PAPER
-> TINY LIVE ONLY_WITH_SEPARATE_AUTHORITY
-> CONTROLLED SCALE / BROADER LIVE ONLY_WITH_SEPARATE_AUTHORITY
```

The exact sequence may vary with Intended Use and consequence, but stage skipping requires explicit justification and authority. The ladder is not an automatic state machine.

## 10. Exploratory trial versus promotion-grade experiment

Exploratory work may discover hypotheses, tune ideas and inspect failures. It must remain labeled exploratory and cannot be retroactively called preregistered promotion evidence because results are favorable.

A promotion-grade experiment preregisters as applicable candidate identity, Intended Use, primary/secondary metrics, failure criteria, sample/scenario requirements, comparison baseline, stopping rules, exclusions, analysis method, thresholds and allowed post-hoc analyses.

A material post-hoc change creates a revised/new experiment or exploratory result unless governed rules explicitly support it.

## 11. Producer-owned research lineage

Research trial lineage remains with the producing Application/branch/component. FSTSimA validates experiments but does not take ownership of producer business semantics.

There is no FSATS-wide mutable experiment-truth store that collapses ownership. Cross-Application validation evidence uses attributable governed contracts.

## 12. Simulation, replay and operational truth separation

```text
SIMULATION_TRUTH != OPERATIONAL_TRUTH
REPLAY_TRUTH != LIVE_TRUTH
PAPER_FILL != LIVE_FILL
SIMULATED_FILL != BROKER_FILL
```

Operational-like inputs do not give simulation/replay action authority.

## 13. Shadow validation

Shadow compares candidate behavior against current operational context without authoritative execution. Outputs remain explicitly non-authoritative.

It may compare intended decision versus actual approved decision, predicted versus observed outcome, latency/readiness, Risk, execution-plan differences, missed opportunities and errors.

```text
SHADOW_DECISION != LIVE_DECISION
```

## 14. Paper validation

Paper uses a non-Live execution environment/API only when separately authorized. Evidence accounts for differences from real execution such as fill model, queue position, slippage, spread, latency, market impact, partial fills, rejection, session behavior and simulator limitations.

```text
PAPER_SUCCESS != LIVE_READY_BY_DEFAULT
```

## 15. Tiny Live

Tiny Live is real bounded financial exposure and is not FSTSimA. It requires separate explicit authority, exact broker account/environment/capital, Unified Risk, Guardian and monitoring controls.

A Tiny-Live envelope defines exact candidate/Intended Use, maximum capital/exposure, maximum loss budget, market/instrument/strategy scope, duration/sample requirements, stop/restrict conditions, rollback/forward-recovery plan, independent monitoring and comparison to Paper/Shadow expectations.

```text
TINY_LIVE = REAL_MONEY_EXPOSURE
TINY_LIVE != FSTSIMA
TINY_LIVE_PASS != GENERAL_LIVE_AUTHORITY
```

No current Part 0 text grants Tiny-Live authority.

## 16. Paper/Live divergence measurement

Where Tiny Live is later authorized, compare Paper/Shadow to real outcomes, including fill rate, slippage, rejection, latency, partial fills, realized spread, execution cost, protection effectiveness, decision-to-fill drift and P&L differences attributable to execution environment.

Divergence is evidence for model/validation changes, never automatic authority expansion.

## 17. Statistical discipline

Promotion-grade evaluation must address data snooping, repeated tuning, multiple comparisons, leakage and selection bias proportionately to consequence.

```text
BEST_BACKTEST != EXPECTED_LIVE_EDGE
STATISTICAL_SIGNIFICANCE != ECONOMIC_SIGNIFICANCE
GROSS_EDGE != NET_EDGE
ONE_STATISTIC != COMPLETE_CREDIBILITY_CASE
```

Net-edge estimation includes material fees, spread, slippage, latency and capacity limits.

Methods such as Probability of Backtest Overfitting, Deflated Sharpe Ratio or other statistical techniques may be useful where appropriate but are not universally mandatory or sufficient unless separately adopted for exact use.

## 18. Determinism and provenance

Reproducible experiments bind exact code/artifact digest, configuration, dataset/Data Product identity, random seed/state initialization, numerical profile, dependency versions, scenario identity, time boundaries and produced evidence digests.

Similar headline metrics do not make unexplained executions equivalent evidence.

## 19. Independent validation

High-consequence candidate producer cannot be the sole verifier, validator, approver and promoter. Independence may involve another CSA/LSA perspective, parent MSA challenge, FSTSimA scenario execution, independent reviewer, Guardian/Risk/control challenge, FSA OS/governance review and Owner decision.

Independence is challenge/separation, not duplicated ownership.

## 20. Dynamic evidence sufficiency

No universal duration/sample count proves every model. Sufficiency is proportional to Intended Use, regime variability, frequency, consequence, statistical uncertainty, independent observations, tail/failure coverage, interaction complexity, novelty and irreversibility.

Calendar duration alone does not substitute for scenario/sample sufficiency.

## 21. Evidence freshness

Evidence may become stale after market/regime change, provider/broker behavior change, model/strategy/code/config change, Risk policy change, execution environment change, instrument/universe change, infrastructure/latency change, external rule/market structure change or discovered data/model defect.

Stale evidence is restricted/revalidated, never silently reused.

## 22. Continuing validity after promotion

Post-adoption monitoring detects performance decay, drift, out-of-scope conditions, Risk degradation, unexpected interactions, execution divergence, data dependency change, increased uncertainty and protection anomalies.

Possible outcomes:

```text
CONFIRM
HOLD
RESTRICT
DEMOTE
REVOKE
RETEST
ROLLBACK / FORWARD_RECOVER
```

Successful history creates no unlimited future trust.

## 23. Promotion state separation

Conceptual chain:

```text
EVIDENCE_READY
-> APPLICATION_EVALUATION_COMPLETE
-> INDEPENDENT_VALIDATION_COMPLETE
-> FSA_OS_GOVERNANCE_REVIEW_COMPLETE_WHERE_REQUIRED_AND_AVAILABLE
-> EXPLICIT_OWNER_OR_VALID_DELEGATED_GOVERNANCE_AUTHORITY
-> APP-001 / MANIFEST / LIFECYCLE ELIGIBILITY
-> BOUNDED_PROMOTION
-> OBSERVE
-> CONFIRM / HOLD / RESTRICT / DEMOTE / REVOKE
```

Each transition requires its own evidence/authority.

```text
FSA_REVIEW != PRODUCTION_ADOPTION
OWNER_SILENCE != AUTHORITY
TIMER_EXPIRY != AUTHORITY
```

FSA runtime/control-plane dependencies remain Foundation-owned under FCR-0012/FCR-0030 Stage 13.

## 24. Hard safety envelope

Candidate confidence/performance never weakens independent hard protection boundaries such as Owner risk ceilings, Guardian restrictions, credential/security boundaries, lifecycle restrictions, non-Live isolation, broker-account/environment binding or maximum autonomous experiment loss/exposure.

```text
MODEL_CONFIDENCE != SAFETY_ENVELOPE_OVERRIDE
```

## 25. Legacy numeric gates

Historical V1.3 numeric thresholds are provenance/examples only, not current defaults, fallback values, proof thresholds, promotion criteria or authority.

```text
V1_3_NUMERIC_GATE = HISTORICAL_REFERENCE_ONLY
```

Current criteria must be justified by Intended Use, evidence, consequence and current Owner/governance policy.

## 26. Cross-market and regime regression

A candidate validated on one market/instrument/regime cannot silently generalize. Scope expansion requires explicit new evidence.

For US Equities and Crypto Spot, validation preserves market-specific liquidity, session, microstructure, fee, volatility and execution differences rather than averaging them away.

## 27. Interaction and cumulative-change testing

A candidate that passes alone may fail combined with other changes. Test cumulative interactions with strategy competition, Risk, portfolio/capital, APP-RSC pressure/coordination, provider degradation, Guardian restriction and relevant Foundation degradation.

## 28. Failure and invalid-evidence behavior

Examples:

- failed mandatory test -> no progression;
- insufficient samples/scenarios -> HOLD/RETEST;
- material post-hoc scope change -> restart final validation;
- stale evidence -> restrict/revalidate;
- Paper/Live divergence beyond accepted bounds -> HOLD/RESTRICT/RETEST;
- material independent-validator disagreement -> unresolved, no promotion;
- FSTSimA non-Live isolation unproven -> no operational connection claim;
- missing Foundation runtime dependency -> design may remain but runtime promotion is blocked;
- stale/missing data, provider outage, broker ambiguity, partial fill, resource pressure, restart, authority conflict, duplicate/replay or degraded Foundation behavior -> explicit scenario/failure evidence required.

## 29. Foundation/FCR dependencies

Material dependencies include FCR-0011 FSTSimA non-Live isolation/egress, FCR-0008 research-only Internet, FCR-0014 broker egress before future Tiny-Live/Live, FCR-0012/FCR-0030 FSA control-plane/review binding, current event/evidence delivery capabilities and APP-001/CON-023 lifecycle/update/admission.

No missing Foundation capability is locally substituted.

## 30. Explicit non-authority

P0-K SHALL NOT grant Paper/Tiny-Live/Live, let FSTSimA acquire Live authority, let PASS auto-promote, let one score hide a blocker, let producer be sole high-consequence approver, convert simulation/replay to operational truth, represent Paper fills as real fills, use historical numeric gates as current defaults, let a canary expand authority ceilings or let stale evidence remain promotion-grade without revalidation.

## 31. Invariants

```text
VALIDATION != AUTHORIZATION
PASS != NEXT_STAGE_AUTHORITY
FOUNDATION_ACTIVE != LIVE_AUTHORIZED
FSTSIMA = INDEPENDENT_NON_LIVE_APPLICATION
FSTSIMA_MSA = 1
FSTSIMA_LSA = 8
FSTSIMA_CSA = 2
FSTSIMA != TINY_LIVE
SIMULATION_TRUTH != OPERATIONAL_TRUTH
PAPER_TRUTH != LIVE_TRUTH
SHADOW_DECISION != LIVE_DECISION
PAPER_SUCCESS != LIVE_READY_BY_DEFAULT
TINY_LIVE = REAL_MONEY_EXPOSURE
TINY_LIVE_PASS != GENERAL_LIVE_AUTHORITY
VALIDATED_SCOPE = MAXIMUM_PROMOTABLE_SCOPE_WITHOUT_NEW_VALIDATION
CREDIBILITY_VECTOR != AVERAGE_SCORE_THAT_HIDES_BLOCKER
MODEL_CONFIDENCE != SAFETY_ENVELOPE_OVERRIDE
V1_3_NUMERIC_GATE = HISTORICAL_REFERENCE_ONLY
```

## 32. Forbidden interpretations

Invalid: FSTSimA is a Trading mode; Paper passed so start Tiny Live; Tiny Live passed so scale; Foundation ACTIVE means Live; average credibility hides failed security; exploratory trial becomes preregistered after good result; one good month proves low-frequency model; one statistic is approval; high model confidence relaxes Guardian/Risk; old V1.3 threshold remains default.

## 33. Mandatory scenarios

Challenge backtest overfit; leakage; insufficient regime diversity; stale evidence; post-hoc metric change; dataset revision; provider behavior change; simulated fill optimism; Shadow disagreement; Paper/Live divergence; Tiny-Live stop condition; candidate outside Intended Use; cross-market scope expansion; interaction with APP-RSC resource pressure; Guardian restriction during validation; FSTSimA egress/isolation failure; duplicate/replay evidence; deterministic rerun mismatch; independent validator disagreement; and rollback/forward-recovery evidence failure.

## 34. Exit gates

```text
INTENDED_USE = EXPLICIT
CREDIBILITY_CASE = COMPLETE_FOR_CONSEQUENCE
VERIFICATION_VALIDATION_UNCERTAINTY = SEPARATED
EXPLORATORY_VS_PROMOTION_EVIDENCE = SEPARATED
FSTSIMA_NON_LIVE_BOUNDARY = EXPLICIT
SHADOW_PAPER_TINY_LIVE_LIVE = SEPARATED
EVIDENCE_FRESHNESS = EXPLICIT
CROSS_MARKET_SCOPE_EXPANSION = GOVERNED
PASS_TO_AUTHORITY_SHORTCUTS = 0
FCR0011_STATE = EXPLICIT_AND_FAIL_CLOSED
```

## 35. Non-grant

Acceptance of P0-K would establish validation/credibility/promotion design only. It would not grant Paper, Shadow, Tiny-Live, Live, provider/broker connectivity or deployment.