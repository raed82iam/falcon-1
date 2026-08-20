# IMP-001 v1.3 — VPL Impact and Stage Verification Mapping

**Status:** CANDIDATE SUPPORTING RECORD / NOT ACTIVE  
**Date:** 2026-08-09  
**Implementation Authority:** NOT GRANTED

## 1. Decision

The current VPL-000 Foundation Verification Master Plan remains scoped to FRS-001 and does not require semantic expansion to cover post-FRS Foundation Stages 11 through 17.

This record maps the existing FRS scenarios to the corrected Stage sequence and defines the planning rule for later post-FRS verification.

## 2. Existing FRS Verification Mapping

| Plan | FRS Scenario | Corrected Stage | Disposition |
|---|---|---|---|
| VPL-001 | FRS-SCN-001 Trusted Bootstrap | Stage 0A through Stage 3 preserved baseline | `PRESERVE_EXISTING_MEANING` |
| VPL-002 | FRS-SCN-002 Unauthorized Action | Stage 4 | `PRESERVE_EXISTING_MEANING` |
| VPL-003 | FRS-SCN-003 Invalid Lifecycle Transition | Stage 4 | `PRESERVE_EXISTING_MEANING` |
| VPL-004 | FRS-SCN-004 Invalid FIL Message | Stage 5 | `PRESERVE_EXISTING_MEANING` |
| VPL-005 | FRS-SCN-005 Health Evidence Loss | Stage 7 | `FUTURE_EXECUTION_AFTER_RECONCILIATION` |
| VPL-006 | FRS-SCN-006 Guardian Restriction | Stage 8 | `FUTURE_EXECUTION_AFTER_RECONCILIATION` |
| VPL-007 | FRS-SCN-007 Controlled Recovery | Stage 9 | `FUTURE_EXECUTION_AFTER_RECONCILIATION` |
| VPL-008 | FRS-SCN-008 Evidence Reconstruction | Stage 10 | `FUTURE_FINAL_FRS_RECONSTRUCTION` |

## 3. No Retroactive Evidence Reclassification

Accepted Stage 0A through Stage 5 evidence remains bound to its original approved scope and closure.

The corrected Master Plan does not require rerunning or relabeling accepted historical evidence merely because future Stage numbering is corrected.

Future Stage design may require targeted regression where a new change could affect preserved behavior. Such regression is prospective verification, not reopening the old closure.

## 4. Post-FRS Verification Rule

Stages 11 through 17 are outside FRS-001 and SHALL receive separately governed verification plans during their own Stage design.

The Master Plan fixes the verification obligation family but not invented future plan IDs or test details.

Each Stage design SHALL define:

- exact governed requirements;
- existing-capability reconciliation result;
- positive and negative scenarios;
- authority and isolation checks;
- failure/degradation/recovery checks where applicable;
- evidence package requirements;
- deterministic/regression requirements;
- independent review role;
- Stage closure gate.

## 5. Minimum Post-FRS Verification Families

### Stage 11
Transport deadline/QoS/observability correctness, bounded overload, starvation prevention, tail-latency evidence, and non-creation of technical criticality by Application urgency.

### Stage 12
Research/non-Live/provider/broker egress-role separation, credential-reference isolation, destination-policy enforcement, revocation, stale-authority rejection, and fail-closed external access.

### Stage 13
Owner/FSA command authenticity, replay resistance, delegation limits, silence-is-not-approval, candidate/evolution provenance, independent promotion/rejection and rollback governance.

### Stage 14
Exact artifact identity/version/digest/provenance, build-time consumption, compatibility/change detection, unavailable-version failure, rollback/replacement, and supply-chain integrity.

### Stage 15
Zero-or-more Application runtime hosting, admission/activation isolation, rejection, suspension, update/replacement/removal, resource binding and failure containment without Application business logic.

### Stage 16
Per-environment realization qualification, evidence scoping, reproducibility, provider/adaptor non-authority, failure/recovery/exit and prohibition on cross-environment validity inference.

### Stage 17
Zero-Application cold start/steady state/restart/recovery, first Application admission, removal back to zero, rejected Application isolation, Application failure isolation and standalone operation in every claimed operational environment.

## 6. Result

`VPL000_MEANING_CHANGE_REQUIRED = NO`

`FRS_VPL_STAGE_REMAP_COMPLETE = YES`

`POST_FRS_VERIFICATION_OBLIGATIONS_IDENTIFIED = YES`

`POST_FRS_EXACT_VPL_BODIES_DEFERRED_TO_STAGE_DESIGN = YES_BY_DESIGN`

`VPL_MAPPING_ACTIVATION_BLOCKER = CLOSED`
