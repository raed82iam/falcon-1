# Stage 6 WP-02 — Pre-Implementation Scope and FCR Review

Status: AUTHORIZED / PRE-IMPLEMENTATION REVIEW
Date: 2026-08-08
Branch: foundation-development

## Owner authorization

The Owner explicitly instructed `ابدأ` after Stage 6 WP-01 was accepted and closed. This is recorded as prospective authorization for Stage 6 WP-02 only.

## Governing requirement

SYS-006 assigns Foundation singular ownership of total-resource truth, protection floors, recovery reserves and allocatable resource truth.

## Exact WP-02 boundary

WP-02 SHALL implement only:

1. exact resource-class total-capacity truth;
2. non-reclaimable Foundation survival/protection floor truth;
3. Foundation recovery-reserve truth;
4. deterministic allocatable-capacity derivation;
5. immutable deterministic Foundation resource-truth snapshots;
6. explicit evidence/epoch binding;
7. fail-closed validation for unavailable, duplicate, contradictory, unit-mismatched, negative or overcommitted truth.

WP-02 SHALL NOT implement:

- per-Application grants, quotas or ceilings;
- Trading or any other Application priority;
- pressure/preemption decisions;
- resource request/decision handling;
- reclamation/redistribution/restoration execution;
- Application load-shedding behavior;
- QoS scheduling;
- external egress/credentials;
- Application-specific business semantics.

## Architectural placement

The resource truth is authoritative Foundation state. It therefore belongs in the existing `Foundation.State` assembly and consumes the accepted Application-neutral resource primitives from `Foundation.Contracts.ResourceGovernance`.

No new permanent production project is required. This preserves the existing singular Foundation State owner and avoids a second state authority.

## FCR reconciliation

### FCR-0007

WP-02 is supporting only. It may provide total/allocatable/floor/reserve truth later consumed by the request/decision boundary, but it SHALL NOT implement requester authorization, TARC binding enforcement, requests, grants, caps or denials.

### FCR-0010

WP-02 is directly supporting because authoritative total-resource truth and protected reserves are prerequisites for later allocation/pressure/load-shedding projection. WP-02 SHALL NOT expose Application-specific pressure/load-shedding behavior.

### Other open FCRs

FCR-0004/0005/0006/0008/0009/0011/0012/0013/0014/0016 do not grant WP-02 implementation authority and SHALL NOT be absorbed into WP-02.

## Pre-implementation conclusion

`WP02_SCOPE = BOUNDED`
`WP02_ARCHITECTURAL_OWNER = Foundation.State`
`WP02_FCR_BLOCKER = NONE`
`WP03_PLUS_AUTHORITY = NONE`
