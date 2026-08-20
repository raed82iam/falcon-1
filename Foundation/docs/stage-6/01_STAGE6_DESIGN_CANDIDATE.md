# Stage 6 Design Candidate — Foundation Resource Governance and Operational Pressure Control

Status: PROPOSED / OWNER DESIGN ACCEPTANCE REQUIRED
Date: 2026-08-08
Branch: foundation-development
Governing basis: Falcon Vision, Falcon Constitution, SYS-006, accepted Stage 0–5 baseline, Issue #1 FCR protocol, Owner Stage 6 priority clarification of 2026-08-08

## 1. Stage objective

Stage 6 establishes the generic Foundation-owned resource-governance plane required by SYS-006. It provides attributable technical resource truth and bounded decisions for allocation, ceilings, floors, reserves, pressure, additional-resource requests, redistribution and restoration without taking ownership of Application business semantics.

## 2. Architectural placement

Stage 6 belongs to Foundation Shared Services/Kernel governance responsibilities. It is upstream of Application resource consumption and may supply governed evidence to accepted services such as Stage 5 delivery flow control.

It does not replace:
- Stage 4 lifecycle/state/evidence/reconciliation owners;
- Stage 5 messaging, routing, delivery, events, protection or Application lifecycle eligibility;
- Application-owned internal resource distribution within an admitted grant.

## 3. Core model

Stage 6 design SHALL distinguish:
- resource capacity truth;
- allocatable capacity;
- protection floor;
- recovery reserve;
- Application allocation;
- quota;
- hard ceiling;
- cross-Application priority class;
- technical priority/criticality within governed work;
- pressure observation;
- resource request;
- grant/deny/defer/rebalance decision;
- enforcement state;
- restoration condition;
- evidence identity and lineage.

Resource classes SHALL be generic and typed. Stage 6 SHALL NOT hard-code Trading business semantics or financial capital as infrastructure resources.

## 4. Owner-approved cross-Application priority rule

The Owner has directed that Trading-related Applications are the highest Application-priority domain within Falcon resource governance.

This rule means:
- the governed Trading Application family receives the highest Application workload priority relative to lower-priority non-Trading Applications such as future Accounting, Warehouse, and similar workloads;
- under resource pressure, Foundation may reclaim bounded reclaimable technical resources from lower-priority Applications and redistribute them to higher-priority Trading workloads according to governed evidence and current need;
- under severe or critical pressure, Foundation may suspend or reduce lower-priority Application allocations down to their currently governed minimum or zero reclaimable Application allocation when necessary to preserve the highest-priority Trading workload;
- temporary redistribution must remain attributable, reversible, evidenced, and subject to restoration when the pressure condition clears;
- lower-priority Applications do not acquire a right to retain unused or reclaimable allocation against a valid higher-priority Trading need.

This cross-Application priority is an explicit Owner policy and therefore is not inferred from Trading business payloads, self-declared urgency, profitability, or an Application-local request.

### Foundation survival boundary

Trading is the highest **Application** priority, not an authority above Falcon Foundation itself.

Foundation survival/protection floors, Authority, Health/Recovery, evidence integrity, security enforcement, and the minimum resources required to keep the governed operating system able to protect, account for, revoke, restore, and control Applications are non-reclaimable except under a separately explicit higher authority compatible with the Vision and Constitution.

Therefore `all resources to Trading` means all resources that are legitimately reclaimable from lower-priority Application workloads, not destruction of the Foundation control plane required to keep Trading safe and governed.

## 5. Authority model

Every resource-governance decision must bind exact:
- subject/Application identity when applicable;
- resource class;
- purpose;
- requested or allocated quantity/limit;
- duration or validity boundary where applicable;
- cross-Application priority basis;
- technical criticality basis where applicable;
- authority basis;
- evidence identity;
- current pressure/system condition;
- restoration/release conditions.

No resource request creates a grant. No observed spare capacity creates authority. No historical grant creates permanent entitlement. No Application may promote its own priority class.

## 6. Isolation model

Foundation must prevent:
- one Application consuming another Application's allocation without an explicit Foundation redistribution decision;
- hidden borrowing outside an explicit governed decision;
- Application-local priorities overriding Foundation-controlled cross-Application priority enforcement;
- an Application self-declaring a higher priority class;
- pressure handling from eroding Foundation survival/protection floors or recovery reserves without explicit higher authority;
- ambiguous or stale enforcement truth from being treated as success.

