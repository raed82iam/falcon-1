# FSATS Complete Blueprint — Repository, Implementation and Verification Plan

**Candidate:** `FSATS-CB-v0.1`
**Status:** `DESIGN_CANDIDATE / NOT_OWNER_ACCEPTED / NOT_CLOSED`
**Implementation Authority:** `NOT GRANTED`

## 1. Purpose

This document turns the design into a code-ready build plan without writing production code or granting implementation authority.

The primary implementation profile follows current Falcon runtime authority:

```text
LANGUAGE = C#
RUNTIME = .NET 10 LTS
TARGET OS = WINDOWS + LINUX
```

## 2. Implementation Shape

Initial deployment uses **one independently governed deployable process per Falcon Application**, with modular-monolith internals.

Reasons:

- preserves APP-001 Application isolation;
- minimizes network hops on hot trading paths;
- reduces distributed-state failure modes;
- remains maintainable by a small initial team;
- allows later module extraction when measured evidence justifies it;
- avoids confusing an LSA responsibility boundary with a mandatory microservice boundary.

## 3. Proposed Repository Tree

```text
applications/FSATS/
├── WORKSTREAM_RULES.md                       # Owner-controlled, read-only
├── Falcon.FSATS.slnx                         # future, separately authorized
├── contracts/                                # declaration-only cross-App business schemas
│   ├── provider-data/
│   ├── guardian-protection/
│   ├── resource-coordination/
│   ├── simulation-validation/
│   └── awareness-handoff/
├── src/
│   ├── Trading/
│   │   ├── Falcon.Trading.Core/
│   │   ├── Falcon.Trading.Risk/
│   │   ├── Falcon.Trading.Execution/
│   │   ├── Falcon.Trading.Awareness/
│   │   ├── Falcon.Trading.Infrastructure/
│   │   └── Falcon.Trading.Host/
│   ├── ProviderManagement/
│   │   ├── Falcon.ProviderManagement.Core/
│   │   ├── Falcon.ProviderManagement.DataFabric/
│   │   ├── Falcon.ProviderManagement.Awareness/
│   │   ├── Falcon.ProviderManagement.Infrastructure/
│   │   └── Falcon.ProviderManagement.Host/
│   ├── TradingGuardian/
│   │   ├── Falcon.TradingGuardian.Core/
│   │   ├── Falcon.TradingGuardian.Awareness/
│   │   ├── Falcon.TradingGuardian.Infrastructure/
│   │   └── Falcon.TradingGuardian.Host/
│   ├── TradingSimulation/
│   │   ├── Falcon.TradingSimulation.Core/
│   │   ├── Falcon.TradingSimulation.Market/
│   │   ├── Falcon.TradingSimulation.Execution/
│   │   ├── Falcon.TradingSimulation.Awareness/
│   │   ├── Falcon.TradingSimulation.Infrastructure/
│   │   └── Falcon.TradingSimulation.Host/
│   └── ResourceCoordination/
│       ├── Falcon.FSATS.ResourceCoordination.Contracts/
│       ├── Falcon.FSATS.ResourceCoordination.Core/
│       └── Falcon.FSATS.ResourceCoordination.FoundationBinding/
├── tests/
│   ├── Architecture/
│   ├── Security/
│   ├── Contracts/
│   ├── Trading/
│   ├── ProviderManagement/
│   ├── TradingGuardian/
│   ├── TradingSimulation/
│   ├── ResourceCoordination/
│   ├── Integration/
│   ├── Failure/
│   └── Performance/
└── tools/
    └── verification-only tooling as separately authorized
```

Exact creation of these paths/projects requires later implementation authorization.

## 4. Project Responsibilities

### `*.Core`

Owns Application business/domain state machines, policies, interfaces and LSA modules. It has no provider/broker/database/network implementation dependencies.

### `*.Risk` / `*.Execution`

Trading hard-boundary projects are split because capital/risk and broker side effects have unusually high consequence and benefit from explicit dependency constraints and dedicated verification.

### `*.Awareness`

