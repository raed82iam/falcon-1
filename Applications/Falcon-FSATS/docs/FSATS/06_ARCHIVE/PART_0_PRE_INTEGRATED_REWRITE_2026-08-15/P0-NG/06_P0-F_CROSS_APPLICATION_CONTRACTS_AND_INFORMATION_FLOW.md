# P0-F — Cross-Application Contracts, Authority, Security and Information Flow

**Status:** `FINAL_CONSOLIDATION_CANDIDATE / P0-NG PLAN OWNER_ACCEPTED / NOT_FINAL_OWNER_CLOSED`  
**Scope:** `P0-F only`  
**Implementation Authority:** `NOT GRANTED`  
**Runtime Route Authority:** `NOT GRANTED`  
**Consolidation Basis:** Owner-accepted P0-NG plan + exact current accepted P0-F `89` through `89I` semantics

---

## 1. Purpose

P0-F defines every material cross-Application interaction as an explicit, attributable, governable contract edge rather than hidden coupling.

This final consolidation directly materializes the exact current accepted **43 Application-to-Application contract families** and the controlling P0-F hardenings into one directly readable design.

No reader is required to compose predecessor files `89` through `89I` to determine current P0-F meaning after final acceptance of this candidate.

---

## 2. Responsibility

P0-F owns the Application-side design for:

- exact cross-Application contract-family identity;
- exact producer and consumer identity;
- purpose and business meaning;
- bilateral declaration;
- authority classification;
- security/trust classification;
- schema/version compatibility;
- information-flow semantics;
- environment/truth classification;
- freshness, deadline and ordering semantics;
- correlation and causation;
- idempotency, duplicate and replay behavior;
- failure/degraded behavior;
- user, Owner, Guardian and Risk command boundaries where cross-Application;
- current Foundation/FCR dependency linkage;
- contract-graph completeness and negative fixtures.

P0-F does **not** implement Foundation FIL, Service Bus, Event System, cryptography, lifecycle, resource governance, external egress, credentials, provider connectivity or broker connectivity.

---

## 3. Prime Contract Rules

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

FSATS is a system boundary, not a producer, consumer, principal or authority source.

---

## 4. Canonical Contract-Family Namespace

Application business contract-family identities use the canonical family form:

`falcon.xapp.<producer-domain>.<producer>.<consumer-domain>.<consumer>.<purpose>`

These are Application architecture contract identities.

They are distinct from:

- Foundation `CON-*` identities;
- Foundation route IDs;
- endpoint IDs;
- schema/message IDs;
- Application package IDs;
- authority/delegation IDs.

```text
P0F_CONTRACT_ID != FOUNDATION_ROUTE_ID
P0F_CONTRACT_ID != ENDPOINT_ID
P0F_CONTRACT_ID != SCHEMA_ID
```

---

# 5. Exact Canonical 43-Family Inventory

The following list is the exact initial migrated Application-to-Application contract-family baseline extracted from the accepted P0-F source bytes.

## 5.1 Trading and FSAPMA — 3

| # | Contract Family ID | Producer | Consumer | Class | Purpose |
|---:|---|---|---|---|---|
| 1 | `falcon.xapp.trading.core.trading.fsapma.data-requirement` | Trading | FSAPMA | REQUEST | provider-independent operational data requirement |
| 2 | `falcon.xapp.trading.fsapma.trading.core.operational-data-product` | FSAPMA | Trading | DATA_PRODUCT | normalized operational trading data |
| 3 | `falcon.xapp.trading.fsapma.trading.core.provider-service-status` | FSAPMA | Trading | PROJECTION/EVENT | provider/data-service status |

## 5.2 Guardian and Trading — 4

| # | Contract Family ID | Producer | Consumer | Class | Purpose |
|---:|---|---|---|---|---|
| 4 | `falcon.xapp.trading.guardian.trading.core.protection-command` | Guardian | Trading | COMMAND | bounded protection/restriction command |
| 5 | `falcon.xapp.trading.core.trading.guardian.safety-projection` | Trading | Guardian | PROJECTION/EVENT | bounded safety/exposure evidence |
| 6 | `falcon.xapp.trading.core.trading.guardian.protection-command-outcome` | Trading | Guardian | OUTCOME | exact protection-command business outcome |
| 7 | `falcon.xapp.trading.guardian.trading.core.protection-release` | Guardian | Trading | COMMAND | scoped recovery/release direction |

## 5.3 Guardian and FSAPMA — 3

