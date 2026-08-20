# AUT-001 — Authority Engine

**Identifier:** AUT-001  
**Version:** 1.1  
**Status:** Approved  
**Effective Date:** 2026-07-25  
**Approval Record:** GOV-012  
**Owner:** Falcon Governance Authority  
**Governing Authority:** Constitution Articles 3–5, 16, 19, 24, 26–27, 30, 39–44  
**Affected Domains:** All  
**Supersedes:** AUT-001 v1.0

## 1. Purpose

The Authority Engine determines whether a governed actor is permitted to perform a governed action within an established jurisdiction under the currently effective authority, delegation chain, constraints, protective restrictions, and operating condition.

It is the operational interpreter of approved authority. It is not the source of that authority.

The Authority Engine verifies jurisdiction and authority. It does not establish jurisdiction, appoint Authority Holders, or legitimize an invalid Authority Chain.

## 2. Scope

AUT-001 governs:

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

## 3. Non-Scope

The Authority Engine SHALL NOT:

- create constitutional or financial policy;
- approve its own permissions;
- execute the requested action;
- infer permission from technical capability;
- convert recommendation into authorization;
- resolve conflicts with the Vision or Constitution in favor of lower authority; or
- grant permanent authority through repeated use.
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

## 4. Decision Model

An authority decision shall resolve:

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

## 5. Normative Requirements

- **AUT-001-REQ-001:** Every material authority decision SHALL be attributable to an authenticated actor and an authoritative policy baseline.
- **AUT-001-REQ-002:** The Authority Engine SHALL deny authority that has no traceable source.
- **AUT-001-REQ-003:** The Authority Engine SHALL apply the highest controlling authority when applicable rules conflict.
- **AUT-001-REQ-004:** A lower authority SHALL NOT waive or weaken a higher constraint.
- **AUT-001-REQ-005:** Permission SHALL be limited to the narrowest scope, duration, and capability sufficient for the declared purpose.
- **AUT-001-REQ-006:** Authority SHALL expire or become invalid when its purpose ends, its conditions fail, its source is withdrawn, or its time limit is reached.
- **AUT-001-REQ-007:** Denial SHALL be the default when actor identity, policy integrity, applicable authority, or material conditions cannot be established.
- **AUT-001-REQ-008:** Emergency authority SHALL remain bounded by non-waivable constitutional and protective constraints.
- **AUT-001-REQ-009:** The Authority Engine SHALL support immediate revocation of material delegated authority.
- **AUT-001-REQ-010:** A recommendation, prediction, lifecycle state, or successful prior action SHALL NOT be treated as permission.
- **AUT-001-REQ-011:** Every material authorization result SHALL record decision, basis, policy version, conditions, and reason.
- **AUT-001-REQ-012:** Authorization evidence SHALL be immutable after the decision is issued.
- **AUT-001-REQ-013:** The Authority Engine SHALL detect and reject attempts to authorize itself or modify its governing authority through ordinary requests.
- **AUT-001-REQ-014:** Protective restrictions issued lawfully by Guardian SHALL constrain authorization until revoked by legitimate authority.
- **AUT-001-REQ-015:** Authority evaluation SHALL be deterministic for the same trusted inputs and effective policy baseline.
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

## 6. Failure and Degraded Behavior

Failure to establish trustworthy jurisdiction, Authority Chain, assignment, delegation, policy, identity, conditions, independence, or protective state SHALL result in denial or an explicitly narrower safe restriction.

Cached authority MAY be used only where an Approved policy explicitly defines eligible authority classes, jurisdiction, scope, consequence ceiling, maximum age, clock-quality requirement, revocation behavior, protective-state behavior, reconciliation, evidence, and acceptable residual risk.

Cached authority SHALL NOT survive known revocation, exceed source expiry, bypass an active Guardian restriction, create jurisdiction, permit authority expansion, authorize constitutional change, or convert an indeterminate result into unrestricted permission.

## 7. Acceptance Evidence

Approval requires evidence that:

- conflicting lower policies cannot override higher constraints;
- authority without valid jurisdiction is denied;
- the full Authority Chain is verified and a broken link denies dependent authority;
- delegation cannot create jurisdiction or exceed its source;
- unauthorized redelegation is denied;
- upstream constraints survive delegation;
- action, resource, purpose, environment, and consequence remain independently bounded;
- cross-jurisdiction and collegiate authority cannot be inferred;
- expired, suspended, revoked, terminated, and non-operative authority is rejected;
- Guardian and other protective restrictions constrain conflicting permission;
- emergency authority cannot persist or silently renew;
- missing provenance or material uncertainty results in denial or safe restriction;
- the Engine cannot establish or grant itself authority;
- restoration requires new evidence and independent confirmation;
- identical trusted inputs produce identical results; and
- every material result is reconstructable from immutable SEC-002-conforming evidence.

## 8. ADR Candidates

- Policy representation and evaluation model;
- distribution and consistency of authority state;
- revocation propagation mechanism; and
- isolation boundary for the Authority Engine.

## 9. Unresolved Matters

- Formal catalog of non-waivable constraints.
- Maximum acceptable authorization latency by consequence class.
- Canonical jurisdiction, decision-class, and consequence-class catalogs.
- Whether CON-002 requires a separately Approved v1.1 amendment for the expanded decision record.
