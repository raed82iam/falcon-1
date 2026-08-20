# Current Free Provider Capability Matrix — 2026-08-13

**Package:** `FSATS-FMOF-PROPOSAL-001`  
**Evidence Type:** `POINT-IN-TIME EXTERNAL RESEARCH / DESIGN INPUT`  
**Status:** `NOT_RUNTIME_CERTIFICATION / NOT_OWNER_ACCEPTANCE / NOT_CONNECTIVITY_AUTHORITY`  
**Snapshot Date:** `2026-08-13 +03:00`  
**Branch:** `application-development`  
**Workspace:** `applications/docs/FSATS/NEW-2/`  
**Applies To:** the historical V1.3 thirteen-provider candidate portfolio and the current FMOF/FSAPMA redesign proposal  

---

# 1. Purpose

This artifact records a point-in-time view of the currently advertised free-access capabilities of the thirteen provider families carried forward from the FSATS V1.3 historical design reference.

Its purpose is not to declare one provider globally "best". Its purpose is to let FSAPMA answer a narrower question:

> Which currently certified provider route can satisfy the exact semantic requirements of this `DataProductRequest` at the required freshness, quality, entitlement, capacity and cost?

Provider capabilities, plans, rate limits, exchange entitlements and terms can change. Therefore this matrix is research evidence only. Every route must be re-certified before credential provisioning and again before runtime activation.

---

# 2. Controlling Interpretation

The thirteen provider families remain a **core candidate portfolio**, not thirteen interchangeable real-time feeds.

```text
CORE_PORTFOLIO_MEMBER
!= ACTIVE_ROUTE
!= CURRENTLY_FREE
!= REALTIME_CAPABLE
!= SEMANTICALLY_COMPATIBLE
!= LEGALLY_USABLE_FOR_EVERY_PURPOSE
!= INDEPENDENT_CONFIRMATION_SOURCE
```

The Owner's current zero-cost evaluation objective is preserved:

```text
PERSONAL_OWNER_ONLY_EVALUATION = TRUE
FREE_FIRST_PROVIDER_ORCHESTRATION = TRUE
AUTOMATIC_PAID_UPGRADE = FORBIDDEN
EXTERNAL_REDISTRIBUTION = FALSE
COMMERCIAL_SERVICE = FALSE
```

The Owner also intends to provision at least three API credentials/connections per provider where the provider's actual account, credential, entitlement and terms model legitimately permits that arrangement.

That is a **capacity target**, not permission to manufacture quota.

```text
THREE_KEYS != THREE_INDEPENDENT_QUOTA_DOMAINS
MULTIPLE_KEYS != PERMISSION_TO_BYPASS_PROVIDER_LIMITS
TECHNICAL_ACCESS != USAGE_RIGHT
```

For sources that do not use API keys, such as SEC `data.sec.gov`, the three-key concept is not applicable. Their published shared access rules remain controlling.

---

# 3. Current Capability Matrix

