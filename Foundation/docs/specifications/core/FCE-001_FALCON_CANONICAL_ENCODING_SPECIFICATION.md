# FCE-001 — Falcon Canonical Encoding Specification

**Identifier:** FCE-001  
**Version:** 1.0  
**Status:** Approved  
**Effective Date:** 2026-07-25  
**Approval Record:** GOV-014  
**Owner:** Falcon Specification Authority  
**Governing Authority:** Falcon Vision; Falcon Constitution; SEC-001; SEC-002; GOV-SEC-001; ADR-I005; ADR-I006  
**Affected Domains:** All  
**Registration Status:** Registered in SPEC-000  
**Implementation Authority:** Not Granted

## 1. Purpose

FCE-001 defines the sole canonical representation for governed Falcon values whose identity, comparison, integrity, derivation, signature, persistence, evidence, or cross-platform interpretation depends on exact bytes or exact text.

It ensures that the same governed meaning produces one representation and that one canonical representation has one governed interpretation.

> **Same meaning, same canonical bytes. Different meaning, different canonical bytes.**

## 2. Authority

FCE-001 is the sole encoding authority for:

- Cryptographic Domain Context;
- operational identifiers;
- canonical timestamps;
- Runtime Epoch IDs;
- protected or signed Time Observations; and
- future governed values explicitly assigned to FCE-001.

IDN-001 defines governed identifier classes, schemes, values, scopes, and profiles.

TIM-001 defines governed time semantics, quality, capabilities, uncertainty, and thresholds.

CRY-001 defines governed cryptographic domains, purposes, profiles, and lifecycle values.

Those Catalogs define meaning. FCE-001 defines canonical representation. They SHALL NOT redefine canonical bytes or canonical text.

## 3. Scope

FCE-001 governs:

- canonical text;
- canonical binary records;
- primitive value encoding;
- composite field framing;
- field ordering;
- absent, null, and empty distinctions;
- identifier text and bytes;
- timestamp text;
- Runtime Epoch ID representation;
- Cryptographic Domain Context encoding;
- Time Observation encoding;
- schema and format versioning;
- parsing and rejection;
- canonicalization boundaries;
- test vectors;
- cross-platform equivalence; and
- evidence of conformance.

## 4. Non-Scope

FCE-001 does not:

- define the business meaning of a field;
- create an identifier;
- determine identity continuity or collision;
- provide current time;
- determine Clock Quality;
- establish authority;
- define a cryptographic algorithm;
- select a key;
- grant key-use permission;
- classify information;
- replace FIL schemas;
- define a database layout;
- define an in-memory representation;
- authorize lenient external parsing at governed boundaries;
- authorize implementation; or
- make noncanonical input trustworthy by re-encoding it silently.

## 5. Canonicality Principles

### 5.1 One Representation

Each supported governed value SHALL have exactly one canonical representation for a declared FCE profile and schema version.

Alternative equivalent representations SHALL be rejected at governed canonical boundaries.

### 5.2 Meaning Before Encoding

The governing Contract or Catalog SHALL establish field meaning, presence, type, allowed values, and constraints before FCE encoding is applied.

Encoding SHALL NOT repair undefined meaning.

### 5.3 Explicit Version

Every composite canonical record SHALL identify:

- FCE format version; and
- governed schema identity and version.

Version SHALL be part of the canonical bytes.

### 5.4 Unambiguous Framing

Composite values SHALL be typed and length-delimited.

Free-form concatenation is prohibited.

No parser may determine boundaries by guessing, locale, platform type size, terminator scanning, or unordered-field iteration.

### 5.5 Deterministic Ordering

Canonical field order SHALL be defined by this Specification and the governing schema.

Map or object iteration order supplied by a runtime, platform, database, or serializer SHALL NOT determine canonical order.

### 5.6 Platform Independence

Canonical bytes and text SHALL be identical across:

- Windows;
- Linux;
- different processor architectures;
- different programming languages;
- different database systems; and
- compliant providers.

### 5.7 Strict Parsing

