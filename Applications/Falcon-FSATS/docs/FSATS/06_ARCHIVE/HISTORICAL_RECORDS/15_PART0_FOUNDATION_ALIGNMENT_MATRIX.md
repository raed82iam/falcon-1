# FSATS V1.4 Part 0 - Foundation Alignment Matrix

**Status:** `PART 0 TARGETED SEMANTIC REVIEW COMPLETE / OWNER REVIEW REQUIRED`  
**Authority:** design only

## Foundation authority snapshot used for this pass

Part 0 reviewed the current `foundation-development` authority at commit:

`0b8dedbd9a45f1f0ef1aa12af587c57271748d6c`

Primary governing documents:

- APP-001 v1.1 - Application Boundary and Lifecycle;
- CON-023 v1.1 - Application Contract and Manifest;
- ADR-I012 v1.1 - Foundation Plug-and-Play Application Integration Boundary;
- ADR-I015 v1.0 - Falcon OS Application and Awareness Alignment;
- SYS-006 v1.1 - Multi-Level Resource Governance.

If these governing semantics materially change before implementation authorization, the alignment must be revalidated.

## Semantic alignment decisions

| ID | Final V1.3 assumption / requirement | Part 0 disposition | Current V1.4 alignment |
|---|---|---|---|
| FA-001 | FSATS contains Guardian, FSAPMA and Trading | `PRESERVE` | preserved as trading-system boundary containing three independent Applications |
| FA-002 | FSATS is a system boundary, not a fourth Application | `PRESERVE + CONFIRM` | fully consistent with APP-001/ADR-I012; FSATS owns no lifecycle/MSA/allocation/credentials |
| FA-003 | one MSA per main Application, one LSA per major branch, CSA per eligible component | `PRESERVE + ALIGN` | preserved and bound to ADR-I015/APP-001/CON-023 |
| FA-004 | Guardian 4 LSA, FSAPMA 6 LSA, Trading 12 LSA | `PRESERVE` | exact room counts/names preserved; stale 2+3+7 proposal removed |
| FA-005 | MSA/LSA/CSA locality and no SA hot-path ownership | `PRESERVE` | promoted to explicit V1.4 invariant; awareness is not an integration path |
| FA-006 | operational external data for Paper/Live enters through FSAPMA; SA Internet is research only | `PRESERVE + FOUNDATION ALIGN` | preserved as V1.3 rule; canonical FCR-0008 / Issue #8 requests generic governed research egress enforcement |
| FA-007 | Guardian may issue scoped binding protection commands to Trading/FSAPMA | `PRESERVE + ALIGN` | business authority preserved; transport must use declared current-Foundation routes; canonical FCR-0004 / Issue #4 and FCR-0009 / Issue #9 apply |
| FA-008 | Guardian may request Foundation health/resources but cannot command Foundation | `PRESERVE + ALIGN` | preserved; request targets current Foundation-owned Guardian/resource authority under SYS-006; canonical FCR-0007 / Issue #7 and FCR-0010 / Issue #10 apply |
| FA-009 | FSATS resource coordinator / FSATS-wide technical resource priority | `SUPERSEDE` | Foundation allocates resources per Application; no FSATS technical pool; each Application sheds within its own grant |
| FA-010 | Trading capital/global exposure/reservation logic | `PRESERVE` | remains Trading business state; explicitly separated from Foundation CPU/RAM/network/storage resources |
| FA-011 | Foundation lifecycle/services are consumed through contracts | `PRESERVE + ALIGN` | APP-001/CON-023/ADR-I012 are governing boundaries; runtime capability availability must be classified, not assumed |
| FA-012 | V1.3 assumes Service Bus/FIL/storage/etc. as existing runtime services | `ALIGN` | design semantics may be consumed where approved, but unavailable future Stage-5 runtime behavior is not assumed; no local substitute |
| FA-013 | V1.3 FSA core-rule compliance verdict/interface | `ALIGN` | current FSA performs OS-governance/compatibility review only; FSA does not grant implementation/deployment/production adoption or judge trading quality |
| FA-014 | MSA/LSA/CSA development path passes through FSA before production adoption | `PRESERVE + ALIGN` | origin-aware CSA->LSA->MSA->FSA / LSA->MSA->FSA / MSA->FSA, followed by separately authorized Owner/governance adoption |
| FA-015 | Guardian crisis states apply to trading system scope | `PRESERVE WITH OWNER STRENGTHENING` | smallest-safe-scope user/account containment first; broad system state only for broad evidenced threat |
| FA-016 | immutable provenance ledger owned by an `FSATS shared provenance service boundary` | `SUPERSEDE OWNERSHIP / PRESERVE SEMANTICS` | FSATS cannot own a shared runtime service. Each Application owns its business decision/evidence state; generic cross-Application journal/evidence transport is represented by canonical FCR-0006 / Issue #6 |
| FA-017 | FSTSimA independent experimentation Application outside FSATS operational authority | `PRESERVE + ALIGN` | preserved as independent APP-001 Application with own MSA, 8 LSAs, lifecycle, allocation, permissions and routes |
| FA-018 | experimentation environment listed as a Foundation service dependency in some older V1.3 integration text | `SUPERSEDE OWNERSHIP` | FSTSimA is not a Foundation service; Foundation only hosts/governs it as an independent Application |
| FA-019 | FSTSimA separate credentials/networks/stores/namespaces/clocks and no Live access | `PRESERVE + FOUNDATION ENFORCEMENT INPUT` | preserved; canonical FCR-0011 / Issue #11 requests enforceable non-Live egress/credential/route isolation |
| FA-020 | Web and Communication outside FSATS | `PRESERVE + ALIGN EXTERNALLY` | remain independent Shared Applications; FSATS owns contracts only, not auth/session/channel delivery internals |
| FA-021 | V1.3 cross-room/versioned contracts | `PRESERVE` | internal Application contracts remain Application-owned; cross-Application routes additionally require ADR-I012 admitted Foundation routing |
| FA-022 | V1.3 34 Integration Contracts | `PRESERVE BUSINESS SEMANTICS + REBIND` | business schemas/errors/ACK-NACK/time/evidence remain migration inputs; obsolete Foundation/FSA/resource route assumptions are aligned to current authority and canonical FCRs |
| FA-023 | Trading runtime deployment/colocation/load shedding | `PRESERVE + ALIGN` | colocation permitted only inside same Application where justified; cross-Application boundaries cannot be fused for latency; FCR-0009 / Issue #9 and FCR-0010 / Issue #10 cover confirmed external gaps |
| FA-024 | Trading security and secrets | `PRESERVE REQUIREMENT + ALIGN OWNERSHIP` | Application declares permissions/security needs; Foundation owns generic security/secret/permission enforcement; vendor credentials remain scoped and never shared across Applications by convenience |
| FA-025 | HA/disaster recovery | `PRESERVE BUSINESS RECOVERY + ALIGN PLATFORM SUPPORT` | internal business recovery remains Application-owned; Foundation owns Application lifecycle/isolation/resource/recovery support |
| FA-026 | implementation module/port map | `PRESERVE + REBIND` | module responsibility retained unless ownership contradicts current Application boundaries; no direct Application internals or old FSATS shared-service owner |
| FA-027 | V1.3 architecture/code-ready status | `PRESERVE PROVENANCE ONLY` | historical evidence only; current V1.4 must pass its own Owner review and separate implementation authorization |
| FA-028 | Paper/Tiny Live/Live not granted by V1.3 readiness | `PRESERVE + APP-001 ALIGN` | Foundation ACTIVE is orthogonal to trading authority; each trading stage has independent evidence/Owner gate |
| FA-029 | Fast Track/hot path/deadline/tail SLO/load shedding | `PRESERVE` | mandatory V1.4 target; never bypass Risk/Guardian/authority/evidence/reconciliation; canonical FCR-0009 / Issue #9 and FCR-0010 / Issue #10 preserve cross-boundary requirements |
| FA-030 | Provider/broker SDK semantics behind adapters | `PRESERVE` | provider semantics remain FSAPMA-owned; execution adapter semantics remain owned boundary; no vendor leakage into strategy/risk logic |

