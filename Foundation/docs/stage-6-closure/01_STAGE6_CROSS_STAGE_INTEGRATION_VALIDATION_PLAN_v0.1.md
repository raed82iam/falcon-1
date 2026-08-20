# Stage 6 — Cross-Stage Integration Validation Plan

Version: v0.1 PROPOSED
Status: PROPOSED / OWNER REVIEW REQUIRED
Date: 2026-08-11
Scope: Stage-level pre-closure validation only

## 1. Owner-directed objective

Before Stage 6 may be considered for final closure, Foundation shall prove that the accepted Stage 6 resource-governance implementation remains coherent with the complete accepted Foundation baseline that precedes it.

This gate is broader than WP-10.

WP-10 proved Stage 6 internal closure coherence across WP-01 through WP-09 and the WP-10 evidence package. This new gate proves that Stage 6 does not break, bypass, contradict, or silently reinterpret accepted Stage 0A through Stage 5 behavior and that the combined current Foundation baseline remains executable and fail-closed.

## 2. Authority and non-authority

The Project Owner has explicitly directed this Stage-level validation before Stage 6 closure.

This direction authorizes planning and execution of a bounded validation gate only after the exact plan is accepted.

It does NOT authorize:

- Stage 6 final closure;
- Stage 7 planning or implementation;
- production semantic changes;
- silent repair of any accepted predecessor Stage;
- Application modifications;
- deployment/runtime activation;
- external connectivity;
- financial/trading authority.

## 3. Closure preservation

The following remain closed while this gate runs:

- Stage 0A;
- Stage 0B;
- Stage 0C;
- Stage 1;
- Stage 2;
- Stage 3;
- Stage 4;
- Stage 5;
- Stage 6 WP-01 through WP-10.

The validation gate does not reopen them by existence.

A real detected defect must be classified and traced to an exact accepted scope before remediation authority exists.

## 4. Exact validation model

The gate shall use one exact detached candidate and one controlled validation environment.

The gate has four layers:

### Layer A — historical executable regression

Re-run all still-executable accepted historical verification surfaces that can be invoked against the current repository truth without fabricating historical conditions.

Required coverage:

1. Stage 0B verifier;
2. Stage 0C verifier;
3. Stage 0C remediation verifier with explicit `--evidence`, `--trace`, and `--root` arguments;
4. Baseline Integrity verifier;
5. Stage 2 WP-01 through WP-04 verifiers;
6. Stage 3 WP-01 through WP-06 verifiers;
7. Stage 4 WP-01 through WP-06 verifiers;
8. Stage 5 WP-01 through WP-10 verifiers;
9. Stage 6 WP-01 through WP-10 verifiers.

Stage 0A is a governed-preparation stage and has no current dedicated executable verifier in the controlled solution. Its role in this gate shall therefore be verified through immutable authority/evidence binding and the current repository governance checks, not by inventing a new historical Stage 0A runtime test.

Stage 1 has no dedicated `Falcon.Stage1.Verifier` project in current repository truth. Its executable role shall be covered through exact controlled-solution Restore/Release Build plus Foundation Architecture/Security and project-graph integrity rather than inventing a historical verifier that did not exist.

### Layer B — current Foundation-wide executable baseline

On the exact same candidate:

1. verify .NET SDK `10.0.302`;
2. Restore the controlled Foundation solution;
3. Release Build;
4. Foundation Architecture validation;
5. Foundation Security validation;
6. exact HEAD and clean-tree checks before and after execution.

Historical Stage 0 verifier projects that are not members of the current controlled solution shall be restored/built explicitly and only for this validation gate. Their absence from current controlled-solution membership shall not be silently changed merely to make the test easier.

### Layer C — dedicated cross-stage integration verifier

A new verification-only project is proposed:

`verification/Falcon.Stage6.CrossStageIntegration.Verifier/`

It shall contain no production implementation and shall not create a new Foundation capability.

Its responsibility is to prove current cross-stage coherence across the accepted Foundation baseline, including at minimum:

1. **Identity continuity** — Stage 6 resource identities, request identities, decision identities, evidence identities and Application identities remain compatible with accepted canonical identity rules.
2. **Canonical encoding / schema continuity** — Stage 6 contracts and evidence remain deterministic and compatible with accepted Stage 0/2 encoding, schema and contract rules.
3. **Authority continuity** — Stage 6 cannot create authority, grant authority through silence, bypass delegation, or execute a resource mutation without the accepted authority boundary.
4. **Lifecycle continuity** — Stage 6 allocation, reduction, revocation, reclamation, rebalance and restoration cannot bypass accepted lifecycle restrictions, suspension, termination or invalid-state boundaries.
5. **State continuity** — Stage 6 state transitions must preserve accepted Foundation state truth and reject impossible/conflicting transitions.
6. **Evidence continuity** — Stage 6 decisions and mutations remain attributable, deterministic, challengeable and compatible with accepted evidence/reconciliation semantics.
7. **Dependency-governance continuity** — Stage 6 must not bypass accepted dependency availability, exact identity, delegation-chain or activation-order requirements.
8. **Communication continuity** — Stage 6 Application-facing resource state/load-shedding signals must remain compatible with the accepted Stage 5 FIL/service-bus/event/message admission/routing/delivery model and must not create a second communication authority.
9. **Replay / duplicate continuity** — replay, duplicate, stale or conflicting resource requests/signals must not create duplicate authority or mutate current truth incorrectly.
10. **Security continuity** — Stage 6 cannot weaken message protection, opaque key/reference handling, fail-closed validation or security boundaries established earlier.
11. **Application-neutrality continuity** — zero Applications remains valid; no Application becomes a Foundation prerequisite; multiple Application identities remain isolated.
12. **Resource/isolation continuity** — one Application's pressure, grants or mutations cannot leak resource authority or mutable state into another Application.
13. **Protected-floor / reserve continuity** — Stage 6 pressure/rebalance behavior cannot silently violate protected Foundation floors or recovery reserves.
14. **No business-semantic leakage** — Stage 6 remains technical Foundation resource governance and does not acquire trading, market, portfolio, provider, broker, strategy or financial business meaning.
15. **No future-stage pullback** — Stage 6 cannot depend on Stage 7+ capabilities or create Stage 7+ authority by implication.
16. **Determinism** — the exact same cross-stage inputs on the same Release outputs produce the same integrated result identity.
17. **Mutation sensitivity** — controlled negative fixtures altering cross-stage identities, authority, lifecycle, state, evidence, dependency, communication or resource boundaries must fail closed for the exact expected reason.

