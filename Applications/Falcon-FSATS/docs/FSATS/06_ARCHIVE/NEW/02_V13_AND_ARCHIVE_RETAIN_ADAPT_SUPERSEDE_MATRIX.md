# FSATS Specialized Implementation Architecture — V1.3 / Archive Retain-Adapt-Supersede Matrix

**Package:** `FSATS-SIA-v0.1`
**Status:** `DESIGN_CANDIDATE`
**Historical Sources:** FSATS V1.3 + accepted P0 + prior P1 design candidates

## 1. Purpose

This file prevents two opposite failures:

1. discarding useful solved design knowledge merely because it is historical; and
2. re-importing historical assumptions that conflict with the current Falcon Foundation, APP-001, CON-023, ADR-I012, ADR-I015 or later Owner direction.

Classification values:

```text
RETAIN = preserve the business/design intent without semantic change
ADAPT = preserve the intent but rebind ownership/contracts/authority/implementation shape
SUPERSEDE = replace the historical realization with a stronger current design
REFERENCE_ONLY = useful historical knowledge, no current authority
REJECT = incompatible with current governing architecture
PENDING_FOUNDATION = required outcome known, exact Foundation-owned realization unresolved
```

## 2. System and Application Model

| Historical concept | Current disposition | Current realization |
|---|---|---|
| FSATS as the Trading System boundary | RETAIN | Non-owning domain/system grouping; no hidden runtime principal by implication |
| Falcon Self-Aware Trading Application | RETAIN | Independent APP-001 Application, exactly one MSA, 13 LSAs |
| FSAPMA | RETAIN | Independent provider/data Application, exactly one MSA, 6 LSAs |
| Falcon Trading Guardian Application | RETAIN | Independent protection Application, exactly one MSA, 4 LSAs |
| FSTSimA / simulation | ADAPT | Independent non-Live simulation/validation Application, exactly one MSA, 8 LSAs |
| Shared Web / Communication concepts inside Trading scope | ADAPT | FSATS defines contracts only; shared Applications remain separately governed unless explicitly placed in FSATS later |
| FSATS system-wide MSA/LSA | REJECT | FSATS boundary has no MSA/LSA; awareness belongs to actual Applications/components |
| One MSA per Application | RETAIN | Mandatory under APP-001/ADR-I015 |
| LSA locality | RETAIN + EXPAND | one LSA per qualified major branch; current total 31 |
| CSA for every component by default | REJECT | CSA optional, only for eligible intelligent components under AWR-008 |

## 3. Markets and Exposure

| Concept | Disposition | Current rule |
|---|---|---|
| US Equities initial market | RETAIN | initial active market profile |
| Crypto Spot initial market | RETAIN | initial active market profile |
| Initial funded exposure 1:1 | RETAIN | no borrowed leverage as initial business rule; broker/exchange semantics must still be explicit |
| Paper-first progression | RETAIN | validation progression only; Paper authority remains separately granted |
| Tiny Live after evidence | RETAIN WITH GOVERNANCE | must satisfy explicit Owner/runtime authority gates |
| Market-specific strategy copies | REJECT | central strategy identity + market applicability/profile binding |
| Market-specific operating profiles | RETAIN | exact sessions, precision, capabilities, liquidity, execution and risk constraints required |

## 4. Strategy and Intelligence

| Concept | Disposition | Current rule |
|---|---|---|
| Central Strategy Catalog | RETAIN | one canonical strategy registry |
| Strategy Controller | RETAIN + SPECIALIZE | deterministic applicability/filter/ensemble/proposal pipeline |
| Trading Schools | RETAIN | school is classification/portfolio of strategy families, not execution authority |
| Classical Trading School | RETAIN | exact strategy specifications required |
| Opportunity Hunting School | RETAIN | exact strategy specifications required |
| Adaptive strategy improvement | ADAPT | candidate-only experimentation through LSA/CSA/FSTSimA + governance; no self-promotion |
| Strategy Self-Awareness | ADAPT | CSA only when eligibility is proven; strategy state cannot self-expand market/risk/authority |
| Meta-Learning framework | RETAIN WITH CONTAINMENT | candidate generation/reweighting only, no production authority |
| Internal competition for capital | ADAPT | ranking signal/allocator input under Unified Risk and capital authority, not autonomous capital authority |

