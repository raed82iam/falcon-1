# FSATS V1.4 Part 0 / P0-E — Canonical Application Identity and CON-023 Manifest Design Contract

**Status:** `P0E_DESIGN_CANDIDATE`
**Scope:** six accepted Applications / 38 accepted major branches
**P0-E final Owner acceptance:** `NOT_GRANTED`

## 1. Purpose

This record converts the accepted P0-C topology labels into canonical design identities and complete CON-023 manifest obligations while preserving the P0-D Foundation/Application boundary.

It is a design contract, not an admitted Manifest, package, runtime instance, activation record, deployment authorization, or production approval.

## 2. Canonical namespace model

The canonical identity families are:

```text
Application ID: falcon.app.<domain>.<application>
Package ID:     falcon.pkg.<domain>.<application>
MSA ID:         falcon.sa.msa.<domain>.<application>
LSA ID:         falcon.sa.lsa.<domain>.<application>.<branch>
CSA ID:         falcon.sa.csa.<domain>.<application>.<branch>.<component>
```

Rules:

- Application ID is stable across ordinary package/version updates.
- Package ID identifies the package family; exact package version and digest identify one package artifact.
- Runtime instance identity is separate and shall be assigned by governed runtime/lifecycle mechanisms.
- Display names and acronyms are labels, not substitutes for canonical IDs.
- FSATS has no Application, Package, MSA, LSA or runtime-principal identity.
- Canonical IDs create no authority, route, permission, resource entitlement or lifecycle state.

## 3. Canonical Application and Package identities

| Application | Canonical Application ID | Canonical Package ID | Canonical MSA ID |
|---|---|---|---|
| Falcon Trading Guardian Application | `falcon.app.trading.guardian` | `falcon.pkg.trading.guardian` | `falcon.sa.msa.trading.guardian` |
| Falcon Self-Aware Provider Management Application | `falcon.app.trading.fsapma` | `falcon.pkg.trading.fsapma` | `falcon.sa.msa.trading.fsapma` |
| Falcon Self-Aware Trading Application | `falcon.app.trading.core` | `falcon.pkg.trading.core` | `falcon.sa.msa.trading.core` |
| Falcon Self-Aware Trading Simulation Application | `falcon.app.validation.fstsima` | `falcon.pkg.validation.fstsima` | `falcon.sa.msa.validation.fstsima` |
| Shared Communication Application | `falcon.app.shared.communication` | `falcon.pkg.shared.communication` | `falcon.sa.msa.shared.communication` |
| Shared Web Application | `falcon.app.shared.web` | `falcon.pkg.shared.web` | `falcon.sa.msa.shared.web` |

These six identities are mutually independent. `trading`, `validation`, and `shared` in the namespace are classification segments only and create no parent runtime owner.

## 4. Canonical LSA identities

### 4.1 Guardian

| Accepted branch | Canonical LSA ID |
|---|---|
| Protection State and Command Governance | `falcon.sa.lsa.trading.guardian.protection-command-governance` |
| Threat, Trigger and Crisis Assessment | `falcon.sa.lsa.trading.guardian.threat-crisis-assessment` |
| Recovery, Reconciliation and Release | `falcon.sa.lsa.trading.guardian.recovery-release` |
| Guardian Learning, Playbook and Protection Improvement | `falcon.sa.lsa.trading.guardian.learning-playbook-improvement` |

### 4.2 FSAPMA

| Accepted branch | Canonical LSA ID |
|---|---|
| Provider Registry and Capability Intelligence | `falcon.sa.lsa.trading.fsapma.provider-registry-capability` |
| Data Products and Data Service Contracts | `falcon.sa.lsa.trading.fsapma.data-products-contracts` |
| Provider Selection, Fallback and Business Route-Lease Planning | `falcon.sa.lsa.trading.fsapma.provider-selection-fallback` |
| Data Quality, Lineage and Provider Reconciliation | `falcon.sa.lsa.trading.fsapma.data-quality-lineage` |
| Provider/API Capacity, Quota and Cost Governance | `falcon.sa.lsa.trading.fsapma.provider-capacity-quota-cost` |
| External Service Role and Provider Onboarding Evidence | `falcon.sa.lsa.trading.fsapma.external-role-onboarding` |

