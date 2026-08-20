# ADR-I004 — Foundation Communication Realization

**Identifier:** ADR-I004  
**Version:** 1.0  
**Status:** Accepted  
**Date:** 2026-07-25  
**Decision Owner:** Falcon Project Owner  
**Scope:** FRS-001 in-process and isolated-process communication realization, delivery truth, ordering, congestion, and transport failure  
**Affected Specifications:** SYS-005, SYS-009, SYS-010, SEC-001, AUT-001, FRS-001  
**Applicable Standards:** STD-003, STD-007, STD-009, STD-012, STD-013  
**Related ADRs:** ADR-F001, ADR-F003, ADR-F004, ADR-F006, ADR-F008, ADR-I001, ADR-I002, ADR-I003  
**Supersedes:** None  
**Superseded By:** None  
**Decision Record:** Project Owner approval recorded on 2026-07-25

## 1. Context

FRS-001 requires concrete communication mechanisms for components inside one trusted process and for components separated by an isolation boundary.

The realization must preserve FIL semantics, authority separation, Guardian enforcement, truthful delivery state, bounded resource use, transport replaceability, and cryptographic protection without introducing distributed messaging infrastructure into the initial Foundation.

## 2. Decision Drivers

- governed FIL communication across component and isolation boundaries;
- no undeclared side channels;
- bounded memory and backpressure;
- local process isolation without public network exposure;
- mutual endpoint identity and required cryptographic protection;
- separation of transport trust from message trust;
- truthful, granular delivery outcomes;
- bounded retry and duplicate-effect prevention;
- correctness under valid message reordering;
- protected communication under congestion and degradation; and
- replacement of transport technology without changing Falcon meaning.

## 3. Higher-Authority Constraints

This decision remains subordinate to the Vision, Constitution, SYS-005, SYS-009, SYS-010, SEC-001, ADR-F003, approved Contracts, the cryptographic-protection baseline, and FRS-001.

Communication transports requests and facts. It does not create authority, own communicated state, establish business success, or override Guardian.

## 4. Alternatives Considered

### Unbounded in-memory queues

This was rejected because an overloaded producer or consumer could exhaust shared memory and impair unrelated Foundation behavior.

### External message broker

Kafka, RabbitMQ, and equivalent broker infrastructure were not selected for FRS-001 because the release is local and non-distributed, and a broker would add dependency, operation, trust, persistence, and failure boundaries not required for the Foundation proof.

### gRPC as the universal Falcon interaction model

This was not selected because FIL already defines Falcon interaction semantics. Making an RPC framework universal would introduce an external contract and risk blending transport receipt with Falcon acceptance and execution.

### Bounded in-process channels and protected local IPC

This was selected because it supports bounded local communication and process isolation using the approved .NET runtime while keeping all transport mechanisms behind Falcon-owned boundaries.

## 5. Decision

### 5.1 Governed Topology

Every material interaction crossing a governed component or isolation boundary SHALL use FIL through the governed Service Bus.

Undeclared communication paths that bypass message identity, validation, authorization, classification, Guardian restriction, routing policy, or required evidence are prohibited.

Direct calls MAY occur only within one declared component and one isolation boundary when they do not cross an authority boundary or evade required evidence.

### 5.2 In-Process Transport

In-process Service Bus routes SHALL use bounded .NET channels behind a Falcon-owned transport abstraction.

Every channel SHALL declare:

- capacity;
- admitted message classes;
- writer and reader ownership;
- full-capacity behavior;
- timeout and cancellation behavior;
- ordering guarantee, if any;
- failure and closure behavior; and
- required evidence.

Unbounded channels are prohibited for governed communication.

Capacity exhaustion SHALL apply explicit backpressure, rejection, or another approved class-specific policy. A message SHALL NOT be silently discarded. A producer SHALL NOT interpret successful enqueueing as delivery, acceptance, execution, persistence, or successful outcome.

### 5.3 Isolated-Process Transport

Local isolated processes SHALL communicate through:

- **Windows:** Named Pipes with explicit operating-system access control; and
- **Linux:** Unix Domain Sockets with explicit owner and file-mode access control.

Transport-specific details SHALL remain within Falcon Transport Adapters.

Endpoints SHALL be local-only for FRS-001. Public interfaces, remote hosts, externally routable listeners, and general network federation are prohibited.

Endpoint creation SHALL fail closed if required operating-system ownership or access restrictions cannot be established and verified.

### 5.4 Cross-Boundary Protection

Operating-system IPC permissions are necessary but insufficient.

