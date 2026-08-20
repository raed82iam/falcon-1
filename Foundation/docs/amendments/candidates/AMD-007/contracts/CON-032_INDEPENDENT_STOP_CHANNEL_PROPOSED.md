# CON-032 — Independent Stop Channel

**Identifier:** CON-032 — Proposed Reservation  
**Version:** Proposed 1.0  
**Status:** Proposed

## Actions

Only Catalog-approved `HOLD`, `DENY_NEW_ACTIVITY`, `ISOLATE_TARGET`, `ENTER_PLATFORM_SAFE`, and `EMERGENCY_STOP` actions are eligible.

## Request

Stop ID, issuer identity, authority instrument, consequence class, target, action, reason/evidence digest, issue/expiry/review time, nonce/replay context, dual-control/quorum evidence where required, and integrity proof.

## Rules

- default deny on unknown identity, authority, scope, integrity, freshness, replay, target, or consequence;
- stop action may narrow but never broaden authority;
- execution result and persisted restriction state are separate;
- duplicate effects are prevented;
- channel loss cannot clear existing restrictions;
- only the competent release process may restore activity;
- every attempt, including rejection, is auditable through CON-031.

