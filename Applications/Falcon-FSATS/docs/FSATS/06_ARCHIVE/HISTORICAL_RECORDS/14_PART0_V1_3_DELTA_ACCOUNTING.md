# FSATS V1.4 Part 0 - V1.3 Delta Accounting

**Status:** `PART 0 WORKING BASELINE / OWNER REVIEW REQUIRED`  
**Authority:** design-only alignment work  
**Source package:** owner-provided final FSATS V1.3 ZIP  
**Source inventory:** 289 ZIP entries, 273 files  
**Source package integrity:** matches the recorded V1.3 package checksum/inventory previously validated.

## 1. Governing rule

V1.3 is the architecture migration baseline. V1.4 is not allowed to delete a V1.3 capability merely because it is absent from a shorter proposal document.

Every V1.3 domain is dispositioned as one of:

- `PRESERVE` - keep the final V1.3 business/safety/performance intent;
- `ALIGN` - keep the intent but rebind ownership, lifecycle, resources, permissions, manifests, routing, security or other platform assumptions to current Foundation;
- `SUPERSEDE` - do not carry forward a structure already removed by final V1.3 or explicitly replaced by current Owner/Foundation authority;
- `OWNER_DECISION` - explicit Owner decision required before V1.4 closure.

## 2. Full package coverage

The 273 files are distributed across these source domains and all are included in Part 0 accounting:

| V1.3 source domain | Files | Part 0 default treatment |
|---|---:|---|
| FSATS Governance | 31 | preserve trading governance; align Foundation-facing ownership/authority references |
| FSATS core system and three core Applications | 86 | preserve mature trading capabilities; align manifests/routes/resources/security/lifecycle |
| Shared Web/Communication Applications | 25 | preserve as independent external Application dependencies; do not absorb into FSATS |
| Integration Contracts | 34 | preserve business semantics; rebind cross-Application transport and authority to current Foundation |
| Implementation and Verification | 48 | preserve build/test/evidence intent and provenance; regenerate current-Falcon expectations |
| Experimentation and Validation Environment | 17 | preserve FSTSimA as independent non-Live Application and Digital City capability |
| Machine-readable baseline/schemas/examples | 26 | preserve canonical business baseline; align schema registration/versioning where needed |
| Package-level summary/checksum/closure files | 6 | preserve as historical source/evidence only |
| **Total** | **273** | complete source-domain coverage |

ZIP directory entries make the package total 289 entries.

## 3. Binding final-V1.3 supersessions

Part 0 SHALL NOT revive the following superseded structures:

- platform-wide Trading Guardian authority;
- FSAPMA outside the FSATS trading-system grouping;
- Guardian provider isolation treated only as an informal request when binding scoped protection authority is required;
- assumed future Falcon-wide financial service as the owner of trading financial truth;
- one fungible global-capital number without account/custodian/currency constraints;
- fixed A/B/C or static Top-N universe as architecture;
- price as a standalone selection gate;
- one-strategy-first architecture;
- syntactic package PASS treated as architecture/activation acceptance;
- outline-only contracts without version/error/ACK-NACK/time/evidence semantics;
- any eleven-room Trading topology;
- any hot path without immutable Intent Classification and applicable exposure/protective feasibility checks;
- any rule requiring positive expected profit for emergency protective execution;
- blind broker retry after an UNKNOWN submission outcome;
- fictional guaranteed close when the market/broker cannot execute.

## 4. Current-Foundation alignment deltas

The following are genuine V1.4 alignment changes rather than redesigns of trading logic:

### D-001 - FSATS system identity

`PRESERVE + ALIGN`.

FSATS remains a trading-system/domain boundary. Under current APP-001/ADR-I012 it is not itself an Application principal and owns no MSA, Foundation lifecycle, Foundation allocation, credentials, hidden state or platform authority.

### D-002 - Per-Application resource governance

`ALIGN`.

The old FSATS-level technical resource coordinator/pool assumption is superseded. Foundation allocates Guardian, FSAPMA, Trading, FSTSimA, Web and Communication independently when each is admitted. An Application redistributes only its own admitted allocation.

Trading capital remains business state and is not Foundation technical-resource authority.

### D-003 - Cross-Application communication

`ALIGN`.

V1.3 business contract semantics are preserved, but direct private coupling is prohibited. Application-to-Application transport requires declared contracts and admitted Foundation routes. Planned-but-unavailable transport requirements are represented through FCRs.

### D-004 - Awareness hierarchy and room locality

`PRESERVE + ALIGN`.

Final V1.3 already contains the current core locality model:

- one MSA per Application;
- one LSA per major branch;
- optional CSA for one eligible intelligent component;
- CSA sees one component;
- LSA sees one room and child CSAs;
- MSA understands only its Application through declared LSA reports/evidence;
- rank creates no cross-room/cross-Application jurisdiction;
- awareness is not an integration path;
- MSA/LSA/CSA remain outside the synchronous hot path.

Current ADR-I015 reinforces this model.

### D-005 - Research Internet versus operational trading data

`PRESERVE + FOUNDATION ALIGNMENT`.

This separation is already explicit in final V1.3:

- operational data used for Paper/Live decisions enters through FSAPMA;
- awareness Internet access is for research, learning and improvement only.

