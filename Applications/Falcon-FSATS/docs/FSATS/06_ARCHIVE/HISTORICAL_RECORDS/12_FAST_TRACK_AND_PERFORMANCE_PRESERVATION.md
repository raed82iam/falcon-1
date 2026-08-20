# FSATS V1.4 PROPOSED — Fast Track and Performance Preservation

**Status:** `PROPOSED / OWNER REVIEW REQUIRED`  
**Implementation authority:** `NOT GRANTED`

## Purpose

V1.4 SHALL preserve the V1.3 Fast Track / hot-path / latency architecture as an explicit architectural requirement. It is not an optional optimization and SHALL NOT be silently lost during Foundation alignment.

## Preserved V1.3 performance model

### 1. Control plane versus latency-sensitive data/decision path

Research, discovery, self-awareness learning, reporting and non-urgent control work SHALL NOT sit on the critical near-trade path when an equivalent precomputed/contracted representation can be used safely.

### 2. Immutable/precomputed truth

Latency-sensitive decisions SHOULD consume immutable or versioned snapshots prepared before the opportunity where safe, including relevant market capability, account capability, risk/capital state, Guardian state and authority/grant identities.

Snapshot use does not remove dispatch-time freshness/authority validation.

### 3. Deadline propagation

The original opportunity deadline is carried end to end. A downstream component receives the remaining budget, not a fresh timeout. Work that cannot complete safely within the remaining budget must reject/expire/degrade rather than silently extend the opportunity lifetime.

### 4. Bounded queues and priority lanes

All runtime queues on the trading path are bounded. Priority ordering protects, at minimum:

1. Guardian/protection work;
2. reconciliation and authoritative truth recovery;
3. open-position management;
4. near-trade/dispatch work;
5. active watch;
6. candidates;
7. discovery;
8. research/learning.

Pressure triggers controlled load shedding instead of unbounded queue growth.

### 5. Load shedding

Research/discovery/candidate breadth is reduced before safety-critical work. Under pressure the system may narrow universe, reduce concurrency or suspend new exposure, but SHALL NOT weaken Guardian, Risk, reconciliation, authority or evidence controls.

### 6. Tail-latency evidence

Performance evidence includes p50, p95, p99, p99.9 and maximum, together with queue depth, event lag, snapshot age, provider delay and broker acknowledgement delay where applicable.

V1.3's initial internal p99 target for latency-sensitive non-HFT opportunities remains a design target subject to empirical validation, not a guaranteed production claim.

### 7. Strategy latency classes

A strategy may only be eligible when its required timing is compatible with observed end-to-end latency and data freshness. Profitability evidence cannot override an infeasible latency class.

### 8. Same-Application colocation

Logical rooms remain isolated by ownership/contracts. Latency-critical components of the same Application may later be colocated in one process/host when evidence supports it. Colocation SHALL NOT erase LSA/CSA boundaries or permit private cross-room state mutation.

### 9. Cross-Application Fast Track

Cross-Application traffic remains governed. V1.4 SHALL NOT bypass Foundation routing or fuse Guardian/FSAPMA/Trading into one hidden process merely to gain speed.

Canonical **FCR-0009** (GitHub Issue #9) requests generic latency/deadline/QoS-aware transport behavior for flows that must cross Application boundaries.

### 10. Resource pressure

Each Application owns load shedding within its admitted Foundation allocation. Foundation owns total-resource truth and redistribution. Canonical **FCR-0010** (GitHub Issue #10) requests the generic pressure/telemetry interface needed for deterministic Application response.

## Red-Team invariants

Fast Track is invalid if any implementation:

- skips Unified Risk;
- skips Guardian state/authority checks;
- skips intent classification or applicable feasibility gates;
- resets the deadline at every hop;
- uses unbounded queues;
- turns stale snapshot data into authoritative truth;
- bypasses reconciliation after ambiguous broker outcomes;
- uses awareness Internet research as operational market data;
- opens direct cross-Application memory/database access;
- self-allocates Foundation resources;
- measures only average latency and ignores tail behavior.

## Acceptance evidence before any future Tiny Live proposal

Replay/load tests must cover normal, burst, provider delay, broker delay, queue saturation, CPU pressure, partial outage, stale snapshot, route degradation and recovery scenarios with declared p99/p99.9 outcomes and no safety-control bypass.
