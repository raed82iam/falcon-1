# FSATS Complete Blueprint — Contracts, State, Evidence, Security and Reliability

**Candidate:** `FSATS-CB-v0.1`
**Status:** `DESIGN_CANDIDATE / NOT_OWNER_ACCEPTED / NOT_CLOSED`
**Implementation Authority:** `NOT GRANTED`

## 1. Purpose

This document defines the non-business plumbing rules that keep FSATS reconstructable, secure and resilient without duplicating Foundation ownership.

## 2. Contract-First Rule

Every cross-Application interaction must use a declared governed contract and route.

No Application may:

- read another Application database directly;
- call another Application internal class/project directly across the Application boundary;
- share mutable in-memory state;
- use a hidden filesystem/database queue as cross-App transport;
- create an undeclared side channel because the governed route is inconvenient.

## 3. Business Contract Envelope

Every material FSATS contract family defines, as applicable:

- immutable family ID;
- schema/version;
- producer Application;
- consumer Application(s);
- purpose;
- business authority class;
- security classification;
- payload identity;
- source/target environment;
- observation/effective/expiry/deadline times;
- correlation/causation;
- idempotency key;
- ordering expectations;
- duplicate behavior;
- correction/supersession behavior;
- truth class (`OPERATIONAL`, `SIMULATION`, `REPLAY`, `EVIDENCE`, etc.);
- acceptance/rejection rules;
- failure/degraded behavior;
- evidence retention references;
- Foundation binding dependencies.

Foundation owns FIL/Service Bus/event security/delivery primitives. Applications own business meaning and business acceptance rules.

## 4. Contract Family Groups

Candidate groups include:

### Provider/Data
- normalized Data Product publication;
- data quality/degradation status;
- provider capability/availability projection.

### Trading/Guardian
- protection observation/evidence;
- protection directive;
- directive outcome/effect evidence;
- recovery/release evidence.

### Resource
- Application resource profile;
- FSARM effective resource decision;
- Foundation additional-resource request/outcome binding;
- pressure/load-shedding state.

### Simulation/Validation
- experiment request/package;
- validation result/evidence;
- market qualification evidence;
- simulator fidelity result.

### Awareness
- Application MSA proposal/evidence package to FSA once FCR-0030 exact interface is available;
- integrity escalation outcome as Foundation design permits.

### Shared Web/Communication
- read models/notifications/Owner commands only through separately governed Shared Application contracts.

## 5. Contract Evolution

Schema evolution must preserve business meaning or explicitly create a new version/family.

Rules:

- additive optional fields may be compatible only when default/absence semantics are explicit;
- authority-bearing fields never receive permissive implicit defaults;
- removal/meaning change requires version transition;
- producer/consumer compatibility is tested;
- replayed old messages retain original schema identity;
- unknown critical version fails closed.

## 6. Idempotency

Any command/action that could create duplicate financial/protection/resource effect requires idempotency.

Idempotency identity binds the semantic operation, not merely one HTTP request.

Examples:

- order submission intent;
- cancel/replace request;
- Guardian directive;
- capital reservation;
- FSARM redistribution plan;
- state transition request.

Duplicate receipt returns/reconstructs the existing outcome rather than applying the effect again.

## 7. Correlation and Causation

Material events preserve both:

- `CorrelationId` — groups a business workflow/incident;
- `CausationId` — identifies the exact event/decision that caused this event.

A trade chain may therefore reconstruct:

```text
DATA OBSERVATION
-> ANALYSIS
-> STRATEGY PROPOSAL
-> RISK DECISION
-> CAPITAL RESERVATION
-> EXECUTION INTENT
-> BROKER EVENT
-> RECONCILIATION
-> PORTFOLIO EFFECT
```

## 8. State Ownership and Persistence

Each Application owns its authoritative business state and persists it behind its own boundary.

The design separates:

- authoritative state;
- derived projection/cache;
- telemetry;
- immutable evidence reference;
- historical analytical store.

A cache/projection is rebuildable and never silently becomes authoritative because its source is unavailable.

## 9. Transaction Boundary

Inside one Application, a local state transition that must atomically preserve business invariants should commit authoritative state and durable publication intent together where the chosen persistence technology supports it.

The implementation may use a transactional-outbox/inbox-like pattern locally, but this pattern remains Application-owned business reliability and does not replace Foundation transport governance.

## 10. Event Sourcing Position

FSATS does not require universal event sourcing.

Use immutable event journals where reconstructability materially benefits the domain, especially:

- orders/executions/reconciliation;
- capital reservations;
- Guardian incidents/directives;
- resource coordination decisions;
- strategy/model candidate lifecycle;
- high-consequence authority/evidence relationships.

