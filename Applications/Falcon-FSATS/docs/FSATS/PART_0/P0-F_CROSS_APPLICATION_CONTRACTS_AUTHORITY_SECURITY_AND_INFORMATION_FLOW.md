# P0-F - Cross-Application Contracts, Authority, Security and Information Flow

**Status:** `OWNER_DIRECTED_INTEGRATED_REWRITE_CANDIDATE / NOT_OWNER_ACCEPTED / NOT_CLOSED`
**Implementation Authority:** `NOT_GRANTED`
**Runtime Route Authority:** `NOT_GRANTED`

## 1. Purpose

P0-F defines every material cross-Application interaction as an explicit, attributable, governable contract edge rather than hidden coupling. It directly preserves the predecessor exact 43-family Application-to-Application baseline and integrates the later accepted P1K contract extensions for APP-RSC, Foundation-facing queries/outcomes, AI recovery/evidence and current Shared Web semantics.

After final Owner acceptance, a programmer must not need the archive to reconstruct the current cross-Application architecture.

## 2. Responsibility

P0-F owns the Application-side design for:

- exact business contract-family identity;
- exact producer and consumer;
- purpose/business meaning;
- bilateral declaration;
- authority class;
- security/trust class;
- schema/version compatibility;
- information-flow semantics;
- truth/environment classification;
- freshness/deadline/ordering;
- correlation/causation;
- idempotency/duplicate/replay/correction;
- failure/degraded behavior;
- current Web/Owner/Guardian/Risk/resource/recovery boundaries;
- Foundation/FCR dependency linkage;
- contract completeness and negative fixtures.

P0-F does not implement Foundation FIL, Service Bus, Event System, cryptography, lifecycle, resource governance, external egress, credentials, provider or broker connectivity.

## 3. Prime contract rules

```text
EVERY_CROSS_APPLICATION_INTERACTION = EXPLICIT_CONTRACT_EDGE
EXACT_PRODUCER = REQUIRED
EXACT_CONSUMER = REQUIRED
CONTAINER_OR_WILDCARD_PARTICIPANT = PROHIBITED
BILATERAL_DECLARATION = REQUIRED
CONTRACT_EDGE_EXISTS != FOUNDATION_ROUTE_AUTHORIZED
FOUNDATION_ROUTE_AUTHORIZED != BUSINESS_ACTION_AUTHORIZED
DELIVERY_ACK != BUSINESS_OUTCOME
REPLAY_TEST_TRUTH != OPERATIONAL_AUTHORITY
```

FSATS is a system boundary, never a producer/consumer/principal/authority source.

## 4. Contract identity namespace

The predecessor P0 business family form remains valid lineage:

`falcon.xapp.<producer-domain>.<producer>.<consumer-domain>.<consumer>.<purpose>`

Business family identity is distinct from Foundation `CON-*`, Foundation route ID, endpoint ID, schema/message ID, Application package ID and authority/delegation identity.

```text
BUSINESS_CONTRACT_ID != FOUNDATION_ROUTE_ID
BUSINESS_CONTRACT_ID != ENDPOINT_ID
BUSINESS_CONTRACT_ID != SCHEMA_ID
```

The later `P1K-*` catalog identities are current implementation/catalog identities and may map/extend predecessor business families. They do not erase predecessor obligations.

# 5. Exact predecessor 43-family baseline

This exact set remains a required semantic migration baseline.

## 5.1 Trading and FSAPMA - 3

| # | Contract Family ID | Producer | Consumer | Class | Purpose |
|---:|---|---|---|---|---|
| 1 | `falcon.xapp.trading.core.trading.fsapma.data-requirement` | Trading | FSAPMA | REQUEST | provider-independent operational data requirement |
| 2 | `falcon.xapp.trading.fsapma.trading.core.operational-data-product` | FSAPMA | Trading | DATA_PRODUCT | normalized operational trading data |
| 3 | `falcon.xapp.trading.fsapma.trading.core.provider-service-status` | FSAPMA | Trading | PROJECTION/EVENT | provider/data-service status |

## 5.2 Guardian and Trading - 4

