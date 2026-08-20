# FSATS Part 2 — Focused Multi-User / Multi-Broker / Multi-API / Multi-Execution Red-Team

**Status:** `RED_TEAM_COMPLETE / MATERIAL_FINDINGS_OPEN / OWNER_CLOSURE_ELIGIBILITY_SUSPENDED`  
**Reviewed Executable Source:** `0d165ddd61d68cb8083daa90aca87cf809e3cba0`  
**Current Documentary Branch:** `application-development`  
**Scope:** all five FSATS Applications, with deepest attack coverage on Trading, FSAPMA and Trading Guardian execution/data/protection boundaries and supporting FSTSimA / APP-RSC interactions  
**Runtime Authority:** `NOT_GRANTED`  
**Part 3:** `NOT_AUTHORIZED / NOT_STARTED`

## 1. Owner-Directed Attack Theme

This focused cycle reopens the Part 2 closure review specifically to challenge whether the current implementation remains safe and attributable when Falcon moves from a simple single-user/single-broker/single-API execution context to simultaneous:

- multiple users/principals;
- multiple Falcon/user accounts;
- multiple brokers;
- multiple broker accounts;
- multiple operational provider APIs and provider accounts;
- multiple environments/markets;
- multiple execution routes;
- concurrent orders, reservations, reconciliation and protection actions.

The controlling invariant is:

```text
FAILURE_OF_USER_A != FAILURE_OF_USER_B
USER_A_CAPITAL != USER_B_CAPITAL
BROKER_A_TRUTH != BROKER_B_TRUTH
BROKER_ACCOUNT_A_TRUTH != BROKER_ACCOUNT_B_TRUTH
PROVIDER_ACCOUNT_A != PROVIDER_ACCOUNT_B
EXECUTION_ROUTE_A != EXECUTION_ROUTE_B
ORDER_ID_ALONE != EXECUTION_IDENTITY
PROTECTION_TARGET_APPLICATION != EXACT_PROTECTION_TARGET_SCOPE
UNKNOWN != SAFE_TO_RETRY
RECONNECTED != RECONCILED
```

## 2. Final Finding Count

```text
CRITICAL = 4
HIGH = 3
MEDIUM = 2

FOCUSED RED-TEAM = FAIL
PART 2 OWNER CLOSURE ELIGIBILITY = SUSPENDED
```

Previous Part 2 executable and Red-Team PASS evidence remains valid historical evidence for the scopes it actually tested. This new focused adversarial question exposes additional multi-dimensional identity/isolation gaps and therefore controls current closure readiness until resolved.

---

# CRITICAL FINDINGS

## C-01 — Capital reservation namespace collapses users/accounts into one currency-global ledger

### Evidence

`CapitalReservationLedger` stores reservations in one `Dictionary<ReservationId, Money>` and calculates aggregate exposure by currency only. `RiskRequest`, `TradingDecisionPipeline.Prepare` and reservation admission carry no PrincipalId, AccountId, Environment, Market, BrokerId, BrokerAccountId or ExecutionRouteId.

### Attack

User A and User B each have an independent USD capital/loss budget of 10.

1. User A reserves USD 8.
2. User B independently attempts USD 8 against User B's own USD 10 budget.
3. A shared ledger observes User A's USD 8 as if it belonged to the same capital namespace and can reject User B.
4. A cross-user reuse of the same ReservationId also collides globally.

The inverse wiring risk is worse: if an account-specific `availableCapital` is supplied to a ledger containing reservations from other capital owners, admission is decided against a mixed accounting population that cannot prove ownership.

### Impact

- cross-user starvation;
- cross-account capital coupling;
- incorrect capital admission/denial;
- inability to reconstruct which capital owner a reservation belongs to;
- violation of minimum-necessary isolation and constitutional cumulative-risk accounting.

### Required property

Capital reservation identity must be bound to the actual capital owner and execution context, at minimum the governed composite identity required by the accepted multi-user model. Reservation uniqueness and aggregation must be scoped intentionally, not globally by Currency and ReservationId alone.

---

