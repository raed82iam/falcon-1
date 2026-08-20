# STG-0C-TRC-001 — Machine-Readable Trace Activation Plan

**Identifier:** STG-0C-TRC-001  
**Version:** 1.0  
**Status:** Approved  
**Prepared:** 2026-07-27  
**Approval Date:** 2026-07-27  
**Approval Record:** GOV-055  
**Governing Authority:** GOV-054; STG-0C-PROP-001; TRC-001; VPL-BST-007  
**Trace Activation:** Not Granted

## 1. Purpose

This candidate defines the bounded Activation case for exact, machine-readable Foundation traceability.

## 2. Required Trace

The trace model shall link:

```text
Authority and Policy
→ Atomic Requirement
→ Contract or Standard
→ Verification Obligation
→ Observed Evidence
→ Derived Evaluation
→ Decision
→ Activated Subject or Explicit Disposition
```

Forward and reverse traversal shall produce the same governed relationships.

## 3. Integrity Rules

- Every node and edge shall have immutable identity, type, provenance, version, and integrity.
- Missing, duplicate, ambiguous, orphaned, cyclicly authoritative, or conflicting links shall fail applicable Gates.
- Supersession shall preserve history; identifiers shall never be reassigned.
- Trace shall not create authority, validity, Acceptance, or Activation.
- Human-readable documents remain authoritative according to GOV-001; machine-readable trace must faithfully represent them.

## 4. Verification

VPL-BST-007 shall test reconstruction, mutation, omission, wrong-version, wrong-scope, and unauthorized-expansion cases.

## 5. Current Effect

No trace expansion is active.
