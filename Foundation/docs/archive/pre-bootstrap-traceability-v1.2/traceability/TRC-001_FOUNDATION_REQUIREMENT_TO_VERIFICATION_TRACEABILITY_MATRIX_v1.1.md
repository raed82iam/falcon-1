# TRC-001 — Foundation Requirement-to-Verification Traceability Matrix

**Version:** 1.1  
**Status:** Approved  
**Matrix Snapshot Date:** 2026-07-25  
**Owner:** Falcon Foundation Governance  
**Governing Authority:** Falcon Vision; Falcon Constitution; GOV-001; ADR-I007; PIPE-001  
**Applicable Baseline:** FRS-001; Approved Foundation Baseline  
**Related Documents:** BLD-001; ENV-001; VPL-000 through VPL-008  
**Supersedes:** TRC-001 v1.0  
**Superseded By:** None  
**Implementation Authority:** Not Granted

## 1. Purpose

This Matrix provides forward and reverse traceability between Falcon Foundation obligations and the verification case required to support them.

It answers:

- What is the governing obligation?
- What harm or invalid condition does it prevent?
- What boundary or subject does it affect?
- What evidence must exist?
- Which Pipeline Gate evaluates it?
- Which VPL scenario demonstrates it?
- What happens if proof is absent?

The Matrix makes omissions visible. It does not create, weaken, reinterpret, or approve a requirement.

## 2. Scope

TRC-001 covers:

- eight FRS-001 release invariants;
- eight FRS-001 demonstration scenarios;
- CON-001 through CON-011;
- VPL-001 through VPL-008;
- all 776 genuine, uniquely identified `*-REQ-*` obligations present in the Approved Foundation document set at the Matrix Snapshot Date, including TRC-001 and ENV-001;
- PIPE-001 Gate coverage;
- forward traceability;
- reverse traceability;
- orphan detection;
- change impact; and
- completeness rules.

## 3. Non-Scope

TRC-001 does not:

- replace the source text of any obligation;
- invent acceptance criteria;
- claim that a test exists;
- claim that evidence has been produced;
- activate a Gate Profile;
- authorize implementation;
- authorize promotion;
- authorize production; or
- authorize financial activity.

## 4. Traceability Model

The governed chain is:

```text
Vision and Constitution
        ↓
Release Invariant or Governed Requirement
        ↓
Risk and Failure Condition
        ↓
Affected Contract or Boundary
        ↓
Verification Obligation
        ↓
Evidence Type
        ↓
Pipeline Gate
        ↓
Verification Session or VPL
        ↓
Derived Evaluation
        ↓
Root Verification Evidence Set
```

Every forward link SHALL have a reverse link.

## 5. Atomic Expansion Rule

Compact notation is used for maintainability.

For example:

```text
SYS-001-REQ-001..014
```

means fourteen independent atomic trace rows:

```text
SYS-001-REQ-001
SYS-001-REQ-002
...
SYS-001-REQ-014
```

The range SHALL NOT be interpreted as:

- one aggregated requirement;
- permission to sample requirements;
- permission to omit an identifier;
- one shared result; or
- proof that all members pass because one member passes.

At Pipeline execution time, PIPE-001 SHALL expand every range into individual Evidence Requirements.

If one identifier within a range requires a narrower verification method, Contract, Gate, or VPL, the generated Evidence Requirement Set SHALL record that atomic refinement without altering TRC-001.

## 6. Trace Record Schema

Every atomic trace record contains:

| Field | Meaning |
|---|---|
| `RequirementId` | Immutable governing obligation |
| `SourceDocument` | Exact authoritative source and version |
| `SourceLocation` | Section or controlled locator |
| `RequirementClass` | Invariant, behavior, security, governance, Contract, evidence, build, or other governed class |
| `RiskIds` | Risks or invalid conditions controlled |
| `SubjectIds` | Affected component, Contract, artifact, or boundary |
| `VerificationMethodIds` | Required method families |
| `EvidenceTypeIds` | Required evidence families |
| `GateIds` | Pipeline Gates that evaluate the record |
| `VplIds` | Applicable approved scenario plans |
| `Platforms` | Windows, Linux, cross-platform, or platform-independent |
| `ResultRule` | Pass, fail, inconclusive, blocked, and not-applicable criteria |
| `Independence` | Producer, evaluator, and authority separation |
| `Status` | Trace-definition status, not verification result |

## 7. Requirement Classes

| Class ID | Meaning |
|---|---|
| `RC-INV` | Release invariant |
| `RC-CON` | Contract obligation |
| `RC-BEH` | Required behavior |
| `RC-SEC` | Security or trust obligation |
| `RC-SAF` | Protective or Safe-state obligation |
| `RC-EVD` | Evidence, provenance, or reconstruction obligation |
| `RC-AUT` | Jurisdiction, authority, or delegation obligation |
| `RC-BLD` | Toolchain, build, or dependency obligation |
| `RC-PIPE` | Pipeline, evaluation, completeness, or promotion obligation |
| `RC-OPS` | Reliability, recovery, health, or operational obligation |
| `RC-REP` | Canonical representation obligation |
| `RC-TIM` | Time, uncertainty, or epoch obligation |
| `RC-IDN` | Identifier obligation |
| `RC-CRY` | Cryptographic obligation |