## C-02 — Execution and reconciliation lack exact multi-user / multi-broker / multi-route identity

### Evidence

`OrderIntent` contains only OrderId, Instrument, Quantity, TrustEpoch and PositionSafetyEnvelope. `IBrokerExecutionPort.ReconcileAsync` accepts only `OrderId`. `ExecutionCoordinator` owns a single `_broker` port and neither submission nor reconciliation carries PrincipalId, AccountId, Environment, Market, BrokerId, BrokerAccountId or ExecutionRouteId.

### Attack

Two independent broker/account contexts contain the same locally meaningful OrderId, or an adapter multiplexes multiple broker accounts behind one port.

An ambiguous submission on User A / Broker A / Account A reaches reconciliation using only OrderId. The interface cannot prove that the returned broker snapshot belongs to the same user, broker account and execution route that originated the submission.

### Impact

- wrong-account reconciliation;
- wrong-broker order truth association;
- inability to safely support simultaneous broker sessions;
- duplicate or contradictory execution truth under route failover;
- capital risk if a wrong snapshot is treated as the order being reconciled.

### Required property

Every execution submission and reconciliation must carry and bind exact principal/account/environment/market/broker/broker-account/execution-route identity plus an attributable submission/idempotency identity. `OrderId` alone is insufficient.

---

## C-03 — Guardian protection command cannot cryptographically/structurally prove exact user/account/broker/route target

### Evidence

`ProtectionCommand` contains `TargetApplication` plus a free-form `TargetScope` string, but no typed PrincipalId, AccountId, BrokerId, BrokerAccountId or ExecutionRouteId. The governed route fingerprint includes TargetScope, but route-outcome binding validates only CommandId, TargetApplication and CorrelationId. `ProtectionCommandOutcome` does not carry TargetScope.

### Attack

A valid Guardian command intended for User A / Broker Account A is dispatched to Trading. A defective or multiplexed downstream route applies the protection to another scope within the same Trading Application but returns the expected CommandId, TargetApplication and CorrelationId.

The current dispatcher can accept that outcome because the outcome does not prove the exact protection scope actually affected.

### Impact

- User A may remain exposed while User B is frozen/closed;
- minimum necessary containment cannot be proven end-to-end;
- protective success can be attributed to the wrong account/route;
- high-consequence Guardian action becomes Application-targeted rather than exact-risk-targeted.

### Required property

Protection target and outcome identity must bind the exact affected principal/account/broker/broker-account/route/position-or-order scope as applicable. A free-form scope string plus Application identity is insufficient for multi-user protective authority.

---

## C-04 — Broker recovery can declare `Recovered` from an incomplete reconciliation proof

### Evidence

`BrokerOutageRecoveryPolicy.Assess` returns `OperationalRecoveryState.Recovered` and `MayResumeRiskIncreasingAction=true` when:

- connectivity is Available;
- `BrokerSubmissionTruth == Reconciled`;
- one `BrokerAccountObservation` is identity-complete and BrokerApiConfirmed.

The model does not structurally require a complete reconciled account set covering balance/cash, all positions, all open/working orders, fills, protections, reservations and every ambiguous prior submission before risk-increasing resume.

### Attack

After an outage, one position observation is broker-confirmed and the caller labels the submission state `Reconciled`, while another orphan working order or unresolved submission still exists on the same broker account.

The current API can return `Recovered` even though complete broker-account truth has not been demonstrated.

### Impact

- new risk may resume while hidden exposure remains;
- duplicate close/order risk;
- orphan protection/order risk;
- inconsistent capital reservations versus real broker state;
- multi-order/multi-execution recovery can be falsely compressed into one boolean-like enum transition.

### Required property

Recovery must require a structurally complete, attributable reconciliation package for the exact user/broker account and all affected execution identities. `Reconciled` must be proven, not merely supplied as an enum value.

---

# HIGH FINDINGS

## H-01 — FSAPMA provider-account / API / credential / environment identity is lost before fetch

### Evidence

