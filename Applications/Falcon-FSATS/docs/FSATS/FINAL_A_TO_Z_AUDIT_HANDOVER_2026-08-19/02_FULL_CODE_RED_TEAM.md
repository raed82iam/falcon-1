# FSATS Full Code Red Team

Date: 2026-08-19
Audit baseline: `5261300fd34c1116d2347d031eb89c78d25e7aca`
Last exact executable-tested source: `4c2b465ccf46ce557386478b73bb2440ab39fe0d`

## Disposition

```text
CRITICAL = 0
HIGH = 3
MEDIUM = 3
LOW = 1
RED_TEAM_RESULT = FINDINGS_OPEN / NOT CLEAN
CURRENT_LIVE_EXPLOITABILITY = CONSTRAINED_BY_DISABLED_RUNTIME_EGRESS
FUTURE_RUNTIME_RELEASE = BLOCKED_BY_HIGH_FINDINGS
```

The exact executable verifier suite remains PASS, but the fresh adversarial manual review found safety cases outside current automated coverage.

## Attack families reviewed

- authority inference and self-grant
- Foundation authority substitution
- cross-Application dependency leakage
- raw broker/provider network egress
- replay/test/simulation truth escalation
- stale/future evidence
- identity substitution
- mismatched contract/payload/provenance/digest
- idempotency conflict and ambiguous dispatch outcomes
- execution/containment race
- stale trust epochs
- recovery/release semantic collapse
- Guardian protective-state relaxation
- broker reconciliation ambiguity
- capital/resource overcommit
- provider quota ambiguity
- simulator-to-operational truth escalation
- current documentation misleading future workstream authority

## HIGH findings

### RT-H-01 — Broker outage recovery permits stale evidence to resume risk

Affected area:
`Trading.Application/BrokerOutageRecovery.cs`

Attack:
1. Supply structurally complete `BrokerApiConfirmed` observation with an old non-default timestamp.
2. Supply a structurally complete reconciliation record and dimension evidence with old non-default timestamps.
3. Set connectivity available and submission state reconciled.
4. Current policy can return `Recovered` with `MayResumeRiskIncreasingAction=true`.

Missing controls:
- maximum evidence age;
- future timestamp rejection;
- coherence window between observation and reconciliation;
- per-dimension freshness relative to the recovery decision.

Risk:
A previously true broker state can be mistaken for current broker truth.

Required tests:
- stale observation rejected;
- stale reconciliation rejected;
- stale single dimension rejected;
- future timestamps rejected;
- mixed-age evidence rejected;
- fresh coherent evidence accepted.

### RT-H-02 — Safety envelope not canonically bound to risk decision and execution identity

Affected areas:
`Trading.Application/TradingServices.cs`
`Trading.Application/ExecutionQueue.cs`
`Trading.Domain/TradingDomain.cs`

Attack:
1. Create a valid-looking non-null safety envelope for a different position/instrument or older protection state.
2. Call `TradingDecisionPipeline.Prepare(...)` with a different risk request.
3. Current preparation checks envelope presence, not semantic binding.
4. Create/queue an `OrderIntent`; queue validation does not independently enforce envelope-to-order binding.

Missing controls:
- account identity binding;
- instrument identity binding;
- approved/requested quantity compatibility;
- trusted risk epoch binding;
- current protection owner/state/evidence binding.

Risk:
An unrelated safety proof can theoretically accompany risk-increasing work.

Required remediation:
one canonical `PositionSafetyEnvelopeBindingGuard` (name illustrative, not prescribed) consumed at preparation and immediately before dispatch eligibility.

### RT-H-03 — Guardian SafeMode can be relaxed without governed recovery

Affected areas:
`TradingGuardian.Domain/GuardianDomain.cs`
`TradingGuardian.Application/GuardianServices.cs`

Attack:
1. Put `CrisisStateMachine` into `SafeMode` via protection/integrity incident.
2. Submit a later lower-severity trusted signal/classification.
3. `Apply(...)` directly assigns `Warning` or `Restricted`.
4. No `BeginRecovery()` or recovery evidence is required.

Additional weakness:
`IncidentClassifier` ignores signal `ObservedAt` and does not enforce severity range.

Risk:
Protection can de-escalate based on stale or weaker observations instead of an explicit controlled-recovery path.

Required remediation:
- monotonic protective transitions during active incident;
- SafeMode exit only through governed recovery;
- recovery epoch/evidence;
- signal freshness and clock validation;
- severity bounds;
- tests proving stale/weak signals cannot relax protection.

## MEDIUM findings

### RT-M-01 — APP-RSC reclaimable resource claim can be internally inconsistent

`DemandIntegrityEvaluator` does not require `Consumption <= Allocation` and accepts a caller-supplied `Reclaimable` amount when bounded only by allocation-minus-minimum-safe. A claim may therefore be integrity/fresh flagged while actual consumption contradicts the implied spare capacity.

Remediation: calculate or constrain reclaimability against actual proven headroom and reject contradictory claims.

### RT-M-02 — Failure locality can rely on stale evidence

`FailureLocalityEvidence.ObservedAt` is carried but not used by locality applicability/containment policy. Stale locality evidence can theoretically narrow containment when stale evidence should fail conservative.

Remediation: time-bound locality evidence; clock-invalid/stale evidence must expand containment.

### RT-M-03 — Current README state drifts from canonical FCR registry

Current README/current-state summaries still present multiple closed FCRs as current obligations. Canonical issue bodies override them, so this is not authority creation, but it is a workstream-navigation hazard.

Remediation: update only current-state README sections. Do not rewrite historical closure artifacts.

## LOW finding

### RT-L-01 — Lower-level input/time helpers are not uniformly defensive

Examples:
- provider anomaly logic does not independently reject an observation timestamp in the future;
- raw simulator/calibration/domain helpers accept some impossible or weakly validated inputs that higher-level Digital City validation currently constrains.

Remediation: harden public/domain methods so each fails closed independently where inexpensive and deterministic.

## Strong controls confirmed

- Disabled broker/provider/protection/Foundation ports at current Host composition.
- No runtime authority granted by readiness results.
- Protection governed route rejects non-operational traffic, bad producer/recipient/authority/correlation/epoch/timing and ambiguous route outcome.
- Operational-data route rejects replay/test/simulation, incomplete provider route identity, bad time order and stale data as current truth.
- Execution queue fences leases, generations, containment intents and dispatch races.
- Foundation recovery binding is exact identity/digest/evidence/provenance bound and mutation-tested.
- FSTSimA qualification output never grants operational, runtime, Paper or Live authority.

## Release rule

No HIGH finding may be accepted as technical debt for a future broker/risk-increasing runtime launch without an explicit new Owner decision after risk review. Default recommendation is remediation before any runtime activation.
