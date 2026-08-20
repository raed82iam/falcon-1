# P0-I - Guardian Protection, Crisis, Survival and Recovery

**Status:** `OWNER_DIRECTED_INTEGRATED_REWRITE_CANDIDATE / NOT_OWNER_ACCEPTED / NOT_CLOSED`
**Implementation Authority:** `NOT_GRANTED`
**Runtime Authority:** `NOT_GRANTED`

## 1. Purpose

P0-I defines the Falcon Trading Guardian Application as the independent owner of Trading protection/crisis scope. It defines how Falcon detects, scopes, restricts, survives, contains, coordinates emergencies, protects open exposure, proves recovery and handles Guardian self-failure without stealing truth or authority from FSAPMA, Trading Execution/Reconciliation, Unified Risk, APP-RSC, Foundation or the Owner.

## 2. Canonical ownership

```text
DATA_QUALITY_SCOPE                -> FSAPMA / Data Product owner
EXECUTION_AMBIGUITY_SCOPE         -> Trading Execution / Reconciliation
TRADING_RISK_SCOPE                -> Unified Risk
FSATS_RESOURCE_COORDINATION_SCOPE -> APP-RSC
FOUNDATION_RESOURCE_AUTHORITY     -> Foundation
FOUNDATION_CONTAINMENT_SCOPE      -> Foundation
PROTECTION_CRISIS_SCOPE           -> Falcon Trading Guardian Application
OWNER_GOVERNANCE_SCOPE            -> Project Owner / valid governance
```

The smallest-safe-scope rule is containment behavior, not authority transfer.

## 3. Guardian topology

Guardian is one independent Falcon Application with:

```text
MSA = 1
LSA = 4
CSA = 1
```

Its awareness evaluates Guardian itself. Operational Guardian controllers execute only accepted protection responsibilities and are not Awareness tiers.

## 4. Guardian responsibility

Guardian owns, within exact delegated scope:

- protection-state classification;
- cross-domain hazard evidence correlation;
- smallest-safe Trading protection scope;
- no-new-exposure/restriction/halt/protective outcomes;
- survival-mode direction;
- governed protection playbooks;
- recovery admission/proof requirements;
- Guardian self-failure handling;
- independent challenge of Trading Risk/execution/data/resource conditions for protection purposes;
- incident evidence and escalation;
- protection command outcome reconciliation.

Guardian does not own underlying source-domain truth.

## 5. Canonical state model

```text
NORMAL
WARNING
RESTRICTED
SAFE_MODE
RECOVERY
```

A concrete implementation may refine states without weakening these semantics. Transitions are evidence-driven, scoped, attributable and reconstructable.

`NORMAL` is never the default for unknown Guardian health or unresolved hazard.

## 6. Trigger taxonomy

Potential triggers include:

- Risk limit/protection breach;
- missing/rejected/disappeared protective order;
- broker/execution ambiguity threatening protection;
- stale/unavailable data affecting safe management;
- provider degradation affecting protected exposure;
- broker-account/credential revocation;
- security/authority integrity failure;
- resource pressure threatening protection/reconciliation;
- Foundation/lifecycle isolation affecting safe operation;
- cross-domain correlated failure;
- Guardian self-health failure;
- repeated/cumulative anomalies;
- Owner/governance protection direction.

A normal loss, expected market variance or valid stop execution is not automatically crisis.

## 7. Detection versus Guardian crisis scope

Domains detect and scope their own failures. Guardian consumes attributable evidence and independently determines protection impact.

```text
DOMAIN_FAILURE_SCOPE != GUARDIAN_PROTECTION_CRISIS_SCOPE
```

Examples:

- one FSAPMA instance fails but an eligible alternative exists -> FSAPMA may recover locally, Guardian crisis may be unnecessary;
- deferrable workload pressure only -> APP-RSC/constituent degradation may resolve without Guardian crisis;
- stale data threatens protection of open exposure -> Guardian may restrict affected Trading scope while FSAPMA remains data-truth owner;
- ambiguous broker state threatens exposure truth -> Guardian may block new actions while Execution/Reconciliation remains broker-truth owner.

## 8. Scoped protection lattice

