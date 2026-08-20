# FCR-0241 Owner Governance FIL Web Binding Checkpoint

Date: 2026-08-18
Branch: `web-development`
Foundation exact tested candidate: `b7afc10b69f81c37938457cb3424e49756ab1532`

## Current disposition

Shared Falcon Web has implemented the exact source-side consuming/request construction boundary for the three canonical FCR-0241 Owner-governance FIL request/response families. Governed executable verification of the exact current Web HEAD remains pending because the available runner cannot resolve `github.com` for a fresh checkout.

```text
FCR0241_FOUNDATION = IMPLEMENTED_AND_GOVERNED_VERIFIED
FCR0241_WEB_EXACT_SOURCE_BINDING = IMPLEMENTED
FCR0241_WEB_SOURCE_RED_TEAM = PASS
FCR0241_WEB_EXECUTABLE_VERIFICATION = PENDING_ENVIRONMENT_CAPABILITY
LIVE_SERVICE_BUS_ACTIVATION = NOT_CLAIMED
```

## Exact Foundation contract consumed

Source authority: `src/Foundation.Contracts/PublicRuntimeRequestResponseTransport.cs`.

### Standing Owner policy management

- request route: `route:foundation:owner-policy-management:web:v1`
- response route: `route:foundation:owner-policy-management-result:web:v1`
- request kind: `Command`
- request type: `Foundation.Authority.StandingOwnerPolicyManagementRequest`
- response type: `Foundation.Authority.StandingOwnerPolicyManagementDecision`
- request schema: `foundation.authority.standing-owner-policy-management.request`
- response schema: `foundation.authority.standing-owner-policy-management.decision`
- admission: `admission:foundation:owner-policy-management:web:v1`

### Standing Owner preapproval evaluation

- request route: `route:foundation:owner-preapproval-evaluation:web:v1`
- response route: `route:foundation:owner-preapproval-evaluation-result:web:v1`
- request kind: `Query`
- request type: `Foundation.Authority.WebOwnerPreapprovalProposal`
- response type: `Foundation.Authority.WebOwnerDerivedAutoAcceptDecision`
- request schema: `foundation.authority.web-owner-preapproval.proposal`
- response schema: `foundation.authority.web-owner-preapproval.decision`
- admission: `admission:foundation:owner-preapproval-evaluation:web:v1`

### Owner rollback order

- request route: `route:foundation:owner-rollback-order:web:v1`
- response route: `route:foundation:owner-rollback-order-result:web:v1`
- request kind: `Command`
- request type: `Foundation.Authority.OwnerRollbackOrderRequest`
- response type: `Foundation.Authority.OwnerRollbackOrderDecision`
- request schema: `foundation.authority.owner-rollback-order.request`
- response schema: `foundation.authority.owner-rollback-order.decision`
- admission: `admission:foundation:owner-rollback-order:web:v1`

All families use Foundation-governed version `1.0.0`, classification `Governance`, request producer `shared-web`, request recipient `foundation.owner-governance`, response producer `foundation.runtime`, response recipient `shared-web`, request transport authority `authority:transport:owner-command-center-request`, response transport authority `authority:transport:owner-governance-response`, request/response TTL ceiling 120 seconds, maximum delivery attempts 3, and same-idempotency retry policy.

## Web implementation

- `src/adapters/foundation-owner-governance-fil-v1.js`
  - freezes the exact canonical profile registry in Web;
  - reconstructs Foundation profile identities with the canonical UTF-8 byte-length hashing algorithm;
  - builds Web request envelopes without self-declaring Foundation acceptance or route availability;
  - validates exact Foundation response transport decisions and FIL envelopes;
  - binds response correlation to the request correlation and response causation to the exact request message identity;
  - validates UTC freshness/TTL, payload digest, profile provenance, producer/recipient, message kind, classification and transport authority;
  - rejects non-canonical caller-created profiles and cross-family substitution.

- `src/adapters/foundation-owner-governance-port-v1.js`
  - applies the existing Foundation semantic decision adapters after FIL transport validation;
  - transport acceptance alone therefore cannot bypass Auto-Accept, policy-management or rollback semantic authority guards.

- `src/core/ports/owner-update-governance-port.js`
  - composes only the three FCR-0241 write/query operations behind the stable Web-owned governance port;
  - standing policy read, proposal inbox, history and rollback execution/status remain with their separately governed authoritative sources.

## Source Red Team

A source review identified and remediated one authority-labeling issue in the first Web request-builder revision: the locally constructed request record used `Accepted=true` / `RouteAvailable=true`. Those fields could imply Foundation acceptance before Foundation had evaluated the request. The revision was corrected so Web reports only `built=true`; only the Foundation response transport decision may carry `Accepted` and `RouteAvailable`.

Adversarial source/test coverage now includes:

- wrong/caller-created profile;
- cross-family route/profile substitution;
- Command/Query/Response kind substitution;
- classification, producer, recipient and authority mismatch;
- response correlation mismatch;
- response causation mismatch;
- payload mutation/digest mismatch;
- stale or non-UTC request/response;
- TTL greater than 120 seconds;
- route activation/authorization/connection/execution/business-authority escalation;
- malformed outcome;
- transport-valid but semantically authority-leaking Foundation decision payload.

```text
OPEN_CRITICAL = 0
OPEN_HIGH = 0
OPEN_MEDIUM = 0
OPEN_PRODUCT_LOW = 0
```

## Preserved boundaries

```text
WEB_REQUEST_BUILT != FOUNDATION_REQUEST_ACCEPTED
FIL_ROUTE_AVAILABLE != ROUTE_ACTIVATED
ROUTE_AVAILABLE != ROUTE_AUTHORIZED != CONNECTION_EXECUTED
PUBLIC_RUNTIME_PROJECTION_TRANSPORT != OWNER_CONTROL_REQUEST_TRANSPORT
APPLICATION_AI_PROPOSAL != OWNER_DECISION
APPLICATION_AI_SELF_APPROVAL = FORBIDDEN
OWNER_SILENCE != OWNER_APPROVAL
AUTO_ACCEPT != EXECUTION_AUTHORITY
AUTO_ACCEPT != DEPLOYMENT_AUTHORITY
AUTO_ACCEPT != BUSINESS_AUTHORITY
ROLLBACK_REQUEST != ROLLBACK_AUTHORIZATION != ROLLBACK_EXECUTION
ROLLBACK_COMPLETED != AUTHORITY_RESTORED
PLUG_AND_PLAY != IMPLICIT_TRUST
```

## Verification limitation

A fresh command-line checkout attempt for `web-development` still fails with `Could not resolve host: github.com`. Therefore no full-current-HEAD `npm test` or `npm run check` PASS is claimed in this checkpoint. The source is ready for executable verification as soon as a governed checkout-capable runner is available.
