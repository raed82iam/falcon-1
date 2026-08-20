# Stage 11 Implementation Plan and Pre-Implementation Red Team

**Stage:** 11 — Transport QoS, Deadline Governance and Observability  
**State:** OWNER-AUTHORIZED FOR FULL EXECUTION  
**Governing specification:** `OPS-001 v1.0` transport-observability scope

## 1. Plan objective

Close only the residual Stage 11 portion of FCR-0009 while preserving all accepted Stage 5 delivery and Stage 6 resource/pressure behavior.

## 2. Work packages

### WP-01 — Specification and source-truth binding
- complete `OPS-001` definition/activation gate;
- bind Stage 11 to accepted `Foundation.MessageDelivery` decision/outcome truth;
- prohibit duplicate delivery/resource authority.

### WP-02 — Transport latency sample derivation
Implement deterministic latency sample derivation from one accepted dispatchable delivery decision and its bound delivery outcome.

Required negative behavior:
- null input rejects;
- non-dispatchable decision rejects;
- decision/outcome identity mismatch rejects;
- route/policy/attempt/correlation/causation mismatch rejects;
- negative timing rejects;
- invalid deadline evidence rejects.

### WP-03 — Aggregate performance snapshot
Implement bounded deterministic aggregation with:
- valid/rejected counts;
- min/max;
- p50/p95/p99 nearest rank;
- deadline within/after counts;
- route identity inventory;
- quality state;
- deterministic evidence identity.

### WP-04 — Evidence quality and adversarial hardening
- duplicate sample cannot bias percentiles;
- partial observation is explicit;
- empty set is Insufficient;
- missing/invalid evidence never becomes zero latency;
- reordered input yields same result identity.

### WP-05 — Integrated Stage 11 verification
Dedicated Stage 11 verifier plus predecessor Stage 5 WP-06 regression, Architecture, Security, deterministic rerun and source-boundary checks.

## 3. Implementation placement

No new permanent Foundation subsystem or assembly will be created for the runtime behavior.

The Stage 11 transport-observability implementation belongs in the existing:

`src/Foundation.MessageDelivery/`

because it consumes delivery decision/outcome truth and does not own dispatch, resource authority or business semantics.

A dedicated verifier project may be added under `verification/` because verifier identities are stage-scoped evidence tooling rather than runtime subsystem identities.

## 4. Pre-implementation Architecture/Consistency review

Result: `PASS_FOR_IMPLEMENTATION`

Reasons:
- uses the accepted communication ownership surface;
- no new Service Bus/Event System/Authority Engine/Resource Manager;
- no cross-workstream write;
- zero-Application state remains valid;
- no Stage 12+ dependency;
- no financial/business semantics;
- source truth remains accepted Stage 5 delivery evidence;
- Stage 6 pressure/priority truth is consumed, not redefined.

## 5. Pre-implementation Red Team

### Attack: Application self-elevates by reporting low latency or a deadline
**Disposition:** BLOCKED BY DESIGN. Observation has no authority effect.

### Attack: missing timing evidence is treated as 0 ms
**Disposition:** BLOCKED. Insufficient/invalid quality required.

### Attack: duplicate a fast sample many times to improve percentile
**Disposition:** BLOCKED. Duplicate sample identity must not bias aggregation.

### Attack: reorder inputs to alter aggregate identity
**Disposition:** BLOCKED. Canonical sorted sample identity required.

### Attack: outcome from another route is paired with a fast decision
**Disposition:** BLOCKED. exact identity binding required.

### Attack: clock inversion creates negative duration
**Disposition:** BLOCKED. negative duration rejected.

### Attack: observed p99 becomes a contractual SLO
**Disposition:** BLOCKED by `LATENCY_OBSERVATION != LATENCY_GUARANTEE`.

### Attack: Stage 11 changes retry/defer/resource behavior based on metrics
**Disposition:** BLOCKED. v1.0 observability is read/derive only and has no execution/mutation surface.

### Attack: introduce Trading/Web/FSATS Fast Track special cases
**Disposition:** BLOCKED. Application-neutral semantics only.

### Attack: smuggle external/provider latency into Stage 11
**Disposition:** BLOCKED. Stage 12/16 concerns remain separately governed.

## 6. Pre-implementation findings

```text
CRITICAL = 0
HIGH = 0
MEDIUM = 0
LOW_BLOCKING = 0
```

Implementation may proceed under the current Stage 11 Owner authorization.

## 7. Mandatory validation

1. exact clean candidate;
2. governed SDK;
3. Restore;
4. Release build;
5. Architecture gate;
6. Security gate;
7. accepted Stage 5 WP-06 verifier regression;
8. Stage 10 verifier regression;
9. Stage 11 verifier;
10. deterministic Stage 11 rerun;
11. exact expected markers;
12. tracked worktree clean;
13. remote candidate stable during validation.