| # | Contract Family ID | Producer | Consumer | Class | Purpose |
|---:|---|---|---|---|---|
| 4 | `falcon.xapp.trading.guardian.trading.core.protection-command` | Guardian | Trading | COMMAND | bounded protection/restriction command |
| 5 | `falcon.xapp.trading.core.trading.guardian.safety-projection` | Trading | Guardian | PROJECTION/EVENT | bounded safety/exposure evidence |
| 6 | `falcon.xapp.trading.core.trading.guardian.protection-command-outcome` | Trading | Guardian | OUTCOME | exact command business outcome |
| 7 | `falcon.xapp.trading.guardian.trading.core.protection-release` | Guardian | Trading | COMMAND | scoped recovery/release direction |

## 5.3 Guardian and FSAPMA - 3

| # | Contract Family ID | Producer | Consumer | Class | Purpose |
|---:|---|---|---|---|---|
| 8 | `falcon.xapp.trading.guardian.trading.fsapma.provider-protection-command` | Guardian | FSAPMA | COMMAND | bounded provider-use protection constraint |
| 9 | `falcon.xapp.trading.fsapma.trading.guardian.provider-integrity-projection` | FSAPMA | Guardian | PROJECTION/EVENT | provider/data integrity evidence |
| 10 | `falcon.xapp.trading.fsapma.trading.guardian.provider-protection-outcome` | FSAPMA | Guardian | OUTCOME | protection application/rejection outcome |

## 5.4 FSTSimA sibling flows - 7

| # | Contract Family ID | Producer | Consumer | Class | Purpose |
|---:|---|---|---|---|---|
| 11 | `falcon.xapp.trading.core.validation.fstsima.validation-input` | Trading | FSTSimA | REQUEST/EVIDENCE_PACKAGE | Trading candidate validation input |
| 12 | `falcon.xapp.trading.guardian.validation.fstsima.validation-input` | Guardian | FSTSimA | REQUEST/EVIDENCE_PACKAGE | Guardian candidate validation input |
| 13 | `falcon.xapp.trading.fsapma.validation.fstsima.validation-input` | FSAPMA | FSTSimA | REQUEST/EVIDENCE_PACKAGE | FSAPMA candidate validation input |
| 14 | `falcon.xapp.trading.fsapma.validation.fstsima.nonlive-data-input` | FSAPMA | FSTSimA | DATA_PRODUCT/EVIDENCE_PACKAGE | explicit non-Live replay/test/calibration input |
| 15 | `falcon.xapp.validation.fstsima.trading.core.validation-evidence` | FSTSimA | Trading | EVIDENCE_PACKAGE | reproducible Trading validation evidence |
| 16 | `falcon.xapp.validation.fstsima.trading.guardian.validation-evidence` | FSTSimA | Guardian | EVIDENCE_PACKAGE | reproducible Guardian validation evidence |
| 17 | `falcon.xapp.validation.fstsima.trading.fsapma.validation-evidence` | FSTSimA | FSAPMA | EVIDENCE_PACKAGE | reproducible FSAPMA validation evidence |

## 5.5 Presentation projections to Shared Web - 4

| # | Contract Family ID | Producer | Consumer | Class |
|---:|---|---|---|---|
| 18 | `falcon.xapp.trading.guardian.shared.web.presentation-projection` | Guardian | Web | PROJECTION/EVENT |
| 19 | `falcon.xapp.trading.fsapma.shared.web.presentation-projection` | FSAPMA | Web | PROJECTION/EVENT |
| 20 | `falcon.xapp.trading.core.shared.web.presentation-projection` | Trading | Web | PROJECTION/EVENT |
| 21 | `falcon.xapp.validation.fstsima.shared.web.presentation-projection` | FSTSimA | Web | PROJECTION/EVENT |

## 5.6 Shared Web intent paths - 4

| # | Contract Family ID | Producer | Consumer | Class |
|---:|---|---|---|---|
| 22 | `falcon.xapp.shared.web.trading.guardian.user-intent` | Web | Guardian | USER_INTENT |
| 23 | `falcon.xapp.shared.web.trading.fsapma.user-intent` | Web | FSAPMA | USER_INTENT |
| 24 | `falcon.xapp.shared.web.trading.core.user-intent` | Web | Trading | USER_INTENT |
| 25 | `falcon.xapp.shared.web.validation.fstsima.user-intent` | Web | FSTSimA | USER_INTENT |

`USER_INTENT` is a historical family-class name. Under the current identity correction, Shared Web owns user/customer identity and resolves it to exact governed broker-account scope where Trading business identity is required. The FSATS target does not acquire a customer/user principal.

## 5.7 Outcomes back to Shared Web - 4

