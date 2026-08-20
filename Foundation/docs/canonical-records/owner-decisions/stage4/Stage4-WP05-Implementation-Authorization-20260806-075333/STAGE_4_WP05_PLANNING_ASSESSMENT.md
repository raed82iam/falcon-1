# Falcon Foundation Stage 4 WP-05 Planning Assessment

## Status

```text
PLANNING_ONLY
NO_IMPLEMENTATION_AUTHORITY
WP05 = UNAUTHORIZED
WP06 = UNAUTHORIZED
GIT_DEPLOYMENT_ACTIVATION = UNAUTHORIZED
```

## Reviewed baseline

- Repository archive: `Falcon1(20260806-044627).zip`
- Branch: `stage3/baseline-integrity-remediation`
- HEAD: `888fb661e9e32f253ea891c5d793d9852caf200d`
- WP-01 through WP-04: accepted and closed

## Canonical WP-05 purpose

WP-05 implements bounded concurrency, uncertain-write classification, and restart reconciliation so Falcon preserves one truthful authoritative successor after conflict, interruption, or ambiguous persistence outcomes.

It must prove:

- expected-version concurrency;
- exactly one successful writer per authoritative version;
- identical duplicate request handling;
- conflicting duplicate rejection;
- stale-write rejection;
- uncertain-write classification;
- lookup by request and decision identity;
- no blind retry;
- reconciliation of authoritative state, decision, evidence, commit result, and accepted fact;
- restart from the last trusted authoritative state;
- explicit divergence classifications;
- no fabricated or regressed state;
- challengeable reconstruction when evidence is incomplete.

## Architectural decision proposed

Create a bounded project:

```text
src/Foundation.Reconciliation
```

This project should own:

- reconciliation request and result models;
- deterministic reconciliation classification;
- restart reconciliation orchestration;
- comparison of authoritative state, persistence commit evidence, Evidence Journal, and Accepted-Fact records;
- explicit divergence and uncertainty outcomes.

It must not own:

- Lifecycle transition execution;
- authoritative state writes;
- Authority decisions;
- Evidence Journal append authority;
- Accepted-Fact publication;
- general retry scheduling;
- WP-06 integrated closure.

## Dependency direction

```text
Foundation.Contracts
        ↓
Foundation.State
        ↓
Foundation.Evidence
        ↓
Foundation.Reconciliation
        ↓
Foundation.Infrastructure
```

`Foundation.Reconciliation` may reference `Foundation.Contracts`, `Foundation.State`, and `Foundation.Evidence`.

`Foundation.State` and `Foundation.Evidence` must not reference `Foundation.Reconciliation`.

`Foundation.Infrastructure` may consume reconciliation results only at the existing Lifecycle boundary.

## Proposed exact implementation allowlist

### New production boundary

```text
src/Foundation.Reconciliation/Foundation.Reconciliation.csproj
src/Foundation.Reconciliation/ReconciliationModels.cs
src/Foundation.Reconciliation/RestartReconciler.cs
src/Foundation.Reconciliation/ReconciliationClassifier.cs
```

### Existing State boundary

```text
src/Foundation.State/AuthoritativeStateModels.cs
src/Foundation.State/DurableAuthoritativeStateStore.cs
src/Foundation.State/FileAuthoritativeStateProvider.cs
src/Foundation.State/Foundation.State.csproj
```

Allowed purpose:

- expected-version write identity;
- persistence commit-result lookup;
- uncertain commit classification;
- duplicate logical request detection;
- conflicting duplicate rejection;
- stale expected-version rejection.

### Existing Evidence boundary

```text
src/Foundation.Evidence/EvidenceModels.cs
src/Foundation.Evidence/FileEvidenceJournalProvider.cs
src/Foundation.Evidence/IntegrityLinkedEvidenceJournal.cs
src/Foundation.Evidence/AcceptedFactPublisher.cs
src/Foundation.Evidence/Foundation.Evidence.csproj
```

Allowed purpose:

- lookup by request, decision, state version, and accepted-fact identity;
- expose read-only reconciliation evidence;
- preserve existing WP-04 append and publication authority.

### Existing integration boundary

```text
src/Foundation.Infrastructure/BootstrapLifecycleControl.cs
src/Foundation.Infrastructure/Foundation.Infrastructure.csproj
```