### 4.3 Trading

| Accepted branch | Canonical LSA ID |
|---|---|
| Operations, Tenant, Account and Environment Control | `falcon.sa.lsa.trading.core.operations-context` |
| Market Profiles, Universe and Instrument Eligibility | `falcon.sa.lsa.trading.core.market-universe-eligibility` |
| Analysis Frameworks and Market Interpretation | `falcon.sa.lsa.trading.core.analysis-interpretation` |
| Trading Schools and Strategy Management | `falcon.sa.lsa.trading.core.schools-strategies` |
| Opportunity, Proposal and Decision Orchestration | `falcon.sa.lsa.trading.core.decision-orchestration` |
| Unified Risk Management | `falcon.sa.lsa.trading.core.unified-risk` |
| Portfolio and Trading Capital Allocation | `falcon.sa.lsa.trading.core.portfolio-capital` |
| Trading Intent and Horizon Governance | `falcon.sa.lsa.trading.core.intent-horizon` |
| Execution and Broker Interaction | `falcon.sa.lsa.trading.core.execution-broker` |
| Position, Fill Allocation and Reconciliation | `falcon.sa.lsa.trading.core.position-reconciliation` |
| Learning, Performance Attribution and Evolution | `falcon.sa.lsa.trading.core.learning-attribution-evolution` |
| Trading Business Continuity, Readiness and Runbooks | `falcon.sa.lsa.trading.core.business-readiness` |

### 4.4 FSTSimA

| Accepted branch | Canonical LSA ID |
|---|---|
| Simulation Run and Environment Control | `falcon.sa.lsa.validation.fstsima.run-environment` |
| Simulation Clock and Time Control | `falcon.sa.lsa.validation.fstsima.simulation-clock` |
| Market Data, Replay and Scenario Feed Simulation | `falcon.sa.lsa.validation.fstsima.market-replay-feed` |
| Provider Simulation | `falcon.sa.lsa.validation.fstsima.provider-simulation` |
| Broker and Execution Simulation | `falcon.sa.lsa.validation.fstsima.broker-execution-simulation` |
| Account, Portfolio and Capital Simulation | `falcon.sa.lsa.validation.fstsima.account-capital-simulation` |
| Fault, Stress and Adversarial Scenario Injection | `falcon.sa.lsa.validation.fstsima.fault-stress-injection` |
| Fidelity, Oracle, Evidence and Validation Assessment | `falcon.sa.lsa.validation.fstsima.fidelity-validation` |

### 4.5 Shared Communication

| Accepted branch | Canonical LSA ID |
|---|---|
| Notification Intake and Source-Truth Boundary | `falcon.sa.lsa.shared.communication.intake-source-truth` |
| Rendering, Templates and Localization | `falcon.sa.lsa.shared.communication.rendering-localization` |
| External Channel Policy and Delivery | `falcon.sa.lsa.shared.communication.channel-delivery` |
| Recipient, Acknowledgement, Escalation and Delivery Evidence | `falcon.sa.lsa.shared.communication.recipient-ack-evidence` |

### 4.6 Shared Web

| Accepted branch | Canonical LSA ID |
|---|---|
| Web Shell, Module Composition and Navigation | `falcon.sa.lsa.shared.web.shell-navigation` |
| Read Models and Presentation Projection | `falcon.sa.lsa.shared.web.read-projection` |
| User Commands, Consent and Interaction | `falcon.sa.lsa.shared.web.command-consent` |
| Session, Entitlement Context and Localization Presentation | `falcon.sa.lsa.shared.web.session-entitlement-presentation` |

## 5. CSA identity and declaration rule

P0-E creates no CSA instance automatically.

A CSA may be declared only when:

1. its parent branch is CSA-eligible under accepted P0-C policy;
2. an exact intelligent component identity exists;
3. component responsibility, measurable output and bounded improvement surface are declared;
4. authoritative state ownership remains with the approved branch/Application owner;
5. the CSA self-development authority and prohibitions are explicit;
6. the origin-aware review route is declared;
7. required FSA/Foundation dependencies are available or fail-closed by FCR.

Canonical CSA identity shall use:

`falcon.sa.csa.<domain>.<application>.<branch>.<component>`

