# Stage 15 Implementation Plan and Pre-Implementation Red Team

**Stage:** 15 — Application Runtime Hosting, Admission, Activation and Capability Isolation  
**Entry reconciliation:** `00_STAGE15_ENTRY_AND_EXISTING_CAPABILITY_RECONCILIATION.md`

## 1. Work-package sequence

### WP-01 — Runtime Host Identity and Zero-Application State

Create deterministic host identity/state with zero Applications explicitly valid.

### WP-02 — Exact Prerequisite Binding

Bind one runtime candidate to canonical evidence derived from:
- exact Stage 14 artifact-consumption decision;
- exact Application Admission decision;
- exact Lifecycle attach-eligibility decision;
- exact Stage 6 Application resource grants.

None of those prerequisites may imply activation.

### WP-03 — Runtime Registration

Register an exact runtime instance only after all prerequisites are current, internally coherent and bound to the same Application/version.

### WP-04 — Separate Activation Authority

Activation requires a separate explicit activation-authority evidence object bound to exact runtime instance, Application, version, validity window and evidence identity.

### WP-05 — Capability Isolation

Runtime capability ownership is explicit and deterministic. An Application may access only its declared capabilities. Direct access to another Application's private capability surface is denied. Duplicate exclusive capability ownership fails closed.

### WP-06 — Suspension and Isolation

Provide generic runtime state transitions for suspend and isolate without confusing state transition with new business authority. An isolated Application loses capability availability to other Applications.

### WP-07 — Removal and Return to Zero

Removal requires an eligible Lifecycle detach/remove decision and reconciles current runtime/capability bindings from host state. Removing the last Application returns to a valid zero-Application state.

### WP-08 — Failure Containment and Coexistence

Prove two domain-independent synthetic Applications can coexist, one can be isolated/removed, and the other remains unaffected.

### WP-09 — Integrated Hardening and Closure Verification

Run full governed build, Architecture, Security, predecessor regressions through Stage 14, Stage 15 verifier twice, deterministic rerun, exact candidate equality and clean worktree.

## 2. Final implementation placement

Stage 15 runtime-host orchestration is an independent production boundary:

`src/Foundation.ApplicationRuntimeHosting/`

Runtime source:

`src/Foundation.ApplicationRuntimeHosting/Stage15ApplicationRuntimeHost.cs`

Verifier:

`verification/Falcon.Stage15.ApplicationRuntimeHosting.Verifier/`

`Foundation.ApplicationRuntimeHosting` has zero production `ProjectReference` dependencies. It consumes normalized canonical predecessor evidence bindings rather than directly coupling to Admission, State, ArtifactPublication or ApplicationLifecycle assemblies. The verifier references the accepted predecessor projects and proves the mappings using real predecessor outputs.

The earlier attempt to compile Stage 15 public activation types into `Foundation.ApplicationLifecycle` was rejected by governed predecessor regression because Stage 5 WP-09 intentionally protects that closed assembly from activation/deployment API expansion. The final independent assembly preserves that predecessor invariant.

## 3. Runtime host states

```text
REGISTERED
ACTIVE
SUSPENDED
ISOLATED
REMOVED
```

`REGISTERED != ACTIVE` is mandatory.

## 4. Activation model

Activation eligibility is not activation authority.

```text
ARTIFACT_CONSUMED != ACTIVATED
ADMITTED != ACTIVATED
LIFECYCLE_ATTACH_ELIGIBLE != ACTIVATED
RESOURCE_GRANTED != ACTIVATED
REGISTERED != ACTIVATED
ACTIVATION_AUTHORITY_VALID + REGISTERED = MAY_TRANSITION_TO_ACTIVE
ACTIVE != BUSINESS_AUTHORITY
```

## 5. Capability model

A runtime capability binding includes:

- capability identity;
- owning Application identity;
- owning runtime instance;
- visibility: `PRIVATE` or `SHARED_DECLARED`;
- exclusivity flag.

Rules:

- PRIVATE capability is accessible only to its owner;
- SHARED_DECLARED capability is accessible only when the consumer declares that exact capability;
- an exclusive capability cannot have two current owners;
- isolation/suspension removes availability but not historical evidence;
- removal removes current availability while preserving decision/evidence identity.

## 6. Pre-implementation and placement Red Team

### Attack: admission treated as activation
Defense: registration and activation are separate. Activation requires exact separate authority evidence.

### Attack: Stage 14 technical consumption becomes runtime authority
Defense: exact artifact consumption is mandatory but carries no activation/deployment/business authority.

### Attack: stale/wrong resource grant reused
Defense: every resource grant must match exact Application principal, be effective at host observation time and carry evidence.

### Attack: runtime identity substitution
Defense: activation authority binds exact runtime instance + Application + version.

### Attack: duplicate runtime instance or Application alias
Defense: current runtime identity and current Application hosting are unique.

### Attack: Application A consumes Application B private capability
Defense: private capability resolution requires self ownership.

### Attack: duplicate exclusive capability
Defense: registration fails closed if another current runtime owns the same exclusive capability.

### Attack: isolated/suspended Application still provides capability
Defense: only ACTIVE providers are available.

### Attack: one Application failure collapses host
Defense: isolate one runtime without mutating unrelated runtime states.

### Attack: removal destroys evidence/history
Defense: current-state removal is separate from immutable decision/evidence identity.

### Attack: zero Applications considered invalid
Defense: empty host is explicitly valid and deterministic.

### Attack: Stage 15 sneaks in environment-specific process/container semantics
Defense: no OS process/container/network/deployment API. Stage 16 remains environment-realization owner.

### Attack: identity/MFA or Web recovery projection gets absorbed opportunistically
Defense: FCR-0152 and FCR-0076 remain separate Foundation obligations outside Stage 15.

### Attack: Stage 15 expands a closed predecessor public assembly
Defense: runtime-host public activation/state surface is isolated in `Foundation.ApplicationRuntimeHosting`; `Foundation.ApplicationLifecycle` remains unchanged and Stage 5 WP-09 plus Stage 10 VPL-004 remain mandatory regressions.

### Attack: independent component creates dependency-graph sprawl
Defense: `Foundation.ApplicationRuntimeHosting` has zero production project references and accepts normalized predecessor evidence bindings. Architecture Guard explicitly verifies that zero-reference boundary.

## 7. Pre-implementation / placement Red Team result

```text
OPEN_CRITICAL = 0
OPEN_HIGH = 0
OPEN_MEDIUM = 0
IMPLEMENTATION_GUARDS_REQUIRED = YES
PRE_IMPLEMENTATION_RED_TEAM = PASS_WITH_MANDATORY_GUARDS
PLACEMENT_REMEDIATION = BOUND_TO_PREDECESSOR_REGRESSION
```

## 8. Acceptance bar

```text
ZERO_APPLICATION_STATE = PASS
TWO_DOMAIN_INDEPENDENT_APPLICATIONS = PASS
EXACT_ARTIFACT_BINDING = PASS
ADMISSION_SEPARATION = PASS
ACTIVATION_SEPARATION = PASS
RESOURCE_GRANT_BINDING = PASS
CAPABILITY_ISOLATION = PASS
FAILURE_ISOLATION = PASS
REMOVAL_BACK_TO_ZERO = PASS
STAGE5_WP09_PUBLIC_SURFACE = PRESERVED
STAGE5_WP10_INTEGRATION = PASS
STAGE10_VPL008 = PASS
NO_STAGE16_REALIZATION = PASS
ARCHITECTURE = PASS
SECURITY = PASS
DETERMINISTIC_RERUN = PASS
```