FSAPMA defines ProviderAccountId, EntitlementState and CredentialReference, but `QuotaLedger` is keyed only by ProviderId. `IProviderEgressPort.FetchAsync` accepts only ProviderId + DataProductId. `ProviderDataCoordinator.FetchAsync` has no ProviderAccountId, credential reference, environment, market, instrument or endpoint/service-role identity.

`OperationalDataProjection` later preserves ProviderId/instrument/product/provenance, but it still has no ProviderAccountId/environment/credential/API endpoint identity, so downstream provenance cannot reconstruct what the upstream fetch boundary never carried.

### Attack

The same provider has multiple API accounts, credentials, environments or service roles with different quotas/entitlements. Calls can consume a shared ProviderId quota and the egress interface cannot prove which exact provider account/credential/environment supplied the value.

### Impact

- quota theft/starvation across provider accounts;
- wrong entitlement/credential use;
- paper/live or environment confusion risk;
- data provenance cannot prove exact API source identity;
- one provider account failure can poison unrelated accounts behind the same ProviderId.

### Required property

Operational provider selection, quota, entitlement, fetch and provenance must use a governed composite provider-route identity, not ProviderId alone.

---

## H-02 — Governed event duplicate/order namespaces are global rather than tenant/execution scoped

### Evidence

Trading, FSAPMA and Trading Guardian event ingress instances key accepted events by raw `EventId` and ordering state by raw `OrderingKey`. Their event envelopes do not carry PrincipalId, AccountId, BrokerId, BrokerAccountId, ProviderAccountId, Environment or ExecutionRouteId.

### Attack

Two users/accounts legitimately emit the same EventId or ordering key naming convention. User A's accepted event can cause User B's event to be interpreted as duplicate/conflict, or User A's sequence number can advance a shared ordering key and make User B's otherwise valid event fail as non-monotonic.

### Impact

- cross-tenant event suppression;
- cross-broker/provider ordering contamination;
- false duplicate/conflict classification;
- one user's traffic can cause another user's governed event stream to fail closed.

### Required property

Event identity and ordering namespaces must be explicitly scoped to the governed business/execution principal for events whose semantics are user/account/provider/broker/route specific.

---

## H-03 — Failure containment locality is accepted as caller-supplied booleans without bound proof

### Evidence

`OperationalFailureContainmentPolicy.Decide` receives `bool localityProven`; `ShouldAffectPeer` receives both `localityProven` and `sharedDependencyProven`. The policy correctly expands when locality is false, but the API itself does not require attributable evidence proving either assertion.

### Attack

A caller incorrectly supplies `localityProven=true` for a broker/API/route incident whose dependency blast radius is actually unknown. The policy can choose scoped containment because it trusts the boolean.

### Impact

- under-containment of shared broker/provider/API failure;
- User B may continue risk under a dependency actually shared with failed User A;
- the code expresses the right rule but does not make the proof mandatory.

### Required property

Locality/shared-dependency conclusions must be derived from or bound to attributable dependency/identity evidence, not naked caller assertions.

---

# MEDIUM FINDINGS

## M-01 — FSTSimA scenario/evidence identity is not multi-tenant or environment scoped

`SimulationRequest` and SimulationCoordinator evidence identity use ScenarioId/seed/requesting Application/purpose/classification, but no user/account/environment/market/broker/provider execution context. Current runtime isolation/egress remains gated, so this is not a current Live execution vulnerability. It is nevertheless a future multi-user qualification/evidence collision risk if parallel user/broker-specific simulations use reused scenario identifiers or evidence is later consumed for user-specific readiness.

Required: bind simulation/qualification evidence to the exact scenario ownership/context where user/account/broker/provider-specific evidence matters, while preserving simulation truth as non-operational.

---

## M-02 — Current executable verifier PASS does not prove cross-user / cross-broker / cross-provider-account / cross-route isolation

The current Behavior and Integration suites prove deterministic core behavior, concurrency of several internal primitives, Application topology and contract-family counts. The focused attacks above use distinct users/accounts/brokers/provider accounts/routes with colliding OrderId, ReservationId, EventId, OrderingKey and ambiguous reconciliation states. Equivalent cross-dimensional adversarial fixtures are not established by the existing `42/42`, `31/31`, `12/12` evidence.

