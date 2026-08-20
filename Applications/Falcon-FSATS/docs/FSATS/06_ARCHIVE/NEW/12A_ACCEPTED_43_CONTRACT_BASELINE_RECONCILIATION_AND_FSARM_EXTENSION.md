# FSATS SIA — Accepted 43-Contract Baseline Reconciliation and FSARM Extension

**Package:** `FSATS-SIA-v0.1`
**Status:** `DESIGN_CANDIDATE / CONTROLLING CONTRACT-INVENTORY RECONCILIATION`
**Basis:** accepted/historical P0-F 43-family materialization + new SIA contract schemas + APP-RSC/FSARM candidate

## 1. Purpose

Correct the initial SIA contract inventory before semantic freeze.

`12_CROSS_APPLICATION_CONTRACT_SCHEMA_AND_ROUTE_CATALOG.md` defined detailed schemas for the new core Application interactions but initially enumerated 37 candidate families and explicitly left historical 43-family reconciliation pending.

Fresh comparison against `P0-F_CROSS_APPLICATION_CONTRACTS_AUTHORITY_SECURITY_AND_INFORMATION_FLOW.md` proves that current accepted/historical P0-F materializes exactly **43 Application-to-Application families**, including Shared Web and Shared Communication edges that were not all represented in the initial SIA inventory.

This file therefore controls the **inventory completeness** of the SIA contract layer:

```text
ACCEPTED P0-F 43 FAMILIES = PRESERVED 43/43
UNEXPLAINED DROP = 0
UNEXPLAINED MERGE = 0
NEW APP-RSC / FSARM FAMILIES = ADDITIVE CANDIDATE EXTENSIONS
```

File 12 remains the detailed schema/route rule source for the families it specifies. This 12A file supplies the complete baseline reconciliation and the missing family specifications required before semantic freeze.

## 2. Canonical Accepted 43 Families Preserved

### Trading <-> FSAPMA: 3

1. `falcon.xapp.trading.core.trading.fsapma.data-requirement`
2. `falcon.xapp.trading.fsapma.trading.core.operational-data-product`
3. `falcon.xapp.trading.fsapma.trading.core.provider-service-status`

### Guardian <-> Trading: 4

4. `falcon.xapp.trading.guardian.trading.core.protection-command`
5. `falcon.xapp.trading.core.trading.guardian.safety-projection`
6. `falcon.xapp.trading.core.trading.guardian.protection-command-outcome`
7. `falcon.xapp.trading.guardian.trading.core.protection-release`

### Guardian <-> FSAPMA: 3

8. `falcon.xapp.trading.guardian.trading.fsapma.provider-protection-command`
9. `falcon.xapp.trading.fsapma.trading.guardian.provider-integrity-projection`
10. `falcon.xapp.trading.fsapma.trading.guardian.provider-protection-outcome`

### FSTSimA sibling flows: 7

11. `falcon.xapp.trading.core.validation.fstsima.validation-input`
12. `falcon.xapp.trading.guardian.validation.fstsima.validation-input`
13. `falcon.xapp.trading.fsapma.validation.fstsima.validation-input`
14. `falcon.xapp.trading.fsapma.validation.fstsima.nonlive-data-input`
15. `falcon.xapp.validation.fstsima.trading.core.validation-evidence`
16. `falcon.xapp.validation.fstsima.trading.guardian.validation-evidence`
17. `falcon.xapp.validation.fstsima.trading.fsapma.validation-evidence`

### Presentation projections -> Shared Web: 4

18. `falcon.xapp.trading.guardian.shared.web.presentation-projection`
19. `falcon.xapp.trading.fsapma.shared.web.presentation-projection`
20. `falcon.xapp.trading.core.shared.web.presentation-projection`
21. `falcon.xapp.validation.fstsima.shared.web.presentation-projection`

### Shared Web user-intent paths: 4

22. `falcon.xapp.shared.web.trading.guardian.user-intent`
23. `falcon.xapp.shared.web.trading.fsapma.user-intent`
24. `falcon.xapp.shared.web.trading.core.user-intent`
25. `falcon.xapp.shared.web.validation.fstsima.user-intent`

### Business outcomes -> Shared Web: 4

26. `falcon.xapp.trading.guardian.shared.web.user-intent-outcome`
27. `falcon.xapp.trading.fsapma.shared.web.user-intent-outcome`
28. `falcon.xapp.trading.core.shared.web.user-intent-outcome`
29. `falcon.xapp.validation.fstsima.shared.web.user-intent-outcome`

