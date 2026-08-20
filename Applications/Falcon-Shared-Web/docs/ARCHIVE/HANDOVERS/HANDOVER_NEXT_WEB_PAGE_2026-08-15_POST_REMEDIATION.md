# Falcon Shared Web — Handover After Red Team Remediation

Date: 2026-08-15  
Repository: `raed82iam/Falcon`  
Writable branch: `web-development`  
Writable scope: `applications/shared/web/**` only

## Continuity rule

Treat this as direct continuation of the existing Shared Web workstream. Do not redesign previously accepted architecture. Before any new Web analysis or implementation, perform a fresh live FCR check and read the canonical current issue body/header. Historical Web planning remains audit history when superseded by a newer Owner decision or canonical FCR body.

## Current status

The first full Red Team report (`RED_TEAM_ALL_WEB_2026-08-15.md`) found one CRITICAL and multiple HIGH/MEDIUM/LOW findings. Those identified source-level blockers have now been remediated.

Controlling post-remediation review:
`applications/shared/web/docs/RED_TEAM_REMEDIATION_REVIEW_2026-08-15.md`

Current truth:

```text
WEB_SOURCE_RED_TEAM_BLOCKERS = REMEDIATED
CRITICAL_SOURCE_FINDINGS_OPEN = 0
HIGH_SOURCE_FINDINGS_OPEN = 0
FULL_EXECUTABLE_GOVERNED_VERIFICATION = PENDING
PRODUCTION_DEPLOYMENT = NOT_AUTHORIZED
FCR_CLOSURE = NOT_CLAIMED
```

A full executable `npm test` / `npm run check` run is still not evidenced because the current execution environment could not resolve `github.com` for repository checkout. Do not reinterpret that as PASS or FAIL of the test suite.

## Important remediation now present

### Output security
- `src/security/safe-html.js` provides the centralized plain-text HTML-encoding boundary.
- Incident/customer/support/timeline, Owner, portfolio/activity, AI, dashboard, alert, catalog and route-placeholder dynamic values are encoded.
- `tests/security-output-encoding.test.mjs` contains hostile markup cases including script/image events, SVG, iframe/quotes, encoded markup and RTL payloads.

### Auth / route policy
- `src/auth.js` requires authoritative authenticated session evidence plus principal identity.
- protected user routes fail closed without an authoritative session.
- Owner routes additionally require `PROJECT_OWNER`.
- FCR-0152 remains the blocker for real OIDC/session/MFA identity binding.

### Incident semantics
- five-minute HIGH no-reply escalation is based on actual view + no reply; dismiss/minimize does not cancel it.
- Owner presentation reports observable no-reply facts without inferring intent/understanding.
- screenshot upload remains fail-closed unless one file has governed `CLEAN` scanner evidence.
- incident screenshot/voice/text controls are independently capability-gated and disabled when unavailable.
- Support takeover requires authoritative Support session/principal/exact takeover capability and remains distinct from execution authority or incident resolution.

### FSATS portfolio contract
- required-nullable presence is enforced;
- exact account/contract/version/truth/freshness/completeness/availability/evidence/reason semantics are validated;
- pagination invariants are enforced;
- order lifecycle states are preserved;
- performance history is validated;
- update correction/supersession lineage is validated;
- validated data is cloned and deeply frozen.

### On-demand analysis
- strict result binding exists;
- completed/partial require a projection;
- non-completed states cannot carry an active projection;
- malformed results fail closed to `MALFORMED_APPLICATION_RESULT`;
- no universe, Strategy, capital or execution side effect is created by Web.

### Demo/runtime separation
- `src/core/data-source-profile.js` makes PREVIEW and AUTHORITATIVE modes mutually exclusive.
- production/authoritative mode cannot silently consume demo fixtures.
- authoritative mode without source data fails closed.

## FCR-0095

Latest known canonical state at remediation review:
- `Status: APPLICATION_VERIFIED`
- `Waiting On: WEB`
- `WEB_IMPLEMENTATION = IN_PROGRESS`
- `WEB_GOVERNED_VERIFICATION = PENDING`
- Support takeover Owner decision is canonical.

Do not close until governed Web verification exists.

## Other Web FCRs

Current Web contract families still requiring final verification/closure evidence include:
- FCR-0125 chart/presentation market data
- FCR-0126 Trading overlays
- FCR-0127 on-demand analysis
- FCR-0128 Strategy/School catalog
- FCR-0130 detailed analysis
- FCR-0133 portfolio/positions/activity/performance

Foundation-held dependencies include:
- FCR-0152 identity/session/MFA
- FCR-0169 unified Falcon OS operational projection
- FCR-0173..0177 presentation WebSocket egress
- FCR-0196..0200 full-market universe/history/broad-stream destinations

No Foundation-held dependency may be bypassed by Web.

## Market-data architecture

Maintain strict separation:

```text
WEB PRESENTATION MARKET DATA
-> WebMarketDataPort
-> Web-owned governed provider route
-> Web display only

FSATS ANALYSIS / TRADING DATA
-> FSATS
-> FSAPMA
-> Application-owned semantics

WEB_PRESENTATION_DATA != FSATS_OPERATIONAL_DATA
```

Configured/pending presentation providers include Binance, Coinbase, Bybit, Alpaca IEX and Finnhub plus Stage-12 REST/broad-market destinations under FCR-0196..0200. Configuration is not activation.

## Credentials

Never request API keys, provider credentials, broker credentials, MFA codes or secrets in chat. When runtime activation eventually requires provider API credentials, identify the provider and required credential type and route it through the governed secret-storage path once that path exists.

## Next work

The next Web step is not another redesign. It is executable/governed verification and residual browser-level review:

1. obtain a usable checkout/execution environment;
2. run `npm test`;
3. run `npm run check`;
4. fix any actual failures found;
5. run keyboard/focus/accessibility review in a browser;
6. run Arabic/English visual/localization review;
7. re-run Web Red Team after actual executable evidence;
8. update every `Waiting On: WEB` FCR with exact evidence;
9. close only the FCRs whose implementation, binding and governed verification requirements are fully satisfied.

No production deployment or live external connection is authorized by this handover.