Canonical parsing SHALL reject noncanonical input. It SHALL NOT normalize and accept it as if the original input were canonical.

External lenient parsing, where separately authorized, SHALL occur before the canonical trust boundary and SHALL preserve conversion provenance.

### 5.8 No Hidden Defaults

Missing required values SHALL be rejected.

Parsers SHALL NOT supply platform, culture, schema, time-zone, precision, identifier, or security defaults that alter governed meaning.

## 6. Encoding Profiles

FCE-001 v1.0 defines:

- **FCE-TEXT-1:** exact canonical UTF-8 text for values assigned a textual canonical form;
- **FCE-BINARY-1:** deterministic binary framing for governed composite values; and
- **FCE-SEQUENCE-1:** deterministic framing of an ordered sequence used only where a governing schema explicitly permits a sequence.

A governing schema SHALL identify the applicable profile.

Profiles SHALL NOT be mixed within one value except where the schema explicitly embeds an FCE-TEXT-1 value inside FCE-BINARY-1.

## 7. Canonical Text — FCE-TEXT-1

### 7.1 Character Encoding

Canonical text SHALL use UTF-8 without a byte-order mark.

It SHALL reject:

- invalid UTF-8;
- overlong encoding;
- surrogate code points;
- byte-order marks;
- prohibited control characters;
- embedded null;
- replacement characters introduced by decoding failure; and
- noncharacters where the governing schema prohibits them.

### 7.2 Unicode

Where a schema permits non-ASCII text, the value SHALL be Unicode Normalization Form C before UTF-8 encoding.

A parser SHALL verify NFC. It SHALL NOT silently normalize protected canonical input.

Security identifiers, schema identifiers, field names, domain identifiers, purpose identifiers, profile identifiers, environment identifiers, and governed enum tokens SHALL use the narrower character set declared by their Catalog or schema.

### 7.3 Case

Case is significant unless the governing Catalog declares a canonical case.

Case conversion SHALL NOT depend on locale.

Where lowercase or uppercase is required, the permitted character set SHALL make the conversion invariant and unambiguous.

### 7.4 Whitespace

Whitespace is significant unless the schema explicitly permits it.

Canonical parsing SHALL NOT trim, collapse, append, or reinterpret whitespace.

### 7.5 Line Endings

Where multiline canonical text is explicitly allowed, the sole line ending SHALL be LF (`0A`).

CRLF and standalone CR SHALL be rejected at canonical boundaries.

## 8. Canonical Binary — FCE-BINARY-1

### 8.1 Record Structure

Every FCE-BINARY-1 record SHALL be:

```text
Magic
FormatVersion
SchemaIdLength
SchemaId
SchemaVersion
FieldCount
Fields
```

The exact framing is:

| Element | Size | Encoding |
|---|---:|---|
| Magic | 4 octets | ASCII `FCE1` = `46 43 45 31` |
| FormatVersion | 2 octets | unsigned big-endian integer; value `1` |
| SchemaIdLength | 2 octets | unsigned big-endian integer |
| SchemaId | declared length | canonical ASCII identifier |
| SchemaVersion | 4 octets | unsigned big-endian integer |
| FieldCount | 2 octets | unsigned big-endian integer |
| Fields | variable | ordered Field Records |

No octets may precede Magic or follow the final declared field.

### 8.2 Field Record

Each Field Record SHALL be:

```text
FieldId
WireType
ValueLength
Value
```

| Element | Size | Encoding |
|---|---:|---|
| FieldId | 2 octets | unsigned big-endian integer |
| WireType | 1 octet | governed FCE wire type |
| ValueLength | 4 octets | unsigned big-endian integer |
| Value | declared length | canonical value bytes |

Field IDs SHALL appear in strictly increasing numeric order.

Duplicate, zero, undeclared, out-of-order, or prohibited Field IDs SHALL be rejected.

Unknown fields SHALL be rejected unless the exact schema version explicitly defines a governed extension range and its interpretation.

### 8.3 Wire Types

FCE-BINARY-1 defines:

| Code | Name | Canonical value |
|---:|---|---|
| `01` | OCTETS | raw octets of declared length |
| `02` | TEXT | FCE-TEXT-1 bytes |
| `03` | UINT | fixed-width unsigned big-endian integer as declared by schema |
| `04` | SINT | fixed-width two's-complement big-endian integer as declared by schema |
| `05` | BOOL | exactly one octet: `00` false or `01` true |
| `06` | RECORD | one complete FCE-BINARY-1 record |
| `07` | SEQUENCE | one complete FCE-SEQUENCE-1 sequence |
| `08` | IDENTIFIER | canonical identifier bytes declared by IDN-001 and section 13 |
| `09` | TIMESTAMP | exactly 27 ASCII octets under section 14 |

No other WireType is valid in format version 1.

The governing schema SHALL declare the WireType for every Field ID. A correct length with the wrong WireType SHALL be rejected.

### 8.4 Integer Encoding

Field schemas SHALL declare integer width.

Permitted widths in FCE-BINARY-1 are 1, 2, 4, 8, or 16 octets.

Integers SHALL use big-endian network byte order.

Variable-width integer encoding, platform-native integer encoding, floating point, decimal text, and language-specific numeric serialization are prohibited unless a future Approved FCE profile defines them.

### 8.5 Boolean Encoding

False SHALL be `00`.

True SHALL be `01`.

Every other value or length SHALL be rejected.

### 8.6 Absent, Null, and Empty

Absence is represented only by omission of an optional Field Record.

FCE-BINARY-1 defines no universal null value.

A field SHALL NOT encode null unless its governing schema defines an explicit semantic enum or union representation.

An empty TEXT, OCTETS, RECORD, or SEQUENCE value is distinct from absence and is permitted only when the schema explicitly allows zero length.

Required fields SHALL NOT be omitted.

### 8.7 Size Limits

The governing schema SHALL define:

- maximum record size;
- maximum field count;
- maximum field length;
- maximum nesting depth; and
- maximum sequence length.

An absent limit SHALL result in rejection at protected boundaries until a governing limit is approved.

## 9. Ordered Sequences — FCE-SEQUENCE-1

FCE-SEQUENCE-1 SHALL contain:

```text
ElementWireType
ElementCount
Elements
```

| Element | Size | Encoding |
|---|---:|---|
| ElementWireType | 1 octet | one permitted FCE wire type |
| ElementCount | 4 octets | unsigned big-endian integer |
| Element | variable | `ValueLength` as 4-octet unsigned big-endian, then canonical value |

Sequence order is semantically significant.

If semantic order is not significant, the governing schema SHALL define one deterministic sort key before encoding.

A set SHALL NOT be encoded from runtime iteration order.

Duplicate handling SHALL be defined by the schema.

## 10. Schema Identity

Schema IDs SHALL:

- be selected from a governed registry;
- use lowercase ASCII letters, digits, hyphen, period, and slash only;
- begin with `falcon/`;
- be immutable in meaning;
- have an explicit positive integer SchemaVersion; and
- remain historically interpretable.

Schema IDs SHALL NOT be free-form runtime values.

Changing field meaning, type, required presence, comparison semantics, or security interpretation requires a new schema version or new Schema ID according to compatibility.

An existing Schema ID and version SHALL NOT be repurposed.

## 11. Field Identity

Within a Schema ID:

- each Field ID is permanent;
- Field ID `0` is prohibited;
- a removed Field ID remains reserved;
- a Field ID SHALL NOT be reassigned;
- field meaning SHALL NOT change within one schema version;
- aliases SHALL NOT create a second canonical field; and
- field order is numeric, not declaration or source order.

Field names aid documentation but SHALL NOT enter canonical binary bytes unless the schema defines them as values.

## 12. Canonical Value Validation

Canonical encoding SHALL occur only after semantic validation.

Canonical decoding SHALL verify:

- framing;
- version;
- schema identity;
- schema version;
- field order;
- field uniqueness;
- WireType;
- length;
- character encoding;
- value constraints;
- required presence;
- prohibited presence;
- enum membership;
- cross-field invariants; and
- complete input consumption.

