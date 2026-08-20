# FSATS SIA — Guardian Directive Action Parameter Specification v1.0

**Package:** `FSATS-SIA-v0.1`
**Status:** `SEMANTIC REMEDIATION / DESIGN CANDIDATE`
**Triggered By:** `RT-GRD-001`
**Owner:** APP-GRD / G-LSA-02

## 1. Purpose

Close the Guardian `ProtectionDirective.Action` payload so each action has exact required/forbidden parameters and target semantics. A consumer shall not infer missing quantities, scope or urgency from prose or local defaults.

## 2. Common Directive Fields

Every Guardian protection directive contains the common fields already defined in file 12:

```text
ProtectionDirectiveId
GuardianIncidentId
AuthorityReference
PolicyVersion
TargetApplicationId
TargetScope
Action
Severity
EffectiveFrom
ExpiresAt?
SupersedesDirectiveId?
RequiredEffectConfirmation
ReasonCodes[]
EvidenceRefs[]
ActionParameters                  // discriminated by Action
```

`ActionParameters` SHALL match exactly one schema below. Unknown action/parameter combination is rejected.

## 3. Target Scope Union

The directive has exactly one or a valid explicit combination of target selectors supported by the action:

```text
TradingAccountId?
MarketId?
InstrumentId?
StrategyVersionId?
OrderChainId?
PositionEpisodeId?
ProviderId?
ProviderRouteId?
DataProductId?
CandidateArtifactId?
```

Rules:

- TargetApplicationId always required;
- no wildcard `*` identity;
- broader scope is represented by absent lower-level selector only when the action schema explicitly permits it;
- a selector from another Application domain is forbidden unless the exact cross-App contract owns that target type;
- target scope never expands after publication; broader protection requires a new/superseding directive.

## 4. Action: RESTRICT_NEW_RISK

### Allowed target

APP-TRD only.

Scope may be:

```text
TradingAccount
TradingAccount + Market
TradingAccount + Market + Instrument
TradingAccount + StrategyVersion
```

### Parameters

```text
struct RestrictNewRiskParameters {
  bool BlockExposureIncrease = true;     // fixed true in v1
}
```

No numeric ceiling required. The directive means zero **new/increased** risk inside target scope while preserving governed reconciliation/risk-reducing actions.

Forbidden parameters: exposure ceiling, broker route, provider route, execution price.

## 5. Action: REDUCE_ALLOWED_EXPOSURE

### Allowed target

APP-TRD.

### Parameters

```text
struct ReduceAllowedExposureParameters {
  ExposureCeilingKind CeilingKind;
  decimal CeilingValue;
  CurrencyId? Currency;                  // required only for ABSOLUTE_GROSS_NOTIONAL
}

ExposureCeilingKind =
  ABSOLUTE_GROSS_NOTIONAL
  | PERCENT_DEPLOYABLE_CAPITAL
  | PERCENT_RISK_EQUITY
```

Constraints:

```text
CeilingValue >= 0
PERCENT_* CeilingValue <= 1.0
ABSOLUTE_GROSS_NOTIONAL requires Currency == TradingAccount RiskBaseCurrency
```

Trading calculates its normal Risk ceiling and Guardian ceiling independently and applies the **minimum**. Guardian cannot increase a preexisting stricter Risk limit.

If the directive ceiling is already exceeded by current confirmed exposure, it creates an exposure-reduction obligation/protection state; it does not declare that exposure was reduced.

## 6. Action: SUSPEND_STRATEGY_SCOPE

### Allowed target

APP-TRD + exact `StrategyVersionId` required.

### Parameters

```text
struct SuspendStrategyParameters {
  bool BlockNewEntries = true;
  ExistingPositionTreatment Treatment;
}

ExistingPositionTreatment =
  MANAGE_UNDER_EXISTING_PROTECTIVE_POLICY
  | REQUIRE_RISK_REASSESSMENT
  | REQUIRE_EXIT_EVALUATION
```

The directive cannot directly close positions. `REQUIRE_EXIT_EVALUATION` hands the current position to T-LSA-07/T-LSA-09 protective evaluation.

## 7. Action: SUSPEND_INSTRUMENT_SCOPE

### Allowed target

APP-TRD + exact MarketId + InstrumentId.

### Parameters

Same `ExistingPositionTreatment` union as Section 6 plus fixed `BlockNewEntries=true`.

Open orders are **not** implicitly canceled. If cancellation is required, Guardian issues `CANCEL_OPEN_ORDERS` in the same incident scope as a separate directive/action so effect tracking remains exact.

## 8. Action: SUSPEND_MARKET_SCOPE

