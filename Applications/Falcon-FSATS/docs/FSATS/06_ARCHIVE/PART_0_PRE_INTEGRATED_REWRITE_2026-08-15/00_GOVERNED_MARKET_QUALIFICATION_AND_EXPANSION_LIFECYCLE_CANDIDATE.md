# FSATS — Governed Market Qualification and Expansion Lifecycle Candidate

**Package:** `FSATS-MARKET-QUALIFICATION-PROPOSAL-001`  
**Decision Type:** `PROJECT OWNER DIRECTED DESIGN CHANGE / NEW-MARKET QUALIFICATION`  
**Classification:** `DCC-3 — MATERIAL_DOMAIN_CHANGE`  
**Status:** `OWNER-DIRECTED DESIGN CANDIDATE / OUTSIDE_CURRENT_R7_FREEZE / NOT_OWNER_ACCEPTED / NOT_CLOSED`  
**Branch:** `application-development`  
**Workspace:** `applications/docs/FSATS/NEW-3/`  
**Implementation Authority:** `NOT_GRANTED`  
**Runtime Authority:** `NOT_GRANTED`  
**Research Internet Egress Authority:** `NOT_GRANTED`  
**Provider / Broker Connectivity Authority:** `NOT_GRANTED`  
**Paper / Tiny Live / Live / Deployment Authority:** `NOT_GRANTED`

---

# 1. Purpose

This candidate materializes the Project Owner's intended future behavior for an instruction such as:

```text
ADD MARKET X
```

The instruction SHALL mean:

```text
AUTHORIZE A BOUNDED NON-LIVE MARKET-QUALIFICATION STUDY AND CANDIDATE-ENGINEERING WORKFLOW FOR MARKET X
```

It SHALL NOT mean:

```text
ADMIT MARKET X FOR TRADING
AUTHORIZE PAPER
AUTHORIZE TINY LIVE
AUTHORIZE LIVE
AUTHORIZE DEPLOYMENT
AUTHORIZE PROVIDER OR BROKER CONNECTIVITY
EXPAND RISK / CAPITAL / EXECUTION AUTHORITY
```

The objective is to allow Falcon's existing specialized intelligence to study a new market deeply, construct the necessary market-specific and cross-cutting candidates, prove them in governed non-Live conditions, remediate failures iteratively, and return an evidence-backed readiness recommendation to the Project Owner.

---

# 2. Controlling Owner Intent

The Project Owner's intended interaction is:

```text
OWNER
  "ADD MARKET X"
        |
        v
BOUNDED MARKET QUALIFICATION MANDATE
        |
        v
SPECIALIZED APPLICATION / LSA / ELIGIBLE CSA WORK
        |
        v
NON-LIVE CANDIDATES
        |
        v
FSTSimA RESEARCH / SIMULATION / TEST / CHALLENGE / VALIDATION
        |
        v
FAIL -> FINDING -> OWNING INTELLIGENCE REMEDIATES -> RETEST
        |
        v
EVIDENCE SUFFICIENT OR HONEST BLOCKER
        |
        v
OWNER-FACING MARKET QUALIFICATION RESULT
```

The preferred successful terminal recommendation is conceptually:

```text
READY_FOR_PAPER_REVIEW
```

but this is a readiness recommendation only.

Mandatory invariant:

```text
READY_FOR_PAPER_REVIEW != PAPER_AUTHORIZED
```

---

# 3. Relationship to Current Accepted FSATS Semantics

This candidate builds prospectively on already accepted principles without rewriting accepted Part 0 history.

Current accepted design already establishes that:

- initial markets are `US_EQUITIES + CRYPTO_SPOT`;
- a later market or asset class is a separately governed scope expansion;
- each admitted market requires a governed Market Profile;
- Trading strategies remain centrally registered rather than copied per market;
- Unified Risk remains Trading-owned;
- Trading Execution remains Trading-owned;
- FSAPMA remains provider/data business owner;
- Guardian remains independent protection/crisis owner;
- FSTSimA remains an independent non-Live simulation/validation Application;
- FSTSimA validation does not create promotion authority;
- Application Awareness may research, identify gaps, develop isolated candidates, test them, produce evidence and recommend them only within governed authority;
- FSA review is OS-governance/compatibility review and is not production adoption;
- final adoption/activation remains separately governed.

This candidate adds the missing end-to-end orchestration and Owner command/result semantics for new-market qualification.

It does not convert this workflow into accepted design until it completes the mandatory fresh review cycle and receives explicit Owner acceptance.

---

