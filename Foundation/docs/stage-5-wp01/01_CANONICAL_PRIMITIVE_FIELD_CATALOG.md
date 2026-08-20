# Stage 5 WP-01 Canonical Primitive Field Catalog

WP-01 defines application-neutral canonical messaging primitives only.

| Field | Type | Purpose |
|---|---|---|
| MessageId | MessageIdentity | Unique canonical message identity |
| MessageKind | FilMessageKind | Command, Query, Response, Event, or Notice |
| Classification | FilMessageClassification | Operational, Governance, Evidence, Health, Security, or Administrative |
| MessageType | string | Dotted canonical semantic type name |
| SchemaId | SchemaIdentity | Stable schema identity reference |
| SchemaVersion | string | Numeric dotted schema version reference |
| Producer | ProducerIdentityReference | Canonical producer reference |
| RecipientScope | RecipientScopeReference | Intended recipient or scope |
| CorrelationId | CorrelationIdentity | Conversation/workflow correlation identity |
| CausationId | CausationIdentity? | Distinct causal predecessor identity |
| Authority | AuthorityReference | Bound authority reference |
| Provenance | ProvenanceReference | Bound provenance/evidence reference |
| IdempotencyId | IdempotencyIdentity | Deterministic duplicate-control identity |
| DeliveryAttemptId | DeliveryAttemptIdentity | Identity of this delivery attempt |
| RetryLineageId | RetryLineageIdentity | Stable retry-family identity |
| Time | CanonicalMessageTime | UTC creation and optional expiry metadata |
| Outcome | CanonicalOutcome | Typed UNKNOWN/SUCCEEDED/FAILED/REJECTED outcome |
| Payload | string | Opaque application payload |
| PayloadSha256 | string | Uppercase hexadecimal SHA-256 payload binding |

Foundation does not interpret application payload business semantics.
