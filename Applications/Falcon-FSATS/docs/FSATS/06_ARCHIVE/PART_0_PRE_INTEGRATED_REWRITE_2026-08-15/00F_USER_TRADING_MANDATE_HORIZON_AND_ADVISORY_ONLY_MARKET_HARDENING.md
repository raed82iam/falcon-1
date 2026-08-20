# FSATS Market Qualification Candidate — User Trading Mandate, Horizon and Advisory-Only Market Hardening

**Package:** `FSATS-MARKET-QUALIFICATION-PROPOSAL-001`  
**Applies To:** `00` + `00A` + `00B` + `00C` + `00D` + `00E` of this NEW-3 package  
**Decision Type:** `OWNER-DIRECTED SEMANTIC HARDENING / USER CAPITAL AUTHORITY / MARKET-USE MODE`  
**Status:** `CONTROLLING CANDIDATE HARDENING / NOT_OWNER_ACCEPTED / NOT_CLOSED`  
**Branch:** `application-development`  
**Implementation / Runtime / Provider / Broker / Research-Egress / Paper / Tiny Live / Live / Deployment Authority:** `NOT_GRANTED`

---

# 1. Purpose

This record materializes the Project Owner's clarification that Falcon SHALL NOT choose the user's permitted relationship with a portfolio/account/market as though trading style were solely an internal optimization choice.

The user owns the decision about how Falcon may participate with the user's capital inside each governed scope.

Falcon may optimize only inside that mandate and inside all stricter market, broker, account, regulatory/access, strategy-validation, Risk, capital, Guardian and system-authority constraints.

This record also hardens the new-market qualification lifecycle so that absence of an eligible automated-trading broker/path, or a market rule that prohibits automated execution, does not automatically make the market useless to Falcon.

Where lawful, technically supportable and evidence-backed, the market may instead qualify for a narrower advisory-only use in which Falcon analyzes, evaluates, applies strategies and Risk reasoning, and produces attributable recommendations without executing the trade.

Mandatory principles:

```text
USER CHOOSES HOW FALCON MAY PARTICIPATE WITH THE USER'S CAPITAL.

FALCON CHOOSES HOW BEST TO OPERATE INSIDE THAT MANDATE.

FALCON SHALL NOT EXPAND THE USER'S MANDATE.
```

and:

```text
NO ELIGIBLE AUTOMATED EXECUTION PATH
!=
MARKET HAS NO VALUE TO FALCON
```

---

# 2. User Trading Mandate Is an Authority Boundary, Not a Preference

The governing concept is the:

```text
USER TRADING MANDATE (UTM)
```

The UTM is a versioned, attributable business-authority boundary defining what Falcon may do with a user's capital in an exact scope.

It SHALL NOT be modeled merely as a cosmetic preference such as a UI setting that downstream components may ignore.

Conceptually, a mandate binds as applicable:

```text
User
Portfolio
Broker
Broker Account
Market / Venue Scope
Asset / Instrument Scope where required
Interaction Mode
Trading Horizon Policy
Capital / Exposure Constraints where separately authorized
Effective Version / Epoch
Activation State
Created / Changed By
Change Evidence
```

The exact implementation contract remains future work and is not authorized by this candidate.

Mandatory distinction:

```text
USER PREFERENCE
!=
USER CAPITAL AUTHORITY
```

A component may recommend a different mode or horizon to the user, but it may not silently apply the recommendation as new authority.

---

# 3. Interaction Mode and Trading Horizon Are Separate Axes

Falcon SHALL NOT collapse execution authority and trading duration into one ambiguous `trading category` field.

The design SHALL distinguish at least:

```text
A. INTERACTION MODE
   What may Falcon do?

B. TRADING HORIZON POLICY
   If trading is permitted, which position-holding horizons may Falcon use?
```

This separation prevents a phrase such as `intraday` from being incorrectly treated as execution permission, and prevents `autonomous` from being incorrectly treated as permission for every holding horizon.

---

# 4. Interaction Modes

