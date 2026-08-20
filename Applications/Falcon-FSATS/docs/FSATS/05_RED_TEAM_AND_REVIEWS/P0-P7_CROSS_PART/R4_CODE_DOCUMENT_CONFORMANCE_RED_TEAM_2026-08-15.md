# FSATS R4 Code-to-Document Fresh Red Team Review

**Date:** `2026-08-15`  
**Exact attacked semantic source:** `aa61f45fae6b4593d387a85600996222a8ee7c55`  
**Architecture review:** `R4_CODE_DOCUMENT_CONFORMANCE_ARCHITECTURE_REVIEW_2026-08-15.md`  
**Purpose:** adversarially test whether current executable source can violate a governing document/FCR semantic while still looking superficially valid.

## 1. Attack model

The review attempted to force the current source to:

- use stale R3 Web contract identities as current authority;
- smuggle provider/raw Web data into FSATS analysis;
- turn an ambiguous instrument into a resolved instrument;
- mutate universe/strategy/execution authority from an analysis request;
- manufacture missing targets/confidence;
- suppress Strategy/School disagreement;
- upgrade stale/partial analysis to current/complete;
- create account-aware Risk without exact account scope;
- enable/hide a `NotApplicable` Strategy;
- collapse broker accounts;
- falsify pagination or correction/supersession lineage;
- admit a legacy provider route as current;
- mismatch provider EndpointId against provider/service role/URL;
- convert URL/configuration into egress authority;
- treat Web display data as FSAPMA operational truth;
- classify arbitrary APP-RSC components as awareness tiers;
- serialize v1 enums numerically or fields with noncanonical casing;
- treat stale broker protection as current protection;
- collapse intentional-without-protection and unexpected protection loss;
- treat ambiguous order outcome as a confirmed order/position;
- invent a PositionId for an order-only ambiguous shadow case;
- collapse FSTSimA simulator evidence into broker truth;
- omit explicit ambiguous execution scenarios;
- restore authority merely because connectivity returned;
- inherit historical executable PASS onto changed source.

## 2. Attacks that were resisted

The source resisted the following at static/source level:

- current FCR Request/Result/Detailed contract identities are explicit;
- raw provider/URL/credential/data-control fields are absent from analysis requests;
- `NEEDS_CLARIFICATION` cannot claim resolution;
- detailed missing target/confidence can remain null/empty;
- partial input cannot become complete synthesis;
- material disagreement cannot become unqualified complete synthesis;
- account-aware/general Risk scope mismatch fails closed;
- `NotApplicable` hidden/enabled states fail closed;
- duplicate broker-account portfolio scopes and invalid page states fail closed;
- portfolio/Strategy update lineage misuse fails closed;
- legacy provider routes cannot win current route selection;
- EndpointId/provider/service-role/URL mismatch fails closed;
- APP-RSC awareness helper is registry-bounded;
- stale protection cannot be labeled broker-confirmed protected;
- unexpected missing/incomplete protection requires follow-up;
- intentional and unexpected unprotected states remain distinct;
- affected positions and affected orders are separate semantic surfaces;
- order-only ambiguity does not require an invented PositionId;
- ambiguous shadow cases require source-order identity plus explicit v1 execution scenarios;
- FSTSimA evidence carries explicit simulator/user-reported/last-broker-confirmed-seed identity;
- reconnect remains separate from recovery/restriction release;
- no new runtime/egress/production authority is granted.

## 3. Findings

### R4-RT-01 MEDIUM — FSTSimA top-level shadow truth/freshness is not strongly typed enough

FCR-0201 requires truth/freshness/provenance/evidence semantics. The current shadow scenario carries a typed `EvidenceTruth`, but the top-level projection carries `FreshnessState` as a free string and no explicit typed top-level simulation truth class.

An arbitrary string such as `BROKER_CONFIRMED_CURRENT` could therefore be inserted into the freshness field even though the projection is simulator evidence.

**Required remediation:** use governed enums for top-level simulation truth and freshness; preserve `SIMULATOR/REPLAY/SYNTHETIC/TEST` versus broker truth separation.

### R4-RT-02 MEDIUM — detailed analysis Current-state check can be bypassed by contradictory input truth/freshness

`WebDetailedAssetAnalysisProjection` prevents `OverallTruthState=Current` when input freshness is non-current, but the input summary has separate `TruthState` and `FreshnessState`. A contradictory summary such as `TruthState=Stale` plus `FreshnessState=Current` could still allow `OverallTruthState=Current`.

**Required remediation:** `OverallTruthState=Current` must require both current input truth and current freshness.

### R4-RT-03 MEDIUM — Web portfolio no-source/null semantics remain documented more strongly than constructed source

`FSATS.WebPortfolioContracts.v1.md` states `NO_SOURCE_VALUE != ZERO` and requires unsupported/not-applicable numeric values to remain null. The current portfolio projection records can still be directly constructed with an unsupported/not-applicable envelope while carrying fabricated numeric values or nonempty derived collections.

**Required remediation:** add constructor-level fail-closed validation to the affected summary/collection/performance projections so unsupported/not-applicable envelopes cannot carry contradictory numeric/derived business truth.

### R4-RT-04 MEDIUM — `[Obsolete]` compatibility markers may become build-breaking under warnings-as-errors

The repository uses `TreatWarningsAsErrors=true`. Current R3 compatibility constants/types/constructor are marked `[Obsolete]`. A historical in-repo or cross-workstream source consumer that still references those compatibility surfaces could therefore fail compilation due to a warning promoted to error, even though the stated intent is compatibility rather than immediate deletion.

**Required remediation:** retain the historical members without `[Obsolete]` compiler warnings; mark them as historical/noncanonical through comments/documentation and ensure current tests/contract IDs use only canonical identities.

## 4. Severity summary

```text
CRITICAL = 0
HIGH = 0
MEDIUM = 4
LOW = 0
```

## 5. Executable evidence

The GitHub Actions runner did not start because of an account billing/payment or spending-limit condition; build/tests/verifiers were not executed.

```text
EXECUTABLE FAILURE CAUSED BY CODE = NOT ESTABLISHED
EXECUTABLE PASS = NOT ESTABLISHED
EXECUTABLE VALIDATION = BLOCKED / NOT EXECUTED
```

## 6. Red Team disposition

```text
R4 RED TEAM = FAIL_REMEDIATION_REQUIRED
PART 7 = MUST NOT START FROM THIS SOURCE
```

The four Medium findings must be remediated, then a fresh Architecture/Consistency review and a fresh Red Team review must target the new exact semantic source. This report grants no Owner acceptance, runtime authority or later-Part authority.
