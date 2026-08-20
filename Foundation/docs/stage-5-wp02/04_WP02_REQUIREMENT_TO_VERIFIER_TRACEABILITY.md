# Stage 5 WP-02 Requirement-to-Verifier Traceability

| Requirement | Primary verifier evidence |
|---|---|
| exact registration and resolution | `positive_registration_and_exact_resolution` |
| duplicate rejection | `duplicate_registration_rejected` |
| definition digest conflict rejection | `conflicting_digest_registration_rejected` |
| same-version owner conflict rejection | `conflicting_owner_registration_rejected` |
| cross-version single-owner invariant | `cross_version_owner_change_rejected` |
| unknown schema fail closed | `unknown_schema_resolution_fails_closed` |
| unknown version fail closed | `unknown_version_resolution_fails_closed` |
| exact compatibility | `exact_compatibility_is_implicit` |
| backward compatibility | `backward_compatibility_explicit` |
| forward compatibility | `forward_compatibility_explicit` |
| explicit incompatible relation | `incompatible_relationship_explicit` |
| undeclared relation fail closed | `undeclared_compatibility_fails_closed` |
| duplicate rule rejection | `duplicate_compatibility_rule_rejected` |
| conflicting rule rejection | `conflicting_compatibility_rule_rejected` |
| exact cannot span versions | `cross_version_exact_rule_rejected` |
| same version requires exact | `same_version_nonexact_rule_rejected` |
| lifecycle forward progression | three lifecycle positive scenarios |
| lifecycle skip/reverse/no-op rejection | three lifecycle negative scenarios |
| undefined enum fail closed | lifecycle and compatibility enum scenarios |
| canonical version grammar | version negative scenarios |
| canonical owner identity | `invalid_owner_identifier_fails_closed` |
| canonical SHA-256 | length/lowercase/character scenarios |
| immutable snapshot surface | `snapshot_is_immutable_surface` |
| deterministic snapshot | `snapshot_sorted_and_deterministic` |
| accepted mutation changes digest | `snapshot_mutation_changes_digest` |
| rejected mutation preserves state | `rejected_operation_does_not_change_snapshot` |
| zero-Application validity | `zero_application_neutrality` |
| independent Applications/owners | `two_independent_schema_owners` |
| payload meaning stays opaque | `payload_meaning_remains_opaque` |
| registry grants no authority | `registry_does_not_grant_authority` |
| WP-01 identity reused | `wp01_schema_identity_is_reused` |

The verifier contains 42 scenarios. WP-03 through WP-10 remain unimplemented and unauthorized.
