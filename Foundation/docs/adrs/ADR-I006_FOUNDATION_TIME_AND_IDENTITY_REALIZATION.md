# ADR-I006 — Foundation Time and Identity Realization

**Identifier:** ADR-I006  
**Version:** 1.0  
**Status:** Accepted  
**Date:** 2026-07-25  
**Decision Owner:** Falcon Project Owner  
**Scope:** FRS-001 operational identifiers, canonical time, runtime time provision, clock quality, uncertainty, monotonic epochs, expiry, and temporal failure  
**Affected Specifications:** SYS-001, SYS-008, SYS-009, SEC-001, AWR-001, AUT-001, FRS-001  
**Applicable Standards:** STD-003, STD-007, STD-009, STD-012, STD-013  
**Related ADRs:** ADR-F002, ADR-F003, ADR-F004, ADR-F005, ADR-F006, ADR-F008, ADR-I003, ADR-I004, ADR-I005  
**Supersedes:** None  
**Superseded By:** None  
**Decision Record:** Project Owner approval recorded on 2026-07-25

## 1. Context

FRS-001 requires canonical operational identifiers, a runtime time source, clock-quality handling, and deterministic expiry evaluation before implementation.

Falcon must distinguish identity from identifier, UTC time from elapsed duration, timestamp order from causality, and observed time from trustworthy time. These distinctions must survive replacement of language, operating system, database, deployment platform, and time source.

## 2. Decision Drivers

- globally unique and typed operational identifiers;
- identifier issuance independent of platform APIs;
- stable retry and duplicate-effect identity;
- one language- and database-independent timestamp representation;
- correct separation of UTC and monotonic time;
- explicit runtime-epoch boundaries;
- evidence-based clock quality;
- decisions evaluated against uncertainty rather than a point estimate;
- privacy control for time-bearing identifiers;
- conservative expiry and validity decisions;
- truthful degradation when time cannot be trusted; and
- canonical cross-platform encoding.

## 3. Higher-Authority Constraints

This decision remains subordinate to the Vision, Constitution, SYS-001, SYS-008, SYS-009, SEC-001, AWR-001, AUT-001, approved Contracts, and FRS-001.

> **Identity does not grant authority. A timestamp does not prove truth. Time order does not prove causality.**

> **Unknown time quality, no permissive time-dependent decision.**

An identifier identifies only within its declared scheme and scope. A clock provides observations; it does not create authority or establish factual truth.

## 4. Alternatives Considered

### Direct use of platform identifier and clock APIs

This was rejected because components would become coupled to .NET, operating-system time, UUID implementation details, and inconsistent test behavior.

### One timestamp for occurrence, receipt, execution, and persistence

This was rejected because materially different observations and owners would be collapsed into a misleading chronology.

### Wall-clock time for durations and timeouts

This was rejected because wall-clock correction, rollback, or jump can invalidate elapsed-time behavior.

### One source self-declaring trusted time

This was rejected because source availability or self-assertion does not prove accuracy, integrity, or acceptable uncertainty.

### Falcon-owned providers with governed catalogs and canonical encoding

This was selected because identity and time policies remain stable while generation mechanisms, deployment sources, and platform implementations remain replaceable.

## 5. Decision

### 5.1 Identity and Identifier Separation

An identifier is a value used to refer to a logical subject. Identity is the governed binding between that identifier and the subject's immutable attributes, provenance, issuer, scope, integrity, and lifecycle.

The Identifier Provider SHALL issue identifier values. It SHALL NOT:

- create the logical subject;
- declare the subject admitted or trustworthy;
- establish identity ownership;
- grant authorization;
- infer authority from possession of an identifier; or
- alter the subject's immutable identity attributes.

The accountable owner of the logical subject SHALL establish and preserve the identity binding.

### 5.2 Falcon Identifier Provider

> **Components SHALL NOT generate operational identifiers directly. All operational identifiers SHALL be issued through the Falcon Identifier Provider Contract.**

Components SHALL NOT depend directly on `Guid.CreateVersion7()`, operating-system random APIs, database identity generation, or another platform-specific identifier API.

