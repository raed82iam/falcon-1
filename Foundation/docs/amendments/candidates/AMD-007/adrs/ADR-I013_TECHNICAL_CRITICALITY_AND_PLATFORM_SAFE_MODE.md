# ADR-I013 — Technical Criticality and Platform Safe Mode

**Identifier:** ADR-I013  
**Version:** Proposed 1.0  
**Status:** Proposed  
**Stage 1 Authority:** Not Granted

## Context

FFG requires generic technical priority and a Safe Mode survival set without learning business value.

## Decision

Falcon SHALL govern technical criticality using `CRITICAL`, `ESSENTIAL`, `STANDARD`, and `OPTIONAL` with immutable catalog meanings.

Classification requires admission authority and evidence. Applications SHALL NOT self-declare effective criticality.

Metadata SHALL cover recovery priority, downtime, minimum resources, Foundation dependencies, communication/persistence/security/authority needs, isolation/restart/failover, degraded mode, safe shutdown, and protection requirements.

FFG SHALL resolve technical conflicts from this metadata and current evidence. It SHALL not infer business priority. Unresolved conflicts SHALL preserve the safest bounded state and escalate.

`PLATFORM_SAFE` SHALL preserve an Approved minimum survival set including authority enforcement, FFG, sufficient independent awareness, security, technical evidence, minimal Runtime/Lifecycle/Resources/FIL/Service Bus/Persistence, and recovery controls.

## Ownership

- until a separately chartered Technical Criticality Approval Authority exists, the Project Owner SHALL remain the only approval authority for a technical-criticality class assignment;
- this interim authority is formally deferred from delegation and SHALL NOT be inferred by FFG, FSA, an Application, admission tooling, or a Catalog owner;
- a future charter may delegate approval only through an explicit governance decision preserving challenge and conflict-of-interest controls;
- the Catalog owner administers values but does not approve an Application assignment;
- Application owner proposes;
- admission authority validates;
- FFG consumes;
- Resource Management executes allocation;
- FSA assesses current technical condition.

## Rejected Alternatives

- Application self-priority;
- static priority embedded in FFG code;
- business value transmitted to FFG;
- universal shutdown whenever one critical workload fails.

## Consequences

A governed Catalog, Application Manifest fields, conflict rules, survival-set profile, evidence, and verification are required before activation.

ADR-I013 SHALL NOT be activated while its Technical Criticality approval authority is unresolved beyond the explicit interim Project Owner rule above.