# 4. Qualification Is a DCC-3 Market-Scope Change

A new market changes Trading domain scope and may affect data, strategy applicability, Risk, capital, broker/execution and protection assumptions.

Therefore the market-qualification package SHALL be classified no lower than:

```text
DCC-3 — MATERIAL_DOMAIN_CHANGE
```

The Owner instruction to `ADD MARKET X` authorizes the bounded study/candidate scope described by this lifecycle only.

It does not pre-approve the resulting market candidate and does not make DCC-3 timer/no-veto or autonomous promotion semantics applicable.

```text
OWNER AUTHORIZES QUALIFICATION
!=
OWNER ACCEPTS RESULTING MARKET
```

---

# 5. Core Operating Principle

Every specialized intelligence continues to own only its legitimate responsibility.

For a new market, each relevant owner SHALL do its own work, but any newly created or materially adapted Trading-domain artifact intended to support the market SHALL first exist as a non-Live candidate and shall be challenged in FSTSimA before it can support a recommendation for Paper review.

Conceptually:

```text
THINK
-> BUILD CANDIDATE
-> FSTSimA
-> TEST / CHALLENGE
-> FAIL / FINDING
-> OWNER REMEDIATES
-> RETEST
-> EVIDENCE
-> APPLICATION EVALUATION
-> READY FOR NEXT OWNER DECISION OR HONEST HOLD/REJECTION
```

This is an iterative evidence loop, not a one-pass checklist.

---

# 6. Ownership Must Not Collapse Into FSTSimA

The Project Owner wants initial development/testing to occur in the simulator, but FSTSimA SHALL NOT become the owner of every business semantic it tests.

Mandatory separation:

```text
FSTSIMA = QUALIFICATION LABORATORY + SIMULATION ENVIRONMENT + FSTSIMA EVIDENCE OWNER
TARGET APPLICATION / LSA = TARGET BUSINESS SEMANTIC OWNER
```

Examples:

```text
FSTSimA tests a Market Profile candidate
-> T-LSA-02 still owns Market Profile business truth

FSTSimA tests a Risk candidate
-> T-LSA-07 still owns Unified Risk business semantics

FSTSimA tests a strategy candidate
-> Trading strategy owners / central Strategy Catalog retain strategy ownership

FSTSimA tests provider behavior
-> FSAPMA still owns provider/data business semantics

FSTSimA tests broker/execution behavior
-> T-LSA-09 still owns Trading execution business semantics

FSTSimA injects crisis conditions
-> Guardian still owns real protection/crisis authority
```

FSTSimA SHALL NOT directly rewrite the target Application's authoritative state or internals.

Candidate transfer and validation evidence exchange require governed cross-Application contracts/routes when implementation is later authorized and the required Foundation capabilities exist.

---

# 7. Application-Level Coordination

For a Trading-market expansion, the Falcon Self-Aware Trading Application remains the owning business Application for the market admission candidate.

Trading MSA SHALL be the Application-level evaluator/coordinator of the complete Trading-domain qualification package.

Trading MSA may coordinate requests to its own LSAs and may request governed supporting work/evidence from FSAPMA, FSTSimA and Guardian through cross-Application boundaries.

Trading MSA SHALL NOT:

- become FSAPMA MSA;
- become Simulation MSA;
- become Guardian MSA;
- directly modify another Application's internals;
- create Foundation capability;
- self-admit the market;
- authorize Paper/Tiny Live/Live;
- bypass FSA/Owner/governance where required.

```text
COORDINATION != OWNERSHIP COLLAPSE
```

---

# 8. Market Qualification Request

The future exact implementation contract remains separately gated, but the semantic request shall carry enough information to prevent authority ambiguity.

Conceptual artifact:

```text
MarketQualificationRequest
{
    RequestId
    OwnerCommandEvidenceRef
    TargetMarketIdentity
    TargetAssetClass
    RequestedQualificationCeiling
    RequestedUseProfile
    InitialCapitalExposureModel
    CostCeiling
    ResearchObjectives[]
    KnownConstraints[]
    ExplicitlyForbiddenAuthorities[]
    EvidenceRequirements[]
    CreatedAt
    ExpiryOrRevalidationRule
}
```

For the intended workflow:

```text
RequestedQualificationCeiling = READY_FOR_PAPER_REVIEW
```

unless the Owner explicitly chooses a narrower study ceiling.

The request SHALL NOT silently contain `PAPER_AUTHORIZED` or any higher execution authority.

---

# 9. Qualification State Model

