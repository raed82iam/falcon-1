# WP-03 Design, Localization and Accessibility Foundation Checkpoint

**Date:** 2026-08-18  
**Branch:** `web-development`  
**Status:** `SOURCE_FOUNDATION_IMPLEMENTED / BROWSER_EXECUTABLE_VERIFICATION_PENDING`

## Scope

This checkpoint records Web-owned source work for WP-03. It does not claim browser/runtime PASS, final WP closure, production deployment authority or any cross-workstream authority.

## Existing good foundation preserved

Fresh source review found that Shared Web already had useful reusable foundations:

- `src/design-system.css` with shared surface/status/control tokens;
- `src/design-system/primitives.js` with status/notice/disabled-control primitives;
- `src/accessibility.css` with focus-visible and reduced-motion handling;
- Arabic/English message catalogs;
- `i18n.set()` document `lang` / `dir` synchronization;
- logical CSS properties already used in the design-system status/notice layer.

WP-03 therefore extends the existing system instead of replacing it.

## Implemented source improvements

### 1. Reusable localized skip-link and main landmarks

`src/composition/shell.js` now provides one shell-owned skip link for Public, Customer and Owner surfaces:

```text
Arabic: تخطي إلى المحتوى الرئيسي
English: Skip to main content
Target: #main
```

Public and workspace content now use a focusable `<main id="main" tabindex="-1">` landmark.

The older fixed Arabic skip link was removed from `index.html` so there is one dynamic source of truth.

`src/accessibility.css` provides keyboard-visible skip-link behavior without interfering with normal layout and preserves reduced-motion handling.

### 2. Reusable accessible presentation primitives

`src/design-system/primitives.js` now includes reusable:

- `sectionCard()`;
- `formField()`;
- `dataTable()`;

in addition to the existing status, notice and disabled-control primitives.

The new primitives preserve text/attribute escaping, semantic headings, label/input binding, description binding, required/disabled state, table captions and column scopes. Table wrappers are exposed as named accessibility regions only when a caption/accessibility name exists.

`src/design-system.css` now contains reusable styling for these primitives, responsive horizontal table overflow, logical text alignment and forced-colors support.

### 3. Language persistence failure resilience

During interactive language changes, `app-ui-bindings.js` already protected the UI from blocked browser storage. Fresh WP-03 review found startup remained vulnerable because `app.js` called `i18n.set()` directly.

Startup is now fail-soft: if browser preference persistence throws, the current language and document direction remain applied and the Web bootstrap continues.

Obsolete static-skip-link synchronization was removed from `app-ui-bindings.js`; the shell now owns localized skip-link presentation while the UI binding owns `lang` / `dir` synchronization.

## Tests added

- `tests/wp03-shell-accessibility.test.mjs`
  - Public skip link and main landmark;
  - User and Owner workspace main landmark;
  - Arabic skip-link localization.

- `tests/wp03-design-primitives.test.mjs`
  - hostile-content escaping;
  - semantic headings;
  - form label/description/required/disabled state;
  - table caption/column scope/named-region behavior;
  - ambiguous table row rejection.

- `tests/wp03-language-resilience.test.mjs`
  - language state remains usable if persistence throws;
  - document `lang` and `dir` synchronize independently of persistence.

## Source Red Team

Findings identified and remediated during WP-03 source review:

1. **Duplicate skip-link source**: static Arabic `index.html` link plus shell dynamic link would create duplicate keyboard targets. Remediated by removing the static link.
2. **Startup persistence failure**: blocked `localStorage` could throw during initial `i18n.set()` and stop bootstrap. Remediated with fail-soft startup handling.
3. **Unnamed accessibility region**: first `dataTable()` revision used `role="region"` without requiring an accessible name. Remediated so region semantics are emitted only with a caption/name.
4. **Obsolete skip-link synchronization**: old DOM binding targeted a removed `[data-skip-link]`. Removed to preserve one responsibility owner.

Current source-review result:

```text
OPEN_CRITICAL = 0
OPEN_HIGH = 0
OPEN_MEDIUM = 0
OPEN_PRODUCT_LOW = 0
```

## Preserved boundaries

```text
ACCESSIBILITY_PRESENTATION != BUSINESS_AUTHORITY
LANGUAGE_PREFERENCE != IDENTITY_OR_TENANT_STATE
STORAGE_FAILURE != WEB_UI_FAILURE
COLOR != SOLE_STATUS_SIGNAL
RTL_LTR_PRESENTATION != BUSINESS_SEMANTIC_CHANGE
WEB_DESIGN_SYSTEM != FOUNDATION_OR_FSATS_TRUTH_OWNER
```

## Verification limitation

The available command-line runner still cannot resolve `github.com`, and no GitHub Actions workflow run is available for the current `web-development` candidate. Therefore this checkpoint does not claim a fresh exact-HEAD `npm test`, `npm run check`, browser, keyboard, RTL/LTR or mobile PASS.

Required before final WP-03 closure:

```text
EXACT_HEAD_NPM_TEST = PASS
EXACT_HEAD_NPM_RUN_CHECK = PASS
PUBLIC_BROWSER_KEYBOARD = PASS
CUSTOMER_BROWSER_KEYBOARD = PASS
OWNER_BROWSER_KEYBOARD = PASS
ARABIC_RTL = PASS
ENGLISH_LTR = PASS
MOBILE_RESPONSIVE = PASS
REDUCED_MOTION = PASS
FORCED_COLORS_APPLICABLE_CHECK = PASS
FRESH_POST_EXECUTABLE_RED_TEAM = PASS
```

## Current decision

```text
WP03_REUSABLE_SOURCE_FOUNDATION = IMPLEMENTED
WP03_SOURCE_RED_TEAM = PASS
WP03_BROWSER_EXECUTABLE_VERIFICATION = PENDING_ENVIRONMENT_CAPABILITY
WP03_FINAL_CLOSURE = NOT_CLAIMED
```

Independent Web source work may continue where its own entry criteria are satisfied. WP-23 remains the whole-Web assurance gate for an exact final candidate.
