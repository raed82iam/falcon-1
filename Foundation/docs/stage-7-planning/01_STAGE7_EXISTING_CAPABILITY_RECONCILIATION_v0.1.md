# Stage 7 — Existing Capability Reconciliation

Version: v0.1
Status: DRAFT / PLANNING EVIDENCE / NOT OWNER ACCEPTED
Date: 2026-08-11

## 1. Authority and scope

Stage 6 is `ACCEPTED_AND_CLOSED`.

Stage 7 planning/design entry is separately authorized by:

`docs/canonical-records/owner-decisions/stage7/Stage7-Planning-Design-Authorization-20260811/OWNER-AUTHORIZATION-STAGE7-PLANNING-DESIGN.md`

Stage 7 implementation authority remains `NOT_GRANTED`.

IMP-001 v1.3 defines Stage 7 as:

`Foundation Health, Self-Awareness and Technical Fitness`

and requires `EXISTING_CAPABILITY_RECONCILIATION` as the mandatory first gate.

This document performs that first planning gate. It does not authorize code or activate any planned Specification.

## 2. Governing sources freshly reconciled

The current source-first reconciliation includes:

- Falcon Vision;
- Falcon Constitution;
- `docs/plans/IMP-001_FOUNDATION_IMPLEMENTATION_WORK_PLAN.md`;
- `docs/specifications/SPEC-000_REGISTRY.md`;
- `docs/04_SPECIFICATION_TREE.md`;
- `docs/specifications/core/AWR-001_SELF_AWARENESS_SYSTEM.md`;
- `docs/specifications/core/SYS-008_HEALTH_MONITORING.md`;
- `docs/specifications/core/SYS-002_LIFECYCLE.md`;
- `docs/specifications/core/AUT-001_AUTHORITY_ENGINE.md`;
- `docs/specifications/foundation/SYS-006_MULTI_LEVEL_RESOURCE_GOVERNANCE.md`;
- `docs/specifications/core/SYS-010_EVENT_SYSTEM.md`;
- `docs/specifications/core/OPS-004_LOGGING.md`;
- `docs/specifications/core/SYS-011_PERSISTENCE.md`;
- `docs/adrs/ADR-I015_FALCON_OS_APPLICATION_AND_AWARENESS_ALIGNMENT.md`;
- current `Falcon.Foundation.ControlledProjectFoundation.slnx`;
- current FCR headers relevant to the Stage transition.

## 3. Existing effective capability/specification baseline

### 3.1 AWR-001 — Foundation Self-Awareness System

Classification: `EXISTS / EFFECTIVE SPECIFICATION / IMPLEMENTATION COMPLETENESS TO BE PROVEN`.

AWR-001 v2.1 is current and effective. It already defines the Foundation Self Model and requires awareness of:

- Foundation identity and admitted baseline;
- component identity/version/lifecycle/integrity;
- runtime and infrastructure condition;
- Service Bus and FIL condition;
- dependency availability/compatibility/criticality;
- resource capacity, pressure and exhaustion risk;
- persistence/backup/restore/corruption condition;
- documentation/configuration integrity;
- security and authority condition;
- faults, contradictions and blind spots;
- isolation/recovery readiness;
- active restrictions;
- Foundation Technical Fitness;
- evidence identity/provenance/freshness/confidence/uncertainty;
- historical versions and supersession.

AWR-001 already defines technical fitness states:

- `FIT`;
- `FIT_WITH_CONSTRAINTS`;
- `DEGRADED`;
- `UNKNOWN`;
- `UNAVAILABLE`;
- `INTEGRITY_FAILURE`;
- `ISOLATION_REQUIRED`;
- `RECOVERY_REQUIRED`;
- `NOT_FIT`.

It explicitly states that awareness does not grant authority and does not replace Health Monitoring, Guardian, Authority Engine, Security, Recovery or Lifecycle.

### 3.2 SYS-008 — Health Monitoring

Classification: `EXISTS / EFFECTIVE SPECIFICATION / IMPLEMENTATION COMPLETENESS TO BE PROVEN`.

SYS-008 already defines health observation/assessment semantics, including:

- subject identity;
- observations and signals;
- freshness and confidence;
- dependency/aggregate health;
- blind spots/unknown state;
- health-state transitions;
- event publication;
- degradation detection.

Canonical health states already exist:

- `HEALTHY`;
- `DEGRADED`;
- `UNHEALTHY`;
- `UNKNOWN`;
- `NOT_APPLICABLE`.

Missing or stale evidence may not become `HEALTHY`.

