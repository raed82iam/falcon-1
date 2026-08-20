# Shared Falcon Web Implementation Architecture

**Status:** ACTIVE IMPLEMENTATION RULE  
**Branch:** `web-development`  
**Scope:** `applications/shared/web/**`  
**Owner implementation authority:** 2026-08-15

## 1. Objective

Build Shared Falcon Web as a long-lived, replaceable, maintainable Falcon Shared Application that can bind to governed Foundation/Application contracts without becoming coupled to their internal implementation.

The architecture optimizes for:

- constitutional and Vision fidelity;
- explicit ownership boundaries;
- truth preservation and fail-closed behavior;
- modularity and replaceability;
- testability;
- accessibility and bilingual UX;
- controlled evolution;
- low-cost maintenance and modification.

## 2. Dependency direction

```text
UI / Feature Presentation
        ↓
Web-owned presentation/state policy
        ↓
Web-owned ports/contracts
        ↓
Governed adapters
        ↓
Foundation / owning Application public contracts
```

Forbidden direction:

```text
UI -> Foundation internals
UI -> FSATS internals
UI -> provider/broker internals
```

The Web layer may display authoritative projections and submit governed requests. It does not acquire the authority represented by those projections or requests.

## 3. Architectural style

Use feature-oriented modular boundaries with ports/adapters at every cross-workstream edge.

Target source shape as implementation matures:

```text
src/
  core/
    ports/
    policy/
    truth/
  platform/
    auth/
    localization/
    persistence/
    navigation/
  design-system/
    primitives/
    patterns/
  features/
    falcon-public/
    my-applications/
    fsats-public/
    fsats-workspace/
    notifications/
    incidents/
    owner-command-center/
  adapters/
    foundation/
    applications/
  composition/
```

Migration to this shape is incremental. Existing behavior must remain verifiable while large modules are decomposed.

### Decomposition checkpoints

Checkpoint 1 established:

- `src/platform/navigation/routes.js` for canonical Web-owned route identity and public/user/owner surface classification;
- `src/design-system/presentation.js` for reusable domain-neutral presentation primitives and escaped display text;
- `src/composition/app-context.js` for explicit dependency injection into future feature renderers.

Checkpoint 2 established:

- `src/composition/shell.js` as the owner of Public Shell, Workspace Shell and Sidebar composition;
- `src/app.js` consumes the shell through explicit composition instead of defining those structures inline;
- canonical navigation flows through `normalizeRoute()` and `routeHash()`;
- `owner-simulator` is explicitly registered as an Owner presentation route rather than bypassing the route registry;
- shell behavior is covered by `tests/shell.test.mjs`.

Checkpoint 3 established:

- `src/features/falcon-public/falcon-public.js` as the Falcon-wide public Home and Applications presentation feature;
- `src/features/fsats-public/fsats-public.js` as the FSATS-specific public landing/sign-in presentation feature;
- `src/app.js` composes those features instead of owning their page rendering inline;
- Falcon Public keeps future Applications visibly non-operational until an actual route/capability exists;
- FSATS Public keeps authentication fail-closed when the operational identity binding is unavailable;
- public Application names are escaped before rendering;
- unlicensed regulatory claims remain absent by design;
- behavior is covered by `tests/falcon-public.test.mjs` and `tests/fsats-public.test.mjs`.

Checkpoint 4 now establishes:

- `src/features/my-applications/my-applications.js` as the authenticated Falcon user Application-discovery/access presentation feature;
- `src/features/fsats-workspace/fsats-workspace.js` as the FSATS regular-user dashboard/workspace presentation feature;
- `src/app.js` composes both features instead of retaining their page/widget rendering inline;
- Application visibility remains presentation only and does not grant entitlement or access authority;
- dashboard layout customization remains Web-owned presentation preference only;
- demo workspace values remain explicitly non-live and cannot satisfy an unavailable authoritative runtime dependency;
- the `trader` route now resolves through the extracted FSATS workspace feature without changing Trading ownership;
- feature behavior is covered by `tests/my-applications.test.mjs` and `tests/fsats-workspace.test.mjs`.

These modules create no business authority, entitlement, identity, runtime transport, licensing state, provider/broker connectivity, or Trading authority. They move only Web-owned presentation behind feature boundaries.

## 4. Port rule

Every live cross-workstream integration shall terminate at a Web-owned port before reaching presentation code.

A port defines what Web needs, not how Foundation or an Application internally works.

