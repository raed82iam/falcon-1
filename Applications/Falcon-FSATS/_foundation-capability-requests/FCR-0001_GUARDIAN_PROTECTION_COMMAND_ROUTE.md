# FCR-0001 — Guardian Protection Command Route Requirements

**Status:** PROPOSED  
**Classification:** `PLANNED_CAPABILITY_INPUT`  
**Requester:** Application workstream  
**Application:** Falcon Trading Guardian Application / Falcon Self-Aware Trading Application  
**Date:** 2026-08-07  

## Requested Foundation capability

A generic governed Application-to-Application communication capability capable of carrying protection/restriction commands between independently governed Falcon Applications without granting direct internal access or Application-specific Foundation special casing.

## Application use case

The Falcon Trading Guardian Application must be able, under separately granted business authority, to communicate bounded protection outcomes to the Trading Application, such as no-new-order, reduce-exposure, restrict-market, halt-trading, or recovery-condition signals.

The route must preserve Application independence and must not make Guardian a Foundation controller or cross-Application superuser.

## Current Foundation evidence checked

- APP-001 — Application Boundary and Lifecycle.
- CON-023 — Falcon Application Contract and Manifest.
- ADR-I012 — Foundation Plug-and-Play Application Integration Boundary.
- ADR-I015 — Falcon OS Application and Awareness Alignment.
- current Stage 5 state: communication work beyond closed WP-01/WP-02 is planned but relevant runtime implementation is not yet authorized/available.

## Observed gap

`PLANNED_NOT_YET_AVAILABLE`.

This request does not claim that Foundation forgot the capability. It provides concrete FSATS requirements before the relevant Stage 5 communication/routing work is designed or implemented.

Required generic outcomes include:

- explicit producer/consumer identity;
- explicit authority binding;
- target/scope binding;
- expiry/time validity;
- correlation/causation;
- idempotency and duplicate-safe handling;
- attributable acceptance/rejection outcome;
- schema compatibility;
- evidence/provenance;
- fail-closed behavior for missing/invalid authority;
- no implication that route existence grants trading authority;
- support for safe rejection when target Application is unavailable, degraded, or incompatible.

## Application-side alternatives considered

1. Direct Guardian access to Trading internals — rejected as prohibited hidden coupling.
2. Shared database/state — rejected because it breaks Application independence and ownership.
3. Local Application-built substitute for Foundation routing — rejected because Application work cannot create Foundation communication semantics.

## Required outcome

Foundation should provide or preserve a generic governed communication boundary capable of expressing the requirements above. This FCR does not prescribe the Foundation implementation.

## Blocking impact

FSATS architecture and contract design can continue.

Future runtime Guardian→Trading protection-route implementation remains blocked until the required Foundation capability is authorized, implemented, and admitted for use.

## Authority rule

This FCR is a request/design input only. It grants no Foundation modification, implementation, deployment, runtime, trading, Paper, Tiny Live, Live, or production authority.
