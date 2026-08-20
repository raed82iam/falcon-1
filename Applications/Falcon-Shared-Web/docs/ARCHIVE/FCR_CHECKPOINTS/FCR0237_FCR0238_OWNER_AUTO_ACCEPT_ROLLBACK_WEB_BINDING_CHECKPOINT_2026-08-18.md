# FCR-0237 / FCR-0238 Owner Auto-Accept + Rollback Web Binding Checkpoint — 2026-08-18

**Workstream:** Shared Falcon Web Application  
**Branch:** `web-development`  
**Scope:** `applications/shared/web/**` only  
**State:** `WEB_SEMANTIC_BINDING_AND_UI_IMPLEMENTED / LIVE_FOUNDATION_REQUEST_TRANSPORT_BLOCKED_BY_FCR-0241 / FULL_CURRENT_HEAD_EXECUTABLE_VERIFICATION_PENDING`

## Owner direction implemented

The Web source now preserves the Project Owner direction that:

```text
APPLICATION_AI_PROPOSAL != OWNER_DECISION
APPLICATION_AI_SELF_APPROVAL = FORBIDDEN
APPLICATION_AI_SELF_ROLLBACK_AUTHORITY = FORBIDDEN
WEB_OWNER_COMMAND_CENTER = ONLY_OWNER_DERIVED_DECISION_SURFACE
AUTO_ACCEPT_ELIGIBLE -> GOVERNED_BACKUP_OR_ROLLBACK_PLAN_REQUIRED
AUTO_ACCEPT != EXECUTION_AUTHORITY
AUTO_ACCEPT != DEPLOYMENT_AUTHORITY
AUTO_ACCEPT != BUSINESS_AUTHORITY
ROLLBACK_REQUEST != ROLLBACK_AUTHORIZATION != ROLLBACK_EXECUTION
ROLLBACK_COMPLETED != AUTHORITY_RESTORED
ROLLBACK_COMPLETED != TRUST_RESTORED
```

## Application FCR-0238 consumption

Added:

- `src/contracts/owner-update-governance-v1.js`
- `src/contracts/owner-update-proposal-boundary-v1.js`

The Web contract adapter mirrors the Application-owned v1 taxonomy and minimum review floor from exact Application candidate:

`1b593a7acb2be01dd2ad6cd124ba6c1df3272ebe`

Standing-preapproval evaluation is locally eligible only for:

- Maintenance
- ModelRefresh
- ParameterTuning
- PresentationOnlySuggestion

and only when all other Application guards pass.

The Web cannot weaken manual review for StrategyRevision, DataSourceChange, BusinessRuleChange, RiskRuleChange, ExecutionBehaviorChange, AuthorityOrSecurityChange, DeploymentOrAdoptionChange, AiSelfDevelopment, or Unknown.

Any high/non-low impact or business/risk/execution/security/authority/deployment behavior change forces manual review.

The exact rollback plan is validated for proposal/change/previous-state/scope binding plus current/compatible/validated state and required evidence.

The ingress guard additionally rejects producer-added self-authority claims such as `AUTO_ACCEPTED`, `OWNER_APPROVED`, `ROLLBACK_AUTHORIZED`, `ProducerClaimsAutoAccept`, or `ProducerClaimsRollbackAuthority` rather than silently ignoring them.

Local Application semantic evaluation returns only:

`STANDING_PREAPPROVAL_ELIGIBLE_FOR_OWNER_DECISION`

It never returns Owner approval.

## Foundation FCR-0237 result consumption

Added:

`src/adapters/foundation-owner-standing-preapproval-v1.js`

It validates Foundation-owned:

- Owner-derived auto-accept decisions;
- standing-policy management decisions;
- Owner rollback-order authorization decisions;
- rollback execution status projections.

Accepted auto-accept decisions require exact decision/proposal/rollback-plan/registration identities and current evidence while requiring:

```text
ExecutionAuthorized = false
DeploymentAuthorized = false
BusinessAuthorityGranted = false
```

Governed denied auto-accept decisions may legitimately return `NONE` for unavailable candidate/plan/registration identities and remain presentable as denials.

Accepted rollback-order decisions require the canonical separate-execution reason while preserving:

```text
RollbackAuthorized = true
RollbackExecuted = false
AuthorityRestored = false
TrustRestored = false
```

Rollback status is rejected if it silently claims restoration of authority, trust, credentials, Live authority, or Kill/release/revival authority.

## Stable Web request port

Added:

`src/core/ports/owner-update-governance-port.js`

