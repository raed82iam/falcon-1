# Stage 6 WP-03 — Pre-Implementation Red-Team and Design

Result: PASS_WITH_HARDENING_REQUIREMENTS

## Chosen placement
Extend `Foundation.State` using accepted WP-01 primitives and accepted WP-02 `FoundationResourceTruthSnapshot`. Do not create a new production project or state owner.

## Mandatory invariants
1. `allocation <= quota <= ceiling` for every Application/resource class.
2. allocation, quota, ceiling and the matching WP-02 truth entry use the exact same canonical unit.
3. no allocation record may reference a resource class absent from WP-02 truth.
4. every allocation evidence reference is bound to the exact WP-02 resource epoch and cannot be future-dated relative to the allocation snapshot.
5. the allocation snapshot cannot predate the WP-02 truth snapshot it consumes.
6. duplicate `(ApplicationPrincipalId, ResourceClassId)` bindings fail closed.
7. duplicate ResourceGrantId values fail closed.
8. for each resource class, aggregate allocation, aggregate quota, and aggregate ceiling each must remain within WP-02 `AllocatableCapacity`.
9. Foundation protection floors and recovery reserves are never exposed as Application capacity.
10. Application-scoped views return only the exact requested Application's allocation records and expose no other Application allocation data.
11. zero Applications is valid.
12. identity is deterministic, immutable, and binds exact WP-02 resource-truth identity, epoch, observation, grants, quantities, lifetimes and evidence.

## Scope-leak checks
Production surface SHALL contain no Trading/TARC names and SHALL implement none of:
- priority/criticality selection;
- pressure/preemption;
- request/decision runtime;
- reclaim/rebalance/restore;
- load shedding;
- Application-internal distribution.

## Red-Team conclusion
No architectural blocker exists if all hardening requirements above are implemented and verifier-covered. WP-04 and later behavior remains unauthorized.
