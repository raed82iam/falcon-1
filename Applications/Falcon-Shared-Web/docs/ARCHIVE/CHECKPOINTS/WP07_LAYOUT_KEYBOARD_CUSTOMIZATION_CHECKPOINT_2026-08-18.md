# WP-07 Layout / Keyboard Customization Checkpoint

**Date:** 2026-08-18  
**Branch:** `web-development`  
**Status:** `SOURCE_LAYOUT_ACCESSIBILITY_HARDENING_IMPLEMENTED / FULL_BROWSER_VERIFICATION_PENDING`

## Implemented

The existing dashboard already preserved Web-owned layout preferences for hide/show/reset/order. This checkpoint hardens the interaction so customization is not mouse-only.

`src/composition/app-ui-bindings.js` now:

- preserves drag-and-drop reordering;
- makes dashboard widgets keyboard focusable;
- declares `Alt+ArrowUp` / `Alt+ArrowDown` keyboard shortcuts;
- reorders widgets through the existing Web-only `store.reorderWidget(...)` path;
- disables the Manage Widgets control when there is nothing to restore;
- when hidden widgets exist, Manage Widgets moves the user to the restore controls rather than acting as a dead button.

No layout action changes Trading/Application state.

```text
WIDGET_ORDER = WEB_PREFERENCE
WIDGET_ORDER != TRADING_PRIORITY
HIDE_WIDGET != DISABLE_TRADING_CAPABILITY
RESIZE_WIDGET != BUSINESS_STATE_CHANGE
KEYBOARD_REORDER != ACTION_AUTHORIZATION
```

## Test added

`tests/dashboard-keyboard-layout.test.mjs`

Coverage:

- move up;
- move down;
- boundary no-op behavior.

## Remaining

- full current-HEAD test execution;
- desktop/mobile browser verification;
- RTL/LTR browser verification on final candidate.

WP-08 provider connectivity remains separately gated by exact authoritative Web runtime principal/policy/credential-reference bindings and is not activated by this checkpoint.
