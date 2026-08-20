# Stage 4 WP-01 Implementation Authorization

**Identifier:** GOV-103  
**Version:** 1.0  
**Status:** Owner confirmation required by authorization script  
**Scope:** Stage 4 WP-01 only  
**WP title:** Default-Deny Authority Engine

## Purpose

Authorize a bounded implementation of the reusable CON-002 Authority Decision boundary.

WP-01 decides whether an exact actor may perform an exact action on an exact resource under an exact policy, scope, purpose, context, delegation, revocation, fitness, and time condition.

The Authority Engine returns an attributable `ALLOW` or `DENY` decision.

It does not execute the action and does not mutate Lifecycle, State, Persistence, or Evidence.

## Exact Implementation Allowlist

### New production paths

- `src/Foundation.Authority/Foundation.Authority.csproj`
- `src/Foundation.Authority/AuthorityEngine.cs`

### New verification paths

- `verification/Falcon.Stage4.WP01.Verifier/Falcon.Stage4.WP01.Verifier.csproj`
- `verification/Falcon.Stage4.WP01.Verifier/Program.cs`

### Existing integration and validation paths

- `Falcon.Foundation.ControlledProjectFoundation.slnx`
- `tests/Falcon.Foundation.Architecture.Tests/Program.cs`
- `tests/Falcon.Foundation.Security.Tests/Program.cs`

### WP-01 documentation paths

- `docs/stage-4-proposal/10_STAGE_4_WP01_IMPLEMENTATION_AUTHORITY.md`
- `docs/reviews/STAGE_4_INDEPENDENT_PLANNING_REVIEW.md`
- `docs/governance/GOV-103_STAGE_4_WP01_IMPLEMENTATION_AUTHORIZATION.md`
- `docs/governance/GOV-000_AUTHORITY_REGISTRY.md`
- `docs/stage-4-proposal/README.md`

No other path is authorized.

## Authorized Deliverables

- default-deny Authority Engine;
- deterministic Authority Decision;
- policy identity and version binding;
- requested-scope containment;
- expiry handling;
- delegation and revocation handling;
- fitness and security-context handling;
- attributable reason codes;
- deterministic reconstruction;
- independent WP-01 verifier;
- architecture and security regression updates required only for the new bounded project.

## Required Scenarios

- valid allow;
- explicit deny;
- malformed request;
- unknown actor;
- missing policy;
- ambiguous policy;
- excessive scope;
- expired request or authority;
- revoked delegation;
- delegation scope exceeded;
- insufficient fitness;
- security-context rejection;
- deterministic replay;
- same request and policy producing the same decision identity;
- no execution or state mutation from Authority Engine.

## Required Verification

- clean Release build;
- Architecture tests;
- Security tests;
- Stage 2 WP-01 and WP-03 regressions;
- Stage 3 WP-01 through WP-06 regressions;
- WP-01 positive and negative scenarios;
- deterministic second run;
- exact source and evidence inventory.

## Explicit Non-Authorities

This authority does not permit:

- WP-02 through WP-06 implementation;
- Lifecycle integration or modification;
- State ownership or persistence implementation;
- Evidence Journal or accepted-fact implementation;
- concurrency or restart reconciliation implementation;
- production FIL or Service Bus implementation;
- changes to `Foundation.Contracts` unless separately authorized;
- changes to Contract Registry;
- external packages or network dependencies;
- Application, trading, broker, market-data, Self-Awareness, or Guardian changes;
- commit, tag, merge, rebase, push, deployment, or runtime activation.
