# TRC-001 — Foundation Requirement-to-Verification Traceability Matrix

**Identifier:** TRC-001  
**Version:** 1.2  
**Status:** Approved  
**Matrix Snapshot Date:** 2026-07-25  
**Effective Date:** 2026-07-25  
**Approval and Activation Record:** GOV-040  
**Amendment Package:** AMD-003  
**Owner:** Falcon Foundation Governance  
**Governing Authority:** Falcon Vision; Falcon Constitution; ADR-I007; ADR-I008; AMD-003; AMD-003-IR-001; PIPE-001 v1.1; SEC-002; CON-020; CON-021  
**Applicable Baseline:** Approved Foundation documentation through GOV-039  
**Implementation Authority:** Not Granted  
**Supersedes:** TRC-001 v1.1  
**Superseded By:** None

## 1. Purpose

TRC-001 v1.2 extends the Foundation traceability case through the complete AMD-003 bootstrap governance boundary.

It preserves the v1.1 matrix and adds forward and reverse traceability for:

- amended Contracts;
- new Authority and enabling-Provider Contracts;
- bootstrap execution and evidence Contracts;
- bootstrap verification plans;
- amended implementation, build, identity, time, cryptographic, security-design, environment, and Pipeline documents;
- bootstrap and governed Gate paths;
- Windows-first Foundation verification;
- future separately admitted Oracle Cloud execution;
- Activation and reconstruction; and
- preserved separation from FRS-001 implementation and operational authority.

This Matrix describes verification obligations. It does not claim that evidence exists or that any subject is active.

## 2. Preserved Matrix

Unless expressly amended here, TRC-001 v1.1 remains controlling for:

- trace record schema;
- requirement, risk, method, evidence, and Gate catalogs;
- FRS-001 invariants and scenarios;
- CON-001 through CON-011 mappings;
- Foundation specification, governance, catalog, design, build, and Pipeline mappings;
- forward and reverse VPL and Gate mappings;
- atomic expansion;
- orphan, conflict, change-impact, and integrity rules;
- requirements `TRC-001-REQ-001` through `TRC-001-REQ-020`; and
- deliberate activation blocks.

Source requirement text remains authoritative over every trace summary.

## 3. Snapshot Population

The v1.2 logical snapshot contains **1,144 genuine unique requirement identifiers**.

The count:

- includes requirements preserved by approved supersession even when the controlling v1.1 document is an amendment-style document;
- includes new Contracts `CON-012` through `CON-021`;
- includes `VPL-BST-000` through `VPL-BST-008`;
- includes the additional requirements in BLD-001 v1.1, IDN-001 v1.1, TIM-001 v1.1, CRY-001 v1.1, DESIGN-SEC-001 v1.1, ENV-001 v1.1, and PIPE-001 v1.1;
- includes the existing TRC-001 requirements; and
- excludes `DEC-001-REQ-014`, which is an identifier-format example in STD-002 and not a governing requirement.

The count is a document-snapshot property. It is not evidence completeness or verification success.

## 4. Supersession-Aware Requirement Rule

When an approved newer document states that prior requirements remain controlling:

- the prior requirement identifiers SHALL remain in the active logical requirement set;
- the archived prior document SHALL remain the authoritative source for its preserved text;
- the current document SHALL establish continued applicability and supersession lineage;
- the trace SHALL identify both the original source version and current controlling version;
- an added requirement SHALL not replace a preserved requirement unless explicit supersession says so; and
- file movement to archive SHALL not make a requirement inactive.

The following active logical ranges are supersession-aware:

| Document | Preserved range | Added range | Active logical range |
|---|---|---|---|
| BLD-001 v1.1 | `BLD-001-REQ-001..023` | `024..039` | `001..039` |
| IDN-001 v1.1 | `IDN-001-REQ-001..030` | `031..045` | `001..045` |
| TIM-001 v1.1 | `TIM-001-REQ-001..030` | `031..045` | `001..045` |
| CRY-001 v1.1 | `CRY-001-REQ-001..030` | `031..045` | `001..045` |
| DESIGN-SEC-001 v1.1 | `DESIGN-SEC-001-REQ-001..034` | `035..053` | `001..053` |
| ENV-001 v1.1 | `ENV-001-REQ-001..030` | `031..050` | `001..050` |
| PIPE-001 v1.1 | `PIPE-001-REQ-001..030` | `031..050` | `001..050` |