## 8. Risk Catalog

| Risk ID | Invalid condition or harm |
|---|---|
| `R-001` | Action without attributable legitimate authority |
| `R-002` | Unknown identity or baseline gains unrestricted operation |
| `R-003` | Unknown or stale fitness retains authority |
| `R-004` | Material history cannot be reconstructed |
| `R-005` | Restricted subject can prevent or bypass Guardian restriction |
| `R-006` | Repair or recovery approves its own completion |
| `R-007` | Verification creates financial or live-capital consequence |
| `R-008` | Implementation silently redefines an Approved obligation |
| `R-009` | Invalid lifecycle transition changes authoritative state |
| `R-010` | Malformed, replayed, expired, or unauthorized FIL is accepted |
| `R-011` | Duplicate, reordered, or ambiguous operation creates unintended effects |
| `R-012` | Persistence uncertainty is represented as acknowledged success |
| `R-013` | Evidence is missing, mutable, substituted, or self-validating |
| `R-014` | Dependency, tool, artifact, or runner identity drifts |
| `R-015` | Cryptographic key, domain, nonce, or custody is misused |
| `R-016` | Time, uncertainty, causality, or epoch is misinterpreted |
| `R-017` | Identifier collision, reuse, or disclosure violates identity policy |
| `R-018` | Configuration source, precedence, or effective state is ambiguous |
| `R-019` | Fault crosses containment boundaries or restoration occurs prematurely |
| `R-020` | Plugin or external dependency acquires hidden coupling or authority |
| `R-021` | Evaluation, acceptance, reliance, or promotion exceeds scope or jurisdiction |
| `R-022` | Build or verification is incomplete, irreproducible, or provider-dependent |
| `R-023` | Self-maintenance or evolution bypasses governance and independent verification |
| `R-024` | Sensitive information, credential, or secret is exposed |
| `R-025` | Canonical representations disagree across platforms |
| `R-026` | A Claim becomes trusted solely through classification or producer assertion |

## 9. Verification Method Catalog

| Method ID | Method |
|---|---|
| `VM-REV` | Governed document and authority review |
| `VM-STA` | Static analysis and structural boundary verification |
| `VM-UNT` | Isolated unit verification |
| `VM-CON` | Contract conformance and negative Contract verification |
| `VM-SCH` | Schema and canonical-vector verification |
| `VM-SEC` | Security and abuse-case verification |
| `VM-INT` | Windows and Linux integration verification |
| `VM-FLT` | Fault injection and degraded-state verification |
| `VM-REC` | Recovery, reconciliation, and independent restoration verification |
| `VM-REP` | Independent reproducibility and byte comparison |
| `VM-AUD` | Evidence reconstruction and audit |
| `VM-CHR` | Challenge and independent authority review |
| `VM-PRV` | Provenance, SBOM, digest, and signature verification |
| `VM-TIM` | Time, uncertainty, epoch, and rollback verification |
| `VM-ORD` | Duplicate, reordering, replay, and idempotency verification |

## 10. Evidence Type Catalog

| Evidence ID | Evidence family |
|---|---|
| `EV-DEC` | Governing decision, jurisdiction, delegation, and authority evidence |
| `EV-MAN` | Canonical manifest, baseline, source, toolchain, and environment identity |
| `EV-LOG` | Immutable observations, logs, measurements, and failures |
| `EV-TST` | Test discovery, execution, result, retry, and coverage evidence |
| `EV-CON` | Contract and schema conformance evidence |
| `EV-SEC` | Threat, abuse, secret, vulnerability, and security review evidence |
| `EV-INT` | Platform integration and boundary evidence |
| `EV-FLT` | Fault, restriction, degradation, and Safe-state evidence |
| `EV-REC` | Recovery, reconciliation, independent validation, and restoration evidence |
| `EV-PRV` | Dependency provenance, SBOM, artifact digest, and build provenance |
| `EV-REP` | Independent rebuild and comparison evidence |
| `EV-TIM` | Time Observation, uncertainty, Clock Quality, and Runtime Epoch evidence |
| `EV-CHL` | Challenge, conflict, resolution, and supersession evidence |
| `EV-ROOT` | Root Verification Evidence Set and completeness evaluation |

## 11. Pipeline Gate Catalog