The candidate shall support the following conceptual modes, subject to later contract naming/refinement:

```text
DISABLED
ADVISORY_ONLY
MANUAL_CONFIRMATION
AUTONOMOUS_TRADING
```

## 4.1 DISABLED

For the exact governed scope, Falcon SHALL NOT create new trading exposure and SHALL NOT initiate new trade recommendations intended for execution unless the mandate is changed through a valid newer authority state.

`DISABLED` SHALL NOT erase or fabricate existing positions, orders, reservations, obligations, Risk state, Guardian restrictions, reconciliation duties or historical evidence.

## 4.2 ADVISORY_ONLY

Falcon may use the separately authorized/available analysis, strategy, market, Risk, portfolio and intelligence capabilities needed to produce an attributable advisory recommendation for the user.

Falcon SHALL NOT submit, route, place, amend, cancel or otherwise execute the trade merely because the recommendation is complete.

Conceptually:

```text
ANALYZE
-> EVALUATE STRATEGY
-> EVALUATE RISK
-> EVALUATE CAPITAL / PORTFOLIO EFFECT
-> PRODUCE ADVICE
-> USER REMAINS EXECUTION ACTOR
```

Mandatory invariant:

```text
ADVICE != ORDER
```

Advisory output may contain, where evidence and policy permit, proposed instrument/action, entry/exit concept, size/risk envelope, expected holding horizon, confidence/uncertainty, assumptions, invalidation conditions and explanation.

Advisory output does not become broker execution authority.

## 4.3 MANUAL_CONFIRMATION

Where the exact market, broker, account, access rules and runtime capabilities permit user-confirmed electronic execution, Falcon may prepare a bounded executable intent but SHALL require the user's valid confirmation before submission.

Conceptually:

```text
ANALYZE
-> DECIDE CANDIDATE
-> RISK / CAPITAL / GUARDIAN GATES
-> PREPARE EXACT ORDER INTENT
-> USER CONFIRMATION
-> REVALIDATE CURRENT AUTHORITY / RISK / MARKET / BROKER STATE
-> EXECUTION ONLY IF STILL VALID
```

Manual confirmation is not a workaround for a market that forbids automated/system-submitted execution. If the applicable rule treats the resulting Falcon-submitted order as prohibited automated execution, this mode is unavailable for that scope.

## 4.4 AUTONOMOUS_TRADING

Where separately authorized and supported, Falcon may execute without per-order user confirmation only inside the exact UTM, Trading Horizon Policy, market/broker/account capability, Risk, capital, Guardian, validation and current system-authority boundaries.

`AUTONOMOUS_TRADING` does not imply unrestricted strategy, horizon, market, leverage, instrument, capital or broker authority.

---

# 5. Trading Horizon Policy

The user SHALL be able to constrain the position-holding horizons allowed for an exact mandate scope.

The design SHALL support named profiles and/or exact bounded constraints without forcing Falcon into one universal holding period.

Conceptual examples include:

```text
INTRADAY_ONLY
SHORT_SWING_ONLY
INTRADAY_AND_SHORT_SWING
ALL_CURRENTLY_VALIDATED_HORIZONS
CUSTOM_HORIZON_POLICY
```

A custom policy may include exact constraints such as:

```text
MAX_POSITION_HOLDING = 7 CALENDAR DAYS
```

or another explicitly defined market-aware clock basis.

The exact clock basis SHALL be explicit where material, including the distinction between calendar time, trading sessions, market operating cycles and other governed clocks.

Mandatory distinction:

```text
ANALYSIS TIMEFRAME
!=
POSITION HOLDING HORIZON
```

A strategy constrained to an intraday position may still use higher-timeframe analytical context when its validated intended use permits it. Conversely, use of a daily/weekly analysis input does not grant permission to hold a position for days/weeks.

---

# 6. Strategy Eligibility Must Respect the User Horizon

The Strategy Controller may consider only strategy/horizon combinations validated for the effective user mandate and exact market/account context.

