# FSATS A-to-Z Audit — Findings and Remediation Plan

Date: 2026-08-19

## Priority order

### Priority 1 — HIGH release blockers

1. Broker recovery evidence freshness and temporal coherence.
2. Canonical Trading safety-envelope binding to account/instrument/quantity/trust/protection identity.
3. Guardian monotonic protective-state transition and governed SafeMode recovery.

These three should be remediated before any future broker/risk-increasing runtime activation scope.

### Priority 2 — MEDIUM safety/governance hardening

4. Resource claim allocation/consumption/reclaimable coherence.
5. Failure-locality evidence freshness before scoped containment.
6. Current README/FCR state reconciliation.

### Priority 3 — LOW defense-in-depth

7. Uniform future-time/range/input validation in lower-level provider/simulator helpers.

## Remediation constraints

- Do not redesign the five-Application architecture.
- Do not move Foundation responsibilities into Application code.
- Do not modify Shared Web source from the Application workstream.
- Do not change historical Part/Owner closure records.
- Do not activate any runtime route as part of remediation.
- Do not create broker/provider/Paper/Shadow/TinyLive/Live authority.

## Required verification after remediation

The remediation candidate must receive all of the following against one exact commit:

```text
RESTORE = PASS
RELEASE BUILD = PASS
DOTNET TEST = PASS
ARCHITECTURE VERIFIER = PASS
SECURITY VERIFIER = PASS
BEHAVIOR VERIFIER = PASS
OPERATIONAL DATA OUTCOME VERIFIER = PASS
OWNER UPDATE GOVERNANCE VERIFIER = PASS
FOUNDATION BINDING VERIFIER = PASS
OWNER FEATURE ENTITLEMENT VERIFIER = PASS
INTEGRATION VERIFIER = PASS
FAILURE VERIFIER = PASS
ALL 9 GOVERNED VERIFIERS = PASS TWICE
NEW ADVERSARIAL TESTS FOR ALL AUDIT FINDINGS = PASS
FINAL TRACKED TREE = CLEAN
```

Then perform:

1. fresh Architecture / Consistency review;
2. fresh broad Red Team;
3. exact executable post-validation reconciliation;
4. Owner review and explicit final decision.

## Minimum adversarial additions

### Broker recovery

- stale observation;
- future observation;
- stale aggregate reconciliation;
- stale one-dimension evidence;
- future one-dimension evidence;
- incoherent observation/reconciliation time gap;
- exact fresh coherent happy path.

### Safety envelope

- wrong instrument;
- wrong account/position ownership context;
- wrong quantity coverage;
- stale trust epoch;
- stale/invalid protection state;
- envelope swapped between two queued orders;
- mutation after preparation rejected at queue/dispatch boundary.

### Guardian

- SafeMode + anomaly remains SafeMode;
- SafeMode + degraded remains SafeMode;
- stale trusted signal cannot relax protection;
- future signal rejected;
- invalid severity rejected;
- only governed recovery evidence/epoch permits relaxation.

### Resource management

- consumption greater than allocation cannot donate;
- reclaimable greater than actual safe headroom rejected;
- contradictory fresh/trusted claim rejected.

### Failure locality

- stale locality evidence expands containment;
- future locality evidence expands/rejects;
- fresh exact locality permits minimum-necessary containment.

### Provider/simulator defense-in-depth

- future provider observation rejected or non-current;
- invalid raw simulation/calibration values fail closed.

## Completion criterion

The audit may be reclassified from `FINDINGS_OPEN` to `CLEAN` only when the exact remediated source passes the full governed suite plus the new adversarial cases and the post-executable Red Team has no unresolved Critical/High/Medium release blockers.