| Gate ID | PIPE-001 stage | Primary decision |
|---|---|---|
| `G-00` | Requirement Set Sealing | Are all obligations fixed before evidence production? |
| `G-01` | Request and Authority Admission | Is the request legitimate and within jurisdiction? |
| `G-02` | Governance and Scope | Is the scope compliant and non-financial? |
| `G-03` | Input and Environment Admission | Are source, environment, time, and isolation proven? |
| `G-04` | Dependency Acquisition and Admission | Are dependencies exact, safe, and admitted? |
| `G-05` | Isolated Build | Was the artifact built deterministically from verified inputs? |
| `G-06` | Static Quality and Boundaries | Do static and structural rules pass? |
| `G-07` | Unit, Contract, Schema, and Vector | Do behavior and boundary Contracts pass? |
| `G-08` | Security Verification | Do security and abuse cases pass? |
| `G-09` | Platform Integration | Does the exact candidate integrate on Windows and Linux? |
| `G-10` | Fault, Degraded, and Recovery | Does failure produce restriction and governed recovery? |
| `G-11` | VPL Execution | Do all applicable FRS-001 scenarios pass? |
| `G-12` | Reproducibility | Can independent environments reproduce the candidate? |
| `G-13` | Packaging, SBOM, and Provenance | Are artifacts immutable, attributable, and fully described? |
| `G-14` | Root Evidence Set Construction | Is the complete governed verification case preserved? |
| `G-15` | Completeness Evaluation | Is every applicable obligation evaluated? |
| `G-16` | Validity and Acceptance | Is the case valid and accepted for the declared scope? |
| `G-17` | Promotion Decision | May this exact artifact advance? |

## 12. Release Invariant Matrix

| Invariant | Primary risks | Subjects | Methods | Evidence | Gates | VPL |
|---|---|---|---|---|---|---|
| `FRS-001-INV-001` | R-001, R-021 | AUT-001; CON-002; CON-009 | VM-REV, VM-CON, VM-SEC, VM-CHR | EV-DEC, EV-CON, EV-SEC | G-01, G-07, G-08, G-16 | VPL-001, VPL-002, VPL-003, VPL-004, VPL-008 |
| `FRS-001-INV-002` | R-002, R-014 | SYS-001; CON-001; CON-010 | VM-CON, VM-SEC, VM-INT | EV-MAN, EV-CON, EV-INT | G-03, G-07, G-09 | VPL-001 |
| `FRS-001-INV-003` | R-003 | AWR-001; SYS-008; CON-006 | VM-CON, VM-FLT | EV-CON, EV-FLT | G-07, G-10 | VPL-005 |
| `FRS-001-INV-004` | R-004, R-013 | DEC-006; OPS-004; CON-005; CON-008 | VM-CON, VM-AUD | EV-LOG, EV-CON, EV-ROOT | G-07, G-14, G-15 | VPL-002 through VPL-008 |
| `FRS-001-INV-005` | R-005 | AUT-002; RSK-005; CON-011 | VM-SEC, VM-FLT | EV-SEC, EV-FLT | G-08, G-10 | VPL-006 |
| `FRS-001-INV-006` | R-006, R-019, R-021 | OPS-003; AUT-002; GOV-AUT-001 | VM-REC, VM-CHR | EV-REC, EV-CHL, EV-DEC | G-10, G-16 | VPL-007 |
| `FRS-001-INV-007` | R-007 | FRS-001; ENV-001; PIPE-001 | VM-REV, VM-STA, VM-INT | EV-MAN, EV-INT | G-02, G-03, G-09 | VPL-001 through VPL-008 |
| `FRS-001-INV-008` | R-008, R-022 | GOV-001; TRC-001; PIPE-001 | VM-REV, VM-STA, VM-AUD | EV-DEC, EV-MAN, EV-ROOT | G-00, G-02, G-06, G-15 | VPL-001, VPL-008 |

Failure of any release invariant is non-waivable for FRS-001.

## 13. Scenario Matrix

| Scenario | Plan | Preconditions | Primary subjects | Gates | Required result |
|---|---|---|---|---|---|
| `FRS-SCN-001` Trusted Bootstrap | VPL-001 | Approved baseline; isolated environment | SYS-001; CON-001; CON-007; CON-009; CON-010 | G-03, G-07, G-09, G-11 | `PASS` |
| `FRS-SCN-002` Unauthorized Action | VPL-002 | Trusted bootstrap | AUT-001; CON-002; CON-009 | G-07, G-08, G-11 | `PASS` by proven denial |
| `FRS-SCN-003` Invalid Lifecycle Transition | VPL-003 | Trusted bootstrap | SYS-002; CON-003 | G-07, G-10, G-11 | `PASS` by rejection without state corruption |
| `FRS-SCN-004` Invalid FIL Message | VPL-004 | Trusted bootstrap; FIL boundary | SYS-005; SYS-009; CON-004 | G-07, G-08, G-11 | `PASS` by explicit rejection |
| `FRS-SCN-005` Health Evidence Loss | VPL-005 | Valid prior health | SYS-008; AWR-001; CON-006 | G-07, G-10, G-11 | `PASS` by reduced authority |
| `FRS-SCN-006` Guardian Restriction | VPL-006 | Admitted protective condition | AUT-002; RSK-005; CON-011 | G-08, G-10, G-11 | `PASS` by enforceable restriction |
| `FRS-SCN-007` Controlled Recovery | VPL-007 | Restriction established | OPS-003; AUT-002; CON-011 | G-10, G-11, G-16 | `PASS` by independent restoration approval |
| `FRS-SCN-008` Evidence Reconstruction | VPL-008 | VPL-001 through VPL-007 evidence | DEC-006; OPS-004; CON-005; CON-008 | G-11, G-14, G-15 | `PASS` by complete reconstruction |

