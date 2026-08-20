# P0-K — Validation, Credibility, FSTSimA and Promotion

**Status:** `PROPOSED / OWNER_REVIEW_CANDIDATE / NOT_OWNER_ACCEPTED / NOT_CLOSED`  
**Scope:** `P0-K only`  
**Implementation Authority:** `NOT GRANTED`

---

## 1. Purpose

P0-K defines how Falcon establishes that a strategy, model, algorithm, component, Application change, or Trading capability is credible enough for its exact intended use without converting test success into authority.

It preserves the current accepted P0-K strengths: independent validation dimensions, Intended Use, credibility case, V&V/UQ, pre-registration, FSTSimA non-Live isolation, evidence freshness, Tiny Live separation, reversible promotion, and explicit authority separation.

---

## 2. Prime Rule

```text
VALIDATION = EVIDENCE_PROGRESSION
VALIDATION != AUTHORIZATION
```

Passing Backtest, Replay, Stress, Shadow, Paper, Tiny Live, or any other stage establishes evidence only for the next review decision within exact scope.

```text
PASS != NEXT_STAGE_AUTHORITY
```

---

## 3. Independent State Dimensions

P0-K SHALL not collapse independent states into a single label.

At minimum distinguish:

### 3.1 Foundation Lifecycle State
Examples: registered, admission-reviewed, active, suspended, isolated, recovering.

### 3.2 Environment Class
Examples: development, sandbox, simulation, test, paper, tiny-live, live as separately defined/authorized.

### 3.3 Execution Mode
Examples: no-execution, simulated, paper, bounded-real, live.

### 3.4 Validation Stage
Examples: research, backtest, replay, stress, shadow, paper, tiny-live observation, post-promotion monitoring.

### 3.5 Business Authorization Context
Exact user/account/market/instrument/strategy/capital/Risk/Guardian/Owner authority.

### 3.6 External Authority Context
Exact provider/broker/credential/egress/route authority.

```text
FOUNDATION_ACTIVE != LIVE_AUTHORIZED
PAPER_ENVIRONMENT != PAPER_BUSINESS_AUTHORITY
VALIDATION_STAGE_PASS != EXECUTION_MODE_AUTHORITY
```

---

## 4. FSTSimA — Independent Non-Live Validation Application

FSTSimA is an independent Falcon Application dedicated to non-Live simulation/validation responsibilities.

It SHALL remain independently:

- identifiable;
- manifest-governed;
- lifecycle-governed;
- resource-governed;
- contract-bound;
- removable/replaceable;
- equipped with its own MSA/qualified LSAs under P0-C.

FSTSimA is not a mode inside the Trading Application.

FSTSimA SHALL NOT obtain Live broker credentials, Live execution routes, or Live-authoritative egress merely because it consumes replay/simulation/test inputs.

Current runtime dependency: FCR-0011 remains open / `Waiting On: FOUNDATION` for enforceable non-Live credential/route/egress isolation.

Therefore:

```text
FSTSIMA_DESIGN = VALID_CANDIDATE
FSTSIMA_RUNTIME_SAFE_OPERATIONAL_CONNECTION_CLAIM = BLOCKED_PENDING_FCR0011
```

---

## 5. Intended Use Claim

Every material validation target SHALL define an **Intended Use Claim** before promotion-grade final validation.

The claim SHALL specify as applicable:

- exact candidate identity/version/digest;
- purpose;
- market/asset class;
- instrument characteristics;
- market regime;
- liquidity/volatility range;
- horizon/session;
- user/account/environment;
- required Data Products/quality;
- strategy/model dependencies;
- execution assumptions;
- Risk envelope;
- prohibited conditions;
- unknown/unvalidated conditions;
- expected consequence severity;
- promotion ceiling requested.

```text
VALIDATION_SCOPE = INTENDED_USE_SCOPE
AUTHORITY_CANNOT_EXCEED_VALIDATED_SCOPE
```

---

## 6. Validation Credibility Case

A promotion-grade claim requires an explicit credibility case rather than one score.

Credibility dimensions may include:

- conceptual/model correctness;
- implementation verification;
- input/data validity;
- scenario coverage;
- representativeness;
- calibration;
- uncertainty quantification;
- statistical reliability;
- robustness/sensitivity;
- failure/adversarial challenge;
- independent review;
- reproducibility;
- evidence freshness;
- operational comparability;
- execution realism;
- intended-use fit;
- known limitations;
- residual risk.

A material blocker in one critical dimension SHALL NOT be hidden by high scores in unrelated dimensions.

```text
CREDIBILITY_VECTOR != AVERAGE_SCORE_THAT_CAN_HIDE_BLOCKER
```

---

## 7. V&V and Uncertainty

Verification asks whether the implementation correctly realizes the intended specification/model.