## 5. Risk, Capital and Portfolio

| Concept | Disposition | Current rule |
|---|---|---|
| Unified Risk Controller | RETAIN | Trading Application business authority; not Guardian and not Foundation |
| Global Capital Reservation Ledger | RETAIN + SPECIALIZE | authoritative Trading business reservation aggregate with deterministic reservation/release/reconciliation |
| Portfolio/capital allocator | RETAIN | owns business allocation within admitted capital/risk rules |
| Risk == Guardian | REJECT | Risk evaluates/controls business exposure; Guardian owns independent protection/restriction/crisis functions |
| Profit outranks survival | REJECT | Vision order is Protect > Manage > Grow |

## 6. Provider / Market Data

| Concept | Disposition | Current rule |
|---|---|---|
| FSAPMA sole operational external market/provider-data gateway | RETAIN | no Trading/Guardian direct operational provider path |
| Provider registry | RETAIN | exact provider identity/capability/entitlement/profile |
| quota-aware routing | RETAIN + SPECIALIZE | deterministic route scoring/gating + reservation/accounting + failure behavior |
| multi-provider reconciliation | RETAIN | canonical Data Product truth + quality/evidence rules |
| provider quality model | RETAIN | business quality remains FSAPMA-owned |
| direct Foundation market-data interpretation | REJECT | Foundation transport treats payload semantics as opaque |
| Application-owned direct Internet provider egress | PENDING_FOUNDATION | FCR-0013 future external egress/credential boundary; design behind fail-closed adapter seam |

## 7. Broker / Execution

| Concept | Disposition | Current rule |
|---|---|---|
| Alpaca Paper first | RETAIN AS TARGET PROFILE | Paper activation not granted by design |
| broker adapters | RETAIN | translate exact canonical execution command/state to broker protocol and reconcile results |
| direct strategy order submission | REJECT | strategies produce proposals; order authority follows decision/risk/capital/execution pipeline |
| broker response == Falcon execution truth | REJECT | broker evidence must reconcile into authoritative Application-owned execution/position state |
| direct broker credentials in Trading code/config | REJECT | governed credential references/egress through future Foundation FCR-0014 boundary |

## 8. Guardian / Protection

| Concept | Disposition | Current rule |
|---|---|---|
| independent Trading Guardian Application | RETAIN | separate Application/authority boundary |
| Guardian may restrict Trading/FSAPMA | RETAIN WITH CONTRACTS | exact protection contract, scope, expiry, authority, idempotency and evidence required |
| Guardian owns Trading Risk | REJECT | distinct responsibility |
| Guardian seizes Foundation resources directly | REJECT | resource effects go through governed FSARM/Foundation resource chain |
| Guardian crisis resource priority evidence | RETAIN | may submit consequence/minimum-safe evidence to FSARM |
| Kill/recovery semantics | ADAPT | Application Guardian semantics separated from Foundation/FSA controls; exact target/scope/authority required |

## 9. FSARM / Resources

| Concept | Disposition | Current rule |
|---|---|---|
| FSATS-wide resource coordinator | RETAIN | FSARM is bounded delegated aggregate coordinator |
| T-LSA-13 as system-wide resource controller | SUPERSEDE | T-LSA-13 is Trading-side resource awareness/evaluation only |
| internal redistribution first | RETAIN | primary FSARM rule |
| Foundation additional request second | RETAIN | only remaining proven deficit is requested |
| request equals grant | REJECT | `REQUESTED_RESOURCE != GRANTED_RESOURCE` |
| FSARM owns total-resource truth | REJECT | Foundation owns total-resource truth/grants/ceilings |
| FSARM opaque pooled capacity with lost Application identity | REJECT | per-Application attribution/accounting/isolation remains mandatory |
| Falcon-wide FSARM above all domains | REFERENCE_ONLY | future backlog only; no current design impact |

## 10. Simulation / Validation

