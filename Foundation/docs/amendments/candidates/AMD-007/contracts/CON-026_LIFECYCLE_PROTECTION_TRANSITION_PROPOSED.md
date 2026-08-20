# CON-026 — Lifecycle Protection Transition

**Identifier:** CON-026 — Proposed Reservation  
**Version:** Proposed 1.0  
**Status:** Proposed

Defines FFG-governed transitions to restricted, suspended, isolated, safe, recovery-guard, failed, stopped, and approved standby states.

Transition identity, subject, current/target state, restriction, authority, cause, evidence, dependencies, persistence, expiry/review, and release prerequisites are mandatory. Invalid, stale, reordered, duplicate, or unauthorized transitions fail closed. Lifecycle executes the transition; it does not decide Guardian release or domain safety.

