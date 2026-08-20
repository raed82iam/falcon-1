# P1-D — Cross-Application Primitive Ownership Remediation

**Status:** `CONTROLLING REMEDIATION CANDIDATE / NOT_OWNER_ACCEPTED / NOT_CLOSED`  
**Supersedes for affected clauses:** the first P1-D freeze target `3d0a402ae152a43d52c854f1dc8e2223f1a62110`  
**Implementation Authority:** `NOT GRANTED`

## 1. Purpose

Fresh Architecture/Consistency analysis of the first P1-D freeze identified two ownership ambiguities that must be corrected before PASS:

1. market/instrument/price semantics cross the FSAPMA -> Trading boundary but no hidden shared FSATS domain owner may be created and FSAPMA must not depend on Trading internals;
2. Safety Continuity and Recovery categories are cross-cutting design semantics, while FSATS itself is not a Falcon Application/runtime owner, so those categories cannot become an ownerless `FSATS.Common` runtime type system.

## 2. FSAPMA -> Trading Market/Instrument Mapping

FSAPMA owns the operational-data producer semantics it emits. Trading owns Trading-domain instrument identity and decision semantics.

Required separation:

```text
FSAPMA OPERATIONAL DATA IDENTITY
!=
TRADING DOMAIN INSTRUMENT IDENTITY
```

FSAPMA may expose producer-owned contract concepts such as:

- provider/venue instrument reference;
- normalized operational-data instrument reference;
- quotation/value representation with explicit source/product/unit/precision/provenance semantics.

Trading may own concepts such as:

- `TradingMarketId`;
- `TradingInstrumentId`;
- Trading-owned price/quantity/value wrappers used for decision/execution semantics.

The exact names remain future code materialization.

When Trading consumes FSAPMA data:

```text
FSAPMA PRODUCER IDENTITY + PROVENANCE
-> EXPLICIT GOVERNED MAPPING
-> TRADING DOMAIN IDENTITY
```

The mapping SHALL be attributable and fail closed if identity is ambiguous, stale, conflicting or unsupported.

Forbidden:

- FSAPMA direct access to Trading internals;
- Trading direct access to FSAPMA internals;
- one ownerless `FSATS.InstrumentId` merely for convenience;
- symbol-text equality as sufficient cross-Application identity proof;
- conversion that drops provider/venue/product provenance needed to establish the mapping.

If P1-K later proves one producer-owned contract type should be consumed directly by another Application, the type remains owned/versioned by its producer package and does not become an FSATS-wide neutral owner by reuse.

## 3. Cross-Cutting Safety / Recovery Categories

Owner-accepted Safety Continuity and AI Repair/Controlled Recovery records define normative cross-cutting semantics, but they do not create `FSATS` as a runtime principal or primitive owner.

Therefore:

```text
CROSS_CUTTING DESIGN SEMANTIC
!=
OWNERLESS SHARED RUNTIME TYPE
```

Each Falcon Application SHALL own its exact operational continuity/recovery state representation for its own business/runtime scope.

Examples:

- Trading owns its exposure/protection/Trading-intelligence continuity states;
- FSAPMA owns provider/data continuity states;
- Guardian owns protection/crisis continuity states;
- FSTSimA owns simulation/validation continuity states;
- APP-RSC owns resource-coordination continuity states.

The normative categories `R1`, `R2`, `R3` remain controlling classification semantics for the accepted repair/recovery model, but P1-D SHALL NOT create a single `FSATS.RecoveryClass` runtime authority package.

Where cross-Application/Web visibility requires a comparable category, P1-K SHALL define a governed producer-owned projection/mapping that states the source Application and exact authoritative state.

```text
PRODUCER APPLICATION STATE
-> GOVERNED PROJECTION / MAPPING
-> CONSUMER VIEW
```

A consumer SHALL NOT infer or reclassify a producer's recovery state locally in a way that changes authority/trust meaning.

## 4. Financial Value Ownership Across Applications

The same rule applies to `Price`, `Money`, `Quantity`, `Percentage`, `Ratio` and related values.

A storage shape or familiar financial name does not establish shared ownership.

- A producer contract owns the semantics of values it publishes.
- A consumer may map those values into its own domain type when its business meaning differs.
- Conversion/mapping SHALL preserve units, currency, precision, source/provenance and materially relevant context.
- No global `FSATS.Money` or `FSATS.Price` runtime package is created by P1-D.

A future genuinely shared financial primitive may be introduced only through an explicit owner and governed review proving that one semantic meaning is actually shared rather than merely similarly represented.

## 5. Verification Additions

Future verification SHALL prove:

1. FSAPMA can remain independently buildable/removable without direct dependency on Trading internals;
2. Trading cannot accept an ambiguous FSAPMA instrument mapping;
3. identical symbol/text from two venues/providers cannot silently map to one Trading instrument without governed identity evidence;
4. no `FSATS.Common`/`FSATS.RecoveryClass`/`FSATS.InstrumentId` ownerless runtime authority package exists;
5. each continuity/recovery projection is attributable to its producing Application;
6. consumer display/composition does not become producer authority;
7. producer-owned contract reuse does not transfer ownership to the consumer.

## 6. Review Consequence

The first freeze remains historical evidence for its exact prior semantic state only.

This remediation changes semantics and therefore requires:

```text
NEW V2 SEMANTIC FREEZE
-> FRESH ARCHITECTURE / CONSISTENCY
-> FRESH RED-TEAM
-> OWNER REVIEW
```

No implementation/runtime authority is granted.
