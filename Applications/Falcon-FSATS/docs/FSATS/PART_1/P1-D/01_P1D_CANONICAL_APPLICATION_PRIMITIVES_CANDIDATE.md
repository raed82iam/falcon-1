# P1-D — Canonical Application-Owned Primitives and Structural Types Candidate

**Status:** `DESIGN_CANDIDATE / NOT_OWNER_ACCEPTED / NOT_CLOSED`  
**Branch:** `application-development`  
**Implementation Authority:** `NOT GRANTED`  
**Runtime Authority:** `NOT GRANTED`

## 1. Purpose

P1-D defines the semantic ownership and design rules for reusable FSATS Application-owned primitives and structural types before code implementation.

It SHALL prevent three failure modes:

1. cloning Foundation-owned identity/authority/communication/evidence/resource semantics inside Applications;
2. collapsing distinct business meanings into generic types merely because their storage shape looks similar;
3. creating unbounded `Common`/`Shared` bags that silently couple otherwise independent Falcon Applications.

P1-D is declaration/design only. It does not create runtime contracts, routes, persistence, implementation code, provider/broker connectivity, or production authority.

## 2. Governing Ownership Rule

For every candidate type:

```text
FOUNDATION_OWNED_SEMANTIC?
 -> CONSUME THE FOUNDATION TYPE/CONTRACT OR OPAQUE GOVERNED REFERENCE

APPLICATION_BUSINESS_OWNED?
 -> DEFINE UNDER THE OWNING APPLICATION

DOMAIN WRAPPER AROUND FOUNDATION IDENTITY?
 -> ALLOW ONLY WHEN THE WRAPPER ADDS APPLICATION BUSINESS MEANING
    WITHOUT REDEFINING FOUNDATION AUTHORITY/IDENTITY

MERELY SAME STORAGE SHAPE?
 -> DO NOT MERGE
```

No type becomes shared merely because two Applications both use `string`, `decimal`, `Guid`, timestamps, percentages, or similar storage.

## 3. Foundation Semantics That SHALL NOT Be Reimplemented

P1-D SHALL NOT create an Application-owned replacement for Foundation-owned semantics including:

- Falcon Application identity/admission/lifecycle identity;
- Foundation service identity;
- Foundation authority/delegation identity;
- Foundation permissions/security principal semantics;
- FIL envelope identity;
- Service Bus route identity;
- Foundation event-system identity;
- canonical correlation/causation/evidence identities where Foundation owns them;
- Foundation lifecycle state;
- Foundation total-resource/grant/ceiling/floor truth;
- Foundation canonical clock/time authority where such a contract is supplied;
- canonical package/provenance/integrity identity owned by Foundation.

Applications MAY hold opaque references to those concepts where required, but the reference SHALL NOT recreate or reinterpret the underlying Foundation authority.

Examples:

```text
FoundationApplicationIdRef
FoundationGrantRef
FoundationEvidenceRef
FoundationCorrelationRef
```

These are conceptual Application-side references only until exact Foundation binding is materialized. They SHALL preserve the original authoritative identity and SHALL NOT mint new Foundation truth.

## 4. Sharing Rule Across the Five FSATS Applications

The accepted P1-C topology contains five independent Falcon Applications.

A primitive SHALL be placed in an Application's own `Domain`/`Contracts` boundary unless genuine cross-Application semantic ownership is proven.

P1-D SHALL NOT create an `FSATS.Common` runtime authority or hidden shared business owner.

```text
FSATS SYSTEM BOUNDARY
Application = NO
Runtime Principal = NO
Business State Owner = NO
Primitive Authority Owner = NO
```

If two Applications need to exchange a business value, the producer Application owns the contract meaning and later exposes it through its producer-owned `*.Contracts` package under P1-K.

## 5. Canonical Primitive Design Requirements

Every implemented primitive derived from this design SHALL define:

- semantic name;
- owning Application/domain;
- underlying representation;
- allowed unit/currency/reference namespace where relevant;
- valid range and invalid states;
- normalization rules;
- equality semantics;
- serialization representation;
- versioning/compatibility rule when externally exposed;
- whether default/empty value is forbidden;
- whether comparison/arithmetic is valid;
- security/sensitivity classification where relevant;
- conversion/mapping rule when wrapping another authoritative identity;
- negative fixtures proving invalid states are rejected.

Primitive constructors/parsers SHALL fail closed on invalid or ambiguous values rather than normalize materially distinct meaning silently.

