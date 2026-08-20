# Stage 7 WP-09 — VPL-005 Executable Health-Evidence-Loss Validation and Hardening

Version: V1
Date: 2026-08-14
Status: IMPLEMENTATION DESIGN / AUTHORIZED UNDER STAGE7 v0.3

## 1. Purpose

WP-09 executes the Stage 7-owned portion of active `VPL-005 v1.1` against the already implemented Stage 7 runtime chain. It is an integration/hardening work package, not a new subsystem.

## 2. Reuse decision

No duplicate production engine is required.

The existing chain already owns the required semantics:

1. `HealthObservationAssessmentRuntime` — governed observation to canonical Health assessment.
2. `HealthEvidenceQualityRuntime` — exact VPL-005 loss classification and effective evidence-quality reduction.
3. `FoundationSelfModelAssertionFactory` + `FoundationSelfModelProjector` — Health consequence into Foundation Self Model projection.
4. `TechnicalFitnessEvaluationRuntime` — scoped technical fitness and exact CON-006 projection.
5. `HealthFitnessGovernedConsumptionRuntime` — bounded AUT-001/Lifecycle/protective-consumer input evidence with no authority grant.
6. `HealthFitnessHistoryRuntime` — governed change fact, history record and reconstructability.
7. `EvidenceAwarenessRuntime` — LastKnown eligibility/expiry, independent challenge and restoration gate.

WP-09 therefore introduces an executable verifier that composes these accepted APIs end-to-end. Production source is changed only if executable evidence exposes a genuine Stage 7 defect.

## 3. VPL-005 loss matrix

The verifier must execute all nine active loss classes:

- MISSING
- STALE
- DELAYED
- CONTRADICTORY
- UNVERIFIABLE
- INACCESSIBLE
- CORRUPTED
- PROVENANCE_FAILURE
- PARTIAL_VISIBILITY

For every required-evidence loss class the verifier shall prove:

- the relation remains explicitly loss-classified;
- effective evidence quality is not `Sufficient`;
- Health cannot remain positively healthy for the affected scope;
- Self Model carries explicit uncertainty/evidence loss;
- technical fitness is reduced from the fresh baseline;
- CON-006 cannot remain `FIT`;
- governed Authority consumption blocks positive authority inference or requires restriction/gating;
- a governed material fitness-change fact can be produced;
- the fact/history basis is attributable and reconstructable.

## 4. Restoration and LastKnown

The verifier shall separately prove:

- eligible LastKnown truth remains tagged and time-bounded;
- expired LastKnown truth is unusable;
- stale cached success cannot substitute for current evidence;
- source reappearance alone remains pending independent reassessment;
- a fresh independent challenge/reassessment can satisfy the Stage 7 restoration gate;
- satisfying the restoration gate restores only admissibility of the fitness input;
- prior authority restriction/denial still requires a new AUT-001 authority decision.

## 5. Isolation and zero-Application behavior

An unaffected technical capability with independent evidence shall remain independently assessable and shall not inherit the affected capability's loss merely by coexisting in Falcon.

The complete executable scenario remains Foundation-only and valid with zero Applications. No Trading, market, broker, portfolio, strategy, Web, MSA, LSA or CSA business semantics are introduced.

## 6. Future-stage boundaries

WP-09 does not execute or claim:

- Guardian command/enforcement or Platform Safe State, Stage 8;
- recovery execution, recovery acceptance, independent release or Controlled Revival, Stage 9;
- FSA/Owner governance or evolution control, Stage 13.

`HEALTH != AUTHORITY`
`FITNESS != AUTHORITY`
`SOURCE_REAPPEARANCE != AUTHORITY_RESTORATION`
`INDEPENDENT_REASSESSMENT != AUTHORITY_GRANT`

## 7. Verification disposition

WP-09 reaches technical PASS only after:

- controlled Release build;
- Foundation Architecture PASS;
- Foundation Security PASS;
- WP-01..WP-08 regressions PASS;
- WP-09 positive, all-nine-loss, fail-closed, restoration, isolation, history/reconstruction and mutation checks PASS;
- deterministic rerun from identical Release outputs;
- stable material executable hashes;
- exact final HEAD and clean worktree.
