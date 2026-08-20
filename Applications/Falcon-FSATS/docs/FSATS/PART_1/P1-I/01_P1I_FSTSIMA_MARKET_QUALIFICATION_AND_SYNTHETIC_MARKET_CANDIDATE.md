# P1-I — FSTSimA Market Qualification and Synthetic Market Candidate

**Status:** `DESIGN_CANDIDATE / OWNER-DIRECTED CLARIFICATION / NOT_OWNER_ACCEPTED / NOT_CLOSED`  
**Branch:** `application-development`  
**Scope:** `P1-I — FSTSimA 8-LSA Implementation Decomposition`  
**Authority Type:** `DESIGN ONLY`  
**Implementation Authority:** `NOT GRANTED`  
**Runtime Authority:** `NOT GRANTED`  
**Paper Authority:** `NOT GRANTED BY THIS ARTIFACT`  
**Tiny-Live Authority:** `NOT GRANTED BY THIS ARTIFACT`  
**Live Authority:** `NOT GRANTED`  

## 1. Purpose

This candidate records the Project Owner-directed clarification that FSTSimA is not limited to replaying known markets or validating already-defined Trading behavior.

FSTSimA shall also serve as the primary FSATS laboratory for:

1. required synthetic-market generation for rare, extreme, adversarial and insufficiently observed market conditions; and
2. qualification of a newly Owner-requested market before that market may be considered for real-world Paper/Shadow evaluation and any later Tiny-Live review.

This clarification is prospective Part 1 design. It does not rewrite accepted Part 0 history and does not itself add a new operational market, Provider, Broker, credential, route, Paper environment, Tiny-Live environment or Live authority.

## 2. Required Synthetic Market Capability

Synthetic Market generation is a required FSTSimA capability, not merely an optional future enhancement.

The capability shall generate controlled market environments suitable for testing conditions that are rare, poorly represented in historical data, structurally novel, or intentionally adversarial.

Synthetic scenarios may include, where relevant to the market being studied:

- flash crashes and abrupt gaps;
- sudden or progressive liquidity disappearance;
- spread expansion;
- abnormal volatility transitions;
- rapid market-regime changes;
- extreme correlation and de-correlation;
- order-book instability;
- session-transition stress;
- execution stress under thin liquidity;
- market microstructure edge cases;
- combined black-swan scenarios;
- conditions not yet observed historically but plausible under the governing market model.

Synthetic Market output shall not be random-price generation without governed market semantics. The generated environment shall preserve the market-specific rules needed for meaningful testing, including applicable liquidity, volatility, timing, session, price-formation, order/execution and market-structure behavior.

Every synthetic result shall retain explicit provenance and classification as synthetic evidence. Synthetic evidence shall never be laundered into historical, Paper, Shadow or Live-authoritative evidence.

An intelligent Synthetic Market or Adversarial Scenario Generator may be declared `CSA_ELIGIBLE` if component-level decomposition proves meaningful specialized self-awareness value. CSA eligibility shall not create architecture, authority, deployment or promotion authority.

## 3. Owner-Initiated New Market Qualification

A new market may enter the FSATS qualification lifecycle only after an explicit Project Owner initiation or other separately governed authority that is valid at that time.

Example Owner intent:

`Add Forex as a candidate market for qualification.`

Such an instruction means only that Falcon may begin studying and qualifying Forex as a candidate market.

It does not mean:

- Forex is an admitted operational market;
- Forex Paper Trading is authorized;
- a Forex Broker or Provider is approved;
- Tiny-Live or Live trading is authorized;
- leverage, derivatives or a new risk model are authorized by implication.

The system shall create an attributable Market Qualification Case with its own identity, requested market, Owner initiation evidence, qualification state, known assumptions, discovered gaps, evidence references and final readiness recommendation.

## 4. New Market Qualification Lifecycle

The candidate lifecycle shall include at least the following governed phases.

### 4.1 Market Discovery and Modeling

Falcon shall study the requested market deeply before real-world trading qualification.

The study shall determine, as applicable:

- market structure;
- sessions and trading hours;
- instruments and instrument semantics;
- price, quantity, lot and precision rules;
- liquidity and spread behavior;
- volatility and regime behavior;
- execution characteristics;
- settlement and fee behavior;
- broker constraints;
- relevant leverage mechanics even when Falcon intends to remain 1:1 funded;
- market-specific risk characteristics;
- required operational data;
- required historical data;
- market-specific restrictions and known limitations.

The output is a Market Model Candidate. It is not operational-market admission.

### 4.2 Existing Provider and Broker Inventory Check

