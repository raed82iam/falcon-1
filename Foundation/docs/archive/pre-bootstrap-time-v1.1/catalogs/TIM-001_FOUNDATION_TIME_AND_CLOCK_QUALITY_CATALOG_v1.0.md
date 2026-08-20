# TIM-001 — Foundation Time and Clock-Quality Catalog

**Version:** 1.0  
**Status:** Approved  
**Effective Date:** 2026-07-25  
**Approval Record:** GOV-017  
**Owner:** Falcon Specification Authority  
**Governing Authority:** Falcon Vision; Falcon Constitution; SEC-001; SEC-002; FCE-001; IDN-001; ADR-I006  
**Affected Domains:** All time-dependent Foundation activity  
**Implementation Authority:** Not Granted
**Superseded By:** TIM-001 v1.1 under GOV-035

## 1. Purpose

TIM-001 is the canonical catalog of Falcon time semantics, Clock Quality states, Clock Capabilities, uncertainty rules, Runtime Epoch rules, temporal decision profiles, and source-class requirements.

It ensures that Falcon never treats a plausible timestamp as proven time and never uses wall-clock order as a substitute for causality or authority.

> **Time is an observation with uncertainty, not an unquestionable fact.**

## 2. Catalog Authority

TIM-001 owns:

- time semantic IDs;
- Clock Quality states;
- Clock Capability IDs;
- source-class IDs;
- leap-behavior values;
- temporal evaluation outcomes;
- Temporal Decision Profile IDs and thresholds;
- Runtime Epoch rules;
- Time Observation semantic requirements;
- uncertainty rules;
- verification-age rules; and
- failure and restoration conditions.

TIM-001 does not own:

- canonical timestamp text or Time Observation bytes, owned by FCE-001;
- Runtime Epoch identifier representation, owned by FCE-001 and IDN-001;
- provider implementation;
- deployment-specific source selection, owned by ENV-001;
- cryptographic algorithms;
- authority;
- causality;
- state succession;
- message ordering;
- financial decision thresholds; or
- implementation authority.

## 3. Core Time Types

### 3.1 UTC Instant

A governed civil-time observation represented in UTC with declared uncertainty, resolution, source, quality, and evidence where material.

### 3.2 Monotonic Observation

A non-decreasing observation used only for elapsed duration within one Runtime Epoch and one approved Clock Source.

### 3.3 Duration

An elapsed interval derived from compatible monotonic observations.

### 3.4 Time Observation

A governed Trust Object containing UTC, monotonic, source, epoch, quality, uncertainty, capabilities, and evidence.

### 3.5 Temporal Boundary

A governed instant against which validity, expiry, activation, or another time-dependent condition is evaluated.

### 3.6 Uncertainty Interval

For observed time `T` and Maximum Uncertainty `U`:

```text
EarliestPossibleTime = T - U
LatestPossibleTime   = T + U
```

The complete interval governs temporal decisions.

## 4. Time Profile Lifecycle

Time Profiles use:

- `DRAFT`;
- `APPROVED`;
- `ACTIVE`;
- `DEGRADED`;
- `SUSPENDED`;
- `RETIRED`; and
- `FORBIDDEN`.

Only an `ACTIVE` Time Profile may establish `VERIFIED` Clock Quality for governed runtime decisions.

Approval of this Catalog does not activate the Time Provider or any Time Profile.

## 5. Foundation Time Profile Registry

| Profile ID | Version | Lifecycle | Scope |
|---|---:|---|---|
| `FALCON-TIME-FOUNDATION-1` | `1` | `APPROVED` | Non-financial Foundation verification and future explicitly authorized Foundation runtime |

`FALCON-TIME-FOUNDATION-1` is not `ACTIVE`.

No production or financial Time Profile is Approved.

## 6. Clock Quality States

| State | Meaning | Permitted reliance |
|---|---|---|
| `VERIFIED` | Approved evidence demonstrates source, integrity, freshness, resolution, capabilities, and uncertainty satisfy the declared Temporal Decision Profile | Only within that exact profile and scope |
| `DEGRADED` | Evidence supports a narrower use, older verification, reduced capability, or wider but bounded uncertainty | Only where governing policy explicitly permits the degraded condition |
| `UNTRUSTED` | Manipulation, contradiction, unacceptable jump, failed verification, integrity failure, or prohibited source prevents reliance | No time-dependent permission |
| `UNKNOWN` | Evidence is insufficient to determine quality | No time-dependent permission unless a separately Approved conservative policy applies |