## 14. Contract Forward Matrix

| Requirement range | Class | Risks | Primary methods | Evidence | Gates | VPL |
|---|---|---|---|---|---|---|
| `CON-001-REQ-001..008` | RC-CON, RC-IDN | R-002, R-017 | VM-CON, VM-SEC, VM-INT | EV-CON, EV-MAN | G-03, G-07, G-09 | VPL-001, VPL-008 |
| `CON-002-REQ-001..008` | RC-CON, RC-AUT | R-001, R-021 | VM-CON, VM-SEC, VM-CHR | EV-DEC, EV-CON | G-01, G-07, G-08, G-16 | VPL-002, VPL-008 |
| `CON-003-REQ-001..008` | RC-CON, RC-BEH | R-009 | VM-CON, VM-FLT | EV-CON, EV-FLT | G-07, G-10 | VPL-003, VPL-008 |
| `CON-004-REQ-001..015` | RC-CON, RC-SEC | R-010, R-011, R-016 | VM-CON, VM-SCH, VM-SEC, VM-ORD | EV-CON, EV-SEC, EV-TIM | G-07, G-08, G-10 | VPL-004, VPL-008 |
| `CON-005-REQ-001..008` | RC-CON, RC-EVD | R-004, R-011, R-013 | VM-CON, VM-ORD, VM-AUD | EV-CON, EV-LOG | G-07, G-10, G-14 | VPL-008 |
| `CON-006-REQ-001..008` | RC-CON, RC-OPS | R-003, R-013, R-016 | VM-CON, VM-FLT, VM-TIM | EV-CON, EV-FLT, EV-TIM | G-07, G-10 | VPL-005, VPL-008 |
| `CON-007-REQ-001..008` | RC-CON, RC-BEH | R-018 | VM-CON, VM-INT, VM-AUD | EV-CON, EV-INT, EV-LOG | G-07, G-09 | VPL-001, VPL-008 |
| `CON-008-REQ-001..008` | RC-CON, RC-EVD | R-004, R-013 | VM-CON, VM-AUD, VM-SEC | EV-CON, EV-LOG, EV-SEC | G-07, G-08, G-14 | VPL-008 |
| `CON-009-REQ-001..008` | RC-CON, RC-SEC, RC-AUT | R-001, R-002, R-021, R-024 | VM-CON, VM-SEC, VM-CHR | EV-DEC, EV-CON, EV-SEC | G-01, G-07, G-08 | VPL-001, VPL-002, VPL-004, VPL-008 |
| `CON-010-REQ-001..010` | RC-CON, RC-EVD | R-002, R-008, R-014 | VM-CON, VM-SCH, VM-PRV | EV-MAN, EV-CON, EV-PRV | G-02, G-03, G-07, G-13 | VPL-001, VPL-008 |
| `CON-011-REQ-001..012` | RC-CON, RC-SAF | R-005, R-006, R-019, R-021 | VM-CON, VM-SEC, VM-FLT, VM-REC | EV-CON, EV-FLT, EV-REC | G-07, G-08, G-10 | VPL-006, VPL-007, VPL-008 |

## 15. Specification and Governance Forward Matrix

The following ranges cover all identified obligations in the Approved Foundation specification and governance set.

