# P0-D - Foundation Capability Contract and Runtime Readiness

**Status:** `OWNER_DIRECTED_INTEGRATED_REWRITE_CANDIDATE / NOT_OWNER_ACCEPTED / NOT_CLOSED`
**Implementation Authority:** `NOT_GRANTED`
**Runtime Authority:** `NOT_GRANTED`

## 1. Purpose

P0-D defines how FSATS consumes Falcon Foundation capabilities without inventing Foundation behavior, duplicating Foundation responsibility or confusing semantic acceptance, implementation, Application verification and runtime authorization.

## 2. Responsibility

P0-D owns the Application-side model for:

- Foundation dependency identification;
- current Foundation state reconciliation;
- APP-001/CON-023/ADR-I012/ADR-I015 compatibility;
- FCR mapping;
- Application-local versus Foundation-owned classification;
- readiness-state separation;
- artifact consumption constraints;
- external egress dependency separation;
- platform-creep detection;
- fail-closed behavior when Foundation capability is missing, stale or incompatible.

P0-D does not own or prescribe Foundation implementation internals.

## 3. Foundation/Application ownership rule

Foundation owns generic Falcon OS/platform responsibilities including, within accepted scope:

- Application identity/lifecycle/admission platform governance;
- contract/manifest governance;
- governed cross-Application communication boundaries;
- Foundation security/integrity/containment boundaries;
- Falcon-wide technical-resource truth/governance;
- Foundation priority/technical-criticality semantics;
- canonical artifact publication/consumption;
- generic evidence/platform semantics;
- generic recovery/release mechanisms where Foundation authority is required.

Applications own their business meaning/state/policies, Application-local orchestration, domain-specific recovery consequences and domain logic.

```text
APPLICATION_NEED != FOUNDATION_IMPLEMENTATION_AUTHORITY
MISSING_FOUNDATION_CAPABILITY != PERMISSION_FOR_LOCAL_FAKE_FOUNDATION
```

APP-RSC remains an Application even though it coordinates FSATS resource use.

## 4. Four independent readiness axes

Every Foundation dependency is classified independently.

### 4.1 Semantic state

```text
SEMANTIC_ACCEPTED
SEMANTIC_PARTIAL
SEMANTIC_MISSING
SEMANTIC_INCOMPATIBLE
```

### 4.2 Implementation/acceptance state

```text
IMPLEMENTED_ACCEPTED
IMPLEMENTED_PENDING_ACCEPTANCE
NOT_IMPLEMENTED_OR_NOT_PROVEN
```

### 4.3 Application verification state

```text
APPLICATION_VERIFIED
APPLICATION_PENDING
NOT_APPLICABLE
```

### 4.4 Runtime authorization state

```text
RUNTIME_AUTHORIZED_FOR_EXACT_SCOPE
RUNTIME_NOT_AUTHORIZED
RUNTIME_UNKNOWN_FAIL_CLOSED
```

No axis backfills another.

```text
SEMANTIC_ACCEPTED != IMPLEMENTED_ACCEPTED
IMPLEMENTED_ACCEPTED != APPLICATION_VERIFIED
APPLICATION_VERIFIED != RUNTIME_AUTHORIZED
```

## 5. Accepted Foundation baseline that FSATS may rely on

The exact current Foundation state must always be refreshed. Part 0 may rely only on capabilities proven accepted in current Foundation evidence, not on historical Stage snapshots as permanent truth.

The predecessor Part 0 used accepted Stage 5 boundaries for Application-neutral communication/manifest/FIL/Service Bus/Event/cryptographic/lifecycle technical governance and accepted Stage 6 resource primitives/total-resource truth/per-Application allocation/priority-vs-criticality semantics. Those capability meanings remain relevant only to the exact degree current Foundation evidence still supports them.

They never imply provider connectivity, broker connectivity, credentials, Trading business authority, route activation, external egress, deployment or Live operation.

## 6. Communication/platform boundary

Where currently accepted Foundation scope provides generic Application communication governance, FSATS may design business contracts against it but cannot infer that a specific business route is active.

```text
CONTRACT_DECLARED != ROUTE_ADMITTED
ROUTE_ADMITTED != ROUTE_ACTIVE
ROUTE_ACTIVE != BUSINESS_AUTHORIZATION
```

Replay/test classification cannot escalate into operational authority merely because the same transport can carry both classes.

## 7. Foundation resource governance boundary

Foundation remains authoritative for:

- Falcon-wide total-resource truth;
- Foundation protection floors and recovery reserves;
- per-Application grants/ceilings/floors/isolation;
- Foundation technical criticality;
- cross-Application Application priority under Foundation policy;
- Foundation revoke/reclaim/restore decisions.

APP-RSC may coordinate only within current authoritative FSATS constituent envelopes and may request only proven residual need through the future canonical boundary.

```text
APPLICATION_RESOURCE_PRIORITY != FOUNDATION_TECHNICAL_CRITICALITY
APP_RSC_COORDINATION != FOUNDATION_RESOURCE_AUTHORITY
APP_RSC_REQUEST != FOUNDATION_GRANT
```

