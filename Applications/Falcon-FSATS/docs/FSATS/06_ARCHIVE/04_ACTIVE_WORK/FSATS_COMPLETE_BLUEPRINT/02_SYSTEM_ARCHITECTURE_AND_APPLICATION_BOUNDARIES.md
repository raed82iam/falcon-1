# FSATS Complete Blueprint — System Architecture and Application Boundaries

**Candidate:** `FSATS-CB-v0.1`
**Status:** `DESIGN_CANDIDATE / NOT_OWNER_ACCEPTED / NOT_CLOSED`
**Implementation Authority:** `NOT GRANTED`

## 1. System Definition

FSATS is Falcon's governed trading-system domain. It coordinates independently governed Falcon Applications to protect, manage and grow capital through trading activity.

FSATS is not itself a Falcon Application and shall not become a hidden fifth Application by implementation convenience.

```text
FSATS
  = SYSTEM / DOMAIN BOUNDARY
  != APPLICATION
  != FOUNDATION SERVICE
  != RUNTIME PRINCIPAL
  != SHARED DATABASE OWNER
  != SHARED CREDENTIAL OWNER
  != MSA OWNER
```

## 2. Topology

```text
FALCON FOUNDATION
│
├── Falcon Self-Aware Trading Application
│   ├── MSA-T
│   └── T-LSA-01 .. T-LSA-13
│
├── Falcon Self-Aware Provider Management Application (FSAPMA)
│   ├── MSA-P
│   └── P-LSA-01 .. P-LSA-06
│
├── Falcon Trading Guardian Application
│   ├── MSA-G
│   └── G-LSA-01 .. G-LSA-04
│
├── Falcon Self-Aware Trading Simulation Application (FSTSimA)
│   ├── MSA-S
│   └── S-LSA-01 .. S-LSA-08
│
└── independent Shared Applications where separately governed
    ├── Shared Communication Application
    └── Shared Web Application

Cross-cutting governed role:
FSARM = FSATS resource-coordination role inside the accepted Foundation resource envelope
```

Every solid line above represents ownership inside one Application. Cross-Application interaction is contract-based and does not permit internal access.

## 3. Application Responsibilities

### 3.1 Falcon Self-Aware Trading Application

Owns trading business intelligence and trading state, including:

- account/environment trading context;
- market/instrument qualification for trading use;
- analysis frameworks;
- strategy catalog usage and strategy orchestration;
- opportunity identification;
- Unified Risk business decisions;
- portfolio/capital allocation and reservation;
- order intent and execution lifecycle;
- broker reconciliation;
- trading learning, analytics and attribution;
- strategy/model experimentation and evolution proposals;
- Trading-side resource awareness.

It does not own provider operational connectivity, independent Guardian protection, Foundation governance or simulation truth.

### 3.2 FSAPMA

Owns operational external information-provider management and normalized operational data products, including:

- provider registry/onboarding;
- provider service-role and API-instance capability truth;
- entitlements and quotas;
- provider selection and routing;
- data normalization and semantic consistency;
- quality verification and reconciliation;
- provider reliability/cost/capacity evidence;
- operational provider egress once separately available and authorized.

FSAPMA is the sole operational external market/provider data gateway for FSATS Applications.

```text
TRADING -> DIRECT MARKET DATA PROVIDER = PROHIBITED
GUARDIAN -> DIRECT MARKET DATA PROVIDER = PROHIBITED
FSTSIMA LIVE-OPERATIONAL PROVIDER PATH = PROHIBITED
```

### 3.3 Falcon Trading Guardian Application

Owns independent trading-system protection, crisis qualification, restriction/control commands, survival coordination and protection recovery evidence.

Guardian can observe governed evidence and issue bounded protection commands under valid authority. It does not become Trading Risk, strategy, execution truth, provider truth, FSARM or Foundation authority.

### 3.4 FSTSimA

Owns non-Live simulation, replay, synthetic market generation, fault injection, broker/exchange simulation, fidelity/calibration and independent validation evidence.