Successfully decoding bytes does not establish factual truth, authority, freshness, or acceptance.

## 13. Operational Identifier Encoding

### 13.1 Text Form

UUID text SHALL use:

```text
xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx
```

It SHALL contain:

- exactly 36 ASCII characters;
- lowercase hexadecimal `0`–`9` and `a`–`f`;
- hyphens at positions 9, 14, 19, and 24 using one-based indexing;
- no braces;
- no prefix;
- no surrounding whitespace; and
- no suffix.

Uppercase, compact, braced, base64, URN, and platform-specific forms are noncanonical.

### 13.2 Binary Form

UUID binary form SHALL contain exactly 16 octets in RFC 9562 network byte order.

Platform-specific mixed-endian, database-native, or in-memory byte ordering SHALL NOT become Falcon interchange or protected canonical form.

### 13.3 Validation

Identifier parsing SHALL reject:

- malformed text or byte length;
- wrong variant;
- unsupported version;
- wrong identifier class;
- nil or Catalog-prohibited reserved values;
- noncanonical protected text;
- identifier and immutable-attribute conflict; and
- representation whose bytes change across supported platforms.

### 13.4 Typed Identifier Context

UUID bytes alone do not express identifier class.

Where substitution between identifier classes could cause harm, the canonical composite record SHALL include the governed Identifier Type ID in a distinct field or use a schema dedicated to that identifier class.

An identifier is not a secret, bearer capability, authorization token, or integrity proof.

## 14. Canonical Timestamp Encoding

### 14.1 Form

The canonical timestamp form SHALL be:

```text
YYYY-MM-DDTHH:MM:SS.ffffffZ
```

It contains exactly 27 ASCII octets.

### 14.2 Required Semantics

Canonical timestamps SHALL use:

- UTC only;
- the RFC 3339 date-time model;
- exactly six fractional decimal digits;
- uppercase `T`;
- uppercase `Z`;
- a four-digit year;
- two-digit month, day, hour, minute, and second;
- seconds `00` through `59`; and
- the Gregorian calendar.

### 14.3 Prohibited Forms

Canonical parsing SHALL reject:

- local time;
- named time zone;
- numeric UTC offset;
- lowercase `t` or `z`;
- a space in place of `T`;
- omitted or additional fractional digits;
- comma decimal separator;
- leap-second `:60`;
- culture-specific date;
- surrounding whitespace;
- equivalent alternative form;
- invalid calendar date; and
- trailing data.

### 14.4 Precision Conversion

When a source is finer than one microsecond, conversion SHALL discard the sub-microsecond remainder toward the earlier instant.

It SHALL NOT round into a later instant.

The governing Time Observation SHALL expand Maximum Uncertainty to cover discarded precision.

When a source is coarser than one microsecond, the six positions remain syntactically required, but resolution and Maximum Uncertainty SHALL expose the source limitation.

### 14.5 Leap Seconds

Canonical form SHALL NOT encode `:60`.

Leap-second input requires an Approved conversion policy that records:

- source time scale;
- leap or smear behavior;
- conversion method;
- provenance;
- resulting uncertainty; and
- duplicate or reversal prevention.

Unknown leap behavior SHALL remain explicit and constrain Clock Quality under TIM-001.

## 15. Runtime Epoch ID Encoding

A Runtime Epoch ID SHALL use an identifier class defined by IDN-001.

Its UUID text and binary representation SHALL follow section 13.

Where encoded within a composite record, Runtime Epoch ID SHALL use WireType `IDENTIFIER` and a schema field dedicated to Runtime Epoch identity.

A Runtime Epoch ID SHALL NOT be inferred from:

- process ID;
- machine name;
- container name;
- boot timestamp;
- operating-system boot identifier;
- database session;
- wall-clock value; or
- deployment label.

Those values may be provenance inputs but are not canonical substitutes.

## 16. Cryptographic Domain Context Encoding

### 16.1 Schema

Cryptographic Domain Context SHALL use:

- Schema ID: `falcon/crypto/domain-context`;
- Schema Version: `1`;
- Profile: FCE-BINARY-1.