| Requirement range | Primary class | Risks | Methods | Gates | Principal VPL |
|---|---|---|---|---|---|
| `AUT-001-REQ-001..045` | RC-AUT, RC-SEC | R-001, R-005, R-021 | VM-REV, VM-CON, VM-SEC, VM-CHR | G-01, G-07, G-08, G-16 | VPL-002, VPL-006, VPL-007, VPL-008 |
| `AUT-002-REQ-001..015` | RC-SAF, RC-AUT | R-003, R-005, R-006, R-019 | VM-CON, VM-SEC, VM-FLT, VM-REC | G-07, G-08, G-10 | VPL-005, VPL-006, VPL-007, VPL-008 |
| `AWR-001-REQ-001..020` | RC-BEH, RC-SAF | R-003, R-013, R-016, R-019 | VM-CON, VM-FLT, VM-AUD | G-07, G-10, G-14 | VPL-005, VPL-006, VPL-007, VPL-008 |
| `DEC-006-REQ-001..018` | RC-EVD | R-004, R-013, R-021, R-026 | VM-CON, VM-AUD, VM-CHR | G-07, G-14, G-15, G-16 | VPL-002 through VPL-008 |
| `EVO-001-REQ-001..024` | RC-BEH, RC-AUT | R-006, R-008, R-019, R-023 | VM-REV, VM-SEC, VM-FLT, VM-REC, VM-CHR | G-02, G-08, G-10, G-16 | VPL-006, VPL-007, VPL-008 |
| `OPS-003-REQ-001..015` | RC-OPS, RC-SAF | R-006, R-012, R-019 | VM-FLT, VM-REC, VM-CHR | G-10, G-16 | VPL-007, VPL-008 |
| `OPS-004-REQ-001..015` | RC-EVD, RC-OPS | R-004, R-013, R-024 | VM-CON, VM-SEC, VM-AUD | G-07, G-08, G-14 | VPL-002 through VPL-008 |
| `PLG-001-REQ-001..018` | RC-SEC, RC-BEH | R-001, R-014, R-020, R-021, R-024 | VM-STA, VM-CON, VM-SEC, VM-FLT | G-04, G-06, G-07, G-08, G-10 | VPL-002, VPL-006, VPL-008 |
| `RSK-005-REQ-001..016` | RC-SAF | R-003, R-005, R-007, R-019 | VM-SEC, VM-FLT, VM-REC | G-02, G-08, G-10 | VPL-005, VPL-006, VPL-007 |
| `SEC-001-REQ-001..028` | RC-SEC | R-001, R-002, R-010, R-013, R-015, R-020, R-021, R-024 | VM-STA, VM-CON, VM-SEC, VM-CHR | G-01, G-04, G-06, G-07, G-08, G-16 | VPL-001, VPL-002, VPL-004, VPL-006, VPL-008 |
| `SEC-002-REQ-001..018` | RC-SEC, RC-EVD | R-004, R-013, R-021, R-026 | VM-REV, VM-SEC, VM-AUD, VM-CHR | G-08, G-14, G-15, G-16, G-17 | VPL-007, VPL-008 |
| `SYS-001-REQ-001..014` | RC-BEH, RC-SEC | R-002, R-003, R-008, R-014, R-019 | VM-STA, VM-CON, VM-INT, VM-FLT | G-03, G-06, G-07, G-09, G-10 | VPL-001, VPL-005, VPL-006, VPL-008 |
| `SYS-002-REQ-001..015` | RC-BEH, RC-SAF | R-005, R-006, R-009, R-019 | VM-CON, VM-FLT, VM-REC | G-07, G-10 | VPL-003, VPL-006, VPL-007, VPL-008 |
| `SYS-005-REQ-001..022` | RC-BEH, RC-SEC | R-010, R-011, R-019, R-020 | VM-CON, VM-SEC, VM-ORD, VM-FLT | G-07, G-08, G-09, G-10 | VPL-004, VPL-006, VPL-008 |
| `SYS-007-REQ-001..015` | RC-BEH, RC-SEC | R-002, R-008, R-018, R-024 | VM-CON, VM-SEC, VM-INT, VM-AUD | G-03, G-07, G-08, G-09 | VPL-001, VPL-008 |
| `SYS-008-REQ-001..015` | RC-OPS, RC-SAF | R-003, R-013, R-016, R-019 | VM-CON, VM-FLT, VM-TIM, VM-REC | G-07, G-10 | VPL-005, VPL-006, VPL-007, VPL-008 |
| `SYS-009-REQ-001..023` | RC-BEH, RC-SEC | R-010, R-011, R-015, R-016, R-025 | VM-CON, VM-SCH, VM-SEC, VM-ORD, VM-TIM | G-07, G-08, G-10 | VPL-004, VPL-008 |
| `SYS-010-REQ-001..015` | RC-EVD, RC-BEH | R-004, R-011, R-013, R-016 | VM-CON, VM-ORD, VM-AUD | G-07, G-10, G-14 | VPL-008 |
| `SYS-011-REQ-001..016` | RC-OPS, RC-EVD | R-004, R-011, R-012, R-013, R-019 | VM-CON, VM-INT, VM-FLT, VM-REC, VM-AUD | G-07, G-09, G-10, G-14 | VPL-003, VPL-007, VPL-008 |

## 16. Catalog, Design, Build, and Pipeline Forward Matrix

