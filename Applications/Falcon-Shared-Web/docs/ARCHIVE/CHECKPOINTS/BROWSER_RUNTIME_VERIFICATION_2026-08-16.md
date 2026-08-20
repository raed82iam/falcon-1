# Shared Falcon Web — Browser Runtime Verification

Status: PENDING_EXECUTION
Date: 2026-08-16
Branch: `web-development`
Scope: Shared Web browser behavior that can be verified without inventing Foundation identity/session/MFA or provider connectivity.

## Authority boundary

This checklist does not activate deployment, provider connectivity, authoritative authentication, Owner authority, customer identity, Support identity, broker connectivity, FSATS runtime authority, or Foundation runtime authority.

Protected Owner/customer browser surfaces remain fail-closed without the authoritative identity/session/MFA boundary tracked by FCR-0152. A local verification server is not an authentication bypass.

## Local server

From `applications/shared/web`:

```powershell
npm.cmd run verify:browser
```

Expected listener:

```text
http://127.0.0.1:4173/
```

The verification server binds only to loopback, serves only the Shared Web subtree, permits only GET/HEAD, disables caching, and does not create provider connectivity.

## Web-only browser checks executable now

Record each as PASS or FAIL with observation evidence.

1. Public page loads at `http://127.0.0.1:4173/` with no missing local JS/CSS resources.
2. Keyboard Tab reaches the skip link and a visible focus indicator is present.
3. Activating the skip link moves navigation toward the main content rather than trapping focus in the header.
4. Continue with Tab/Shift+Tab across public controls. Every keyboard-focusable control has a visible focus indicator and no focus is lost off-screen.
5. Activate public buttons/links with keyboard Enter/Space where applicable. No mouse-only public control is required for normal navigation.
6. Switch Arabic/English using the visible language control. Confirm `lang` and page direction change together: Arabic = RTL, English = LTR.
7. Refresh after language selection. Confirm the saved language preference is preserved.
8. Resize Edge below 760 px width. Confirm public content remains usable without horizontal page-level clipping and controls remain keyboard reachable.
9. Navigate directly to protected hashes such as `#trader` and `#owner`. Without an authoritative session, each must fail closed to the sign-in/public authentication surface rather than exposing protected workspace content.
10. Open DevTools Console only for observation. There must be no uncaught exception produced by ordinary public navigation, language switching, resize, or protected-route denial.

## Browser checks intentionally blocked by missing authoritative runtime

Do not mark these PASS from source/unit tests alone:

- authenticated Customer workspace keyboard/focus traversal;
- authenticated Owner workspace keyboard/focus traversal;
- mobile authenticated workspace `<details>` navigation behavior;
- authoritative Google/Microsoft sign-in and MFA/session lifecycle;
- authoritative Owner/customer/Support role resolution;
- production incident persistence namespace/policy;
- governed screenshot scanner runtime;
- exact local Whisper.cpp/Piper runtime binding;
- authorized Support/contact transport;
- live provider route connectivity or credentials.

These require the relevant authoritative runtime/binding and must remain fail-closed until available.

## Result format

```text
COMMIT = <exact web-development commit>
EDGE_PUBLIC_LOAD = PASS|FAIL
KEYBOARD_SKIP_LINK = PASS|FAIL
KEYBOARD_FOCUS_VISIBLE = PASS|FAIL
PUBLIC_KEYBOARD_ACTIVATION = PASS|FAIL
ARABIC_RTL = PASS|FAIL
ENGLISH_LTR = PASS|FAIL
LANGUAGE_PERSISTENCE = PASS|FAIL
RESPONSIVE_PUBLIC_760 = PASS|FAIL
PROTECTED_TRADER_FAIL_CLOSED = PASS|FAIL
PROTECTED_OWNER_FAIL_CLOSED = PASS|FAIL
PUBLIC_CONSOLE_ERRORS = PASS|FAIL
AUTHENTICATED_BROWSER_SURFACES = BLOCKED_BY_FCR_0152
LIVE_PROVIDER_CONNECTIVITY = NOT_ACTIVATED
```

A fully green Web-only public-browser result is evidence for the public/fail-closed browser portion only. It does not close FCR-0095 or provider FCRs whose remaining authoritative runtime bindings are still absent.
