# FSATS Application Runtime Readiness Projection v1

**Status:** `PART_7 DECLARATION-ONLY CONTRACT`  
**Runtime Route:** `NOT MATERIALIZED / NOT AUTHORIZED`

## Purpose

Defines a versioned declaration-only projection for a future governed consumer to understand one independent FSATS Application's Application-owned pre-runtime readiness result without mistaking that result for Foundation admission, activation, release, reintroduction or runtime authority.

## Contract Identity

```text
FSATS.ApplicationRuntimeReadinessProjection.v1
```

## Required Fields

```text
projectionId
applicationId
evaluationId
environment
evaluatedAt
localReadinessPassed
externalGatesSatisfied
eligibleForAdmissionReview
readyForExternalReleaseReview
condition
reasonCode
configurationEvidenceReference
healthEvidenceReference
recoveryEvidenceReference
dependencyEvidenceReferences[]
permissionEvidenceReferences[]
routeEvidenceReferences[]
externalHoldCodes[]
grantsRuntimeAuthority
```

`grantsRuntimeAuthority` MUST be `false` for every Part 7 projection.

## Application-Specific Scope Extensions

Trading SHALL bind readiness to exact `BrokerId + BrokerAccountId + Environment` and SHALL NOT carry FSATS customer/user identity.

FSAPMA SHALL bind provider-route readiness to exact `ProviderId + ProviderAccountId + Environment + ServiceRole + ApiInstanceId + EndpointId + CredentialReference`.

Trading Guardian SHALL bind protection readiness to exact protection target/environment and preserve containment/reconciliation truth.

APP-RSC SHALL bind resource readiness to current coordination epoch and externally attributable Foundation envelope/reference without minting Foundation grant/total-resource truth.

FSTSimA SHALL declare exact non-Live execution class. Paper/Live are not eligible through this contract.

## Mandatory Semantics

```text
ELIGIBLE_FOR_ADMISSION_REVIEW != ADMITTED
ELIGIBLE_FOR_ADMISSION_REVIEW != ACTIVE
LOCAL_READINESS_PASSED != EXTERNAL_GATES_SATISFIED
READY_FOR_EXTERNAL_RELEASE_REVIEW != RELEASED
REPAIR_SUCCESS != RELEASE
RESTART != RECOVERY
ROUTE_DECLARED != ROUTE_AUTHORIZED
PERMISSION_REQUESTED != PERMISSION_GRANTED
CREDENTIAL_REFERENCE != CREDENTIAL_AUTHORITY
PROJECTION != RUNTIME_AUTHORITY
```

## External Holds

An unresolved Foundation/FCR/runtime dependency SHALL appear as an explicit external hold. This projection cannot clear an FCR, create a Foundation capability, activate a route or consume secret bytes.

## Ownership

The producing Application owns its local readiness truth. Foundation remains owner of admission/activation/Lifecycle and generic release/reintroduction execution. Shared Web, if it later consumes this projection, is presentation/request surface only and does not recompute readiness.