The Falcon Identifier Provider SHALL:

- select the approved Identifier Profile for the requested identifier class and Deployment Profile;
- validate requester permission to request that class;
- use the approved time and randomness inputs;
- produce the canonical representation;
- preserve declared uniqueness and collision controls;
- expose the profile and scheme used;
- apply privacy policy;
- issue required evidence; and
- reject unknown classes, profiles, scopes, or noncanonical requests.

Changing the provider realization or an approved generation profile SHALL NOT change stable Falcon Contracts or logical identity meaning.

### 5.3 Operational Identifier Profile

The default internal FRS-001 operational identifier profile SHALL use UUID version 7 under RFC 9562 for:

- message identity;
- logical operation identity;
- event identity;
- decision identity;
- evidence identity;
- delivery-attempt identity;
- correlation identity;
- recovery-execution identity; and
- runtime-epoch identity where IDN-001 permits it.

UUIDv7 is selected for standardized uniqueness properties and storage locality. Its embedded approximate creation time is not authoritative time.

> **The timestamp embedded in UUIDv7 SHALL NOT establish causality, freshness, expiry, authority, factual occurrence time, guaranteed creation order, arrival order, or execution order.**

If the active Deployment Profile prohibits timestamp disclosure at a boundary, IDN-001 MAY select another Approved Identifier Profile for that class and boundary.

UUIDv8 or another future scheme SHALL NOT be used merely because it is available. It requires an Approved profile defining generation, uniqueness, privacy, canonical representation, compatibility, and migration.

### 5.4 Typed Identifiers

Every operational identifier SHALL have a declared identifier class. An identifier valid for one class SHALL NOT be accepted as another class.

The canonical textual model is:

```text
urn:falcon:id:<class>:<uuid>
```

Initial classes include:

```text
message
operation
event
decision
evidence
attempt
correlation
recovery
runtime-epoch
```

IDN-001 SHALL define the complete class catalog, permitted schemes, scopes, privacy rules, and lifecycle.

### 5.5 Semantic and Artifact Identifiers

Governed semantic subjects SHALL retain registered stable identifiers rather than receive arbitrary operational UUIDs. These include:

- Specifications;
- Standards;
- ADRs;
- Contracts;
- schemas;
- component types;
- Domain IDs;
- Crypto Profiles; and
- configuration keys.

Artifact identity SHALL include canonical artifact name, version, cryptographic digest, signature where required, and provenance. UUID possession alone SHALL NOT establish artifact identity.

Human, instance, and workload identity SHALL be established through the issuer, certificate or equivalent proof, environment, artifact, scope, validity, and revocation state defined by FDN-003 and CON-009.

### 5.6 Identifier Canonical Representation

UUID text SHALL use lowercase hexadecimal with the standard hyphenated representation.

UUID binary exchange SHALL use RFC 9562 network byte order. Platform-specific in-memory or database byte ordering SHALL NOT become a Falcon interchange format.

Canonical parsing SHALL reject:

- wrong identifier class;
- unsupported scheme or version;
- noncanonical text at protected boundaries;
- nil or prohibited reserved values;
- malformed variant or version;
- identifier and immutable-attribute conflict; and
- representation that changes bytes across Windows, Linux, or PostgreSQL.

An identifier SHALL NOT be treated as a secret, password, bearer capability, authorization token, or integrity proof.

### 5.7 Identity Continuity and Collision

> **Repeated reference to an existing identifier for the same immutable logical subject constitutes identity continuity. Issuance or use of the same identifier for a different logical subject, or with conflicting immutable identity attributes, constitutes an identity collision and SHALL trigger rejection, containment, and reconciliation.**

Retry SHALL preserve the original logical Operation ID or Message ID as required while creating a distinct Delivery Attempt ID. This is continuity, not collision.

An identity collision SHALL NOT be resolved by overwriting, silently selecting one record, or issuing a new identifier that conceals the conflict.

Replacement, trust restoration, or changed immutable identity SHALL create a new identity and preserve the supersession or lineage relationship.

