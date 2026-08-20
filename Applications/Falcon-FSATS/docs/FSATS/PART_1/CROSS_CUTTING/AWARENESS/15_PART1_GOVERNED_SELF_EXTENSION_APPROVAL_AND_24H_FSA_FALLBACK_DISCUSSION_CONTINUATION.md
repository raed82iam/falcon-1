# FSATS Part 1 — Governed Self-Extension Approval, Evidence, Rollback and 24-Hour FSA Fallback Discussion Continuation

**Status:** `DESIGN_DISCUSSION_RECORD / OWNER-DIRECTED / NOT_OWNER_ACCEPTED / NOT_CLOSED`  
**Branch:** `application-development`  
**Authority Type:** `DESIGN DISCUSSION ONLY`  
**Implementation Authority:** `NOT GRANTED`  
**Runtime Authority:** `NOT GRANTED`  
**Paper / Shadow / Tiny-Live / Live Authority:** `NOT GRANTED BY THIS ARTIFACT`  
**Part 0 Historical Baseline:** `PRESERVED / NOT REWRITTEN`  
**Continuation Of:** `14_PART1_GOVERNED_SELF_EXTENSION_RESEARCH_AND_CONTAINMENT_DISCUSSION_RECORD.md`

---

## 1. Purpose

This file continues and preserves the same Owner discussion recorded in `14_PART1_GOVERNED_SELF_EXTENSION_RESEARCH_AND_CONTAINMENT_DISCUSSION_RECORD.md`.

The two files SHALL be read together as one discussion set.

This continuation captures the later Owner clarifications concerning:

- the exact self-development evaluation and approval chain;
- the role of CSA and LSA as specialized research/development entities;
- mandatory explanation of why a development exists and what benefit it is expected to provide;
- FSTSimA experimental evidence and product-quality assessment;
- MSA Application-level evaluation;
- independent validation and Red-Team review;
- FSA system-level governance review rather than Trading/business review;
- Owner Review Page and the 24-hour review window;
- bounded FSA fallback approval after Owner non-response;
- classes that FSA may never approve;
- mandatory rollback for every FSA-approved candidate;
- exact-candidate, evidence and revalidation requirements;
- unresolved Risk design terminology and the separation of Risk authority from Risk model development.

This file does not activate any of these mechanisms. It preserves design intent for later governed reconciliation and final Part 1 review.

---

## 2. Core Approval Principle

The primary approval authority for self-developed production changes remains the Owner.

A technically successful candidate does not approve itself.

The intended invariant is:

`BUILD_PASS != TEST_PASS != FSTSIMA_PASS != MSA_RECOMMENDATION != RED_TEAM_PASS != FSA_SYSTEM_PASS != OWNER_APPROVAL`

A candidate may progress automatically through delegated research, development, simulation and remediation only within its authorized evolution envelope.

Production adoption remains separately governed.

---

## 3. CSA / LSA — Specialized Research, Development and Initial Evidence

CSA and LSA are the primary specialized research and self-development entities under the current Owner-directed model.

A CSA / LSA development proposal shall not begin merely with `I found something better`.

The proposing entity must establish an attributable development rationale that explains at minimum:

- the exact current problem, weakness, limitation or capability gap;
- why the issue belongs to the proposing entity's specialization;
- why development is justified now;
- the expected benefit;
- how the expected benefit will be measured;
- the proposed scope of change;
- assets/components/models/code expected to change;
- known risks and possible side effects;
- research used and its provenance when Internet research was used;
- exact tests required;
- candidate identity/version;
- rollback expectations.

The entity may then, only inside its legitimate scope and Mission/Research Scope Contract:

`RESEARCH -> DESIGN -> DEVELOP -> TEST -> REMEDIATE -> RETEST -> PRODUCE_EVIDENCE`

The CSA/LSA shall preserve both successful and failed evidence. It shall not hide unsuccessful experiments or redefine success criteria after seeing the result.

---

## 4. Mandatory FSTSimA Experimental Assessment