The explicit Owner-approved Trading Application priority rule is a governed cross-Application policy and does not transfer Trading business semantics into Foundation.

## 7. Pressure, preemption and degradation

Pressure is technical Foundation truth, not Application business truth.

Stage 6 must support deterministic states sufficient to represent normal, constrained, protected/reserved, preemption/reclamation, degraded, critical and restoration conditions without inventing business actions.

Foundation may progressively reclaim resources from lower-priority Applications when higher-priority demand cannot be satisfied from free allocatable capacity. Reclamation must follow explicit policy, evidence, isolation, ordering and restoration rules rather than arbitrary starvation.

Applications may receive only their attributable state/decision/evidence and may perform Application-owned load shedding within their admitted authority.

## 8. Additional-resource request boundary

A generic Application may submit an evidenced additional-resource request through its governed Application boundary. Foundation owns the decision and may allow, cap, deny, defer, reclaim from lower-priority Applications, or rebalance according to current authority, Owner-approved cross-Application priority, technical criticality, Foundation survival/protection floors, recovery reserves, isolation and availability.

FCR-0007 is a direct design input for this boundary.

The existing Trading design already identifies Trading Guardian as an emergency resource-escalation producer. Foundation has requested Application-side clarification through FCR-0007 regarding any additional ordinary requesters, internal evidence originators, exact message families and Trading internal degradation/priority semantics. Foundation SHALL NOT invent those Application-owned details.

## 9. Per-Application resource-state boundary

Foundation must expose enough attributable technical resource state for an Application to understand its own grant, ceiling, pressure/enforcement condition, request outcome, reclamation/rebalance state and restoration conditions without exposing or granting another Application's allocation.

FCR-0010 is a direct design input for this boundary.

Foundation has requested Application-side clarification through FCR-0010 for exact Trading consumers, message/data fields, degradation hierarchy, reaction to reduction/revocation/restoration, and the boundary between Application-internal projections and Foundation-facing principals.

## 10. Trading internal-priority opacity

The Owner-approved rule establishes Trading as the highest cross-Application priority domain. It does not authorize Foundation to decide which Trading strategy, market, order, broker, provider, LSA, component, or business workflow is more valuable.

Trading-internal prioritization and degradation semantics remain Application-owned and must be supplied through governed declarations/evidence. Foundation may enforce the technical resource envelope and the Owner-approved cross-Application priority policy while treating Trading business meaning as opaque.

## 11. Integration rules

Stage 6 may provide governed pressure/priority/allocation evidence to Stage 5 WP-06 and other future generic consumers, but consumer services remain owners of their own behavior.

Stage 6 must not create routes, deliver messages, publish events, activate Applications, or execute Application business actions.

## 12. Security and fail-closed rules

Missing, stale, revoked, malformed, contradictory or ambiguous authority/resource/enforcement evidence must fail closed or reduce authority as required by SYS-006.

Resource-governance evidence must not contain secrets or Application business payloads.

A claimed Trading priority must bind to the governed Application identity/policy and cannot be asserted by arbitrary messages, components, or untrusted metadata.

## 13. Determinism and auditability

Materially equivalent inputs must produce equivalent decisions and identities. Material mutation of subject, resource class, quantity/limit, authority, pressure, priority, evidence or restoration conditions must alter the governed decision identity or outcome where applicable.

Resource reclamation and restoration must be reconstructable, including which lower-priority allocation was reduced, why, under what authority, for which higher-priority need, for how long, and how/when it was restored.

## 14. Stage boundaries

Explicitly out of scope:
- financial capital allocation and portfolio/Risk semantics;
- strategy/market/instrument/broker/provider prioritization;
- Trading-internal business-priority decisions not supplied through an approved Application contract;
- external egress and credentials;
- research Internet access;
- FSA autonomous-evolution control plane;
- new transport QoS semantics beyond consuming resource truth;
- deployment/runtime activation/baseline activation;
- Stage 7–9 implementation.

## 15. Acceptance model

Stage 6 implementation, if later authorized, must pass:
- dedicated verifier coverage for every accepted WP;
- Architecture gate;
- Security gate;
- Baseline Integrity;
- all accepted predecessor regressions;
- deterministic reruns;
- independent architecture/security/completeness review;
- FCR reconciliation;
- explicit Owner acceptance and closure.

This document is a design candidate and does not grant implementation authority.
