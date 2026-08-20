# FSATS SIA — Broker-Neutral Adaptive Market Universe Redesign Proposal

**Package:** `FSATS-SIA-v0.1`  
**Proposal:** `07A`  
**Affected Application:** `APP-TRD`  
**Primary affected branch:** `T-LSA-02 — Market & Instrument Universe`  
**Status:** `DESIGN_CHANGE_PROPOSAL / OUTSIDE_CURRENT_R7_FREEZE / NOT_OWNER_ACCEPTED / NOT_CLOSED`  
**Development-change classification:** `DCC-3 — MATERIAL_DOMAIN_CHANGE`  
**Date:** `2026-08-12`  

## 1. Purpose

This proposal redesigns the current US-equities universe-selection model so Falcon can serve clients fairly across different portfolio sizes, brokers, account capabilities, fractional-share support, permissions and execution constraints.

It specifically evaluates and proposes replacement of the current design in `07_TRADING_APPLICATION_13_LSA_SPECIALIZED_ARCHITECTURE.md` that uses capital-based price zones and fixed Top-10 selections.

This file is deliberately separate from file `07`. It does not silently rewrite the current R7 semantic freeze. If the Project Owner chooses this direction, the affected current SIA files must be changed explicitly and then undergo a fresh semantic freeze, fresh Architecture/Consistency review and fresh Red-Team review before Owner acceptance.

This proposal grants no implementation, runtime, provider, broker, Paper, Shadow, Tiny Live, Live, deployment, autonomous promotion or financial authority.

## 2. Problem Being Corrected

The current T-LSA-02 candidate uses this basic shape:

- small deployable capital is restricted toward lower nominal share prices;
- larger capital unlocks additional price bands;
- each tier selects a fixed Top-10 set;
- fractional-share capability can widen feasibility but does not remove the price-zone structure.

That design creates several architectural problems in a multi-client, multi-broker Falcon deployment.

First, nominal share price is not a reliable global proxy for account affordability when fractional trading is available. A small account may be able to obtain a risk-sized fractional position in a high-priced instrument, while another account or broker may require whole shares.

Second, broker capability is not uniform. One broker may support an instrument, fractional quantity, a session or an order type that another broker does not support. Therefore a universe selected from one broker's capabilities must not become the universe for all clients.

Third, a fixed Top-10 list can starve clients whose broker cannot trade several members of that list. Replacing Top-10 with one fixed Top-100 list reduces but does not solve the architectural problem.

Fourth, requiring every globally selected instrument to be executable at every supported broker would create a lowest-common-denominator universe. This would discard valid opportunities available to some clients merely because another broker does not support them.

Fifth, some ranking inputs in the current candidate mix market-level quality with account-, broker- or portfolio-specific feasibility. Those concerns should be separated so a global market opportunity is not confused with a particular client's ability or authority to act on it.

## 3. Governing Architectural Basis

This redesign preserves the current Falcon separation of responsibility.

`APP-TRD` remains the owner of Trading business semantics. `FSAPMA` remains the operational market/provider-data gateway. Foundation remains Application-neutral and does not interpret Trading business meaning. Broker execution and account state remain governed through the existing Trading execution/account boundaries and future Foundation egress/credential capabilities where required.

The proposal is consistent with the following current architecture principles:

- Application business logic remains Application-owned;
- undeclared routes, permissions, authority and dependencies remain denied;
- technical reachability does not create Trading authority;
- market analysis does not create execution authority;
- account, broker, risk and execution constraints remain explicit;
- resource pressure may reduce analysis breadth or depth but may not weaken safety, data, authority or risk gates;
- immutable versioned evidence and snapshots remain required for reconstructability.

## 4. Historical Review Result

The preserved V1.3 reference remains historical design input rather than current authority.

The current `02_V13_AND_ARCHIVE_RETAIN_ADAPT_SUPERSEDE_MATRIX.md` explicitly retains the initial US Equities market, Crypto Spot, the initial funded 1:1 exposure model, market-specific operating profiles and other Trading concepts. It does not identify fixed capital-price zones or fixed Top-10 universe selection as a historical concept that must be retained.

