# FCR-0241 / FCR-0237 / FCR-0238 Renewed Foundation Binding Checkpoint

Date: 2026-08-18
Workstream: Shared Falcon Web Application
Branch: `web-development`
Foundation exact official executable commit: `9d7f699dc5545c51a3415be2cddca8a757ac7738`
Application exact semantic contract for FCR-0238: `1b593a7acb2be01dd2ad6cd124ba6c1df3272ebe`
Web full current-HEAD verified candidate: `780b85eed754e07df94628aff6e7fda0c17e4869`

## Purpose

Renew and reverify Shared Web consuming bindings after Foundation completed the reopened FCR-0241 validation and handed FCR-0241, FCR-0237 and FCR-0238 back to `Waiting On: WEB`.

This checkpoint does not activate a Service Bus route, deployment, execution, rollback execution, Trading authority or business authority.

## Foundation evidence consumed

The exact Foundation candidate reports:

- controlled restore PASS;
- Release build PASS with zero warnings and zero errors;
- architecture PASS;
- baseline security PASS with zero findings;
- repository security surface PASS with zero findings;
- all 86 governed verifiers PASS;
- Stage0C remediation evidence and trace PASS;
- FCR-0241 rerun 1 PASS 62/62;
- FCR-0241 rerun 2 PASS 62/62;
- deterministic SHA-256 identical across both reruns: `74D77692B38D04F0555FAC13C03908AFC8ED63A86D5ABE23895A876A2DE141ED`;
- final working tree clean and final validation marker PASS;
- fresh Foundation Red Team open Critical/High/Medium/Low = `0/0/0/0`.

The final official Foundation commit changes only validation-runner output capture. Product contracts and verifier logic are unchanged by that final one-line runner correction.

## Exact canonical transport reconciliation

Shared Web re-read the exact Foundation `OwnerGovernanceRequestResponseProfiles` and `PublicRuntimeRequestResponseTransport` implementation at `9d7f699...`.

The Web profiles match the exact Foundation definitions for all three families:

1. standing Owner policy management;
2. standing Owner preapproval evaluation;
3. Owner rollback order.

Exact reconciled properties include:

- contract/schema version `1.0.0`;
- Governance classification;
- Published contract state;
- producer/recipient identities;
- request/response transport authorities;
- exact request/response routes;
- exact request/response message types;
- exact request/response schema identities;
- request TTL 120 seconds;
- response TTL 120 seconds;
- maximum delivery attempts 3;
- retry requires stable idempotency identity;
- deterministic profile identity algorithm and evidence identities.

Observed deterministic Web profile identities:

```text
standing-policy-management = sha256/D1CE72B11CA89E253B284372C2A354C6B31FC0B45D7A2EEBE6AF3F33EBCBB1B4
standing-preapproval-evaluation = sha256/1D2D8DB267611AE266EF0200BAAD05C8B2C5B98F1F5D8BB74FD67607D63EADBB
rollback-order = sha256/84446D23B9B3BC28EBACC4753A0EE9BE57276DCA34A38C4652D9D72D55B2B20C
```

## Renewed Web finding 1: request freshness at response observation time

The prior Web transport consumer validated response freshness using an observation time captured before the asynchronous `exchange()` call. A request that expired while waiting for a response could therefore be evaluated against a stale pre-exchange clock reading.

Remediation in `src/adapters/foundation-owner-governance-fil-v1.js`:

- validates request freshness against an explicit observation time when building the request;
- re-reads the clock after `exchange()` returns;
- validates the response against that post-exchange observation time;
- revalidates the original request against the same observation time;
- rejects a response when the originating request is no longer current.

Fail-closed result:

```text
OWNER_GOVERNANCE_REQUEST_NO_LONGER_CURRENT
```

## Renewed Web finding 2: exact semantic identity narrowing

The prior semantic consumer was broader than the exact Foundation output in three places:

- successful standing-policy management accepted a non-canonical registration identity string;
- accepted Auto-Accept accepted any non-`NONE` underlying authority identifier;
- semantic decision timestamps accepted arbitrary parseable timezone offsets instead of the exact UTC-producing Foundation boundary.

Remediation in `src/adapters/foundation-owner-standing-preapproval-v1.js` now requires:

- applied policy decisions: exact `sha256/<64 hex>` registration identity;
- denied policy decisions: `RegistrationIdentitySha256 = NONE`;
- accepted Auto-Accept: exact `authority-decision/sha256/<64 hex>` underlying authority identity;
- Auto-Accept, policy, rollback decision and rollback-status timestamps: canonical UTC (`Z` or `+00:00`).

Authority separation remains unchanged and fail closed.

## Initial targeted executable evidence

Before a full checkout-backed environment was available, exact affected source was executed in isolated Node harnesses:

```text
SEMANTIC_TESTS = PASS 10/10
FCR0241_FIL_TARGETED = PASS
FCR0241_0237_0238_INTEGRATED_TARGETED = PASS
POLICY_MANAGEMENT = PASS
AUTO_ACCEPT_NON_AUTHORITY = PASS
ROLLBACK_SEPARATION = PASS
```

