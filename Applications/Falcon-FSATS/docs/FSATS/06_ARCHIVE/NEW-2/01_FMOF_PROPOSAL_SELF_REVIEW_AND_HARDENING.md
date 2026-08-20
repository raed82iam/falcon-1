# FMOF Proposal Self-Review and Hardening

**Package:** `FSATS-FMOF-PROPOSAL-001`  
**Companion to:** `00_BROKER_NEUTRAL_MULTI_MARKET_OPPORTUNITY_FABRIC_REDESIGN_PROPOSAL.md`  
**Status:** `PROPOSAL-LEVEL SELF-REVIEW / HARDENING / NOT_OWNER_ACCEPTED / NOT_CLOSED`  
**Authority:** `NONE`  
**Official Architecture/Consistency Review:** `NOT PERFORMED FOR FMOF MATERIALIZED SEMANTICS`  
**Official Red-Team Review:** `NOT PERFORMED FOR FMOF MATERIALIZED SEMANTICS`  
**Interpretation Rule:** If this hardening document narrows or corrects wording in Proposal 00, the narrower interpretation in this document is the intended FMOF proposal-package semantics presented to the Project Owner.

---

# 1. Purpose

After writing Proposal 00, a fresh proposal-level adversarial review was performed before presenting the package as ready for Owner judgment.

The review found no reason to abandon FMOF, but it identified several areas where the first proposal wording could permit future semantic drift if left uncorrected.

This file hardens those boundaries now.

It does not modify R7, create a new semantic freeze, or claim an official Architecture/Consistency or Red-Team PASS.

---

# 2. Hardening Finding H-001: Pre-Risk Account Projection Must Not Own Capital Feasibility

## Finding

Proposal 00 uses the name:

`AccountActionabilityProjection`

before Strategy, Unified Risk and Capital Reservation.

It also mentions minimum-order feasibility.

That wording can be misread as granting a pre-Risk layer authority to decide whether an Account has enough capital to trade.

That would overlap T-LSA-08 and weaken the current separation of responsibilities.

## Required FMOF interpretation

For the proposal package, the pre-Strategy/pre-Risk projection is narrowed to:

`AccountStructuralEligibilityProjection`

Candidate states:

```text
STRUCTURALLY_ELIGIBLE
STRUCTURALLY_ELIGIBLE_WITH_RESTRICTIONS
STRUCTURALLY_INELIGIBLE
UNKNOWN
CONFLICTED
```

It may answer only structural/access questions such as:

- Is this the correct Account and Broker binding?
- Is the account currently enabled for the relevant product/market?
- Does the Broker/account class expose the required structural capability?
- Is fractional trading structurally permitted for this instrument/account combination?
- Are the relevant session/order capabilities structurally available?
- What Broker-declared minimum quantity/notional constraint would a later sized order have to satisfy?
- Is a jurisdiction/account/product restriction known?

It SHALL NOT decide:

```text
ACCOUNT_HAS_ENOUGH_CAPITAL
CAPITAL_SHOULD_BE_ALLOCATED
RISK_LIMIT_ALLOWS_THIS_TRADE
PORTFOLIO_CAN_ABSORB_THIS_EXPOSURE
RESERVATION_CAN_SUCCEED
FINAL_ORDER_SIZE
```

Those remain downstream.

Correct chain:

```text
GlobalOpportunityArtifact
+
BrokerCapabilityProjection
+
AccountStructuralContext
=
AccountStructuralEligibilityProjection

-> Strategy evaluation
-> Unified Risk
-> T-LSA-08 Capital / Reservation
-> T-LSA-09 Execution
```

A Broker-declared minimum notional is capability metadata. Whether the Account can or should fund that notional is a Capital/Risk question.

## Consequence

If FMOF is Owner-accepted, `AccountActionabilityProjection` in Proposal 00 should be materialized under the narrower `AccountStructuralEligibilityProjection` semantic unless the Owner explicitly chooses another exact name during semantic materialization.

---

# 3. Hardening Finding H-002: “Actionable” Must Be Reserved for a Later State

To avoid semantic inflation:

```text
STRUCTURAL_ELIGIBILITY
!=
STRATEGY_APPROVAL
!=
RISK_APPROVAL
!=
CAPITAL_RESERVATION
!=
EXECUTION_AUTHORITY
```

