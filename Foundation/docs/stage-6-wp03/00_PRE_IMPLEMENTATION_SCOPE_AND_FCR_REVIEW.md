# Stage 6 WP-03 — Pre-Implementation Scope and FCR Review

Status: AUTHORIZED_FOR_IMPLEMENTATION

## Purpose
Implement the generic Foundation-owned per-Application allocation/quota/ceiling/isolation state boundary over the accepted WP-02 total-resource truth.

## In scope
- ApplicationPrincipalId-bound resource allocation state.
- ResourceGrantId-bound grant identity.
- Exact resource class and unit binding.
- Current allocation, quota, and ceiling.
- Required invariant: allocation <= quota <= ceiling.
- Exact WP-02 resource-truth snapshot identity and epoch binding.
- Aggregate fail-closed protection against consumption beyond WP-02 AllocatableCapacity.
- Application-scoped view returning only that Application's allocations.
- Deterministic immutable snapshot identity and evidence attribution.
- Zero-Application validity.

## FCR reconciliation
FCR-0010 is directly relevant only to the allocation/ceiling/isolation prerequisite in WP-03. WP-03 does not claim the pressure/enforcement/load-shedding/request/restoration portions of FCR-0010.

FCR-0007 resource request/decision runtime remains outside WP-03.

## Out of scope
- cross-Application priority/technical criticality policy (WP-04);
- pressure/preemption/enforcement state (WP-05);
- resource request/decision boundary (WP-06);
- reclamation/redistribution/rebalance/restoration (WP-07);
- Application load-shedding signal boundary (WP-08);
- integration/hardening and Stage 6 closure (WP-09/WP-10);
- Trading/TARC-specific production semantics;
- Application-internal distribution or business decisions.

## Architectural placement
The existing `Foundation.State` remains the singular resource-state owner. WP-03 extends it rather than creating a second state owner or a new production project.

## Authority
Owner authorization record: `docs/canonical-records/owner-decisions/stage6/Stage6-WP03-Implementation-Authorization-20260809-010700/OWNER-AUTHORIZATION-STAGE6-WP03-IMPLEMENTATION.txt`.
