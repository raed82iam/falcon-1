# WP-03 Design, Localization and Accessibility Checkpoint — 2026-08-18

**Workstream:** Shared Falcon Web Application  
**Branch:** `web-development`  
**Scope:** `applications/shared/web/**` only  
**WP:** Master Web Plan V2 / WP-03 — Design system, localization and accessibility foundation  
**State:** `FOUNDATION_SLICE_IMPLEMENTED / SOURCE_REVIEW_PASS / FEATURE_ADOPTION_IN_PROGRESS / FULL_EXECUTABLE_AND_BROWSER_VERIFICATION_PENDING / NOT_OWNER_CLOSED`

## Entry

WP-03 started from:

`0561e9ab8bac549c0bf056f350084e690cc842ac`

Existing source already contained useful accessibility and localization foundations that were preserved rather than rebuilt:

- Arabic RTL / English LTR document direction through `createI18n`;
- keyboard-native mobile navigation;
- `aria-current` for active routes;
- visible `:focus-visible` rules;
- `prefers-reduced-motion` handling;
- skip link to `#main`;
- output-encoding helpers;
- existing dark Falcon visual variables and responsive breakpoints.

## Findings addressed

### Localized skip-link continuity

The static document skip-link was Arabic-only even after switching to English.

The index now marks the skip link with `data-skip-link`, and `app-ui-bindings.js` synchronizes:

- `html.lang`;
- `html.dir`;
- localized skip-link copy.

Arabic: `تجاوز إلى المحتوى`  
English: `Skip to content`

### Preference-storage failure resilience

Current `createI18n.set(...)` applies language/direction before persisting `falcon.lang` to browser storage. Browser storage can be unavailable in constrained/privacy environments.

The Web binding now catches preference-persistence failure so the rendered language change remains usable rather than crashing the UI. This does not falsely claim that persistence succeeded.

```text
PREFERENCE_PERSISTENCE_FAILURE != LANGUAGE_RENDER_FAILURE
PREFERENCE_NOT_SAVED != UI_UNAVAILABLE
```

Generic dashboard preference storage remains a later layout/persistence concern and was not broadened into this slice.

## Reusable design primitives

Added:

`src/design-system/primitives.js`

Current primitives:

- `StatusTone`;
- `statusBadge(...)`;
- `notice(...)`;
- `visuallyHidden(...)`;
- `disabledControlAttributes(...)`.

Properties:

- all user/source text is encoded before HTML insertion;
- status semantics have explicit non-color symbols;
- unavailable state is visually differentiated beyond color;
- live-region role is constrained to `status` or `alert`;
- disabled controls expose native + ARIA disabled state;
- optional accessibility descriptions are attribute-encoded.

Added:

`src/design-system.css`

It provides semantic aliases for surfaces/content/status, reusable status/notice styles, visually-hidden utility, minimum control target size, disabled styling, and forced-colors support.

The semantic aliases intentionally sit above existing Falcon variables instead of redesigning the established visual language from zero.

## Tests added/extended

- `tests/design-system-primitives.test.mjs`
- `tests/wp03-design-accessibility.test.mjs`
- extended `tests/app-ui-bindings.test.mjs`

Coverage targets include:

- output encoding;
- non-color status cue;
- explicit accessible-label behavior;
- empty hidden text remains empty;
- alert/status role restriction;
- safe disabled reason attributes;
- semantic token presence;
- forced-colors support;
- skip-link localization;
- RTL/LTR synchronization;
- language change resilience when browser preference storage throws;
- continued keyboard focus/reduced-motion rules.

`npm run check` now includes `src/design-system/primitives.js`.

## Source Red Team

Compared with WP-03 entry:

```text
CROSS_WORKSTREAM_INTERNAL_IMPORT = NONE
DIRECT_NETWORK_TRANSPORT = NONE
BUSINESS_AUTHORITY_CHANGE = NONE
AUTHENTICATION_AUTHORITY_CHANGE = NONE
TRADING_SEMANTIC_CHANGE = NONE
FOUNDATION_TRUTH_OWNERSHIP_CHANGE = NONE
UNESCAPED_NEW_DYNAMIC_HTML = NONE IDENTIFIED
COLOR_ONLY_STATUS_PRIMITIVE = REMEDIATED_BY_SYMBOL + TEXT
FORCED_COLORS_BASELINE = ADDED
OPEN_CRITICAL = 0
OPEN_HIGH = 0
OPEN_MEDIUM = 0
OPEN_PRODUCT_LOW = 0
```

This is source review, not a substitute for executable/browser verification.

## Verification truth

The current execution environment previously failed repository checkout before tests because `github.com` could not be resolved. No newer checkout-backed capability has been proven in this slice.

Therefore:

```text
CURRENT_HEAD_NPM_TEST = NOT_RUN
CURRENT_HEAD_NPM_RUN_CHECK = NOT_RUN
CURRENT_HEAD_BROWSER_VERIFICATION = NOT_RUN
WP03_EXECUTABLE_ACCEPTANCE = NOT_ELIGIBLE_YET
```

No historical PASS is inherited.

## Current disposition

```text
WP03_EXISTING_ACCESSIBILITY_BASELINE = PRESERVED
WP03_SEMANTIC_DESIGN_TOKENS = IMPLEMENTED
WP03_ACCESSIBLE_STATUS_NOTICE_PRIMITIVES = IMPLEMENTED
WP03_LOCALIZED_SKIP_LINK = IMPLEMENTED
WP03_LANGUAGE_STORAGE_FAILURE_RESILIENCE = IMPLEMENTED
WP03_FORCED_COLORS_BASELINE = IMPLEMENTED
WP03_FEATURE_PRIMITIVE_ADOPTION = IN_PROGRESS
WP03_FULL_BROWSER_RTL_LTR_MOBILE = PENDING
WP03_OWNER_CLOSURE = NOT_REQUESTED
```

## Next

Adopt the reusable primitives where current feature-local status/notice markup is semantically equivalent, prioritizing Owner and Incident surfaces. Do not alter authoritative data meaning or turn UI status into authority. Then perform exact current-candidate executable/browser verification and post-executable Red Team before Owner closure.