The exact runtime state machine remains future implementation design, but the normative semantic distinctions shall include at least:

```text
REQUEST_RECEIVED
-> AUTHORITY_AND_SCOPE_RESOLVED
-> QUALIFICATION_PLANNED
-> MARKET_RESEARCH_IN_PROGRESS
-> DOMAIN_CANDIDATES_IN_PROGRESS
-> FSTSIMA_QUALIFICATION_IN_PROGRESS
-> FINDINGS_REMEDIATION_LOOP
-> EVIDENCE_RECONCILIATION
-> APPLICATION_EVALUATION
-> TERMINAL READINESS OUTCOME
```

Permitted terminal readiness outcomes shall include conceptually:

```text
READY_FOR_PAPER_REVIEW
HOLD_RETEST
INSUFFICIENT_EVIDENCE
BLOCKED_BY_FOUNDATION_CAPABILITY
BLOCKED_BY_PROVIDER_OR_DATA_GAP
BLOCKED_BY_BROKER_OR_EXECUTION_GAP
UNACCEPTABLE_RISK
NOT_READY
REJECT_MARKET_CANDIDATE
```

A terminal outcome SHALL state exact blockers/unknowns rather than fabricating readiness.

---

# 10. Trading LSA Responsibilities During Market Qualification

## 10.1 T-LSA-01 — Operations, Account & Environment

Evaluate as applicable:

- intended business environment;
- account/environment prerequisites;
- broker/account compatibility context;
- market session availability;
- operational prerequisites and environmental restrictions.

## 10.2 T-LSA-02 — Market & Instrument Universe

Own the new `MarketProfileCandidate` and market/instrument qualification truth.

It shall study as applicable:

- market identity and asset class;
- exchange/venue structure;
- sessions, calendars and auctions;
- instrument types and eligibility;
- tick, quantity, lot and minimum-order rules;
- price-limit / halt / circuit-breaker behavior;
- liquidity characteristics;
- volatility characteristics and regimes;
- settlement/account constraints;
- corporate-action or equivalent market rules where applicable;
- Data Product requirements;
- strategy-applicability inputs;
- execution constraints;
- market-specific Risk inputs without taking Unified Risk ownership;
- known prohibited/unsupported conditions;
- intended-use and validation requirements.

The Market Profile candidate is not admitted merely because research is complete.

## 10.3 T-LSA-03 — Analysis Frameworks

Determine which existing analysis frameworks are valid, invalid, insufficient or require bounded candidate adaptation for Market X.

Any new/adapted analysis method remains a candidate until separately validated for the exact intended use.

## 10.4 T-LSA-04 / T-LSA-05 — Trading Schools

Evaluate applicability of current trading schools and specialized opportunity-discovery methods to the new Market Profile.

A School may identify capability gaps or new method candidates but cannot create market admission, Risk or execution authority.

## 10.5 T-LSA-06 — Strategy Orchestration & Decision

Evaluate the central Strategy Catalog against the new Market Profile.

Required outputs shall distinguish at least:

```text
EXISTING_STRATEGY_VALIDATED_FOR_MARKET
EXISTING_STRATEGY_CONDITIONALLY_APPLICABLE
EXISTING_STRATEGY_REQUIRES_ADAPTATION
EXISTING_STRATEGY_NOT_APPLICABLE
NEW_STRATEGY_CANDIDATE_REQUIRED
```

Strategies SHALL NOT be duplicated merely because a new market is added.

```text
STRATEGY_A_US
STRATEGY_A_CRYPTO
STRATEGY_A_MARKET_X
```

shall not be created as separate copies when one centrally registered strategy plus explicit applicability/adaptation evidence is sufficient.

## 10.6 T-LSA-07 — Unified Risk Management

Own the `MarketRiskCandidate` and all Trading Risk business interpretation.

It shall evaluate as applicable:

- per-trade and per-instrument risk;
- per-market exposure;
- volatility/regime risk;
- liquidity risk;
- gap/jump risk;
- price-limit/halt risk;
- concentration and correlation effects;
- drawdown/session loss consequences;
- execution/slippage risk;
- market/account constraints;
- worst credible loss;
- interaction with existing portfolio risk;
- no-trade/restrict conditions.

The resulting Risk candidate may include market-specific parameters/envelopes inside valid higher-level authority, but SHALL NOT silently redefine global Risk authority or exceed Owner/governance ceilings.

```text
MARKET PROFILE PROVIDES RISK INPUTS
!=
MARKET PROFILE OWNS RISK DECISION
```

