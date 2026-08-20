# Risk Evidence Standard

**Identifier:** STD-005  
**Version:** 1.0  
**Status:** Approved  
**Effective Date:** 2026-07-24  
**Approval Record:** GOV-006  
**Owner:** Falcon Standards Authority  
**Governing Authority:** Falcon Constitution Articles 7–10, 13, 15, 19, 22–24, 31, and 38
**Supersedes:** None  
**Superseded By:** None

## 1. Purpose

This Standard defines the minimum evidence discipline for material risk claims, assessments, acceptances, restrictions, and residual-risk decisions in Falcon.

It governs how risk is evidenced. It does not define risk limits, appetite, capital policy, or ownership that belongs in Specifications.

## 2. Applicability

This Standard applies when a decision, capability, change, dependency, exception, release, or operation may materially affect:

- capital;
- authority or constitutional control;
- security or trust;
- data or state integrity;
- operational continuity or recoverability;
- evidence and accountability;
- external obligations; or
- Falcon’s ability to preserve future choice.

## 3. Required Risk Evidence

Every material risk record SHALL identify:

1. risk ID, class, owner, and decision context;
2. asset, authority, duty, or objective exposed;
3. credible cause, event, and consequence;
4. affected scope, dependencies, and correlated conditions;
5. current controls and their independent enforceability;
6. evidence source, provenance, quality, freshness, and limitations;
7. likelihood or uncertainty representation appropriate to available knowledge;
8. severity, reversibility, duration, and maximum credible harm;
9. aggregate and cumulative exposure where relevant;
10. assumptions, blind spots, contradictions, and unknowns;
11. alternatives, including delay, reduction, isolation, or non-action;
12. proposed treatment and expected residual risk;
13. acceptance authority and authority limit;
14. monitoring, stop, escalation, and expiry conditions; and
15. review date and Decision Ledger reference when material.

## 4. Evidence Quality

Risk evidence SHALL be relevant, attributable, version-bound, time-bound, and proportionate to consequence.

Facts, estimates, scenarios, assumptions, interpretations, model outputs, and judgments SHALL remain distinguishable.

Absence of observed failure SHALL NOT establish safety. Historical success SHALL NOT establish competence under materially different conditions.

## 5. Uncertainty

Uncertainty SHALL be represented explicitly and SHALL NOT be hidden by false precision.

Unknown required evidence SHALL NOT be treated as favorable evidence. Where severe harm is plausible and knowledge is insufficient, the risk decision SHALL favor reduced exposure, reversible action, delay, containment, or non-action.

Confidence SHALL identify its basis and applicable conditions. A confidence value without calibration or meaning SHALL NOT satisfy this Standard.

## 6. Aggregation and Dependency

Risk SHALL be assessed beyond the isolated request when shared causes, dependencies, concentration, correlated failure, sequential actions, or cumulative exposure may change consequence.

Fragmenting, renaming, or distributing one material exposure SHALL NOT reduce the authority required to accept it.

## 7. Control Evidence

A claimed control SHALL identify:

- the condition it prevents, detects, limits, or recovers;
- its owner and authority;
- its enforcement boundary;
- dependency and failure modes;
- test or operating evidence;
- bypass and abuse analysis;
- monitoring and degradation behavior; and
- conditions under which the control is no longer trustworthy.

A control SHALL NOT be credited solely from documentation or from claims produced by the subject it constrains.

## 8. Risk Decision Vocabulary

- **Avoid:** Do not create or continue the exposure.
- **Reduce:** Limit likelihood, consequence, duration, reach, or uncertainty.
- **Transfer:** Assign a defined consequence to a capable external party without concealing retained obligations.
- **Accept:** Authorize explicit residual risk within legitimate limits.
- **Escalate:** Refer the decision to higher or independent authority.
- **Inconclusive:** Required evidence is insufficient for a responsible decision.

`Inconclusive` SHALL NOT be treated as acceptance.

## 9. Acceptance Authority

Risk may be accepted only by an authority whose mandate covers the risk class, scope, duration, and maximum possible consequence.

The proposer, beneficiary, model, strategy, change producer, or constrained subject SHALL NOT approve risk beyond its delegated authority or approve its own exemption from a higher protection.

Acceptance SHALL expire or require review when assumptions, evidence, exposure, dependencies, controls, or authority change materially.

## 10. Residual Risk and Monitoring

Residual risk SHALL state what remains after verified controls, not what is hoped to remain.

Monitoring SHALL define indicators, evidence freshness, thresholds, responsible owner, stop conditions, escalation path, and response time appropriate to consequence.

If required monitoring or control evidence becomes unavailable, the risk decision SHALL be reassessed and affected authority reduced as required.

## 11. Prohibited Practices

Risk evidence SHALL NOT:

- equate probability with certainty;
- ignore maximum consequence because probability is low;
- use expected profit to waive non-waivable protection;
- count the same control independently more than once;
- conceal dependency or aggregate exposure;
- treat accepted risk as eliminated risk;
- preserve acceptance after its conditions fail; or
- label a known constitutional or protection violation as ordinary technical debt.

## 12. Compatibility and Transition

Approval of this Standard SHALL NOT retroactively invalidate a prior approved risk record solely because its form differs.

Existing risk decisions SHALL be brought into conformance before renewal, expansion, material reliance, or changed acceptance. A known exposure that exceeds legitimate authority or protection SHALL be contained immediately and SHALL NOT wait for administrative migration.

## 13. Acceptance Evidence

Conformance requires:

- complete risk lineage and ownership;
- explicit uncertainty and maximum-consequence analysis;
- aggregate and dependency analysis where relevant;
- independently supportable control evidence;
- legitimate acceptance authority;
- residual-risk, monitoring, stop, and expiry conditions; and
- reconstruction of the decision from preserved evidence.

## 14. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Falcon Standards Authority | Approved | GOV-006 | 2026-07-24 |
