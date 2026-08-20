# ADR-I003 — Foundation Persistence Realization

**Identifier:** ADR-I003  
**Version:** 1.0  
**Status:** Accepted  
**Date:** 2026-07-25  
**Decision Owner:** Falcon Project Owner  
**Scope:** FRS-001 authoritative-state and evidence persistence technology, transaction boundary, failure behavior, and recovery obligations  
**Affected Specifications:** SYS-011, OPS-003, OPS-004, SYS-008, AWR-001, AUT-001, FRS-001  
**Applicable Standards:** STD-003, STD-007, STD-009, STD-012, STD-013  
**Related ADRs:** ADR-F002, ADR-F005, ADR-F008, ADR-I001, ADR-I002  
**Supersedes:** None  
**Superseded By:** None  
**Decision Record:** Project Owner approval recorded on 2026-07-25

## 1. Context

FRS-001 requires an implementation technology and transaction model for authoritative state and append-only evidence before source work begins.

The selected technology must support durable transactions, explicit concurrency control, integrity verification, recovery, least privilege, and complete reconstruction without becoming part of Falcon's stable meaning or creating authority of its own.

## 2. Decision Drivers

- one authoritative current state per governed state class;
- atomic preservation of related state and evidence where possible;
- explicit failed, partial, duplicate, and uncertain outcomes;
- prevention of duplicate effects during retry and recovery;
- append-only, integrity-linked evidence;
- controlled concurrency and conflict detection;
- least-privilege separation of persistence responsibilities;
- verified backup and restoration;
- replaceability of the persistence technology; and
- maintainability for long-term Foundation evolution.

## 3. Higher-Authority Constraints

This decision remains subordinate to the Vision, Constitution, SYS-011, ADR-F002, ADR-F005, ADR-F008, approved Contracts, and FRS-001.

Stored data does not create authority. A database success does not establish that an action was authorized, and a database record does not replace its authoritative Falcon owner.

## 4. Alternatives Considered

### Embedded database as the authoritative Foundation store

An embedded database would reduce initial operational setup. It was not selected because the long-term Foundation requires stronger separation of roles, independently managed persistence, and a clearer path for concurrent Falcon processes.

### Event history as the only operational state

This was rejected by ADR-F005. Complete replay SHALL NOT be the only means of recovering current authoritative state.

### Multiple persistence technologies in the initial Foundation

This was rejected because it would create additional failure boundaries, reconciliation paths, operational ownership, and verification burden before a demonstrated need exists.

### PostgreSQL behind a Falcon-owned persistence boundary

This was selected because it supports the required transaction, concurrency, durability, access-control, and recovery model while remaining isolatable and replaceable.

## 5. Decision

### 5.1 Selected Technology

PostgreSQL SHALL be the authoritative persistence technology for FRS-001.

The exact supported PostgreSQL major and maintenance version, distribution, artifact digest, driver version, and approved source SHALL be pinned in the implementation baseline before build or deployment. Maintenance updates require review and evidence; major-version replacement requires an Accepted ADR.

FRS-001 SHALL use one authoritative PostgreSQL persistence boundary. Additional authoritative database products, distributed consensus, multi-primary operation, and automatic cross-site failover are outside scope.

### 5.2 No Persistence Technology Assumption

No Falcon layer, Contract, or Core logic SHALL assume PostgreSQL or any specific persistence technology.

Falcon components SHALL interact only through Falcon-owned persistence Contracts. PostgreSQL types, exceptions, queries, transaction APIs, schemas, and provider-specific semantics SHALL remain inside the Persistence Adapter and its owned implementation boundary.

PostgreSQL is a replaceable implementation decision. It is not part of Falcon's identity, meaning, authority, or stable Contracts.

### 5.3 Persistence Model

FRS-001 SHALL preserve:

1. **Authoritative State:** the latest accepted state owned by the responsible Falcon authority;
2. **Immutable Evidence:** attributable, append-only, integrity-linked history required for reconstruction; and
3. **Recovery Data:** checkpoints, migration identity, reconciliation state, and other governed data required to restore and verify the first two classes.

