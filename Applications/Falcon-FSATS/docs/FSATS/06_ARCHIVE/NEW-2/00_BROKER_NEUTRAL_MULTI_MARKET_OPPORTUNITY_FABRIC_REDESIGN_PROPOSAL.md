# Falcon FSATS Broker-Neutral Multi-Market Opportunity Fabric Redesign Proposal

**Proposal ID:** `FSATS-FMOF-PROPOSAL-001`  
**Architecture Name:** `Falcon Market Opportunity Fabric (FMOF)`  
**Workspace:** `applications/docs/FSATS/NEW-2/`  
**Branch:** `application-development`  
**Status:** `DESIGN_CHANGE_PROPOSAL / OUTSIDE_CURRENT_R7_FREEZE / NOT_OWNER_ACCEPTED / NOT_CLOSED`  
**Change Classification:** `DCC-3 — MATERIAL_DOMAIN_CHANGE`  
**Current R7 Semantic Freeze:** `UNCHANGED BY THIS PROPOSAL`  
**Implementation Authority:** `NOT_GRANTED`  
**Runtime Authority:** `NOT_GRANTED`  
**Paper / Tiny Live / Live / Deployment Authority:** `NOT_GRANTED`  
**Broker / Provider Connectivity Authority:** `NOT_GRANTED`  
**Research Date for External Facts:** `2026-08-12`  

---

# 1. Executive Decision Proposal

The current FSATS universe architecture should **not** be evolved by merely replacing `Top-10` with `Top-100`, increasing the number of symbols, or changing price-zone thresholds.

That would enlarge the same abstraction error rather than remove it.

The proposed architecture replaces the semantic idea of a single fixed `Qualified Universe` with a **Broker-Neutral, Account-Neutral, Multi-Market Opportunity Fabric** that separates six different truths that the current design partially collapses together:

```text
1. MARKET / INSTRUMENT TRUTH
2. OBSERVATION / DATA-COVERAGE TRUTH
3. DISCOVERY / OPPORTUNITY TRUTH
4. BROKER CAPABILITY TRUTH
5. ACCOUNT ACCESS / FEASIBILITY TRUTH
6. RISK / CAPITAL / EXECUTION AUTHORITY TRUTH
```

The central design rule is:

```text
AN INSTRUMENT MAY EXIST IN THE MARKET
WITHOUT BEING CURRENTLY OBSERVABLE BY EVERY PROVIDER,
TRADABLE BY EVERY BROKER,
ACTIONABLE BY EVERY ACCOUNT,
OR AUTHORIZED FOR A TRADE.

NONE OF THOSE LATER FAILURES MAY DELETE THE EARLIER TRUTH.
```

This creates an architecture that can serve:

- many users;
- many accounts;
- different brokers at the same time;
- multiple markets with different market mechanics;
- fractional and whole-share execution models;
- providers with different coverage, freshness, quotas and commercial entitlements;
- small and large accounts without making nominal share price a hidden fairness gate;
- constrained resources without converting resource pressure into false market truth.

This proposal preserves the strongest parts of R7, including exact ownership, Risk/Capital separation, FSAPMA provider ownership, broker reconciliation, Guardian independence, provenance, deterministic failure handling, resource governance, strategy centralization, FSTSimA isolation and Awareness boundaries.

It changes only the semantic area that needs to change and declares the downstream impact explicitly.

---

# 2. Authority and Source Basis

This proposal was formed only after source-first review of the complete current `applications/docs/FSATS/NEW/**` workspace, current governing Application sources, current FCR state, exposed V1.3 reference evidence, and current official external documentation.

Governing order remains:

```text
Falcon Vision
>
Falcon Constitution
>
Current explicit Owner decisions
>
Approved Specifications / Contracts / Accepted ADRs
>
Current Foundation capability and FCR state
>
Current FSATS semantic design
>
This proposal
>
Historical P0 / P1 / V1.3 design knowledge
```

The proposal preserves these higher-order requirements:

```text
PROTECT > MANAGE > GROW
UNKNOWN != SAFE
CAPABILITY != AUTHORITY
OBSERVATION != TRUTH BY ITSELF
RECOMMENDATION != AUTHORIZATION
BROKER EVIDENCE != APPLICATION STATE
PROVIDER COVERAGE != MARKET EXISTENCE
ACCOUNT FEASIBILITY != MARKET QUALITY
RESOURCE SCARCITY != MARKET INVALIDITY
OWNER ACCEPTANCE != IMPLEMENTATION AUTHORITY
```

The V1.3 historical reference remains design input only. The currently exposed reference evidence reports an original 289-entry owner package, but the current reference surface does not expose all 289 original file bodies. This proposal therefore does not claim a fresh byte-by-byte reread of unavailable V1.3 bodies.

---

# 3. Problem Statement

## 3.1 Current Price Zones encode the wrong abstraction

The current US-equities model uses `ZONE_C / ZONE_B / ZONE_A` and fixed Top-10 selection patterns tied partly to nominal share price and capital size.

This creates an architectural bias because nominal share price is not the same thing as account feasibility.

A $1,000 stock can be feasible for a small account when the relevant broker/account combination permits a sufficiently small fractional order. A $10 stock can still be infeasible because of account restrictions, broker capability, minimum order rules, risk, liquidity, execution quality or capital constraints.

Therefore:

```text
NOMINAL_PRICE != ACCOUNT_FEASIBILITY
LOW_PRICE != GOOD_SMALL_ACCOUNT_OPPORTUNITY
HIGH_PRICE != LARGE_ACCOUNT_ONLY
```

## 3.2 Fixed Top-K is not a Market Universe

A fixed Top-10 or Top-100 is a work-selection result, not an adequate definition of the market universe.

Using fixed K as the canonical universe causes:

- blind spots outside the cutoff;
- cutoff churn;
- repeated deep analysis of incumbents;
- starvation of emerging instruments;
- concentration of attention in a narrow style or liquidity cohort;
- accidental bias toward the capabilities of whichever Broker/account was used during ranking;
- lost opportunities when market structure changes faster than the fixed list refreshes.

The correct semantic distinction is:

```text
UNIVERSE = WHAT IS KNOWN TO EXIST / BE ELIGIBLE FOR CONSIDERATION

WORKSET = WHAT FALCON CHOOSES TO SPEND EXPENSIVE ANALYSIS ON NOW
```

Those are not the same object.

## 3.3 Current global ranking leaks downstream facts upstream

The current `07E` ranking includes dimensions such as diversification and spread/execution fitness.

Some of these are not globally neutral:

- diversification is portfolio/account dependent;
- concentration is portfolio/account dependent;
- executable spread/cost may depend on Broker, route, market-data coverage and session;
- minimum quantity/notional is Broker/account dependent;
- fractional capability is Broker/instrument/account dependent.

A global market score must not quietly include private portfolio or Broker-specific execution facts and then present them as universal opportunity truth.

## 3.4 Provider coverage can masquerade as market truth

FSAPMA correctly owns provider capability, entitlement, quota, data quality and routing. The current universe design must make this separation stronger.

