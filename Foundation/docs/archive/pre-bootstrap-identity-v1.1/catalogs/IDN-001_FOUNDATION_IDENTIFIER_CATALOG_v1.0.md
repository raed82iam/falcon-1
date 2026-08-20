# IDN-001 — Foundation Identifier Catalog

**Version:** 1.0  
**Status:** Approved  
**Effective Date:** 2026-07-25  
**Approval Record:** GOV-016  
**Owner:** Falcon Specification Authority  
**Governing Authority:** Falcon Vision; Falcon Constitution; SEC-001; SEC-002; FCE-001; ADR-I006  
**Affected Domains:** All  
**Implementation Authority:** Not Granted
**Superseded By:** IDN-001 v1.1 under GOV-034

## 1. Purpose

IDN-001 is the canonical catalog of Falcon identifier classes, schemes, issuance profiles, scopes, privacy rules, continuity rules, collision behavior, and lifecycle obligations.

It ensures that identifiers remain typed, replaceable, collision-controlled, and distinct from identity, authority, secrets, and time evidence.

> **An identifier refers to a subject. It does not prove what the subject is, whether it is trusted, or what it may do.**

## 2. Catalog Authority

IDN-001 owns:

- Identifier Class IDs;
- Identifier Profile IDs;
- permitted identifier schemes by class;
- issuer category;
- uniqueness scope;
- continuity behavior;
- privacy classification;
- exposure rule;
- lifecycle rule;
- collision rule; and
- reserved identifier values.

IDN-001 does not own:

- canonical UUID bytes or timestamp text, owned by FCE-001;
- logical-subject creation;
- identity binding;
- admission;
- trust;
- authority;
- authentication;
- artifact digest algorithms;
- clock quality;
- implementation; or
- financial permission.

## 3. Identity and Identifier Separation

An identifier is a value used to refer to a logical subject.

Identity is the governed binding between:

- identifier;
- logical subject;
- immutable attributes;
- provenance;
- issuer;
- scope;
- integrity;
- lifecycle;
- validity; and
- revocation state where applicable.

The Identifier Provider issues identifier values. It SHALL NOT:

- create the logical subject;
- establish identity ownership;
- declare admission;
- declare trust;
- authenticate a subject;
- grant authority;
- infer authority from possession;
- alter immutable identity attributes; or
- resolve identity collision by itself.

The accountable subject owner establishes and preserves the identity binding.

## 4. Identifier Kinds

IDN-001 distinguishes:

### 4.1 Operational Identifier

Issued for a runtime or governed operational subject such as a message, event, decision, execution, or evidence record.

### 4.2 Registered Semantic Identifier

Assigned through a governed registry to a stable semantic subject such as a Specification, Contract, schema, domain, profile, configuration key, or component type.

### 4.3 Artifact Identity

A composite identity including canonical artifact name, version, cryptographic digest, signature where required, provenance, and applicable manifest binding.

### 4.4 Human, Instance, and Workload Identity

A governed identity established through issuer, credential or equivalent proof, environment, artifact, scope, validity, and revocation state.

A UUID alone does not establish this identity.

## 5. Identifier Lifecycle

Identifier Profiles use:

- `DRAFT`;
- `APPROVED`;
- `ACTIVE`;
- `DEPRECATED`;
- `RETIRED`; and
- `FORBIDDEN`.

Identifier Class definitions use:

- `DRAFT`;
- `APPROVED`;
- `ACTIVE`;
- `DEPRECATED`;
- `RETIRED`; and
- `RESERVED`.

Only an `ACTIVE` Identifier Profile may issue new Approved operational identifiers.

Approval of this Catalog does not activate an implementation.

## 6. Identifier Profile Registry

| Profile ID | Version | Scheme | Lifecycle | Permitted boundary |
|---|---:|---|---|---|
| `FALCON-ID-UUID7-INTERNAL-1` | `1` | UUIDv7 under RFC 9562 | `APPROVED` | Internal trusted Foundation boundary only |

No external-exposure Identifier Profile is Approved in IDN-001 v1.0.

`FALCON-ID-UUID7-INTERNAL-1` SHALL NOT be used outside its permitted boundary until the active Deployment Profile approves exposure after the privacy assessment in section 15.

## 7. UUIDv7 Internal Profile

`FALCON-ID-UUID7-INTERNAL-1` SHALL:

- use UUID version 7 under RFC 9562;
- use the Falcon Identifier Provider;
- obtain time through the Falcon Time Provider;
- obtain random material through an Approved cryptographic random provider;
- use FCE-001 canonical representation;
- set the RFC variant and version correctly;
- reject nil and reserved values;
- expose the profile and scheme used;
- preserve class and scope;
- produce issuance evidence where required; and
- apply collision detection and reconciliation policy.

The timestamp embedded in UUIDv7 is approximate generation information.

It SHALL NOT establish:

- authoritative occurrence time;
- creation truth;
- causality;
- freshness;
- expiry;
- authority;
- admission;
- guaranteed generation order;
- arrival order;
- execution order;
- persistence order; or
- Clock Quality.

## 8. Typed Textual Model

Where a Contract requires a typed textual identifier, the canonical semantic model is:

```text
urn:falcon:id:<class>:<uuid>
```

The `<class>` value SHALL be the exact Identifier Class token from this Catalog.

The `<uuid>` value SHALL use the FCE-001 lowercase hyphenated UUID representation.

The prefix `urn:falcon:id:` is lowercase and immutable.

Typed textual parsing SHALL reject:

- unknown class;
- uppercase or aliased prefix;
- wrong scheme;
- unsupported UUID version;
- noncanonical UUID text;
- nil or reserved UUID;
- surrounding whitespace;
- additional segments;
- class substitution; and
- trailing data.

FCE-001 remains the authority for canonical UUID text and bytes.

## 9. Operational Identifier Class Registry

All classes in this section have:

- Kind: Operational Identifier;
- Initial Profile: `FALCON-ID-UUID7-INTERNAL-1`;
- Initial Lifecycle: `APPROVED`;
- External exposure: Prohibited unless section 15 is satisfied;
- Reassignment: Prohibited; and
- Canonical representation: FCE-001.

### 9.1 Interaction and Causality

| Class token | Meaning | Issuer request owner | Continuity rule |
|---|---|---|---|
| `message` | One logical FIL message | Message producer | Retry of the same logical message preserves Message ID when the Contract requires |
| `attempt` | One delivery attempt for a logical message | Delivery authority | Every delivery attempt receives a new ID |
| `correlation` | One governed interaction or workflow correlation scope | Workflow initiator | Preserved across members of the same declared correlation |
| `request` | One material governed request | Request initiator | Same logical idempotent request preserves ID where Contract requires |
| `operation` | One logical operation across retries and attempts | Operation owner | Retry preserves Operation ID; distinct execution attempt receives another class |
| `execution` | One execution attempt of a logical operation | Execution authority | Every distinct execution attempt receives a new ID |

`Causation ID` is a reference role, not an Identifier Class. It SHALL carry the existing typed identifier of the directly causing message, event, decision, operation, or other class permitted by the governing Contract. Recording causation SHALL NOT reclassify or replace the cause's identity.

### 9.2 Events, Decisions, and Authority

| Class token | Meaning | Issuer request owner | Continuity rule |
|---|---|---|---|
| `event` | One immutable event occurrence record | Event producer | Correction creates a new Event ID linked to original |
| `decision` | One governed decision | Decision authority or evaluator | Re-evaluation creates a new Decision ID |
| `authority-request` | One Authority Engine evaluation request | Requesting actor | Retry preserves ID only under approved idempotency policy |
| `authority-instrument` | One governed assignment, delegation, restriction, or revocation instrument | Competent authority | Amendment or replacement creates a new ID and lineage |
| `delegation` | One bounded authority delegation | Delegating authority | Redelegation receives a distinct ID |
| `jurisdiction` | One governed jurisdiction record | Competent higher authority | Meaning change requires a new ID |
| `restriction` | One protective or authority restriction | Competent protective authority | Revision creates a new ID unless Contract defines versioned continuity |
| `challenge` | One governed challenge case | Challenger or intake authority | Additional evidence remains under same case; new grounds may require a new ID |

### 9.3 Lifecycle, Health, and Awareness

| Class token | Meaning | Issuer request owner | Continuity rule |
|---|---|---|---|
| `transition` | One lifecycle transition decision or occurrence | Lifecycle authority | Every material transition receives a new ID |
| `transition-request` | One requested lifecycle transition | Requesting actor | Retry behavior governed by request idempotency |
| `assessment` | One health, fitness, awareness, or validity assessment | Assessment authority | Re-assessment creates a new ID |
| `observation` | One governed observation | Observation producer | Correction creates a new ID and lineage |
| `runtime-epoch` | One declared runtime epoch | Runtime authority through Identifier Provider | New epoch conditions require a new ID |
| `clock-source` | One governed Clock Source identity instance | Time authority | Material source identity change creates a new ID |

