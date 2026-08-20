# FDN-001 — Foundation State Authority and Persistence Catalog

**Identifier:** FDN-001  
**Version:** 1.0  
**Status:** Approved  
**Effective Date:** 2026-07-24  
**Approval Record:** GOV-008  
**Owner:** Falcon Foundation Authority  
**Governing Authority:** FRS-001; ADR-F002; ADR-F005  
**Supersedes:** None  
**Superseded By:** None

## 1. Purpose

This catalog assigns one authoritative owner and source to every FRS-001 state class and defines its persistence, evidence, concurrency, and recovery obligations.

## 2. State-Class Catalog

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

## 3. Contract Field Ownership

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

## 4. Universal Rules

1. A cache, log, message, event, report, replica, or snapshot does not acquire authority merely by containing a value.
2. Every accepted change SHALL identify prior version, requester, authority, result, and evidence.
3. Unknown integrity, conflict, or freshness SHALL be explicit and SHALL restrict dependent authority.
4. Corrections append; they do not rewrite accepted history.
5. Restoration reconciles ownership, version, causation, integrity, and unresolved restrictions.

## 5. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner | Approved | GOV-008 | 2026-07-24 |
