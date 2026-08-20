# FCR-0002 — Market Data Delivery Requirements

**Status:** PROPOSED  
**Classification:** `PLANNED_CAPABILITY_INPUT`  
**Requester:** Application workstream  
**Application:** FSAPMA / Falcon Self-Aware Trading Application  
**Date:** 2026-08-07  

## Requested Foundation capability

A generic governed cross-Application delivery capability suitable for operational market-data messages and data-service responses between independently governed Falcon Applications.

## Application use case

FSAPMA owns provider-facing trading data management and must deliver provider-independent normalized data to authorized consuming Applications such as the Trading Application, while preserving freshness, quality, provenance, isolation, and fail-closed behavior.

## Current Foundation evidence checked

- APP-001.
- CON-023.
- ADR-I012.
- ADR-I015.
- current Stage 5 communication status where later routing/delivery implementation remains not yet authorized/available.

## Observed gap

`PLANNED_NOT_YET_AVAILABLE`.

This FCR supplies Application requirements for planned Foundation communication work and does not claim a missing Foundation design commitment.

Required generic outcomes include:

- producer/consumer identity;
- schema/version compatibility;
- message correlation and causation where applicable;
- payload integrity identity;
- freshness/timestamp support;
- provenance/lineage attachment;
- quality/degraded-status metadata carriage without Foundation interpreting trading meaning;
- duplicate-safe/idempotent handling;
- explicit delivery/rejection outcome;
- consumer isolation;
- back-pressure / flow-control behavior appropriate for high-rate operational data;
- safe degradation when delivery truth or freshness cannot be established;
- replay/test traffic separation from authoritative operational delivery.

## Application-side alternatives considered

1. Direct provider connections from Trading — rejected because it bypasses FSAPMA ownership.
2. Shared database/message files between Applications — rejected as hidden coupling.
3. FSAPMA-owned replacement Service Bus — rejected as Foundation ownership leakage.

## Required outcome

Foundation should provide or preserve a generic governed communication/delivery boundary able to satisfy these Application requirements without becoming trading-specific.

## Blocking impact

Provider, market, normalization, quality, and contract design can continue.

Future runtime FSAPMA→Trading operational data delivery remains blocked until the required Foundation capability is authorized, implemented, and admitted.

## Authority rule

This FCR is a request/design input only and grants no Foundation modification, implementation, external connectivity, market-data activation, deployment, or trading authority.
