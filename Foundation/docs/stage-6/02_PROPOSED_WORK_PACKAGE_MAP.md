# Stage 6 Proposed Work Package Map

Status: PROPOSED / OWNER DESIGN ACCEPTANCE REQUIRED / FORWARD-INPUT RECONCILIATION PENDING
Date: 2026-08-08
Branch: foundation-development
Forward study: `docs/stage-6/07_FORWARD_FCR_ARCHITECTURE_IMPACT_STUDY.md`

## Proposed decomposition

### WP-01 — Canonical Resource Governance Primitives
Defines immutable generic identities and evidence surfaces for resource class, capacity, allocation, quota, ceiling, floor, reserve, pressure, cross-Application priority, technical criticality, request, decision, reclamation and restoration conditions.

The primitives SHALL remain Application-neutral and SHALL be capable of later canonical publication/consumption through a separately governed Foundation artifact-consumption boundary without embedding an ad-hoc source-copy or branch-coupling mechanism into Stage 6.

Boundary: data/identity semantics only. No global allocation controller yet. No artifact/package publication implementation.

### WP-02 — Foundation Resource Truth and Protection Reserves
Establishes the singular Foundation-owned total-resource truth model, allocatable capacity, Foundation survival/protection floors and recovery reserves, with deterministic snapshot/evidence and fail-closed handling for unavailable or contradictory truth.

Boundary: Foundation technical resource truth only. No Application business semantics.

### WP-03 — Application Allocation, Quota, Ceiling and Isolation
Implements attributable Application grants, quotas and hard ceilings; prevents cross-Application consumption and hidden borrowing; preserves zero-Application validity and makes allocations explicitly reclaimable/non-reclaimable according to governed policy.

Boundary: Foundation allocation envelope only. Applications remain owners of internal distribution inside the admitted grant.

### WP-04 — Cross-Application Priority and Technical Criticality Governance
Implements governed cross-Application priority classes and technical criticality with exact authority/evidence binding.

The Owner-approved policy places Trading-related Applications in the highest Application-priority domain relative to lower-priority non-Trading Applications such as future Accounting, Warehouse and similar workloads. No Application may self-promote its priority class. Trading remains below non-reclaimable Foundation survival/protection/control floors.

Priority records SHALL remain generic and attributable so accepted/future transport consumers may consume Foundation technical-class truth without allowing producer-declared QoS or business urgency to mint Foundation criticality.

Boundary: technical infrastructure priority and explicit Owner cross-Application policy only. No Trading strategy/Risk/market/business judgment. No QoS scheduling or transport service-level implementation.

### WP-05 — Resource Pressure, Preemption and Enforcement-State Truth
Implements attributable pressure/enforcement observations, constrained/degraded/critical states, reclamation eligibility and deterministic evidence. Provides consumer-facing Foundation truth without creating consumer business actions.

Pressure truth SHALL be stable enough for accepted Stage 5 WP-06 and future QoS/observability consumers to consume without creating a second pressure-truth owner.

Boundary: pressure/preemption truth only. No transport redesign, tail-latency service, QoS scheduler or Application load-shedding policy ownership.

### WP-06 — Additional Resource Request and Decision Boundary
Implements the generic evidenced request flow required by SYS-006 and directly informed by FCR-0007. Foundation may allow, cap, deny, defer or trigger governed reclamation/rebalance according to authority, availability, Owner-approved cross-Application priority, Foundation floors/reserves, isolation and technical criticality.

Boundary: request/decision semantics only. Request does not imply grant. Application-facing principals and exact Trading message semantics remain pending/controlled by Application declarations.

Implementation/finalization of Application-facing principal validation SHALL wait for the requested Application clarification identifying which Application-level principals may submit ordinary versus emergency resource requests and the exact declared message/evidence contract. Foundation SHALL NOT invent those Trading-owned declarations.

### WP-07 — Governed Reclamation, Redistribution, Rebalance and Restoration
Implements bounded resource reclamation from lower-priority Applications, redistribution/rebalance decisions and progressive restoration conditions, preserving Foundation survival/protection floors, recovery reserves and isolation. Temporary grants must expire/release according to governed evidence and must not become permanent entitlement.

Under severe/critical pressure, lower-priority Application allocations may be reduced to zero reclaimable allocation when required to support the highest-priority Trading Application workload, while Foundation control-plane survival floors remain protected.

Boundary: technical resource reclamation/redistribution only; no Application business recovery logic.

### WP-08 — Per-Application Resource-State and Load-Shedding Signal Boundary
Exposes only the requesting Application's attributable grant/ceiling/pressure/enforcement/reclamation/request-outcome/restoration evidence needed for Application-owned load shedding, directly informed by FCR-0010.

Boundary: Foundation supplies technical truth; Application decides its own business degradation actions. Internal Trading degradation hierarchy remains Application-owned and must be supplied/declared by the Application rather than inferred by Foundation.

### WP-09 — Stage 6 Integration, Cross-Subsystem Consumption and Hardening
Verifies that Stage 5 WP-06 and other accepted/future generic consumers can consume Stage 6 pressure/priority/reclamation evidence without Stage 6 taking ownership of messaging, routing, delivery, events, lifecycle or Application semantics. Performs security/isolation/replay/determinism hardening.

