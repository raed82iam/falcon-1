# FSATS Specialized Implementation Architecture — Authority, Source and Current-State Baseline

**Package:** `FSATS-SIA-v0.1`
**Status:** `DESIGN_CANDIDATE / SOURCE_BASELINE`
**Branch:** `application-development`
**Implementation Authority:** `NOT_GRANTED`

## 1. Purpose

This file freezes the evidence set used to build the specialized FSATS implementation architecture. It prevents later implementation detail from being based on memory, stale Part/P0/P1 assumptions, or an outdated Foundation state.

## 2. Source Hierarchy

The design SHALL be interpreted in this order:

```text
FALCON VISION
> FALCON CONSTITUTION
> CURRENT OWNER DECISIONS
> APPROVED SPECIFICATIONS / CONTRACTS / ACCEPTED ADRs
> CURRENT FOUNDATION CAPABILITY / FCR DISPOSITION
> CURRENT ACCEPTED FSATS DESIGN SEMANTICS
> THIS NEW DESIGN CANDIDATE
> HISTORICAL P0/P1/V1.3 REFERENCE KNOWLEDGE
```

A lower layer cannot silently override a higher layer.

## 3. Exact Governing Sources Read For This Candidate

| Source | Current identity used | Status / relevance |
|---|---|---|
| `applications/FSATS/WORKSTREAM_RULES.md` | blob `07373b0f5c12e5186025c46aa02b906582a73cc1` | Owner-controlled mandatory workstream rule |
| `applications/README.md` | blob `e9b3a059878adb8ed47135db4f707943bb2e5fd1` | Application workspace boundary |
| `applications/FSATS/README.md` | blob `551ff1fef12500cadb11b2f1d9f1eafbdae8ab56` | repository-recorded FSATS state |
| Falcon Vision | blob `7a8afe912e1840e84815ecfa95db0f1c9c45a8b6` | Approved supreme purpose authority |
| Falcon Constitution | current `foundation-development` source | Ratified / Approved |
| APP-001 v1.1 | blob `af31ab590a351b0e9f8c47ad2bf7048f3a2b676f` | Approved Application boundary/lifecycle |
| CON-023 v1.1 | blob `658177581b2c83b95c19a623b530f1655682b367` | Approved Application Contract/Manifest |
| ADR-I012 v1.1 | blob `0a0a8ce8a686af7553828f1478a3b09362a037f6` | Accepted Plug-and-Play integration boundary |
| ADR-I015 v1.0 | blob `efc330d4718ec3272875825068eaa70ccc0b3fdd` | Accepted Application/Awareness alignment |
| Accepted Part 0 index | blob `efeed99fb65144655fa5025a204066bca141bffa` | historical accepted current-design source |
| Part 0 Awareness final acceptance | blob `f89d15aba192538900520e8eaa13b5fc21c6673b` | later controlling accepted Awareness amendment |
| Part 1 decomposition | current candidate source `01_PART1NG_WORK_PACKAGE_DECOMPOSITION.md` | archive/reference input for this rebuild |
| V1.3 Reference Status | historical scratch reference | historical architecture/solved-problem input only |
| V1.3 delivery validation | package SHA-256 `d7fbde9fa0a584d9bb77f388016c9deb5fecdc30b0b9c3c0c7087743b32ac223` | historical 289-entry code-ready package evidence |

## 4. Owner Working Direction For `NEW`

Current Owner instruction for this work session is interpreted narrowly as:

- build the specialized architecture in `applications/docs/FSATS/NEW/`;
- use earlier P0/P1 and V1.3 as archive/reference knowledge rather than modifying them;
- do not claim the new package is accepted until the Owner explicitly accepts the exact reviewed semantic version.

This working direction does not retroactively rewrite repository history or change the bytes/status labels inside old P0/P1 artifacts.

## 5. Current Application Authority

```text
WRITE BRANCH = application-development
NORMAL WRITE SCOPE = applications/**
NEW PACKAGE SCOPE = applications/docs/FSATS/NEW/**
FOUNDATION WRITE AUTHORITY = NONE
REFERENCE/V1.3 WRITE AUTHORITY = NONE
IMPLEMENTATION CODE AUTHORITY = NONE
RUNTIME AUTHORITY = NONE
```

This package is design/documentation only.

## 6. Accepted Application Topology Constraints

The candidate SHALL preserve the current Application/Awareness model unless a later explicit Owner decision changes it:

```text
Falcon Self-Aware Trading Application
  MSA = 1
  LSA = 13

Falcon Self-Aware Provider Management Application (FSAPMA)
  MSA = 1
  LSA = 6

Falcon Trading Guardian Application
  MSA = 1
  LSA = 4

Falcon Self-Aware Trading Simulation Application (FSTSimA)
  MSA = 1
  LSA = 8

FSATS SYSTEM BOUNDARY
  MSA = 0
  LSA = 0
```

Total Application MSAs = 4. Total LSAs = 31. CSA remains optional and component-specific only where AWR-008 eligibility is proven.

## 7. Foundation Communication Capability Baseline

For Application design, the accepted Stage 5 boundary is treated as a generic platform substrate, not business authority.

The Application candidate may design exact Application-owned contracts against the following accepted generic capabilities where the corresponding FCR says the Foundation portion exists:

- Application communication manifest declarations/validation;
- FIL validation/admission;
- Service Bus route declaration/eligibility/selection/isolation;
- bounded delivery semantics, retry/idempotency/expiry/flow-control evidence;
- truthful event publication/replay classification/evidence;
- cryptographic message protection;
- lifecycle decision/evidence eligibility;
- integrated Stage 5 verification.

Application business semantics SHALL remain opaque to Foundation.

## 8. Current FCR Snapshot

### 8.1 `Waiting On: APPLICATION` — implementation hold, not a current documentary blocker

| FCR | Current meaning for this package |
|---|---|
| FCR-0004 | Design the exact Guardian protection-command contract/binding. Keep open until future implementation + binding fixtures are executable-verified. |
| FCR-0005 | Design exact FSAPMA operational-data contract/binding. Keep open until future implementation + binding fixtures are executable-verified. |
| FCR-0006 | Design exact event/evidence/replay usage. Keep open until future implementation + fixtures are executable-verified. |
| FCR-0010 | Design Application consumption of current resource-pressure/load-shedding boundary. Keep open until consuming implementation exists and is verified. |
| FCR-0031 | Design FSARM binding to Foundation Stage 6 resource boundary. Keep open until FSARM consuming implementation/bindings/fixtures exist and are verified. |

No additional Application comment is required merely because this design candidate is being written. The canonical FCR bodies explicitly require actual implementation evidence for the next closing action.

### 8.2 `Waiting On: FOUNDATION`

| FCR | Candidate handling |
|---|---|
| FCR-0012 | FSA internals/control plane SHALL NOT be designed locally. Application side specifies only required outbound/inbound semantics and fails closed. |
| FCR-0030 | Exact MSA-to-FSA Foundation interface/transport remains Foundation-owned. Application candidate defines its submission package and a binding adapter seam, not the Foundation endpoint implementation. |

### 8.3 Future-stage / no immediate actor

- FCR-0008 — Awareness research-only Internet egress;
- FCR-0009 — transport QoS/deadline governance;
- FCR-0011 — FSTSimA non-Live isolation/egress enforcement;
- FCR-0013 — FSAPMA external provider egress/credential boundary;
- FCR-0014 — broker execution egress/credential boundary;
- FCR-0016 — canonical cross-workstream Foundation artifact consumption.

Any runtime feature requiring one of these future capabilities is designed behind an explicit fail-closed capability gate.

## 9. Foundation Stage 6 Reconciliation

The current Foundation branch HEAD observed for this candidate is:

`f5eea8266852c6bf1f5695d6e11b6b437e570cad`

Its latest commit records Stage 6 WP-10 V3 post-implementation static Red-Team and states executable rerun and Owner closure are still pending.

Current FCR-0010/FCR-0031 bodies are more recent and more specific than stale summary lines in an older Foundation README for the resource chain. They state:

```text
WP-05 THROUGH WP-09 = FOUNDATION IMPLEMENTED / OWNER CLOSED
WP-10 = INTERNAL STAGE CLOSURE VERIFICATION, NOT A SECOND APPLICATION API
WP-10 OWNER CLOSURE = NOT YET
STAGE 6 OWNER CLOSURE = NOT YET
STAGE 7 AUTHORITY = NOT GRANTED
```

This candidate therefore binds only to the Application-facing resource boundary exposed through the accepted chain and does not depend on WP-10 as a runtime capability.

## 10. V1.3 Historical Baseline Facts Preserved As Reference

Historical V1.3 delivery evidence reports:

```text
ARCHIVE ENTRIES = 289
MARKETS = 2
PROVIDERS = 13
INITIAL ACTIVE PROVIDER TARGETS = 7
TRADING SCHOOLS = 2
STRATEGY MODELS = 10
LSA ROOMS = 12 (historical V1.3 topology, not current topology)
PACKAGE / SEMANTIC / SCHEMA / TRACEABILITY /
BASELINE-PRESERVATION / STATE-MACHINE /
RED-TEAM / STRUCTURAL VALIDATION = PASS
```

The current topology has evolved and SHALL NOT be forced back to the historical 12-LSA count.

## 11. Hard Design Invariants

```text
PROTECT > MANAGE > GROW
SELF_AWARENESS != AUTHORITY
INSTALLATION != ADMISSION
ADMISSION != ACTIVATION
ROUTE_EXISTS != AUTHORITY
DELIVERY != BUSINESS_SUCCESS
EVENT_PUBLISHED != BUSINESS_SUCCESS
FSA_REVIEW != PRODUCTION_ADOPTION
OWNER_SILENCE != APPROVAL
REQUESTED_RESOURCE != GRANTED_RESOURCE
FSARM != FOUNDATION_RESOURCE_GOVERNANCE
APPLICATION_BUSINESS_MEANING = APPLICATION_OWNED
FOUNDATION_SPECIAL_CASE_FOR_FSATS = FORBIDDEN
HIDDEN_CROSS_APPLICATION_COUPLING = FORBIDDEN
```

## 12. Unknown/Conflict Rule

If a later source read contradicts this baseline:

1. freeze the affected design assumption;
2. identify the higher/current authority;
3. record the exact semantic delta;
4. remediate the candidate;
5. rerun Architecture/Consistency and Red-Team reviews for the affected semantic version.

No contradiction may be silently averaged or guessed away.