### 9.4 Evidence, Trust, and Evaluation

| Class token | Meaning | Issuer request owner | Continuity rule |
|---|---|---|---|
| `evidence` | One immutable evidence item | Evidence producer | Correction creates a new ID linked to original |
| `record` | One immutable logging or audit record | Record producer | Append-only; never reassigned |
| `claim` | One attributable Trust Claim | Claim issuer | Changed proposition, scope, or evidence creates a new ID |
| `validity-assessment` | One scoped Validity Assessment | Validity evaluator | Re-evaluation creates a new ID |
| `acceptance` | One Acceptance Decision | Accepting authority | Change creates a new ID |
| `evaluation-context` | One immutable Evaluation Context | Context authority | Material context change creates a new ID or governed new version |
| `verification-session` | One verification execution session | Verification pipeline | Retry or rerun creates a new ID |
| `evidence-requirement-set` | One immutable obligation snapshot | Requirements authority | Changed obligation snapshot creates a new ID |
| `evidence-set` | One Root Verification Evidence Set | Evidence aggregation authority | Correction or added evidence creates a new ID linked to prior |
| `promotion-decision` | One artifact promotion decision | Promotion Authority | Every decision receives a new ID |

### 9.5 Recovery and Maintenance

| Class token | Meaning | Issuer request owner | Continuity rule |
|---|---|---|---|
| `recovery` | One governed recovery case | Recovery Authority | Case continuity preserves ID |
| `recovery-execution` | One execution attempt within a recovery case | Recovery executor | Every attempt receives a new ID |
| `maintenance` | One governed maintenance case | Maintenance Authority | Case continuity preserves ID |
| `maintenance-execution` | One maintenance execution attempt | Maintenance executor | Every attempt receives a new ID |
| `change-candidate` | One proposed change identity | Change producer | Material change creates a new ID or version under governing Contract |

### 9.6 Foundation and Runtime Subjects

| Class token | Meaning | Issuer request owner | Continuity rule |
|---|---|---|---|
| `instance` | One Falcon instance identity reference | Identity bootstrap authority | Replacement or changed immutable identity creates a new ID |
| `workload` | One admitted workload identity reference | Workload identity authority | Re-issuance may preserve subject identity only under governing identity Contract |
| `manifest` | One immutable manifest instance | Manifest producer | Change creates a new ID and version lineage |
| `release` | One governed release candidate or release identity | Release Authority | New release receives a new ID |
| `ceremony` | One governed signing or trust ceremony | Ceremony authority | Every ceremony receives a new ID |
| `revocation-source` | One governed revocation source instance | Security Authority | Material source replacement creates a new ID |
| `security-context` | One security-context observation or assertion object | Security context authority | Material context change creates a new ID |
| `configuration-snapshot` | One immutable configuration snapshot | Configuration Authority | Any material change creates a new ID |
| `policy-snapshot` | One immutable policy snapshot | Policy Authority | Any material change creates a new ID |
| `artifact` | One artifact occurrence reference used with full Artifact Identity | Artifact producer | UUID alone never establishes Artifact Identity |

## 10. Registered Semantic Identifier Classes

The following use governed registry tokens, not arbitrary operational UUIDs:

| Semantic class | Example form | Governing owner |
|---|---|---|
| Vision document | `VISION-001` or approved canonical title/version | Constitutional governance |
| Constitution | approved constitutional identifier and version | Constitutional governance |
| Governance document | `GOV-...` | Governance authority |
| Specification | `DOMAIN-NNN` | SPEC-000 |
| Standard | `STD-NNN` | STD-000 |
| Contract | `CON-NNN` | CON-000 |
| ADR | `ADR-...` | ADR-000 |
| Verification Plan | `VPL-NNN` | Verification governance |
| Foundation definition | `FDN-NNN` | Foundation governance |
| Schema | governed Schema ID and version | Schema registry |
| FIL type | governed Type ID and version | FIL catalog |
| Cryptographic Domain | `falcon/...` Domain ID | CRY-001 |
| Cryptographic Purpose | `falcon/purpose/...` | CRY-001 |
| Crypto Profile | governed Profile ID and version | CRY-001 |
| FCE schema | `falcon/...` Schema ID and version | FCE schema registry |
| Configuration key | governed configuration token | Configuration Authority |
| Component type | governed component-type token | Component registry |

