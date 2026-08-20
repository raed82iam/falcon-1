# FSATS Specialized Implementation Architecture — System and Application Topology Specification

**Package:** `FSATS-SIA-v0.1`
**Status:** `DESIGN_CANDIDATE`

## 1. Purpose

Define the exact runtime ownership topology before code/project decomposition. This file answers: what is an Application, what is only a grouping, who owns each business state, where each MSA/LSA belongs, and what cross-boundary access is forbidden.

## 2. Canonical System Boundary

```text
Falcon OS / Foundation
└── Falcon Applications
    ├── FSATS domain grouping (NON-OWNING)
    │   ├── Falcon Self-Aware Trading Application
    │   ├── Falcon Self-Aware Provider Management Application (FSAPMA)
    │   ├── Falcon Trading Guardian Application
    │   └── Falcon Self-Aware Trading Simulation Application (FSTSimA)
    └── Other Falcon Applications / domains
```

`FSATS` is a system/domain boundary and documentation grouping. It SHALL NOT by its name alone become:

- an APP-001 Application;
- a lifecycle principal;
- an MSA/LSA owner;
- an authoritative database owner;
- a hidden Service Bus endpoint;
- a hidden permission principal;
- a Foundation resource grant holder;
- an execution identity.

A future FSATS-level runtime coordinator must have an explicit governed identity and placement. FSARM is handled separately in `11_FSARM_SPECIALIZED_RESOURCE_ARCHITECTURE.md`.

## 3. Canonical Application Set

### APP-TRD — Falcon Self-Aware Trading Application

**Purpose:** convert governed market/data evidence into disciplined trading decisions and manage the complete Trading business lifecycle within explicit risk/capital/execution authority.

Owns:

- market/instrument business universe;
- analysis/feature/strategy business semantics;
- trade proposals and decision orchestration;
- Unified Risk business semantics;
- portfolio/capital business state;
- capital reservations;
- execution intent and order/position business lifecycle;
- Trading learning/analytics/evolution candidates;
- Trading-local resource demand/effect knowledge;
- exactly one Trading MSA and 13 Trading LSAs.

Does not own:

- external provider operational access/data acquisition orchestration;
- independent Guardian protection authority;
- Foundation resource truth/grants;
- FSATS-wide resource redistribution;
- FSA/Foundation governance;
- Foundation transport/lifecycle/security primitives.

### APP-PMA — Falcon Self-Aware Provider Management Application (FSAPMA)

**Purpose:** sole operational external market/reference/provider-data management Application for FSATS.

Owns:

- provider registry/onboarding;
- provider capability/entitlement/business availability;
- Data Product semantics and normalization;
- provider route selection/business routing;
- provider quality/reconciliation;
- provider quota/cost/reliability business state;
- exactly one FSAPMA MSA and 6 FSAPMA LSAs.

Does not own:

- Trading strategy/business decisions;
- Trading Risk/capital/portfolio;
- Guardian authority;
- Foundation network/credential/total-resource authority;
- FSARM resource authority beyond reporting its own needs/reclaimability.

### APP-GRD — Falcon Trading Guardian Application

**Purpose:** independent Trading-domain protection, incident qualification, restriction, crisis coordination and protection recovery evidence.

Owns:

- Trading protection incident state;
- Guardian protection directives/commands within granted authority;
- crisis/protection state;
- protection reconciliation and recovery evidence;
- exactly one Guardian MSA and 4 Guardian LSAs.

Does not own:

- Trading alpha/strategy/portfolio optimization;
- Unified Risk calculations as a business owner;
- order execution truth;
- provider truth;
- Foundation Guardian/FSA controls;
- Foundation resource governance;
- FSARM resource redistribution.

### APP-SIM — Falcon Self-Aware Trading Simulation Application (FSTSimA)

**Purpose:** non-Live simulation, shadow/replay, scenario generation, fault injection, fidelity assessment and independent validation evidence.

Owns:

- simulation time/scenario state;
- synthetic/replayed market environment;
- simulated provider/external-service behavior;
- simulated broker/exchange/execution;
- simulated account/capital/settlement;
- fault/latency/crisis injection;
- simulator fidelity/calibration;
- independent oracle/reproducibility/validation assessment;
- exactly one FSTSimA MSA and 8 FSTSimA LSAs.

Does not own:

- Live authority;
- production promotion/adoption;
- operational provider or broker credentials;
- authoritative Trading positions/capital;
- Guardian production authority;
- Foundation lifecycle/security/resource authority.

## 4. Awareness Topology

