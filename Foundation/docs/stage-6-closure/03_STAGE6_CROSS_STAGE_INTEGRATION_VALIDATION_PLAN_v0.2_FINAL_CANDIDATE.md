# Stage 6 — Cross-Stage Integration Validation Plan

Version: v0.2 FINAL CANDIDATE
Status: PROPOSED / OWNER REVIEW REQUIRED
Date: 2026-08-11
Scope: Stage-level pre-closure validation only
Supersedes for review: v0.1 proposal

## 1. Owner-directed objective

Before Stage 6 may be considered for final closure, Foundation shall prove that the accepted Stage 6 resource-governance implementation remains coherent with the complete accepted Foundation baseline that precedes it.

This gate is intentionally broader than WP-10.

WP-10 proved Stage 6 internal closure coherence across WP-01 through WP-09 plus the WP-10 evidence package. This new gate must prove both:

1. every accepted predecessor Stage has an explicit tested/bound relationship with Stage 6; and
2. the whole accepted chain remains coherent as one Foundation baseline.

## 2. Authority and non-authority

The Project Owner has explicitly directed this Stage-level validation before Stage 6 closure.

This document is the exact proposed validation design. Implementation begins only after explicit Owner acceptance of this plan.

Plan acceptance, when granted, authorizes only the verification/harness/evidence work described here.

It does NOT authorize:

- Stage 6 final closure;
- reopening any accepted Stage/WP by implication;
- production semantic changes;
- silent repair of a predecessor;
- Stage 7 planning or implementation;
- Application modification;
- deployment/runtime activation;
- external connectivity;
- financial/trading authority.

## 3. Closure preservation

The following remain closed while this gate is planned and executed:

- Stage 0A;
- Stage 0B;
- Stage 0C;
- Stage 1;
- Stage 2;
- Stage 3;
- Stage 4;
- Stage 5;
- Stage 6 WP-01 through WP-10.

A test failure is evidence requiring classification. It is not automatic reopening authority.

## 4. Controlling proof model

The gate has five distinct evidence layers. They are not interchangeable.

### Layer A — immutable accepted-history binding

For every Stage 0A through Stage 5, the validation package shall identify the exact accepted closure/authority evidence used as the predecessor truth source.

The package shall record, as available under each historical gate:

- Stage identity;
- accepted closure/authority locator;
- immutable commit/blob/file identity;
- SHA-256 when the accepted evidence is byte-addressable;
- historical gate note when a later documentary form did not exist.

No modern wrapper may be misrepresented as an original historical closure artifact.

### Layer B — historical executable regression/supporting evidence

Run all still-executable historical verifiers that can be invoked against the current exact candidate without fabricating a historical environment:

- Stage 0B verifier;
- Stage 0C verifier;
- Stage 0C remediation verifier with explicit arguments;
- Baseline Integrity verifier;
- Stage 2 WP-01..WP-04;
- Stage 3 WP-01..WP-06;
- Stage 4 WP-01..WP-06;
- Stage 5 WP-01..WP-10;
- Stage 6 WP-01..WP-10.

Historical rerun results are regression/supporting evidence, not standalone proof that Stage 6 integrates with every predecessor.

If a historical verifier fails, the failure must first be classified for current successor applicability. A historical tool that encoded a legitimately historical repository shape cannot silently become a retroactive new requirement. A proven semantic predecessor regression remains blocking.

### Layer C — current Foundation-wide executable baseline

On one exact detached candidate:

1. exact SDK `10.0.302`;
2. controlled-solution Restore;
3. controlled-solution Release Build;
4. Foundation Architecture validation;
5. Foundation Security validation;
6. exact project/solution boundary check;
7. no `applications/**` or `reference/**` participation;
8. exact HEAD and clean-tree checks before and after validation.

Stage 0B/0C historical verifier projects remain outside current controlled-solution membership unless already present. They shall be restored/built explicitly for regression only.

The new current cross-stage verifier shall be added to the controlled Foundation solution because it becomes part of the current Stage 6 closure-validation surface.

No build is permitted after executable validation begins. All executable gates after the build phase use `--no-build` / `--no-restore` where supported.

