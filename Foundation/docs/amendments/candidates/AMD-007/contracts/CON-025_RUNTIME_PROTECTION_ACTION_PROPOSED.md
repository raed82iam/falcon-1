# CON-025 — Runtime Protection Action

**Identifier:** CON-025 — Proposed Reservation  
**Version:** Proposed 1.0  
**Status:** Proposed

Defines authorized FFG requests and Runtime results for isolate instance, suspend workload, terminate runtime, activate Approved standby, block execution, and restore Approved runtime state.

Every action SHALL include action/target identity, mandate, consequence, scope, prerequisites, deadline, idempotency identity, expected state, evidence, and rollback. Runtime SHALL validate current state and authority, execute only owned actions, distinguish accepted from completed, prevent duplicate effects, and return actual/uncertain/failed outcome. It SHALL NOT release the governing restriction.