```text
APP-TRD
└── MSA-TRD
    ├── T-LSA-01 Operations, Account & Environment
    ├── T-LSA-02 Market & Instrument Universe
    ├── T-LSA-03 Analysis Frameworks
    ├── T-LSA-04 Classical Trading School
    ├── T-LSA-05 Opportunity Hunting School
    ├── T-LSA-06 Strategy Orchestration & Decision
    ├── T-LSA-07 Unified Risk Management
    ├── T-LSA-08 Portfolio & Capital Management
    ├── T-LSA-09 Execution & Position Lifecycle
    ├── T-LSA-10 Trading Learning & Knowledge
    ├── T-LSA-11 Trading Analytics & Attribution
    ├── T-LSA-12 Strategy Evolution & Experimentation
    └── T-LSA-13 Trading Resource Awareness & Evaluation

APP-PMA
└── MSA-PMA
    ├── P-LSA-01 Provider Registry & Onboarding
    ├── P-LSA-02 Data Products, Semantics & Normalization
    ├── P-LSA-03 Provider Capability, Account & Entitlement
    ├── P-LSA-04 Provider Selection, Routing & Delivery
    ├── P-LSA-05 Data Quality, Verification & Reconciliation
    └── P-LSA-06 Quota, Capacity, Cost & Reliability

APP-GRD
└── MSA-GRD
    ├── G-LSA-01 Protection Observation & Incident Qualification
    ├── G-LSA-02 Protection Scope, Restriction & Command Governance
    ├── G-LSA-03 Crisis State, Survival & Protection Coordination
    └── G-LSA-04 Reconciliation, Recovery & Protection Evidence

APP-SIM
└── MSA-SIM
    ├── S-LSA-01 Simulation Time & Scenario
    ├── S-LSA-02 Market Environment Simulation
    ├── S-LSA-03 Provider & External Service Simulation
    ├── S-LSA-04 Broker, Exchange & Execution Simulation
    ├── S-LSA-05 Account, Capital & Settlement Simulation
    ├── S-LSA-06 Fault, Latency & Crisis Injection
    ├── S-LSA-07 Fidelity & Calibration
    └── S-LSA-08 Oracle, Evidence, Reproducibility & Validation Assessment
```

Invariants:

```text
MSA_COUNT_PER_APPLICATION = 1
LSA_COUNT_PER_DECLARED_MAJOR_BRANCH = 1
CSA = OPTIONAL / ELIGIBILITY_REQUIRED
FSATS_MSA_COUNT = 0
FSATS_LSA_COUNT = 0
AWARENESS_RANK != AUTHORITY
```

## 5. Canonical Ownership Rule

Every authoritative business aggregate SHALL have exactly one owning Application and one primary owning LSA/component.

No authoritative aggregate may be co-owned by two Applications.

Cross-Application consumers receive only governed contract projections/events/commands; they do not mutate the producer's internal aggregate directly.

## 6. Cross-Application Access Matrix

| Source | Target | Direct internal access | Governed contract access |
|---|---|---:|---:|
| Trading | FSAPMA | FORBIDDEN | REQUIRED |
| Trading | Guardian | FORBIDDEN | REQUIRED |
| Trading | FSTSimA | FORBIDDEN | REQUIRED |
| FSAPMA | Trading | FORBIDDEN | REQUIRED |
| FSAPMA | Guardian | FORBIDDEN | REQUIRED |
| FSAPMA | FSTSimA | FORBIDDEN | REQUIRED |
| Guardian | Trading | FORBIDDEN | REQUIRED |
| Guardian | FSAPMA | FORBIDDEN | REQUIRED |
| Guardian | FSTSimA | FORBIDDEN | REQUIRED where needed |
| FSTSimA | any production Application | FORBIDDEN | only explicitly non-authoritative simulation/replay/validation contract |

Forbidden direct access includes another Application's:

- database/schema;
- files;
- memory/cache;
- dependency injection container;
- internal service object;
- private API;
- credentials;
- local queue;
- unpublished event stream;
- resource allocator internals.

## 7. FSAPMA Data Gateway Invariant

Operational external market/reference/provider data for FSATS SHALL enter through FSAPMA.

```text
EXTERNAL OPERATIONAL DATA
-> governed Foundation egress boundary when available
-> FSAPMA provider adapter
-> FSAPMA normalization / quality / reconciliation
-> canonical governed Data Product contract
-> authorized consumer Application
```

Trading/Guardian SHALL NOT bypass FSAPMA to solve provider latency or quota problems.

Research material is not operational market data and follows the separate awareness/FSTSimA research boundary.

## 8. Trading Decision Invariant