Before requesting any new external dependency, Falcon shall determine whether currently available Providers and Brokers are sufficient for qualification and later Paper/Shadow study.

The inventory check shall evaluate whether the existing environment provides adequate:

- market coverage;
- historical data;
- real-time or appropriately delayed data;
- data depth and granularity;
- API/streaming capability;
- entitlement;
- quality and reliability;
- Paper Trading capability;
- broker execution API capability;
- account and jurisdiction eligibility;
- 1:1 funded-operation compatibility;
- later Tiny-Live suitability where relevant.

If the existing capabilities are sufficient, qualification may continue using the governed existing Provider/Broker set.

If they are insufficient, Falcon shall not invent an external dependency or silently choose one. It shall enter the Owner Decision Package process defined below.

### 4.3 Provider Research and Recommendation

FSAPMA owns Provider-side research and comparison because operational Provider/data business truth remains FSAPMA-owned.

FSAPMA shall evaluate candidate Providers using relevant criteria including:

- market and instrument coverage;
- real-time versus delayed availability;
- historical depth;
- tick, bar, quote and order-book capability where relevant;
- data quality and provenance;
- reliability and outage behavior;
- latency;
- API and streaming capability;
- rate limits and quotas;
- account entitlement;
- pricing and free-tier availability;
- licensing restrictions;
- account or geographic restrictions;
- documentation quality;
- redundancy and reconciliation value;
- suitability as primary, secondary or verification Provider.

Provider research shall normally identify a ranked set rather than one unexplained choice, including where applicable:

- recommended primary Provider;
- recommended secondary or verification Provider;
- lower-cost alternative;
- higher-quality alternative;
- material trade-offs.

### 4.4 Broker Research and Recommendation

Trading-side Account/Environment and Execution ownership shall evaluate Broker candidates. FSAPMA shall not become the owner of Broker execution semantics.

Broker comparison shall evaluate relevant criteria including:

- market support;
- Paper Trading availability and quality;
- API quality;
- supported order types;
- execution behavior;
- commissions and fees;
- spreads where applicable;
- minimum order or lot rules;
- account eligibility and geographic restrictions;
- authentication and sandbox behavior;
- reliability;
- rate limits;
- session support;
- settlement behavior;
- compatibility with 1:1 funded operation;
- suitability for later Shadow and Tiny-Live qualification;
- expected integration complexity.

### 4.5 Owner External Capability Decision Package

When one or more required Providers or Brokers are missing, Falcon shall produce a complete Market External Capability Decision Report for the Project Owner.

The report shall identify at minimum:

- requested market;
- missing external capabilities;
- why the existing Provider/Broker set is insufficient;
- ranked Provider candidates;
- ranked Broker candidates;
- recommended configuration;
- recommended primary and backup choices where applicable;
- current known pricing/cost model;
- free versus paid classification;
- Paper availability;
- API capability;
- data quality and historical coverage;
- real-time capability;
- material restrictions;
- reliability considerations;
- expected integration effort;
- redundancy/reconciliation implications;
- trade-offs of the recommended versus lower-cost or alternate choices;
- whether one external party can serve multiple roles and whether role separation is preferable;
- known risks, gaps and unresolved questions;
- the exact Owner decision required to proceed.

The system shall provide a recommendation, not merely an unranked catalog.

The Owner may approve the recommendation, select an alternative, request additional research, reject the proposed external capability, or defer the candidate market.

Falcon shall not, on its own authority:

- create or fund an external account;
- purchase a subscription;
- accept commercial terms;
- enter payment details;
- create or activate credentials;
- connect a new Provider or Broker operationally;
- grant itself external egress or trading authority.

### 4.6 Data and Provider Qualification

After the required Provider capability is available under proper authority, FSAPMA shall qualify operational and historical data for the candidate market.

FSTSimA may consume governed qualification evidence and appropriate non-Live datasets but shall not replace FSAPMA Provider/data truth.

### 4.7 Synthetic and Historical Market Construction

FSTSimA shall build a qualification environment combining, as applicable:

- historical replay;
- synthetic market generation;
- stress scenarios;
- provider simulation;
- broker/exchange/execution simulation;
- account/capital/settlement simulation;
- fault, latency and crisis injection.

Synthetic and historical evidence shall remain separately attributable.

### 4.8 Strategy Compatibility and Capability-Gap Study

Trading shall determine which centrally governed strategies are suitable for the candidate market and how the Market Model constrains or configures them.

Strategies shall not be duplicated into a market-specific registry merely because a new market is being studied.

The study shall determine:

- eligible existing strategies;
- unsuitable strategies;
- required configuration differences;
- missing analysis capability;
- missing strategy capability;
- risk-model gaps;
- execution-model gaps.

Any required new or modified strategy shall follow the governed Strategy Evolution and Experimentation path. Market qualification shall not create an authority shortcut for production strategy changes.

### 4.9 Deep Simulation Qualification

The candidate market shall undergo evidence-driven simulation for as long as necessary to reach a defensible readiness state. The design shall not impose an arbitrary fixed number of days.

Qualification may last days, weeks or longer depending on evidence quality, market complexity, available history, discovered weaknesses and required remediation.

Testing shall cover, as applicable:

- historical regimes;
- synthetic rare events;
- strategy combinations;
- Trading Risk behavior;
- portfolio/capital behavior;
- execution quality;
- provider failures;
- broker failures;
- Guardian crisis behavior;
- resource pressure and FSARM interaction;
- recovery;
- reproducibility;
- repeated and adversarial experiments.

### 4.10 Market Qualification Assessment

FSTSimA fidelity/calibration and independent validation assessment shall produce an explicit qualification state supported by evidence.

Candidate states may include:

- `NOT_READY`;
- `MORE_STUDY_REQUIRED`;
- `BLOCKED_BY_DATA`;
- `BLOCKED_BY_PROVIDER`;
- `BLOCKED_BY_BROKER_MODEL`;
- `BLOCKED_BY_STRATEGY_GAP`;
- `BLOCKED_BY_RISK_GAP`;
- `READY_FOR_PAPER_QUALIFICATION_REVIEW`.

Readiness is a recommendation, not authority.

### 4.11 Owner Gate for Real-World Paper/Shadow Qualification

When the candidate market reaches `READY_FOR_PAPER_QUALIFICATION_REVIEW`, Falcon shall provide the Project Owner with a complete readiness report and the exact decision required.

No Paper or Shadow operation shall begin from the readiness state alone.

The transition requires the governing authority applicable at that time, including Project Owner approval where required.

### 4.12 Real-World Paper and Shadow Qualification

Once separately authorized, the candidate market enters real-world qualification using Paper and Shadow capabilities as applicable.

The qualification shall compare at minimum, where evidence exists:

- simulation versus Paper fills;
- simulated versus observed latency;
- modeled versus observed spreads and liquidity;
- simulated Provider behavior versus operational data behavior;
- strategy behavior under real-time conditions;
- Trading Risk behavior;
- execution sequencing;
- slippage assumptions;
- market-session effects;
- missed-opportunity and false-opportunity behavior.

Shadow mode shall permit Falcon to observe real operational market data and produce hypothetical decisions/orders without creating Live execution authority.

### 4.13 Paper/Shadow Divergence and Calibration Loop

FSTSimA shall use Paper/Shadow evidence to measure and correct divergence between simulation and real-world behavior.

Qualification is not one-way. A candidate market may return repeatedly from Paper/Shadow study to simulation, calibration, strategy testing, risk testing or execution-model remediation.

Examples of divergence that may require remediation include:

- underestimated slippage;
- inaccurate spread models;
- incorrect latency models;
- poor Provider assumptions;
- overconfident strategy behavior;
- inaccurate execution or liquidity modeling.

### 4.14 Tiny-Live Readiness Review

Only after sufficient Paper/Shadow and simulation evidence may Falcon recommend:

`READY_FOR_TINY_LIVE_REVIEW`.

The supporting report shall include, as applicable:

- study duration;
- scenario and regime coverage;
- number and diversity of Paper observations/trades;
- Shadow evidence;
- simulation/Paper divergence;
- worst observed drawdown;
- risk behavior;
- execution quality;
- Provider reliability;
- known limitations;
- unresolved risks;
- Guardian readiness;
- recovery readiness;
- confidence and evidence-quality assessment.

`READY_FOR_TINY_LIVE_REVIEW` is not Tiny-Live authority.

Tiny-Live may begin only under a separate valid Owner/governance authorization applicable at that time.

## 5. Initial Market Qualification Baseline

The initial intended FSATS market scope remains:

- US Equities;
- Crypto Spot;
- 1:1 funded exposure;
- no leverage by implication.

The intended first real-world qualification mode for these initial markets is Paper Trading, subject to the separately required implementation/runtime/Paper authority.

FSTSimA shall operate in parallel as the simulation, synthetic/stress, fidelity and calibration laboratory so Falcon can begin comparing simulation behavior with real-world Paper/Shadow evidence as soon as those environments are separately authorized.

