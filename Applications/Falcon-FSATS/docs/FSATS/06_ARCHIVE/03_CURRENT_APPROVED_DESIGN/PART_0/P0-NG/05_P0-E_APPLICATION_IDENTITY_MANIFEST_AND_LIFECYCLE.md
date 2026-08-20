# P0-E — Application Identity, Manifest, Lifecycle and Deployment Eligibility

**Status:** `PROPOSED / OWNER_REVIEW_CANDIDATE / NOT_OWNER_ACCEPTED / NOT_CLOSED`  
**Scope:** `P0-E only`  
**Implementation Authority:** `NOT GRANTED`

---

## 1. Purpose

P0-E makes every FSATS Application independently identifiable, governable, admissible, updateable, suspendable, recoverable, replaceable, and removable without collapsing Application grouping, Foundation lifecycle, business authorization, or runtime environment into one state.

---

## 2. Responsibility

P0-E owns the FSATS-side design for:

- Application/package identity;
- manifest declarations;
- owner roles;
- MSA/LSA/CSA declarations;
- dependencies and capability declarations;
- permissions/resource/persistence/communication/health declarations;
- update/migration/rollback/removal semantics;
- unresolved authority-bearing field behavior;
- deployment-eligibility explanation.

Foundation remains authoritative for APP-001/CON-023 lifecycle/admission contracts.

---

## 3. Canonical Identity Families

Every Application SHALL have stable attributable identities sufficient to distinguish at least:

- Application identity;
- package/artifact identity;
- version and digest/provenance;
- owning role;
- MSA identity;
- LSA identities;
- eligible CSA identities where applicable;
- dependency identities;
- declared contract/capability identities;
- environment/account/user/service-role identities where relevant.

Display names are not canonical identifiers.

Wildcards such as `latest` SHALL NOT be used where the resolved version carries authority, compatibility, or reproducibility meaning.

---

## 4. FSATS Container Rule

FSATS grouping is architecture organization only.

It SHALL NOT be represented as:

- a hidden Application;
- a Foundation lifecycle principal;
- a shared MSA;
- a shared credential principal;
- a shared resource-grant principal;
- a shared mutable business-state owner.

If Foundation does not define a `ContainerId` or equivalent contract field, P0-E SHALL NOT invent one in CON-023.

Container membership may exist in an Application-owned architecture registry/profile as non-authoritative structural metadata.

---

## 5. Required Application Manifest Semantics

For each Application, the design SHALL define, as applicable:

- exact Application identity;
- exact package/version/digest;
- business purpose and responsibility;
- owning role;
- prohibited Foundation responsibilities;
- MSA identity;
- LSA declarations and branch ownership;
- CSA eligibility policy;
- dependencies and compatible versions;
- provided/consumed capabilities;
- required permissions;
- resource declarations;
- persistence/state declarations;
- communication/event declarations;
- health/readiness evidence;
- security requirements;
- self-development escalation routes;
- Guardian/FSA conformance interfaces where applicable;
- update/migration behavior;
- rollback/corrective-action behavior;
- removal behavior.

Missing authority-bearing values have no permissive default.

---

## 6. APP-001 Lifecycle Separation

Canonical Foundation lifecycle states remain distinct:

```text
PACKAGE_RECEIVED
 -> IDENTIFIED
 -> VALIDATED
 -> REGISTERED
 -> ADMISSION_REVIEWED
 -> ACTIVATION_ELIGIBLE
 -> ACTIVE
```

with governed outcomes such as rejected, quarantined, suspended, degraded, isolated, recovering, update-pending, rollback, removal-pending, removed, archived.

No lifecycle state implies the next.

---

## 7. Business Authorization Separation

Foundation `ACTIVE` does not mean:

- broker connectivity authorized;
- provider connectivity authorized;
- Trading enabled;
- Paper enabled;
- Tiny Live enabled;
- Live enabled;
- a user/account authorized;
- a strategy approved;
- capital approved for exposure.

```text
FOUNDATION_ACTIVE != BUSINESS_AUTHORIZED
```

Business/environment authorization is separately governed by Application/Owner/Guardian/Risk/contract rules as applicable.

---

## 8. Deployment Eligibility Vector

P0-NG defines a non-authoritative explanatory vector:

```text
IDENTITY_VALID
MANIFEST_VALID
DEPENDENCIES_RESOLVED
SECURITY_ELIGIBLE
RESOURCE_ELIGIBLE
COMMUNICATION_COMPATIBLE
STATE_MIGRATION_ELIGIBLE
ROLLBACK_READY
BUSINESS_AUTHORITY_SEPARATELY_VALID
```

This vector explains readiness only.

```text
DEPLOYMENT_ELIGIBILITY_VECTOR = EXPLANATION
DEPLOYMENT_ELIGIBILITY_VECTOR != APP001_ADMISSION
DEPLOYMENT_ELIGIBILITY_VECTOR != OWNER_DEPLOYMENT_AUTHORITY
```

---

## 9. Update Semantics

An Application update SHALL preserve or explicitly reconcile:

- artifact identity/provenance;
- dependency compatibility;
- contract compatibility;
- permissions;
- resources;
- state migration;
- schema/persistence changes;
- communication/security declarations;
- rollback or approved corrective action;
- evidence and health expectations;
- awareness topology if affected.

