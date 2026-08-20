# FCR-0006 — Latency-Aware Application Transport and QoS

**Status:** PROPOSED APPLICATION REQUIREMENT INPUT  
**Classification:** `PLANNED_CAPABILITY_INPUT`  
**Requester:** FSAPMA / Falcon Self-Aware Trading Application / Falcon Trading Guardian Application  
**Foundation modification authority:** NOT GRANTED

## Requested Foundation capability

A generic governed Application-to-Application transport capability able to preserve bounded latency-sensitive delivery semantics for declared traffic classes without exposing business payload meaning to Foundation.

## Exact FSATS use case

FSATS V1.3 preserves a latency-sensitive Trading Data Plane / Fast Track architecture. Cross-Application flows such as normalized market data from FSAPMA to Trading, protection commands from Guardian to Trading, reconciliation and open-position protection events must not be forced through an unbounded or undifferentiated route that destroys the application's deadline and safety model.

FSATS requires a generic Foundation outcome capable of carrying declared transport metadata such as:

- message traffic class / technical priority class;
- end-to-end deadline or remaining deadline budget;
- bounded queue behavior;
- overload/backpressure visibility;
- deterministic rejection/expiry when a deadline cannot be met safely;
- idempotency/correlation/causation identity;
- route health and tail-latency evidence;
- isolation between Applications/tenants/routes;
- no weakening of security, authority, Risk, Guardian, or evidence controls to gain speed.

## Foundation evidence checked

- APP-001 requires declared contracts/routes and Application isolation.
- CON-023 requires communication, resource, degraded-behavior and evidence declarations.
- SYS-006 defines Foundation technical priority/resource governance.
- Relevant Stage 5 communication/runtime implementation is not yet assumed available by FSATS.

## Observed gap

`PLANNED / NOT YET AVAILABLE` for the concrete runtime behavior required by this use case.

This FCR does not assert that Foundation must implement FSATS-specific Fast Track logic. It requests a generic transport boundary capable of preserving declared latency/deadline/QoS metadata and safe overload behavior.

## Application-side alternatives

FSATS may colocate latency-critical components that belong to the same Application where allowed, but SHALL NOT bypass Application boundaries, Foundation-admitted routes, security, authority, evidence, or another Application's ownership merely to reduce latency.

## Required boundary outcome

Foundation should provide or explicitly reject/limit a generic declared mechanism by which Applications can request and observe latency-sensitive route behavior while Foundation retains routing, technical-priority, isolation, security and resource authority.

## Blocking impact

- Does NOT block V1.4 design.
- Blocks any future claim that cross-Application Fast Track latency behavior is implementation-ready until the Foundation transport contract is confirmed.
- Does not block independent same-Application design, simulation or latency modeling.

## Authority rule

This FCR is a design input only. Foundation may accept, reject, merge, defer or solve the requirement generically. It grants no implementation, runtime, Paper, Tiny Live or Live authority.
