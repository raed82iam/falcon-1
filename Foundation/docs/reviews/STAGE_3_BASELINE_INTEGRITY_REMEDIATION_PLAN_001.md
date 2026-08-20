# Falcon Baseline Integrity Remediation Design

## Proposed branch

`stage3/baseline-integrity-remediation`

## Required parent

- Commit: `888fb661e9e32f253ea891c5d793d9852caf200d`
- Tree: `b2f9e5fc1439e4382bfb7484fd714e6d483bf2a9`
- Tag: `falcon-foundation-stage3-wp05-baseline-20260803`

The current clean `stage3/wp06-e2e-plugin-admission` branch remains preserved as a superseded design-initiation branch. It is not an implementation branch.

## Phase A — Documentary and build integrity

1. establish text, line-ending, SDK, and language-version controls;
2. reconcile all 27 active canonical activation targets;
3. repair active mojibake;
4. resolve active registry ownership and authority metadata;
5. update current overview and authority registry;
6. produce byte-level documentary inventory and cross-reference checks.

## Phase B — Core fail-closed and concurrency hardening

1. structured Contract Registry keys and deterministic snapshots;
2. first-observation Admission and Registration IDs;
3. null-safe contract, bootstrap, lifecycle, and authority validation;
4. synchronized mutable control state;
5. canonical identities and duplicate declaration rejection.

## Phase C — Enabling and security-provider hardening

1. future-time, uncertainty, overflow, and atomic identifier continuity;
2. exact key/secret reference binding;
3. custody lifetime synchronization;
4. immutable byte boundaries;
5. exact certificate subject rules;
6. evidence membership, digest, integrity, and profile-set validation.

## Phase D — Gate hardening and regression

1. harden the security test itself;
2. update existing verifiers for every corrected contract;
3. add `Falcon.BaselineIntegrity.Verifier`;
4. run all existing regression gates;
5. run deterministic replay from unchanged DLLs;
6. run concurrency stress and stale-reference challenge cases;
7. perform independent review without reusing implementation helpers.

## Phase E — Closure boundary

Successful implementation ends only at:

`BASELINE_INTEGRITY_REMEDIATED_AND_VERIFIED_READY_FOR_INDEPENDENT_REVIEW`

No commit or tag is authorized by the implementation authority candidate.

After independent review and Owner acceptance, a separate closure authority may create a local remediation commit and tag.

Only then may a fresh WP-06 branch be created from the remediated baseline.
