# P0-A - Governance, Authority and Evidence

**Status:** `OWNER_DIRECTED_INTEGRATED_REWRITE_CANDIDATE / NOT_OWNER_ACCEPTED / NOT_CLOSED`
**Implementation Authority:** `NOT_GRANTED`
**Runtime Authority:** `NOT_GRANTED`

## 1. Purpose

P0-A is the governance/evidence kernel for the integrated FSATS Part 0. It defines what is authoritative, what is evidence, what is historical provenance, what is unresolved and what must never be inferred. Its purpose is to stop later design or implementation from converting memory, technical capability, stale documents, partial PASS results, timestamps, repeated outcomes or historical precedent into Falcon authority.

## 2. Responsibility

P0-A governs design-time interpretation across P0-A through P0-L. It does not own Foundation governance, Project Owner authority, Application business logic, implementation approval, runtime authorization or FCR ownership outside the Application workstream.

## 3. Canonical authority order

```text
FALCON_VISION
> FALCON_CONSTITUTION
> VALID_OWNER / COMPETENT_GOVERNANCE_DECISIONS
> CURRENT_ACCEPTED_FALCON_ARCHITECTURE / FOUNDATION_AUTHORITY
> APP-001 / CON-023 / ADR-I012 / ADR-I015 / OTHER_CURRENT_AUTHORITIES
> CURRENT_OWNER_ACCEPTED_FSATS_DESIGN
> CURRENT_ACCEPTED_PLANS / POLICIES / PROCEDURES
> IMPLEMENTATION
> RECORDED_OUTCOMES
> HISTORICAL / REFERENCE MATERIAL
```

A lower source cannot silently amend a higher source. A later valid Owner clarification, correction, reopen, amendment or closure controls an earlier Owner decision only within the exact scope actually changed.

## 4. Source classes

Every material source is classified as one of:

- `GOVERNING_CURRENT`;
- `OWNER_CURRENT`;
- `CURRENT_ACCEPTED_DESIGN`;
- `CURRENT_FOUNDATION_AUTHORITY`;
- `CURRENT_FCR_STATE`;
- `CURRENT_REVIEW_EVIDENCE`;
- `CURRENT_IMPLEMENTATION_EVIDENCE`;
- `HISTORICAL_PROVENANCE`;
- `REFERENCE_ONLY`;
- `EXTERNAL_CHALLENGE_EVIDENCE`;
- `UNRESOLVED`.

Historical/reference/external challenge evidence may inform or challenge current design but cannot override current governing authority.

## 5. Evidence classes

Material claims distinguish at least:

- authoritative fact;
- observed fact;
- measured result;
- derived result;
- model estimate;
- assumption;
- interpretation;
- hypothesis;
- proposed design;
- review evidence;
- test evidence;
- implementation evidence;
- runtime outcome evidence;
- unresolved question.

A claim cannot be upgraded because it is convenient, repeated or successful.

## 6. Owner decision ordering

For every material Owner-controlled subject:

```text
EARLIER_OWNER_STATE
-> LATER_OWNER_CHANGE / CLARIFICATION / REOPEN / AMENDMENT
-> APPLY_EXACT_CHANGE
-> FRESH_ARCHITECTURE / CONSISTENCY_REVIEW_IF_SEMANTICS_CHANGED
-> FRESH_RED_TEAM_IF_SEMANTICS_CHANGED
-> APPLY_FINDINGS
-> REPEAT_REVIEW_IF_REQUIRED
-> FINAL_OWNER_STATE_ONLY_IF_EXPLICITLY_GRANTED
```

`APPROVED WITH CHANGE X` is not final acceptance of changed bytes until the requested change is applied and freshly reviewed.

## 7. State separation

```text
DRAFT
!= REVIEW_CANDIDATE
!= ARCHITECTURE_PASS
!= RED_TEAM_PASS
!= READY_FOR_OWNER_REVIEW
!= OWNER_ACCEPTED
!= OWNER_ACCEPTED_AND_CLOSED
!= IMPLEMENTATION_AUTHORIZED
!= IMPLEMENTED
!= VERIFIED
!= RUNTIME_AUTHORIZED
```

No state implies the next. Git commit success proves only that Git accepted the bytes.

## 8. Decision Proof Envelope

The Decision Proof Envelope is a reconstructability abstraction, not a shared mutable authority store and not an authority source.

For a material decision/action it should reference as applicable:

- subject identity;
- actor identity;
- exact Application identity;
- `BrokerId + BrokerAccountId` and Environment where Trading business identity is material;
- provider/account/service-role/API-instance identity where data sourcing is material;
- authority source and exact scope;
- policy/version;
- evidence identities;
- data freshness/provenance;
- Risk decision/version;
- capital reservation;
- Guardian/control epoch;
- Owner/governance control state;
- APP-RSC coordination epoch/resource evidence where material;
- Foundation resource outcome identity where material;
- Intended Use/validation evidence;
- correlation/causation identities;
- exact action identity;
- observed outcome/reconciliation identity;
- correction/supersession lineage.

```text
DPE_PRESENT != AUTHORITY_GRANTED
DPE_COMPLETE != DECISION_CORRECT
```

## 9. Reference synthesis gate

Before a material Part 0 semantic change is ready for review, reconcile:

- current Falcon Vision;
- current Constitution;
- current Owner decisions;
- current accepted FSATS semantics;
- current Foundation authority;
- current FCR state;
- latest valid review/evidence for affected scope;
- current executable source/tests where implementation evidence matters;
- useful historical design/reference evidence;
- applicable external challenge evidence.

A source is not omitted merely because it is inconvenient.