If a free provider covers only part of real-time market activity, then:

```text
NOT OBSERVED BY THIS PROVIDER
!=
DOES NOT EXIST
```

Similarly:

```text
FREE DATA COVERAGE
!=
COMPLETE MARKET COVERAGE
```

Provider loss, quota exhaustion or entitlement change should degrade `ObservationCoverage`, not silently shrink the canonical economic universe and call the smaller set the market.

## 3.5 A single Broker cannot define opportunity for all users

For a multi-user system:

```text
BROKER_A_UNSUPPORTED(INSTRUMENT_X)
DOES NOT IMPLY
GLOBAL_UNSUPPORTED(INSTRUMENT_X)
```

If Broker B supports the instrument and a Broker B client can safely act on it, Broker A's limitation must not erase the opportunity globally.

## 3.6 Multi-market ranking requires market-native semantics

US equities and Crypto Spot do not share identical:

- session structure;
- trading calendar;
- liquidity pattern;
- quote behavior;
- tick/step behavior;
- 24/7 continuity;
- venue structure;
- corporate-action semantics;
- volatility distribution;
- data cadence.

A raw score that pretends these distributions are directly interchangeable can be numerically neat and semantically wrong.

---

# 4. What the Existing 07A Proposal Got Right

The existing `07A_BROKER_NEUTRAL_ADAPTIVE_UNIVERSE_REDESIGN_PROPOSAL.md` is materially stronger than the fixed-zone design and should be treated as a predecessor design candidate, not discarded knowledge.

It correctly establishes:

- broad canonical market consideration instead of Top-K as universe definition;
- shared cheap/wide scanning;
- Global Opportunity separated from Broker capability;
- Account feasibility separated from global market quality;
- per-account actionable projection;
- Fractional capability as a capability, not an automatic action authorization;
- adaptive deep-analysis breadth instead of a permanent Top-100;
- coverage-aware scheduling;
- global analysis reuse between clients;
- Broker-cohort reuse where semantics match;
- portfolio diversification/concentration owned downstream by Risk/Capital;
- T-LSA-02 as consumer of Broker/account projections rather than owner of those facts;
- FSAPMA as the sole operational provider-data gateway;
- fairness as fair access to consideration rather than equal trade counts.

This proposal keeps those strengths.

---

# 5. Where 07A Is Still Not Strong Enough

The predecessor proposal still leaves important ambiguity in a system intended to scale to many users, brokers and markets.

## 5.1 It lacks first-class Observation Coverage Truth

The design distinguishes global opportunity from Broker and account feasibility, but it does not fully materialize:

```text
WHAT EXISTS
versus
WHAT FALCON CAN CURRENTLY OBSERVE WITH SUFFICIENT QUALITY
```

Without that truth plane, a provider outage can still contaminate universe semantics.

## 5.2 Instrument identity remains too symbol-centric

A durable system must survive:

- symbol change;
- symbol reuse;
- corporate action;
- delisting/relisting;
- share-class distinctions;
- ADR/depositary representation;
- venue/listing differences;
- provider symbol differences;
- Broker symbol/asset-ID differences;
- Crypto base/quote pair representation changes.

`SYMBOL = IDENTITY` is not strong enough.

## 5.3 Coverage-aware scheduling lacks a measurable anti-starvation mechanism

Saying “be fair” is not enough for deterministic implementation.

The system needs an explicit ledger showing:

- when an instrument was last considered;
- when it was last deeply analyzed;
- why it was skipped;
- how much consideration debt it has accumulated;
- which evidence became stale;
- whether one Broker/user cohort is consuming a disproportionate share of expensive analysis.

## 5.4 Broker cohorts need semantic fingerprints, not Broker names

Two accounts at the same Broker may differ in:

- entitlements;
- account type;
- fractional permissions;
- region/jurisdiction;
- trading session capability;
- product access;
- order type support.

Therefore caching by `BrokerName` alone is unsafe.

## 5.5 Cross-market comparability needs explicit normalization boundaries

A market-neutral architecture must preserve market-native truth first, then create explicit comparable projections only where semantics justify them.

## 5.6 UNKNOWN must be a first-class result

The design must prevent these false collapses:

```text
UNKNOWN -> UNSUPPORTED
UNKNOWN -> SUPPORTED
UNKNOWN -> NOT_ACTIONABLE
UNKNOWN -> ACTIONABLE
```

Unknown is evidence insufficiency, not a convenient Boolean.

## 5.7 Shared analysis needs explicit privacy and reuse boundaries

A multi-user system must reuse expensive global analysis without allowing user/account holdings, balances or restrictions to leak into shared opportunity artifacts.

---

# 6. Proposed Architecture: Falcon Market Opportunity Fabric

FMOF is not a new Falcon Application and not a new Foundation service.

It is a **Trading Application domain architecture** spanning existing Trading responsibilities while respecting their ownership boundaries.

Conceptual flow:

```text
FSAPMA NORMALIZED OPERATIONAL DATA
              |
              v
CANONICAL MARKET / INSTRUMENT REGISTRY VIEW
              |
              v
OBSERVATION COVERAGE TRUTH
              |
              v
CHEAP / WIDE MARKET DISCOVERY
              |
              v
ADAPTIVE DEEP-ANALYSIS PLANNER
              |
              v
GLOBAL OPPORTUNITY ARTIFACTS
              |
        +-----+-----+
        |           |
        v           v
BROKER CAPABILITY   ACCOUNT / ACCESS CONTEXT
PROJECTION           PROJECTION
        |           |
        +-----+-----+
              v
ACCOUNT ACTIONABILITY PROJECTION
              |
              v
STRATEGY EVALUATION / ORCHESTRATION
              |
              v
UNIFIED RISK
              |
              v
CAPITAL RESERVATION
              |
              v
EXECUTION / BROKER RECONCILIATION
```

No arrow in this diagram implies transfer of authority.

---

# 7. Six Truth Planes

## 7.1 Plane A: Canonical Market / Instrument Truth

Canonical artifact:

`CanonicalMarketUniverseSnapshot`

Purpose:

- represent the current evidence-backed set of instruments/listings known to Falcon for a declared market scope;
- preserve active/inactive/unknown lifecycle state;
- remain Broker-neutral and account-neutral;
- expose completeness explicitly.

Proposed required fields:

```text
CanonicalMarketUniverseSnapshot
- SnapshotId
- MarketId
- MarketProfileVersion
- AsOf
- ExpiresAt
- UniverseCompleteness
- IdentityGraphVersion
- InstrumentEntries[]
- EvidenceRefs[]
- ProvenanceDigest
```

`UniverseCompleteness` candidate states:

```text
COMPLETE_FOR_DECLARED_SCOPE
PARTIAL
UNKNOWN
CONFLICTED
```

Falcon must never claim completeness when its evidence does not establish it.

## 7.2 Plane B: Observation / Data-Coverage Truth

Canonical artifact:

`ObservationCoverageSnapshot`

Purpose:

- state what Falcon can currently observe;
- keep provider limitations out of market existence truth;
- record freshness, quality, lineage, quota/entitlement limitations and known gaps.

Candidate states per Data Product / instrument / market:

```text
OBSERVED_VALID
OBSERVED_DEGRADED
STALE
NOT_CURRENTLY_COVERED
QUOTA_CONSTRAINED
ENTITLEMENT_CONSTRAINED
CONFLICTED
UNKNOWN
```

Invariant:

```text
OBSERVATION_GAP != MARKET_DELETION
```

## 7.3 Plane C: Discovery / Opportunity Truth

Canonical artifacts:

- `MarketDiscoverySnapshot`
- `DeepAnalysisCandidate`
- `GlobalOpportunityArtifact`

This plane uses market information only.

It SHALL NOT consume account holdings, account cash, account concentration or private portfolio state to produce a global opportunity assessment.

## 7.4 Plane D: Broker Capability Truth

Canonical projection:

`BrokerCapabilityProjection`

Owner remains Trading Execution / T-LSA-09 and its certified Broker adapter/profile semantics.

Candidate support states:

```text
SUPPORTED
SUPPORTED_WITH_RESTRICTIONS
UNSUPPORTED
TEMPORARILY_UNAVAILABLE
UNKNOWN
CONFLICTED
```

Projection may include:

- exact Broker service/account-class identity;
- tradability;
- fractional capability;
- minimum quantity/notional;
- quantity step;
- order-type capability;
- session capability;
- market/product access;
- evidence acquisition path;
- effective time;
- certification identity/version;
- expiration/revalidation trigger.

A Broker projection is not market truth.

## 7.5 Plane E: Account Access / Feasibility Truth

Canonical projection:

`AccountActionabilityProjection`

This projection is private to the relevant User/Account boundary.

Candidate states:

```text
ACTIONABLE_CANDIDATE
CONDITIONALLY_ACTIONABLE
NOT_ACTIONABLE
UNKNOWN
CONFLICTED
```

It may consider structural access facts such as:

- Account enabled/healthy state;
- Broker binding;
- product permission;
- regional/account restrictions;
- fractional permission where applicable;
- minimum-order feasibility;
- session access;
- known account-level eligibility restrictions.

It SHALL NOT grant final Risk or capital authority.

## 7.6 Plane F: Risk / Capital / Execution Authority Truth

This proposal does not move or weaken existing ownership.

```text
T-LSA-07 = Unified Risk
T-LSA-08 = Capital / Reservation
T-LSA-09 = Execution / Broker Reconciliation
Guardian = independent protection authority within its governed scope
```

Final action remains gated by current R7 semantics.

---

# 8. Canonical Instrument Identity Graph

## 8.1 Why this is required

A symbol is a representation, not a durable identity.

FMOF therefore proposes an `InstrumentIdentityGraphView` that references authoritative facts from their current owners rather than becoming a new source of external/provider truth.

Proposed identity layers:

```text
EconomicInstrumentId
    |
    +-> MarketInstrumentId / ListingIdentity
            |
            +-> ProviderRepresentationId(s)
            |
            +-> BrokerRepresentationId(s)
```

For Crypto:

```text
CryptoAssetIdentity
    +
QuoteAssetIdentity
    +
Market / Venue Profile
    -> CanonicalPairIdentity
```

## 8.2 Ownership preservation

- FSAPMA remains owner of provider mappings and normalized external data evidence.
- Trading Market/Universe domain owns the Trading-side canonical market/instrument projection.
- T-LSA-09 owns Broker representation/capability mappings.
- No component may edit another owner's mapping store.

## 8.3 Identity must survive lifecycle events

Required future verification scenarios include:

- ticker symbol change;
- symbol reused by another security later;
- corporate-action successor/predecessor;
- delisting and relisting;
- share-class distinction;
- ADR versus ordinary share;
- Broker changes its internal asset ID;
- Provider changes symbol convention;
- Crypto venue changes pair notation.

If identity cannot be established confidently:

```text
IDENTITY = UNKNOWN / CONFLICTED
=> NO NEW RISK BASED ON GUESSED EQUIVALENCE
```

---

# 9. Cheap/Wide Discovery Layer

FMOF retains the predecessor proposal's strong wide-scan principle but makes it explicit that “wide” does not mean “expensive real-time deep analysis for every instrument on every tick.”

The wide layer operates as a rolling market census using the cheapest currently certified Data Products adequate for the declared discovery purpose.

It may combine:

- periodic reference/universe census;
- low-cost bars;
- low-cost quote/trade summaries where available;
- market movers/deltas where certified;
- event/freshness triggers;
- prior state to avoid recomputing unchanged evidence.

Each MarketProfile declares an allowed `DiscoveryCoveragePolicy`.

The output is not a trade signal and not final strategy evaluation.

```text
WIDE DISCOVERY = WHO DESERVES MORE ATTENTION?
DEEP ANALYSIS = WHAT DOES THE EVIDENCE ACTUALLY SAY?
STRATEGY/RISK = SHOULD THIS ACCOUNT DO ANYTHING?
```

---

# 10. No Fixed-K Semantic Universe

FMOF explicitly rejects these as architectural universe definitions:

```text
TOP_10 = UNIVERSE
TOP_100 = UNIVERSE
CHEAPEST_100 = UNIVERSE
BROKER_A_TRADABLE_SET = GLOBAL_UNIVERSE
ACCOUNT_A_AFFORDABLE_SET = GLOBAL_UNIVERSE
```

A deep-analysis cycle may analyze 20, 80, 137, 400 or another number of candidates depending on:

- current resources;
- provider quota state;
- market activity;
- evidence staleness;
- number of material changes;
- analysis cost;
- FSARM/resource-pressure state;
- required coverage guarantees.

That count is an output of the `AdaptiveDeepAnalysisPlan`, never the semantic definition of what the market contains.

---

# 11. Adaptive Deep-Analysis Planner

Canonical artifact:

`AdaptiveDeepAnalysisPlan`

The planner spends expensive compute/data budget deliberately.

It receives only evidence needed for analysis scheduling. It does not receive authority to trade.

## 11.1 Four mandatory service lanes

The candidate planner SHALL have at least four logical lanes:

```text
LANE 1: EXPLOITATION
Strong current market evidence / high expected analytical value

LANE 2: EXPLORATION
New, changed or insufficiently sampled candidates that could otherwise never become incumbents

LANE 3: COVERAGE-DEBT RECOVERY
Eligible candidates that have waited too long or have repeatedly lost contention for analysis

LANE 4: FRESHNESS / EVENT URGENCY
Material market/reference/event changes requiring re-analysis before stale results are reused
```

The lanes are **analysis fairness**, not capital fairness and not trade quotas.

## 11.2 Spillover rule

If a lane cannot consume its governed service budget because no eligible work exists, unused capacity may spill to other lanes according to a versioned deterministic policy.

A lane budget may never bypass:

- invalid data;
- unknown identity;
- Guardian restrictions;
- authority restrictions;
- current resource ceiling;
- required freshness/quality floors.

