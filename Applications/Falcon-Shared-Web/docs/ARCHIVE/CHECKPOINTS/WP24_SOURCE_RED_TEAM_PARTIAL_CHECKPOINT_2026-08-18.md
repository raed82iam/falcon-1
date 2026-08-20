# Shared Falcon Web — WP-24 Source Red Team Partial Checkpoint

Date: 2026-08-18
Branch: `web-development`
Scope: `applications/shared/web/**`
Status: `SOURCE_RED_TEAM_PARTIAL_COMPLETE / FULL_CURRENT_HEAD_EXECUTABLE_AND_BROWSER_RED_TEAM_PENDING / NOT_OWNER_CLOSED`

## Purpose

Record the source-level adversarial review completed while Foundation FCR-0241 remains `Waiting On: FOUNDATION`. This checkpoint is not the final WP-24 closure and does not claim full current-HEAD executable or browser verification.

## Reviewed source areas

- Owner Command Center truthfulness and unavailable-state handling.
- Owner Approvals / standing-policy / proposal / rollback presentation.
- Owner emergency AI request/outcome separation.
- Shared Web MSA / Customer Support LSA responsibility boundaries.
- Owner request routing and compound-request classification.
- Customer explanation and tenant/session isolation policy.
- Incident scanner / voice / persistence / Support runtime boundaries.
- My Applications entitlement separation and subscription/tier presentation.
- FSATS analysis presentation truth/freshness policy.
- Portfolio/activity truth/freshness/correction/supersession presentation.
- Provider presentation readiness and fail-closed runtime binding state.

## Findings remediated during this review

1. Owner Command Center contained hard-coded user/audit operational facts.
   - Removed.
   - Unavailable data now remains unavailable.

2. Owner health could be styled positively even when unavailable.
   - Remediated with explicit unavailable styling.

3. Owner controls without governed runtime paths appeared actionable.
   - Remediated by disabling unbound controls.

4. Owner Approval disposition tone could render `AUTO_REJECTED` positively because `AUTO` matched before rejection.
   - Rejection/failure/revocation now take precedence.

5. Standing policies could appear `Active` solely because `revoked=false` even if expired/non-current.
   - Expired/non-current policies are fail-closed and cannot be treated as active.

6. Manual Owner review control appeared actionable without a governed manual-review action path.
   - Control is disabled until a real bound action path exists.

7. Malformed proposal rendering could risk taking down the whole Owner approvals page.
   - Proposal display now degrades fail-closed to manual review.

8. Owner request ownership classification selected the first matching keyword owner.
   - Mixed/ambiguous ownership hints now return `UNKNOWN` and require Owner clarification.
   - Exact scoped target paths retain precedence where supplied.

9. Screenshot scanner and local voice runtime could previously be discovered through global runtime names.
   - Global discovery removed; explicit composition injection is required.

10. Authoritative incident runtime could otherwise fall back to local IndexedDB semantics.
    - Production/authoritative persistence now requires explicit tenant-scoped persistence binding; preview may use local IndexedDB.

11. Support request recording could be confused with Support delivery.
    - Governed Support transport acceptance is separated from local recording.

## Current source invariants

```text
OWNER_VIEW != OWNER_APPROVAL
OWNER_SILENCE != OWNER_APPROVAL
AUTO_ACCEPT_ELIGIBLE != AUTO_ACCEPTED
AUTO_ACCEPTED != EXECUTION_AUTHORITY
ROLLBACK_REQUESTED != ROLLBACK_ACCEPTED != ROLLBACK_COMPLETED
REQUEST_SENT != ACTION_ACCEPTED != ACTION_COMPLETED
WEB_MSA_SELF_AWARENESS != AUTHORITY
AMBIGUOUS_OWNER_REQUEST != ROUTABLE_WORK
CARD_VISIBLE != ENTITLED
FEATURE_ACCESS != ACTION_AUTHORIZATION
PREVIEW_PERSISTENCE != PRODUCTION_TENANT_PERSISTENCE
SUPPORT_REQUEST_RECORDED != SUPPORT_REQUEST_DELIVERED
GLOBAL_AI_KILL != FALCON_SHUTDOWN
WEB_PRESENTATION_DATA != FSATS_OPERATIONAL_DATA
```

## Source verification registration

All newly added source modules are included in the current `npm run check` command, including:

- `src/core/web-awareness-model.js`
- `src/core/owner-request-router.js`
- `src/core/customer-explanation-policy.js`
- `src/core/ports/incident-support-transport-port.js`
- `src/incidents/incident-persistence-binding.js`
- `src/incidents/incident-accessibility.js`
- `src/features/ai/analysis-presentation-policy.js`
- current Owner governance / Owner Command Center / Owner emergency source.

Tests are registered under the existing `node --test tests/*.test.mjs` glob.

## Current Red Team disposition

```text
OPEN_SOURCE_CRITICAL = 0
OPEN_SOURCE_HIGH = 0
OPEN_SOURCE_MEDIUM = 0
OPEN_SOURCE_PRODUCT_LOW = 0
```

This statement applies only to the source findings reviewed in this checkpoint after remediation. It is not a final WP-24 result.

## Pending before WP-24 closure

- exact current-HEAD checkout-backed `npm test`;
- exact current-HEAD `npm run check`;
- browser RTL/LTR/keyboard/mobile verification;
- runtime-dependent provider/incident/voice/support verification where exact bindings exist;
- renewed FCR-0241/0237/0238 Web verification after Foundation handoff;
- broad final Red Team over the exact frozen WP-23 candidate.

```text
SOURCE_RED_TEAM_PARTIAL_COMPLETE != WP24_COMPLETE
WP24_COMPLETE != OWNER_ACCEPTANCE
OWNER_ACCEPTANCE != DEPLOYMENT_AUTHORITY
```