### Notification/report requests -> Shared Communication: 4

30. `falcon.xapp.trading.guardian.shared.communication.notification-request`
31. `falcon.xapp.trading.fsapma.shared.communication.notification-request`
32. `falcon.xapp.trading.core.shared.communication.notification-request`
33. `falcon.xapp.validation.fstsima.shared.communication.notification-request`

### Communication delivery outcomes: 4

34. `falcon.xapp.shared.communication.trading.guardian.delivery-outcome`
35. `falcon.xapp.shared.communication.trading.fsapma.delivery-outcome`
36. `falcon.xapp.shared.communication.trading.core.delivery-outcome`
37. `falcon.xapp.shared.communication.validation.fstsima.delivery-outcome`

### Communication recipient responses: 4

38. `falcon.xapp.shared.communication.trading.guardian.recipient-response`
39. `falcon.xapp.shared.communication.trading.fsapma.recipient-response`
40. `falcon.xapp.shared.communication.trading.core.recipient-response`
41. `falcon.xapp.shared.communication.validation.fstsima.recipient-response`

### Shared Web <-> Shared Communication: 2

42. `falcon.xapp.shared.web.shared.communication.recipient-response-intent`
43. `falcon.xapp.shared.communication.shared.web.communication-status-projection`

Count proof:

```text
3 + 4 + 3 + 7 + 4 + 4 + 4 + 4 + 4 + 4 + 2 = 43
```

## 3. Relationship Between Accepted IDs and SIA Short IDs

The short IDs in file 12 are implementation/document navigation aliases only. The accepted `falcon.xapp.*` identity remains the semantic baseline identity unless Owner-approved successor versioning explicitly changes it.

Examples:

```text
TRD-PMA-001
-> falcon.xapp.trading.core.trading.fsapma.data-requirement

PMA-TRD-001
-> falcon.xapp.trading.fsapma.trading.core.operational-data-product

GRD-TRD-001
-> falcon.xapp.trading.guardian.trading.core.protection-command

TRD-GRD-002
-> falcon.xapp.trading.core.trading.guardian.safety-projection

TRD-GRD-003
-> falcon.xapp.trading.core.trading.guardian.protection-command-outcome

GRD-TRD-002
-> falcon.xapp.trading.guardian.trading.core.protection-release
```

Before implementation, generated contract metadata SHALL contain both the canonical family identity and the SIA alias where an alias is used.

An alias collision or missing canonical identity is a verifier failure.

## 4. Core 1-17 Schema Mapping

The detailed payload rules in file 12 apply to the accepted baseline as follows:

- accepted #1 maps to `TRD-PMA-001` Data Product Demand Declaration;
- accepted #2 maps to `PMA-TRD-001` Normalized Data Product Delivery;
- accepted #3 is represented by PMA Trading quality/capability/route status projections and SHALL be published as **one compatible provider-service-status family with typed sub-state**, not silently split into unrelated authority families;
- accepted #4 maps to `GRD-TRD-001`;
- accepted #5 maps to `TRD-GRD-002`;
- accepted #6 maps to `TRD-GRD-003`;
- accepted #7 maps to `GRD-TRD-002`;
- accepted #8 maps to `GRD-PMA-001` plus exact release/supersession semantics under the same accepted family/version successor policy;
- accepted #9 maps to PMA Guardian provider/data protection observation + route projection schema;
- accepted #10 maps to provider protection effect/outcome schema and SHALL be explicitly added to generated metadata;
- accepted #11-17 map to the FSTSimA request/evidence schemas in file 12, with #14 specifically requiring explicit non-Live Data Product classification.

No accepted family is replaced by multiple new families in a way that changes bilateral declaration semantics without an explicit compatible schema/version mapping.

## 5. Missing Shared Web / Communication Schema Rules Added Here

Shared Web and Shared Communication are external to the core FSATS business Applications and are separately governed Applications. FSATS owns only the business contract meaning it publishes/consumes at these edges.

### 5.1 Presentation Projection #18-21

Common payload:

```text
ProjectionId
SourceApplicationId
ProjectionType
SubjectScope
PresentationState
DataClassification
EffectiveAt
ExpiresAt?
SourceStateVersionRefs[]
ReasonCodes[]
EvidenceRefs[]
```

Rules:

- read-only least-privilege projection;
- no secret/credential/private raw payload leakage;
- not an authority token;
- stale projection visibly marked/stops being current;
- Web cannot mutate source state by changing displayed values.

### 5.2 Web User Intent #22-25

Common payload:

```text
UserIntentId
AuthenticatedUserPrincipalRef
Session/InteractionRef
TargetApplicationId
IntentType
TargetScope
RequestedParameters
UserConfirmationEvidenceRef?  // required for consequence class by policy
ClientObservedStateVersionRef?
EffectiveAt
ExpiresAt
IdempotencyKey
```

Rules:

- `USER_INTENT` is not target business authorization;
- target Application independently validates user authority, current state and business rules;
- unknown/unregistered IntentType is denied;
- stale client state may require confirmation/rejection;
- replayed user intent is subject to idempotency and expiry;
- Web does not sign/forge Guardian/Trading business authority.

### 5.3 User Intent Outcome #26-29

```text
UserIntentId
OutcomeId
TargetApplicationId
OutcomeState = RECEIVED | ACCEPTED | REJECTED | APPLYING | COMPLETED | PARTIAL | FAILED | UNKNOWN
CurrentStateProjectionRef?
ReasonCodes[]
EvidenceRefs[]
```

Technical receipt is distinct from business acceptance/completion.

### 5.4 Notification Request #30-33

```text
NotificationRequestId
SourceApplicationId
BusinessEvent/SubjectRef
NotificationClass
RecipientSelectorRef
ContentTemplateId/Version
TemplateParameters
SensitivityClass
UrgencyClass
ExpiresAt?
IdempotencyKey
EvidenceRefs[]
```

Communication Application owns actual channel selection/delivery workflow under its contract. Source Application does not gain Communication internals.

### 5.5 Communication Delivery Outcome #34-37

```text
NotificationRequestId
DeliveryOutcomeId
ChannelClass
Outcome = QUEUED | SENT | DELIVERED | FAILED | EXPIRED | UNKNOWN
ProviderMessageRef?  // non-secret, policy-controlled
ObservedAt
ReasonCodes[]
EvidenceRefs[]
```

`SENT`/`DELIVERED` are Communication delivery semantics, not proof the recipient understood or acted.

### 5.6 Communication Recipient Response #38-41

```text
NotificationRequestId
RecipientResponseId
ResponseType
AuthenticatedRecipientRef when available/required
ResponsePayload
ReceivedAt
EvidenceRefs[]
```

The original source Application decides business meaning. Communication cannot turn a human reply into Trading/Guardian authority automatically.

### 5.7 Web -> Communication Recipient Response Intent #42

Carries authenticated user intent to respond to an exact Communication-owned interaction/workflow. Same user-intent security/idempotency rules apply.

### 5.8 Communication -> Web Status Projection #43

Read-only Communication-owned delivery/response status projection with least-privilege fields and explicit staleness.

## 6. Authority Class Preservation

The accepted P0-F authority classes remain controlling:

```text
INFORMATION_REQUEST
OWNER_TRUTH_PUBLICATION
DELEGATED_PROTECTION_COMMAND
NONAUTHORITATIVE_VALIDATION_EXCHANGE
USER_INTENT_FORWARDING
SHARED_SERVICE_REQUEST
BUSINESS_OUTCOME_RETURN
```

File 12 aliases/classes SHALL map to these without increasing authority.

In particular:

```text
USER_INTENT != BUSINESS_AUTHORITY
PROJECTION != COMMAND
DELIVERY_OUTCOME != BUSINESS_OUTCOME
VALIDATION_EVIDENCE != PROMOTION_AUTHORITY
```

## 7. Environment / Truth Separation

Every accepted family preserves explicit truth class:

```text
LIVE_OPERATIONAL
PAPER_OPERATIONAL
SHADOW
REPLAY
SIMULATION
TEST
RESEARCH
PRESENTATION_ONLY where applicable
```

A non-Live/presentation family cannot be routed into an operational command consumer merely because payload shapes are compatible.

## 8. Bilateral Declaration Requirement

All 43 accepted baseline families require compatible declaration by both participants.

For Web/Communication edges, the future/current separate Application workstream owns its own manifest side. FSATS SHALL declare only its exact expected edge and fail closed until the counterparty manifest/route is valid.

The absence of Shared Web or Shared Communication runtime implementation does not permit direct UI/email/SMS integrations inside Trading/Guardian/FSAPMA/FSTSimA.