Where the product or change is materially testable through simulation, FSTSimA shall provide independent experimental evidence before the candidate reaches final approval review.

FSTSimA does not approve the candidate.

It evaluates what happened under controlled experimentation.

The FSTSimA report should eventually cover, as applicable:

- exact candidate identity tested;
- scenario set and scenario provenance;
- historical replay results;
- Synthetic Market results;
- rare/extreme/adversarial scenario results;
- failure-injection results;
- deterministic/reproducibility status;
- fidelity status;
- calibration status;
- performance and latency behavior;
- resource-pressure behavior;
- failure/degraded behavior;
- recovery behavior;
- edge-case coverage;
- observed regressions;
- unresolved limitations;
- confidence in the evidence;
- whether the product behaved as claimed by its developer;
- product-quality assessment within FSTSimA's validation jurisdiction.

The intended separation is:

`DEVELOPER_CLAIM != EXPERIMENTAL_RESULT`

FSTSimA exists to provide independent experimental truth about the candidate, not to promote it.

---

## 5. MSA — Application-Level Evaluation and Recommendation

The MSA evaluates the candidate from the perspective of the complete Application.

It does not replace the specialized CSA/LSA and does not become the developer of every branch.

The MSA should evaluate at minimum:

- whether the requested development actually belongs to the proposing CSA/LSA specialization;
- whether the proposer stayed inside its ownership and authority;
- whether the candidate solves the stated problem;
- whether the claimed benefit is real and relevant to the Application;
- whether a local improvement harms another LSA/component;
- cross-branch effects;
- state effects;
- contract effects;
- dependency effects;
- permission/authority effects;
- resource effects;
- failure/degraded effects;
- persistence and migration effects where applicable;
- compatibility with existing Application behavior;
- compatibility with FSTSimA experimental results;
- whether any unexpected or undeclared behavior appeared;
- whether rollback is operationally feasible at the Application level;
- whether the candidate should be recommended, rejected or returned for rework.

Possible recommendation states may include:

- `RECOMMEND_APPROVAL`
- `RECOMMEND_REJECTION`
- `REQUIRES_REWORK`
- `INSUFFICIENT_EVIDENCE`

The MSA recommendation is not production approval.

---

## 6. Independent Validation and Red-Team Review

A self-developed candidate shall receive an independent challenge stage before Owner review when the consequence class requires it.

The independent validation / Red-Team function must not merely repeat unit tests.

It should challenge the complete approval package, including:

- the candidate itself;
- CSA/LSA rationale;
- claimed specialization ownership;
- claimed benefit;
- test design;
- FSTSimA evidence;
- MSA evaluation;
- security effects;
- governance effects;
- architecture effects;
- contract effects;
- authority effects;
- hidden or undeclared behavior;
- evidence completeness;
- evidence contradictions;
- reward-hacking or metric manipulation;
- changed acceptance criteria;
- scope creep;
- privilege expansion;
- dependency changes;
- rollback credibility;
- whether the tested artifact is the exact artifact proposed for approval.

The Red-Team should explicitly test whether the developer or MSA has unintentionally or deliberately reframed the problem to make the candidate appear successful.

`BUILDER != FINAL_VERIFIER`

`MSA_RECOMMENDATION != UNCHALLENGEABLE_TRUTH`

The exact runtime placement/actor for independent validation and Red-Team remains an open architecture question. No new Application or authority is created by this discussion record.

---

## 7. FSA — System-Level Governance Review Only

FSA reviews the candidate from the Falcon system/governance perspective.

FSA SHALL NOT re-evaluate the candidate as a Trading expert.

For a Trading strategy, FSA does not decide whether the strategy is profitable, which market is attractive or whether a signal is economically superior.

Those judgments remain inside legitimate Application ownership and the evidence chain below FSA.

FSA should evaluate at minimum:

