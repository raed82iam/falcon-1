# CON-028 — FIL Protection Profile

**Identifier:** CON-028 — Proposed Reservation  
**Version:** Proposed 1.0  
**Status:** Proposed

Extends CON-004 for protection messages without changing the generic FIL envelope.

It SHALL define protected message kinds, schema/version, sender/receiver binding, authority context, security classification, priority authority, integrity/encryption, expiry, replay, delivery-attempt, acknowledgment, dead-letter, emergency-route eligibility, correlation, causation, and evidence reference.

FIL validation SHALL reject unknown mandatory fields, downgrade, ambiguity, unauthorized priority, wrong recipient, invalid integrity, replay, and expired messages. FIL SHALL not interpret Application business reason or create authority.