A semantic identifier SHALL NOT be replaced by an operational UUID merely for convenience.

## 11. Artifact Identity

Artifact Identity SHALL include:

- canonical artifact name;
- artifact type;
- version;
- cryptographic digest algorithm;
- digest value;
- signature identity where required;
- provenance;
- build or production context;
- applicable manifest;
- lifecycle;
- supersession; and
- Trust Object evidence.

An `artifact` UUID may refer to an Artifact Identity record. It SHALL NOT replace the complete Artifact Identity.

Two artifacts sharing a name or UUID but differing in digest are not the same immutable artifact.

## 12. Identifier Provider Contract Obligations

Components SHALL NOT generate operational identifiers directly.

All operational identifiers SHALL be issued through the Falcon Identifier Provider Contract.

The Provider SHALL:

- accept a declared Identifier Class;
- accept a declared Identifier Profile;
- verify requester permission to request that class;
- verify the active Deployment Profile;
- use the Falcon Time Provider where the scheme requires time;
- use the approved randomness provider;
- generate scheme-valid values;
- reject nil and reserved values;
- produce FCE-001 canonical form;
- bind class and scope;
- expose scheme and profile;
- apply privacy rules;
- enforce environment separation;
- preserve collision controls;
- issue evidence where required; and
- reject unknown class, profile, scope, environment, or scheme.

The Provider SHALL NOT:

- create the subject;
- decide identity continuity by itself;
- assign ownership;
- admit a subject;
- grant trust;
- grant authority;
- expose the UUIDv7 timestamp as authoritative time;
- allow a caller to choose raw provider-specific parameters;
- return platform-specific UUID bytes; or
- silently switch schemes.

## 13. Canonical Representation Boundary

FCE-001 exclusively governs:

- UUID text;
- UUID bytes;
- typed identifier use in FCE records;
- canonical timestamp representation; and
- Runtime Epoch ID representation.

IDN-001 defines classes, profiles, values, and semantics.

IDN-001 SHALL NOT redefine:

- byte order;
- hexadecimal case;
- hyphen placement;
- UTF-8 rules;
- FCE record framing; or
- timestamp encoding.

## 14. Continuity and Collision

### 14.1 Identity Continuity

Repeated reference to an existing identifier for the same immutable logical subject is identity continuity.

Continuity SHALL preserve:

- subject;
- identifier class;
- immutable attributes;
- issuer;
- scope;
- provenance;
- lifecycle; and
- governing Contract.

### 14.2 Identity Collision

Use or issuance of the same identifier for:

- a different logical subject;
- a different identifier class;
- conflicting immutable attributes;
- an incompatible issuer or scope; or
- an incompatible Artifact Identity

is an identity collision.

Collision SHALL cause:

- rejection;
- containment;
- evidence preservation;
- affected-authority restriction;
- Health Monitoring notification;
- Self-Awareness update;
- Guardian notification where material harm is possible; and
- governed reconciliation.

Collision SHALL NOT be resolved by:

- overwrite;
- last-write-wins;
- silently selecting one subject;
- merging incompatible subjects;
- relabeling the class;
- deleting evidence; or
- issuing a new ID that conceals the original conflict.

### 14.3 Retry

Retry of one logical operation or message SHALL preserve its logical ID where the governing Contract requires idempotent continuity.

Each distinct attempt SHALL receive a distinct `attempt`, `recovery-execution`, `maintenance-execution`, or other applicable execution ID.

Continuity is not collision.

## 15. Privacy and Boundary Exposure

UUIDv7 discloses approximate generation time.

Before an identifier crosses an untrusted or public boundary, the active Deployment Profile SHALL assess:

- approximate creation-time disclosure;
- activity-volume inference;
- correlation across boundaries;
- predictability;
- enumeration;
- retention;
- public exposure;
- subject sensitivity;
- linkage to other records; and
- compatibility.

If disclosure is unacceptable:

- the internal UUIDv7 SHALL NOT be exposed;
- an alternative Approved Identifier Profile is required;
- the external identifier SHALL remain typed and collision-controlled;
- mapping SHALL be governed and protected;
- internal authority SHALL NOT transfer through the mapping; and
- provenance and reconciliation SHALL be preserved.

IDN-001 v1.0 approves no alternative external profile. Therefore external exposure is denied by default.

## 16. Runtime Epoch Identifier Policy

`runtime-epoch` uses the internal UUIDv7 profile where the Deployment Profile permits.

