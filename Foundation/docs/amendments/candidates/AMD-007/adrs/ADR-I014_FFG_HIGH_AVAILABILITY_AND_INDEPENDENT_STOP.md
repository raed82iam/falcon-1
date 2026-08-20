# ADR-I014 — FFG High Availability and Independent Stop

**Identifier:** ADR-I014  
**Version:** Proposed 1.0  
**Status:** Proposed  
**Stage 1 Authority:** Not Granted

## Decision

FFG SHALL not be a single point of failure or a self-validating cluster.

The architecture SHALL provide:

- independently identified primary and standby protection instances;
- protected shared restriction state with split-brain prevention;
- independent heartbeat/watchdog evidence;
- verified identity, version, configuration, policy, authority, and state before takeover;
- a physically or logically independent stop channel;
- manual authorized emergency access;
- quorum for Catalog-declared irreversible or broad actions;
- fail-closed behavior when leadership/authority is uncertain;
- independent compromise isolation and evidence.

## Stop Channel

The stop channel SHALL:

- remain usable when FFG, Service Bus, or ordinary Runtime control is compromised where technically possible;
- support only pre-authorized bounded stop/restrict actions;
- require strong identity, authority, integrity, replay defense, dual control where required, and immutable evidence;
- never grant restart, release, broader authority, or business action;
- persist its restriction through CON-029.

## Failover

Failover SHALL preserve restriction state and never broaden authority. Unverified standby, divergent state, unknown revocation, or quorum loss SHALL retain the narrowest trustworthy protection and escalate.

## Rejected Alternatives

- one FFG process;
- standby inheriting authority merely by starting;
- stop channel sharing all ordinary dependencies;
- majority quorum without identity/mandate verification;
- automatic release after failover.

## Consequences

CON-029, CON-030, CON-031, CON-032, AUT-003, survival-set requirements, environment profiles, and verification are mandatory before activation.

