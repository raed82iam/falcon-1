# Stage 7 — WP-03 Foundation Self Model Runtime — Implementation Design and Trace

**Date:** 2026-08-12  
**Subject:** `WP-03 — Foundation Self Model Runtime`  
**Status:** `IMPLEMENTATION CANDIDATE / PRE-EXECUTABLE REVIEW`  
**Stage 7 Plan:** `v0.3 OWNER_ACCEPTED`  
**Implementation Authority:** `GRANTED`  
**Predecessor:** `WP-02 TECHNICALLY_VALIDATED / OWNER CLOSURE DEFERRED`

## 1. Purpose

Define the bounded executable realization of the AWR-001 Foundation Self Model required by Stage 7 WP-03 without duplicating authoritative predecessor truth, inventing Application semantics, evaluating Technical Fitness early, or pulling Stage 8, Stage 9, Stage 13 or later control-plane authority backward.

The WP-03 product is a deterministic Foundation-only projection over attributable technical assertions and the already-governed SYS-008 Health assessment surface.

## 2. Governing Sources

Controlling sources for this candidate are:

- Falcon Vision and Falcon Constitution;
- AWR-001 v2.1;
- SYS-008 current effective Health policy as activated through Stage 7 Gate 0B;
- CON-006 current active Health/Fitness contract boundary;
- Stage 7 Implementation Plan v0.3;
- Stage 7 Gate 0A Exact Code Reuse / Ownership Census;
- Stage 7 Owner implementation authorization;
- WP-01 canonical Health/Fitness primitives;
- WP-02 Health observation/assessment runtime and final technical validation;
- Foundation Workstream Rules.

AWR-002 through AWR-005 remain unactivated and are not used as normative sources.

## 3. Exact WP-03 Responsibility

WP-03 SHALL implement only the Foundation Self Model projection required by AWR-001.

The projection SHALL represent, with attributable evidence and explicit uncertainty, the required Foundation areas:

1. Foundation identity;
2. admitted baseline;
3. Core component identity;
4. Core component version;
5. lifecycle condition as observed source truth;
6. Core component integrity;
7. runtime condition;
8. infrastructure condition;
9. SYS-008 Health condition;
10. Service Bus technical condition;
11. FIL technical condition;
12. dependency availability;
13. dependency compatibility;
14. dependency criticality;
15. resource capacity;
16. resource pressure;
17. resource exhaustion risk;
18. persistence condition;
19. backup condition;
20. restore condition;
21. corruption condition;
22. documentation integrity;
23. configuration integrity;
24. technical security condition;
25. technical authority condition;
26. known incident condition;
27. known fault condition;
28. contradiction condition;
29. blind-spot condition;
30. isolation readiness as observed state;
31. recovery readiness as observed state;
32. active restrictions as observed state;
33. Foundation Technical Fitness representation;
34. pending conformance-case representation.

The final two areas are representational only in WP-03. WP-03 SHALL NOT compute Technical Fitness and SHALL NOT implement conformance/adoption governance. Until later governed producers exist, they may be represented explicitly as unknown/insufficient rather than inferred.

## 4. Production Ownership

A new bounded production project is justified:

`src/Foundation.SelfAwareness/Foundation.SelfAwareness.csproj`

Intended exact project references:

- `Foundation.Contracts`;
- `Foundation.HealthFitness`.

The project SHALL NOT reference:

- Applications;
- Foundation.Authority;
- Guardian/protection command surfaces;
- Lifecycle control implementations;
- Recovery implementations;
- external connectivity;
- trading/market/financial code.

Concrete predecessor adapters/integration into Lifecycle, dependency, resource, persistence, security and other accepted truth owners are deferred to WP-06. Durable persistence/reconstruction is deferred to WP-07.

This keeps WP-03 focused on projection semantics rather than source acquisition or ownership transfer.

## 5. Canonical Self Model Assertion

The candidate runtime shall define an immutable assertion representation containing at least:

- `AssertionId`;
- `SubjectId`;
- exact `Area`;
- assertion kind;
- temporal awareness view;
- technical `Scope`;
- opaque technical `ValueIdentity`;
- `AuthoritativeSourceId`;
- `SourceOwner`;
- `EvidenceReference`;
- `EvidenceQuality` using the existing Stage 7 enum;
- `Confidence`;
- explicit `Uncertainty`;
- `FreshnessReference` identifying the governing source/rule/validity basis rather than creating a new freshness policy;
- `RuleId` and `RuleVersion`;
- `ObservationTime`;
- `EffectiveTime`;
- `Expiry`;
- optional `SupersedesAssertionId` lineage.

The Self Model stores source attribution. It does not become the owner of that source fact.

## 6. Fact / Estimate / Assumption / Interpretation / Unknown Separation

AWR-001-REQ-002 is made executable through an explicit assertion-kind enum:

- `FACT`;
- `ESTIMATE`;
- `ASSUMPTION`;
- `INTERPRETATION`;
- `UNKNOWN`.

The projector SHALL preserve the declared kind. It SHALL NOT silently upgrade an estimate, assumption, interpretation or unknown into fact.

An `UNKNOWN` assertion SHALL NOT carry `EQ-SUFFICIENT` evidence quality.

## 7. Current / Last-Known / Expected / Desired / Historical Separation

AWR-001-REQ-012 is made executable through an AWR-specific knowledge-view enum:

- `CURRENT`;
- `LAST_KNOWN`;
- `EXPECTED`;
- `DESIRED`;
- `HISTORICAL`.

This enum describes the Self Model's awareness viewpoint. It is not a duplicate State persistence engine and does not replace `Foundation.State.StateRepresentationKind`.

Rules:

- `CURRENT` assertions require observation/effective time no later than model time and expiry later than model time;
- expired evidence cannot remain represented as `CURRENT`;
- `LAST_KNOWN` may preserve an expired formerly trustworthy observation with its original age/expiry visible;
- `EXPECTED` and `DESIRED` are never interpreted as current source truth;
- `HISTORICAL` remains non-current and lineage-preserving.

WP-03 does not invent a second SYS-008 freshness window. `FreshnessReference`, source expiry, evidence quality and exact times preserve the existing freshness basis.

## 8. Required Coverage and Honest Unknowns

AWR-001 says the Self Model SHALL represent the required Foundation areas.

Therefore the projector SHALL require coverage for every WP-03 area by at least one `CURRENT` or `LAST_KNOWN` assertion.

If trustworthy current source evidence is unavailable, the caller must provide an explicit attributable `UNKNOWN` assertion for the affected area with insufficient/invalid evidence quality and explicit uncertainty. Missing coverage SHALL NOT be interpreted as healthy/default/empty success.

This rule permits zero-Application operation without requiring any Application assertion.

## 9. Contradiction Preservation

For the same material current awareness key, conflicting current assertions SHALL:

- both remain present;
- produce a deterministic contradiction record;
- identify the conflicting assertion IDs;
- remain visible in model identity/evidence identity;
- never be silently collapsed into one favorable value.

WP-03 exposes contradiction. It does not decide the later Fitness consequence. That belongs to WP-04/WP-05 under their exact governed responsibilities.

## 10. Historical Lineage

The model shall preserve lineage through:

- optional previous model identity;
- assertion-level supersession reference;
- deterministic content identity over the full sorted assertion set and contradiction set.

WP-03 provides lineage semantics in the projection. Durable storage/reconstruction against the accepted State/Evidence substrate remains WP-07 work.

## 11. Deterministic Projection

`FoundationSelfModelProjector` shall:

1. validate canonical model/foundation/baseline identities;
2. validate every assertion and enum;
3. reject duplicate assertion IDs;
4. reject future-dated observations;
5. enforce current-vs-expired representation rules;
6. require complete minimum-area coverage;
7. preserve explicit unknowns;
8. detect material current contradictions deterministically;
9. sort assertions and contradiction members deterministically;
10. compute deterministic model evidence identity and model identity;
11. return an immutable/read-only snapshot.

Changing a material assertion source, evidence reference, value, time, quality, uncertainty, lineage or rule identity SHALL change the resulting model identity.

## 12. Health-to-Self-Model Boundary

WP-03 shall be able to represent the already-produced `CanonicalHealthAssessment` as attributable Self Model evidence without re-evaluating Health.

The Self Model SHALL preserve the Health assessment identity/evidence basis and state. It SHALL NOT:

- recompute SYS-008 Health policy;
- turn Health into authority;
- use its own interpretation as required positive proof for Health;
- create a circular positive proof chain.

Concrete source acquisition/adapters remain WP-06.

## 13. Explicit Non-Scope

WP-03 SHALL NOT implement:

- Technical Fitness computation or CON-006 projection, owned by WP-04;
- broad drift/blind-spot/independent-challenge evaluation, owned by WP-05;
- concrete accepted predecessor adapters/integrations, owned by WP-06;
- durable Self Model persistence/reconstruction/events, owned by WP-07;
- Authority/Lifecycle/protective-consumer enforcement, owned by WP-08;
- VPL-005 integrated evidence-loss hardening, owned by WP-09;
- Guardian / Safe-State enforcement, Stage 8;
- recovery execution/release, Stage 9;
- broad QoS/observability, Stage 11;
- FSA/Owner governance, Monitor AI, Kill/Factory Reset/Controlled Revival or self-development governance, Stage 13;
- Application MSA/LSA/CSA internals;
- Application business/trading/financial meaning.

## 14. Verification Surface

Add:

`verification/Falcon.Stage7.WP03.Verifier`

The verifier shall reference only the production surfaces required to verify WP-03.

Minimum executable scenarios:

1. complete zero-Application Foundation model projects successfully;
2. deterministic rerun produces identical model identity;
3. missing required area fails closed;
4. explicit unknown area is preserved and does not become sufficient fact;
5. fact/estimate/assumption/interpretation/unknown remain distinct;
6. current/last-known/expected/desired/historical remain distinct;
7. expired assertion cannot masquerade as current;
8. future-dated observation is rejected;
9. conflicting current assertions remain visible as deterministic contradiction;
10. source/evidence/value/time/quality/uncertainty mutation changes model identity;
11. duplicate assertion identity is rejected;
12. Health assessment can be represented without Self Model recomputing Health;
13. no production authority/Guardian/Lifecycle/Recovery action surface exists;
14. no Application business or MSA/LSA/CSA dependency exists;
15. no persistence/reconstruction claim is made before WP-07;
16. Architecture harness requires exact project references and controlled-solution membership;
17. WP-01 and WP-02 regressions remain green.

## 15. Expected Candidate Change Surface

The executable candidate is expected to touch only:

- `src/Foundation.SelfAwareness/Foundation.SelfAwareness.csproj`;
- `src/Foundation.SelfAwareness/FoundationSelfModelRuntime.cs`;
- `verification/Falcon.Stage7.WP03.Verifier/Falcon.Stage7.WP03.Verifier.csproj`;
- `verification/Falcon.Stage7.WP03.Verifier/Program.cs`;
- `tests/Falcon.Foundation.Architecture.Tests/Program.cs`;
- `Falcon.Foundation.ControlledProjectFoundation.slnx`.

Any additional production/predecessor change requires fresh classification before it is allowed.

## 16. Stop Conditions

Stop rather than invent semantics if implementation proves that a required WP-03 behavior cannot be derived from AWR-001, current SYS-008/CON-006, the accepted Stage 7 plan and accepted predecessor contracts.

A discovered genuine normative gap shall be classified:

`MISSING_NORMATIVE_DEFINITION`

and routed through the required specification-definition/review gate.

## 17. Candidate Disposition

```text
WP03_NORMATIVE_BASIS = SUFFICIENT_FOR_BOUNDED_PROJECTION
AWR002_TO_AWR005_ACTIVATION = NOT_REQUIRED
NEW_PRODUCTION_OWNER = Foundation.SelfAwareness
SELF_MODEL = DERIVED_PROJECTION_ONLY
SOURCE_TRUTH_OWNERSHIP_TRANSFER = FORBIDDEN
TECHNICAL_FITNESS_EVALUATION = DEFERRED_TO_WP04
DRIFT_INDEPENDENT_CHALLENGE = DEFERRED_TO_WP05
PREDECESSOR_INTEGRATION = DEFERRED_TO_WP06
PERSISTENCE_RECONSTRUCTION = DEFERRED_TO_WP07
STAGE8_STAGE9_STAGE13_AUTHORITY = NOT_CREATED
READY_FOR_WP03_PRE_EXECUTABLE_RED_TEAM = YES
```
