# WP-06 My Applications / Subscription / Tier Checkpoint

**Date:** 2026-08-18  
**Branch:** `web-development`  
**Status:** `SOURCE_PRESENTATION_FOUNDATION_IMPLEMENTED / AUTHORITATIVE_SUBSCRIPTION_CONTRACT_PENDING / FULL_EXECUTABLE_BROWSER_VERIFICATION_PENDING`

## Implemented

- My Applications now separates Application-card visibility from entitlement.
- Preview navigation is explicitly labelled Preview and does not imply entitlement or runtime activation.
- Authoritative FSATS navigation remains locked unless a current external entitlement model explicitly grants access with `businessAuthorityGranted=false`.
- A reusable Standard/VIP subscription presentation component now exists.
- When no authoritative subscription contract is available, Standard/VIP may be shown only as product-direction labels with `Contract unavailable`; no price, trial, downgrade or upgrade state is invented.
- Tier price/access claims are emitted only from an explicitly authoritative tier model.

```text
CARD_VISIBLE != ENTITLED
TIER_VISIBLE != ENTITLED
ENTITLED != ACTION_AUTHORIZED
ENTITLED != TRADING_EXECUTION_AUTHORITY
PRICE_NOT_CONTRACTED != PERMISSION_TO_GUESS_PRICE
TRIAL_DIRECTION != ACTIVE_TRIAL
```

## Tests added

`tests/my-applications-entitlement-presentation.test.mjs`

Coverage includes:

- authoritative FSATS access locked without entitlement;
- Preview route labelled as Preview;
- current non-authority entitlement enables navigation;
- no price/trial invention when contract is absent;
- tier access requires authoritative tier truth.

## Current limitation

Final commercial plan names, pricing/payment, final customer entitlement lifecycle and migration/preset-retention policy remain intentionally unfinalized until their authoritative Application/product contracts exist.

This does not block independent WP-07 Web layout work.
