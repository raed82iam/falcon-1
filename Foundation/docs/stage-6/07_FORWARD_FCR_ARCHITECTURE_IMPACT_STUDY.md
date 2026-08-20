# Stage 6 Forward FCR Architecture Impact Study

Status: PLANNING STUDY / NO IMPLEMENTATION AUTHORITY
Date: 2026-08-08
Branch: foundation-development
Scope reviewed: canonical open FCRs FCR-0004 through FCR-0014, their available reconciliation comments, current Stage 0-5 accepted baseline, SYS-006, APP-001, ADR-I012, ADR-I015, and current Application-side dependency/contract records.

## 1. Purpose

This study exists to prevent late architectural patching. It does not ask whether an FCR is merely open. It asks:

1. what generic Foundation capability family actually owns the need;
2. whether the Foundation portion is already implemented by accepted Stage 5 work;
3. whether Stage 6 must implement it, prepare a prerequisite for it, or keep it out of Stage 6;
4. what future prerequisite must exist before Application runtime integration or FCR closure;
5. whether any request is currently hidden inside comments rather than represented as its own canonical FCR.

No future Stage number is assigned here unless already governed elsewhere. Future needs are grouped by capability family so later Stage design can place them deliberately rather than inherit accidental numbering.

## 2. Executive conclusion

The open FCR set is not eleven unrelated future features. It resolves into six architectural capability families:

A. Stage-5 communication capability verification and Application binding: FCR-0004, FCR-0005, FCR-0006.
B. Stage-6 resource governance: FCR-0007, FCR-0010, plus the resource-priority dependency portion of FCR-0009.
C. Transport QoS and observability: remaining FCR-0009.
D. Governed external access / egress / credential-reference security: FCR-0008, FCR-0011, FCR-0013, FCR-0014.
E. FSA / Owner governance and bounded autonomous evolution control plane: FCR-0012.
F. Foundation artifact publication and canonical Application build-time consumption: hidden cross-cutting gap raised in FCR-0004 comments; it is NOT the protection-route request and must not remain buried there.

This decomposition materially reduces duplication risk.

## 3. FCR-by-FCR disposition

### FCR-0004 — Guardian governed protection command route

Current architectural reading:
- The original generic transport need overlaps accepted Stage 5 message admission, governed routing, delivery, event/evidence and cryptographic protection.
- Foundation must not implement Trading protection semantics such as halt/exposure-reduction/no-new-order as Foundation business logic.
- The Application contract may declare those business semantics while Foundation preserves identity, authority binding, delivery isolation, expiry, idempotency/replay context and evidence as generic infrastructure semantics.

Correct placement:
- Do NOT create a new Stage-6 protection-route subsystem.
- Perform a post-Stage-5 compatibility/application-verification exercise against the accepted generic Stage-5 surfaces.
- If a concrete generic Foundation gap remains after Application binding, raise that exact generic gap rather than a Trading-specific route implementation.

Hidden dependency discovered in comments:
- Application requested a canonical build/package consumption mechanism for accepted Foundation capabilities across `foundation-development` and `application-development` without source copy, branch merge, stale binary or local fork.
- Foundation previously documented `WP03_CANONICAL_CROSS_BRANCH_BUILD_CONSUMPTION = NOT_YET_APPROVED`.
- This need is NOT FCR-0004 runtime routing and must be promoted into its own canonical FCR/capability planning item.

### FCR-0005 — FSAPMA operational market-data delivery contract

Current architectural reading:
- Stage 5 WP-05 verified the generic producer/consumer routing and isolation portion.
- Stage 5 WP-06 implemented generic delivery, retry/idempotency, bounded failure/degradation, flow-control and truthful transport outcome behavior.
- Schema/version compatibility is predecessor-governed; market-data quality/confidence meaning remains Application-owned.

Correct placement:
- No new Stage-6 market-data delivery implementation.
- Foundation-side generic transport appears substantially implemented by Stage 5.
- Remaining work is exact Application contract binding/verification and confirmation of any residual gap.
- Freshness/quality/provenance fields that are business/data-product semantics belong in the Application contract/schema, not in a market-data special case inside Foundation.

