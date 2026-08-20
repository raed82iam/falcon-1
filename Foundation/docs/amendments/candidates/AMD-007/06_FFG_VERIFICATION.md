# FFG Verification

**Status:** Proposed

AUT-002 v2.1 and ADR-I011 satisfy the required FFG boundary:

- FFG is inside Foundation;
- it owns Platform modes, technical containment, cross-Application isolation, persistence of restrictions, and Platform release conditions;
- it consumes only technical identity, state, criticality, dependencies, requirements, and evidence;
- it cannot understand or modify business meaning;
- it does not own Runtime, Lifecycle, Resources, Service Bus, FIL, Security, Persistence, Recovery, Self-Repair, or Self-Evolution;
- it independently validates CON-022 requests;
- it may reject, investigate, narrow, accept, or strengthen requests;
- restart/failover/time do not clear restrictions;
- compromise is independently containable.

Activation remains blocked by technical-criticality, survival-set, trigger, consequence/release, HA, stop-channel, duration/quorum, registration, Manifest, and execution-interface gaps.

