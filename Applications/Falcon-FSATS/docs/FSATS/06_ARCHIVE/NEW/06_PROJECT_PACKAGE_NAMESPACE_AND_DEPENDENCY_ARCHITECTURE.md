# FSATS Specialized Implementation Architecture — Project, Package, Namespace and Dependency Architecture

**Package:** `FSATS-SIA-v0.1`
**Status:** `DESIGN_CANDIDATE`
**Target Runtime Family:** `.NET 10 / C# 14` unless a later governed Application build decision changes the language/runtime

## 1. Purpose

Define a physical code structure that enforces the logical Application/LSA boundaries without turning every boundary into a network hop.

The target is a modular in-process architecture per Application with assembly-level LSA boundaries, independently governed Application hosts, contract-only cross-Application dependencies and Foundation adapters behind explicit seams.

## 2. Solution Topology

Proposed controlled solution:

```text
Falcon.Applications.FSATS.slnx

  shared/application-owned compile-time libraries
  four independent Application hosts
  LSA module assemblies
  contract assemblies
  Foundation adapter assemblies
  tests / architecture tests / verifiers
```

The solution file is a build convenience only. It is not an FSATS runtime principal and grants no cross-Application access.

## 3. Shared Application-Owned Libraries

Only these shared libraries are permitted initially:

### `Falcon.FSATS.DomainPrimitives`

Contains only semantics proven identical across FSATS Applications, such as exact wrappers/enums defined in the canonical type catalog that are genuinely cross-domain.

Forbidden content:

- mutable state;
- database context;
- service locator;
- controllers;
- provider/broker clients;
- Foundation clones;
- strategy/risk/Guardian business logic.

### `Falcon.FSATS.ContractMetadata`

Generated/read-only metadata describing canonical contract family IDs, schema IDs, versions and compatibility declarations.

It SHALL NOT own payload business semantics; payload types live in the producer/semantic-owner contract assembly.

### `Falcon.FSATS.Testing`

Test-only builders/fixtures. It SHALL NOT be referenced by production assemblies.

No other cross-Application shared runtime library may be introduced without architecture review proving that it does not become a hidden fifth Application or shared mutable owner.

## 4. Application Project Pattern

Every Application uses this base project pattern:

```text
<Domain>.Host
<Domain>.Contracts
<Domain>.FoundationAdapters
<Domain>.Persistence
<Domain>.Awareness
<Domain>.LSA.<NN>.<Name>   // one assembly per major LSA branch
<Domain>.Tests
<Domain>.Architecture.Tests
<Domain>.Verifier
```

Rationale:

- one process/host avoids unnecessary network latency inside one Application;
- one assembly per LSA gives compile-time ownership/isolation and independent testability;
- cross-Application contracts remain small and versioned;
- Foundation bindings are isolated from business modules;
- persistence infrastructure cannot silently become domain logic.

## 5. Trading Projects

Canonical project codes:

```text
Falcon.Trading.Host
Falcon.Trading.Contracts
Falcon.Trading.FoundationAdapters
Falcon.Trading.Persistence
Falcon.Trading.Awareness

Falcon.Trading.LSA01.OperationsAccountEnvironment
Falcon.Trading.LSA02.MarketInstrumentUniverse
Falcon.Trading.LSA03.AnalysisFrameworks
Falcon.Trading.LSA04.ClassicalTrading
Falcon.Trading.LSA05.OpportunityHunting
Falcon.Trading.LSA06.StrategyOrchestrationDecision
Falcon.Trading.LSA07.UnifiedRisk
Falcon.Trading.LSA08.PortfolioCapital
Falcon.Trading.LSA09.ExecutionPosition
Falcon.Trading.LSA10.LearningKnowledge
Falcon.Trading.LSA11.AnalyticsAttribution
Falcon.Trading.LSA12.StrategyEvolution
Falcon.Trading.LSA13.ResourceAwareness
```

Root namespace mirrors project name.

## 6. FSAPMA Projects

```text
Falcon.ProviderManagement.Host
Falcon.ProviderManagement.Contracts
Falcon.ProviderManagement.FoundationAdapters
Falcon.ProviderManagement.Persistence
Falcon.ProviderManagement.Awareness

Falcon.ProviderManagement.LSA01.ProviderRegistry
Falcon.ProviderManagement.LSA02.DataProductsNormalization
Falcon.ProviderManagement.LSA03.CapabilityEntitlement
Falcon.ProviderManagement.LSA04.SelectionRoutingDelivery
Falcon.ProviderManagement.LSA05.QualityReconciliation
Falcon.ProviderManagement.LSA06.QuotaCapacityCostReliability
```