| # | Contract Family ID | Producer | Consumer | Class | Purpose |
|---:|---|---|---|---|---|
| 8 | `falcon.xapp.trading.guardian.trading.fsapma.provider-protection-command` | Guardian | FSAPMA | COMMAND | bounded provider-use protection constraint |
| 9 | `falcon.xapp.trading.fsapma.trading.guardian.provider-integrity-projection` | FSAPMA | Guardian | PROJECTION/EVENT | provider/data integrity evidence |
| 10 | `falcon.xapp.trading.fsapma.trading.guardian.provider-protection-outcome` | FSAPMA | Guardian | OUTCOME | provider-protection application/rejection outcome |

## 5.4 FSTSimA sibling flows — 7

| # | Contract Family ID | Producer | Consumer | Class | Purpose |
|---:|---|---|---|---|---|
| 11 | `falcon.xapp.trading.core.validation.fstsima.validation-input` | Trading | FSTSimA | REQUEST/EVIDENCE_PACKAGE | Trading candidate validation input |
| 12 | `falcon.xapp.trading.guardian.validation.fstsima.validation-input` | Guardian | FSTSimA | REQUEST/EVIDENCE_PACKAGE | Guardian candidate validation input |
| 13 | `falcon.xapp.trading.fsapma.validation.fstsima.validation-input` | FSAPMA | FSTSimA | REQUEST/EVIDENCE_PACKAGE | FSAPMA candidate validation input |
| 14 | `falcon.xapp.trading.fsapma.validation.fstsima.nonlive-data-input` | FSAPMA | FSTSimA | DATA_PRODUCT/EVIDENCE_PACKAGE | explicitly non-Live replay/test/calibration input |
| 15 | `falcon.xapp.validation.fstsima.trading.core.validation-evidence` | FSTSimA | Trading | EVIDENCE_PACKAGE | reproducible Trading validation evidence |
| 16 | `falcon.xapp.validation.fstsima.trading.guardian.validation-evidence` | FSTSimA | Guardian | EVIDENCE_PACKAGE | reproducible Guardian validation evidence |
| 17 | `falcon.xapp.validation.fstsima.trading.fsapma.validation-evidence` | FSTSimA | FSAPMA | EVIDENCE_PACKAGE | reproducible FSAPMA validation evidence |

## 5.5 Domain/validation presentation projections to Shared Web — 4

| # | Contract Family ID | Producer | Consumer | Class |
|---:|---|---|---|---|
| 18 | `falcon.xapp.trading.guardian.shared.web.presentation-projection` | Guardian | Web | PROJECTION/EVENT |
| 19 | `falcon.xapp.trading.fsapma.shared.web.presentation-projection` | FSAPMA | Web | PROJECTION/EVENT |
| 20 | `falcon.xapp.trading.core.shared.web.presentation-projection` | Trading | Web | PROJECTION/EVENT |
| 21 | `falcon.xapp.validation.fstsima.shared.web.presentation-projection` | FSTSimA | Web | PROJECTION/EVENT |

## 5.6 Shared Web user-intent paths — 4

| # | Contract Family ID | Producer | Consumer | Class |
|---:|---|---|---|---|
| 22 | `falcon.xapp.shared.web.trading.guardian.user-intent` | Web | Guardian | USER_INTENT |
| 23 | `falcon.xapp.shared.web.trading.fsapma.user-intent` | Web | FSAPMA | USER_INTENT |
| 24 | `falcon.xapp.shared.web.trading.core.user-intent` | Web | Trading | USER_INTENT |
| 25 | `falcon.xapp.shared.web.validation.fstsima.user-intent` | Web | FSTSimA | USER_INTENT |

The existence of a user-intent family does not authorize every possible command type. Unspecified command types are denied.

## 5.7 Business outcomes back to Shared Web — 4

| # | Contract Family ID | Producer | Consumer | Class |
|---:|---|---|---|---|
| 26 | `falcon.xapp.trading.guardian.shared.web.user-intent-outcome` | Guardian | Web | OUTCOME |
| 27 | `falcon.xapp.trading.fsapma.shared.web.user-intent-outcome` | FSAPMA | Web | OUTCOME |
| 28 | `falcon.xapp.trading.core.shared.web.user-intent-outcome` | Trading | Web | OUTCOME |
| 29 | `falcon.xapp.validation.fstsima.shared.web.user-intent-outcome` | FSTSimA | Web | OUTCOME |

Each outcome SHALL bind the exact originating user-intent identity and distinguish technical receipt from business acceptance/completion.

## 5.8 Notification/report requests to Shared Communication — 4

| # | Contract Family ID | Producer | Consumer | Class |
|---:|---|---|---|---|
| 30 | `falcon.xapp.trading.guardian.shared.communication.notification-request` | Guardian | Communication | NOTIFICATION_REQUEST |
| 31 | `falcon.xapp.trading.fsapma.shared.communication.notification-request` | FSAPMA | Communication | NOTIFICATION_REQUEST |
| 32 | `falcon.xapp.trading.core.shared.communication.notification-request` | Trading | Communication | NOTIFICATION_REQUEST |
| 33 | `falcon.xapp.validation.fstsima.shared.communication.notification-request` | FSTSimA | Communication | NOTIFICATION_REQUEST |

