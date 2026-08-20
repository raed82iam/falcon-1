# Stage 7 WP-06 — Accepted Predecessor Truth Integration

**Design and Trace:** V1  
**Date:** 2026-08-14  
**Status:** `PRE_EXECUTABLE_DESIGN_CANDIDATE / IMPLEMENTATION_READY`  
**Foundation Branch:** `foundation-development`  
**Starting Accepted Closure State:** Gate 0A through WP-05 = `ACCEPTED_AND_CLOSED`  
**WP-06 Owner Closure:** `NOT_YET`

## 1. Purpose

Implement the bounded Stage 7 integration layer that qualifies accepted predecessor truth for Health/Self-Awareness/Fitness consumption without duplicating, mutating, repairing, or taking ownership of Stage 3 through Stage 6, Security/Trust, Evidence, Logging, Persistence, Authority, Lifecycle, Event, or Resource truth.

WP-06 closes the source-authenticity and accepted-predecessor integration obligation intentionally deferred by WP-05.

## 2. Controlling Sources

- Falcon Vision and Constitution;
- accepted Stage 7 Implementation Plan v0.3;
- Gate 0A reuse/ownership census;
- accepted Stage 7 WP-01 through WP-05 runtime and evidence;
- accepted Stage 3 dependency/configuration truth;
- accepted Stage 4 Authority/Lifecycle/state/evidence/reconciliation truth;
- accepted Stage 5 contracts/message/event/protection truth;
- accepted Stage 6 resource truth/pressure/isolation/load-shedding truth;
- accepted Security/Trust identity truth;
- accepted Logging/Persistence evidence.

No later Stage capability or Application business semantic is activated by this WP.

## 3. Exact WP-06 Domains

The canonical integration coverage domains are:

1. `STAGE3_DEPENDENCY_CONFIGURATION`;
2. `STAGE4_AUTHORITY_LIFECYCLE_STATE`;
3. `STAGE4_EVIDENCE_RECONCILIATION`;
4. `STAGE5_CONTRACT_MESSAGE_EVENT_PROTECTION`;
5. `STAGE6_RESOURCE_PRESSURE_ISOLATION_LOAD_SHEDDING`;
6. `SECURITY_TRUST_IDENTITY`;
7. `LOGGING_PERSISTENCE`.

Every required domain must be explicitly represented. Omission is not success.

## 4. Ownership Model

WP-06 consumes an immutable normalized attestation envelope supplied from the authoritative predecessor owner/adaptor. The envelope never becomes the source of predecessor truth. It only records enough exact identity, ownership, schema/version, provenance, integrity, temporal and operational-classification evidence to decide whether a predecessor fact may support current Stage 7 awareness.

Invariants:

- `PROJECTION != SOURCE_TRUTH`;
- `INTEGRATION_RESULT != AUTHORITY`;
- `REPLAY != CURRENT_TRUTH`;
- `HISTORICAL != CURRENT_TRUTH`;
- `SOURCE_REAPPEARANCE != AUTHORITY_RESTORATION`;
- `UNAVAILABLE_TRUTH -> NO_POSITIVE_CURRENT_AWARENESS`;
- predecessor owner identity remains exact and unchanged;
- WP-06 contains no predecessor mutation or repair API.

## 5. Minimal Source Change Surface

Create:

- `src/Foundation.HealthFitness/PredecessorTruthIntegrationRuntime.cs`;
- `verification/Falcon.Stage7.WP06.Verifier/`;
- `tests/Falcon.Foundation.Architecture.Tests/Stage7Wp06ArchitectureGuard.cs`.

Update only controlled solution membership to include the WP-06 verifier.

`Foundation.HealthFitness` retains its existing project-reference boundary. WP-06 SHALL NOT add a dependency mesh from HealthFitness directly to closed predecessor projects.

## 6. Canonical Source Definition

Every predecessor source definition binds:

- definition identity;
- exact WP-06 domain;
- source ID;
- source owner;
- truth kind;
- contract/schema ID;
- contract/schema version;
- governing authority identity.

All identifiers are canonical and all enums must be defined.

## 7. Canonical Evidence Envelope

Every evidence envelope binds:

- evidence ID/reference;
- exact domain;
- source ID and source owner;
- truth kind;
- contract/schema ID and version;
- subject/capability/scope;
- source record identity/version;
- payload digest;
- provenance reference and status;
- integrity reference and status;
- source-authenticity status;
- availability status;
- operational classification;
- observation/effective/assessment/expiry times;
- reason.

