# Owner Communication and Approval Center — Candidate Approval Requirements

**Identifier:** OCAC-001 — Proposed  
**Status:** Approved Requirements — Realization Not Authorized  
**Approval Record:** GOV-061  
**Owner:** Project Owner Governance Interface  
**Implementation Authority:** Not Granted

## 1. Purpose

The Owner Communication and Approval Center is the private governed interface through which FSA presents a candidate proposal and the Project Owner records an explicit decision.

It communicates approval. It does not manufacture approval.

## 2. Boundary

The Center SHALL:

- authenticate the Project Owner or explicitly authorized governance identity;
- present one immutable candidate case;
- make requested scope and consequence understandable;
- record an explicit version-specific decision;
- preserve evidence and history.

The Center SHALL NOT:

- allow FSA to approve on behalf of the Owner;
- infer approval from silence, elapsed time, viewing, or prior decisions;
- alter the candidate or Evidence Set;
- widen approval conditions;
- activate or deploy merely because a decision was recorded;
- expose Application business information unnecessarily.

## 3. Mandatory Approval Package

Each package SHALL contain:

- Proposal ID and creation time;
- requesting FSA identity and authority;
- target, owner, current Approved version, and candidate version;
- current condition, weakness, evidence, observation period, and root cause;
- why Self-Repair is insufficient;
- exact modifications, removed behavior, and added behavior;
- technical impact on components, dependencies, and Applications;
- expected benefit and measured improvement;
- risks, uncertainty, compatibility, security, authority, constitutional, architectural, and specification assessments;
- Sandbox and Digital City results;
- independent validation and Evidence Set identity;
- deployment, Canary, monitoring, rollback, corrective-action, downtime, and service-impact plans;
- FSA recommendation;
- exact Owner decision and scope requested.

## 4. Identity and Security Controls

- Owner identity SHALL satisfy an Approved assurance profile.
- Candidate, package, and Evidence Set SHALL be cryptographically or equivalently integrity-bound.
- The decision SHALL bind Owner identity, candidate, Evidence Set, scope, stage, conditions, time, expiry where applicable, and signature/integrity evidence.
- Replay, substitution, downgrade, wrong-recipient, stale context, and unauthorized delegation SHALL be rejected.
- Secret, credential, and unnecessary Application business data SHALL not appear.

## 5. Owner Decisions

- `APPROVE_FOR_CANARY`
- `APPROVE_FOR_PRODUCTION`
- `APPROVE_WITH_CONDITIONS`
- `REQUEST_CHANGES`
- `REQUEST_MORE_EVIDENCE`
- `DEFER`
- `REJECT`

The interface SHALL explain:

- what is approved;
- exact version and Evidence Set;
- authorized deployment stage;
- conditions and validity;
- whether production is included;
- whether later approval remains required;
- approved rollback conditions.

## 6. Audit and Historical Preservation

The Center SHALL preserve:

- presented package digest;
- Owner identity and assurance;
- decision and reason;
- time and uncertainty;
- scope, conditions, and expiry;
- challenges and corrections;
- subsequent deployment and rollback references;
- immutable history.

Corrections create a new linked record. Historical decisions are never rewritten.

## 7. Separation from Deployment

The Center records approval only.

Release, Runtime, Lifecycle, Security, Authority, and deployment mechanisms SHALL independently verify the decision and its conditions before acting.

## 8. Failure Behavior

If identity, package integrity, Evidence Set, authority, scope, or decision binding is missing or uncertain:

- no approval is established;
- no deployment authority is emitted;
- the case remains pending, invalid, or rejected;
- evidence and an alert are preserved.

## 9. Acceptance Evidence

Acceptance requires tests for:

- authenticated Owner decision;
- wrong identity and unauthorized delegate rejection;
- candidate and Evidence Set substitution rejection;
- silence and timeout producing no approval;
- decision non-transferability;
- condition and stage enforcement;
- immutable history;
- separation from deployment;
- FSA inability to self-approve.
