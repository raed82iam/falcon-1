# P1-D — Precision, Absence and Authoritative-Reference Hardening

**Status:** `CONTROLLING CANDIDATE SUPPLEMENT / NOT_OWNER_ACCEPTED / NOT_CLOSED`  
**Controls:** `01_P1D_CANONICAL_APPLICATION_PRIMITIVES_CANDIDATE.md` for the clauses below  
**Implementation Authority:** `NOT GRANTED`

## 1. Purpose

This supplement closes pre-freeze ambiguity in numeric precision, absence/unknown semantics and external/authoritative reference handling.

## 2. Numeric Precision and Rounding

Financial/resource primitives SHALL NOT silently alter value to fit a representation.

```text
INPUT VALUE
-> VALIDATE REPRESENTABILITY / ALLOWED PRECISION
-> ACCEPT EXACTLY
   OR
-> REJECT / REQUIRE EXPLICIT GOVERNED ROUNDING
```

Rules:

- binary floating point SHALL NOT be used where it can create material financial equality/arithmetic ambiguity;
- numeric conversion SHALL use checked overflow behavior;
- silent truncation is forbidden;
- silent rounding is forbidden;
- any rounding required by an exact market, broker, currency, settlement or contract rule SHALL name the rounding rule and boundary explicitly;
- instrument price/quantity precision SHALL come from authoritative instrument/venue/product metadata where applicable rather than one global FSATS decimal scale;
- currency minor-unit conventions SHALL not be assumed sufficient for every financial calculation; exact business/settlement rule governs;
- notional/exposure calculations SHALL define the point at which rounding is lawful rather than rounding every intermediate operation;
- numeric serialization SHALL be deterministic and culture-independent.

A value that cannot be represented without violating the governing precision rule SHALL fail closed rather than silently mutate.

## 3. Absence, Zero and Unknown Are Distinct

P1-D adopts:

```text
ABSENT != ZERO != UNKNOWN != NOT_APPLICABLE
```

Examples:

- no stop-loss value supplied != stop-loss value of zero;
- unknown broker quantity != zero broker quantity;
- no Foundation grant reference available != a proven zero grant;
- not-applicable provider quota != zero remaining quota;
- unavailable confidence evidence != confidence score zero.

Externally exposed/persisted primitives SHALL encode these distinctions explicitly when they are materially possible. Default language/runtime values SHALL NOT silently collapse them.

## 4. External / Authoritative Reference Rule

An opaque reference to Foundation, broker, provider, exchange, Shared Application or other authoritative external identity SHALL preserve enough issuer/namespace/context to prevent same-text identity collision.

Conceptual rule:

```text
REFERENCE = ISSUER/NAMESPACE + VALUE + CONTEXT WHERE REQUIRED
```

Application code SHALL NOT treat arbitrary locally constructed strings/bytes as authoritative external identity merely because the shape is valid.

For Foundation-owned references:

```text
LOCAL PARSE/CONSTRUCTION != FOUNDATION ISSUANCE
LOCAL STORAGE != FOUNDATION AUTHORITY
REFERENCE PRESERVATION != SEMANTIC REIMPLEMENTATION
```

For broker/exchange identities:

- broker-generated order/execution/position identifiers retain broker/account/environment context where needed;
- client-generated identifiers remain distinct from broker-issued identifiers;
- an identical textual ID issued by different brokers/accounts/environments is not assumed identical.

For Falcon user identity:

- FSATS SHALL consume a governed Falcon user/actor identity reference from the authoritative identity boundary;
- FSATS SHALL NOT create a second canonical Falcon-user identity system in P1-D;
- Application business account/subscription/account-binding identifiers may reference the Falcon user but do not replace it.

## 5. Foundation Resource Quantity / Unit Rule

APP-RSC SHALL NOT define a competing technical resource-unit system.

Where APP-RSC expresses `MinimumSafeRequirement`, `DesiredResourceLevel`, `ResidualResourceNeed` or related evidence:

- the business meaning/evidence classification may be APP-RSC/Application-owned;
- the referenced technical resource class/unit/quantity SHALL bind to the authoritative Foundation resource contract when such Foundation truth is involved;
- APP-RSC may calculate a business-side residual need but cannot convert that calculation into a Foundation grant/ceiling/floor by type construction;
- no local enum may silently reinterpret Foundation CPU/memory/storage/network/accelerator/other resource classes or their canonical units.

## 6. Generic Ratio / Percentage Rule

Storage similarity SHALL NOT create one universally bounded business percentage.

`Ratio`, `Percentage` and `BasisPoints` describe units/representation only. Every semantic wrapper that uses them SHALL separately declare its valid business range.

Examples:

- `ConfidenceScore` may be bounded according to its own contract;
- return percentage may legitimately exceed ranges that would be invalid for confidence;
- utilization may have different valid bounds/overflow policy from drawdown or allocation fraction.

A generic numeric unit SHALL NOT be used as a substitute for a semantic type where range/meaning affects safety.

## 7. Reason / Outcome Codes

Reason/outcome categories SHALL be owned by the producer domain, versioned when externally exposed, and shall not become one global `FSATSReasonCode` namespace.

Unknown future external reason values SHALL remain distinguishable from locally known reasons and shall not silently map to success, safe, denied or zero-impact.

## 8. Verification Additions

Future verification SHALL include:

1. precision beyond allowed instrument/broker rules is rejected unless an explicit rounding boundary is invoked;
2. overflow fails rather than wraps;
3. locale-specific decimal formatting cannot change serialized value;
4. `ABSENT`, `ZERO`, `UNKNOWN`, `NOT_APPLICABLE` remain distinguishable;
5. same textual external ID from different issuer/context does not compare equal when context is material;
6. locally constructed Foundation reference cannot manufacture Foundation issuance/authority;
7. APP-RSC business resource intent cannot create a Foundation technical resource unit/grant by local enum/type construction;
8. a generic percentage/ratio cannot bypass semantic-specific range validation.

This supplement changes no implementation/runtime authority.
