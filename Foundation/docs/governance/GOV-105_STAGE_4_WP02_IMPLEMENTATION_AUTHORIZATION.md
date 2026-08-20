# Stage 4 WP-02 Implementation Authorization

**Identifier:** GOV-105  
**Version:** 1.0  
**Status:** Owner confirmation required by authorization script  
**Scope:** Stage 4 WP-02 only  
**WP title:** Authoritative Lifecycle Integration and Hardening

## Purpose

Authorize a bounded implementation that integrates the accepted WP-01 Authority Engine with the existing Stage 3 Lifecycle execution boundary.

WP-02 must reuse and harden the accepted Lifecycle implementation. It must not create a second Lifecycle controller.

## Exact Implementation Allowlist

### Existing production paths

- `src/Foundation.Infrastructure/Foundation.Infrastructure.csproj`
- `src/Foundation.Infrastructure/BootstrapLifecycleControl.cs`

### New verification paths

- `verification/Falcon.Stage4.WP02.Verifier/Falcon.Stage4.WP02.Verifier.csproj`
- `verification/Falcon.Stage4.WP02.Verifier/Program.cs`

### Existing integration and validation paths

- `Falcon.Foundation.ControlledProjectFoundation.slnx`
- `tests/Falcon.Foundation.Architecture.Tests/Program.cs`
- `tests/Falcon.Foundation.Security.Tests/Program.cs`

### WP-02 documentation paths

- `docs/stage-4-proposal/12_STAGE_4_WP02_IMPLEMENTATION_AUTHORITY.md`
- `docs/governance/GOV-105_STAGE_4_WP02_IMPLEMENTATION_AUTHORIZATION.md`
- `docs/governance/GOV-000_AUTHORITY_REGISTRY.md`
- `docs/stage-4-proposal/README.md`

No other path is authorized.

## Authorized Deliverables

- consume a WP-01 Authority Result at the existing Lifecycle execution boundary;
- reject missing, malformed, denied, expired, mismatched, or replay-conflicting authority decisions;
- bind the authority decision to the exact lifecycle request, subject, source state, target state, requester, scope, and observation time;
- preserve legal-transition validation from Stage 3;
- preserve optimistic state-version validation;
- preserve duplicate and conflicting transition behavior;
- preserve protective restriction and controlled recovery rules;
- expose exact attributable rejection reasons;
- add an independent WP-02 verifier;
- prove that one accepted authority decision cannot be reused for another lifecycle request or transition.

## Required Scenarios

- authorized valid transition succeeds;
- denied authority decision rejects transition;
- missing authority decision rejects transition;
- malformed authority decision rejects transition;
- authority decision request identity mismatch rejects transition;
- actor or requester mismatch rejects transition;
- action mismatch rejects transition;
- subject or resource mismatch rejects transition;
- target-state or scope mismatch rejects transition;
- expired decision rejects transition;
- stale source state rejects transition;
- invalid lifecycle transition rejects transition;
- duplicate identical transition is handled safely;
- conflicting duplicate is rejected;
- unauthorized retry and replay do not create permission;
- failed transition reports actual state;
- accepted transition produces exactly one lifecycle event;
- Stage 3 lifecycle behavior remains unchanged where no WP-02 authority integration is involved.

## Required Verification

- clean Release build;
- Architecture tests;
- Security tests;
- WP-01 verifier regression;
- Stage 2 regressions;
- Stage 3 WP-01 through WP-06 regressions;
- WP-02 positive and negative scenarios;
- deterministic replay;
- mutation tests for authority-to-transition bindings;
- exact source and evidence inventory.

## Explicit Non-Authorities

This authority does not permit:

- creating a second Lifecycle controller;
- changing the canonical Lifecycle vocabulary or transition graph;
- modifying `Foundation.Core/LifecycleControl.cs`;
- modifying `Foundation.Contracts`;
- modifying Contract Registry;
- WP-03 through WP-06 implementation;
- State ownership or durable persistence;
- Evidence Journal or accepted-fact implementation;
- concurrency, uncertain-write, or restart reconciliation beyond existing Stage 3 behavior;
- production FIL or Service Bus implementation;
- external packages or network dependencies;
- Application, trading, broker, market-data, Self-Awareness, or Guardian changes;
- commit, tag, merge, rebase, push, deployment, or runtime activation.
