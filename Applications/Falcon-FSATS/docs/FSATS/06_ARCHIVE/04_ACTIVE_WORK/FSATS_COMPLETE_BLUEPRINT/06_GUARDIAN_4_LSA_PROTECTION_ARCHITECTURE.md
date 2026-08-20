# FSATS Complete Blueprint — Falcon Trading Guardian 4-LSA Protection Architecture

**Candidate:** `FSATS-CB-v0.1`
**Status:** `DESIGN_CANDIDATE / NOT_OWNER_ACCEPTED / NOT_CLOSED`
**Application:** `Falcon Trading Guardian Application`
**MSA:** `MSA-G`
**LSA Count:** `4`
**Implementation Authority:** `NOT GRANTED`

## 1. Mission

Trading Guardian is an independent protection Application whose purpose is to preserve capital, contain trading-system failure, enforce bounded protection restrictions, support crisis survival and require evidence before unrestricted recovery.

Guardian exists because the system being protected must not be the sole authority deciding whether it is healthy enough to continue.

## 2. Independence Rule

```text
GUARDIAN != TRADING APPLICATION
GUARDIAN != UNIFIED RISK
GUARDIAN != EXECUTION TRUTH
GUARDIAN != PROVIDER TRUTH
GUARDIAN != FSARM
GUARDIAN != FOUNDATION GUARDIAN
GUARDIAN != OWNER
```

Guardian can restrict/stop eligible trading behavior through governed protection contracts. It does not choose profitable trades or become a shadow trading engine.

## 3. G-LSA-01 — Protection Observation and Incident Qualification

### Owns

- governed protection observations;
- incident candidates;
- anomaly correlation;
- severity/consequence assessment;
- protection evidence intake;
- health-of-protection-input assessment.

### Observation classes

Potential inputs include attributable evidence about:

- capital/drawdown stress;
- repeated risk rejections;
- order/execution anomalies;
- broker reconciliation ambiguity;
- stale/conflicted market data;
- provider degradation affecting protected positions;
- strategy/decision instability;
- abnormal order rate/message storm;
- account/environment mismatch;
- resource starvation affecting critical obligations;
- security/authority failures surfaced through governed Foundation/Application signals;
- Awareness integrity incidents relevant to trading safety.

Observation does not automatically equal incident or command.

### Components

- `ProtectionObservationIngestor`.
- `IncidentCorrelator`.
- `SeverityEvaluator`.
- `ProtectionEvidenceValidator`.
- `IncidentStateMachine`.

## 4. Incident Model

Candidate incident states:

```text
OBSERVED
-> QUALIFYING
-> CONFIRMED / DISMISSED
-> ACTIVE
-> CONTAINED
-> RECOVERING
-> RESOLVED
```

Each incident carries:

- incident identity;
- trigger evidence;
- affected scope;
- severity;
- consequence class;
- uncertainty;
- active directives;
- expiry/review time;
- recovery prerequisites;
- evidence lineage.

## 5. G-LSA-02 — Protection Scope, Restriction and Command Governance

### Owns

- protection policy evaluation;
- exact target scope;
- bounded command/directive creation;
- command precedence;
- expiry;
- idempotency;
- supersession;
- release prerequisites.

### Candidate directive families

Depending on separately accepted contracts/authority:

- `BLOCK_NEW_RISK`;
- `RESTRICT_MARKET`;
- `RESTRICT_INSTRUMENT`;
- `RESTRICT_STRATEGY`;
- `RESTRICT_ORDER_TYPE`;
- `REDUCE_MAXIMUM_RISK_ENVELOPE`;
- `MANAGED_EXIT_ONLY`;
- `SUSPEND_AUTONOMOUS_TRADING`;
- `REQUIRE_RECONCILIATION`;
- `REQUIRE_OPERATOR_REVIEW`;
- `EMERGENCY_STOP_AFFECTED_SCOPE`.

The exact command set must be bound to current accepted protection contracts before implementation.

### No blind liquidation

Guardian does not respond to every anomaly by immediately liquidating every position.

Protection must consider:

- current position risk;
- market liquidity;
- trading halt/session state;
- data quality;
- broker truth;
- execution feasibility;
- potential harm from forced liquidation;
- existing protective orders;
- incident scope;
- current Owner policy.

When safe liquidation/managed exit cannot be proven, the valid state may be `HOLD / NO_NEW_RISK / RECONCILE` rather than blind action.

## 6. Command Truth

A protection command includes at minimum where applicable:

- command identity;
- issuing Guardian Application identity;
- authority reference;
- target Application/account/environment;
- affected scope;
- exact directive;
- effective time;
- expiry/review time;
- incident identity;
- causation/correlation;
- idempotency identity;
- superseded command reference;
- evidence references.

A consumer must fail closed if a material authority/target/expiry identity cannot be proven.

FCR-0004 remains open until the actual consuming implementation and route fixtures are verified.

## 7. G-LSA-03 — Crisis State, Survival and Protection Coordination

### Owns

- Guardian crisis state;
- protection priority evidence;
- survival obligations;
- crisis resource need evidence;
- cross-incident coordination;
- emergency degradation plan;
- crisis-to-recovery transition prerequisites.

### Candidate Guardian states

```text
NORMAL
HEIGHTENED
PROTECTIVE
CRISIS
SURVIVAL
RECOVERY_HOLD
```

State does not itself create more authority. Each allowed action still requires its governing policy/contract.

