# AI Repair / Controlled Recovery Stale-Baseline Hardening V2

**Status:** `CONTROLLING HARDENING CANDIDATE / NOT_OWNER_ACCEPTED / NOT_CLOSED`

This record supplements records 09 and 12.

## Current-Validity Rule for R1 Recovery Targets

A previously approved/trusted artifact, baseline or deterministic corrective target is not automatically eligible for R1 merely because it was trusted historically.

Before automatic restoration/reload/rollback, the target SHALL be proven:

- exact and attributable;
- not revoked/superseded for the intended recovery use;
- compatible with current Application identity and manifest obligations;
- compatible with current security and permission requirements;
- compatible with current dependency/state/schema requirements;
- within the current authority ceiling;
- free of a known defect or incident that invalidates its reuse;
- supported by required current evidence.

```text
HISTORICALLY TRUSTED
!=
CURRENTLY ELIGIBLE FOR AUTOMATIC RECOVERY
```

If current eligibility cannot be established, R1 automatic return is denied and the incident escalates to R2/R3 as appropriate.

A killed/untrusted subject cannot select a stale baseline merely because it prefers that state.

## Non-Grant

No runtime pre-authorization is created by this documentary candidate.
