# FSATS Market Qualification R4 — Project Owner Review Gate

**Package:** `FSATS-MARKET-QUALIFICATION-PROPOSAL-001`  
**Candidate Semantic Freeze:** `8b06940513e8ffba97d62a2589cd584e250ed7e8`  
**Architecture / Consistency:** `PASS`  
**Architecture Review:** `01C_FRESH_ARCHITECTURE_AND_CONSISTENCY_REVIEW_R4.md`  
**Fresh Red-Team:** `120 / 120 PASS`  
**Red-Team Review:** `02A_FRESH_RED_TEAM_REVIEW_R4.md`  
**Critical Open:** `0`  
**High Open:** `0`  
**Medium Open:** `0`  
**Post-Review Semantic Change:** `NONE`  
**Status:** `READY_FOR_PROJECT_OWNER_FINAL_REVIEW / NOT_OWNER_ACCEPTED / NOT_CLOSED`  
**Implementation / Runtime / Provider / Broker / Advisory / Manual-Execution / Research-Egress / Paper / Tiny Live / Live / Deployment Authority:** `NOT_GRANTED`

---

# 1. Exact Candidate Presented to Owner

The exact semantic candidate presented for final Owner decision is the unchanged seven-file set at commit:

```text
8b06940513e8ffba97d62a2589cd584e250ed7e8
```

Files:

```text
00_GOVERNED_MARKET_QUALIFICATION_AND_EXPANSION_LIFECYCLE_CANDIDATE.md
00A_PRE_REVIEW_AUTHORITY_AND_OWNER_COMMAND_RUNTIME_HARDENING.md
00B_PRE_REVIEW_MARKET_ACCESS_SCOPE_AND_VALUE_COMPLETENESS_HARDENING.md
00C_PRE_RED_TEAM_BOUNDED_AUTONOMY_RESOURCE_COST_AND_RESEARCH_SECURITY_HARDENING.md
00D_PRE_REVIEW_CANDIDATE_ISOLATION_AND_CROSS_MARKET_REGRESSION_HARDENING.md
00E_PRE_REVIEW_REQUEST_IDENTITY_IDEMPOTENCY_AND_REPLAY_HARDENING.md
00F_USER_TRADING_MANDATE_HORIZON_AND_ADVISORY_ONLY_MARKET_HARDENING.md
```

The R4 review files were added after the freeze. Git comparison from the semantic freeze to the completed R4 Red-Team state shows only these two added review files:

```text
01C_FRESH_ARCHITECTURE_AND_CONSISTENCY_REVIEW_R4.md
02A_FRESH_RED_TEAM_REVIEW_R4.md
```

No semantic candidate file changed after the R4 freeze.

---

# 2. Owner-Directed User Trading Mandate Captured

R4 establishes the prospective `USER TRADING MANDATE (UTM)` as a versioned, attributable business-authority boundary.

The user selects how Falcon may participate with the user's capital for an exact portfolio/broker/account/market scope.

Falcon then optimizes only inside that mandate and all stricter independent constraints.

Core rule:

```text
USER CHOOSES HOW FALCON MAY PARTICIPATE.
FALCON CHOOSES HOW BEST TO OPERATE INSIDE THAT MANDATE.
FALCON NEVER EXPANDS THE USER'S MANDATE.
```

---

# 3. Interaction Mode and Holding Horizon Are Independent

R4 separates:

```text
INTERACTION MODE
```

from:

```text
TRADING HORIZON POLICY
```

Candidate interaction modes:

```text
DISABLED
ADVISORY_ONLY
MANUAL_CONFIRMATION
AUTONOMOUS_TRADING
```

Candidate horizon policies may include named profiles and exact bounded rules such as:

```text
INTRADAY_ONLY
SHORT_SWING_ONLY
INTRADAY_AND_SHORT_SWING
ALL_CURRENTLY_VALIDATED_HORIZONS
CUSTOM_HORIZON_POLICY
MAX_POSITION_HOLDING = <explicit governed clock rule>
```

R4 also makes explicit:

```text
ANALYSIS TIMEFRAME != POSITION HOLDING HORIZON
```