FSTSimA must remain unable to create Live financial effect merely because it can model a Live scenario.

### 3.5 FSARM

FSARM coordinates resource usage across the four Applications under the accepted Foundation coordination envelope. It is not a business decision engine and cannot create resource capacity.

Its design is detailed separately.

## 4. Three Truth Domains

The architecture deliberately separates three kinds of truth:

### Operational truth
Real provider/broker/account/environment observations used for permitted operational decisions.

### Simulation truth
Replay, synthetic, modeled, injected or paper outcomes that may support evidence but cannot masquerade as Live operational truth.

### Governance/evidence truth
Identity, authority, decision lineage, review, restriction, provenance and validation evidence.

Every material payload or stored observation must be classifiable. Ambiguous replay/simulation/operational classification fails closed at any boundary that could create financial effect.

## 5. Environment Model

Every material state/action is scoped to an explicit environment identity.

Candidate environment classes:

```text
DEV
TEST
SIMULATION
PAPER
SHADOW
TINY_LIVE
LIVE
```

Environment classes do not imply authority. A separately authorized environment instance must bind exact Application, user/account, provider/broker capability, credential references, risk envelope and lifecycle state.

Hard rule:

```text
PAPER_IDENTITY != LIVE_IDENTITY
SIMULATION_DATA != LIVE_TRUTH
SHADOW_ACTION != EXECUTION_AUTHORITY
TINY_LIVE != UNRESTRICTED_LIVE
```

## 6. Initial Market Boundary

Initial market design scope:

- US Equities;
- Crypto Spot.

Initial funded-exposure policy target:

```text
BORROWED_LEVERAGE = 0
MAX_FUNDED_EXPOSURE = 1:1
```

Derivatives, options, futures, leveraged crypto, margin expansion and other markets require separate future design/validation/Owner authorization.

Short-selling capability is disabled by default in the initial profile and may only be introduced by a separate broker/market/risk qualification and Owner decision.

## 7. Market Capability Profile

Markets are not hardcoded by name throughout the system. Each market loads a governed Market Capability Profile containing, as applicable:

- market identity and asset class;
- trading calendar/session model;
- price/quantity precision;
- minimum/maximum increments;
- supported order types/time-in-force;
- fractional-share/quantity rules;
- settlement model;
- corporate-action behavior;
- data products required;
- liquidity characteristics;
- volatility regimes;
- execution constraints;
- broker capabilities;
- regulatory/operational restrictions supplied by governed policy;
- Risk constraints;
- strategy applicability features;
- validation requirements;
- unsupported/unknown features.

A strategy or order cannot assume a capability absent from the active profile.

## 8. Dynamic Instrument Universe

FSATS does not stream expensive full-depth/high-rate data for the entire market merely to select a small set of opportunities.

The candidate uses a progressive universe funnel:

```text
CATALOG UNIVERSE
-> BROKER/PROVIDER ELIGIBLE UNIVERSE
-> CHEAP-SCAN DISCOVERY UNIVERSE
-> QUALIFIED UNIVERSE
-> DYNAMIC WATCHLIST
-> ACTIVE ANALYSIS SET
-> TRADE CANDIDATE SET
-> POSITION / ACTIVE ORDER SET
```

Each narrower tier receives richer data and more compute only when evidence justifies it.

This supports free/basic provider limits, resource efficiency and rapid dynamic replacement of instruments whose quality score falls.

### Capital-aware opportunity zones

The historical A/B/C zone idea is retained as a configurable capital-aware opportunity framework, but not as a price-only classifier.

Each zone can use a multidimensional score including:

- price relative to deployable capital;
- fractional-trading capability;
- liquidity and spread;
- volatility;
- data quality;
- execution quality;
- market/session availability;
- strategy fitness;
- recent opportunity quality;
- risk efficiency;
- broker eligibility.

A deployment profile may select approximately the top N instruments per zone, with N policy-configurable. Falling scores remove an instrument automatically after governed hysteresis/cooldown rather than preserving stale membership.

## 9. Cross-Application Data Rule