## Full current-HEAD executable verification

A clean checkout-backed verification was then executed on the Project Owner machine against exact Web HEAD:

```text
HEAD = 780b85eed754e07df94628aff6e7fda0c17e4869
BRANCH = web-development
REPOSITORY = raed82iam/Falcon
WORKING_TREE_BEFORE = CLEAN
NODE = v24.19.0
NPM = 11.17.0
```

The first full run exposed six regressions. They were reviewed individually rather than suppressed:

- two real Web regressions were remediated: missing document skip-link and missing localized skip-link synchronization;
- four tests were stale relative to already-governed current behavior and were narrowed to the current contracts: incident composition ownership, canonical Owner Home destination, entitlement-gated My Applications access, and post-exchange FCR-0241 observation timing.

The corrected exact candidate was then re-run from a clean checkout.

Final executable evidence:

```text
NPM_TEST = PASS
TESTS = 435
PASS = 435
FAIL = 0
CANCELLED = 0
SKIPPED = 0
TODO = 0
NPM_RUN_CHECK = PASS
WORKING_TREE_AFTER = CLEAN
FULL_CURRENT_HEAD_NODE_VERIFICATION = PASS
```

This full suite includes FCR-0241 transport tests, FCR-0237 standing Owner preapproval semantics, FCR-0238 combined policy/rollback semantics, Stage 16 identity/session tests, FCR-0242 entitlement tests, provider-binding fail-closed tests, incident persistence/support/voice tests, Owner Command Center truthfulness, MSA/LSA boundaries, request routing, accessibility, security escaping, portfolio/activity/analysis contracts and public/workspace composition.

Browser RTL/LTR/mobile/keyboard/runtime interaction verification remains a separate WP-23 sub-gate and is not converted into route activation or deployment authority.

## Renewed Source Red Team

The rebind and remediation remain confined to `applications/shared/web/**`.

```text
FOUNDATION_FILES_CHANGED = 0
APPLICATION_FILES_CHANGED = 0
LIVE_ROUTE_ACTIVATION_ADDED = NO
NETWORK_EXECUTION_ADDED = NO
BUSINESS_AUTHORITY_ADDED = NO
OPEN_CRITICAL = 0
OPEN_HIGH = 0
OPEN_MEDIUM = 0
OPEN_PRODUCT_LOW = 0
```

## Mandatory separation preserved

```text
WEB_OWNER_COMMAND_CENTER = ONLY_OWNER_DERIVED_DECISION_SURFACE
APPLICATION_AI_PROPOSAL != OWNER_DECISION
APPLICATION_AI_SELF_APPROVAL = FORBIDDEN
OWNER_SILENCE != OWNER_APPROVAL
AUTO_ACCEPT != EXECUTION_AUTHORITY
AUTO_ACCEPT != DEPLOYMENT_AUTHORITY
AUTO_ACCEPT != BUSINESS_AUTHORITY
ROLLBACK_REQUEST != ROLLBACK_AUTHORIZATION != ROLLBACK_EXECUTION
ROLLBACK_COMPLETED != AUTHORITY_RESTORED
FIL_ROUTE_AVAILABLE != ROUTE_ACTIVATED
ROUTE_AVAILABLE != ROUTE_AUTHORIZED != CONNECTION_EXECUTED
PLUG_AND_PLAY != IMPLICIT_TRUST
PUBLIC_RUNTIME_PROJECTION_TRANSPORT != OWNER_CONTROL_REQUEST_TRANSPORT
```

## Current disposition

```text
FCR0241_FOUNDATION = IMPLEMENTED_AND_GOVERNED_VERIFIED
FCR0241_WEB_RENEWED_BINDING = IMPLEMENTED
FCR0241_WEB_TARGETED_EXECUTABLE = PASS
FCR0237_WEB_RENEWED_SEMANTIC_CONSUMER = PASS
FCR0238_APPLICATION_SEMANTIC_CONTRACT = PRESERVED
FCR0238_WEB_RENEWED_COMBINED_CONSUMER = PASS
WEB_RENEWED_SOURCE_RED_TEAM = PASS
FULL_CURRENT_HEAD_WEB_NODE_SUITE = PASS_435_OF_435
NPM_RUN_CHECK = PASS
WORKING_TREE = CLEAN
BROWSER_RUNTIME_VERIFICATION = PENDING_WP23_SUBGATE
LIVE_SERVICE_BUS_ACTIVATION = NOT_CLAIMED
DEPLOYMENT_AUTHORITY = NOT_CREATED
EXECUTION_AUTHORITY = NOT_CREATED
BUSINESS_AUTHORITY = NOT_CREATED
```

FCR-specific renewed verification is complete and now includes full current-HEAD Node regression evidence. The three FCRs remain open only for explicit Project Owner closure decision. Browser/runtime verification continues under the repository-wide WP-23 gate and is not an FCR-specific authority prerequisite.