# SYS-003 — Service Catalog

**Identifier:** SYS-003  
**Version:** Proposed 1.0  
**Status:** Proposed  
**Stage 1 Authority:** Not Granted

## Purpose

Provide the authoritative technical inventory of Foundation services, Applications, Suites, Guardians, and approved standbys without storing Application business meaning.

## Required Records

Identity, kind, version, owner, runtime identity, artifact/provenance, provided/consumed Contracts, routes, dependencies, technical criticality reference, resource profile, Health/Lifecycle/Security Contracts, authority reference, Guardian capabilities/request classes, standby/failover, isolation, recovery priority, status, admission/activation references, and evidence.

## Requirements

- registration SHALL NOT grant authority or activation;
- business purpose/data SHALL remain opaque or outside the Catalog;
- every update SHALL be authorized, versioned, attributable, integrity-protected, and challengeable;
- stale, conflicting, revoked, or unknown records SHALL not support permissive operation;
- removal SHALL reconcile dependents and preserve history;
- FSA, FFG, Health, Runtime, Lifecycle, Resources, Security, and Service Bus SHALL consume only their required technical view.

## Acceptance

Zero-Application Foundation, Application/Guardian registration, stale/conflict handling, standby identity, removal, payload exclusion, and complete reconstruction.