Clock Quality is always scoped.

`VERIFIED` for coarse evidence timing does not imply `VERIFIED` for certificate validity, replay control, command expiry, or short validity windows.

## 7. Temporal Evaluation Outcomes

| Outcome | Meaning |
|---|---|
| `PROVEN_TRUE` | The complete uncertainty interval satisfies the temporal condition |
| `PROVEN_FALSE` | The complete uncertainty interval violates the temporal condition |
| `TEMPORALLY_UNCERTAIN` | The uncertainty interval crosses a material boundary |
| `UNAVAILABLE` | Required time observation or evidence is unavailable |
| `INVALID` | The observation, source, epoch, integrity, or policy is invalid |

Only `PROVEN_TRUE` satisfies a required temporal condition.

## 8. Clock Capability Catalog

| Capability ID | Meaning |
|---|---|
| `UTC_AVAILABLE` | Source provides a UTC-correlated observation |
| `MONOTONIC_AVAILABLE` | Source provides monotonic observations |
| `SOURCE_AUTHENTICATED` | Source identity is authenticated under an Approved mechanism |
| `TAMPER_EVIDENT` | Material tampering can be detected under the declared profile |
| `SECURE_VERIFICATION` | Verification evidence is protected and independently assessable |
| `RESTART_CONTINUITY` | Source claims continuity across declared restart class |
| `MIGRATION_CONTINUITY` | Source claims continuity across declared migration class |
| `FAILOVER_CONTINUITY` | Source claims continuity across declared failover class |
| `LEAP_BEHAVIOR_DECLARED` | Source declares leap-second or smear behavior |
| `RESOLUTION_DECLARED` | Source resolution is known |
| `STABILITY_DECLARED` | Stability or drift behavior is governed |
| `HOLDOVER_SUPPORTED` | Source supports bounded holdover with uncertainty growth |
| `MULTI_SOURCE_VERIFIED` | Observation was assessed against the required independent sources |
| `OFFLINE_VERIFIABLE` | Required verification can be performed without Internet availability |
| `CORRELATION_EVIDENCED` | UTC and monotonic correlation is governed and evidenced |

A capability claim does not prove the capability.

Reliance requires Deployment Profile permission and verification evidence.

## 9. Clock Source Class Catalog

| Source Class ID | Meaning | Initial authorization |
|---|---|---|
| `falcon/time/source/system-utc` | Operating-system civil clock behind the Falcon Time Provider | Not independently authorized |
| `falcon/time/source/system-monotonic` | Operating-system monotonic counter behind the Falcon Time Provider | Not independently authorized |
| `falcon/time/source/secure-rtc` | Protected real-time clock | Not independently authorized |
| `falcon/time/source/hypervisor` | Hypervisor-provided time | Not independently authorized |
| `falcon/time/source/cloud` | Cloud-provider time service | Not independently authorized |
| `falcon/time/source/authenticated-network` | Authenticated network time source | Not independently authorized |
| `falcon/time/source/governed-local` | Governed local or offline verification source | Not independently authorized |
| `falcon/time/source/composite` | Governed evaluation of multiple source observations | Not independently authorized |

These are source classes, not active source instances.

ENV-001 SHALL identify exact source instances, trust assumptions, independence, capabilities, failure behavior, and activation state.

No source class establishes `VERIFIED` merely by returning a value.

## 10. Leap Behavior Catalog

| Leap Behavior ID | Meaning |
|---|---|
| `UTC_STEP_DECLARED` | Source declares governed UTC leap-step behavior |
| `SMEAR_DECLARED` | Source declares a governed smear policy and interval |
| `TAI_CONVERSION_DECLARED` | Source uses another scale with Approved UTC conversion |
| `LEAP_UNSUPPORTED` | Source cannot represent or verify leap behavior for affected decisions |
| `LEAP_UNKNOWN` | Behavior is unknown |

`LEAP_UNKNOWN` and `LEAP_UNSUPPORTED` SHALL NOT alone establish `VERIFIED` for a decision materially affected by leap behavior.

