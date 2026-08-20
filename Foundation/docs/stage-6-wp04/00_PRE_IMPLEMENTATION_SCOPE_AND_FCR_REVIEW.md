# Stage 6 WP-04 — Pre-Implementation Scope and FCR Review

Status: AUTHORIZED_FOR_IMPLEMENTATION / PRE-IMPLEMENTATION REVIEW

## Purpose
Implement the generic Foundation-owned cross-Application resource-priority and technical-criticality governance boundary over the accepted Stage 6 WP-01 through WP-03 resource-governance prerequisites.

## Governing authority reviewed
- Falcon Document Authority (`docs/03_DOCUMENT_AUTHORITY.md`).
- Foundation Workstream Rules (`docs/development/FOUNDATION_WORKSTREAM_RULES.md`).
- TREE-001 canonical placement for SYS-006.
- SYS-006 Multi-Level Resource Governance.
- Stage 6 Owner Priority Clarification and Application Input Request.
- Stage 6 WP-01, WP-02 and WP-03 accepted-and-closed prerequisites.
- Stage 6 WP-04 Owner Authorization.
- FCR-0010 current Issue body and relevant chronological evidence.

## Preconditions
- Stage 6 WP-01 = ACCEPTED_AND_CLOSED.
- Stage 6 WP-02 = ACCEPTED_AND_CLOSED.
- Stage 6 WP-03 = ACCEPTED_AND_CLOSED.
- WP-04 Owner authorization is prospective and scoped to WP-04 only.
- `foundation-development` is the only writable branch for this workstream.

## In scope
- Foundation-governed cross-Application resource-priority truth.
- Foundation-governed technical-criticality truth for technical resource governance.
- Exact separation between `ResourcePriorityClassId` and `TechnicalCriticalityClassId`.
- Application-neutral policy/state representation that can bind an admitted Application to an effective Application resource-priority class.
- Application-neutral technical-criticality representation that is Foundation-derived/governed rather than caller-self-declared.
- Exact evidence, epoch, effective-time, supersession and deterministic identity binding required for WP-04 truth.
- Preservation of the Owner rule that Trading-related Applications occupy the highest Application-level resource-priority domain for Foundation-governed technical resources only.
- Preservation of Foundation survival/protection/control, Authority, Health/Recovery, security/evidence integrity and non-reclaimable reserve/floor precedence above Application workloads.
- Fail-closed handling of absent, malformed, stale, future-effective, expired, ambiguous or cross-Application-substituted priority/criticality truth.
- Zero-Application validity.

## Required separation
Application resource priority and Foundation technical criticality are not interchangeable:
- Application priority is a governed cross-Application ordering/classification input.
- Technical criticality is a Foundation-governed technical classification.
- Application-origin urgency, QoS, Guardian signals, business importance or caller-proposed priority are evidence only unless independently admitted by the exact Foundation policy/authority boundary.
- Identity or possession of a priority/criticality value does not create resource authority.

## Owner priority clarification treatment
The Owner clarification is interpreted narrowly:
- Trading-related Applications receive the highest Application-level priority for Foundation-governed technical resources.
- This priority does not grant Trading general governance, security, lifecycle, data-truth, Risk, execution, strategy, market, instrument, order, provider, broker, awareness-jurisdiction or Owner authority.
- Foundation protected control/survival capacity remains above all Application workloads.
- No Trading/TARC-specific production type, namespace, rule engine or business semantic is introduced in WP-04.

## FCR reconciliation
FCR-0010 is relevant to WP-04 only for the generic priority/technical-criticality governance prerequisite.

WP-04 SHALL NOT claim completion of FCR-0010 overall. The following remain later separately authorized scope:
- pressure state calculation and pressure runtime;
- preemption and enforcement-state runtime;
- load shedding;
- resource request/decision runtime and TARC requester-role enforcement;
- reclamation, redistribution, rebalance and restoration;
- Application-facing runtime pressure/request-outcome projection.

FCR-0007 remains outside WP-04 because requester authorization and request/decision processing belong to a later Work Package.

## Out of scope
- pressure/preemption/enforcement state (WP-05);
- additional-resource request/decision boundary (WP-06);
- reclamation/redistribution/rebalance/restoration (WP-07);
- per-Application resource-state/load-shedding signal boundary (WP-08);
- Stage 6 integration/hardening and closure verification (WP-09/WP-10);
- Application-internal resource distribution;
- Trading/TARC-specific production behavior;
- Application business semantics;
- resource allocation changes beyond the already accepted WP-03 allocation/quota/ceiling truth;
- external connectivity, credentials, broker/provider behavior or Application artifact-consumption mechanics.

## Architectural placement constraint
WP-04 shall extend the existing Foundation resource-governance state/contract model rather than create a second Foundation resource-state owner or an Application-specific subsystem. A new permanent runtime service/project requires separate architectural justification and is not implied by this authorization.

## Documentary discrepancy discovered before implementation
The root `README.md` current-state summary is stale: it still describes Stage 6 WP-01 as authorized/in-progress and WP-02 through WP-10 as unauthorized. That summary conflicts temporally with later canonical Owner closure/authorization records.

Classification: DOCUMENTATION / GOVERNANCE-STATE SYNC DEFECT.

The stale README does not revoke later explicit Owner authority, but it is unsafe as a current-state summary and must not be used as the implementation authority source for WP-04. Canonical Owner records and WP closure reconciliation govern the current state.

This discrepancy must remain visible in the WP-04 Red-Team and shall be corrected through a controlled documentation reconciliation before WP-04 is presented as implementation-complete or owner-ready.

## Authority
Owner authorization record:
`docs/canonical-records/owner-decisions/stage6/Stage6-WP04-Implementation-Authorization-20260809-035600/OWNER-AUTHORIZATION-STAGE6-WP04-IMPLEMENTATION.txt`.

WP-05 and later remain unauthorized.
