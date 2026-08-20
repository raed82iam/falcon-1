# P0-H — Self-Aware Trading Core, 13-LSA Model and TARC

**Status:** `PROPOSED / OWNER_REVIEW_CANDIDATE / NOT_OWNER_ACCEPTED / NOT_CLOSED`  
**Scope:** `P0-H only`  
**Implementation Authority:** `NOT GRANTED`

---

## 1. Purpose

P0-H defines the canonical internal architecture of the Falcon Self-Aware Trading Application, including the current 13-LSA topology, initial market/exposure scope, market/universe logic, analysis, trading schools, strategy orchestration, Unified Risk, portfolio/capital, execution/reconciliation, learning/analytics/evolution, and Trading resource management through T-LSA-13/TARC.

The goal is to eliminate the need to reconstruct current Trading truth from an older 12-LSA map plus later amendments while preserving the current Owner-directed initial Trading scope explicitly.

---

## 2. Canonical Application Boundary

The Falcon Self-Aware Trading Application is one independent Falcon Application with exactly one MSA.

It owns Trading business logic only.

It does not own:

- Foundation lifecycle/admission;
- Foundation total-resource truth;
- Foundation communication/security infrastructure;
- FSAPMA operational external-data acquisition;
- Guardian protection/crisis authority;
- FSTSimA validation Application authority;
- external broker egress/security authority;
- Owner governance authority.

---

## 3. Initial Trading Scope

The initial P0-NG Trading scope preserves the current Owner-directed design intent:

```text
INITIAL_MARKETS
= US_EQUITIES
+ CRYPTO_SPOT

INITIAL_EXPOSURE_MODEL
= 1_TO_1_FUNDED_EXPOSURE
```

For avoidance of doubt, the initial scope does **not** imply authority for:

- margin leverage above funded capital;
- derivatives;
- options;
- futures;
- leveraged tokens or equivalent leveraged instruments;
- short exposure that creates borrowing/leverage obligations;
- cross-user pooled capital;
- any market or asset class not separately admitted through governed design/validation/authority.

```text
INITIAL_1_TO_1_FUNDED_SCOPE != PERMISSION_FOR_LEVERAGE
US_EQUITIES_AND_CRYPTO_SPOT != ALL_MARKETS
```

A later market, asset class, leverage model, derivative capability, or materially different capital model is a separately governed scope expansion requiring the applicable Market Profile, Risk, execution, validation, Foundation dependency, Owner/governance, and lifecycle review.

This P0-H scope definition is design semantics only. It does not grant Paper, Tiny Live, Live, broker connectivity, capital deployment, or execution authority.

---

## 4. Canonical 13-LSA Trading Topology

```text
Trading MSA
|
+-- T-LSA-01 Operations, Account & Environment
+-- T-LSA-02 Market & Instrument Universe
+-- T-LSA-03 Analysis Frameworks
+-- T-LSA-04 Classical Trading School
+-- T-LSA-05 Opportunity Hunting School
+-- T-LSA-06 Strategy Orchestration & Decision
+-- T-LSA-07 Unified Risk Management
+-- T-LSA-08 Portfolio & Capital Management
+-- T-LSA-09 Execution & Position Lifecycle
+-- T-LSA-10 Trading Learning & Knowledge
+-- T-LSA-11 Trading Analytics & Attribution
+-- T-LSA-12 Strategy Evolution & Experimentation
+-- T-LSA-13 Trading Resource Management
```

This 13-room map is current for the P0-NG candidate.

The historical 12-room topology is provenance only.

---

# 5. T-LSA-01 — Operations, Account & Environment

Owns Trading-side awareness/evaluation of:

- user/account/environment context;
- business operating mode;
- account readiness;
- environment classification;
- session/business availability;
- broker/account bindings as business context;
- bounded resource readiness consumed from T-LSA-13/TARC;
- operational prerequisites required by Trading decisions.

Does not own:

- Foundation lifecycle;
- resource governance;
- TARC;
- broker execution;
- Risk;
- Guardian crisis authority.

```text
T_LSA01_RESOURCE_READINESS_CONSUMER != RESOURCE_OWNER
```