- Vision conformance;
- Constitution conformance;
- governance conformance;
- authority correctness;
- Application ownership/boundary correctness;
- cross-Application isolation;
- Foundation compatibility;
- architecture compatibility;
- security boundaries;
- permission boundaries;
- containment boundaries;
- self-development rules;
- Research Scope compliance where applicable;
- whether any scope/authority expansion occurred;
- whether any hidden Foundation substitute was created;
- exact candidate/evidence identity;
- evidence completeness and consistency from the system perspective;
- whether the MSA submitting the recommendation is the legitimate MSA for that Application;
- whether the MSA attestation/evaluation is authentic and unmodified;
- whether the MSA evaluation remained within its jurisdiction;
- whether the MSA omitted a system-level issue that should have blocked progression;
- whether the candidate preserves rollback/removal requirements;
- whether the candidate belongs to an FSA-fallback-eligible action class.

FSA should perform a system-level Red-Team/challenge of the MSA recommendation and the complete package from the OS/governance perspective.

This does not make FSA a Trading Risk, strategy, market, Provider, Broker or execution authority.

`FSA_SYSTEM_REVIEW != APPLICATION_BUSINESS_REVIEW`

---

## 8. MSA Attestation Verification

A specific Owner clarification is that FSA must not trust an MSA recommendation merely because it originated from an MSA-labelled entity.

FSA should verify at minimum:

- exact registered Application identity;
- exact registered MSA identity;
- origin and provenance of the recommendation;
- integrity of the MSA evaluation package;
- binding between the MSA recommendation and the exact candidate;
- binding between the MSA recommendation and the exact FSTSimA / validation evidence;
- whether the recommendation changed after validation;
- whether the MSA had authority to issue the evaluation;
- whether the MSA evaluated an object outside its Application scope;
- whether the MSA attempted to convert recommendation authority into adoption authority.

Any unresolved authenticity, provenance or authority issue blocks FSA fallback approval.

---

## 9. Complete Owner Evidence Package

The Owner should receive one coherent Approval Package rather than a collection of unrelated messages.

The complete package should eventually contain at minimum:

- development reason;
- current problem/gap;
- expected benefit;
- benefit measurement;
- proposer identity and specialization;
- exact candidate identity/version/hash;
- changed components/assets;
- research summary and provenance where applicable;
- developer test report;
- failed/negative test evidence;
- FSTSimA experimental/quality report;
- MSA Application-level evaluation and recommendation;
- independent validation report;
- Red-Team report;
- FSA system/governance review;
- security/authority findings;
- unresolved limitations;
- exact dependencies/configuration relevant to approval;
- rollback plan;
- rollback validation evidence;
- approval-class identity;
- whether the candidate is eligible for FSA 24-hour fallback;
- exact Owner decision requested.

A concise Owner-facing summary should make the key decision understandable without forcing the Owner to read every low-level artifact first.

---

## 10. Owner Review Page

The candidate enters the Owner approval state only after the complete required evidence package is present and the exact candidate is frozen for review.

The review state should be explicit, for example:

`OWNER_REVIEW_READY`

Only this state starts the proposed 24-hour window.

The Owner review surface should expose at minimum:

- What changed?
- Why was it changed?
- What benefit is expected and what benefit was demonstrated?
- What are the risks?
- What did FSTSimA find?
- What did MSA recommend?
- What did independent validation / Red-Team find?
- What did FSA conclude from the Falcon system/governance perspective?
- Is rollback available and tested?
- Is this action class `FSA_24H_FALLBACK_ELIGIBLE`?
- What exact decision is requested from the Owner?

Owner actions should include at minimum:

- `APPROVE`
- `REJECT`
- `REQUEST_CHANGES`
- `HOLD`

`HOLD` stops the fallback progression while the Owner deliberately retains the decision.

---

## 11. 24-Hour Window Principle

The 24-hour mechanism is NOT silent approval.

The intended principle is:

`OWNER_SILENCE != OWNER_APPROVAL`

`TIMER_EXPIRY != NEW_AUTHORITY`

The FSA fallback authority exists only because the Owner has explicitly pre-delegated a bounded action class to FSA before the specific candidate reaches the review window.

