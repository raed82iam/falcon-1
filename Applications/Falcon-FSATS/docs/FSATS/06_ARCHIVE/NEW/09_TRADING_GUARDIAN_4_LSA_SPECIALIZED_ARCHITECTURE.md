# Falcon Trading Guardian Application — 4-LSA Specialized Implementation Architecture

**Package:** `FSATS-SIA-v0.1`
**Application:** `APP-GRD`
**MSA:** `MSA-GRD`
**Status:** `DESIGN_CANDIDATE`

## 1. Mission

APP-GRD is an independent Trading-domain protection Application. It observes protection-relevant evidence, qualifies incidents, issues only explicitly authorized scoped protection directives, coordinates crisis protection obligations, and governs evidence-based release/recovery recommendations within its Application authority.

Guardian is not Trading Risk, FSARM, FSA, Foundation Guardian, a provider manager or a strategy/execution engine.

## 2. Protection Principle

```text
DETECTION != INCIDENT
INCIDENT != AUTHORITY
AUTHORITY != DIRECTIVE
DIRECTIVE_DELIVERY != TARGET_COMPLIANCE
TARGET_COMPLIANCE != RECOVERY
RECOVERY_EVIDENCE != OWNER / LIFECYCLE RELEASE
```

Each distinction is preserved in state/evidence.

## 3. Protection Evidence Inputs

Guardian may consume governed projections/events from:

- APP-TRD: risk decisions, exposure, open positions/orders, drawdown, execution ambiguity, account/market readiness, operational state;
- APP-PMA: Data Product quality, provider-route degradation/conflict/unavailability, operational data continuity;
- APP-SIM: non-authoritative validation/adversarial evidence only, never current production truth;
- FSARM: effective resource pressure/outcome projections relevant to protection;
- Foundation adapters: technical lifecycle/security/communication/resource truth exposed to the Application;
- Guardian-local Monitor AI / health evidence.

Guardian SHALL NOT read another Application database or internal memory to obtain these facts.

## 4. G-LSA-01 — Protection Observation & Incident Qualification

### Components

- `G01.ProtectionSignalRegistry`
- `G01.SignalIngestor`
- `G01.SignalDeduplicator`
- `G01.IncidentCorrelationEngine`
- `G01.IncidentPolicyEvaluator`
- `G01.GuardianIncidentAggregate`
- `G01.IncidentEvidenceAssembler`
- `G01.IncidentCorrelationModel` (optional intelligent model)

### Signal classes

Initial canonical classes:

```text
AUTHORITY_OR_PERMISSION_FAILURE
SECURITY_OR_INTEGRITY_FAILURE
MARKET_DATA_STALE_OR_CONFLICTED
PROVIDER_PATH_FAILURE
BROKER_OR_EXECUTION_AMBIGUITY
ORDER_STATE_CONFLICT
POSITION_RECONCILIATION_FAILURE
CAPITAL_OR_RESERVATION_CONFLICT
RISK_LIMIT_BREACH_OR_NEAR_BREACH
DRAWDOWN_OR_LOSS_ESCALATION
MARKET_HALT_OR_DISLOCATION
LIQUIDITY_COLLAPSE
RESOURCE_STARVATION
APPLICATION_HEALTH_DEGRADATION
AWARENESS_INTEGRITY_ANOMALY
PROTECTION_DIRECTIVE_NONCOMPLIANCE
```

A signal is immutable and carries exact source/evidence identity.

### Incident qualification

The deterministic `IncidentPolicyVersion` contains ordered predicates. For a correlated signal set, the engine selects the highest-severity matching predicate; ties select the narrower scope when it fully contains the risk, otherwise the broader safe scope.

Policy predicate fields:

```text
RequiredSignalClasses[]
OptionalCorroboration[]
MinimumSourceIndependence
MaximumEvidenceAge
ImpactClass
ImminenceClass
ConfidenceFloor if applicable
RequiredScopeBinding
EscalationSeverity
DefaultProtectionActionSet
```

No generic numeric AI score can override an explicit high-consequence predicate.

### Incident state

```text
OBSERVED
QUALIFYING
QUALIFIED
PROTECTION_ACTIVE
CONTAINED
RECOVERY_ASSESSMENT
RESOLVED
CLOSED

side states:
FALSE_POSITIVE
DUPLICATE
SUPERSEDED
EVIDENCE_INSUFFICIENT
ESCALATED
```

A qualified incident cannot move directly to CLOSED while active protection obligations remain.

### Deduplication

Incident dedupe key is policy-defined using scope + root signal class + source causation window. A duplicate signal appends evidence to the existing incident; it does not mint a competing incident that can issue contradictory directives.

### CSA

`IncidentCorrelationModel` may be CSA-eligible. Incident policy/authority/state machine remains deterministic and outside CSA control.

## 5. G-LSA-02 — Protection Scope, Restriction & Command Governance

### Components

