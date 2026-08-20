# VPL-GDN-001 — Foundation Guardian Acceptance-Evidence Plan

**Version:** Proposed 1.0  
**Status:** Approved Plan — Execution Not Authorized  
**Approval Record:** GOV-060  
**Governing Sources:** Proposed ADR-I010; proposed AUT-002 v2.0; AUT-001; CON-011; ADR-F008

## 1. Objective

Demonstrate that FFG can impose bounded, persistent, independently enforceable technical protection without acquiring Application business knowledge or ownership of the mechanisms executing its directives.

## 2. Mandatory Evidence Cases

### Boundary

- FFG rejects business payload interpretation and financial decision requests.
- technical criticality is taken only from an authorized governed source.
- Application protection requests expose technical treatment, not business rationale.
- FSA, FFG, AUT-001, and execution ownership remain distinct.

### Containment

- one faulty component is isolated;
- one harmful Application runtime is isolated;
- unaffected higher-priority technical operation is preserved;
- resource exhaustion, restart loop, message storm, invalid FIL traffic, and cascading failure are contained;
- failed or uncertain isolation causes entry to `PLATFORM_SAFE`.

### Safe Mode

- the approved survival set remains available;
- nonessential activity is denied;
- authority, evidence, security, minimal communication, persistence, and recovery control remain protected;
- unknown state is not reported as normal.

### Persistence and Release

- restrictions survive process, Guardian, Application, and Foundation restart;
- failover does not expand or silently transfer authority;
- time passage and self-attestation cannot release restriction;
- `PLATFORM_RECOVERY_GUARD` requires progressive verified restoration;
- the competent release authority, not the restricted actor, authorizes release.

### Compromise and Independence

- compromised FFG is isolated without clearing protection;
- compromised FSA is restricted through an independent path;
- loss of AUT-001 limits FFG to explicitly pre-authorized fail-safe actions;
- independent stop channel remains available;
- neither FSA nor FFG conclusively verifies itself in a material dispute.

### Evidence

- every intervention is fully reconstructable;
- contradictory and uncertain evidence remains visible;
- failed mandatory intervention is recorded as protection failure;
- no intervention evidence is erased or rewritten.

## 3. Required Evidence Set

The Root Verification Evidence Set SHALL preserve:

- applicable obligations and versions;
- environment and configuration context;
- Guardian identity and mandate;
- trigger and contradictory evidence;
- decision, uncertainty, scope, and consequence;
- issued directives and execution results;
- restriction persistence and recovery observations;
- independent verifier identity;
- release decision and authority;
- integrity, provenance, and completeness evaluation.

## 4. Acceptance Condition

Acceptance requires a `COMPLETE` and valid Evidence Set for the approved verification scope. Passing this plan does not authorize production, Stage 1, deployment, financial connection, or autonomous operation.