## 11.3 Resource-pressure behavior

When FSARM/Foundation resource pressure reduces available analysis capacity:

- expensive deep-analysis breadth shrinks first;
- discovery cadence may degrade explicitly;
- experiment/exploration intensity may reduce according to policy;
- stale artifacts are marked stale rather than silently reused;
- open-order/position/risk/protection/reconciliation workloads remain protected by the existing resource-priority architecture.

```text
LESS COMPUTE != LOWER SAFETY STANDARD
```

---

# 12. Opportunity Consideration Ledger

FMOF proposes a Trading-owned `OpportunityConsiderationLedger`.

Its purpose is to prevent invisible starvation and make coverage measurable.

Candidate fields:

```text
OpportunityConsiderationRecord
- MarketId
- MarketInstrumentId
- LastWideScanAt
- LastDeepAnalysisAt
- LastMeaningfulChangeAt
- LastEligibleAt
- LastSkippedAt
- LastSkipReason
- ConsecutiveEligibleSkips
- CoverageDebt
- ObservationFreshnessState
- RequiredReconsiderationAt
- LastAnalysisPolicyVersion
- LastInputDigest
```

The ledger does **not** store account balances or private portfolio holdings.

## 12.1 Fairness definition

```text
FAIRNESS = FAIR ACCESS TO CONSIDERATION
```

Not:

```text
FAIRNESS != EQUAL TRADE COUNT
FAIRNESS != EQUAL CAPITAL
FAIRNESS != EQUAL SCORE
FAIRNESS != FORCE A BAD INSTRUMENT INTO ANALYSIS
```

An instrument earns consideration service only while it satisfies the relevant hard discovery eligibility and evidence conditions.

---

# 13. Preventing Score Feedback Loops

A pure “highest score gets analyzed again” model creates a rich-get-richer loop:

```text
MORE ANALYSIS
-> BETTER FEATURES
-> HIGHER CONFIDENCE
-> HIGHER PRIORITY
-> MORE ANALYSIS
```

FMOF breaks this loop through:

- Exploration lane;
- Coverage-debt lane;
- evidence-age penalties;
- input-change triggers;
- explicit last-considered history;
- separation of `AnalysisPriority` from `TradeOpportunityQuality`.

A candidate may receive analysis because Falcon is uncertain or under-covered, not because Falcon already believes it is a good trade.

This is epistemically important:

```text
NEED_TO_KNOW_MORE != EXPECTED_TO_BUY
```

---

# 14. Global Opportunity Artifact

After deep analysis, Trading may produce a reusable:

`GlobalOpportunityArtifact`

Required property:

```text
GLOBAL OPPORTUNITY ARTIFACT CONTAINS NO CLIENT-PRIVATE FINANCIAL STATE
```

Candidate bindings:

- `MarketId`;
- `MarketInstrumentId`;
- `MarketProfileVersion`;
- feature-set/model/algorithm versions;
- input Data Product identities and digests;
- observation quality/freshness;
- global market-native opportunity metrics;
- uncertainty/confidence;
- valid-from / valid-until;
- provenance refs;
- analysis policy version.

It must not contain:

- User balance;
- Account buying power;
- private holdings;
- private concentration;
- account-specific Risk limit;
- Broker account credential/ref;
- client-specific capital allocation.

This makes expensive analysis safely reusable.

---

# 15. Broker Capability Projection and Cohort Reuse

## 15.1 Broker brand is not a sufficient cache key

FMOF proposes:

`BrokerCapabilityFingerprint`

The fingerprint is derived from exact effective semantics, for example:

```text
BrokerId
BrokerAdapterProfileVersion
Environment
AccountClass / applicable capability class
Market/Product entitlement set
Fractional semantics
Order-type semantics
Session semantics
Min quantity/notional semantics
Required evidence paths
Relevant restriction profile
EffectiveAt / ExpiresAt
```

Only projections with equivalent governed fingerprints may share a Broker-cohort cache.

## 15.2 Multi-broker system does not imply unsafe smart routing

This proposal explicitly distinguishes:

```text
MULTI_BROKER_SYSTEM
!=
ONE_ORDER_AUTOMATICALLY_ROUTED ACROSS MULTIPLE BROKERS
```

FSATS may serve Account A bound to Broker A and Account B bound to Broker B simultaneously.

Cross-Broker order migration, smart order routing across brokerage accounts or automatic failover of unreconciled orders remains a separate future design problem and is **not authorized or implied** by FMOF.

---

# 16. Fractional Shares and Small-Account Fairness

Fractional capability is treated as one part of feasibility, never a global universe filter.

Correct semantic chain:

```text
INSTRUMENT EXISTS
-> MARKET/DATA EVIDENCE VALID
-> GLOBAL OPPORTUNITY MAY EXIST
-> BROKER SUPPORTS OR DOES NOT SUPPORT FRACTIONAL EXECUTION
-> ACCOUNT MAY OR MAY NOT BE ELIGIBLE
-> MINIMUM NOTIONAL / QUANTITY / SESSION / ORDER-TYPE RULES APPLY
-> RISK/CAPITAL STILL DECIDE FINAL SIZE/AUTHORITY
```

Therefore:

```text
FRACTIONABLE = TRUE
!=
ACTIONABLE = TRUE
```

And:

```text
HIGH NOMINAL SHARE PRICE
!=
SMALL ACCOUNT EXCLUSION
```

This removes the unfairness created by price-zone architecture while preserving conservative feasibility checks.

---

# 17. Account Actionability Is a Projection, Not Global Truth

For each relevant Account:

```text
GlobalOpportunityArtifact
+
BrokerCapabilityProjection
+
Account Access Context
+
Current allowed feasibility projection
=
AccountActionabilityProjection
```

The output can explain exactly why a global opportunity is not currently available to one account:

```text
BROKER_UNSUPPORTED
FRACTIONAL_UNSUPPORTED
ACCOUNT_PRODUCT_RESTRICTED
SESSION_UNAVAILABLE
MIN_NOTIONAL_NOT_FEASIBLE
BROKER_CAPABILITY_UNKNOWN
ACCOUNT_STATE_UNKNOWN
DATA_BECAME_STALE
```

That reason affects only the appropriate account/projection.

It does not rewrite global market/opportunity history.

---

# 18. Final Risk and Capital Remain Downstream

FMOF intentionally does not move portfolio-sensitive facts into global discovery.

These remain downstream:

- concentration;
- correlation to current holdings;
- existing exposure;
- drawdown state;
- daily/weekly loss state;
- strategy capital competition;
- reserved capital;
- current position/open-order obligations;
- final risk-adjusted quantity.

Therefore the current global `DiversificationScore` concept should not remain a universal Market Universe score if this proposal is adopted.

Diversification remains meaningful, but its correct location is T-LSA-07/T-LSA-08 and applicable proposal/capital competition semantics.

---

# 19. Multi-User Compute Reuse Without Private-State Leakage

FMOF uses three reuse levels:

## Level 1: Global market analysis

Reusable across all users when exact input/version semantics match.

