# Verification Standard

**Identifier:** STD-008  
**Version:** 1.0  
**Status:** Approved  
**Effective Date:** 2026-07-24  
**Approval Record:** GOV-006  
**Owner:** Falcon Standards Authority  
**Governing Authority:** Falcon Constitution Articles 13, 16–18, 24, 30–34, 36A–36D, and 40–44
**Supersedes:** None  
**Superseded By:** None

## 1. Purpose

This Standard defines how Falcon verification plans, executions, evidence, results, and independence are governed.

Verification demonstrates claims within a declared scope. It does not create authority, guarantee absence of defects, or convert a lower artifact into higher authority.

## 2. Applicability

Verification is required for:

- Specification acceptance criteria;
- Contract conformance;
- release invariants and scenarios;
- security, recovery, rollback, and Safe-state claims;
- capital and risk controls;
- capability admission and replacement;
- self-maintenance and self-evolution evidence;
- authority, revocation, and independence claims; and
- correction of material non-conformance.

## 3. Required Verification Plan

Every material verification plan SHALL identify:

1. plan ID, version, owner, status, and governing sources;
2. claim, requirement, invariant, or Contract being verified;
3. scope and explicit non-scope;
4. prerequisites and environment;
5. subject and artifact identities;
6. initial authoritative state;
7. roles, permissions, independence, and conflicts;
8. controlled inputs, fixtures, and fault injections;
9. procedure and observation points;
10. expected positive, negative, degraded, and abuse outcomes;
11. evidence to be collected and integrity protection;
12. pass, fail, inconclusive, and blocked criteria;
13. cleanup, rollback, and containment;
14. repeatability and known limitations; and
15. required approval and review.

## 4. Traceability

Every verification claim SHALL trace to an approved requirement, Contract, accepted ADR consequence, release invariant, or approved risk control.

Every required source SHALL trace forward to one or more verification claims. Unverified mandatory requirements SHALL remain visible and SHALL block any claim of complete conformance.

Tests SHALL NOT create hidden requirements.

## 5. Environment and Subject Identity

Verification evidence SHALL identify the exact subject, artifact, dependency, configuration, authority baseline, data set, environment, and time conditions evaluated.

Results from simulation, test, Digital Twin, Shadow, Canary, or non-authoritative environments SHALL state the differences from the target environment.

Evidence from one artifact or configuration SHALL NOT be transferred silently to another.

## 6. Required Cases

Verification SHALL include, where applicable:

- expected valid behavior;
- invalid, missing, stale, duplicate, expired, revoked, and contradictory inputs;
- unauthorized and wrong-context actions;
- boundary, concurrency, partial-failure, and uncertain-outcome cases;
- dependency and communication loss;
- restart, restoration, rollback, and repeated attempt;
- abuse and bypass attempts;
- evidence loss or corruption; and
- safe degradation and stop behavior.

The absence of an applicable negative case SHALL be justified.

## 7. Result Vocabulary

- **PASS:** Complete valid evidence satisfies every required criterion.
- **FAIL:** Evidence contradicts or fails at least one required criterion.
- **INCONCLUSIVE:** Required evidence is missing, corrupt, stale, contradictory, or outside declared assurance.
- **BLOCKED:** A prerequisite prevents safe execution before the verification begins.

Only `PASS` satisfies a verification criterion. `INCONCLUSIVE` and `BLOCKED` SHALL NOT be converted to `PASS` by assumption, schedule pressure, or approval.

## 8. Evidence

Verification evidence SHALL be attributable, reproducible where practical, version-bound, time-bound, complete for the claim, protected against undetected alteration, and explicit about uncertainty and omitted coverage.

The record SHALL distinguish setup, stimulus, observation, assertion, result, and reviewer decision.

Secrets and prohibited sensitive values SHALL NOT enter ordinary verification evidence.

## 9. Independence

Required independence SHALL be proportionate to consequence.

The actor that implements, repairs, restores, or promotes a high-consequence subject SHALL NOT be its sole verifier or approving authority.

Independent verification SHALL use independently obtained or integrity-verified evidence and SHALL not rely solely on the subject’s self-report.

Automation may support independent verification only when identity, authority, evidence source, decision logic, and ability to challenge the subject remain meaningfully separated.

## 10. Repeatability, Flakiness, and Change

A repeatable verification SHALL define controlled inputs and permitted variation.

An intermittent result SHALL be treated as unresolved evidence, not averaged into success. Retry SHALL preserve every prior result and reason.

Material change to subject, environment, dependency, configuration, authority, schema, or verification method SHALL trigger impact review and re-verification.

## 11. Containment and Cleanup

Fault injection and adverse testing SHALL be authorized, bounded, observable, reversible where practical, and incapable of creating unauthorized financial consequence.

Cleanup SHALL restore or reconcile authoritative state and preserve test evidence. Cleanup success SHALL be verified separately from test execution where consequence requires.

## 12. Prohibited Practices

Verification SHALL NOT:

- test only expected success;
- use production capital or financial authority without explicit higher authorization;
- modify evidence after outcome;
- select or omit results to create a favorable appearance;
- treat mocks or simulation as target-environment proof without qualification;
- accept a self-reported state as sole evidence where independent observation is required;
- allow a passing local test to waive a failed invariant; or
- claim complete coverage without traceability evidence.

## 13. Compatibility and Transition

Verification evidence accepted before this Standard remains historical evidence within its declared assurance and SHALL NOT be relabeled.

New or materially changed verification plans and every verification used for a new release, promotion, authority grant, or risk acceptance SHALL conform. Missing historical coverage SHALL remain an explicit limitation.

## 14. Acceptance Evidence

Conformance requires:

- complete requirement-to-verification traceability;
- identified subject and environment;
- positive, negative, degraded, and abuse coverage as applicable;
- explicit result criteria;
- protected evidence;
- proportionate independence;
- controlled containment and cleanup;
- preserved failed and inconclusive results; and
- successful authorized reconstruction.

## 15. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Falcon Standards Authority | Approved | GOV-006 | 2026-07-24 |