| # | Contract Family ID | Producer | Consumer | Class |
|---:|---|---|---|---|
| 26 | `falcon.xapp.trading.guardian.shared.web.user-intent-outcome` | Guardian | Web | OUTCOME |
| 27 | `falcon.xapp.trading.fsapma.shared.web.user-intent-outcome` | FSAPMA | Web | OUTCOME |
| 28 | `falcon.xapp.trading.core.shared.web.user-intent-outcome` | Trading | Web | OUTCOME |
| 29 | `falcon.xapp.validation.fstsima.shared.web.user-intent-outcome` | FSTSimA | Web | OUTCOME |

Every outcome binds exact initiating request/intent and distinguishes technical receipt from business acceptance/completion.

## 5.8 Notification/report requests to Shared Communication - 4

| # | Contract Family ID | Producer | Consumer | Class |
|---:|---|---|---|---|
| 30 | `falcon.xapp.trading.guardian.shared.communication.notification-request` | Guardian | Communication | NOTIFICATION_REQUEST |
| 31 | `falcon.xapp.trading.fsapma.shared.communication.notification-request` | FSAPMA | Communication | NOTIFICATION_REQUEST |
| 32 | `falcon.xapp.trading.core.shared.communication.notification-request` | Trading | Communication | NOTIFICATION_REQUEST |
| 33 | `falcon.xapp.validation.fstsima.shared.communication.notification-request` | FSTSimA | Communication | NOTIFICATION_REQUEST |

## 5.9 Communication delivery outcomes - 4

| # | Contract Family ID | Producer | Consumer | Class |
|---:|---|---|---|---|
| 34 | `falcon.xapp.shared.communication.trading.guardian.delivery-outcome` | Communication | Guardian | OUTCOME |
| 35 | `falcon.xapp.shared.communication.trading.fsapma.delivery-outcome` | Communication | FSAPMA | OUTCOME |
| 36 | `falcon.xapp.shared.communication.trading.core.delivery-outcome` | Communication | Trading | OUTCOME |
| 37 | `falcon.xapp.shared.communication.validation.fstsima.delivery-outcome` | Communication | FSTSimA | OUTCOME |

## 5.10 Communication recipient responses - 4

| # | Contract Family ID | Producer | Consumer | Class |
|---:|---|---|---|---|
| 38 | `falcon.xapp.shared.communication.trading.guardian.recipient-response` | Communication | Guardian | OUTCOME/EVENT |
| 39 | `falcon.xapp.shared.communication.trading.fsapma.recipient-response` | Communication | FSAPMA | OUTCOME/EVENT |
| 40 | `falcon.xapp.shared.communication.trading.core.recipient-response` | Communication | Trading | OUTCOME/EVENT |
| 41 | `falcon.xapp.shared.communication.validation.fstsima.recipient-response` | Communication | FSTSimA | OUTCOME/EVENT |

Source Application decides any resulting business action under its own authority.

## 5.11 Shared Web and Shared Communication - 2

| # | Contract Family ID | Producer | Consumer | Class | Purpose |
|---:|---|---|---|---|---|
| 42 | `falcon.xapp.shared.web.shared.communication.recipient-response-intent` | Web | Communication | USER_INTENT | response intent for exact Communication-owned workflow |
| 43 | `falcon.xapp.shared.communication.shared.web.communication-status-projection` | Communication | Web | PROJECTION/EVENT | least-privilege delivery/response status projection |

## 5.12 Count proof

```text
TRADING_FSAPMA = 3
GUARDIAN_TRADING = 4
GUARDIAN_FSAPMA = 3
FSTSIMA_SIBLING = 7
PRESENTATION_TO_WEB = 4
WEB_INTENTS = 4
OUTCOMES_TO_WEB = 4
NOTIFICATION_REQUESTS = 4
DELIVERY_OUTCOMES = 4
RECIPIENT_RESPONSES = 4
WEB_COMMUNICATION = 2
TOTAL = 43
```

```text
PREDECESSOR_43_FAMILIES_PRESERVED = 43/43
UNEXPLAINED_DROPS = 0
UNEXPLAINED_MERGES = 0
```

The 43 are a predecessor minimum baseline, not the current maximum.

# 6. Current P1K contract catalog extensions

Later accepted implementation design adds/normalizes current `P1K` families. The current catalog is `1.0.0-part2` and `runtimeRoutesActive = false`.

Current catalog families include:

| ID | Current semantic purpose |
|---|---|
| `P1K-001` | FSAPMA Operational Data Delivery |
| `P1K-002` | FSAPMA Data Quality Correction Event |
| `P1K-003` | Trading Decision Intent Evidence Projection |
| `P1K-004` | Trading Exposure/Order/Position Safety Projection |
| `P1K-005` | Guardian Protection Restriction Command |
| `P1K-006` | Protection Command Outcome |
| `P1K-007` | Application Safety Incident Evidence |
| `P1K-008` | Constituent Resource Evidence Submission -> APP-RSC |
| `P1K-009` | APP-RSC Effective Coordination Outcome |
| `P1K-010` | Resource Coordination Acknowledgement Result |
| `P1K-011` | APP-RSC Residual Resource Request -> Foundation resource boundary |
| `P1K-012` | Foundation Resource Authority Outcome -> APP-RSC |
| `P1K-013` | APP-RSC Coordination Epoch Fencing Projection |
| `P1K-014` | FSTSimA Scenario/Simulation Input Request |
| `P1K-015` | FSTSimA Validation/Qualification Evidence |
| `P1K-016` | AI/Awareness Integrity Incident Projection |
| `P1K-017` | AI Containment/Kill State Projection |
| `P1K-018` | Controlled Revival Evidence/Decision Request |
| `P1K-019` | Shared Web Informational Query/Response |
| `P1K-020` | Shared Web Owner Command Request/Application Outcome |
| `P1K-021` | FSATS Application -> Foundation Information/Evidence/Capability Query |
| `P1K-022` | Foundation -> FSATS Application Authoritative Event/Decision/Query |

These families are current contract-catalog semantics and extend/consolidate predecessor obligations. They do not imply active runtime routes.

```text
P1K_RUNTIME_ROUTES_ACTIVE = FALSE
ROUTE_EXISTS != AUTHORITY
```

# 7. Bilateral declaration

Every material family is declared compatibly by both participants before runtime integration is admitted.

Producer/requester declaration includes family ID, exact intended counterparty, capability/purpose, authority/permission requirement, schema/version rule, environment/truth classification and failure/degraded behavior.

Consumer/responder declaration includes the same family ID, exact permitted producer/requester, compatible capability, authority/permission rule, compatible schema/version, environment/truth classification and failure/degraded behavior.

One-sided declaration, participant mismatch, incompatible version, capability mismatch or authority mismatch fails closed.

```text
PRODUCER_DECLARED != RELATIONSHIP_ADMITTED
CONSUMER_DECLARED != RELATIONSHIP_ADMITTED
```

# 8. Application authority classes

Each family binds an effective authority class.

### INFORMATION_REQUEST
Requests owner behavior. Requester does not acquire responder ownership.

### OWNER_TRUTH_PUBLICATION
Publishes bounded owner-controlled truth via Data Product/projection/event. Consumer use does not transfer ownership.

### DELEGATED_PROTECTION_COMMAND
Exact Guardian protection/release authority only, bounded to active attributable scope.

### NONAUTHORITATIVE_VALIDATION_EXCHANGE
FSTSimA inputs/evidence/non-Live data. Never creates Live authority or automatic promotion.

### WEB_OR_OWNER_INTENT_FORWARDING
Carries authenticated/attributed Web-side customer/Owner intent. UI interaction does not equal target business authorization and customer identity remains Web-owned.

### SHARED_SERVICE_REQUEST
Requests Communication-owned behavior without transferring source business meaning.

### BUSINESS_OUTCOME_RETURN
Returns bounded result tied to initiating request/command/intent with no unrelated follow-on authority.

### RESOURCE_EVIDENCE_OR_REQUEST
Carries constituent evidence to APP-RSC or proven APP-RSC residual need to Foundation. It never equals a Foundation grant.

### FOUNDATION_AUTHORITY_OUTCOME
Carries Foundation-owned resource/capability/lifecycle/evidence outcome to an exact Application. Application consumption does not transfer Foundation authority.

### INTEGRITY_RECOVERY_EVIDENCE
Carries AI/Awareness integrity/containment/revival evidence under P0-C/Foundation governance and creates no self-release authority.

# 9. Security semantic classes

## CONTROL_CRITICAL
Guardian/recovery/control commands and outcomes require exact identity, strong integrity/authenticity via accepted Foundation mechanisms, authority binding, anti-replay/idempotency, expiry/freshness, least privilege, evidence and fail-closed downgrade.

