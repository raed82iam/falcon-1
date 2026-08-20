# FSATS Application Workstream Rules

**Status:** `OWNER-CONTROLLED / MANDATORY WORKSTREAM RULES`  
**Scope:** `FSATS Application workstream`  
**Writable branch for ordinary Application work:** `application-development`  
**Modification authority for this file:** `PROJECT OWNER ONLY`  
**Application worker authority over this file:** `READ-ONLY / NO MODIFY / NO MOVE / NO DELETE / NO RENAME`

## 0. Owner Control and Immutability Rule

This file is a Project Owner-controlled workstream-governance artifact.

Any ChatGPT page, Codex worker, Application worker, automation, implementation agent, review agent, or other Application-side actor SHALL treat this file as read-only.

Application-side work SHALL NOT:

- modify this file;
- rewrite or weaken any rule in this file;
- delete this file;
- move or rename this file;
- replace this file with another workstream-rules artifact;
- bypass this file by creating a conflicting local rule;
- reinterpret another document as authorization to ignore this file.

Only the Project Owner may explicitly authorize a change to this file.

A general instruction to continue FSATS work, modify Application files, reorganize documentation, fix a problem, or update the repository does **not** constitute authorization to change this file.

If a worker believes a rule here is incorrect, outdated, contradictory, or blocking necessary work, it SHALL stop the affected action, report the issue to the Project Owner, and propose a correction. It SHALL NOT edit this file on its own authority.

---

## 1. Start From the Current Governing Sources

Before analysis, planning, proposing, editing, reviewing, or implementing anything:

1. Read `applications/README.md` for the Application workspace boundary and current Application-wide rules.
2. Read `applications/FSATS/README.md` for the current FSATS state, scope, navigation, and authorization boundaries.
3. Review the current Falcon Vision.
4. Review the current Falcon Constitution.
5. Review the current effective Application boundary authorities that directly govern FSATS Application work:
   - `APP-001` — Application Boundary and Lifecycle;
   - `CON-023` — Falcon Application Contract and Manifest;
   - `ADR-I012` — Foundation Plug-and-Play Application Integration Boundary;
   - `ADR-I015` — Falcon OS Application and Awareness Alignment.
6. Do not substitute memory, previous conversation context, assumptions, or historical knowledge for current repository evidence.

These four Application-boundary authorities are mandatory review inputs for any work involving Application identity, lifecycle, manifests, Foundation integration, awareness placement, cross-Application boundaries, ownership, or runtime authority.

If repository evidence and remembered context differ, investigate the difference before proceeding.

---

## 2. Establish Authority Before Designing a Solution

Before thinking about the technical solution, determine:

- Who currently has authority over the subject?
- What stage, Part, Work Package, or scope is active?
- What is closed?
- What is open?
- What is explicitly unauthorized?
- Is there an Owner decision affecting the subject?
- Is there a reopen, clarification, amendment, or closure record that changes the effective state?
- Is implementation authorized, or is the current work design/review only?

No solution may be proposed as executable until its authority basis is understood.

---

## 3. Review the Complete Current Approved Design

Read the complete current approved design relevant to the work.

This includes:

- the directly applicable Part or Work Package artifacts;
- applicable hardening records;
- applicable amendments;
- applicable cross-cutting design artifacts;
- any later accepted artifact that changes or supplements the original scope.

Do not assume that one file alone represents the complete effective design when current authority is distributed across multiple accepted artifacts.

Where scope-specific artifacts and cross-cutting amendments both apply, they SHALL be read together as one effective design set.

---

## 4. Review Applicable Owner Decisions

Review all Owner decisions relevant to the subject, including where applicable:

- `ACCEPTANCE`
- `CHANGES`
- `REOPEN`
- `CLOSURE`
- Owner clarifications
- Owner scope decisions
- Owner corrections
- Owner-authorized amendments

An older acceptance record SHALL NOT override a later valid Owner change, reopen, clarification, amendment, or closure record.

Owner decisions must be interpreted in their actual sequence and scope.

---

## 5. Review the Latest Architecture and Red-Team Evidence