Trading-related Application importance cannot override protected Foundation survival/protection/control capacity.

## 8. What resource capability acceptance does not imply

Even where Foundation resource primitives/decision semantics are implemented, that does not automatically establish:

- canonical APP-RSC production binding;
- preemption/runtime shedding implementation for every Application;
- valid residual-request transport;
- live rebalance/restoration authority;
- APP-RSC direct access to Foundation internals;
- constituent direct Foundation resource requests.

Final canonical Application consumption remains separately governed.

## 9. Current material Foundation/FCR dependency matrix

Live issue state is the source of truth. Current material dependencies include:

| FCR | Application need | Current owning side / expected Foundation stage | Current Part 0 behavior |
|---|---|---|---|
| FCR-0008 | research-only Internet egress | FOUNDATION / Stage 12 | research runtime blocked |
| FCR-0009 | QoS/deadline-aware transport | FOUNDATION / Stage 11 | design semantics allowed, runtime capability not assumed |
| FCR-0010 | resource runtime consumption | Foundation resource capability exists but final canonical consumption pending Stage 14 path | no invented runtime binding |
| FCR-0011 | FSTSimA non-Live isolation/egress | FOUNDATION / Stage 12 | no safe external non-Live connection claim |
| FCR-0012 | FSA governance/integrity/control plane | FOUNDATION / Stage 13 | no local FSA implementation |
| FCR-0013 | FSAPMA provider egress/credential references | FOUNDATION / Stage 12 | provider runtime connectivity blocked |
| FCR-0014 | broker execution egress/credential references | FOUNDATION / Stage 12 | broker runtime connectivity blocked |
| FCR-0016 | canonical Foundation artifact publication/Application consumption | FOUNDATION / Stage 14 | no source-copy/local package workaround |
| FCR-0030 | MSA-to-FSA interface/transport | FOUNDATION / Stage 13 | no invented MSA-FSA runtime binding |
| FCR-0031 | APP-RSC canonical resource runtime binding | FOUNDATION dependency on Stage 14 consumption | APP-RSC final runtime binding blocked |

Additional live Foundation recovery/containment FCRs must be refreshed when their scope becomes material.

## 10. FCR discipline

```text
FCR_SUBMITTED != FOUNDATION_COMMITMENT_TO_IMPLEMENT
FCR_ACCEPTED_FOR_PLANNING != IMPLEMENTATION_AUTHORITY
FOUNDATION_IMPLEMENTED != APPLICATION_VERIFIED
APPLICATION_VERIFIED != OWNER_ACCEPTED_FSATS_RUNTIME
```

When a required Foundation capability is missing/partial/incompatible:

- keep dependency explicit;
- fail closed for affected runtime claim;
- use FCR channel;
- do not modify Foundation from Application work without explicit Owner authority;
- do not build an Application-local generic substitute for Foundation ownership.

## 11. Application-local mechanism versus Foundation duplication

An Application-local mechanism is valid when entirely inside one Application business boundary, not presented as generic Falcon-wide service, removable with the Application, not authoritative for Foundation-owned truth and not used to bypass lifecycle/security/resource/communication governance.

Examples may include bounded Application-local caches, queues, state machines, orchestration, indexes, policy evaluation and domain-specific recovery.

Reclassification/platform-creep review is required if the mechanism begins to:

- serve unrelated Applications generically;
- own shared cross-Application mutable truth;
- decide Foundation admission/lifecycle;
- decide total resources;
- create hidden communication bypass;
- become required for Falcon-wide operation outside its owner.

## 12. APP-RSC anti-platform-creep rule

APP-RSC is especially constrained because its domain is resource coordination.

APP-RSC may:

- ingest separately attributable constituent resource evidence;
- coordinate within current FSATS envelopes;
- perform bounded internal redistribution/degradation/rebalance;
- reconcile Foundation outcomes;
- assemble a proven residual request when the canonical boundary exists.

APP-RSC may not:

- become reusable Falcon-wide resource governance;
- own non-FSATS Application resource truth;
- mint Foundation grants/criticality/floors;
- bypass Foundation isolation/priority/governance;
- become a hidden FSATS container principal.

## 13. Canonical artifact consumption boundary

Application design may bind to immutable accepted Foundation artifact identities, but known SHA/digest is not the canonical consumption mechanism.

Until FCR-0016/Stage 14 is implemented and verified:

```text
COPY_FOUNDATION_SOURCE_INTO_APPLICATION = PROHIBITED
MOVING_BRANCH_HEAD_AS_CANONICAL_DEPENDENCY = PROHIBITED
UNCONTROLLED_LOCAL_PACKAGE = PROHIBITED
UNRESOLVED_ACCEPTED_ARTIFACT_VERSION = FAIL_CLOSED
TEST_ONLY_STRUCTURAL_COMPATIBILITY != PRODUCTION_RUNTIME_BINDING
```