Canonical timestamps SHALL NOT encode `:60`.

## 11. Time Semantic Catalog

| Semantic ID | Meaning | Accountable source |
|---|---|---|
| `occurred_at` | Fact owner's claim about occurrence | Owner of the fact |
| `observed_at` | Time an observer recorded an observation | Observer |
| `created_at` | Time an artifact or message was created | Creator |
| `received_at` | Time a destination transport received an item | Receiving transport boundary |
| `accepted_at` | Time the responsible owner accepted an item | Accepting owner |
| `executed_at` | Time governed execution occurred | Execution authority |
| `persisted_at` | Time required persistence committed | Persistence Authority |
| `effective_at` | Time governed state became effective | Authoritative state owner |
| `verified_at` | Time a verification was completed | Verification authority |
| `issued_at` | Time a governed object or credential was issued | Issuer |
| `expires_at` | Governing expiry boundary | Owner of the validity policy |
| `not_before` | Governing activation boundary | Owner of the validity policy |
| `revoked_at` | Time revocation became effective | Revocation Authority |
| `last_verified_at` | Most recent qualifying verification time | Verification authority |

A generic `timestamp` semantic is prohibited for governed material fields.

An intermediary SHALL preserve producer time and add its own separately named observation.

## 12. Canonical Representation Boundary

FCE-001 exclusively governs:

- `YYYY-MM-DDTHH:MM:SS.ffffffZ`;
- timestamp parsing;
- precision truncation representation;
- Runtime Epoch ID representation;
- protected Time Observation field encoding; and
- canonical Time Observation bytes.

TIM-001 governs meaning, quality, capabilities, thresholds, and uncertainty.

TIM-001 SHALL NOT redefine canonical text, UUID bytes, FCE fields, or binary framing.

## 13. Time Provider Obligations

Every runtime SHALL obtain governed time exclusively through the Falcon Time Provider Contract.

Components SHALL NOT directly read:

- operating-system wall clock;
- platform monotonic counter;
- hypervisor clock;
- database server time;
- cloud time;
- NTP or NTS;
- hardware clock;
- environment-provided timestamp; or
- language runtime time API.

The Time Provider SHALL:

- identify the Time Profile;
- identify exact source instances;
- produce Time Observations;
- preserve UTC and monotonic distinction;
- declare Runtime Epoch;
- declare Clock Quality;
- declare Maximum Uncertainty;
- declare resolution;
- expose verification age;
- expose capabilities;
- expose source contradiction;
- produce evidence;
- apply Deployment Profile thresholds;
- reject invalid source combinations; and
- fail explicitly.

The Provider SHALL NOT:

- self-declare `VERIFIED`;
- hide uncertainty;
- treat one successful response as trust restoration;
- convert wall time into monotonic duration;
- compare different Runtime Epochs;
- supply local time as governed UTC;
- silently change source;
- hide clock correction; or
- fabricate success.

## 14. UTC and Monotonic Use

UTC SHALL be used for governed instants:

- occurrence claims;
- observation;
- creation;
- receipt;
- acceptance;
- execution;
- persistence;
- validity;
- expiry;
- evidence; and
- effective state.

Monotonic time SHALL be used for in-epoch elapsed durations:

- timeout;
- retry delay;
- watchdog interval;
- health interval;
- execution duration;
- bounded waiting; and
- local scheduling delay.

> **Durations use monotonic time. Civil timestamps use UTC.**

Wall-clock correction SHALL NOT extend an elapsed timeout.

Monotonic time SHALL NOT become authoritative UTC without an Approved correlation observation and uncertainty treatment.

## 15. Runtime Epoch Rules

Monotonic observations are comparable only when all are equal:

- Runtime Epoch ID;
- Clock Source ID;
- monotonic unit or conversion rule; and
- declared continuity profile.

A new Runtime Epoch is required upon:

- restart;
- process replacement;
- container recreation;
- virtual-machine migration;
- live migration;
- failover;
- platform transition;
- Time Provider replacement;
- monotonic source replacement;
- counter reset;
- frequency change not covered by the profile; or
- loss of proven continuity.

An active Deployment Profile MAY preserve an epoch only with explicit continuity capability and verification evidence.