The term `ACTIONABLE` should not be used as a pre-Risk synonym for “Broker/account structurally supports this instrument.”

Recommended terminology:

```text
GLOBAL OPPORTUNITY
-> STRUCTURALLY ELIGIBLE FOR ACCOUNT
-> STRATEGY-ELIGIBLE CANDIDATE
-> RISK-APPROVED PROPOSAL
-> CAPITAL-RESERVED PROPOSAL
-> EXECUTION-ELIGIBLE INTENT
```

This keeps language aligned with authority.

---

# 4. Hardening Finding H-003: Broker Execution Quality Must Not Leak into Global Market Truth

Proposal 00 references `MarketExecutionEnvironmentQuality` as a possible MarketCapitalFitness input.

This term is accepted only under a narrow interpretation.

## Allowed global meaning

Market-wide or market-profile microstructure facts that are not specific to one Broker/account/route, for example:

- evidence-backed market liquidity regime;
- market-wide spread/volatility conditions under defined Data Product semantics;
- market session/venue state;
- normalized market impact proxy when defined independently of a particular Broker.

## Forbidden global meaning

```text
BROKER_A_FILL_QUALITY
BROKER_A_ROUTE_COST
BROKER_A_SLIPPAGE
ACCOUNT_A_COMMISSION
ACCOUNT_A_REBATE
BROKER_A_MINIMUM_ORDER
BROKER_A_FRACTIONAL_EXECUTION_QUALITY
```

Those remain Broker/account/execution facts.

If a metric cannot be proven Broker-neutral, it must not enter a global MarketOpportunity score as universal truth.

---

# 5. Hardening Finding H-004: Cost and Legal Usage Rights Are Different Dimensions

## Finding

The first proposal correctly rejects the permanent assumption that provider plans remain free forever.

For a future multi-user/commercial Falcon, one more separation is mandatory:

```text
PRICE = 0
DOES NOT IMPLY
RIGHT TO USE / DISPLAY / REDISTRIBUTE / COMMERCIALIZE = YES
```

Official provider material checked on `2026-08-12` demonstrates that providers distinguish personal, internal, commercial and external-distribution rights.

Therefore FSAPMA certification must not model provider suitability using cost/quota/coverage alone.

## Proposed exact capability dimensions

A certified Provider/Data Product route should expose, where relevant:

```text
ProviderCostClass
EntitlementFingerprint
UsageRightsFingerprint
CommercialUseClass
DisplayRight
NonDisplayRight
ExternalDistributionRight
DerivedDataRight
RedistributionRight
AIOrAutomatedAnalysisRight       where terms require distinction
MarketOrExchangeLicenseRefs[]
UserPopulationScope
EnvironmentScope
EffectiveAt
ExpiresAt
RevalidationTrigger
EvidenceRefs[]
```

Candidate usage-right states may include:

```text
PERSONAL_ONLY
INTERNAL_NON_COMMERCIAL
INTERNAL_BUSINESS
EXTERNAL_DISPLAY_ALLOWED
EXTERNAL_DISTRIBUTION_ALLOWED
DERIVED_OUTPUT_ONLY
REQUIRES_SEPARATE_EXCHANGE_LICENSE
UNKNOWN
CONFLICTED
```

The exact taxonomy must be reconciled with current FSAPMA entitlement/licensing types before adding anything new.

## Fail-closed rule

```text
USAGE_RIGHTS_UNKNOWN
!=
USAGE_ALLOWED
```

No provider route may become valid for a new multi-user/commercial use case merely because it is technically reachable or costs zero.

---

# 6. Current Official Licensing Evidence

This evidence is point-in-time research and must be re-certified by FSAPMA before actual runtime use.

## Twelve Data

Twelve Data's current Individual pricing page states that Individual access is for personal, internal and non-commercial purposes. Its Business materials distinguish internal business usage from external display/distribution capability, and its support documentation states that redistribution can require separate agreements and exchange licensing.

Sources:

`https://twelvedata.com/pricing`

`https://twelvedata.com/pricing-business`

`https://support.twelvedata.com/en/articles/5332349-commercial-and-personal-usage`

## Finnhub

