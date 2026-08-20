# FSATS SIA — Initial Risk Base-Currency and Crypto Quote Policy v1.0

**Package:** `FSATS-SIA-v0.1`
**Status:** `SEMANTIC REMEDIATION / DESIGN CANDIDATE`
**Triggered By:** `RT-RISK-002`
**Owner:** APP-TRD / T-LSA-01, T-LSA-07, T-LSA-08

## 1. Purpose

Remove ambiguity in non-base-currency valuation by deliberately constraining the initial SIA rather than inventing a multi-hop FX/crypto conversion graph before it is needed and validated.

## 2. Initial Trading Risk Base Currency

For the initial US Equities + Crypto Spot Paper profile:

```text
RiskBaseCurrency = USD
```

This is a candidate Application/account profile requirement. Broker/account certification must confirm the account can truthfully express reconciled NAV/cash/fees in or convert them canonically to USD under the admitted account profile.

If the selected broker/account cannot satisfy the USD RiskBaseCurrency profile exactly, that account/profile is not eligible for this initial SIA configuration.

## 3. US Equities Currency Eligibility

Initial US Equities instrument eligibility requires:

```text
Instrument quote/trading currency = USD
```

A security primarily requiring non-USD valuation/conversion is outside the initial profile and requires a later market/account conversion policy.

## 4. Crypto Spot Quote Eligibility

Initial Crypto Spot Trading eligibility requires:

```text
Instrument QuoteAssetId / QuoteCurrency = USD exactly
```

Examples conceptually compatible only when provider/broker certification confirms the exact instrument identity:

```text
BTC/USD
ETH/USD
...
```

A pair quoted in:

- USDT;
- USDC;
- EUR;
- BTC;
- ETH;
- any other asset/currency

is **not** eligible under v1 solely because the quote asset is popularly treated as USD-like or convertible.

```text
STABLECOIN != USD BY ASSUMPTION
```

A future stablecoin/non-USD quote profile requires explicit valuation, depeg/counterparty, liquidity and conversion semantics.

## 5. Direct Position Valuation

For an eligible Crypto Spot pair `BASE/USD`:

```text
PositionUSDValue = BaseAssetQuantity * CanonicalCurrentUSDReferencePrice
```

The reference price comes from the exact validated Data Product/valuation profile and records:

- InstrumentId;
- DataProductId/version;
- effective time;
- quality/freshness;
- provider/reconciliation provenance;
- price type used for valuation.

No hidden FX conversion graph exists in v1.

## 6. Valuation Price Type

For RiskEquity/current open-position risk in the initial profile:

### Long position conservative current liquidation reference

When a valid executable top quote exists:

```text
RiskValuationPrice = BidPrice
```

because a long position would normally liquidate into the bid.

If valid quote is unavailable but a governed protective fallback price is permitted:

```text
FallbackRiskValuationPrice = min(
  latest valid completed bar Close,
  latest valid trade price
)
```

only when each fallback source is within its separate protective-valuation freshness bound and the system marks valuation `DEGRADED`.

New risk still requires VALID current quote according to strategy/execution profiles.

If no permitted fallback is valid -> valuation UNKNOWN; new risk denied and Guardian/Risk protection may escalate.

### Cash

USD cash = exact reconciled cash amount.

No non-USD cash balance may be silently valued as USD under initial profile. Material non-USD cash appearing unexpectedly => `RECONCILIATION_REQUIRED` until handled by a future/admitted conversion profile or externally reconciled account correction.

## 7. Corporate Action / Asset Mutation

If a corporate action, symbol migration, crypto asset migration or broker action results in a non-USD-quoted asset/receivable:

- preserve exact asset identity and quantity;
- do not assign zero value;
- mark valuation UNKNOWN/RECONCILIATION_REQUIRED unless an admitted direct USD valuation product exists;
- block new risk while material account equity cannot be bounded;
- allow governed reconciliation/protective action.

## 8. Fees

Initial profile requires broker/account fee evidence to be expressible in USD.

If a crypto venue charges a non-USD asset fee under a future profile, exact asset fee quantity must be reconciled and converted by a separately admitted conversion policy. Such a venue/account behavior is outside v1 unless certification provides direct authoritative USD fee-equivalent evidence accepted by the account profile.

## 9. No Synthetic Stablecoin Parity

The following are forbidden in v1:

```text
USDT = 1 USD hardcode
USDC = 1 USD hardcode
any stablecoin = USD
last known conversion = current conversion
```

If stablecoin-quoted trading is later added, its MarketProfile must model:

- canonical stablecoin identity;
- USD conversion Data Product;
- freshness/quality;
- depeg risk;
- conversion liquidity;
- provider/broker semantics;
- tail-risk treatment.

## 10. Risk Accounting Integration

07B references a future deterministic conversion path for non-base assets. For **initial v1 eligible trading instruments**, this file controls:

```text
MULTI_HOP_CONVERSION_GRAPH = NOT_USED
ELIGIBLE_QUOTE_CURRENCY = USD ONLY
```

Therefore RiskEquity, daily/weekly return and drawdown use direct USD account/position valuation for initial eligible instruments.

A material non-USD balance is an exception/reconciliation condition, not a second implementation choice.

## 11. Provider / Broker Certification Integration

DP-001 Instrument Reference and BrokerProfile must agree on exact quote currency/asset.

Mismatch examples:

- provider says BTC/USD but broker instrument maps BTC/USDT;
- symbol alias loses quote-asset identity;
- broker reports fee asset not admitted by account profile.

=> instrument/account capability is incompatible and cannot be enabled.

Symbol text alone is never enough; canonical base/quote identities must match.

## 12. Future Extension Rule

Adding non-USD or stablecoin quote assets is a semantic Market/Risk profile expansion requiring:

```text
new conversion Data Product contract/profile
canonical conversion path algorithm
freshness/quality/tie-break rules
conversion tail/depeg risk
account/capital persistence semantics
strategy/Risk validation
fresh A/C + Red-Team + Owner acceptance
```

No coding worker may implement it opportunistically because a provider endpoint offers the pair.

## 13. Negative Fixtures

Verifier SHALL reject:

1. initial TradingAccount RiskBaseCurrency != USD;
2. US equity non-USD quote under v1;
3. Crypto pair quote asset != USD under v1;
4. USDT/USDC treated as USD automatically;
5. provider/broker base/quote mismatch;
6. non-USD fee silently treated as USD;
7. unknown valuation converted to zero;
8. stale last-known price used for new-risk valuation;
9. multi-hop conversion graph activated in v1;
10. future quote-currency expansion without new profile/review.

## 14. Finding Disposition

```text
RT-RISK-002 = REMEDIATED_AT_DESIGN_CANDIDATE_LEVEL
INITIAL_RISK_BASE_CURRENCY = USD
INITIAL_US_EQUITIES_CURRENCY = USD
INITIAL_CRYPTO_QUOTE_ASSET = USD ONLY
MULTI_HOP_CONVERSION = OUT_OF_SCOPE v1
```