No CSA may be created for symmetry, deterministic wrappers, passive storage, simple adapters or ordinary registries without a new justified manifest/topology change.

## 6. Common Manifest header

Every one of the six manifests SHALL declare, without null/implicit authority-bearing defaults:

```text
ManifestSchema = CON-023 v1.1 or later explicitly compatible approved version
ApplicationId = exact canonical ID
ApplicationDisplayName = exact human-readable name
ApplicationVersion = exact immutable version for the declared package
PackageId = exact canonical package ID
PackageVersion = exact immutable package version
PackageDigest = exact governed integrity digest
PackageProvenance = exact source/build/release evidence reference
Owner = accountable Application owner identity/role
Purpose = bounded approved purpose
LifecycleState = exact APP-001 state, not inferred
Compatibility = explicit supported Foundation/contract versions
```

### Version rule

P0-E does not fabricate a release number before the implementation/release artifact exists. Instead it establishes a fail-closed requirement:

```text
APPLICATION_VERSION = REQUIRED_EXACT_VALUE_BEFORE_VALIDATION
PACKAGE_VERSION = REQUIRED_EXACT_VALUE_BEFORE_VALIDATION
PACKAGE_DIGEST = REQUIRED_EXACT_VALUE_BEFORE_VALIDATION
```

A placeholder, branch name, `latest`, build date alone, or FSATS design version SHALL NOT satisfy runtime Manifest validity.

## 7. Common business-boundary declaration

Each Manifest SHALL declare:

- owned business responsibilities;
- authoritative business truth families it owns;
- responsibilities explicitly not owned;
- prohibited Foundation responsibilities;
- prohibited sibling-Application internals;
- prohibited inference of authority from FSATS membership, awareness rank, shared use or successful prior behavior.

Foundation responsibilities including lifecycle, admission, generic routing/delivery, global technical resource governance, Foundation security authority and Foundation-owned platform truth SHALL NOT be claimed by an Application.

## 8. Common dependency declaration

Every dependency SHALL identify:

- exact dependency/capability/contract ID;
- compatible version/range policy;
- required/optional status;
- purpose;
- authority basis;
- unavailable/degraded behavior;
- FCR identity when the required Foundation capability is partial/missing;
- whether absence blocks validation, activation, a feature, or only a non-authoritative optional function.

Unknown dependency versions SHALL fail validation rather than silently resolving to `latest`.

## 9. Common Foundation-contract classes

Where used, the Manifest SHALL declare exact approved contract identifiers at package-validation time. Candidate classes include:

- `CON-001` identity;
- `CON-002` authority decision;
- `CON-003` lifecycle;
- `CON-004` FIL envelope;
- `CON-005` event;
- `CON-006` health/fitness;
- `CON-007` configuration;
- `CON-008` evidence/logging;
- `CON-009` security context;
- `CON-011` protective restriction where applicable;
- `CON-012` authority instrument where applicable;
- `CON-013` delegation/revocation where applicable;
- `CON-014` identifier provider;
- `CON-015` time provider;
- `CON-017` secret provider where applicable;
- `CON-018` certificate/identity provider where applicable;
- `CON-023` Application Manifest.

Listing a contract does not create permission or runtime availability.

## 10. Common permissions/security declaration

No Application uses wildcard permissions.

Each permission request SHALL declare:

- permission identity;
- target capability/resource/route class;
- purpose;
- scope;
- environment;
- authority source;
- expiry/revocation behavior where applicable;
- fail-closed behavior when denied/unknown;
- evidence requirement.

Security profiles SHALL declare credential classes needed, forbidden credential classes, secret-access boundaries, external-egress requirements, network/route restrictions, isolation requirements and audit/evidence obligations.

No Manifest stores raw secrets.

## 11. Common resource declaration

Each Application SHALL declare independently:

- resource classes required;
- minimum viable allocation;
- normal requested allocation;
- hard ceilings requested;
- technical priority requests where legitimate;
- degraded behavior below normal allocation;
- fail-closed threshold where continued operation is unsafe/untrustworthy;
- resource-pressure handling;
- restoration behavior.

Applications may distribute only their admitted allocation internally. They may not read or consume sibling allocation truth unless explicitly exposed through a governed Foundation contract.

Financial/trading capital is not a Foundation technical resource.

