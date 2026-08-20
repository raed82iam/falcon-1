# ADR-F005 — Persistence and Evidence Integrity Model

**Identifier:** ADR-F005  
**Version:** 1.0  
**Status:** Accepted  
**Date:** 2026-07-24  
**Decision Owner:** Falcon Project Owner  
**Scope:** Foundation state persistence, evidence preservation, and integrity verification  
**Affected Specifications:** SYS-011, OPS-004, DEC-006, CON-008, FRS-001  
**Applicable Standards:** STD-003, STD-013  
**Related ADRs:** ADR-F002, ADR-F004, ADR-F006  
**Supersedes:** None  
**Superseded By:** None  
**Decision Record:** Project Owner approval recorded on 2026-07-24

## 1. Context

Falcon must preserve both its current authoritative state and the evidence that explains how that state was reached. Treating logs as current state would make operation ambiguous; storing only current state would make decisions, failures, corrections, and recovery impossible to reconstruct.

FRS-001 must establish trustworthy persistence and detectable evidence integrity without claiming financial recordkeeping, distributed consensus, or immutable storage beyond what can be demonstrated.

## 2. Decision Drivers

- preserve one authoritative current state per ADR-F002;
- reconstruct material actions and transitions completely;
- detect alteration, deletion, duplication, gaps, and uncertain writes;
- distinguish operational state from historical evidence;
- support controlled recovery without fabricated certainty;
- append corrections without rewriting accepted history;
- protect sensitive information and restrict evidence access;
- prove restoration before calling a backup recoverable; and
- avoid unnecessary distributed-ledger complexity.

## 3. Higher-Authority Constraints

This decision is constrained by:

- the Vision requirements for capital protection, disciplined operation, and long-term maintainability;
- constitutional requirements for truthful state, evidence, accountability, security, and recoverability;
- SYS-011 requirements for explicit ownership, integrity, durability, consistency, restoration, and provenance;
- OPS-004 requirements for attributable, protected, append-only corrections and visible evidence loss;
- DEC-006 requirements for immutable decision history and separation of decision stages;
- CON-008 requirements for structured Foundation evidence and distinct accepted, persisted, and retained states; and
- FRS-001 requirements for complete reconstruction and controlled recovery.

## 4. Alternatives Considered

### 4.1 Current-state storage only

Falcon could preserve only the latest authoritative values.

This was rejected because history, causation, rejected actions, partial failures, and corrections could not be reconstructed reliably.

### 4.2 Event history as the only operational state

Falcon could reconstruct every current value exclusively from a complete event history.

This was not selected for FRS-001 because it would couple operational recovery to total replay completeness and make every state owner dependent on one persistence pattern.

### 4.3 Separated current state and append-only evidence

Falcon preserves authoritative current state in owner-controlled state records and preserves material history in a protected append-only evidence journal.

This alternative was selected because it keeps operational truth and historical proof distinct while linking them for reconstruction and recovery.

### 4.4 Distributed ledger or blockchain

Falcon could use a distributed consensus ledger for evidence integrity.

This was rejected for the Foundation because Falcon controls the initial trust domain, distributed operation is out of scope, and equivalent Foundation integrity can be achieved with materially less complexity.

## 5. Decision

FRS-001 SHALL use a dual persistence model:

1. **Authoritative state records** preserve the latest accepted state owned by the responsible authority.
2. **An append-only evidence journal** preserves attributable material requests, decisions, transitions, persistence results, failures, corrections, and recovery actions.

State records SHALL identify owner, subject, version, effective time, integrity status, and the evidence reference for the accepted change. Concurrent changes SHALL use explicit conflict detection so that one accepted prior version cannot silently produce two authoritative successors.

Accepted evidence records SHALL NOT be edited or silently deleted. A correction SHALL append a new linked record that preserves the original. Evidence shall distinguish attempted, accepted, persisted, externally retained, failed, partial, and uncertain outcomes.

Evidence integrity SHALL use cryptographic record digests linked within bounded journal segments. Governed checkpoints SHALL anchor completed segments so that modification, deletion, insertion, reordering, or gaps are detectable. Cryptographic protection demonstrates integrity and provenance within the declared trust model; it does not make every recorded statement factually true.

Where state change and its evidence can share one trustworthy atomic boundary, they SHALL commit together. Where they cannot, Falcon SHALL preserve a durable change identity and reconciliation status; an uncertain outcome SHALL remain explicit and SHALL NOT be reported as success.

Snapshots, indexes, search views, caches, and reports are derived aids. They SHALL NOT replace the authoritative state owner or the append-only evidence source.

Backup and restoration SHALL cover state, evidence, schemas, ownership metadata, versions, integrity anchors, and required security context. A backup SHALL NOT be described as recoverable until restoration and integrity verification have succeeded in an isolated test.

After failure, Falcon SHALL reconcile authoritative state, the last valid evidence checkpoint, incomplete changes, and integrity status before restoring unrestricted authority. Unknown integrity SHALL cause consequence-appropriate restriction.

This decision does not select a database product, storage vendor, cryptographic algorithm, retention duration, or distributed replication mechanism.

## 6. Consequences

- Falcon can answer both “what is true now?” and “how did it become true?”
- Current-state reads do not require replaying all historical evidence.
- Accepted history cannot be corrected by concealment or rewriting.
- Tampering and missing evidence become detectable.
- Partial and uncertain persistence cannot masquerade as success.
- Recovery requires reconciliation rather than blind restart.
- State and evidence schemas, retention, backup, and access require explicit governance.
- Additional storage and integrity-verification cost is accepted in exchange for accountability and recoverability.

## 7. Risks and Mitigations

- **Risk:** State and evidence could diverge during partial failure.  
  **Mitigation:** Use a shared atomic boundary where possible; otherwise preserve durable change identity, explicit uncertainty, and reconciliation.

- **Risk:** A hash-linked journal could be mistaken for proof that every statement is true.  
  **Mitigation:** Describe it only as integrity and provenance evidence; retain source trust and authority assessments.

- **Risk:** Sensitive data could be preserved unnecessarily in permanent evidence.  
  **Mitigation:** Apply classification, minimization, redaction, access control, and governed retention before acceptance.

- **Risk:** Evidence growth could impair operation.  
  **Mitigation:** Use governed segmentation, checkpoints, retention classes, and derived indexes without rewriting protected history.

- **Risk:** Backups could exist but be unusable.  
  **Mitigation:** Require isolated restoration tests and integrity verification before claiming recoverability.

## 8. Compatibility and Transition

No prior Falcon1 ADR is superseded.

Every FRS-001 authoritative state class shall declare its state owner, persistence obligations, evidence linkage, concurrency rule, and recovery behavior before implementation authorization. Every audit-critical action shall map to CON-008 evidence.

Financial books, regulatory records, distributed persistence, and long-term retention policy remain outside FRS-001 and require later governed decisions.

## 9. Conformance Evidence

Conformance shall be demonstrated by evidence that:

- current state and historical evidence are distinguishable and linked;
- concurrent changes cannot silently create two authoritative successors;
- modification, deletion, insertion, reordering, and gaps in protected evidence are detectable;
- corrections append without replacing original records;
- failed, partial, duplicate, and uncertain writes remain explicit;
- audit-critical evidence loss causes the required restriction;
- an authorized reviewer can reconstruct every FRS-001 scenario;
- a backup can be restored and independently integrity-verified; and
- recovery does not restore unrestricted authority while state or evidence integrity is unknown.

## 10. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner and current Constitutional Authority | Accepted | رائد عموره — “موافق على القرار الخامس” | 2026-07-24 |