### 3.3 Accepted predecessor capabilities Stage 7 must consume, not duplicate

Classification: `EXISTS / ACCEPTED PREDECESSOR BASELINE`.

- `SYS-002 Lifecycle`: authoritative lifecycle state and governed transitions.
- `AUT-001 Authority Engine`: deny-by-default authorization and restriction-sensitive decisions.
- Stage 3 dependency governance: dependency identity, compatibility, availability and activation truth.
- Stage 4 state/evidence/reconciliation: authoritative state and evidence boundaries.
- Stage 5 communication/event system: governed publication/delivery/replay semantics.
- Stage 6 resource governance: resource truth, pressure, protection floors, recovery reserves, per-Application isolation and load-shedding state.
- `OPS-004 Logging`: attributable operational evidence and logging-failure visibility.
- `SYS-011 Persistence`: durable governed state, integrity, history and recovery-aware persistence.

Stage 7 shall consume those truths. It shall not establish a parallel lifecycle, authority engine, event bus, persistence owner or resource governor.

## 4. Planned-but-not-effective Specification subjects

The current registry/tree contains the following Self-Awareness subjects as proposed new Specifications with no current effective body:

- `AWR-002 — Fitness to Operate`;
- `AWR-003 — Confidence and Uncertainty`;
- `AWR-004 — Temporal Awareness`;
- `AWR-005 — Drift and Blind-Spot Detection`.

The canonical path for AWR-002 is registered, but the file is not currently present at that path.

These planned subjects do not create requirements or implementation authority by registration alone.

Mandatory rule:

If Stage 7 implementation requires behavior that depends on a planned subject lacking an effective body, that subject must pass `SPECIFICATION_DEFINITION_REVIEW_ACTIVATION_GATE` before implementation of that missing behavior.

However, AWR-001 already contains substantial fitness, confidence/freshness, temporal and drift/blind-spot obligations. Therefore Stage 7 shall not automatically create or activate AWR-002..AWR-005 merely because they exist in the tree. The planning phase must first determine whether AWR-001/SYS-008 already govern the required behavior adequately.

## 5. Awareness hierarchy boundary

ADR-I015 remains controlling:

- FSA belongs to Foundation;
- MSA belongs to exactly one Application;
- LSA belongs to one major Application branch;
- CSA is optional and belongs to one eligible intelligent Application component;
- awareness rank does not create authority or cross-owner access.

Therefore:

`AWR-006 / AWR-007 / AWR-008 = EFFECTIVE BUT APPLICATION-AWARENESS SPECIFICATIONS, NOT STAGE7 FOUNDATION IMPLEMENTATION TARGETS`.

Stage 7 may define Foundation-side compatibility/technical evidence boundaries required by FSA, but it shall not implement Application MSA/LSA/CSA internals.

## 6. Future-Stage boundary reconciliation

### Stage 8 boundary

Stage 8 owns Foundation Guardian, protective restriction and platform Safe State completion.

Stage 7 may detect and publish health/fitness conditions and expose evidence to Guardian/Authority/Lifecycle. Stage 7 shall not absorb Guardian command authority, Safe-State enforcement or independent release semantics.

### Stage 9 boundary

Stage 9 owns controlled recovery, independent recovery validation, controlled reintroduction and release authority.

Stage 7 may report `RECOVERY_REQUIRED` and provide evidence. It shall not declare recovery complete or release a recovered subject.

### Stage 11 boundary

Stage 11 owns transport QoS, deadline governance and observability for that later scope.

Stage 7 shall not pull broad future QoS/transport observability work backward. It may consume existing logs/events/health evidence required by current effective Stage 7 specifications.

### Stage 13 boundary

FCR-0012 and FCR-0030 are explicitly assigned to Stage 13 for FSA/Owner governance, FSA integrity containment/monitoring, MSA-to-FSA governed interface and bounded evolution control-plane work.

Stage 7 shall not pull Stage 13 Owner/FSA governance, Monitor-AI, FSA Kill/Factory Reset, direct-Internet policy, or self-development promotion controls backward.

## 7. Current implementation-surface observation

The current controlled Foundation solution contains accepted Stage 0 through Stage 6 production projects and verifiers, but no dedicated project currently named as a Stage 7 Health, Foundation Self-Awareness or Technical Fitness implementation/verifier surface.

This establishes that no dedicated Stage 7 project has yet been admitted to the controlled solution.

It does NOT by itself prove that no health/awareness-related helper type exists inside an older project. A code-level ownership/type census remains required before implementation planning can claim a specific implementation gap.