- `G02.ProtectionAuthorityResolver`
- `G02.ScopeResolver`
- `G02.DirectivePolicyEngine`
- `G02.ProtectionDirectiveAggregate`
- `G02.CommandPublisher`
- `G02.DirectiveOutcomeTracker`
- `G02.ExpiryRevocationManager`

### Protection scope

A directive binds exactly one scope expression composed from allowed dimensions:

```text
ApplicationId
TradingAccountId
MarketId?
InstrumentId?
StrategyId?
ProviderId/ProviderRouteId?
OrderChainId/PositionId?
Environment
```

Wildcard/global scope is allowed only if the explicit Guardian authority/policy permits it and narrower scope cannot safely contain the incident.

### Directive fields

```text
ProtectionDirectiveId
GuardianIncidentId
PolicyVersion
AuthorityReference
TargetApplicationId
TargetScope
ProtectionAction
Severity
EffectiveFrom
ExpiresAt or explicit non-time revocation condition
IdempotencyKey
Causation/Correlation refs
RequiredAcknowledgementClass
ReasonCodes[]
EvidenceRefs[]
```

### Authority gate

Before issuing a directive:

1. Guardian Application/lifecycle is eligible;
2. incident is QUALIFIED or a separately governed emergency predicate exists;
3. exact action is within the currently effective Guardian authority instrument;
4. target/scope is within that authority;
5. directive policy permits action for incident predicate/severity;
6. no newer superseding directive already dominates it;
7. Foundation route/security capability is available;
8. directive is not a replay/simulation/test input masquerading as operational authority.

Failure => no directive; incident remains visible and escalates where required.

### Protection actions

Current canonical actions:

```text
RESTRICT_NEW_RISK
REDUCE_ALLOWED_EXPOSURE
SUSPEND_STRATEGY_SCOPE
SUSPEND_INSTRUMENT_SCOPE
SUSPEND_MARKET_SCOPE
CANCEL_OPEN_ORDERS
EXIT_POSITION_SCOPE
ISOLATE_PROVIDER_ROUTE
REQUEST_RESOURCE_PRIORITY
HOLD_PROMOTION
```

Each target Application owns the business mechanics of complying with a valid directive. Guardian owns the directive meaning and tracks outcome evidence.

### Cancel / exit distinction

`CANCEL_OPEN_ORDERS` requests cancellation of eligible open orders. It does not represent them as canceled until APP-TRD reconciliation confirms terminal state.

`EXIT_POSITION_SCOPE` is a high-consequence protection directive requesting Trading to generate/execute a risk-reducing exit path under its execution/reconciliation logic. Guardian does not fabricate broker fills or position state.

### Idempotency/supersession

Same `ProtectionDirectiveId` is immutable. Repeated delivery is idempotent. A semantic change creates a new directive with `SupersedesDirectiveId`.

Stronger active directive dominates weaker conflicting directive on the same scope according to policy order. Downgrade/release requires explicit recovery evidence; simple expiry cannot automatically restore risk if the incident remains active.

## 6. G-LSA-03 — Crisis State, Survival & Protection Coordination

### Components

- `G03.CrisisDetector`
- `G03.CrisisEpisodeAggregate`
- `G03.ProtectionObligationRegistry`
- `G03.SurvivalPlanBuilder`
- `G03.CrossApplicationProtectionCoordinator`
- `G03.FSARMProtectionNeedPublisher`
- `G03.CrisisProgressEvaluator`

### Crisis entry

A CrisisEpisode opens when policy maps one or more qualified incidents to a crisis state.

Initial crisis levels:

```text
NONE
ELEVATED
SEVERE
CRITICAL
EMERGENCY
```

Crisis level is not Foundation technical criticality and does not self-mint resource authority.

### Protection obligation order

When applicable, default Guardian survival priority is:

1. preserve authority/security/evidence ability needed to contain/reconcile;
2. preserve open-position/order truth and ability to cancel/exit safely;
3. preserve minimum operational Data Products required for protection;
4. prevent new risk;
5. preserve capital/reservation reconciliation;
6. preserve incident/directive communication and evidence;
7. degrade/stop nonessential discovery/analytics/experimentation.

### FSARM interaction

Guardian publishes immutable `GuardianResourceNeedReport` containing:

```text
CrisisEpisodeId
ActiveProtectionObligations[]
MinimumSafeResourceByClass
DesiredResourceByClass
ConsequenceOfStarvation
CurrentGuardianUsage
Reclaimability within Guardian
Deadline/urgency business evidence
EvidenceRefs
```

Guardian cannot directly reclaim FSTSimA/Trading/FSAPMA resources. FSARM decides bounded internal redistribution under its policy; Foundation remains final total-resource authority.

`REQUEST_RESOURCE_PRIORITY` means submit protection consequence evidence. It does not mean Guardian assigns itself Foundation technical criticality.

### Cross-Application coordination

Guardian may issue separately authorized protection directives to multiple Applications. It shall preserve independent per-target directive identities/outcomes rather than one opaque global command.

### Crisis exit

