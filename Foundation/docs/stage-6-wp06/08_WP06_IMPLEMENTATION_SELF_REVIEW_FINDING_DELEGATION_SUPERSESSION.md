# Stage 6 WP-06 — Implementation Self-Review Finding

**Status:** OPEN / REMEDIATION REQUIRED BEFORE VERIFIER COMPLETION  
**Severity:** HIGH  
**Date:** 2026-08-10

## Finding

The first WP-06 production implementation commit introduced bounded request authority lifetime, request replay protection, coordinator fencing generation/token and split-brain rejection, but it does not yet represent **request/delegation supersession generation independently from coordinator fencing**.

This is insufficient for the accepted WP-06 v0.2 requirement that stale/superseded delegation be rejected even when the requester is not an aggregate coordinator, and even when coordinator fencing itself is otherwise valid.

## Why this matters

Coordinator fencing and delegation supersession are related but not identical controls:

- fencing prevents stale/conflicting coordinator instances from acting concurrently;
- delegation supersession determines which requester authority generation is currently valid.

A direct Application requester also requires stale delegated authority rejection, so coordinator fencing cannot substitute for generic delegation supersession.

## Required remediation

Before dedicated WP-06 verifier completion:

1. add an explicit monotonically ordered authority/delegation generation to the generic request authority binding;
2. make it deterministic request identity material;
3. make the processor track the highest accepted authority generation for the exact requester-role/scope key;
4. reject a lower generation as stale/superseded;
5. reject conflicting authority identity at the same generation;
6. verify both direct-Application and aggregate-coordinator supersession behavior;
7. preserve coordinator fencing as a separate control.

## Scope classification

This remediation is inside the already authorized WP-06 request/decision boundary.

It does not require changing WP-01 through WP-05 production behavior and does not authorize WP-07/WP-08.

## Current state

`WP06_IMPLEMENTATION = IN_PROGRESS`

`WP06_PRODUCTION_FIRST_SLICE = WRITTEN`

`DELEGATION_SUPERSESSION_GAP = OPEN_HIGH`

`WP06_TECHNICAL_ACCEPTANCE = NOT_YET`

`WP06_EXECUTABLE_VALIDATION = NOT_YET`

`WP07_WP08_IMPLEMENTATION_AUTHORITY = NOT_GRANTED`