## 5.9 Communication delivery outcomes — 4

| # | Contract Family ID | Producer | Consumer | Class |
|---:|---|---|---|---|
| 34 | `falcon.xapp.shared.communication.trading.guardian.delivery-outcome` | Communication | Guardian | OUTCOME |
| 35 | `falcon.xapp.shared.communication.trading.fsapma.delivery-outcome` | Communication | FSAPMA | OUTCOME |
| 36 | `falcon.xapp.shared.communication.trading.core.delivery-outcome` | Communication | Trading | OUTCOME |
| 37 | `falcon.xapp.shared.communication.validation.fstsima.delivery-outcome` | Communication | FSTSimA | OUTCOME |

## 5.10 Communication recipient responses — 4

| # | Contract Family ID | Producer | Consumer | Class |
|---:|---|---|---|---|
| 38 | `falcon.xapp.shared.communication.trading.guardian.recipient-response` | Communication | Guardian | OUTCOME/EVENT |
| 39 | `falcon.xapp.shared.communication.trading.fsapma.recipient-response` | Communication | FSAPMA | OUTCOME/EVENT |
| 40 | `falcon.xapp.shared.communication.trading.core.recipient-response` | Communication | Trading | OUTCOME/EVENT |
| 41 | `falcon.xapp.shared.communication.validation.fstsima.recipient-response` | Communication | FSTSimA | OUTCOME/EVENT |

The source Application decides any resulting business action under its own authority.

## 5.11 Shared Web and Shared Communication — 2

| # | Contract Family ID | Producer | Consumer | Class | Purpose |
|---:|---|---|---|---|---|
| 42 | `falcon.xapp.shared.web.shared.communication.recipient-response-intent` | Web | Communication | USER_INTENT | user response intent for an exact Communication-owned workflow |
| 43 | `falcon.xapp.shared.communication.shared.web.communication-status-projection` | Communication | Web | PROJECTION/EVENT | least-privilege communication delivery/response status projection |

## 5.12 Count Proof

```text
TRADING_FSAPMA = 3
GUARDIAN_TRADING = 4
GUARDIAN_FSAPMA = 3
FSTSIMA_SIBLING = 7
PRESENTATION_TO_WEB = 4
WEB_USER_INTENTS = 4
OUTCOMES_TO_WEB = 4
NOTIFICATION_REQUESTS = 4
DELIVERY_OUTCOMES = 4
RECIPIENT_RESPONSES = 4
WEB_COMMUNICATION = 2
--------------------------------
TOTAL = 43
```

The 43-family set is the exact initial migrated baseline, not a Falcon maximum. A future new family requires governed design and authority review.

```text
EXACT_CURRENT_ACCEPTED_FAMILIES_MATERIALIZED = 43/43
UNEXPLAINED_DROPS = 0
UNEXPLAINED_MERGES = 0
```

---

# 6. Bilateral Declaration

Every one of the 43 families SHALL be declared compatibly by both participants before runtime integration may be admitted.

Producer/requester declaration SHALL include at least:

- exact contract-family identity;
- exact intended counterparty;
- provided/requested capability;
- authority/permission requirement;
- schema/message version rule;
- environment/truth classification;
- failure/degraded behavior.

Consumer/responder declaration SHALL include at least:

- exact same family identity;
- exact permitted producer/requester;
- consumed/responded capability;
- authority/permission requirement;
- compatible schema/message version rule;
- environment/truth classification;
- failure/degraded behavior.

One-sided declaration, participant mismatch, incompatible version, capability mismatch or authority mismatch fails closed.

```text
PRODUCER_DECLARED != RELATIONSHIP_ADMITTED
CONSUMER_DECLARED != RELATIONSHIP_ADMITTED
```

---

# 7. Application Authority Classes

Each family SHALL bind one effective Application authority class.

### INFORMATION_REQUEST
Asks an owner to perform its own behavior. The requester does not acquire responder ownership.

### OWNER_TRUTH_PUBLICATION
Publishes bounded owner-controlled business truth through DATA_PRODUCT, PROJECTION or operational EVENT semantics. Consumer use does not transfer ownership.

### DELEGATED_PROTECTION_COMMAND
Applies only to exact Guardian protection/release commands and requires active attributable Guardian authority bounded to exact scope.

### NONAUTHORITATIVE_VALIDATION_EXCHANGE
Applies to FSTSimA validation inputs/evidence and non-Live replay/test/calibration. Never creates Live authority or automatic promotion.