Implements MSA/LSA/eligible CSA awareness logic, self-knowledge projections, candidate generation interfaces and Monitor AI integration. It does not own operational side effects.

### `*.Infrastructure`

Implements persistence and external/Foundation adapters behind Core-owned interfaces.

### `*.Host`

Composition root only: lifecycle integration, configuration binding, dependency injection, startup/shutdown and adapter wiring. Business policy does not hide in the Host.

### `ResourceCoordination.FoundationBinding`

Contains only the exact future/accepted Foundation coordinator/resource contract bindings. It must not be implemented against guessed APIs. Host/admission realization remains gated by current Foundation evidence.

## 5. LSA Placement

LSAs are explicit modules/namespaces inside the relevant Application projects.

Example Trading Core structure:

```text
Falcon.Trading.Core/
├── Operations/                 # T-LSA-01
├── Markets/                    # T-LSA-02
├── Analysis/                   # T-LSA-03
├── ClassicalSchool/            # T-LSA-04
├── OpportunityHunting/         # T-LSA-05
├── StrategyOrchestration/      # T-LSA-06
├── PortfolioCapital/           # T-LSA-08
├── LearningKnowledge/          # T-LSA-10
├── AnalyticsAttribution/       # T-LSA-11
├── StrategyEvolution/          # T-LSA-12
└── ResourceAwareness/          # T-LSA-13
```

T-LSA-07 hard Risk policy is in `Falcon.Trading.Risk`.
T-LSA-09 execution lifecycle is in `Falcon.Trading.Execution`.

This is still one Trading Application.

## 6. Dependency Rules

Mandatory architecture-test rules:

```text
CORE -> no INFRASTRUCTURE/HOST dependency
RISK -> no BROKER-SPECIFIC IMPLEMENTATION dependency
EXECUTION DOMAIN -> no PROVIDER DATA IMPLEMENTATION dependency
AWARENESS -> no direct BROKER/PROVIDER side-effect dependency
INFRASTRUCTURE -> may implement CORE interfaces
HOST -> may reference Application internal projects for composition only
APPLICATION_A -> no direct project reference to APPLICATION_B internals
FSATS CONTRACT DECLARATIONS -> no runtime state/authority
```

Foundation source is never copied into Application projects.

## 7. Cross-Application Contract Materialization

Canonical business contracts are declared under `contracts/` with exact producer/consumer ownership and then mapped/generated into Application-local types as needed.

This avoids a mutable shared runtime library becoming a hidden system owner.

Each contract declaration includes schema version, truth class, authority class, security profile, correlation/causation/idempotency and Foundation binding metadata.

## 8. Persistence Profile

Recommended initial persistence profile for Owner review:

- PostgreSQL for durable operational Application state;
- separate logical database/schema and credentials per Application boundary;
- no cross-Application SQL queries;
- Application-owned migrations;
- transactional state/outbox pattern where exact business atomicity requires it;
- append-only journals for high-consequence order/capital/protection/resource/candidate evidence where appropriate.

For tests:

- pure in-memory fakes for unit tests;
- SQLite only for low-risk repository tests that do not depend on PostgreSQL-specific semantics;
- PostgreSQL-compatible integration tests for transaction/concurrency/migration behavior to avoid false confidence from a different database engine.

Persistence remains behind replaceable Application interfaces. This design choice becomes binding only if the Owner accepts this candidate.

## 9. Cache Profile

Initial design prefers bounded in-process caches/projections when safe.

Do not add Redis or a distributed cache until measured need demonstrates value. A cache must always declare authoritative source, freshness and invalidation behavior.

## 10. Messaging Profile

Cross-Application messaging uses current Falcon Foundation communication/event capabilities through declared contracts.

Do not introduce Kafka, RabbitMQ or another parallel cross-Application transport merely for convenience unless a future governed gap proves it necessary.

Application-local channels/queues are allowed for internal concurrency when bounded and non-authoritative outside the Application.

## 11. External Adapter Pattern

Provider/broker integrations use ports/adapters.

Examples:

```text
IProviderMarketDataAdapter
IBrokerTradingAdapter
IBrokerAccountAdapter
IFoundationRouteAdapter
IFoundationResourceAdapter
ICredentialReferenceResolver
```

Interfaces are Application-owned abstractions. Implementations bind exact vendor/Foundation contracts.

Vendor DTOs do not escape the adapter boundary into domain state.

## 12. Configuration

Configuration is typed, versionable and environment-scoped.

Configuration classes include:

- Market Profiles;
- strategy applicability;
- Risk policy references;
- provider/broker capability profiles;
- resource profiles;
- feature toggles;
- environment identities;
- simulation profiles;
- monitor/model configuration.

Secrets are never normal configuration values.

## 13. Observability Implementation Profile

Use .NET-native diagnostics/OpenTelemetry-compatible instrumentation for:

- traces;
- metrics;
- logs.

Instrumentation is behind Falcon evidence/security rules and must control cardinality and sensitive fields.

## 14. Coding Standards

Future implementation should enforce at least:

- nullable reference types enabled;
- warnings-as-errors for governed projects unless an exact justified exception exists;
- analyzers/security rules;
- deterministic/reproducible build settings;
- explicit cancellation for I/O/long-running work;
- async I/O without sync-over-async;
- immutable records/value objects where appropriate;
- bounded collections/queues;
- explicit time abstractions for testability;
- explicit money/quantity/currency types rather than unsafe primitive mixing;
- no floating-point use for exact monetary/accounting quantities without a justified bounded purpose;
- reason-code enums/contracts for governed decisions;
- no exception-driven normal business state machines.

## 15. Numeric Semantics

Trading values require explicit type and unit.

Examples:

- `Money` = decimal amount + currency;
- `Price` = decimal + quote currency + precision metadata where needed;
- `Quantity` = decimal + asset/unit semantics;
- `Notional` = Money;
- ratios/percentages = bounded typed values;
- confidence = bounded probability/score type with interpretation metadata.

Do not mix percent (5) and fraction (0.05) semantics in raw numeric fields.

## 16. Time Semantics

Use an injectable authoritative time abstraction supplied/mapped according to Foundation time semantics.

Domain types distinguish:

- observed time;
- received time;
- effective time;
- expiry;
- deadline;
- simulation time.

Tests use deterministic/fake clocks.

## 17. Initial Implementation Work Slices

No slice below is authorized yet. They are future separately authorizable units.

### IB-01 — Solution skeleton and architecture enforcement

- .NET 10 solution/projects;
- build settings;
- architecture tests;
- no business runtime behavior.

### IB-02 — Canonical Application-owned primitives

- money/quantity/price/domain IDs;
- result/reason/state types;
- serialization/property tests.

### IB-03 — Application identities and Manifest materialization

- exact CON-023 declarations for four Applications;
- Foundation dependency mappings;
- no activation.

### IB-04 — Contract declaration set

- cross-App families;
- schema generation/validation;
- no active routes.

### IB-05 — Trading deterministic core

- T-LSA-01/02/03/06 base structures;
- market/universe/analysis/decision contracts;
- no broker side effects.

### IB-06 — Unified Risk and capital ledger

- hard pre-trade gate;
- reservation state machine;
- concurrency/idempotency fixtures.

### IB-07 — Execution and reconciliation core

- canonical order state/event model;
- ambiguity/reconciliation;
- broker adapter interfaces only initially.

### IB-08 — FSAPMA deterministic data fabric

- provider registry/capability/product/quality/quota/routing models;
- mock adapters first.

### IB-09 — Guardian deterministic protection core

- incident/directive/recovery state machines;
- command contract fixtures.

### IB-10 — FSTSimA deterministic engine

- clock/replay/scenario/evidence;
- provider/broker fault simulators;
- no Live path.

### IB-11 — FSARM pure coordination core

- profiles/priority evidence/redistribution/residual-need/fencing;
- Foundation binding mocked until exact consumption evidence permits.

### IB-12 — Awareness framework

- Self-Knowledge;
- MSA/LSA structures;
- CSA eligibility;
- typed AI outputs;
- no external research or production promotion.