The embedded UUID timestamp is not epoch start evidence.

A new Runtime Epoch ID is required upon:

- restart;
- process replacement;
- container recreation;
- platform transition;
- provider replacement;
- virtual-machine migration;
- live migration;
- failover; or
- any event that breaks proven monotonic continuity,

unless the active Deployment Profile explicitly proves continuity.

Runtime Epoch IDs from different epochs SHALL NOT make monotonic values comparable.

## 17. Reserved and Prohibited Values

The following are prohibited for operational issuance:

- nil UUID;
- UUID with wrong RFC variant;
- UUID with unsupported version;
- all-zero or Catalog-reserved sentinel;
- caller-supplied arbitrary value presented as newly issued;
- value already bound to a different subject;
- wrong-class identifier;
- noncanonical protected representation;
- value from an unapproved Identifier Profile;
- value generated outside the Identifier Provider; and
- value whose required issuance evidence is invalid.

No special UUID SHALL mean:

- unknown;
- not applicable;
- system;
- anonymous;
- wildcard;
- all subjects;
- default authority; or
- unlimited scope.

Those meanings require explicit schema fields or governed semantic tokens.

## 18. Identifier Evidence

Material issuance evidence SHALL identify:

- identifier;
- class;
- profile and version;
- scheme;
- requester;
- Provider identity;
- environment;
- scope;
- issuance Time Observation;
- randomness-provider profile;
- privacy decision where applicable;
- subject-binding owner;
- outcome;
- rejection reason;
- correlation;
- integrity identity; and
- Trust Object lineage.

Issuance evidence proves the issuance event under its scope. It does not prove the subject's claims, admission, trust, or authority.

## 19. Provider and Collision Failure

Provider failure, unsupported profile, unavailable trusted inputs, invalid time input, random-source failure, noncanonical output, privacy-policy conflict, or detected collision SHALL:

- prevent fabricated success;
- reject affected issuance;
- preserve failure evidence;
- notify Health Monitoring;
- update Self-Awareness and Fitness to Operate;
- deny identity-dependent authority where required conditions are unproven;
- invoke consequence-appropriate Guardian restriction;
- isolate unaffected operation only where independent fitness is proven; and
- enter governed reconciliation or recovery.

Provider return does not automatically restore trust or authority.

## 20. Profile Evolution

A new Identifier Profile requires:

- Profile ID and version;
- scheme;
- eligible classes;
- uniqueness model;
- collision analysis;
- privacy analysis;
- canonical representation;
- provider inputs;
- environment scope;
- exposure scope;
- compatibility;
- migration;
- mapping rule where applicable;
- failure behavior;
- test vectors;
- lifecycle;
- authority; and
- Approval.

UUIDv8 or any future scheme SHALL NOT be used merely because it is available.

Changing the provider implementation without changing governed output does not require changing logical identity meaning.

Changing scheme, privacy behavior, collision model, or canonical interpretation requires governed profile change.

## 21. Catalog Requirements