A self-development candidate does not bypass this update path.

---

## 10. State Migration

State migration SHALL define:

- source state/version;
- target state/version;
- transformation rules;
- invariants to preserve;
- partial-failure behavior;
- idempotency/retry safety where relevant;
- validation evidence;
- rollback or forward-recovery limitations;
- externally irreversible side effects that cannot be rolled back.

Migration success does not imply business reactivation authority.

---

## 11. Rollback / Corrective Action

Rollback SHALL NOT be treated as a magic reversal.

Before rollback, evaluate:

- exact previous accepted artifact;
- state compatibility;
- contract compatibility;
- dependent Application compatibility;
- schema/persistence compatibility;
- external side effects already committed;
- whether literal rollback is safer than forward recovery.

If literal rollback is unsafe/impossible, use an explicitly governed corrective/forward-recovery path.

---

## 12. Removal

Application removal SHALL reconcile:

- authority/permissions;
- routes/contracts;
- resources;
- state/persistence;
- dependent Applications;
- retained evidence/audit records;
- credentials/references;
- health/service registration;
- open obligations/exposure where relevant.

Removal cannot silently orphan a mandatory dependency or transfer business ownership to Foundation.

---

## 13. Unresolved Authority-Bearing Field Rule

When a required identity/version/permission/resource/route/security/authority value is unresolved:

```text
NO_DEFAULT_PERMISSION
NO_WILDCARD_AUTHORITY
NO_INVENTED_VALUE
FAIL_CLOSED_FOR_AFFECTED_CAPABILITY
DECLARE_RESOLUTION_SOURCE_AND_GATE
```

---

## 14. Failure / Degraded Behavior

Manifest mismatch, digest mismatch, incompatible dependency, unknown permission, unresolved security context, or failed migration SHALL result in an explicit governed non-active/degraded/quarantined/update-pending state as appropriate.

A failed Application lifecycle event cannot be bypassed by a business controller.

Application business recovery remains Application-owned within accepted lifecycle constraints.

---

## 15. Cross-Boundary Rules

- Applications use Foundation only through declared contracts;
- hidden access to another Application's internals is prohibited;
- cross-Application interaction belongs to P0-F and declared manifests;
- resource declarations do not create grants;
- communication declarations do not create routes unless Foundation/governance admits them;
- self-awareness topology does not create permission;
- FSATS grouping does not create shared authority.

---

## 16. Foundation / FCR Dependencies

P0-E depends on current APP-001/CON-023/ADR-I012/ADR-I015 semantics.

FCR-0016 remains relevant to canonical cross-workstream accepted Foundation artifact consumption. A known artifact identity does not by itself provide the canonical build-time mechanism.

Other FCRs may block runtime permissions/routes/egress even when the Application manifest design is complete.

---

## 17. Explicit Non-Authority

P0-E SHALL NOT:

- invent Foundation manifest fields;
- grant activation;
- authorize deployment;
- authorize provider/broker connectivity;
- convert declared resource need into a resource grant;
- convert communication declaration into business authority;
- treat `ACTIVE` as Live trading permission;
- let a self-development candidate mutate its own authoritative manifest directly.

---

## 18. Invariants

```text
APPLICATION_IDENTITY = STABLE_AND_ATTRIBUTABLE
PACKAGE_IDENTITY != APPLICATION_IDENTITY
INSTALLATION != REGISTRATION != ADMISSION != ACTIVATION
FOUNDATION_ACTIVE != BUSINESS_AUTHORIZED
DECLARED_RESOURCE_REQUIREMENT != GRANTED_RESOURCE
DECLARED_ROUTE != AUTHORIZED_ACTIVE_BUSINESS_ROUTE
TOPOLOGY != PERMISSION
SELF_DEVELOPMENT_PASS != MANIFEST_MUTATION
UNRESOLVED_AUTHORITY_FIELD = FAIL_CLOSED
```

---

## 19. Forbidden Interpretations

Invalid interpretations include:

- “the package is registered, so it may run Live”;
- “the manifest lists a broker route, so execution is authorized”;
- “a resource requirement equals an entitlement”;
- “the FSATS container is a lifecycle principal”;
- “a new LSA means the manifest can be changed automatically”;
- “rollback always restores the system exactly”;
- “a compatible-looking dependency can use `latest`”.

---

## 20. Exit Gates

```text
IDENTITY_COLLISIONS = 0
INVENTED_FOUNDATION_FIELDS = 0
WILDCARD_AUTHORITY_VERSIONS = 0
LIFECYCLE_BUSINESS_AUTHORITY_CONFLATION = 0
UNRESOLVED_AUTHORITY_FIELDS_WITH_PERMISSIVE_DEFAULT = 0
UPDATE_MIGRATION_MODEL = COMPLETE
ROLLBACK_CORRECTIVE_ACTION_MODEL = COMPLETE
REMOVAL_MODEL = COMPLETE
FCR0016_CONSUMPTION_BOUNDARY = EXPLICIT
```

---

## 21. Next Authorized Gate

P0-E acceptance would establish identity/manifest/lifecycle design only. It would not grant Application activation, runtime route activation, external connectivity, deployment, Paper, Tiny Live, or Live authority.