The first runtime port is implemented in:

`src/core/runtime-port.js`

The default adapter is deliberately unavailable/fail-closed. Missing governed runtime bindings must remain unavailable rather than becoming demo truth or fabricated authority.

## 5. Transport rule

Network transport is an adapter responsibility.

Presentation modules shall not directly call:

- `fetch`;
- `WebSocket`;
- `EventSource`;
- Foundation internal APIs;
- Application internal APIs.

Architecture tests enforce this boundary prospectively.

## 6. Deployment portability rule

Infrastructure vendors are replaceable implementation bindings, not Falcon dependencies.

Shared Web defines required deployment capabilities independently of vendor identity. The current provider-neutral capability model is implemented in:

`src/core/deployment-profile.js`

The detailed portability rule is documented in:

`docs/DEPLOYMENT_PORTABILITY.md`

Runtime source code shall not hard-code Cloud, CDN, WAF, hosting, observability, storage or similar provider identities. Provider selection and provider-specific configuration belong to governed deployment binding outside presentation/business source.

Initial operational candidates may use one provider for edge capabilities and another for compute, but replacement of either must not require redesign of Shared Web features or Falcon Application semantics.

```text
CAPABILITY != VENDOR
PROVIDER_SELECTED != DEPLOYMENT_AUTHORIZED
PROVIDER_CONFIGURED != PROVIDER_VERIFIED
CURRENT_PROVIDER != PERMANENT_PROVIDER
```

Architecture tests enforce the provider-neutral runtime-source boundary.

## 7. State rule

State is classified before storage:

- authoritative projection state: sourced from the owning contract and never invented by Web;
- Web interaction state: navigation, expanded panels, transient forms, layout interaction;
- user preference state: language, layout and other permitted presentation preferences;
- demo fixture state: development-only and visibly non-live.

No demo fixture may silently satisfy an unavailable authoritative runtime dependency.

## 8. Feature modularity rule

A feature module should own only the presentation behavior for one coherent user capability.

Feature modules communicate through stable Web-owned interfaces rather than reaching into each other's internals.

Shared primitives belong in the design system only when they are genuinely reusable. Domain-specific semantics stay in the owning feature/Application.

## 9. Framework policy

No framework is adopted merely because it is popular.

A framework or build-system migration must demonstrate material benefit in maintainability, accessibility, testing, performance, team/tooling ergonomics or long-term replaceability, and must preserve Falcon boundaries.

Current external engineering references reviewed for this implementation direction include:

- React official guidance on separating complex state logic and context wiring;
- TypeScript official project-reference guidance for logical separation and scalable type checking;
- Vite official guidance for modern modular build tooling;
- W3C WCAG 2.2 as the accessibility baseline.

These are engineering inputs, not Falcon authority. Falcon Vision, Constitution, governing specifications, Owner decisions and current contracts remain superior.

## 10. Quality gates

Before a Web implementation area is considered complete:

1. ownership/authority re-check;
2. contract compatibility check;
3. syntax/type/static validation appropriate to the chosen stack;
4. unit tests for policy and truth preservation;
5. architecture-boundary tests;
6. accessibility verification;
7. Arabic RTL and English LTR verification;
8. error/stale/partial/unavailable state verification;
9. security review;
10. Red-Team review;
11. governed cross-workstream verification where applicable.

## 11. Current migration priority

The current `src/app.js` remains a functional implementation checkpoint but is still broader than the final maintainable composition shape.

It shall continue to be decomposed incrementally while preserving behavior and tests.

Current sequence:

1. route registry, presentation primitives and composition context: COMPLETE FOR CURRENT CHECKPOINT;
2. shell/navigation behavior migration from `src/app.js`: COMPLETE FOR CURRENT CHECKPOINT;
3. Falcon Public and FSATS Public feature extraction: COMPLETE FOR CURRENT CHECKPOINT;
4. My Applications and FSATS dashboard/workspace extraction and composition: COMPLETE FOR CURRENT CHECKPOINT;
5. Portfolio / Activity / Markets / AI / Notifications user feature slices: NEXT;
6. Owner Command Center feature slices;
7. runtime adapter specialization by owning contract family;
8. accessibility and localization hardening.

`WORKING_UI != FINAL_ARCHITECTURE`
`POPULAR_TECHNOLOGY != BEST_FALCON_FIT`
`MAINTAINABILITY != PERMISSION_TO_BREAK_AUTHORITY`