## 8. Crisis Resource Interaction

Guardian may report to FSARM:

- current protection obligation;
- minimum-safe resource requirement;
- urgency;
- consequence of starvation;
- reclaimability of Guardian noncritical tasks;
- restoration need.

FSARM may prioritize eligible resource redistribution based on this evidence, but Guardian cannot seize another Application's resources directly or become Foundation Resource Governance.

## 9. Protection Survival Set

During severe degradation, Guardian prioritizes a minimal survival set such as:

- incident evidence continuity;
- current restriction state;
- command/revocation state;
- open-position risk awareness where trustworthy data exists;
- reconciliation trigger/coordination;
- Owner/governance visibility path where available;
- self-health and fail-safe state.

Noncritical analytics and learning degrade first.

## 10. G-LSA-04 — Reconciliation, Recovery and Protection Evidence

### Owns

- verification that protection directives took effect;
- unresolved-command/outcome tracking;
- incident evidence package;
- recovery prerequisites;
- release recommendation evidence;
- post-incident review;
- protection effectiveness metrics.

### Protection outcome distinction

```text
COMMAND_ISSUED
!= COMMAND_DELIVERED
!= COMMAND_ACCEPTED
!= COMMAND_EFFECTIVE
!= SYSTEM_SAFE
```

Guardian must observe attributable outcome evidence rather than infer success from transport acknowledgement.

## 11. Release and Recovery

Protection restrictions are not released merely because the original alert disappears.

Recovery evidence may include:

- incident root cause understood or bounded;
- current authoritative state reconciled;
- affected data/provider/broker path healthy enough;
- risk/capital truth restored;
- no stale conflicting directives;
- required monitoring stable;
- required independent validation complete;
- exact release authority valid.

Candidate flow:

```text
INCIDENT CONTAINED
-> RECOVERY HOLD
-> EVIDENCE COLLECTION
-> RECOVERY VALIDATION
-> RELEASE AUTHORITY
-> SCOPED RELEASE
-> HEIGHTENED OBSERVATION
-> NORMAL
```

## 12. Guardian Self-Health

Guardian itself can fail or become compromised.

It therefore maintains:

- identity/configuration baseline;
- dependency health;
- input-quality health;
- command-path health;
- evidence-path health;
- resource pressure;
- clock/freshness state;
- MSA-G integrity state;
- Monitor AI findings.

Unknown Guardian integrity must reduce permissible autonomous trading authority, not increase it.

## 13. MSA-G

MSA-G understands the complete Guardian Application, including:

- incident detection quality;
- false-positive/false-negative patterns;
- command effectiveness;
- recovery quality;
- systemic blind spots;
- protection policy weaknesses;
- resource fitness;
- current limitations;
- candidate improvements.

MSA-G does not grant Guardian new command authority.

## 14. Monitor AI for MSA-G

Two independent Monitor AI perspectives observe MSA-G behavior for integrity anomalies.

They do not decide trading protection policy or execute commands. Material disagreement triggers the minimum Awareness Integrity Check.

## 15. Suggested CSA Candidates

Possible CSA eligibility:

- anomaly/incident correlation intelligence;
- crisis consequence estimator;
- recovery-quality anomaly detector.

Hard protection-command authority logic, expiry validation and command idempotency remain deterministic/governed rather than CSA-owned.

## 16. Crisis Playbook Model

Guardian playbooks are versioned governed policies with:

- trigger class;
- required evidence;
- allowed directive set;
- target scope rules;
- escalation threshold;
- minimum observation interval where applicable;
- stop conditions;
- recovery prerequisites;
- Owner-only protected decisions;
- test fixtures.

A playbook is not an excuse to hide policy inside code.

## 17. Black-Swan Design

Black-swan handling assumes models may be wrong and correlations may break.

Protection emphasizes:

- hard capital/exposure ceilings;
- bounded order rate/notional;
- loss/drawdown escalation;
- stale/conflicted data rejection;
- liquidity deterioration detection;
- execution ambiguity containment;
- no-new-risk states;
- resource protection for reconciliation/open-position safety;
- independent Guardian restriction;
- manual/Owner emergency control;
- post-event evidence preservation.

No AI prediction of a black swan is required for the protection architecture to function.

## 18. Fail-Safe Behavior

Examples:

- command route unavailable -> affected Trading scope cannot assume Guardian permission;
- Guardian state unknown -> Trading uses safer pre-agreed state;
- conflicting active directives -> stricter compatible directive controls until reconciliation;
- expired command with missing release evidence -> do not infer unrestricted operation if a persistent restriction state is required by policy;
- Guardian evidence store degraded -> no silent incident closure.

## 19. Acceptance Gates

```text
GUARDIAN_TRADING_BUSINESS_OWNERSHIP = 0
GUARDIAN_RISK_ENGINE_DUPLICATION = 0
BLIND_GLOBAL_LIQUIDATION_POLICY = 0
COMMAND_WITHOUT_AUTHORITY_IDENTITY = 0
TRANSPORT_ACK_AS_PROTECTION_SUCCESS = 0
GUARDIAN_DIRECT_RESOURCE_SEIZURE = 0
GUARDIAN_SELF_RELEASE_WITHOUT_REQUIRED_AUTHORITY = 0
UNRECONCILED_CONFLICTING_DIRECTIVES = 0
GUARDIAN_FAILURE_EXPANDS_TRADING_AUTHORITY = 0
```