Therefore this proposal does not treat the current Zone/Top-10 realization as protected historical intent. It may be superseded by a stronger current design while preserving the larger Falcon intent: scan the supported market intelligently, find high-quality opportunities, protect capital, operate within broker/account capability and scale across clients.

The original 289-entry V1.3 source package is not fully mirrored as source bodies in the current historical-reference branch. This review therefore does not claim a fresh reread of all 289 original bodies. It relies on the preserved V1.3 status record, the current retain/adapt/supersede matrix and the current SIA candidate.

## 5. External Research Findings

Official brokerage/regulatory material reviewed on 2026-08-12 supports the architectural concern.

FINRA's current investor guidance states that fractional-share availability and handling differ by brokerage firm. Some firms provide a broad range of fractional securities while others provide a limited range or none. FINRA also notes that fractional shares can make higher-priced securities accessible to smaller accounts, but execution and trading-session behavior can differ.

Alpaca documents instrument-level fractional eligibility through asset capability information such as `fractionable`; not every asset is necessarily fractionable. Alpaca also exposes instrument tradability/capability information that must be evaluated rather than assumed globally.

Interactive Brokers currently advertises fractional access across thousands of eligible stocks and ETFs, demonstrating that nominal full-share price alone is not an adequate universal small-account gate.

Dynamic-universe design literature and platform practice also support separating broad market eligibility from dynamically selected analysis candidates. Dynamic selection reduces dependence on a permanently hand-picked security list and allows the active analysis set to follow current evidence.

These external sources are design evidence only. They do not create Falcon authority and do not define Falcon's final broker contracts or runtime behavior.

## 6. Proposed Decision

Falcon SHOULD replace fixed capital-price Zones and fixed Top-K universe membership with a **Broker-Neutral Adaptive Market Universe Architecture**.

The key rule is:

**Falcon first determines what is a valid market opportunity independently of any one client's broker or cash balance, then projects that opportunity through each client's actual broker, account, risk and capital constraints before it can become actionable for that client.**

No single broker defines the global market universe.

No smallest account defines the global market universe.

No richest account defines the global market universe.

No fixed number such as 10 or 100 defines the semantic universe.

A numerical analysis budget may exist operationally, but it is a dynamic resource/scheduling outcome, not the definition of which instruments Falcon considers to exist or potentially matter.

## 7. Proposed Architecture

```text
SUPPORTED MARKET INSTRUMENTS
          |
          v
CANONICAL MARKET UNIVERSE
          |
          v
GLOBAL MARKET ELIGIBILITY
          |
          v
CHEAP / WIDE GLOBAL OPPORTUNITY SCAN
          |
          v
GLOBAL OPPORTUNITY SNAPSHOT
          |
          +-------------------------------+
          |                               |
          |                        SHARED DEEP-ANALYSIS
          |                        PRIORITY / COVERAGE PLAN
          |                               |
          v                               v
BROKER CAPABILITY PROJECTION      EXPENSIVE MARKET ANALYSIS
          |                               |
          +---------------+---------------+
                          |
ACCOUNT OPERATING /      v
CAPITAL / PERMISSION -> CLIENT ACTIONABLE UNIVERSE
CONTEXT                   |
                          v
                 STRATEGY EVALUATION
                          |
                          v
                 STRATEGY ORCHESTRATION
                          |
                          v
                       RISK
                          |
                          v
                       CAPITAL
                          |
                          v
                      EXECUTION
```

The global layers are reusable across clients. Broker capability is reusable across accounts sharing the same certified broker capability profile where the semantics are identical. Account feasibility remains account-specific.

## 8. Layer 1 — Canonical Market Universe

`CanonicalMarketUniverse` is the broad market scope Falcon is prepared to identify, observe and reason about for a supported market profile.

For US Equities, it is not defined as "the ten best stocks" or "the hundred best stocks." It may contain hundreds or thousands of instruments depending on the supported market definition, available governed Data Products, resource policy and instrument-identity quality.

An instrument belongs here only under a canonical `InstrumentId` and versioned market/listing identity. Ticker text alone is not sufficient identity.

The canonical universe is broker-neutral. Broker A not offering an instrument does not delete that instrument from the market for Broker B's clients.

## 9. Layer 2 — Global Market Eligibility

