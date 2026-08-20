# Stage 6 WP-04 — Pre-Implementation Red-Team and Design

Result: PASS_WITH_HARDENING_REQUIREMENTS

## Reviewed basis
- Falcon Document Authority.
- Foundation Workstream Rules.
- TREE-001 canonical SYS-006 placement.
- SYS-006 Multi-Level Resource Governance.
- Stage 6 Owner priority clarification.
- Stage 6 WP-01, WP-02 and WP-03 accepted-and-closed prerequisites.
- Stage 6 WP-04 Owner authorization.
- FCR-0010 current state and chronology.
- Existing WP-01 resource-governance primitives, including distinct `ResourcePriorityClassId` and `TechnicalCriticalityClassId`.
- Existing WP-03 Application allocation/quota/ceiling/isolation state.
- Root README after controlled Stage 6 current-state synchronization.

## Proposed implementation shape
Extend the existing Application-neutral Foundation resource-governance state model. Preserve the existing distinction between primitive identity and governed truth.

The WP-04 production model should provide two separate governed surfaces:
1. cross-Application resource-priority policy/truth;
2. technical-criticality policy/truth.

Neither surface is an allocator, pressure engine, preemption engine, request processor, reclamation engine or load-shedding engine.

## Mandatory invariants
1. `ResourcePriorityClassId` and `TechnicalCriticalityClassId` remain distinct types and distinct semantic jurisdictions.
2. A priority/criticality identifier is value/identity only and cannot authorize resource use.
3. Every effective Application-priority binding identifies exactly one admitted `ApplicationPrincipalId`, one effective priority class, attributable evidence, epoch and effective lifetime.
4. Duplicate effective priority bindings for the same Application fail closed.
5. Priority truth cannot be future-effective, expired, stale for the consumed observation, or epoch-mismatched.
6. Cross-Application substitution fails closed; one Application cannot consume or impersonate another Application's priority binding.
7. Technical criticality must be Foundation-governed. Caller/Application/Guardian/TARC/QoS/business urgency is evidence only and cannot directly mint or elevate Foundation technical criticality.
8. Any technical-criticality binding must preserve attributable evidence, exact scope, epoch, observation/effective time and deterministic identity.
9. Foundation survival/protection/control, Authority, Health/Recovery, security/evidence integrity and non-reclaimable floor/reserve classes remain outside Application priority competition and above Application workloads.
10. The Owner Trading-priority rule is represented generically at the Foundation policy boundary and must not introduce Trading/TARC-specific production types or business logic.
11. Priority or criticality truth must not modify accepted WP-03 allocation/quota/ceiling quantities in WP-04.
12. Zero Applications remains valid.
13. Public state is immutable and deterministic.
14. Application business semantics remain opaque to Foundation.

## Red-Team attack cases
The dedicated WP-04 verifier must cover at least:
- malformed/blank priority and criticality identifiers;
- identity-equals-authority confusion;
- duplicate Application priority bindings;
- duplicate policy/binding identities;
- unknown Application binding relative to the consumed allocation snapshot where an allocation-bound policy is required;
- wrong resource epoch;
- future evidence;
- future-effective policy;
- expired policy;
- stale/superseded policy represented as current;
- cross-Application substitution;
- caller-proposed technical criticality elevation;
- Application business priority converted directly into Foundation technical criticality;
- Application priority attempting to outrank Foundation protected control/survival domains;
- Trading-specific type/name leakage into production namespace;
- priority truth mutating allocation/quota/ceiling;
- pressure/preemption/request/reclaim/load-shedding symbols or behavior leaking into WP-04;
- non-deterministic identity or mutable public state;
- zero-Application case.

## Scope-leak checks
Production surface SHALL implement none of:
- resource-pressure state transitions;
- preemption;
- enforcement-state runtime;
- resource request admission/decision;
- requester-role authorization or TARC runtime binding;
- reclamation, redistribution, rebalance or restoration;
- load shedding;
- Application-internal allocation/distribution;
- Trading strategy, Risk, execution, market, instrument, order, broker or provider semantics.

## FCR conclusion
FCR-0010 remains OPEN. WP-04 can satisfy only its generic priority/technical-criticality prerequisite. No claim of Foundation implementation completion or Application verification is permitted for pressure, enforcement, requests, shedding, restoration or redistribution.

## Architecture conclusion
No architecture blocker was found for a bounded WP-04 implementation that extends the existing Foundation resource-governance model and preserves all invariants above.

A new permanent production subsystem is not justified by the reviewed requirements and would require separate review.

## Documentation sync finding
### Finding D-WP04-001
Severity: HIGH
Classification: Documentation / Governance-State Sync Defect
Status: RESOLVED_AND_RED_TEAM_RECHECKED

Original condition:
The root `README.md` current-state section predated the accepted closure of Stage 6 WP-01 through WP-03 and the explicit WP-04 authorization.

Controlled remediation:
- synchronized the README current Stage 6 status with the canonical Owner records;
- recorded WP-01 through WP-03 as accepted/closed;
- recorded WP-04 as authorized/in progress;
- preserved WP-05 through WP-10 and Stage 7 through Stage 9 as unauthorized;
- preserved historical Stage 5 content and unrelated architecture text;
- updated current non-authorities so later Stage 6 runtime behavior is not accidentally authorized.

Remediation commit:
`f8e6d5e6b68595360059dcec5fe9eed6ef17fb83`

Post-remediation assessment:
- README no longer contradicts the controlling Stage 6 Owner records;
- no new implementation authority was created by the synchronization;
- FCR-0010 remains open and later runtime scope remains excluded;
- no production code changed during remediation;
- no architectural blocker remains from D-WP04-001.

## Current verdict
`WP04_SCOPE_RECONCILIATION = PASS`

`WP04_PRE_IMPLEMENTATION_RED_TEAM = PASS_WITH_HARDENING_REQUIREMENTS`

`D-WP04-001 = RESOLVED`

`WP04_PRODUCTION_IMPLEMENTATION = AUTHORIZED_TO_PROCEED_WITHIN_EXACT_WP04_SCOPE`

`WP05_AND_LATER_AUTHORITY = NOT_GRANTED`
