# Programmer Manual — Falcon Shared Web

**Audience:** Programmer / Web Engineer / Maintainer  
**Language:** English  
**Scope:** Architecture, capabilities, boundaries, development workflow, testing, and Foundation onboarding preparation for Shared Falcon Web

## 1. Application identity

Package: `falcon-shared-web`  
Current package version: `0.1.0`  
Module mode: ES Modules (`"type": "module"`)  
Owned workstream scope:

`applications/shared/web/**`

Shared Web is an independently governed Falcon Application. Its job is public/customer/owner/support presentation plus governed request submission. It does not own Foundation internals, FSATS operational truth, deployment, connectivity, trading authority, or business authority.

## 2. Architecture boundaries

Never break these invariants:

- Presentation is not Authority.
- Web must not directly import Foundation or ordinary Application internals.
- A role fact alone must not create a session or route authority.
- Web market data must not become FSATS operational input.
- Web provider credentials are distinct from FSAPMA and customer broker credentials.
- Credential reference IDs are not secret bytes.
- Registered is not Activated.
- Route policy bound is not connection executed.
- Request sent is not action accepted and is not action completed.

## 3. Important source directories

### `src/core/`

Contains stable policies, boundaries, ports, readiness logic, and preflight logic, including:

- runtime port
- provider binding/profile/readiness
- market-data plan
- Web provider runtime policy
- Web incident runtime policy
- Web runtime preflight
- Web awareness model
- Owner request router
- Foundation plug-ready preflight

### `src/core/ports/`

Stable Web-facing interfaces for external truth and governed action channels:

- FSATS runtime port
- Falcon system runtime port
- Web market-data port
- Owner AI emergency port
- Owner update governance port
- incident Support transport port

A port defines shape and boundary only. It does not imply a real transport is currently bound.

### `src/adapters/`

Adapters convert governed Foundation/FSATS contract material into Web-safe projections while preserving authority separation. Current adapter families include Foundation FIL, identity/session, recovery, governance, FSATS portfolio, analysis, incident, and Project Owner entitlement adapters.

### `src/composition/`

Composition layer:

- `runtime-bootstrap.js`
- `app-context.js`
- `fsats-authoritative-data.js`
- `shell.js`
- `incident-ui-runtime.js`
- `app-view-registry.js`
- `app-ui-bindings.js`
- `owner-surfaces.js`

Composition injects dependencies. It must never fabricate missing truth.

### `src/features/`

Feature surfaces include:

- Falcon public
- FSATS public
- My Applications
- FSATS workspace
- Portfolio
- Activity
- Markets
- Advisory Markets
- AI
- Notifications
- Settings
- Catalog
- Owner Home
- Owner Command Center
- Owner Approvals
- Owner Provider Actions
- Owner AI Emergency
- Customer Incident

### `src/incidents/`

Incident timeline, content safety, persistence, controller, accessibility, and screenshot handling.

### `src/voice/`

Voice policy, browser microphone, local voice runtime, live voice session, and incident voice controller.

### `src/security/`

Central safe-output boundary such as `safe-html.js`.

### `src/design-system/`

Presentation helpers and reusable semantic/accessibility primitives.

### `tests/`

Node test suite. All `*.test.mjs` files are executed by `node --test`.

### `tools/`

Verification tooling, including the browser verification server.

### `governance/`

Machine-readable onboarding material:

- `SHARED_WEB_APPLICATION_ADMISSION_MANIFEST_V1.json`
- `WEB_FOUNDATION_PLUG_READY_PREPARATION_V1.json`

## 4. Route registry

Canonical routes are defined in:

`src/platform/navigation/routes.js`

### Public

- `home`
- `apps`
- `login`
- `register`
- `fsats`

### User

- `my-apps`
- `trader`
- `markets`
- `advisory-markets`
- `portfolio`
- `activity`
- `ai`
- `notifications`
- `settings`

### Owner

