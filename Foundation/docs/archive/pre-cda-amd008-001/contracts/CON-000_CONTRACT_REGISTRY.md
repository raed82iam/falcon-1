# Falcon Contract Registry

**Identifier:** CON-000  
**Version:** 1.6  
**Status:** Approved  
**Effective Date:** 2026-07-25  
**Approval Record:** GOV-030  
**Owner:** Falcon Contract Authority
**Supersedes:** CON-000 v1.5

## Purpose

This registry controls the contracts required to implement the Foundation Release.

A Contract defines an observable boundary between governed participants. It does not choose the technology that realizes the boundary.

## Foundation Contracts

| ID | Contract | Status | Primary Specification |
|---|---|---|---|
| CON-001 | Core Identity | Approved | SYS-001 |
| CON-002 | Authority Decision | Approved | AUT-001 |
| CON-003 | Lifecycle | Approved | SYS-002 |
| CON-004 | FIL Envelope | Approved | SYS-009 |
| CON-005 | Event | Approved | SYS-010 |
| CON-006 | Health and Fitness | Approved | SYS-008, AWR-001 |
| CON-007 | Configuration | Approved | SYS-007 |
| CON-008 | Evidence and Logging v1.1 | Approved and Active | OPS-004, DEC-006, SEC-002, CON-021 |
| CON-009 | Security Context | Approved | SEC-001 |
| CON-010 | Foundation Baseline Manifest v1.1 | Approved and Active | SYS-001, SEC-001, SYS-007, ADR-I008 |
| CON-011 | Protective Restriction | Approved | AUT-001, AUT-002, SYS-002, OPS-003 |
| CON-012 | Authority Instrument | Approved | GOV-AUT-001, AUT-001, SEC-002 |
| CON-013 | Delegation and Revocation | Approved | GOV-AUT-001, AUT-001, SEC-002 |
| CON-014 | Identifier Provider | Approved | IDN-001, FCE-001, SEC-001, SEC-002 |
| CON-015 | Time Provider | Approved | TIM-001, FCE-001, SEC-001, SEC-002 |
| CON-016 | Cryptographic Provider | Approved | CRY-001, SEC-001, SEC-002, DESIGN-SEC-001 |
| CON-017 | Secret Provider | Approved | CRY-001, SEC-001, SEC-002, DESIGN-SEC-001 |
| CON-018 | Certificate and Identity Provider | Approved | CRY-001, SEC-001, SEC-002, DESIGN-SEC-001 |
| CON-019 | Randomness Provider | Approved | CRY-001, IDN-001, SEC-001, DESIGN-SEC-001 |
| CON-020 | Bootstrap Execution Context | Approved | ADR-I008, GOV-AUT-001, ENV-001, SEC-002 |
| CON-021 | Bootstrap Evidence and Provenance | Approved | ADR-I008, SEC-002, FCE-001, CON-008 |

## Contract Rule

Every Contract SHALL define:

- participants;
- authority;
- inputs and outputs;
- preconditions and postconditions;
- invariants;
- errors and rejection;
- compatibility;
- evidence;
- security; and
- acceptance examples.

Serialization, transport, storage, framework, and programming-language choices require ADRs or design decisions and SHALL NOT be inferred from a Contract.
