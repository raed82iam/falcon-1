# P0-A — Governance, Authority and Evidence

**Status:** `PROPOSED / OWNER_REVIEW_CANDIDATE / NOT_OWNER_ACCEPTED / NOT_CLOSED`  
**Scope:** `P0-A only`  
**Implementation Authority:** `NOT GRANTED`

---

## 1. Purpose

P0-A defines how FSATS determines what is authoritative, what is evidence, what is historical context, what is unresolved, and what may never be inferred.

Its purpose is to stop later design and implementation from converting memory, technical capability, stale documents, partial PASS results, or historical precedent into Falcon authority.

---

## 2. Responsibility

P0-A owns the design-time governance/evidence kernel for P0-A through P0-K.

It does not own Foundation governance, Project Owner authority, Application business logic, runtime authorization, or implementation approval.

---

## 3. Canonical Authority Order

```text
FALCON_VISION
  > FALCON_CONSTITUTION
  > VALID_OWNER / GOVERNANCE DECISIONS
  > CURRENT_ACCEPTED FALCON ARCHITECTURE / FOUNDATION AUTHORITY
  > APP-001 / CON-023 / ADR-I012 / ADR-I015 AND OTHER APPLICABLE CURRENT AUTHORITIES
  > CURRENT_ACCEPTED FSATS DESIGN
  > CURRENT ACCEPTED PLANS / POLICIES / PROCEDURES
  > IMPLEMENTATION
  > RECORDED OUTCOMES
```

A lower source cannot silently amend a higher source.

A later valid Owner clarification, reopen, amendment, correction, or closure controls an older Owner decision only within its actual scope.

---

## 4. Source Classes

Every material source SHALL be classified as one of:

- `GOVERNING_CURRENT`;
- `OWNER_CURRENT`;
- `CURRENT_ACCEPTED_DESIGN`;
- `CURRENT_FOUNDATION_AUTHORITY`;
- `CURRENT_FCR_STATE`;
- `CURRENT_REVIEW_EVIDENCE`;
- `HISTORICAL_PROVENANCE`;
- `REFERENCE_ONLY`;
- `EXTERNAL_CHALLENGE_EVIDENCE`;
- `UNRESOLVED`.

`HISTORICAL_PROVENANCE`, `REFERENCE_ONLY`, and `EXTERNAL_CHALLENGE_EVIDENCE` cannot override a current governing source.

---

## 5. Evidence Classes

Material claims SHALL distinguish at least:

- authoritative fact;
- observed fact;
- measured result;
- derived result;
- model estimate;
- assumption;
- interpretation;
- hypothesis;
- proposed design;
- test evidence;
- runtime outcome evidence;
- unresolved question.

A claim cannot be upgraded to a stronger class because it is convenient or repeatedly observed.

---

## 6. Owner Decision Ordering

For every material Owner-controlled subject, P0-A requires an ordered decision chain:

```text
EARLIER_OWNER_STATE
  -> LATER_OWNER_CHANGE / CLARIFICATION / REOPEN / AMENDMENT
  -> FRESH REVIEW IF SEMANTICS CHANGED
  -> FINAL OWNER STATE ONLY IF EXPLICITLY GRANTED
```

An Owner statement such as `approved with change X` does not automatically create final acceptance of the changed bytes. The requested semantic change must be applied and freshly reviewed before final acceptance is requested.

---

## 7. Design / Review / Acceptance / Closure Separation

```text
DRAFT
!= REVIEW_CANDIDATE
!= ARCHITECTURE_PASS
!= RED_TEAM_PASS
!= READY_FOR_OWNER_REVIEW
!= OWNER_ACCEPTED
!= OWNER_ACCEPTED_AND_CLOSED
!= IMPLEMENTATION_AUTHORIZED
!= RUNTIME_AUTHORIZED
```

No state implies the next.

Git commit success proves Git accepted bytes only.

---

## 8. Decision Proof Envelope

P0-NG uses the **Decision Proof Envelope (DPE)** as a design abstraction for reconstructability.

A DPE is not a central runtime service, not a shared mutable authority store, and not an authority source.

For a material decision/action, the reconstructable proof SHOULD reference as applicable:

- subject identity;
- actor identity;
- Application/user/account/environment identity;
- authority source;
- exact authority scope;
- policy/version;
- evidence identities;
- data freshness/provenance;
- Risk decision/version;
- capital reservation;
- Guardian/control epoch;
- user/Owner/subscription control state;
- broker/provider role and capability evidence;
- resource truth where materially relevant;
- intended-use/validation evidence;
- causation/correlation identities;
- action identity;
- observed outcome/reconciliation identity.

```text
DPE_PRESENT != AUTHORITY_GRANTED
DPE_COMPLETE != DECISION_CORRECT
```

The DPE supports reconstruction and challenge.

---

## 9. Reference Synthesis Gate

Before a material P0 design or semantic change is considered ready for review, the responsible work SHALL reconcile:

- current Vision;
- current Constitution;
- current Owner decisions;
- current approved P0 semantics;
- current Foundation authority;
- current FCR state;
- latest valid reviews for the affected scope;
- useful historical design/reference evidence;
- applicable external challenge evidence.

A source cannot be omitted merely because it is inconvenient.

---

## 10. Freshness and Current-State Rule

Current status, Foundation capability, FCR state, implementation availability, Owner state, and runtime authorization are time-sensitive evidence.

They SHALL be refreshed before final review and before any later implementation dependency claim.

Conversation memory is never a substitute for the current repository/evidence state.

---

## 11. Historical Record Rule

Historical records are immutable provenance.

Correction uses:

```text
PRESERVE_HISTORICAL_RECORD
+
CREATE_CONTROLLING_CORRECTION / AMENDMENT / SUPERSESSION
```

not silent rewriting.

A consolidated current design may state the current truth directly while retaining predecessor history separately.

---

## 12. External Research Rule

External standards, papers, vendor documentation, engineering literature, and regulatory material may:

- challenge a design;
- identify missing controls;
- supply engineering methods;
- provide evidence about common failure modes.

They do not automatically become Falcon authority or prove legal applicability.

Any adopted external method must still conform to Falcon authority and ownership.

---

## 13. Failure / Ambiguity Behavior

When a material source conflict cannot be resolved:

```text
MARK_CONFLICT
STOP_AFFECTED_AUTHORITY_CLAIM
FAIL_CLOSED_WHERE_MATERIAL
ESCALATE_TO_CORRECT_OWNER / GOVERNANCE CHANNEL
```

When evidence is stale or unavailable, do not silently reuse an older permissive value.

---

## 14. Explicit Non-Authority

P0-A SHALL NOT:

- create Owner authority;
- create Foundation authority;
- grant implementation;
- authorize a runtime route;
- validate business semantics;
- close an FCR;
- treat historical success as approval;
- treat technical PASS as Owner acceptance.

---

## 15. Invariants

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
```

---

## 16. Forbidden Interpretations

The following interpretations are invalid:

- “the old document said it, so the later Owner correction can be ignored”;
- “the test passed, so Owner acceptance is implied”;
- “the feature exists in code, so it is authorized”;
- “the FCR is accepted for planning, so Foundation has implemented it”;
- “V1.3 did it this way, so current design must preserve the mechanism”;
- “the latest commit is automatically the accepted baseline”;
- “no one objected, therefore the decision is approved”.

---

## 17. Evidence / Traceability

Each material P0-NG rule must be traceable to its governing/problem evidence and later to review/test obligations.

P0-B owns the detailed trace graph. P0-A defines the rule that the trace must exist.

---

## 18. Exit Gates

```text
AUTHORITY_SOURCE_AMBIGUITY = 0
OWNER_PRECEDENCE_AMBIGUITY = 0
EVIDENCE_CLASS_AMBIGUITY = 0
HISTORICAL_CURRENT_CONFLATION = 0
PASS_ACCEPTANCE_CONFLATION = 0
CURRENT_SOURCE_RETRIEVAL_METHOD = DEFINED
DPE_ROLE = DEFINED_AND_NON_AUTHORITATIVE
REFERENCE_SYNTHESIS_GATE = DEFINED
```

---

## 19. Next Authorized Gate

Completion or future acceptance of P0-A does not authorize P0-B implementation or any runtime behavior. It only allows later design to rely on these governance/evidence semantics within separately authorized scope.