### USER_INTENT_FORWARDING
Carries authenticated/attributed human intent. UI interaction is not target business authorization.

### SHARED_SERVICE_REQUEST
Requests Communication-owned delivery behavior without transferring source business meaning.

### BUSINESS_OUTCOME_RETURN
Returns bounded result state tied to the exact initiating request/command/intent and creates no unrelated follow-on authority.

## 7.1 Segment Mapping

- Trading -> FSAPMA data requirement: `INFORMATION_REQUEST`.
- FSAPMA -> Trading operational data/status: `OWNER_TRUTH_PUBLICATION`.
- Guardian -> Trading protection/release: `DELEGATED_PROTECTION_COMMAND`.
- Trading -> Guardian safety projection: `OWNER_TRUTH_PUBLICATION`.
- Trading -> Guardian command outcome: `BUSINESS_OUTCOME_RETURN`.
- Guardian -> FSAPMA protection: `DELEGATED_PROTECTION_COMMAND`.
- FSAPMA -> Guardian integrity projection: `OWNER_TRUTH_PUBLICATION`.
- FSAPMA -> Guardian protection outcome: `BUSINESS_OUTCOME_RETURN`.
- Domain/validation -> FSTSimA inputs and FSAPMA non-Live data: `NONAUTHORITATIVE_VALIDATION_EXCHANGE`.
- FSTSimA -> domain validation evidence: `NONAUTHORITATIVE_VALIDATION_EXCHANGE`.
- Domain/validation -> Web presentation: `OWNER_TRUTH_PUBLICATION` limited to presentation.
- Web -> domain/validation: `USER_INTENT_FORWARDING`.
- Domain/validation -> Web outcomes: `BUSINESS_OUTCOME_RETURN`.
- Domain/validation -> Communication: `SHARED_SERVICE_REQUEST`.
- Communication -> source outcomes/responses: `BUSINESS_OUTCOME_RETURN`.
- Web -> Communication recipient-response intent: `USER_INTENT_FORWARDING`.
- Communication -> Web communication status: `OWNER_TRUTH_PUBLICATION` limited to Communication-owned state.

---

# 8. Security Semantic Classes

## CONTROL_CRITICAL
Guardian protection/release commands and command outcomes require exact identities, strong integrity/authenticity using accepted Foundation mechanisms, authority binding, anti-replay/idempotency, expiry/freshness, least privilege, immutable evidence and fail-closed downgrade behavior.

## OPERATIONAL_TRADING_SENSITIVE
Operational market-data requirements/products, provider integrity/status, and Trading safety/exposure projections require integrity, provenance, freshness, correction lineage, confidentiality where needed, exact counterparties and no secret leakage.

## NONLIVE_VALIDATION_SENSITIVE
FSTSimA exchanges require explicit non-Live classification, exact artifact/provenance identities, isolation from Live credentials/routes, appropriate confidentiality, and zero production-mutation authority.

## USER_INTERACTION_SENSITIVE
Web projections/intents/outcomes require user/session/entitlement attribution, privacy and tenant isolation, anti-replay/idempotency for material commands, least-privilege projections, consequence-appropriate confirmation evidence and no secrets in UI payloads.

## COMMUNICATION_SENSITIVE
Notification requests/outcomes/responses require recipient/tenant/Application attribution, confidentiality classification preservation, dedupe/expiry, truthful channel outcomes and no cross-consumer leakage.

Where multiple classes apply, requirements are cumulative and the stronger applicable requirement controls.

---

# 9. Schema, Version and Lifecycle Compatibility

Before an updated Application package is admitted:

- both sides' declared family identities must still match;
- exact supported schema/message versions must have a non-empty compatible intersection;
- authority/security classes must remain compatible;
- mandatory fields may not disappear silently;
- semantic reinterpretation requires governed versioning;
- rollback must restore a compatible contract set or remain isolated;
- a producer update/removal cannot silently break a dependent Application.

Authority-bearing/safety-relevant runtime semantics SHALL NOT use an unresolved `latest` version.

---

# 10. Time, Freshness, Ordering, Correlation and Causation

Where material, each contract distinguishes:

- source/event time;
- publication time;
- receive time;
- decision/effective time;
- expiry/validity horizon;
- dispatch time;
- outcome time;
- correction/supersession order.

A transport hop SHALL NOT silently reset an end-to-end deadline.

Correlation groups related workflow items. Causation identifies the immediate cause. A multi-hop workflow SHALL preserve both without flattening causation into one ambiguous correlation ID.

---

# 11. Idempotency, Duplicate, Replay and Correction

Every material contract SHALL state whether it is:

- idempotent;
- deduplicated by identity;
- retryable;
- replayable only under non-authoritative classification;
- non-replayable;
- correctable/supersedable.