## OPERATIONAL_TRADING_SENSITIVE
Market-data requirements/products, provider status/integrity, Trading safety/exposure/order/portfolio projections require integrity, provenance, freshness, correction lineage, confidentiality where needed, exact counterparties and no secret leakage.

## NONLIVE_VALIDATION_SENSITIVE
FSTSimA exchanges require explicit non-Live classification, artifact/provenance identity, isolation from Live credentials/routes and zero production-mutation authority.

## WEB_INTERACTION_SENSITIVE
Web projections/intents/outcomes require authenticated customer/Owner context, privacy/tenant isolation, broker-account scope resolution where applicable, anti-replay/idempotency for material commands, least privilege and no secrets in UI payloads.

## COMMUNICATION_SENSITIVE
Notification/outcome/response flows require recipient/tenant/Application attribution, confidentiality preservation, dedupe/expiry, truthful channel outcome and no cross-consumer leakage.

## RESOURCE_AUTHORITY_SENSITIVE
APP-RSC/Foundation resource flows require exact constituent/Application identity, current Foundation envelope/outcome identity, coordination epoch/fencing, anti-replay, evidence freshness and strict request-versus-grant separation.

Where classes overlap, requirements are cumulative and stronger applicable requirement controls.

# 10. Schema/version/lifecycle compatibility

Before an updated Application package is admitted:

- both sides' family identities still match;
- supported schema/message versions intersect compatibly;
- authority/security classes remain compatible;
- mandatory fields do not disappear silently;
- semantic reinterpretation uses governed versioning;
- rollback restores compatible contract set or remains isolated;
- update/removal cannot silently break a mandatory dependent.

Authority-bearing/safety-relevant semantics never use unresolved `latest`.

# 11. Time, freshness, ordering, correlation and causation

Where material distinguish source/event, publication, receive, decision/effective, expiry/validity, dispatch, outcome and correction/supersession time.

A transport hop must not silently reset an end-to-end deadline.

Correlation groups workflow items; causation identifies immediate cause. Multi-hop workflows preserve both.

# 12. Idempotency, duplicate, replay and correction

Every material family states whether it is idempotent, deduplicated by identity, safely retryable, replayable only as non-authoritative, non-replayable or correctable/supersedable.

```text
DUPLICATE_DELIVERY != DUPLICATE_BUSINESS_ACTION
REPLAY_TEST_MESSAGE != OPERATIONAL_COMMAND
VALID_SIGNATURE != LIVE_ACTION_AUTHORITY
```

Transport retry never blindly repeats an externally ambiguous non-idempotent action.

# 13. Canonical operational-data flow

```text
Trading data requirement
-> Trading -> FSAPMA contract
-> Foundation-governed admitted transport
-> FSAPMA validation/provider realization
-> normalized governed Data Product
-> FSAPMA -> Trading contract
-> Trading freshness/quality/provenance validation
-> Trading-owned analysis/Risk/decision
```

Trading never bypasses FSAPMA. Foundation delivery does not prove data sufficiency for a trade.

# 14. Canonical Guardian protection flow

```text
Domain-owned evidence
-> Guardian
-> Guardian protection assessment
-> exact scoped protection command
-> governed transport
-> target validates authority/scope/currentness
-> target applies only owned behavior under restriction
-> exact business outcome
-> Guardian reconciliation
-> later release only as new governed command
```

Guardian never becomes Trading Risk, FSAPMA provider, APP-RSC or Foundation owner. Transport ACK is not protection effect.

# 15. Canonical FSTSimA validation flow

```text
Owning Application candidate
-> immutable exact validation input
-> FSTSimA non-Live validation
-> immutable validation evidence
-> owning Application evaluation
-> origin-correct awareness/FSA/Owner path where applicable
```

FSTSimA cannot promote/adopt the candidate or inherit Live authority.

# 16. Current APP-RSC resource flow

The predecessor TARC direct Trading/Foundation path is superseded.

```text
TRADING / FSAPMA / GUARDIAN / FSTSIMA
-> separately attributable resource evidence
-> APP-RSC P1K-008
-> APP-RSC current-epoch validation
-> bounded internal FSATS coordination
-> P1K-009 effective outcome
-> P1K-010 acknowledgement/reconciliation

IF PROVEN RESIDUAL NEED REMAINS:
APP-RSC
-> P1K-011 residual request
-> FOUNDATION RESOURCE AUTHORITY
-> P1K-012 authoritative outcome
-> APP-RSC reconciliation
-> P1K-013 epoch/fencing state as applicable
```