Values from different epochs SHALL NOT be:

- subtracted;
- ordered directly;
- used to compute duration;
- merged; or
- treated as continuous.

## 16. Time Observation Semantic Profile

A material Time Observation SHALL contain:

- Observation ID;
- ObservedUtc;
- MonotonicObservation when supported;
- ClockSourceId;
- RuntimeEpochId when required;
- ClockQuality;
- MaximumUncertainty;
- resolution;
- LastVerification;
- VerificationAge;
- ClockCapabilities;
- EvidenceReference;
- Time Profile ID and version;
- Deployment Profile ID and version;
- leap behavior;
- provenance;
- integrity identity; and
- lifecycle.

FCE-001 defines the canonical protected representation.

The observation is a Trust Object under SEC-002.

## 17. Temporal Decision Profiles

The following initial profiles apply only to non-financial Foundation scope and have lifecycle `APPROVED`, not `ACTIVE`.

### 17.1 `FALCON-TIME-EVIDENCE-COARSE-1`

| Property | Value |
|---|---|
| Purpose | Evidence, audit, and operational reporting where sub-second ordering is not relied upon |
| Required Clock Quality | `VERIFIED` or explicitly accepted `DEGRADED` |
| Maximum Uncertainty for `VERIFIED` | 5 seconds |
| Maximum Verification Age for `VERIFIED` | 15 minutes |
| Minimum capabilities | `UTC_AVAILABLE`, `RESOLUTION_DECLARED`, `LEAP_BEHAVIOR_DECLARED` |
| Single-source policy | Permitted only when ENV-001 explicitly authorizes and independently verifies it |
| Failure | Preserve observation as uncertain; prohibit use for security validity or order |

### 17.2 `FALCON-TIME-SECURITY-VALIDITY-1`

| Property | Value |
|---|---|
| Purpose | Certificate, key, manifest, delegation, Security Context, and policy validity |
| Required Clock Quality | `VERIFIED` |
| Maximum Uncertainty | 1 second |
| Maximum Verification Age | 5 minutes |
| Minimum capabilities | `UTC_AVAILABLE`, `SOURCE_AUTHENTICATED`, `SECURE_VERIFICATION`, `RESOLUTION_DECLARED`, `LEAP_BEHAVIOR_DECLARED` |
| Source policy | No single source unless ENV-001 explicitly permits it with equivalent verification evidence |
| Failure | Deny or restrict time-dependent authority |

### 17.3 `FALCON-TIME-FIL-VALIDITY-1`

| Property | Value |
|---|---|
| Purpose | Command, Query, Response, Notice, replay, and delivery validity |
| Required Clock Quality | `VERIFIED` |
| Maximum Uncertainty | 250 milliseconds |
| Maximum Verification Age | 60 seconds |
| Minimum capabilities | `UTC_AVAILABLE`, `SOURCE_AUTHENTICATED`, `SECURE_VERIFICATION`, `RESOLUTION_DECLARED`, `LEAP_BEHAVIOR_DECLARED` |
| Source policy | No single source unless ENV-001 explicitly permits it |
| Failure | `TEMPORALLY_UNCERTAIN`; reject or apply narrower Contract policy |

### 17.4 `FALCON-TIME-DURATION-1`

| Property | Value |
|---|---|
| Purpose | Timeout, retry delay, watchdog, health interval, execution duration, bounded wait |
| Required Clock Quality | Monotonic capability valid within one Runtime Epoch |
| Maximum resolution for `VERIFIED` | 1 millisecond |
| UTC requirement | None for elapsed calculation |
| Minimum capabilities | `MONOTONIC_AVAILABLE`, `RESOLUTION_DECLARED` |
| Epoch policy | Same Runtime Epoch and Clock Source only |
| Failure | End or restrict the affected wait; never extend from wall-clock correction |

### 17.5 `FALCON-TIME-IDENTIFIER-ISSUANCE-1`

| Property | Value |
|---|---|
| Purpose | UUIDv7 generation input |
| Required Clock Quality | `VERIFIED` or explicitly bounded `DEGRADED` under Identifier Provider policy |
| Maximum Uncertainty for `VERIFIED` | 1 second |
| Maximum Verification Age for `VERIFIED` | 5 minutes |
| Minimum capabilities | `UTC_AVAILABLE`, `RESOLUTION_DECLARED` |
| Ordering claim | Prohibited |
| Failure | Identifier Provider fails or uses a separately Approved collision-safe degraded policy |

