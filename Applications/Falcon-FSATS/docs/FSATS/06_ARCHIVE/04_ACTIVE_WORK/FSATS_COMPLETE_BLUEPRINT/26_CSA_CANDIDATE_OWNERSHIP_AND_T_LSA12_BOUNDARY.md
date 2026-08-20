# FSATS Complete Blueprint — CSA Candidate Ownership and T-LSA-12 Boundary Clarification

**Candidate:** `FSATS-CB-v0.1 / CSA SEMANTIC REVISION`
**Status:** `CONTROLLING_PRE_FREEZE_CLARIFICATION / NOT_OWNER_ACCEPTED / NOT_CLOSED`
**Date:** `2026-08-11`
**Implementation Authority:** `NOT GRANTED`

## 1. Purpose

This clarification removes a possible ownership ambiguity between:

- strategy-originated CSA self-development; and
- `T-LSA-12 — Strategy Evolution & Experimentation`.

It is part of the same Owner-requested CSA semantic revision and controls interpretation of `25_OWNER_REQUESTED_CSA_ASSIGNMENT_AND_ELIGIBILITY_REGISTER.md` and the earlier Blueprint wording for `T-LSA-12`.

## 2. Strategy CSA Is an Awareness Companion, Not Direct Trading Authority

Each assigned strategy CSA is the specialized self-awareness companion for exactly one strategy identity.

The underlying strategy execution logic may remain deterministic or otherwise separately governed. CSA assignment does not convert the strategy signal logic into an autonomous self-modifying runtime path.

The strategy CSA owns only the awareness/evolution responsibility for its strategy component:

- self-evaluation;
- performance/regime understanding;
- failure-pattern understanding;
- feature and execution-sensitivity understanding;
- improvement hypotheses;
- component-local candidate specification;
- component-local candidate evidence and lineage.

It has no direct order, Risk, capital, broker, promotion or deployment authority.

## 3. Exact Candidate Ownership Rule

For a candidate whose actual origin is one strategy CSA:

```text
CANDIDATE BUSINESS OWNER = THAT STRATEGY COMPONENT
AWARENESS ORIGIN = THAT STRATEGY CSA
PARENT REVIEW = T-LSA-04 OR T-LSA-05, AS APPLICABLE
```

The strategy CSA may create or modify only isolated candidate assets that belong to its own strategy component and only when separate candidate-development authority exists.

It may not modify another strategy's assets.

## 4. T-LSA-12 Boundary

`T-LSA-12 — Strategy Evolution & Experimentation` remains the Trading branch responsible for:

- cross-strategy and school-level experimentation;
- Adaptive Meta-Learning;
- experiment orchestration;
- comparison frameworks;
- anti-overfitting controls;
- candidate challenge tooling;
- retirement/merge/split proposals;
- evidence-package generation for governed review;
- T-LSA-12-originated strategy evolution candidates.

It does **not** silently become the owner of a candidate whose actual origin and asset ownership belong to a strategy CSA.

When T-LSA-12 assists with testing a strategy-CSA candidate, it acts as a governed experimentation/challenge facility. Candidate origin and ownership remain attributable to the strategy component.

## 5. Adaptive Meta-Learner Boundary

`CSA-T-META-001` may:

- evaluate the Meta-Learner's own performance;
- improve the Meta-Learner's same-responsibility methods through isolated candidates;
- propose cross-strategy hypotheses, feature candidates, weighting candidates, applicability changes, combination candidates and retirement candidates through `T-LSA-12`.

It may **not** directly modify authoritative assets owned by another strategy component.

A Meta-Learner proposal targeting another strategy is a proposal to that governed Trading ownership/review path, not a write permission over the target strategy.

## 6. Origin-Correct Review Examples

### Strategy CSA origin

```text
CSA-T-CLS-001
-> T-LSA-04
-> MSA-T
-> FSA OS-governance/compatibility review
-> separate Owner/governance adoption decision
```

### Opportunity strategy CSA origin

```text
CSA-T-HNT-004
-> T-LSA-05
-> MSA-T
-> FSA OS-governance/compatibility review
-> separate Owner/governance adoption decision
```

### Meta-Learner origin

```text
CSA-T-META-001
-> T-LSA-12
-> MSA-T
-> FSA OS-governance/compatibility review
-> separate Owner/governance adoption decision
```

No lower tier is inserted beneath the actual origin, and no sibling LSA becomes a substitute parent.

## 7. FSTSimA Role

FSTSimA may execute governed simulation/validation work for an Application-owned candidate without becoming the business owner of that candidate.

```text
TEST EXECUTOR / EVIDENCE PRODUCER != CANDIDATE BUSINESS OWNER
```

FSTSimA validation success does not change candidate origin or grant promotion authority.

## 8. No New Authority

This clarification creates no implementation, runtime, research-egress, Paper, Shadow, Tiny Live, Live, deployment, FSA or self-promotion authority.
