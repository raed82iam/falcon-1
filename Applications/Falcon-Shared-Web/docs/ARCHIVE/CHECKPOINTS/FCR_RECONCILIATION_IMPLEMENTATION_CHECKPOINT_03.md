# Shared Web FCR Reconciliation — Implementation Checkpoint 03

Date: 2026-08-15  
Branch: `web-development`  
Scope: `applications/shared/web/**` only  
Checkpoint HEAD at preparation: `58a1e502ce5c63abc40a17a954fdd95a89660892`

## Purpose

This checkpoint reconciles the current Web implementation against every FCR whose canonical current header has `Waiting On: WEB`, plus the Foundation-held dependencies that constrain truthful Web activation.

The implementation remains fail-closed where runtime authority or authoritative identity/system truth does not yet exist.

## FCR-0095 — Incident/customer/support interaction

Implemented/reconciled:
- persistent incident conversation model;
- explicit modes `FALCON_ACTIVE`, `SUPPORT_ESCALATED_FALCON_ACTIVE`, `SUPPORT_TAKEOVER`;
- Support can type only after explicit visible takeover semantics;
- Falcon becomes silent observer during Support takeover;
- Support cannot impersonate Falcon;
- takeover/escalation/delivery/acknowledgement do not resolve an incident;
- credentials remain prohibited in chat;
- screenshot policy remains one-at-a-time and no-secrets;
- exact five-minute High incident viewed/no-reply alert calculation;
- Owner-reported contact attempts remain attributable human evidence, not telephony proof/broker truth;
- one-time customer return notice semantics preserve historical/current truth separation;
- critical unresolved/minimized visual treatment and reduced-motion fallback;
- Support takeover initiation is fail-closed in Owner UI until authoritative Support/session identity exists under the FCR-0152 dependency.

Important evidence commits include:
`7c3d2427bebebaad21a6c904b74c3726c3d60dcb`, `0d4749d43d006da3eabe01ff6f3bd1ce84312877`, `f67e9743f4ae4e54e52b2346923f0e1386905350`, `a151f0a8b39cc49a25e26b391630a81f2ae143ad`, `967e6a58e4a197239d22596ca91d4d4bb2b8a844`, `09f4eab92684ba59f9f4f70640b0457e5c3cd04f`.

## FCR-0125 — Chart/presentation market data

Implemented/reconciled:
- raw presentation market data separated from the FSATS runtime contract family;
- new `WebMarketDataPort` owns Web presentation-only universe/snapshot/history/stream needs;
- existing FCR-0173..0177 WebSocket destinations are registered fail-closed;
- FSATS `chart` remains compatibility-only rather than the default raw display path;
- full-market sourcing plan established:
  - US equities: full universe/history plus dynamic live window;
  - Crypto Spot: broad-market primary plus on-demand/secondary routes;
- normalized Web observation explicitly carries `presentationOnly=true` and `eligibleForFsatsInput=false`.

Additional exact destination gaps were raised as independent FCRs rather than hidden in implementation:
- FCR-0196 Alpaca US-equity universe;
- FCR-0197 Alpaca US-equity historical bars;
- FCR-0198 Binance Crypto Spot universe;
- FCR-0199 Binance Crypto Spot historical klines;
- FCR-0200 Binance broad-market mini ticker WebSocket.

Evidence includes:
`3a5f5fc0220e7e0c91eeee6c5096aabd4bbd8280`, `d871321d78b1a85ba63f358698298bc157a9b0ec`, `3ebabb42cefae22e6fdd39faf9c80adcd3a552b5`, `85aa040661ab3b03661fcf99c5ed59967c73fabc`.

No external connectivity is activated.

## FCR-0126 — Trading School/Strategy overlays

Implemented/reconciled:
- provider-neutral overlay presenter;
- only Application-supplied element types are accepted: `POINT`, `PRICE_LEVEL`, `HORIZONTAL_LINE`, `VERTICAL_LINE`, `ZONE`, `SERIES`, `MARKER`, `ANNOTATION`;
- non-applicable overlays are not rendered as current Trading truth;
- unknown element types fail closed;
- update handling requires exact `OverlayProjectionId` and recognized update type;
- Web does not reconstruct Strategy/School logic from rendering primitives.

Evidence: `11a9830404c1d560df07f46b975875d980a6f873`, tests `2f4e64885d428bd38fea600ecf5214546fe819e7`.

## FCR-0127 — On-demand analysis