## 7. Guardian Projects

```text
Falcon.TradingGuardian.Host
Falcon.TradingGuardian.Contracts
Falcon.TradingGuardian.FoundationAdapters
Falcon.TradingGuardian.Persistence
Falcon.TradingGuardian.Awareness

Falcon.TradingGuardian.LSA01.ObservationIncident
Falcon.TradingGuardian.LSA02.RestrictionCommand
Falcon.TradingGuardian.LSA03.CrisisProtectionCoordination
Falcon.TradingGuardian.LSA04.RecoveryEvidence
```

## 8. FSTSimA Projects

```text
Falcon.TradingSimulation.Host
Falcon.TradingSimulation.Contracts
Falcon.TradingSimulation.FoundationAdapters
Falcon.TradingSimulation.Persistence
Falcon.TradingSimulation.Awareness

Falcon.TradingSimulation.LSA01.TimeScenario
Falcon.TradingSimulation.LSA02.MarketEnvironment
Falcon.TradingSimulation.LSA03.ProviderExternalSimulation
Falcon.TradingSimulation.LSA04.BrokerExecutionSimulation
Falcon.TradingSimulation.LSA05.AccountCapitalSettlement
Falcon.TradingSimulation.LSA06.FaultLatencyCrisis
Falcon.TradingSimulation.LSA07.FidelityCalibration
Falcon.TradingSimulation.LSA08.OracleEvidenceValidation
```

## 9. FSARM Project Gate

No project named `Falcon.FSATS.ResourceManager` or equivalent may be created until `11_FSARM_SPECIALIZED_RESOURCE_ARCHITECTURE.md` resolves and the Owner accepts the exact principal/host model.

This prevents the project name from silently manufacturing a runtime authority.

## 10. Allowed Project Reference Rules

### 10.1 Within one Application

Allowed:

- Host -> all LSA modules;
- Host -> Persistence/FoundationAdapters/Awareness;
- LSA -> own Application Contracts where needed;
- LSA -> `Falcon.FSATS.DomainPrimitives` only for shared semantic primitives;
- LSA -> explicit same-Application LSA abstraction only when the dependency matrix in the specialized Application file allows it;
- Persistence -> domain/public persistence ports, never the reverse;
- FoundationAdapters -> contract/port interfaces, never business-module internals.

### 10.2 Across Applications

Allowed production reference:

```text
Consumer Application -> Producer/Semantic-Owner *.Contracts assembly
```

Forbidden:

```text
Trading -> ProviderManagement.LSA04
Trading -> TradingGuardian.LSA02
Guardian -> Trading.LSA07
Any Application -> another Application.Persistence
Any Application -> another Application.Host
```

Cross-Application runtime interaction still occurs through governed Foundation transport, not by invoking the contract assembly as a service object.

## 11. Contract Assembly Ownership

Outbound contract payloads are owned by the Application that owns the business meaning.

Examples:

- normalized `DataProduct` payloads -> `Falcon.ProviderManagement.Contracts`;
- `TradeStateProjection` -> `Falcon.Trading.Contracts`;
- `ProtectionDirective` -> `Falcon.TradingGuardian.Contracts`;
- `SimulationValidationEvidence` -> `Falcon.TradingSimulation.Contracts`.

A request contract may be owned by the responder if it defines the service/capability semantics. Ownership must be declared in the canonical contract catalog.

Contract assemblies contain:

- immutable DTO/value definitions;
- schema IDs/versions;
- validation rules that are part of the contract;
- no transport client;
- no business service implementation;
- no database access;
- no mutable singleton state.

## 12. Internal LSA Communication

Within one Application, prefer explicit in-process ports/events over direct access to another LSA's internals.

Allowed patterns:

1. synchronous read/query port for low-latency deterministic current-state lookups;
2. synchronous command port when one LSA is the sole owner of the target aggregate;
3. immutable internal event for state-change fan-out;
4. read-only snapshot/projection for analytical consumers.

Forbidden:

- direct repository access to another LSA's aggregate;
- direct mutation of another LSA's state object;
- service locator/dynamic reflection to bypass declared ports;
- cyclic command dependencies.

## 13. Dependency Direction Pattern

```text
Contracts / DomainPrimitives
        ^
        |
LSA Domain Modules
        ^
        |
Application Host / Orchestration
        |
        +--> Persistence adapters
        +--> Foundation adapters
        +--> external provider/broker adapters only in owning Application
```

Infrastructure depends inward on abstractions. Domain modules do not depend on concrete infrastructure.

