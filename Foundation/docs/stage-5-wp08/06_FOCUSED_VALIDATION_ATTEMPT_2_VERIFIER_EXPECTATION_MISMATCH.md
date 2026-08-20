# Stage 5 WP-08 — Focused Validation Attempt 2 Verifier Expectation Mismatch

**Work Package:** Stage 5 WP-08 — Cryptographic Message Protection  
**Date:** 2026-08-08  
**Attempt:** Focused Validation Attempt 2  
**Validated HEAD:** `7d2fe81e065e5369a222f8fd8ecb295a0234f575`  
**Outcome:** PARTIAL PASS — verifier expectation mismatch, remediation required and completed

## 1. Passed gates before WP-08 execution

- Restore: PASS
- Release Build: PASS
- Architecture Tests: PASS
- Security Tests: PASS, 133 files scanned, 0 findings
- Stage 5 WP-01 regression: PASS
- Stage 5 WP-02 regression: PASS
- Stage 5 WP-03 regression: 30/30 PASS
- Stage 5 WP-04 regression: 53/53 PASS
- Stage 5 WP-05 regression: 51/51 PASS
- Stage 5 WP-06 regression: 58/58 PASS
- Stage 5 WP-07 regression: 48/48 PASS

## 2. WP-08 result

WP-08 execution reached `47/48 PASS`.

The only failing scenario was:

- `wrong_recipient_context_rejected`

All other cryptographic message-protection scenarios passed.

## 3. Classification

`VERIFIER_EXPECTATION_MISMATCH`

The production `MessageProtector.Verify()` performs governed profile/key/context input validation before the protected-context digest comparison. When the expected recipient scope is substituted, the supplied key reference no longer permits that recipient scope and validation fails closed with:

`CRYPTO_KEY_SCOPE_MISMATCH`

The verifier scenario had expected the later generic reason:

`CRYPTO_CONTEXT_MISMATCH`

The security behavior was therefore fail closed and stricter/more specific than the verifier expectation. No plaintext was released and no cryptographic bypass was observed.

## 4. Bounded remediation

Only the WP-08 verifier was changed.

- `wrong_recipient_context_rejected` now expects `CRYPTO_KEY_SCOPE_MISMATCH`.
- Verifier helper methods were renamed to `CreateProfile`, `CreateKey`, `CreateContext`, and `DefaultPlaintext` to avoid the prior record-property/helper name-shadowing class of failure.
- All 48 named scenarios remain present.
- `Foundation.MessageProtection` production source was not modified by this remediation.

## 5. Current status

```text
WP08_FOCUSED_VALIDATION_ATTEMPT_2 = 47/48_PASS
WP08_ATTEMPT_2_FAILURE_CLASSIFICATION = VERIFIER_EXPECTATION_MISMATCH
WP08_PRODUCTION_CRYPTO_DEFECT = NOT_OBSERVED
WP08_VERIFIER_REMEDIATION = APPLIED
WP08_FOCUSED_VALIDATION = RERUN_REQUIRED
WP08_OWNER_ACCEPTANCE_AND_CLOSURE = NOT_GRANTED
WP09_WP10 = UNAUTHORIZED
```

A complete focused-validation rerun from Restore through two deterministic WP-08 verifier executions is required before focused validation can be accepted as PASS.
