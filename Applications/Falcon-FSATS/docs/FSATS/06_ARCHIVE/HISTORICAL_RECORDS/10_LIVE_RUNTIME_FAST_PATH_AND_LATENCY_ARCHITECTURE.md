# FSATS V1.4 PROPOSED — Live Runtime Fast Path and Latency Architecture

**Status:** PROPOSED DESIGN / OWNER REVIEW REQUIRED  
**Authority:** design only; no implementation, deployment, Paper, Tiny Live, or Live authority.

## 1. Purpose

Preserve the mature V1.3 latency and fast-path design intent while rebinding all cross-Application and Foundation integration to the current Falcon Foundation.

Performance SHALL NOT be achieved by bypassing Risk, Guardian protection, authority, evidence, reconciliation, or declared Application boundaries.

## 2. Three runtime planes

V1.4 preserves three logically distinct runtime planes:

1. **Trading Data Plane** — latency-sensitive market state, feature state, decision evaluation, fast Risk, capital reservation, execution feasibility, dispatch binding, and execution.
2. **Control and Awareness Plane** — MSA/LSA/CSA supervision, learning, attribution, reporting, research, governance evidence enrichment, and development work. This plane SHALL NOT synchronously block the Trading Data Plane.
3. **Independent Protection Plane** — current Guardian protection state, protection commands, safety epochs, open-position protection, and emergency/protective coordination. Protection remains independently observable and shall not depend on slow research/analytics work.

Logical separation does not require network separation. Components may be colocated when evidence proves that remote boundaries create unacceptable tail latency, while ownership/contracts remain logically intact.

## 3. Canonical latency-sensitive dispatch path

The V1.4 design preserves the final V1.3 intent that a latency-sensitive dispatch path is bounded and deterministic:

```text
Approved Trading Decision or Authorized Guardian Protective Command
→ Order Intent Classification
→ Fresh Executable Market Snapshot + Broker-Truth Snapshot
→ Fast Risk Guard OR Protective Risk-Reduction Guard
→ Exposure-Creating Intent: Execution-Adjusted Edge Revalidation
  OR
  Protective Intent: Protective Execution Feasibility Evaluation
→ Fast Allocation / Capital Reservation Guard
→ Atomic Reservation / Capital Lock
→ Dispatch Binding and Final Validation
→ Execution Gateway / Broker Adapter
→ Broker Response / UNKNOWN handling
→ Reconciliation
```

No stage may silently remove a safety gate merely to reduce latency.

## 4. Fast versus full controllers

V1.4 preserves the V1.3 split between asynchronous deep computation and bounded local fast guards:

- Full Risk computes deep/versioned risk envelopes asynchronously.
- Fast Risk applies the latest valid signed/versioned envelope locally in the hot path.
- Portfolio/Capital planning may compute asynchronously; reservation/lock operations remain atomic and bounded.
- Provider routing should be prepared before an opportunity reaches the dispatch path where possible.
- Protective Risk-Reduction Guard evaluates bounded risk-reduction feasibility and duplicate-order safety without requiring positive expected profit.

Fast guards SHALL NOT invent weaker safety policy. They apply precomputed, valid, attributable policy within explicit freshness/version bounds.

## 5. Precomputation and immutable snapshots

Latency-critical decisions SHALL consume immutable/versioned snapshots rather than trigger broad synchronous recomputation.

Relevant snapshots may include:

- market regime;
- features;
- liquidity state;
- instrument eligibility;
- strategy eligibility;
- Risk envelope;
- available/reserved capital;
- correlation/exposure state;
- Guardian state/epoch;
- provider route state;
- broker/account capability;
- executable market truth.

Every dispatch SHALL bind to exact required snapshot identities/versions/digests and reject stale, expired, revoked, or inconsistent bindings.

## 6. Deadline propagation

The original opportunity/protection deadline travels through the whole hot path.

A downstream stage receives the **remaining** time budget. It SHALL NOT reset a fresh full timeout at every hop.

If safe completion is no longer possible inside the remaining budget, the path returns an explicit deadline/expiry outcome and SHALL NOT dispatch stale intent.

## 7. Bounded queues and priority lanes

All live queues must be bounded.

