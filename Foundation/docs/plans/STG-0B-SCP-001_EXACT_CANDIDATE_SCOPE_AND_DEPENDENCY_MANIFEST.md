# STG-0B-SCP-001 — Exact Candidate Scope and Dependency Manifest

**Identifier:** STG-0B-SCP-001  
**Version:** 1.0  
**Status:** Approved  
**Proposal Date:** 2026-07-26  
**Approval Date:** 2026-07-26  
**Governing Authority:** AMD-003; IMP-001 v1.2; STG-0B-PROP-001  
**Approval Record:** GOV-051  
**Implementation Authority:** Granted for the enumerated candidates only

## 1. Purpose

This manifest fixes the maximum Stage 0B candidate scope.

## 2. Enumerated Candidate Subjects

| Candidate ID | Subject | Governing Baseline |
|---|---|---|
| CND-FCE-001 | Canonical Encoding support | FCE-001 |
| CND-TRUST-001 | Trust Object and evidence primitives | SEC-002 |
| CND-IDN-001 | Identifier Provider | CON-014; IDN-001; VPL-BST-003 |
| CND-TIM-001 | Time Provider | CON-015; TIM-001; VPL-BST-004 |
| CND-CRY-001 | Cryptographic Provider Adapter | CON-016; CRY-001; DESIGN-SEC-001; VPL-BST-005 |
| CND-SEC-001 | Secret Provider | CON-017; DESIGN-SEC-001; VPL-BST-005 |
| CND-CID-001 | Certificate and Identity Provider | CON-018; DESIGN-SEC-001; VPL-BST-005 |
| CND-RND-001 | Randomness Provider Adapter | CON-019; CRY-001; VPL-BST-005 |
| CND-TRC-001 | Machine-readable traceability expansion support | TRC-001 |
| CND-PIPE-001 | Bootstrap Pipeline harness | PIPE-001; BLD-001 |
| CND-FIX-001 | Isolated verification fixtures | VPL-BST-003 through VPL-BST-005 |

No other candidate is within scope.

## 3. Dependency Policy

Permitted dependency classes:

- .NET 10 SDK materials admitted by STG-0B-BLD-001;
- .NET Base Class Library;
- Falcon-owned candidate Contracts and test fixtures;
- approved Foundation documents;
- and synthetic material declared by STG-0B-SYN-001.

External runtime or package dependencies are excluded from the initial Stage 0B scope.

Any external dependency need shall stop the affected candidate and require a new documented decision.

## 4. Replaceability

Platform and vendor functions shall remain behind Falcon-owned Contracts or Adapters.

No candidate may expose:

- vendor types;
- platform-specific semantics;
- database-specific semantics;
- cryptographic-provider-specific semantics;
- Windows-specific meaning;
- or external serialization formats across Falcon boundaries.

## 5. Explicit Exclusions

Excluded:

- all operational Core behavior;
- FIL operational transport;
- Service Bus;
- Persistence implementation;
- Guardian and Recovery;
- Self-Awareness and Self-Maintenance;
- logging and event runtime;
- financial Domains;
- trading, prediction, planning, risk, execution, and portfolio behavior;
- UI and shared applications;
- cloud deployment;
- and production packaging.

## 6. Scope Change Rule

Adding, merging, reinterpreting, or expanding a Candidate ID requires a new approved scope record.

Implementation convenience shall not expand scope.
