# Shared Falcon Web Implementation Plan

**Status:** OWNER-AUTHORIZED IMPLEMENTATION  
**Owner authorization:** 2026-08-15, explicit instruction: `ابدأ implementation كامل.`  
**Branch:** `web-development`  
**Writable scope:** `applications/shared/web/**`

## Boundary

This authorization enables Shared Web implementation only. It does not transfer Foundation or FSATS business authority to Web and does not by itself authorize production deployment, broker/provider connectivity, or another workstream's code changes.

## Work packages

1. WP-00 Design and authority freeze
2. WP-01 Shared design system and bilingual RTL/LTR foundation
3. WP-02 Public Falcon OS surface
4. WP-03 Sign-in/account entry UX shell
5. WP-04 My Applications and application switching shell
6. WP-05 FSATS public product surface
7. WP-06 FSATS user workspace and persisted customizable layout
8. WP-07 Chart, asset and School/Strategy presentation bindings
9. WP-08 Falcon AI detailed-analysis presentation
10. WP-09 Portfolio, positions, activity and performance presentation
11. WP-10 Notification and incident interaction surfaces
12. WP-11 Owner Command Center, system-controller only
13. WP-12 Preferences and persistence
14. WP-13 Governed contract adapter binding
15. WP-14 Accessibility, architecture, security and truth-state verification
16. WP-15 Owner acceptance freeze
17. WP-16 Production runtime/deployment remains separately governed

## First implementation slice

The first implementation establishes a dependency-light browser application with:
- Arabic and English with RTL/LTR switching and saved language preference;
- Public Falcon OS multi-Application landing surface;
- FSATS public/sign-in surface;
- FSATS user workspace with draggable/hideable/restorable layout and persisted preferences;
- Portfolio, markets, activity, AI, notifications and settings presentation shells;
- Owner Command Center focused on Falcon system control rather than trading;
- explicit development-preview labelling for fixtures;
- contract identity/truth-state definitions for FCR-0125/0126/0127/0128/0130/0133;
- a runtime adapter that fails unavailable instead of inventing production truth;
- legal marketing guardrails against premature regulatory/licensing claims;
- automated tests for truth preservation, catalog applicability UX and regulatory-claim suppression.

## Runtime truth rule

Development fixtures are presentation fixtures only and must be visibly labelled. Production data must come from governed authoritative owners. A missing runtime source must remain `UNAVAILABLE`/unknown as applicable and must never be silently replaced with demo truth.

## FCR closure rule

FCR-0095, 0125, 0126, 0127, 0128, 0130 and 0133 remain open until each affected Web implementation is complete and the required governed verification exists. This plan or a UI preview alone is not closure evidence.
