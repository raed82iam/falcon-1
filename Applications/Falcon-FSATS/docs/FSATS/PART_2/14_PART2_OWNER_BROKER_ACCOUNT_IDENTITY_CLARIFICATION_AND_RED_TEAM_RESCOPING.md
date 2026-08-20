# FSATS Part 2 — Owner Broker-Account Identity Clarification and Focused Red-Team Rescoping

**Status:** `OWNER_CLARIFICATION_RECORDED / PRIOR_FOCUSED_RED_TEAM_RESCOPED / FINDINGS_REMAIN_OPEN`  
**Branch:** `application-development`  
**Authority:** Project Owner clarification dated 2026-08-15  
**Affected Review:** `13_PART2_MULTI_USER_MULTI_BROKER_MULTI_API_MULTI_EXECUTION_RED_TEAM.md`  
**Reviewed Executable Source:** `0d165ddd61d68cb8083daa90aca87cf809e3cba0`  
**Runtime Authority:** `NOT_GRANTED`  
**Part 3:** `NOT_AUTHORIZED / NOT_STARTED`

## 1. Owner Clarification

The Project Owner clarified that FSATS trading runtime does not own, model, or require customer/user identity.

```text
FSATS_USER_ID = NONE
FSATS_USERNAME = NONE
FSATS_CUSTOMER_ID = NONE
FSATS_CUSTOMER_ACCOUNT_OWNERSHIP_GRAPH = NONE
```

FSATS operates on broker accounts.

The operational account identifier is the unique account identifier supplied by the broker. Because Falcon is multi-broker, the safe governed identity is:

```text
BROKER_ACCOUNT_IDENTITY = BrokerId + BrokerAccountId
```

Where Paper/Live, sandbox/production, or another broker environment can materially alter meaning, identity shall additionally bind the relevant environment:

```text
BROKER_ACCOUNT_CONTEXT = BrokerId + BrokerAccountId + Environment
```

FSATS shall not care whether one human owns one account or ten accounts. Each broker account is an independent operating subject unless evidence proves a wider shared dependency or broker-wide incident.

## 2. Web Boundary

Shared Web owns customer/user/contact mapping.

```text
FSATS
  -> BrokerId + BrokerAccountId + incident/business semantics
  -> Shared Web
  -> Web resolves broker account to customer/user/contact
  -> Web performs customer-facing notification/interaction
```

FSATS shall not store or require the customer's username, user ID, customer ID, or account-to-person ownership relationship for trading execution, containment, reconciliation, or Guardian targeting.

This clarification was also posted prospectively to:

- FCR-0014 broker execution egress boundary;
- FCR-0095 Shared Web Guardian/account notification boundary.

## 3. Failure Scope and Escalation

Normal failure scope starts at the affected broker account.

```text
ACCOUNT_LOCAL_FAILURE
-> CONTAIN AFFECTED BROKER ACCOUNT
-> PRESERVE UNAFFECTED BROKER ACCOUNTS
```

If evidence shows multiple accounts under the same broker are affected, Falcon may widen the incident scope.

If evidence shows the broker service itself is unavailable:

```text
BROKER_ACCOUNT_FAILURES
-> SHARED BROKER DEPENDENCY CONFIRMED
-> BROKER-WIDE INCIDENT
-> AFFECTED BROKER ACCOUNTS REMAIN ATTRIBUTABLE
```

Falcon shall not infer broker-wide failure merely because more than one account reports a problem. Conversely, once broker-wide unavailability is established, Falcon shall not treat each account as an unrelated isolated incident.

The Web notification boundary follows the same scope: FSATS reports the broker/account facts; Web resolves and informs the affected customer(s).

## 4. Effect on Prior Focused Red-Team

The prior focused Red-Team remains immutable historical evidence of the review as originally performed. Its `user/principal` wording is superseded prospectively by this Owner clarification.

The correction does **not** clear the technical findings. It changes the governing identity axis from user-centric to broker-account-centric.

Current focused finding count remains:

```text
CRITICAL = 4
HIGH = 3
MEDIUM = 2
FOCUSED RED-TEAM = FAIL
PART 2 OWNER CLOSURE REVIEW = SUSPENDED
```

## 5. Rescoped Findings

### C-01 — Capital reservation must be broker-account scoped

Current reservation aggregation remains currency-global. The required isolation is no longer `User A vs User B`; it is:

```text
Broker A / Account 1 capital
!=
Broker A / Account 2 capital
!=
Broker B / Account X capital
```

