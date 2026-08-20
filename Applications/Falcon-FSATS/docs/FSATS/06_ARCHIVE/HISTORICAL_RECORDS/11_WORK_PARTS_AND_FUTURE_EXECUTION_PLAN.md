# FSATS V1.4 PROPOSED - Work Parts and Future Execution Plan

**Status:** `PART 0 IN PROGRESS / FUTURE IMPLEMENTATION NOT AUTHORIZED`  
**Implementation authority:** `NOT GRANTED`

This file defines the proposed future implementation sequence only. It does not authorize implementation.

## Planning principle

V1.4 preserves the final V1.3 causal build logic and inserts current-Foundation alignment gates before runtime integration.

A Part may later be constructed in parallel with another Part only when the owning contracts, Application boundaries and canonical state dependencies are already stable. Activation authority never follows automatically from construction progress.

# Proposed total: 11 Parts

The earlier ten-Part draft is superseded because Part 0 confirmed that FSTSimA is an independent non-Live Falcon Application in final V1.3 and must not be absorbed into Trading Part 8.

## Part 0 - V1.4 Alignment Closure and Authority Package

**Purpose:** finish migration/alignment before any code.

Contains:

- complete V1.3 source-domain and artifact accounting;
- retain/align/supersede/Owner-decision traceability;
- binding final-V1.3 supersession preservation;
- current Foundation dependency matrix;
- canonical GitHub FCR inventory;
- corrected Application identity inventory;
- core FSATS Manifest candidates plus FSTSimA alignment candidate;
- CON-023 completeness register;
- 4 Guardian + 6 FSAPMA + 12 Trading room map;
- 8 FSTSimA room map as an independent adjacent Application;
- Shared Web/Communication external-Application boundary preservation;
- cross-Application contract families;
- Foundation lifecycle versus trading authority model;
- per-Application resource ownership model;
- research-only Internet versus operational FSAPMA data boundary;
- Fast Track/performance preservation specification;
- Architecture Review and Red-Team closure;
- Owner review gate;
- separate implementation authorization package only if later requested.

**Exit gate:** Owner accepts the V1.4 alignment design and separately authorizes implementation. No later Part may infer authority from Part 0 completion.

---

## Part 1 - Canonical Primitives, Application Shells and Contract Spine

Contains Application identities/package boundaries, CON-023 Manifest implementations for authorized scope, canonical IDs/envelopes/errors/time/authority/provenance, schema/version rules, ports/dependency inversion, health/degraded-state interfaces, evidence identities, room registration/access boundaries, and no operational provider/broker effects.

**Fast Track:** define deadline/traffic-class metadata without enabling runtime routes.

**Gate:** architecture/contract/security tests; no hidden coupling; no runtime authority.

---

## Part 2 - FSAPMA Operational Data Foundation

Contains Provider Registry/onboarding, provider/account/broker capability profiles, canonical trading data products, provider selection/routing/fallback business logic, quota/capacity/free-first logic, quality/freshness/lineage/reconciliation, provider adapters, route/stream requirements and all six FSAPMA rooms.

**Foundation dependencies:** canonical FCR-0005 / Issue #5, FCR-0009 / Issue #9 and FCR-0010 / Issue #10 as applicable.

**Gate:** provider simulation/sandbox evidence, quota exhaustion, stale/corrected data, conflict and failover tests. No Live trading authority.

---

## Part 3 - Trading Core Truth, Markets, Horizons and Immutable Intent

Contains account/environment context, US Equities and Crypto Spot market profiles, dynamic universe, eligibility, horizon profiles/grants, deterministic clocks, immutable Trading Intent, thesis continuity, aggregate-risk grouping, event-derived lifecycle projections and canonical read models.

**Fast Track:** precomputed immutable snapshots and freshness identities required by later decision paths.

**Gate:** deterministic replay, clock/horizon tests, intent immutability, universe correctness and disabled-horizon enforcement.

---

## Part 4 - Analysis, Schools, Strategies and Opportunity Intelligence

Contains Analysis Frameworks, Classical School, Opportunity Hunting School, Central Strategy Catalog, Strategy Controller, ten V1.3 strategy models, regime/feature interpretation, confidence/evidence ceilings, strategy latency classes, market/strategy compatibility and no per-market strategy duplication.

**Gate:** deterministic/counterfactual tests, claim-scope checks, latency-class feasibility and strategy isolation.

---

## Part 5 - Unified Risk, Portfolio/Capital, Final Decision and Fast Track

Contains Unified Risk, hidden-correlation/common-factor/concentration controls, account/custodian/currency-aware capital truth, hierarchical capital reservation, trading-business allocation, economics/EV contract, final decision binding, exposure/protective intent classification, Fast Risk/feasibility guards, immutable/precomputed snapshots, deadline propagation, bounded priority queues, p50/p95/p99/p99.9/max instrumentation and load shedding.