```text
DUPLICATE_DELIVERY != DUPLICATE_BUSINESS_ACTION
REPLAY_TEST_MESSAGE != OPERATIONAL_COMMAND
VALID_SIGNATURE != LIVE_ACTION_AUTHORITY
```

---

# 12. Canonical End-to-End Operational Data Flow

```text
Trading business data requirement
 -> exact Trading->FSAPMA contract
 -> Foundation-governed admitted transport
 -> FSAPMA request validation/provider realization
 -> FSAPMA normalized operational Data Product
 -> exact FSAPMA->Trading contract
 -> Trading freshness/quality/provenance validation
 -> Trading-owned analysis/Risk/decision
```

Trading never bypasses FSAPMA for operational market data.

Foundation delivery does not prove data sufficiency for a trade.

---

# 13. Canonical Guardian Protection Flow

```text
Domain-owned evidence/projection
 -> Guardian
 -> Guardian protection assessment
 -> exact scoped Guardian protection command
 -> governed Foundation transport
 -> target validates authority/scope/currentness
 -> target applies only owned behavior under restriction
 -> exact business command outcome
 -> Guardian reconciliation
 -> later release only as a new governed command
```

Guardian does not become Trading Risk owner, FSAPMA provider owner, broker truth owner or Foundation resource owner.

Transport ACK is not protection effect.

---

# 14. Canonical FSTSimA Validation Flow

```text
Owning Application candidate
 -> immutable exact validation input package
 -> FSTSimA non-Live validation
 -> immutable validation evidence
 -> owning Application awareness/business evaluation
 -> origin-correct CSA/LSA/MSA/FSA/Owner governance as applicable
```

FSTSimA cannot promote/adopt the candidate and cannot inherit Live authority from realistic data.

---

# 15. Shared Web Read/Write Flow

Read path:

```text
Authoritative source Application
 -> least-privilege presentation projection
 -> Shared Web read model
```

Web never becomes source truth owner.

Write path:

```text
User interaction
 -> attributed user/session/entitlement/consent evidence
 -> exact USER_INTENT to exact target Application
 -> target authority/business validation
 -> target bounded action/rejection
 -> exact user-intent outcome
 -> Web presentation
```

```text
UI_CLICK != BUSINESS_AUTHORIZATION
```

---

# 16. Shared Communication Flow

```text
Source Application notification/report request
 -> Shared Communication
 -> recipient/channel/delivery workflow
 -> truthful delivery outcome
 -> optional Communication-owned recipient response state
 -> exact outcome back to source Application
 -> source Application decides any business consequence
```

Web-assisted recipient response is intent to Communication, not acknowledgement truth by itself.

```text
SENT != DELIVERED != READ != ACKNOWLEDGED
```

---

# 17. Current Foundation-Facing Resource Flow

The old pre-TARC interpretation in historical P0-F material is superseded by the current Owner-controlled resource model.

For the Falcon Self-Aware Trading Application:

```text
DOMAIN / GUARDIAN / MSA / LSA / CSA RESOURCE NEED OR URGENCY EVIDENCE
 -> TARC
 -> TARC POLICY / CURRENT ADMITTED ALLOCATION ASSESSMENT
 -> TARC <-> FOUNDATION RESOURCE GOVERNANCE
 -> FOUNDATION GRANT / PARTIAL / CAP / DENY / REDUCE / REVOKE / RESTORE
 -> TARC INTERNAL RESPONSE
```

Only TARC is the Trading-side Foundation resource requester/controller role.

```text
GUARDIAN_DIRECT_FOUNDATION_RESOURCE_REQUEST = PROHIBITED
FSA_RESOURCE_REQUEST_ENDPOINT = PROHIBITED
REQUESTED_RESOURCE != GRANTED_RESOURCE
```

FCR-0007 and FCR-0010 remain open and `Waiting On: FOUNDATION` for the later runtime capability stages.

---

# 18. Awareness Research Egress Flow

```text
MSA / LSA / ELIGIBLE CSA RESEARCH NEED
 -> owning Application
 -> Foundation-governed research egress when available
 -> research content as learning/evidence only
 -> governed candidate-development path
```

Research output cannot be used as operational market data or direct Live action input.

FCR-0008 remains open / `Waiting On: FOUNDATION`.

---

# 19. Application Evolution to FSA/Owner Flow

```text
ACTUAL ORIGIN
 -> required parent Application awareness reviews
 -> final Application MSA assessment
 -> FSA OS/Foundation governance review
 -> Owner / valid pre-existing bounded delegated authority
 -> separate APP-001 / manifest / admission / deployment lifecycle
```

FSA does not replace Application business judgment.

Owner silence does not create authority.

FCR-0012 remains open / `Waiting On: FOUNDATION` for the runtime governance control plane.

---

