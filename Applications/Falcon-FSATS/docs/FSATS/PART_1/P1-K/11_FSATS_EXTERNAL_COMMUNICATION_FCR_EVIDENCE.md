# FSATS Part 1 — External Communication FCR Evidence

**Status:** `FCR EVIDENCE / DESIGN GAP IDENTIFIED / NOT_OWNER_ACCEPTED / NOT_CLOSED`  
**Branch:** `application-development`  
**Implementation Authority:** `NOT GRANTED`  
**Runtime Authority:** `NOT GRANTED`

## Purpose

Record the Application-side evidence for a cross-workstream FCR covering the exact communication behavior between FSATS Applications, Shared Applications such as Shared Web, and Falcon Foundation.

## Current Established Rules

- FSATS is a non-owning system boundary and is not a runtime principal.
- The actual sender/receiver is always an independently admitted Falcon Application or a governed Foundation endpoint.
- Direct access to another Application's internals is prohibited.
- Cross-Application communication uses declared governed contracts and admitted routes.
- Foundation owns inter-Application communication rules while Application business semantics remain Application-owned.
- FIL / Service Bus transport does not create authority.
- Route existence does not create authority.

## Identified Partial Gap

The current Part 1/P1-K material establishes semantic families and boundaries, but does not yet provide one shared, exact cross-workstream contract model for all of these interactions:

1. FSATS Application -> Shared Application information publication/projection.
2. Shared Application -> FSATS Application information query/request and response.
3. Shared Application -> FSATS Application authority-bearing user request/command request and governed outcome.
4. FSATS Application -> Foundation information/evidence submission.
5. FSATS Application -> Foundation capability/state/authority query and response.
6. Foundation -> FSATS Application authoritative platform event/decision/outcome.
7. Foundation -> FSATS Application evidence/query request and response.

The exact externally observable contract behavior, identity attribution, authority classification, request/response semantics, event semantics, freshness, correlation, rejection, fail-closed behavior, and route admission expectations require reconciliation with the Foundation communication boundary and Shared Web planning boundary.

## Required Cross-Workstream Result

A common model shall preserve:

```text
FSATS_SYSTEM != RUNTIME_PRINCIPAL
INFORMATION_OWNER = PRODUCER_APPLICATION
BUSINESS_AUTHORITY_OWNER = DECISION_APPLICATION
WEB = PRESENTATION / REQUEST SURFACE ONLY
FOUNDATION = PLATFORM / GOVERNANCE AUTHORITY ONLY
DELIVERY != ACCEPTANCE
ROUTE_EXISTENCE != AUTHORITY
REQUEST != COMMAND_AUTHORITY
```

This record does not prescribe Foundation or Shared Web internals and creates no runtime route or implementation authority.