| Requirement range | Primary class | Risks | Methods | Gates | Principal VPL |
|---|---|---|---|---|---|
| `BLD-001-REQ-001..023` | RC-BLD | R-007, R-008, R-013, R-014, R-022, R-024 | VM-REV, VM-STA, VM-SEC, VM-REP, VM-PRV | G-02 through G-06, G-08, G-12, G-13 | VPL-001, VPL-008 |
| `CRY-001-REQ-001..030` | RC-CRY, RC-SEC | R-010, R-013, R-015, R-020, R-024 | VM-SCH, VM-SEC, VM-FLT, VM-CHR | G-07, G-08, G-10, G-16 | VPL-001, VPL-004, VPL-006, VPL-008 |
| `DESIGN-SEC-001-REQ-001..034` | RC-CRY, RC-SEC | R-002, R-006, R-015, R-019, R-024 | VM-STA, VM-SEC, VM-FLT, VM-REC, VM-CHR | G-03, G-06, G-08, G-10, G-16 | VPL-001, VPL-004, VPL-006, VPL-007, VPL-008 |
| `FCE-001-REQ-001..032` | RC-REP | R-004, R-010, R-015, R-016, R-017, R-025 | VM-SCH, VM-INT, VM-REP | G-07, G-09, G-12 | VPL-001, VPL-004, VPL-008 |
| `ENV-001-REQ-001..030` | RC-BLD, RC-SEC, RC-OPS | R-002, R-007, R-013, R-014, R-015, R-016, R-019, R-022, R-024 | VM-REV, VM-STA, VM-SEC, VM-INT, VM-FLT, VM-REP, VM-PRV, VM-TIM | G-02 through G-13, G-15, G-16 | VPL-001 through VPL-008 |
| `GOV-AUT-001-REQ-001..024` | RC-AUT | R-001, R-006, R-021, R-026 | VM-REV, VM-SEC, VM-CHR, VM-AUD | G-01, G-02, G-08, G-16, G-17 | VPL-002, VPL-006, VPL-007, VPL-008 |
| `GOV-SEC-001-REQ-001..025` | RC-AUT, RC-SEC | R-001, R-006, R-015, R-021, R-024 | VM-REV, VM-SEC, VM-CHR, VM-AUD | G-01, G-02, G-08, G-16 | VPL-001, VPL-006, VPL-007, VPL-008 |
| `IDN-001-REQ-001..030` | RC-IDN | R-002, R-011, R-016, R-017, R-025 | VM-CON, VM-SCH, VM-SEC, VM-INT | G-03, G-07, G-08, G-09 | VPL-001, VPL-004, VPL-008 |
| `PIPE-001-REQ-001..030` | RC-PIPE, RC-EVD | R-007, R-008, R-013, R-014, R-021, R-022, R-026 | VM-REV, VM-STA, VM-AUD, VM-CHR, VM-REP, VM-PRV | G-00 through G-17 | VPL-001 through VPL-008 |
| `TIM-001-REQ-001..030` | RC-TIM | R-003, R-010, R-011, R-016, R-025 | VM-CON, VM-SCH, VM-TIM, VM-FLT, VM-INT | G-03, G-07, G-08, G-09, G-10 | VPL-001, VPL-004, VPL-005, VPL-006, VPL-008 |
| `TRC-001-REQ-001..020` | RC-EVD, RC-PIPE | R-008, R-013, R-021, R-022, R-026 | VM-REV, VM-STA, VM-AUD, VM-CHR | G-00, G-02, G-14, G-15, G-16 | VPL-008 |

## 17. Reverse VPL Matrix

| VPL | Governing invariants | Required Contract families | Principal requirement families |
|---|---|---|---|
| VPL-001 | INV-001, INV-002, INV-007, INV-008 | CON-001, CON-007, CON-009, CON-010 | SYS-001, SYS-007, SEC-001, IDN-001, TIM-001, FCE-001, BLD-001 |
| VPL-002 | INV-001, INV-004, INV-007 | CON-002, CON-008, CON-009 | AUT-001, GOV-AUT-001, SEC-001, DEC-006, OPS-004 |
| VPL-003 | INV-001, INV-004, INV-007 | CON-003, CON-008 | SYS-002, SYS-011, DEC-006, OPS-004 |
| VPL-004 | INV-001, INV-004, INV-007 | CON-004, CON-008, CON-009 | SYS-005, SYS-009, SEC-001, FCE-001, CRY-001, IDN-001, TIM-001 |
| VPL-005 | INV-003, INV-004, INV-007 | CON-006, CON-008 | SYS-008, AWR-001, AUT-002, RSK-005, TIM-001 |
| VPL-006 | INV-004, INV-005, INV-007 | CON-006, CON-008, CON-011 | AUT-002, RSK-005, SYS-002, SYS-008, EVO-001 |
| VPL-007 | INV-004, INV-006, INV-007 | CON-008, CON-011 | OPS-003, AUT-002, EVO-001, GOV-AUT-001, SEC-002 |
| VPL-008 | INV-001, INV-004, INV-007, INV-008 | CON-001 through CON-011 | Every requirement family in Sections 14 through 16 |

VPL trace identifies scenario-level demonstration. It does not replace lower-level unit, Contract, security, integration, fault, or reproducibility evidence.

## 18. Reverse Gate Matrix