### 16.2 Fields

| Field ID | Name | WireType | Presence |
|---:|---|---|---|
| `1` | ContextFormatVersion | UINT, 2 octets | Required |
| `2` | FalconIdentity | IDENTIFIER | Required |
| `3` | EnvironmentIdentity | TEXT | Required |
| `4` | InstanceOrSharingScope | TEXT | Required |
| `5` | DomainId | TEXT | Required |
| `6` | PurposeId | TEXT | Required |
| `7` | Direction | TEXT | Required when the domain is directional; otherwise prohibited |
| `8` | ProtectionProfileId | TEXT | Required |
| `9` | ProtectionProfileVersion | UINT, 4 octets | Required |
| `10` | AlgorithmId | TEXT | Required |
| `11` | KeyVersion | UINT, 4 octets | Required |

ContextFormatVersion SHALL equal `1`.

### 16.3 Governed Values

EnvironmentIdentity, InstanceOrSharingScope, DomainId, PurposeId, Direction, ProtectionProfileId, and AlgorithmId SHALL be values from the governing Catalogs.

Components SHALL NOT create, alias, reinterpret, case-convert, concatenate, or normalize those values independently.

### 16.4 Derivation Input

The complete FCE-BINARY-1 record is the canonical Domain Context derivation input.

No field may be omitted because its value is assumed by deployment, provider, caller, key, or configuration.

No free-form string concatenation is permitted.

### 16.5 Validation

Domain Context SHALL be rejected when:

- a required field is missing;
- a prohibited directional field is present;
- a Catalog value is unknown or inactive;
- identity is invalid;
- profile and algorithm are incompatible;
- purpose is not permitted for the domain;
- environment or sharing scope conflicts with the key;
- key version is invalid;
- field order or type is wrong; or
- re-encoding differs from received protected bytes.

## 17. Time Observation Encoding

### 17.1 Schema

Protected or signed Time Observation SHALL use:

- Schema ID: `falcon/time/observation`;
- Schema Version: `1`;
- Profile: FCE-BINARY-1.

### 17.2 Fields

| Field ID | Name | WireType | Presence |
|---:|---|---|---|
| `1` | ObservationId | IDENTIFIER | Required |
| `2` | ObservedUtc | TIMESTAMP | Required |
| `3` | MonotonicObservation | UINT, 8 octets | Required when supported by the Clock Source; otherwise prohibited |
| `4` | ClockSourceId | TEXT | Required |
| `5` | RuntimeEpochId | IDENTIFIER | Required when MonotonicObservation is present; otherwise governed by TIM-001 |
| `6` | ClockQuality | TEXT | Required |
| `7` | MaximumUncertaintyMicroseconds | UINT, 8 octets | Required |
| `8` | ResolutionMicroseconds | UINT, 8 octets | Required |
| `9` | LastVerification | TIMESTAMP | Required for quality states that depend on verification; otherwise governed by TIM-001 |
| `10` | VerificationAgeMicroseconds | UINT, 8 octets | Required when LastVerification is present |
| `11` | ClockCapabilities | SEQUENCE of TEXT | Required |
| `12` | EvidenceReference | IDENTIFIER | Required for protected or signed observations |
| `13` | ObservationFormatVersion | UINT, 2 octets | Required |

ObservationFormatVersion SHALL equal `1`.

### 17.3 Capabilities

ClockCapabilities SHALL use Catalog tokens sorted by their canonical UTF-8 byte sequence, strictly ascending, without duplicates.

### 17.4 Monotonic Scope

MonotonicObservation SHALL be interpreted only with the declared RuntimeEpochId and ClockSourceId.

Canonical encoding SHALL NOT imply comparability across runtime epochs or clock sources.

### 17.5 Uncertainty

MaximumUncertaintyMicroseconds encodes the non-negative inclusive uncertainty bound defined by TIM-001.

FCE-001 encodes the value. It does not determine whether the value is acceptable for a decision.

## 18. Canonical Comparison

Equality for a canonical value SHALL be defined by:

1. the same governing schema identity and version; and
2. identical canonical bytes.