Exact numeric technical-resource values SHALL be supplied and evidenced during implementation/performance sizing before activation. Missing values cannot be replaced by unlimited/default capacity.

## 12. Common persistence/configuration/evidence declaration

Each Manifest SHALL declare:

- owned persistent state families;
- retention class/policy reference;
- migration behavior;
- consistency/recovery requirements;
- configuration identity/versioning;
- immutable evidence/provenance references;
- backup/restore or approved corrective-action requirements where applicable;
- data classification/security constraints.

Application persistence shall not silently become a shared cross-Application database.

## 13. Common communication declaration

Each Manifest SHALL declare every cross-Application communication family needed by the Application, but P0-E does not create route authority.

For each family declare at minimum:

- producer/requester;
- consumer/responder class;
- purpose;
- message/data-product/event family;
- schema/version policy;
- required authority/permission reference;
- freshness/deadline where material;
- failure/degraded behavior;
- evidence/correlation/causation requirements;
- FCR dependency where Foundation support remains partial/missing.

Exact cross-Application contract semantics are completed in P0-F.

## 14. Common health and failure-containment declaration

Each Application SHALL expose Foundation-compatible health/fitness evidence without transferring business ownership to Foundation.

The Manifest SHALL declare:

- technical health interface;
- business readiness interface separately where applicable;
- failure containment boundary;
- degraded state behavior;
- isolation-safe behavior;
- stale/unknown dependency behavior;
- restart/recovery prerequisites;
- whether new business actions are prohibited under uncertainty.

Application failure SHALL NOT require Foundation redesign or sibling-Application failure.

## 15. Common APP-001 lifecycle contract

All six Applications SHALL support the APP-001 lifecycle independently:

```text
PACKAGE_RECEIVED
→ IDENTIFIED
→ VALIDATED
→ REGISTERED
→ ADMISSION_REVIEWED
→ ACTIVATION_ELIGIBLE
→ ACTIVE
```

And governed outcomes including:

```text
REJECTED
QUARANTINED
SUSPENDED
DEGRADED
ISOLATED
RECOVERING
UPDATE_PENDING
ROLLBACK
REMOVAL_PENDING
REMOVED
ARCHIVED
```

No state implies the next.

### Installation/identification

- verify exact Application ID, Package ID, versions, provenance and integrity;
- reject identity collision or purpose/owner mismatch.

### Validation

- validate complete CON-023 declaration;
- validate dependency compatibility;
- validate awareness topology against accepted identities;
- validate permission/security/resource declarations;
- validate rollback/corrective-action and removal plan;
- fail closed on unresolved required FCR/runtime dependency.

### Registration

Registration records identity and declared interfaces only. It grants no business authority or activation.

### Admission

Admission is a distinct Foundation/governance decision against current state, authority, dependencies, resources, security and compatibility.

### Activation

Activation requires separate valid authority. `ACTIVATION_ELIGIBLE` is not `ACTIVE`.

### Suspension/isolation/degradation

The Application SHALL define safe business behavior and preserve evidence/state integrity. Isolation must not create hidden cross-Application recovery access.

### Update

Update SHALL bind exact prior and next package identities and preserve/reconcile compatibility, state migration, permissions, dependencies, routes, evidence and rollback/corrective action.

### Recovery

Foundation coordinates platform lifecycle recovery; Application owns internal business recovery. Return to unrestricted business action requires required evidence/authority.

### Replacement/removal

Removal SHALL reconcile routes, permissions, resources, state, dependencies, evidence and retained records. No dependent Application may be silently broken or given substitute authority.

## 16. Self-development declaration

Every Manifest SHALL declare the actual origin-aware paths:

```text
CSA-origin → Parent LSA → Application MSA → FSA governance review
LSA-origin → Application MSA → FSA governance review
MSA-origin → FSA governance review
```

FSA is not an Application business decision layer. Production-bound promotion remains governed by accepted P0-C autonomous-evolution semantics and available Foundation capability/FCR state.

FCR-0012 remains blocking for runtime bounded autonomous-promotion/Owner-absence control-plane behavior until Foundation disposition/implementation/application verification is complete.

## 17. Guardian/protection declaration

Every Manifest SHALL explicitly state whether the Application:

- is the Falcon Trading Guardian Application;
- consumes Guardian protection state/commands;
- supplies evidence/status used by Guardian;
- has no Guardian dependency.

No Application may infer Guardian authority from naming or FSATS membership.

Guardian command transport, if used, depends on governed contracts/routes and applicable open FCR state. Guardian cannot directly mutate sibling internals.

## 18. FCR fail-closed rule

A Manifest may reference a partial/missing Foundation capability through its canonical FCR but SHALL NOT represent that capability as implemented or runtime available.

For each such dependency, the Manifest SHALL declare one of:

```text
BLOCK_PACKAGE_VALIDATION
BLOCK_ADMISSION
BLOCK_ACTIVATION
BLOCK_FEATURE_ONLY
OPTIONAL_NON_AUTHORITATIVE
```

Runtime behavior depending on an unavailable required capability is disabled/fail-closed. No Application-local substitute may claim the missing Foundation authority.

## 19. Application-specific manifest obligations

### 19.1 Guardian — `falcon.app.trading.guardian`

**Purpose:** trading-domain threat/crisis assessment, scoped protection-state/command governance, recovery/release, and Guardian learning/playbook improvement.

**Owns:** Guardian protection state, threat assessment, scoped protection-command intent, Guardian recovery/release evidence, Guardian learning candidates.

**Does not own:** Trading normal Risk, Trading execution/positions, FSAPMA provider truth, Foundation lifecycle/routing/resources/security.

**Primary Foundation/FCR dependencies:** APP-001, CON-023, governed communication/admission/routing/delivery/event/evidence, FCR-0004 for complete protection-command route semantics, FCR-0007 for resource escalation request boundary where used.

**Protection relationship:** authority source for accepted Guardian-domain protection outcomes only; transport does not enlarge command scope.

**Degraded rule:** uncertainty cannot create more Trading authority. When required Guardian truth is unavailable/stale, dependent Applications must follow declared safe/restrictive behavior.

### 19.2 FSAPMA — `falcon.app.trading.fsapma`

**Purpose:** sole trading operational external-data/provider-management gateway.

**Owns:** provider registry/capabilities, trading data products/contracts, provider selection/fallback business logic, provider data quality/lineage/reconciliation, provider API quota/cost state, provider onboarding/service-role evidence.

**Does not own:** broker execution truth, Trading decisions, Foundation connectivity/routing/resources/security.

**Primary Foundation/FCR dependencies:** APP-001, CON-023, governed communication/admission/routing/delivery/event/evidence, external connectivity/security permission, FCR-0005 for complete operational data delivery boundary where unresolved, FCR-0008 only for awareness research egress (not operational provider data).

**Operational-data rule:** operational market/provider data used by Trading/Guardian crosses only through declared FSAPMA business contracts plus governed Foundation transport. Awareness research Internet is not an operational-data source.

### 19.3 Trading — `falcon.app.trading.core`

**Purpose:** complete Trading business operation: operating context, market/universe eligibility, analysis, schools/strategies, decision orchestration, Unified Risk, portfolio/capital, intent/horizons, execution/broker interaction, position reconciliation, learning/evolution, and Trading business readiness.

**Owns:** Trading business truths assigned in accepted P0-C.

**Does not own:** Foundation lifecycle/transport/resources/security, provider acquisition, Guardian protection state.

**Primary Foundation/FCR dependencies:** APP-001, CON-023, governed communication/admission/routing/delivery/event/evidence; FSAPMA and Guardian business contracts later defined in P0-F; FCR-0009/0010 where performance/resource-pressure semantics remain incomplete; FCR-0012 for bounded autonomous-evolution runtime governance.

**Risk rule:** dynamic Trading Risk remains Application-owned; Foundation/FSA review does not calculate or choose Trading Risk.

**Degraded rule:** insufficient operational truth, authority, Guardian protection state, broker truth, reconciliation or required Foundation capability must reduce authority and may deny new exposure.

### 19.4 FSTSimA — `falcon.app.validation.fstsima`

**Purpose:** independent non-Live simulation, replay, stress, adversarial validation and evidence generation.

**Owns:** simulation truths only.

**Does not own:** Live market truth, Live broker authority, production state, Owner adoption authority.

