# FSATS Part 2 — Broker-Account / Multi-Broker / Multi-API / Multi-Execution Remediation

**Status:** `REMEDIATION_IMPLEMENTED / EXECUTABLE_VALIDATION_PENDING`  
**Branch:** `application-development`  
**Authority:** Existing Project Owner Part 2 full implementation authorization + Owner broker-account identity clarification dated 2026-08-15  
**Historical Focused Red-Team:** `13_PART2_MULTI_USER_MULTI_BROKER_MULTI_API_MULTI_EXECUTION_RED_TEAM.md`  
**Controlling Identity Correction:** `14_PART2_OWNER_BROKER_ACCOUNT_IDENTITY_CLARIFICATION_AND_RED_TEAM_RESCOPING.md`  
**Part 2 Owner Closure:** `NOT_GRANTED`  
**Part 3:** `NOT_AUTHORIZED / NOT_STARTED`  
**Runtime / Provider / Broker / Paper / Live Authority:** `NOT_GRANTED`

## 1. Owner Identity Model Applied

The remediation implements the Owner clarification that FSATS does not own or operate on customer/user identity.

```text
FSATS_USER_ID = NONE
FSATS_CUSTOMER_ID = NONE
FSATS_USERNAME = NONE

TRADING OPERATING SUBJECT = BROKER ACCOUNT
BROKER ACCOUNT BUSINESS IDENTITY = BrokerId + BrokerAccountId
ENVIRONMENT = ADDITIONAL IDENTITY DIMENSION WHERE MATERIAL
WEB = OWNER OF BROKER-ACCOUNT -> CUSTOMER/CONTACT MAPPING
```

A human may own one or many broker accounts. That ownership graph is outside FSATS trading semantics.

Account-local failure remains account-local unless attributable evidence proves a larger shared dependency such as broker-wide, provider-wide, provider-account-wide or execution-route-wide failure.

## 2. C-01 — Broker-Account Capital Reservation Isolation

Implemented in:

- `applications/FSATS/src/Trading/Falcon.FSATS.Trading.Domain/TradingDomain.cs`
- `applications/FSATS/src/Trading/Falcon.FSATS.Trading.Application/TradingServices.cs`

Changes:

- introduced validated `BrokerAccountContext(BrokerId, BrokerAccountId, Environment)`;
- capital reservation key is now `BrokerAccountContext + ReservationId`;
- aggregate reservation arithmetic is limited to the same broker account and currency;
- the same ReservationId may exist independently in different broker-account namespaces;
- Trading decision preparation now requires exact broker-account context before capital reservation.

Adversarial regression covers concurrent aggregate admission, duplicate reservation identity inside one account, currency isolation, separate broker-account budgets, and same ReservationId across distinct broker accounts.

**Remediation disposition:** `IMPLEMENTED / EXECUTABLE_PROOF_PENDING`.

## 3. C-02 — Exact Execution and Reconciliation Identity

Implemented in:

- `applications/FSATS/src/Trading/Falcon.FSATS.Trading.Application/TradingServices.cs`

New execution identity binds:

```text
BrokerId
BrokerAccountId
Environment
ExecutionRouteId
SubmissionId
OrderId
```

`IBrokerExecutionPort.ReconcileAsync` no longer accepts `OrderId` alone. Submission and reconciliation outcomes must return the same exact `BrokerExecutionIdentity`; mismatches fail closed to `ReconciliationRequired`.

Cross-account identical OrderId is explicitly challenged by the new adversarial regression.

**Remediation disposition:** `IMPLEMENTED / EXECUTABLE_PROOF_PENDING`.

## 4. C-03 — Guardian Exact Protective Target and Outcome

Implemented in:

- `applications/FSATS/src/TradingGuardian/Falcon.FSATS.TradingGuardian.Contracts/ProtectionContracts.cs`
- `applications/FSATS/src/TradingGuardian/Falcon.FSATS.TradingGuardian.Application/GovernedProtectionRoute.cs`
- `applications/FSATS/src/TradingGuardian/Falcon.FSATS.TradingGuardian.Application/GuardianServices.cs`

Free-form target scope was replaced by typed `ProtectionTarget` with governed target kinds including:

```text
Application
Broker
BrokerAccount
ExecutionRoute
Order
Position
```

The command fingerprint binds the exact canonical target. A route outcome must return the exact requested target in addition to command/application/correlation identity. A correct Application with the wrong broker account now fails closed to `ReconciliationRequired`.

**Remediation disposition:** `IMPLEMENTED / EXECUTABLE_PROOF_PENDING`.

## 5. C-04 — Complete Broker-Account Recovery Proof

Implemented in:

- `applications/FSATS/src/Trading/Falcon.FSATS.Trading.Application/BrokerOutageRecovery.cs`

`Recovered` may now be produced only for the exact broker account when all required reconciliation dimensions are proven:

```text
balance and buying power
positions
working orders
fills / partial fills
protection orders
capital reservations
ambiguous prior submissions
```

A mismatched broker account or incomplete reconciliation remains `AwaitingBrokerReconciliation` and cannot resume risk-increasing action.

`UNKNOWN_SUBMISSION != SAFE_TO_RETRY` remains preserved.

**Remediation disposition:** `IMPLEMENTED / EXECUTABLE_PROOF_PENDING`.

## 6. H-01 — FSAPMA Provider-Route / Account / API Isolation

Implemented in:

- `applications/FSATS/src/FSAPMA/Falcon.FSATS.FSAPMA.Domain/ProviderDomain.cs`
- `applications/FSATS/src/FSAPMA/Falcon.FSATS.FSAPMA.Application/ProviderServices.cs`
- `applications/FSATS/src/FSAPMA/Falcon.FSATS.FSAPMA.Contracts/ProviderContracts.cs`
- `applications/FSATS/src/FSAPMA/Falcon.FSATS.FSAPMA.Application/GovernedOperationalDataDelivery.cs`