Key:

```text
MarketId
MarketInstrumentId
InputDataDigest
FeatureProfileVersion
Model/AlgorithmVersion
MarketProfileVersion
AnalysisPolicyVersion
AsOf
```

## Level 2: Broker capability cohort

Reusable only for equivalent `BrokerCapabilityFingerprint`.

No User balance/holdings are included.

## Level 3: Account actionability

Private per `UserId / AccountId / Environment`.

Not shared between accounts merely because they use the same Broker.

This structure prevents Falcon from rescanning or deeply reanalyzing the whole market once per client.

```text
ONE MARKET CHANGE
-> ONE GLOBAL RECOMPUTATION WHEN POSSIBLE
-> MANY CHEAP BROKER/ACCOUNT PROJECTIONS
```

---

# 20. Multi-Market Architecture

FMOF is N-market capable, but this proposal does not authorize new markets beyond the current governed FSATS market scope.

Current US Equities and Crypto Spot remain the initial market candidates/current design scope as governed by the existing SIA.

Future markets attach through a versioned `MarketProfile` and do not require rewriting the core FMOF semantics.

Each MarketProfile owns market-native rules such as:

- trading/session calendar;
- venue conventions;
- reference universe policy;
- instrument lifecycle semantics;
- data-product requirements;
- discovery cadence;
- liquidity/volatility normalization;
- comparable opportunity features;
- strategy applicability;
- execution constraints delegated to the correct owner;
- market-specific freshness rules.

---

# 21. Cross-Market Normalization Rule

The system must first calculate market-native evidence.

```text
US_EQUITY_NATIVE_FEATURES
CRYPTO_NATIVE_FEATURES
FUTURE_MARKET_X_NATIVE_FEATURES
```

Only after explicit normalization may a metric be used for shared resource prioritization.

FMOF proposes two separate score concepts:

## 21.1 `MarketNativeOpportunityScore`

Meaning:

- opportunity quality within the semantics of its own MarketProfile.

Not necessarily directly comparable across markets.

## 21.2 `AnalysisUtilityScore`

Meaning:

- expected value of spending the next unit of analysis resource on this candidate, normalized by a versioned resource-planning policy.

It may consider:

- information gain;
- evidence staleness;
- change magnitude;
- analysis cost;
- coverage debt;
- urgency.

It must not masquerade as expected trading return.

This prevents the resource scheduler from comparing a Crypto 24/7 volatility number directly with a US-equity regular-session volatility number as though they were the same statistical object.

---

# 22. Provider Strategy: Free-First, Truth-First, Never Free-Blind

The Owner's current operating intent to exploit free provider capability should be preserved.

The architecture should encode:

```text
FREE_FIRST = YES
UNAUTHORIZED_PAID_UPGRADE = NO
FREE_PROVIDER_LIMITS_ARE_VISIBLE = YES
FREE_PLAN_LIMITS_DEFINE_MARKET_TRUTH = NO
```

FSAPMA remains the owner of:

- provider certification;
- plan/entitlement facts;
- rate/quota limits;
- current coverage;
- cost class;
- provider health;
- data quality;
- route selection/failover;
- quota reservation.

FMOF consumes those projections.

## 22.1 Proposed provider cost classes

```text
FREE
ZERO_MARGINAL_COST_WITHIN_CURRENT_ENTITLEMENT
PAID
UNKNOWN
```

A route marked `PAID` remains unavailable unless separately authorized by Owner/governance policy.

## 22.2 Why this matters

External provider plans change over time. Hardcoding “Provider X is free forever” into Trading architecture would convert a commercial website fact into a permanent system invariant.

Instead:

```text
CURRENT PROVIDER FACT
-> FSAPMA POINT-IN-TIME CERTIFICATION
-> VERSIONED ENTITLEMENT PROFILE
-> FMOF OBSERVATION COVERAGE
```

---

# 23. Current Official Research Evidence

The following evidence was checked on `2026-08-12`. It supports the architecture but is not itself a permanent Falcon semantic baseline.

## 23.1 Fractional behavior is Broker-specific

FINRA states that fractional-share availability varies by brokerage firm, with some firms supporting broad sets, some limited sets and some none. FINRA also notes that order handling can differ between firms.

Source:  
`https://www.finra.org/investors/insights/investing-fractional-shares`

Interactive Brokers currently advertises fractional access to more than 10,500 U.S. stocks and ETFs and entry from USD 1, showing that nominal share price is not a universal account-access boundary.

Source:  
`https://www.interactivebrokers.com/en/trading/fractional-trading.php`

Alpaca currently exposes instrument-specific `fractionable` capability and states that not every asset is fractionable.

Source:  
`https://docs.alpaca.markets/us/v1.1/docs/fractional-trading`

## 23.2 Free market data can be materially incomplete

Alpaca currently documents its Basic market-data plan as free while real-time US-equity coverage is limited to IEX and websocket subscriptions are limited relative to the paid plan.

Source:  
`https://docs.alpaca.markets/us/docs/about-market-data-api`

This is direct evidence for:

```text
FREE REAL-TIME SOURCE
!=
COMPLETE US MARKET OBSERVATION
```

## 23.3 Free providers have different quotas and entitlements

Alpha Vantage currently states that the majority of endpoints are available free with a standard usage limit of 25 requests/day, while some real-time/premium functions require premium entitlement.

Sources:  
`https://www.alphavantage.co/support/`  
`https://www.alphavantage.co/premium/`

Twelve Data currently lists a free Basic tier with limited API/WS credits and daily usage while larger plans have broader limits/capability.

Source:  
`https://twelvedata.com/pricing`

Finnhub currently lists a free tier with 60 API calls/minute and materially different dataset depth from its paid offering.

Source:  
`https://finnhub.io/pricing`

Architectural conclusion:

```text
PROVIDER FREE-TIER STATUS IS A VERSIONED CAPABILITY FACT,
NOT A MARKET DEFINITION AND NOT A PERMANENT ARCHITECTURAL PROMISE.
```

---

# 24. Observation Coverage and Provider Independence

A provider brand count is not enough to establish independent confirmation.

FMOF preserves the current FSAPMA upstream-lineage principle:

```text
TWO PROVIDER BRANDS
MAY STILL REPRESENT
ONE MATERIAL UPSTREAM SOURCE
```

`ObservationCoverageSnapshot` therefore should reference:

- provider route identity;
- upstream lineage identity where known;
- Data Product identity/version;
- market/venue scope;
- completeness declaration;
- independent-source count;
- freshness;
- quality state;
- quota/entitlement state.

This prevents Falcon from overstating epistemic confidence merely because two APIs return the same upstream data.

---

# 25. Degraded Operation

FMOF defines degradation as loss of knowledge or analysis capacity, not fictional certainty.

Examples:

## Provider outage

```text
Provider unavailable
-> ObservationCoverage degrades
-> affected artifacts expire/degrade
-> alternate certified route may be requested through FSAPMA
-> canonical market instrument is not deleted merely because the provider disappeared
```