---

# 6. T-LSA-02 — Market & Instrument Universe

Owns Trading-side market/instrument eligibility, Market Profiles, dynamic Candidate Universe, managed-instrument awareness, instrument classification, and market-specific constraints.

It SHALL distinguish:

```text
CANDIDATE_UNIVERSE != MANAGED_POSITION_SET
```

An instrument leaving the candidate universe does not erase an existing position or obligation.

## 6.1 Market Profiles

Each admitted market SHALL have a governed Market Profile defining as applicable:

- market identity;
- asset class;
- trading/session/calendar structure;
- instrument-eligibility rules;
- liquidity characteristics and constraints;
- volatility characteristics;
- price/quantity/tick/minimum-order semantics;
- operational Data Product requirements;
- analysis requirements;
- strategy-applicability inputs;
- execution constraints;
- settlement/account constraints where material;
- market-specific Risk inputs/constraints without taking Unified Risk ownership;
- broker capability dependencies;
- known prohibited/unsupported conditions;
- validation/intended-use requirements.

Initial Market Profiles SHALL cover:

- US Equities;
- Crypto Spot.

A Market Profile supplies facts/constraints. It does not duplicate strategies and does not own Unified Risk.

## 6.2 Dynamic Universe

The Candidate Universe may change as instrument eligibility, liquidity, broker availability, market conditions, data quality, capital context, and governed ranking criteria change.

A dynamic universe change SHALL NOT:

- erase existing exposure;
- override Guardian/Risk restrictions;
- create broker capability;
- create a strategy;
- create capital authority;
- convert a non-admitted market into an admitted market.

---

# 7. T-LSA-03 — Analysis Frameworks

Owns reusable Trading analysis frameworks and attributable analysis evidence generation.

Analysis families may include technical, statistical, quantitative, structural, regime, flow, anomaly, pattern, behavioral, or other separately accepted methods.

Analysis SHALL expose as applicable:

- method/version;
- required inputs;
- applicable context;
- evidence/confidence;
- assumptions;
- uncertainty;
- known failure modes;
- freshness/validity;
- provenance.

It does not authorize trades.

```text
ANALYSIS_SIGNAL != TRADING_DECISION
ANALYSIS_CONFIDENCE != RISK_AUTHORITY
```

Eligible intelligent analysis components may own CSA under P0-C.

---

# 8. T-LSA-04 — Classical Trading School

Owns awareness/evaluation of Classical Trading School methods, applicability, evidence, limitations, interaction effects, and improvement opportunities.

Strategies remain centrally registered rather than duplicated per market.

The School may produce evidence/recommendations to Strategy Orchestration, but does not own final Trading decision, capital, Unified Risk, execution, Guardian protection, or market admission.

---

# 9. T-LSA-05 — Opportunity Hunting School

Owns specialized opportunity-discovery methods, including bounded whale/activity hunting, anomaly/opportunity discovery, and other separately accepted specialized search methods.

It may rank, score, or recommend opportunities only inside admitted market/instrument and data contexts.

```text
OPPORTUNITY_FOUND != TRADE_AUTHORIZED
```

Opportunity Hunting does not own final Trading decision, Unified Risk, capital, or execution.

---

# 10. T-LSA-06 — Strategy Orchestration & Decision

Owns the central Strategy Catalog/Controller interaction and construction of Trading decision candidates.

## 10.1 Central Strategy Catalog

Strategies SHALL be centrally registered rather than duplicated per market.

Each strategy declares as applicable:

- strategy identity/version;
- school/method family;
- supported Market Profile classes;
- asset/instrument applicability;
- applicability predicates;
- required Data Products/features;
- intended horizon/session;
- known limitations;
- Risk assumptions/requirements;
- execution requirements;
- validation/Intended Use evidence;
- experiment lineage;
- current lifecycle/status.

A market-specific fact belongs in the Market Profile or other correct owner, not as a duplicated copy of the strategy.

## 10.2 Strategy Controller

The Strategy Controller uses current Market Profiles, strategy applicability, analysis evidence, Trading state, and other authorized inputs to determine eligible strategies for the current context.

