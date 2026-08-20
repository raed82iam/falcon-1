# P1-C — Exact Project and Package Topology Candidate

**Status:** `DESIGN_CANDIDATE / REVIEW_REQUIRED / NOT_OWNER_ACCEPTED / NOT_CLOSED`  
**Scope:** `PART_1 / P1-C ONLY`  
**Implementation Authority:** `NOT GRANTED`  
**Runtime Authority:** `NOT GRANTED`

## 1. Purpose

This record materializes the exact future physical build topology for the five current FSATS Falcon Applications without creating implementation code, runtime routes, or a hidden FSATS runtime principal.

It is subordinate to the Falcon Vision, Falcon Constitution, APP-001, CON-023, ADR-I012, ADR-I015, the current Part 1 reading order, the Owner-accepted APP-RSC changed scope, current Foundation dispositions, and applicable FCRs.

## 2. Governing Topology Invariant

```text
FSATS = BUILD / DESIGN / SYSTEM GROUPING BOUNDARY ONLY
FSATS != FALCON APPLICATION
FSATS != RUNTIME PRINCIPAL
FSATS != AUTHORITY OWNER
FSATS MSA = 0
FSATS LSA = 0
```

The five independently admitted Applications remain:

```text
1. Falcon Self-Aware Trading Application
2. FSAPMA
3. Falcon Trading Guardian Application
4. FSTSimA
5. APP-RSC — Falcon Self-Aware Resource Management Application
```

Each Application SHALL remain independently identifiable, buildable, packageable, testable, admissible, activatable, isolatable, recoverable, replaceable and removable.

## 3. Canonical Future Workspace

The future implementation workspace SHALL be rooted under the current Application-owned subtree:

```text
applications/FSATS/
├── Falcon.FSATS.slnx
├── src/
│   ├── Trading/
│   ├── FSAPMA/
│   ├── TradingGuardian/
│   ├── FSTSimA/
│   └── ResourceManagement/
├── tests/
│   ├── Trading/
│   ├── FSAPMA/
│   ├── TradingGuardian/
│   ├── FSTSimA/
│   ├── ResourceManagement/
│   └── Architecture/
├── verification/
└── manifests/
```

`Falcon.FSATS.slnx` is a build/workspace composition artifact only. It SHALL NOT become an executable, service identity, lifecycle principal, message producer, authority source, state owner, persistence owner, or Application identity.

No implementation directory or project is created by this design record.

## 4. Standard Per-Application Project Pattern

Each FSATS Application SHALL use the same five-project structural pattern unless a later reviewed P1-C amendment proves a different pattern is required.

```text
<App>.Contracts
<App>.Domain
<App>.Application
<App>.Infrastructure
<App>.Awareness
```

### 4.1 Contracts

Public, versioned, producer-owned contract surface only.

Responsibilities:
- externally consumable request/response/event/projection contract declarations;
- stable public DTO/schema-facing types owned by that Application;
- no business execution logic;
- no persistence logic;
- no direct access to another Application's internals;
- exact cross-Application families remain subject to P1-K and FCR-0080 where applicable.

### 4.2 Domain

Application-owned business/domain truth and invariants.

Responsibilities:
- domain entities/value objects/policies owned by the Application;
- business state-transition invariants;
- no Foundation implementation logic;
- no transport/service-bus implementation;
- no dependency on Infrastructure.

### 4.3 Application

Application-level use-case orchestration and operational controllers.

Responsibilities:
- use-case coordination;
- application services;
- operational controllers that are not Awareness entities;
- declared ports consumed by Infrastructure;
- authority checks at the Application boundary where owned by the Application.

### 4.4 Infrastructure

Application-owned adapters and technical realization of declared ports.

Responsibilities:
- persistence adapters;
- Foundation contract/binding adapters;
- FIL/Service Bus adapters when later authorized/materialized;
- configuration/evidence adapters;
- no ownership of business truth merely because it stores or transports it.

### 4.5 Awareness

The single Application MSA plus that Application's qualified LSAs and eligible CSAs.

Responsibilities:
- MSA/LSA/CSA implementation modules according to accepted Awareness boundaries;
- self-awareness evaluation and governed self-development interfaces;
- no operational authority expansion from Awareness rank.

LSAs SHALL be modules/namespaces inside the owning Application's `Awareness` project unless a later independently reviewed technical reason requires a separate project. P1-C SHALL NOT create one project per LSA by default.

## 5. Exact Future Project Names

### 5.1 Trading

```text
Falcon.FSATS.Trading.Contracts
Falcon.FSATS.Trading.Domain
Falcon.FSATS.Trading.Application
Falcon.FSATS.Trading.Infrastructure
Falcon.FSATS.Trading.Awareness
```

Canonical future path:

```text
applications/FSATS/src/Trading/
```