## Free quota exhausted

```text
Quota exhausted
-> route unavailable / deferred according to FSAPMA
-> wide/deep analysis cadence may reduce
-> coverage state records the gap
-> no hidden paid call
-> no fabricated freshness
```

## Broker capability stale

```text
BrokerCapabilityProjection expired
-> Actionability becomes UNKNOWN / unavailable as appropriate
-> no optimistic execution eligibility
```

## Account context stale

```text
Account feasibility evidence stale
-> no new risk based on old buying-power/access assumptions
```

## Resource pressure

```text
Resource pressure
-> analysis breadth/cadence degrades explicitly
-> protection/reconciliation truth remains protected
-> no stale opportunity is upgraded to fresh merely to keep throughput high
```

---

# 26. Proposed Core Types

The following are candidate semantic types if the Owner accepts this direction:

```text
CanonicalMarketUniverseSnapshotId
InstrumentIdentityGraphVersion
EconomicInstrumentId
MarketInstrumentId
ProviderRepresentationId
BrokerRepresentationId
ObservationCoverageSnapshotId
MarketDiscoverySnapshotId
DeepAnalysisCandidateId
AdaptiveDeepAnalysisPlanId
OpportunityConsiderationRecordId
GlobalOpportunityArtifactId
BrokerCapabilityFingerprint
BrokerCapabilityProjectionId
AccountActionabilityProjectionId
StrategyEligibleOpportunitySnapshotId
```

The final type catalog must determine which identifiers already exist and should be extended rather than duplicated.

No duplicate canonical identity should be introduced merely because this Proposal uses a clearer conceptual name.

---

# 27. Proposed State Semantics

## 27.1 Global opportunity lifecycle

```text
DISCOVERED
-> ANALYSIS_QUEUED
-> ANALYZING
-> VALID
-> STALE
-> SUPERSEDED

Any state may enter:
INSUFFICIENT_DATA
CONFLICTED
INVALIDATED
```

## 27.2 Account actionability lifecycle

```text
UNASSESSED
-> ASSESSING
-> ACTIONABLE_CANDIDATE
   |-> CONDITIONALLY_ACTIONABLE
   |-> NOT_ACTIONABLE
   |-> UNKNOWN
   |-> CONFLICTED
-> EXPIRED / SUPERSEDED
```

None of these states means Risk-approved, Capital-reserved or Executable.

---

# 28. Strategy Compatibility

Existing strategy logic should not be rewritten merely to adopt FMOF.

Instead, provide an explicit compatibility projection:

`StrategyEligibleOpportunitySnapshot`

This snapshot replaces the semantic role currently played by the overloaded `QualifiedUniverseSnapshot` at the strategy gate.

It represents:

```text
GLOBAL OPPORTUNITY VALID
+
REQUIRED DATA VALID
+
RELEVANT BROKER/ACCOUNT STRUCTURAL ACTIONABILITY
+
STRATEGY MARKET/TIMEFRAME APPLICABILITY
```

It still does not grant Risk/Capital/Execution authority.

This allows most strategy formulas in `17/17A/17B/17C` to remain intact.

---

# 29. MarketCapitalFitness Compatibility

Current `07D` uses Top-10 qualified-universe concepts in market-capital fitness.

If FMOF is accepted, this must be changed because market fitness cannot depend on an arbitrary fixed K.

Recommended successor inputs:

```text
MarketOpportunityBreadth
MarketOpportunityDepth
MarketObservationQuality
MarketExecutionEnvironmentQuality
MarketUncertaintyPenalty
ValidatedStrategyOpportunityDensity
```

These should be computed from versioned distributional summaries, not “best 10” only.

Capital allocation remains a separate downstream control and must not feed private-account capital back into global universe existence.

---

# 30. Privacy and Information Boundary

FMOF proposes a strict data-minimization boundary for shared analysis.

Shared global artifacts SHALL NOT require:

```text
UserId
AccountId
CashBalance
Holdings
Private Risk Limits
Private P&L
Credential References
```

Per-account projections may reference required private state under Trading's existing security/persistence rules.

Global scheduler telemetry may use aggregate demand counts or opaque cohort identifiers where useful, but it must not expose another client's holdings or financial condition to shared decision artifacts.

---

# 31. Provenance Requirements

Every material FMOF artifact should participate in the current immutable provenance architecture.

A `GlobalOpportunityArtifact` must be reconstructable to at least:

```text
MarketProfileVersion
-> CanonicalMarketUniverseSnapshot
-> InstrumentIdentityGraphVersion
-> ObservationCoverageSnapshot
-> Data Product identities / digests
-> Feature/algorithm/model versions
-> AdaptiveDeepAnalysisPlan
-> Analysis policy version
-> Resource-state evidence used for work selection
-> Result / expiry / supersession
```

An `AccountActionabilityProjection` additionally binds:

```text
GlobalOpportunityArtifact
-> BrokerCapabilityProjection / Fingerprint
-> AccountContext evidence
-> actionability rule version
```

A final trade continues through existing provenance:

```text
Strategy evaluation
-> TradeProposal
-> RiskDecision
-> CapitalReservation
-> ExecutionIntent / OrderAttempt
-> Broker evidence
-> Fill / Position / Capital reconciliation
```

The provenance graph remains an index/reference model and does not become a second business-state owner.

---

# 32. Ownership Matrix

| Subject | Proposed Owner / Authority Boundary |
|---|---|
| Provider acquisition, quota, entitlement, quality, provider symbol mapping | `APP-PMA / FSAPMA` |
| Canonical Trading market universe and opportunity semantics | `APP-TRD / T-LSA-02` |
| Market/account operating context | `APP-TRD / T-LSA-01` |
| Features | existing Trading feature owner |
| Strategy catalog/evaluation | existing Trading Strategy domain |
| Unified Risk | `T-LSA-07` |
| Capital/portfolio/reservation | `T-LSA-08` |
| Broker capability/execution/reconciliation | `T-LSA-09` |
| Guardian protection | `APP-GRD` under current governed authority |
| Aggregate FSATS resource coordination | current FSARM candidate/current governed resource architecture, without changing Foundation authority |
| Foundation resource truth/grants | Falcon Foundation |
| External provider egress | future governed Foundation Stage 12 / FCR-0013 boundary |
| External Broker egress | future governed Foundation Stage 12 / FCR-0014 boundary |
| Awareness research egress | remains separately governed/unresolved according to current Awareness/FCR design |

Critical invariant:

```text
T-LSA-02 MAY CONSUME BROKER / ACCOUNT PROJECTIONS.
T-LSA-02 DOES NOT BECOME OWNER OF BROKER OR ACCOUNT TRUTH.
```

---

# 33. Why This Scales Better Than Top-100

A Top-100 redesign scales linearly in the wrong dimension: it merely makes a bigger fixed list.

FMOF scales by separating shared and private work:

```text
MARKET-WIDE CHEAP DISCOVERY      = SHARED
EXPENSIVE GLOBAL DEEP ANALYSIS   = SHARED WHEN INPUTS MATCH
BROKER CAPABILITY PROJECTION     = SHARED BY EXACT SEMANTIC COHORT
ACCOUNT ACTIONABILITY            = PRIVATE AND CHEAP
RISK / CAPITAL / EXECUTION       = ACCOUNT-SPECIFIC
```

For 1,000 users, Falcon should not perform 1,000 independent scans of the same unchanged US-equity market snapshot.

The target scaling behavior is closer to:

```text
MARKET WORK
+
BROKER-COHORT WORK
+
PER-ACCOUNT PROJECTION WORK
```

rather than:

```text
FULL MARKET WORK * NUMBER OF ACCOUNTS
```

---

# 34. Red-Team Attack Set for This Proposal

This is a proposal-level adversarial design review, not an official R7/R8 Red-Team PASS.

The following attacks must be explicitly covered if the design is adopted.

## RT-FMOF-001: Broker A cannot trade X, so X disappears globally

Defense: Broker capability projection is downstream from canonical universe/global opportunity.

## RT-FMOF-002: $1,000 share automatically excluded from small account

Defense: nominal price is not global feasibility; fractional/minimum/risk/capital are projected downstream.

## RT-FMOF-003: `fractionable=true` interpreted as executable

Defense: Broker/account/session/order/minimum/Risk/Capital gates still required.

## RT-FMOF-004: Free IEX-only data represented as complete US market truth

Defense: ObservationCoverage explicitly records coverage source/scope/completeness.

## RT-FMOF-005: Provider outage deletes instrument

Defense: provider coverage affects observation state, not canonical existence by implication.

## RT-FMOF-006: Top-scoring incumbents monopolize deep analysis

Defense: exploration + coverage-debt lanes and consideration ledger.

## RT-FMOF-007: Fairness forces trades

Defense: fairness applies only to analysis consideration, never trade/capital allocation.

## RT-FMOF-008: One Broker cohort consumes all expensive analysis

Defense: global work is Broker-neutral; cohort-specific work is separated and scheduler can enforce cohort service policy without distorting global score.

## RT-FMOF-009: Same Broker name hides different capability entitlements

Defense: cache key is `BrokerCapabilityFingerprint`, not brand.

## RT-FMOF-010: Account private holdings influence global score

Defense: global artifacts reject client-private financial state.

## RT-FMOF-011: Stale account feasibility reused

Defense: explicit AsOf/expiry and UNKNOWN state.

## RT-FMOF-012: `UNKNOWN` Broker support treated as false

Defense: typed UNKNOWN remains separate; it does not delete global opportunity.

## RT-FMOF-013: `UNKNOWN` Broker support treated as true

Defense: UNKNOWN cannot become action permission.

## RT-FMOF-014: Symbol reused for another company

Defense: stable identity graph and evidence-backed lifecycle mapping.

## RT-FMOF-015: Dual listing / ADR incorrectly merged

Defense: EconomicInstrument and MarketInstrument/representation layers remain explicit; uncertain equivalence fails closed.

## RT-FMOF-016: Crypto 24/7 score directly compared with equity session score

Defense: market-native scoring first, explicit normalized AnalysisUtility only for resource comparison.

## RT-FMOF-017: Quota pressure silently keeps using stale data

Defense: observation freshness/coverage state expires; reduced capacity does not upgrade stale evidence.

## RT-FMOF-018: Paid provider used automatically when free quota ends

Defense: cost class and paid-route authority are explicit; unauthorized paid route is unavailable.

## RT-FMOF-019: Global analysis reused after model/input version changes

Defense: reuse key binds exact data digest and algorithm/profile versions.

## RT-FMOF-020: Cross-Broker failover duplicates an ambiguous order

Defense: FMOF does not authorize cross-Broker smart routing/failover; current reconciliation-before-retry semantics remain.

## RT-FMOF-021: Resource scheduler mistakes analysis priority for expected profit

Defense: `AnalysisUtilityScore` and `MarketNativeOpportunityScore` are separate typed semantics.

## RT-FMOF-022: Discovery result bypasses strategy/Risk

Defense: discovery/deep-analysis artifacts have no execution authority; current T06 -> T07 -> T08 -> T09 path remains.

## RT-FMOF-023: One free provider's plan changes overnight

Defense: FSAPMA point-in-time certification/entitlement version changes ObservationCoverage; architecture remains stable.

## RT-FMOF-024: Account balance change contaminates global cached opportunity

Defense: balance is absent from global artifact and belongs to private downstream projection/Risk/Capital.

## RT-FMOF-025: Coverage debt resurrects invalid instrument

Defense: fairness service applies only after hard identity/data/discovery eligibility; hard invalidity dominates debt.

No current evidence found requires abandoning this architecture because of these attacks, but formal Architecture/Consistency and fresh Red-Team review remain mandatory after any accepted semantic materialization.

---

# 35. Proposed Migration / Compatibility Strategy

If the Owner accepts FMOF, do not perform a destructive all-at-once semantic rename without reconciliation.

Recommended transition:

```text
CURRENT
QualifiedUniverseSnapshot

BECOMES AN EXPLICIT DERIVED COMPATIBILITY VIEW DURING MIGRATION

SOURCE CHAIN:
CanonicalMarketUniverseSnapshot
-> ObservationCoverageSnapshot
-> MarketDiscoverySnapshot
-> AdaptiveDeepAnalysisPlan
-> GlobalOpportunityArtifact
-> BrokerCapabilityProjection
-> AccountActionabilityProjection
-> StrategyEligibleOpportunitySnapshot
```

Once all downstream consumers use the new exact semantics, the old overloaded snapshot identity may be superseded through normal documentary governance.

Historical R7 artifacts remain preserved.

---

# 36. Expected File Impact If Owner Accepts

This Proposal itself changes none of these files.

If accepted, the semantic integration is expected to require coordinated updates/reconciliation at least in:

```text
04_CANONICAL_DOMAIN_TYPE_CATALOG.md
07_TRADING_APPLICATION_13_LSA_SPECIALIZED_ARCHITECTURE.md
07A_BROKER_NEUTRAL_ADAPTIVE_UNIVERSE_REDESIGN_PROPOSAL.md
07D_MARKET_CAPITAL_FITNESS_AND_DYNAMIC_ALLOCATION_SPEC.md
07E_UNIVERSE_RANKING_EXACT_FORMULA_SPEC.md
08A_INITIAL_CANONICAL_DATA_PRODUCT_AND_QUALITY_PROFILE.md
12_CROSS_APPLICATION_CONTRACT_SCHEMA_AND_ROUTE_CATALOG.md        if contract payload shapes require it
13_CANONICAL_STATE_MACHINE_CATALOG.md
14_PERSISTENCE_TRANSACTION_AND_CONCURRENCY_SPEC.md
15_RUNTIME_SCHEDULING_QUEUE_AND_BACKPRESSURE_SPEC.md
16_MARKET_PROVIDER_BROKER_AND_EXECUTION_PROFILE_SPEC.md
17_STRATEGY_AND_INTELLIGENCE_EXACT_SPECIFICATIONS.md
18_AWARENESS_CSA_MONITOR_AND_SELF_DEVELOPMENT_SPEC.md
19A_IMMUTABLE_AUDIT_PROVENANCE_GRAPH_SPEC.md
20_TRACEABILITY_VERIFICATION_AND_CODEX_IMPLEMENTATION_CONTRACT.md
```

