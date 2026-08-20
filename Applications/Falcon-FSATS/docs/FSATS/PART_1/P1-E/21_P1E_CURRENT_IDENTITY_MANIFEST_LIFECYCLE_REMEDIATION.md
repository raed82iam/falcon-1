# P1-E — Current Identity, Manifest and Lifecycle Remediation

**Status:** `DESIGN_CANDIDATE / NOT_OWNER_ACCEPTED / NOT_CLOSED`  
**Branch:** `application-development`  
**Implementation Authority:** `NOT_GRANTED`  
**Runtime Authority:** `NOT_GRANTED`

## Purpose

This record prospectively corrects and completes P1-E after the Owner-accepted APP-RSC fifth-Application decision, Owner-accepted P1-C topology, Owner-accepted P1-D primitive/type ownership, Safety Continuity V2, and AI Repair / Controlled Recovery V3.

Historical P1-E records remain preserved. Where they classify FSARM as a non-Application coordinator, this record and the later APP-RSC Owner records control prospectively.

## Current Application Set

FSATS remains a non-owning, non-runtime system boundary. The current FSATS Application set is exactly:

1. Falcon Self-Aware Trading Application
2. Falcon Self-Aware Provider Management Application (FSAPMA)
3. Falcon Trading Guardian Application
4. Falcon Self-Aware Trading Simulation Application (FSTSimA)
5. APP-RSC — Falcon Self-Aware Resource Management Application

`FSATS_SYSTEM` is not an Application, runtime principal, lifecycle owner, authority owner, state owner, resource owner, Manifest owner or package principal.

## Per-Application Manifest Minimum

Each of the five Applications SHALL independently declare and preserve at minimum:

- immutable Application identity, canonical name, version, owner and purpose;
- deployable package identity and version;
- provenance, integrity and compatibility evidence;
- owned business boundary and explicit non-responsibilities;
- exact MSA identity;
- exact current major-branch LSA identities;
- CSA eligibility policy;
- provided and consumed capability/contract declarations;
- Foundation dependency declarations;
- permission/security profile;
- Application resource profile including minimum-safe, desired, ceiling/useful-bound, pressure, reclaimability, degraded/shedding and restoration semantics where applicable;
- persistence/configuration/evidence requirements;
- lifecycle, health, containment, recovery, rollback/corrective-action, replacement and removal behavior;
- Guardian/protection interface as applicable;
- origin-aware self-development route;
- Safety Continuity declaration;
- AI repair/recovery authority declaration.

`DECLARED != GRANTED`, `VALID != ADMITTED`, `ADMITTED != ACTIVE`, and `ACTIVE != PRODUCTION_AUTHORIZED` remain mandatory.

## Package Identity Binding

The Owner-accepted P1-C package identities are the physical package identities for future materialization:

- `Falcon.FSATS.Trading`
- `Falcon.FSATS.FSAPMA`
- `Falcon.FSATS.TradingGuardian`
- `Falcon.FSATS.FSTSimA`
- `Falcon.FSATS.ResourceManagement`

Corresponding producer-owned public contract packages use the `*.Contracts` package identity. Package/project identity does not replace Application identity or create authority.

## P1-D Primitive Ownership Binding

Manifest fields and later schemas SHALL use the Owner-accepted P1-D ownership rules:

- Foundation-owned semantics are referenced/consumed, never cloned;
- cross-Application contract semantics remain producer-owned;
- FSAPMA operational instrument identity and Trading business/domain instrument identity remain distinct and explicitly mapped;
- financial values preserve exact unit/precision semantics;
- `ABSENT != ZERO != UNKNOWN != NOT_APPLICABLE`;
- authoritative external references preserve issuer/namespace/context;
- simulation identities cannot masquerade as operational identities;
- APP-RSC resource evidence does not become Foundation grant/resource truth;
- no ownerless `FSATS.Common` runtime semantic package is created.

## Application Lifecycle

Every Application binds to APP-001 lifecycle semantics and shall support governed outcomes including rejected, quarantined, suspended, degraded, isolated, recovering, update-pending, rollback, removal-pending, removed and archived.

Application lifecycle state and internal AI/Awareness trust state are related but distinct.

```text
AI_KILLED != APPLICATION_REMOVED
AI_KILLED != AUTOMATIC_APPLICATION_SHUTDOWN
APPLICATION_ACTIVE != ALL_INTERNAL_AI_TRUSTED
RESTARTED != RECOVERED
```

## Safety Continuity Manifest Requirement

Each Application SHALL declare its AI-independent safety/degraded-continuity behavior sufficient for its own responsibility. The exact behavior remains Application-owned.

For any killed/untrusted intelligent scope:

- containment targets the minimum proven-sufficient scope;
- unknown trust blast radius expands containment;
- stale/queued/cached/scheduled work derived from revoked intelligence is fenced according to causation/epoch rules to be materialized later;
- unaffected trusted functions may continue only inside their existing authority;
- functions requiring the killed intelligence and lacking a trusted fallback fail closed;
- existing obligations remain owned and monitored rather than becoming orphaned;
- continuity state required for safe recovery shall be reconstructable outside the killed subject's volatile memory;
- restart alone never restores trust.

For Trading specifically, existing exposure protection/reconciliation obligations SHALL remain governed during AI containment. No Manifest may claim that loss is impossible or that broker-side protection is infallible.

## AI Repair / Controlled Recovery Declaration

Each Application SHALL declare the repair/recovery authority path for intelligent components using:

```text
DETECT
-> CONTAIN
-> INVESTIGATE
-> REPAIR IN ISOLATION
-> INDEPENDENT VALIDATION
-> CONTROLLED REVIVAL
```