The 13 Trading LSAs are Awareness modules under `Falcon.FSATS.Trading.Awareness`; Trading operational controllers and use cases remain under `Falcon.FSATS.Trading.Application` according to P1-F.

### 5.2 FSAPMA

```text
Falcon.FSATS.FSAPMA.Contracts
Falcon.FSATS.FSAPMA.Domain
Falcon.FSATS.FSAPMA.Application
Falcon.FSATS.FSAPMA.Infrastructure
Falcon.FSATS.FSAPMA.Awareness
```

Canonical future path:

```text
applications/FSATS/src/FSAPMA/
```

The six FSAPMA LSAs are Awareness modules under `Falcon.FSATS.FSAPMA.Awareness`. The Provider Controller remains an operational controller under the Application layer and does not become a CSA merely because it is intelligent or adaptive.

### 5.3 Trading Guardian

```text
Falcon.FSATS.TradingGuardian.Contracts
Falcon.FSATS.TradingGuardian.Domain
Falcon.FSATS.TradingGuardian.Application
Falcon.FSATS.TradingGuardian.Infrastructure
Falcon.FSATS.TradingGuardian.Awareness
```

Canonical future path:

```text
applications/FSATS/src/TradingGuardian/
```

The four Guardian LSAs are Awareness modules under `Falcon.FSATS.TradingGuardian.Awareness`.

### 5.4 FSTSimA

```text
Falcon.FSATS.FSTSimA.Contracts
Falcon.FSATS.FSTSimA.Domain
Falcon.FSATS.FSTSimA.Application
Falcon.FSATS.FSTSimA.Infrastructure
Falcon.FSATS.FSTSimA.Awareness
```

Canonical future path:

```text
applications/FSATS/src/FSTSimA/
```

The eight FSTSimA LSAs are Awareness modules under `Falcon.FSATS.FSTSimA.Awareness`.

### 5.5 APP-RSC

The canonical Falcon Application identity remains `APP-RSC — Falcon Self-Aware Resource Management Application`. The physical .NET project token SHALL be `ResourceManagement`; this token is a technical namespace/path token only and does not replace the canonical Application identity.

```text
Falcon.FSATS.ResourceManagement.Contracts
Falcon.FSATS.ResourceManagement.Domain
Falcon.FSATS.ResourceManagement.Application
Falcon.FSATS.ResourceManagement.Infrastructure
Falcon.FSATS.ResourceManagement.Awareness
```

Canonical future path:

```text
applications/FSATS/src/ResourceManagement/
```

APP-RSC MSA and its three qualified LSAs are modules under `Falcon.FSATS.ResourceManagement.Awareness`. The operational Resource Strategy Controller belongs to `Falcon.FSATS.ResourceManagement.Application` and SHALL NOT be collapsed into the MSA.

```text
MSA_RSC != RESOURCE_STRATEGY_CONTROLLER
AWARENESS != OPERATIONAL_CONTROL
```

## 6. Internal Project Dependency Direction

Within one Application only, the permitted default compile-time dependency direction is:

```text
Contracts      -> no Application-internal project dependency
Domain         -> no Infrastructure dependency
Application    -> Domain + Contracts
Awareness      -> Domain + Contracts
Infrastructure -> Application + Domain + Contracts
```

An executable/bootstrap composition root, if later required by implementation design, SHALL be owned by the corresponding Application and may compose that Application's `Application`, `Infrastructure`, and `Awareness` projects. Its exact executable-host form remains future implementation materialization and is not created by P1-C.

Forbidden internal cycles include:

```text
Domain -> Infrastructure
Contracts -> Domain
Contracts -> Application
Application -> Infrastructure
Awareness -> Infrastructure
```

Any exception requires an explicit later P1-C semantic amendment and fresh review.

## 7. Cross-Application Dependency Rule

No FSATS Application may directly reference another FSATS Application's `Domain`, `Application`, `Infrastructure`, or `Awareness` project.

```text
Trading.Application -> FSAPMA.Application = FORBIDDEN
Guardian.Domain -> Trading.Domain = FORBIDDEN
APP-RSC.Application -> FSTSimA.Infrastructure = FORBIDDEN
```

Cross-Application compile-time consumption, where P1-K later proves a compile-time dependency is required, SHALL use only an exact versioned producer-owned `*.Contracts` package or an approved Foundation-owned contract package.

Direct cross-Application `ProjectReference` is prohibited even to another Application's `*.Contracts` project. Local build convenience SHALL NOT erase package/version/provenance boundaries.

Runtime communication SHALL remain governed by admitted contracts/routes. `PackageReference` or schema availability does not grant runtime route or authority.

```text
PACKAGE_AVAILABLE != ROUTE_ADMITTED
ROUTE_ADMITTED != AUTHORITY
DELIVERY != ACCEPTANCE
```