No source-code freshness threshold is invented. Currentness is based on the evidence-bound governed expiry supplied with the predecessor evidence.

## 8. Positive Current-Awareness Eligibility

Evidence may support positive current awareness only when all are true:

- definition/envelope domain, source, owner, truth kind, schema and version match exactly;
- availability = available;
- source authenticity = verified;
- integrity = verified;
- provenance = verified;
- classification = authoritative current;
- observation/effective/assessment times are valid and non-future;
- expiry is after assessment time;
- required canonical identities/digests/references are present.

The resulting quality is `Sufficient` only for this exact qualified integration relation. It does not upgrade any weaker WP-02/WP-05 evidence quality.

## 9. Fail-Closed Mapping

- missing/unavailable/inaccessible source -> `Insufficient`, no current support;
- stale/expired source -> `Insufficient`, no current support;
- authoritative historical/replay/test/simulation/non-authoritative source -> `Insufficient`, no current support;
- unverified authenticity -> `Insufficient`, no current support;
- authenticity mismatch -> `Invalid`;
- integrity unverified -> `Insufficient`;
- corrupted integrity -> `Invalid`;
- provenance unverified -> `Insufficient`;
- provenance failure -> `Invalid`;
- definition/source/owner/schema/version mismatch -> validation failure;
- malformed/future/impossible temporal evidence -> validation failure.

No failure path fabricates a predecessor repair.

## 10. WP-05 Binding

WP-06 provides an explicit validation bridge to a WP-05 `HealthEvidenceRelationAssessment`.

For a positive `AVAILABLE` WP-05 relation, the corresponding WP-06 result must be current-awareness eligible and exact source/source-owner/evidence-reference binding must match. A replayed, stale, unavailable, unauthenticated, corrupted, provenance-failed, historical, test or non-authoritative predecessor result cannot be represented to WP-05 as positively available current evidence.

This resolves the prior `SOURCE_AUTHENTICITY = PENDING_WP06` boundary without rewriting WP-05 Health truth.

## 11. Coverage Aggregation

WP-06 exposes deterministic coverage evaluation over all seven required domains.

- exactly one qualified result per required domain is required for complete current coverage;
- omission produces incomplete/insufficient coverage;
- duplicate domain results fail closed;
- any invalid result makes aggregate quality invalid;
- any unavailable/stale/replay/non-current result prevents complete current coverage;
- aggregate identity is independent of input ordering through canonical domain ordering.

## 12. Architecture Boundaries

WP-06 SHALL NOT:

- add direct references from `Foundation.HealthFitness` to predecessor implementation projects;
- modify Stage 3..6 source truth;
- implement predecessor repair;
- issue Authority decisions;
- command Guardian;
- transition Lifecycle;
- execute Recovery;
- create Event/Persistence engines;
- interpret Application business meaning;
- implement Stage 8/9/11/12/13/14 behavior.

## 13. Verification Requirements

The verifier shall cover at minimum:

- one valid current source for each of all seven domains;
- deterministic aggregate independent of input order;
- source owner/source/schema/version mismatch rejection;
- malformed identity/enum/time rejection;
- future-dated evidence rejection;
- stale expiry rejection;
- replay/historical/test/simulation/non-authoritative current-use rejection;
- unavailable/inaccessible truth reduction;
- unverified and mismatched authenticity;
- unverified/corrupted integrity;
- unverified/failed provenance;
- duplicate and missing domain coverage;
- WP-05 exact relation binding;
- no optimistic WP-05 `AVAILABLE` binding from non-current predecessor truth;
- mutation sensitivity;
- architecture guard proving no predecessor dependency mesh or forbidden later-stage surface.

WP-01 through WP-05 regressions, Architecture and Security must remain PASS.

## 14. Pre-Executable Conclusion

`TRUE_PREDECESSOR_DEFECT_FOUND = NO`

`PREDECESSOR_REOPEN_REQUIRED = NO`

`WP06_SCOPE_EXPANSION = NO`

`WP06_READY_FOR_PRE_EXECUTABLE_RED_TEAM = YES`

No technical PASS or this design document closes WP-06. Owner closure remains separate after executable evidence and post-executable review.