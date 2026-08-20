# P1-H — Trading Guardian 4-LSA Code-Ready Decomposition

**Status:** `DESIGN_MATERIALIZATION / OWNER_DIRECTED_COMPLETION_CYCLE`  
**Scope:** `P1-H DESIGN ONLY`  
**Implementation Authority:** `NOT_GRANTED`  
**Runtime Authority:** `NOT_GRANTED`

## 1. Boundary

Falcon Trading Guardian is an independent protection/crisis Falcon Application. It owns protection observation, incident qualification, restriction/command governance, crisis survival coordination, reconciliation/recovery protection evidence. It does not own Trading strategy, Trading Risk, broker execution truth, provider truth, APP-RSC resource authority, Foundation lifecycle/security/resource governance or Owner authority.

Physical placement follows P1-C:

```text
TradingGuardian.Contracts
TradingGuardian.Domain
TradingGuardian.Application
TradingGuardian.Infrastructure
TradingGuardian.Awareness
TradingGuardian.Host
```

## 2. G-LSA-01 Protection Observation & Incident Qualification

Components: `ProtectionObserver`, `IncidentDetector`, `IncidentClassifier`, `TrustSignalCorrelator`, `ProtectionEvidenceCollector`.

Observes attributable protection-relevant evidence from Trading, FSAPMA, APP-RSC, Foundation-facing projections and Guardian-local controls. Observation never becomes business truth ownership of another Application. Incident qualification distinguishes anomaly, degraded state, material protection incident, integrity incident and unknown condition.

## 3. G-LSA-02 Protection Scope, Restriction & Command Governance

Components: `ProtectionScopeResolver`, `RestrictionPolicyEvaluator`, `ProtectionCommandIssuer`, `CommandLeaseRegistry`, `CommandEvidenceBinder`, `KillScopePlanner`.

Every protection directive requires exact issuer identity, authority basis, target, scope, reason, effective time, expiry/lease where applicable, causation/correlation, idempotency identity and evidence. Target selection follows minimum-necessary containment when the blast radius is proven; unknown/potentially propagated trust damage expands containment until a trustworthy boundary is established.

Guardian does not receive one magical global button. Distinct controls remain conceptually separate:

```text
AI_KILL
NEW_RISK_FREEZE
ORDER_ENTRY_KILL
CANCEL_WORKING_ENTRIES
EMERGENCY_POSITION_EXIT
```

The exact runtime authority for each remains separately governed.

## 4. G-LSA-03 Crisis State, Survival & Protection Coordination

Components: `CrisisStateMachine`, `SafetyContinuityCoordinator`, `DeterministicSafetyKernel`, `ProtectionObligationRegistry`, `EmergencyActionPlanner`.

Guardian contains two conceptually distinct layers:

```text
INTELLIGENT PROTECTION
+ DETERMINISTIC SAFETY KERNEL
```

The deterministic Safety Kernel must not depend on the continued trust of Guardian AI. If Guardian intelligence is killed/untrusted, independently trustworthy deterministic protection may continue within previously governed safety authority.

Safety Kernel allowed purposes are limited to protect, freeze, reconcile, reduce, exit when required by approved emergency policy, and deny unsafe expansion. It may not discover opportunities, choose strategies, increase position size, optimize profit, learn new strategies or expand Risk policy.

## 5. G-LSA-04 Reconciliation, Recovery & Protection Evidence

Components: `ProtectionReconciler`, `IncidentLedgerAdapter`, `RecoveryReadinessEvaluator`, `ProtectionStateRebuilder`, `ControlledRevivalEvidenceAssembler`.

Owns Guardian-side evidence for what was restricted, what remained protected, what was revoked, what remains unknown, and what recovery proof exists. It does not declare another Application trusted merely because Guardian sees healthy output.

Restart/recovery distinctions remain:

```text
RESTARTED != RECOVERED
REPAIRED != TRUSTED
TESTED != RELEASED
```

## 6. Trading Exposure Continuity

AI failure or Kill must not orphan existing financial exposure. Guardian consumes authoritative Trading projections for positions/orders/protection/reconciliation-needed states without taking ownership of Trading execution truth.

When affected intelligence is untrusted:

- no new intelligent risk;
- open/pending/partial exposure remains under an identified safety owner;
- valid broker-native protection is preserved/reconciled where trustworthy;
- missing but repairable protection may be re-established only inside the last valid safety envelope;
- unknown/unprotected exposure triggers reconciliation and bounded protect/reduce/exit behavior according to approved emergency policy;
- auto-liquidation of all positions is not an unconditional default.

## 7. Command Delivery Boundary

Protection commands cross Application boundaries only through P1-K governed contracts/routes and Foundation-approved communication capability. Delivery does not imply target acceptance; command outcomes distinguish request sent, accepted/rejected, applied, partially applied, expired, revoked and reconciliation-required states.

FCR-0004 remains an implementation hold until code/routes/fixtures exist.

## 8. APP-RSC Resource Interaction

Guardian publishes attributable crisis/protection resource need, minimum-safe requirement, urgency and consequence-of-starvation evidence to APP-RSC. APP-RSC may preferentially coordinate eligible existing FSATS resources toward proven protection obligations but Guardian may not seize sibling resources or mint Foundation capacity.

Guardian protection urgency is evidence, not an automatic Foundation priority rewrite.

## 9. Failure and Independence Rules

- Guardian AI failure -> deterministic Safety Kernel may continue only if independently trustworthy;
- Safety Kernel trust failure -> affected automated protection authority fails closed/escalates; do not pretend AI can replace it;
- command route unavailable -> display/record inability; no simulated success;
- Trading truth stale/unknown -> no invented position state; require reconciliation;
- APP-RSC unavailable -> Guardian remains within current admitted resources and requests cannot become peer seizure;
- evidence path failure -> preserve local attributable incident state and escalate missing authoritative evidence;
- monitor disagreement -> integrity investigation path, not majority voting.

## 10. Required Later Implementation Tests

False positive incident; false negative challenge; target-scope ambiguity; expired command replay; duplicate Kill request; command delivered but target rejects; command accepted but execution outcome unknown; Guardian AI killed while Safety Kernel continues; Safety Kernel state corrupted; open position with missing stop; broker stop valid while AI work is fenced; partial fill during emergency restriction; APP-RSC pressure during crisis; stale Guardian command after Controlled Revival; restart attempts to clear incident; evidence tampering; unauthorized sibling resource seizure.

## 11. P1-H Closure Invariants

- exactly four major Guardian branches;
- deterministic safety is independent enough from killed Guardian intelligence to preserve bounded protection when trustworthy;
- Guardian never becomes Trading Risk/execution/provider truth;
- protection scope is attributable, scoped, expiring/revocable where applicable and replay-safe;
- safety continuity preserves obligations without creating unbounded emergency authority;
- APP-RSC resource interaction preserves Foundation and sibling ownership;
- recovery cannot be self-declared by the failed subject.