This artifact does not itself activate Paper Trading for US Equities or Crypto Spot.

## 6. Cross-Application Ownership During Market Qualification

Market qualification is a multi-Application FSATS process. FSTSimA hosts and coordinates the qualification laboratory but does not absorb the business ownership of the other Applications.

Ownership remains separated as follows:

- **FSTSimA:** simulation, synthetic markets, replay, adversarial scenarios, fidelity, calibration, validation evidence and qualification-laboratory state;
- **FSAPMA:** Provider discovery/evaluation, operational Provider/data truth, entitlement, routing, quality, reconciliation, quota/cost/reliability evidence;
- **Trading:** Market Model business meaning, strategy compatibility, Trading Risk, portfolio/capital semantics, broker/execution evaluation and Trading-side Paper/Shadow behavior;
- **Guardian:** protection/crisis qualification and protection evidence;
- **FSARM:** FSATS resource coordination only, without taking Provider, Trading, Guardian or simulation business authority;
- **Foundation:** Foundation-owned lifecycle, security, admission, total-resource truth, final Foundation resource authority and other Foundation responsibilities.

No Application may bypass governed cross-Application contracts or take another Application's internal ownership merely because all participate in the same Market Qualification Case.

## 7. Authority and Promotion Separation

The following distinctions are mandatory:

- discovery is not onboarding;
- recommendation is not approval;
- Provider/Broker selection recommendation is not commercial commitment;
- simulation PASS is not Paper authority;
- Paper readiness is not Paper activation;
- Paper/Shadow success is not Tiny-Live authority;
- Tiny-Live readiness is not Tiny-Live activation;
- repeated success does not manufacture permanent authority;
- self-awareness does not create authority.

Every promotion to a higher-consequence environment shall identify the separate valid authority required at that stage.

## 8. Required Evidence and Traceability

Every Market Qualification Case shall preserve enough evidence to reconstruct:

- who requested the candidate market;
- what scope was requested;
- which Market Model version was studied;
- which Providers and Brokers were considered;
- what options were recommended to the Owner and why;
- what Owner decisions were made;
- which external capabilities were actually authorized/onboarded;
- which datasets and scenario versions were used;
- which synthetic models and seeds were used;
- which Trading/FSAPMA/Guardian/FSTSimA/FSARM versions participated;
- simulation results;
- Paper/Shadow results when authorized;
- calibration changes;
- known limitations and unresolved risks;
- the evidence supporting any readiness recommendation.

Historical qualification evidence shall be preserved. Later corrections shall not rewrite earlier results.

## 9. Fail-Closed Requirements

The qualification process shall fail closed where required authority-bearing facts cannot be established.

Examples include:

- requested market has no valid Owner initiation;
- required Provider/Broker capability is unavailable and no Owner decision exists;
- Provider/Broker authority or credential boundary is unresolved;
- operational data cannot be distinguished from replay/synthetic data;
- simulation provenance is incomplete;
- validation evidence is contaminated or non-reproducible where reproducibility is required;
- Paper or Tiny-Live authority is absent;
- a required Foundation capability remains unavailable.

A blocked state shall produce evidence and the exact next decision or capability needed. It shall not be silently bypassed.

## 10. P1-I Materialization Requirement

The final P1-I decomposition shall incorporate this clarification into the exact components, states, interfaces, contracts and verifiers for the eight accepted FSTSimA LSAs without adding a ninth LSA by implication.

Expected mapping includes at minimum:

- S-LSA-01: Market Qualification Case scenario/time orchestration;
- S-LSA-02: required Synthetic Market and market-environment modeling;
- S-LSA-03: Provider/external-service simulation;
- S-LSA-04: Broker/exchange/execution simulation;
- S-LSA-05: account/capital/settlement simulation;
- S-LSA-06: rare-event, fault, latency and crisis injection;
- S-LSA-07: simulation-versus-real-world fidelity measurement and calibration;
- S-LSA-08: independent evidence, reproducibility, readiness-state and validation assessment.

Provider discovery remains FSAPMA-owned and Broker evaluation remains Trading-owned even when FSTSimA consumes their evidence inside the Market Qualification Case.

No separate ninth FSTSimA LSA is created by this clarification.

## 11. Review State

This is a semantic Part 1 candidate addition requested by the Project Owner during active Part 1 design review.

It is intentionally not treated as final Owner acceptance or closure at this stage.

Per the current Owner-directed review approach, individual Part 1 sections remain candidates while the Owner reviews and adjusts the full Part 1 set. Final per-section and integrated review will occur after the full Part 1 design has been assembled and stabilized.
