# Stage 6 WP-08 — Existing Capability Reconciliation

**Status:** PLANNING INPUT / NOT IMPLEMENTATION AUTHORITY  
**Stage/WP:** Stage 6 WP-08 — Per-Application Resource State and Load-Shedding Signal Boundary  
**Foundation baseline inspected:** `88ad68ff81de49a2f9b5be9852baaf24df2398a4`  
**Date:** 2026-08-10

## 1. Governing constraints

This reconciliation is Foundation-only and follows `docs/development/FOUNDATION_WORKSTREAM_RULES.md`.

Preserved accepted state:

- Stage 6 WP-01 through WP-07 are `ACCEPTED_AND_CLOSED`.
- WP-07 exact accepted technical baseline is `5db97f6b99dafabe76baa4a1893ffb84e2cc119e`.
- WP-08 implementation authority is **NOT GRANTED**.
- WP-09/WP-10 are not authorized.

`IMP-001 v1.3` identifies WP-08 as **Per-Application Resource State and Load-Shedding Signal Boundary**.

## 2. Applicable FCR state

### FCR-0010

Current Foundation disposition preserves WP-07 as closed and identifies the remaining future WP-08 obligation as the per-Application resource-state/load-shedding signal boundary.

Historical and current FCR material requires a generic Application-facing view of its own resource state, including accepted resource allocation/ceiling/pressure and relevant request/restoration state, without exposing another Application's allocation or transferring Foundation authority.

### FCR-0031

The accepted FSARM boundary remains relevant as a consumer-side coordination requirement. WP-08 must preserve:

- Foundation-authoritative grants/ceilings and total-resource truth remain Foundation-owned.
- Delegated effective redistribution remains distinct from authoritative allocation mutation.
- `INTERNAL_REDISTRIBUTION_FIRST` remains preserved.
- Borrowed effective capacity retains exact source Application + source Grant provenance.
- No opaque aggregate pool is created.

## 3. Existing accepted capabilities that WP-08 SHALL reuse

### WP-01 — canonical resource primitives

Already provides canonical identities, quantities, pressure/reclaimability/decision primitives, evidence references, epochs, correlation/causation identities and deterministic identity foundations.

WP-08 SHALL NOT duplicate these primitives.

### WP-02 — Foundation resource truth, floors and reserves

Already provides Foundation total-resource truth and protected/recovery capacity semantics.

WP-08 may reference accepted resource truth but SHALL NOT redefine allocatable capacity, protection floors or recovery reserves.

### WP-03 — Application allocation/quota/ceiling/isolation

Already provides exact per-Application authoritative allocation/grant/quota/ceiling truth and Application-scoped views.

WP-08 SHALL reuse this as the authoritative allocation predecessor and SHALL NOT create a second allocation source of truth.

### WP-04 — Application priority and technical criticality

Already separates Application priority from Foundation technical criticality.

WP-08 may consume accepted technical/resource ordering evidence where needed, but SHALL NOT turn technical pressure into Application business priority and SHALL NOT choose which internal Application component/strategy/service is shed first.

### WP-05 — pressure, preemption eligibility and enforcement-state truth

Already provides:

- Application-scoped resource pressure truth;
- pressure availability/state;
- utilization basis points;
- enforcement observation state;
- preemption eligibility-for-consideration;
- exact Application-scoped view.

WP-08 SHALL consume this truth. It SHALL NOT recalculate pressure using a competing model or treat reclaimability/pressure as mutation authority.

### WP-06 — additional resource request/decision truth

Already provides direct/aggregate request attribution and Foundation decision outcomes with exact predecessor identities.

WP-08 may expose the latest relevant request/decision status to the exact affected Application where needed for reconstructable resource-state reporting. It SHALL NOT resubmit requests, decide requests, or reinterpret Grant/PartialGrant/Cap/Deny/Defer.

### WP-07 — redistribution/rebalance/restoration and accepted post-mutation truth

Already provides:

- delegated effective redistribution within a governed envelope;
- borrowed effective-capacity provenance;
- Foundation-authoritative Reduce/Revoke/Restore mutation lane;
- effect-application evidence;
- accepted post-mutation truth;
- distinction between intent, applied effect and accepted truth;
- quiescence and restoration-basis protections.

WP-08 SHALL consume accepted WP-07 truth and evidence. It SHALL NOT create a second redistribution/mutation executor.

## 4. Capability gap that remains for WP-08

The accepted predecessors do **not** yet provide one generic, canonical Application-facing resource-state projection that combines the exact Application's relevant accepted resource truth into one reconstructable view.

The accepted predecessors also do **not** yet provide a generic load-shedding **signal boundary** that tells an Application what resource-pressure response is required or recommended while keeping Application-internal shedding selection outside Foundation ownership.

Therefore WP-08 remains a real capability gap.

## 5. Required WP-08 ownership

WP-08 should own only these generic Foundation capabilities:

1. **Per-Application Resource State Projection**
   - exact Application identity;
   - exact resource class;
   - authoritative grant/allocation/quota/ceiling predecessor identity;
   - currently effective capacity derived from accepted WP-07 state where applicable;
   - exact pressure/enforcement state from WP-05;
   - exact relevant WP-06 request/decision reference where present;
   - exact WP-07 mutation/effective-distribution evidence reference where relevant;
   - deterministic identity and observation time;
   - Application isolation: an Application sees only its own projection.

2. **Load-Shedding Signal Boundary**
   - generic technical signal only;
   - exact Application + resource class attribution;
   - source truth/evidence identities;
   - required or recommended reduction expressed as a quantity/target capacity, not as Application business-component instructions;
   - deterministic and reconstructable signal identity;
   - fail closed when required predecessor truth is unavailable/stale/inconsistent.

## 6. Explicit non-ownership

WP-08 SHALL NOT:

- execute load shedding inside an Application;
- select which Strategy, Guardian function, simulator, provider, component, model or business workflow is degraded/stopped first;
- mutate Foundation authoritative grants/ceilings;
- execute redistribution/reclamation/rebalance/restoration;
- submit or decide additional-resource requests;
- mint authority from pressure, priority, reclaimability or a signal;
- expose another Application's resource state;
- create an FSATS/TARC/FSARM-specific Foundation contract;
- implement WP-09 cross-subsystem integration/hardening;
- create runtime deployment, production, external-access or financial authority.

## 7. Key semantic boundary

`RESOURCE_STATE_PROJECTION != RESOURCE_AUTHORITY`

`LOAD_SHEDDING_SIGNAL != LOAD_SHEDDING_EXECUTOR`

`PRESSURE != AUTHORITY`

`RECLAIMABILITY != MUTATION_AUTHORITY`

`APPLICATION_INTERNAL_SHEDDING_ORDER = APPLICATION_OWNED`

`FOUNDATION_RESOURCE_REDUCTION_REQUIREMENT = FOUNDATION_DERIVED_FROM_ACCEPTED_RESOURCE_TRUTH`

## 8. Reconciliation result

**Result: CAPABILITY GAP CONFIRMED / WP-08 PLANNING REQUIRED.**

No existing accepted capability should be reopened or redesigned. WP-08 should be implemented prospectively as a projection/signal layer over exact accepted WP-03/WP-05/WP-06/WP-07 truth, with strict Application isolation and no Application business semantics.

This reconciliation grants no implementation authority.
