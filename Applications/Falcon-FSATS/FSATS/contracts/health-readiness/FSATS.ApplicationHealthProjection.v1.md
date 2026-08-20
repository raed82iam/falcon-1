# FSATS.ApplicationHealthProjection.v1

**Status:** `PART5_DECLARATION_ONLY / NO_RUNTIME_ROUTE_AUTHORITY`  
**Owner:** each producing FSATS Application for its own business meaning  
**Scope:** bounded operational health/readiness projection declaration

## Purpose

Declare a common cross-boundary shape for communicating Application-owned operational health/readiness truth without creating a shared runtime health owner, hidden cross-Application coupling, Foundation lifecycle authority, or customer/user identity ownership.

## Producer Rule

Each producing Application owns the meaning of its own projection:

```text
FSATS-TRADING
FSATS-FSAPMA
FSATS-TRADING-GUARDIAN
FSATS-FSTSIMA
APP-RSC
```

FSATS itself is not a producer and is not a mutable health authority.

## Required Projection Fields

A projection SHALL include at minimum:

```text
ContractId = FSATS.ApplicationHealthProjection.v1
ApplicationId
ProjectionId
ObservedAtUtc
ValidUntilUtc
EvidenceId
EvidenceIntegrityState
Condition
ReasonCode
CurrentEvidence
RequiresReconciliation
DegradationState
RuntimeAuthorityGranted = false
```

Application-specific bounded subject identity SHALL be included where material.

For Trading:

```text
BrokerId
BrokerAccountId
Environment
```

Trading SHALL NOT add `UserId`, `Username`, `CustomerId`, customer name, contact identity, or broker-account-to-customer ownership mapping.

For FSAPMA, provider/provider-account/service-role/environment identity may be included. Secret bytes SHALL NOT be included.

For Trading Guardian, exact protected target and incident/correlation identity may be included.

For APP-RSC, current coordinator epoch and governed Foundation envelope/reference identity may be included, but the projection SHALL NOT claim or mint Foundation resource authority.

For FSTSimA, run/evidence classification and qualification state may be included while preserving replay/synthetic/operational distinctions.

## Canonical Conditions

The semantic vocabulary is:

```text
HEALTHY
DEGRADED_SAFE
RECONCILIATION_REQUIRED
CONTAINED
NOT_READY
UNKNOWN
```

Application-local code MAY use typed local enums, but any serialized projection shall map exactly to this vocabulary or a future separately versioned successor.

## Freshness and Integrity

```text
PROJECTION_PRESENT != CURRENT
LAST_KNOWN != CURRENT
NO_SIGNAL != HEALTHY
```

A consumer SHALL evaluate `ObservedAtUtc`, `ValidUntilUtc`, and evidence integrity before treating the projection as current.

Unknown, expired, future-dated, malformed, or integrity-failed evidence SHALL NOT be upgraded to `HEALTHY` by a consumer.

## Authority Boundary

```text
HEALTHY != AUTHORIZED
READY != ACTIVE
APPLICATION_HEALTH_PROJECTION != FOUNDATION_HEALTH
APPLICATION_HEALTH_PROJECTION != FOUNDATION_LIFECYCLE_DECISION
PROJECTION != RUNTIME_ROUTE_AUTHORITY
PROJECTION != OWNER_APPROVAL
```

`RuntimeAuthorityGranted` is fixed to `false` for this contract version.

No consumer may infer admission, activation, Foundation release, provider/broker connectivity, Paper/Live authority, deployment, or later-Part authority from this projection.

## Coupling Boundary

Consumers receive the projection only. They SHALL NOT obtain direct access to the producer's database, files, memory, components, credentials, or internal health evaluator.

```text
PROJECTION_CONSUMPTION != INTERNAL_STATE_ACCESS
```

## Shared Web Boundary

This contract may be consumed by Shared Web only through separately governed Web-owned implementation. This Part 5 declaration does not modify `applications/shared/web/**` and does not grant Web runtime/deployment authority.

Shared Web remains owner of customer/user/contact presentation and broker-account-to-customer mapping.

## Versioning

Breaking semantic or field changes require a new contract version and governed compatibility review. Contract presence or schema validity does not itself create a route, consumer admission, authority, activation, or production approval.