Byte equality across different schema identities or versions SHALL NOT by itself establish semantic equality.

Ordering SHALL NOT be inferred from canonical bytes unless the governing schema explicitly defines bytewise order as semantic order.

UUIDv7 byte order may support time-oriented sorting, but sorting SHALL NOT be interpreted as complete causality, authority, or event order.

## 19. Integrity, Signing, and Derivation Boundaries

Before hashing, signing, MAC calculation, key derivation, or protected equality:

- the semantic value SHALL be validated;
- the exact schema and FCE version SHALL be selected;
- canonical bytes SHALL be produced once;
- the complete bytes SHALL be protected; and
- evidence SHALL identify the profile and schema.

Verification SHALL operate on the received protected canonical bytes.

A verifier SHALL NOT:

- parse leniently;
- normalize;
- reorder;
- discard unknown fields;
- fill missing fields;
- convert case;
- alter precision;
- reinterpret Catalog tokens; or
- reserialize noncanonical input and then declare the original valid.

Where protocol processing requires decoding and reconstruction, the reconstructed canonical bytes SHALL exactly match the protected bytes.

## 20. Persistence and Interchange

Falcon Contracts may store or transport:

- canonical bytes directly;
- canonical text directly; or
- a noncanonical operational representation accompanied by the authoritative canonical value where explicitly permitted.

The governing Contract SHALL identify which representation is authoritative.

Database column types, JSON property order, object layout, platform GUID bytes, serializer defaults, and transport encoding SHALL NOT redefine canonical form.

Persistence retrieval SHALL verify that the recovered canonical representation remains identical to the stored integrity identity where required.

## 21. JSON Boundary

JSON is not the universal FCE canonical binary format.

Where a Contract uses JSON:

- the Contract governs JSON structure;
- FIL-001 governs FIL schema;
- FCE-001 governs any embedded canonical identifier, timestamp, or encoded canonical record;
- JSON object property order SHALL NOT be used as a canonical order unless a separately Approved profile defines it;
- insignificant JSON formatting SHALL NOT enter protected FCE meaning; and
- a JSON serializer SHALL NOT be used as a substitute for FCE-BINARY-1.

## 22. Error Model

Canonical processing SHALL distinguish:

- unsupported FCE format version;
- unknown schema;
- unsupported schema version;
- malformed framing;
- invalid length;
- trailing data;
- wrong field order;
- duplicate field;
- unknown or prohibited field;
- wrong WireType;
- invalid UTF-8;
- non-NFC text;
- noncanonical case;
- invalid identifier;
- invalid timestamp;
- unknown Catalog value;
- missing required field;
- prohibited field;
- size-limit violation;
- semantic constraint failure;
- cross-field conflict;
- canonical-byte mismatch; and
- unsupported conversion.

Errors SHALL be explicit and attributable.

Canonical failure SHALL NOT fall back to:

- platform serialization;
- culture-dependent parsing;
- local time;
- unordered map encoding;
- plaintext concatenation;
- legacy identifier bytes;
- a different schema version;
- a guessed Catalog value; or
- permissive acceptance.

## 23. Versioning and Evolution

FCE format versions are immutable in meaning.

A new format version is required when framing or primitive canonical representation changes incompatibly.

A new schema version is required when a governed composite type changes:

- required fields;
- field type;
- field meaning;
- allowed presence;
- ordering semantics;
- security interpretation;
- comparison semantics; or
- validation invariants.

Compatibility SHALL be explicit.

Parsers SHALL NOT:

- treat an unknown version as the latest known version;
- silently downgrade;
- reuse removed Field IDs;
- reinterpret old bytes using new meaning; or
- emit multiple canonical encodings for one version.

Historical versions SHALL remain interpretable for retained evidence and recovery as required.

## 24. Canonicalization Authority

Components SHALL NOT implement independent canonicalization rules.

They SHALL use the Falcon Canonical Encoding Contract defined by the applicable Specification and schema.

Adapters MAY convert external formats before admission, but SHALL:

- identify the source representation;
- validate the source;
- apply an Approved conversion;
- record provenance;
- expose loss or uncertainty;
- produce the canonical Falcon representation; and
- preserve the original where evidence policy requires.

An Adapter SHALL NOT claim the source was canonical.

## 25. Test Vector Governance

Every FCE schema SHALL have Approved:

- positive vectors;
- negative vectors;
- boundary vectors;
- cross-platform vectors;
- version vectors;
- malformed-input vectors;
- ordering vectors;
- Unicode vectors where text permits Unicode;
- identifier byte-order vectors;
- timestamp precision vectors;
- leap-second rejection vectors;
- Domain Context vectors; and
- Time Observation vectors.

Each vector SHALL identify:

- vector ID;
- FCE profile and version;
- schema ID and version;
- semantic input;
- exact canonical text or hexadecimal bytes;
- expected result;
- rejection reason where negative;
- source authority;
- integrity identity; and
- approval status.

Vectors are Trust Objects governed by SEC-002.

Generated vectors SHALL NOT approve themselves.

## 26. Cross-Platform Verification

Conformance SHALL be demonstrated on Approved Windows and Linux environments.

Independent implementations or independently configured realizations SHALL produce identical bytes for every positive vector and identical rejection classes for every negative vector.

Verification SHALL include:

- different processor byte orders where available or simulated;
- platform GUID conversion traps;
- database UUID round trips;
- timestamp parsing and persistence round trips;
- Unicode normalization traps;
- unordered map iteration;
- integer boundary values;
- maximum permitted sizes;
- unknown fields;
- version mismatch;
- Domain Context derivation input; and
- Time Observation protection input.

One implementation comparing output to itself does not establish cross-platform conformance.

## 27. Security Requirements

- **FCE-001-REQ-001:** Every governed canonical value SHALL identify its FCE profile and applicable schema version.
- **FCE-001-REQ-002:** One supported semantic value SHALL produce exactly one canonical representation within a declared profile and schema version.
- **FCE-001-REQ-003:** Canonical parsing SHALL reject alternative equivalent representations.
- **FCE-001-REQ-004:** Composite values SHALL use typed, length-delimited, deterministic framing.
- **FCE-001-REQ-005:** Free-form concatenation SHALL NOT be used for governed composite canonical values.
- **FCE-001-REQ-006:** Field order SHALL be deterministic and independent of runtime iteration.
- **FCE-001-REQ-007:** Required fields SHALL NOT receive hidden defaults.
- **FCE-001-REQ-008:** Canonical text SHALL use valid UTF-8 without BOM and comply with declared normalization and character constraints.
- **FCE-001-REQ-009:** FCE-BINARY-1 integers SHALL use the schema-declared fixed width and big-endian byte order.
- **FCE-001-REQ-010:** Absence, null, and empty SHALL remain distinct.
- **FCE-001-REQ-011:** Schema and Field IDs SHALL be immutable in meaning and SHALL NOT be reassigned.
- **FCE-001-REQ-012:** UUID text SHALL use lowercase standard hyphenated form.
- **FCE-001-REQ-013:** UUID bytes SHALL use RFC 9562 network byte order.
- **FCE-001-REQ-014:** Canonical timestamps SHALL use exactly `YYYY-MM-DDTHH:MM:SS.ffffffZ`.
- **FCE-001-REQ-015:** Sub-microsecond conversion SHALL discard toward the earlier instant and expand uncertainty.
- **FCE-001-REQ-016:** Canonical timestamps SHALL reject leap-second `:60`.
- **FCE-001-REQ-017:** Runtime Epoch IDs SHALL use the governed identifier representation and SHALL NOT be inferred from platform values.
- **FCE-001-REQ-018:** Cryptographic Domain Context SHALL use the exact schema and field framing defined by section 16.
- **FCE-001-REQ-019:** Domain and Purpose IDs SHALL be governed Catalog values, not free text.
- **FCE-001-REQ-020:** Protected or signed Time Observations SHALL use the exact schema and field framing defined by section 17.
- **FCE-001-REQ-021:** Canonical-byte equality across different schemas or versions SHALL NOT establish semantic equality by itself.
- **FCE-001-REQ-022:** Integrity verification SHALL NOT normalize noncanonical protected input into acceptance.
- **FCE-001-REQ-023:** Database, platform, language, serializer, or transport representation SHALL NOT redefine canonical form.
- **FCE-001-REQ-024:** Unknown format, schema, version, WireType, or prohibited field SHALL cause explicit rejection.
- **FCE-001-REQ-025:** Canonical failure SHALL NOT cause permissive fallback.
- **FCE-001-REQ-026:** Historical canonical versions SHALL remain interpretable for governed retention and recovery.
- **FCE-001-REQ-027:** Components SHALL NOT implement, alias, or reinterpret canonicalization independently.
- **FCE-001-REQ-028:** External conversion SHALL preserve provenance, loss, uncertainty, and source representation as required.
- **FCE-001-REQ-029:** Every canonical schema SHALL have Approved positive, negative, boundary, and cross-platform test vectors.
- **FCE-001-REQ-030:** Windows and Linux conforming realizations SHALL produce identical canonical bytes.
- **FCE-001-REQ-031:** Successful decoding SHALL NOT be interpreted as truth, authority, freshness, or acceptance.
- **FCE-001-REQ-032:** FCE-001 SHALL define representation only and SHALL NOT redefine meanings owned by CRY-001, IDN-001, TIM-001, FIL-001, or other governing artifacts.

