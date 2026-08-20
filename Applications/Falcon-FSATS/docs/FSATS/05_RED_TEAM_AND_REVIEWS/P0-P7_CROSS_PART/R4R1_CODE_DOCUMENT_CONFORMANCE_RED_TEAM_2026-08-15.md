# FSATS R4R1 Code-to-Document Fresh Red Team Review

**Date:** `2026-08-15`  
**Exact attacked semantic source:** `6925a38a3476466fb847f2d0a87349fdb1ce23e9`  
**Architecture / Consistency review:** `R4R1_CODE_DOCUMENT_CONFORMANCE_ARCHITECTURE_REVIEW_2026-08-15.md`  
**Review mode:** fresh adversarial static code-to-document review after R4 remediation  
**Executable evidence:** `PENDING / SEPARATE`

## 1. Attack objective

The Red Team attempted to make the exact source violate a governing document/FCR semantic while still appearing structurally valid.

The attack set included the original R4 attack surface plus direct regression attacks against every R4 Medium finding.

## 2. R4 finding regression attacks

### Attack A — smuggle broker truth into FSTSimA shadow truth

Attempted conditions:
- arbitrary free-string top-level freshness/truth;
- `BROKER_CONFIRMED_CURRENT` or equivalent top-level FSTSimA truth;
- `CURRENT` diagnostic freshness treated as current broker-account truth.

Result:
- top-level truth is `EmergencyShadowProjectionTruth`;
- top-level freshness is `EmergencyShadowFreshnessState`;
- available truth members are only Simulator/Replay/Synthetic/Test;
- no Broker/Live truth class is exposed;
- document explicitly preserves `SHADOW_PROJECTION_TRUTH != BROKER_TRUTH`.

**Attack result:** `RESISTED`.

### Attack B — stale truth plus current freshness upgraded to current synthesis

Attempted input:

```text
Input TruthState = STALE
Input FreshnessState = CURRENT
OverallTruthState = CURRENT
```

Result: constructor rejects because current overall truth requires both current input truth and current freshness. Dedicated adversarial fixture covers the contradictory pair.

**Attack result:** `RESISTED`.

### Attack C — fabricate zero/derived portfolio truth under no-source availability

Attempted payloads:
- `UNSUPPORTED` portfolio summary with numeric `0`;
- `UNSUPPORTED` position collection with a derived position row;
- `UNSUPPORTED` order/activity collection with a derived activity row;
- `NOT_APPLICABLE` performance projection with numeric `0` or nonempty history.

Result: validating constructors reject these payloads. Empty/null business payload remains legal together with explicit availability/reason semantics.

**Attack result:** `RESISTED`.

### Attack D — make historical compatibility surface break warnings-as-errors builds

Attempted condition: retain `[Obsolete]` on historical R3 constants/types/constructor while `TreatWarningsAsErrors=true`.

Result: compiler-level Obsolete markers have been removed from the retained compatibility surfaces. Their status remains historical/noncanonical through comments/documentation. Dedicated reflection-based adversarial checks fail if Obsolete attributes return.

**Attack result:** `RESISTED_STATICALLY`.

Executable compilation remains separately required to prove there is no other source-compatibility break.

## 3. Broader code-document attacks

The Red Team additionally challenged the source to:

- make R3 `Projection`/`Command` identities canonical again;
- expose provider/API/URL/credential/raw Web data controls in FSATS analysis requests;
- turn `NEEDS_CLARIFICATION` into resolved-instrument truth;
- manufacture detailed-analysis target/confidence;
- suppress material Strategy/School disagreement into false complete synthesis;
- create account-aware Risk without exact account scope;
- hide or enable a not-applicable Strategy;
- collapse broker accounts;
- fabricate pagination continuation or correction/supersession lineage;
- admit a legacy provider route as current;
- accept provider EndpointId/provider/service-role/URL mismatch;
- treat Web presentation data as FSATS operational analysis truth;
- treat an affected ambiguous order as a confirmed position;
- omit source order identity from an ambiguous shadow case;
- omit explicit NOT_EXECUTED/PARTIALLY_EXECUTED/FULLY_EXECUTED scenarios from v1 ambiguity;
- label stale protection as broker-confirmed protected;
- collapse intentional-unprotected and unexpectedly-unprotected states;
- treat reconnect as recovery or restriction release;
- convert a Web request, UI action, projection, simulation, or configuration into execution/runtime authority.

