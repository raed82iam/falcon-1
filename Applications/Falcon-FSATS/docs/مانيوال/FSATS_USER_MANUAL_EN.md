# Falcon Self-Aware Trading System (FSATS) — User Manual

**Edition:** 2026-08-19  
**Language:** English  
**Audience:** Falcon owners, traders, operators, supervisors, reviewers, and non-developer users  
**System:** Falcon Self-Aware Trading System (FSATS)  
**Current posture:** Application implementation and onboarding preparation are technically verified; actual Foundation Admission, canonical Runtime Registration, Activation, provider/broker connectivity, Paper, Shadow, Tiny-Live, Live trading, and deployment remain separately governed and are not implied by this manual.

> This manual explains what FSATS is, how a user should understand its outputs and controls, and how to interact with it safely. It does not create trading authority, activation authority, deployment authority, or financial advice.

---

# 1. What FSATS is

FSATS is Falcon's governed self-aware trading system. It is designed to analyze, simulate, protect, coordinate, and eventually execute trading workflows while preserving strict separation between analysis, recommendation, authority, and execution.

FSATS is not one giant program. It is a governed system boundary composed of five independently governed Falcon Applications:

```text
1. Trading Application
2. FSAPMA — Falcon Self-Aware Provider Management Application
3. Falcon Trading Guardian Application
4. FSTSimA — Falcon Trading Simulation Application
5. APP-RSC — FSATS Resource Coordination Application
```

FSATS itself is a non-owning system boundary. The five Applications own their own responsibilities.

---

# 2. What each Application does

## 2.1 Trading Application

The Trading Application owns trading-domain intelligence and trading workflow decisions. It is responsible for areas such as:

- market interpretation;
- strategy selection and coordination;
- trading opportunity evaluation;
- broker-account-scoped trading workflow;
- trade proposal and execution preparation;
- trading-domain state and evidence;
- risk-aware decision composition inside its authorized scope.

A trading decision is not automatically an executed order.

```text
TRADING_DECISION != BROKER_EXECUTION
```

## 2.2 FSAPMA

FSAPMA manages operational provider access for FSATS. Its responsibility includes:

- provider capability awareness;
- provider suitability;
- quotas and rate limits;
- provider route readiness;
- provider health and failure handling;
- operational market-data provider coordination.

FSAPMA does not become Trading merely because Trading consumes its data.

```text
PROVIDER_DATA != TRADING_AUTHORITY
```

## 2.3 Trading Guardian

Trading Guardian is the trading-domain protection Application. It protects the trading system by monitoring governed protection conditions and supporting restriction, containment, and safe operating decisions within its authorized scope.

Guardian protection does not become portfolio strategy or broker authority.

```text
PROTECTION != TRADING_STRATEGY
PROTECTION != BROKER_AUTHORITY
```

## 2.4 FSTSimA

FSTSimA is the independent governed trading simulation Application. It supports simulation, deterministic scenarios, fault injection, Digital City validation, calibration, replay, and non-Live qualification evidence.

Simulation results are evidence, not operational truth.

```text
SIMULATION_RESULT != LIVE_MARKET_TRUTH
SIMULATION_PASS != PAPER_AUTHORITY
SIMULATION_PASS != LIVE_AUTHORITY
```

## 2.5 APP-RSC

APP-RSC coordinates FSATS-side resource needs within the FSATS domain. It helps the FSATS Applications understand and coordinate resource demand and degraded behavior.

APP-RSC is not Falcon Foundation Resource Governance.

```text
APP-RSC != FOUNDATION_RESOURCE_GOVERNANCE
```

---

# 3. Self-Awareness in FSATS

FSATS uses bounded self-awareness.

Current Application topology:

```text
Trading:          1 MSA / 13 LSA / 3 CSA
FSAPMA:           1 MSA /  6 LSA / 1 CSA
Trading Guardian: 1 MSA /  4 LSA / 1 CSA
FSTSimA:          1 MSA /  8 LSA / 2 CSA
APP-RSC:          1 MSA /  3 LSA / 0 CSA initially

TOTAL: 5 Applications / 5 MSA / 34 LSA / 7 CSA
```

The levels mean:

- **MSA** understands one whole Application.
- **LSA** understands one major branch or subsystem inside an Application.
- **CSA** understands one eligible intelligent component.
- **FSA** is Foundation-level and is not inside FSATS Applications.

The escalation direction is conceptually:

```text
CSA -> LSA -> MSA -> FSA review where applicable
```

Self-awareness does not create authority.

