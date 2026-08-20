# AMD-002 — Authority Jurisdiction Verification Amendment

**Version:** 1.0  
**Status:** Approved  
**Effective Date:** 2026-07-25  
**Approval Record:** GOV-012  
**Owner:** Falcon Governance Authority  
**Governing Authority:** Falcon Vision; Falcon Constitution; GOV-AUT-001 v1.0; SEC-002 v1.0; AUT-001 v1.0; ADR-I007  
**Target Version:** AUT-001 v1.1  
**Supersedes:** None  
**Superseded By:** None  
**Implementation Authority:** Not Granted

## 1. Purpose

AMD-002 defines the exact changes required for AUT-001 v1.1 to verify jurisdiction and the complete Authority Chain before granting permission.

The amendment converts the governance rules of GOV-AUT-001 into binding Authority Engine obligations without creating jurisdiction, assigning authority, or selecting an implementation.

## 2. Amendment Boundary

This amendment:

- adds jurisdiction to the Authority Engine decision model;
- requires verification of assignment, delegation, and redelegation;
- adds consequence, environment, independence, and protective-state checks;
- formalizes authority lifecycle handling;
- expands default-deny conditions;
- expands immutable decision evidence; and
- adds conformance evidence.

It does not:

- create a jurisdiction;
- appoint an Authority Holder;
- grant a permission;
- authorize financial activity;
- authorize implementation;
- define internal architecture;
- select storage, policy, or evaluation technology; or
- permit the Authority Engine to interpret itself as a source of authority.

## 3. Compatibility Decision

AUT-001 v1.1 strengthens AUT-001 v1.0.

Any v1.0 authority decision that cannot establish its jurisdiction, complete Authority Chain, delegation validity, consequence boundary, and applicable restrictions SHALL NOT be accepted as equivalent to a v1.1 decision.

Migration SHALL fail closed. Missing jurisdiction metadata SHALL NOT be inferred from actor identity, resource ownership, technical access, previous decisions, or successful operation.

AUT-001 v1.0 SHALL remain preserved as historical authority for its effective period. AUT-001 v1.1 SHALL supersede it prospectively only after explicit Approval and activation.

## 4. Purpose Amendment

Replace the first paragraph of AUT-001 section 1 with:

> The Authority Engine determines whether a governed actor is permitted to perform a governed action within an established jurisdiction under the currently effective authority, delegation chain, constraints, protective restrictions, and operating condition.

Retain:

> It is the operational interpreter of approved authority. It is not the source of that authority.

Add:

> The Authority Engine verifies jurisdiction and authority. It does not establish jurisdiction, appoint Authority Holders, or legitimize an invalid Authority Chain.

## 5. Scope Amendment

Expand AUT-001 section 2 to govern:

- actor and authority identity;
- jurisdiction identity and validity;
- Authority Instrument identity and state;
- complete Authority Chain;
- assignment, delegation, and redelegation validity;
- permission evaluation;
- action, resource, purpose, environment, consequence, time, and condition limits;
- separation-of-duty and independence constraints;
- prohibitions and protective restrictions;
- denial, restriction, suspension, revocation, expiry, and termination;
- conflict resolution between subordinate policies;
- authorization evidence; and
- operation under incomplete, conflicted, stale, uncertain, or untrustworthy authority data.

## 6. Non-Scope Amendment

Add that the Authority Engine SHALL NOT:

- establish, widen, merge, or reinterpret jurisdiction;
- appoint or recognize an Authority Holder without a valid Authority Instrument;
- create authority through delegation;
- authorize an action outside the jurisdiction of the source authority;
- infer authority from identity, role name, access, possession, expertise, urgency, or previous success;
- combine insufficient authorities into sufficient authority without an Approved collegiate instrument;
- treat a signature, record, or successful integrity check as proof of valid authority by itself;
- restore its own unrestricted authority after material compromise;
- resolve a material challenge to its own authority conclusively; or
- allow emergency authority to create enduring authority.

## 7. Decision Model Amendment

AUT-001 v1.1 SHALL resolve:

- **Request ID:** which exact authorization request is evaluated;
- **Actor:** who or what requests permission;
- **Action:** the governed act requested;
- **Resource:** the subject affected;
- **Purpose:** the legitimate objective served;
- **Environment:** where the action would occur;
- **Consequence:** the maximum material consequence of the requested action;
- **Jurisdiction:** the established domain within which authority may exist;
- **Authority:** the Authority Instrument permitting the decision class;
- **Authority Chain:** the trace to the Jurisdictional Source;
- **Delegation:** every applicable delegation and redelegation;
- **Scope:** the permitted boundary;
- **Conditions:** facts that must be true;
- **Constraints:** limits that remain binding;
- **Prohibitions:** rules that deny the action;
- **Independence:** required separation of authority;
- **Protective State:** Guardian and other controlling restrictions;
- **Duration:** when authority begins and ends;
- **Trust State:** validity of material identity, policy, evidence, and context;
- **Provenance:** the higher authority from which permission derives; and
- **Evidence:** the immutable basis and result of evaluation.

## 8. Required Evaluation Sequence

AUT-001 v1.1 SHALL evaluate a material request in the following governing order:

```text
Request and Actor Identity
    ↓
Applicable Higher Authority
    ↓
Jurisdiction
    ↓
Authority Instrument
    ↓
Authority Chain
    ↓
Delegation and Redelegation
    ↓
Action, Resource, Purpose, and Environment Scope
    ↓
Consequence Ceiling
    ↓
Conditions, Constraints, and Prohibitions
    ↓
Separation of Duty and Independence
    ↓
Protective Restrictions
    ↓
Trust and Time Validity
    ↓
Decision
    ↓
Immutable Evidence
```

A denial at a controlling stage SHALL NOT be converted to permission by a later stage.

## 9. Normative Requirements

Retain AUT-001-REQ-001 through AUT-001-REQ-015.

Add:

- **AUT-001-REQ-016:** Every material authority decision SHALL identify and verify the applicable Jurisdiction ID.
- **AUT-001-REQ-017:** The Authority Engine SHALL deny authority when jurisdiction is absent, invalid, expired, dissolved, conflicted, materially uncertain, or outside the requested action.
- **AUT-001-REQ-018:** Every authority decision SHALL identify the Authority Instrument and verify that its source is competent within the applicable jurisdiction.
- **AUT-001-REQ-019:** The Authority Engine SHALL verify the complete Authority Chain to its Jurisdictional Source.
- **AUT-001-REQ-020:** A missing, invalid, expired, suspended, revoked, terminated, conflicted, or unverifiable Authority Chain link SHALL deny affected authority.
- **AUT-001-REQ-021:** Delegation SHALL NOT create, widen, transfer, merge, or reinterpret jurisdiction.
- **AUT-001-REQ-022:** A delegate SHALL NOT receive or exercise authority greater than the delegator can lawfully exercise.
- **AUT-001-REQ-023:** Redelegation SHALL be denied unless every governing upstream instrument explicitly permits it.
- **AUT-001-REQ-024:** Every delegation layer SHALL preserve all applicable upstream purpose, scope, consequence, environment, duration, condition, prohibition, independence, evidence, suspension, and revocation constraints.
- **AUT-001-REQ-025:** The Authority Engine SHALL verify that the requested action, resource, purpose, environment, and consequence remain within the effective authority.
- **AUT-001-REQ-026:** Permission in one authority dimension SHALL NOT imply permission in another.
- **AUT-001-REQ-027:** Required separation-of-duty, independence, quorum, or collegiate-authority conditions SHALL be satisfied before permission is granted.
- **AUT-001-REQ-028:** Multiple insufficient authorities SHALL NOT be aggregated into sufficient authority unless an Approved collegiate or joint Authority Instrument explicitly authorizes the combination.
- **AUT-001-REQ-029:** Cross-jurisdiction action SHALL require valid authority competent across every materially affected jurisdiction.
- **AUT-001-REQ-030:** Active Guardian restrictions and other lawful protective restrictions SHALL constrain conflicting subordinate permission.
- **AUT-001-REQ-031:** Emergency authority SHALL be accepted only for its declared containment purpose, jurisdiction, consequence, conditions, and duration.
- **AUT-001-REQ-032:** Emergency authority SHALL NOT create enduring jurisdiction, permanent authority, unrelated permission, or silent renewal.
- **AUT-001-REQ-033:** Authority suspension, revocation, expiry, termination, restriction, or loss of trust SHALL propagate to dependent authority according to the governing instrument.
- **AUT-001-REQ-034:** Only `ACTIVE` authority or the explicitly preserved subset of `RESTRICTED` authority MAY grant permission.
- **AUT-001-REQ-035:** `DRAFT`, `PENDING_ACCEPTANCE`, `SUSPENDED`, `EXPIRED`, `REVOKED`, `TERMINATED`, and `ARCHIVED` authority SHALL NOT grant permission.
- **AUT-001-REQ-036:** Technical ability, access, possession, role name, expertise, urgency, recommendation, prior permission, repeated behavior, or successful outcome SHALL NOT establish authority.
- **AUT-001-REQ-037:** Material conflict of interest SHALL be evaluated according to governing policy before affected authority is exercised.
- **AUT-001-REQ-038:** The Authority Engine SHALL NOT establish jurisdiction, appoint Authority Holders, create Authority Instruments, or approve its own authority.
- **AUT-001-REQ-039:** Restoration of material authority SHALL require a new attributable decision supported by restoration evidence and the required independent confirmation.
- **AUT-001-REQ-040:** The authority whose conduct or trust is under material review SHALL NOT be the sole authority that restores itself.
- **AUT-001-REQ-041:** A material challenge requiring suspension under governing policy SHALL restrict affected authority until resolved by a competent independent authority.
- **AUT-001-REQ-042:** Every decision SHALL preserve the exact jurisdiction, Authority Instrument, Authority Chain, policy, context, conditions, restrictions, and evidence used.
- **AUT-001-REQ-043:** Authority evidence SHALL conform to SEC-002 and remain attributable, integrity-verifiable, immutable after issuance, and independently challengeable.
- **AUT-001-REQ-044:** Authority uncertainty SHALL reduce authority and SHALL NOT be resolved optimistically.
- **AUT-001-REQ-045:** No delegated authority SHALL acquire jurisdiction beyond that explicitly established by the governing model.

## 10. Authority Evaluation Input

The governed input for every material evaluation SHALL include:

- Request ID;
- Actor ID and authenticated identity evidence;
- Action ID;
- Resource ID;
- Purpose ID;
- Environment ID;
- requested consequence class;
- Jurisdiction ID and version;
- Authority ID and version;
- Authority Chain references;
- delegation and redelegation references;
- policy baseline ID and version;
- applicable constraints and prohibitions;
- separation-of-duty state;
- protective-restriction state;
- time observation and clock quality;
- evaluation context;
- trust-state references; and
- correlation and causation identity.

Unknown required fields SHALL NOT receive permissive defaults.

## 11. Authority Decision Output

Every material result SHALL produce an immutable Authority Decision containing:

- Decision ID;
- Request ID;
- decision outcome;
- denied, restricted, or permitted action;
- effective scope;
- effective consequence ceiling;
- effective conditions;
- effective restrictions;
- Jurisdiction ID;
- Authority ID;
- complete Authority Chain reference;
- policy baseline;
- Evaluation Context;
- evaluator identity;
- decision time;
- expiry;
- reason codes;
- evidence references;
- challenge path; and
- integrity identity.

The canonical Contract remains governed by CON-002 and any separately Approved version amendment.

AMD-002 does not silently modify CON-002.

## 12. Decision Outcomes

AUT-001 v1.1 SHALL distinguish:

- `PERMIT`;
- `PERMIT_RESTRICTED`;
- `DENY`;
- `INDETERMINATE`; and
- `ERROR`.

Only `PERMIT` and `PERMIT_RESTRICTED` grant operative permission.

`PERMIT_RESTRICTED` SHALL declare the exact preserved subset and constraints.

`INDETERMINATE` and `ERROR` SHALL deny the requested action unless an Approved higher-authority policy expressly defines a narrower safe alternative.