## 10.7 T-LSA-08 — Portfolio & Capital Management

Evaluate:

- currency/funding effects;
- capital reservation implications;
- portfolio concentration;
- cross-market allocation interactions;
- settlement/cash timing;
- current funded-exposure constraints;
- consequences for existing markets/positions.

## 10.8 T-LSA-09 — Execution & Position Lifecycle

Own execution-business qualification including as applicable:

- supported order semantics;
- session/order restrictions;
- acknowledgement/rejection behavior;
- partial fills;
- cancel/amend behavior;
- latency;
- spread/slippage;
- liquidity/market-impact assumptions;
- exchange/broker capability requirements;
- settlement/reconciliation implications;
- ambiguous outcome handling.

## 10.9 T-LSA-10 / T-LSA-11 — Learning, Knowledge, Analytics and Attribution

Preserve attributable lessons, market/strategy behavior knowledge, decision/process evidence, performance attribution and statistical/uncertainty limitations discovered during qualification.

Qualification success SHALL NOT rewrite unfavorable evidence.

## 10.10 T-LSA-12 — Strategy Evolution & Experimentation

Coordinate Trading-owned candidate experiments for new/adapted strategies/models and their interaction with FSTSimA.

It may iterate candidate versions but cannot self-promote them or convert experiment success into market authority.

## 10.11 T-LSA-13 — Trading Resource Management

Evaluate qualification workload/resource demand, pressure and resource tradeoffs inside Trading. It does not grant Foundation resources or override FSARM/Foundation resource governance.

---

# 11. FSAPMA Responsibilities During Market Qualification

FSAPMA remains the sole operational provider-management business owner.

For Market X it shall determine, as applicable:

- exact Data Product requirements received from the consuming market/analysis/strategy needs;
- currently known provider capability coverage;
- account/plan/entitlement requirements;
- quality/freshness/history/venue semantics;
- quota/capacity/cost/reliability;
- usage-right constraints;
- provider independence/upstream lineage where material;
- missing provider capability gaps;
- candidate new provider/product needs.

A discovered provider is not automatically certified or active.

```text
FOUND_PROVIDER != CERTIFIED_PROVIDER != ACTIVE_PROVIDER
```

The current `NEW-2/05` awareness-driven provider-gap hardening is a related unaccepted candidate and may be reconciled with this lifecycle in a future combined semantic freeze; it is not silently promoted to accepted design by this record.

---

# 12. Guardian Responsibilities During Market Qualification

Guardian remains independent protection/crisis owner.

For Market X it shall contribute/challenge as applicable:

- market-specific crisis conditions;
- exchange halt / venue outage / abnormal-market protection implications;
- stale/conflicted data danger;
- broker/execution ambiguity consequences;
- protection/restriction needs;
- recovery/reconciliation implications;
- no-trade or forced-restriction conditions within Guardian authority.

FSTSimA may simulate Guardian-relevant crisis scenarios, but simulation does not create real Guardian authority.

---

# 13. FSTSimA Is the Mandatory Non-Live Qualification Laboratory

For the intended new-market workflow, FSTSimA is the mandatory independent non-Live laboratory for material qualification evidence to the maximum extent supported by the exact intended use and later authorized capabilities.

Its eight branches are used according to their existing ownership:

```text
S-LSA-01  Simulation Time and Scenario
S-LSA-02  Market Environment Simulation
S-LSA-03  Provider and External Service Simulation
S-LSA-04  Broker, Exchange and Execution Simulation
S-LSA-05  Account, Capital and Settlement Simulation
S-LSA-06  Fault, Latency and Crisis Injection
S-LSA-07  Fidelity and Calibration
S-LSA-08  Oracle, Evidence, Reproducibility and Validation Assessment
```

The qualification campaign may include as applicable:

- historical scenario execution;
- replay;
- synthetic/counterfactual scenarios;
- market-regime variation;
- liquidity stress;
- volatility stress;
- gap/price-limit/halt scenarios;
- provider outage/staleness/duplication/quota exhaustion;
- broker rejection/latency/partial-fill/cancel races;
- execution/slippage/market-impact assumptions;
- account/capital/settlement effects;
- dependency/fault/latency/crisis injection;
- strategy interaction and conflict testing;
- Risk boundary/tail-condition testing;
- protection behavior challenge;
- fidelity/calibration;
- independent oracle/evidence/reproducibility assessment.

No single successful scenario establishes readiness.

---

# 14. Mandatory Candidate-Finding-Remediation Loop