`GlobalMarketEligibility` applies conditions that are genuinely market/instrument-level and independent of a particular customer's account.

Examples include:

- canonical instrument identity is known and non-conflicted;
- supported market/listing state is valid;
- required operational Data Products exist and meet the required quality/freshness floor;
- unresolved corporate-action identity conflicts do not exist;
- market-level suspension or prohibition is respected;
- minimum market-quality/liquidity conditions required for Falcon analysis are satisfied;
- at least one currently supported strategy family can meaningfully evaluate the instrument when that is required by the market profile.

Failure of a broker-specific capability SHALL NOT make an instrument globally ineligible.

Failure of one account's affordability SHALL NOT make an instrument globally ineligible.

## 10. Layer 3 — Cheap/Wide Global Opportunity Scan

Falcon SHOULD scan the globally eligible universe using a relatively inexpensive first-stage discovery process.

The goal is breadth, not final Trading judgment.

This stage identifies where current evidence suggests analysis resources may be valuable. It can use market-level signals such as participation, volume behavior, market liquidity, useful volatility, broad opportunity-density indicators, data quality and regime-sensitive discovery signals.

It SHALL NOT emit execution authority, Risk approval or a final Trade Proposal.

This wide scan should be shared across clients whenever the input market evidence is identical. Falcon should not repeat the same broad market computation once per customer.

## 11. Layer 4 — Global Opportunity Snapshot

The wide scan produces an immutable, versioned `GlobalOpportunitySnapshot`.

It SHOULD contain enough information to explain why an instrument deserves more or less analysis priority without claiming that every client can trade it.

A global opportunity state therefore answers:

**"Is this instrument currently worth Falcon's market-analysis attention?"**

It does not answer:

**"Can client X execute this instrument through broker Y now?"**

Those are intentionally different questions.

## 12. Layer 5 — Broker Capability Projection

Each broker/instrument combination requires a versioned capability projection before the instrument can become actionable through that broker.

The projection may include, where relevant:

- instrument available/not available/unknown;
- tradable/not tradable/unknown;
- whole-share only or fractional capability;
- minimum quantity and/or minimum notional;
- price and quantity precision;
- supported order types;
- supported market sessions and extended-hours behavior;
- short capability if a future authorized strategy requires it;
- execution restrictions;
- broker-specific fees or cost inputs where available;
- capability source, version, freshness and evidence;
- uncertainty and fail-closed behavior.

This proposal does not give T-LSA-02 a competing source of broker truth.

Authoritative broker execution capability remains owned by the appropriate Trading execution boundary, currently centered on `T-LSA-09.BrokerCapabilityResolver` and governed broker adapters. T-LSA-02 consumes the resulting versioned broker capability projection for universe/actionability purposes.

Actual external broker connectivity and credentials remain dependent on the separately governed Foundation capability path, including FCR-0014. This proposal creates no broker egress.

## 13. Layer 6 — Account Feasibility Projection

A second projection determines whether an otherwise valid instrument is feasible for the specific Trading account.

Inputs include, where applicable:

- `TradingAccountId` and broker-account mapping;
- account permissions and jurisdictional limitations;
- current environment classification and separate runtime authority;
- reconciled buying power/effective deployable capital;
- currency and settlement constraints;
- fractional permission or account-level fractional restriction;
- minimum notional/quantity interaction;
- current positions and capital reservations;
- account-specific Risk envelope;
- applicable Guardian restrictions;
- account freshness/readiness state.

Account capability and operating context remain owned by the existing T-LSA-01/T-LSA-07/T-LSA-08/T-LSA-09 boundaries as appropriate. T-LSA-02 consumes their governed projections rather than becoming the authoritative owner of account, Risk, capital or broker state.

## 14. Layer 7 — Client Actionable Universe

For each account, Falcon derives a `ClientActionableUniverseSnapshot` from the intersection of:

- current globally eligible instruments;
- current market opportunity evidence or analysis eligibility;
- broker capability for that account's broker;
- account feasibility;
- applicable Guardian/protection state;
- required lifecycle/authority readiness for the intended action class.

This is not a permanent client watchlist. It is a versioned projection of what Falcon can legitimately continue evaluating or potentially act on for that client under current evidence.