An atomic expansion that scans only the current amendment text and omits preserved identifiers is `INCOMPLETE`.

## 5. Bootstrap Trace Chain

The governed bootstrap chain is:

```text
ADR-I008 and AMD-003
        ↓
Jurisdiction and Authority Instrument
        ↓
CON-020 Bootstrap Execution Context
        ↓
External Identity, Time, Environment, Tools, and Evidence
        ↓
Preparation and Enabling-Provider Candidates
        ↓
VPL-BST Verification
        ↓
CON-021 Preserved Evidence
        ↓
Root Verification Evidence Set
        ↓
Completeness and Scoped Validity
        ↓
Separate Exact Activation Decisions
        ↓
VPL-BST-008 Reconstruction
        ↓
Eligible Transition to PIPE-001 Governed Mode
```

Every forward link SHALL have a reverse link.

No link grants implementation, operational, production, or financial authority.

## 6. Additional Risk Catalog

| Risk ID | Invalid condition or harm |
|---|---|
| `R-027` | Bootstrap mechanism is misrepresented as Falcon-native operational trust |
| `R-028` | Candidate Provider, environment, Pipeline, or evidence subject certifies or activates itself |
| `R-029` | External identity or time is silently upgraded to Falcon operational identity or verified time |
| `R-030` | Test, candidate, or synthetic material enters operational custody or authority |
| `R-031` | An inactive Profile, Gate, Provider, environment, or tool is used as active |
| `R-032` | Bootstrap, candidate, local-default, or weaker fallback returns after Activation |
| `R-033` | Supersession hides preserved requirements and creates false trace completeness |
| `R-034` | Platform or cloud Provider redefines Falcon Contracts, evidence, authority, or Pipeline meaning |
| `R-035` | Windows-scoped evidence is represented as Linux, cloud, Oracle Cloud, or universal validity |
| `R-036` | Cleanup, destruction, migration, or cloud exit leaves unrecorded residual material or authority |

## 7. Additional Verification Methods

| Method ID | Method |
|---|---|
| `VM-BST` | Bootstrap authority, context, identity, time, and external-control verification |
| `VM-CAP` | Enabling-Provider capability and negative-boundary probing |
| `VM-ACT` | Exact Profile, Provider, environment, Gate, Pipeline, or trace Activation verification |
| `VM-CLN` | Cleanup, destruction, revocation, quarantine, and residual-uncertainty verification |
| `VM-EXT` | Provider replacement, platform transition, cloud portability, and exit verification |

## 8. Additional Evidence Types

| Evidence ID | Evidence family |
|---|---|
| `EV-AUTI` | Authority Instrument, Authority Chain, jurisdiction, delegation, expiry, and revocation |
| `EV-BCTX` | Immutable CON-020 Bootstrap Execution Context |
| `EV-BEXT` | CON-021 external bootstrap evidence, origin, custody, and import lineage |
| `EV-CAND` | Candidate subject, Adapter, synthetic material, capability, and negative results |
| `EV-ACT` | Scoped validity, completeness, Acceptance, and exact Activation decision |
| `EV-CLN` | Cleanup, destruction, revocation, quarantine, residual material, and uncertainty |
| `EV-EXT` | Platform or Provider migration, replacement, recoverability, and exit evidence |

## 9. Bootstrap Gate Catalog

