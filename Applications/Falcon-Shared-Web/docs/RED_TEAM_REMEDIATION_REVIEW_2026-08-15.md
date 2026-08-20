# Shared Falcon Web — Post-Remediation Red Team Review

Date: 2026-08-15  
Branch: `web-development`  
Writable scope: `applications/shared/web/**` only  
Review basis: remediation following `RED_TEAM_ALL_WEB_2026-08-15.md`

## Executive result

The production-blocking source defects identified by the first Red Team have been remediated in the Shared Web source and dedicated negative/security tests have been added.

This review does **not** claim full executable verification. A full checkout/test run could not be performed in the current execution environment because GitHub DNS resolution failed (`Could not resolve host: github.com`). Therefore `npm test`, `npm run check`, browser accessibility testing, and governed runtime verification remain pending evidence.

```text
WEB_SOURCE_RED_TEAM_BLOCKERS = REMEDIATED
WEB_SOURCE_SECURITY_DIRECTION = PASS_WITH_EXECUTABLE_VERIFICATION_PENDING
WEB_EXECUTABLE_GOVERNED_VERIFICATION = PENDING
WEB_EXTERNAL_RUNTIME_ACTIVATION = FAIL_CLOSED
WEB_AUTHORITATIVE_IDENTITY_RUNTIME = FAIL_CLOSED
WEB_PRODUCTION_READY = NO
FCR_CLOSURE = NOT_CLAIMED
```

---

## Finding disposition

### RT-WEB-001 — CRITICAL — Dynamic HTML injection / DOM-stored XSS

**Disposition: SOURCE REMEDIATED / EXECUTABLE SECURITY VERIFICATION PENDING**

Implemented:
- centralized `src/security/safe-html.js` output-encoding boundary;
- dynamic incident/customer/support/timeline content encoded;
- Owner/service/incident projections encoded;
- portfolio/activity values encoded;
- AI analysis/horizon/strategy/school/synthesis values encoded;
- dashboard/alerts/position/trade values encoded;
- catalog and route placeholder values encoded;
- hostile-input tests include script/image event, SVG, iframe/quote, encoded markup and RTL payloads.

No regex sanitizer is used as the security boundary.

### RT-WEB-002 — HIGH — Owner/Support routes lacked route guard

**Disposition: SOURCE REMEDIATED / FCR-0152 RUNTIME DEPENDENCY PRESERVED**

Implemented:
- `isAuthoritativeSession()` requires authenticated state, authoritative-session evidence and principal identity;
- `canAccessRoute()` rejects protected workspace routes without authoritative session;
- Owner routes additionally require `PROJECT_OWNER` role;
- direct hash navigation no longer renders protected content when no authoritative session exists;
- default auth adapter remains unavailable/fail-closed, so no live identity is fabricated while FCR-0152 remains Foundation-held.

### RT-WEB-003 — HIGH — Five-minute escalation cancelled by dismiss

**Disposition: REMEDIATED**

`ownerDelayAlert()` now uses actual viewed time + absence of reply. Dismiss/minimize does not cancel the timer. Tests explicitly cover dismissed/no-reply reaching escalation.

Owner presentation reports the observable fact only and does not infer that the customer ignored, understood or rejected the message.

### RT-WEB-004 — HIGH — Screenshot no-secret policy trusted caller metadata

**Disposition: SOURCE FAIL-CLOSED REMEDIATION COMPLETE / GOVERNED SCANNER BINDING PENDING**

A screenshot is accepted only when:
- exactly one file is presented;
- it is not already flagged as secret-bearing;
- `securityScanState == CLEAN`;
- scan provenance is exactly `GOVERNED_UPLOAD_SECURITY_SCANNER`.

Caller metadata alone is insufficient. Until a governed scanner/upload path exists, screenshot transport remains unavailable rather than accepting unverified content.

### RT-WEB-005 — HIGH — Portfolio v1 validation was permissive

**Disposition: SOURCE REMEDIATED / GOVERNED RUNTIME BINDING PENDING**

Implemented exact Web-side validation for:
- required-but-nullable field presence;
- broker-account scope;
- exact contract/version;
- truth/freshness/completeness/availability/evidence/reason;
- pagination token invariants;
- activity lifecycle states;
- performance history item semantics;
- portfolio update kind and correction/supersession lineage.

Validated payloads are cloned and deeply frozen after validation.

### RT-WEB-006 — HIGH — Support takeover helper was not authorization

**Disposition: SOURCE REMEDIATED / FCR-0152 RUNTIME DEPENDENCY PRESERVED**

Support participation now requires:
- explicit takeover;
- visible Support identity;
- authoritative Support session;
- non-empty Support principal identity;
- exact `INCIDENT_SUPPORT_TAKEOVER` capability.

Owner UI disables takeover unless the current authoritative session carries a permitted Support/Owner role and the exact capability. Client booleans alone cannot enable Support transport semantics.

### RT-WEB-007 — MEDIUM — No hostile-output security suite