## 10. Current-state freshness

The following are time-sensitive and must be refreshed before final review and later before dependent implementation claims:

- branch HEAD/current bytes;
- Owner acceptance/authorization state;
- Foundation stage/WP/capability state;
- open FCR headers and latest material comments;
- implementation/test status;
- runtime authorization/deployment/connectivity state.

Conversation memory is never current repository truth.

## 11. FCR protocol

Repository Issue #1 is the FCR protocol source. Current permitted `Waiting On` values are:

```text
FOUNDATION
APPLICATION
WEB
NONE
```

`Waiting On: OWNER` is prohibited. If the Application needs Owner clarification, the FCR remains owned by `APPLICATION` while that clarification is obtained.

Before substantive FSATS work, refresh live FCRs. An FCR with `Waiting On: APPLICATION` is handled first when it is material to the requested work.

```text
FCR_SUBMITTED != FOUNDATION_COMMITMENT_TO_IMPLEMENT
FCR_ACCEPTED_FOR_PLANNING != IMPLEMENTED
FOUNDATION_IMPLEMENTED != APPLICATION_VERIFIED
APPLICATION_VERIFIED != RUNTIME_AUTHORIZED
```

An FCR is coordination evidence, never implementation authority.

## 12. Historical-record rule

Historical bytes are immutable provenance. Correction normally uses:

```text
PRESERVE_HISTORICAL_RECORD
+ CREATE_CONTROLLING_CURRENT_REWRITE / CORRECTION / SUPERSESSION
```

The integrated rewrite is permitted to state current truth directly because the predecessor Part 0 tree is preserved under:

`applications/docs/FSATS/06_ARCHIVE/PART_0_PRE_INTEGRATED_REWRITE_2026-08-15/`

The archive does not remain required implementation reading once this integrated rewrite is Owner-accepted.

## 13. External research rule

External standards, papers, vendor docs, engineering literature and regulatory material may challenge design, identify missing controls or supply methods. They do not automatically become Falcon authority or prove legal applicability. Any adopted method remains subordinate to Falcon ownership/governance.

## 14. Failure and ambiguity behavior

If a material source conflict, authority scope, identity, provenance, FCR state or evidence requirement cannot be resolved:

```text
MARK_CONFLICT
STOP_AFFECTED_AUTHORITY_CLAIM
FAIL_CLOSED_WHERE_MATERIAL
ESCALATE_TO_CORRECT_OWNER / WORKSTREAM / GOVERNANCE_CHANNEL
```

Stale evidence cannot silently reuse an older permissive value.

## 15. Split-brain and competing-current-state rule

If two sources both claim to be current and their precedence cannot be proven, neither wins by filename, timestamp, branch position or confidence. The affected semantic is `UNRESOLVED` until authority lineage proves the controlling source.

```text
NEWEST_TIMESTAMP != AUTOMATIC_AUTHORITY
FILENAME_FINAL != OWNER_ACCEPTED
DIRECTORY_CURRENT != GOVERNING_BY_ITSELF
```

## 16. Explicit non-authority

P0-A does not:

- create Owner or Foundation authority;
- grant implementation/runtime/deployment;
- activate routes/provider/broker connectivity;
- validate Trading business semantics by itself;
- close FCRs;
- convert historical success into approval;
- convert technical PASS into Owner acceptance.

## 17. Invariants

```text
CURRENT_AUTHORITY > HISTORICAL_REFERENCE
LATER_VALID_OWNER_DECISION > EARLIER_OWNER_DECISION_WITHIN_EXACT_SCOPE
TECHNICAL_PASS != OWNER_ACCEPTANCE
OWNER_ACCEPTANCE != IMPLEMENTATION_AUTHORITY
DESIGN_ACCEPTANCE != RUNTIME_AVAILABILITY
FCR_ACCEPTED_FOR_PLANNING != IMPLEMENTED
EVIDENCE != AUTHORITY
CONFIDENCE != AUTHORITY
SUCCESS != AUTHORITY
MEMORY != CURRENT_REPOSITORY_TRUTH
NEWEST_FILE != CURRENT_AUTHORITY
```

## 18. Forbidden interpretations

Invalid interpretations include:

- old document overrides later Owner correction;
- test PASS implies Owner acceptance;
- code existence implies authorization;
- FCR planning acceptance implies Foundation implementation;
- V1.3 mechanism remains mandatory because historical;
- latest commit/file is automatically accepted baseline;
- silence/non-objection equals approval;
- APP-RSC exists in code therefore its Foundation binding is runtime-authorized;
- a Web click equals Trading authorization.

## 19. Evidence and traceability obligation

Every material current P0 rule must trace to governing/problem evidence and later review/test obligations. P0-B owns detailed traceability; P0-A requires the trace to exist.

## 20. Exit gates

```text
AUTHORITY_SOURCE_AMBIGUITY = 0
OWNER_PRECEDENCE_AMBIGUITY = 0
EVIDENCE_CLASS_AMBIGUITY = 0
HISTORICAL_CURRENT_CONFLATION = 0
PASS_ACCEPTANCE_CONFLATION = 0
CURRENT_SOURCE_RETRIEVAL_METHOD = DEFINED
FCR_PROTOCOL = EXPLICIT
DPE_ROLE = DEFINED_AND_NON_AUTHORITATIVE
REFERENCE_SYNTHESIS_GATE = DEFINED
SPLIT_BRAIN_CURRENT_SOURCE_RULE = FAIL_CLOSED
```

## 21. Non-grant

Acceptance of P0-A would establish governance/evidence semantics only. It would not authorize any implementation or runtime behavior.