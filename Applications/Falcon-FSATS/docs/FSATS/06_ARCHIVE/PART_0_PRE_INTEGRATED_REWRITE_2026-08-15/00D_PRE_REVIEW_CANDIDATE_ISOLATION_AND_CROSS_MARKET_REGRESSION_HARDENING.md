# FSATS Market Qualification Candidate — Candidate Isolation and Cross-Market Regression Hardening

**Package:** `FSATS-MARKET-QUALIFICATION-PROPOSAL-001`  
**Applies To:** `00 + 00A + 00B + 00C`  
**Decision Type:** `PRE-REVIEW SHARED-ARTIFACT / REGRESSION HARDENING`  
**Status:** `CONTROLLING CANDIDATE HARDENING / NOT_OWNER_ACCEPTED / NOT_CLOSED`  
**Branch:** `application-development`  
**Implementation / Runtime / Paper / Tiny Live / Live / Deployment Authority:** `NOT_GRANTED`

---

# 1. Purpose

The central Strategy Catalog and other reusable Trading capabilities create a legitimate regression risk: a change developed to qualify Market X could accidentally alter behavior for already supported markets.

This hardening requires isolated candidate identity and cross-market regression evidence before a shared artifact change may support a new-market readiness recommendation.

---

# 2. No In-Place Mutation of Trusted/Active Shared Artifacts

A market-qualification experiment SHALL NOT directly overwrite an accepted/trusted/active shared strategy, analysis method, Risk rule, execution rule, provider product mapping or other shared artifact merely to test Market X.

Material changes shall use an isolated attributable candidate identity/version.

Conceptually:

```text
TRUSTED_SHARED_ARTIFACT_VERSION_N
        |
        +--> REMAINS UNCHANGED DURING QUALIFICATION
        |
        v
ISOLATED_CANDIDATE_VERSION_N_PLUS_1
        |
        v
FSTSIMA / GOVERNED TESTING
```

A candidate that fails is rejected/remediated without mutating the trusted predecessor.

---

# 3. Exact Applicability Scope

Every strategy/model/analysis/Risk/execution candidate created or adapted for Market X shall declare the exact applicability scope it claims.

Examples:

```text
MARKET_X_ONLY
US_EQUITIES + MARKET_X
ALL CURRENTLY SUPPORTED MARKET_PROFILE_CLASS_Y
```

The candidate SHALL NOT silently widen itself to all markets.

A market-specific parameter belongs in the correct market/profile/configuration layer when that preserves architecture and truth better than changing generic strategy logic.

---

# 4. Central Strategy Regression Rule

Strategies remain centrally registered, but central registration does not mean one modification is safe everywhere.

If a shared Strategy candidate changes logic that can affect an already supported market, qualification SHALL include regression evidence for every materially affected existing intended-use scope.

Possible outcomes include:

```text
NEW VERSION IMPROVES MARKET X AND PRESERVES EXISTING VALIDATED SCOPES
-> MAY CONTINUE THROUGH GOVERNED REVIEW

NEW VERSION HELPS MARKET X BUT DEGRADES EXISTING MARKET Y
-> DO NOT SILENTLY REPLACE THE CURRENT VERSION
-> RE-DESIGN / NARROW APPLICABILITY / MAINTAIN SEPARATE GOVERNED VERSION OR REJECT
```

Version coexistence, applicability predicates or configuration specialization may be used only through the separately governed strategy lifecycle and SHALL NOT become uncontrolled duplication.

---

# 5. Analysis / Risk / Execution Regression

The same rule applies to reusable shared Trading capabilities.

A candidate change intended for Market X SHALL be regression-tested against materially affected existing scopes when it changes shared:

- analysis logic;
- feature/model behavior;
- Unified Risk logic or common envelopes;
- portfolio/capital coordination;
- execution/reconciliation logic;
- common strategy orchestration behavior;
- data-semantic interpretation.

Market-specific facts/constraints should remain scoped to Market X when possible rather than mutating global behavior.

---

# 6. Cross-Market Interaction Testing

Market X may be individually safe but harmful when combined with existing markets.

FSTSimA qualification shall therefore include cross-market interaction testing where material, including as applicable:

- correlated losses;
- capital competition/reservation;
- liquidity stress occurring simultaneously;
- shared provider/broker capacity pressure;
- resource contention;
- common-factor exposure;
- currency/funding interactions;
- simultaneous crisis/reconciliation load;
- strategy conflicts across markets.

```text
MARKET_X_STANDALONE_PASS != MULTI_MARKET_SYSTEM_PASS
```

---

# 7. Evidence Binding and Staleness

A PASS shall bind to exact:

- candidate identity/version/digest;
- applicability scope;
- Market Profile version;
- Risk version;
- strategy/model version;
- relevant provider/broker capability evidence;
- scenario/evidence set.

Changing any material shared artifact or applicability scope makes affected regression evidence stale and requires re-evaluation.

---

# 8. No Backdoor Promotion Through Market Qualification

A candidate that passes Market X qualification does not automatically replace its existing trusted predecessor.

```text
MARKET_X QUALIFICATION PASS
!=
SHARED ARTIFACT PRODUCTION REPLACEMENT AUTHORITY
```

Shared artifact adoption remains subject to its normal origin-aware Application review, FSA/governance compatibility review where required, and separate Owner/governance adoption authority.

---

# 9. Reviewed-Candidate Composition Update

The candidate requiring fresh review is now:

```text
00
+ 00A
+ 00B
+ 00C
+ 00D
```

All earlier review evidence is historical for its exact earlier freeze after this semantic addition.

Fresh Architecture/Consistency and fresh Red-Team must bind to the exact commit containing all five candidate files unchanged.

---

# 10. Non-Grant

This hardening grants no implementation, runtime, market admission, shared-artifact replacement, provider/broker connectivity, Paper, Tiny Live, Live or deployment authority.