## Binding corrections produced by this matrix

Part 0 treats these as mandatory corrections to the earlier V1.4 draft:

1. remove all stale 2+3+7 LSA references;
2. preserve FSTSimA as independent non-Live Application;
3. remove any statement that Trading Application owns the simulator itself;
4. classify research-only Internet separation as preserved V1.3 rule, not a newly invented policy;
5. remove FSATS-wide technical resource coordinator/pool ownership;
6. remove `FSATS shared provenance service` ownership while preserving immutable provenance semantics;
7. replace old FSA `core-rule verdict/approval` wording with current OS-governance/compatibility review semantics;
8. stop treating FSTSimA/experimentation environment as a Foundation-owned service;
9. preserve Fast Track without cross-Application private shortcuts;
10. preserve current Foundation lifecycle/trading-authority separation;
11. use repository Issue-derived canonical FCR identities only.

## Targeted semantic review result

The Foundation-facing ALIGN families in the current Part 0 package are dispositioned at design level. Exact runtime bindings remain pending where the corresponding Foundation capability/contract is not yet available or has not yet been confirmed.

The final FCR gap scan is recorded separately in `17_PART0_FINAL_FCR_GAP_SCAN.md` and found no additional confirmed Foundation gap beyond canonical FCR-0004 through FCR-0011.

## Remaining before Part 0 closure

- final consistency scan across all current V1.4 proposal files;
- final Architecture Review;
- final Red-Team rerun;
- Owner review gate.

No item in this matrix authorizes implementation or runtime activity.