These classes SHALL be logically separated by schema, ownership, access, and mutation policy while remaining capable of sharing one trustworthy atomic transaction where required.

Derived views, indexes, caches, reports, exports, and temporary buffers are not authoritative sources.

### 5.4 Transaction and Concurrency Model

Where an accepted state change and its evidence share one persistence boundary, both SHALL commit in one database transaction or neither SHALL become accepted.

Every authoritative mutation SHALL include:

- a unique operation identity;
- the expected prior state identity or version;
- the resulting state identity or version;
- the responsible authority and security context;
- the related evidence identity; and
- an explicit persisted outcome.

Optimistic concurrency or a stronger approved control SHALL prevent one accepted prior version from silently producing conflicting authoritative successors.

Operations that may be retried SHALL use idempotency or an equivalent approved deduplication mechanism keyed by stable operation identity.

### 5.5 Evidence Integrity

Accepted evidence SHALL be append-only from the application authority's perspective. Corrections SHALL append linked records and SHALL NOT rewrite the original.

Evidence records SHALL use integrity-linked bounded segments and governed checkpoints as required by ADR-F005. Privilege separation, database constraints, and cryptographic integrity controls SHALL work together; no single control SHALL be described as absolute immutability.

### 5.6 Persistence Failure Policy

Persistence outcomes SHALL be explicit and SHALL include at least `PERSISTED`, `FAILED`, and `UNCERTAIN`.

#### Failed atomic change

If authoritative state and its required evidence do not commit together within the required atomic boundary:

- the transaction SHALL be rolled back;
- the prior authoritative state SHALL remain controlling;
- success SHALL NOT be acknowledged; and
- the failure SHALL be reported to the responsible authorities.

#### Uncertain outcome

If Falcon cannot prove whether a write committed, the outcome SHALL be `UNCERTAIN`.

The operation SHALL NOT be executed again based on assumption or guesswork. Reconciliation SHALL use the original operation identity or another approved verification mechanism that prevents duplicate effects.

Affected authority SHALL remain restricted until reconciliation establishes the actual outcome or an approved recovery decision safely resolves the uncertainty without concealing it.

#### Critical evidence failure

If evidence required for audit, accountability, authorization proof, recovery, or reconstruction cannot be preserved, the dependent action SHALL be denied or the affected scope SHALL be restricted according to consequence.

Broad or authority-critical evidence failure SHALL invoke the applicable Guardian protection and Safe-state policy.

#### Non-critical temporary evidence

Evidence explicitly classified as non-critical MAY use a bounded, encrypted, tamper-evident local spool when its approved policy permits it.

The spool:

- SHALL NOT become an authoritative source;
- SHALL NOT prove success of an authoritative change;
- SHALL have bounded capacity and expiry behavior;
- SHALL preserve original identities and ordering evidence; and
- SHALL reconcile into the authoritative journal without creating duplicate records.

#### Failure that cannot be recorded

If even the persistence failure cannot be recorded durably, the available independent notification path SHALL inform the competent authorities.

Responsibility is separated as follows:

- **Persistence Authority** detects the failure and reports `FAILED` or `UNCERTAIN`;
- **Evidence Authority** assesses loss of provability and evidence integrity;
- **Health Monitoring Authority** assesses operational-health impact;
- **Self-Awareness Authority** updates confidence, known limitations, and Fitness to Operate;
- **Guardian** imposes consequence-appropriate protective restrictions;
- **Recovery Authority** leads reconciliation and restoration; and
- **Independent Verifier** confirms that the hazardous or uncertain condition has ended before unrestricted authority is restored.

No single participant that caused, repaired, or is subject to the failure may independently declare full recovery.

### 5.7 Governing Persistence Principle

> **No persistence, no acknowledged state change. Unknown persistence, no unrestricted authority.**

If Falcon cannot prove what happened, it SHALL NOT behave as though the outcome is known.

### 5.8 Recovery and Restoration

Restoration of PostgreSQL availability SHALL NOT automatically restore Falcon authority.

Recovery SHALL reconcile:

- authoritative state and versions;
- the last valid evidence checkpoint;
- committed, failed, incomplete, and uncertain operations;
- duplicate-effect risk;
- schema and migration identity;
- integrity and security context;
- unresolved Guardian restrictions; and
- backup or restored-data provenance.

Unrestricted authority may return only through the approved release path after independent verification.

A backup SHALL NOT be represented as recoverable until isolated restoration, integrity validation, and reconstruction have succeeded.

### 5.9 Security and Operations

PostgreSQL access SHALL use distinct least-privilege roles appropriate to migration, authoritative-state mutation, evidence append, governed read, backup, and recovery responsibilities.

Falcon runtime identities SHALL NOT receive database-owner, superuser, schema-destruction, or unrestricted evidence-mutation privilege.

Connections, credentials, secrets, stored sensitive data, backups, and recovery material SHALL follow SEC-001 and the approved cryptographic-protection baseline.

Database administration SHALL NOT grant Falcon authority and SHALL NOT permit administrators to fabricate an accepted Falcon decision.

### 5.10 Scope Limitation

This decision does not authorize source implementation, database installation, external network access, production deployment, financial data, trading state, broker connectivity, or live-capital behavior.

## 6. Consequences

- Foundation gains one defined persistence technology and transaction boundary.
- State and evidence can commit atomically where they share that boundary.
- PostgreSQL remains isolated and replaceable.
- Failed and uncertain writes cannot masquerade as success.
- Duplicate-effect prevention becomes mandatory.
- Persistence failure can reduce authority and invoke Guardian protection.
- Availability restoration remains distinct from trustworthy recovery.
- Operating PostgreSQL adds patching, backup, monitoring, and recovery responsibilities.

## 7. Risks and Mitigations

- **Technology leakage:** enforce Falcon-owned persistence Contracts, Adapter isolation, and boundary tests.
- **State and evidence divergence:** use one atomic transaction where possible and explicit reconciliation otherwise.
- **Duplicate effects after an uncertain write:** preserve operation identity and require idempotent or verified reconciliation.
- **Database privilege abuse:** separate roles, deny runtime ownership privileges, and retain attributable administration evidence.
- **False immutability claim:** combine append-only permissions, constraints, linked integrity, checkpoints, and independent verification.
- **Local spool becoming a shadow authority:** restrict it to approved non-critical evidence and prohibit authoritative-success claims.
- **Availability mistaken for recovery:** require reconciliation and independent verification before authority restoration.
- **Vendor lock-in:** contain PostgreSQL behind the Adapter and preserve migration and exit evidence.

## 8. Compatibility and Transition

This decision realizes, but does not redefine, ADR-F005.

Any future replacement of PostgreSQL SHALL preserve Falcon-owned Contracts, state ownership, evidence integrity, operation identities, uncertainty status, restrictions, and recovery proof. Replacement requires a superseding ADR, migration plan, rollback plan, and independently verified reconstruction.

The governing persistence principle may be referenced by Persistence, Guardian, Recovery, Health Monitoring, and Self-Awareness documents. Changes to already Approved artifacts require a separately governed amendment and SHALL NOT be implied by this ADR.

## 9. Conformance Evidence

Conformance requires:

- a pinned PostgreSQL implementation identity and approved source;
- proof that PostgreSQL details do not cross the Persistence Adapter;
- project dependency and boundary-analysis results;
- atomic state-and-evidence commit and rollback tests;
- concurrency-conflict tests;
- idempotency and duplicate-effect prevention tests;
- forced `FAILED` and `UNCERTAIN` outcome tests;
- evidence-loss restriction and Guardian-response tests;
- local-spool bounds, encryption, reconciliation, and non-authority tests if the spool is enabled;
- role and privilege verification;
- backup, isolated restore, integrity, and reconstruction evidence;
- independent recovery-release evidence; and
- proof that no financial data or behavior has entered FRS-001.

## 10. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner and current Constitutional Authority | Accepted | رائد عموره — “موافق على PostgreSQL وسياسة التخزين بصيغتها النهائية ضمن ADR-I003.” | 2026-07-25 |