Allowed purpose:

- consume a reconciliation result before unrestricted restart continuation;
- fail closed on uncertain or divergent reconstruction;
- preserve the single existing Lifecycle controller.

### New verifier

```text
verification/Falcon.Stage4.WP05.Verifier/Falcon.Stage4.WP05.Verifier.csproj
verification/Falcon.Stage4.WP05.Verifier/Program.cs
```

### Test boundaries

```text
tests/Falcon.Foundation.Architecture.Tests/Program.cs
tests/Falcon.Foundation.Security.Tests/Program.cs
```

### Solution and documentation

```text
Falcon.Foundation.ControlledProjectFoundation.slnx
docs/governance/GOV-109_STAGE_4_WP05_IMPLEMENTATION_AUTHORIZATION.md
docs/stage-4-proposal/16_STAGE_4_WP05_IMPLEMENTATION_AUTHORITY.md
docs/governance/GOV-000_AUTHORITY_REGISTRY.md
docs/stage-4-proposal/README.md
```

## Explicitly prohibited

```text
src/Foundation.Core/LifecycleControl.cs
src/Foundation.Authority/**
Application projects
Trading projects
FIL production implementation
Service Bus production implementation
Guardian implementation
Deployment infrastructure
Secrets or live connectivity
WP-06 verifier or closure logic
Git commit, merge, tag, push, rebase
Runtime activation or deployment
```

Any modification outside the final authorized allowlist must fail closed.

## Required result classifications

WP-05 should use explicit deterministic classifications, including at minimum:

```text
CONSISTENT
DUPLICATE_IDENTICAL
CONFLICTING_DUPLICATE
STALE_WRITE
UNCERTAIN_BEFORE_COMMIT
UNCERTAIN_AFTER_COMMIT
STATE_AHEAD_OF_EVIDENCE
EVIDENCE_AHEAD_OF_STATE
ACCEPTED_FACT_MISSING
ACCEPTED_FACT_WITHOUT_DURABLE_STATE
CURRENT_STATE_CORRUPTED
EVIDENCE_JOURNAL_INVALID
TRUSTED_STATE_RECONSTRUCTED
CHALLENGE_REQUIRED
RECONCILIATION_FAILED_CLOSED
```

Names may be refined during implementation authority preparation, but no ambiguous success classification is acceptable.

## Required formal scenarios

1. Two writers use the same expected version; exactly one succeeds.
2. Identical replay returns the same governed outcome without a second effect.
3. Same request identity with changed content fails closed.
4. Stale expected version fails closed.
5. Failure before commit is classified without fabricating success.
6. Commit succeeds but response is lost; lookup reconstructs the committed result.
7. Crash during result return does not create a second state successor.
8. State exists but Evidence is incomplete; unrestricted continuation is blocked.
9. Evidence claims a state version not present durably; fail closed.
10. Accepted Fact is missing after durable state; reconciliation remains challengeable.
11. Accepted Fact exists without matching durable state; fail closed.
12. Current-state record is corrupted; reconstruct only from trusted history or fail closed.
13. Evidence Journal truncation is detected.
14. Restart after uncertain write reaches the same deterministic classification twice.
15. No blind retry occurs.
16. No state regression or fabricated state occurs.
17. WP-01 through WP-04 regressions remain PASS.
18. Stage 2 and Stage 3 regressions remain PASS.
19. Time is not used to expire Owner authority, work, continuation rights, or evidence validity.

## Required closure evidence

- exact branch, HEAD, and tree identity;
- exact authorized file allowlist;
- pre-change and applied source hashes;
- clean Release build;
- Architecture PASS;
- Security PASS with zero findings;
- WP-01 through WP-04 regression PASS;
- Stage 2 and Stage 3 regression PASS;
- WP-05 verifier PASS twice;
- deterministic output digest;
- mutation cases for duplicate, conflict, corruption, truncation, and uncertain commit;
- independent implementation review;
- separate final Owner acceptance.

## Planning decision

```text
WP05_PLANNING_ASSESSMENT = READY
RECOMMENDED_BOUNDARY = Foundation.Reconciliation
READY_TO_PREPARE_OWNER_IMPLEMENTATION_AUTHORIZATION
IMPLEMENTATION_NOT_YET_AUTHORIZED
```