## 28. Failure and Uncertainty

When canonical representation cannot be established:

- protected processing SHALL be rejected;
- affected Claim SHALL remain unverified;
- authority depending on the value SHALL be denied or restricted;
- persistence SHALL NOT acknowledge the value as a governed canonical state change;
- evidence SHALL record the failure without substituting a guessed value;
- original input SHALL be preserved where safe and required;
- uncertainty SHALL remain explicit; and
- reconciliation SHALL use the original identity and source representation.

Unknown canonical state SHALL NOT be treated as valid canonical state.

## 29. Conformance Evidence

Approval of a realization requires evidence that:

- every positive vector produces exact expected bytes;
- every negative vector is rejected for the expected class;
- Windows and Linux output is identical;
- UUID network byte order survives platform and database round trips;
- timestamp output is exact and culture-independent;
- finer precision truncates toward the earlier instant;
- leap-second input is rejected without Approved conversion;
- unordered input cannot change canonical bytes;
- duplicate and out-of-order fields are rejected;
- missing and unknown fields fail correctly;
- Domain Context cannot collide through ambiguous concatenation;
- Time Observation capabilities sort deterministically;
- noncanonical protected input cannot be normalized into acceptance;
- schema downgrade is rejected;
- size and nesting limits are enforced;
- external conversion preserves provenance; and
- no Catalog or component redefines FCE representation.

## 30. Required Dependent Artifacts

Full pre-implementation completion requires:

- registration of FCE-001 in SPEC-000;
- CRY-001 Cryptographic Domain and Profile Catalog;
- IDN-001 Foundation Identifier Catalog;
- TIM-001 Foundation Time and Clock-Quality Catalog;
- governed FCE schema registry;
- Approved Domain Context vectors;
- Approved identifier and timestamp vectors;
- Approved Time Observation vectors;
- DESIGN-SEC-001 provider and custody design;
- affected Contract amendment decisions;
- TRC-001 traceability; and
- PIPE-001 verification gates.

## 31. Foundational Rules

> **Canonical representation is a trust boundary, not a formatting preference.**

> **A parser may reject ambiguity; it may not invent meaning.**

> **A canonical value may prove stable representation. It does not prove truth, authority, or permission.**

> **Catalogs govern values. Specifications govern rules. FCE-001 governs canonical representation.**

## 32. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner and current Constitutional Authority | Approved | GOV-014 | 2026-07-25 |

This Approval adopts FCE-001 v1.0 into the Foundation Baseline and registers it in SPEC-000.

It does not authorize implementation, change a Catalog, modify a Contract, activate a cryptographic profile, or authorize protected operations.