`ProviderRouteIdentity` now binds:

```text
ProviderId
ProviderAccountId
Environment
ServiceRole
CredentialReference
```

Quota is scoped by the exact provider route. Fetch requests/results must preserve the exact route identity. Operational data provenance carries the provider-account/environment/service-role/credential-reference identity and rejects incomplete or mismatched routes.

Secret bytes remain outside ordinary Application state; only governed credential references are carried.

**Remediation disposition:** `IMPLEMENTED / EXECUTABLE_PROOF_PENDING`.

## 7. H-02 — Scoped Event and Ordering Namespaces

Implemented in Trading, FSAPMA and Trading Guardian `GovernedEventIngress` implementations.

Event identity and ordering state now use:

```text
ScopeKey + EventId
ScopeKey + OrderingKey
```

Helpers establish broker-account, execution-route and provider-route scopes where applicable. Equivalent raw EventId/OrderingKey values in independent broker accounts no longer share one namespace.

**Remediation disposition:** `IMPLEMENTED / EXECUTABLE_PROOF_PENDING`.

## 8. H-03 — Evidence-Bound Failure Locality

Implemented in:

- `applications/FSATS/src/Trading/Falcon.FSATS.Trading.Domain/OperationalFailureContainment.cs`

Naked caller booleans were replaced by `FailureLocalityEvidence` containing an evidence reference, observed time, explicit blast-radius classification, affected account set where applicable, and shared-dependency identity where applicable.

Supported proven blast radii include account-local, explicit account set, broker-wide, provider-account-wide, provider-wide, execution-route-wide and unknown.

Policy behavior:

```text
PROVEN ACCOUNT-LOCAL -> contain exact account
PROVEN BROKER-WIDE -> affect applicable accounts of same broker/environment
UNKNOWN -> expand containment
```

**Remediation disposition:** `IMPLEMENTED / EXECUTABLE_PROOF_PENDING`.

## 9. M-01 — FSTSimA Evidence Scope

Implemented in:

- `applications/FSATS/src/FSTSimA/Falcon.FSATS.FSTSimA.Contracts/SimulationContracts.cs`
- `applications/FSATS/src/FSTSimA/Falcon.FSATS.FSTSimA.Application/SimulationServices.cs`

Simulation qualification evidence can now bind broker-account/environment or provider-account context when the scenario is context-specific. Evidence identity includes the canonical simulation scope so identical scenario/seed values in independent contexts do not collide.

Simulation truth remains non-operational and creates no Paper/Live authority.

**Remediation disposition:** `IMPLEMENTED / EXECUTABLE_PROOF_PENDING`.

## 10. M-02 — Cross-Dimensional Adversarial Regression

Created:

- `applications/FSATS/tests/Behavior/Falcon.FSATS.Behavior.Verifier/BrokerAccountIsolationAdversarialChecks.cs`

Updated existing adversarial suites and the canonical Behavior verifier runner.

Coverage now includes:

- independent capital budgets for separate broker accounts;
- same ReservationId across separate broker accounts;
- same OrderId across separate broker accounts;
- cross-account reconciliation identity attack;
- Guardian outcome reporting the wrong broker account;
- provider-account quota isolation under the same ProviderId;
- wrong provider-route result identity;
- same EventId and OrderingKey in separate account scopes;
- incomplete and wrong-account broker reconciliation;
- account-local versus broker-wide versus unknown failure blast radius;
- simulation evidence collision across separate broker-account scopes.

The Operational Data Outcome verifier was also updated to supply complete provider-route provenance under the hardened contract.

**Remediation disposition:** `IMPLEMENTED / EXECUTABLE_PROOF_PENDING`.

## 11. Cross-Workstream Synchronization

Current FCR semantics were synchronized without changing ownership:

- FCR-0014 remains `Waiting On: FOUNDATION` for future governed broker egress and now explicitly preserves account-centric execution identity;
- FCR-0013 remains `Waiting On: FOUNDATION` for future governed provider egress and now explicitly preserves provider-route/account identity;
- FCR-0095 remains `Waiting On: WEB`; Web owns broker-account-to-customer/contact mapping and customer interaction.

No Foundation or Shared Web source file was modified.

## 12. Validation State

GitHub Actions cannot currently provide executable evidence because recent workflow runs fail before job startup with an account billing/spending-limit message. This is infrastructure unavailability, not a Falcon build/test result.

Therefore the truthful current state is:

```text
SOURCE REMEDIATION = IMPLEMENTED
STATIC CONSISTENCY SWEEP = PERFORMED
EXECUTABLE RESTORE/BUILD = PENDING LOCAL EXACT-COMMIT VALIDATION
DIRECT BEHAVIOR = PENDING
GOVERNED VERIFIERS = PENDING
FRESH ARCHITECTURE / CONSISTENCY = PENDING EXECUTABLE PROOF
FRESH POST-REMEDIATION RED-TEAM = PENDING EXECUTABLE PROOF
OWNER CLOSURE REVIEW = NOT_ELIGIBLE YET
```

No PASS shall be inferred from source changes alone.

## 13. Non-Grant

```text
PART 3 = NOT_AUTHORIZED / NOT_STARTED
RUNTIME = NOT_AUTHORIZED
PROVIDER / BROKER CONNECTIVITY = NOT_AUTHORIZED
PAPER / SHADOW / TINY-LIVE / LIVE / DEPLOYMENT = NOT_AUTHORIZED
```

This remediation changes only the authorized Part 2 Application source/test/documentary scope and does not activate external connectivity or later work.