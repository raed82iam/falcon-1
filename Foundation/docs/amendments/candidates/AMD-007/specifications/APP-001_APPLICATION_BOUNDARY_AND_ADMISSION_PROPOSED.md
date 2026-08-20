# APP-001 — Application Boundary and Admission

**Identifier:** APP-001  
**Version:** Proposed 1.0  
**Status:** Proposed  
**Stage 1 Authority:** Not Granted

## Purpose

Define the generic Plug-and-Play Application/Suite boundary and governed lifecycle above Falcon Foundation.

## Requirements

- every Application/Suite SHALL have immutable identity, owner, version, package integrity, Manifest, Contracts, permissions, dependencies, technical criticality proposal, health/lifecycle/recovery behavior, evidence, upgrade, rollback, and removal plan;
- Applications SHALL use only Approved platform Contracts and SHALL NOT couple to Foundation internals;
- package installation SHALL NOT grant registration, admission, activation, authority, or business approval;
- every Application SHALL be independently startable, stoppable, suspendable, isolatable, upgradeable, replaceable, rollback-capable, and removable;
- removal SHALL reconcile owned resources/state and leave Foundation complete;
- Foundation SHALL treat business payload as opaque;
- every Suite SHALL declare Guardian requirement as `REQUIRED`, `OPTIONAL`, or `NOT_APPLICABLE`;
- missing/untrusted required Guardian SHALL block full activation;
- technical criticality requires independent admission approval;
- FSA conformance SHALL not replace Owner, Architecture, business, security, or activation authority.

## Lifecycle

`PACKAGED → IDENTIFIED → VALIDATED → SANDBOXED → CONFORMANCE_ASSESSED → ADMITTED → REGISTERED → ACTIVATION_ELIGIBLE → ACTIVE`, with explicit rejected, suspended, isolated, rollback, removal, and archived outcomes.

## Acceptance

Acceptance requires zero-Application Foundation completeness; independent install/upgrade/isolation/rollback/removal; Trading and Accounting coexistence without Foundation redesign; hidden-coupling rejection; required-Guardian failure; and historical reconstruction.