Implemented/reconciled:
- exact Web-side intent builder for `FSATS.WebOnDemandAnalysisRequest.v1` semantic fields;
- request identity, correlation, instrument reference, optional hints, analysis intent and request time preserved;
- explicit no-side-effect flags document that the Web request creates no universe mutation, Strategy activation, capital reservation, order intent or execution authority;
- result presenter preserves `COMPLETED`, `PARTIAL`, `UNAVAILABLE`, `UNSUPPORTED`, `NEEDS_CLARIFICATION`, `REJECTED` and fails closed for unknown states.

Evidence: `8138c78d6149c5b330c7a458737e9adca07e00ac`, tests `f8644604d946e282b8514b4bd71b2c75c16e1b3f`.

No executable cross-Application route is invented.

## FCR-0128 — Dynamic Strategy/School catalog

Current implementation preserves the authoritative catalog boundary:
- catalog entries are supplied externally rather than hard-coded as product truth;
- retired/replaced entries are not offered as current;
- `CATALOG_PRESENT + NOT_APPLICABLE -> VISIBLE_DISABLED_WITH_REASON` is preserved;
- visibility does not imply activation, entitlement or Trading authority.

The Web surface now localizes the School/Strategy selector presentation without changing Application semantics.

## FCR-0130 — Detailed AI analysis

Implemented/reconciled:
- AI surface consumes supplied detailed-analysis data;
- horizon, Strategy, School and synthesis sections preserve supplied values;
- missing values remain explicit;
- structured synthesis no longer leaks object placeholders;
- short summary is shown first and full detail remains under `عرض التحليل الكامل` / `Show full analysis`;
- AI explanation does not become analysis truth or Trading authority.

Evidence: `097031e80f069b65f64b70baa1abaf0d9935f102`, `58a1e502ce5c63abc40a17a954fdd95a89660892`, tests `61758163a2e4a519fedce4f302330a204d911f00`.

## FCR-0133 — Portfolio / positions / activity / performance

Implemented/reconciled against Application commit `b922ef446dd0b99257acddfedfe81193ac1489fb`:
- exact v1 broker-account scope validation;
- exact contract/version fail-closed validation;
- exact truth/freshness/completeness/availability semantics;
- null numeric values are preserved and never zero-filled;
- exact order states include `UNKNOWN_BROKER_OUTCOME` and `CANCELLED`;
- hard-coded `FILLED` removed from activity UI;
- hard-coded portfolio/dashboard performance percentages removed;
- presenter understands Application envelope fields and exact `order.value` / `instrument.value` identities.

Evidence includes:
`35f0367c3da19790d0cdeac505b5276d6172795e`, `9f59b71f549f0722da10f082774528ed1a38f711`, `45c99c22c9c2773aa2809623d2cb5aa10af1bf89`, `80e4cfdad3cbf4948ba6e31194fa077d8b491274`, `549d69ddaeef21f65f64c716cdc9c101c7b0a403`, `5d49df0b2e88488017b41ca46fcdeaa9cbeadcaa`.

The Application payload semantics are now materialized Web-side, but the executable public runtime transport remains unmaterialized/ungranted by the Application contract. Web does not invent one.

## Foundation-held activation boundaries preserved

- FCR-0152: authoritative external identity/Falcon identity/session/MFA remains Foundation-held; live OIDC/MFA and Support-role identity binding remain fail-closed.
- FCR-0169: authoritative unified Falcon OS operational projection remains Foundation-held; Web must not synthesize Foundation internals into authoritative OS truth.
- FCR-0173..0177 and FCR-0196..0200: external presentation-data egress remains Foundation Stage 12 held; route configuration does not activate connectivity.

## Verification status

Source and test artifacts were updated to cover the reconciled semantics, including architecture separation, portfolio v1 payloads, market-data plan, incident/support semantics, on-demand analysis and Trading overlays.

However, full executable `npm test` / `npm run check` evidence is **not claimed by this checkpoint**. The current execution environment could not resolve GitHub for a full checkout, so governed executable verification remains pending. No CI PASS is invented.

Therefore:

```text
WEB_FCR_RECONCILIATION_IMPLEMENTATION = SUBSTANTIALLY_ADVANCED
WEB_SOURCE_BINDINGS = IMPLEMENTED_WHERE_PUBLIC_SEMANTICS_EXIST
EXTERNAL_RUNTIME_ACTIVATION = FAIL_CLOSED
AUTHORITATIVE_IDENTITY_BINDING = FAIL_CLOSED
FULL_EXECUTABLE_GOVERNED_VERIFICATION = PENDING
FCR_CLOSURE = NOT_CLAIMED
```
