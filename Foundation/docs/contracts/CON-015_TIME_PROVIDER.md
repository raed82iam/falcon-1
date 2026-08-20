# CON-015 — Time Provider Contract

**Identifier:** CON-015  
**Version:** 1.0  
**Status:** Approved  
**Effective Date:** 2026-07-25  
**Approval Record:** GOV-027  
**Owner:** Falcon Contract Authority  
**Governing Authority:** Falcon Vision; Falcon Constitution; TIM-001; FCE-001; SEC-001; SEC-002; ADR-I006; ADR-I008; AMD-003; AMD-003-IR-001  
**Admission Authority:** CON-000  
**Implementation Authority:** Not Granted

## 1. Purpose

This Contract defines the sole governed boundary through which Falcon runtimes obtain time observations, monotonic observations, Clock Quality, uncertainty, capabilities, and verification evidence.

Components SHALL NOT obtain operational time directly from an operating system, runtime, database, network, hypervisor, device, cloud service, or vendor API.

A time observation is evidence with declared quality and uncertainty. It is not absolute truth.

## 2. Participants

- **Requester:** the governed participant requesting a Time Observation.
- **Time Provider:** the participant producing the governed observation.
- **Clock Source:** the identified source or source set observed by the Provider.
- **Clock Verification Authority:** the authority evaluating Clock Quality for the active Deployment Profile.
- **Runtime Authority:** the participant establishing Runtime Epoch identity and continuity.
- **Security Authority:** the authority governing time-security profiles where applicable.
- **Evidence Authority:** the participant preserving observation and verification evidence.

## 3. Provider Profile and State

Every Time Provider instance SHALL identify:

- Provider identity;
- Time Provider Profile ID and version;
- active Deployment Profile;
- Provider lifecycle state;
- Clock Source IDs;
- Runtime Epoch ID;
- capabilities;
- precision and resolution;
- uncertainty model;
- verification method and evidence;
- last verification;
- verification age;
- active configuration identity;
- applicable policy versions; and
- health and trust state.

Provider lifecycle states are:

- `CANDIDATE`;
- `ACTIVE`;
- `RESTRICTED`;
- `SUSPENDED`;
- `RETIRED`; and
- `FORBIDDEN`.

Only an applicable `ACTIVE` Provider may issue operational Time Observations. `RESTRICTED` may issue observations only with its explicitly preserved quality ceiling and scope.

## 4. Time Observation Request

A Request SHALL contain:

- request ID;
- authenticated requester identity;
- requested observation purpose;
- required Time Type;
- minimum Clock Quality;
- maximum acceptable uncertainty;
- maximum verification age;
- required capabilities;
- environment and Deployment Profile;
- requested evidence level;
- correlation and causation;
- request time context where available; and
- authority context.

The Requester SHALL NOT select a raw platform clock or reinterpret Provider quality.

## 5. Time Observation

A Time Observation SHALL contain:

- observation ID;
- request ID;
- `OBSERVED` or `REJECTED`;
- Observed UTC;
- Monotonic Observation, when supported;
- Clock Source ID or governed source-set identity;
- Runtime Epoch ID;
- Clock Quality;
- Maximum Uncertainty;
- Earliest Possible Time;
- Latest Possible Time;
- precision and resolution;
- Last Verification;
- Verification Age;
- Clock Capabilities;
- Deployment Profile;
- Provider identity and Profile;
- evidence reference;
- applicable constraints and reason;
- canonical representation; and
- integrity protection.

Observed UTC and monotonic values SHALL remain semantically distinct.

## 6. Canonical UTC

Operational UTC representation SHALL:

- use UTC only;
- conform to FCE-001 canonical timestamp encoding;
- use the approved precision;
- apply the approved rounding or truncation rule;
- reject noncanonical offsets and ambiguous forms;
- handle leap-second input according to TIM-001;
- preserve source precision without inventing accuracy; and
- produce the same canonical value across conforming platforms.

Storage or platform representation SHALL NOT redefine Falcon time semantics.

## 7. Clock Quality

Clock Quality SHALL use the governed states and rules in TIM-001.

`VERIFIED` requires approved verification evidence appropriate to the active Deployment Profile.

No single source establishes `VERIFIED` merely by existing unless the active Deployment Profile explicitly permits it.

Clock Quality SHALL be downgraded when:

- verification expires;
- source identity changes materially;
- uncertainty exceeds policy;
- drift or discontinuity exceeds policy;
- Runtime Epoch continuity breaks;
- required evidence is missing or invalid;
- sources conflict materially; or
- security or integrity becomes uncertain.

