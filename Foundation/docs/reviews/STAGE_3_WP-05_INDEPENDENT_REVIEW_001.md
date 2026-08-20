# Stage 3 WP-05 Independent Review 001

## Status

**BLOCKED — FINDINGS REPRODUCED**

## Evidence

The initial WP-05 clean build and existing regression suite passed. A separate challenge executable was then built outside the Falcon repository and executed against the exact Release assemblies.

- Challenge assessment: `WP05_BLOCKING_FINDINGS_REPRODUCED`
- Restore exit code: `0`
- Challenge build exit code: `0`
- Challenge run exit code: `0`
- Blocking findings reproduced: `5`
- Challenge execution errors: `0`
- Repository changed-path set unchanged: `True`
- Staged paths after: `0`

## Blocking findings

1. Request, transition, and event identities were reusable after selected rejection paths.
2. Bootstrap expected bindings were supplied by the same request being validated.
3. Lifecycle authority, time, dependency, release, and recovery claims were caller booleans rather than validated records.
4. Accepted bootstrap evidence expiry was not enforced at lifecycle entry.
5. A protectively restricted `STOPPED` subject had no controlled recovery path.

## Decision

WP-05 was not accepted, committed, tagged, merged, or pushed. `GOV-096` ceased to provide execution authority after its stop condition was met. A new prospective remediation authority was required.
