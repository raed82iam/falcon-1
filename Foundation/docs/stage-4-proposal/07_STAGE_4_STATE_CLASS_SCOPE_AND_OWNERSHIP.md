# Stage 4 State-Class Scope and Ownership

## Purpose

Define the exact FDN-001 state classes governed by Stage 4.

This document is a planning artifact only. It does not authorize implementation.

## In-Scope State Classes

| State class | Authoritative owner | Authoritative source | Persistence rule | Concurrency rule | Recovery rule | Read authority | Write authority | Retention |
|---|---|---|---|---|---|---|---|---|
| Lifecycle State | Foundation Lifecycle owner established by Stage 3 | Durable authoritative lifecycle record | Persist every accepted version | Compare expected version; one successor per version | Reconcile durable state, lifecycle event, and evidence | Governed Foundation readers | Lifecycle owner only through authorized transition boundary | Full history |
| Authority Policy Baseline | Foundation authority-policy owner | Approved canonical policy snapshot | Persist versioned policy snapshots | Replacement by explicit version; no in-place mutation | Recover last trusted non-revoked snapshot | Authority Engine and approved reviewers | Policy owner under explicit governance authority | Full history |
| Authority Decision | Authority Engine | Canonical evaluated decision record | Persist every final allow or deny decision required by policy | Deterministic identity; duplicate identical request returns same decision | Reconstruct from request, policy, evidence, and decision digest | Execution boundary, evidence verifier, approved reviewers | Authority Engine only | Full audit retention |
| State Ownership Declaration | Foundation state-ownership registry owner | Canonical ownership registry | Persist every accepted ownership version | One active owner per state subject and namespace | Reject conflicting or missing ownership; recover last trusted version | Foundation control-plane readers | Ownership registry owner only | Full history |
| Operational Evidence | Evidence Journal owner | Append-only evidence journal | Append after governed decision and execution outcome | Monotonic journal sequence and integrity linkage | Validate chain; gaps require reconciliation | Approved verifiers and governed readers | Evidence Journal only | Full audit retention |
| Accepted-Fact Event | Accepted-Fact Event owner | Immutable accepted-fact store | Emit only after durable accepted state effect | One event identity per accepted fact | Compare event with state and journal | Governed event readers and verifiers | Accepted-Fact publisher only after durable commit proof | Full audit retention |
| Persistence Commit State | Persistent State provider owner | Provider commit ledger or equivalent canonical record | Persist commit identity, outcome, revision, and provider evidence | Idempotency identity plus expected revision | Query by request and commit identity; unresolved outcome remains uncertain | State coordinator and verifier | Persistent State provider only | Until fully superseded plus audit history |
| Reconciliation State | Restart Reconciler owner | Reconciliation result bound to state, journal, event, and provider evidence | Persist each completed reconciliation classification | One result per reconciliation request identity and evidence set | May be recomputed from unchanged evidence with deterministic result | Foundation control-plane readers | Restart Reconciler only | Full audit retention |

## Explicitly Out of Scope

Stage 4 does not take ownership of:

- Application business state;
- market data;
- portfolio state;
- trading positions;
- strategy state;
- Self-Awareness internal learning state;
- FIL transport state;
- Service Bus delivery state;
- Guardian crisis state beyond existing declared interfaces.

## Ownership Rules

1. Every state subject and namespace has one active authoritative owner.
2. Observers, caches, projections, replicas, and last-known copies are never authoritative by default.
3. Persistence technology does not own state merely because it stores bytes.
4. Authority Engine does not own lifecycle or domain state.
5. Evidence Journal does not become the current-state store.
6. Accepted-Fact Event does not precede durable state proof.
7. Owner replacement requires explicit governed authority and a new version.
8. Missing or conflicting ownership fails closed.

## FDN-001 Coverage

The selected classes above are the Stage 4 state classes for which WP-03, WP-04, and WP-05 must prove the applicable FDN-001 requirements.

Any additional state class requires a separately reviewed planning amendment before implementation.