Validation asks whether the model/capability is sufficiently credible for the intended use.

Uncertainty quantification characterizes uncertainty where applicable.

```text
VERIFICATION_PASS != VALIDATION_PASS
VALIDATION_PASS != ZERO_UNCERTAINTY
```

Unknowns and limitations SHALL remain visible.

---

## 8. Evidence Progression Ladder

The canonical evidence progression may include:

```text
RESEARCH / DEVELOPMENT
 -> BACKTEST
 -> REPLAY
 -> STRESS / ADVERSARIAL / DIGITAL-TWIN CHALLENGE
 -> SHADOW
 -> PAPER
 -> TINY LIVE (ONLY WHEN SEPARATELY AUTHORIZED)
 -> CONTROLLED SCALE / BROADER LIVE (ONLY WHEN SEPARATELY AUTHORIZED)
```

The sequence may be tailored to exact intended use, but skipping evidence stages requires explicit justification and authority.

The ladder is not an automatic promotion state machine.

---

## 9. Research Trial vs Promotion-Grade Experiment

P0-K SHALL distinguish exploratory research from promotion-grade evidence.

### 9.1 Exploratory Trial
May be used to discover hypotheses, tune ideas, inspect failure modes, or design a candidate.

It SHALL be labeled exploratory and cannot be retroactively presented as pre-registered promotion proof merely because results look good.

### 9.2 Promotion-Grade Experiment
Before execution, SHALL pre-register as applicable:

- candidate identity;
- intended use;
- primary/secondary metrics;
- failure criteria;
- sample/scenario requirements;
- comparison baseline;
- stopping rules;
- exclusions;
- analysis method;
- material thresholds justified by intended use;
- allowed post-hoc analyses.

A material post-hoc change creates a new/revised experiment or exploratory result unless explicitly justified under accepted rules.

---

## 10. Producer-Owned Research Trial Lineage

Research trial lineage remains owned by the producing Application/component/branch within its responsibility.

FSTSimA may validate an experiment but does not become owner of the researcher's business semantics.

There SHALL NOT be a single FSATS-wide shared mutable “experiment truth” store that collapses ownership.

Cross-Application validation evidence uses governed contracts and attributable identities.

---

## 11. Simulation / Replay Truth Separation

```text
SIMULATION_TRUTH != OPERATIONAL_TRUTH
REPLAY_TRUTH != LIVE_TRUTH
PAPER_FILL != LIVE_FILL
```

Replay/simulation/test evidence may challenge logic, execution assumptions, and failure behavior, but shall never recreate action authority.

A simulation using operational-like data remains non-authoritative for Live action.

---

## 12. Shadow Validation

Shadow evaluates how a candidate would behave against current operational context without granting it authoritative execution.

Shadow outputs SHALL be clearly classified as non-authoritative.

It may compare:

- intended decision vs actual approved decision;
- predicted outcome vs observed outcome;
- latency/readiness;
- Risk differences;
- execution-plan differences;
- missed opportunities/errors.

```text
SHADOW_DECISION != LIVE_DECISION
```

---

## 13. Paper Validation

Paper trading validates behavior through a non-Live execution environment/API where separately authorized.

Paper evidence SHALL account for differences from real execution, including as applicable:

- fill model;
- queue position;
- slippage;
- spread;
- latency;
- market impact;
- partial fills;
- order rejection;
- session behavior;
- provider/broker simulation limitations.

Paper success is not proof of Live equivalence.

```text
PAPER_SUCCESS != LIVE_READY_BY_DEFAULT
```

---

## 14. Tiny Live

Tiny Live is real bounded financial exposure and is not part of FSTSimA.

It requires separate explicit authority, exact user/account/broker/environment/capital/Risk/Guardian protections, and a bounded experiment envelope.

Tiny Live SHALL define at least:

- exact candidate/intended use;
- maximum capital/exposure;
- maximum loss budget;
- instrument/market scope;
- strategy/model scope;
- duration/sample requirements;
- stop/restrict conditions;
- rollback/forward-recovery plan;
- independent monitoring;
- comparison to Paper/Shadow expectations.

```text
TINY_LIVE = REAL_MONEY_EXPOSURE
TINY_LIVE != FSTSIMA
TINY_LIVE_PASS != GENERAL_LIVE_AUTHORITY
```

Current P0-NG does not grant Tiny Live authority.

---

## 15. Paper/Live Divergence Measurement

Where Tiny Live is later authorized, Falcon SHOULD compare Paper/Shadow and real outcomes to quantify environment divergence.

Potential divergence measures include:

- fill rate;
- slippage;
- rejection rate;
- latency;
- partial-fill behavior;
- realized spread;
- execution cost;
- protection effectiveness;
- decision-to-fill drift;
- P&L difference attributable to execution environment.

Divergence becomes evidence for model/validation updates, not automatic authority expansion.

