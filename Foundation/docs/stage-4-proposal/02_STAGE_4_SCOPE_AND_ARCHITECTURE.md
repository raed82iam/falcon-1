# Stage 4 Scope and Architecture

## 1. Canonical Objective

Stage 4 establishes accountable state change across Falcon Foundation.

It must prove that a sensitive Foundation action or lifecycle transition:

- is evaluated against explicit authority;
- is attributable;
- cannot exceed its scope;
- changes exactly one authoritative state;
- is persisted without hidden or competing truth;
- produces reconstructable evidence;
- survives restart without fabrication.

## 2. Existing Stage 3 Baseline

Stage 3 already provides:

- admitted identities and manifests;
- Contract Registry and Service Catalog controls;
- dependency-governed bootstrap context;
- lifecycle request and result structures;
- lifecycle transition validation;
- authority-result validation at the lifecycle boundary;
- state versions, attempts, events, and rejection evidence;
- deterministic verification through WP-06.

Stage 4 shall extend and integrate this baseline. It shall not create a second Lifecycle controller or silently replace accepted Stage 3 behavior.

## 3. Stage 4 Logical Responsibilities

### Authority Evaluation

Implements CON-002 as a reusable default-deny decision boundary.

It evaluates identity, action, resource, purpose, scope, context, policy, fitness, expiry, delegation, and revocation.

It returns `ALLOW` or `DENY`. It does not execute the action.

### Lifecycle Authority Integration

Uses CON-003 and the accepted Stage 3 lifecycle baseline.

It ensures that:

- the source state is authoritative and current;
- invalid, stale, duplicate, conflicting, and unauthorized transitions are rejected;
- accepted request is not confused with completed transition;
- failed transitions expose actual resulting state;
- completed transitions produce exactly one authoritative event.

### State Authority and Persistence

Implements FDN-001 ownership rules:

- one authoritative owner;
- one authoritative source;
- singular write authority;
- explicit state class;
- durable versioning;
- reconstruction without rewriting history;
- Application business-state isolation.

### Evidence and Accepted Facts

Provides an append-only, integrity-linked evidence journal.

Accepted facts become immutable events only after the authoritative state effect is durably proven.

### Conflict and Reconciliation

Handles:

- stale versions;
- competing writers;
- duplicate requests;
- uncertain write outcome;
- restart reconciliation;
- state/evidence divergence.

## 4. Required Execution Flow

```text
Governed Request
    ↓
Identity and Contract Validation
    ↓
Authority Evaluation under CON-002
    ↓
Execution Boundary
    ↓
Authoritative Owner and Current Version Validation
    ↓
Lifecycle or State Mutation under CON-003 / FDN-001
    ↓
Durable State Result
    ↓
Integrity-Linked Evidence
    ↓
Immutable Accepted-Fact Event
```

A denial must stop before execution.

An accepted authority decision alone is never proof that execution occurred.

## 5. Hard Boundaries

Stage 4 shall not:

- read Markdown documents as runtime policy or state;
- grant authority from technical reach or prior success;
- create a second source of truth;
- treat cached, observed, desired, expected, or last-known state as authoritative;
- let a component self-certify its own successful transition;
- emit accepted-fact events before durable success;
- infer Application business meaning;
- implement FIL or Service Bus behavior reserved for Stage 5;
- implement Self-Awareness behavior reserved for later stages.