### Allowed target

APP-TRD + exact MarketId.

### Parameters

```text
BlockNewEntries = true
ExistingPositionTreatment = ...
```

No implicit provider isolation or broker-route mutation. Those belong to their own target Application/actions.

## 9. Action: CANCEL_OPEN_ORDERS

### Allowed target

APP-TRD.

Scope must identify at least one of:

```text
TradingAccountId
MarketId
InstrumentId
OrderChainId
StrategyVersionId
```

### Parameters

```text
struct CancelOpenOrdersParameters {
  CancelEligibility Eligibility = ALL_CANCELABLE_RISK_ORDERS;
}
```

`ALL_CANCELABLE_RISK_ORDERS` excludes broker orders whose current reconciled state/venue semantics prove cancellation impossible/terminal. The target attempts cancel through T-LSA-09.

Outcome progression:

```text
DIRECTIVE_RECEIVED
-> CANCEL_REQUESTS_CREATED
-> broker outcomes/reconciliation
-> EFFECT_CONFIRMED only when every targeted eligible order is terminal/reconciled to the required protection state
```

A cancel request sent is not effect confirmation.

## 10. Action: EXIT_POSITION_SCOPE

### Allowed target

APP-TRD + exact TradingAccountId and one of PositionEpisodeId or exact InstrumentId scope.

### Parameters

```text
struct ExitPositionParameters {
  ExitTarget Target = FLAT;
  ExitUrgency Urgency;
  Duration? DesiredCompletionHorizon;    // optional evidence/scheduling goal, not authority expiry
}

ExitTarget = FLAT
ExitUrgency = CONTROLLED | URGENT | EMERGENCY
```

Trading T-LSA-07/T-LSA-09 translates urgency into the valid risk-reducing execution profile. Guardian does not choose broker order type, price, quantity step or fabricate fill.

`DesiredCompletionHorizon` is not permission to violate broker/market/security/price-truth requirements.

Effect confirmed only from reconciled position state meeting FLAT for the targeted episode/scope.

## 11. Action: HOLD_PROMOTION

### Allowed target

APP-TRD or APP-SIM candidate/evolution workflow with exact `CandidateArtifactId` required.

### Parameters

```text
struct HoldPromotionParameters {
  HoldReasonClass ReasonClass;
}

HoldReasonClass =
  SAFETY_EVIDENCE_INSUFFICIENT
  | INTEGRITY_INVESTIGATION
  | PROTECTION_INCIDENT_ACTIVE
  | VALIDATION_CREDIBILITY_FAILURE
```

This action can block promotion/adoption processing for the candidate. It cannot accept/reject the candidate permanently, modify its bytes or become Owner approval authority.

## 12. Provider Action: ISOLATE_PROVIDER_ROUTE

This is carried on the Guardian -> FSAPMA provider-protection family, not the Trading directive family.

Required scope:

```text
TargetApplicationId = APP-PMA
ProviderId
ProviderRouteId?                 // if whole-provider isolation, absent only when policy/authority explicitly covers provider
```

Parameters:

```text
struct IsolateProviderRouteParameters {
  IsolationMode Mode = IMMEDIATE_EXCLUDE_FROM_ELIGIBILITY;
}
```

FSAPMA excludes the targeted provider/route and re-runs normal route selection among other eligible routes. Guardian does not pick the replacement.

## 13. Provider Action: RESTRICT_PROVIDER_ROUTE

Parameters:

```text
struct RestrictProviderRouteParameters {
  ProviderRestrictionKind Kind;
}

ProviderRestrictionKind =
  NO_NEW_OPERATIONAL_REQUESTS
  | VALIDATION_ONLY
  | REQUIRE_MULTI_SOURCE_CONFIRMATION
```

`VALIDATION_ONLY` means route may be used only as non-canonical corroboration according to P-LSA-05 policy, not as sole canonical operational Data Product source.

## 14. Provider Action: REQUIRE_MULTI_SOURCE_VALIDATION

Target APP-PMA + exact Provider/DataProduct/Instrument scope as applicable.

Parameters:

```text
MinimumIndependentSources >= 2
```

Actual sources must pass FSAPMA upstream-lineage independence checks. Guardian cannot count brands as independent.

If enough independent sources are unavailable, the Data Product becomes unavailable/degraded according to quality policy; FSAPMA does not lower the requirement.

## 15. Resource-Priority Semantics — Not A Guardian Protection Directive Action

R2 review found that `REQUEST_RESOURCE_PRIORITY` must not be interpreted as a Guardian command granting/reordering Foundation resource authority.

Controlling v1 rule:

```text
REQUEST_RESOURCE_PRIORITY IS NOT A PROTECTION DIRECTIVE ACTION.
```

Guardian reports current protection/crisis resource need through the `GRD-RSC-001` Resource Demand Report defined in files 11/12/12A when APP-RSC exists.

That report includes:

- active protection obligation refs;
- minimum-safe need;
- consequence-of-starvation evidence;
- reclaimability/degradation state.

APP-RSC applies its governed resource algorithm; Foundation technical criticality/resource authority remains Foundation-owned.

If APP-RSC is not accepted/available, Guardian remains within its current effective allocation and local minimum-safe degradation; it does not seize resources from peer Applications.

Any earlier generic list containing `REQUEST_RESOURCE_PRIORITY` as a Guardian action is superseded by this exact route separation.

## 16. Release Semantics

Release uses the accepted `protection-release` family and references exact active/superseded directive(s).

Payload includes:

```text
ProtectionReleaseId
GuardianIncidentId
ReleasedDirectiveIds[]
TargetApplicationId/Scope
ReleaseMode = FULL | PARTIAL
RemainingRestrictionRefs[]
AuthorityReference
PolicyVersion
EffectiveFrom
ReasonCodes[]
EvidenceRefs[]
```

No release by silence/timer alone.

For PARTIAL release, remaining restrictions are explicit. Target recalculates effective local restrictions as intersection of all still-valid Guardian/Risk/authority rules.

## 17. Directive Effect Outcome Schema Tightening

Target outcome for any action includes:

```text
ProtectionDirectiveId
Action
TargetScopeDigest
OutcomeState
AppliedParameterDigest
CurrentEffectiveRestriction/Position/OrderStateRef?
UnresolvedTargetRefs[]
ReasonCodes[]
EvidenceRefs[]
```

For exposure ceiling, effect confirmation records the effective local ceiling and current exposure state.

For cancel, effect confirmation records terminal/reconciled targeted order set.

For exit, effect confirmation records reconciled FLAT position state.

`AppliedParameterDigest` must match the received directive parameters; mismatch = integrity/reconciliation failure.

## 18. Required / Forbidden Parameter Matrix

| Action | Required | Forbidden / ignored not allowed |
|---|---|---|
| RESTRICT_NEW_RISK | exact Trading scope | numeric exposure ceiling |
| REDUCE_ALLOWED_EXPOSURE | CeilingKind, CeilingValue (+Currency if absolute) | broker/provider route selection |
| SUSPEND_STRATEGY_SCOPE | StrategyVersionId, Treatment | provider fields |
| SUSPEND_INSTRUMENT_SCOPE | MarketId, InstrumentId, Treatment | implicit cancel/exit |
| SUSPEND_MARKET_SCOPE | MarketId, Treatment | provider isolation |
| CANCEL_OPEN_ORDERS | exact cancel scope | fill/position-success assertion |
| EXIT_POSITION_SCOPE | account + position/instrument, Urgency | broker order-type/price command |
| HOLD_PROMOTION | CandidateArtifactId, ReasonClass | candidate acceptance/rejection |
| ISOLATE_PROVIDER_ROUTE | ProviderId/Route scope | replacement provider identity |
| RESTRICT_PROVIDER_ROUTE | Provider scope, Kind | Trading order semantics |
| REQUIRE_MULTI_SOURCE_VALIDATION | source/data scope, min sources>=2 | source brands counted without lineage validation |

Extra action-specific fields not declared in the selected schema cause validation failure; consumers do not silently ignore potentially material unknown parameters.

## 19. Verification Families

Verifier SHALL cover:

1. discriminated-union schema exactness;
2. action/target-Application compatibility;
3. required/forbidden fields;
4. invalid percent/absolute ceiling units;
5. Guardian ceiling can only tighten local effective ceiling;
6. suspend does not imply cancel/exit;
7. cancel request != effect;
8. exit request != fill/flat truth;
9. hold promotion != adoption authority;
10. provider isolate does not choose replacement;
11. multi-source count uses independent lineage;
12. resource-priority request cannot be encoded as Guardian authority command;
13. release is explicit with remaining restrictions;
14. AppliedParameterDigest mismatch rejected;
15. unknown action/version/extra material parameter fails closed.

## 20. Finding Disposition

```text
RT-GRD-001 = REMEDIATED_AT_DESIGN_CANDIDATE_LEVEL
GUARDIAN_ACTION_PARAMETERS = SCHEMA_CLOSED
REQUEST_RESOURCE_PRIORITY_AS_COMMAND = REMOVED
COMMAND_EFFECT = ACTION_SPECIFIC_AND_RECONCILED
```