The integration review must decide whether each file needs semantic changes or only compatibility references. No update should be made merely because it appears in this impact list.

---

# 37. Explicit Non-Changes

This proposal does **not** propose changing:

- Falcon Vision;
- Falcon Constitution;
- APP-001;
- CON-023;
- ADR-I012;
- ADR-I015;
- FSAPMA's status as sole operational provider-data gateway;
- Foundation ownership of total resource governance;
- current Guardian separation;
- Unified Risk ownership;
- Capital Reservation ownership;
- broker reconciliation-before-retry safety;
- FSTSimA non-Live boundary;
- current Awareness hierarchy;
- current FSA Foundation ownership;
- current unresolved Awareness research-egress governance by implication;
- current initial funded 1:1 Risk model;
- current strategy catalog merely for the sake of this redesign;
- current market authorization merely because the architecture becomes N-market capable.

---

# 38. Architecture Quality Tests

A final accepted FMOF semantic design should satisfy all of the following:

```text
TEST 01: Add a new Broker without changing global market universe semantics.
TEST 02: Remove a Broker without deleting opportunities available through other Brokers.
TEST 03: Add a new user/account without rescanning the entire unchanged market.
TEST 04: Change account balance without invalidating global opportunity artifacts.
TEST 05: Change fractional support without rewriting market truth.
TEST 06: Provider outage degrades observation truth, not economic existence truth.
TEST 07: Free-tier quota exhaustion cannot trigger hidden paid usage.
TEST 08: High nominal price cannot alone exclude a fractionally feasible instrument.
TEST 09: Low nominal price cannot alone admit a weak/infeasible instrument.
TEST 10: Emerging candidate eventually receives consideration without forced trade.
TEST 11: Market-native scores cannot be compared cross-market without explicit normalization.
TEST 12: UNKNOWN never becomes permission.
TEST 13: Shared artifact contains no account-private financial state.
TEST 14: Same Broker brand with different entitlements cannot share unsafe cache.
TEST 15: Symbol change/reuse cannot silently alias two economic instruments.
TEST 16: Current strategy/Risk/Capital/Execution authority chain remains intact.
TEST 17: FMOF remains usable with only free certified providers, but truthfully degrades when their limits prevent sufficient observation.
TEST 18: Adding a future MarketProfile does not require Foundation redesign.
```

---

# 39. Recommended Owner Decision

Recommended direction:

```text
ACCEPT FMOF AS THE DESIGN DIRECTION
FOR REPLACING PRICE-ZONE / FIXED-TOP-K UNIVERSE SEMANTICS,
SUBJECT TO GOVERNED SEMANTIC MATERIALIZATION AND FRESH REVIEW.
```

This is **not** a request to accept exact implementation code, runtime behavior or every future numeric scheduling parameter.

The proposed Owner decision is architectural:

1. accept separation of the six truth planes;
2. accept Broker-neutral / account-neutral global market opportunity semantics;
3. accept first-class Observation Coverage Truth;
4. accept durable instrument identity mapping instead of symbol identity;
5. accept no fixed-K semantic universe;
6. accept adaptive deep-analysis scheduling with explicit anti-starvation/coverage evidence;
7. accept global analysis reuse plus Broker-cohort and private account projections;
8. accept that fractional capability is downstream feasibility, not a price-zone substitute;
9. accept market-native scoring plus explicit cross-market normalization;
10. accept Free-First but never Free-Blind provider semantics;
11. preserve current Risk/Capital/Guardian/FSAPMA/Foundation authority boundaries.

---

# 40. Required Governance Lifecycle After Owner Direction

If the Owner accepts the direction, the correct next sequence is:

```text
OWNER ACCEPTS FMOF DESIGN DIRECTION
->
MATERIALIZE EXACT SEMANTIC CHANGES IN A NEW CANDIDATE VERSION
->
RECONCILE ALL AFFECTED FILES
->
VERIFY NO UNINTENDED FILE / AUTHORITY / CONTRACT DRIFT
->
CREATE NEW SEMANTIC FREEZE
->
FRESH ARCHITECTURE / CONSISTENCY REVIEW
->
FRESH RED-TEAM REVIEW
->
REMEDIATE AND REPEAT IF ANY SEMANTIC FINDING EXISTS
->
RETURN THE EXACT REVIEWED VERSION TO OWNER
->
OWNER FINAL ACCEPTANCE OR CHANGE REQUEST
```

R7 A/C and Red-Team PASS remain historical valid evidence for the exact R7 freeze only. They must not be presented as PASS evidence for FMOF-modified semantics.

---

# 41. Final Proposal Position

The target architecture should not ask:

> Which ten or hundred instruments should Falcon allow this account to see?

It should ask six separate questions:

```text
1. WHAT INSTRUMENTS DOES FALCON HAVE EVIDENCE EXIST IN THIS MARKET SCOPE?
2. WHAT CAN FALCON CURRENTLY OBSERVE, HOW WELL, AND WITH WHAT COVERAGE GAPS?
3. WHICH CANDIDATES DESERVE CHEAP OR EXPENSIVE ANALYTICAL ATTENTION NOW?
4. WHICH GLOBAL OPPORTUNITIES DOES THIS BROKER CAPABILITY SET SUPPORT?
5. WHICH OF THOSE ARE STRUCTURALLY ACTIONABLE FOR THIS ACCOUNT?
6. WHICH, IF ANY, SURVIVE STRATEGY, RISK, CAPITAL, GUARDIAN AND EXECUTION AUTHORITY?
```

That separation is the core of FMOF.

It removes the structural disadvantage to small accounts caused by nominal-price zones, prevents one Broker or provider from becoming the definition of the market, avoids fixed-list blindness, makes resource scarcity visible instead of epistemically destructive, and allows Falcon to reuse intelligence efficiently across many users while keeping private financial state private and final authority account-specific.

The proposed architecture is intentionally designed so that adding more users, Brokers or future Markets increases projections and profiles rather than forcing a redesign of Falcon's market-opportunity concept.

```text
BROAD MARKET AWARENESS
+
HONEST OBSERVABILITY
+
ADAPTIVE ATTENTION
+
BROKER-NEUTRAL INTELLIGENCE
+
ACCOUNT-SPECIFIC FEASIBILITY
+
UNCHANGED HARD RISK / CAPITAL / EXECUTION AUTHORITY
=
FALCON MARKET OPPORTUNITY FABRIC
```

**Proposal State:** `READY_FOR_PROJECT_OWNER_REVIEW / NOT_OWNER_ACCEPTED / NOT_CLOSED`  
**Implementation Authority:** `NOT_GRANTED`  
**Runtime Authority:** `NOT_GRANTED`