| # | Provider family | Current free-access character | Real-time / streaming at free level | Historical / EOD at free level | Current free quota / capacity evidence | Best zero-cost role in FSAPMA | Critical caution |
|---|---|---|---|---|---|---|---|
| 1 | **Alpaca** | Free Basic market-data plan for US stocks/ETFs, plus broker/Paper capabilities under separate account scopes | **Yes, but US equities are IEX-only**. Basic supports 30 WebSocket symbols. Options use indicative pricing rather than OPRA | Since 2016; Basic restricts the latest 15 minutes of historical data; historical API 200 calls/min | 30 WS equity symbols; 200 historical calls/min | Real-time IEX observation, streaming, recent/historical equity data where IEX semantics are sufficient; separately broker/Paper evidence when acting as broker | IEX-only cannot be presented as consolidated all-US-exchange/NBBO truth. Broker/account capacity is separately protected when used for execution |
| 2 | **SEC EDGAR** | Official public regulatory source; `data.sec.gov` REST JSON APIs require no API key | New filings/submissions are published promptly, but this is **regulatory event data**, not a price feed | Submission history, XBRL company facts/concepts/frames, indexes and archives | SEC fair-access guideline: no more than **10 requests/sec per user regardless of machines** | Authoritative regulatory filings, structured disclosures, XBRL, filing-event evidence | No API keys. Never multiply machines/clients to evade the global fair-access rule. Not a quote/bar provider |
| 3 | **FRED / ALFRED** | Official St. Louis Fed economic-data APIs; API key required | Not a tick/quote feed. Data follows economic release/update cadence | Strong macro time series; ALFRED/FRED vintage dates support point-in-time revision history | Distinct key per application; documentation says application users should use their own keys. Limits may be imposed dynamically | Macro series, economic releases, revision-aware and point-in-time research | Third-party series can carry separate copyright/use restrictions. Multiple keys must follow the stated application/user identity model rather than quota multiplication |
| 4 | **Finnhub** | Free personal-use tier | Free table advertises WebSocket for **50 symbols** and company-news real-time updates | Company news includes one year; free company profile v2 and US fundamental coverage are advertised. Broad free OHLC history is not asserted by the current pricing table | **60 API calls/minute**; 50 WebSocket symbols | News/non-price intelligence, company profile/fundamental support, limited streaming contribution subject to endpoint certification | Do not infer an unlisted free OHLC/tick entitlement from the vendor name alone. Certify each endpoint/product |
| 5 | **Financial Modeling Prep (FMP)** | Free Basic tier intended for testing/exploration | No free real-time backbone is established by current Basic pricing | **End-of-day historical data** plus profile/reference data; 150+ endpoints can be explored | **250 calls/day** | EOD/reference/profile support, low-rate verification and backfill where the exact endpoint is free | Current Basic pricing does not justify treating FMP as a free intraday/realtime source. Exact historical depth and endpoint entitlement must be certified |
| 6 | **Alpha Vantage** | Majority of datasets available on free API under a small standard daily limit | **No free US real-time or 15-minute delayed entitlement**. Default quote/current market endpoints are EOD unless paid entitlement is present | Broad historical/time-series, indicators and other datasets can be free depending on endpoint | **25 API requests/day** standard free limit | Low-rate historical/reference/indicator/fundamental/news/macro support and independent spot verification where semantics match | Realtime and 15-minute delayed US market data are premium/entitlement-controlled. Too little free quota for wide scanning |
| 7 | **Twelve Data** | Free Basic individual plan, internal non-display | **Yes**: real-time US equities/ETFs, forex and crypto; trial WebSocket capacity | Time-series/reference/technical indicators available subject to credits and product depth | **8 API credits/minute, 800/day + 8 trial WS credits** | Strong free multi-asset realtime complement, batch/reference/indicator support | Credit weights vary by endpoint. Individual plans are personal/internal and do not grant redistribution/commercial display |
| 8 | **Tiingo** | Free Starter individual/internal-use plan | IEX feed and WebSocket/REST support. A real-time derived reference price can be available without full IEX TOPS; raw TOPS bid/ask/last fields require current IEX entitlement | **30+ years historical** advertised; Tiingo EOD composite, crypto and news; news has 3 months queryable history plus data going forward | **500 unique symbols/month, 50 requests/hour, 1,000/day, 1 GB/month** | Very strong historical/EOD research source, IEX-derived real-time reference contribution, news; useful complement to Alpaca/Twelve Data | Derived real-time reference price is not equivalent to entitled raw TOPS/NBBO. Internal-use restriction must be respected |
| 9 | **Tradier Sandbox / Brokerage API** | Sandbox available for test/Paper workflows; real-time market data requires Tradier Brokerage account | **Sandbox is 15-minute delayed** for equities/options. Brokerage API is real-time consolidated for account holders | Sandbox and market endpoints usable for testing within product support | Per access token, per app+user: Market Data **120/min production, 60/min sandbox**; Standard 120/60; Trading 60/60 | Sandbox: delayed test/Paper market data. Brokerage account API: potential secondary market-data contributor only from certified spare capacity | User broker API has PRIMARY protected execution/account/reconciliation duty. Secondary market-data use must never consume protected execution capacity |
| 10 | **Massive** | Stocks Basic free, Individual use | Free Stocks Basic is **EOD**, not real-time; pricing advertises API surface but plan recency is EOD | **2 years historical**, full US ticker coverage, reference/corporate actions/technical indicators/minute aggregates at EOD recency | **5 API calls/minute** | Historical/EOD/reference research candidate only after rights certification | **Major rights gate**: current Market Data Terms state personal/non-business use and, unless separately agreed, display-only; non-display/derived investment-strategy use is restricted. Falcon automated strategy use must remain `RIGHTS_RESTRICTED` unless licensed/confirmed |
| 11 | **Nasdaq Data Link** | Mix of free/open and premium datasets | Platform has streaming/real-time APIs, but **free real-time market data is not assumed**. Availability is dataset-specific | Strong free/open research datasets; Tables data are not real-time and most update daily with about a one-day lag | Authenticated free Tables: **300 calls/10 sec, 2,000/10 min, 50,000/day; concurrency 1** | On-demand economic/research/alternative datasets and specialized historical tables | Dataset-level entitlement determines what is free. High API rate limit does not mean premium datasets become free or realtime |
| 12 | **Marketstack** | Free plan | No free intraday/realtime | **EOD, one year history**, splits/dividends, ticker/exchange information | Current pricing/signup: **100 requests/month** | Very-low-rate EOD/reference backup and occasional verification | FAQ contains inconsistent text mentioning 1,000 requests while current pricing/signup and another FAQ sentence say 100. Treat 100/month as current planning evidence and recertify before use |
| 13 | **EODHD** | Free Starter, personal use | General free account is not a broad real-time feed. Demo key exposes all API types only for a small fixed demo-symbol set | Any ticker EOD history limited to **past year** on free registered plan; splits/dividends included in Free Starter | **20 API calls/day**, published Free Starter also lists 20/min | Low-rate EOD backup, limited verification, demo/testing for selected symbols | Demo real-time access for a few symbols must not be generalized to the full free account universe |