| Gate ID | PIPE-001 v1.1 stage | Primary decision |
|---|---|---|
| `BG-00` | Authority Instrument and Context Admission | Is the bootstrap action lawfully bounded before execution? |
| `BG-01` | Evidence Requirement Set Sealing | Are all bootstrap obligations fixed independently before evidence production? |
| `BG-02` | Preparation Environment Admission | Is the preparation environment exact, isolated, non-financial, and attributable? |
| `BG-03` | Tool and Dependency Bundle Verification | Are exact inputs genuine, bounded, and reproducible? |
| `BG-04` | Candidate Subject Admission | Is the exact candidate eligible for the declared bounded case? |
| `BG-05` | Enabling Provider Candidate Verification | Does the candidate enforce its Contract without creating operational trust? |
| `BG-06` | Environment Candidate Verification | Is the exact environment valid only for its declared scope? |
| `BG-07` | Pipeline and Trace Candidate Verification | Do Pipeline and trace subjects enforce obligations without self-promotion? |
| `BG-08` | Cleanup and Evidence Export | Are temporary authority and material removed or honestly quarantined with evidence preserved? |
| `BG-09` | Independent Evidence Reconstruction | Can an independent reviewer reconstruct the complete bootstrap and Activation path? |
| `BG-10` | Completeness and Scoped Validity | Is the verification case whole and valid for the declared scope? |
| `BG-11` | Separate Exact Activation | May this exact subject become active under competent jurisdiction? |

Bootstrap Gates do not replace PIPE-001 v1.0 `G-00` through `G-17`.

## 10. Authority and Bootstrap Contract Matrix

| Requirement range | Class | Risks | Methods | Evidence | Gates | Principal VPL |
|---|---|---|---|---|---|---|
| `CON-012-REQ-001..015` | RC-CON, RC-AUT | R-001, R-021, R-027, R-031 | VM-REV, VM-BST, VM-CHR | EV-AUTI, EV-DEC | BG-00, BG-10, BG-11 | VPL-BST-001 through VPL-BST-008 |
| `CON-013-REQ-001..016` | RC-CON, RC-AUT | R-001, R-021, R-028, R-031 | VM-REV, VM-CON, VM-BST, VM-CHR | EV-AUTI, EV-DEC, EV-CHL | BG-00, BG-10, BG-11 | VPL-BST-001, VPL-BST-006, VPL-BST-007, VPL-BST-008 |
| `CON-020-REQ-001..015` | RC-CON, RC-AUT, RC-SEC | R-007, R-027, R-028, R-029, R-030, R-031 | VM-BST, VM-SEC, VM-FLT, VM-CHR | EV-AUTI, EV-BCTX, EV-BEXT | BG-00 through BG-09 | VPL-BST-001 through VPL-BST-008 |
| `CON-021-REQ-001..016` | RC-CON, RC-EVD | R-004, R-013, R-027, R-028, R-029, R-033 | VM-BST, VM-AUD, VM-PRV, VM-CHR | EV-BEXT, EV-ROOT, EV-CHL | BG-01, BG-08, BG-09, BG-10 | VPL-BST-001 through VPL-BST-008 |

## 11. Enabling Provider Contract Matrix

