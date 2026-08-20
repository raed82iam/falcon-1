# P0-E - Application Identity, Manifest, Lifecycle and Deployment Eligibility

**Status:** `OWNER_DIRECTED_INTEGRATED_REWRITE_CANDIDATE / NOT_OWNER_ACCEPTED / NOT_CLOSED`
**Implementation Authority:** `NOT_GRANTED`
**Runtime Authority:** `NOT_GRANTED`

## 1. Purpose

P0-E makes every FSATS Application independently identifiable, governable, admissible, updateable, suspendable, recoverable, replaceable and removable without collapsing Application grouping, Foundation lifecycle, business authorization, deployment eligibility or runtime environment into one state.

## 2. Responsibility

P0-E owns the FSATS-side design for:

- Application/package/artifact identity;
- manifest declarations;
- owner roles;
- MSA/LSA/CSA declarations;
- dependencies/capabilities;
- permissions/resources/persistence/communication/health/security declarations;
- business subject identity where relevant;
- update/migration/rollback/forward-recovery/removal semantics;
- unresolved authority-bearing field behavior;
- deployment-eligibility explanation;
- canonical Foundation artifact-consumption dependency.

Foundation remains authoritative for APP-001/CON-023 lifecycle/admission platform semantics.

## 3. Canonical current Application identities

Current FSATS contains exactly five independent Applications:

```text
TRADING
FSAPMA
TRADING_GUARDIAN
FSTSIMA
APP_RSC
```

Each is an independent Application/lifecycle principal. FSATS itself is not a sixth principal.

## 4. Canonical identity families

Every Application has stable attributable identities sufficient to distinguish at least:

- Application identity;
- package/artifact identity;
- version/digest/provenance;
- owning role;
- MSA identity;
- LSA identities;
- eligible CSA identities where applicable;
- dependency identities;
- provided/consumed contract/capability identities;
- configuration/state schema identities;
- environment identities;
- broker/provider/account/service-role identities where business material.

Display names are not canonical identifiers. Wildcards such as `latest` are prohibited where resolved version carries authority, compatibility or reproducibility meaning.

## 5. FSATS container rule

FSATS grouping is architecture organization only. It is not:

- hidden Application;
- Foundation lifecycle principal;
- shared MSA;
- shared credential principal;
- shared resource-grant principal;
- shared mutable business-state owner;
- cross-Application authority principal.

If Foundation does not define `ContainerId` or equivalent, P0-E does not invent one in CON-023. Structural membership may exist as non-authoritative Application-owned architecture metadata.

## 6. Broker-account business identity correction

Current Trading runtime identity is broker-account centric:

```text
FSATS_USER_ID = NONE
FSATS_USERNAME = NONE
FSATS_CUSTOMER_ID = NONE
TRADING_OPERATING_SUBJECT = BROKER_ACCOUNT
BROKER_ACCOUNT_IDENTITY = BrokerId + BrokerAccountId
ENVIRONMENT = additional identity dimension where material
```

Shared Web may own customer/user/contact -> broker-account mapping. That external mapping is not an FSATS customer principal and is not copied into FSATS Manifest as customer identity.

Provider-account/API identity is separate from broker-account identity.

## 7. Required Application manifest semantics

For each Application the design declares as applicable:

- exact Application identity;
- exact package/version/digest;
- business purpose/responsibility;
- owning role;
- prohibited Foundation responsibilities;
- MSA identity;
- LSA declarations/branch ownership;
- CSA eligibility/current declared CSAs;
- dependencies/compatible versions;
- provided/consumed capabilities/contracts;
- required permissions;
- resource declarations;
- persistence/state declarations;
- communication/event declarations;
- health/readiness evidence;
- security requirements;
- self-development escalation routes;
- Guardian/FSA conformance interfaces where applicable;
- update/migration behavior;
- rollback/corrective/forward-recovery behavior;
- removal behavior.

Missing authority-bearing values have no permissive default.

## 8. Exact topology declarations

Manifest/design topology must agree with current P0-C:

```text
Trading: 1 MSA / 13 LSA / 3 CSA
FSAPMA: 1 MSA / 6 LSA / 1 CSA
Guardian: 1 MSA / 4 LSA / 1 CSA
FSTSimA: 1 MSA / 8 LSA / 2 CSA
APP-RSC: 1 MSA / 3 LSA / 0 CSA initially
```

Topology creates no permission, route or resource authority.

## 9. APP-RSC manifest boundary

APP-RSC is independently admitted like the other Applications. Its manifest/design must preserve:

```text
APP_RSC_SCOPE = FSATS_ONLY
APP_RSC_IS_FALCON_APPLICATION = YES
APP_RSC_IS_FOUNDATION_RESOURCE_GOVERNANCE = NO
APP_RSC_IS_FSATS_CONTAINER = NO
APP_RSC_CAN_MINT_FOUNDATION_GRANTS = NO
```

Its exact MSA/three LSA identities must remain attributable. Foundation resource authority is a dependency, not an internal APP-RSC role.

## 10. APP-001 lifecycle separation

Conceptual lifecycle remains distinct:

```text
PACKAGE_RECEIVED
-> IDENTIFIED
-> VALIDATED
-> REGISTERED
-> ADMISSION_REVIEWED
-> ACTIVATION_ELIGIBLE
-> ACTIVE
```

with governed non-happy states such as rejected, quarantined, suspended, degraded, isolated, recovering, update-pending, rollback/corrective-action, removal-pending, removed and archived as supported by current Foundation contracts.

No lifecycle state implies the next.

```text
INSTALLATION != REGISTRATION != ADMISSION != ACTIVATION
```

## 11. Business authorization separation

Foundation `ACTIVE` does not mean:

- provider/broker connectivity authorized;
- Trading enabled;
- Paper/Shadow/Tiny-Live/Live enabled;
- broker account authorized for an action;
- strategy approved;
- market admitted;
- capital approved for exposure.

```text
FOUNDATION_ACTIVE != BUSINESS_AUTHORIZED
```

Business/environment authorization is separately governed by Application/Owner/Guardian/Risk/contract rules.

## 12. Deployment Eligibility Vector

The integrated design uses a non-authoritative explanatory vector:

```text
IDENTITY_VALID
MANIFEST_VALID
DEPENDENCIES_RESOLVED
SECURITY_ELIGIBLE
RESOURCE_ELIGIBLE
COMMUNICATION_COMPATIBLE
STATE_MIGRATION_ELIGIBLE
ROLLBACK_OR_FORWARD_RECOVERY_READY
BUSINESS_AUTHORITY_SEPARATELY_VALID
VALIDATION_SCOPE_VALID
```

```text
DEPLOYMENT_ELIGIBILITY_VECTOR = EXPLANATION
DEPLOYMENT_ELIGIBILITY_VECTOR != APP001_ADMISSION
DEPLOYMENT_ELIGIBILITY_VECTOR != OWNER_DEPLOYMENT_AUTHORITY
```

## 13. Update semantics

An Application update preserves/reconciles:

- artifact identity/provenance;
- dependency compatibility;
- contract compatibility;
- permissions;
- resources;
- state migration;
- schema/persistence changes;
- communication/security declarations;
- rollback/corrective-action path;
- evidence/health expectations;
- awareness topology where affected;
- broker-account/environment compatibility where Trading behavior is affected;
- validation/Intended Use scope where behavior changes.

A self-development candidate never bypasses this path and cannot directly mutate its own authoritative Manifest.

## 14. State migration

State migration defines:

- source state/version;
- target state/version;
- transformation rules;
- invariants to preserve;
- partial-failure behavior;
- idempotency/retry safety where relevant;
- validation evidence;
- rollback/forward-recovery limitations;
- externally irreversible side effects;
- authority/control epochs that must survive or be invalidated.

Migration success does not imply business reactivation authority.

## 15. Rollback and corrective action

Rollback is not magic reversal. Evaluate:

- exact previous accepted artifact;
- state/schema compatibility;
- contract compatibility;
- dependent Application compatibility;
- persisted/learned/adaptive state compatibility;
- external side effects already committed;
- broker/order/position consequences where Trading is involved;
- whether forward recovery is safer than literal rollback.

```text
OLD_ARTIFACT_AVAILABLE != SAFE_ROLLBACK
RESTARTED != RECOVERED
```

If literal rollback is unsafe/impossible, use explicitly governed forward recovery/corrective action.

## 16. Trusted baseline interaction

Application/awareness recovery may need exact trusted artifact/config/model/manifest/policy/state evidence. P0-C's distinction remains:

```text
LAST_TRUSTED_BASELINE != FACTORY_TRUSTED_BASELINE
```

No manifest declaration alone proves recoverability. Generic protected persistence/release remains Foundation-owned where applicable.

## 17. Removal

Application removal reconciles:

- authority/permissions;
- routes/contracts;
- resources;
- state/persistence;
- dependent Applications;
- retained evidence/audit records;
- credential references;
- health/service registration;
- open business obligations/exposure;
- APP-RSC coordination/resource claims;
- current Foundation lifecycle state.

Removal cannot silently orphan mandatory dependency or transfer business ownership to Foundation/another Application.

## 18. Unresolved authority-bearing fields

When required identity/version/permission/resource/route/security/authority value is unresolved:

```text
NO_DEFAULT_PERMISSION
NO_WILDCARD_AUTHORITY
NO_INVENTED_VALUE
FAIL_CLOSED_FOR_AFFECTED_CAPABILITY
DECLARE_RESOLUTION_SOURCE_AND_GATE
```

## 19. Failure/degraded behavior

Manifest mismatch, digest mismatch, incompatible dependency, unknown permission, unresolved security context, stale canonical dependency, invalid broker-account/environment binding or failed migration leads to explicit governed non-active/degraded/quarantined/update-pending state as appropriate.

A failed lifecycle event cannot be bypassed by a business controller. Application business recovery remains Application-owned within valid lifecycle constraints.

## 20. Cross-boundary rules

