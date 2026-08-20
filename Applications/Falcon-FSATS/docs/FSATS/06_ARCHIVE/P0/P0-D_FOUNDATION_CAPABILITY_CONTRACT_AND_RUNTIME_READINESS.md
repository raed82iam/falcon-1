# P0-D — Foundation Capability Contract and Runtime Readiness

**Status:** `PROPOSED / OWNER_REVIEW_CANDIDATE / NOT_OWNER_ACCEPTED / NOT_CLOSED`  
**Scope:** `P0-D only`  
**Implementation Authority:** `NOT GRANTED`

---

## 1. Purpose

P0-D defines how FSATS consumes Falcon Foundation capabilities without inventing Foundation behavior, duplicating Foundation responsibilities, or confusing design acceptance with implemented/runtime availability.

---

## 2. Responsibility

P0-D owns the Application-side model for:

- Foundation dependency identification;
- current Foundation state reconciliation;
- APP-001/CON-023/ADR-I012/ADR-I015 compatibility;
- FCR dependency mapping;
- Application-local versus Foundation-owned responsibility classification;
- runtime readiness status;
- platform-creep detection.

P0-D does not own Foundation implementation or internal Foundation design.

---

## 3. Foundation / Application Ownership Rule

Foundation owns generic OS/platform responsibilities including current accepted scope for:

- Application identity/lifecycle/admission platform governance;
- contract/manifest governance;
- governed cross-Application communication boundaries;
- Foundation security/integrity boundaries;
- Foundation technical-resource truth/governance;
- generic evidence/platform semantics;
- other explicitly accepted Foundation responsibilities.

Applications own their business meaning, business state, business policies, Application-local orchestration, Application-local recovery, and domain logic.

```text
APPLICATION_NEED != FOUNDATION_IMPLEMENTATION_AUTHORITY
MISSING_FOUNDATION_CAPABILITY != PERMISSION_FOR_LOCAL_FAKE_FOUNDATION
```

---

## 4. Four Independent Readiness Axes

Every Foundation dependency SHALL be classified independently.

### 4.1 Semantic State

- `SEMANTIC_ACCEPTED`;
- `SEMANTIC_PARTIAL`;
- `SEMANTIC_MISSING`;
- `SEMANTIC_INCOMPATIBLE`.

### 4.2 Implementation / Acceptance State

- `IMPLEMENTED_ACCEPTED`;
- `IMPLEMENTED_PENDING_ACCEPTANCE`;
- `NOT_IMPLEMENTED_OR_NOT_PROVEN`.

### 4.3 Application Verification State

- `APPLICATION_VERIFIED`;
- `APPLICATION_PENDING`;
- `NOT_APPLICABLE`.

### 4.4 Runtime Authorization State

- `RUNTIME_AUTHORIZED_FOR_EXACT_SCOPE`;
- `RUNTIME_NOT_AUTHORIZED`;
- `RUNTIME_UNKNOWN_FAIL_CLOSED`.

A permissive value on one axis cannot backfill another.

```text
SEMANTIC_ACCEPTED != IMPLEMENTED_ACCEPTED
IMPLEMENTED_ACCEPTED != APPLICATION_VERIFIED
APPLICATION_VERIFIED != RUNTIME_AUTHORIZED
```

---

## 5. Current Foundation State Relevant to this Candidate

Current Foundation documentary evidence used by this candidate:

```text
STAGE_0_THROUGH_STAGE_5 = ACCEPTED_AND_CLOSED

STAGE_5_WP01_THROUGH_WP10 = ACCEPTED_AND_CLOSED

STAGE_6_WP01 = ACCEPTED_AND_CLOSED
STAGE_6_WP02 = ACCEPTED_AND_CLOSED
STAGE_6_WP03 = ACCEPTED_AND_CLOSED
STAGE_6_WP04 = ACCEPTED_AND_CLOSED
STAGE_6_WP05_THROUGH_WP10 = NOT_AUTHORIZED
STAGE_7_THROUGH_STAGE_9_IMPLEMENTATION = NOT_AUTHORIZED
```

This snapshot must be refreshed before final Owner acceptance and later before implementation that depends on Foundation.

---

## 6. Accepted Stage 5 Boundaries

Within their exact closed scopes, P0-NG may rely on accepted existence of Application-neutral Foundation boundaries including:

- Application Communication Manifest declaration/validation;
- bounded FIL validation/message admission;
- Service Bus route declaration/eligibility/selection and route/endpoint isolation decisions;
- bounded generic delivery/retry/terminal containment/flow-control semantics within accepted WP-06 scope;
- Event System publication/replay classification and fail-closed replay-to-operational escalation prevention within accepted WP-07 scope;
- bounded cryptographic message protection within WP-08 scope;
- bounded Application lifecycle decision/evidence eligibility within WP-09 scope;
- Stage 5 integrated technical verification within WP-10 scope.

These accepted technical boundaries do not imply:

- provider connectivity;
- broker connectivity;
- Live credentials;
- Application business completion;
- Trading authority;
- deployment authority;
- route activation for a particular FSATS business interaction;
- external egress authority.

---

## 7. Accepted Stage 6 Resource Governance Foundation

### WP-01 — Canonical Resource Governance Primitives
Accepted generic resource-governance identity/value primitives include separation of admitted Application identity from requester/controller role identity and related request/decision/evidence identities.

Identity does not create authority.

### WP-02 — Foundation Resource Truth / Protection Floors / Recovery Reserves
Accepted Foundation-side total-resource truth, Foundation protection floors, recovery reserves, and allocatable-capacity prerequisites.

### WP-03 — Application Allocation / Quota / Ceiling / Isolation
Accepted bounded per-Application allocation/quota/ceiling/isolation state prerequisites.

These enable FSATS to design TARC against attributable admitted Trading allocation/ceiling truth without giving TARC total-resource authority.

### WP-04 — Cross-Application Priority / Foundation Technical Criticality
WP-04 is now `ACCEPTED_AND_CLOSED` within its exact Foundation-owned scope.

P0-NG may therefore rely on the accepted generic distinction and governance relation for:

- cross-Application **Application resource priority** under admitted/versioned policy relations;
- Foundation-owned **technical criticality** governance;
- explicit separation of Application priority from Foundation technical criticality;
- preservation of Foundation survival/protection/control capacity, non-reclaimable reserves, Authority, Health/Recovery, security/evidence integrity, and minimum Foundation governance/revocation/restoration capacity above Application workloads;
- rejection of caller-proposed or Application-internal urgency/priority as self-minted Foundation criticality.

Controlling invariant:

```text
APPLICATION_RESOURCE_PRIORITY != FOUNDATION_TECHNICAL_CRITICALITY
```

Trading-related Applications are in the highest cross-Application **Application** resource-priority domain under current Owner clarification, but this creates no general Trading authority and no right to override protected Foundation floors.

---

## 8. What WP-04 Closure Does Not Authorize

WP-04 closure explicitly does **not** authorize or prove:

- resource-pressure handling beyond exact accepted predecessor/transport consumption scope;
- preemption;
- enforcement-state runtime;
- full Application-facing load shedding;
- additional-resource request/decision runtime;
- reclamation;
- redistribution;
- rebalance;
- restoration;
- TARC-specific Foundation production behavior;
- Foundation control of Trading-internal distribution;
- Application business semantics.

Those remain later separately authorized Stage 6 scope and/or open FCR dependencies.

```text
WP04_ACCEPTED != FULL_RESOURCE_RUNTIME
```

---

## 9. FCR Discipline

An FCR records a valid need/gap/partial/incompatible Foundation capability.

```text
FCR_SUBMITTED != FOUNDATION_COMMITMENT_TO_IMPLEMENT
FCR_ACCEPTED_FOR_PLANNING != IMPLEMENTATION_AUTHORITY
FOUNDATION_IMPLEMENTED != APPLICATION_VERIFIED
APPLICATION_VERIFIED != OWNER_ACCEPTED_FSATS_RUNTIME
```

When a required Foundation capability is missing/partial/incompatible:

- keep the dependency explicit;
- fail closed for the affected runtime claim;
- use the FCR channel;
- do not modify Foundation from Application work;
- do not create an Application-local generic replacement for Foundation ownership.

---

## 10. Application-Local Mechanism vs Foundation Duplication Test

An Application-local mechanism is normally valid when it is:

- entirely inside one Application boundary;
- owned by that Application business/domain responsibility;
- not presented as a reusable Falcon-wide platform service;
- not used to bypass Foundation lifecycle/security/resource/communication governance;
- independently removable with the Application;
- not the authoritative source of Foundation-owned truth.

Examples may include bounded Application-local:

- caches;
- queues;
- business state machines;
- orchestration;
- internal indexes;
- policy evaluation;
- domain-specific recovery.

A reclassification/platform-creep review is required when a local mechanism begins to:

- serve unrelated Applications generically;
- own shared cross-Application mutable truth;
- decide Foundation lifecycle/admission;
- decide Foundation total resources;
- provide hidden communication bypass;
- become required for Falcon-wide operation outside the owning Application.

---

## 11. Artifact Consumption Boundary

Application design may bind to immutable accepted Foundation artifact identities.

Canonical cross-workstream build/package/publication/consumption mechanics remain an open Foundation capability under FCR-0016.

Until resolved:

```text
COPY_FOUNDATION_SOURCE_INTO_APPLICATION = PROHIBITED
MOVING_BRANCH_HEAD_AS_CANONICAL_DEPENDENCY = PROHIBITED
UNCONTROLLED_LOCAL_PACKAGE = PROHIBITED
UNRESOLVED_ACCEPTED_ARTIFACT_VERSION = FAIL_CLOSED
```

---

## 12. Failure Behavior

If Foundation capability state is unknown, stale, incompatible, or unverified:

- affected runtime feature remains disabled/fail-closed;
- current approved Application design may remain semantically valid if dependency is explicitly marked;
- no local substitute is silently activated;
- current FCR and Foundation evidence are refreshed;
- if needed, the Application requests clarification through FCR.

Foundation outage/degradation does not transfer Foundation authority to an Application.

---

## 13. Explicit Non-Authority

P0-D SHALL NOT:

- prescribe Foundation internals;
- implement Foundation capability;
- close an FCR;
- mark a capability Application-verified without evidence;
- treat Stage authorization as capability availability;
- inflate WP-04 closure into later resource runtime;
- grant runtime authority;
- allow FSATS grouping to become a Foundation principal.

---

## 14. Invariants

```text
FOUNDATION_OWNS_FOUNDATION
APPLICATION_OWNS_APPLICATION_BUSINESS
APPLICATION_MISSING_CAPABILITY != LOCAL_FOUNDATION_SUBSTITUTE_PERMISSION
SEMANTIC_ACCEPTED != IMPLEMENTED_ACCEPTED
IMPLEMENTED_ACCEPTED != APPLICATION_VERIFIED
APPLICATION_VERIFIED != RUNTIME_AUTHORIZED
FCR_ACCEPTED_FOR_PLANNING != IMPLEMENTED
STAGE6_WP04 = ACCEPTED_AND_CLOSED_WITHIN_EXACT_SCOPE
APPLICATION_RESOURCE_PRIORITY != FOUNDATION_TECHNICAL_CRITICALITY
WP04_ACCEPTED != PRESSURE_PREEMPTION_SHEDDING_REQUEST_REBALANCE_RESTORATION_RUNTIME
FOUNDATION_ARTIFACT_IDENTITY != APPROVED_CROSS_WORKSTREAM_CONSUMPTION_MECHANISM
```

---

## 15. Forbidden Interpretations

Invalid interpretations include:

- “Stage 5 is closed, therefore broker/provider connectivity exists”;
- “WP-04 is closed, therefore TARC resource request runtime exists”;
- “Trading is the highest Application priority, therefore Trading is Foundation-critical”;
- “Guardian urgency can mint Foundation criticality”;
- “WP-04 closure authorizes preemption/load shedding/rebalance/restoration”;
- “the FCR is valid, so Application can implement Foundation's missing part locally”;
- “TARC requester identity means the request runtime boundary exists”;
- “Application ACTIVE means business Live operation is authorized”;
- “artifact SHA is known, so copying Foundation source is canonical consumption”.

---

## 16. Exit Gates

```text
FOUNDATION_OWNERSHIP_CONFLICTS = 0
LOCAL_HIDDEN_FOUNDATION_SUBSTITUTES = 0
FEATURE_TO_FCR_LINKAGE = COMPLETE
READINESS_AXIS_CONFLATION = 0
STAGE6_WP04_STATE = ACCEPTED_AND_CLOSED
WP04_LATER_RUNTIME_OVERCLAIM = 0
APPLICATION_PRIORITY_TECHNICAL_CRITICALITY_CONFLATION = 0
FCR0016_ARTIFACT_CONSUMPTION_OVERCLAIM = 0
CURRENT_FOUNDATION_STATE = EXPLICIT_AND_REFRESHABLE
```

---

## 17. Next Authorized Gate

P0-D acceptance would establish dependency/readiness semantics only. It would not authorize later Stage 6 resource runtime, Application integration runtime, broker/provider egress, research egress, resource request runtime, or deployment.