Use ordinary transactional state plus attributable history where full event sourcing would add complexity without material safety value.

## 11. Evidence Graph

Falcon should be able to reconstruct why a material decision/action occurred without relying on free-form logs.

Evidence nodes may include:

- identity;
- authority;
- input Data Products;
- model/strategy/config version;
- decision;
- risk gate;
- reservation;
- Guardian state;
- execution event;
- reconciliation;
- validation case;
- candidate change;
- Owner/governance decision.

Edges express cause, derivation, ownership, review and supersession.

The graph is a reconstructability model, not a central authority service.

## 12. Audit vs Telemetry

Telemetry helps operate and debug the system. Audit/evidence proves governed facts.

```text
LOG LINE != AUTHORITY RECORD
METRIC != DECISION EVIDENCE
TRACE != BUSINESS STATE
```

The implementation may use OpenTelemetry for traces/metrics/logs because its .NET signals are mature and stable, but Falcon authority/evidence semantics remain governed independently.

## 13. Sensitive Data

Logs/traces/evidence must not casually include:

- raw API secrets;
- passwords;
- bearer tokens;
- private keys;
- unnecessary personal data;
- full broker credentials;
- unrestricted model prompts containing secrets.

Sensitive references use controlled identifiers and redaction.

## 14. Credential Model

Applications refer to credentials by governed credential reference/identity. They do not own a generic secrets platform.

Required separation:

```text
PAPER BROKER CREDENTIAL != LIVE BROKER CREDENTIAL
PROVIDER CREDENTIAL != BROKER CREDENTIAL
RESEARCH EGRESS CREDENTIAL != OPERATIONAL EGRESS CREDENTIAL
APPLICATION_A CREDENTIAL != APPLICATION_B CREDENTIAL
```

External egress remains blocked until the relevant Foundation capability is implemented and authorized.

## 15. Least Privilege

Every Application/process/tool route receives only the permissions needed for its exact responsibility.

Examples:

- FSAPMA provider adapters cannot place broker orders;
- Trading broker adapter cannot use research egress;
- FSTSimA cannot obtain Live broker order authority;
- Monitor AI cannot deploy/kill/release by itself;
- Strategy models cannot write Risk policy;
- web/dashboard readers cannot mutate trading state unless an explicit command contract and authority exists.

## 16. Tool-Use Security for AI

AI/agent tools are typed and allowlisted.

Each tool contract defines:

- caller eligibility;
- action scope;
- parameters;
- data sensitivity;
- side-effect class;
- authority requirement;
- timeout;
- output schema;
- audit/evidence requirement.

High-consequence side effects never depend only on a language-model interpretation of a natural-language instruction.

## 17. Supply-Chain Security

Implementation must later bind:

- exact package versions;
- approved feeds/sources;
- integrity hashes/signatures where available;
- dependency vulnerability review;
- reproducible restore/build evidence;
- software bill of materials as appropriate;
- model artifact provenance;
- prompt/config provenance;
- external tool/version provenance.

No external AI-generated or downloaded artifact goes directly from research to trusted production.

## 18. Secure Development Lifecycle

The implementation plan adopts secure-development practices throughout design, code, build, test, release and remediation rather than adding security at the end.

Required behavior includes:

- threat modeling for new authority/egress/data boundaries;
- code review proportional to consequence;
- automated architecture/security tests;
- dependency scanning;
- secret scanning;
- negative authorization tests;
- fuzz/property tests for parsers/state machines where useful;
- root-cause remediation rather than patch-only recurrence.

## 19. Reliability Patterns

Within Application ownership and without duplicating Foundation transport internals, use appropriate:

- bounded queues;
- backpressure;
- cancellation/timeouts;
- retry budgets;
- exponential backoff/jitter where appropriate;
- circuit breakers;
- bulkheads;
- rate limits;
- concurrency limits;
- idempotent consumers;
- health/readiness checks;
- load shedding;
- graceful degradation;
- reconciliation after uncertain outcomes.

## 20. Retry Rule

Never retry blindly when the prior action may have succeeded.

Classify operations:

- safely idempotent/read-only;
- idempotent by business key;
- non-idempotent/ambiguous.

Order submission, capital effects and protection commands require semantic idempotency and/or reconciliation before retry.

## 21. Deadline / Expiry

Every time-sensitive workflow distinguishes:

- observation time;
- receive time;
- decision time;
- effective time;
- expiry/deadline;
- completion time.

Stale decisions/messages are rejected rather than interpreted as current authority.