**Primary Foundation/FCR dependencies:** APP-001, CON-023, governed communication/event/evidence, FCR-0006 as applicable to replay/event semantics until complete, FCR-0011 for enforced non-Live credential/route/egress isolation.

**Mandatory security profile:** Live credentials/routes/authoritative egress are forbidden. Ambiguous environment authority fails closed.

**Evidence rule:** simulation/replay PASS creates evidence only, never Live or production authority.

### 19.5 Shared Communication — `falcon.app.shared.communication`

**Purpose:** governed intake of notification/report requests, rendering/localization, external-channel policy/delivery, recipient/ack/escalation and delivery evidence.

**Owns:** communication-delivery business state only.

**Does not own:** source Application business truth/severity, Foundation Service Bus, Trading/Guardian state.

**Primary Foundation dependencies:** APP-001, CON-023, governed communication/admission/routing/delivery/event/evidence, external channel egress/security permissions as separately declared.

**Source-truth rule:** rendering or channel behavior cannot silently reinterpret source business meaning or create source authority.

### 19.6 Shared Web — `falcon.app.shared.web`

**Purpose:** Web shell/module composition, governed presentation projections, user command/consent capture, session/entitlement-context presentation and localization.

**Owns:** UI/presentation state and user interaction capture only.

**Does not own:** authentication/authorization source truth, Trading/provider/Guardian business truth, Foundation lifecycle/routing/security authority.

**Primary Foundation dependencies:** APP-001, CON-023, identity/security context, governed communication/event/evidence, session/entitlement information through declared authoritative interfaces.

**Command rule:** UI action is request/intent evidence only. Shared Web cannot create business authority, Owner governance authority or execute sibling business actions directly.

## 20. Package provenance and build-binding requirements

Before validation, each exact package SHALL bind:

```text
ApplicationId
ApplicationVersion
PackageId
PackageVersion
SourceCommit
BuildIdentity
ArtifactDigest
ManifestDigest
DependencyLock/ResolvedDependencyEvidence
Build/ValidationEvidenceReferences
```

Any mismatch between declared identity and actual artifact fails validation.

Rebuilding different bytes under the same immutable package version/digest identity is prohibited.

## 21. Compatibility and migration rules

- Compatibility must be explicit; absence of known incompatibility is not compatibility.
- A dependency version outside declared compatibility cannot be silently admitted.
- Schema/contract upgrade may require dual-read/dual-write or migration only when specifically designed and authorized; P0-E does not authorize such runtime techniques by default.
- State migration must preserve authoritative identity, provenance and rollback/corrective-action truth.
- Purpose/ownership changes require material identity/governance review, not ordinary version update.

## 22. Removal and orphan-prevention rules

Before removal/replacement, the lifecycle plan must prove:

- no active route/authority silently targets the removed identity;
- permissions/secrets/resources are revoked/reconciled;
- durable state is retained/migrated/disposed according to policy;
- dependent Applications receive explicit compatible outcome or are blocked safely;
- evidence remains reconstructable;
- MSA/LSA identities do not remain active as orphan authorities;
- removal does not mutate Foundation design.

## 23. Downstream boundaries

P0-E defines what the six manifests must declare.

It does not finalize:

- cross-Application payload/contract semantics: P0-F;
- FSAPMA operational data architecture: P0-G;
- detailed Trading business architecture: P0-H;
- Guardian/crisis/resource escalation behavior: P0-I;
- performance/QoS/load-shedding details: P0-J;
- FSTSimA validation/environment proof: P0-K.

Those later work packages may add exact declarations but may not contradict or silently omit the P0-E identity/lifecycle invariants.

## 24. P0-E candidate state

```text
CANONICAL_APPLICATION_IDS = 6
CANONICAL_PACKAGE_IDS = 6
CANONICAL_MSA_IDS = 6
CANONICAL_LSA_IDS = 38
CSA_INSTANCES_CREATED = 0
CON023_REQUIRED_FIELD_FAMILIES = COVERED
APP001_LIFECYCLE = COVERED
P0E_ARCHITECTURE_REVIEW = REQUIRED
P0E_RED_TEAM = REQUIRED
P0E_FINAL_OWNER_ACCEPTANCE = NOT_GRANTED
P0F_THROUGH_P0L = NOT_STARTED
```