```text
SELF_AWARENESS != AUTHORITY
```

---

# 4. The identity model a user must understand

FSATS trading operations are scoped to a broker account, not to an internal FSATS user identity.

The controlling business identity is:

```text
BrokerId + BrokerAccountId
```

Environment is an additional identity dimension where material.

FSATS does not own customer identity, username, customer profile, or contact mapping. Those mappings belong to the Shared Web boundary.

This prevents one broker account from being confused with another.

---

# 5. How to read FSATS states

FSATS deliberately distinguishes states that ordinary trading software often mixes together.

## 5.1 Ready does not mean running

```text
RUNTIME_READY != RUNTIME_AUTHORIZED
ACTIVATION_ELIGIBLE != ACTIVE
ADMISSION_READY != ADMITTED
```

## 5.2 Registered does not mean active

```text
RUNTIME_REGISTERED != ACTIVATED
```

## 5.3 A route existing does not mean a connection was opened

```text
ROUTE_EXISTS != CONNECTION_AUTHORIZED
ROUTE_AUTHORIZED != CONNECTION_EXECUTED
```

## 5.4 Data access does not mean permission to trade

```text
DATA_ACCESS != BUSINESS_AUTHORITY
PROVIDER_CONNECTIVITY != EXECUTION_AUTHORITY
```

## 5.5 A request is not an outcome

```text
REQUEST_SENT != ACTION_ACCEPTED != ACTION_COMPLETED
```

Whenever the interface shows one of these stages, treat it literally.

---

# 6. Current system status

At the current documented baseline:

- Parts 0 through 10 are Project Owner accepted and closed.
- Part 11 Application-side onboarding preparation has been implemented and technically verified.
- Five Admission candidate packages and five Runtime Registration templates are materialized.
- Foundation-side preparation has verified the generic plug-ready contract path by composition.
- No Foundation redesign is required.
- No Application redesign is required for the current preparation package.

However:

```text
ACTUAL_ADMISSION                      = NOT AUTHORIZED / NOT EXECUTED
ACTUAL_CANONICAL_RUNTIME_REGISTRATION = NOT AUTHORIZED / NOT EXECUTED
RUNTIME_ACTIVATION                    = NOT AUTHORIZED / NOT EXECUTED
PROVIDER_CONNECTIVITY                 = NOT AUTHORIZED / NOT EXECUTED
BROKER_CONNECTIVITY                   = NOT AUTHORIZED / NOT EXECUTED
PAPER                                 = NOT AUTHORIZED
SHADOW                                = NOT AUTHORIZED
TINY-LIVE                             = NOT AUTHORIZED
LIVE                                  = NOT AUTHORIZED
DEPLOYMENT                            = NOT AUTHORIZED
```

This is an important user-facing distinction: technical readiness is not production activation.

---

# 7. What a user should expect from the interface

The Shared Web Application is the presentation and user-interaction surface. FSATS remains the trading-domain backend boundary.

Depending on which separately governed Web features are enabled, a user may eventually see surfaces for:

- system status;
- broker-account selection/context;
- market and instrument information;
- trading analysis;
- strategy-related outputs;
- Guardian protection state;
- simulation and qualification results;
- resource/degradation status;
- provider status;
- alerts, decisions, and evidence;
- Owner-controlled actions;
- operational history and audit information.

A screen, button, or displayed route does not itself create authority.

```text
PRESENTATION != AUTHORITY
```

---

# 8. Understanding recommendations and decisions

FSATS outputs should be read using four different concepts:

## Analysis

What the system currently observes or infers.

## Recommendation

What the system proposes as a possible action.

## Authority

Whether the proposed action is actually permitted.

## Execution

Whether the permitted action was actually performed and confirmed.

Never collapse them into one state.

Example:

```text
Analysis: opportunity detected
Recommendation: BUY candidate
Authority: not yet granted
Execution: none
```

That is not a trade.

---

# 9. Guardian states

Guardian may expose protection-oriented states such as normal, degraded, restricted, contained, or other governed conditions.

Users must preserve these distinctions:

```text
SAFE_STATE != NORMAL_OPERATION
CONTAINED != RELEASED
PROTECTIVE_RESTRICTION != BUSINESS_AUTHORITY
```

A restriction should never be bypassed merely because market conditions look attractive.

---

# 10. Simulation and Digital City results

FSTSimA is intentionally separated from operational trading.

A user may see simulation evidence for:

- strategies;
- failure scenarios;
- degraded operation;
- deterministic replay;
- calibration;
- fault injection;
- scenario comparison;
- Digital City validation.

