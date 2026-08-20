# Stage 5 WP-08 — Implementation Boundary

**Work Package:** Stage 5 WP-08 — Cryptographic Message Protection  
**Status:** IMPLEMENTATION AUTHORIZED / IN PROGRESS  
**Branch:** `foundation-development`

## Production Boundary

WP-08 production implementation is isolated in:

- `src/Foundation.MessageProtection/Foundation.MessageProtection.csproj`
- `src/Foundation.MessageProtection/MessageProtection.cs`

The production project has zero project references. It therefore cannot acquire Application, trading, predecessor-runtime, resource-governance, egress, or lifecycle authority through dependency coupling.

## Verification Boundary

Dedicated verifier:

- `verification/Falcon.Stage5.WP08.Verifier/Falcon.Stage5.WP08.Verifier.csproj`
- `verification/Falcon.Stage5.WP08.Verifier/Program.cs`

The verifier references only `Foundation.MessageProtection`.

## Controlled Integration

The production project and verifier are integrated into:

- `Falcon.Foundation.ControlledProjectFoundation.slnx`
- `tests/Falcon.Foundation.Architecture.Tests/Program.cs`
- `.github/workflows/foundation-ci.yml`

The architecture integration is additive. The change from the pre-WP08-integration harness to the WP08-aware harness contains additions only and removes no predecessor architecture gate.

## Cryptographic Boundary

The current implementation uses the .NET platform `AesGcm` primitive under a replaceable governed profile identified as `AES-256-GCM`, with a 256-bit key, 96-bit nonce and 128-bit authentication tag.

WP-08 authenticates the exact `ProtectedMessageContext` as Additional Authenticated Data. The bound context contains canonical message identity/digest, producer, recipient scope, information classification, schema identity/version, optional route/delivery/event identities and event classification, correlation/causation, protection-policy identity and observation time.

## Key Boundary

- `CryptographicKeyReference` carries non-secret key metadata only.
- `ICryptographicKeyMaterialResolver` provides key bytes only at the narrow operation boundary.
- Real long-term key custody, vault/KMS/HSM/PKI and credential-management infrastructure are outside WP-08.
- Secret/private key bytes may not be embedded in source, ordinary configuration, messages, logs or verification evidence.

## Security Boundary

A successful cryptographic verification proves only that the supplied ciphertext/tag verified under the specified key/profile and exact authenticated context. It does not prove:

- message business truth;
- authority;
- admission;
- route eligibility;
- delivery success;
- event truth;
- Application action; or
- financial correctness.

## Later-Work Holds

WP-08 does not authorize:

- WP-09 lifecycle/package attachment;
- WP-10 integrated closure;
- Internet egress;
- Live credential/broker-route enforcement;
- resource governance;
- Application business logic;
- deployment/runtime/baseline activation; or
- external connectivity.

## Current Static Condition

One pre-validation hardening item is still under review: nonce-provider exception handling currently contains a debugger-dependent exception filter. Runtime validation SHALL NOT begin until this item is remediated or formally demonstrated non-material by the final static security review.
