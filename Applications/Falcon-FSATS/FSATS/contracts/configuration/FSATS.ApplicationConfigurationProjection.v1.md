# FSATS.ApplicationConfigurationProjection.v1

**Status:** `DECLARATION_ONLY / NO_RUNTIME_AUTHORITY`  
**Owner:** producing Falcon Application

## Purpose

Declare a bounded cross-boundary projection of Application-owned configuration identity and evaluation truth without creating a shared mutable configuration owner or exposing Application internals.

## Required Projection Fields

```text
ApplicationId
ConfigurationId
ConfigurationVersion
ConfigurationEpoch
ConfigurationDigest
EnvironmentIdentity (when material)
EvidenceId
EvidenceIntegrity
Compatibility
Condition
ReasonCode
PreservesAuthorityBoundary
PreservesEnvironmentBoundary
CanApplyByConfigurationOnly
RuntimeAuthorityGranted = false
Observed/ProducedAtUtc when projected operationally
```

Application-specific extensions remain producer-owned and must not transfer business ownership to the consumer.

## Invariants

```text
PROJECTION_CONSUMPTION != INTERNAL_CONFIG_ACCESS
PROJECTION_PRESENT != CONFIG_CURRENT
PROJECTION_VALID != AUTHORITY_GRANTED
CONFIG_PRESENT != ACTIVE
CONFIG_VALID != ADMITTED
FEATURE_ENABLED_IN_CONFIG != FEATURE_AUTHORIZED
POLICY_REFERENCE != POLICY_AUTHORITY
SECRET_REFERENCE != SECRET_BYTES
RuntimeAuthorityGranted = false
```

## Ownership

The producing Application remains authoritative for the business meaning of its configuration. A consumer may render, compare or gate on the projection only under separately governed contracts and authority.

This declaration does not create Foundation configuration authority, Shared Web implementation authority, runtime routes, external egress, Paper/Live authority, deployment authority, or Part 7 authority.