If a strategy's validated intended holding horizon exceeds the user's allowed horizon, Falcon SHALL NOT make the strategy compliant by arbitrarily truncating the trade.

Mandatory invariant:

```text
STRATEGY VALIDATED FOR 30-DAY HOLD
+
USER MAX HOLD = 7 DAYS
!=
CUT THE 30-DAY STRATEGY OFF AT DAY 7
```

Instead:

```text
STRATEGY / HORIZON COMBINATION = NOT ELIGIBLE FOR THAT MANDATE
```

unless that same central strategy has a separately validated <=7-day operating mode with its own intended-use evidence.

This preserves the accepted central Strategy Catalog model and prevents hidden semantic mutation of a strategy merely to fit a user's capital-duration constraint.

---

# 7. User Mandate Scoping and Explicit Overrides

A user may govern different portfolios, brokers, accounts and markets differently.

Example:

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

A default is a fallback, not an invisible prohibition against every more-specific explicit mandate.

A more-specific user mandate may broaden or narrow the user's fallback only when it is itself an explicit, attributable, current user authority for that exact scope.

If overlapping mandates of equal applicable specificity conflict materially and current authority cannot be resolved deterministically, Falcon SHALL use the safer/narrower effective state and surface the conflict rather than inventing broader authority.

No scope inheritance rule may silently broaden execution authority.

---

# 8. Effective Trading Authority Is the Narrowest Valid Intersection

After the applicable user mandate is resolved, effective authority is bounded by all independent constraints.

Conceptually:

```text
EFFECTIVE_TRADING_AUTHORITY
=
APPLICABLE_USER_TRADING_MANDATE
INTERSECT MARKET_RULES
INTERSECT MARKET_ACCESS / REGULATORY_CONSTRAINTS
INTERSECT BROKER_CAPABILITY
INTERSECT ACCOUNT_CAPABILITY
INTERSECT STRATEGY_VALIDATED_INTENDED_USE
INTERSECT TRADING_HORIZON_POLICY
INTERSECT UNIFIED_RISK
INTERSECT CAPITAL_AVAILABILITY / RESERVATION
INTERSECT GUARDIAN_RESTRICTIONS
INTERSECT CURRENT_SYSTEM_AUTHORITY
```

The intersection may narrow authority to `ADVISORY_ONLY`, `NO_NEW_EXPOSURE`, `NO_TRADE`, another safe state, or complete prohibition.

Falcon may reduce effective authority when required by a stricter independent constraint.

Falcon SHALL NOT raise effective authority beyond the user's mandate merely because the market/broker/system technically supports more.

---

# 9. Mandate Version / Epoch and Stale-Authority Safety

Each material UTM change SHALL create a new attributable version/epoch or equivalent immutable authority identity.

A later stricter mandate SHALL invalidate new-exposure actions that depend on an older broader mandate unless the action is re-evaluated and proven valid under the new current mandate.

Conceptually:

```text
MANDATE_V17 = AUTONOMOUS_TRADING
MANDATE_V18 = ADVISORY_ONLY

NEW ORDER BOUND TO V17
AFTER V18 BECOMES EFFECTIVE
-> STALE AUTHORITY
-> MUST NOT EXECUTE AS NEW EXPOSURE
```

The exact implementation shall bind current mandate identity/version into decision/execution authority when that runtime is later authorized.

---

# 10. Existing Positions Survive a Mandate Downgrade as Obligations

Changing a mandate from autonomous/manual execution to advisory/disabled SHALL NOT erase or fabricate existing exposure.

Mandatory invariant:

```text
MANDATE DOWNGRADE
!=
POSITION DISAPPEARS
```

The safer default for new risk is:

```text
NO NEW EXPOSURE
```

while existing positions/orders/reservations/settlement obligations remain subject to authoritative position truth, Unified Risk, Guardian protection, reconciliation and the exact governed transition policy.

The system SHALL NOT blindly liquidate merely because the user lowered future trading authority if liquidation itself is unsafe, unsupported, unauthorized, market-closed, ambiguous or inconsistent with stronger protection requirements.