Identifiers SHALL NOT be reassigned to a different subject.

### 5.8 Canonical Time Representation

> **Falcon SHALL define one canonical timestamp representation independent of any programming language, operating system, database, or deployment platform.**

The FRS-001 canonical timestamp form SHALL be:

```text
YYYY-MM-DDTHH:MM:SS.ffffffZ
```

It SHALL use:

- UTC only;
- the RFC 3339 date-time model;
- exactly six fractional decimal digits;
- uppercase `T` and `Z`;
- a four-digit year;
- seconds from `00` through `59`; and
- one canonical representation for one represented instant.

Local times, named time zones, numeric offsets, culture-specific formats, omitted fractional digits, and alternative equivalent forms are prohibited in governed interchange, signing, canonicalization, persistence boundaries, and evidence.

Local presentation MAY occur at the user-interface boundary but SHALL NOT alter the governed UTC value.

### 5.9 Precision and Conversion

When a source has precision finer than one microsecond, canonical conversion SHALL discard the sub-microsecond remainder toward the earlier instant and SHALL expand the declared uncertainty to cover the discarded precision.

Conversion SHALL NOT silently round into a later instant.

When a source has precision coarser than one microsecond, serialization SHALL NOT claim false precision. The observation's resolution and Maximum Uncertainty SHALL expose the limitation even though the canonical field contains six decimal positions.

Parsing at canonical or cryptographically protected boundaries SHALL accept only the canonical form. Lenient external parsing, if later required, SHALL occur in a separate Adapter and SHALL produce a new canonical value with provenance and conversion evidence.

### 5.10 Leap-Second Handling

The canonical representation SHALL NOT encode `:60`.

Every Clock Source capability profile SHALL declare its leap-second and time-smear behavior.

Input containing a leap-second representation SHALL NOT enter governed canonical form without an Approved conversion policy that:

- identifies the source scale and behavior;
- produces one canonical UTC representation;
- records conversion provenance;
- expands uncertainty appropriately; and
- prevents duplicate or reversed instants.

A source with unknown leap behavior SHALL NOT alone establish `VERIFIED` quality for a decision materially affected by that behavior.

### 5.11 Falcon Time Provider

> **Every runtime SHALL obtain time exclusively through the Falcon Time Provider Contract. The implementation of that Contract is deployment-specific and SHALL remain transparent to components.**

Components SHALL NOT directly read the operating-system wall clock, platform monotonic counter, hypervisor clock, database server time, cloud time, NTP, NTS, or hardware clock.

An implementation MAY use:

- a system clock;
- secure real-time clock;
- hypervisor time;
- cloud time service;
- authenticated network time;
- governed local source; or
- approved combination of independent sources.

The Deployment Profile SHALL define the permitted realization, verification, trust assumptions, availability behavior, and required evidence.

.NET `TimeProvider`, operating-system APIs, and database time are replaceable implementation mechanisms inside Adapters; they are not Falcon Contracts or authoritative policy.

### 5.12 UTC and Monotonic Time

UTC wall time SHALL be used for governed instants such as occurrence claims, observations, receipt, acceptance, execution, persistence, validity, and evidence.

Monotonic time SHALL be used for in-epoch elapsed durations such as:

- timeout;
- retry delay;
- watchdog interval;
- health interval;
- execution duration; and
- bounded waiting.

> **Durations SHALL use monotonic time. Civil timestamps SHALL use UTC time.**

Wall-clock correction SHALL NOT extend an elapsed timeout. Monotonic time SHALL NOT be converted into an authoritative UTC instant without an Approved correlation observation and uncertainty treatment.

### 5.13 Runtime Epoch Identity

> **Monotonic observations SHALL be comparable only within the same Runtime Epoch and approved Clock Source. Runtime Epochs are deployment-defined and SHALL NOT be assumed to survive restart, migration, failover, or platform transition unless explicitly supported.**

Every monotonic observation SHALL identify:

- Runtime Epoch ID;
- Clock Source ID;
- monotonic value;
- frequency or conversion rule;
- resolution; and
- applicable Clock Capabilities.

