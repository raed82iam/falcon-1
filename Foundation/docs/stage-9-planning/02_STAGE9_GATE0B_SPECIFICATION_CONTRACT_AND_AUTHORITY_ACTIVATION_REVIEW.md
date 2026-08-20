# Stage 9 Gate 0B — Specification, Contract and Authority Activation Review

**Stage:** 9 — Controlled Recovery and Independent Release  
**Gate:** 0B — SPECIFICATION_DEFINITION_REVIEW_ACTIVATION_GATE  
**Status:** PASS_FOR_STAGE9_PLAN_PREPARATION / OWNER PLAN ACCEPTANCE STILL REQUIRED  
**Date:** 2026-08-15  
**Branch:** `foundation-development`

## 1. Purpose

Gate 0A established that Stage 9 has strong accepted predecessor capabilities and a materially adjacent planned Specification subject, AUT-003, that is not effective and has no current canonical body.

IMP-001 requires a definition/activation review before implementing missing behavior where a material planned Specification subject lacks an effective body.

This Gate determines whether Stage 9 requires activation of AUT-003 or any new Contract/ADR before implementation can be planned safely.

## 2. Effective normative coverage already available

### OPS-003 Recovery

OPS-003 v1.0 is Approved/effective and directly owns generic recovery semantics. It defines:

- authorized initiation;
- containment/assessment;
- versioned recovery plan;
- restoration;
- authoritative-state reconciliation;
- configuration/authority/security/data/dependency validation;
- independent validation;
- failed-validation denial;
- staged reintroduction;
- persistent Guardian restriction;
- bounded attempts/abort/escalation;
- recovery closure evidence.

### AUT-001 Authority Engine

AUT-001 v1.1 is Approved/effective and already requires:

- active protective restrictions constrain authority;
- material authority restoration requires a new attributable decision;
- restoration evidence and independent confirmation;
- subject under review cannot be sole authority restoring itself;
- authority uncertainty fails closed/reduces authority.

### AUT-002 Guardian

AUT-002 v1.0 is Approved/effective and already defines:

- release-condition ownership in the protective restriction;
- persistent restriction across restart;
- authorized evidence required for release;
- Guardian does not own ordinary recovery execution;
- Guardian cannot self-declare recovery complete.

### SYS-002 Lifecycle

SYS-002 v1.0 is Approved/effective and already defines:

- Lifecycle as transition owner;
- AUT-001 authorization for governed transitions;
- OPS-003 coordination for recovery transitions;
- no return to `RUNNING` before recovery validation.

### SYS-011 Persistence

SYS-011 v1.0 is Approved/effective and already defines:

- authoritative durable truth;
- verified restoration;
- corruption/uncertainty containment;
- restoration reconciliation of authority/version/causality;
- provenance preservation.

### CON-011 Protective Restriction

CON-011 v1.0 is Approved and directly consumed by Recovery. It already defines:

- Recovery Authority = authorized repair coordinator;
- Release Authority = authority permitted to approve release after independent validation;
- release conditions;
- independent-verification class;
- release-authority identity/role;
- persistent restriction;
- `Repair completion != release`;
- release requires satisfied conditions + independent validation + authorized approval + controlled Lifecycle transition + new authority decision;
- every release result produces reconstructable evidence.

### VPL-007 Controlled Recovery

VPL-007 v1.0 is Approved and provides the executable acceptance model and mandatory negative variants for Stage 9.

## 3. AUT-003 review

Registry entry:

`AUT-003 — Intervention, Revocation, and Recovery`

Current state:

- effective version: `NONE`;
- status: `NOT YET EFFECTIVE`;
- canonical body: absent;
- primary dependencies: AUT-001, AUT-002, OPS-003.

### 3.1 Overlap assessment

The generic recovery/release semantics Stage 9 needs are already normatively owned by OPS-003, AUT-001, AUT-002, SYS-002, SYS-011 and CON-011.

Creating and activating AUT-003 solely to restate the same semantics would risk:

- duplicate recovery ownership with OPS-003;
- duplicate protective-release ownership with AUT-002/CON-011;
- duplicate authority-restoration semantics with AUT-001;
- unnecessary ambiguity about whether Recovery or Guardian owns release.

### 3.2 Gate disposition

For Stage 9 v0.1 planning:

`AUT003_ACTIVATION_REQUIRED_BEFORE_STAGE9 = FALSE`

Reason:

The missing Stage 9 runtime behavior can be implemented as realization/integration of already-effective OPS-003 + AUT-001 + AUT-002 + SYS-002 + SYS-011 + CON-011 semantics without inventing a new normative authority domain.

AUT-003 remains a planned future Specification subject and SHALL NOT be cited as effective authority. If a future requirement emerges that cannot be expressed without changing or extending the existing ownership model, AUT-003 must return to a separately governed definition/activation review.

## 4. Unresolved matters from effective Specifications and their safe Stage 9 treatment

### 4.1 Recovery objectives by consequence class

OPS-003 lists recovery objectives by consequence class as unresolved.

Stage 9 SHALL NOT invent global RTO/RPO values or financial/live-capital recovery objectives.

Instead, every `RecoveryPlan` must carry its own authorized, versioned attempt/abort/validation parameters within the consequence and authority already granted. Missing required plan bounds fail closed.