It may compare/combine recommendations under governed rules but cannot override:

- Unified Risk;
- capital reservation/availability;
- Guardian restrictions;
- user/Owner/subscription controls;
- broker/account/environment capability;
- execution safety;
- market admission;
- current validation scope.

## 10.3 Decision Completeness

Trading decision outcomes SHALL explicitly support states such as:

- `TRADE_CANDIDATE`;
- `NO_TRADE`;
- `DEFER`;
- `INSUFFICIENT_EVIDENCE`;
- `REJECTED_BY_RISK`;
- `RESTRICTED_BY_CONTROL`;
- other separately defined non-action states where required.

Falcon SHALL not force a trade merely because one strategy produced a signal.

---

# 11. T-LSA-07 — Unified Risk Management

Owns Trading Risk business semantics and runtime Risk decisions within accepted authority.

Unified Risk SHALL consider as applicable:

- per-trade risk;
- per-instrument exposure;
- per-strategy exposure;
- per-market exposure;
- correlated-cluster/common-factor exposure;
- aggregate portfolio risk;
- concentration;
- drawdown;
- daily/session loss;
- liquidity;
- volatility;
- execution/slippage risk;
- existing obligations/open exposure;
- intended risk versus worst credible loss;
- capital-preservation floors;
- current market/account/environment context;
- current Guardian restrictions as independent constraints.

## 11.1 Dynamic but Bounded Risk

Risk values may adapt inside valid Owner/governance envelopes when separately authorized and evidence-supported.

```text
DYNAMIC_RISK != UNBOUNDED_RISK
```

Risk may always reduce toward zero/no-trade when protection requires it.

Initial 1:1 funded scope does not permit Risk to create leverage by selecting a larger exposure.

## 11.2 Risk Resize

If Unified Risk changes a proposed size or another material Risk property, the result is a new bounded decision state.

Downstream gates SHALL bind the new exact Risk decision/version.

Old strategy confidence or an earlier pre-dispatch proof is not execution authority for the resized intent.

## 11.3 Risk Non-Authority

Unified Risk does not own:

- Guardian cross-domain crisis classification;
- Foundation resources;
- broker outcome truth;
- FSAPMA data truth;
- strategy catalog ownership;
- Owner authority.

---

# 12. T-LSA-08 — Portfolio & Capital Management

Owns Trading portfolio/capital business semantics including:

- Trading capital availability model;
- portfolio composition;
- allocation/reservation policy;
- concentration/business exposure coordination;
- capital reservations for admitted Trading intents;
- reservation release/reconciliation;
- multi-market capital coordination;
- user/account/portfolio scope;
- enforcement of the initial 1:1 funded capital model within Trading business semantics.

## 12.1 Global Capital Reservation Ledger

Trading SHALL maintain a governed capital-reservation mechanism sufficient to prevent double allocation across simultaneous candidate decisions and markets.

A reservation SHALL bind as applicable:

- reservation identity;
- user/account/portfolio;
- market/instrument;
- Trading Intent;
- amount/notional;
- currency;
- Risk decision/version;
- effective/expiry state;
- release/commit/reconciliation state;
- causation/correlation.

```text
TRADING_CAPITAL_RESERVATION != BROKER_BUYING_POWER_TRUTH
TRADING_CAPITAL_RESERVATION != FOUNDATION_RESOURCE_GRANT
```

Broker funds/buying-power truth is separately checked through authorized execution/broker paths when required.

## 12.2 1:1 Funded Exposure Invariant

For the initial scope:

```text
AUTHORIZED_TRADING_EXPOSURE
<= GOVERNED_AVAILABLE_FUNDED_CAPITAL
```

subject also to stricter Unified Risk, Guardian, user/Owner, broker, market, and validation limits.

The funded-capital rule is a ceiling, not a target.

---

# 13. T-LSA-09 — Execution & Position Lifecycle

Owns Trading execution business semantics, broker order lifecycle, execution-outcome interpretation, position lifecycle, and reconciliation.

## 13.1 Exact Binding

Before broker submission, Trading SHALL bind exact:

- user;
- account;
- environment;
- market/instrument;
- broker/service role;
- broker account;
- order intent;
- capability requirements;
- Risk decision/version;
- capital reservation;
- Guardian/user/Owner/subscription control epochs;
- applicable validation/intended-use scope.

## 13.2 Broker Capability Profile

Broker capability state SHALL distinguish:

- supported;
- unsupported;
- conditionally supported;
- unknown/unverified.

```text
UNKNOWN != SUPPORTED
```

Unsupported/unknown capabilities SHALL not be silently emulated if emulation changes Risk/protection/business semantics without separately reviewed design and authority.

## 13.3 Outcome Truth

```text
ORDER_REQUEST != SUBMISSION_ATTEMPT
SUBMISSION_ATTEMPT != BROKER_ACK
BROKER_ACK != FILL
PARTIAL_FILL != FULL_FILL
CANCEL_REQUEST != CANCELLED
CLOSE_REQUEST != ZERO_EXPOSURE
```

## 13.4 Ambiguous Execution

Submission timeout, uncertain broker state, late ACK, partial fill, cancel/replace race, or conflicting broker evidence SHALL enter reconciliation rather than blind retry.

Retry is allowed only when the business action is proven safe/idempotent or a new governed action is constructed.

## 13.5 Execution Runtime Cell

An Execution Runtime Cell may later be used as an implementation fault-domain pattern.

It is not a new Application, LSA, authority source, broker principal, or cross-Application identity.

---

# 14. T-LSA-10 — Trading Learning & Knowledge

Owns Trading knowledge derived from attributable internal outcomes and approved learning processes.

It may maintain:

- durable lessons;
- process/outcome learning;
- market/strategy behavior knowledge;
- failure lessons;
- knowledge provenance;
- drift observations;
- competence/uncertainty knowledge;
- known-invalidated beliefs;
- links to experiment and decision evidence.

It does not directly promote a strategy/model or create Trading authority.

Learning feeds the governed evolution path through the actual origin owner and P0-C governance.

---

# 15. T-LSA-11 — Trading Analytics & Attribution

Owns Trading analytics, attribution, and performance/effectiveness measurement.

It SHALL distinguish:

- decision quality;
- outcome quality;
- strategy contribution;
- Risk contribution;
- execution contribution;
- market/regime context;
- capital usage;
- opportunity cost/counterfactual evidence where valid;
- uncertainty/statistical limitations.

Profit alone does not prove sound decision quality.

Analytics does not create strategy, Risk, execution, or Owner authority.

---

# 16. T-LSA-12 — Strategy Evolution & Experimentation

Owns branch-level awareness/evaluation for Trading strategy/model evolution and experimentation while P0-C owns the general self-development governance lifecycle.

T-LSA-12 may coordinate:

- strategy/model candidate experiments;
- sandbox/replay/Paper evidence collection when separately authorized;
- experiment preregistration;
- comparison to baselines;
- candidate lineage;
- interaction with FSTSimA validation;
- experiment portfolio management;
- evidence handoff to Trading MSA.

It SHALL NOT:

- self-promote candidates;
- bypass Trading MSA;
- bypass FSA/Owner governance;
- turn Paper success into Live authority;
- expand market/Risk/leverage scope without renewed validation and governance;
- treat experimentation authority as broker execution authority.

---

# 17. T-LSA-13 — Trading Resource Management

Owns Trading Application resource awareness/evaluation.

It understands:

- admitted Trading allocation/ceiling/resource truth visible from Foundation;
- internal Trading workload demand;
- resource pressure;
- workload criticality evidence;
- internal reservation/distribution state;
- throttling/shedding effects;
- restoration/recovery evidence;
- need for additional Foundation resources.

T-LSA-13 is an awareness/evaluation branch. It is not the operational controller.

---

# 18. TARC — Trading Application Resource Controller

TARC is the sole operational resource controller for the Falcon Self-Aware Trading Application and the sole Trading-side role authorized to communicate Trading Application resource requests/outcomes with Foundation when the Foundation runtime capability exists.

```text
T_LSA13 != TARC
```

Within valid authority, TARC MAY:

- maintain the internal resource picture;
- distribute the actual admitted Trading allocation;
- reserve/rebalance/throttle/shed/restore internally;
- aggregate resource need/pressure/evidence;
- resolve effective internal resource tier from admitted versioned policy;
- submit ordinary/emergency Trading resource requests when the runtime boundary exists and internal redistribution is insufficient;
- consume Foundation resource decisions/outcomes.

TARC SHALL NOT:

- own total-resource truth;
- create Foundation grants;
- own Trading Risk;
- own strategy evolution;
- own architecture development;
- own Guardian crisis authority;
- become an MSA/LSA/CSA;
- act as resource controller for FSAPMA/FSTSimA/Guardian/Shared Applications.

```text
REQUESTED_RESOURCE != GRANTED_RESOURCE
```

---

## 19. TARC Requester Singularity

Trading internal roles may provide need/pressure/urgency evidence to TARC, including:

- Trading MSA;
- LSAs;
- CSAs;
- Unified Risk;
- Execution;
- strategies;
- analysis components;
- Guardian-related protection signals.

They SHALL NOT independently request Trading Application resources from Foundation.

```text
TRADING_FOUNDATION_RESOURCE_REQUESTER = TARC_ONLY
```

No Guardian direct/break-glass Foundation resource request exists for Trading Application resources.

TARC failure/unavailability fails closed and does not mint a second requester.

Future separately authorized redundancy must preserve one fenced logical requester identity and reject stale/split-brain authority.

---

## 20. Resource Priority Separation

Trading business workload lane, TARC internal resource tier, Foundation Application resource priority, and Foundation technical criticality are distinct dimensions.

Caller-proposed priority is evidence only.

```text
BUSINESS_LANE
!= TARC_RESOURCE_TIER
!= FOUNDATION_APPLICATION_PRIORITY
!= FOUNDATION_TECHNICAL_CRITICALITY
```

Stage 6 WP-04 accepted Application-priority / technical-criticality governance does not create TARC request runtime or let Trading override protected Foundation floors.

---

# 21. Canonical Trade Admission Chain

The current chain is:

```text
CONTEXT / AUTHORITY
 -> MARKET / INSTRUMENT ELIGIBILITY
 -> FSAPMA DATA PRODUCT ELIGIBILITY / FRESHNESS
 -> ANALYSIS
 -> STRATEGY APPLICABILITY
 -> DECISION COMPLETENESS
 -> UNIFIED RISK DECISION
 -> CAPITAL RESERVATION
 -> USER / OWNER / SUBSCRIPTION CONTROLS
 -> GUARDIAN RESTRICTIONS
 -> EXACT BROKER / ACCOUNT / ENVIRONMENT / CAPABILITY ELIGIBILITY
 -> TRADING INTENT FINALIZATION
 -> EXECUTION PLAN / ORDER PREPARATION
 -> PRE-DISPATCH SAFETY VALIDATION
 -> DISPATCH-TIME REVALIDATION OF MATERIAL MUTABLE GATES
 -> BROKER SUBMISSION ATTEMPT
 -> BROKER OUTCOME TRUTH
 -> T-LSA-09 EXECUTION / POSITION RECONCILIATION
```

No earlier stage assumes a later gate will pass.

---

## 22. Pre-Dispatch Safety

Pre-dispatch safety occurs only after exact Trading Intent and Execution Plan exist.

It validates as applicable:

- exact order parameters;
- current Risk decision/version;
- current capital reservation;
- current Guardian/control epochs;
- current user/Owner/subscription state;
- current broker/account/environment capability;
- order-size/price/notional sanity;
- initial 1:1 funded exposure rule;
- duplicate/idempotency identity;
- market/session eligibility;
- material Data Product freshness;
- maximum authorized exposure/consequence.

It is not a strategy-selection stage.

---

## 23. Dispatch-Time Revalidation

Material mutable gates SHALL be rechecked immediately before broker dispatch as required by consequence and latency.

At minimum, a stale material change in:

- Guardian restriction epoch;
- user/Owner stop/restriction state;
- Unified Risk validity;
- capital reservation/funded-capital validity;
- account/broker capability;
- market/session eligibility;
- critical Data Product validity;
- environment/business authorization;

