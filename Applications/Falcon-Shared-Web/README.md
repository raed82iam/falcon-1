# Shared Falcon Web Application

**Branch:** `web-development`  
**Status:** `OWNER-AUTHORIZED WORKSTREAM / IMPLEMENTATION AUTHORIZED / GOVERNED RUNTIME BINDING AND DEPLOYMENT SUBJECT TO THEIR OWN AUTHORITY`  
**Implementation authorization:** Project Owner, 2026-08-15: `ابدأ implementation كامل.`  
**Canonical writable subtree:** `applications/shared/web/**`

## Purpose

This subtree is reserved for the Shared Falcon Web Application: a reusable Falcon Shared Application that provides common Web/UI capabilities to Falcon Applications through governed contracts/routes.

It is not Falcon Foundation, not FSATS, and not an owner of domain-specific business logic.

## Ownership boundary

The Shared Web workstream MAY write only within:

`applications/shared/web/**`

unless the Project Owner explicitly grants a broader change.

The Shared Web workstream SHALL treat as read-only:

- `foundation-development` and Foundation-owned files;
- `application-development` ordinary Application/FSATS-owned files;
- `main`;
- `reference/fsats-v1.3-scratch`;
- canonical Vision, Constitution, Specifications, ADRs, Contracts, Standards, governance, evidence, and other authority artifacts outside its authorized subtree.

Cross-workstream needs are handled through the shared FCR protocol. FCR participation never grants cross-workstream file-write authority.

## Required governing inputs

Before architecture, design, implementation, review, or repository changes, read current repository evidence including:

- Falcon Vision;
- Falcon Constitution;
- `applications/README.md`;
- current effective `APP-001`;
- current effective `CON-023`;
- current effective `ADR-I012`;
- current effective `ADR-I015`;
- applicable Foundation contracts and current Foundation authority;
- applicable Application/Shared Application ownership decisions;
- current FCR state;
- `applications/shared/web/WORKSTREAM_RULES.md`.

## Shared versus domain-specific rule

```text
Generic + intentionally reusable across Falcon
→ Shared Falcon Web Application

Primarily domain-specific
→ owning Falcon Application
```

Shared Web may provide reusable UI infrastructure, shell, navigation, layouts, design-system components, accessibility, responsive behavior, generic visualizations, and other reusable presentation capabilities when approved.

Trading-specific semantics, strategy/risk/provider/broker logic, domain decisions, and other business truth remain owned by their respective Applications.

## Current implementation gate

Shared Web implementation is now explicitly Owner-authorized inside this subtree.

This authorization does **not** by itself:

- authorize Shared Web to modify another workstream;
- make Shared Web the owner of Foundation or Application business truth;
- authorize production deployment or external connectivity that remains separately governed;
- permit Web to fabricate missing runtime contracts, data, authority, broker/provider truth, or execution outcomes.

Development/demo fixtures, when used to build and verify presentation, must be visibly identified as non-live and must never silently substitute for authoritative runtime state.

See `docs/IMPLEMENTATION_PLAN.md` for the active implementation sequence.

## Maintainability and Foundation integration rule

Shared Web is implemented as a replaceable Falcon Shared Application, not as a UI coupled to Foundation internals.

Cross-workstream integration follows:

```text
UI / Feature Presentation
        ↓
Web-owned presentation/state policy
        ↓
Web-owned ports/contracts
        ↓
Governed adapters
        ↓
Foundation / owning Application public contracts
```

Direct UI dependencies on Foundation internals, FSATS internals, broker internals or provider internals are prohibited.

The active implementation architecture is documented in `docs/IMPLEMENTATION_ARCHITECTURE.md`.
The best-in-class selection rule is documented in `docs/IMPLEMENTATION_BEST_IN_CLASS_RULE.md`.

Technology selection must seek the strongest proven fit for Falcon after research and comparison. Popularity, novelty, convenience or implementation speed alone are not sufficient reasons to adopt a technology.

## Prime rule

**Source first. Authority second. Compare third. Decide fourth. Change last.**
