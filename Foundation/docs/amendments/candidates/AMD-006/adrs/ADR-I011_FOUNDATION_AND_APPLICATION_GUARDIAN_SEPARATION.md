# ADR-I011 — Foundation and Application Guardian Separation

**Identifier:** ADR-I011  
**Version:** Proposed 1.0  
**Status:** Accepted — Documentary Activation Deferred  
**Decision Record:** GOV-062  
**Date:** 2026-07-27  
**Decision Owner:** Falcon Project Owner  
**Related Decisions:** ADR-I010; GOV-060; GOV-061  
**Stage 1 Authority:** Not Granted

## Context and Problem

Falcon requires technical platform protection and domain-aware capital protection. A single Guardian would either force Foundation to understand trading or leave trading protection unable to reason about exposure, positions, orders, and execution safety.

Falcon must also allow future Accounting, Medical, Inventory, Transportation, or other Application Suites without redesigning Foundation.

## Decision

Falcon SHALL separate:

1. FFG inside Falcon Foundation.
2. Application Guardians outside Foundation.
3. Trading Guardian as the first specified Application Guardian and a mandatory independent dependency of the Trading Application Suite.

FFG owns Platform protective modes and cross-Application technical isolation.

Trading Guardian owns Trading-domain protective modes and restrictions within its Trading mandate.

Application Guardians MAY request investigation, containment, isolation, or Platform Safe Mode through CON-022. They SHALL NOT directly isolate another Application or activate a Platform mode.

## Scope

This decision governs Guardian placement, knowledge, authority, Safe Mode separation, cross-Application requests, request validation, release coordination, and AUT-002 migration.

## Non-Scope

It does not define trading strategy, risk limits, broker behavior, technical realization, deployment topology, production authority, or Stage 1 work.

## Knowledge Boundary

Trading Guardian MAY understand authorized Trading-domain meaning required for protection.

FFG receives only minimal technical claims and evidence references. It SHALL NOT receive business payload merely because a request originates from an Application Guardian.

## Protection Flow

```text
Trading-domain danger
        ↓
Trading Guardian restriction
        ↓ when shared technical protection is required
CON-022 technical request
        ↓
FFG independent validation
        ↓
FFG decision: reject, investigate, narrow, accept, or strengthen
        ↓
competent Foundation mechanism executes
        ↓
FSA and independent evidence verify technical recovery
        ↓
FFG releases Platform restriction
        ↓ separately
Trading Guardian releases Trading restriction
```

## Safe Mode Boundary

Platform modes and Trading modes are orthogonal.

- Platform return to normal does not establish Trading safety.
- Trading return to normal cannot override a Platform restriction.
- the effective permission is the intersection of applicable Platform, Trading, AUT-001, Security, Risk, and Lifecycle constraints.

## Alternatives

### One universal Guardian

Rejected: violates domain independence and knowledge minimization.

### Trading Guardian inside FSATA

Rejected: loss or compromise of FSATA could remove independent protection.

### Trading Guardian directly controls Foundation

Rejected: creates cross-Application authority outside its jurisdiction.

### FFG accepts requests without independent evidence

Rejected: permits an Application Guardian to suppress competitors or propagate compromise.

## Consequences

- a Trading Suite cannot activate without a trusted Trading Guardian;
- uninstalling Trading Suite may remove Trading Guardian without reducing Foundation completeness;
- FFG remains reusable for any Application domain;
- CON-022 and qualified Safe Mode names become mandatory;
- Trading and Platform release require separate evidence and authority;
- supporting manifests, catalogs, Contracts, and verification plans are required.

## Risks and Controls

| Risk | Control |
|---|---|
| abusive isolation request | identity, authority, evidence, rate limit, independent validation |
| conflicting Guardians | governed technical criticality; escalate unresolved business priority |
| Trading Guardian becomes execution engine | directives only; Broker Execution performs broker action |
| FFG receives business data | minimal schema and business-payload prohibition |
| provisional containment becomes permanent | expiry/review, evidence, escalation, explicit renewal |
| circular release | separate technical/domain verification and competent release authorities |

## Compatibility and Migration

AUT-002 v1.0, AUT-002 v2.0, ADR-I010, GOV-060, and GOV-061 remain preserved. AUT-002 v2.1 is a Proposed successor refinement. No supersession occurs until a separate Owner approval and activation change set.

## Approval Requirement

This ADR requires explicit Project Owner approval. Acceptance would not authorize implementation or activation.