---

# 4. Portfolio Role Map

The current free portfolio should therefore be interpreted by **capability class**, not provider count.

## 4.1 Real-time / streaming candidates

Current zero-cost candidates include, subject to exact endpoint and rights certification:

- Alpaca Basic for US equity IEX semantics;
- Twelve Data Basic for quota-limited real-time US equities/ETFs, forex and crypto;
- Tiingo IEX/derived reference-price paths under its current entitlement model;
- Finnhub limited WebSocket contribution where the requested endpoint semantics are certified;
- a user's Tradier Brokerage market-data capacity only when that account exists, the route is entitled, and protected execution/account capacity is not affected.

These are **not equivalent feeds**. IEX-only, derived-reference, venue-specific and consolidated semantics must stay distinct.

## 4.2 Historical / EOD candidates

Strong or useful free historical/EOD roles currently include:

- Tiingo: long-depth 30+ year historical capability;
- Alpaca: history since 2016, with Basic latest-15-minute restriction;
- Massive: two-year EOD-recency stock history subject to rights gate;
- FMP: EOD history under Basic, endpoint/depth certification required;
- Alpha Vantage: broad low-rate historical/time-series access;
- Marketstack: one-year EOD at very low monthly quota;
- EODHD: one-year EOD at very low daily quota.

## 4.3 Official / specialized truth sources

- SEC EDGAR: filings, submissions and XBRL regulatory evidence;
- FRED / ALFRED: macroeconomic series and point-in-time vintage/revision evidence;
- Nasdaq Data Link: dataset-specific research/economic/alternative tables and open datasets.

## 4.4 News / non-price intelligence

- Finnhub: free company news with one-year coverage and real-time updates;
- Tiingo: three months queryable news plus data going forward;
- Alpha Vantage may contribute low-rate news/sentiment or other datasets where the exact endpoint remains free and certified.

---

# 5. Multi-Credential / Quota-Domain Rule

The Owner's intended provider capacity model is:

```text
13 CORE PROVIDER FAMILIES
    |
    +-- target 3+ dedicated provider API credentials/routes each WHERE LEGITIMATELY SUPPORTED
    |
    +-- each credential/route mapped to its actual QuotaDomainIdentity
    |
    +-- never assume credential count equals independent quota count
```

FSAPMA must represent at least:

```text
ProviderFamilyId
ServiceId
PlanId
CredentialIdentity
RouteIdentity
QuotaDomainIdentity
QuotaScope
RateLimitModel
RemainingCapacity
ResetInstant
ConcurrencyLimit
WebSocketLimit
BatchLimit
EndpointWeight
PurposeEntitlement
UsageRightsProfile
UpstreamLineageIdentity
```

