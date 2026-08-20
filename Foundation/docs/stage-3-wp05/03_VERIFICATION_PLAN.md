# Stage 3 WP-05 Verification Plan

## Clean sequence

1. Delete controlled `bin` and `obj` outputs.
2. Restore the controlled solution using the isolated NuGet configuration.
3. Build one clean Release with zero warnings and zero errors.
4. Run Architecture Tests and Security Tests from that Release output.
5. Run Stage 2 WP-01 through WP-04.
6. Run Stage 3 WP-01 through WP-04.
7. Hash the WP-05 verifier DLL.
8. Run WP-05 twice without rebuilding.
9. Prove the DLL remained unchanged and complete outputs are identical.
10. Run `git fsck` and confirm exact path scope.
11. Run a second independent challenge against the same Release assemblies.

## Mandatory remediation scenarios

- canonical policy accepts valid evidence;
- caller-selected policy values fail closed;
- unapproved graph digest fails closed;
- identity reuse after contract rejection fails closed;
- transition/event reuse after unknown-subject rejection fails closed;
- non-empty transition/event identities are consumed even when request ID is missing;
- missing authority record fails closed;
- missing time-provider record fails closed;
- missing running dependency record fails closed;
- evidence-bundle digest mismatch fails closed;
- lifecycle entry after bootstrap expiry fails closed;
- restricted `STOPPED → RECOVERING` without release fails closed;
- controlled release permits recovery and clears restriction only after acceptance;
- `RECOVERING → READY` without independent validation fails closed;
- validated recovery succeeds;
- restart limit and terminal retirement remain enforced.

## Pass rule

All gates must pass from one clean Release build, WP-05 output must be deterministic, no unauthorized path may change, and the second independent challenge must not reproduce any original finding.