The timer starts only when:

- all mandatory evidence is complete;
- the exact candidate is frozen;
- the Owner Review Page is available;
- state is `OWNER_REVIEW_READY`;
- the exact review-ready timestamp is immutably recorded.

The timer shall not start from developer completion, first test PASS, FSTSimA completion, MSA recommendation or FSA preliminary review.

---

## 12. Owner Action During the 24-Hour Window

If the Owner explicitly approves, the Owner decision controls.

If the Owner rejects, the candidate is rejected.

If the Owner requests changes, the 24-hour window is invalidated and a changed candidate must repeat the required review chain.

If the Owner selects `HOLD`, fallback progression stops until the Owner releases or decides the item.

The FSA fallback path exists only when the Owner provides no explicit decision within the valid review window and the candidate remains otherwise eligible.

---

## 13. FSA Fallback Approval Is a Pre-Delegated Exception

FSA is not a substitute Owner.

It may approve after the 24-hour window only for action classes that the Owner explicitly classified in advance as eligible.

Every candidate must carry an action-class decision such as:

- `FSA_24H_FALLBACK_ELIGIBLE = YES`
- `FSA_24H_FALLBACK_ELIGIBLE = NO`
- `FSA_24H_FALLBACK_ELIGIBLE = CONDITIONAL`

FSA SHALL NOT decide for itself that a new class should be eligible merely because the change appears safe.

The Delegation Matrix itself is Owner-controlled.

---

## 14. FSA Fallback Approval Requires Exact Compliance

The fallback path is intentionally stricter than ordinary discretionary Owner approval.

FSA shall not use approximate compliance or interpret away missing requirements.

The proposed principle is:

`FSA_FALLBACK_REQUIRES_EXACT_COMPLIANCE`

The fallback review must establish at minimum:

- all required contracts satisfied;
- all required tests completed;
- all required evidence present;
- no unresolved Critical/High/Medium finding as defined by the applicable policy;
- no unexplained behavior;
- no authority expansion;
- no scope expansion;
- no hidden dependency;
- no evidence conflict;
- no mismatch between tested and proposed candidate;
- no missing rollback requirement;
- action class explicitly FSA-eligible;
- no Owner HOLD/REJECT/CHANGE decision;
- exact candidate unchanged since `OWNER_REVIEW_READY`.

Where the applicable contract requires exact artifact identity, any material difference blocks fallback.

FSA verifies compliance. It does not reinterpret the governing requirements to make a candidate eligible.

---

## 15. Exact Candidate Binding

Approval must bind to the exact candidate that was reviewed.

The approval package should eventually bind at minimum:

- candidate identity;
- version;
- exact digest/hash where applicable;
- code/model artifact identity;
- configuration identity where material;
- dependency identities/versions where material;
- test evidence;
- FSTSimA evidence;
- MSA recommendation;
- independent validation / Red-Team evidence;
- FSA review;
- rollback package.

A semantic change to the candidate after review invalidates the previous approval window.

The changed candidate must return through the appropriate development, test, review and Owner-review lifecycle.

`REVIEWED_CANDIDATE_A != MODIFIED_CANDIDATE_B`

---

## 16. Mandatory Rollback for FSA Approval

A major Owner clarification is now explicit:

**Every candidate approved through FSA fallback must have a rollback plan.**

More strongly:

**FSA fallback approval requires a tested, evidence-backed rollback plan bound to the exact candidate.**

The rollback package should eventually include, as applicable:

- exact prior trusted state/version;
- trigger conditions for rollback;
- authority allowed to trigger rollback;
- rollback steps;
- affected dependencies;
- state/data migration reversal or compatibility handling where needed;
- route/configuration restoration;
- validation after rollback;
- evidence preserved across rollback;
- maximum acceptable rollback time;
- failure handling if rollback itself fails;
- proof that rollback was tested in an appropriate environment.

If rollback is absent, untested, ambiguous or not proven feasible:

`FSA_FALLBACK_APPROVAL = NOT_PERMITTED`