The default port is fail-closed and returns `OWNER_UPDATE_GOVERNANCE_TRANSPORT_UNAVAILABLE` for policy management, proposal evaluation, rollback request, history/status and related operations.

No direct Foundation/Application internal call, endpoint, provider, or Service Bus route is embedded in Web source.

## Owner Command Center surface

Added:

`src/features/owner-approvals/owner-update-governance.js`

and composed it through:

`src/composition/owner-surfaces.js`

The Owner approvals surface now contains:

1. **Standing Approvals** — current governed policies, version, risk ceiling, expiry/evidence, edit/revoke request controls.
2. **Proposal Inbox** — Application/AI proposal facts, class/version/impact, rollback plan, local eligibility result, and manual review.
3. **Auto-Accepted History** — exact decision/policy/rollback plan/evidence and Owner `Request rollback` control.

When governed request transport is unavailable, policy mutation, auto-accept evaluation and rollback-order controls are explicitly disabled with accessible reasons.

A visible or locally eligible proposal never becomes Auto Accepted by the UI.

## FCR-0241 transport gap

During Web binding, source-first review found that FCR-0237 defines exact Foundation service/evaluator contracts but the inspected Foundation source/evidence did not identify an exact Falcon-native FIL request/response route/schema/profile for Web policy management, preapproval evaluation and Owner rollback orders.

Web therefore opened repository Issue #241, canonical:

`FCR-0241 — standing Owner preapproval and rollback command FIL transport contract`

It is `Waiting On: FOUNDATION`.

This gap blocks only live cross-workstream request submission. It does not block Web semantic validation, presentation, history model, fail-closed port, or UI construction.

## Test coverage authored

Added:

- `tests/owner-update-governance-v1.test.mjs`
- `tests/owner-update-proposal-boundary-v1.test.mjs`
- `tests/owner-update-governance-surface.test.mjs`
- `tests/foundation-owner-standing-preapproval-v1.test.mjs`
- `tests/foundation-owner-standing-preapproval-denial.test.mjs`

Coverage includes:

- governed low-impact Maintenance eligibility without approval authority;
- producer self-classification rejection;
- producer self-auto-accept/rollback claims rejection;
- manual-floor classes cannot be weakened;
- execution-changing Maintenance disguise fails closed;
- stale/incompatible/unvalidated rollback plans fail closed;
- AI self-development missing FSA evidence fails closed;
- supersession identity requirement;
- stale proposal fingerprint/policy version invalidates prior Owner disposition;
- rollback request only from OwnerViaSharedWeb and exact plan identity;
- rollback lifecycle state-skipping rejection;
- accepted Foundation decision authority separation;
- governed Foundation denial handling;
- rollback status cannot silently restore separate authority/trust.

`npm run check` includes the new contract, ingress guard, Foundation decision adapter, fail-closed port and Owner approvals feature.

## Source Red Team

Diff review from `4ce37cf56c647673eb64311c02c9df9c44ebc9e5` through the checkpoint candidate found all changes confined to `applications/shared/web/**`.

```text
DIRECT_FOUNDATION_INTERNAL_IMPORT = NONE
DIRECT_APPLICATION_INTERNAL_IMPORT = NONE
DIRECT_NETWORK_TRANSPORT_IN_PRESENTATION = NONE
PRODUCER_SELF_APPROVAL_ACCEPTED = NO
LOCAL_ELIGIBILITY_MINTS_OWNER_AUTHORITY = NO
AUTO_ACCEPT_GRANTS_EXECUTION = NO
AUTO_ACCEPT_GRANTS_DEPLOYMENT = NO
AUTO_ACCEPT_GRANTS_BUSINESS_AUTHORITY = NO
ROLLBACK_REQUEST_IMPLIES_EXECUTION = NO
ROLLBACK_STATUS_SILENT_AUTHORITY_RESTORE_ALLOWED = NO
LIVE_FIL_ROUTE_INVENTED = NO
OPEN_CRITICAL = 0
OPEN_HIGH = 0
OPEN_MEDIUM = 0
OPEN_PRODUCT_LOW = 0
```

## Executable verification truth

The available runner still cannot perform a fresh repository checkout because `github.com` DNS resolution fails. Therefore no false whole-repository PASS is claimed.

```text
FULL_CURRENT_HEAD_NPM_TEST = NOT_RUN
FULL_CURRENT_HEAD_NPM_RUN_CHECK = NOT_RUN
BROWSER_VERIFICATION = NOT_RUN
```

The source and adversarial test set are implemented. Final live FCR-0237 request-transport binding waits only on FCR-0241. Whole-Web executable acceptance remains the later WP-23 gate.