## 6. Trading Application Primitive Families

The Trading Application may own the following semantic families, subject to later exact code materialization.

### 6.1 Trading Account / Environment References

Candidate concepts:

- `TradingAccountRef`
- `TradingEnvironment`
- `TradingMode`

Rules:

- `TradingAccountRef` identifies an Application business account binding and is not a broker secret or Foundation user identity;
- environment values SHALL distinguish materially different execution contexts such as paper/simulation/live when later authorized;
- environment/mode SHALL NOT itself grant runtime authority;
- `LIVE` as a value SHALL NOT imply Live authorization.

### 6.2 Market and Instrument Identity

Candidate concepts:

- `MarketId`
- `InstrumentId`
- `InstrumentSymbol`
- `AssetClass`

Rules:

- identity SHALL be unambiguous within an exact market/venue/context;
- display symbol SHALL NOT be assumed globally unique;
- canonical instrument identity SHALL not depend only on ticker text;
- delisted/expired/replaced instruments remain historically identifiable.

### 6.3 Financial Quantity Types

Candidate concepts:

- `CurrencyCode`
- `Money`
- `Price`
- `Quantity`
- `NotionalValue`
- `ExposureAmount`
- `Ratio`
- `Percentage`
- `BasisPoints`

Rules:

```text
MONEY != PRICE != NOTIONAL != EXPOSURE
QUANTITY REQUIRES INSTRUMENT/UNIT CONTEXT
PERCENTAGE != RATIO != BASIS_POINTS
```

- no floating binary representation where it could create material financial ambiguity;
- currency SHALL be explicit for money/notional/exposure values;
- price SHALL bind to instrument/quotation semantics where needed;
- negative/zero rules depend on exact business meaning and are not globally assumed;
- arithmetic across incompatible currency/unit contexts is rejected unless an explicit governed conversion exists.

### 6.4 Confidence / Quality / Fitness

Candidate concepts:

- `ConfidenceScore`
- `DataQualityScore`
- `StrategyFitnessScore`
- `ModelFitnessScore`

Rules:

```text
CONFIDENCE != QUALITY != FITNESS
```

A common bounded numeric representation MAY be reused internally only through separately named semantic types. They SHALL NOT become interchangeable merely because all may use a normalized range.

Confidence SHALL NOT create authority.

## 7. FSAPMA Primitive Families

FSAPMA may own:

- `ProviderId`
- `ProviderAccountRef`
- `DataProductId`
- `ProviderServiceRole`
- `ProviderCapabilityRef`
- `ProviderQuotaObservation`
- `DataFreshnessState`
- `DataQualityState`
- `ProviderAvailabilityState`

Rules:

- provider identity is not credential identity;
- one provider may expose multiple accounts/service roles/products;
- provider quota/capacity business semantics remain separate from Foundation technical-resource grants;
- data freshness/quality states describe operational data condition and do not become Foundation health truth;
- secret bytes/tokens/API keys are not primitives in ordinary domain state; future credential handling uses governed credential references under FCR-0013/Foundation Stage 12.

## 8. Trading Execution / Broker Primitive Families

Trading-owned execution semantics may include:

- `BrokerId`
- `BrokerAccountRef`
- `OrderId`
- `ClientOrderId`
- `PositionId`
- `ExecutionId`
- `OrderSide`
- `OrderType`
- `TimeInForce`
- `OrderLifecycleState`
- `ExecutionOutcome`
- `ReconciliationState`

Rules:

- broker/exchange identifiers SHALL remain distinct from Falcon/Foundation message identities;
- client-generated IDs and broker-generated IDs SHALL remain distinguishable;
- `REQUESTED`, `ACCEPTED`, `WORKING`, `PARTIALLY_FILLED`, `FILLED`, `CANCEL_PENDING`, `CANCELLED`, `REJECTED`, `UNKNOWN/RECONCILIATION_REQUIRED` or equivalent semantics SHALL not be collapsed into one boolean success flag;
- unknown broker truth SHALL not be converted to failure/success without reconciliation;
- broker credentials remain governed references, not ordinary domain strings.

Exact final enums belong to later code-ready decomposition/P1-K and are not frozen by example labels in this candidate.

## 9. Guardian Primitive Families

Guardian may own protection-domain types such as:

- `ProtectionIncidentId`
- `ProtectionScope`
- `ProtectionSeverity`
- `RestrictionClass`
- `ProtectionDirectiveId`
- `ProtectionDirectiveState`
- `CrisisState`
- `ProtectionEvidenceState`

Rules:

- Guardian protection truth SHALL remain distinct from Trading risk/position truth;
- a Guardian directive identity does not become Foundation authority identity;
- severity does not itself create permission to exceed accepted authority;
- expiration, scope, idempotency and authoritative outcome semantics are material and later bind through P1-K.

## 10. FSTSimA Primitive Families

FSTSimA may own:

- `SimulationRunId`
- `ScenarioId`
- `SimulationClockPosition`
- `SimulationSeed`
- `ReplayClassification`
- `FidelityScore`
- `CalibrationState`
- `ValidationAssessmentState`
- `FaultInjectionId`

Rules:

- simulation time is not Falcon authoritative wall-clock time;
- simulated order/provider/event identities SHALL be explicitly non-operational and shall not collide semantically with Live/authoritative identities;
- simulation evidence cannot become production authority;
- `FidelityScore != ValidationAssessmentState`;
- deterministic seed/scenario identity must be sufficient for reproducibility when later implemented.

## 11. APP-RSC Primitive Families

APP-RSC may own FSATS resource-coordination business semantics while Foundation retains authoritative Falcon-wide resource truth.

Candidate APP-RSC concepts:

- `ResourceDemandIntentId`
- `ResourceDemandIntent`
- `MinimumSafeRequirement`
- `DesiredResourceLevel`
- `ReclaimabilityProfile`
- `DegradationClass`
- `WorkloadPriorityEvidence`
- `ResourcePressureEvidence`
- `ResourceCoordinationEpoch`
- `EffectiveCoordinationOutcome`
- `ResidualResourceNeed`
- `RestorationReadiness`

Rules:

```text
REQUESTED_RESOURCE != PROVEN_RESIDUAL_NEED != FOUNDATION_GRANTED_RESOURCE
APP_RSC_EFFECTIVE_COORDINATION != FOUNDATION_AUTHORITATIVE_RESOURCE_TRUTH
```

APP-RSC types SHALL NOT mint or redefine:

- Foundation grants;
- Foundation ceilings/floors;
- Foundation total-resource identity;
- Foundation priority authority;
- technical criticality owned by Foundation.

When APP-RSC must refer to Foundation-authoritative state, it SHALL use an exact opaque/governed Foundation reference plus Application-owned interpretation/evidence where permitted.

`ResourceCoordinationEpoch` is Application-owned only for APP-RSC coordination/fencing. It SHALL NOT replace Foundation correlation/causation/authority epochs.

## 12. Safety Continuity Primitive Families

The Owner-accepted Safety Continuity and AI Repair/Controlled Recovery design requires exact Application-owned business state without creating a duplicate Foundation lifecycle system.

Candidate cross-cutting semantic families, owned by the relevant Application exposing the state:

- `IntelligenceAvailabilityState`
- `ContainmentImpactState`
- `SafetyContinuityState`
- `ExposureProtectionState`
- `ContinuityObligationState`
- `SafetyEnvelopeId`
- `RecoveryIncidentId`
- `RecoveryClass`
- `RecoveryValidationState`
- `ControlledRevivalState`

Canonical recovery classes:

```text
R1 = bounded pre-authorized non-semantic restoration only
R2 = material/new intelligent semantics; Owner approval before Controlled Revival
R3 = critical/unknown/protected-boundary incident; Owner/governance decision required
```

Rules:

- these states describe Application-owned continuity/recovery meaning and SHALL NOT replace Foundation lifecycle states;
- `AI_KILLED` or equivalent is not automatically `APPLICATION_SUSPENDED`;
- `RESTARTED != RECOVERED`;
- `REPAIRED != TRUSTED`;
- `TESTED != RELEASED`;
- `UNKNOWN` SHALL be a real state where authoritative truth cannot be established and shall not normalize to healthy/safe;
- safety-envelope state required for existing obligations shall be reconstructable outside the sole volatile control of the killed intelligence;
- exact kill/authority/correlation references remain governed Foundation/Application contract bindings under later WPs.

## 13. Identifier Design Rule

Application-owned identifiers SHALL be strongly typed by semantic namespace.

For example:

```text
ProviderId("X") != BrokerId("X")
OrderId("123") != PositionId("123")
SimulationRunId("123") != ExecutionId("123")
```