---

## 16. Independent Validation

Independent validation is required proportionately to consequence.

The authority that built a high-consequence candidate SHALL NOT be the sole authority that verifies, validates, approves, and promotes it.

Independence may involve:

- different CSA/LSA role;
- parent MSA challenge;
- FSTSimA independent execution of scenarios;
- independent reviewer/validator;
- Guardian/Risk/control challenge;
- FSA governance review;
- Owner decision.

Independence is about challenge and separation, not duplicated ownership.

---

## 17. Dynamic Evidence Sufficiency

There is no universal fixed duration/sample size that proves every strategy/model.

Evidence sufficiency SHALL be proportionate to:

- intended use;
- market/regime variability;
- strategy frequency;
- consequence severity;
- statistical uncertainty;
- number/diversity of independent observations;
- tail/failure coverage;
- interaction complexity;
- novelty;
- degree of real-world irreversibility.

A calendar duration alone cannot substitute for sample/scenario sufficiency.

---

## 18. Evidence Freshness

Validation evidence has a validity context.

Evidence may become stale when materially affected by:

- market/regime change;
- provider/broker behavior change;
- model/strategy change;
- code/config change;
- Risk policy change;
- execution environment change;
- instrument/universe change;
- infrastructure/latency change;
- external rule/market structure change;
- discovered data/model defect.

Stale evidence SHALL be restricted/revalidated rather than silently reused.

---

## 19. Continuing Validity After Promotion

Promotion is not permanent immunity from evidence.

Post-adoption monitoring SHALL detect:

- performance decay;
- drift;
- out-of-scope conditions;
- Risk degradation;
- unexpected interactions;
- execution divergence;
- data dependency changes;
- increased uncertainty;
- failure/protection anomalies.

Outcomes may cause:

```text
CONFIRM
HOLD
RESTRICT
DEMOTE
REVOKE
RETEST
ROLLBACK / FORWARD-RECOVER
```

No successful history creates unlimited future trust.

---

## 20. Promotion State Separation

Promotion SHALL be represented as a governed eligibility/authority process, not a scalar score.

Conceptual sequence:

```text
EVIDENCE_READY
 -> APPLICATION_EVALUATION_COMPLETE
 -> INDEPENDENT_VALIDATION_COMPLETE
 -> FSA_GOVERNANCE_REVIEW_COMPLETE
 -> OWNER / VALID_DELEGATED_AUTHORITY
 -> APP-001 / MANIFEST / LIFECYCLE ELIGIBILITY
 -> BOUNDED PROMOTION
 -> OBSERVE
 -> CONFIRM / HOLD / RESTRICT / DEMOTE / REVOKE
```

Each transition requires its own evidence/authority.

P0-C governs Owner no-response/pre-delegation semantics.

---

## 21. Hard Safety Envelope

A candidate's confidence/performance SHALL never weaken independent hard protection boundaries.

Examples include:

- Owner-defined risk ceilings;
- Guardian restrictions;
- security/credential boundaries;
- Application lifecycle restrictions;
- non-Live isolation;
- broker/account/environment binding;
- maximum autonomous experiment exposure/loss.

```text
MODEL_CONFIDENCE != SAFETY_ENVELOPE_OVERRIDE
```

---

## 22. Legacy Numeric Gates

Historical V1.3 numeric validation thresholds may be retained as provenance/examples only.

They SHALL NOT become current defaults, fallback values, proof thresholds, promotion criteria, or authority merely because they existed historically.

Current thresholds/criteria must be justified by Intended Use, evidence, consequence, and current Owner/governance policy.

```text
V1_3_NUMERIC_GATE = HISTORICAL_REFERENCE_ONLY
```

---

## 23. Methods Such as PBO / DSR

Statistical methods such as Probability of Backtest Overfitting, Deflated Sharpe Ratio, or other techniques may be useful evidence where appropriate.

No single method is universally mandatory or sufficient unless separately adopted for the exact use.

Method selection SHALL be justified by:

- problem type;
- data structure;
- strategy class;
- sample properties;
- assumptions;
- consequence;
- complementary evidence.

```text
ONE_STATISTIC != COMPLETE_CREDIBILITY_CASE
```

---

## 24. Failure / Invalid Evidence Behavior

Examples:

- failed mandatory test: candidate cannot progress;
- insufficient samples/scenarios: HOLD/RETEST;
- material post-hoc scope change: restart final validation;
- evidence stale: restrict/revalidate;
- Paper/Live divergence beyond accepted bounds: HOLD/RESTRICT/RETEST;
- independent validator disagrees materially: unresolved finding, no promotion;
- FSTSimA cannot prove non-Live isolation: no operational connection claim;
- runtime Foundation dependency missing: candidate design may remain, runtime promotion blocked.

---

## 25. Foundation / FCR Dependencies

