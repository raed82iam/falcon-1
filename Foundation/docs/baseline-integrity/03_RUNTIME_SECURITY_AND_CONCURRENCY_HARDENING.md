# Runtime, Security, Concurrency, and Evidence Hardening Audit

## Status

**REMEDIATION REQUIRED**

## Mandatory hardening groups

### 1. Build and source-text determinism

- add `global.json` for SDK `10.0.302`;
- pin C# `14.0`;
- add `.gitattributes` and `.editorconfig`;
- require UTF-8 and governed line endings;
- prevent untracked stage evidence and IDE caches from polluting repository status.

### 2. Public-boundary fail-closed behavior

Every public validation/evaluation entry point in scope must treat null, malformed, incomplete, and adversarial nested input as a deterministic rejection or failed validation result.

No verifier may accept an unhandled exception as correct fail-closed behavior.

### 3. Stateful identity reservation

- non-empty `AdmissionId` is consumed at first observation;
- non-empty `RegistrationId` is consumed at first observation;
- lifecycle identity rules remain unchanged and continue to reserve at first observation;
- rejected attempts emit no success evidence.

### 4. Structured identities

Delimiter-composed registry, subject, evidence, and reference keys are replaced with structured ordinal keys or length-prefixed canonical encodings.

### 5. Concurrency

Stateful registries, admission control, service catalog, identifier continuity, custody, and secret operations must be linearizable under concurrent calls.

All exposed collections are immutable snapshots with deterministic order.

### 6. Time and uncertainty

Reject:

- future verification evidence;
- negative uncertainty;
- default timestamps;
- overflow-prone uncertainty arithmetic;
- stale evidence beyond the approved bound.

### 7. Cryptographic and secret reference safety

Rotate, use, and revoke require exact reference ID, version, domain, and purpose binding.

Stale references must fail closed and must not affect the current material.

Mutable byte arrays must not expose internal state.

### 8. Evidence completeness

- canonical hexadecimal digests;
- unique requirement and evidence identities;
- no undeclared evidence;
- integrity independently verified rather than trusted from caller booleans;
- exact unique provider profile set.

### 9. Security-gate integrity

The security test must:

- discover the repository root deterministically;
- fail when governed roots are absent;
- fail on unreadable files;
- scan source, tests, verification, and root configurations;
- prove minimum/nonzero scan counts;
- retain explicit prohibited-surface checks.

### 10. WP-06 seam preservation

The remediation includes regression corrections needed for the already identified WP-02 → WP-04 → WP-05 seams, but it does not implement or close WP-06.

WP-06 receives a new authority only after the remediated baseline is accepted and tagged.
