# Shared Falcon Web Application Workstream Rules

**Status:** `OWNER-CONTROLLED / MANDATORY WORKSTREAM RULES`  
**Scope:** `Shared Falcon Web Application workstream`  
**Writable branch:** `web-development`  
**Authorized writable subtree:** `applications/shared/web/**`  
**Modification authority for this file:** `PROJECT OWNER ONLY`  
**Web worker authority over this file:** `READ-ONLY / NO MODIFY / NO MOVE / NO DELETE / NO RENAME`

## 0. Owner Control and Immutability Rule

This file is a Project Owner-controlled workstream-governance artifact.

Any ChatGPT page, Codex worker, Web worker, automation, implementation agent, review agent, or other Web-side actor SHALL treat this file as read-only.

Web-side work SHALL NOT:

- modify this file;
- rewrite or weaken any rule in this file;
- delete this file;
- move or rename this file;
- replace it with another workstream-rules artifact;
- bypass it by creating a conflicting local rule;
- reinterpret another document as authority to ignore it.

Only the Project Owner may explicitly authorize a change to this file.

If a worker believes a rule here is incorrect, outdated, contradictory, or blocking necessary work, it SHALL stop the affected action, report the issue to the Project Owner, and propose a correction. It SHALL NOT edit this file on its own authority.

---

## 1. Mandatory Fresh Source Review

Before analysis, planning, proposing, editing, reviewing, or implementing anything, read current repository evidence and do not substitute memory for current files.

At minimum review, where applicable:

1. Falcon Vision.
2. Falcon Constitution.
3. `applications/README.md`.
4. `applications/shared/web/README.md`.
5. Current effective `APP-001`.
6. Current effective `CON-023`.
7. Current effective `ADR-I012`.
8. Current effective `ADR-I015`.
9. Applicable Foundation contracts and current Foundation authority.
10. Applicable Application/Shared Application ownership decisions.
11. Current FCR state.
12. Current approved Web design artifacts once they exist.
13. Applicable Owner decisions and latest valid reviews.

If current repository evidence and remembered context differ, investigate the difference before proceeding.

---

## 2. Establish Authority Before Solution Design

Before thinking about a solution, determine:

- who owns the capability;
- what is Shared Web-owned;
- what is Foundation-owned;
- what is Application/domain-owned;
- what is approved;
- what is candidate only;
- what is implementation-authorized;
- what is explicitly unauthorized;
- whether an Owner decision or FCR constrains the work.

No technical convenience creates architectural authority.

---

## 3. Strict Repository Write Boundary

The Shared Web workstream MAY write only on:

`web-development`

and only within:

`applications/shared/web/**`

unless the Project Owner explicitly authorizes a broader change.

The Web workstream SHALL NOT write to:

- `foundation-development`;
- `application-development`;
- `main`;
- `reference/fsats-v1.3-scratch`;
- Foundation-owned source/tests/docs;
- FSATS-owned files;
- other Application-owned files;
- repository control-plane files outside its authorized subtree.

The Web workstream MAY read the current Foundation and Application branches/references necessary to understand contracts, ownership, integration, and current authority.

Read access does not create write authority.

---

## 4. Shared Versus Domain-Specific Ownership Rule

Use this classification:

```text
Generic + intentionally reusable across Falcon
→ Shared Falcon Web Application

Primarily domain-specific
→ owning Falcon Application
```

Shared Web SHALL NOT absorb domain-specific business ownership merely because that behavior is displayed in a browser.

Examples of domain-owned behavior include trading strategy decisions, risk decisions, broker/provider control, market logic, accounting business rules, or any other Application-specific business truth.

Shared Web may provide reusable presentation/infrastructure components through governed contracts without owning the underlying domain semantics.

---

## 5. Web Is Not a Source of Business Authority

Presentation does not create authority.

A visible value does not become authoritative merely because Web displays it.

A button click does not itself prove permission, business completion, execution success, trade success, Foundation admission, lifecycle completion, or any other governed state.

Shared Web must consume and present authoritative results from their owning Application/Foundation boundaries.

---

## 6. Mandatory FCR Check Before Every Response

Before answering the Project Owner on **every Shared Web workstream turn**, the Web worker SHALL first check the current repository-wide FCR state.

If an FCR is `Waiting On: WEB` and requires a Web response:

- if the Web worker has enough current, evidence-backed information to answer it, the worker SHALL provide the required Web response through the governed FCR channel before or as part of completing the Owner-facing response;
- if the Web worker does not have enough information, the worker SHALL begin its response to the Project Owner by stating exactly what required fact, evidence, decision, contract state, implementation state, or authority is missing;
- the worker SHALL NOT guess, invent, or silently defer a Web-owned FCR response it is already able to provide.

If an FCR is `Waiting On: APPLICATION`, the Web workstream SHALL NOT answer an Application-owned business/domain decision on behalf of the Application.

If an FCR is `Waiting On: FOUNDATION`, the Web workstream SHALL NOT answer on behalf of Foundation.

If an FCR is `Waiting On: OWNER`, no workstream may substitute its own decision for the Project Owner.

Permitted handoff values are:

- `FOUNDATION`
- `APPLICATION`
- `WEB`
- `OWNER`
- `NONE`

FCR participation is coordination authority only. It never grants cross-workstream file-write authority.

---