Priority order shall preserve safety-critical work before optional work. Exact implementation is not authorized here, but V1.4 requires distinct priority classes for at least:

1. Guardian/protection and open-position safety;
2. broker truth / execution reconciliation;
3. near-trade / dispatch-critical work;
4. active-watch updates;
5. candidate-universe work;
6. broad discovery;
7. research / learning / analytics.

Queue saturation SHALL cause governed load shedding rather than unbounded waiting.

## 8. Load shedding

Under CPU, memory, event-lag, queue-depth, provider-capacity, broker-latency, or snapshot-age pressure, optional work is shed first.

Research, broad discovery, candidate refresh, reporting, and noncritical analytics SHALL be reduced before:

- Guardian/protection;
- open positions;
- broker truth;
- orders;
- capital reservation truth;
- reconciliation;
- execution safety.

Load shedding SHALL NOT weaken mandatory Risk, authority, Guardian, reconciliation, or execution-binding checks.

## 9. Strategy latency classes and live-feasibility gate

Each strategy version SHALL declare a latency class and evidence including:

- opportunity lifetime model;
- minimum data freshness;
- maximum decision time;
- provider/broker assumptions;
- required fallback behavior;
- measured tail latency.

A strategy cannot become Live-eligible merely because its average latency is acceptable. Tail latency, safety margin, broker path, failure/degradation behavior, binding, and reconciliation must fit the opportunity lifetime.

V1.4 makes no HFT claim.

## 10. Tail-latency evidence

Performance evidence shall include at least:

- p50;
- p95;
- p99;
- p99.9;
- maximum;
- queue wait;
- snapshot age;
- deadline misses;
- timeout/cancellation rate;
- external provider latency;
- broker acknowledgement latency;
- protection-lane starvation.

V1.3 numeric budgets are retained as historical benchmark hypotheses only, not current V1.4 guarantees. V1.4 implementation must establish budgets from measured deployment evidence.

## 11. Hot-path prohibitions

The latency-sensitive path SHALL NOT depend synchronously on:

- MSA/LSA/CSA research or self-development;
- dashboards;
- broad analytics/database queries;
- runtime model loading;
- first-use connection establishment;
- unbounded queues;
- cross-region synchronous calls where avoidable;
- nonessential reporting/provenance enrichment;
- direct Internet research.

Operational trading truth still enters through FSAPMA and governed contracts.

## 12. Colocation without ownership collapse

Latency-critical components may be colocated in a Trading runtime node/process/host when later evidence justifies it.

Colocation SHALL NOT:

- merge LSA room ownership;
- create direct hidden cross-Application access;
- turn in-process calls into undeclared authority;
- bypass Foundation communication/admission requirements where a boundary remains cross-Application.

Physical optimization is subordinate to logical ownership and contract boundaries.

## 13. Simulator / Shadow parity

The same production execution-domain logic should be exercised against governed simulated/paper adapters rather than maintaining a separate permissive simulation-only execution path.

Simulation/Shadow/Paper shall reproduce hot-path ordering, intent classification, Risk/feasibility gates, reservation, binding, UNKNOWN handling, reconciliation, and deadline semantics as closely as the mode permits.

Simulation success is evidence only and grants no Live authority.

## 14. Foundation dependencies

Some low-latency behavior crosses current Foundation boundaries that are planned but not yet available. V1.4 therefore requires Application-side design requirements to be raised early through FCRs where appropriate, particularly for:

- latency-sensitive governed Application routes;
- priority/flow semantics that preserve protection and near-trade traffic;
- bounded delivery/retry behavior;
- route readiness/lease or equivalent pre-established delivery semantics;
- observability of queueing and tail latency;
- fail-closed behavior when a route cannot meet freshness/deadline constraints.

FSATS SHALL NOT implement a hidden replacement Service Bus or bypass Foundation to obtain speed.

## 15. V1.4 acceptance gate

V1.4 design is not ready for Owner acceptance unless Fast Path / latency intent is traceably preserved in:

- Trading execution design;
- Risk design;
- FSAPMA data-delivery design;
- Guardian protection design;
- cross-Application contract requirements;
- Foundation dependency/FCR register;
- Simulator/Shadow/Paper design;
- Red-Team review;
- future verification plan.