Any transition action must remain separately justified and authorized.

---

# 11. Market Qualification Must Determine Supported Interaction Modes

The new-market qualification lifecycle in `00` and the market-access hardening in `00B` are hereby refined.

A Market X qualification SHALL determine not merely whether the market can support fully automated Paper/Live trading, but the maximum evidence-backed interaction modes available for the exact intended scope.

The qualification shall evaluate as applicable:

- whether analysis/advisory use is permitted and technically supportable;
- whether required market/data rights permit the intended analytical/advisory use;
- whether a valid operational Data Product can be supplied through FSAPMA when later authorized;
- whether any broker/account access exists;
- whether the broker exposes an eligible execution interface;
- whether market/broker/account rules permit system-submitted automated execution;
- whether user-confirmed electronic execution is permitted and technically distinguishable where relevant;
- whether only human/manual external execution is allowed;
- which Trading Horizon Policies are feasible for the market/account/broker combination;
- what execution/settlement limitations would narrow use;
- what legal/access/regulatory unknowns prevent a stronger mode claim.

This market qualification fact set constrains but does not replace the user's mandate.

---

# 12. Missing Automated Broker / Automated Trading Prohibition Does Not Automatically Reject the Market

The earlier qualification logic is refined by the following mandatory distinctions:

```text
NO BROKER FOUND
!=
NO DATA / ANALYSIS VALUE

BROKER EXISTS BUT NO ELIGIBLE AUTOMATED API
!=
MARKET MUST BE REJECTED

MARKET PROHIBITS AUTOMATED TRADING
!=
MARKET PROHIBITS ANALYSIS / ADVICE
```

If automated execution is unavailable or prohibited, Falcon SHALL evaluate whether a narrower evidence-backed use is valid.

A market may therefore qualify conceptually as:

```text
ADVISORY_ONLY_CAPABLE
```

when all required analysis/data/access/legal/intended-use conditions are satisfied for advisory use even though autonomous execution is unavailable.

If Falcon cannot establish that advisory use itself is permitted/supportable, it SHALL NOT assume permission.

Unknown remains `UNKNOWN`.

---

# 13. Advisory-Only Market Operating Model

For a market whose effective maximum mode is `ADVISORY_ONLY`, Falcon may, when the necessary future runtime capabilities and data authority are separately granted:

```text
OBTAIN GOVERNED OPERATIONAL DATA THROUGH FSAPMA
-> APPLY MARKET PROFILE
-> APPLY ELIGIBLE ANALYSIS FRAMEWORKS
-> APPLY ELIGIBLE CENTRAL STRATEGIES
-> APPLY UNIFIED RISK / PORTFOLIO IMPACT ANALYSIS
-> PRODUCE ATTRIBUTABLE RECOMMENDATION
-> USER EXECUTES OUTSIDE FALCON
```

Falcon SHALL NOT claim broker submission, fill, position or account outcome truth for a user-executed trade unless a separately governed mechanism later provides authoritative evidence for that state.

Advisory-only use SHALL therefore distinguish:

```text
FALCON RECOMMENDATION
!=
USER EXECUTION
!=
BROKER ACK
!=
FILL
!=
POSITION TRUTH
```

If the user later records/imports execution evidence through an authorized mechanism, that separate mechanism shall govern how external execution becomes attributable portfolio/position truth.

This hardening does not invent such a mechanism or authority now.

---

# 14. Qualification Readiness Outcomes Are No Longer One-Dimensional

The successful terminal recommendation in `00` is refined.

`READY_FOR_PAPER_REVIEW` remains valid only for an exact intended use that actually has an eligible separately governable Paper execution path.

Market qualification may instead return a narrower readiness class such as:

```text
READY_FOR_ADVISORY_REVIEW
READY_FOR_MANUAL_CONFIRMATION_REVIEW
READY_FOR_PAPER_REVIEW
```

or a constrained factual outcome such as:

