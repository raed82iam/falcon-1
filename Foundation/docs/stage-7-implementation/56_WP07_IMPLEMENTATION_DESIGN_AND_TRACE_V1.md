# Stage 7 WP-07 — Implementation Design and Trace v1

## Scope

WP-07 implements the Stage 7-owned Health/Fitness Events, Persistence and Reconstruction integration required by the accepted Stage 7 v0.3 plan.

It reuses accepted predecessor substrates instead of creating duplicate engines:

- `Foundation.EventSystem` for event truth classification and event relation semantics;
- `Foundation.State` for durable authoritative-state record identity/history substrate;
- `Foundation.HealthFitness` for canonical health/fitness assessments;
- `Foundation.SelfAwareness` only as an identity/basis source where required by Stage 7 history reconstruction.

## Architecture choice

A new Stage7-owned adapter project is introduced:

`src/Foundation.HealthHistory/Foundation.HealthHistory.csproj`

Reason: directly adding EventSystem/State references to `Foundation.HealthFitness` would turn the core assessment project into a persistence/event hub and would violate the WP-06 architecture guard that intentionally keeps accepted predecessor integration outside the HealthFitness core.

The adapter may depend on accepted predecessor projects. Predecessor projects are not modified.

## Required behavior

The adapter shall:

1. materialize health-state-change and fitness-change facts from canonical assessments;
2. preserve event type, schema, version, owner and provenance identity;
3. preserve `EventTruthClassification` exactly;
4. require replay events to remain explicitly replay-classified and related to the original event;
5. require corrections to be new events using `CorrectionOf`, never in-place history mutation;
6. materialize deterministic `AuthoritativeStateRecord` snapshots for Stage7-owned assessment/history evidence;
7. reconstruct the exact assessment/self-model/fitness basis only when state-record digest and payload identity bindings verify;
8. reject corrupted or identity-mutated history;
9. expose logging/persistence loss as evidence-quality loss rather than optimistic reconstruction;
10. keep history/assessment authority separate from operational permission or recovery authority.

## Explicit exclusions

WP-07 does not:

- create an event bus or event publication engine;
- create a logging engine;
- create a persistence engine;
- modify Stage 4/5 state/event semantics;
- implement Authority consumption (WP-08);
- implement Guardian/Safe-State enforcement (Stage 8);
- implement recovery execution or independent release (Stage 9);
- implement FSA/Owner evolution governance (Stage 13);
- add Application business semantics.

## Ownership

```text
HEALTH/FITNESS ASSESSMENT = Foundation.HealthFitness
WP07 HISTORY ADAPTER = Foundation.HealthHistory
EVENT TRUTH CLASSIFICATION/RELATION = Foundation.EventSystem
DURABLE RECORD SHAPE/DIGEST = Foundation.State
AUTHORITY = NOT OWNED BY WP07
RECOVERY = NOT OWNED BY WP07
```

## Verification

WP-07 verifier shall cover positive, negative, mutation, replay, correction, persistence-corruption, reconstruction and deterministic identity scenarios, plus regressions for WP-01 through WP-06 and Architecture/Security in the Owner-local executable test.