must prevent stale opening work from dispatch.

---

## 24. Control Epochs

Material control changes SHALL create attributable epochs/versions so stale work can be invalidated.

Examples:

- user stop-new-exposure;
- Owner restriction;
- Guardian directive;
- Risk invalidation;
- subscription/entitlement change;
- broker/account authority change.

```text
STALE_CONTROL_EPOCH -> NO_NEW_DISPATCH
```

Existing positions remain managed through valid protection/reconciliation paths.

---

## 25. No Implicit Broker Fallback

A failed broker route/account SHALL not cause automatic fallback to another broker/account unless an explicit future policy is separately designed, Risk-reviewed, authorized, and validated.

Different brokers may have different account ownership, buying power, order capabilities, fees/slippage, market access, positions, and protection semantics.

```text
BROKER_A_FAILURE != AUTHORITY_TO_USE_BROKER_B
```

---

## 26. Counterfactual Decision Ledger

Trading may maintain counterfactual evidence describing what might have occurred under an alternative/declined decision for learning/analytics.

Counterfactual evidence SHALL be clearly classified and never represented as realized P&L, executed-order truth, or authority.

```text
COUNTERFACTUAL_OUTCOME != REALIZED_OUTCOME
```

---

## 27. Cross-Boundary Dependencies

- FSAPMA provides operational Data Products through declared P0-F contracts;
- Guardian provides protection/restriction outcomes through governed route capability;
- Foundation provides lifecycle/resource/communication/security boundaries within accepted scopes;
- broker execution requires FCR-0014 external execution egress/credential boundary;
- TARC Foundation resource request runtime depends on FCR-0007/FCR-0010 later capability;
- Stage 6 WP-04 priority/technical-criticality governance is accepted but does not supply those later runtimes;
- P0-J defines performance/resource/QoS overload semantics;
- P0-K defines validation/credibility/promotion semantics;
- P0-C governs self-development of Trading components/branches/Application.

---

## 28. Failure / Degraded Behavior

Examples:

- strategy confidence high and Risk rejects: no trade;
- proposed exposure exceeds funded 1:1 scope: no new exposure;
- capital reservation conflicts with current broker funds truth: hold/reconcile, no fabricated funds;
- control epoch changes before dispatch: cancel/suppress stale opening work;
- broker capability unknown: unsupported for affected action;
- submission timeout: reconcile before unsafe retry;
- partial fill/cancel race: preserve exact executed/open quantity truth;
- Candidate Universe removes a held instrument: position remains managed;
- TARC unavailable: no alternate Trading Foundation resource requester;
- Data Product degraded: new exposure authority reduced/denied according to policy while existing exposure receives protective management;
- proposed new market outside US Equities/Crypto Spot initial scope: reject until separate scope expansion is accepted.

---

## 29. Explicit Non-Authority

Trading Application SHALL NOT:

- acquire operational external data outside FSAPMA;
- own Guardian protection/crisis scope;
- own Foundation total resources;
- request Foundation Trading resources through any role other than TARC;
- infer broker egress from provider egress;
- let analysis/strategy confidence override Unified Risk;
- treat broker ACK as fill;
- treat counterfactual outcomes as realized truth;
- introduce leverage/derivatives/new markets by implication;
- exceed the initial 1:1 funded exposure model because a strategy/Risk model is confident.

---

## 30. Invariants

```text
TRADING_MSA_COUNT = 1
TRADING_LSA_COUNT = 13
INITIAL_MARKETS = US_EQUITIES + CRYPTO_SPOT
INITIAL_EXPOSURE_MODEL = 1_TO_1_FUNDED
INITIAL_1_TO_1_FUNDED_SCOPE != PERMISSION_FOR_LEVERAGE
OUT_OF_SCOPE_MARKET != AUTOMATIC_MARKET_ADMISSION
T_LSA13 != TARC
TARC = SOLE_TRADING_FOUNDATION_RESOURCE_REQUEST_ROLE
T_LSA01 != RESOURCE_OWNER
ANALYSIS_SIGNAL != TRADING_AUTHORITY
OPPORTUNITY_FOUND != TRADE_AUTHORIZED
STRATEGY_CONFIDENCE != RISK_OVERRIDE
TRADING_CAPITAL_RESERVATION != BROKER_BUYING_POWER_TRUTH
UNKNOWN_BROKER_CAPABILITY != SUPPORTED
ORDER_REQUEST != BROKER_ACK != FILL
AMBIGUOUS_EXECUTION -> RECONCILIATION_BEFORE_UNSAFE_RETRY
BROKER_A_FAILURE != BROKER_B_AUTHORITY
COUNTERFACTUAL_OUTCOME != REALIZED_OUTCOME
```

