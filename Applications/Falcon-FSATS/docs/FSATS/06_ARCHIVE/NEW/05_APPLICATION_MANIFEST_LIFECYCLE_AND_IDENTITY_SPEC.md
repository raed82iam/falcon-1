# FSATS Specialized Implementation Architecture — Application Manifest, Lifecycle and Identity Specification

**Package:** `FSATS-SIA-v0.1`
**Status:** `DESIGN_CANDIDATE`
**Governing Contracts:** APP-001 v1.1; CON-023 v1.1; ADR-I012 v1.1; ADR-I015 v1.0

## 1. Purpose

Materialize the CON-023 declaration requirements into an exact Application-side specification without copying Foundation Manifest implementation or inventing Foundation lifecycle semantics.

This document defines the Application-owned declaration content and invariants. The exact build-time type binding to `Foundation.ApplicationManifest` remains subject to the governed Foundation artifact-consumption boundary; source copying is forbidden.

## 2. Canonical Application Identity Record

Each FSATS Application SHALL have one immutable logical identity record:

```text
ApplicationIdentity
  ApplicationId                 // stable logical identity
  ApplicationCode               // APP-TRD | APP-PMA | APP-GRD | APP-SIM
  ApplicationName
  ApplicationVersion            // semantic package/application version
  OwnerDomain
  PurposeDigest                 // digest of governed purpose declaration
  PackageIdentity
  PackageProvenanceRef
  PackageIntegrityRef
  ManifestSchemaIdentity
  ManifestSchemaVersion
```

Rules:

- `ApplicationId` does not change for an ordinary compatible version update;
- purpose/owner changes are material identity changes and require governed review;
- package version is not Application authority;
- manifest validity does not imply admission/activation;
- display name never substitutes for ApplicationId.

## 3. Canonical CON-023 Declaration Sections

Every Application manifest candidate SHALL declare all of these sections explicitly:

1. Identity and purpose.
2. Package identity/provenance/integrity/compatibility.
3. Owned business boundary.
4. Explicit prohibited Foundation responsibilities.
5. Foundation dependencies and compatible versions/identities.
6. Provided Application capabilities/contracts.
7. Consumed Application capabilities/contracts.
8. Permissions and requested authority classes.
9. Security profile and credential-reference needs.
10. Resource minimum/desired/ceiling/degraded behavior.
11. Persistence requirements.
12. Communication/routes/events requirements.
13. Configuration classes and mutability.
14. Evidence/audit requirements.
15. Lifecycle/update/rollback/removal behavior.
16. Health/failure containment interfaces.
17. Exactly one MSA identity.
18. Complete major-branch + LSA set.
19. Optional CSA eligibility declarations.
20. Awareness self-development origin/escalation interfaces.
21. Guardian/protection interface declarations.
22. Rollback/corrective-action plan.

Omission is a validation failure, not permission.

## 4. Manifest Status Versus Lifecycle

Application-side records SHALL preserve these distinctions:

```text
PACKAGE_VALID != REGISTERED
REGISTERED != ADMITTED
ADMITTED != ACTIVATION_ELIGIBLE
ACTIVATION_ELIGIBLE != ACTIVE
ACTIVE != PAPER_AUTHORIZED
PAPER_AUTHORIZED != LIVE_AUTHORIZED
```

Environment/business authority is separately governed and SHALL NOT be encoded as a shortcut lifecycle transition.

## 5. Lifecycle State Consumption

The Application SHALL consume Foundation lifecycle state rather than own the canonical Foundation lifecycle machine.

Application-owned handlers MAY react to:

```text
IDENTIFIED
VALIDATED
REGISTERED
ADMISSION_REVIEWED
ACTIVATION_ELIGIBLE
ACTIVE
DEGRADED
SUSPENDED
ISOLATED
RECOVERING
UPDATE_PENDING
ROLLBACK
REMOVAL_PENDING
REMOVED
ARCHIVED
QUARANTINED / REJECTED where exposed by the Foundation contract
```

The Application must not self-transition the Foundation lifecycle state.

## 6. APP-TRD Manifest Candidate

### 6.1 Purpose

`APP-TRD` owns Trading-domain decision, risk, capital, portfolio, execution business workflow and Trading intelligence within explicit authority.

### 6.2 Major branches / LSAs

Exactly the following 13 major branches are declared:

- T-LSA-01 Operations, Account & Environment;
- T-LSA-02 Market & Instrument Universe;
- T-LSA-03 Analysis Frameworks;
- T-LSA-04 Classical Trading School;
- T-LSA-05 Opportunity Hunting School;
- T-LSA-06 Strategy Orchestration & Decision;
- T-LSA-07 Unified Risk Management;
- T-LSA-08 Portfolio & Capital Management;
- T-LSA-09 Execution & Position Lifecycle;
- T-LSA-10 Trading Learning & Knowledge;
- T-LSA-11 Trading Analytics & Attribution;
- T-LSA-12 Strategy Evolution & Experimentation;
- T-LSA-13 Trading Resource Awareness & Evaluation.