Recovery classes SHALL preserve the Owner-accepted intent:

- `R1`: minor/proven-local, pre-authorized non-semantic restoration only, using a currently valid/non-revoked/compatible approved state or corrective action; bounded attempts and automatic escalation on recurrence/probation failure;
- `R2`: material repair, including new code/model/behavior or material intelligent-state change; Falcon may prepare and validate the repair, but Owner approval is required before Controlled Revival;
- `R3`: critical/unknown trust failure, including authority/permission/core-architecture change, evidence tampering, containment/monitor interference, inability to establish a trusted baseline, or unknown/wide blast radius; Owner/governance decision is mandatory before revival.

`OWNER_ATTENTION != OWNER_MANUAL_REPAIR`.

A killed/untrusted subject SHALL NOT be the sole authority that diagnoses itself, approves its own repair, restores its own trust, or releases itself from containment.

## Trading Application

Trading remains one Application with one MSA and 13 current LSAs. Its Manifest owns Trading decisions, Trading Risk, portfolio/capital management, execution/position lifecycle, strategy orchestration, Trading learning/analytics/evolution, and Trading-side resource evidence. It does not own FSAPMA provider truth, Guardian protection authority, APP-RSC resource coordination authority, Foundation governance, or FSA.

Trading Manifest SHALL declare the safety owner and recovery obligations for open positions, orders, partial fills, protection state and reconciliation during AI degradation.

## FSAPMA

FSAPMA remains one Application with one MSA and 6 current LSAs. It owns provider registry/onboarding, data products/normalization, capability/entitlement, provider selection/routing/delivery, quality/reconciliation, and quota/cost/reliability semantics.

Its degraded-continuity declaration SHALL distinguish adaptive/intelligent provider selection from independently trustworthy deterministic quota, freshness, validation, reconciliation and pre-approved failover behavior where such behavior is later authorized.

Provider credential references remain FSAPMA-consumed for operational-data roles; secret bytes do not become Shared Web-owned state.

## Trading Guardian

Guardian remains one Application with one MSA and 4 current LSAs. Its Manifest owns Trading protection/crisis semantics but does not become Trading Risk, execution truth, provider truth, APP-RSC or Foundation governance.

Guardian SHALL distinguish AI-assisted protection intelligence from independently trustworthy deterministic protection controls. Kill of Guardian AI does not by itself revoke valid deterministic hard protections that remain outside the affected trust blast radius.

## FSTSimA

FSTSimA remains one non-Live Application with one MSA and 8 current LSAs. Its Manifest SHALL preserve non-Live identity/isolation, deterministic/reproducible evidence, and explicit prohibition against simulation/replay identity masquerading as operational authority.

Its fault/crisis injection and recovery validation may provide evidence for AI containment/recovery verification without itself granting production adoption or Controlled Revival authority.

## APP-RSC

APP-RSC is the fifth independent Falcon Application:

```text
APPLICATION_ID = APP-RSC
CANONICAL_NAME = Falcon Self-Aware Resource Management Application
SCOPE = FSATS_ONLY
MSA = 1
LSA = 3
CSA = 0 initially
```

Current branches:

- R-LSA-01 Resource Picture, Demand Integrity and Coordination Envelope
- R-LSA-02 Redistribution, Degradation and Rebalance
- R-LSA-03 Foundation Binding, Restoration and Resource Evidence

`MSA_RSC != RESOURCE_STRATEGY_CONTROLLER` and `AWARENESS != OPERATIONAL_CONTROL`.

APP-RSC owns FSATS-only effective resource coordination inside the valid Foundation envelope. It does not own Foundation authoritative grants, ceilings, floors, total-resource truth or non-FSATS resources.

Its degraded-continuity Manifest SHALL freeze or restrict intelligent redistribution when APP-RSC intelligence is killed/untrusted, preserve valid Foundation envelope/protected minima and attribution, deny peer resource seizure, and permit only independently authorized deterministic protection/restoration behavior.

## Cross-Application / Web / Foundation Boundary

No Manifest may declare direct access to another Application's internals. Cross-Application and Shared Web interaction uses governed producer-owned contracts and admitted routes.

Current FCR-0080 establishes that a generic Foundation communication boundary exists; exact P1-K producer/consumer/schema/FIL/route/authority bindings remain pending and shall fail closed until materialized.

FCR-0031 confirms APP-RSC identity compatibility with the existing Foundation Stage 6 resource boundary. Final implementation/binding verification remains a later Application hold.

FCR-0082 confirms planning compatibility for generic AI/FSA safety continuity while exact generic Foundation runtime realization remains future Foundation work. This does not block current Application design.

## Removal / Replacement

Removal or replacement of any Application SHALL reconcile at minimum:

- identity and lifecycle state;
- contracts/routes/subscriptions;
- permissions and delegated authority;
- resources and APP-RSC constituent scope where applicable;
- persisted state and migration/retention obligations;
- evidence/audit;
- open obligations and safety ownership;
- containment/recovery state;
- stale epochs/fencing;
- dependency impact.

No sibling Application inherits removed Application authority by default. Removal/replacement must not require Foundation redesign.

## P1-E Completion Gate

P1-E can be Owner-accepted only after a fresh exact semantic freeze, Architecture/Consistency review and Red-Team review confirm that all five Applications can be represented by complete, independent, internally consistent Manifest/lifecycle designs without creating hidden FSATS ownership, Foundation semantic cloning, cross-Application internal coupling, orphaned safety obligations, or self-restoring AI trust.

This record grants no implementation/runtime authority.
