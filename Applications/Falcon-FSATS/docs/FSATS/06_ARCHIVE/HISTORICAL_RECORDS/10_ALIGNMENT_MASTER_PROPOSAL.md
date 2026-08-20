# FSATS V1.4 PROPOSED - Alignment Master Proposal

**Status:** `PART 0 ALIGNMENT BASELINE / OWNER REVIEW REQUIRED`  
**Implementation authority:** `NOT_GRANTED`  
**Runtime authority:** `NOT_GRANTED`  
**Paper / Tiny Live / Live authority:** `NOT_GRANTED`

## 1. Governing migration rule

FSATS V1.4 is not a greenfield redesign.

```text
FSATS V1.4
= Final FSATS V1.3 Architecture
+ Current Falcon Foundation Alignment
+ Post-V1.3 Owner Clarifications
- Structures already superseded/removed in final V1.3
```

A mature V1.3 capability remains target architecture unless one of these is demonstrated:

1. conflict with current Foundation authority/boundary;
2. explicit later Owner change;
3. final V1.3 supersession/removal;
4. material Red-Team conflict requiring Owner decision.

Absence from a shorter V1.4 summary never means deletion.

## 2. Preserved Application topology

### Inside FSATS operational boundary

1. Falcon Trading Guardian Application.
2. Falcon Self-Aware Provider Management Application (FSAPMA).
3. Falcon Self-Aware Trading Application.

FSATS is a non-owning trading-system/domain boundary, not an Application principal.

### Independent adjacent Applications preserved from V1.3

4. Falcon Self-Aware Trading Simulator Application (FSTSimA), independent non-Live validation Application outside FSATS operational authority.
5. Falcon Web Application, independent Shared Application.
6. Falcon Communication Application, independent Shared Application.

The adjacent Applications are not absorbed into FSATS merely because they support trading workflows.

## 3. Preserved awareness topology

### FSATS

- Guardian: 1 MSA + 4 LSA rooms.
- FSAPMA: 1 MSA + 6 LSA rooms.
- Trading: 1 MSA + 12 LSA rooms.

### Independent validation Application

- FSTSimA: 1 Simulation MSA + 8 Simulation LSA rooms.

### Locality invariants

Final V1.3 already establishes and V1.4 preserves:

- CSA sees one eligible component only;
- LSA sees one declared room and child eligible CSAs only;
- MSA understands only its own Application through LSA reports/evidence;
- rank creates no cross-room/cross-Application jurisdiction;
- awareness is never a hidden integration path;
- all MSA/LSA/CSA entities remain outside synchronous operational hot paths.

Current ADR-I015 reinforces these rules.

## 4. Preserved trading baseline

Unless separately changed by Owner/governance, preserve:

- US Equities and Crypto Spot;
- 13 provider identities, with 7 initial active targets and 6 reserve/conditional/deferred;
- cash-funded 1:1 initial model;
- long-only initial authority;
- 2 trading schools;
- 10 central strategy models;
- all approved V1.3 analysis frameworks;
- dynamic effective universe/state sets rather than fixed A/B/C architecture;
- INTRADAY and SHORT_SWING implementation-eligible horizons;
- MEDIUM_TERM and LONG_TERM disabled future horizons;
- Unified Trading Risk;
- correlation/common-factor/concentration controls;
- portfolio/capital and hierarchical reservation logic;
- immutable Trading Intent and deterministic horizon/clock/continuity controls;
- execution state machine, partial fills, logical allocation, reconciliation and position lifecycle;
- Guardian protection states/playbooks/scoped containment/open-position protection/recovery;
- evidence, provenance, replay and reconstruction;
- Learning, Analytics, Attribution and governed Strategy Evolution;
- contract-first rooms, ports/adapters and vendor-semantic isolation;
- independent architecture, semantic, Red-Team and evidence gates.

## 5. Operational-data and research boundary

This is a preserved final-V1.3 rule, not a new V1.4 invention:

- operational external data used in Paper/Live decisions enters through FSAPMA;
- awareness Internet access is research/learning/improvement only;
- research output cannot directly become market truth or a trading command;
- Foundation-governed research egress is requested through canonical **FCR-0008 / Issue #8** where runtime enforcement is not yet available.

## 6. Current Foundation alignment deltas

### 6.1 Application lifecycle and manifests

Each independent Falcon Application receives its own APP-001 lifecycle, CON-023 Manifest, health/failure containment, resource declaration, permissions, communication and evidence boundaries.

### 6.2 Resource ownership

The old FSATS-wide technical resource pool/coordinator assumption is superseded.

Foundation allocates technical resources per Application. Each Application distributes only its own admitted allocation. Financial trading capital remains Application business state.