Guardian targets the smallest safe scope supported by evidence and dependency impact. Scope dimensions may include exact broker account, market, instrument, strategy, order/action class, broker route, provider-dependent consumer path, Trading Application or broader FSATS Trading protection scope where evidence requires.

A local fault cannot be globalized for convenience; a broad/common failure cannot be falsely kept local.

## 9. Directive identity and epochs

Every authoritative Guardian directive binds as applicable:

- directive identity;
- Guardian identity/authority basis;
- exact target scope;
- protection state/action;
- reason/evidence identities;
- effective time;
- expiry/validity if bounded;
- control epoch;
- supersession/revocation identity;
- correlation/causation;
- recovery conditions.

Newer valid epochs invalidate stale opening work. Superseded directives cannot be replayed as current authority.

## 10. Guardian protection outcomes

Depending on policy/evidence Guardian may issue:

- warning/observation;
- no-new-exposure;
- no-new-orders;
- strategy/market/account/instrument restriction;
- protective-only operation;
- exposure-reduction request through valid Trading/Risk/Execution paths;
- SAFE_MODE;
- hold pending reconciliation;
- staged recovery.

Guardian never invents broker, position or data truth to make a protection decision easier.

## 11. Guardian and Unified Risk

Unified Risk owns Trading Risk decisions. Guardian may consume Risk evidence, add an independent protection restriction and restrict more tightly where broader protection evidence warrants it.

Guardian cannot rewrite Risk values, tune Risk algorithms, claim Risk ownership or remove a valid Risk block because Guardian state improves.

```text
GUARDIAN_PROTECTION != UNIFIED_RISK_DECISION
```

## 12. Guardian and Execution/Reconciliation

Execution/Reconciliation owns broker submission/ACK/fill/cancel/replace truth, position lifecycle and ambiguity resolution.

Guardian may restrict based on ambiguity but cannot resolve ambiguity by assumption.

```text
GUARDIAN_RESTRICTION_ON_AMBIGUITY != BROKER_OUTCOME_TRUTH
```

## 13. Guardian and FSAPMA

FSAPMA owns provider/Data Product truth, degradation, entitlement, continuity and failover semantics. Guardian may consume readiness/degradation evidence and issue bounded provider-use protection directives where accepted, but it does not choose provider credentials, route instances, correction logic or normal failover.

## 14. Guardian and APP-RSC

The historical direct Guardian-to-TARC resource path is superseded by the APP-RSC fifth-Application model.

Guardian may publish attributable protection urgency, minimum-safe requirement and degradation-consequence evidence into the APP-RSC resource path. APP-RSC may consider it under current policy and Foundation-protected floors.

Guardian cannot submit a Foundation resource request, even as emergency/break-glass authority.

```text
GUARDIAN_PROTECTION_URGENCY -> APP_RSC_RESOURCE_EVIDENCE
APP_RSC -> INTERNAL_FSATS_COORDINATION
APP_RSC -> FOUNDATION_RESIDUAL_REQUEST_ONLY_IF_PROVEN_AND_RUNTIME_AVAILABLE
FOUNDATION -> AUTHORITATIVE_RESOURCE_OUTCOME
```

```text
GUARDIAN_URGENCY != FOUNDATION_TECHNICAL_CRITICALITY
GUARDIAN_RESOURCE_SIGNAL != FOUNDATION_RESOURCE_GRANT
```

## 15. Guardian and Foundation

Foundation owns generic platform containment, lifecycle, security and total-resource authority. Guardian cannot command Foundation internals or reinterpret Foundation containment as Trading business truth.

```text
FOUNDATION_CONTAINMENT != GUARDIAN_CRISIS_AUTHORITY
GUARDIAN_CRISIS != FOUNDATION_INTERNAL_CONTROL_AUTHORITY
```

Foundation evidence may cause Guardian to alter Trading protection scope.

## 16. Guardian and Owner/Web controls

Owner/customer-facing requests remain separate authority sources and must be transported through governed contracts. Ordinary user resume cannot override Guardian restriction. A Shared Web button/click is a request, not proof of accepted/completed protection action.

```text
UI_CLICK != AUTHORIZATION
REQUEST_SENT != ACTION_ACCEPTED != ACTION_COMPLETED
```

Owner governance may change Guardian policy/design through valid governance, but runtime command semantics remain explicit rather than inferred.

## 17. Protection Playbook Registry