Exactly one `MSA-TRD` is declared.

### 6.3 Required consumed Application capabilities

- normalized operational Data Products from APP-PMA;
- Provider/Data Quality state projections from APP-PMA;
- Guardian restriction/protection directives from APP-GRD;
- Guardian incident/recovery status relevant to Trading scope;
- non-authoritative simulation/validation evidence from APP-SIM;
- FSARM effective resource outcomes/pressure projections when available.

### 6.4 Provided Application capabilities

- trade/risk/capital/execution state projections for authorized consumers;
- Guardian observation/protection evidence;
- FSAPMA data-demand profile/consumption feedback;
- FSTSimA validation inputs/candidate definitions without production authority;
- FSARM Trading resource demand/minimum/reclaimability/degradation evidence.

### 6.5 Explicit prohibited responsibilities

APP-TRD SHALL NOT declare itself owner of:

- Foundation lifecycle/admission;
- Foundation resource grants/ceilings;
- Foundation Service Bus/FIL/event infrastructure;
- external provider operational data acquisition;
- Foundation FSA;
- Guardian independent protection authority;
- FSATS-wide resource redistribution.

## 7. APP-PMA Manifest Candidate

Exactly one `MSA-PMA` and six LSAs are declared.

Consumed capabilities:

- Application data-demand/subscription requirements;
- Guardian provider-route restriction/isolation directives where authorized;
- FSARM effective resource outcomes;
- Foundation provider-egress/credential-reference capability only when FCR-0013 becomes available.

Provided capabilities:

- canonical Data Products;
- data-quality/reconciliation outcomes;
- provider capability/entitlement state;
- provider route/health/availability projections;
- provider quota/cost/reliability projections;
- FSARM resource reports.

Prohibited responsibilities:

- Trading decisions/risk/capital;
- Foundation network/credential authority;
- Foundation technical resource truth;
- Guardian business authority;
- broker order execution.

## 8. APP-GRD Manifest Candidate

Exactly one `MSA-GRD` and four LSAs are declared.

Consumed capabilities:

- Trading exposure/order/position/risk evidence required for protection;
- FSAPMA data/provider health evidence required for protection;
- Foundation transport/security/lifecycle/resource signals where contractually exposed;
- FSARM effective resource state/outcomes.

Provided capabilities:

- protection incidents;
- scoped protection restrictions/directives;
- crisis state/protection coordination requests;
- recovery/protection evidence;
- FSARM crisis resource need/minimum/consequence evidence.

Prohibited responsibilities:

- ordinary Trading alpha/risk/portfolio optimization;
- provider normalization/routing business ownership;
- Foundation Guardian/FSA;
- direct Foundation resource seizure;
- FSARM general resource coordination.

## 9. APP-SIM Manifest Candidate

Exactly one `MSA-SIM` and eight LSAs are declared.

Consumed capabilities:

- versioned candidate strategy/model/config definitions;
- sanitized/replayable historical Data Products where authorized;
- historical execution evidence where authorized;
- scenario/fault definitions;
- FSARM effective resource outcomes.

Provided capabilities:

- simulation results;
- shadow/replay outcomes;
- fidelity/calibration evidence;
- independent validation assessment;
- reproducibility evidence;
- resource reclaimability/checkpoint/restoration evidence.

Prohibited responsibilities:

- Live operational market/broker credential use;
- production order execution;
- production adoption/promotion;
- authoritative Trading capital/position state;
- falsifying simulation output as operational truth.

FCR-0011 remains the Foundation-owned future non-Live isolation/egress enforcement dependency.

## 10. Foundation Dependency Declaration Pattern

Every Foundation dependency SHALL be declared with:

```text
DependencySemanticId
RequiredCapability
MinimumCompatibleVersion/Identity
AcceptedEvidenceIdentity or governed pin when available
ConsumptionMode = DESIGN_ONLY | BUILD_REQUIRED | RUNTIME_REQUIRED
CurrentAvailability = AVAILABLE | PARTIAL | FUTURE | UNKNOWN
FCR if any
FailClosedBehavior
RevalidationTrigger
```

Moving branch HEAD SHALL NOT be the only dependency identity.

## 11. Cross-Application Contract Declaration Pattern

For every provided/consumed cross-Application family:

```text
ContractFamilyId
Direction
ProducerApplicationId
ConsumerApplicationId
PayloadSchemaId + Version
BusinessMeaningOwner
AuthorityClass
SecurityClass
OperationalClassification
RouteRequirement
Expiry/DeadlineRule
IdempotencyRule
ReplayRule
FailureRule
EvidenceRule
```

