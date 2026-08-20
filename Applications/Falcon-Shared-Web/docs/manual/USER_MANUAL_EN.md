# Standard User Manual — Falcon Shared Web

**Audience:** Standard User  
**Language:** English  
**Scope:** Public and authenticated customer surfaces in Shared Falcon Web

## 1. What is Falcon Shared Web?

Shared Falcon Web is Falcon's shared presentation and interaction layer. It presents governed information and allows governed requests, but it is not itself an operational truth source and does not own trading, execution, entitlement, or business authority.

## 2. Public pages

The following public destinations exist:

- Home `#/home`
- Applications `#/apps`
- Sign in `#/login`
- Create account `#/register`
- FSATS public page `#/fsats`

The existence of a route does not grant any operational entitlement.

## 3. Authenticated user pages

When an authoritative session and the required access entitlement are available, the user workspace can expose:

- My Applications `#/my-apps`
- FSATS workspace `#/trader`
- Markets `#/markets`
- Advisory Markets `#/advisory-markets`
- Portfolio `#/portfolio`
- Activity `#/activity`
- AI `#/ai`
- Notifications `#/notifications`
- Settings `#/settings`

A visible link or card does not automatically mean the account is entitled to use the feature. Missing authoritative entitlement remains locked or unavailable.

## 4. My Applications and plans

The My Applications surface can present `STANDARD` and `VIP` tiers, but plan truth must be authoritative.

- Pricing is not guessed.
- Trial state is not inferred.
- Upgrade state is not inferred.
- A visible tier is not proof of entitlement.

Rule: `TIER_VISIBLE ≠ ENTITLED ≠ ACTION_AUTHORIZED`.

## 5. Markets and charts

Market pages are presentation-only surfaces. Web market observations do not become FSATS operational inputs and do not authorize trade execution.

If a source is unavailable or not bound, the UI must show an unavailable state instead of inventing market data.

## 6. Portfolio and activity

- Portfolio values come from supplied authoritative projections only.
- Missing values remain missing and are not silently converted to zero.
- Activity preserves the source lifecycle state.
- `PARTIALLY_FILLED` remains distinct from `FILLED`.
- Unknown broker outcomes remain unknown rather than being converted into success or rejection.

## 7. AI and analysis

Detailed AI analysis is shown only when the supplied result is current, complete, and eligible for detailed presentation. Stale, partial, or clarification-required results are restricted instead of being presented as complete.

The Web layer does not create a trading strategy or execution decision on its own.

## 8. Notifications and support

Incident and support functions may depend on authoritative production bindings such as:

- principal / tenant / session identity
- tenant-scoped persistence
- governed screenshot scanning
- governed Support transport
- local voice runtime

When these are not available, the function remains fail closed.

Never send passwords, API keys, secrets, or credentials in chat. Screenshots containing secrets must be rejected.

## 9. Language and accessibility

- Arabic and English are supported.
- Arabic uses RTL presentation.
- Navigation is keyboard accessible.
- Visible keyboard focus is provided.
- A skip link targets the main content.
- Reduced-motion and forced-colors support is included where applicable.

## 10. Preview and Unavailable

`Preview` means illustrative non-live presentation.  
`Unavailable` means authoritative data or binding is missing or unavailable.  
Neither state may be silently upgraded to Live.

## 11. User safety rules

- A visible button is not proof of authority.
- Do not place secrets into ordinary Web fields.
- A screenshot is not broker-confirmed truth.
- A Support message is not trading approval or business authorization.
- If a state is unclear, treat it as unavailable until confirmed by the authoritative source.

## 12. Current status

Shared Web is fully prepared for later Foundation onboarding, but actual Admission, Runtime Registration, deployment, provider connectivity, and production operation require a separate authorized operation. This manual does not claim that the live environment is currently bound.