The following invariants apply:

`BROKER_A_UNAVAILABLE != GLOBAL_UNAVAILABLE`

`HIGH_NOMINAL_SHARE_PRICE != SMALL_ACCOUNT_INELIGIBLE`

`FRACTIONABLE != AUTOMATICALLY_ACTIONABLE`

`GLOBAL_OPPORTUNITY != CLIENT_ACTIONABLE`

`CLIENT_ACTIONABLE != TRADE_RECOMMENDATION`

`MARKET_ANALYSIS != AUTHORITY_TO_TRADE`

`TOP_K != UNIVERSE_DEFINITION`

Unknown material broker/account capability fails closed for that client/action. It does not silently delete the instrument from other clients' universes.

## 15. Fractional-Share Semantics

Fractional-share capability is a feasibility capability, not a global quality signal.

Example:

A client has USD 300 deployable capital. An instrument trades at USD 600. The client's broker supports fractional trading for the exact instrument, the account has the required permission, the minimum notional is compatible, Risk allows only a USD 30 exposure and execution-cost constraints are acceptable.

The nominal USD 600 share price SHALL NOT globally exclude the instrument merely because the account cannot buy one whole share.

For another client whose broker permits only whole shares in that instrument, the same opportunity may be infeasible for new exposure.

Conversely, a USD 5 stock SHALL NOT become preferred merely because it is cheap. Poor liquidity, unacceptable spread, bad data quality, excessive execution uncertainty, unsupported strategy applicability or Risk constraints can still reject it.

Therefore nominal share price is removed as a global Zone gate. Price remains an input wherever actual quantity/notional feasibility requires it.

## 16. Multi-Broker Fairness Without Lowest-Common-Denominator Selection

Falcon SHOULD NOT build the global universe by intersecting only the instruments supported by every connected broker.

That would let the least capable broker constrain all other clients.

Falcon SHOULD instead retain the union of valid supported-market opportunities, then derive broker/account-specific actionability.

However, a purely global priority queue could still spend all expensive-analysis resources on opportunities accessible to only one broker cohort. To prevent systematic client starvation, the deep-analysis scheduler SHOULD be **coverage-aware**.

Coverage-aware scheduling means it considers the active broker/account capability cohorts represented in the system and ensures that high-quality candidate demand from each eligible cohort receives due consideration, while still prioritizing opportunity quality, safety, freshness and resource efficiency.

It does not guarantee an artificial equal number of opportunities to each customer. Market conditions and broker capabilities can legitimately produce different numbers of actionable opportunities.

The fairness requirement is **fair access to consideration under the same governed quality rules**, not fabricated equality of trade count.

## 17. Adaptive Deep-Analysis Budget Instead of Fixed Top-10/Top-100

Falcon SHOULD NOT encode 10, 100 or another fixed count as the semantic definition of the selected universe.

The number of instruments receiving expensive analysis in a cycle is an operational scheduling result derived from:

- current opportunity distribution;
- required freshness/deadlines;
- data-product availability and cost;
- computational cost of the required features/strategies;
- current FSARM/Foundation resource state;
- active client/broker coverage demand;
- maximum acceptable decision latency;
- existing cached/shared analysis;
- confidence that additional breadth has positive expected analytical value.

A quiet market may justify deep analysis of fewer than 100 instruments.

A highly active market with sufficient resources may justify more than 100.

If resources are constrained, Falcon reduces analysis breadth or depth according to governed priorities. It SHALL NOT weaken hard Data, Risk, Guardian, broker, authority or execution gates to maintain a target count.

## 18. Shared Analysis and Scaling Across Clients

The architecture SHOULD avoid one full market scan per customer.

Market-level computations are shared when they depend on the same immutable market evidence and model/version inputs.

Broker capability projections may be shared by accounts that truly share the same broker capability semantics/version.

Account feasibility remains account-specific.

A scalable planning model is:

```text
GLOBAL MARKET SCAN ONCE
        |
        v
GLOBAL CANDIDATES / OPPORTUNITIES
        |
        +--> BROKER COHORT A DEMAND --+
        +--> BROKER COHORT B DEMAND --+--> UNION / DEDUPLICATE ANALYSIS DEMAND
        +--> BROKER COHORT C DEMAND --+             |
                                                   v
                                      SHARED EXPENSIVE ANALYSIS
                                                   |
                         +-------------------------+-------------------------+
                         v                         v                         v
                    ACCOUNT A                 ACCOUNT B                 ACCOUNT C
                   ACTIONABILITY             ACTIONABILITY             ACTIONABILITY
```

This allows Falcon to scale to many clients without making broker differences disappear or multiplying identical market work unnecessarily.

## 19. Scoring Redesign

The current T-LSA-02 rank combines useful market factors, but the redesigned architecture SHOULD separate scores by semantic ownership.

### 19.1 Global Market Quality / Opportunity Priority

This layer may use market-level inputs such as:

- liquidity quality;
- participation/volume behavior;
- Data Product quality;
- useful/tradable volatility characteristics;
- market-level spread/microstructure where the evidence is broker-neutral;
- broad opportunity-density signals;
- market/regime applicability.

This score prioritizes attention. It does not determine account feasibility or execution authority.

### 19.2 Broker Execution Feasibility

Broker-specific considerations include:

- exact instrument availability;
- fractional/whole-share capability;
- minimum quantity/notional;
- supported order type and session;
- route-specific constraints;
- broker-specific expected execution cost where applicable.

These SHALL NOT be encoded as if they were universal market facts.

### 19.3 Account Actionability

Account-specific considerations include:

- deployable capital;
- current buying power;
- permission/jurisdiction;
- Risk-compatible size;
- capital reservations;
- account-specific fractional permission and minimums;
- current restrictions/readiness.

### 19.4 Portfolio Fit

Diversification, concentration and correlation depend on the client's current portfolio and proposed exposure. They therefore belong primarily in the downstream Risk/portfolio-capital evaluation (`T-LSA-07`/`T-LSA-08`) rather than being treated as a universal static property of the instrument.

### 19.5 Weight Policy

This proposal does not freeze replacement weights such as `25/20/15/...`.

Exact formulas and weights should be versioned, testable and calibrated with simulation/evidence. Architecture should define semantic inputs, hard boundaries and deterministic/reproducible calculation requirements without prematurely claiming that one permanent weight set is optimal across market regimes.

## 20. Proposed T-LSA-02 Responsibility Shape

T-LSA-02 remains `Market & Instrument Universe`, but its semantics evolve.

Recommended responsibility set:

- `T02.MarketProfileRegistry` — retain;
- `T02.InstrumentMaster` — retain;
- `T02.GlobalUniverseEligibilityEngine` — replaces capital-zone eligibility semantics with market-level eligibility;
- `T02.GlobalOpportunityScanner` — cheap/wide broker-neutral market discovery;
- `T02.ActionabilityProjectionBuilder` — consumes governed broker/account projections without owning their authoritative state;
- `T02.AdaptiveAnalysisPlanner` — creates resource- and coverage-aware analysis demand;
- `T02.UniverseSnapshotStore` — retain and extend to store typed global/actionable/analysis-demand snapshots.

The existing `T02.TradabilityResolver` concept should be decomposed or renamed during adoption so "tradability" cannot ambiguously mix market-level eligibility, broker availability and account feasibility.

The existing `T02.UniverseRanker` may survive only if its responsibility is explicitly narrowed to a versioned market-attention priority. It must not silently combine broker/account/portfolio semantics.

Exact component names remain proposal-level until adoption into the current SIA candidate.

## 21. Interfaces With Neighboring LSAs

### T-LSA-01 — Operations, Account & Environment

Provides versioned account operating context, account/broker mapping and readiness/capability projections required to derive account actionability.

### T-LSA-03 — Analysis Frameworks

Consumes global/analysis-demand snapshots and produces reusable versioned features. Expensive features are computed only when justified by the analysis plan.

### T-LSA-04 / T-LSA-05 — Trading Schools

Evaluate only instruments that satisfy required analysis/applicability conditions. T-LSA-05's existing cheap-discovery/expensive-confirmation concept is aligned with this proposal and should be reused rather than duplicated.

### T-LSA-07 — Unified Risk

