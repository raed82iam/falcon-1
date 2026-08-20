# Stage 6 WP-07 — Executable Verifier Fixture Remediation Red-Team

Status: PASS
Date: 2026-08-10
Scope: verifier-only remediation after executable validation reached WP-07 Run 1.

## Trigger

Executable validation against production-remediated HEAD `58faf956f2e94424275e3b01cccbbcc7d04ac2b1` passed Restore, Release Build, Foundation Architecture, Foundation Security, and Stage 6 WP-01 through WP-06 predecessor verifiers. WP-07 verifier Run 1 reached 27/28 and failed only at `restore_above_basis_rejected` before reaching the intended Restore-above-basis assertion.

Observed exception:
`ArgumentException: Maximum borrow-out exceeds movable granted capacity. (Parameter 'members')`

## Root Cause

The failing verifier fixture constructed a reduced authoritative allocation for AppA with allocation=10 and then created the default coordination envelope with `minA=10` and `outA=10`. Under the accepted WP-07 production rule, movable granted capacity is `allocation - protected minimum = 0`, therefore positive borrow-out authority of 10 is correctly rejected before the test can exercise Restore-above-basis behavior.

This is a verifier fixture defect, not a production defect.

## Remediation

Verifier file only:
`verification/Falcon.Stage6.WP07.Verifier/ProgramV2.cs`

Exact change inside `RestoreAboveBasisRejected`:
- before: `Effective(Envelope(reduced), reduced)`
- after: `Effective(Envelope(reduced, outA: 0), reduced)`

Rationale: this test needs an exact quiesced effective-distribution witness only. It does not require or test borrow-out authority. Setting AppA borrow-out to zero makes the fixture valid while preserving the intended production assertion that a Restore target exceeding the captured historical basis must be rejected.

Remediation commit:
`23a6aa178966bf347ad891476a02b9a173a9f921`

## Red-Team Checks

1. Production code changed: NO.
2. Foundation contracts changed: NO.
3. WP-01 through WP-06 accepted behavior changed: NO.
4. WP-07 authority semantics changed: NO.
5. Reclaimability semantics weakened: NO.
6. Protected-effective-minimum enforcement weakened: NO.
7. Quota/ceiling headroom converted into granted capacity: NO.
8. Restore historical-basis bound weakened: NO.
9. WP-08 surface introduced: NO.
10. Test now bypasses intended Restore assertion: NO. The fixture is corrected only so execution reaches that assertion.

## Result

Critical: 0
High: 0
Medium: 0

PASS

## Authority / Closure

`WP07_IMPLEMENTATION_AUTHORITY = GRANTED`
`WP07_EXECUTABLE_VALIDATION = REMEDIATED_REVALIDATION_REQUIRED`
`WP07_TECHNICAL_ACCEPTANCE = NOT_YET`
`WP07_OWNER_CLOSURE = NOT_YET`
`WP08_IMPLEMENTATION_AUTHORITY = NOT_GRANTED`

No Application compatibility handoff or Owner closure is authorized until full exact-HEAD executable validation passes, including WP-07 verifier twice from the same Release outputs and final repository integrity verification.