## 14. Same-Application Dependency Cycle Rule

The LSA dependency graph SHALL be a directed graph with no synchronous command cycle.

Asynchronous event feedback is allowed only when:

- causation identity is preserved;
- loop termination/idempotency is explicit;
- no event storm is possible from the same semantic change;
- the resulting eventual consistency is acceptable for that data.

A cycle detected in the build graph is an architecture failure unless explicitly approved as a test-only dependency.

## 15. Public/Internal Surface Rule

Each LSA assembly defaults to internal implementation types.

Public surface is limited to:

- declared same-Application port interfaces/types;
- contract-facing application services used by the Host;
- explicitly CSA-observable Self-Knowledge projection where applicable.

Repositories, entities, persistence models and algorithm implementation classes remain internal.

## 16. Persistence Project Rule

There is one persistence adapter project per Application, but authoritative table/schema ownership remains tagged by LSA aggregate.

The persistence project SHALL NOT become a business service. It implements ports defined by the owning LSA modules and is prohibited from:

- deciding risk;
- allocating capital;
- choosing providers;
- changing protection state;
- interpreting simulation validity;
- orchestrating strategies.

## 17. Foundation Adapter Rule

`*.FoundationAdapters` is the only Application project that may directly depend on Foundation build artifacts/packages once the canonical consumption mechanism exists.

Responsibilities:

- map Application Manifest declaration to accepted Foundation Manifest type;
- map FIL/Service Bus/event/security/lifecycle/resource/time/identifier/evidence boundaries;
- preserve exact Foundation identities/semantics;
- translate Foundation unavailability into explicit Application fail-closed outcomes;
- expose Application-owned ports to the Host/modules.

It SHALL NOT:

- copy Foundation source;
- emulate missing Foundation capability;
- interpret Application business decisions;
- convert missing authority into local allow.

FCR-0016 gates canonical cross-workstream build consumption.

## 18. Provider / Broker Adapter Placement

Provider adapters exist only under FSAPMA, conceptually as implementation submodules behind P-LSA-04/P-LSA-02 ports.

Broker adapters exist only under Trading execution, conceptually behind T-LSA-09 ports.

They may be separate assemblies for volatile third-party dependencies:

```text
Falcon.ProviderManagement.Adapter.<ProviderName>
Falcon.Trading.BrokerAdapter.<BrokerName>
```

These assemblies are loaded/registered only through an allowlisted adapter registry and do not receive authority merely by being present.

## 19. Build-Time Dependency Matrix

Legend: `A` allowed; `C` contracts only; `F` Foundation adapter only; `X` forbidden.

| From \ To | Same App LSA | Same App Persistence | Same App FoundationAdapter | Other App Contracts | Other App LSA/Host/Persistence | Foundation concrete artifacts |
|---|---:|---:|---:|---:|---:|---:|
| Application Host | A | A | A | C | X | X |
| LSA Module | declared A | X | via port only | C | X | X |
| Persistence | via ports | n/a | X | X | X | X |
| FoundationAdapters | via ports/contracts | X | n/a | C only if mapping requires | X | F |
| Contract assembly | X | X | X | X | X | X |

## 20. Architecture Test Rules

The architecture test project SHALL fail if it finds:

1. a project reference from one Application implementation assembly to another Application implementation assembly;
2. a domain LSA referencing a Persistence concrete assembly;
3. a domain LSA referencing Foundation concrete artifacts directly;
4. production referencing test/verification packages;
5. contract assembly referencing infrastructure/business implementation;
6. circular synchronous project dependency;
7. namespace `FSATS.*` owning mutable runtime business state without an explicit governed owner;
8. Provider adapters outside FSAPMA;
9. Broker adapters outside Trading;
10. FSTSimA production reference from Trading as an authority source;
11. any hidden `Common`, `Shared`, `Utils` project containing mutable cross-Application state or business control.

## 21. Versioning

Project/package versions follow semantic compatibility at the exported surface.

- internal implementation-only change: patch;
- backward-compatible exported contract addition: minor, if schema compatibility rules permit;
- breaking contract/state/persistence semantic: major or explicitly governed successor identity;
- authority/purpose change is not ordinary SemVer alone and requires governance.

Contract schema versions remain explicit even when assembly/package SemVer changes.

## 22. Removal Test

The build architecture SHALL support removing any one Application projects from the solution and still building Foundation-independent remaining Application code/contract tests where the removed Application is replaced by contract stubs/availability-failure fixtures.

No compile-time dependency on another Application implementation may make safe removal impossible.