Owns account/portfolio Risk gating and sizing. T-LSA-02 may consume an eligibility projection where needed but must not own or bypass Risk authority.

### T-LSA-08 — Portfolio & Capital

Owns capital truth/reservations and portfolio state. Portfolio diversification/concentration decisions remain downstream here and in T-LSA-07.

### T-LSA-09 — Execution & Position Lifecycle

Owns authoritative broker execution capability resolution and execution reconciliation. It provides versioned broker/instrument capability projections to T-LSA-02 rather than allowing T-LSA-02 to invent broker truth.

### T-LSA-13 / FSARM

Provide the governed Application/resource state used to determine scan breadth, expensive-analysis capacity and degraded scheduling. Resource pressure changes work allocation, not safety or eligibility truth.

### FSAPMA

Continues to provide governed operational Data Products. T-LSA-02 does not become an external market-data provider.

## 22. Snapshot Model

The adopted design SHOULD distinguish at least the following immutable versioned snapshot meanings:

- `CanonicalMarketUniverseSnapshot`;
- `GlobalEligibilitySnapshot`;
- `GlobalOpportunitySnapshot`;
- `BrokerInstrumentCapabilitySnapshot` consumed from the broker/execution boundary;
- `TradingOperatingContextSnapshot` consumed from T-LSA-01;
- `ClientActionableUniverseSnapshot`;
- `AnalysisDemandSnapshot`;
- downstream `FeatureSnapshot` and strategy evidence.

Every material decision cycle pins exact snapshot identities so later broker changes, capital changes or market changes cannot retroactively alter what Falcon knew at decision time.

## 23. Fail-Closed Rules

The following conditions fail closed at their own scope:

- unknown/conflicted canonical identity: globally unavailable for governed analysis/action until resolved;
- required market Data Product unavailable or stale beyond policy: affected market/instrument analysis blocked;
- broker capability unknown for an action: affected broker/account action blocked, global opportunity retained where otherwise valid;
- fractional capability unknown when fractional sizing is required: fractional action blocked;
- account buying power or permission stale/unknown: new exposure blocked for that account;
- Guardian prohibition: affected action blocked according to exact scope;
- Risk denial: proposal cannot create new exposure;
- resource shortage: expensive analysis deferred/reduced, hard gates unchanged;
- stale actionability snapshot at execution: revalidation required or intent rejected.

## 24. Privacy and Client Isolation

Coverage-aware scheduling SHALL NOT leak one client's private account state to another client.

Shared analysis demand should use the minimum necessary cohort/demand metadata. Account buying power, positions, reservations, permissions and strategy outcomes remain isolated according to Application security and persistence rules.

A client must not be able to infer another client's holdings or capital merely because a shared market analysis was scheduled.

## 25. Illustrative Cases

### Case A — Small Account, Fractional-Capable Broker

A USD 300 account encounters a USD 600 instrument with strong market opportunity evidence. The broker supports fractional trading for that exact instrument and account, the minimum notional is compatible and downstream Risk permits a USD 30 position.

The instrument can remain actionable. It is not rejected because the full-share price exceeds the account value.

### Case B — Same Instrument, Whole-Share-Only Broker

Another USD 300 account uses a broker/account that does not support fractional trading for the instrument.

The instrument remains a valid global opportunity but is not actionable for new exposure in that account at that moment.

### Case C — Cheap but Poor-Quality Instrument

A USD 4 instrument has inadequate liquidity, unacceptable spread or unreliable Data Product quality.

Low nominal price does not rescue it. It fails the applicable market/data hard gates.

### Case D — Broker A Cannot Trade a Strong Opportunity

Broker A marks an instrument unavailable while Broker B supports it.

Broker A clients cannot act on it. Broker B clients may continue through account/Risk/capital evaluation. Falcon does not erase the instrument globally.

### Case E — Resource Pressure

The wide scan identifies 240 worthwhile candidates but available analysis resources can responsibly process only 85 before evidence freshness expires.

Falcon builds a coverage-aware priority plan for the 85. The number 85 is a runtime scheduling result, not a permanent universe definition. Remaining candidates are deferred or rescanned according to freshness and resource policy.

## 26. Verification Requirements If Adopted