The Owner may later decide separately whether a non-rollbackable high-consequence change is acceptable, but FSA shall not take that discretionary risk under the 24-hour fallback mechanism.

---

## 17. Automatic Cancellation of the 24-Hour Window

The review window should fail closed if the evidence package ceases to be trustworthy or current.

Candidate cancellation triggers may include:

- candidate hash/version changes;
- semantic code/model/configuration change;
- new Red-Team finding;
- new security finding;
- new contradictory evidence;
- dependency change that affects validity;
- contract change;
- permission/authority change;
- rollback invalidation;
- FSTSimA evidence invalidation;
- MSA recommendation invalidation;
- FSA system concern;
- Owner `REQUEST_CHANGES`;
- Owner `REJECT`;
- other governed validity failure.

After cancellation, a new complete review-ready package and new timestamp are required before any future fallback window.

---

## 18. FSA Final Revalidation After Timer Expiry

Timer expiry does not itself produce approval.

After the valid 24-hour period expires without an Owner decision, FSA must perform a final revalidation immediately before exercising its delegated authority.

The revalidation should confirm at minimum:

- exact candidate unchanged;
- exact evidence package unchanged or validly supplemented without semantic drift;
- all contracts still satisfied;
- all tests/reviews still valid;
- no new findings;
- no new security event relevant to the candidate;
- dependencies/configuration still valid;
- rollback still valid and tested;
- system state does not create a new incompatibility;
- the action class is still FSA-fallback eligible;
- no Owner HOLD/REJECT/REQUEST_CHANGES has appeared;
- no higher authority/gating condition blocks adoption.

Only after this final revalidation may FSA issue a delegated fallback approval for an eligible action class.

---

## 19. FSA Approval Audit Record

Every fallback approval must remain distinguishable from explicit Owner approval.

The record should eventually include at minimum:

- `APPROVAL_AUTHORITY = FSA_DELEGATED_24H_FALLBACK`;
- exact pre-delegation policy / Delegation Matrix reference;
- exact candidate identity/version/hash;
- Owner Review Ready timestamp;
- fallback eligibility timestamp;
- Owner response status;
- action class;
- eligibility result;
- FSA final revalidation result;
- evidence-completeness result;
- open-findings result;
- authority/scope-expansion result;
- rollback-plan identity;
- rollback-test evidence;
- final approval result;
- post-adoption observation requirements where applicable.

The audit trail must make it impossible to confuse FSA delegated approval with direct Owner approval.

---

## 20. Three Approval Classes Under Discussion

The latest discussion distinguishes three broad classes.

### 20.1 FSA 24H ELIGIBLE

Potential examples, only when explicitly pre-delegated and all conditions pass:

- bounded strategy candidate inside an already authorized market/application scope;
- bounded algorithm improvement inside an existing component without authority expansion;
- Provider selection/routing logic improvement inside existing FSAPMA authority;
- simulation-model improvement;
- bounded performance optimization;
- bug fix inside existing ownership;
- Risk model/calculation improvement that does not change Risk policy, authority or permitted capital exposure.

These examples are design candidates, not a final Delegation Matrix.

### 20.2 OWNER ONLY / NON-DELEGABLE

Potential Owner-only classes include:

- Vision change;
- Constitution change;
- Owner authority change;
- FSA authority/jurisdiction change;
- MSA/LSA/CSA authority expansion;
- Kill Switch / containment weakening or authority redesign;
- self-development approval-rule change;
- 24-hour rule or Delegation Matrix change;
- Internet Research scope expansion at an authority/policy level;
- Foundation responsibility change;
- material Application ownership/architecture change;
- creation of a new Application when it changes governed topology;
- capital-protection policy change;
- Risk authority / permitted-loss / exposure / leverage authority change;
- unauthorized Tiny-Live / Live authority creation;
- other high-consequence scope/authority expansion defined by governance.

No timer can manufacture FSA authority over an Owner-only action.