and forbids arbitrarily truncating a longer validated strategy merely to fit a shorter user holding limit.

---

# 4. Per-Broker / Per-Account / Per-Market User Control

The same user may deliberately choose different modes for different capital scopes.

Example captured by the candidate:

```text
USER = MOHAMMAD

DEFAULT
  = ADVISORY_ONLY

ALPACA / ACCOUNT-A / US_EQUITIES
  = AUTONOMOUS_TRADING
  + INTRADAY_ONLY

BROKER-B / ACCOUNT-B / US_EQUITIES
  = AUTONOMOUS_TRADING
  + ALL_CURRENTLY_VALIDATED_HORIZONS

MARKET-X
  = ADVISORY_ONLY
```

More-specific broader overrides must be explicit, attributable and current. Ambiguous equal-specificity conflicts fail narrower rather than silently broader.

---

# 5. Effective Authority Is Always Narrowed by Independent Constraints

R4 defines the effective operating authority conceptually as:

```text
APPLICABLE USER TRADING MANDATE
∩ MARKET RULES
∩ MARKET ACCESS / REGULATORY CONSTRAINTS
∩ BROKER CAPABILITY
∩ ACCOUNT CAPABILITY
∩ VALIDATED STRATEGY / INTENDED USE
∩ TRADING HORIZON POLICY
∩ UNIFIED RISK
∩ CAPITAL AVAILABILITY / RESERVATION
∩ GUARDIAN RESTRICTIONS
∩ CURRENT SYSTEM AUTHORITY
```

A user may request a broader mode than a market permits; Falcon narrows.

A market/broker may permit a broader mode than the user wants; Falcon remains inside the user's mandate.

---

# 6. Mandate Change and Existing Position Safety

R4 adds version/epoch semantics so an old broader user mandate cannot silently authorize a new order after the user has changed to a narrower mode.

It also preserves existing position/obligation truth:

```text
MANDATE DOWNGRADE != POSITION ERASED
MANDATE DOWNGRADE != BLIND LIQUIDATION
```

New exposure may be stopped while existing exposure remains subject to position truth, Risk, Guardian, reconciliation and exact transition authority.

---

# 7. Market Qualification Now Handles Markets Without Automated Trading

The prior Market Qualification model is prospectively refined so a market is not reduced to a binary `automatically tradable / reject` outcome.

Mandatory distinctions:

```text
NO BROKER FOUND
!=
NO ANALYTICAL VALUE

NO ELIGIBLE AUTOMATED BROKER/API
!=
MARKET MUST BE REJECTED

MARKET PROHIBITS AUTOMATED TRADING
!=
MARKET PROHIBITS ANALYSIS / ADVICE
```

Where advisory use is lawful, supportable and evidence-backed, Falcon may qualify the market for a narrower advisory use without pretending that automated execution exists.

Advisory use still requires valid data/access/intended-use evidence and does not bypass FSAPMA or legal/access constraints.

---

# 8. Advisory-Only Semantics

For an advisory-only market, future separately authorized operation would conceptually be:

```text
GOVERNED OPERATIONAL DATA THROUGH FSAPMA
-> MARKET PROFILE
-> ELIGIBLE ANALYSIS
-> ELIGIBLE CENTRAL STRATEGIES
-> UNIFIED RISK / PORTFOLIO EFFECT
-> ATTRIBUTABLE RECOMMENDATION
-> USER EXECUTES OUTSIDE FALCON
```

R4 preserves:

```text
FALCON RECOMMENDATION
!= USER EXECUTION
!= BROKER ACK
!= FILL
!= POSITION TRUTH
```

No external manual execution truth is fabricated when no governed evidence path exists.

---

# 9. New Market Qualification Readiness Model

A successful market qualification may now return an exact mode-specific readiness class rather than forcing every useful market toward Paper execution readiness.

Examples:

```text
READY_FOR_ADVISORY_REVIEW
READY_FOR_MANUAL_CONFIRMATION_REVIEW
READY_FOR_PAPER_REVIEW
```

with capability facts such as:

```text
ADVISORY_ONLY_CAPABLE
AUTOMATED_EXECUTION_UNAVAILABLE
AUTOMATED_EXECUTION_PROHIBITED_FOR_INTENDED_SCOPE
BROKER_EXECUTION_PATH_NOT_FOUND
BROKER_EXECUTION_PATH_UNVERIFIED
```

Mandatory separation remains:

```text
READY_FOR_ADVISORY_REVIEW != ADVISORY_RUNTIME_AUTHORIZED
READY_FOR_MANUAL_CONFIRMATION_REVIEW != EXECUTION_AUTHORIZED
READY_FOR_PAPER_REVIEW != PAPER_AUTHORIZED
```

---

# 10. `ADD MARKET X` Result Is Extended

The prospective `ADD MARKET X` workflow now studies not only whether Market X can reach automated Paper review, but the exact feasible participation envelope.

The Owner-facing result will be able to state:

- canonical market/access state;
- maximum supported interaction mode;
- automated execution support/prohibition/unavailability;
- broker/account path state;
- eligible Trading Horizons;
- advisory readiness;
- Paper execution readiness where applicable;
- strategy applicability/adaptation;
- Risk profile/constraints;
- data/provider gaps and rights;
- operational/economic value case;
- exact next Owner decision.

A market that is valuable for advisory use but cannot be traded automatically is therefore represented honestly rather than discarded or mislabeled as Paper-ready.

---

# 11. Ownership and Current Architecture Preserved

R4 creates no new Application or LSA.

It preserves:

- T-LSA-01 user/account/environment context;
- T-LSA-02 Market Profile/market-rule ownership;
- T-LSA-06 strategy/horizon eligibility;
- T-LSA-07 Unified Risk;
- T-LSA-08 portfolio/capital;
- T-LSA-09 execution/position lifecycle;
- FSAPMA provider/data ownership;
- Guardian independent protection;
- FSTSimA non-Live validation/evidence ownership;
- Foundation/Application boundaries under APP-001, CON-023, ADR-I012 and ADR-I015.

The exact future UTM controller/storage/contract implementation placement remains separately gated.

---

# 12. Fresh Review Result

Architecture/Consistency R4:

```text
RESULT = PASS
CRITICAL = 0
HIGH = 0
MEDIUM = 0
```

Fresh static Red-Team R4:

```text
SCENARIOS = 120
PASS = 120
FAIL = 0
OPEN CRITICAL/HIGH/MEDIUM = 0
```

The Red-Team includes fresh regression of all 90 prior R3 scenarios plus 30 new attacks focused on:

- user mandate authority;
- default/scope overrides;
- mandate-version races;
- horizon/strategy integrity;
- advisory-only operation;
- no automated broker path;
- markets prohibiting automated execution;
- external user execution truth;
- position handling during mandate downgrade.

No semantic change occurred after the R4 semantic freeze.

---

# 13. Current Non-Authority

Even if the Project Owner accepts R4, documentary acceptance alone will not grant:

- implementation;
- runtime UTM enforcement/storage;
- provider/broker connectivity;
- user account access;
- operational advisory runtime;
- user-confirmed execution runtime;
- autonomous execution;
- Paper;
- Tiny Live;
- Live;
- deployment;
- research Internet egress;
- market admission;
- legal/licensing authority.

Every such future capability remains subject to its own governed authority and evidence.

---

# 14. Exact Owner Decision Required

The pending final decision is:

```text
ACCEPT the exact reviewed Market Qualification R4 semantic freeze
8b06940513e8ffba97d62a2589cd584e250ed7e8
as the controlling prospective FSATS design for:

1. governed User Trading Mandates by user/portfolio/broker/account/market scope;
2. separate Interaction Mode and Trading Horizon Policy;
3. mandate-version/stale-authority protection;
4. advisory-only operation semantics;
5. market qualification that preserves advisory value when automated execution is unavailable/prohibited;
6. mode-specific market readiness outcomes.
```

or:

```text
REQUEST CHANGES
```

If the Owner requests any semantic change, the changed candidate must receive a new semantic freeze and fresh Architecture/Consistency + fresh Red-Team before final acceptance.

No Owner acceptance is recorded by this gate itself.