```text
TRADING_DIRECT_FOUNDATION_RESOURCE_REQUEST = PROHIBITED
GUARDIAN_DIRECT_FOUNDATION_RESOURCE_REQUEST = PROHIBITED
FSA_RESOURCE_REQUEST_ENDPOINT = PROHIBITED
APP_RSC_REQUEST != FOUNDATION_GRANT
REQUESTED_RESOURCE != PROVEN_RESIDUAL_NEED != GRANTED_RESOURCE
```

Final canonical runtime binding remains pending FCR-0016/FCR-0031.

# 17. Current Shared Web read path

```text
WEB AUTHENTICATED CUSTOMER CONTEXT
-> WEB RESOLVES CUSTOMER -> EXACT BROKER-ACCOUNT SCOPE SET
-> Web sends exact governed broker-account scope required for query
-> target Application validates scope
-> target returns bounded owner-truth projection
-> Web renders returned truth
```

```text
FSATS_USER_ID = NONE
FSATS_CUSTOMER_ID = NONE
WEB_OWNS_CUSTOMER_TO_BROKER_ACCOUNT_MAPPING = YES
WEB_DISPLAY != BUSINESS_TRUTH_OWNER
```

For current portfolio/activity/performance semantics, Trading owns the truth and FCR-0133 tracks implementation-ready binding metadata.

# 18. Current Shared Web command/write path

```text
Web interaction
-> authenticated customer/Owner context
-> Web resolves exact target and broker-account scope where applicable
-> consequence-appropriate confirmation/evidence
-> exact request/intent to exact Application
-> target authority/business validation
-> bounded action/rejection
-> exact business outcome
-> Web renders outcome
```

```text
UI_CLICK != BUSINESS_AUTHORIZATION
REQUESTED != ACCEPTED
ACCEPTED != COMPLETED
```

Web never becomes Trading/Guardian/FSAPMA/FSTSimA/APP-RSC authority.

# 19. Portfolio/positions/activity/performance projection semantics

Trading-owned semantic identities currently include:

```text
FSATS.WebPortfolioViewRequest.v1
FSATS.WebPortfolioSummaryProjection.v1
FSATS.WebPositionCollectionProjection.v1
FSATS.WebOrderTradeActivityProjection.v1
FSATS.WebPortfolioPerformanceProjection.v1
FSATS.WebPortfolioProjectionUpdate.v1
```

Required envelope semantics include as applicable RequestId/CorrelationId, exact broker-account scope, ProjectionId/Version, AsOfTime, truth/freshness state, currency/unit, evidence reference, reason/limitations and correction/supersession identity.

Mandatory distinctions:

```text
WEB_DISPLAY != PORTFOLIO_TRUTH_OWNER
WEB_DISPLAY != EXECUTION_TRUTH_OWNER
WEB_DISPLAY != BROKER_TRUTH_OWNER
WEB_DISPLAY != PERFORMANCE_CALCULATION_AUTHORITY
ORDER_REQUESTED != ORDER_ACCEPTED
ORDER_ACCEPTED != PARTIALLY_FILLED
PARTIALLY_FILLED != FILLED
CANCEL_REQUESTED != CANCELED
REPLACEMENT_REQUESTED != REPLACED
UNKNOWN_BROKER_OUTCOME != REJECTED
NO_SOURCE_VALUE != ZERO
LAST_KNOWN != CURRENT
PARTIAL != COMPLETE
STALE != CURRENT
PROJECTION != EXECUTION_AUTHORITY
```

# 20. Stop-new-exposure semantics

A customer-facing stop request is scoped by Web to the exact governed broker-account set. FSATS does not retain a customer principal.

When accepted/effective for exact scope it must:

1. create a new immutable control epoch;
2. deny new opening/increase-exposure intents;
3. suppress not-yet-dispatched opening work;
4. identify already-dispatched non-terminal opening/increase-exposure orders;
5. attempt cancellation of cancellable pending opening orders;
6. continue protective/reduce-only/closing management of existing exposure;
7. reconcile ACK/fill/rejection/cancellation truth;
8. classify unavoidable race fills as explicit post-stop exposure exceptions;
9. keep exceptions under Risk/Guardian/position management until reconciled;
10. present truthful state through Web.

