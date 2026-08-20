# Stage 5 WP-08 — Implementation Design

**Work Package:** Stage 5 WP-08 — Cryptographic Message Protection  
**Status:** IMPLEMENTATION AUTHORIZED / IN PROGRESS  
**Branch:** `foundation-development`  
**Predecessors:** WP-01 through WP-07 accepted and closed

## 1. Design Goal

Provide a bounded, Application-neutral cryptographic message-protection layer that can protect and verify already-governed communication artifacts without changing their business meaning or predecessor authority semantics.

The implementation SHALL use approved platform cryptography and SHALL NOT invent Falcon-specific cryptographic algorithms or protocols.

## 2. Architectural Position

WP-08 sits after the canonical/admission/routing/delivery/event-truth boundaries as a protection concern. It may bind cryptographic context to predecessor evidence, but it may not reinterpret those predecessors.

Conceptually:

```text
Canonical FIL / Message Identity
        ↓
Admission / Route / Delivery / Event Context
        ↓
WP-08 Protected Context Binding
        ↓
Approved Cryptographic Profile
        ↓
Authenticated Protection / Verification
        ↓
Cryptographic Evidence
```

Cryptographic success means only that the configured protection claim verified for the exact protected context. It does not mean that an Application action is authorized, correct, executed, or successful.

## 3. Core Types

The production boundary SHOULD expose typed immutable objects equivalent in responsibility to the following:

### `CryptographicProtectionProfile`

Binds:
- profile ID;
- profile version;
- approved algorithm suite identity;
- nonce/IV size policy;
- authentication-tag size policy;
- allowed key classes;
- profile lifecycle status: Approved / Deprecated / Prohibited / Disabled;
- minimum effective time and optional expiry;
- policy evidence identity.

The first implementation SHALL use an approved .NET platform AEAD primitive and keep the profile replaceable. The primitive SHALL NOT be encoded as business meaning.

### `CryptographicKeyReference`

Carries metadata only:
- key reference ID;
- key class;
- owner identity;
- key version;
- status: Active / NotYetActive / Revoked / Retired / Disabled / Unknown;
- effective time;
- optional expiry;
- permitted profile IDs;
- permitted producer/recipient/scope bindings where applicable;
- evidence identity.

Private/secret key bytes SHALL NOT be embedded in this object, ordinary configuration, logs, or evidence.

### `ProtectedMessageContext`

Binds the exact cryptographic protection claim to non-secret canonical context such as:
- canonical message ID;
- canonical message digest;
- producer identity;
- intended recipient or recipient scope;
- information classification;
- schema identity/version where applicable;
- route decision identity where applicable;
- delivery decision identity where applicable;
- event identity/classification where applicable;
- correlation/causation identifiers where applicable;
- protection-required policy identity;
- issued/observed time as needed for validity evaluation.

Fields that do not apply to a protected object SHALL be represented explicitly and deterministically, not ambiguously omitted in a way that permits substitution.

### `ProtectedMessagePackage`

Carries only non-secret protection material required to verify the message:
- profile identity/version;
- key reference identity/version;
- nonce/IV;
- ciphertext;
- authentication tag if the platform representation separates it;
- protected-context digest;
- package identity;
- protection evidence identity.

### `CryptographicProtectionDecision`

Result of protection request:
- Protected / Rejected;
- reason code;
- profile identity;
- key-reference identity;
- protected-context identity/digest;
- package identity when successful;
- deterministic evidence identity;
- no secret material.

### `CryptographicVerificationDecision`

Result of verification request:
- Verified / Rejected;
- exact reason code;
- verified context identity/digest;
- profile/key reference identity;
- recovered plaintext only to the authorized caller boundary when verification succeeds;
- deterministic evidence identity;
- no secret material in diagnostics.

## 4. Cryptographic Primitive Boundary

The initial profile SHALL use a standard platform authenticated-encryption primitive suitable for message-level AEAD. The implementation SHALL:

- use .NET platform cryptography only;
- use randomly generated cryptographic nonces of the required size and validate supplied nonces during verification;
- authenticate the exact protected context as Additional Authenticated Data (AAD) or an equivalent approved binding;
- reject authentication-tag failure without exposing partial plaintext;
- reject unsupported profile/algorithm/parameter combinations;
- permit future profile replacement without changing business contracts.

No custom cipher, MAC construction, key derivation construction, signature scheme, or wire protocol may be introduced by WP-08.

## 5. Key Material Boundary

WP-08 needs cryptographic key bytes at the narrow cryptographic operation boundary, but it SHALL NOT become the long-term key-management system.

The implementation SHALL separate:

1. governed key reference/status metadata;
2. a bounded key-material resolver/provider interface used by the cryptographic primitive; and
3. cryptographic evidence that records only the key reference/version, never the secret bytes.

Verifier fixtures may use ephemeral in-memory test key material. Production source SHALL NOT embed real keys.

A future KMS/HSM/vault adapter may implement the resolver interface under separate authority without requiring message-protection semantics to be redesigned.

## 6. Protection Policy

Protection requests SHALL fail closed when any required condition is invalid, including:

- missing protection profile;
- unknown profile;
- prohibited/disabled/deprecated profile when policy disallows it;
- unsupported parameters;
- missing key reference;
- wrong key class/profile binding;
- key not yet active;
- key expired;
- key revoked/retired/disabled/unknown;
- producer or recipient/scope binding mismatch;
- invalid/missing canonical message identity/digest;
- invalid required predecessor context binding;
- missing required classification;
- protection policy requiring encryption but no usable key/material resolver result;
- nonce generation failure;
- cryptographic provider failure.

