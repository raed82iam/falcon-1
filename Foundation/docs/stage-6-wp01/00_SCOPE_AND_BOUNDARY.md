# Stage 6 WP-01 — Scope and Boundary

Status: AUTHORIZED / PRE-IMPLEMENTATION
Date: 2026-08-08
Branch: foundation-development

## Purpose

WP-01 establishes canonical Application-neutral resource-governance primitives that later Stage 6 Work Packages may use without redefining identity, quantity, evidence, priority, pressure, request/decision or lifecycle semantics.

## In scope

- Canonical strongly typed identifiers for:
  - resource class;
  - Application principal;
  - resource allocation/grant;
  - resource request;
  - resource decision;
  - evidence;
  - correlation/causation;
  - resource epoch/version;
  - priority class;
  - technical criticality class.
- Canonical resource quantity/unit primitive.
- Canonical bounded lifetime/effective-window primitive.
- Canonical evidence-reference primitive.
- Canonical pressure-state vocabulary.
- Canonical resource-decision vocabulary sufficient to distinguish GRANT/PARTIAL_GRANT/CAP/DENY/DEFER/REVOKE/REDUCE/RESTORE.
- Canonical reclaimability classification as a primitive vocabulary only.
- Deterministic canonical identity material/hash helper for immutable resource-governance records.
- Fail-closed primitive validation for blank/malformed IDs, invalid quantities, invalid units and invalid lifetimes.

## Explicitly out of scope

- total-resource truth or capacity discovery;
- allocation controller;
- quota/ceiling enforcement;
- Foundation survival-floor calculation;
- priority-policy evaluation;
- pressure calculation;
- request approval/denial engine;
- reclamation/redistribution/rebalance/restoration execution;
- Application-facing resource message transport;
- load-shedding business decisions;
- Trading semantics;
- QoS scheduling/latency enforcement;
- artifact package/feed/publication/consumption implementation;
- external egress/credential handling;
- deployment/runtime/baseline activation.

## Application-response integration

FCR-0007/FCR-0010 Application declarations influence primitive completeness only. WP-01 does not encode Trading-specific principal names, business degradation order or message-route behavior into Foundation production semantics.

The later Owner clarification makes Trading-related Applications the highest cross-Application Application-priority domain. WP-01 therefore provides a generic priority-class primitive capable of representing this future policy, but WP-01 does not assign any Application to a priority class. Assignment/policy enforcement belongs to later authorized Stage 6 work.

## Mandatory invariants

- `RESOURCE_REQUEST != RESOURCE_GRANT`.
- `RESOURCE_AVAILABILITY != RESOURCE_AUTHORITY`.
- `APPLICATION_PRIORITY != FOUNDATION_CONTROL_PLANE_PRIORITY`.
- `APPLICATION_INTERNAL_PRIORITY != FOUNDATION_PRIORITY_CLASS`.
- `TEMPORARY_GRANT != PERMANENT_ENTITLEMENT`.
- `PRESSURE_OBSERVED != PERMISSION_TO_EXCEED_CEILING`.
- identifiers and evidence references are immutable values, not authority.
- presence of a priority identifier does not create priority authority.
- presence of a decision identifier does not prove a decision result.
- no primitive may embed Trading, Accounting, Warehouse or other Application business semantics.

## Authorization boundary

Only WP-01 is authorized. WP-02 through WP-10 remain unauthorized.
