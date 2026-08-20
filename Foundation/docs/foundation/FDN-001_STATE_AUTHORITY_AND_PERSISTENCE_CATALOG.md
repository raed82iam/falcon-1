# FDN-001 — State Authority and Persistence Catalog

**Version:** 1.1  
**Status:** Approved  
**Approval Record:** GOV-063  
**Documentary Activation:** Active  
**Activation Record:** GOV-092; GOV-093; GOV-094  
**Effective Documentary Instant:** 2026-07-31 22:54:57 +03:00  
**Supersedes:** FDN-001 v1.0  
**Identifier:** FDN-001  
**Canonical Target:** `docs/foundation/FDN-001_STATE_AUTHORITY_AND_PERSISTENCE_CATALOG.md`  
**Owner:** Falcon Foundation Data Authority  
**Governing Authority:** GOV-063; AWR-001 v2.1; SYS-011; SEC-001; CON-006  
**Implementation Authority:** Not Granted  
**Stage 1 Authority:** Not Granted  

## 1. Purpose

FDN-001 defines the authoritative state classes used by Falcon Foundation and the persistence and recovery rules that govern them.

It distinguishes what Foundation owns, what Applications own, what may be derived, what may be cached, what may be observed, and what remains historical.

## 2. Scope

FDN-001 governs:

- authoritative state ownership;
- derived state;
- cached state;
- observed state;
- last-known state;
- expected state;
- desired state;
- historical state;
- persistence ownership;
- write authority;
- read authority;
- retention;
- versioning;
- reconstruction;
- conflict resolution;
- recovery; and
- audit.

## 3. Non-Scope

FDN-001 does not:

- define Application business meaning;
- define trading meaning;
- define financial meaning;
- choose a storage technology;
- define a code implementation;
- grant activation authority;
- grant implementation authority;
- grant Stage 1 authority; or
- create multiple sources of truth for the same authoritative state class.

## 4. Owners and Authority Boundaries

Every authoritative state class SHALL have exactly one authoritative state owner.

Foundation state owners MAY include, subject to the governing tree and contract surface:

- Foundation Identity Owner;
- Service Owner;
- Configuration Owner;
- Dependency Owner;
- Health Evidence Owner;
- Recovery Owner;
- Persistence Owner;
- Evidence Owner; and
- Historical Lineage Owner.

Applications SHALL own their own business state, even when Foundation stores, transmits, caches, or observes technical representations of it.

No owner may interpret another owner’s business meaning.

## 5. State Classes

FDN-001 recognizes the following state classes:

- `AUTHORITATIVE_STATE`
- `DERIVED_STATE`
- `CACHED_STATE`
- `OBSERVED_STATE`
- `LAST_KNOWN_STATE`
- `EXPECTED_STATE`
- `DESIRED_STATE`
- `HISTORICAL_STATE`

## 6. Normative Requirements

- **FDN-001-REQ-001:** Every state class SHALL declare one authoritative owner.
- **FDN-001-REQ-002:** Authoritative state SHALL be the single source of truth for its class.
- **FDN-001-REQ-003:** Derived state SHALL identify its source state and derivation rule.
- **FDN-001-REQ-004:** Cached state SHALL identify cache owner, origin, freshness, and invalidation policy.
- **FDN-001-REQ-005:** Observed state SHALL identify observer, time, source, and confidence.
- **FDN-001-REQ-006:** Last-known state SHALL be explicitly labeled and SHALL never be mistaken for current authoritative state.
- **FDN-001-REQ-007:** Expected state SHALL identify the rule, schedule, or agreement that defines expectation.
- **FDN-001-REQ-008:** Desired state SHALL identify the requesting authority and purpose.
- **FDN-001-REQ-009:** Historical state SHALL preserve prior values, lineage, and decision context.
- **FDN-001-REQ-010:** Persistence owner, read authority, and write authority SHALL be declared for every authoritative state class.
- **FDN-001-REQ-011:** Write authority SHALL be singular for authoritative state.
- **FDN-001-REQ-012:** Read authority MAY be plural only when explicitly permitted by policy.
- **FDN-001-REQ-013:** Retention SHALL be explicit and SHALL distinguish operational, recovery, audit, and historical retention.
- **FDN-001-REQ-014:** Versioning SHALL preserve reconstruction of prior state without rewriting history.
- **FDN-001-REQ-015:** Reconstruction SHALL preserve the original source and all intermediate derivations needed to explain the state.
- **FDN-001-REQ-016:** Conflict resolution SHALL prefer authoritative source rules over derived or cached values.
- **FDN-001-REQ-017:** Recovery SHALL restore the last trusted authoritative state where policy permits.
- **FDN-001-REQ-018:** Audit SHALL preserve ownership, source, time, version, and conflict history.
- **FDN-001-REQ-019:** No authoritative state SHALL have multiple simultaneous writable sources unless explicitly governed as a single coordinated writer.
- **FDN-001-REQ-020:** Hidden writes, silent shadow copies, and undeclared authority replication SHALL be rejected.
- **FDN-001-REQ-021:** Application business state SHALL remain Application-owned even when observed or stored by Foundation.
- **FDN-001-REQ-022:** Foundation SHALL not infer Application business meaning from technical storage identity alone.

## 7. Persistence Rules

Every persisted state SHALL declare:

- identity;
- owner;
- state class;
- source;
- effective time;
- version;
- retention class;
- write authority;
- read authority;
- reconstruction relation; and
- audit relation.

Persistence technology choices are implementation detail only. The technology may not redefine ownership or truth.

## 8. Conflict and Failure Behavior

If state is missing, conflicting, stale, corrupted, or only partially visible:

- the affected state SHALL be marked explicitly;
- the relevant fitness or trust decision MAY degrade;
- write authority SHALL not be inferred;
- recovery SHALL preserve the last trusted state where policy permits;
- reconstructed state SHALL remain challengeable; and
- no hidden fallback source SHALL become authoritative by accident.

## 9. Invariants

1. One authoritative owner per authoritative state class.
2. One source of truth per authoritative class.
3. Derived is not authoritative.
4. Cached is not authoritative.
5. Observed is not the same as owned.
6. Last-known is not current.
7. Desired is not realized.
8. Historical does not mean inactive.

## 10. Acceptance Evidence

Acceptance requires proof of:

- named authoritative owners;
- class separation;
- singular write authority;
- reconstruction of prior state;
- loss handling;
- conflict resolution;
- no duplicate truth source; and
- business-state isolation from technical observation.

## 11. Preservation Annex: Active Edition Content Carried Forward

To keep this successor self-contained, the active edition's authoritative catalog content is preserved below and superseded only by the clarified governance above.

### 11.1 State-Class Catalog

| State class | Authoritative owner | Authoritative source | Persistence | Concurrency rule | Recovery rule |
|---|---|---|---|---|---|
| Falcon release baseline | Foundation Release Authority | Verified CON-010 manifest | Durable, immutable version | Replacement only by new approved manifest | Re-verify from root anchor |
| Falcon instance identity | Security Authority | CON-001 identity record | Durable for instance lifetime | Unique active instance ID | Reissue only through governed trust recovery |
| Core component identity and admission | Kernel Authority | Kernel admission record bound to CON-001 | Durable admission history | One active identity per admitted instance | Reconcile manifest, artifact, identity, and admission |
| Security context | Security Authority | CON-009 context record | Durable when used materially | New context on renewal or restoration | Never reactivate compromised context |
| Effective configuration | Configuration Authority | CON-007 immutable snapshot | Durable material snapshots | Atomic version comparison | Restore last verified compatible snapshot |
| Lifecycle state | Lifecycle Authority | CON-003 authoritative state record | Durable state and full transition history | Compare expected prior version | Reconcile durable state before transition |
| Authority policy baseline | Governance Authority | Approved authority baseline | Durable immutable version | New version, no in-place semantic mutation | Restore exact approved version |
| Authority decision | Authority Engine | CON-002 decision record | Append-only evidence | One result per request evaluation identity | Re-evaluate; never fabricate prior permission |
| FIL message | Original producer | CON-004 message record | Per message class | Immutable logical identity | Retry preserves logical identity |
| Event fact | Declared fact owner | CON-005 event record | Durable when required for reconstruction | Immutable event identity | Replay is marked and cannot recreate authority |
| Operational evidence | Evidence Authority | CON-008 journal | Append-only, integrity-linked | Append with unique record ID | Restore through verified checkpoint and gap analysis |
| Raw health observation | Evidence source owner | Signed observation | Retained through fitness decision window | New observation, never overwrite | Re-observe from trustworthy source |
| Health assessment | Health Monitoring Authority | Assessment record | Durable when material | One assessment per scope/evidence set | Recompute from preserved or new evidence |
| Self Model | Self-Awareness Authority | Versioned Self Model snapshot | Durable material versions | New version from declared inputs | Rebuild and expose uncertainty or contradiction |
| Fitness to Operate | Self-Awareness Authority | CON-006 Fitness decision | Durable decision evidence | Scoped decision identity | Reassess; prior fitness does not revive automatically |
| Guardian restriction | Guardian | CON-011 restriction record | Durable until lawful release | Stricter controlling restriction wins | Restore restriction before affected authority |
| Recovery plan and state | Recovery Authority | Approved recovery record | Durable through closure | One active plan per declared recovery scope unless coordinated | Resume or abandon under plan; never self-release |
| Verification result | Verification Authority | Approved VPL execution record | Durable and immutable after acceptance | New execution ID per run | Re-run; do not overwrite prior result |

### 11.2 Contract Field Ownership

| Contract | Field group | Authoritative owner |
|---|---|---|
| CON-001 | subject, artifact, instance, owner, admission identity | Security Authority for identity; Kernel Authority for admission result |
| CON-002 | request context | requester; authorization result and basis | Authority Engine |
| CON-003 | requested target | requester; accepted state and transition | Lifecycle Authority |
| CON-004 | envelope assertion and payload | original producer; transport result | Service Bus |
| CON-005 | established fact | declared fact owner; delivery observation | Service Bus |
| CON-006 | observation | evidence source; health assessment | Health Monitoring; Fitness result | Self-Awareness Authority |
| CON-007 | item definition | Configuration Authority; supplied value | authorized source; effective value | Configuration Authority |
| CON-008 | source fact | source actor; accepted evidence and integrity status | Evidence Authority |
| CON-009 | identity assurance | Security Authority; delegated authority reference | delegating authority |
| CON-010 | release contents and approval | Foundation Release Authority |
| CON-011 | restriction | Guardian; enforcement result | respective enforcement authority; release result | declared Release Authority |
