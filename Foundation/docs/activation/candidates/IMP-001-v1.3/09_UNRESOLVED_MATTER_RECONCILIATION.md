# IMP-001 v1.3 — Current-Effective Unresolved-Matter Reconciliation

**Status:** CANDIDATE SUPPORTING RECORD / ROADMAP DISPOSITION COMPLETE  
**Date:** 2026-08-09  
**Implementation Authority:** NOT GRANTED

## 1. Purpose

This record prevents `Unresolved Matters`, ADR-candidate lists, dependent-artifact clauses, or stale documentary wording in current-effective Specifications from being misclassified as hidden defects in accepted Stages or silently ignored during Master Plan succession.

This is a roadmap disposition record. It does not claim implementation completion where evidence has not been established.

## 2. Decision Rule

Each identified matter receives one of:

- `RESOLVED_BY_CURRENT_EFFECTIVE_GOVERNANCE_OR_ACCEPTED_BASELINE`;
- `FUTURE_FOUNDATION_STAGE_RECONCILIATION`;
- `POST_FOUNDATION_DOMAIN_OWNED`;
- `DOCUMENTARY_CONSISTENCY_REMEDIATION`;
- `NOT_A_MASTER_PLAN_ACTIVATION_BLOCKER`.

A future placement closes the Master Plan coverage question only. It does not claim technical implementation or verification completion.

## 3. SYS-001 Kernel

Current unresolved text:

- formal list of essential versus nonessential Core functions;
- ratifying authority for Core component admission.

Disposition:

- Essential/nonessential classification -> `FUTURE_FOUNDATION_STAGE_RECONCILIATION`, principally Stage 8 protective/Safe-State design and Stage 17 standalone operational-readiness acceptance. Existing accepted resource survival/protection floors and current Core ownership/authority surfaces must be reused rather than duplicated.
- Core-admission ratifying authority -> `RESOLVED_OR_PARTIALLY_RESOLVED_BY_CURRENT_EFFECTIVE_GOVERNANCE_OR_ACCEPTED_BASELINE`, through AUT-001/GOV-AUT-001/SYS-003 and current authority/admission governance; exact residual wording shall be checked during Stage 15 existing-capability reconciliation before any new admission implementation.

Master-plan effect: no accepted closure is reopened; no orphan future obligation remains.

## 4. SYS-002 Lifecycle

Current unresolved text:

- consequence-based transition timeouts;
- formal dependency cycle resolution policy.

Disposition:

- Consequence-based transition timeouts -> `FUTURE_FOUNDATION_STAGE_RECONCILIATION`, Stage 11 deadline/QoS governance with Stage 15 lifecycle-runtime integration where applicable.
- Dependency cycle resolution -> current SYS-004 already rejects circular mandatory dependencies and governs dependency ordering; classify `RESOLVED_OR_PARTIALLY_RESOLVED_BY_CURRENT_EFFECTIVE_GOVERNANCE_OR_ACCEPTED_BASELINE`, with exact residual behavior checked before Stage 15 runtime lifecycle work.

Master-plan effect: mapped; not a Stage 4 closure defect.

## 5. SYS-005 Service Bus

Current unresolved text:

- communication consequence classes;
- required delivery semantics for protective control messages.

Disposition:

- Communication consequence classes -> `FUTURE_FOUNDATION_STAGE_RECONCILIATION`, Stage 11, consuming Stage 6 technical-priority truth and not permitting producer-created criticality.
- Protective control-message delivery -> accepted Stage 5 WP-06 already owns bounded delivery semantics, governed technical priority, congestion handling and truthful transport evidence. Remaining protective-control consequences shall be reconciled in Stage 8 and Stage 11 without rebuilding Stage 5. Classification: `PARTIALLY_SATISFIED_REUSE_REQUIRED`.

Master-plan effect: mapped; Stage 5 remains closed.

## 6. PLG-001 Capability Passport and Admission

