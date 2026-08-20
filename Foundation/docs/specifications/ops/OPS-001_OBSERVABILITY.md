# OPS-001 — Observability

**Identifier:** OPS-001  
**Version:** 1.0  
**Status:** Approved  
**Effective Date:** 2026-08-16  
**Owner:** Falcon Operational Integrity Authority  
**Governing Authority:** Falcon Vision; Falcon Constitution; IMP-001 v1.3; Stage 11 Full Execution Authorization 2026-08-16  
**Activation Scope:** Stage 11 transport-performance observability baseline  
**Implementation Authority:** Provided separately by the Stage 11 Full Execution Authorization  
**Supersedes:** None

## 1. Purpose

OPS-001 defines the truthful, attributable and fail-closed observability behavior required to measure and summarize Foundation transport performance without turning measurements into authority, business priority, or performance guarantees.

Version 1.0 establishes the Stage 11 transport-observability baseline. It does not claim that every future Falcon observability subject is implemented by Stage 11.

## 2. Scope

OPS-001 v1.0 governs:

- transport timing observations derived from accepted Foundation delivery truth;
- observation identity, scope, provenance and quality;
- deterministic latency derivation;
- bounded aggregate transport-performance snapshots;
- observed latency percentile/quantile projections;
- completeness, invalidity and insufficiency semantics;
- deadline outcome observation;
- Application-neutral transport-performance reporting;
- evidence needed to reproduce an aggregate result.

## 3. Non-Scope

OPS-001 v1.0 does not:

- schedule, dispatch, route, deliver or retry messages;
- create a second Service Bus or delivery controller;
- allocate resources or modify Stage 6 resource authority;
- create traffic-class authority;
- create Application business priority or Fast Track semantics;
- guarantee a latency SLO;
- claim deployment, environment, provider, broker or external-network performance;
- create financial or trading authority;
- create Guardian, Lifecycle, Recovery or FSA authority.

## 4. Governing truth sources

Transport observability SHALL consume accepted transport truth rather than invent a parallel event model.

Material timing input SHALL be attributable to accepted Foundation delivery evidence including, as applicable:

- route decision identity;
- delivery decision identity;
- correlation and causation identity;
- policy identity/version;
- attempt number;
- accepted dispatch/observation time;
- accepted outcome observation and outcome time;
- deadline/expiry where governed;
- producer/Application scope where present;
- evidence reference.

A measurement SHALL NOT become more authoritative than its source evidence.

## 5. Normative requirements

### OPS-001-REQ-001 — Attributable observation
Every accepted transport-performance sample SHALL identify the exact route/delivery scope and evidence from which it was derived.

### OPS-001-REQ-002 — Monotonic timing
An outcome time earlier than its governed start/dispatch observation time SHALL be rejected as invalid rather than converted into a negative or zero latency.

### OPS-001-REQ-003 — Missing evidence
Missing required timing evidence SHALL produce an explicit insufficient/invalid result. Missing evidence SHALL NOT be represented as zero latency.

### OPS-001-REQ-004 — Deterministic derivation
For the same canonical input set and aggregation request, the produced samples and aggregate snapshot SHALL be deterministic.

### OPS-001-REQ-005 — Bounded aggregation
Aggregation SHALL operate on an explicit finite input set and SHALL report the accepted sample count, rejected/invalid count and requested scope.

### OPS-001-REQ-006 — Percentile semantics
Where sufficient accepted samples exist, the aggregate SHALL expose observed p50, p95 and p99 latency using one deterministic documented nearest-rank rule. Percentiles describe the observed sample set only.

### OPS-001-REQ-007 — No guarantee inference
An observed percentile SHALL NOT be represented as a latency guarantee, SLO, SLA, reserved capacity, future performance promise or authority decision.

### OPS-001-REQ-008 — Deadline observation
Where a governed deadline/expiry is present, the sample MAY classify the observed outcome as within deadline or after deadline. This classification is observational and SHALL NOT create new routing, retry, authority or business consequences.

### OPS-001-REQ-009 — Scope isolation
An aggregate SHALL not silently mix different requested route scopes. Cross-route aggregation requires an explicit aggregate scope and shall preserve enough evidence to identify contributing route identities.