# 20. User Trading Command Semantics

## 20.1 User Stop New Exposure

`UC-STOP-NEW-EXPOSURE` is strictly scoped to the authenticated requesting user's governed account/environment scope unless separately authorized otherwise.

When effective it SHALL:

1. establish a new immutable command/control epoch;
2. deny new opening/increase-exposure intents;
3. suppress not-yet-dispatched opening/increase-exposure orders;
4. identify already-dispatched non-terminal opening/increase-exposure orders;
5. cancel or attempt cancellation of every cancellable pending opening/increase-exposure order;
6. continue protective/reduce-only/closing management of existing exposure;
7. reconcile ACK/fill/rejection/cancellation truth;
8. classify unavoidable race fills as explicit `POST_STOP_EXPOSURE_EXCEPTION`;
9. keep such exceptions under Risk/Guardian/position management until reconciled;
10. present truthful user state.

Minimum user-visible states include:

```text
STOP_REQUEST_RECEIVED
STOP_EFFECTIVE_NO_NEW_INTENTS
PENDING_OPEN_ORDER_CANCELLATION_IN_PROGRESS
STOP_EFFECTIVE_WITH_IN_FLIGHT_EXCEPTION
STOP_EFFECTIVE_CLEAN
```

A clean state cannot be claimed while an opening order remains capable of creating exposure.

## 20.2 User Close All Positions

`UC-CLOSE-ALL-USER-POSITIONS` is a materially higher-consequence request requiring explicit elevated confirmation and consequence disclosure.

It SHALL first suppress new opening exposure and address pending opening orders before or as part of liquidation orchestration.

User intent is not proof that positions are closed. Broker/execution/reconciliation truth controls the actual outcome.

## 20.3 User Resume

A later resume is a new attributable command and new control epoch. It cannot resurrect invalidated pre-stop work.

User resume cannot override:

- an active Owner restriction;
- Guardian restriction;
- Unified Risk block;
- subscription/entitlement restriction;
- security/lifecycle/broker/account restriction.

---

# 21. Project Owner Trading Override Semantics

Owner-only current manual Trading control classes include:

- `OWNER_STOP_NEW_EXPOSURE`;
- `OWNER_RESUME_NEW_EXPOSURE`;
- `OWNER_CLOSE_POSITIONS`.

Owner commands may target one user, an immutable explicit user set, or an explicitly defined all-user scope.

Every Owner command SHALL bind:

- authenticated Owner identity and authority context;
- exact command class;
- exact target snapshot/scope class;
- exact position selection where applicable;
- non-empty Owner-provided reason;
- risk warning/version shown;
- elevated confirmation;
- trusted time/order evidence;
- command identity and anti-replay evidence;
- resulting control epoch/state.

Ordinary users do not inherit multi-user/all-user Owner authority.

## 21.1 Owner Command Precedence

Within the same affected Trading-control scope:

1. independent Guardian / Unified Risk / regulatory / broker safety restrictions remain separately authoritative;
2. active Project Owner Trading-control commands outrank ordinary user Trading-control commands;
3. user commands apply only where no conflicting active Owner command exists.

```text
OWNER_STOP + USER_RESUME -> USER_RESUME_REJECTED
OWNER_COMMAND != GUARDIAN_OR_RISK_POLICY_MUTATION
```

An Owner command persists across user logout, restart, failover or process replacement until explicitly superseded/revoked/expired according to its accepted semantics.

If current Owner command state cannot be reconstructed with integrity, affected new exposure fails closed.

---

# 22. Subscription and Position Lifecycle

Subscription status constrains **new-position admission** before expiry.

```text
POSITION_MAX_LIFECYCLE + EXIT_MARGIN <= REMAINING_AUTHORIZED_SUBSCRIPTION_WINDOW
```

Longer-horizon positions become ineligible earlier as expiry approaches. Exact time thresholds belong to later Trading design and are not invented here.

Subscription expiry alone is not an unconditional forced-liquidation instruction.

```text
SUBSCRIPTION_EXPIRY != FORCED_LIQUIDATION_TRIGGER
```

The design objective is:

```text
OPEN_POSITIONS_AT_PLANNED_SUBSCRIPTION_EXPIRY = 0
```

but execution truth must never be falsified.

## 22.1 POST_EXPIRY_MANAGED_EXIT

If authoritative subscription state is expired while residual positions or opening-risk orders remain, the user/account enters:

`POST_EXPIRY_MANAGED_EXIT`

While active:

- no new exposure is permitted;
- exposure-increasing strategy actions are denied;
- not-yet-transmitted opening orders are suppressed;
- cancellable pending opening orders are cancellation-attempted;
- monitoring continues;
- Unified Risk continues;
- Guardian protection continues;
- protective/reduce-only/closing actions remain permitted where otherwise valid;
- broker/execution reconciliation continues;
- user-visible residual truth remains available.