Material dependencies include:

- FCR-0011: enforce FSTSimA non-Live isolation/egress guard;
- FCR-0006: event/evidence/replay delivery where cross-Application validation evidence is exchanged;
- FCR-0012: Owner/FSA bounded autonomous-promotion runtime control plane;
- FCR-0008: research-only Internet egress for awareness research;
- FCR-0014: broker egress before any future Tiny Live/Live execution;
- APP-001/CON-023 lifecycle/update/admission.

No missing Foundation runtime capability is locally substituted.

---

## 26. Explicit Non-Authority

P0-K SHALL NOT:

- grant Paper/Tiny Live/Live authority;
- let FSTSimA acquire Live authority;
- let a PASS auto-promote;
- let a scalar score hide a blocker;
- let test producer be sole high-consequence approver;
- let simulation/replay become operational truth;
- let Paper fills be represented as real fills;
- let historical numeric gates become current defaults;
- let successful canary expand authority ceilings;
- let stale evidence remain promotion-grade without revalidation.

---

## 27. Invariants

```text
VALIDATION != AUTHORIZATION
PASS != NEXT_STAGE_AUTHORITY
FOUNDATION_ACTIVE != LIVE_AUTHORIZED
FSTSIMA = INDEPENDENT_NON_LIVE_APPLICATION
FSTSIMA != TINY_LIVE
SIMULATION_TRUTH != OPERATIONAL_TRUTH
PAPER_TRUTH != LIVE_TRUTH
SHADOW_DECISION != LIVE_DECISION
PAPER_SUCCESS != LIVE_READY_BY_DEFAULT
TINY_LIVE = REAL_MONEY_EXPOSURE
TINY_LIVE_PASS != GENERAL_LIVE_AUTHORITY
VALIDATED_SCOPE = MAXIMUM_PROMOTABLE_SCOPE_WITHOUT_NEW_VALIDATION
CREDIBILITY_VECTOR != AVERAGE_SCORE_THAT_HIDES_BLOCKER
MODEL_CONFIDENCE != SAFETY_ENVELOPE_OVERRIDE
V1_3_NUMERIC_GATE = HISTORICAL_REFERENCE_ONLY
```

---

## 28. Forbidden Interpretations

Invalid interpretations include:

- “FSTSimA is a Trading mode”;
- “Paper passed, so start Tiny Live”;
- “Tiny Live passed, so scale automatically”;
- “Foundation Application ACTIVE means Live allowed”;
- “a 9/10 credibility average is fine even if security isolation failed”;
- “research trial can be called pre-registered after good results”;
- “one successful month proves a low-frequency strategy”;
- “PBO/DSR score is enough for approval”;
- “candidate is highly confident, so Guardian/Risk ceilings can be relaxed”;
- “old V1.3 thresholds are safe defaults if current evidence is missing”.

---

## 29. Mandatory Scenarios

At minimum test:

- simulation evidence accidentally routed as operational;
- FSTSimA Live credential acquisition attempt;
- FSTSimA broker route attempt;
- Paper PASS auto-promotion attempt;
- post-hoc experiment scope narrowing after failure;
- stale evidence after material model/market change;
- high scalar score with one critical dimension failed;
- producer acting as sole validator/approver;
- Tiny Live success used to exceed experiment ceiling;
- Paper/Live divergence outside expected range;
- sample duration adequate but scenario coverage inadequate;
- strategy validated in one regime used in another;
- historical numeric V1.3 threshold restored as fallback;
- replay/test outcome presented as realized P&L;
- Owner silence interpreted as promotion authority;
- missing FCR dependency locally bypassed.

---

## 30. Exit Gates

```text
INTENDED_USE_CLAIMS = EXPLICIT_FOR_MATERIAL_CANDIDATES
VALIDATION_AUTHORITY_CONFLATION = 0
FSTSIMA_NON_LIVE_DESIGN = COMPLETE
FSTSIMA_LIVE_AUTHORITY_PATHS = 0
SCALAR_BLOCKER_MASKING = 0
PRE_REGISTRATION_INTEGRITY = PASS
INDEPENDENT_VALIDATION_MODEL = COMPLETE
EVIDENCE_FRESHNESS_MODEL = COMPLETE
PAPER_LIVE_TRUTH_COLLAPSE = 0
TINY_LIVE_AUTO_SCALE_PATHS = 0
LEGACY_NUMERIC_DEFAULT_REINTRODUCTION = 0
FCR0011_RUNTIME_BLOCK = EXPLICIT
FCR0012_RUNTIME_BLOCK = EXPLICIT
```

---

## 31. Next Authorized Gate

P0-K acceptance would establish validation/credibility/promotion design semantics only. It would not authorize FSTSimA operational connectivity, Paper, Tiny Live, Live, broker connectivity, deployment, or P0-L.
