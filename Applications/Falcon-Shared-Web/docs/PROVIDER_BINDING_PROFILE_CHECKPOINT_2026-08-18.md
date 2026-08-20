# Shared Web Provider Binding Profile Checkpoint

**Date:** 2026-08-18  
**Branch:** `web-development`  
**Status:** `WEB_BINDING_PROFILE_IMPLEMENTED / TARGETED_EXECUTABLE_PASS / AUTHORITATIVE_RUNTIME_BINDING_VALUES_PENDING`

## Scope

This checkpoint covers the Shared Web presentation-provider binding profile for:

- FCR-0173 Binance trade presentation stream;
- FCR-0174 Coinbase presentation stream;
- FCR-0175 Bybit presentation stream;
- FCR-0176 Alpaca IEX presentation stream;
- FCR-0177 Finnhub presentation stream;
- FCR-0196 Alpaca US-equity universe;
- FCR-0197 Alpaca historical bars;
- FCR-0198 Binance Spot universe;
- FCR-0199 Binance historical klines;
- FCR-0200 Binance broad-market mini ticker.

It also supports the parent FCR-0125 and FCR-0220 completion path.

## Implemented

New profile:

`src/core/provider-binding-profile.js`

The profile composes all ten exact Stage 12 routes and evaluates them through the existing fail-closed `evaluateProviderBindingReadiness(...)` boundary.

It requires runtime-supplied:

- exact Web principal identity;
- exact Web service role;
- per-route route-policy binding;
- per-route governed verification state;
- opaque Web credential references where required;
- explicit channel credential requirement for channel-dependent routes.

No value is invented by the profile.

```text
WEB_PROVIDER_BINDING_PROFILE != CONNECTIVITY_ACTIVATION
WEB_PRINCIPAL_STRING != AUTHORITY_UNLESS_AUTHORITATIVELY_SUPPLIED
CREDENTIAL_REFERENCE != SECRET
PUBLIC_ENDPOINT != UNRESTRICTED_EGRESS_AUTHORITY
WEB_PRESENTATION_DATA != FSATS_OPERATIONAL_DATA
WEB_RAW_DISPLAY_DATA -/-> FSATS_ANALYSIS_INPUT
ROUTE_AUTHORIZED != CONNECTION_EXECUTED
```

## Targeted executable verification

A standalone exact logic harness executed:

```text
NODE_CHECK_PROVIDER_BINDING_PROFILE = PASS
PROVIDER_BINDING_PROFILE_TESTS = 5/5 PASS
```

Covered:

- all ten FCR identities exactly once;
- complete fail-closed state with no runtime principal/policy context;
- complete readiness metadata remains `connectivityActivated=false`;
- raw secret-like values rejected as credential references;
- missing credential references rejected;
- Coinbase channel-dependent authentication stays fail-closed when requirement is unknown.

## Current blocker

The current Web source does not provide an authoritative runtime source for the actual Web principal/service-role, route-policy decisions or provider credential-reference bindings.

Therefore this checkpoint does **not** mark FCR-0125, FCR-0220 or FCR-0173..0177/FCR-0196..0200 complete.

```text
WEB_BINDING_PROFILE = IMPLEMENTED
WEB_BINDING_PROFILE_TARGETED_EXECUTABLE = PASS
AUTHORITATIVE_RUNTIME_BINDING_VALUES = PENDING
CONNECTIVITY_ACTIVATION = NOT_CLAIMED
PROVIDER_FCR_CLOSURE = NOT_ELIGIBLE_YET
```