Use simulation to understand behavior and qualification evidence.

Do not interpret it as proof that the same result will occur in a live market.

---

# 11. Providers and market data

When provider functionality is authorized in the future, FSAPMA will coordinate operational provider access according to governed provider capabilities, quota, route, health, and failure rules.

Users should understand:

```text
PROVIDER_AVAILABLE != PROVIDER_AUTHORIZED
PROVIDER_AUTHORIZED != CONNECTION_EXECUTED
DATA_RECEIVED != TRADE_AUTHORIZED
```

Stale, unavailable, unknown, invalid, or conflicting data must remain visible as such where material. It should not silently become trusted current truth.

---

# 12. Broker execution

Broker execution is a distinct final operational boundary.

Even if Trading has a valid proposal and market data is available:

```text
BROKER_ROUTE_EXISTS != ORDER_AUTHORIZED
ORDER_AUTHORIZED != ORDER_SENT
ORDER_SENT != BROKER_ACCEPTED
BROKER_ACCEPTED != FILLED
```

The user interface should present these states separately when broker execution is eventually enabled.

---

# 13. Risk and protection expectations

FSATS is designed around capital protection as well as opportunity management.

Users should expect the system to prefer a governed hold, restriction, denial, or degraded state over pretending uncertain information is safe.

The operating principle is:

```text
UNKNOWN OR AMBIGUOUS AUTHORITY -> DENY / HOLD
```

This can make the system appear conservative. That is intentional.

---

# 14. Updates and changes

Material FSATS changes are versioned and revalidated. A previous PASS does not automatically cover new semantics.

For users, this means:

- a newer version may require new validation;
- an old accepted result should not be assumed current after a material change;
- silent upgrades are not an accepted authority shortcut;
- release and activation remain governed decisions.

```text
COMPATIBLE_UPDATE != SILENT_UPGRADE_AUTHORITY
```

---

# 15. What to do when the system says Unknown, Stale, Degraded, or Denied

Do not try to interpret these states as hidden approval.

- **Unknown** means the required fact is not established.
- **Stale** means evidence may no longer be current enough.
- **Degraded** means the system can operate only within a reduced safe envelope.
- **Denied** means the requested action did not satisfy a governed gate.
- **Held** means progression is intentionally paused pending evidence or authority.

When trust or authority is required:

```text
UNKNOWN != YES
STALE != CURRENT
DENIED != RETRY UNTIL ACCEPTED
```

---

# 16. User safety checklist before relying on an FSATS action

Before treating any significant trading action as executable, confirm the interface clearly distinguishes and establishes, where applicable:

- correct broker account;
- correct market/instrument;
- current data state;
- current strategy/decision state;
- current Guardian state;
- current resource state;
- current provider state;
- current broker route state;
- explicit authority state;
- execution status;
- broker confirmation status;
- evidence/audit references.

If any authority-critical item is Unknown, do not infer approval.

---

# 17. What FSATS does not promise

FSATS does not promise:

- guaranteed profit;
- perfect prediction;
- uninterrupted provider availability;
- uninterrupted broker availability;
- zero market risk;
- that simulation equals live performance;
- that a technically valid action is automatically authorized;
- that a successful request is automatically completed.

Its design goal is governed, explainable, auditable, bounded trading operation, not magical certainty.

---

# 18. Quick glossary

**FSATS:** Falcon Self-Aware Trading System.  
**Trading:** trading intelligence and trading workflow Application.  
**FSAPMA:** provider management Application for operational FSATS data access.  
**Trading Guardian:** trading-domain protection Application.  
**FSTSimA:** governed simulation and Digital City validation Application.  
**APP-RSC:** FSATS-side resource coordination Application.  
**MSA:** awareness for one whole Application.  
**LSA:** awareness for one major branch.  
**CSA:** awareness for one eligible intelligent component.  
**FSA:** Foundation Self-Awareness, outside Applications.  
**Admission:** Foundation acceptance of an Application candidate. Not activation.  
**Runtime Registration:** technical registration into Foundation hosting. Not activation.  
**Activation:** separate authority to become active.  
**Fail closed:** uncertain or invalid authority-critical state results in deny/hold, not automatic permission.

---

# 19. Final user rule

The simplest way to use FSATS correctly is to keep four questions separate:

```text
What does Falcon know?
What does Falcon recommend?
What is Falcon authorized to do?
What did Falcon actually execute and confirm?
```

If those four answers are not the same, do not treat them as the same.

**FSATS is designed to make that separation visible, governed, explainable, and auditable.**