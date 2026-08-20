# Stage 6 WP-01 — Requirement to Verifier Traceability

Status: PRE-IMPLEMENTATION
Date: 2026-08-08

## Requirements

| ID | Requirement | Planned verifier obligation |
|---|---|---|
| S6W1-R01 | Resource class identity is nonblank, canonical and immutable. | valid_resource_class_identity / blank_resource_class_rejected |
| S6W1-R02 | Application principal identity is generic and nonblank. | valid_application_identity / blank_application_identity_rejected |
| S6W1-R03 | Allocation/grant identity cannot be blank. | grant_identity_validation |
| S6W1-R04 | Request identity cannot be blank. | request_identity_validation |
| S6W1-R05 | Decision identity cannot be blank. | decision_identity_validation |
| S6W1-R06 | Evidence identity cannot be blank. | evidence_identity_validation |
| S6W1-R07 | Correlation and causation identities remain distinguishable. | correlation_causation_distinct |
| S6W1-R08 | Epoch/version identity cannot be blank. | epoch_identity_validation |
| S6W1-R09 | Priority-class identity is generic and does not self-authorize priority. | priority_identity_is_value_only |
| S6W1-R10 | Technical-criticality identity is generic and distinct from Application internal priority. | criticality_identity_is_value_only |
| S6W1-R11 | Resource quantity cannot be negative. | negative_quantity_rejected |
| S6W1-R12 | Resource quantity requires a canonical unit. | missing_unit_rejected |
| S6W1-R13 | Zero quantity is representable. | zero_quantity_valid |
| S6W1-R14 | Pressure vocabulary distinguishes NORMAL/CONSTRAINED/DEGRADED/CRITICAL. | pressure_enum_exact |
| S6W1-R15 | Decision vocabulary distinguishes GRANT/PARTIAL_GRANT/CAP/DENY/DEFER/REVOKE/REDUCE/RESTORE. | decision_enum_exact |
| S6W1-R16 | Reclaimability vocabulary distinguishes RECLAIMABLE/NON_RECLAIMABLE/TEMPORARY. | reclaimability_enum_exact |
| S6W1-R17 | Effective lifetime rejects end-before-start. | invalid_lifetime_rejected |
| S6W1-R18 | Open-ended lifetime must be explicit, not inferred from missing invalid data. | explicit_open_ended_lifetime |
| S6W1-R19 | Evidence reference preserves identity, type/scope and timestamp/version material. | evidence_reference_complete |
| S6W1-R20 | Deterministic canonical hashing returns identical identity for identical primitive material. | deterministic_identity_repeat |
| S6W1-R21 | Material change changes deterministic identity. | deterministic_identity_material_change |
| S6W1-R22 | Canonical identity composition uses unambiguous length-delimited material. | delimiter_collision_resistance_fixture |
| S6W1-R23 | `RESOURCE_REQUEST != RESOURCE_GRANT`. | request_and_grant_types_distinct |
| S6W1-R24 | `RESOURCE_AVAILABILITY != RESOURCE_AUTHORITY`. | no_authority_from_quantity |
| S6W1-R25 | Presence of priority-class value does not mint Foundation priority authority. | no_authority_from_priority_value |
| S6W1-R26 | Presence of decision identity does not prove GRANT. | decision_identity_not_result |
| S6W1-R27 | Temporary grant vocabulary cannot silently represent permanent entitlement. | temporary_is_distinct |
| S6W1-R28 | Pressure value does not imply permission to exceed a ceiling. | pressure_not_authority |
| S6W1-R29 | Foundation survival/control resources can be represented distinctly without embedding an Application name. | generic_control_plane_scope |
| S6W1-R30 | No production API references Trading/FSATS/Accounting/Warehouse business types or names. | application_neutral_public_surface |
| S6W1-R31 | No WP-02+ allocation/pressure/reclamation engine is introduced. | no_later_wp_runtime_engine |
| S6W1-R32 | FCR-0016 package/feed/build-consumption mechanics are not implemented. | no_artifact_consumption_mechanics |
| S6W1-R33 | No external connectivity/credential API is introduced. | no_egress_or_credentials |
| S6W1-R34 | Primitives remain usable with zero Applications. | zero_application_validity |
| S6W1-R35 | Public records are immutable value objects. | immutable_public_primitives |
| S6W1-R36 | Invalid/malformed primitive creation fails closed with deterministic exception categories. | malformed_primitives_fail_closed |

## Gate

The verifier must cover every requirement above before WP-01 can be recommended for Owner closure.