A clean state cannot be claimed while an opening order remains capable of creating exposure.

# 21. Close-all semantics

A close-all request for exact governed broker-account scope is higher consequence and requires elevated confirmation/consequence disclosure. It first suppresses new opening exposure and addresses pending opening orders before/as part of liquidation orchestration.

Intent is not proof that positions are closed. Broker/execution/reconciliation truth controls actual outcome. P0-I no-blind-liquidation rules remain controlling under ambiguity.

# 22. Resume semantics

Resume is a new attributable request/control epoch and cannot resurrect invalidated pre-stop work. Resume cannot override active Owner restriction, Guardian restriction, Unified Risk block, entitlement/subscription restriction, security/lifecycle/broker/account restriction or stale/unknown truth.

# 23. Owner command semantics

Owner-facing command classes may include bounded stop-new-exposure, resume and close-position control within accepted policy. Each material Owner request binds authenticated Owner authority, command class, exact immutable broker-account/target scope snapshot, selected positions where applicable, non-empty reason, warning/version, elevated confirmation, trusted time/order evidence, command identity/anti-replay and resulting control epoch/state.

Owner authority does not mutate Guardian/Risk policy ownership. Owner command persistence/reconstruction must be integrity-proven across restart/failover where the command remains active. Unknown current Owner command state fails closed for affected new exposure.

# 24. Entitlement/subscription lifecycle preservation

Where a current customer/subscription product entitlement is configured outside FSATS and conveyed as governed business authorization, it constrains new-position admission. FSATS does not own customer/subscription identity merely because it consumes a bounded authorization outcome.

Historical safety semantics remain preserved where applicable:

```text
SUBSCRIPTION_OR_ENTITLEMENT_EXPIRY != BLIND_FORCED_LIQUIDATION_TRIGGER
```

If authorization expires while residual positions/opening-risk orders remain, the affected broker-account scope enters a managed-exit condition: no new exposure, opening work suppressed/cancel-attempted, monitoring/Risk/Guardian/protective/reduce-only/closing/reconciliation continue until authoritative truth proves no positions and no opening orders capable of creating new exposure remain.

Renewal never erases exception/audit evidence and cannot override other gates.

# 25. Shared Communication flow

```text
Source Application notification/report request
-> Shared Communication
-> recipient/channel/delivery workflow
-> truthful delivery outcome
-> optional recipient response state
-> exact outcome to source Application
-> source Application decides any business consequence
```

```text
SENT != DELIVERED != READ != ACKNOWLEDGED
```

Communication owns delivery/response workflow truth, not source business authority.

# 26. Awareness research egress flow

```text
AUTHORIZED APPLICATION AWARENESS RESEARCH NEED
-> owning Application
-> FSTSimA specialized research path for Trading-domain research as current design requires
-> Foundation-governed research egress when implemented/authorized
-> research content classified as learning/evidence only
-> governed candidate-development path
```

Research output cannot be operational provider data or direct Live input. FCR-0008/FCR-0011 remain runtime dependencies.

# 27. Application evolution -> FSA/Owner flow

```text
ACTUAL_ORIGIN
-> required parent Application awareness reviews
-> final MSA Application assessment
-> FSA OS/Foundation governance review where required/available
-> Owner / valid bounded delegated authority
-> separate APP-001 / manifest / admission / deployment lifecycle
```

FSA does not replace Application business judgment. Owner silence does not create authority. FCR-0012/FCR-0030 remain future Foundation runtime dependencies.

# 28. Integrity/containment/revival flow

Current P1K integrity/recovery families preserve:

```text
Application/Awareness integrity evidence
-> governed incident projection
-> authorized containment decision/path where Foundation capability exists
-> containment/kill state evidence
-> remediation/trusted-baseline evidence
-> controlled-revival request/evidence
-> independent governance/revalidation
-> authorized release/revival only from correct authority
```

```text
SIGNAL != CONTAINMENT_AUTHORITY
KILL != RECOVERY
RESTART != TRUST_RESTORATION
REVIVAL_REQUEST != REVIVAL_AUTHORIZATION
```

Foundation Stage 13/FCR-0012/FCR-0030 remain controlling for generic FSA/AI control-plane capability.

# 29. Foundation query/event boundary

`P1K-021` and `P1K-022` provide current explicit Application/Foundation information/evidence/capability query and authoritative event/decision/query semantics where supported by current Foundation capability.

