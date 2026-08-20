# OPS-004 — Logging

**Identifier:** OPS-004  
**Version:** 1.0  
**Status:** Approved  
**Effective Date:** 2026-07-24  
**Approval Record:** GOV-003
**Owner:** Falcon Evidence Authority  
**Governing Authority:** Constitution Articles 13, 16–17, 20, 26, 31–33, 40–44  
**Affected Domains:** All

## 1. Purpose

Logging preserves trustworthy operational records required to understand Falcon behavior, investigate failure, establish accountability, and support verification.

Logs are evidence. They are not automatically the authoritative state of the subject they describe.

## 2. Scope

OPS-004 governs:

- log record identity and structure;
- source attribution;
- severity and classification;
- time and ordering metadata;
- integrity and retention;
- access and redaction;
- correlation;
- delivery failure; and
- audit-relevant records.

## 3. Non-Scope

Logging does not:

- own business or component state;
- replace events;
- grant authority;
- guarantee truth merely because a statement was logged;
- store secrets by default;
- replace metrics or traces; or
- permit retroactive editing of evidence.

## 4. Normative Requirements

- **OPS-004-REQ-001:** Every log record SHALL identify source, record time, severity, event category, and message or structured fact.
- **OPS-004-REQ-002:** Material records SHALL include correlation and causation identifiers where available.
- **OPS-004-REQ-003:** Source identity SHALL be authenticated or its trust limitation SHALL be explicit.
- **OPS-004-REQ-004:** Log severity SHALL follow governed semantics and SHALL NOT be selected solely for convenience.
- **OPS-004-REQ-005:** Material authority decisions, lifecycle transitions, Guardian interventions, security decisions, recovery actions, and configuration changes SHALL produce audit-relevant records.
- **OPS-004-REQ-006:** Sensitive information SHALL be classified, minimized, and redacted according to SEC-001.
- **OPS-004-REQ-007:** Secrets, private keys, credentials, and prohibited financial personal data SHALL NOT appear in ordinary logs.
- **OPS-004-REQ-008:** Accepted records SHALL be protected against undetected alteration and unauthorized deletion.
- **OPS-004-REQ-009:** Retention SHALL be defined by record class, legal obligation, investigation need, and risk.
- **OPS-004-REQ-010:** Logging failure SHALL be observable and SHALL NOT silently discard audit-critical records.
- **OPS-004-REQ-011:** Backpressure SHALL be bounded and SHALL NOT permit logging to cause uncontrolled Core failure.
- **OPS-004-REQ-012:** Loss of audit-critical logging SHALL trigger consequence-appropriate restriction or escalation.
- **OPS-004-REQ-013:** Clock source and clock-quality limitations SHALL be preserved where ordering matters.
- **OPS-004-REQ-014:** Logs SHALL support authorized search and reconstruction without weakening access controls.
- **OPS-004-REQ-015:** Corrections SHALL append clarifying evidence and SHALL NOT rewrite accepted historical records.

## 5. Failure and Degraded Behavior

Logging degradation SHALL be reported as loss of evidence quality.

Falcon SHALL define which activities may continue when ordinary logging is impaired and which SHALL stop when audit-critical logging cannot be preserved.

## 6. Acceptance Evidence

Approval requires evidence for:

- required audit event coverage;
- secret and sensitive-data protection;
- tamper detection;
- retention enforcement;
- loss and backpressure behavior;
- authorized reconstruction of a material action; and
- visibility of logging failure.

## 7. ADR Candidates

- Log transport and storage technology;
- structured record format;
- integrity protection model;
- retention tiers; and
- audit-log isolation.

## 8. Unresolved Matters

- Regulatory retention requirements by operating jurisdiction.
- Audit-critical event catalog.