Current unresolved text:

- capability consequence classes;
- Core responsibilities that are constitutionally non-pluggable.

Disposition:

- Capability consequence classes -> `FUTURE_FOUNDATION_STAGE_RECONCILIATION`, Stage 14/15 admission and runtime isolation design, consuming existing authority/resource/security consequence models where available.
- Constitutionally non-pluggable Core responsibilities -> `FUTURE_FOUNDATION_STAGE_RECONCILIATION`, Stage 15, constrained by SYS-001, Constitution, current Core ownership and `ZERO_APPLICATION_OPERATION_IS_VALID`. This question must not be answered by Application needs.

PLG-001 also states that Falcon Cell realization requires an ADR. Therefore any Stage 15 design selecting Falcon Cells as the isolation realization must create/accept the required ADR prospectively.

## 7. DEC-006 Decision Ledger

Current unresolved text:

- materiality thresholds by decision class;
- retention obligations by jurisdiction.

Disposition: `POST_FOUNDATION_DOMAIN_OWNED` for detailed decision/financial policy unless a future Foundation-specific Specification defines a generic infrastructure requirement.

Foundation may provide generic evidence/storage/security/traceability primitives, but shall not define financial/domain materiality or jurisdictional retention policy inside the Foundation Master Plan.

## 8. RSK-005 Capital Safety Plane

Current unresolved text:

- capital risk taxonomy and upper-limit catalog;
- jurisdiction/institution-specific external protection.

Disposition: `POST_FOUNDATION_DOMAIN_OWNED`.

These are capital/risk/financial protection matters outside the non-financial Foundation implementation roadmap. They are not missing requirements of accepted Foundation Stages and do not block IMP-001 v1.3 activation.

## 9. AWR-001 Documentary Consistency

AWR-001 v2.1 is current-effective and activated by its controlling metadata/activation lineage, but its body contains stale candidate-era wording near the end indicating pending/proposed-successor status.

Disposition: `DOCUMENTARY_CONSISTENCY_REMEDIATION`.

This is not an architecture or implementation defect and does not invalidate AWR-001 activation. The coordinated documentary activation package shall include a versioned administrative/documentary correction or governed amendment that removes the contradictory stale candidate wording without changing AWR-001 requirement meaning.

## 10. FCE-001 and Dependent-Artifact Clauses

FCE-001 lists required dependent artifacts including catalogs, schema registry, vectors, security design, Contract amendment decisions, TRC and PIPE coverage.

Disposition: `EXISTING_CAPABILITY_RECONCILIATION_REQUIRED`, not automatic future work.

The accepted Stage 0 through Stage 3 baseline already contains substantial enabling-provider, catalog, pipeline, trace and security-design work. Any FCE-dependent future Stage must first prove the exact residual gap before creating implementation work.

## 11. Registered Future Specification Subjects

The 38 registry-only `NOT YET EFFECTIVE` subjects remain visible planned subjects and do not supply normative implementation requirements by title.

Their Master Plan coverage is closed by one of:

- assigned Foundation Stage subject to `SPECIFICATION_DEFINITION_REVIEW_ACTIVATION_GATE`;
- `POST_FOUNDATION_DOMAIN_OWNED`;
- dependency-only visibility.

No requirement is invented from those rows.

## 12. Master-Plan Activation Result

`UNRESOLVED_MATTER_INVENTORY_DISPOSITIONED_FOR_ROADMAP = YES`

`ACCEPTED_CLOSURE_DEFECT_PROVEN = NO`

`UNASSIGNED_KNOWN_FOUNDATION_FAMILY_FROM_THIS_REVIEW = NO`

`AWR001_DOCUMENTARY_CLEANUP_REQUIRED_IN_ACTIVATION_PACKAGE = YES`

`UNRESOLVED_MATTER_MASTER_PLAN_BLOCKER = CLOSED`

Technical completion of future matters remains separately gated by their assigned Stage/WP.