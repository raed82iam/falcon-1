# FCR-0004 — Trading Guardian to Foundation Guardian Resource Escalation

**Status:** PROPOSED  
**Requester:** Application workstream  
**Application:** Falcon Trading Guardian Application  
**Classification:** `PLANNED_CAPABILITY_INPUT`  
**Date:** 2026-08-07

## Requested Foundation capability

A generic governed Application-protection route that allows an Application Guardian to submit an evidenced request to the Foundation-owned Guardian/protection/resource-governance boundary for additional technical resources for the affected Application during a material broad domain threat.

## Application use case

Trading Guardian normally contains problems at the smallest safe scope. If a broad trading-domain incident threatens safe operation and the admitted Application allocation is insufficient for required protection/reconciliation work, Trading Guardian needs to request additional Foundation-controlled capacity without self-allocating resources or gaining Foundation authority.

## Current Foundation evidence checked

- APP-001 Application Boundary and Lifecycle;
- CON-023 Application Contract and Manifest;
- ADR-I012 Plug-and-Play Application Integration Boundary;
- ADR-I015 Application and Awareness Alignment;
- SYS-006 Multi-Level Resource Governance.

## Observed gap

`PLANNED / NOT YET CONFIRMED AVAILABLE FOR THIS USE CASE`.

Current Foundation authority clearly retains resource allocation and prevents Applications from self-approving additional resources. FSATS needs the future generic protection/request boundary to preserve that ownership while allowing rapid evidenced escalation.

This FCR does not claim Foundation is deficient and does not prescribe a Foundation implementation.

## Application-side alternatives considered

1. Trading Guardian self-allocates Foundation resources — rejected; violates SYS-006.
2. FSATS owns a shared resource coordinator — rejected; creates hidden Foundation resource authority.
3. Application simply fails when allocation is insufficient — unsafe for broad protection/reconciliation incidents.

## Required outcome

A generic Foundation-owned mechanism should be able to receive an attributable, scoped, expiring and evidenced Application protection/resource request; independently approve, deny, cap or modify it; and return an attributable decision without transferring Foundation resource authority to the requesting Application.

The request should support affected Application identity, incident scope/severity, requested resource class/outcome, existing allocation pressure, protection purpose, duration/expiry, evidence identity and restoration/release conditions.

## Blocking impact

V1.4 architecture can continue.

Future runtime use of Trading Guardian resource escalation remains blocked until an appropriate Foundation capability/contract is available and separately authorized.

## Authority rule

This FCR is a request/design input only. It does not authorize an Application worker to modify Foundation and creates no implementation, runtime, deployment, financial or production authority.