Quality SHALL NOT be upgraded by the Requester.

## 8. Uncertainty

Temporal decisions SHALL be evaluated against the uncertainty interval, not the observed timestamp alone.

For observation `T` with maximum uncertainty `U`:

- earliest possible time is `T - U`;
- latest possible time is `T + U`; and
- a boundary that overlaps this interval is uncertain.

Uncertain temporal validity SHALL produce the conservative result required by governing policy. It SHALL NOT produce unrestricted authority.

## 9. Monotonic Time and Runtime Epoch

Monotonic observations are comparable only when:

- Runtime Epoch ID is identical;
- approved clock source and capability continuity is established;
- scale and unit are compatible; and
- no discontinuity invalidates comparison.

Runtime epochs SHALL NOT be assumed to survive restart, process replacement, container recreation, migration, failover, provider replacement, or platform transition unless the active Deployment Profile explicitly proves continuity.

Monotonic time SHALL NOT be converted into UTC without an Approved governed mapping and uncertainty.

## 10. Temporal Decision Support

A requester evaluating expiry, freshness, timeout, ordering, replay windows, validity, or retention SHALL declare:

- decision purpose;
- required quality;
- acceptable uncertainty;
- governing boundary;
- handling of overlap and unknown regions; and
- evidence obligations.

Arrival order, identifier order, persistence order, and UTC order SHALL NOT be assumed equivalent.

## 11. Bootstrap Boundary

Before Time Provider Activation, external observations MAY support preparation and candidate verification only when marked `BOOTSTRAP_EXTERNAL`.

Each bootstrap observation SHALL preserve:

- source;
- observing environment;
- resolution;
- known uncertainty;
- external continuity boundary;
- wall and monotonic values separately;
- provenance; and
- the explicit statement that Falcon Clock Quality is not established.

Bootstrap time SHALL NOT be reclassified as Falcon `VERIFIED` time.

A `CANDIDATE` Provider may produce observations only as the subject under authorized verification. It SHALL NOT establish its own Activation, validate production material, or provide operational time.

## 12. Preconditions

Before issuing an operational observation:

- requester identity and permission SHALL be verified;
- the Provider Profile and Deployment Profile SHALL be active and applicable;
- Clock Source identity SHALL be known;
- Runtime Epoch identity SHALL be valid;
- required capabilities SHALL be available;
- current quality and uncertainty SHALL satisfy the request;
- verification evidence SHALL be within its validity conditions;
- integrity and security conditions SHALL hold; and
- observation evidence SHALL be preservable where required.

## 13. Postconditions

After success:

- the observation is canonical, attributable, scoped, and integrity-verifiable;
- quality and uncertainty are explicit;
- UTC and monotonic meanings remain separate;
- Runtime Epoch limits are visible;
- the requester can make a conservative governed temporal decision; and
- the observation grants no authority by itself.

## 14. Invariants

- Operational time SHALL be obtained only through this Contract.
- Observed time SHALL always carry quality and uncertainty.
- Precision SHALL NOT be represented as accuracy.
- UTC and monotonic time SHALL remain distinct.
- Monotonic values SHALL NOT cross Runtime Epoch boundaries without proven continuity.
- `BOOTSTRAP_EXTERNAL` time SHALL NOT become Falcon `VERIFIED` time.
- Clock Quality SHALL be scoped to the active Deployment Profile.
- Time order SHALL NOT imply causality or arrival order.
- Material uncertainty SHALL restrict decisions.
- A Time Provider SHALL NOT decide the business or authority outcome that consumes its observation.

## 15. Rejection and Failure

An observation SHALL be rejected or downgraded when:

- requester identity or permission is unverified;
- Provider or Deployment Profile is inapplicable;
- Clock Source or Runtime Epoch identity is missing;
- quality, uncertainty, verification age, or capabilities fail the request;
- source conflict or discontinuity is unresolved;
- canonical representation cannot be produced;
- integrity or provenance is invalid;
- a bootstrap observation is presented as operational;
- a candidate attempts self-validation; or
- required evidence cannot be preserved.

Failure SHALL NOT cause silent fallback to direct platform time. The requester SHALL receive a bounded reason and the safest available declared quality, or rejection.

## 16. Compatibility