String/Guid equality alone SHALL NOT make different semantic identities equal.

Externally supplied identities SHALL preserve original issuer/namespace when ambiguity is possible.

## 14. State and Enum Rule

Enums/state categories SHALL:

- have explicit unknown/unsupported handling where external or evolving inputs may exceed current knowledge;
- reject undefined authority-bearing values;
- not use ordinal numeric ordering as business priority unless explicitly designed that way;
- distinguish terminal/non-terminal state where materially relevant;
- preserve raw external value/evidence when needed for reconciliation rather than silently coercing it.

`UNKNOWN` does not mean `DENIED`, `FAILED`, `SAFE`, or `ZERO` unless the exact owning domain explicitly defines that interpretation.

## 15. Serialization Rule

For externally persisted or contract-exposed primitives:

- representation SHALL be deterministic;
- culture/locale-dependent numeric/date parsing is forbidden;
- canonical units SHALL be declared;
- version compatibility SHALL be explicit;
- material semantic changes require a new compatible version or governed migration rather than silently changing meaning under the same identity;
- secret material SHALL not be serialized through ordinary domain primitives;
- Foundation-owned identifiers SHALL preserve exact authoritative representation or governed mapping.

## 16. Equality and Comparison Rule

Equality SHALL reflect semantic identity, not incidental display values.

Comparison/arithmetic is permitted only where the domain defines it.

Examples:

```text
InstrumentSymbol("ABC") == InstrumentSymbol("ABC")
DOES NOT PROVE
InstrumentId(marketA,"ABC") == InstrumentId(marketB,"ABC")
```

```text
Money(100,"USD") + Money(100,"SAR")
= INVALID WITHOUT EXPLICIT CONVERSION
```

## 17. No Authority Through Primitive Construction

Creating/parsing a type SHALL NOT create business or runtime authority.

Examples:

```text
TradingMode(LIVE) != LIVE_AUTHORIZED
ProtectionSeverity(CRITICAL) != UNBOUNDED_GUARDIAN_AUTHORITY
RecoveryClass(R1) != AUTOMATIC_RELEASE_AUTHORITY
DesiredResourceLevel(X) != FOUNDATION_GRANT(X)
```

Authority must arrive through the separately governed authority/lifecycle/contract path.

## 18. Negative Verification Obligations

Future P1-L/code verification SHALL include at minimum:

1. incompatible currencies cannot be arithmetically combined without explicit conversion;
2. display ticker cannot masquerade as globally unique instrument identity;
3. provider/broker/account/credential identities remain distinct;
4. simulation/replay identifiers cannot enter operational execution paths as authoritative Live identities;
5. confidence/quality/fitness types are not assignment-compatible by accident;
6. APP-RSC demand/coordination values cannot deserialize as Foundation grant/ceiling truth;
7. `LIVE` mode value cannot bypass runtime authority gates;
8. unknown external state cannot silently become healthy/safe/success;
9. killed-AI recovery state cannot become trusted/released merely because process restart occurred;
10. Foundation-owned opaque references cannot be locally minted as authoritative Foundation truth;
11. two semantically different identifiers with identical underlying bytes are not equal/interchangeable;
12. invalid/default/empty identifiers fail closed where forbidden.

## 19. Deferred Exact Materialization

P1-D defines semantics and constraints, not final implementation code.

Later WPs SHALL determine exact code type names/namespaces only while preserving this ownership model:

- P1-E: Manifest/lifecycle/package binding;
- P1-F through P1-J: exact Application branch/component usage;
- P1-K: contract-exposed schema/version/FIL/route bindings;
- P1-L: executable positive/negative/adversarial verification.

Any later need for a primitive whose ownership cannot be determined SHALL stop and resolve ownership before implementation.

## 20. Candidate Closure Criteria

P1-D may be presented for Owner acceptance only when fresh review establishes that:

- every proposed shared primitive has one accountable semantic owner;
- no Foundation semantic is cloned;
- no hidden `FSATS.Common` business authority is created;
- financial units/currencies are explicit;
- identifiers preserve semantic namespaces;
- safety-continuity/recovery types do not replace Foundation lifecycle/authority;
- APP-RSC business resource evidence does not become Foundation grant truth;
- simulation identity cannot masquerade as operational identity;
- negative verification obligations cover the principal invalid-state/cross-owner confusion cases.

No implementation/runtime authority follows from P1-D acceptance.