No static path reviewed established those prohibited equivalences.

## 4. Identity and ownership attack

The attack attempted to reintroduce customer/user identity into FSATS through FCR-0201 incident projections.

Current public incident contracts remain broker-account scoped:

```text
BrokerId + BrokerAccountId + Environment
```

The controlling separation remains:

```text
FSATS_USER_ID = NONE
FSATS_CUSTOMER_ID = NONE
WEB_OWNS_CUSTOMER_TO_BROKER_ACCOUNT_MAPPING = YES
```

**Attack result:** `RESISTED`.

## 5. FSTSimA authority attack

The attack attempted to promote emergency shadow evidence into operational broker/execution truth or into Shadow trading authority.

Current semantics preserve:

```text
SIMULATOR_ESTIMATE != BROKER_TRUTH
SHADOW_PROJECTION_TRUTH != BROKER_TRUTH
SHADOW_POSITION != CONFIRMED_LIVE_POSITION
SHADOW_MONITORING != EXECUTION_CONFIRMATION
```

No Paper/Shadow/Tiny-Live/Live execution authority is created by the contract or by this remediation.

**Attack result:** `RESISTED`.

## 6. Source-compatibility attack still requiring executable evidence

The Red Team identified one item that cannot be truthfully closed by static inspection alone:

- portfolio projection records now use explicit validating record constructors rather than positional record declarations;
- public names/types/constructor order are preserved;
- a hidden in-repository dependency on compiler-generated deconstruction or other positional-record conveniences can only be conclusively excluded by exact compilation/tests.

This is not an open static semantic finding because no governing document promises positional deconstruction and no contradictory source usage was established during review. It is an **executable-evidence requirement**.

```text
STATIC FINDING = NONE
EXECUTABLE COMPATIBILITY PROOF = REQUIRED
```

## 7. Static severity summary

```text
CRITICAL = 0
HIGH = 0
MEDIUM = 0
LOW = 0
```

All four R4 Medium findings are statically remediated on the attacked semantic source.

## 8. Red Team disposition

```text
R4R1 CODE <-> DOCUMENT RED TEAM = PASS_STATIC
R4-RT-01 = CLOSED_STATICALLY
R4-RT-02 = CLOSED_STATICALLY
R4-RT-03 = CLOSED_STATICALLY
R4-RT-04 = CLOSED_STATICALLY
```

This is deliberately **not** labeled full executable PASS.

## 9. Executable evidence boundary

No exact executable PASS is inherited from R3, earlier Part 2 evidence, or any earlier commit.

For exact source `6925a38a3476466fb847f2d0a87349fdb1ce23e9`:

```text
RESTORE = PENDING
RELEASE BUILD = PENDING
DOTNET TEST = PENDING
APPLICATION VERIFIERS = PENDING
EXACT EXECUTABLE VALIDATION = PENDING
```

A full final `CODE <-> DOCUMENT = ALIGNED` implementation claim and FCR-0201 handoff should wait for exact executable evidence because the current task includes implemented source, not documentary design only.

## 10. Authority non-grant

```text
PART 7 = NOT STARTED
RUNTIME AUTHORITY = NOT_GRANTED
PROVIDER/BROKER EGRESS AUTHORITY = NOT_GRANTED
PAPER = NOT_AUTHORIZED
SHADOW TRADING = NOT_AUTHORIZED
TINY LIVE = NOT_AUTHORIZED
LIVE = NOT_AUTHORIZED
DEPLOYMENT = NOT_AUTHORIZED
```

The Red Team PASS_STATIC is a conformance result, not an Owner acceptance, activation, deployment, runtime, or execution decision.