Review the applicable Architecture, Consistency, Technical Verification, and Red-Team evidence.

Priority must be given to the latest valid review performed against the latest semantic version of the affected design.

A previous PASS is not automatically valid after a semantic change.

If bytes or semantics changed after the review that produced the PASS, the previous review SHALL NOT be presented as current evidence for the changed portion.

Required review sequence:

```text
Semantic Change
↓
Fresh Architecture / Consistency Review
↓
Fresh Red-Team Review
↓
Owner Review
```

If Red-Team review causes another semantic modification:

```text
Red-Team Finding
↓
Remediation
↓
New Semantic Version
↓
Fresh Review Again
```

The review cycle continues until the exact version presented to the Owner has valid current review evidence.

---

## 6. Use FSATS V1.3 Only as Historical Design Reference

FSATS V1.3 may be reviewed when useful to understand:

- previous design intent;
- previous functionality;
- previously solved problems;
- lessons learned;
- previous alternatives;
- what changed;
- why it changed.

V1.3 is a design/history reference only.

It does not override:

1. Falcon Vision;
2. Falcon Constitution;
3. current Owner decisions;
4. current approved Falcon architecture;
5. current Foundation authority and contracts;
6. current accepted FSATS design.

A better justified current solution may replace the V1.3 approach, provided the material difference is documented through the governed process.

---

## 7. Verify Current Foundation Dependencies and FCR State

Any dependency on Falcon Foundation must be verified against the current Foundation state.

Before relying on a Foundation capability:

- verify that the required capability actually exists;
- verify its current authority/status;
- verify the applicable Foundation contract or boundary;
- distinguish design/planning acceptance from implementation availability;
- verify relevant FCR state.

If an FCR is currently:

`Waiting On: APPLICATION`

the Application workstream SHALL review and address the required Application-side action before proceeding with work that depends on it.

The Application workstream SHALL NOT:

- invent a Foundation capability that does not exist;
- treat an FCR planning status as proof of implementation;
- repair Foundation code or architecture from the Application branch;
- create a hidden Application-side replacement for a Foundation-owned responsibility.

If a required Foundation capability is missing or incompatible, use the governed FCR process.

### 7.1 Mandatory FCR Check Before Every Response

Before answering the Project Owner on **every FSATS Application-workstream turn**, the Application worker SHALL first check the current FCR state relevant to the Application workstream.

If any FCR is currently waiting on the Application and requires an Application response:

- if the Application worker has enough current, evidence-backed information to answer it, the worker SHALL provide the required Application response through the governed FCR channel before or as part of completing the Owner-facing response;
- if the Application worker does **not** have enough information to answer it safely, the worker SHALL begin its response to the Project Owner by stating exactly what required fact, evidence, decision, implementation state, contract state, or other information is missing;
- the worker SHALL NOT guess, invent, or silently defer an Application-owned FCR response that it is already able to provide.

If no FCR currently requires an Application response, the worker may proceed with the normal response after completing the FCR check.

This rule applies even when the Owner's immediate question is about another FSATS topic.

---

## 8. Analyze Only After the Evidence Set Is Established

Only after the preceding reviews are complete may the workstream perform design analysis or propose a solution.

The analysis must compare, where applicable:

- Falcon Vision;
- Falcon Constitution;
- current approved FSATS design;
- Owner decisions;
- Architecture and Red-Team reviews;
- FSATS V1.3 historical design knowledge;
- current Foundation contracts and boundaries, including `APP-001`, `CON-023`, `ADR-I012`, and `ADR-I015` where applicable;
- current FCR state;
- current repository evidence.

The proposed result must come from this comparison.

Evidence SHALL NOT be selected afterward merely to justify a conclusion that was already assumed.

---

## 9. Organizational Changes Must Not Silently Change Semantics

If the requested change is organizational only:

- do not change document semantics;
- prefer history-preserving moves such as `git mv` where practical;
- preserve document history;
- preserve byte identity where byte preservation is required;
- preserve historical artifacts;
- clearly separate Current from Historical material;
- verify that no unique artifact was lost.