### 6.3 Cross-Application communication

V1.3 business contract semantics are preserved, but cross-Application transport uses declared Foundation-governed routes. No direct private memory/database/file shortcut is permitted.

### 6.4 Guardian resource escalation

Trading Guardian may submit an evidenced resource request for an affected Application during a broad trading threat. Foundation retains approve/deny/cap/rebalance authority. Canonical **FCR-0007 / Issue #7** carries the confirmed runtime request-boundary gap.

### 6.5 Foundation lifecycle versus trading authority

Foundation `ACTIVE` is only Application lifecycle state. It does not grant Shadow, Paper, Tiny Live or Live trading authority.

## 7. Fast Track and performance preservation

V1.4 preserves final V1.3 performance architecture:

- Trading Data Plane separation from slower Control/Awareness work;
- Independent Protection Plane;
- Fast Risk / feasibility guards;
- immutable/precomputed snapshots where safe;
- end-to-end deadline propagation without resetting timeouts at each hop;
- bounded queues and priority lanes;
- p50/p95/p99/p99.9/max tail-latency evidence;
- strategy latency classes and live-feasibility checks;
- load shedding of research/discovery/lower-priority work before protection/reconciliation/near-trade work;
- same-Application colocation where justified without breaking logical room ownership;
- final dispatch rechecks for authority, Guardian, grants, intent, freshness, Risk and feasibility;
- no safety/governance bypass for speed.

Canonical **FCR-0009 / Issue #9** records cross-Application latency/deadline/QoS requirements. Canonical **FCR-0010 / Issue #10** records resource-pressure/load-shedding visibility requirements.

## 8. Independent FSTSimA preservation

FSTSimA remains outside FSATS operational authority and must not be hidden inside the Trading Application.

Preserve:

- same Falcon logic/contracts with simulation-specific clocks and external adapters;
- eight Simulation LSA rooms;
- Replay, Synthetic Stress, Microstructure, Provider/Broker/Execution, Account/Settlement, Fault/Latency/Crisis simulation;
- fidelity levels and calibration;
- Digital City;
- truth oracle, reproducibility and provenance;
- separate credentials, networks, stores, namespaces, clocks and authority scopes;
- no Live credentials, no production mutation, no self-promotion and no self-fidelity approval.

Canonical **FCR-0011 / Issue #11** records the required Foundation enforcement for non-Live Application isolation and egress denial.

## 9. Foundation Capability Request policy

Confirmed Foundation gaps are raised through `applications/FCR_WORKFLOW.md` and repository Issue #1. Canonical identity comes from the GitHub Issue number, not a local manually allocated filename.

Current canonical submitted FCR set:

1. FCR-0004 / Issue #4 - Guardian protection command route.
2. FCR-0005 / Issue #5 - normalized operational market-data delivery.
3. FCR-0006 / Issue #6 - event/evidence/replay delivery.
4. FCR-0007 / Issue #7 - Trading Guardian to Foundation resource escalation.
5. FCR-0008 / Issue #8 - research-only Internet egress for awareness.
6. FCR-0009 / Issue #9 - latency-aware Application transport/QoS.
7. FCR-0010 / Issue #10 - Application resource-pressure/load-shedding signals.
8. FCR-0011 / Issue #11 - non-Live FSTSimA isolation and egress guard.

All are `SUBMITTED`. FCRs are design inputs only and grant no Foundation modification/runtime authority.

## 10. Part 0 acceptance rule

Part 0 can be presented for Owner acceptance only when:

- full V1.3 source-domain delta accounting is present;
- all binding final-V1.3 supersessions are protected;
- all 22 FSATS LSA rooms and 8 FSTSimA rooms are correctly mapped;
- Application identities distinguish FSATS core from adjacent FSTSimA/Web/Communication Applications;
- Manifest candidates and CON-023 completeness register are complete at design level;
- cross-Application contract matrix is corrected;
- Foundation dependency/FCR inventory is complete at design level;
- Fast Track/performance preservation is explicit;
- Foundation lifecycle and trading execution authority are separated;
- Architecture Review and Red-Team have no unresolved P0/Critical finding;
- unresolved non-P0 design decisions are surfaced to Owner rather than silently guessed.

Part 0 completion still does not authorize code. Implementation requires a separate explicit Owner authorization.

## 11. Future execution plan

The corrected future plan contains 11 Parts, with FSTSimA as its own Part rather than being embedded inside Trading learning/evolution.

## 12. Non-goals

This proposal does not authorize code, deployment, provider/broker connectivity, Paper trading, Tiny Live, Live trading, paid services, merge to Foundation, or production adoption.
