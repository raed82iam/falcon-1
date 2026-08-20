# AUT-003 — Guardian Intervention, Release, and Compromise

**Identifier:** AUT-003  
**Version:** Proposed 1.0  
**Status:** Proposed  
**Stage 1 Authority:** Not Granted

## Purpose

Govern Guardian consequence classes, intervention ceilings, release independence, compromise containment, and emergency expiry.

## Consequence Classes

- `G0_OBSERVE`: observation/notification only.
- `G1_LOCAL_RESTRICT`: reversible restriction of one capability or route.
- `G2_ISOLATE`: component/runtime/Application isolation.
- `G3_PLATFORM_CONTAIN`: Platform-wide containment with preserved unaffected operation.
- `G4_PLATFORM_SAFE`: Platform Safe Mode and survival set.
- `G5_EMERGENCY_STOP`: independent stop where continued execution is untrustworthy.

Exact actions, quorum, duration, and release authority belong to the governed Catalog.

## Release

Release SHALL require trigger containment, repair/reconciliation, independent verification proportional to consequence, FFG condition evaluation, competent release authority, persisted successor restriction state, controlled Lifecycle/AUT-001 restoration, and heightened monitoring.

The restricted actor, repairer, FSA, FFG, or evidence producer SHALL NOT alone release a material restriction involving its own conduct.

## Compromise

Suspected Guardian compromise SHALL:

- freeze or narrow its authority;
- preserve current restrictions;
- invoke independent watchdog/stop paths;
- isolate affected identity/runtime;
- reject self-release and self-recovery;
- activate only a verified Approved standby under separate authority;
- preserve immutable evidence;
- require new identity, mandate validation, reconciliation, and independent restoration.

## Duration

Every autonomous action above `G1` SHALL have a review deadline. Expiry does not silently release danger; it invokes the Catalog’s fail-safe continuation, escalation, quorum, or stop rule.

## Acceptance

Every consequence class, excessive action rejection, independent release, compromised FFG, compromised Application Guardian, standby takeover, split-brain, stop channel, expiry, and reconstruction.

