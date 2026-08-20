# ADR-I012 — Foundation Plug-and-Play Application Model

**Identifier:** ADR-I012  
**Version:** Proposed 1.0  
**Status:** Proposed  
**Stage 1 Authority:** Not Granted

## Context

PLG-001 defines capability admission, but Falcon lacks a complete Application/Suite boundary, technical Manifest, Guardian declaration, and independent lifecycle.

## Decision

Every Application Suite SHALL be an independently identifiable, installable, registrable, startable, stoppable, suspendable, isolatable, upgradeable, replaceable, rollback-capable, and removable governed package.

Foundation SHALL integrate Applications only through Approved Manifests, Contracts, authority, FIL, Service Bus, Runtime, Lifecycle, Resources, Persistence, Security, Health, Catalog, evidence, and recovery boundaries.

Foundation SHALL NOT branch on Application business type or inspect business payload meaning.

## Admission

Admission SHALL validate package identity, Manifest, ownership, Guardian declaration, technical criticality, authority requests, Contracts/routes, resources/storage/runtime, security, Sandbox, required Digital City results, FSA conformance, Guardian readiness, registration, and controlled activation.

Installation, admission, activation, and business approval remain separate.

## Guardian Declaration

Every Suite SHALL declare Guardian policy as `REQUIRED`, `OPTIONAL`, or `NOT_APPLICABLE`.

A required Guardian that is missing or untrusted SHALL block full Suite activation.

## Alternatives Rejected

- direct Application coupling to Foundation internals;
- business-type switches inside Foundation;
- installation implying authority;
- Suite lifecycle that cannot be isolated or rolled back independently.

## Consequences

APP-001, APP-002, CON-023, CON-024, Service Catalog, Resource Governance, Runtime/Lifecycle interfaces, and admission evidence become prerequisites.

Acceptance of this ADR grants no implementation or activation authority.