---

## 31. Forbidden Interpretations

Invalid interpretations include:

- “P0-H is market-agnostic, so any market is initially allowed”;
- “1:1 means target leverage of one rather than a funded maximum”;
- “Risk can approve leverage because it owns Risk”;
- “Crypto scope includes derivatives because the underlying is crypto”;
- “T-LSA-13 is TARC”;
- “Trading MSA can directly request Foundation resources”;
- “Guardian emergency can bypass TARC”;
- “T-LSA-01 owns resources because it sees readiness”;
- “a strategy signal is a trade”;
- “Risk resize can reuse an old pre-dispatch proof”;
- “capital reservation proves broker buying power”;
- “unknown broker capability is probably supported”;
- “submission timeout means order failed, so retry immediately”;
- “instrument left Candidate Universe, so liquidate automatically”;
- “successful counterfactual outcome rewrites prior decision quality”.

---

## 32. Mandatory Scenarios

At minimum challenge:

- US Equities order under initial scope;
- Crypto Spot order under initial scope;
- leveraged crypto product presented as Crypto Spot;
- options/futures request presented under US Equities scope;
- exposure above current governed funded capital;
- simultaneous US Equity/Crypto reservations competing for same capital;
- Risk resize after strategy decision;
- capital reservation vs stale broker buying power;
- stop/control epoch change before dispatch;
- duplicate Trading Intent;
- submission timeout then late broker ACK;
- partial fill then cancellation;
- cancel/replace race;
- unknown broker capability;
- unsupported native protection;
- universe removal with open exposure;
- one user/account execution failure while another remains healthy;
- unauthorized broker fallback attempt;
- strategy confidence high / Risk reject;
- NO_TRADE later profitable;
- hidden correlation rise in crisis regime;
- TARC caller priority inflation;
- TARC unavailable/split-brain/stale requester authority;
- attempt to onboard a third market without scope expansion governance.

---

## 33. Exit Gates

```text
TRADING_13_LSA_OWNERSHIP = PASS
INITIAL_MARKET_SCOPE = EXPLICIT
INITIAL_1_TO_1_FUNDED_SCOPE = EXPLICIT
IMPLICIT_LEVERAGE_OR_DERIVATIVE_AUTHORITY = 0
IMPLICIT_NEW_MARKET_AUTHORITY = 0
RESPONSIBILITY_COOWNERSHIP = 0
TRADE_ADMISSION_CHAIN = COMPLETE
RISK_AUTHORITY_COLLISIONS = 0
CAPITAL_DOUBLE_ALLOCATION_PATHS = 0
STALE_CONTROL_EXECUTION_PATHS = 0
BROKER_CAPABILITY_UNKNOWN_AS_SUPPORTED = 0
EXECUTION_AMBIGUITY_BLIND_RETRY = 0
COUNTERFACTUAL_REALIZED_TRUTH_COLLAPSE = 0
TARC_ALTERNATE_FOUNDATION_REQUESTER_PATHS = 0
TARC_GUARDIAN_AUTHORITY_COLLISIONS = 0
FCR0007_RUNTIME_STATE = EXPLICIT
FCR0010_RUNTIME_STATE = EXPLICIT
FCR0014_RUNTIME_STATE = EXPLICIT
```

---

## 34. Next Authorized Gate

P0-H acceptance would establish Trading architecture semantics only. It would not authorize broker connectivity, broker credentials, order submission, resource-request runtime, Paper, Tiny Live, Live, deployment, leverage, derivatives, or additional markets.