Every isolated-process route SHALL enforce:

- operating-system endpoint restriction;
- mutually authenticated endpoint identity;
- encrypted transport under the approved protection profile;
- cryptographic binding required by CON-004;
- independent FIL producer-identity and message-integrity verification;
- expiry, replay, recipient, and downgrade enforcement; and
- explicit rejection on failed or unknown protection.

The protected stream mechanism SHALL remain inside the Transport Adapter. Exact algorithms, certificates, key custody, rotation, and trust anchors are governed by the cryptographic and identity decisions.

Plaintext fallback and permissive retry after protection failure are prohibited.

### 5.5 Channel Trust Does Not Establish Message Trust

> **A trusted channel does not make every message trusted.**

Successful channel authentication establishes only the verified properties of that channel. Every FIL message SHALL pass distinct checks for:

1. channel protection;
2. FIL structure and schema;
3. original producer identity and message integrity;
4. time, expiry, recipient, replay, and classification;
5. producer and route authorization;
6. applicable Guardian restrictions;
7. owning-component acceptance; and
8. governed execution and persistence.

A failure at any stage SHALL NOT be represented as success at a later stage.

### 5.6 Message Framing and Limits

Cross-process FIL messages SHALL use an explicit versioned frame around the canonical FIL representation selected by ADR-F004.

The frame SHALL declare sufficient information to detect:

- unsupported frame versions;
- invalid or excessive length;
- truncation;
- frame substitution or corruption;
- incomplete reads;
- prohibited multiplexing; and
- mismatch with the protected FIL envelope.

Maximum frame, envelope, payload, and batch sizes SHALL be configured by approved message class and enforced before material allocation or deserialization.

The frame is a transport mechanism. It SHALL NOT redefine FIL meaning or become an authority-bearing Contract.

### 5.7 Delivery-State Truth

Falcon SHALL distinguish at minimum:

- `ADMITTED`: the Service Bus accepted the message for governed handling;
- `DISPATCHED`: a delivery attempt was initiated;
- `RECEIVED`: the destination transport acknowledged receipt;
- `ACCEPTED`: the owning component accepted the request or fact for governed handling;
- `EXECUTED`: the governed action completed;
- `PERSISTED`: required state and evidence were durably committed;
- `FAILED`: the applicable stage is proven to have failed; and
- `UNCERTAIN`: the applicable outcome cannot be proven.

An earlier state SHALL NOT imply a later state. In particular, receipt does not imply acceptance, acceptance does not imply execution, and execution does not imply persistence.

Every material transition between these states SHALL be attributable to its responsible owner.

### 5.8 Retry, Identity, and Duplicate Effects

Retry SHALL be bounded by count, time, expiry, consequence, destination health, and approved message-class policy.

The logical message or operation identity SHALL remain stable across retries. Every delivery attempt SHALL have a distinct attempt identity.

Non-idempotent commands SHALL NOT be retried automatically unless an approved deduplication or outcome-verification mechanism prevents duplicate effects.

When outcome is `UNCERTAIN`, ADR-I003 reconciliation policy applies. A new message identity SHALL NOT be used to conceal or bypass an unresolved original operation.

### 5.9 Communication Ordering Independence

> **Message arrival order shall not be assumed unless explicitly guaranteed by the governing Contract. Components must remain correct under valid reordering whenever ordering is not part of the Contract.**

The default is no arrival-order guarantee.

Send, arrival, admission, acceptance, execution, and persistence order SHALL remain distinct. Correlation and causation SHALL NOT imply delivery order.

Current authoritative state SHALL NOT be inferred from arrival order alone. State identity, version, causation, authority, and reconciliation status SHALL govern.

Where ordering is required, the governing Contract SHALL declare:

- ordering scope;
- ordering key or partition;
- sequence identity;
- start, reset, and rollover rules;
- duplicate and gap detection;
- missing-message wait bound;
- late and stale-message behavior;
- retry and replay behavior; and
- recovery and reconciliation policy.

FRS-001 SHALL NOT claim one global order across all Falcon messages.

A sequence number SHALL NOT create authority, revive an expired message, validate an untrusted producer, or override Guardian.

### 5.10 Congestion and Protective Communication

The Service Bus SHALL reserve bounded capacity and governed scheduling for authorized protective communication, including:

- Guardian restrictions;
- authority revocation;
- Safe-state control;
- critical containment; and
- recovery control required to preserve safety.

Protective classification and priority SHALL require explicit authority. A producer cannot promote its own message merely by labeling it urgent.