There SHALL be no plaintext fallback when protection is required.

## 7. Verification Policy

Verification SHALL fail closed for, at minimum:

- unknown/prohibited/disabled/downgraded profile;
- profile version mismatch;
- key reference/version mismatch;
- revoked/expired/not-yet-valid/disabled key;
- wrong producer;
- wrong intended recipient or scope;
- wrong classification;
- wrong canonical message identity/digest;
- wrong route/delivery/event context where bound;
- wrong correlation/causation where bound;
- protected-context digest mismatch;
- malformed nonce/tag/ciphertext;
- authentication-tag/integrity failure;
- package identity mismatch;
- policy evidence mismatch;
- unavailable key material.

Failure SHALL NOT release plaintext.

## 8. Anti-Substitution and Anti-Replay Boundary

WP-08 SHALL prevent cryptographic substitution by binding all material non-secret context into the authenticated protection context.

WP-08 SHALL NOT redefine WP-07 replay truth or WP-06 idempotency. If predecessor replay/test/operational classification or event identity is present and material, WP-08 SHALL bind it so cryptographic protection cannot be copied to a different classification/context undetected.

Nonce reuse for the same key/profile combination SHALL be treated as invalid where required by the selected primitive. The implementation/verifier SHALL include explicit nonce-reuse protection or detection at the bounded operation scope.

## 9. Evidence and Diagnostics

Cryptographic evidence SHALL be deterministic where inputs are deterministic except for intentionally random protection outputs such as nonces/ciphertext. Evidence SHALL preserve enough identity to reconstruct what was protected/verified without storing secret/private key material or sensitive plaintext.

Diagnostics SHALL be redaction-safe. Authentication failures SHALL use bounded reason codes and SHALL NOT expose key bytes, plaintext fragments, or sensitive cryptographic internals.

## 10. Expected Rejection Reasons

The implementation SHALL use exact bounded reason identifiers. The initial set may include:

```text
CRYPTO_PROFILE_REQUIRED
CRYPTO_PROFILE_UNKNOWN
CRYPTO_PROFILE_PROHIBITED
CRYPTO_PROFILE_DISABLED
CRYPTO_PROFILE_DEPRECATED
CRYPTO_PROFILE_NOT_YET_EFFECTIVE
CRYPTO_PROFILE_EXPIRED
CRYPTO_PARAMETERS_UNSUPPORTED
CRYPTO_KEY_REFERENCE_REQUIRED
CRYPTO_KEY_REFERENCE_UNKNOWN
CRYPTO_KEY_CLASS_MISMATCH
CRYPTO_KEY_PROFILE_MISMATCH
CRYPTO_KEY_NOT_YET_ACTIVE
CRYPTO_KEY_EXPIRED
CRYPTO_KEY_REVOKED
CRYPTO_KEY_RETIRED
CRYPTO_KEY_DISABLED
CRYPTO_KEY_SCOPE_MISMATCH
CRYPTO_CONTEXT_INVALID
CRYPTO_CONTEXT_MISMATCH
CRYPTO_RECIPIENT_MISMATCH
CRYPTO_CLASSIFICATION_MISMATCH
CRYPTO_PREDECESSOR_BINDING_MISMATCH
CRYPTO_KEY_MATERIAL_UNAVAILABLE
CRYPTO_NONCE_INVALID
CRYPTO_NONCE_REUSE
CRYPTO_PACKAGE_MALFORMED
CRYPTO_PACKAGE_IDENTITY_MISMATCH
CRYPTO_AUTHENTICATION_FAILED
CRYPTO_PROTECTION_REQUIRED
CRYPTO_PROVIDER_FAILURE
```

Exact names may be tightened during implementation but SHALL remain deterministic and verifier-covered.

## 11. Verification Strategy

The dedicated WP-08 verifier SHALL include positive and adversarial scenarios for:

- approved authenticated protection and verification;
- exact plaintext recovery only after successful verification;
- ciphertext tampering;
- tag tampering;
- nonce tampering/reuse;
- wrong key/version;
- revoked/expired/not-yet-active key;
- prohibited/deprecated/downgraded profile;
- wrong producer/recipient/scope;
- classification substitution;
- canonical message digest substitution;
- route/delivery/event identity substitution where bound;
- replay/test versus operational classification substitution where bound;
- malformed package;
- unavailable key material;
- no plaintext fallback;
- no plaintext on failed verification;
- evidence redaction/no secret material;
- deterministic context/package/evidence identities as applicable;
- Application neutrality and payload opacity; and
- predecessor regression preservation.

## 12. Integration Boundary

Expected production project:

`src/Foundation.MessageProtection/Foundation.MessageProtection.csproj`

Expected verifier:

`verification/Falcon.Stage5.WP08.Verifier/Falcon.Stage5.WP08.Verifier.csproj`

The project SHALL be integrated additively into the controlled solution, architecture harness, security harness, and CI without weakening existing WP-01 through WP-07 gates.

## 13. Current State

```text
STAGE5_WP08_IMPLEMENTATION = AUTHORIZED_AND_IN_PROGRESS
WP08_SCOPE_REVIEW = COMPLETE
WP08_IMPLEMENTATION_DESIGN = DEFINED
WP08_RUNTIME_VALIDATION = NOT_YET_EXECUTED
WP08_OWNER_ACCEPTANCE_AND_CLOSURE = NOT_GRANTED
STAGE5_WP09_THROUGH_WP10 = UNAUTHORIZED
```