These do not create a generic Application command channel into Foundation internals and cannot substitute for missing Stage 11/12/13/14 capabilities.

# 30. Failure/degraded behavior

Every edge defines behavior for producer/consumer/route unavailable, deadline expired, stale truth, duplicate/replay, schema mismatch, authority failure, partial delivery, ambiguous outcome, retry eligibility and recovery/supersession.

Mandatory synchronous cycles that can deadlock or amplify retries are prohibited unless deliberately broken by accepted safe design.

Unknown/unavailable never becomes success/zero/current.

# 31. Foundation/FCR dependencies

Current material dependencies are refreshed live. Important ones include:

- FCR-0008 research egress;
- FCR-0009 QoS/deadline transport;
- FCR-0010 resource runtime/canonical consumption state;
- FCR-0011 FSTSimA non-Live isolation;
- FCR-0012 FSA/Owner evolution control plane;
- FCR-0013 provider egress/credential refs;
- FCR-0014 broker egress/credential refs;
- FCR-0016 canonical Foundation artifact consumption;
- FCR-0030 MSA-to-FSA runtime binding;
- FCR-0031 APP-RSC canonical resource binding;
- FCR-0133 Shared Web portfolio/activity implementation binding metadata.

No missing capability is locally invented.

# 32. Negative fixtures

At minimum reject/test:

- undeclared producer/consumer;
- wildcard/FSATS-container participant;
- one-sided declaration;
- schema/version mismatch;
- stale/replayed Guardian command;
- replay/test evidence entering operational action path;
- Web customer identifier treated as FSATS customer identity;
- Web intent acting without target authorization;
- stop/resume stale control epoch;
- close-all represented complete before broker reconciliation;
- Communication SENT represented ACKNOWLEDGED;
- FSTSimA evidence self-promoting candidate;
- Trading/Guardian direct Foundation resource request bypassing APP-RSC;
- APP-RSC request treated as grant;
- stale APP-RSC coordination epoch;
- Foundation outcome mismatch;
- research output entering FSAPMA operational Data Product without authorized reacquisition;
- provider credential role reused for broker execution;
- ambiguous non-idempotent external action blindly retried;
- integrity incident self-authorizing revival.

# 33. Invariants

```text
FSATS_AS_CONTRACT_PRINCIPAL = PROHIBITED
EXACT_COUNTERPARTIES = REQUIRED
PREDECESSOR_43_FAMILIES_PRESERVED = 43/43
P1K_CURRENT_CATALOG_FAMILIES = 22
P1K_RUNTIME_ROUTES_ACTIVE = FALSE
DELIVERY != ACCEPTANCE
REQUEST != AUTHORIZATION
ROUTE_EXISTS != AUTHORITY
REPLAY != OPERATIONAL
UNKNOWN != SUCCESS
STALE != CURRENT
BROKER_ACCOUNT_IDENTITY = PRESERVED
CUSTOMER_IDENTITY = WEB_OWNED
APP_RSC_RESOURCE_FAMILIES = EXPLICIT
TRADING_DIRECT_FOUNDATION_RESOURCE_REQUEST = PROHIBITED
GUARDIAN_DIRECT_FOUNDATION_RESOURCE_REQUEST = PROHIBITED
```

# 34. Exit gates

```text
PREDECESSOR_43_FAMILY_COUNT = 43/43
CURRENT_P1K_FAMILY_COUNT = 22/22
UNEXPLAINED_CONTRACT_DROPS = 0
UNEXPLAINED_CONTRACT_MERGES = 0
WILDCARD_PARTICIPANTS = 0
AUTHORITY_CLASS_AMBIGUITY = 0
SECURITY_CLASS_AMBIGUITY = 0
BROKER_ACCOUNT_CUSTOMER_IDENTITY_CONFLATION = 0
APP_RSC_FOUNDATION_AUTHORITY_CONFLATION = 0
REPLAY_OPERATIONAL_ESCALATION_PATHS = 0
AMBIGUOUS_EXTERNAL_ACTION_BLIND_RETRY = 0
RUNTIME_ROUTE_OVERCLAIM = 0
```

# 35. Non-grant

Acceptance of P0-F would establish contract/information-flow architecture only. It would not activate routes, provider/broker/research egress, credentials, APP-RSC canonical Foundation runtime binding, Paper, Shadow, Tiny-Live, Live or deployment.