These thresholds do not authorize financial use.

ENV-001 SHALL prove that selected sources can meet them before activation.

ENV-001 MAY impose stricter thresholds. It SHALL NOT relax a TIM-001 threshold or remove a required capability without a separately Approved TIM-001 amendment.

## 18. Clock Quality Verification

`VERIFIED` requires Approved verification evidence appropriate to:

- Time Profile;
- Temporal Decision Profile;
- Deployment Profile;
- source identity;
- source independence;
- source authentication;
- integrity;
- freshness;
- uncertainty;
- resolution;
- leap behavior;
- holdover state; and
- current environment.

No single source establishes `VERIFIED` by itself unless the Deployment Profile explicitly permits it and records equivalent verification evidence.

The source SHALL NOT verify itself conclusively.

## 19. Uncertainty Evaluation

For observed time `T`, uncertainty `U`, and boundary `B`:

### 19.1 Not-Before

`PROVEN_TRUE` only when:

```text
T - U >= B
```

`PROVEN_FALSE` when:

```text
T + U < B
```

Otherwise the result is `TEMPORALLY_UNCERTAIN`.

### 19.2 Expiry with Exclusive Boundary

Unexpired is `PROVEN_TRUE` only when:

```text
T + U < B
```

Expired is `PROVEN_TRUE` only when:

```text
T - U >= B
```

Otherwise the result is `TEMPORALLY_UNCERTAIN`.

### 19.3 Expiry with Inclusive Boundary

The governing Contract SHALL define the exact comparison. Absence of an explicit inclusive rule SHALL default to the safer exclusive interpretation.

### 19.4 Unknown Uncertainty

Unknown, invalid, negative, overflowed, or unbounded uncertainty SHALL NOT be treated as zero.

## 20. Precision and Resolution

When source precision is finer than one microsecond:

- canonical conversion discards the remainder toward the earlier instant;
- conversion SHALL NOT round later; and
- uncertainty SHALL expand to include discarded precision.

When source precision is coarser:

- canonical syntax still uses six fractional digits;
- resolution SHALL expose the true limitation; and
- the value SHALL NOT claim false precision.

Resolution and uncertainty are distinct.

A precise source can be inaccurate. An accurate source can have coarse resolution.

## 21. Leap-Second and Smear Policy

Input containing second `60` SHALL NOT enter canonical form without an Approved conversion policy.

Every active source SHALL declare one Leap Behavior ID.

A conversion policy SHALL identify:

- source scale;
- leap behavior;
- smear interval where applicable;
- conversion algorithm;
- ambiguity handling;
- duplicate prevention;
- reversed-time prevention;
- provenance;
- uncertainty expansion; and
- test vectors.

ID `LEAP_UNKNOWN` SHALL cause `DEGRADED`, `UNKNOWN`, or `UNTRUSTED` as required by the decision profile.

## 22. Holdover and Offline Operation

FRS-001 does not require Internet availability.

An offline or holdover Deployment Profile SHALL define:

- initial verification;
- source identity;
- last verification;
- starting uncertainty;
- uncertainty growth;
- stability evidence;
- maximum holdover;
- downgrade thresholds;
- loss-of-confidence threshold;
- restart behavior;
- leap behavior;
- reconciliation;
- restoration evidence; and
- permitted Temporal Decision Profiles.

Verification age and uncertainty SHALL grow according to the Approved source profile.

Holdover SHALL transition from `VERIFIED` to `DEGRADED`, `UNKNOWN`, or `UNTRUSTED` when thresholds are exceeded.

## 23. Validity and Expiry Rules

Commands and Queries that are expired or `TEMPORALLY_UNCERTAIN` at a required boundary SHALL be rejected or restricted according to their Contract.

Event semantics remain separate:

- occurrence validity;
- delivery validity;
- replay validity; and
- historical retention.

Expiry of an Event delivery window does not erase an established historical occurrence.

Certificate, key, manifest, configuration override, delegation, Security Context, and policy validity each use their assigned Temporal Decision Profile.

