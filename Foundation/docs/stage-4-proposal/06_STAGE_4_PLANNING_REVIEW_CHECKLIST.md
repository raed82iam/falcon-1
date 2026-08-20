# Stage 4 Planning Review Checklist

## Source Fidelity

- [ ] Plan preserves the canonical Stage 4 title and purpose.
- [ ] Every canonical deliverable maps to a Work Package.
- [ ] CON-002 obligations are fully mapped.
- [ ] CON-003 obligations are fully mapped.
- [ ] FDN-001 requirements are fully mapped.
- [ ] VPL-002 procedure and pass rule are preserved.
- [ ] VPL-003 procedure and pass rule are preserved.

## State-Class Scope

- [ ] Exact Stage 4 state classes are enumerated in document 07.
- [ ] Each class has an authoritative owner and source.
- [ ] Persistence, concurrency, recovery, read, write, and retention rules are declared.
- [ ] Application business state remains out of scope.
- [ ] Additional state classes require a reviewed amendment.

## VPL-002 FIL Path

- [ ] Document 08 explicitly resolves the approved FIL-path requirement.
- [ ] The selected adapter is verification-only.
- [ ] No Stage 5 production transport is introduced.
- [ ] Retry and replay are exercised through the modeled FIL boundary.
- [ ] Independent verification examines authoritative state directly.

## Candidate Implementation Boundaries

- [ ] Document 09 identifies candidate production boundaries per WP.
- [ ] Candidate verifier boundaries are named.
- [ ] Candidate integration points are named.
- [ ] Prohibited shortcuts are explicit.
- [ ] Each future WP authority must provide an exact path allowlist.

## No Stage 3 Duplication

- [ ] Existing Stage 3 Lifecycle implementation is reused.
- [ ] No second Lifecycle controller is proposed.
- [ ] Bootstrap Context and Dependency evidence remain intact.
- [ ] Stage 3 regression gates remain mandatory.
- [ ] Accepted Stage 3 deterministic behavior is not silently redefined.

## Architecture

- [ ] Authority decision is separate from execution.
- [ ] State owner is separate from observer and cache.
- [ ] Persistence technology cannot redefine ownership.
- [ ] Evidence cannot replace authoritative state.
- [ ] Accepted event occurs only after proven state effect.
- [ ] Restart cannot fabricate truth.

## Work Package Quality

- [ ] Each WP has one bounded purpose.
- [ ] Dependencies are sequential and explicit.
- [ ] Each WP has measurable exit criteria.
- [ ] Each WP can have an exact implementation allowlist.
- [ ] Failure and rollback are defined.
- [ ] Independent verification is required where the baseline requires it.

## Governance

- [ ] Planning documents are stored under `docs/stage-4-proposal`.
- [ ] External authority evidence remains under `C:\Falcon\Stage4`.
- [ ] Documents are not runtime policy or state.
- [ ] Implementation remains unauthorized.
- [ ] Git operations remain unauthorized.

## Planning Review Decision

A successful independent planning review may produce:

```text
STAGE4_PLANNING_REVIEW = PASS
READY_FOR_OWNER_STAGE4_IMPLEMENTATION_AUTHORITY_REVIEW
```

It does not itself authorize implementation.
