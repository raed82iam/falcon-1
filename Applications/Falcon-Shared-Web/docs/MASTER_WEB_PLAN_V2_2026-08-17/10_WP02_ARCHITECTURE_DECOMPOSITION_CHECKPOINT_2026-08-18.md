# WP-02 Architecture Decomposition and Composition Stabilization Checkpoint

**Date:** 2026-08-18  
**Branch:** `web-development`  
**Status:** `SOURCE_STABILIZATION_IMPLEMENTED / EXECUTABLE_VERIFICATION_PENDING`

## Scope

This checkpoint records the current Web-owned WP-02 source stabilization work. It does not claim whole-Web executable PASS, implementation-baseline closure, production deployment authority, live connectivity, or any Foundation/Application authority.

## Fresh entry state

Before this work, the current source already had substantial decomposition:

- route/view registry isolated in composition;
- public/customer feature modules extracted;
- Owner surfaces composed through `composition/owner-surfaces.js`;
- incident UI runtime isolated;
- Foundation/Application adapters behind Web-owned boundaries;
- no direct `fetch()` or `WebSocket` calls found in presentation feature modules during fresh source inspection.

The remaining obvious inline presentation slices in `src/app.js` were:

1. Settings page markup.
2. School/Strategy catalog presentation markup.

WP-02 therefore did not justify a broad rewrite.

## Implemented changes

### 1. Settings extraction

Created:

`src/features/settings/settings.js`

The feature owns only Web presentation of language/RTL-LTR preference state and creates no Falcon/FSATS/business authority.

### 2. Catalog presentation extraction

Created:

`src/features/catalog/catalog-presentation.js`

The feature renders the existing Application-derived catalog presentation model and preserves safe text encoding. It does not recreate applicability, entitlement, strategy, activation or Trading business logic.

### 3. App bootstrap simplification

Updated:

`src/app.js`

The file now composes the extracted settings and catalog features instead of owning their markup. `app.js` remains the browser bootstrap/composition point for routing, session reference, data-source selection, shell composition, incident runtime and binding.

### 4. Architecture regression guard

Created:

`tests/wp02-architecture-boundaries.test.mjs`

The guard recursively inspects `src/features/**` and fails if presentation features acquire:

- direct `fetch()`;
- direct `WebSocket`;
- `XMLHttpRequest`;
- `EventSource`;
- direct `innerHTML` mutation;
- direct cross-workstream internal imports referencing Foundation/FSATS branch/internal paths.

It also asserts that settings/catalog rendering no longer lives inline in `app.js`.

### 5. Extracted feature tests

Created:

`tests/wp02-extracted-features.test.mjs`

Coverage includes:

- Settings language-state rendering;
- catalog safe encoding;
- enabled/disabled catalog presentation behavior against the current `applicability` contract;
- dependency fail-closed validation.

### 6. Source-check coverage

Updated `package.json` `npm run check` source list to include the two extracted feature modules and the FCR-0241 Owner-governance adapters already added during the current cycle.

## Preserved architecture boundaries

```text
FEATURE_PRESENTATION != NETWORK_TRANSPORT
FEATURE_PRESENTATION != FOUNDATION_INTERNALS
FEATURE_PRESENTATION != FSATS_INTERNALS
WEB_CATALOG_PRESENTATION != APPLICATION_CATALOG_TRUTH
WEB_SETTINGS != BUSINESS_AUTHORITY
COMPOSITION != AUTHORITY
PREVIEW_DATA != AUTHORITATIVE_RUNTIME_TRUTH
```

Owner surfaces remain composed through `composition/owner-surfaces.js`; this checkpoint does not move Owner approval, rollback, Kill, provider, Trading or Foundation authority into Web presentation code.

## Source Red Team

Fresh source review focused on regression risk from the extraction and the new architecture guard.

Findings remediated during the work:

1. The first catalog test fixture used a non-canonical boolean-style applicability shape. It was corrected to the current contract field `applicability` with canonical values such as `APPLICABLE` and `NOT_APPLICABLE`.

Current source-review result:

```text
OPEN_CRITICAL = 0
OPEN_HIGH = 0
OPEN_MEDIUM = 0
OPEN_PRODUCT_LOW = 0
```

## Verification state

Current exact source HEAD when this checkpoint was prepared:

`ae5926bc47710c8f5f8c32aa03bab234ac5bfae5`

GitHub reports no workflow run for this exact commit. The available command-line execution environment has previously been unable to resolve `github.com` for a fresh checkout, so a new whole-Web executable PASS is not claimed here.

Required before WP-02 final closure:

```text
npm test = PASS on exact current candidate
npm run check = PASS on exact current candidate
WP02 architecture guard = PASS
applicable source/security review = PASS
fresh WP02 Red Team = PASS
```

## Current decision

```text
WP02_SOURCE_DECOMPOSITION = IMPLEMENTED
WP02_COMPOSITION_STABILIZATION = IMPLEMENTED
WP02_ARCHITECTURE_REGRESSION_GUARD = IMPLEMENTED
WP02_EXECUTABLE_VERIFICATION = PENDING_ENVIRONMENT_CAPABILITY
WP02_FINAL_CLOSURE = NOT_CLAIMED
```

Independent Web work may continue where its own entry criteria are satisfied. Verification-only pending state does not authorize bypassing WP-23 whole-Web assurance later.