Required future regression coverage must include at least:

- two users, same currency, independent budgets;
- same ReservationId across two owner namespaces;
- same OrderId across two broker accounts;
- one user with two brokers simultaneously;
- one broker with multiple accounts;
- route failover where prior submission outcome is unknown;
- two provider accounts under one ProviderId with separate quota/credentials;
- provider account failure without poisoning peer provider account;
- same EventId/OrderingKey across independent tenants;
- Guardian action targeted to one user/account while peer remains unaffected;
- Guardian route returning correct Application but wrong exact target scope;
- reconnect with incomplete all-orders/all-positions reconciliation must remain non-recovered;
- concurrent execution and protection actions across distinct accounts.

---

## 3. Cross-Application Disposition

### Falcon Self-Aware Trading Application

`FAIL` for focused multi-user/multi-broker/multi-execution readiness because capital reservation, order intent and reconciliation identity do not maintain the accepted composite scope end-to-end.

### FSAPMA

`FAIL` for focused multi-API/provider-account readiness because account/credential/environment identity exists conceptually but is collapsed at quota/fetch interfaces and absent from operational projection identity.

### Falcon Trading Guardian Application

`FAIL` for focused multi-user protective targeting because exact principal/account/broker/route scope is not structurally carried and the route outcome cannot prove the exact target scope applied.

### FSTSimA

`CONDITIONALLY CONFORMANT FOR CURRENT NON-RUNTIME SCOPE / MEDIUM FUTURE MULTI-TENANT EVIDENCE GAP`.

### APP-RSC

No Critical/High multi-user trading-identity defect was found in the reviewed resource-coordination primitives. APP-RSC intentionally coordinates at independent Application/resource-class level rather than owning user/broker business state. It must not become a workaround for missing Trading/FSAPMA/Guardian identity isolation.

---

## 4. FCR / Foundation Boundary

This Red-Team does not authorize Application implementation of missing Foundation runtime egress.

Relevant current external holds remain:

- FCR-0013 — operational provider egress / credential-reference boundary: `Waiting On: FOUNDATION`;
- FCR-0014 — broker execution egress / credential-reference boundary: `Waiting On: FOUNDATION`;
- FCR-0030 and other Foundation-owned future bindings remain Foundation-owned;
- FCR-0095 remains Web-owned where applicable.

The Application must nevertheless define and verify its own exact business/execution identity semantics before those future Foundation routes can be safely consumed. Foundation egress cannot repair an Application order/reservation/protection identity that was already collapsed before the boundary.

---

## 5. Closure Consequence

The earlier Part 2 final review record remains historical evidence and is not rewritten.

This new Owner-directed focused adversarial cycle changes current closure readiness:

```text
PREVIOUS PART2 FINAL RED-TEAM PASS = HISTORICAL FOR PRIOR REVIEW SCOPE
NEW FOCUSED MULTI-* RED-TEAM = FAIL
OPEN CRITICAL = 4
OPEN HIGH = 3
OPEN MEDIUM = 2
PART2 OWNER CLOSURE REVIEW ELIGIBILITY = SUSPENDED
PART2 OWNER CLOSURE = NOT_GRANTED
PART3 = NOT_AUTHORIZED / NOT_STARTED
RUNTIME / PROVIDER / BROKER / PAPER / LIVE = NOT_AUTHORIZED
```

No production/business source was modified by this review.

Required next governed sequence if the Project Owner directs remediation:

```text
C-01 capital identity isolation
-> C-02 execution/reconciliation composite identity
-> C-03 Guardian exact protective target/outcome identity
-> C-04 complete broker-account reconciliation proof
-> H-01 FSAPMA provider-route/account/API identity
-> H-02 tenant-scoped event identity/order namespaces
-> H-03 evidence-bound containment locality
-> M-01 simulation evidence scoping
-> M-02 cross-dimensional adversarial regression suite
-> exact executable validation
-> fresh Architecture / Consistency
-> fresh full Red-Team including this focused matrix
-> Owner closure review
```