### 20.3 NORMALLY FSA-ELIGIBLE BUT BLOCKED BY IMPERFECT PACKAGE

An otherwise eligible action becomes non-approvable by FSA when any required condition is not exact.

Examples include:

- missing test;
- skipped test;
- open finding;
- hash mismatch;
- unknown dependency;
- unresolved ambiguity;
- unexpected behavior;
- evidence conflict;
- changed candidate;
- incomplete provenance;
- unverified code/model;
- unproven rollback;
- invalidated FSTSimA evidence;
- invalidated MSA recommendation;
- security uncertainty.

Such a candidate must be returned for remediation or explicit Owner decision according to the applicable rule.

---

## 21. Delegation Matrix

A future Owner-controlled **FSA Fallback Delegation Matrix** should classify action types and exact conditions.

FSA SHALL NOT modify this matrix on its own authority.

The matrix should eventually define for each action class:

- class identity;
- Owner-only / FSA-eligible / conditional status;
- consequence level;
- maximum Evolution Distance;
- required FSTSimA evidence;
- required MSA evaluation;
- required independent validation;
- required Red-Team depth;
- required FSA system review;
- rollback requirements;
- observation/stability-window requirements;
- disqualifying conditions;
- whether the 24-hour path is permitted;
- any market/environment limits;
- any capital-exposure prohibition;
- any security/authority tripwires.

`FSA_CANNOT_EXPAND_ITS_OWN_DELEGATION`

---

## 22. Risk Clarification — Authority vs Models

The earlier discussion used the phrase `Risk Authority` specifically to mean Risk permissions and authority boundaries, not an assumption that Risk necessarily owns `strategies` equivalent to Trading strategies.

Examples of Risk authority/policy questions include:

- who may deny a trade for Risk reasons;
- who may reduce position size;
- who may impose exposure limits;
- who may define maximum permitted loss;
- who may modify leverage authority;
- who may change capital-protection floors;
- who may require additional protection;
- who may enter or exit a governed Risk restriction state.

The Owner has explicitly stated that whether Falcon Risk should contain `Risk Strategies`, fixed rules, adaptive models, model orchestration or another structure requires separate research and architecture study.

This file therefore SHALL NOT establish a final `Risk Strategy` architecture.

---

## 23. Provisional Risk Separation for the Approval Model

Until the dedicated Risk study is completed, the discussion preserves a useful provisional separation:

### Hard / Governing Risk Policy and Authority

Examples may include:

- absolute exposure boundaries;
- capital-protection rules;
- maximum authorized loss boundaries;
- leverage authority;
- Owner-approved hard restrictions;
- Guardian-imposed restrictions where authoritative;
- other non-negotiable risk constraints.

These are not assumed to be self-modifiable by a Risk model.

### Adaptive Risk Models / Measurements

Potential examples include:

- volatility-aware risk measurement;
- liquidity-aware risk measurement;
- correlation-aware risk measurement;
- drawdown-aware models;
- event-aware models;
- tail-risk models;
- execution-risk models;
- dynamic sizing models where permitted by policy.

Such models may potentially be self-developed inside bounded authority, but model improvement does not authorize changing the hard Risk policy or capital-exposure authority.

Provisional invariant:

`IMPROVE_RISK_MEASUREMENT != CHANGE_ALLOWED_RISK`

A future study must determine whether a Risk Model Orchestrator or another combination mechanism is appropriate.

---

## 24. Consolidated Self-Development Approval Chain

The latest Owner-directed chain is:

`CSA / LSA`

-> specialized research only within governed scope

-> explain reason for development

-> explain expected benefit and how it will be measured

-> develop/test/remediate

-> produce full evidence

-> `FSTSimA`

-> independent experimental testing

-> quality/fidelity/stress/failure assessment

-> exact product evidence

-> `MSA`

-> Application-level evaluation

-> specialization/ownership check

-> benefit/impact/integration assessment

-> final Application recommendation

-> `Independent Validation / Red-Team`

-> challenge product, developer claims, FSTSimA evidence and MSA assessment