The exact Shared Application / Foundation communication families and bindings remain blocked where FCR-0080 requires Foundation disposition.

## 8. Foundation Dependency Rule

Application projects SHALL NOT reference Foundation source projects directly and SHALL NOT copy Foundation source or recreate Foundation-owned semantics.

Foundation dependencies SHALL be consumed only through canonical published/approved Foundation artifacts and contracts when their exact availability and authority are established by P1-B/P1-K and applicable FCRs.

```text
APPLICATION -> FOUNDATION SOURCE PROJECT = FORBIDDEN
APPLICATION -> APPROVED FOUNDATION CONTRACT/PACKAGE = ALLOWED WHEN GOVERNED
```

A missing Foundation package/binding fails closed and uses the FCR process; it does not justify a local substitute.

## 9. Contract Package Identity Rule

The public package IDs reserved by this candidate are:

```text
Falcon.FSATS.Trading.Contracts
Falcon.FSATS.FSAPMA.Contracts
Falcon.FSATS.TradingGuardian.Contracts
Falcon.FSATS.FSTSimA.Contracts
Falcon.FSATS.ResourceManagement.Contracts
```

These packages expose only the public contract surface approved by P1-K. They SHALL NOT export Domain entities, persistence models, controller internals, Awareness internals, credentials, direct database handles, or internal service implementations.

Package versions SHALL be immutable once published. Compatibility policy, exact version fields and provenance bindings are completed by P1-E/P1-K and do not become runtime authority.

## 10. Shared Library Rule

No general `Falcon.FSATS.Common`, `SharedKernel`, or system-wide business project is authorized by this candidate.

A common storage shape is not enough to justify shared semantic ownership.

If P1-D later proves a genuinely Application-owned primitive is shared across multiple FSATS Applications, it SHALL identify one explicit owner and mapping rule. Foundation-owned identity/time/correlation/evidence/security/lifecycle/resource semantics SHALL be consumed from Foundation rather than re-created in an FSATS shared library.

This rule prevents the non-owning FSATS system boundary from becoming a hidden code owner through a growing common library.

## 11. Test and Verification Topology

Future tests SHALL mirror ownership rather than create a hidden runtime layer.

```text
applications/FSATS/tests/Trading/
applications/FSATS/tests/FSAPMA/
applications/FSATS/tests/TradingGuardian/
applications/FSATS/tests/FSTSimA/
applications/FSATS/tests/ResourceManagement/
applications/FSATS/tests/Architecture/
```

Per-Application tests MAY reference that Application's projects. Architecture tests MAY inspect metadata/dependency graphs across Applications but SHALL NOT become business runtime dependencies.

The architecture verification set SHALL prove at minimum:
- no cross-Application project reference;
- no Foundation source project reference;
- no project/assembly representing FSATS as a runtime principal;
- every project maps to exactly one Application owner except non-runtime architecture verification artifacts;
- only `*.Contracts` packages are eligible cross-Application compile-time surfaces;
- no public contract package exports internal implementation namespaces;
- APP-RSC remains independently removable/buildable;
- forbidden dependency cycles are absent.

## 12. Replacement and Removal Impact

Replacing/removing one Application SHALL permit removal of its five owned projects and its package/runtime registrations without requiring source modification inside sibling Applications.

Sibling Applications may require a compatible replacement contract package/runtime counterparty where they depend on an admitted capability, but SHALL NOT require sibling internal-code edits merely because the provider implementation changed.

APP-RSC removal additionally SHALL fence stale coordination identity/epoch and prevent new cross-Application resource redistribution; no sibling becomes replacement resource coordinator automatically.

## 13. P1-C Boundary Against P1-D Through P1-K

P1-C owns physical placement, project identity and dependency-direction rules only.

It does not finalize:
- exact business primitive definitions: P1-D;
- complete Manifest values and lifecycle records: P1-E;
- per-LSA component decomposition: P1-F through P1-J;
- exact contract family IDs/schemas/FIL routes: P1-K;
- external communication behavior currently awaiting FCR-0080 disposition;
- implementation code, executable hosts, deployment or runtime activation.

## 14. Candidate Closure Conditions

P1-C may proceed to Owner review only when fresh review proves:

1. every future implementation project has exactly one Application owner;
2. all five Applications remain independent lifecycle/package principals;
3. FSATS remains a non-runtime grouping boundary;
4. no cross-Application project reference is required;
5. public contract packages are the only eligible Application-to-Application compile-time surfaces;
6. Foundation source coupling is prohibited;
7. APP-RSC remains independently buildable/removable and Foundation-compatible;
8. the 34 LSAs remain owned without forcing a 34-project topology;
9. FCR-0080-dependent bindings remain explicitly unresolved rather than guessed;
10. no implementation/runtime authority is inferred.

This candidate is not Owner-accepted merely because it is written to the repository.