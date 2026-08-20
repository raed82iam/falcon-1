# VIP User Manual — Falcon Shared Web

**Audience:** VIP User  
**Language:** English  
**Scope:** Customer surfaces plus the current governed meaning of the VIP tier

## 1. What does VIP mean today?

Shared Web supports two presentation tiers: `STANDARD` and `VIP`. The presence of a VIP card or label is not enough to prove that the account is VIP.

VIP is considered current only when an authoritative model explicitly states:

- `authoritative = true`
- `entitled = true`
- `current = true`

Otherwise the VIP tier remains locked or unavailable.

Rule: `TIER_VISIBLE ≠ ENTITLED ≠ ACTION_AUTHORIZED`.

## 2. How is VIP different from Standard?

In the current Web implementation, VIP is a subscription/access presentation tier, not an administrative role and not unrestricted authority.

VIP does not:

- unlock Owner pages
- grant Business Authority
- grant Trading Authority
- bypass authentication
- bypass entitlement checks
- turn a visible route or control into executable authority

Any real additional VIP feature must come from an authoritative contract. The Web layer does not invent one.

## 3. Customer pages available to a VIP user

With a valid authoritative session and required entitlements, VIP uses the same core customer workspace:

- My Applications `#/my-apps`
- FSATS workspace `#/trader`
- Markets `#/markets`
- Advisory Markets `#/advisory-markets`
- Portfolio `#/portfolio`
- Activity `#/activity`
- AI `#/ai`
- Notifications `#/notifications`
- Settings `#/settings`

Any additional benefit must be explicitly authorized by supplied entitlement truth.

## 4. Price, trial, and upgrade

Shared Web does not infer:

- VIP price
- subscription duration
- trial state
- upgrade path
- discounts
- priority support
- special trading limits

If these facts are not supplied by an authoritative contract, they remain unavailable.

## 5. Markets and data

VIP does not change the operational-data boundary:

- Web market data is presentation only.
- Web market data does not become FSATS operational input.
- More or faster visible data does not authorize execution.
- A configured provider route does not equal connectivity authority.

## 6. AI and analysis

Even if VIP later receives broader presentation access, truth rules do not change:

- Full detail requires Current + Complete analysis.
- Stale, Partial, or Needs Clarification states remain restricted.
- Web presentation never converts analysis into execution authority.

## 7. Support and incidents

Any additional VIP support level must come from an authoritative contract or runtime capability. Priority support is not inferred from the VIP label alone.

Incident/support functions still depend on governed identity, session, persistence, transport, screenshot scanning, and local voice bindings where applicable.

## 8. Security

- Never share credentials or secrets.
- VIP does not bypass security controls.
- VIP does not grant access to Owner or Support internals.
- VIP is not trading authority.
- A visible VIP card is not entitlement unless the underlying model is authoritative.

## 9. Current status

VIP is supported as a Web subscription presentation tier. Exact commercial benefits and VIP-specific capabilities are intentionally not fabricated by Shared Web and must come from authoritative contracts when they exist.