## 9. New APP-RSC / FSARM Additive Families

If APP-RSC is Owner-accepted, the resource families defined in file 12 are **new additive contract families**, not replacements for any of the accepted 43.

Canonical candidate IDs:

### Constituent -> APP-RSC resource reports

44. `falcon.xapp.trading.core.resource.fsarm.resource-demand-report`
45. `falcon.xapp.trading.fsapma.resource.fsarm.resource-demand-report`
46. `falcon.xapp.trading.guardian.resource.fsarm.resource-demand-report`
47. `falcon.xapp.validation.fstsima.resource.fsarm.resource-demand-report`

### APP-RSC -> constituent coordination directives

48. `falcon.xapp.resource.fsarm.trading.core.resource-coordination-directive`
49. `falcon.xapp.resource.fsarm.trading.fsapma.resource-coordination-directive`
50. `falcon.xapp.resource.fsarm.trading.guardian.resource-coordination-directive`
51. `falcon.xapp.resource.fsarm.validation.fstsima.resource-coordination-directive`

### Constituent -> APP-RSC effect outcomes

52. `falcon.xapp.trading.core.resource.fsarm.resource-effect-outcome`
53. `falcon.xapp.trading.fsapma.resource.fsarm.resource-effect-outcome`
54. `falcon.xapp.trading.guardian.resource.fsarm.resource-effect-outcome`
55. `falcon.xapp.validation.fstsima.resource.fsarm.resource-effect-outcome`

### APP-RSC state projections

56. `falcon.xapp.resource.fsarm.trading.core.effective-resource-state`
57. `falcon.xapp.resource.fsarm.trading.fsapma.effective-resource-state`
58. `falcon.xapp.resource.fsarm.trading.guardian.effective-resource-state`
59. `falcon.xapp.resource.fsarm.validation.fstsima.effective-resource-state`

Candidate total if APP-RSC accepted:

```text
43 ACCEPTED BASELINE FAMILIES
+ 16 APP-RSC ADDITIVE FAMILIES
= 59 CURRENT SIA FAMILIES
```

The `RSC-ALL-001` convenience concept in file 12 SHALL materialize as four exact bilateral families #56-59. No wildcard FSATS consumer is allowed.

## 10. APP-RSC Contract Non-Grant

Families #44-59 remain candidate-only until:

1. APP-RSC topology delta passes A/C and fresh Red-Team;
2. Owner explicitly accepts APP-RSC/FSARM Application placement and exact contract additions;
3. required Foundation Manifest/route/resource capabilities are available and separately bound;
4. later implementation authority is granted.

The 43 existing families remain preserved regardless of whether APP-RSC is accepted.

## 11. Contract Count Invariants

Before semantic freeze:

```text
BASELINE_ACCEPTED_FAMILIES = 43 EXACT
BASELINE_UNEXPLAINED_DROP = 0
BASELINE_UNEXPLAINED_MERGE = 0
BASELINE_WILDCARD_PARTICIPANTS = 0
NEW_RESOURCE_FAMILIES = 16 CANDIDATE
TOTAL_IF_APP_RSC_ACCEPTED = 59
```

## 12. Negative Fixtures Added By This Reconciliation

Verifier SHALL reject:

- missing any canonical baseline family #1-43;
- replacing a Web/Communication edge with direct integration inside an FSATS Application;
- one-sided bilateral declaration;
- presentation projection treated as command;
- Web user intent treated as automatic Trading/Guardian authority;
- Communication SENT treated as recipient/business success;
- recipient response treated as source-Application action without source validation;
- wildcard `RSC-ALL` runtime route instead of four exact consumers;
- APP-RSC family appearing in an accepted baseline count before Owner acceptance;
- contract alias with no canonical `falcon.xapp.*` family identity;
- same canonical family mapped to incompatible multiple business meanings.

## 13. Reconciliation Result

```text
INITIAL SIA CONTRACT INVENTORY FINDING = REMEDIATED AT DESIGN-CANDIDATE LEVEL
ACCEPTED P0-F 43 FAMILY COVERAGE = 43/43
NEW APP-RSC FAMILY COUNT = 16 CANDIDATE
UNEXPLAINED LOSS OF ACCEPTED CONTRACTS = 0
```

This reconciliation itself remains a design candidate until the complete SIA package passes fresh review and Owner decision.
