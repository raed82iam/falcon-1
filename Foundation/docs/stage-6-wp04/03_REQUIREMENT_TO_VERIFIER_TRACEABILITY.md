# Stage 6 WP-04 — Requirement-to-Verifier Traceability

Status: IMPLEMENTED / FOCUSED VALIDATION READY

## Governing basis
This traceability is based on the controlling Stage 6 WP-04 authorization, SYS-006, accepted WP-01 through WP-03 predecessors, the Owner resource-priority clarification, and the full controlling FCR-0010/FCR-0007 chronology.

## Traceability

| Requirement / invariant | Production surface | Verifier evidence |
|---|---|---|
| Application priority and technical criticality remain distinct | `ResourcePriorityClassId` / `TechnicalCriticalityClassId`; separate definitions, relations and bindings | `priority_and_criticality_types_are_distinct`; `application_binding_has_no_criticality_field`; `technical_binding_has_no_application_priority_field` |
| Ordering comes from admitted policy, not invented numeric precedence | `ResourcePriorityClassRelation`; `TechnicalCriticalityClassRelation`; versioned policy metadata | `direct_priority_relation`; `transitive_priority_relation`; `direct_criticality_relation`; `transitive_criticality_relation`; `numeric_precedence_not_in_public_surface` |
| Same class does not outrank itself | relation constructors + `Outranks` / `IsMoreCritical` | `same_priority_class_does_not_outrank_itself`; `same_criticality_class_is_not_more_critical`; self-relation rejection tests |
| Policy graph is fail closed | duplicate/unknown/cycle validation | duplicate relation, unknown endpoint and cycle rejection tests |
| Priority policy is attributable/versioned/effective | `PriorityPolicyVersion`, `PriorityPolicyEvidence`, `PriorityPolicyLifetime` | wrong epoch, future evidence, future effective, expired policy, blank version, identity mutation tests |
| Technical-criticality policy is independently attributable/versioned/effective | `CriticalityPolicyVersion`, `CriticalityPolicyEvidence`, `CriticalityPolicyLifetime` | independent criticality policy epoch/relation tests and identity mutation |
| Binding applies only to admitted/current Application allocation predecessor | `ApplicationResourcePriorityBinding` validated against `ApplicationResourceAllocationSnapshot` | unknown Application and duplicate Application binding rejection |
| Technical criticality binds exact technical scope/resource class | `TechnicalCriticalityBinding` | duplicate technical binding, unknown resource, unknown criticality class rejection |
| Cross-Application substitution does not expose another Application policy | `GetApplicationView` scopes by exact `ApplicationPrincipalId` | `application_view_is_scoped`; `unknown_application_view_has_no_binding` |
| Foundation protected floors/reserves are outside Application priority competition | no Foundation protected priority rank exists in WP-04; WP-02 predecessor remains authoritative | `foundation_protected_floor_not_application_ranking_field`; allocation predecessor preservation |
| Trading priority is representable only through generic admitted policy/configuration | no Trading/TARC production type or hard-coded rule | `production_surface_has_no_trading_terms` |
| Caller/Application urgency cannot directly mint Foundation technical criticality | priority and criticality bindings are structurally separate | separation tests above |
| WP-04 does not alter WP-03 quantities | allocation snapshot is consumed read-only | `allocation_quantities_remain_unmodified`; `allocation_snapshot_changes_identity` |
| WP-05+ runtime behavior remains absent | WP-04 contains policy/state only | `production_surface_has_no_wp05_runtime_terms` |
| Public truth is deterministic and reconstructable | canonical ordering + SHA-256 identity over predecessor, policy metadata, relations and bindings | `ordering_is_deterministic`; policy/relation/allocation identity mutation tests; `identity_is_uppercase_sha256` |
| Zero-Application Foundation remains valid | empty class/relation/binding sets allowed over valid predecessor truth | `zero_application_validity` |

## FCR boundary
WP-04 satisfies only the priority / technical-criticality policy prerequisite required by the open resource-governance FCRs.

It does not implement or claim closure for:
- pressure state runtime;
- preemption or enforcement runtime;
- resource request/decision processing or TARC requester-role enforcement;
- reclamation, redistribution, rebalance or restoration;
- Application-facing pressure/load-shedding projections.

Those remain separately authorized later Stage 6 Work Packages.

## Validation gate
Focused validation SHALL run against the exact current `foundation-development` technical baseline and SHALL include:
- clean exact-baseline checkout;
- Restore;
- Release Build;
- Architecture tests;
- Security tests;
- accepted Stage 6 WP-01, WP-02 and WP-03 verifier regressions;
- WP-04 verifier twice for deterministic rerun;
- final HEAD/worktree preservation checks.

Full historical closure regression remains a later gate after focused validation and post-focused Red-Team.
