# FSATS Complete Blueprint — User, Account, Market and Growth Model

**Candidate:** `FSATS-CB-v0.1`
**Status:** `DESIGN_CANDIDATE / NOT_OWNER_ACCEPTED / NOT_CLOSED`
**Implementation Authority:** `NOT GRANTED`

## 1. Purpose

FSATS begins intentionally small but must not require architectural surgery when evidence justifies expansion.

Initial rollout principle:

```text
1 OWNER USER
+ 2 INITIAL MARKETS
+ 1 INITIAL PAPER BROKER PROFILE
-> PROVE SYSTEM
-> ADD MARKET / BROKER CAPABILITY
-> ADD TINY LIVE/LIVE ONLY UNDER AUTHORITY
-> ADD USERS ONLY AFTER PRODUCT / LEGAL / SECURITY READINESS
```

## 2. User Model

Initial system has one Owner user. Multi-user is architecturally supported but not an initial operational requirement.

A future `UserId` is an explicit business/security scope attached to account, portfolio, policy, evidence and commands where relevant.

The initial single-user implementation must not use `global static current user` assumptions that make future isolation impossible.

## 3. Account Model

An account is not interchangeable with a user or broker.

Canonical relation:

```text
USER
-> BROKER RELATIONSHIP
-> BROKER ACCOUNT
-> ENVIRONMENT
-> MARKET / CAPABILITY PROFILE
```

One user may eventually have multiple broker accounts. One broker may expose Paper and Live accounts with separate identities/credentials.

## 4. Portfolio Scope

Portfolio state is always scoped to an exact user/account/environment combination or a separately governed aggregate view.

No capital is silently pooled across users or legal accounts.

Future cross-account portfolio optimization must use explicit allocation/authority rules.

## 5. Environment Separation

The following must remain separately identifiable even for the same user/broker:

- Paper;
- Shadow;
- Tiny Live;
- Live.

Test/simulation capital and Live capital cannot share authoritative ledger rows merely because the numeric amounts match.

## 6. Multi-Market Architecture

Market expansion uses configuration/profile + provider/broker capability + validation, not a new copy of the entire Trading Application.

One Trading Application can operate multiple governed market profiles while preserving market-specific state and constraints.

Market-specific strategies are created only when logic is genuinely market-specific. Otherwise one central strategy identity carries applicability rules.

## 7. Cross-Market Capital

Portfolio/Capital Management owns allocation across active markets.

The initial target may distribute capital conservatively across US Equities and Crypto based on current opportunity/risk evidence rather than a permanent 50/50 rule.

A deployment profile may start with a simple Owner-approved distribution for validation, but the architecture supports dynamic allocation within hard global/market ceilings.

## 8. Market Allocation Inputs

Potential inputs:

- opportunity quality;
- expected risk-adjusted return;
- drawdown state;
- liquidity;
- strategy coverage/fitness;
- diversification/correlation;
- provider/broker health;
- session availability;
- execution quality;
- capital efficiency;
- uncertainty;
- Guardian/Risk posture.

Dynamic allocation cannot exceed Owner/governed market/account limits.

## 9. Instrument Scaling

The dynamic universe architecture allows thousands of catalog instruments without simultaneously applying rich analysis and streaming to all of them.

Scaling controls include:

- staged universe tiers;
- per-provider subscription budgets;
- batch scanning;
- incremental feature computation;
- event-driven promotion/demotion;
- hot-set caching;
- resource-aware scheduling;
- FSARM resource signals.

## 10. Provider Scaling

Providers are additive by Service Role, not one monolithic `MarketDataProvider` switch.

Example future deployment can use:

- Provider A for cheap historical bars;
- Provider B for real-time equities;
- Provider C for crypto order book;
- Provider D as validation/fallback source.

FSAPMA selects routes based on current capability/quality/quota/cost.

## 11. Broker Scaling

Trading Execution remains broker-independent at domain level.

Adding a broker creates:

- capability profile;
- adapter;
- canonical state mapping;
- account model;
- environment credential references;
- integration tests;
- reconciliation tests;
- performance/failure evidence.

Strategies and Unified Risk do not depend on broker SDK types.

## 12. Strategy Scaling

Strategy Catalog supports:

- multiple strategy versions;
- lifecycle states;
- market applicability;
- school membership;
- validation evidence;
- resource/capital requirements;
- controlled retirement.

