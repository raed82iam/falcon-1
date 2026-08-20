# GOV-099 — Stage 3 Baseline Integrity Remediation Execution Authority

## Status

**APPROVED / EFFECTIVE FOR CONTROLLED BASELINE-INTEGRITY REMEDIATION**

## Owner decision

- Owner: `Raed Ammoura`
- Decision: `APPROVED`
- Approval timestamp: `2026-08-03T11:23:53+03:00`
- Approval reference:
  `OWNER-APPROVAL-GOV-099-BASELINE-INTEGRITY-20260803`

## Effectiveness

This authority became effective when the Owner executed the approval package on `2026-08-03T11:23:53+03:00`. It authorizes only the listed baseline-integrity remediation and verification work. It does not authorize commit, tag, WP-06 implementation, or Stage 4.

## Supersession

This candidate supersedes the unapproved external WP-06 GOV-099 candidate.

The earlier candidate never became effective and shall remain historical evidence only.

## Bound baseline

- Required source branch:
  `stage3/wp05-bootstrap-lifecycle`
- Required commit:
  `888fb661e9e32f253ea891c5d793d9852caf200d`
- Required tree:
  `b2f9e5fc1439e4382bfb7484fd714e6d483bf2a9`
- Required annotated tag:
  `falcon-foundation-stage3-wp05-baseline-20260803`
- Required tag object:
  `6e267607c6cddf6b8204d478b65149b48fbc3aed`
- Required working tree: clean

## Authorized branch

Create one local branch:

`stage3/baseline-integrity-remediation`

from the exact required commit.

The current `stage3/wp06-e2e-plugin-admission` branch shall remain untouched and on hold.

## Authorized objective

Correct current active baseline integrity defects discovered by the full-repository audit while preserving all historical, archived, candidate, closure, and frozen baseline evidence.

## Exact implementation boundary

Only the `71` paths in the approved allowlist may be added or modified.

No other path is authorized.

## Authorized remediation groups

1. active documentary activation reconciliation;
2. active UTF-8 and metadata correction;
3. build SDK, language, encoding, and line-ending pinning;
4. security-test fail-closed hardening;
5. public-boundary null and malformed-input rejection;
6. stateful concurrency and deterministic snapshots;
7. structured identity and first-observation replay prevention;
8. time, uncertainty, identifier continuity, stale key/secret reference, certificate, and byte-alias hardening;
9. evidence-set and environment-profile hardening;
10. regression correction for the known WP-02 → WP-04 → WP-05 evidence seams;
11. one new baseline-integrity verifier;
12. design, governance, manifest, verification, and independent-review evidence.

## Historical preservation rule

The authority does not permit modification of:

- `docs/archive/**`;
- candidate source packages under `docs/activation/candidates/**`;
- prior review or closure evidence, except creation of new correction/review records at new paths;
- the WP-05 commit or tag.

Current canonical targets may be corrected because they are active surfaces, not frozen historical evidence.

## Required result

The implementation and clean verification must end at:

`BASELINE_INTEGRITY_REMEDIATED_AND_VERIFIED_READY_FOR_INDEPENDENT_REVIEW`

## Stop conditions

Stop immediately before commit if:

- baseline branch, commit, tree, tag, or cleanliness differs;
- any non-allowlisted path changes;
- historical/candidate/archive bytes change;
- any active canonical target remains stale or corrupted;
- any warning or build error occurs;
- any existing regression fails;
- any negative proof fails;
- complete verifier output is nondeterministic;
- any concurrency or stale-reference challenge reproduces a defect;
- the security scanner can false-pass;
- a new blocking finding appears.

## Commit boundary

This approved authority does not authorize:

- staging;
- commit;
- tag;
- moving `main`;
- merge or rebase;
- push;
- deployment or runtime activation;
- network, cloud, Service Bus, Event Bus, FIL transport, broker, market-data, trading, or financial activity;
- WP-06 implementation;
- Stage 4.

A separate closure authority is required after independent review and final Owner acceptance.
## Controlled correction GOV-099-CORR-001

- Correction approval timestamp: `2026-08-03T12:59:43+03:00`
- Correction approval reference:
  `OWNER-APPROVAL-GOV-099-CORR-001-BASELINE-INTEGRITY-20260803`
- Incorrect allowlisted path removed:
  `verification/Falcon.Stage0C.Remediation.Verifier/Program.cs`
- Correct repository path authorized:
  `verification/Falcon.Stage0C.RemediationVerifier/Program.cs`
- Exact changed-path count remains: `71`
- Scope, objective, non-authorities, stop conditions, and commit boundary are unchanged.

This is a path-identity correction only. It does not expand the approved remediation purpose or authorize any additional operation.
