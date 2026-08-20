# FDN-002 — Foundation FIL Interaction and Schema Catalog

**Identifier:** FDN-002  
**Version:** 1.1  
**Status:** Approved  
**Effective Date:** 2026-07-24  
**Approval Record:** GOV-010  
**Owner:** Falcon Communication Authority  
**Governing Authority:** FRS-001; ADR-F003; ADR-F004; SYS-005; SYS-009; SYS-010  
**Canonical Schema:** `schemas/FIL-001_FOUNDATION_MESSAGE.schema.json`  
**Supersedes:** None  
**Superseded By:** None

## 1. Purpose

This catalog declares every permitted FRS-001 cross-boundary interaction and the released schema, compatibility, valid examples, and rejection cases used before implementation.

An interaction absent from this catalog is not authorized by the Foundation baseline.

## 2. Interaction Catalog

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

## 3. Direct Interaction Rule

## 3A. Minimum Cryptographic Protection

| Interaction | Minimum profile |
|---|---|
| inside one protected execution boundary | FIL-P1 |
| crossing a process or equivalent security boundary | FIL-P2 |
| confidential authority, Guardian, recovery, security, or evidence payload crossing an intermediary | FIL-P3 |
| retained confidential or restricted payload or dead-letter evidence | FIL-P4 |

Every interaction SHALL declare classification, recipient binding, temporal/replay policy, and key purpose. Plaintext fallback is prohibited.

Direct calls are permitted only inside one declared component and isolation boundary. Cross-boundary access to state, files, databases, process controls, or private interfaces is prohibited unless declared above or approved through a later catalog version and authority review.

## 4. Schema Profile

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

## 5. Valid Examples

### Command

```json
{"message_id":"01J00000000000000000000001","kind":"Command","type":"foundation.lifecycle.command.v1","schema_id":"urn:falcon:fil:schema:foundation-message:1.1","schema_version":"1.1","producer_id":"kernel:test","created_at":"2026-07-24T00:00:00Z","purpose":"trusted-bootstrap","classification":"INTERNAL","target":"lifecycle:test","authority_context":"authctx:test","expires_at":"2026-07-24T00:01:00Z","priority_authority":"foundation.normal","protection_profile":"FIL-P2","protection_version":"1.0","integrity_scope":["envelope","payload"],"key_ref":"key:test","replay_policy":"kind-default","integrity":{"profile":"test","value":"verified-test-value"},"payload":{"component_id":"component:test","expected_state":"READY","target_state":"RUNNING","reason":"approved verification"}}
```

### Query

```json
{"message_id":"01J00000000000000000000002","kind":"Query","type":"foundation.authority.query.v1","schema_id":"urn:falcon:fil:schema:foundation-message:1.1","schema_version":"1.1","producer_id":"component:test","created_at":"2026-07-24T00:00:00Z","purpose":"authorization-check","classification":"INTERNAL","target":"authority:test","expires_at":"2026-07-24T00:01:00Z","priority_authority":"foundation.normal","protection_profile":"FIL-P2","protection_version":"1.0","integrity_scope":["envelope","payload"],"key_ref":"key:test","replay_policy":"kind-default","integrity":{"profile":"test","value":"verified-test-value"},"payload":{"action":"foundation.lifecycle.transition","resource":"component:test"}}
```

### Response

```json
{"message_id":"01J00000000000000000000003","kind":"Response","type":"foundation.authority.response.v1","schema_id":"urn:falcon:fil:schema:foundation-message:1.1","schema_version":"1.1","producer_id":"authority:test","created_at":"2026-07-24T00:00:01Z","purpose":"authorization-result","classification":"INTERNAL","correlation_id":"01J00000000000000000000002","priority_authority":"foundation.normal","protection_profile":"FIL-P2","protection_version":"1.0","integrity_scope":["envelope","payload"],"key_ref":"key:test","replay_policy":"kind-default","integrity":{"profile":"test","value":"verified-test-value"},"payload":{"decision":"DENY","reason":"authority-not-granted"}}
```

### Event

```json
{"message_id":"01J00000000000000000000004","kind":"Event","type":"foundation.lifecycle.changed.v1","schema_id":"urn:falcon:fil:schema:foundation-message:1.1","schema_version":"1.1","producer_id":"lifecycle:test","created_at":"2026-07-24T00:00:02Z","purpose":"state-fact","classification":"INTERNAL","fact_owner":"lifecycle:test","causation_id":"01J00000000000000000000001","priority_authority":"foundation.normal","protection_profile":"FIL-P2","protection_version":"1.0","integrity_scope":["envelope","payload"],"key_ref":"key:test","replay_policy":"kind-default","integrity":{"profile":"test","value":"verified-test-value"},"payload":{"component_id":"component:test","prior_state":"READY","state":"RUNNING","transition_id":"transition:test"}}
```

### Notice

```json
{"message_id":"01J00000000000000000000005","kind":"Notice","type":"foundation.health.observation.v1","schema_id":"urn:falcon:fil:schema:foundation-message:1.1","schema_version":"1.1","producer_id":"probe:test","created_at":"2026-07-24T00:00:03Z","purpose":"health-observation","classification":"INTERNAL","expires_at":"2026-07-24T00:01:03Z","priority_authority":"foundation.normal","protection_profile":"FIL-P2","protection_version":"1.0","integrity_scope":["envelope","payload"],"key_ref":"key:test","replay_policy":"kind-default","integrity":{"profile":"test","value":"verified-test-value"},"payload":{"subject":"component:test","signal":"heartbeat","observed":"available"}}
```

## 6. Mandatory Rejection Cases

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

## 7. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner | Approved | GOV-008 | 2026-07-24 |