### FCR-0006 — event evidence and replay delivery

Current architectural reading:
- Stage 5 WP-05 provides attributable route isolation/context.
- WP-06 provides delivery lineage, bounded retry/idempotency, ordering declarations and correlation/causation preservation.
- WP-07 provides event truth/publication, replay/test/simulation versus authoritative-operational classification, replay non-authority, corrections and event evidence semantics.

Correct placement:
- No new Stage-6 event/replay subsystem.
- Treat as post-Stage-5 Application verification/reconciliation unless a concrete requirement remains unimplemented.
- Live action authority must never be recreated from replay/test evidence.

### FCR-0007 — Trading Guardian resource escalation request

Current architectural reading:
- Direct Stage-6 requirement under SYS-006.
- Stage 5 explicitly deferred request semantics/allocation decision authority to the Resource Governance owner.

Correct Stage-6 placement:
- WP-01: request/decision identity primitives.
- WP-03: affected Application allocation/ceiling isolation envelope.
- WP-04: Application priority class and technical criticality/authority basis.
- WP-06: canonical additional-resource request and Foundation allow/cap/deny/defer decision boundary.
- WP-07: rebalance/temporary grant/restoration/release semantics.
- WP-08: attributable decision/result projection back to the Application.

Required clarification already requested from Application:
- which Application-level principals may submit ordinary resource requests;
- which principal(s) may submit emergency escalation requests;
- exact resource-request message families and required evidence;
- degradation/restoration contract and internal Trading priority handoff.

### FCR-0008 — awareness research-only Internet egress

Current architectural reading:
- This is not a resource-governance feature and not Stage-6 implementation scope.
- It belongs to the same generic external-access security family as provider and broker egress, but uses a distinct purpose/authority role.

Correct capability family:
Governed External Access / Egress / Credential-Reference Security.

Reusable generic primitives should include:
- Application/principal/service-role identity;
- purpose and environment classification;
- destination/service policy;
- permission and revocation;
- credential-reference isolation where credentials are applicable;
- audit/session/denial evidence;
- fail-closed missing/stale/revoked/ambiguous context.

Research role rule:
- research/learning/development only;
- no operational market/provider truth;
- no direct live business action authority;
- no inheritance from operational provider or broker-execution authority.

### FCR-0009 — latency deadline and QoS-aware transport

Current architectural reading:
- Stage 5 implemented substantial prerequisites: expiry/deadline preservation where already governed, bounded flow control, defer/degradation behavior, governed technical traffic class, truthful delivery outcome evidence.
- Stage 5 explicitly left tail-latency aggregation/observability and latency SLO/guarantee behavior outside WP-06.
- Stage 6 owns the resource-priority/pressure truth that a future QoS implementation may consume.

Correct placement split:
- Stage 6 DOES own prerequisites: technical priority authority, pressure truth, allocation ceilings/reserves, rebalance evidence, and consumer-safe resource state.
- Stage 6 MUST NOT implement queue scheduling, transport bandwidth reservation, latency SLO guarantees or tail-latency observability merely to close FCR-0009.
- Remaining need belongs to a future Transport QoS + Observability capability family, likely consuming SYS-006 truth and Stage-5 delivery surfaces.

Stage-6 design impact:
- WP-04 technical priority records must be generic, attributable and consumable by transport without allowing Application self-declared Foundation criticality.
- WP-05 pressure truth and WP-08 Application projection must be stable enough for future QoS consumers.
- WP-09 must verify compatibility between Stage-6 resource truth and accepted Stage-5 WP-06 delivery pressure/technical-class consumption.

### FCR-0010 — resource pressure and load-shedding signals

Current architectural reading:
- Direct Stage-6 requirement.
- Stage 5 WP-06 already consumes Foundation-governed delivery pressure truth but does not provide the general Application-facing allocation/pressure/request-result interface or global allocation engine.