Passage of time or one plausible observation SHALL NOT release Guardian restriction.

## 24. Causality and Ordering

The following are distinct:

- Causation ID;
- authoritative state version;
- Contract sequence;
- UTC observation;
- monotonic duration;
- UUIDv7 sort order;
- send order;
- arrival order;
- persistence order; and
- execution order.

None SHALL be substituted for another.

Time does not establish causality.

UUIDv7 does not establish authoritative order.

Arrival order SHALL NOT be assumed unless the governing Contract explicitly guarantees it.

Chronology conflict SHALL be exposed and reconciled. History SHALL NOT be rewritten automatically.

## 25. Time Evidence

Material Time Provider evidence SHALL include:

- Time Observation ID;
- Time Profile;
- Temporal Decision Profile;
- Deployment Profile;
- source identities;
- source observations;
- source independence assessment;
- authentication and integrity result;
- divergence;
- resolution;
- Maximum Uncertainty;
- LastVerification;
- VerificationAge;
- leap behavior;
- holdover state;
- Runtime Epoch;
- capabilities;
- Clock Quality result;
- evaluator;
- decision time;
- failure reasons;
- challenge path; and
- integrity identity.

Evidence is governed by SEC-002.

## 26. Failure and Restoration

The following SHALL cause explicit downgrade, rejection, or restriction:

- clock rollback;
- unacceptable forward jump;
- source contradiction;
- excessive divergence;
- excessive uncertainty;
- excessive verification age;
- unknown source;
- source-authentication failure;
- integrity failure;
- epoch mismatch;
- counter reset;
- undeclared leap behavior;
- provider failure;
- overflow;
- unavailable required evidence; or
- temporal profile mismatch.

Failure SHALL:

- prevent fabricated success;
- notify Health Monitoring;
- update Self-Awareness Temporal Awareness and Fitness to Operate;
- deny time-dependent authority where required conditions are unproven;
- invoke consequence-appropriate Guardian restriction;
- preserve independently fit unaffected operation only;
- preserve evidence; and
- enter governed reconciliation or recovery.

Provider return, source agreement, restart, elapsed time, or one plausible value SHALL NOT automatically restore `VERIFIED` quality or authority.

Restoration requires new verification evidence and the Approved independent release path.

## 27. Catalog Requirements

- **TIM-001-REQ-001:** Every governed material time use SHALL declare a time semantic and Temporal Decision Profile.
- **TIM-001-REQ-002:** Every runtime SHALL obtain governed time only through the Falcon Time Provider Contract.
- **TIM-001-REQ-003:** Components SHALL NOT directly read platform, database, network, hardware, hypervisor, cloud, or language clocks.
- **TIM-001-REQ-004:** Civil timestamps SHALL use UTC and elapsed durations SHALL use monotonic observations.
- **TIM-001-REQ-005:** Monotonic observations SHALL be compared only within one Runtime Epoch, Clock Source, and conversion rule.
- **TIM-001-REQ-006:** Loss of proven monotonic continuity SHALL create a new Runtime Epoch.
- **TIM-001-REQ-007:** Time Observations SHALL contain the semantic fields and evidence required by section 16.
- **TIM-001-REQ-008:** FCE-001 SHALL remain the sole authority for canonical timestamp and Time Observation representation.
- **TIM-001-REQ-009:** Clock Quality SHALL be scoped to a declared Temporal Decision Profile.
- **TIM-001-REQ-010:** A source SHALL NOT self-authorize `VERIFIED` quality.
- **TIM-001-REQ-011:** `VERIFIED` SHALL require current Approved evidence satisfying profile thresholds.
- **TIM-001-REQ-012:** No single source SHALL alone establish `VERIFIED` unless ENV-001 explicitly permits it with equivalent verification evidence.
- **TIM-001-REQ-013:** Temporal conditions SHALL be evaluated against the complete uncertainty interval.
- **TIM-001-REQ-014:** Unknown or unbounded uncertainty SHALL NOT be treated as zero.
- **TIM-001-REQ-015:** A condition crossed by the uncertainty interval SHALL return `TEMPORALLY_UNCERTAIN`.
- **TIM-001-REQ-016:** Only `PROVEN_TRUE` SHALL satisfy a required temporal condition.
- **TIM-001-REQ-017:** Canonical time conversion SHALL truncate sub-microsecond precision toward the earlier instant and expand uncertainty.
- **TIM-001-REQ-018:** Coarse source resolution SHALL remain explicit and SHALL NOT claim false precision.
- **TIM-001-REQ-019:** Leap second `:60` SHALL require Approved conversion and SHALL NOT appear in canonical form.
- **TIM-001-REQ-020:** Every active source SHALL declare leap or smear behavior.
- **TIM-001-REQ-021:** Generic governed `timestamp` fields SHALL be prohibited in favor of explicit semantics.
- **TIM-001-REQ-022:** Intermediaries SHALL preserve producer time and append their own observations.
- **TIM-001-REQ-023:** Time, causality, state version, Contract sequence, UUID order, arrival order, and persistence order SHALL remain distinct.
- **TIM-001-REQ-024:** Commands and Queries with unproven required validity SHALL be rejected or restricted.
- **TIM-001-REQ-025:** Event delivery expiry SHALL NOT erase established historical occurrence.
- **TIM-001-REQ-026:** Offline and holdover operation SHALL grow uncertainty and downgrade quality at Approved thresholds.
- **TIM-001-REQ-027:** Clock failure SHALL restrict time-dependent authority and SHALL NOT fabricate success.
- **TIM-001-REQ-028:** Provider return or plausible time SHALL NOT automatically restore quality or authority.
- **TIM-001-REQ-029:** `FALCON-TIME-FOUNDATION-1` and all initial Temporal Decision Profiles SHALL remain `APPROVED`, not `ACTIVE`, until separately activated.
- **TIM-001-REQ-030:** Catalog Approval SHALL NOT activate a Time Provider or authorize implementation.

