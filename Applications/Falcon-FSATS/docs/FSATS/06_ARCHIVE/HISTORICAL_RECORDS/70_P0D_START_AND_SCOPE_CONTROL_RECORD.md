# FSATS V1.4 Part 0 / P0-D — Start and Scope Control Record

**Status:** `P0D_AUTHORIZED_BY_OWNER_CONTINUATION_AND_STARTED`  
**Scope:** `P0-D only — Foundation Alignment and Anti-Reimplementation Boundary`  
**Application branch:** `application-development`  
**P0-C predecessor:** `OWNER_ACCEPTED_AND_CLOSED`  
**P0-D Owner acceptance:** `NOT_GRANTED`  
**P0-E through P0-L:** `NOT_STARTED`

## 1. Authority and purpose

The Project Owner explicitly directed continuation to the next Part 0 work package after final P0-C acceptance and closure.

This record starts P0-D only. It does not authorize P0-E, Part 1 remediation, runtime implementation, Foundation modification, deployment, Paper, Tiny Live or Live operation.

P0-D exists to make one boundary non-ambiguous:

> Every requirement that belongs to Falcon Foundation must be consumed through current accepted Foundation contracts/capabilities or represented as a fail-closed Foundation Capability Request. No FSATS Application may reimplement, emulate, seize, infer or locally recreate Foundation authority merely because an expected Foundation runtime capability is incomplete.

## 2. Fresh source state

Application writable branch was confirmed unchanged immediately before P0-D start:

```text
application-development @ 38e5068220956d14a214e7f64a148a3bc2bd8b1b
comparison status = IDENTICAL
```

The Foundation source was freshly re-read from `foundation-development` rather than relying on the older FCR snapshot.

Current Foundation documentary state reports:

```text
Stage 5 WP-01 = ACCEPTED / CLOSED
Stage 5 WP-02 = ACCEPTED / CLOSED
Stage 5 WP-03 = ACCEPTED / CLOSED
Stage 5 WP-04 = ACCEPTED / CLOSED
Stage 5 WP-05 = ACCEPTED / CLOSED
Stage 5 WP-06 = ACCEPTED / CLOSED
Stage 5 WP-07 = IMPLEMENTATION AUTHORIZED / IN PROGRESS
Stage 5 WP-08 through WP-10 = NOT AUTHORIZED
```

Current accepted Foundation communication semantics include:

- WP-03 Application Communication Manifest declaration/validation;
- WP-04 bounded FIL validation and message-admission decision;
- WP-05 bounded Service Bus route declaration/eligibility/selection and route/endpoint isolation;
- WP-06 bounded Application-neutral delivery semantics, expiry/idempotency/retry/failure containment/flow control/technical priority/correlation-causation preservation and deterministic delivery evidence;
- WP-07 authorized Event System / Truthful Publication work, not yet accepted/closed at P0-D start.

## 3. Mandatory Foundation anchors

P0-D SHALL align at minimum to:

- Falcon Vision;
- Falcon Constitution;
- APP-001 Application Boundary and Lifecycle;
- CON-023 Falcon Application Contract and Manifest;
- current CON-000 Contract Registry;
- ADR-I012 generic Plug-and-Play Application integration boundary;
- ADR-I015 Application/Awareness alignment;
- SYS-006 Foundation resource-governance semantics;
- current Stage 5 accepted communication boundaries and current implementation/evidence state;
- canonical FCR shared registry and current FCR dispositions.

P0-D SHALL distinguish approved documentary semantics from current runtime capability evidence. An approved contract does not prove a requested runtime capability is implemented.

## 4. Mandatory dependency disposition vocabulary

Each material Foundation-facing dependency SHALL receive exactly one effective P0-D disposition:

- `FOUNDATION_CAPABILITY_ACCEPTED`
- `FOUNDATION_CAPABILITY_PARTIAL`
- `FOUNDATION_CAPABILITY_MISSING`
- `APPLICATION_OWNED`

Where a dependency has both a Foundation portion and an Application business portion, those portions SHALL be split explicitly rather than forcing one false owner over the whole feature.

## 5. Anti-reimplementation invariants

Applications SHALL NOT implement local substitutes for Foundation-owned:

- Application admission or platform lifecycle authority;
- FIL validation authority;
- Service Bus route authority or Foundation route selection;
- Foundation transport/delivery authority;
- Foundation event-truth/publication authority;
- Foundation total-resource allocation or platform pressure truth;
- Foundation technical priority authority;
- Foundation security/credential/egress enforcement;
- Foundation time/identity/authority-provider truth where governed contracts apply;
- Foundation-level FSA governance capability;
- Foundation schema/contract registry ownership;
- any other capability explicitly assigned to Foundation by governing specifications/contracts.

Missing capability SHALL produce `FAIL_CLOSED`, `DISABLED_PENDING_FOUNDATION_CAPABILITY`, or a bounded degraded design that does not recreate the missing authority.

## 6. Application-owned invariants

Foundation alignment SHALL NOT be used to transfer Application business logic upward.

Applications remain owners of their declared business semantics, including as applicable:

- Trading strategies, schools, frameworks and opportunity logic;
- Trading Risk business semantics and dynamic risk decisions;
- portfolio/trading capital allocation semantics;
- broker execution and trading position truth;
- FSAPMA provider/business selection and provider-quality semantics;
- Guardian trading-protection business meaning inside declared authority;
- FSTSimA simulation/scenario/fidelity business semantics;
- Application-specific self-awareness evaluation;
- Application load-shedding choices inside the Application's admitted allocation;
- Web presentation/business interaction semantics;
- Communication rendering/channel/business-delivery semantics where not Foundation transport authority.

## 7. FCR rule

An FCR is a cross-workstream request/disposition record only.

```text
FCR ACCEPTED_FOR_PLANNING
!= FOUNDATION IMPLEMENTED
!= APPLICATION AUTHORITY
```

P0-D may update its own understanding of an FCR using fresh Foundation evidence, but SHALL NOT mark Foundation implementation complete or close an FCR from the Application workstream.

## 8. Exit gate

P0-D may reach Owner review only when:

1. every material Foundation-facing requirement is dispositioned;
2. accepted versus partial versus missing capability is grounded in current Foundation evidence;
3. Application-owned business semantics remain Application-owned;
4. no local Foundation substitute is permitted;
5. every partial/missing Foundation runtime dependency maps to a canonical FCR or a newly justified FCR;
6. downstream work receives fail-closed obligations instead of invented platform behavior;
7. fresh Architecture/Consistency and Red-Team review reaches zero open findings.

## 9. Current state

```text
P0A = OWNER_ACCEPTED_AND_CLOSED
P0B = OWNER_ACCEPTED_AND_CLOSED
P0C = OWNER_ACCEPTED_AND_CLOSED
P0D = DESIGN_DEVELOPMENT_IN_PROGRESS
P0D_FINAL_OWNER_ACCEPTANCE = NOT_GRANTED
P0E_THROUGH_P0L = NOT_STARTED
PART1 = FROZEN_PENDING_PART0_REMEDIATION
PART2_THROUGH_PART10 = NOT_AUTHORIZED
RUNTIME / PAPER / TINY_LIVE / LIVE = NOT_GRANTED
FOUNDATION_MODIFICATION_FROM_APPLICATION_WORKSTREAM = NOT_AUTHORIZED
```