### Layer D — exact integrated rerun

After all predecessor/current verifiers pass, the dedicated cross-stage verifier shall run twice from the same Release outputs.

The verifier DLL identity shall be hashed before run 1 and after run 2 and must remain identical.

## 5. Proposed executable order

1. exact candidate/remote identity preflight;
2. detached checkout;
3. pre-validation clean-tree check;
4. exact SDK verification;
5. controlled-solution Restore;
6. controlled-solution Release Build;
7. explicitly restore/build historical Stage 0B, Stage 0C and Stage 0C remediation verifier projects if they are not already covered by the controlled solution;
8. Stage 0B verifier;
9. Stage 0C verifier;
10. Stage 0C remediation verifier with generated evidence and trace paths;
11. Baseline Integrity verifier;
12. Foundation Architecture;
13. Foundation Security;
14. Stage 2 WP-01..WP-04;
15. Stage 3 WP-01..WP-06;
16. Stage 4 WP-01..WP-06;
17. Stage 5 WP-01..WP-10;
18. Stage 6 WP-01..WP-10;
19. dedicated Cross-Stage Integration verifier run 1;
20. dedicated Cross-Stage Integration verifier run 2 from the same Release outputs;
21. cross-stage verifier DLL hash equality;
22. final exact HEAD;
23. final clean working tree;
24. refreshed remote candidate unchanged;
25. transcript SHA-256.

## 6. Evidence package

The final validation package shall contain at minimum:

- exact candidate SHA;
- exact branch;
- SDK identity;
- full ordered gate result;
- Stage 0B evidence artifact;
- Stage 0C evidence artifact;
- Stage 0C remediation evidence artifact;
- Stage 0C remediation trace artifact;
- all verifier exit codes and summaries;
- dedicated cross-stage verifier run-1/run-2 summaries;
- dedicated verifier DLL SHA-256 before/after;
- exact HEAD/clean-tree/remote confirmation;
- complete transcript SHA-256;
- failure classification if any gate fails.

## 7. Failure classification

Every failure must be classified before remediation:

- `CROSS_STAGE_VERIFIER_OR_HARNESS_DEFECT`
- `HISTORICAL_VERIFIER_CURRENT_COMPATIBILITY_DEFECT`
- `STAGE6_SUCCESSOR_COMPATIBILITY_DEFECT`
- `PREDECESSOR_ACCEPTED_SCOPE_DEFECT_REQUIRES_EXPLICIT_TRACE`
- `STAGE6_ACCEPTED_SCOPE_DEFECT_REQUIRES_EXPLICIT_TRACE`
- `AUTHORITY_OR_GOVERNANCE_CONFLICT`
- `UNRESOLVED_FCR_OR_DOCUMENTARY_BLOCKER`

No automatic repair is permitted.

A verifier/harness-only defect may be remediated only inside the validation package under the exact granted test authority.

A true defect inside a closed Stage/WP requires exact defect evidence and separate governed remediation authority before modifying the accepted scope.

## 8. Red-Team requirement

Before an executable candidate is frozen, the test design and verifier implementation shall receive a fresh static Red-Team.

After executable PASS, a fresh post-executable Red-Team/reconciliation shall challenge at minimum:

- false PASS caused by merely rerunning independent verifiers without proving cross-stage interaction;
- hidden predecessor bypass;
- authority/lifecycle/state/evidence contradictions;
- Stage 5 communication versus Stage 6 signal incompatibility;
- resource isolation leakage;
- zero-Application regression;
- Stage 7+ authority leakage;
- historical closure reinterpretation;
- test harness mutation of the tested outputs;
- stale/moving branch evidence.

## 9. Stage 6 closure rule

A PASS from this gate does not itself close Stage 6.

Required sequence:

`CROSS_STAGE_PLAN_ACCEPTED`
-> `VERIFIER_IMPLEMENTED`
-> `STATIC_RED_TEAM_PASS`
-> `EXACT_EXECUTABLE_VALIDATION_PASS`
-> `POST_EXECUTABLE_RED_TEAM_PASS`
-> `FINAL_STAGE6_CLOSURE_READINESS_REPORT`
-> `SEPARATE_OWNER_STAGE6_CLOSURE_DECISION`

## 10. Current authority state

`STAGE6_WP10 = ACCEPTED_AND_CLOSED`

`STAGE6 = OPEN_PENDING_CROSS_STAGE_INTEGRATION_VALIDATION`

`CROSS_STAGE_VALIDATION_PLAN = PROPOSED_v0.1`

`CROSS_STAGE_VALIDATION_IMPLEMENTATION = NOT_YET`

`STAGE7_PLANNING_AUTHORITY = NOT_GRANTED`

`STAGE7_IMPLEMENTATION_AUTHORITY = NOT_GRANTED`