Restart, container recreation, process replacement, virtual-machine migration, live migration, failover, platform transition, or provider replacement SHALL create a new Runtime Epoch unless the active Deployment Profile explicitly proves continuity.

Values from different epochs SHALL NOT be subtracted or ordered directly.

### 5.14 Time Observation

A material Time Observation SHALL contain:

```text
ObservedUtc
MonotonicObservation
ClockSourceId
RuntimeEpochId
ClockQuality
MaximumUncertainty
LastVerification
VerificationAge
ClockCapabilities
EvidenceReference
```

Clock Capabilities SHALL declare, as applicable:

- UTC availability;
- monotonic support;
- resolution and stability;
- source authentication;
- tamper evidence;
- restart continuity;
- migration and failover continuity;
- leap-second behavior; and
- secure-verification capability.

A capability claim SHALL NOT prove the capability. The active Deployment Profile and verification evidence SHALL establish whether it may be relied upon.

### 5.15 Clock-Quality States

TIM-001 SHALL define at least:

| Status | Meaning |
|---|---|
| `VERIFIED` | Approved verification evidence demonstrates that source, integrity, freshness, and uncertainty meet the declared decision profile |
| `DEGRADED` | Evidence supports only a narrower set of uses or a wider uncertainty bound |
| `UNTRUSTED` | Detected manipulation, contradiction, unacceptable jump, or failed verification prevents reliance |
| `UNKNOWN` | Available evidence is insufficient to determine acceptable quality |

Clock quality SHALL be scoped. A clock acceptable for coarse operational reporting may be unacceptable for certificate validity, replay prevention, or a short command-expiry window.

### 5.16 Clock-Quality Verification

> **VERIFIED clock quality SHALL require approved verification evidence appropriate for the active Deployment Profile. No single time source SHALL, by itself, establish VERIFIED status unless explicitly permitted by that Deployment Profile.**

The Deployment Profile SHALL declare:

- permitted sources and source identities;
- required source independence;
- minimum source count where applicable;
- maximum divergence and uncertainty;
- maximum verification age;
- source-authentication and integrity requirements;
- single-source policy;
- holdover and offline behavior;
- downgrade thresholds; and
- restoration evidence.

A source SHALL NOT self-authorize `VERIFIED` status merely by responding successfully.

FRS-001 SHALL NOT require Internet availability. Its local or offline Deployment Profile SHALL explicitly state how initial verification, holdover, drift, uncertainty growth, and loss of confidence are handled.

### 5.17 Temporal Uncertainty

> **Temporal decisions SHALL be evaluated against the uncertainty interval, not against the observed timestamp alone.**

For observation `T` with maximum uncertainty `U`:

```text
EarliestPossibleTime = T - U
LatestPossibleTime   = T + U
```

A temporal condition is proven only when the complete uncertainty interval satisfies it.

If the complete interval violates the condition, the condition is proven false.

If the interval crosses a material boundary, the result SHALL be `TEMPORALLY_UNCERTAIN` and the dependent decision SHALL follow its conservative failure policy.

Unknown or unbounded uncertainty SHALL NOT be treated as zero.

### 5.18 Validity and Expiry

For a not-before boundary, Falcon may treat an item as active only when the earliest possible current time is at or after the required boundary.

For an expiry boundary, Falcon may treat an item as unexpired only when the latest possible current time is before the declared expiry according to the Contract's inclusive or exclusive rule.

Commands and Queries that are expired or temporally uncertain at a required validity boundary SHALL be rejected or restricted.

Event occurrence, delivery validity, and replay validity SHALL remain separate. Expiry of an Event delivery window does not erase an established historical fact.

Certificate, key, manifest, configuration override, delegation, and Security Context validity SHALL each use their governing temporal policy.

Passage of time, restart, source recovery, or one apparently correct observation SHALL NOT release a Guardian restriction.

### 5.19 Distinct Time Semantics

Falcon SHALL distinguish:

- `occurred_at`: the fact owner's claim about occurrence;
- `observed_at`: when an observer recorded its observation;
- `created_at`: when an artifact or message was created;
- `received_at`: when a destination transport received it;
- `accepted_at`: when the responsible owner accepted it;
- `executed_at`: when governed execution occurred;
- `persisted_at`: when required persistence committed; and
- `effective_at`: when governed state became effective.

Each field SHALL identify its accountable source and clock evidence where material.

An intermediary SHALL NOT overwrite producer time. It SHALL add its own separately named observation.

A generic `timestamp` field SHALL NOT replace distinct temporal meanings.

### 5.20 Causality and Ordering

Causation ID, authoritative state version, Contract sequence, and temporal observation SHALL remain distinct.

- Causation expresses a declared causal relationship.
- State version expresses authoritative succession.
- Sequence expresses ordering only within its declared Contract scope.
- Time expresses an observation with uncertainty.
- UUIDv7 embeds approximate generation time for its profile but establishes none of the above.

Clock order, UUID sort order, send order, arrival order, and persistence order SHALL NOT be substituted for one another.

When chronology conflicts with authoritative state, causation, or integrity evidence, Falcon SHALL expose the contradiction and reconcile it rather than rewrite history automatically.

### 5.21 Identifier Privacy

> **Deployment Profiles exposing identifiers outside trusted boundaries SHALL evaluate whether disclosure of approximate creation time is acceptable. Alternative Identifier Profiles MAY be selected where privacy requirements prohibit timestamp disclosure.**

The assessment SHALL consider:

- creation-time disclosure;
- activity-volume inference;
- correlation across boundaries;
- predictability and enumeration;
- retention and public exposure; and
- compatibility consequences.

An alternative profile SHALL remain typed, canonical, non-authoritative, collision-controlled, and governed through IDN-001.

### 5.22 Time and Identifier Failure Policy

The Time Provider or Identifier Provider SHALL expose failure and uncertainty explicitly.

Detected clock rollback, unacceptable forward jump, source contradiction, excessive verification age, unknown source, integrity failure, epoch mismatch, identifier collision, or provider failure SHALL:

- prevent fabricated success;
- notify Health Monitoring;
- update Self-Awareness Temporal Awareness and Fitness to Operate;
- deny time- or identity-dependent authority where the required condition is not proven;
- invoke consequence-appropriate Guardian restriction;
- preserve unaffected operation only where isolation and independent fitness are proven; and
- enter governed reconciliation or recovery.

Return of the provider or a plausible current value SHALL NOT automatically restore trust or authority. Recovery requires new verification evidence and the approved independent release path.

### 5.23 Canonical Encoding Ownership

> **Canonical encoding of identifiers and timestamps SHALL be defined by the Falcon Canonical Encoding Specification rather than by individual Catalogs or components.**

`FCE-001` SHALL be titled:

> **Falcon Canonical Encoding Specification**

It SHALL be the sole encoding authority for:

- Cryptographic Domain Context;
- operational identifiers;
- canonical timestamps;
- Runtime Epoch IDs; and
- protected or signed Time Observations.

IDN-001 defines governed identifier values and profiles. TIM-001 defines governed time types, quality, capabilities, and thresholds. Neither Catalog may independently redefine canonical bytes or text.

### 5.24 Required Pre-Implementation Artifacts

Implementation of governed identifier or time behavior SHALL NOT begin until:

1. `IDN-001 — Foundation Identifier Catalog` is Approved;
2. `TIM-001 — Foundation Time and Clock-Quality Catalog` is Approved;
3. expanded `FCE-001 — Falcon Canonical Encoding Specification` is Approved and registered;
4. the FRS-001 Deployment Profile defines identifier and time-source realizations;
5. canonical cross-platform vectors are Approved;
6. clock-failure, uncertainty, collision, privacy, restart, migration, and recovery tests are complete; and
7. affected Contract or schema amendments, if required, are separately Approved.

### 5.25 Scope Limitation

This decision does not authorize source implementation, external time-service connectivity, production identity issuance, remote deployment, distributed consensus time, financial integration, or live-capital behavior.

