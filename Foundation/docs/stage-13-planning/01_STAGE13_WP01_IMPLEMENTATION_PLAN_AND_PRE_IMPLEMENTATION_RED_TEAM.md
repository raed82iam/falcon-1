# Stage 13 WP-01 Implementation Plan and Pre-Implementation Red Team

**Scope:** Falcon-wide Independent AI Kill Control Plane + Falcon Safe Core  
**Authority:** Project Owner authorized Stage 13 WP-01 implementation on 2026-08-16.  

## Implementation plan

1. Add an AI-target registry with exact target kind, parent hierarchy, owning scope, executable-AI flag, evidence and validity interval.
2. Add a single Foundation-owned `AiKillControlRuntime` with dual ingress identity (`WebOwner`, `ExternalOwner`). Ingress is attributable transport context, never authority.
3. Reuse AUT-001 for exact legitimate authority. Add a stronger structural guard: any currently registered AI target is prohibited from acting as Kill Control Plane actor.
4. Resolve targeted requests only against one exact active registration and its descendants. Unknown/stale/invalid identity fails closed and does not widen.
5. Permit all-AI blast radius only through exact `GlobalAiKill + AllAi + falcon:all-ai` semantics.
6. Preserve a non-AI Safe Core on every decision and explicitly deny any implication of full-Falcon shutdown.
7. Add an authority enforcer that blocks contained AI from obtaining authority after Kill, including after restart or review-deadline passage.
8. Provide no release, recovery, restore-trust or revival method in WP-01.
9. Add a dedicated Stage 13 WP-01 executable verifier and controlled-solution registration.

## FSA hard isolation

The Kill Control Plane lives in Foundation.Authority and has no runtime reference to `Foundation.SelfAwareness` or `Foundation.Guardian`. FSA is an ordinary killable AI target from the control plane's perspective and cannot be an actor when registered as FSA.

```text
CONTROL_DIRECTION = KILL_CONTROL_PLANE -> FSA
FSA -> KILL_CONTROL_PLANE_CONTROL = PROHIBITED
```

## Safe Core

Minimum preserved non-AI capabilities are represented as:

- Owner control;
- AI Kill control;
- Lifecycle enforcement;
- authority revocation;
- security;
- audit/evidence;
- forensics;
- recovery infrastructure;
- emergency communications.

The list is a WP-01 minimum contract, not a deployment topology and not permission to bypass normal authority.

## Pre-implementation Red Team

The design was challenged before code write against the following attacks/failures:

- FSA tries to issue its own Kill request;
- another Application AI tries to issue Kill;
- AUT-001 policy accidentally lists an AI as allowed actor;
- unknown target identity;
- wrong target-kind binding;
- duplicate target identity;
- stale registration producing orphan hierarchy;
- missing parent;
- parent cycle;
- attempt to register the Kill Control Plane itself as AI;
- normal Kill attempting ALL_AI;
- Global AI Kill using a non-global target;
- defined-group descendant containment;
- restart trying to restore killed AI authority;
- review deadline interpreted as release;
- Web ingress treated as authority;
- external ingress implemented as a second Kill engine;
- Global AI Kill accidentally stopping non-AI Owner/Safe-Core authority;
- FSA/SelfAwareness runtime becoming a Kill Control dependency;
- Guardian runtime becoming a Kill Control dependency;
- release/recovery API leaking into WP-01;
- Application Trading/business semantics leaking into Foundation.Authority.

## Pre-implementation Red Team disposition

All identified Critical/High/Medium design risks are blocked by explicit fail-closed controls in the implementation plan. Executable proof is still required and this document does not claim test success.

```text
PRE_IMPLEMENTATION_RED_TEAM = PASS_FOR_IMPLEMENTATION
OPEN_CRITICAL = 0
OPEN_HIGH = 0
OPEN_MEDIUM = 0
EXECUTABLE_VALIDATION = REQUIRED
POST_EXECUTABLE_RED_TEAM = REQUIRED
```