### IB-13 — Strategy schools and central Strategy Controller

- catalog;
- classical/opportunity modules;
- applicability/conflict resolution;
- no direct broker path.

### IB-14 — Learning/analytics/evolution laboratory

- attribution;
- candidate lifecycle;
- experiment integration with FSTSimA;
- adaptive meta-learning remains sandboxed.

### IB-15 — Foundation communication/resource integration

Only when current accepted Foundation artifacts can be consumed canonically and required FCR bindings are available.

### IB-16 — Provider operational egress

Blocked until FCR-0013 Stage 12 capability is implemented/authorized and Application verification can occur.

### IB-17 — Broker Paper egress

Blocked until FCR-0014 Stage 12 capability is implemented/authorized. Paper credentials/domain remain isolated.

### IB-18 — Awareness research egress

Blocked until FCR-0008 Stage 12 capability is implemented/authorized.

### IB-19 — FSTSimA enforced non-Live egress/isolation

Blocked until FCR-0011 Stage 12 capability is implemented/authorized.

### IB-20 — MSA-to-FSA production-bound handoff

Blocked until FCR-0012/FCR-0030 Foundation reconciliation/implementation evidence permits exact binding.

### IB-21 — Integrated Paper readiness

Requires all Paper dependencies, deterministic/failure/security/performance evidence and separate Owner authorization.

## 18. Safe Parallelization

After IB-01/02 common structural prerequisites are stable, these can proceed largely in parallel if separately authorized:

- Trading deterministic modules;
- FSAPMA deterministic modules;
- Guardian deterministic modules;
- FSTSimA deterministic modules;
- FSARM pure algorithmic core;
- Awareness framework;
- contract/test tooling.

Integration slices wait for exact contract/Foundation dependencies.

## 19. Build Gates

Every implementation slice must pass as applicable:

- restore with pinned dependencies;
- Release build;
- architecture tests;
- security tests;
- unit/property tests;
- contract/schema tests;
- deterministic rerun tests;
- failure/adversarial tests;
- integration tests;
- performance tests;
- exact clean-tree/digest evidence.

## 20. Architecture Tests

Automatically reject at least:

- cross-Application internal project references;
- Core referencing Infrastructure/Host;
- Awareness referencing broker/provider side-effect implementations;
- strategy modules referencing broker adapter implementation;
- Guardian depending on Trading internals;
- Simulation depending on Live broker implementation;
- Foundation source copied into Application;
- prohibited dependency cycles.

## 21. Security Tests

Test at least:

- secret leakage;
- environment crossing;
- unauthorized route/action;
- stale/expired authority;
- replay-to-operational escalation;
- forged identity/correlation;
- malformed external payloads;
- AI tool misuse;
- untrusted candidate artifact;
- credential-role confusion.

## 22. Deterministic State-Machine Tests

Property/model-based tests should cover:

- capital reservations;
- order lifecycle;
- cancel/replace races;
- Guardian directive precedence;
- resource coordinator epochs;
- lifecycle restrictions;
- candidate promotion states.

Invalid transitions are explicit test cases.

## 23. Performance Test Plan

Measure end-to-end and component tails under:

- normal load;
- market burst;
- provider reconnect;
- large discovery scan;
- simultaneous strategy signals;
- broker event burst;
- resource pressure;
- Guardian crisis;
- observability load.

Hot-path budget must be allocated across acquisition, analysis, risk, reservation and execution rather than defined as one vague system latency target.

## 24. Implementation Authorization Rule

Each future slice requires:

```text
ACCEPTED DESIGN BASIS
+ CURRENT FOUNDATION/FCR CHECK
+ EXACT FILE/SCOPE ALLOWLIST
+ OWNER IMPLEMENTATION AUTHORIZATION
```

Completion of one slice never authorizes the next automatically.

## 25. Definition of Implementation Done

For any slice:

```text
CODE WRITTEN
!= TECHNICALLY VERIFIED
!= APPLICATION VERIFIED
!= OWNER CLOSED
!= RUNTIME AUTHORIZED
```

All states must remain separate in documentary evidence.
