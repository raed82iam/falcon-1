# OPS-005 — Platform Safe Mode Survival Set

**Identifier:** OPS-005 — Proposed Reservation  
**Version:** Proposed 1.0  
**Status:** Proposed  
**Governing Sources:** proposed ADR-I013; proposed AUT-002 v2.1  
**Stage 1 Authority:** Not Granted

## 1. Purpose

Define the minimum trusted capabilities, dependencies, resource floors, sequence, and fail-safe behavior required for Falcon Foundation to enter, sustain, recover from, and exit `PLATFORM_SAFE`.

## 2. Mandatory Survival Components

The survival set SHALL contain:

1. FFG decision continuity or independently authorized fallback protection;
2. AUT-001 authority validation or its Approved fail-safe subset;
3. identity, certificate, cryptographic, secret, time, and randomness capabilities required by the active protection profile;
4. minimal Runtime and Lifecycle control;
5. minimal Resource Governance control;
6. minimal FIL validation and Service Bus protection route;
7. protected Platform-restriction persistence;
8. Health Monitoring and independent watchdog evidence;
9. Security enforcement;
10. audit-critical Logging and evidence custody;
11. Recovery control; and
12. independent stop channel.

No mandatory component may be omitted because the failing subject requests continued operation.

## 3. Conditional Components

A component or Application MAY survive only when:

- its Approved technical criticality and survival profile require it;
- all mandatory dependencies are trusted and available;
- its minimum resource floor can be preserved;
- it can operate within `PLATFORM_SAFE` authority;
- isolation from unsafe peers is trustworthy;
- its continued operation reduces rather than increases material danger; and
- FFG has sufficient evidence to preserve it.

Examples include FSA or an Approved standby, selected Persistence services, recovery adapters, external notification, and technically critical Applications. Business importance alone is inadmissible.

## 4. Dependency Rules

Every survival entry SHALL declare identity, version, owner, required providers, authority, communication routes, persistence, security, resources, health, startup/shutdown/recovery dependencies, degraded behavior, and evidence.

Circular dependencies SHALL be rejected unless an Approved bootstrap profile proves a bounded safe resolution.

No survival component may depend on an optional component for a mandatory safety function.

## 5. Minimum Resources

The survival profile SHALL declare non-zero bounded floors for compute, memory, storage, I/O, communication, queue capacity, evidence capacity, recovery reserve, and cryptographic/security operations.

Resource Management SHALL reserve these floors before admitting conditional workloads.

When floors cannot be met:

1. optional and conditional workloads are shed in reverse technical-criticality order;
2. evidence and restriction integrity are preserved;
3. FFG escalates to a narrower emergency survival profile or independent stop;
4. unrestricted operation remains prohibited.

Exact values belong to an Approved environment-specific Catalog, not this Specification.

## 6. Entry and Startup Order

The minimum startup order is:

1. immutable baseline, identity, integrity, and restriction-state verification;
2. minimal Security and authority enforcement;
3. protected evidence/logging and trusted time sufficient for reconstruction;
4. independent stop and watchdog;
5. FFG or Approved fallback protection;
6. minimal Resource, Runtime, and Lifecycle control;
7. minimal FIL and Service Bus protection control plane;
8. protected Persistence and Recovery control;
9. FSA/Health technical awareness required by the active profile;
10. conditional technically critical workloads in dependency order.

No later step may start when a mandatory earlier dependency is untrusted unless an Approved fail-safe profile explicitly permits a narrower state.

## 7. Shutdown Order

The controlled shutdown order is generally the reverse dependency order:

1. prevent new nonessential admission and authority;
2. suspend optional and unsafe conditional workloads;
3. quiesce dependent routes and producers;
4. preserve authoritative state and evidence;
5. terminate conditional services;
6. retain FFG, authority, Security, evidence, minimal Persistence, Recovery, watchdog, and stop controls until all protected effects are reconciled;
7. perform final controlled termination.

An emergency stop MAY abbreviate graceful steps but SHALL preserve the maximum trustworthy restriction and evidence possible.

## 8. Recovery Order

1. preserve Platform restriction;
2. verify baseline, identity, authority, Security, and restriction state;
3. restore mandatory survival components;
4. restore evidence, Persistence, Resource, Runtime, Lifecycle, FIL, Service Bus, Health, FSA, and Recovery dependencies;
5. independently verify each recovery cohort;
6. admit conditional critical workloads;
7. enter `PLATFORM_RECOVERY_GUARD`;
8. restore lower-criticality workloads progressively;
9. release only through competent authority.

Restart, elapsed time, silence, or one healthy observation SHALL NOT advance recovery.

## 9. Fail-Safe Behavior

- unknown restriction state → default deny;
- unknown authority → only pre-authorized fail-safe subset;
- FFG unavailable → preserve restrictions, invoke fallback/stop, block high-risk activity;
- Persistence uncertain → no acknowledged state change and no blind re-execution;
- Service Bus/FIL untrusted → preserve local restrictions and use only Approved independent channel;
- Security trust loss → isolate affected identities and deny broader authority;
- insufficient evidence → remain in `PLATFORM_SAFE` or stop;
- resource floor failure → shed workload and preserve control/evidence;
- cascading failure → smallest trustworthy containment, then independent stop if containment is untrusted.

## 10. Acceptance Evidence

Acceptance requires deterministic dependency validation, startup/shutdown/recovery sequencing, resource-floor exhaustion, missing mandatory component, conditional Application admission/denial, FFG loss, authority loss, Persistence uncertainty, communication loss, security compromise, restart, failover, emergency stop, and complete reconstruction.

## 11. Normative Requirements

- **OPS-005-REQ-001:** The mandatory survival set SHALL remain domain-independent.
- **OPS-005-REQ-002:** Conditional survival SHALL require Approved technical criticality and current safety evidence.
- **OPS-005-REQ-003:** Mandatory dependencies and resource floors SHALL be explicit.
- **OPS-005-REQ-004:** Startup, shutdown, and recovery order SHALL be enforced and evidenced.
- **OPS-005-REQ-005:** Missing trust SHALL not produce broader permission.
- **OPS-005-REQ-006:** Platform restrictions SHALL survive restart, failover, and deployment.
- **OPS-005-REQ-007:** Independent stop SHALL remain available when ordinary control paths fail.
- **OPS-005-REQ-008:** Business value SHALL NOT determine survival.