APP-RSC final canonical binding is additionally fenced by FCR-0031.

## 14. External egress separation

Foundation-governed external paths remain separate authority classes:

```text
RESEARCH_EGRESS
!= OPERATIONAL_PROVIDER_EGRESS
!= BROKER_EXECUTION_EGRESS
```

One vendor, endpoint family or credential infrastructure does not merge those authority contexts.

Applications cannot bypass missing Stage 12 capability by opening generic network/credential access locally.

## 15. FSA/recovery boundary

FSA internals and generic Foundation containment/release remain Foundation-owned. Applications may define business consequences and evidence requirements but cannot mint OS-level isolation/release authority.

Current FSA governance/control plane and MSA-to-FSA transport remain FCR-0012/FCR-0030 future Stage 13 dependencies.

Current generic Foundation recovery/release state must be refreshed from current Foundation Stage 9/FCR evidence when material.

## 16. Failure behavior

If Foundation capability state is unknown, stale, incompatible or unverified:

- affected runtime feature is disabled/fail-closed;
- valid Application design may remain if dependency is explicit;
- no local substitute silently activates;
- current FCR/Foundation evidence is refreshed;
- clarification is requested through owning channel if needed.

Foundation outage/degradation never transfers Foundation authority to an Application.

## 17. Runtime degradation rule

A previously valid Foundation dependency becoming unavailable does not automatically invalidate all Application business state. The affected Application must determine safe degraded behavior inside already-granted authority while preventing new actions that require the unavailable capability.

```text
DEPENDENCY_DEGRADED != AUTHORITY_TRANSFER
DEPENDENCY_DEGRADED != AUTOMATIC_STATE_ERASURE
```

Existing exposure/protection/reconciliation obligations remain managed through whatever valid bounded capabilities remain.

## 18. Explicit non-authority

P0-D does not:

- prescribe Foundation internals;
- implement Foundation capabilities;
- close FCRs;
- mark Application verification without evidence;
- treat Stage authorization as capability availability;
- inflate a Foundation WP into later resource runtime;
- grant runtime authority;
- let FSATS grouping become Foundation principal;
- let APP-RSC become Falcon-wide resource governor.

## 19. Invariants

```text
FOUNDATION_OWNS_FOUNDATION
APPLICATION_OWNS_APPLICATION_BUSINESS
APP_RSC_IS_APPLICATION_NOT_FOUNDATION
APPLICATION_MISSING_CAPABILITY != LOCAL_FOUNDATION_SUBSTITUTE_PERMISSION
SEMANTIC_ACCEPTED != IMPLEMENTED_ACCEPTED
IMPLEMENTED_ACCEPTED != APPLICATION_VERIFIED
APPLICATION_VERIFIED != RUNTIME_AUTHORIZED
FCR_ACCEPTED_FOR_PLANNING != IMPLEMENTED
APPLICATION_RESOURCE_PRIORITY != FOUNDATION_TECHNICAL_CRITICALITY
FOUNDATION_ARTIFACT_IDENTITY != CANONICAL_CONSUMPTION_MECHANISM
RESEARCH_EGRESS != PROVIDER_EGRESS != BROKER_EGRESS
```

## 20. Forbidden interpretations

Invalid: Stage 5 closed means broker/provider connectivity; resource primitives exist so APP-RSC runtime binding exists; Trading priority means Foundation-critical; Guardian urgency mints Foundation criticality; FCR valid means Application implements Foundation gap; Application ACTIVE means Live; known SHA means copying Foundation source is canonical; APP-RSC is resource-related so may govern all Falcon Applications.

## 21. Mandatory scenarios

Challenge missing provider egress; missing broker egress; FSTSimA isolation unavailable; FSA runtime unavailable; APP-RSC current envelopes known but canonical binding unavailable; Foundation outcome stale; Foundation artifact SHA known but no Stage 14 consumer; local component attempting generic cross-Application resource service; Foundation outage during existing exposure; and Application attempting to inherit Foundation authority after dependency failure.

## 22. Exit gates

```text
FOUNDATION_OWNERSHIP_CONFLICTS = 0
LOCAL_HIDDEN_FOUNDATION_SUBSTITUTES = 0
APP_RSC_PLATFORM_CREEP = 0
FEATURE_TO_FCR_LINKAGE = COMPLETE
READINESS_AXIS_CONFLATION = 0
APPLICATION_PRIORITY_TECHNICAL_CRITICALITY_CONFLATION = 0
FCR0016_ARTIFACT_CONSUMPTION_OVERCLAIM = 0
EGRESS_AUTHORITY_CONFLATION = 0
CURRENT_FOUNDATION_STATE = EXPLICIT_AND_REFRESHABLE
```

## 23. Non-grant

Acceptance of P0-D would establish dependency/readiness semantics only. It would not authorize provider/broker/research egress, APP-RSC canonical runtime binding, deployment, Paper, Shadow, Tiny-Live or Live.