- Applications use Foundation only through declared/governed boundaries;
- hidden peer-internal access is prohibited;
- cross-Application interaction belongs to P0-F and manifests;
- resource declarations do not create grants;
- communication declarations do not create routes/business authority;
- awareness topology does not create permission;
- FSATS grouping does not create shared authority;
- APP-RSC membership/coordination does not create a hidden container identity.

## 21. Foundation artifact consumption

P0-E depends on APP-001/CON-023/ADR-I012/ADR-I015 and current Foundation contracts.

FCR-0016 remains the canonical cross-workstream Foundation artifact publication/Application consumption dependency. A known artifact SHA does not establish a production consumption mechanism.

Until Stage 14 canonical consumption exists and is verified:

```text
COPY_FOUNDATION_SOURCE_INTO_APPLICATION = PROHIBITED
MOVING_BRANCH_HEAD_AS_CANONICAL_DEPENDENCY = PROHIBITED
UNCONTROLLED_LOCAL_PACKAGE = PROHIBITED
```

APP-RSC final Foundation resource binding additionally remains fenced by FCR-0031.

## 22. External egress/credential declarations

A Manifest may declare needed provider/broker/research external capabilities and credential-reference roles, but declaration does not create them.

Current runtime dependencies remain separate:

```text
FCR-0008 = research egress
FCR-0013 = FSAPMA provider egress / credential references
FCR-0014 = broker execution egress / credential references
```

Secret bytes are not normal Application manifest/business payloads.

## 23. Awareness manifest impact

Each Application Manifest/design eventually declares exact MSA/LSA/eligible CSA identities, awareness goals/responsibility, authority/permissions, self-development eligibility/prohibited change classes, origin-correct escalation, monitor/integrity interfaces where supported by final contract model, trusted baseline references and recovery/re-entry expectations.

If CON-023 lacks a needed generic field, Application work uses FCR/architecture reconciliation rather than inventing a Foundation field.

## 24. Explicit non-authority

P0-E does not:

- invent Foundation Manifest fields;
- grant activation/deployment;
- authorize provider/broker connectivity;
- convert resource declaration into grant;
- convert communication declaration into active route/business authority;
- treat ACTIVE as Live permission;
- let self-development mutate authoritative Manifest;
- create FSATS customer identity;
- create APP-RSC Foundation authority.

## 25. Invariants

```text
APPLICATION_COUNT = 5
APPLICATION_IDENTITY = STABLE_AND_ATTRIBUTABLE
PACKAGE_IDENTITY != APPLICATION_IDENTITY
FSATS_IS_LIFECYCLE_PRINCIPAL = NO
FOUNDATION_ACTIVE != BUSINESS_AUTHORIZED
DECLARED_RESOURCE_REQUIREMENT != GRANTED_RESOURCE
DECLARED_ROUTE != AUTHORIZED_ACTIVE_BUSINESS_ROUTE
TOPOLOGY != PERMISSION
SELF_DEVELOPMENT_PASS != MANIFEST_MUTATION
UNRESOLVED_AUTHORITY_FIELD = FAIL_CLOSED
TRADING_BUSINESS_IDENTITY = BrokerId + BrokerAccountId
FSATS_USER_CUSTOMER_IDENTITY = NONE
APP_RSC_IS_APPLICATION_NOT_FOUNDATION
```

## 26. Forbidden interpretations

Invalid: registered package may run Live; manifest lists broker route so execution is authorized; resource requirement equals entitlement; FSATS container is lifecycle principal; new LSA automatically changes Manifest/permission; rollback always restores exactly; `latest` compatible-looking dependency is acceptable; APP-RSC manifest can request itself a Foundation grant; Web customer identifier belongs inside FSATS runtime identity.

## 27. Mandatory scenarios

Challenge identity collision; stale/wildcard dependency; digest mismatch; permission unknown; APP-RSC accidentally declared as container/Foundation; Trading Manifest carrying FSATS customer identity; awareness topology change without Manifest update; migration partial failure; rollback after external fill; removal with open exposure; removal with dependent contract; canonical Foundation artifact unavailable; provider/broker egress declared but unimplemented.

## 28. Exit gates

```text
IDENTITY_COLLISIONS = 0
FIVE_APPLICATION_MANIFEST_TOPOLOGY = CONSISTENT
INVENTED_FOUNDATION_FIELDS = 0
WILDCARD_AUTHORITY_VERSIONS = 0
LIFECYCLE_BUSINESS_AUTHORITY_CONFLATION = 0
BROKER_ACCOUNT_IDENTITY_CONFLATION = 0
UNRESOLVED_AUTHORITY_FIELDS_WITH_PERMISSIVE_DEFAULT = 0
UPDATE_MIGRATION_MODEL = COMPLETE
ROLLBACK_FORWARD_RECOVERY_MODEL = COMPLETE
REMOVAL_MODEL = COMPLETE
FCR0016_CONSUMPTION_BOUNDARY = EXPLICIT
APP_RSC_MANIFEST_FOUNDATION_AUTHORITY_COLLISION = 0
```

## 29. Non-grant

Acceptance of P0-E would establish identity/manifest/lifecycle design only. It would not grant activation, deployment, runtime routes, external connectivity, Paper, Shadow, Tiny-Live or Live.