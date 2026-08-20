# Stage 5 WP-01 Requirement-to-Verifier Traceability

| Requirement | Verifier scenarios |
|---|---|
| Five message kinds | positive_command/query/response/event/notice |
| Deterministic identity | deterministic_digest |
| Deterministic equality | deterministic_equality, one_field_mutation_unequal |
| Typed identity separation | typed_identifier_separation |
| Immutable envelope/value objects | reflection_immutability |
| Message kind binding | kind_substitution_changes_identity |
| Classification binding | classification_substitution_changes_identity |
| Producer/recipient binding | producer_mutation_detected, recipient_mutation_detected |
| Schema identity/version binding | schema_identity_mutation_detected, schema_version_mutation_detected |
| Authority/provenance binding | authority_mutation_detected, provenance_mutation_detected |
| Idempotency/delivery/retry binding | idempotency_mutation_detected, delivery_attempt_mutation_detected, retry_lineage_mutation_detected |
| Payload integrity | payload_mutation_rejected_at_construction |
| Correlation/causation distinction | correlation_causation_rejected_at_construction |
| UNKNOWN preserved | unknown_remains_unknown |
| Malformed outcome fail-closed | invalid_outcome_code, blank/noncanonical reason |
| Invalid enums fail-closed | invalid_kind, invalid_classification |
| Canonical grammar | invalid_identifier, invalid_message_type, invalid_schema_version |
| SHA-256 grammar | invalid_sha_length/lowercase/character |
| UTC/expiry rules | non_utc_time, expiry_order |
| Zero-Application validity | zero_application_neutrality |
| Multi-Application independence | two_independent_application_identities |
| Legacy compatibility | legacy_fil_envelope_preserved |