Qualification SHALL support repeated cycles rather than forcing the first candidate through.

Conceptually:

```text
OWNING LSA / ELIGIBLE CSA PRODUCES CANDIDATE VERSION N
        |
        v
FSTSimA EXECUTES APPLICABLE TEST CAMPAIGN
        |
        +--> PASS EVIDENCE
        |
        +--> FAIL / WEAKNESS / UNCERTAINTY / CONTRADICTION
                    |
                    v
            RETURN FINDING TO TRUE OWNER
                    |
                    v
            OWNER REMEDIATES OR REJECTS
                    |
                    v
            CANDIDATE VERSION N+1
                    |
                    v
                 RETEST
```

A material candidate change invalidates stale validation evidence for the changed scope.

Unfavorable findings SHALL be preserved.

---

# 15. Research and External Evidence Boundary

Qualification may require external research, but research authority must remain distinct from direct Internet authority and operational provider data.

Mandatory distinctions:

```text
OWNS RESEARCH PROBLEM != HAS UNRESTRICTED INTERNET
RESEARCH INPUT != OPERATIONAL MARKET TRUTH
FSTSimA RESEARCH / SANDBOX != LIVE PROVIDER ROUTE
```

Trading-domain external research shall use the accepted bounded FSTSimA research/sandbox direction when the required Foundation capabilities are separately authorized and implemented.

FCR-0008 remains the future research-only Internet-egress dependency.

FCR-0011 remains the future FSTSimA non-Live isolation/egress dependency.

Operational provider data remains FSAPMA-owned and SHALL NOT be replaced by unverified Internet research.

If required research/egress/isolation capability is unavailable, the affected qualification state shall honestly become blocked/limited rather than using an Application-local bypass.

---

# 16. Evidence Sufficiency Has No Fixed Calendar Shortcut

The Owner may receive the result after the qualification has run for an appropriate period, but no fixed number of days/weeks alone proves readiness.

Evidence sufficiency shall depend on the exact intended use and may consider:

- market/regime diversity;
- strategy frequency;
- sample size;
- tail/failure coverage;
- statistical uncertainty;
- liquidity/volatility diversity;
- execution realism;
- novelty;
- interaction complexity;
- consequence severity;
- fidelity to trusted reference evidence;
- reproducibility;
- residual unknowns.

```text
TIME_ELAPSED != EVIDENCE_SUFFICIENT
```

Falcon may conclude `HOLD_RETEST` or `INSUFFICIENT_EVIDENCE` instead of waiting indefinitely or fabricating confidence.

---

# 17. Market Qualification Evidence Package

Before a `READY_FOR_PAPER_REVIEW` recommendation, the owning Trading Application shall assemble an attributable package conceptually containing as applicable:

```text
MarketQualificationEvidencePackage
{
    QualificationRequestIdentity
    OwnerCommandEvidence
    TargetMarketIdentity
    CandidateVersionSet
    IntendedUseClaim
    MarketProfileCandidateAndEvidence
    InstrumentEligibilityEvidence
    DataProductRequirements
    ProviderCapabilityAndRightsEvidence
    ProviderGapsAndCandidateSolutions
    AnalysisApplicabilityMatrix
    StrategyApplicabilityMatrix
    StrategyAdaptationCandidates
    NewStrategyCandidates
    UnifiedRiskCandidateAndEvidence
    PortfolioCapitalImpact
    ExecutionBrokerCapabilityEvidence
    GuardianProtectionChallengeEvidence
    FSTSimAScenarioCoverage
    FSTSimAFidelityCalibrationEvidence
    FSTSimAOracleReproducibilityAssessment
    FailedAndRejectedCandidateHistory
    KnownLimitations
    ResidualUnknowns
    ResidualRisks
    ExternalFoundationDependencies
    EvidenceFreshnessAndRevalidationTriggers
    ApplicationMSAEvaluation
    RecommendedNextState
}
```

Evidence identity/version/provenance must be sufficient to prevent a changed candidate from inheriting old PASS evidence.

---

# 18. Readiness Criteria for `READY_FOR_PAPER_REVIEW`

`READY_FOR_PAPER_REVIEW` may be recommended only when, for the exact requested intended use:

- the market identity and Market Profile candidate are sufficiently defined;
- material market rules/constraints are known or explicitly bounded;
- required Data Products and unresolved data gaps are explicit;
- provider capability/rights/cost/quality truth is sufficiently established for the proposed Paper phase or remaining limitations are explicitly compatible with that phase;
- strategy applicability is explicit;
- required adaptations/new candidates have applicable validation evidence;
- Unified Risk candidate is complete for the proposed scope;
- capital/portfolio interactions are understood sufficiently for Paper;
- broker/execution assumptions/capabilities required for Paper are defined;
- Guardian/protection implications are evaluated;
- required FSTSimA scenario/fidelity/reproducibility evidence is complete enough for the Intended Use Claim;
- no unresolved Critical/High blocker is hidden by aggregate scoring;
- material disagreement/contradiction is reconciled or causes a hold;
- evidence is fresh enough for the claim;
- Trading MSA has completed Application-level evaluation;
- all unavailable Foundation/runtime prerequisites are explicitly declared.

A high average score SHALL NOT hide a material blocker.

---

# 19. Owner-Facing Qualification Result

The Owner should receive a concise decision package rather than raw internal logs by default.

Conceptual successful report:

```text
MARKET QUALIFICATION COMPLETE

Market: X
Qualification Result: COMPLETE
Recommendation: READY_FOR_PAPER_REVIEW

Market Profile:
  CREATED / VALIDATED FOR REQUESTED SCOPE

Strategies:
  Existing validated: N
  Existing conditional/restricted: N
  Existing rejected: N
  Adapted candidates: N
  New candidates: N

Risk:
  Market-specific Risk profile: COMPLETE
  Key envelopes/limits: SUMMARY
  No-trade/restrict conditions: SUMMARY

Providers / Data:
  Compatible current candidates: N
  New provider/product candidates: N
  Remaining gaps: SUMMARY

Broker / Execution:
  Compatible/conditional/incompatible: SUMMARY

FSTSimA:
  Historical / Replay / Synthetic / Stress / Adversarial / Crisis / Fidelity / Reproducibility: SUMMARY

Residual Risks / Unknowns:
  SUMMARY

Exact Next Owner Decision:
  AUTHORIZE PAPER FOR MARKET X? YES / NO / REQUEST MORE QUALIFICATION
```

The report SHALL preserve material failures and residual uncertainty.

---

# 20. Paper Is a Separate Governed Stage

FSTSimA may qualify the market candidate for a Paper review decision, but Paper itself may involve an external Paper broker/API and is not automatically part of FSTSimA's internal simulation authority.

Mandatory progression:

```text
MARKET QUALIFICATION
-> READY_FOR_PAPER_REVIEW
-> EXPLICIT SEPARATE OWNER / GOVERNANCE PAPER AUTHORITY
-> PAPER
-> PAPER EVIDENCE / DIVERGENCE REVIEW
-> READY_FOR_TINY_LIVE_REVIEW, IF JUSTIFIED
-> EXPLICIT SEPARATE OWNER / GOVERNANCE TINY-LIVE AUTHORITY
-> TINY LIVE
-> READY_FOR_LIVE_REVIEW, IF JUSTIFIED
-> EXPLICIT SEPARATE OWNER / GOVERNANCE LIVE AUTHORITY
-> LIVE WITHIN EXACT AUTHORIZED SCOPE
```

Mandatory invariants:

```text
READY_FOR_PAPER_REVIEW != PAPER_AUTHORIZED
PAPER_PASS != TINY_LIVE_AUTHORIZED
TINY_LIVE_PASS != LIVE_AUTHORIZED
```

No earlier Owner instruction shall be interpreted as implicit authority for a later higher-risk phase.

---

# 21. Requalification and Evidence Staleness

Market qualification is not permanent immunity from change.

A material change may require partial or full requalification, including as applicable:

- exchange/market-rule change;
- session/calendar change;
- instrument/tick/lot/price-limit change;
- provider product/rights/plan/quality change;
- broker API/order/execution behavior change;
- strategy/model/code/config change;
- Risk policy/envelope change;
- capital/exposure model change;
- significant market-regime change;
- discovered simulation/fidelity defect;
- evidence provenance/integrity failure.

```text
STALE QUALIFICATION EVIDENCE != CURRENT READINESS
```

---

# 22. Fail-Closed Conditions

The affected qualification shall stop, hold, restrict or reject rather than fabricate readiness when material truth cannot be established.

Examples:

```text
MARKET RULE UNKNOWN AND MATERIAL
-> HOLD / RESEARCH / RESTRICT

REQUIRED DATA PRODUCT UNAVAILABLE
-> BLOCKED / EXPLICIT DEGRADATION ONLY IF INTENDED USE ALLOWS

PROVIDER RIGHTS UNKNOWN
-> NOT ELIGIBLE FOR RELIED-UPON USE

BROKER SAFETY-CRITICAL TRUTH PATH UNKNOWN
-> AFFECTED PAPER/EXECUTION CAPABILITY INELIGIBLE

RISK CANDIDATE FAILS TAIL SCENARIOS
-> REMEDIATE / RETEST / NOT READY

FSTSIMA FIDELITY INADEQUATE FOR CLAIM
-> NO PROMOTION-GRADE READINESS CLAIM

MATERIAL CANDIDATE CHANGED AFTER PASS
-> STALE PASS / RETEST CHANGED SCOPE

FOUNDATION RUNTIME DEPENDENCY UNAVAILABLE
-> DESIGN MAY CONTINUE; RUNTIME CLAIM BLOCKED
```

---

# 23. Foundation / FCR Dependencies

This candidate does not create Foundation runtime capability.

Relevant dependencies include as applicable:

- `FCR-0008` — Awareness research-only Internet egress;
- `FCR-0011` — FSTSimA non-Live isolation/egress;
- `FCR-0006` — governed event/evidence/replay delivery across Applications;
- `FCR-0005` — governed operational producer-to-consumer delivery boundary for FSAPMA data;
- `FCR-0012` — FSA/Owner governance and bounded evolution control plane;
- `FCR-0030` — exact MSA-to-FSA governed interface;
- `FCR-0014` — broker egress/credential boundary before later authorized broker-connected stages;
- `FCR-0010` / `FCR-0031` — future Application resource-pressure/resource-management implementation verification as applicable;
- `FCR-0016` — canonical Foundation artifact consumption boundary as applicable to implementation.

Application-held implementation-verification FCRs remain open until the related implementation actually exists and is verified.

No missing Foundation capability may be replaced by a local Application shortcut.

---

# 24. Shared Web / Owner Command Boundary

Shared Web may eventually provide a conversational/guided Owner interface for a command such as `Add market X`, but Web does not own the Trading meaning of the command.

The exact command must preserve:

```text
OWNER WORDING
-> ATTRIBUTABLE OWNER REQUEST
-> APPLICATION-OWNED INTERPRETATION
-> EXACT QUALIFICATION AUTHORITY SCOPE
-> CORRELATED RESULT / EVIDENCE
```

Web-owned AI normalization must not silently add Paper/Tiny Live/Live authority or alter target-market/domain semantics.

Current FCR-0077 tracks this cross-workstream planning relationship.

---

# 25. Required Future Implementation Materialization

Before implementation of this lifecycle, Part 1 or a later governed scope must materialize, as applicable:

- exact `MarketQualificationRequest` contract;
- exact request identity/authority binding;
- Trading-MSA orchestration state;
- per-owner candidate/evidence interfaces;
- governed Trading <-> FSAPMA <-> FSTSimA <-> Guardian contract families;
- exact qualification state machine;
- candidate/version/digest bindings;
- FSTSimA campaign and evidence contracts;
- finding/remediation/retest semantics;
- readiness-state contract;
- Owner-facing result contract;
- evidence freshness/requalification triggers;
- Application/Owner decision bindings;
- positive/negative/adversarial fixtures;
- fail-closed behavior for every unavailable Foundation dependency.

This document fixes design semantics, not implementation topology or runtime APIs.

---

# 26. Required Fresh Red-Team Coverage

The eventual reviewed version of this lifecycle shall be challenged against at least:

1. `Add market X` silently starts Paper.
2. `Add market X` silently grants Live/provider/broker connectivity.
3. FSTSimA becomes owner of Market Profile.
4. FSTSimA rewrites Unified Risk directly.
5. FSTSimA promotes a strategy itself.
6. Trading MSA modifies FSAPMA internals.
7. Web AI expands Owner intent.
8. Market Profile duplicates central strategies.
9. Existing strategy is declared compatible without market-specific evidence.
10. New strategy candidate bypasses FSTSimA.
11. Risk candidate passes average score while tail-risk blocker exists.
12. Market-specific Risk candidate silently changes global Risk ceiling.
13. Research content becomes operational market truth.
14. Unrestricted Internet is inferred from research responsibility.
15. Provider discovered in research becomes active automatically.
16. Provider rights are unknown but qualification passes.
17. Free/trial provider is represented as durable free capacity.
18. Broker capability marketing language is treated as certified execution truth.
19. Simulated fill is represented as Paper/Live fill.
20. Material candidate changes after validation but keeps old PASS.
21. Failed scenarios are discarded from the final package.
22. Time elapsed is treated as proof of evidence sufficiency.
23. Low-frequency strategy is declared validated without sufficient samples/regimes.
24. FSTSimA fidelity is poor but readiness is still issued.
25. Guardian crisis failure is averaged away by strategy performance.
26. Missing Foundation route/egress capability is replaced locally.
27. Cross-Application direct internal access is used for convenience.
28. New market is admitted because qualification finished.
29. `READY_FOR_PAPER_REVIEW` is displayed as `PAPER READY/AUTHORIZED`.
30. Paper PASS automatically starts Tiny Live.
31. Tiny Live PASS automatically starts Live.
32. Stale market/provider/broker evidence remains valid indefinitely.
33. Market rule changes but no requalification occurs.
34. Unknown material condition is interpreted optimistically.
35. Strategy adaptation creates a hidden per-market duplicate registry.
36. Market expansion changes leverage/capital model without explicit scope review.
37. Qualification uses unsupported broker truth and blind retry assumptions.
38. Same intelligence builds and solely validates a high-consequence candidate.
39. FSA review is treated as production adoption.
40. Owner silence/timer is used to approve DCC-3 market expansion.