## 6. Consequences

- Components become independent of platform UUID and clock APIs.
- UUIDv7 provides the internal default without becoming authoritative time.
- Identifier privacy can vary by governed deployment boundary.
- UTC and monotonic time receive separate purposes.
- Monotonic comparison is constrained by Runtime Epoch.
- Clock trust becomes evidence- and Deployment-Profile-based.
- Temporal decisions become interval-based and conservative.
- Identity continuity remains compatible with retry while collisions become explicit.
- FCE-001 becomes the single canonical encoding authority.
- IDN-001 and TIM-001 become mandatory implementation gates.

## 7. Risks and Mitigations

- **Identifier Provider becomes an identity authority:** restrict it to issuance mechanics; preserve logical identity ownership elsewhere.
- **UUIDv7 time mistaken for truth:** prohibit use for causality, expiry, authority, or authoritative chronology.
- **Timestamp privacy leakage:** require boundary assessment and allow an Approved alternative profile.
- **Platform UUID byte-order mismatch:** mandate RFC network byte order and cross-platform vectors.
- **Clock rollback corrupts timeout:** use monotonic time within one Runtime Epoch.
- **Monotonic values compared across epochs:** carry Runtime Epoch ID and reject invalid comparison.
- **False precision:** record resolution and expand uncertainty.
- **Single source self-declares trust:** require Deployment Profile verification evidence.
- **Leap handling diverges:** define source capability and one approved conversion policy.
- **Validity decided optimistically:** evaluate the full uncertainty interval.
- **Collision concealed as retry:** distinguish repeated reference from conflicting subject binding.
- **Canonical logic fragments:** assign all representation to FCE-001.

## 8. Compatibility and Transition

This decision realizes existing FIL, identity, persistence, and health requirements without changing their approved meaning.

CON-004, CON-009, FIL-001, FDN-003, FDN-004, or other Approved artifacts may require versioned amendments to adopt typed identifiers, exact canonical timestamps, Runtime Epoch, or Time Observation fields. This ADR does not modify those artifacts automatically.

Future identifier, time, or synchronization profiles require IDN-001 or TIM-001 versioning and an ADR when the change is architecturally consequential. Stable logical identity, historical interpretation, uncertainty, and canonical compatibility SHALL be preserved.

## 9. Conformance Evidence

Conformance requires:

- proof that components cannot generate governed operational identifiers directly;
- UUIDv7 and alternative-profile vectors where applicable;
- typed-identifier wrong-class rejection;
- RFC network-byte-order verification across Windows, Linux, and PostgreSQL;
- identifier continuity, collision, and retry tests;
- UUID privacy assessment for every exposed boundary;
- exact canonical timestamp parsing and serialization vectors;
- sub-microsecond conversion and uncertainty-expansion tests;
- leap-second and time-smear policy tests;
- proof that components cannot directly access platform clocks;
- UTC versus monotonic-use analysis;
- Runtime Epoch restart, migration, failover, and mismatch tests;
- clock-quality downgrade and restoration evidence;
- uncertainty-interval boundary tests;
- rollback, forward-jump, contradiction, stale-verification, and unknown-source tests;
- evidence that channel, UUID, timestamp, and arrival ordering do not replace causality or state version;
- independent release after time or identity failure; and
- proof that no financial or live-capital identity or time path exists.

## 10. References

- RFC 3339, Date and Time on the Internet: Timestamps.
- RFC 8915, Network Time Security for the Network Time Protocol.
- RFC 9562, Universally Unique Identifiers.
- Microsoft .NET 10 `Guid.CreateVersion7` documentation.
- Microsoft .NET 10 `TimeProvider` documentation.

## 11. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner and current Constitutional Authority | Accepted | رائد عموره — “موافق على سياسة الوقت والهوية بصيغتها النهائية ضمن ADR-I006، وعلى إضافة IDN-001 وTIM-001 وتوسيع FCE-001 ليصبح Falcon Canonical Encoding Specification.” | 2026-07-25 |
