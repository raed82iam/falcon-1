# FCR-0235 — Stage 16 FIL / Service Bus Plug-and-Play Integration Hold

Date: 2026-08-17
Branch: `web-development`
Scope: `applications/shared/web/**`

## Current state

The Shared Web Stage 16 adapter currently present in source is a fail-closed compatibility adapter around the accepted Foundation `SecurityContextProjection` semantics. It is not the final Falcon-native runtime integration binding.

Project Owner clarified that Shared Web is intended to integrate with Falcon as plug-and-play and that Falcon communication rules, including FIL and the applicable platform communication boundaries, must be preserved.

Fresh FCR review found:

- FCR-0080 confirms that the generic Falcon communication/Application-contract boundary exists and explicitly includes FIL/delivery requirements at design level.
- FCR-0016 confirms the accepted Stage 14 canonical exact artifact-consumption boundary for immutable artifact identity/version/digest/evidence/compatibility.
- FCR-0152 defines the accepted Stage 16 authoritative identity/session/MFA semantics but does not identify an exact Shared Web FIL envelope/schema, Service Bus/runtime route, plug-and-play discovery contract, or exact canonical runtime-consumption identity.

Therefore FCR-0235 was opened with `Waiting On: FOUNDATION` to obtain the exact Foundation-owned integration contract or governed residual implementation placement.

## Controlling rule

Until FCR-0235 is dispositioned:

- Web SHALL NOT claim direct consumption of `Foundation.IdentityRuntime` as the final integration architecture.
- Web SHALL NOT invent a private endpoint, hidden direct call, or one-off coupling.
- Web SHALL NOT bypass FIL, Service Bus, public-contract, admission, publication, or canonical-consumption rules where they apply.
- The existing Stage 16 Web adapter SHALL remain compatibility/fail-closed preparation only.
- FCR-0152 final Web runtime binding/verification and closure are blocked on the exact Foundation response.

## Required plug-and-play properties

The final integration must be replaceable and governed, including exact producer/consumer identity, schema/version compatibility, freshness/revocation semantics, authoritative session truth, fail-closed unavailability/incompatibility handling, and no implicit business authority.

Mandatory distinctions:

`PLUG_AND_PLAY != IMPLICIT_TRUST`

`AUTHENTICATION != AUTHORIZATION`

`ROLE_FACT != AUTHORITY_DECISION`

`FOUNDATION_SECURITY_CONTEXT != WEB_SURFACE_GRANT`

`TECHNICAL_CONSUMPTION != BUSINESS_AUTHORITY`

`PUBLICATION != ACTIVATION`

## FCR state

FCR-0235: `SUBMITTED / Waiting On: FOUNDATION`

FCR-0152: `Waiting On: FOUNDATION` pending FCR-0235 disposition, then return to Web for exact final binding/verification.
