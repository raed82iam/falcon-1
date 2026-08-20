# FCR-0095 Runtime Binding Hardening Checkpoint

**Date:** 2026-08-18  
**Branch:** `web-development`  
**Status:** `WEB_RUNTIME_BOUNDARIES_HARDENED / EXECUTABLE_SUITE_PENDING / EXTERNAL_RUNTIME_BINDINGS_STILL_UNAVAILABLE`

## Completed Web-owned hardening

### Screenshot scanner

`src/incidents/screenshot-upload-controller.js` no longer discovers `globalThis.FalconGovernedScreenshotScanner`.
A governed scanner must be explicitly injected. Missing scanner remains:

`GOVERNED_SCREENSHOT_SCANNER_UNAVAILABLE`

### Local voice

`src/voice/browser-local-voice-binding.js` no longer discovers `globalThis.FalconLocalVoiceRuntime`.
Whisper.cpp/Piper capability must be explicitly injected by composition. Missing capability remains `UNAVAILABLE`; no remote or paid fallback exists.

### Incident persistence

`src/incidents/incident-persistence-binding.js` separates preview-local storage from authoritative production persistence:

- PREVIEW may use local IndexedDB;
- AUTHORITATIVE requires an explicit authoritative tenant-scoped persistence binding;
- missing/invalid production binding fails closed;
- production binding must state `businessAuthorityGranted=false`.

The real `app.js` path now uses this policy and cannot silently reinterpret browser-local persistence as production tenant storage.

### Support request transport

`src/core/ports/incident-support-transport-port.js` defines a Web-owned fail-closed Support request transport boundary.

The incident UI now records/displays `Support requested` only after an exact transport decision verifies:

- accepted and delivered;
- exact incident identity;
- exact current principal identity;
- exact current session identity;
- request identity and evidence reference;
- `authorityGranted=false`.

Unavailable or rejected transport leaves the incident in its prior state and tells the customer that Support was not delivered.

```text
LOCAL_SUPPORT_EVENT != SUPPORT_REQUEST_DELIVERED
SUPPORT_REQUEST_DELIVERED != SUPPORT_AVAILABLE
SUPPORT_AVAILABLE != SUPPORT_TAKEOVER
SUPPORT_TAKEOVER != BUSINESS_AUTHORITY
TRANSPORT_ACCEPTANCE != PORTFOLIO_OR_TRADING_AUTHORITY
```

## Tests added

- `tests/incident-runtime-binding-policy.test.mjs`
- `tests/incident-support-transport.test.mjs`

These tests cover authoritative persistence fail-closed behavior, explicit local voice binding, Support transport default-unavailable behavior, exact session/incident binding, and authority-leak rejection.

## Still pending

No external runtime capability was fabricated. Remaining FCR-0095 production completion still includes, where applicable:

- actual governed screenshot scanner implementation/binding;
- actual local Whisper.cpp/Piper executable assets/binding;
- actual authorized Support/contact transport implementation;
- authoritative production tenant persistence binding;
- full current-HEAD executable/browser verification.

`FCR0095_COMPLETE = NO`