| Gate | Required source families |
|---|---|
| G-00 | PIPE-001; TRC-001; FRS-001; all applicable approved requirement families |
| G-01 | GOV-AUT-001; GOV-SEC-001; AUT-001; SEC-001; CON-002; CON-009 |
| G-02 | Vision; Constitution; FRS-001; GOV-001; RSK-005; EVO-001; BLD-001; PIPE-001 |
| G-03 | SYS-001; SYS-007; SEC-001; IDN-001; TIM-001; BLD-001; ENV-001; CON-001; CON-007; CON-009; CON-010 |
| G-04 | BLD-001; PLG-001; SEC-001; CRY-001 |
| G-05 | BLD-001; PIPE-001; FCE-001 |
| G-06 | BLD-001; SYS-001; PLG-001; SEC-001; DESIGN-SEC-001 |
| G-07 | CON-001 through CON-011; all behavioral Specifications; FCE-001; IDN-001; TIM-001; CRY-001 |
| G-08 | SEC-001; SEC-002; AUT-001; AUT-002; GOV-AUT-001; GOV-SEC-001; CRY-001; DESIGN-SEC-001 |
| G-09 | SYS-001 through SYS-011 where Approved; BLD-001; ENV-001; FCE-001; IDN-001; TIM-001 |
| G-10 | AUT-002; RSK-005; AWR-001; OPS-003; SYS-002; SYS-005; SYS-008; SYS-011; TIM-001 |
| G-11 | FRS-001; VPL-000 through VPL-008; all scenario-linked families |
| G-12 | BLD-001; ENV-001; PIPE-001; FCE-001 |
| G-13 | BLD-001; PIPE-001; SEC-002; CON-008; CON-010; OPS-004 |
| G-14 | SEC-002; PIPE-001; DEC-006; OPS-004; CON-005; CON-008 |
| G-15 | SEC-002; PIPE-001; TRC-001; Evidence Requirement Set |
| G-16 | GOV-AUT-001; SEC-002; PIPE-001; applicable acceptance authorities |
| G-17 | GOV-AUT-001; SEC-002; PIPE-001; exact Root Verification Evidence Set |

## 19. Forward Completeness Rules

Every Approved atomic requirement in scope SHALL map to:

1. at least one Risk ID;
2. at least one affected subject;
3. at least one verification method;
4. at least one evidence type;
5. at least one Pipeline Gate;
6. an explicit VPL mapping or the reason scenario-level demonstration is not applicable;
7. a result rule;
8. an independence rule; and
9. a trace lifecycle state.

Absence of any mandatory forward link makes the trace `INCOMPLETE`.

## 20. Reverse Completeness Rules

Every:

- Gate;
- VPL;
- Contract test;
- security case;
- integration case;
- fault case;
- recovery case;
- artifact;
- evidence type; and
- promotion condition

SHALL trace back to at least one Approved obligation.

Evidence with no governing obligation is orphan evidence.

An evaluation with no governing rule is an orphan conclusion.

A test with no requirement may be retained as exploratory evidence, but it SHALL NOT satisfy mandatory completeness until governed.

## 21. Atomic Refinement

Before PIPE-001 Activation, the machine-readable execution representation SHALL expand this Matrix to one record per immutable requirement identifier.

Atomic refinement MAY:

- narrow Risk IDs;
- add a Contract;
- add a verification method;
- add evidence;
- add a Gate;
- add a VPL;
- impose stricter independence; or
- impose stricter platform coverage.

Atomic refinement SHALL NOT:

- remove a family-level mapping;
- weaken a requirement;
- reduce a non-waivable Gate;
- change source meaning;
- invent authority; or
- represent missing analysis as not applicable.

A conflicting refinement requires TRC-001 review.

## 22. Orphan and Conflict States

Trace state is:

| State | Meaning |
|---|---|
| `COMPLETE` | Required forward and reverse links exist |
| `INCOMPLETE` | One or more mandatory links are absent |
| `CONFLICTED` | Sources or mappings disagree materially |
| `STALE` | A governing source changed after the trace snapshot |
| `INVALID` | Identity, integrity, provenance, or source authority failed |
| `NOT_APPLICABLE` | Explicitly outside the declared baseline under governing authority |

Unknown is not complete.

## 23. Change Impact

A change to any Approved:

- requirement;
- Contract;
- invariant;
- risk;
- Gate;
- VPL;
- evidence schema;
- environment;
- toolchain;
- authority model; or
- promotion rule

SHALL:

1. identify affected trace records;
2. mark them `STALE` before relying on prior evidence;
3. evaluate forward impact;
4. evaluate reverse impact;
5. update the Matrix or atomic expansion;
6. update verification obligations;
7. preserve historical trace state; and
8. require new evidence where material.

Changing a title or file path without changing the immutable requirement meaning may preserve the Requirement ID, but provenance SHALL record the movement.

## 24. Trace Integrity

TRC-001 and every execution expansion are Trust Objects under SEC-002.

They SHALL have:

- identity;
- version;
- canonical digest;
- source snapshot;
- provenance;
- lineage;
- validity scope;
- lifecycle;
- supersession;
- challenge path; and
- authority evidence.

The author of a requirement SHALL NOT be the sole authority declaring its verification trace complete for material promotion.

## 25. Matrix Requirements

