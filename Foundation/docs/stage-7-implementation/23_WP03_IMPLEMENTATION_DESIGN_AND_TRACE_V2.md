# Stage 7 — WP-03 Foundation Self Model Runtime — Implementation Design and Trace V2

**Date:** 2026-08-12  
**Subject:** `WP-03 — Foundation Self Model Runtime`  
**Status:** `IMPLEMENTATION CANDIDATE / REVISED BEFORE EXECUTABLE VALIDATION`  
**Supersedes for implementation:** `21_WP03_IMPLEMENTATION_DESIGN_AND_TRACE.md`  
**Revision Basis:** fresh source-first review after the Foundation Workstream Rules synchronization at commit `81793e3b2a7d06506d0733f6eec1ab5ccd191dc0`  
**Stage 7 Plan:** `v0.3 OWNER_ACCEPTED`  
**Implementation Authority:** `GRANTED`  
**Predecessor:** `WP-02 TECHNICALLY_VALIDATED / OWNER CLOSURE DEFERRED`

## 1. Purpose

Preserve the bounded WP-03 design from V1 while closing two pre-executable projection-boundary gaps discovered before any WP-03 production code was written or built.

The WP-03 product remains a deterministic Foundation-only projection over attributable technical assertions and the already-governed SYS-008 Health assessment surface.

## 2. Governing Sources Re-read

The fresh review re-read the current:

- Falcon Vision;
- Falcon Constitution;
- AWR-001 v2.1;
- Stage 7 Implementation Plan v0.3;
- Foundation Workstream Rules;
- current WP-02 `CanonicalHealthAssessment` runtime surface;
- WP-03 V1 design and Red-Team.

The intervening Workstream Rules change adds the Shared Web workstream boundary and `Waiting On: WEB`. It does not alter AWR-001, SYS-008, CON-006, Stage 7 WP-03 semantics, or WP-02 runtime behavior.

## 3. Preserved WP-03 Boundaries

All V1 boundaries remain unchanged:

- `Foundation.SelfAwareness` is a projection, not an authoritative source owner;
- production references remain exactly `Foundation.Contracts` and `Foundation.HealthFitness`;
- no Application or Shared Web business semantics;
- no Technical Fitness computation before WP-04;
- no broad drift/challenge engine before WP-05;
- no concrete predecessor adapters before WP-06;
- no durable persistence/reconstruction before WP-07;
- no Authority/Lifecycle/protective enforcement before WP-08;
- no Stage 8 Guardian enforcement;
- no Stage 9 recovery execution/release;
- no Stage 13 FSA/Owner governance, Monitor AI, Kill, Factory Reset or Controlled Revival.

## 4. Revision R1 — Current Coverage Cannot Be Satisfied by Last-Known Alone

### V1 gap

V1 stated that every required Self Model area could satisfy coverage through either `CURRENT` or `LAST_KNOWN`.

That is too weak for AWR-001 failure/degraded behavior. AWR-001 requires Falcon to preserve the last trustworthy assessment with its age **and** mark the affected current state as unknown when awareness quality is insufficient.

Therefore an expired `LAST_KNOWN` fact cannot stand in for present knowledge.

### V2 rule

Every required WP-03 Self Model area SHALL have at least one `CURRENT` assertion.

If trustworthy current evidence is unavailable:

- the required `CURRENT` assertion SHALL be explicit `UNKNOWN`;
- it SHALL carry `EQ-INSUFFICIENT` or `EQ-INVALID` as applicable;
- uncertainty SHALL identify the missing/stale/unverifiable condition;
- any trustworthy prior observation MAY additionally remain as `LAST_KNOWN` with its original time and expiry visible.

`LAST_KNOWN` alone SHALL NOT satisfy minimum current-area coverage.

This preserves:

```text
LAST_KNOWN != CURRENT
MISSING_CURRENT_TRUTH -> CURRENT_UNKNOWN
HISTORY_PRESERVED != CURRENT_READINESS
```

## 5. Revision R2 — Health Projection Requires Structural Input Validation

### V1 gap

WP-03 consumes `CanonicalHealthAssessment`, but the current WP-02 surface exposes that type as a public immutable record and does not expose a separate public structural validator for arbitrary caller-constructed instances.

A caller could therefore construct a syntactically typed but structurally invalid Health assessment and submit it directly to the Self Model factory.