```text
DATA PRODUCT / ACCOUNT / MARKET CONTEXT
-> ANALYSIS / FEATURES
-> STRATEGY EVALUATIONS
-> ORCHESTRATION / CONFLICT RESOLUTION
-> TRADE PROPOSAL
-> UNIFIED RISK
-> CAPITAL RESERVATION / PORTFOLIO CHECK
-> EXECUTION INTENT
-> BROKER ADAPTER / RECONCILIATION
-> POSITION / ORDER STATE
```

No strategy, AI, provider adapter or Guardian may skip this pipeline to submit an ordinary Trading order.

Independent Guardian protection commands may restrict/cancel/contain according to explicit authority and contract, but do not become ordinary strategy execution.

## 9. Risk / Guardian Separation

```text
UNIFIED RISK = BUSINESS EXPOSURE EVALUATION AND TRADING CONTROL
GUARDIAN = INDEPENDENT PROTECTION / INCIDENT / RESTRICTION / CRISIS AUTHORITY
```

Unified Risk can decline or resize a proposal because it violates business risk rules.

Guardian can independently restrict/suspend/cancel/contain an affected scope when its explicit protection authority and incident conditions are satisfied.

Neither substitutes for the other.

## 10. FSARM Resource Topology

Conceptual current relation:

```text
Foundation Resource Governance
          <->
         FSARM
     /     |      |      \
Trading  FSAPMA Guardian FSTSimA
```

Rules:

- Foundation retains total-resource truth and final grants/ceilings;
- constituent Applications retain exact identity/accounting;
- FSARM owns bounded FSATS-wide effective coordination only;
- T-LSA-13/P-LSA-06/G-LSA-03/FSTSimA resource reporters provide Application-owned demand/effect evidence;
- no constituent Application independently performs FSATS-wide redistribution;
- no resource request is a grant.

## 11. Application Failure Isolation

A failure in one Application SHALL NOT permit:

- corruption of another Application's authoritative state;
- privilege escalation;
- bypass of Foundation admission/security;
- conversion of replay/simulation traffic into authoritative traffic;
- collapse of FSATS into one shared mutable database/process owner;
- silent resource takeover.

Cross-Application failure is handled by explicit degraded/unavailable contract outcomes and protection/resource policies.

## 12. Removal / Replacement Invariant

Every Application SHALL be replaceable/removable without Foundation redesign.

Removal readiness must reconcile:

- routes/contracts;
- active dependencies;
- outstanding commands/requests;
- resource allocation/accounting;
- retained evidence;
- credential references;
- durable business state;
- lifecycle/recovery obligations;
- awareness identities/proposals;
- outstanding capital/order/position consequences where applicable.

A business obligation that cannot be safely abandoned blocks ordinary removal until separately reconciled.

## 13. Shared Libraries Rule

A shared Application-owned library MAY exist only for truly identical domain semantics.

It SHALL NOT:

- become a fifth hidden Application;
- own mutable authoritative cross-Application state;
- host Foundation substitutes;
- create hidden cross-Application service calls;
- own a singletons-based global controller merely for convenience.

Shared types must retain explicit owning semantic domains and contracts.

## 14. Acceptance Fixtures

The topology verifier SHALL reject at least:

1. FSATS declared as an Application without explicit Owner/governed decision;
2. any Application with zero or multiple MSAs;
3. any declared major branch without exactly one LSA;
4. a CSA with no eligible intelligent component/parent LSA;
5. Trading direct provider operational egress;
6. strategy direct broker execution;
7. Guardian ownership of Unified Risk;
8. FSARM ownership of Foundation total-resource truth;
9. direct project/runtime access to another Application internals;
10. FSTSimA authoritative Live traffic;
11. one mutable database aggregate co-owned by multiple Applications;
12. route existence treated as business authority;
13. hidden FSATS runtime principal created by package/namespace name only.

## 15. Topology Decision Markers

```text
FSATS = NON_OWNING_SYSTEM_BOUNDARY
APPLICATION_COUNT_IN_CURRENT_FSATS = 4
MSA_COUNT = 4
LSA_COUNT = 31
DIRECT_CROSS_APPLICATION_INTERNAL_ACCESS = FORBIDDEN
OPERATIONAL_PROVIDER_DATA_GATEWAY = FSAPMA
TRADING_RISK_OWNER = APP-TRD
PROTECTION_OWNER = APP-GRD
NON_LIVE_VALIDATION_OWNER = APP-SIM
FSATS_WIDE_RESOURCE_COORDINATION = FSARM / BOUNDED
FOUNDATION_TOTAL_RESOURCE_AUTHORITY = FOUNDATION
```