- Canonical timestamp representation SHALL be governed solely by FCE-001.
- Time types, qualities, capabilities, uncertainty rules, and source profiles SHALL be governed solely by TIM-001.
- Consumers SHALL NOT depend on a programming language, operating system, database, hypervisor, device, cloud, network protocol, or vendor.
- Unknown mandatory fields, unsupported profiles, or unrecognized quality states SHALL cause rejection or restriction.
- A Clock Source or Provider may be replaced behind this Contract without changing consuming components.
- Historical observations SHALL retain their original semantics and SHALL NOT be reinterpreted under later profiles.

## 17. Evidence

Evidence SHALL preserve:

- request and observation;
- requester, Provider, and source identity;
- Runtime Epoch;
- profiles and configuration;
- Observed UTC and monotonic values;
- quality and uncertainty;
- precision and resolution;
- verification method, result, age, and provenance;
- source conflicts, drift, discontinuities, and downgrades;
- bootstrap or candidate classification;
- rejection and fallback prevention;
- canonical representation and digest; and
- responsible authorities.

Time evidence is governed as Trust Objects under SEC-002.

## 18. Security

- Requests, observations, verification evidence, and source identity SHALL receive integrity and provenance protection.
- Replay, delay, rollback, source substitution, time shifting, and verification-age manipulation SHALL be detected according to profile.
- Security-validity decisions SHALL use the uncertainty interval and required Clock Quality.
- A compromised or uncertain source SHALL trigger downgrade, restriction, and protective notification proportionate to consequence.
- Time availability SHALL NOT justify bypassing identity, integrity, authority, or evidence requirements.
- Synthetic time is permitted only in explicit isolated test profiles.

## 19. Normative Requirements

- **CON-015-REQ-001:** Every runtime SHALL obtain operational time exclusively through the Falcon Time Provider Contract.
- **CON-015-REQ-002:** Every Time Observation SHALL declare source, Runtime Epoch, Clock Quality, uncertainty, capabilities, and verification evidence.
- **CON-015-REQ-003:** Canonical timestamp representation SHALL conform to FCE-001.
- **CON-015-REQ-004:** Time semantics and quality SHALL conform to TIM-001 and the active Deployment Profile.
- **CON-015-REQ-005:** `VERIFIED` quality SHALL require applicable approved evidence.
- **CON-015-REQ-006:** Temporal decisions SHALL evaluate the full uncertainty interval.
- **CON-015-REQ-007:** UTC and monotonic observations SHALL remain semantically distinct.
- **CON-015-REQ-008:** Monotonic comparison SHALL remain within a proven Runtime Epoch and source continuity boundary.
- **CON-015-REQ-009:** Runtime continuity SHALL NOT be assumed across restart, migration, failover, or platform transition.
- **CON-015-REQ-010:** `BOOTSTRAP_EXTERNAL` observations SHALL NOT be reclassified as Falcon `VERIFIED` time.
- **CON-015-REQ-011:** Candidate observations SHALL remain non-operational and SHALL NOT establish candidate Activation.
- **CON-015-REQ-012:** Material source conflict, uncertainty, discontinuity, or stale verification SHALL downgrade or reject the observation.
- **CON-015-REQ-013:** Failure SHALL NOT cause silent fallback to direct platform time.
- **CON-015-REQ-014:** Time Provider replacement SHALL remain transparent to consuming components.
- **CON-015-REQ-015:** A Time Observation SHALL NOT establish causality, arrival order, authority, or business truth.
- **CON-015-REQ-016:** Observation and verification evidence SHALL be immutable, attributable, and reconstructable.

## 20. Acceptance Examples

Acceptance requires verified examples showing:

- canonical equivalent UTC across supported platforms;
- approved precision and leap-second handling;
- verified source under an applicable Deployment Profile;
- quality downgrade after verification expiry;
- conservative evaluation at an uncertainty-overlapping boundary;
- distinction between precision and accuracy;
- valid monotonic comparison within one Runtime Epoch;
- rejection across incompatible epochs;
- discontinuity and source-conflict detection;
- rejection of unsupported capability or excessive uncertainty;
- isolation of synthetic and candidate time;
- preservation of `BOOTSTRAP_EXTERNAL` observations without quality upgrade;
- failure without direct-clock fallback;
- inability of time order to establish causality; and
- replaceability of the source behind the Contract.

## 21. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner and current Constitutional Authority | Approved | GOV-027 | 2026-07-25 |

This Approval admits CON-015 as a governed Foundation Contract. It does not activate a Time Provider, Clock Source, or Profile; establish `VERIFIED` time; authorize implementation; or authorize financial activity.