-> evaluate hidden effects, authority, security, architecture, evidence and rollback

-> `FSA`

-> verify Vision, Constitution, governance, authority, architecture, contracts, isolation, security and Foundation/system compatibility

-> verify legitimate MSA identity and attestation

-> perform system-level Red-Team/challenge of the MSA recommendation

-> do NOT replace Trading/business judgment

-> `Complete Evidence Package`

-> `Owner Review Page`

-> `OWNER_REVIEW_READY`

-> start exact 24-hour review window

-> `Owner` remains primary approval authority

-> if Owner acts: Owner decision controls

-> if Owner does not act: evaluate whether action class is pre-delegated to FSA

-> if not eligible: wait for Owner

-> if eligible: FSA final revalidation

-> exact unchanged candidate + all conditions pass + no disqualifying finding + tested rollback

-> `FSA_DELEGATED_24H_FALLBACK_APPROVAL`

This is the current discussion model. It is not yet accepted current law or runtime authority.

---

## 25. Core Invariants Added by This Continuation

- `OWNER_SILENCE != OWNER_APPROVAL`
- `TIMER_EXPIRY != AUTHORITY`
- `PREDELEGATION_CREATES_FALLBACK_AUTHORITY`
- `FSA_CANNOT_DEFINE_ITS_OWN_FALLBACK_SCOPE`
- `FSA_FALLBACK_REQUIRES_EXACT_COMPLIANCE`
- `FSA_FALLBACK_REQUIRES_TESTED_ROLLBACK`
- `FSA_SYSTEM_REVIEW != APPLICATION_BUSINESS_REVIEW`
- `MSA_RECOMMENDATION != PRODUCTION_APPROVAL`
- `FSTSIMA_PASS != PROMOTION_AUTHORITY`
- `RED_TEAM_PASS != OWNER_APPROVAL`
- `REVIEW_READY_VERSION != LATER_MODIFIED_VERSION`
- `IMPROVE_RISK_MEASUREMENT != CHANGE_ALLOWED_RISK`
- `OWNER_ONLY_ACTIONS_REMAIN_OWNER_ONLY_AFTER_ANY_TIMEOUT`

---

## 26. Open Questions for the Next Discussion

The following matters remain deliberately open:

1. exact FSA Fallback Delegation Matrix;
2. exact list of Owner-only/non-delegable action classes;
3. whether the 24-hour duration is globally fixed or can be longer by consequence class while never being shortened below an Owner-approved minimum;
4. exact Owner Review Page schema;
5. exact definition of `HOLD`, expiry and release semantics;
6. exact independent validation / Red-Team runtime actor and placement;
7. exact required rollback depth by change class;
8. post-FSA-approval observation/stability windows;
9. automatic rollback triggers after FSA-approved deployment;
10. exact Risk architecture: Risk Policy, Risk Models, Risk Controls, possible orchestration and CSA eligibility;
11. exact MSA/FSA authority reconciliation with current APP-001 / CON-023 / ADR-I015;
12. exact Stage 13 Foundation requirements for Owner/FSA fallback approval, rollback and audit;
13. exact Stage 12 implications of the revised LSA/CSA-only-by-default specialized research direction;
14. exact cross-Part mapping into P1-F through P1-L and any cross-cutting Part 1 governance artifact.

---

## 27. Non-Authority and Preservation Rule

This continuation SHALL NOT:

- rewrite Part 0 history;
- claim Owner acceptance;
- modify current governing Vision/Constitution/APP-001/CON-023/ADR-I012/ADR-I015;
- activate self-development runtime;
- activate Internet research;
- activate FSA fallback approval;
- start any actual 24-hour timer;
- grant production adoption;
- grant Paper/Shadow/Tiny-Live/Live authority;
- define final Risk architecture;
- create a new independent-validation Application by implication;
- modify Foundation files or Foundation planning;
- convert a discussion direction into authority.

It preserves the Owner discussion so that the next FSATS page can continue from the exact current intent without reconstructing it from memory.