| Concept | Disposition | Current rule |
|---|---|---|
| Simulator / Shadow / Replay | RETAIN + SPECIALIZE | independent FSTSimA with non-Live classification and reproducibility |
| simulation event treated as Live | REJECT | explicit authoritative/non-authoritative classification |
| synthetic markets | RETAIN | generated scenarios with exact provenance and deterministic seeds |
| fidelity calibration | RETAIN | S-LSA-07 |
| independent validation assessment | RETAIN + SEPARATE | S-LSA-08 cannot be collapsed into the simulator being assessed |
| FSTSimA resource reclaimability | RETAIN + SPECIALIZE | deferrable simulation work may be shed/reclaimed without corrupting frozen evidence |
| direct Live egress from FSTSimA | REJECT / PENDING_FOUNDATION | FCR-0011 non-Live isolation boundary remains future capability |

## 11. Awareness / AI

| Concept | Disposition | Current rule |
|---|---|---|
| CSA -> LSA -> MSA -> FSA | RETAIN WITH ORIGIN RULE | route begins at actual proposal origin; lower tiers are not artificially inserted |
| MSA owns complete Application awareness | RETAIN | business/domain quality remains Application-owned |
| FSA owns OS governance compatibility review | RETAIN | Foundation-owned; not business evaluator |
| Awareness rank creates authority | REJECT | rank != jurisdiction != authority |
| self-development | RETAIN WITH CONTAINMENT | observe/research/propose/candidate/test/evidence only under authority; no self-deploy |
| direct Trading MSA Internet research | SUPERSEDE | current accepted direction routes research through governed non-Live/FSTSimA mechanism when capability exists |
| FSA direct Internet | REJECT | explicitly forbidden in current Owner handoff |
| 24-hour Owner silence fallback | REFERENCE_ONLY / UNAUTHORIZED | cannot create authority until separate Foundation/governance reconciliation and Owner decision |
| two Monitor AI perspectives per FSATS MSA | RETAIN AS ACCEPTED APPLICATION DIRECTION | bounded oversight, not awareness tier or autonomous authority |
| monitor disagreement by majority vote | REJECT | material disagreement triggers integrity check |

## 12. Communication / Contracts

| Concept | Disposition | Current rule |
|---|---|---|
| contract-first communication | RETAIN | exact schema/producer/consumer/authority/failure/evidence required |
| direct Application-to-Application internals | REJECT | ADR-I012 prohibits hidden coupling |
| route existence creates authority | REJECT | authority is explicit and independent |
| idempotency/correlation/causation/provenance | RETAIN + NORMALIZE | use accepted Foundation transport primitives where Foundation-owned; Application owns payload semantics |
| replay/test/live classification | RETAIN | explicit in every relevant contract/event |

## 13. Persistence and Evidence

| Concept | Disposition | Current rule |
|---|---|---|
| immutable causal/audit provenance | RETAIN + EXPAND | decision/evidence graph with exact identities, state versions and outcomes |
| history rewrite for cleanup | REJECT | append correction/supersession; preserve historical truth |
| generic single database for all Applications | REJECT | Application state ownership and isolation mandatory |
| coding worker chooses transaction semantics | REJECT | authoritative aggregates receive exact consistency strategy in this SIA |

## 14. V1.3 Validation Structure To Preserve As A Quality Pattern

The current package SHALL reproduce or exceed the V1.3 quality pattern with distinct proof surfaces for:

```text
PACKAGE STRUCTURE
SEMANTICS
SCHEMAS
TRACEABILITY
BASELINE PRESERVATION
STATE MACHINES
STRUCTURAL RULES
RED-TEAM
```

It SHALL additionally adopt the stronger Foundation pattern of:

```text
EXACT SEMANTIC FREEZE
+ ARCHITECTURE / CONSISTENCY REVIEW
+ SECURITY / AUTHORITY REVIEW
+ DETERMINISTIC VERIFIERS
+ NEGATIVE / ADVERSARIAL FIXTURES
+ EXACT ARTIFACT IDENTITY
+ OWNER GATE
```

## 15. Supersession Rule

This matrix does not rewrite the historical design.

When a current implementation decision differs from V1.3/P0/P1, the difference SHALL be traceable to:

- higher/current authority;
- later Owner direction;
- stronger failure/safety requirement;
- current Foundation boundary;
- proven implementation ambiguity;
- or a documented superior design that preserves the same intent without violating Falcon authority.

Unexplained semantic drift is forbidden.