Only active/eligible strategy versions load into decision orchestration for a given market/environment.

## 13. AI Model Scaling

AI models are accessed through bounded component interfaces rather than hardcoded one-provider assumptions.

Model profiles may include:

- model/provider identity;
- capability;
- latency/cost;
- context/output limits;
- privacy/data policy;
- local/remote classification;
- evaluation evidence;
- tool permissions;
- current health.

Changing model provider does not change Awareness authority.

## 14. Multi-User Future Isolation

Before user count exceeds one, implementation must prove:

- user/account identity isolation;
- authorization/role model;
- data isolation;
- credential isolation;
- portfolio/capital isolation;
- rate/resource fairness;
- evidence/audit attribution;
- command ownership;
- incident blast radius;
- deletion/retention policy;
- legal/compliance readiness appropriate to intended operation.

The current design does not grant public/commercial service authority.

## 15. Resource Fairness

Future multi-user operation may require a second-level Application business allocator below Foundation/FSARM resource governance.

It must not allow one user to starve protection/reconciliation obligations of another user or mint Foundation priority.

This is future scope and must be designed when multi-user authorization exists.

## 16. Shared Web Application Boundary

The Shared Web Application remains outside FSATS ownership.

FSATS may expose governed read models and commands to it, but Web does not receive direct database access or become trading authority.

Future dashboard views may include:

- system/application readiness;
- portfolios/positions/orders;
- strategy state;
- provider health;
- Guardian state;
- resource state;
- simulation/validation evidence;
- AI/Awareness state;
- Owner decision requests.

Any mutating command requires exact authentication/authorization and governed contract semantics.

## 17. Shared Communication Application Boundary

Notifications/communications remain separate from trading authority.

FSATS publishes notification intents/events; the Shared Communication Application decides delivery mechanics according to its own governed responsibility.

Failure to send a notification does not silently change trading policy unless an explicit policy says a required Owner communication path is a prerequisite for the action.

## 18. Deployment Growth

Initial deployment may run the four Application processes and supporting databases on one sufficiently provisioned host/VM while preserving logical security/process/data boundaries.

Future distribution across machines/containers is allowed without changing business ownership.

Physical colocation is not shared authority.

## 19. Containerization

Containerization may be used when it improves reproducibility/isolation/operations, but is not an architectural requirement for every local development action.

The first concern is reproducible governed application boundaries, not maximizing orchestration complexity.

## 20. Operational Profiles

Configuration profiles should support at least:

```text
LOCAL_DEVELOPMENT
CI_VALIDATION
SIMULATION
PAPER
SHADOW
TINY_LIVE
LIVE
```

Each profile has explicit disabled capabilities. Higher-consequence profiles are not enabled by copying a lower profile and flipping an undocumented flag.

## 21. Initial Cost Discipline

Early validation should prefer free/low-cost provider capability where it does not invalidate the experiment.

Cost optimization never justifies:

- using stale/insufficient data for a claim requiring better data;
- violating provider terms;
- bypassing security;
- hiding data-coverage limitations;
- treating one-venue equity data as full-market consolidated truth.

## 22. Commercialization Boundary

The initial architecture can become multi-user, but proving the technology does not itself authorize commercial service.

Before commercial users:

- legal/licensing analysis;
- security/privacy readiness;
- support/operations model;
- billing/subscription if needed;
- user agreement/risk disclosures as applicable;
- stronger HA/DR objectives;
- incident-response maturity;
- independent security testing;
- capacity planning;
- Owner/governance decision.

These are future requirements, not current implementation authority.

## 23. Acceptance Gates

```text
SINGLE_USER_HARDCODE_PREVENTS_FUTURE_ISOLATION = 0
CROSS_USER_CAPITAL_POOL_BY_ACCIDENT = 0
MARKET_COPY_OF_WHOLE_TRADING_APP = 0
BROKER_SDK_LEAKS_INTO_STRATEGY_DOMAIN = 0
PROVIDER_SDK_LEAKS_INTO_TRADING_DOMAIN = 0
PHYSICAL_COLOCATION_AS_SHARED_AUTHORITY = 0
MULTI_USER_COMMERCIAL_AUTHORITY_BY_TECHNICAL_READINESS = 0
```