Declarations SHALL be generated from the canonical contract catalog rather than duplicated by hand in each manifest.

## 12. Resource Declaration Pattern

Each Application SHALL declare a resource profile by class:

```text
ResourceClass
MinimumSafe
DesiredNormal
MaximumUseful
ReclaimabilityClass
DegradedBehavior
CheckpointRequirement
RestorationRequirement
ProtectionConsequences
BusinessPriorityEvidenceSource
```

Values may be configured/versioned, but the semantic meaning is fixed.

`DesiredNormal` and `MaximumUseful` are requests/constraints, not Foundation grants.

## 13. Security Declaration Pattern

Each Application SHALL declare:

- allowed contract families;
- requested technical communication scope;
- data sensitivity classes;
- credential-reference needs;
- external egress need by purpose;
- broker/provider endpoint categories where future Foundation egress is required;
- prohibited egress categories;
- evidence retention/redaction classes;
- administrative/configuration authority boundaries.

Secrets/credentials SHALL NOT be embedded in the manifest.

## 14. Awareness Declaration Pattern

Per Application:

```text
ExactlyOneMSA
MajorBranches[] each ExactlyOneLSA
CSAEligibilityPolicy
EligibleComponents[]
MonitorAIPerspectives = 2 for each current FSATS MSA under accepted direction
SelfDevelopmentAllowedPurposes
ProtectedProperties
OriginAwareEscalationPaths
MSA->FSA binding = PENDING FOUNDATION FCR-0030 when runtime binding required
```

A CSA identity is not declared for deterministic/passive components merely to increase AI coverage.

## 15. Update Compatibility Classes

Application package updates are classified:

```text
PATCH_COMPATIBLE
BACKWARD_COMPATIBLE_MINOR
CONTRACT_AFFECTING
STATE_MIGRATION_REQUIRED
AUTHORITY_AFFECTING
PURPOSE_OR_OWNER_IDENTITY_CHANGE
BREAKING
```

Rules:

- `PATCH_COMPATIBLE` cannot alter observable contract/state/authority semantics;
- any contract/schema/state-machine change requires compatibility analysis;
- authority/purpose/owner changes require governed review and cannot be auto-promoted;
- state migration must be deterministic, reversible where required, and evidence-backed;
- inability to establish compatibility fails closed.

## 16. Application-Owned Pre-Removal Checks

Before Foundation lifecycle may safely complete removal, the Application shall be able to report/reconcile:

### APP-TRD
- no unreconciled open broker order obligations;
- positions/capital/settlement disposition explicitly handled;
- reservations released/reconciled;
- audit/evidence retained;
- cross-App subscriptions/routes drained.

### APP-PMA
- active consumers informed/degraded;
- provider sessions/leases closed as governed;
- outstanding data delivery/evidence reconciled;
- credential references released by Foundation path when applicable.

### APP-GRD
- no active protection directive silently abandoned;
- active incidents handed off/closed through governance;
- retained forensic/protection evidence preserved.

### APP-SIM
- frozen accepted evidence preserved;
- in-progress non-authoritative runs canceled/checkpointed;
- no production dependency relies on a running simulation as authority.

## 17. FSARM Manifest/Identity Gate

FSARM is intentionally **not** silently declared as a fifth Application in this file.

Its exact placement is a material architecture decision owned by `11_FSARM_SPECIALIZED_RESOURCE_ARCHITECTURE.md` and the Owner review gate.

Until that decision is accepted:

```text
FSARM_HIDDEN_APPLICATION = FORBIDDEN
FSARM_UNDECLARED_RUNTIME_PRINCIPAL = FORBIDDEN
FSARM_FOUNDATION_SERVICE_BY_APPLICATION_FIAT = FORBIDDEN
FSARM_EXACT_RUNTIME_IDENTITY = DESIGN_GATE
```

Other Application manifests SHALL declare their resource-report/coordination contract requirements against the logical FSARM role, but implementation wiring remains blocked until the exact FSARM principal/host identity is resolved.

## 18. Manifest Verification Families

A dedicated manifest verifier SHALL cover at minimum:

1. exactly four current accepted Application identities unless later Owner topology changes;
2. one MSA per Application;
3. exact LSA sets/counts;
4. no duplicate Application/LSA/contract identity;
5. complete CON-023 sections;
6. no undeclared dependency/route/authority;
7. no Foundation-owned responsibility claimed;
8. no cross-App direct internal dependency;
9. exact contract catalog consistency;
10. exact resource profile consistency;
11. exact security profile consistency;
12. FSTSimA non-Live declarations;
13. awareness origin/escalation consistency;
14. lifecycle/update/removal completeness;
15. no moving-HEAD-only Foundation dependency pin where immutable identity is required;
16. unresolved future Foundation capability maps to explicit fail-closed behavior;
17. FSARM runtime identity cannot be invented implicitly.