| Requirement range | Class | Risks | Methods | Evidence | Gates | Principal VPL |
|---|---|---|---|---|---|---|
| `CON-014-REQ-001..016` | RC-CON, RC-IDN | R-002, R-017, R-028, R-029, R-031 | VM-CON, VM-CAP, VM-BST, VM-CHR | EV-CAND, EV-BEXT, EV-ACT | BG-04, BG-05, BG-10, BG-11 | VPL-BST-003, VPL-BST-008 |
| `CON-015-REQ-001..016` | RC-CON, RC-TIM | R-003, R-016, R-028, R-029, R-031 | VM-CON, VM-CAP, VM-TIM, VM-BST | EV-CAND, EV-TIM, EV-BEXT, EV-ACT | BG-04, BG-05, BG-10, BG-11 | VPL-BST-004, VPL-BST-008 |
| `CON-016-REQ-001..016` | RC-CON, RC-CRY, RC-SEC | R-015, R-024, R-028, R-030, R-031, R-032 | VM-CON, VM-CAP, VM-SEC, VM-FLT | EV-CAND, EV-SEC, EV-ACT | BG-04, BG-05, BG-08, BG-10, BG-11 | VPL-BST-005, VPL-BST-008 |
| `CON-017-REQ-001..015` | RC-CON, RC-CRY, RC-SEC | R-024, R-028, R-030, R-031, R-032 | VM-CON, VM-CAP, VM-SEC, VM-CLN | EV-CAND, EV-SEC, EV-CLN, EV-ACT | BG-04, BG-05, BG-08, BG-10, BG-11 | VPL-BST-005, VPL-BST-008 |
| `CON-018-REQ-001..015` | RC-CON, RC-SEC, RC-IDN | R-001, R-002, R-015, R-028, R-030, R-031 | VM-CON, VM-CAP, VM-SEC, VM-CHR | EV-CAND, EV-SEC, EV-ACT | BG-04, BG-05, BG-10, BG-11 | VPL-BST-005, VPL-BST-008 |
| `CON-019-REQ-001..014` | RC-CON, RC-CRY, RC-SEC | R-015, R-024, R-028, R-030, R-031 | VM-CON, VM-CAP, VM-SEC, VM-FLT | EV-CAND, EV-SEC, EV-ACT | BG-04, BG-05, BG-08, BG-10, BG-11 | VPL-BST-005, VPL-BST-008 |

## 12. Amended Contract Matrix

| Requirement range | Class | Risks | Methods | Evidence | Gates | Principal VPL |
|---|---|---|---|---|---|---|
| `CON-008-REQ-001..020` | RC-CON, RC-EVD | R-004, R-013, R-027, R-028, R-029, R-033 | VM-CON, VM-BST, VM-AUD, VM-CHR | EV-LOG, EV-BEXT, EV-ROOT | BG-01, BG-08, BG-09, BG-10; G-07, G-14 | VPL-BST-001 through VPL-BST-008; VPL-008 |
| `CON-010-REQ-001..020` | RC-CON, RC-EVD | R-002, R-008, R-014, R-027, R-031, R-033 | VM-CON, VM-SCH, VM-PRV, VM-BST | EV-MAN, EV-BEXT, EV-ACT | BG-04, BG-06, BG-07, BG-09, BG-11; G-02, G-03, G-13 | VPL-BST-006 through VPL-BST-008; VPL-001, VPL-008 |

## 13. Bootstrap Verification Matrix

| Plan | Requirement range | Primary risks | Primary gates | Required proof |
|---|---|---|---|---|
| `VPL-BST-000` | `VPL-BST-000-REQ-001..010` | R-007, R-027 through R-033 | BG-00 through BG-11 | complete bounded bootstrap verification case |
| `VPL-BST-001` | `VPL-BST-001-REQ-001..008` | R-007, R-027, R-029, R-031 | BG-00, BG-01, BG-02, BG-08 | preparation without borrowed future trust |
| `VPL-BST-002` | `VPL-BST-002-REQ-001..008` | R-014, R-022, R-027, R-031 | BG-01, BG-02, BG-03, BG-08 | exact attributable tool and dependency bundle |
| `VPL-BST-003` | `VPL-BST-003-REQ-001..008` | R-017, R-028, R-029, R-031 | BG-04, BG-05, BG-08, BG-10 | bounded Identifier Provider candidate |
| `VPL-BST-004` | `VPL-BST-004-REQ-001..008` | R-016, R-028, R-029, R-031 | BG-04, BG-05, BG-08, BG-10 | conservative Time Provider candidate |
| `VPL-BST-005` | `VPL-BST-005-REQ-001..012` | R-015, R-024, R-028, R-030 through R-032 | BG-04, BG-05, BG-08, BG-10 | custody, purpose, isolation, and no fallback |
| `VPL-BST-006` | `VPL-BST-006-REQ-001..010` | R-007, R-014, R-028, R-031, R-034 through R-036 | BG-06, BG-08, BG-10, BG-11 | exact scoped Environment Activation |
| `VPL-BST-007` | `VPL-BST-007-REQ-001..012` | R-008, R-013, R-022, R-028, R-031, R-033 | BG-07, BG-08, BG-10, BG-11 | Pipeline and trace without self-promotion |
| `VPL-BST-008` | `VPL-BST-008-REQ-001..010` | R-004, R-013, R-021, R-027 through R-033 | BG-09, BG-10, BG-11 | independent complete reconstruction |