**Foundation dependencies:** canonical FCR-0009 / Issue #9 and FCR-0010 / Issue #10.

**Gate:** risk-splitting, stale-snapshot, pressure/load, deadline exhaustion and tail-latency tests. Fast Track never bypasses Risk/Guardian/authority/evidence/reconciliation controls.

---

## Part 6 - Guardian Protection Kernel, Scoped Containment and Resource Escalation

Contains all four Guardian rooms, crisis states, triggers/playbooks, Minimum Viable Protection Kernel, open-position protection, smallest-safe-scope containment, broad restrictions only for broad threat, recovery/reconciliation/release, extreme/unknown regime resilience, Guardian evidence/epoch and governed resource escalation.

**Foundation dependencies:** canonical FCR-0004 / Issue #4, FCR-0007 / Issue #7, FCR-0009 / Issue #9 and FCR-0010 / Issue #10.

**Gate:** incident-scope, false-positive blast radius, Guardian outage/fail-safe, resource-request denial and recovery tests.

---

## Part 7 - Execution, Position Lifecycle, Reconciliation and Broker Boundary

Contains Dispatch Binding, final authority/Guardian/grant/freshness checks, broker adapter boundary, idempotent order state machine, partial fills/cancel races, UNKNOWN query/reconcile-before-retry, logical fill allocation, multi-intent ownership, position lifecycle, time exit/overdue liquidation obligation, no fictional fills, authoritative event correction and protection-path integration.

**Fast Track:** near-trade/dispatch remains deadline-bound and latency-instrumented but never skips final checks.

**Gate:** failure injection, broker ambiguity, duplicate/out-of-order events, restart/replay, capital reservation and protection tests.

---

## Part 8 - Trading Evidence, Learning, Analytics and Strategy Evolution

This Part contains Trading-owned learning/evolution capabilities only. FSTSimA itself is not implemented inside this Part.

Contains Trading Learning and Knowledge, Analytics and Attribution, Strategy Evolution and Experimentation, counterfactual evaluation, immutable provenance requirements, evidence sufficiency/confidence ceilings, decay detection, candidate generation/registry, interaction graph/change budget, quarantine/freeze/cooldown/rollback, promotion-evidence building, research-only Internet candidate inputs, and governed FSTSimA interfaces.

**Foundation dependencies:** canonical FCR-0006 / Issue #6 and FCR-0008 / Issue #8.

**Gate:** reproducibility, hidden holdout/falsification, mutation testing, no candidate self-promotion and no research-to-live shortcut.

---

## Part 9 - Independent FSTSimA and Digital City Validation Application

**Position:** separate Falcon Application outside FSATS operational authority.

Contains FSTSimA Application shell/Manifest, Simulation MSA, eight Simulation LSAs, clocks/scenarios, market/provider/broker/account/fault/latency simulation, fidelity/calibration, truth oracle/reproducibility/provenance, Digital City, adversarial scenario library, separate credentials/stores/namespaces/authority and enforced non-Live egress.

**Foundation dependencies:** canonical FCR-0006 / Issue #6 and FCR-0011 / Issue #11, plus ordinary Application lifecycle/Manifest/resources/security/routing capabilities.

**Gate:** deterministic reproduction, fidelity declarations, accidental-Live-egress denial, production-state mutation denial, simulator bias/calibration evidence and replay isolation.

---

## Part 10 - Shared Application Integration and Trading Validation Stages

### 10A - Shared Application integration

Falcon Communication and Falcon Web contracts, dashboard/report/control-center requirements, authentication/authorization staying with their owners, and no hidden absorption into FSATS.

### 10B - Validation progression

`Sandbox -> FSTSimA -> Digital City -> Live Shadow -> Paper -> Formal Paper Proof -> Tiny Live Calibration -> Controlled Scaling`

Foundation `ACTIVE` grants none of these trading authorities. Paper, Tiny Live and Live each require explicit Owner/governance authorization, current Foundation capabilities and evidence gates.

**Gate:** stage-specific evidence only. No automatic promotion.

---

# Why 11 Parts

Eleven Parts preserve four independent truths:

1. code cannot begin before current-Foundation alignment closes;
2. FSAPMA, Trading and Guardian remain independent Applications;
3. deterministic Risk/capital/protection/execution foundations precede advanced adaptive promotion;
4. FSTSimA remains an independent non-Live Application rather than being hidden inside Trading.

# Global stop rules

At every Part:

- if a needed generic Foundation capability is confirmed missing/partial/incompatible, raise/update a canonical GitHub FCR and block only dependent integration;
- do not create a local fake Foundation service;
- no Part grants the next Part automatically;
- no build/test success grants Paper/Tiny Live/Live authority;
- any material architectural change to final V1.3 intent requires explicit traceability and Owner review;
- awareness entities remain outside the synchronous operational hot path;
- independent Application boundaries may not be fused for latency convenience.
