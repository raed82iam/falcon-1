# Shared Falcon Web — Full Red Team Review

Date: 2026-08-15  
Branch: `web-development`  
Writable scope reviewed: `applications/shared/web/**`  
Pre-review branch HEAD: `757c7d48fcda17b5b8dbfb886765fe71b1467273`  
Review type: architecture + security + truth/authority + FCR compliance + implementation completeness

## Executive result

**Overall status: RED TEAM FAILED — remediation required before governed Web completion or FCR closure.**

The current Web implementation has materially improved truth/authority separation and fail-closed runtime boundaries, but the review found production-blocking Web-owned defects. The most serious issue is unsafe HTML composition from dynamic customer/support/Application text. The current Owner presentation route also lacks an authentication/authorization presentation guard. FCR-0095 additionally has a five-minute escalation semantic mismatch.

No production-readiness, security-pass, full executable-test-pass, or FCR-closure claim is authorized by this report.

## Severity summary

| Severity | Count | Disposition |
|---|---:|---|
| CRITICAL | 1 | BLOCKER |
| HIGH | 5 | BLOCKER / MUST REMEDIATE |
| MEDIUM | 5 | MUST ADDRESS OR EXPLICITLY DEFER WITH EVIDENCE |
| LOW | 2 | CLEANUP |

---

# Findings

## RT-WEB-001 — CRITICAL — Dynamic HTML injection / DOM-stored XSS surface

### Evidence

`src/app.js` assigns composed feature markup to `root.innerHTML`.

Multiple feature modules interpolate externally sourced or user-derived values into markup without a shared HTML-encoding boundary. Confirmed examples include:

- `src/features/notifications/notifications.js`
  - `message.text`
  - `c.outstandingAction`
  - incident timeline `e.at`, `e.label`, `e.source`
- `src/features/owner-command-center/owner-command-center.js`
  - incident descriptions and service names/status presentation
- other feature/data presentation modules use the same string-template composition model.

A customer/support/Application-controlled string such as markup containing an event handler can cross from data into browser DOM as executable HTML.

### Why this is critical

Incident conversation is explicitly multi-party and persistent. A stored malicious customer/support message can therefore become a stored XSS vector when another user or Support/Owner opens the incident. That crosses customer/support/operator boundaries and can compromise session/UI integrity once authoritative sessions exist.

### Required remediation

1. Introduce one Web-owned output-encoding primitive for plain text, or move dynamic values to DOM `textContent`/safe node creation.
2. Treat all Application, customer, Support, broker-label, incident, catalog, analysis and timeline text as untrusted presentation data unless explicitly typed as trusted markup.
3. Do not solve this with ad-hoc regex sanitization.
4. Add hostile-input tests covering `<script>`, event attributes, malformed tags, SVG payloads, encoded payloads, quotes and RTL text.
5. No runtime adapter may bypass the encoding boundary.

**Status: OPEN / BLOCKING.**

---

## RT-WEB-002 — HIGH — Owner/Support routes are directly reachable without a route guard

### Evidence

`src/app.js` maps `owner`, `owner-apps`, `owner-incidents`, `owner-approvals`, `owner-users`, `owner-audit`, `owner-settings`, and `owner-simulator` directly in the route/view table. Rendering is selected from the URL hash. `routeAuthenticatedIdentity()` exists, but direct navigation is not guarded by authoritative session/role state.

### Impact

Today these pages are preview/fail-closed and do not grant business authority. However, leaving the route reachable creates a dangerous future foot-gun: once sensitive projections or Support actions are bound, URL navigation could expose operator-only presentation without authoritative identity gating.

### Required remediation

- Add a fail-closed route policy before rendering Owner/Support surfaces.
- Until FCR-0152 provides authoritative identity/session/MFA binding, Owner routes must render an unavailable/unauthorized surface, not operator content.
- UI route access still must not be treated as business authorization.

**Status: OPEN / PRODUCTION BLOCKER.**

---

## RT-WEB-003 — HIGH — FCR-0095 five-minute escalation is incorrectly cancelled by dismiss state

### Evidence

`src/incidents.js::ownerDelayAlert()` returns `false` when `dismissedAt` is present:

`if (!viewedAt || repliedAt || dismissedAt || !now) return false;`

Current Owner decision/FCR semantics require the high-priority timer to remain based on actual user view + no reply. Minimizing/dismissing the incident is not reply, acknowledgement, authorization or resolution.

### Impact

A user can dismiss/minimize without replying and prevent the required Support escalation path.