Passing `VPL-BST` does not satisfy or replace VPL-000 through VPL-008.

## 14. Amended Foundation Document Matrix

| Active logical requirement range | Primary classes | Risks | Methods | Gates | Principal VPL |
|---|---|---|---|---|---|
| `BLD-001-REQ-001..039` | RC-BLD, RC-EVD | R-007, R-013, R-014, R-022, R-027, R-031 through R-034 | VM-REV, VM-STA, VM-PRV, VM-REP, VM-BST | BG-01 through BG-03, BG-07 through BG-10; G-02 through G-06, G-12, G-13 | VPL-BST-001, VPL-BST-002, VPL-BST-007, VPL-BST-008 |
| `IDN-001-REQ-001..045` | RC-IDN, RC-SEC | R-002, R-017, R-028, R-029, R-031, R-032 | VM-SCH, VM-CAP, VM-CON, VM-CHR | BG-04, BG-05, BG-08, BG-10, BG-11; G-03, G-07 | VPL-BST-003, VPL-BST-008 |
| `TIM-001-REQ-001..045` | RC-TIM, RC-SAF | R-003, R-016, R-028, R-029, R-031, R-032 | VM-TIM, VM-CAP, VM-FLT, VM-REC | BG-04, BG-05, BG-08, BG-10, BG-11; G-03, G-07, G-10 | VPL-BST-004, VPL-BST-008 |
| `CRY-001-REQ-001..045` | RC-CRY, RC-SEC | R-015, R-024, R-028, R-030 through R-032 | VM-SCH, VM-CAP, VM-SEC, VM-FLT, VM-CLN | BG-04, BG-05, BG-08, BG-10, BG-11; G-07, G-08, G-10 | VPL-BST-005, VPL-BST-008 |
| `DESIGN-SEC-001-REQ-001..053` | RC-CRY, RC-SEC | R-006, R-015, R-019, R-024, R-028, R-030 through R-036 | VM-STA, VM-CAP, VM-SEC, VM-FLT, VM-REC, VM-CLN, VM-EXT | BG-04 through BG-06, BG-08 through BG-11; G-03, G-06, G-08, G-10, G-16 | VPL-BST-005, VPL-BST-006, VPL-BST-008 |
| `ENV-001-REQ-001..050` | RC-BLD, RC-SEC, RC-OPS | R-002, R-007, R-014 through R-016, R-019, R-022, R-024, R-027 through R-036 | VM-BST, VM-STA, VM-SEC, VM-INT, VM-FLT, VM-REP, VM-CLN, VM-EXT | BG-00 through BG-11; G-02 through G-13, G-15, G-16 | VPL-BST-001, VPL-BST-002, VPL-BST-006 through VPL-BST-008 |
| `PIPE-001-REQ-001..050` | RC-PIPE, RC-AUT, RC-EVD | R-001, R-007, R-008, R-013, R-021, R-022, R-027 through R-035 | VM-REV, VM-BST, VM-STA, VM-AUD, VM-CHR, VM-ACT | BG-00 through BG-11; G-00 through G-17 | VPL-BST-000, VPL-BST-007, VPL-BST-008 |

## 15. Governance and Approval Trace

