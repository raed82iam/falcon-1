# CON-029 — Platform Restriction State Persistence

**Identifier:** CON-029 — Proposed Reservation  
**Version:** Proposed 1.0  
**Status:** Proposed

## State

Restriction ID/version, Platform mode, issuer/mandate, active restrictions, isolated/protected entities, authority effects, evidence, issue/review/expiry time, release conditions, recovery state, Guardian identity/version, integrity/provenance, reconciliation identity, and supersession.

## Rules

- no acknowledged restriction change before required persistence;
- uncertain persistence means `UNCERTAIN`, no blind retry, and no broader authority;
- restore trusted restriction state before ordinary startup;
- restart, failover, deployment, or Guardian replacement SHALL not clear state;
- correction appends; release creates a separately authorized successor state;
- conflicting replicas require fail-closed reconciliation and independent evidence.