- **TRC-001-REQ-001:** Every Approved Foundation requirement SHALL have forward and reverse traceability.
- **TRC-001-REQ-002:** Compact ranges SHALL expand to independent atomic trace records.
- **TRC-001-REQ-003:** A range SHALL NOT permit sampling or shared pass.
- **TRC-001-REQ-004:** Every requirement SHALL map to risk, subject, method, evidence, Gate, result, and independence.
- **TRC-001-REQ-005:** Every Gate and governed verification item SHALL trace to an Approved obligation.
- **TRC-001-REQ-006:** Orphan evidence SHALL NOT satisfy mandatory completeness.
- **TRC-001-REQ-007:** Orphan conclusions SHALL NOT authorize acceptance or promotion.
- **TRC-001-REQ-008:** Every FRS-001 invariant SHALL remain non-waivable.
- **TRC-001-REQ-009:** VPL mapping SHALL supplement, not replace, lower-level verification.
- **TRC-001-REQ-010:** Source text SHALL remain authoritative over trace summaries.
- **TRC-001-REQ-011:** Source change SHALL make affected trace records stale until reviewed.
- **TRC-001-REQ-012:** Historical trace snapshots SHALL remain immutable.
- **TRC-001-REQ-013:** Atomic refinement MAY strengthen but SHALL NOT weaken family mappings.
- **TRC-001-REQ-014:** Missing or ambiguous trace SHALL produce `INCOMPLETE`, `CONFLICTED`, or `INVALID`, never `COMPLETE`.
- **TRC-001-REQ-015:** Trace completeness SHALL be independently evaluated.
- **TRC-001-REQ-016:** Trace status SHALL NOT be represented as verification result.
- **TRC-001-REQ-017:** TRC-001 SHALL be included in the Evidence Requirement Set and Root Verification Evidence Set.
- **TRC-001-REQ-018:** No financial requirement or verification path SHALL enter FRS-001 through trace expansion.
- **TRC-001-REQ-019:** Execution-time expansion SHALL preserve exact source version and location.
- **TRC-001-REQ-020:** Approval of TRC-001 SHALL NOT authorize implementation, evidence production, Pipeline Activation, or promotion.

## 26. Conformance Evidence

Conformance requires proof that:

- all 776 snapshot requirement IDs expand exactly once;
- no range contains a missing or duplicate ID;
- every FRS-001 invariant has forward and reverse links;
- every FRS-001 scenario maps to one Approved VPL;
- every Contract family maps to at least one Gate and verification method;
- every Gate maps back to Approved sources;
- every VPL maps back to invariants and requirement families;
- orphan requirements are detected;
- orphan tests and evidence are detected;
- source changes mark affected records stale;
- atomic refinements cannot remove inherited mappings;
- trace summaries cannot override source text;
- incomplete trace blocks Evidence Set completeness;
- historical snapshots remain reconstructable; and
- no trace introduces a financial path.

## 27. Known Limitations and Deliberate Blocks

At version 1.0:

- the normative human-readable snapshot is complete at atomic range semantics;
- the illustrative `DEC-001-REQ-014` text in STD-002 is excluded because it is an identifier-format example, not an Approved requirement;
- machine-readable atomic expansion has not been produced;
- ENV-001 is not Approved;
- Gate Profiles remain `PROPOSED`;
- no Verification Session has executed;
- no trace record has a verification result;
- no Evidence Requirement Set exists; and
- no completeness or promotion Claim is made.

These conditions prevent Pipeline Activation.

## 28. Required Before Activation

TRC-001 execution use requires:

1. TRC-001 Approval;
2. registration as a governed Foundation Matrix;
3. machine-readable expansion of every atomic identifier;
4. zero missing or duplicate identifiers;
5. exact source version and location binding;
6. independent forward and reverse completeness review;
7. approved schemas and canonical encoding;
8. integration with PIPE-001 Evidence Requirement Sets;
9. ENV-001 Approval;
10. Gate Profile Approval and Activation;
11. challenge and supersession verification;
12. financial-isolation verification; and
13. explicit implementation authority.

## 29. Foundational Rules

> **A requirement without a verification path is an unproven obligation.**

> **Evidence without a governing requirement is an orphan Claim.**

> **A passing test does not satisfy a requirement it was never governed to verify.**

> **Traceability preserves meaning across change; it does not create meaning.**

> **Nothing may disappear between obligation and evidence.**

## 30. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner and current Constitutional Authority | Approved | GOV-021 | 2026-07-25 |
| Project Owner and current Constitutional Authority | Approved v1.1 trace extension for ENV-001 | GOV-022 | 2026-07-25 |

Approval approves:

- the traceability model;
- the risk, method, evidence, and Gate catalogs;
- the invariant and scenario mappings;
- the Contract mappings;
- complete atomic-range coverage of the 776 identified requirements;
- reverse VPL and Gate mappings; and
- the deliberate activation blocks.

It does not:

- claim that tests or evidence exist;
- activate an atomic trace implementation;
- activate a Gate Profile or Pipeline;
- activate ENV-001 or an Environment Profile;
- authorize implementation;
- authorize promotion;
- authorize production; or
- authorize financial activity.