WP-03 must not recompute Health policy, but it also must not project malformed Health material as if it were an already-governed WP-02 result.

### V2 rule

Before `FromHealthAssessment(...)` creates a Self Model assertion, it SHALL perform bounded structural validation of the supplied `CanonicalHealthAssessment` using only already-defined WP-01/WP-02 representation invariants:

- all Health/Evidence/Consequence enums are defined;
- canonical assessment, subject, capability, evidence, rule and reduced-dependency identities are valid;
- confidence, contradiction, blind-spot and reason fields are non-empty;
- observation and assessment times are non-default;
- `ObservationTime <= AssessmentTime`.

This validation SHALL NOT:

- recompute SYS-008 Health;
- reinterpret evidence condition;
- change Health state;
- change evidence quality;
- create a new freshness window;
- infer Fitness or authority.

The exact WP-02 Health assessment identity and evidence reference remain bound into the projected assertion.

## 6. Shared Web Boundary Synchronization

The current Workstream Rules now explicitly identify `web-development` as a separate external workstream.

WP-03 remains Foundation-only. The executable verifier SHALL therefore challenge both ordinary Application/business leakage and Shared Web leakage in the public WP-03 production surface.

No Web-owned file or branch is modified by WP-03.

## 7. Required Coverage Set

The same 34 Foundation areas from V1 remain mandatory. No area is added or removed by this revision.

The only coverage change is semantic:

```text
V1: CURRENT OR LAST_KNOWN could satisfy required-area presence
V2: CURRENT is mandatory; LAST_KNOWN is supplemental history only
```

Technical Fitness and Pending Conformance remain representational `CURRENT UNKNOWN` in WP-03 until their governed producers exist.

## 8. Revised Executable Scenarios

The WP-03 verifier SHALL additionally prove:

1. a required area represented only by `LAST_KNOWN` fails closed;
2. `CURRENT UNKNOWN + LAST_KNOWN` for the same required area is preserved and succeeds as honest awareness;
3. a caller-constructed Health assessment with an undefined Health state is rejected before projection;
4. a caller-constructed Health assessment with impossible observation/assessment time order is rejected before projection;
5. a caller-constructed Health assessment with malformed canonical identity is rejected before projection;
6. the public WP-03 production surface contains no Shared Web ownership/business symbol.

All V1 executable scenarios remain required.

## 9. Expected Candidate Change Surface

Unchanged from V1:

- `src/Foundation.SelfAwareness/Foundation.SelfAwareness.csproj`;
- `src/Foundation.SelfAwareness/FoundationSelfModelRuntime.cs`;
- `verification/Falcon.Stage7.WP03.Verifier/Falcon.Stage7.WP03.Verifier.csproj`;
- `verification/Falcon.Stage7.WP03.Verifier/Program.cs`;
- `tests/Falcon.Foundation.Architecture.Tests/Program.cs`;
- `Falcon.Foundation.ControlledProjectFoundation.slnx`.

No WP-02 production file requires modification. Consumer-side structural validation is sufficient and avoids silently reopening predecessor semantics.

## 10. Stop Conditions

Stop rather than invent semantics if implementation requires a new Health policy, new authority meaning, new freshness threshold, predecessor semantic repair, Application/Web business interpretation, or a later-stage control-plane decision.

A genuine missing normative behavior remains:

`MISSING_NORMATIVE_DEFINITION`

and must return through the governed definition/review path.

## 11. Candidate Disposition

```text
WP03_V1_DESIGN = SUPERSEDED_FOR_IMPLEMENTATION_BY_V2
CURRENT_AREA_COVERAGE = REQUIRED
LAST_KNOWN_ALONE = INSUFFICIENT_FOR_CURRENT_COVERAGE
MISSING_CURRENT_TRUTH = EXPLICIT_CURRENT_UNKNOWN
HEALTH_PROJECTION_INPUT = STRUCTURALLY_VALIDATED
HEALTH_POLICY_REEVALUATION = FORBIDDEN
SHARED_WEB_OWNERSHIP_LEAKAGE = FORBIDDEN
SOURCE_TRUTH_OWNERSHIP_TRANSFER = FORBIDDEN
TECHNICAL_FITNESS_EVALUATION = DEFERRED_TO_WP04
STAGE8_STAGE9_STAGE13_AUTHORITY = NOT_CREATED
READY_FOR_FRESH_PRE_EXECUTABLE_RED_TEAM = YES
```