**Disposition: REMEDIATED AT SOURCE/TEST ARTIFACT LEVEL**

Added `tests/security-output-encoding.test.mjs`, including malicious incident, portfolio, activity, analysis, Owner and route inputs.

Executable run remains pending environmental access.

### RT-WEB-008 — MEDIUM — Preview/demo and authoritative composition ambiguity

**Disposition: SOURCE REMEDIATED**

Added `src/core/data-source-profile.js` with mutually exclusive `PREVIEW` and `AUTHORITATIVE` modes.

Rules:
- preview data and authoritative data cannot coexist;
- authoritative mode rejects preview data;
- authoritative mode without an authoritative source fails closed to unavailable data;
- preview labeling is driven by explicit source mode rather than inferred from the displayed values.

### RT-WEB-009 — MEDIUM — Unavailable incident/support actions looked actionable

**Disposition: SOURCE REMEDIATED**

Screenshot, voice and text-reply actions are independently capability-gated and rendered `disabled aria-disabled=true` when unavailable. Support takeover uses the same fail-closed presentation behavior.

Minimize remains a Web-local interaction and does not constitute reply, acknowledgement, authorization or resolution.

### RT-WEB-010 — MEDIUM — On-demand analysis result validation was permissive

**Disposition: SOURCE REMEDIATED**

Added strict `bindOnDemandAnalysisResultV1()` validation. Completed/partial results require an analysis projection; non-completed states cannot smuggle an active projection; required request/correlation/result/instrument/intent/as-of/limitations fields are enforced. Malformed input fails presentation closed with `MALFORMED_APPLICATION_RESULT`.

### RT-WEB-011 — MEDIUM — Verification/accessibility/localization gaps

**Disposition: PARTIALLY REMEDIATED / EXECUTABLE GOVERNED VERIFICATION STILL PENDING**

Added source-level smoke coverage for:
- `aria-disabled` on unavailable incident/Support controls;
- accessible reply input labels;
- Arabic/English presence of new security/escalation copy;
- Arabic no-English fallback for the new incident-unavailable state;
- existing reduced-motion CSS remains present.

Still required before governed completion:
- actual full `npm test`;
- actual full `npm run check`;
- browser keyboard/focus review;
- browser accessibility review;
- complete Arabic/English visual review;
- production-profile/runtime verification.

This is the principal remaining Web-owned verification gate.

### RT-WEB-012 — LOW — Shallow freezing

**Disposition: REMEDIATED**

Added `src/core/immutable.js::deepFreeze()` and applied clone + deep-freeze to validated portfolio and on-demand-analysis contract data. Tests assert nested objects are frozen.

### RT-WEB-013 — LOW — Historical planning may be mistaken for current policy

**Disposition: REMEDIATED BY HANDOVER PRECEDENCE**

Historical files remain audit evidence. Current FCR issue body/header and latest Owner decisions control current implementation. The post-remediation handover restates that precedence.

---

## Preserved boundaries

The remediation does not grant or activate:
- external market-data egress;
- provider/broker connectivity;
- provider API credentials;
- production deployment;
- Trading/universe/capital/execution authority;
- Foundation/Application write authority;
- live OIDC/session/MFA truth;
- Support authority without an authoritative identity/capability binding.

The Web market-data boundary remains presentation-only and cannot feed raw Web provider observations back into FSATS.

## Verification truth

The source/test review is materially stronger after remediation, but no full executable PASS is claimed. The available execution environment could not clone the repository because DNS resolution for GitHub failed. That limitation cannot be converted into a PASS.

## Post-remediation decision

```text
RT_WEB_001 = SOURCE_REMEDIATED
RT_WEB_002 = SOURCE_REMEDIATED_FCR0152_PENDING
RT_WEB_003 = REMEDIATED
RT_WEB_004 = SOURCE_FAIL_CLOSED_SCANNER_BINDING_PENDING
RT_WEB_005 = SOURCE_REMEDIATED_RUNTIME_BINDING_PENDING
RT_WEB_006 = SOURCE_REMEDIATED_FCR0152_PENDING
RT_WEB_007 = TEST_ARTIFACTS_ADDED_EXECUTION_PENDING
RT_WEB_008 = SOURCE_REMEDIATED
RT_WEB_009 = SOURCE_REMEDIATED
RT_WEB_010 = SOURCE_REMEDIATED
RT_WEB_011 = PARTIAL_EXECUTABLE_GOVERNED_VERIFICATION_PENDING
RT_WEB_012 = REMEDIATED
RT_WEB_013 = REMEDIATED

CRITICAL_SOURCE_FINDINGS_OPEN = 0
HIGH_SOURCE_FINDINGS_OPEN = 0
WEB_SOURCE_REMEDIATION = COMPLETE_FOR_IDENTIFIED_RED_TEAM_BLOCKERS
FULL_EXECUTABLE_VERIFICATION = PENDING
PRODUCTION_READINESS = NOT_GRANTED
FCR_CLOSURE_ELIGIBILITY = NOT_YET_CLAIMED
```