Correct Stage-6 placement:
- WP-02 total resource truth / floors / reserves.
- WP-03 allocation/quota/ceiling/isolation.
- WP-05 pressure and enforcement-state truth.
- WP-07 redistribution/restoration.
- WP-08 per-Application projection for Application-owned load shedding.

Boundary:
- Foundation says what allocation/ceiling/pressure/restoration state is true.
- Application decides which of its own business features/components to shed inside its jurisdiction.

### FCR-0011 — FSTSimA non-Live isolation and egress guard

Current architectural reading:
- Stage 5 event classification supports replay/test/non-authoritative truth separation but does NOT provide credential acquisition denial, endpoint/egress isolation or non-Live security-profile enforcement.
- This is not Stage-6 resource governance.

Correct capability family:
Governed External Access / Egress / Credential-Reference Security, with a generic environment/authority isolation policy.

Required generic behavior:
- declared environment class (non-Live/test/simulation vs operational/live);
- deny acquisition/use of incompatible credential or service roles;
- deny incompatible external destination/service roles;
- preserve replay/test inputs through separately authorized paths;
- auditable fail-closed denial.

Do not build an FSTSimA-special firewall in Foundation.

### FCR-0012 — FSA Owner governance and bounded autonomous evolution control plane

Current architectural reading:
- Separate major governance capability family.
- Not Stage-6 resource governance.
- Needs authenticated/replay-resistant Owner commands, durable review package, trusted order/time evidence, exact delegation validation, final governance revalidation, journal, suspension/revocation/recovery coordination and strict CSA/LSA/MSA/FSA jurisdiction separation.

Correct capability family:
FSA / Owner Governance and Bounded Evolution Control Plane.

Dependencies to plan deliberately:
- accepted Authority Engine semantics;
- lifecycle/rollback evidence;
- trustworthy audit/order/time evidence;
- direct Owner governance interface;
- Application proposal evidence from completed CSA/LSA/MSA chain;
- protected control and revocation checks.

Hard boundary:
- FSA never becomes Trading/Risk/strategy/market/order/provider/broker business authority.
- Owner silence/timer expiry never equals approval and never creates authority.

### FCR-0013 — FSAPMA operational-provider egress / credential reference

Correct capability family:
Governed External Access / Egress / Credential-Reference Security.

Role:
Operational data-provider service role.

Must share generic machinery with FCR-0008/0011/0014 while preserving independent authority:
- exact Application/service-role/destination/environment/purpose;
- credential reference and secret isolation;
- allow/deny/revocation;
- session/evidence;
- provider role cannot imply broker-execution role;
- operational provider role cannot imply awareness research role.

No provider-specific market-data logic belongs in Foundation.

### FCR-0014 — Trading broker execution egress / credential reference

Correct capability family:
Governed External Access / Egress / Credential-Reference Security.

Role:
Broker-execution service role.

Specific generic bindings required in addition to shared egress primitives:
- exact Application/user/account/environment/service-role/destination/purpose identity;
- independent broker-execution authority even when the same vendor or credential source also serves market data;
- request/response/denial/session evidence;
- fail closed on credential/account/destination/authority mismatch.

No order strategy, Risk decision, instrument selection or broker-specific Trading business logic belongs in Foundation.

## 4. Cross-FCR dependency graph

### Family A — accepted Stage-5 communication + Application verification
FCR-0004 -> Stage-5 admission/routing/delivery/security compatibility + Application contract binding.
FCR-0005 -> Stage-5 routing/delivery + Application data-contract binding.
FCR-0006 -> Stage-5 routing/delivery/event/replay + Application verification.

These should be reconciled before inventing any new communication subsystem.

### Family B — Stage-6 Resource Governance
FCR-0007 -> direct.
FCR-0010 -> direct.
FCR-0009 -> consumes Stage-6 technical priority/pressure/allocation truth but is not fully owned by Stage 6.

### Family C — Transport QoS + Observability
Residual FCR-0009 after Stage-5/Stage-6 prerequisites.

Needs future explicit design for tail latency, scheduling/service policy and observability. Must consume rather than duplicate Stage-6 resource truth.

