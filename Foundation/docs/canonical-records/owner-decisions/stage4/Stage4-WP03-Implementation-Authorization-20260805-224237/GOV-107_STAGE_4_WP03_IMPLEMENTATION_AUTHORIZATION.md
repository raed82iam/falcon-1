# GOV-107 — Falcon Foundation Stage 4 WP-03 Implementation Authorization

## Status

Approved / Effective for bounded WP-03 implementation only

## Authorized Work Package

Stage 4 WP-03 — State Ownership and Durable Current-State Persistence

## Purpose

Implement the FDN-001 authoritative-state rules and durable current-state model for the exact Stage 4 state classes defined in `07_STAGE_4_STATE_CLASS_SCOPE_AND_OWNERSHIP.md`.

## Authorized Production Boundaries

- `src/Foundation.State/Foundation.State.csproj`
- `src/Foundation.State/AuthoritativeStateModels.cs`
- `src/Foundation.State/StateOwnershipRegistry.cs`
- `src/Foundation.State/DurableAuthoritativeStateStore.cs`
- `src/Foundation.State/FileAuthoritativeStateProvider.cs`
- `src/Foundation.Infrastructure/BootstrapLifecycleControl.cs`

## Authorized Verification and Test Boundaries

- `verification/Falcon.Stage4.WP03.Verifier/Falcon.Stage4.WP03.Verifier.csproj`
- `verification/Falcon.Stage4.WP03.Verifier/Program.cs`
- `tests/Falcon.Foundation.Architecture.Tests/Program.cs`
- `tests/Falcon.Foundation.Security.Tests/Program.cs`
- `Falcon.Foundation.ControlledProjectFoundation.slnx`

## Authorized Documentation Boundaries

- `docs/governance/GOV-107_STAGE_4_WP03_IMPLEMENTATION_AUTHORIZATION.md`
- `docs/stage-4-proposal/14_STAGE_4_WP03_IMPLEMENTATION_AUTHORITY.md`
- `docs/governance/GOV-000_AUTHORITY_REGISTRY.md`
- `docs/stage-4-proposal/README.md`

## Required State Classes

WP-03 must model the exact Stage 4 classes:

1. Lifecycle State
2. Authority Policy Baseline
3. Authority Decision
4. State Ownership Declaration
5. Operational Evidence
6. Accepted-Fact Event
7. Persistence Commit State
8. Reconciliation State

WP-03 may define ownership and persistence metadata for all eight classes, but it must not implement the WP-04 Evidence Journal, accepted-fact publishing, or WP-05 reconciliation engine.

## Required Controls

The implementation must prove:

- one active authoritative owner per state subject and namespace;
- explicit authoritative source;
- explicit persistence owner;
- explicit read and write authorities;
- singular write authority;
- versioned authoritative current state;
- source identity and effective time;
- retention classification;
- compare-expected-version update;
- immutable prior-state history;
- explicit state classification labels;
- missing, stale, conflicting, corrupted, and partial state classifications;
- no cache, observation, projection, replica, last-known, expected, desired, or historical copy becomes authoritative accidentally;
- Application business-state isolation;
- deterministic reconstruction from unchanged durable state bytes.

## Dependency Direction

- `Foundation.State` may reference `Foundation.Contracts`.
- `Foundation.Infrastructure` may reference `Foundation.State` only for the bounded Lifecycle durable-state integration.
- `Foundation.State` must not reference `Foundation.Infrastructure`, `Foundation.Authority`, applications, trading, FIL, Service Bus, Guardian, or external packages.
- Persistence technology stores bytes but does not become the authoritative owner.

## Explicitly Not Authorized

- WP-04 through WP-06;
- Evidence Journal implementation;
- accepted-fact event publication;
- uncertain-write handling;
- restart reconciliation;
- multi-writer coordination beyond compare-expected-version semantics required by WP-03;
- changes to `Foundation.Core/LifecycleControl.cs`;
- changes to `Foundation.Contracts` or Contract Registry;
- production FIL or Service Bus;
- applications, trading, broker, market-data, Self-Awareness, or Guardian work;
- external packages or network dependencies;
- commit, tag, merge, rebase, push, deployment, or runtime activation;
- any path outside the exact allowlist.

## Exit

WP-03 may be submitted for independent review only after:

- clean Release build;
- Architecture Test PASS;
- Security Test PASS;
- WP-01 and WP-02 regressions PASS;
- Stage 2 regressions PASS;
- Stage 3 WP-01 through WP-06 regressions PASS;
- WP-03 verifier PASS twice with identical deterministic output;
- exact source inventory and evidence ZIP creation.

## Final Authorization State

```text
FALCON_FOUNDATION_STAGE4_WP03_IMPLEMENTATION_AUTHORIZED
STAGE4_WP04_THROUGH_WP06_UNAUTHORIZED
```