Guardian uses a governed Playbook Registry for material scenarios. Each playbook defines:

- playbook identity/version;
- trigger evidence;
- target scope-resolution rules;
- initial state/action;
- dependencies;
- required domain-owner interactions;
- Minimum Viable Protection Set;
- fallback ladder;
- escalation criteria;
- evidence/journal requirements;
- stop/hold/restrict conditions;
- recovery prerequisites;
- staged re-admission;
- verification/fault-injection fixtures.

A playbook is executable/testable design, not permission to bypass other owners.

## 18. MVPS - Minimum Viable Protection Set

For each material open-exposure scenario Falcon identifies the minimum capabilities/resources required for safe protection. MVPS may include:

- sufficiently authoritative position/exposure truth;
- current Risk/protection restrictions;
- execution/reconciliation capability;
- required operational data or explicitly accepted degraded substitute;
- required broker-account/credential route;
- minimum technical resources;
- evidence/journal capability.

If actual resources cannot sustain MVPS, Guardian escalates protection state while APP-RSC/Foundation remain resource authority paths.

## 19. EPCP - Emergency Protection Coordination Profile

EPCP defines emergency coordination without authority merger. It identifies Guardian protection scope, Risk role, Execution/Reconciliation role, FSAPMA data role, APP-RSC resource-coordination role, Foundation platform/resource role, Owner/governance role, communication/evidence paths, degraded dependencies and recovery order.

```text
EMERGENCY_COORDINATION != SHARED_UNBOUNDED_AUTHORITY
```

## 20. Protection fallback ladder

When ideal protection is unavailable, use the safest valid path based on truthful state, potentially:

1. prevent new exposure;
2. preserve/reconcile existing protective orders;
3. attempt a valid alternative protective action within the same authorized broker-account route;
4. reduce exposure only when execution truth and Risk/protection rules support it;
5. hold/reconcile when ambiguity creates over-close/reversal risk;
6. suspend affected scope;
7. escalate for Owner/operational intervention.

## 21. No blind liquidation

```text
OPEN_EXPOSURE + UNCERTAINTY != SELL_EVERYTHING_BLINDLY
```

Before protective reduction/closure, Falcon establishes enough execution/position truth to avoid unsafe duplication, reversal or over-close, or uses a separately accepted emergency mechanism designed for that exact uncertainty.

```text
UNKNOWN_BROKER_OUTCOME != POSITION_FLAT
CANCEL_REQUESTED != CANCELED
EXIT_REQUESTED != EXIT_COMPLETED
```

## 22. Guardian self-failure

Guardian unavailable/unhealthy/uncertain cannot be represented as NORMAL. Self-failure handling defines health evidence, loss detection, affected scope, fail-safe restriction, preserved independent Risk/execution protections, stale-directive handling, restart/recovery evidence and re-admission criteria.

No sibling Application or LSA inherits Guardian authority automatically.

## 23. Recovery and release

Recovery is not clearing a flag. It requires evidence that the trigger is resolved/bounded, current state reconstructed, execution/position truth reconciled, required data sufficiently restored, Risk current, resource/technical readiness sufficient, stale work invalidated, security/credential trust restored where affected and no newer Owner/Guardian/Risk restriction exists.

Conceptual sequence:

```text
SAFE / RESTRICTED
-> RECOVERY_VALIDATION
-> LIMITED_RE_ADMISSION
-> OBSERVE
-> NORMAL_ONLY_WITH_EVIDENCE
```

Generic Foundation containment/recovery/release authority remains Foundation-owned. Current Foundation Stage 9/FCR-0076/FCR-0082 state must be refreshed where material. AI/FSA-specific containment/revival remains subject to FCR-0012/FCR-0030 Stage 13.

```text
TIMEOUT != RELEASE_AUTHORITY
RESTART != TRUST_RESTORATION
```

## 24. Notification separation

```text
ALERT_SENT != ALERT_DELIVERED != HUMAN_ACK != PROTECTION_EFFECTIVE
```

Notification outage cannot prevent independently valid protective action.

## 25. Fault injection

Controlled non-Live challenge includes disappearing protective order, ambiguous broker order, broker-account/credential revocation, provider failure, stale data, resource starvation, Guardian restart, stale directive replay, overlapping scopes, false recovery, halted market, notification outage and conflicting truth.