V1.4 therefore preserves this rule and raises Foundation requirements only for governed research egress/security enforcement.

### D-006 - Guardian scope and resource escalation

`PRESERVE + ALIGN`.

Guardian remains trading-scoped. Binding trading-protection commands stay scoped, attributable and expiring. The current Owner rule strengthens incident containment to the smallest safe user/account/market/system scope.

Guardian may request additional technical resources during a broad evidenced trading threat, but current Foundation owns approval, denial, ceilings and redistribution.

### D-007 - Foundation lifecycle versus trading authority

`PRESERVE + ALIGN`.

V1.3 already separates architecture/code readiness from Paper/Tiny Live/Live authority. V1.4 additionally binds this to APP-001: Foundation `ACTIVE` is only Application lifecycle state and never grants Paper/Tiny Live/Live business authority.

### D-008 - FSTSimA boundary

`PRESERVE + ALIGN`.

Final V1.3 defines the Falcon Self-Aware Trading Simulator Application (`FSTSimA`) as an independent non-Live Application outside FSATS operational authority.

It SHALL NOT be absorbed into Trading Application merely for implementation convenience.

Its preserved eight LSA rooms are:

1. Simulation Time and Scenario.
2. Market Environment Simulation.
3. Provider and External Service Simulation.
4. Broker, Exchange, and Execution Simulation.
5. Account, Capital, and Settlement Simulation.
6. Fault, Latency, and Crisis Injection.
7. Fidelity and Calibration.
8. Evidence, Oracle, and Reproducibility.

FSTSimA uses the same Falcon business logic/contracts with simulation-specific clocks and external adapters, while remaining technically separated from Live credentials/state/authority.

### D-009 - Shared Web and Communication Applications

`PRESERVE + ALIGN EXTERNALLY`.

Both remain independent Shared Falcon Applications outside FSATS. FSATS consumes presentation, command and notification capabilities through governed contracts only.

### D-010 - Fast Track / latency architecture

`PRESERVE`.

Part 0 confirms that the V1.3 performance architecture is mandatory target design, including:

- Trading Data Plane versus Control/Awareness Plane versus Independent Protection Plane;
- deadline propagation without timeout reset at each hop;
- immutable/precomputed snapshots where safe;
- Fast Risk / feasibility guards;
- bounded priority queues;
- tail-latency SLOs including p99/p99.9;
- strategy latency classes;
- load shedding that removes research/discovery/lower-priority work before protection, reconciliation and near-trade work;
- same-Application colocation where justified without breaking logical room ownership;
- no Risk/Guardian/authority/evidence/reconciliation bypass for speed.

## 5. Correct Application and awareness inventory

### Inside FSATS operational boundary

- Guardian: 1 MSA + 4 LSA rooms.
- FSAPMA: 1 MSA + 6 LSA rooms.
- Trading: 1 MSA + 12 LSA rooms.

Total inside FSATS: 3 Applications, 3 MSAs, 22 LSA rooms.

### Adjacent independent Applications preserved from V1.3

- FSTSimA: 1 MSA + 8 LSA rooms, outside FSATS operational authority.
- Falcon Web Application: independent Shared Application.
- Falcon Communication Application: independent Shared Application.

## 6. Machine-readable baseline preservation

Part 0 preserves the final V1.3 fixed initial trading baseline unless a later explicit Owner decision changes it:

- markets: 2 - US Equities and Crypto Spot;
- providers: 13;
- initial active-target providers: 7;
- reserve/conditional/deferred providers: 6;
- trading schools: 2 - Classical and Opportunity Hunting;
- strategy models: 10;
- Trading LSA rooms: 12;
- funding: cash-funded 1:1;
- initial direction: long-only;
- leverage/margin/short/options/futures/perpetuals: disabled in the initial authority;
- implementation-eligible horizons: INTRADAY and SHORT_SWING;
- MEDIUM_TERM and LONG_TERM: disabled future horizons until separately activated.

## 7. Part 0 semantic review priority

The highest-priority `ALIGN` families are:

1. package/Application manifests;
2. Foundation/FSA integration assumptions;
3. technical resource preemption/allocation language;
4. cross-Application route/Service Bus assumptions;
5. security/credential/egress ownership;
6. runtime deployment/HA/isolation assumptions;
7. Shared Application boundaries;
8. FSTSimA non-Live isolation;
9. implementation blueprints that encode obsolete package ownership;
10. integration contracts that name an old Foundation service or authority path.

## 8. FCR rule during Part 0

Whenever a preserved V1.3 requirement needs a generic Foundation capability that is missing, incompatible, planned but unavailable, or not yet confirmed, Part 0 creates or updates an FCR. FSATS SHALL NOT weaken the preserved requirement merely to avoid a Foundation dependency.

## 9. Current Part 0 status

**Complete at source-domain accounting level:** all 273 V1.3 files are covered by a declared migration family and no V1.3 domain is silently excluded.

**Still required before Part 0 closure:** targeted semantic resolution of every Foundation-facing `ALIGN` family, corrected contract matrix, FCR closure inventory, final manifest completeness review, revised work-Part sequence preserving FSTSimA independence, final Architecture Review, final Red-Team and Owner review gate.

This document does not authorize implementation.