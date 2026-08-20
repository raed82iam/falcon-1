# Stage 4 Candidate Implementation Boundaries

## Status

Planning candidate only. No implementation authority is granted.

## Architectural Rule

Stage 4 must not collapse into a universal `Foundation.Infrastructure` module.

Authority decision, authoritative state, persistence, and evidence must remain separated by dependency direction and responsibility.

## Candidate Project Boundaries

| Work Package | Candidate production boundary | Candidate verifier boundary | Candidate integration points | Prohibited shortcuts |
|---|---|---|---|---|
| WP-01 | `src/Foundation.Authority` or equivalent bounded project | `verification/Falcon.Stage4.WP01.Verifier` | `Foundation.Contracts`, Contract Registry, Security/fitness inputs | Writing lifecycle state, persistence, or evidence directly from Authority Engine |
| WP-02 | Existing Stage 3 lifecycle owner plus minimal authority integration boundary | `verification/Falcon.Stage4.WP02.Verifier` | `Foundation.Infrastructure/BootstrapLifecycleControl.cs`, lifecycle contracts, Authority Result consumption | Creating a second Lifecycle controller or rewriting Stage 3 lifecycle behavior |
| WP-03 | `src/Foundation.State` and a bounded persistence abstraction/provider | `verification/Falcon.Stage4.WP03.Verifier` | `Foundation.Contracts`, lifecycle owner, state registry | Treating cache, log, timestamp, or provider storage as authority |
| WP-04 | `src/Foundation.Evidence` or equivalent bounded project | `verification/Falcon.Stage4.WP04.Verifier` | Integrity provider, state commit result, authority decision result | Using ordinary logs as authoritative evidence or emitting accepted fact before commit |
| WP-05 | State/persistence/reconciliation boundaries only | `verification/Falcon.Stage4.WP05.Verifier` | State provider, Evidence Journal, accepted-fact store | Blind retry, last-write-wins, or fabricated restart state |
| WP-06 | No new production logic unless a verified gap is separately authorized | `verification/Falcon.Stage4.WP06.Verifier` | VPL-002 adapter, VPL-003 integrated verification, regression scripts | Fixing production code inside closure verifier without separate authority |

## Candidate Shared Contract Changes

Potential contract additions or extensions may be required for:

- Authority Request;
- Authority Result;
- State Ownership Declaration;
- Authoritative State Record;
- Persistence Commit Result;
- Evidence Journal Entry;
- Accepted-Fact Event;
- Uncertain Write Result;
- Reconciliation Result.

Before any contract identity is created or changed, the WP implementation authority must decide whether the requirement is:

1. an extension of an existing canonical contract;
2. a new canonical contract identity;
3. an internal record that does not cross a governed boundary.

## Candidate Test Boundaries

Potential updates may include:

- `tests/Falcon.Foundation.Architecture.Tests`
- `tests/Falcon.Foundation.Security.Tests`

Only the exact files named in a future WP authority may be modified.

## Candidate Solution and Build Boundaries

Potential updates may include:

- `Falcon.Foundation.ControlledProjectFoundation.slnx`
- `Directory.Build.props` only if a separately proven requirement exists.

No solution or build file is implicitly authorized by this planning document.

## Exact Future Allowlist Rule

Before each WP implementation:

1. bind repository branch, HEAD, and tree;
2. enumerate every production, test, verifier, project, solution, and documentation path;
3. bind payload hashes for supplied files;
4. name all new dependencies;
5. declare dependency direction;
6. declare expected build and verifier commands;
7. declare rollback;
8. declare residual risks;
9. forbid all paths outside the exact allowlist;
10. keep commit, tag, merge, rebase, push, deployment, and runtime activation unauthorized.

## Explicitly Prohibited Paths Unless Separately Justified

- Application projects;
- trading projects;
- market-data projects;
- broker projects;
- Self-Awareness projects;
- FIL and Service Bus production projects;
- Guardian implementation;
- deployment infrastructure;
- secrets, credentials, or live connectivity configuration.