An organizational cleanup SHALL NOT become an undocumented design change.

---

## 10. Semantic Changes Require a Full Review Cycle

A semantic change does not become approved merely because it was written into a file.

Normal lifecycle:

```text
Candidate
↓
Architecture / Consistency Review
↓
Fresh Red-Team Review
↓
Owner Review
↓
Owner Acceptance
```

If the Owner requests a change after a previous review, the previous PASS is no longer sufficient for the changed scope.

Required sequence:

```text
Owner Requested Change
↓
Apply Change
↓
Fresh Architecture / Consistency Review
↓
Fresh Red-Team Review
↓
Report Results to Owner
↓
Owner Final Decision
```

An Owner statement such as `Approved with change X` does not automatically mean that the modified version is finally accepted.

The requested change must first be applied and the required fresh review cycle completed before final acceptance is requested.

---

## 11. Technical PASS Is Not Owner Acceptance

These states are different:

```text
Build PASS
≠
Architecture PASS
≠
Red-Team PASS
≠
Owner Acceptance
≠
Closure
```

The workstream SHALL NOT state:

- `OWNER_ACCEPTED`
- `OWNER_ACCEPTED_AND_CLOSED`
- `CLOSED`
- or an equivalent final Owner state

unless the Project Owner has explicitly granted that state.

Technical validation can establish technical readiness. It cannot manufacture Owner authority.

---

## 12. Never Expand Scope Without Authorization

The workstream must remain inside the current authorized scope.

Examples:

- If the authorized work is P0-K, do not start P0-L.
- If implementation is not authorized, do not write implementation code.
- If the request is review-only, do not modify the repository.
- If the request is planning-only, do not convert the plan into implementation.
- If a closed artifact contains a possible problem, report it before reopening or changing it.
- Do not treat completion of one scope as automatic authorization for the next scope.

When authority for the next action is unclear:

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
EXPAND SCOPE
↓
COMMIT
```

---

## 13. Historical Records Must Not Be Rewritten to Repair the Past

Historical records must remain preserved even when they contain obsolete, superseded, incomplete, or later-corrected information.

Do not erase or silently rewrite history merely to make the repository appear cleaner.

If correction is necessary:

```text
Historical Record
↓
Preserve
+
New Controlling Correction / Amendment / Supersession Record
```

The current authority must clearly identify what supersedes or corrects the historical state.

---

## 14. Every Material Claim Must Be Evidence-Backed

Before presenting a material statement as fact, ask:

**“Which current file, Owner decision, review record, contract, commit, or other authoritative evidence proves this?”**

If no clear evidence exists:

- do not present the statement as established fact;
- identify it as an assumption, inference, unresolved question, or proposed interpretation;
- investigate further when required.

Statements about current status, authority, closure, implementation availability, dependencies, or approval require especially strong evidence.

---

## 15. Mandatory Continuity Check Before Starting New Work

Before beginning a new Part, Work Package, major review cycle, or continuation session, perform a fresh continuity check.

Minimum sequence:

```text
Current GitHub State
↓
applications/README.md
↓
applications/FSATS/README.md
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
Current Approved Design
↓
Applicable Owner Decisions
↓
Latest Applicable Reviews
↓
Current Foundation / FCR State
↓
Determine Exact Authorized Next Work
```

Do not start from conversation memory alone.

Do not assume that the state observed in a previous session remains current.

---

## 16. Repository Write Discipline

A repository write must correspond to an authorized action.

Before writing:

- determine the authorized branch;
- verify the current branch state;
- verify the exact requested scope;
- determine whether the action is organizational, semantic, review, or implementation;
- ensure no unrelated scope is modified.

After writing:

- inspect the actual diff;
- verify no unintended files changed;
- perform the required technical/review checks;
- report what changed;
- do not silently promote the resulting state beyond its actual authority.

A successful commit is evidence that Git accepted bytes. It is not evidence that Falcon accepted the decision.

---

# Prime Operating Rule

The FSATS Application workstream SHALL always follow:

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
