# Stage 5 WP-06 — FCR Pre-Validation Disposition

**Status:** PRE_VALIDATION_FCR_REVIEW_COMPLETE / REMEDIATED_PENDING_RUNTIME_VALIDATION  
**Authority:** `Stage5-WP06-Implementation-Authorization-20260808-003200`  
**Branch:** `foundation-development`

## Purpose

This record performs the required feature-by-feature Foundation disposition of all currently open Application Foundation Capability Requests before Stage 5 WP-06 validation.

An FCR is a request/problem input only. It does not create implementation authority, prescribe Foundation internals, or expand the authorized WP-06 scope.

## Open FCR inventory reviewed

- FCR-0004 — governed protection command route
- FCR-0005 — operational market-data delivery contract
- FCR-0006 — event evidence and replay delivery
- FCR-0007 — Foundation resource escalation request boundary
- FCR-0008 — research-only Internet egress boundary
- FCR-0009 — latency deadline and QoS-aware transport
- FCR-0010 — resource pressure and load-shedding signals
- FCR-0011 — non-Live isolation and egress guard

FCR-0001 is the shared operating protocol; FCR-0002 and FCR-0003 are closed documentary/control records.

## FCR-0004

`FOUNDATION_PROBLEM = VALID`  
`WP06_RELEVANCE = DIRECT_PARTIAL`

WP-06-owned facets: delivery guarantee, expiry, idempotency-aware retry, fail-closed retry/dead-letter, truthful dispatch/ack/outcome separation, governed protective/revocation traffic treatment, deterministic evidence, and correlation/causation preservation during transport.

Guardian-specific semantics, successful protection/business effect, and replay/test truth remain outside WP-06.

**Pre-validation remediation:** implemented. WP-06 now consumes the exact canonical FIL envelope, verifies its SHA-256 against the accepted WP-04 admission digest, and preserves correlation/causation identities in immutable delivery decisions and outcomes. Runtime validation remains pending.

## FCR-0005

`FOUNDATION_PROBLEM = VALID`  
`WP06_RELEVANCE = DIRECT_PARTIAL`

WP-06-owned facets: generic delivery semantics, freshness through accepted expiry, idempotency-aware duplicate protection, degradation/health-aware handling, delivery outcome evidence, authority preservation, and bounded failure behavior.

Schema/version compatibility is predecessor-owned by WP-02/WP-04. Market-data quality/confidence business meaning remains opaque to Foundation. Replay-safe event truth remains later-owned.

No Application-specific market-data special case is introduced.

## FCR-0006

`FOUNDATION_PROBLEM = VALID`  
`WP06_RELEVANCE = PARTIAL`

WP-06 owns scoped ordering, idempotency-aware duplicate protection for attempts, retry/dead-letter behavior, acknowledgement/outcome evidence, and correlation/causation transport preservation.

Event truth ownership, immutable event publication, replay truth, correction semantics, evidence journal retention, and replay-to-live authority rules remain WP-07 or other separately authorized owners.

**Pre-validation remediation:** implemented for correlation/causation transport preservation only. WP-06 does not absorb event/replay truth semantics. Runtime validation remains pending.

## FCR-0007

`FOUNDATION_PROBLEM = VALID`  
`WP06_RELEVANCE = INDIRECT_OUT_OF_SCOPE`  
`WP06_DISPOSITION = DEFER`

WP-06 may later transport an already-governed resource request message, but it SHALL NOT invent the resource request/allocation decision contract. Request, approval, cap, denial, rebalance, duration, and restoration conditions remain SYS-006 resource-governance work.

## FCR-0008

`FOUNDATION_PROBLEM = VALID`  
`WP06_RELEVANCE = OUT_OF_SCOPE`  
`WP06_DISPOSITION = DEFER`

Research-only Internet egress, destination policy, operational-data separation, revocation, and egress security enforcement are not Service Bus delivery semantics.

## FCR-0009

`FOUNDATION_PROBLEM = VALID`  
`WP06_RELEVANCE = DIRECT_PARTIAL`

WP-06-owned facets: preserve accepted expiry/deadline without resetting it, governed technical traffic class, deterministic route/producer pressure gating, bounded overload behavior, elevated-capacity reservation, cross-Application pressure isolation, and delivery outcome timing evidence.

Aggregate/tail-latency observability and percentile reporting remain later observability work. Application-specific Fast Track policy is prohibited.

**Pre-validation remediation:** pressure ceilings/reserves are now bound to an explicit governed pressure authority/result and exact Application/route identity. Runtime validation remains pending.

## FCR-0010

`FOUNDATION_PROBLEM = VALID`  
`WP06_RELEVANCE = DIRECT_FOR_PRESSURE_CONSUMPTION_PARTIAL_OVERALL`

WP-06 owns only delivery-side consumption and enforcement of governed pressure state needed for flow control. It does not own the complete Application-facing resource telemetry/request API.

Current WP-06 implementation now requires:

- exact producer Application and WP-05 route-decision binding;
- explicit `AuthorityResult`-backed pressure authority;
- exact authorized global/route/producer ceilings and elevated reserve;
- exact effective scope `service-bus-pressure-truth`;
- fail-closed handling for malformed, DENY, future, expired or mismatched pressure authority;
- explicit pressure observation time;
- restoration/rebalance conditions and evidence;
- deterministic identity binding of pressure state and pressure authority.

General allocation telemetry, resource-request submission/outcome, redistribution and the broader SYS-006 resource engine remain outside WP-06.

**Pre-validation remediation:** implemented; runtime validation remains pending.

## FCR-0011

`FOUNDATION_PROBLEM = VALID`  
`WP06_RELEVANCE = OUT_OF_SCOPE`  
`WP06_DISPOSITION = DEFER`

Non-Live credential/route acquisition denial, Live-authoritative egress isolation, and replay/test versus Live access enforcement belong to authority/security/Plug-and-Play/egress owners, not WP-06 delivery semantics.

## Current gate

```text
STAGE5_WP06_IMPLEMENTATION = AUTHORIZED_AND_IN_PROGRESS
WP06_FCR_PRE_VALIDATION_REVIEW = COMPLETE
RT_08 = REMEDIATED_PENDING_RUNTIME_VALIDATION
RT_09 = REMEDIATED_PENDING_RUNTIME_VALIDATION
WP06_FCR_REMEDIATION = COMPLETE_FOR_PRE_VALIDATION_SCOPE
WP06_FOCUSED_VALIDATION = READY_TO_RUN
STAGE5_WP07_THROUGH_WP10 = UNAUTHORIZED
```

No FCR is closed by this disposition. Final WP-06 FCR reconciliation must record verified completed portions versus remaining/deferred portions in each relevant GitHub Issue after successful technical validation and before Owner closure.
