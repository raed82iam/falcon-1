# Stage 4 WP-03 Implementation Authority

## Work Package

WP-03 — State Ownership and Durable Current-State Persistence

## Controlling Sources

- `03_STAGE_4_IMPLEMENTATION_WORK_PACKAGE_PLAN.md`
- `07_STAGE_4_STATE_CLASS_SCOPE_AND_OWNERSHIP.md`
- `09_STAGE_4_CANDIDATE_IMPLEMENTATION_BOUNDARIES.md`
- `GOV-107_STAGE_4_WP03_IMPLEMENTATION_AUTHORIZATION.md`

## Authorized Design

WP-03 will introduce one bounded `Foundation.State` project.

The project will own:

- canonical state-class metadata;
- canonical ownership declarations;
- authoritative current-state records;
- immutable prior-state versions;
- a persistence abstraction;
- a deterministic file-backed provider suitable for isolated verification;
- explicit failure classifications;
- compare-expected-version updates.

The existing Lifecycle owner remains the owner of Lifecycle State. `Foundation.State` provides governed persistence and ownership enforcement but does not become the Lifecycle owner.

## Required State Labels

The implementation must distinguish:

- authoritative;
- derived;
- cached;
- observed;
- last-known;
- expected;
- desired;
- historical.

Only an accepted authoritative record written through its declared singular writer may become authoritative current state.

## Lifecycle Integration Boundary

The existing Stage 4-authorized Lifecycle path may persist each accepted Lifecycle version through `Foundation.State`.

A failed or rejected Lifecycle transition must not create a new authoritative persisted version.

WP-03 must not create a second Lifecycle controller or alter the canonical transition graph.

## Persistence Boundary

The file-backed provider is an isolated bounded provider for durable verification.

It must:

- use deterministic canonical bytes;
- write a current record and immutable history;
- detect missing, partial, malformed, corrupted, and conflicting durable state;
- reject stale expected versions;
- avoid silent fallback;
- preserve owner and source identity;
- support deterministic reload from the same bytes.

It must not implement WP-05 uncertain-write or restart reconciliation semantics.

## Exact Allowlist

```text
src/Foundation.State/Foundation.State.csproj
src/Foundation.State/AuthoritativeStateModels.cs
src/Foundation.State/StateOwnershipRegistry.cs
src/Foundation.State/DurableAuthoritativeStateStore.cs
src/Foundation.State/FileAuthoritativeStateProvider.cs
src/Foundation.Infrastructure/BootstrapLifecycleControl.cs
verification/Falcon.Stage4.WP03.Verifier/Falcon.Stage4.WP03.Verifier.csproj
verification/Falcon.Stage4.WP03.Verifier/Program.cs
tests/Falcon.Foundation.Architecture.Tests/Program.cs
tests/Falcon.Foundation.Security.Tests/Program.cs
Falcon.Foundation.ControlledProjectFoundation.slnx
docs/governance/GOV-107_STAGE_4_WP03_IMPLEMENTATION_AUTHORIZATION.md
docs/stage-4-proposal/14_STAGE_4_WP03_IMPLEMENTATION_AUTHORITY.md
docs/governance/GOV-000_AUTHORITY_REGISTRY.md
docs/stage-4-proposal/README.md
```

## Non-Authority

WP-04 through WP-06, Git operations, deployment, activation, external connectivity, and every non-allowlisted path remain unauthorized.

## Authorization State

```text
FALCON_FOUNDATION_STAGE4_WP03_IMPLEMENTATION_AUTHORIZED
```
