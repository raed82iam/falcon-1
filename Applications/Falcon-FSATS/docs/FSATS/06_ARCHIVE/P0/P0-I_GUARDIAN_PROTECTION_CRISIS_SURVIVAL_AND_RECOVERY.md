# P0-I — Guardian Protection, Crisis, Survival and Recovery

**Status:** `PROPOSED / OWNER_REVIEW_CANDIDATE / NOT_OWNER_ACCEPTED / NOT_CLOSED`  
**Scope:** `P0-I only`  
**Implementation Authority:** `NOT GRANTED`

---

## 1. Purpose

P0-I defines the Falcon Trading Guardian Application as the independent owner of Trading protection/crisis scope and establishes how Falcon restricts, survives, contains, recovers, and proves recovery without stealing domain ownership from FSAPMA, Unified Risk, Execution, TARC, Foundation, or the Owner.

---

## 2. Canonical Ownership

```text
DATA_QUALITY_SCOPE           -> FSAPMA / Data Product owner
EXECUTION_AMBIGUITY_SCOPE    -> Trading Execution / Reconciliation
RISK_SCOPE                   -> Unified Risk
RESOURCE_PRESSURE_SCOPE      -> TARC for Trading Application resources
FOUNDATION_CONTAINMENT_SCOPE -> Foundation
PROTECTION_CRISIS_SCOPE      -> Falcon Trading Guardian Application
OWNER_GOVERNANCE_SCOPE       -> Project Owner / valid governance
```

The smallest-safe-scope principle is a containment rule, not a transfer of authority.

---

## 3. Guardian Responsibility

Guardian owns Trading protection/crisis semantics, including as applicable:

- protection-state classification;
- correlation of cross-domain hazard evidence;
- selection of the smallest safe Trading protection scope;
- scoped no-new-exposure/restriction/halt/protective outcomes;
- survival-mode direction within Guardian authority;
- protection playbooks;
- recovery admission/proof requirements;
- Guardian self-failure handling;
- independent challenge of Trading risk/execution/data/resource conditions for protection purposes.

Guardian does not own the underlying business truth of the source domain.

---

## 4. Guardian State Model

Canonical states:

```text
NORMAL
WARNING
RESTRICTED
SAFE_MODE
RECOVERY
```

State transitions SHALL be evidence-driven, scoped, attributable, and reconstructable.

`NORMAL` is not a default for unknown Guardian health or unresolved hazard.

---

## 5. Trigger Taxonomy

Potential protection triggers include, where material:

- Risk limit/protection breach;
- missing/rejected/disappeared protective order;
- broker/execution ambiguity threatening protection;
- stale/unavailable operational data affecting safe management;
- provider degradation affecting protected exposure;
- account/credential revocation;
- security/authority integrity failure;
- resource pressure threatening protection/reconciliation;
- Foundation/lifecycle isolation affecting safe operation;
- cross-domain correlated failure;
- Guardian self-health failure;
- repeated or cumulative anomalies;
- Owner/governance protection direction.

A normal loss, valid stop-loss execution, or expected trading variability is not automatically a crisis.

---

## 6. Detection vs Guardian Crisis Scope

Domains detect and scope their own failures.

Guardian consumes attributable evidence and independently determines protection impact.

```text
DOMAIN_FAILURE_SCOPE != GUARDIAN_PROTECTION_CRISIS_SCOPE
```

Examples:

- one FSAPMA API instance fails but a healthy authorized alternative exists: FSAPMA may recover locally; Guardian crisis may be unnecessary;
- discovery workload resource pressure only: TARC may shed discovery locally; Guardian crisis may be unnecessary;
- data freshness loss threatens protection of open positions: Guardian may restrict the affected Trading scope based on FSAPMA evidence;
- ambiguous broker state threatens exposure truth: Guardian may restrict new actions while Execution/Reconciliation remains owner of broker truth.

Guardian does not rewrite the domain evidence to make its protection decision easier.

---

## 7. Scoped Protection Lattice

Guardian restrictions SHALL target the smallest safe Trading scope supported by evidence and dependency impact.

Potential scope dimensions include:

- user;
- account;
- market;
- instrument;
- strategy;
- order/action class;
- broker/account route;
- provider-dependent consumer path;
- Trading Application;
- broader FSATS Trading protection scope only when evidence requires it.

A local problem SHALL NOT be globalized for convenience.

A broad/common failure SHALL NOT be falsely kept local.

---

## 8. Guardian Directive Identity and Epochs

Every authoritative Guardian directive SHALL bind as applicable:

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

Newer valid epochs invalidate stale opening work as applicable.

Old directives SHALL not be replayed after supersession.

---

## 9. Guardian Protection Outcomes

Depending on accepted policy and evidence, Guardian may impose outcomes such as:

- warning/observation;
- no-new-exposure;
- no-new-orders;
- strategy/market/account/instrument restriction;
- protective-only operation;
- exposure-reduction request through valid Trading/Execution/Risk paths;
- SAFE_MODE;
- hold pending reconciliation;
- recovery staging.

Guardian SHALL NOT invent broker truth, position truth, or data truth while issuing protection outcomes.

---

## 10. Guardian and Unified Risk

Unified Risk owns Trading Risk business decisions and Risk restrictions.

Guardian may:

- consume Risk evidence;
- add an independent protection restriction;
- restrict Trading more tightly when broader protection evidence warrants it.

Guardian SHALL NOT:

- rewrite Risk model values;
- tune Risk algorithms;
- claim Risk ownership;
- remove a valid Risk block merely because Guardian state improves.

```text
GUARDIAN_PROTECTION != UNIFIED_RISK_DECISION
```

---

## 11. Guardian and Execution/Reconciliation

Execution/Reconciliation owns:

- broker order state;
- submission/ACK/fill/cancel truth;
- position lifecycle truth;
- execution ambiguity resolution.

Guardian may restrict Trading based on execution ambiguity but SHALL NOT resolve ambiguity by assumption.

```text
GUARDIAN_RESTRICTION_ON_AMBIGUITY != BROKER_OUTCOME_TRUTH
```

---

## 12. Guardian and FSAPMA

FSAPMA owns provider/Data Product truth, degradation, entitlement, continuity, and provider failover semantics.

Guardian may consume FSAPMA readiness/degradation evidence and impose Trading restrictions.

Guardian SHALL NOT choose provider internals, credential instances, Data Product correction logic, or provider failover mechanisms.

---

## 13. Guardian and TARC

TARC owns Trading Application resource control and the sole Trading-side Foundation resource request role.

Guardian may send resource urgency/need/protection evidence to TARC.

Guardian SHALL NOT request Trading resources directly from Foundation, including emergency/break-glass conditions.

```text
GUARDIAN_PROTECTION_URGENCY -> TARC_EVIDENCE
TARC -> FOUNDATION_RESOURCE_REQUEST_WHEN_CAPABILITY_EXISTS
FOUNDATION -> FINAL_RESOURCE_OUTCOME
```

TARC remains resource owner; Guardian remains protection/crisis owner.

---

## 14. Guardian and Foundation

Foundation owns Foundation containment/platform/security/lifecycle/resource decisions.

Guardian SHALL NOT command Foundation internals or reinterpret Foundation containment as Trading business crisis authority.

Foundation containment evidence may cause Guardian to alter Trading protection scope.

```text
FOUNDATION_CONTAINMENT != GUARDIAN_CRISIS_AUTHORITY
GUARDIAN_CRISIS != FOUNDATION_INTERNAL_CONTROL_AUTHORITY
```

---

## 15. Guardian and Owner/User Controls

User/Owner commands remain separate authority sources.

Ordinary user resume SHALL NOT override Guardian restriction.

Owner trading commands SHALL NOT silently weaken independent Guardian protection unless an explicit higher-order governance rule grants that exact relationship.

Owner may exercise governance authority over Guardian design/policy through valid governance, but runtime business-command semantics remain explicitly defined rather than inferred.

---

## 16. Guardian Protection Playbook Registry

Guardian SHALL use a governed Playbook Registry for material protection scenarios.

Each playbook SHALL define:

- playbook identity/version;
- trigger evidence requirements;
- target scope-resolution rules;
- initial protection state/action;
- dependencies;
- required domain-owner interactions;
- minimum viable protection/survival capabilities;
- fallback ladder;
- escalation criteria;
- evidence/journal requirements;
- stop/hold/restrict conditions;
- recovery prerequisites;
- staged re-admission;
- verification/fault-injection fixtures.

A playbook is an executable/testable design specification, not automatic authority to bypass other owners.

---

## 17. MVPS — Minimum Viable Protection Set

For each material open-exposure scenario, Falcon SHALL identify the minimum capabilities/resources required to continue protection safely.

MVPS may include as applicable:

- authoritative enough position/exposure truth;
- current Risk/protection restrictions;
- execution/reconciliation capability;
- required operational data or explicit degraded substitute already accepted for protection;
- required credential/account route;
- minimum technical resources;
- evidence/journal capability.

When resources are insufficient even for MVPS, Guardian escalates protection state while TARC/Foundation handle resource authority.

---

## 18. EPCP — Emergency Protection Coordination Profile

EPCP defines how independent owners coordinate under emergency without merging authority.

It SHALL identify:

- Guardian protection scope;
- Risk restriction role;
- Execution/Reconciliation role;
- FSAPMA data role;
- TARC resource role;
- Foundation platform/resource role;
- Owner/governance role;
- communication/evidence paths;
- degraded dependencies;
- recovery order.

```text
EMERGENCY_COORDINATION != SHARED_UNBOUNDED_AUTHORITY
```

---

## 19. Protection Fallback Ladder

When the ideal protective action is unavailable, Guardian/Trading SHALL use the safest valid available path based on truthful state.

The ladder may include:

- prevent new exposure;
- preserve/reconcile existing protective orders;
- attempt valid alternative protective order/action within same authorized account/route;
- reduce exposure only when execution truth and Risk/protection rules support it;
- hold/reconcile when action ambiguity creates reversal/over-close risk;
- suspend affected scope;
- escalate for Owner/operational intervention.

Blind liquidation is prohibited when current truth is insufficient and liquidation could over-close, reverse, duplicate, or otherwise worsen exposure.

---

## 20. No Blind Liquidation Rule

```text
OPEN_EXPOSURE + UNCERTAINTY != SELL_EVERYTHING_BLINDLY
```

Before protective reduction/closure, Falcon must establish enough authoritative execution/position truth to avoid materially unsafe duplicate/reversal/over-close behavior, or use a separately accepted emergency mechanism designed for the exact uncertainty.

Protection prioritizes capital and future choice, not action for appearance.

---

## 21. Guardian Self-Failure

Guardian unavailable/unhealthy/uncertain SHALL NOT be represented as `NORMAL`.

Guardian self-failure playbook SHALL define:

- health evidence;
- loss-of-Guardian detection;
- affected scope;
- fail-safe restriction state;
- preserved independent Risk/execution protections;
- restart/recovery evidence;
- stale directive handling;
- re-admission criteria.

No sibling Application/LSA automatically inherits Guardian authority.

---

## 22. Recovery

Recovery is not simply clearing a fault flag.

Recovery SHALL require evidence that:

- triggering hazard is resolved or bounded;
- current state is reconstructed;
- execution/position truth is reconciled;
- data/provider truth is sufficiently restored where required;
- Risk is current;
- resource/technical readiness is sufficient;
- stale directives/work are invalidated;
- security/credential trust is restored where affected;
- no newer Owner/Guardian/Risk restriction exists;
- observation/stabilization period is satisfied where appropriate.

Recovery SHALL be staged:

```text
SAFE / RESTRICTED
 -> RECOVERY_VALIDATION
 -> LIMITED_RE-ADMISSION
 -> OBSERVE
 -> NORMAL_ONLY_WITH_EVIDENCE
```

---

## 23. Notification Separation

Communication/notification is advisory evidence of awareness to humans or systems, not proof that protection action succeeded.

```text
ALERT_SENT != ALERT_DELIVERED != HUMAN_ACK != PROTECTION_EFFECTIVE
```

Notification outage SHALL NOT prevent independently valid protective action.

---

## 24. Fault Injection

Guardian design SHALL be regularly challenged through controlled non-Live fault injection covering material scenarios such as:

- disappearing protective order;
- ambiguous broker order;
- account credential revocation;
- provider failure;
- stale data;
- resource starvation;
- Guardian restart;
- stale directive replay;
- overlapping scopes;
- false recovery;
- halted market;
- notification outage;
- split evidence/conflicting truth.

Fault injection never authorizes Live harm.

---

## 25. Foundation / FCR Dependencies

- FCR-0004: governed Guardian protection-command route, open / Waiting On FOUNDATION;
- FCR-0006: event/evidence/replay semantics where wider end-to-end capability remains open;
- FCR-0007/FCR-0010: TARC resource request/pressure runtime, not Guardian direct request;
- FCR-0009: transport deadline/QoS behavior where required;
- current Stage 5 communication/event/crypto capabilities apply within exact accepted scopes.

Guardian design may be accepted while runtime integration remains blocked by unresolved Foundation capabilities.

---

## 26. Explicit Non-Authority

Guardian SHALL NOT:

- own provider/Data Product truth;
- own broker/execution/position truth;
- tune Unified Risk;
- request Trading resources directly from Foundation;
- own Foundation containment;
- infer Owner approval;
- use notification delivery as proof of protection;
- blind-liquidate under unresolved execution truth;
- globalize local problems without evidence;
- keep a common failure artificially local.

---

## 27. Invariants

```text
PROTECTION_CRISIS_SCOPE = GUARDIAN_OWNED
DOMAIN_FAILURE_SCOPE != GUARDIAN_PROTECTION_CRISIS_SCOPE
DATA_QUALITY_SCOPE = FSAPMA_OWNED
EXECUTION_AMBIGUITY_SCOPE = EXECUTION_RECONCILIATION_OWNED
RISK_SCOPE = UNIFIED_RISK_OWNED
RESOURCE_PRESSURE_SCOPE = TARC_OWNED
FOUNDATION_CONTAINMENT_SCOPE = FOUNDATION_OWNED
GUARDIAN_DIRECT_TRADING_RESOURCE_REQUEST = PROHIBITED
GUARDIAN_UNAVAILABLE != NORMAL
ALERT_DELIVERED != PROTECTION_EFFECTIVE
BLIND_LIQUIDATION = PROHIBITED_WHEN_IT_CAN_CREATE_UNCONTROLLED_HARM
RECOVERY_WITHOUT_PROOF = PROHIBITED
```

---

## 28. Forbidden Interpretations

Invalid interpretations include:

- “whoever detects the problem owns the crisis”;
- “Guardian can request Foundation resources because it is an emergency”;
- “TARC can declare SAFE_MODE because resource pressure caused the problem”;
- “FSAPMA can declare Trading crisis because data failed”;
- “Foundation containment means Foundation decides Trading positions”;
- “Guardian can invent position truth to protect faster”;
- “if Guardian restarts, return to NORMAL”;
- “send an alert and the protection obligation is complete”;
- “uncertain position means liquidate blindly”.

---

## 29. Mandatory Scenarios

At minimum test:

- missing/rejected/disappeared protective order;
- ambiguous broker order state;
- one user credential revoked with open position;
- one user failure remains local;
- common broker outage broadens scope correctly;
- compromised credential during emergency;
- no valid execution path with open exposure;
- Guardian restart during SAFE_MODE;
- old directive replay after new epoch;
- overlapping local/global directives;
- insufficient resources even for MVPS;
- notification outage during protective action;
- false recovery;
- halted market preventing closure;
- blind liquidation creating reversal/over-close risk;
- Guardian direct Foundation resource request attempt;
- TARC attempting Guardian crisis classification;
- FSAPMA local failure incorrectly globalized by Guardian;
- Foundation containment incorrectly treated as Trading crisis decision.

---

## 30. Exit Gates

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
DIRECT_GUARDIAN_TRADING_RESOURCE_REQUEST_PATHS = 0
RECOVERY_WITHOUT_PROOF = 0
FCR0004_RUNTIME_STATE = EXPLICIT
```

---

## 31. Next Authorized Gate

P0-I acceptance would establish Guardian protection/crisis design only. It would not activate Guardian runtime command routes, resource-request runtime, broker/provider connectivity, Paper, Tiny Live, Live, or deployment.
