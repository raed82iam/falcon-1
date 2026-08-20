# P1-C — Host and Deployable Package Completion

**Status:** `CONTROLLING_CANDIDATE_COMPLETION / REVIEW_REQUIRED / NOT_OWNER_ACCEPTED / NOT_CLOSED`  
**Controls:** `01_P1C_EXACT_PROJECT_PACKAGE_TOPOLOGY_CANDIDATE.md` where this record is more specific  
**Implementation Authority:** `NOT GRANTED`  
**Runtime Authority:** `NOT GRANTED`

## Reason

The first exact-topology candidate defined `Contracts / Domain / Application / Infrastructure / Awareness` but left the executable/bootstrap composition root for each independently activatable Falcon Application unresolved. P1-C cannot be exact while that physical boundary remains deferred.

This record completes that gap before semantic freeze.

## Standard Six-Project Pattern

Every current FSATS Falcon Application SHALL use:

```text
<App>.Contracts
<App>.Domain
<App>.Application
<App>.Infrastructure
<App>.Awareness
<App>.Host
```

`Host` is the owning Application's executable/bootstrap composition root. It may compose only that Application's `Application`, `Infrastructure`, `Awareness`, and admitted contract/Foundation packages.

`Host`:
- is not a separate Falcon Application;
- is not a separate lifecycle authority;
- owns no business truth merely because it is executable;
- may not contain sibling Application internals;
- may not become an FSATS-wide runtime coordinator.

## Exact Added Host Projects

```text
Falcon.FSATS.Trading.Host
Falcon.FSATS.FSAPMA.Host
Falcon.FSATS.TradingGuardian.Host
Falcon.FSATS.FSTSimA.Host
Falcon.FSATS.ResourceManagement.Host
```

The APP-RSC canonical identity remains `APP-RSC — Falcon Self-Aware Resource Management Application`; `ResourceManagement` remains only the technical project/path token.

## Exact Internal Dependency Direction

Within one Application only:

```text
Contracts      -> none
Domain         -> none
Application    -> Domain + Contracts
Awareness      -> Application + Domain + Contracts
Infrastructure -> Application + Domain + Contracts
Host           -> Application + Infrastructure + Awareness + Contracts
```

No dependency cycle is allowed. The Host is the only normal composition root that references both Infrastructure and Awareness.

## Exact Deployable Package IDs

Each Falcon Application SHALL produce one independently identifiable deployable Application package:

```text
Falcon.FSATS.Trading
Falcon.FSATS.FSAPMA
Falcon.FSATS.TradingGuardian
Falcon.FSATS.FSTSimA
Falcon.FSATS.ResourceManagement
```

Each package maps to exactly one immutable Falcon Application identity under P1-E/CON-023 and contains its Host plus owned assemblies, Manifest, and admitted dependencies.

Package discovery/installation does not imply admission or activation.

The producer-owned public contract package IDs remain:

```text
Falcon.FSATS.Trading.Contracts
Falcon.FSATS.FSAPMA.Contracts
Falcon.FSATS.TradingGuardian.Contracts
Falcon.FSATS.FSTSimA.Contracts
Falcon.FSATS.ResourceManagement.Contracts
```

## Cross-Application Rule

Direct cross-Application `ProjectReference` remains forbidden, including a direct reference to another Application's `*.Contracts` project.

Where P1-K later proves compile-time consumption is required, only an exact versioned producer-owned `*.Contracts` package or approved Foundation-owned package may be consumed.

```text
PACKAGE_AVAILABLE != ROUTE_ADMITTED
ROUTE_ADMITTED != AUTHORITY
DELIVERY != ACCEPTANCE
```

## Replacement / Removal Completion

Replacing or removing one Application SHALL allow removal of its six owned projects, deployable package and runtime registrations without source modification inside sibling Applications.

APP-RSC removal additionally fences stale coordinator identity/epoch and creates no sibling authority inheritance.

## Review Effect

The P1-C semantic review target SHALL include both:

1. `01_P1C_EXACT_PROJECT_PACKAGE_TOPOLOGY_CANDIDATE.md`
2. this controlling completion record.

No semantic freeze or review performed before this completion may be treated as current P1-C review evidence.