FCR-0009 remains the future Foundation transport-QoS/deadline capability. Application business expiry rules are designed now without pretending that unavailable transport features exist.

## 22. Queue and Backpressure Policy

Queues are bounded.

On overload:

1. preserve protection/reconciliation/open-position critical work;
2. protect execution-critical/current operational data as applicable;
3. degrade lower-priority analytics/discovery;
4. pause simulation/research/background work;
5. reject new work rather than accumulate unbounded stale backlog.

Old market data or decisions can become dangerous; buffering forever is not reliability.

## 23. Concurrency Control

Use explicit concurrency ownership for high-consequence aggregates such as:

- account/portfolio capital;
- one order chain;
- one instrument position lifecycle;
- Guardian directive scope;
- FSARM plan epoch.

Techniques may include optimistic concurrency/version checks, partitioned single-writer processing or locks where justified. The implementation chooses per aggregate based on performance and correctness evidence.

## 24. Recovery

Recovery distinguishes:

- restart;
- state restore;
- reconciliation;
- authority restore;
- trust restore.

On restart, an Application must re-establish:

- identity/lifecycle state;
- current authority/restrictions;
- persistent business state;
- broker/provider/resource projections;
- unresolved workflows;
- evidence continuity.

It must not assume a clean slate merely because process memory is empty.

## 25. Broker/Provider Reconnect

Reconnect sequence includes:

```text
REESTABLISH AUTHENTICATED SESSION
-> REFRESH CAPABILITY / ENTITLEMENT
-> RECOVER STREAM / SNAPSHOT
-> DETECT GAPS
-> RECONCILE STATE
-> MARK READY FOR INTENDED USE
```

Network connectivity alone is not readiness.

## 26. Environment Isolation

Data stores, queues/topics/routes, credentials, broker accounts and evidence must carry environment identity.

Cross-environment leakage is a high-severity defect.

Test/replay events must never be accepted by an operational consumer as Live authority because they share a schema.

## 27. Failure Classes

Common failure classes:

- transient transport;
- dependency unavailable;
- stale data;
- conflicting truth;
- invalid identity/authority;
- quota/capacity exhaustion;
- resource pressure;
- persistence failure;
- partial state transition;
- broker ambiguity;
- model/AI failure;
- integrity/security incident.

Each class has an explicit bounded response. `catch(Exception) -> retry forever` is prohibited.

## 28. Observability

Each Application emits standardized metrics/logs/traces for operations while keeping business evidence separate.

Key metric families:

- end-to-end decision latency;
- provider latency/quality;
- strategy proposal rates;
- Risk rejects/reductions;
- reservation pressure;
- order state latency;
- fill/slippage;
- reconciliation divergence;
- Guardian incidents/directives;
- resource pressure/shedding;
- simulation/fidelity results;
- AI integrity/monitor anomalies.

High-cardinality labels must be controlled to avoid resource failure through observability itself.

## 29. Performance Classes

Candidate business urgency classes:

- `PROTECTION_CRITICAL`;
- `EXECUTION_CRITICAL`;
- `LIVE_DATA_CRITICAL`;
- `NORMAL_OPERATIONAL`;
- `ANALYTICS`;
- `SIMULATION_RESEARCH`.

These are Application business classifications. They do not mint Foundation technical criticality. Mapping to Foundation transport/resource QoS requires governed contracts and current capability.

## 30. Tail-Latency Principle

Performance validation measures percentiles/tails, not only averages.

For critical paths track at least conceptually:

- p50;
- p95;
- p99;
- max/outlier behavior;
- timeout/rejection rate under load.

A low average with dangerous p99 is not acceptable for protection/execution.

## 31. Security Failure Rule

When identity, authority, credential, evidence integrity or environment separation cannot be proven for a high-consequence operation:

```text
UNKNOWN -> DENY / HOLD / ISOLATE
```

Security uncertainty never creates fallback permission.

## 32. Acceptance Gates

```text
DIRECT_CROSS_APP_DATABASE_ACCESS = 0
UNDECLARED_CROSS_APP_ROUTE = 0
AUTHORITY_FIELD_PERMISSIVE_DEFAULT = 0
DUPLICATE_FINANCIAL_EFFECT_PATH = 0
TEST_REPLAY_TO_LIVE_ESCALATION = 0
RAW_SECRET_IN_SOURCE_OR_NORMAL_LOG = 0
UNBOUNDED_QUEUE = 0
BLIND_RETRY_OF_AMBIGUOUS_EFFECT = 0
RESTART_AS_TRUST_RESTORATION = 0
TELEMETRY_AS_AUTHORITY = 0
```