Mandatory forward-compatibility checks:
- accepted Stage 5 WP-06 consumes Stage 6 pressure/technical-class truth without duplicate ownership;
- residual FCR-0009 QoS/observability work can consume Stage 6 priority/pressure evidence without Stage 6 implementing QoS;
- Stage 6 creates no dependency on research/provider/broker egress, FSA control-plane or canonical artifact-publication implementation;
- Stage 6 resource contracts remain suitable for later separately governed canonical Application consumption.

Boundary: integration and evidence compatibility, not a new global orchestrator.

### WP-10 — Integrated Stage 6 Closure Verification
Runs end-to-end Stage 6 verification, predecessor regression, FCR/completeness reconciliation and final closure-readiness evidence. Prefer verifier/integration evidence over a permanent Stage-6-wide runtime aggregation subsystem unless a concrete composition defect proves generic production glue is necessary.

WP-10 SHALL distinguish technical Stage-6 completion from FCR closure. FCR-0007/FCR-0010 may remain open pending required Application verification even if the Foundation implementation is technically accepted and closed.

Boundary: closure verification only; no later-Stage authority.

## Dependency order

WP-01 -> WP-02 -> WP-03 -> WP-04 -> WP-05 -> WP-06 -> WP-07 -> WP-08 -> WP-09 -> WP-10

A later WP may consume accepted predecessors but may not redefine them.

## Direct and forward FCR mapping

### Direct Stage-6 ownership

- FCR-0007 -> primarily WP-06, with WP-01/WP-03/WP-04/WP-07/WP-08 supporting identity, allocation, priority, reclamation, restoration and result projection.
- FCR-0010 -> primarily WP-05/WP-08, with WP-02/WP-03/WP-07 supporting resource truth, allocation/ceiling, reclamation/rebalance/restoration semantics.

Foundation planning clarification requests have been posted to FCR-0007 and FCR-0010 requesting exact Trading-side principals, message families, fields, internal degradation hierarchy, escalation sources and reactions to reduction/revocation/restoration. Those Application-owned details shall not be invented by Foundation.

### Stage-6 prerequisite/compatibility only

- FCR-0009 -> WP-04/WP-05/WP-09 must provide authoritative priority/pressure evidence and integration compatibility. Residual queue scheduling, latency SLO, tail-latency observability and QoS execution remain outside Stage 6.

### Revalidate against accepted Stage 5; do not rebuild

- FCR-0004 -> generic communication infrastructure must be revalidated against accepted Stage 5; Application protection-command semantics remain Application-owned.
- FCR-0005 -> generic route/delivery portions are substantially Stage-5-owned; remaining work is Application contract binding/verification or a demonstrated residual generic gap.
- FCR-0006 -> generic routing/delivery/event/replay portions are Stage-5-owned; remaining work is Application verification or a demonstrated residual generic gap.

### Separate future capability families; not Stage-6 implementation

- FCR-0008 + FCR-0011 + FCR-0013 + FCR-0014 -> one generic External Access / Egress / Credential-Reference Security family with distinct independently authorized research, non-Live, provider-operational and broker-execution roles.
- FCR-0012 -> separate FSA / Owner Governance and Bounded Evolution Control Plane family.

### Hidden cross-cutting gap requiring canonicalization

The Application-side request buried in FCR-0004 comments for canonical Foundation artifact publication/Application build-time consumption is NOT FCR-0004 runtime routing. Foundation has requested that Application workstream raise it as a dedicated canonical FCR. Stage 6 shall not invent a local package/feed/source-copy workaround.

## Mandatory invariants across all WPs

- Foundation remains valid with zero Applications.
- Trading-related Applications are the highest Owner-approved **Application** priority domain.
- Foundation survival/protection/control floors remain above Application workload allocation and are not consumed merely because Trading is highest Application priority.
- No Trading-specific business semantics are moved into Foundation.
- No Application may consume another Application's allocation without an explicit Foundation reclamation/redistribution decision.
- No Application may self-promote its priority class.
- Application-local urgency cannot mint Foundation technical criticality.
- Resource availability cannot mint authority.
- Resource request cannot mint a grant.
- Temporary resource grant cannot silently become permanent entitlement.
- Reclamation and restoration must be attributable, reversible where required, and evidenced.
- Resource recovery cannot widen authority.
- Pressure evidence must not imply business completion or business action.
- Stage 6 must not create deployment/runtime activation/external connectivity authority.
- Stage 6 must not absorb QoS scheduling, external egress, FSA governance, artifact publication/consumption or Application business degradation logic.

## Planning hold before final Owner design acceptance

The ten-WP structure remains technically coherent and the forward FCR study found no reason to redesign Stage 6 into a broader catch-all stage. However final Stage-6 design acceptance should remain on planning hold until the following inputs are reconciled:

1. Application response to the FCR-0007/FCR-0010 clarification request for exact resource-request principals/messages/degradation semantics.
2. Canonicalization of the hidden Foundation artifact publication/Application consumption gap into its own FCR or an explicit Application response that withdraws/reclassifies that need.

This planning hold prevents late patching of WP-06/WP-08 and keeps cross-workstream build integration from becoming an ad-hoc exception.

## Governance

This map is proposed only. None of WP-01 through WP-10 is authorized for implementation until the Stage 6 design/map is explicitly accepted and the applicable WP is separately authorized by the Owner.