The protective path SHALL NOT bypass identity, integrity, authorization, expiry, replay, or Guardian policy.

Scheduling SHALL prevent ordinary traffic from starving protective communication and prevent protective traffic from becoming an unbounded resource-exhaustion path.

### 5.11 Communication Failure Policy

The Service Bus SHALL report degradation and failure truthfully.

When required communication cannot be trusted:

- success SHALL NOT be claimed;
- dependent action SHALL be denied, delayed, or restricted;
- unrelated scopes MAY continue only when trustworthy isolation and independent fitness are established;
- Health Monitoring SHALL assess the communication impairment;
- Self-Awareness SHALL update known limitation and Fitness to Operate;
- Guardian SHALL impose consequence-appropriate restrictions; and
- unresolved messages and outcomes SHALL enter governed reconciliation.

Restoration of a channel SHALL NOT automatically restore authority. Pending, duplicate, expired, rejected, and uncertain messages SHALL be reconciled before the applicable restriction is released.

### 5.12 No Communication Technology Assumption

No Falcon component, Contract, Core policy, or FIL payload SHALL assume:

- .NET channels;
- Named Pipes;
- Unix Domain Sockets;
- the physical frame format;
- a shared process;
- a shared machine; or
- a particular future broker or network protocol.

Components SHALL depend only on Falcon-owned communication Contracts. Transport replacement SHALL NOT change FIL meaning, authority, stable component behavior, or Guardian enforcement.

### 5.13 Scope Limitation

This decision does not authorize source implementation, package installation, remote networking, external message brokers, distributed operation, production deployment, financial messages, market data, broker connectivity, or live-capital behavior.

## 6. Consequences

- In-process communication is bounded and backpressured.
- Isolated processes communicate locally without public network exposure.
- Cross-boundary transport and FIL messages receive distinct protection and validation.
- Delivery truth remains separate from execution and persistence.
- Retry cannot hide unresolved or duplicated effects.
- Components remain correct when valid messages arrive out of order.
- Protective communication retains bounded capacity during congestion.
- Transport technologies remain replaceable.
- Foundation accepts a local Service Bus as essential infrastructure without claiming high availability.

## 7. Risks and Mitigations

- **Resource exhaustion:** use bounded channels, frame-size limits, quotas, timeouts, and flow control.
- **False trust from authenticated transport:** independently validate every FIL producer, message, permission, and Guardian restriction.
- **IPC permission drift:** verify endpoint ownership and access at creation and during health assessment; fail closed otherwise.
- **Duplicate effects:** preserve logical operation identity and require deduplication or verified reconciliation.
- **Hidden order dependence:** test reorder, delay, duplication, gaps, and stale arrival for every unordered Contract.
- **Protective-lane abuse:** require authority for priority and maintain bounded fair scheduling.
- **Service Bus failure:** expose degradation, restrict dependent authority, and reconcile before restoration.
- **Transport lock-in:** isolate every mechanism behind Falcon-owned Transport Adapters.
- **Custom framing defect:** keep framing minimal, versioned, bounded, fuzz-tested, and semantically subordinate to FIL.

## 8. Compatibility and Transition

This decision realizes, but does not redefine, ADR-F003 and ADR-F004.

Future remote, brokered, federated, or distributed communication requires a new Accepted ADR. It SHALL preserve FIL, authority ownership, delivery-state truth, duplicate-effect protection, ordering declarations, Guardian enforcement, and complete evidence.

## 9. Conformance Evidence

Conformance requires:

- bounded-channel capacity and backpressure tests;
- proof that no governed channel is unbounded;
- Windows Named Pipe and Linux Unix Domain Socket access-control tests;
- proof that FRS-001 creates no public or remote listener;
- mutual endpoint authentication and encrypted-route tests;
- FIL producer, integrity, expiry, replay, recipient, and downgrade rejection tests;
- delivery-state separation tests;
- bounded retry and duplicate-effect prevention tests;
- reordering, delay, duplicate, gap, stale, and replay tests;
- protective-lane congestion and starvation tests;
- frame-size, truncation, corruption, version, and fuzz tests;
- Service Bus failure, restriction, reconciliation, and independent release evidence;
- transport-replacement boundary tests; and
- proof that no financial or live-capital communication exists.

## 10. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner and current Constitutional Authority | Accepted | رائد عموره — “موافق على سياسة الاتصال بصيغتها النهائية، متضمنة استقلال ترتيب وصول الرسائل، ضمن ADR-I004.” | 2026-07-25 |