## 28. Conformance Evidence

Conformance requires evidence that:

- direct clock reads by components are rejected or structurally unavailable;
- UTC and monotonic use cannot be confused;
- wall-clock rollback cannot extend timeout;
- different Runtime Epochs cannot be compared;
- source success cannot self-declare `VERIFIED`;
- Temporal Decision Profile thresholds are enforced;
- uncertainty interval produces correct true, false, and uncertain outcomes;
- unknown uncertainty cannot become zero;
- sub-microsecond conversion never rounds later;
- coarse resolution remains visible;
- leap-second input is rejected without Approved conversion;
- generic timestamp semantics fail review;
- intermediary observations do not overwrite producer time;
- UUIDv7 and wall-clock order cannot replace causality;
- arrival order cannot be inferred;
- holdover grows uncertainty and downgrades;
- Clock Quality failure restricts authority;
- provider recovery does not automatically restore trust; and
- FCE-001 vectors pass across Windows and Linux.

## 29. Required Before Activation

No initial Time Profile or Temporal Decision Profile SHALL become `ACTIVE` until:

1. TIM-001 is Approved;
2. FCE-001 is Approved and registered;
3. IDN-001 is Approved;
4. the Falcon Time Provider Contract is Approved;
5. ENV-001 defines exact source instances and independence;
6. Windows and Linux capabilities are verified;
7. source authentication and integrity are proven;
8. uncertainty-growth and holdover rules are approved;
9. leap behavior is declared and tested;
10. Runtime Epoch behavior is tested across restart, migration, and failover;
11. positive, negative, boundary, contradiction, and recovery vectors pass;
12. affected Contracts are reviewed for explicit time semantics;
13. independent activation evidence is complete; and
14. explicit implementation authority exists.

## 30. Foundational Rules

> **A timestamp without source, uncertainty, and scope is not sufficient time evidence.**

> **VERIFIED is earned by evidence; it is not declared by a clock.**

> **Temporal truth is evaluated across an interval, not at one point.**

> **Durations use monotonic time. Civil timestamps use UTC.**

> **Time observes chronology. It does not create causality, authority, or state succession.**

## 31. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner and current Constitutional Authority | Approved | GOV-017 | 2026-07-25 |

This Approval adopts TIM-001 v1.0 into the Foundation Baseline.

It does not activate a Time Profile, authorize a source, authorize a Time Provider, modify a Contract, authorize implementation, or authorize financial use.