Before this design can become accepted SIA, deterministic verification and adversarial fixtures should demonstrate at minimum:

1. a high-priced fractionable instrument can remain feasible for a small account when every required constraint permits it;
2. the same instrument can be infeasible for a whole-share-only account without becoming globally ineligible;
3. Broker A unavailability cannot remove a Broker B opportunity globally;
4. a broker capability `UNKNOWN` fails closed only for affected broker/account actionability;
5. low nominal share price cannot bypass liquidity/data/Risk gates;
6. global market scoring contains no hidden account buying-power dependency;
7. broker-specific execution cost does not masquerade as universal market truth;
8. portfolio diversification/concentration is not decided globally before the client portfolio is known;
9. resource pressure reduces scheduling breadth without weakening hard gates;
10. two clients sharing identical market evidence reuse shared analysis deterministically;
11. private account state is not exposed through shared analysis artifacts;
12. stale broker/account projections force revalidation before risk-increasing execution;
13. no fixed Top-K assumption is required for correctness;
14. identical pinned inputs and policy versions produce reconstructable deterministic snapshots;
15. strategy ranking/analysis cannot create Risk, capital or execution authority.

## 27. Supersession Map If Owner Chooses Adoption

If adopted after governed review, the new design should explicitly supersede these current T-LSA-02 candidate semantics:

- `ZONE_C / ZONE_B / ZONE_A` as capital/nominal-price universe gates;
- fixed `target selected count: 10` semantics;
- cumulative fixed Top-10 tier sets;
- any implication that one globally ranked set must be executable for every client;
- any global ranking input that actually depends on a client's broker/account/portfolio state.

The following current intent should be retained:

- canonical market/instrument identity;
- broker/account tradability must be known before action;
- Data Product quality and liquidity remain hard safety/quality concerns;
- Guardian restrictions remain authoritative within their scope;
- supported strategy applicability remains explicit;
- execution feasibility/cost remains required before execution;
- immutable universe snapshots and pinned decision-cycle evidence;
- ranking/discovery does not itself create a Trade Recommendation or execution authority;
- intelligent ranking may be CSA-eligible only where eligibility is proven, while hard gates remain outside autonomous model authority.

## 28. Governance Classification

This is not a cosmetic change.

It materially changes Market & Instrument Universe business semantics, client/broker treatment, candidate selection and the meaning of capital/price eligibility. Under the current R7 Development Change Classification model it is therefore classified as `DCC-3 — MATERIAL_DOMAIN_CHANGE`.

It is not eligible for the bounded 24-hour pre-delegation path.

Owner/governance review is required before it can replace current SIA semantics.

If the Owner directs adoption, the affected candidate bytes must be changed, a fresh semantic freeze created, and fresh Architecture/Consistency and Red-Team reviews performed over those exact bytes before final Owner acceptance.

## 29. Recommendation

**Recommended direction: ADOPT THE BROKER-NEUTRAL ADAPTIVE UNIVERSE MODEL, subject to the normal governed semantic-change lifecycle.**

Do not replace fixed Top-10 with fixed Top-100 as the final architecture.

Use the whole supported canonical market as the broad universe, aggressively share the cheap/wide market scan, separate global opportunity quality from broker/account actionability, use fractional capability as a per-broker/per-account feasibility fact, and allocate expensive analysis dynamically with coverage-aware scheduling.

This gives Falcon the widest responsible opportunity surface without disadvantaging small portfolios, without allowing one broker to define another broker's clients, and without creating unnecessary per-client duplicate market analysis.

## 30. Research Notes

External research reviewed on 2026-08-12 included:

- FINRA, `Investing in Fractional Shares`, current guidance on broker-dependent fractional availability and execution differences;
- Alpaca documentation for fractional trading and instrument-level `fractionable` capability;
- Interactive Brokers current fractional-trading materials showing broad fractional eligibility across thousands of stocks/ETFs;
- current dynamic-universe documentation/practice from quantitative trading platforms, used only as architectural comparison for broad filtering and dynamic candidate selection.

External documentation is evidence for design comparison only. Falcon's own governed contracts, capability profiles, broker certifications, market profiles and evidence remain authoritative for Falcon runtime behavior.