### Required remediation

- Remove dismiss/minimize as a cancellation condition.
- Cancel/reset only on a governed qualifying interaction according to the canonical FCR semantics.
- Add explicit tests for minimize/dismiss/no-reply continuing to 5-minute escalation.

**Status: OPEN / FCR-0095 BLOCKER.**

---

## RT-WEB-004 — HIGH — Screenshot no-secret policy trusts caller metadata rather than content evidence

### Evidence

`validateScreenshotMeta()` accepts an upload when `containsSecret !== true` and `fileCount === 1`. There is no content-level inspection/redaction boundary in the current Web implementation.

### Impact

Caller-provided metadata cannot enforce `SCREENSHOT_UPLOAD = ONE_AT_A_TIME_AND_NO_SECRETS`. A screenshot containing API keys, broker credentials, OTP/MFA material or account secrets can be mislabeled and accepted once upload transport is connected.

### Required remediation

- Keep upload fail-closed until governed secret-detection/redaction/confirmation semantics exist.
- Treat metadata as advisory only.
- Add file type/size/content policy and explicit secret-handling path before transport binding.

**Status: OPEN / BINDING BLOCKER.**

---

## RT-WEB-005 — HIGH — Portfolio v1 adapter accepts malformed payload omissions that the Application contract marks required

### Evidence

`src/adapters/fsats-portfolio-v1.js` calls `nullableNumber(payload[field])`, and `nullableNumber()` accepts `undefined` by converting it to `null`. Therefore a field can be omitted entirely and still pass validation.

The Application contract distinguishes required nullable fields from absent fields. The adapter also does not fully validate:

- page shape / continuation invariants;
- performance history item truth/freshness/reason fields;
- correction/supersession lineage;
- update sequence/idempotency invariants.

### Impact

Malformed/incomplete future payloads can be silently normalized into apparently valid v1 projections, weakening fail-closed contract compatibility.

### Required remediation

- Validate required-field presence separately from nullable value validity.
- Validate page and history invariants.
- Add exact update/correction/supersession validator if update messages are consumed.
- Add malformed-contract negative tests.

**Status: OPEN / FCR-0133 COMPLETION BLOCKER.**

---

## RT-WEB-006 — HIGH — Support takeover state helper is not an authorization boundary

### Evidence

`supportTakeoverAllowed()` evaluates only presentation state booleans:

- `mode === SUPPORT_TAKEOVER`
- `explicitTakeover === true`
- `supportIdentityVisible === true`

This is acceptable as a presentation helper, but it is insufficient for an actual Support action. No authoritative Support principal/session/capability reference is validated in this helper.

### Impact

If a future event handler or transport uses this helper as permission to send Support messages, client state could be mistaken for authorization.

### Required remediation

- Rename/document it explicitly as presentation eligibility only, or require a separate authoritative Support-session capability input from the FCR-0152 binding.
- No Support transport action may depend solely on client booleans.

**Status: OPEN / KNOWN FCR-0152 DEPENDENCY.**

---

## RT-WEB-007 — MEDIUM — No dedicated hostile-output security test suite

Current architecture tests protect imports/transports/vendor neutrality, but the reviewed test set does not contain a dedicated XSS/output-encoding security suite.

Required: create explicit Web security tests for dynamic rendering, credentials, incident text, catalog/analysis text, owner text, URL/hash handling and unsafe markup.

**Status: OPEN.**

---

## RT-WEB-008 — MEDIUM — Preview/demo data remains composited into authenticated workspace surfaces

The project labels preview data, which is good, but `src/app.js` still composes `demo` data into authenticated workspace features. This is acceptable only as development preview.

Before any production/runtime binding:

- demo fixture injection must be structurally separated from authoritative runtime composition;
- production profile must fail closed when authoritative projections are unavailable;
- demo mode must never coexist ambiguously with live monetary values.

**Status: OPEN / PRE-PRODUCTION GATE.**

---

## RT-WEB-009 — MEDIUM — Incident/Support action buttons are presentation-only but not uniformly disabled/labeled as unavailable

Current controls include screenshot, voice, minimize and Support takeover presentation elements while several governed transports/identity capabilities are unavailable. Some are inert, some are guarded, but the pattern is not uniform.

Required: every unavailable action needs explicit disabled/unavailable semantics and must not look completed simply because the control exists.

**Status: OPEN.**

---

## RT-WEB-010 — MEDIUM — On-demand analysis result presenter is permissive for missing required identity/provenance fields