Fault injection never authorizes Live harm.

## 26. Evidence preservation

Every material Guardian action preserves trigger evidence, authority source, scope, command identity, correlation/causation, protection epoch, observed outcome and correction/supersession lineage.

## 27. Failure/degraded truth

```text
UNKNOWN != SAFE
STALE != CURRENT
PARTIAL != COMPLETE
```

Uncertain truth produces bounded fail-safe behavior, not fabricated certainty.

## 28. Foundation/FCR dependencies

Relevant current dependencies are refreshed from live FCRs before implementation. These include governed Guardian command transport where applicable, P0-F event/evidence delivery, FCR-0009 QoS where required, APP-RSC/FCR-0010/FCR-0031 resource runtime boundaries, FCR-0016 canonical artifact consumption, and Stage 9/Stage 13 recovery/AI control-plane dependencies.

## 29. Explicit non-authority

Guardian SHALL NOT own provider/Data Product truth, broker/execution/position truth, Unified Risk, APP-RSC resource coordination, Foundation containment/resource authority, infer Owner approval, treat notification as protection proof, blind-liquidate unresolved exposure, globalize local faults without evidence or keep common failures artificially local.

## 30. Invariants

```text
PROTECTION_CRISIS_SCOPE = GUARDIAN_OWNED
DOMAIN_FAILURE_SCOPE != GUARDIAN_PROTECTION_CRISIS_SCOPE
DATA_QUALITY_SCOPE = FSAPMA_OWNED
EXECUTION_AMBIGUITY_SCOPE = EXECUTION_RECONCILIATION_OWNED
RISK_SCOPE = UNIFIED_RISK_OWNED
FSATS_RESOURCE_COORDINATION_SCOPE = APP_RSC_OWNED
FOUNDATION_CONTAINMENT_SCOPE = FOUNDATION_OWNED
GUARDIAN_DIRECT_FOUNDATION_RESOURCE_REQUEST = PROHIBITED
GUARDIAN_UNAVAILABLE != NORMAL
ALERT_DELIVERED != PROTECTION_EFFECTIVE
BLIND_LIQUIDATION = PROHIBITED_WHEN_IT_CAN_CREATE_UNCONTROLLED_HARM
RECOVERY_WITHOUT_PROOF = PROHIBITED
```

## 31. Forbidden interpretations

Invalid: detector owns crisis; Guardian requests Foundation resources because emergency; APP-RSC declares Guardian SAFE_MODE; FSAPMA declares Trading crisis because data failed; Foundation containment decides Trading positions; Guardian invents position truth; Guardian restart means NORMAL; alert completes protection; uncertainty means blind liquidation.

## 32. Mandatory scenarios

Test missing/rejected/disappeared protective order; ambiguous broker state; broker-account credential revoked with open position; isolated account failure; common broker outage; compromised credential during emergency; no valid execution path; Guardian restart during SAFE_MODE; stale directive replay; overlapping directives; insufficient MVPS resources; notification outage; false recovery; halted market; liquidation reversal/over-close risk; Guardian direct Foundation-resource request attempt; APP-RSC attempting Guardian crisis authority; FSAPMA local failure incorrectly globalized; and Foundation containment incorrectly treated as Trading position decision.

## 33. Exit gates

```text
PROTECTION_SCOPE_OWNER = GUARDIAN_ONLY
DOMAIN_SCOPE_OWNERSHIP_COLLISIONS = 0
PLAYBOOK_REQUIRED_FAMILIES = COVERED
MVPS = EXPLICIT
EPCP = EXPLICIT
UNBOUNDED_GUARDIAN_ACTION = 0
BLIND_LIQUIDATION_PATHS = 0
GUARDIAN_SELF_FAILURE_AS_NORMAL = 0
UNJUSTIFIED_SCOPE_GLOBALIZATION = 0
DIRECT_GUARDIAN_FOUNDATION_RESOURCE_REQUEST_PATHS = 0
RECOVERY_WITHOUT_PROOF = 0
```

## 34. Non-grant

Acceptance of P0-I would establish Guardian protection/crisis design only. It would not activate Guardian runtime routes, broker/provider connectivity, Foundation resource request authority, Paper, Shadow, Tiny-Live, Live or deployment.