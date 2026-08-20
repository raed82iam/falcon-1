# FCR-0003 — Event, Evidence, and Replay Delivery Requirements

**Status:** PROPOSED  
**Classification:** `PLANNED_CAPABILITY_INPUT`  
**Requester:** Application workstream  
**Application:** Trading Guardian / FSAPMA / Falcon Self-Aware Trading Application  
**Date:** 2026-08-07  

## Requested Foundation capability

A generic governed event/evidence delivery capability that supports reconstructable Application-to-Application event flows, replay-safe consumption, and clear separation between authoritative operational traffic and replay/test traffic.

## Application use case

FSATS requires attributable reconstruction of provider degradation, Guardian decisions, trading-state changes, execution outcomes, and other cross-Application evidence without allowing replayed historical messages to create unintended live business effects.

## Current Foundation evidence checked

- APP-001.
- CON-023.
- ADR-I012.
- ADR-I015.
- current Stage 5 communication state where later event/journal/delivery semantics remain planned but not yet authorized/available for runtime use.

## Observed gap

`PLANNED_NOT_YET_AVAILABLE`.

This FCR is early Application design input for planned Foundation communication/evidence work.

Required generic outcomes include:

- immutable event identity;
- producer and consumer identity;
- correlation and causation lineage;
- schema/version identity;
- provenance/evidence identity;
- attributable delivery/rejection result;
- deterministic replay-safe message identity;
- explicit operational vs replay/test context;
- protection against replay traffic triggering unintended authoritative actions;
- ordering/duplication semantics sufficient for independent reconstruction;
- retention/reconstruction references where governed by Foundation;
- failure visibility when evidence or delivery certainty is insufficient;
- Foundation payload opacity preserved for business semantics.

## Application-side alternatives considered

1. Application-specific cross-Application event journal — rejected because it could duplicate Foundation communication/evidence semantics.
2. Reusing live commands during replay without context separation — rejected as unsafe.
3. Shared event database between Applications — rejected as hidden coupling.

## Required outcome

Foundation should provide or preserve generic event/evidence communication semantics that allow Applications to reconstruct and safely replay governed histories without interpreting domain payload meaning or granting business authority through replay.

## Blocking impact

V1.4 evidence, reconciliation, and replay design can continue.

Future runtime cross-Application event/replay integration remains blocked until the relevant Foundation capability is authorized, implemented, and admitted.

## Authority rule

This FCR is a request/design input only. It grants no Foundation modification, runtime activation, replay-to-live authority, deployment, or trading authority.
