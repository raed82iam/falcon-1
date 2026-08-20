# Stage 4 Requirement and Verification Matrix

## Canonical Deliverable Mapping

| Canonical Stage 4 deliverable | Work Package |
|---|---|
| Default-deny Authority Engine | WP-01 |
| CON-002 attributable decisions | WP-01 |
| Authoritative Lifecycle model and CON-003 transitions | WP-02 |
| FDN-001 state ownership enforcement | WP-03 |
| Current-state persistence | WP-03 |
| Integrity-linked evidence journal | WP-04 |
| Immutable events for accepted facts | WP-04 |
| Concurrency conflict handling | WP-05 |
| Uncertain-write handling | WP-05 |
| Restart reconciliation without fabricated state | WP-05 |
| VPL-002 and VPL-003 | WP-06 |

## State-Class Scope Mapping

The exact Stage 4 state classes are defined in:

- `07_STAGE_4_STATE_CLASS_SCOPE_AND_OWNERSHIP.md`

WP-03, WP-04, and WP-05 must not silently introduce additional authoritative state classes.

## Contract Mapping

| Requirement family | Primary WP | Final verification |
|---|---|---|
| CON-002-REQ-001..008 | WP-01 | VPL-002 |
| CON-003-REQ-001..008 | WP-02 | VPL-003 |
| FDN-001-REQ-001..022 | WP-03, WP-04, WP-05 | VPL-002 and VPL-003 |
| CON-008 evidence obligations | WP-04 | VPL-002 and VPL-003 |
| CON-009 security context | WP-01 | VPL-002 |
| CON-012 authority instrument | WP-01 | VPL-002 |
| CON-013 delegation and revocation | WP-01 | VPL-002 |
| CON-015 time evidence | WP-01, WP-03, WP-04 | both |
| CON-016 integrity provider | WP-04 | both |
| CON-020 bootstrap execution context | WP-02 | VPL-003 |
| CON-021 bootstrap provenance | WP-02, WP-04 | VPL-003 |

## VPL-002 FIL-Path Mapping

The mandatory FIL-path branch is resolved by:

- `08_STAGE_4_VPL002_FIL_PATH_RESOLUTION.md`

This uses a verification-only adapter and does not implement Stage 5 transport.

## VPL-002 Required Assertions

1. Authentication does not imply authorization.
2. Prior success does not imply authority.
3. Message validity does not imply authority.
4. Expired authority is denied.
5. Revoked authority is denied.
6. Retry and replay do not create authority.
7. The verification-only FIL-modeled path does not create authority.
8. No prohibited side effect occurs.
9. Authoritative state remains unchanged.
10. Denial evidence is complete.
11. Independent verification examines the state owner and execution boundary.

## VPL-003 Required Assertions

1. Exactly one authoritative lifecycle state exists.
2. Only a valid authorized transition changes it.
3. Invalid target is rejected.
4. Stale source is rejected.
5. Duplicate request cannot create duplicate transition.
6. Competing requests produce at most one successor.
7. Unauthorized requester is rejected.
8. Failed transition exposes actual state.
9. Completed transition emits exactly one authoritative event.
10. Restart does not fabricate or regress state.
11. Independent verification compares durable state and evidence.

## Implementation Boundary Mapping

Candidate project, verifier, integration, and prohibited boundaries are defined in:

- `09_STAGE_4_CANDIDATE_IMPLEMENTATION_BOUNDARIES.md`

A future WP authority must replace candidate boundaries with an exact path allowlist.

## Closure Evidence Per WP

Every WP implementation package must bind:

- exact repository branch, HEAD, and tree;
- exact file allowlist;
- payload hashes;
- build and verifier commands;
- positive and negative scenario inventory;
- deterministic evidence digests;
- mutation and replay results;
- regression results;
- rollback instructions;
- residual risks;
- explicit non-authorities.