No universal numeric recovery objective is created by Stage 9.

### 4.2 Independent validation authority matrix

OPS-003 lists the matrix as unresolved.

Stage 9 SHALL NOT invent a permanent global matrix.

Instead:

- the active restriction declares the required independent-verification class;
- the recovery case binds an exact Independent Verifier identity;
- AUT-001 validates the verifier's authority/independence for the requested validation action;
- the verifier may not be the subject, Guardian or repair actor;
- unknown/insufficient verifier authority fails closed.

A future matrix may standardize this without changing Stage 9's safety semantics.

### 4.3 Release authority by consequence class

AUT-002 leaves the ratifying authority matrix unresolved.

Stage 9 SHALL not infer one.

Instead:

- CON-011 restriction carries release-authority identity or role;
- Stage 9 requires an exact release request against that declared authority;
- AUT-001 evaluates whether the exact actor is permitted to release the exact restriction under the current authority chain;
- missing/ambiguous/conflicted release authority fails closed.

### 4.4 Probationary / heightened observation

AUT-002 already defines `HEIGHTENED` and `RECOVERY_GUARD` protective semantics, and VPL-007 requires heightened monitoring before normal status.

Stage 9 may implement a generic post-release observation state only as a binding of those existing semantics. It SHALL NOT implement FSA-specific Controlled Revival, Monitor AI or Factory Reset behavior.

## 5. Contract review

### 5.1 CON-011 sufficiency

CON-011 already contains the contract semantics needed for restriction-to-recovery-to-release binding.

No new external/cross-workstream recovery contract is required for the initial Stage 9 implementation plan.

Internal Stage 9 records may be added to Foundation-owned code provided they preserve CON-011 fields/ownership and do not silently redefine Contract meaning.

### 5.2 Existing contracts that remain owners

- CON-002 Authority Decision remains the authority decision boundary.
- CON-003 Lifecycle remains the lifecycle transition boundary.
- CON-008 Evidence and Logging remains evidence/logging boundary.
- CON-009 Security Context remains security-context boundary.
- CON-011 Protective Restriction remains the restriction/release-condition boundary.

Stage 9 SHALL compose these rather than create parallel contracts.

## 6. ADR review

OPS-003 lists ADR candidates for recovery orchestration, checkpoint/restore technology, staged reintroduction and recovery isolation environment.

For the initial Stage 9 implementation plan:

- no storage/checkpoint technology choice is needed;
- no deployment isolation technology is being selected;
- the existing accepted Foundation components are being composed through deterministic in-process/domain records and verifiers;
- Lifecycle and Guardian remain existing owners.

Therefore a new architecture ADR is not mandatory before planning the initial bounded implementation.

If implementation later requires a new durable recovery coordinator service, distributed consensus model, new isolation technology or storage/checkpoint technology, work SHALL stop and a governed ADR shall be created before that decision is embedded in code.

## 7. Required Stage 9 authority model

The plan shall preserve these distinct actors:

1. **Subject** — component/scope being recovered.
2. **Guardian** — owns active protective restriction/release conditions.
3. **Repair Actor** — performs authorized corrective/restoration action.
4. **Recovery Coordinator** — records/coordinates recovery case and plan; does not grant release.
5. **Independent Verifier** — independently validates recovery evidence.
6. **Declared Release Authority** — requests/decides release only within AUT-001 authority.
7. **Lifecycle** — owns reintroduction transition.
8. **AUT-001 Authority Engine** — owns authority decision, including restored authority.
9. **Evidence/Persistence** — own durable/reconstructable truth according to existing specifications.

Mandatory separation:

- Subject != Independent Verifier
- Subject != Release Authority
- Guardian != Release Authority
- Repair Actor != Independent Verifier
- Repair Actor != Release Authority
- Recovery Coordinator != automatic Release Authority
- Lifecycle != Recovery Validator
- Recovery PASS != Authority restoration

## 8. Gate 0B result

`STAGE9_GATE0B_SPECIFICATION_DEFINITION_REVIEW = PASS`

`OPS003_EFFECTIVE_RECOVERY_OWNER = TRUE`

`AUT003_EFFECTIVE = FALSE`

`AUT003_ACTIVATION_REQUIRED_FOR_INITIAL_STAGE9 = FALSE`

`NEW_RECOVERY_CONTRACT_REQUIRED_FOR_INITIAL_STAGE9 = FALSE`

`NEW_ARCHITECTURE_ADR_REQUIRED_FOR_INITIAL_STAGE9 = FALSE`

`CON011_RESTRICTION_RELEASE_CONTRACT = PRESERVE`

`AUT001_AUTHORITY_OWNER = PRESERVE`

`SYS002_LIFECYCLE_OWNER = PRESERVE`

`STAGE13_FSA_SPECIFIC_SCOPE = PRESERVE`

`STAGE9_PRODUCTION_IMPLEMENTATION = STILL_PENDING_OWNER_ACCEPTED_IMPLEMENTATION_PLAN`

## 9. Next action

Prepare the exact Stage 9 WP-01 through WP-10 implementation plan, then perform pre-implementation Architecture/Consistency and Red Team review before requesting Owner acceptance for implementation.