```text
ADVISORY_ONLY_CAPABLE
AUTOMATED_EXECUTION_UNAVAILABLE
AUTOMATED_EXECUTION_PROHIBITED_FOR_INTENDED_SCOPE
BROKER_EXECUTION_PATH_NOT_FOUND
BROKER_EXECUTION_PATH_UNVERIFIED
```

These are readiness/capability classifications only.

Mandatory invariants:

```text
READY_FOR_ADVISORY_REVIEW != ADVISORY_RUNTIME_AUTHORIZED
READY_FOR_MANUAL_CONFIRMATION_REVIEW != EXECUTION_AUTHORIZED
READY_FOR_PAPER_REVIEW != PAPER_AUTHORIZED
```

A market SHALL NOT be described as `READY_FOR_PAPER_REVIEW` merely because analysis and advisory use are ready while the Paper execution path is absent.

Conversely, absence of Paper readiness SHALL NOT erase a valid advisory qualification result.

---

# 15. Market Qualification Request Semantics Are Extended

The conceptual `MarketQualificationRequest` in `00` is refined to carry or resolve, as applicable:

```text
RequestedInteractionModeCeiling
RequestedTradingHorizonPolicy
RequestedAdvisoryUse
RequestedExecutionUse
```

The Owner may ask Falcon to study the broad feasible capability range, or may deliberately request a narrower intended use.

If the Owner says only:

```text
ADD MARKET X
```

the qualification workflow may study the full bounded feasible interaction-mode range, but SHALL NOT interpret that instruction as authority to execute in any of those modes.

The final result shall state what was studied, what is feasible, what is not feasible, and what separate next decision would be required.

---

# 16. Owner-Facing Market Qualification Summary Is Extended

The final market qualification package shall summarize, as applicable:

```text
Market:
  <canonical identity>

Market Access / Rules:
  COMPATIBLE / CONDITIONAL / BLOCKED / UNKNOWN

Maximum Supported Interaction Mode:
  DISABLED / ADVISORY_ONLY / MANUAL_CONFIRMATION / AUTONOMOUS_TRADING
  or UNKNOWN / CONDITIONAL

Automated Execution:
  SUPPORTED / CONDITIONAL / PROHIBITED / UNAVAILABLE / UNVERIFIED

Broker / Account Path:
  VERIFIED / CONDITIONAL / NOT_FOUND / UNVERIFIED / NOT_REQUIRED_FOR_ADVISORY_ONLY

Eligible Trading Horizons:
  <validated exact horizon set / constraints>

Advisory Capability:
  READY_FOR_ADVISORY_REVIEW / NOT_READY / BLOCKED / UNKNOWN

Paper Execution Capability:
  READY_FOR_PAPER_REVIEW / NOT_READY / BLOCKED / NOT_APPLICABLE

Strategies:
  <existing validated / adapted candidates / new candidates / not applicable>

Risk:
  <qualified constraints / blockers / unknowns>

Data / Provider:
  <required products / coverage / gaps / rights / cost>

Value Case:
  SUPPORTS ADMISSION REVIEW / MARGINAL / DOES NOT JUSTIFY ADMISSION / INSUFFICIENT EVIDENCE

Exact Next Owner Decision:
  <bounded next decision>
```

The summary SHALL NOT collapse a market into a binary `tradable/not tradable` label when a valid narrower advisory use exists.

---

# 17. Examples

## 17.1 User chooses intraday automation on one broker

```text
USER = MOHAMMAD
BROKER = ALPACA
ACCOUNT = A
MARKET = US_EQUITIES

UTM.INTERACTION_MODE = AUTONOMOUS_TRADING
UTM.HORIZON_POLICY = INTRADAY_ONLY
```

Falcon may consider only strategies/horizon modes that remain valid after all other constraints.

A 30-day strategy is not eligible merely because it is profitable.

## 17.2 Same user chooses broader horizons on another broker