The state terminates only when authoritative reconciliation establishes:

```text
OPEN_POSITIONS = 0
AND OPENING_ORDERS_CAPABLE_OF_CREATING_NEW_EXPOSURE = 0
```

Renewal does not erase historical exception evidence and restores ordinary eligibility only after authoritative renewal plus all other gates.

User or Owner Trading-resume commands do not manufacture subscription entitlement.

---

# 23. Shared Web and Shared Communication Non-Authority

Shared Web:

- captures/presents intent;
- presents warnings/confirmation;
- forwards exact attributable evidence;
- displays authoritative outcomes;
- does not itself stop/resume/liquidate/approve Trading behavior.

Shared Communication:

- delivers source-owned notifications/reports;
- owns communication delivery/recipient workflow truth;
- does not create/broaden source business authority.

```text
WEB != OWNER
WEB != TRADING_AUTHORITY
COMMUNICATION != SOURCE_BUSINESS_AUTHORITY
```

---

# 24. Failure and Degraded Behavior

Each edge SHALL define behavior for:

- producer unavailable;
- consumer unavailable;
- route unavailable;
- deadline expired;
- stale truth;
- duplicate/replay;
- schema/version mismatch;
- authority/permission failure;
- partial delivery;
- ambiguous business outcome;
- retry eligibility;
- recovery/supersession.

Transport retry SHALL NOT blindly repeat an externally ambiguous, non-idempotent business action.

Mandatory synchronous cycles that can deadlock or amplify retries are prohibited unless deliberately broken by a safe accepted design.

---

# 25. Foundation / FCR Runtime Dependencies

Current material open dependencies include:

- FCR-0004 Guardian governed protection-command route;
- FCR-0005 operational market-data delivery;
- FCR-0006 event/evidence/replay delivery;
- FCR-0007 TARC resource request/decision boundary;
- FCR-0008 awareness research egress;
- FCR-0009 deadline/QoS-aware transport;
- FCR-0010 resource pressure/shedding/reclamation/restoration stages;
- FCR-0011 FSTSimA non-Live enforcement;
- FCR-0012 FSA/Owner evolution control plane;
- FCR-0013 provider egress/credential references;
- FCR-0014 broker execution egress/credential references;
- FCR-0016 canonical Foundation artifact consumption.

Open FCRs remain fail-closed for the dependent runtime behavior.

`ACCEPTED_FOR_PLANNING` never means runtime implemented.

---

# 26. Explicit Non-Authority

P0-F SHALL NOT:

- create or activate Foundation routes;
- create provider/broker egress;
- create credential entitlement;
- convert message delivery into business success;
- permit container/wildcard authority;
- merge distinct business contract identities merely to simplify documentation;
- let Web/Communication become source authority;
- let FSTSimA/replay traffic become operational authority;
- let Guardian bypass TARC for Trading resource requests;
- let user resume override Owner/Guardian/Risk/subscription restrictions;
- let Owner Trading command silently modify Guardian/Risk policy;
- let subscription expiry fabricate closed-position truth.

---

# 27. Canonical Invariants

```text
P0F_INITIAL_CONTRACT_BASELINE = EXACT_43_FAMILIES
EXACT_43_MATERIALIZED = 43/43
UNEXPLAINED_DROP = 0
UNEXPLAINED_MERGE = 0
BILATERAL_DECLARATION = REQUIRED
CONTAINER_PARTICIPANT = PROHIBITED
NOTICE != COMMAND
REQUEST != GRANT
DELIVERY_ACK != BUSINESS_OUTCOME
REPLAY_TEST_TRUTH != OPERATIONAL_AUTHORITY
USER_STOP_NEW_EXPOSURE != BLIND_LIQUIDATION
CLOSE_REQUEST != ZERO_EXPOSURE
OWNER_COMMAND != AUTOMATIC_GUARDIAN_OR_RISK_OVERRIDE
OWNER_STOP_CANNOT_BE_ORDINARY_USER_OVERRIDDEN
SUBSCRIPTION_EXPIRY != FORCED_LIQUIDATION_TRIGGER
POST_EXPIRY_MANAGED_EXIT != ACTIVE_SUBSCRIPTION
GUARDIAN_DIRECT_TRADING_RESOURCE_REQUEST = PROHIBITED
TARC = SOLE_TRADING_FOUNDATION_RESOURCE_REQUEST_ROLE
REQUESTED_RESOURCE != GRANTED_RESOURCE
```

---

# 28. Forbidden Interpretations

Invalid interpretations include:

- “Service Bus route exists, therefore Guardian business authority exists”;
- “message delivered, therefore a position is closed”;
- “Web click is execution authorization”;
- “valid replay signature permits Live action”;
- “user stop means blindly liquidate all positions”;
- “Owner stop can be cancelled by an affected ordinary user”;
- “Owner resume bypasses Guardian/Risk”;
- “subscription expiry means forced liquidation”;
- “expired subscription with residual position means the position disappears”;
- “same metadata/security profile means two contract families may be merged”;
- “Guardian emergency can request Trading resources directly from Foundation”;
- “FSA is the operational Trading resource endpoint”.

---

# 29. Mandatory Negative / Adversarial Fixtures

At minimum the final implementation/verification design SHALL cover:

- all 43 exact identities present and unique;
- producer/consumer mismatch;
- wildcard/container participant;
- one-sided declaration;
- incompatible schema/version;
- unauthorized producer/wrong consumer;
- stale/expired command;
- replayed user/Owner/Guardian command;
- duplicate close command;
- replay truth republished as operational;
- delivery ACK represented as business outcome;
- user stop race with pending opening order;
- unavoidable race fill correctly classified and managed;
- stale pre-stop work arriving after new epoch;
- resume attempting to resurrect stale orders;
- user resume against Owner restriction;
- Owner resume against Guardian/Risk block;
- forged/cross-user command scope;
- Owner command missing reason/warning/confirmation;
- Owner command scope snapshot ambiguity;
- subscription progressive restriction failure;
- expiry treated as forced liquidation;
- residual post-expiry position represented as closed;
- renewal assumed from pending/unverified state;
- cross-user data leakage;
- mandatory synchronous cycle/deadlock;
- retry amplification;
- Guardian direct Foundation resource request attempt;
- FSA used as operational resource endpoint.

---

# 30. Traceability / Supersession

This candidate directly consolidates the current effective semantics formerly distributed across:

- `89_P0F_CANONICAL_CROSS_APPLICATION_CONTRACT_AND_INFORMATION_FLOW_CANDIDATE.md`;
- `89A_P0F_EXACT_SHARED_APPLICATION_AND_COVERAGE_HARDENING.md`;
- `89B_P0F_BILATERAL_DECLARATION_AUTHORITY_SECURITY_AND_VERSION_HARDENING.md`;
- `89C_P0F_END_TO_END_INFORMATION_FLOW_AND_SECURITY_BINDING_HARDENING.md`;
- `89D_P0F_USER_COMMAND_SCOPE_AND_ACCOUNTABILITY_HARDENING.md`;
- `89E_P0F_USER_STOP_ORDER_RACE_AND_PENDING_EXPOSURE_HARDENING.md`;
- `89F_P0F_OWNER_TRADING_OVERRIDE_AUTHORITY_AND_ACCOUNTABILITY_HARDENING.md`;
- `89G_P0F_OWNER_COMMAND_PRECEDENCE_AND_NON_OVERRIDABLE_USER_SCOPE_HARDENING.md`;
- `89H_P0F_SUBSCRIPTION_EXPIRY_AND_POSITION_LIFECYCLE_HARDENING.md`;
- `89I_P0F_POST_EXPIRY_MANAGED_EXIT_EXCEPTION_HARDENING.md`.

Historical bytes remain provenance until the final Owner-approved archival operation.

The old Guardian-direct resource escalation interpretation is **not** carried forward. Current TARC semantics control.

---

# 31. Exit Gates

```text
EXACT_CURRENT_ACCEPTED_FAMILIES_MATERIALIZED = 43/43
UNEXPLAINED_DROPS = 0
UNEXPLAINED_MERGES = 0
UNDECLARED_EDGES = 0
ONE_SIDED_EDGES = 0
WILDCARD_CURRENT_PARTICIPANTS = 0
AUTHORITY_CLASS_MAPPING = COMPLETE
SECURITY_CLASS_MAPPING = COMPLETE
USER_STOP_RACE_MODEL = COMPLETE
OWNER_PRECEDENCE_MODEL = COMPLETE
SUBSCRIPTION_MANAGED_EXIT_MODEL = COMPLETE
GUARDIAN_TARC_RESOURCE_SEPARATION = PASS
DELIVERY_BUSINESS_OUTCOME_CONFLATION = 0
REPLAY_OPERATIONAL_ESCALATION_PATHS = 0
FRESH_ARCHITECTURE_REVIEW = REQUIRED
FRESH_RED_TEAM_REVIEW = REQUIRED
FINAL_OWNER_CLOSURE = REQUIRED
```

---

## 32. Next Authorized Gate

This P0-F final consolidation candidate establishes design semantics only after final review and Owner closure.

It does not activate cross-Application routes, research egress, provider/broker connectivity, credentials, resource-request runtime, Paper, Tiny Live, Live, deployment, leverage, derivatives, or additional markets.
