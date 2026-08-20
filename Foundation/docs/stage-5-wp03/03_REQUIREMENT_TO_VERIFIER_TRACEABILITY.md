# Stage 5 WP-03 — Requirement-to-Verifier Traceability

**Status:** Implementation traceability  

| Requirement area | Verification coverage |
|---|---|
| Zero-Application validity | `zero_application_foundation_is_valid` |
| Two independent Applications | `two_independent_application_manifests_register`, `two_application_digests_are_independent` |
| Duplicate Manifest rejection | `duplicate_manifest_registration_rejected` |
| Conflicting Manifest rejection | `conflicting_manifest_registration_rejected` |
| Stable Manifest/Application/Owner binding | `manifest_identity_binding_conflict_rejected` |
| Fail-closed unknown Manifest resolution | `unknown_manifest_resolution_fails_closed` |
| Deterministic Manifest resolution | `known_manifest_resolves` |
| WP-02 schema resolution required | `unresolved_schema_reference_fails_closed` |
| Retired schema rejected | `retired_schema_reference_fails_closed` |
| Registered/Active/Deprecated schema handling | `supported_schema_lifecycle_states_validate` |
| Duplicate governed references rejected | `duplicate_manifest_references_rejected` |
| Duplicate communication declaration rejected | `duplicate_communication_declaration_rejected` |
| Direction/role consistency | `invalid_direction_role_combinations_rejected` |
| Canonical versions/identities | `invalid_versions_and_identifiers_rejected` |
| Communication declaration required | `empty_communication_set_fails_closed` |
| Deterministic SHA-256 | `canonical_digest_is_deterministic`, `canonical_digest_is_order_independent_for_sets`, `different_manifest_content_changes_digest` |
| Deterministic snapshot | `snapshot_order_is_deterministic` |
| Manifest does not grant authority | `manifest_validity_does_not_grant_authority` |
| Manifest does not create routes | `manifest_validity_does_not_create_route` |
| Payload opacity | `manifest_model_contains_no_business_payload` |
| No FSATS privilege | `fsats_receives_no_special_treatment` |

## Acceptance note

Verifier source coverage is necessary but not sufficient for WP-03 closure. Owner acceptance additionally requires clean build evidence, architecture tests, security tests, predecessor verifier regression, deterministic rerun evidence, independent architecture review, independent red-team review, independent completeness review, and explicit Owner closure.
