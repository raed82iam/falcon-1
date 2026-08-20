# GOV-108 — Stage 4 WP-04 Implementation Authorization

## Status

APPROVED / EFFECTIVE only after exact Owner confirmation through the authorization script.

## Authorized Work Package

**Stage 4 WP-04 — Integrity-Linked Evidence Journal and Immutable Accepted Facts**

## Purpose

Authorize a bounded implementation that makes Foundation decisions, denials, durable state effects, and accepted completed facts attributable, reconstructable, append-only, and tamper-evident.

## Required Outcomes

WP-04 shall implement:

1. append-only evidence records;
2. unique deterministic evidence identities;
3. integrity linkage to the prior accepted journal record;
4. explicit actor, request, decision, reason, state subject, state version, source identity, and persistence outcome;
5. evidence for both allow and deny decisions;
6. execution-boundary evidence;
7. persistence-outcome evidence;
8. one immutable accepted-fact event only after a proven durable accepted state effect;
9. no accepted-fact event for denied, rejected, incomplete, partial, corrupted, conflicting, or failed actions;
10. detection of gap, deletion, insertion, replacement, reorder, and duplication;
11. correction by a new evidence record, never by rewriting history;
12. deterministic replay from unchanged bytes.

## Time Independence Direction

Time may be recorded only as optional descriptive metadata.

WP-04 shall not:

- expire Owner authority;
- invalidate work because the Owner paused;
- require completion within a time window;
- use time as the source of permission;
- deny continuation because time passed;
- make journal integrity depend on wall-clock progression.

Integrity, authority, and continuation shall be proven by identity, version, state, evidence, lineage, and Owner authority.

## Exact Production Boundary

Authorized new production project:

- `src/Foundation.Evidence/Foundation.Evidence.csproj`
- `src/Foundation.Evidence/EvidenceModels.cs`
- `src/Foundation.Evidence/IntegrityLinkedEvidenceJournal.cs`
- `src/Foundation.Evidence/FileEvidenceJournalProvider.cs`
- `src/Foundation.Evidence/AcceptedFactPublisher.cs`

Authorized integration paths:

- `src/Foundation.Infrastructure/Foundation.Infrastructure.csproj`
- `src/Foundation.Infrastructure/BootstrapLifecycleControl.cs`

## Exact Verification and Test Boundary

- `verification/Falcon.Stage4.WP04.Verifier/Falcon.Stage4.WP04.Verifier.csproj`
- `verification/Falcon.Stage4.WP04.Verifier/Program.cs`
- `tests/Falcon.Foundation.Architecture.Tests/Program.cs`
- `tests/Falcon.Foundation.Security.Tests/Program.cs`
- `Falcon.Foundation.ControlledProjectFoundation.slnx`

## Exact Documentation Boundary

- `docs/governance/GOV-108_STAGE_4_WP04_IMPLEMENTATION_AUTHORIZATION.md`
- `docs/stage-4-proposal/15_STAGE_4_WP04_IMPLEMENTATION_AUTHORITY.md`
- `docs/governance/GOV-000_AUTHORITY_REGISTRY.md`
- `docs/stage-4-proposal/README.md`

## Dependency Direction

The implementation shall preserve:

```text
Foundation.Evidence -> Foundation.Contracts
Foundation.Evidence -> Foundation.State
Foundation.Evidence -> Foundation.Authority
Foundation.Infrastructure -> Foundation.Evidence
```

No dependency from Authority, State, or Contracts back to Evidence is authorized.

## Required Verification Scenarios

The WP-04 verifier shall prove at least:

1. first journal append;
2. deterministic evidence identity;
3. valid integrity-linked successor;
4. allow-decision evidence;
5. deny-decision evidence;
6. accepted durable state change creates exactly one accepted-fact event;
7. rejected or failed durable state change creates no accepted-fact event;
8. commit rejection remains attributable without fabricating acceptance;
9. duplicate identical evidence is idempotent or rejected deterministically;
10. duplicate identity with changed content fails closed;
11. deleted record is detected;
12. inserted record is detected;
13. replaced record is detected;
14. reordered records are detected;
15. broken prior-link is detected;
16. journal truncation is classified explicitly;
17. correction appends a new record and does not rewrite history;
18. unchanged durable bytes replay identically;
19. application business state remains out of scope;
20. time is not used as a validity or permission gate.

## Explicit Non-Authorities

This authorization does not permit:

- WP-05 concurrency, uncertain-write, retry, or restart reconciliation;
- WP-06 integrated closure;
- modification of `Foundation.Core/LifecycleControl.cs`;
- modification of `Foundation.Contracts`;
- modification of Contract Registry;
- ordinary logs becoming authoritative evidence;
- accepted-fact emission before durable commit proof;
- external packages;
- external connectivity;
- FIL or Service Bus production;
- application, trading, broker, market-data, Guardian, or Self-Awareness implementation;
- secrets, credentials, signing keys, or live cryptographic infrastructure;
- Git commit, tag, merge, rebase, or push;
- deployment or runtime activation;
- any path outside the exact allowlist.

## Closure Condition

WP-04 remains open until:

- Release build passes;
- Architecture and Security gates pass;
- WP-01 through WP-03 regressions pass;
- Stage 2 and Stage 3 regressions pass;
- WP-04 verifier passes twice with identical output;
- an independent implementation review passes;
- the Owner explicitly accepts and closes WP-04.