Reservation admission, reservation uniqueness, available-capital truth and release must bind to the correct broker-account context.

### C-02 — Execution/reconciliation must bind exact broker-account context

The required identity is:

```text
BrokerId
+ BrokerAccountId
+ Environment where material
+ ExecutionRouteId
+ OrderId / Submission identity
```

No user/principal field is required. `OrderId` alone remains insufficient.

### C-03 — Guardian protection must target exact broker-account/execution scope

Guardian commands and outcomes must prove the exact affected broker account and, where applicable, route/order/position scope.

The requirement is not user targeting. The requirement is exact broker-account protection targeting.

### C-04 — Recovery must prove complete broker-account reconciliation

`Recovered` and permission to resume risk-increasing activity require complete reconciliation of the affected broker account scope. One confirmed observation is insufficient if other positions, orders, fills, protections, reservations, or prior ambiguous submissions remain unresolved.

### H-01 — FSAPMA provider identity remains independently scoped

Provider accounts are not customer users. FSAPMA must preserve provider/service account, credential, quota, environment and route identity for operational-data access where applicable.

### H-02 — Event identity/order namespaces must be account/execution scoped where business semantics require it

Replace prior `tenant/user` wording with the actual governed business namespace, normally broker account, broker route, provider route, or Application-global scope depending on event semantics.

### H-03 — Locality/shared-dependency proof must remain evidence-bound

Containment decisions must prove whether a failure is account-local, broker-account-set scoped, broker-wide, provider-wide, route-wide, or otherwise shared. Naked booleans remain insufficient evidence for high-consequence containment scope.

### M-01 — FSTSimA context scoping

Any broker/account/environment-specific qualification evidence must bind that exact context. No user identity is required.

### M-02 — Cross-dimensional adversarial regression

Required regression matrix is corrected to account-centric scenarios, including at minimum:

- two broker accounts with the same currency and independent capital;
- same ReservationId across independent broker-account namespaces;
- same OrderId across two broker accounts;
- one broker with multiple accounts;
- one Falcon deployment using multiple brokers simultaneously;
- one broker-account failure without affecting peer account;
- broker-wide outage affecting multiple accounts only after shared dependency is proven;
- route failover with unknown prior submission outcome;
- same EventId/OrderingKey across independent account/execution namespaces;
- Guardian action targeting one broker account while peers remain unaffected;
- Guardian route returning correct Application but wrong broker-account scope;
- reconnect with incomplete account reconciliation remaining non-recovered;
- provider account/API quota and credential isolation;
- concurrent execution/protection actions across distinct broker accounts.

## 6. Current Source Correction Required

The clarified Owner model also means existing source fields named `PrincipalId` in trading failure/recovery paths must not be interpreted as customer/user identity.

Where those fields currently model a separate trading-side principal beyond broker account identity, remediation must remove or redefine them so the implementation does not invent a customer/user identity layer inside FSATS.

No source change is performed by this clarification record itself.

## 7. Cross-Workstream Boundary

```text
TRADING / GUARDIAN BUSINESS TARGET = BROKER ACCOUNT
WEB CUSTOMER TARGETING = WEB OWNED
FOUNDATION GENERIC EGRESS / SECURITY = FOUNDATION OWNED
```

FSATS shall not become a customer identity service. Shared Web shall not become trading truth authority. Foundation shall not interpret customer/business ownership semantics.

## 8. Current Disposition

```text
OWNER ACCOUNT-IDENTITY CLARIFICATION = RECORDED
PRIOR USER-CENTRIC WORDING = SUPERSEDED PROSPECTIVELY
BROKER-ACCOUNT-CENTRIC MODEL = CONTROLLING

OPEN CRITICAL = 4
OPEN HIGH = 3
OPEN MEDIUM = 2
PART 2 OWNER CLOSURE REVIEW = NOT_ELIGIBLE / SUSPENDED
PART 2 OWNER CLOSURE = NOT_GRANTED

PART 3 = NOT_AUTHORIZED / NOT_STARTED
RUNTIME = NOT_AUTHORIZED
PROVIDER / BROKER CONNECTIVITY = NOT_AUTHORIZED
PAPER / SHADOW / TINY-LIVE / LIVE / DEPLOYMENT = NOT_AUTHORIZED
```

Any remediation must be followed by exact executable validation, fresh Architecture/Consistency review, fresh full Red-Team, and explicit Owner review before Part 2 may become closure-eligible again.