Finnhub's current pricing page labels its Free plan license as `Personal Use. Terms apply`.

Source:

`https://finnhub.io/pricing`

## Massive

Massive's current Market Data Terms state a limited license for personal, non-business and non-commercial use under the cited terms and explicitly restrict building an application for end users other than the subscriber under that license path.

Source:

`https://massive.com/legal/market-data-terms-of-service`

## Architectural conclusion

For the current single-user proof phase, a free route may still be usable when its actual terms and Falcon's use are compatible.

Before Falcon becomes a licensed multi-user/commercial system, every operational Data Product must be recertified against the exact intended user population and use mode.

This is not a reason to abandon free providers.

It is a reason to distinguish:

```text
FREE COST
FROM
LEGAL USAGE AUTHORITY
```

---

# 7. Hardening Finding H-005: Market Universe Is Falcon's Evidence-Backed Canonical View, Not Omniscience

The phrase `Market Truth` must not be interpreted as claiming Falcon has metaphysical or exchange-complete knowledge of every instrument at all times.

The canonical object is:

```text
FALCON'S CURRENT EVIDENCE-BACKED CANONICAL MARKET UNIVERSE
```

Its completeness is always explicit:

```text
COMPLETE_FOR_DECLARED_SCOPE
PARTIAL
UNKNOWN
CONFLICTED
```

This prevents a new provider limitation from silently deleting known instruments, while also preventing Falcon from claiming knowledge it does not possess.

---

# 8. Hardening Finding H-006: Provider Free-Tier Constraints Must Not Distort Opportunity Scores

Provider quota/cost constraints belong to Observation Coverage and analysis scheduling.

They may affect:

- whether a candidate can be refreshed now;
- evidence freshness;
- analysis cost;
- confidence;
- work scheduling;
- degradation state.

They SHALL NOT create a rule such as:

```text
EXPENSIVE_TO_OBSERVE = BAD MARKET OPPORTUNITY
```

A candidate can remain economically attractive while Falcon lacks sufficient current evidence to act on it.

Correct state:

```text
POTENTIALLY_INTERESTING_BUT_INSUFFICIENT_CURRENT_EVIDENCE
```

not:

```text
LOW_QUALITY_OPPORTUNITY
```

unless the market evidence itself justifies that conclusion.

---

# 9. Hardening Finding H-007: Multi-User Fairness Must Not Become Client-Demand Popularity Bias

A multi-user system can accidentally produce a new feedback loop:

```text
MORE USERS ON BROKER A
-> MORE REQUESTS FOR BROKER A SYMBOLS
-> MORE DEEP ANALYSIS
-> BETTER COVERAGE FOR BROKER A
-> EVEN MORE APPARENT OPPORTUNITY FOR BROKER A
```

FMOF must prevent this.

Global discovery/deep-analysis priority must remain primarily market/evidence driven.

Broker/account demand may justify downstream projection work, but it SHALL NOT silently become a global market-quality score.

If client demand is ever allowed to influence shared analysis scheduling, it must be:

- explicitly typed as resource-demand information;
- bounded;
- separate from opportunity quality;
- unable to starve under-covered market candidates;
- privacy-preserving;
- versioned and reviewable.

---

# 10. Hardening Finding H-008: Shared Caches Need Exact Invalidators

Reuse is safe only when invalidation is as rigorous as creation.

The final FMOF design should define invalidators for at least:

```text
MarketProfileVersion changed
Instrument identity changed/conflicted
Data Product version changed
Input digest changed
ObservationCoverage changed materially
Feature profile changed
Model/algorithm changed
Analysis policy changed
BrokerCapabilityFingerprint changed
Account structural context changed
Guardian restriction changed where relevant
Artifact expired
Material market event invalidated assumptions
```

A cache hit is not evidence of semantic validity by itself.

---

# 11. Hardening Finding H-009: Analysis Fairness Cannot Promise Full Deep Coverage of an Unbounded Universe

The Opportunity Consideration Ledger must not create an impossible guarantee that every known instrument receives expensive deep analysis within a fixed short period.

The required guarantee is narrower:

1. broad/cheap universe census follows a MarketProfile-governed coverage policy;
2. candidates satisfying hard discovery eligibility enter consideration accounting;
3. eligible candidates cannot be silently starved forever by score incumbents;
4. deep-analysis service guarantees are bounded by available governed resource and provider/data capability;
5. unmet coverage obligations remain visible as debt/degradation rather than being declared complete.

Thus:

```text
FAIR CONSIDERATION
!=
UNLIMITED COMPUTE PROMISE
```

---

# 12. Revised Core Pipeline After Hardening

The hardened FMOF proposal should be read as:

```text
EVIDENCE-BACKED CANONICAL MARKET UNIVERSE
        |
        v
OBSERVATION COVERAGE TRUTH
        |
        v
CHEAP / WIDE DISCOVERY
        |
        v
ADAPTIVE DEEP-ANALYSIS PLAN
        |
        v
GLOBAL OPPORTUNITY ARTIFACT
        |
        +-----------------------+
        |                       |
        v                       v
BROKER CAPABILITY          ACCOUNT STRUCTURAL CONTEXT
        |                       |
        +-----------+-----------+
                    v
ACCOUNT STRUCTURAL ELIGIBILITY
                    |
                    v
STRATEGY EVALUATION
                    |
                    v
UNIFIED RISK
                    |
                    v
CAPITAL / RESERVATION
                    |
                    v
EXECUTION / BROKER RECONCILIATION
```

Provider acquisition and provider entitlement/usage-right facts remain owned by FSAPMA.

---

# 13. Additional Required Types If FMOF Is Accepted

During semantic materialization, reconcile existing types before adding new ones. Candidate additional semantics are:

```text
UsageRightsFingerprint
DataUsageRightsClass
CommercialUseClass
ExternalDistributionRight
AccountStructuralEligibilityProjectionId
CoverageObligationState
CacheInvalidationReason
```

Do not create duplicate types if current FSAPMA/Trading catalogs already provide equivalent exact semantics.

---

# 14. Additional Impact if Owner Accepts

The Proposal 00 impact list remains valid, with one additional review requirement:

- FSAPMA provider certification/entitlement semantics must be checked for explicit legal usage-right, redistribution and commercial-use coverage.

This does not automatically mean a new Application, LSA or Foundation contract is required.

First reuse the existing provider entitlement/certification architecture if it can express the required semantics exactly.

If a generic Foundation external-access capability is missing, use the existing FCR process rather than implementing a hidden Application-side substitute.

---

# 15. Self-Review Disposition

Proposal-level findings:

```text
H-001 PRE-RISK CAPITAL-OWNERSHIP AMBIGUITY       -> HARDENED
H-002 ACTIONABLE TERMINOLOGY TOO EARLY            -> HARDENED
H-003 BROKER EXECUTION QUALITY LEAK RISK           -> HARDENED
H-004 FREE COST VS USAGE RIGHTS                    -> HARDENED
H-005 MARKET-TRUTH OVERCLAIM RISK                  -> HARDENED
H-006 PROVIDER COST DISTORTING OPPORTUNITY          -> HARDENED
H-007 CLIENT-POPULARITY BIAS                        -> HARDENED
H-008 CACHE INVALIDATION                            -> HARDENED
H-009 IMPOSSIBLE FAIR-COVERAGE PROMISE              -> HARDENED
```

No `Critical` or `High` proposal-level architectural contradiction remains known after applying these narrower interpretations to the FMOF package.

This statement is **not** an official Architecture/Consistency or Red-Team PASS.

Formal review can occur only after the Owner accepts the direction and exact affected SIA semantics are materialized into a new candidate freeze.

---

# 16. Owner Review Package

The FMOF proposal presented to the Project Owner is now the combination of:

```text
00_BROKER_NEUTRAL_MULTI_MARKET_OPPORTUNITY_FABRIC_REDESIGN_PROPOSAL.md
+
01_FMOF_PROPOSAL_SELF_REVIEW_AND_HARDENING.md
```

If the Owner accepts the FMOF direction, semantic materialization SHALL incorporate the hardenings in this file rather than mechanically copying broader wording from Proposal 00.

**Package State:** `READY_FOR_PROJECT_OWNER_REVIEW / NOT_OWNER_ACCEPTED / NOT_CLOSED`  
**Implementation Authority:** `NOT_GRANTED`  
**Runtime Authority:** `NOT_GRANTED`
