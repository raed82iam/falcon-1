# Stage 6 Cross-Stage Integration Validation Plan — Red-Team V1

Date: 2026-08-11
Reviewed plan: `01_STAGE6_CROSS_STAGE_INTEGRATION_VALIDATION_PLAN_v0.1.md`
Disposition: REWORK_REQUIRED

## Severity summary

- Critical: 0
- High: 2
- Medium: 4
- Low: 0

## HIGH-01 — No explicit stage-by-stage binding matrix

The proposed verifier responsibilities are grouped by semantic concern but do not require an explicit proof row for every accepted predecessor Stage against Stage 6.

Risk: individual semantic checks and historical verifier reruns could all pass while one predecessor-to-Stage6 boundary remains untested.

Required correction: mandate an exact binding matrix covering Stage 0A, Stage 0B, Stage 0C, Stage 1, Stage 2, Stage 3, Stage 4 and Stage 5 against Stage 6, with at least one positive and one fail-closed/negative proof where technically meaningful.

## HIGH-02 — Historical verifier reruns can be confused with cross-stage closure proof

Some historical verifiers were written for historical candidate/activation gates and may not have been designed as perpetual successor-compatibility gates.

Risk: a historical verifier could fail because later repository structure legitimately evolved outside its accepted scope, creating a false Stage 6 blocker; conversely, a PASS does not by itself prove current cross-stage integration.

Required correction: classify historical verifier execution as regression/supporting evidence. A historical-verifier failure must first be classified for current applicability before it can become a Stage 6 closure blocker. The dedicated current cross-stage verifier plus current Architecture/Security/accepted-current verifiers remain the controlling integration proof.

## MEDIUM-01 — Missing whole-chain end-to-end scenario

The plan lacks one mandatory scenario that crosses the accepted chain from foundational identity/encoding through contract/schema, dependency/bootstrap, authority/lifecycle/state/evidence, communication/event transport, and finally Stage 6 resource governance and Application-facing resource-state/load-shedding projection.

Required correction: add at least one deterministic whole-chain positive scenario and mutation-based fail-closed variants.

## MEDIUM-02 — New verifier solution-membership rule is ambiguous

The plan proposes a new verifier project but does not state whether it enters the controlled solution or is built separately.

Required correction: define exact solution membership. Recommended: add the new current Stage-level verifier to the controlled solution because it is part of the current validation surface, while leaving historical Stage 0 verifier membership unchanged.

## MEDIUM-03 — Generated historical evidence can dirty the tested repository

Stage 0B/0C and especially Stage 0C remediation can write evidence/trace files. If those paths are inside the detached repository, the final clean-tree gate would fail for harness reasons or, worse, generated evidence could become part of tested state.

Required correction: all generated validation evidence must be written to an isolated evidence root outside the repository worktree.

## MEDIUM-04 — Stage 0A and Stage 1 evidence model needs exact proof

The plan correctly states that current repository truth has no dedicated Stage 0A or Stage 1 verifier, but the substitute checks are too general.

Required correction:
- Stage 0A: bind exact accepted authority/closure evidence and prove no current Stage 6 path bypasses those governance/non-authority constraints.
- Stage 1: verify controlled project/solution graph, architecture boundary, security baseline, and Foundation-only workstream isolation as the current executable continuation of the accepted project-foundation scope.

## Red-Team conclusion

`PLAN_v0.1 = NOT_READY_FOR_OWNER_ACCEPTANCE`

The objective is valid and compatible with current governance, but the test design must be tightened before implementation authority can be safely derived from Owner acceptance.

No production change is authorized by this review.
No accepted Stage/WP is reopened.
Stage 6 remains open.
Stage 7 authority remains NOT GRANTED.
