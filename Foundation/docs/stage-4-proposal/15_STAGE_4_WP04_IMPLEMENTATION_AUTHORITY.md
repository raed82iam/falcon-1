# Stage 4 WP-04 Implementation Authority

## Work Package

**WP-04 — Integrity-Linked Evidence Journal and Immutable Accepted Facts**

## Objective

Implement a bounded Foundation Evidence capability that records decisions and outcomes as append-only, integrity-linked evidence and emits immutable accepted-fact events only after durable accepted state effects are proven.

## Core Rule

```text
DECISION != ACCEPTED FACT
STATE ATTEMPT != ACCEPTED FACT
DURABLE COMMIT PROOF + ACCEPTED OUTCOME = ELIGIBLE ACCEPTED FACT
```

A denial shall be recorded and reconstructable, but it shall never become an accepted-fact event.

## Journal Rules

- append-only;
- deterministic record identity;
- monotonic sequence;
- previous-record digest linkage;
- canonical digest over material fields;
- immutable prior records;
- no in-place correction;
- exact duplicate handling;
- changed-content duplicate rejection;
- explicit chain validation result.

## Accepted-Fact Rules

An accepted-fact event requires all of:

1. an allowed governed decision;
2. an accepted execution outcome;
3. a durable commit result;
4. the committed state subject and version;
5. evidence identity and journal position;
6. deterministic accepted-fact identity.

No event is permitted when the action is denied, rejected, incomplete, uncertain, partial, corrupted, conflicting, or not durably committed.

## Time Independence

Timestamps may exist only as descriptive evidence metadata.

They shall not:

- expire Owner authority;
- invalidate a paused work package;
- control whether the Owner may resume;
- establish permission;
- make integrity depend on elapsed time.

## Bounded Integration

WP-04 may consume:

- Authority decision results;
- Lifecycle execution results;
- durable state commit results.

It may not become:

- the Authority Engine;
- the current-state store;
- the Lifecycle controller;
- the WP-05 reconciler;
- an ordinary logging service.

## Exact Allowlist

The exact path allowlist is bound by the Owner authorization package created by the authorization script.

Everything outside that allowlist remains prohibited.

## Exit

WP-04 exits only when every accepted fact is linked to a proven durable state change, every denial is attributable, tampering is detected, deterministic replay passes, independent review passes, and the Owner closes the work package.
