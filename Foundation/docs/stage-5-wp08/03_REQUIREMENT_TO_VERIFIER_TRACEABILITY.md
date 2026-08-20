# Stage 5 WP-08 — Requirement-to-Verifier Traceability

**Verifier:** `verification/Falcon.Stage5.WP08.Verifier`  
**Named Scenarios:** 48  
**Runtime Status:** NOT YET EXECUTED

## Traceability Summary

### Approved protection and recovery
- `approved_profile_protects`
- `approved_profile_verifies_and_recovers_exact_plaintext`
- `arbitrary_binary_payload_round_trips`

### Integrity/authentication tampering
- `ciphertext_tampering_rejected`
- `tag_tampering_rejected`
- `nonce_tampering_rejected`
- `wrong_key_material_authentication_rejected`
- `failed_verification_releases_no_plaintext`

### Context binding / anti-substitution
- `wrong_recipient_context_rejected`
- `wrong_classification_context_rejected`
- `wrong_message_digest_context_rejected`
- `wrong_route_context_rejected`
- `wrong_delivery_context_rejected`
- `wrong_event_context_rejected`
- `wrong_replay_classification_context_rejected`
- `wrong_correlation_context_rejected`
- `wrong_causation_context_rejected`
- `empty_optional_predecessor_bindings_are_deterministic`

### Profile governance / downgrade resistance
- `wrong_profile_version_rejected`
- `prohibited_profile_rejected`
- `disabled_profile_rejected`
- `deprecated_profile_rejected`
- `future_profile_rejected`
- `expired_profile_rejected`
- `unsupported_algorithm_rejected`
- `unsupported_parameters_rejected`

### Key-reference lifecycle / scope
- `wrong_key_class_rejected`
- `wrong_key_profile_rejected`
- `future_key_rejected`
- `expired_key_rejected`
- `revoked_key_rejected`
- `retired_key_rejected`
- `disabled_key_rejected`
- `unknown_key_rejected`
- `wrong_key_scope_rejected`
- `key_material_unavailable_rejected`
- `key_version_binding_enforced`

### Nonce safety
- `nonce_reuse_rejected`
- `invalid_nonce_size_rejected`

### Deterministic identities / evidence
- `context_digest_is_deterministic`
- `profile_identity_is_deterministic`
- `key_reference_identity_is_deterministic`
- `package_identity_changes_with_ciphertext`
- `profile_binding_changes_package_identity`

### Secret/plaintext evidence hygiene
- `protection_evidence_contains_no_plaintext`
- `protection_evidence_contains_no_key_material`
- `verification_evidence_contains_no_key_material`

### Application neutrality / payload opacity
- `application_names_do_not_change_semantics`
- `arbitrary_binary_payload_round_trips`

## Security Requirement Mapping

- SEC-001 REQ-020: approved message-level authenticated encryption → positive protect/verify + tamper scenarios.
- SEC-001 REQ-022: identity/recipient/classification/material context binding → context-substitution scenarios.
- SEC-001 REQ-023/025: key lifecycle and invalid/revoked context rejection → key/profile lifecycle scenarios.
- SEC-001 REQ-024: no secret key material in evidence → evidence-hygiene scenarios.
- SEC-001 REQ-026: fail closed / no plaintext fallback → failed-verification and unavailable-key scenarios.
- SEC-001 REQ-027: replaceable governed profile → profile identity/version/status scenarios.
- SEC-001 REQ-028: no custom cryptography → architecture/static review plus unsupported-algorithm scenario.
- SEC-002 identity/integrity/canonical representation obligations → deterministic profile/key/context/package identity scenarios.

## Validation Gate

This traceability document records intended verifier coverage only. It does not claim PASS. Runtime validation remains blocked until static hardening and governance reconciliation are complete.
