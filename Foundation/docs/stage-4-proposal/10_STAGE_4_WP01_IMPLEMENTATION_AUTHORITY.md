# Stage 4 WP-01 Implementation Authority

## Work Package

```text
WP-01 — Default-Deny Authority Engine
```

## Current State

```text
STAGE4_PLANNING_REVIEW = PASS
WP01_IMPLEMENTATION_AUTHORITY = OWNER_CONFIRMATION_REQUIRED
WP02_THROUGH_WP06 = UNAUTHORIZED
```

## Governing Authority

- `../governance/GOV-103_STAGE_4_WP01_IMPLEMENTATION_AUTHORIZATION.md`

## Exact Scope

WP-01 may implement only:

- the bounded `Foundation.Authority` production project;
- the WP-01 verifier;
- solution membership;
- architecture and security validation required by the new project.

It may not integrate the decision into Lifecycle or any execution boundary. That belongs to WP-02.

## Core Rule

```text
Authority Decision ≠ Action Execution
```

The Authority Engine may return a decision and evidence identity. It may not perform the requested action or modify authoritative state.

## Required Exit

```text
WP01_BUILD = PASS
WP01_ARCHITECTURE = PASS
WP01_SECURITY = PASS
WP01_VERIFIER = PASS
WP01_DETERMINISTIC_REPLAY = PASS
STAGE3_REGRESSION = PASS
READY_FOR_WP01_INDEPENDENT_REVIEW
```
