# Stage 6 WP-03 — Requirement-to-Verifier Traceability

| Requirement | Verification coverage |
|---|---|
| allocation <= quota <= ceiling | `allocation_below_quota_below_ceiling`, rejection scenarios |
| exact WP-02 resource-class/unit binding | `truth_unit_mismatch_rejected`, `unknown_resource_class_rejected` |
| exact resource epoch/evidence binding | `evidence_epoch_mismatch_rejected`, `future_evidence_rejected` |
| current effective allocation only | future/expired allocation rejection scenarios |
| no duplicate Application/resource binding | `duplicate_application_resource_binding_rejected` |
| no duplicate grant identity | `duplicate_grant_identity_rejected` |
| no use of protection floor/recovery reserve | floor/reserve isolation scenarios |
| aggregate allocation within allocatable capacity | `aggregate_allocation_over_allocatable_rejected` |
| aggregate quota within allocatable capacity | `aggregate_quota_over_allocatable_rejected` |
| aggregate ceiling within allocatable capacity | `aggregate_ceiling_over_allocatable_rejected` |
| exact boundary allowed | `exact_allocatable_boundary_is_valid` |
| zero-Application validity | `zero_application_validity` |
| deterministic immutable state identity | ordering/repeat/mutation/SHA scenarios |
| exact Application-scoped isolation | own-view / unknown-view / source-snapshot scenarios |
| read-only public collections | allocation/view read-only scenarios |
| no Application identity or grant identity authority creation | identity authority-negative scenarios |
| no Trading semantics | `production_surface_has_no_trading_terms` |
| no WP-04+ runtime semantics | later-WP surface checks |
| exactly one WP-02 truth predecessor | `resource_truth_remains_singular_predecessor` |

WP-03 verifier is intentionally bounded to allocation/quota/ceiling/isolation state and does not validate later Stage 6 runtime engines.