### Layer D — mandatory predecessor-to-Stage6 binding matrix

The dedicated current verifier must expose an exact matrix. No predecessor Stage may be represented only by an aggregate claim.

Required rows:

#### Stage 0A <-> Stage 6

Positive proof:
- Stage 6 consumes only authority available under governed Foundation execution and does not convert preparation/authorization history into runtime authority.

Fail-closed proof:
- missing/invalid governing authority identity cannot become permission to allocate, mutate, reclaim, rebalance or restore resources.

Evidence proof:
- exact Stage 0A accepted authority/closure evidence is bound into the validation package.

#### Stage 0B <-> Stage 6

Positive proof:
- canonical identity/time/randomness/security primitives used by current Foundation behavior remain compatible with deterministic Stage 6 resource identities and evidence.

Fail-closed proof:
- malformed/noncanonical/ambiguous identity material cannot create Stage 6 resource truth or authority.

#### Stage 0C <-> Stage 6

Positive proof:
- activated enabling-provider behavior used by current Foundation remains compatible with Stage 6 deterministic decisions/evidence.

Fail-closed proof:
- invalid/restricted/revoked enabling state cannot be converted into Stage 6 permission.

#### Stage 1 <-> Stage 6

Positive proof:
- Stage 6 remains inside the accepted controlled project/architecture boundary and builds as part of the Foundation-only solution.

Fail-closed proof:
- Application/reference leakage or undeclared project-boundary dependency fails Architecture/current solution checks.

#### Stage 2 <-> Stage 6

Positive proof:
- Stage 6 resource requests, decisions, state/evidence identities and public declarations comply with accepted contract/schema/evidence primitives.

Fail-closed proof:
- incompatible schema/version/identity/evidence binding is rejected rather than coerced.

#### Stage 3 <-> Stage 6

Positive proof:
- Stage 6 respects trusted bootstrap/configuration/dependency-governance truth and exact dependency identity.

Fail-closed proof:
- unavailable dependency, invalid delegation chain, missing exact version/identity or impossible activation order cannot produce Stage 6 resource authority.

#### Stage 4 <-> Stage 6

Positive proof:
- Stage 6 mutations obey accepted authority, lifecycle, state, evidence and reconciliation boundaries.

Fail-closed proof:
- suspended/revoked/expired/unauthorized/invalid-state subjects cannot gain or restore resource authority; evidence conflict remains visible.

#### Stage 5 <-> Stage 6

Positive proof:
- Stage 6 Application-facing resource-state/load-shedding signals use the accepted communication/event/admission/routing/delivery boundary without creating a second transport authority.

Fail-closed proof:
- unauthoritative, replayed, duplicate, stale, conflicting or incompatible delivery cannot create duplicate resource authority or silently mutate current truth.

### Layer E — mandatory whole-chain scenarios

A matrix is necessary but not sufficient. At least one deterministic scenario must cross the full accepted chain.

#### Whole-chain positive scenario

The scenario shall bind, in one causally traceable flow:

`governed preparation/authority`
-> `canonical identity/time/security primitives`
-> `controlled project boundary`
-> `contract/schema/evidence identity`
-> `bootstrap/configuration/dependency truth`
-> `authority/lifecycle/state/evidence truth`
-> `Stage 5 admitted communication/event path`
-> `Stage 6 resource request/decision/mutation/projection`
-> `attributable resulting evidence`

The result shall prove that Stage 6 can operate only through the accepted preceding Foundation rules rather than beside them.

#### Whole-chain negative variants

At minimum mutate one element from each predecessor family and prove fail-closed behavior for the exact expected reason:

- authority missing/invalid;
- identity noncanonical;
- project/ownership boundary violation;
- schema/version mismatch;
- dependency unavailable or delegation invalid;
- lifecycle restricted/revoked;
- event replay/duplicate/stale/conflicting;
- cross-Application resource isolation violation;
- protected floor/reserve violation attempt.

No negative case may succeed merely because a downstream Stage 6 object is otherwise syntactically valid.

## 5. Dedicated current verifier

Proposed project:

`verification/Falcon.Stage6.CrossStageIntegration.Verifier/Falcon.Stage6.CrossStageIntegration.Verifier.csproj`

It is verification-only.

It shall:

- reference only the minimum accepted Foundation production projects/contracts required for public-behavior validation;
- not modify production source;
- avoid duplicating predecessor verifier internals where public observable behavior can prove the same boundary;
- emit deterministic named scenario results;
- expose exact reason codes for negative/fail-closed scenarios;
- produce a deterministic integrated result identity from exact inputs;
- include mutation-sensitivity cases;
- run twice from the same Release outputs.

The verifier shall be added to `Falcon.Foundation.ControlledProjectFoundation.slnx` only after plan acceptance as part of the current validation surface.

## 6. Stage 0A and Stage 1 special handling

### Stage 0A

Current repository truth has no dedicated Stage 0A executable verifier.

Therefore Stage 0A is proven through:

- exact immutable authority/closure evidence binding;
- current governance/workstream-boundary validation;
- dedicated Stage0A<->Stage6 positive and fail-closed scenarios.

No fake historical runtime verifier shall be invented.

### Stage 1

Current repository truth has no `Falcon.Stage1.Verifier` project.

Therefore Stage 1 is proven through:

- exact controlled-solution identity;
- Restore/Release Build;
- Architecture validation;
- Security validation;
- Foundation-only project/workstream boundary;
- dedicated Stage1<->Stage6 project/ownership dependency scenarios.

No fake historical Stage 1 verifier shall be invented.

## 7. Evidence isolation

All generated executable evidence shall be outside the detached repository worktree under an isolated validation root, for example:

`C:\Falcon\Stage6-CrossStage-Validation\<timestamp>\Evidence`

This includes:

- Stage 0B `--evidence` output;
- Stage 0C `--evidence` output;
- Stage 0C remediation `--evidence` output;
- Stage 0C remediation `--trace` output;
- transcript;
- hash records.

The repository must remain clean before and after validation.

## 8. Exact build/run phase separation

### Build phase

1. clone exact Foundation branch with required history;
2. freeze exact candidate;
3. detached checkout;
4. pre-clean check;
5. SDK check;
6. restore controlled solution;
7. restore Stage 0B/0C/remediation historical verifier projects explicitly;
8. Release build controlled solution once;
9. Release build Stage 0B/0C/remediation historical verifier projects once if not produced by the controlled solution;
10. record exact current cross-stage verifier DLL SHA-256.

After build phase completes, no build/restore is permitted.

### Run phase

1. Stage 0B regression;
2. Stage 0C regression;
3. Stage 0C remediation regression;
4. Baseline Integrity;
5. Foundation Architecture;
6. Foundation Security;
7. Stage 2 WP-01..WP-04;
8. Stage 3 WP-01..WP-06;
9. Stage 4 WP-01..WP-06;
10. Stage 5 WP-01..WP-10;
11. Stage 6 WP-01..WP-10;
12. Cross-Stage Integration run 1;
13. Cross-Stage Integration run 2 from same Release outputs;
14. cross-stage verifier DLL SHA-256 after run 2;
15. final exact HEAD;
16. final clean tree;
17. refresh remote;
18. verify candidate still equals remote Foundation branch;
19. hash complete transcript.

## 9. Historical verifier applicability rule

A historical verifier result has three possible dispositions:

- `PASS_CURRENTLY_APPLICABLE`;
- `FAIL_CURRENT_SEMANTIC_REGRESSION`;
- `NOT_APPLICABLE_AS_SUCCESSOR_GATE_REQUIRES_HISTORICAL_EVIDENCE_BINDING`.

`NOT_APPLICABLE...` is not a free pass. It requires documented proof that the failure is caused only by legitimate successor repository evolution outside the accepted historical scope, plus preserved immutable historical closure evidence.

A real semantic regression is blocking.

## 10. Additional Stage6 continuity scenarios

The dedicated verifier must also prove:

- zero Applications is valid;
- multiple Applications remain isolated;
- one Application cannot consume another's grant/state;
- protected floors and recovery reserves cannot be silently breached;
- Foundation technical criticality/resource priority cannot be minted by Application business preference;
- Stage 6 creates no financial/trading authority;
- Stage 6 does not require Stage 7+ behavior;
- no Stage 7+ authority is present in the test surface.

## 11. Failure classification

Every failure is classified before remediation:

- `CROSS_STAGE_VERIFIER_OR_HARNESS_DEFECT`
- `HISTORICAL_VERIFIER_SUCCESSOR_APPLICABILITY_CONFLICT`
- `PREDECESSOR_TO_STAGE6_COMPATIBILITY_DEFECT`
- `WHOLE_CHAIN_COMPATIBILITY_DEFECT`
- `PREDECESSOR_ACCEPTED_SCOPE_DEFECT_REQUIRES_EXPLICIT_TRACE`
- `STAGE6_ACCEPTED_SCOPE_DEFECT_REQUIRES_EXPLICIT_TRACE`
- `AUTHORITY_OR_GOVERNANCE_CONFLICT`
- `UNRESOLVED_FCR_OR_DOCUMENTARY_BLOCKER`

Only verifier/harness/evidence-package defects may be fixed under the bounded validation authority after plan acceptance.

A true defect inside a closed Stage/WP requires exact defect evidence and separate governed remediation authority.

## 12. Required evidence package

The final package shall contain:

- exact candidate SHA;
- exact remote Foundation branch SHA before/after;
- exact SDK;
- immutable predecessor Stage evidence matrix;
- explicit predecessor-to-Stage6 binding matrix with scenario IDs;
- historical verifier applicability dispositions;
- all executable summaries and exit codes;
- whole-chain scenario results;
- Stage 0B/0C/remediation generated evidence outside repo;
- cross-stage verifier run 1/run 2 results;
- cross-stage verifier DLL SHA-256 before/after;
- exact HEAD/clean-tree proof;
- transcript SHA-256;
- failure classification records if applicable.

## 13. Red-Team gates

### Pre-executable Red-Team

Must verify:

- every Stage 0A..5 has an explicit Stage6 binding row;
- whole-chain scenario cannot be satisfied by isolated mocks only;
- public production semantics rather than verifier self-claims are being tested;
- no predecessor is silently reinterpreted;
- no Application/reference scope is modified;
- no Stage 7+ authority leaks backward;
- generated evidence stays outside repo;
- no build occurs during run phase.

### Post-executable Red-Team

Must challenge:

- false PASS from independent verifier aggregation;
- untested Stage-to-Stage edge;
- whole-chain bypass;
- authority/lifecycle/state/evidence contradiction;
- Stage 5 communication vs Stage 6 signal incompatibility;
- cross-Application resource leakage;
- protected-floor/reserve violation;
- zero-Application regression;
- historical closure reinterpretation;
- stale/moving candidate;
- test harness mutation of outputs.

## 14. Stage 6 closure rule

A PASS from this validation does not itself close Stage 6.

Required sequence:

`PLAN_ACCEPTED`
-> `VALIDATION_VERIFIER_IMPLEMENTED`
-> `PRE_EXECUTABLE_RED_TEAM_PASS`
-> `EXACT_CROSS_STAGE_EXECUTABLE_VALIDATION_PASS`
-> `POST_EXECUTABLE_RED_TEAM_PASS`
-> `FINAL_STAGE6_CLOSURE_READINESS_REPORT`
-> `SEPARATE_OWNER_STAGE6_CLOSURE_DECISION`

## 15. Current state

`STAGE6_WP01_TO_WP10 = ACCEPTED_AND_CLOSED`

`STAGE6 = OPEN`

`CROSS_STAGE_VALIDATION_PLAN = PROPOSED_v0.2_FINAL_CANDIDATE`

`CROSS_STAGE_VALIDATION_IMPLEMENTATION = NOT_YET`

`STAGE6_OWNER_CLOSURE = NOT_YET`

`STAGE7_PLANNING_AUTHORITY = NOT_GRANTED`

`STAGE7_IMPLEMENTATION_AUTHORITY = NOT_GRANTED`