Applications exchange governed business products, not internal objects or shared tables.

Examples:

- FSAPMA publishes normalized Data Products and quality evidence.
- Trading publishes attributable trading/protection evidence required by Guardian.
- Guardian publishes bounded protection/restriction directives and state.
- Trading and other Applications publish resource demand/reclaimability evidence to FSARM.
- FSTSimA publishes validation evidence, not Live authority.

No consumer may infer more authority from a payload than its contract explicitly carries.

## 10. Data Ownership

Each state has exactly one authoritative business owner.

Examples:

```text
PROVIDER CAPABILITY TRUTH -> FSAPMA
NORMALIZED MARKET DATA PRODUCT -> FSAPMA
TRADING DECISION STATE -> TRADING APPLICATION
TRADING RISK BUSINESS DECISION -> TRADING APPLICATION / UNIFIED RISK
PORTFOLIO / CAPITAL BUSINESS STATE -> TRADING APPLICATION
BROKER ORDER INTENT -> TRADING APPLICATION
BROKER OBSERVED EXECUTION STATE -> TRADING APPLICATION AFTER RECONCILIATION
PROTECTION STATE -> TRADING GUARDIAN
SIMULATION SCENARIO / RESULT -> FSTSIMA
FOUNDATION TOTAL RESOURCE TRUTH -> FOUNDATION
FSATS EFFECTIVE RESOURCE COORDINATION -> FSARM WITHIN DELEGATED ENVELOPE
```

Caches and projections are not new authority owners.

## 11. Synchronous vs Asynchronous Interaction

The design defaults to asynchronous attributable contracts for cross-Application state propagation and event/evidence flow.

Synchronous request/reply is permitted only when the business need requires an immediate bounded answer and the Foundation route/capability supports it.

Fast execution paths must not accumulate remote calls merely because services are separately owned. The design therefore keeps most hot-path business logic within the owning Application boundary and uses precomputed/projection state where safe.

## 12. Deployment Shape

The initial implementation should prefer one independently deployable process boundary per Falcon Application, with modular-monolith internals rather than one process per LSA.

```text
APPLICATION = DEPLOYMENT / SECURITY / LIFECYCLE BOUNDARY
LSA = MAJOR INTERNAL RESPONSIBILITY / AWARENESS BOUNDARY
CSA = OPTIONAL INTELLIGENT COMPONENT AWARENESS
```

This reduces distributed-system failure modes while preserving strict cross-Application isolation. A future LSA/module may be split into a separate process only after measured performance, availability or containment evidence justifies it and the Application boundary remains valid.

## 13. No Hidden Shared Core

Reusable code is permitted only when semantic ownership is clear.

A shared library must not become:

- a hidden state owner;
- a hidden authority service;
- a cross-Application database facade;
- a bypass around Foundation communication;
- a place where unrelated business meanings are collapsed because their storage shapes match.

Cross-Application schemas may be generated from canonical governed declarations, but generated types do not create a shared runtime owner.

## 14. Failure Containment

Failure of one Application must not automatically stop all others.

Examples:

- FSAPMA degradation may force Trading into `NO_NEW_RISK` while Guardian and reconciliation continue.
- Trading MSA failure may suspend new strategy decisions while independent protection/reconciliation evidence remains preserved.
- FSTSimA failure must not impair Live/Paper trading operation.
- Guardian degradation reduces permissible trading authority and may force safer state.
- FSARM degradation prevents unproven redistribution and falls back to last valid resource assignments/Foundation-authoritative limits rather than inventing capacity.

## 15. Current Design Success Conditions

Architecture is acceptable only if:

- every business state has one owner;
- every cross-App edge is contract-governed;
- no Foundation special case is required for Trading meaning;
- every Application can be removed/replaced without redesigning Foundation;
- FSATS container owns no hidden runtime state;
- operational, simulation and governance truth cannot be silently conflated;
- resource and awareness roles do not create business authority;
- hot-path latency remains feasible without boundary bypass;
- the initial one-user/two-market profile can grow without changing the Foundation architecture.