- `owner-home`
- `owner`
- `owner-apps`
- `owner-incidents`
- `owner-approvals`
- `owner-ai-emergency`
- `owner-provider-actions`
- `owner-users`
- `owner-audit`
- `owner-settings`
- `owner-simulator`

Unknown hashes normalize to public Home rather than executing an unknown route.

## 5. Authentication and authorization

`src/auth.js` and Foundation identity/session adapters preserve fail-closed behavior.

Rules:

- Missing authoritative identity never creates a synthetic user.
- Role alone never creates route authority.
- Owner routes require the Owner surface grant.
- Customer routes require the applicable customer access/entitlement.
- Project Owner does not automatically inherit customer FSATS access.
- Support or unknown roles are not inferred into Owner/customer surfaces.
- A Web authoritative session carrying business authority is invalid.

## 6. Preview versus Authoritative mode

`runtime-bootstrap.js` keeps these states separate:

- Preview requires explicit preview data.
- Authoritative mode rejects preview data.
- Partial authoritative binding fails closed rather than silently falling back to Preview.
- Arbitrary unmarked objects cannot masquerade as authoritative contract data.
- Raw secret-shaped configuration is rejected.
- Opaque credential references remain allowed where specifically required.

## 7. Standard and VIP tiers

`src/features/my-applications/subscription-presentation.js` supports:

- `STANDARD`
- `VIP`

Entitlement is not inferred from the tier name. A tier is current only when its model is authoritative, entitled, and current.

Do not invent:

- pricing
- trial state
- upgrade state
- VIP-specific benefits

## 8. Market data and provider routes

Shared Web provider presentation routes are separate from FSAPMA operational provider routes.

Opaque Web credential references are required only for:

- FCR-0176 Alpaca IEX
- FCR-0177 Finnhub
- FCR-0196 Alpaca assets
- FCR-0197 Alpaca bars

Public no-credential presentation routes:

- FCR-0173 Binance trade stream
- FCR-0174 Coinbase public feed
- FCR-0175 Bybit public spot
- FCR-0198 Binance exchangeInfo
- FCR-0199 Binance klines
- FCR-0200 Binance miniTicker

Raw secret bytes must never enter ordinary Web state.

## 9. Portfolio and activity truth rules

- Missing or nullable values remain missing instead of being zero-filled.
- Unknown broker outcomes stay unknown.
- Pagination metadata fails closed when contradictory.
- `PARTIALLY_FILLED` remains distinct from `FILLED`.
- Simulator/shadow truth must never become broker truth.

## 10. AI presentation

Web presents Application-supplied analysis only.

- `CURRENT + COMPLETE` may expose full detail.
- Stale results are restricted.
- Partial results do not expose full detail.
- Needs Clarification cannot claim resolved identity.
- Material disagreement cannot be silently converted into COMPLETE.

## 11. Incident runtime

Production incident readiness requires authoritative bindings for:

- principal / tenant / session
- tenant-scoped persistence
- governed screenshot scanner
- governed Support transport
- local Whisper.cpp/Piper runtime

Authoritative runtime must not fall back to preview persistence or preview facilities when these are missing.

Rules:

- credentials in chat are rejected
- screenshots containing secrets are rejected
- screenshots without governed scan evidence fail closed
- Support takeover requires explicit authoritative capability
- Support transport remains transport-only and does not create authority

## 12. Voice

- Browser microphone starts only after explicit user request.
- Ordinary voice messages do not auto-stop on silence.
- Live voice uses the governed patience rule before Falcon reply.
- Local voice runtime must be explicitly injected.
- Local-only policy does not silently introduce a remote fallback.

## 13. Owner governance

Owner governance ports support governed request families such as:

- policy management
- standing preapproval evaluation
- rollback order

Web must never produce:

- self-approval
- hidden auto-accept
- rollback-executed claims without authoritative outcome
- restored authority from a status label alone

## 14. Owner AI Emergency

Code rules include:

- ambiguous target fails closed
- `ALL_AI` is reserved for Global AI Kill
- Global AI Kill preserves Safe Core and is not Falcon shutdown
- accepted outcome is not completed outcome
- release/revival is not local Web authority

## 15. XSS and content safety

Untrusted content must pass through output-encoding helpers. Tests cover hostile HTML/SVG/iframe-like payloads across incident, portfolio, activity, AI, and Owner projections.

Do not introduce unencoded `innerHTML` from user/external input. Use existing primitives/helpers or encode before composition.

## 16. Accessibility and localization

Preserve:

- Arabic and English
- RTL Arabic presentation
- keyboard-native navigation
- visible focus
- skip link
- accessible labels
- semantic headings and regions
- reduced-motion support
- forced-colors support
- mobile viewport containment

## 17. Tests and verification

From:

`applications/shared/web`

run:

```powershell
npm.cmd test
npm.cmd run check
npm.cmd run verify:browser
```

`npm test` runs all `tests/*.test.mjs`.  
`npm run check` runs syntax checks across critical source modules.  
`verify:browser` launches the browser verification server for manual browser checks.

The last executable plug-ready verification before these manual-only documentation commits was:

```text
HEAD = 38c5db80adc52e6555ebe8aee821d83659c513d3
TESTS = 479
PASS = 479
FAIL = 0
npm run check = PASS
WORKTREE = CLEAN
```

Adding these manuals changes Git HEAD but does not change executable source.

## 18. Foundation full plug-ready preparation

Shared Web materializes exactly:

```text
APPLICATION = FALCON_SHARED_WEB
ADMISSION_CANDIDATES = 1
RUNTIME_REGISTRATION_TEMPLATES = 1
REQUEST_PAIRS = 1
```

Bound preparation baseline:

- CON-023 1.1
- CON-001 1.0 dependency
- FDN-006 1.0
- FDN-007 1.0

Full plug-ready contract preflight is verified.  
Full plug-ready preflight is verified by composition.  
Foundation change required is false.

These remain bind-at-operation rather than preparation gaps:

- exact Web artifact identity
- canonical admission evidence
- lifecycle attach eligibility/decision identity
- Foundation resource grants
- authoritative observed-at
- provider service principal/role
- opaque credential references
- principal/tenant/session
- production persistence
- screenshot scanner
- Support transport
- local voice runtime

## 19. Actual-link boundary

Full plug-ready does not mean actual execution:

- Actual Admission is not executed.
- Canonical Runtime Registration is not executed.
- Activation is not executed.
- Deployment is not executed.
- Provider connectivity is not executed.
- Business and Trading authority are not granted.

Do not change these states from Web without the separately authorized governed operation.

## 20. Adding a feature safely

1. Identify surface ownership: Public, User, or Owner.
2. Identify the authoritative truth source and contract.
3. If the dependency is external, define a stable port first.
4. Add an adapter that preserves authority separation.
5. Keep the feature presentation-only over supplied projection data.
6. Inject dependencies in composition instead of manufacturing them.
7. Add fail-closed tests for missing, malformed, stale, contradictory, and authority-bearing input.
8. Add XSS/security/accessibility tests as applicable.
9. Run `npm test` and `npm run check`.
10. If UI behavior changed, run browser verification.
11. Cross-workstream dependencies go through FCR. Do not edit Foundation or another Application from the Web workstream.

## 21. Working documentation

Start with:

- `docs/README.md`
- `docs/CURRENT/README.md`
- `docs/MASTER_WEB_PLAN_V2_2026-08-17/`
- `docs/manual/`

The FCR Issue body is the canonical current lifecycle header; comments are the chronological audit trail.

## 22. Maintenance rule

When forced to choose between making the UI look more live and preserving truth, preserve truth. Shared Web should fail closed, show unavailable/stale/partial states explicitly, and preserve presentation/authority separation even when the result is visually less glamorous.