| Decision | Controlled subjects | Trace effect |
|---|---|---|
| GOV-024 | AMD-003 | authorizes amendment drafting, not execution |
| GOV-025 | AMD-003-IR-001 | establishes Contract and VPL impact decisions |
| GOV-026 through GOV-029 | CON-012 through CON-021 | admits authority, Provider, bootstrap context, and evidence Contracts |
| GOV-030 | CON-008 v1.1; CON-010 v1.1 | activates evidence-origin and baseline-manifest amendments |
| GOV-031 | VPL-BST-000 through VPL-BST-008 | admits bootstrap verification plans without execution |
| GOV-032 | IMP-001 v1.2 | establishes staged preparation and implementation boundary |
| GOV-033 | BLD-001 v1.1 | establishes build baseline classes without activation |
| GOV-034 | IDN-001 v1.1 | establishes Identifier candidate boundary without Provider activation |
| GOV-035 | TIM-001 v1.1 | establishes Time candidate boundary without verified time |
| GOV-036 | CRY-001 v1.1 | establishes security candidate Profile without material or Provider activation |
| GOV-037 | DESIGN-SEC-001 v1.1 | establishes Windows-first, portable custody design |
| GOV-038 | ENV-001 v1.1 | establishes Windows-first environment and future OCI direction |
| GOV-039 | PIPE-001 v1.1 | establishes bootstrap and governed Pipeline modes without Gate activation |

Approval records establish documentary authority. They do not substitute for verification evidence or subject Activation.

## 16. Windows and Cloud Trace Rule

Every environment-dependent atomic record SHALL declare one:

- `WINDOWS`;
- `LINUX`;
- `OCI`;
- `CROSS_PLATFORM`;
- `PLATFORM_INDEPENDENT`; or
- another governed deployment profile.

Rules:

- Windows is the ordered first Foundation execution target;
- Windows evidence SHALL remain Windows-scoped;
- Linux evidence requires an independently admitted Linux subject;
- OCI evidence requires a future approved OCI Environment Profile;
- provider-neutral requirements SHALL be tested for Contract and Adapter isolation;
- platform-specific evidence SHALL not be generalized without an approved cross-platform method; and
- OCI service evidence SHALL not establish Falcon authority, completeness, acceptance, or portability by itself.

## 17. Self-Awareness and Evolution Trace Rule

Every Self-Awareness, maintenance, repair, update, migration, or evolution requirement SHALL trace through:

```text
Observation
    ↓
Diagnosis
    ↓
Governed Change Proposal
    ↓
Authority and Jurisdiction
    ↓
Immutable Candidate
    ↓
Independent Verification
    ↓
Completeness and Scoped Validity
    ↓
Competent Acceptance
    ↓
Exact Activation
    ↓
Monitoring and Rollback
```

Any path that permits Self-Awareness to change policy, widen authority, accept its own Claim, activate its own change, bypass Guardian, or avoid independent verification is `INVALID`.

## 18. Forward Completeness Additions

Every AMD-003 atomic trace record SHALL additionally identify:

1. bootstrap or governed mode;
2. Authority Instrument class;
3. CON-020 context class where applicable;
4. evidence origin;
5. external or Falcon identity and time class;
6. candidate or active lifecycle;
7. activation subject and authority;
8. cleanup or residual-uncertainty obligation;
9. platform scope;
10. fallback prohibition; and
11. explicit non-authorities.

Missing any applicable field makes the trace `INCOMPLETE`.

## 19. Reverse Completeness Additions

Every:

- Authority Instrument class;
- Bootstrap Execution Context;
- external identity or time mechanism;
- Provider candidate;
- Adapter candidate;
- bootstrap Gate;
- VPL-BST procedure;
- Activation Decision;
- cleanup or quarantine result;
- platform or cloud Profile; and
- bootstrap evidence import or reconstruction

SHALL trace back to at least one approved requirement.

An untraced candidate or Activation subject is prohibited from execution or reliance.

## 20. Machine-Readable Expansion

Before any bootstrap or governed Gate becomes active, TRC-001 SHALL be expanded into one canonical record per genuine active logical requirement identifier.

The expansion SHALL:

- contain exactly the active logical requirement population for the bound source snapshot;
- resolve supersession-aware preserved requirements;
- exclude documented non-requirement examples;
- bind exact source version, archived source where applicable, and current controlling version;
- reject missing, duplicate, malformed, or discontinuous identifiers;
- preserve forward and reverse links;
- produce a canonical digest;
- remain immutable after sealing;
- support independent reconstruction; and
- be verified through VPL-BST-007 and VPL-BST-008.

The human-readable count of 1,144 SHALL be independently recalculated from the bound snapshot before Activation.

Any mismatch produces `INCOMPLETE` or `INVALID`, never automatic correction.

## 21. Trace States

The v1.1 states remain controlling:

- `COMPLETE`;
- `INCOMPLETE`;
- `CONFLICTED`;
- `STALE`;
- `INVALID`; and
- `NOT_APPLICABLE`.

For AMD-003:

- `COMPLETE` requires both documentary links and correct supersession-aware atomic expansion;
- `STALE` applies when any source, version, Contract, VPL, Gate, environment, Provider, authority, or platform scope changes;
- `CONFLICTED` applies when archived and controlling meanings disagree materially;
- `INVALID` applies when identity, provenance, integrity, authority, source, or canonical expansion cannot be established; and
- unknown or merely inferred links are not complete.

## 22. Requirements Added

- **TRC-001-REQ-021:** TRC-001 SHALL preserve every requirement retained through approved supersession.
- **TRC-001-REQ-022:** Archived source location SHALL NOT make a preserved requirement inactive.
- **TRC-001-REQ-023:** Atomic expansion SHALL identify original source version and current controlling version.
- **TRC-001-REQ-024:** The v1.2 snapshot SHALL cover 1,144 genuine unique logical requirement identifiers, subject to independent pre-Activation recalculation.
- **TRC-001-REQ-025:** `DEC-001-REQ-014` SHALL remain excluded as a non-normative identifier example.
- **TRC-001-REQ-026:** CON-012 through CON-021 SHALL have forward and reverse traceability.
- **TRC-001-REQ-027:** VPL-BST-000 through VPL-BST-008 SHALL have forward and reverse traceability.
- **TRC-001-REQ-028:** Every AMD-003 authority stage SHALL trace from jurisdiction through exact Activation.
- **TRC-001-REQ-029:** Bootstrap and Falcon-native identity, time, evidence, and lifecycle states SHALL remain distinguishable.
- **TRC-001-REQ-030:** Candidate and independent-control evidence SHALL remain separately traceable.
- **TRC-001-REQ-031:** Every bootstrap Gate SHALL trace to approved requirements and VPL-BST evidence.
- **TRC-001-REQ-032:** Bootstrap Gate evidence SHALL NOT satisfy a governed Release Candidate Gate by relabeling.
- **TRC-001-REQ-033:** Every Profile, Provider, environment, Gate, Pipeline, and trace Activation SHALL be separately traceable.
- **TRC-001-REQ-034:** Cleanup, destruction, quarantine, and residual uncertainty SHALL remain traceable.
- **TRC-001-REQ-035:** Windows-scoped evidence SHALL not establish Linux, OCI, or universal validity.
- **TRC-001-REQ-036:** Future OCI requirements and evidence SHALL remain provider-specific at the Adapter boundary and provider-neutral at Falcon Contracts.
- **TRC-001-REQ-037:** Every active dependency SHALL trace a prohibition against bootstrap, candidate, local-default, or weaker fallback.
- **TRC-001-REQ-038:** Self-Awareness maintenance and evolution SHALL trace through independent verification and competent exact Activation.
- **TRC-001-REQ-039:** Self-Awareness SHALL not accept, activate, promote, or widen the authority of its own change.
- **TRC-001-REQ-040:** Every machine-readable atomic record SHALL preserve applicable authority, context, origin, lifecycle, platform, cleanup, and non-authority fields.
- **TRC-001-REQ-041:** A population mismatch, duplicate, missing ID, source conflict, or unverifiable supersession SHALL prevent trace completeness.
- **TRC-001-REQ-042:** Approval of TRC-001 v1.2 SHALL not create trace evidence, activate a Gate or Pipeline, issue authority, or authorize implementation.

