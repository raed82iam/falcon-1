# Stage 6 WP-06 — Existing Capability Reconciliation

**Status:** DRAFT / PLANNING INPUT ONLY  
**Stage:** 6 — Foundation Resource Governance and Operational Pressure Control  
**Work Package:** WP-06 — Additional Resource Request and Decision Boundary  
**Implementation Authority:** NOT GRANTED  
**Owner Acceptance:** NOT YET  
**Date:** 2026-08-10

## 1. Purpose

This record reconciles the accepted Stage 6 WP-01 through WP-05 baseline and currently applicable FCR inputs before drafting WP-06 planning.

It does not reopen any accepted closure and does not authorize implementation.

## 2. Preserved predecessor closures

The following are treated as accepted and closed exactly within their authorized scopes:

- Stage 6 WP-01 — Canonical Resource Governance Primitives
- Stage 6 WP-02 — Foundation Resource Truth, Protection Floors and Recovery Reserves
- Stage 6 WP-03 — Application Allocation, Quota, Ceiling and Isolation
- Stage 6 WP-04 — Cross-Application Priority and Technical Criticality Governance
- Stage 6 WP-05 — Resource Pressure, Preemption Eligibility and Enforcement-State Truth

`PRESERVE_ACCEPTED_CLOSURES = TRUE`

`CLOSURE_DEFECT_REQUIRES_EXPLICIT_TRACE = TRUE`

No unmet WP-06 requirement is evidence of a predecessor closure defect.

## 3. Existing reusable primitives

WP-01 already established canonical primitives that WP-06 shall reuse rather than duplicate, including:

- `ResourceRequestId`
- `ResourceDecisionId`
- `ResourceGrantId`
- `ResourceDecisionKind`
- `ResourceQuantity`
- `ResourceClassId`
- `ApplicationPrincipalId`
- `CorrelationId`
- `CausationId`
- `ResourceEpochId`
- `ResourceEvidenceReference`
- deterministic canonical identity support

The accepted decision enumeration already includes:

- `Grant`
- `PartialGrant`
- `Cap`
- `Deny`
- `Defer`
- `Revoke`
- `Reduce`
- `Restore`

WP-06 shall not create competing identities or duplicate decision semantics.

## 4. Accepted predecessor truth available to WP-06

WP-06 may consume, but shall not reinterpret or mutate, accepted predecessor truth from:

### WP-02
- Foundation total-resource truth
- protection floors
- recovery reserves
- allocatable-capacity truth

### WP-03
- exact Application identity
- exact grant identity
- allocation
- quota
- ceiling
- isolation and exact per-Application attribution

### WP-04
- Foundation-governed Application priority classes
- technical criticality classes
- exact governed bindings

Priority and technical criticality remain distinct and do not mint business authority.

### WP-05
- exact resource pressure truth
- exact Application-scoped pressure truth
- preemption eligibility for consideration only
- enforcement-state observation only

Pressure, eligibility and observed enforcement do not themselves authorize a new grant, mutation, reclamation or redistribution.

## 5. Applicable FCR inputs

### FCR-0007
The historical TARC-specific requester concept is prospectively superseded by the FSARM-compatible requester model.

For future WP-06 planning:

- FSARM is the prospective delegated aggregate resource coordinator/requester.
- the exact coordinator identity and coordination scope must be governed.
- constituent Applications remain exact, independently attributable and isolated.
- FSARM may not self-mint Foundation grant or ceiling authority.
- `INTERNAL_REDISTRIBUTION_FIRST` remains mandatory.
- `FOUNDATION_ADDITIONAL_REQUEST_SECOND` remains mandatory.
- a Foundation request is for proven residual need that cannot be safely satisfied inside the valid coordination envelope, or where a Foundation-authoritative grant/ceiling mutation is required.
- `REQUESTED_RESOURCE != GRANTED_RESOURCE` remains mandatory.

### FCR-0031
WP-06 must be compatible with a future governed FSARM coordination envelope, but WP-06 shall not implement internal redistribution mechanics owned by later WP-07.

WP-06 therefore needs enough coordinator/requester identity and scope evidence to make aggregate requests attributable and bounded without converting FSARM into an opaque Application principal or Foundation principal.

### FCR-0010
WP-06 contributes the request/decision portion of the broader resource-state/request/redistribution/load-shedding capability family. WP-07 and WP-08 remain separately gated.

## 6. Existing capability conclusion

The Foundation already owns the identities, resource truth, allocation truth, priority/criticality truth and pressure truth required to evaluate additional-resource requests.

The missing WP-06 capability is the governed runtime boundary that:

1. accepts a bounded attributable additional-resource request;
2. proves the requester and represented constituent scope;
3. binds the request to current accepted Foundation resource/allocation/pressure truth;
4. proves residual unmet need rather than caller preference alone;
5. produces one exact attributable Foundation resource decision;
6. preserves request/decision correlation, causation, expiry, evidence and deterministic identity;
7. fails closed on stale, split-brain, mismatched, forged or unauthorized request state;
8. does not execute WP-07 reclamation/redistribution/rebalance/restoration mechanics.

## 7. Reconciliation result

`WP06_EXISTING_CAPABILITY_RECONCILIATION = COMPLETE_FOR_PLANNING`

`WP06_IMPLEMENTATION_AUTHORITY = NONE`

`WP01_WP05_CLOSURES_REOPENED = NO`

`WP07_WP08_AUTHORITY_CREATED = NO`