## 7. Cross-Workstream Change Rule

When Shared Web needs something from another owner:

```text
Web identifies need
↓
FCR / governed handoff
↓
Waiting On: FOUNDATION or APPLICATION
↓
Owning workstream acts in its own branch/files
↓
Evidence returned through FCR
↓
Web verifies/consumes
```

Shared Web SHALL NOT directly repair or implement the other workstream's responsibility.

Likewise, other workstreams SHALL NOT modify Shared Web-owned files to resolve a Web-owned action without explicit Owner authorization.

---

## 8. Full Evidence Set Before Analysis

Only after source and authority review may the workstream analyze or propose a solution.

Compare, where applicable:

- Falcon Vision;
- Falcon Constitution;
- APP-001;
- CON-023;
- ADR-I012;
- ADR-I015;
- current Foundation contracts;
- current Application ownership/design evidence;
- current Shared Web design;
- applicable Owner decisions;
- latest Architecture/Consistency/Security/Red-Team reviews;
- current FCR state;
- current repository evidence.

Evidence SHALL NOT be selected afterward merely to justify a conclusion already assumed.

---

## 9. Planning and Implementation Are Different Authorities

Architecture study, UX planning, visual design, component planning, or repository organization do not themselves authorize implementation or runtime activation.

If work is planning-only, do not implement.

If work is review-only, do not modify repository content.

If implementation is not authorized, do not create production code merely because a design appears complete.

---

## 10. Semantic Change Review Cycle

A semantic change does not become approved merely because it was written.

Normal lifecycle:

```text
Candidate
↓
Architecture / Consistency Review
↓
Security / Red-Team where applicable
↓
Validation
↓
Owner Review
↓
Owner Acceptance
```

If any review causes another semantic change, the modified version requires a fresh applicable review cycle.

An Owner statement such as `approved with change X` does not automatically mean the modified version is finally accepted. Apply the change, re-review the changed version, report the result, then obtain final Owner decision.

---

## 11. PASS Is Not Owner Acceptance

```text
Build PASS
≠
Component Test PASS
≠
Accessibility PASS
≠
Architecture PASS
≠
Security PASS
≠
Red-Team PASS
≠
OWNER_ACCEPTED
≠
CLOSED
```

The Web workstream SHALL NOT state `OWNER_ACCEPTED`, `OWNER_ACCEPTED_AND_CLOSED`, `CLOSED`, or equivalent final Owner states unless the Project Owner explicitly grants that state.

---

## 12. Never Expand Scope Without Authorization

Do not expand from Shared Web into Foundation, FSATS, or another Application.

Do not start a later phase merely because an earlier phase looks complete.

Do not add deployment, external connectivity, authentication authority, domain business logic, or runtime integration unless separately authorized.

When authority is unclear:

```text
STOP
↓
REPORT
↓
OWNER DECISION
```

Not:

```text
ASSUME
↓
BUILD
↓
JUSTIFY LATER
```

---

## 13. Historical Records and Organizational Changes

Organizational cleanup SHALL NOT silently alter semantics.

Preserve historical records, superseded decisions, and prior evidence. Do not rewrite history to make the repository appear cleaner.

If a controlling correction is needed, preserve the historical artifact and add the governed correction/supersession record.

Prefer history-preserving moves when reorganizing approved artifacts.

---

## 14. Every Material Claim Must Be Evidence-Backed

Before presenting a material statement as fact, ask:

**Which current artifact proves this?**

If no clear current evidence exists, identify the statement as an assumption, inference, unresolved question, or proposal rather than established fact.

Claims about ownership, authority, approval, implementation availability, Foundation capability, Application semantics, security, closure, or FCR state require especially strong evidence.

---

## 15. Mandatory Continuity Check Before New Work

Before beginning a new phase, major design area, implementation package, major review cycle, or continuation session, perform a fresh continuity check:

```text
Current GitHub State
↓
applications/README.md
↓
applications/shared/web/README.md
↓
Falcon Vision
↓
Falcon Constitution
↓
APP-001
↓
CON-023
↓
ADR-I012
↓
ADR-I015
↓
Applicable Foundation Contracts
↓
Applicable Application Ownership/Design Evidence
↓
Current Shared Web Approved Design
↓
Applicable Owner Decisions
↓
Latest Applicable Reviews
↓
Current FCR State
↓
Determine Exact Authorized Next Work
```

Do not start from conversation memory alone.

---

## 16. Repository Write Discipline

Before any write:

- confirm `web-development`;
- confirm the target is inside `applications/shared/web/**`;
- confirm the exact authorized scope;
- determine whether the action is organizational, semantic, review, design, or implementation;
- ensure no unrelated file is changed.

After any write:

- inspect the actual diff;
- verify no unintended files changed;
- run the required applicable checks;
- report what changed;
- do not silently promote the result beyond its actual authority.

A successful commit proves only that Git accepted bytes. It does not prove Falcon or the Project Owner accepted the decision.

---

# Prime Operating Rule

```text
SOURCE
↓
AUTHORITY
↓
COMPARE
↓
DECIDE
↓
CHANGE
```

Never:

```text
MEMORY
↓
ASSUMPTION
↓
DECISION
↓
SEARCH FOR SUPPORTING EVIDENCE
```

**Source first. Authority second. Compare third. Decide fourth. Change last.**