## 23. Conformance Evidence

Conformance requires proof that:

- all 1,144 logical identifiers expand exactly once;
- all preserved v1.0 identifiers remain present under their controlling v1.1 lineage;
- the excluded DEC-001 example does not enter the population;
- CON-012 through CON-021 map in both directions;
- VPL-BST-000 through VPL-BST-008 map in both directions;
- every bootstrap Gate maps to approved obligations;
- every AMD-003 approval record maps to its exact subject and non-authorities;
- candidate, external-control, imported, and Falcon-native evidence remain distinguishable;
- external identity and time cannot be upgraded;
- inactive subjects cannot appear active;
- Windows validity cannot be generalized to Linux or OCI;
- OCI-specific concerns remain behind Adapter and Environment Profile boundaries;
- self-maintenance changes cannot bypass independent verification;
- cleanup and residual uncertainty remain visible;
- orphan requirements, candidates, evidence, evaluations, and Activation decisions are detected;
- a changed source marks affected records stale;
- a mismatched population blocks completeness; and
- no trace creates implementation, production, financial connectivity, or financial authority.

## 24. Known Deliberate Blocks

At v1.2:

- the human-readable supersession-aware snapshot is defined;
- machine-readable atomic expansion has not been produced;
- the 1,144 count has not been independently verified through an active Pipeline;
- no bootstrap Gate Profile is active;
- no governed Gate Profile is active;
- no Pipeline or runner is active;
- no Identifier, Time, Security, or other enabling Provider is active;
- no Environment Profile or Activation Manifest is active;
- no Verification Session or VPL-BST plan has executed;
- no Root Verification Evidence Set exists;
- no trace completeness Claim is made;
- no OCI Environment Profile exists; and
- no implementation authority exists.

These blocks prevent Pipeline and trace execution use.

## 25. Required Before Trace Activation

TRC-001 execution use requires:

1. TRC-001 v1.2 Approval;
2. exact registration as the controlling Foundation Matrix;
3. canonical machine-readable expansion of every active logical identifier;
4. independent recalculation of the population;
5. zero missing, duplicate, malformed, discontinuous, or conflicting identifiers;
6. exact original and controlling source bindings;
7. complete forward and reverse links;
8. approved canonical schema and encoding;
9. successful VPL-BST-007 and VPL-BST-008 execution;
10. independent trace completeness evaluation;
11. approved and active applicable Gate Profile;
12. complete Challenge and supersession verification;
13. financial-isolation verification; and
14. a separate exact Trace Activation Decision.

## 26. Foundational Rules

> **Supersession changes control; it does not erase preserved obligations.**

> **Bootstrap evidence proves only the bounded bootstrap case it was governed to observe.**

> **A trace gap is a visible block, not permission to infer the missing link.**

> **Platform validity does not travel farther than its evidence.**

> **Nothing may disappear between authority, obligation, evidence, evaluation, and Activation.**

## 27. Supersession

With this Approval:

- TRC-001 v1.2 supersedes v1.1;
- every v1.1 mapping and requirement not expressly amended remains controlling;
- AMD-003 Contracts, VPL-BST plans, document amendments, approvals, Gates, and Activation boundaries enter the governed Matrix;
- no trace expansion, evidence, verification result, or Activation is created; and
- implementation, production, promotion, and financial authority remain ungranted.

## 28. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner and current Constitutional Authority | Approved and Activated | GOV-040 | 2026-07-25 |

This Approval adopts TRC-001 v1.2 as the controlling Foundation Traceability Matrix and archives v1.1.

It does not:

- claim that the 1,144 requirements have passed verification;
- create the machine-readable expansion;
- activate a Gate, Pipeline, runner, Provider, environment, or trace implementation;
- execute a VPL or VPL-BST plan;
- issue an Authority Instrument;
- authorize implementation;
- authorize promotion;
- authorize production;
- authorize financial connectivity; or
- authorize financial activity.