- **IDN-001-REQ-001:** Every operational identifier SHALL have a declared Identifier Class and Profile.
- **IDN-001-REQ-002:** Components SHALL obtain operational identifiers only through the Falcon Identifier Provider Contract.
- **IDN-001-REQ-003:** Components SHALL NOT call platform, database, random, or language identifier generation directly.
- **IDN-001-REQ-004:** Identifier Provider SHALL NOT create subjects, establish trust, or grant authority.
- **IDN-001-REQ-005:** `FALCON-ID-UUID7-INTERNAL-1` SHALL use UUIDv7 under RFC 9562.
- **IDN-001-REQ-006:** UUIDv7 embedded time SHALL NOT establish authoritative time, causality, freshness, expiry, authority, or guaranteed ordering.
- **IDN-001-REQ-007:** Operational identifiers SHALL use the class definitions and scopes in section 9.
- **IDN-001-REQ-008:** An identifier valid for one class SHALL NOT be accepted as another class.
- **IDN-001-REQ-009:** FCE-001 SHALL remain the sole authority for canonical identifier text and bytes.
- **IDN-001-REQ-010:** Registered semantic subjects SHALL use stable registry identifiers rather than arbitrary operational UUIDs.
- **IDN-001-REQ-011:** UUID possession alone SHALL NOT establish Artifact Identity.
- **IDN-001-REQ-012:** Human, instance, and workload identity SHALL require governed binding beyond a UUID.
- **IDN-001-REQ-013:** Retry SHALL preserve the logical identifier and create a distinct attempt identifier where the governing Contract requires.
- **IDN-001-REQ-014:** Repeated reference to the same immutable subject SHALL be treated as continuity, not collision.
- **IDN-001-REQ-015:** Reuse for a different subject, class, or conflicting immutable attributes SHALL be treated as collision.
- **IDN-001-REQ-016:** Collision SHALL cause rejection, containment, evidence, and reconciliation.
- **IDN-001-REQ-017:** Collision SHALL NOT be hidden by overwrite, silent selection, merge, relabeling, or replacement issuance.
- **IDN-001-REQ-018:** Identifiers SHALL NOT be reassigned to a different subject.
- **IDN-001-REQ-019:** Nil, wrong-variant, unsupported-version, wrong-class, reserved, and noncanonical identifiers SHALL be rejected.
- **IDN-001-REQ-020:** Identifiers SHALL NOT be treated as secrets, passwords, bearer capabilities, authorization tokens, or integrity proof.
- **IDN-001-REQ-021:** External UUIDv7 exposure SHALL require an Approved privacy assessment and Deployment Profile permission.
- **IDN-001-REQ-022:** External exposure SHALL be denied when timestamp disclosure is unacceptable and no alternative Approved profile exists.
- **IDN-001-REQ-023:** IDN-001 v1.0 SHALL approve no external alternative identifier profile.
- **IDN-001-REQ-024:** Runtime Epoch ID SHALL change when monotonic continuity is not explicitly proven.
- **IDN-001-REQ-025:** Runtime Epoch IDs SHALL NOT make monotonic observations comparable across epochs.
- **IDN-001-REQ-026:** Material issuance SHALL produce attributable evidence without claiming subject truth or authority.
- **IDN-001-REQ-027:** Provider or collision failure SHALL restrict affected authority and SHALL NOT fabricate success.
- **IDN-001-REQ-028:** Provider return SHALL NOT automatically restore trust or authority.
- **IDN-001-REQ-029:** New schemes and profiles SHALL require explicit governance, compatibility, privacy, migration, and test evidence.
- **IDN-001-REQ-030:** Catalog Approval SHALL NOT activate a Provider or authorize implementation.

## 22. Conformance Evidence

Conformance requires evidence that:

- direct identifier generation by components is rejected or structurally unavailable;
- every issued identifier resolves to a known class and profile;
- UUIDv7 variant and version are correct;
- UUID text and bytes conform to FCE-001 across Windows, Linux, and PostgreSQL;
- nil and reserved values fail;
- wrong-class substitution fails;
- UUIDv7 time is never used as authoritative time or order;
- retry preserves logical identity while attempts remain distinct;
- semantic registry IDs are not replaced by UUIDs;
- UUID alone cannot satisfy Artifact Identity;
- identity continuity is distinguished from collision;
- collision cannot overwrite or hide conflicting subjects;
- external exposure is denied without privacy approval;
- no alternative external profile is silently selected;
- Runtime Epoch changes when continuity is not proven;
- provider failure cannot return fabricated success;
- issuance evidence remains attributable and immutable; and
- Provider recovery does not automatically restore authority.

## 23. Required Before Activation

The internal UUIDv7 profile SHALL NOT become operationally `ACTIVE` until:

1. IDN-001 is Approved;
2. FCE-001 is Approved and registered;
3. TIM-001 is Approved;
4. the Identifier Provider Contract is Approved;
5. the Time Provider and randomness dependencies are defined;
6. all Identifier Class mappings are verified against affected Contracts;
7. privacy boundary rules are present in ENV-001;
8. positive, negative, collision, retry, and cross-platform vectors pass;
9. PostgreSQL round-trip byte order is verified;
10. failure and recovery paths are verified; and
11. explicit implementation authority exists.

## 24. Foundational Rules

> **Identity is a governed binding. An identifier is only one value within that binding.**

> **Same subject and same immutable identity means continuity. Different subject or conflicting immutable identity means collision.**

> **Retry preserves logical identity; execution attempts remain distinct.**

> **UUIDv7 carries approximate generation time. It does not carry authoritative time or causality.**

> **Possession of an identifier grants nothing.**

## 25. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner and current Constitutional Authority | Approved | GOV-016 | 2026-07-25 |

This Approval adopts IDN-001 v1.0 into the Foundation Baseline.

It does not activate an Identifier Profile, authorize a Provider, modify a Contract, authorize implementation, or authorize external exposure.