Examples:

- SEC: no key, one published fair-access boundary; multiplying machines is explicitly not a new quota domain.
- FRED: keys identify applications/users according to its documentation; extra keys are not free quota multipliers.
- Tradier: limits are documented per access token and per app+user; execution/trading capacity remains separately protected.
- Other providers: independent-quota behavior is `UNKNOWN_UNTIL_CERTIFIED` unless current provider documentation and account behavior prove otherwise.

---

# 6. Required Re-Certification Before Runtime Use

Every provider route must carry a dated `ProviderEndpointCapabilityCertification` covering at least:

1. provider/service/plan identity;
2. endpoint and transport;
3. exact canonical Data Products supported;
4. markets/instruments;
5. real-time/delayed/EOD/historical semantics;
6. granularity and history depth;
7. required fields and known field limitations;
8. venue/consolidation semantics;
9. adjustment/session/timezone semantics;
10. rate/quota/concurrency/batch/stream limits;
11. credential and `QuotaDomainIdentity` behavior;
12. personal/internal/non-display/display/redistribution/derived-use rights;
13. cost class and automatic-paid-upgrade prohibition;
14. upstream lineage and independence classification;
15. observed quality, latency, freshness and correction behavior;
16. certification evidence source and timestamp;
17. expiry/review trigger.

A stale capability certification must not silently continue as active truth.

---

# 7. Official Source Evidence Used For This Snapshot

The matrix above was prepared from current official provider pages available on 2026-08-13. Key sources include:

- Alpaca Market Data API plan comparison: `https://docs.alpaca.markets/us/docs/about-market-data-api`
- SEC EDGAR APIs: `https://www.sec.gov/search-filings/edgar-application-programming-interfaces`
- SEC Developer Resources / Fair Access: `https://www.sec.gov/about/developer-resources`
- FRED API overview / API keys / terms: `https://fred.stlouisfed.org/docs/api/fred/overview.html`, `https://fred.stlouisfed.org/docs/api/fred/v2/api_key.html`, `https://fred.stlouisfed.org/docs/api/terms_of_use.html`
- Finnhub pricing: `https://finnhub.io/pricing`
- Financial Modeling Prep pricing: `https://site.financialmodelingprep.com/developer/docs/pricing/`
- Alpha Vantage support / documentation / premium entitlement: `https://www.alphavantage.co/support/`, `https://www.alphavantage.co/documentation/`, `https://www.alphavantage.co/premium/`
- Twelve Data individual pricing and usage guidance: `https://twelvedata.com/pricing`, `https://support.twelvedata.com/en/articles/5332349-commercial-and-personal-usage`
- Tiingo pricing and IEX documentation: `https://www.tiingo.com/about/pricing`, `https://www.tiingo.com/documentation/iex`
- Tradier market data and rate limiting: `https://docs.tradier.com/docs/market-data`, `https://docs.tradier.com/docs/rate-limiting`
- Massive stock pricing and Market Data Terms: `https://massive.com/pricing?product=stocks`, `https://massive.com/legal/market-data-terms-of-service`
- Nasdaq Data Link getting started / rate limits / Tables API: `https://docs.data.nasdaq.com/docs/getting-started`, `https://docs.data.nasdaq.com/docs/rate-limits-1`, `https://docs.data.nasdaq.com/docs/api-and-analysis-tools-for-tables-data`
- Marketstack pricing and signup: `https://marketstack.com/pricing/`, `https://marketstack.com/signup/free`
- EODHD free historical plan and API documentation: `https://eodhd.com/lp/historical-eod-api`, `https://eodhd.com/financial-apis/api-for-historical-data-and-volumes`

---

# 8. Non-Grant

This artifact does not:

- activate any provider;
- authorize acquisition of credentials;
- authorize provider or broker connectivity;
- certify any provider for live Trading;
- authorize Paper, Tiny Live, Live or deployment;
- authorize paid subscriptions;
- make the point-in-time capability matrix permanent;
- change the accepted R7 baseline;
- constitute overall Owner acceptance of FMOF.

It is dated research evidence supporting the active design proposal only.
