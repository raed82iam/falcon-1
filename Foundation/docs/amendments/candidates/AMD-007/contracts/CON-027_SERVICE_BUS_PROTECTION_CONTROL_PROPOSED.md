# CON-027 — Service Bus Protection Control

**Identifier:** CON-027 — Proposed Reservation  
**Version:** Proposed 1.0  
**Status:** Proposed

Defines protected route actions: quarantine sender, restrict publication, suspend route, preserve control route, rate-limit, stop retry amplification, isolate dead-letter pressure, and restore progressively.

Every control requires authority, route/sender identity, technical trigger, scope, duration/review, priority, evidence, and release reference. Service Bus SHALL preserve original producer, reject replay/expiry/unauthorized change, expose acknowledgment/dead-letter/backpressure outcomes, and never interpret business payload meaning.

