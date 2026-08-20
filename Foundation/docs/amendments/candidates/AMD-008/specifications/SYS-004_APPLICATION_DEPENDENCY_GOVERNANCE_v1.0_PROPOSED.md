# SYS-004 — Application Dependency Governance

**Identifier:** SYS-004  
**Version:** 1.0  
**Status:** APPROVED PENDING COORDINATED ACTIVATION  
**Approval Record:** GOV-063  
**Activation:** Not Authorized

## Purpose

Govern dependencies required by Falcon Foundation and admitted Applications without acquiring Application business responsibility.

## Requirements

Foundation Dependency Governance SHALL maintain:

- dependency identity, owner, kind, source, version, and integrity;
- declaring consumer and declared purpose;
- compatibility range and resolved version;
- required, optional, conditional, and prohibited relationships;
- startup, shutdown, update, recovery, and removal ordering;
- health, availability, failure, timeout, and degraded-state policy;
- isolation boundary and failure-propagation limit;
- replacement, migration, rollback, and evidence requirements.

Applications SHALL declare all Foundation, external, and inter-Application dependencies through their Application Contract. Hidden dependencies SHALL be rejected.

Foundation SHALL determine technical compatibility, admission impact, lifecycle order, resource impact, and containment requirements. It SHALL NOT determine how an Application interprets business outcomes produced by a dependency.

## Failure Policy

A dependency failure SHALL:

- be detected and attributed;
- expose affected consumers without disclosing unrelated Application internals;
- prevent unsafe activation or continued authority;
- contain propagation across Application boundaries;
- invoke declared degraded, suspension, isolation, recovery, or removal behavior;
- preserve evidence and uncertainty;
- avoid assuming successful recovery until independently verified.

Circular mandatory dependencies, unresolved version conflicts, unknown dependency identity, or unavailable required containment SHALL block activation.

## Acceptance

Validation SHALL cover incompatible versions, missing dependencies, circular dependencies, failure propagation, degraded operation, ordered recovery, update rollback, Application removal, and coexistence of independent Applications.
