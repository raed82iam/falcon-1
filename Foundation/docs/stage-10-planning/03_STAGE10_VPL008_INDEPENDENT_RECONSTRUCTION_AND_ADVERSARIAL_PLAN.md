# Stage 10 — VPL-008 Independent Reconstruction and Adversarial Plan

**Status:** IMPLEMENTATION_READY_VERIFICATION_PLAN  
**Scope:** FRS-SCN-008 only  
**Production Runtime Authority:** NONE

## Objective

Prove that FRS-001 scenarios VPL-001 through VPL-007 can be reconstructed from governed evidence on the current Foundation candidate without undocumented knowledge and without converting missing evidence into success.

## Reconstruction Set

The Stage 10 verifier shall execute and bind these current proof surfaces:

1. Trusted Bootstrap — Stage 3 WP-06 integrated bootstrap/admission/dependency/lifecycle verifier, with prerequisite chain present.
2. Unauthorized Action — Stage 4 WP-01 verifier.
3. Invalid Lifecycle Transition — Stage 4 WP-02 verifier.
4. Invalid FIL Message — Stage 5 WP-10 integrated verifier.
5. Health Evidence Loss — Stage 7 WP-10 integrated verifier.
6. Guardian Restriction — Stage 8 WP-10 integrated verifier.
7. Controlled Recovery — Stage 9 WP-10 integrated verifier.

The verifier shall bind the outputs in canonical VPL order and calculate a deterministic reconstruction identity over the exact scenario IDs, verifier identities, exit results and required semantic markers.

## Required Positive Result

PASS requires all seven reconstructed predecessor scenarios to pass on the current candidate and all expected semantic markers to be present.

A previous historical PASS cannot compensate for a current executable failure.

## Mandatory Adversarial Variants

The reconstruction package shall be copied in memory and challenged with at least:

1. mutation of one material scenario result;
2. deletion of one required scenario;
3. insertion of an unknown scenario;
4. reordering of scenarios;
5. duplication of a scenario;
6. missing required semantic marker;
7. correction appended to history;
8. attempted correction by rewriting the original record.

Required outcomes:

```text
MATERIAL_MUTATION = DETECTED
REQUIRED_DELETION = DETECTED
UNKNOWN_INSERTION = DETECTED
REORDERING = DETECTED
DUPLICATION = DETECTED
REQUIRED_MARKER_LOSS = DETECTED
APPENDED_CORRECTION = ACCEPTABLE_WITH_LINEAGE
HISTORY_REWRITE = REJECTED
```

## Independent-Reconstruction Boundary

The Stage 10 executable verifier is a verification actor only. It shall not:

- alter production state;
- grant, restore or revoke runtime authority;
- perform Lifecycle transitions;
- release Guardian restrictions;
- execute Recovery;
- write accepted evidence history;
- infer business or financial semantics;
- connect externally;
- depend on any Application.

## FRS-001 Release Invariants

Stage 10 shall verify, from the reconstructed chain and governing sources, that:

- material action remains attributable to authority;
- unknown identity/baseline cannot become unrestricted startup;
- unknown required Fitness cannot become positive authority;
- material authority/transitions remain reconstructable;
- Guardian restriction is independent of the subject;
- Recovery cannot approve its own completion;
- no financial consequence path is introduced by the demonstration;
- implementation cannot silently redefine the approved Specification.

## Non-Financial Boundary

The Stage 10 verifier must remain valid with zero Applications and must reject any controlled-solution dependency on `applications/**` or Trading/Web business assemblies.

Stage 10 PASS is not:

- deployment approval;
- production readiness;
- external-connectivity approval;
- broker/market-data approval;
- trading readiness;
- financial authority;
- Stage 11 or later authorization.

## Execution Gate

The verifier must run after Release build and Architecture/Security tests. Its technical PASS is evidence for Stage 10 review but cannot self-approve the Foundation Release. The final Release Authority decision remains separate.
