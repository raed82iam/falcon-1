# APP-001 — Application Boundary and Lifecycle

**Identifier:** APP-001  
**Version:** 1.1  
**Status:** APPROVED PENDING COORDINATED ACTIVATION  
**Approval Record:** GOV-063  
**Activation:** Not Authorized

## Application Invariant

A Falcon Application is an independent, contract-governed Plug-in Application hosted by Falcon OS. Application business logic remains Application-owned.

Every Application SHALL be independently installable, identifiable, validatable, registerable, admissible, activatable, observable, updateable, suspendable, isolatable, recoverable, replaceable, and removable.

## Lifecycle

```text
PACKAGE_RECEIVED
  → IDENTIFIED
  → VALIDATED
  → REGISTERED
  → ADMISSION_REVIEWED
  → ACTIVATION_ELIGIBLE
  → ACTIVE
```

Governed outcomes SHALL include rejected, quarantined, suspended, degraded, isolated, recovering, update-pending, rollback, removal-pending, removed, and archived.

Installation, registration, validation, admission, and activation are distinct decisions. No state implies the next state.

## Boundary Requirements

- all Foundation use SHALL occur through declared contracts;
- undeclared permissions, routes, dependencies, resources, storage, or services SHALL be denied;
- direct access to another Application's internals is forbidden;
- Application identity and ownership SHALL remain stable and attributable;
- update SHALL preserve compatibility, migration, rollback or approved corrective action, and evidence;
- removal SHALL reconcile authority, routes, resources, state, dependencies, evidence, and retained records;
- replacement or removal SHALL not compromise Foundation, shared services, or another Application;
- Application failure SHALL be isolated and contained;
- internal Application business recovery remains Application-owned.

## Awareness and Guardian Integration

Every Application SHALL declare exactly one MSA. Every declared major Application branch SHALL own exactly one LSA responsible for that branch. The Application SHALL also declare its optional CSA eligibility policy, health reporting, origin-aware self-development escalation, Application Guardian requirements, and FSA conformance interfaces.

Production-bound self-development SHALL use the route matching its actual origin:

- CSA → Parent LSA → Application MSA → FSA;
- LSA → Application MSA → FSA;
- Application MSA → FSA.

Foundation-originated proposals SHALL use FSA and the separate Foundation self-development governance and approval lifecycle. No artificial lower tier may be inserted.

FSA performs final OS-governance and compatibility review only. Under GOV-AUT-001 and GOV-001, FSA review does not grant documentary activation, implementation, deployment, or production adoption. Final activation and adoption require separately authorized Project Owner and governance decisions.

## Acceptance

Acceptance requires evidence for coexistence of at least two domain-independent Applications, independent lifecycle control, denied hidden coupling, quota enforcement, failure containment, communication isolation, update/rollback, and complete removal without Foundation redesign.