---

# 27. Canonical Invariants

```text
ADD_MARKET_X = AUTHORIZE_BOUNDED_NON_LIVE_QUALIFICATION
ADD_MARKET_X != MARKET_ADMISSION
ADD_MARKET_X != PAPER_AUTHORITY
ADD_MARKET_X != TINY_LIVE_AUTHORITY
ADD_MARKET_X != LIVE_AUTHORITY

NEW_MARKET_QUALIFICATION = DCC_3_MINIMUM
OWNER_QUALIFICATION_AUTHORITY != OWNER_MARKET_ADOPTION

EACH_INTELLIGENCE_OWNS_ITS_OWN_RESPONSIBILITY
FSTSIMA = NON_LIVE_QUALIFICATION_LAB
FSTSIMA_VALIDATION != TARGET_BUSINESS_OWNERSHIP
FSTSIMA_PASS != PROMOTION_AUTHORITY

MARKET_PROFILE != STRATEGY_CATALOG
MARKET_PROFILE_RISK_INPUT != UNIFIED_RISK_OWNERSHIP
STRATEGIES_REMAIN_CENTRAL

CANDIDATE -> FSTSIMA -> FINDING -> TRUE_OWNER_REMEDIATION -> RETEST
MATERIAL_CANDIDATE_CHANGE -> CHANGED_SCOPE_REVALIDATION

RESEARCH_RESPONSIBILITY != UNRESTRICTED_INTERNET
RESEARCH_INPUT != OPERATIONAL_MARKET_TRUTH
DISCOVERED_PROVIDER != CERTIFIED_PROVIDER != ACTIVE_PROVIDER

TIME_ELAPSED != EVIDENCE_SUFFICIENT
READINESS_SCORE_CANNOT_HIDE_MATERIAL_BLOCKER

READY_FOR_PAPER_REVIEW != PAPER_AUTHORIZED
PAPER_PASS != TINY_LIVE_AUTHORIZED
TINY_LIVE_PASS != LIVE_AUTHORIZED

FSA_REVIEW != PRODUCTION_ADOPTION
OWNER_SILENCE != AUTHORITY
TIMER_EXPIRY != DCC_3_MARKET_ADOPTION
```

---

# 28. Explicit Non-Grant

This candidate does not grant:

- Part 0 reopen or modification authority;
- Part 1 acceptance or closure;
- implementation authority;
- runtime route activation;
- research Internet egress;
- provider/broker connectivity;
- credentials;
- market admission;
- Paper;
- Tiny Live;
- Live;
- deployment;
- capital deployment;
- leverage expansion;
- autonomous market adoption;
- autonomous promotion;
- Foundation implementation authority;
- Shared Web write authority over FSATS.

---

# 29. Current Documentary State

```text
MARKET_QUALIFICATION_LIFECYCLE = MATERIALIZED_DESIGN_CANDIDATE
OWNER_DIRECTION = RECORDED
ARCHITECTURE_CONSISTENCY_REVIEW = REQUIRED_FRESH
RED_TEAM_REVIEW = REQUIRED_FRESH
OWNER_FINAL_ACCEPTANCE = NOT_YET
IMPLEMENTATION_AUTHORITY = NOT_GRANTED
RUNTIME_AUTHORITY = NOT_GRANTED
```

The exact candidate version must complete the fresh Architecture/Consistency and fresh Red-Team cycle before it is presented for final Owner acceptance.
