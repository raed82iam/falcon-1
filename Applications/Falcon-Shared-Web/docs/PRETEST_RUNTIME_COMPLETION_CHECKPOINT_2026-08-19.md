# Shared Falcon Web — Pre-Test Runtime Completion Checkpoint

Date: 2026-08-19
Branch: `web-development`
Scope: `applications/shared/web/**`
Status: `SOURCE_RUNTIME_BINDING_PREP_COMPLETE / FINAL_EXECUTABLE_RETEST_REQUIRED`

## Purpose

This checkpoint closes the remaining Web-owned source preparation that can be completed without inventing deployment identities, secret material, production storage, Support delivery, local voice executables, or route activation authority.

It does not claim live connectivity, deployment, business authority, Trading authority, broker authority or production readiness.

## FCR lifecycle cleanup completed before this checkpoint

The following FCRs were closed after their previously pending full current-HEAD Node gate passed 435/435 with `npm run check` PASS and a clean checkout:

- FCR-0076;
- FCR-0152;
- FCR-0235;
- FCR-0242.

FCR-0241, FCR-0237 and FCR-0238 were already Owner-accepted-and-closed after the same full-suite evidence.

## Provider runtime source completion

New source:

- `src/core/web-provider-runtime-policy.js`;
- `src/core/web-runtime-preflight.js`.

The exact Stage12 Shared Web presentation-route set is centralized across FCR-0173..0177 and FCR-0196..0200.

Source-owned policy facts now include:

- exact route-policy binding for the ten governed destinations;
- exact-route verification requirement;
- connectivity remains false;
- credential-bearing Web routes are limited to FCR-0176, FCR-0177, FCR-0196 and FCR-0197;
- Coinbase FCR-0174 is constrained to unauthenticated public presentation-market-data use; private/authenticated channels are outside this binding and require a new governed decision rather than silent widening.

Runtime facts that remain explicit external inputs:

- authoritative Shared Web provider service principal identity;
- authoritative Shared Web service role identity;
- opaque valid Web credential references for FCR-0176, FCR-0177, FCR-0196 and FCR-0197.

Secret bytes are not accepted by the Web binding policy.

## Incident runtime source completion

New source:

- `src/core/web-incident-runtime-policy.js`;
- `src/core/web-runtime-preflight.js`.

`src/app.js` now consumes the centralized incident runtime policy instead of wiring persistence/scanner/voice/Support nulls independently.

Authoritative incident readiness requires all of:

- exact principal + tenant + session identity;
- authoritative tenant-scoped production persistence binding;
- governed screenshot scanner;
- governed Support transport;
- explicitly injected local Whisper.cpp + Piper runtime.

Authoritative mode cannot silently fall back to Preview IndexedDB. Missing scanner/Support/local voice dependencies remain fail closed. Remote paid voice fallback remains prohibited.

## AI presentation integration completed

`src/features/ai/analysis-presentation-policy.js` is now consumed by `src/features/ai/ai.js`.

Detailed analysis is shown only when the Application-supplied result is `COMPLETED` and truth, freshness, completeness and synthesis are all current/complete. Stale completed results, partial results and clarification-required results cannot expose full detailed projections.

## Live Voice requirement review

The governing Owner incident decision record was re-read. It defines:

- ordinary voice-message silence auto-stop = disabled;
- Live Voice Guidance silence tolerance before Falcon reply = 15 seconds after speech;
- no authoritative maximum Live Voice session duration is defined.

No unapproved timeout was invented.

## New pre-test coverage authored

Added tests:

- `tests/web-runtime-preflight.test.mjs`;
- `tests/web-provider-runtime-policy.test.mjs`;
- `tests/web-incident-runtime-policy.test.mjs`;
- `tests/ai-presentation-policy-integration.test.mjs`.

`npm run check` now includes:

- `src/core/web-provider-runtime-policy.js`;
- `src/core/web-incident-runtime-policy.js`;
- `src/core/web-runtime-preflight.js`.

## Current external/environment inputs that cannot be fabricated in source

```text
WEB_PROVIDER_SERVICE_PRINCIPAL = REQUIRED_FROM_GOVERNED_RUNTIME
WEB_PROVIDER_SERVICE_ROLE = REQUIRED_FROM_GOVERNED_RUNTIME
WEB_PROVIDER_CREDENTIAL_REFERENCES = REQUIRED_FOR_FCR0176/FCR0177/FCR0196/FCR0197
PRODUCTION_INCIDENT_PERSISTENCE = REQUIRED_FROM_GOVERNED_RUNTIME
GOVERNED_SCREENSHOT_SCANNER = REQUIRED_FROM_GOVERNED_RUNTIME
GOVERNED_SUPPORT_TRANSPORT = REQUIRED_FROM_GOVERNED_RUNTIME
LOCAL_WHISPER_CPP_PIPER_RUNTIME = REQUIRED_FROM_DEPLOYMENT_ENVIRONMENT
BROWSER_RTL_LTR_KEYBOARD_FOCUS_MOBILE = EXECUTABLE_TEST_REQUIRED
```

These are test/deployment environment bindings, not missing semantic behavior that Web can safely invent.

## Mandatory separation

```text
SOURCE_BINDING_READY != RUNTIME_IDENTITY_ISSUED
CREDENTIAL_REFERENCE_ID != SECRET_BYTES
ROUTE_POLICY_BOUND != CONNECTION_EXECUTED
PROVIDER_PREFLIGHT_READY != CONNECTIVITY_ACTIVATED
INCIDENT_PREFLIGHT_READY != BUSINESS_AUTHORITY
BROWSER_TEST_PASS != DEPLOYMENT_AUTHORITY
FULL_WEB_TEST_PASS != LIVE_ACTIVATION
```

## Next gate

Run a fresh full checkout-backed test on the new post-hardening HEAD. If Node checks pass, proceed to browser/runtime preflight on the same frozen candidate. Any failure is remediated before WP-24 final broad Red Team.