### Family D — External Access / Egress / Credential Governance
FCR-0008 + FCR-0011 + FCR-0013 + FCR-0014.

Design once as a generic framework with separate authority roles. Never implement four disconnected stacks.

### Family E — FSA / Owner Governance + Bounded Evolution
FCR-0012.

Keep separate from Resource Governance and from Trading business self-awareness.

### Family F — Foundation Artifact Publication / Application Consumption
Hidden request in FCR-0004 comments.

This needs its own canonical request and future Foundation design. It is a prerequisite for clean compile-time consumption and verification of accepted Foundation contracts by separated Application workstreams.

Potential governing domains to evaluate when designed: PIPE-001, PLG-001 and related replaceable-capability/supply-chain governance. This study does not activate proposed specifications or choose an implementation mechanism.

## 5. Stage-6 design changes required now

The ten-WP Stage-6 structure remains valid; no WP count change is required.

Required hardening:

1. WP-01 shall define resource-governance identities/contracts in a way that is independent of any one Application and can later be published through a separately governed artifact-consumption mechanism.
2. WP-04 shall support an Owner/governance-approved Application priority class as an exact policy input. Trading-related Applications currently carry the highest Owner-designated Application priority, while Foundation survival/protection/recovery floors remain non-reclaimable by Application priority alone.
3. WP-04 shall preserve the distinction between Application rank and message/operation/business priority.
4. WP-05 shall expose authoritative resource-pressure truth without embedding QoS scheduling or Application load-shedding logic.
5. WP-06 shall accept only declared authorized Application-level request principals; exact Trading-side principals/messages remain pending Application clarification.
6. WP-07 shall support bounded reclaim/rebalance/restoration, including reduction of lower-priority Application allocations to zero reclaimable allocation where governed conditions allow, while preserving Foundation survival/protection/recovery floors.
7. WP-08 shall expose only the affected Application's attributable resource state and decision outcomes.
8. WP-09 shall include explicit compatibility verification with accepted Stage-5 WP-06 pressure/technical-class consumption and preserve future FCR-0009 QoS consumption without implementing QoS.
9. Stage-6 closure shall not claim FCR-0007 or FCR-0010 CLOSED until the FCR protocol's Application verification requirement is met where implementation is required.
10. Stage 6 shall not absorb External Egress, FSA control-plane, artifact publication/consumption or transport-observability implementation merely because those capabilities consume resource truth.

## 6. Planning hazards avoided by this study

- Building duplicate Stage-5 routing/event capabilities for FCR-0004/0005/0006.
- Creating Trading-specific resource logic inside Foundation.
- Implementing four separate egress/credential stacks for research, simulation, providers and brokers.
- Letting QoS recreate its own pressure/priority truth instead of consuming SYS-006 resource governance.
- Letting FSA become a Trading decision layer.
- Leaving canonical Application consumption of Foundation artifacts as an informal branch/path workaround.
- Treating an open FCR as proof that Foundation implementation is absent without revalidating against later accepted Stages.

## 7. Current planning state

STAGE6_FCR_FORWARD_STUDY = COMPLETE
STAGE6_WP_MAP_COUNT_CHANGE_REQUIRED = NO
STAGE6_WP_MAP_HARDENING_REQUIRED = YES
FCR_0007_STAGE6_DIRECT = YES
FCR_0010_STAGE6_DIRECT = YES
FCR_0009_STAGE6_PREREQUISITE_ONLY = YES
FCR_0004_0005_0006_REBUILD_FROM_SCRATCH = NO
EXTERNAL_EGRESS_FAMILY = FCR_0008 + FCR_0011 + FCR_0013 + FCR_0014
FSA_CONTROL_PLANE_FAMILY = FCR_0012
FOUNDATION_ARTIFACT_CONSUMPTION_GAP = CONFIRMED_HIDDEN_CROSS_CUTTING_NEED
FUTURE_STAGE_NUMBER_ASSIGNMENT = DEFERRED_TO_OWNER_ACCEPTED_FUTURE_STAGE_DESIGN

This study grants no implementation authority.