## 13. Failure and Degraded Behavior Amendment

Replace AUT-001 section 6 with:

> Failure to establish trustworthy jurisdiction, Authority Chain, assignment, delegation, policy, identity, conditions, independence, or protective state SHALL result in denial or an explicitly narrower safe restriction.

Cached authority MAY be used only where an Approved policy explicitly defines:

- eligible authority classes;
- jurisdiction;
- scope;
- consequence ceiling;
- maximum age;
- clock-quality requirement;
- revocation behavior;
- protective-state behavior;
- reconciliation;
- evidence; and
- acceptable residual risk.

Cached authority SHALL NOT:

- survive known revocation;
- exceed source expiry;
- bypass an active Guardian restriction;
- create jurisdiction;
- permit authority expansion;
- authorize constitutional change; or
- convert an indeterminate result into unrestricted permission.

## 14. Acceptance Evidence Amendment

AUT-001 v1.1 Approval requires evidence that:

- authority without jurisdiction is denied;
- invalid or dissolved jurisdiction is denied;
- the full Authority Chain is verified;
- a broken chain link denies dependent authority;
- delegation cannot create or widen jurisdiction;
- a delegate cannot exceed the delegator;
- unauthorized redelegation is denied;
- all upstream constraints survive delegation;
- action, resource, purpose, environment, and consequence are independently bounded;
- permission in one dimension does not imply another;
- cross-jurisdiction action requires competent combined authority;
- insufficient authorities cannot be combined informally;
- separation-of-duty requirements cannot be self-waived;
- active Guardian restriction constrains conflicting permission;
- emergency authority cannot persist or silently renew;
- suspension, revocation, expiry, termination, and restriction propagate correctly;
- non-operative authority states grant no permission;
- technical access and prior success grant no authority;
- conflict-of-interest policy is enforced;
- the Authority Engine cannot establish or approve its own authority;
- restoration requires new evidence and independent confirmation;
- material challenges apply required restrictions;
- uncertainty fails closed;
- identical trusted inputs and effective policy produce identical decisions; and
- every decision is reconstructable from immutable SEC-002-conforming evidence.

## 15. Cross-Document Invariants

1. No jurisdiction, no authority.
2. No authority, no permission.
3. No verified Authority Chain, no exercise.
4. Delegation does not create jurisdiction.
5. Delegation cannot exceed its source.
6. Capability is not permission.
7. Authentication is not authorization.
8. Integrity is not legitimacy.
9. Recommendation is not approval.
10. Emergency authority is containment authority, not enduring authority.
11. Guardian restriction cannot become general policy authority.
12. Restoration is a new governed decision, not automatic recovery of prior power.
13. Uncertainty reduces authority.
14. The Authority Engine interprets authority; it does not create it.
15. The Foundation remains non-financial.

## 16. Review and Activation

Before Approval, review SHALL confirm:

- exact compatibility with GOV-AUT-001 and SEC-002;
- no new jurisdiction or appointment;
- no implied financial authority;
- no collision with Guardian's protective mandate;
- no ambiguity between authority state and decision outcome;
- complete default-deny behavior;
- deterministic evaluation for identical trusted inputs;
- Contract impact is explicit;
- complete traceability and negative verification coverage; and
- preservation of AUT-001 v1.0.

Upon Approval:

1. preserve AUT-001 v1.0 as immutable history;
2. create canonical AUT-001 v1.1 with Approval metadata;
3. update the Specification Registry;
4. update ROADMAP-001 and FRS-001;
5. record prospective supersession;
6. evaluate whether CON-002 requires a separate version amendment;
7. add requirements to TRC-001 and PIPE-001 when authored; and
8. keep implementation unauthorized until all remaining gates and explicit Project Owner authorization are complete.

## 17. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner and current Constitutional Authority | Approved | GOV-012 | 2026-07-25 |

This Approval authorizes activation of AUT-001 v1.1 and prospective supersession of AUT-001 v1.0.

It does not change a Contract, establish jurisdiction, grant authority, or authorize implementation.
