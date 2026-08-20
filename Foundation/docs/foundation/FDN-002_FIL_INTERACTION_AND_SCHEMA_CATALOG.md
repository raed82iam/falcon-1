# FDN-002 — FIL Interaction and Schema Catalog

**Version:** 1.2  
**Status:** Approved  
**Approval Record:** GOV-063  
**Documentary Activation:** Active  
**Activation Record:** GOV-092; GOV-093; GOV-094  
**Effective Documentary Instant:** 2026-07-31 22:54:57 +03:00  
**Supersedes:** FDN-002 v1.1  
**Identifier:** FDN-002  
**Canonical Target:** `docs/foundation/FDN-002_FIL_INTERACTION_AND_SCHEMA_CATALOG.md`  
**Owner:** Falcon Communication Authority  
**Governing Authority:** GOV-063; AWR-001 v2.1; CON-004; CON-005; CON-008; CON-021; SYS-005; SYS-009; SYS-010; SEC-002  
**Implementation Authority:** Not Granted  
**Stage 1 Authority:** Not Granted  

## 1. Purpose

FDN-002 defines the governed interaction classes, envelope metadata, schema identity, validation rules, rejection behavior, retry behavior, deduplication behavior, ordering rules, correlation rules, expiry rules, and dead-letter behavior for FIL.

It protects transport meaning without claiming ownership of Application business content.

## 2. Scope

FDN-002 governs:

- FIL interaction classes;
- envelope metadata;
- schema identity;
- versioning;
- compatibility;
- validation;
- rejection;
- retry;
- deduplication;
- ordering;
- correlation;
- expiry;
- dead-letter handling; and
- reconstruction of message lineage.

## 3. Non-Scope

FDN-002 does not:

- define Application business meaning;
- define trading or financial meaning;
- decide the content of a trade, order, strategy, or business action;
- grant execution authority;
- grant activation authority;
- grant implementation authority;
- grant Stage 1 authority; or
- turn transport metadata into content ownership.

## 4. Owners and Authority Boundaries

- Communication Authority owns FIL interaction classes and schema rules.
- Service Bus owns transport movement only.
- AWR-001 owns technical fitness and Foundation conformance implications.
- Authority Engine owns acceptance and authority decisions.
- Guardian may impose protective restriction when risk demands it.

The message payload may carry Application meaning, but FIL SHALL NOT own that meaning.

## 5. Interaction Classes

This catalog recognizes the following interaction classes:

- `REQUEST`
- `DECISION`
- `EVENT`
- `EVIDENCE`
- `HEALTH_OBSERVATION`
- `FITNESS_RESULT`
- `RESTRICTION`
- `RECOVERY_COMMAND`
- `RECONSTRUCTION_REQUEST`

## 6. Envelope Metadata

Every FIL envelope SHALL declare:

- message identity;
- interaction class;
- producer identity;
- recipient identity;
- contract identity;
- schema identity;
- schema version;
- authority context;
- correlation identity;
- causation identity where applicable;
- creation time;
- expiry time where applicable;
- protection class;
- replay class;
- deduplication key where applicable; and
- evidence relation where applicable.

Envelope metadata SHALL remain separate from Application payload meaning.

## 7. Schema and Compatibility Rules

- Every schema SHALL be versioned and attributable.
- Every schema SHALL have one declared owner.
- Compatibility SHALL be explicit: `exact`, `backward`, `forward`, or `incompatible`.
- Message validation SHALL reject undeclared schema versions unless a governing compatibility rule explicitly permits them.
- Schema changes SHALL not silently alter payload meaning.
- Envelope compatibility and payload meaning SHALL be assessed separately.

## 8. Normative Requirements

- **FDN-002-REQ-001:** Every FIL interaction class SHALL have a governed identity.
- **FDN-002-REQ-002:** Every message SHALL declare producer, recipient, contract, schema, version, and authority context.
- **FDN-002-REQ-003:** Envelope metadata SHALL be separable from payload meaning.
- **FDN-002-REQ-004:** Application payload meaning SHALL remain owned by the Application contract, not by FIL or Service Bus.
- **FDN-002-REQ-005:** Message validation SHALL check identity, schema, version, authority context, expiry, and compatibility before acceptance.
- **FDN-002-REQ-006:** Incompatible messages SHALL be rejected.
- **FDN-002-REQ-007:** Expired messages SHALL be rejected or quarantined according to policy and SHALL not be treated as current truth.
- **FDN-002-REQ-008:** Duplicate messages SHALL be deduplicated or rejected according to contract.
- **FDN-002-REQ-009:** Ordering SHALL not be assumed unless explicitly guaranteed by the governing contract.
- **FDN-002-REQ-010:** Correlation and causation identities SHALL be preserved for reconstructability.
- **FDN-002-REQ-011:** Dead-letter behavior SHALL preserve failure evidence and reason.
- **FDN-002-REQ-012:** Retry SHALL occur only within declared policy and SHALL not create duplicate effects.
- **FDN-002-REQ-013:** Retry exhaustion SHALL transition the message to governed failure handling.
- **FDN-002-REQ-014:** Sensitive envelope or payload material SHALL follow governed protection rules.
- **FDN-002-REQ-015:** FIL SHALL NOT infer business truth from message transport success alone.
- **FDN-002-REQ-016:** Service Bus SHALL NOT become a source of business authority by carrying a message.
- **FDN-002-REQ-017:** Message lineage SHALL remain reconstructable.

## 9. Failure and Recovery Behavior

When a message is missing, duplicated, delayed, expired, corrupted, incompatible, or rejected:

- the failure SHALL be visible;
- the reason SHALL be recorded;
- confidence in affected evidence MAY decrease;
- retry SHALL obey declared policy;
- dead-letter handling SHALL preserve evidence;
- isolation MAY be required if the failure is systemic; and
- no unsupported business inference SHALL be made from transport failure.

Recovery MAY re-deliver or reconstruct the message only if the contract allows it and only within governed authority.

## 10. Invariants

1. Transport success is not business truth.
2. Envelope metadata is not payload meaning.
3. Schema version is not a license to reinterpret meaning.
4. Order is explicit, never assumed.
5. Retry without deduplication is unsafe.
6. Dead-letter is evidence, not erasure.

## 11. Validation and Acceptance Evidence

Acceptance requires examples showing:

- exact schema and interaction-class identification;
- successful and rejected compatibility checks;
- explicit ordering behavior;
- duplicate handling;
- expiry handling;
- dead-letter preservation;
- correlation and reconstruction;
- separation between envelope metadata and payload meaning; and
- no transport-derived business authority.

## 12. Preservation Annex: Active Edition Content Carried Forward

The active edition's concrete interaction catalog, schema profile, valid examples, and mandatory rejections are preserved below so this successor remains self-contained.

### 12.1 Interaction Catalog

| Type ID | Kind | Producer | Consumer or topic | Purpose | Authority owner | Delivery expectation |
|---|---|---|---|---|---|---|
| `foundation.lifecycle.command.v1` | Command | Kernel, Guardian, Recovery | Lifecycle | Request governed transition | Lifecycle | At-least-once attempt; idempotent logical request |
| `foundation.lifecycle.response.v1` | Response | Lifecycle | requester | Report rejection or accepted transition result | Lifecycle | Correlated response |
| `foundation.lifecycle.changed.v1` | Event | Lifecycle | `foundation.lifecycle` | Assert accepted state change | Lifecycle | Durable for reconstruction |
| `foundation.authority.query.v1` | Query | admitted Core actor | Authority Engine | Request permission decision | Authority Engine | Request/response |
| `foundation.authority.response.v1` | Response | Authority Engine | requester | Return permit or deny with basis | Authority Engine | Correlated; no implied execution |
| `foundation.authority.decided.v1` | Event | Authority Engine | `foundation.authority` | Assert material authority result | Authority Engine | Durable for reconstruction |
| `foundation.health.observation.v1` | Notice | admitted evidence source | Health Monitoring | Supply non-authoritative observation | evidence source | Expiring |
| `foundation.health.assessed.v1` | Event | Health Monitoring | `foundation.health` | Assert health assessment | Health Monitoring | Durable when material |
| `foundation.fitness.query.v1` | Query | Authority Engine, Guardian | Self-Awareness | Request scoped Fitness | Self-Awareness | Request/response |
| `foundation.fitness.response.v1` | Response | Self-Awareness | requester | Return Fitness and uncertainty | Self-Awareness | Expiring |
| `foundation.guardian.restriction.v1` | Command | Guardian | Authority Engine, Lifecycle, enforcement points | Impose CON-011 restriction | Guardian | Protective priority; durable acceptance |
| `foundation.guardian.restricted.v1` | Event | Guardian | `foundation.protection` | Assert issued restriction | Guardian | Durable |
| `foundation.recovery.command.v1` | Command | authorized recovery initiator | Recovery | Begin or advance approved plan | Recovery | Idempotent plan step |
| `foundation.recovery.response.v1` | Response | Recovery | requester | Report step result without self-release | Recovery | Correlated |
| `foundation.recovery.validated.v1` | Event | Independent Verifier | `foundation.recovery` | Assert validation result | Verification Authority | Durable |
| `foundation.configuration.query.v1` | Query | admitted Core actor | Configuration | Request effective snapshot or permitted item | Configuration | Read-only |
| `foundation.configuration.changed.v1` | Event | Configuration | `foundation.configuration` | Assert activated snapshot | Configuration | Durable |
| `foundation.evidence.notice.v1` | Notice | Evidence Authority | Health Monitoring, Guardian | Report evidence degradation | Evidence Authority | Protective, expiring |

### 12.2 Schema Profile

- representation: UTF-8 JSON;
- schema mechanism: JSON Schema Draft 2020-12;
- canonicalization for integrity: RFC 8785;
- envelope Contract: CON-004;
- schema ID: `urn:falcon:fil:schema:foundation-message:1.1`;
- compatibility: released version 1 is immutable;
- unknown fields: rejected unless the owning schema explicitly permits them;
- duplicate JSON member names: rejected before schema validation;
- exact decimal values: strings constrained by the owning payload schema;
- times: normalized UTC strings.

### 12.3 Valid Examples

The active edition command, query, response, event, and notice examples remain preserved as governing reference examples for the successor. Their envelope metadata rules, rejection cases, and approval record remain unchanged in effect.

### 12.4 Mandatory Rejection Cases

| Case | Required result |
|---|---|
| malformed UTF-8 or JSON | reject at representation stage |
| duplicate member name | reject before schema validation |
| missing message ID, kind, type, schema, producer, time, purpose, classification, integrity, or payload | reject at envelope stage |
| unsupported schema ID or version | explicit unsupported rejection |
| expired message | reject before governed action |
| Command without target or authority context | reject |
| Response without correlation ID | reject |
| Event without fact owner | reject |
| invalid integrity evidence | reject before authorization |
| authenticated but unauthorized producer, route, or fact assertion | deny after structural validation |
| replay where prohibited | reject or quarantine with evidence |