No Stage 7 production source shall be added until that census and the plan review are complete.

## 8. Canonical separation of responsibilities for Stage 7 planning

The planning model shall preserve:

```text
OBSERVATIONS / SOURCE TRUTH
        ↓
SYS-008 HEALTH MONITORING
        ↓
QUALIFIED HEALTH ASSESSMENTS
        ↓
AWR-001 FOUNDATION SELF MODEL + TECHNICAL FITNESS
        ↓
FITNESS / UNCERTAINTY / CONSTRAINT EVIDENCE
        ↓
AUT-001 AUTHORITY DECISION
        ↓
OPTIONAL GOVERNED LIFECYCLE / GUARDIAN CONSUMPTION
```

Key invariants:

- Health is not authority.
- Fitness is not authority.
- FSA is not Guardian.
- FSA is not Lifecycle.
- FSA is not Recovery Authority.
- Events do not grant permission.
- Logs are evidence, not authoritative state merely because they exist.
- Resource pressure truth remains Stage 6-owned.
- Application business/domain meaning remains outside Foundation Stage 7.

## 9. Preliminary capability disposition

| Capability family | Reconciliation disposition |
|---|---|
| Health semantics/model | EXISTS in SYS-008; implementation completeness to prove |
| Foundation Self Model | EXISTS in AWR-001; implementation completeness to prove |
| Foundation Technical Fitness semantics | EXISTS in AWR-001; implementation completeness to prove |
| Health event publication | REQUIRED by SYS-008 and can consume Stage 5; implementation completeness to prove |
| Lifecycle awareness | consume existing SYS-002 truth; do not duplicate |
| Authority-aware fitness use | consume AUT-001; fitness must not grant permission |
| Dependency awareness | consume accepted Stage 3 truth |
| Resource pressure awareness | consume accepted Stage 6 truth |
| Evidence/history/reconstruction | consume accepted Stage 4 + OPS-004 + SYS-011; Stage 7 projection/history work to define |
| Confidence/uncertainty | materially present in AWR-001; separate AWR-003 activation not automatic |
| Temporal awareness/freshness | materially present in AWR-001/SYS-008; separate AWR-004 activation not automatic |
| Drift/blind spots | materially present in AWR-001; separate AWR-005 activation not automatic |
| Guardian/Safe State enforcement | Stage 8, out of Stage 7 implementation scope |
| Recovery release | Stage 9, out of Stage 7 implementation scope |
| FSA/Owner control plane and Monitor AI | Stage 13, out of Stage 7 implementation scope |
| MSA/LSA/CSA internals | Application-owned, out of Foundation Stage 7 scope |

## 10. Preliminary gap statement

The current documentary baseline is not missing the core Stage 7 meaning. The principal planning problem is to determine the exact implementation and integration gap between the already-effective `SYS-008` + `AWR-001` requirements and the accepted Stage 0..6 runtime capabilities.

Therefore Stage 7 should be an implementation/integration completion stage around existing effective Health and Foundation Awareness semantics, not a redesign of self-awareness.

## 11. Required next planning work

Before any implementation authorization request, Foundation must:

1. complete a code-level current implementation census for SYS-008/AWR-001 obligations;
2. map every AWR-001 and SYS-008 requirement to `EXISTS / PARTIAL / MISSING / FUTURE-STAGE`;
3. map required predecessor interfaces from Stages 3, 4, 5 and 6;
4. determine whether any AWR-002..AWR-005 subject truly requires an effective specification body;
5. identify exact contracts/events/state identities required for Stage 7;
6. produce a bounded Work Package decomposition;
7. produce verification and negative-test requirements for every Work Package;
8. perform Architecture/Consistency review;
9. perform fresh Red-Team review;
10. present the resulting Stage 7 plan to the Project Owner before implementation.

## 12. Current disposition

`STAGE6 = ACCEPTED_AND_CLOSED`

`STAGE7_PLANNING_AND_DESIGN = AUTHORIZED`

`STAGE7_EXISTING_CAPABILITY_RECONCILIATION = STARTED / INITIAL_DOCUMENTARY_RECONCILIATION_COMPLETE`

`STAGE7_CODE_LEVEL_IMPLEMENTATION_CENSUS = REQUIRED`

`STAGE7_PLAN = NOT_YET_OWNER_ACCEPTED`

`STAGE7_IMPLEMENTATION_AUTHORITY = NOT_GRANTED`

`STAGE8_AUTHORITY = NOT_GRANTED`