### OPS-001-REQ-010 — Application neutrality
No Application name, market, broker, strategy, user, customer, Web surface, or trading-specific term may receive privileged Stage 11 semantics.

### OPS-001-REQ-011 — Source truth preservation
Stage 11 observability SHALL not rewrite accepted delivery outcomes, pressure truth, traffic classes, authority decisions, event history or resource state.

### OPS-001-REQ-012 — Quality state
Every aggregate SHALL expose a quality state that distinguishes at least:

- `Complete` — all supplied observations valid for the requested aggregate;
- `Partial` — at least one valid sample and at least one rejected/invalid supplied observation;
- `Insufficient` — no valid sample is available for a requested aggregate;
- `Invalid` — the aggregate request itself is structurally invalid.

### OPS-001-REQ-013 — Evidence identity
Every successful aggregate SHALL have a deterministic evidence identity binding its canonical accepted sample set, rejected count, scope and calculated values.

### OPS-001-REQ-014 — Correction behavior
Later corrected evidence SHALL produce a new aggregate/evidence identity. A prior accepted aggregate SHALL not be silently rewritten in place.

### OPS-001-REQ-015 — Zero-Application validity
Foundation with zero Applications remains valid. An empty observation set is not a Foundation failure; a requested latency aggregate over that empty set is `Insufficient` rather than fabricated success.

## 6. Percentile rule

For `N > 0` accepted non-negative latency samples sorted ascending, percentile `p` uses nearest-rank selection:

`rank = ceil(p * N)`

with one-based rank clamped to `1..N`.

OPS-001 v1.0 uses:

- p50 = `p = 0.50`;
- p95 = `p = 0.95`;
- p99 = `p = 0.99`.

No interpolation is performed in v1.0. This makes the result reproducible without environment-specific numerical behavior.

## 7. Required aggregate truth

A successful or partial transport-performance snapshot SHALL expose at least:

- requested scope;
- valid sample count;
- rejected sample count;
- minimum observed latency;
- maximum observed latency;
- p50 observed latency;
- p95 observed latency;
- p99 observed latency;
- within-deadline count where deadline evidence exists;
- after-deadline count where deadline evidence exists;
- quality state;
- contributing route identities;
- deterministic evidence identity.

## 8. Fail-closed behavior

The Stage 11 observability boundary SHALL reject or reduce result quality for:

- negative duration;
- mismatched route/delivery identity;
- invalid attempt number;
- non-UTC or otherwise invalid timing where the source contract requires UTC;
- missing mandatory evidence reference;
- duplicate material sample identity where duplication would bias aggregation;
- structurally invalid aggregate scope;
- input conflict that prevents trustworthy calculation.

Unknown or invalid timing SHALL NOT be normalized into apparently healthy performance.

## 9. Security and authority boundary

Observability data can expose operational behavior and SHALL preserve evidence/security rules applicable to its source.

Mandatory distinctions:

```text
OBSERVABILITY != AUTHORITY
LATENCY_OBSERVATION != LATENCY_GUARANTEE
LOW_LATENCY != BUSINESS_PRIORITY
P95_PASS != RESOURCE_AUTHORITY
DEADLINE_MISS_OBSERVED != GUARDIAN_ACTION
MEASUREMENT != RELEASE_DECISION
```

## 10. Acceptance evidence

OPS-001 v1.0 transport scope is acceptable only when executable verification demonstrates at least:

1. deterministic positive samples;
2. p50/p95/p99 calculation;
3. reordered input produces the same aggregate identity;
4. negative duration rejection;
5. route identity mismatch rejection;
6. duplicate sample rejection or non-biasing handling;
7. partial-quality behavior;
8. empty set produces `Insufficient`;
9. deadline classifications are observational only;
10. no authority/resource/delivery mutation surface;
11. zero-Application neutrality;
12. deterministic rerun.

## 11. Future evolution

Future observability subjects may extend OPS-001 through governed successor versions. Stage 11 acceptance of this v1.0 transport baseline SHALL NOT be interpreted as implementation of all future metrics, logging, tracing, infrastructure monitoring, deployment telemetry, external-network monitoring, or Application business observability.