```text
USER = MOHAMMAD
BROKER = BROKER_B
ACCOUNT = B
MARKET = US_EQUITIES

UTM.INTERACTION_MODE = AUTONOMOUS_TRADING
UTM.HORIZON_POLICY = ALL_CURRENTLY_VALIDATED_HORIZONS
```

The word `ALL` means all horizons currently validated and otherwise permitted for that exact scope, not unlimited future authority.

## 17.3 User wants advice only in Market X

```text
USER = MOHAMMAD
MARKET = MARKET_X
UTM.INTERACTION_MODE = ADVISORY_ONLY
```

Even if an automated broker later becomes available, Falcon SHALL remain advisory-only for this user/scope until the user changes the mandate through a valid newer authority state.

## 17.4 Market X forbids automated execution

```text
USER REQUESTED MODE = AUTONOMOUS_TRADING
MARKET MAXIMUM SUPPORTED MODE = ADVISORY_ONLY

EFFECTIVE MODE = ADVISORY_ONLY
```

Falcon may report the mismatch and advisory capability, but SHALL NOT treat the user's broader request as permission to violate the market rule.

---

# 18. Ownership Mapping

This hardening does not create a new Application or LSA.

Existing responsibilities remain:

```text
T-LSA-01
= user/account/environment mandate context and operational readiness awareness

T-LSA-02
= market rules / Market Profile / instrument eligibility inputs

T-LSA-06
= strategy/horizon eligibility and decision candidate construction

T-LSA-07
= Unified Risk

T-LSA-08
= portfolio/capital/reservation business semantics

T-LSA-09
= execution and position lifecycle truth

FSAPMA
= operational provider/data business semantics

Guardian
= independent protection / restriction / crisis authority

FSTSimA
= non-Live qualification / evidence environment
```

The exact operational controller/storage/contract placement of UTM materialization remains implementation design and SHALL be assigned without creating duplicate authority ownership.

---

# 19. Relationship to the Previously Reviewed R3 Freeze

The R3 semantic freeze at:

```text
7cf8db73a9a062d7ac260b8d974e9b706ff29cd6
```

and its R3 Architecture/Consistency and `90/90` Red-Team reviews remain preserved as historical evidence for the exact six-file semantics they reviewed.

This Owner-directed hardening is a post-R3 semantic change.

Therefore:

```text
R3 PASS
!=
CURRENT PASS FOR THE CHANGED SEMANTICS
```

The new candidate semantic set is:

```text
00_GOVERNED_MARKET_QUALIFICATION_AND_EXPANSION_LIFECYCLE_CANDIDATE.md
+
00A_PRE_REVIEW_AUTHORITY_AND_OWNER_COMMAND_RUNTIME_HARDENING.md
+
00B_PRE_REVIEW_MARKET_ACCESS_SCOPE_AND_VALUE_COMPLETENESS_HARDENING.md
+
00C_PRE_RED_TEAM_BOUNDED_AUTONOMY_RESOURCE_COST_AND_RESEARCH_SECURITY_HARDENING.md
+
00D_PRE_REVIEW_CANDIDATE_ISOLATION_AND_CROSS_MARKET_REGRESSION_HARDENING.md
+
00E_PRE_REVIEW_REQUEST_IDENTITY_IDEMPOTENCY_AND_REPLAY_HARDENING.md
+
00F_USER_TRADING_MANDATE_HORIZON_AND_ADVISORY_ONLY_MARKET_HARDENING.md
```

Where `00F` is more specific than `00` or `00B` concerning user mandate, trading horizon, automated-execution availability, advisory-only market use or terminal readiness mode, `00F` controls for the new candidate review set.

A fresh semantic freeze, fresh Architecture/Consistency review and fresh Red-Team review are mandatory before renewed Owner final review.

---

# 20. Non-Grant

This hardening grants no implementation, runtime route, user-account access, provider/broker connectivity, credentials, legal status, licensing, market admission, operational advisory runtime, manual-confirmation execution, autonomous trading, Paper, Tiny Live, Live, deployment, research Internet egress, spending or self-promotion authority.

It defines candidate business semantics only.
