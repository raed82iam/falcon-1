# Stage 8 WP-05 CON-011 Consumer Remediation

Status: PRE-EXECUTABLE RETEST CHECKPOINT

Observed failure:
- WP-05 retest reached the WP-04 regression verifier.
- WP-04 failed because the Authority consumer classified the canonical Guardian-published CON-011 record as malformed after the producer was corrected from `ACTIVE` to `IMPOSED`.

Root cause:
- `GuardianRestrictionContractPublisher` correctly publishes `RestrictionRecord.Result = IMPOSED`.
- `ProtectiveRestrictionAuthorityEnforcer.IsStructurallyValid()` still required `Result = ACTIVE`.
- The consumer therefore rejected a canonical imposed restriction and emitted `RestrictionMalformed` instead of `RestrictedByGuardian`.

Remediation:
- Preserve the canonical CON-011 producer value `IMPOSED`.
- Update the Authority consumer to require `IMPOSED` for an enforceable imposed restriction.
- Do not weaken the canonical validator.
- Do not change the WP-04 expected denial semantics.
- Do not add Stage 9 recovery/release behavior.

Required retest:
- Controlled Release build.
- Architecture and Security gates.
- Stage 7 cross-stage regression.
- Stage 8 WP-01 through WP-04 regressions.
- Stage 8 WP-05 executable verifier twice.
- Determinism and binary hash stability.
- Exact candidate HEAD and clean worktree.

Owner closure is not requested at this checkpoint.