`presentOnDemandAnalysisResultV1()` falls back many fields to `null` rather than rejecting malformed `COMPLETED`/`PARTIAL` results. A completed analysis should not silently survive missing request/correlation/result/instrument/as-of/provenance fields when the Application contract requires them.

Required: strict semantic validator before presentation; permissive UI fallback may be used only after payload validity is established.

**Status: OPEN / FCR-0127 HARDENING.**

---

## RT-WEB-011 — MEDIUM — Current check script is syntax-only and does not establish security/accessibility/localization correctness

`npm run check` runs `node --check` over JS modules. This is useful syntax validation, not governed verification.

Required governed completion still needs at minimum:

- full `npm test` evidence;
- syntax/check evidence;
- architecture boundary tests;
- hostile-input/security tests;
- accessibility review including keyboard/focus/reduced motion;
- Arabic/English localization review;
- production-profile/demo separation verification.

**Status: OPEN.**

---

## RT-WEB-012 — LOW — Shallow freezing leaves nested contract data mutable

Several adapters use `Object.freeze({...payload})` while nested objects/items remain mutable. This is not currently an authority bypass, but it weakens deterministic presentation evidence and can produce accidental post-validation mutation.

Required: clone/freeze governed projection fields or document immutable consumption discipline.

**Status: OPEN.**

---

## RT-WEB-013 — LOW — Historical planning documents can still be mistaken for current canonical policy

The Web docs contain a long chronological design history, including superseded Owner-observer-only semantics. The current FCR body is canonical, but future pages can still accidentally read historical planning as active policy.

Required: the new handover must clearly state precedence and identify superseded planning records as audit history, not current authority.

**Status: addressed by post-red-team handover documentation, but historical files remain intentionally preserved.**

---

# Positive controls that survived Red Team

The following controls are materially good and should be preserved during remediation:

- Web source remains inside `applications/shared/web/**`.
- FSATS and Web presentation-market-data contract families are separated.
- `WEB_PRESENTATION_DATA != FSATS_OPERATIONAL_DATA` is preserved by design.
- external market-data destinations are fail-closed behind Foundation Stage 12/FCRs.
- raw Web presentation data is explicitly ineligible for FSATS input.
- auth adapter is fail-closed by default.
- no live OAuth/MFA/session truth is fabricated.
- no external provider/broker connection is activated by current Web code.
- no hard-coded portfolio percentages or fake filled-order states remain in the reconciled surfaces.
- order lifecycle includes unknown broker outcome semantics.
- AI/detailed analysis preserves missing values and avoids claiming Web as analysis truth owner.
- Support cannot impersonate Falcon by current intended semantics.
- takeover/escalation/ack/delivery are kept distinct from incident resolution.
- FCR-0196 through FCR-0200 correctly expose additional full-market destination gaps rather than hiding them.

---

# Red Team remediation order

1. **RT-WEB-001 XSS/output encoding**
2. **RT-WEB-002 Owner route guard**
3. **RT-WEB-003 FCR-0095 escalation timer**
4. **RT-WEB-004 screenshot secret enforcement boundary**
5. **RT-WEB-005 strict portfolio contract validation**
6. **RT-WEB-006 Support authoritative capability separation**
7. RT-WEB-010 strict on-demand result validation
8. RT-WEB-007 security test suite
9. RT-WEB-008 production/demo composition split
10. RT-WEB-009 unavailable action UX
11. accessibility/localization/governed verification
12. low-severity immutability/documentation cleanup

Do not close FCR-0095, FCR-0125, FCR-0126, FCR-0127, FCR-0128, FCR-0130, or FCR-0133 based on the current implementation checkpoint alone.

## Final Red Team decision

```text
WEB_ARCHITECTURE_DIRECTION = ACCEPTABLE_WITH_BLOCKERS
WEB_TRUTH_AUTHORITY_SEPARATION = SUBSTANTIALLY_IMPROVED
WEB_SECURITY = FAIL
WEB_OWNER_ACCESS_PRESENTATION = FAIL
WEB_INCIDENT_FCR_0095_SEMANTICS = FAIL_ON_DISMISS_TIMER_CASE
WEB_CONTRACT_VALIDATION = PARTIAL
WEB_EXTERNAL_EGRESS = CORRECTLY_FAIL_CLOSED
WEB_AUTHORITY_FABRICATION = NOT_FOUND
WEB_FULL_EXECUTABLE_VERIFICATION = NOT_PROVEN
WEB_FCR_CLOSURE_ELIGIBILITY = NO
```
