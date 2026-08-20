# WP-04 Public Falcon OS Experience Checkpoint

**Date:** 2026-08-18  
**Branch:** `web-development`  
**Status:** `SOURCE_FOUNDATION_IMPLEMENTED / EXECUTABLE_BROWSER_VERIFICATION_PENDING`

## Scope

This record covers the current Web-owned Public Falcon / FSATS discovery source progress. It does not claim live authentication, real account creation, subscription authority, operational availability, regulatory approval, deployment or final WP closure.

## Implemented

### Falcon hierarchy truth

Public Falcon preserves:

```text
FALCON OS
-> FSATS (current Trading system)
-> FSATA / FSAPMA / FTGA / FSTSimA / APP-RSC inside FSATS
```

and does not present non-Trading future domains as children of FSATS.

### Future-system presentation

Against the Owner-accepted Product Blueprint, Public Falcon now presents:

- Future Accounting System;
- Future Warehouse System;
- Other Future Falcon Systems.

Every future-system card is explicitly marked `Future / Not operational`, has no `data-nav` runtime link, and exposes only a disabled `Not currently available` control.

```text
FUTURE_SYSTEM_VISIBLE != OPERATIONAL_SYSTEM
FUTURE_SYSTEM_VISIBLE != SUBSCRIPTION_AVAILABLE
FUTURE_SYSTEM_VISIBLE != RUNTIME_AUTHORITY
```

### Public account entry points

Public Home now exposes distinct:

- Sign In -> `login`;
- Create Account -> `register`.

`register` is a Web PUBLIC route and reuses the existing fail-closed FSATS onboarding presentation. After render, focus is directed to the first onboarding field. The form remains non-operational while the authoritative identity/account-creation boundary is unavailable.

```text
REGISTER_ROUTE = PUBLIC_PRESENTATION
REGISTER_ROUTE != ACCOUNT_CREATED
REGISTER_ROUTE != FALCON_IDENTITY
REGISTER_ROUTE != ENTITLEMENT
REGISTER_ROUTE != BUSINESS_AUTHORITY
```

### Shared FSATS preview catalog

The actual Web composition now injects the same `data.fsatsApps` preview catalog into both:

- Falcon Public hierarchy presentation;
- FSATS Public child-Application presentation.

The FSATS renderer retains only an isolated-renderer fallback for standalone/test use; the composed application path no longer owns two independent current preview catalogs.

Supplied catalog names and short names are escaped before rendering.

```text
WEB_PREVIEW_CATALOG_SOURCE = ONE_COMPOSED_SOURCE
WEB_PREVIEW_CATALOG != APPLICATION_RUNTIME_TRUTH
CATALOG_INPUT != TRUSTED_HTML
```

## Tests added

- `tests/wp04-public-product-truth.test.mjs`
  - Falcon OS / FSATS hierarchy;
  - all five current FSATS internal Applications;
  - exact future families;
  - future/non-operational disabled semantics;
  - no navigation link from future cards;
  - distinct Sign In / Create Account entry points;
  - Arabic future presentation;
  - no unapproved regulatory/licensing claims.

- `tests/wp04-public-registration-route.test.mjs`
  - canonical `register` route;
  - PUBLIC surface classification;
  - unauthenticated accessibility of the public presentation route;
  - reuse of FSATS public onboarding rather than a protected workspace.

- `tests/wp04-fsats-public-catalog-composition.test.mjs`
  - supplied shared catalog is consumed;
  - catalog changes flow through composition;
  - hostile catalog names/short names render as escaped text;
  - invalid catalog shape fails closed.

## Source Red Team

Findings found and remediated during the WP-04 source cycle:

1. **Catalog drift risk**: Falcon Public and FSATS Public could maintain independent current preview lists. Remediated by composing the same `data.fsatsApps` into both public features.
2. **Supplied-catalog HTML injection risk**: after catalog composition was opened, the historical FSATS renderer directly interpolated name/short-name fields that had previously been static. Remediated with escaped presentation before the new input path is considered source-ready.
3. **Create Account indirect entry**: first Public Home revision sent both Sign In and Create Account to `login`. Remediated with a distinct PUBLIC `register` route and onboarding focus.

Current source-review result:

```text
OPEN_CRITICAL = 0
OPEN_HIGH = 0
OPEN_MEDIUM = 0
OPEN_PRODUCT_LOW = 0
```

## Verification limitation

A fresh exact-current-HEAD checkout cannot currently be executed because the available runner cannot resolve `github.com`, and no GitHub Actions run is available for the current Web candidate.

No exact-current-HEAD `npm test`, `npm run check`, browser or mobile PASS is claimed here.

## Current decision

```text
WP04_FUTURE_SYSTEM_TRUTH = IMPLEMENTED
WP04_PUBLIC_REGISTER_ROUTE = IMPLEMENTED
WP04_SHARED_PREVIEW_CATALOG_COMPOSITION = IMPLEMENTED
WP04_SOURCE_RED_TEAM = PASS
WP04_REGULATORY_CLAIM_GUARD = TEST_DEFINED / EXECUTION_PENDING
WP04_EXECUTABLE_BROWSER_VERIFICATION = PENDING_ENVIRONMENT_CAPABILITY
WP04_FINAL_CLOSURE = NOT_CLAIMED
```