Crisis level may decrease only when the policy-required incident/protection evidence supports the lower level. Time passage alone is insufficient.

## 7. G-LSA-04 — Reconciliation, Recovery & Protection Evidence

### Components

- `G04.DirectiveReconciler`
- `G04.IncidentRootCauseRecorder`
- `G04.RecoveryReadinessEvaluator`
- `G04.RestrictionReleasePlanner`
- `G04.ProtectionEvidenceStore`
- `G04.PostIncidentAssessment`
- `G04.GuardianSelfHealthEvaluator`

### Reconciliation

For each active directive Guardian tracks:

```text
SENT
DELIVERED_OR_DELIVERY_UNKNOWN
TARGET_ACKNOWLEDGED
TARGET_REJECTED
TARGET_APPLYING
TARGET_EFFECT_CONFIRMED
TARGET_EFFECT_PARTIAL
TARGET_EFFECT_FAILED
SUPERSEDED
RELEASE_PENDING
RELEASED
```

Transport delivery is never represented as target effect.

### Recovery readiness

A restriction may be proposed for release only when:

1. root incident is CONTAINED/RECOVERY_ASSESSMENT or otherwise policy-eligible;
2. target state is reconciled and not materially ambiguous;
3. data/provider/account/broker prerequisites required for safe resumption are healthy enough;
4. no higher-severity overlapping directive remains active;
5. resource minimums are restored where required;
6. authority/security state is trustworthy;
7. required post-incident evidence is complete.

Unknown => keep/reduce trust, do not auto-release.

### Release

Release is an explicit new/superseding directive or governed lifecycle/authority event, not deletion of old evidence.

### Evidence preservation

Incident, signals, directives, target outcomes, resource requests/outcomes, root cause and release evidence are append-only/immutable records with explicit correction/supersession links.

### Guardian self-health

If Guardian cannot establish its own required identity/authority/evidence integrity, it fails closed for risk-increasing permissions and escalates inability to protect. It SHALL NOT silently declare the system safe because Guardian itself is unavailable.

## 8. Guardian MSA

MSA-GRD understands Guardian end-to-end condition, incident quality, false-positive/false-negative evidence, directive effectiveness, crisis/recovery quality and improvement opportunities.

It may recommend changes/candidates under awareness governance. It cannot modify active authority instruments, self-approve weaker protection policy, deploy its own candidate, control Foundation Guardian/FSA or declare Owner/governance recovery approval.

Two Monitor AI perspectives apply under file 18.

## 9. Guardian / Trading Risk Boundary

Examples:

- T-LSA-07 sees a proposal exceeds max loss -> Risk DENY; no Guardian incident required unless policy says breach itself is protection-relevant.
- Execution state becomes irreconcilably ambiguous with potential duplicate exposure -> Guardian may qualify incident and restrict new risk/cancel scope.
- Portfolio drawdown reaches business degradation threshold -> T-LSA-07 reduces risk; Guardian independently evaluates whether protection policy requires broader restriction.
- Market data is conflicted -> Trading fails new risk; Guardian may also isolate provider route or restrict affected market if severity policy matches.

Duplicate action may be semantically consistent but ownership remains distinct and evidence shows which authority acted.

## 10. Guardian Failure Behavior

If Guardian transport to a target fails:

- directive remains not-effect-confirmed;
- retry uses Foundation bounded delivery/idempotency semantics;
- inability to establish effect escalates incident severity/visibility according to policy;
- Guardian SHALL NOT assume protection happened.

If Guardian receives conflicting target outcomes, state becomes `TARGET_EFFECT_PARTIAL` or `CONFLICTED`, not success.

## 11. Security

- directive creation requires exact active authority reference;
- target recipient and scope binding are cryptographically/transport-bound through Foundation capabilities;
- replay/test/simulation directives cannot affect operational target routes;
- directive logs redact sensitive data but preserve evidence identity;
- Guardian cannot modify the authoritative transport/security checks it depends on;
- no target Application may forge Guardian identity by emitting a lookalike payload.

## 12. Verification Families

Guardian verifier SHALL cover at least:

1. exactly four LSAs + one MSA;
2. signal != incident;
3. incident != authority;
4. deterministic incident policy/tie-break;
5. scope narrowing/broadening rules;
6. exact action-to-authority mapping;
7. replay/simulation command rejection;
8. idempotent directive delivery;
9. directive supersession/downgrade rules;
10. cancel request != canceled truth;
11. exit directive != fill/position truth;
12. Guardian != Trading Risk;
13. Guardian != FSARM/Foundation resource authority;
14. Guardian crisis priority evidence != Foundation technical criticality;
15. directive delivery != effect;
16. unknown target effect fails safe;
17. release requires explicit recovery evidence;
18. historical evidence preserved after release;
19. compromised/unhealthy Guardian cannot self-attest safety;
20. intelligent correlation model cannot override deterministic authority/hard incident rules;
21. deterministic replay from identical evidence/policy versions;
22. no direct cross-Application state access.
